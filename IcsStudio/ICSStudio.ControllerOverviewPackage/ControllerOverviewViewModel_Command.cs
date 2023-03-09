using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using ICSStudio.CommunicationsPackage.View;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.UIInterfaces.Command;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIInterfaces.Project;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ICSStudio.ControllerOverviewPackage
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public sealed partial class ControllerOverviewViewModel
    {
        public RelayCommand<RoutedEventArgs> CommunicationMenuOpenedCommand { get; }

        public string GoOnlineMenu
        {
            get
            {
                if (_controller != null && _controller.IsOnline)
                    return _goOffline;

                return _goOnline;
            }
        }

        public RelayCommand GoOnlineCommand { get; }
        public RelayCommand UploadCommand { get; }
        public RelayCommand DownloadCommand { get; }
        public RelayCommand<ControllerOperationMode> ChangeOperationModeCommand { get; }
        public RelayCommand ClearFaultsCommand { get; }
        public RelayCommand GoToFaultsCommand { get; }
        public RelayCommand ControllerPropertiesCommand { get; }
        public RelayCommand<Button> ButtonCommand { get; }
     

        private void ExecuteCommunicationMenuOpened(RoutedEventArgs args)
        {
            RaisePropertyChanged("GoOnlineMenu");
            ControllerPropertiesCommand.RaiseCanExecuteChanged();
            UploadCommand.RaiseCanExecuteChanged();
            DownloadCommand.RaiseCanExecuteChanged();
            ChangeOperationModeCommand.RaiseCanExecuteChanged();
            ClearFaultsCommand.RaiseCanExecuteChanged();
            GoToFaultsCommand.RaiseCanExecuteChanged();
        }

        private bool CanExecuteControllerProperties()
        {
            if (_controller != null && !string.IsNullOrEmpty(_controller.ProjectLocaleName))
            {
                return true;
            }

            return false;
        }

        private void ExecuteControllerProperties()
        {
            var service = Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            if (service != null && uiShell != null)
            {
                var window = service.CreateControllerProperties(_controller);
                window?.Show(uiShell);
            }
        }

        private void ExecuteGoOnline()
        {
            var projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            var controller = projectInfoService?.Controller;
            var currentProject = projectInfoService?.CurrentProject;

            if (controller != null && currentProject != null && !string.IsNullOrEmpty(currentProject.RecentCommPath))
            {
                var commandService =
                    Package.GetGlobalService(typeof(SCommandService)) as ICommandService;

                commandService?.GoOnlineOrOffline(controller, currentProject.RecentCommPath);
            }
            else
            {
                var dialog = new WhoActiveDialog
                {
                    Owner = Application.Current.MainWindow
                };

                var result = dialog.ShowDialog();

                if (result.HasValue)
                {
                    var mainWindow = Application.Current.MainWindow;
                    if (mainWindow != null)
                    {
                        if (!mainWindow.IsActive)
                            mainWindow.Activate();
                    }
                }
            }
        }

        private void ExecuteUpload()
        {
            var commandService =
                Package.GetGlobalService(typeof(SCommandService)) as ICommandService;

            var projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            var currentProject = projectInfoService?.CurrentProject;

            string commPath = string.Empty;
            if (currentProject != null)
                commPath = currentProject.RecentCommPath;

            commandService?.Upload(_controller, commPath);
        }

        private bool CanExecuteUpload()
        {
            if (_controller != null && _controller.IsOnline)
                return false;

            return true;
        }

        private void ExecuteDownload()
        {
            var commandService =
                Package.GetGlobalService(typeof(SCommandService)) as ICommandService;

            var projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            var currentProject = projectInfoService?.CurrentProject;

            string commPath = string.Empty;
            if (currentProject != null)
                commPath = currentProject.RecentCommPath;

            commandService?.Download(_controller, commPath);
        }

        private bool CanExecuteDownload()
        {
            if (_controller == null)
                return false;

            if (string.IsNullOrEmpty(_controller.ProjectLocaleName))
                return false;

            return !_controller.IsOnline;
        }

        private void ExecuteChangeOperationMode(ControllerOperationMode mode)
        {
            var commandService =
                Package.GetGlobalService(typeof(SCommandService)) as ICommandService;

            commandService?.ChangeOperationMode(_controller, mode);
        }

        private bool CanExecuteChangeOperation(ControllerOperationMode mode)
        {
            if (_controller == null)
                return false;

            if (!_controller.IsOnline)
                return false;

            //OperationModeDebug模式（Test Mode模式），目前还没有开发，暂时禁用Controller Overview下的Test Mode
            if (mode == ControllerOperationMode.OperationModeDebug) return false;

            if (_controller.KeySwitchPosition == ControllerKeySwitch.RemoteKeySwitch)
            {
                if (_controller.OperationMode == ControllerOperationMode.OperationModeProgram 
                    || _controller.OperationMode == ControllerOperationMode.OperationModeRun 
                    || _controller.OperationMode == ControllerOperationMode.OperationModeDebug)
                {
                    if (_controller.OperationMode != mode)
                        return true;
                }
            }

            return false;
        }

        private void ExecuteClearFaults()
        {
            var commandService =
                Package.GetGlobalService(typeof(SCommandService)) as ICommandService;

            commandService?.ClearFaults(_controller);
        }

        private bool CanExecuteClearFaults()
        {
            if (_controller != null && _controller.IsOnline)
            {
                if (_controller.OperationMode == ControllerOperationMode.OperationModeFaulted)
                    return true;
            }

            return false;
        }

        private void ExecuteGoToFaults()
        {
            var service = Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            if (service != null && uiShell != null)
            {
                var window = service.CreateControllerProperties(_controller, 1);

                window?.Show(uiShell);

            }
        }

        private bool CanExecuteGoToFaults()
        {
            if (_controller != null && _controller.IsOnline)
            {
                if (_controller.OperationMode == ControllerOperationMode.OperationModeFaulted)
                    return true;
            }

            return false;
        }
    }
}
