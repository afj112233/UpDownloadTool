using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Components.Controls;
using ICSStudio.Diagrams;
using ICSStudio.Dialogs.Filter;
using ICSStudio.Dialogs.NewTag;
using ICSStudio.Gui.View;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Notification;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.QuickWatchPackage.View.Models;
using ICSStudio.QuickWatchPackage.View.UI;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.SimpleServices.PredefinedType;
using Microsoft.VisualStudio.Shell;
using TagItem = ICSStudio.QuickWatchPackage.View.Models.TagItem;
using Task = System.Threading.Tasks.Task;

namespace ICSStudio.QuickWatchPackage.View
{
    internal partial class MonitorTagsViewModel : ViewModelBase, IConsumer
    {
        private MonitorTagItem _selectedItem;

        private bool _canUserAddRows;
        private bool _descending;
        private bool _includeTagMembersInSorting;
        private readonly object _collectionLock = new object();
        private readonly MonitorControl _control;

        public MonitorTagsViewModel(MonitorControl control, QuickWatchViewModel parentModel)
        {
            _control = control;
            ParentModel = parentModel;
            Controller = SimpleServices.Common.Controller.GetInstance();
            MonitorTagCollection = new MonitorTagItemCollection();

            OldSelectItem = null;

            BindingOperations.EnableCollectionSynchronization(MonitorTagCollection, _collectionLock);

            SortCommand = new RelayCommand<string>(ExecuteSort);
            SortIncludeTagMembersCommand = new RelayCommand<string>(ExecuteSortIncludeTagMembers);

            CopyCommand = new RelayCommand(ExecuteCopy, CanExecuteCopy);
            CutCommand = new RelayCommand(ExecuteCut, CanExecuteCut);
            PasteCommand = new RelayCommand(ExecutePaste, CanExecutePaste);
            DeleteCommand = new RelayCommand(ExecuteDelete, CanExecuteDelete);

            if (!string.IsNullOrEmpty(Controller.Name))
            {
                NameFilterCommand = new RelayCommand<Button>(ExecuteNameFilterCommand);
                NameFilterPopup = new NameFilterPopup();
                WeakEventManager<FilterViewModel, PropertyChangedEventArgs>.AddHandler(NameFilterPopup.FilterViewModel,
                    "PropertyChanged", FilterViewModel_PropertyChanged);
            }
            Notifications.ConnectConsumer(this);

            LanguageManager.GetInstance().SetLanguage(control);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        private void LanguageChanged(object sender, EventArgs e)
        {
            var control = sender as MonitorControl;
            LanguageManager.GetInstance().SetLanguage(control);

            RaisePropertyChanged(nameof(ValueColumnName));
        }

        public void UnlockQuickWatch()
        {
            CanUserAddRows = true;

            if (NameFilterPopup == null)
            {
                NameFilterCommand = new RelayCommand<Button>(ExecuteNameFilterCommand);
                NameFilterPopup = new NameFilterPopup();
            }
            else
            {
                //MonitorTagCollection.Add(new MonitorTagItem());
            }

            WeakEventManager<FilterViewModel, PropertyChangedEventArgs>.AddHandler(NameFilterPopup.FilterViewModel,
                "PropertyChanged", FilterViewModel_PropertyChanged);
        }

        public void LockQuickWatch()
        {
            CanUserAddRows = false;
            if (NameFilterPopup?.FilterViewModel != null)
                WeakEventManager<FilterViewModel, PropertyChangedEventArgs>.RemoveHandler(
                    NameFilterPopup.FilterViewModel, "PropertyChanged", FilterViewModel_PropertyChanged);
        }

        public Dictionary<ITag, TagNameNode> NameList => NameFilterPopup.FilterViewModel.AutoCompleteData;

        public NameFilterPopup NameFilterPopup { get; private set; }

        private void FilterViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var item = OldSelectItem == null ? _selectedItem : OldSelectItem;
            if (item == null) return;
            if (e.PropertyName == "Name")
            {
                if (!string.IsNullOrEmpty(NameFilterPopup.FilterViewModel.Name)&&NameFilterPopup.FilterViewModel.SelectedTag!=null)
                {
                    //item.IsGetDetail = false;
                    //item.Name = NameFilterPopup.FilterViewModel.Name;

                    //item.IsGetDetail = true;
                    //item.OnPropertyChanged("Name");
                    if (_currentCell != null)
                    {
                        var textBox =
                            VisualTreeHelpers.FindFirstVisualChildOfType<FastAutoCompleteTextBox>(_currentCell);
                        if (textBox != null)
                            textBox.Text = NameFilterPopup.FilterViewModel.Name;
                    }
                }
                return;
            }

            if (e.PropertyName == "Hide")
            {
                NameFilterPopup.IsOpen = false;
                bool isCorrect = true;
                if (TagNameValidationRule
                    .ValidateName(NameFilterPopup.FilterViewModel.Name, item, ParentModel, ref isCorrect).IsValid)
                {
                    item.ParentScope =
                        Controller.Programs[NameFilterPopup.FilterViewModel.SelectedTag?.Scope];
                    var name = NameFilterPopup.FilterViewModel.Name;
                    if (name.StartsWith("\\"))
                        name = name.Substring(name.IndexOf(".") + 1);
                    item.Name = name;

                    item.OnPropertyChanged("Name");

                    _control.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    _control.MainDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
                    _control.Refresh();
                }
            }
        }

