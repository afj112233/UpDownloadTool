using System;
using System.Text.RegularExpressions;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.ViewModel;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.MultiLanguage;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.AddOnInstruction.Panel
{
    class GeneralViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        //private SimpleServices.DataType.AddOnInstruction _addOnInstruction;
        private bool _isDirty;
        private readonly IController _controller;
        private string _name;
        private string _description;
        private string _type;
        private int _major;
        private int _minor;
        private string _extendedText;
        private string _revisionNote;
        private string _vendor;
        private AoiDefinition _aoiDefinition;
        private bool _onlineEnable;
        private bool _isReadOnlyNote;

        public GeneralViewModel(General panel, IAoiDefinition aoiDefinition)
        {
            Control = panel;
            panel.DataContext = this;
            _aoiDefinition = (AoiDefinition) aoiDefinition;
            ChangeTypeCommand = new RelayCommand(ExecuteChangeTypeCommand);
            _controller = aoiDefinition.Routines["Logic"].ParentController;
            Name = aoiDefinition.Name;
            Description = aoiDefinition.Description;
            Type = ChangeTypeToName(aoiDefinition.Routines["Logic"].Type);
            Major = int.Parse(_aoiDefinition.Revision.Split('.')[0] == ""
                ? "1"
                : _aoiDefinition.Revision.Split('.')[0]);
            Minor = int.Parse(_aoiDefinition.Revision.Split('.')[1] == ""
                ? "0"
                : _aoiDefinition.Revision.Split('.')[1]);
            ExtendedText = _aoiDefinition.ExtendedText;
            RevisionNote = _aoiDefinition.RevisionNote;
            Vendor = _aoiDefinition.Vendor;
            SetControlEnable();

            Controller controller = _aoiDefinition.ParentController as Controller;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }
            
        }

        public bool IsReadOnlyNote
        {
            set { Set(ref _isReadOnlyNote , value); }
            get { return _isReadOnlyNote; }
        }

        public override void Cleanup()
        {
            Controller controller = (Controller) _aoiDefinition.ParentController;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                SetControlEnable();

                IsDirty = false;
            });
        }

        public void SetControlEnable()
        {
            if (_aoiDefinition.ParentController.IsOnline || _aoiDefinition.IsSealed)
            {
                OnlineEnable = false;
            }
            else
            {
                OnlineEnable = true;
            }

            IsReadOnlyNote = _aoiDefinition.IsSealed;
        }

        public bool OnlineEnable
        {
            set { Set(ref _onlineEnable, value); }
            get { return _onlineEnable; }
        }

        public void Compare()
        {
            IsDirty = false;
            string revision = $"{Major}.{Minor}";
            if (Name != _aoiDefinition.Name) IsDirty = true;
            else if (Description != _aoiDefinition.Description) IsDirty = true;
            else if (revision != _aoiDefinition.Revision) IsDirty = true;
            else if (ExtendedText != _aoiDefinition.ExtendedText) IsDirty = true;
            else if (RevisionNote != _aoiDefinition.RevisionNote) IsDirty = true;
            else if (Vendor != _aoiDefinition.Vendor) IsDirty = true;
        }

        public void Save()
        {
            string name = "";

            if (Major > 65535 || Major < 0)
            {
                name = "major";
            }

            if (Minor > 65535 || Minor < 0)
            {
                name = "minor";
            }

            if (name != "")
            {
                string warningReason = LanguageManager.GetInstance().ConvertSpecifier("Value must be within 0 and 65535.");
                string warningMessage = LanguageManager.GetInstance().ConvertSpecifier("Invalid") + $" {name} " +
                                        LanguageManager.GetInstance().ConvertSpecifier("Revision Value.");
                var warningDialog = new WarningDialog(warningMessage, warningReason)
                    {Owner = Application.Current.MainWindow};
                warningDialog.ShowDialog();
            }

            _aoiDefinition.Name = Name;
            _aoiDefinition.Description = Description;
            string revision = $"{Major}.{Minor}";
            _aoiDefinition.Revision = revision;
            _aoiDefinition.ExtendedText = ExtendedText;
            _aoiDefinition.RevisionNote = RevisionNote;
            _aoiDefinition.Vendor = Vendor;
            IsDirty = false;
        }

        public string Name
        {
            set
            {
                Set(ref _name, value);
                Compare();
            }
            get { return _name; }
        }

        public string Description
        {
            set
            {
                Set(ref _description, value);
                Compare();
            }
            get { return _description; }
        }

        public string Type
        {
            set
            {
                Set(ref _type, value);
                Compare();
            }
            get { return _type; }
        }

        public int Major
        {
            set
            {
                Set(ref _major, value);
                Compare();
            }
            get { return _major; }
        }

        public int Minor
        {
            set
            {
                Set(ref _minor, value);
                Compare();
            }
            get { return _minor; }
        }

        public string ExtendedText
        {
            set
            {
                Set(ref _extendedText, value);
                Compare();
            }
            get { return _extendedText; }
        }

        public string RevisionNote
        {
            set
            {
                Set(ref _revisionNote, value);
                Compare();
            }
            get { return _revisionNote; }
        }

        public string Vendor
        {
            set
            {
                Set(ref _vendor, value);
                Compare();
            }
            get { return _vendor; }
        }

        public RelayCommand ChangeTypeCommand { set; get; }

        public void ExecuteChangeTypeCommand()
        {
            var viewModel = new ChangeTypeViewModel(_aoiDefinition.Routines["Logic"].Type,_aoiDefinition);
            var dialog = new View.ChangeType();
            dialog.Width = 400;
            dialog.Height = 250;
            dialog.DataContext = viewModel;
            dialog.Owner = Application.Current.MainWindow;
            dialog.ShowDialog();
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
            set
            {
                Set(ref _isDirty, value);
                IsDirtyChanged?.Invoke(this, new EventArgs());
            }
            get { return _isDirty; }
        }

        public event EventHandler IsDirtyChanged;

        public string ChangeTypeToName(RoutineType type)
        {
            string name = "";
            switch (type)
            {
                case RoutineType.RLL:
                    name = "Ladder Diagram";
                    break;
                case RoutineType.FBD:
                    name = "Function Block Diagram";
                    break;
                case RoutineType.ST:
                    name = "Structured Text";
                    break;
            }

            return name;
        }

        public bool IsValidName()
        {
            string warningMessage = LanguageManager.GetInstance().ConvertSpecifier("Failed to modify the instruction.");
            string warningReason = string.Empty;
            bool isValid = true;

            if (string.IsNullOrEmpty(Name))
            {
                isValid = false;
                warningReason = LanguageManager.GetInstance().ConvertSpecifier("Name is invalid.");
            }

            if (isValid)
            {
                if (Name.Length > 40 || Name.EndsWith("_") ||
                    Name.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    isValid = false;
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("Name is invalid.");
                }
            }

            if (isValid)
            {
                Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
                if (!regex.IsMatch(Name))
                {
                    isValid = false;
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("Name is invalid."); ;
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
                    if (keyWord.Equals(Name, StringComparison.OrdinalIgnoreCase))
                    {
                        isValid = false;
                        warningReason = LanguageManager.GetInstance().ConvertSpecifier("Name is invalid.");
                    }
                }
            }

            //和数据类型重名的判断
            if (isValid)
            {
                foreach (var item in _controller.DataTypes)
                {
                    if (item != _aoiDefinition.datatype && item.Name.Equals(Name, StringComparison.OrdinalIgnoreCase))
                    {
                        isValid = false;
                        warningReason =
                            "A User-Defined Data Type by the same name already exists,which would collide with the instruction's Add-On-Defined Data Type.";
                    }
                }

            }

            //和指令重名的判断
            if (isValid)
            {
                var stInstructionCollection = Controller.GetInstance().STInstructionCollection;
                var rllInstructionCollection = Controller.GetInstance().RLLInstructionCollection;
                var fbdInstructionCollection = Controller.GetInstance().FBDInstructionCollection;

                if (stInstructionCollection.FindInstruction(Name) != null || 
                    rllInstructionCollection.FindInstruction(Name) != null ||
                    fbdInstructionCollection.FindInstruction(Name) != null)
                {
                    if (_aoiDefinition.Name != Name)
                    {
                        isValid = false;
                        warningReason = LanguageManager.GetInstance().ConvertSpecifier("This is a reserved instruction name.");
                    }
                }
            }

            if (!isValid)
            {
                var warningDialog = new ICSStudio.Dialogs.Warning.WarningDialog(warningMessage, warningReason)
                    {Owner = Application.Current.MainWindow};
                warningDialog.ShowDialog();
            }

            return isValid;
        }
    }
}
