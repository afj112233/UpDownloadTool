using System.Windows;
using System.Windows.Input;

namespace ICSStudio.UIServicesPackage.AxisVirtualProperties.Panel
{
    /// <summary>
    /// MotionPlanner.xaml 的交互逻辑
    /// </summary>
    public partial class MotionPlanner
    {
        public MotionPlanner()
        {
            InitializeComponent();
        }

        private void MotionPlanner_OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(IntegerUpDown);
        }
    }
}
