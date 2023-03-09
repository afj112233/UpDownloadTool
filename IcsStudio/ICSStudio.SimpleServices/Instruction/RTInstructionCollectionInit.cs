using ICSStudio.Interfaces.DataType;
using System.Collections.Generic;
using System.Diagnostics;
using ICSStudio.SimpleServices.PredefinedType;

namespace ICSStudio.SimpleServices.Instruction
{
    public partial class RTInstructionCollection
    {
        private void Init()
        {

            _instrs["FBDADD"] = new RTInstructionInfo("FBDADD", DINT.Inst, new List<IDataType>
            {
                FBD_MATH.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDSUB"] = new RTInstructionInfo("FBDSUB", DINT.Inst, new List<IDataType>
            {
                FBD_MATH.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDMUL"] = new RTInstructionInfo("FBDMUL", DINT.Inst, new List<IDataType>
            {
                FBD_MATH.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDDIV"] = new RTInstructionInfo("FBDDIV", DINT.Inst, new List<IDataType>
            {
                FBD_MATH.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDMOD"] = new RTInstructionInfo("FBDMOD", DINT.Inst, new List<IDataType>
            {
                FBD_MATH.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDNEG"] = new RTInstructionInfo("FBDNEG", DINT.Inst, new List<IDataType>
            {
                FBD_MATH_ADVANCED.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDNOT"] = new RTInstructionInfo("FBDNOT", DINT.Inst, new List<IDataType>
            {
                FBD_CONVERT.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["ALMD"] = new RTInstructionInfo("ALMD", DINT.Inst, new List<IDataType>
            {
                ALARM_DIGITAL.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["ALMA"] = new RTInstructionInfo("ALMA", DINT.Inst, new List<IDataType>
            {
                ALARM_ANALOG.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["LN"] = new RTInstructionInfo("LN", REAL.Inst, new List<IDataType>
            {
                REAL.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["FBDLN"] = new RTInstructionInfo("FBDLN", DINT.Inst, new List<IDataType>
            {
                FBD_MATH_ADVANCED.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["LOGF"] = new RTInstructionInfo("LOGF", REAL.Inst, new List<IDataType>
            {
                REAL.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["FBDLOG"] = new RTInstructionInfo("FBDLOG", DINT.Inst, new List<IDataType>
            {
                FBD_MATH_ADVANCED.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["XPY"] = new RTInstructionInfo("XPY", REAL.Inst, new List<IDataType>
            {
                REAL.Inst,
                REAL.Inst,
            },
            new List<bool>
            {
                false,
                false,
            });
            _instrs["COP"] = new RTInstructionInfo("COP", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                true,
                true,
                true,
                true,
                true,
                true,
                false,
            });
            _instrs["CPS"] = new RTInstructionInfo("CPS", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                true,
                true,
                true,
                true,
                true,
                true,
                false,
            });
            _instrs["LOWER"] = new RTInstructionInfo("LOWER", DINT.Inst, new List<IDataType>
            {
                STRING.Inst,
                DINT.Inst,
                STRING.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                true,
                false,
                true,
                false,
            });
            _instrs["UPPER"] = new RTInstructionInfo("UPPER", DINT.Inst, new List<IDataType>
            {
                STRING.Inst,
                DINT.Inst,
                STRING.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                true,
                false,
                true,
                false,
            });
            _instrs["STOD"] = new RTInstructionInfo("STOD", DINT.Inst, new List<IDataType>
            {
                STRING.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                true,
                false,
            });
            _instrs["DTOS"] = new RTInstructionInfo("DTOS", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                STRING.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                false,
            });
            _instrs["STOR"] = new RTInstructionInfo("STOR", REAL.Inst, new List<IDataType>
            {
                STRING.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                true,
                false,
            });
            _instrs["RTOS"] = new RTInstructionInfo("RTOS", DINT.Inst, new List<IDataType>
            {
                REAL.Inst,
                STRING.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                false,
            });
            _instrs["FIND"] = new RTInstructionInfo("FIND", DINT.Inst, new List<IDataType>
            {
                STRING.Inst,
                DINT.Inst,
                STRING.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                true,
                false,
                true,
                false,
                false,
                true,
            });
            _instrs["INSERT"] = new RTInstructionInfo("INSERT", DINT.Inst, new List<IDataType>
            {
                STRING.Inst,
                DINT.Inst,
                STRING.Inst,
                DINT.Inst,
                DINT.Inst,
                STRING.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                true,
                false,
                true,
                false,
                false,
                true,
                false,
            });
            _instrs["MID"] = new RTInstructionInfo("MID", DINT.Inst, new List<IDataType>
            {
                STRING.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                STRING.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                true,
                false,
                false,
                false,
                true,
                false,
            });
            _instrs["CONCAT"] = new RTInstructionInfo("CONCAT", DINT.Inst, new List<IDataType>
            {
                STRING.Inst,
                DINT.Inst,
                STRING.Inst,
                DINT.Inst,
                STRING.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                true,
                false,
                true,
                false,
                true,
                false,
            });
            _instrs["DELETE"] = new RTInstructionInfo("DELETE", DINT.Inst, new List<IDataType>
            {
                STRING.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                STRING.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                true,
                false,
                false,
                false,
                true,
                false,
            });
            _instrs["XIC"] = new RTInstructionInfo("XIC", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                false,
            });
            _instrs["XIO"] = new RTInstructionInfo("XIO", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                false,
            });
            _instrs["ONS"] = new RTInstructionInfo("ONS", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                false,
                true,
            });
            _instrs["OSF"] = new RTInstructionInfo("OSF", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                false,
                true,
                true,
            });
            _instrs["OSR"] = new RTInstructionInfo("OSR", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                false,
                true,
                true,
            });
            _instrs["OTE"] = new RTInstructionInfo("OTE", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
            });
            _instrs["OTL"] = new RTInstructionInfo("OTL", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
            });
            _instrs["OTU"] = new RTInstructionInfo("OTU", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
            });
            _instrs["OSRI"] = new RTInstructionInfo("OSRI", DINT.Inst, new List<IDataType>
            {
                FBD_ONESHOT.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["OSFI"] = new RTInstructionInfo("OSFI", DINT.Inst, new List<IDataType>
            {
                FBD_ONESHOT.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["INTG"] = new RTInstructionInfo("INTG", DINT.Inst, new List<IDataType>
            {
                INTEGRATOR.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["PI"] = new RTInstructionInfo("PI", DINT.Inst, new List<IDataType>
            {
                PROP_INT.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["PMUL"] = new RTInstructionInfo("PMUL", DINT.Inst, new List<IDataType>
            {
                PULSE_MULTIPLIER.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["SCRV"] = new RTInstructionInfo("SCRV", DINT.Inst, new List<IDataType>
            {
                S_CURVE.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["SOC"] = new RTInstructionInfo("SOC", DINT.Inst, new List<IDataType>
            {
                SEC_ORDER_CONTROLLER.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["UPDN"] = new RTInstructionInfo("UPDN", DINT.Inst, new List<IDataType>
            {
                UP_DOWN_ACCUM.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDHMIBC"] = new RTInstructionInfo("FBDHMIBC", DINT.Inst, new List<IDataType>
            {
                HMIBC.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["DERV"] = new RTInstructionInfo("DERV", DINT.Inst, new List<IDataType>
            {
                DERIVATIVE.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["HPF"] = new RTInstructionInfo("HPF", DINT.Inst, new List<IDataType>
            {
                FILTER_HIGH_PASS.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["LPF"] = new RTInstructionInfo("LPF", DINT.Inst, new List<IDataType>
            {
                FILTER_LOW_PASS.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["NTCH"] = new RTInstructionInfo("NTCH", DINT.Inst, new List<IDataType>
            {
                FILTER_NOTCH.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["LDL2"] = new RTInstructionInfo("LDL2", DINT.Inst, new List<IDataType>
            {
                LEAD_LAG_SEC_ORDER.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["SQRTF"] = new RTInstructionInfo("SQRTF", REAL.Inst, new List<IDataType>
            {
                REAL.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["SQRTI"] = new RTInstructionInfo("SQRTI", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["IABS"] = new RTInstructionInfo("IABS", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["LABS"] = new RTInstructionInfo("LABS", LINT.Inst, new List<IDataType>
            {
                LINT.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["FABS"] = new RTInstructionInfo("FABS", REAL.Inst, new List<IDataType>
            {
                REAL.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["DABS"] = new RTInstructionInfo("DABS", LREAL.Inst, new List<IDataType>
            {
                LREAL.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["FBDSQRT"] = new RTInstructionInfo("FBDSQRT", DINT.Inst, new List<IDataType>
            {
                FBD_MATH_ADVANCED.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDABS"] = new RTInstructionInfo("FBDABS", DINT.Inst, new List<IDataType>
            {
                FBD_MATH_ADVANCED.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDXPY"] = new RTInstructionInfo("FBDXPY", DINT.Inst, new List<IDataType>
            {
                FBD_MATH.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["MVMT"] = new RTInstructionInfo("MVMT", DINT.Inst, new List<IDataType>
            {
                FBD_MASKED_MOVE.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["BTDT"] = new RTInstructionInfo("BTDT", DINT.Inst, new List<IDataType>
            {
                FBD_BIT_FIELD_DISTRIBUTE.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["SWPBW"] = new RTInstructionInfo("SWPBW", INT.Inst, new List<IDataType>
            {
                INT.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["SWPBD"] = new RTInstructionInfo("SWPBD", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                false,
            });
            _instrs["FBDAND"] = new RTInstructionInfo("FBDAND", DINT.Inst, new List<IDataType>
            {
                FBD_LOGICAL.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDOR"] = new RTInstructionInfo("FBDOR", DINT.Inst, new List<IDataType>
            {
                FBD_LOGICAL.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDXOR"] = new RTInstructionInfo("FBDXOR", DINT.Inst, new List<IDataType>
            {
                FBD_LOGICAL.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["DEGF"] = new RTInstructionInfo("DEGF", REAL.Inst, new List<IDataType>
            {
                REAL.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["DEGI"] = new RTInstructionInfo("DEGI", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["FBDDEG"] = new RTInstructionInfo("FBDDEG", DINT.Inst, new List<IDataType>
            {
                FBD_MATH_ADVANCED.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["RADF"] = new RTInstructionInfo("RADF", REAL.Inst, new List<IDataType>
            {
                REAL.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["RADI"] = new RTInstructionInfo("RADI", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["FBDRAD"] = new RTInstructionInfo("FBDRAD", DINT.Inst, new List<IDataType>
            {
                FBD_MATH_ADVANCED.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["TRUNCF"] = new RTInstructionInfo("TRUNCF", REAL.Inst, new List<IDataType>
            {
                REAL.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["TRUNCI"] = new RTInstructionInfo("TRUNCI", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["TRUNC"] = new RTInstructionInfo("TRUNC", DINT.Inst, new List<IDataType>
            {
                REAL.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["FBDTRUNC"] = new RTInstructionInfo("FBDTRUNC", DINT.Inst, new List<IDataType>
            {
                FBD_MATH_ADVANCED.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FRD"] = new RTInstructionInfo("FRD", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["FBDFRD"] = new RTInstructionInfo("FBDFRD", DINT.Inst, new List<IDataType>
            {
                FBD_CONVERT.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["TOD"] = new RTInstructionInfo("TOD", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["FBDTOD"] = new RTInstructionInfo("FBDTOD", DINT.Inst, new List<IDataType>
            {
                FBD_CONVERT.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["MAW"] = new RTInstructionInfo("MAW", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_CIP_DRIVE.Inst,
                MOTION_INSTRUCTION.Inst,
                DINT.Inst,
                REAL.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
                false,
                false,
            });
            _instrs["MDW"] = new RTInstructionInfo("MDW", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_CIP_DRIVE.Inst,
                MOTION_INSTRUCTION.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
            });
            _instrs["MAR"] = new RTInstructionInfo("MAR", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_CIP_DRIVE.Inst,
                MOTION_INSTRUCTION.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                REAL.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
                false,
                false,
                false,
                false,
                false,
            });
            _instrs["MDR"] = new RTInstructionInfo("MDR", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_CIP_DRIVE.Inst,
                MOTION_INSTRUCTION.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
                false,
            });
            _instrs["MAOC"] = new RTInstructionInfo("MAOC", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_COMMON.Inst,
                DINT.Inst,
                MOTION_INSTRUCTION.Inst,
                DINT.Inst,
                DINT.Inst,
                OUTPUT_CAM.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                REAL.Inst,
                OUTPUT_COMPENSATION.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                REAL.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                false,
                true,
                true,
                false,
                true,
                false,
                false,
                false,
                false,
                true,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
            });
            _instrs["MDOC"] = new RTInstructionInfo("MDOC", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_COMMON.Inst,
                DINT.Inst,
                MOTION_INSTRUCTION.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                false,
                true,
                false,
            });
            _instrs["MGS"] = new RTInstructionInfo("MGS", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                MOTION_GROUP.Inst,
                MOTION_INSTRUCTION.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
                false,
            });
            _instrs["MGSD"] = new RTInstructionInfo("MGSD", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                MOTION_GROUP.Inst,
                MOTION_INSTRUCTION.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
            });
            _instrs["MGSR"] = new RTInstructionInfo("MGSR", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                MOTION_GROUP.Inst,
                MOTION_INSTRUCTION.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
            });
            _instrs["MGSP"] = new RTInstructionInfo("MGSP", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                MOTION_GROUP.Inst,
                MOTION_INSTRUCTION.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
            });
            _instrs["MAS"] = new RTInstructionInfo("MAS", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_COMMON.Inst,
                MOTION_INSTRUCTION.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
            });
            _instrs["MAH"] = new RTInstructionInfo("MAH", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_COMMON.Inst,
                MOTION_INSTRUCTION.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
            });
            _instrs["MAJ"] = new RTInstructionInfo("MAJ", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_COMMON.Inst,
                MOTION_INSTRUCTION.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
            });
            _instrs["MAM"] = new RTInstructionInfo("MAM", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_COMMON.Inst,
                MOTION_INSTRUCTION.Inst,
                DINT.Inst,
                REAL.Inst,
                REAL.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                true,
                false,
                false,
                true,
                false,
                false,
            });
            _instrs["MAG"] = new RTInstructionInfo("MAG", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_COMMON.Inst,
                AXIS_COMMON.Inst,
                MOTION_INSTRUCTION.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
                true,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
            });
            _instrs["MCD"] = new RTInstructionInfo("MCD", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_COMMON.Inst,
                MOTION_INSTRUCTION.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
            });
            _instrs["MRP"] = new RTInstructionInfo("MRP", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_COMMON.Inst,
                MOTION_INSTRUCTION.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
                false,
                false,
                false,
            });
            _instrs["MCCP"] = new RTInstructionInfo("MCCP", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                MOTION_INSTRUCTION.Inst,
                CAM.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                REAL.Inst,
                CAM_PROFILE.Inst,
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
                false,
                false,
                false,
                false,
                false,
                true,
                false,
                false,
            });
            _instrs["MCSV"] = new RTInstructionInfo("MCSV", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                MOTION_INSTRUCTION.Inst,
                CAM_PROFILE.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                REAL.Inst,
                REAL.Inst,
                REAL.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
                false,
                false,
                false,
                true,
                true,
                true,
            });
            _instrs["MAPC"] = new RTInstructionInfo("MAPC", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_COMMON.Inst,
                AXIS_COMMON.Inst,
                MOTION_INSTRUCTION.Inst,
                DINT.Inst,
                CAM_PROFILE.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
                true,
                false,
                true,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
            });
            _instrs["MATC"] = new RTInstructionInfo("MATC", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_COMMON.Inst,
                MOTION_INSTRUCTION.Inst,
                DINT.Inst,
                CAM_PROFILE.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
                false,
                true,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
            });
            _instrs["MDAC"] = new RTInstructionInfo("MDAC", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_COMMON.Inst,
                AXIS_COMMON.Inst,
                MOTION_INSTRUCTION.Inst,
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
                true,
                false,
                false,
            });
            _instrs["MSO"] = new RTInstructionInfo("MSO", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_COMMON.Inst,
                MOTION_INSTRUCTION.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
            });
            _instrs["MSF"] = new RTInstructionInfo("MSF", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_COMMON.Inst,
                MOTION_INSTRUCTION.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
            });
            _instrs["MASD"] = new RTInstructionInfo("MASD", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_COMMON.Inst,
                MOTION_INSTRUCTION.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
            });
            _instrs["MASR"] = new RTInstructionInfo("MASR", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_COMMON.Inst,
                MOTION_INSTRUCTION.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
            });
            _instrs["MAFR"] = new RTInstructionInfo("MAFR", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_COMMON.Inst,
                MOTION_INSTRUCTION.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
            });
            _instrs["MDO"] = new RTInstructionInfo("MDO", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_COMMON.Inst,
                MOTION_INSTRUCTION.Inst,
                REAL.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
                false,
                false,
            });
            _instrs["MDF"] = new RTInstructionInfo("MDF", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_COMMON.Inst,
                MOTION_INSTRUCTION.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
            });
            _instrs["MDS"] = new RTInstructionInfo("MDS", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_CIP_DRIVE.Inst,
                MOTION_INSTRUCTION.Inst,
                REAL.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
                false,
                false,
            });
            _instrs["MAAT"] = new RTInstructionInfo("MAAT", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_COMMON.Inst,
                MOTION_INSTRUCTION.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
            });
            _instrs["MRAT"] = new RTInstructionInfo("MRAT", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_COMMON.Inst,
                MOTION_INSTRUCTION.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
            });
            _instrs["MAHD"] = new RTInstructionInfo("MAHD", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_COMMON.Inst,
                MOTION_INSTRUCTION.Inst,
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
                false,
                false,
            });
            _instrs["MRHD"] = new RTInstructionInfo("MRHD", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                AXIS_CIP_DRIVE.Inst,
                MOTION_INSTRUCTION.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
                false,
            });
            _instrs["MCCD"] = new RTInstructionInfo("MCCD", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                COORDINATE_SYSTEM.Inst,
                MOTION_INSTRUCTION.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
            });
            _instrs["MCCM"] = new RTInstructionInfo("MCCM", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                COORDINATE_SYSTEM.Inst,
                MOTION_INSTRUCTION.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                REAL.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
                false,
                true,
                false,
                false,
                false,
                true,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                true,
                false,
                false,
                true,
                false,
                false,
            });
            _instrs["MCLM"] = new RTInstructionInfo("MCLM", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                COORDINATE_SYSTEM.Inst,
                MOTION_INSTRUCTION.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                REAL.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
                false,
                true,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                true,
                false,
                false,
                true,
                false,
                false,
            });
            _instrs["MCS"] = new RTInstructionInfo("MCS", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                COORDINATE_SYSTEM.Inst,
                MOTION_INSTRUCTION.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
            });
            _instrs["MCSD"] = new RTInstructionInfo("MCSD", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                COORDINATE_SYSTEM.Inst,
                MOTION_INSTRUCTION.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
            });
            _instrs["MCSR"] = new RTInstructionInfo("MCSR", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                COORDINATE_SYSTEM.Inst,
                MOTION_INSTRUCTION.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
            });
            _instrs["MCT"] = new RTInstructionInfo("MCT", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                COORDINATE_SYSTEM.Inst,
                COORDINATE_SYSTEM.Inst,
                MOTION_INSTRUCTION.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
                true,
                true,
                false,
                false,
                true,
                false,
                false,
            });
            _instrs["MCTP"] = new RTInstructionInfo("MCTP", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                COORDINATE_SYSTEM.Inst,
                COORDINATE_SYSTEM.Inst,
                MOTION_INSTRUCTION.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
                true,
                true,
                false,
                false,
                true,
                false,
                false,
                false,
                true,
                false,
                false,
                true,
                false,
                false,
            });
            _instrs["MDCC"] = new RTInstructionInfo("MDCC", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                COORDINATE_SYSTEM.Inst,
                AXIS_COMMON.Inst,
                MOTION_INSTRUCTION.Inst,
                DINT.Inst,
                REAL.Inst,
            },
            new List<bool>
            {
                false,
                true,
                true,
                true,
                false,
                false,
            });
            _instrs["DFF"] = new RTInstructionInfo("DFF", DINT.Inst, new List<IDataType>
            {
                FLIP_FLOP_D.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["JKFF"] = new RTInstructionInfo("JKFF", DINT.Inst, new List<IDataType>
            {
                FLIP_FLOP_JK.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["RESD"] = new RTInstructionInfo("RESD", DINT.Inst, new List<IDataType>
            {
                DOMINANT_RESET.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["SETD"] = new RTInstructionInfo("SETD", DINT.Inst, new List<IDataType>
            {
                DOMINANT_SET.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["TON"] = new RTInstructionInfo("TON", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                TIMER.Inst,
            },
            new List<bool>
            {
                false,
                true,
            });
            _instrs["TOF"] = new RTInstructionInfo("TOF", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                TIMER.Inst,
            },
            new List<bool>
            {
                false,
                true,
            });
            _instrs["RTO"] = new RTInstructionInfo("RTO", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                TIMER.Inst,
            },
            new List<bool>
            {
                false,
                true,
            });
            _instrs["TONR"] = new RTInstructionInfo("TONR", DINT.Inst, new List<IDataType>
            {
                FBD_TIMER.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["TOFR"] = new RTInstructionInfo("TOFR", DINT.Inst, new List<IDataType>
            {
                FBD_TIMER.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["RTOR"] = new RTInstructionInfo("RTOR", DINT.Inst, new List<IDataType>
            {
                FBD_TIMER.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["CTUD"] = new RTInstructionInfo("CTUD", DINT.Inst, new List<IDataType>
            {
                FBD_COUNTER.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["CTD"] = new RTInstructionInfo("CTD", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                COUNTER.Inst,
            },
            new List<bool>
            {
                false,
                true,
            });
            _instrs["CTU"] = new RTInstructionInfo("CTU", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                COUNTER.Inst,
            },
            new List<bool>
            {
                false,
                true,
            });
            _instrs["SINF"] = new RTInstructionInfo("SINF", REAL.Inst, new List<IDataType>
            {
                REAL.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["ATANF"] = new RTInstructionInfo("ATANF", REAL.Inst, new List<IDataType>
            {
                REAL.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["COSF"] = new RTInstructionInfo("COSF", REAL.Inst, new List<IDataType>
            {
                REAL.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["TANF"] = new RTInstructionInfo("TANF", REAL.Inst, new List<IDataType>
            {
                REAL.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["ASINF"] = new RTInstructionInfo("ASINF", REAL.Inst, new List<IDataType>
            {
                REAL.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["ACOSF"] = new RTInstructionInfo("ACOSF", REAL.Inst, new List<IDataType>
            {
                REAL.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["POW"] = new RTInstructionInfo("POW", LREAL.Inst, new List<IDataType>
            {
                LREAL.Inst,
                LREAL.Inst,
            },
            new List<bool>
            {
                false,
                false,
            });
            _instrs["FBDCOS"] = new RTInstructionInfo("FBDCOS", DINT.Inst, new List<IDataType>
            {
                FBD_MATH_ADVANCED.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDSIN"] = new RTInstructionInfo("FBDSIN", DINT.Inst, new List<IDataType>
            {
                FBD_MATH_ADVANCED.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDTAN"] = new RTInstructionInfo("FBDTAN", DINT.Inst, new List<IDataType>
            {
                FBD_MATH_ADVANCED.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDACOS"] = new RTInstructionInfo("FBDACOS", DINT.Inst, new List<IDataType>
            {
                FBD_MATH_ADVANCED.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDASIN"] = new RTInstructionInfo("FBDASIN", DINT.Inst, new List<IDataType>
            {
                FBD_MATH_ADVANCED.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDATAN"] = new RTInstructionInfo("FBDATAN", DINT.Inst, new List<IDataType>
            {
                FBD_MATH_ADVANCED.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["ALM"] = new RTInstructionInfo("ALM", DINT.Inst, new List<IDataType>
            {
                ALARM.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDCC"] = new RTInstructionInfo("FBDCC", DINT.Inst, new List<IDataType>
            {
                CC.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDIMC"] = new RTInstructionInfo("FBDIMC", DINT.Inst, new List<IDataType>
            {
                IMC.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDMMC"] = new RTInstructionInfo("FBDMMC", DINT.Inst, new List<IDataType>
            {
                MMC.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["POSP"] = new RTInstructionInfo("POSP", DINT.Inst, new List<IDataType>
            {
                POSITION_PROP.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["SCL"] = new RTInstructionInfo("SCL", DINT.Inst, new List<IDataType>
            {
                SCALE.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["SRTP"] = new RTInstructionInfo("SRTP", DINT.Inst, new List<IDataType>
            {
                SPLIT_RANGE.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["TOT"] = new RTInstructionInfo("TOT", DINT.Inst, new List<IDataType>
            {
                TOTALIZER.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["PIDE"] = new RTInstructionInfo("PIDE", DINT.Inst, new List<IDataType>
            {
                PID_ENHANCED.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["LDLG"] = new RTInstructionInfo("LDLG", DINT.Inst, new List<IDataType>
            {
                LEAD_LAG.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["DEDT"] = new RTInstructionInfo("DEDT", DINT.Inst, new List<IDataType>
            {
                DEADTIME.Inst,
                REAL.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                true,
                true,
                false,
            });
            _instrs["FGEN"] = new RTInstructionInfo("FGEN", DINT.Inst, new List<IDataType>
            {
                FUNCTION_GENERATOR.Inst,
                REAL.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                true,
                true,
                false,
                true,
                false,
                true,
                false,
                true,
                false,
            });
            _instrs["ESEL"] = new RTInstructionInfo("ESEL", DINT.Inst, new List<IDataType>
            {
                SELECT_ENHANCED.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["HLL"] = new RTInstructionInfo("HLL", DINT.Inst, new List<IDataType>
            {
                HL_LIMIT.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["MUX"] = new RTInstructionInfo("MUX", DINT.Inst, new List<IDataType>
            {
                MULTIPLEXER.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["RLIM"] = new RTInstructionInfo("RLIM", DINT.Inst, new List<IDataType>
            {
                RATE_LIMITER.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["SEL"] = new RTInstructionInfo("SEL", DINT.Inst, new List<IDataType>
            {
                SELECT.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["SNEG"] = new RTInstructionInfo("SNEG", DINT.Inst, new List<IDataType>
            {
                SELECTABLE_NEGATE.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["SSUM"] = new RTInstructionInfo("SSUM", DINT.Inst, new List<IDataType>
            {
                SELECTED_SUMMER.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["MAXC"] = new RTInstructionInfo("MAXC", DINT.Inst, new List<IDataType>
            {
                MAXIMUM_CAPTURE.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["MINC"] = new RTInstructionInfo("MINC", DINT.Inst, new List<IDataType>
            {
                MINIMUM_CAPTURE.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["MAVE"] = new RTInstructionInfo("MAVE", DINT.Inst, new List<IDataType>
            {
                MOVING_AVERAGE.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                true,
                true,
                false,
                false,
                true,
                false,
                false,
            });
            _instrs["MSTD"] = new RTInstructionInfo("MSTD", DINT.Inst, new List<IDataType>
            {
                MOVING_STD_DEV.Inst,
                REAL.Inst,
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                true,
                true,
                false,
                false,
            });
            _instrs["MSG"] = new RTInstructionInfo("MSG", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                MESSAGE.Inst,
            },
            new List<bool>
            {
                false,
                true,
            });
            _instrs["GSV"] = new RTInstructionInfo("GSV", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                false,
                true,
                true,
                true,
            });
            _instrs["SSV"] = new RTInstructionInfo("SSV", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
                DINT.Inst,
            },
            new List<bool>
            {
                false,
                true,
                false,
                true,
                true,
                true,
            });
            _instrs["UID"] = new RTInstructionInfo("UID", DINT.Inst, new List<IDataType>
            {
            },
            new List<bool>
            {
            });
            _instrs["UIE"] = new RTInstructionInfo("UIE", DINT.Inst, new List<IDataType>
            {
            },
            new List<bool>
            {
            });
            _instrs["SYSLOAD"] = new RTInstructionInfo("SYSLOAD", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["SYSOP"] = new RTInstructionInfo("SYSOP", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["SYSSTATUS"] = new RTInstructionInfo("SYSSTATUS", DINT.Inst, new List<IDataType>
            {
                DINT.Inst,
            },
            new List<bool>
            {
                false,
            });
            _instrs["D2SD"] = new RTInstructionInfo("D2SD", DINT.Inst, new List<IDataType>
            {
                DISCRETE_2STATE.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["D3SD"] = new RTInstructionInfo("D3SD", DINT.Inst, new List<IDataType>
            {
                DISCRETE_3STATE.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDLIM"] = new RTInstructionInfo("FBDLIM", DINT.Inst, new List<IDataType>
            {
                FBD_LIMIT.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDMEQ"] = new RTInstructionInfo("FBDMEQ", DINT.Inst, new List<IDataType>
            {
                FBD_MASK_EQUAL.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDEQU"] = new RTInstructionInfo("FBDEQU", DINT.Inst, new List<IDataType>
            {
                FBD_COMPARE.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDNEQ"] = new RTInstructionInfo("FBDNEQ", DINT.Inst, new List<IDataType>
            {
                FBD_COMPARE.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDLES"] = new RTInstructionInfo("FBDLES", DINT.Inst, new List<IDataType>
            {
                FBD_COMPARE.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDGRT"] = new RTInstructionInfo("FBDGRT", DINT.Inst, new List<IDataType>
            {
                FBD_COMPARE.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDLEQ"] = new RTInstructionInfo("FBDLEQ", DINT.Inst, new List<IDataType>
            {
                FBD_COMPARE.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDGEQ"] = new RTInstructionInfo("FBDGEQ", DINT.Inst, new List<IDataType>
            {
                FBD_COMPARE.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDBAND"] = new RTInstructionInfo("FBDBAND", DINT.Inst, new List<IDataType>
            {
                FBD_BOOLEAN_AND.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDBOR"] = new RTInstructionInfo("FBDBOR", DINT.Inst, new List<IDataType>
            {
                FBD_BOOLEAN_OR.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDBXOR"] = new RTInstructionInfo("FBDBXOR", DINT.Inst, new List<IDataType>
            {
                FBD_BOOLEAN_XOR.Inst,
            },
            new List<bool>
            {
                true,
            });
            _instrs["FBDBNOT"] = new RTInstructionInfo("FBDBNOT", DINT.Inst, new List<IDataType>
            {
                FBD_BOOLEAN_NOT.Inst,
            },
            new List<bool>
            {
                true,
            });
        }
    }
}
