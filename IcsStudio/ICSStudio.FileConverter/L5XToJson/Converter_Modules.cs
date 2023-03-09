using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using AutoMapper;
using ICSStudio.Cip;
using ICSStudio.FileConverter.L5XToJson.Objects;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Utils;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace ICSStudio.FileConverter.L5XToJson
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public partial class Converter
    {
        public static JObject ToDeviceModule(XmlElement node, List<string> nameList)
        {
            var type = (CipDeviceType)int.Parse(node.Attributes["ProductType"].Value.Trim());
            switch (type)
            {
                case CipDeviceType.CIPMotionDrive:
                    return ToCIPMotionDrive(node);
                case CipDeviceType.CommunicationsAdapter:
                    return ToCommAdapter(node);
                case CipDeviceType.ProgrammableLogicController:
                    return ToPLC(node);
                case CipDeviceType.GeneralPurposeDiscreteIO:
                    return ToGeneralPurposeDiscreteIO(node, nameList);
                case CipDeviceType.GeneralPurposeAnalogIO:
                    return ToGeneralPurposeAnalogIO(node, nameList);
                case CipDeviceType.Generic:
                    return ToGenericModule(node);
                default:
                    Console.WriteLine($"Unsupported CIPDeviceType {type}");
                    Debug.Assert(false);
                    return ToDeviceCommon(node);
            }
        }

        public static JObject ToDeviceCommon(XmlElement node)
        {
            // ReSharper disable once UnusedVariable
            var productType = (CipDeviceType)int.Parse(node.Attributes["ProductType"].Value.Trim());

            JObject module = JObject.FromObject(new DeviceModule(node));

            module.AddOrIgnore(node, "Use");

            var xmlNodeList = node.GetElementsByTagName("Port");
            JArray ports = new JArray();
            foreach (XmlElement element in xmlNodeList)
            {
                Debug.Assert(element != null);
                Port port = new Port(element);
                ports.Add(JObject.FromObject(port));
            }

            module.Add("Ports", ports);

            return SortJObject(module);
        }

        private static JObject ToCIPMotionDrive(XmlElement node)
        {
            var module = ToDeviceCommon(node);

            var configDataNode = node.SelectSingleNode("Communications/ConfigData/Data")?.FirstChild;
            if (!string.IsNullOrEmpty(configDataNode?.Value))
            {
                var abCIPDrive = AB_CIP_Drive_C_2.FromCDATA(configDataNode?.Value);
                CIPMotionDriveConfigData configData = Mapper.Map<CIPMotionDriveConfigData>(abCIPDrive);
                module.Add("ConfigData", JObject.FromObject(configData));
            }

            // ExtendedProperties
            JObject extObject = new JObject();
            JObject pubObject = new JObject();

            var publicNode = node.SelectSingleNode("ExtendedProperties/public");

            foreach (XmlElement childNode in publicNode.ChildNodes)
            {
                pubObject.Add(childNode.Name, childNode.FirstChild.Value);
            }

            extObject.Add("Public", pubObject);
            module.Add("ExtendedProperties", extObject);

            return SortJObject(module);
        }

        public static JObject ToCommAdapter(XmlElement node)
        {
            var module = ToDeviceCommon(node);

            var communication = new JObject();

            var commMethodAttr = node.SelectSingleNode("Communications")?.Attributes["CommMethod"];
            if (commMethodAttr != null)
            {
                communication.Add("CommMethod", uint.Parse(commMethodAttr.Value));
            }

            var conns = new JArray();
            foreach (XmlElement connection in node.SelectNodes("Communications/Connections/Connection"))
            {
                var conn = ToConnection(connection);
                conns.Add(conn);
            }

            communication.Add("Connections", conns);
            module.Add("Communications", communication);

            JObject ext = new JObject();
            var extNode = node.SelectSingleNode("ExtendedProperties/public");
            var p = new JObject();
            p.Add("ConfigID", extNode.SelectSingleNode("ConfigID").FirstChild.Value);
            p.Add("CatNum", extNode.SelectSingleNode("CatNum").FirstChild.Value);
            ext.Add("Public", p);
            module.Add("ExtendedProperties", ext);
            return SortJObject(module);
        }

        private static JObject ToPLC(XmlElement node)
        {
            var module = ToDeviceCommon(node);

            return module;
        }

        public static JObject ToGeneralPurposeDiscreteIO(XmlElement node, List<string> nameList)
        {
            var module = ToDeviceCommon(node);
            var code = (CipDeviceCode)(int)module["ProductCode"];

            var communication = new JObject();

            var comms = node.SelectSingleNode("Communications") as XmlElement;
            if (comms.HasAttribute("CommMethod"))
            {
                var method = uint.Parse(comms.Attributes["CommMethod"].Value);
                communication.Add("CommMethod", method);
            }

            var conns = new JArray();
            foreach (XmlElement connection in node.SelectNodes("Communications/Connections/Connection"))
            {
                var conn = ToConnection(connection);
                conns.Add(conn);
            }

            communication.Add("Connections", conns);


            //TODO(gjc): Listen Only Rack is ConfigData
            var configTagNode = comms.SelectSingleNode("ConfigTag") as XmlElement;
            if (configTagNode != null)
            {
                var config = ToGPIOConfigTag(code, configTagNode);
                DataParse.ParseModuleTagComments(config, configTagNode);
                communication.Add("ConfigTag", config);
            }

            module.Add("Communications", communication);

            JObject ext = new JObject();
            var extNode = node.SelectSingleNode("ExtendedProperties/public");
            var p = new JObject
            {
                { "ConfigID", extNode.SelectSingleNode("ConfigID").FirstChild.Value },
                { "CatNum", extNode.SelectSingleNode("CatNum").FirstChild.Value }
            };
            ext.Add("Public", p);
            module.Add("ExtendedProperties", ext);

            HandleEmptyModuleName(module, nameList);

            return SortJObject(module);
        }

        private static JObject ToGeneralPurposeAnalogIO(XmlElement node, List<string> nameList)
        {
            var module = ToDeviceCommon(node);
            var code = (CipDeviceCode)(int)module["ProductCode"];

            var communication = new JObject();

            var comms = node.SelectSingleNode("Communications") as XmlElement;
            if (comms.HasAttribute("CommMethod"))
            {
                var method = uint.Parse(comms.Attributes["CommMethod"].Value);
                communication.Add("CommMethod", method);
            }

            var conns = new JArray();
            foreach (XmlElement connection in node.SelectNodes("Communications/Connections/Connection"))
            {
                var conn = ToConnection(connection);
                conns.Add(conn);
            }

            communication.Add("Connections", conns);

            var configTagNode = comms.SelectSingleNode("ConfigTag") as XmlElement;
            if (configTagNode != null)
            {
                var config = ToGPIOConfigTag(code, configTagNode);
                communication.Add("ConfigTag", config);
            }

            module.Add("Communications", communication);

            JObject ext = new JObject();
            var extNode = node.SelectSingleNode("ExtendedProperties/public");
            var p = new JObject
            {
                { "ConfigID", extNode.SelectSingleNode("ConfigID").FirstChild.Value },
                { "CatNum", extNode.SelectSingleNode("CatNum").FirstChild.Value }
            };
            ext.Add("Public", p);
            module.Add("ExtendedProperties", ext);

            HandleEmptyModuleName(module, nameList);

            return SortJObject(module);
        }

        private static JObject ToGenericModule(XmlElement node)
        {
            var module = ToDeviceCommon(node);

            var communications = new JObject();
            module.Add("Communications", communications);

            var communicationsNode = node.SelectSingleNode("Communications") as XmlElement;

            communications.AddOrIgnore<uint>(communicationsNode, "CommMethod");
            communications.AddOrIgnore<uint>(communicationsNode, "PrimCxnInputSize");
            communications.AddOrIgnore<uint>(communicationsNode, "PrimCxnOutputSize");

            // Connections

            var connections = new JArray();
            foreach (XmlElement connection in node.SelectNodes("Communications/Connections/Connection"))
            {
                var conn = ToConnection(connection);

                conn.AddOrIgnore<int>(connection, "InputCxnPoint");
                conn.AddOrIgnore<int>(connection, "OutputCxnPoint");
                conn.AddOrIgnore<int>(connection, "InputSize");
                conn.AddOrIgnore<int>(connection, "OutputSize");

                connections.Add(conn);
            }

            communications.Add("Connections", connections);

            //ConfigTag
            var configTagNode = communicationsNode.SelectSingleNode("ConfigTag") as XmlElement;
            if (configTagNode != null)
            {
                JObject configTag = new JObject();

                configTag.Add<int>(configTagNode, "ConfigSize");
                configTag.Add<ExternalAccess>(configTagNode, "ExternalAccess");
                configTag.Add("DataType",
                    configTagNode.SelectSingleNode("Data/Structure").Attributes["DataType"].Value);

                DataParse.ParseModuleTagData(configTag, configTagNode);

                communications.Add("ConfigTag", configTag);
            }

            return module;
        }

        private static JObject ToGPIOConfigTag(CipDeviceCode code, XmlElement node)
        {
            var config = new JObject();

            config.Add<int>(node, "ConfigSize");
            config.Add<ExternalAccess>(node, "ExternalAccess");

            switch (code)
            {
                case CipDeviceCode.B1734IB4:
                case CipDeviceCode.B1734OB4:
                case CipDeviceCode.B1734OB8:
                case CipDeviceCode.EmbeddedIO:
                case CipDeviceCode.B1734IB8:
                case CipDeviceCode.B1734IE4C:
                case CipDeviceCode.B1734OE4C:
                case CipDeviceCode.ICDIQ16:
                case CipDeviceCode.ICDOV16:
                case CipDeviceCode.ICDIR4:
                    var dataNodes = node.SelectNodes("Data");

                    Debug.Assert(dataNodes.Count == 2);
                    Debug.Assert(dataNodes[0].Attributes["Format"].Value.Equals("L5K"));
                    Debug.Assert(dataNodes[1].Attributes["Format"].Value.Equals("Decorated"));

                    var text = (dataNodes[0] as XmlElement).InnerText;
                    text = text.Replace("[", "");
                    text = text.Replace("]", "");
                    var result = text.Split(',');
                    var array = new JArray();
                    for (int i = 0; i < result.Length; i++)
                    {
                        array.Add(int.Parse(result[i].Trim()));
                    }

                    config.Add("Data", array);
                    config.Add("DataType", dataNodes[1].SelectSingleNode("Structure").Attributes["DataType"].Value);
                    break;
                default:
                    Debug.WriteLine($"Unsupported DeviceCode {code}");
                    break;
            }

            return SortJObject(config);
        }

        private static JObject ToConnection(XmlElement node)
        {
            var conn = ToConnectionCommon(node);
            var input = node.SelectSingleNode("InputTag") as XmlElement;
            if (input != null)
            {
                var inputTag = new JObject();

                inputTag.Add<ExternalAccess>(input, "ExternalAccess");

                inputTag.Add("DataType",
                    input.SelectSingleNode("Data/Structure").Attributes["DataType"].Value);
                DataParse.ParseModuleTagData(inputTag, input);
                DataParse.ParseModuleTagComments(inputTag,input);
                conn.Add("InputTag", inputTag);
            }

            var output = node.SelectSingleNode("OutputTag") as XmlElement;
            if (output != null)
            {
                var outputTag = new JObject();
                outputTag.Add<ExternalAccess>(output, "ExternalAccess");
                outputTag.Add("DataType", output.SelectSingleNode("Data/Structure").Attributes["DataType"].Value);
                DataParse.ParseModuleTagData(outputTag, output);
                DataParse.ParseModuleTagComments(outputTag, output);
                conn.Add("OutputTag", outputTag);
            }

            return SortJObject(conn);
        }

        private static JObject ToConnectionCommon(XmlElement node)
        {
            var conn = new JObject();

            conn.Add(node, "Name");
            conn.Add<int>(node, "RPI");
            conn.Add(node, "Type");
            conn.Add<int>(node, "EventID");
            conn.Add<bool>(node, "ProgrammaticallySendEventTrigger");

            conn.AddOrIgnore<bool>(node, "Unicast");

            return SortJObject(conn);
        }

        private static void HandleEmptyModuleName(JObject module, List<string> nameList)
        {
            string name = module["Name"].ToString();

            if (string.IsNullOrEmpty(name))
            {
                string catalogNumber = module["CatalogNumber"].ToString();
                string address = string.Empty;

                var ports = module["Ports"] as JArray;
                if (ports != null)
                {
                    var port = ports[0];

                    address = port["Address"].ToString();

                }

                string newName = catalogNumber.RemoveSeries() + "_" + address;
                newName = newName.Replace('-', '_');

                newName = CreateName(newName, nameList);

                module["Name"] = newName;
            }

        }

        private static string CreateName(string name, List<string> nameList)
        {
            int index = 0;
            string newName = name;

            while (true)
            {
                if (!nameList.Contains(newName, StringComparer.OrdinalIgnoreCase))
                    return newName;

                index++;

                newName = $"{name}{index}";
            }
        }
    }
}
