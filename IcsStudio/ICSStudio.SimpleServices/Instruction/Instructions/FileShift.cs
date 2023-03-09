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
    internal class BSLInstruction : FixedInstruction
    {
        public BSLInstruction(ParamInfoList infos) : base("BSL", infos)
        {

        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            parameters.RemoveAt(parameters.Count - 1);
            return Utils.ParseExprList(parameters);
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            Console.WriteLine($"{paramList}");
            var addr = paramList.nodes[4] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == CONTROL.Inst);
            addr.Accept(gen);
            var en = CONTROL.Inst["EN"];
            var dn = CONTROL.Inst["DN"];
            var er = CONTROL.Inst["ER"];
            var pos = CONTROL.Inst["POS"];
            Utils.GenClearBit(gen.masm(), en);
            Utils.GenClearBit(gen.masm(), dn);
            Utils.GenClearBit(gen.masm(), er);
            
            gen.masm().BiPush(0);
            gen.masm().Store(pos.DataTypeInfo.DataType, DINT.Inst);
        }
    }
    internal class BSRInstruction : FixedInstruction
    {
        public BSRInstruction(ParamInfoList infos) : base("BSR", infos)
        {

        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            parameters.RemoveAt(parameters.Count - 1);
            return Utils.ParseExprList(parameters);
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[4] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == CONTROL.Inst);
            addr.Accept(gen);
            var en = CONTROL.Inst["EN"];
            var dn = CONTROL.Inst["DN"];
            var er = CONTROL.Inst["ER"];
            var pos = CONTROL.Inst["POS"];
            Utils.GenClearBit(gen.masm(), en);
            Utils.GenClearBit(gen.masm(), dn);
            Utils.GenClearBit(gen.masm(), er);
            Utils.GenClearBit(gen.masm(), pos);
            gen.masm().Pop();
        }
    }
    //prescan:see FFL Flow chart
    internal class FFLInstruction : FixedInstruction
    {
        public FFLInstruction(ParamInfoList infos) : base("FFL", infos)
        {

        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            parameters.RemoveRange(parameters.Count - 2, 2);

            return Utils.ParseExprList(parameters);
        }
    }
    //prescan:see FFU Flow chart
    internal class FFUInstruction : FixedInstruction
    {
        public FFUInstruction(ParamInfoList infos) : base("FFU", infos)
        {

        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            parameters.RemoveRange(parameters.Count - 2, 2);

            return Utils.ParseExprList(parameters);
        }
    }
    //prescan:see LFL Flow chart
    internal class LFLInstruction : FixedInstruction
    {
        public LFLInstruction(ParamInfoList infos) : base("LFL", infos)
        {

        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            parameters.RemoveRange(parameters.Count - 2, 2);
            return Utils.ParseExprList(parameters);
        }
    }
    //prescan:see LFU Flow chart
    internal class LFUInstruction : FixedInstruction
    {
        public LFUInstruction(ParamInfoList infos) : base("LFU", infos)
        {

        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            parameters.RemoveRange(parameters.Count - 2, 2);
            return Utils.ParseExprList(parameters);
        }
    }
}
