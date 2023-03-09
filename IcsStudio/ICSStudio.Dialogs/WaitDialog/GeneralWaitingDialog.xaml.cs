using System.Windows;
using System.Windows.Input;

namespace ICSStudio.Dialogs.WaitDialog
{
    /// <summary>
    /// WaitingDialog.xaml 的交互逻辑
    /// </summary>
    public partial class GeneralWaitingDialog : Window
    {
        public GeneralWaitingDialog(GeneralWaitingViewModel vm)
        {
            InitializeComponent();
            
            DataContext = vm;

            Owner = Application.Current.MainWindow;

            Loaded += GeneralWaitingDialog_Loaded;
            KeyDown += GeneralWaitingDialog_KeyDown;
            MouseLeftButtonDown += GeneralWaitingDialog_MouseLeftButtonDown;
        }

        private void GeneralWaitingDialog_Loaded(object sender, RoutedEventArgs e)
        {
            (DataContext as GeneralWaitingViewModel)?.Start();
        }

        private void GeneralWaitingDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.System && e.Key == Key.F4)
                e.Handled = true;
        }

        private void GeneralWaitingDialog_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
