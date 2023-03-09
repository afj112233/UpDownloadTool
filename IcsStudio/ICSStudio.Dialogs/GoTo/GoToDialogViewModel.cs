using System;
using System.Collections.Generic;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;

namespace ICSStudio.Dialogs.GoTo
{
    public class GoToDialogViewModel : ViewModelBase
    {
        private readonly ITag _tag;
        private bool? _dialogResult;
        private string _selectedKind;
        private IRoutine _routine;
        private int _lineCount;
        private Visibility _callVisibility;
        private Visibility _unEnabledTextBoxVisibility;
        private Visibility _lineVisibility;
        private readonly string _tagName;
        private string _context;
        //private IProgramModule _program;

        public GoToDialogViewModel(int lineNumber, int lineCount, IRoutine routine, string tagName, ITag tag,
            IProgramModule program)
        {
            _tag = tag;
            SelectedKind = "Line";
            _routine = routine;
            //_program = program;
            _lineCount = lineCount;
            LineNumber = lineNumber.ToString();
            _tagName = tagName;
            GoToCommand = new RelayCommand(ExecuteGoToCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            if (program != null)
            {
                KindList = new List<string>()
                {
                    "Cross Reference", "Edit", "Line", "Properties"
                };
            }
            else if (routine != null)
            {
                KindList = new List<string>()
                    {"Called Routines", "Calling Routines", "Cross Reference", "Edit", "Line", "Properties"};
                Name = routine.Name;
            }
            else if (tag != null)
            {
                KindList = new List<string>() {"Cross Reference", "Edit", "Line", "Monitor", "Properties"};
                Name = tagName;
            }
            else
            {
                KindList = new List<string>() {"Line", "New"};
                Name = tagName;
            }
        }

        public string Name { set; get; }

        public string LineNumber { set; get; }

        #region Command

        public RelayCommand GoToCommand { set; get; }

        private void ExecuteGoToCommand()
        {
            if (SelectedKind.Equals("Line"))
            {
                bool isInvalid = true;
                if (LineNumber.Equals("end", StringComparison.OrdinalIgnoreCase))
                {
                    DialogResult = true;
                    return;
                }

                foreach (var c in LineNumber)
                {
                    if (!char.IsNumber(c))
                    {
                        isInvalid = false;
                    }
                }

                if (!(isInvalid && int.Parse(LineNumber) > 0 && int.Parse(LineNumber) < _lineCount))
                {
                    isInvalid = false;
                }

                if (!isInvalid)
                {
                    MessageBox.Show(
                        $"Invalid Line Number.Please enter a number from 1 to {_lineCount - 1} or the word 'End'.",
                        "ICS Studio", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
                    return;
                }

                DialogResult = true;
            }
            else
            {
                DialogResult = true;
            }
        }

        public RelayCommand CancelCommand { set; get; }

        private void ExecuteCancelCommand()
        {
            DialogResult = false;
        }

        #endregion

        public Visibility CallVisibility
        {
            set { Set(ref _callVisibility, value); }
            get { return _callVisibility; }
        }

        public Visibility UnEnabledTextBoxVisibility
        {
            set { Set(ref _unEnabledTextBoxVisibility, value); }
            get { return _unEnabledTextBoxVisibility; }
        }

        public Visibility LineVisibility
        {
            set { Set(ref _lineVisibility, value); }
            get { return _lineVisibility; }
        }

        public string Context
        {
            set { Set(ref _context, value); }
            get { return LanguageManager.GetInstance().ConvertSpecifier(_context); }
        }

        public string SelectedKind
        {
            set
            {
                _selectedKind = value;
                ChooseText();
            }
            get { return _selectedKind; }
        }

        public bool? DialogResult
        {
            set { Set(ref _dialogResult, value); }
            get { return _dialogResult; }
        }

        public List<string> KindList { get; }

        private void ChooseText()
        {
            CallVisibility = Visibility.Collapsed;
            UnEnabledTextBoxVisibility = Visibility.Collapsed;
            LineVisibility = Visibility.Collapsed;
            if (_routine != null)
            {
                Name = _routine.Name;
            }
            else
            {
                Name = _tagName;
            }

            RaisePropertyChanged("Name");
            if (SelectedKind.Equals("Called Routines") || SelectedKind.Equals("Calling Routines"))
            {
                Context = "Edit routine:";
                CallVisibility = Visibility.Visible;
            }
            else if (SelectedKind.Equals("Cross Reference"))
            {
                Context = "Show cross reference for:";
                UnEnabledTextBoxVisibility = Visibility.Visible;
            }
            else if (SelectedKind.Equals("Edit"))
            {
                Context = "Edit routine:";
                UnEnabledTextBoxVisibility = Visibility.Visible;
            }
            else if (SelectedKind.Equals("Properties"))
            {
                Context = "Edit properties of:";
                if (_tag != null)
                {
                    Name = _tag.Name;
                }
                else
                {
                    Name = _routine.Name;
                }

                RaisePropertyChanged("Name");
                UnEnabledTextBoxVisibility = Visibility.Visible;
            }
            else if (SelectedKind.Equals("Monitor"))
            {
                Context = "Monitor tag:";
                UnEnabledTextBoxVisibility = Visibility.Visible;
            }
            else if (SelectedKind.Equals("New"))
            {
                Context = "new tag:";
                UnEnabledTextBoxVisibility = Visibility.Visible;
            }
            else if (SelectedKind.Equals("Line"))
            {
                Context = "Place the caret on line:";
                LineVisibility = Visibility.Visible;
            }
        }
    }
}
