using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Components.Controls;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.QuickWatchPackage.View;
using ICSStudio.QuickWatchPackage.View.Models;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Tags;

namespace ICSStudio.QuickWatchPackage
{
    internal class QuickWatchViewModel : ViewModelBase
    {
        private readonly QuickWatchPackageControl _control;
        private bool _isEnabled;
        private string _listName = string.Empty;
        private Visibility _refreshCommandVisibility;
        private Visibility _removeCommandVisibility;
        private WatchListItem _selectedItem;

        public  QuickWatchViewModel(QuickWatchPackageControl control)
        {
            _control = control;
            RemoveCommand = new RelayCommand(ExecuteRemoveCommand, CanExecuteRemoveCommand);
            RefreshCommand = new RelayCommand(ExecuteRefreshCommand);
            MonitorTagsViewModel = new MonitorTagsViewModel(control.MonitorControl, this);
            WeakEventManager<MonitorTagItemCollection, NotifyCollectionChangedEventArgs>.AddHandler(
                MonitorTagsViewModel.MonitorTagCollection, "CollectionChanged", MonitorTagCollection_CollectionChanged);
            //MonitorTagsViewModel.MonitorTagCollection.CollectionChanged += MonitorTagCollection_CollectionChanged;
            WatchListItems.Add(new WatchListItem
            {
                Name = "Quick Watch",
                Category = "0"
            });

            SelectedItem = WatchListItems[0];
            WatchListCollectionView = new ListCollectionView(WatchListItems);
            WatchListCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
            if (string.IsNullOrEmpty(Controller.GetInstance().Name))
            {
                IsEnabled = false;
                MonitorTagsViewModel.CanUserAddRows = false;
            }

            LanguageManager.GetInstance().SetLanguage(control);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged",
                LanguageChangedHandler);
        }

        public bool IsEnabled
        {
            set { Set(ref _isEnabled, value); }
            get { return _isEnabled; }
        }

        public IProgramModule ExplicitProgramModule { set; get; } = null;

        public MonitorTagsViewModel MonitorTagsViewModel { get; }
        public ListCollectionView WatchListCollectionView { get; }
        public ObservableCollection<WatchListItem> WatchListItems { get; } = new ObservableCollection<WatchListItem>();

        public WatchListItem SelectedItem
        {
            set
            {
                Set(ref _selectedItem, value);
                if (value != null)
                {
                    if (_selectedItem.Category == "0")
                    {
                        RemoveCommandVisibility = Visibility.Visible;
                        RefreshCommandVisibility = Visibility.Collapsed;
                        MonitorTagsViewModel.CanUserAddRows = true;
                    }
                    else
                    {
                        RemoveCommandVisibility = Visibility.Collapsed;
                        RefreshCommandVisibility = Visibility.Visible;
                        MonitorTagsViewModel.CanUserAddRows = false;
                    }

                    MonitorTagsViewModel.Update(_selectedItem?.Container);

                    if (_selectedItem != null &&
                        (_selectedItem.Name.Equals("Quick Watch") || _selectedItem.Name.Equals("快速观察"))) ListName = "";
                    else ListName = _selectedItem?.Name;
                    RemoveCommand.RaiseCanExecuteChanged();
                }
            }
            get { return _selectedItem; }
        }

        public WatchListItem SelectedWatchListItem { set; get; }

