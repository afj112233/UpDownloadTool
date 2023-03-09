using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Text;
using ICSStudio.DeviceProfiles.Common;
using ICSStudio.DeviceProfiles.DIOEnetAdapter;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.Interfaces.Notification;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.Utils;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.DeviceModule
{
    public class CommunicationsAdapter : DeviceModule
    {
        public CommunicationsAdapter(IController controller, DIOEnetAdapterProfiles profiles) : base(controller)
        {
            Contract.Assert(profiles != null);

            Type = DeviceType.Adapter;
            Profiles = profiles;

            LoadDefaultValue();

            PropertyChanged += OnPropertyChanged;
        }

        public int ChassisSize
        {
            get
            {
                var pointIOPort = GetFirstPort(PortType.PointIO);
                return pointIOPort.Bus.Size;
            }
            set
            {
                var pointIOPort = GetFirstPort(PortType.PointIO);
                if (pointIOPort != null && pointIOPort.Bus.Size != value)
                {
                    pointIOPort.Bus.Size = value;

                    RebuildDeviceTag();

                    Notifications.Publish(new MessageData() { Type = MessageData.MessageType.Verify });
                }
            }
        }

        public DIOEnetAdapterProfiles Profiles { get; }

        public DiscreteIOCommunications Communications { get; set; }

        public uint ConfigID { get; set; }

        public bool IsEnhancedRack
            => (ConfigID & (uint)DIOConnectionTypeMask.EnhancedRack) > 0;

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                var tagCollection = InputTag?.ParentCollection as TagCollection;
                if (tagCollection != null)
                    tagCollection.IsNeedVerifyRoutine = false;
                if (InputTag != null) InputTag.Name = $"{Name}:I";

                if (OutputTag != null) OutputTag.Name = $"{Name}:O";

                if (tagCollection != null)
                {
                    CodeSynchronization.GetInstance().UpdateModuleName(this);
                    CodeSynchronization.GetInstance().Update();
                    tagCollection.IsNeedVerifyRoutine = true;
                }

                if (ParentController != null)
                {
                    foreach (var item in ParentController.DeviceModules)
                    {
                        var deviceModule = item as DeviceModule;
                        if (deviceModule != null && deviceModule.ParentModule == this)
                        {
                            var discreteIO = deviceModule as DiscreteIO;
                            discreteIO?.UpdateModuleTagName();

                            var analogIO = deviceModule as AnalogIO;
                            analogIO?.UpdateModuleTagName();
                        }
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
                        ioConnection.InputTag.Data = System.Convert.ToBase64String(data.ToArray());

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
                        ioConnection.OutputTag.Data = System.Convert.ToBase64String(data.ToArray());

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

                module.Add("Communications", JToken.FromObject(Communications));
            }


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

        public void CheckDataType()
        {
            if (IsEnhancedRack)
            {
                // recalculate crc
                var inputDataType = ExportInputDataType();
                if (inputDataType != null)
                    Communications.Connections[0].InputTag.DataType = (string)inputDataType["Name"];

                var outputDataType = ExportOutputDataType();
                if (outputDataType != null)
                    Communications.Connections[0].OutputTag.DataType = (string)outputDataType["Name"];
            }
        }

        public void ChangeConnectionConfigID(uint connectionConfigID)
        {
            if (ConfigID == connectionConfigID)
                return;

            ConfigID = connectionConfigID;

            uint commMethod = Profiles.GetCommMethodByConfigID(connectionConfigID, Major);
            //TODO(gjc): EnRack keep CommMethod???
            if (commMethod != 0)
                Communications.CommMethod = commMethod;

            Communications.Connections.Clear();

            var definition = Profiles.GetConnectionConfigDefinitionByID(connectionConfigID);
            if (definition.Connections != null && definition.Connections.Count > 0)
            {
                var inputDataType = ExportInputDataType();

                var inputTag =
                    inputDataType == null
                        ? null
                        : new InputTag
                        {
                            ExternalAccess = ExternalAccess.ReadWrite,
                            DataType = inputDataType["Name"].ToString()
                        };

                var outputDataType = ExportOutputDataType();
                var outputTag =
                    outputDataType == null
                        ? null
                        : new OutputTag
                        {
                            ExternalAccess = ExternalAccess.ReadWrite,
                            DataType = outputDataType["Name"].ToString()
                        };

                var connectionDefinition = Profiles.GetConnectionDefinitionByID(definition.Connections[0]);

                var connection = new DiscreteIOConnection
                {
                    Name = connectionDefinition.Name,
                    Type = connectionDefinition.Type,
                    RPI = connectionDefinition.RPI,
                    EventID = 0,
                    ProgrammaticallySendEventTrigger = false,
                    Unicast = true,
                    InputTag = inputTag,
                    OutputTag = outputTag
                };

                Communications.Connections.Add(connection);
            }

            RebuildDeviceTag();

            Notifications.Publish(new MessageData() { Type = MessageData.MessageType.Verify });
        }

        public int GetMaxChildSlot()
        {
            var maxChildSlot = 0;

            if (ParentController != null)
                foreach (var item in ParentController.DeviceModules)
                {
                    var deviceModule = item as DeviceModule;
                    if (deviceModule != null && deviceModule.ParentModule == this)
                    {
                        var port = deviceModule.GetFirstPort(PortType.PointIO);
                        if (port != null)
                        {
                            var slot = int.Parse(port.Address);
                            if (slot > maxChildSlot)
                                maxChildSlot = slot;
                        }
                    }
                }

            return maxChildSlot;
        }

        public uint GetChildRackConnectionMask()
        {
            uint connectionMask = 0;
            if (ParentController != null)
                foreach (var item in ParentController.DeviceModules)
                {
                    var deviceModule = item as DeviceModule;
                    if (deviceModule != null && deviceModule.ParentModule == this)
                    {
                        var discreteIO = deviceModule as DiscreteIO;
                        if (discreteIO != null)
                        {
                            var connectionConfigID = discreteIO.ConfigID;

                            connectionMask |= connectionConfigID;
                        }
                    }
                }

            return connectionMask & (uint)(DIOConnectionTypeMask.ListenOnlyRack
                                           | DIOConnectionTypeMask.Rack
                                           | DIOConnectionTypeMask.EnhancedRack);
        }

        public DrivePort GetDrivePortInProfiles(string portType)
        {
            return Profiles.GetDrivePort(portType);
        }

        public JObject ExportInputDataType()
        {
            var dataTypeDefinition = GetInputDataTypeDefinition();
            if (dataTypeDefinition != null)
            {
                if (dataTypeDefinition.Name.Contains("{ChassisSize}"))
                    return DataTypeDefinitionToJObject(dataTypeDefinition, ChassisSize);

                if (dataTypeDefinition.Name.Contains("{CRC}"))
                    return DataTypeDefinitionWithCRCToJObject(dataTypeDefinition, ChassisSize, true);
            }

            return null;
        }

        public JObject ExportOutputDataType()
        {
            var dataTypeDefinition = GetOutputDataTypeDefinition();
            if (dataTypeDefinition != null)
            {
                if (dataTypeDefinition.Name.Contains("{ChassisSize}"))
                    return DataTypeDefinitionToJObject(dataTypeDefinition, ChassisSize);

                if (dataTypeDefinition.Name.Contains("{CRC}"))
                    return DataTypeDefinitionWithCRCToJObject(dataTypeDefinition, ChassisSize, false);
            }

            return null;
        }

        public override void PostLoadJson()
        {
            RebuildDeviceTag();

            if (ConfigTag != null)
            {
                //TODO(gjc): add code here
            }

            if (InputTag != null)
            {
                if (Communications.Connections[0].InputTag.Data != null)
                {
                    InputTag.DataWrapper.Data.Update(ConvertBase64Data(Communications.Connections[0].InputTag.Data));
                    InputTag.ChildDescription = Communications.Connections[0].InputTag.Comments;
                }
            }

            if (OutputTag != null)
            {
                if (Communications.Connections[0].OutputTag.Data != null)
                {
                    OutputTag.DataWrapper.Data.Update(ConvertBase64Data(Communications.Connections[0].OutputTag.Data));
                    OutputTag.ChildDescription = Communications.Connections[0].OutputTag.Comments;
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

            UpdateMemberOffset();
        }

        public override void RemoveDeviceTag()
        {
            DeleteTag(ConfigTag);
            DeleteTag(InputTag);
            DeleteTag(OutputTag);
        }

        private void RebuildConfigTag()
        {
            //TODO(gjc): need check  ConfigTag == null?
            DeleteTag(ConfigTag);
            ConfigTag = null;
        }

        private void RebuildInputTag()
        {
            DeleteTag(InputTag);
            InputTag = null;

            var inputDataType = ExportInputDataType();

            AddMemberDataType(true);

            InputTag = CreateTag($"{Name}:I", inputDataType);
        }

        private void RebuildOutputTag()
        {
            DeleteTag(OutputTag);
            OutputTag = null;

            var outputDataType = ExportOutputDataType();

            AddMemberDataType(false);

            OutputTag = CreateTag($"{Name}:O", outputDataType);
        }

        private void UpdateMemberOffset()
        {
            if (IsEnhancedRack && ParentController != null)
            {
                var dataTypeCollection = ParentController.DataTypes as DataTypeCollection;
                Contract.Assert(dataTypeCollection != null);

                var discreteIOList = new List<DiscreteIO>();

                if (ParentController?.DeviceModules != null)
                    foreach (var module in ParentController.DeviceModules)
                    {
                        var discreteIO = module as DiscreteIO;
                        if (discreteIO != null
                            && discreteIO.ParentModule == this
                            && discreteIO.IsEnhancedRack)
                            discreteIOList.Add(discreteIO);
                    }

                discreteIOList.Sort((x, y) => x.Slot.CompareTo(y.Slot));

                ushort prodDataOffset = 0;
                ushort consDataOffset = 0;

                foreach (var discreteIO in discreteIOList)
                {
                    ushort enProdDataOffset = prodDataOffset;
                    ushort enProdSize = 0;
                    ushort enConsDataOffset = consDataOffset;
                    ushort enConsSize = 0;

                    // input
                    JObject dataTypeDefinition = discreteIO.ExportInputDataType();
                    if (dataTypeDefinition != null)
                    {
                        var dataType = dataTypeCollection[(string)dataTypeDefinition["Name"]] as ModuleDefinedDataType;
                        Contract.Assert(dataType != null);

                        enProdSize = (ushort)dataType.ByteSize;

                        prodDataOffset += (ushort)dataType.ByteSize;
                    }

                    if (discreteIO.ConfigTag != null)
                    {
                        discreteIO.ConfigTag.SetStringValue("EnProdDataOffset", enProdDataOffset.ToString());
                        discreteIO.ConfigTag.SetStringValue("EnProdSize", enProdSize.ToString());
                    }

                    // output
                    dataTypeDefinition = discreteIO.ExportOutputDataType();
                    if (dataTypeDefinition != null)
                    {
                        var dataType = dataTypeCollection[(string)dataTypeDefinition["Name"]] as ModuleDefinedDataType;
                        Contract.Assert(dataType != null);

                        enConsSize = (ushort)dataType.ByteSize;

                        consDataOffset += (ushort)dataType.ByteSize;
                    }

                    if (discreteIO.ConfigTag != null)
                    {
                        discreteIO.ConfigTag.SetStringValue("EnConsDataOffset", enConsDataOffset.ToString());
                        discreteIO.ConfigTag.SetStringValue("EnConsSize", enConsSize.ToString());
                    }

                    //Console.WriteLine($"{discreteIO.DisplayText} {enProdDataOffset},{enProdSize},{enConsDataOffset},{enConsSize}");
                }
            }
        }

        private void AddMemberDataType(bool beInput)
        {
            if (ParentController != null && IsEnhancedRack)
            {
                var dataTypeCollection = ParentController.DataTypes as DataTypeCollection;

                Contract.Assert(dataTypeCollection != null);

                foreach (var module in ParentController.DeviceModules)
                {
                    var discreteIO = module as DiscreteIO;
                    if (discreteIO != null && discreteIO.ParentModule == this)
                        if ((discreteIO.ConfigID &
                             (uint)DIOConnectionTypeMask.EnhancedRack) > 0)
                        {
                            var dataTypeDefinition =
                                beInput ? discreteIO.ExportInputDataType() : discreteIO.ExportOutputDataType();

                            if (dataTypeDefinition != null)
                            {
                                var dataTypeName = dataTypeDefinition["Name"].ToString();

                                if (dataTypeCollection[dataTypeName] == null)
                                {
                                    var dataType = new ModuleDefinedDataType(dataTypeDefinition);
                                    dataType.PostInit(dataTypeCollection);
                                    dataTypeCollection.AddDataType(dataType);
                                }
                            }
                        }
                }
            }
        }

        private Tag CreateTag(string tagName, JObject dataTypeDefinition)
        {
            if (dataTypeDefinition != null && ParentController != null)
            {
                var dataTypeCollection = ParentController.DataTypes as DataTypeCollection;
                var tagCollection = ParentController.Tags as TagCollection;

                Contract.Assert(dataTypeCollection != null);
                Contract.Assert(tagCollection != null);

                var dataTypeName = dataTypeDefinition["Name"].ToString();

                if (dataTypeCollection[dataTypeName] == null)
                {
                    var dataType = new ModuleDefinedDataType(dataTypeDefinition);
                    dataType.PostInit(dataTypeCollection);
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
                    Upstream = true,
                    Bus = new Bus()
                };

                if (port.Type == PortType.PointIO)
                {
                    port.Address = "0";
                    port.Bus.Size = 1;
                }

                if (profilesPort.ExtendedProperties != null)
                    if (profilesPort.ExtendedProperties.DownstreamOnly)
                        port.Upstream = false;

                Ports.Add(port);
            }

            //////
            var defaultConnectionConfig = Profiles.GetDefaultConnectionConfigByMajor(Major);
            var defaultConnectionConfigID = defaultConnectionConfig.Item1;
            var defaultCommMethod = defaultConnectionConfig.Item2;

            ConfigID = defaultConnectionConfigID;

            var definition = Profiles.GetConnectionConfigDefinitionByID(defaultConnectionConfigID);
            var connectionDefinition = Profiles.GetConnectionDefinitionByID(definition.Connections[0]);

            //////
            InputTag inputTag = null;
            if (connectionDefinition.InputTag != null)
            {
                var dateType = connectionDefinition.InputTag.DataType;
                dateType = dateType.Replace("{ChassisSize}", "1");
                inputTag = new InputTag
                {
                    ExternalAccess = ExternalAccess.ReadWrite,
                    DataType = dateType
                };
            }

            OutputTag outputTag = null;
            if (connectionDefinition.OutputTag != null)
            {
                var dateType = connectionDefinition.OutputTag.DataType;
                dateType = dateType.Replace("{ChassisSize}", "1");
                outputTag = new OutputTag
                {
                    ExternalAccess = ExternalAccess.ReadWrite,
                    DataType = dateType
                };
            }

            Communications = new DiscreteIOCommunications
            {
                CommMethod = defaultCommMethod,
                Connections = new List<DiscreteIOConnection>
                {
                    new DiscreteIOConnection
                    {
                        Name = connectionDefinition.Name,
                        Type = connectionDefinition.Type,
                        RPI = connectionDefinition.RPI,
                        EventID = 0,
                        ProgrammaticallySendEventTrigger = false,
                        Unicast = true,
                        InputTag = inputTag,
                        OutputTag = outputTag
                    }
                }
            };
        }

        private DataTypeDefinition GetOutputDataTypeDefinition()
        {
            var definition = Profiles.GetConnectionConfigDefinitionByID(ConfigID);
            if (definition.Connections != null && definition.Connections.Count > 0)
            {
                var connectionDefinition = Profiles.GetConnectionDefinitionByID(definition.Connections[0]);
                if (connectionDefinition.OutputTag != null)
                {
                    var dataType = connectionDefinition.OutputTag.DataType;
                    return Profiles.GetDataTypeDefinitionByName(dataType);
                }
            }

            return null;
        }

        private DataTypeDefinition GetInputDataTypeDefinition()
        {
            var definition = Profiles.GetConnectionConfigDefinitionByID(ConfigID);
            if (definition.Connections != null && definition.Connections.Count > 0)
            {
                var connectionDefinition = Profiles.GetConnectionDefinitionByID(definition.Connections[0]);
                if (connectionDefinition.InputTag != null)
                {
                    var dataType = connectionDefinition.InputTag.DataType;
                    return Profiles.GetDataTypeDefinitionByName(dataType);
                }
            }

            return null;
        }

        private JObject DataTypeDefinitionToJObject(DataTypeDefinition dataTypeDefinition, int chassisSize)
        {
            Contract.Assert(dataTypeDefinition.Name.Contains("{ChassisSize}"));

            var members = new JArray();

            foreach (var memberDefinition
                     in dataTypeDefinition.Members)
            {
                var member = new JObject();

                var name = memberDefinition.Name.Replace("{ChassisSize}", $"{chassisSize}");

                member.Add("Name", name);
                member.Add("DataType", memberDefinition.DataType);

                if (memberDefinition.Hidden)
                    member.Add("Hidden", true);

                member.Add("Radix", (int)memberDefinition.Radix);

                var dim = 0;
                if (memberDefinition.Dimension != null)
                    dim = int.Parse(memberDefinition.Dimension.Replace("{ChassisSize}", $"{chassisSize}"));

                if (dim > 0)
                    member.Add("Dimension", dim);

                if (memberDefinition.DataType == "BIT")
                {
                    member.Add("BitNumber", memberDefinition.BitNumber);
                    member.Add("Target", memberDefinition.Target);
                }

                members.Add(member);
            }

            return new JObject
            {
                { "Name", dataTypeDefinition.Name.Replace("{ChassisSize}", $"{ChassisSize}") },
                { "Class", (int)EnumUtils.Parse<DataTypeClass>(dataTypeDefinition.Class) },
                { "Members", members }
            };
        }

        private JObject DataTypeDefinitionWithCRCToJObject(DataTypeDefinition dataTypeDefinition, int chassisSize,
            bool beInput)
        {
            Contract.Assert(IsEnhancedRack);
            Contract.Assert(dataTypeDefinition.Name.Contains("{CRC}"));

            var members = new JArray();

            foreach (var memberDefinition
                     in dataTypeDefinition.Members)
            {
                var member = new JObject
                {
                    { "Name", memberDefinition.Name }, { "DataType", memberDefinition.DataType }
                };

                if (memberDefinition.Hidden)
                    member.Add("Hidden", true);

                member.Add("Radix", (int)memberDefinition.Radix);

                var dim = 0;
                if (memberDefinition.Dimension != null)
                    dim = int.Parse(memberDefinition.Dimension);

                if (dim > 0)
                    member.Add("Dimension", dim);

                if (memberDefinition.DataType == "BIT")
                {
                    member.Add("BitNumber", memberDefinition.BitNumber);
                    member.Add("Target", memberDefinition.Target);
                }

                members.Add(member);
            }

            // enhanced rack io module
            var discreteIOList = new List<DiscreteIO>();

            if (ParentController?.DeviceModules != null)
                foreach (var module in ParentController.DeviceModules)
                {
                    var discreteIO = module as DiscreteIO;
                    if (discreteIO != null && discreteIO.ParentModule == this)
                        if ((discreteIO.ConfigID &
                             (uint)DIOConnectionTypeMask.EnhancedRack) > 0)
                            discreteIOList.Add(discreteIO);
                }

            discreteIOList.Sort((x, y) => x.Slot.CompareTo(y.Slot));

            foreach (var discreteIO in discreteIOList)
            {
                var dataType = beInput ? discreteIO.ExportInputDataType() : discreteIO.ExportOutputDataType();

                if (dataType != null)
                {
                    var member = new JObject
                    {
                        { "Name", $"Slot{discreteIO.Slot:D2}" }, { "DataType", dataType["Name"] }
                    };

                    members.Add(member);
                }
            }

            // CRC
            var builder = new StringBuilder();
            builder.Append(chassisSize);

            foreach (var member in members) builder.Append(member["Name"]);

            var crc = Utils.Utils.HashByCrc32C(builder.ToString());

            return new JObject
            {
                { "Name", dataTypeDefinition.Name.Replace("{CRC}", $"_{crc}") },
                { "Class", (int)EnumUtils.Parse<DataTypeClass>(dataTypeDefinition.Class) },
                { "Members", members }
            };
        }
    }

    public class InputTag
    {
        public ExternalAccess ExternalAccess { get; set; }
        public string DataType { get; set; }
        public JToken Data { get; set; }
        public JArray Comments { get; set; }
    }

    public class OutputTag
    {
        public ExternalAccess ExternalAccess { get; set; }
        public string DataType { get; set; }
        public JToken Data { get; set; }
        public JArray Comments { get; set; }
    }

}