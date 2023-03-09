using System;
using System.Collections.Generic;
using System.Text;

namespace ICSStudio.SimpleServices.Online
{
    public class MajorFaultInfo
    {
        public int Id { get; set; }
        public long Timestamp { get; set; }
        public int Type { get; set; }
        public int Code { get; set; }
        public string Aux { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(GetDateTime(Timestamp));
            builder.AppendLine($"(Type {Type:D2}) {GetMajorFaultType(Type)}");
            builder.AppendLine($"(Code {Code:D2}) {GetMajorFaultCause(Type, Code)}");

            if (!string.IsNullOrEmpty(Aux))
            {
                builder.AppendLine(Aux);
            }

            return builder.ToString();
        }

        private static string GetDateTime(long timeStamp)
        {
            DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timeStamp);
            return time.ToLocalTime().ToString("yyyy/MM/dd hh:mm:ss");
        }

        private static string GetMajorFaultType(int type)
        {
            switch (type)
            {
                case 1:
                    return "Power-up Fault";
                case 3:
                    return "I/O Fault";
                case 4:
                    return "Program Fault(can be trapped by a fault routine)";
                case 6:
                    return "Watchdog Fault";
                case 7:
                    return "Nonvolatile Fault";
                case 8:
                    return "Mode Change Fault";
                case 10:
                    return "Energy Storage Fault";
                case 11:
                    return "Motion Fault";
                case 13:
                    return "Real Time Clock Fault";
                case 17:
                    return "Diagnostic Fault";
                case 18:
                    return "CIP Motion Fault";
            }

            return string.Empty;
        }

