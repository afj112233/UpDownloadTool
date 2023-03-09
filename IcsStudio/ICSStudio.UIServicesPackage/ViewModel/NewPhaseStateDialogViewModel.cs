using System;
using System.Collections;
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
    class NewPhaseStateDialogViewModel : ViewModelBase
    {
        private IProgramModule _program;
        private bool? _dialogResult;

        public NewPhaseStateDialogViewModel(IProgramModule program)
        {
            _program = program;
            OkCommand = new RelayCommand(ExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            StateType = new List<string>() {"Aborting", "Resetting", "Restarting", "Running", "Stopping"};
            foreach (var routine in program.Routines)
            {
                if (routine.Name == "Aborting" || routine.Name == "Resetting" || routine.Name == "Restarting" ||
                    routine.Name == "Running" ||
                    routine.Name == "Stopping")
                {
                    StateType.Remove(routine.Name);
                }
            }

            SelectedState = StateType[0];

            List<RoutineType> listRoutineTypes = new List<RoutineType>()
                {RoutineType.RLL, RoutineType.SFC, RoutineType.FBD, RoutineType.ST};
            TypeList = listRoutineTypes.Select(x =>
                {
                    var name = "";
                    switch (x)
                    {
                        case RoutineType.RLL:
                            name = "Ladder Diagram";
                            break;
                        case RoutineType.SFC:
                            name = "Sequential Function Chart";
                            break;
                        case RoutineType.FBD:
                            name = "Function Block Diagram";
                            break;
                        case RoutineType.ST:
                            name = "Structured Text";
                            break;
                    }

                    return new
                    {
                        Value = x,
                        DisplayName = name
                    };
                }
            ).ToList();
            SelectedType = RoutineType.RLL;
            ProgramName = program.Name;
        }
        public string ProgramName { set; get; }
        public string Description { set; get; }
        public List<string> StateType { set; get; }
        public string SelectedState { set; get; }
        public IList TypeList { set; get; }
        public RoutineType SelectedType { set; get; }
        public RelayCommand OkCommand { set; get; }

        public void ExecuteOkCommand()
        {
            if (SelectedType == RoutineType.RLL)
            {
                RLLRoutine rllRoutine = new RLLRoutine(_program.ParentController)
                {
                    Name = SelectedState,
                    Description = Description,
                    IsMainRoutine = false,
                    IsFaultRoutine = false,
                };
                ((Program) _program).AddRoutine(rllRoutine);
            }

            if (SelectedType == RoutineType.SFC)
            {
                SFCRoutine sfcRoutine = new SFCRoutine(_program.ParentController)
                {
                    Name = SelectedState,
                    Description = Description,
                    IsMainRoutine = false,
                    IsFaultRoutine = false,
                };
                ((Program) _program).AddRoutine(sfcRoutine);
            }

            if (SelectedType == RoutineType.FBD)
            {
                FBDRoutine fbdRoutine = new FBDRoutine(_program.ParentController,null)
                {
                    Name = SelectedState,
                    Description = Description,
                    IsMainRoutine = false,
                    IsFaultRoutine = false,
                };
                ((Program) _program).AddRoutine(fbdRoutine);
            }

            if (SelectedType == RoutineType.ST)
            {
                STRoutine stRoutine = new STRoutine(_program.ParentController)
                {
                    Name = SelectedState,
                    Description = Description,
                    IsMainRoutine = false,
                    IsFaultRoutine = false,
                };
                ((Program) _program).AddRoutine(stRoutine);
            }

            DialogResult = true;
        }

        public RelayCommand CancelCommand { set; get; }

        public void ExecuteCancelCommand()
        {
            DialogResult = false;
        }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }
    }
}
