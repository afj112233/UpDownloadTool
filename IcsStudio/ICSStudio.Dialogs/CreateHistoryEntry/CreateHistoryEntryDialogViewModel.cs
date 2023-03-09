using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ICSStudio.Dialogs.CreateHistoryEntry
{
    public class CreateHistoryEntryDialogViewModel : ViewModelBase
    {
        private bool _dialogResult;
        public CreateHistoryEntryDialogViewModel()
        {
            OkCommand = new RelayCommand(ExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
        }

        public string Description { set; get; }

        public RelayCommand OkCommand { set; get; }

        public void ExecuteOkCommand()
        {
            DialogResult = true;
        }

        public RelayCommand CancelCommand { set; get; }

        public void ExecuteCancelCommand()
        {
            DialogResult = false;
        }

        public bool DialogResult
        {
            set { Set(ref _dialogResult, value); }
            get { return _dialogResult; }
        }
    }
}
