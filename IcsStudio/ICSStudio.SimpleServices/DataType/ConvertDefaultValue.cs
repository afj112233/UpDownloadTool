using System;
using System.Diagnostics;
using System.Linq;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.PredefinedType;

namespace ICSStudio.SimpleServices.DataType
{
    public enum Type
    {
        INT8,
        INT16,
        INT32,
        INT64,
        REAL,
    }

    public partial class ValueConverter
    {
        private static Type _type = Type.INT32;

        public static string ConvertValue(string input, DisplayStyle oldStyle, DisplayStyle newStyle, int bitSize,
            Type type)
        {
            _type = type;
            input = oldStyle == DisplayStyle.Ascii
                ? input
                : FormatOp.RemoveFormat(input, oldStyle != DisplayStyle.Ascii);
            //input = FormatOp.FormatSpecial(input);
            if (oldStyle == DisplayStyle.Ascii)
            {
                if (bitSize == 8)
                {
                    input = ValueConverter.ToSByte(input, oldStyle).ToString();
                }
                else if (bitSize == 16)
                {
                    input = ValueConverter.ToShort(input, oldStyle).ToString();
                }
                else if (bitSize == 32)
                {
                    if (type == Type.REAL)
                    {
                        var bytes = ValueConverter.ToBytes(input);
                        while (bytes.Count < 4 || bytes.Count > 4)
                        {
                            if (bytes.Count < 4) bytes.Insert(0, 0);
                            if (bytes.Count > 4) bytes.Remove(bytes[bytes.Count - 1]);
                        }

                        return BitConverter.ToSingle(bytes.ToArray(), 0).ToString("g9");
                    }
                    else
                    {
                        input = ValueConverter.ToInt(input, oldStyle).ToString();
                    }
                }
                else if (bitSize == 64)
                    input = ValueConverter.ToLong(input, oldStyle).ToString();
                else
                {
                    Debug.Assert(false);
                }
            }
            else if (oldStyle == DisplayStyle.Float)
            {
                if (FormatOp.IsPositiveInfinity(input))
                {
                    input = "Infinity";
                    return input;
                }
                else if (FormatOp.IsNegativeInfinity(input))
                {
                    input = "-Infinity";
                    return input;
                }
                else
                {
                    input = ((int) double.Parse(string.IsNullOrEmpty(input) ? "0" : input)).ToString();
                }
            }
            else if (oldStyle == DisplayStyle.Exponential)
            {
                if (FormatOp.IsPositiveInfinity(input))
                {
                    input = "Infinity";
                    return input;
                }
                else if (FormatOp.IsNegativeInfinity(input))
                {
                    input = "-Infinity";
                    return input;
                }
                else
                {
                    input = ((int) double.Parse(string.IsNullOrEmpty(input) ? "0" : input)).ToString();
                }
            }
            else
            {
                input = string.IsNullOrEmpty(input) ? "0" : input;
            }

            return ConvertGenericBinary(input, ChangeStyleToByte(oldStyle),
                ChangeStyleToByte(newStyle), bitSize);
        }

        public static string ConvertGenericBinaryToAny(string input, DisplayStyle? toType, Type type, int size)
        {
            _type = type;
            switch (toType)
            {
                case DisplayStyle.Binary:
                    return input;
                case DisplayStyle.Octal:
                    return ConvertGenericBinaryFromBinary(input, 8, size);
                case DisplayStyle.Decimal:
                    return ConvertGenericBinaryFromBinary(input, 10, size);
                case DisplayStyle.Hex:
                    return ConvertGenericBinaryFromBinary(input, 16, size);
                case DisplayStyle.Ascii:
                    var str = ConvertGenericBinaryFromBinary(input, 0, size);
                    //str = FormatOp.UnFormatSpecial(str);
                    return str;
            }

            return "";
        }

        public static string ConvertGenericAnyToBinary(string input, DisplayStyle? fromType, int size)
        {
            switch (fromType)
            {
                case DisplayStyle.Binary:
                    return input;
                case DisplayStyle.Octal:
                    return ConvertGenericBinaryFromOctal(input, 2, size);
                case DisplayStyle.Decimal:
                    return ConvertGenericBinaryFromDecimal(input, 2, size);
                case DisplayStyle.Hex:
                    return ConvertGenericBinaryFromHexadecimal(input, 2, size);
            }

            return "";
        }

        public static string ConvertGenericAnyToDecimal(string input, DisplayStyle? fromType, int size)
        {
            switch (fromType)
            {
                case DisplayStyle.Binary:
                    return ConvertGenericBinaryFromBinary(input, 10, size);
                case DisplayStyle.Octal:
                    return input;
                case DisplayStyle.Decimal:
                    return ConvertGenericBinaryFromDecimal(input, 10, size);
                case DisplayStyle.Hex:
                    return ConvertGenericBinaryFromHexadecimal(input, 10, size);
            }

            return "0";
        }

        public static string IntToAscii(int value, int bitSize)
        {
            var bytes = BitConverter.GetBytes(value).Reverse().ToList();
            while (bytes.Count < bitSize/8)
            {
                bytes.Insert(0, 0);
            }

            while (bytes.Count > bitSize/8)
            {
                bytes.Remove(0);
            }

            return string.Join("",
                bytes.Select(ToStringExtensions.ToAsciiDisplay));
        }

        public static string Reverse(string reverseString)
        {
            var output = string.Empty;
            for (int i = reverseString.Length; i > 0; i--)
            {
                output += reverseString.Substring(i - 1, 1);
            }

            return FormatOp.UnFormatSpecial(output);
        }

