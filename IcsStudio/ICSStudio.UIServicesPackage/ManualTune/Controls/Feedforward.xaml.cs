using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace ICSStudio.UIServicesPackage.ManualTune.Controls
{
    /// <summary>
    /// Feedforward.xaml 的交互逻辑
    /// </summary>
    public partial class Feedforward
    {
        public Feedforward()
        {
            InitializeComponent();
        }

        private void SingleUpDown_KeyUp(object sender, KeyEventArgs e)
        {
            SingleUpDown singleUpDown = sender as SingleUpDown;
            if (e.Key == Key.Enter)
            {
                singleUpDown?.GetBindingExpression(SingleUpDown.ValueProperty)?.UpdateSource();
            }
        }

        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
        {
            SingleUpDown singleUpDown = sender as SingleUpDown;
            if (!singleUpDown.Value.HasValue)
                singleUpDown?.GetBindingExpression(SingleUpDown.ValueProperty)?.UpdateTarget();
        }
    }
}
