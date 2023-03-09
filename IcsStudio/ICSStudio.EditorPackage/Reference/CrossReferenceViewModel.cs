using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.AvalonEdit.Variable;
using ICSStudio.Components.Controls;
using ICSStudio.Dialogs.Filter;
using ICSStudio.EditorPackage.Reference.Extend;
using ICSStudio.Gui.Utils;
using ICSStudio.Gui.View;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Editor;
using ICSStudio.UIInterfaces.Parser;
using NLog;
using Type = ICSStudio.UIInterfaces.Editor.Type;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.EditorPackage.Reference
{
    public partial class CrossReferenceViewModel : ViewModelBase, IEditorPane
    {
        private int _tagTabControlIndex = 0;
        private bool _tagShowEnable;
        private string _tagScopeContent;
        private Visibility _filterVisibility;
        private bool _scopeEnabled;
        private Visibility _logicTabControlVisibility;
        private Visibility _aoiLogicTabControlVisibility = Visibility.Collapsed;
        private Visibility _tagTabControlVisibility;
        private Visibility _hierarchyTabControlVisibility;
        private Visibility _connectionTabControlVisibility;
        private Type _selectedType;
        private SelectionTextType _scopeType;
        private Visibility _simpleLogicTabControlVisibility = Visibility.Collapsed;
        private string _selectedNameList;
        private string _referencesCount;

        public CrossReferenceViewModel(Type filterType, IProgramModule parentProgram, string name)
        {
            var userControl = new CrossReference();
            NameFilterCommand = new RelayCommand<Button>(ExecuteNameFilterCommand);
            OpenSTCommand = new RelayCommand<RoutedEventArgs>(ExecuteOpenSTCommand);
            OpenMonitorCommand = new RelayCommand<RoutedEventArgs>(ExecuteOpenMonitorCommand);
            Controller = SimpleServices.Common.Controller.GetInstance();
            TypeList = EnumHelper.ToDataSource<Type>();
            Caption = LanguageManager.GetInstance().ConvertSpecifier("Cross Reference");
            Control = userControl;
            Control.DataContext = this;
            RefreshCommand = new RelayCommand(ExecuteRefreshCommand);
            //NameFilterAdorner = new NameFilterAdorner(userControl, controller, parentProgram);
            //NameFilterAdorner.FilterViewModel.PropertyChanged += FilterViewModel_PropertyChanged;
            NameFilterPopup = new NameFilterPopup(parentProgram, null, true);
            NameFilterPopup.FilterViewModel.PropertyChanged += FilterViewModel_PropertyChanged;
            TagShowContent = "Show All";
            _selectedType = filterType;
            ScopeType = SelectionTextType.Scope;
            TagScopeContent = parentProgram == null ? Controller.Name : parentProgram.Name;

            if (filterType == Type.Label)
                LabelProgram = parentProgram.Name;

            if (!string.IsNullOrEmpty(name))
            {
                Name = name;
                ExecuteRefreshCommand();
            }
            ChooseNameControlShow();
            AddListen();
            RefreshReferencesCount(TagTabControlIndex);
        }

        public void AddListen()
        {
            Controller.AOIDefinitionCollection.CollectionChanged += AOIDefinitionCollection_CollectionChanged;
            foreach (var aoi in Controller.AOIDefinitionCollection)
            {
                PropertyChangedEventManager.AddHandler(aoi, OnAoiPropertyChanged, "Name");
            }
        }

        private void AOIDefinitionCollection_CollectionChanged(object sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (var item in e.NewItems)
                {
                    var aoi = (AoiDefinition)item;
                    PropertyChangedEventManager.AddHandler(aoi, OnAoiPropertyChanged, "Name");
                    if (SelectedType == Type.AOI)
                    {
                        ComboboxNameList.Add(aoi.Name);
                    }
                }

            if (e.OldItems != null)
                foreach (var item in e.OldItems)
                {
                    var aoi = (AoiDefinition)item;
                    PropertyChangedEventManager.RemoveHandler(aoi, OnAoiPropertyChanged, "Name");
                    if (SelectedType == Type.AOI)
                    {
                        ComboboxNameList.Remove(aoi.Name);
                    }
                }
        }

        private void OnAoiPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (SelectedType == Type.AOI)
            {
                if (Thread.CurrentThread == Application.Current.Dispatcher.Thread)
                {
                    var aoi = (AoiDefinition)sender;
                    var index = ((AoiDefinitionCollection)Controller.AOIDefinitionCollection).GetIndex(aoi);
                    bool isSelected = SelectedNameList == aoi.OldName;
                    ComboboxNameList[index] = aoi.Name;
                    if (isSelected)
                        SelectedNameList = aoi.Name;
                }
                else
                {
                    ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        var aoi = (AoiDefinition)sender;
                        var index = ((AoiDefinitionCollection)Controller.AOIDefinitionCollection).GetIndex(aoi);
                        bool isSelected = SelectedNameList == aoi.OldName;
                        ComboboxNameList[index] = aoi.Name;
                        if (isSelected)
                            SelectedNameList = aoi.Name;
                    });
                }
            }
        }

        public void RemoveListen()
        {
            Controller.AOIDefinitionCollection.CollectionChanged -= AOIDefinitionCollection_CollectionChanged;
            foreach (var aoi in Controller.AOIDefinitionCollection)
            {
                PropertyChangedEventManager.RemoveHandler(aoi, OnAoiPropertyChanged, "Name");
            }
        }

        public override void Cleanup()
        {
            NameFilterPopup.FilterViewModel.PropertyChanged -= FilterViewModel_PropertyChanged;
            RemoveListen();
        }

        public void UpdateTagSearch(string name, string scope)
        {
            SelectedType = Type.Tag;

            if (!string.IsNullOrEmpty(name))
            {
                TagScopeContent = scope;
                Name = name;
                ExecuteRefreshCommand();
            }
        }

        public void UpdateRoutineSearch(string name, string scope)
        {
            SelectedType = Type.Routine;

            if (string.IsNullOrEmpty(name))
                return;

            TagScopeContent = scope;
            SelectedNameList = name;
            ExecuteRefreshCommand();
        }


        public void UpdateLabelSearch(string name, string scope, string routine)
        {
            SelectedType = Type.Label;

            if (string.IsNullOrEmpty(name))
                return;

            TagScopeContent = routine;
            Name = name;
            SetLabel();
            ScopeType = SelectionTextType.Label;
            SelectedNameList = name;
            LabelProgram = scope;
            ExecuteRefreshCommand();
        }


        public void UpdateAoiInstructionSearch(string name)
        {
            SelectedType = Type.AOI;
            SelectedNameList = name;
            TagScopeContent = "";
            ExecuteRefreshCommand();
        }

        private void RefreshReferencesCount(int value)
        {
            switch (value)
            {
                case 0:
                    ReferencesCount = LogicItems.Count.ToString();
                    break;
                case 1:
                    ReferencesCount = AOILogicItems.Count.ToString();
                    break;
                case 2:
                    ReferencesCount = SimpleLogicItems.Count.ToString();
                    break;
                case 3:
                    ReferencesCount = TagItems.Count.ToString();
                    break;
                case 4:
                    ReferencesCount = "0";
                    break;
                case 5:
                    ReferencesCount = "0";
                    break;
                default:
                    break;
            }
        }

        public Visibility LogicTabControlVisibility
        {
            set { Set(ref _logicTabControlVisibility, value); }
            get { return _logicTabControlVisibility; }
        }

        public Visibility SimpleLogicTabControlVisibility
        {
            set { Set(ref _simpleLogicTabControlVisibility, value); }
            get { return _simpleLogicTabControlVisibility; }
        }

        public Visibility AOILogicTabControlVisibility
        {
            set { Set(ref _aoiLogicTabControlVisibility, value); }
            get { return _aoiLogicTabControlVisibility; }
        }

        public Visibility TagTabControlVisibility
        {
            set { Set(ref _tagTabControlVisibility, value); }
            get { return _tagTabControlVisibility; }
        }

        public Visibility HierarchyTabControlVisibility
        {
            set { Set(ref _hierarchyTabControlVisibility, value); }
            get { return _hierarchyTabControlVisibility; }
        }

        public Visibility ConnectionTabControlVisibility
        {
            set { Set(ref _connectionTabControlVisibility, value); }
            get { return _connectionTabControlVisibility; }
        }

        public SelectionTextType ScopeType
        {
            set { Set(ref _scopeType, value); }
            get { return _scopeType; }
        }

        public bool ScopeEnabled
        {
            set { Set(ref _scopeEnabled, value); }
            get { return _scopeEnabled; }
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

        public RelayCommand<RoutedEventArgs> OpenSTCommand { set; get; }

        private void ExecuteOpenSTCommand(RoutedEventArgs e)
        {
            OpenRoutine();
            e.Handled = true;
        }

        public RelayCommand<RoutedEventArgs> OpenMonitorCommand { set; get; }

        private void ExecuteOpenMonitorCommand(RoutedEventArgs e)
        {
            OpenMonitor();
            e.Handled = true;
        }

        public ObservableCollection<string> ComboboxNameList { get; } = new ObservableCollection<string>();

        public string SelectedNameList
        {
            set { Set(ref _selectedNameList, value); }
            get { return _selectedNameList; }
        }

        public Controller Controller { get; }

        public void OpenMonitor()
        {
            ICreateEditorService createDialogService =
                Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SCreateEditorService)) as
                    ICreateEditorService;
            if (SelectedTagItem != null)
            {
                var program = Controller.Programs[SelectedTagItem.Scope];

                var tagName = SelectedTagItem.Tag;

                createDialogService?.CreateMonitorEditTags(Controller,
                    program != null ? (ITagCollectionContainer)program : Controller, tagName);
            }

            if (SelectedSimpleLogicItem != null)
            {
                var program = Controller.Programs[SelectedSimpleLogicItem.Container];
                if (program == null)
                    return;

                var routine = program.Routines[SelectedSimpleLogicItem.Routine];
                var rll = routine as RLLRoutine;

                if (rll != null)
                    createDialogService?.CreateRLLEditor(rll, SelectedSimpleLogicItem.Line, SelectedSimpleLogicItem.LineOffset);
            }
        }

        public void OpenRoutine()
        {
            ICreateEditorService createDialogService =
                Package.GetGlobalService(typeof(SCreateEditorService)) as
                    ICreateEditorService;

            var selectedItem = SelectedType == Type.AOI ? SelectedAOILogicItem : SelectedLogicItem;

            IProgramModule program = Controller.Programs[selectedItem.Container] ??
                                     (IProgramModule)(Controller.AOIDefinitionCollection as AoiDefinitionCollection)
                                     ?.Find(selectedItem.Container);

            var routine = program?.Routines[selectedItem.Routine];
            Debug.Assert(routine != null, selectedItem.Routine);

            if (routine.Type == RoutineType.ST)
            {
                createDialogService?.CreateSTEditor(routine, selectedItem.OnlineEditType, selectedItem.Line - 1,
                    selectedItem.LineOffset,
                    (SelectedType == Type.AOI ? SelectedNameList.Length : selectedItem.Reference?.Length ?? 0));
            }
            else if (routine.Type == RoutineType.RLL)
            {
                createDialogService?.CreateRLLEditor(routine, selectedItem.Line, selectedItem.LineOffset);
            }
            else
            {
                //TODO(gjc): add other type here
                Debug.Assert(false);
            }

        }

        public string TagScopeContent
        {
            set
            {
                if (_tagScopeContent != value)
                {
                    Set(ref _tagScopeContent, value);
                    NameFilterPopup.ResetScope(IsController ? "" : TagScopeContent, IsController);
                    Name = "";
                    if (SelectedType == Type.Routine)
                    {
                        SetRoutine();
                    }
                }
            }
            get { return _tagScopeContent; }
        }

        public bool IsController { set; get; } = true;

        public string TagShowContent { set; get; }

        public Visibility FilterVisibility
        {
            set
            {
                Set(ref _filterVisibility, value);
                RaisePropertyChanged("NameComboboxVisibility");
            }
            get { return _filterVisibility; }
        }

        public Visibility NameComboboxVisibility
        {
            get { return _filterVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible; }
        }

        public int TagTabControlIndex
        {
            set
            {
                RefreshReferencesCount(value);
                Set(ref _tagTabControlIndex, value);
            }
            get { return _tagTabControlIndex; }
        }

        public bool TagShowEnable
        {
            set { Set(ref _tagShowEnable, value); }
            get { return _tagShowEnable; }
        }

        public string ReferencesCount
        {
            get { return _referencesCount; }
            set { Set(ref _referencesCount, value); }
        }

        private void FilterViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                RaisePropertyChanged("Name");
            }

            if (e.PropertyName == "Hide")
            {
                //var layer = AdornerLayer.GetAdornerLayer(Control);
                //layer?.Remove(NameFilterAdorner);
                NameFilterPopup.IsOpen = false;
            }
        }

        //public NameFilterAdorner NameFilterAdorner { private set; get; }

        public NameFilterPopup NameFilterPopup { get; }

        public RelayCommand RefreshCommand { set; get; }

        public string LabelProgram { get; set; }

        public void ExecuteRefreshCommand()
        {
            try
            {
                ClearTabControl();

                if (SelectedType == Type.Tag)
                {
                    DoTagFilter();
                }
                else if (SelectedType == Type.DataType)
                {
                    DoDataTypeFilter();
                }
                else if (SelectedType == Type.Routine)
                {
                    DoRoutineFilter();
                }
                else if (SelectedType == Type.Program)
                {

                }
                else if (SelectedType == Type.EquipmentPhase || SelectedType == Type.EquipmentSequence)
                {

                }
                else if (SelectedType == Type.AOI)
                {
                    DoAOIFilter();
                }
                else if (SelectedType == Type.Task)
                {

                }
                else if (SelectedType == Type.Module)
                {
                    DoModuleFilter();
                }
                else if (SelectedType == Type.Label)
                {
                    DoLabelFilter();
                }

                RefreshReferencesCount(TagTabControlIndex);
            }
            catch (Exception)
            {
                Logger.Error($"CrossReference.Refresh.Error.Name:{Name}({TagShowContent})");
            }
        }

        internal static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private string _caption;

        public ObservableCollection<LogicItem> LogicItems { get; } = new ObservableCollection<LogicItem>();
        public ObservableCollection<LogicItem> SimpleLogicItems { get; } = new ObservableCollection<LogicItem>();
        public ObservableCollection<LogicItem> AOILogicItems { get; } = new ObservableCollection<LogicItem>();
        public LogicItem SelectedLogicItem { set; get; }
        public LogicItem SelectedSimpleLogicItem { set; get; }
        public LogicItem SelectedAOILogicItem { set; get; }

        public string Name
        {
            set { NameFilterPopup.FilterViewModel.Name = value; }
            get { return NameFilterPopup.FilterViewModel.Name; }
        }

        public Dictionary<ITag, TagNameNode> NameList => NameFilterPopup.FilterViewModel.AutoCompleteData;

        public IList TypeList { set; get; }

        public Type SelectedType
        {
            set
            {
                if (_selectedType != value)
                {
                    _selectedType = value;
                    ChooseNameControlShow();
                }
            }
            get { return _selectedType; }
        }

        public string Caption
        {
            get { return _caption; }
            set
            {
                _caption = value;
                RaisePropertyChanged(Caption);
            }
        }

        public UserControl Control { get; }
        public Action CloseAction { get; set; }
        public Action<string> UpdateCaptionAction { get; set; }

        public ObservableCollection<TagItem> TagItems { get; } = new ObservableCollection<TagItem>();
        public TagItem SelectedTagItem { set; get; }

        #region Filter

        private ITag GetTag()
        {
            if (string.IsNullOrEmpty(Name)) return null;
            var aoi = (Controller.AOIDefinitionCollection as AoiDefinitionCollection)?.Find(TagScopeContent);
            if (aoi != null)
            {
                return ObtainValue.NameToTag(Name, null, aoi)?.Item1;
            }

            return ObtainValue.NameToTag(Name, null, Controller.Programs[TagScopeContent])?.Item1;
        }

        private bool IsMatch(IVariableInfo variable, List<string> filterName)
        {
            if (string.IsNullOrEmpty(variable.Name)) return false;
            var crossReferenceRegex = variable.GetCrossReferenceRegex();
            var crossReferenceParentRegex = variable.GetCrossReferenceParentRegex();
            var regex = new Regex(crossReferenceRegex, RegexOptions.IgnoreCase);
            var regexParent = string.IsNullOrEmpty(crossReferenceParentRegex)
                ? null
                : new Regex($"^{crossReferenceParentRegex}$", RegexOptions.IgnoreCase);
            for (int i = 0; i < filterName.Count; i++)
            {
                var filter = filterName[i];
                if (i == 0)
                {
                    if (regex.IsMatch(filter) || IsMember(variable.Name, filter)) return true;
                    if (regexParent?.IsMatch(filter) ?? false) return true;
                }
                else
                {
                    if (regex.IsMatch(filter)) return true;
                }
            }

            return false;
        }

        private bool IsMatchName(string target, List<string> filterName)
        {
            if (string.IsNullOrEmpty(target)) return false;

            if (filterName.Any(f => f.Equals(target, StringComparison.OrdinalIgnoreCase) ||
                                    (f.Equals(Name, StringComparison.OrdinalIgnoreCase)
                                        ? IsMember(target, f)
                                        : f.Equals(target, StringComparison.OrdinalIgnoreCase))))
                return true;
            return false;
        }

        private bool IsMember(string target, string item)
        {
            if (target.StartsWith(item, StringComparison.OrdinalIgnoreCase))
            {
                Regex regex = new Regex($@"{item.Replace(".", "\\.")}(?!\w)", RegexOptions.IgnoreCase);
                return regex.IsMatch(target);
            }

            return false;
        }

        private void DoTagFilter()
        {
            if (string.IsNullOrEmpty(Name)) return;
            if (ObtainValue.HasVariableInDim(Name)) return;

            var tag = GetTag() as Tag;
            if (tag == null) return;

            CreateTagItem(tag.Name, tag.ParentCollection.ParentProgram?.Name ?? Controller.Name, null);
            var filter = new List<string>() { Name };

            if (tag.ParentCollection.ParentProgram is AoiDefinition)
            {
                var aoi = tag.ParentCollection.ParentProgram;
                foreach (var routine in aoi.Routines)
                {
                    var st = routine as STRoutine;
                    if (st != null)
                    {
                        SearchAndAddLogicItem(st, tag, filter);
                    }
                }

                return;
            }
            else
            {
                int dotIndex = Name.LastIndexOf(".");
                if (dotIndex > -1)
                {
                    var parentMember = GetMemberParent(tag, Name);
                    filter.Add(parentMember);
                    if (parentMember.EndsWith("]"))
                    {
                        var index = parentMember.LastIndexOf("[");
                        if (index > -1)
                            filter.Add(parentMember.Substring(0, index));
                    }

                    var children = GetMemberChildren(tag, Name);
                    if (children != null)
                    {
                        filter.AddRange(children);
                    }

                    if (tag.DataTypeInfo.DataType.IsAxisType)
                    {
                        if (!filter.Contains(tag.Name))
                            filter.Add(tag.Name);
                    }
                }
                else
                {
                    if (Name.EndsWith("]"))
                    {
                        var index = Name.LastIndexOf("[");
                        if (index > -1)
                            filter.Add(Name.Substring(0, index));
                    }
                }

                var count = filter.Count;
                if (!string.IsNullOrEmpty(tag.ParentCollection.ParentProgram?.Name))
                    for (int i = 0; i < count; i++)
                    {
                        filter.Add($"\\{tag.ParentCollection.ParentProgram?.Name}.{filter[i]}");
                    }

                foreach (var program in Controller.Programs)
                {
                    foreach (var r in program.Routines)
                    {
                        var st = r as STRoutine;
                        if (st != null)
                        {
                            SearchAndAddLogicItem(st, tag, filter);
                        }

                        RLLRoutine rll = r as RLLRoutine;
                        if (rll != null)
                        {
                            SearchAndAddLogicItem(rll, tag, Name);
                        }
                    }
                }

                return;
            }
        }

        private void SearchAndAddLogicItem(STRoutine st, Tag tag, List<string> filter)
        {
            {
                var variableInfos = st.GetCurrentVariableInfos(OnlineEditType.Original).ToList().Where(v =>
                    (v.OriginalTag == null ? v.Tag == tag : v.OriginalTag == tag) && IsMatch(v, filter)).ToList();

                foreach (var variableInfo in variableInfos)
                {
                    var description = Tag.GetChildDescription(tag.Description, tag.DataTypeInfo, tag.ChildDescription,
                        Tag.GetOperand(variableInfo.Name));
                    try
                    {
                        var info = variableInfo.GetLocation();
                        AddLogicItem(info.Item1 - 1, info.Item2 - 1,
                            st.CodeText[info.Item1 - 1],
                            st.ParentCollection.ParentProgram.Name, st.Name,
                            CheckIsDestructive(info.Item2 + variableInfo.Name.Length - 1, st.CodeText,
                                info.Item1 - 1),
                            st.Type, OnlineEditType.Original, variableInfo.Name, description);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Logger.Error($"routine({st.Name}--{st.CodeText.Count})");
                    }
                }
            }

            if (st.PendingCodeText != null)
            {
                var variableInfos = st.GetCurrentVariableInfos(OnlineEditType.Pending).ToList().Where(v =>
                    v.Tag == tag && IsMatch(v, filter)).ToList();
                foreach (var variableInfo in variableInfos)
                {
                    var description = Tag.GetChildDescription(tag.Description, tag.DataTypeInfo, tag.ChildDescription,
                        Tag.GetOperand(variableInfo.Name));
                    try
                    {
                        var info = variableInfo.GetLocation();
                        AddLogicItem(info.Item1 - 1, info.Item2 - 1,
                            st.PendingCodeText[info.Item1 - 1],
                            st.ParentCollection.ParentProgram.Name, st.Name,
                            CheckIsDestructive(info.Item2 + variableInfo.Name.Length - 1, st.CodeText,
                                info.Item1 - 1),
                            st.Type, OnlineEditType.Pending, variableInfo.Name, description);
                    }
                    catch (Exception)
                    {
                        Logger.Error($"routine({st.Name}--{st.CodeText.Count})");
                    }
                }
            }

            if (st.TestCodeText != null)
            {
                var variableInfos = st.GetCurrentVariableInfos(OnlineEditType.Test).ToList().Where(v =>
                    v.Tag == tag && IsMatch(v, filter)).ToList();
                foreach (var variableInfo in variableInfos)
                {
                    var description = Tag.GetChildDescription(tag.Description, tag.DataTypeInfo, tag.ChildDescription,
                        Tag.GetOperand(variableInfo.Name));
                    var info = variableInfo.GetLocation();
                    try
                    {
                        AddLogicItem(info.Item1 - 1, info.Item2 - 1,
                            st.TestCodeText[info.Item1 - 1],
                            st.ParentCollection.ParentProgram.Name, st.Name,
                            CheckIsDestructive(info.Item2 + variableInfo.Name.Length - 1, st.CodeText,
                                info.Item1 - 1),
                            st.Type, OnlineEditType.Test, variableInfo.Name, description);
                    }
                    catch (Exception)
                    {
                        Logger.Error($"routine({st.Name}--{st.CodeText.Count})");
                    }
                }
            }
        }

        private string GetMemberParent(ITag tag, string name)
        {
            return ObtainValue.GetParentName(tag.DataTypeInfo.DataType, name.Split('.').ToList(), 0, "");
        }

        private List<string> GetMemberChildren(ITag tag, string name)
        {
            return ObtainValue.GetChildrenName(tag.DataTypeInfo.DataType, name.Split('.').ToList(), 0, "");
        }

        private void DoDataTypeFilter()
        {
            if (!string.IsNullOrEmpty(SelectedNameList))
            {
                var dataType = (AssetDefinedDataType)Controller.DataTypes[SelectedNameList];
                Debug.Assert(dataType != null, SelectedNameList);
                var dataTypes = Controller.DataTypes
                    .Where(d => (d is UserDefinedDataType || d is AOIDataType) &&
                                ((AssetDefinedDataType)d).IsMember(dataType)).ToList();

                var tags = Controller.Tags.Where(t => dataTypes.Contains(t.DataTypeInfo.DataType)).ToList();
                foreach (var program in Controller.Programs)
                {
                    var temp = program.Tags.Where(t => dataTypes.Contains(t.DataTypeInfo.DataType)).ToList();
                    tags.AddRange(temp);
                }

                foreach (var aoi in Controller.AOIDefinitionCollection)
                {
                    var temp = aoi.Tags.Where(t => dataTypes.Contains(t.DataTypeInfo.DataType)).ToList();
                    tags.AddRange(temp);
                }

                foreach (var tag in tags)
                {
                    CreateTagItem(tag.Name, tag.ParentCollection.ParentProgram?.Name ?? Controller.Name,
                        $"{tag.DataTypeInfo.DataType.Name}(Data Type)");
                }
            }
        }

        private void DoRoutineFilter()
        {
            if (string.IsNullOrEmpty(Name)) return;

            var program = Controller.Programs[TagScopeContent];
            if (program == null)
                return;

            foreach (var r in program.Routines)
            {
                var st = r as STRoutine;
                if (st != null)
                {
                    continue;
                }

                RLLRoutine rll = r as RLLRoutine;
                if (rll != null)
                {
                    SearchAndAddRoutineReference(rll, Name);
                }
            }
        }

        private void DoLabelFilter()
        {
            if (string.IsNullOrEmpty(Name)) return;

            if (string.IsNullOrEmpty(LabelProgram)) return;

            var program = Controller.Programs[LabelProgram];
            if (program == null)
                return;

            var routine = program.Routines[TagScopeContent];
            var rll = routine as RLLRoutine;

            if (rll != null)
            {
                SearchAndAddLabelReference(rll, Name);
            }
        }

        private void DoAOIFilter()
        {
            if (string.IsNullOrEmpty(SelectedNameList)) return;
            var aoi = ((AoiDefinitionCollection)Controller.AOIDefinitionCollection).Find(SelectedNameList);
            Debug.Assert(aoi != null);
            if (aoi == null)
            {
                return;
            }

            //var nestedList = GetNested(aoi.datatype);
            foreach (var program in Controller.Programs)
            {
                FilterAOIInRoutine(program, aoi);
            }

            foreach (var aoiDefinition in Controller.AOIDefinitionCollection)
            {
                if (aoi == aoiDefinition) continue;
                FilterAOIInRoutine(aoiDefinition, aoi);
            }
        }

        private void FilterAOIInRoutine(IProgramModule program, AoiDefinition aoi)
        {
            Regex regex = new Regex($@"{SelectedNameList}[ \r\n]*\((.|\r|\n)*\)");
            foreach (var routine in program.Routines)
            {
                var st = routine as STRoutine;
                if (st != null)
                {
                    var originalInfos = st.GetCurrentVariableInfos(OnlineEditType.Original).ToList().Where(v =>
                        v.IsAOI && aoi.Name.Equals(v.Name, StringComparison.OrdinalIgnoreCase));
                    foreach (var originalInfo in originalInfos)
                    {
                        var variable = (VariableInfo)originalInfo;
                        var node = variable.Parameters.nodes[0];
                        var astName = (ASTName)(node is ASTNameAddr ? ((ASTNameAddr)node).name : node);
                        var name = ObtainValue.GetAstName(astName);
                        var tag = ObtainValue.NameToTag(name, null,
                            program)?.Item1 as Tag;
                        if (tag == null) continue;
                        var position = originalInfo.GetLocation();
                        AddAOILogicItem(position.Item1, position.Item2 - 1, program.Name, st.Name, name, st.Type,
                            Tag.GetChildDescription(tag.Description, tag.DataTypeInfo, tag.ChildDescription,
                                Tag.GetOperand(name)));
                    }

                    continue;
                }

                var rll = routine as RLLRoutine;
                if (rll != null)
                {
                    var codeText = string.Join("\n", rll.CodeText);
                    var matches = regex.Matches(codeText);
                    foreach (Match match in matches)
                    {
                        int line = GetLine(match.Index, codeText);
                        Regex regex2 = new Regex(@"(?<=\([ \r\n]*)(\w*)(?=[ \r\n]*\))");
                        var tag = regex2.Match(match.Value).Value;
                        AddAOILogicItem(line, match.Index, program.Name, rll.Name, tag, rll.Type, "");
                    }
                }

                var fbd = routine as FBDRoutine;
                if (fbd != null)
                {
                    //TODO(zyl):filter fbd
                }
            }
        }

        private bool IsDataTypeNested(IDataType target, IDataType source)
        {
            if (target.Equal(source)) return true;
            var compositiveType = target as CompositiveType;
            if (compositiveType != null)
            {
                foreach (var member in compositiveType.TypeMembers)
                {
                    if (IsDataTypeNested(member.DataTypeInfo.DataType, source)) return true;
                }
            }

            return false;
        }

        private List<IDataType> GetNested(IDataType target)
        {
            var nestedList = new List<IDataType>();

            foreach (var controllerDataType in Controller.DataTypes)
            {
                if (controllerDataType.IsPredefinedType) continue;
                if (IsDataTypeNested(controllerDataType, target))
                {
                    nestedList.Add(controllerDataType);
                }
            }

            return nestedList;
        }

        private void DoModuleFilter()
        {
            var module = Controller.DeviceModules[SelectedNameList];
            if (module != null)
            {
                var discreteIO = module as DiscreteIO;
                if (discreteIO != null)
                {
                    var o = discreteIO.OutputTag;
                    if (o != null)
                        CreateTagItem(o.Name, Controller.Name, $"{discreteIO.Name}(Module)");
                    var i = discreteIO.InputTag;
                    if (i != null)
                        CreateTagItem(i.Name, Controller.Name, $"{discreteIO.Name}(Module)");
                    var c = discreteIO.ConfigTag;
                    if (c != null)
                        CreateTagItem(c.Name, Controller.Name, $"{discreteIO.Name}(Module)");
                }

                var driver = module as CIPMotionDrive;
                if (driver != null)
                {
                    //TODO(zyl):get tag
                }

                var communications = module as CommunicationsAdapter;
                if (communications != null)
                {
                    var o = communications.OutputTag;
                    if (o != null)
                        CreateTagItem(o.Name, Controller.Name, $"{communications.Name}(Module)");
                    var i = communications.InputTag;
                    if (i != null)
                        CreateTagItem(i.Name, Controller.Name, $"{communications.Name}(Module)");
                    var children = Controller.DeviceModules
                        .Where(d => (d as DiscreteIO)?.ParentModule == communications).ToList();
                    foreach (DiscreteIO child in children)
                    {
                        var output = child.OutputTag;
                        if (output != null)
                            CreateTagItem(output.Name, Controller.Name, $"{communications.Name}(Module)");
                        var input = child.InputTag;
                        if (input != null)
                            CreateTagItem(input.Name, Controller.Name, $"{communications.Name}(Module)");
                        var config = child.ConfigTag;
                        if (config != null)
                            CreateTagItem(config.Name, Controller.Name, $"{communications.Name}(Module)");
                    }
                }
            }
        }

        #endregion

        private void ClearTabControl()
        {
            AOILogicItems.Clear();
            LogicItems.Clear();
            SimpleLogicItems.Clear();
            TagItems.Clear();
        }

        private int GetLine(int offset, string codeText)
        {
            if (offset < 0) return 0;
            string temp = codeText.Substring(0, offset);
            Regex regex = new Regex(@"\n");
            return regex.Matches(temp).Count;
        }

        private void CreateTagItem(string tagName, string scope, string aliasFor)
        {
            var item = new TagItem();
            item.Scope = scope;
            item.Tag = tagName;
            item.BaseTag = tagName;
            item.AliasFor = aliasFor;
            TagItems.Add(item);
        }

        private string FormatName(string originName)
        {
            if (string.IsNullOrEmpty(originName)) return originName;
            string name = originName;
            int flag = name.IndexOf("[");
            while (flag != -1)
            {
                name = name.Insert(flag, "\\");
                flag = name.IndexOf("[", flag + 2);
            }

            flag = name.IndexOf("]");
            while (flag != -1)
            {
                name = name.Insert(flag, "\\");
                flag = name.IndexOf("]", flag + 2);
            }

            flag = name.IndexOf(".");
            while (flag != -1)
            {
                name = name.Insert(flag, "\\");
                flag = name.IndexOf(".", flag + 2);
            }

            return name;
        }

        private void SearchTagInfo(IProgram program, IProgram targetProgram, bool isOtherProgram = false)
        {
            string name = FormatName(Name);
            FindLocation(name, program, targetProgram, isOtherProgram);
            Regex regex = null;
            regex = new Regex(@"\.[0-9]+$");
            var matched = regex.Match(name);
            if (matched.Success)
            {
                string parentName = name.Substring(0, matched.Index - 1);
                FindLocation(parentName, program, targetProgram, isOtherProgram);
            }

        }

        private void FindLocation(string name, IProgram program, IProgram targetProgram, bool isOtherProgram = false)
        {
            Regex regex = null;
            if (targetProgram == null)
            {
                regex = new Regex($@"(?<![\w\.#]){name}(?![\w])");
            }
            else
            {
                regex = isOtherProgram
                    ? new Regex($@"(?<![\w\.#])(\\{targetProgram.Name}.{name})(?![\w])")
                    : new Regex($@"(?<![\w\.#])((\\{targetProgram.Name}.)?{name})(?![\w])");
            }

            foreach (var routine in program.Routines)
            {
                var st = routine as STRoutine;
                if (st != null)
                {
                    int lineCount = 0;
                    foreach (var code in st.CodeText)
                    {
                        var newCode = ConvertCommentToWhiteBlank(code);
                        var matches = regex.Matches(newCode);
                        foreach (Match match in matches)
                        {
                            if (LogicItems.Any(l =>
                                    l.Routine == st.Name && l.Line == lineCount && l.LineOffset == match.Index &&
                                    l.OnlineEditType == OnlineEditType.Original)) continue;
                            AddLogicItem(match, lineCount, code, program.Name, st.Name,
                                CheckIsDestructive(match, st.CodeText, lineCount), st.Type, OnlineEditType.Original);
                        }

                        lineCount++;
                    }

                    if (st.PendingCodeText != null)
                    {
                        lineCount = 0;
                        foreach (var code in st.PendingCodeText)
                        {
                            var newCode = ConvertCommentToWhiteBlank(code);
                            var matches = regex.Matches(newCode);
                            foreach (Match match in matches)
                            {
                                if (LogicItems.Any(l =>
                                        l.Routine == st.Name && l.Line == lineCount && l.LineOffset == match.Index &&
                                        l.OnlineEditType == OnlineEditType.Pending)) continue;
                                AddLogicItem(match, lineCount, code, program.Name, st.Name,
                                    CheckIsDestructive(match, st.CodeText, lineCount), st.Type, OnlineEditType.Pending);
                            }

                            lineCount++;
                        }
                    }

                    if (st.TestCodeText != null)
                    {
                        lineCount = 0;
                        foreach (var code in st.TestCodeText)
                        {
                            var newCode = ConvertCommentToWhiteBlank(code);
                            var matches = regex.Matches(newCode);
                            foreach (Match match in matches)
                            {
                                if (LogicItems.Any(l =>
                                        l.Routine == st.Name && l.Line == lineCount && l.LineOffset == match.Index &&
                                        l.OnlineEditType == OnlineEditType.Test)) continue;
                                AddLogicItem(match, lineCount, code, program.Name, st.Name,
                                    CheckIsDestructive(match, st.CodeText, lineCount), st.Type, OnlineEditType.Test);
                            }

                            lineCount++;
                        }
                    }
                }
            }
        }

        private string ConvertCommentToWhiteBlank(string code)
        {
            code = RemoveRegion(code);
            string newCode = "";
            int p = code.IndexOf("(*");
            if (p != -1)
            {
                while (p != -1)
                {
                    int p2 = code.IndexOf("*)", p) + 1;
                    if (p2 == 0) p2 = code.Length - 1;

                    newCode += code.Substring(newCode.Length, p - newCode.Length);
                    int length = p2 - p + 1;
                    string space = "";
                    while (length > 0)
                    {
                        space = space + " ";
                        length--;
                    }

                    newCode = newCode + space;

                    p = code.IndexOf("(*", p2 + 1);
                }

                if (newCode.Length != code.Length)
                {
                    newCode = newCode + code.Substring(newCode.Length);
                }
            }
            else
                newCode = code;

            string code2 = newCode;
            newCode = "";
            p = code2.IndexOf("/*");
            if (p != -1)
            {
                while (p != -1)
                {
                    int p2 = code2.IndexOf("*/", p) + 1;
                    if (p2 == 0) p2 = code2.Length - 1;

                    newCode += code2.Substring(newCode.Length, p - newCode.Length);
                    int length = p2 - p + 1;
                    string space = "";
                    while (length > 0)
                    {
                        space = space + " ";
                        length--;
                    }

                    newCode = newCode + space;

                    p = code2.IndexOf("/*", p2 + 1);
                }

                if (newCode.Length != code2.Length)
                {
                    newCode = newCode + code2.Substring(newCode.Length);
                }
            }
            else
                newCode = code2;

            p = newCode.IndexOf("//");
            if (p != -1)
            {
                while (p != -1)
                {
                    int p2 = code.IndexOf("\n", p);
                    if (p2 == -1) p2 = newCode.Length - 1;
                    int length = p2 - p + 1;
                    newCode = newCode.Remove(p, length);
                    string space = "";
                    while (length > 0)
                    {
                        space = space + " ";
                        length--;
                    }

                    newCode = newCode.Insert(p, space);
                    p = newCode.IndexOf("//");
                }

                if (newCode.Length != code.Length) newCode = newCode + code.Length;
            }

            return newCode;
        }

        private string RemoveRegion(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            Regex regex = new Regex("((^|\n)( )*(#region )).*", RegexOptions.IgnoreCase);
            Regex endRegex = new Regex("(^|\n)( )*(#endregion)(( )+.*)?", RegexOptions.IgnoreCase);
            var matches = regex.Matches(text);
            var endMatches = endRegex.Matches(text);
            foreach (Match match in matches)
            {
                var blank = GetBlank(match.Length);
                text = text.Remove(match.Index, match.Length).Insert(match.Index, blank);
            }

            foreach (Match match in endMatches)
            {
                var blank = GetBlank(match.Length);
                text = text.Remove(match.Index, match.Length).Insert(match.Index, blank);
            }

            return text;
        }

        private string GetBlank(int length)
        {
            string blank = "";
            if (length > -1)
            {
                while (length > 0)
                {
                    blank = blank + " ";
                    length--;
                }
            }

            return blank;
        }

        private void AddLogicItem(int lineCount, int offset, string lineContent, string container, string routine,
            bool isDestructive, RoutineType routineType, OnlineEditType onlineEditType, string reference,
            string description)
        {
            var item = new LogicItem(lineCount + 1, offset, isDestructive, routineType, onlineEditType);
            item.Element = lineContent.Trim();
            item.Container = container;
            item.Routine = routine;
            item.Reference = reference;
            item.Description = description;

            var logicItem = LogicItems.FirstOrDefault(l =>
                l.Container == container && l.Line >= lineCount && l.LineOffset >= offset);
            if (logicItem != null)
            {
                var index = LogicItems.IndexOf(logicItem);
                LogicItems.Insert(index, item);
            }
            else
                LogicItems.Add(item);
        }

        private void AddLogicItem(ObservableCollection<LogicItem> collection, int lineCount, int offset, string lineContent, string container, string routine,
            bool isDestructive, RoutineType routineType, OnlineEditType onlineEditType, string reference,
            string description)
        {
            var item = new LogicItem(lineCount + 1, offset, isDestructive, routineType, onlineEditType);
            item.Element = lineContent.Trim();
            item.Container = container;
            item.Routine = routine;
            item.Reference = reference;
            item.Description = description;

            var logicItem = collection.FirstOrDefault(l =>
                l.Container == container && l.Line >= lineCount && l.LineOffset >= offset);
            if (logicItem != null)
            {
                var index = collection.IndexOf(logicItem);
                collection.Insert(index, item);
            }
            else
                collection.Add(item);
        }

        private int GetColumn(List<string> codeText, int offset, int line)
        {
            var codeTextArray = codeText.ToArray();

            int count = codeTextArray.Length;

            if (line < count)
                count = line;

            var text = string.Join("\n", codeText.ToArray(), 0, count);

            return offset - text.Length;
        }

        private void AddLogicItem(Match match, int lineCount, string lineContent, string container, string routine,
            bool isDestructive, RoutineType routineType, OnlineEditType onlineEditType)
        {
            var item = new LogicItem(lineCount + 1, match.Index, isDestructive, routineType, onlineEditType);
            item.Element = lineContent;
            item.Container = container;
            item.Routine = routine;
            item.Reference = Name;
            LogicItems.Add(item);
        }

        private void AddAOILogicItem(int line, int offset, string container, string routine, string tag,
            RoutineType routineType, string description)
        {
            var item = new LogicItem(line, offset , false, routineType, OnlineEditType.Original);
            item.Container = container;
            item.Routine = routine;
            item.BaseTag = tag;
            item.Description = description;
            var logicItem = AOILogicItems.FirstOrDefault(l =>
                l.Container == container && l.Line >= line && l.LineOffset >= offset);
            if (logicItem != null)
            {
                var index = AOILogicItems.IndexOf(logicItem);
                AOILogicItems.Insert(index, item);
            }
            else
                AOILogicItems.Add(item);
        }

        private bool CheckIsDestructive(Match match, List<string> codeText, int line)
        {
            while (line < codeText.Count)
            {
                var code = "";
                if (line < codeText.Count) code = ConvertCommentToWhiteBlank(codeText[line]);
                Stack<char> stack = new Stack<char>();
                for (int i = match.Index + match.Length; i < code.Length; i++)
                {
                    if (code[i] == ':')
                    {
                        if (i + 1 < code.Length)
                        {
                            if (code[i + 1] == '=') return true;
                        }

                        return false;
                    }

                    if (code[i] == ' ' || code[i] == '\r' || code[i] == '\n' || char.IsLetter(code[i]) ||
                        code[i] == '.') continue;
                    if (code[i] == '[')
                    {
                        stack.Push(code[i]);
                        continue;
                    }

                    if (code[i] == ']')
                    {
                        if (stack.Count == 0) return false;
                        stack.Pop();
                        continue;
                    }

                    return false;
                }

                line++;
            }

            return false;
        }

        private bool CheckIsDestructive(int end, List<string> codeText, int line)
        {
            while (line < codeText.Count)
            {
                var code = "";
                if (line < codeText.Count) code = ConvertCommentToWhiteBlank(codeText[line]);
                Stack<char> stack = new Stack<char>();
                for (int i = end; i < code.Length; i++)
                {
                    if (code[i] == ':')
                    {
                        if (i + 1 < code.Length)
                        {
                            if (code[i + 1] == '=') return true;
                        }

                        return false;
                    }

                    if (code[i] == ' ' || code[i] == '\r' || code[i] == '\n' || char.IsLetter(code[i]) ||
                        code[i] == '.') continue;
                    if (code[i] == '[')
                    {
                        stack.Push(code[i]);
                        continue;
                    }

                    if (code[i] == ']')
                    {
                        if (stack.Count == 0) return false;
                        stack.Pop();
                        continue;
                    }

                    return false;
                }

                line++;
            }

            return false;
        }

        private void ChooseNameControlShow()
        {
            FilterVisibility = Visibility.Collapsed;
            ScopeEnabled = false;
            LogicTabControlVisibility = Visibility.Collapsed;
            SimpleLogicTabControlVisibility = Visibility.Collapsed;
            AOILogicTabControlVisibility = Visibility.Collapsed;
            TagTabControlVisibility = Visibility.Collapsed;
            HierarchyTabControlVisibility = Visibility.Collapsed;
            ConnectionTabControlVisibility = Visibility.Collapsed;
            ClearTabControl();
            ComboboxNameList.Clear();
            TagScopeContent = "";
            if (SelectedType == Type.Tag)
            {
                TagTabControlIndex = 0;
                FilterVisibility = Visibility.Visible;
                TagScopeContent = Controller.Name;
                ScopeEnabled = true;
                TagScopeContent = Controller.Name;
                LogicTabControlVisibility = Visibility.Visible;
                TagTabControlVisibility = Visibility.Visible;
                HierarchyTabControlVisibility = Visibility.Visible;
                ConnectionTabControlVisibility = Visibility.Visible;
            }
            else if (SelectedType == Type.DataType)
            {
                TagScopeContent = "";
                TagTabControlIndex = 3;
                var userDefineDataTypes = Controller.DataTypes.Where(d => d is UserDefinedDataType || d is AOIDataType)
                    .ToList();
                foreach (var dataType in userDefineDataTypes)
                {
                    ComboboxNameList.Add(dataType.Name);
                }

                TagTabControlVisibility = Visibility.Visible;
            }
            else if (SelectedType == Type.Routine)
            {
                SimpleLogicTabControlVisibility = Visibility.Visible;
                TagTabControlIndex = 2;
                SetRoutine();
                ScopeEnabled = true;
            }
            else if (SelectedType == Type.Program)
            {
                TagTabControlIndex = 2;
                TagScopeContent = "";
                SimpleLogicTabControlVisibility = Visibility.Visible;
                foreach (var program in Controller.Programs)
                {
                    ComboboxNameList.Add(program.Name);
                }
            }
            else if (SelectedType == Type.EquipmentPhase || SelectedType == Type.EquipmentSequence)
            {
                TagTabControlIndex = 2;
                TagScopeContent = "";
                SimpleLogicTabControlVisibility = Visibility.Visible;
            }
            else if (SelectedType == Type.AOI)
            {
                TagTabControlIndex = 1;
                TagScopeContent = "";
                AOILogicTabControlVisibility = Visibility.Visible;
                foreach (var aoi in Controller.AOIDefinitionCollection.OrderBy(a => a.Name))
                {
                    ComboboxNameList.Add(aoi.Name);
                }
            }
            else if (SelectedType == Type.Task)
            {
                TagTabControlIndex = 2;
                TagScopeContent = "";
                SimpleLogicTabControlVisibility = Visibility.Visible;
                foreach (var controllerTask in Controller.Tasks)
                {
                    ComboboxNameList.Add(controllerTask.Name);
                }
            }
            else if (SelectedType == Type.Module)
            {
                TagTabControlIndex = 2;
                TagScopeContent = "";
                SimpleLogicTabControlVisibility = Visibility.Visible;
                TagTabControlVisibility = Visibility.Visible;
                ConnectionTabControlVisibility = Visibility.Visible;
                foreach (var module in Controller.DeviceModules)
                {
                    if (module is LocalModule) continue;
                    ComboboxNameList.Add(module.Name);
                }
            }
            else if (SelectedType == Type.Label)
            {
                SimpleLogicTabControlVisibility = Visibility.Visible;
                ScopeEnabled = true;
                TagTabControlIndex = 2;
                ScopeType = SelectionTextType.Label;
                SetLabel();
                TagScopeContent = "";
                return;
            }

            ScopeType = SelectionTextType.Scope;
        }

        private void SetRoutine()
        {
            ComboboxNameList.Clear();
            if (TagScopeContent != null)
            {
                var program = Controller.Programs[TagScopeContent];
                if (program != null)
                {
                    foreach (var routine in program.Routines)
                    {
                        ComboboxNameList.Add(routine.Name);
                    }
                }
                else
                {
                    var aoi = (Controller.AOIDefinitionCollection as AoiDefinitionCollection)?.Find(TagScopeContent);
                    if (aoi != null)
                    {
                        foreach (var routine in aoi.Routines)
                        {
                            ComboboxNameList.Add(routine.Name);
                        }
                    }
                }
            }
        }

        private void SetLabel()
        {
            ComboboxNameList.Clear();

            if (string.IsNullOrEmpty(LabelProgram)) return;

            var program = Controller.Programs[LabelProgram];
            if (program == null)
                return;

            var routine = program.Routines[TagScopeContent];
            var rll = routine as RLLRoutine;

            if (rll == null)
                return;

            var parserService =
                Package.GetGlobalService(typeof(SParserService)) as IParserService;

            var parseInformation = parserService?.GetCachedParseInformation(rll);

            if (parseInformation == null)
                return;

            foreach (var instruction in parseInformation.Instructions.OrderBy(p => p.Name))
            {
                if (!instruction.Name.Equals("LBL", StringComparison.OrdinalIgnoreCase) &&
                    !instruction.Name.Equals("JMP", StringComparison.OrdinalIgnoreCase))
                    continue;

                var label = instruction.Parameters?[0]?.Name;
                if (!ComboboxNameList.Contains(label))
                    ComboboxNameList.Add(label);
            }

            //ComboboxNameList = new ObservableCollection<string>(ComboboxNameList.OrderBy(p => p));
        }
    }

    public class LogicItem
    {
        private readonly int _line;
        private readonly bool _isDestructive;
        private readonly RoutineType _routineType;

        public LogicItem(int line, int lineOffset, bool isDestructive, RoutineType routineType,
            OnlineEditType onlineEditType)
        {
            _line = line;
            _routineType = routineType;
            LineOffset = lineOffset;
            _isDestructive = isDestructive;
            OnlineEditType = onlineEditType;
        }

        public OnlineEditType OnlineEditType { set; get; }
        public string Element { set; get; }
        public string Container { set; get; }
        public string Routine { set; get; }

        public string Location
        {
            get
            {
                if (_routineType == RoutineType.RLL)
                {
                    return $"Rung {_line}";
                }

                if (_routineType == RoutineType.FBD)
                {
                    return $"Sheet 0,C0";
                }

                return $"({OnlineEditType})Line {_line}";
            }
        }

        public int LineOffset { get; }

        public int Line => _line;
        public string Reference { set; get; }
        public string BaseTag { set; get; }

        public string Description { set; get; }

        public string Destructive => _isDestructive ? "Y" : "N";

    }

    public class TagItem
    {
        public string Scope { set; get; }
        public string Tag { set; get; }
        public string AliasFor { set; get; }
        public string BaseTag { set; get; }
    }

    public class ConnectionItem
    {
        public string Reference { set; get; }
        public string EndPoint { set; get; }
        public string Usage { set; get; }
    }
}
