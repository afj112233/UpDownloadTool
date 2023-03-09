using System.Windows;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.ViewModel;

namespace ICSStudio.UIServicesPackage.View
{
    /// <summary>
    /// OnliningDialog.xaml 的交互逻辑
    /// </summary>
    public partial class OnliningDialog
    {
        private readonly OnliningViewModel _viewModel;

        public OnliningDialog(Controller controller)
        {
            InitializeComponent();

            _viewModel = new OnliningViewModel(controller);

            DataContext = _viewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.GoOnline();
        }
    }
}
