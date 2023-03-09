using System;
using System.Collections.Generic;

namespace ICSStudio.Cip.Other
{
    public class FaultEntry
    {
        private static readonly Dictionary<byte, string> SourceDic = new Dictionary<byte, string>
        {
            { 0, "Faults Cleared" },
            { 1, "Initialization Fault" },
            { 2, "Initialization Fault - Mfg" },
            { 3, "Axis Fault" },
            { 4, "Axis Fault - Mfg" },
            { 5, "Motion Fault" },
            { 6, "Module Fault" },
            { 7, "Group Fault" },
            { 8, "Configuration Fault" },
            { 9, "APR Fault" },
            { 10, "APR Fault - Mfg" },
            { 11, "Axis Safety Fault" },
            { 12, "Axis Safety Fault - Mfg" },
            { 128, "Guard Fault" }

        };

        //TODO(gjc): need edit later
        private static readonly Dictionary<byte, string> ConditionDic = new Dictionary<byte, string>()
        {
            { 0, "Control Sync Fault" },
            { 2, "Fault Reset" },
            { 4, "Connection Reset" },
            { 6, "Module Connection Fault" },
            { 55, "Excessive Velocity Error" },
            { 255, "Fault Log Reset" }
        };

        private static readonly Dictionary<byte, string> ActionDic = new Dictionary<byte, string>()
        {
            { 0, "No Action" },
            { 1, "Planner Stop" },
            { 2, "Ramped Stop" },
            { 3, "Torque Limited Stop" },
            { 4, "Immediate Stop(Coast)" }
        };

        private static readonly Dictionary<byte, string> EndStateDic = new Dictionary<byte, string>()
        {
            { 0, "No Action" },
            { 1, "Hold" },
            { 2, "Disabled" },
            { 3, "Shutdown" }
        };

        public byte Type { get; set; }
        public byte Code { get; set; }
        public byte SubCode { get; set; }
        public byte StopAction { get; set; }
        public byte StateChange { get; set; }
        public long Timestamp { get; set; }

        public string DateTime
        {
            get
            {
                DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddTicks(Timestamp * 10);
                return time.ToLocalTime().ToString("yyyy/MM/dd hh:mm:ss.fff");
            }
        }

        public string Source
        {
            get
            {
                if (SourceDic.ContainsKey(Type))
                    return SourceDic[Type];

                return $"Add Source: {Type}!";
            }
        }

        public string Condition
        {
            get
            {
                if (ConditionDic.ContainsKey(Code))
                    return ConditionDic[Code];

                return $"Add Condition: {Code}";
            }
        }

        public string Action
        {
            get
            {
                if (ActionDic.ContainsKey(StopAction))
                    return ActionDic[StopAction];

                return $"Add Action: {StopAction}";
            }
        }

        public string EndState
        {
            get
            {
                if (EndStateDic.ContainsKey(StateChange))
                    return EndStateDic[StateChange];

                return $"Add EndState: {StateChange}";
            }
        }
    }

    public class AlarmEntry
    {
        private static readonly Dictionary<byte, string> SourceDic = new Dictionary<byte, string>()
        {
            { 0, "No Alarms" },
            { 1, "Start Inhibit" },
            { 2, "Start Inhibit - Mfg" },
            { 3, "Axis Alarm" },
            { 4, "Axis Alarm - Mfg" },
            { 5, "Motion Alarm" },
            { 6, "Module Alarm" },
            { 7, "Group Alarm" },
            { 8, "Remote Get Alarm" },
            { 9, "Axis Safety Alarm" },
            { 10, "Axis Safety Alarm - Mfg" }
        };

        //TODO(gjc): need edit later
        private static readonly Dictionary<byte, string> ConditionDic = new Dictionary<byte, string>()
        {
            { 5, "Safe Torque Off" },
            { 255, "Alarm Log Reset" }
        };

        public byte Type { get; set; }
        public byte Code { get; set; }
        public byte SubCode { get; set; }
        public byte State { get; set; }
        public long Timestamp { get; set; }

        public string DateTime
        {
            get
            {
                DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddTicks(Timestamp * 10);
                return time.ToLocalTime().ToString("yyyy/MM/dd hh:mm:ss.fff");
            }
        }

        public string Source
        {
            get
            {
                if (SourceDic.ContainsKey(Type))
                    return SourceDic[Type];

                return $"Add Source: {Type}!";
            }
        }

        public string Condition
        {
            get
            {
                if (ConditionDic.ContainsKey(Code))
                    return ConditionDic[Code];

                return $"Add Condition: {Code}";
            }
        }

        public string Action
        {
            get
            {
                if (State == 0)
                    return "Alarm Off";

                if (State == 1)
                    return "Alarm On";

                return $"Wrong State: {State}";
            }
        }

        public string EndState => string.Empty;
    }

}
