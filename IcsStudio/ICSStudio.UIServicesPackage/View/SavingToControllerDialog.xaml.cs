using System.Windows;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.ViewModel;

namespace ICSStudio.UIServicesPackage.View
{
    /// <summary>
    /// SavingToControllerDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SavingToControllerDialog
    {
        private readonly SavingToControllerViewModel _viewModel;

        public SavingToControllerDialog(Controller controller)
        {
            InitializeComponent();

            _viewModel = new SavingToControllerViewModel(controller);

            DataContext = _viewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveToController();
        }
    }
}
