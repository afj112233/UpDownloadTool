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
using System.Windows.Shapes;

namespace ICSStudio.Diagrams.Chart.ItemControl
{
    /// <summary>
    /// DialogInput.xaml 的交互逻辑
    /// </summary>
    public partial class DialogInput : Window
    {
        public DialogInput()
        {
            InitializeComponent();
        }

        private void DialogInput_OnDeactivated(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Close();
            }
        }
    }
}
