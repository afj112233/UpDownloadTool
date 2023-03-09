using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    internal class DateTimeBrowserViewModel:ViewModelBase
    {
        private bool? _dialogResult;

        public DateTimeBrowserViewModel(DateTime dateTime)
        {
            Hour = dateTime.Hour;
            Minute = dateTime.Minute;
            Second = dateTime.Second;
            Date = dateTime;
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

        public DateTime? GetTime()
        {
            if (Date == null) return null;
            return new DateTime(((DateTime) Date).Year, ((DateTime) Date).Month, ((DateTime) Date).Day, Hour, Minute,
                Second);
        }

        public bool? DialogResult
        {
            set { Set(ref _dialogResult , value); }
            get { return _dialogResult; }
        }

        public DateTime? Date { set; get; }
        public int Hour { set; get; }
        public int Minute { set; get; }
        public int Second { set; get; }
    }
}
