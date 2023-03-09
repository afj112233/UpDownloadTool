using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using ICSStudio.Components.Model;
using ICSStudio.Components.View;

namespace ICSStudio.Components.Converters
{
    public class ErrorLevelToImageSourceConverter : IValueConverter
    {
        private static readonly DrawingImage EmptyImage = new DrawingImage();
        private readonly SystemIconToImageSourceConverter _systemIconConverter 
            = new SystemIconToImageSourceConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || targetType == null || !targetType.IsAssignableFrom(typeof(ImageSource)))
                return DependencyProperty.UnsetValue;

            ErrorLevel errorLevel;
            try
            {
                errorLevel = (ErrorLevel) Enum.Parse(typeof(ErrorLevel), value.ToString(), true);
            }
            catch (ArgumentException)
            {
                return DependencyProperty.UnsetValue;
            }
            catch (OverflowException)
            {
                return DependencyProperty.UnsetValue;
            }

            switch (errorLevel)
            {
                case ErrorLevel.NoError:
                    return EmptyImage;
                case ErrorLevel.Warning:
                    return _systemIconConverter.Convert(SystemIconType.Warning, targetType, parameter,
                        culture);
                case ErrorLevel.Error:
                    return _systemIconConverter.Convert(SystemIconType.Error, targetType, parameter, culture);
                default:
                    return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Converting from an image source is not supported.");
        }
    }
}
