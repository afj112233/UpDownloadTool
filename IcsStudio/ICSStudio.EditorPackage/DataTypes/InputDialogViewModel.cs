using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;

namespace ICSStudio.EditorPackage.DataTypes
{
    public class InputDialogViewModel:ViewModelBase
    {
        private bool? _dialogResult;

        public InputDialogViewModel(string value)
        {
            Description = value;
            OkCommand=new RelayCommand(ExecuteOkCommand);
            CancelCommand=new RelayCommand(ExecuteCancelCommand);
        }

        public bool? DialogResult
        {
            set { Set(ref _dialogResult , value); }
            get { return _dialogResult; }
        }

        public string Description { set; get; }

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
    }
}
