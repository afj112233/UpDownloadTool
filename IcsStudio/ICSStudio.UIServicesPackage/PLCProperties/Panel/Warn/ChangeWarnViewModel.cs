using System;
using System.Collections.Generic;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;

namespace ICSStudio.UIServicesPackage.PLCProperties.Panel.Warn
{
    class ChangeWarnViewModel : ViewModelBase
    {
        //private string _old;
        //private string _newOne;

        public ChangeWarnViewModel(string old,string newOne)
        {
            Main = "\n"
                   + LanguageManager.GetInstance().ConvertSpecifier("Changing the controller type from a")
                   + $" {old} "
                   + LanguageManager.GetInstance().ConvertSpecifier("to a")
                   + $"\n{newOne} "
                   + LanguageManager.GetInstance().ConvertSpecifier("will affect the project in a number of ways:");
            OkCommand = new RelayCommand(ExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
        }

        public List<string> TypeList { get; }

        public string Main { get; }

        public bool? DialogResult { set; get; }

        public RelayCommand OkCommand { get; }

        private void ExecuteOkCommand()
        {
            DialogResult = true;
            RaisePropertyChanged("DialogResult");
        }

        public RelayCommand CancelCommand { get; }

        private void ExecuteCancelCommand()
        {
            DialogResult = false;
            RaisePropertyChanged("DialogResult");
        }
    }
}
