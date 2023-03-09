using System;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Tags;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.EditLogs
{
    internal class AddTagLog : EditLog
    {
        internal AddTagLog(string program, string tagName)
        {
            Program = program;
            TagName = tagName;
        }

        public string Program { get; private set; }

        public string TagName { get; private set; }

        public override JObject ConvertToJObject()
        {
            JObject jObject = base.ConvertToJObject();

            if (!string.IsNullOrEmpty(Program))
            {
                jObject.Add("Program", Program);
            }

            if (!string.IsNullOrEmpty(TagName))
            {
                jObject.Add("TagName", TagName);
            }

            return jObject;
        }

        internal static AddTagLog Create(ITagCollection tagCollection, Tag tag)
        {
            AddTagLog log = new AddTagLog(string.Empty, tag.Name);

            log.EditTime = DateTime.Now;
            log.Action = "AddTag";

            if (tagCollection.IsControllerScoped)
                log.Context = "Ctrl";

            IProgram program = tagCollection.ParentProgram as IProgram;
            IAoiDefinition aoiDefinition = tagCollection.ParentProgram as IAoiDefinition;

            if (program != null)
            {
                log.Context = "Program";
                log.Program = program.Name;
            }

            if (aoiDefinition != null)
            {
                log.Context = "AOI";
                log.Program = aoiDefinition.Name;
            }
            
            return log;
        }

        public override bool CanOnlineUpdate
        {
            get
            {
                if (Context == "AOI")
                    return false;

                if (Context == "Ctrl" || Context == "Program")
                    return true;

                return false;
            }
        }

        public void UpdateTagName(string newValue)
        {
            TagName = newValue;
        }
    }
}
