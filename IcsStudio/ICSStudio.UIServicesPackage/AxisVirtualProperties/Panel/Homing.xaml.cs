using System.Windows;
using System.Windows.Input;

namespace ICSStudio.UIServicesPackage.AxisVirtualProperties.Panel
{
    /// <summary>
    /// Homing.xaml 的交互逻辑
    /// </summary>
    public partial class Homing
    {
        public Homing()
        {
            InitializeComponent();
        }

        private void Homing_OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(SingleUpDown);
        }
    }
}
