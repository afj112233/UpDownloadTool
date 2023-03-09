using System;
using System.Collections.Generic;
using System.Diagnostics;
using ICSStudio.Interfaces.Common;
using System.Diagnostics.Contracts;
using ICSStudio.DeviceProfiles.Common;
using ICSStudio.DeviceProfiles.DIOEnetAdapter;
using ICSStudio.DeviceProfiles.DIOModule;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.Interfaces.Notification;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.Utils;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.DeviceModule
{
    public class AnalogIO : DeviceModule
    {
        public AnalogIO(IController controller, DIOModuleProfiles profiles) : base(controller)
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
                        CalculateConfigSize();

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

            if (Communications.Connections != null && Communications.Connections.Count > 0)
            {
                var ioConnection = Communications.Connections[0];

                if (InputTag != null && ioConnection.InputTag?.Data != null)
                {
                    try
                    {
                        InputTag.DataWrapper.Data.Update(ConvertBase64Data(ioConnection.InputTag.Data));
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }

                    InputTag.ChildDescription = ioConnection.InputTag.Comments;
                }

                if (OutputTag != null && ioConnection.OutputTag?.Data != null)
                {
                    try
                    {
                        OutputTag.DataWrapper.Data.Update(ConvertBase64Data(ioConnection.OutputTag.Data));
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }

                    OutputTag.ChildDescription = ioConnection.OutputTag.Comments;
                }

            }

            Notifications.Publish(new MessageData() { Type = MessageData.MessageType.Verify });
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
            Notifications.Publish(new MessageData()
            {
                Object = new List<ITag>() { ConfigTag, InputTag, OutputTag }, Type = MessageData.MessageType.DelTag
            });
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
                    throw new NotImplementedException("Check here!");
                    //((DeviceModule)ParentModule).RebuildDeviceTag();
                }
            }

            Notifications.Publish(new MessageData() { Type = MessageData.MessageType.Verify });
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

        #region CalculateConfigSize

        private int CalculateConfigSize()
        {
            var collection = ParentController.DataTypes as DataTypeCollection;
            Contract.Assert(collection != null);

            var dataTypeDefinition = ExportConfigDataType();
            if (dataTypeDefinition == null)
                return 0;

            var definitionList = dataTypeDefinition["Members"]?.ToObject<List<DataTypeMemberDefinition>>();
            if (definitionList == null)
                return 0;

            int offset = 0;
            int align = 4;
            foreach (var definition in definitionList)
            {
                if (definition.DataType == "BIT")
                    continue;

                if (definition.DataType == "BITFIELD")
                    continue;

                var tp = collection[definition.DataType];

                int dim = definition.Dimension != null
                    ? int.Parse(definition.Dimension)
                    : 0;

                align = Math.Max(align, tp.AlignSize);
                offset = AlignUp(offset, RealAlignSize(dim, tp));

                offset += RealSize(dim, tp.ByteSize, tp is BOOL);
            }

            return offset - 8;
        }

        int AlignUp(int value, int align)
        {
            return (value + align - 1) / align * align;
        }

        int RealAlignSize(int dim, IDataType tp)
        {
            if (dim != 0 && tp is BOOL) return 4;
            return tp.AlignSize;
        }

        private int RealSize(int dim, int typeSize, bool isBool)
        {
            Contract.Assert(!isBool || dim != 0);
            if (isBool) return dim / 8;
            return dim == 0 ? typeSize : typeSize * dim;
        }

        #endregion

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

        private void DeleteTag(Tag tag)
        {
            if (tag != null && ParentController != null)
            {
                var tagCollection = ParentController.Tags as TagCollection;
                Contract.Assert(tagCollection != null);

                tagCollection.DeleteTag(tag, true, true, false);
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
            int cfgSize = CalculateConfigSize() + 4;
            int cfgIDNum = ioConfigTag.Instance;

            Array.Copy(BitConverter.GetBytes(cfgSize), 0, dataValue, 0, 4);
            Array.Copy(BitConverter.GetBytes(cfgIDNum), 0, dataValue, 4, 4);

            // Enhanced Rack
            if ((ConfigID & (uint)DIOConnectionTypeMask.EnhancedRack) > 0)
            {
                throw new NotImplementedException("Check here!");

                /*
                var connectionDefinition =
                    Profiles.DIOModuleTypes.GetConnectionDefinitionByID(
                        connectionConfigDefinition.Connections[0]);

                ushort enProdConnPt = (ushort) connectionDefinition.InputCxnPoint.GetValueOrDefault();
                Array.Copy(BitConverter.GetBytes(enProdConnPt), 0, dataValue, 10, 2);

                ushort enConsConnPt = (ushort) connectionDefinition.OutputCxnPoint.GetValueOrDefault();
                Array.Copy(BitConverter.GetBytes(enConsConnPt), 0, dataValue, 16, 2);
                */

                // other update by adapter
                //EnProdDataOffset
                //EnProdSize
                //EnConsDataOffset
                //EnConsSize
            }

            ConfigTag.DataWrapper.Data.Update(dataValue, 0);

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
    }
}
