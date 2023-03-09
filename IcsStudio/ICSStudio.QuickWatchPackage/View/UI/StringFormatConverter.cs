using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;

namespace ICSStudio.QuickWatchPackage.View.UI
{
    public class StringFormatConverter : IMultiValueConverter
    {
        public object Convert(
            object[] values, System.Type targetType,
            object parameter, CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue ||
                values[1] == DependencyProperty.UnsetValue)
                return string.Empty;

            if (values[0] == null)
                return string.Empty;

            if (values[0] is StructOrArrayValue)
                return values[0].ToString();

            var displayStyle = (DisplayStyle) values[1];

            if (values[0] is bool) return ((bool) values[0]).ToString(displayStyle);

            if (values[0] is sbyte) return ((sbyte) values[0]).ToString(displayStyle);

            if (values[0] is short) return ((short) values[0]).ToString(displayStyle);

            if (values[0] is int) return ((int) values[0]).ToString(displayStyle);

            if (values[0] is long) return ((long) values[0]).ToString(displayStyle);

            if (values[0] is float) return ((float) values[0]).ToString(displayStyle);


            if (values[0] is string)
                return values[0];

            //TODO(gjc):add code here
            return string.Empty;
        }

        public object[] ConvertBack(
            object value, System.Type[] targetTypes,
            object parameter, CultureInfo culture)
        {
            //DependencyProperty.UnsetValue, Binding.DoNothing
            return new[] {value, Binding.DoNothing};
        }
    }
}