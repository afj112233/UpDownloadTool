using System.Windows;
using System.Windows.Input;

namespace ICSStudio.Dialogs.WaitDialog
{
    /// <summary>
    /// FindingDialog.xaml 的交互逻辑
    /// </summary>
    public partial class FindingDialog : Window
    {
        public FindingDialog()
        {
            InitializeComponent();
            KeyDown += FindingDialog_KeyDown;
            MouseLeftButtonDown += delegate{DragMove();} ;
        }

        private void FindingDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4)
                e.Handled = true;
        }
    }
}