        public RelayCommand<Button> NameFilterCommand { get; private set; }

        private void ExecuteNameFilterCommand(Button sender)
        {
            if (string.IsNullOrEmpty(Controller.Name)) return;
            var parentGrid = VisualTreeHelpers.FindVisualParentOfType<DataGridCell>(sender);
            if (!NameFilterPopup.IsOpen)
                NameFilterPopup.ResetAll(ParentModel.ExplicitProgramModule?.Name, parentGrid);
            _currentCell = parentGrid;
            NameFilterPopup.IsOpen = !NameFilterPopup.IsOpen;
        }

        private DataGridCell _currentCell = null;
        public QuickWatchViewModel ParentModel { get; }

        public void Clear()
        {
            MonitorTagCollection.Clear();
        }

        public IController Controller { get; }
        public ITagCollectionContainer Scope { get; private set; }
        public MonitorTagItemCollection MonitorTagCollection { get; set; }

        public bool CanUserAddRows
        {
            set
            {
                var defaultView = CollectionViewSource.GetDefaultView(MonitorTagCollection) as ListCollectionView;
                if (defaultView != null)
                {
                    if (defaultView.IsAddingNew)
                        defaultView.CancelNew();

                    if (defaultView.IsEditingItem)
                        defaultView.CancelEdit();
                }

                Set(ref _canUserAddRows, value);
            }
            get { return _canUserAddRows; }
        }

        public MonitorTagItem OldSelectItem { get; set; }

        public MonitorTagItem SelectedMonitorTagItem
        {
            get { return _selectedItem; }
            set
            {
                if (value != null)
                {
                    OldSelectItem = _selectedItem;
                }

                Set(ref _selectedItem, value);
            }
        }

        private void Value_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NameFilterPopup.FilterViewModel.Name = ((MonitorTagItem) sender).Name;
        }

        public List<MonitorTagItem> SelectedMonitorTagItems { get; private set; }

        public bool IsInAOI => Scope?.Tags?.ParentProgram is AoiDefinition;

        private string _valueColumnName;

        public string ValueColumnName
        {
            get
            {
                if (IsInAOI)
                    return "Default";
                _valueColumnName = LanguageManager.GetInstance().ConvertSpecifier("Value");
                //LanguageManager.GetInstance().LanguageChanged += ValueColumnNameLanguageChanged;
                return _valueColumnName;
            }
            set
            {
                Set(ref _valueColumnName, value);
            }
        }
        private void ValueColumnNameLanguageChanged(object sender, EventArgs e)
        {
            ValueColumnName = LanguageManager.GetInstance().ConvertSpecifier("Value");
        }

        public Visibility UsageVisibility
        {
            get
            {
                if (Scope == Controller)
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }


        public override void Cleanup()
        {
            Notifications.DisconnectConsumer(this);

            MonitorTagCollection.Update(null);

            BindingOperations.DisableCollectionSynchronization(MonitorTagCollection);

            base.Cleanup();
        }

        public void Refresh()
        {
            MonitorTagCollection.Refresh();
            RaisePropertyChanged("ValueColumnName");
        }

        public void Update(ITagCollectionContainer scope)
        {
            MonitorTagCollection.Update(scope);
            Scope = scope;
            RaisePropertyChanged("ValueColumnName");
        }

        public void SetFocusName(string focusName)
        {
            SelectedMonitorTagItem = MonitorTagCollection.GetTagItemAndExpandByName(focusName);
        }

        public void Consume(MessageData message)
        {
            if (message.Type == MessageData.MessageType.PullFinished
                || message.Type == MessageData.MessageType.Restored)
            {
                ThreadHelper.JoinableTaskFactory.RunAsync(
                    async delegate
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        foreach (var item in MonitorTagCollection)
                        {
                            item.OnPropertyChanged("Value");
                        }
                    });
            }

            if (message.Type == MessageData.MessageType.MonitorTag)
            {
                //ThreadHelper.JoinableTaskFactory.RunAsync(
                //    async delegate
                //    {
                //        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                //        var program = message.Object as Program;
                //        if (program != null)
                //        {
                //            foreach (var t in program.Tags)
                //            {
                //                //((LargeCollection<string>)NameList).AddRange(0, GetAllTagMemberName(t));
                //                if (NameList.ContainsKey(t))
                //                {
                //                    var tagNameNode = NameList[t];
                //                    tagNameNode.SubNameNodes.Clear();
                //                }
                //                else
                //                {
                //                    NameList[t] = GetAllTagMemberName(t);
                //                }
                //            }
                //        }
                //    });
            }
        }

        //private List<string> GetAllTagMemberName(ITag tag)
        //{
        //    var 
        //    FilterViewModel.ParseMember(list,tag.Name,tag.DataTypeInfo);
        //    list.Add(tag.Name);
        //    return list;
        //}

    }
}
