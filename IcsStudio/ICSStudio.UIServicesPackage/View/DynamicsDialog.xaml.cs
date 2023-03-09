using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ICSStudio.MultiLanguage;
using ICSStudio.UIServicesPackage.ViewModel;

namespace ICSStudio.UIServicesPackage.View
{
    /// <summary>
    ///     DynamicsDialog.xaml 的交互逻辑
    /// </summary>
    public partial class DynamicsDialog : Window
    {
        public DynamicsDialog()
        {
            InitializeComponent();

            LanguageManager.GetInstance().SetLanguage(this);
            if (!"English".Equals(LanguageInfo.CurrentLanguage)) Width = 500;
        }

        private void DynamicsDialog_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var textBox = Keyboard.FocusedElement as TextBox;
            if (textBox != null)
            {
                var tRequest = new TraversalRequest(FocusNavigationDirection.Next);
                textBox.MoveFocus(tRequest);
            }
        }
    }

    public class CalculatePatternToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (DynamicsViewModel.Pattern)value == (DynamicsViewModel.Pattern)int.Parse(System.Convert.ToString(parameter));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(bool)value) return null;
            return (DynamicsViewModel.Pattern)int.Parse(System.Convert.ToString(parameter));
        }
    }
}