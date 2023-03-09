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

    internal class HPFInstruction : FixedFBDInstruction
    {
        public HPFInstruction(ParamInfoList infos) : base("HPF", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) &&
                         (addr.type as RefType).type == FILTER_HIGH_PASS.Inst);
            addr.Accept(gen);
            var enableOut = FILTER_HIGH_PASS.Inst["EnableOut"];
            var enableIn = FILTER_HIGH_PASS.Inst["EnableIn"];
            var firstScan = FILTER_HIGH_PASS.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("HPFTag", FILTER_HIGH_PASS.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => FILTER_HIGH_PASS.Inst["EnableIn"];
        protected override string InstrName => "HPF";
    }

    internal class LPFInstruction : FixedFBDInstruction
    {
        public LPFInstruction(ParamInfoList infos) : base("LPF", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == FILTER_LOW_PASS.Inst);
            addr.Accept(gen);
            var enableOut = FILTER_LOW_PASS.Inst["EnableOut"];
            var enableIn = FILTER_LOW_PASS.Inst["EnableIn"];
            var firstScan = FILTER_LOW_PASS.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("LPFTag", FILTER_LOW_PASS.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => FILTER_LOW_PASS.Inst["EnableIn"];
        protected override string InstrName => "LPF";
    }

    internal class NTCHInstruction : FixedFBDInstruction
    {
        public NTCHInstruction(ParamInfoList infos) : base("NTCH", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == FILTER_NOTCH.Inst);
            addr.Accept(gen);
            var enableOut = FILTER_NOTCH.Inst["EnableOut"];
            var enableIn = FILTER_NOTCH.Inst["EnableIn"];
            var firstScan = FILTER_NOTCH.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("NTCHTag", FILTER_NOTCH.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => FILTER_NOTCH.Inst["EnableIn"];
        protected override string InstrName => "NTCH";
    }

    internal class LDL2Instruction : FixedFBDInstruction
    {
        public LDL2Instruction(ParamInfoList infos) : base("LDL2", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) &&
                         (addr.type as RefType).type == LEAD_LAG_SEC_ORDER.Inst);
            addr.Accept(gen);
            var enableOut = LEAD_LAG_SEC_ORDER.Inst["EnableOut"];
            var enableIn = LEAD_LAG_SEC_ORDER.Inst["EnableIn"];
            var firstScan = LEAD_LAG_SEC_ORDER.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("LDL2Tag", LEAD_LAG_SEC_ORDER.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => LEAD_LAG_SEC_ORDER.Inst["EnableIn"];
        protected override string InstrName => "LDL2";
    }

    internal class DERVInstruction : FixedFBDInstruction
    {
        public DERVInstruction(ParamInfoList infos) : base("DERV", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == DERIVATIVE.Inst);
            addr.Accept(gen);
            var enableOut = DERIVATIVE.Inst["EnableOut"];
            var enableIn = DERIVATIVE.Inst["EnableIn"];
            var firstScan = DERIVATIVE.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("DERVTag", DERIVATIVE.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => DERIVATIVE.Inst["EnableIn"];
        protected override string InstrName => "DERV";
    }
}