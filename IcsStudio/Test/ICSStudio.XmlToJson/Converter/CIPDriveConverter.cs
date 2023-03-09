using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICSStudio.XmlToJson.Converter
{
    class CIPDriveConverter
    {
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
            JObject moduleType = new JObject();

            // Identity
            moduleType.Add("Identity",
                ToIdentity(doc.SelectSingleNode("ModuleTypeInstall/ModuleTypes/ModuleType/Identity") as XmlElement));

            // Ports
            moduleType.Add("Ports",
                ToPorts(doc.SelectSingleNode("ModuleTypeInstall/ModuleTypes/ModuleType/Ports") as XmlElement));

            // Categories
            moduleType.Add("Categories",
                ToCategories(
                    doc.SelectSingleNode("ModuleTypeInstall/ModuleTypes/ModuleType/Categories") as XmlElement));

            // ExtendedProperties
            moduleType.Add("ExtendedProperties",
                ToExtendedProperties(
                    doc.SelectSingleNode("ModuleTypeInstall/ModuleTypes/ModuleType/ExtendedProperties") as XmlElement));

            // Schema/Drive
            moduleType.Add("Schema",
                ToSchema(doc.SelectSingleNode("ModuleTypeInstall/ModuleTypes/ModuleType/Schema") as XmlElement));

            return moduleType;
        }

        private static JToken ToIdentity(XmlElement node)
        {
            JObject identity = new JObject();

            JArray descriptions = new JArray();
            JArray majorRevs = new JArray();

            foreach (var element in node.ChildNodes.OfType<XmlElement>())
            {
                switch (element.Name)
                {
                    case "CatalogNumber":
                        identity.Add("CatalogNumber", element.InnerText);
                        break;

                    case "Description":
                        JObject description = new JObject
                        {
                            {"LCID", uint.Parse(element.Attributes["LCID"].Value)},
                            {"Text", element.InnerText}
                        };

                        descriptions.Add(description);
                        break;

                    case "VendorID":
                        identity.Add("VendorID", int.Parse(element.InnerText));
                        break;

                    case "ProductType":
                        identity.Add("ProductType", int.Parse(element.InnerText));
                        break;

                    case "ProductName":
                        identity.Add("ProductCode", int.Parse(element.Attributes["ProductCode"].Value));
                        identity.Add("ProductName", element.InnerText);
                        break;

                    case "Variant":
                        if (element.HasAttribute("MajorRev"))
                            majorRevs.Add(int.Parse(element.Attributes["MajorRev"].Value));
                        break;

                    case "IconFile":
                        break;

                    default:
                        throw new NotImplementedException("Add code for ToIdentity!");
                }
            }

            if (descriptions.Count > 0)
                identity.Add("Descriptions", descriptions);

            if (majorRevs.Count > 0)
                identity.Add("MajorRevs", majorRevs);

            return identity;
        }

        private static JArray ToPorts(XmlElement node)
        {
            JArray ports = new JArray();

            foreach (var element in node.ChildNodes.OfType<XmlElement>())
            {
                JObject port = new JObject();

                if (element.HasAttribute("Number"))
                    port.Add("Number", int.Parse(element.Attributes["Number"].Value));

                foreach (var subElement in element.ChildNodes.OfType<XmlElement>())
                {
                    switch (subElement.Name)
                    {
                        case "Type":
                            port.Add("Type", subElement.InnerText);
                            break;

                        case "Label":
                            port.Add("Label", subElement.InnerText);
                            break;

                        case "Width":
                            port.Add("Width", int.Parse(subElement.InnerText));
                            break;

                        case "ConnectorOffset":
                            port.Add("ConnectorOffset", int.Parse(subElement.InnerText));
                            break;

                        default:
                            throw new NotImplementedException("Add code for ToPorts!");

                    }
                }

                ports.Add(port);
            }


            return ports;
        }

        private static JArray ToCategories(XmlElement node)
        {
            JArray categories = new JArray();

            foreach (var element in node.ChildNodes.OfType<XmlElement>())
            {
                categories.Add(element.Name);
            }

            return categories;
        }

        private static JObject ToExtendedProperties(XmlElement node)
        {
            JObject extendedProperties = new JObject();

            extendedProperties.Add("PSFile", "Add file here!");

            foreach (var element in node.ChildNodes.OfType<XmlElement>())
            {
                switch (element.Name)
                {
                    case "UnicastOnly":
                        extendedProperties.Add("UnicastOnly", int.Parse(element.InnerText));
                        break;

                    case "MsiUpgradeCode":
                    case "MsiFeatureName":
                    case "PSModuleSpecificFile":
                    case "PVModuleSpecificFile":
                    case "PVCLSID":
                    case "PSCLSID":
                        break;

                    default:
                        throw new NotImplementedException("Add code for ToExtendedProperties!");
                }
            }

            return extendedProperties;
        }

        private static JObject ToSchema(XmlElement node)
        {
            JObject drive = new JObject();

            XmlElement driveElement = node.SelectSingleNode("Drive") as XmlElement;

            if (driveElement != null)
            {
                foreach (var element in driveElement.ChildNodes.OfType<XmlElement>())
                {
                    switch (element.Name)
                    {
                        case "OptionalAttributeRevision":
                            JObject optionalAttributeRevision = new JObject();

                            optionalAttributeRevision.Add("Revision", int.Parse(element.InnerText));

                            if (element.HasAttribute("MinMajorRev"))
                                optionalAttributeRevision.Add("MinMajorRev", int.Parse(element.Attributes["MinMajorRev"].Value));

                            drive.Add("OptionalAttributeRevision", optionalAttributeRevision);

                            break;

                        case "AxisType":
                            drive.Add("AxisType", element.InnerText);
                            break;

                        case "TimeSync":
                            JObject timeSync = new JObject();

                            foreach (var childElement in element.ChildNodes.OfType<XmlElement>())
                            {
                                timeSync.Add(childElement.Name, true);
                            }

                            drive.Add("TimeSync", timeSync);
                            break;

                        case "UpdateableFlash":
                            drive.Add("UpdateableFlash", true);
                            break;

                        case "Cyclic":
                            JObject cyclic = new JObject();

                            foreach (var childElement in element.ChildNodes.OfType<XmlElement>())
                            {
                                cyclic.Add(childElement.Name, int.Parse(childElement.InnerText));
                            }

                            drive.Add("Cyclic", cyclic);
                            break;

                        case "Safety":
                            JObject safety = new JObject();

                            foreach (var childElement in element.ChildNodes.OfType<XmlElement>())
                            {
                                safety.Add(childElement.Name, true);
                            }

                            drive.Add("Safety", safety);
                            break;

                        case "MotionSafety":
                            //drive.Add("MotionSafety", ToMotionSafety(element));
                            break;

                        case "SupportedConfigBits":
                            drive.Add("SupportedConfigBits", ToSupportedConfigBits(element));
                            break;

                        case "SupportedAttributes":
                            JArray supportedAttributes = new JArray();

                            foreach (var childElement in element.ChildNodes.OfType<XmlElement>())
                            {
                                JObject attribute = new JObject();

                                attribute.Add("Attribute", childElement.Name);

                                if (childElement.HasAttribute("MinMajorRev"))
                                    attribute.Add("MinMajorRev",
                                        int.Parse(childElement.Attributes["MinMajorRev"].Value));

                                supportedAttributes.Add(attribute);
                            }

                            drive.Add("SupportedAttributes", supportedAttributes);
                            break;

                        case "PowerStructures":
                            JArray powerStructures = new JArray();

                            foreach (var subElement in element.SelectNodes("PowerStructure").OfType<XmlElement>())
                            {
                                JObject powerStructure = new JObject();

                                if (subElement.HasAttribute("ID"))
                                    powerStructure.Add("ID", int.Parse(subElement.Attributes["ID"].Value));

                                if (subElement.HasAttribute("Integrated"))
                                    powerStructure.Add("Integrated",
                                        bool.Parse(subElement.Attributes["Integrated"].Value));

                                if (subElement.HasAttribute("DutySelect"))
                                    powerStructure.Add("DutySelect", subElement.Attributes["DutySelect"].Value);

                                if (subElement.HasAttribute("Voltage"))
                                    powerStructure.Add("Voltage", subElement.Attributes["Voltage"].Value);

                                foreach (var xmlElement in subElement.ChildNodes.OfType<XmlElement>())
                                {
                                    switch (xmlElement.Name)
                                    {
                                        case "CatalogNumber":
                                            var catalogNumber = ToTextLCIDArray(xmlElement.ChildNodes);
                                            powerStructure.Add("CatalogNumber", catalogNumber);
                                            break;

                                        case "Description":
                                            var description = ToTextLCIDArray(xmlElement.ChildNodes);
                                            powerStructure.Add("Description", description);
                                            break;

                                        case "BusConfiguration":
                                            var busConfiguration = ToBusConfiguration(xmlElement);
                                            powerStructure.Add("BusConfiguration", busConfiguration);
                                            break;

                                        default:
                                            throw new NotImplementedException("add code for PowerStructure!");
                                    }
                                }

                                powerStructures.Add(powerStructure);
                            }


                            drive.Add("PowerStructures", powerStructures);
                            break;

                        case "Feedback":
                            drive.Add("Feedback", ToFeedback(element));
                            break;

                        case "Axes":
                            drive.Add("Axes", ToAxes(element));
                            break;

                        case "Attributes":
                            drive.Add("Attributes", ToAttributes(element));
                            break;

                        default:
                            throw new NotImplementedException("Add code for ToSchema!");
                    }
                }
            }

            return drive;
        }

        private static JArray ToSupportedConfigBits(XmlElement element)
        {
            JArray configBits = new JArray();

            foreach (var subElement in element.ChildNodes.OfType<XmlElement>())
            {
                configBits.Add(subElement.Name);
            }

            return configBits;
        }

        private static JObject ToAttributes(XmlElement element)
        {
            JObject attributes = new JObject();

            foreach (var subElement in element.ChildNodes.OfType<XmlElement>())
            {
                switch (subElement.Name)
                {
                    case "SupportedAttributes":
                        attributes.Add("SupportedAttributes", ToSupportedAttributes(subElement));
                        break;

                    case "SupportedEnums":
                        attributes.Add("SupportedEnums", ToSupportedEnums(subElement));
                        break;

                    case "SupportedEnumsPerAxisConfiguration":
                        attributes.Add("SupportedEnumsPerAxisConfiguration",
                            ToSupportedEnumsPerAxisConfiguration(subElement));
                        break;


                    case "SupportedEnumsPerAxisConfigurationAndFeedbackConfiguration":
                        attributes.Add("SupportedEnumsPerAxisConfigurationAndFeedbackConfiguration",
                            ToSupportedEnumsPerAxisConfigurationAndFeedbackConfiguration(subElement));
                        break;

                    case "CurrentLoopBandwidthScalingFactor":
                        attributes.Add("CurrentLoopBandwidthScalingFactor",
                            ToCurrentLoopBandwidthScalingFactor(subElement));
                        break;

                    case "SupportedMotorTests":
                        attributes.Add("SupportedMotorTests", ToSupportedMotorTests(subElement));
                        break;

                    case "ProductSpecificAttributeDefaults":
                        attributes.Add("ProductSpecificAttributeDefaults",
                            ToProductSpecificAttributeDefaults(subElement));
                        break;

                    case "ProductSpecificAttributeDefaultsAC":
                        //attributes.Add("ProductSpecificAttributeDefaultsAC",
                        //    ToProductSpecificAttributeDefaultsAC(subElement));
                        break;

                    case "SupportedExceptions":
                        attributes.Add("SupportedExceptions", ToSupportedExceptions(subElement));
                        break;

                    case "FeedbackOnlySupportedExceptions":
                        attributes.Add("FeedbackOnlySupportedExceptions",
                            ToSupportedExceptions(subElement));
                        break;

                    default:
                        throw new NotImplementedException("Add code for ToAttributes!");
                }
            }

            return attributes;
        }

        private static JObject ToSupportedExceptions(XmlElement element)
        {
            JObject exceptions = new JObject();

            foreach (var xmlElement in element.ChildNodes.OfType<XmlElement>())
            {
                exceptions.Add(xmlElement.Name, ToExceptionActionList(xmlElement));
            }

            return exceptions;
        }

        private static JArray ToExceptionActionList(XmlElement element)
        {
            JArray list = new JArray();

            foreach (var xmlElement in element.ChildNodes.OfType<XmlElement>())
            {
                JObject exceptionAction = new JObject();

                exceptionAction.Add("Exception", xmlElement.Name);

                if (xmlElement.HasAttribute("MinMajorRev"))
                    exceptionAction.Add("MinMajorRev", int.Parse(xmlElement.Attributes["MinMajorRev"].Value));

                if (xmlElement.HasChildNodes)
                {
                    exceptionAction.Add("Action", ToValueList(xmlElement));
                }

                list.Add(exceptionAction);
            }

            return list;
        }

        private static JArray ToProductSpecificAttributeDefaults(XmlElement element)
        {
            JArray defaults = new JArray();

            foreach (var subElement in element.ChildNodes.OfType<XmlElement>())
            {
                JObject attributeDefault = new JObject
                {
                    {"Attribute", subElement.Name},
                    {"Default", subElement.InnerText}
                };

                if (subElement.HasAttribute("MinMajorRev"))
                    attributeDefault.Add("MinMajorRev", int.Parse(subElement.Attributes["MinMajorRev"].Value));

                defaults.Add(attributeDefault);
            }

            return defaults;
        }

        private static JArray ToSupportedMotorTests(XmlElement element)
        {
            JArray tests = new JArray();

            foreach (var xmlElement in element.ChildNodes.OfType<XmlElement>())
            {
                tests.Add(ToSupportedMotorTest(xmlElement));
            }

            return tests;
        }

        private static JObject ToSupportedMotorTest(XmlElement element)
        {
            JObject test = new JObject();

            test.Add("Name", element.Name);

            JArray methods = new JArray();
            test.Add("Methods", methods);


            foreach (var methodElement in element.ChildNodes.OfType<XmlElement>())
            {
                JObject method = new JObject();

                method.Add("Value", methodElement.Name);

                if (methodElement.HasAttribute("MinMajorRev"))
                    method.Add("MinMajorRev", int.Parse(methodElement.Attributes["MinMajorRev"].Value));

                methods.Add(method);
            }

            return test;
        }

        private static JObject ToCurrentLoopBandwidthScalingFactor(XmlElement element)
        {
            JObject scalingFactor = new JObject();

            foreach (var xmlElement in element.ChildNodes.OfType<XmlElement>())
            {
                scalingFactor.Add(xmlElement.Name, double.Parse(xmlElement.InnerText));
            }

            return scalingFactor;
        }

        private static JArray ToSupportedEnumsPerAxisConfigurationAndFeedbackConfiguration(XmlElement element)
        {
            JArray supportedEnums = new JArray();

            foreach (var enumElement in element.ChildNodes.OfType<XmlElement>())
            {
                supportedEnums.Add(ToSupportedEnumPerAxisConfigurationAndFeedbackConfiguration(enumElement));
            }

            return supportedEnums;

        }

        private static JObject ToSupportedEnumPerAxisConfigurationAndFeedbackConfiguration(XmlElement element)
        {
            JObject supportedEnum = new JObject();

            supportedEnum.Add("Name", element.Name);

            foreach (var xmlElement in element.ChildNodes.OfType<XmlElement>())
            {
                supportedEnum.Add(xmlElement.Name, ToValueListWithFeedback(xmlElement));
            }

            return supportedEnum;
        }

        private static JObject ToValueListWithFeedback(XmlElement element)
        {
            JObject result = new JObject();

            foreach (var xmlElement in element.ChildNodes.OfType<XmlElement>())
            {
                result.Add(xmlElement.Name, ToValueList(xmlElement));
            }

            return result;
        }

        private static JArray ToSupportedEnumsPerAxisConfiguration(XmlElement element)
        {
            JArray supportedEnums = new JArray();

            foreach (var enumElement in element.ChildNodes.OfType<XmlElement>())
            {
                supportedEnums.Add(ToSupportedEnumPerAxisConfiguration(enumElement));
            }

            return supportedEnums;
        }

        private static JObject ToSupportedEnumPerAxisConfiguration(XmlElement element)
        {
            JObject supportedEnum = new JObject();

            supportedEnum.Add("Name", element.Name);

            foreach (var xmlElement in element.ChildNodes.OfType<XmlElement>())
            {
                supportedEnum.Add(xmlElement.Name, ToValueList(xmlElement));
            }


            return supportedEnum;
        }

        private static JArray ToValueList(XmlElement element)
        {
            JArray valueList = new JArray();

            foreach (var valueElement in element.ChildNodes.OfType<XmlElement>())
            {
                JObject enumValue = new JObject();

                enumValue.Add("Value", valueElement.InnerText);

                if (valueElement.HasAttribute("MinMajorRev"))
                    enumValue.Add("MinMajorRev", int.Parse(valueElement.Attributes["MinMajorRev"].Value));

                valueList.Add(enumValue);
            }

            return valueList;

        }

        private static JArray ToSupportedEnums(XmlElement element)
        {
            JArray supportedEnums = new JArray();

            foreach (var subElement in element.ChildNodes.OfType<XmlElement>())
            {
                supportedEnums.Add(ToSupportedEnum(subElement));
            }

            return supportedEnums;
        }

        private static JObject ToSupportedEnum(XmlElement element)
        {
            JObject supportedEnum = new JObject();

            supportedEnum.Add("Name", element.Name);

            if (element.HasAttribute("MinMajorRev"))
                supportedEnum.Add("MinMajorRev", int.Parse(element.Attributes["MinMajorRev"].Value));

            JArray values = new JArray();
            supportedEnum.Add("Values", values);

            foreach (var valueElement in element.ChildNodes.OfType<XmlElement>())
            {
                JObject enumValue = new JObject();

                enumValue.Add("Value", valueElement.InnerText.Trim());

                if (valueElement.HasAttribute("MinMajorRev"))
                    enumValue.Add("MinMajorRev", int.Parse(valueElement.Attributes["MinMajorRev"].Value));

                values.Add(enumValue);
            }

            return supportedEnum;
        }

        private static JObject ToSupportedAttributes(XmlElement element)
        {
            JObject supportedAttributes = new JObject();

            foreach (var subElement in element.ChildNodes.OfType<XmlElement>())
            {
                supportedAttributes.Add(subElement.Name, ToSupportedAttributeList(subElement));
            }

            return supportedAttributes;
        }

        private static JArray ToSupportedAttributeList(XmlElement element)
        {
            JArray list = new JArray();

            foreach (var attributeElement in element.ChildNodes.OfType<XmlElement>())
            {
                JObject attribute = new JObject();

                attribute.Add("Value", attributeElement.Name);

                if (attributeElement.HasAttribute("MinMajorRev"))
                    attribute.Add("MinMajorRev", int.Parse(attributeElement.Attributes["MinMajorRev"].Value));

                list.Add(attribute);
            }

            return list;
        }


        private static JArray ToAxes(XmlElement element)
        {
            JArray axes = new JArray();

            foreach (var axisElement in element.ChildNodes.OfType<XmlElement>())
            {
                axes.Add(ToSchemaAxis(axisElement));
            }

            return axes;
        }

        private static JObject ToSchemaAxis(XmlElement element)
        {
            Contract.Assert(element.Name.StartsWith("Axis"));

            JObject axis = new JObject
            {
                {"Number", int.Parse(element.Name.Substring(4).Trim())}
            };

            foreach (var subElement in element.ChildNodes.OfType<XmlElement>())
            {
                switch (subElement.Name)
                {
                    case "SupportedAxisConfigurations":
                        axis.Add("SupportedAxisConfigurations", ToSupportedAxisConfigurations(subElement));
                        break;

                    case "AllowableFeedbackPorts":
                        axis.Add("AllowableFeedbackPorts", ToAllowableFeedbackPorts(subElement));
                        break;

                    default:
                        throw new NotImplementedException("Add code for ToSchemaAxis!");
                }
            }

            return axis;
        }

        private static JObject ToAllowableFeedbackPorts(XmlElement element)
        {
            JObject feedbackPorts = new JObject();

            foreach (var xmlElement in element.ChildNodes.OfType<XmlElement>())
            {
                JArray portNumbers = new JArray();

                foreach (var subElement in xmlElement.ChildNodes.OfType<XmlElement>())
                {
                    JObject portNumber = new JObject();
                    portNumber.Add("Number", int.Parse(subElement.InnerText));

                    if (subElement.HasAttribute("Default"))
                        portNumber.Add("Default", bool.Parse(subElement.Attributes["Default"].Value));

                    portNumbers.Add(portNumber);
                }

                feedbackPorts.Add(xmlElement.Name, portNumbers);
            }

            return feedbackPorts;
        }

        private static JObject ToSupportedAxisConfigurations(XmlElement element)
        {
            JObject configurations = new JObject();

            JArray values = new JArray();
            configurations.Add("Values", values);

            foreach (var subElement in element.ChildNodes.OfType<XmlElement>())
            {
                switch (subElement.Name)
                {
                    case "Value":
                        values.Add(subElement.InnerText);
                        break;

                    case "DefaultAxisConfigurationValue":
                        configurations.Add("Default", subElement.InnerText);
                        break;

                    default:
                        throw new NotImplementedException("Add code for ToSupportedAxisConfigurations!");
                }
            }

            return configurations;
        }

        private static JObject ToFeedback(XmlElement element)
        {
            JObject feedback = new JObject();

            foreach (var subElement in element.ChildNodes.OfType<XmlElement>())
            {
                switch (subElement.Name)
                {
                    case "Ports":
                        feedback.Add("Ports", ToSchemaPorts(subElement));
                        break;

                    case "DevicePorts":
                        feedback.Add("DevicePorts", ToSchemaDevicePorts(subElement));
                        break;

                    case "Devices":
                        feedback.Add("Devices", ToSchemaDevices(subElement));
                        break;
                    default:
                        throw new NotImplementedException("Add code for ToFeedback!");
                }
            }

            return feedback;
        }

        private static JArray ToSchemaDevices(XmlElement element)
        {
            JArray devices = new JArray();

            foreach (var deviceElement in element.ChildNodes.OfType<XmlElement>())
            {
                Contract.Assert(deviceElement.Name.Equals("Device"));

                devices.Add(ToSchemaDevice(deviceElement));
            }

            return devices;
        }

        private static JObject ToSchemaDevice(XmlElement element)
        {
            JObject device = new JObject();

            device.Add("CardType", int.Parse(element.Attributes["CardType"].Value));
            device.Add("Channels", int.Parse(element.Attributes["Channels"].Value));

            foreach (var subElement in element.ChildNodes.OfType<XmlElement>())
            {
                switch (subElement.Name)
                {
                    case "CatalogNumber":
                        device.Add("CatalogNumber", subElement.InnerText);
                        break;

                    case "FeedbackTypes":
                        JArray types = new JArray();
                        device.Add("FeedbackTypes", types);

                        foreach (var typeElement in subElement.ChildNodes.OfType<XmlElement>())
                        {
                            JObject type = new JObject();

                            type.Add("Value", typeElement.FirstChild.InnerText);

                            if (typeElement.HasAttribute("MinMajorRev"))
                                type.Add("MinMajorRev", int.Parse(typeElement.Attributes["MinMajorRev"].Value));

                            types.Add(type);
                        }

                        break;

                    case "RegistrationInputs":
                        device.Add("RegistrationInputs", int.Parse(subElement.InnerText));
                        break;


                    case "AxisFeatures":
                        JArray features = new JArray();
                        device.Add("AxisFeatures", features);

                        foreach (var valueElement in subElement.ChildNodes.OfType<XmlElement>())
                        {
                            features.Add(valueElement.InnerText);
                        }

                        break;

                    case "HomeSequence":
                        JArray homeSequence = new JArray();
                        device.Add("HomeSequence", homeSequence);

                        foreach (var valueElement in subElement.ChildNodes.OfType<XmlElement>())
                        {
                            homeSequence.Add(valueElement.InnerText);
                        }

                        break;

                    default:
                        throw new NotImplementedException("Add code for ToSchemaDevice!");
                }
            }

            return device;
        }

        private static JArray ToSchemaDevicePorts(XmlElement element)
        {
            JArray devicePorts = new JArray();

            foreach (var devicePortElement in element.ChildNodes.OfType<XmlElement>())
            {
                Contract.Assert(devicePortElement.Name.Equals("DevicePort"));

                JObject devicePort = new JObject();

                devicePort.Add("Number",
                    int.Parse(devicePortElement.Attributes["Number"].Value));
                devicePort.Add("FeedbackPortNumber",
                    int.Parse(devicePortElement.Attributes["FeedbackPortNumber"].Value));

                if (devicePortElement.HasAttribute("DefaultCardType"))
                    devicePort.Add("DefaultCardType", int.Parse(devicePortElement.Attributes["DefaultCardType"].Value));

                devicePorts.Add(devicePort);
            }

            return devicePorts;
        }

        private static JArray ToSchemaPorts(XmlElement element)
        {
            JArray ports = new JArray();

            foreach (var portElement in element.ChildNodes.OfType<XmlElement>())
            {
                Contract.Assert(portElement.Name.Equals("Port"));

                JObject port = new JObject
                {
                    {"Number", int.Parse(portElement.Attributes["Number"].Value)}
                };

                if (portElement.HasAttribute("Hidden"))
                    port.Add("Hidden", bool.Parse(portElement.Attributes["Hidden"].Value));

                foreach (var subElement in portElement.ChildNodes.OfType<XmlElement>())
                {
                    switch (subElement.Name)
                    {
                        case "Description":
                            port.Add("Description", ToTextLCIDArray(subElement.ChildNodes));
                            break;
                        default:
                            throw new NotImplementedException("Add code for ToSchemaPorts!");
                    }
                }

                ports.Add(port);
            }

            return ports;
        }

        private static JObject ToBusConfiguration(XmlElement element)
        {
            JObject busConfiguration = new JObject();

            JArray busCompatibilityIDList = new JArray();
            busConfiguration.Add("BusCompatibilityID", busCompatibilityIDList);

            foreach (var subElement in element.ChildNodes.OfType<XmlElement>())
            {
                switch (subElement.Name)
                {
                    case "BusCompatibilityID":
                        JObject busCompatibilityID = new JObject();

                        busCompatibilityID.Add("ID", int.Parse(subElement.InnerText));

                        if (subElement.HasAttribute("MinMajorRev"))
                            busCompatibilityID.Add("MinMajorRev",
                                int.Parse(subElement.Attributes["MinMajorRev"].Value));

                        busCompatibilityIDList.Add(busCompatibilityID);
                        break;

                    case "VerifyJumper":
                        JObject verifyJumper = new JObject();
                        verifyJumper.Add("Value", true);

                        if (subElement.HasAttribute("MaxMajorRev"))
                            verifyJumper.Add("MaxMajorRev", int.Parse(subElement.Attributes["MaxMajorRev"].Value));

                        busConfiguration.Add("VerifyJumper", verifyJumper);
                        break;

                    case "RelativeRating":
                        busConfiguration.Add("RelativeRating", int.Parse(subElement.InnerText));
                        break;

                    case "MaxBusMasters":
                        busConfiguration.Add("MaxBusMasters", int.Parse(subElement.InnerText));
                        break;

                    case "MaxBusFollowers":
                        busConfiguration.Add("MaxBusFollowers", int.Parse(subElement.InnerText));
                        break;

                    case "MaxInGroup":
                        busConfiguration.Add("MaxInGroup", int.Parse(subElement.InnerText));
                        break;

                    default:
                        throw new NotImplementedException("Add code for ToBusConfiguration!");
                }
            }

            return busConfiguration;
        }

        private static JArray ToTextLCIDArray(XmlNodeList nodeList)
        {
            JArray array = new JArray();

            foreach (var element in nodeList.OfType<XmlElement>())
            {
                JObject textLCID = new JObject
                {
                    {"Text", element.InnerText},
                    {"LCID", uint.Parse(element.Attributes["LCID"].Value)}
                };

                array.Add(textLCID);
            }

            return array;
        }
    }
}
