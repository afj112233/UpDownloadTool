using System;
using System.Globalization;
using System.Linq;
using Xceed.Wpf.Toolkit;

namespace ICSStudio.Gui.Controls
{
    public class SingleUpDownExtensions : SingleUpDown
    {
        protected override string ConvertValueToText()
        {
            if (Value != null)
            {
                return ToString((float)Value, "Float");
            }

            return string.Empty;
        }

        public string ToString(float value, string format)
        {

            string result =
                value.ToString("r", CultureInfo.InvariantCulture)
                    .ToLower(CultureInfo.InvariantCulture);
            if (result.Contains('e'))
                result = value.ToString("e8", CultureInfo.InvariantCulture);

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
                        result = value.ToString("e8");
                    return result;

                default:
                    {
                        throw new ArgumentOutOfRangeException(nameof(format), format, null);
                    }
            }
        }
    }
}
