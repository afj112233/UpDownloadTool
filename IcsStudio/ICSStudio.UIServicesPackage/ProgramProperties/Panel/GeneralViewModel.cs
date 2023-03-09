using System;
using System.Collections.Specialized;
using System.ComponentModel;
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

namespace ICSStudio.UIServicesPackage.ProgramProperties.Panel
{
    class GeneralViewModel<T> : ViewModelBase, IOptionPanel, ICanBeDirty where T : FrameworkElement
    {
        private readonly Controller _controller;
        private string _name;
        private string _description;
        private bool _isDirty;
        private Program _program;
        private IProgram _parentProgram;
        private int _major;
        private int _minor;
        private string _extendedText;
        private string _revisionNote;
        private string _imageSource;
        private string _row4Name;

        public GeneralViewModel(T panel, IProgram program)
        {
            Control = panel;

            panel.DataContext = this;
            _program = program as Program;
            _controller = _program?.ParentController as Controller ?? Controller.GetInstance();
            Name = program.Name;
            Description = program.Description;
            TaskName = "<none>";
            Parent = "<none>";
            UseAsFolder = _program.ProgramProperties.UseAsFolder;
            IsEnabled = UseAsFolder;

            if (program.ParentTask != null)
            {
                TaskName = program.ParentTask.Name;
                program.ParentTask.PropertyChanged += OnPropertyChanged;

                switch (program.ParentTask.Type)
                {
                    case TaskType.Event:
                        _imageSource = @"..\Panel\Images\Task_Event.png";
                        break;
                    case TaskType.Periodic:
                        _imageSource = @"..\Panel\Images\Task_Periodic.png";
                        break;
                    case TaskType.Continuous:
                        _imageSource = @"..\Panel\Images\Task_Continuous.png";
                        break;
                    case TaskType.NullType:
                        _imageSource = @"..\Panel\Images\Star.png";
                        break;
                    default:
                        _imageSource = @"..\Panel\Images\Star.png";
                        break;
                }
            }

            Row3Visibility = Visibility.Visible;
            if (program.Type == ProgramType.Phase)
            {
                Row3Visibility = Visibility.Collapsed;
                Row4Name = $"{LanguageManager.GetInstance().ConvertSpecifier("Schedule")}:";
            }
            else if (program.Type == ProgramType.Sequence)
            {
                Major = int.Parse(_program.ProgramProperties.Revision.Split('.')[0]);
                Minor = int.Parse(_program.ProgramProperties.Revision.Split('.')[1]);
                RevisionNote = _program.ProgramProperties.RevisionNote;
                ExtendedText = _program.ProgramProperties.RevisionExtension;
                Row4Name = $"{LanguageManager.GetInstance().ConvertSpecifier("Schedule")}:";
            }
            else
            {
                Row4Name = $"{LanguageManager.GetInstance().ConvertSpecifier("Schedule")}:";
            }

            if (_program.ParentCollection.ParentController.PowerLossProgram == program.Name ||
                program.Name == _program.ParentCollection.ParentController.MajorFaultProgram)
            {
                Row3Visibility = Visibility.Collapsed;
                VisibilityRow34 = Visibility.Collapsed;
            }

            foreach (var item in program.ParentController.Programs)
            {
                if (item.ChildCollection[program.Name] != null)
                {
                    _parentProgram = item;
                    item.PropertyChanged += OnPropertyChanged;
                    Parent = item.Name;
                }
            }

            program.PropertyChanged += OnPropertyChanged;
            program.ParentCollection.CollectionChanged += OnCollectionChanged;

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                _controller, "IsOnlineChanged", OnIsOnlineChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            Row4Name = $"{LanguageManager.GetInstance().ConvertSpecifier("Schedule")}:";
        }

        public override void Cleanup()
        {
            _program.PropertyChanged -= OnPropertyChanged;
            _program.ParentCollection.CollectionChanged -= OnCollectionChanged;

            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                _controller, "IsOnlineChanged", OnIsOnlineChanged);
        }

