using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.UIServicesPackage.ProgramProperties;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    class NewProgramDialogViewModel : ViewModelBase
    {
        private IProgramCollection _iProgramCollection;
        private readonly IController _controller;
        private string _string;
        private bool? _dialogResult;

        public NewProgramDialogViewModel(ProgramType type, ITask task)
        {
            IProjectInfoService projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            _controller = projectInfoService?.Controller;
            TasksList = new List<string>();
            ParentList = new List<string>();

            _iProgramCollection = _controller.Programs;

            ITaskCollection iTaskCollection = _controller?.Tasks;
            CancelCommand = new RelayCommand(CancelCommandClick, false);
            foreach (var item in iTaskCollection)
            {
                TasksList.Add(item.Name);
            }

            TasksList.Add("Power-Up Handler");
            TasksList.Add("Controller Fault Handler");
            SelectedTask = string.IsNullOrEmpty(task?.Name) ? "<none>" : task?.Name;

            if (type == ProgramType.Normal)
            {
                OKCommand = new RelayCommand(OKCommandClick);
                Title = "New Program";
                Type1 = Visibility.Visible;
                Type2 = Visibility.Collapsed;
                Type3 = Visibility.Collapsed;
            }

            if (type == ProgramType.Phase)
            {
                OKCommand = new RelayCommand(OKCommandClick);
                Title = "New Equipment Phase";
                Type2 = Visibility.Visible;
                Type1 = Visibility.Collapsed;
                Type3 = Visibility.Collapsed;
            }

            if (type == ProgramType.Sequence)
            {
                OKCommand = new RelayCommand(OKCommandClick, CanOKCommand);
                Title = "New Equipment Sequence";
                Type3 = Visibility.Visible;
                Type2 = Visibility.Collapsed;
                Type1 = Visibility.Collapsed;
            }

            TasksList.Insert(0, "<none>");


            ParentList.Insert(0, "<none>");
            foreach (var program in _iProgramCollection)
            {
                ParentList.Add(program.Name);
            }

            SelectParent = "<none>";

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(Title));
        }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        public bool Row34Enabled { set; get; } = true;

        public Visibility Type1 { set; get; }
        public Visibility Type2 { set; get; }
        public Visibility Type3 { set; get; }
        public List<string> TasksList { set; get; }

        public string SelectedTask
        {
            set
            {
                _selectedTask = value;
                if (_selectedTask == "Controller Fault Handler" || _selectedTask == "Power-Up Handler")
                {
                    Row34Enabled = false;
                    SelectParent = "<none>";
                    RaisePropertyChanged("SelectParent");
                }
                else
                {
                    Row34Enabled = true;
                }

                RaisePropertyChanged("Row34Enabled");
            }
            get { return _selectedTask; }
        }

        public List<string> ParentList { set; get; }
        public string SelectParent { set; get; }

        public string Title
        {
            get { return LanguageManager.GetInstance().ConvertSpecifier(_title); }
            set { Set(ref _title, value); }
        }

        private string _title;
        private bool _useAsFloder;
        private string _selectedTask;
        public bool IsEnable { set; get; } = true;

        public bool UseAsFloder
        {
            set
            {
                _useAsFloder = value;
                if (value)
                {
                    IsEnable = false;
                    SelectedTask = "<none>";
                    RaisePropertyChanged("IsEnable");
                    RaisePropertyChanged("SelectedTask");
                }
                else
                {
                    IsEnable = true;
                    RaisePropertyChanged("IsEnable");
                }
            }
            get { return _useAsFloder; }
        }

        public bool Inhibit { set; get; }
        public bool IsOpen { set; get; }
        public RelayCommand OKCommand { set; get; }
        public RelayCommand CancelCommand { set; get; }

        public bool CanOKCommand()
        {

            bool isValid = !string.IsNullOrEmpty(Name);

            if (isValid)
            {
                Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
                if (!regex.IsMatch(Name))
                {
                    isValid = false;
                }
            }

            if (isValid)
            {
                var program = _iProgramCollection[Name];
                if (program != null)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public string Name
        {
            set
            {
                _string = value;
                OKCommand.RaiseCanExecuteChanged();
            }
            get { return _string; }
        }

        public int Major { set; get; }
        public int Minor { set; get; }
        public string ExtendedText { set; get; }
        public string RevisionNote { set; get; }

        public string Description { set; get; }

        public void OKCommandClick()
        {
            if (IsValidTagName(Name))
            {
                ITask task = null;
                SimpleServices.Common.ProgramProperties programProperties =
                    new SimpleServices.Common.ProgramProperties();
                programProperties.UseAsFolder = UseAsFloder;
                if (SelectedTask == "Power-Up Handler")
                {
                    ((Controller)_iProgramCollection.ParentController).PowerLossProgram = Name;
                }
                else if (SelectedTask == "Controller Fault Handler")
                {
                    ((Controller)_iProgramCollection.ParentController).MajorFaultProgram = Name;
                }
                else if (SelectedTask != "<none>")
                {
                    task = _iProgramCollection.ParentController.Tasks[SelectedTask];
                }

                ProgramType programType = ProgramType.Normal;

                Program program = new Program(_iProgramCollection.ParentController)
                {
                    Name = Name,
                    Description = Description,
                    ParentTask = task,
                    Inhibited = Inhibit,
                    Type = programType,
                    ProgramProperties = programProperties,
                };

                if (Type2 == Visibility.Visible)
                {
                    program.Type = ProgramType.Phase;
                }

                if (Type3 == Visibility.Visible)
                {
                    program.Type = ProgramType.Sequence;
                    programProperties.Revision = $"{Major}.{Minor}";
                    programProperties.RevisionExtension = ExtendedText;
                    programProperties.RevisionNote = RevisionNote;
                    SequenceRoutine routine = new SequenceRoutine(_controller)
                    {
                        Name = "Diagram",
                        Description = Description,
                        IsMainRoutine = true,
                        IsFaultRoutine = false,
                    };
                    program.AddRoutine(routine);
                    //program.MainRoutineName = "Diagram";
                }

                if (SelectParent != "<none>")
                {
                    (_iProgramCollection.ParentController.Programs[SelectParent].ChildCollection as ProgramCollection)
                        .AddProgram(program);
                }

                (_iProgramCollection.ParentController.Programs as ProgramCollection).AddProgram(program);


                DialogResult = true;
                if (IsOpen)
                {
                    OpenProperties(program);
                }
            }
        }

        public void CancelCommandClick()
        {
            DialogResult = false;
        }

        public void OpenProperties(IProgram program)
        {
            var viewModel = new ProgramPropertiesViewModel(program,false);
            TabbedOptionsDialog dialog = new TabbedOptionsDialog(viewModel);
            viewModel.CloseAction = dialog.Close;
            dialog.Width = 615;
            dialog.Height = 430;
            if (program.Type == ProgramType.Phase)
            {
                dialog.Height = 510;
                dialog.Width = 550;
            }
            else if (program.Type == ProgramType.Sequence)
            {
                dialog.Height = 470;
                dialog.Width = 530;
            }

            dialog.Owner = Application.Current.MainWindow;
            dialog.Show();
        }

        private bool IsValidTagName(string name)
        {
            string warningMessage = LanguageManager.GetInstance().ConvertSpecifier("Failed to create program.");
            string warningReason = string.Empty;
            bool isValid = true;

            if (string.IsNullOrEmpty(name))
            {
                isValid = false;
                warningReason = LanguageManager.GetInstance().ConvertSpecifier("Invalid name.");
            }

            if (isValid)
            {
                if (name.Length > 40 || name.EndsWith("_") ||
                    name.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    isValid = false;
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("Invalid name.");
                }
            }

            if (isValid)
            {
                Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
                if (!regex.IsMatch(name))
                {
                    isValid = false;
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("Invalid name.");
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
                        warningReason = LanguageManager.GetInstance().ConvertSpecifier("Invalid name.");
                    }
                }
            }

            if (isValid)
            {
                var program = _iProgramCollection[name];
                if (program != null)
                {
                    isValid = false;
                    warningMessage = LanguageManager.GetInstance().ConvertSpecifier("Failed to create program'")
                                     + $"{name}" 
                                     + LanguageManager.GetInstance().ConvertSpecifier("New Program.'.");

                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("Already exists.");
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

        public override void Cleanup()
        {
            base.Cleanup();

            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }
    }
}
