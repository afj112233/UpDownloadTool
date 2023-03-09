using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ICSStudio.Dialogs.ConfigDialogs.UI
{
    class ConvertStringToDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float) return ToString((float)value);

            return string.Empty;
        }

        public string ToString(float value)
        {

            string result =
                value.ToString("r", CultureInfo.InvariantCulture)
                    .ToLower(CultureInfo.InvariantCulture);
            if (result.Contains('e'))
                return value.ToString("e9", CultureInfo.InvariantCulture);

            // rm003T, page853
            if (float.IsNaN(value))
                return "1.#QNAN";

            if (float.IsPositiveInfinity(value))
            {
                return "1.$";
            }

            if (float.IsNegativeInfinity(value))
            {
                return "-1.$";
            }

            if (!result.Contains('.') && !result.Contains("e"))
            {
                result += ".0";
            }
            else
            {
                if (value >= 10e9)
                {
                    result = value.ToString("e9");
                }
                else if (value <= 10e-11 && value > 0)
                {
                    result = value.ToString("e9");
                }
                else
                {
                    result = value.ToString("F8").TrimEnd('.', '0');
                }
            }

            return result;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
