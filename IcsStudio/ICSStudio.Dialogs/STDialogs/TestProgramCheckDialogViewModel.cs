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
    internal class TestProgramCheckDialogViewModel:ViewModelBase
    {
        private bool? _dialogResult;
        public TestProgramCheckDialogViewModel(IProgram program,bool isTest)
        {
            string keyword = isTest ? "Test" : "Untest";
            Line0 = $"{keyword} Edits for program '{program.Name}'.";
            Line4 =
                $"The {keyword} Accepted Program Edits operation will leave the following\noutputs in their last state:";
            Line6 = $"{keyword} accepted program edits?";
            YesCommand=new RelayCommand(ExecuteYesCommand);
            NoCommand=new RelayCommand(ExecuteNoCommand);
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
        public string Line0 { get; }
        public string Line4 { get; }
        public string Line6 { get; }
        public List<EditRoutineInfo> EditedRoutines { get; }=new List<EditRoutineInfo>();
    }

    internal class EditRoutineInfo
    {
        public EditRoutineInfo(IRoutine routine)
        {
            Name = routine.Name;
            Type = routine.Type;
        }

        public string Name { get; }
        public RoutineType Type {  get; }
    }
}
