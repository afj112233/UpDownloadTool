using System;
using System.Diagnostics;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.PredefinedType;

namespace ICSStudio.SimpleServices.Tags
{
    public static class DataHelper
    {
        public static void Copy(DataWrapper.DataWrapper newDataWrapper, DataWrapper.DataWrapper oldDataWrapper)
        {
            try
            {
                if (newDataWrapper == null || oldDataWrapper == null)
                    return;

                var newDataTypeInfo = newDataWrapper.DataTypeInfo;
                var oldDataTypeInfo = oldDataWrapper.DataTypeInfo;

                var newArrayField = newDataWrapper.Data as ArrayField;
                var oldArrayField = oldDataWrapper.Data as ArrayField;

                if (newDataTypeInfo.DataType == oldDataTypeInfo.DataType)
                {
                    if (newArrayField != null && oldArrayField != null)
                    {
                        int newCount = newArrayField.fields.Count;
                        int oldCount = oldArrayField.fields.Count;

                        int count = Math.Min(newCount, oldCount);

                        for (int i = 0; i < count; i++)
                        {
                            newArrayField.fields[i].Item1.Update(oldArrayField.fields[i].Item1.ToJToken());
                        }
                    }
                    else if (newArrayField != null)
                    {
                        newArrayField.fields[0].Item1.Update(oldDataWrapper.Data.ToJToken());
                    }
                    else if (oldArrayField != null)
                    {
                        newDataWrapper.Data.Update(oldArrayField.fields[0].Item1.ToJToken());
                    }
                    else
                    {
                        try
                        {
                            newDataWrapper.Data.Update(oldDataWrapper.Data.ToJToken());
                        }
                        catch (Exception)
                        {
                            //ignore
                        }
                    }

                }
                else if (newDataTypeInfo.DataType.IsAtomic && oldDataTypeInfo.DataType.IsAtomic)
                {
                    //TODO(gjc): need edit here
                    try
                    {
                        newDataWrapper.Data.Update(oldDataWrapper.Data.ToJToken());
                    }
                    catch (Exception)
                    {
                        //ignore
                    }
                }
            }
            catch (Exception)
            {
                //ignore
            }

        }

