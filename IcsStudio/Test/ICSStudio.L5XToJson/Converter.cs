using System;
using System.Diagnostics;
using System.Xml;
using AutoMapper;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.Interfaces.Common;
using ICSStudio.L5XToJson.Objects;
using ICSStudio.Utils;
using Newtonsoft.Json.Linq;

namespace ICSStudio.L5XToJson
{
    public class Converter
    {
        public static JObject ToJObject(XmlDocument doc)
        {
            JObject controller = new JObject();

            var xmlNode = (XmlElement) doc.SelectSingleNode("RSLogix5000Content/Controller");
            controller.Add("Name", xmlNode?.GetAttribute("Name"));

            // Modules
            JArray modules = new JArray();
            controller.Add("Modules", modules);

            xmlNode = (XmlElement) doc.SelectSingleNode("RSLogix5000Content/Controller/Modules");
            var xmlNodeList = xmlNode?.GetElementsByTagName("Module");
            if (xmlNodeList != null)
            {
                foreach (XmlElement node in xmlNodeList)
                {
                    modules.Add(ToDeviceModule(node));
                }
            }

            // Programs
            JArray programs = new JArray();
            controller.Add("Programs", programs);

            xmlNode = (XmlElement) doc.SelectSingleNode("RSLogix5000Content/Controller/Programs");
            xmlNodeList = xmlNode?.GetElementsByTagName("Program");
            if (xmlNodeList != null)
            {
                foreach (XmlElement node in xmlNodeList)
                {
                    programs.Add(ToProgram(node));
                }
            }

            // Tags
            JArray tags = new JArray();
            controller.Add("Tags", tags);

            xmlNode = (XmlElement) doc.SelectSingleNode("RSLogix5000Content/Controller/Tags");
            xmlNodeList = xmlNode?.GetElementsByTagName("Tag");
            if (xmlNodeList != null)
            {
                foreach (XmlElement node in xmlNodeList)
                {
                    tags.Add(ToTag(node));
                }
            }

            // Tasks
            JArray tasks = new JArray();
            controller.Add("Tasks", tasks);

            xmlNode = (XmlElement) doc.SelectSingleNode("RSLogix5000Content/Controller/Tasks");
            xmlNodeList = xmlNode?.GetElementsByTagName("Task");
            if (xmlNodeList != null)
            {
                foreach (XmlElement node in xmlNodeList)
                {
                    tasks.Add(ToTask(node));
                }
            }

            // DataTypes
            JArray dataTypes = new JArray();
            controller.Add("DataTypes", dataTypes);

            xmlNode = (XmlElement) doc.SelectSingleNode("RSLogix5000Content/Controller/DataTypes");
            xmlNodeList = xmlNode?.GetElementsByTagName("DataType");
            if (xmlNodeList != null)
            {
                foreach (XmlElement node in xmlNodeList)
                {
                    dataTypes.Add(ToDataType(node));
                }
            }

            xmlNode = (XmlElement)doc.SelectSingleNode("RSLogix5000Content/Controller/AddOnInstructionDefinitions");
            xmlNodeList = xmlNode?.GetElementsByTagName("AddOnInstructionDefinition");

            JArray aoiDefinitions = new JArray();
            if (xmlNodeList != null)
            {
                foreach (XmlElement node in xmlNodeList)
                {
                    aoiDefinitions.Add(ToAoiDefinition(node));
                }
            }

            controller.Add("AddOnInstructionDefinitions", aoiDefinitions);

            JObject time = new JObject();
            xmlNode = (XmlElement)doc.SelectSingleNode("RSLogix5000Content/Controller/TimeSynchronize");
            time.Add("Priority1", int.Parse(xmlNode.Attributes["Priority1"].Value));
            time.Add("Priority2", int.Parse(xmlNode.Attributes["Priority2"].Value));
            time.Add("PTPEnable", bool.Parse(xmlNode.Attributes["PTPEnable"].Value));
            controller.Add("TimeSynchronize", time);

            return controller;
        }

