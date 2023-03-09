using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ICSStudio.Cip.Objects;
using ICSStudio.Database.Database;
using ICSStudio.Database.Table.Motion;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DeviceModule;
using Newtonsoft.Json.Linq;
using ICSStudio.DeviceProfiles.MotionDrive2;
using ICSStudio.DeviceProfiles.MotionDrive2.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.Utils;

namespace ICSStudio.SimpleServices.DeviceModule
{
    public class CIPMotionDrive : DeviceModule
    {
        private ITag[] _associatedAxes;

        private CIPMotionDriveConfigData _configData;

        public CIPMotionDrive(IController controller, MotionDriveProfiles profiles) : base(controller)
        {
            Type = DeviceType.CIPMotionDrive;
            Profiles = profiles;

            LoadDefaultValue();
        }

        public MotionDriveProfiles Profiles { get; }

        public CIPMotionDriveConfigData ConfigData
        {
            get { return _configData; }
            set
            {
                _configData = value;

                ValidateConfigData(_configData);
            }
        }

        public uint ConfigID { get; set; }

        //
        public int PowerStructureID { get; set; }

        public string PowerStructure => GetPowerStructureCatalogNumberByID(PowerStructureID);

        public bool VerifyPowerRating
        {
            get { return (ConfigData.ConfigurationBits & 1) > 0; }
            set
            {
                if (value)
                {
                    var bitHost = ConfigData.ConfigurationBits;
                    bitHost |= 0x1;
                    ConfigData.ConfigurationBits = bitHost;
                }
                else
                {
                    var bitHost = ConfigData.ConfigurationBits;
                    bitHost &= (0xFE);
                    ConfigData.ConfigurationBits = bitHost;
                }
            }
        }
        //

