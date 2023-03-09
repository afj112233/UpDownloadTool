namespace ICSStudio.DeviceProperties.Dialogs
{
    /// <summary>
    /// RefreshingDialog.xaml 的交互逻辑
    /// </summary>
    public partial class RefreshingDialog
    {
        public RefreshingDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            RefreshingViewModel viewModel = DataContext as RefreshingViewModel;
            viewModel?.Refresh();
        }
    }
}
