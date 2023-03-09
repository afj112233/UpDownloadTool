using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Components.Controls;
using ICSStudio.Dialogs.Filter;
using ICSStudio.Gui.Utils;
using ICSStudio.Gui.View;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.StxEditor.ViewModel.CodeSnippets;
using ICSStudio.UIInterfaces.Editor;
using ICSStudio.UIServicesPackage.View;
using Microsoft.VisualStudio.Shell;
using Pen = ICSStudio.SimpleServices.Common.Pen;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    internal class NewTrendViewModel : ViewModelBase
    {
        private string _title;
        private string _titleValue;
        private Visibility _firstVisibility;
        private Visibility _nextVisibility;
        private string _selectedTag;
        private bool? _dialogResult;
        private ScopeItem _selectedScope;
        private string _failedToCreateTrace;
        private string _nameIsInvalid;

        public NewTrendViewModel(NewTrend userControl, IController controller)
        {
            Controller = controller;
            Control = userControl;
            ScopeList = new List<ScopeItem>();
            ScopeList.Add(new ScopeItem(controller.Name, true));
            foreach (var program in controller.Programs)
            {
                ScopeList.Add(new ScopeItem(program.Name));
            }

            NameFilterPopup = userControl.NameFilterPopup;
            SelectedScope = ScopeList[0];
            FirstVisibility = Visibility.Visible;
            NextVisibility = Visibility.Collapsed;
            Period = 10;
            NameFilterCommand = new RelayCommand<Button>(ExecuteNameFilterCommand);
            AddCommand = new RelayCommand(ExecuteAddCommand, CanExecuteAddCommand);
            RemoveCommand = new RelayCommand(ExecuteRemoveCommand, CanExecuteRemoveCommand);
            BackCommand = new RelayCommand(ExecuteBackCommand, CanExecuteBackCommand);
            NextCommand = new RelayCommand(ExecuteNextCommand, CanExecuteNextCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            OKCommand = new RelayCommand(ExecuteOKCommand);
            PeriodList = EnumHelper.ToDataSource<TimeType>();
            PeriodList.RemoveAt(3);
            NameFilterPopup.FilterViewModel.Visibility = Visibility.Collapsed;
            NameFilterPopup.FilterViewModel.IsShowScope = false;
            NameFilterPopup.FilterViewModel.IsCrossReference = true;
            NameFilterPopup.FilterViewModel.PropertyChanged += FilterViewModel_PropertyChanged;

            _failedToCreateTrace = LanguageManager.GetInstance().ConvertSpecifier("FailedToCreateTrace");
            _nameIsInvalid = LanguageManager.GetInstance().ConvertSpecifier("NameIsInvalid");
            WeakEventManager<LanguageManager,EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            Title = LanguageManager.GetInstance().ConvertSpecifier(TitleValue);
            _failedToCreateTrace = LanguageManager.GetInstance().ConvertSpecifier("FailedToCreateTrace");
            _nameIsInvalid = LanguageManager.GetInstance().ConvertSpecifier("NameIsInvalid");
        }
       
        public List<ScopeItem> ScopeList { get; }

        public ScopeItem SelectedScope
        {
            set
            {
                _selectedScope = value;
                NameFilterPopup.ResetScope(value.IsController ? "" : value.Name, value.IsController);
            }
            get { return _selectedScope; }
        }

        public bool? DialogResult
        {
            set { Set(ref _dialogResult, value); }
            get { return _dialogResult; }
        }

        public Window Control { get; }

        public ObservableCollection<string> TagsToTrend { get; } = new ObservableCollection<string>();

        #region Command

        public RelayCommand CancelCommand { get; }

        private void ExecuteCancelCommand()
        {
            DialogResult = false;
        }

        public RelayCommand OKCommand { get; }

        private void ExecuteOKCommand()
        {
            if (!Verify())
            {
                ExecuteBackCommand();
                return;
            }

            var trend = new Trend(Controller);
            trend.Name = Name;
            trend.Description = Description;
            trend.SamplePeriod = (int) GetPeriod();
            foreach (var tag in TagsToTrend)
            {
                var pen = new Pen();
                pen.Name = tag;
                pen.Color = $"16{TrendObject.GetColor(TagsToTrend.IndexOf(tag)).ToString()}";
                trend.Add(pen);
            }

            Controller.Trends.Add(trend);
            DialogResult = true;

            var createEditorService = Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
            if(Controller.IsOnline)
                createEditorService?.CreateTrend(trend, true);
            else
                createEditorService?.CreateTrend(trend);
        }

        private bool Verify()
        {
            Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
            if (string.IsNullOrEmpty(Name) || !regex.IsMatch(Name))
            {
                MessageBox.Show(_failedToCreateTrace+" '" + Name + "'.\n"+ _nameIsInvalid+".", "ICS Studio", MessageBoxButton.OKCancel,
                    MessageBoxImage.Asterisk);
                return false;
            }

            var trend = Controller.Trends[Name];
            if (trend != null)
            {
                MessageBox.Show(_failedToCreateTrace+" '" + Name + "'.\n"+LanguageManager.GetInstance().ConvertSpecifier("AlreadyExists")+".", "ICS Studio", MessageBoxButton.OKCancel,
                    MessageBoxImage.Asterisk);
                return false;
            }

            // key word


            if (Name.Length > 40 || Name.EndsWith("_") ||
                Name.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
            {
                MessageBox.Show(_failedToCreateTrace+" '" + Name + "'.\n"+ _nameIsInvalid+".", "ICS Studio", MessageBoxButton.OKCancel,
                    MessageBoxImage.Asterisk);
                return false;
            }

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
                    MessageBox.Show(_failedToCreateTrace+" '" + Name + "'.\n"+ _nameIsInvalid+".", "ICS Studio", MessageBoxButton.OKCancel,
                        MessageBoxImage.Asterisk);
                    return false;
                }
            }

            var period = GetPeriod();
            if (period < 1 || period > 30 * 60 * 1000)
            {
                MessageBox.Show(
                    _failedToCreateTrace+" \'" + Name + "'.\n"+LanguageManager.GetInstance().ConvertSpecifier("SamplePeriodShouldBe")+".",
                    "ICS Studio", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
                return false;
            }

            return true;
        }

        private double GetPeriod()
        {
            double period = 0;
            if (SelectedPeriodType == TimeType.Minute)
            {
                period = Period * 30 * 1000;
            }
            else if (SelectedPeriodType == TimeType.Second)
            {
                period = Period * 30;
            }
            else
            {
                period = Period;
            }

            return period;
        }

        public RelayCommand<Button> NameFilterCommand { set; get; }

        private void ExecuteNameFilterCommand(Button sender)
        {
            var parentGrid = VisualTreeHelpers.FindVisualParentOfType<Grid>(sender);
            var autoCompleteBox = VisualTreeHelpers.FindFirstVisualChildOfType<TextBox>(parentGrid);
            if (!NameFilterPopup.IsOpen)
                NameFilterPopup.ResetPosition(autoCompleteBox);
            NameFilterPopup.IsOpen = !NameFilterPopup.IsOpen;
        }

        public RelayCommand BackCommand { get; }

        private void ExecuteBackCommand()
        {
            FirstVisibility = Visibility.Visible;
            NextVisibility = Visibility.Collapsed;
            BackCommand.RaiseCanExecuteChanged();
            NextCommand.RaiseCanExecuteChanged();
        }

        private bool CanExecuteBackCommand()
        {
            if (NextVisibility == Visibility.Visible) return true;
            return false;
        }

        public RelayCommand NextCommand { get; }

        private void ExecuteNextCommand()
        {
            FirstVisibility = Visibility.Collapsed;
            NextVisibility = Visibility.Visible;
            BackCommand.RaiseCanExecuteChanged();
            NextCommand.RaiseCanExecuteChanged();
        }

        private bool CanExecuteNextCommand()
        {
            if (FirstVisibility == Visibility.Visible) return true;
            return false;
        }

        public RelayCommand AddCommand { get; }

        private void ExecuteAddCommand()
        {
            if (TagsToTrend.Count == 8)
            {
                MessageBox.Show("Only 8 tags can be trended at a time.", "ICS Studio", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                return;
            }

            var name = TagName;
            var program = SelectedScope.IsController ? null : Controller.Programs[SelectedScope.Name];
            if (program != null) name = $@"\{program.Name}.{name}";
            TagsToTrend.Add(name);
            AddCommand.RaiseCanExecuteChanged();
        }

        private bool CanExecuteAddCommand()
        {
            var program = SelectedScope.IsController ? null : Controller.Programs[SelectedScope.Name];
            if (!SelectedScope.IsController && program == null) return false;
            var isTag = ExtendOperation.IsNumber(TagName, Controller, program);
            if (isTag)
            {
                string name = TagName;
                if (program != null) name = $@"\{program.Name}.{name}";
                if (!TagsToTrend.Contains(name)) return true;
            }

            return false;
        }

        public RelayCommand RemoveCommand { get; }

        private void ExecuteRemoveCommand()
        {
            TagsToTrend.Remove(SelectedTag);
            AddCommand.RaiseCanExecuteChanged();
            RemoveCommand.RaiseCanExecuteChanged();
        }

        private bool CanExecuteRemoveCommand()
        {
            if (!string.IsNullOrEmpty(SelectedTag))
                return true;
            return false;
        }

        #endregion

        public string SelectedTag
        {
            set
            {
                _selectedTag = value;
                RemoveCommand.RaiseCanExecuteChanged();
            }
            get { return _selectedTag; }
        }

        public NameFilterPopup NameFilterPopup { private set; get; }
        public Dictionary<ITag,TagNameNode> NameList => NameFilterPopup.FilterViewModel.AutoCompleteData;

        public string TagName
        {
            set
            {
                NameFilterPopup.FilterViewModel.Name = value;
                AddCommand.RaiseCanExecuteChanged();
            }
            get { return NameFilterPopup.FilterViewModel.Name; }
        }

        public IController Controller { get; }

        public TimeType SelectedPeriodType { set; get; }

        public IList PeriodList { get; }

        public Visibility FirstVisibility
        {
            set
            {
                Set(ref _firstVisibility, value);
                if (_firstVisibility == Visibility.Visible)
                    TitleValue = "NewTrendGeneral";
            }
            get { return _firstVisibility; }
        }

        public Visibility NextVisibility
        {
            set
            {
                Set(ref _nextVisibility, value);
                if (_nextVisibility == Visibility.Visible)
                    TitleValue = "NewTrendAddOrConfigureVariable";
            }
            get { return _nextVisibility; }
        }

        public ObservableCollection<ITag> Tags { get; } = new ObservableCollection<ITag>();

        public string Name { set; get; }

        public string Description { set; get; }

        public int Period { set; get; }

        public string TitleValue
        {
            set
            {
                _titleValue = value;
                Title = LanguageManager.Instance.ConvertSpecifier(_titleValue);
            }
            get { return _titleValue; }
        }

        public string Title
        {
            set { Set(ref _title, value); }
            get { return _title; }
        }

        private void FilterViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                RaisePropertyChanged("TagName");
                AddCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == "Hide")
            {
                NameFilterPopup.IsOpen = false;
            }
        }

    }
}
