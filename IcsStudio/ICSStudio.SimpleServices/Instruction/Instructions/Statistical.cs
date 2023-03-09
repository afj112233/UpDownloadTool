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

    internal class MAVEInstruction : FixedInstruction
    {
        public MAVEInstruction(ParamInfoList infos) : base("MAVE", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == MOVING_AVERAGE.Inst);
            addr.Accept(gen);
            var enableOut = MOVING_AVERAGE.Inst["EnableOut"];
            var enableIn = MOVING_AVERAGE.Inst["EnableIn"];
            var firstScan = MOVING_AVERAGE.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("MAVETag", MOVING_AVERAGE.Inst);
            var param2 = new Tuple<string, IDataType>("StorageArray", new ArrayTypeNormal(REAL.Inst));
            var param3 = new Tuple<string, IDataType>("WeightArray", new ArrayTypeNormal(REAL.Inst));
            return new List<Tuple<string, IDataType>>() {param1, param2, param3};
        }
    }

    internal class MSTDInstruction : FixedInstruction
    {
        public MSTDInstruction(ParamInfoList infos) : base("MSTD", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == MOVING_STD_DEV.Inst);
            addr.Accept(gen);
            var enableOut = MOVING_STD_DEV.Inst["EnableOut"];
            var enableIn = MOVING_STD_DEV.Inst["EnableIn"];
            var firstScan = MOVING_STD_DEV.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("MSTDTag", MOVING_STD_DEV.Inst);
            var param2 = new Tuple<string, IDataType>("StorageArray", new ArrayTypeNormal(REAL.Inst));
            return new List<Tuple<string, IDataType>>() {param1, param2};
        }
    }

    internal class MINCInstruction : FixedFBDInstruction
    {
        public MINCInstruction(ParamInfoList infos) : base("MINC", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == MINIMUM_CAPTURE.Inst);
            addr.Accept(gen);
            var enableOut = MINIMUM_CAPTURE.Inst["EnableOut"];
            var enableIn = MINIMUM_CAPTURE.Inst["EnableIn"];
            var firstScan = MINIMUM_CAPTURE.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("MINCTag", MINIMUM_CAPTURE.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => MINIMUM_CAPTURE.Inst["EnableIn"];
        protected override string InstrName => "MINC";
    }

    internal class MAXCInstruction : FixedFBDInstruction
    {
        public MAXCInstruction(ParamInfoList infos) : base("MAXC", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == MAXIMUM_CAPTURE.Inst);
            addr.Accept(gen);
            var enableOut = MAXIMUM_CAPTURE.Inst["EnableOut"];
            var enableIn = MAXIMUM_CAPTURE.Inst["EnableIn"];
            var firstScan = MAXIMUM_CAPTURE.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);

            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("MAXCTag", MAXIMUM_CAPTURE.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => MAXIMUM_CAPTURE.Inst["EnableIn"];
        protected override string InstrName => "MAXC";
    }

}
