using GalaSoft.MvvmLight;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.UIServicesPackage.ViewModel;

namespace ICSStudio.UIServicesPackage.View
{
    /// <summary>
    /// AutotuneProgressDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AutotuneProgressDialog
    {
        private readonly AutotuneProgressViewModel _autotuneProgressViewModel;

        public AutotuneProgressDialog(ICipMessager messager, IMessageRouterRequest request)
        {
            InitializeComponent();

            _autotuneProgressViewModel = new AutotuneProgressViewModel(messager, request);
            DataContext = _autotuneProgressViewModel;
        }

        public string TestState => _autotuneProgressViewModel.TestState;

        private void Window_Closed(object sender, System.EventArgs e)
        {
            ((ICleanup) _autotuneProgressViewModel)?.Cleanup();
        }
    }
}
