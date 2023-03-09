using GalaSoft.MvvmLight;

namespace ICSStudio.DeviceProperties.Dialogs
{
    internal abstract class ResettingViewModel : ViewModelBase
    {
        private bool? _dialogResult;

        private string _message;

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        public string Message
        {
            get { return _message; }
            set { Set(ref _message, value); }
        }

        public abstract void Reset();
    }
}
