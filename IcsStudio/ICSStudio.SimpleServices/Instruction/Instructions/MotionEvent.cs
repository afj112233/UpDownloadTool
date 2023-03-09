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

    internal class MAWInstruction : Fixed2PosTokenInstruction
    {
        public MAWInstruction(ParamInfoList infos) : base("MAW", infos)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.MAW.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.MAW.ParseRLLParameters(parameters);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("AXis",
                new ExpectType(AXIS_GENERIC.Inst, AXIS_GENERIC_DRIVE.Inst, AXIS_SERVO.Inst, AXIS_SERVO_DRIVE.Inst,
                    AXIS_CIP_DRIVE.Inst));
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            var param3 =
                new Tuple<string, IDataType>("Trigger Condition", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param4 =
                new Tuple<string, IDataType>("Position", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            return new List<Tuple<string, IDataType>>() {param1, param2, param3, param4};
        }
    }

    internal class MDWInstruction : Fixed2PosTokenInstruction
    {
        public MDWInstruction(ParamInfoList infos) : base("MDW", infos)
        {

        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Axis",
                new ExpectType(AXIS_GENERIC.Inst, AXIS_GENERIC_DRIVE.Inst, AXIS_SERVO.Inst, AXIS_SERVO_DRIVE.Inst,
                    AXIS_CIP_DRIVE.Inst));
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            return new List<Tuple<string, IDataType>>() {param1, param2};
        }
    }

    internal class MARInstruction : Fixed2PosTokenInstruction
    {
        public MARInstruction(ParamInfoList infos) : base("MAR", infos)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.MAR.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.MAR.ParseRLLParameters(parameters);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Axis", AXIS_CIP_DRIVE.Inst);
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            var param3 =
                new Tuple<string, IDataType>("Trigger Condition", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param4 = new Tuple<string, IDataType>("Windowed Registration",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param5 = new Tuple<string, IDataType>("Min.Position",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param6 = new Tuple<string, IDataType>("Max.Position",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param7 = new Tuple<string, IDataType>("Input Number", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            return new List<Tuple<string, IDataType>>() {param1, param2, param3, param4, param5, param6, param7};
        }
    }

    internal class MDRInstruction : Fixed2PosTokenInstruction
    {
        public MDRInstruction(ParamInfoList infos) : base("MDR", infos)
        {

        }
        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.MDR.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.MDR.ParseRLLParameters(parameters);
        }
        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Axis",
                new ExpectType(AXIS_GENERIC.Inst, AXIS_GENERIC_DRIVE.Inst, AXIS_SERVO.Inst, AXIS_SERVO_DRIVE.Inst,
                    AXIS_CIP_DRIVE.Inst));
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            var param3 = new Tuple<string, IDataType>("Input Number", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            return new List<Tuple<string, IDataType>>() {param1, param2, param3};
        }
    }

    internal class MDOCInstruction : Fixed2PosTokenInstruction
    {
        public MDOCInstruction(ParamInfoList infos) : base("MDOC", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var type = MOTION_INSTRUCTION.Inst;
            var instr = paramList.nodes[3] as ASTNameAddr;

            Debug.Assert(CheckInstrParamType.CheckMotionParam(instr));

            paramList.nodes[3].Accept(gen);

            Utils.GenClearBit(gen.masm(), type["EN"]);
            Utils.GenClearBit(gen.masm(), type["DN"]);
            Utils.GenClearBit(gen.masm(), type["ER"]);
            Utils.GenClearBit(gen.masm(), type["IP"]);
            gen.masm().Pop();
            gen.masm().BiPush(0);


        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.MDOC.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.MDOC.ParseRLLParameters(parameters);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Axis",
                new ExpectType(AXIS_CONSUMED.Inst, AXIS_VIRTUAL.Inst, AXIS_GENERIC.Inst, AXIS_GENERIC_DRIVE.Inst,
                    AXIS_SERVO.Inst, AXIS_SERVO_DRIVE.Inst, AXIS_CIP_DRIVE.Inst));
            var param2 =
                new Tuple<string, IDataType>("Execution Target", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param3 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            var param4 = new Tuple<string, IDataType>("Disarm Type", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            return new List<Tuple<string, IDataType>>() {param1, param2, param3, param4};
        }
    }

    internal class MAOCInstruction : Fixed2PosTokenInstruction
    {
        public MAOCInstruction(ParamInfoList infos) : base("MAOC", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var type = MOTION_INSTRUCTION.Inst;
            var instr = paramList.nodes[3] as ASTNameAddr;

            Debug.Assert(CheckInstrParamType.CheckMotionParam(instr));

            paramList.nodes[3].Accept(gen);

            Utils.GenClearBit(gen.masm(), type["EN"]);
            Utils.GenClearBit(gen.masm(), type["DN"]);
            Utils.GenClearBit(gen.masm(), type["ER"]);
            Utils.GenClearBit(gen.masm(), type["IP"]);
            gen.masm().Pop();
            gen.masm().BiPush(0);


        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.MAOC.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.MAOC.ParseRLLParameters(parameters);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Axis",
                new ExpectType(AXIS_CONSUMED.Inst, AXIS_VIRTUAL.Inst, AXIS_GENERIC.Inst, AXIS_GENERIC_DRIVE.Inst,
                    AXIS_SERVO.Inst, AXIS_SERVO_DRIVE.Inst, AXIS_CIP_DRIVE.Inst));
            var param2 =
                new Tuple<string, IDataType>("Execution Target", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param3 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            var param4 = new Tuple<string, IDataType>("Output", DINT.Inst);
            var param5 = new Tuple<string, IDataType>("Input", DINT.Inst);
            var param6 = new Tuple<string, IDataType>("Output Cam", new ArrayTypeDimOne(OUTPUT_CAM.Inst));
            var param7 =
                new Tuple<string, IDataType>("Cam Start Position", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst,REAL.Inst));
            var param8 =
                new Tuple<string, IDataType>("Cam End Position", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst,REAL.Inst));
            var param9 =
                new Tuple<string, IDataType>("Output Compensation", new ArrayTypeDimOne(OUTPUT_COMPENSATION.Inst));
            var param10 =
                new Tuple<string, IDataType>("Execution Mode", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param11 =
                new Tuple<string, IDataType>("Execution Schedule", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param12 =
                new Tuple<string, IDataType>("Axis Arm Position", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst,REAL.Inst));
            var param13 =
                new Tuple<string, IDataType>("Cam Arm Position", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst,REAL.Inst));
            var param14 =
                new Tuple<string, IDataType>("Position Reference", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            return new List<Tuple<string, IDataType>>()
            {
                param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12,
                param13, param14
            };
        }
    }
}
