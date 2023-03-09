using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSGateway.Components.Common;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Command;
using ICSStudio.UIInterfaces.Project;
using Microsoft.VisualStudio.Shell;
using ICSStudio.MultiLanguage;

namespace ICSStudio.CommunicationsPackage.ViewModel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class WhoActiveViewModel : ViewModelBase
    {
        private string _communicationPath;
        private bool? _dialogResult;
        private bool _isBusy;
        private Node _selectedNode;
        
        private readonly BackgroundWorker _downloadWorker;

        public WhoActiveViewModel()
        {
            IProjectInfoService projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            CurrentProject = projectInfoService?.CurrentProject;

            Controller = SimpleServices.Common.Controller.GetInstance();
            var controller = (Controller) Controller;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }

            CloseCommand = new RelayCommand(ExecuteCloseCommand);
            SetProjectPathCommand =
                new RelayCommand(ExecuteSetProjectPathCommand, CanExecuteSetProjectPathCommand);
            ClearProjectPathCommand =
                new RelayCommand(ExecuteClearProjectPathCommand, CanExecuteClearProjectPathCommand);

            GoOnlineOrOfflineCommand =
                new RelayCommand(ExecuteGoOnlineOrOfflineCommand, CanExecuteGoOnlineOrOfflineCommand);
            UploadCommand = new RelayCommand(ExecuteUploadCommand, CanExecuteUploadCommand);
            DownloadCommand = new RelayCommand(ExecuteDownloadCommand, CanExecuteDownloadCommand);
            SelectedItemChangedCommand = new RelayCommand<Node>(ExecuteSelectedItemChangedCommand);

            Title = LanguageManager.GetInstance().ConvertSpecifier(Title);

            _downloadWorker = new BackgroundWorker();
            _downloadWorker.DoWork += DoDownloadWork;
            _downloadWorker.RunWorkerCompleted += OnDownloadWorkCompleted;
        }

        private void OnDownloadWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            GoOnlineOrOfflineCommand.RaiseCanExecuteChanged();
            UploadCommand.RaiseCanExecuteChanged();
            DownloadCommand.RaiseCanExecuteChanged();

            if (Controller.IsOnline)
            {
                DialogResult = true;
            }
        }

        private void DoDownloadWork(object sender, DoWorkEventArgs e)
        {
            var commandService =
                Package.GetGlobalService(typeof(SCommandService)) as ICommandService;

            commandService?.DownloadSync(Controller, CommunicationPath);
        }

        public string Title { get; } = "Net Path";
        public IController Controller { get; }

        public IProject CurrentProject { get; }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set { Set(ref _isBusy, value); }
        }

        public string CommunicationPath
        {
            get { return _communicationPath; }
            set
            {
                Set(ref _communicationPath, value);

                SetProjectPathCommand.RaiseCanExecuteChanged();
                GoOnlineOrOfflineCommand.RaiseCanExecuteChanged();
                UploadCommand.RaiseCanExecuteChanged();
                DownloadCommand.RaiseCanExecuteChanged();
            }
        }

        public string ProjectCommunicationPath
        {
            get
            {
                if (Controller == null)
                    return LanguageManager.GetInstance().ConvertSpecifier("<none>");

                if (string.IsNullOrWhiteSpace(Controller.ProjectCommunicationPath))
                    return LanguageManager.GetInstance().ConvertSpecifier("<none>");

                return Controller.ProjectCommunicationPath;
            }
        }

        public string GoOnlineOrOffline
        {
            get
            {
                if (Controller == null)
                    return LanguageManager.GetInstance().ConvertSpecifier("Login");

                if (Controller.IsOnline)
                    return LanguageManager.GetInstance().ConvertSpecifier("Logout");

                return LanguageManager.GetInstance().ConvertSpecifier("Login");
            }
        }

        public RelayCommand GoOnlineOrOfflineCommand { get; }
        public RelayCommand UploadCommand { get; }
        public RelayCommand DownloadCommand { get; }

        public RelayCommand CloseCommand { get; }
        public RelayCommand SetProjectPathCommand { get; }
        public RelayCommand ClearProjectPathCommand { get; }
        public RelayCommand<Node> SelectedItemChangedCommand { get; }

        private void ExecuteSelectedItemChangedCommand(Node selectedNode)
        {
            _selectedNode = selectedNode;
            CommunicationPath = _selectedNode.NetNodeItem.IP;
        }

        private bool IsIconDevice()
        {
            if (_selectedNode == null) return false;

            if (!(_selectedNode.NetNodeItem.Vendor == "1447" && _selectedNode.NetNodeItem.DeviceType == "14"))
                return false;

            return true;
        }

        private bool CanExecuteDownloadCommand()
        {
            if (!IsIconDevice())
                return false;

            if (Controller.IsOnline)
                return false;

            if (_downloadWorker.IsBusy)
                return false;

            if (!string.IsNullOrWhiteSpace(CommunicationPath) &&
                !string.IsNullOrWhiteSpace(Controller.ProjectLocaleName))
                return true;

            return false;
        }

        private void ExecuteDownloadCommand()
        {
            CurrentProject.RecentCommPath = CommunicationPath;

            _downloadWorker.RunWorkerAsync();

            DownloadCommand.RaiseCanExecuteChanged();

        }

        private bool CanExecuteUploadCommand()
        {
            if (!IsIconDevice()) return false;

            if (Controller.IsOnline)
                return false;

            return !string.IsNullOrWhiteSpace(CommunicationPath);
        }

        private void ExecuteUploadCommand()
        {
            CurrentProject.RecentCommPath = CommunicationPath;

            var commandService =
                Package.GetGlobalService(typeof(SCommandService)) as ICommandService;

            commandService?.Upload(Controller, CommunicationPath);
        }

        private bool CanExecuteGoOnlineOrOfflineCommand()
        {
            if (!IsIconDevice()) return false;

            return !string.IsNullOrWhiteSpace(CommunicationPath);
        }

        private void ExecuteGoOnlineOrOfflineCommand()
        {
            CurrentProject.RecentCommPath = CommunicationPath;

            var commandService =
                Package.GetGlobalService(typeof(SCommandService)) as ICommandService;

            commandService?.GoOnlineOrOffline(Controller, CommunicationPath);
        }

        private void ExecuteClearProjectPathCommand()
        {
            if (Controller != null)
            {
                Controller.ProjectCommunicationPath = string.Empty;

                SetProjectPathCommand.RaiseCanExecuteChanged();
                ClearProjectPathCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged("ProjectCommunicationPath");
            }
        }

        private bool CanExecuteClearProjectPathCommand()
        {
            if (string.IsNullOrWhiteSpace(Controller.ProjectLocaleName))
                return false;

            if (string.IsNullOrWhiteSpace(Controller.ProjectCommunicationPath))
                return false;

            return true;
        }

        private void ExecuteSetProjectPathCommand()
        {
            if (Controller != null)
            {
                Controller.ProjectCommunicationPath = CommunicationPath;

                SetProjectPathCommand.RaiseCanExecuteChanged();
                ClearProjectPathCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged("ProjectCommunicationPath");
            }
        }

        private bool CanExecuteSetProjectPathCommand()
        {
            if (string.IsNullOrWhiteSpace(Controller.ProjectLocaleName))
                return false;

            if (string.IsNullOrWhiteSpace(Controller.ProjectCommunicationPath) &&
                !string.IsNullOrWhiteSpace(CommunicationPath))
                return true;

            return false;
        }

        private void ExecuteCloseCommand()
        {
            DialogResult = false;
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                RaisePropertyChanged("GoOnlineOrOffline");

                UploadCommand.RaiseCanExecuteChanged();
                DownloadCommand.RaiseCanExecuteChanged();
            });
        }
    }
}