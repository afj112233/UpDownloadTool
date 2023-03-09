using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Xml;
using ICSStudio.Database.Database;
using ICSStudio.Database.Table.Motion;
using ICSStudio.FileConverter.JsonToL5X.Model;
using ICSStudio.FileConverter.L5XToJson.Objects;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.Utils;
using ConnectionType = ICSStudio.FileConverter.JsonToL5X.Model.ConnectionType;
using DeviceModule = ICSStudio.SimpleServices.DeviceModule.DeviceModule;
using PortType = ICSStudio.FileConverter.JsonToL5X.Model.PortType;

namespace ICSStudio.FileConverter.JsonToL5X
{
    public static partial class Converter
    {
        private static ModuleType CreateLocalModuleType()
        {
            ModuleType localModule = new ModuleType
            {
                Name = "Local",
                CatalogNumber = "1756-L84E",
                Vendor = 1,
                ProductType = 14,
                ProductCode = 167,
                Major = 32,
                Minor = 11,
                ParentModule = "Local",
                ParentModPortId = 1,
                Inhibited = BoolEnum.@false,
                MajorFault = BoolEnum.@true,
                EKey = new ModuleEKeyType { State = EKeyingStateEnum.Disabled }
            };

            PortType icpPort = new PortType
            {
                Id = 1,
                Address = "0",
                Type = "ICP",
                Upstream = BoolEnum.@false,
                Bus = new BusType { Size = 17, SizeSpecified = true }
            };

            PortType ethernetPort = new PortType
            {
                Id = 2,
                Type = "Ethernet",
                Upstream = BoolEnum.@false,
                Bus = new BusType()
            };

            localModule.Ports = new[] { icpPort, ethernetPort };

            return localModule;
        }

        private static ModuleType ToModuleType(DeviceModule deviceModule)
        {
            // ignore local
            if (deviceModule.Name.Equals("Local", StringComparison.OrdinalIgnoreCase))
                return null;

            // ignore embedded io
            LocalModule localModule = deviceModule.ParentModule as LocalModule;
            if (localModule != null)
            {
                var port = localModule.GetPortById(deviceModule.ParentModPortId);
                if (port.Type != SimpleServices.DeviceModule.PortType.Ethernet)
                    return null;
            }

            CommunicationsAdapter adapter = deviceModule as CommunicationsAdapter;
            if (adapter != null)
            {
                return AdapterToModuleType(adapter);
            }

            CIPMotionDrive motionDrive = deviceModule as CIPMotionDrive;
            if (motionDrive != null)
            {
                return MotionDriveToModuleType(motionDrive);
            }

            DiscreteIO discreteIO = deviceModule as DiscreteIO;
            if (discreteIO != null)
            {
                return DiscreteIOToModuleType(discreteIO);
            }

            AnalogIO analogIO = deviceModule as AnalogIO;
            if (analogIO != null)
            {
                return AnalogIOToModuleType(analogIO);
            }

            GeneralEthernet generalEthernet = deviceModule as GeneralEthernet;
            if (generalEthernet != null)
            {
                return GeneralEthernetToModuleType(generalEthernet);
            }

            //TODO(gjc): add code here

            return null;
        }


        private static ModuleType GeneralEthernetToModuleType(GeneralEthernet generalEthernet)
        {
            if (generalEthernet == null)
                return null;

            ModuleType moduleType = new ModuleType();

            moduleType.Name = generalEthernet.Name;
            moduleType.CatalogNumber = generalEthernet.CatalogNumber;
            moduleType.Vendor = (ushort)generalEthernet.Vendor;
            moduleType.ProductType = (ushort)generalEthernet.ProductType;
            moduleType.ProductCode = (ushort)generalEthernet.ProductCode;
            moduleType.Major = (byte)generalEthernet.Major;
            moduleType.Minor = (byte)generalEthernet.Minor;
            moduleType.ParentModule = generalEthernet.ParentModule.Name;
            moduleType.ParentModPortId = (ushort)generalEthernet.ParentModPortId;
            moduleType.Inhibited = generalEthernet.Inhibited ? BoolEnum.@true : BoolEnum.@false;
            moduleType.MajorFault = generalEthernet.MajorFault ? BoolEnum.@true : BoolEnum.@false;

            moduleType.EKey = new ModuleEKeyType()
            {
                State = ToEKeyingStateEnum(generalEthernet.EKey)
            };

            //ports
            List<PortType> portTypes = new List<PortType>();

            foreach (var port in generalEthernet.Ports)
            {
                PortType portType = new PortType();

                portType.Id = (ushort)port.Id;
                portType.Address = port.Address;
                portType.Type = port.Type.ToString();
                portType.Upstream =
                    port.Upstream
                        ? BoolEnum.@true
                        : BoolEnum.@false;

                portTypes.Add(portType);
            }

            moduleType.Ports = portTypes.ToArray();
            //end ports

            moduleType.Communications = ToCommunicationsType(generalEthernet);

            return moduleType;
        }