        public override JObject ConvertToJObject()
        {
            var module = base.ConvertToJObject();

            //TODO(gjc):need edit here
            
            var data = JToken.FromObject(ConfigData);

            //special for D1S15，D1S30，D1S70
            if (string.Equals(CatalogNumber, "ICM-D1S15", StringComparison.OrdinalIgnoreCase)
                || string.Equals(CatalogNumber, "ICM-D1S30", StringComparison.OrdinalIgnoreCase)
                || string.Equals(CatalogNumber, "ICM-D1S70", StringComparison.OrdinalIgnoreCase))
            {
                data["ConverterACInputVoltage"] = 460;
            }

            module.Add("ConfigData", data);

            //TODO(gjc):need edit later
            if (ExtendedProperties != null)
                module.Add("ExtendedProperties", JToken.FromObject(ExtendedProperties));

            return module;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int AddAxis(ITag axis, int axisNumber)
        {
            if (_associatedAxes == null || axis == null)
                return -1;

            if (axisNumber <= 0 || axisNumber > _associatedAxes.Length)
                return -1;

            if (!axis.DataTypeInfo.DataType.Name.Equals("AXIS_CIP_DRIVE"))
                return -1;

            if (_associatedAxes[axisNumber - 1] != null)
                return -2;

            _associatedAxes[axisNumber - 1] = axis;

            return 0;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int RemoveAxis(ITag axis, int axisNumber)
        {
            if (_associatedAxes == null || axis == null)
                return -1;

            if (axisNumber <= 0 || axisNumber > _associatedAxes.Length)
                return -1;

            if (!axis.DataTypeInfo.DataType.Name.Equals("AXIS_CIP_DRIVE"))
                return -1;

            if (_associatedAxes[axisNumber - 1] != axis)
                return -2;

            _associatedAxes[axisNumber - 1] = null;

            return 0;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ITag GetAxis(int axisNumber)
        {
            if (_associatedAxes == null)
                return null;

            if (axisNumber <= 0 || axisNumber > _associatedAxes.Length)
                return null;


            return _associatedAxes[axisNumber - 1];
        }

        public ITag[] GetAssociatedAxes()
        {
            return _associatedAxes.ToArray();
        }

        public bool ContainAxis(ITag axis)
        {
            return _associatedAxes?.Contains(axis) ?? false;
        }

        public IList GetSupportAxisConfigurationList(int axisNumber)
        {
            var axis = Profiles?.Schema.Axes[axisNumber - 1];
            if (axis?.SupportedAxisConfigurations != null)
            {
                var supportList = new List<AxisConfigurationType>();

                foreach (var axisConfigurationsValue in axis.SupportedAxisConfigurations.Values)
                {
                    var axisConfiguration
                        = EnumUtils.Parse<AxisConfigurationType>(axisConfigurationsValue);

                    if (!supportList.Contains(axisConfiguration))
                        supportList.Add(axisConfiguration);
                }

                return supportList;
            }

            return null;
        }

        public List<T> GetEnumList<T>(string enumName, AxisConfigurationType axisConfiguration) where T : struct
        {
            if (Profiles == null)
                return null;

            var enumList = new List<T>();

            // SupportedEnumsPerAxisConfiguration
            foreach (var supportedEnum in Profiles.Schema.Attributes.SupportedEnumsPerAxisConfiguration)
                if (string.Equals(enumName, supportedEnum.Name))
                {
                    List<SupportedValue<string>> supportedValues = null;
                    switch (axisConfiguration)
                    {
                        case AxisConfigurationType.FeedbackOnly:
                            supportedValues = supportedEnum.FeedbackOnly;
                            break;
                        case AxisConfigurationType.FrequencyControl:
                            supportedValues = supportedEnum.FrequencyControl;
                            break;
                        case AxisConfigurationType.PositionLoop:
                            supportedValues = supportedEnum.PositionLoop;
                            break;
                        case AxisConfigurationType.VelocityLoop:
                            supportedValues = supportedEnum.VelocityLoop;
                            break;
                        case AxisConfigurationType.TorqueLoop:
                            supportedValues = supportedEnum.TorqueLoop;
                            break;
                    }

                    if (supportedValues != null)
                        foreach (var supportedValue in supportedValues)
                            if (Major >= supportedValue.MinMajorRev)
                            {
                                var resultValue = EnumUtils.Parse<T>(supportedValue.Value);

                                if (!enumList.Contains(resultValue))
                                    enumList.Add(resultValue);
                            }

                    return enumList;
                }


            // SupportedEnums
            foreach (var supportedEnum in Profiles.Schema.Attributes.SupportedEnums)
                if ((Major >= supportedEnum.MinMajorRev)
                    && string.Equals(supportedEnum.Name, enumName))
                {
                    foreach (var enumValue in supportedEnum.Values)
                    {
                        var resultValue = EnumUtils.Parse<T>(enumValue.Value);

                        if (!enumList.Contains(resultValue))
                            enumList.Add(resultValue);
                    }

                    return enumList;
                }

            return enumList;
        }

        public List<T> GetDriveSupportedEnumList<T>(string enumName) where T : struct
        {
            if (Profiles == null)
                return null;

            var enumList = new List<T>();
            foreach (var supportedEnum in Profiles.Schema.Attributes.SupportedEnums)
                if ((Major >= supportedEnum.MinMajorRev)
                    && string.Equals(supportedEnum.Name, enumName))
                {
                    foreach (var enumValue in supportedEnum.Values)
                    {
                        var resultValue = EnumUtils.Parse<T>(enumValue.Value);
                        if (!enumList.Contains(resultValue))
                            enumList.Add(resultValue);
                    }

                    return enumList;
                }

            return enumList;
        }

        public List<int> CandidateAxisNumbers(ITag axisTag)
        {
            if (_associatedAxes == null)
                return null;

            List<int> candidateAxisNumbers = new List<int>();
            for (int i = 0; i < _associatedAxes.Length; i++)
            {
                if (_associatedAxes[i] == axisTag || _associatedAxes[i] == null)
                    candidateAxisNumbers.Add(i + 1);
            }

            return candidateAxisNumbers;
        }

        public string GetPowerStructureCatalogNumberByID(int id, int lcid = 1033)
        {
            if (Profiles != null)
            {
                foreach (var powerStructure in Profiles.Schema.PowerStructures)
                    if (powerStructure.ID == id)
                        return powerStructure.GetCatalogNumber(lcid);
            }

            return string.Empty;
        }

        public List<FeedbackType> GetSupportFeedback1Types(int axisNumber)
        {
            int motorFeedbackPort = ConfigData.FeedbackPortSelect[(axisNumber - 1) * 4];

            if (Profiles?.Schema.Feedback == null
                || motorFeedbackPort == 0)
                return null;

            return GetSupportFeedbackTypesByPortNumber(motorFeedbackPort);
        }

        public List<FeedbackType> GetSupportFeedback2Types(int axisNumber)
        {
            int loadFeedbackPort = ConfigData.FeedbackPortSelect[(axisNumber - 1) * 4 + 1];
            if (Profiles?.Schema.Feedback == null
                || loadFeedbackPort == 0)
                return null;


            return GetSupportFeedbackTypesByPortNumber(loadFeedbackPort);
        }

        private List<FeedbackType> GetSupportFeedbackTypesByPortNumber(int number)
        {
            var feedbackDevice = Profiles.Schema.Feedback.GetDeviceByPortNumber(number);
            if (feedbackDevice == null)
                return null;

            var supportList = new List<FeedbackType>();

            foreach (var feedbackType in feedbackDevice.FeedbackTypes)
                if (Major >= feedbackType.MinMajorRev)
                {
                    //TODO(gjc): EnDat 2.1 and EnDat 2.2 ignore???
                    if (feedbackType.Value == "EnDat 2.1" ||
                        feedbackType.Value == "EnDat 2.2")
                        continue;

                    var feedbackValue = EnumUtils.Parse<FeedbackType>(feedbackType.Value);

                    if (!supportList.Contains(feedbackValue))
                        supportList.Add(feedbackValue);
                }

            return supportList;
        }

        public List<HomeSequenceType> GetSupportHomeSequenceTypes(int axisNumber)
        {
            int motorFeedbackPort = ConfigData.FeedbackPortSelect[(axisNumber - 1) * 4];

            if (Profiles?.Schema.Feedback == null
                || motorFeedbackPort == 0)
                return null;

            return GetSupportHomeSequenceTypesByPortNumber(motorFeedbackPort);
        }

        private List<HomeSequenceType> GetSupportHomeSequenceTypesByPortNumber(int number)
        {
            var feedbackDevice = Profiles.Schema.Feedback.GetDeviceByPortNumber(number);
            if (feedbackDevice == null)
                return null;

            var supportList = new List<HomeSequenceType>();
            foreach (var homeSequence in feedbackDevice.HomeSequence)
            {
                var homeSequenceValue = EnumUtils.Parse<HomeSequenceType>(homeSequence);

                if (!supportList.Contains(homeSequenceValue))
                    supportList.Add(homeSequenceValue);
            }

            return supportList;
        }

        public List<MotorType> GetSupportMotorType(AxisConfigurationType axisConfiguration)
        {
            var configurations = Profiles?.Schema?.Attributes?.SupportedEnumsPerAxisConfigurationAndFeedbackConfiguration;
            if (configurations != null)
            {
                SupportedEnumPerAxisConfigurationAndFeedbackConfiguration motorTypeConfiguration = null;
                foreach (var configuration in configurations)
                {
                    if (configuration.Name == "MotorType")
                    {
                        motorTypeConfiguration = configuration;
                        break;
                    }
                }

                if (motorTypeConfiguration != null)
                {
                    List<SupportedValue<string>> motorTypes = null;
                    switch (axisConfiguration)
                    {
                        case AxisConfigurationType.FeedbackOnly:
                            break;
                        case AxisConfigurationType.FrequencyControl:
                            motorTypes = motorTypeConfiguration.FrequencyControl.NoFeedback;
                            break;
                        case AxisConfigurationType.PositionLoop:
                            motorTypes = motorTypeConfiguration.PositionLoop.Feedback;
                            break;
                        case AxisConfigurationType.VelocityLoop:
                            motorTypes = motorTypeConfiguration.VelocityLoop.Feedback;
                            break;
                        case AxisConfigurationType.TorqueLoop:
                            motorTypes = motorTypeConfiguration.TorqueLoop.Feedback;
                            break;
                        case AxisConfigurationType.ConverterOnly:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(axisConfiguration), axisConfiguration, null);
                    }

                    if (motorTypes != null)
                    {
                        List<MotorType> supportMotorTypes = new List<MotorType>();

                        foreach (var supportedValue in motorTypes)
                        {
                            if (Major >= supportedValue.MinMajorRev)
                            {
                                supportMotorTypes.Add(EnumUtils.Parse<MotorType>(supportedValue.Value));
                            }
                        }

                        return supportMotorTypes;
                    }
                }
            }

            return null;
        }

        public bool SupportAxisAttribute(AxisConfigurationType axisConfiguration, string attribute)
        {
            if (Profiles == null)
                return false;

            return Profiles.SupportAxisAttribute(axisConfiguration, attribute, Major);
        }

        public List<ITag> CheckAxisUpdatePeriod()
        {
            int baseCount = 0;
            int alternate1Count = 0;
            int alternate2Count = 0;

            List<ITag> axisList = new List<ITag>();

            foreach (var tag in _associatedAxes.OfType<Tag>())
            {
                AxisCIPDrive axisCIPDrive = tag.DataWrapper as AxisCIPDrive;
                if (axisCIPDrive?.AssignedGroup != null)
                {
                    axisList.Add(tag);

                    var updateSchedule =
                        (AxisUpdateScheduleType)Convert.ToByte(axisCIPDrive.CIPAxis.AxisUpdateSchedule);
                    switch (updateSchedule)
                    {
                        case AxisUpdateScheduleType.Base:
                            baseCount++;
                            break;
                        case AxisUpdateScheduleType.Alternate1:
                            alternate1Count++;
                            break;
                        case AxisUpdateScheduleType.Alternate2:
                            alternate2Count++;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            if (baseCount == 0 && alternate1Count == 0 && alternate2Count == 0)
                return null;

            if (baseCount != 0 && alternate1Count == 0 && alternate2Count == 0)
                return null;

            if (baseCount == 0 && alternate1Count != 0 && alternate2Count == 0)
                return null;

            if (baseCount == 0 && alternate1Count == 0 && alternate2Count != 0)
                return null;

            return axisList;
        }

        private void LoadDefaultValue()
        {
            if (Profiles != null)
            {
                CatalogNumber = Profiles.Identity.CatalogNumber;
                ProductCode = Profiles.Identity.ProductCode;
                ProductType = Profiles.Identity.ProductType;
                Vendor = Profiles.Identity.VendorID;
                EKey = ElectronicKeyingType.CompatibleModule;
                Connection = ConnectionType.Motion;

                _associatedAxes = new ITag[Profiles.Schema.Axes.Count];

                // Ports
                foreach (var profilesPort in Profiles.Ports)
                {
                    Port port = new Port()
                    {
                        Id = profilesPort.Number,
                        Type = EnumUtils.Parse<PortType>(profilesPort.Type),
                        Upstream = true
                    };

                    Ports.Add(port);
                }

                var defaultModuleType = Profiles.GetDefaultModuleType();

                Major = defaultModuleType.MajorRev;
                Minor = 1;

                PowerStructureID = Profiles.Schema.PowerStructures[0].ID;

                var defaultConnection = "MotionSyncAndMotionDiagnostics";
                var connectionConfig =
                    Profiles.GetConnectionConfig(defaultModuleType.ModuleDefinitionID, defaultConnection);

                ConfigID = connectionConfig;

                // default value
                var configDefinition =
                    Profiles.ModuleTypes.GetConnectionConfigDefinitionByID(ConfigID);

                var dataValue = Profiles.ModuleTypes.GetDataValueByID(configDefinition.ConfigTag.ValueID).ToArray();

                var defaultValue = AB_CIP_Drive_C_2.FromDatValue(dataValue);

                // ConfigData
                ConfigData = new CIPMotionDriveConfigData
                {
                    ConfigurationBits = defaultValue.ConfigurationBits,
                    BusRegulatorAction = new List<byte>(defaultValue.BusRegulatorAction),
                    DigitalInputConfiguration = new List<byte>(defaultValue.DigitalInputConfiguration),
                    NumberOfConfigurableInputs = new List<byte>(defaultValue.NumberOfConfigurableInputs),
                    FeedbackPortSelect = new List<byte>(defaultValue.FeedbackPortSelect),
                    ConverterACInputVoltage = (ushort)defaultValue.ConverterACInputVoltage,
                    ConverterACInputPhasing = defaultValue.ConverterACInputPhasing,

                    BusRegulatorThermalOverloadUserLimit = defaultValue.BusRegulatorThermalOverloadUserLimit,
                    ConverterThermalOverloadUserLimit = defaultValue.ConverterThermalOverloadUserLimit,

                    ExternalShuntRegulatorID = defaultValue.ExternalShuntRegulatorID,

                    BusSharingGroup = defaultValue.BusSharingGroup,
                    BusConfiguration = defaultValue.BusConfiguration,
                    BusUndervoltageUserLimit = defaultValue.BusUndervoltageUserLimit,
                    ShuntRegulatorResistorType = defaultValue.ShuntRegulatorResistorType

                };

                DBHelper dbHelper = new DBHelper();
                string vendor = dbHelper.GetVendorName(Vendor);

                // ExtendedProperties
                ExtendedProperties = new ExtendedProperties()
                {
                    Public = new Dictionary<string, string>()
                    {
                        { "Vendor", $"{vendor}" },
                        { "CatNum", $"{CatalogNumber}" },
                        { "ConfigID", $"{ConfigID}" },
                    }
                };

                foreach (var devicePort in Profiles.Schema.Feedback.DevicePorts)
                {
                    ExtendedProperties.Public.Add($"FeedbackDevice{devicePort.Number}",
                        $"{devicePort.FeedbackPortNumber}");
                }
            }

        }

        private void ResetDefaultValue(AB_CIP_Drive_C_2 defaultValue)
        {
            int axesCount = Profiles.Schema.Axes.Count;

            for (int i = axesCount; i < 8; i++)
            {
                defaultValue.FeedbackPortSelect[i * 4] = 0;
                defaultValue.FeedbackPortSelect[i * 4 + 1] = 0;
                defaultValue.FeedbackPortSelect[i * 4 + 2] = 0;
                defaultValue.FeedbackPortSelect[i * 4 + 3] = 0;
            }
        }

        private void ValidateConfigData(CIPMotionDriveConfigData configData)
        {
            MotionDbHelper motionDbHelper = new MotionDbHelper();
            Drive drive = motionDbHelper.GetMotionDrive(
                CatalogNumber,
                configData.ConverterACInputVoltage,
                configData.ConverterACInputPhasing);

            if (drive == null)
            {
                var drives = motionDbHelper.GetMotionDrive(CatalogNumber);
                if (drives != null && drives.Count > 0)
                {
                    drive = drives[0];

                    configData.ConverterACInputVoltage = (ushort)drive.ConverterACInputVoltage;
                    configData.ConverterACInputPhasing = (byte)drive.ConverterACInputPhasing;
                }

            }
        }


    }
}
