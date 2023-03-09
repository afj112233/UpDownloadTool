using System.Windows;
using System.Windows.Input;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.ViewModel;

namespace ICSStudio.UIServicesPackage.View
{
    /// <summary>
    /// DownloadingDialog.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadingDialog
    {
        private readonly DownloadingViewModel _viewModel;

        private readonly string _preserveFilePath;

        private readonly string _restoreFilePath;

        public DownloadingDialog(
            Controller controller,
            string preserveFilePath, string restoreFilePath)
        {
            InitializeComponent();
            KeyDown += DownloadingDialog_KeyDown;

            _preserveFilePath = preserveFilePath;
            _restoreFilePath = restoreFilePath;

            _viewModel = new DownloadingViewModel(controller);
            DataContext = _viewModel;
        }

        public DownloadingViewModel ViewModel => _viewModel;

        private void DownloadingDialog_KeyDown(object sender,KeyEventArgs e)
        {
            if(e.Key == Key.System && e.SystemKey == Key.F4)
                e.Handled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.Download(_preserveFilePath, _restoreFilePath);
        }
    }
}
