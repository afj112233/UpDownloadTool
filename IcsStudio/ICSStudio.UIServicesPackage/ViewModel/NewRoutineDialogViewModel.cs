using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Dialogs.GlobalSetting;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    class NewRoutineDialogViewModel : ViewModelBase
    {
        private readonly RoutineCollection _routineCollection;
        private bool? _dialogResult;
        private readonly IProgramModule _program;
        private IProgramModule _selectedProgram;
        private int _count = 0;
        private RoutineType _selectedType;

        public NewRoutineDialogViewModel(IProgramModule program,string defaultName = default(string))
        {
            OkCommand = new RelayCommand(ExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);

            _program = program;
            _routineCollection = program.Routines as RoutineCollection;
            List<IProgram> list = new List<IProgram>();
            foreach (var item in program.ParentController.Programs)
            {
                if (item.Type != ProgramType.Sequence)
                {
                    list.Add(item);
                }
            }

            ProgramList = list.Select(val => new {DisplayName = val.Name, Value = val}).ToList();
            SelectedProgram = program;

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
            _selectedType = GlobalSetting.GetInstance().RoutineSetting.CurrentRoutineType;
            if (_selectedType == RoutineType.Typeless)
            {
                _selectedType = RoutineType.ST;
            }

            if (!string.IsNullOrEmpty(defaultName)) Name = defaultName;

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            
        }

        public void SetAssignment(IProgramModule program)
        {
            AssignmentList = new List<string>() {"<none>"};
            SelectedAssignment = "<none>";
            IsContains(program);
            if (_count == 0 && AssignmentList.Contains("Main"))
            {
                SelectedAssignment = "Main";
                _count++;
            }

            RaisePropertyChanged("AssignmentList");
            RaisePropertyChanged("SelectedAssignment");
        }

        public void IsContains(IProgramModule program)
        {
            if (program.Type == ProgramType.Phase)
            {
                AssignmentList.Add("Fault");
                AssignmentList.Add("Prestate");
            }
            else
            {
                AssignmentList.Add("Main");
                AssignmentList.Add("Fault");
            }

            foreach (var routine in program.Routines)
            {
                if (routine.IsFaultRoutine)
                {
                    AssignmentList.Remove("Fault");
                }

                if (routine.IsMainRoutine)
                {
                    if (program.Type == ProgramType.Phase)
                    {
                        AssignmentList.Remove("Prestate");
                    }
                    else
                    {
                        AssignmentList.Remove("Main");
                    }
                }
            }
        }

        public IList AssignmentList { set; get; }

        public RoutineType SelectedType
        {
            set { Set(ref _selectedType, value); }
            get { return _selectedType; }
        }

        public string SelectedAssignment { set; get; }
        public RelayCommand OkCommand { set; get; }

        public void ExecuteOkCommand()
        {
            if (IsValidTagName(Name))
            {
                //if (SelectedAssignment == "Main" || SelectedAssignment == "Prestate")
                //{
                //    ((Program)SelectedProgram).MainRoutineName = Name;
                //}
                //else if (SelectedAssignment == "Fault")
                //{
                //    ((Program)SelectedProgram).FaultRoutineName = Name;
                //}

                if (_selectedType == RoutineType.RLL)
                {
                    RLLRoutine rllRoutine = new RLLRoutine(_program.ParentController)
                    {
                        Name = Name,
                        Description = Description,
                        IsMainRoutine = SelectedAssignment == "Main" || SelectedAssignment == "Prestate",
                        IsFaultRoutine = SelectedAssignment == "Fault",
                    };
                    ((Program) SelectedProgram).AddRoutine(rllRoutine);
                }

                if (_selectedType == RoutineType.SFC)
                {
                    SFCRoutine sfcRoutine = new SFCRoutine(_program.ParentController)
                    {
                        Name = Name,
                        Description = Description,
                        IsMainRoutine = SelectedAssignment == "Main" || SelectedAssignment == "Prestate",
                        IsFaultRoutine = SelectedAssignment == "Fault",
                    };
                    ((Program) SelectedProgram).AddRoutine(sfcRoutine);
                }

                if (_selectedType == RoutineType.FBD)
                {
                    FBDRoutine fbdRoutine = new FBDRoutine(_program.ParentController, null)
                    {
                        Name = Name,
                        Description = Description,
                        IsMainRoutine = SelectedAssignment == "Main" || SelectedAssignment == "Prestate",
                        IsFaultRoutine = SelectedAssignment == "Fault"
                    };

                    ((Program) SelectedProgram).AddRoutine(fbdRoutine);
                }

                if (_selectedType == RoutineType.ST)
                {
                    STRoutine stRoutine = new STRoutine(_program.ParentController)
                    {
                        Name = Name,
                        Description = Description,
                        IsMainRoutine = SelectedAssignment == "Main" || SelectedAssignment == "Prestate",
                        IsFaultRoutine = SelectedAssignment == "Fault",
                        CodeText = new List<string>()
                    };
                    ((Program) SelectedProgram).AddRoutine(stRoutine);
                }

                GlobalSetting.GetInstance().RoutineSetting.CurrentRoutineType = _selectedType;
                DialogResult = true;
            }
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

        public string Name { set; get; }
        public string Description { set; get; }
        public IList TypeList { set; get; }
        public IList ProgramList { set; get; }


        public IProgramModule SelectedProgram
        {
            set
            {
                _selectedProgram = value;
                SetAssignment(value);
            }
            get { return _selectedProgram; }
        }

        private bool IsValidTagName(string name)
        {
            string warningMessage = LanguageManager.GetInstance().ConvertSpecifier("Failed to create the routine...") 
                                    + $"{name}"
                                    + LanguageManager.GetInstance().ConvertSpecifier("New Routine.'.");
            string warningReason = string.Empty;
            bool isValid = true;

            if (string.IsNullOrEmpty(name))
            {
                isValid = false;
                warningReason = LanguageManager.GetInstance().ConvertSpecifier("The name is empty.");
            }

            if (isValid)
            {
                if (name.Length > 40 || name.EndsWith("_") ||
                    name.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    isValid = false;
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("The name is invalid.");
                }
            }

            if (isValid)
            {
                Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
                if (!regex.IsMatch(name))
                {
                    isValid = false;
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("The name is invalid.");
                }
            }

            // key word
            if (isValid)
            {
                string[] keyWords =
                {
                    "goto",
                    "repeat", "until", "or", "end_repeat",
                    "return", "exit",
                    "if", "then", "elsif", "else", "end_if",
                    "case", "of", "end_case",
                    "for", "to", "by", "do", "end_for",
                    "while", "end_while",
                    "not", "mod", "and", "xor", "or",
                    "ABS","SQRT",
                    "LOG","LN",
                    "DEG","RAD","TRN",
                    "ACS","ASN","ATN","COS","SIN","TAN"
                };
                foreach (var keyWord in keyWords)
                {
                    if (keyWord.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        isValid = false;
                        warningReason = LanguageManager.GetInstance().ConvertSpecifier("The name is invalid.");
                    }
                }
            }

            if (isValid)
            {
                if (_program.Type == ProgramType.Phase)
                {
                    if (name == "Aborting" || name == "Resetting" || name == "Restarting" || name == "Running" ||
                        name == "Stopping")
                    {
                        isValid = false;
                        warningReason = LanguageManager.GetInstance().ConvertSpecifier("Name can not be the same...");
                    }
                }
            }

            if (isValid)
            {
                foreach (var routine in _routineCollection)
                {
                    if (routine != null && routine.Name.Equals(name,StringComparison.OrdinalIgnoreCase))
                    {
                        isValid = false;
                        warningReason = LanguageManager.GetInstance().ConvertSpecifier("Already exists.");
                    }
                }

            }

            if (isValid)
            {
                if (SelectedType == RoutineType.SFC || SelectedType == RoutineType.FBD)
                {
                    isValid = false;
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("This type of routine...");
                }

            }

            if (!isValid)
            {
                var warningDialog = new WarningDialog(warningMessage, warningReason)
                    {Owner = Application.Current.MainWindow};
                warningDialog.ShowDialog();
            }

            return isValid;
        }
    }
}
