using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.DeviceProperties.Adapters.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class ChassisSizeViewModel : DeviceOptionPanel
    {
        private int _chassisSizeInModule;
        private bool _isChassisSizeUpdated;

        public ChassisSizeViewModel(UserControl control, ModifiedDIOEnetAdapter modifiedAdapter) : base(control)
        {
            ModifiedAdapter = modifiedAdapter;

            SetChassisSizeCommand =
                new RelayCommand(ExecuteSetChassisSizeCommand, CanExecuteSetChassisSizeCommand);
            RefreshCommand =
                new RelayCommand(ExecuteRefreshCommand, CanExecuteRefreshCommand);
        }

        private bool CanExecuteRefreshCommand()
        {
            if (!ModifiedAdapter.Controller.IsOnline)
                return false;

            if (!_isChassisSizeUpdated)
                return false;

            return true;
        }

        private void ExecuteRefreshCommand()
        {
            RefreshChassisSize();
        }

        private bool CanExecuteSetChassisSizeCommand()
        {
            if (!ModifiedAdapter.Controller.IsOnline)
                return false;

            if (!_isChassisSizeUpdated)
                return false;

            if (ChassisSizeFromGeneralTab == ChassisSizeInModule)
                return false;

            return true;
        }

        private void ExecuteSetChassisSizeCommand()
        {
            if (MessageBox.Show(LanguageManager.GetInstance().ConvertSpecifier("Modify Chassis Size") + $" {ChassisSizeFromGeneralTab}!",
                LanguageManager.GetInstance().ConvertSpecifier("Set Chassis Size"), MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
                {
                    int result = await SetChassisSize((ushort)ChassisSizeFromGeneralTab);

                    if (result == 0)
                    {
                        RefreshChassisSize();
                    }

                });
            }
        }

        public ModifiedDIOEnetAdapter ModifiedAdapter { get; }

        public IController Controller => ModifiedAdapter.Controller;

        public int ChassisSizeFromGeneralTab => ModifiedAdapter.ChassisSize;

        public int ChassisSizeInModule
        {
            get { return _chassisSizeInModule; }
            set
            {
                Set(ref _chassisSizeInModule, value);

                SetChassisSizeCommand.RaiseCanExecuteChanged();
            }
        }

        public Visibility ChassisSizeVisibility => Controller.IsOnline ? Visibility.Visible : Visibility.Hidden;

        public RelayCommand SetChassisSizeCommand { get; }
        public RelayCommand RefreshCommand { get; }

        public override void Show()
        {
            RaisePropertyChanged("ChassisSizeVisibility");
            RaisePropertyChanged("ChassisSizeFromGeneralTab");

            SetChassisSizeCommand.RaiseCanExecuteChanged();
            RefreshCommand.RaiseCanExecuteChanged();

            if (!Controller.IsOnline)
            {
                _isChassisSizeUpdated = false;
            }
            else
            {
                UpdateChassisSize();
            }
        }

        private void UpdateChassisSize()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                if (_isChassisSizeUpdated)
                    return;

                try
                {
                    await TaskScheduler.Default;

                    int chassisSize = await GetChassisSize();

                    if (chassisSize < 0)
                        return;

                    _isChassisSizeUpdated = true;

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    ChassisSizeInModule = chassisSize;
                    RaisePropertyChanged("ChassisSizeVisibility");
                    SetChassisSizeCommand.RaiseCanExecuteChanged();
                    RefreshCommand.RaiseCanExecuteChanged();

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }

            });

        }

        private void RefreshChassisSize()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                try
                {
                    await TaskScheduler.Default;

                    int chassisSize = await GetChassisSize();

                    if (chassisSize < 0)
                        return;

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    ChassisSizeInModule = chassisSize;
                    RaisePropertyChanged("ChassisSizeVisibility");
                    SetChassisSizeCommand.RaiseCanExecuteChanged();
                    RefreshCommand.RaiseCanExecuteChanged();

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }

            });
        }

        private async Task<int> GetChassisSize()
        {
            Controller myController = Controller as Controller;
            if (myController != null)
            {
                ICipMessager messager = await myController.GetMessager(ModifiedAdapter.OriginalAdapter);

                if (messager == null)
                    return -1;

                var request = CreateGetChassisSizeRequest();
                var response = await messager.SendRRData(request);
                if ((response != null) && (response.GeneralStatus == (byte) CipGeneralStatusCode.Success))
                {
                    ushort chassisSize = BitConverter.ToUInt16(response.ResponseData, 0);
                    return chassisSize;
                }

                return -1;

            }

            return -1;
        }

        private async Task<int> SetChassisSize(ushort chassisSize)
        {
            Controller myController = Controller as Controller;
            if (myController != null)
            {
                ICipMessager messager = await myController.GetMessager(ModifiedAdapter.OriginalAdapter);

                if (messager == null)
                    return -1;

                var request = CreateSetChassisSizeRequest(chassisSize);
                var response = await messager.SendRRData(request);
                if (response != null
                    && response.GeneralStatus == (byte) CipGeneralStatusCode.Success)
                {
                    return 0;
                }

                return -1;

            }

            return -1;
        }

        private IMessageRouterRequest CreateSetChassisSizeRequest(ushort chassisSize)
        {
            return new SetChassisSizeRequest(chassisSize);
        }

        private IMessageRouterRequest CreateGetChassisSizeRequest()
        {
            return new GetChassisSizeRequest();
        }


        private class GetChassisSizeRequest : IMessageRouterRequest
        {
            public byte[] ToByteArray()
            {
                string message = "0e,04,21,00,00,03,24,01,30,01";

                return message.Split(',').Select(item => Convert.ToByte(item, 16)).ToArray();
            }
        }

        private class SetChassisSizeRequest : IMessageRouterRequest
        {
            private readonly ushort _chassisSize;

            public SetChassisSizeRequest(ushort chassisSize)
            {
                _chassisSize = chassisSize;
            }

            public byte[] ToByteArray()
            {
                string message = "10,04,21,00,00,03,24,01,30,01";

                var list = message.Split(',').Select(item => Convert.ToByte(item, 16)).ToList();

                list.AddRange(BitConverter.GetBytes(_chassisSize));

                return list.ToArray();
            }
        }
    }
}
