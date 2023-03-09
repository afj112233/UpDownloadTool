using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICSStudio.XmlToJson.Converter
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class PS1734DiscreteConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlFile">PS1734_Discrete.xml</param>
        /// <param name="jsonFile">PS1734_Discrete.json</param>
        public static void XmlToJson(string xmlFile, string jsonFile)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlFile);

                var jObject = ToJObject(doc);

                using (var sw = File.CreateText(jsonFile))
                using (var jw = new JsonTextWriter(sw))
                {
                    jw.Formatting = Newtonsoft.Json.Formatting.Indented;
                    jObject.WriteTo(jw);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }

        private static JObject ToJObject(XmlDocument doc)
        {
            JObject discreteAOPModuleTypes = new JObject();

            // ModuleTypes
            JArray moduleTypes = new JArray();
            discreteAOPModuleTypes.Add("ModuleTypes", moduleTypes);

            UpdateModuleTypes(doc.SelectNodes("AOPModuleTypes/ModuleType"), moduleTypes);

            // Modules
            JArray modules = new JArray();
            discreteAOPModuleTypes.Add("Modules", modules);

            UpdateModules(doc.SelectNodes("AOPModuleTypes/Module"), modules);

            // ModuleDefinitions
            JArray moduleDefinitions = new JArray();
            discreteAOPModuleTypes.Add("ModuleDefinitions", moduleDefinitions);

            UpdateModuleDefinitions(doc.SelectNodes("AOPModuleTypes/ModuleDefinition"), moduleDefinitions);

            // Bus
            var busNode = doc.SelectSingleNode("AOPModuleTypes/Bus");
            if (busNode != null)
            {
                discreteAOPModuleTypes.Add("Bus", ToBus(busNode as XmlElement));
            }

            // ConnectionConfigDefinitions
            JArray connectionConfigDefinitions = new JArray();
            discreteAOPModuleTypes.Add("ConnectionConfigDefinitions", connectionConfigDefinitions);

            UpdateConnectionConfigDefinitions(doc.SelectNodes("AOPModuleTypes/Construction"),
                connectionConfigDefinitions);

            // ConnectionDefinitions
            JArray connectionDefinitions = new JArray();
            discreteAOPModuleTypes.Add("ConnectionDefinitions", connectionDefinitions);

            UpdateConnectionDefinitions(doc.SelectNodes("AOPModuleTypes/Connection"), connectionDefinitions);

            // DataTypeDefinitions
            JArray dataTypeDefinitions = new JArray();
            discreteAOPModuleTypes.Add("DataTypeDefinitions", dataTypeDefinitions);

            UpdateDataTypeDefinitions(doc.SelectNodes("AOPModuleTypes/DataType"), dataTypeDefinitions);

            // DataValueDefinitions
            JArray dataValueDefinitions = new JArray();
            discreteAOPModuleTypes.Add("DataValueDefinitions", dataValueDefinitions);

            UpdateDataValueDefinitions(doc.SelectNodes("AOPModuleTypes/Value"), dataValueDefinitions);

            // StringDefines
            JArray stringDefines = new JArray();
            discreteAOPModuleTypes.Add("StringDefines", stringDefines);

            UpdateStringDefines(doc.SelectNodes("AOPModuleTypes/Strings"), stringDefines);

            return discreteAOPModuleTypes;
        }

        

        private static void UpdateStringDefines(XmlNodeList xmlNodeList, JArray stringDefines)
        {
            if (xmlNodeList != null)
            {
                foreach (XmlElement node in xmlNodeList)
                {
                    stringDefines.Add(ToStringDefinition(node));
                }
            }
        }

        private static void UpdateDataValueDefinitions(XmlNodeList xmlNodeList, JArray dataValueDefinitions)
        {
            if (xmlNodeList != null)
            {
                foreach (XmlElement node in xmlNodeList)
                {
                    dataValueDefinitions.Add(ToDataValueDefinition(node));
                }
            }
        }

        private static void UpdateDataTypeDefinitions(XmlNodeList xmlNodeList, JArray dataTypeDefinitions)
        {
            if (xmlNodeList != null)
            {
                foreach (XmlElement node in xmlNodeList)
                {
                    dataTypeDefinitions.Add(ToDataTypeDefinition(node));
                }
            }
        }



        private static void UpdateConnectionDefinitions(XmlNodeList xmlNodeList, JArray connectionDefinitions)
        {
            if (xmlNodeList != null)
            {
                foreach (XmlElement node in xmlNodeList)
                {
                    connectionDefinitions.Add(ToConnectionDefinition(node));
                }
            }
        }

        private static void UpdateConnectionConfigDefinitions(XmlNodeList xmlNodeList,
            JArray connectionConfigDefinitions)
        {
            if (xmlNodeList != null)
            {
                foreach (XmlElement node in xmlNodeList)
                {
                    connectionConfigDefinitions.Add(ToConnectionConfigDefinition(node));
                }
            }
        }

        private static void UpdateModuleDefinitions(XmlNodeList xmlNodeList, JArray moduleDefinitions)
        {
            if (xmlNodeList != null)
            {
                foreach (XmlElement node in xmlNodeList)
                {
                    moduleDefinitions.Add(ToModuleDefinition(node));
                }
            }

        }

        private static void UpdateModules(XmlNodeList xmlNodeList, JArray modules)
        {
            if (xmlNodeList != null)
            {
                foreach (XmlElement node in xmlNodeList)
                {
                    modules.Add(ToModule(node));
                }
            }
        }

        private static void UpdateModuleTypes(XmlNodeList xmlNodeList, JArray moduleTypes)
        {
            if (xmlNodeList != null)
            {
                foreach (XmlElement node in xmlNodeList)
                {
                    moduleTypes.Add(ToModuleType(node));
                }
            }

        }

        private static JObject ToModuleType(XmlElement node)
        {
            JObject moduleType = new JObject
            {
                {"VendorID", int.Parse(node.Attributes["VendorID"].Value)},
                {"ProductType", int.Parse(node.Attributes["ProductType"].Value)},
                {"ProductCode", int.Parse(node.Attributes["ProductCode"].Value)}
            };

            JArray variants = new JArray();
            moduleType.Add("Variants", variants);

            var variantList = node.GetElementsByTagName("Variant");
            foreach (XmlElement v in variantList)
            {
                JObject variant = new JObject
                {
                    {"MajorRev", int.Parse(v.Attributes["MajorRev"].Value)},
                    {"ModuleID", v.Attributes["ModuleID"].Value},
                    {"ModuleDefinitionID", v.Attributes["ModuleDefinitionID"].Value}
                };

                if (v.HasAttribute("Default"))
                    variant.Add("Default", bool.Parse(v.Attributes["Default"].Value));

                variants.Add(variant);
            }

            return moduleType;
        }

        private static JObject ToModule(XmlElement node)
        {
            JObject module = new JObject
            {
                {"ID", node.Attributes["ID"].Value},
                {"NumberOfInputs", int.Parse(node.Attributes["NumberOfInputs"].Value)},
                {"NumberOfOutputs", int.Parse(node.Attributes["NumberOfOutputs"].Value)},
                {"DriverType", int.Parse(node.Attributes["DriverType"].Value)},
                {"CIPObjectDefinesID", node.Attributes["CIPObjectDefinesID"].Value}
            };

            if(node.HasAttribute("SupportsReset"))
                module.Add("SupportsReset", bool.Parse(node.Attributes["SupportsReset"].Value));

            if(node.HasAttribute("FixedUserName"))
                module.Add("FixedUserName", bool.Parse(node.Attributes["FixedUserName"].Value));

            if(node.HasAttribute("BusID"))
                module.Add("BusID",node.Attributes["BusID"].Value);

            return module;
        }

        private static JObject ToModuleDefinition(XmlElement node)
        {
            JObject moduleDefinition = new JObject
            {
                {"ID", node.Attributes["ID"].Value},
                {"StringsID", node.Attributes["StringsID"].Value}
            };

            XmlElement selectionsNode = node.SelectSingleNode("Selections") as XmlElement;
            XmlElement selectionTreeNode = node.SelectSingleNode("SelectionTree") as XmlElement;

            // Connection

            var connectionNode = selectionsNode?.FirstChild;
            if (connectionNode?.Attributes != null && connectionNode.Attributes["ID"].Value == "Connection")
            {
                JObject connection = new JObject();
                moduleDefinition.Add("Connection", connection);

                connection.Add("StringID", uint.Parse(connectionNode.Attributes["StringID"].Value));

                // Choices
                JArray choices = new JArray();
                connection.Add("Choices", choices);

                foreach (XmlElement childNode in connectionNode.ChildNodes)
                {
                    string connectionID = childNode.Attributes["ID"].Value;

                    JObject choice = new JObject
                    {
                        {"ID", connectionID},
                        {"StringID", uint.Parse(childNode.Attributes["StringID"].Value)}
                    };

                    if (selectionTreeNode != null)
                    {
                        foreach (XmlElement xmlElement in selectionTreeNode.ChildNodes)
                        {
                            if (xmlElement.Attributes["ChoiceID"].Value == connectionID)
                            {
                                XmlElement childElement = xmlElement.FirstChild as XmlElement;
                                if (childElement != null)
                                {
                                    choice.Add("DataFormat", childElement.Attributes["ChoiceID"].Value);
                                    choice.Add("ConfigID", uint.Parse(childElement.Attributes["ConfigID"].Value));
                                    if (childElement.HasAttribute("CommMethod"))
                                        choice.Add("CommMethod",
                                            uint.Parse(childElement.Attributes["CommMethod"].Value));
                                    if (childElement.HasAttribute("FilterMask"))
                                        choice.Add("FilterMask",
                                            uint.Parse(childElement.Attributes["FilterMask"].Value));
                                }
                                else
                                {
                                    if (xmlElement.HasAttribute("ConfigID"))
                                        choice.Add("ConfigID", uint.Parse(xmlElement.Attributes["ConfigID"].Value));
                                    if (xmlElement.HasAttribute("Default"))
                                        choice.Add("Default", bool.Parse(xmlElement.Attributes["Default"].Value));
                                }

                                break;
                            }
                        }
                    }

                    choices.Add(choice);
                }
            }

            // DataFormat
            var dataFormatNode = connectionNode?.NextSibling;
            if (dataFormatNode?.Attributes != null && dataFormatNode.Attributes["ID"].Value == "DataFormat")
            {
                JObject dataFormat = new JObject();
                moduleDefinition.Add("DataFormat", dataFormat);

                dataFormat.Add("StringID", uint.Parse(dataFormatNode.Attributes["StringID"].Value));

                // Choices
                JArray choices = new JArray();
                dataFormat.Add("Choices", choices);

                foreach (XmlElement childNode in dataFormatNode.ChildNodes)
                {
                    JObject choice = new JObject
                    {
                        {"ID", childNode.Attributes["ID"].Value},
                        {"StringID", uint.Parse(childNode.Attributes["StringID"].Value)}
                    };

                    choices.Add(choice);
                }
            }


            return moduleDefinition;
        }

        private static JObject ToBus(XmlElement node)
        {
            JObject bus = new JObject()
            {
                {"ID",node.Attributes["ID"].Value }
            };

            var upstreamNode = node.SelectSingleNode("Upstream");
            if (upstreamNode != null)
            {
                JObject upstream = new JObject();

                var excludeAddressRangeNode = upstreamNode.SelectSingleNode("Exclusions/ExcludeAddressRange") as XmlElement;
                if (excludeAddressRangeNode != null)
                {
                    int minAddress = int.Parse(excludeAddressRangeNode.Attributes["MinAddress"].Value);
                    int maxAddress = int.Parse(excludeAddressRangeNode.Attributes["MaxAddress"].Value);

                    JObject excludeAddressRange = new JObject()
                    {
                        {"MinAddress",minAddress }, {"MaxAddress",maxAddress }
                    };

                    JObject exclusions = new JObject()
                    {
                        {"ExcludeAddressRange",excludeAddressRange }
                    };

                    upstream.Add("Exclusions",exclusions);
                }
                
                bus.Add("Upstream",upstream);
            }

            return bus;
        }

        private static JObject ToConnectionConfigDefinition(XmlElement node)
        {
            JObject definition = new JObject
            {
                {"ConfigID", uint.Parse(node.Attributes["ConfigID"].Value)}
            };

            foreach (XmlElement childNode in node.ChildNodes.OfType<XmlElement>())
            {
                if (childNode.Name == "ConfigTag")
                {
                    JObject configTag = new JObject
                    {
                        {"ValueID", childNode.Attributes["ValueID"].Value},
                        {"Instance", uint.Parse(childNode.Attributes["Instance"].Value)}
                    };

                    XmlElement dataTypeNode = childNode.FirstChild as XmlElement;
                    if (dataTypeNode != null && dataTypeNode.Name == "DataType")
                    {
                        configTag.Add("DataType", dataTypeNode.Attributes["Name"].Value);
                    }

                    definition.Add("ConfigTag", configTag);
                }

                if (childNode.Name == "Connections")
                {
                    JArray connections = new JArray();
                    foreach (XmlElement connectionNode in childNode.ChildNodes)
                    {
                        connections.Add(connectionNode.Attributes["ConnectionID"].Value);
                    }

                    definition.Add("Connections", connections);
                }
            }

            return definition;
        }

        private static JObject ToConnectionDefinition(XmlElement node)
        {
            JObject definition = new JObject
            {
                {"ID", node.Attributes["ID"].Value}
            };

            if (node.HasAttribute("Name"))
                definition.Add("Name", node.Attributes["Name"].Value);

            if (node.HasAttribute("Type"))
                definition.Add("Type", node.Attributes["Type"].Value);

            if (node.HasAttribute("MinRPI"))
                definition.Add("MinRPI", uint.Parse(node.Attributes["MinRPI"].Value));

            if (node.HasAttribute("MaxRPI"))
                definition.Add("MaxRPI", uint.Parse(node.Attributes["MaxRPI"].Value));

            if (node.HasAttribute("RPI"))
                definition.Add("RPI", uint.Parse(node.Attributes["RPI"].Value));

            if (node.HasAttribute("InputCxnPoint"))
                definition.Add("InputCxnPoint", uint.Parse(node.Attributes["InputCxnPoint"].Value));

            if (node.HasAttribute("OutputCxnPoint"))
                definition.Add("OutputCxnPoint", uint.Parse(node.Attributes["OutputCxnPoint"].Value));

            foreach (XmlNode c in node.ChildNodes)
            {
                if (c.NodeType == XmlNodeType.Element)
                {
                    XmlElement childNode = c as XmlElement;

                    if (childNode != null)
                    {
                        if (childNode.Name == "InputTag")
                        {
                            JObject tag = new JObject();

                            XmlElement dataTypeNode = childNode.FirstChild as XmlElement;
                            if (dataTypeNode != null && dataTypeNode.Name == "DataType")
                            {
                                tag.Add("DataType", dataTypeNode.Attributes["Name"].Value);
                            }

                            definition.Add("InputTag", tag);
                        }

                        if (childNode.Name == "OutputTag")
                        {
                            JObject tag = new JObject();

                            XmlElement dataTypeNode = childNode.FirstChild as XmlElement;
                            if (dataTypeNode != null && dataTypeNode.Name == "DataType")
                            {
                                tag.Add("DataType", dataTypeNode.Attributes["Name"].Value);
                            }

                            definition.Add("OutputTag", tag);
                        }

                        if (childNode.Name == "InAliasTag")
                        {
                            JObject tag = new JObject
                            {
                                {"Suffix", childNode.Attributes["Suffix"].Value}
                            };

                            definition.Add("InAliasTag", tag);
                        }

                        if (childNode.Name == "OutAliasTag")
                        {
                            JObject tag = new JObject
                            {
                                {"Suffix", childNode.Attributes["Suffix"].Value}
                            };

                            definition.Add("OutAliasTag", tag);
                        }

                    }

                }
            }

            return definition;
        }

        private static JObject ToDataTypeDefinition(XmlElement node)
        {
            JObject definition = new JObject
            {
                {"Name", node.Attributes["Name"].Value},
                {"Class", node.Attributes["Class"].Value}
            };

            var memberList = node.SelectNodes("Members/Member");
            if (memberList != null)
            {
                JArray members = new JArray();
                foreach (XmlElement m in memberList)
                {
                    JObject member = new JObject
                    {
                        {"Name", m.Attributes["Name"].Value},
                        {"DataType", m.Attributes["DataType"].Value}
                    };

                    if (m.HasAttribute("Hidden"))
                        member.Add("Hidden", bool.Parse(m.Attributes["Hidden"].Value));

                    if (m.HasAttribute("Radix"))
                        member.Add("Radix", m.Attributes["Radix"].Value);

                    if (m.HasAttribute("Min"))
                        member.Add("Min", m.Attributes["Min"].Value);

                    if (m.HasAttribute("Max"))
                        member.Add("Max", m.Attributes["Max"].Value);

                    if (m.HasAttribute("BitNumber"))
                        member.Add("BitNumber", uint.Parse(m.Attributes["BitNumber"].Value));

                    if (m.HasAttribute("Enum"))
                        member.Add("Enum", m.Attributes["Enum"].Value);

                    if (m.HasAttribute("Target"))
                        member.Add("Target", m.Attributes["Target"].Value);

                    members.Add(member);
                }

                definition.Add("Members", members);
            }

            return definition;
        }

        private static JObject ToDataValueDefinition(XmlElement node)
        {
            JObject definition = new JObject {{"ID", node.Attributes["ID"].Value}};

            StringBuilder dataBuilder = new StringBuilder();
            foreach (var dataElement in node.ChildNodes.OfType<XmlText>())
            {
                dataBuilder.Append(string.Join(",", dataElement.Value.Split(',').Select(x => x.Trim())));
            }

            definition.Add("Data", dataBuilder.ToString());

            return definition;
        }

        private static JToken ToStringDefinition(XmlElement node)
        {
            JObject definition = new JObject {{"ID", node.Attributes["ID"].Value}};

            var stringList = node.SelectNodes("String");

            if (stringList != null)
            {
                JArray strings = new JArray();

                foreach (XmlElement stringNode in stringList)
                {
                    JObject stringObject = new JObject {{"ID", uint.Parse(stringNode.Attributes["ID"].Value)}};

                    JArray descriptions = new JArray();
                    stringObject.Add("Descriptions", descriptions);

                    foreach (XmlElement textNode in stringNode.ChildNodes)
                    {
                        JObject text = new JObject
                        {
                            {"LCID", uint.Parse(textNode.Attributes["LCID"].Value)},
                            {"Text", textNode.FirstChild?.Value}
                        };

                        descriptions.Add(text);
                    }

                    strings.Add(stringObject);
                }

                definition.Add("Strings", strings);
            }

            return definition;
        }
    }
}
