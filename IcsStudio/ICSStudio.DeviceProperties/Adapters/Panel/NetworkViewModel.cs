using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;

namespace ICSStudio.DeviceProperties.Adapters.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class NetworkViewModel : DeviceOptionPanel
    {
        private byte _topology;
        private byte _status;

        private readonly DLRDescriptor _descriptor;

        public NetworkViewModel(UserControl panel, ModifiedDIOEnetAdapter modifiedAdapter) : base(panel)
        {
            ModifiedAdapter = modifiedAdapter;

            RefreshCommand = new RelayCommand(ExecuteRefreshCommand, CanExecuteRefreshCommand);

            _descriptor = new DLRDescriptor();
        }

        public ModifiedDIOEnetAdapter ModifiedAdapter { get; }

        public IController Controller => ModifiedAdapter.Controller;

        public RelayCommand RefreshCommand { get; }

        public string NetworkTopology
        {
            get
            {
                if (!ModifiedAdapter.Controller.IsOnline)
                    return string.Empty;

                return _descriptor.GetNetworkTopology(_topology);
            }
        }

        public string NetworkStatus
        {
            get
            {
                if (!ModifiedAdapter.Controller.IsOnline)
                    return string.Empty;

                return _descriptor.GetNetworkStatus(_status);
            }
        }

        public Visibility NetworkVisibility
            => ModifiedAdapter.Controller.IsOnline ? Visibility.Visible : Visibility.Hidden;


        public override void Show()
        {
            RefreshCommand.RaiseCanExecuteChanged();

            if (Controller.IsOnline)
            {
                ExecuteRefreshCommand();
            }
            else
            {
                _topology = byte.MaxValue;
                _status = byte.MaxValue;

                RaisePropertyChanged("NetworkVisibility");
                RaisePropertyChanged("NetworkTopology");
                RaisePropertyChanged("NetworkStatus");
            }
        }


        private bool CanExecuteRefreshCommand()
        {
            if (!ModifiedAdapter.Controller.IsOnline)
                return false;

            //TODO(gjc): need edit later
            // check device status?

            return true;
        }

        private void ExecuteRefreshCommand()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                try
                {
                    await TaskScheduler.Default;

                    Controller myController = Controller as Controller;
                    if (myController != null)
                    {
                        ICipMessager messager = await myController.GetMessager(ModifiedAdapter.OriginalAdapter);
                        if (messager == null)
                        {
                            _topology = byte.MaxValue;
                            _status = byte.MaxValue;
                        }
                        else
                        {
                            CIPDLR cipDLR = new CIPDLR(1, messager);

                            _topology = await cipDLR.GetNetworkTopology();

                            _status = await cipDLR.GetNetWorkStatus();
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);

                    _topology = byte.MaxValue;
                    _status = byte.MaxValue;
                }
                finally
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    RefreshCommand.RaiseCanExecuteChanged();

                    RaisePropertyChanged("NetworkVisibility");
                    RaisePropertyChanged("NetworkTopology");
                    RaisePropertyChanged("NetworkStatus");
                }
            });
        }

    }
}
