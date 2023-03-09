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

    internal class MAATInstruction : Fixed2PosTokenInstruction
    {
        public MAATInstruction(ParamInfoList infos) : base("MAAT", infos)
        {

        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Axis", new ExpectType(AXIS_SERVO.Inst, AXIS_SERVO_DRIVE.Inst));
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            return new List<Tuple<string, IDataType>>() {param1, param2};
        }
    }

    internal class MRATInstruction : Fixed2PosTokenInstruction
    {
        public MRATInstruction(ParamInfoList infos) : base("MRAT", infos)
        {

        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Axis",
                new ExpectType(AXIS_SERVO.Inst, AXIS_SERVO_DRIVE.Inst, AXIS_CIP_DRIVE.Inst));
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            return new List<Tuple<string, IDataType>>() {param1, param2};
        }
    }

    internal class MAHDInstruction : Fixed2PosTokenInstruction
    {
        public MAHDInstruction(ParamInfoList infos) : base("MAHD", infos)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.MAHD.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.MAHD.ParseRLLParameters(parameters);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Axis", new ExpectType(AXIS_SERVO.Inst, AXIS_SERVO_DRIVE.Inst));
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            var param3 =
                new Tuple<string, IDataType>("Diagnostic Test", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param4 =
                new Tuple<string, IDataType>("Observed Direction", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            return new List<Tuple<string, IDataType>>() {param1, param2, param3, param4};
        }
    }

    internal class MRHDInstruction : Fixed2PosTokenInstruction
    {
        public MRHDInstruction(ParamInfoList infos) : base("MRHD", infos)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.MRHD.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.MRHD.ParseRLLParameters(parameters);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Axis",
                new ExpectType(AXIS_SERVO.Inst, AXIS_SERVO_DRIVE.Inst, AXIS_CIP_DRIVE.Inst));
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            var param3 =
                new Tuple<string, IDataType>("Diagnostic Test", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            return new List<Tuple<string, IDataType>>() {param1, param2, param3};
        }
    }

}
