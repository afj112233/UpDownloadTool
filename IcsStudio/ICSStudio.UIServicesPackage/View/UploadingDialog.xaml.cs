using System.Windows;
using System.Windows.Input;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.ViewModel;

namespace ICSStudio.UIServicesPackage.View
{
    /// <summary>
    /// UploadingDialog.xaml 的交互逻辑
    /// </summary>
    public partial class UploadingDialog
    {
        private readonly UploadingViewModel _viewModel;

        public UploadingDialog(Controller controller, string fileName)
        {
            InitializeComponent();

            KeyDown += UploadingDialog_KeyDown;

            _viewModel = new UploadingViewModel(controller, fileName);
            DataContext = _viewModel;
        }

        private void UploadingDialog_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4)
                e.Handled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.Upload();
        }
    }
}