        public static bool IsTagDataTruncatedOrLost(
            DataWrapper.DataWrapper dataWrapper,
            DataTypeInfo newDataTypeInfo
        )
        {
            var oldDataTypeInfo = dataWrapper.DataTypeInfo;

            if (newDataTypeInfo.DataType != oldDataTypeInfo.DataType)
            {
                if (newDataTypeInfo.DataType.IsAtomic && oldDataTypeInfo.DataType.IsAtomic)
                {
                    //TODO(gjc): need edit here, for array
                    if (newDataTypeInfo.Dim1 > 0 || oldDataTypeInfo.Dim1 > 0)
                        return true;

                    if (newDataTypeInfo.DataType is BOOL)
                    {
                        return CheckDataTruncatedForBOOL(dataWrapper.Data);
                    }

                    if (newDataTypeInfo.DataType is SINT)
                    {
                        return CheckDataTruncatedForSINT(dataWrapper.Data);
                    }

                    if (newDataTypeInfo.DataType is INT)
                    {
                        return CheckDataTruncatedForINT(dataWrapper.Data);
                    }

                    if (newDataTypeInfo.DataType is DINT)
                    {
                        return CheckDataTruncatedForDINT(dataWrapper.Data);
                    }

                    if (newDataTypeInfo.DataType is REAL)
                    {
                        return CheckDataTruncatedForREAL(dataWrapper.Data);
                    }

                    //TODO(gjc): need edit here, remove later
                    Debug.WriteLine($"add handle for {newDataTypeInfo.DataType}.");
                    return true;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        #region Truncated

        private static bool CheckDataTruncatedForBOOL(IField field)
        {
            LRealField lRealField = field as LRealField;
            if (lRealField != null)
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (lRealField.value == 1.0 || lRealField.value == 0)
                    return false;

                return true;
            }

            RealField realField = field as RealField;
            if (realField != null)
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (realField.value == 1.0 || realField.value == 0)
                    return false;

                return true;
            }

            Int64Field int64Field = field as Int64Field;
            if (int64Field != null)
            {
                if (int64Field.value == 1 || int64Field.value == 0)
                    return false;

                return true;
            }

            Int32Field int32Field = field as Int32Field;
            if (int32Field != null)
            {
                if (int32Field.value == 1 || int32Field.value == 0)
                    return false;

                return true;
            }

            Int16Field int16Field = field as Int16Field;
            if (int16Field != null)
            {
                if (int16Field.value == 1 || int16Field.value == 0)
                    return false;

                return true;
            }

            Int8Field int8Field = field as Int8Field;
            if (int8Field != null)
            {
                if (int8Field.value == 1 || int8Field.value == 0)
                    return false;

                return true;
            }

            UInt64Field uint64Field = field as UInt64Field;
            if (uint64Field != null)
            {
                if (uint64Field.value == 1 || uint64Field.value == 0)
                    return false;

                return true;
            }

            UInt32Field uint32Field = field as UInt32Field;
            if (uint32Field != null)
            {
                if (uint32Field.value == 1 || uint32Field.value == 0)
                    return false;

                return true;
            }

            UInt16Field uint16Field = field as UInt16Field;
            if (uint16Field != null)
            {
                if (uint16Field.value == 1 || uint16Field.value == 0)
                    return false;

                return true;
            }

            UInt8Field uint8Field = field as UInt8Field;
            if (uint8Field != null)
            {
                if (uint8Field.value == 1 || uint8Field.value == 0)
                    return false;

                return true;
            }

            BoolField boolField = field as BoolField;
            if (boolField != null)
                return false;

            Debug.WriteLine("add code in CheckDataTruncatedForBool.");

            return true;
        }

        private static bool CheckDataTruncatedForSINT(IField field)
        {

            LRealField lRealField = field as LRealField;
            if (lRealField != null)
            {
                if (lRealField.value >= sbyte.MinValue && lRealField.value <= sbyte.MaxValue)
                    return false;

                return true;
            }

            RealField realField = field as RealField;
            if (realField != null)
            {
                double double0 = realField.value;
                if (double0 >= 127.5 || double0 < -128.5)
                    return true;

                return false;
            }

            Int64Field int64Field = field as Int64Field;
            if (int64Field != null)
            {
                if (int64Field.value >= sbyte.MinValue && int64Field.value <= sbyte.MaxValue)
                    return false;

                return true;
            }

            Int32Field int32Field = field as Int32Field;
            if (int32Field != null)
            {
                if (int32Field.value >= sbyte.MinValue && int32Field.value <= sbyte.MaxValue)
                    return false;

                return true;
            }

            Int16Field int16Field = field as Int16Field;
            if (int16Field != null)
            {
                if (int16Field.value >= sbyte.MinValue && int16Field.value <= sbyte.MaxValue)
                    return false;

                return true;
            }

            Int8Field int8Field = field as Int8Field;
            if (int8Field != null)
                return false;

            UInt64Field uint64Field = field as UInt64Field;
            if (uint64Field != null)
            {
                if (uint64Field.value <= (ulong)sbyte.MaxValue)
                    return false;

                return true;
            }

            UInt32Field uint32Field = field as UInt32Field;
            if (uint32Field != null)
            {
                if (uint32Field.value <= sbyte.MaxValue)
                    return false;

                return true;
            }

            UInt16Field uint16Field = field as UInt16Field;
            if (uint16Field != null)
            {
                if (uint16Field.value <= sbyte.MaxValue)
                    return false;

                return true;
            }

            UInt8Field uint8Field = field as UInt8Field;
            if (uint8Field != null)
            {
                if (uint8Field.value <= sbyte.MaxValue)
                    return false;

                return true;
            }

            BoolField boolField = field as BoolField;
            if (boolField != null)
                return false;

            Debug.WriteLine("add code in CheckDataTruncatedForBool.");

            return true;
        }

        private static bool CheckDataTruncatedForINT(IField field)
        {

            LRealField lRealField = field as LRealField;
            if (lRealField != null)
            {
                if (lRealField.value >= Int16.MinValue && lRealField.value <= Int16.MaxValue)
                    return false;

                return true;
            }

            RealField realField = field as RealField;
            if (realField != null)
            {
                double double0 = realField.value;
                if (double0 >= 32767.5 || double0 < -32768.5)
                    return true;

                return false;
            }

            Int64Field int64Field = field as Int64Field;
            if (int64Field != null)
            {
                if (int64Field.value >= Int16.MinValue && int64Field.value <= Int16.MaxValue)
                    return false;

                return true;
            }

            Int32Field int32Field = field as Int32Field;
            if (int32Field != null)
            {
                if (int32Field.value >= Int16.MinValue && int32Field.value <= Int16.MaxValue)
                    return false;

                return true;
            }

            Int16Field int16Field = field as Int16Field;
            if (int16Field != null)
                return false;

            Int8Field int8Field = field as Int8Field;
            if (int8Field != null)
                return false;

            UInt64Field uint64Field = field as UInt64Field;
            if (uint64Field != null)
            {
                if (uint64Field.value <= (ulong)Int16.MaxValue)
                    return false;

                return true;
            }

            UInt32Field uint32Field = field as UInt32Field;
            if (uint32Field != null)
            {
                if (uint32Field.value <= Int16.MaxValue)
                    return false;

                return true;
            }

            UInt16Field uint16Field = field as UInt16Field;
            if (uint16Field != null)
            {
                if (uint16Field.value <= Int16.MaxValue)
                    return false;

                return true;
            }

            UInt8Field uint8Field = field as UInt8Field;
            if (uint8Field != null)
                return false;

            BoolField boolField = field as BoolField;
            if (boolField != null)
                return false;

            Debug.WriteLine("add code in CheckDataTruncatedForBool.");

            return true;
        }

        private static bool CheckDataTruncatedForDINT(IField field)
        {

            LRealField lRealField = field as LRealField;
            if (lRealField != null)
            {
                if (lRealField.value >= Int32.MinValue && lRealField.value <= Int32.MaxValue)
                    return false;

                return true;
            }

            RealField realField = field as RealField;
            if (realField != null)
            {
                double double0 = realField.value;
                if (double0 >= 2147483647.5 || double0 <= -2147483648.5)
                    return true;

                return false;

            }

            Int64Field int64Field = field as Int64Field;
            if (int64Field != null)
            {
                if (int64Field.value >= Int32.MinValue && int64Field.value <= Int32.MaxValue)
                    return false;

                return true;
            }

            Int32Field int32Field = field as Int32Field;
            if (int32Field != null)
                return false;

            Int16Field int16Field = field as Int16Field;
            if (int16Field != null)
                return false;

            Int8Field int8Field = field as Int8Field;
            if (int8Field != null)
                return false;

            UInt64Field uint64Field = field as UInt64Field;
            if (uint64Field != null)
            {
                if (uint64Field.value <= Int32.MaxValue)
                    return false;

                return true;
            }

            UInt32Field uint32Field = field as UInt32Field;
            if (uint32Field != null)
            {
                if (uint32Field.value <= Int32.MaxValue)
                    return false;

                return true;
            }

            UInt16Field uint16Field = field as UInt16Field;
            if (uint16Field != null)
                return false;

            UInt8Field uint8Field = field as UInt8Field;
            if (uint8Field != null)
                return false;

            BoolField boolField = field as BoolField;
            if (boolField != null)
                return false;

            Debug.WriteLine("add code in CheckDataTruncatedForBool.");

            return true;
        }

        private static bool CheckDataTruncatedForREAL(IField field)
        {

            LRealField lRealField = field as LRealField;
            if (lRealField != null)
            {
                if (lRealField.value >= float.MinValue && lRealField.value <= float.MaxValue)
                    return false;

                return true;
            }

            RealField realField = field as RealField;
            if (realField != null)
                return false;

            Int64Field int64Field = field as Int64Field;
            if (int64Field != null)
            {
                float temp0 = Convert.ToSingle(int64Field.value);
                Int64 temp1 = Convert.ToInt64(temp0);
                if (int64Field.value == temp1)
                    return false;

                return true;
            }

            Int32Field int32Field = field as Int32Field;
            if (int32Field != null)
            {
                float float0 = Convert.ToSingle(int32Field.value);
                double double0 = float0;

                if (double0 >= 2147483647.5 || double0 <= -2147483648.5)
                    return true;

                Int32 int0 = Convert.ToInt32(float0);
                if (int32Field.value == int0)
                    return false;

                return true;

            }

            Int16Field int16Field = field as Int16Field;
            if (int16Field != null)
                return false;

            Int8Field int8Field = field as Int8Field;
            if (int8Field != null)
                return false;

            UInt64Field uint64Field = field as UInt64Field;
            if (uint64Field != null)
            {
                if (uint64Field.value <= 16777216)
                    return false;

                return true;
            }

            UInt32Field uint32Field = field as UInt32Field;
            if (uint32Field != null)
            {
                if (uint32Field.value <= 16777216)
                    return false;

                return true;
            }

            UInt16Field uint16Field = field as UInt16Field;
            if (uint16Field != null)
                return false;

            UInt8Field uint8Field = field as UInt8Field;
            if (uint8Field != null)
                return false;

            BoolField boolField = field as BoolField;
            if (boolField != null)
                return false;

            Debug.WriteLine("add code in CheckDataTruncatedForBool.");

            return true;
        }

        #endregion

        public static void CopyStringDataWrapper(
            DataWrapper.DataWrapper newDataWrapper,
            DataWrapper.DataWrapper oldDataWrapper)
        {
            Debug.Assert(newDataWrapper != null);
            Debug.Assert(oldDataWrapper != null);

            Debug.Assert(newDataWrapper.DataTypeInfo == oldDataWrapper.DataTypeInfo);

            if (newDataWrapper.DataTypeInfo.Dim1 > 0)
            {
                var newArrayField = newDataWrapper.Data as ArrayField;
                var oldArrayField = oldDataWrapper.Data as ArrayField;

                Debug.Assert(newArrayField != null);
                Debug.Assert(oldArrayField != null);

                int count = newArrayField.Size();

                for (int i = 0; i < count; i++)
                {
                    CopyStringField(newArrayField.fields[i].Item1, oldArrayField.fields[i].Item1);
                }

            }
            else
            {
                CopyStringField(newDataWrapper.Data, oldDataWrapper.Data);
            }

        }

        private static void CopyStringField(IField newField, IField oldField)
        {
            ICompositeField newCompositeField = newField as ICompositeField;
            ICompositeField oldCompositeField = oldField as ICompositeField;

            Debug.Assert(newCompositeField != null);
            Debug.Assert(oldCompositeField != null);

            Int32Field newLenField = newCompositeField.fields[0].Item1 as Int32Field;
            Int32Field oldLenField = oldCompositeField.fields[0].Item1 as Int32Field;
            Debug.Assert(newLenField != null);
            Debug.Assert(oldLenField != null);

            ArrayField newDataField = newCompositeField.fields[1].Item1 as ArrayField;
            ArrayField oldDataField = oldCompositeField.fields[1].Item1 as ArrayField;
            Debug.Assert(newDataField != null);
            Debug.Assert(oldDataField != null);

            int oldLength = oldLenField.value;
            int oldMaxLength = oldDataField.Size();
            int newMaxLength = newDataField.Size();

            int copyCount = oldLength;
            if (copyCount < 0)
                copyCount = 0;
            if (copyCount > oldMaxLength)
                copyCount = oldMaxLength;

            if (copyCount > newMaxLength)
                copyCount = newMaxLength;

            newLenField.value = copyCount;

            for (int i = 0; i < copyCount; i++)
            {
                var newInt8Field = newDataField.fields[i].Item1 as Int8Field;
                var oldInt8Field = oldDataField.fields[i].Item1 as Int8Field;
                Debug.Assert(newInt8Field != null);
                Debug.Assert(oldInt8Field != null);
                
                newInt8Field.value = oldInt8Field.value;
            }

        }

        public static void CopyUdtDataWrapper(DataWrapper.DataWrapper newDataWrapper,
            DataWrapper.DataWrapper oldDataWrapper)
        {
            Debug.Assert(newDataWrapper != null);
            Debug.Assert(oldDataWrapper != null);

            Debug.Assert(newDataWrapper.DataTypeInfo == oldDataWrapper.DataTypeInfo);

            if (newDataWrapper.DataTypeInfo.Dim1 > 0)
            {
                var newArrayField = newDataWrapper.Data as ArrayField;
                var oldArrayField = oldDataWrapper.Data as ArrayField;

                Debug.Assert(newArrayField != null);
                Debug.Assert(oldArrayField != null);

                int count = newArrayField.Size();

                for (int i = 0; i < count; i++)
                {
                    CopyUdtField(newArrayField.fields[i].Item1, oldArrayField.fields[i].Item1);
                }

            }
            else
            {
                CopyUdtField(newDataWrapper.Data, oldDataWrapper.Data);
            }
        }

        private static void CopyUdtField(IField newField, IField oldField)
        {
            ICompositeField newCompositeField = newField as ICompositeField;
            ICompositeField oldCompositeField = oldField as ICompositeField;

            Debug.Assert(newCompositeField != null);
            Debug.Assert(oldCompositeField != null);

            int newFieldsCount = newCompositeField.fields.Count;
            int oldFieldsCount = oldCompositeField.fields.Count;

            int copyFieldsCount = newFieldsCount < oldFieldsCount ? newFieldsCount : oldFieldsCount;

            for (int i = 0; i < copyFieldsCount; i++)
            {
                try
                {
                    newCompositeField.fields[i].Item1.Update(oldCompositeField.fields[i].Item1.ToJToken());
                }
                catch (Exception)
                {
                    //ignore
                }
                
            }
        }
    }
}
