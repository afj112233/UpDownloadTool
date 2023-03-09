using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.Dialogs.STDialogs
{
    internal class FinalizeDialogViewModel:ViewModelBase
    {
        private bool? _dialogResult;
        public FinalizeDialogViewModel(IProgram program)
        {
            Line0 = $"Finalize all edits in program '{program.Name}'.";
            YesCommand = new RelayCommand(ExecuteYesCommand);
            NoCommand = new RelayCommand(ExecuteNoCommand);
            foreach (var routine in program.Routines)
            {
                var st = routine as STRoutine;
                if (st != null)
                {
                    if (st.TestCodeText!=null||st.PendingCodeText!=null)
                    {
                        EditedRoutines.Add(new EditRoutineInfo(st));
                    }
                }
            }
        }
        public string Line0 { get; }
        public RelayCommand YesCommand { get; }

        private void ExecuteYesCommand()
        {
            DialogResult = true;
        }
        public RelayCommand NoCommand { get; }

        private void ExecuteNoCommand()
        {
            DialogResult = false;
        }
        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }
        public List<EditRoutineInfo> EditedRoutines { get; } = new List<EditRoutineInfo>();
    }
}
