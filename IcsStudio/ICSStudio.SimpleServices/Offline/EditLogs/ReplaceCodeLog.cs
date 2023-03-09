using System;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.EditLogs
{
    internal class ReplaceCodeLog : EditLog
    {
        public ReplaceCodeLog(string program, string aoi, string routine)
        {
            Program = program;
            AOI = aoi;
            Routine = routine;
        }

        public string Program { get; private set; }

        public string AOI { get; private set; }

        public string Routine { get; private set; }

        public override JObject ConvertToJObject()
        {
            JObject jObject = base.ConvertToJObject();

            if (!string.IsNullOrEmpty(Program))
            {
                jObject.Add("Program", Program);
            }

            if (!string.IsNullOrEmpty(AOI))
            {
                jObject.Add("AOI", AOI);
            }

            jObject.Add("Routine", Routine);

            return jObject;
        }

        public override bool CanOnlineUpdate
        {
            get
            {
                if (!string.IsNullOrEmpty(Program))
                    return true;

                if (!string.IsNullOrWhiteSpace(AOI))
                    return false;

                return false;
            }
        }

        public static ReplaceCodeLog Create(IRoutine routine)
        {
            if (routine == null)
                return null;

            ReplaceCodeLog log = new ReplaceCodeLog(string.Empty, string.Empty, string.Empty);

            log.EditTime = DateTime.Now;
            log.Action = "Replace";
            
            IProgram program = routine.ParentCollection?.ParentProgram as IProgram;
            IAoiDefinition aoiDefinition = routine.ParentCollection?.ParentProgram as IAoiDefinition;

            log.Context = "Routine";

            if (program != null)
            {
                log.Program = program.Name;
            }

            if (aoiDefinition != null)
            {
                log.AOI = aoiDefinition.Name;
            }

            log.Routine = routine.Name;

            return log;
        }

        public void UpdateEditTime(DateTime editTime)
        {
            EditTime = editTime;
        }
    }
}
