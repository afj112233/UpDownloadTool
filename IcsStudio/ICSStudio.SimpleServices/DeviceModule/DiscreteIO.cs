using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using ICSStudio.DeviceProfiles.Common;
using ICSStudio.DeviceProfiles.DIOEnetAdapter;
using ICSStudio.DeviceProfiles.DIOModule;
using ICSStudio.DeviceProfiles.DIOModule.Common;
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
    public class DiscreteIO : DeviceModule
    {
        public DiscreteIO(IController controller, DIOModuleProfiles profiles) : base(controller)
        {
            Contract.Assert(profiles != null);

            Type = DeviceType.ChassisIO;
            Profiles = profiles;

            LoadDefaultValue();
        }

        public DIOModuleProfiles Profiles { get; }

        public DiscreteIOCommunications Communications { get; set; }

        public uint ConfigID { get; set; }

        public int Slot
        {
            get
            {
                var pointIO = GetFirstPort(PortType.PointIO);
                return int.Parse(pointIO.Address);
            }
            set
            {
                var pointIO = GetFirstPort(PortType.PointIO);
                if (int.Parse(pointIO.Address) != value)
                {
                    pointIO.Address = value.ToString();
                    RaisePropertyChanged();

                    var tagCollection = InputTag?.ParentCollection as TagCollection;
                    if (tagCollection != null)
                        tagCollection.IsNeedVerifyRoutine = false;

                    UpdateConfigTagName();
                    UpdateInputTagName();
                    UpdateOutputTagName();

                    if (tagCollection != null)
                    {
                        CodeSynchronization.GetInstance().UpdateModuleName(this);
                        CodeSynchronization.GetInstance().Update();
                        tagCollection.IsNeedVerifyRoutine = true;
                    }
                }
            }
        }

        public bool IsRack
            => (ConfigID & (uint)DIOConnectionTypeMask.Rack) > 0;

        public bool IsEnhancedRack
            => (ConfigID & (uint)DIOConnectionTypeMask.EnhancedRack) > 0;

        public override JObject ConvertToJObject()
        {
            var module = base.ConvertToJObject();

            if (Communications != null)
            {
                if (Communications.Connections != null && Communications.Connections.Count > 0)
                {
                    var ioConnection = Communications.Connections[0];

                    if (InputTag != null && ioConnection?.InputTag != null)
                    {
                        ioConnection.InputTag.DataType = InputTag.DataTypeInfo.DataType.Name;
                        var data = new List<byte>();
                        InputTag.DataWrapper.Data.ToMsgPack(data);
                        ioConnection.InputTag.Data = Convert.ToBase64String(data.ToArray());

                        if (InputTag.ChildDescription != null && InputTag.ChildDescription.Count > 0)
                        {
                            ioConnection.InputTag.Comments = InputTag.ChildDescription;
                        }
                        else
                        {
                            ioConnection.InputTag.Comments = null;
                        }
                    }

                    if (OutputTag != null && ioConnection?.OutputTag != null)
                    {
                        ioConnection.OutputTag.DataType = OutputTag.DataTypeInfo.DataType.Name;
                        var data = new List<byte>();
                        OutputTag.DataWrapper.Data.ToMsgPack(data);
                        ioConnection.OutputTag.Data = Convert.ToBase64String(data.ToArray());

                        if (OutputTag.ChildDescription != null && OutputTag.ChildDescription.Count > 0)
                        {
                            ioConnection.OutputTag.Comments = OutputTag.ChildDescription;
                        }
                        else
                        {
                            ioConnection.OutputTag.Comments = null;
                        }
                    }
                }

                if (Communications.ConfigTag != null && ConfigTag != null)
                {
                    var data = new List<byte>();
                    ConfigTag.DataWrapper.Data.ToMsgPack(data);
                    Communications.ConfigTag.Data = Convert.ToBase64String(data.ToArray());

                    Communications.ConfigTag.ConfigSize =
                        CalculateConfigSize(ConfigTag.DataWrapper.DataTypeInfo.DataType);

                    if (ConfigTag.ChildDescription != null && ConfigTag.ChildDescription.Count > 0)
                    {
                        Communications.ConfigTag.Comments = ConfigTag.ChildDescription;
                    }
                    else
                    {
                        Communications.ConfigTag.Comments = null;
                    }
                }

                // handle for Connections null
                if (Communications.Connections == null)
                    Communications.Connections = new List<DiscreteIOConnection>();

                module.Add("Communications", JToken.FromObject(Communications));
            }

            //
            ExtendedProperties = new ExtendedProperties
            {
                Public = new Dictionary<string, string>
                {
                    { "CatNum", $"{Profiles.CatalogNumber}" },
                    { "ConfigID", $"{ConfigID}" }
                }
            };

            module.Add("ExtendedProperties", JToken.FromObject(ExtendedProperties));

            return module;
        }

        private int CalculateConfigSize(IDataType dataType)
        {
            //TODO(gjc): need check here!!!
            ModuleDefinedDataType moduleDefinedDataType = dataType as ModuleDefinedDataType;
            if (moduleDefinedDataType != null)
            {
                return moduleDefinedDataType.ByteSize - 8;
            }

            return 0;
        }

        public void ChangeConnectionConfigID(uint connectionConfigID)
        {
            if (ConfigID == connectionConfigID)
                return;

            uint oldConnectionConfigID = ConfigID;
            uint newConnectionConfigID = connectionConfigID;

            ConfigID = connectionConfigID;

            // TODO(gjc): ib8 lorack CommMethod = 3 ?
            Communications.CommMethod =
                Profiles.GetCommMethodByConfigID(connectionConfigID, Major);

            var connectionConfigDefinition =
                Profiles.DIOModuleTypes.GetConnectionConfigDefinitionByID(connectionConfigID);

            if (connectionConfigDefinition != null)
            {
                if (connectionConfigDefinition.ConfigTag != null
                    && !Profiles
                        .GetConnectionStringByConfigID(ConfigID, Major)
                        .Contains("Listen Only"))
                    Communications.ConfigTag = new ConfigTag
                    {
                        ExternalAccess = ExternalAccess.ReadWrite,
                        DataType = connectionConfigDefinition.ConfigTag.DataType,
                        ConfigSize = 0,
                        Data = null
                    };
                else
                    Communications.ConfigTag = null;

                //Connections
                var connectionDefinition =
                    Profiles.DIOModuleTypes.GetConnectionDefinitionByID(connectionConfigDefinition
                        .Connections[0]);
                if (connectionDefinition.RPI.HasValue)
                {
                    var discreteIOConnection = new DiscreteIOConnection
                    {
                        Name = connectionDefinition.Name,
                        RPI = connectionDefinition.RPI.GetValueOrDefault(),
                        Type = connectionDefinition.Type,
                        EventID = 0,
                        ProgrammaticallySendEventTrigger = false,
                        Unicast = true
                    };

                    if (connectionDefinition.InputTag != null)
                        discreteIOConnection.InputTag = new InputTag
                        {
                            ExternalAccess = ExternalAccess.ReadWrite,
                            DataType = connectionDefinition.InputTag.DataType
                        };

                    if (connectionDefinition.OutputTag != null)
                        discreteIOConnection.OutputTag = new OutputTag
                        {
                            ExternalAccess = ExternalAccess.ReadWrite,
                            DataType = connectionDefinition.OutputTag.DataType
                        };

                    Communications.Connections = new List<DiscreteIOConnection>
                    {
                        discreteIOConnection
                    };
                }
                else
                {
                    Communications.Connections = null;
                }
            }
            else
            {
                Communications.ConfigTag = null;
                Communications.Connections = null;
            }

            RebuildDeviceTag();

            // For EnhancedRack
            if (ParentModule != null && ParentController != null)
            {
                if ((oldConnectionConfigID & (uint)DIOConnectionTypeMask.EnhancedRack) > 0
                    || (newConnectionConfigID & (uint)DIOConnectionTypeMask.EnhancedRack) > 0)
                {
                    ((DeviceModule)ParentModule).RebuildDeviceTag();
                }
            }

            Notifications.Publish(new MessageData() { Type = MessageData.MessageType.Verify });
        }

        public override void PostLoadJson()
        {
            RebuildDeviceTag();

            if (ConfigTag != null)
            {
                if (Communications.ConfigTag?.Data != null)
                {
                    ConfigTag.DataWrapper.Data.Update(ConvertBase64Data(Communications.ConfigTag.Data));
                    ConfigTag.ChildDescription = Communications.ConfigTag.Comments;
                }
            }

            if (InputTag != null)
            {
                //TODO(gjc): add code here?
            }

            if (OutputTag != null)
            {
                //TODO(gjc): add code here?
            }
        }

        public override void RebuildDeviceTag()
        {
            MarkReferenceTagRoutine();
            RebuildConfigTag();
            RebuildInputTag();
            RebuildOutputTag();
        }

        public override void RemoveDeviceTag()
        {
            DeleteTag(ConfigTag);
            DeleteTag(InputTag);
            DeleteTag(OutputTag);
        }

        public void RebuildConfigTag()
        {
            DeleteTag(ConfigTag);

            ConfigTag = null;

            if (ParentModule != null)
            {
                var dataTypeDefinition = ExportConfigDataType();
                var tagName = $"{ParentModule.Name}:{Slot}:C";

                if (!Profiles
                        .GetConnectionStringByConfigID(ConfigID, Major)
                        .Contains("Listen Only"))
                {
                    ConfigTag = CreateTag(tagName, dataTypeDefinition);
                }

                // load default
                UpdateConfigTagWithDefaultValue();
            }
        }

        private void UpdateConfigTagWithDefaultValue()
        {
            if (ConfigTag == null)
                return;

            var connectionConfigDefinition =
                Profiles.DIOModuleTypes.GetConnectionConfigDefinitionByID(ConfigID);

            var ioConfigTag = connectionConfigDefinition.ConfigTag;
            if (ioConfigTag == null)
                return;

            var dataValue = Profiles.DIOModuleTypes.GetDataValueByID(ioConfigTag.ValueID).ToArray();

            //
            int cfgSize = CalculateConfigSize(ConfigTag.DataWrapper.DataTypeInfo.DataType) + 4;
            int cfgIDNum = ioConfigTag.Instance;

            Array.Copy(BitConverter.GetBytes(cfgSize), 0, dataValue, 0, 4);
            Array.Copy(BitConverter.GetBytes(cfgIDNum), 0, dataValue, 4, 4);

            // Enhanced Rack
            if ((ConfigID & (uint)DIOConnectionTypeMask.EnhancedRack) > 0)
            {
                var connectionDefinition =
                    Profiles.DIOModuleTypes.GetConnectionDefinitionByID(
                        connectionConfigDefinition.Connections[0]);

                ushort enProdConnPt = (ushort)connectionDefinition.InputCxnPoint.GetValueOrDefault();
                Array.Copy(BitConverter.GetBytes(enProdConnPt), 0, dataValue, 10, 2);

                ushort enConsConnPt = (ushort)connectionDefinition.OutputCxnPoint.GetValueOrDefault();
                Array.Copy(BitConverter.GetBytes(enConsConnPt), 0, dataValue, 16, 2);

                // other update by adapter
                //EnProdDataOffset
                //EnProdSize
                //EnConsDataOffset
                //EnConsSize
            }

            ConfigTag.DataWrapper.Data.Update(dataValue, 0);

        }


        public void RebuildInputTag()
        {
            DeleteTag(InputTag);

            InputTag = null;

            if (ParentModule != null)
            {
                var dataTypeDefinition = ExportInputDataType();
                var tagName = $"{ParentModule.Name}:{Slot}:I";

                if ((ConfigID & (uint)DIOConnectionTypeMask.EnhancedRack) == 0)
                    InputTag = CreateTag(tagName, dataTypeDefinition);

                if (InputTag == null)
                {
                    //var inAliasTag = GetInAliasTag();
                    //if (inAliasTag != null)
                    //{
                    //    //Try add Alias Tag
                    //    //TODO(gjc): need edit here for display style
                    //    string aliasSpecifier = $"{ParentModule.Name}:I.Data[{Slot}]";
                    //    InputTag = CreateAliasTag(tagName, aliasSpecifier, DisplayStyle.Binary);
                    //}
                }
            }
        }

        public void RebuildOutputTag()
        {
            DeleteTag(OutputTag);

            OutputTag = null;

            if (ParentModule != null)
            {
                var dataTypeDefinition = ExportOutputDataType();
                var tagName = $"{ParentModule.Name}:{Slot}:O";

                if ((ConfigID & (uint)DIOConnectionTypeMask.EnhancedRack) == 0)
                    OutputTag = CreateTag(tagName, dataTypeDefinition);

                if (OutputTag == null)
                {
                    //var outAliasTag = GetOutAliasTag();
                    //if (outAliasTag != null)
                    //{
                    //    //Try add Alias Tag
                    //    string aliasSpecifier = $"{ParentModule.Name}:O.Data[{Slot}]";
                    //    OutputTag = CreateAliasTag(tagName, aliasSpecifier, DisplayStyle.Binary);
                    //}

                }
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

        private Tag CreateAliasTag(string tagName, string aliasSpecifier, DisplayStyle displayStyle)
        {
            if (ParentController != null && ParentModule != null)
            {
                var tagCollection = ParentController.Tags as TagCollection;
                Contract.Assert(tagCollection != null);

                var tag = TagsFactory.CreateAliasTag(
                    tagCollection,
                    tagName, aliasSpecifier, displayStyle);

                if (tag != null)
                {
                    tag.IsModuleTag = true;
                    tag.Verify();

                    tagCollection.AddTag(tag, false, false);
                }


                return tag;
            }

            return null;
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

        public void UpdateModuleTagName()
        {
            var tagCollection = InputTag?.ParentCollection as TagCollection;
            if (tagCollection != null)
                tagCollection.IsNeedVerifyRoutine = false;

            UpdateConfigTagName();
            UpdateInputTagName();
            UpdateOutputTagName();

            if (tagCollection != null)
            {
                CodeSynchronization.GetInstance().UpdateModuleName(this);
                CodeSynchronization.GetInstance().Update();
                tagCollection.IsNeedVerifyRoutine = true;
            }
        }

        #region Private

        private void UpdateConfigTagName()
        {
            if (ConfigTag != null) ConfigTag.Name = $"{ParentModule.Name}:{Slot}:C";
        }

        private void UpdateOutputTagName()
        {
            if (OutputTag != null)
                OutputTag.Name = $"{ParentModule.Name}:{Slot}:O";
        }

        private void UpdateInputTagName()
        {
            if (InputTag != null)
                InputTag.Name = $"{ParentModule.Name}:{Slot}:I";
        }

        private void LoadDefaultValue()
        {
            if (Profiles == null)
                return;

            ProductCode = Profiles.ProductCode;
            ProductType = Profiles.ProductType;
            Vendor = Profiles.VendorID;
            EKey = ElectronicKeyingType.CompatibleModule;

            var defaultModuleType = Profiles.GetDefaultModuleType();

            Major = defaultModuleType.MajorRev;
            Minor = 1;
            var series = Profiles.GetSeriesByMajor(Major);
            CatalogNumber = Profiles.CatalogNumber + "/" + series;

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

            // other
            // default data connection
            var defaultConnection = "Data";
            var connectionConfig =
                Profiles.GetConnectionConfig(defaultModuleType.ModuleDefinitionID, defaultConnection);
            var connectionConfigID = connectionConfig.Item1;


            Communications = new DiscreteIOCommunications();

            ChangeConnectionConfigID(connectionConfigID);
        }

        private DataTypeDefinition GetConfigDataTypeDefinition()
        {
            var connectionConfigDefinition =
                Profiles.DIOModuleTypes.GetConnectionConfigDefinitionByID(ConfigID);

            if (connectionConfigDefinition?.ConfigTag != null)
                return Profiles.DIOModuleTypes.GetDataTypeDefinitionByName(
                    connectionConfigDefinition.ConfigTag.DataType);

            return null;
        }

        private DataTypeDefinition GetInputDataTypeDefinition()
        {
            var connectionConfigDefinition =
                Profiles.DIOModuleTypes.GetConnectionConfigDefinitionByID(ConfigID);

            if (connectionConfigDefinition?.Connections != null && connectionConfigDefinition.Connections.Count > 0)
            {
                var connectionDefinition =
                    Profiles.DIOModuleTypes.GetConnectionDefinitionByID(connectionConfigDefinition
                        .Connections[0]);

                if (connectionDefinition?.InputTag != null)
                    return Profiles.DIOModuleTypes.GetDataTypeDefinitionByName(connectionDefinition.InputTag
                        .DataType);
            }

            return null;
        }

        private DataTypeDefinition GetOutputDataTypeDefinition()
        {
            var connectionConfigDefinition =
                Profiles.DIOModuleTypes.GetConnectionConfigDefinitionByID(ConfigID);

            if (connectionConfigDefinition?.Connections != null && connectionConfigDefinition.Connections.Count > 0)
            {
                var connectionDefinition =
                    Profiles.DIOModuleTypes.GetConnectionDefinitionByID(connectionConfigDefinition
                        .Connections[0]);

                if (connectionDefinition?.OutputTag != null)
                    return Profiles.DIOModuleTypes.GetDataTypeDefinitionByName(connectionDefinition.OutputTag
                        .DataType);
            }

            return null;
        }

        private IOAliasTag GetInAliasTag()
        {
            var connectionConfigDefinition =
                Profiles.DIOModuleTypes.GetConnectionConfigDefinitionByID(ConfigID);

            if (connectionConfigDefinition?.Connections != null && connectionConfigDefinition.Connections.Count > 0)
            {
                var connectionDefinition =
                    Profiles.DIOModuleTypes.GetConnectionDefinitionByID(connectionConfigDefinition
                        .Connections[0]);

                return connectionDefinition.InAliasTag;
            }

            return null;

        }

        private IOAliasTag GetOutAliasTag()
        {
            var connectionConfigDefinition =
                Profiles.DIOModuleTypes.GetConnectionConfigDefinitionByID(ConfigID);

            if (connectionConfigDefinition?.Connections != null && connectionConfigDefinition.Connections.Count > 0)
            {
                var connectionDefinition =
                    Profiles.DIOModuleTypes.GetConnectionDefinitionByID(connectionConfigDefinition
                        .Connections[0]);

                return connectionDefinition.OutAliasTag;
            }

            return null;
        }

        #endregion
    }

    public class DiscreteIOCommunications
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public uint? CommMethod { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<DiscreteIOConnection> Connections { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ConfigTag ConfigTag { get; set; }
    }

    public class ConfigTag
    {
        public int ConfigSize { get; set; }
        public ExternalAccess ExternalAccess { get; set; }
        public string DataType { get; set; }
        public JToken Data { get; set; }
        public JArray Comments { get; set; }
    }

    public class DiscreteIOConnection
    {
        public string Name { get; set; }
        public uint RPI { get; set; }
        public string Type { get; set; }
        public int EventID { get; set; }
        public bool ProgrammaticallySendEventTrigger { get; set; }
        public bool? Unicast { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public InputTag InputTag { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public OutputTag OutputTag { get; set; }
    }
}