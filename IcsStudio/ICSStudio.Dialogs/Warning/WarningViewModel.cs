using System;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.MultiLanguage;

namespace ICSStudio.Dialogs.Warning
{
    class WarningViewModel : ViewModelBase
    {
        private bool? _dialogResult;
        private readonly string _message;
        private readonly string _reason;

        public WarningViewModel(string message, string reason, string errorCode)
        {
            _message = message;
            _reason = reason;

            WaringMessage=LanguageManager.GetInstance().ConvertSpecifier(_message);
            WaringReason=LanguageManager.GetInstance().ConvertSpecifier(_reason);

            ErrorCode = errorCode;

            OkCommand = new RelayCommand(ExecuteOkCommand);
            HelpCommand = new RelayCommand(ExecuteHelpCommand);

            WeakEventManager<LanguageManager,EventArgs>.AddHandler(LanguageManager.GetInstance(),"LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            WaringMessage = LanguageManager.GetInstance().ConvertSpecifier(_message);
            WaringReason = LanguageManager.GetInstance().ConvertSpecifier(_reason);
        }

        public string WaringMessage { get; set; }
        public string WaringReason { get; set; }
        public string ErrorCode { get; set; }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        public RelayCommand OkCommand { get; }
        public RelayCommand HelpCommand { get; }

        private void ExecuteOkCommand()
        {
            DialogResult = true;
        }

        private void ExecuteHelpCommand()
        {
            //TODO(gjc): add code here
        }
    }
}
