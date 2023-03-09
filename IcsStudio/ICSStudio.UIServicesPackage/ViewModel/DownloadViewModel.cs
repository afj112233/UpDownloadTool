using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.Objects;
using ICSStudio.Dialogs.GlobalSetting;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;
using ICSStudio.MultiLanguage;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    public class DownloadViewModel : ViewModelBase
    {
        private bool? _dialogResult;

        private bool _timeSyncEnabled;

        private readonly Controller _controller;

        private bool _isPreserve;
        private bool _isRestore;
        private bool _isBackUp;
        private bool _isPreserveEnabled;

        private string _preserveFilePath;
        private string _restoreFilePath;

        public DownloadViewModel(IController controller)
        {
            _controller = controller as Controller;
            Name = LanguageManager.GetInstance().ConvertSpecifier("DownloadOfflineProject") + controller.Name +
                   LanguageManager.GetInstance().ConvertSpecifier("ToTheController");

            _isBackUp = false;

            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await RefreshDefaultSettingAsync(controller as Controller);
            });

            DownloadCommand = new RelayCommand(ExecuteDownload, CanExecuteDownload);
            CancelCommand = new RelayCommand(ExecuteCancel);

            PreserveCommand = new RelayCommand(ExecutePreserve, CanExecutePreserve);
            RestoreCommand = new RelayCommand(ExecuteRestore, CanExecuteRestore);

            WeakEventManager<Controller, EventArgs>.AddHandler(
                _controller, "KeySwitchChanged", OnKeySwitchChanged);
            WeakEventManager<Controller, EventArgs>.AddHandler(
                _controller, "OperationModeChanged", OnOperationModeChanged);

            UpdateSpecialDownloadOptions();

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            
        }

        private void UpdateSpecialDownloadOptions()
        {
            _downloadOptionsVisibility = Visibility.Visible;
            _isOnlineChange = false;
            _isDownload = true;
        }

        private async Task RefreshDefaultSettingAsync(Controller controller)
        {
            CIPController cipController = new CIPController(0, controller.CipMessager);
            var res = await cipController.GetHasProject();
            if (res < 1)
            {
                _isPreserve = false;
                _isPreserveEnabled = false;
            }
            else
            {
                _isPreserveEnabled = true;
                _isPreserve = GlobalSetting.GetInstance().DownloadSetting.IsPreserve;
                _isRestore = GlobalSetting.GetInstance().DownloadSetting.IsRestore;
            }

            if (_isPreserve)
                _isRestore = !_isPreserve;

            RaisePropertyChanged(nameof(IsPreserve));
            RaisePropertyChanged(nameof(IsRestore));
            RaisePropertyChanged(nameof(IsPreserveEnabled));
            RaisePropertyChanged(nameof(BackUpIsEnable));
        }

        private void ExecuteRestore()
        {
            _restoreFilePath = "";
            OpenFileDialog openDlg = new OpenFileDialog()
            {
                Title = @"Load From File",
                Filter = @"json文件(*.json)|*.json"
            };

            if (openDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _restoreFilePath = openDlg.FileName;
            }

            RaisePropertyChanged(nameof(RestoreFilePath));
        }

        private void ExecutePreserve()
        {

            _preserveFilePath = "";
            SaveFileDialog saveDlg = new SaveFileDialog()
            {
                Title = @"Store File",
                Filter = @"json文件(*.json)|*.json"
            };

            if (saveDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _preserveFilePath = saveDlg.FileName;
            }

            RaisePropertyChanged(nameof(PreserveFilePath));
        }

        private bool CanExecuteRestore()
        {
            return _isRestore;
        }

        private bool CanExecutePreserve()
        {
            return (_isPreserve && _isBackUp);
        }

        public string Name { get; }



        public bool IsPreserveEnabled
        {
            get { return _isPreserveEnabled; }
        }

        public bool IsPreserve
        {
            get { return _isPreserve; }
            set
            {
                if (_isPreserve != value)
                {
                    _isPreserve = value;
                    GlobalSetting.GetInstance().DownloadSetting.IsPreserve = value;

                    if (_isPreserve)
                    {
                        _isRestore = false;
                        GlobalSetting.GetInstance().DownloadSetting.IsRestore = _isRestore;
                    }
                    
                    RaisePropertyChanged(nameof(IsRestore));
                    RaisePropertyChanged(nameof(BackUpIsEnable));
                    RaisePropertyChanged(nameof(RestoreFileTextIsEnable));
                    RaisePropertyChanged(nameof(PreserveFileTextIsEnable));

                    RestoreCommand.RaiseCanExecuteChanged();
                    PreserveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsRestore
        {
            get { return _isRestore; }
            set
            {
                if (_isRestore != value)
                {
                    _isRestore = value;
                    GlobalSetting.GetInstance().DownloadSetting.IsRestore = value;

                    if (_isRestore)
                    {
                        _isPreserve = false;
                        GlobalSetting.GetInstance().DownloadSetting.IsPreserve = _isPreserve;
                    }
                    
                    RaisePropertyChanged(nameof(IsPreserve));
                    RaisePropertyChanged(nameof(BackUpIsEnable));
                    RaisePropertyChanged(nameof(RestoreFileTextIsEnable));
                    RaisePropertyChanged(nameof(PreserveFileTextIsEnable));

                    RestoreCommand.RaiseCanExecuteChanged();
                    PreserveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsBackUp
        {
            get { return _isBackUp; }
            set
            {
                if (_isBackUp != value)
                {
                    _isBackUp = value;

                    RaisePropertyChanged(nameof(PreserveFileTextIsEnable));

                    PreserveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool BackUpIsEnable
        {
            get { return _isPreserve; }
        }

        public bool PreserveFileTextIsEnable
        {
            get { return _isPreserve && _isBackUp; }
        }

        public bool RestoreFileTextIsEnable
        {
            get { return _isRestore; }
        }

        public string PreserveFilePath
        {
            get { return _preserveFilePath; }
            set { Set(ref _preserveFilePath, value); }
        }

        public string RestoreFilePath
        {
            get { return _restoreFilePath; }
            set { Set(ref _restoreFilePath, value); }
        }

        #region Special for download optinons

        private Visibility _downloadOptionsVisibility;

        public Visibility DownloadOptionsVisibility
        {
            get { return _downloadOptionsVisibility; }
        }

        private bool _isOnlineChange;

        public bool IsOnlineChange
        {
            get { return _isOnlineChange; }
            set { Set(ref _isOnlineChange, value); }
        }

        private bool _isDownload;

        public bool IsDownload
        {
            get { return _isDownload; }
            set { Set(ref _isDownload, value); }
        }

        #endregion


        public RelayCommand PreserveCommand { get; set; }
        public RelayCommand RestoreCommand { get; set; }

        public string Warning
        {
            get
            {
                if (KeySwitchPosition == ControllerKeySwitch.RunKeySwitch)
                {
                    return "The keyswitch is in the RUN position. Move it to REM or PROG in order to download.";
                }

                if (KeySwitchPosition == ControllerKeySwitch.RemoteKeySwitch
                    && OperationMode == ControllerOperationMode.OperationModeRun)
                {
                    return
                        "The controller is in Remote Run mode. The mode will be changed to Remote Program prior to download.";
                }

                return string.Empty;
            }
        }

        public ControllerKeySwitch KeySwitchPosition
        {
            get { return _controller.KeySwitchPosition; }
        }

        public ControllerOperationMode OperationMode
        {
            get { return _controller.OperationMode; }
        }

        public bool? DialogResult
        {
            set { Set(ref _dialogResult, value); }
            get { return _dialogResult; }
        }

        public RelayCommand DownloadCommand { get; }
        public RelayCommand CancelCommand { get; }

        public Visibility TimeSyncVisibility
        {
            get
            {
                if (_controller.HasMotionGroup())
                {
                    Controller controller = _controller;
                    if (controller != null && !controller.TimeSetting.PTPEnable)
                        return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
        }

        public Visibility IsChinese
        {
            get
            {
                if ("English".Equals(LanguageInfo.CurrentLanguage))
                {
                    return Visibility.Collapsed;
                }

                return Visibility.Visible;
            }
        }

        public Visibility IsEnglish
        {
            get
            {
                if ("English".Equals(LanguageInfo.CurrentLanguage))
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
        }

        public bool TimeSyncEnabled
        {
            get { return _timeSyncEnabled; }
            set { Set(ref _timeSyncEnabled, value); }
        }

        private void ExecuteCancel()
        {
            DialogResult = false;
        }

        private bool CanExecuteDownload()
        {
            if (KeySwitchPosition == ControllerKeySwitch.RunKeySwitch)
                return false;

            return true;
        }

        private void ExecuteDownload()
        {
            var isPathNull = CheckPathIsNull();

            if (!isPathNull)
            {
                if (TimeSyncEnabled)
                {

                    Controller controller = _controller;
                    if (controller != null)
                    {
                        controller.TimeSetting.PTPEnable = true;
                    }
                }
            
                DialogResult = !isPathNull;
            }
        }

        private bool CheckPathIsNull()
        {
            if (IsPreserve && IsBackUp)
            {
                if (_preserveFilePath == null)
                {
                    System.Windows.MessageBox.Show(LanguageManager.GetInstance().ConvertSpecifier("Chosen configuration requires path for backup file. Please correct it before downloading."),"ICS Designer" , MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return true;
                }
            }
            if (IsRestore)
            {
                if (_restoreFilePath == null)
                {
                    System.Windows.MessageBox.Show(LanguageManager.GetInstance().ConvertSpecifier("Chosen configuration requires path for restore file. Please correct it before downloading."), "ICS Designer", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return true;
                }
            }

            return false;
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

        private void Refresh()
        {
            RaisePropertyChanged(nameof(Warning));

            DownloadCommand.RaiseCanExecuteChanged();
        }
    }
}
