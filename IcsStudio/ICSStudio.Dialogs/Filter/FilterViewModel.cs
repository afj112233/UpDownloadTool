using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Annotations;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.PredefinedType;
using System.Runtime.Serialization;
using System.Threading;
using ICSStudio.Components.Controls;
using ICSStudio.Dialogs.NewTag;
using ICSStudio.Gui.Controls;
using ICSStudio.Interfaces.Notification;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Notification;
using DataType = ICSStudio.SimpleServices.DataType.DataType;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Task = System.Threading.Tasks.Task;

namespace ICSStudio.Dialogs.Filter
{
    public class FilterViewModel : ViewModelBase, IConsumer
    { 
        private Visibility _visibilityCol3;
        private Item _selectedShowItem;
        private readonly Controller _controller;
        private IProgramModule _parentProgram;
        private string _name;
        private bool _showController = true;
        private bool _showProgram = true;
        private string _filterName;
        private string _selectedOther;
        private TagItem _selectedTag;
        private readonly List<Item> _items;
        private FilterOnType _selectedFilterOnType;
        private bool _doSetCheckBox = true;
        private bool _isAoi = false;
        private bool _doRename = true;

        //private bool _doChanged = true;
        private readonly List<string> _usageList = new List<string>();
        private readonly List<string> _dataTypes = new List<string>();
        private readonly List<string> _usages = new List<string>();
        private bool _inputs;
        private bool _inOuts;
        private bool _outputs;
        private bool _locals;
        private bool _controllerCheckBoxEnable = true;
        private bool _programCheckBoxEnable = true;
        private Visibility _visibility;

        public FilterViewModel(IController controller, IProgramModule parentProgram, bool isOnlyShowDataType,
            bool isShowScope, string preFilterDataTypes,
            bool isCrossReference = false)
        {
            _isCrossReference = isCrossReference;
            IsShowScope = isShowScope;
            Visibility = isOnlyShowDataType ? Visibility.Collapsed : Visibility.Visible;
            Notifications.ConnectConsumer(this);
            _controller = (Controller)controller;
            WeakEventManager<Controller, EventArgs>.AddHandler(_controller, "Loaded", Controller_Loaded);
            _parentProgram = parentProgram;
            Command = new RelayCommand(ExecuteCommand);
            //NameList = new NameCollection();
            AutoCompleteData=new Dictionary<ITag, TagNameNode>();
            VisibilityCol3 = Visibility.Collapsed;
            _items = new List<Item>();
            ShowController = true;
            ShowProgram = true;
            if (parentProgram is AoiDefinition)
            {
                ShowAOI = Visibility.Visible;
                ShowLastRow = Visibility.Collapsed;
                ShowNormal = Visibility.Collapsed;
                _inputs = true;
                _inOuts = true;
                _outputs = true;
                _locals = true;
                _isAoi = true;
            }
            else
            {
                ShowAOI = Visibility.Collapsed;
                ShowLastRow = Visibility.Visible;
                ShowNormal = Visibility.Visible;
            }
            
            _items.Add(new Item() { Name = "All Variables", Category = "0" });
            _items.Add(new Item() { Name = "Configure...", Category = "0" });
            _items.Add(new Item() { Name = "STRING" });
            if (!string.IsNullOrEmpty(preFilterDataTypes))
            {
                var item = new Item() { Name = preFilterDataTypes };
                //ShowList.AddNewItem(item);
                _items.Add(item);
                SelectedShowItem = item;
            }
            else
            {
                SelectedShowItem = _items[0];
            }

            ShowList = new ListCollectionView(_items);
            ShowList.GroupDescriptions.Add(new PropertyGroupDescription("Category"));

            SelectedFilterOnType = FilterOnType.All;
            FilterOnList = EnumHelper.ToDataSource<FilterOnType>();
            //SetNameList();
            SetDataTypeItem();
            //SetTagItems();
            SetOtherProgram();
            CollectionChangedEventManager.AddHandler(FilterDataTypes, FilterDataTypes_CollectionChanged);
            if (!string.IsNullOrEmpty(preFilterDataTypes))
            {
                SetDataTypeCheck(preFilterDataTypes);
            }

            if (OtherProgramList.Any())
                SelectedOther = OtherProgramList[0];
            //FilterDataTypes.CollectionChanged += FilterDataTypes_CollectionChanged;
            SetDataGridFocus();
            DataGridDoubleClickCommand = new RelayCommand(ExecuteDataGridDoubleClickCommand);
            Guid = Guid.NewGuid();
            _show = LanguageManager.GetInstance().ConvertSpecifier("Show");
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        public bool IsCrossReference
        {
            set
            {
                _isCrossReference = value;
                if (_isCrossReference)
                {
                    ShowLastRow = Visibility.Collapsed;
                    ControllerCheckBoxEnable = false;
                    ProgramCheckBoxEnable = false;
                    if (_parentProgram == null) ShowProgram = false;
                    else if (_controller == null) ShowController = false;
                    else
                    {
                        ShowController = false;
                    }
                }
                else
                {
                    ShowLastRow = Visibility.Visible;
                    ControllerCheckBoxEnable = true;
                    ProgramCheckBoxEnable = true;
                    if (_parentProgram == null) ShowProgram = true;
                    else if (_controller == null) ShowController = true;
                    else
                    {
                        ShowController = true;
                    }
                }
            }
            get { return _isCrossReference; }
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            _show = LanguageManager.GetInstance().ConvertSpecifier("Show");
        }

        public Guid Guid { get; }
        public bool IsShowScope { set; get; }

        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                RaisePropertyChanged();
            }
        }

