using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.Other;
using ICSStudio.DeviceProfiles.Generic;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.DeviceProperties.Dialogs;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DeviceModule;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.DeviceProperties.Generic.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class ModuleInfoViewModel : DeviceOptionPanel
    {
        private IdentityDescriptor _descriptor;

        public ModuleInfoViewModel(UserControl control, ModifiedGeneralEthernet modifiedModule) : base(control)
        {
            ModifiedModule = modifiedModule;

            RefreshCommand =
                new RelayCommand(ExecuteRefreshCommand, CanExecuteRefreshCommand);
            ResetCommand =
                new RelayCommand(ExecuteResetCommand, CanExecuteResetCommand);
        }

        public ModifiedGeneralEthernet ModifiedModule { get; }
        public GeneralEthernet OriginalModule => ModifiedModule?.OriginalModule;
        public GenericEnetModuleProfiles Profiles => OriginalModule?.Profiles;

        public IController Controller => ModifiedModule.Controller;

        public Visibility ModuleInfoVisibility
            => Controller.IsOnline ? Visibility.Visible : Visibility.Hidden;

        public string Vendor => _descriptor != null ? _descriptor.Vendor : string.Empty;
        public string ProductType => _descriptor != null ? _descriptor.ProductType : string.Empty;
        public string ProductCode => _descriptor != null ? _descriptor.ProductCode : string.Empty;

        public string Revision => _descriptor != null ? _descriptor.Revision : string.Empty;

        public string SerialNumber => _descriptor != null ? _descriptor.SerialNumber : string.Empty;
        public string ProductName => _descriptor != null ? _descriptor.ProductName : string.Empty;

        public string MajorFault => _descriptor != null ? _descriptor.MajorFault : string.Empty;
        public string MinorFault => _descriptor != null ? _descriptor.MinorFault : string.Empty;
        public string InternalState => _descriptor != null ? _descriptor.InternalState : string.Empty;

        public string Configured => _descriptor != null ? _descriptor.Configured : string.Empty;
        public string Owned => _descriptor != null ? _descriptor.Owned : string.Empty;

        public string ModuleIdentity
        {
            get
            {
                if (_descriptor != null)
                {
                    int vendor = OriginalModule.Vendor;
                    int productCode = OriginalModule.ProductCode;
                    int productType = OriginalModule.ProductType;
                    int majorRevision = OriginalModule.Major;

                    return _descriptor.GetModuleIdentity(vendor, productCode, productType, majorRevision);
                }

                return string.Empty;
            }
        }

        public RelayCommand RefreshCommand { get; }
        public RelayCommand ResetCommand { get; }

        #region Command

        private bool CanExecuteResetCommand()
        {
            if (!Controller.IsOnline)
                return false;

            return true;
        }

        private void ExecuteResetCommand()
        {
            //string message = "DANGER. Connection Interruption.\n";
            //message += "Reset should not be performed on a module currently being used for control.";
            //message += "The connection to the module will be broken, and control may be interrupted.\n";
            //message += "Continue with Reset?";

            var result = MessageBox.Show(LanguageManager.GetInstance().ConvertSpecifier("Adapter.ModuleInfo"), "ICS Studio",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    ResettingDialog dialog = new ResettingDialog
                    {
                        Owner = Application.Current.MainWindow,
                        DataContext = new DeviceModuleResetting(OriginalModule)
                    };

                    var dialogResult = dialog.ShowDialog();
                    if (dialogResult.HasValue && dialogResult.Value)
                    {
                        // refresh
                    }
                    else
                    {
                        //TODO(gjc): add code here
                    }

                });

            }
        }

        private bool CanExecuteRefreshCommand()
        {
            if (!Controller.IsOnline)
                return false;

            return true;
        }

        private void ExecuteRefreshCommand()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                RefreshingDialog dialog = new RefreshingDialog
                {
                    Owner = Application.Current.MainWindow
                };

                var viewModel = new DeviceModuleRefreshing(OriginalModule);

                dialog.DataContext = viewModel;

                dialog.ShowDialog();

                _descriptor = viewModel.Descriptor;

                RaisePropertyChanged("ModuleInfoVisibility");

                RaisePropertyChanged("Vendor");
                RaisePropertyChanged("ProductType");
                RaisePropertyChanged("ProductCode");
                RaisePropertyChanged("Revision");
                RaisePropertyChanged("SerialNumber");
                RaisePropertyChanged("ProductName");

                RaisePropertyChanged("MajorFault");
                RaisePropertyChanged("MinorFault");
                RaisePropertyChanged("InternalState");

                RaisePropertyChanged("Configured");
                RaisePropertyChanged("Owned");
                RaisePropertyChanged("ModuleIdentity");

            });
        }

        #endregion
    }
}
