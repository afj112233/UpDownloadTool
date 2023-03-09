using ICSStudio.QuickWatchPackage.View.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ICSStudio.QuickWatchPackage.View.UI
{
    [ValueConversion(typeof(TagItem), typeof(Visibility))]
    public class ConfigureVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TagItem tagItem = value as TagItem;

            if (tagItem?.Tag != null)
            {
                var dataTypeInfo = tagItem.Tag.DataTypeInfo;
                var typeName = dataTypeInfo.DataType.Name;

                if (typeName.Equals("CAM", StringComparison.OrdinalIgnoreCase)
                    || typeName.Equals("CAM_PROFILE", StringComparison.OrdinalIgnoreCase))
                {
                    if (dataTypeInfo.Dim1 > 0)
                        return Visibility.Visible;
                }

                if (typeName.Equals("PID", StringComparison.OrdinalIgnoreCase)
                    || typeName.Equals("MESSAGE", StringComparison.OrdinalIgnoreCase)
                    || typeName.Equals("AXIS_CONSUMED", StringComparison.OrdinalIgnoreCase)
                    || typeName.Equals("AXIS_GENERIC", StringComparison.OrdinalIgnoreCase)
                    || typeName.Equals("AXIS_GENERIC_DRIVE", StringComparison.OrdinalIgnoreCase)
                    || typeName.Equals("AXIS_CIP_DRIVE", StringComparison.OrdinalIgnoreCase)
                    || typeName.Equals("AXIS_VIRTUAL", StringComparison.OrdinalIgnoreCase)
                    || typeName.Equals("AXIS_SERVO", StringComparison.OrdinalIgnoreCase)
                    || typeName.Equals("AXIS_SERVO_DRIVE", StringComparison.OrdinalIgnoreCase)
                    || typeName.Equals("Motion_Group", StringComparison.OrdinalIgnoreCase))
                {
                    return Visibility.Visible;
                }
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
