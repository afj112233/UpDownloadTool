using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.PredefinedType;

namespace ICSStudio.SimpleServices.DataType
{
    public class FormatOp
    {
        public static string ConvertMemberName(string name)
        {
            return name.Replace(":SINT", "").Replace(":INT", "").Replace(":DINT", "")
                .Replace(":LINT", "");
        }

        public static string RemoveFormat(string value, bool isRemoveFrontZero = true)
        {
            if (string.IsNullOrEmpty(value)) return value;
            if (value.Length > 1 && value[0] == '\'' && value[value.Length - 1] == '\'')
            {
                value = value.Remove(value.Length - 1, 1).Remove(0, 1);
                isRemoveFrontZero = false;
            }

            if (Regex.IsMatch(value, @"[0-9]+\.[0-9]+([Ee](\+|\-)?[0-9]+)?")) isRemoveFrontZero = false;
            if (isRemoveFrontZero)
                return RemoveFrontZero(value.Replace("_", "").Replace("2#", "").Replace("8#", "").Replace("16#", ""));
            else
                return value.Replace("_", "").Replace("2#", "").Replace("8#", "").Replace("16#", "");
        }

        public static string FormatValue(string value, DisplayStyle displayStyle, IDataType dataType)
        {
            value = RemoveFormat(value, displayStyle != DisplayStyle.Ascii);
            string convertedStr = "";
            if (dataType is BOOL)
            {
                switch (displayStyle)
                {
                    case DisplayStyle.Binary:
                        return $"2#{value}";

                    case DisplayStyle.Octal:
                        return $"8#{value}";

                    case DisplayStyle.Decimal:
                        return value;

                    case DisplayStyle.Hex:
                        return $"16#{value}";

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (dataType is SINT)
            {
                string temp;
                const int bitSize = sizeof(byte) * 8;

                switch (displayStyle)
                {
                    case DisplayStyle.Binary:
                        temp = value.PadLeft(bitSize, '0');
                        temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{4})+$))", "_");
                        return "2#" + temp;

                    case DisplayStyle.Octal:
                        temp = value.PadLeft(bitSize / 3 + 1, '0');
                        temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{3})+$))", "_");
                        return "8#" + temp;

                    case DisplayStyle.Decimal:
                        return value.ToString();