        public string ListName
        {
            set
            {
                if (_listName.Equals(value, StringComparison.OrdinalIgnoreCase)) return;
                Set(ref _listName, value);
                if (_selectedItem != null)
                {
                    if (WatchListItems.IndexOf(_selectedItem) == 0)
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                        }
                        else
                        {
                            //var count = WatchListItems.Count(item => item.Category == "0");
                            //_selectedItem.Name = _listName;
                            var newItem = new WatchListItem { Name = "Quick Watch", Category = "0" };
                            WatchListItems.Insert(0, newItem);
                            //SelectedItem = newItem;
                            _selectedItem.Name = _listName;
                            WatchListCollectionView?.Refresh();
                        }
                    }
                    else
                    {
                        _selectedItem.Name = _listName;
                        WatchListCollectionView?.Refresh();
                    }
                }
            }
            get { return _listName; }
        }

        public Visibility RemoveCommandVisibility
        {
            set { Set(ref _removeCommandVisibility, value); }
            get { return _removeCommandVisibility; }
        }

        public Visibility RefreshCommandVisibility
        {
            set { Set(ref _refreshCommandVisibility, value); }
            get { return _refreshCommandVisibility; }
        }

        public RelayCommand RemoveCommand { get; }

        public bool Editing { set; get; } = false;

        public RelayCommand RefreshCommand { get; }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            var control = sender as MonitorControl;
            LanguageManager.GetInstance().SetLanguage(control);
            SelectedItem.RaisePropertyChanged(nameof(SelectedItem.Name));
        }

        private void MonitorTagCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RemoveCommand.RaiseCanExecuteChanged();
        }

        public void UnlockQuickWatch()
        {
            IsEnabled = true;
            MonitorTagsViewModel.UnlockQuickWatch();
        }

        public void LockQuickWatch()
        {
            IsEnabled = false;
            MonitorTagsViewModel.LockQuickWatch();
        }

        public void SetAoiMonitor(IRoutine routine, AoiDataReference reference)
        {
            var watchListItem = WatchListItems.FirstOrDefault(w => w.MonitorObject == routine);
            if (watchListItem == null) return;
            if (SelectedItem == watchListItem) SelectedItem = WatchListItems[0];

            if (reference.Offset == -1)
            {
                watchListItem.Name = $"{routine.ParentCollection.ParentProgram.Name} - {routine.Name}<definition>";
                ((MonitorTagCollection)watchListItem.Container.Tags).Clear();
                watchListItem.AoiDataReference = null;
                foreach (var variableInfo in ((STRoutine)routine).GetCurrentVariableInfos(OnlineEditType.Original))
                    if (variableInfo.IsDisplay)
                    {
                        var tag = ParseTag(variableInfo.Name, (Tag)variableInfo.Tag,
                            routine.ParentCollection.ParentProgram, null);

                        ((MonitorTagCollection)watchListItem.Container.Tags).AddTag(tag);
                    }
            }
            else
            {
                watchListItem.Name =
                    $"{routine.ParentCollection.ParentProgram.Name} - {routine.Name} - {reference.Title}";
                ((MonitorTagCollection)watchListItem.Container.Tags).Clear();
                watchListItem.AoiDataReference = reference;
                foreach (var variableInfo in ((STRoutine)routine).GetAllReferenceTags())
                    ((MonitorTagCollection)watchListItem.Container.Tags).AddTag(variableInfo);
                //if (variableInfo.IsDisplay)
                //{
                //    var tag = ParseTag(variableInfo.Name,(Tag)variableInfo.Tag, reference.GetReferenceProgram(), reference.TransformTable);

                //    ((MonitorTagCollection)watchListItem.Container.Tags).AddTag(tag);
                //}
            }
        }

        public void Clean()
        {
            if (!IsEnabled) return;
            WatchListItems.Clear();
            WatchListItems.Add(new WatchListItem
            {
                Name = "Quick Watch",
                Category = "0"
            });
            SelectedItem = WatchListItems[0];
        }

        internal List<IProgramModule> GetReachableScope()
        {
            var list = new List<IProgramModule>();

            if (ExplicitProgramModule != null) list.Add(ExplicitProgramModule);

            foreach (IProgramModule program in Controller.GetInstance().Programs)
            {
                if (program == ExplicitProgramModule) continue;
                if (program.IsInMonitor) list.Add(program);
            }

            foreach (var watchListItem in WatchListItems.Where(w => string.IsNullOrEmpty(w.Category)))
            {
                var st = watchListItem.MonitorObject as STRoutine;
                if (st != null)
                    if (!list.Contains(st.ParentCollection.ParentProgram))
                        if (!(st.ParentCollection.ParentProgram is AoiDefinition))
                            list.Add(st.ParentCollection.ParentProgram);
            }

            return list;
        }

        public void AddMonitorRoutine(IRoutine routine)
        {
            var st = routine as STRoutine;
            if (st != null)
            {
                var item = new WatchListItem(st);
                if (routine.ParentCollection.ParentProgram is AoiDefinition)
                    item.Name = $"{st.ParentCollection.ParentProgram.Name} - {st.Name}<definition>";
                else
                    item.Name = $"{st.ParentCollection.ParentProgram.Name} - {st.Name}";
                WatchListItems.Add(item);
                var list = st.GetCurrentVariableInfos(OnlineEditType.Original).ToList();
                var autoList = new List<string>();
                foreach (var variableInfo in list)
                    if (variableInfo.IsDisplay)
                    {
                        var tag = ParseTag(variableInfo.Name, (Tag)variableInfo.Tag,
                            routine.ParentCollection.ParentProgram, null);
                        ((MonitorTagCollection)item.Container.Tags).AddTag(tag);
                        if (!MonitorTagsViewModel.NameList.ContainsKey(tag))
                        {
                            MonitorTagsViewModel.NameList[tag] = new TagNameNode(variableInfo.Name);
                            autoList.Add(variableInfo.Name);
                        }
                    }

                if (st.PendingEditsExist)
                {
                    list = st.GetCurrentVariableInfos(OnlineEditType.Pending).ToList();
                    foreach (var variableInfo in list)
                        if (variableInfo.IsDisplay && !autoList.Contains(variableInfo.Name))
                        {
                            var tag = ParseTag(variableInfo.Name, (Tag)variableInfo.Tag,
                                routine.ParentCollection.ParentProgram, null);
                            ((MonitorTagCollection)item.Container.Tags).AddTag(tag);
                            if (!MonitorTagsViewModel.NameList.ContainsKey(tag))
                            {
                                MonitorTagsViewModel.NameList[tag] = new TagNameNode(variableInfo.Name);
                                autoList.Add(variableInfo.Name);
                            }
                        }
                }

                if (st.TestCodeText != null)
                {
                    list = st.GetCurrentVariableInfos(OnlineEditType.Test).ToList();
                    foreach (var variableInfo in list)
                        if (variableInfo.IsDisplay && !autoList.Contains(variableInfo.Name))
                        {
                            var tag = ParseTag(variableInfo.Name, (Tag)variableInfo.Tag,
                                routine.ParentCollection.ParentProgram, null);
                            ((MonitorTagCollection)item.Container.Tags).AddTag(tag);
                            if (!MonitorTagsViewModel.NameList.ContainsKey(tag))
                            {
                                MonitorTagsViewModel.NameList[tag] = new TagNameNode(variableInfo.Name);
                                autoList.Add(variableInfo.Name);
                            }
                        }
                }
            }
        }

        private ITag ParseTag(string name, Tag t, IProgramModule parentScope, Hashtable transform)
        {
            var info = ObtainValue.GetSpecifierDataInfo(name, t, parentScope, transform);
            if (info != null)
            {
                var tag = info.Item4;
                if (!tag.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    var memberTag = new MemberTag(tag);
                    memberTag.Name = name;
                    memberTag.DisplayStyle = info.Item5;
                    memberTag.DataWrapper = new DataWrapper(info.Item2, info.Item3);
                    memberTag.Index = info.Item1;
                    memberTag.Transform = transform;
                    memberTag.AddListen();
                    memberTag.IsInCreate = false;
                    return memberTag;
                }

                return tag;
            }

            return null;
        }


        public void RemoveMonitorRoutine(IRoutine routine)
        {
            var item = WatchListItems.FirstOrDefault(w => w.MonitorObject == routine);
            if (item != null)
            {
                if (SelectedItem == item) SelectedItem = WatchListItems[0];
                WatchListItems.Remove(item);
            }
        }

        public void AddScope(ITagCollectionContainer container)
        {
        }

        public void RemoveScope(ITagCollectionContainer container)
        {
        }

        private void ExecuteRemoveCommand()
        {
            if (WatchListItems.IndexOf(SelectedItem) == 0)
            {
                if (MessageBox.Show("Clear Watch Tag(s)?", "ICS Studio",
                        MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    ((MonitorTagCollection)((MonitorContainer)SelectedItem.Container).Tags).Clear();
                    MonitorTagsViewModel.Clear();
                }
            }
            else
            {
                if (MessageBox.Show($"Delete the Quick Watch List \"{_selectedItem.Name}\"?", "ICS Studio",
                        MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    WatchListItems.Remove(SelectedItem);
                    SelectedItem = WatchListItems[0];
                    WatchListCollectionView?.Refresh();
                }
            }

            RemoveCommand.RaiseCanExecuteChanged();
        }

        private bool CanExecuteRemoveCommand()
        {
            if (Editing) return false;
            if (SelectedItem == null) return false;
            if (SelectedItem.Category == "0") return MonitorTagsViewModel.MonitorTagCollection.Count > 0;
            //if (WatchListItems.IndexOf(SelectedItem) == 0)
            //{
            //    return SelectedItem.Container.Tags.Count > 0;
            //}
            return true;
        }

        private void ExecuteRefreshCommand()
        {
            var st = SelectedItem.MonitorObject as STRoutine;
            if (st == null) return;
            ((MonitorTagCollection)SelectedItem.Container.Tags).Clear();
            foreach (var variableInfo in st.GetAllReferenceTags())
                ((MonitorTagCollection)SelectedItem.Container.Tags).AddTag(variableInfo);
            //if (variableInfo.IsDisplay)
            //{
            //    var tag = ParseTag(variableInfo.Name,(Tag)variableInfo.Tag, st.ParentCollection.ParentProgram, SelectedItem.AoiDataReference?.TransformTable);
            //    ((MonitorTagCollection)SelectedItem.Container.Tags).AddTag(tag);
            //}
            MonitorTagsViewModel.Refresh();
        }
    }

    public class WatchListItem : ViewModelBase
    {
        private string _name;

        public WatchListItem()
        {
            Container = new MonitorContainer();
        }

        public WatchListItem(STRoutine routine)
        {
            Container = new MonitorContainer();
            MonitorObject = routine;
        }

        public WatchListItem(ITagCollectionContainer container)
        {
            Container = container;
        }

        public object MonitorObject { get; }

        public ITagCollectionContainer Container { get; }

        public string Name
        {
            set { Set(ref _name, value); }
            get
            {
                if (_name.Equals("Quick Watch") || _name.Equals("快速观察"))
                    _name = LanguageManager.GetInstance().ConvertSpecifier("QuickWatch");
                return _name;
            }
        }

        public string Category { set; get; }

        public AoiDataReference AoiDataReference { set; get; }

        private void LanguageChanged(object sender, EventArgs e)
        {
            if (Name.Equals("Quick Watch") || Name.Equals("快速观察"))
                Name = LanguageManager.GetInstance().ConvertSpecifier("QuickWatch");
        }
    }
}