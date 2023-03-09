using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.EditLogs
{
    internal class ChangeAxisPropertyLog : EditLog
    {
        internal ChangeAxisPropertyLog(string tagName, string propertyName)
        {
            TagName = tagName;

            PropertyNames = new List<string> { propertyName };
        }

        internal ChangeAxisPropertyLog(string tagName, List<string> propertyNames)
        {
            TagName = tagName;

            PropertyNames = new List<string>(propertyNames);
        }

        public string TagName { get; private set; }
        public List<string> PropertyNames { get; }

        public override JObject ConvertToJObject()
        {
            JObject jObject = base.ConvertToJObject();

            jObject.Add(nameof(TagName), TagName);
            jObject.Add(nameof(PropertyNames), JArray.FromObject(PropertyNames));

            return jObject;
        }

        public override bool CanOnlineUpdate => true;

        public static ChangeAxisPropertyLog Create(string tagName, string propertyName)
        {
            ChangeAxisPropertyLog log = new ChangeAxisPropertyLog(tagName, propertyName);

            log.EditTime = DateTime.Now;
            log.Action = "ChangeAxisProperty";

            log.Context = "Ctrl";

            return log;
        }

        public void AddPropertyName(string propertyName)
        {
            if (!PropertyNames.Contains(propertyName))
            {
                PropertyNames.Add(propertyName);
                EditTime = DateTime.Now;
            }

        }
    }
}