        private static ModuleType AnalogIOToModuleType(AnalogIO analogIO)
        {
            if (analogIO == null)
                return null;

            ModuleType moduleType = new ModuleType();

            moduleType.Name = analogIO.Name;
            moduleType.CatalogNumber = analogIO.CatalogNumber;
            moduleType.Vendor = (ushort)analogIO.Vendor;
            moduleType.ProductType = (ushort)analogIO.ProductType;
            moduleType.ProductCode = (ushort)analogIO.ProductCode;
            moduleType.Major = (byte)analogIO.Major;
            moduleType.Minor = (byte)analogIO.Minor;
            moduleType.ParentModule = analogIO.ParentModule.Name;
            moduleType.ParentModPortId = (ushort)analogIO.ParentModPortId;
            moduleType.Inhibited = analogIO.Inhibited ? BoolEnum.@true : BoolEnum.@false;
            moduleType.MajorFault = analogIO.MajorFault ? BoolEnum.@true : BoolEnum.@false;

            moduleType.EKey = new ModuleEKeyType()
            {
                State = ToEKeyingStateEnum(analogIO.EKey)
            };

            //ports
            List<PortType> portTypes = new List<PortType>();

            foreach (var port in analogIO.Ports)
            {
                PortType portType = new PortType();

                portType.Id = (ushort)port.Id;
                portType.Address = port.Address;
                portType.Type = port.Type.ToString();
                portType.Upstream =
                    port.Upstream
                        ? BoolEnum.@true
                        : BoolEnum.@false;

                portTypes.Add(portType);
            }

            moduleType.Ports = portTypes.ToArray();
            //end ports

            moduleType.Communications = ToCommunicationsType(analogIO);

            moduleType.ExtendedProperties = ToExtendedProperties(analogIO);

            return moduleType;
        }

        private static ModuleType DiscreteIOToModuleType(DiscreteIO discreteIO)
        {
            if (discreteIO == null)
                return null;

            ModuleType moduleType = new ModuleType();

            moduleType.Name = discreteIO.Name;
            moduleType.CatalogNumber = discreteIO.CatalogNumber;
            moduleType.Vendor = (ushort)discreteIO.Vendor;
            moduleType.ProductType = (ushort)discreteIO.ProductType;
            moduleType.ProductCode = (ushort)discreteIO.ProductCode;
            moduleType.Major = (byte)discreteIO.Major;
            moduleType.Minor = (byte)discreteIO.Minor;
            moduleType.ParentModule = discreteIO.ParentModule.Name;
            moduleType.ParentModPortId = (ushort)discreteIO.ParentModPortId;
            moduleType.Inhibited = discreteIO.Inhibited ? BoolEnum.@true : BoolEnum.@false;
            moduleType.MajorFault = discreteIO.MajorFault ? BoolEnum.@true : BoolEnum.@false;

            moduleType.EKey = new ModuleEKeyType()
            {
                State = ToEKeyingStateEnum(discreteIO.EKey)
            };

            //ports
            List<PortType> portTypes = new List<PortType>();

            foreach (var port in discreteIO.Ports)
            {
                PortType portType = new PortType();

                portType.Id = (ushort)port.Id;
                portType.Address = port.Address;
                portType.Type = port.Type.ToString();
                portType.Upstream =
                    port.Upstream
                        ? BoolEnum.@true
                        : BoolEnum.@false;

                portTypes.Add(portType);
            }

            moduleType.Ports = portTypes.ToArray();
            //end ports

            moduleType.Communications = ToCommunicationsType(discreteIO);

            moduleType.ExtendedProperties = ToExtendedProperties(discreteIO);

            return moduleType;
        }

        private static ModuleType MotionDriveToModuleType(CIPMotionDrive motionDrive)
        {
            if (motionDrive == null)
                return null;

            ModuleType moduleType = new ModuleType();

            moduleType.Name = motionDrive.Name;
            moduleType.CatalogNumber = motionDrive.CatalogNumber;
            moduleType.Vendor = (ushort)motionDrive.Vendor;
            moduleType.ProductType = (ushort)motionDrive.ProductType;
            moduleType.ProductCode = (ushort)motionDrive.ProductCode;
            moduleType.Major = (byte)motionDrive.Major;
            moduleType.Minor = (byte)motionDrive.Minor;
            moduleType.ParentModule = motionDrive.ParentModule.Name;
            moduleType.ParentModPortId = (ushort)motionDrive.ParentModPortId;
            moduleType.Inhibited = motionDrive.Inhibited ? BoolEnum.@true : BoolEnum.@false;
            moduleType.MajorFault = motionDrive.MajorFault ? BoolEnum.@true : BoolEnum.@false;

            moduleType.EKey = new ModuleEKeyType()
            {
                State = ToEKeyingStateEnum(motionDrive.EKey)
            };

            //ports
            List<PortType> portTypes = new List<PortType>();

            foreach (var port in motionDrive.Ports)
            {
                PortType portType = new PortType();

                portType.Id = (ushort)port.Id;
                portType.Address = port.Address;
                portType.Type = port.Type.ToString();
                portType.Upstream =
                    port.Upstream
                        ? BoolEnum.@true
                        : BoolEnum.@false;

                portTypes.Add(portType);
            }

            moduleType.Ports = portTypes.ToArray();
            //end ports

            moduleType.Communications = ToCommunicationsType(motionDrive);

            moduleType.ExtendedProperties = ToExtendedProperties(motionDrive);

            return moduleType;
        }