        private void Controller_Loaded(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                Reset();
            });
        }

        private void Reset()
        {
            SetDataTypeItem();
            SetOtherProgram();
            SetTagItems();
        }

        public static void ParseMember(List<string> list, string baseName, DataTypeInfo dataTypeInfo)
        {
            if (dataTypeInfo.Dim3 > 0)
            {
                for (int i = 0; i < dataTypeInfo.Dim3; i++)
                {
                    for (int j = 0; j < dataTypeInfo.Dim2; j++)
                    {
                        for (int k = 0; k < dataTypeInfo.Dim1; k++)
                        {
                            var name = $"{baseName}[{i},{j},{k}]";
                            list.Add(name);
                            ParseMember(list, name, new DataTypeInfo() { DataType = dataTypeInfo.DataType });
                        }
                    }
                }

                return;
            }

            if (dataTypeInfo.Dim2 > 0)
            {
                for (int i = 0; i < dataTypeInfo.Dim2; i++)
                {
                    for (int j = 0; j < dataTypeInfo.Dim1; j++)
                    {
                        var name = $"{baseName}[{i},{j}]";
                        list.Add(name);
                        ParseMember(list, name, new DataTypeInfo() { DataType = dataTypeInfo.DataType });
                    }
                }

                return;
            }

            if (dataTypeInfo.Dim1 > 0)
            {
                for (int i = 0; i < dataTypeInfo.Dim1; i++)
                {
                    var name = $"{baseName}[{i}]";
                    list.Add(name);
                    ParseMember(list, name, new DataTypeInfo() { DataType = dataTypeInfo.DataType });
                }

                return;
            }

            if (dataTypeInfo.DataType.IsBool || dataTypeInfo.DataType.IsReal) return;
            if (dataTypeInfo.DataType.IsInteger)
            {
                for (int i = 0; i < dataTypeInfo.DataType.BitSize; i++)
                {
                    var name = $"{baseName}.{i}";
                    list.Add(name);
                }
            }

            var compositive = dataTypeInfo.DataType as CompositiveType;
            if (compositive != null)
            {
                foreach (var member in compositive.TypeMembers)
                {
                    var name = $"{baseName}.{member.Name}";
                    list.Add(name);
                    ParseMember(list, name, member.DataTypeInfo);
                }
            }
        }

        public void SetDataTypeFilter(string filters)
        {
            SetDataTypeCheck(filters);
            ExecuteCommand();
        }

        public override void Cleanup()
        {
            WeakEventManager<Controller, EventArgs>.RemoveHandler(_controller, "Loaded", Controller_Loaded);
            CollectionChangedEventManager.RemoveHandler(FilterDataTypes, FilterDataTypes_CollectionChanged);
            Notifications.DisconnectConsumer(this);

            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        public bool Inputs
        {
            set
            {
                _inputs = value;
                SetTagItems();
            }
            get { return _inputs; }
        }

        public bool InOuts
        {
            set
            {
                _inOuts = value;
                SetTagItems();
            }
            get { return _inOuts; }
        }

        public bool Outputs
        {
            set
            {
                _outputs = value;
                SetTagItems();
            }
            get { return _outputs; }
        }

        public bool Locals
        {
            set
            {
                _locals = value;
                SetTagItems();
            }
            get { return _locals; }
        }

        public Visibility ShowAOI { set; get; }

        public Visibility ShowNormal { set; get; }

        internal void SetScopeName(string name, bool showController = true)
        {
            if (string.IsNullOrEmpty(_controller.Name)) return;
            if (name?.Equals(_parentProgram?.Name) ?? false) return;
            Name = "";
            SelectedTag = null;
            _showController = showController;
            RaisePropertyChanged("ShowController");
            if (!_isCrossReference)
            {
                ProgramCheckBoxEnable = true;
                ControllerCheckBoxEnable = true;
                ShowAOI = Visibility.Collapsed;
                ShowLastRow = Visibility.Visible;
                ShowNormal = Visibility.Visible;
            }

            _isAoi = false;

            var program = _controller.Programs[name];
            if (program != null)
            {
                _parentProgram = program;
                _showProgram = true;
                RaisePropertyChanged("ShowProgram");
                SetOtherProgram();
                SetTagItems();
                return;
            }
            else
            {
                _showProgram = false;
                RaisePropertyChanged("ShowProgram");
            }

            var aoi = (_controller.AOIDefinitionCollection as AoiDefinitionCollection)?.Find(name);
            if (aoi != null)
            {
                _parentProgram = aoi;
                ShowAOI = Visibility.Visible;
                ShowLastRow = Visibility.Collapsed;
                ShowNormal = Visibility.Collapsed;
                Inputs = true;
                InOuts = true;
                Outputs = true;
                Locals = true;
                _isAoi = true;
                SetDataTypeItem();
                SetTagItems();
                return;
            }

            if (!_isCrossReference)
            {
                ProgramCheckBoxEnable = false;
                ShowProgram = false;
                _showController = true;
                RaisePropertyChanged("ShowController");
            }

            _parentProgram = null;

            SetTagItems();
        }

        public RelayCommand DataGridDoubleClickCommand { get; }

        private void ExecuteDataGridDoubleClickCommand()
        {
            RaisePropertyChanged("Hide");
        }

        public bool Hide { get; }

        public string ProgramContent
        {
            get
            {
                var name = _parentProgram == null ? LanguageManager.GetInstance().ConvertSpecifier("FilterProgram") : _parentProgram.Name;
                return _show + $" {name} "+LanguageManager.GetInstance().ConvertSpecifier("Variable");
            }
        }

        public Visibility ShowLastRow { set; get; } = Visibility.Visible;

        public bool ControllerCheckBoxEnable
        {
            set { Set(ref _controllerCheckBoxEnable, value); }
            get { return _controllerCheckBoxEnable; }
        }

        public bool ProgramCheckBoxEnable
        {
            set { Set(ref _programCheckBoxEnable, value); }
            get { return _programCheckBoxEnable; }
        }

        public ObservableCollection<string> OtherProgramList { set; get; } = new ObservableCollection<string>();

        public string SelectedOther
        {
            set
            {
                if (string.CompareOrdinal(_selectedOther, value) != 0)
                {
                    _selectedOther = value;
                    SetTagItems();
                }
            }
            get { return _selectedOther; }
        }

        public TagItem SelectedTag
        {
            set
            {
                Set(ref _selectedTag, value);
                if (_doRename && value != null)
                    Name = value.Name;
            }
            get { return _selectedTag; }
        }

        public ObservableCollection<string> FilterList { set; get; } = new ObservableCollection<string>();

        public string FilterName
        {
            set
            {
                _filterName = value;
                SetTagItems();
            }
            get { return _filterName ?? ""; }
        }

        public ObservableCollection<string> FilterDataTypes { get; } = new ObservableCollection<string>();

        private bool CheckUsage(string usage)
        {
            var usageItems = DataTypeItems.FirstOrDefault(d => d.Name == "Usage");
            if (usageItems != null)
            {
                var item = usageItems.Children.FirstOrDefault(u => u.Name.Equals(usage));
                if (item != null)
                {
                    if (item.CheckType == CheckType.Null) return true;
                    return item.CheckType != CheckType.Null;
                }
            }
            return true;
        }

        private string GetUsage(ITag tag)
        {
            if (tag.ParentCollection.ParentProgram == null) return "<controller>";
            if (tag.Usage == Usage.SharedData) return "Public";
            return tag.Usage.ToString();
        }

        readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);
        private string _show;
        private bool _isCrossReference;

        private void SetTagItems()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await _semaphoreSlim.WaitAsync();
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                TagItems.Clear();
                FilterList.Clear();
                AutoCompleteData.Clear();

                await TaskScheduler.Default;

                if (_isAoi)
                {
                    if (_parentProgram != null)
                        foreach (var tag in _parentProgram.Tags)
                        {
                            if (IsInAoiDataTypes(tag.DataTypeInfo.DataType.Name, tag.Usage))
                            {
                                if (!string.IsNullOrEmpty(FilterName))
                                {
                                    if (tag.Name.StartsWith(FilterName, StringComparison.OrdinalIgnoreCase))
                                    {
                                        await CreateItem(tag);
                                    }
                                }
                                else
                                {
                                    await CreateItem(tag);
                                }
                            }

                        }
                    _semaphoreSlim.Release();
                    return;
                }

                if (ShowProgram && _parentProgram != null)
                {
                    foreach (var tag in _parentProgram.Tags)
                    {
                        if (IsInDataTypes(tag.DataTypeInfo.DataType.Name, ExtendUsage.ToString(tag.Usage)))
                        {
                            if (!string.IsNullOrEmpty(FilterName))
                            {
                                if (tag.Name.StartsWith(FilterName, StringComparison.OrdinalIgnoreCase))
                                {
                                    await CreateItem(tag);
                                }
                            }
                            else
                            {
                                await CreateItem(tag);
                            }
                        }

                    }
                }

                if (ShowController)
                {
                    foreach (var tag in _controller.Tags)
                    {
                        if (IsInDataTypes(tag.DataTypeInfo.DataType.Name, "<controller>"))
                        {
                            if (!string.IsNullOrEmpty(FilterName))
                            {
                                if (tag.Name.StartsWith(FilterName, StringComparison.OrdinalIgnoreCase))
                                {
                                    await CreateItem(tag, false, true);
                                }
                            }
                            else
                            {
                                await CreateItem(tag, false, true);
                            }
                        }
                    }
                }

                if ((!string.IsNullOrEmpty(SelectedOther)) && SelectedOther != "<none>")
                {
                    var program = _controller.Programs[SelectedOther];
                    foreach (var tag in program.Tags)
                    {
                        if (IsInDataTypes(tag.DataTypeInfo.DataType.Name, ExtendUsage.ToString(tag.Usage)))
                        {
                            if (!string.IsNullOrEmpty(FilterName))
                            {
                                if (tag.Name.StartsWith(FilterName, StringComparison.OrdinalIgnoreCase))
                                {
                                    await CreateItem(tag, true);
                                }
                            }
                            else
                            {
                                await CreateItem(tag, true);
                            }
                        }
                    }
                }
                
                _semaphoreSlim.Release();
            });
        }

        private async Task CreateItem(ITag tag, bool isOtherProgram = false, bool isControllerTag = false)
        {
            if (!CheckUsage(GetUsage(tag))) return;

            string name = tag.Name;

            if (isOtherProgram)
            {
                name = $@"\{SelectedOther}.{name}";
            }
            else
            {
                if (IsShowScope && tag.ParentCollection.ParentProgram != null)
                {
                    name = $"\\{tag.ParentCollection.ParentProgram.Name}.{name}";
                }
            }

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (TagItems.ToList().All(t => t.Tag != tag))
            {
                
                var item = new TagItem(TagItems, tag, name);
                AutoCompleteData[tag] = item.NameList;
                FilterList.Add(name);
                item.Name = name;
                item.Scope = tag.ParentCollection.ParentProgram?.Name ?? "";
                item.Description = tag.Description;
                item.DataTypeInfo = tag.DataTypeInfo;
                item.Usage = isControllerTag ? "<controller>" : ExtendUsage.ToString(tag.Usage);
                item.IsControllerTag = isControllerTag;
                TagItems.Add(item);
            }
        }
        
        public bool ShowController
        {
            set
            {
                var changed = _showController != value;
                Set(ref _showController, value);
                if (changed)
                    SetTagItems();
            }
            get { return _showController; }
        }

        public bool ShowProgram
        {
            set
            {
                var changed = _showProgram != value;
                Set(ref _showProgram, value);
                if (changed)
                    SetTagItems();
                RaisePropertyChanged("ProgramContent");
            }
            get { return _showProgram; }
        }

        public FilterOnType SelectedFilterOnType
        {
            set
            {
                _selectedFilterOnType = value;
                //TODO(ZYL):add this condition
            }
            get { return _selectedFilterOnType; }
        }

        //TODO(gjc): use Concurrent Collections???
        public List<DataTypeItem> DataTypeItems { set; get; } = new List<DataTypeItem>();

        public bool NeedGetChild { set; get; } = true;
        public bool NeedRaisePropertyChanged { set; get; } = true;
        public string Name
        {
            set
            {
                _name = value;
                ExtendName();
                _doRename = false;
                SetDataGridFocus();
                _doRename = true;
                if (NeedRaisePropertyChanged)
                    RaisePropertyChanged();
            }
            get { return _name; }
        }

        public void SetOtherSelectedProgram(string name)
        {
            SelectedOther = name;
        }

        //public NameCollection NameList { set; get; }

        public Dictionary<ITag, TagNameNode> AutoCompleteData { get; }

        public Visibility VisibilityCol3
        {
            set
            {
                Set(ref _visibilityCol3, value);
                SizeUpdateEvent?.Invoke(this, new WidthChangedArg(_visibilityCol3 == Visibility.Visible ? 160 : -160));
            }
            get { return _visibilityCol3; }
        }

        public event EventHandler SizeUpdateEvent;

        public ListCollectionView ShowList { set; get; }

        public IList FilterOnList { set; get; }

        public TagItemCollection TagItems { set; get; } = new TagItemCollection();

        public void ResetSelectedShowItem()
        {
            VisibilityCol3 = Visibility.Collapsed;
            var item = (Item)ShowList.GetItemAt(1);
            item.Name = "Configure...";
        }

        public Item SelectedShowItem
        {
            set
            {
                if (_selectedShowItem == value) return;
                if (!_doSetCheckBox)
                {
                    Set(ref _selectedShowItem, value);
                    return;
                }

                bool resetTagItem = true;
                bool isInGroup = true;
                if (value?.Name.Equals("Configure...") ?? false)
                {
                    VisibilityCol3 = Visibility.Visible;
                    var item = (Item)ShowList.GetItemAt(1);
                    item.Name = "Hide Configuration...";
                    isInGroup = false;
                    resetTagItem = false;
                }
                else if (value?.Name.Equals("Hide Configuration...") ?? false)
                {
                    VisibilityCol3 = Visibility.Collapsed;
                    var item = (Item)ShowList.GetItemAt(1);
                    item.Name = "Configure...";
                    isInGroup = false;
                    resetTagItem = false;
                }

                if (value?.Name.Equals("All Variables") ?? false)
                {
                    foreach (var item in DataTypeItems)
                    {
                        if (item.Name == "Usage") continue;
                        item.CheckType = CheckType.Null;
                    }

                    //_doChanged = false;
                    SetAllNull(true);
                    //_doChanged = true;
                    isInGroup = false;
                }

                if (_selectedShowItem != value && isInGroup && value != null)
                {
                    //_doChanged = false;
                    SetAllNull(true);
                    SetDataTypeCheck(value.Name);
                    //_doChanged = true;
                }

                if (resetTagItem)
                {
                    SetTagItems();
                    Name = Name;
                }
                Set(ref _selectedShowItem, value);
            }
            get { return _selectedShowItem; }
        }

        private void FilterDataTypes_CollectionChanged(object sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                var item = (string)e.NewItems[0];
                if (_usageList.Contains(item)) _usages.Add(item);
                else _dataTypes.Add(item);
            }

            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                var item = (string)e.OldItems[0];
                _usages.Remove(item);
                _dataTypes.Remove(item);
            }

        }

        private void SetOtherProgram()
        {
            OtherProgramList.Clear();
            OtherProgramList.Add("<none>");
            foreach (var program in _controller.Programs)
            {
                if (program == _parentProgram) continue;
                OtherProgramList.Add(program.Name);
            }

            SelectedOther = OtherProgramList[0];
        }

        private void SetAllNull(bool isUsageNull)
        {
            foreach (var dataTypeItem in DataTypeItems)
            {
                if (dataTypeItem.Name == "Usage")
                {
                    if (!isUsageNull) continue;
                }

                foreach (var child in dataTypeItem.Children)
                {
                    child.CheckType = CheckType.Null;
                }
            }
        }

        private void SetDataGridFocus()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                var item = TagItems.FirstOrDefault(t => t.Name.Equals(Name));
                SelectedTag = item;
            }
        }

        //private int GetSize(DataTypeInfo dataTypeInfo)
        //{
        //    return Math.Max(1, dataTypeInfo.Dim1) * Math.Max(1, dataTypeInfo.Dim2) * Math.Max(1, dataTypeInfo.Dim3);
        //}
        private void ExtendName()
        {
            try
            {
                if (!string.IsNullOrEmpty(Name))
                {
                    if (Name[Name.Length - 1] == '[')
                    {
                        var item = TagItems.FirstOrDefault(t => t.Name.Equals(Name.Substring(0, Name.Length - 1)));
                        if (item != null)
                            //if (GetSize(item.DataTypeInfo) < 5000)
                        {
                            item.NeedGetChild = NeedGetChild;
                            item.IsExtend = true;
                            item.NeedGetChild = true;
                        }

                        return;
                    }

                    bool isInOtherProgram = Name.IndexOf(@"\") == 0;
                    var names = Name.Split('.');
                    if (isInOtherProgram)
                    {
                        var otherProgram = names[0].Substring(1);
                        var program = _controller.Programs[otherProgram];
                        if (program != null && OtherProgramList.Contains(program.Name))
                            SelectedOther = program.Name;
                    }

                    if (names.Length >= 1)
                    {
                        for (int i = 0; i < names.Length; i++)
                        {
                            if (i == 0 && isInOtherProgram)
                            {
                                continue;
                            }

                            if (names[i].IndexOf("[") > 0)
                            {
                                var nameWithoutDim = GetSplitNameWithoutDim(names, i);
                                var tagItem = TagItems.FirstOrDefault(t =>
                                    t.Name.Equals(nameWithoutDim, StringComparison.OrdinalIgnoreCase));
                                if (tagItem != null)
                                {
                                    //if (GetSize(tagItem.DataTypeInfo) < 5000)
                                    tagItem.NeedGetChild = NeedGetChild;
                                    tagItem.IsExtend = true;
                                    tagItem.NeedGetChild = false;
                                }
                            }

                            var name = GetSplitName(names, i);
                            var item = TagItems.FirstOrDefault(t =>
                                t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                            if (item != null)
                            {
                                //if (GetSize(item.DataTypeInfo) < 5000)
                                item.NeedGetChild = NeedGetChild;
                                item.IsExtend = true;
                                item.NeedGetChild = true;
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                    //else
                    //{
                    //    if (names[0].IndexOf("[") > 0)
                    //    {
                    //        var nameWithoutDim = GetSplitNameWithoutDim(names, 0);
                    //        var tagItem = TagItems.FirstOrDefault(t =>
                    //            t.Name.Equals(nameWithoutDim, StringComparison.OrdinalIgnoreCase));
                    //        if (tagItem != null)
                    //        {
                    //            //if (GetSize(tagItem.DataTypeInfo) < 5000)
                    //            tagItem.NeedGetChild = NeedGetChild;
                    //            tagItem.IsExtend = true;
                    //            tagItem.NeedGetChild = true;
                    //        }
                    //    }
                    //}
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debug.Assert(false,e.Message);
            }
        }

        private void SetDataTypeCheck(string names)
        {
            var s = names.Split(',');
            foreach (var s1 in s)
            {
                var item = DataTypeItems.Select(d =>
                    d.Children.FirstOrDefault(c => c.Name.Equals(s1, StringComparison.OrdinalIgnoreCase))).ToList();
                if (item.Count > 0)
                {
                    foreach (var dataTypeItem in item)
                    {
                        if (dataTypeItem != null)
                        {
                            dataTypeItem.CheckType = CheckType.Half;
                        }
                    }
                }
            }
        }

        private string GetDataTypeCheckName()
        {
            string names = "";
            foreach (var item in _dataTypes)
            {
                if (_usageList.Contains(item)) continue;
                names = names + "," + item;
            }

            if (names.Length > 0)
            {
                names = names.Substring(1);
            }

            return names;
        }

        private string GetSplitName(string[] names, int index)
        {
            string name = "";
            for (int i = 0; i <= index; i++)
            {
                if (name == "")
                    name = names[i];
                else
                    name = name + "." + names[i];
            }

            return name;
        }

        private string GetSplitNameWithoutDim(string[] names, int index)
        {
            string name = "";
            for (int i = 0; i <= index; i++)
            {
                var s = names[i];
                if (i == index && s.IndexOf("[") > 0)
                {
                    s = s.Substring(0, s.IndexOf("["));
                }

                if (name == "")
                    name = s;
                else
                    name = name + "." + s;
            }

            return name;
        }

        private bool IsInDataTypes(string dataType, string usage)
        {
            foreach (var u in _usages)
            {
                _dataTypes.Remove(u);
            }

            if (_usages.Count > 0)
            {
                if (!_usages.Contains(usage)) return false;
                else
                {
                    if (_dataTypes.Count == 0) return true;
                    return _dataTypes.Contains(dataType);
                }
            }
            else
            {
                if (_dataTypes.Count == 0) return true;
                return _dataTypes.Contains(dataType);
            }
        }

        private bool IsInAoiDataTypes(string dataType, Usage usage)
        {
            var usages = new List<Usage>();
            if (Inputs) usages.Add(Usage.Input);
            if (Outputs) usages.Add(Usage.Output);
            if (InOuts) usages.Add(Usage.InOut);
            if (Locals) usages.Add(Usage.Local);
            if (usages.Contains(usage))
            {
                if (_dataTypes.Count == 0) return true;
                if (_dataTypes.Contains(dataType)) return true;
            }

            return false;

        }

        private RelayCommand Command { set; get; }

        private void ExecuteCommand()
        {
            var strings = GetDataTypeCheckName();
            SetFilter(strings);
        }

        private void SetFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                _doSetCheckBox = false;
                SelectedShowItem = (Item)ShowList.GetItemAt(0);
                _doSetCheckBox = true;
                SetTagItems();
                return;
            }

            //ShowList.RemoveAt(2);
            var eItem = _items.FirstOrDefault(i => i.Name.Equals(filter));
            if (eItem != null)
            {
                //var sItem = ShowList.GetItemAt(_items.IndexOf(eItem));
                _doSetCheckBox = false;
                SelectedShowItem = (Item)eItem;
                _doSetCheckBox = true;
                SetTagItems();
                return;
            }

            _doSetCheckBox = false;
            var item = new Item() { Name = filter };
            _items.Add(item);
            ShowList.AddNewItem(item);
            SelectedShowItem = item;
            _doSetCheckBox = true;
            SetTagItems();
        }

        public void SetUsage(string unexpectedUsages)
        {
            if (string.IsNullOrEmpty(unexpectedUsages)) return;
            var usageItems = DataTypeItems.FirstOrDefault(d => d.Name == "Usage");
            foreach (var usage in usageItems.Children)
            {
                if (unexpectedUsages.Contains(usage.Name))
                {
                    usage.CheckType = CheckType.Null;
                }
                else
                {
                    usage.CheckType = CheckType.Half;
                }
            }

            SetTagItems();
        }

        private void SetDataTypeItem()
        {
            bool isFirst = true;
            DataTypeItems.Clear();

            #region UserDefined

            var userDefined = _controller.DataTypes
                .Where(d => d is UserDefinedDataType && d.FamilyType != FamilyType.StringFamily).ToList();
            if (userDefined.Count > 0)
            {
                var userDefinedRoot = new DataTypeItem(FilterDataTypes, Command)
                    { Name = "User-Defined", CanChooseAll = true };
                isFirst = false;
                userDefinedRoot.Index = 1;
                foreach (var dataType in userDefined)
                {
                    var d = new DataTypeItem(FilterDataTypes, Command) { Name = dataType.Name };
                    userDefinedRoot.AddChild(d);
                }

                DataTypeItems.Add(userDefinedRoot);
            }

            #endregion

            #region String

            var stringRoot = new DataTypeItem(FilterDataTypes, Command) { Name = "Strings", CanChooseAll = true };
            if (isFirst)
            {
                stringRoot.Index = 1;
            }
            
            var strings = _controller.DataTypes.Where(d => d.FamilyType == FamilyType.StringFamily);
            foreach (var dataType in strings)
            {
                var s1 = new DataTypeItem(FilterDataTypes, Command) { Name = dataType.Name };
                stringRoot.AddChild(s1);
            }

            DataTypeItems.Add(stringRoot);

            #endregion

            #region Add-On-Defined

            var aois = _controller.AOIDefinitionCollection.ToList();
            if (aois.Count > 0)
            {
                var aoiRoot = new DataTypeItem(FilterDataTypes, Command)
                    { Name = "Add-On-Defined", CanChooseAll = true };
                foreach (var dataType in aois)
                {
                    var d = new DataTypeItem(FilterDataTypes, Command) { Name = dataType.Name };
                    aoiRoot.AddChild(d);
                }

                DataTypeItems.Add(aoiRoot);
            }

            #endregion

            #region Predefined

            var dataTypeList = new List<string>();
            var predefined = _controller.DataTypes.Where(d => d.IsPredefinedType).ToList();
            if (predefined.Count > 0)
            {
                var predefinedRoot = new DataTypeItem(FilterDataTypes, Command)
                    { Name = "Predefined", CanChooseAll = true };
                foreach (var dataType in predefined)
                {
                    if (dataType.FamilyType == FamilyType.StringFamily ||
                        dataType.Name.Equals("string", StringComparison.OrdinalIgnoreCase)) continue;
                    var dataTypeName = dataType.Name;
                    if (dataTypeName.IndexOf(":") > 0)
                    {
                        dataTypeName = dataTypeName.Substring(0, dataTypeName.IndexOf(":"));
                    }

                    if (dataTypeList.Contains(dataTypeName)) continue;
                    dataTypeList.Add(dataTypeName);
                    var d = new DataTypeItem(FilterDataTypes, Command) { Name = dataTypeName };
                    predefinedRoot.AddChild(d);
                }

                DataTypeItems.Add(predefinedRoot);
            }

            #endregion

            #region Module-Defined

            var moduleDefined = _controller.DataTypes.Where(d => d is ModuleDefinedDataType).ToList();
            if (moduleDefined.Count > 0)
            {
                var moduleRoot = new DataTypeItem(FilterDataTypes, Command)
                    { Name = "Module-Defined", CanChooseAll = true };
                foreach (var dataType in moduleDefined)
                {
                    var d = new DataTypeItem(FilterDataTypes, Command) { Name = dataType.Name };
                    moduleRoot.AddChild(d);
                }

                DataTypeItems.Add(moduleRoot);
            }

            #endregion

            if (_isAoi) return;

            #region Usage

            var usageRoot = new DataTypeItem(FilterDataTypes, Command) { Name = "Usage", CanChooseAll = true };
            {
                var local = new DataTypeItem(FilterDataTypes, Command) { Name = ExtendUsage.ToString(Usage.Local) };
                usageRoot.AddChild(local);

                var input = new DataTypeItem(FilterDataTypes, Command) { Name = ExtendUsage.ToString(Usage.Input) };
                usageRoot.AddChild(input);

                var output = new DataTypeItem(FilterDataTypes, Command) { Name = ExtendUsage.ToString(Usage.Output) };
                usageRoot.AddChild(output);

                var inout = new DataTypeItem(FilterDataTypes, Command) { Name = ExtendUsage.ToString(Usage.InOut) };
                usageRoot.AddChild(inout);

                var @public = new DataTypeItem(FilterDataTypes, Command)
                    { Name = ExtendUsage.ToString(Usage.SharedData) };
                usageRoot.AddChild(@public);

                var c = new DataTypeItem(FilterDataTypes, Command) { Name = "<controller>" };
                usageRoot.AddChild(c);

                _usageList.Add("Local");
                _usageList.Add("Input");
                _usageList.Add("Output");
                _usageList.Add("InOut");
                _usageList.Add("Public");
                _usageList.Add("<controller>");
            }
            DataTypeItems.Add(usageRoot);

            #endregion
        }

        public void Consume(MessageData message)
        {
            if (message.Type == MessageData.MessageType.AddTag || message.Type == MessageData.MessageType.DelTag)
            {
                ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    if (message.Type == MessageData.MessageType.AddTag)
                    {
                        var tag = message.Object as ITag;

                        await CreateItem(tag);
                    }
                    else
                    {
                        var tags = message.Object as List<ITag>;
                        if (tags == null) return;
                        var item = TagItems.FirstOrDefault(t => tags.Contains(t.Tag));
                        if (item != null)
                        {
                            item.IsExtend = false;
                            TagItems.Remove(item);
                        }
                    }
                });

            }
        }

    }

    public enum FilterOnType
    {
        [EnumMember(Value = "<all>")] All,
        Unused,
        Produced,
        Consumed,
        [EnumMember(Value = "Can Be Forced")] CanBeForced,
        Alias
    }

    public class Item : INotifyPropertyChanged
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Category { get; set; } = "1";
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TagItem : ViewModelBase
    {
        public TagItem(IList parentCollection, ITag tag,string name)
        {
            Tag = tag;
            ParentCollection = parentCollection;
            Name = name;
            NameList = new TagNameNode(name);
            ExpendCommand = new RelayCommand(ExecuteExpendCommand);
            CheckHasChild();
        }

        public ITag Tag { get; }

        public string Scope { set; get; }
        public bool IsControllerTag { set; get; } = false;
        public IList ParentCollection { set; get; }
        public TagNameNode NameList { set; get; }
        public string Name { set; get; }

        public string DataType => FormatOp.ConvertMemberName(DataTypeInfo.ToString());

        public DataTypeInfo DataTypeInfo
        {
            set
            {
                _dataTypeInfo = value;
                CheckHasChild();
                //SetChild(this, _dataTypeInfo);
            }
            get { return _dataTypeInfo; }
        }

        public string Description { set; get; }
        public string Usage { set; get; }
        public List<TagItem> ChildItems = new List<TagItem>();

        public Visibility ExpanderCloseVis
        {
            set { Set(ref _expanderCloseVis, value); }
            get { return _expanderCloseVis; }
        }

        public Visibility ExpanderVis
        {
            set { Set(ref _expanderVis, value); }
            get { return _expanderVis; }
        }

        public Thickness Left { set; get; }

        public Visibility ShowExpand => (ExpanderCloseVis == Visibility.Visible || ExpanderVis == Visibility.Visible)
            ? Visibility.Visible
            : Visibility.Hidden;

        public RelayCommand ExpendCommand { set; get; }

        private bool _isExtend;
        private Visibility _expanderCloseVis;
        private Visibility _expanderVis;
        private DataTypeInfo _dataTypeInfo;

        public bool IsExtend
        {
            set
            {
                if (_isExtend != value)
                {
                    if (_expanderCloseVis == Visibility.Collapsed && _expanderVis == Visibility.Collapsed)
                    {
                        _isExtend = value;
                        return;
                    }

                    Set(ref _isExtend, value);
                    if (!UpdateCollection())
                    {
                        Set(ref _isExtend, false);
                        return;
                    }

                    if (ExpanderCloseVis == Visibility.Collapsed)
                    {
                        ExpanderCloseVis = Visibility.Visible;
                    }
                    else if (ExpanderCloseVis == Visibility.Visible)
                    {
                        ExpanderCloseVis = Visibility.Collapsed;
                    }
                    else
                    {
                        ExpanderCloseVis = Visibility.Visible;
                    }

                    if (ExpanderVis == Visibility.Collapsed)
                    {
                        ExpanderVis = Visibility.Visible;
                    }
                    else if (ExpanderVis == Visibility.Visible)
                    {
                        ExpanderVis = Visibility.Collapsed;
                    }
                    else
                    {
                        ExpanderVis = Visibility.Visible;
                    }

                }
            }
            get { return _isExtend; }
        }

        public bool NeedGetChild { set; get; } = true;

        private bool UpdateCollection()
        {
            if (ParentCollection != null)
            {
                if (_isExtend)
                {
                    if (ChildItems.Count == 0)
                        SetChild(this, DataTypeInfo);
                    int index = ParentCollection.IndexOf(this) + 1;
                    (ParentCollection as TagItemCollection)?.AddRange(index, ChildItems);
                }
                else
                {
                    foreach (var childItem in ChildItems)
                    {
                        if (childItem.ChildItems.Count > 0)
                        {
                            childItem.IsExtend = false;
                            childItem.ExpanderCloseVis = Visibility.Hidden;
                            childItem.ExpanderVis = Visibility.Visible;
                        }
                    }

                    Remove();
                }

            }

            return true;
        }

        private void Remove()
        {
            (ParentCollection as TagItemCollection)?.RemoveTagItems(ChildItems);
            //foreach (var childItem in ChildItems)
            //{
            //    ParentCollection.Remove(childItem);
            //    childItem.IsExtend = false;
            //}
        }

        private void ExecuteExpendCommand()
        {
            IsExtend = !IsExtend;
        }

        private void CheckHasChild()
        {
            ExpanderCloseVis = Visibility.Collapsed;
            ExpanderVis = Visibility.Collapsed;
            if (DataTypeInfo.DataType == null) return;
            int dim = 0;
            dim = Math.Max(DataTypeInfo.Dim1, 1) * Math.Max(DataTypeInfo.Dim2, 1) * Math.Max(DataTypeInfo.Dim3, 1);

            IDataType dataType = DataTypeInfo.DataType;
            if (DataTypeInfo.Dim1 != 0 || DataTypeInfo.Dim2 != 0 || DataTypeInfo.Dim3 != 0)
            {
                ExpanderCloseVis = Visibility.Collapsed;
                ExpanderVis = Visibility.Visible;

            }
            else if (dataType is CompositiveType)
            {
                ExpanderCloseVis = Visibility.Collapsed;
                ExpanderVis = Visibility.Visible;
            }
            else
            {
                if (dataType is BOOL | dataType is REAL)
                {
                    return;
                }

                if (dim > 0)
                {
                    ExpanderCloseVis = Visibility.Collapsed;
                    ExpanderVis = Visibility.Visible;
                }
                else
                {
                    ExpanderCloseVis = Visibility.Collapsed;
                    ExpanderVis = Visibility.Visible;
                }
            }

            RaisePropertyChanged("ShowExpand");
        }

        private void SetChild(TagItem tagItem, DataTypeInfo dataTypeInfo)
        {
            //int dim = 0;
            //string dataTypeString = dataTypeInfo.DataType.Name;
            //dim = Math.Max(dataTypeInfo.Dim1, 1) * Math.Max(dataTypeInfo.Dim2, 1) * Math.Max(dataTypeInfo.Dim3, 1);

            IDataType dataType = dataTypeInfo.DataType;
            if (dataTypeInfo.Dim1 != 0 || dataTypeInfo.Dim2 != 0 || dataTypeInfo.Dim3 != 0)
            {
                //tagItem.ExpanderCloseVis = Visibility.Collapsed;
                //tagItem.ExpanderVis = Visibility.Visible;

                if (dataTypeInfo.Dim3 > 0)
                {
                    for (int i = 0; i < dataTypeInfo.Dim3; i++)
                    {
                        for (int j = 0; j < dataTypeInfo.Dim2; j++)
                        {
                            for (int k = 0; k < dataTypeInfo.Dim1; k++)
                            {
                                TagItem child = new TagItem(ParentCollection, tagItem.Tag, $"{tagItem.Name}[{i},{j},{k}]")
                                {
                                    //ChildDescription = ChildDescription,,
                                    IsControllerTag = tagItem.IsControllerTag
                                };
                                child.Scope = Scope;
                                //child.DataType = dataTypeString;
                                child.DataTypeInfo = new DataTypeInfo() { DataType = dataTypeInfo.DataType };
                                child.Left = new Thickness(tagItem.Left.Left + 10, 0, 0, 0);
                                //child.Description = GetChildDescription(child.Name);
                                tagItem.ChildItems.Add(child);
                                //SetChild(child, new DataTypeInfo() { DataType = dataType });

                                tagItem.NameList.SubNameNodes.Add(child.NameList);
                            }
                        }
                    }
                }
                else if (dataTypeInfo.Dim2 > 0)
                {
                    for (int j = 0; j < dataTypeInfo.Dim2; j++)
                    {
                        for (int k = 0; k < dataTypeInfo.Dim1; k++)
                        {
                            TagItem child = new TagItem(ParentCollection, tagItem.Tag,$"{tagItem.Name}[{j},{k}]")
                            {
                                //ChildDescription = ChildDescription,
                                IsControllerTag = tagItem.IsControllerTag
                            };
                            child.Scope = Scope;
                            //child.DataType = dataTypeString;
                            child.DataTypeInfo = new DataTypeInfo() { DataType = dataTypeInfo.DataType };
                            child.Left = new Thickness(tagItem.Left.Left + 10, 0, 0, 0);
                            //child.Description = GetChildDescription(child.Name);
                            tagItem.ChildItems.Add(child);
                            //SetChild(child, new DataTypeInfo() { DataType = dataType });
                            tagItem.NameList.SubNameNodes.Add(child.NameList);
                        }
                    }
                }
                else
                {
                    for (int k = 0; k < dataTypeInfo.Dim1; k++)
                    {
                        TagItem child = new TagItem(ParentCollection, tagItem.Tag,
                            $"{tagItem.Name}[{k}]")
                        {
                            //ChildDescription = ChildDescription,
                            IsControllerTag = tagItem.IsControllerTag
                        };
                        child.Scope = Scope;
                        //child.DataType = dataTypeString;
                        child.DataTypeInfo = new DataTypeInfo() { DataType = dataTypeInfo.DataType };
                        child.Left = new Thickness(tagItem.Left.Left + 10, 0, 0, 0);
                        //child.Description = GetChildDescription(child.Name);
                        tagItem.ChildItems.Add(child);
                        //SetChild(child, new DataTypeInfo() { DataType = dataType });
                        tagItem.NameList.SubNameNodes.Add(child.NameList);
                    }

                }

            }
            else if (dataType is CompositiveType)
            {
                //tagItem.ExpanderCloseVis = Visibility.Collapsed;
                //tagItem.ExpanderVis = Visibility.Visible;
                foreach (DataTypeMember itemMember in (dataType as CompositiveType).TypeMembers)
                {
                    if(itemMember.IsHidden)continue;
                    var index = itemMember.FieldIndex;
                    if (dataType is AOIDataType)
                    {
                        if (!(dataType as AOIDataType).IsMemberShowInOtherAoi(itemMember.Name)) continue;
                    }

                    TagItem child = new TagItem(ParentCollection, tagItem.Tag,
                      $"{tagItem.Name}.{itemMember.Name}")
                    {
                        //ChildDescription = ChildDescription,
                        IsControllerTag = tagItem.IsControllerTag
                    };
                    child.Scope = Scope;
                    //child.Description = GetChildDescription(child.Name);
                    child.DataTypeInfo = itemMember.DataTypeInfo;
                    //child.DataType = itemMember.DataTypeInfo.Dim1 > 0
                    //    ? $"{FormatOp.ConvertMemberName(itemMember.DataTypeInfo.DataType.Name)}[{itemMember.DataTypeInfo.Dim1}]"
                    //    : FormatOp.ConvertMemberName(itemMember.DataTypeInfo.DataType.Name);

                    child.Left = new Thickness(tagItem.Left.Left + 10, 0, 0, 0);


                    tagItem.ChildItems.Add(child);
                    tagItem.NameList.SubNameNodes.Add(child.NameList);
                    //SetChild(child, itemMember.DataTypeInfo);
                }

            }
            else
            {
                if (dataType is BOOL | dataType is REAL)
                {
                    return;
                }

                for (int i = 0; i < dataType.BitSize; i++)
                {
                    TagItem child = new TagItem(ParentCollection, tagItem.Tag, $"{tagItem.Name}.{i}")
                    {
                        DataTypeInfo = new DataTypeInfo() { DataType = CreateBoolDataType(dataTypeInfo.DataType) },
                        IsControllerTag = tagItem.IsControllerTag
                    };
                    //child.Description = GetChildDescription(child.Name);
                    child.Left = new Thickness(tagItem.Left.Left + 10, 0, 0, 0);
                    tagItem.ChildItems.Add(child);
                    tagItem.NameList.SubNameNodes.Add(child.NameList);
                }
            }
        }

        private DataType CreateBoolDataType(IDataType dataType)
        {
            if (dataType is SINT) return BOOL.SInst;
            if (dataType is INT) return BOOL.IInst;
            if (dataType is DINT) return BOOL.DInst;
            if (dataType is LINT) return BOOL.LInst;
            return BOOL.Inst;
        }

    }

    public class DataTypeItem : ViewModelBase
    {
        private string _name;
        private readonly IList _filterList;
        private List<DataTypeItem> _children = new List<DataTypeItem>();
        private CheckType _checkType;

        public DataTypeItem(IList filterList, RelayCommand command)
        {
            _filterList = filterList;
            Command = command;

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(Name));
        }

        public string Name
        {
            set { Set(ref _name, value); }
            get { return LanguageManager.GetInstance().ConvertSpecifier(_name); }
        }

        public int Index { set; get; } = 2;

        public CheckType CheckType
        {
            set
            {
                Set(ref _checkType, value);
                if (Children.Count == 0)
                {
                    if (value == CheckType.Half)
                    {
                        if (!_filterList.Contains(Name))
                            _filterList?.Add(Name);
                    }
                    else if (value == CheckType.Null)
                    {
                        _filterList?.Remove(Name);
                    }
                }

                if (DoUpdate)
                    Update();
            }
            get { return _checkType; }
        }

        public RelayCommand Command { set; get; }

        public bool DoUpdate { set; get; } = true;

        public List<DataTypeItem> Children
        {
            set { _children = value; }
            get { return _children; }
        }

        public void AddChild(DataTypeItem child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public DataTypeItem Parent { set; get; }
        public bool CanChooseAll { set; get; }

        private void Update()
        {
            if (CheckType == CheckType.Null)
            {
                if (Children.Count > 0)
                {
                    foreach (var child in Children)
                    {
                        child.DoUpdate = false;
                        child.CheckType = CheckType.Null;
                        child.DoUpdate = true;
                    }
                }
                else if (Parent != null)
                {
                    var checkedItem = Parent.Children.Where(c => c.CheckType == CheckType.Half && c != this).ToList();
                    if (checkedItem.Count > 0)
                    {
                        Parent.DoUpdate = false;
                        Parent.CheckType = CheckType.Half;
                        Parent.DoUpdate = true;
                    }
                    else
                    {
                        Parent.DoUpdate = false;
                        Parent.CheckType = CheckType.Null;
                        Parent.DoUpdate = true;
                    }
                }
            }

            else if (CheckType == CheckType.All)
            {
                if (Children.Count > 0)
                {
                    foreach (var child in Children)
                    {
                        child.DoUpdate = false;
                        child.CheckType = CheckType.Half;
                        child.DoUpdate = true;
                    }
                }
            }

            else
            {
                var uncheckedItem = Parent.Children.FirstOrDefault(c => c.CheckType == CheckType.Null);
                if (uncheckedItem == null)
                {
                    Parent.DoUpdate = false;
                    Parent.CheckType = CheckType.All;
                    Parent.DoUpdate = true;
                }
                else
                {
                    Parent.DoUpdate = false;
                    Parent.CheckType = CheckType.Half;
                    Parent.DoUpdate = true;
                }
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();

            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }
    }

    public class WidthChangedArg : EventArgs
    {
        public WidthChangedArg(double changedSize)
        {
            ChangedSize = changedSize;
        }

        public double ChangedSize { get; }
    }

   
}
