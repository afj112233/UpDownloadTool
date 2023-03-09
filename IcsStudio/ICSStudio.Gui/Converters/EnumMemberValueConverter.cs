using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace ICSStudio.Gui.Converters
{
    public class EnumMemberValueConverter : EnumConverter
    {
        public EnumMemberValueConverter(Type type) : base(type)
        {
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return (destinationType == typeof(string)) || base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == typeof(string))
                if (value != null)
                {
                    var field = EnumType.GetField(value.ToString());
                    if (field != null)
                    {
                        var enumMemberAttribute =
                            (EnumMemberAttribute)
                            field.GetCustomAttributes(typeof(EnumMemberAttribute), false).FirstOrDefault();
                        if (enumMemberAttribute != null)
                            return enumMemberAttribute.Value;
                    }
                }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo currentCulture, object value)
        {
            var s = value as string;
            if (s != null)
                foreach (var field in EnumType.GetFields())
                {
                    var displayNameAttribute =
                        (EnumMemberAttribute)
                        field.GetCustomAttributes(typeof(EnumMemberAttribute), false).FirstOrDefault();
                    if ((displayNameAttribute != null) && Equals(s, displayNameAttribute.Value))
                        return field.GetValue(field.Name);
                }

            return base.ConvertFrom(context, currentCulture, value);
        }
    }
}
