using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using OxyPlot;

namespace ICSStudio.Dialogs.ConfigDialogs
{
    public class ColorConverter : IValueConverter
    {
        //当值从绑定源传播给绑定目标时调用
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var oxyColor = (OxyColor) value;
            Color color = Color.FromArgb(oxyColor.A, oxyColor.R, oxyColor.G, oxyColor.B);
            return color;
        }

        //当值从绑定目标传播给绑定源时调用
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color) value;
            OxyColor oxyColor = OxyColor.FromArgb(color.A, color.R, color.G, color.B);
            return oxyColor;
        }
    }
}