        private static ModuleType AdapterToModuleType(CommunicationsAdapter adapter)
        {
            if (adapter == null)
                return null;

            ModuleType moduleType = new ModuleType();

            moduleType.Name = adapter.Name;
            moduleType.CatalogNumber = adapter.CatalogNumber;
            moduleType.Vendor = (ushort)adapter.Vendor;
            moduleType.ProductType = (ushort)adapter.ProductType;
            moduleType.ProductCode = (ushort)adapter.ProductCode;
            moduleType.Major = (byte)adapter.Major;
            moduleType.Minor = (byte)adapter.Minor;
            moduleType.ParentModule = adapter.ParentModule.Name;
            moduleType.ParentModPortId = (ushort)adapter.ParentModPortId;
            moduleType.Inhibited = adapter.Inhibited ? BoolEnum.@true : BoolEnum.@false;
            moduleType.MajorFault = adapter.MajorFault ? BoolEnum.@true : BoolEnum.@false;

            moduleType.EKey = new ModuleEKeyType()
            {
                State = ToEKeyingStateEnum(adapter.EKey)
            };

            //ports
            List<PortType> portTypes = new List<PortType>();

            foreach (var port in adapter.Ports)
            {
                PortType portType = new PortType();

                portType.Id = (ushort)port.Id;
                portType.Address = port.Address;
                portType.Type = port.Type.ToString();
                portType.Upstream =
                    port.Upstream
                        ? BoolEnum.@true
                        : BoolEnum.@false;

                if (port.Type != SimpleServices.DeviceModule.PortType.Ethernet)
                {
                    portType.Bus = new BusType
                    {
                        Size = (ushort)port.Bus.Size,
                        SizeSpecified = true
                    };
                }

                portTypes.Add(portType);
            }

            moduleType.Ports = portTypes.ToArray();
            //end ports

            moduleType.Communications = ToCommunicationsType(adapter);

            moduleType.ExtendedProperties = ToExtendedProperties(adapter);

            return moduleType;
        }

        private static XmlElement ToExtendedProperties(CommunicationsAdapter adapter)
        {
            XmlDocument xmlDocument = new XmlDocument();

            XmlElement extendedProperties = xmlDocument.CreateElement("public");

            XmlNode xmlNode = xmlDocument.CreateNode("element", "ConfigID", "");
            xmlNode.InnerText = adapter.ConfigID.ToString();
            extendedProperties.AppendChild(xmlNode);

            xmlNode = xmlDocument.CreateNode("element", "CatNum", "");
            xmlNode.InnerText = adapter.CatalogNumber.RemoveSeries();
            extendedProperties.AppendChild(xmlNode);

            return extendedProperties;
        }

        private static XmlElement ToExtendedProperties(CIPMotionDrive motionDrive)
        {
            XmlDocument xmlDocument = new XmlDocument();

            XmlElement extendedProperties = xmlDocument.CreateElement("public");

            XmlNode xmlNode = xmlDocument.CreateNode("element", "Vendor", "");
            xmlNode.InnerText = motionDrive.ExtendedProperties.Public["Vendor"];
            extendedProperties.AppendChild(xmlNode);

            xmlNode = xmlDocument.CreateNode("element", "CatNum", "");
            xmlNode.InnerText = motionDrive.ExtendedProperties.Public["CatNum"];
            extendedProperties.AppendChild(xmlNode);

            if (motionDrive.ExtendedProperties.Public.ContainsKey("FeedbackDevice1"))
            {
                xmlNode = xmlDocument.CreateNode("element", "FeedbackDevice1", "");
                xmlNode.InnerText = motionDrive.ExtendedProperties.Public["FeedbackDevice1"];
                extendedProperties.AppendChild(xmlNode);
            }

            if (motionDrive.ExtendedProperties.Public.ContainsKey("FeedbackDevice2"))
            {
                xmlNode = xmlDocument.CreateNode("element", "FeedbackDevice2", "");
                xmlNode.InnerText = motionDrive.ExtendedProperties.Public["FeedbackDevice2"];
                extendedProperties.AppendChild(xmlNode);
            }

            if (motionDrive.ExtendedProperties.Public.ContainsKey("FeedbackDevice3"))
            {
                xmlNode = xmlDocument.CreateNode("element", "FeedbackDevice3", "");
                xmlNode.InnerText = motionDrive.ExtendedProperties.Public["FeedbackDevice3"];
                extendedProperties.AppendChild(xmlNode);
            }

            if (motionDrive.ExtendedProperties.Public.ContainsKey("FeedbackDevice4"))
            {
                xmlNode = xmlDocument.CreateNode("element", "FeedbackDevice4", "");
                xmlNode.InnerText = motionDrive.ExtendedProperties.Public["FeedbackDevice4"];
                extendedProperties.AppendChild(xmlNode);
            }

            //ConfigID
            xmlNode = xmlDocument.CreateNode("element", "ConfigID", "");
            xmlNode.InnerText = motionDrive.ExtendedProperties.Public["ConfigID"];
            extendedProperties.AppendChild(xmlNode);

            return extendedProperties;
        }