        private static JObject ToAoiDefinition(XmlElement node)
        {
            JObject aoi = new JObject();
            aoi.Add("Name", node.Attributes["Name"].Value);
            aoi.Add("Revision", node.Attributes["Revision"].Value);
            aoi.Add("ExecutePrescan", bool.Parse(node.Attributes["ExecutePrescan"].Value));
            aoi.Add("ExecutePostscan", bool.Parse(node.Attributes["ExecutePostscan"].Value));
            aoi.Add("ExecuteEnableInFalse", bool.Parse(node.Attributes["ExecuteEnableInFalse"].Value));
            aoi.Add("CreatedDate", (node.Attributes["CreatedDate"].Value));
            aoi.Add("CreatedBy", (node.Attributes["CreatedBy"].Value));
            aoi.Add("EditedDate", (node.Attributes["EditedDate"].Value));
            aoi.Add("EditedBy", (node.Attributes["EditedBy"].Value));
            aoi.Add("SoftwareRevision", (node.Attributes["SoftwareRevision"].Value));

            var ps = (XmlElement)node.SelectSingleNode("Parameters");
            var paramList = ps?.GetElementsByTagName("Parameter");

            JArray parameters = new JArray();

            foreach (XmlElement p in paramList)
            {
                parameters.Add(ToAoiParameter(p));
            }

            aoi.Add("Parameters", parameters);

            var tg = (XmlElement)node.SelectSingleNode("LocalTags");
            var tagList = tg?.GetElementsByTagName("LocalTag");

            JArray tags = new JArray();

            foreach (XmlElement t in tagList)
            {
                tags.Add(ToAoiTag(t));
            }

            aoi.Add("LocalTags", tags);

            JArray routines = new JArray();
            var xmlNodeList = node.GetElementsByTagName("Routine");
            foreach (XmlElement element in xmlNodeList)
            {
                routines.Add(JObject.FromObject(ToRoutine(element)));
            }
            aoi.Add("Routines", routines);
            return aoi;
        }


        private static JObject ToAoiTag(XmlElement node)
        {
            JObject tag = new JObject();
     
            var attrs = node.Attributes;
            tag.Add("Name", attrs["Name"].Value);
            tag.Add("DataType", attrs["DataType"].Value);
            if (node.HasAttribute("Dimensions"))
            {
                tag.Add("Dimensions", int.Parse(attrs["Dimensions"].Value));
            }
            tag.Add("Radix", (int)EnumUtils.Parse<DisplayStyle>(attrs["Radix"].Value));
            if (node.HasAttribute("ExternalAccess"))
            {
                tag.Add("ExternalAccess", (int)EnumUtils.Parse<ExternalAccess>(attrs["ExternalAccess"].Value));
            }

            return tag;
        }


        private static JObject ToAoiParameter(XmlElement node)
        {
            JObject parameter = new JObject();

            var attrs = node.Attributes;
            parameter.Add("Name", attrs["Name"].Value);
            parameter.Add("TagType", (int) EnumUtils.Parse<TagType>(attrs["TagType"].Value));
            parameter.Add("DataType", attrs["DataType"].Value);
            if (node.HasAttribute("Usage")) {
                parameter.Add("Usage", (int) EnumUtils.Parse<Usage>(attrs["Usage"].Value));
            }
            if (node.HasAttribute("Radix"))
            {
                parameter.Add("Radix", (int)EnumUtils.Parse<DisplayStyle>(attrs["Radix"].Value));
            }
            parameter.Add("Required", bool.Parse(attrs["Required"].Value));
            parameter.Add("Visible", bool.Parse(attrs["Visible"].Value));
            if (node.HasAttribute("ExternalAccess"))
            {
                parameter.Add("ExternalAccess", (int) EnumUtils.Parse<ExternalAccess>(attrs["ExternalAccess"].Value));
            }

            if (node.HasAttribute("Constant"))
            {
                parameter.Add("Constant", bool.Parse(attrs["Constant"].Value));
            }

            var description = node.SelectSingleNode("Description");
            if (description != null)
            {
                parameter.Add("Description", description.FirstChild.Value);
            }
            return parameter;
        }

        private static JObject ToDataType(XmlElement xmlNode)
        {
            JObject dataType = new JObject
            {
                {"Name", xmlNode.Attributes["Name"].Value},
                {"Family", (int) EnumUtils.Parse<FamilyType>(xmlNode.Attributes["Family"].Value)},
                {"Class", (int) EnumUtils.Parse<DataTypeClass>(xmlNode.Attributes["Class"].Value)}
            };

            var description = xmlNode.SelectSingleNode("Description");
            if (description != null)
            {
                dataType.Add("Description", description.FirstChild.Value);
            }

            // Members
            JArray members = new JArray();
            dataType.Add("Members", members);

            var xmlNodeList = xmlNode.GetElementsByTagName("Member");
            foreach (XmlElement node in xmlNodeList)
            {
                members.Add(ToDataTypeMember(node));
            }

            return dataType;
        }

