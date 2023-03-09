using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.PredefinedType;

namespace ICSStudio.SimpleServices.Instruction.Instructions
{
    using ParamInfo = Tuple<Predicate<ASTExpr>, IDataType, ParameterType>;
    using ParamInfoList = List<Tuple<Predicate<ASTExpr>, IDataType, ParameterType>>;

    //prescan:The instruction will prescan the named subroutine if it has never been prescanned before.
    internal class FORInstruction : RLLInstruction
    {
        public FORInstruction(ParamInfoList infos) : base("FOR")
        {

        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            var res = paramList;
            res.nodes[1]=new ASTNameAddr(res.nodes[1].Accept(checker) as ASTName);
            res.nodes[2] = res.nodes[2].Accept(checker);
            res.nodes[3] = res.nodes[3].Accept(checker);
            res.nodes[4] = res.nodes[4].Accept(checker);
            return res;
        }

        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            var name = paramList.nodes[0] as ASTName;
            var index = paramList.nodes[1] as ASTNameAddr;
            var initial = paramList.nodes[2] as ASTExpr;
            var terminal = paramList.nodes[3] as ASTExpr;
            var step = paramList.nodes[4] as ASTExpr;
            //增加for循环计数器
            gen.masm().CLoadInteger(0);
            gen.masm().CallName("SYSOP", 1);
            gen.masm().Pop();

            var exit_label = new MacroAssembler.Label();
            index.Accept(gen);
            initial.Accept(gen);
            gen.masm().Store(index.ref_type.type, initial.type);
            gen.RepeatAction(() =>
            {
                gen.masm().RawCallRoutine((name.id_list.nodes[0] as ASTNameItem).id + ".Logic", 0);
                gen.masm().Pop();
               
                gen.masm().CLoadInteger(0);
                
                gen.masm().CallName("SYSSTATUS", 1);
                gen.masm().JneL(exit_label);

                index.Accept(gen);
                gen.masm().Dup();
                gen.masm().Load(index.ref_type.type);
                step.Accept(gen);
                gen.masm().Add(CodeGenVisitor.GetPrimitiveType(index.ref_type.type));
                gen.masm().Store(index.ref_type.type, DINT.Inst);

                gen.masm().ECheck();              
            },initial,terminal,step, exit_label);

            //如果有BRK，则清除BRK状态
            gen.masm().CLoadInteger(2);
            gen.masm().CallName("SYSOP", 1);
            gen.masm().Pop();

            //检查是不是有别的错误，有的话返回
            gen.masm().ECheck();

            //减少for循环计数器
            gen.masm().CLoadInteger(1);
            gen.masm().CallName("SYSOP", 1);
            gen.masm().Pop();

        }
    }
    internal class BRKInstruction : RLLInstruction
    {
        public BRKInstruction(ParamInfoList infos) : base("BRK")
        {

        }

        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            gen.masm().BiPush(2);
            gen.masm().BiPush(100);
            gen.masm().Throw();
        }
    }

}
