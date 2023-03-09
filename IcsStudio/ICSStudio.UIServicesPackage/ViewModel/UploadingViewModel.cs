using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Dialogs.GlobalSetting;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    public class UploadingViewModel : ViewModelBase
    {
        private readonly Controller _controller;
        private readonly string _fileName;

        private bool? _dialogResult;

        private string _message;
        private double _progress;

        public UploadingViewModel(Controller controller, string fileName)
        {
            _controller = controller;
            _fileName = fileName;

            CancelCommand = new RelayCommand(ExecuteCancel, CanExecuteCancel);
        }

        public double Progress
        {
            set { Set(ref _progress , value); }
            get { return _progress; }
        }

        public string Message
        {
            get { return _message; }
            set { Set(ref _message, value); }
        }

        public void Upload()
        {
            //TODO(gjc): need edit here
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                int result = int.MinValue;
                try
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    Message = LanguageManager.GetInstance().ConvertSpecifier("UploadingProject");

                    await TaskScheduler.Default;
                    await _controller.Upload(false);

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    Progress += 25;
                    Message = LanguageManager.GetInstance().ConvertSpecifier("SavingProject");

                    await TaskScheduler.Default;
                    _controller.Save(_fileName, false);

                    _controller.ProjectLocaleName = _fileName;

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    Progress += 25;
                    Message = LanguageManager.GetInstance().ConvertSpecifier("SynchronizingDataForProject") +$" '{_controller.Name}'";

                    await TaskScheduler.Default;
                    result = await _controller.RebuildTagSyncControllerAsync();

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    Progress += 25;
                    Message = LanguageManager.GetInstance().ConvertSpecifier("UpdateControllerState");

                    await TaskScheduler.Default;
                    await _controller.UpdateState();
                }
                catch (Exception e)
                {
                    //ignore
                    _controller.Log($"upload failed:{e}");
                    result = -100;
                }
                finally
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    GlobalSetting.GetInstance().MonitorTagSetting.CheckFilterType();
                    DialogResult = result == 0;
                }


            });
        }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        public RelayCommand CancelCommand { get; }

        private bool CanExecuteCancel()
        {
            //TODO(gjc): add code here
            return false;
        }

        private void ExecuteCancel()
        {
            //TODO(gjc): add code here
        }
    }
}