        private static JObject ToDataTypeMember(XmlElement xmlNode)
        {
            // Name,DataType,Dimension,Radix,Hidden,ExternalAccess
            JObject dataTypeMember = new JObject
            {
                {"Name", xmlNode.Attributes["Name"].Value},
                {"DataType", xmlNode.Attributes["DataType"].Value},
                {"Dimension", int.Parse(xmlNode.Attributes["Dimension"].Value)},
                {"Radix", (int) EnumUtils.Parse<DisplayStyle>(xmlNode.Attributes["Radix"].Value)},
                {"Hidden", bool.Parse(xmlNode.Attributes["Hidden"].Value)}
            };

            // Target, BitNumber
            if (xmlNode.Attributes["DataType"].Value.Equals("BIT"))
            {
                dataTypeMember.Add("Target", xmlNode.Attributes["Target"].Value);
                dataTypeMember.Add("BitNumber", int.Parse(xmlNode.Attributes["BitNumber"].Value));
            }

            dataTypeMember.Add("ExternalAccess",
                (int) EnumUtils.Parse<ExternalAccess>(xmlNode.Attributes["ExternalAccess"].Value));

            return dataTypeMember;
        }

        private static JObject ToTask(XmlElement xmlNode)
        {
            JObject task = new JObject
            {
                {"DisableUpdateOutputs", bool.Parse(xmlNode.Attributes["DisableUpdateOutputs"].Value)},
                {"InhibitTask", bool.Parse(xmlNode.Attributes["InhibitTask"].Value)},
                {"Name", xmlNode.Attributes["Name"].Value},
                {"Priority", int.Parse(xmlNode.Attributes["Priority"].Value)},
                {"Type", (int) EnumUtils.Parse<TaskType>(xmlNode.Attributes["Type"].Value)},
                {"Watchdog", int.Parse(xmlNode.Attributes["Watchdog"].Value)}
            };

            if ((int) task["Type"] == (int) TaskType.Periodic)
            {
                task["Rate"] = float.Parse(xmlNode.Attributes["Rate"].Value);
            }

            // SchededPrograms
            JArray schededPrograms = new JArray();
            task.Add("SchededPrograms", schededPrograms);

            var xmlNodeList = xmlNode.GetElementsByTagName("ScheduledProgram");
            foreach (XmlElement node in xmlNodeList)
            {
                schededPrograms.Add(node.Attributes["Name"].Value);
            }

            return task;
        }

        private static JObject ToTag(XmlElement xmlNode)
        {
            JObject tag = new JObject
            {
                {"DataType", xmlNode.Attributes["DataType"].Value},
                {"Name", xmlNode.Attributes["Name"].Value},
                {"TagType", (int) EnumUtils.Parse<TagType>(xmlNode.Attributes["TagType"].Value)},
            };

            int[] dims = new[] {0, 0, 0};
            if (xmlNode.HasAttribute("Dimensions"))
            {
                string[] array = xmlNode.Attributes["Dimensions"].Value.Split(' ');
                Debug.Assert(array.Length <= 3, array.Length.ToString());
                JArray res = new JArray();

                for (int i = 0; i < array.Length; ++i)
                {
                    var dim = int.Parse(array[i]);
                    res.Add(int.Parse(array[i]));
                    dims[i] = dim;
                }

                tag["Dimensions"] = res;
            }


            if (xmlNode.HasAttribute("Radix"))
                tag.Add("Radix", (int) EnumUtils.Parse<DisplayStyle>(xmlNode.Attributes["Radix"].Value));
            if (xmlNode.HasAttribute("Constant"))
                tag.Add("Constant", bool.Parse(xmlNode.Attributes["Constant"].Value) ? 1 : 0);

            tag.Add("ExternalAccess",
                (int) EnumUtils.Parse<ExternalAccess>(xmlNode.Attributes["ExternalAccess"].Value));

            // Description
            var description = (XmlElement) xmlNode.SelectSingleNode("Description");
            if (description != null)
            {
                tag.Add("Description", description.FirstChild.Value);
            }

            // Other Data
            var data_type = xmlNode.Attributes["DataType"].Value;
            if (data_type.Equals("MOTION_GROUP"))
            {
                var parameters = (XmlElement) xmlNode.SelectSingleNode("Data/MotionGroupParameters");
                if (parameters != null)
                {
                    tag.Add("Alternate1UpdateMultiplier",
                        int.Parse(parameters.Attributes["Alternate1UpdateMultiplier"].Value));
                    tag.Add("Alternate2UpdateMultiplier",
                        int.Parse(parameters.Attributes["Alternate2UpdateMultiplier"].Value));
                    tag.Add("AutoTagUpdate", parameters.Attributes["AutoTagUpdate"].Value);
                    tag.Add("CoarseUpdatePeriod", int.Parse(parameters.Attributes["CoarseUpdatePeriod"].Value));
                    tag.Add("GeneralFaultType", parameters.Attributes["GeneralFaultType"].Value);
                    tag.Add("GroupType", parameters.Attributes["GroupType"].Value);
                    tag.Add("PhaseShift", parameters.Attributes["PhaseShift"].Value);
                }
            }
            else if (data_type.Equals("AXIS_CIP_DRIVE"))
            {
                //TODO(gjc): add code here
                var parameters = (XmlElement) xmlNode.SelectSingleNode("Data/AxisParameters");
                if (parameters != null)
                {
                    tag.Add("Parameters", AxisParameters.ToJObject(parameters));
                }
            }
            else if (data_type.Equals("AXIS_VIRTUAL"))
            {
                var parameters = (XmlElement) xmlNode.SelectSingleNode("Data/AxisParameters");
                if (parameters != null)
                {
                    tag.Add("Parameters", AxisVirtualParameters.ToJObject(parameters));
                }
            }
            else if (data_type.Equals("CAM_PROFILE"))
            {
                Debug.Assert(dims[1] == 0 && dims[2] == 0, $"{dims[1]}:{dims[2]}");

                var data = xmlNode.SelectSingleNode("Data")?.FirstChild.Value;
                data = data.Trim();

                if (dims[0] == 0)
                {
                    tag.Add("Data", CAMProfile.ToJObject(data));
                }
                else
                {
                    data = data.Substring(1, data.Length - 2);
                    var array = new JArray();
                    int offset = 0;
                    for (int i = 0; i < dims[0]; ++i)
                    {
                        int begin = data.IndexOf('[', offset);
                        int end = data.IndexOf(']', offset);
                        array.Add(CAMProfile.ToJObject(data.Substring(begin, end - begin + 1)));
                        offset = end + 1;
                    }

                    tag.Add("Data", array);
                }
            }

            return tag;
        }

