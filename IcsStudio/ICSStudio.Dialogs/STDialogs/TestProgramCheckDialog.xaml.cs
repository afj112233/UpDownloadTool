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
using ICSStudio.Interfaces.Common;

namespace ICSStudio.Dialogs.STDialogs
{
    /// <summary>
    /// TestProgramCheckDialog.xaml 的交互逻辑
    /// </summary>
    public partial class TestProgramCheckDialog 
    {
        public TestProgramCheckDialog(IProgram program,bool isTest)
        {
            InitializeComponent();
            DataContext=new TestProgramCheckDialogViewModel(program,isTest);
        }

        private void Selector_OnSelected(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
