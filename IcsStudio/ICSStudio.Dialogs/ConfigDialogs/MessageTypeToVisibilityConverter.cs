using System;
using System.Globalization;
using System.Windows;
using ICSStudio.Interfaces.DataType;
using IValueConverter = System.Windows.Data.IValueConverter;

namespace ICSStudio.Dialogs.ConfigDialogs
{
    public class MessageTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            MessageTypeEnum messageType = (MessageTypeEnum) value;
            MessageTypeEnum parameterType = (MessageTypeEnum) parameter;

            if (messageType == parameterType)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
