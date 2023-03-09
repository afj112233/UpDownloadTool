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
    using System.Diagnostics;
    using ParamInfo = Tuple<Predicate<ASTExpr>, IDataType, ParameterType>;
    using ParamInfoList = List<Tuple<Predicate<ASTExpr>, IDataType, ParameterType>>;

    internal class InputOutputUtils
    {
        public static string ConvertExprToString(ASTNode expr)
        {
            if (expr is ASTEmpty)
                return "";
            if (expr is ASTName)
            {
                return ((expr as ASTName).id_list.nodes[0] as ASTNameItem).id;
            }
            else
            {
                Debug.Assert(false);
                return "";
            }
        }
    }

    internal class GSVInstruction : IXInstruction
    {
        public GSVInstruction()
        {
            Name = "GSV";
        }
        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.GSV.ParseSTParameters(parameters); ;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.GSV.ParseRLLParameters(parameters);
        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            var res = new ASTNodeList();
            res.AddNode(paramList.nodes[0].Accept(checker));
            var param1 = paramList.nodes[1];
            res.AddNode(new ASTExprConstString(InputOutputUtils.ConvertExprToString(param1)) { ContextStart = param1.ContextStart, ContextEnd = param1.ContextEnd });
            res.AddNode(paramList.nodes[2].Accept(checker));
            res.AddNode(paramList.nodes[3].Accept(checker));
            return res;
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {

            paramList.nodes[0].Accept(gen);
            paramList.nodes[1].Accept(gen);
            paramList.nodes[2].Accept(gen);
            gen.GenCopyParameter((paramList.nodes[3] as ASTName).Expr);
            gen.masm().CallName("GSV", 6);
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            gen.masm().BiPush(0);

        }


    }

    //prescan:The .EWS, .ST, .DN, and .ER bits are cleared.
    internal class MSGInstruction : FixedTokenInstruction
    {
        public MSGInstruction(ParamInfoList infos) : base("MSG", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[1] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == MESSAGE.Inst);
            addr.Accept(gen);

            var ew = MESSAGE.Inst["EW"];
            Utils.GenClearBit(gen.masm(), ew);

            var er = MESSAGE.Inst["ER"];
            Utils.GenClearBit(gen.masm(), er);

            var dn = MESSAGE.Inst["DN"];
            Utils.GenClearBit(gen.masm(), dn);

            var st = MESSAGE.Inst["ST"];
            Utils.GenClearBit(gen.masm(), st);

            var en = MESSAGE.Inst["EN"];
            Utils.GenClearBit(gen.masm(), en);

            var to = MESSAGE.Inst["TO"];
            Utils.GenClearBit(gen.masm(), to);

            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Message Control", MESSAGE.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }
    }

    internal class SSVInstruction : IXInstruction
    {
        public SSVInstruction()
        {
            Name = "SSV";
        }
        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.SSV.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.SSV.ParseRLLParameters(parameters);
        }


        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            var res = new ASTNodeList();
            res.AddNode(paramList.nodes[0].Accept(checker));
            var param1 = paramList.nodes[1];
            res.AddNode(new ASTExprConstString(InputOutputUtils.ConvertExprToString(param1)) {ContextStart = param1.ContextStart,ContextEnd = param1.ContextEnd});
            res.AddNode(paramList.nodes[2].Accept(checker));
            res.AddNode(paramList.nodes[3].Accept(checker));
            return res;
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {

            paramList.nodes[0].Accept(gen);
            paramList.nodes[1].Accept(gen);
            paramList.nodes[2].Accept(gen);
            gen.GenCopyParameter((paramList.nodes[3] as ASTName).Expr);
            gen.masm().CallName("SSV", 6);
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            gen.masm().BiPush(0);

        }
    }

    internal class IOTInstruction : FixedInstruction
    {
        public IOTInstruction(ParamInfoList infos) : base("IOT", infos)
        {

        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Update Tag", new ExceptType(BOOL.Inst));
            return new List<Tuple<string, IDataType>>() {param1};
        }
    }

}
