using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.CipConnection;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Online;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices;
using ICSStudio.UIInterfaces.Command;
using ICSStudio.UIInterfaces.Project;

namespace ICSStudio.UIServicesPackage
{
    public enum ConnectedToType
    {
        Login,
        Download,
        Upload
    }

    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public partial class ConnectedToViewModel : ViewModelBase
    {
        private readonly Controller _controller;

        private readonly CompareResult _compareResult;

        private bool? _dialogResult;
        
        private readonly MajorFaultHelper _majorFaultHelper;
        private readonly ConcurrentQueue<MajorFaultInfo> _majorFaultInfos;

        private Timer _getMajorFaultTimer;
        private string communicationPath;

        public ConnectedToViewModel(
            Controller controller, 
            ConnectedToType connectedToType, 
            CompareResult compareResult)
        {
            _controller = controller;

            ConnectedToType = connectedToType;

            _compareResult = compareResult;

            CancelCommand = new RelayCommand(ExecuteCancel);
            ClearMajorsCommand = new RelayCommand(ExecuteClearMajors, CanExecuteClearMajors);

            SelectFileCommand = new RelayCommand(ExecuteSelectFile, CanExecuteSelectFile);
            DownloadCommand = new RelayCommand(ExecuteDownload, CanExecuteDownload);
            UploadCommand = new RelayCommand(ExecuteUpload,CanExecuteUpload);

            UsePLCCommand = new RelayCommand(ExecuteCorrelate);
            UsePCCommand = new RelayCommand(ExecuteCorrelate);

            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                _controller, "IsOnlineChanged", OnIsOnlineChanged);
            WeakEventManager<Controller, EventArgs>.AddHandler(
                _controller, "KeySwitchChanged", OnKeySwitchChanged);
            WeakEventManager<Controller, EventArgs>.AddHandler(
                _controller, "OperationModeChanged", OnOperationModeChanged);

            _majorFaultHelper = new MajorFaultHelper(_controller.CipMessager);
            _majorFaultInfos = new ConcurrentQueue<MajorFaultInfo>();

            _majorFaultResult = LanguageManager.GetInstance().ConvertSpecifier("No major faults since last cleared.");
            _recentFaults = string.Empty;

            _getMajorFaultTimer = new Timer(500);
            _getMajorFaultTimer.Elapsed += GetMajorFaultHandle;
            _getMajorFaultTimer.AutoReset = false;
            communicationPath = (_controller.CipMessager as DeviceConnection)?.IpAddress;

            if (_controller.IsConnected)
            {
                StartGetMajorFaultTimer();
            }
            
            CreateInformation();
        }

        private bool CanExecuteDownload()
        {
            if (_controller == null)
                return false;

            //if (!_controller.IsConnected)
            //return false;
            if (_controller.IsOnline)
                return false;

            if (string.IsNullOrEmpty(_controller.ProjectLocaleName))
                return false;

            return true;
        }

        private bool CanExecuteUpload()
        {
            if (_controller == null)
                return false;

            //if (!_controller.IsConnected)
                //return false;
            if (_controller.IsOnline)
                return false;
            var projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            var currentProject = projectInfoService?.CurrentProject;
            return !string.IsNullOrEmpty(currentProject?.RecentCommPath);
        }

        private void ExecuteDownload()
        {
            if (_getMajorFaultTimer != null)
            {
                _getMajorFaultTimer.Stop();
                _getMajorFaultTimer.Elapsed -= GetMajorFaultHandle;
                _getMajorFaultTimer = null;
            }

            var commandService =
                Package.GetGlobalService(typeof(SCommandService)) as ICommandService;

            //DeviceConnection deviceConnection = _controller.CipMessager as DeviceConnection;
            //Debug.Assert(deviceConnection != null);

            //commandService?.DownloadSync(_controller, deviceConnection.IpAddress);
            commandService?.DownloadSync(_controller, communicationPath);

            if (_controller.IsOnline)
                DialogResult = _controller.IsOnline;
            UploadCommand.RaiseCanExecuteChanged();
            DownloadCommand.RaiseCanExecuteChanged();
        }

        private void ExecuteUpload()
        {
            if (_getMajorFaultTimer != null)
            {
                _getMajorFaultTimer.Stop();
                _getMajorFaultTimer.Elapsed -= GetMajorFaultHandle;
                _getMajorFaultTimer = null;
            }

            var commandService =
                Package.GetGlobalService(typeof(SCommandService)) as ICommandService;

            var projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            var currentProject = projectInfoService?.CurrentProject;

            string commPath = string.Empty;
            if (currentProject != null)
                commPath = currentProject.RecentCommPath;

            //Debug.Assert(_controller.CipMessager is DeviceConnection);
            commandService?.UploadSync(_controller, commPath);

            if (_controller.IsOnline)
                DialogResult = _controller.IsOnline;
            UploadCommand.RaiseCanExecuteChanged();
            DownloadCommand.RaiseCanExecuteChanged();
        }

