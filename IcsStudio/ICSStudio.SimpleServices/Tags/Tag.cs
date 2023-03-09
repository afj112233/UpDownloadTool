using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Json;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.Utils;
using ICSStudio.Utils.TagExpression;
using MessagePack;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using NLog;

namespace ICSStudio.SimpleServices.Tags
{
    [SuppressMessage("ReSharper", "UseNameofExpression")]
    public class Tag : BaseComponent, ITag
    {
        internal static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly TagCollection _parentCollection;
        private string _name;
        private DataWrapper.DataWrapper _dataWrapper;

        private ExternalAccess _externalAccess;
        private bool _isConstant;
        private Usage _usage;
        private DisplayStyle _displayStyle;
        private bool _isRequired;
        private bool _isVisible;
        private TagType _tagType;

        public override string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    CodeSynchronization.WaitReferenceRoutineCompile(this);

                    string oldName = _name;

                    OldName = oldName;

                    _name = value;

                    RaisePropertyChanged<string>(nameof(Name), oldName, _name);
                }
            }
        }

        public void UpdateConnectionEvent()
        {
            UpdateConnection?.Invoke(this, new EventArgs());
        }

        public event EventHandler UpdateConnection;

        public override string Description
        {
            get { return _description1; }
            set
            {
                if (_description1 != value)
                {
                    if (ChildDescription == null)
                    {
                        ChildDescription = new JArray();
                    }

                    var exist = ChildDescription.FirstOrDefault(c => "".Equals((string)c["Operand"]));
                    if (exist != null)
                    {
                        exist["Value"] = value;
                    }
                    else
                    {
                        var obj = new JObject();
                        obj["Operand"] = "";
                        obj["Value"] = value;
                        ChildDescription.Add(obj);
                    }

                    _description1 = value;
                    RaisePropertyChanged();
                }
            }
        }

        public Tag(TagCollection parentCollection)
        {
            _parentCollection = parentCollection;
            _usage = Usage.Local;
        }

        public void UpdateDataWrapper(DataWrapper.DataWrapper dataWrapper, DisplayStyle display)
        {
            _displayStyle = display;
            DataWrapper = dataWrapper;
        }

        public void Override(JObject config)
        {
            JObjectExtensions jsonTag = new JObjectExtensions(config);
            var dims = jsonTag["Dimensions"] as JArray;
            int dim1 = 0;
            int dim2 = 0;
            int dim3 = 0;
            if (dims != null)
            {
                switch (dims.Count)
                {
                    case 1:
                        dim1 = (int)dims[0];
                        break;
                    case 2:
                        dim2 = (int)dims[0];
                        dim1 = (int)dims[1];
                        break;
                    case 3:
                        dim3 = (int)dims[0];
                        dim2 = (int)dims[1];
                        dim1 = (int)dims[2];
                        break;
                }
            }

            if (dim1 == 0) Debug.Assert(dim2 == 0 && dim3 == 0);
            if (dim2 == 0) Debug.Assert(dim3 == 0);

            TagType tagType;
            if (EnumUtils.TryParse((string)jsonTag["TagType"], out tagType))
                TagType = tagType;

            //TODO(gjc,2022,12.03):special handle remove later
            //现场在线新建了consumed的tag
            if (tagType == TagType.Consumed)
            {
                tagType = TagType.Base;
                TagType = tagType;
            }
            //end special handle

            if (tagType == TagType.Base)
                IsVerified = true;

            if (tagType == TagType.Alias)
            {
                AliasSpecifier =
                    ((Controller)ParentController).GetFinalName(typeof(ITag), (string)jsonTag["AliasFor"]);
                IsVerified = false;
            }

            if (tagType == TagType.Produced)
            {
                if (jsonTag["PLCMappingFile"] != null)
                    PLCMappingFile = (int)jsonTag["PLCMappingFile"];
            }

            Usage usage;
            if (jsonTag["Usage"] != null && EnumUtils.TryParse((string)jsonTag["Usage"], out usage))
                Usage = usage;
            DisplayStyle = DisplayStyle.NullStyle;
            if (jsonTag["Radix"] != null)
            {
                DisplayStyle displayStyle;
                if (EnumUtils.TryParse((string)jsonTag["Radix"], out displayStyle))
                    DisplayStyle = displayStyle;
            }

            bool isConstant;
            if (bool.TryParse((string)jsonTag["Constant"], out isConstant))
                IsConstant = isConstant;

            ExternalAccess externalAccess;
            if (EnumUtils.TryParse((string)jsonTag["ExternalAccess"], out externalAccess))
                ExternalAccess = externalAccess;

            Description = (string)jsonTag["Description"];

            ChildDescription = jsonTag["Comments"] as JArray ?? new JArray();

            DataWrapper.DataWrapper dataWrapper = null;
            if (TagType == TagType.Base || TagType == TagType.Produced)
            {
                var dataTypeName = (string)jsonTag["DataType"];
                dataTypeName =
                    ((Controller)_parentCollection.ParentController).GetFinalName(typeof(IDataType), dataTypeName);
                var dataType = ParentCollection.ParentController.DataTypes[dataTypeName];
                Debug.Assert(dataType != null, dataTypeName);

                if (dataType is MOTION_GROUP)
                {
                    dataWrapper = MotionGroup.Create(dataType);
                    MotionGroup motionGroup = (MotionGroup)dataWrapper;
                    motionGroup.Alternate1UpdateMultiplier = (int)jsonTag["Alternate1UpdateMultiplier"];
                    motionGroup.Alternate2UpdateMultiplier = (int)jsonTag["Alternate2UpdateMultiplier"];
                    motionGroup.AutoTagUpdate = (bool)jsonTag["AutoTagUpdate"];
                    motionGroup.CoarseUpdatePeriod = (int)jsonTag["CoarseUpdatePeriod"];

                    GeneralFaultType generalFaultType;
                    if (EnumUtils.TryParse((string)jsonTag["GeneralFaultType"], out generalFaultType))
                        motionGroup.GeneralFaultType = generalFaultType;

                    if (jsonTag["GroupType"] != null)
                    {
                        GroupType groupType;
                        if (EnumUtils.TryParse((string)jsonTag["GroupType"], out groupType))
                            motionGroup.GroupType = groupType;
                    }

                    motionGroup.PhaseShift = (int)jsonTag["PhaseShift"];
                }
                else if (dataType is AXIS_CIP_DRIVE)
                {
                    dataWrapper = AxisCIPDrive.Create(dataType, ParentCollection.ParentController);
                    AxisCIPDrive axisCIPDrive = (AxisCIPDrive)dataWrapper;
                    Debug.Assert(axisCIPDrive != null);
                    axisCIPDrive.Parameters = jsonTag["Parameters"].ToObject<AxisCIPDriveParameters>();
                }
                else if (dataType is AXIS_VIRTUAL)
                {
                    dataWrapper = AxisVirtual.Create(dataType, ParentCollection.ParentController);
                    AxisVirtual axisVirtual = (AxisVirtual)dataWrapper;
                    Debug.Assert(axisVirtual != null);
                    axisVirtual.Parameters = jsonTag["Parameters"].ToObject<AxisVirtualParameters>();
                }
                else if (dataType is MESSAGE)
                {
                    dataWrapper = MessageDataWrapper.Create(dataType, ParentCollection.ParentController);
                    MessageDataWrapper messageDataWrapper = (MessageDataWrapper)dataWrapper;
                    Contract.Assert(messageDataWrapper != null);

                    try
                    {
                        messageDataWrapper.Parameters = jsonTag["Parameters"].ToObject<MessageParameters>();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }

                    //use parameters, ignore data,
                    messageDataWrapper.ParametersToMembers();
                }
                else
                {
                    try
                    {
                        JToken data = jsonTag["Data"];

                        if (data != null && data.Type == JTokenType.String)
                        {
                            var bytes = Convert.FromBase64String((string)jsonTag["Data"]);

                            string json = MessagePackSerializer.ToJson(bytes);

                            data = JToken.Parse(json);
                        }


                        dataWrapper = new DataWrapper.DataWrapper(dataType,
                            dim1, dim2, dim3, data);
                    }
                    catch (Exception e)
                    {
                        Debug.Assert(true, e.Message);
                        dataWrapper = new DataWrapper.DataWrapper(dataType, dim1, dim2, dim3, null);
                    }

                }
            }

            DataWrapper = dataWrapper;
        }

        public JArray ChildDescription { set; get; } = new JArray();

        public static string GetChildDescription(string tagDescription, DataTypeInfo dataTypeInfo,
            JArray descriptionField, string operand, bool needTopDesc = true)
        {
            try
            {
                if (operand == null) return "";
                if (operand == "")
                {
                    if (string.IsNullOrEmpty(tagDescription))
                    {
                        var desc = descriptionField?.FirstOrDefault(d => "" == (string)d?["Operand"])?["Value"]
                            ?.ToString();
                        if (string.IsNullOrEmpty(desc))
                            return dataTypeInfo.DataType?.Description;
                        return desc;
                    }

                    return tagDescription;
                }

                //if (descriptionField == null) tag.ChildDescription = new JArray();
                var description = descriptionField?.FirstOrDefault(d =>
                        ((JObject)d)["Operand"]?.ToString().Equals(operand, StringComparison.OrdinalIgnoreCase) ??
                        false)?[
                        "Value"]
                    ?.ToString();
                if (string.IsNullOrEmpty(description))
                {
                    var topDesc = GetTopDescription(tagDescription, dataTypeInfo, descriptionField, operand);

                    if (operand.IndexOf(".", StringComparison.Ordinal) < 0) return topDesc;
                    return string.IsNullOrEmpty(topDesc)
                        ? (dataTypeInfo.DataType as CompositiveType)?.GetDescription(operand) ?? ""
                        : $"{(needTopDesc ? topDesc + " " : "")}{(dataTypeInfo.DataType as CompositiveType)?.GetDescription(operand) ?? ""}";
                }

                return description;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return "";
        }

        private static string GetTopDescription(string tagDescription, DataTypeInfo dataTypeInfo,
            JArray descriptionField, string operand)
        {
            var parentLevel = new List<string>();
            var p = operand;
            while (!string.IsNullOrEmpty(p))
            {
                p = GetParentOperand(p);
                parentLevel.Add(p);
            }

            for (int i = parentLevel.Count - 1; i >= 0; i--)
            {
                var name = parentLevel[i];
                var desc = GetChildDescription(tagDescription, dataTypeInfo, descriptionField, name, false);
                if (!string.IsNullOrEmpty(desc)) return desc;
            }

            return "";
        }

        private static string GetParentOperand(string operand)
        {
            var lastDot = operand.LastIndexOf(".");
            if (lastDot > -1)
            {
                var tail = operand.Substring(lastDot);
                if (tail.Contains("]"))
                {
                    var rCount = tail.Count(c => c == ']');
                    var lCount = tail.Count(c => c == '[');
                    var d = rCount - lCount;
                    if (d > 0)
                    {
                        operand = RemoveDim(operand, d);
                        Console.WriteLine(operand);
                        return operand;
                    }
                }

                operand = operand.Substring(0, lastDot);
            }
            else
            {
                return "";
            }

            Console.WriteLine(operand);
            return operand;
        }

        private static string RemoveDim(string operand, int count)
        {
            while (count > 0)
            {
                var lastLBracket = operand.LastIndexOf('[');
                operand = operand.Substring(0, lastLBracket);
                count--;
            }

            return operand;
        }

        public void SetChildDescription(string operand, string description)
        {
            if (ChildDescription == null) ChildDescription = new JArray();

            if (operand == "")
            {
                Description = description;
                RaisePropertyChanged("Description");
                return;
            }

            var td = ChildDescription.FirstOrDefault(d =>
                ((JObject)d)["Operand"].ToString().Equals(operand, StringComparison.OrdinalIgnoreCase));
            if (td != null)
            {
                if (string.IsNullOrEmpty(description))
                {
                    td.Remove();
                    RaisePropertyChanged("Description");
                    return;
                }

                td["Value"] = description;
            }
            else
            {
                if (string.IsNullOrEmpty(description)) return;
                td = new JObject { ["Operand"] = operand, ["Value"] = description };
                ChildDescription.Add(td);
            }

            RaisePropertyChanged("Description");
        }

        public static string GetOperand(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            var operand = name;
            var baseName = name;
            int dotIndex = baseName?.IndexOf(".") ?? -1;
            if (dotIndex > -1)
            {
                // ReSharper disable once PossibleNullReferenceException
                baseName = baseName.Substring(0, dotIndex);
            }

            int pIndex = baseName?.IndexOf("[") ?? -1;
            if (pIndex > -1)
            {
                baseName = baseName.Substring(0, pIndex);
            }

            operand = operand.Substring(baseName.Length);
            if (operand == ".") return "";
            return operand;
        }

        public override IController ParentController => _parentCollection?.ParentController;

        public ITagCollection ParentCollection => _parentCollection;

        public DataWrapper.DataWrapper DataWrapper
        {
            get { return _dataWrapper; }
            set
            {
                var oldDataWrapper = _dataWrapper;

                if (_dataWrapper == value)
                    return;

                _dataWrapper = value;

                OnDataWrapperChanged(_dataWrapper, oldDataWrapper);

                RaisePropertyChanged(nameof(DataWrapper), oldDataWrapper, _dataWrapper);
            }
        }

        public string GetConnectionsInfo(string special)
        {
            int member = 0;
            int subMember = 0;
            foreach (var connection in ParentController.ParameterConnections.GetTagParameterConnections(this))
            {
                if (connection.SourcePath.StartsWith(special, StringComparison.OrdinalIgnoreCase))
                {
                    if (connection.SourcePath.Length > special.Length)
                    {
                        subMember++;
                    }
                    else
                    {
                        member++;
                    }
                }

                if (connection.DestinationPath.StartsWith(special, StringComparison.OrdinalIgnoreCase))
                {
                    if (connection.DestinationPath.Length > special.Length)
                    {
                        subMember++;
                    }
                    else
                    {
                        member++;
                    }
                }
            }

            return "{" + member + ":" + subMember + "} Connections";
        }

        private void OnDataWrapperChanged(DataWrapper.DataWrapper newData, DataWrapper.DataWrapper oldData)
        {
            if (oldData != null)
            {
                if (oldData.DataTypeInfo.DataType is UserDefinedDataType ||
                    oldData.DataTypeInfo.DataType is AOIDataType)
                {
                    PropertyChangedEventManager.RemoveHandler(oldData.DataTypeInfo.DataType,
                        UserDefined_PropertyChanged, "");
                }

                oldData.ParentTag = null;
            }

            if (newData != null)
            {
                if (newData.DataTypeInfo.DataType is UserDefinedDataType ||
                    newData.DataTypeInfo.DataType is AOIDataType)
                {
                    PropertyChangedEventManager.AddHandler(newData.DataTypeInfo.DataType, UserDefined_PropertyChanged,
                        "");
                }

                newData.ParentTag = this;
            }

            if (newData?.DataTypeInfo != oldData?.DataTypeInfo)
                DataHelper.Copy(newData, oldData);
        }

        public DataTypeInfo DataTypeInfo
        {
            get
            {
                if (DataWrapper != null)
                {
                    return DataWrapper.DataTypeInfo;
                }

                return new DataTypeInfo();
            }
            set
            {
                if (DataWrapper.DataTypeInfo != value)
                {
                    DataWrapper = new DataWrapper.DataWrapper(value.DataType, value.Dim1, value.Dim2, value.Dim3, null);
                }
            }
        }

        public int TargetUid => Uid;
        public string AliasSpecifier { get; set; }

        public bool IsAlias => TagType == TagType.Alias;

        public bool IsConsuming { get; set; }
        public bool IsProducing { get; set; }
        public bool IsProgramTag { get; set; }
        public bool IsUDITag { get; set; }
        public string AliasBaseSpecifier { get; private set; }

        private ITag _baseTag;

        public ITag BaseTag
        {
            get
            {
                if (_baseTag == null && TagType == TagType.Base)
                {
                    _baseTag = this;
                }

                return _baseTag;
            }
            private set { _baseTag = value; }
        }

        public DisplayStyle DisplayStyle
        {
            get
            {
                if (_displayStyle == DisplayStyle.NullStyle)
                {
                    if (DataTypeInfo.DataType.IsInteger || DataTypeInfo.DataType.IsBool)
                    {
                        return DisplayStyle.Decimal;
                    }
                    else if (DataTypeInfo.DataType.IsReal)
                    {
                        return DisplayStyle.Float;
                    }
                }

                return _displayStyle;
            }
            set
            {
                if (_displayStyle != value)
                {
                    _displayStyle = value;
                    RaisePropertyChanged();

                    Notifications.SendNotificationData(new TagNotificationData()
                    {
                        Tag = this,
                        Type = TagNotificationData.NotificationType.Attribute,
                        Attribute = nameof(DisplayStyle)
                    });
                }
            }
        }

        public Usage Usage
        {
            get { return _usage; }
            set
            {
                if (_usage != value)
                {
                    var old = _usage;
                    _usage = value;
                    RaisePropertyChanged(nameof(Usage), old, _usage);
                }
            }
        }

        public ExternalAccess ExternalAccess
        {
            get { return _externalAccess; }
            set
            {
                if (_externalAccess != value)
                {
                    _externalAccess = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsConstant
        {
            get { return _isConstant; }
            set
            {
                if (_isConstant != value)
                {
                    var old = _isConstant;
                    _isConstant = value;
                    RaisePropertyChanged(nameof(IsConstant), old, _isConstant);
                }
            }
        }

        public bool IsSequencing { get; set; }
        public bool IsHidden { get; set; }

        public bool IsRequired
        {
            get { return _isRequired; }
            set
            {
                if (_isRequired != value)
                {
                    _isRequired = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string FullName { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsStorage { get; set; }

        public bool IsProgramParameter
        {
            get { return Usage != Usage.Local; }
        }

        public bool IsControllerScoped
        {
            get
            {
                if (ParentController == null)
                    return false;

                if (ParentController.Tags == ParentCollection)
                    return true;

                return false;
            }
        }

        public TagType TagType
        {
            get { return _tagType; }
            set
            {
                _tagType = value;
                RaisePropertyChanged();
            }
        }

        public bool IsModuleTag { get; set; }

        public override bool IsDeleted
        {
            get
            {
                if (_parentCollection == null)
                    return true;

                if (!_parentCollection.Contains(this))
                    return true;

                if (_parentCollection.ParentProgram == null && _parentCollection.ParentController == null)
                    return true;

                if (_parentCollection.ParentProgram != null && _parentCollection.ParentProgram.IsDeleted)
                    return true;

                return false;
            }
        }

        public byte[] GetByteValue(int byteOffset, int byteLength)
        {
            throw new NotImplementedException();
        }

        public void SetByteValue(int byteOffset, int byteLength, byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public void SetStringValue(string specifier, string value)
        {
            CompositiveType compositiveType = DataTypeInfo.DataType as CompositiveType;
            var typeMember = compositiveType?.TypeMembers[specifier] as DataTypeMember;

            ICompositeField compositeField = DataWrapper.Data as ICompositeField;

            if (typeMember != null && compositeField != null)
            {
                IField field = compositeField.fields[typeMember.FieldIndex].Item1;
                bool isBit = false;
                int bitOffset = 0;

                if (typeMember.IsBit && typeMember.Dim1 == 0)
                {
                    isBit = true;
                    bitOffset = typeMember.BitOffset;
                    field.SetBitValue(typeMember.BitOffset, bool.Parse(value));
                }
                else
                {
                    field.Update(value);
                }

                RaisePropertyChanged($"{Name}.{specifier}");

                Notifications.SendNotificationData(new TagNotificationData()
                {
                    Tag = this,
                    Type = TagNotificationData.NotificationType.Value,
                    Field = field,
                    IsBit = isBit,
                    BitOffset = bitOffset
                });
            }
        }

        public bool GetBitValue(int bitOffset)
        {
            throw new NotImplementedException();
        }

        public void SetBitValue(int bitOffset, bool value)
        {
            throw new NotImplementedException();
        }

        public string GetMemberValue(string member, bool allowPrivateMemberReferences)
        {
            CompositiveType compositiveType = DataTypeInfo.DataType as CompositiveType;
            var typeMember = compositiveType?.TypeMembers[member] as DataTypeMember;

            ICompositeField compositeField = DataWrapper.Data as ICompositeField;

            if (typeMember != null && compositeField != null)
            {
                if (typeMember.IsBit)
                {
                    IField field = compositeField.fields[typeMember.FieldIndex].Item1;
                    return field.GetBitValue(typeMember.BitOffset).ToString();
                }
                else
                {
                    IField field = compositeField.fields[typeMember.FieldIndex].Item1;
                    return field.GetString();
                }

            }

            return string.Empty;
        }

        public bool IsDataTruncated(int uid, int dim1, int dim2, int dim3)
        {
            throw new NotImplementedException();
        }

        public string GetQualifierDescription(string qualifier, ref bool passThrough,
            bool allowPrivateMemberReferences = false)
        {
            throw new NotImplementedException();
        }

        public void SetQualifierDescription(string qualifier, string description)
        {
            throw new NotImplementedException();
        }

        public string GetMetadataValueString(MetadataDefinitionID metadataID, string qualifier, ref bool passThrough,
            bool allowPrivateMemberReferences = false)
        {
            throw new NotImplementedException();
        }

        public void SetMetadataValueString(MetadataDefinitionID metadataID, string qualifier, string description)
        {
            throw new NotImplementedException();
        }

        public uint TrackingGroups { get; set; }
        public int PLCMappingFile { get; set; }

        public JObject ConvertToJObject()
        {
            try
            {
                if (string.IsNullOrEmpty(DataTypeInfo.DataType?.Name))
                {
                    Logger.Error($"{Name} dataType is null!!!");
                }

                var tag = new JObject
                {
                    { "DataType", DataTypeInfo.DataType?.Name },
                    { "Name", Name },
                    { "TagType", (int)TagType },
                    { "Radix", (int)DisplayStyle },
                    { "Constant", IsConstant },
                    { "ExternalAccess", (int)ExternalAccess },
                    { "Usage", (int)Usage }
                };

                if (TagType == TagType.Produced)
                {
                    tag.Add("PLCMappingFile", PLCMappingFile);
                }
                else if (TagType == TagType.Alias)
                {
                    tag.Add("AliasFor", AliasSpecifier);
                }

                if (!string.IsNullOrEmpty(Description))
                {
                    tag.Add("Description", Description);
                }

                if (ChildDescription != null && ChildDescription.Count > 0)
                {
                    tag.Add("Comments", ChildDescription);
                }

                if (DataTypeInfo.Dim1 != 0)
                {
                    var array = new JArray();

                    if (DataTypeInfo.Dim3 != 0)
                    {
                        Contract.Assert(DataTypeInfo.Dim2 != 0);

                        array.Add(DataTypeInfo.Dim3);
                        array.Add(DataTypeInfo.Dim2);
                        array.Add(DataTypeInfo.Dim1);
                    }
                    else if (DataTypeInfo.Dim2 != 0)
                    {
                        array.Add(DataTypeInfo.Dim2);
                        array.Add(DataTypeInfo.Dim1);
                    }
                    else
                    {
                        array.Add(DataTypeInfo.Dim1);
                    }

                    tag["Dimensions"] = array;
                }

                // MotionGroup
                {
                    MotionGroup motionGroup = DataWrapper as MotionGroup;
                    if (motionGroup != null)
                    {
                        tag.Add("Alternate1UpdateMultiplier", motionGroup.Alternate1UpdateMultiplier);
                        tag.Add("Alternate2UpdateMultiplier", motionGroup.Alternate2UpdateMultiplier);
                        tag.Add("AutoTagUpdate", motionGroup.AutoTagUpdate);
                        tag.Add("CoarseUpdatePeriod", motionGroup.CoarseUpdatePeriod);
                        tag.Add("GeneralFaultType",
                            JsonConvert.SerializeObject(motionGroup.GeneralFaultType, new StringEnumConverter())
                                .Replace("\"", ""));
                        tag.Add("GroupType",
                            JsonConvert.SerializeObject(motionGroup.GroupType, new StringEnumConverter())
                                .Replace("\"", ""));
                        tag.Add("PhaseShift", motionGroup.PhaseShift);
                    }
                }

                var serializer = JsonSerializer.Create(
                    new JsonSerializerSettings
                    {
                        ContractResolver = new ShouldSerializeContractResolver()

                    });
                serializer.NullValueHandling = NullValueHandling.Ignore;

                {
                    AxisCIPDrive axisCIPDrive = DataWrapper as AxisCIPDrive;
                    if (axisCIPDrive != null)
                    {
                        if (axisCIPDrive.Parameters != null)
                            tag.Add("Parameters", JToken.FromObject(axisCIPDrive.Parameters, serializer));
                        else
                        {
                            Logger.Error($"{Name}.Parameters is null!!!");
                        }
                    }
                }

                {
                    AxisVirtual axisVirtual = DataWrapper as AxisVirtual;
                    if (axisVirtual != null)
                    {
                        if (axisVirtual.Parameters != null)
                            tag.Add("Parameters", JToken.FromObject(axisVirtual.Parameters, serializer));
                        else
                        {
                            Logger.Error($"{Name}.Parameters is null!!!");
                        }
                    }
                }

                {
                    MessageDataWrapper messageDataWrapper = DataWrapper as MessageDataWrapper;
                    if (messageDataWrapper != null)
                    {
                        // use Parameters, not data
                        tag.Add("Parameters", messageDataWrapper.ToParametersObject());
                        return tag;
                    }
                }

                var binData = new List<byte>();

                if (DataWrapper?.Data != null)
                    DataWrapper.Data.ToMsgPack(binData);
                else
                {
                    Logger.Error($"{Name}.DataWrapper.Data is null!");
                }

                var data = System.Convert.ToBase64String(binData.ToArray());
                if (data != null)
                    tag.Add("Data", data);

                return tag;
            }
            catch (Exception e)
            {
                Logger.Error($"Tag convert failed:{e}");

                throw;
            }

        }

        public void Verify()
        {
            if (TagType == TagType.Base)
                IsVerified = true;

            if (TagType == TagType.Alias)
            {
                IsVerified = false;

                TagExpressionParser parser = new TagExpressionParser();

                var tagExpression = parser.Parser(AliasSpecifier);

                SimpleTagExpression simpleTagExpression = parser.GetSimpleTagExpression(tagExpression);

                Tag refTag;
                if (!string.IsNullOrEmpty(simpleTagExpression.Scope))
                {
                    IProgram program = ParentController.Programs[simpleTagExpression.Scope];
                    if (program == null)
                        return;

                    refTag = program.Tags[simpleTagExpression.TagName] as Tag;
                }
                else
                {
                    refTag = ParentCollection[simpleTagExpression.TagName] as Tag ??
                             ParentController.Tags[simpleTagExpression.TagName] as Tag;
                }

                if (refTag == null)
                    return;

                if (!refTag.IsVerified)
                    refTag.Verify();

                if (!refTag.IsVerified)
                    return;

                // DataWrapper
                var dataWrapper = CreateDataWrapper(refTag, simpleTagExpression);
                if (dataWrapper == null)
                    return;

                DataWrapper = dataWrapper;

                // AliasBaseSpecifier
                var aliasBaseSpecifier = GetAliasBaseSpecifier(refTag, simpleTagExpression);

                AliasBaseSpecifier = aliasBaseSpecifier;

                var baseTag = GetBaseTag(refTag);
                BaseTag = baseTag;


                IsVerified = true;
            }

        }

        private string GetAliasBaseSpecifier(Tag refTag, SimpleTagExpression simpleTagExpression)
        {
            if (refTag.IsAlias)
            {
                string refAliasBaseSpecifier = refTag.AliasBaseSpecifier;

                TagExpressionParser parser = new TagExpressionParser();
                return refAliasBaseSpecifier + parser.GetNextTagExpression(simpleTagExpression);

            }

            return AliasSpecifier;
        }

        private ITag GetBaseTag(Tag refTag)
        {
            if (refTag.IsAlias)
                return refTag.BaseTag;

            return refTag;
        }

        private DataWrapper.DataWrapper CreateDataWrapper(Tag refTag, SimpleTagExpression simpleTagExpression)
        {
            if (simpleTagExpression.Next == null)
            {
                return refTag.DataWrapper;
            }

            DataTypeInfo dataTypeInfo = refTag.DataTypeInfo;
            IField field = refTag.DataWrapper.Data;

            TagExpressionBase expression = simpleTagExpression;

            while (expression.Next != null)
            {
                expression = expression.Next;

                int bitMemberNumber = -1;
                var bitMemberNumberAccessExpression = expression as BitMemberNumberAccessExpression;
                var bitMemberExpressionAccessExpression = expression as BitMemberExpressionAccessExpression;

                if (bitMemberNumberAccessExpression != null)
                {
                    bitMemberNumber = bitMemberNumberAccessExpression.Number;
                }
                else if (bitMemberExpressionAccessExpression != null)
                {
                    if (bitMemberExpressionAccessExpression.Number.HasValue)
                        bitMemberNumber = bitMemberExpressionAccessExpression.Number.Value;
                    else
                        return null;

                }

                if (bitMemberNumber >= 0)
                {
                    if (dataTypeInfo.DataType.IsInteger && dataTypeInfo.Dim1 == 0)
                    {
                        //TODO(gjc): add code here
                    }
                    else
                    {
                        return null;
                    }


                    break;
                }

                var memberAccessExpression = expression as MemberAccessExpression;
                if (memberAccessExpression != null)
                {
                    var compositeField = field as ICompositeField;
                    var compositiveType = dataTypeInfo.DataType as CompositiveType;
                    if (compositeField != null && compositiveType != null && dataTypeInfo.Dim1 == 0)
                    {
                        var dataTypeMember = compositiveType.TypeMembers[memberAccessExpression.Name] as DataTypeMember;
                        if (dataTypeMember == null)
                            return null;

                        field = compositeField.fields[dataTypeMember.FieldIndex].Item1;
                        dataTypeInfo = dataTypeMember.DataTypeInfo;
                    }
                    else
                    {
                        return null;
                    }

                }

                var elementAccessExpression = expression as ElementAccessExpression;
                if (elementAccessExpression != null)
                {
                    ArrayField arrayField = field as ArrayField;
                    BoolArrayField boolArrayField = field as BoolArrayField;


                    int index;
                    switch (elementAccessExpression.Indexes.Count)
                    {
                        case 1:
                            if (dataTypeInfo.Dim1 > 0 && dataTypeInfo.Dim2 == 0 && dataTypeInfo.Dim3 == 0)
                            {
                                index = elementAccessExpression.Indexes[0];
                                break;
                            }
                            else
                                return null;
                        case 2:
                            if (dataTypeInfo.Dim1 > 0 && dataTypeInfo.Dim2 > 0 && dataTypeInfo.Dim3 == 0)
                            {
                                index = elementAccessExpression.Indexes[0] * dataTypeInfo.Dim1 +
                                        elementAccessExpression.Indexes[1];
                                break;
                            }
                            else
                                return null;
                        case 3:
                            if (dataTypeInfo.Dim1 > 0 && dataTypeInfo.Dim2 > 0 && dataTypeInfo.Dim3 > 0)
                            {
                                index = elementAccessExpression.Indexes[0] * dataTypeInfo.Dim2 * dataTypeInfo.Dim1 +
                                        elementAccessExpression.Indexes[1] * dataTypeInfo.Dim1 +
                                        elementAccessExpression.Indexes[2];
                                break;
                            }
                            else
                                return null;
                        default:
                            throw new NotImplementedException();
                    }

                    if (dataTypeInfo.Dim1 > 0 && arrayField != null)
                    {
                        dataTypeInfo = new DataTypeInfo { DataType = dataTypeInfo.DataType };
                        field = arrayField.fields[index].Item1;
                    }
                    else if (dataTypeInfo.Dim1 > 0 && boolArrayField != null)
                    {
                        //TODO(gjc): add code here
                        return null;
                    }
                    else
                    {
                        return null;
                    }

                }

            }

            return new DataWrapper.DataWrapper(dataTypeInfo, field);
        }

        private bool _hasUpdateData;
        private string _description1;

        private void UserDefined_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                RaisePropertyChanged("DataType.Name");
                return;
            }

            if (e.PropertyName == "Description")
            {
                RaisePropertyChanged("Description");
                return;
            }

            if (e.PropertyName == "RequestTagUpdateData")
            {
                if (((AssetDefinedDataType)sender).RequestTagUpdateData)
                {
                    if (!_hasUpdateData)
                    {
                        _hasUpdateData = true;
                        RaisePropertyChanged("Data");
                    }
                }
                else
                {
                    _hasUpdateData = false;
                }

                return;
            }

            if (e.PropertyName == "ByteSize")
            {
                var userDefined = DataTypeInfo.DataType as UserDefinedDataType;

                var oldDataWrapper = _dataWrapper;
                var newDataWrapper =
                    new DataWrapper.DataWrapper(
                        DataTypeInfo.DataType,
                        DataTypeInfo.Dim1, DataTypeInfo.Dim2, DataTypeInfo.Dim3, null);

                //if (isStringType)
                //    DataHelper.CopyStringDataWrapper(newDataWrapper, oldDataWrapper);
                //else
                //    DataHelper.CopyUdtDataWrapper(newDataWrapper, oldDataWrapper);

                _dataWrapper = newDataWrapper;

                //////
                if (userDefined != null)
                {
                    if (userDefined.MemberChangedList.Count > 0)
                        UpdateMemberNameInCode(userDefined.MemberChangedList);
                }

                var aoiDataType = DataTypeInfo.DataType as AOIDataType;
                if (aoiDataType != null)
                {
                    if (aoiDataType.MemberChangedList.Count > 0)
                        UpdateMemberNameInCode(aoiDataType.MemberChangedList);
                }

                RaisePropertyChanged(nameof(DataWrapper), oldDataWrapper, newDataWrapper);
            }

        }

        private void UpdateMemberNameInCode(List<Tuple<string, string>> memberChangedList)
        {
            var codeSynchronization = CodeSynchronization.GetInstance();
            var aoi = ParentCollection.ParentProgram as AoiDefinition;
            if (aoi != null)
            {
                foreach (var routine in aoi.Routines)
                {
                    var st = routine as STRoutine;
                    if (st != null)
                    {
                        foreach (var tuple in memberChangedList)
                        {
                            var oldName = DataTypeInfo.Dim1 > 0
                                ? $"{Name}( \\[.*?\\])?\\.{tuple.Item1}"
                                : $"{Name}\\.{tuple.Item1}";
                            string programName = ParentCollection.ParentProgram?.Name ?? "";
                            Regex regex = new Regex($@"\b(?<!\.){oldName}(?![\s\w\(])", RegexOptions.IgnoreCase);
                            codeSynchronization.Add(st,
                                new UpdateCodeParameter(programName, tuple.Item1, tuple.Item2, regex.ToString(), Name));
                        }

                    }

                    //TODO(ZYL):add other
                }
            }
            else
            {
                foreach (var tuple in memberChangedList)
                {
                    var oldName = DataTypeInfo.Dim1 > 0
                        ? $"{Name}( \\[.*?\\])?\\.{tuple.Item1}"
                        : $"{Name}\\.{tuple.Item1}";
                    Regex regex = ParentCollection.ParentProgram == null
                        ? new Regex($@"\b(?<!\.){oldName}(?![\s\w\(])", RegexOptions.IgnoreCase)
                        : new Regex($@"(\\\b{ParentCollection.ParentProgram.Name}\.|\b(?<!\.)){oldName}(?![\s\w\(])",
                            RegexOptions.IgnoreCase);
                    string unChangedName = tuple.Item1, changedName = tuple.Item2;
                    if (unChangedName.EndsWith("(\\[.*?\\])?"))
                        unChangedName = unChangedName.Replace("(\\[.*?\\])?", "");
                    if (changedName.EndsWith("(\\[.*?\\])?"))
                        changedName = changedName.Replace("(\\[.*?\\])?", "");
                    UpdateMemberNameInTrend(regex, unChangedName, changedName);
                    UpdateMemberNameInMessage(regex, unChangedName, changedName);
                }

                foreach (var program in ParentController.Programs)
                {
                    foreach (var routine in program.Routines)
                    {
                        var st = routine as STRoutine;
                        if (st != null)
                        {
                            //update original
                            {
                                if (st.GetAllReferenceTags().Any(t => t == this))

                                    foreach (var tuple in memberChangedList)
                                    {
                                        var oldName = DataTypeInfo.Dim1 > 0
                                            ? $"{Name}( \\[.*?\\])?\\.{tuple.Item1}"
                                            : $"{Name}\\.{tuple.Item1}";

                                        string programName = ParentCollection.ParentProgram?.Name ?? "";
                                        Regex regex = string.IsNullOrEmpty(programName)
                                            ? new Regex($@"\b(?<!\.){oldName}(?![\s\w\(])", RegexOptions.IgnoreCase)
                                            : new Regex($@"(\\\b{programName}\.|\b(?<!\.)){oldName}(?![\s\w\(])",
                                                RegexOptions.IgnoreCase);
                                        codeSynchronization.Add(st,
                                            new UpdateCodeParameter(programName, tuple.Item1, tuple.Item2,
                                                regex.ToString(), Name));

                                    }
                            }
                        }

                        //TODO(ZYL):add other
                    }
                }
            }
        }

        private void UpdateMemberNameInTrend(Regex regex, string unchangedName,
            string changedName)
        {

            foreach (TrendObject trend in Controller.GetInstance().Trends)
            {
                trend.AxisPenName = UpdateSpecifierName(regex, unchangedName, changedName, trend.AxisPenName);

                foreach (PenObject pen in trend.Pens)
                {
                    pen.Name = UpdateSpecifierName(regex, unchangedName, changedName, pen.Name);
                }
            }
        }

        private string UpdateSpecifierName(Regex regex, string unchangedName,
            string changedName, string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            if (regex.IsMatch(name))
            {
                var memberRegex = new Regex(@"\." + unchangedName + @"(\[.*?\])?(?![\s\w\(])");
                var match = memberRegex.Matches(name);
                if (match.Count > 0)
                {
                    var lastMatch = match[match.Count - 1];
                    var index = lastMatch.Index + 1;
                    name = name.Remove(index, unchangedName.Length);
                    name = name.Insert(index, changedName);
                }
            }

            return name;
        }

        private void UpdateMemberNameInMessage(Regex regex, string unchangedName,
            string changedName)
        {
            if (ParentCollection.ParentProgram != null) return;
            foreach (var tag in ParentCollection.Where(t => ((Tag)t).DataWrapper is MessageDataWrapper))
            {
                var message = (MessageDataWrapper)((Tag)tag).DataWrapper;
                message.SourceElement = UpdateSpecifierName(regex, unchangedName, changedName, message.SourceElement);
                message.DestinationElement =
                    UpdateSpecifierName(regex, unchangedName, changedName, message.DestinationElement);
            }
        }

        protected override void DisposeAction()
        {
            if (DataTypeInfo.DataType is UserDefinedDataType || DataTypeInfo.DataType is AOIDataType)
            {
                PropertyChangedEventManager.RemoveHandler(DataTypeInfo.DataType,
                    UserDefined_PropertyChanged, "");
            }

            _dataWrapper = null;
            _description1 = null;

            ChildDescription = null;
        }
    }
}
