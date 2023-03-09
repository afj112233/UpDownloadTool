using System;
using System.Windows;
using ICSStudio.MultiLanguage;

namespace ICSStudio.Dialogs.ConfigDialogs
{
    /// <summary>
    /// PIDSetupDialog.xaml 的交互逻辑
    /// </summary>
    public partial class PidSetupDialog
    {
        private PidSetUpViewModel _pidSetUpViewModel;

        public PidSetupDialog(PidSetUpViewModel viewModel)
        {
            InitializeComponent();
            _pidSetUpViewModel = viewModel;
            DataContext = _pidSetUpViewModel;
            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            if (_pidSetUpViewModel._isTuningDirty)
            {
                if (_pidSetUpViewModel.CheckSP())
                {
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                        new Action(() => SP.Focus()));
                }
                else if (_pidSetUpViewModel.CheckSO())
                {
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                        new Action(() => SO.Focus()));
                }
                else if (_pidSetUpViewModel.CheckBIAS())
                {
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                        new Action(() => BIAS.Focus()));
                }
            }
            else if (_pidSetUpViewModel._isConfigurationDirty)
            {
                if (_pidSetUpViewModel.CheckMINOMAXO())
                {
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                        new Action(() => MINO.Focus()));
                }

                else if (_pidSetUpViewModel.CheckUDP())
                {
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                        new Action(() => UPD.Focus()));
                }
            }
            else if (_pidSetUpViewModel._isAlarmsDirty)
            {
                if (_pidSetUpViewModel.CheckPVHPVL())
                {
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                        new Action(() => PVL.Focus()));
                }

                else if (_pidSetUpViewModel.CheckDVPDVN())
                {
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                        new Action(() => DVN.Focus()));
                }
            }
            else if (_pidSetUpViewModel._isScalingDirty)
            {
                if (_pidSetUpViewModel.CheckMAXIMINI())
                {
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                        new Action(() => MINI.Focus()));
                }

                else if (_pidSetUpViewModel.CheckMAXCVMINCV())
                {
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                        new Action(() => MINCV.Focus()));
                }

                else if (_pidSetUpViewModel.CheckMAXTIEMINTIE())
                {
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                        new Action(() => MINTIE.Focus()));
                }
            }
        }
    }
}
