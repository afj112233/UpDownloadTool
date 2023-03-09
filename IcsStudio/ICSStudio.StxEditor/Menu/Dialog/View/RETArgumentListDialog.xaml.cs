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
using ICSStudio.Gui.View;

namespace ICSStudio.StxEditor.Menu.Dialog.View
{
    /// <summary>
    /// RETArgumentListDialog.xaml 的交互逻辑
    /// </summary>
    public partial class RETArgumentListDialog : Window
    {
        //Ret\sbr
        public RETArgumentListDialog()
        {
            InitializeComponent();
        }
        
        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var cell=VisualTreeHelpers.FindVisualParentOfType<DataGridCell>((AutoCompleteBox) sender);
            cell.IsEditing = false;
        }
    }
}
