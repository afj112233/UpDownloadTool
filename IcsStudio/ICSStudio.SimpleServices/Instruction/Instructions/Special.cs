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
    //prescan:See FBC Flow Chart (Prescan)
    internal class FBCInstruction : FixedInstruction
    {
        public FBCInstruction(ParamInfoList infos) : base("FBC", infos)
        {

        }


        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            parameters.RemoveRange(parameters.Count - 2, 2);
            parameters.RemoveRange(parameters.Count - 3, 2);

            return Utils.ParseExprList(parameters);
        }
    }

    //prescan:See DDT Flow Chart (Prescan)
    internal class DDTInstruction : FixedInstruction
    {
        public DDTInstruction(ParamInfoList infos) : base("DDT", infos)
        {

        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            parameters.RemoveRange(parameters.Count - 2, 2);
            parameters.RemoveRange(parameters.Count - 3, 2);

            return Utils.ParseExprList(parameters);
        }
    }

    //prescan:The Reference = Source AND Mask.
    internal class DTRInstruction : RLLInstruction
    {
        public DTRInstruction(ParamInfoList infos) : base("DTR")
        {

        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            var res=paramList.Accept(checker) as ASTNodeList;
            res.nodes[2]=new ASTNameAddr(res.nodes[2] as ASTName);
            return res;
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var source = paramList.nodes[0];
            var mask = paramList.nodes[1];
            var reference = paramList.nodes[2] as ASTNameAddr;
            mask.Accept(gen);
            source.Accept(gen);
            gen.masm().And(CodeGenVisitor.GetPrimitiveType((source as ASTExpr).type));
            reference.Accept(gen);
            gen.masm().Swap();
            gen.masm().Store(reference.ref_type.type, DINT.Inst);
        }

        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            var source = paramList.nodes[0];
            var mask = paramList.nodes[1];
            var reference = paramList.nodes[2] as ASTNameAddr;
            gen.masm().Pop();
            mask.Accept(gen);
            source.Accept(gen);
            gen.masm().And(CodeGenVisitor.GetPrimitiveType((source as ASTExpr).type));
            gen.masm().Dup();
            reference.Accept(gen);
            gen.masm().Dup();
            gen.masm().Load(reference.ref_type.type);
            gen.masm().Swap();
            gen.masm().SwapX1();
            gen.masm().Store(reference.ref_type.type, DINT.Inst);
            gen.masm().Cmp(MacroAssembler.PrimitiveType.DINT);
            gen.masm().Ne(DINT.Inst);
        }
    }

}