        private static JObject ToFBDBlock(XmlElement config)
        {
            var block = new JObject();
            var klass  = config.Name;
            block["Class"] = klass;

            var attrs = config.Attributes;
            if (klass.Equals("IRef") || klass.Equals("ORef"))
            {
                block["ID"] = int.Parse(attrs["ID"].Value);
                block["Operand"] = attrs["Operand"].Value;
            } else if (klass.Equals("Block"))
            {
                block["ID"] = int.Parse(attrs["ID"].Value);
                block["Type"] = attrs["Type"].Value;
                block["Operand"] = attrs["Operand"].Value;
            } else if (klass.Equals("Wire") || klass.Equals("FeedbackWire"))
            {
                block["FromID"] = int.Parse(attrs["FromID"].Value);
                block["ToID"] = int.Parse(attrs["ToID"].Value);
                if (config.HasAttribute("ToParam"))
                {
                    block["ToParam"] = attrs["ToParam"].Value;
                }

                if (config.HasAttribute("FromParam"))
                {
                    block["FromParam"] = attrs["FromParam"].Value;
                }
            } else if (klass.Equals("AddOnInstruction"))
            {
                block["ID"] = int.Parse(attrs["ID"].Value);
                block["Operand"] = attrs["Operand"].Value;
                var parameters = config.SelectNodes("InOutParameter");
                var ps = new JArray();
                foreach (XmlElement param in parameters)
                {
                    var p = new JObject(); 
                    p["Name"] = param.GetAttribute("Name");
                    p["Argument"] = param.GetAttribute("Argument");
                    ps.Add(p);
                }
                block["Parameters"] = ps;
                //Console.WriteLine(block);
            } else if (klass.Equals("ICon"))
            {
                block["ID"] = int.Parse(attrs["ID"].Value);
                block["Name"] = attrs["Name"].Value;
            }
            else if (klass.Equals("OCon"))
            {
                block["ID"] = int.Parse(attrs["ID"].Value);
                block["Name"] = attrs["Name"].Value;
            }
            else
            {
                Debug.Assert(false, $"Unknown block name {klass}");
            }
            return block;
        }

