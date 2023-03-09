using System;
using System.Collections.Generic;
using System.Diagnostics;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.PredefinedType;

namespace ICSStudio.SimpleServices.Instruction.Instructions
{
    using ParamInfoList = List<Tuple<Predicate<ASTExpr>, IDataType, ParameterType>>;

    internal class ALMInstruction : FixedFBDInstruction
    {
        public ALMInstruction(ParamInfoList infos) : base("ALM", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var alm = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(alm != null && (alm.type is RefType) && (alm.type as RefType).type == ALARM.Inst);
            paramList.nodes[0].Accept(gen);
            var enable = ALARM.Inst["EnableIn"];
            Utils.GenClearBit(gen.masm(), enable);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("ALMTag", ALARM.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => SCALE.Inst["EnableIn"];
        protected override string InstrName => "ALM";

    }

    internal class SCLInstruction : FixedFBDInstruction
    {
        public SCLInstruction(ParamInfoList infos) : base("SCL", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == SCALE.Inst);
            addr.Accept(gen);
            var enableOut = SCALE.Inst["EnableOut"];
            var enableIn = SCALE.Inst["EnableIn"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("SCLTag", SCALE.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => SCALE.Inst["EnableIn"];
        protected override string InstrName => "SCL";
    }

    //prescan:no clear information
    internal class PIDInstruction : FixedInstruction
    {
        public PIDInstruction(ParamInfoList infos) : base("PID", infos)
        {

        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("PID", PID.Inst);
            var param2 = new Tuple<string, IDataType>("Process Variable",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param3 =
                new Tuple<string, IDataType>("Tieback", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param4 = new Tuple<string, IDataType>("Control Variable",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param5 = new Tuple<string, IDataType>("PID Master Loop", PID.Inst);
            var param6 = new Tuple<string, IDataType>("Inhold Bit", BOOL.Inst);
            var param7 = new Tuple<string, IDataType>("Inhold Value",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            return new List<Tuple<string, IDataType>>() {param1, param2, param3, param4, param5, param6, param7};
        }
    }

    internal class PIDEInstruction : FixedFBDInstruction
    {
        public PIDEInstruction(ParamInfoList infos) : base("PIDE", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == PID_ENHANCED.Inst);
            addr.Accept(gen);
            var enableOut = PID_ENHANCED.Inst["EnableOut"];
            var enableIn = PID_ENHANCED.Inst["EnableIn"];
            var firstScan = PID_ENHANCED.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("PIDETag", PID_ENHANCED.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => PID_ENHANCED.Inst["EnableIn"];
        protected override string InstrName => "PIDE";
    }

    internal class FPIDEInstruction : FixedInstruction
    {
        public FPIDEInstruction(ParamInfoList infos) : base("PIDE", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == PID_ENHANCED.Inst);
            addr.Accept(gen);
            var enableOut = PID_ENHANCED.Inst["EnableOut"];
            var enableIn = PID_ENHANCED.Inst["EnableIn"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            gen.masm().Pop();
        }
    }

    internal class IMCInstruction : FixedFBDInstruction
    {
        public IMCInstruction(ParamInfoList infos) : base("IMC", infos)
        {
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == IMC.Inst);
            addr.Accept(gen);
            var enableOut = IMC.Inst["EnableOut"];
            var enableIn = IMC.Inst["EnableIn"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("IMCTag", IMC.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => IMC.Inst["EnableIn"];
        protected override string InstrName => "FBDIMC";
    }

    internal class CCInstruction : FixedFBDInstruction
    {
        public CCInstruction(ParamInfoList infos) : base("CC", infos)
        {
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == CC.Inst);
            addr.Accept(gen);
            var enableOut = CC.Inst["EnableOut"];
            var enableIn = CC.Inst["EnableIn"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("CCTag", CC.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => CC.Inst["EnableIn"];
        protected override string InstrName => "FBDCC";
    }

    internal class MMCInstruction : FixedFBDInstruction
    {
        public MMCInstruction(ParamInfoList infos) : base("MMC", infos)
        {
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == MMC.Inst);
            addr.Accept(gen);
            var enableOut = MMC.Inst["EnableOut"];
            var enableIn = MMC.Inst["EnableIn"];
            var firstScan = MMC.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("MMCTag", MMC.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => MMC.Inst["EnableIn"];
        protected override string InstrName => "FBDMMC";
    }

    internal class RMPSInstruction : FixedInstruction
    {
        public RMPSInstruction(ParamInfoList infos) : base("RMPS", infos)
        {
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == RAMP_SOAK.Inst);
            addr.Accept(gen);
            var enableOut = RAMP_SOAK.Inst["EnableOut"];
            var enableIn = RAMP_SOAK.Inst["EnableIn"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("RMPSTag", RAMP_SOAK.Inst);
            var param2 = new Tuple<string, IDataType>("RampValue", new ArrayTypeNormal(REAL.Inst));
            var param3 = new Tuple<string, IDataType>("SoakValue", new ArrayTypeNormal(REAL.Inst));
            var param4 = new Tuple<string, IDataType>("SoakTime", new ArrayTypeNormal(REAL.Inst));
            return new List<Tuple<string, IDataType>>() {param1, param2, param3, param4};
        }
    }

    internal class POSPInstruction : FixedFBDInstruction
    {
        public POSPInstruction(ParamInfoList infos) : base("POSP", infos)
        {
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == POSITION_PROP.Inst);
            addr.Accept(gen);
            var enableOut = POSITION_PROP.Inst["EnableOut"];
            var enableIn = POSITION_PROP.Inst["EnableIn"];
            var firstScan = POSITION_PROP.Inst["firstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("POSPTag", POSITION_PROP.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => POSITION_PROP.Inst["EnableIn"];
        protected override string InstrName => "POSP";
    }

    internal class SRTPInstruction : FixedFBDInstruction
    {
        public SRTPInstruction(ParamInfoList infos) : base("SRTP", infos)
        {
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == SPLIT_RANGE.Inst);
            addr.Accept(gen);
            var enableOut = SPLIT_RANGE.Inst["EnableOut"];
            var enableIn = SPLIT_RANGE.Inst["EnableIn"];
            var firstScan = SPLIT_RANGE.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("SRTPTag", SPLIT_RANGE.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => SPLIT_RANGE.Inst["EnableIn"];
        protected override string InstrName => "SRTP";
    }

    internal class LDLGInstruction : FixedFBDInstruction
    {
        public LDLGInstruction(ParamInfoList infos) : base("LDLG", infos)
        {
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == LEAD_LAG.Inst);
            addr.Accept(gen);
            var enableOut = LEAD_LAG.Inst["EnableOut"];
            var enableIn = LEAD_LAG.Inst["EnableIn"];
            var firstscan = LEAD_LAG.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstscan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("LDLGTag", LEAD_LAG.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => LEAD_LAG.Inst["EnableIn"];
        protected override string InstrName => "LDLG";
    }

    internal class FGENInstruction : FixedInstruction
    {
        public FGENInstruction(ParamInfoList infos) : base("FGEN", infos)
        {
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) &&
                         (addr.type as RefType).type == FUNCTION_GENERATOR.Inst);
            addr.Accept(gen);
            var enableOut = FUNCTION_GENERATOR.Inst["EnableOut"];
            var enableIn = FUNCTION_GENERATOR.Inst["EnableIn"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("FGENTag", FUNCTION_GENERATOR.Inst);
            var param2 = new Tuple<string, IDataType>("X1", new ArrayTypeNormal(REAL.Inst));
            var param3 = new Tuple<string, IDataType>("Y1", new ArrayTypeNormal(REAL.Inst));
            var param4 = new Tuple<string, IDataType>("X2", new ArrayTypeNormal(REAL.Inst));
            var param5 = new Tuple<string, IDataType>("Y2", new ArrayTypeNormal(REAL.Inst));
            return new List<Tuple<string, IDataType>>() {param1, param2, param3, param4, param5};
        }
    }

    internal class TOTInstruction : FixedFBDInstruction
    {
        public TOTInstruction(ParamInfoList infos) : base("TOT", infos)
        {
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == TOTALIZER.Inst);
            addr.Accept(gen);
            var enableOut = TOTALIZER.Inst["EnableOut"];
            var enableIn = TOTALIZER.Inst["EnableIn"];
            var firstScan = TOTALIZER.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("TOTTag", TOTALIZER.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => TOTALIZER.Inst["EnableIn"];
        protected override string InstrName => "TOT";
    }

    internal class DEDTInstruction : FixedInstruction
    {
        public DEDTInstruction(ParamInfoList infos) : base("DEDT", infos)
        {
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == DEADTIME.Inst);
            addr.Accept(gen);
            var enableOut = DEADTIME.Inst["EnableOut"];
            var enableIn = DEADTIME.Inst["EnableIn"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("DEDTTag", DEADTIME.Inst);
            var param2 = new Tuple<string, IDataType>("StorageArray", new ArrayTypeDimOne(REAL.Inst));
            return new List<Tuple<string, IDataType>>() {param1, param2};
        }
    }

    internal class D2SDInstruction : FixedFBDInstruction
    {
        public D2SDInstruction(ParamInfoList infos) : base("D2SD", infos)
        {
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == DISCRETE_2STATE.Inst);
            addr.Accept(gen);
            var enableOut = DISCRETE_2STATE.Inst["EnableOut"];
            var enableIn = DISCRETE_2STATE.Inst["EnableIn"];
            var prescan = DISCRETE_2STATE.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), prescan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("D2SDTag", DISCRETE_2STATE.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => DISCRETE_2STATE.Inst["EnableIn"];
        protected override string InstrName => "D2SD";
    }

    internal class D3SDInstruction : FixedFBDInstruction
    {
        public D3SDInstruction(ParamInfoList infos) : base("D3SD", infos)
        {
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == DISCRETE_3STATE.Inst);
            addr.Accept(gen);
            var enableOut = DISCRETE_3STATE.Inst["EnableOut"];
            var enableIn = DISCRETE_3STATE.Inst["EnableIn"];
            var prescan = DISCRETE_3STATE.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), prescan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("D3SDTag", DISCRETE_3STATE.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => DISCRETE_3STATE.Inst["EnableIn"];
        protected override string InstrName => "D3SD";
    }

}