        private bool CanExecuteSelectFile()
        {
            return false;
        }

        private void ExecuteSelectFile()
        {
            //TODO(gjc): add code here
        }

        public void ExecuteCorrelate()
        {
            if (_getMajorFaultTimer != null)
            {
                _getMajorFaultTimer.Stop();
                _getMajorFaultTimer.Elapsed -= GetMajorFaultHandle;
                _getMajorFaultTimer = null;
            }

            var commandService =
                Package.GetGlobalService(typeof(SCommandService)) as ICommandService;

            commandService?.CorrelateSync(_controller);

            DialogResult = _controller.IsOnline;
        }

        public ConnectedToType ConnectedToType { get; }


        public override void Cleanup()
        {
            if (_getMajorFaultTimer != null)
            {
                _getMajorFaultTimer.Stop();
                _getMajorFaultTimer.Elapsed -= GetMajorFaultHandle;
                _getMajorFaultTimer = null;
            }
            
            base.Cleanup();
        }

        public string Title
        {
            get
            {
                switch (ConnectedToType)
                {
                    case ConnectedToType.Login:
                        return "Connected To Login";
                    case ConnectedToType.Download:
                        return "Connected To Download";
                    case ConnectedToType.Upload:
                        return "Connected To Upload";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        public RelayCommand SelectFileCommand { get; }
        public RelayCommand DownloadCommand { get; }
        public RelayCommand UploadCommand { get; }
        public RelayCommand CancelCommand { get; }

        public RelayCommand UsePLCCommand { get; }
        public RelayCommand UsePCCommand { get; }

        private void ExecuteCancel()
        {
            DialogResult = false;
        }

        public Visibility DownloadVisibility
        {
            get
            {
                if (ConnectedToType == ConnectedToType.Login)
                    return Visibility.Visible;

                return Visibility.Visible;
            }
        }

        public Visibility UploadVisibility
        {
            get
            {
                if (ConnectedToType == ConnectedToType.Login)
                    return Visibility.Visible;

                return Visibility.Visible;
            }
        }

        public Visibility GoOnlineVisibility
        {
            get
            {
                if (ConnectedToType == ConnectedToType.Login)
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        public Visibility UsePLCVisibility
        {
            get
            {
                if (_compareResult == CompareResult.ControllerIsNewer)
                    return Visibility.Visible;

                return Visibility.Hidden;
            }
        }

        public Visibility UsePCVisibility
        {
            get
            {
                if (_compareResult == CompareResult.OfflineProjectIsNewer)
                    return Visibility.Visible;

                return Visibility.Hidden;
            }
        }

        [SuppressMessage("ReSharper", "NotAccessedVariable")]
        private void OnOperationModeChanged(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (_controller != null && _controller.IsConnected)
                {
                    if (_controller.OperationMode != ControllerOperationMode.OperationModeFaulted)
                    {
                        await _majorFaultHelper.Reset();

                        while (!_majorFaultInfos.IsEmpty)
                        {
                            MajorFaultInfo info;
                            _majorFaultInfos.TryDequeue(out info);
                        }

                        _majorFaultResult = LanguageManager.GetInstance()
                            .ConvertSpecifier("No major faults since last cleared.");
                        _recentFaults = string.Empty;
                    }
                }

                Refresh();
            });
        }

        private void OnKeySwitchChanged(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                Refresh();
            });
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                Refresh();
            });
        }

        private void Refresh()
        {
            ClearMajorsCommand.RaiseCanExecuteChanged();

            RaisePropertyChanged("MajorFaultResult");
            RaisePropertyChanged("RecentFaults");
        }

        private void GetMajorFaultHandle(object sender, ElapsedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                try
                {
                    await TaskScheduler.Default;

                    List<MajorFaultInfo> infos = new List<MajorFaultInfo>();
                    int result = await _majorFaultHelper.GetNextMajorFaultInfos(infos);

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    if (result == 0)
                        AddMajorFaultInfos(infos);

                    Refresh();

                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
                finally
                {
                    if (_controller.IsConnected)
                    {
                        _getMajorFaultTimer?.Start();
                    }
                }

            });
        }

        
    }
}
