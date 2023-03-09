using System.Windows;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.ViewModel;

namespace ICSStudio.UIServicesPackage.View
{
    /// <summary>
    /// UploadAndCompareDialog.xaml 的交互逻辑
    /// </summary>
    public partial class UploadAndCompareDialog
    {
        public UploadAndCompareDialog(Controller controller)
        {
            InitializeComponent();

            ViewModel = new UploadAndCompareViewModel(controller);

            DataContext = ViewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.UploadAndCompare();
        }

        public UploadAndCompareViewModel ViewModel { get; }
    }
}
