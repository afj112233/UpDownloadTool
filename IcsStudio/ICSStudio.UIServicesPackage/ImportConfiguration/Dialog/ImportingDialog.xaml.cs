using System.Windows;
using System.Windows.Input;

namespace ICSStudio.UIServicesPackage.ImportConfiguration.Dialog
{
    /// <summary>
    /// ImportingDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ImportingDialog : Window
    {
        public ImportingDialog()
        {
            InitializeComponent();
            KeyDown += ImportingDialog_KeyDown; 
            Loaded += ImportingDialog_Loaded;
        }

        private void ImportingDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4)
                e.Handled = true;
        }

        private void ImportingDialog_Loaded(object sender, RoutedEventArgs e)
        {
            ((ImportingViewModel)DataContext).Start();
        }

        private void ImportingDialog_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    } 
}
