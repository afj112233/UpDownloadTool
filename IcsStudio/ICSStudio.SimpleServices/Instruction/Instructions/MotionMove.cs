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

    internal class MASInstruction : Fixed2PosTokenInstruction
    {
        public MASInstruction(ParamInfoList infos) : base("MAS", infos)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.MAS.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.MAS.ParseRLLParameters(parameters);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Axis",
                new ExpectType(AXIS_VIRTUAL.Inst, AXIS_GENERIC.Inst, AXIS_GENERIC_DRIVE.Inst, AXIS_SERVO.Inst,
                    AXIS_SERVO_DRIVE.Inst, AXIS_CIP_DRIVE.Inst));
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            var param3 = new Tuple<string, IDataType>("Stop Type", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param4 = new Tuple<string, IDataType>("Change Decel", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param5 = new Tuple<string, IDataType>("Decel Rate",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param6 = new Tuple<string, IDataType>("Decel Units", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param7 =
                new Tuple<string, IDataType>("Change Decel Jerk", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param8 = new Tuple<string, IDataType>("Decel Jerk",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param9 = new Tuple<string, IDataType>("Jerk Units", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            return new List<Tuple<string, IDataType>>()
                {param1, param2, param3, param4, param5, param6, param7, param8, param9};
        }
    }

    internal class MAHInstruction : Fixed2PosTokenInstruction
    {
        public MAHInstruction(ParamInfoList infos) : base("MAH", infos)
        {

        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Axis",
                new ExpectType(AXIS_VIRTUAL.Inst, AXIS_GENERIC.Inst, AXIS_GENERIC_DRIVE.Inst, AXIS_SERVO.Inst,
                    AXIS_SERVO_DRIVE.Inst, AXIS_CIP_DRIVE.Inst));
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            return new List<Tuple<string, IDataType>>() {param1, param2};
        }
    }

    internal class MAJInstruction : Fixed2PosTokenInstruction
    {
        public MAJInstruction(ParamInfoList infos) : base("MAJ", infos)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.MAJ.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.MAJ.ParseRLLParameters(parameters);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Axis",
                new ExpectType(AXIS_VIRTUAL.Inst, AXIS_GENERIC.Inst, AXIS_GENERIC_DRIVE.Inst, AXIS_SERVO.Inst,
                    AXIS_SERVO_DRIVE.Inst, AXIS_CIP_DRIVE.Inst));
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            var param3 = new Tuple<string, IDataType>("Direction", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param4 =
                new Tuple<string, IDataType>("Speed", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param5 = new Tuple<string, IDataType>("Speed Units", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param6 = new Tuple<string, IDataType>("Accel Rate",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param7 = new Tuple<string, IDataType>("Accel Units", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param8 = new Tuple<string, IDataType>("Decel Rate",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param9 = new Tuple<string, IDataType>("Decel Units", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param10 = new Tuple<string, IDataType>("Profile", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param11 = new Tuple<string, IDataType>("Accel Jerk",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param12 = new Tuple<string, IDataType>("Decel Jerk",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param13 = new Tuple<string, IDataType>("Jerk Units", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param14 = new Tuple<string, IDataType>("Merge", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param15 = new Tuple<string, IDataType>("Merge Speed", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param16 = new Tuple<string, IDataType>("Lock Position",new ZeroType());
            var param17 =
                new Tuple<string, IDataType>("Lock Direction", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            return new List<Tuple<string, IDataType>>()
            {
                param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12,
                param13, param14, param15, param16, param17
            };
        }
    }

    internal class MAMInstruction : Fixed2PosTokenInstruction
    {
        public MAMInstruction(ParamInfoList infos) : base("MAM", infos)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.MAM.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.MAM.ParseRLLParameters(parameters);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Axis",
                new ExpectType(AXIS_VIRTUAL.Inst, AXIS_GENERIC.Inst, AXIS_GENERIC_DRIVE.Inst, AXIS_SERVO.Inst,
                    AXIS_SERVO_DRIVE.Inst, AXIS_CIP_DRIVE.Inst));
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            var param3 = new Tuple<string, IDataType>("Move Type", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param4 =
                new Tuple<string, IDataType>("Position", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param5 =
                new Tuple<string, IDataType>("Speed", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param6 = new Tuple<string, IDataType>("Speed Units", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param7 = new Tuple<string, IDataType>("Accel Rate",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param8 = new Tuple<string, IDataType>("Accel Units", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param9 = new Tuple<string, IDataType>("Decel Rate",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param10 = new Tuple<string, IDataType>("Decel Units", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param11 = new Tuple<string, IDataType>("Profile", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param12 = new Tuple<string, IDataType>("Accel Jerk",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param13 = new Tuple<string, IDataType>("Decel Jerk",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param14 = new Tuple<string, IDataType>("Jerk Units", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param15 = new Tuple<string, IDataType>("Merge", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param16 = new Tuple<string, IDataType>("Merge Speed", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param17 = new Tuple<string, IDataType>("Lock Position", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst,REAL.Inst));
            var param18 =
                new Tuple<string, IDataType>("Lock Direction", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param19 =
                new Tuple<string, IDataType>("Event Distance", new ExpectType(DINT.Inst, new ArrayTypeNormal(REAL.Inst)));
            var param20 =
                new Tuple<string, IDataType>("Calculated Data", new ExpectType(DINT.Inst, new ArrayTypeNormal(REAL.Inst)));
            return new List<Tuple<string, IDataType>>()
            {
                param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12,
                param13, param14, param15, param16, param17, param18, param19, param20
            };
        }
    }

    internal class MAGInstruction : Fixed2PosTokenInstruction
    {
        public MAGInstruction(ParamInfoList infos) : base("MAG", infos)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.MAG.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.MAG.ParseRLLParameters(parameters);
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

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Slave Axis",
                new ExpectType(AXIS_VIRTUAL.Inst, AXIS_GENERIC.Inst, AXIS_GENERIC_DRIVE.Inst, AXIS_SERVO.Inst,
                    AXIS_SERVO_DRIVE.Inst, AXIS_CIP_DRIVE.Inst));
            var param2 = new Tuple<string, IDataType>("Master Axis",
                new ExpectType(AXIS_CONSUMED.Inst, AXIS_VIRTUAL.Inst, AXIS_GENERIC.Inst, AXIS_GENERIC_DRIVE.Inst,
                    AXIS_SERVO.Inst, AXIS_SERVO_DRIVE.Inst, AXIS_CIP_DRIVE.Inst));
            var param3 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            var param4 = new Tuple<string, IDataType>("Direction", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param5 =
                new Tuple<string, IDataType>("Ratio", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param6 = new Tuple<string, IDataType>("Slave Counts", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param7 = new Tuple<string, IDataType>("Master Counts", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param8 =
                new Tuple<string, IDataType>("Master Reference", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param9 = new Tuple<string, IDataType>("Ratio Format", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param10 = new Tuple<string, IDataType>("Clutch", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param11 = new Tuple<string, IDataType>("Accel Rate",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param12 = new Tuple<string, IDataType>("Accel Units", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            return new List<Tuple<string, IDataType>>()
                {param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12};
        }
    }

    internal class MCDInstruction : Fixed2PosTokenInstruction
    {
        public MCDInstruction(ParamInfoList infos) : base("MCD", infos)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.MCD.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.MCD.ParseRLLParameters(parameters);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Axis",
                new ExpectType(AXIS_VIRTUAL.Inst, AXIS_GENERIC.Inst, AXIS_GENERIC_DRIVE.Inst, AXIS_SERVO.Inst,
                    AXIS_SERVO_DRIVE.Inst, AXIS_CIP_DRIVE.Inst));
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            var param3 = new Tuple<string, IDataType>("Motion Type", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param4 = new Tuple<string, IDataType>("Change Speed", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param5 =
                new Tuple<string, IDataType>("Speed", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param6 = new Tuple<string, IDataType>("Change Accel", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param7 = new Tuple<string, IDataType>("Accel Rate",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param8 = new Tuple<string, IDataType>("Change Decel", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param9 = new Tuple<string, IDataType>("Decel Rate",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param10 =
                new Tuple<string, IDataType>("Change Accel Jerk", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param11 = new Tuple<string, IDataType>("Accel Jerk",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param12 =
                new Tuple<string, IDataType>("Change Decel Jerk", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param13 = new Tuple<string, IDataType>("Decel Jerk",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param14 = new Tuple<string, IDataType>("Speed Units", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param15 = new Tuple<string, IDataType>("Accel Units", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param16 = new Tuple<string, IDataType>("Decel Units", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param17 = new Tuple<string, IDataType>("Jerk Units", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            return new List<Tuple<string, IDataType>>()
            {
                param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12,
                param13, param14, param15, param16, param17
            };
        }
    }

    internal class MRPInstruction : Fixed2PosTokenInstruction
    {
        public MRPInstruction(ParamInfoList infos) : base("MRP", infos)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.MRP.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.MRP.ParseRLLParameters(parameters);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Axis",
                new ExpectType(AXIS_VIRTUAL.Inst, AXIS_GENERIC.Inst, AXIS_GENERIC_DRIVE.Inst, AXIS_SERVO.Inst,
                    AXIS_SERVO_DRIVE.Inst, AXIS_CIP_DRIVE.Inst));
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            var param3 = new Tuple<string, IDataType>("Type", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param4 =
                new Tuple<string, IDataType>("Position Select", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param5 =
                new Tuple<string, IDataType>("Position", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));

            return new List<Tuple<string, IDataType>>() {param1, param2, param3, param4, param5};
        }
    }

    internal class MCCPInstruction : Fixed2PosTokenInstruction
    {
        public MCCPInstruction(ParamInfoList infos) : base("MCCP", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var type = MOTION_INSTRUCTION.Inst;
            var instr = paramList.nodes[1] as ASTNameAddr;

            Debug.Assert(CheckInstrParamType.CheckMotionParam(instr));

            paramList.nodes[1].Accept(gen);

            Utils.GenClearBit(gen.masm(), type["EN"]);
            Utils.GenClearBit(gen.masm(), type["DN"]);
            Utils.GenClearBit(gen.masm(), type["ER"]);
            Utils.GenClearBit(gen.masm(), type["IP"]);
            gen.masm().Pop();
            gen.masm().BiPush(0);


        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            var param2 = new Tuple<string, IDataType>("Cam", new ArrayTypeDimOne(CAM.Inst));
            var param3 = new Tuple<string, IDataType>("Length", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param4 = new Tuple<string, IDataType>("Start Slope",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param5 =
                new Tuple<string, IDataType>("End Slope", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param6 = new Tuple<string, IDataType>("Cam Profile", new ArrayTypeDimOne(CAM_PROFILE.Inst));

            return new List<Tuple<string, IDataType>>() {param1, param2, param3, param4, param5, param6};
        }
    }

    internal class MCSVInstruction : Fixed2PosTokenInstruction
    {
        public MCSVInstruction(ParamInfoList infos) : base("MCSV", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var type = MOTION_INSTRUCTION.Inst;
            var instr = paramList.nodes[1] as ASTNameAddr;

            Debug.Assert(CheckInstrParamType.CheckMotionParam(instr));

            paramList.nodes[1].Accept(gen);

            Utils.GenClearBit(gen.masm(), type["EN"]);
            Utils.GenClearBit(gen.masm(), type["DN"]);
            Utils.GenClearBit(gen.masm(), type["ER"]);
            Utils.GenClearBit(gen.masm(), type["IP"]);
            gen.masm().Pop();
            gen.masm().BiPush(0);


        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            var param2 = new Tuple<string, IDataType>("Cam Profile", new ArrayTypeDimOne(CAM_PROFILE.Inst));
            var param3 = new Tuple<string, IDataType>("Master Value", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param4 = new Tuple<string, IDataType>("Slave Value", REAL.Inst);
            var param5 = new Tuple<string, IDataType>("Slope Value", REAL.Inst);
            var param6 = new Tuple<string, IDataType>("Slope Derivative", REAL.Inst);

            return new List<Tuple<string, IDataType>>() {param1, param2, param3, param4, param5, param6};
        }
    }

    internal class MAPCInstruction : Fixed2PosTokenInstruction
    {
        public MAPCInstruction(ParamInfoList infos) : base("MAPC", infos)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.MAPC.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.MAPC.ParseRLLParameters(parameters);
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
            Utils.GenClearBit(gen.masm(), type["AC"]);
            Utils.GenClearBit(gen.masm(), type["PC"]);
            gen.masm().Pop();
            gen.masm().BiPush(0);


        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Slave Axis",
                new ExpectType(AXIS_VIRTUAL.Inst, AXIS_GENERIC.Inst, AXIS_GENERIC_DRIVE.Inst, AXIS_SERVO.Inst,
                    AXIS_SERVO_DRIVE.Inst, AXIS_CIP_DRIVE.Inst));
            var param2 = new Tuple<string, IDataType>("Master Axis",
                new ExpectType(AXIS_CONSUMED.Inst, AXIS_VIRTUAL.Inst, AXIS_GENERIC.Inst, AXIS_GENERIC_DRIVE.Inst,
                    AXIS_SERVO.Inst, AXIS_SERVO_DRIVE.Inst, AXIS_CIP_DRIVE.Inst));
            var param3 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            var param4 = new Tuple<string, IDataType>("Direction", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param5 = new Tuple<string, IDataType>("Cam Profile", new ArrayTypeDimOne(CAM_PROFILE.Inst));
            var param6 = new Tuple<string, IDataType>("Slave Scaling",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param7 = new Tuple<string, IDataType>("Master Scaling",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param8 = new Tuple<string, IDataType>("Execution Mode", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param9 =
                new Tuple<string, IDataType>("Execution Schedule", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param10 = new Tuple<string, IDataType>("Master Lock Position",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param11 = new Tuple<string, IDataType>("Cam Lock Position",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param12 =
                new Tuple<string, IDataType>("Master Reference", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param13 =
                new Tuple<string, IDataType>("Master Direction", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));

            return new List<Tuple<string, IDataType>>()
            {
                param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12,
                param13
            };
        }
    }

    internal class MATCInstruction : Fixed2PosTokenInstruction
    {
        public MATCInstruction(ParamInfoList infos) : base("MATC", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var type = MOTION_INSTRUCTION.Inst;
            var instr = paramList.nodes[2] as ASTNameAddr;

            Debug.Assert(CheckInstrParamType.CheckMotionParam(instr));

            paramList.nodes[2].Accept(gen);

            Utils.GenClearBit(gen.masm(), type["EN"]);
            Utils.GenClearBit(gen.masm(), type["DN"]);
            Utils.GenClearBit(gen.masm(), type["ER"]);
            Utils.GenClearBit(gen.masm(), type["IP"]);
            Utils.GenClearBit(gen.masm(), type["AC"]);
            Utils.GenClearBit(gen.masm(), type["PC"]);
            gen.masm().Pop();
            gen.masm().BiPush(0);
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.MATC.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.MATC.ParseRLLParameters(parameters);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Slave Axis",
                new ExpectType(AXIS_VIRTUAL.Inst, AXIS_GENERIC.Inst, AXIS_GENERIC_DRIVE.Inst, AXIS_SERVO.Inst,
                    AXIS_SERVO_DRIVE.Inst, AXIS_CIP_DRIVE.Inst));
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            var param3 = new Tuple<string, IDataType>("Direction", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param4 = new Tuple<string, IDataType>("Cam Profile", new ArrayTypeDimOne(CAM_PROFILE.Inst));
            var param5 = new Tuple<string, IDataType>("Distance Scaling",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param6 = new Tuple<string, IDataType>("Time Scaling",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param7 = new Tuple<string, IDataType>("Execution Mode", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param8 =
                new Tuple<string, IDataType>("Execution Schedule", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param9 = new Tuple<string, IDataType>("Lock Position",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param10 =
                new Tuple<string, IDataType>("Lock Direction", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param11 =
                new Tuple<string, IDataType>("Instruction Mode", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));

            return new List<Tuple<string, IDataType>>()
                {param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11};
        }
    }

    internal class MDACInstruction : Fixed2PosTokenInstruction
    {
        public MDACInstruction(ParamInfoList infos) : base("MDAC", infos)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.MDAC.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.MDAC.ParseRLLParameters(parameters);
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

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Slave Axis",
                new ExpectType(AXIS_VIRTUAL.Inst, AXIS_GENERIC.Inst, AXIS_GENERIC_DRIVE.Inst, AXIS_SERVO.Inst,
                    AXIS_SERVO_DRIVE.Inst, AXIS_CIP_DRIVE.Inst));
            var param2 = new Tuple<string, IDataType>("Master Axis",
                new ExpectType(AXIS_CONSUMED.Inst, AXIS_VIRTUAL.Inst, AXIS_GENERIC.Inst, AXIS_GENERIC_DRIVE.Inst,
                    AXIS_SERVO.Inst, AXIS_SERVO_DRIVE.Inst, AXIS_CIP_DRIVE.Inst));
            var param3 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            var param4 = new Tuple<string, IDataType>("Motion Type", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param5 =
                new Tuple<string, IDataType>("Master Reference", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));

            return new List<Tuple<string, IDataType>>() {param1, param2, param3, param4, param5};
        }
    }
}
