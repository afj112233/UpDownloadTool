using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ICSStudio.Components.Converters
{
    public class ClippedTextBoxTooltipConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2)
                return "";
            string str = values[0] as string;

            TextBox textBox = values[1] as TextBox;
            if (textBox == null)
                return "";

            // ReSharper disable once AssignNullToNotNullAttribute
            textBox.Text = str;
            if (textBox.TextWrapping == TextWrapping.NoWrap)
            {
                textBox.Measure(new Size(double.PositiveInfinity, textBox.ActualHeight));
                if (textBox.DesiredSize.Width > textBox.ActualWidth)
                    return str;
            }
            else
            {
                textBox.Measure(new Size(textBox.ActualWidth, double.PositiveInfinity));
                if (textBox.DesiredSize.Height > textBox.ActualHeight)
                    return str;
            }
            return "";
        }

        public object[] ConvertBack(
            object value,
            Type[] targetTypes,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