        public void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_program.ParentTask == null)
            {
                TaskName = "<none>";
                RaisePropertyChanged("TaskName");
                return;
            }

            TaskName = _program.ParentTask.Name;
            RaisePropertyChanged("TaskName");
        }

        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_program == sender)
            {
                if (e.PropertyName == "ParentTask")
                {
                    if (_program.ParentTask == null)
                    {
                        TaskName = "<none>";
                        RaisePropertyChanged("TaskName");
                        return;
                    }

                    TaskName = _program.ParentTask.Name;
                    RaisePropertyChanged("TaskName");
                }
            }

            if (_program.ParentTask == sender)
            {
                if (e.PropertyName == "Name")
                {
                    TaskName = _program.ParentTask.Name;
                    RaisePropertyChanged("TaskName");
                }
            }

            if (_parentProgram == sender)
            {
                if (e.PropertyName == "Name")
                {
                    Parent = _parentProgram.Name;
                    RaisePropertyChanged("Parent");
                }
            }
        }

        public string ImageSource
        {
            get { return _imageSource; }
            set { Set(ref _imageSource, value); }
        }

        public Visibility Row3Visibility { get; }
        public Visibility VisibilityRow34 { get; } = Visibility.Visible;
        public string TaskName { set; get; }
        public string Row4Name
        {
            set
            {
                Set(ref _row4Name, value);
            }
            get
            {
                return _row4Name;
            }
        }
        public bool IsEnabled { set; get; }
        private bool _useAsFolder;


        public bool UseAsFolder
        {
            set
            {
                _useAsFolder = value;

                Compare();
            }
            get { return _useAsFolder; }
        }

        public int Major
        {
            set
            {
                _major = value;
                Compare();
            }
            get { return _major; }
        }

        public int Minor
        {
            set
            {
                _minor = value;
                Compare();
            }
            get { return _minor; }
        }

        public string ExtendedText
        {
            set
            {
                _extendedText = value;
                Compare();
            }
            get { return _extendedText; }
        }

        public string RevisionNote
        {
            set
            {
                _revisionNote = value;
                Compare();
            }
            get { return _revisionNote; }
        }

        public bool IsOnlineEnabled => !_controller.IsOnline;

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

        public string Parent { get; set; }

        public bool CheckInvalid()
        {
            if (!IsValidTaskName(Name)) return false;
            return true;
        }

        public void Compare()
        {
            IsDirty = false;
            if (Name != _program.Name) IsDirty = true;
            if (!(Description ?? "").Equals(_program.Description ?? "")) IsDirty = true;
            if (UseAsFolder != _program.ProgramProperties.UseAsFolder) IsDirty = true;
            if (_program.Type == ProgramType.Sequence)
            {
                if (Major.ToString() != _program.ProgramProperties.Revision.Split('.')[0]) IsDirty = true;
                if (Minor.ToString() != _program.ProgramProperties.Revision.Split('.')[1]) IsDirty = true;
                if (!(ExtendedText ?? "").Equals(_program.ProgramProperties.RevisionExtension ?? "")) IsDirty = true;
                if (!(RevisionNote ?? "").Equals(_program.ProgramProperties.RevisionNote ?? "")) IsDirty = true;
            }

        }

        public void Save()
        {
            _program.Name = Name;
            _program.Description = Description;
            _program.ProgramProperties.UseAsFolder = UseAsFolder;
            if (!UseAsFolder)
            {
                IsEnabled = false;
                RaisePropertyChanged("IsEnabled");
            }

            if (_program.Type == ProgramType.Sequence)
            {
                _program.ProgramProperties.Revision = $"{Major}.{Minor}";
                _program.ProgramProperties.RevisionExtension = ExtendedText;
                _program.ProgramProperties.RevisionNote = RevisionNote;
            }
        }

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

        private bool IsValidTaskName(string name)
        {
            string warningMessage = LanguageManager.GetInstance().ConvertSpecifier("Failed to save a new program.");
            string warningReason = string.Empty;
            bool isValid = true;

            if (string.IsNullOrEmpty(name))
            {
                isValid = false;
                warningReason = LanguageManager.GetInstance().ConvertSpecifier("program name is empty.");
            }

            if (isValid)
            {
                if (name.Length > 40 || name.EndsWith("_") ||
                    name.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    isValid = false;
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("program name invalid.");
                }
            }

            if (isValid)
            {
                Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
                if (!regex.IsMatch(name))
                {
                    isValid = false;
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("program name invalid.");
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
                        warningReason = LanguageManager.GetInstance().ConvertSpecifier("program name invalid.");
                    }
                }
            }

            if (isValid)
            {
                var program = this._program.ParentController.Programs[name];
                if (program != null && program != this._program)
                {
                    isValid = false;
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("Already exists.");
                }
            }


            //
            if (!isValid)
            {
                var warningDialog = new WarningDialog(warningMessage, warningReason)
                { Owner = Application.Current.MainWindow };
                warningDialog.ShowDialog();
            }

            return isValid;
        }

        public event EventHandler IsDirtyChanged;

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
