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
using CTask = ICSStudio.SimpleServices.Common.CTask;

namespace ICSStudio.UIServicesPackage.TaskProperties.panel
{
    public class GeneralViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private readonly Controller _controller;
        private bool _isDirty;
        private readonly CTask _task;
        public string _name;
        public string _description;

        public GeneralViewModel(General panel, ITask task)
        {
            _controller = task.ParentController as Controller ?? Controller.GetInstance();

            panel.DataContext = this;
            Control = panel;
            _task = task as CTask;
            Name = task.Name;
            Description = task.Description;

            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                _controller, "IsOnlineChanged", OnIsOnlineChanged);
        }

        public void Compare()
        {
            IsDirty = false;
            if (Name != _task.Name) IsDirty = true;
            if (!(Description == null ? "" : Description).Equals(_task.Description == null ? "" : _task.Description))
                IsDirty = true;
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

        public object Owner { get; set; }
        public object Control { get; }

        public bool CheckInvalid()
        {
            if (!IsValidTaskName(Name)) return false;
            return true;
        }

        public void Save()
        {
            _task.Name = Name;
            _task.Description = Description;
        }

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
            string warningMessage = LanguageManager.GetInstance().ConvertSpecifier("Failed to save a new task '");
            warningMessage += $"{_task.Name}";
            warningMessage += LanguageManager.GetInstance().ConvertSpecifier("'.");
            string warningReason = string.Empty;
            bool isValid = true;

            if (string.IsNullOrEmpty(name))
            {
                isValid = false;
                warningReason = LanguageManager.GetInstance().ConvertSpecifier("task name invalid.");
            }

            if (isValid)
            {
                if (name.Length > 40 || name.EndsWith("_") ||
                    name.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    isValid = false;
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("task name invalid.");
                }
            }

            if (isValid)
            {
                Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
                if (!regex.IsMatch(name))
                {
                    isValid = false;
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("task name invalid.");
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
                        warningReason = LanguageManager.GetInstance().ConvertSpecifier("task name invalid.");
                    }
                }
            }

            if (isValid)
            {
                var task = this._task.ParentController.Tasks[name];
                if (task != null && task != this._task)
                {
                    isValid = false;
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

        public event EventHandler IsDirtyChanged;

        public override void Cleanup()
        {
            base.Cleanup();
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
