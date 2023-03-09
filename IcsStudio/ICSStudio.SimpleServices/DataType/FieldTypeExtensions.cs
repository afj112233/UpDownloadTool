using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.DataType;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace ICSStudio.SimpleServices.DataType
{
    public static class FieldTypeExtensions
    {
        #region JToken

        public static void Update(this IField field, JToken data)
        {
            ICompositeField compositeField = field as ICompositeField;
            compositeField?.Update(data);

            ArrayField arrayField = field as ArrayField;
            arrayField?.Update(data);

            BoolArrayField boolArrayField = field as BoolArrayField;
            boolArrayField?.Update(data);

            if (data is JArray)
            {
                data = data[0];
            }

            LRealField lRealField = field as LRealField;
            lRealField?.Update(data);

            RealField realField = field as RealField;
            realField?.Update(data);

            Int64Field int64Field = field as Int64Field;
            int64Field?.Update(data);

            Int32Field int32Field = field as Int32Field;
            int32Field?.Update(data);

            Int16Field int16Field = field as Int16Field;
            int16Field?.Update(data);

            Int8Field int8Field = field as Int8Field;
            int8Field?.Update(data);

            UInt64Field uint64Field = field as UInt64Field;
            uint64Field?.Update(data);

            UInt32Field uint32Field = field as UInt32Field;
            uint32Field?.Update(data);

            UInt16Field uint16Field = field as UInt16Field;
            uint16Field?.Update(data);

            UInt8Field uint8Field = field as UInt8Field;
            uint8Field?.Update(data);

            BoolField boolField = field as BoolField;
            boolField?.Update(data);
        }

        public static void Update(this ICompositeField field, JToken data)
        {
            if (field != null && data != null)
            {
                JArray arrayData = data as JArray;

                Contract.Assert(arrayData != null);
                Contract.Assert(field.fields.Count == arrayData.Count);

                int count = field.fields.Count;
                for (int i = 0; i < count; i++)
                {
                    field.fields[i].Item1.Update(arrayData[i]);
                }
            }
        }

        public static void Update(this ArrayField field, JToken data)
        {
            if (field != null && data != null)
            {
                JArray arrayData = data as JArray;

                Contract.Assert(arrayData != null);
                Contract.Assert(field.fields.Count == arrayData.Count);

                int count = field.fields.Count;
                for (int i = 0; i < count; i++)
                {
                    field.fields[i].Item1.Update(arrayData[i]);
                }
            }
        }

        public static void Update(this BoolArrayField field, JToken data)
        {
            if (field != null)
            {
                if (data != null)
                {
                    Contract.Assert(data.Type == JTokenType.Array);

                    for (int i = 0; i < field.BitCount / 32; i++)
                    {
                        field.Add(i * 32, (int)data[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < field.BitCount / 32; i++)
                    {
                        field.Add(i * 32, 0);
                    }
                }
            }
        }

        public static void Update(this LRealField field, JToken token)
        {
            JValue data = token as JValue;

            if (field != null && data != null)
            {
                if (data.Type == JTokenType.Integer)
                {
                    if (data.Value is long)
                    {
                        field.value = (double)Convert.ToInt64(data);
                        return;
                    }
                }

                if (data.Type == JTokenType.Float)
                {
                    if (data.Value is float)
                    {
                        field.value = (double)Convert.ToSingle(data);
                        return;
                    }
                }

                field.value = (double)data;
            }
        }

        public static void Update(this RealField field, JToken token)
        {
            JValue data = token as JValue;

            if (field != null && data != null)
            {
                if (data.Type == JTokenType.Integer)
                {
                    if (data.Value is long)
                    {
                        field.value = (float)Convert.ToInt64(data);
                        return;
                    }
                }

                if (data.Type == JTokenType.Float)
                {
                    if (data.Value is double)
                    {
                        field.value = (float)Convert.ToDouble(data);
                        return;
                    }
                }

                field.value = (float)data;
            }
        }

        public static void Update(this Int64Field field, JToken token)
        {
            JValue data = token as JValue;

            if (field != null && data != null)
            {
                if (data.Type == JTokenType.Float)
                {
                    if (data.Value is float)
                    {
                        field.value = Convert.ToInt64(Convert.ToSingle(data));
                        return;
                    }

                    if (data.Value is double)
                    {
                        field.value = Convert.ToInt64(Convert.ToDouble(data));
                        return;
                    }
                }

                field.value = (Int64)Convert.ToInt64(data);
            }
        }

        public static void Update(this UInt64Field field, JToken token)
        {
            JValue data = token as JValue;

            if (field != null && data != null)
            {
                if (data.Type == JTokenType.Float)
                {
                    if (data.Value is float)
                    {
                        field.value = (UInt64)Convert.ToSingle(data);
                        return;
                    }

                    if (data.Value is double)
                    {
                        field.value = (UInt64)Convert.ToDouble(data);
                        return;
                    }
                }

                field.value = (UInt64)data;
            }
        }

        public static void Update(this Int32Field field, JToken token)
        {
            JValue data = token as JValue;

            if (field != null && data != null)
            {
                if (data.Type == JTokenType.Float)
                {
                    if (data.Value is float)
                    {
                        float float0 = Convert.ToSingle(data);
                        double double0 = float0;

                        if (double0 >= 2147483647.5)
                        {
                            field.value = Int32.MaxValue;
                        }
                        else if (double0 <= -2147483648.5)
                        {
                            field.value = Int32.MinValue;
                        }
                        else
                        {
                            field.value = Convert.ToInt32(float0);
                        }

                        return;
                    }

                    if (data.Value is double)
                    {
                        field.value = Convert.ToInt32(Convert.ToDouble(data));
                        return;
                    }
                }

                field.value = (Int32)Convert.ToInt64(data);
            }
        }

        public static void Update(this UInt32Field field, JToken token)
        {
            JValue data = token as JValue;

            if (field != null && data != null)
            {
                if (data.Type == JTokenType.Float)
                {
                    if (data.Value is float)
                    {
                        field.value = (UInt32)Convert.ToSingle(data);
                        return;
                    }

                    if (data.Value is double)
                    {
                        field.value = (UInt32)Convert.ToDouble(data);
                        return;
                    }
                }

                field.value = (UInt32)data;
            }
        }

        public static void Update(this Int16Field field, JToken token)
        {
            JValue data = token as JValue;

            if (field != null && data != null)
            {
                if (data.Type == JTokenType.Float)
                {
                    if (data.Value is float)
                    {
                        float float0 = Convert.ToSingle(data);
                        double double0 = float0;
                        if (double0 >= 32767.5)
                        {
                            field.value = Int16.MaxValue;
                        }
                        else if (double0 < -32768.5)
                        {
                            field.value = Int16.MinValue;
                        }
                        else
                        {
                            field.value = Convert.ToInt16(float0);
                        }

                        return;
                    }

                    if (data.Value is double)
                    {
                        field.value = Convert.ToInt16(Convert.ToDouble(data));
                        return;
                    }
                }

                field.value = (Int16)Convert.ToInt64(data);
            }
        }

        public static void Update(this UInt16Field field, JToken token)
        {
            JValue data = token as JValue;

            if (field != null && data != null)
            {
                if (data.Type == JTokenType.Float)
                {
                    if (data.Value is float)
                    {
                        field.value = (UInt16)Convert.ToSingle(data);
                        return;
                    }

                    if (data.Value is double)
                    {
                        field.value = (UInt16)Convert.ToDouble(data);
                        return;
                    }
                }

                field.value = (UInt16)data;
            }
        }

        public static void Update(this Int8Field field, JToken token)
        {
            JValue data = token as JValue;

            if (field != null && data != null)
            {
                if (data.Type == JTokenType.Float)
                {
                    if (data.Value is float)
                    {
                        float float0 = Convert.ToSingle(data);
                        double double0 = float0;
                        if (double0 >= 127.5)
                        {
                            field.value = SByte.MaxValue;
                        }
                        else if (double0 < -128.5)
                        {
                            field.value = SByte.MinValue;
                        }
                        else
                        {
                            field.value = Convert.ToSByte(float0);
                        }

                        return;
                    }

                    if (data.Value is double)
                    {
                        field.value = Convert.ToSByte(Convert.ToDouble(data));
                        return;
                    }
                }

                field.value = (sbyte)Convert.ToInt64(data);
            }
        }

        public static void Update(this UInt8Field field, JToken token)
        {
            JValue data = token as JValue;

            if (field != null && data != null)
            {
                if (data.Type == JTokenType.Float)
                {
                    if (data.Value is float)
                    {
                        field.value = (byte)Convert.ToSingle(data);
                        return;
                    }

                    if (data.Value is double)
                    {
                        field.value = (byte)Convert.ToDouble(data);
                        return;
                    }
                }

                field.value = (byte)data;
            }
        }

        public static void Update(this BoolField field, JToken token)
        {
            JValue data = token as JValue;

            if (field != null && data != null)
            {

                byte temp = 0;

                if (data.Type == JTokenType.Float)
                {
                    if (data.Value is float)
                    {
                        var s = Convert.ToSingle(data);
                        temp = s == 0 ? (byte)0 : (byte)1;
                    }

                    if (data.Value is double)
                    {
                        var d = Convert.ToByte(Convert.ToDouble(data));
                        temp = d == 0 ? (byte)0 : (byte)1;
                    }

                    field.value = temp;
                    return;
                }


                if (data.Type == JTokenType.Integer)
                {
                    temp = (byte)Convert.ToInt64(data);

                    if (temp > 1)
                        temp = 1;

                    field.value = temp;
                    return;
                }

                throw new NotImplementedException("BoolField add handle here!");
            }
        }

        #endregion

        #region byte[]

        public static void Update(this IField field, byte[] dataBytes, int index)
        {
            ICompositeField compositeField = field as ICompositeField;
            compositeField?.Update(dataBytes, index);


            ArrayField arrayField = field as ArrayField;
            arrayField?.Update(dataBytes, index);

            BoolArrayField boolArrayField = field as BoolArrayField;
            boolArrayField?.Update(dataBytes, index);

            LRealField lRealField = field as LRealField;
            lRealField?.Update(dataBytes, index);

            RealField realField = field as RealField;
            realField?.Update(dataBytes, index);

            Int64Field int64Field = field as Int64Field;
            int64Field?.Update(dataBytes, index);

            Int32Field int32Field = field as Int32Field;
            int32Field?.Update(dataBytes, index);

            Int16Field int16Field = field as Int16Field;
            int16Field?.Update(dataBytes, index);

            Int8Field int8Field = field as Int8Field;
            int8Field?.Update(dataBytes, index);

            //TODO(gjc): add UInt64Field, UInt32Field, UInt16Field, UInt8Field

            BoolField boolField = field as BoolField;
            boolField?.Update(dataBytes, index);
        }

        public static void Update(this ICompositeField field, byte[] dataBytes, int index)
        {
            if (field != null && dataBytes != null)
            {
                int count = field.fields.Count;
                for (int i = 0; i < count; i++)
                {
                    field.fields[i].Item1.Update(dataBytes, index + field.fields[i].Item2);
                }
            }
        }

        public static void Update(this ArrayField field, byte[] dataBytes, int index)
        {
            if (field != null && dataBytes != null)
            {
                int count = field.fields.Count;
                for (int i = 0; i < count; i++)
                {
                    field.fields[i].Item1.Update(dataBytes, index + field.fields[i].Item2);
                }
            }
        }

        public static void Update(this BoolArrayField field, byte[] dataBytes, int index)
        {
            if (field != null && dataBytes != null)
            {
                for (int i = 0; i < field.BitCount / 32; i++)
                {
                    field.Add(i * 32, BitConverter.ToInt32(dataBytes, index + i * 4));
                }
            }
        }

        public static void Update(this LRealField field, byte[] dataBytes, int index)
        {
            if (field != null && dataBytes != null)
            {
                field.value = BitConverter.ToDouble(dataBytes, index);
            }
        }

        public static void Update(this RealField field, byte[] dataBytes, int index)
        {
            if (field != null && dataBytes != null)
            {
                field.value = BitConverter.ToSingle(dataBytes, index);
            }
        }

        public static void Update(this Int64Field field, byte[] dataBytes, int index)
        {
            if (field != null && dataBytes != null)
            {
                field.value = BitConverter.ToInt64(dataBytes, index);
            }
        }

        public static void Update(this Int32Field field, byte[] dataBytes, int index)
        {
            if (field != null && dataBytes != null)
            {
                field.value = BitConverter.ToInt32(dataBytes, index);
            }
        }

        public static void Update(this Int16Field field, byte[] dataBytes, int index)
        {
            if (field != null && dataBytes != null)
            {
                field.value = BitConverter.ToInt16(dataBytes, index);
            }
        }

        public static void Update(this Int8Field field, byte[] dataBytes, int index)
        {
            if (field != null && dataBytes != null)
            {
                field.value = (sbyte)dataBytes[index];
            }
        }

        public static void Update(this BoolField field, byte[] dataBytes, int index)
        {
            if (field != null && dataBytes != null)
            {
                byte value = dataBytes[index];
                Contract.Assert(value == 0 || value == 1);
                field.value = value;
            }
        }

        #endregion

        #region String

        public static void Update(this IField field, string data)
        {
            Int16Field int16Field = field as Int16Field;
            if (int16Field != null)
            {
                int16Field.Update(data);
                return;
            }


            RealField realField = field as RealField;
            if (realField != null)
            {
                realField.Update(data);
                return;
            }

            Int8Field int8Field = field as Int8Field;
            if (int8Field != null)
            {
                int8Field.Update(data);
                return;
            }

            Int32Field int32Field = field as Int32Field;
            if (int32Field != null)
            {
                int32Field.Update(data);
                return;
            }

            throw new Exception("Add other type handle!");
        }

        public static void Update(this Int16Field field, string data)
        {
            Int16 int16;
            UInt16 uint16;
            if (Int16.TryParse(data, out int16))
            {
                field.value = int16;
            }
            else if (UInt16.TryParse(data, out uint16))
            {
                field.value = (Int16)uint16;
            }
        }

        public static void Update(this RealField field, string data)
        {
            float floatValue;
            double doubleValue;
            if (float.TryParse(data, out floatValue))
            {
                field.value = floatValue;
            }
            else if (double.TryParse(data, out doubleValue))
            {
                field.value = (float)doubleValue;
            }
        }

        public static void Update(this Int8Field field, string data)
        {
            byte value0;
            sbyte value1;
            if (sbyte.TryParse(data, out value1))
            {
                field.value = value1;
            }
            else if (byte.TryParse(data, out value0))
            {
                field.value = (sbyte)value0;
            }
        }

        public static void Update(this Int32Field field, string data)
        {
            Int32 int32;
            UInt32 uint32;
            if (Int32.TryParse(data, out int32))
            {
                field.value = int32;
            }
            else if (UInt32.TryParse(data, out uint32))
            {
                field.value = (Int32)uint32;
            }
        }

        public static string GetString(this IField field)
        {
            Int8Field int8Field = field as Int8Field;
            if (int8Field != null)
                return int8Field.GetString();

            Int16Field int16Field = field as Int16Field;
            if (int16Field != null)
                return int16Field.GetString();

            Int32Field int32Field = field as Int32Field;
            if (int32Field != null)
                return int32Field.GetString();

            Int64Field int64Field = field as Int64Field;
            if (int64Field != null)
                return int64Field.GetString();

            RealField realField = field as RealField;
            if (realField != null)
                return realField.GetString();

            BoolField boolField = field as BoolField;
            if (boolField != null)
                return boolField.GetString();

            UInt8Field uint8Field = field as UInt8Field;
            if (uint8Field != null)
                return uint8Field.GetString();

            UInt32Field uint32Field = field as UInt32Field;
            if (uint32Field != null)
                return uint32Field.GetString();

            throw new Exception("Add other type handle!");
        }

        public static string GetString(this BoolField field)
        {
            return field.value.ToString();
        }

        public static string GetString(this Int8Field field)
        {
            return field.value.ToString();
        }

        public static string GetString(this UInt8Field field)
        {
            return field.value.ToString();
        }

        public static string GetString(this Int16Field field)
        {
            return field.value.ToString();
        }

        public static string GetString(this Int32Field field)
        {
            return field.value.ToString();
        }

        public static string GetString(this UInt32Field field)
        {
            return field.value.ToString();
        }

        public static string GetString(this Int64Field field)
        {
            return field.value.ToString();
        }

        public static string GetString(this RealField field)
        {
            return ToString(field.value, "Float");
        }

        public static string ToString(float value, string format)
        {
            string result =
                value.ToString("r", CultureInfo.InvariantCulture)
                    .ToLower(CultureInfo.InvariantCulture);
            if (result.Contains('e'))
                result = value.ToString("e9", CultureInfo.InvariantCulture);

            // rm003T, page853
            if (float.IsNaN(value))
                return "1.#QNAN";

            if (float.IsPositiveInfinity(value))
            {
                switch (format)
                {
                    case "Exponential":
                        return "1.#INF0000e+000";
                    case "Float":
                        return "1.$";
                }
            }

            if (float.IsNegativeInfinity(value))
            {
                switch (format)
                {
                    case "Exponential":
                        return "-1.#INF0000e+000";
                    case "Float":
                        return "-1.$";
                }
            }

            switch (format)
            {
                case "Exponential":
                    if (result.Contains('e'))
                        return result;

                    return value.ToString("e8", CultureInfo.InvariantCulture);

                case "Float":
                    if (!result.Contains('.') && !result.Contains("e"))
                        result += ".0";
                    if (value >= 10e9)
                        result = value.ToString("e9");
                    return result;

                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
                }
            }
        }

        //
        public static string GetString(this IField field, DisplayStyle displayStyle)
        {
            Int8Field int8Field = field as Int8Field;
            if (int8Field != null)
                return int8Field.GetString(displayStyle);

            Int16Field int16Field = field as Int16Field;
            if (int16Field != null)
                return int16Field.GetString(displayStyle);

            Int32Field int32Field = field as Int32Field;
            if (int32Field != null)
                return int32Field.GetString(displayStyle);

            Int64Field int64Field = field as Int64Field;
            if (int64Field != null)
                return int64Field.GetString(displayStyle);

            RealField realField = field as RealField;
            if (realField != null)
                return realField.GetString(displayStyle);

            BoolField boolField = field as BoolField;
            if (boolField != null)
                return boolField.GetString(displayStyle);

            throw new Exception("Add other type handle!");
        }

        public static string GetString(this BoolField field, DisplayStyle displayStyle)
        {
            return field.value.ToString(displayStyle);
        }

        public static string GetString(this Int8Field field, DisplayStyle displayStyle)
        {
            return field.value.ToString(displayStyle);
        }

        public static string GetString(this Int16Field field, DisplayStyle displayStyle)
        {
            return field.value.ToString(displayStyle);
        }

        public static string GetString(this Int32Field field, DisplayStyle displayStyle)
        {
            return field.value.ToString(displayStyle);
        }

        public static string GetString(this Int64Field field, DisplayStyle displayStyle)
        {
            return field.value.ToString(displayStyle);
        }

        public static string GetString(this RealField field, DisplayStyle displayStyle)
        {
            return field.value.ToString(displayStyle);
        }

        #endregion

        #region Bit

        public static bool GetBitValue(this IField field, int bitOffset)
        {
            Int8Field int8Field = field as Int8Field;
            if (int8Field != null)
            {
                return BitValue.Get(int8Field.value, bitOffset);
            }

            UInt8Field uint8Field = field as UInt8Field;
            if (uint8Field != null)
            {
                return BitValue.Get(uint8Field.value, bitOffset);
            }

            Int16Field int16Field = field as Int16Field;
            if (int16Field != null)
            {
                return BitValue.Get(int16Field.value, bitOffset);
            }

            Int32Field int32Field = field as Int32Field;
            if (int32Field != null)
            {
                return BitValue.Get(int32Field.value, bitOffset);
            }

            Int64Field int64Field = field as Int64Field;
            if (int64Field != null)
            {
                return BitValue.Get(int64Field.value, bitOffset);
            }

            BoolArrayField boolArrayField = field as BoolArrayField;
            if (boolArrayField != null)
            {
                return boolArrayField.Get(bitOffset);
            }

            BoolField boolField = field as BoolField;
            if (boolField != null)
            {
                if (bitOffset > 0) throw new Exception("Error bitOffset!");
                return boolField.value == 1;
            }

            throw new Exception("Add other type handle!");
        }

        public static void SetBitValue(this IField field, int bitOffset, bool value)
        {
            Int8Field int8Field = field as Int8Field;
            if (int8Field != null)
            {
                int8Field.value = BitValue.Set(int8Field.value, bitOffset, value);
                return;
            }

            UInt8Field uint8Field = field as UInt8Field;
            if (uint8Field != null)
            {
                uint8Field.value = BitValue.Set(uint8Field.value, bitOffset, value);
                return;
            }

            Int16Field int16Field = field as Int16Field;
            if (int16Field != null)
            {
                int16Field.value = BitValue.Set(int16Field.value, bitOffset, value);
                return;
            }

            Int32Field int32Field = field as Int32Field;
            if (int32Field != null)
            {
                int32Field.value = BitValue.Set(int32Field.value, bitOffset, value);
                return;
            }

            BoolArrayField boolArrayField = field as BoolArrayField;
            if (boolArrayField != null)
            {
                boolArrayField.Set(bitOffset, value);
                return;
            }

            BoolField boolField = field as BoolField;
            if (boolField != null)
            {
                if (bitOffset > 0) throw new Exception("Error bitOffset!");
                boolField.value = (byte)(value ? 1 : 0);
                return;
            }

            throw new Exception("Add other type handle!");
        }

        #endregion
    }
}
