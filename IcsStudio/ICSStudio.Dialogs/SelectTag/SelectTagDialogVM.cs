using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Dialogs.Filter;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.Dialogs.SelectTag
{
    internal class SelectTagDialogVM:ViewModelBase
    {
        private bool? _dialogResult;

        public SelectTagDialogVM(FilterViewModel filterViewModel)
        {
            FilterViewModel = filterViewModel;
            PropertyChangedEventManager.AddHandler(filterViewModel,FilterViewModel_PropertyChanged,"Hide");
            OkCommand=new RelayCommand(ExecuteOkCommand);
            CancelCommand=new RelayCommand(ExecuteCancelCommand);
        }

        public override void Cleanup()
        {
            PropertyChangedEventManager.RemoveHandler(FilterViewModel, FilterViewModel_PropertyChanged, "Hide");
        }

        private void FilterViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            DialogResult = true;
        }

        public FilterViewModel FilterViewModel { get; }

        public bool? DialogResult
        {
            set { Set(ref _dialogResult , value); }
            get { return _dialogResult; }
        }

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