                    case DisplayStyle.Hex:
                        temp = value.PadLeft(bitSize / 4, '0');
                        temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{4})+$))", "_");
                        return "16#" + temp;

                    case DisplayStyle.Ascii:
                        return $"'{value}'";
                    case DisplayStyle.NullStyle:
                        return value;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (dataType is INT)
            {
                string temp;
                const int bitSize = sizeof(short) * 8;

                switch (displayStyle)
                {
                    case DisplayStyle.Binary:
                        temp = value.PadLeft(bitSize, '0');
                        temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{4})+$))", "_");
                        return "2#" + temp;

                    case DisplayStyle.Octal:
                        temp = value.PadLeft(bitSize / 3 + 1, '0');
                        temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{3})+$))", "_");
                        return "8#" + temp;

                    case DisplayStyle.Decimal:
                        return value.ToString();

                    case DisplayStyle.Hex:
                        temp = value.PadLeft(bitSize / 4, '0');
                        temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{4})+$))", "_");
                        return "16#" + temp;

                    case DisplayStyle.Ascii:
                        return $"'{value}'";
                    case DisplayStyle.NullStyle:
                        return value;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (dataType is DINT)
            {
                string temp;
                const int bitSize = sizeof(int) * 8;

                switch (displayStyle)
                {
                    case DisplayStyle.Binary:
                        temp = value.PadLeft(bitSize, '0');
                        temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{4})+$))", "$1_");
                        return "2#" + temp;

                    case DisplayStyle.Octal:
                        temp = value.PadLeft(bitSize / 3 + 1, '0');
                        temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{3})+$))", "$1_");
                        return "8#" + temp;

                    case DisplayStyle.Decimal:
                        return value.ToString();

                    case DisplayStyle.Hex:
                        temp = value.PadLeft(bitSize / 4, '0');
                        temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{4})+$))", "$1_");
                        return "16#" + temp;

                    case DisplayStyle.Ascii:
                        return $"'{value}'";
                    case DisplayStyle.NullStyle:
                        return value;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (dataType is LINT)
            {
                string temp;
                const int bitSize = sizeof(long) * 8;

                switch (displayStyle)
                {
                    case DisplayStyle.Binary:
                        temp = value.PadLeft(bitSize, '0');
                        temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{4})+$))", "$1_");
                        return "2#" + temp;

                    case DisplayStyle.Octal:
                        temp = value.PadLeft(bitSize / 3 + 1, '0');
                        temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{3})+$))", "$1_");
                        return "8#" + temp;

                    case DisplayStyle.Decimal:
                        return value.ToString();

                    case DisplayStyle.Hex:
                        temp = value.PadLeft(bitSize / 4, '0');
                        temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{4})+$))", "$1_");
                        return "16#" + temp;

                    case DisplayStyle.Ascii:
                        return $"'{value}'";
                    case DisplayStyle.NullStyle:
                        return value;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (dataType is REAL)
            {
                switch (displayStyle)
                {
                    case DisplayStyle.Exponential:
                        if (IsPositiveInfinity(value))
                            return "1.#INF0000e+000";
                        else if (IsNegativeInfinity(value))
                            return "-1.#INF0000e+000";
                        return (float.Parse(value)).ToString(displayStyle);
                    case DisplayStyle.Float:
                        if (IsPositiveInfinity(value))
                            return "1.$";
                        else if (IsNegativeInfinity(value))
                            return "-1.$";
                        else
                        {
                            if (string.IsNullOrEmpty(value))
                            {
                                value = float.Parse("0").ToString(displayStyle);
                            }
                            
                            if (value.Equals("1.#QNAN"))
                            {
                                return "1.#QNAN";
                            }
                            else
                            {
                                return (float.Parse(value)).ToString(displayStyle);
                            }
                        }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(displayStyle), displayStyle, null);
                }
            }else if (dataType.FamilyType == FamilyType.StringFamily)
            {
                convertedStr = $"'{UnFormatSpecial(value)}'";
            }

            return convertedStr;
        }

        public static string ConvertField(IField dataField, DisplayStyle displayStyle, int index = -1)
        {
            var boolArray = dataField as BoolArrayField;
            if (boolArray != null)
            {
                if (index < 0) return null;
                var boolValue = boolArray.Get(index);
                return boolValue.ToString(displayStyle);
            }

            var boolField = dataField as BoolField;
            if (boolField != null)
            {
                var boolValue = ValueConverter.ToBoolean(boolField.value.ToString());
                return boolValue.ToString(displayStyle);
            }

            var int8Field = dataField as Int8Field;
            if (int8Field != null)
            {
                //TODO(gjc): need edit here, byte->sbyte
                if (index > -1) return int8Field.GetBitValue(index).ToString(displayStyle);
                return int8Field.value.ToString(displayStyle);
            }

            var int16Field = dataField as Int16Field;
            if (int16Field != null)
            {
                if (index > -1) return int16Field.GetBitValue(index).ToString(displayStyle);
                return int16Field.value.ToString(displayStyle);

            }

            var int32Field = dataField as Int32Field;
            if (int32Field != null)
            {
                if (index > -1) return int32Field.GetBitValue(index).ToString(displayStyle);
                return int32Field.value.ToString(displayStyle);

            }

            var int64Field = dataField as Int64Field;
            if (int64Field != null)
            {
                if (index > -1) return int64Field.GetBitValue(index).ToString(displayStyle);
                return int64Field.value.ToString(displayStyle);

            }

            var realField = dataField as RealField;
            if (realField != null)
            {
                Debug.Assert(index == -1);
                var floatValue = ValueConverter.ToFloat(realField.value.ToString("g9"));
                return floatValue.ToString(displayStyle);
            }

            return null;
        }

        public static bool IsPositiveInfinity(string value)
        {
            if (value.Equals("Infinity", StringComparison.OrdinalIgnoreCase) || value.Equals("1.$") ||
                value.Equals("∞"))
                return true;
            Regex regex = new Regex(@"^1\.#INF");
            if (regex.IsMatch(value)) return true;
            return false;
        }

        public static bool IsNegativeInfinity(string value)
        {
            if (value.Equals("-Infinity", StringComparison.OrdinalIgnoreCase) || value.Equals("-1.$") ||
                value.Equals("-∞"))
                return true;
            Regex regex = new Regex(@"^-1\.#INF");
            if (regex.IsMatch(value)) return true;
            return false;
        }

        public static string FormatSpecial(string value)
        {
            if (value == null) return "";
            //string str = "";
            //string temp = "";
            //foreach (var c in value)
            //{
            //    temp = temp + c;
            //    var formatTemp = Format(temp);
            //    if (temp != formatTemp)
            //    {
            //        str = str + formatTemp;
            //        temp = "";
            //    }
            //}

            //if (temp.Length > 0)
            //    str = str + temp;

            return Format(value);
        }

        public static string UnFormatSpecial(string value)
        {
            string str = value;
            str = str.Replace("$", "$$");
            str = Regex.Replace(str, "\u0001", "$01");
            str = Regex.Replace(str, "\u0002", "$02");
            str = Regex.Replace(str, "\u0003", "$03");
            str = Regex.Replace(str, "\u0004", "$04");
            str = Regex.Replace(str, "\u0005", "$05");
            str = Regex.Replace(str, "\u0006", "$06");
            str = Regex.Replace(str, "\u0007", "$07");
            str = Regex.Replace(str, "\u0008", "$08");
            str = Regex.Replace(str, "\u0009", "$09");
            str = Regex.Replace(str, "\r", @"$r", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "\n", @"$l", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "\t", @"$t", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "\f", @"$p", RegexOptions.IgnoreCase);
            str = str.Replace("\0", "$00").Replace("'", "$'");
            return str;
        }

        public static string RemoveFrontZero(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            int length = 0;
            foreach (var c in s)
            {
                if (c == '0') length++;
                else break;
            }

            if (length == s.Length) length--;
            return s.Substring(length);
        }

        public static DateTime? ParseL5XControllerDate(string date)
        {
            try
            {
                var dateSplit = date.Split(' ');
                dateSplit[0] = "";

                date = string.Join(" ", dateSplit);
                return DateTime.ParseExact(date, " MMM dd hh:mm:ss yyyy", CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string ConvertToL5XControllerDate(DateTime dateTime)
        {
            return
                $"{dateTime.DayOfWeek.ToString().Substring(0, 3)} {dateTime.ToString("MMM dd hh:mm:ss yyyy", CultureInfo.InvariantCulture)}";
        }

        public static string ConvertValue(string value, DisplayStyle displayStyle, IDataType dataType)
        {
            try
            {
                bool isBin = false;
                bool isOct = false;
                bool isHex = false;
                bool isAscii = false;
                if (value.Contains("2#"))
                {
                    isBin = true;
                }
                else if (value.Contains("8#"))
                {
                    isOct = true;
                }
                else if (value.Contains("16#"))
                {
                    isHex = true;
                }
                else if (value.StartsWith("'") && value.EndsWith("'"))
                {
                    isAscii = true;
                }

                if (displayStyle == DisplayStyle.Ascii) return value;
                value = FormatOp.RemoveFormat(value, false);
                if (isBin)
                {
                    if (dataType is SINT)
                    {

                        value = Convert.ToSByte(value, 2).ToString();
                    }
                    else if (dataType is INT)
                    {

                        value = Convert.ToInt16(value, 2).ToString();
                    }
                    else if (dataType is DINT)
                    {
                        value = Convert.ToInt32(value, 2).ToString();
                    }
                    else if (dataType is LINT)
                    {
                        value = Convert.ToInt64(value, 2).ToString();
                    }
                    else
                    {
                        Debug.Assert(false);
                    }
                }
                else if (isOct)
                {
                    if (dataType is SINT)
                    {

                        value = Convert.ToSByte(value, 8).ToString();
                    }
                    else if (dataType is INT)
                    {

                        value = Convert.ToInt16(value, 8).ToString();
                    }
                    else if (dataType is DINT)
                    {
                        value = Convert.ToInt32(value, 8).ToString();
                    }
                    else if (dataType is LINT)
                    {
                        value = Convert.ToInt64(value, 8).ToString();
                    }
                    else
                    {
                        Debug.Assert(false);
                    }
                }
                else if (isHex)
                {
                    if (dataType is SINT)
                    {

                        value = Convert.ToSByte(value, 16).ToString();
                    }
                    else if (dataType is INT)
                    {

                        value = Convert.ToInt16(value, 16).ToString();
                    }
                    else if (dataType is DINT)
                    {
                        value = Convert.ToInt32(value, 16).ToString();
                    }
                    else if (dataType is LINT)
                    {
                        value = Convert.ToInt64(value, 16).ToString();
                    }
                    else
                    {
                        Debug.Assert(false);
                    }
                }
                else if (isAscii)
                {
                    var type = ValueConverter.SelectIntType(dataType);
                    value = ValueConverter.ConvertValue(value, DisplayStyle.Ascii,
                        type == Type.REAL ? DisplayStyle.Float : DisplayStyle.Decimal,
                        dataType.BitSize, type);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return value;
        }

        private static string Format(string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            str = Regex.Replace(str, @"\$r", "\r", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"\$l", "\n", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"\$t", "\t", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"\$p", "\f", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"\$00", "\0");
            str = Regex.Replace(str, @"\$\'", "'");
            str = Regex.Replace(str, @"\$01", "\u0001");
            str = Regex.Replace(str, @"\$02", "\u0002");
            str = Regex.Replace(str, @"\$03", "\u0003");
            str = Regex.Replace(str, @"\$04", "\u0004");
            str = Regex.Replace(str, @"\$05", "\u0005");
            str = Regex.Replace(str, @"\$06", "\u0006");
            str = Regex.Replace(str, @"\$07", "\u0007");
            str = Regex.Replace(str, @"\$08", "\u0008");
            str = Regex.Replace(str, @"\$09", "\u0009");
            str = Regex.Replace(str, @"\$\$", "$");
            return str;
        }
    }
}
