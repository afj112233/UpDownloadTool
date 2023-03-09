namespace ICSStudio.DeviceProperties.Dialogs
{
    /// <summary>
    /// ResettingDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ResettingDialog
    {
        public ResettingDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ResettingViewModel viewModel = DataContext as ResettingViewModel;
            viewModel?.Reset();
        }
    }
}
