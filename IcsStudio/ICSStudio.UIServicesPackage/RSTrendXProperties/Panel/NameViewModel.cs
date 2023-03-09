using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.UIServicesPackage.RSTrendXProperties.Panel
{
    class NameViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private readonly ITrend _trend;
        private string _name;
        private bool _isDirty;
        private string _description;
        private string _failedToCreateTrace;
        private string _nameIsInvalid;

        public NameViewModel(Name panel, ITrend trend)
        {
            Control = panel;
            panel.DataContext = this;
            _trend = trend;
            Name = trend.Name;
            Description = trend.Description;
            IsDirty = false;

            var trendLog = trend as TrendLog;
            PathVisibility = trendLog == null ? Visibility.Collapsed : Visibility.Visible;
            Enable = trendLog == null;
            if (trendLog != null)
            {
                var file = new FileInfo(trendLog.FilePath);
                FileName = file.Name;
                Path = file.Directory.ToString();
            }

            _failedToCreateTrace = LanguageManager.GetInstance().ConvertSpecifier("FailedToCreateTrace");
            _nameIsInvalid = LanguageManager.GetInstance().ConvertSpecifier("NameIsInvalid");

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            _failedToCreateTrace = LanguageManager.GetInstance().ConvertSpecifier("FailedToCreateTrace");
            _nameIsInvalid = LanguageManager.GetInstance().ConvertSpecifier("NameIsInvalid");
        }

        public string Path { set; get; }

        public string FileName { set; get; }

        public bool Enable { set; get; }

        public Visibility PathVisibility { set; get; }

        public string Name
        {
            set
            {
                Set(ref _name, value);
                IsDirty = true;
            }
            get { return _name; }
        }

        public bool IsClosing { set; get; }
        public void Save()
        {
            if(IsClosing)return;
            if (!IsDirty) return;
            if (_trend.GraphTitle.Equals(_trend.Name))
            {
                {
                    var trend = _trend as Trend;
                    if (trend != null)
                    {
                        trend.GraphTitle = Name;
                    }
                }
                {
                    var trend = _trend as TrendLog;
                    if (trend != null)
                    {
                        trend.GraphTitle = Name;
                    }
                }
            }

            _trend.Name = Name;
            _trend.Description = Description;

            IsDirty = false;
        }

        public string Description
        {
            set
            {
                _description = value;
                IsDirty = true;
            }
            get { return _description; }
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
                Set(ref _isDirty, value);
                IsDirtyChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler IsDirtyChanged;

        public bool Verify()
        {
            if (Name.Equals(_trend.Name)) return true;

            if (string.IsNullOrEmpty(Name))
            {
                MessageBox.Show(_failedToCreateTrace+$" '{_trend.Name}'.\n"+ _nameIsInvalid+".", "ICS Studio",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Asterisk);
                return false;
            }

            if (Name.Length > 40 || Name.EndsWith("_") ||
                Name.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
            {
                MessageBox.Show(_failedToCreateTrace+$" '{_trend.Name}'.\n"+ _nameIsInvalid+".", "ICS Studio",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Asterisk);
                return false;
            }


            Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
            var controller = _trend.ParentController;
            if (string.IsNullOrEmpty(Name) || !regex.IsMatch(Name))
            {
                MessageBox.Show(_failedToCreateTrace+$" '{_trend.Name}'.\n"+ _nameIsInvalid+".", "ICS Studio",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Asterisk);
                return false;
            }

            // key word

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
                    MessageBox.Show(_failedToCreateTrace+$" '{_trend.Name}'.\n"+ _nameIsInvalid+".", "ICS Studio",
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Asterisk);
                    return false;
                }
            }

            var trend = controller.Trends[Name];
            if (trend != null)
            {
                MessageBox.Show(_failedToCreateTrace+$" '{_trend.Name}'.\n"+LanguageManager.GetInstance().ConvertSpecifier("AlreadyExists") +".", "ICS Studio",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Asterisk);
                return false;
            }

            return true;
        }
    }
}