        private static JObject ToRoutine(XmlElement element)
        {
            var type = element.Attributes["Type"].Value;
            if (type.Equals("ST"))
            {
                RoutineInfo routine = new RoutineInfo { Type = (int)RoutineType.ST, Name = element.Attributes["Name"].Value };

                var lineList = element.GetElementsByTagName("Line");
                foreach (XmlElement line in lineList)
                {
                    routine.CodeText.Add(line.FirstChild.Value);
                }

                return JObject.FromObject(routine);
            }
            else if (type.Equals("RLL"))
            {
                RoutineInfo routine = new RoutineInfo() { Type = (int)RoutineType.RLL, Name = element.Attributes["Name"].Value };
                var rungList = element.GetElementsByTagName("Rung");
                foreach (XmlElement rung in rungList)
                {
                    Debug.Assert(rung.Attributes["Type"].Value.Equals("N"));
                    var text = rung.SelectSingleNode("Text");
                    if (text != null)
                    {
                        routine.CodeText.Add(text.FirstChild.Value);
                    }
                }
                return JObject.FromObject(routine);
            }
            else if (type.Equals("FBD"))
            {
                var res = new JObject();
                res["Type"] = (int) RoutineType.FBD;
                res["Name"] = element.Attributes["Name"].Value;
                var sheets = new JArray();
                var sheetList = element.GetElementsByTagName("Sheet");
                foreach (XmlElement sheet in sheetList)
                {
                    var st = new JObject();
                    st["Number"] = int.Parse(sheet.Attributes["Number"].Value);
                    var blocks = new JArray();
                    foreach (XmlElement block in sheet)
                    {
                        blocks.Add(ToFBDBlock(block));
                    }

                    st["Blocks"] = blocks;
                    sheets.Add(st);
                }

                res["Sheets"] = sheets;

                return res;
            }
            else
            {
                throw new ApplicationException($"add other {type} routine!");
            }
        }

        private static JObject ToProgram(XmlElement xmlNode)
        {
            Debug.Assert(xmlNode != null);
            JObject program = new JObject
            {
                {"Disabled", bool.Parse(xmlNode.Attributes["Disabled"].Value)},
                {"MainRoutineName", xmlNode.Attributes["MainRoutineName"]?.Value},
                {"Name", xmlNode.Attributes["Name"].Value}
            };

            // Routines
            JArray routines = new JArray();
            var xmlNodeList = xmlNode.GetElementsByTagName("Routine");
            foreach (XmlElement element in xmlNodeList)
            {
                routines.Add(JObject.FromObject(ToRoutine(element)));
            }

            program.Add("Routines", routines);

            // Tags
            JArray tags = new JArray();
            xmlNodeList = xmlNode.GetElementsByTagName("Tag");
            foreach (XmlElement element in xmlNodeList)
            {
                // TODO(gjc): use ToTag?
                JObject tag = new JObject
                {
                    {"DataType", element.Attributes["DataType"].Value},
                    {"Name", element.Attributes["Name"].Value},
                    {"TagType", (int) EnumUtils.Parse<TagType>(element.Attributes["TagType"].Value)},
                };

                if (element.HasAttribute("Radix"))
                    tag.Add("Radix", (int) EnumUtils.Parse<DisplayStyle>(element.Attributes["Radix"].Value));
                if (element.HasAttribute("Usage"))
                    tag.Add("Usage", (int) EnumUtils.Parse<Usage>(element.Attributes["Usage"].Value));
                if (element.HasAttribute("Constant"))
                    tag.Add("Constant", bool.Parse(element.Attributes["Constant"].Value) ? 1 : 0);

                tag.Add("ExternalAccess",
                    (int) EnumUtils.Parse<ExternalAccess>(element.Attributes["ExternalAccess"].Value));

                tags.Add(tag);
            }

            program.Add("Tags", tags);
            return program;
        }

        public static JObject ToDeviceModule(XmlElement xmlNode)
        {
            var productType = int.Parse(xmlNode.Attributes["ProductType"].Value.Trim());

            JObject deviceModule = JObject.FromObject(new DeviceModule(xmlNode));

            // EKey
            XmlElement eKeyNode = (XmlElement) xmlNode.SelectSingleNode("EKey");
            if (eKeyNode != null)
            {
                if (eKeyNode.HasAttribute("State"))
                    deviceModule.Add("EKey", eKeyNode.Attributes["State"].Value);
            }

            if (productType == 0x25)
            {
                // Ports
                var xmlNodeList = xmlNode.GetElementsByTagName("Port");
                JArray ports = new JArray();

                foreach (XmlElement element in xmlNodeList)
                {
                    Port port = new Port(element);
                    ports.Add(JObject.FromObject(port));
                }

                deviceModule.Add("Ports", ports);

                // ConfigData
                var configDataNode = xmlNode.SelectSingleNode("Communications/ConfigData/Data")?.FirstChild;

                var abCIPDrive = AB_CIP_Drive_C_2.FromCDATA(configDataNode?.Value);
                CIPMotionDriveConfigData configData = Mapper.Map<CIPMotionDriveConfigData>(abCIPDrive);

                deviceModule.Add("ConfigData", JObject.FromObject(configData));

            }

            return deviceModule;
        }
    }
}