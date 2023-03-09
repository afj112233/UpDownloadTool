using System;
using ICSStudio.Interfaces.Common;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.EditLogs
{
    internal class SetInhibitLog : EditLog
    {
        internal SetInhibitLog(string name, bool value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public bool Value { get; set; }

        public override JObject ConvertToJObject()
        {
            JObject jObject = base.ConvertToJObject();

            jObject.Add("Name", Name);

            jObject.Add("Value", Value);

            return jObject;
        }

        public override bool CanOnlineUpdate => true;

        internal static SetInhibitLog Create(ITask task)
        {
            if (task == null)
                return null;

            SetInhibitLog setInhibitLog = new SetInhibitLog(task.Name, task.IsInhibited);

            setInhibitLog.EditTime = DateTime.Now;
            setInhibitLog.Action = "SetInhibit";
            setInhibitLog.Context = "Task";

            return setInhibitLog;
        }

        internal static SetInhibitLog Create(IProgram program)
        {
            if (program == null)
                return null;

            SetInhibitLog setInhibitLog = new SetInhibitLog(program.Name, program.Inhibited);

            setInhibitLog.EditTime = DateTime.Now;
            setInhibitLog.Action = "SetInhibit";
            setInhibitLog.Context = "Program";

            return setInhibitLog;
        }

        public void UpdateValue(bool value, DateTime editTime)
        {
            Value = value;
            EditTime = editTime;
        }
    }
}