        private static XmlElement ToExtendedProperties(DiscreteIO discreteIO)
        {
            XmlDocument xmlDocument = new XmlDocument();

            XmlElement extendedProperties = xmlDocument.CreateElement("public");

            XmlNode xmlNode = xmlDocument.CreateNode("element", "ConfigID", "");
            xmlNode.InnerText = discreteIO.ConfigID.ToString();
            extendedProperties.AppendChild(xmlNode);

            xmlNode = xmlDocument.CreateNode("element", "CatNum", "");
            xmlNode.InnerText = discreteIO.CatalogNumber.RemoveSeries();
            extendedProperties.AppendChild(xmlNode);

            return extendedProperties;
        }

        private static XmlElement ToExtendedProperties(AnalogIO analogIO)
        {
            XmlDocument xmlDocument = new XmlDocument();

            XmlElement extendedProperties = xmlDocument.CreateElement("public");

            XmlNode xmlNode = xmlDocument.CreateNode("element", "ConfigID", "");
            xmlNode.InnerText = analogIO.ConfigID.ToString();
            extendedProperties.AppendChild(xmlNode);

            xmlNode = xmlDocument.CreateNode("element", "CatNum", "");
            xmlNode.InnerText = analogIO.CatalogNumber.RemoveSeries();
            extendedProperties.AppendChild(xmlNode);

            return extendedProperties;
        }

        private static CommunicationsType ToCommunicationsType(CommunicationsAdapter adapter)
        {
            CommunicationsType communicationsType = new CommunicationsType();

            if (adapter.Communications.CommMethod.HasValue)
            {
                communicationsType.CommMethod = adapter.Communications.CommMethod.Value;
                communicationsType.CommMethodSpecified = true;
            }

            ConnectionCollection connections = GetConnectionCollection(adapter);

            communicationsType.Items = new object[] { connections };

            return communicationsType;
        }

        private static CommunicationsType ToCommunicationsType(DiscreteIO discreteIO)
        {
            CommunicationsType communicationsType = new CommunicationsType();

            if (discreteIO.Communications.CommMethod.HasValue)
            {
                communicationsType.CommMethod = discreteIO.Communications.CommMethod.Value;
                communicationsType.CommMethodSpecified = true;
            }

            // ConfigData
            ConfigTagType configTagType = GetConfigTagType(discreteIO);

            communicationsType.Items = new object[] { configTagType };

            return communicationsType;
        }

        private static CommunicationsType ToCommunicationsType(AnalogIO analogIO)
        {
            CommunicationsType communicationsType = new CommunicationsType();

            if (analogIO.Communications.CommMethod.HasValue)
            {
                communicationsType.CommMethod = analogIO.Communications.CommMethod.Value;
                communicationsType.CommMethodSpecified = true;
            }

            // ConfigData
            ConfigTagType configTagType = GetConfigTagType(analogIO);

            communicationsType.Items = new object[] { configTagType };

            return communicationsType;
        }

