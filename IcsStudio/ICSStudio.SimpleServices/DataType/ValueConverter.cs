using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Exceptions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ICSStudio.SimpleServices.DataType
{
    public partial class ValueConverter
    {
        public static List<byte> ToBytes(string value)
        {
            string input;
            if (value.StartsWith("'") && value.EndsWith("'") && value.Length >= 2)
                input = value.Substring(1, value.Length - 2);
            else
                input = value;

            var byteList = new List<byte>();

            for (int i = 0; i < input.Length;)
            {
                int left = input.Length - i;
                if (input[i] == '\'')
                {
                    throw new ExtraCharactersException();
                }

                if (input[i] == '$')
                {
                    if (left == 1)
                    {
                        throw new MissCharacterException();
                    }

                    if (input[i + 1] == 'n' || input[i + 1] == 'N')
                    {
                        byteList.Add(13); // $r
                        byteList.Add(10); // $l
                        i += 2;
                        continue;
                    }

                    if (input[i + 1] == 't' || input[i + 1] == 'T')
                    {
                        byteList.Add(9);
                        i += 2;
                        continue;
                    }

                    if (input[i + 1] == 'l' || input[i + 1] == 'L')
                    {
                        byteList.Add(10);
                        i += 2;
                        continue;
                    }

                    if (input[i + 1] == 'p' || input[i + 1] == 'P')
                    {
                        byteList.Add(12);
                        i += 2;
                        continue;
                    }

                    if (input[i + 1] == 'r' || input[i + 1] == 'R')
                    {
                        byteList.Add(13);
                        i += 2;
                        continue;
                    }

                    if (input[i + 1] == '$')
                    {
                        byteList.Add(36);
                        i += 2;
                        continue;
                    }

                    if (input[i + 1] == '\'')
                    {
                        byteList.Add(39);
                        i += 2;
                        continue;
                    }

                    if (left >= 3)
                    {
                        try
                        {
                            var temp = Convert.ToByte(input.Substring(i + 1, 2), 16);
                            byteList.Add(temp);
                            i += 3;
                            continue;
                        }
                        catch (Exception)
                        {
                            throw new InvalidCharacterCombinationException();
                        }

                    }

                    // left == 2
                    throw new InvalidCharacterCombinationException();
                }

                if (input[i] >= 32 && input[i] <= 126)
                {
                    byteList.Add((byte) input[i]);
                    i++;
                }
                else
                    throw new InvalidCharacterException();

            }

            return byteList;
        }

        public static bool ToBoolean(string value)
        {
            var last = value[value.Length - 1];

            if (last == '0')
                return false;

            if (last == '1')
                return true;

            throw new FormatException("Format String can be only 0 or 1.");
        }

        public static float ToFloat(string value)
        {
            if ("1.$".Equals(value))
            {
                return float.PositiveInfinity;
            }

            if ("-1.$".Equals(value))
            {
                return float.NegativeInfinity;
            }

            if ("1.#QNAN".Equals(value))
            {
                return float.NaN;
            }

            if ("-1.#QNAN".Equals(value))
            {
                return float.NaN;
            }

            var result = float.Parse(value);
            return result;
        }

        public static byte ToByte(string value, DisplayStyle displayStyle)
        {
            if (displayStyle == DisplayStyle.Ascii)
            {
                var bytes = ValueConverter.ToBytes(value);

                return bytes[0];
            }

            if (value.StartsWith("2#"))
            {
                value = value.Substring(2, value.Length - 2);
                value = value.Replace("_", "");

                return Convert.ToByte(value, 2);
            }

            if (value.StartsWith("8#"))
            {
                value = value.Substring(2, value.Length - 2);
                value = value.Replace("_", "");

                return Convert.ToByte(value, 8);
            }

            if (value.StartsWith("16#"))
            {
                value = value.Substring(3, value.Length - 3);
                value = value.Replace("_", "");

                return Convert.ToByte(value, 16);
            }

            if (displayStyle == DisplayStyle.Binary)
            {
                return Convert.ToByte(value, 2);
            }

            if (displayStyle == DisplayStyle.Octal)
            {
                return Convert.ToByte(value, 8);
            }

            if (displayStyle == DisplayStyle.Hex)
            {
                return Convert.ToByte(value, 16);
            }

            return byte.Parse(value);
        }

        public static sbyte ToSByte(string value, DisplayStyle displayStyle)
        {
            if (displayStyle == DisplayStyle.Ascii)
            {
                var bytes = ValueConverter.ToBytes(value);

                return (sbyte) bytes[0];
            }

            if (value.StartsWith("2#"))
            {
                value = value.Substring(2, value.Length - 2);
                value = value.Replace("_", "");

                return (sbyte) Convert.ToByte(value, 2);
            }

            if (value.StartsWith("8#"))
            {
                value = value.Substring(2, value.Length - 2);
                value = value.Replace("_", "");

                return (sbyte) Convert.ToByte(value, 8);
            }

            if (value.StartsWith("16#"))
            {
                value = value.Substring(3, value.Length - 3);
                value = value.Replace("_", "");

                return (sbyte) Convert.ToByte(value, 16);
            }

            if (displayStyle == DisplayStyle.Binary)
            {
                return (sbyte) Convert.ToByte(value, 2);
            }

            if (displayStyle == DisplayStyle.Octal)
            {
                return (sbyte) Convert.ToByte(value, 8);
            }

            if (displayStyle == DisplayStyle.Hex)
            {
                return (sbyte) Convert.ToByte(value, 16);
            }

            return sbyte.Parse(value);
        }

        public static short ToShort(string value, DisplayStyle displayStyle)
        {
            if (displayStyle == DisplayStyle.Ascii)
            {
                var bytes = ValueConverter.ToBytes(value);

                while (bytes.Count < 2) bytes.Add(0);

                return (short) (bytes[0] * 256 + bytes[1]);
            }

            if (value.StartsWith("2#"))
            {
                value = value.Substring(2, value.Length - 2);
                value = value.Replace("_", "");

                return Convert.ToInt16(value, 2);
            }

            if (value.StartsWith("8#"))
            {
                value = value.Substring(2, value.Length - 2);
                value = value.Replace("_", "");

                return Convert.ToInt16(value, 8);
            }

            if (value.StartsWith("16#"))
            {
                value = value.Substring(3, value.Length - 3);
                value = value.Replace("_", "");

                return Convert.ToInt16(value, 16);
            }

            if (displayStyle == DisplayStyle.Binary)
            {
                return Convert.ToInt16(value, 2);
            }

            if (displayStyle == DisplayStyle.Octal)
            {
                return Convert.ToInt16(value, 8);
            }

            if (displayStyle == DisplayStyle.Hex)
            {
                return Convert.ToInt16(value, 16);
            }

            return short.Parse(value);
        }

        public static int ToInt(string value, DisplayStyle displayStyle)
        {
            if (displayStyle == DisplayStyle.Ascii)
            {
                var bytes = ValueConverter.ToBytes(value);

                while (bytes.Count < 4) bytes.Add(0);

                int result = 0;
                foreach (var t in bytes) result = result * 256 + t;

                return result;
            }

            if (value.StartsWith("2#"))
            {
                value = value.Substring(2, value.Length - 2);
                value = value.Replace("_", "");

                return Convert.ToInt32(value, 2);
            }

            if (value.StartsWith("8#"))
            {
                value = value.Substring(2, value.Length - 2);
                value = value.Replace("_", "");

                return Convert.ToInt32(value, 8);
            }

            if (value.StartsWith("16#"))
            {
                value = value.Substring(3, value.Length - 3);
                value = value.Replace("_", "");

                return Convert.ToInt32(value, 16);
            }

            if (displayStyle == DisplayStyle.Binary)
            {
                return Convert.ToInt32(value, 2);
            }

            if (displayStyle == DisplayStyle.Octal)
            {
                return Convert.ToInt32(value, 8);
            }

            if (displayStyle == DisplayStyle.Hex)
            {
                return Convert.ToInt32(value, 16);
            }

            return int.Parse(value);
        }

        public static long ToLong(string value, DisplayStyle displayStyle)
        {
            if (displayStyle == DisplayStyle.Ascii)
            {
                var bytes = ValueConverter.ToBytes(value);

                while (bytes.Count < 8) bytes.Add(0);

                long result = 0;
                foreach (var t in bytes) result = result * 256 + t;

                return result;
            }

            if (value.StartsWith("2#"))
            {
                value = value.Substring(2, value.Length - 2);
                value = value.Replace("_", "");

                return Convert.ToInt64(value, 2);
            }

            if (value.StartsWith("8#"))
            {
                value = value.Substring(2, value.Length - 2);
                value = value.Replace("_", "");

                return Convert.ToInt64(value, 8);
            }

            if (value.StartsWith("16#"))
            {
                value = value.Substring(3, value.Length - 3);
                value = value.Replace("_", "");

                return Convert.ToInt64(value, 16);
            }

            if (displayStyle == DisplayStyle.Binary)
            {
                return Convert.ToInt64(value, 2);
            }

            if (displayStyle == DisplayStyle.Octal)
            {
                return Convert.ToInt64(value, 8);
            }

            if (displayStyle == DisplayStyle.Hex)
            {
                return Convert.ToInt64(value, 16);
            }

            return long.Parse(value);
        }

        public static void CheckValueOverflow(DisplayStyle? style, string defaultValue, ref int flag, double max)
        {
            //if (string.IsNullOrEmpty(Name)) return;
            try
            {
                if (style == DisplayStyle.Binary)
                {
                    Regex regex = new Regex("^[0-1]*$");
                    if (flag == 0 && !regex.IsMatch(defaultValue)) flag = 2;
                    else if (Convert.ToInt32(defaultValue, 2) < -(max + 1) ||
                             Convert.ToInt32(defaultValue, 2) > max) flag = 3;
                }
                else if (style == DisplayStyle.Octal)
                {
                    Regex regex = new Regex("^[0-7]*$");
                    if (flag == 0 && !regex.IsMatch(defaultValue)) flag = 2;
                    else if (Convert.ToInt32(defaultValue, 8) < -(max + 1) ||
                             Convert.ToInt32(defaultValue, 8) > max) flag = 3;
                }
                else if (style == DisplayStyle.Decimal)
                {
                    Regex regex = new Regex("^(-)?[0-9]*$");
                    if (flag == 0 && !regex.IsMatch(defaultValue)) flag = 2;
                    else if (double.Parse(defaultValue) < -(max + 1) ||
                             double.Parse(defaultValue) > max) flag = 3;
                }
                else if (style == DisplayStyle.Hex)
                {
                    Regex regex = new Regex("^[0-9A-Fa-f]*$");
                    if (flag == 0 && !regex.IsMatch(defaultValue)) flag = 2;
                    else if (Convert.ToInt32(defaultValue, 16) < -(max + 1) ||
                             Convert.ToInt32(defaultValue, 16) > max) flag = 3;
                }
                else if (style == DisplayStyle.Ascii)
                {
                    //Int64 value =
                    //    ConvertDefaultValue.AsciiToInt(defaultValue ?? "0", (int) Math.Log(max + 1, 2) + 1);
                    //var bytes = ValueConverter.ToBytes(defaultValue);
                    long value = 0;
                    if (Math.Abs(max - 127) < float.Epsilon)
                    {
                        value = ValueConverter.ToSByte(defaultValue, DisplayStyle.Ascii);
                    }
                    else if (Math.Abs(max - 32767) < float.Epsilon)
                    {
                        value = ValueConverter.ToShort(defaultValue, DisplayStyle.Ascii);
                    }
                    else if (Math.Abs(max - Int32.MaxValue) < float.Epsilon)
                    {
                        value = ValueConverter.ToInt(defaultValue, DisplayStyle.Ascii);
                    }
                    else if (Math.Abs(max - Int64.MaxValue) < float.Epsilon)
                    {
                        value = ValueConverter.ToLong(defaultValue, DisplayStyle.Ascii);
                    }

                    //int result = 0;
                    //foreach (var t in bytes) result = result * 256 + t;
                    if (value < -(max + 1) || value > max) flag = 3;
                    //var value = ValueConverter.ToInt(defaultValue, DisplayStyle.Ascii);
                    //if (value < -(max + 1) || value > max) flag = 3;
                }
                else if (style == DisplayStyle.Float || style == DisplayStyle.Exponential)
                {
                    float result = 0;
                    if ("1.$".Equals(defaultValue) || "-1.$".Equals(defaultValue))
                    {
                        flag = 0;
                        return;
                    }
                    var r = float.TryParse(defaultValue, out result);
                    if (!r)
                    {
                        flag = 3;
                    }
                }
            }
            catch (Exception)
            {
                flag = 3;
            }
        }
    }
}
