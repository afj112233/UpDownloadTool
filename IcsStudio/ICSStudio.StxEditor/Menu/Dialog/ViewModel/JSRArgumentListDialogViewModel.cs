using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.StxEditor.Menu.Dialog.ViewModel
{
    internal class JSRArgumentListDialogViewModel : ViewModelBase
    {
        private bool? _dialogResult;

        public JSRArgumentListDialogViewModel(IProgramModule program, string routineName)
        {
            OKCommand = new RelayCommand(ExecuteOKCommand);
            ApplyCommand = new RelayCommand(ExecuteApplyCommand, CanExecuteApplyCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            Routines.Add("<none>");
            var mainRoutine = program.Routines[program.MainRoutineName];
            foreach (var r in program.Routines)
            {
                if (r == mainRoutine) continue;
                Routines.Add(r.Name);
            }

            SelectedRoutine = Routines.Contains(routineName) ? routineName : "<none>";
            if (!string.IsNullOrEmpty(routineName) && SelectedRoutine == "<none>") _canExecuteApplyCommand = true;
            else _canExecuteApplyCommand = false;
        }

        public bool? DialogResult
        {
            set { Set(ref _dialogResult, value); }
            get { return _dialogResult; }
        }

        public RelayCommand OKCommand { get; }

        private void ExecuteOKCommand()
        {
            DialogResult = true;
        }

        public RelayCommand CancelCommand { get; }

        private void ExecuteCancelCommand()
        {
            DialogResult = false;
        }

        private bool _canExecuteApplyCommand = false;
        private string _selectedRoutine;

        public RelayCommand ApplyCommand { get; }

        private void ExecuteApplyCommand()
        {
            RaisePropertyChanged("IsUpdated");
            _canExecuteApplyCommand = false;
            ApplyCommand.RaiseCanExecuteChanged();
        }

        internal bool CanExecuteApplyCommand()
        {
            return _canExecuteApplyCommand;
        }

        public bool IsUpdated { set; get; }
        public List<string> Routines { get; } = new List<string>();

        public string SelectedRoutine
        {
            set
            {
                _selectedRoutine = value;
                _canExecuteApplyCommand = true;
                ApplyCommand.RaiseCanExecuteChanged();
            }
            get { return _selectedRoutine; }
        }
    }
}
