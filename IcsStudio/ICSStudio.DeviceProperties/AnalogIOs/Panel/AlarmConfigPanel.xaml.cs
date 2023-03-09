using System.Windows;
using System.Windows.Input;

namespace ICSStudio.DeviceProperties.AnalogIOs.Panel
{
    /// <summary>
    /// AlarmConfigPanel.xaml 的交互逻辑
    /// </summary>
    public partial class AlarmConfigPanel
    {
        public AlarmConfigPanel()
        {
            InitializeComponent();
        }

        private void AlarmConfigPanel_OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(RadioButton);
        }
    }
}
