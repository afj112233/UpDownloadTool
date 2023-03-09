using System.Windows;
using ICSStudio.DownloadChange;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.ViewModel;

namespace ICSStudio.UIServicesPackage.View
{
    /// <summary>
    /// DownloadChangeDialog.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadChangeDialog
    {
        private readonly DownloadChangeViewModel _viewModel;

        public DownloadChangeDialog(Controller controller, 
            ProjectDiffModel diffModel, bool isDownloadAxisParameters)
        {
            InitializeComponent();

            _viewModel = new DownloadChangeViewModel(controller, diffModel, isDownloadAxisParameters);

            DataContext = _viewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.DownloadChange();
        }
    }
}
