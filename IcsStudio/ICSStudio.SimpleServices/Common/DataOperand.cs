using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.Utils.TagExpression;

namespace ICSStudio.SimpleServices.Common
{
    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("ReSharper", "MergeCastWithTypeCheck")]
    internal class DataOperand : IDataOperand
    {
        private readonly DataServer _dataServer;

        private string _operand;
        private ITagCollection _tagCollection;

        public ITagCollection TagCollection => _tagCollection;

        private bool _allowPrivateMemberReferences;
        private ITagDataContext _dataContext;

        private bool _isOperandValid;
        private DataOperandInfo _info;

        private bool _monitorValue;
        private bool _monitorAttribute;

        public DataOperand(DataServer dataServer)
        {
            _dataServer = dataServer;
        }

        public override string ToString()
        {
            return _operand;
        }

        public IDataServer DataServer => _dataServer;

        public DataTypeInfo DataTypeInfo
        {
            get
            {
                if (_info != null)
                    return _info.DataTypeInfo;

                return default(DataTypeInfo);
            }
        }

        public IField Field => _info.Field;

        public ITag Tag => _info?.Tag;

        public bool IsMonitoring => _monitorValue || _monitorAttribute;

        public bool BoolValue
        {
            get
            {
                if (_info != null)
                {
                    if (_info.IsBit)
                    {
                        return _info.Field.GetBitValue(_info.BitOffset);
                    }

                    BoolField boolField = _info.Field as BoolField;
                    if (boolField != null)
                    {
                        return boolField.value != 0;
                    }

                    throw new NotImplementedException();
                }

                return false;
            }
        }

        public int Int32Value
        {
            get
            {
                if (_info != null)
                {
                    UInt32Field uInt32Field = _info.Field as UInt32Field;
                    if (uInt32Field != null)
                        return (int)uInt32Field.value;

                    Int32Field int32Field = _info.Field as Int32Field;
                    if (int32Field != null)
                        return int32Field.value;

                    throw new NotImplementedException();
                }

                return 0;
            }
        }

        public short Int16Value
        {
            get
            {
                if (_info != null)
                {
                    UInt16Field uint16Field = _info.Field as UInt16Field;
                    if (uint16Field != null)
                        return (short)uint16Field.value;

                    Int16Field int16Field = _info.Field as Int16Field;
                    if (int16Field != null)
                        return int16Field.value;

                    throw new NotImplementedException();
                }

                return 0;
            }
        }

        public sbyte Int8Value
        {
            get
            {
                if (_info != null)
                {
                    UInt8Field uint8Field = _info.Field as UInt8Field;
                    if (uint8Field != null)
                        return (sbyte)uint8Field.value;

                    Int8Field int8Field = _info.Field as Int8Field;
                    if (int8Field != null)
                        return int8Field.value;
                }

                return 0;
            }
        }

        public uint UInt32Value
        {
            get
            {
                if (_info != null)
                {
                    UInt32Field uInt32Field = _info.Field as UInt32Field;
                    if (uInt32Field != null)
                        return uInt32Field.value;

                    Int32Field int32Field = _info.Field as Int32Field;
                    if (int32Field != null)
                        return (uint)int32Field.value;

                    throw new NotImplementedException();
                }

                return 0;
            }
        }

        public ushort UInt16Value
        {
            get
            {
                if (_info != null)
                {
                    UInt16Field uint16Field = _info.Field as UInt16Field;
                    if (uint16Field != null)
                        return uint16Field.value;

                    Int16Field int16Field = _info.Field as Int16Field;
                    if (int16Field != null)
                        return (ushort)int16Field.value;

                    throw new NotImplementedException();
                }

                return 0;
            }
        }

        public byte UInt8Value
        {
            get
            {
                if (_info != null)
                {
                    UInt8Field uint8Field = _info.Field as UInt8Field;
                    if (uint8Field != null)
                        return uint8Field.value;

                    Int8Field int8Field = _info.Field as Int8Field;
                    if (int8Field != null)
                        return (byte)int8Field.value;

                    throw new NotImplementedException();
                }

                return 0;
            }
        }

        public double DoubleValue
        {
            get
            {
                if (_info != null)
                {
                    RealField realField = _info.Field as RealField;
                    if (realField != null)
                        return realField.value;

                    throw new NotImplementedException();
                }

                return 0;
            }
        }

        private bool IsOutOfRange()
        {
            if (_info.IsBit)
            {
                var field = _info.Field;
                int offset = _info.BitOffset;

                Int8Field int8Field = field as Int8Field;
                if (int8Field != null)
                {
                    if (offset > 7)
                        return true;
                }

                Int16Field int16Field = field as Int16Field;
                if (int16Field != null)
                {
                    if (offset > 15)
                        return true;
                }

                Int32Field int32Field = field as Int32Field;
                if (int32Field != null)
                {
                    if (offset > 31)
                        return true;
                }

                Int64Field int64Field = field as Int64Field;
                if (int64Field != null)
                {
                    if (offset > 63)
                        return true;
                }

                BoolArrayField boolArrayField = field as BoolArrayField;
                if (boolArrayField != null)
                {
                    if (offset >= boolArrayField.BitCount)
                        return true;
                }

                BoolField boolField = field as BoolField;
                if (boolField != null)
                {
                    if (offset > 0)
                        return true;
                }
            }

            return false;
        }

        public string FormattedValueString
        {
            get
            {
                if (!_isOperandValid)
                    return "??";

                if (_info == null)
                    return string.Empty;

                if (_info.IsDynamicField)
                    return "??";

                var field = _info.Field;
                var displayStyle = _info.DisplayStyle;

                if (_info.IsBit)
                {
                    var boolArrayField = field as BoolArrayField;
                    if (boolArrayField != null && boolArrayField.BitCount <= _info.BitOffset)
                        return "??";

                    if (IsOutOfRange())
                        return "??";

                    var bitValue = field.GetBitValue(_info.BitOffset);

                    return bitValue ? "1" : "0";
                }

                try

                {
                    string str = "NotImplemented";

                    //Debug.WriteLine($"{_info.Tag.Name},{_info.Expression},{_info.DisplayStyle}");

                    if (field is BoolField)
                        str = ((BoolField)field).value.ToString(displayStyle);
                    else if (field is UInt8Field)
                        str = ((UInt8Field)field).value.ToString(displayStyle);
                    else if (field is UInt16Field)
                        str = ((UInt16Field)field).value.ToString(displayStyle);
                    else if (field is UInt32Field)
                        str = ((UInt32Field)field).value.ToString(displayStyle);
                    else if (field is UInt64Field)
                        str = ((UInt64Field)field).value.ToString(displayStyle);
                    else if (field is Int8Field)
                        str = ((Int8Field)field).value.ToString(displayStyle);
                    else if (field is Int16Field)
                        str = ((Int16Field)field).value.ToString(displayStyle);
                    else if (field is Int32Field)
                        str = ((Int32Field)field).value.ToString(displayStyle);
                    else if (field is Int64Field)
                        str = ((Int64Field)field).value.ToString(displayStyle);
                    else if (field is RealField)
                        str = ((RealField)field).value.ToString(displayStyle);
                    else if (field is STRINGField)
                        str = ((STRINGField)field).ToString(displayStyle);
                    else if (field is IArrayField)
                        str = "??";

                    if (IsString)
                    {
                        str = ((ICompositeField)field).ToString(displayStyle);
                    }

                    return str;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    return "???";
                }

            }
        }

        public object OriginalValue
        {
            get
            {
                if (!_isOperandValid)
                    return "??";

                if (_info == null)
                    return string.Empty;

                if (_info.IsDynamicField)
                    return "??";

                var field = _info.Field;
                var displayStyle = _info.DisplayStyle;

                if (_info.IsBit)
                {
                    var boolArrayField = field as BoolArrayField;
                    if (boolArrayField != null && boolArrayField.BitCount <= _info.BitOffset)
                        return "??";

                    if (IsOutOfRange())
                        return "??";

                    var bitValue = field.GetBitValue(_info.BitOffset);

                    return bitValue;
                }

                try
                {
                    object str = null;

                    if (field is BoolField)
                        str = ((BoolField)field).value;
                    else if (field is UInt8Field)
                        str = ((UInt8Field)field).value;
                    else if (field is UInt16Field)
                        str = ((UInt16Field)field).value;
                    else if (field is UInt32Field)
                        str = ((UInt32Field)field).value;
                    else if (field is UInt64Field)
                        str = ((UInt64Field)field).value;
                    else if (field is Int8Field)
                        str = ((Int8Field)field).value;
                    else if (field is Int16Field)
                        str = ((Int16Field)field).value;
                    else if (field is Int32Field)
                        str = ((Int32Field)field).value;
                    else if (field is Int64Field)
                        str = ((Int64Field)field).value;
                    else if (field is RealField)
                        str = ((RealField)field).value;
                    else if (field is STRINGField)
                        str = (STRINGField)field;
                    else if (field is ArrayField)
                        str = "??";

                    if (IsString)
                        str = ((ICompositeField)field).ToString(displayStyle);

                    return str;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    return "???";
                }
            }
        }

        public bool IsValueValid
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsOperandValid => _isOperandValid;

        public bool IsAtomic => _info.DataTypeInfo.DataType.IsAtomic;

        public bool IsString
        {
            get
            {
                if (_isOperandValid)
                {
                    if (_info.DataTypeInfo.Dim1 == 0 && _info.DataTypeInfo.DataType.IsStringType)
                        return true;
                }

                return false;
            }
        }

        public bool IsForced
        {
            get { throw new NotImplementedException(); }
        }

        public bool BoolForcedValue
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsBool
        {
            get
            {
                if (_info != null)
                {
                    if (_info.IsBit)
                        return true;

                    if (_info.DataTypeInfo.DataType.IsBool)
                        return true;
                }

                return false;
            }
        }

        public bool IsConstant
        {
            get
            {
                Tag tag = _info?.Tag;

                if (tag != null)
                    return tag.IsConstant;

                return false;
            }
        }

        public Usage Usage
        {
            get
            {
                Tag tag = _info?.Tag;

                if (tag != null)
                    return tag.Usage;

                return Usage.NullParameterType;
            }
        }

        public TagExpressionBase GetTagExpression()
        {
            return _info?.Expression;
        }

        public byte[] GetRawValue()
        {
            if (!_isOperandValid)
                return null;


            throw new NotImplementedException();
        }

        public void SetOperandString(string operand)
        {
            SetOperandString(operand, null, false, null);
        }

        public void SetOperandString(string operand, ITagCollection tagCollection)
        {
            SetOperandString(operand, tagCollection, false, null);
        }

        public void SetOperandString(
            string operand, ITagCollection tagCollection,
            bool allowPrivateMemberReferences)
        {
            SetOperandString(operand, tagCollection, allowPrivateMemberReferences, null);
        }

        public void SetOperandString(
            string operand, ITagCollection tagCollection,
            bool allowPrivateMemberReferences,
            ITagDataContext dataContext)
        {
            _operand = operand;
            _tagCollection = tagCollection;
            _allowPrivateMemberReferences = allowPrivateMemberReferences;
            _dataContext = dataContext;

            _isOperandValid = VerifyOperand();

            _dataServer.OnDataOperandCreated(this);

        }

        public void SetValue(string tagValue)
        {
            SetValue(tagValue, false);
        }

        public void SetValue(string tagValue, bool allowPrivateMemberReferences)
        {
            if (_info == null)
                return;

            IField newField = null;
            int bitOffset = _info.BitOffset;
            var field = _info.Field;
            bool isBit = false;
            if (DataTypeInfo.DataType.IsBool || _info.IsBit)
            {
                //field即bit
                bool boolValue = ValueConverter.ToBoolean(tagValue);

                var boolField = field as BoolField;
                if (boolField != null)
                {
                    boolField.value = (byte)(boolValue ? 1 : 0);
                    newField = boolField;
                }

                var boolArrayField = field as BoolArrayField;
                if (boolArrayField != null)
                {
                    boolArrayField.Set(bitOffset, boolValue);
                    newField = boolArrayField;
                }

                //field的成员为bit
                //var boolField = field as BoolField;
                //if (boolField != null)
                //{
                //    boolField.value = (byte)(boolValue ? 1 : 0); ;
                //    newField = boolField;
                //}
                var int8Field = field as Int8Field;
                if (int8Field != null)
                {
                    var newValue = BitValue.Set(int8Field.value, bitOffset, boolValue);
                    int8Field.value = newValue;
                    newField = int8Field;
                }

                var int16Field = field as Int16Field;
                if (int16Field != null)
                {
                    var newValue = BitValue.Set(int16Field.value, bitOffset, boolValue);
                    int16Field.value = newValue;
                    newField = int16Field;
                }

                var int32Field = field as Int32Field;
                if (int32Field != null)
                {
                    var newValue = BitValue.Set(int32Field.value, bitOffset, boolValue);
                    int32Field.value = newValue;
                    newField = int32Field;
                }

                var int64Field = field as Int64Field;
                if (int64Field != null)
                {
                    var newValue = BitValue.Set(int64Field.value, bitOffset, boolValue);
                    int64Field.value = newValue;
                    newField = int64Field;
                }

                isBit = true;
            }
            else if (DataTypeInfo.DataType.IsNumber)
            {
                var displayStyle = _info.DisplayStyle;
                var int8Field = field as Int8Field;
                if (int8Field != null)
                {
                    var newValue = ValueConverter.ToSByte(tagValue, displayStyle);
                    int8Field.value = newValue;
                    newField = int8Field;
                }

                var int16Field = field as Int16Field;
                if (int16Field != null)
                {
                    var newValue = ValueConverter.ToShort(tagValue, displayStyle);
                    int16Field.value = newValue;
                    newField = int16Field;
                }

                var int32Field = field as Int32Field;
                if (int32Field != null)
                {
                    var newValue = ValueConverter.ToInt(tagValue, displayStyle);
                    int32Field.value = newValue;
                    newField = int32Field;
                }

                var int64Field = field as Int64Field;
                if (int64Field != null)
                {
                    var newValue = ValueConverter.ToLong(tagValue, displayStyle);
                    int64Field.value = newValue;
                    newField = int64Field;
                }

                var realField = field as RealField;
                if (realField != null)
                {
                    var newValue = ValueConverter.ToFloat(tagValue);
                    realField.value = newValue;
                    newField = realField;
                }
            }
            else if (IsString)
            {
                try
                {
                    var bytes = ValueConverter.ToBytes(tagValue);

                    var compositeField = field as ICompositeField;

                    if (compositeField.EqualsByteList(bytes))
                        return;

                    compositeField.Set(bytes);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                }
            }

            if (newField == null)
                return;

            Notifications.SendNotificationData(new TagNotificationData()
            {
                Tag = _info.Tag,
                Type = TagNotificationData.NotificationType.Value,
                Field = newField,
                IsBit = isBit,
                BitOffset = bitOffset
            });
        }

        public void ForceUpdate()
        {
            throw new NotImplementedException();
        }

        public void StartMonitoring(bool monitorValue, bool monitorAttribute)
        {
            _monitorValue = monitorValue;
            _monitorAttribute = monitorAttribute;
        }

        public void StopMonitoring(bool stopMonitorValue, bool stopMonitorAttribute)
        {
            if (_monitorValue && stopMonitorValue)
                _monitorValue = false;

            if (_monitorAttribute && stopMonitorAttribute)
                _monitorAttribute = false;
        }

        public bool CheckClientChangeValueAccess(ITagDataContext dataContext)
        {
            throw new NotImplementedException();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler DisplayStyleChanged;
        //public event EventHandler<DataOperandAttributeChangedEventArgs> OperandAttributeChanged;

        internal void NotifyDataOperandChanged(NotificationData notificationData)
        {
            TagNotificationData tagNotificationData = notificationData as TagNotificationData;
            if (tagNotificationData == null)
                return;

            if (_info == null)
                return;

            if (tagNotificationData.Tag.BaseTag != _info.Tag.BaseTag)
                return;


            if (_monitorValue && tagNotificationData.Type == TagNotificationData.NotificationType.Value)
            {
                if (tagNotificationData.Field == _info.Field)
                {
                    if (tagNotificationData.IsBit && _info.IsBit)
                    {
                        if (tagNotificationData.BitOffset != _info.BitOffset)
                            return;
                    }

                    OnPropertyChanged(new PropertyChangedEventArgs("RawValue"));
                    OnPropertyChanged(new PropertyChangedEventArgs("BoolValue"));
                    OnPropertyChanged(new PropertyChangedEventArgs("Int8Value"));
                    OnPropertyChanged(new PropertyChangedEventArgs("Int16Value"));
                    OnPropertyChanged(new PropertyChangedEventArgs("Int32Value"));
                    OnPropertyChanged(new PropertyChangedEventArgs("UInt8Value"));
                    OnPropertyChanged(new PropertyChangedEventArgs("UInt16Value"));
                    OnPropertyChanged(new PropertyChangedEventArgs("UInt32Value"));
                    OnPropertyChanged(new PropertyChangedEventArgs("FormattedValueString"));
                }
            }

            if (_monitorAttribute && tagNotificationData.Type == TagNotificationData.NotificationType.Attribute)
            {
                if (tagNotificationData.Attribute == "DisplayStyle")
                {
                    var info = GetDataOperandInfo(_operand);
                    if (info != null)
                    {
                        _info.DisplayStyle = info.DisplayStyle;
                    }
                }

                //TODO(gjc): add code here
            }
        }

        protected void NotifyPropertyChanged(string propertyName) =>
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        protected virtual void OnDisplayStyleChanged(EventArgs e)
        {
            DisplayStyleChanged?.Invoke(this, e);
        }

        private DataOperandInfo GetDataOperandInfo(string operand)
        {
            // get tag and expression
            TagExpressionParser parser = new TagExpressionParser();
            var tagExpression = parser.Parser(operand);
            if (tagExpression == null)
                return null;

            SimpleTagExpression simpleTagExpression = parser.GetSimpleTagExpression(tagExpression);

            ITagCollection tagCollection;

            if (!string.IsNullOrEmpty(simpleTagExpression.Scope))
            {
                IProgram program = _dataServer.ParentController.Programs[simpleTagExpression.Scope];
                if (program == null)
                    return null;

                tagCollection = program.Tags;
            }
            else
            {
                tagCollection = _tagCollection ?? _dataServer.ParentController.Tags;
            }

            var tag = tagCollection[simpleTagExpression.TagName] as Tag;

            if (tag == null)
                return null;

            // data type and value
            DataTypeInfo dataTypeInfo = tag.DataTypeInfo;
            IField field = tag.DataWrapper.Data;
            bool isDynamicField = false;
            DisplayStyle displayStyle = tag.DisplayStyle;
            bool isBit = false;
            int bitOffset = -1;

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
                    else if (bitMemberExpressionAccessExpression.ExpressionNumber != null)
                    {
                        bitMemberNumber = 0;
                        isDynamicField = true;
                    }
                }

                if (bitMemberNumber >= 0)
                {
                    if (!dataTypeInfo.DataType.IsInteger || dataTypeInfo.Dim1 != 0)
                        return null;

                    isBit = true;
                    bitOffset = bitMemberNumber;
                    displayStyle = DisplayStyle.Decimal;

                    break;
                }

                //
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

                        //if (dataTypeMember.IsHidden)
                        //    return null;

                        field = compositeField.fields[dataTypeMember.FieldIndex].Item1;
                        dataTypeInfo = dataTypeMember.DataTypeInfo;
                        displayStyle = dataTypeMember.DisplayStyle;

                        if (dataTypeMember.IsBit && dataTypeInfo.Dim1 == 0)
                        {
                            isBit = true;
                            bitOffset = dataTypeMember.BitOffset;
                            displayStyle = DisplayStyle.Decimal;
                        }


                    }
                    else
                    {
                        return null;
                    }

                }

                //
                var elementAccessExpression = expression as ElementAccessExpression;
                if (elementAccessExpression != null
                    && elementAccessExpression.Indexes != null
                    && elementAccessExpression.Indexes.Count > 0)
                {
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

                    ArrayField arrayField = field as ArrayField;
                    BoolArrayField boolArrayField = field as BoolArrayField;

                    if (arrayField == null && boolArrayField == null)
                        return null;

                    if (arrayField != null)
                    {
                        if (index < 0 || index >= arrayField.Size())
                            return null;

                        dataTypeInfo = new DataTypeInfo { DataType = dataTypeInfo.DataType };
                        field = arrayField.fields[index].Item1;
                    }

                    if (boolArrayField != null)
                    {
                        if (index < 0 || index >= boolArrayField.BitCount)
                            return null;

                        dataTypeInfo = new DataTypeInfo { DataType = dataTypeInfo.DataType };
                        field = boolArrayField;
                        isBit = true;
                        bitOffset = index;
                    }
                }

                if (elementAccessExpression != null
                    && elementAccessExpression.ExpressionIndexes != null
                    && elementAccessExpression.ExpressionIndexes.Count > 0)
                {
                    // ignore expression valid

                    ArrayField arrayField = field as ArrayField;
                    BoolArrayField boolArrayField = field as BoolArrayField;

                    if (arrayField == null && boolArrayField == null)
                        return null;

                    int index = 0;
                    if (arrayField != null)
                    {
                        dataTypeInfo = new DataTypeInfo { DataType = dataTypeInfo.DataType };
                        field = arrayField.fields[index].Item1;
                    }

                    if (boolArrayField != null)
                    {
                        dataTypeInfo = new DataTypeInfo { DataType = dataTypeInfo.DataType };
                        field = boolArrayField;
                        isBit = true;
                        bitOffset = index;
                    }

                    isDynamicField = true;
                }

            }

            return new DataOperandInfo
            {
                Expression = tagExpression,
                Tag = tag,
                DisplayStyle = displayStyle,
                Field = field,
                IsDynamicField = isDynamicField,
                DataTypeInfo = dataTypeInfo,
                IsBit = isBit,
                BitOffset = bitOffset
            };

        }

        private bool VerifyOperand()
        {
            _info = null;

            var info = GetDataOperandInfo(_operand);

            if (info == null)
                return false;

            _info = info;

            return true;
        }
    }
    public class DataOperandInfo
    {
        public TagExpressionBase Expression { get; set; }
        public Tag Tag { get; set; }
        public DataTypeInfo DataTypeInfo { get; set; }

        public DisplayStyle DisplayStyle { get; set; }
        public IField Field { get; set; }
        public bool IsDynamicField { get; set; }

        public bool IsBit { get; set; }
        public int BitOffset { get; set; }
    }
}
