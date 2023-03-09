using System.ComponentModel;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Gui.Annotations;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.Dialogs.WaitDialog
{
    public class FindingDialogVM : ViewModelBase
    {
        private string _findInfo;
        private readonly FindingMessage _message;
        private bool? _dialogResult;

        public FindingDialogVM(FindingMessage message)
        {
            _message = message;
            message.PropertyChanged += Message_PropertyChanged;
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
        }

        public override void Cleanup()
        {
            _message.PropertyChanged -= Message_PropertyChanged;
        }

        private void Message_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                FindInfo = _message.Message;
            });
        }

        public bool? DialogResult
        {
            set { Set(ref _dialogResult , value); }
            get { return _dialogResult; }
        }

        public string FindInfo
        {
            set { Set(ref _findInfo, value); }
            get { return _findInfo; }
        }

        public RelayCommand CancelCommand { get; }

        private void ExecuteCancelCommand()
        {
            DialogResult = false;
        }
    }

    public class FindingMessage : INotifyPropertyChanged
    {
        private string _message;

        public string Message
        {
            set
            {
                _message = value;
                OnPropertyChanged();
            }
            get { return _message; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
