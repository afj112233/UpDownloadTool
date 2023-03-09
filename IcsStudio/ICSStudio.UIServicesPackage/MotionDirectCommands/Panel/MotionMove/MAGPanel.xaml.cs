using System.Windows.Controls;
using System.Windows.Input;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.Panel.MotionMove
{
    /// <summary>
    /// MAGPanel.xaml 的交互逻辑
    /// </summary>
    public partial class MAGPanel : UserControl
    {
        public MAGPanel()
        {
            InitializeComponent();
        }

        private void PropertyGrid_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                PropertyGrid.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                e.Handled = true;
            }
        }
    }
}
