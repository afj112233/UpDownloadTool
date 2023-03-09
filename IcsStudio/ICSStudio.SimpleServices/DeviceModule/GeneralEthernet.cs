using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Globalization;
using ICSStudio.DeviceProfiles.Common;
using ICSStudio.DeviceProfiles.Generic;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.Interfaces.Notification;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.DeviceModule
{
    public class GeneralEthernet : DeviceModule
    {
        public GeneralEthernet(IController controller, GenericEnetModuleProfiles profiles) : base(controller)
        {
            Contract.Assert(profiles != null);

            Type = DeviceType.BlockIO;
            Profiles = profiles;

            LoadDefaultValue();

            PropertyChanged += OnPropertyChanged;

        }

        public GenericEnetModuleProfiles Profiles { get; }

        public GeneralEthernetCommunications Communications { get; set; }

        //
        public int ConfigCxnPoint { get; set; }

        private void LoadDefaultValue()
        {
            if (Profiles == null)
                return;

            ProductCode = Profiles.ProductCode;
            ProductType = Profiles.ProductType;
            Vendor = Profiles.VendorID;
            EKey = ElectronicKeyingType.Disabled;

            Major = Profiles.MajorRevs[0];
            Minor = 1;
            CatalogNumber = Profiles.CatalogNumber;

            // Port
            foreach (var profilesPort in Profiles.Ports)
            {
                var port = new Port
                {
                    Id = profilesPort.Number,
                    Type = EnumUtils.Parse<PortType>(profilesPort.Type),
                    Address = string.Empty,
                    Upstream = true
                };

                if (port.Type == PortType.PointIO) port.Address = "0";

                Ports.Add(port);
            }

            // Communications
            Communications = new GeneralEthernetCommunications();

            EnetModuleType moduleType = Profiles.ModuleProperties[0];
            string commMethodID = moduleType.DefaultCommMethod;

            CommMethod defaultCommMethod = Profiles.GetCommMethodByID(commMethodID);
            Contract.Assert(defaultCommMethod != null);

            Communications.CommMethod = uint.Parse(commMethodID, NumberStyles.HexNumber);
            int inputSize = defaultCommMethod.PrimaryConnectionInputSize.Default *
                            defaultCommMethod.PrimaryConnectionInputSize.Multiplier;
            int outputSize = defaultCommMethod.PrimaryConnectionOutputSize.Default *
                             defaultCommMethod.PrimaryConnectionOutputSize.Multiplier;

            Communications.PrimCxnInputSize = inputSize;
            Communications.PrimCxnOutputSize = outputSize;

            // Connections
            GeneralEthernetConnection connection = new GeneralEthernetConnection();
            connection.EventID = 0;
            connection.Name = "Standard";
            connection.ProgrammaticallySendEventTrigger = false;
            connection.RPI = (uint)moduleType.DefaultUpdateRate;
            connection.Type = "Output";
            connection.Unicast = true;
            connection.InputCxnPoint = 0;
            connection.OutputCxnPoint = 0;
            connection.InputSize = inputSize;
            connection.OutputSize = outputSize;

            // InputTag
            var inputTag = new InputTag();
            inputTag.ExternalAccess = ExternalAccess.ReadWrite;
            inputTag.DataType = defaultCommMethod.InputTag.DataType.Replace("*", inputSize.ToString());
            inputTag.Data = null;

            connection.InputTag = inputTag;

            // OutputTag
            var outputTag = new OutputTag();
            outputTag.ExternalAccess = ExternalAccess.ReadWrite;
            outputTag.DataType = defaultCommMethod.OutputTag.DataType.Replace("*", outputSize.ToString());
            outputTag.Data = null;

            connection.OutputTag = outputTag;

            Communications.Connections = new List<GeneralEthernetConnection>() { connection };

            // ConfigTag
            var configOption = defaultCommMethod.GetConfigOptionByID(defaultCommMethod.DefaultConfigOptionsID);
            Contract.Assert(configOption != null);

            var config = moduleType.GetConfigByID(configOption.ConfigID);

            ConfigTag configTag = new ConfigTag();
            configTag.ConfigSize = config.ConfigSize.Default;
            configTag.ExternalAccess = ExternalAccess.ReadWrite;
            configTag.DataType = config.DataType;
            configTag.Data = null;

            Communications.ConfigTag = configTag;

            // rebuild device tag
            RebuildDeviceTag();

            Notifications.Publish(new MessageData() { Type = MessageData.MessageType.Verify });
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                var tagCollection = InputTag?.ParentCollection as TagCollection;
                if (tagCollection != null)
                    tagCollection.IsNeedVerifyRoutine = false;
                if (ConfigTag != null) ConfigTag.Name = $"{Name}:C";

                if (InputTag != null) InputTag.Name = $"{Name}:I";

                if (OutputTag != null) OutputTag.Name = $"{Name}:O";
                if (tagCollection != null)
                {
                    CodeSynchronization.GetInstance().UpdateModuleName(this);
                    CodeSynchronization.GetInstance().Update();
                    tagCollection.IsNeedVerifyRoutine = true;
                }
            }
        }

        public override void PostLoadJson()
        {
            RebuildDeviceTag();

            if (ConfigTag != null)
            {
                if (Communications.ConfigTag.Data != null)
                {
                    ConfigTag.DataWrapper.Data.Update(ConvertBase64Data(Communications.ConfigTag.Data));
                }

                ConfigCxnPoint = int.Parse(ConfigTag.GetMemberValue("CfgIDNum", true));
            }

            if (InputTag != null)
            {
                if (Communications.Connections[0].InputTag.Data != null)
                {
                    InputTag.DataWrapper.Data.Update(ConvertBase64Data(Communications.Connections[0].InputTag.Data));
                }
            }

            if (OutputTag != null)
            {
                if (Communications.Connections[0].OutputTag.Data != null)
                {
                    OutputTag.DataWrapper.Data.Update(ConvertBase64Data(Communications.Connections[0].OutputTag.Data));
                }
            }

            Notifications.Publish(new MessageData() { Type = MessageData.MessageType.Verify });
        }

        public override JObject ConvertToJObject()
        {
            var module = base.ConvertToJObject();

            if (Communications != null)
            {
                if (Communications.Connections != null && Communications.Connections.Count > 0)
                {

                    var connection = Communications.Connections[0];

                    if (InputTag != null && connection.InputTag != null)
                    {
                        connection.InputTag.DataType = InputTag.DataTypeInfo.DataType.Name;
                        var data = new List<byte>();
                        InputTag.DataWrapper.Data.ToMsgPack(data);
                        connection.InputTag.Data = System.Convert.ToBase64String(data.ToArray());
                        ;
                    }

                    if (OutputTag != null && connection.OutputTag != null)
                    {
                        connection.OutputTag.DataType = OutputTag.DataTypeInfo.DataType.Name;
                        var data = new List<byte>();
                        OutputTag.DataWrapper.Data.ToMsgPack(data);
                        connection.OutputTag.Data = System.Convert.ToBase64String(data.ToArray());
                        ;
                    }
                }

                if (ConfigTag != null && Communications.ConfigTag != null)
                {
                    Communications.ConfigTag.DataType = ConfigTag.DataTypeInfo.DataType.Name;
                    var data = new List<byte>();
                    ConfigTag.DataWrapper.Data.ToMsgPack(data);
                    Communications.ConfigTag.Data = System.Convert.ToBase64String(data.ToArray());
                    ;
                }

                module.Add("Communications", JToken.FromObject(Communications));
            }

            return module;
        }

        public override void RebuildDeviceTag()
        {
            var tagCollection = InputTag?.ParentCollection as TagCollection;
            if (tagCollection != null)
                tagCollection.IsNeedVerifyRoutine = false;
            MarkReferenceTagRoutine();
            RebuildConfigTag();
            RebuildInputTag();
            RebuildOutputTag();
            if (tagCollection != null)
            {
                CodeSynchronization.GetInstance().UpdateModuleName(this);
                CodeSynchronization.GetInstance().Update();
                tagCollection.IsNeedVerifyRoutine = true;
            }
        }

        private void RebuildOutputTag()
        {
            DeleteTag(OutputTag);

            OutputTag = null;

            var tagName = $"{Name}:O";

            var dataTypeDefinition = ExportOutputDataType();

            OutputTag = CreateTag(tagName, dataTypeDefinition);
        }

        private void RebuildInputTag()
        {
            DeleteTag(InputTag);

            InputTag = null;

            var tagName = $"{Name}:I";

            var dataTypeDefinition = ExportInputDataType();

            InputTag = CreateTag(tagName, dataTypeDefinition);
        }

        private void RebuildConfigTag()
        {
            DeleteTag(ConfigTag);

            ConfigTag = null;

            var tagName = $"{Name}:C";
            var dataTypeDefinition = ExportConfigDataType();

            ConfigTag = CreateTag(tagName, dataTypeDefinition);

            if (ConfigTag != null)
            {

                int cfgSize = Communications.ConfigTag.ConfigSize + 4;

                ConfigTag?.SetStringValue("CfgSize", cfgSize.ToString());
                ConfigTag?.SetStringValue("CfgIDNum", ConfigCxnPoint.ToString());
            }
        }

        public override void RemoveDeviceTag()
        {
            DeleteTag(ConfigTag);
            DeleteTag(InputTag);
            DeleteTag(OutputTag);
        }

        public JObject ExportConfigDataType()
        {
            var dataTypeDefinition = GetConfigDataTypeDefinition();
            return dataTypeDefinition?.ToJObject();
        }

        public JObject ExportInputDataType()
        {
            var dataTypeDefinition = GetInputDataTypeDefinition();
            return dataTypeDefinition?.ToJObject();
        }

        public JObject ExportOutputDataType()
        {
            var dataTypeDefinition = GetOutputDataTypeDefinition();
            return dataTypeDefinition?.ToJObject();
        }

        private DataTypeDefinition GetOutputDataTypeDefinition()
        {
            CommMethod commMethod = Profiles.GetCommMethodByID(Communications.CommMethod.ToString("X"));

            var moduleType = Profiles.ModuleProperties[0];

            DataTypeDefinition dataTypeDefinition
                = moduleType.GetDataTypeDefinition(commMethod.OutputTag.DataType);

            DataTypeDefinition newDefinition
                = JsonConvert.DeserializeObject<DataTypeDefinition>(JsonConvert.SerializeObject(dataTypeDefinition));

            newDefinition.Name
                = newDefinition.Name.Replace("*", Communications.PrimCxnOutputSize.ToString());

            newDefinition.Members[0].Dimension =
                (Communications.PrimCxnOutputSize / commMethod.PrimaryConnectionOutputSize.Multiplier).ToString();

            return newDefinition;
        }

        private DataTypeDefinition GetInputDataTypeDefinition()
        {
            CommMethod commMethod = Profiles.GetCommMethodByID(Communications.CommMethod.ToString("X"));

            var moduleType = Profiles.ModuleProperties[0];

            DataTypeDefinition dataTypeDefinition
                = moduleType.GetDataTypeDefinition(commMethod.InputTag.DataType);

            DataTypeDefinition newDefinition
                = JsonConvert.DeserializeObject<DataTypeDefinition>(JsonConvert.SerializeObject(dataTypeDefinition));

            newDefinition.Name
                = newDefinition.Name.Replace("*", Communications.PrimCxnInputSize.ToString());

            newDefinition.Members[0].Dimension =
                (Communications.PrimCxnInputSize / commMethod.PrimaryConnectionInputSize.Multiplier).ToString();

            return newDefinition;
        }

        private DataTypeDefinition GetConfigDataTypeDefinition()
        {
            var configTag = Communications.ConfigTag;
            var moduleType = Profiles.ModuleProperties[0];

            DataTypeDefinition dataTypeDefinition
                = moduleType.GetDataTypeDefinition(configTag.DataType);

            return dataTypeDefinition;
        }

        private void DeleteTag(Tag tag)
        {
            if (tag != null && ParentController != null)
            {
                var tagCollection = ParentController.Tags as TagCollection;
                Contract.Assert(tagCollection != null);

                tagCollection.DeleteTag(tag, true, true, false);
            }
        }

        private Tag CreateTag(string tagName, JObject dataTypeDefinition)
        {
            if (dataTypeDefinition != null && ParentController != null && ParentModule != null)
            {
                var dataTypeCollection = ParentController.DataTypes as DataTypeCollection;
                var tagCollection = ParentController.Tags as TagCollection;

                Contract.Assert(dataTypeCollection != null);
                Contract.Assert(tagCollection != null);

                var dataTypeName = dataTypeDefinition["Name"].ToString();

                if (dataTypeCollection[dataTypeName] == null)
                {
                    var dataType = new ModuleDefinedDataType(dataTypeDefinition);
                    dataType.PostInit(ParentController.DataTypes);
                    dataTypeCollection.AddDataType(dataType);
                }

                var tag = TagsFactory.CreateTag(
                    tagCollection,
                    tagName, dataTypeName, 0, 0, 0);

                if (tag != null)
                {
                    tag.IsModuleTag = true;
                    tagCollection.AddTag(tag, false, false);
                }

                return tag;
            }

            return null;
        }

        public void ChangeInputSize(int inputSize)
        {
            Communications.PrimCxnInputSize = inputSize;
            Communications.Connections[0].InputSize = inputSize;

            RebuildInputTag();
        }

        public void ChangeOutputSize(int outputSize)
        {
            Communications.PrimCxnOutputSize = outputSize;
            Communications.Connections[0].OutputSize = outputSize;

            RebuildOutputTag();
        }

        public void ChangeConfigTag(int configCxnPoint, int configSize)
        {
            ConfigCxnPoint = configCxnPoint;

            Communications.ConfigTag.ConfigSize = configSize;

            int cfgSize = configSize + 4;

            if (ConfigTag != null)
            {
                ConfigTag.SetStringValue("CfgSize", cfgSize.ToString());
                ConfigTag.SetStringValue("CfgIDNum", configCxnPoint.ToString());
            }
        }

        public void ChangeCommMethod(
            uint commMethod,
            int inputCxnPoint, int inputSize,
            int outputCxnPoint, int outputSize,
            int configCxnPoint, int configSize)
        {
            Communications.CommMethod = commMethod;

            Communications.PrimCxnInputSize = inputSize;
            Communications.Connections[0].InputSize = inputSize;

            Communications.PrimCxnOutputSize = outputSize;
            Communications.Connections[0].OutputSize = outputSize;

            ConfigCxnPoint = configCxnPoint;
            Communications.ConfigTag.ConfigSize = configSize;

            RebuildDeviceTag();

            Notifications.Publish(new MessageData() { Type = MessageData.MessageType.Verify });
        }
    }

    public class GeneralEthernetCommunications
    {
        public uint CommMethod { get; set; }
        public int PrimCxnInputSize { get; set; }
        public int PrimCxnOutputSize { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<GeneralEthernetConnection> Connections { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ConfigTag ConfigTag { get; set; }
    }

    public class GeneralEthernetConnection
    {
        public string Name { get; set; }
        public uint RPI { get; set; }
        public string Type { get; set; }
        public int EventID { get; set; }
        public bool ProgrammaticallySendEventTrigger { get; set; }
        public bool? Unicast { get; set; }

        public int InputCxnPoint { get; set; }
        public int OutputCxnPoint { get; set; }

        public int InputSize { get; set; }
        public int OutputSize { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public InputTag InputTag { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public OutputTag OutputTag { get; set; }
    }
}
