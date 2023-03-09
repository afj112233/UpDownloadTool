using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Online;
using ICSStudio.UIInterfaces.Error;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    class SavingToControllerViewModel : ViewModelBase
    {
        private readonly Controller _controller;

        private string _message;

        private bool? _dialogResult;

        public SavingToControllerViewModel(Controller controller)
        {
            _controller = controller;
        }

        public string Message
        {
            get { return _message; }
            set { Set(ref _message, value); }
        }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        public void SaveToController()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(
                async delegate
                {
                    var outputService = Package.GetGlobalService(typeof(SErrorOutputService)) as IErrorOutputService;

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    outputService?.AddMessages(BeginMessage, null);

                    await TaskScheduler.Default;

                    OnlineEditHelper onlineEditHelper = new OnlineEditHelper(_controller.CipMessager);
                    int result = await onlineEditHelper.SaveToController();

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    if (result == 0)
                    {
                        outputService?.AddMessages(
                            LanguageManager.GetInstance().ConvertSpecifier("Save to controller successfully."), null);

                        DialogResult = true;
                    }
                    else
                    {
                        outputService?.AddWarnings(
                            LanguageManager.GetInstance().ConvertSpecifier("Save to controller unsuccessfully!"), null);

                        DialogResult = false;
                    }

                });
        }

        private string BeginMessage
        {
            get
            {
                switch (LanguageInfo.CurrentLanguage)
                {
                    case "English":
                        return "Saving to Controller......";

                    case "简体中文":
                        return "保存至控制器......";

                    default:
                        return "Saving to Controller......";
                }
            }
        }

    }
}
