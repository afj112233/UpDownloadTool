using System;
using System.Text.RegularExpressions;
using System.Windows;
using GalaSoft.MvvmLight;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.AxisVirtualProperties.Panel
{
    class TagViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private readonly SimpleServices.Tags.Tag _tag1;
        private readonly ITag _tag;
        private bool _isDirty;
        private string _name;
        private string _description;
        private string _external;
        private string _type;
        public object Owner { get; set; }
        public object Control { get; }

        public TagViewModel(Tag panel, ITag tag)
        {
            Control = panel;
            panel.DataContext = this;
            this._tag = tag;
            _tag1 = (SimpleServices.Tags.Tag) tag;
            Name = _tag1?.Name;
            Description = _tag1?.Description;
            Type = _tag1?.TagType.ToString();
            DataType = _tag1?.DataTypeInfo.DataType.Name;
            Scope = _tag1?.ParentController.Name;
            External = _tag1?.ExternalAccess.ToString();

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", OnLanguageChanged);
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                _tag.ParentController as Controller, "IsOnlineChanged", OnIsOnlineChanged);
        }

        public void Compare()
        {
            IsDirty = false;
            if (!(Name == (_tag1.Name == null ? "" : _tag1.Name))) IsDirty = true;
            if (!(Description == null ? "" : Description).Equals(_tag1.Description == null ? "" : _tag1.Description))
                IsDirty = true;
        }

        public bool CheckInvalid()
        {
            return IsValidTagName(Name);
        }

        public void Save()
        {
            _tag.Name = Name;
            _tag.Description = Description;
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

        public string Type
        {
            set { _type = value; }
            get
            {
                var type = LanguageManager.GetInstance().ConvertSpecifier(_type);
                return string.IsNullOrEmpty(type) ? _type : type;
            }
        }

        public string DataType { set; get; }
        public string Scope { set; get; }

        public string External
        {
            set { _external = value; }
            get
            {
                var external = LanguageManager.GetInstance().ConvertSpecifier(_external);
                return string.IsNullOrEmpty(external) ? _external : external;
            }
        }

        public void LoadOptions()
        {

        }

        public bool SaveOptions()
        {
            return true;
        }

        public bool IsEnabled => !_tag.ParentController.IsOnline;

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

        private bool IsValidTagName(string name)
        {
            string warningMessage = "Failed to modify the properties for the tag.";
            string warningReason = string.Empty;
            bool isValid = true;

            if (string.IsNullOrEmpty(name))
            {
                isValid = false;
                warningReason = "Tag name is empty.";
            }

            if (isValid)
            {
                if (name.Length > 40 || name.EndsWith("_") ||
                    name.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    isValid = false;
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("VariableNameInvalid");
                }
            }

            if (isValid)
            {
                Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
                if (!regex.IsMatch(name))
                {
                    isValid = false;
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("VariableNameInvalid");
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
                    "not", "mod", "and", "xor", "or"
                };
                foreach (var keyWord in keyWords)
                {
                    if (keyWord.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        isValid = false;
                        warningReason = LanguageManager.GetInstance().ConvertSpecifier("VariableNameInvalid");
                    }
                }
            }

            if (isValid)
            {
                var tag = this._tag.ParentController.Tags[name];
                if (tag != null && tag != this._tag)
                {
                    isValid = false;
                    warningReason = "Already exists.";
                }
            }


            //
            if (!isValid)
            {
                var warningDialog = new WarningDialog(warningMessage, warningReason)
                    {Owner = Application.Current.MainWindow};
                warningDialog.ShowDialog();
            }

            return isValid;
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(Type));
            RaisePropertyChanged(nameof(External));
        }

        public override void Cleanup()
        {
            base.Cleanup();
            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
                LanguageManager.GetInstance(), "LanguageChanged", OnLanguageChanged);
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                _tag.ParentController as Controller, "IsOnlineChanged", OnIsOnlineChanged);
        }

        public void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                RaisePropertyChanged(nameof(IsEnabled));
            });
        }
    }
}
