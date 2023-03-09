using System.Collections.Generic;

namespace ICSStudio.Cip.Other
{
    public class CommandResultDescriptor
    {
        private static readonly Dictionary<ushort, string> CommandResultErrorCodes;
        private static readonly Dictionary<uint, string> CommandExtendedErrorCodes;

        static CommandResultDescriptor()
        {
            CommandResultErrorCodes = new Dictionary<ushort, string>
            {
                {0, "No Error"},
                {1, "Reserved for future use"},
                {2, "Reserved for future use"},
                {3, "Execution Collision"},
                {4, "Servo On State Error"},
                {5, "Servo Off State Error"},
                {6, "Drive On State Error"},
                {7, "Shutdown State Error"},
                {8, "Wrong Axis Type"},
                {9, "Overtravel Condition"},
                {10, "Master Axis Conflict"},
                {11, "Axis Not Configured"},
                {12, "Servo Message Failure"},
                {13, "Value Out Of Range"},
                {14, "Tune Process Error"},
                {15, "Test Process Error"},
                {16, "Home In Process Error"},
                {17, "Axis Mode Not Rotary"},
                {18, "Axis Type Unused"},
                {19, "Group Not Synchronized"},
                {20, "Axis In Faulted State"},
                {21, "Group In Faulted State"},
                {22, "Axis In Motion"},
                {23, "Illegal Dynamic Change"},
                {24, "Illegal Controller Mode"},
                {25, "Illegal Instruction"},
                {26, "Illegal Cam Length"},
                {27, "Illegal Cam Profile Length"},
                {28, "Illegal Cam Type"},
                {29, "Illegal Cam Order"},
                {30, "Cam Profile Being Calculated"},
                {31, "Cam Profile Being Used"},
                {32, "Cam Profile Not Calculated"},
                {33, "Position Cam Not Enabled"},
                {34, "Registration in Progress"},
                {35, "Illegal Execution Target"},
                {36, "Illegal Output Cam"},
                {37, "Illegal Output Compensation"},
                {38, "Illegal Axis Data Type"},
                {39, "Process Conflict"},
                {40, "Drive Locally Disabled"},
                {41, "Illegal Homing Config"},
                {42, "Shutdown Status Timeout"},
                {43, "Coordinate System Queue Full"},
                {44, "Circular Collinearity Error"},
                {45, "Circular Start End Error"},
                {46, "Circular R1 R2 Mismatch Error"},
                {47, "Circular Infinite Solution Error"},
                {48, "Circular No Solutions Error"},
                {49, "Circular Small R Error"},
                {50, "Coordinate System Not in Group"},
                {51, "Invalid Actual Tolerance"},
                {52, "Coordination Motion In Process Error"},
                {53, "Axis Is Inhibited"},
                {54, "Zero Max Decel"},
                {61, "Connection Conflict"},
                {62, "Transform In Progress"},
                {63, "Axis In Transform Motion"},
                {64, "Ancillary Not Supported"},
                {65, "Axis Position Overflow"},
                {66, "Error66"},
                {67, "Invalid Transform Position"},
                {68, "Transform At Origin"},
                {69, "Max Joint Velocity Exceeded"},
                {70, "Axes In Transform Must Be Linear"},
                {71, "Transform Is Canceling"},
                {72, "Max Joint Angle Exceeded"},
                {73, "Coord System Chaining Error"},
                {74, "Invalid Orientation Angle"},
                {75, "Instruction Not Supported"},
                {76, "Zero Max Decel Jerk"},
                {77, "Transform Direction Not Supported"},
                {78, "Not Allowed While Stopping"},
                {79, "Invalid Planner State"},
                {80, "Incorrect Output Connection"},
                {81, "Partial Group Shutdown Reset"},
                {82, "CIP axis in incorrect state"},
                {83, "Illegal Control Mode or Method"},
                {84, "Drive Digital Input Not Assigned"},
                {85, "Redefine Position in Process"},
                {86, "Optional attribute not supported"},
                {87, "Not Allowed While In Direct Motion"},
                {88, "Not Allowed While Planner Active"},
                {93, "MDSC Not Activated"},
                {94, "MDSC Units Conflict"},
                {95, "MDSC Lock Direction Conflict"},
                {96, "MDSC MDAC All Conflict"},
                {97, "MDSC Idle Master and Slave Moving"},
                {98, "MDSC Lock Direction Master Direction Mismatch"},
                {99, "Feature Not Supported"},
                {100, "Axis Not At Rest"},
                {101, "MDSC Calculated Data Size Error"},
                {102, "MDSC Lock While Moving"},
                {103, "MDSC Invalid Slave Speed Reduction"},
                {104, "Error104"},
                {105, "MDSC Invalid Mode Or Master Change"},
                {106, "Merge To Current Using Seconds Illegal"},
                {107, "There is not any corrective action that can be taken"},
                {108, "Motion coordinated instructions cannot contain multiplexed axes"},

                {0x2710,"MAM Not Support In Frequency Control"},
                {0x2711,"MCD Not Support In Frequency Control"}
            };

            CommandExtendedErrorCodes = new Dictionary<uint, string>
            {
                {0, ""},
                {1, ""},
                {2, ""},
                {3, ""},
                {4, ""},
                {5, ""}
            };
        }

        public static string GetErrorCodeString(ushort errorCode)
        {
            if (CommandResultErrorCodes.ContainsKey(errorCode))
                return CommandResultErrorCodes[errorCode];

            return "Unknown error";
        }



        public static string GetExtendedErrorCodeString(uint extendedErrorCode)
        {
            // TODO(gjc): add code here

            //TODO(tlm):错误代码信息完善后，完善上面的CommandExtendedErrorCodes，并释放注释代码。
            //if (CommandExtendedErrorCodes.ContainsKey(extendedErrorCode))
            //    return CommandExtendedErrorCodes[extendedErrorCode];

            return string.Empty;
        }
    }
}
