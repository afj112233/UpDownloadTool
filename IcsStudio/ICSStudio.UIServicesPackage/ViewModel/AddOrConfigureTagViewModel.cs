using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Components.Controls;
using ICSStudio.Dialogs.Filter;
using ICSStudio.Gui.View;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.StxEditor.ViewModel.CodeSnippets;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    class AddOrConfigureTagViewModel:ViewModelBase
    {
        //private readonly ITrend _trend;
        private string _selectedTag;
        private bool? _dialogResult;
        private readonly List<string> _penList;
        private readonly List<string> _addPenList=new List<string>();
        private readonly List<string> _removePenList=new List<string>();
        private ScopeItem _selectedScope;
        private string _trendName;

        public AddOrConfigureTagViewModel(System.Windows.Window window,string trendName,IController controller,List<string> penList)
        {
            _trendName=trendName;
            Title = LanguageManager.GetInstance().ConvertSpecifier("Add/Configure Variable")+$" - {_trendName}";
            window.DataContext = this;
            NameFilterPopup = new NameFilterPopup();
            NameFilterPopup.FilterViewModel.IsCrossReference = true;

            Controller = controller;
            _penList = penList;
            ScopeList = new List<ScopeItem>();
            ScopeList.Add(new ScopeItem(controller.Name,true));
            foreach (var program in controller.Programs)
            {
                ScopeList.Add(new ScopeItem(program.Name));
            }

            SelectedScope = ScopeList[0];
            OKCommand =new RelayCommand(ExecuteOKCommand);
            CancelCommand=new RelayCommand(ExecuteCancelCommand);

            NameFilterCommand = new RelayCommand<Button>(ExecuteNameFilterCommand);
            AddCommand = new RelayCommand(ExecuteAddCommand, CanExecuteAddCommand);
            RemoveCommand = new RelayCommand(ExecuteRemoveCommand, CanExecuteRemoveCommand);
            SetPens();
            NameFilterPopup.FilterViewModel.PropertyChanged += FilterViewModel_PropertyChanged;
            
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            Title = LanguageManager.GetInstance().ConvertSpecifier("Add/Configure Variable") + $" - {_trendName}";
        }

        public List<ScopeItem> ScopeList { get; }

        public ScopeItem SelectedScope
        {
            set
            {
                _selectedScope = value;
                NameFilterPopup.ResetScope(value.IsController ?"": value.Name, value.IsController);
            }
            get { return _selectedScope; }
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

        private void SetPens()
        {
            TagsToTrend.Clear();
            foreach (var pen in _penList)
            {
                TagsToTrend.Add(pen);
            }
        }

        public IController Controller { get; }

        public string TagName
        {
            set
            {
                NameFilterPopup.FilterViewModel.Name = value;
                AddCommand.RaiseCanExecuteChanged();
            }
            get { return NameFilterPopup.FilterViewModel.Name; }
        }
        
        public NameFilterPopup NameFilterPopup { private set; get; }

        public ObservableCollection<string> TagsToTrend { get; } = new ObservableCollection<string>();

        public Dictionary<ITag, TagNameNode> NameList => NameFilterPopup.FilterViewModel.AutoCompleteData;
        
        public string SelectedTag
        {
            set
            {
                _selectedTag = value;
                RemoveCommand.RaiseCanExecuteChanged();
            }
            get { return _selectedTag; }
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
            _addPenList.Add(name);
            _removePenList.Remove(name);
            TagsToTrend.Add(name);
            AddCommand.RaiseCanExecuteChanged();
        }

        private bool CanExecuteAddCommand()
        {
            var program = SelectedScope.IsController ? null : Controller.Programs[SelectedScope.Name];
            if (!SelectedScope.IsController &&program == null) return false;

            var isTag = ExtendOperation.IsNumber(TagName, Controller, program);
            if (isTag)
            {
                string name = TagName;
                if (program != null)
                {
                    var cx = false;
                    foreach (var i in program.Tags)
                    {
                        if (i.Name == TagName)
                            cx = true;
                        if (TagName.Contains(i.Name + "."))
                            cx = true;
                    }

                    if (cx == false)
                        return false;
                }
                if (program != null) name = $@"\{program.Name}.{name}";
                if (!TagsToTrend.Contains(name)) return true;
            }
            return false;
        }

        public RelayCommand RemoveCommand { get; }

        private void ExecuteRemoveCommand()
        {
            _removePenList.Add(SelectedTag);
            _addPenList.Remove(SelectedTag);
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

        public RelayCommand OKCommand { get; }

        private void ExecuteOKCommand()
        {
            _penList.Clear();
            foreach (var pen in TagsToTrend)
            {
                _penList.Add(pen);
            }
            DialogResult = true;
            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        public RelayCommand CancelCommand { get; }

        private void ExecuteCancelCommand()
        {
            DialogResult = false;
        }

        public bool? DialogResult
        {
            set { Set(ref _dialogResult , value); }
            get { return _dialogResult; }
        }

        public string Title { get; set; }

        public List<string> AddPenList => _addPenList;

        public List<string> RemovePenList => _removePenList;
    }

    public class ScopeItem
    {
        public ScopeItem(string name, bool isController=false)
        {
            Name = name;
            IsController = isController;
        }

        public string Name { get; }

        public bool IsController { get; }
    }
}
