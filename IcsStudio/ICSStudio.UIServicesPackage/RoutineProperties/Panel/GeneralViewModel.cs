using System;
using System.Text.RegularExpressions;
using System.Windows;
using GalaSoft.MvvmLight;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.RoutineProperties.Panel
{
    class GeneralViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private readonly Controller _controller;
        private IRoutine _routine;
        private bool _isDirty;
        private string _name;
        private string _description;
        private IProgram _program;
        private string _row4Name;
        private string _row5Name;
        private string _type;

        public GeneralViewModel(General panel, IRoutine routine)
        {
            Control = panel;
            panel.DataContext = this;
            _routine = routine;
            _controller = _routine.ParentController as Controller ??Controller.GetInstance();
            Name = routine.Name;
            Description = routine.Description;
            if (routine.Name == "Aborting" || routine.Name == "Resetting" ||
                routine.Name == "Restarting" || routine.Name == "Running" ||
                routine.Name == "Stopping" || routine.Type == RoutineType.Sequence)
            {
                Visible1 = Visibility.Collapsed;
                Visible2 = Visibility.Visible;
            }
            else
            {
                Visible1 = Visibility.Visible;
                Visible2 = Visibility.Collapsed;
            }

            switch (_routine.Type)
            {
                case RoutineType.RLL:
                    RLLRoutine rllRoutine = _routine as RLLRoutine;
                    if (rllRoutine != null)
                    {
                        NumberOfRungs = rllRoutine.CodeText.Count.ToString();
                    }

                    break;
                case RoutineType.ST:
                    STRoutine stRoutine = _routine as STRoutine;
                    if (stRoutine != null)
                    {
                        NumberOfRungs = stRoutine.CodeText.Count.ToString();
                    }

                    break;
                //TODO(tlm): add code here
            }
            

            IsEnable = true;
            NameIsEnabled = true;
            if (routine.ParentCollection.ParentProgram is AoiDefinition)
            {
                InProgram = routine.ParentCollection.ParentProgram.Name;
                _program = null;
                IsEnable = !((AoiDefinition)routine.ParentCollection.ParentProgram).IsSealed;
                NameIsEnabled = false;
                Visible1 = Visibility.Collapsed;
                Visible2 = Visibility.Visible;
            }
            else
            {
                foreach (var program in _routine.ParentController.Programs)
                {
                    foreach (var routine2 in program.Routines)
                    {
                        if (routine2 == routine)
                        {
                            InProgram = program.Name;
                            _program = program;
                        }
                    }
                }
            }

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                _controller, "IsOnlineChanged", OnIsOnlineChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(Type));
            RaisePropertyChanged(nameof(Row4Name));
            RaisePropertyChanged(nameof(Row5Name));
        }

        public bool IsOnlineEnabled => !_controller.IsOnline;
        public bool IsEnable { set; get; }
        public bool NameIsEnabled { get; set; }

        public void Compare()
        {
            IsDirty = false;
            if (Name != _routine.Name) IsDirty = true;
            if (Description != _routine.Description) IsDirty = true;
        }

        public void Save()
        {
            _routine.Name = Name;
            _routine.Description = Description;
        }

        public string Name
        {
            set
            {
                _name = value;
                Compare();
            }
            get { return _name; }
        }

        public string Description
        {
            set
            {
                _description = value;
                Compare();
            }
            get { return _description; }
        }

        public Visibility Visible1 { set; get; }
        public Visibility Visible2 { set; get; }

        public string Row4Name
        {
            set
            {
                Set(ref _row4Name, value);
            }
            get
            {
                if (_routine.ParentCollection.ParentProgram is AoiDefinition)
                {
                    return LanguageManager.GetInstance().ConvertSpecifier("In instruction:");
                }
                else
                {
                    return LanguageManager.GetInstance().ConvertSpecifier("In Program:");
                }
            }
        }

        public string Row5Name
        {
            set
            {
                Set(ref _row5Name, value);
            }
            get
            {
                switch (_routine.Type)
                {
                    case RoutineType.RLL:
                        return LanguageManager.GetInstance().ConvertSpecifier("Number of Rungs:");
                    case RoutineType.SFC:
                        return LanguageManager.GetInstance().ConvertSpecifier("Number of Steps:");
                    case RoutineType.FBD:
                        return LanguageManager.GetInstance().ConvertSpecifier("Number of Sheets:");
                    case RoutineType.ST:
                        return LanguageManager.GetInstance().ConvertSpecifier("Number of Lines:");
                    case RoutineType.Sequence:
                        return LanguageManager.GetInstance().ConvertSpecifier("Number of Steps:");
                    default:
                        return string.Empty;
                }
            }
        }

        public string Type
        {
            set
            {
                Set(ref _type, value);
            }
            get
            {
                switch (_routine.Type)
                {
                    case RoutineType.RLL:
                        return LanguageManager.GetInstance().ConvertSpecifier("Ladder Diagram");
                    case RoutineType.SFC:
                        return LanguageManager.GetInstance().ConvertSpecifier("Sequential Function Chart");
                    case RoutineType.FBD:
                        return LanguageManager.GetInstance().ConvertSpecifier("Function Block Diagram");
                    case RoutineType.ST:
                        return LanguageManager.GetInstance().ConvertSpecifier("Structured Text");
                    case RoutineType.Sequence:
                        return LanguageManager.GetInstance().ConvertSpecifier("Equipment Sequence Diagram");
                    default:
                        return string.Empty;
                }
            }
        }

        public string InProgram { set; get; }
        public string NumberOfRungs { set; get; }
        public object Owner { get; set; }
        public object Control { get; }

        public void LoadOptions()
        {

        }

        public bool SaveOptions()
        {
            return true;
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    IsDirtyChanged?.Invoke(this, EventArgs.Empty);
                }

                Set(ref _isDirty, value);
            }
        }

        public event EventHandler IsDirtyChanged;

        public bool IsValidRoutineName(bool showTip)
        {
            var name = Name;
            string warningMessage = LanguageManager.GetInstance().ConvertSpecifier("Failed to modify the properties for the routine '");
            warningMessage += $"{_routine.Name}";
            warningMessage += LanguageManager.GetInstance().ConvertSpecifier("'.");
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
                if (_program?.Type == ProgramType.Phase)
                {
                    if (_routine.Name != "Aborting" && _routine.Name != "Resetting" && _routine.Name != "Restarting" &&
                        _routine.Name != "Running" &&
                        _routine.Name != "Stopping")
                    {
                        if (name == "Aborting" || name == "Resetting" || name == "Restarting" || name == "Running" ||
                            name == "Stopping")
                        {
                            isValid = false;
                            warningReason = LanguageManager.GetInstance().ConvertSpecifier("Name can not be the same as an Equipment Phase State Routine name.");
                        }
                    }
                }
            }

            if (isValid)
            {
                foreach (var routine in _routine.ParentCollection)
                {
                    if (name == routine.Name && routine != _routine)
                    {
                        isValid = false;
                        warningReason = LanguageManager.GetInstance().ConvertSpecifier("Already exists.");
                    }
                }

            }

            if (!isValid && showTip)
            {
                var warningDialog = new WarningDialog(warningMessage, warningReason)
                { Owner = Application.Current.MainWindow };
                warningDialog.ShowDialog();
            }

            return isValid;
        }

        public override void Cleanup()
        {
            base.Cleanup();
            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                _controller, "IsOnlineChanged", OnIsOnlineChanged);
        }

        public virtual void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                RaisePropertyChanged(nameof(IsOnlineEnabled));
            });
        }
    }
}