        private static readonly Dictionary<long, string> MajorFaultCauseDictionary
            = new Dictionary<long, string>
            {
                { (1L << 32) | 1L, "The controller powered on in Run mode." },
                { (1L << 32) | 16L, "I/O communication configuration fault detected. (CompactLogix 1768-L4x controllers only.)" },
                { (1L << 32) | 40L, "If the controller uses a battery, then the battery does not contain enough charge to save the user program on power down." },
                { (1L << 32) | 60L, "For a controller with no memory card installed, the controller:\n.Detected a non-recoverable fault.\n.Cleared the project from memory." },
                { (1L << 32) | 61L, "For a controller with a memory card installed." },
                { (1L << 32) | 62L, "For a controller with a Secure Digital (SD) card installed." },

                { (3L << 32) | 16L, "A required I/O module connection failed." },
                { (3L << 32) | 20L, "Possible problem with the chassis." },
                { (3L << 32) | 21L, "Possible problem with the chassis." },
                { (3L << 32) | 23L, "At least one required connection was not established before going into Run mode." },

                { (4L << 32) | 16L, "Unknown instruction encountered." },
                { (4L << 32) | 20L, "Array subscript too large, or CONTROL data type POS or LEN invalid." },
                { (4L << 32) | 21L, "Control structure .LEN or .POS < 0." },
                { (4L << 32) | 31L, "The parameters of the JSR instruction do not match those of the associated SBR or RET instruction." },
                { (4L << 32) | 34L, "A timer instruction has a negative preset or accumulated value." },
                { (4L << 32) | 42L, "JMP to a label that did not exist or was deleted." },
                { (4L << 32) | 82L, "A sequential function chart (SFC) called a subroutine and the subroutine tried to jump back to the calling SFC. Occurs when the SFC uses either a JSR or FOR instruction to call the subroutine." },
                { (4L << 32) | 83L, "The data tested was not inside the required limits." },
                { (4L << 32) | 84L, "Stack overflow." },
                { (4L << 32) | 89L, "In an SFR instruction, the target routine does not contain the target step." },
                { (4L << 32) | 90L, "Using a safety instruction outside a safety task." },
                { (4L << 32) | 91L, "Equipment Phase instruction is being called from outside an Equipment Phase program." },
                { (4L << 32) | 94L, "Nesting limits exceeded." },
                { (4L << 32) | 990L, "User-defined major fault." },
                { (4L << 32) | 991L, "User-defined major fault." },
                { (4L << 32) | 992L, "User-defined major fault." },
                { (4L << 32) | 993L, "User-defined major fault." },
                { (4L << 32) | 994L, "User-defined major fault." },
                { (4L << 32) | 995L, "User-defined major fault." },
                { (4L << 32) | 996L, "User-defined major fault." },
                { (4L << 32) | 997L, "User-defined major fault." },
                { (4L << 32) | 998L, "User-defined major fault." },
                { (4L << 32) | 999L, "User-defined major fault." },

                { (6L << 32) | 1L, "Task watchdog expired." },

                { (7L << 32) | 40L, "Store to nonvolatile memory failed." },
                { (7L << 32) | 41L, "Load from nonvolatile memory failed due to controller type mismatch." },
                { (7L << 32) | 42L, "Load from nonvolatile memory failed because the firmware revision of the project in nonvolatile memory does not match the firmware revision of the controller." },
                { (7L << 32) | 43L, "Load from nonvolatile memory failed due to bad checksum." },
                { (7L << 32) | 44L, "Failed to restore processor memory." },
                { (7L << 32) | 50L, "The log file certificate can not be verified. When the controller starts up it attempts to verify the log file key/certificate combination." },

                { (8L << 32) | 1L, "Attempted to place controller in Run mode with keyswitch during download." },

                { (11L << 32) | 1L, "Actual position has exceeded positive overtravel limit." },
                { (11L << 32) | 2L, "Actual position has exceeded negative overtravel limit." },
                { (11L << 32) | 3L, "Actual position has exceeded position error tolerance." },
                { (11L << 32) | 4L, "Encoder channel A, B, or Z connection is broken." },
                { (11L << 32) | 5L, "Encoder noise event detected or the encoder signals are not in quadrature." },
                { (11L << 32) | 6L, "Drive Fault input was activated." },
                { (11L << 32) | 7L, "Synchronous connection incurred a failure." },
                { (11L << 32) | 8L, "Servo module has detected a serious hardware fault." },
                { (11L << 32) | 9L, "Asynchronous Connection has incurred a failure." },
                { (11L << 32) | 10L, "Motor fault has occurred." },
                { (11L << 32) | 11L, "Motor thermal fault has occurred." },
                { (11L << 32) | 12L, "Motor thermal fault has occurred." },
                { (11L << 32) | 13L, "SERCOS ring fault has occurred." },
                { (11L << 32) | 14L, "Drive enable input fault has occurred." },
                { (11L << 32) | 15L, "Drive phase loss fault has occurred." },
                { (11L << 32) | 16L, "Drive guard fault has occurred." },
                { (11L << 32) | 32L, "The motion task has experienced an overlap." },

                { (12L << 32) | 32L, "Power to a disqualified secondary controller has been cycled and no partner chassis or controller was found upon power up." },
                { (12L << 32) | 33L, "An unpartnered controller has been identified in the new primary chassis after a switchover." },
                { (12L << 32) | 34L, "Just after a switchover occurs, the keyswitch positions of the primary and secondary controllers are mismatched." },

                { (14L << 32) | 1L, "Safety Task watchdog expired." },
                { (14L << 32) | 2L, "An error exists in a routine of the safety task." },
                { (14L << 32) | 3L, "Safety Partner is missing." },
                { (14L << 32) | 4L, "Safety Partner is unavailable." },
                { (14L << 32) | 5L, "Safety Partner hardware is incompatible." },
                { (14L << 32) | 6L, "Safety Partner firmware is incompatible." },
                { (14L << 32) | 7L, "Safety task is inoperable." },
                { (14L << 32) | 8L, "Coordinated System Time Master (CST) not found." },
                { (14L << 32) | 9L, "Safety partner nonrecoverable controller fault." },

                { (17L << 32) | 34L, "Controller internal temperature has exceeded operating limit." },
                { (17L << 32) | 37L, "Controller has recovered from an internal temperature fault." },

                { (18L << 32) | 1L, "The CIP Motion drive has not initialized correctly." },
                { (18L << 32) | 2L, "The CIP Motion drive has not initialized correctly." },
                { (18L << 32) | 3L, "The Physical Axis Fault bit is set, indicating a fault on the physical axis." },
                { (18L << 32) | 4L, "The Physical Axis Fault bit is set, indicating fault on the physical axis." },
                { (18L << 32) | 5L, "A motion fault occurred." },
                { (18L << 32) | 6L, "A CIP Motion Drive fault has occurred." },
                { (18L << 32) | 7L, "A motion group fault has occurred." },
                { (18L << 32) | 8L, "A fault has occurred during the configuration of a CIP Motion Drive." },
                { (18L << 32) | 9L, "An Absolute Position Recovery (APR) fault has occurred and the absolute position of the axis cannot be recovered." },
                { (18L << 32) | 10L, "An Absolute Position Recovery (APR) fault has occurred and the absolute position of the axis cannot be recovered." },
                { (18L << 32) | 128L, "A fault specific to the Guard Motion safety function has occurred." }
            };

        private static string GetMajorFaultCause(int type, int code)
        {
            long key = ((long)(uint)type << 32) | (uint)code;

            if (MajorFaultCauseDictionary.ContainsKey(key))
                return MajorFaultCauseDictionary[key];

            return string.Empty;
        }
    }
}
