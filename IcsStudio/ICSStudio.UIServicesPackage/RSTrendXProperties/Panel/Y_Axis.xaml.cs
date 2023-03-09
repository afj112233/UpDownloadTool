using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ICSStudio.UIServicesPackage.RSTrendXProperties.Panel
{
    /// <summary>
    /// Y_Axis.xaml 的交互逻辑
    /// </summary>
    public partial class Y_Axis : UserControl
    {
        public Y_Axis()
        {
            InitializeComponent();
        }

        private void Y_Axis_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
            {
#pragma warning disable VSTHRD001 // 避免旧线程切换 API
                Dispatcher.BeginInvoke(DispatcherPriority.Render, (Action)(() => ((Y_AxisViewModel)DataContext).Save()));
#pragma warning restore VSTHRD001 // 避免旧线程切换 API
            }
        }
    }
}
