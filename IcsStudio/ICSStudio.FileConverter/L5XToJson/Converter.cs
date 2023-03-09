using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using AutoMapper;
using ICSStudio.FileConverter.L5XToJson.Objects;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Annotations;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.VisualStudio.Shell;
using ICSStudio.UIInterfaces.Error;

namespace ICSStudio.FileConverter.L5XToJson
{

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public partial class Converter
    {
        static Converter()
        {
            Mapper.Initialize(cfg => { cfg.CreateMap<AB_CIP_Drive_C_2, CIPMotionDriveConfigData>(); });
        }

        private static JObject SortJObject(JObject obj)
        {
            return Utils.Utils.SortJObject(obj);
        }

        public static void L5XToJson(string l5xFile, string jsonFile)
        {
            var docWithLine = new XPathDocument(l5xFile); // with line info
            XmlDocument doc = new XmlDocument();
            doc.Load(l5xFile);

            var jObject = ToJObject(doc, false, docWithLine);
            var isExist = File.Exists(jsonFile);
            using (var sw = File.CreateText(jsonFile))
            using (var jw = new JsonTextWriter(sw))
            {
                jw.Formatting = Newtonsoft.Json.Formatting.Indented;
                jObject.WriteTo(jw);
                if (isExist)
                {
                    var recoveryPath = $"{jsonFile}.Recovery";
                    if (File.Exists(recoveryPath))
                    {
                        File.Delete(recoveryPath);
                    }
                }
            }
        }
        
        public static JObject ToJObject(XmlDocument doc, bool isTmp = false, XPathDocument docWithLine = null)
        {
            JObject controller = new JObject();
            var instance = Controller.GetInstance();
            var xmlNode = (XmlElement)doc.SelectSingleNode("RSLogix5000Content/Controller");

            controller.Add("Name", xmlNode.GetAttribute("Name"));

            controller.AddOrIgnore(xmlNode, "Use");
            controller.AddOrIgnore(xmlNode, "PowerLossProgram");
            controller.AddOrIgnore(xmlNode, "MajorFaultProgram");
            controller.AddOrIgnore(xmlNode, "ProjectSN");
            controller.AddOrIgnore(xmlNode, "TimeSlice");
            controller.AddOrIgnore(xmlNode, "MatchProjectToController");
            controller.AddOrIgnore(xmlNode, "ProjectCreationDate");
            controller.AddOrIgnore(xmlNode, "LastModifiedDate");
            controller.AddOrIgnore(xmlNode, "EtherNetIPMode");

            if (xmlNode.HasAttribute("CommPath"))
            {
                string commPath = xmlNode.GetAttribute("CommPath");

                int index = commPath.LastIndexOf('\\');
                string ipAddress = index >= 0 ? commPath.Substring(index + 1) : commPath;

                controller.Add("CommPath", ipAddress);
            }

            // Modules
            JArray modules = new JArray();
            controller.Add("Modules", modules);

            xmlNode = (XmlElement)doc.SelectSingleNode("RSLogix5000Content/Controller/Modules");
            var xmlNodeList = xmlNode?.GetElementsByTagName("Module");
            if (xmlNodeList != null)
            {
                List<string> nameList = new List<string>();

                foreach (XmlElement node in xmlNodeList)
                {
                    if (node.HasAttribute("Use"))
                    {
                        if ("Reference".Equals(node.Attributes["Use"].Value)) continue;
                    }

                    JObject module = ToDeviceModule(node, nameList);
                    nameList.Add(module["Name"].ToString());

                    modules.Add(module);
                }
            }

            // DataTypes
            JArray dataTypes = new JArray();
            controller.Add("DataTypes", dataTypes);

            xmlNode = (XmlElement)doc.SelectSingleNode("RSLogix5000Content/Controller/DataTypes");
            xmlNodeList = xmlNode?.GetElementsByTagName("DataType");
            if (xmlNodeList != null)
            {
                foreach (XmlElement node in xmlNodeList)
                {
                    var dataType = ToDataType(node);
                    dataTypes.Add(dataType);
                    instance.AddDataType(dataType, isTmp);
                }
            }

            xmlNode = (XmlElement)doc.SelectSingleNode("RSLogix5000Content/Controller/AddOnInstructionDefinitions");
            xmlNodeList = xmlNode?.GetElementsByTagName("AddOnInstructionDefinition");

            JArray aoiDefinitions = new JArray();
            if (xmlNodeList != null)
            {
                foreach (XmlElement node in xmlNodeList)
                {
                    var aoi = ToAoiDefinition(node);
                    aoiDefinitions.Add(aoi);
                    instance.AddAOIDefinition(aoi, isTmp);
                }
            }

            instance.FinalizeTypeCreation();

            xmlNodeList = xmlNode?.GetElementsByTagName("EncodedData");
            if (xmlNodeList != null && xmlNodeList.Count > 0)
            {
                throw new NotSupportedException("L5X含有加密数据无法解析");
            }

            controller.Add("AddOnInstructionDefinitions", aoiDefinitions);

            // Programs
            JArray programs = new JArray();

            xmlNode = (XmlElement)doc.SelectSingleNode("RSLogix5000Content/Controller/Programs");
            xmlNodeList = xmlNode?.GetElementsByTagName("Program");
            if (xmlNodeList != null)
            {
                foreach (XmlElement node in xmlNodeList)
                {
                    var program = ToProgram(node);
                    if (program != null)
                        programs.Add(program);
                }
            }

            controller.Add("Programs", programs);

            // Tags
            JArray tags = new JArray();
            controller.Add("Tags", tags);

            xmlNode = (XmlElement)doc.SelectSingleNode("RSLogix5000Content/Controller/Tags");
            xmlNodeList = xmlNode?.GetElementsByTagName("Tag");
            if (xmlNodeList != null)
            {
                var i = 0;
                foreach (XmlElement node in xmlNodeList)
                {
                    i++;
                    var regex = new Regex("^[a-zA-Z_][a-zA-Z0-9_]*$");
                    var nodeName = node.Attributes["Name"].Value;
                    if (!regex.IsMatch(nodeName))
                    {
                        var _outputService =
                            Package.GetGlobalService(typeof(SErrorOutputService)) as IErrorOutputService;
                        var j = 0;
                        var position = string.Empty;
                        if (docWithLine != null)
                        {
                            foreach (XPathNavigator item in docWithLine.CreateNavigator().Select("//*"))
                                if (item.Name.Equals("Tag"))
                                {
                                    j++;
                                    if (i == j)
                                    {
                                        position = $"Line {((IXmlLineInfo)item).LineNumber}: ";
                                        break;
                                    }
                                }
                        }

                        var errorMessage =
                            $"{position}Error creating  'Tag[@Name=\"{nodeName}\"]'  (Requested item could not be found.).\n";
                        errorMessage += $"RSLogix5000Content/Controller/Tags/Tag[@Name=\"{nodeName}\"]";
                        _outputService?.AddErrors(errorMessage, OrderType.None, OnlineEditType.Original, null, null,
                            null);
                        continue;
                    }

                    var tag = ToTag(node);
                    if (tag != null)
                        tags.Add(tag);
                }
            }

            // Tasks
            JArray tasks = new JArray();
            controller.Add("Tasks", tasks);

            xmlNode = (XmlElement)doc.SelectSingleNode("RSLogix5000Content/Controller/Tasks");
            xmlNodeList = xmlNode?.GetElementsByTagName("Task");
            if (xmlNodeList != null)
            {
                foreach (XmlElement node in xmlNodeList)
                {
                    tasks.Add(ToTask(node));
                }
            }

            // ParameterConnections
            xmlNode = (XmlElement)doc.SelectSingleNode("RSLogix5000Content/Controller/ParameterConnections");
            xmlNodeList = xmlNode?.GetElementsByTagName("ParameterConnection");
            if (xmlNodeList != null && xmlNodeList.Count > 0)
            {
                JArray parameterConnections = new JArray();
                controller.Add("ParameterConnections", parameterConnections);

                foreach (XmlElement node in xmlNodeList)
                {
                    parameterConnections.Add(ToParameterConnection(node));
                }
            }


            // TimeSynchronize
            JObject time = new JObject();
            xmlNode = (XmlElement)doc.SelectSingleNode("RSLogix5000Content/Controller/TimeSynchronize");
            if (xmlNode != null)
            {
                time.Add("Priority1", int.Parse(xmlNode.Attributes["Priority1"].Value));
                time.Add("Priority2", int.Parse(xmlNode.Attributes["Priority2"].Value));
                time.Add("PTPEnable", bool.Parse(xmlNode.Attributes["PTPEnable"].Value));

                controller.Add("TimeSynchronize", time);
            }

            // Trends
            var trends = ToTrends((XmlElement)doc.SelectSingleNode("RSLogix5000Content/Controller/Trends"));
            if (trends != null)
                controller.Add("Trends", trends);


            return SortJObject(controller);
        }

        private static JObject ToParameterConnection(XmlElement node)
        {
            JObject parameterConnection = new JObject();

            parameterConnection.Add(node, "EndPoint1");
            parameterConnection.Add(node, "EndPoint2");

            return parameterConnection;
        }

        private static JObject ToAoiDefinition(XmlElement node)
        {
            JObject aoi = new JObject();

            aoi.AddOrIgnore(node, "DataType");

            aoi.AddOrIgnore(node, "Use");

            aoi.Add(node, "Name");

            var description = node.SelectSingleNode("Description");
            aoi.Add("Description", description?.FirstChild?.Value ?? "");

            aoi.Add("RevisionExtension", node.Attributes["RevisionExtension"]?.Value ?? "");

            var revisionNote = (XmlElement)node.SelectSingleNode("RevisionNote");
            aoi.Add("RevisionNote", revisionNote?.FirstChild?.Value ?? "");

            aoi.Add("Vendor", node.Attributes["Vendor"]?.Value ?? "");

            var additionalHelpText = node.SelectSingleNode("AdditionalHelpText");
            aoi.Add("AdditionalHelpText", additionalHelpText?.FirstChild?.Value ?? "");

            var signatureHistory = (XmlElement)node.SelectSingleNode("SignatureHistory");
            if (signatureHistory != null)
            {
                var histories = signatureHistory.GetElementsByTagName("HistoryEntry");
                var oHistories = new JArray();
                foreach (XmlElement history in histories)
                {
                    var item = new JObject();
                    item["User"] = history.Attributes["User"]?.Value;
                    item["Timestamp"] = history.Attributes["Timestamp"]?.Value;
                    item["SignatureID"] = history.Attributes["SignatureID"]?.Value.Replace("16#", "").Replace("_", "")
                        .ToUpper();
                    var itemDescription = history.SelectSingleNode("Description");
                    item["Description"] = itemDescription?.FirstChild?.Value;
                    oHistories.AddFirst(item);
                }

                aoi.Add("SignatureHistory", oHistories);
            }

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

            if (tagList != null)
            {
                foreach (XmlElement t in tagList)
                {
                    tags.Add(ToAoiTag(t));
                }
            }

            aoi.Add("LocalTags", tags);

            JArray routines = new JArray();
            var xmlNodeList = node.GetElementsByTagName("Routine");
            foreach (XmlElement element in xmlNodeList)
            {
                var routineJObj = ToRoutine(element);
                if (routineJObj != null) routines.Add(JObject.FromObject(routineJObj));
            }

            aoi.Add("Routines", routines);

            return SortJObject(aoi);
        }

        private static JObject ToAoiTag(XmlElement node)
        {
            JObject tag = new JObject();

            var attrs = node.Attributes;

            tag.Add("Name", attrs["Name"].Value);
            tag.Add("DataType", attrs["DataType"].Value);

            int dim = 0;
            if (node.HasAttribute("Dimensions"))
            {
                tag.Add("Dimensions", int.Parse(attrs["Dimensions"].Value));
                dim = int.Parse(attrs["Dimensions"].Value);
            }

            tag.AddOrIgnore<DisplayStyle>(node, "Radix");

            tag.AddOrIgnore<ExternalAccess>(node, "ExternalAccess");

            var description = node.SelectSingleNode("Description");
            if (description != null)
            {
                tag.Add("Description", description.FirstChild.Value);
            }

            var data = DataParse.Parse(tag, node, attrs["DataType"].Value, new[] { dim, 0, 0 },
                DataParse.DataParseType.Aoi);
            if (data != null)
            {
                tag.Add("DefaultData", data);
            }

            JArray commentsArray = new JArray();
            var comments = node.SelectSingleNode("Comments");
            if (comments != null)
            {
                var commentsList = ((XmlElement)comments).GetElementsByTagName("Comment");
                foreach (XmlElement c in commentsList)
                {
                    JObject comment = new JObject { ["Operand"] = c.GetAttribute("Operand"), ["Value"] = c.InnerText };
                    commentsArray.Add(comment);
                }
            }

            if (commentsArray.Count > 0)
                tag.Add("Comments", commentsArray);

            return SortJObject(tag);
        }

        private static JObject ToAoiParameter(XmlElement node)
        {
            JObject parameter = new JObject();

            var attrs = node.Attributes;

            parameter.Add("Name", attrs["Name"].Value);
            parameter.Add("TagType", (int)EnumUtils.Parse<TagType>(attrs["TagType"].Value));
            parameter.Add("DataType", attrs["DataType"].Value);

            parameter.AddOrIgnore<Usage>(node, "Usage");

            parameter.AddOrIgnore<DisplayStyle>(node, "Radix");

            parameter.AddOrIgnore(node, "Dimensions");

            parameter.Add<bool>(node, "Required");
            parameter.Add<bool>(node, "Visible");

            parameter.AddOrIgnore<ExternalAccess>(node, "ExternalAccess");

            parameter.AddOrIgnore<bool>(node, "Constant");

            var description = node.SelectSingleNode("Description");
            if (description != null)
            {
                parameter.Add("Description", description.FirstChild.Value);
            }

            if (!"InOut".Equals(node.Attributes["Usage"].Value))
            {
                var defaultData = node.GetElementsByTagName("DefaultData");
                JObject value = new JObject();
                var data = ((XmlElement)defaultData[1])?.SelectSingleNode("DataValue") as XmlElement;
                if (data != null)
                {
                    value.AddOrIgnore(data, "DataType");
                    value.AddOrIgnore(data, "Radix");
                    value.AddOrIgnore(data, "Value");
                    parameter["DefaultData"] = value;
                }
                else
                {
                    if (attrs["Name"].Value == "EnableIn" || attrs["Name"].Value == "EnableOut")
                    {
                        //value["DataType"] = "BOOL";
                        //value["Radix"] = DisplayStyle.Decimal.ToString();
                        //value["Value"] = attrs["Name"].Value == "EnableIn" ? 1 : 0;
                        parameter["DefaultData"] = attrs["Name"].Value == "EnableIn" ? 1 : 0;
                    }
                    else
                    {
                        var d = defaultData[0].InnerText;
                        if (d.Contains("."))
                        {
                            parameter["DefaultData"] = float.Parse(d);
                        }
                        else
                        {
                            parameter["DefaultData"] = int.Parse(d);
                        }
                    }
                }
            }

            JArray commentsArray = new JArray();
            var comments = node.SelectSingleNode("Comments");
            if (comments != null)
            {
                var commentsList = ((XmlElement)comments).GetElementsByTagName("Comment");
                foreach (XmlElement c in commentsList)
                {
                    JObject comment = new JObject
                    {
                        ["Operand"] = c.GetAttribute("Operand"), ["Value"] = c.InnerText
                    };
                    commentsArray.Add(comment);
                }
            }

            if (commentsArray.Count > 0)
                parameter.Add("Comments", commentsArray);

            return SortJObject(parameter);
        }

        private static JObject ToDataType(XmlElement xmlNode)
        {
            JObject dataType = new JObject
            {
                { "Name", xmlNode.Attributes["Name"].Value },
                { "Family", (int)EnumUtils.Parse<FamilyType>(xmlNode.Attributes["Family"].Value) },
                { "Class", (int)EnumUtils.Parse<DataTypeClass>(xmlNode.Attributes["Class"].Value) }
            };

            dataType.AddOrIgnore(xmlNode, "Use");

            dataType.AddOrIgnore(xmlNode, "DataType");

            var description = xmlNode.SelectSingleNode("Description");
            dataType.Add("Description", description != null ? description.FirstChild.Value : "");

            // Members
            JArray members = new JArray();
            dataType.Add("Members", members);

            var xmlNodeList = xmlNode.GetElementsByTagName("Member");
            foreach (XmlElement node in xmlNodeList)
            {
                members.Add(ToDataTypeMember(node));
            }

            return SortJObject(dataType);
        }

        private static JObject ToDataTypeMember(XmlElement xmlNode)
        {
            // Name,DataType,Dimension,Radix,Hidden,ExternalAccess
            JObject dataTypeMember = new JObject
            {
                { "Name", xmlNode.Attributes["Name"].Value },
                { "DataType", xmlNode.Attributes["DataType"].Value },
                { "Dimension", int.Parse(xmlNode.Attributes["Dimension"].Value) },
                { "Radix", (int)EnumUtils.Parse<DisplayStyle>(xmlNode.Attributes["Radix"].Value) },
                { "Hidden", bool.Parse(xmlNode.Attributes["Hidden"].Value) }
            };

            // Target, BitNumber
            if (xmlNode.Attributes["DataType"].Value.Equals("BIT"))
            {
                dataTypeMember.Add("Target", xmlNode.Attributes["Target"].Value);
                dataTypeMember.Add("BitNumber", int.Parse(xmlNode.Attributes["BitNumber"].Value));
            }

            var description = xmlNode.SelectSingleNode("Description");
            dataTypeMember.Add("Description", description != null ? description.FirstChild.Value : "");
            dataTypeMember.Add("ExternalAccess",
                (int)EnumUtils.Parse<ExternalAccess>(xmlNode.Attributes["ExternalAccess"].Value));

            return SortJObject(dataTypeMember);
        }

        private static JObject ToTask(XmlElement xmlNode)
        {
            JObject task = new JObject
            {
                { "DisableUpdateOutputs", bool.Parse(xmlNode.Attributes["DisableUpdateOutputs"].Value) },
                { "InhibitTask", bool.Parse(xmlNode.Attributes["InhibitTask"].Value) },
                { "Name", xmlNode.Attributes["Name"].Value },
                { "Priority", int.Parse(xmlNode.Attributes["Priority"].Value) },
                { "Type", (int)EnumUtils.Parse<TaskType>(xmlNode.Attributes["Type"].Value) },
                { "Watchdog", int.Parse(xmlNode.Attributes["Watchdog"].Value) }
            };

            if ((int)task["Type"] == (int)TaskType.Periodic)
            {
                task["Rate"] = float.Parse(xmlNode.Attributes["Rate"].Value);
            }

            // Description
            var xmlNodeList = xmlNode.GetElementsByTagName("Description");
            if (xmlNodeList.Count > 0)
            {
                task.Add("Description", xmlNodeList[0].FirstChild.Value);
            }

            // SchededPrograms
            JArray schededPrograms = new JArray();
            task.Add("SchededPrograms", schededPrograms);

            xmlNodeList = xmlNode.GetElementsByTagName("ScheduledProgram");
            foreach (XmlElement node in xmlNodeList)
            {
                schededPrograms.Add(node.Attributes["Name"].Value);
            }

            return SortJObject(task);
        }

        private static JObject ToTag(XmlElement xmlNode)
        {

            var type = EnumUtils.Parse<TagType>(xmlNode.Attributes["TagType"]?.Value ?? "Base");
            if (xmlNode.HasAttribute("Use") && xmlNode.GetAttribute("Use").Equals("Reference"))
            {
                JObject tagReference = new JObject
                {
                    { "Name", xmlNode.Attributes["Name"].Value },
                    { "TagType", (int)type },
                    { "Use", "Reference" }
                };
                return tagReference;
            }

            JObject tag = new JObject
            {
                { "Name", xmlNode.Attributes["Name"].Value },
                { "TagType", (int)type },
            };

            tag.AddOrIgnore(xmlNode, "DataType");

            int[] dims = { 0, 0, 0 };
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

            if (type == TagType.Alias)
            {
                throw new NotSupportedException("不支持别名标签");
                //tag.Add(xmlNode, "AliasFor");
            }

            if (type == TagType.Produced)
            {
                var mapping = xmlNode.GetElementsByTagName("ProduceInfo");
                Debug.Assert(mapping.Count == 1);

                //tag["PLCMappingFile"] = int.Parse(mapping[0].Attributes["PLCMappingFile"].Value);
                tag.AddOrIgnore<int>(mapping[0] as XmlElement, "PLCMappingFile");
            }

            tag.AddOrIgnore<DisplayStyle>(xmlNode, "Radix");
            tag.AddOrIgnore<bool>(xmlNode, "Constant");
            tag.AddOrIgnore<Usage>(xmlNode, "Usage");
            tag.AddOrIgnore<ExternalAccess>(xmlNode, "ExternalAccess");

            // Description
            var description = (XmlElement)xmlNode.SelectSingleNode("Description");
            if (description != null)
            {
                tag.Add("Description", description.FirstChild.Value);
            }

            // Comments
            JArray commentsArray = new JArray();
            var comments = xmlNode.SelectSingleNode("Comments");
            if (comments != null)
            {
                var commentsList = ((XmlElement)comments).GetElementsByTagName("Comment");
                foreach (XmlElement c in commentsList)
                {
                    JObject comment = new JObject
                    {
                        ["Operand"] = c.GetAttribute("Operand"), ["Value"] = c.InnerText
                    };
                    commentsArray.Add(comment);
                }
            }

            if (commentsArray.Count > 0)
                tag.Add("Comments", commentsArray);


            // Other Data
            if (type == TagType.Base)
            {
                var data_type = xmlNode.Attributes["DataType"].Value;
                var data = DataParse.Parse(tag, xmlNode, data_type, dims, DataParse.DataParseType.Normal);
                if (data != null)
                {

                    var res = new List<byte>();
                    EncodeData(res, data);
                    tag.Add("Data", Function.EncodeByteArray(res));
                }
            }

            return tag;
        }

        private static void EncodeData(List<byte> res, JToken data)
        {
            var t = data.Type;
            switch (t)
            {
                case JTokenType.Array:
                    var array = data as JArray;
                    MsgPackUtils.EncodeArraySize(res, array.Count);
                    foreach (var d in array)
                    {
                        EncodeData(res, d);
                    }

                    break;
                case JTokenType.Integer:
                    var value = (data as JValue).Value;
                    if (value is int)
                    {
                        MsgPackUtils.EncodeInt32(res, (int)value);
                    }
                    else if (value is long)
                    {
                        MsgPackUtils.EncodeInt64(res, (Int64)value);
                    }
                    else if (value is UInt32)
                    {
                        MsgPackUtils.EncodeUInt32(res, (UInt32)value);
                    }
                    else if (value is UInt64)
                    {
                        MsgPackUtils.EncodeUInt64(res, (UInt64)value);
                    }
                    else
                    {
                        Debug.Assert(false, "");
                        MsgPackUtils.EncodeInt32(res, 0);
                    }

                    break;
                case JTokenType.Float:
                    MsgPackUtils.EncodeFloat64(res, (double)data);
                    break;
                case JTokenType.Boolean:
                    MsgPackUtils.EncodeBool(res, (bool)data);
                    break;
                default:
                    Debug.Assert(false, "");
                    break;
            }

        }

        private static JObject ToFBDBlock(XmlElement config)
        {
            var block = new JObject();
            var className = config.Name;
            block["Class"] = className;

            var attrs = config.Attributes;
            if (className.Equals("IRef") || className.Equals("ORef"))
            {
                block["ID"] = int.Parse(attrs["ID"].Value);
                block["Operand"] = attrs["Operand"].Value;
                block["X"] = int.Parse(attrs["X"].Value);
                block["Y"] = int.Parse(attrs["Y"].Value);
            }
            else if (className.Equals("Block"))
            {
                block["ID"] = int.Parse(attrs["ID"].Value);
                block["Type"] = attrs["Type"].Value;
                block["Operand"] = attrs["Operand"].Value;
                block["X"] = int.Parse(attrs["X"].Value);
                block["Y"] = int.Parse(attrs["Y"].Value);
                block["VisiblePins"] = attrs["VisiblePins"].Value;
            }
            else if (className.Equals("Wire") || className.Equals("FeedbackWire"))
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
            }
            else if (className.Equals("AddOnInstruction"))
            {
                block["ID"] = int.Parse(attrs["ID"].Value);
                block["Name"] = attrs["Name"].Value;
                block["Operand"] = attrs["Operand"].Value;

                var parameters = config.SelectNodes("InOutParameter");
                var ps = new JArray();
                foreach (XmlElement param in parameters)
                {
                    var p = new JObject();
                    p["Name"] = param.GetAttribute("Name");
                    p["Argument"] = param.GetAttribute("Argument");
                    ps.Add(SortJObject(p));
                }

                block["Parameters"] = ps;
                //Console.WriteLine(block);
            }
            else if (className.Equals("ICon"))
            {
                block["ID"] = int.Parse(attrs["ID"].Value);
                block["Name"] = attrs["Name"].Value;
                block["X"] = int.Parse(attrs["X"].Value);
                block["Y"] = int.Parse(attrs["Y"].Value);
            }
            else if (className.Equals("OCon"))
            {
                block["ID"] = int.Parse(attrs["ID"].Value);
                block["Name"] = attrs["Name"].Value;
                block["X"] = int.Parse(attrs["X"].Value);
                block["Y"] = int.Parse(attrs["Y"].Value);
            }
            else if (className.Equals("Function"))
            {
                block["ID"] = int.Parse(attrs["ID"].Value);
                block["Type"] = attrs["Type"].Value;
                block["X"] = int.Parse(attrs["X"].Value);
                block["Y"] = int.Parse(attrs["Y"].Value);
            }
            else if (className.Equals("TextBox"))
            {
                //TODO(gjc): add code here
            }
            else if (className.Equals("Attachment"))
            {
                //TODO(gjc): add code here
            }
            else
            {
                Debug.Assert(false, $"Unknown block name {className}");
            }

            return SortJObject(block);
        }

