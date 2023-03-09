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

    internal class MCSInstruction : Fixed2PosTokenInstruction
    {
        public MCSInstruction(ParamInfoList infos) : base("MCS", infos)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.MCS.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.MCS.ParseRLLParameters(parameters);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Coordinate", COORDINATE_SYSTEM.Inst);
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

    internal class MCLMInstruction : Fixed2PosTokenInstruction
    {
        public MCLMInstruction(ParamInfoList infos) : base("MCLM", infos)
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
            return AllFInstructions.MCLM.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.MCLM.ParseRLLParameters(parameters);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Coordinate", COORDINATE_SYSTEM.Inst);
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            var param3 = new Tuple<string, IDataType>("Move Type", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param4 = new Tuple<string, IDataType>("Position", new ArrayTypeDimOne(REAL.Inst));
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
            var param15 =
                new Tuple<string, IDataType>("Termination Type", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param16 = new Tuple<string, IDataType>("Merge", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param17 = new Tuple<string, IDataType>("Merge Speed", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param18 = new Tuple<string, IDataType>("Command Tolerance",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param19 = new Tuple<string, IDataType>("Lock Position",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param20 =
                new Tuple<string, IDataType>("Lock Direction", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param21 = new Tuple<string, IDataType>("Event Distance",
                new ExpectType(new ArrayTypeNormal(SINT.Inst), new ArrayTypeNormal(INT.Inst),
                    new ArrayTypeNormal(DINT.Inst), new ArrayTypeNormal(REAL.Inst)));
            var param22 = new Tuple<string, IDataType>("Calculated Data",
                new ExpectType(new ArrayTypeNormal(SINT.Inst), new ArrayTypeNormal(INT.Inst),
                    new ArrayTypeNormal(DINT.Inst), new ArrayTypeNormal(REAL.Inst)));
            return new List<Tuple<string, IDataType>>()
            {
                param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12,
                param13, param14, param15, param16, param17, param18, param19, param20, param21, param22
            };
        }
    }

    internal class MCCMInstruction : Fixed2PosTokenInstruction
    {
        public MCCMInstruction(ParamInfoList infos) : base("MCCM", infos)
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
            return AllFInstructions.MCCM.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.MCCM.ParseRLLParameters(parameters);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Coordinate", COORDINATE_SYSTEM.Inst);
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            var param3 = new Tuple<string, IDataType>("Move Type", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param4 = new Tuple<string, IDataType>("Position", REAL.Inst);
            var param5 = new Tuple<string, IDataType>("Circle Type", new ArrayTypeNormal(REAL.Inst));
            var param6 = new Tuple<string, IDataType>("Via/Center/Radius", new ArrayTypeNormal(REAL.Inst));
            var param7 = new Tuple<string, IDataType>("Direction", new ArrayTypeNormal(REAL.Inst));
            var param8 =
                new Tuple<string, IDataType>("Speed", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param9 = new Tuple<string, IDataType>("Speed Units", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param10 = new Tuple<string, IDataType>("Accel Rate",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param11 = new Tuple<string, IDataType>("Accel Units", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param12 = new Tuple<string, IDataType>("Decel Rate",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param13 = new Tuple<string, IDataType>("Decel Units", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param14 = new Tuple<string, IDataType>("Profile", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param15 = new Tuple<string, IDataType>("Accel Jerk",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param16 = new Tuple<string, IDataType>("Decel Jerk",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param17 = new Tuple<string, IDataType>("Jerk Units", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param18 =
                new Tuple<string, IDataType>("Termination Type", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param19 = new Tuple<string, IDataType>("Merge", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param20 = new Tuple<string, IDataType>("Merge Speed", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param21 = new Tuple<string, IDataType>("Command Tolerance",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param22 = new Tuple<string, IDataType>("Lock Position",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param23 =
                new Tuple<string, IDataType>("Lock Direction", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param24 = new Tuple<string, IDataType>("Event Distance", new ArrayTypeNormal(REAL.Inst));
            var param25 = new Tuple<string, IDataType>("Calculated Data", new ArrayTypeNormal(REAL.Inst));
            return new List<Tuple<string, IDataType>>()
            {
                param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12,
                param13, param14, param15, param16, param17, param18, param19, param20, param21, param22, param23,
                param24, param25
            };
        }
    }

    internal class MCCDInstruction : Fixed2PosTokenInstruction
    {
        public MCCDInstruction(ParamInfoList infos) : base("MCCD", infos)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.MCCD.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.MCCD.ParseRLLParameters(parameters);
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
            gen.masm().Pop();
            gen.masm().BiPush(0);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Coordinate", COORDINATE_SYSTEM.Inst);
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            var param3 = new Tuple<string, IDataType>("Move Type", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param4 = new Tuple<string, IDataType>("Change Speed", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param5 =
                new Tuple<string, IDataType>("Speed", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param6 = new Tuple<string, IDataType>("Speed Units", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param7 = new Tuple<string, IDataType>("Change Accel", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param8 = new Tuple<string, IDataType>("Accel Rate",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param9 = new Tuple<string, IDataType>("Accel Units", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param10 = new Tuple<string, IDataType>("Change Decel", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param11 = new Tuple<string, IDataType>("Decel Rate",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param12 = new Tuple<string, IDataType>("Decel Units", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param13 =
                new Tuple<string, IDataType>("Change Accel Jerk", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param14 = new Tuple<string, IDataType>("Accel Jerk",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param15 = new Tuple<string, IDataType>("Change Decel Jerk",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param16 = new Tuple<string, IDataType>("Decel Jerk", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param17 = new Tuple<string, IDataType>("Jerk Units", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param18 = new Tuple<string, IDataType>("Scope", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));

            return new List<Tuple<string, IDataType>>()
            {
                param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12,
                param13, param14, param15, param16, param17, param18
            };
        }
    }

    internal class MCTInstruction : Fixed2PosTokenInstruction
    {
        public MCTInstruction(ParamInfoList infos) : base("MCT", infos)
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

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Source System", COORDINATE_SYSTEM.Inst);
            var param2 = new Tuple<string, IDataType>("Target System", COORDINATE_SYSTEM.Inst);
            var param3 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            var param4 = new Tuple<string, IDataType>("Orientation", new ArrayTypeNormal(REAL.Inst));
            var param5 = new Tuple<string, IDataType>("Translation", new ArrayTypeNormal(REAL.Inst));
            return new List<Tuple<string, IDataType>>() {param1, param2, param3, param4, param5};
        }
    }

    internal class MCTPInstruction : Fixed2PosTokenInstruction
    {
        public MCTPInstruction(ParamInfoList infos) : base("MCTP", infos)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.MCTP.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.MCTP.ParseRLLParameters(parameters);
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
            var param1 = new Tuple<string, IDataType>("Source System", COORDINATE_SYSTEM.Inst);
            var param2 = new Tuple<string, IDataType>("Target System", COORDINATE_SYSTEM.Inst);
            var param3 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            var param4 = new Tuple<string, IDataType>("Orientation", new ArrayTypeNormal(REAL.Inst));
            var param5 = new Tuple<string, IDataType>("Translation", new ArrayTypeNormal(REAL.Inst));
            var param6 = new Tuple<string, IDataType>("Translation Direction",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param7 = new Tuple<string, IDataType>("Reference Position", new ArrayTypeNormal(REAL.Inst));
            var param8 = new Tuple<string, IDataType>("Transform Position", new ArrayTypeNormal(REAL.Inst));
            return new List<Tuple<string, IDataType>>()
                {param1, param2, param3, param4, param5, param6, param7, param8};
        }
    }

    internal class MCSDInstruction : Fixed2PosTokenInstruction
    {
        public MCSDInstruction(ParamInfoList infos) : base("MCSD", infos)
        {

        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Coordinate System", COORDINATE_SYSTEM.Inst);
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            return new List<Tuple<string, IDataType>>() {param1, param2};
        }
    }

    internal class MCSRInstruction : Fixed2PosTokenInstruction
    {
        public MCSRInstruction(ParamInfoList infos) : base("MCSR", infos)
        {

        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Coordinate System", COORDINATE_SYSTEM.Inst);
            var param2 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            return new List<Tuple<string, IDataType>>() {param1, param2};
        }
    }

    internal class MDCCInstruction : Fixed2PosTokenInstruction
    {
        public MDCCInstruction(ParamInfoList infos) : base("MDCC", infos)
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
            return AllFInstructions.MDCC.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.MDCC.ParseRLLParameters(parameters);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Slave System", COORDINATE_SYSTEM.Inst);
            var param2 = new Tuple<string, IDataType>("Master Axis",
                new ExpectType(AXIS_CONSUMED.Inst, AXIS_VIRTUAL.Inst, AXIS_GENERIC.Inst, AXIS_GENERIC_DRIVE.Inst,
                    AXIS_SERVO.Inst, AXIS_SERVO_DRIVE.Inst, AXIS_CIP_DRIVE.Inst));
            var param3 = new Tuple<string, IDataType>("Motion Control", MOTION_INSTRUCTION.Inst);
            var param4 =
                new Tuple<string, IDataType>("Master Reference", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param5 = new Tuple<string, IDataType>("Nominal Master Velocity", REAL.Inst);
            return new List<Tuple<string, IDataType>>() {param1, param2, param3, param4, param5};
        }
    }

}
