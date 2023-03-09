using System.Windows;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.ViewModel;

namespace ICSStudio.UIServicesPackage.View
{
    /// <summary>
    /// ComparingDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ComparingDialog
    {
        private readonly ComparingViewModel _viewModel;

        public ComparingDialog(Controller controller)
        {
            InitializeComponent();

            _viewModel = new ComparingViewModel(controller);

            DataContext = _viewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.Compare();
        }

        public ComparingViewModel ViewModel => _viewModel;
    }
}
