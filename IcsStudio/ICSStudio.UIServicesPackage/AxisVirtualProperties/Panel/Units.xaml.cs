using System.Windows;
using System.Windows.Input;

namespace ICSStudio.UIServicesPackage.AxisVirtualProperties.Panel
{
    /// <summary>
    /// Units.xaml 的交互逻辑
    /// </summary>
    public partial class Units
    {
        public Units()
        {
            InitializeComponent();
        }

        private void Units_OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(NameBox);
            if (NameBox.IsFocused)
            {
                NameBox.SelectAll();
            }
        }
    }
}
