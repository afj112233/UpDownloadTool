using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Project;
using Microsoft.VisualStudio.Shell;
using Button = System.Windows.Controls.Button;

namespace ICSStudio.ControllerOverviewPackage
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public sealed partial class ControllerOverviewViewModel : ViewModelBase
    {
        public readonly SolidColorBrush DefaultBrush = new SolidColorBrush(Colors.DarkGray);
        public readonly SolidColorBrush OKBrush = new SolidColorBrush(Colors.SpringGreen);
        public readonly SolidColorBrush FaultBrush = new SolidColorBrush(Colors.Red);

        private readonly Controller _controller;

        private readonly IProject _currentProject;

        private string _programMode;
        private string _runMode;
        private string _testMode;
        private string _batteryOk;
        private string _bat;
        private string _ioNotPresent;
        private string _offline;
        private string _goOffline;
        private string _goOnline;
        private string _none;

        public ControllerOverviewViewModel()
        {
            _controller = Controller.GetInstance();

            CommunicationMenuOpenedCommand = new RelayCommand<RoutedEventArgs>(ExecuteCommunicationMenuOpened);
            GoOnlineCommand = new RelayCommand(ExecuteGoOnline);
            UploadCommand = new RelayCommand(ExecuteUpload, CanExecuteUpload);
            DownloadCommand = new RelayCommand(ExecuteDownload, CanExecuteDownload);
            ChangeOperationModeCommand =
                new RelayCommand<ControllerOperationMode>(ExecuteChangeOperationMode, CanExecuteChangeOperation);
            ClearFaultsCommand = new RelayCommand(ExecuteClearFaults, CanExecuteClearFaults);
            GoToFaultsCommand = new RelayCommand(ExecuteGoToFaults, CanExecuteGoToFaults);
            ControllerPropertiesCommand = new RelayCommand(ExecuteControllerProperties, CanExecuteControllerProperties);
            ButtonCommand = new RelayCommand<Button>(ExecuteButtonCommand);

            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                _controller, "IsOnlineChanged", OnIsOnlineChanged);
            WeakEventManager<Controller, EventArgs>.AddHandler(
                _controller, "KeySwitchChanged", OnKeySwitchChanged);
            WeakEventManager<Controller, EventArgs>.AddHandler(
                _controller, "OperationModeChanged", OnOperationModeChanged);
            PropertyChangedEventManager.AddHandler(_controller, OnControllerPropertyChanged, string.Empty);


            IProjectInfoService projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            if (projectInfoService != null)
            {
                _currentProject = projectInfoService.CurrentProject;
                PropertyChangedEventManager.AddHandler(_currentProject, OnProjectPropertyChanged, string.Empty);
            }

            ChangeTextLanguage();

            WeakEventManager<LanguageManager,EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            ChangeTextLanguage();
        }

        private void ChangeTextLanguage()
        {
            _programMode = LanguageManager.GetInstance().ConvertSpecifier("Program Mode");
            _runMode = LanguageManager.GetInstance().ConvertSpecifier("Run Mode");
            _testMode = LanguageManager.GetInstance().ConvertSpecifier("Test Mode");
            _batteryOk = LanguageManager.GetInstance().ConvertSpecifier("Battery Ok");
            _bat = LanguageManager.GetInstance().ConvertSpecifier("Bat");
            _ioNotPresent = LanguageManager.GetInstance().ConvertSpecifier("I/O Not Present");
            _goOffline = LanguageManager.GetInstance().ConvertSpecifier("Logout");
            _offline = LanguageManager.GetInstance().ConvertSpecifier("Offline");
            _goOnline = LanguageManager.GetInstance().ConvertSpecifier("Login");
            _none = LanguageManager.GetInstance().ConvertSpecifier("None");

            Refresh();
        }

        private void ExecuteButtonCommand(Button obj)
        {
            var menu = obj.ContextMenu;
            if (menu != null)
            {
                menu.PlacementTarget = obj;
                menu.IsOpen = true;
            }
        }

        public SolidColorBrush OperationModeColor
        {
            get
            {
                if (_controller.IsConnected && _controller.OperationMode == ControllerOperationMode.OperationModeRun)
                    return OKBrush;

                return DefaultBrush;
            }
        }

        public string OperationModeDisplay
        {
            get
            {
                if (_controller.IsConnected)
                {
                    switch (_controller.OperationMode)
                    {
                        case ControllerOperationMode.OperationModeFaulted:
                            return _programMode;
                        case ControllerOperationMode.OperationModeRun:
                            return _runMode;
                        case ControllerOperationMode.OperationModeProgram:
                            return _programMode;
                        case ControllerOperationMode.OperationModeDebug:
                            return _testMode;
                    }
                }

                return "RUN";
            }
        }

        public SolidColorBrush ControllerStateColor
        {
            get
            {
                if (_controller.IsConnected)
                {
                    if (_controller.OperationMode == ControllerOperationMode.OperationModeFaulted)
                        return FaultBrush;

                    return OKBrush;
                }

                return DefaultBrush;
            }
        }

        public string ControllerStateDisplay
        {
            get
            {
                if (_controller.IsConnected)
                {
                    if (_controller.OperationMode == ControllerOperationMode.OperationModeFaulted)
                        return "Controller Fault";

                    return "Controller OK";
                }

                return "OK";
            }
        }

        public string BatteryStateDisplay
        {
            get
            {
                if (_controller.IsConnected)
                    return _batteryOk;

                return _bat;
            }
        }

        public string IOStateDisplay
        {
            get
            {
                if (_controller.IsConnected)
                    return _ioNotPresent;

                return "I/O";
            }
        }

        public string KeySwitchAndOperation
        {
            get
            {
                if (!_controller.IsConnected)
                    return _offline;

                if (_controller.OperationMode == ControllerOperationMode.OperationModeFaulted)
                    return "Faulted";

                if (_controller.KeySwitchPosition == ControllerKeySwitch.RemoteKeySwitch)
                {
                    switch (_controller.OperationMode)
                    {
                        case ControllerOperationMode.OperationModeNull:
                            break;
                        case ControllerOperationMode.OperationModeRun:
                            return "Rem Run";
                        case ControllerOperationMode.OperationModeProgram:
                            return "Rem Prog";
                        case ControllerOperationMode.OperationModeFaulted:
                            break;
                        case ControllerOperationMode.OperationModeDebug:
                            return "Rem Test";
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (_controller.KeySwitchPosition == ControllerKeySwitch.RunKeySwitch)
                {
                    return "Run";
                }

                if (_controller.KeySwitchPosition == ControllerKeySwitch.ProgramKeySwitch)
                {
                    return "Program";
                }

                return "Add Code here";
            }
        }

        public string RecentCommPath
        {
            get
            {
                if (_currentProject == null)
                    return _none;

                if (string.IsNullOrEmpty(_currentProject.RecentCommPath))
                    return _none;

                if (string.IsNullOrEmpty(_controller.ProjectCommunicationPath))
                    return _currentProject.RecentCommPath + "*";

                return _currentProject.RecentCommPath;
            }
        }

        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        private void Refresh()
        {
            RaisePropertyChanged("OperationModeColor");
            RaisePropertyChanged("OperationModeDisplay");
            RaisePropertyChanged("ControllerStateColor");
            RaisePropertyChanged("ControllerStateDisplay");

            RaisePropertyChanged("BatteryStateDisplay");
            RaisePropertyChanged("IOStateDisplay");

            RaisePropertyChanged("KeySwitchAndOperation");
            
            RaisePropertyChanged("GoOnlineMenu");
            RaisePropertyChanged("RecentCommPath");
        }

        private void OnOperationModeChanged(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
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


        private void OnProjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "RecentCommPath")
            {
                RaisePropertyChanged("RecentCommPath");
            }
        }

        private void OnControllerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ProjectCommunicationPath")
            {
                RaisePropertyChanged("RecentCommPath");
            }
        }
    }
}
