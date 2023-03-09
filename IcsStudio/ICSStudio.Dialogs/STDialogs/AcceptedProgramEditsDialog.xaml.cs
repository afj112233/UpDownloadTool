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
    /// AcceptedPrgramEditsDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AcceptedProgramEditsDialog 
    {
        public AcceptedProgramEditsDialog(IProgram program,bool isAccept)
        {
            InitializeComponent();
            DataContext=new AcceptedProgramEditsDialogViewModel(program,isAccept);
        }
    }
}
