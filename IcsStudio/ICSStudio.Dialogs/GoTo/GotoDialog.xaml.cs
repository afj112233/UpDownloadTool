using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ICSStudio.MultiLanguage;

namespace ICSStudio.Dialogs.GoTo
{
    /// <summary>
    /// GotoDialog.xaml 的交互逻辑
    /// </summary>
    public partial class GotoDialog 
    {
        public GotoDialog()
        {
            InitializeComponent();

            LanguageManager.GetInstance().SetLanguage(this);
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            var listBox = (ListBox) sender;
            listBox.ScrollIntoView(listBox.SelectedItem);
            LineNumberTextBox.CaretIndex = LineNumberTextBox.Text.Length;
            LineNumberTextBox.Focus();
        }
    }

    public class LanguageConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return LanguageManager.GetInstance().ConvertSpecifier(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
