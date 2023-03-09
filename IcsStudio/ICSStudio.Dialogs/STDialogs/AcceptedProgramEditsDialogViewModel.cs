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
    internal class AcceptedProgramEditsDialogViewModel:ViewModelBase
    {
        private bool? _dialogResult;
        public AcceptedProgramEditsDialogViewModel(IProgram program, bool isAccept)
        {
            string keyword = isAccept ? "Assemble" : "Cancel";
            Line0 = $"{keyword} edits for program '{program.Name}'.";
            Line3 = $"{keyword} accepted program edits?";
            YesCommand = new RelayCommand(ExecuteYesCommand);
            NoCommand = new RelayCommand(ExecuteNoCommand);
            foreach (var routine in program.Routines)
            {
                var st = routine as STRoutine;
                if (st != null)
                {
                    if (st.TestCodeText.Count > 0)
                    {
                        EditedRoutines.Add(new EditRoutineInfo(st));
                    }
                }
            }
        }
        public string Line0 { get; }
        public string Line3 { get; }
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
