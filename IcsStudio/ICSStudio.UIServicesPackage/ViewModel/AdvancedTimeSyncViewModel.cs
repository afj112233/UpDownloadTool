using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    internal class AdvancedTimeSyncViewModel:ViewModelBase
    {
        private bool? _dialogResult;

        public AdvancedTimeSyncViewModel()
        {
            OkCommand=new RelayCommand(ExecuteOkCommand);
            CancelCommand=new RelayCommand(ExecuteCancelCommand);
        }

        public RelayCommand OkCommand { set; get; }

        private void ExecuteOkCommand()
        {
            DialogResult = true;
        }

        public RelayCommand CancelCommand { set; get; }

        private void ExecuteCancelCommand()
        {
            DialogResult = false;
        }

        public bool? DialogResult
        {
            set { Set(ref _dialogResult , value); }
            get { return _dialogResult; }
        }
    }
}
