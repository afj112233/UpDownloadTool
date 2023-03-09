using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    class ChangeControllerViewModel : ViewModelBase
    {
        private LocalModule _old;
        private bool? _dialogResult;

        public ChangeControllerViewModel()
        {
            _old = Controller.GetInstance().DeviceModules["Local"] as LocalModule;
            Old = _old?.CatalogNumber;
            Type = _old?.DisplayText;
            OkCommand = new RelayCommand(ExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);

            TypeList = new List<string>()
            {
                "ICC-B010ERM", "ICC-B020ERM", "ICC-B030ERM", "ICC-B050ERM", "ICC-B080ERM", "ICC-B0100ERM",
                "ICC-P010ERM", "ICC-P020ERM", "ICC-P030ERM", "ICC-P050ERM", "ICC-P080ERM", "ICC-P0100ERM",
                "ICC-T0100ERM"
            };
            SelectedType = TypeList.Contains(_old.CatalogNumber)?_old.CatalogNumber:TypeList[0];
        }

        public string Old { get; }

        public string SelectedType { set; get; }

        public List<string> TypeList { get; }

        public bool? DialogResult
        {
            set { Set(ref _dialogResult, value); }
            get { return _dialogResult; }
        }

        public string Type { get; set; }

        public RelayCommand OkCommand { get; }

        private void ExecuteOkCommand()
        {
            DialogResult = true;
        }

        public RelayCommand CancelCommand { get; }

        private void ExecuteCancelCommand()
        {
            DialogResult = false;
        }
    }
}
