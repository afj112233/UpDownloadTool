using System.Windows;
using System.Windows.Input;

namespace ICSStudio.DeviceProperties.ServoDrives.Panel
{
    /// <summary>
    /// GeneralPanel.xaml 的交互逻辑
    /// </summary>
    public partial class GeneralPanel
    {
        public GeneralPanel()
        {
            InitializeComponent();
        }

        private void GeneralPanel_OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(ValidateNameControl);
            if (ValidateNameControl.IsFocused)
            {
                ValidateNameControl.SelectAll();
            }
        }
    }
}
