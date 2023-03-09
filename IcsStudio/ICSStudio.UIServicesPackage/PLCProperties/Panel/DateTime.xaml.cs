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

namespace ICSStudio.UIServicesPackage.PLCProperties.Panel
{
    /// <summary>
    /// DateTime.xaml 的交互逻辑
    /// </summary>
    public partial class DateTime : UserControl
    {
        public DateTime()
        {
            InitializeComponent();
        }

        private void UIElement_OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            checkbox.Foreground = checkbox.IsEnabled
                ? new SolidColorBrush(Colors.Black)
                : new SolidColorBrush(Colors.LightGray);
        }
    }
}
