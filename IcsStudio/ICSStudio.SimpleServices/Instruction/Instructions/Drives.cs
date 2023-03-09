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

    internal class PMULInstruction : FixedFBDInstruction
    {
        public PMULInstruction(ParamInfoList infos) : base("PMUL", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) &&
                         (addr.type as RefType).type == PULSE_MULTIPLIER.Inst);
            addr.Accept(gen);
            var enableOut = PULSE_MULTIPLIER.Inst["EnableOut"];
            var enableIn = PULSE_MULTIPLIER.Inst["EnableIn"];
            var firstScan = PULSE_MULTIPLIER.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("PMULTag", PULSE_MULTIPLIER.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => PULSE_MULTIPLIER.Inst["EnableIn"];
        protected override string InstrName => "PMUL";
    }

    internal class SCRVInstruction : FixedFBDInstruction
    {
        public SCRVInstruction(ParamInfoList infos) : base("SCRV", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == S_CURVE.Inst);
            addr.Accept(gen);
            var enableOut = S_CURVE.Inst["EnableOut"];
            var enableIn = S_CURVE.Inst["EnableIn"];
            var firstScan = S_CURVE.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("SCRVTag", S_CURVE.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => S_CURVE.Inst["EnableIn"];
        protected override string InstrName => "SCRV";
    }

    internal class PIInstruction : FixedFBDInstruction
    {
        public PIInstruction(ParamInfoList infos) : base("PI", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == PROP_INT.Inst);
            addr.Accept(gen);
            var enableOut = PROP_INT.Inst["EnableOut"];
            var enableIn = PROP_INT.Inst["EnableIn"];
            var firstScan = PROP_INT.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        /*
        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Utils.ThrowNotImplemented(this.Name);
            gen.masm().BiPush(0);
        }
        */
        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("PITag", PROP_INT.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => PROP_INT.Inst["EnableIn"];
        protected override string InstrName => "PI";
    }

    internal class INTGInstruction : FixedFBDInstruction
    {
        public INTGInstruction(ParamInfoList infos) : base("INTG", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == INTEGRATOR.Inst);
            addr.Accept(gen);
            var enableOut = INTEGRATOR.Inst["EnableOut"];
            var enableIn = INTEGRATOR.Inst["EnableIn"];
            var firstScan = INTEGRATOR.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("INTGTag", INTEGRATOR.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => INTEGRATOR.Inst["EnableIn"];
        protected override string InstrName => "INTG";
    }

    internal class SOCInstruction : FixedFBDInstruction
    {
        public SOCInstruction(ParamInfoList infos) : base("SOC", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) &&
                         (addr.type as RefType).type == SEC_ORDER_CONTROLLER.Inst);
            addr.Accept(gen);
            var enableOut = SEC_ORDER_CONTROLLER.Inst["EnableOut"];
            var enableIn = SEC_ORDER_CONTROLLER.Inst["EnableIn"];
            var firstScan = SEC_ORDER_CONTROLLER.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 =
                new Tuple<string, IDataType>("SOCTag", SEC_ORDER_CONTROLLER.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => SEC_ORDER_CONTROLLER.Inst["EnableIn"];
        protected override string InstrName => "SOC";
    }

    internal class UPDNInstruction : FixedFBDInstruction
    {
        public UPDNInstruction(ParamInfoList infos) : base("UPDN", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == UP_DOWN_ACCUM.Inst);
            addr.Accept(gen);
            var enableOut = UP_DOWN_ACCUM.Inst["EnableOut"];
            var enableIn = UP_DOWN_ACCUM.Inst["EnableIn"];
            var firstScan = UP_DOWN_ACCUM.Inst["FirstScan"];

            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);

            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("UPDNTag", UP_DOWN_ACCUM.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => UP_DOWN_ACCUM.Inst["EnableIn"];
        protected override string InstrName => "UPDN";
    }


}
