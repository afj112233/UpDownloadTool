using System.Linq;
using Newtonsoft.Json.Linq;

namespace ICSStudio.DownloadChange
{
    internal class InfoExtractor
    {
        public static JObject ExtractProgramProperties(JObject program)
        {
            JObject sortedProgram = Utils.Utils.SortJObject(program);

            if (sortedProgram.ContainsKey("Routines"))
            {
                sortedProgram.Remove("Routines");
            }

            if (sortedProgram.ContainsKey("Tags"))
            {
                sortedProgram.Remove("Tags");
            }

            //TODO(gjc): edit later
            if (sortedProgram.ContainsKey("Description"))
            {
                sortedProgram.Remove("Description");
            }

            return sortedProgram;
        }

        public static JObject TrimProgram(JObject program)
        {
            if (program.ContainsKey("NativeCode"))
            {
                program.Remove("NativeCode");
            }

            if (program.ContainsKey("MainRoutineName"))
            {
                string mainRoutineName = program["MainRoutineName"].ToString().Trim();
                if (string.IsNullOrEmpty(mainRoutineName))
                {
                    program.Remove("MainRoutineName");
                }

            }

            if (program.ContainsKey("FaultRoutineName"))
            {
                string faultRoutineName = program["FaultRoutineName"].ToString().Trim();
                if (string.IsNullOrEmpty(faultRoutineName))
                {
                    program.Remove("FaultRoutineName");
                }
            }

            return program;
        }

        public static JArray ExtractTags(JObject jObject)
        {
            JArray tags = null;
            JArray sortedTags = new JArray();

            if (jObject.ContainsKey("Tags"))
            {
                tags = jObject.GetValue("Tags") as JArray;
            }

            if (tags != null)
            {
                foreach (var tag in tags.OfType<JObject>())
                {
                    sortedTags.Add(Utils.Utils.SortJObject(TrimTag(tag)));
                }
            }

            return sortedTags;
        }

        private static JObject TrimTag(JObject tag)
        {
            string[] trimPropertyNames = new[] { "Data", "Description", "Comments", "Radix" };

            foreach (var propertyName in trimPropertyNames)
            {
                if (tag.ContainsKey(propertyName))
                {
                    tag.Remove(propertyName);
                }
            }

            return tag;
        }

        public static JArray ExtractRoutines(JObject jObject)
        {
            JArray routines = null;
            JArray sortedRoutines = new JArray();

            if (jObject.ContainsKey("Routines"))
            {
                routines = jObject.GetValue("Routines") as JArray;
            }

            if (routines != null)
            {
                foreach (var routine in routines.OfType<JObject>())
                {
                    sortedRoutines.Add(Utils.Utils.SortJObject(TrimRoutine(routine)));
                }
            }

            return sortedRoutines;

        }

        private static JObject TrimRoutine(JObject routine)
        {
            if (routine.ContainsKey("Logic"))
            {
                routine.Remove("Logic");
            }

            if (routine.ContainsKey("Prescan"))
            {
                routine.Remove("Prescan");
            }

            if (routine.ContainsKey("Pool"))
            {
                routine.Remove("Pool");
            }

            if (routine.ContainsKey("Description"))
            {
                routine.Remove("Description");
            }

            if (routine.ContainsKey("Rungs"))
            {
                JArray rungs = routine["Rungs"] as JArray;

                if (rungs != null)
                {
                    foreach (var rung in rungs.OfType<JObject>())
                    {
                        if (rung.ContainsKey("Comment"))
                        {
                            rung.Remove("Comment");
                        }
                    }
                }

            }

            return routine;
        }
    }
}
