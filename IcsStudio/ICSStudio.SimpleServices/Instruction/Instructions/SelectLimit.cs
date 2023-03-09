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

    internal class SELInstruction : FixedFBDInstruction
    {
        public SELInstruction(ParamInfoList infos) : base("SEL", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == SELECT.Inst);
            addr.Accept(gen);
            var enableOut = SELECT.Inst["EnableOut"];
            var enableIn = SELECT.Inst["EnableIn"];
            var firstScan = SELECT.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        protected override IDataTypeMember EnableInMember => SELECT.Inst["EnableIn"];
        protected override string InstrName => "SEL";
    }

    internal class ESELInstruction : FixedFBDInstruction
    {
        public ESELInstruction(ParamInfoList infos) : base("ESEL", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == SELECT_ENHANCED.Inst);
            addr.Accept(gen);
            var enableOut = SELECT_ENHANCED.Inst["EnableOut"];
            var enableIn = SELECT_ENHANCED.Inst["EnableIn"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("ESELTag", SELECT_ENHANCED.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => SELECT_ENHANCED.Inst["EnableIn"];
        protected override string InstrName => "ESEL";
    }

    internal class SSUMInstruction : FixedFBDInstruction
    {
        public SSUMInstruction(ParamInfoList infos) : base("SSUM", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == SELECTED_SUMMER.Inst);
            addr.Accept(gen);
            var enableOut = SELECTED_SUMMER.Inst["EnableOut"];
            var enableIn = SELECTED_SUMMER.Inst["EnableIn"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("SSUMTag", SELECTED_SUMMER.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => SELECTED_SUMMER.Inst["EnableIn"];
        protected override string InstrName => "SSUM";
    }

    internal class SNEGInstruction : FixedFBDInstruction
    {
        public SNEGInstruction(ParamInfoList infos) : base("SNEG", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(
                addr != null && (addr.type is RefType) && (addr.type as RefType).type == SELECTABLE_NEGATE.Inst);
            addr.Accept(gen);
            var enableOut = SELECTABLE_NEGATE.Inst["EnableOut"];
            var enableIn = SELECTABLE_NEGATE.Inst["EnableIn"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("SNEGTag", SELECTABLE_NEGATE.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => SELECTABLE_NEGATE.Inst["EnableIn"];
        protected override string InstrName => "SNEG";
    }

    internal class MUXInstruction : FixedFBDInstruction
    {
        public MUXInstruction(ParamInfoList infos) : base("MUX", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == MULTIPLEXER.Inst);
            addr.Accept(gen);
            var enableOut = MULTIPLEXER.Inst["EnableOut"];
            var enableIn = MULTIPLEXER.Inst["EnableIn"];
            var firstScan = MULTIPLEXER.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        protected override IDataTypeMember EnableInMember => MULTIPLEXER.Inst["EnableIn"];
        protected override string InstrName => "MUX";
    }

    internal class HLLInstruction : FixedFBDInstruction
    {
        public HLLInstruction(ParamInfoList infos) : base("HLL", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == HL_LIMIT.Inst);
            addr.Accept(gen);
            var enableOut = HL_LIMIT.Inst["EnableOut"];
            var enableIn = HL_LIMIT.Inst["EnableIn"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("HLLTag", HL_LIMIT.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => HL_LIMIT.Inst["EnableIn"];
        protected override string InstrName => "HLL";
    }

    internal class RLIMInstruction : FixedFBDInstruction
    {
        public RLIMInstruction(ParamInfoList infos) : base("RLIM", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == RATE_LIMITER.Inst);
            addr.Accept(gen);
            var enableOut = RATE_LIMITER.Inst["EnableOut"];
            var enableIn = RATE_LIMITER.Inst["EnableIn"];
            var firstScan = RATE_LIMITER.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("RLIMTag", RATE_LIMITER.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => RATE_LIMITER.Inst["EnableIn"];
        protected override string InstrName => "RLIM";
    }
}
