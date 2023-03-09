using System.Windows;
using System.Windows.Input;

namespace ICSStudio.UIServicesPackage.AxisVirtualProperties.Panel
{
    /// <summary>
    /// Dynamics.xaml 的交互逻辑
    /// </summary>
    public partial class Dynamics
    {
        public Dynamics()
        {
            InitializeComponent();
        }

        private void Dynamics_OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(SingleUpDown);
        }
    }
}
