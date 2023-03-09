using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.EditLogs
{
    internal abstract class EditLog : IEditLog
    {
        public virtual bool CanOnlineUpdate { get; }

        public DateTime EditTime { get; protected set; }

        public string Action { get; protected set; }

        public string Context { get; protected set; }

        public JObject Data { get; protected set; }

        public virtual int DoOnlineUpdate()
        {
            return -1;
        }

        public virtual JObject ConvertToJObject()
        {
            JObject jObject = new JObject
            {
                { "EditTime", EditTime },
                { "Action", Action },
                { "Context", Context }
            };

            if (Data != null)
            {
                jObject.Add("Data", Data);
            }

            return jObject;
        }

        internal static IEditLog CreateLog(JObject jObject)
        {
            string editTime = jObject["EditTime"].ToString();
            string action = jObject["Action"].ToString();
            string context = jObject["Context"].ToString();

            JObject data = null;
            if (jObject.ContainsKey("Data"))
                data = jObject["Data"] as JObject;

            string program = string.Empty;
            if (jObject.ContainsKey("Program"))
            {
                program = jObject["Program"].ToString();
            }

            string aoi = string.Empty;
            if (jObject.ContainsKey("AOI"))
            {
                aoi = jObject["AOI"].ToString();
            }

            string routine = string.Empty;
            if (jObject.ContainsKey("Routine"))
            {
                routine = jObject["Routine"].ToString();
            }

            EditLog editLog;

            switch (action)
            {
                case "AddTag":
                    string tagName = string.Empty;
                    if (jObject.ContainsKey("TagName"))
                    {
                        tagName = jObject["TagName"].ToString();
                    }

                    editLog = new AddTagLog(program, tagName);
                    break;

                case "Replace":
                    editLog = new ReplaceCodeLog(program, aoi, routine);
                    break;

                case "SetInhibit":
                    if (jObject.ContainsKey("Name") && jObject.ContainsKey("Value"))
                    {
                        string name = jObject["Name"].ToString();
                        bool value = (bool)jObject["Value"];

                        editLog = new SetInhibitLog(name, value);
                    }
                    else
                    {
                        throw new ArgumentException("Name or Value");
                    }

                    break;

                case "ChangeAxisProperty":
                    if (jObject.ContainsKey("TagName") && jObject.ContainsKey("PropertyNames"))
                    {
                        string axisName = jObject["TagName"].ToString();
                        List<string> names = jObject["PropertyNames"].ToObject<List<string>>();

                        editLog = new ChangeAxisPropertyLog(axisName, names);
                    }
                    else
                    {
                        throw new ArgumentException("TagName or PropertyNames");
                    }

                    break;

                case "SetPeriod":
                    if (jObject.ContainsKey("Name") && jObject.ContainsKey("Value"))
                    {
                        string name = jObject["Name"].ToString();
                        float value = (float)jObject["Value"];

                        editLog = new SetPeriodLog(name, value);
                    }
                    else
                    {
                        throw new ArgumentException("Name or Value");
                    }

                    break;

                default:
                    throw new NotImplementedException($"Add code for action:{action}!");
            }

            {
                editLog.EditTime = DateTime.Parse(editTime);
                editLog.Action = action;
                editLog.Context = context;
                editLog.Data = data;
            }

            return editLog;
        }
    }
}
