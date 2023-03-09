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

namespace ICSStudio.Diagrams.Chart
{
    /// <summary>
    /// TextBox.xaml 的交互逻辑
    /// </summary>
    public partial class TextBox : UserControl
    {
        public TextBox()
        {
            InitializeComponent();
        }

        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBox1.Visibility = Visibility.Collapsed;
            TextBox2.Visibility = Visibility.Visible;
        }

        private void TextBox2_OnLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox2.Visibility = Visibility.Collapsed;
            TextBox1.Visibility = Visibility.Visible;
        }
    }
}
