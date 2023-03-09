using System.Windows;
using System.Windows.Input;

namespace ICSStudio.DeviceProperties.DiscreteIOs.Panel
{
    /// <summary>
    /// ConnectionPanel.xaml 的交互逻辑
    /// </summary>
    public partial class ConnectionPanel
    {
        public ConnectionPanel()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(SingleUpDown);
        }
    }
}
