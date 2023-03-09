using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSStudio.EditorPackage.DataTypes.Change;
using ICSStudio.FileConverter.JsonToL5X;
using ICSStudio.FileConverter.JsonToL5X.Model;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Tags;

namespace ICSStudio.EditorPackage.DataTypes
{
    public partial class NewDataTypeViewModel
    {
        private readonly DataTypeChangeManager _dataTypeChangeManager;

        private readonly Dictionary<ITag, object> _tagValueDic;

        private void PreApply()
        {
            if (_isExist)
            {
                try
                {
                    _tagValueDic.Clear();

                    // 1. get tag impact by this datatype
                    List<ITag> tags = GetAllTagsImpactedByDataType(_dataType);

                    // 2. save value
                    if (tags.Count > 0)
                    {
                        foreach (var tag in tags.OfType<Tag>())
                        {
                            if (tag.DataTypeInfo.Dim1 == 0)
                            {
                                _tagValueDic.Add(tag,
                                    Converter.ToDataStructure(null, tag.DataTypeInfo.DataType, tag.DataWrapper.Data));
                            }
                            else if (tag.DataTypeInfo.Dim1 > 0)
                            {
                                _tagValueDic.Add(tag, Converter.ToDataArray(tag));
                            }

                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }

        }

        private List<ITag> GetAllTagsImpactedByDataType(IDataType dataType)
        {
            List<ITag> tags = new List<ITag>();

            foreach (var tag in _controller.Tags)
            {
                if (IsReference(dataType, tag.DataTypeInfo.DataType))
                {
                    tags.Add(tag);
                }
            }

            foreach (var program in _controller.Programs)
            {
                foreach (var tag in program.Tags)
                {
                    if (IsReference(dataType, tag.DataTypeInfo.DataType))
                    {
                        tags.Add(tag);
                    }
                }
            }

            return tags;
        }

        private static bool IsReference(IDataType parentType, IDataType memberType)
        {
            if (parentType == memberType)
                return true;

            CompositiveType compositiveType = memberType as CompositiveType;
            if (compositiveType != null)
            {
                foreach (var member in compositiveType.TypeMembers)
                {
                    if (IsReference(parentType, member.DataTypeInfo.DataType))
                        return true;
                }
            }

            return false;
        }

        private void PostApply()
        {
            if (_isExist)
            {
                try
                {
                    // restore value
                    foreach (var keyValuePair in _tagValueDic)
                    {
                        Tag tag = keyValuePair.Key as Tag;
                        if (tag != null)
                        {
                            DataStructure dataStructure = keyValuePair.Value as DataStructure;
                            DataArray dataArray = keyValuePair.Value as DataArray;

                            if (dataStructure != null)
                                RestoreValue(tag.DataWrapper.Data, tag.DataTypeInfo.DataType, dataStructure);

                            if(dataArray != null)
                                RestoreArrayValue(tag.DataWrapper.Data, tag.DataTypeInfo, dataArray);
                        }

                    }

                    _tagValueDic.Clear();

                    // reset log
                    _dataTypeChangeManager.ResetLog();

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
        }

        private void RestoreArrayValue(IField field, DataTypeInfo dataTypeInfo, DataArray dataArray)
        {
            ArrayField arrayField = field as ArrayField;

            if (arrayField != null && dataArray?.Element != null)
            {
                int copyLength = arrayField.fields.Count;
                if (copyLength > dataArray.Element.Length)
                    copyLength = dataArray.Element.Length;

                for (int i = 0; i < copyLength; i++)
                {
                    var element = dataArray.Element[i];
                    if (element.Structure != null && element.Structure.Length > 0)
                    {
                        RestoreValue(arrayField.fields[i].Item1, dataTypeInfo.DataType, element.Structure[0]);
                    }
                }
            }

        }

        private void RestoreValue(IField dataField, IDataType dataType, DataStructure dataStructure)
        {
            if (dataStructure != null && dataStructure.Items.Length > 0)
            {
                if (string.Equals(dataType.Name, dataStructure.DataType, StringComparison.OrdinalIgnoreCase))
                {
                    var compositeField = dataField as ICompositeField;
                    var compositiveType = dataType as CompositiveType;

                    if (dataType.IsStringType)
                    {
                        RestoreStringValue(dataField, dataType, dataStructure);
                        return;
                    }

                    if (compositeField != null && compositiveType != null)
                    {
                        foreach (var item in dataStructure.Items)
                        {
                            DataArray array = item as DataArray;
                            DataValue value = item as DataValue;
                            DataStructure structure = item as DataStructure;

                            if (array != null)
                            {
                                DataTypeMember member = compositiveType[array.Name] as DataTypeMember;
                                if (member != null)
                                {
                                    if (member.Dim1 > 0)
                                    {
                                        int length = Math.Max(member.Dim1, 1)
                                                     * Math.Max(member.Dim2, 1)
                                                     * Math.Max(member.Dim3, 1);
                                        if (length > array.Element.Length)
                                            length = array.Element.Length;

                                        var arrayField = compositeField.fields[member.FieldIndex].Item1 as ArrayField;
                                        var boolArrayField =
                                            compositeField.fields[member.FieldIndex].Item1 as BoolArrayField;

                                        if (arrayField?.fields != null && arrayField.fields.Count > 0)
                                        {
                                            for (int i = 0; i < length; i++)
                                            {
                                                var field = arrayField.fields[i].Item1;
                                                UpdateField(field, member, array, i);
                                            }
                                        }
                                        else if (boolArrayField != null)
                                        {
                                            for (int i = 0; i < length; i++)
                                            {
                                                boolArrayField.Set(i, DataArrayElementToBool(array, i));
                                            }
                                        }

                                    }
                                    else
                                    {
                                        var field = compositeField.fields[member.FieldIndex].Item1;
                                        UpdateField(field, member, array, 0);
                                    }
                                }
                            }
                            else if (value != null)
                            {
                                DataTypeMember member = compositiveType[value.Name] as DataTypeMember;
                                if (member != null)
                                {
                                    if (member.Dim1 > 0)
                                    {
                                        var arrayField = compositeField.fields[member.FieldIndex].Item1 as ArrayField;
                                        var boolArrayField =
                                            compositeField.fields[member.FieldIndex].Item1 as BoolArrayField;

                                        if (arrayField?.fields != null && arrayField.fields.Count > 0)
                                        {
                                            UpdateField(arrayField.fields[0].Item1, member, value);
                                        }
                                        else
                                        {
                                            boolArrayField?.Set(0, ConvertToBool(value));
                                        }
                                    }
                                    else
                                    {
                                        var field = compositeField.fields[member.FieldIndex].Item1;

                                        UpdateField(field, member, value);
                                    }
                                }
                            }
                            else if (structure != null)
                            {
                                DataTypeMember member = compositiveType[structure.Name] as DataTypeMember;
                                if (member != null)
                                {
                                    if (string.Equals(structure.DataType, member.DataType.Name,
                                            StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (member.DataType.IsStringType)
                                        {
                                            var field = compositeField.fields[member.FieldIndex].Item1;
                                            RestoreStringValue(field, member.DataType, structure);
                                        }
                                        else
                                        {
                                            var field = compositeField.fields[member.FieldIndex].Item1;
                                            RestoreValue(field, member.DataType, structure);
                                        }
                                    }
                                    else
                                    {
                                        //TODO(gjc): add here for different type???
                                    }

                                }
                            }

                        }
                    }
                }
            }

        }

        private void RestoreStringValue(IField dataField, IDataType dataType, DataStructure dataStructure)
        {
            if (!dataType.IsStringType)
                return;

            var compositeField = dataField as ICompositeField;
            Debug.Assert(compositeField != null);

            Int32Field lenField = compositeField.fields[0].Item1 as Int32Field;
            Debug.Assert(lenField != null);

            ArrayField arrayField = compositeField.fields[1].Item1 as ArrayField;
            Debug.Assert(arrayField != null);

            List<byte> stringData = ConvertToString(dataStructure);

            if (stringData != null && stringData.Count > 0)
            {
                int copyLength = stringData.Count;

                if (copyLength > arrayField.fields.Count)
                    copyLength = arrayField.fields.Count;

                for (int i = 0; i < copyLength; i++)
                {
                    var int8Field = arrayField.fields[i].Item1 as Int8Field;
                    Debug.Assert(int8Field != null);

                    int8Field.value = (sbyte)stringData[i];
                }

                lenField.value = copyLength;
            }

        }

        private List<byte> ConvertToString(DataStructure dataStructure)
        {
            if (dataStructure.Items != null && dataStructure.Items.Length == 2)
            {
                DataValue lenData = dataStructure.Items[0] as DataValue;
                DataValue textData = dataStructure.Items[1] as DataValue;

                int length = 0;
                if (lenData != null)
                {
                    length = int.Parse(lenData.Value);
                }

                List<byte> bytes = null;
                if (textData != null && textData.Text != null && textData.Text.Length > 0)
                {
                    bytes = ValueConverter.ToBytes(textData.Text[0].Value);
                }

                if (length>0 && bytes != null)
                {
                    List<byte> result = new List<byte>();

                    int copyLength = length;
                    if (copyLength > bytes.Count)
                        copyLength = bytes.Count;

                    for (int i = 0; i < copyLength; i++)
                    {
                        result.Add(bytes[i]);
                    }

                    return result;
                }

            }
            
            return null;
        }

        private void UpdateField(IField field, DataTypeMember member, DataArray dataArray, int index)
        {
            if (member.IsBit)
            {
                bool bitValue = DataArrayElementToBool(dataArray, index);
                field.SetBitValue(member.BitOffset, bitValue);
            }
            else
            {
                if (index >= dataArray.Element.Length)
                    return;

                var element = dataArray.Element[index];

                Int8Field int8Field = field as Int8Field;
                if (int8Field != null)
                {
                    int8Field.value = DataArrayElementToInt8(dataArray, index);
                    return;
                }

                Int16Field int16Field = field as Int16Field;
                if (int16Field != null)
                {
                    int16Field.value = DataArrayElementToInt16(dataArray, index);
                    return;
                }

                Int32Field int32Field = field as Int32Field;
                if (int32Field != null)
                {
                    int32Field.value = DataArrayElementToInt32(dataArray, index);
                    return;
                }

                Int64Field int64Field = field as Int64Field;
                if (int64Field != null)
                {
                    int64Field.value = DataArrayElementToInt64(dataArray, index);
                    return;
                }

                RealField realField = field as RealField;
                if (realField != null)
                {
                    realField.value = DataArrayElementToReal(dataArray, index);
                    return;
                }

                var compositeField = field as ICompositeField;
                if (compositeField != null)
                {
                    if (element.Structure != null && element.Structure.Length > 0)
                    {
                        RestoreValue(field, member.DataType, element.Structure[0]);
                    }

                }

            }
        }

        private float DataArrayElementToReal(DataArray dataArray, int index)
        {
            string dataType = dataArray.DataType.ToUpper();

            if (index >= dataArray.Element.Length)
                return 0;

            var element = dataArray.Element[index];

            if (dataType == "BOOL")
            {
                return int.Parse(element.Value);
            }

            if (dataType == "SINT")
                return sbyte.Parse(element.Value);

            if (dataType == "INT")
                return short.Parse(element.Value);

            if (dataType == "DINT")
                return int.Parse(element.Value);

            if (dataType == "REAL")
                return float.Parse(element.Value);

            return 0;
        }

        private long DataArrayElementToInt64(DataArray dataArray, int index)
        {
            string dataType = dataArray.DataType.ToUpper();

            if (index >= dataArray.Element.Length)
                return 0;

            var element = dataArray.Element[index];

            if (dataType == "BOOL")
            {
                return int.Parse(element.Value);
            }

            if (dataType == "SINT")
                return sbyte.Parse(element.Value);

            if (dataType == "INT")
                return short.Parse(element.Value);

            if (dataType == "DINT")
                return int.Parse(element.Value);

            if (dataType == "REAL")
                return Convert.ToInt64(float.Parse(element.Value));

            return 0;
        }

        private int DataArrayElementToInt32(DataArray dataArray, int index)
        {
            string dataType = dataArray.DataType.ToUpper();

            if (index >= dataArray.Element.Length)
                return 0;

            var element = dataArray.Element[index];

            if (dataType == "BOOL")
            {
                return int.Parse(element.Value);
            }

            if (dataType == "SINT")
                return sbyte.Parse(element.Value);

            if (dataType == "INT")
                return short.Parse(element.Value);

            if (dataType == "DINT")
                return int.Parse(element.Value);

            if (dataType == "REAL")
                return Convert.ToInt32(float.Parse(element.Value));


            return 0;
        }

        private short DataArrayElementToInt16(DataArray dataArray, int index)
        {
            string dataType = dataArray.DataType.ToUpper();

            if (index >= dataArray.Element.Length)
                return 0;

            var element = dataArray.Element[index];

            if (dataType == "BOOL")
            {
                return short.Parse(element.Value);
            }

            if (dataType == "SINT")
                return sbyte.Parse(element.Value);

            if (dataType == "INT")
                return short.Parse(element.Value);

            if (dataType == "DINT")
                return (short)int.Parse(element.Value);

            if (dataType == "REAL")
                return Convert.ToInt16(float.Parse(element.Value));

            return 0;
        }

        private sbyte DataArrayElementToInt8(DataArray dataArray, int index)
        {
            string dataType = dataArray.DataType.ToUpper();

            if (index >= dataArray.Element.Length)
                return 0;

            var element = dataArray.Element[index];

            if (dataType == "BOOL")
            {
                return sbyte.Parse(element.Value);
            }

            if (dataType == "SINT")
                return sbyte.Parse(element.Value);

            if (dataType == "INT")
                return (sbyte)short.Parse(element.Value);

            if (dataType == "DINT")
                return (sbyte)int.Parse(element.Value);

            if (dataType == "REAL")
                return Convert.ToSByte(float.Parse(element.Value));

            return 0;
        }

        private bool DataArrayElementToBool(DataArray dataArray, int index)
        {
            string dataType = dataArray.DataType.ToUpper();

            if (index >= dataArray.Element.Length)
                return false;

            var element = dataArray.Element[index];

            if (dataType == "BOOL")
            {
                switch (element.Value)
                {
                    case "0":
                        return false;
                    case "1":
                        return true;
                }
            }

            if (dataType == "SINT" || dataType == "INT" || dataType == "DINT")
            {
                int temp = int.Parse(element.Value);
                return temp != 0;
            }

            if (dataType == "REAL")
            {
                float temp = float.Parse(element.Value);
                if (Math.Abs(temp) < float.Epsilon)
                    return false;

                return true;
            }

            return false;
        }

        private void UpdateField(IField field, DataTypeMember member, DataValue value)
        {
            if (member.IsBit)
            {
                bool bitValue = ConvertToBool(value);
                field.SetBitValue(member.BitOffset, bitValue);
            }
            else
            {
                Int8Field int8Field = field as Int8Field;
                if (int8Field != null)
                {
                    int8Field.value = ConvertToInt8(value);
                    return;
                }

                Int16Field int16Field = field as Int16Field;
                if (int16Field != null)
                {
                    int16Field.value = ConvertToInt16(value);
                    return;
                }

                Int32Field int32Field = field as Int32Field;
                if (int32Field != null)
                {
                    int32Field.value = ConvertToInt32(value);
                    return;
                }

                Int64Field int64Field = field as Int64Field;
                if (int64Field != null)
                {
                    int64Field.value = ConvertToInt64(value);
                    return;
                }

                RealField realField = field as RealField;
                if (realField != null)
                {
                    realField.value = ConvertToReal(value);
                    return;
                }

                //Debug.WriteLine($"UpdateField Add handle for {dataType}");

            }
        }

        private float ConvertToReal(DataValue value)
        {
            string dataType = value.DataType.ToUpper();

            if (dataType == "BOOL")
            {
                return int.Parse(value.Value);
            }

            if (dataType == "SINT")
                return sbyte.Parse(value.Value);

            if (dataType == "INT")
                return short.Parse(value.Value);

            if (dataType == "DINT")
                return int.Parse(value.Value);

            if (dataType == "REAL")
                return float.Parse(value.Value);

            return 0;
        }

        private long ConvertToInt64(DataValue value)
        {
            string dataType = value.DataType.ToUpper();

            if (dataType == "BOOL")
            {
                return int.Parse(value.Value);
            }

            if (dataType == "SINT")
                return sbyte.Parse(value.Value);

            if (dataType == "INT")
                return short.Parse(value.Value);

            if (dataType == "DINT")
                return int.Parse(value.Value);

            if (dataType == "REAL")
                return Convert.ToInt64(float.Parse(value.Value));

            return 0;
        }

        private int ConvertToInt32(DataValue value)
        {
            string dataType = value.DataType.ToUpper();

            if (dataType == "BOOL")
            {
                return int.Parse(value.Value);
            }

            if (dataType == "SINT")
                return sbyte.Parse(value.Value);

            if (dataType == "INT")
                return short.Parse(value.Value);

            if (dataType == "DINT")
                return int.Parse(value.Value);

            if (dataType == "REAL")
                return Convert.ToInt32(float.Parse(value.Value));


            return 0;
        }

        private short ConvertToInt16(DataValue value)
        {
            string dataType = value.DataType.ToUpper();

            if (dataType == "BOOL")
            {
                return short.Parse(value.Value);
            }

            if (dataType == "SINT")
                return sbyte.Parse(value.Value);

            if (dataType == "INT")
                return short.Parse(value.Value);

            if (dataType == "DINT")
                return (short)int.Parse(value.Value);

            if (dataType == "REAL")
                return Convert.ToInt16(float.Parse(value.Value));


            return 0;
        }

        private sbyte ConvertToInt8(DataValue value)
        {
            string dataType = value.DataType.ToUpper();

            if (dataType == "BOOL")
            {
                return sbyte.Parse(value.Value);
            }

            if (dataType == "SINT")
                return sbyte.Parse(value.Value);

            if (dataType == "INT")
                return (sbyte)short.Parse(value.Value);

            if (dataType == "DINT")
                return (sbyte)int.Parse(value.Value);

            if (dataType == "REAL")
                return Convert.ToSByte(float.Parse(value.Value));


            return 0;
        }

        private bool ConvertToBool(DataValue value)
        {
            string dataType = value.DataType.ToUpper();

            if (dataType == "BOOL")
            {
                if (value.Value == "0")
                    return false;

                if (value.Value == "1")
                    return true;

                return bool.Parse(value.Value);
            }


            if (dataType == "SINT" || dataType == "INT" || dataType == "DINT")
            {
                int temp = int.Parse(value.Value);
                return temp != 0;
            }

            if (dataType == "REAL")
            {
                float temp = float.Parse(value.Value);
                if (Math.Abs(temp) < float.Epsilon)
                    return false;

                return true;
            }

            Debug.WriteLine($"ConvertToBool Add handle for {dataType}");

            return false;
        }
    }
}
