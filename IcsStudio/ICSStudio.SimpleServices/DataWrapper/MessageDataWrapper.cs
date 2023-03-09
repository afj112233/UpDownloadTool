using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Text;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.PredefinedType;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.DataWrapper
{
    public class MessageDataWrapper : DataWrapper
    {
        private readonly IController _controller;
        private MessageParameters _parameters;

        public static MessageDataWrapper Create(IDataType dataType, IController controller)
        {
            if (dataType == null
                || !dataType.Name.Equals("MESSAGE", StringComparison.OrdinalIgnoreCase))
                return null;

            MessageDataWrapper dataWrapper = new MessageDataWrapper(dataType, controller);

            return dataWrapper;
        }

        private MessageDataWrapper(IDataType dataType, IController controller)
            : base(dataType, 0, 0, 0, null)
        {
            _parameters = new MessageParameters();

            _controller = controller;

            InitializeParameters(_parameters);
        }

        public MessageParameters Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        public ushort ServiceCode
        {
            get { return _parameters.ServiceCode; }
            set { _parameters.ServiceCode = value; }
        }

        public ushort ClassID
        {
            get
            {
                Int16Field field = GetClassField();

                return (ushort)field.value;
            }

            set
            {
                Int16Field field = GetClassField();
                field.value = (short)value;

                _parameters.ObjectType = value;

                NotifyParentPropertyChanged($"{ParentTag?.Name}.Class");
            }
        }

        public uint InstanceID
        {
            get
            {
                Int32Field field = GetInstanceField();
                return (uint)field.value;
            }
            set
            {
                Int32Field field = GetInstanceField();
                field.value = (int)value;

                _parameters.TargetObject = value;

                NotifyParentPropertyChanged($"{ParentTag?.Name}.Instance");
            }
        }

        public ushort AttributeID
        {
            get
            {
                Int16Field field = GetAttributeField();

                return (ushort)field.value;
            }

            set
            {
                Int16Field field = GetAttributeField();
                field.value = (short)value;

                _parameters.AttributeNumber = value;

                NotifyParentPropertyChanged($"{ParentTag?.Name}.Attribute");
            }
        }

        public short SourceLength
        {
            get
            {
                Int16Field field = GetSourceLengthField();

                return field.value;
            }

            set
            {
                Int16Field field = GetSourceLengthField();
                field.value = value;

                _parameters.RequestedLength = (ushort)value;

                NotifyParentPropertyChanged($"{ParentTag?.Name}.REQ_LEN");

            }
        }

        public string SourceElement
        {
            get { return _parameters.LocalElement; }
            set { _parameters.LocalElement = value; }
        }

        public string DestinationElement
        {
            get { return _parameters.DestinationTag; }
            set { _parameters.DestinationTag = value; }
        }

        public string ConnectionPath
        {
            get
            {
                STRINGField field = GetPathField();

                List<byte> bytes = field.Get();

                if (bytes.Count == 0)
                    return string.Empty;

                if (bytes.Count == 2)
                {
                    return $"{bytes[0]}, {bytes[1]}";
                }

                if (bytes.SequenceEqual(new byte[] { 0x1F, 0, 0, 0 }))
                    return "THIS";

                // ip address
                if (bytes.Count >= 10 && bytes[0] == 18)
                {
                    int ipLength = bytes[1];

                    if (ipLength <= bytes.Count - 2)
                    {
                        string ipAddress
                            = Encoding.ASCII.GetString(bytes.ToArray(), 2, ipLength);

                        return $"2, {ipAddress}";
                    }
                }

                return string.Empty;
            }

            set
            {
                STRINGField field = GetPathField();
                List<byte> bytes = ConnectionPathToBytes(value);
                field.Set(bytes);

                _parameters.ConnectionPath = value;

                //TODO(gjc): check later
                NotifyParentPropertyChanged($"{ParentTag?.Name}.Path");
            }
        }

        public int LocalIndex
        {
            get
            {
                Int32Field field = GetLocalIndexField();

                return field.value;
            }

            set
            {
                Int32Field field = GetLocalIndexField();
                field.value = value;

                _parameters.LocalIndex = (uint)value;

                NotifyParentPropertyChanged($"{ParentTag?.Name}.LocalIndex");

            }
        }

        public bool TimedOut
        {
            get { return GetTimedOut(); }
            set
            {
                SetTimedOut(value);

                _parameters.TimedOut = value;

                NotifyParentPropertyChanged($"{ParentTag?.Name}.TO");
            }
        }

        public bool Connected
        {
            get
            {
                if (_parameters.ConnectedFlag == 1)
                    return true;

                return false;
            }

            set { _parameters.ConnectedFlag = value ? (byte)1 : (byte)2; }
        }

        public bool CacheConnections
        {
            get { return GetCacheConnections(); }
            set
            {
                SetCacheConnections(value);

                _parameters.CacheConnections = value;

                NotifyParentPropertyChanged($"{ParentTag?.Name}.EN_CC");

            }
        }

        public uint UnconnectedTimeout
        {
            get
            {
                Int32Field field = GetUnconnectedTimeoutField();
                return (uint)field.value;
            }
            set
            {
                Int32Field field = GetUnconnectedTimeoutField();
                field.value = (int)value;

                _parameters.UnconnectedTimeout = value;

                NotifyParentPropertyChanged($"{ParentTag?.Name}.UnconnectedTimeout");

            }
        }



        public uint ConnectionRate
        {
            get
            {
                Int32Field field = GetConnectionRateField();
                return (uint)field.value;
            }
            set
            {
                Int32Field field = GetConnectionRateField();
                field.value = (int)value;

                _parameters.ConnectionRate = value;

                NotifyParentPropertyChanged($"{ParentTag?.Name}.ConnectionRate");

            }
        }

        public byte TimeoutMultiplier
        {
            get
            {
                Int8Field field = GetTimeoutMultiplierField();
                return (byte)field.value;
            }
            set
            {
                Int8Field field = GetTimeoutMultiplierField();
                field.value = (sbyte)value;

                _parameters.TimeoutMultiplier = value;

                NotifyParentPropertyChanged($"{ParentTag?.Name}.TimeoutMultiplier");

            }
        }

        public void PostLoadJson()
        {
            //TODO(gjc): add code here
        }

        private void InitializeParameters(MessageParameters parameters)
        {
            parameters.MessageType = (byte)MessageTypeEnum.Unconfigured;
            parameters.RequestedLength = 1;
            parameters.CommTypeCode = 0;
            parameters.LocalIndex = 0;

            parameters.CacheConnections = true;
            parameters.UnconnectedTimeout = 30000000;
            parameters.ConnectionRate = 7500000;
            // member
            // Flags, 0x200(512)
            // REQ_LEN, 1
            // UnconnectedTimeout, 30000000
            // ConnectionRate, 7500000

            CompositiveType compositiveType = DataTypeInfo.DataType as CompositiveType;
            ICompositeField compositeField = Data as ICompositeField;

            Contract.Assert(compositiveType != null);
            Contract.Assert(compositeField != null);

            var typeMember = compositiveType.TypeMembers["Flags"] as DataTypeMember;
            if (typeMember != null)
            {
                IField field = compositeField.fields[typeMember.FieldIndex].Item1;
                field.Update("512");
            }

            typeMember = compositiveType.TypeMembers["REQ_LEN"] as DataTypeMember;
            if (typeMember != null)
            {
                IField field = compositeField.fields[typeMember.FieldIndex].Item1;
                field.Update("1");
            }

            typeMember = compositiveType.TypeMembers["UnconnectedTimeout"] as DataTypeMember;
            if (typeMember != null)
            {
                IField field = compositeField.fields[typeMember.FieldIndex].Item1;
                field.Update("30000000");
            }

            typeMember = compositiveType.TypeMembers["ConnectionRate"] as DataTypeMember;
            if (typeMember != null)
            {
                IField field = compositeField.fields[typeMember.FieldIndex].Item1;
                field.Update("7500000");
            }
        }

        public void ParametersToMembers()
        {
            if (Parameters != null)
            {
                MessageTypeEnum messageType = (MessageTypeEnum)Parameters.MessageType;
                if (messageType == MessageTypeEnum.CIPGeneric)
                {
                    //REQ_LEN->RequestedLength
                    //Path->ConnectionPath
                    //Class->ObjectType
                    //Instance->TargetObject
                    //Attribute->AttributeNumber
                    //LocalIndex->LocalIndex

                    CompositiveType compositiveType = DataTypeInfo.DataType as CompositiveType;
                    ICompositeField compositeField = Data as ICompositeField;

                    Contract.Assert(compositiveType != null);
                    Contract.Assert(compositeField != null);

                    var typeMember = compositiveType.TypeMembers["REQ_LEN"] as DataTypeMember;
                    if (typeMember != null)
                    {
                        IField field = compositeField.fields[typeMember.FieldIndex].Item1;
                        field.Update(Parameters.RequestedLength.ToString());
                    }

                    typeMember = compositiveType.TypeMembers["Class"] as DataTypeMember;
                    if (typeMember != null)
                    {
                        IField field = compositeField.fields[typeMember.FieldIndex].Item1;
                        field.Update(Parameters.ObjectType.ToString());
                    }

                    typeMember = compositiveType.TypeMembers["Instance"] as DataTypeMember;
                    if (typeMember != null)
                    {
                        IField field = compositeField.fields[typeMember.FieldIndex].Item1;
                        field.Update(Parameters.TargetObject.ToString());
                    }

                    typeMember = compositiveType.TypeMembers["Attribute"] as DataTypeMember;
                    if (typeMember != null)
                    {
                        IField field = compositeField.fields[typeMember.FieldIndex].Item1;
                        field.Update(Parameters.AttributeNumber.ToString());
                    }

                    typeMember = compositiveType.TypeMembers["LocalIndex"] as DataTypeMember;
                    if (typeMember != null)
                    {
                        IField field = compositeField.fields[typeMember.FieldIndex].Item1;
                        field.Update(Parameters.LocalIndex.ToString());
                    }


                    typeMember = compositiveType.TypeMembers["Path"] as DataTypeMember;
                    if (typeMember != null)
                    {
                        STRINGField stringField = compositeField.fields[typeMember.FieldIndex].Item1 as STRINGField;

                        List<byte> bytes = ConnectionPathToBytes(Parameters.ConnectionPath);
                        stringField.Set(bytes);
                    }

                    //TO->TimedOut
                    //EN_CC->CacheConnections
                    //UnconnectedTimeout->UnconnectedTimeout
                    //ConnectionRate->ConnectionRate
                    //TimeoutMultiplier->TimeoutMultiplier
                    typeMember = compositiveType.TypeMembers["TO"] as DataTypeMember;
                    if (typeMember != null && typeMember.IsBit)
                    {
                        IField field = compositeField.fields[typeMember.FieldIndex].Item1;
                        field.SetBitValue(typeMember.BitOffset, Parameters.TimedOut);
                    }

                    typeMember = compositiveType.TypeMembers["EN_CC"] as DataTypeMember;
                    if (typeMember != null && typeMember.IsBit)
                    {
                        IField field = compositeField.fields[typeMember.FieldIndex].Item1;
                        field.SetBitValue(typeMember.BitOffset, Parameters.CacheConnections);
                    }

                    typeMember = compositiveType.TypeMembers["UnconnectedTimeout"] as DataTypeMember;
                    if (typeMember != null)
                    {
                        IField field = compositeField.fields[typeMember.FieldIndex].Item1;
                        field.Update(Parameters.UnconnectedTimeout.ToString());
                    }

                    typeMember = compositiveType.TypeMembers["ConnectionRate"] as DataTypeMember;
                    if (typeMember != null)
                    {
                        IField field = compositeField.fields[typeMember.FieldIndex].Item1;
                        field.Update(Parameters.ConnectionRate.ToString());
                    }

                    typeMember = compositiveType.TypeMembers["TimeoutMultiplier"] as DataTypeMember;
                    if (typeMember != null)
                    {
                        IField field = compositeField.fields[typeMember.FieldIndex].Item1;
                        field.Update(Parameters.TimeoutMultiplier.ToString());
                    }
                }
            }
        }

        private List<byte> ConnectionPathToBytes(string connectionPath)
        {
            //TODO(gjc): need edit here

            List<byte> bytes = new List<byte>();

            if (string.IsNullOrEmpty(connectionPath))
                return bytes;

            if (string.Equals(connectionPath, "THIS"))
            {
                bytes.Add(31);
                bytes.Add(0);
                bytes.Add(0);
                bytes.Add(0);
                return bytes;
            }

            if (!connectionPath.Contains(",") && _controller != null)
            {
                var deviceModule = _controller.DeviceModules[connectionPath] as DeviceModule.DeviceModule;

                if (deviceModule != null)
                {
                    var port = deviceModule.GetFirstPort(PortType.Ethernet);
                    if (port != null)
                    {
                        connectionPath = $"2, {port.Address}";
                    }
                }
            }

            if (connectionPath.Contains(","))
            {
                var nodes = connectionPath.Split(',');
                if (nodes.Length == 2)
                {
                    try
                    {
                        byte pathType = byte.Parse(nodes[0].Trim());
                        if (pathType == 1)
                        {
                            bytes.Add(pathType);
                            bytes.Add(byte.Parse(nodes[1].Trim()));
                        }
                        else if (pathType == 2)
                        {
                            IPAddress ipAddress;
                            if (IPAddress.TryParse(nodes[1].Trim(), out ipAddress))
                            {
                                byte[] ipBytes = Encoding.Default.GetBytes(ipAddress.ToString());

                                bytes.Add(18);
                                bytes.Add((byte)ipBytes.Length);
                                bytes.AddRange(ipBytes);

                                if (ipBytes.Length % 2 == 1)
                                {
                                    bytes.Add(0);
                                }
                            }
                            else
                            {
                                bytes.Add(pathType);
                                bytes.Add(byte.Parse(nodes[1].Trim()));
                            }
                        }


                        return bytes;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }

            return bytes;
        }

        private Int16Field GetClassField()
        {
            CompositiveType compositiveType = DataTypeInfo.DataType as CompositiveType;
            ICompositeField compositeField = Data as ICompositeField;

            Contract.Assert(compositeField != null);
            Contract.Assert(compositiveType != null);

            var typeMember = compositiveType.TypeMembers["Class"] as DataTypeMember;
            Contract.Assert(typeMember != null);

            Int16Field field = compositeField.fields[typeMember.FieldIndex].Item1 as Int16Field;
            Contract.Assert(field != null);

            return field;
        }

        private Int32Field GetInstanceField()
        {
            CompositiveType compositiveType = DataTypeInfo.DataType as CompositiveType;
            ICompositeField compositeField = Data as ICompositeField;

            Contract.Assert(compositeField != null);
            Contract.Assert(compositiveType != null);

            var typeMember = compositiveType.TypeMembers["Instance"] as DataTypeMember;
            Contract.Assert(typeMember != null);

            Int32Field field = compositeField.fields[typeMember.FieldIndex].Item1 as Int32Field;
            Contract.Assert(field != null);

            return field;
        }

        private Int16Field GetAttributeField()
        {
            CompositiveType compositiveType = DataTypeInfo.DataType as CompositiveType;
            ICompositeField compositeField = Data as ICompositeField;

            Contract.Assert(compositeField != null);
            Contract.Assert(compositiveType != null);

            var typeMember = compositiveType.TypeMembers["Attribute"] as DataTypeMember;
            Contract.Assert(typeMember != null);

            Int16Field field = compositeField.fields[typeMember.FieldIndex].Item1 as Int16Field;
            Contract.Assert(field != null);

            return field;
        }

        private Int16Field GetSourceLengthField()
        {
            CompositiveType compositiveType = DataTypeInfo.DataType as CompositiveType;
            ICompositeField compositeField = Data as ICompositeField;

            Contract.Assert(compositeField != null);
            Contract.Assert(compositiveType != null);

            var typeMember = compositiveType.TypeMembers["REQ_LEN"] as DataTypeMember;
            Contract.Assert(typeMember != null);

            Int16Field field = compositeField.fields[typeMember.FieldIndex].Item1 as Int16Field;
            Contract.Assert(field != null);

            return field;
        }

        private STRINGField GetPathField()
        {
            CompositiveType compositiveType = DataTypeInfo.DataType as CompositiveType;
            ICompositeField compositeField = Data as ICompositeField;

            Contract.Assert(compositeField != null);
            Contract.Assert(compositiveType != null);

            var typeMember = compositiveType.TypeMembers["Path"] as DataTypeMember;
            Contract.Assert(typeMember != null);

            STRINGField field = compositeField.fields[typeMember.FieldIndex].Item1 as STRINGField;
            Contract.Assert(field != null);

            return field;
        }

        private Int32Field GetLocalIndexField()
        {
            CompositiveType compositiveType = DataTypeInfo.DataType as CompositiveType;
            ICompositeField compositeField = Data as ICompositeField;

            Contract.Assert(compositeField != null);
            Contract.Assert(compositiveType != null);

            var typeMember = compositiveType.TypeMembers["LocalIndex"] as DataTypeMember;
            Contract.Assert(typeMember != null);

            Int32Field field = compositeField.fields[typeMember.FieldIndex].Item1 as Int32Field;
            Contract.Assert(field != null);

            return field;
        }

        private bool GetTimedOut()
        {
            CompositiveType compositiveType = DataTypeInfo.DataType as CompositiveType;
            ICompositeField compositeField = Data as ICompositeField;

            Contract.Assert(compositeField != null);
            Contract.Assert(compositiveType != null);

            var typeMember = compositiveType.TypeMembers["TO"] as DataTypeMember;
            Contract.Assert(typeMember != null);
            Contract.Assert(typeMember.IsBit);

            var field = compositeField.fields[typeMember.FieldIndex].Item1;
            Contract.Assert(field != null);

            return field.GetBitValue(typeMember.BitOffset);
        }

        private void SetTimedOut(bool value)
        {
            CompositiveType compositiveType = DataTypeInfo.DataType as CompositiveType;
            ICompositeField compositeField = Data as ICompositeField;

            Contract.Assert(compositeField != null);
            Contract.Assert(compositiveType != null);

            var typeMember = compositiveType.TypeMembers["TO"] as DataTypeMember;
            Contract.Assert(typeMember != null);
            Contract.Assert(typeMember.IsBit);

            var field = compositeField.fields[typeMember.FieldIndex].Item1;
            Contract.Assert(field != null);

            field.SetBitValue(typeMember.BitOffset, value);
        }

        private bool GetCacheConnections()
        {
            CompositiveType compositiveType = DataTypeInfo.DataType as CompositiveType;
            ICompositeField compositeField = Data as ICompositeField;

            Contract.Assert(compositeField != null);
            Contract.Assert(compositiveType != null);

            var typeMember = compositiveType.TypeMembers["EN_CC"] as DataTypeMember;
            Contract.Assert(typeMember != null);
            Contract.Assert(typeMember.IsBit);

            var field = compositeField.fields[typeMember.FieldIndex].Item1;
            Contract.Assert(field != null);

            return field.GetBitValue(typeMember.BitOffset);
        }

        private void SetCacheConnections(bool value)
        {
            CompositiveType compositiveType = DataTypeInfo.DataType as CompositiveType;
            ICompositeField compositeField = Data as ICompositeField;

            Contract.Assert(compositeField != null);
            Contract.Assert(compositiveType != null);

            var typeMember = compositiveType.TypeMembers["EN_CC"] as DataTypeMember;
            Contract.Assert(typeMember != null);
            Contract.Assert(typeMember.IsBit);

            var field = compositeField.fields[typeMember.FieldIndex].Item1;
            Contract.Assert(field != null);

            field.SetBitValue(typeMember.BitOffset, value);
        }

        private Int32Field GetUnconnectedTimeoutField()
        {
            CompositiveType compositiveType = DataTypeInfo.DataType as CompositiveType;
            ICompositeField compositeField = Data as ICompositeField;

            Contract.Assert(compositeField != null);
            Contract.Assert(compositiveType != null);

            var typeMember = compositiveType.TypeMembers["UnconnectedTimeout"] as DataTypeMember;
            Contract.Assert(typeMember != null);

            Int32Field field = compositeField.fields[typeMember.FieldIndex].Item1 as Int32Field;
            Contract.Assert(field != null);

            return field;
        }

        private Int32Field GetConnectionRateField()
        {
            CompositiveType compositiveType = DataTypeInfo.DataType as CompositiveType;
            ICompositeField compositeField = Data as ICompositeField;

            Contract.Assert(compositeField != null);
            Contract.Assert(compositiveType != null);

            var typeMember = compositiveType.TypeMembers["ConnectionRate"] as DataTypeMember;
            Contract.Assert(typeMember != null);

            Int32Field field = compositeField.fields[typeMember.FieldIndex].Item1 as Int32Field;
            Contract.Assert(field != null);

            return field;
        }

        private Int8Field GetTimeoutMultiplierField()
        {
            CompositiveType compositiveType = DataTypeInfo.DataType as CompositiveType;
            ICompositeField compositeField = Data as ICompositeField;

            Contract.Assert(compositeField != null);
            Contract.Assert(compositiveType != null);

            var typeMember = compositiveType.TypeMembers["TimeoutMultiplier"] as DataTypeMember;
            Contract.Assert(typeMember != null);

            Int8Field field = compositeField.fields[typeMember.FieldIndex].Item1 as Int8Field;
            Contract.Assert(field != null);

            return field;
        }


        public JObject ToParametersObject()
        {
            JObject parametersObject = new JObject();

            // sync
            Parameters.RequestedLength = (ushort)SourceLength;
            Parameters.ConnectionPath = ConnectionPath;
            Parameters.ObjectType = ClassID;
            Parameters.TargetObject = InstanceID;
            Parameters.AttributeNumber = AttributeID;
            Parameters.LocalIndex = (uint)LocalIndex;

            Parameters.TimedOut = TimedOut;
            Parameters.CacheConnections = CacheConnections;
            Parameters.UnconnectedTimeout = UnconnectedTimeout;
            Parameters.ConnectionRate = ConnectionRate;
            Parameters.TimeoutMultiplier = TimeoutMultiplier;

            parametersObject.Add("MessageType", Parameters.MessageType);

            var messageType = (MessageTypeEnum)Parameters.MessageType;
            if (messageType == MessageTypeEnum.Unconfigured)
            {
                parametersObject.Add("RequestedLength", Parameters.RequestedLength);
                parametersObject.Add("CommTypeCode", Parameters.CommTypeCode);
                parametersObject.Add("LocalIndex", Parameters.LocalIndex);
            }
            else if (messageType == MessageTypeEnum.CIPGeneric)
            {
                parametersObject.Add("RequestedLength", Parameters.RequestedLength);
                parametersObject.Add("ConnectedFlag", Parameters.ConnectedFlag);
                parametersObject.Add("ConnectionPath", Parameters.ConnectionPath);
                parametersObject.Add("CommTypeCode", Parameters.CommTypeCode);
                parametersObject.Add("ServiceCode", Parameters.ServiceCode);
                parametersObject.Add("ObjectType", Parameters.ObjectType);
                parametersObject.Add("TargetObject", Parameters.TargetObject);
                parametersObject.Add("AttributeNumber", Parameters.AttributeNumber);
                parametersObject.Add("LocalIndex", Parameters.LocalIndex);

                if (Parameters.ConnectedFlag == 1 && Parameters.CacheConnections)
                    parametersObject.Add("CacheConnections", Parameters.CacheConnections);

                if (!string.IsNullOrEmpty(Parameters.LocalElement))
                    parametersObject.Add("LocalElement", Parameters.LocalElement);

                if (!string.IsNullOrEmpty(Parameters.DestinationTag))
                    parametersObject.Add("DestinationTag", Parameters.DestinationTag);

                parametersObject.Add("LargePacketUsage", Parameters.LargePacketUsage);

                //
                parametersObject.Add("TimedOut", Parameters.TimedOut);
                parametersObject.Add("UnconnectedTimeout", Parameters.UnconnectedTimeout);
                parametersObject.Add("ConnectionRate", Parameters.ConnectionRate);
                parametersObject.Add("TimeoutMultiplier", Parameters.TimeoutMultiplier);

            }

            return parametersObject;
        }
    }
}
