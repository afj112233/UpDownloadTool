using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.ViewModel;

namespace ICSStudio.UIServicesPackage.View
{
    /// <summary>
    /// ConnectingDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ConnectingDialog
    {
        private readonly ConnectingViewModel _viewModel;

        public ConnectingDialog(Controller controller, string commPath)
        {
            InitializeComponent();

            _viewModel = new ConnectingViewModel(controller, commPath);

            DataContext = _viewModel;
        }

        public ConnectingViewModel ViewModel => _viewModel;

        private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _viewModel.Connect();
        }
    }
}
