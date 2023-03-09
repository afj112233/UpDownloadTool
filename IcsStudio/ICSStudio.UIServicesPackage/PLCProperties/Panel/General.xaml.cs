using System.Windows;
using System.Windows.Input;

namespace ICSStudio.UIServicesPackage.PLCProperties.Panel
{
    /// <summary>
    /// General.xaml 的交互逻辑
    /// </summary>
    public partial class General
    {
        public General()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(NameBox);
            if (NameBox.IsFocused)
            {
                NameBox.SelectAll();
            }
        }
    }
}