        private static CommunicationsType ToCommunicationsType(GeneralEthernet generalEthernet)
        {
            CommunicationsType communicationsType = new CommunicationsType();


            communicationsType.CommMethod = generalEthernet.Communications.CommMethod;
            communicationsType.CommMethodSpecified = true;


            communicationsType.PrimCxnInputSize = (ushort)generalEthernet.Communications.PrimCxnInputSize;
            communicationsType.PrimCxnInputSizeSpecified = true;

            communicationsType.PrimCxnOutputSize = (ushort)generalEthernet.Communications.PrimCxnOutputSize;
            communicationsType.PrimCxnOutputSizeSpecified = true;

            // ConfigData
            ConfigTagType configData = GetConfigTagType(generalEthernet);

            // Connections
            ConnectionCollection connections = GetConnectionCollection(generalEthernet);

            communicationsType.Items = new object[] { configData, connections };

            return communicationsType;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static ConfigTagType GetConfigTagType(DiscreteIO discreteIO)
        {
            ConfigTagType configTagType = new ConfigTagType();

            configTagType.ConfigSize = (ulong)discreteIO.Communications.ConfigTag.ConfigSize;
            configTagType.ConfigSizeSpecified = true;

            configTagType.ExternalAccess = ToExternalAccess(discreteIO.ConfigTag.ExternalAccess);
            configTagType.ExternalAccessSpecified = true;

            List<object> items = new List<object>();

            Data l5kData = ToL5KData(discreteIO.ConfigTag);
            if (l5kData != null)
                items.Add(l5kData);

            Data decoratedData = ToDecoratedData(discreteIO.ConfigTag);
            if (decoratedData != null)
                items.Add(decoratedData);

            if (items.Count > 0)
                configTagType.Items = items.ToArray();

            return configTagType;
        }

        private static ConfigTagType GetConfigTagType(AnalogIO analogIO)
        {
            ConfigTagType configTagType = new ConfigTagType();

            configTagType.ConfigSize = (ulong)analogIO.Communications.ConfigTag.ConfigSize;
            configTagType.ConfigSizeSpecified = true;

            configTagType.ExternalAccess = ToExternalAccess(analogIO.ConfigTag.ExternalAccess);
            configTagType.ExternalAccessSpecified = true;

            List<object> items = new List<object>();

            // ReSharper disable once InconsistentNaming
            Data l5kData = ToL5KData(analogIO.ConfigTag);
            if (l5kData != null)
                items.Add(l5kData);

            Data decoratedData = ToDecoratedData(analogIO.ConfigTag);
            if (decoratedData != null)
                items.Add(decoratedData);

            if (items.Count > 0)
                configTagType.Items = items.ToArray();

            return configTagType;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static ConfigTagType GetConfigTagType(GeneralEthernet generalEthernet)
        {
            ConfigTagType configTagType = new ConfigTagType();

            configTagType.ConfigSize = (ulong)generalEthernet.Communications.ConfigTag.ConfigSize;
            configTagType.ConfigSizeSpecified = true;

            configTagType.ExternalAccess = ToExternalAccess(generalEthernet.ConfigTag.ExternalAccess);
            configTagType.ExternalAccessSpecified = true;

            Data l5kData = ToL5KData(generalEthernet.ConfigTag);
            Data decoratedData = ToDecoratedData(generalEthernet.ConfigTag);

            configTagType.Items = new object[] { l5kData, decoratedData };

            return configTagType;
        }

        private static CommunicationsType ToCommunicationsType(CIPMotionDrive motionDrive)
        {
            CommunicationsType communicationsType = new CommunicationsType();

            // ConfigData
            ConfigDataType configData = GetConfigDataType(motionDrive);

            // Connections
            ConnectionCollection connections = GetConnectionCollection(motionDrive);

            communicationsType.Items = new object[] { configData, connections };

            return communicationsType;
        }

        private static ConfigDataType GetConfigDataType(CIPMotionDrive motionDrive)
        {
            ConfigDataType configDataType = new ConfigDataType();

            configDataType.ConfigSize = 376;
            configDataType.ConfigSizeSpecified = true;

            Data configL5KData = GetConfigL5KData(motionDrive);

            configDataType.Data = new[] { configL5KData };

            return configDataType;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static Data GetConfigL5KData(CIPMotionDrive motionDrive)
        {
            Data l5kData = new Data
            {
                Format = "L5K"
            };

            var configDefinition =
                motionDrive.Profiles.ModuleTypes.GetConnectionConfigDefinitionByID(motionDrive.ConfigID);

            var dataValue = motionDrive.Profiles.ModuleTypes.GetDataValueByID(configDefinition.ConfigTag.ValueID)
                .ToArray();

            var defaultValue = FromDatValue(dataValue);

            int configControl = defaultValue.ConfigControl[0];
            if (motionDrive.CatalogNumber.StartsWith("ICM-D1", StringComparison.OrdinalIgnoreCase))
            {
                configControl = 166331495;
            }
            else if (motionDrive.CatalogNumber.StartsWith("ICM-D3", StringComparison.OrdinalIgnoreCase))
            {
                configControl = 434767079;
            }
            else if (motionDrive.CatalogNumber.StartsWith("ICM-D5", StringComparison.OrdinalIgnoreCase))
            {
                configControl = 434767079;
            }

            defaultValue.ConfigControl[0] = configControl;

            //motionDrive.ConfigData->defaultValue
            defaultValue.ConfigurationBits = motionDrive.ConfigData.ConfigurationBits;
            defaultValue.BusRegulatorAction = motionDrive.ConfigData.BusRegulatorAction.ToArray();
            defaultValue.DigitalInputConfiguration = motionDrive.ConfigData.DigitalInputConfiguration.ToArray();
            defaultValue.NumberOfConfigurableInputs = motionDrive.ConfigData.NumberOfConfigurableInputs.ToArray();

            defaultValue.NumberOfConfiguredAxes = (byte)motionDrive.Profiles.Schema.Axes.Count;
            defaultValue.FeedbackPortSelect = motionDrive.ConfigData.FeedbackPortSelect.ToArray();

            List<short> selectedCardType = new List<short>();
            foreach (var feedbackPort in motionDrive.ConfigData.FeedbackPortSelect)
            {
                //if (feedbackPort == 0)
                //    selectedCardType.Add(0);
                //else
                //{
                //    var feedbackDevice = motionDrive.Profiles.Schema.Feedback.GetDeviceByPortNumber(feedbackPort);
                //    if (feedbackDevice != null)
                //        selectedCardType.Add((short)feedbackDevice.CardType);
                //    else
                //        selectedCardType.Add(0);

                //}

                selectedCardType.Add(feedbackPort);
            }

            defaultValue.SelectedCardType = selectedCardType.ToArray();

            defaultValue.ConverterACInputVoltage = (short)motionDrive.ConfigData.ConverterACInputVoltage;
            defaultValue.ConverterACInputPhasing = motionDrive.ConfigData.ConverterACInputPhasing;

            defaultValue.BusRegulatorThermalOverloadUserLimit =
                motionDrive.ConfigData.BusRegulatorThermalOverloadUserLimit;
            defaultValue.ConverterThermalOverloadUserLimit = motionDrive.ConfigData.ConverterThermalOverloadUserLimit;

            defaultValue.ExternalShuntRegulatorID = motionDrive.ConfigData.ExternalShuntRegulatorID;
            defaultValue.BusSharingGroup = motionDrive.ConfigData.BusSharingGroup;
            defaultValue.BusConfiguration = motionDrive.ConfigData.BusConfiguration;
            defaultValue.BusUndervoltageUserLimit = motionDrive.ConfigData.BusUndervoltageUserLimit;
            defaultValue.ShuntRegulatorResistorType = motionDrive.ConfigData.ShuntRegulatorResistorType;

            bool isSupportConverterACInputVoltage =
                motionDrive.Profiles.SupportDriveAttribute("ConverterACInputVoltage", motionDrive.Major);
            bool isSupportConverterACInputPhasing =
                motionDrive.Profiles.SupportDriveAttribute("ConverterACInputPhasing", motionDrive.Major);

            MotionDbHelper motionDbHelper = new MotionDbHelper();
            Drive drive = null;

            if (isSupportConverterACInputVoltage && isSupportConverterACInputPhasing)
            {
                drive = motionDbHelper.GetMotionDrive(
                    motionDrive.CatalogNumber,
                    motionDrive.ConfigData.ConverterACInputVoltage,
                    motionDrive.ConfigData.ConverterACInputPhasing);
            }
            else if (isSupportConverterACInputVoltage)
            {
                //TODO(gjc): add code here
            }
            else if (isSupportConverterACInputPhasing)
            {
                //TODO(gjc): add code here
            }
            else
            {
                var drives = motionDbHelper.GetMotionDrive(motionDrive.CatalogNumber);
                if (drives != null && drives.Count == 1)
                {
                    drive = drives[0];
                    defaultValue.DriveClassID = drive.DriveId;
                }
            }

            if (drive != null)
            {
                defaultValue.DriveClassID = drive.DriveId;
            }

            defaultValue.InverterSupport = 1;
            //defaultValue.FWOptionBits
            if (motionDrive.CatalogNumber.StartsWith("ICM-D5", StringComparison.OrdinalIgnoreCase))
            {
                defaultValue.InverterSupport = 5;
            }

            int size = Marshal.SizeOf(defaultValue);
            Contract.Assert(size % 4 == 0);
            defaultValue.CfgSize = size - 4;

            int[] array = new int[size / 4];

            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(defaultValue, ptr, true);

            Marshal.Copy(ptr, array, 0, array.Length);

            Marshal.FreeHGlobal(ptr);

            string value = $"[{string.Join(",", array)}]";

            XmlDocument xmlDocument = new XmlDocument();
            l5kData.Text = new XmlNode[] { xmlDocument.CreateCDataSection(value) };

            return l5kData;
        }

        public static AB_CIP_Drive_C_2 FromDatValue(byte[] dataValue)
        {
            AB_CIP_Drive_C_2 temp = new AB_CIP_Drive_C_2();
            int size = Marshal.SizeOf(temp);
            Contract.Assert(size == dataValue.Length);

            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(dataValue, 0, ptr, dataValue.Length);

            temp = Marshal.PtrToStructure<AB_CIP_Drive_C_2>(ptr);
            Marshal.FreeHGlobal(ptr);

            return temp;
        }

        private static ConnectionCollection GetConnectionCollection(GeneralEthernet generalEthernet)
        {
            ConnectionCollection connections = new ConnectionCollection();

            ConnectionType connectionType = GetConnectionType(generalEthernet);

            connections.Connection = new[] { connectionType };

            return connections;
        }

        private static ConnectionCollection GetConnectionCollection(CommunicationsAdapter adapter)
        {
            ConnectionCollection connections = new ConnectionCollection();

            ConnectionType connectionType = GetConnectionType(adapter);

            connections.Connection = new[] { connectionType };

            return connections;
        }

        private static ConnectionCollection GetConnectionCollection(CIPMotionDrive motionDrive)
        {
            ConnectionCollection connections = new ConnectionCollection();

            //MotionDiagnostics
            ConnectionType motionDiagnostics = GetMotionDiagnosticsConnection(motionDrive);

            //MotionSync
            ConnectionType motionSync = GetMotionSyncConnection(motionDrive);

            connections.Connection = new[] { motionDiagnostics, motionSync };

            return connections;
        }

        private static ConnectionType GetMotionDiagnosticsConnection(CIPMotionDrive motionDrive)
        {
            //TODO(gjc): need edit later
            ConnectionType connectionType = new ConnectionType();

            connectionType.Name = "MotionDiagnostics";
            connectionType.RPI = 1000;
            connectionType.Type = ConnTypeEnum.DiagnosticInput;
            connectionType.EventID = 0;
            connectionType.ProgrammaticallySendEventTrigger = BoolEnum.@false;

            connectionType.InputTag = GetMotionDiagnosticsInputTag(motionDrive);

            return connectionType;
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static InputTagType GetMotionDiagnosticsInputTag(CIPMotionDrive motionDrive)
        {
            //TODO(gjc): need edit later
            InputTagType inputTagType = new InputTagType();

            inputTagType.ExternalAccess = ExternalAccessEnum.ReadWrite;
            inputTagType.ExternalAccessSpecified = true;

            Data decoratedData = new Data();
            decoratedData.Format = "Decorated";

            DataStructure structure = new DataStructure() { DataType = "AB:Motion_Diagnostics:S:1" };

            List<object> items = new List<object>();

            DataValue dataValue = new DataValue()
            {
                Name = "LostControllerToDriveTransmissions",
                DataType = "INT",
                Radix = RadixEnum.Decimal,
                RadixSpecified = true, Value = "0"
            };
            items.Add(dataValue);

            dataValue = new DataValue()
            {
                Name = "LateControllerToDriveTransmissions",
                DataType = "INT",
                Radix = RadixEnum.Decimal,
                RadixSpecified = true,
                Value = "0"
            };
            items.Add(dataValue);

            dataValue = new DataValue()
            {
                Name = "LostDriveToControllerTransmissions",
                DataType = "INT",
                Radix = RadixEnum.Decimal,
                RadixSpecified = true,
                Value = "0"
            };
            items.Add(dataValue);

            dataValue = new DataValue()
            {
                Name = "LateDriveToControllerTransmissions",
                DataType = "INT",
                Radix = RadixEnum.Decimal,
                RadixSpecified = true,
                Value = "0"
            };
            items.Add(dataValue);

            dataValue = new DataValue()
            {
                Name = "LastControllerToDriveTime",
                DataType = "INT",
                Radix = RadixEnum.Decimal,
                RadixSpecified = true,
                Value = "0"
            };
            items.Add(dataValue);

            dataValue = new DataValue()
            {
                Name = "AverageControllerToDriveTime",
                DataType = "INT",
                Radix = RadixEnum.Decimal,
                RadixSpecified = true,
                Value = "0"
            };
            items.Add(dataValue);

            dataValue = new DataValue()
            {
                Name = "MaximumControllerToDriveTime",
                DataType = "INT",
                Radix = RadixEnum.Decimal,
                RadixSpecified = true,
                Value = "0"
            };
            items.Add(dataValue);

            dataValue = new DataValue()
            {
                Name = "LastDriveToControllerTime",
                DataType = "INT",
                Radix = RadixEnum.Decimal,
                RadixSpecified = true,
                Value = "0"
            };
            items.Add(dataValue);

            dataValue = new DataValue()
            {
                Name = "AverageDriveToControllerTime",
                DataType = "INT",
                Radix = RadixEnum.Decimal,
                RadixSpecified = true,
                Value = "0"
            };
            items.Add(dataValue);

            dataValue = new DataValue()
            {
                Name = "MaximumDriveToControllerTime",
                DataType = "INT",
                Radix = RadixEnum.Decimal,
                RadixSpecified = true,
                Value = "0"
            };
            items.Add(dataValue);

            dataValue = new DataValue()
            {
                Name = "LastSystemClockJitter",
                DataType = "DINT",
                Radix = RadixEnum.Decimal,
                RadixSpecified = true,
                Value = "0"
            };
            items.Add(dataValue);

            dataValue = new DataValue()
            {
                Name = "AverageSystemClockJitter",
                DataType = "DINT",
                Radix = RadixEnum.Decimal,
                RadixSpecified = true,
                Value = "0"
            };
            items.Add(dataValue);

            dataValue = new DataValue()
            {
                Name = "MaximumSystemClockJitter",
                DataType = "DINT",
                Radix = RadixEnum.Decimal,
                RadixSpecified = true,
                Value = "0"
            };
            items.Add(dataValue);

            dataValue = new DataValue()
            {
                Name = "TimingStatisticsEnabled",
                DataType = "SINT",
                Radix = RadixEnum.Decimal,
                RadixSpecified = true,
                Value = "0"
            };
            items.Add(dataValue);

            dataValue = new DataValue()
            {
                Name = "ControllerToDriveConnectionSize",
                DataType = "INT",
                Radix = RadixEnum.Decimal,
                RadixSpecified = true,
                Value = "0"
            };
            items.Add(dataValue);

            dataValue = new DataValue()
            {
                Name = "DriveToControllerConnectionSize",
                DataType = "INT",
                Radix = RadixEnum.Decimal,
                RadixSpecified = true,
                Value = "0"
            };
            items.Add(dataValue);

            dataValue = new DataValue()
            {
                Name = "NominalControllerToDriveTime",
                DataType = "INT",
                Radix = RadixEnum.Decimal,
                RadixSpecified = true,
                Value = "0"
            };
            items.Add(dataValue);

            dataValue = new DataValue()
            {
                Name = "NominalDriveToControllerTime",
                DataType = "INT",
                Radix = RadixEnum.Decimal,
                RadixSpecified = true,
                Value = "0"
            };
            items.Add(dataValue);

            dataValue = new DataValue()
            {
                Name = "CoarseUpdatePeriod",
                DataType = "INT",
                Radix = RadixEnum.Decimal,
                RadixSpecified = true,
                Value = "0"
            };
            items.Add(dataValue);

            structure.Items = items.ToArray();
            decoratedData.Items = new[] { (object)structure };
            inputTagType.Items = new[] { (object)decoratedData };

            return inputTagType;
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static ConnectionType GetMotionSyncConnection(CIPMotionDrive motionDrive)
        {
            //TODO(gjc): check later
            ConnectionType connectionType = new ConnectionType
            {
                Name = "MotionSync",
                RPI = 2000,
                Type = ConnTypeEnum.MotionSync,
                EventID = 0,
                ProgrammaticallySendEventTrigger = BoolEnum.@false
            };

            return connectionType;
        }

        private static ConnectionType GetConnectionType(CommunicationsAdapter adapter)
        {
            ConnectionType connectionType = new ConnectionType();

            var connection = adapter.Communications.Connections[0];

            connectionType.Name = connection.Name;
            connectionType.RPI = connection.RPI;
            connectionType.Type = ToConnTypeEnum(connection.Type);
            connectionType.EventID = (ulong)connection.EventID;
            connectionType.ProgrammaticallySendEventTrigger =
                connection.ProgrammaticallySendEventTrigger ? BoolEnum.@true : BoolEnum.@false;
            if (connection.Unicast.HasValue)
            {
                connectionType.Unicast = connection.Unicast.Value ? BoolEnum.@true : BoolEnum.@false;
                connectionType.UnicastSpecified = true;
            }

            //InputTag
            connectionType.InputTag = GetInputTag(adapter.InputTag);

            //OutputTag
            connectionType.OutputTag = GetOutputTag(adapter.OutputTag);

            return connectionType;
        }

        private static ConnectionType GetConnectionType(GeneralEthernet generalEthernet)
        {
            ConnectionType connectionType = new ConnectionType();

            var connection = generalEthernet.Communications.Connections[0];

            connectionType.Name = connection.Name;
            connectionType.RPI = connection.RPI;
            connectionType.Type = ToConnTypeEnum(connection.Type);

            connectionType.InputCxnPoint = (ushort)connection.InputCxnPoint;
            connectionType.InputCxnPointSpecified = true;

            connectionType.OutputCxnPoint = (ushort)connection.OutputCxnPoint;
            connectionType.OutputCxnPointSpecified = true;

            connectionType.OutputSize = (ushort)connection.OutputSize;
            connectionType.OutputSizeSpecified = true;

            connectionType.InputSize = (ushort)connection.InputSize;
            connectionType.InputSizeSpecified = true;

            connectionType.EventID = (ulong)connection.EventID;
            connectionType.ProgrammaticallySendEventTrigger =
                connection.ProgrammaticallySendEventTrigger ? BoolEnum.@true : BoolEnum.@false;
            if (connection.Unicast.HasValue)
            {
                connectionType.Unicast = connection.Unicast.Value ? BoolEnum.@true : BoolEnum.@false;
                connectionType.UnicastSpecified = true;
            }

            //InputTag
            connectionType.InputTag = GetInputTag(generalEthernet.InputTag);

            //OutputTag
            connectionType.OutputTag = GetOutputTag(generalEthernet.OutputTag);

            return connectionType;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static InputTagType GetInputTag(Tag inputTag)
        {
            if (inputTag == null)
                return null;

            InputTagType inputTagType = new InputTagType();

            if (inputTag.ExternalAccess == ExternalAccess.ReadWrite
                || inputTag.ExternalAccess == ExternalAccess.ReadOnly ||
                inputTag.ExternalAccess == ExternalAccess.None)
            {
                inputTagType.ExternalAccess = ToExternalAccess(inputTag.ExternalAccess);
                inputTagType.ExternalAccessSpecified = true;
            }

            List<object> items = new List<object>();

            Data l5kData = ToL5KData(inputTag);
            if (l5kData != null)
                items.Add(l5kData);

            Data decoratedData = ToDecoratedData(inputTag);
            if (decoratedData != null)
                items.Add(decoratedData);

            if (items.Count > 0)
                inputTagType.Items = items.ToArray();

            return inputTagType;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static OutputTagType GetOutputTag(Tag outputTag)
        {
            if (outputTag == null)
                return null;

            OutputTagType outputTagType = new OutputTagType();

            if (outputTag.ExternalAccess == ExternalAccess.ReadWrite
                || outputTag.ExternalAccess == ExternalAccess.ReadOnly ||
                outputTag.ExternalAccess == ExternalAccess.None)
            {
                outputTagType.ExternalAccess = ToExternalAccess(outputTag.ExternalAccess);
                outputTagType.ExternalAccessSpecified = true;
            }

            List<object> items = new List<object>();

            Data l5kData = ToL5KData(outputTag);
            if (l5kData != null)
                items.Add(l5kData);

            Data decoratedData = ToDecoratedData(outputTag);
            if (decoratedData != null)
                items.Add(decoratedData);

            if (items.Count > 0)
                outputTagType.Items = items.ToArray();

            return outputTagType;
        }

        private static ConnTypeEnum ToConnTypeEnum(string connectionType)
        {
            switch (connectionType)
            {
                case "Output":
                    return ConnTypeEnum.Output;
                case "Input":
                    return ConnTypeEnum.Input;

                default:
                    throw new NotImplementedException($"add code for ToConnTypeEnum:{connectionType}");
            }
        }

        private static EKeyingStateEnum ToEKeyingStateEnum(ElectronicKeyingType eKey)
        {
            switch (eKey)
            {
                case ElectronicKeyingType.ExactMatch:
                    return EKeyingStateEnum.ExactMatch;
                case ElectronicKeyingType.CompatibleModule:
                    return EKeyingStateEnum.CompatibleModule;
                case ElectronicKeyingType.Disabled:
                    return EKeyingStateEnum.Disabled;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eKey), eKey, null);
            }
        }
    }
}
