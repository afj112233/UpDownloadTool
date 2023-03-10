using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.Other;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.DeviceProperties.Dialogs;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.DeviceProperties.AnalogIOs.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class ModuleInfoViewModel : DeviceOptionPanel
    {
        private IdentityDescriptor _descriptor;

        public ModuleInfoViewModel(UserControl panel, ModifiedAnalogIO modifiedAnalogIO) : base(panel)
        {
            ModifiedAnalogIO = modifiedAnalogIO;

            RefreshCommand = new RelayCommand(ExecuteRefreshCommand, CanExecuteRefreshCommand);
            ResetCommand = new RelayCommand(ExecuteResetCommand, CanExecuteResetCommand);
        }

        public ModifiedAnalogIO ModifiedAnalogIO { get; }

        public IController Controller => ModifiedAnalogIO.Controller;

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

        public string Configured => _descriptor != null ? _descriptor.Configured : string.Empty;
        public string Owned => _descriptor != null ? _descriptor.Owned : string.Empty;

        public string ProtectionMode => _descriptor != null ? _descriptor.ProtectionMode : string.Empty;

        public string ModuleIdentity
        {
            get
            {
                if (_descriptor != null)
                {
                    int vendor = ModifiedAnalogIO.OriginalAnalogIO.Vendor;
                    int productCode = ModifiedAnalogIO.OriginalAnalogIO.ProductCode;
                    int productType = ModifiedAnalogIO.OriginalAnalogIO.ProductType;
                    int majorRevision = ModifiedAnalogIO.OriginalAnalogIO.Major;

                    return _descriptor.GetModuleIdentity(vendor, productCode, productType, majorRevision);
                }

                return string.Empty;
            }
        }

        public RelayCommand RefreshCommand { get; }
        public RelayCommand ResetCommand { get; }

        public override void Show()
        {
            RefreshCommand.RaiseCanExecuteChanged();
            ResetCommand.RaiseCanExecuteChanged();

            if (!Controller.IsOnline)
            {
                _descriptor = null;
                RaisePropertyChanged("ModuleInfoVisibility");

                RaisePropertyChanged("Vendor");
                RaisePropertyChanged("ProductType");
                RaisePropertyChanged("ProductCode");
                RaisePropertyChanged("Revision");
                RaisePropertyChanged("SerialNumber");
                RaisePropertyChanged("ProductName");

                RaisePropertyChanged("MajorFault");
                RaisePropertyChanged("MinorFault");


                RaisePropertyChanged("Configured");
                RaisePropertyChanged("Owned");
                RaisePropertyChanged("ModuleIdentity");

                RaisePropertyChanged("ProtectionMode");
            }
            else
            {
                ExecuteRefreshCommand();
            }
        }

        #region Command

        private bool CanExecuteResetCommand()
        {
            if (!Controller.IsOnline)
                return false;

            return true;
        }

        private bool CanExecuteRefreshCommand()
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
                        DataContext = new IOModuleResetting(ModifiedAnalogIO.OriginalAnalogIO)
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



        private void ExecuteRefreshCommand()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                RefreshingDialog dialog = new RefreshingDialog
                {
                    Owner = Application.Current.MainWindow
                };

                var viewModel = new IOModuleRefreshing(ModifiedAnalogIO.OriginalAnalogIO);

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

                RaisePropertyChanged("Configured");
                RaisePropertyChanged("Owned");
                RaisePropertyChanged("ModuleIdentity");

                RaisePropertyChanged("ProtectionMode");
            });
        }

        #endregion
    }
}
