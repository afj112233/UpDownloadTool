using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ICSStudio.ToolsPackage.SourceProtection
{
    public class ProtectWarningViewModel : ViewModelBase
    {
        private bool? _dialogResult;

        public ProtectWarningViewModel(List<string> reasonList)
        {
            Reasons = new ObservableCollection<string>(reasonList);

            OkCommand = new RelayCommand(ExecuteOk);
        }

        public ObservableCollection<string> Reasons { get; }

        public RelayCommand OkCommand { get; }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        private void ExecuteOk()
        {
            DialogResult = true;
        }
    }
}
