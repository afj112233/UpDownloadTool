using System;
using System.Windows.Input;
using ICSStudio.UIServicesPackage.ViewModel;

namespace ICSStudio.UIServicesPackage.View
{
    /// <summary>
    /// VerifyDialog.xaml 的交互逻辑
    /// </summary>
    public partial class VerifyDialog
    {
        private readonly VerifyViewModel _viewModel;

        public VerifyDialog()
        {
            InitializeComponent();

            KeyDown += VerifyDialog_KeyDown;

            _viewModel = new VerifyViewModel();
            DataContext = _viewModel;
        }

        private void VerifyDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4)
                e.Handled = true;
        }

        private void VerifyDialog_OnContentRendered(object sender, EventArgs e)
        {
            _viewModel.Verify();
        }
    }
}
