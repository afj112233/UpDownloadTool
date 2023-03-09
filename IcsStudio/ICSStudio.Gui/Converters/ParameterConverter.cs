using System;
using System.Globalization;
using System.Windows.Data;
using ICSStudio.Gui.Controls;

namespace ICSStudio.Gui.Converters
{
    public class ParameterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            UpdateCommandParameterInfo commandParameterInfo = new UpdateCommandParameterInfo
            {
                Max = (double) values[0],
                Min = (double) values[1],
                Step = (double) values[2],
                Strategy = (SpinStrategy) values[3]
            };

            bool direction;
            bool.TryParse(values[4].ToString(), out direction);
            commandParameterInfo.Direction = direction;

            commandParameterInfo.CurrVal = (string) values[5];
            commandParameterInfo.DefaultVal = (double) values[6];

            return commandParameterInfo;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
