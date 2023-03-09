using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    public class GraphTitleViewModel : ViewModelBase
    {
        private TrendObject _trend;
        private bool? _dialogResult;

        public GraphTitleViewModel(ITrend trend)
        {
            Title = trend.GraphTitle;
            _trend = trend as TrendObject;
            OkCommand = new RelayCommand(ExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
        }

        public bool? DialogResult
        {
            set { Set(ref _dialogResult, value); }
            get { return _dialogResult; }
        }

        public string Title { set; get; }

        public RelayCommand OkCommand { get; }

        private void ExecuteOkCommand()
        {
            _trend.GraphTitle = Title;
            DialogResult = true;
        }

        public RelayCommand CancelCommand { get; }

        private void ExecuteCancelCommand()
        {
            DialogResult = false;
        }
    }
}
