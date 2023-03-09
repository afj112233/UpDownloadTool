using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Data;

namespace ICSStudio.Gui.Converters
{
    [ValueConversion(typeof(Enum), typeof(string))]
    public class EnumToDisplayNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            FieldInfo field = value.GetType().GetField(value.ToString());
            if (field != null)
            {
                EnumMemberAttribute enumMemberAttribute = (EnumMemberAttribute)field.GetCustomAttributes(typeof(EnumMemberAttribute), false).FirstOrDefault();
                if (enumMemberAttribute != null)
                    return enumMemberAttribute.Value;

                BrowsableAttribute browsableAttribute = (BrowsableAttribute)field.GetCustomAttributes(typeof(BrowsableAttribute), false).FirstOrDefault();
                if (browsableAttribute != null && !browsableAttribute.Browsable)
                    return string.Empty;
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
