using System;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.EditLogs
{
    internal class SetPeriodLog : EditLog
    {
        internal SetPeriodLog(string name, float value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public float Value { get; set; }

        public override JObject ConvertToJObject()
        {
            JObject jObject = base.ConvertToJObject();

            jObject.Add("Name", Name);

            jObject.Add("Value", Value);

            return jObject;
        }

        public override bool CanOnlineUpdate => true;

        internal static SetPeriodLog Create(ITask task)
        {
            CTask myTask = task as CTask;
            if (myTask == null)
                return null;

            SetPeriodLog setPeriodLog = new SetPeriodLog(myTask.Name, myTask.Rate);

            setPeriodLog.EditTime = DateTime.Now;
            setPeriodLog.Action = "SetPeriod";
            setPeriodLog.Context = "Task";

            return setPeriodLog;
        }

        public void UpdateValue(float value, DateTime editTime)
        {
            Value = value;
            EditTime = editTime;
        }
    }
}
