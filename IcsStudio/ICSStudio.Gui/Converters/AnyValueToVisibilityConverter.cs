using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ICSStudio.Gui.Converters
{
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class AnyValueToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                bool flag = System.Convert.ToBoolean(value, CultureInfo.CurrentCulture);

                string convertParameter = parameter as string;

                if (convertParameter != null)
                {
                    convertParameter = convertParameter.ToLower(CultureInfo.CurrentCulture);

                    //
                    if (convertParameter.Contains("falsetovisible"))
                    {
                        if (!flag)
                            return Visibility.Visible;

                        return Visibility.Collapsed;
                    }
                }

                //
                if (flag)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                return Visibility.Collapsed;
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
