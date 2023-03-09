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
using ICSStudio.Diagrams.Chart.ItemControl;

namespace ICSStudio.Diagrams.Chart
{
    /// <summary>
    /// SubroutineOrReturn.xaml 的交互逻辑
    /// </summary>
    public partial class SubroutineOrReturn : UserControl
    {
        private Window _dialog;
        public SubroutineOrReturn()
        {
            InitializeComponent();
        }

        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
        {
            //var cell = VisualHelper.FindParent<DataGridCell>((System.Windows.Controls.TextBox) sender);
            //var dataGrid = VisualHelper.FindParent<DataGrid>(cell);
            //int length = 0;
            //for (int i = 0; i < dataGrid.Items.Count; i++)
            //{
            //    var c=dataGrid.GetCell(i, 1);
            //    length=Math.Max(((Properties)c.DataContext).Value.Length,length);
            //}

            //double width = CalculateCellWidth(length);
            //for (int i = 0; i < dataGrid.Items.Count; i++)
            //{
            //    var c = dataGrid.GetCell(i, 1);
            //    c.Width = width;
            //}

            //dataGrid.Width = Math.Max(width + 70,135);
            _dialog?.Close();
        }
        
        private double CalculateCellWidth(int length)
        {
            return Math.Ceiling((double)length / 2) * 20;
        }

        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this._dialog == null||!_dialog.IsActive)
            {
                var textBox = (System.Windows.Controls.TextBox)sender;

                var p = textBox.PointToScreen(new Point());

                _dialog = new DialogInput();
                var vm = new DialogInputViewModel((Properties)textBox.DataContext);

                _dialog.DataContext = vm;
                _dialog.Top = p.Y;
                _dialog.Left = p.X;
                _dialog.Show();
            }

            e.Handled = true;
        }
    }

}
