using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ICSStudio.DownloadChange
{
    internal class ProjectInfoExtractor
    {
        private JObject _controllerProperties;
        private JArray _controllerTags;

        public ProjectInfoExtractor(JObject projectInfo)
        {
            ProjectInfo = projectInfo;
        }

        public JObject ProjectInfo { get; }

        public JObject ControllerProperties
        {
            get
            {
                if (_controllerProperties != null)
                    return _controllerProperties;

                _controllerProperties = ExtractControllerProperties(ProjectInfo);
                return _controllerProperties;
            }
        }

        public JArray ControllerTags
        {
            get
            {
                if (_controllerTags != null)
                    return _controllerTags;

                _controllerTags = ExtractTags(ProjectInfo);
                return _controllerTags;
            }
        }

        private JArray _dataTypes;

        public JArray DataTypes
        {
            get
            {
                if (_dataTypes != null)
                    return _dataTypes;

                _dataTypes = ExtractDataTypes(ProjectInfo);

                return _dataTypes;
            }
        }

        private JArray _aoiDefinitions;

        public JArray AOIDefinitions
        {
            get
            {
                if (_aoiDefinitions != null)
                    return _aoiDefinitions;

                _aoiDefinitions = ExtractAOIDefinitions(ProjectInfo);

                return _aoiDefinitions;
            }
        }

        private JArray _tasks;

        public JArray Tasks
        {
            get
            {
                if (_tasks != null)
                    return _tasks;

                _tasks = ExtractTasks(ProjectInfo);

                return _tasks;
            }
        }

        private JArray _programs;

        public JArray Programs
        {
            get
            {
                if (_programs != null)
                    return _programs;

                _programs = ExtractPrograms(ProjectInfo);

                return _programs;
            }
        }

        private JArray ExtractPrograms(JObject jObject)
        {
            JArray programs = null;

            if (jObject.ContainsKey("Programs"))
            {
                programs = jObject.GetValue("Programs") as JArray;
            }

            return programs ?? new JArray();
        }

        private JArray _modules;

        public JArray Modules
        {
            get
            {
                if (_modules != null)
                    return _modules;

                _modules = ExtractModules(ProjectInfo);

                return _modules;
            }
        }

        private JArray ExtractModules(JObject jObject)
        {
            JArray modules = null;
            JArray sortedModules = new JArray();

            if (jObject.ContainsKey("Modules"))
            {
                modules = jObject.GetValue("Modules") as JArray;
            }

            if (modules != null)
            {
                foreach (var module in modules.OfType<JObject>())
                {
                    sortedModules.Add(Utils.Utils.SortJObject(TrimModule(module)));
                }
            }

            return sortedModules;
        }

        private JObject TrimModule(JObject module)
        {
            if (module.ContainsKey("Description"))
            {
                module.Remove("Description");
            }

            // remove input and output tag date
            if (module.ContainsKey("Communications"))
            {
                JObject communications = module["Communications"] as JObject;

                if (communications != null && communications.ContainsKey("ConfigTag"))
                {
                    JObject configTag = communications["ConfigTag"] as JObject;
                    if (configTag != null)
                    {
                        if (configTag.ContainsKey("Comments"))
                        {
                            configTag.Remove("Comments");
                        }
                    }
                }

                if (communications != null && communications.ContainsKey("Connections"))
                {
                    JArray connections = communications["Connections"] as JArray;
                    if (connections != null && connections.Count > 0)
                    {
                        JObject connection = connections[0] as JObject;

                        if (connection != null)
                        {
                            if (connection.ContainsKey("InputTag"))
                            {
                                JObject inputTag = connection["InputTag"] as JObject;
                                if (inputTag != null)
                                {
                                    if (inputTag.ContainsKey("Data"))
                                        inputTag.Remove("Data");

                                    if (inputTag.ContainsKey("Comments"))
                                        inputTag.Remove("Comments");
                                }

                                JObject outputTag = connection["OutputTag"] as JObject;
                                if (outputTag != null)
                                {
                                    if (outputTag.ContainsKey("Data"))
                                        outputTag.Remove("Data");

                                    if (outputTag.ContainsKey("Comments"))
                                        outputTag.Remove("Comments");
                                }
                            }
                        }
                    }
                }
            }

            return module;
        }

        private static JArray ExtractTasks(JObject jObject)
        {
            JArray tasks = null;
            JArray sortedTasks = new JArray();
            if (jObject.ContainsKey("Tasks"))
            {
                tasks = jObject.GetValue("Tasks") as JArray;
            }

            if (tasks != null)
            {
                foreach (var task in tasks.OfType<JObject>())
                {
                    sortedTasks.Add(Utils.Utils.SortJObject(TrimTask(task)));
                }
            }

            return sortedTasks;
        }

        private static JObject TrimTask(JObject task)
        {
            if (task.ContainsKey("Description"))
            {
                task.Remove("Description");
            }

            return task;
        }

        private static JArray ExtractAOIDefinitions(JObject jObject)
        {
            JArray aoiDefinitions = null;
            JArray sortedAois = new JArray();

            if (jObject.ContainsKey("AddOnInstructionDefinitions"))
            {
                aoiDefinitions = jObject.GetValue("AddOnInstructionDefinitions") as JArray;
            }

            if (aoiDefinitions != null)
            {
                foreach (var aoiDefinition in aoiDefinitions.OfType<JObject>())
                {
                    sortedAois.Add(Utils.Utils.SortJObject(TrimAOIDefinition(aoiDefinition)));
                }
            }

            return sortedAois;
        }

        private static JObject TrimAOIDefinition(JObject aoiDefinition)
        {
            string[] trimPropertyNames =
            {
                "Description", "AdditionalHelpText",
                "Functions", "Pool",
                "CreatedBy", "CreatedDate",
                "EditedBy", "EditedDate"
            };

            foreach (var propertyName in trimPropertyNames)
            {
                if (aoiDefinition.ContainsKey(propertyName))
                    aoiDefinition.Remove(propertyName);
            }

            // remove description and comments
            // LocalTags
            if (aoiDefinition.ContainsKey("LocalTags"))
            {
                JArray tags = aoiDefinition["LocalTags"] as JArray;
                if (tags != null && tags.Count > 0)
                {
                    foreach (var tag in tags.OfType<JObject>())
                    {
                        TrimCommentsInTag(tag);
                    }
                }
            }

            // Parameters
            if (aoiDefinition.ContainsKey("Parameters"))
            {
                JArray tags = aoiDefinition["Parameters"] as JArray;
                if (tags != null && tags.Count > 0)
                {
                    foreach (var tag in tags.OfType<JObject>())
                    {
                        TrimCommentsInTag(tag);
                    }
                }
            }

            if (aoiDefinition.ContainsKey("Routines"))
            {
                JArray routines = aoiDefinition["Routines"] as JArray;
                if (routines != null && routines.Count > 0)
                {
                    foreach (var routine in routines.OfType<JObject>())
                    {
                        TrimRoutine(routine);
                    }
                }
            }

            return aoiDefinition;
        }

        private static void TrimRoutine(JObject routine)
        {
            if (routine.ContainsKey("Description"))
            {
                routine.Remove("Description");
            }
        }

        private static void TrimCommentsInTag(JObject tag)
        {
            if (tag.ContainsKey("Description"))
            {
                tag.Remove("Description");
            }

            if (tag.ContainsKey("Comments"))
            {
                tag.Remove("Comments");
            }
        }

        private static JArray ExtractDataTypes(JObject jObject)
        {
            JArray dataTypes = null;
            JArray sortedDataTypes = new JArray();

            if (jObject.ContainsKey("DataTypes"))
            {
                dataTypes = jObject.GetValue("DataTypes") as JArray;
            }

            if (dataTypes != null)
            {
                foreach (var dataType in dataTypes.OfType<JObject>())
                {
                    sortedDataTypes.Add(Utils.Utils.SortJObject(TrimDataType(dataType)));
                }
            }

            return sortedDataTypes;
        }

        private static JObject TrimDataType(JObject dataType)
        {
            if (dataType.ContainsKey("Description"))
            {
                dataType.Remove("Description");
            }

            if (dataType.ContainsKey("EngineeringUnit"))
            {
                dataType.Remove("EngineeringUnit");
            }

            //TODO(gjc):need check later
            if (dataType.ContainsKey("Class"))
            {
                dataType.Remove("Class");
            }

            List<string> memberRemoveKeys = new List<string>()
            {
                "Description", "EngineeringUnit", "DisplayName"
            };

            if (dataType.ContainsKey("Members"))
            {
                JArray members = dataType["Members"] as JArray;
                if (members != null)
                {
                    foreach (var member in members.OfType<JObject>())
                    {
                        foreach (string removeKey in memberRemoveKeys)
                        {
                            if (member.ContainsKey(removeKey))
                                member.Remove(removeKey);
                        }

                    }
                }
            }

            return dataType;
        }

        private static JArray ExtractTags(JObject jObject)
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
            

            string dataType = tag["DataType"].ToString();
            if (string.Equals(dataType, "AXIS_CIP_DRIVE"))
            {
                if (tag.ContainsKey("Logs"))
                {
                    tag.Remove("Logs");
                }

                if (tag.ContainsKey("Parameters"))
                {
                    JObject parameters = tag["Parameters"] as JObject;
                    if (parameters != null)
                    {
                        JObject sortedParameters = Utils.Utils.SortJObject(parameters);

                        tag["Parameters"] = sortedParameters;
                    }
                }
            }


            return tag;
        }

        private static JObject ExtractControllerProperties(JObject jObject)
        {
            JObject info = new JObject();

            if (jObject.ContainsKey("Name"))
            {
                info.Add("Name", jObject.GetValue("Name"));
            }

            if (jObject.ContainsKey("MajorFaultProgram"))
            {
                string value = jObject.GetValue("MajorFaultProgram")?.ToString();
                if (!string.IsNullOrEmpty(value))
                    info.Add("MajorFaultProgram", value);
            }

            if (jObject.ContainsKey("PowerLossProgram"))
            {
                string value = jObject.GetValue("PowerLossProgram")?.ToString();
                if (!string.IsNullOrEmpty(value))
                    info.Add("PowerLossProgram", value);
            }

            if (jObject.ContainsKey("TimeSynchronize"))
            {
                info.Add("TimeSynchronize", jObject.GetValue("TimeSynchronize"));
            }

            return info;
        }
    }
}
