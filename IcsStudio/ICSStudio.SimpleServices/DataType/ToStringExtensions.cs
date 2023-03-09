using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ICSStudio.Interfaces.DataType;

namespace ICSStudio.SimpleServices.DataType
{
    public static class ToStringExtensions
    {
        public static string ToString(this bool value, DisplayStyle displayStyle)
        {
            if (value)
                switch (displayStyle)
                {
                    case DisplayStyle.Binary:
                        return "2#1";

                    case DisplayStyle.Octal:
                        return "8#1";

                    case DisplayStyle.Decimal:
                        return "1";

                    case DisplayStyle.Hex:
                        return "16#1";

                    default:
                        throw new ArgumentOutOfRangeException();
                }

            switch (displayStyle)
            {
                case DisplayStyle.Binary:
                    return "2#0";

                case DisplayStyle.Octal:
                    return "8#0";

                case DisplayStyle.Decimal:
                    return "0";

                case DisplayStyle.Hex:
                    return "16#0";

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static string ToString(this byte value, DisplayStyle displayStyle)
        {
            string temp;
            const int bitSize = sizeof(byte) * 8;

            switch (displayStyle)
            {
                case DisplayStyle.Binary:
                    temp = Convert.ToString(value, 2).PadLeft(bitSize, '0');
                    temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{4})+$))", "$1_");
                    return "2#" + temp;

                case DisplayStyle.Octal:
                    temp = Convert.ToString(value, 8).PadLeft(bitSize / 3 + 1, '0');
                    temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{3})+$))", "$1_");
                    return "8#" + temp;

                case DisplayStyle.Decimal:
                    return value.ToString();

                case DisplayStyle.Hex:
                    temp = Convert.ToString(value, 16).PadLeft(bitSize / 4, '0');
                    temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{4})+$))", "$1_");
                    return "16#" + temp;

                case DisplayStyle.Ascii:
                    return $"'{ToAsciiDisplay(value)}'";

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static string ToString(this sbyte value, DisplayStyle displayStyle)
        {
            string temp;
            const int bitSize = sizeof(sbyte) * 8;

            switch (displayStyle)
            {
                case DisplayStyle.Binary:
                    temp = Convert.ToString((byte) value, 2).PadLeft(bitSize, '0');
                    temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{4})+$))", "$1_");
                    return "2#" + temp;

                case DisplayStyle.Octal:
                    temp = Convert.ToString((byte) value, 8).PadLeft(bitSize / 3 + 1, '0');
                    temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{3})+$))", "$1_");
                    return "8#" + temp;

                case DisplayStyle.Decimal:
                    return value.ToString();

                case DisplayStyle.Hex:
                    temp = Convert.ToString((byte) value, 16).PadLeft(bitSize / 4, '0');
                    temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{4})+$))", "$1_");
                    return "16#" + temp;

                case DisplayStyle.Ascii:
                    return $"'{ToAsciiDisplay((byte) value)}'";

                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static string ToString(this short value, DisplayStyle displayStyle)
        {
            string temp;
            const int bitSize = sizeof(short) * 8;

            switch (displayStyle)
            {
                case DisplayStyle.Binary:
                    temp = Convert.ToString(value, 2).PadLeft(bitSize, '0');
                    temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{4})+$))", "$1_");
                    return "2#" + temp;

                case DisplayStyle.Octal:
                    temp = Convert.ToString(value, 8).PadLeft(bitSize / 3 + 1, '0');
                    temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{3})+$))", "$1_");
                    return "8#" + temp;

                case DisplayStyle.Decimal:
                    return value.ToString();

                case DisplayStyle.Hex:
                    temp = Convert.ToString(value, 16).PadLeft(bitSize / 4, '0');
                    temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{4})+$))", "$1_");
                    return "16#" + temp;

                case DisplayStyle.Ascii:
                    var result = string.Join("",
                        BitConverter.GetBytes(value).Reverse().Select(ToAsciiDisplay));

                    return $"'{result}'";

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static string ToString(this ushort value, DisplayStyle displayStyle)
        {
            string temp;
            const int bitSize = sizeof(ushort) * 8;

            switch (displayStyle)
            {
                case DisplayStyle.Binary:
                    temp = Convert.ToString(value, 2).PadLeft(bitSize, '0');
                    temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{4})+$))", "$1_");
                    return "2#" + temp;

                case DisplayStyle.Octal:
                    temp = Convert.ToString(value, 8).PadLeft(bitSize / 3 + 1, '0');
                    temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{3})+$))", "$1_");
                    return "8#" + temp;

                case DisplayStyle.Decimal:
                    return value.ToString();

                case DisplayStyle.Hex:
                    temp = Convert.ToString(value, 16).PadLeft(bitSize / 4, '0');
                    temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{4})+$))", "$1_");
                    return "16#" + temp;

                case DisplayStyle.Ascii:
                    var result = string.Join("",
                        BitConverter.GetBytes(value).Reverse().Select(ToAsciiDisplay));

                    return $"'{result}'";

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static string ToString(this int value, DisplayStyle displayStyle)
        {
            string temp;
            const int bitSize = sizeof(int) * 8;

            switch (displayStyle)
            {
                case DisplayStyle.Binary:
                    temp = Convert.ToString(value, 2).PadLeft(bitSize, '0');
                    temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{4})+$))", "$1_");
                    return "2#" + temp;

                case DisplayStyle.Octal:
                    temp = Convert.ToString(value, 8).PadLeft(bitSize / 3 + 1, '0');
                    temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{3})+$))", "$1_");
                    return "8#" + temp;

                case DisplayStyle.Decimal:
                    return value.ToString();

                case DisplayStyle.Hex:
                    temp = Convert.ToString(value, 16).PadLeft(bitSize / 4, '0');
                    temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{4})+$))", "$1_");
                    return "16#" + temp;

                case DisplayStyle.Ascii:
                    var result = string.Join("",
                        BitConverter.GetBytes(value).Reverse().Select(ToAsciiDisplay));

                    return $"'{result}'";
                default:
                    return "error";
            }
        }

        public static string ToString(this uint value, DisplayStyle displayStyle)
        {
            string temp;
            const int bitSize = sizeof(uint) * 8;

            switch (displayStyle)
            {
                case DisplayStyle.Binary:
                    temp = Convert.ToString(value, 2).PadLeft(bitSize, '0');
                    temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{4})+$))", "$1_");
                    return "2#" + temp;

                case DisplayStyle.Octal:
                    temp = Convert.ToString(value, 8).PadLeft(bitSize / 3 + 1, '0');
                    temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{3})+$))", "$1_");
                    return "8#" + temp;

                case DisplayStyle.Decimal:
                    return value.ToString();

                case DisplayStyle.Hex:
                    temp = Convert.ToString(value, 16).PadLeft(bitSize / 4, '0');
                    temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{4})+$))", "$1_");
                    return "16#" + temp;

                case DisplayStyle.Ascii:
                    var result = string.Join("",
                        BitConverter.GetBytes(value).Reverse().Select(ToAsciiDisplay));

                    return $"'{result}'";
                default:
                    return "error";
            }
        }

        public static string ToString(this long value, DisplayStyle displayStyle)
        {
            string temp;
            const int bitSize = sizeof(long) * 8;

            long second;
            long ticks;
            DateTime utcTime;
            DateTime localTime;
            string part0;
            string part1;
            string part2;

            switch (displayStyle)
            {
                case DisplayStyle.Binary:
                    temp = Convert.ToString(value, 2).PadLeft(bitSize, '0');
                    temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{4})+$))", "$1_");
                    return "2#" + temp;

                case DisplayStyle.Octal:
                    temp = Convert.ToString(value, 8).PadLeft(bitSize / 3 + 1, '0');
                    temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{3})+$))", "$1_");
                    return "8#" + temp;

                case DisplayStyle.Decimal:
                    return value.ToString();

                case DisplayStyle.Hex:
                    temp = Convert.ToString(value, 16).PadLeft(bitSize / 4, '0');
                    temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{4})+$))", "$1_");
                    return "16#" + temp;

                case DisplayStyle.Ascii:
                    var result = string.Join("",
                        BitConverter.GetBytes(value).Reverse().Select(ToAsciiDisplay));

                    return $"'{result}'";

                case DisplayStyle.DateTime:
                    long microsecond = value % 1000000;
                    second = value / 1000000;

                    ticks = second * 10000000 + 621355968000000000;

                    utcTime = new DateTime(ticks, DateTimeKind.Utc);
                    localTime = utcTime.ToLocalTime();

                    part0 = localTime.ToString("yyyy-MM-dd-HH:mm:ss");
                    part1 = microsecond.ToString("D6");
                    part1 = part1.Insert(3, "_");
                    part2 = localTime.ToString("zzz");

                    return $"DT#{part0}.{part1}(UTC{part2})";

                case DisplayStyle.DateTimeNS:
                    long nanosecond = value % 1000000000;
                    second = value / 1000000000;

                    ticks = second * 10000000 + 621355968000000000;

                    utcTime = new DateTime(ticks, DateTimeKind.Utc);
                    localTime = utcTime.ToLocalTime();

                    part0 = localTime.ToString("yyyy-MM-dd-HH:mm:ss");
                    part1 = nanosecond.ToString("D9");
                    part1 = part1.Insert(3, "_");
                    part1 = part1.Insert(7, "_");
                    part2 = localTime.ToString("zzz");

                    return $"LDT#{part0}.{part1}(UTC{part2})";

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static string ToString(this ulong value, DisplayStyle displayStyle)
        {
            //TODO(gjc): need check later
            string temp;
            const int bitSize = sizeof(ulong) * 8;

            long second;
            long ticks;
            DateTime utcTime;
            DateTime localTime;
            string part0;
            string part1;
            string part2;

            switch (displayStyle)
            {
                case DisplayStyle.Binary:
                    temp = Convert.ToString((long) value, 2).PadLeft(bitSize, '0');
                    temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{4})+$))", "$1_");
                    return "2#" + temp;

                case DisplayStyle.Octal:
                    temp = Convert.ToString((long) value, 8).PadLeft(bitSize / 3 + 1, '0');
                    temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{3})+$))", "$1_");
                    return "8#" + temp;

                case DisplayStyle.Decimal:
                    return value.ToString();

                case DisplayStyle.Hex:
                    temp = Convert.ToString((long) value, 16).PadLeft(bitSize / 4, '0');
                    temp = Regex.Replace(temp, @"((?<=\w)(?=(\w{4})+$))", "$1_");
                    return "16#" + temp;

                case DisplayStyle.Ascii:
                    var result = string.Join("",
                        BitConverter.GetBytes(value).Reverse().Select(ToAsciiDisplay));

                    return $"'{result}'";

                case DisplayStyle.DateTime:
                    long microsecond = (long) value % 1000000;
                    second = (long) value / 1000000;

                    ticks = second * 10000000 + 621355968000000000;

                    utcTime = new DateTime(ticks, DateTimeKind.Utc);
                    localTime = utcTime.ToLocalTime();

                    part0 = localTime.ToString("yyyy-MM-dd-HH:mm:ss");
                    part1 = microsecond.ToString("D6");
                    part1 = part1.Insert(3, "_");
                    part2 = localTime.ToString("zzz");

                    return $"DT#{part0}.{part1}(UTC{part2})";

                case DisplayStyle.DateTimeNS:
                    long nanosecond = (long) value % 1000000000;
                    second = (long) value / 1000000000;

                    ticks = second * 10000000 + 621355968000000000;

                    utcTime = new DateTime(ticks, DateTimeKind.Utc);
                    localTime = utcTime.ToLocalTime();

                    part0 = localTime.ToString("yyyy-MM-dd-HH:mm:ss");
                    part1 = nanosecond.ToString("D9");
                    part1 = part1.Insert(3, "_");
                    part1 = part1.Insert(7, "_");
                    part2 = localTime.ToString("zzz");

                    return $"LDT#{part0}.{part1}(UTC{part2})";

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static string ToString(this float value, DisplayStyle displayStyle)
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
                switch (displayStyle)
                {
                    case DisplayStyle.Exponential:
                        return "1.#INF0000e+000";
                    case DisplayStyle.Float:
                        return "1.$";
                }
            }

            if (float.IsNegativeInfinity(value))
            {
                switch (displayStyle)
                {
                    case DisplayStyle.Exponential:
                        return "-1.#INF0000e+000";
                    case DisplayStyle.Float:
                        return "-1.$";
                }
            }


            switch (displayStyle)
            {
                case DisplayStyle.Exponential:
                    if (result.Contains('e'))
                        return result;

                    return value.ToString("e8", CultureInfo.InvariantCulture);

                case DisplayStyle.Float:
                    if (!result.Contains('.') && !result.Contains("e"))
                        result += ".0";
                    if (value >= 10e9)
                        result = value.ToString("e9");
                    return result;

                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(displayStyle), displayStyle, null);
                }
            }
        }

        public static string ToAsciiDisplay(this byte value)
        {
            switch (value)
            {
                case 9:
                    return "$t";
                case 10:
                    return "$l";
                case 12:
                    return "$p";
                case 13:
                    return "$r";
                case 36:
                    return "$$";
                case 39:
                    return "$'";
            }

            if (value >= 32 && value <= 126)
                return $"{(char) value}";

            return $"${value:X2}";
        }
    }
}
