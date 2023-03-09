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

    internal class HMIBCInstruction : FixedFBDInstruction
    {
        public HMIBCInstruction(ParamInfoList infos) : base("HMIBC", infos)
        {

        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("HMIBC", HMIBC.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => HMIBC.Inst["EnableIn"];
        protected override string InstrName => "FBDHMIBC";

    }

    //rll instruction
    internal class R_HMIBCInstruction : FixedFBDInstruction
    {
        public R_HMIBCInstruction(ParamInfoList infos) : base("HMIBC", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == HMIBC.Inst);
            addr.Accept(gen);
            var enableOut = HMIBC.Inst["EnableOut"];
            Utils.GenClearBit(gen.masm(), enableOut);
            gen.masm().Pop();
        }

        protected override string InstrName => "HMIBC";

    }
}