        public static Type SelectIntType(IDataType dataType)
        {
            if (dataType is SINT)
            {
                return Type.INT8;
            }

            if (dataType is INT)
            {
                return Type.INT16;
            }

            if (dataType is DINT)
            {
                return Type.INT32;
            }

            if (dataType is LINT)
            {
                return Type.INT64;
            }

            if (dataType is REAL)
            {
                return Type.REAL;
            }

            return Type.INT32;
        }

        public static Type SelectIntType(IField field)
        {
            if (field is Int8Field)
            {
                return Type.INT8;
            }

            if (field is Int16Field)
            {
                return Type.INT16;
            }

            if (field is Int32Field)
            {
                return Type.INT32;
            }

            if (field is Int64Field)
            {
                return Type.INT64;
            }

            if (field is RealField)
            {
                return Type.REAL;
            }

            return Type.INT32;
        }

        private static byte ChangeStyleToByte(DisplayStyle style)
        {
            byte value = 10;
            switch (style)
            {
                case DisplayStyle.Binary:
                    value = 2;
                    break;
                case DisplayStyle.Octal:
                    value = 8;
                    break;
                case DisplayStyle.Decimal:
                    value = 10;
                    break;
                case DisplayStyle.Hex:
                    value = 16;
                    break;
                case DisplayStyle.Ascii:
                    value = 0;
                    break;
            }

            return value;
        }

        private static string ConvertGenericBinary(string input, byte fromType, byte toType, int size)
        {
            string output = input;
            switch (fromType)
            {
                case 0:
                    input = FormatOp.FormatSpecial(input);
                    output = ConvertGenericBinaryFromAscii(input, toType, size);
                    break;
                case 2:
                    output = ConvertGenericBinaryFromBinary(input, toType, size);
                    break;
                case 8:
                    output = ConvertGenericBinaryFromOctal(input, toType, size);
                    break;
                case 10:
                    output = ConvertGenericBinaryFromDecimal(input, toType, size);
                    break;
                case 16:
                    output = ConvertGenericBinaryFromHexadecimal(input, toType, size);
                    break;
                default:
                    break;
            }

            return output;
        }

        private static string ConvertGenericBinaryFromAscii(string input, byte toType, int size)
        {
            input = FormatOp.FormatSpecial(input);
            Int64 value = Int64.Parse(input);
            switch (toType)
            {
                case 2:
                    input = Convert.ToString(value, 2);
                    break;
                case 8:
                    input = Convert.ToString(value, 8);
                    break;
                case 10:
                    input = value.ToString();
                    break;
                case 16:
                    input = Convert.ToString(value, 16);
                    break;
                default:
                    break;
            }

            return input;
        }

        private static string ConvertGenericBinaryFromBinary(string input, byte toType, int size)
        {
            switch (toType)
            {

                case 0:
                    input = IntToAscii((int) GetIntValue(input, 2), size);
                    break;
                case 8:
                    input = Convert.ToString(GetIntValue(input, 2), 8);
                    break;
                case 10:
                    input = GetIntValue(input, 2).ToString();
                    break;
                case 16:
                    input = Convert.ToString(GetIntValue(input, 2), 16);
                    break;
                default:
                    break;
            }

            return input;
        }

        private static string ConvertGenericBinaryFromOctal(string input, byte toType, int size)
        {
            switch (toType)
            {
                case 0:
                    input = IntToAscii((int) GetIntValue(input, 8), size);
                    break;
                case 2:
                    input = Convert.ToString(GetIntValue(input, 8), 2);
                    break;
                case 10:
                    input = GetIntValue(input, 8).ToString();
                    break;
                case 16:
                    input = Convert.ToString(GetIntValue(input, 8), 16);
                    break;
                default:
                    break;
            }

            return input;
        }

        private static string ConvertGenericBinaryFromDecimal(string input, int toType, int size)
        {
            string output = "";
            int intInput = (int) Convert.ToDouble(input);
            switch (toType)
            {
                case 0:
                    output = IntToAscii((int) GetIntValue(input, 10), size);
                    break;
                case 2:
                    output = Convert.ToString(intInput, 2);
                    break;
                case 8:
                    output = Convert.ToString(intInput, 8);
                    break;
                case 16:
                    output = Convert.ToString(intInput, 16);
                    break;
                default:
                    output = input;
                    break;
            }

            return output;
        }

        private static string ConvertGenericBinaryFromHexadecimal(string input, int toType, int size)
        {
            try
            {
                switch (toType)
                {
                    case 0:
                        input = IntToAscii((int) GetIntValue(input, 16), size);
                        break;
                    case 2:
                        input = Convert.ToString(GetIntValue(input, 16), 2);
                        break;
                    case 8:
                        input = Convert.ToString(GetIntValue(input, 16), 8);
                        break;
                    case 10:
                        input = GetIntValue(input, 16).ToString();
                        break;
                    default:
                        break;
                }

                return input;
            }
            catch (Exception)
            {
                return "0";
            }
        }

        private static long GetIntValue(string value, int fromBase)
        {
            if (_type == Type.INT64)
            {
                return Convert.ToInt64(value, fromBase);
            }

            if (_type == Type.INT16)
            {
                return Convert.ToInt16(value, fromBase);
            }

            if (_type == Type.INT8)
            {
                try
                {
                    return Convert.ToSByte(value, fromBase);
                }
                catch (Exception e)
                {
                    Debug.Assert(false, e.ToString());
                    return 0;
                }
            }

            if (_type == Type.REAL)
            {

            }

            return Convert.ToInt32(value, fromBase);
        }
    }
}