        [CanBeNull]
        private static JObject ToRoutine(XmlElement element)
        {
            if (element.Attributes["Type"] == null)
            {
                if (element.HasAttribute("Use") && "Context".Equals(element.Attributes["Use"].Value))
                {
                    RoutineInfo routine = new RoutineInfo
                    {
                        Type = (int)RoutineType.RLL,
                        Name = element.Attributes["Name"].Value
                    };
                    routine.Use = "Context";

                    var rungList = element.GetElementsByTagName("Rung");
                    routine.Rungs = new List<RungInfo>();
                    foreach (XmlElement rung in rungList)
                    {
                        RungInfo rungInfo = new RungInfo();

                        rungInfo.Use = rung.Attributes["Use"].Value;
                        rungInfo.Number = uint.Parse(rung.Attributes["Number"].Value);
                        rungInfo.Type = rung.Attributes["Type"].Value;

                        var text = rung.SelectSingleNode("Text");
                        if (text != null) rungInfo.Text = text.FirstChild.Value;

                        routine.Rungs.Add(rungInfo);
                    }

                    var jObj = SortJObject(JObject.FromObject(routine));
                    return jObj;
                }
            }

            var type = element.Attributes["Type"].Value;

            if (type.Equals("ST"))
            {
                RoutineInfo routine = new RoutineInfo
                    { Type = (int)RoutineType.ST, Name = element.Attributes["Name"].Value };

                if (element.HasAttribute("Use"))
                {
                    routine.Use = element.GetAttribute("Use");
                }

                var stContents = element.SelectNodes("STContent");
                if (stContents.Count == 1)
                {
                    var lineList = stContents[0].SelectNodes("Line");
                    foreach (XmlElement line in lineList)
                    {
                        routine.CodeText.Add(line.FirstChild.Value);
                    }
                }
                else
                {
                    foreach (XmlElement stContent in stContents)
                    {
                        if (stContent.Attributes["OnlineEditType"].Value == "PendingEdits")
                        {
                            var lineList = stContent.SelectNodes("Line");
                            routine.PendingCodeText = new List<string>();
                            foreach (XmlElement line in lineList)
                            {
                                routine.PendingCodeText.Add(line.FirstChild.Value);
                            }
                        }
                        else if (stContent.Attributes["OnlineEditType"].Value == "TestEdits")
                        {
                            var lineList = stContent.SelectNodes("Line");
                            routine.TestCodeText = new List<string>();
                            foreach (XmlElement line in lineList)
                            {
                                routine.TestCodeText.Add(line.FirstChild.Value);
                            }
                        }
                        else
                        {
                            var lineList = stContent.SelectNodes("Line");
                            foreach (XmlElement line in lineList)
                            {
                                routine.CodeText.Add(line.FirstChild.Value);
                            }
                        }
                    }
                }

                var elements = element.GetElementsByTagName("Description");
                if (elements.Count > 0)
                {
                    routine.Description = elements[0].FirstChild.Value;
                }

                return SortJObject(JObject.FromObject(routine));
            }

            if (type.Equals("RLL"))
            {
                RoutineInfo routine = new RoutineInfo
                {
                    Type = (int)RoutineType.RLL,
                    Name = element.Attributes["Name"].Value
                };

                if (element.HasAttribute("Use"))
                {
                    routine.Use = element.GetAttribute("Use");
                }

                var rungList = element.GetElementsByTagName("Rung");
                routine.Rungs = new List<RungInfo>();
                uint number = 0;

                foreach (XmlElement rung in rungList)
                {
                    RungInfo rungInfo = new RungInfo();

                    rungInfo.Number = number;
                    rungInfo.Type = rung.Attributes["Type"].Value;

                    var text = rung.SelectSingleNode("Text");
                    if (text != null)
                    {
                        rungInfo.Text = text.FirstChild.Value;
                    }

                    var comment = rung.SelectSingleNode("Comment");
                    if (comment != null)
                    {
                        rungInfo.Comment = comment.FirstChild.Value;
                    }

                    routine.Rungs.Add(rungInfo);
                    number++;
                }

                var elements = element.GetElementsByTagName("Description");
                if (elements.Count > 0)
                {
                    routine.Description = elements[0].FirstChild.Value;
                }

                return SortJObject(JObject.FromObject(routine));
            }

            if (type.Equals("FBD"))
            {
                var res = new JObject();
                res["Type"] = (int)RoutineType.FBD;
                res["Name"] = element.Attributes["Name"].Value;
                var sheets = new JArray();
                var sheetList = element.GetElementsByTagName("Sheet");
                foreach (XmlElement sheet in sheetList)
                {
                    var st = new JObject();

                    var blocks = new JArray();
                    foreach (XmlElement block in sheet)
                    {
                        if (block.Name == "Description")
                        {
                            st["Description"] = block.FirstChild.InnerText;
                        }
                        else
                        {
                            blocks.Add(ToFBDBlock(block));
                        }

                    }

                    st.AddOrIgnore(element, "Use");

                    st["Blocks"] = blocks;
                    st["Number"] = int.Parse(sheet.Attributes["Number"].Value);

                    sheets.Add(st);
                }

                res["Sheets"] = sheets;
                var elements = element.GetElementsByTagName("Description");
                if (elements.Count > 0)
                {
                    res["Description"] = elements[0].FirstChild.Value;
                }

                return SortJObject(res);
            }

            if (type.Equals("SFC"))
            {
                var res = new JObject();

                res["Name"] = element.Attributes["Name"].Value;
                res["Type"] = (int)RoutineType.SFC;

                #region SFCContent

                var sfcElement = element.GetElementsByTagName("SFCContent")[0];
                var content = new JObject();
                content["SheetSize"] = sfcElement.Attributes["SheetSize"].Value;
                content["SheetOrientation"] = sfcElement.Attributes["SheetOrientation"].Value;
                content["StepName"] = sfcElement.Attributes["StepName"].Value;
                content["TransitionName"] = sfcElement.Attributes["TransitionName"].Value;
                content["ActionName"] = sfcElement.Attributes["ActionName"].Value;
                content["StopName"] = sfcElement.Attributes["StopName"].Value;
                var contents = ToSFCContent(sfcElement);
                if (contents != null)
                    content["Contents"] = contents;

                #endregion

                res["SFCContent"] = content;
                if (element.HasAttribute("Use"))
                {
                    res.Add("Use", element.GetAttribute("Use"));
                }

                var elements = element.GetElementsByTagName("Description");
                if (elements.Count > 0)
                {
                    res["Description"] = elements[0].FirstChild.Value;
                }

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
                { "Inhibited", bool.Parse(xmlNode.Attributes["Disabled"]?.Value ?? "false") },
            };

            program.Add(xmlNode, "Name");

            program.AddOrIgnore(xmlNode, "Use");


            if (xmlNode.HasAttribute("Type"))
            {
                switch (xmlNode.Attributes["Type"].Value)
                {
                    case "EquipmentPhase":
                        program.Add("Type", (int)ProgramType.Phase);
                        break;
                    case "EquipmentSequence":
                        program.Add("Type", (int)ProgramType.Sequence);
                        break;
                }
            }

            program.AddOrIgnore<int>(xmlNode, "InitialStepIndex");

            program.AddOrIgnore<InitialStateType>(xmlNode, "InitialState");

            program.AddOrIgnore<CompleteStateIfNotImplType>(xmlNode, "CompleteStateIfNotImpl");

            program.AddOrIgnore<LossOfCommCmdType>(xmlNode, "LossOfCommCmd");

            program.AddOrIgnore<ExternalRequestActionType>(xmlNode, "ExternalRequestAction");

            program.AddOrIgnore<bool>(xmlNode, "UseAsFolder");

            //TODO(gjc): edit later for UseAsFolder=true
            if (program.ContainsKey("UseAsFolder"))
            {
                if ((bool)program["UseAsFolder"])
                    return null;
            }

            program.AddOrIgnore<bool>(xmlNode, "AutoValueAssignStepToPhase");

            program.AddOrIgnore<bool>(xmlNode, "AutoValueAssignPhaseToStepOnComplete");

            program.AddOrIgnore<bool>(xmlNode, "AutoValueAssignPhaseToStepOnStopped");

            program.AddOrIgnore<bool>(xmlNode, "AutoValueAssignPhaseToStepOnAborted");

            program.AddOrIgnore(xmlNode, "MainRoutineName");
            program.AddOrIgnore(xmlNode, "PreStateRoutineName");
            program.AddOrIgnore(xmlNode, "FaultRoutineName");

            // Description
            var xmlNodeList = xmlNode.GetElementsByTagName("Description");
            if (xmlNodeList.Count > 0)
            {
                program.Add("Description", xmlNodeList[0].FirstChild.Value);
            }

            // Routines
            JArray routines = new JArray();
            xmlNodeList = xmlNode.GetElementsByTagName("Routine");
            foreach (XmlElement element in xmlNodeList)
            {
                var routineJObj = ToRoutine(element);
                if (routineJObj != null) routines.Add(routineJObj);
            }

            program.Add("Routines", routines);

            // Tags
            JArray tags = new JArray();
            xmlNodeList = xmlNode.GetElementsByTagName("Tag");
            foreach (XmlElement element in xmlNodeList)
            {
                var tag = ToTag(element);
                if (tag != null)
                    tags.Add(SortJObject(tag));
            }

            program.Add("Tags", tags);
            return SortJObject(program);
        }

    }
}