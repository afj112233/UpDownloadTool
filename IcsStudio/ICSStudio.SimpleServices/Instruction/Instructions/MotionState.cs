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

    internal class MSOInstruction : Fixed2B3PosTokenInstruction
    {
        public MSOInstruction(ParamInfoList infos) : base("MSO", infos)
        {

        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Axis", AXIS_CIP_DRIVE.Inst);
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            return new List<Tuple<string, IDataType>>()
                {param1, param2};
        }
    }

    //prescan:The rung-condition-out is set to false.
    internal class MSFInstruction : Fixed2B3PosTokenInstruction
    {
        public MSFInstruction(ParamInfoList infos) : base("MSF", infos)
        {

        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Axis", AXIS_CIP_DRIVE.Inst);
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            return new List<Tuple<string, IDataType>>()
                {param1, param2};
        }
    }

    internal class MASDInstruction : Fixed2B3PosTokenInstruction
    {
        public MASDInstruction(ParamInfoList infos) : base("MASD", infos)
        {

        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Axis",
                new ExpectType(AXIS_VIRTUAL.Inst, AXIS_GENERIC.Inst, AXIS_GENERIC_DRIVE.Inst, AXIS_SERVO.Inst,
                    AXIS_SERVO_DRIVE.Inst, AXIS_CIP_DRIVE.Inst));
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            return new List<Tuple<string, IDataType>>()
                {param1, param2};
        }
    }

    internal class MDOInstruction : Fixed2B3PosTokenInstruction
    {
        public MDOInstruction(ParamInfoList infos) : base("MDO", infos)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.MDO.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.MDO.ParseRLLParameters(parameters);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Axis", AXIS_SERVO.Inst);
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            var param3 = new Tuple<string, IDataType>("Drive Output",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param4 = new Tuple<string, IDataType>("Drive Units", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            return new List<Tuple<string, IDataType>>()
                {param1, param2, param3, param4};
        }
    }

    internal class MASRInstruction : Fixed2B3PosTokenInstruction
    {
        public MASRInstruction(ParamInfoList infos) : base("MASR", infos)
        {

        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Axis",
                new ExpectType(AXIS_VIRTUAL.Inst, AXIS_GENERIC.Inst, AXIS_GENERIC_DRIVE.Inst, AXIS_SERVO.Inst,
                    AXIS_SERVO_DRIVE.Inst, AXIS_CIP_DRIVE.Inst));
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            return new List<Tuple<string, IDataType>>()
                {param1, param2};
        }
    }

    internal class MDFInstruction : Fixed2B3PosTokenInstruction
    {
        public MDFInstruction(ParamInfoList infos) : base("MDF", infos)
        {

        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Axis", AXIS_SERVO.Inst);
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            return new List<Tuple<string, IDataType>>()
                {param1, param2};
        }
    }

    internal class MDSInstruction : Fixed2B3PosTokenInstruction
    {
        public MDSInstruction(ParamInfoList infos) : base("MDS", infos)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.MDS.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.MDS.ParseRLLParameters(parameters);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Axis", AXIS_CIP_DRIVE.Inst);
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            var param3 =
                new Tuple<string, IDataType>("Speed", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param4 = new Tuple<string, IDataType>("Speed Units", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            return new List<Tuple<string, IDataType>>()
                {param1, param2, param3, param4};
        }
    }

    internal class MAFRInstruction : Fixed2B3PosTokenInstruction
    {
        public MAFRInstruction(ParamInfoList infos) : base("MAFR", infos)
        {

        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Axis",
                new ExpectType(AXIS_VIRTUAL.Inst, AXIS_GENERIC.Inst, AXIS_GENERIC_DRIVE.Inst, AXIS_SERVO.Inst,
                    AXIS_SERVO_DRIVE.Inst, AXIS_CIP_DRIVE.Inst));
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            return new List<Tuple<string, IDataType>>()
                {param1, param2};
        }
    }

}
