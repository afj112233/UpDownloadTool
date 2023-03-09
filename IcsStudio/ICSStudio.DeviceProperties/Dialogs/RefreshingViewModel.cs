using GalaSoft.MvvmLight;
using ICSStudio.Cip.Other;

namespace ICSStudio.DeviceProperties.Dialogs
{
    internal abstract class RefreshingViewModel : ViewModelBase
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

        public IdentityDescriptor Descriptor { get; protected set; }

        public abstract void Refresh();
    }
}
