using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    internal class SQIInstruction : FixedInstruction
    {
        public SQIInstruction(ParamInfoList infos) : base("SQI", infos)
        {

        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            parameters.RemoveRange(parameters.Count - 2, 2);
            return Utils.ParseExprList(parameters);
        }
    }
    internal class SQOInstruction : FixedInstruction
    {
        public SQOInstruction(ParamInfoList infos) : base("SQO", infos)
        {


        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            parameters.RemoveRange(parameters.Count - 2, 2);
            return Utils.ParseExprList(parameters);
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[6] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == CONTROL.Inst);
            addr.Accept(gen);
            var en = CONTROL.Inst["EN"];
            Utils.GenSetBit(gen.masm(), en);
            gen.masm().Pop();
        }
    }
    internal class SQLInstruction : FixedInstruction
    {
        public SQLInstruction(ParamInfoList infos) : base("SQL", infos)
        {

        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            parameters.RemoveRange(parameters.Count - 2, 2);
            return Utils.ParseExprList(parameters);
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[5] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == CONTROL.Inst);
            addr.Accept(gen);
            var en = CONTROL.Inst["EN"];
            Utils.GenSetBit(gen.masm(), en);
            gen.masm().Pop();
        }
    }
}
