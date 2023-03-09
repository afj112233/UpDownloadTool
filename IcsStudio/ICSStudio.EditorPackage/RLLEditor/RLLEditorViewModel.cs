using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Notification;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.Ladder.Controls;
using ICSStudio.Ladder.Graph;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.Tags;
using Microsoft.VisualStudio.Shell;
using ICSStudio.UIInterfaces.Editor;
using Microsoft.VisualStudio.Threading;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Shapes;
using ICSStudio.Dialogs.ConfigDialogs;
using ICSStudio.EditorPackage.Extension;
using ICSStudio.Gui.Utils;
using ICSStudio.Ladder.Utils;
using ICSStudio.SimpleServices.CodeAnalysis;
using ICSStudio.SimpleServices.Instruction;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIInterfaces.Error;
using ICSStudio.Utils.TagExpression;
using Microsoft.VisualStudio.Shell.Interop;
using ICSStudio.Dialogs.NewTag;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Ladder;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.SimpleServices.Online;
using ControllerKeySwitch = ICSStudio.Interfaces.Common.ControllerKeySwitch;
using ICSStudio.Ladder.Graph.Styles;
using ICSStudio.UIInterfaces.ControllerOrganizer;
using ICSStudio.UIInterfaces.GlobalClipboard;
using ICSStudio.UIInterfaces.Project;
using InstructionDefinition = ICSStudio.Ladder.Graph.InstructionDefinition;
using InstructionParameter = ICSStudio.Ladder.Graph.InstructionParameter;
using Type = ICSStudio.UIInterfaces.Editor.Type;
using ICSStudio.MultiLanguage;

//using Newtonsoft.Json;

namespace ICSStudio.EditorPackage.RLLEditor
{
    public class RLLEditorViewModel : ViewModelBase, IEditorPane, ICanApply, IConsumer, IGlobalClipboard
    {
        private readonly Controller _controller;
        private readonly IDataServer _dataServer;
        private readonly ITagCollectionContainer _globalContainer;
        private readonly ITagCollectionContainer _localContainer;

        private DispatcherTimer _timer;

        private readonly Grid _mainGrid;
        private readonly BrowseAdorner _browseAdorner;
        public BrowseAdorner BrowseAdorner => _browseAdorner;
        private readonly InputAdorner _inputAdorner;
        public InputAdorner InputAdorner => _inputAdorner;
        private readonly BrowseEnumAdorner _enumAdorner;
        public BrowseEnumAdorner EnumAdorner => _enumAdorner;

        //private string _referenceInstance;
        private bool _isAOIRefeferencePage;

        private Visibility _onlineEditToolBarVisible;
        public Visibility OnlineEditToolBarVisible
        {
            get { return _onlineEditToolBarVisible; }
            set { Set(ref _onlineEditToolBarVisible, value); }
        }

        private bool _onlineEditToolBarEnable;
        public bool OnlineEditToolBarEnable
        {
            get { return _onlineEditToolBarEnable; }
            set { Set(ref _onlineEditToolBarEnable, value); }

        }

        private bool _isDirty;

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    if (value)
                    {
                        if (!Caption.EndsWith("*"))
                        {
                            Caption = $"{Caption}*";
                        }
                        SetProjectDirty();
                    }
                    else
                    {
                        if (Caption.EndsWith("*"))
                        {
                            Caption = Caption.Substring(0, Caption.Length - 1);
                        }
                    }
                }
            }
        }

        public RLLEditorViewModel(IRoutine routine, UserControl userControl)
        {
            var control = userControl as RLLEditorControl;
            if (control != null)
                _mainGrid = control.MainGrid;
            Routine = routine as RLLRoutine;
            Contract.Assert(Routine != null);

            _controller = Routine?.ParentController as Controller;
            Contract.Assert(_controller != null);

            var aoi = Routine?.ParentCollection.ParentProgram as AoiDefinition;
            if (aoi != null)
            {
                AoiVisibility = Visibility.Visible;
                //CollectionChangedEventManager.AddHandler(aoi.References, References_CollectionChanged);
                Reference.Add(new AoiDataReference(null, null, null, OnlineEditType.Original, $"{aoi.Name} <definition>"));
                foreach (var reference in aoi.References)
                {
                    if (reference?.ParamList == null) continue;
                    AddReference(reference);
                }

                MarkReference();
                SelectedReference = Reference[0];
                PropertyChangedEventManager.AddHandler(aoi, Aoi_PropertyChanged, "IsSealed");
            }

            var bindings = BoxInstructionStyleRenderer.Bindings;
            var controllerName = _controller?.Name;
            if (controllerName != null && !bindings.ContainsKey(controllerName))
                bindings[controllerName] = new Dictionary<string, BindingSource>();

            var scope = Routine?.ParentCollection?.ParentProgram?.Name;
            if (scope != null && !bindings.ContainsKey(scope))
                bindings[scope] = new Dictionary<string, BindingSource>();

            ShowLdtEditorCommand = new RelayCommand(ExecuteShowLdtEditor);
            FinishLdtEdit = new RelayCommand<string>(ExecuteFinishLdtEdit);
            PasteCommand = new RelayCommand(ExecutePaste);
            CopyCommand = new RelayCommand(ExecuteCopy);
            DeleteCommand = new RelayCommand(ExecuteDelete, CanExecuteDelete);
            CutCommand = new RelayCommand(ExecuteCut, CanExecuteDelete);
            TestCommand = new RelayCommand(ExecuteTest);
            ToggleCommand = new RelayCommand(ExecuteToggle);
            EditRungCommand = new RelayCommand(ExecuteEditRung);
            EditRungCommentCommand = new RelayCommand(ExecuteEditRungComment);
            ImportRungsCommand = new RelayCommand(ExecuteImportRungs, CanExecuteImportRungs);
            ExportRungsCommand = new RelayCommand(ExecuteExportRungs, CanExecuteExportRungs);
            AddRungCommand = new RelayCommand(ExecuteAddRung);
            NewTagCommand = new RelayCommand(ExecuteNewTag);

            OnlineEdit = new RelayCommand(ExecuteOnlineEdit, CanExecuteOnlineEdit);
            AcceptEdit = new RelayCommand(ExecuteAcceptEdit, CanExecuteAcceptEdit);
            CancelEdit = new RelayCommand(ExecuteCancelEdit, CanExecuteCancelEdit);
            //CancelAcceptedEdit = new RelayCommand(ExecuteCancelAcceptedEdit, CanExecuteCancelAcceptedEdit);
            FinalizeEdit = new RelayCommand(ExecuteFinalizeEdit, CanExecuteFinalizeEdit);

            UndoCommand = new RelayCommand(ExecuteUndo, CanExecuteUndo);
            RedoCommand = new RelayCommand(ExecuteRedo, CanExecuteRedo);

            HotKeyPressedCommand = new RelayCommand<KeyEventArgs>(OnHotKeyPressed);

            Control = userControl;
            userControl.DataContext = this;

            UpdateAoiInstructionDefinition(_controller);
            InitializingLadderControl();

            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                _controller, "IsOnlineChanged", OnIsOnlineChanged);
            WeakEventManager<Controller, EventArgs>.AddHandler(
                _controller, "KeySwitchChanged", OnKeySwitchChanged);
            WeakEventManager<Controller, EventArgs>.AddHandler(
                _controller, "OperationModeChanged", OnOperationModeChanged);

            _dataServer = _controller?.CreateDataServer();
            _globalContainer = _controller;
            _localContainer = Routine?.ParentCollection?.ParentProgram;
            Notifications.ConnectConsumer(this);
            RegisterOperand();

            foreach (var rung in Graph.Rungs)
            {
                if (!rung.IsEndRung)
                    FindMainOperandDescription(rung);
            }

            _timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 100)
            };
            _timer.Tick += timer_Tick;
            _timer.Start();

            _dataServer?.StartMonitoring(true, true);

            LadderDiagramService.OnDoubleClickRung += ShowRungText;
            LadderDiagramService.OnDoubleClick += ShowTagBrowser;
            LadderDiagramService.OnUpdateGraph += UpdateGraph;
            LadderDiagramService.OnUpdateGraphAndFocus += UpdateGraphAndFocus;
            LadderDiagramService.OnUpdateGraphAfterDrag += UpdateGraphAfterDrag;
            LadderDiagramService.OnUpdateTag += UpdateTag;
            LadderDiagramService.OnInitGraph += InitGraph;
            LadderDiagramService.OnGetToolTip += GetToolTip;
            LadderDiagramService.OnShowConfig += ShowConfig;
            //LadderDiagramService.OnClickMenu += ClickMenu;

            if (_mainGrid != null)
            {
                _browseAdorner = new BrowseAdorner(_mainGrid, _controller, routine.ParentCollection.ParentProgram);
                _inputAdorner = new InputAdorner(_mainGrid);
                _enumAdorner = new BrowseEnumAdorner(_mainGrid);
            }

            _onlineEditToolBarVisible = _controller?.IsOnline == true ? Visibility.Visible : Visibility.Collapsed;
            OnlineEditToolBarEnable = _controller.KeySwitchPosition != ControllerKeySwitch.RunKeySwitch;

            Refresh();

            _caption = $"{Routine.ParentCollection.ParentProgram?.Name} - {Routine.Name}";
        }

        public override void Cleanup()
        {
            _timer.Stop();
            _dataServer.StopMonitoring(true, true);
            LadderDiagramService.OnDoubleClickRung -= ShowRungText;
            LadderDiagramService.OnDoubleClick -= ShowTagBrowser;
            LadderDiagramService.OnUpdateGraph -= UpdateGraph;
            LadderDiagramService.OnUpdateGraphAndFocus -= UpdateGraphAndFocus;
            LadderDiagramService.OnUpdateGraphAfterDrag -= UpdateGraphAfterDrag;
            LadderDiagramService.OnUpdateTag -= UpdateTag;
            LadderDiagramService.OnInitGraph -= InitGraph;
            LadderDiagramService.OnGetToolTip -= GetToolTip;
            LadderDiagramService.OnShowConfig -= ShowConfig;
            //LadderDiagramService.OnClickMenu -= ClickMenu;

            CleanupDropPoints();
            base.Cleanup();
        }

        private void InitDropPoints()
        {
            var bottomControl = BottomControl as CanvasControl;
            var topControl = TopControl as CanvasControl;
            if (bottomControl == null || topControl == null)
                return;

            InitDictionary(BoxInstructionStyleRenderer.DropPoints, bottomControl);
            InitDictionary(BoxInstructionStyleRenderer.DropPoints, topControl);
            InitDictionary(BoxInstructionStyleRenderer.RungDropPoints, bottomControl);
            InitDictionary(BoxInstructionStyleRenderer.RungDropPoints, topControl);
            InitDictionary(BoxInstructionStyleRenderer.BranchlvDropPoints, bottomControl);
            InitDictionary(BoxInstructionStyleRenderer.BranchlvDropPoints, topControl);
            InitDictionary(BoxInstructionStyleRenderer.TagDropPoints, bottomControl);
            InitDictionary(BoxInstructionStyleRenderer.TagDropPoints, topControl);
            InitDictionary(BoxInstructionStyleRenderer.ConstantDropPoints, bottomControl);
            InitDictionary(BoxInstructionStyleRenderer.ConstantDropPoints, topControl);
        }

        private void CleanupDropPoints()
        {
            var bottomControl = BottomControl as CanvasControl;
            var topControl = TopControl as CanvasControl;
            if (bottomControl == null || topControl == null)
                return;

            CleanupDictionary(BoxInstructionStyleRenderer.DropPoints, bottomControl);
            CleanupDictionary(BoxInstructionStyleRenderer.DropPoints, topControl);
            CleanupDictionary(BoxInstructionStyleRenderer.RungDropPoints, bottomControl);
            CleanupDictionary(BoxInstructionStyleRenderer.RungDropPoints, topControl);
            CleanupDictionary(BoxInstructionStyleRenderer.BranchlvDropPoints, bottomControl);
            CleanupDictionary(BoxInstructionStyleRenderer.BranchlvDropPoints, topControl);
            CleanupDictionary(BoxInstructionStyleRenderer.TagDropPoints, bottomControl);
            CleanupDictionary(BoxInstructionStyleRenderer.TagDropPoints, topControl);
            CleanupDictionary(BoxInstructionStyleRenderer.ConstantDropPoints, bottomControl);
            CleanupDictionary(BoxInstructionStyleRenderer.ConstantDropPoints, topControl);
        }

        private void InitDictionary(Dictionary<CanvasControl, List<Rectangle>> dic, CanvasControl ctrl)
        {
            if (!dic.ContainsKey(ctrl))
                dic[ctrl] = new List<Rectangle>();
        }

        private void CleanupDictionary(Dictionary<CanvasControl, List<Rectangle>> dic, CanvasControl ctrl)
        {
            if (!dic.ContainsKey(ctrl))
                return;

            dic[ctrl].Clear();
            dic.Remove(ctrl);
        }

        public delegate void RemoveAdornerDelegate(Adorner adorner);

        public static RemoveAdornerDelegate OnRemoveAdorner;

        private void Aoi_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        private readonly object _syncReference = new object();

        private void AddReference(AoiDataReference aoiDataReference)
        {
            lock (_syncReference)
            {
                List<AoiDataReference> list = Reference.ToList();
                var referenceItem = Reference.LastOrDefault(r => r.Routine == aoiDataReference?.Routine &&
                                                            r.Line == aoiDataReference?.Line &&
                                                            r.Column == aoiDataReference.Column &&
                                                            r.InnerAoiDefinition ==
                                                            aoiDataReference.InnerAoiDefinition &&
                                                            r.InnerDataReference ==
                                                            aoiDataReference.InnerDataReference);

                if (referenceItem != null)
                {
                    if (Equals(SelectedReference, referenceItem))
                    {
                        SelectedReference = Reference[0];
                    }

                    Reference.Remove(referenceItem);
                    list = Reference.ToList();
                }

                var aoiNode = aoiDataReference.ParamList?[0] as ASTName;

                var program = aoiDataReference.Routine.ParentCollection.ParentProgram;
                var aoiTagName = ObtainValue.GetAstName(aoiNode);
                if (aoiDataReference.InnerDataReference != null)
                {
                    var referenceName =
                        ObtainValue.GetAstName(aoiDataReference.InnerDataReference.ParamList?[0] as ASTName);
                    aoiDataReference.Title = $"{aoiTagName} ({referenceName})";
                }
                else
                {
                    if (aoiTagName.StartsWith("\\"))
                    {
                        var scope = aoiTagName.Substring(1, aoiTagName.IndexOf(".", StringComparison.Ordinal) - 1);
                        aoiDataReference.Title = $"{aoiTagName} ({scope})";
                    }
                    else
                    {
                        var tagName = aoiTagName;
                        if (tagName.IndexOf(".", StringComparison.Ordinal) > -1)
                        {
                            tagName = tagName.Substring(0, tagName.IndexOf(".", StringComparison.Ordinal));
                        }

                        var tag = program.Tags[tagName];
                        var scope = tag?.ParentCollection.ParentProgram.Name ?? "Controller";
                        aoiDataReference.Title = $"{aoiTagName} ({scope})";
                    }
                }

                var item = Reference.FirstOrDefault(r => AoiDataReferenceExtend.CompareIndex(r, aoiDataReference));
                if (item != null)
                {
                    Reference.Insert(Reference.IndexOf(item), aoiDataReference);
                    return;
                }
                else
                {
                    item = Reference.LastOrDefault(r => r.Title.StartsWith(aoiTagName));
                    if (item != null)
                    {
                        Reference.Insert(list.IndexOf(item) + 1, aoiDataReference);
                        return;
                    }
                }

                Reference.Add(aoiDataReference);
            }
        }

        private void MarkReference()
        {
            AoiDataReference lastItem = null;
            string title = "";
            int count = 1;
            foreach (var aoiDataReference in Reference)
            {
                if (lastItem != null)
                {
                    var nextTitle = RemoveIndex(aoiDataReference.Title);

                    if (title == nextTitle)
                    {
                        lastItem.Title = $"{title}[{count}]";
                        aoiDataReference.Title = $"{title}[{++count}]";
                        lastItem = aoiDataReference;
                        title = nextTitle;
                    }
                    else
                    {
                        lastItem = aoiDataReference;
                        title = RemoveIndex(lastItem.Title);
                        count = 1;
                    }
                }
                else
                {
                    lastItem = aoiDataReference;
                    title = RemoveIndex(aoiDataReference.Title);
                }
            }
        }

        private string RemoveIndex(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            if (name.EndsWith("]"))
            {
                var index = name.LastIndexOf("[", StringComparison.Ordinal);
                return name.Substring(0, index);
            }

            return name;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            IProgramModule referenceProgram = null;
            if (_isAOIRefeferencePage)
                referenceProgram = SelectedReference?.GetReferenceProgram();

            foreach (var p in operands)
            {
                if (Routine == null)
                {
                    break;
                }

                if (Routine.IsDeleted)
                    break;

                string operandName = p.GetTagExpression()?.ToString();
                string tagVal = p.FormattedValueString;
                string scope = Routine.ParentCollection.ParentProgram.Name;

                if (string.IsNullOrEmpty(operandName))
                    continue;

                if (!BoxInstructionStyleRenderer.Bindings.ContainsKey(scope))
                    continue;

                if (!_isAOIRefeferencePage)
                {
                    //如果是梯形图页面，直接更新就好
                    if (p.TagCollection == Routine.ParentCollection.ParentProgram.Tags ||
                        p.TagCollection.ParentProgram == null)
                        BoxInstructionStyleRenderer.UpdateTag(scope, operandName, tagVal);
                }
                else
                {
                    //如果是AOI中的梯形图监控，需要额外处理
                    //operands中包含所在域中的tag或tag成员，以及aoi实例上下文中所引用的tag

                    //调用AOI实例的Program的collection（实例可能是在controller中，但是调用只会在program下的routine中）
                    var referenceCollection =
                        referenceProgram?.Tags; //SelectedReference.Routine.ParentCollection.ParentProgram.Tags;

                    //既不在program中也不在controller中（在AOI定义中<definition>）
                    if (p.TagCollection != referenceCollection && p.TagCollection != _controller.Tags)
                        continue;

                    //供梯形图中更新UI使用，应该去掉前缀
                    //if (_referenceTable?.ContainsValue(operandName) == true)
                    //{
                    //AOI实例所在的上下文中，实例成员所引用的tag
                    if (_referenceTable != null)
                        foreach (DictionaryEntry pair in _referenceTable)
                        {
                            //可能会有多个成员引用同一个tag，所以需要遍历每个引用了它的成员
                            if (pair.Value.ToString() == operandName)
                            {
                                var updateName =
                                    pair.Key
                                        .ToString(); //nameTable[pair.Key.ToString()].Replace($"{referenceInstanceName}.", "");
                                BoxInstructionStyleRenderer.UpdateTag(scope, updateName, tagVal);
                            }
                        }
                    //}
                    //else if (operandName != null)
                    //{
                    //    updateName = operandName.Replace($"{referenceInstanceName}.", "");

                    //    //AOI中，scope为Routine名
                    //    BoxInstructionStyleRenderer.UpdateTag(scope, updateName, tagVal);
                    //}
                }
            }

            if (_controller.IsOnline)
            {
                TagSyncController tagSyncController =
                    _controller.Lookup(typeof(TagSyncController)) as TagSyncController;
                if (tagSyncController == null)
                    return;

                IEnumerable<IDataOperand> curDataOperands;
                if (SelectedReference == null)
                    curDataOperands = operands.Where(p =>
                        p.TagCollection == Routine?.ParentCollection?.ParentProgram?.Tags ||
                        p.TagCollection?.ParentProgram == null);
                else
                    curDataOperands = operands.Where(p =>
                        p.TagCollection == referenceProgram?.Tags || p.TagCollection?.ParentProgram == null ||
                        p.TagCollection == Routine?.ParentCollection?.ParentProgram.Tags);

                foreach (var operand in curDataOperands)
                {
                    if (operand.IsOperandValid)
                    {
                        tagSyncController.Update(operand.Tag, operand.GetTagExpression()?.ToString());
                    }
                    else
                    {
                        Debug.WriteLine($"{operand} is invalid!");
                    }
                }
            }
        }

        static List<IDataOperand> operands = new List<IDataOperand>();
        public static void ClearOperands() => operands.Clear();

        public string GetSimpleTag(string operand)
        {
            TagExpressionParser parser = new TagExpressionParser();
            var tagExpression = parser.Parser(operand);
            if (tagExpression == null)
                return null;

            SimpleTagExpression simpleTagExpression = parser.GetSimpleTagExpression(tagExpression);
            //if (simpleTagExpression == null)
            //    return null;

            return simpleTagExpression?.TagName;
            //ITagCollection tagCollection = _localContainer.Tags.FirstOrDefault(p => p.Name == simpleTagExpression.TagName) == null ? _globalContainer.Tags : _localContainer.Tags;
            //return tagCollection[simpleTagExpression.TagName];
        }

        public IDataOperand GetOperand(string tag)
        {
            var operand = operands.FirstOrDefault(p => p.GetTagExpression()?.ToString() == tag && (p.TagCollection == Routine.ParentCollection.ParentProgram?.Tags || p.TagCollection == _controller.Tags));

            // && p.TagCollection == (Routine.ParentCollection.ParentProgram.Tags ?? _controller.Tags));//这种写法，如果tag在controller而不在Program中时，会不匹配

            //&& p.TagCollection == Routine.ParentCollection.ParentProgram?.Tags || p.TagCollection == _controller.Tags

            return operand;
        }

        public DataOperandInfo GetDataOperandInfo(string operand)
        {
            var collection = _localContainer.Tags;
            // get tag and expression
            TagExpressionParser parser = new TagExpressionParser();
            var tagExpression = parser.Parser(operand);
            if (tagExpression == null)
                return null;

            SimpleTagExpression simpleTagExpression = parser.GetSimpleTagExpression(tagExpression);

            ITagCollection tagCollection = collection.FirstOrDefault(p => p.Name == simpleTagExpression.TagName) == null ? _controller.Tags : collection;

            var tag = tagCollection[simpleTagExpression.TagName] as Tag;

            if (tag == null)
                return null;

            // data type and value
            DataTypeInfo dataTypeInfo = tag.DataTypeInfo;
            IField field = tag.DataWrapper.Data;
            bool isDynamicField = false;
            DisplayStyle displayStyle = tag.DisplayStyle;
            bool isBit = false;
            int bitOffset = -1;

            TagExpressionBase expression = simpleTagExpression;
            while (expression.Next != null)
            {
                expression = expression.Next;

                int bitMemberNumber = -1;
                var bitMemberNumberAccessExpression = expression as BitMemberNumberAccessExpression;
                var bitMemberExpressionAccessExpression = expression as BitMemberExpressionAccessExpression;

                if (bitMemberNumberAccessExpression != null)
                {
                    bitMemberNumber = bitMemberNumberAccessExpression.Number;
                }
                else if (bitMemberExpressionAccessExpression != null)
                {
                    //TODO(gjc): need check here
                    if (bitMemberExpressionAccessExpression.Number.HasValue)
                        bitMemberNumber = bitMemberExpressionAccessExpression.Number.Value;
                    else if (bitMemberExpressionAccessExpression.ExpressionNumber != null)
                        bitMemberNumber = 0;
                }

                if (bitMemberNumber >= 0)
                {
                    if (!dataTypeInfo.DataType.IsInteger || dataTypeInfo.Dim1 != 0)
                        return null;

                    isBit = true;
                    bitOffset = bitMemberNumber;
                    displayStyle = DisplayStyle.Decimal;

                    break;
                }

                //
                var memberAccessExpression = expression as MemberAccessExpression;
                if (memberAccessExpression != null)
                {
                    var compositeField = field as ICompositeField;
                    var compositiveType = dataTypeInfo.DataType as CompositiveType;
                    if (compositeField != null && compositiveType != null && dataTypeInfo.Dim1 == 0)
                    {
                        var dataTypeMember = compositiveType.TypeMembers[memberAccessExpression.Name] as DataTypeMember;
                        if (dataTypeMember == null)
                            return null;

                        //if (dataTypeMember.IsHidden)
                        //    return null;

                        field = compositeField.fields[dataTypeMember.FieldIndex].Item1;
                        dataTypeInfo = dataTypeMember.DataTypeInfo;
                        displayStyle = dataTypeMember.DisplayStyle;

                        if (dataTypeMember.IsBit && dataTypeInfo.Dim1 == 0)
                        {
                            isBit = true;
                            bitOffset = dataTypeMember.BitOffset;
                            displayStyle = DisplayStyle.Decimal;
                        }


                    }
                    else
                    {
                        return null;
                    }

                }

                //
                var elementAccessExpression = expression as ElementAccessExpression;
                if (elementAccessExpression != null
                    && elementAccessExpression.Indexes != null
                    && elementAccessExpression.Indexes.Count > 0)
                {
                    int index;

                    switch (elementAccessExpression.Indexes.Count)
                    {
                        case 1:
                            if (dataTypeInfo.Dim1 > 0 && dataTypeInfo.Dim2 == 0 && dataTypeInfo.Dim3 == 0)
                            {
                                index = elementAccessExpression.Indexes[0];
                                break;
                            }
                            else
                                return null;
                        case 2:
                            if (dataTypeInfo.Dim1 > 0 && dataTypeInfo.Dim2 > 0 && dataTypeInfo.Dim3 == 0)
                            {
                                index = elementAccessExpression.Indexes[0] * dataTypeInfo.Dim1 +
                                        elementAccessExpression.Indexes[1];
                                break;
                            }
                            else
                                return null;
                        case 3:
                            if (dataTypeInfo.Dim1 > 0 && dataTypeInfo.Dim2 > 0 && dataTypeInfo.Dim3 > 0)
                            {
                                index = elementAccessExpression.Indexes[0] * dataTypeInfo.Dim2 * dataTypeInfo.Dim1 +
                                        elementAccessExpression.Indexes[1] * dataTypeInfo.Dim1 +
                                        elementAccessExpression.Indexes[2];
                                break;
                            }
                            else
                                return null;
                        default:
                            throw new NotImplementedException();
                    }

                    ArrayField arrayField = field as ArrayField;
                    BoolArrayField boolArrayField = field as BoolArrayField;

                    if (arrayField == null && boolArrayField == null)
                        return null;

                    if (arrayField != null)
                    {
                        if (index < 0 || index >= arrayField.Size())
                            return null;

                        dataTypeInfo = new DataTypeInfo { DataType = dataTypeInfo.DataType };
                        field = arrayField.fields[index].Item1;
                    }

                    if (boolArrayField != null)
                    {
                        if (index < 0 || index >= boolArrayField.getBitCount())
                            return null;

                        dataTypeInfo = new DataTypeInfo { DataType = dataTypeInfo.DataType };
                        field = boolArrayField;
                        isBit = true;
                        bitOffset = index;
                    }
                }

                if (elementAccessExpression != null
                    && elementAccessExpression.ExpressionIndexes != null
                    && elementAccessExpression.ExpressionIndexes.Count > 0)
                {
                    // ignore expression valid

                    ArrayField arrayField = field as ArrayField;
                    BoolArrayField boolArrayField = field as BoolArrayField;

                    if (arrayField == null && boolArrayField == null)
                        return null;

                    int index = 0;
                    if (arrayField != null)
                    {
                        dataTypeInfo = new DataTypeInfo { DataType = dataTypeInfo.DataType };
                        field = arrayField.fields[index].Item1;
                    }

                    if (boolArrayField != null)
                    {
                        dataTypeInfo = new DataTypeInfo { DataType = dataTypeInfo.DataType };
                        field = boolArrayField;
                        isBit = true;
                        bitOffset = index;
                    }

                    isDynamicField = true;
                }

            }

            return new DataOperandInfo
            {
                Expression = tagExpression,
                Tag = tag,
                DisplayStyle = displayStyle,
                Field = field,
                IsDynamicField = isDynamicField,
                DataTypeInfo = dataTypeInfo,
                IsBit = isBit,
                BitOffset = bitOffset
            };

        }

        public void RegisterOperand()
        {
            //1、直接Tag
            List<string> tags = _paraList.Where(p => !p.Contains('.') && !p.Contains('[')).Distinct().ToList();
            tags.ForEach(tag =>
            {
                ITagCollectionContainer container = null;
                if (_localContainer.Tags.Any(p => p.Name == tag))
                    container = _localContainer;
                else if (_globalContainer.Tags.Any(p => p.Name == tag))
                    container = _globalContainer;

                if (container != null && !(operands.Exists(p =>
                        p.GetTagExpression()?.ToString() == tag && p.TagCollection == container.Tags)))
                {
                    var data = _dataServer.CreateDataOperand(container.Tags, tag);
                    data.StartMonitoring(true, true);
                    operands.Add(data);
                }
            });

            //2、Tag成员和数组的元素
            List<string> tagsWithMember = _paraList.Where(p => p.Contains('.') || p.Contains('[')).Distinct().ToList();
            foreach (var item in tagsWithMember)
            {
                string tag = GetTagName(item);

                ITagCollectionContainer container = null;
                if (_localContainer.Tags.Any(p => p.Name == tag))
                    container = _localContainer;
                else if (_globalContainer.Tags.Any(p => p.Name == tag))
                    container = _globalContainer;
                if (container != null && !(operands.Exists(p =>
                        p.GetTagExpression()?.ToString() == item && p.TagCollection == container.Tags)))
                {
                    var data = _dataServer.CreateDataOperand(container.Tags, item);
                    data.StartMonitoring(true, true);
                    operands.Add(data);
                }
            }
        }

        private void UpdateAoiInstructionDefinition(Controller controller)
        {
            var aoiInstructionDefinitions = InstructionDefinition.AoiInstructionDefinitions;

            foreach (var aoi in controller.AOIDefinitionCollection.OfType<AoiDefinition>())
            {
                string keyName = aoi.Name.ToUpper();
                if (aoiInstructionDefinitions.ContainsKey(keyName))
                    aoiInstructionDefinitions[keyName] = aoi.GetRLLInstructionDefinition();
                else
                    aoiInstructionDefinitions.Add(keyName, aoi.GetRLLInstructionDefinition());

                aoiInstructionDefinitions[keyName].Parameters[0].IsMainOperand = true;
            }

            //string json = JsonConvert.SerializeObject(aoiInstructionDefinitions, Formatting.Indented);
        }

        public RLLRoutine Routine { get; }

        private string _caption;//= $"{Routine.ParentCollection.ParentProgram?.Name} - {Routine.Name}";
        public string Caption //=> $"{Routine.ParentCollection.ParentProgram?.Name} - {Routine.Name}";
        {
            private set
            {
                _caption = value;
                UpdateCaptionAction?.Invoke(Caption);
            }
            get { return _caption; }
        }
        public UserControl Control { get; }
        public Action CloseAction { get; set; }
        public Action<string> UpdateCaptionAction { get; set; }

        public object TopControl { get; private set; }
        public object BottomControl { get; private set; }

        public IGraph Graph { get; private set; }

        private void InitializingLadderControl()
        {
            string prog = Routine.ParentCollection.ParentProgram?.Name;
            var topControl = new LadderControl
            {
                FontFamily = new FontFamily("Consolas"),
                FontSize = 12,
                Scope = prog,
                Routine = Routine.Name
            };

            topControl.BeginInit();
            topControl.EndInit();

            var bottomControl = new LadderControl
            {
                FontFamily = new FontFamily("Consolas"),
                FontSize = 12,
                Scope = prog,
                Routine = Routine.Name
            };

            bottomControl.BeginInit();
            bottomControl.EndInit();

            Graph = bottomControl.Graph;
            topControl.Graph = Graph;

            Graph.IsOnline = _controller.IsOnline;
            InitializingLadderGraph(Graph);
            GetParams();

            TopControl = topControl;
            BottomControl = bottomControl;

            LadderDiagramService.InitSelectedVisuals(bottomControl, topControl);

            InitDropPoints();
            Debug.Assert(Graph.Rungs.Count == Routine.Rungs.Count + 1);
            if (Graph.Rungs.Count != Routine.Rungs.Count + 1)
                return;

            int i = 0;
            foreach (var rung in Graph.Rungs)
            {
                // rung.VerifyResult = VerifyResult.Success;
                if (i < Routine.Rungs.Count && Graph.IsOnline)
                    rung.EditState = (EditState)(int)Routine.Rungs[i++].Type;
            }
        }

        private List<string> _paraList = new List<string>();

        private void InitializingLadderGraph(IGraph graph)
        {
            try
            {
                RLLGrammarAnalysis grammarAnalysis = new RLLGrammarAnalysis();
                ASTRLLRoutine astRllRoutine = grammarAnalysis.Parse(Routine.RungsText, Routine.ParentController as Controller, Routine);
                //RLLGrammarParser.Parse(Routine.RungsText, Routine.ParentController as Controller);

                LadderElementBuilder builder = new LadderElementBuilder(graph);
                astRllRoutine.Accept(builder);

                var rungs = graph.Rungs;
                Debug.Assert(rungs.Count == Routine.Rungs.Count + 1);
                for (int i = 0; i < Routine.Rungs.Count; ++i)
                {
                    var rungType = Routine.Rungs[i];
                    var rllRung = rungs[i];

                    if (!string.IsNullOrEmpty(rungType.Comment))
                        rllRung.RungComment = rungType.Comment;

                    rllRung.EditState = (EditState)(int)rungType.Type;
                }

                foreach (var rung in Graph.Rungs)
                {
                    rung.VerifyResult = VerifyResult.Success;
                }

                foreach (var errorInfo in grammarAnalysis.Errors)
                {
                    var index = errorInfo.RungIndex;
                    if (Graph.Rungs.Count <= index)
                        continue;

                    if (errorInfo.Level == Level.Error)
                    {
                        if (Graph.Rungs[index].VerifyResult != VerifyResult.Error)
                            Graph.Rungs[index].VerifyResult = VerifyResult.Error;
                    }

                    if (errorInfo.Level == Level.Warning)
                    {
                        //Warning也需要标记感叹号
                        //if (Graph.Rungs[index].VerifyResult != VerifyResult.Error)
                        //    Graph.Rungs[index].VerifyResult = VerifyResult.Error;
                        //if (Graph.Rungs[index].VerifyResult != VerifyResult.Warning)
                        //    Graph.Rungs[index].VerifyResult = VerifyResult.Warning;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                //throw;
            }
        }

        private void FindMainOperandDescription(ILadderItem item)
        {
            IList<ILadderItem> instructions = new List<ILadderItem>();
            if (item is IRung)
                instructions = ((IRung)item).Instructions;
            if (item is IBranchLevel)
                instructions = ((IBranchLevel)item).Instructions;

            foreach (var element in instructions)
            {
                IInstruction inst = element as IInstruction;
                if (inst != null)
                {
                    FindMainOperandDescription(inst);
                }

                IBranch branch = element as IBranch;
                if (branch != null)
                {
                    foreach (var level in branch.BranchLevels)
                    {
                        FindMainOperandDescription(level);
                    }
                }
            }
        }

        private void FindMainOperandDescription(IInstruction instruction)
        {
            string scope = Routine.ParentCollection.ParentProgram.Name;

            var paras = instruction.Parameters;

            try
            {
                var definitions = instruction.Definition?.Parameters?.Where(p =>
                    !p.Type.Contains("Target") || (p.Type == "ReadWrite_DataTarget" && p.DataType == "SINT")).ToList();

                if (paras == null || definitions?.Count == 0)
                    return;

                var mainOperand = definitions?.FirstOrDefault(p => p.IsMainOperand);
                if (mainOperand == null)
                    return;

                var index = definitions.IndexOf(mainOperand);
                var operandName = paras[index];

                var operand = operands.FirstOrDefault(p =>
                    p.GetTagExpression()?.ToString() == operandName && (p.TagCollection.ParentProgram == null ||
                                                                        p.TagCollection.ParentProgram.Name == scope));

                if (mainOperand.DataType.Equals("routine", StringComparison.OrdinalIgnoreCase))
                {
                    var routine = Routine.ParentCollection[operandName];
                    instruction.MainOperandDescription = routine?.Description;
                    return;
                }

                if (operand == null)
                    return;

                var tag = operand.Tag as Tag;
                //tag?.SetChildDescription(Tag.GetOperand(operandName), description);
                if (tag == null)
                    return;

                var description = Tag.GetChildDescription(tag.Description, tag.DataTypeInfo, tag.ChildDescription,
                    Tag.GetOperand(operandName));
                instruction.MainOperandDescription = description;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private bool IsLegalOperand(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            if (name.EndsWith("_") || name.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
                return false;

            Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_\[\]\.:]*$");
            if (!regex.IsMatch(name))
                return false;

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

            if (keyWords.ToList().Exists(p => p.Equals(name, StringComparison.OrdinalIgnoreCase)))
                return false;

            return true;
        }

        List<string> InstructionsContainControl = new List<string>
        {
            "FAL", "AVE", "FSC", "SRT", "STD", "BSL", "BSR", "FFL", "FFU", "LFL", "LFU", "SQI", "SQL", "SQO", "DDT",
            "FBC"
        };

        private List<string> InstructionsContainTimerOrCounter = new List<string> { "TON", "TOF", "RTO", "CTU", "CTD" };

        private void GetParams()
        {
            if (Graph == null)
                return;
            _paraList.Clear();

            Graph.Instructions.ToList().ForEach(inst =>
            {
                //_paraList.AddRange(inst.Parameters.Where(IsLegalOperand));
                var definitions = inst.Definition?.Parameters;
                int i = 0;
                if (definitions != null)
                    foreach (var definition in definitions)
                    {
                        if (i >= inst.Parameters.Count)
                            break;
                        var para = inst.Parameters[i];

                        //TODO:Ender， 确认对CPS以外的Write_none的影响
                        if (definition.Type == "Write_none" && inst.Mnemonic != "CPS")
                        {
                            i++;
                            continue;
                        }

                        //if (ControlTypeSet.Exists(p => p.Equals(definition.DataType, StringComparison.OrdinalIgnoreCase)))
                        //{
                        //    i++;
                        //    continue;
                        //}

                        if (definition.Type.Contains("DataTarget"))
                        {
                            if (para.Contains("?"))
                                i++;
                            continue;
                        }

                        if (IsLegalOperand(para))
                            _paraList.Add(para);
                        i++;
                    }
                //}

                if (InstructionsContainControl.Contains(inst.Mnemonic))
                {
                    if (inst.Definition != null)
                    {
                        var controlNames = GetControlName(inst.Definition.Parameters, inst.Parameters);
                        if (controlNames != null)
                        {
                            foreach (var controlName in controlNames)
                            {
                                _paraList.AddRange(new List<string>
                                {
                                    //controlName,
                                    $"{controlName}.LEN", $"{controlName}.POS"
                                });
                            }
                        }
                    }
                }

                //"TON", "TOF", "RTO", "CTU", "CTD"
                if (InstructionsContainTimerOrCounter.Contains(inst.Mnemonic))
                {
                    var instanceName = inst.Parameters[0];
                    if (instanceName != "?")
                    {
                        _paraList.AddRange(new List<string>
                        {
                            instanceName, $"{instanceName}.PRE", $"{instanceName}.ACC"
                        });
                    }
                }

                bool isAOI = InstructionDefinition.AoiInstructionDefinitions.ContainsKey(inst.Mnemonic.ToUpper());

                if (isAOI && inst.Parameters?.Count > 0)
                {
                    string aoiInstance = inst.Parameters[0];
                    if (inst.Definition != null)
                    {
                        var visibleNotRequireds = inst.Definition.Parameters
                            .Where(p => !string.IsNullOrEmpty(p.Label) && p.Type == "Read_DataTarget").ToList();
                        _paraList.AddRange(visibleNotRequireds.Select(p => $"{aoiInstance}.{p.Label}"));
                    }

                    //var bitLegs = inst.Definition.BitLegs;
                    //_paraList.AddRange(bitLegs.Select(p => $"{aoiInstance}.{p}"));
                }

                // AOI也需要添加bitLeg
                if (inst.Definition?.BitLegs != null)
                {
                    string controlName;
                    if (InstructionDefinition.AoiInstructionDefinitions.ContainsKey(inst.Mnemonic.ToUpper()))
                        controlName = inst.Parameters?[0];
                    else
                    {
                        var controlNames = GetControlName(inst.Definition.Parameters, inst.Parameters)?.ToList();
                        controlName = controlNames?.Count > 0 ? controlNames[0] : null;
                    }

                    if (controlName != null)
                    {
                        _paraList.AddRange(inst.Definition.BitLegs.Select(p => $"{controlName}.{p}"));
                    }
                }
            });
            _paraList = _paraList.Distinct().ToList(); //.Where(p =>
            //{
            //    double result;
            //    return !double.TryParse(p, out result);
            //}).ToList();
        }

        List<string> ControlTypeSet = new List<string>
        {
            "Control", "MOTION_INSTRUCTION", "PHASE_INSTRUCTION", "EXT_ROUTINE_CONTROL", "TIMER", "COUNTER", "MESSAGE",
            "MSG"
        };

        private IEnumerable<string> GetControlName(List<InstructionParameter> parasDefinition,
            IList<string> paras)
        {
            int index = 0;
            foreach (var para in parasDefinition)
            {
                string parameter = null;
                if (index >= parasDefinition.Count || index >= paras.Count)
                    yield break;

                if ((para.Type == "Read_DataSource"
                     || para.Type == "Read_none"
                     || para.Type == "Write_DataSource"
                     || para.Type == "Write_none"
                     || para.Type == "ReadWrite_DataSource"
                     || para.Type == "ReadWrite_none"))
                {
                    parameter = paras[index];
                }

                if (!para.Type.Contains("DataTarget") || paras[index].Contains("?"))
                    index++;

                if (parameter != null &&
                    ControlTypeSet.Exists(p => p.Equals(para.DataType, StringComparison.OrdinalIgnoreCase)))
                {
                    yield return parameter;
                }
            }
        }

        public bool UpdateLadderGraph(FocusInfo info)
        {
            try
            {
                Graph = ((LadderControl)BottomControl).Graph = new DefaultGraph();
                ((LadderControl)TopControl).Graph = Graph;
                RLLGrammarAnalysis grammarAnalysis = new RLLGrammarAnalysis();

                ASTRLLRoutine astRllRoutine =
                    grammarAnalysis.Parse(Routine.RungsText, Routine.ParentController as Controller, Routine);
                LadderElementBuilder builder = new LadderElementBuilder(Graph);
                astRllRoutine.Accept(builder);
                GetParams();
                RegisterOperand();

                //Graph的Rungs中包含EndRung
                Debug.Assert(Graph.Rungs.Count == Routine.Rungs.Count + 1);

                int i = 0;
                foreach (var rung in Graph.Rungs)
                {
                    rung.VerifyResult = VerifyResult.Success;
                    if (i == info.RungIndex && info.Offset < 0)
                        Graph.FocusedItem = rung;

                    if (i < Routine.Rungs.Count)
                    {
                        rung.RungComment = Routine.Rungs[i].Comment;
                        rung.EditState = (EditState)(int)Routine.Rungs[i++].Type;
                        FindMainOperandDescription(rung);
                    }
                }

                foreach (var errorInfo in grammarAnalysis.Errors)
                {
                    var index = errorInfo.RungIndex;
                    if (Graph.Rungs.Count <= index)
                        continue;

                    if (errorInfo.Level == Level.Error)
                    {
                        if (Graph.Rungs[index].VerifyResult != VerifyResult.Error)
                            Graph.Rungs[index].VerifyResult = VerifyResult.Error;
                    }

                    if (errorInfo.Level == Level.Warning)
                    {
                        //Warning也需要标记感叹号
                        //if (Graph.Rungs[index].VerifyResult != VerifyResult.Error)
                        //    Graph.Rungs[index].VerifyResult = VerifyResult.Error;
                        //if (Graph.Rungs[index].VerifyResult != VerifyResult.Warning)
                        //    Graph.Rungs[index].VerifyResult = VerifyResult.Warning;                    
                    }
                }

                Graph.IsOnline = _controller.IsOnline;
                Graph.FocusInfo = info;

                ((LadderControl)BottomControl).RefreshLadderLayout();
                ((LadderControl)TopControl).RefreshLadderLayout();
                IsDirty = true;
                RefreshExecutable();

                Refresh();
                ((LadderControl)BottomControl).Focus();
                return true;
            }
            catch (InstructionException instException)
            {
                _outputService?.AddMessages(instException.Message, null);
                return false;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }


        public bool UpdateLadderGraph(int focusIndex = -1)
        {
            var info = new FocusInfo { RungIndex = focusIndex, Offset = -1 };
            return UpdateLadderGraph(info);
        }

        public void UpdateRungComment(int rungIndex, string comment)
        {
            var oldRungs = Routine.CloneRungs();
            Routine.Rungs[rungIndex].Comment = comment;
            Graph.Rungs[rungIndex].RungComment = comment;
            UpdateLadderGraph();
            RecordChange(oldRungs);
        }

        public void UpdateTagDescription(IInstruction instruction, string description)
        {
            if (instruction == null)
                return;

            var oldRungs = Routine.CloneRungs();
            string scope = Routine.ParentCollection.ParentProgram.Name;
            var paras = instruction.Parameters;
            var definitions = instruction.Definition.Parameters.Where(p => !p.Type.Contains("Target") || (p.Type == "ReadWrite_DataTarget" && p.DataType == "SINT")).ToList();

            if (paras == null || definitions.Count == 0)
                return;

            var mainOperand = definitions.FirstOrDefault(p => p.IsMainOperand);
            if (mainOperand == null)
                return;

            var index = definitions.IndexOf(mainOperand);
            var operandName = paras[index];

            if (mainOperand.DataType.Equals("routine", StringComparison.OrdinalIgnoreCase))
            {
                var routine = Routine.ParentCollection[operandName];
                if (routine == null)
                    return;

                routine.Description = description;
            }
            else
            {
                var operand = operands.FirstOrDefault(p =>
                    p.GetTagExpression()?.ToString() == operandName && (p.TagCollection.ParentProgram == null ||
                                                                        p.TagCollection.ParentProgram.Name == scope));

                if (operand == null)
                    return;

                var tag = operand.Tag as Tag;
                tag?.SetChildDescription(Tag.GetOperand(operandName), description);
            }

            UpdateLadderGraph();
            RecordChange(oldRungs);
        }

        public string GetMainOperandDescription(IInstruction instruction)
        {
            if (instruction == null)
                return null;

            string scope = Routine.ParentCollection.ParentProgram.Name;
            var paras = instruction.Parameters;
            var definitions = instruction.Definition.Parameters.Where(p => !p.Type.Contains("Target") || (p.Type == "ReadWrite_DataTarget" && p.DataType == "SINT")).ToList();

            if (paras == null || definitions.Count == 0)
                return null;

            var mainOperand = definitions.FirstOrDefault(p => p.IsMainOperand);
            if (mainOperand == null)
                return null;

            var index = definitions.IndexOf(mainOperand);
            var operandName = paras[index];

            return GetDescription(operandName, scope);
        }

        public void ExecuteEditMainOperandDescription(IInstruction instruction)
        {
            if (instruction == null)
                return;

            string scope = Routine.ParentCollection.ParentProgram.Name;
            var paras = instruction.Parameters;
            var definitions = instruction.Definition.Parameters.Where(p =>
                !p.Type.Contains("Target") || (p.Type == "ReadWrite_DataTarget" && p.DataType == "SINT")).ToList();

            if (paras == null || definitions.Count == 0)
                return;

            var mainOperand = definitions.FirstOrDefault(p => p.IsMainOperand);
            if (mainOperand == null)
                return;

            var index = definitions.IndexOf(mainOperand);
            var operandName = paras[index];
            string description;

            if (mainOperand.DataType.Equals("routine", StringComparison.OrdinalIgnoreCase))
            {
                var routine = Routine.ParentCollection[operandName];
                if (routine == null)
                    return;

                //routine.Description = description;
                description = routine.Description;
            }
            else
            {
                var operand = operands.FirstOrDefault(p =>
                    p.GetTagExpression()?.ToString() == operandName && (p.TagCollection.ParentProgram == null ||
                                                                        p.TagCollection.ParentProgram.Name == scope));

                if (operand == null)
                    return;

                var tag = operand.Tag as Tag;
                if (tag == null)
                    return;

                if (operandName.Contains('.') || operandName.Contains('['))
                {
                    var simpleTag = tag.Name;
                    description = Tag.GetChildDescription(tag.Description, tag.DataTypeInfo, tag.ChildDescription,
                        operandName.Substring(simpleTag.Length));
                }
                else
                    description = tag.Description;
            }

            EditingControl = null;
            Graph.FocusedItem = instruction;
            var layer = AdornerLayer.GetAdornerLayer(_mainGrid);
            try
            {
                if (!HasAdorner(layer, InputAdorner))
                    layer?.Add(InputAdorner);

                InputAdorner.Resize();
                InputAdorner.ResetAdorner(_original, 1, description); //new Point(500,500)
                InputAdorner.SetTextFocus();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message + e.StackTrace);
            }
        }

        private void OnOperationModeChanged(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                Refresh();
            });
        }

        private void OnKeySwitchChanged(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                Refresh();
                RefreshExecutable();
                //OnlineEditToolBarEnable = _controller.KeySwitchPosition != ControllerKeySwitch.RunKeySwitch;
                //Graph.IsKeyOn = OnlineEditToolBarEnable;
            });
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                Refresh();
                OnlineEditToolBarVisible = _controller.IsOnline ? Visibility.Visible : Visibility.Collapsed;
                Graph.IsOnline = _controller.IsOnline;
            });
        }

        private void Refresh()
        {
            bool isToExecute = RoutineExtend.CheckRoutineInRun(Routine);

            if (_isAOIRefeferencePage)
            {
                isToExecute = RoutineExtend.CheckRoutineInRun(SelectedReference.Routine);
            }

            if (_controller.IsOnline)
            {
                if (_controller.OperationMode == ControllerOperationMode.OperationModeRun && isToExecute)
                {
                    Graph.PowerFlowStyle = PowerFlowStyle.PowerOn;
                }
                else
                {
                    Graph.PowerFlowStyle = PowerFlowStyle.HalfPowerOn;
                }

            }
            else
            {
                Graph.PowerFlowStyle = PowerFlowStyle.PowerOff;
            }
        }

        private Visibility _ldtEditorVisible = Visibility.Collapsed;

        public Visibility LdtEditorVisible
        {
            get { return _ldtEditorVisible; }
            set { Set(ref _ldtEditorVisible, value); }
        }

        private string _ldtText = string.Empty;

        public string LdtText
        {
            get { return _ldtText; }
            set { Set(ref _ldtText, value); }
        }

        private string _rungInfo;

        public string RungInfo
        {
            get { return _rungInfo; }
            set { Set(ref _rungInfo, value); }
        }

        public RelayCommand ShowLdtEditorCommand { get; set; }

        private void ExecuteShowLdtEditor()
        {
            //在线禁止编辑全部代码
            if (_controller.IsOnline)
                return;

            _selRungIndex = 0;
            _isEditRung = false;
            RungInfo = "Ladder Diagram Text";
            LdtText = string.Join("   ", Routine.RungsText);
            LdtEditorVisible = Visibility.Visible;
        }

        private bool _isEditRung;
        private int? _selRungIndex;

        private void ShowRungText(IRung rung)
        {
            //校验是否当前窗口
            if (((LadderControl)BottomControl).Graph != rung.ParentCollection.Graph)
                return;

            string text = LadderHelper.GetText(rung);

            _selRungIndex = null;

            if (!rung.IsEndRung)
            {
                if (!_controller.IsOnline)
                {
                    _selRungIndex = rung.RungIndex;
                    _isEditRung = true;
                    RungInfo = $"Rung:{rung.RungIndex}";
                    LdtText = text;
                }
                else
                {
                    if (rung.EditState == EditState.Normal)// || rung.EditState == EditState.AcceptEdit || rung.EditState == EditState.AcceptInsert
                    {
                        //相当于Online Edit
                        ExecuteOnlineEdit();
                    }
                    else if (rung.EditState == EditState.Edit || rung.EditState == EditState.Insert)//|| rung.EditState == EditState.EditAcceptEdit 
                    {
                        //直接弹出文本框
                        _selRungIndex = rung.RungIndex;
                        _isEditRung = true;
                        RungInfo = $"Rung:{rung.RungIndex}";
                        LdtText = text;
                        LdtEditorVisible = Visibility.Visible;
                    }

                    return;
                }
            }
            else
            {
                //TODO:如果是EndRung，则新增一行
                _selRungIndex = Routine.Rungs.Count;
                _isEditRung = true;
                LdtText = ";";
                Routine.Rungs.Add(new RungType
                { Text = LdtText, Type = _controller.IsOnline ? RungTypeEnum.Insert : RungTypeEnum.Normal });
                RungInfo = $"Rung:{_selRungIndex}";

                UpdateLadderGraph(_selRungIndex.Value);
                ClearChange();
            }

            //打开编辑框，编辑前，需备份原Text

            LdtEditorVisible = Visibility.Visible;
        }

        public RelayCommand<string> FinishLdtEdit { get; set; }

        readonly IErrorOutputService _outputService =
            Package.GetGlobalService(typeof(SErrorOutputService)) as IErrorOutputService;

        private void ExecuteFinishLdtEdit(string para)
        {
            if (para.Equals("true"))
            {
                var oldRungs = Routine.CloneRungs();

                string newText = LdtText;

                if (!LdtText.Trim().EndsWith(";"))
                    LdtText = LdtText.Trim() + ";";

                if (!_controller.IsOnline)
                {
                    bool verified = false;

                    try
                    {
                        // 分析RungText合法性
                        RLLGrammarAnalysis grammarAnalysis = new RLLGrammarAnalysis();
                        if (_selRungIndex != null)
                            grammarAnalysis.SetStartRungIndex(_selRungIndex.Value);
                        _outputService?.RemoveError(Routine, Routine.CurrentOnlineEditType);
                        grammarAnalysis.Analysis(LdtText, _controller, Routine);

                        //if (LdtText == ";")
                        //    grammarAnalysis.Errors.Clear();

                        //foreach (var error in grammarAnalysis.Errors)
                        //{
                        //    if (error.Level == Level.Error)
                        //        _outputService?.AddErrors(error.Info, OrderType.None, OnlineEditType.Original,
                        //            error.RungIndex, null, Routine);
                        //    if (error.Level == Level.Warning)
                        //        _outputService?.AddWarnings(error.Info, Routine);
                        //}
                        //bool existError = grammarAnalysis.Errors.Exists(p => p.Level == Level.Error || p.Level == Level.Warning);

                        //if (grammarAnalysis.Errors.Count(p => p.Level == Level.Error) < 1)
                        //{
                        if (_isEditRung)
                        {
                            // 仅更新所选Rung
                            var selRung = Graph.FocusedItem as IRung;//BoxInstructionStyleRenderer.focusElement.Model as IRung;
                            if (selRung == null)
                                return;

                            Graph.FocusedItem = selRung;

                            //if (existError)
                            //    selRung.VerifyResult = VerifyResult.Error;

                            UpdateGraph(Graph, LdtText);
                            LdtEditorVisible = Visibility.Collapsed;
                            return;
                        }

                        // 如果是整体更新，则newText即LdtText；如果是更新单行Rung，则newText是修改后的整体Code
                        List<string> codeText = newText.Trim()
                            .Split(new[] { "   " }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList();
                        Routine.UpdateRungs(codeText);
                        verified = true;
                        //}
                    }
                    catch (ErrorInfo error)
                    {
                        Console.WriteLine(error.Message + error.StackTrace);
                        if (error.Level == Level.Error)
                            _outputService?.AddErrors(error.Info, OrderType.None, OnlineEditType.Original, error.RungIndex,
                                null, Routine);
                        if (error.Level == Level.Warning)
                            _outputService?.AddWarnings(error.Info, Routine);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message + e.StackTrace);
                        _outputService?.AddErrors("Unknown Error.", OrderType.None, OnlineEditType.Original, _selRungIndex,
                            null, Routine);
                    }

                    if (verified)
                    {
                        if (!UpdateLadderGraph())
                        {
                            Routine.UpdateRungs(oldRungs);
                            UpdateLadderGraph();
                            return;
                        }

                        RecordChange(oldRungs);

                        _outputService?.RemoveError(Routine);

                        //更新完梯形图，没有问题才收起输入框
                        LdtEditorVisible = Visibility.Collapsed;

                        RegisterOperand();
                    }
                }
                else
                {
                    //在线编辑
                    FinishLdtEditOnline(newText);
                }
            }
            else
            {
                LdtEditorVisible = Visibility.Collapsed;
            }
        }

        private void FinishLdtEditOnline(string newText)
        {
            //var rung = GetCurRung();
            //if (rung == null || rung.IsEndRung)
            //    return;

            if (_selRungIndex == null)
                return;

            int rungIndex = _selRungIndex.Value;//rung.RungIndex;
            bool verified = false;

            //var backup = Routine.Rungs.Select(p => p.Clone());
            var oldRungs = Routine.CloneRungs();

            try
            {
                // 分析RungText合法性
                RLLGrammarAnalysis grammarAnalysis = new RLLGrammarAnalysis();
                if (_selRungIndex != null)
                    grammarAnalysis.SetStartRungIndex(rungIndex);
                _outputService?.RemoveError(Routine, Routine.CurrentOnlineEditType);
                grammarAnalysis.Analysis(newText, _controller, Routine);

                foreach (var error in grammarAnalysis.Errors)
                {
                    if (error.Level == Level.Error)
                        _outputService?.AddErrors(error.Info, OrderType.None, OnlineEditType.Original, error.RungIndex, error.Offset, Routine);
                    if (error.Level == Level.Warning)
                        _outputService?.AddWarnings(error.Info, Routine, error.RungIndex, error.Offset);
                }

                if (grammarAnalysis.Errors.Count(p => p.Level == Level.Error) < 1)
                {
                    Routine.Rungs[rungIndex].Text = newText;
                    UpdateLadderGraph(rungIndex);
                    verified = true;
                }
            }
            catch (ErrorInfo error)
            {
                Console.WriteLine(error.Message + error.StackTrace);
                if (error.Level == Level.Error)
                    _outputService?.AddErrors(error.Info, OrderType.None, OnlineEditType.Original, error.RungIndex,
                        error.Offset, Routine);
                if (error.Level == Level.Warning)
                    _outputService?.AddWarnings(error.Info, Routine, error.RungIndex,
                        error.Offset);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
                _outputService?.AddErrors("Unknown Error.", OrderType.None, OnlineEditType.Original, _selRungIndex, null, Routine);
            }

            if (!verified)
            {
                //Routine.UpdateRungs(_ldtTextBackup);
                Routine.UpdateRungs(oldRungs);
                UpdateLadderGraph();
                return;
            }

            RecordChange(oldRungs);

            LdtEditorVisible = Visibility.Collapsed;
        }

        private void VerifyRung()//IRung rung
        {
            //if (!Graph.Rungs.Contains(rung))
            //    return;
            var rung = Graph.FocusedItem as IRung;
            if (rung == null || rung.IsEndRung)
                return;

            try
            {
                var codeText = LadderHelper.GetText(rung);
                //_outputService?.RemoveMessage(Routine);
                //_outputService?.RemoveError(Routine, Routine.CurrentOnlineEditType);
                //_outputService?.RemoveWarning(Routine);
                _outputService?.Cleanup();
                _outputService?.AddMessages(
                    $"Verifying rung in routine: {Routine.ParentCollection.ParentProgram.Name} - {Routine.Name} ...",
                    Routine);

                RLLGrammarAnalysis grammarAnalysis = new RLLGrammarAnalysis();
                grammarAnalysis.SetStartRungIndex(rung.RungIndex);
                grammarAnalysis.Analysis(codeText, _controller, Routine);

                foreach (var error in grammarAnalysis.Errors)
                {
                    if (error.Level == Level.Error)
                        _outputService?.AddErrors(error.Info, OrderType.None, OnlineEditType.Original, error.RungIndex,
                            error.Offset, Routine);
                    if (error.Level == Level.Warning)
                        _outputService?.AddWarnings(error.Info, Routine, error.RungIndex, error.Offset);
                }

                var errorCount = grammarAnalysis.Errors.Count(p => p.Level == Level.Error);
                var warningCount = grammarAnalysis.Errors.Count(p => p.Level == Level.Warning);
                _outputService?.AddMessages($"Complete - {errorCount} error(s), {warningCount} warning(s)", null);
            }
            catch (ErrorInfo error)
            {
                Console.WriteLine(error.Message + error.StackTrace);
                if (error.Level == Level.Error)
                    _outputService?.AddErrors(error.Info, OrderType.None, OnlineEditType.Original, error.RungIndex,
                        error.Offset, Routine);
                if (error.Level == Level.Warning)
                    _outputService?.AddWarnings(error.Info, Routine, error.RungIndex, error.Offset);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
                _outputService?.AddErrors("Unknown Error.", OrderType.None, OnlineEditType.Original, 0, null, Routine);
            }
        }

        public void ClickMenu(IGraph graph, MenuItemType type)
        {
            if (graph != Graph)
                return;

            switch (type)
            {
                case MenuItemType.Cut:
                    ExecuteCut();
                    break;
                case MenuItemType.Copy:
                    ExecuteCopy();
                    break;
                case MenuItemType.Paste:
                    ExecutePaste();
                    break;
                case MenuItemType.Delete:
                    ExecuteDelete();
                    break;
                case MenuItemType.Add:
                    ExecuteAddRung();
                    break;
                case MenuItemType.Edit:
                    ExecuteEditRung();
                    break;
                case MenuItemType.EditComment:
                    ExecuteEditRungComment();
                    break;
                case MenuItemType.ImportRungs:
                    ExecuteImportRungs();
                    break;
                case MenuItemType.ExportRungs:
                    ExecuteExportRungs();
                    break;
                case MenuItemType.VerifyRung:
                    VerifyRung();
                    break;
                default:
                    throw new Exception("Unknown Menu Item");
            }
        }

        public void OpenAOILogic(string mnemonic)
        {
            var aoiDef =
                _controller.AOIDefinitionCollection.FirstOrDefault(p =>
                    p.Name.Equals(mnemonic, StringComparison.OrdinalIgnoreCase));

            if (aoiDef == null)
                return;

            var routine = aoiDef.Routines["Logic"];
            if (routine == null)
                return;

            ICreateEditorService createDialogService =
                Package.GetGlobalService(typeof(SCreateEditorService)) as
                    ICreateEditorService;
            if (routine is RLLRoutine)
                createDialogService?.CreateRLLEditor(routine);
            if (routine is STRoutine)
                createDialogService?.CreateSTEditor(routine);
        }

        public RelayCommand PasteCommand { get; }

        public bool CanPasted()
        {
            return true;
        }

        public void DoPaste()
        {
            ExecutePaste();
        }

        public void ExecutePaste()
        {
            var iData = Clipboard.GetDataObject();

            if (iData == null)
                return;

            if (!iData.GetDataPresent(DataFormats.Text))
                return;

            string clipStr = iData.GetData(DataFormats.Text) as string;

            var oldRungs = Routine.CloneRungs();
            FocusInfo info = new FocusInfo { RungIndex = -1, Offset = -1 };

            if (!_controller.IsOnline)
            {
                IRung rung = GetCurRung();
                if (rung == null)
                    return;

                int rungIndex = rung.RungIndex;
                var rungs = Routine.Rungs;

                bool copyIsRung = clipStr?.Contains(";") == true;
                if (rungs.Count > 1 && copyIsRung && rung.EditState == EditState.Edit)
                    return;

                info.RungIndex = copyIsRung ? rungIndex + 1 : rungIndex;

                string newText = BoxInstructionStyleRenderer.GenerateTextWithCopy(clipStr);

                if (string.IsNullOrEmpty(newText))
                    return;

                List<string> newCodes = newText.Trim().Split(new[] { "   " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                Debug.Assert(rungIndex <= rungs.Count);

                if (newCodes.Count > 1)
                {
                    //插入Rung
                    if (rung.IsEndRung)
                    {
                        rungs.AddRange(newCodes.Select(p => new RungType { Text = p }));
                    }
                    else
                        rungs.InsertRange(rungIndex + 1, newCodes.GetRange(1, newCodes.Count - 1).Select(p => new RungType { Text = p }));
                }
                else if (newCodes.Count == 1)
                {
                    if (rung.IsEndRung)
                    {
                        //插入到EndRung前面
                        if (copyIsRung)
                        {
                            rungs.Add(new RungType { Text = newCodes[0] });
                            info.RungIndex = rungIndex;
                        }
                    }
                    else
                    {
                        //修改Rung
                        rungs[rungIndex].Text = newCodes[0];

                        var focusItem = Graph.FocusedItem;
                        if (focusItem is IInstruction || focusItem is IBranchLevel)
                            info.Offset = focusItem.Offset + 1;

                        if (focusItem is IBranch)
                            info.Offset = OffsetHelper.GetBranchLastOffset((IBranch)focusItem) + 1;
                    }
                }
                else
                    return;

                if (!UpdateLadderGraph(info))
                {
                    Routine.UpdateRungs(oldRungs);
                    UpdateLadderGraph();
                    MessageBox.Show("Syntax Error!", "Notice");
                }
                else
                    RecordChange(oldRungs);
            }
            else
                OnlinePast(clipStr);
        }

        public RelayCommand DeleteCommand { get; }

        private void ExecuteDelete()
        {
            var oldRungs = Routine.CloneRungs();
            //int focusIndex = -1;
            FocusInfo info = new FocusInfo { RungIndex = -1, Offset = -1 };
            if (!_controller.IsOnline)
            {
                bool selIsRung = Graph.FocusedItem is IRung;
                if (selIsRung)
                {
                    var selElements = LeftPowerRailStyleRenderer.GetSelRungs();

                    var selModels = selElements.Select(p => p.Model as IRung).ToList();
                    var firstRung = selModels.ToList().OrderBy(q => q.RungIndex).FirstOrDefault();
                    info.RungIndex = firstRung.RungIndex;
                    //info.RungIndex = selModels.FirstOrDefault()?.RungIndex ?? -1;

                    foreach (var rung in selModels)
                    {
                        if (rung == null || rung.IsEndRung)
                            continue;

                        Routine.Rungs[rung.RungIndex].Mark = EditMark.Delete;
                    }

                    int index = 0;
                    while (index < Routine.Rungs.Count)
                    {
                        var curRung = Routine.Rungs[index];
                        if (curRung.Mark == EditMark.Delete)
                            Routine.Rungs.RemoveAt(index);
                        else
                            index++;
                    }
                }
                else
                {
                    var selRung = Graph.FocusedItem.GetParentRung();
                    if (selRung == null)
                        return;
                    int rungIndex = selRung.RungIndex;

                    string newText = BoxInstructionStyleRenderer.Delete();

                    if (string.IsNullOrEmpty(newText) || newText == "UnFocused" || newText == "UnSelected")
                        return;

                    bool lastIsBranch = false;
                    var parentItem = Graph.FocusedItem.ParentItem;
                    if (parentItem is IRung)
                    {
                        var rung = parentItem as IRung;
                        if (rung.Instructions.LastOrDefault() == Graph.FocusedItem)
                            lastIsBranch = true;
                    }

                    if (parentItem is IBranchLevel)
                    {
                        var level = parentItem as IBranchLevel;
                        if (level.Instructions.LastOrDefault() == Graph.FocusedItem)
                            lastIsBranch = true;
                    }

                    Routine.Rungs[rungIndex].Text = newText;

                    int maxOffset = OffsetHelper.GetNewOffset(selRung) - 1;
                    int selOffset = Graph.FocusedItem.Offset;

                    info.RungIndex = rungIndex;
                    if (newText.Trim() == ";")
                    {
                    }
                    else if (maxOffset == selOffset)
                    {
                        info.Offset = maxOffset - 1;
                    }
                    else if (lastIsBranch)
                    {
                        info.Offset = Graph.FocusedItem.Offset - 1;
                    }
                    else
                    {
                        info.Offset = Graph.FocusedItem.Offset;
                    }
                    //TODO(Ender): 考虑多选删除后的定位
                }

                if (!UpdateLadderGraph(info))
                {
                    Routine.UpdateRungs(oldRungs);
                    UpdateLadderGraph();
                }
                else
                    RecordChange(oldRungs);
            }
            else
                OnlineDelete();
        }

        public bool CanExecuteDelete()
        {
            IRung rung = GetCurRung();
            if (rung == null || (LeftPowerRailStyleRenderer.GetSelRungs().Count == 1 && rung.IsEndRung))
                return false;

            if (!_controller.IsOnline)
                return true;

            if (rung.EditState == EditState.Normal || rung.EditState == EditState.Edit || rung.EditState == EditState.Insert)// ||rung.EditState == EditState.AcceptEdit  ||rung.EditState == EditState.AcceptInsert || rung.EditState == EditState.EditAcceptEdit
                return true;

            return false;
        }

        public RelayCommand CutCommand { get; }

        public bool CanCut()
        {
            return true;
        }


        public void DoCut()
        {
            ExecuteCut();
        }

        public void ExecuteCut()
        {
            ExecuteCopy();

            //先复制再删除
            ExecuteDelete();
        }

        public RelayCommand CopyCommand { get; }

        public bool CanCopy()
        {
            return true;
        }

        public void DoCopy()
        {
            ExecuteCopy();
        }

        public void ExecuteCopy()
        {
            string codeText = string.Empty;
            var selElements = LeftPowerRailStyleRenderer.GetSelRungs();

            if (selElements == null || selElements.Count < 1)
                selElements = BoxInstructionStyleRenderer.GetSelElements();

            if (selElements == null || selElements.Count < 1)
                return;

            var first = selElements[0].Model;
            if (first is IRung)
            {
                codeText = LeftPowerRailStyleRenderer.GetSelText();
            }
            else if (first is IInstruction || first is IBranch || first is IBranchLevel)
            {
                codeText = BoxInstructionStyleRenderer.GetSelText();
            }

            Clipboard.SetDataObject(codeText);
        }

        #region 在线编辑

        private IRung GetCurRung()
        {
            var item = Graph.FocusedItem;
            if (item == null)
                return null;

            IRung rung;
            if (item is IRung)
                rung = item as IRung;
            else
                rung = item.GetParentRung();

            return rung;
        }

        public RelayCommand OnlineEdit { get; }

        private void ExecuteOnlineEdit()
        {
            IRung rung = GetCurRung();
            if (rung == null)
                return;

            int rungIndex = rung.RungIndex;
            if (rung.EditState == EditState.Normal)//|| rung.EditState == EditState.AcceptEdit
            {
                var oldRungs = Routine.CloneRungs();
                //原行号置为i，复制到下一行后置为r
                var routineRung = Routine.Rungs[rungIndex];
                Routine.Rungs.Insert(rungIndex + 1, routineRung.Clone());
                //如果是已提交的编辑或插入，再次编辑时需要标记为编辑已提交的编辑(EditAcceptEdit)
                //Routine.Rungs[rungIndex].Type = rung.EditState == EditState.Normal ? RungTypeEnum.Edit : RungTypeEnum.EditAcceptEdit;
                //Routine.Rungs[rungIndex + 1].Type = rung.EditState == EditState.Normal ? RungTypeEnum.EditOriginal : RungTypeEnum.EditAcceptEditOriginal;
                Routine.Rungs[rungIndex].Type = RungTypeEnum.Edit;
                Routine.Rungs[rungIndex + 1].Type = RungTypeEnum.EditOriginal;
                UpdateLadderGraph(rungIndex);
                RecordChange(oldRungs);
            }

            //if (rung.EditState == EditState.AcceptInsert)
            //{
            //    var routineRung = Routine.Rungs[rungIndex];
            //    Routine.Rungs.Insert(rungIndex + 1, routineRung.Clone());
            //    Routine.Rungs[rungIndex].Type = RungTypeEnum.Insert;
            //    Routine.Rungs[rungIndex + 1].Type = RungTypeEnum.AcceptInsertOriginal;
            //    OnlineUpdateLadderGraph(rungIndex);
            //}
        }

        bool CanExecuteOnlineEdit()
        {
            if (!_controller.IsOnline)
                return false;

            if (_controller.KeySwitchPosition == ControllerKeySwitch.RunKeySwitch)
                return false;

            IRung rung = GetCurRung();
            if (rung == null || rung.IsEndRung)
                return false;

            if (rung.EditState == EditState.Normal)//|| rung.EditState == EditState.AcceptEdit || rung.EditState == EditState.AcceptInsert
            {
                return true;
            }

            return false;
        }

        public RelayCommand AcceptEdit { get; }

        private void ExecuteAcceptEdit()
        {
            //先进行语法分析
            RLLGrammarAnalysis grammarAnalysis = new RLLGrammarAnalysis();
            _outputService?.RemoveMessage(Routine);
            _outputService?.RemoveWarning(Routine);
            _outputService?.RemoveError(Routine, Routine.CurrentOnlineEditType);
            grammarAnalysis.Analysis(Routine);
            foreach (var error in grammarAnalysis.Errors)
            {
                if (error.Level == Level.Error)
                    _outputService?.AddErrors(error.Info, OrderType.None, OnlineEditType.Original, error.RungIndex,
                        error.Offset, Routine);
                if (error.Level == Level.Warning)
                    _outputService?.AddWarnings(error.Info, Routine, error.RungIndex,
                        error.Offset);
            }
            if (grammarAnalysis.Errors.Any())
                return;

            var rungs = Routine.Rungs;

            var needHandle = rungs.Exists(p => p.Type != RungTypeEnum.Normal);

            if (needHandle)
            {
                //逐一Accept Edit，然后Finalize
                int rungIndex = 0;
                while (rungIndex < rungs.Count)
                {
                    var rung = rungs[rungIndex];
                    if (rung.Type == RungTypeEnum.Edit)
                    {
                        Debug.Assert(rungs.Count > rungIndex + 1);
                        //该行置为Normal，删除下一行
                        rungs[rungIndex].Type = RungTypeEnum.Normal;
                        rungs.RemoveAt(rungIndex + 1);
                        rungIndex++;
                    }
                    else if (rung.Type == RungTypeEnum.Delete)
                    {
                        //删除该行
                        rungs.RemoveAt(rungIndex);
                    }
                    else if (rung.Type == RungTypeEnum.Insert)
                    {
                        //该行置为Normal
                        rungs[rungIndex].Type = RungTypeEnum.Normal;
                        rungIndex++;
                    }
                    else
                    {
                        rungIndex++;
                    }
                }

                UpdateLadderGraph();

                //Update Routine
                ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
                {
                    OnlineEditHelper helper = new OnlineEditHelper(_controller.CipMessager);

                    //Update Routine
                    await helper.ReplaceRoutine(Routine);

                    await helper.UpdateProgram(Routine.ParentCollection.ParentProgram as Program);
                    //Notifications.Publish(new MessageData() { Object = routines2Update, Type = MessageData.MessageType.UpdateLadderGraph });
                });

                //Accept后，清空Stack
                ClearChange();
            }
            //IRung rung = GetCurRung();
            //if (rung == null)
            //    return;

            //int rungIndex = rung.RungIndex;

            //var codeText = LadderHelper.GetText(rung);
            //RLLGrammarAnalysis grammarAnalysis = new RLLGrammarAnalysis();
            //grammarAnalysis.SetStartRungIndex(rung.RungIndex);
            //grammarAnalysis.Analysis(codeText, _controller, Routine);

            //_outputService?.RemoveMessage(Routine);
            //_outputService?.RemoveError(Routine, Routine.CurrentOnlineEditType);
            //foreach (var error in grammarAnalysis.Errors)
            //{
            //    if (error.Level == Level.Error)
            //        _outputService?.AddErrors(error.Info, OrderType.None, OnlineEditType.Original, error.RungIndex,
            //            null, Routine);
            //    if (error.Level == Level.Warning)
            //        _outputService?.AddWarnings(error.Info, Routine);
            //}

            //if (grammarAnalysis.Errors.Any())
            //    return;

            //if (rung.EditState == EditState.Edit)
            //{
            //    //原行号置为I，复制到下一行后置为R
            //    Routine.Rungs[rungIndex].Type = RungTypeEnum.AcceptEdit;
            //    Routine.Rungs[rungIndex + 1].Type = RungTypeEnum.AcceptEditOriginal;

            //    OnlineUpdateLadderGraph(rungIndex);
            //}

            //if (rung.EditState == EditState.Insert)
            //{
            //    //直接置为I
            //    Routine.Rungs[rungIndex].Type = RungTypeEnum.AcceptInsert;
            //    if (Routine.Rungs.Count > rungIndex + 1 &&
            //        Routine.Rungs[rungIndex + 1].Type == RungTypeEnum.AcceptInsertOriginal)
            //    {
            //        Routine.Rungs.RemoveAt(rungIndex + 1);
            //    }

            //    OnlineUpdateLadderGraph(rungIndex);
            //}

            //if (rung.EditState == EditState.Delete)
            //{
            //    //置为D
            //    Routine.Rungs[rungIndex].Type = RungTypeEnum.AcceptDelete;
            //    OnlineUpdateLadderGraph(rungIndex);
            //}

            //if (rung.EditState == EditState.DeleteAcceptEdit || rung.EditState == EditState.DeleteAcceptInsert)
            //{
            //    //删除dI，并将下一行置为Normal
            //    Routine.Rungs.RemoveAt(rungIndex);
            //    Routine.Rungs[rungIndex].Type = RungTypeEnum.Normal;
            //    OnlineUpdateLadderGraph(rungIndex);
            //}

            //if (rung.EditState == EditState.EditAcceptEdit)
            //{
            //    //删除下一行，并将i置为I
            //    Routine.Rungs.RemoveAt(rungIndex + 1);
            //    Routine.Rungs[rungIndex].Type = RungTypeEnum.AcceptEdit;
            //    OnlineUpdateLadderGraph(rungIndex);
            //}
        }

        bool CanExecuteAcceptEdit()
        {
            if (!_controller.IsOnline)
                return false;

            if (_controller.KeySwitchPosition == ControllerKeySwitch.RunKeySwitch)
                return false;

            return Routine.Rungs.Exists(q => q.Type != RungTypeEnum.Normal);
            //IRung rung = GetCurRung();
            //if (rung == null || rung.IsEndRung)
            //    return false;

            //if (rung.EditState == EditState.Edit || rung.EditState == EditState.Insert || rung.EditState == EditState.Delete)//|| rung.EditState == EditState.DeleteAcceptEdit || rung.EditState == EditState.DeleteAcceptInsert || rung.EditState == EditState.EditAcceptEdit
            //{
            //    return true;
            //}

            //return false;
        }

        public RelayCommand CancelEdit { get; }

        private void ExecuteCancelEdit()
        {
            IRung rung = GetCurRung();
            if (rung == null)
                return;

            var oldRungs = Routine.CloneRungs();
            int rungIndex = rung.RungIndex;

            if (rung.EditState == EditState.Edit)
            {
                //删除i，并将下一行置为Normal
                Routine.Rungs.RemoveAt(rungIndex);
                Routine.Rungs[rungIndex].Type = RungTypeEnum.Normal;
                UpdateLadderGraph(rungIndex);
                RecordChange(oldRungs);
            }

            if (rung.EditState == EditState.Insert)
            {
                //删除i
                Routine.Rungs.RemoveAt(rungIndex);
                UpdateLadderGraph(rungIndex);
                RecordChange(oldRungs);
            }

            if (rung.EditState == EditState.Delete)
            {
                //置为Normal
                Routine.Rungs[rungIndex].Type = RungTypeEnum.Normal;
                UpdateLadderGraph(rungIndex);
                RecordChange(oldRungs);
            }

            //if (rung.EditState == EditState.DeleteAcceptEdit)
            //{
            //    //dI置为I
            //    Routine.Rungs[rungIndex].Type = RungTypeEnum.AcceptEdit;
            //    OnlineUpdateLadderGraph(rungIndex);
            //}

            //if (rung.EditState == EditState.DeleteAcceptInsert)
            //{
            //    //dI置为I
            //    Routine.Rungs[rungIndex].Type = RungTypeEnum.AcceptInsert;
            //    OnlineUpdateLadderGraph(rungIndex);
            //}

            //if (rung.EditState == EditState.EditAcceptEdit)
            //{
            //    //删除i，并将下一行置为I
            //    Routine.Rungs.RemoveAt(rungIndex);
            //    Routine.Rungs[rungIndex].Type = RungTypeEnum.AcceptEdit;
            //    OnlineUpdateLadderGraph(rungIndex);
            //}
        }

        bool CanExecuteCancelEdit()
        {
            if (!_controller.IsOnline)
                return false;

            //if (_controller.KeySwitchPosition == ControllerKeySwitch.RunKeySwitch)
            //    return false;

            IRung rung = GetCurRung();
            if (rung == null || rung.IsEndRung)
                return false;

            if (rung.EditState == EditState.Edit || rung.EditState == EditState.Delete || rung.EditState == EditState.Insert)//|| rung.EditState == EditState.DeleteAcceptEdit  || rung.EditState == EditState.DeleteAcceptInsert || rung.EditState == EditState.EditAcceptEdit
            {
                return true;
            }

            return false;
        }

        //public RelayCommand CancelAcceptedEdit { get; }

        //private void ExecuteCancelAcceptedEdit()
        //{
        //    var routines = Routine.ParentCollection;
        //    List<IRoutine> routines2Update = new List<IRoutine>();

        //    foreach (var iRoutine in routines)
        //    {
        //        RLLRoutine routine = iRoutine as RLLRoutine;
        //        if (routine == null)
        //            continue;

        //        var rungs = routine.Rungs;

        //        //var needHandle = rungs.Exists(p =>
        //        //    p.Type == RungTypeEnum.AcceptEdit || p.Type == RungTypeEnum.DeleteAcceptEdit ||
        //        //    p.Type == RungTypeEnum.AcceptDelete || p.Type == RungTypeEnum.AcceptInsert ||
        //        //    p.Type == RungTypeEnum.DeleteAcceptInsert || p.Type == RungTypeEnum.EditAcceptEdit);
        //        //if (needHandle)
        //        //    routines2Update.Add(iRoutine);

        //        //int rungIndex = 0;
        //        //while (rungIndex < rungs.Count)
        //        //{
        //        //    var rung = rungs[rungIndex];
        //        //    if (rung.Type == RungTypeEnum.AcceptEdit || rung.Type == RungTypeEnum.DeleteAcceptEdit)
        //        //    {
        //        //        //删除该行，下一行置为Normal
        //        //        rungs.RemoveAt(rungIndex);

        //        //        if (rungIndex < rungs.Count)
        //        //            rungs[rungIndex].Type = RungTypeEnum.Normal;
        //        //        rungIndex++;
        //        //    }
        //        //    else if (rung.Type == RungTypeEnum.AcceptDelete)
        //        //    {
        //        //        //该行置为Normal
        //        //        rungs[rungIndex].Type = RungTypeEnum.Normal;
        //        //        rungIndex++;
        //        //    }
        //        //    else if (rung.Type == RungTypeEnum.AcceptInsert || rung.Type == RungTypeEnum.DeleteAcceptInsert)
        //        //    {
        //        //        if (rungs.Count > rungIndex + 1 && rungs[rungIndex + 1].Type == RungTypeEnum.AcceptInsertOriginal)
        //        //        {
        //        //            rungs.RemoveAt(rungIndex);
        //        //            rungs[rungIndex].Type = RungTypeEnum.AcceptInsert;
        //        //        }
        //        //        //删除该行
        //        //        else
        //        //            rungs.RemoveAt(rungIndex);
        //        //    }
        //        //    else if (rung.Type == RungTypeEnum.EditAcceptEdit)
        //        //    {
        //        //        //删除下一行，R置为r
        //        //        rungs[rungIndex].Type = RungTypeEnum.Edit;
        //        //        rungs.RemoveAt(rungIndex + 1);
        //        //        rungs[rungIndex + 1].Type = RungTypeEnum.EditOriginal;
        //        //        rungIndex += 2;
        //        //    }
        //        //    else if (rung.Type == RungTypeEnum.Insert && rungs.Count > rungIndex + 1 && rungs[rungIndex + 1].Type == RungTypeEnum.AcceptInsertOriginal)
        //        //    {
        //        //        //编辑已提交的插入；删除下一行
        //        //        rungs.RemoveAt(rungIndex + 1);
        //        //        rungIndex++;
        //        //    }
        //        //    else
        //        //    {
        //        //        rungIndex++;
        //        //    }
        //        //}
        //    }

        //    //Notifications.Publish(new MessageData() { Object = routines2Update, Type = MessageData.MessageType.UpdateLadderGraph });

        //    RefreshExecutable();
        //}

        //bool CanExecuteCancelAcceptedEdit()
        //{
        //    return false;
        //    //if (!_controller.IsOnline)
        //    //    return false;

        //    //if (_controller.KeySwitchPosition == ControllerKeySwitch.RunKeySwitch)
        //    //    return false;

        //    //var routines = Routine.ParentCollection;
        //    //var acceptedEditRoutine = routines.FirstOrDefault(p =>
        //    //{
        //    //    if (p is RLLRoutine)
        //    //    {
        //    //        var routine = p as RLLRoutine;

        //    //        return routine.Rungs.Exists(q => q.Type == RungTypeEnum.AcceptEdit || q.Type == RungTypeEnum.AcceptDelete || q.Type == RungTypeEnum.AcceptInsert || q.Type == RungTypeEnum.DeleteAcceptEdit || q.Type == RungTypeEnum.DeleteAcceptInsert || q.Type == RungTypeEnum.EditAcceptEdit || q.Type == RungTypeEnum.AcceptInsertOriginal);
        //    //    }

        //    //    return false;
        //    //});

        //    //return acceptedEditRoutine != null;
        //}

        public RelayCommand FinalizeEdit { get; }

        private void ExecuteFinalizeEdit()
        {
            var routines = Routine.ParentCollection;

            //1.进行语法分析
            RLLGrammarAnalysis grammarAnalysis = new RLLGrammarAnalysis();

            foreach (var iRoutine in routines)
            {
                RLLRoutine routine = iRoutine as RLLRoutine;
                if (routine == null)
                    continue;

                _outputService?.RemoveMessage(routine);
                _outputService?.RemoveWarning(routine);
                _outputService?.RemoveError(routine, routine.CurrentOnlineEditType);
                grammarAnalysis.Analysis(routine);
            }

            foreach (var error in grammarAnalysis.Errors)
            {
                if (error.Level == Level.Error)
                    _outputService?.AddErrors(error.Info, OrderType.None, OnlineEditType.Original, error.RungIndex,
                        error.Offset, Routine);
                if (error.Level == Level.Warning)
                    _outputService?.AddWarnings(error.Info, Routine, error.RungIndex,
                        error.Offset);
            }

            if (grammarAnalysis.Errors.Any())
                return;

            //2.编译
            var program = Routine.ParentCollection.ParentProgram as Program;
            bool compilePassed = false;
            try
            {
                program?.GenCode();
                compilePassed = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            if (!compilePassed)
            {
                _outputService?.AddMessages("Complie Failed!", program);
                return;
            }

            List<IRoutine> routines2Update = new List<IRoutine>();

            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                OnlineEditHelper helper = new OnlineEditHelper(_controller.CipMessager);

                foreach (var iRoutine in routines)
                {
                    RLLRoutine routine = iRoutine as RLLRoutine;
                    if (routine == null)
                        continue;

                    var rungs = routine.Rungs;

                    var needHandle = rungs.Exists(p => p.Type != RungTypeEnum.Normal);

                    if (needHandle)
                    {
                        routines2Update.Add(iRoutine);

                        //逐一Accept Edit，然后Finalize
                        int rungIndex = 0;
                        while (rungIndex < rungs.Count)
                        {
                            var rung = rungs[rungIndex];
                            if (rung.Type == RungTypeEnum.Edit)// || rung.Type == RungTypeEnum.AcceptEdit
                            {
                                Debug.Assert(rungs.Count > rungIndex + 1);
                                //该行置为Normal，删除下一行
                                rungs[rungIndex].Type = RungTypeEnum.Normal;
                                rungs.RemoveAt(rungIndex + 1);
                                rungIndex++;
                            }
                            //else if (rung.Type == RungTypeEnum.DeleteAcceptEdit)
                            //{
                            //    //删除该行，下一行置为Normal
                            //    rungs.RemoveAt(rungIndex);

                            //    //if (rungIndex < rungs.Count)
                            //    rungs[rungIndex].Type = RungTypeEnum.Normal;
                            //    rungIndex++;
                            //}
                            else if (rung.Type == RungTypeEnum.Delete)// || rung.Type == RungTypeEnum.AcceptDelete || rung.Type == RungTypeEnum.DeleteAcceptInsert
                            {
                                //删除该行
                                rungs.RemoveAt(rungIndex);
                            }
                            else if (rung.Type == RungTypeEnum.Insert)//|| rung.Type == RungTypeEnum.AcceptInsert
                            {
                                //该行置为Normal
                                rungs[rungIndex].Type = RungTypeEnum.Normal;
                                //if (rungs.Count > rungIndex + 1 &&
                                //    rungs[rungIndex + 1].Type == RungTypeEnum.AcceptInsertOriginal)
                                //{
                                //    rungs.RemoveAt(rungIndex + 1);
                                //}
                                rungIndex++;
                            }
                            //else if (rung.Type == RungTypeEnum.EditAcceptEdit)
                            //{
                            //    Debug.Assert(rungs.Count > rungIndex + 2);
                            //    //该行置为Normal，删除下两行
                            //    rungs[rungIndex].Type = RungTypeEnum.Normal;
                            //    rungs.RemoveAt(rungIndex + 1);
                            //    rungs.RemoveAt(rungIndex + 1);
                            //    rungIndex++;
                            //}
                            else
                            {
                                rungIndex++;
                            }
                        }

                        //Update Routine
                        await helper.ReplaceRoutine(routine);
                    }
                }



                await helper.UpdateProgram(Routine.ParentCollection.ParentProgram as Program);

                Notifications.Publish(new MessageData() { Object = routines2Update, Type = MessageData.MessageType.UpdateLadderGraph });
            });
        }

        bool CanExecuteFinalizeEdit()
        {
            if (!_controller.IsOnline)
                return false;

            if (_controller.KeySwitchPosition == ControllerKeySwitch.RunKeySwitch)
                return false;

            var routines = Routine.ParentCollection;
            var editRoutine = routines.FirstOrDefault(p =>
            {
                if (p is RLLRoutine)
                {
                    var routine = p as RLLRoutine;
                    return routine.Rungs.Exists(q => q.Type != RungTypeEnum.Normal);
                }

                return false;
            });

            return editRoutine != null;
        }

        public void RefreshExecutable()
        {
            OnlineEdit.RaiseCanExecuteChanged();
            AcceptEdit.RaiseCanExecuteChanged();
            CancelEdit.RaiseCanExecuteChanged();
            //CancelAcceptedEdit.RaiseCanExecuteChanged();
            FinalizeEdit.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
            CutCommand.RaiseCanExecuteChanged();
            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();
        }

        private void OnlineDelete()
        {
            if (_controller.KeySwitchPosition == ControllerKeySwitch.RunKeySwitch)
                return;

            IRung rung = GetCurRung();
            if (rung == null)
                return;

            int rungIndex = rung.RungIndex;
            if (rung.IsEndRung)
                return;

            FocusInfo info = new FocusInfo { RungIndex = -1, Offset = -1 };
            var oldRungs = Routine.CloneRungs();

            bool selIsRung = Graph.FocusedItem is IRung;

            if (selIsRung)
            {
                var selElements = LeftPowerRailStyleRenderer.GetSelRungs();
                var selModels = selElements.Select(p => p.Model as ILadderItem);

                var firstRung = selModels.Select(p => p as IRung).ToList().OrderBy(q => q.RungIndex).FirstOrDefault();
                info.RungIndex = firstRung.RungIndex;

                foreach (var selModel in selModels)
                {
                    var selRung = selModel as IRung;
                    if (selRung == null || selRung.IsEndRung)
                        continue;

                    var selRungIndex = selRung.RungIndex;
                    if (Routine.Rungs[selRungIndex].Type == RungTypeEnum.Normal)
                        Routine.Rungs[selRungIndex].Type = RungTypeEnum.Delete;
                    else if (Routine.Rungs[selRungIndex].Type == RungTypeEnum.Insert)
                        Routine.Rungs[selRungIndex].Mark = EditMark.Delete;
                    else if (Routine.Rungs[selRungIndex].Type == RungTypeEnum.Edit)
                    {
                        Routine.Rungs[selRungIndex].Mark = EditMark.Delete;
                        Routine.Rungs[selRungIndex + 1].Type = RungTypeEnum.Normal;
                    }
                    //else if (Routine.Rungs[selRungIndex].Type == RungTypeEnum.AcceptEdit)
                    //    Routine.Rungs[selRungIndex].Type = RungTypeEnum.DeleteAcceptEdit;
                    //else if (Routine.Rungs[selRungIndex].Type == RungTypeEnum.AcceptInsert)
                    //    Routine.Rungs[selRungIndex].Type = RungTypeEnum.DeleteAcceptInsert;
                    //else if (rung.EditState == EditState.EditAcceptEdit)
                    //{
                    //    //删除正在编辑的已提交过的编辑
                    //    Routine.Rungs[selRungIndex].Mark = EditMark.Delete;
                    //    Routine.Rungs[selRungIndex + 1].Type = RungTypeEnum.AcceptEdit;
                    //}
                }

                //删除标记的Rung
                int index = 0;
                while (index < Routine.Rungs.Count)
                {
                    var curRung = Routine.Rungs[index];
                    if (curRung.Mark == EditMark.Delete)
                        Routine.Rungs.RemoveAt(index);
                    else
                        index++;
                }

                UpdateLadderGraph(info);
                RecordChange(oldRungs);
            }
            else if (rung.EditState == EditState.Edit || rung.EditState == EditState.Insert)//|| rung.EditState == EditState.EditAcceptEdit
            {
                //只能删除编辑状态的rung的元素
                string newText = BoxInstructionStyleRenderer.DeleteOnline();

                if (string.IsNullOrEmpty(newText) || newText == "UnFocused" || newText == "UnSelected")
                    return;

                //bool result = UpdateRungsByText(deletedText);
                //if (!result)
                //   Routine.UpdateRungs(_ldtTextBackup);
                Routine.Rungs[rungIndex].Text = newText;

                UpdateLadderGraph();
                RecordChange(oldRungs);
            }
        }

        private void OnlinePast(string clipStr)
        {
            if (_controller.KeySwitchPosition == ControllerKeySwitch.RunKeySwitch)
                return;

            IRung rung = GetCurRung();
            if (rung == null)
                return;

            var oldRungs = Routine.CloneRungs();
            FocusInfo info = new FocusInfo { RungIndex = -1, Offset = -1 };
            int rungIndex = rung.RungIndex;
            var rungs = Routine.Rungs;

            bool copyIsRung = clipStr.Contains(";");

            //以下几种情况，不能粘贴行
            if (rungs.Count > 1 && copyIsRung)
            {
                //当所选rung为Edit、EditAcceptEdit时，不可插入rung
                if (rung.EditState == EditState.Edit)// || rung.EditState == EditState.EditAcceptEdit || rung.EditState == EditState.AcceptEdit || rung.EditState == EditState.DeleteAcceptEdit || rung.EditState == EditState.EditAcceptEditOriginal
                    return;

                //if (rung.EditState == EditState.EditOriginal && rungs[rungIndex - 1].Type == RungTypeEnum.EditAcceptEdit)
                //    return;

                //if (rung.EditState == EditState.Insert && rungs.Count > rungIndex + 1 &&
                //    rungs[rungIndex + 1].Type == RungTypeEnum.AcceptInsertOriginal)
                //    return;
            }
            info.RungIndex = copyIsRung ? rungIndex + 1 : rungIndex;
            string newText = BoxInstructionStyleRenderer.GenerateTextWithCopy(clipStr);

            if (string.IsNullOrEmpty(newText))
                return;

            List<string> newCodes = newText.Trim().Split(new[] { "   " }, StringSplitOptions.RemoveEmptyEntries).ToList();

            Debug.Assert(rungIndex <= rungs.Count);

            if (newCodes.Count > 1)
            {
                //插入Rung
                if (rung.IsEndRung)
                {
                    rungs.AddRange(newCodes.Select(p => new RungType { Text = p, Type = RungTypeEnum.Insert }));
                }
                else
                    rungs.InsertRange(rungIndex + 1, newCodes.GetRange(1, newCodes.Count - 1).Select(p => new RungType { Text = p, Type = RungTypeEnum.Insert }));//new RungType { Text = newCodes[1], Type = RungTypeEnum.Insert }
            }
            else if (newCodes.Count == 1)
            {
                if (rung.IsEndRung)
                {
                    //插入到EndRung前面
                    if (copyIsRung)
                    {
                        rungs.Add(new RungType { Text = newCodes[0], Type = RungTypeEnum.Insert });
                        info.RungIndex = rungIndex;
                    }
                }
                else
                {
                    //修改Rung
                    rungs[rungIndex].Text = newCodes[0];

                    var focusItem = Graph.FocusedItem;
                    if (focusItem is IInstruction || focusItem is IBranchLevel)
                        info.Offset = focusItem.Offset + 1;

                    if (focusItem is IBranch)
                        info.Offset = OffsetHelper.GetBranchLastOffset((IBranch)focusItem) + 1;
                }
            }
            else
                return;

            UpdateLadderGraph(info);
            RecordChange(oldRungs);
        }

        #endregion

        public RelayCommand TestCommand { get; }

        private void ExecuteTest()
        {
            //BoxInstructionStyleRenderer.UpdateTag("speed1", $"{i++}");
        }

        public void Consume(MessageData message)
        {
            //if (message.Type == MessageData.MessageType.PullFinished)
            //{
            //    operands.ForEach(p =>
            //    {
            //        string tagName = p.GeTagExpression().ToString();
            //        string tagVal = p.FormattedValueString;
            //        BoxInstructionStyleRenderer.UpdateTag(tagName, tagVal);
            //    });
            //}
            if (message.Type == MessageData.MessageType.UpdateLadderGraph)
            {
                var data = message.Object as List<IRoutine>;
                if (data != null && data.Contains(Routine))
                {
                    UpdateLadderGraph();
                    ClearChange();
                }
            }
        }

        public void MonitorTag(string tag)
        {
            string tagName = GetTagName(tag);
            ITagCollectionContainer container = null;
            if (_localContainer.Tags.Any(p => p.Name == tagName))
                container = _localContainer;
            else if (_globalContainer.Tags.Any(p => p.Name == tagName))
                container = _globalContainer;

            if (container == null)
                return;

            var createEditorService =
                Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;

            createEditorService?.CreateMonitorEditTags(_controller, container, tag);
        }

        public void NewTag(string tag, string operandName, string dataType)
        {
            //var aoiDefinition = _controller.AOIDefinitionCollection as AoiDefinitionCollection;
            //var aoi = aoiDefinition?.Find(dataType);
            //ITagCollectionContainer container = null;
            var collection = Routine.ParentCollection.ParentProgram.Tags;
            bool? result = null;
            if (SelectedReference == null)
            {
                NewTagViewModel viewModel = new NewTagViewModel(dataType, _localContainer.Tags, Usage.NullParameterType, null, tag, false);
                result = new NewTagDialog(viewModel)
                {
                    Owner = Application.Current.MainWindow
                }.ShowDialog();
                //container = viewModel.TagCollectionContainer;
            }
            else if (!_isAOIRefeferencePage)
            {
                //SelectedReference.InnerAoiDefinition
                NewAoiTagViewModel viewModel = new NewAoiTagViewModel(dataType, Routine.ParentCollection.ParentProgram as IAoiDefinition, Usage.NullParameterType, false, tag);
                result = new NewAoiTagDialog(viewModel)
                {
                    Owner = Application.Current.MainWindow
                }.ShowDialog();
                //container = viewModel.AoiCollectionContainer;
            }

            if (result == true)
            {
                ITagCollection curCollection = null;
                if (collection.Any(p => p.Name == tag))
                    curCollection = collection;
                else if (_controller.Tags.Any(p => p.Name == tag))
                    curCollection = _controller.Tags;
                if (!operands.Exists(p =>
                    p.GetTagExpression()?.ToString() == operandName && p.TagCollection == curCollection))
                {
                    var data = _dataServer.CreateDataOperand(curCollection, operandName);
                    data.StartMonitoring(true, true);
                    operands.Add(data);
                    if (data.DataTypeInfo.ToString().Equals("Control", StringComparison.OrdinalIgnoreCase))
                    {
                        data = _dataServer.CreateDataOperand(collection, $"{operandName}.LEN");
                        data.StartMonitoring(true, true);
                        operands.Add(data);
                        data = _dataServer.CreateDataOperand(collection, $"{operandName}.POS");
                        data.StartMonitoring(true, true);
                        operands.Add(data);
                    }
                    else if (data.DataTypeInfo.ToString().Equals("TIMER", StringComparison.OrdinalIgnoreCase) ||
                             data.DataTypeInfo.ToString().Equals("COUNTER", StringComparison.OrdinalIgnoreCase))
                    {
                        data = _dataServer.CreateDataOperand(collection, $"{operandName}.PRE");
                        data.StartMonitoring(true, true);
                        operands.Add(data);
                        data = _dataServer.CreateDataOperand(collection, $"{operandName}.ACC");
                        data.StartMonitoring(true, true);
                        operands.Add(data);
                    }
                }

                UpdateLadderGraph();
            }
        }

        private string GetTagName(string operand)
        {
            if (!operand.Contains('.') && !operand.Contains('['))
                return operand;

            int index = operand.IndexOf('[');
            if (index >= 0)
            {
                //含[
                operand = operand.Substring(0, index);
            }

            operand = operand.Contains('.') ? operand.Split('.')[0] : operand;

            return operand;
        }

        public RelayCommand EditRungCommand { get; }

        private void ExecuteEditRung()
        {
            if (_ldtEditorVisible == Visibility.Visible)
            {
                ((LadderControl)BottomControl).Focus();
                ExecuteFinishLdtEdit("true");
                return;
            }

            var rung = Graph.FocusedItem as IRung;
            if (rung == null || rung.IsEndRung)
                return;

            //var text = LadderHelper.GetText(rung);
            ShowRungText(rung);
        }

        public RelayCommand EditRungCommentCommand { get; }
        private void ExecuteEditRungComment()
        {
            var rung = Graph.FocusedItem as IRung;

            if (rung != null)
            {
                if (rung.IsEndRung)
                    return;
                EditingControl = null;
                var comment = rung.RungComment;
                var layer = AdornerLayer.GetAdornerLayer(_mainGrid);
                try
                {
                    if (!HasAdorner(layer, InputAdorner))
                        layer?.Add(InputAdorner);

                    InputAdorner.Resize();
                    InputAdorner.ResetAdorner(_original, 1, comment);//new Point(500,500)
                    InputAdorner.SetTextFocus();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message + e.StackTrace);
                }
            }

            var instruction = Graph.FocusedItem as IInstruction;
            if (instruction != null)
            {
                EditingControl = null;
                var description = GetMainOperandDescription(instruction);
                var layer = AdornerLayer.GetAdornerLayer(_mainGrid);
                try
                {
                    if (!HasAdorner(layer, InputAdorner))
                        layer?.Add(InputAdorner);

                    InputAdorner.Resize();
                    InputAdorner.ResetAdorner(_original, 1, description);//new Point(500,500)
                    InputAdorner.SetTextFocus();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message + e.StackTrace);
                }
            }
        }

        public RelayCommand ImportRungsCommand { get; }

        private void ExecuteImportRungs()
        {
            if (CanExecuteImportRungs())
            {
                var selElements = LeftPowerRailStyleRenderer.GetSelRungs();
                var selModels = selElements.Select(p => p.Model as IRung).ToList().OrderBy(r => r.RungIndex).ToList();
                var startIndex = selModels.FirstOrDefault()?.RungIndex ?? -1;
                var endIndex = selModels.LastOrDefault()?.RungIndex ?? -1;

                var projectInfoService = Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
                var isSuccess = projectInfoService?.ImportData(ProjectItemType.Routine, Routine, startIndex, endIndex);
                if (isSuccess ?? false)
                {
                    ClearChange();
                    UpdateAoiInstructionDefinition(_controller);
                    UpdateLadderGraph();
                }
            }
        }

        private bool CanExecuteImportRungs()
        {
            var selElements = LeftPowerRailStyleRenderer.GetSelRungs();
            var selModels = selElements.Select(p => p.Model as IRung).ToList().OrderBy(r => r.RungIndex).ToList();
            var startIndex = selModels.FirstOrDefault()?.RungIndex ?? -1;
            var endIndex = selModels.LastOrDefault()?.RungIndex ?? -1;
            var rungs = Routine.CloneRungs();
            if (startIndex < 0 || endIndex > rungs.Count || startIndex > endIndex) return false;

            var index = startIndex;
            foreach (var item in selModels) if (item.RungIndex != index++) return false;

            return true;
        }

        public RelayCommand ExportRungsCommand { get; }

        private void ExecuteExportRungs()
        {
            if (CanExecuteExportRungs())
            {
                var selElements = LeftPowerRailStyleRenderer.GetSelRungs();
                var selModels = selElements.Select(p => p.Model as IRung).ToList().OrderBy(r => r.RungIndex).ToList();
                var startIndex = selModels.FirstOrDefault()?.RungIndex ?? -1;
                var endIndex = selModels.LastOrDefault()?.RungIndex ?? -1;

                var controllerOrganizerService =
                    Package.GetGlobalService(typeof(SControllerOrganizerService)) as IControllerOrganizerService;
                controllerOrganizerService?.RungsExport(Routine, startIndex, endIndex);
            }
        }

        private bool CanExecuteExportRungs()
        {
            var selElements = LeftPowerRailStyleRenderer.GetSelRungs();
            var selModels = selElements.Select(p => p.Model as IRung).ToList().OrderBy(r => r.RungIndex).ToList();
            var startIndex = selModels.FirstOrDefault()?.RungIndex ?? -1;
            var endIndex = selModels.LastOrDefault()?.RungIndex ?? -1;
            var rungs = Routine.CloneRungs();
            if (startIndex < 0 || endIndex > rungs.Count || startIndex > endIndex) return false;

            var index = startIndex;
            foreach (var item in selModels) if (item.RungIndex != index++) return false;

            return true;
        }

        public RelayCommand AddRungCommand { get; }

        private void ExecuteAddRung()
        {
            UpdateGraph(Graph, ";;;");
        }

        public void AddBranchLevel(IBranch branch)
        {
            var oldRungs = Routine.CloneRungs();
            var rung = branch.GetParentRung();
            string newText = LadderHelper.GetTextAfterAddBranchLevel(branch);
            Routine.Rungs[rung.RungIndex].Text = newText;
            FocusInfo info = new FocusInfo();
            info.RungIndex = rung.RungIndex;
            info.Offset = OffsetHelper.GetBranchLastOffset(branch) + 1;
            UpdateLadderGraph(info);
            RecordChange(oldRungs);
        }

        public RelayCommand ToggleCommand { get; }

        private void ExecuteToggle()
        {
            var model = Graph.FocusedItem;//BoxInstructionStyleRenderer.focusElement?.Model;
            List<string> bitCommands = new List<string>() { "XIC", "XIO", "OTE", "OTU", "OTL" };
            var inst = model as IInstruction;
            if (inst != null && bitCommands.Contains(inst.Mnemonic))
            {
                ToggleBit(inst.Parameters[0]);
            }
        }

        public RelayCommand NewTagCommand { get; }

        private void ExecuteNewTag()
        {
            var tb = Graph.FocusTextBlock; //BoxInstructionStyleRenderer.focusElement;
            var instruction = Graph.FocusedItem as IInstruction;
            if (tb == null || instruction == null)
                return;

            if (!(tb.Tag is int))
                return;

            string tag = tb.Text;
            var index = (int)tb.Tag;

            double result;
            if (double.TryParse(tb.Text, out result))
                return;

            InstructionParameter para = instruction.Definition.Parameters.Where(p => !p.Type.Contains("Target") || (p.Type == "ReadWrite_DataTarget" && p.DataType == "SINT")).ToList()[index];
            var operand = GetOperand(tag);

            if (para == null)
                return;

            bool isExisted = (operand != null);
            if (!isExisted)
            {
                if (para.DataType.Equals("routine", StringComparison.OrdinalIgnoreCase))
                {
                    if (Routine.ParentCollection[tag] != null)
                        isExisted = true;
                }
            }

            var simpleTag = GetSimpleTag(tag);
            string dataType = simpleTag == tag ? para.DataType : "DINT";
            if (!isExisted)
                NewTag(simpleTag, tag, dataType);
        }

        public void UpdateTagVal(string tag, string value)
        {
            string tagName = GetTagName(tag);
            var item = _localContainer.Tags.FirstOrDefault(p => p.Name == tagName) ??
                       _globalContainer.Tags.FirstOrDefault(p => p.Name == tagName);

            if (item == null)
                return;

            ITagCollectionContainer container = null;
            if (_localContainer.Tags.Any(p => p.Name == tagName))
                container = _localContainer;
            else if (_globalContainer.Tags.Any(p => p.Name == tagName))
                container = _globalContainer;

            if (container == null)
                return;

            var operand = operands.FirstOrDefault(p =>
                p.GetTagExpression()?.ToString() == tag && p.TagCollection == container.Tags);
            if (operand == null)
                return;

            bool isOnline = _controller.IsOnline;

            //if (item.DataTypeInfo.DataType.IsAtomic || operand.IsAtomic)
            //{
            //var value = operand.BoolValue ? 0 : 1;

            if (isOnline)
            {
                SetTagValueToPLC(item, tag, value);
            }
            else
            {
                operand.SetValue(value);
            }

            //}
            //UpdateLadderGraph();
        }

        public void ToggleBit(string tag)
        {
            string tagName = GetTagName(tag);
            var item = _localContainer.Tags.FirstOrDefault(p => p.Name == tagName) ??
                       _globalContainer.Tags.FirstOrDefault(p => p.Name == tagName);

            if (item == null)
                return;

            var operand = operands.FirstOrDefault(p =>
                p.GetTagExpression()?.ToString() == tag && p.TagCollection == item.ParentCollection);
            if (operand == null)
                return;

            ITagCollectionContainer container = null;
            if (_localContainer.Tags.Any(p => p.Name == tagName))
                container = _localContainer;
            else if (_globalContainer.Tags.Any(p => p.Name == tagName))
                container = _globalContainer;

            if (container == null)
                return;
            bool isOnline = _controller.IsOnline;

            if (item.DataTypeInfo.DataType.IsBool || operand.IsBool)
            {
                var boolVal = operand.BoolValue;
                var value = boolVal ? 0 : 1;

                if (isOnline)
                {
                    SetTagValueToPLC(item, tag, value.ToString());
                }
                else
                {
                    operand.SetValue(value.ToString());
                }
            }
        }

        public void GotoReference(string tag, Type type)
        {
            var createEditorService = Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
            if (type == Type.Label)
                createEditorService?.CreateCrossReference(Routine, tag);
            else
                createEditorService?.CreateCrossReference(type,
                    Routine.ParentCollection.ParentProgram, tag);
        }

        private void SetTagValueToPLC(ITag tag, string specifier, string value)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await TaskScheduler.Default;
                await _controller.SetTagValueToPLC(tag, specifier, value);
            });
        }

        readonly Point _original = new Point(0, 0);
        public object EditingControl;

        private void ShowTagBrowser(IInstruction instruction, string tag, object sender)
        {

            var layer = AdornerLayer.GetAdornerLayer(_mainGrid);
            try
            {
                if (instruction == null)
                {
                    var tb = sender as TextBlock;
                    if (tb != null && tb.Tag is IRung)
                    {
                        EditingControl = null;
                        var tagRung = tb.Tag as IRung;
                        Graph.FocusedItem = tagRung;
                        try
                        {
                            if (!HasAdorner(layer, InputAdorner))
                                layer?.Add(InputAdorner);

                            InputAdorner.Resize();
                            InputAdorner.ResetAdorner(_original, 1, tagRung?.RungComment);
                            InputAdorner.SetTextFocus();
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message + e.StackTrace);
                        }
                        //var rung = tb.Tag as IRung;
                        //UpdateRungComment(rung.RungIndex, tag);
                        return;
                    }

                    if (tb != null && tb.Tag is IInstruction)
                    {
                        EditingControl = null;
                        Graph.FocusedItem = tb.Tag as IInstruction;
                        try
                        {
                            if (!HasAdorner(layer, InputAdorner))
                                layer?.Add(InputAdorner);

                            InputAdorner.Resize();
                            InputAdorner.ResetAdorner(_original, 1, tag);
                            InputAdorner.SetTextFocus();
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message + e.StackTrace);
                        }

                        return;
                    }
                }
                var rungState = instruction.GetParentRung().EditState;
                bool isOnlineEditable = !_controller.IsOnline || (rungState == EditState.Edit || rungState == EditState.Insert);// ||rungState == EditState.EditAcceptEdit
                if (sender is VisualGroup)
                {
                    if (!isOnlineEditable)
                        return;

                    //修改指令（Box或Contact的标题）
                    var group = sender as VisualGroup;
                    EditingControl = group;

                    var tb = group.Children[5] as TextBlock;

                    var gt = tb?.TransformToVisual(_mainGrid);
                    var point = gt?.Transform(_original) ?? new Point(0, 33);

                    if (!HasAdorner(layer, BrowseAdorner))
                        layer?.Add(BrowseAdorner);

                    BrowseAdorner.ResetAdorner(point, 1, tag, false);
                    BrowseAdorner.SetCompleteBoxSource(_controller.RLLInstructionCollection.Instructions);
                    BrowseAdorner.SetTextFocus();

                    //if (!HasAdorner(layer, InputAdorner))
                    //    layer?.Add(InputAdorner);

                    //InputAdorner.Resize();
                    //InputAdorner.ResetAdorner(point, 1, tag);
                    //InputAdorner.SetTextFocus();
                }

                if (sender is TextBlock)
                {
                    //修改参数
                    var tb = sender as TextBlock;
                    EditingControl = tb;

                    var gt = tb.TransformToVisual(_mainGrid);
                    var point = gt.Transform(_original);

                    if (tb.Tag is int && instruction != null)
                    {
                        var index = (int)tb.Tag;

                        InstructionDefinition definition =
                            InstructionDefinition.GetDefinition(instruction.Mnemonic);
                        if (definition == null)
                            return;

                        //InstructionParameter para;

                        //if (definition != null)
                        //{
                        var parameters = definition.Parameters.Where(p =>
                            !p.Type.Contains("DataTarget") ||
                            (p.Type == "ReadWrite_DataTarget" && p.DataType == "SINT"));

                        InstructionParameter para = parameters.ToList()[index];
                        //}
                        //else
                        //{
                        //    //AOI
                        //    var aoiCollection = _controller.AOIDefinitionCollection as AoiDefinitionCollection;
                        //    var aoi = aoiCollection?.Find(instruction.Mnemonic);
                        //    var parameters = SimpleServices.CodeAnalysis.RLLRungParser.GetRLLInstructionDefinition(aoi)?.Parameters;

                        //    para = parameters?[index];
                        //}
                        //if ((instruction.Mnemonic == "FAL" || instruction.Mnemonic == "FSC") && index > 1)
                        //    para = definition.Parameters.Where(p => !p.Type.Contains("DataTarget")).ToList()[index - 2];
                        //else
                        //    para = definition.Parameters.Where(p => !p.Type.Contains("DataTarget")).ToList()[index];

                        bool isExpression = para?.Label == "Expression";

                        string interrealated = "";
                        if (para?.IsEnum == true)
                        {
                            if (!isOnlineEditable)
                                return;

                            //枚举类型
                            if ((instruction.Mnemonic == "GSV" || instruction.Mnemonic == "SSV") && index == 2)
                            {
                                interrealated = instruction.Parameters[0];
                            }

                            var enumVals =
                                SimpleServices.Instruction.Utils.GetInstrEnumValues(instruction.Mnemonic, index,
                                    interrealated);
                            if (!HasAdorner(layer, EnumAdorner))
                                layer?.Add(EnumAdorner);

                            EnumAdorner.ResetAdorner(point, 1, tag, enumVals);
                            EnumAdorner.SetTextFocus();
                        }
                        else
                        //是Tag
                        {
                            if (!isOnlineEditable)
                                return;

                            if (isExpression)
                            {
                                if (!HasAdorner(layer, InputAdorner))
                                    layer?.Add(InputAdorner);

                                InputAdorner.Resize(true);
                                InputAdorner.ResetAdorner(point, 1, tag);
                                InputAdorner.SetTextFocus();
                            }
                            else
                            {
                                if (para?.DataType.Equals("routine", StringComparison.OrdinalIgnoreCase) == true)
                                {
                                    var routines = Routine.ParentCollection.Where(q => q.Name != Routine.ParentCollection.ParentProgram.MainRoutineName).Select(p => p.Name).ToList();

                                    if (!HasAdorner(layer, EnumAdorner))
                                        layer?.Add(EnumAdorner);

                                    EnumAdorner.ResetAdorner(point, 1, tag, routines);
                                    EnumAdorner.SetTextFocus();

                                    return;
                                }

                                if (!HasAdorner(layer, BrowseAdorner))
                                    layer?.Add(BrowseAdorner);

                                BrowseAdorner.ResetAdorner(point, 1, tag);
                                BrowseAdorner.SetCompleteBoxSource(GetTagList());
                                BrowseAdorner.SetTextFocus();
                            }
                        }
                    }

                    if (tb.Tag is string)
                    {
                        var tagName = tb.Tag as string;
                        if (tagName?.Contains("?") == true)
                            return;

                        //var simpleTag = GetSimpleTag(tagName);
                        //if (GetOperand(simpleTag) == null)
                        if (GetDataOperandInfo(tagName) == null)
                            return;

                        //是Tag值
                        if (!HasAdorner(layer, InputAdorner))
                            layer?.Add(InputAdorner);

                        InputAdorner.Resize();
                        InputAdorner.ResetAdorner(point, 1, tag);
                        InputAdorner.SetTextFocus();
                    }

                    if (tb.Tag is IInstruction)
                    {
                        if (!isOnlineEditable)
                            return;

                        if (!HasAdorner(layer, BrowseAdorner))
                            layer?.Add(BrowseAdorner);

                        BrowseAdorner.ResetAdorner(point, 1, tag, false);
                        BrowseAdorner.SetCompleteBoxSource(_controller.RLLInstructionCollection.Instructions);
                        BrowseAdorner.SetTextFocus();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message + e.StackTrace);
            }
        }

        private List<string> GetTagList()
        {
            List<string> list = new List<string>();
            list.AddRange(Routine.ParentCollection.ParentProgram.Tags.Select(p => p.Name));
            list.AddRange(_controller.Tags.Select(p => p.Name));
            list = list.Distinct().ToList();

            return list;
        }

        private bool HasAdorner(AdornerLayer layer, Adorner adorner)
        {
            if (layer == null)
                return false;

            var adorners = layer.GetAdorners(_mainGrid);
            if (adorners == null)
                return false;
            return adorners.Contains(adorner);
        }

        private void UpdateGraphAfterDrag(IGraph graph, int fromIndex, string fromText, int toIndex, string toText, int offset = 0)
        {
            if (graph != Graph)
                return;
            var oldRungs = Routine.CloneRungs();
            FocusInfo info = new FocusInfo { RungIndex = -1, Offset = offset };

            if (!_controller.IsOnline)
            {
                if (fromIndex == -1)
                {
                    //拖拽rung
                    var removedRungsIndex = fromText.Split(',').Select(p => int.Parse(p));
                    var removedRungs = removedRungsIndex.Select(p => Routine.Rungs[p]);

                    foreach (var removedRung in removedRungs)
                    {
                        removedRung.Mark = EditMark.Delete;
                    }

                    var dragIndex = removedRungsIndex.FirstOrDefault();

                    var editedRungsText = toText.Trim().Split(new[] { "   " }, StringSplitOptions.RemoveEmptyEntries);

                    if (toIndex > Routine.Rungs.Count)
                        return;

                    if (toIndex != Routine.Rungs.Count)
                        Routine.Rungs[toIndex].Mark = EditMark.Delete;

                    for (int i = 0; i < editedRungsText.Length; ++i)
                    {
                        Routine.Rungs.Insert(toIndex + i, new RungType { Text = editedRungsText[i] });
                    }

                    int index = 0;
                    info.RungIndex = dragIndex < toIndex ? toIndex - 1 : toIndex;
                    info.Offset = -1;

                    while (index < Routine.Rungs.Count)
                    {
                        var curRung = Routine.Rungs[index];
                        if (curRung.Mark == EditMark.Delete)
                            Routine.Rungs.RemoveAt(index);
                        else
                            index++;
                    }

                    UpdateLadderGraph(info);
                    RecordChange(oldRungs);
                }
                else
                {
                    //拖拽其他元素
                    Routine.Rungs[toIndex].Text = toText;
                    if (fromIndex != toIndex)
                        Routine.Rungs[fromIndex].Text = fromText;
                    info.RungIndex = toIndex;

                    UpdateLadderGraph(info);
                    RecordChange(oldRungs);
                }
            }
            else
            {
                if (fromIndex == -1)
                {
                    //拖拽rung
                    var draggedRungsIndex = fromText.Split(',').Select(p => int.Parse(p)).ToList();
                    var draggedRungs = draggedRungsIndex.Select(p => Routine.Rungs[p]).ToList();

                    var editedRungsText = toText.Trim().Split(new[] { "   " }, StringSplitOptions.RemoveEmptyEntries);

                    if (toIndex > Routine.Rungs.Count)
                        return;

                    if (toIndex == Routine.Rungs.Count)
                    {
                        //说明拽到了End Rung
                        Routine.Rungs.AddRange(editedRungsText.Select(p => new RungType { Text = p, Type = RungTypeEnum.Insert }));
                    }
                    else
                    {
                        for (int i = 0; i < editedRungsText.Length - 1; ++i)
                        {
                            Routine.Rungs.Insert(toIndex + i, new RungType { Text = editedRungsText[i], Type = RungTypeEnum.Insert });
                        }
                    }

                    foreach (var draggedRung in draggedRungs)
                    {
                        if (draggedRung.Type == RungTypeEnum.Normal)
                            draggedRung.Type = RungTypeEnum.Delete;
                        else if (draggedRung.Type == RungTypeEnum.Edit)
                        {
                            var rungIndex = Routine.Rungs.IndexOf(draggedRung);
                            if (rungIndex < 0 || rungIndex > Routine.Rungs.Count - 2)
                                continue;

                            Routine.Rungs.RemoveAt(rungIndex);
                            Routine.Rungs[rungIndex].Type = RungTypeEnum.Normal;
                        }
                        else if (draggedRung.Type == RungTypeEnum.Insert)//|| draggedRung.Type == RungTypeEnum.EditAcceptEdit
                            Routine.Rungs.Remove(draggedRung);

                        //else DoNothing
                    }

                    UpdateLadderGraph();
                    RecordChange(oldRungs);
                }
                else
                {
                    var toRung = Routine.Rungs[toIndex];
                    bool canDrop = toRung.Type == RungTypeEnum.Edit || toRung.Type == RungTypeEnum.Insert;//||toRung.Type == RungTypeEnum.EditAcceptEdit
                    if (!canDrop)
                        return;

                    toRung.Text = toText;

                    var fromRung = Routine.Rungs[fromIndex];
                    bool modifyFromRung = fromRung.Type == RungTypeEnum.Edit || fromRung.Type == RungTypeEnum.Insert;// ||fromRung.Type == RungTypeEnum.EditAcceptEdit
                    if (toIndex != fromIndex && modifyFromRung)
                        fromRung.Text = fromText;

                    UpdateLadderGraph();
                    RecordChange(oldRungs);
                }
            }
        }

        public void UpdateGraph(IGraph graph, string text)
        {
            if (graph != Graph)
                return;

            var focusItem = graph.FocusedItem;
            if (focusItem == null)
                return;

            var oldRungs = Routine.CloneRungs();
            var rung = focusItem as IRung ?? focusItem.GetParentRung();
            if (rung == null)
                return;

            if (!_controller.IsOnline)
            {
                //;;;为Add Rung或双击工具栏的Rung，插到焦点Rung下一行
                //;;为拖拽，插到焦点Rung上一行
                if (text == ";;;" || text == ";;")
                {
                    int focusIndex;
                    if (rung.IsEndRung)
                    {
                        Routine.Rungs.Add(new RungType { Text = ";" });
                        focusIndex = Routine.Rungs.Count - 1;
                    }
                    else
                    {
                        int index = 0;
                        if (text == ";;;")
                            index = rung.RungIndex + 1;
                        if (text == ";;")
                            index = rung.RungIndex;
                        Routine.Rungs.Insert(index, new RungType { Text = ";" });
                        focusIndex = rung.RungIndex + 1;
                    }
                    UpdateLadderGraph(focusIndex);
                    RecordChange(oldRungs);
                    return;
                }

                Routine.Rungs[rung.RungIndex].Text = text;
            }
            else
            {
                if (text == ";;" || text == ";;;")
                {
                    if (text == ";;;" && rung.EditState == EditState.Edit)// || (rung.EditState == EditState.Insert && Routine.Rungs[rung.RungIndex + 1].Type == RungTypeEnum.AcceptInsertOriginal) || (rung.EditState == EditState.EditOriginal && Routine.Rungs[rung.RungIndex - 1].Type == RungTypeEnum.EditAcceptEdit) || rung.EditState == EditState.EditAcceptEdit || rung.EditState == EditState.AcceptEdit || rung.EditState == EditState.DeleteAcceptEdit
                        return;

                    if (text == ";;" && rung.EditState == EditState.EditOriginal)
                        return;

                    int focusIndex;
                    if (rung.IsEndRung)
                    {
                        Routine.Rungs.Add(new RungType { Text = ";", Type = RungTypeEnum.Insert });
                        focusIndex = Routine.Rungs.Count - 1;
                    }
                    else
                    {
                        int index = 0;
                        if (text == ";;;")
                            index = rung.RungIndex + 1;
                        if (text == ";;")
                            index = rung.RungIndex;
                        Routine.Rungs.Insert(index, new RungType { Text = ";", Type = RungTypeEnum.Insert });
                        focusIndex = rung.RungIndex + 1;
                    }

                    UpdateLadderGraph(focusIndex);
                    RecordChange(oldRungs);
                    return;
                }

                if (rung.EditState == EditState.Edit || rung.EditState == EditState.Insert)// || rung.EditState == EditState.EditAcceptEdit
                {
                    Routine.Rungs[rung.RungIndex].Text = text;
                }
                else
                    return;
            }
            UpdateLadderGraph();
            RecordChange(oldRungs);
            //RegisterOperand();
        }

        public void UpdateGraphAndFocus(IGraph graph, string text, int offset)
        {
            if (graph != Graph)
                return;

            var focusItem = graph.FocusedItem;
            if (focusItem == null)
                return;

            var oldRungs = Routine.CloneRungs();
            var rung = focusItem as IRung ?? focusItem.GetParentRung();
            if (rung == null)
                return;

            FocusInfo info = new FocusInfo { RungIndex = rung.RungIndex, Offset = offset };
            graph.FocusInfo = info;

            if (!_controller.IsOnline)
            {
                if (text == ";;")
                {
                    int focusIndex;
                    if (rung.IsEndRung)
                    {
                        Routine.Rungs.Add(new RungType { Text = ";" });
                        focusIndex = Routine.Rungs.Count - 1;
                    }
                    else
                    {
                        Routine.Rungs.Insert(rung.RungIndex + 1, new RungType { Text = ";" });
                        focusIndex = rung.RungIndex + 1;
                    }
                    UpdateLadderGraph(focusIndex);
                    RecordChange(oldRungs);
                    return;
                }

                Routine.Rungs[rung.RungIndex].Text = text;
            }
            else
            {
                if (text == ";;")
                {
                    if (rung.EditState == EditState.Edit)// || (rung.EditState == EditState.Insert && Routine.Rungs[rung.RungIndex + 1].Type == RungTypeEnum.AcceptInsertOriginal) || (rung.EditState == EditState.EditOriginal && Routine.Rungs[rung.RungIndex - 1].Type == RungTypeEnum.EditAcceptEdit) || rung.EditState == EditState.EditAcceptEdit || rung.EditState == EditState.AcceptEdit || rung.EditState == EditState.DeleteAcceptEdit
                        return;

                    if (rung.IsEndRung)
                        Routine.Rungs.Add(new RungType { Text = ";", Type = RungTypeEnum.Insert });
                    else
                        Routine.Rungs.Insert(rung.RungIndex + 1, new RungType { Text = ";", Type = RungTypeEnum.Insert });
                }
                else if (rung.EditState == EditState.Edit || rung.EditState == EditState.Insert)// || rung.EditState == EditState.EditAcceptEdit
                {
                    Routine.Rungs[rung.RungIndex].Text = text;
                }
                else
                    return;
            }
            UpdateLadderGraph(info);
            RecordChange(oldRungs);
            //RegisterOperand();
        }

        //private void OnlineUpdateGraph(IGraph graph, string text, EditState state, int rungIndex)
        //{
        //    if (graph != Graph)
        //        return;

        //    var oldRungs = Routine.CloneRungs();
        //    Debug.Assert(rungIndex <= Routine.Rungs.Count);
        //    if (rungIndex > Routine.Rungs.Count)
        //        return;

        //    if (state == EditState.Insert)
        //    {
        //        Routine.Rungs.Insert(rungIndex, new RungType { Text = ";", Type = RungTypeEnum.Insert });
        //    }

        //    if (state == EditState.Edit && !string.IsNullOrEmpty(text))
        //    {
        //        Routine.Rungs[rungIndex].Text = text;
        //    }

        //    UpdateLadderGraph();
        //    RecordChange(oldRungs);
        //    RegisterOperand();
        //}

        private void UpdateTag(IInstruction instruction, string tag, int index, string val, bool isChangeTag)
        {
            if (instruction == null)
                return;

            var oldRungs = Routine.CloneRungs();

            if (isChangeTag && index < instruction.Parameters.Count)
            {
                instruction.Parameters[index] = val;

                //TODO:更新Routine
                var rung = instruction.GetParentRung();
                var rungs = Routine.Rungs;
                //rungs[rung.RungIndex] = new RungType { Text = LadderHelper.GetText(rung) };
                rungs[rung.RungIndex].Text = LadderHelper.GetText(rung);

                FocusInfo info = new FocusInfo { RungIndex = rung.RungIndex, Offset = instruction.Offset };
                UpdateLadderGraph(info);

                RecordChange(oldRungs);
                RegisterOperand();
            }

            if (!isChangeTag)
            {
                if (ValidateValue(tag, val))
                {
                    //更新Tag值
                    UpdateTagVal(tag, val);

                    //更新Tag值后，会自动更新界面
                }
                else if (!string.IsNullOrEmpty(tag))
                {
                    MessageBox.Show(
                        $"Failed to modify the tag value.\nString invalid.",
                        "ICSStudio", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        public bool ValidateValue(string tag, string value)
        {
            string tagName = GetTagName(tag);
            ITagCollectionContainer container = null;
            if (_localContainer.Tags.Any(p => p.Name == tagName))
                container = _localContainer;
            else if (_globalContainer.Tags.Any(p => p.Name == tagName))
                container = _globalContainer;

            var operand = operands.FirstOrDefault(p => p?.GetTagExpression().ToString() == tag && p?.TagCollection == container?.Tags);// GetDataOperandInfo(tag);//
            if (operand == null)
                return false;

            return operand.ValidateValue(value);
        }

        public ObservableCollection<AoiDataReference> Reference { get; } =
            new ObservableCollection<AoiDataReference>();

        private AoiDataReference _selectedReference;

        public AoiDataReference SelectedReference
        {
            set
            {
                _selectedReference = value;

                _isAOIRefeferencePage = value.AoiContext != null;
                //_referenceInstance = value.ParamList == null ? null : ObtainValue.GetAstName(value.ParamList[0] as ASTName);

                if (_isAOIRefeferencePage)
                    RegisterAOITag();
                RaisePropertyChanged();

                if (Graph != null)
                    Refresh();
            }
            get { return _selectedReference; }
        }

        private Hashtable _referenceTable = new Hashtable();

        private void RegisterAOITag()
        {
            _referenceTable.Clear();
            var transformTable = SelectedReference.GetFinalTransformTable(); //SelectedReference.TransformTable;

            //该AOI的定义(用于获取Properties中各参数的Usage，以及结合引用表，找到Usage未InOut类型的参数所在实例上下文中引用的tag)
            var refrenceAOI = SelectedReference.ReferenceAoi;

            foreach (var para in _paraList)
            {
                //如果是tag则直接获取到tag名；如果是tag成员，则获取到成员所在tag
                var tagName = GetTagName(para);

                //isAtomic为false表示para为数组的元素或tag的成员
                bool isAtomic = para == tagName;
                //tagDefine为AOI中该参数的定义
                var tagDefine = refrenceAOI.Tags.FirstOrDefault(p => p.Name == tagName);
                if (tagDefine == null)
                    continue;
                //需订阅的tag名
                string monitorName;

                //TODO: 编辑AOI后，应该更新FinalTransFromTable
                if (!transformTable.ContainsKey(tagName.ToUpper()))
                    continue;

                string refTagName = transformTable[tagName.ToUpper()].ToString();

                // 1、找到tag实例上下文中对应的tag的collection
                ITagCollection monitorCollection = GetTagCollection(refTagName);
                if (monitorCollection == null)
                    continue;

                // 2、加入映射表  eg: "AOI_controller.bools[0]"  <==>  "b[0]"
                _referenceTable[para] = para.Replace(tagName, refTagName);

                if (tagDefine.Usage == Usage.InOut)
                {
                    //_referenceTable[$"{_referenceInstance}.{para}"] = para.Replace(tagDefine.Name, refTagName);

                    // 3、注册上下文中的tag
                    monitorName = isAtomic ? refTagName : para.Replace(tagName, refTagName);
                }
                else
                {
                    //_referenceTable[refTagName] = tagDefine.Name;//para.Replace(tagDefine.Name, refTagName);
                    monitorName = para.Replace(tagName, refTagName);
                }

                //已订阅，则跳过
                if (operands.Exists(p =>
                        p.GetTagExpression()?.ToString() == monitorName && p.TagCollection == monitorCollection))
                    continue;

                var data = _dataServer.CreateDataOperand(monitorCollection, monitorName);
                data.StartMonitoring(true, true);
                operands.Add(data);
            }
        }

        ITagCollection GetTagCollection(string tagName)
        {
            string tag = GetTagName(tagName);

            if (SelectedReference.GetReferenceProgram().Tags.ToList().Exists(p => p.Name == tag))
                return SelectedReference.GetReferenceProgram().Tags;
            if (_controller.Tags.ToList().Exists(p => p.Name == tag))
                return _controller.Tags;

            return null;
        }

        public Visibility AoiVisibility { set; get; } = Visibility.Collapsed;

        #region 定位相关逻辑

        public void LocateElement(int rungIndex, int offset)
        {
            FocusInfo info = new FocusInfo { RungIndex = rungIndex, Offset = offset };
            Graph.FocusInfo = info;

            if (offset >= 0)
                Graph.FocusedItem = null;

            var control = (LadderControl)BottomControl;
            BoxInstructionStyleRenderer.ClearSelections((LadderControl)TopControl);
            BoxInstructionStyleRenderer.ClearSelections(control);
            control.RefreshLadderLayout();
            control.LocateRung();
        }

        #endregion

        private void InitGraph(CanvasControl host)
        {
            if (host != BottomControl && host != TopControl)
                return;

            ExecuteShowLdtEditor();
            _outputService?.AddErrors("Error: Missing operand or argument.", OrderType.None, OnlineEditType.Original, 0,
                null, Routine);
        }

        private ToolTip GetToolTip(string scope, string routine, string tagName, string dataType,
            IInstruction instruction, int pos)
        {
            if (Routine?.ParentCollection?.ParentProgram?.Name != scope)
                return null;

            if (Routine?.Name != routine)
                return null;

            //double doubleVal;
            //if (double.TryParse(tagName, out doubleVal))
            //    return new ToolTip { Content = $"Value: {tagName}" };
            bool isNumber = RLLGrammarAnalysis.IsNumber(tagName);
            if (isNumber)
                return new ToolTip { Content = $"Value: {tagName}" };

            if (tagName.Equals("s:fs", StringComparison.OrdinalIgnoreCase))
                return new ToolTip { Content = "s:fs - FirstScan Bit" };

            if (dataType.Equals("routine", StringComparison.OrdinalIgnoreCase))
            {
                bool isRoutineDefined = Routine?.ParentCollection?[tagName] != null;
                if (isRoutineDefined)
                    return new ToolTip { Content = $"Routine: {tagName}" };

                return new ToolTip { Content = $"Undefined routine: {tagName}" };
            }

            if (dataType.Equals("label", StringComparison.OrdinalIgnoreCase))
            {
                return new ToolTip { Content = $"Label: {tagName}" };
            }

            var aoiDefinitionCollection = _controller.AOIDefinitionCollection as AoiDefinitionCollection;
            var aoi = aoiDefinitionCollection?.Find(dataType);
            bool isAOI = aoi != null;

            //找到Operand
            var operand = operands.FirstOrDefault(p =>
                p.GetTagExpression()?.ToString() == tagName && (p.TagCollection.ParentProgram == null ||
                                                                p.TagCollection.ParentProgram.Name == scope));
            if (operand == null)
            {
                //var enumVal = SimpleServices.Instruction.Utils.ParseEnum(mnemonic, i, actualPara);
                //if (enumVal >= 0 && (para.DataType == "SINT" || para.DataType == "DINT"))
                //    checkResult = CheckTypeResult.Passed;

                // var enumVal = SimpleServices.Instruction.Utils.ParseEnum(tagName);
                var instructionDefinition =
                    InstructionDefinition.GetDefinition(instruction.Mnemonic);
                if (instructionDefinition != null)
                {
                    var list = instructionDefinition.Parameters.Where(p => p.Label != " ").ToList();
                    if (list.Count > 0 && list[pos].IsEnum)
                        return new ToolTip { Content = tagName };
                }
                //else
                //{
                //    var aoiDefinitionCollection = _controller.AOIDefinitionCollection as AoiDefinitionCollection;
                //    var aoi = aoiDefinitionCollection.Find(instruction.Mnemonic);
                //    if(aoi != null)
                //    {

                //    }
                //}

                return new ToolTip { Content = $"Undefined Tag: {tagName}" };
            }

            var tag = operand.Tag;
            var usage = tag.Usage;
            bool isTag = operand.GetTagExpression() is SimpleTagExpression;

            StackPanel sp = new StackPanel();
            TextBlock row1 = new TextBlock();
            row1.TextAlignment = TextAlignment.Left;
            row1.Text = isTag ? $"Tag: {tagName}" : $"Tag Element: {tagName}";
            if (operand.IsBool && operand.FormattedValueString == "??")
                row1.Text = $"Invalid Bit Specifier: {tagName}";
            sp.Children.Add(row1);

            string dataTypeRow;
            if (isTag)
            {
                dataTypeRow = $"Data Type: {operand.DataTypeInfo.ToString()}";
            }
            else
            {
                dataTypeRow = operand.IsBool
                    ? $"Element Data Type: BOOL"
                    : $"Element Data Type: {operand.DataTypeInfo.ToString()}";
            }

            if (!operand.IsBool || operand.FormattedValueString != "??")
                sp.Children.Add(new TextBlock { Text = $"{dataTypeRow}", TextAlignment = TextAlignment.Left });

            bool isLocalTag = operand.TagCollection.ParentProgram != null;

            string usageStr = usage.ToString();//Enum.GetName(typeof(Usage), usage);//
            string row2;
            if (usage == Usage.Local)
                row2 = "Usage: Local Tag";
            else
                row2 = $"Usage: {usageStr} Parameter";

            if (isLocalTag)
                sp.Children.Add(new TextBlock { Text = row2, TextAlignment = TextAlignment.Left });

            if (!isTag)
            {
                string tagDataType = $"Tag Data Type: {operand.Tag.DataTypeInfo.ToString()}"; //operand.
                sp.Children.Add(new TextBlock { Text = tagDataType, TextAlignment = TextAlignment.Left });
            }


            if (SelectedReference != null && usage == Usage.InOut)
            {
                string argument = "Argument: ";
                if (!_isAOIRefeferencePage)
                    argument += "<not specified>";
                else
                {
                    var referencedOperandName = _referenceTable[tagName].ToString();
                    var referencedProgram = SelectedReference.GetReferenceProgram();
                    var referencedCollection = referencedProgram.Tags;

                    TagExpressionParser parser = new TagExpressionParser();
                    var tagExpression = parser.Parser(referencedOperandName);
                    SimpleTagExpression simpleTagExpression = parser.GetSimpleTagExpression(tagExpression);

                    if (referencedCollection.ToList().Exists(p => p.Name == simpleTagExpression.TagName))
                        argument += $@"\{referencedProgram.Name}.{referencedOperandName}";
                    else
                        argument += referencedOperandName;
                }

                sp.Children.Add(new TextBlock { Text = argument, TextAlignment = TextAlignment.Left });
            }

            string scopeStr = isLocalTag ? operand.TagCollection.ParentProgram.Name : "Controller";
            string row3;
            if (isTag)
                row3 = $"Scope: {scopeStr}";
            else
                row3 = $"Tag Scope: {scopeStr}";
            sp.Children.Add(new TextBlock { Text = row3, TextAlignment = TextAlignment.Left });

            string valueStr = string.Empty;
            bool showValue = true;

            if (operand.DataTypeInfo.DataType.IsStruct && !operand.DataTypeInfo.DataType.IsStringType)
                showValue = false;

            if (operand.IsBool && operand.FormattedValueString == "??")
                showValue = false;

            if (showValue)
            {
                if (SelectedReference == null)
                {
                    valueStr = $"Value: {operand.FormattedValueString}";
                }
                else
                {
                    if (!_isAOIRefeferencePage)
                    {
                        if (usage == Usage.InOut)
                        {
                            showValue = false;
                        }
                        else
                        {
                            valueStr = $"Value: {operand.FormattedValueString}";
                        }
                    }
                    else
                    {
                        if (usage == Usage.InOut)
                        {
                            var referencedOperandName = _referenceTable[tagName].ToString();
                            var referencedProgram = SelectedReference.GetReferenceProgram();
                            var referencedOperand = operands.FirstOrDefault(p =>
                                p.GetTagExpression().ToString() == referencedOperandName &&
                                (p.TagCollection.ParentProgram == null ||
                                 p.TagCollection.ParentProgram.Name == referencedProgram.Name));
                            if (referencedOperand == null)
                                showValue = false;
                            else
                            {
                                valueStr = $"Value: {referencedOperand.FormattedValueString}";
                            }
                        }
                        else
                        {
                            valueStr = $"Value: {operand.FormattedValueString}";
                        }
                    }
                }
            }

            if (showValue && !isAOI)
                sp.Children.Add(new TextBlock
                {
                    Text = valueStr,
                    TextAlignment = TextAlignment.Left
                });

            string description = GetDescription(tagName, scope);
            if (!string.IsNullOrEmpty(description))
            {
                sp.Children.Add(new TextBlock
                {
                    Text = $"Description: {description}",
                    TextAlignment = TextAlignment.Left
                });
            }

            ToolTip toolTip = new ToolTip();
            toolTip.Content = sp;
            return toolTip;
        }

        private string GetDescription(string tagName, string scope)
        {
            if (scope != Routine.ParentCollection.ParentProgram.Name)
                return null;

            var operand = operands.FirstOrDefault(p =>
                p.GetTagExpression()?.ToString() == tagName && (p.TagCollection.ParentProgram == null || p.TagCollection.ParentProgram.Name == scope));

            if (operand == null)
                return null;

            string description;
            var tag = operand.Tag as Tag;
            if (tag != null && (tagName.Contains('.') || tagName.Contains('[')))
            {//
                var simpleTag = tag.Name;
                description = Tag.GetChildDescription(tag.Description, tag.DataTypeInfo, tag.ChildDescription, tagName.Substring(simpleTag.Length));
            }
            else
                description = tag?.Description;

            return description ?? string.Empty;
        }

        public string GetAOI(string instr)
        {
            var aoi = _controller.AOIDefinitionCollection.FirstOrDefault(p =>
                p.Name.Equals(instr, StringComparison.OrdinalIgnoreCase));
            if (aoi == null)
                return null;

            return aoi.Name;
        }

        private void ShowConfig(string scope, string routine, string tag)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (Routine.ParentCollection.ParentProgram.Name != scope)
                return;

            if (routine != Routine.Name)
                return;

            if (SelectedReference != null)
                return;

            var operand = operands.FirstOrDefault(p =>
                p.GetTagExpression()?.ToString() == tag && (p.TagCollection.ParentProgram == null ||
                                                            p.TagCollection.ParentProgram.Name == scope));

            if (operand == null)
                return;

            string typeName = operand.DataTypeInfo.DataType.Name;
            var newTag = operand.Tag;

            if (typeName.Equals("PID", StringComparison.OrdinalIgnoreCase))
            {
                ArrayField field = (newTag as Tag)?.DataWrapper.Data as ArrayField;
                PidSetupDialog pidSetupDialog = new PidSetupDialog(new PidSetUpViewModel(field, newTag))
                {
                    Owner = Application.Current.MainWindow
                };
                pidSetupDialog.ShowDialog();
            }

            if ((typeName.Equals("CAM", StringComparison.OrdinalIgnoreCase)
                || typeName.Equals("CAM_PROFILE", StringComparison.OrdinalIgnoreCase))
                && operand.DataTypeInfo.Dim1 > 0)
            {
                CamEditorDialog dialog = new CamEditorDialog(new CamEditorViewModel(newTag))
                {
                    Owner = Application.Current.MainWindow
                };
                dialog.ShowDialog();
            }

            if (typeName.Equals("AXIS_CIP_DRIVE", StringComparison.OrdinalIgnoreCase))
            {
                ICreateDialogService createDialogService =
                    Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;
                var uiShell = (IVsUIShell)Package.GetGlobalService(typeof(SVsUIShell));

                var window =
                    createDialogService?.CreateAxisCIPDriveProperties(newTag);
                window?.Show(uiShell);
            }

            if (typeName.Equals("Motion_Group", StringComparison.OrdinalIgnoreCase))
            {
                var createDialogService =
                    (ICreateDialogService)Package.GetGlobalService(typeof(SCreateDialogService));

                var uiShell = (IVsUIShell)Package.GetGlobalService(typeof(SVsUIShell));

                var window =
                    createDialogService?.CreateMotionGroupProperties(newTag);
                window?.Show(uiShell);
            }

            if (typeName.Equals("MESSAGE", StringComparison.OrdinalIgnoreCase))
            {
                var title = $"Message {LanguageManager.GetInstance().ConvertSpecifier("Configuration")} - {newTag.Name}";
                MessageConfigurationDialog dialog = new MessageConfigurationDialog(newTag, title)
                {
                    Owner = Application.Current.MainWindow
                };

                dialog.ShowDialog();
            }

            var aoiDef = _controller.AOIDefinitionCollection.FirstOrDefault(p => p.Name == typeName);
            if (aoiDef != null)
            {
                //var createDialogService =
                //    (ICreateDialogService)Package.GetGlobalService(typeof(SCreateDialogService));

                //var uiShell = (IVsUIShell)Package.GetGlobalService(typeof(SVsUIShell));

                //var window =
                //    createDialogService?.AddOnInstructionProperties(aoiDef);
                //window?.Show(uiShell);
            }
        }

        private void SetProjectDirty()
        {
            IProjectInfoService projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            projectInfoService?.SetProjectDirty();
        }

        public int Apply()
        {
            IsDirty = false;
            ClearChange();
            return 0;
        }

        public bool IsAOI(string mnemonic)
        {
            return _controller.AOIDefinitionCollection.Any(p =>
                p.Name.Equals(mnemonic, StringComparison.OrdinalIgnoreCase));
        }

        #region Redo/Undo

        public RelayCommand UndoCommand { get; }

        bool CanExecuteUndo()
        {
            if (_rungUndoStack.Count < 1)
                return false;

            return true;
        }

        private void ExecuteUndo()
        {
            var rungs = _rungUndoStack.Pop() as List<RungType>;
            if (rungs == null)
                return;

            _rungRedoStack.Push(Routine.CloneRungs());
            Routine.UpdateRungs(rungs);
            UpdateLadderGraph(Graph.FocusInfo);
        }

        public RelayCommand RedoCommand { get; }

        bool CanExecuteRedo()
        {
            if (_rungRedoStack.Count < 1)
                return false;

            return true;
        }

        private void ExecuteRedo()
        {
            var rungs = _rungRedoStack.Pop() as List<RungType>;
            if (rungs == null)
                return;

            _rungUndoStack.Push(Routine.CloneRungs());
            Routine.UpdateRungs(rungs);
            UpdateLadderGraph(Graph.FocusInfo);
        }

        public void RecordChange(List<RungType> rungs)
        {
            _rungUndoStack.Push(rungs);
            _rungRedoStack.Clear();
            RefreshExecutable();
        }

        private void ClearChange()
        {
            _rungRedoStack.Clear();
            _rungUndoStack.Clear();
            RefreshExecutable();
        }

        private Stack _rungUndoStack = new Stack();
        private Stack _rungRedoStack = new Stack();

        #endregion

        #region HotKey

        public RelayCommand<KeyEventArgs> HotKeyPressedCommand { get; }

        private void OnHotKeyPressed(KeyEventArgs args)
        {
            var focusItem = Graph.FocusedItem;
            if (focusItem == null)
                return;

            switch (args.Key)
            {
                case Key.F2:
                    //InsertElement("XIC");
                    break;
                case Key.F3:
                    //InsertElement("XIO");
                    break;
                case Key.F4:
                    InsertElement("XIC");
                    //InsertElement("OTE");
                    break;
                case Key.F5:
                    InsertElement("XIO");
                    //InsertElement("ONS");
                    break;
                case Key.F6:
                    InsertElement("OTE");
                    //InsertElement("OSF");
                    break;
                case Key.F7:
                    InsertElement("FunctionBlock");
                    break;
                case Key.F8:
                    InsertElement("Rung");
                    break;
                case Key.D1:
                    ToggleBit(true);
                    break;
                case Key.D0:
                    ToggleBit(false);
                    break;
                case Key.NumPad1:
                    ToggleBit(true);
                    break;
                case Key.NumPad0:
                    ToggleBit(false);
                    break;
            }
        }

        private void InsertElement(string mnemonic)
        {
            if (mnemonic == "XIC" || mnemonic == "XIO" || mnemonic == "OTE" || mnemonic == "ONS" || mnemonic == "OSF" || mnemonic == "Rung")
                BoxInstructionStyleRenderer.InsertElement(Graph, mnemonic);

            if (mnemonic == "FunctionBlock")
            {
                var layer = AdornerLayer.GetAdornerLayer(_mainGrid);
                try
                {
                    if (!HasAdorner(layer, BrowseAdorner))
                        layer?.Add(BrowseAdorner);

                    EditingControl = null;
                    var point = new Point(0, 0);

                    BrowseAdorner.ResetAdorner(point, 1, "", false);
                    BrowseAdorner.SetCompleteBoxSource(_controller.RLLInstructionCollection.Instructions);
                    BrowseAdorner.SetTextFocus();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message + e.StackTrace);
                }
            }
        }

        private void ToggleBit(bool val)
        {
            var focusInstruction = Graph.FocusedItem as IInstruction;
            if (focusInstruction == null)
                return;

            var mnemonic = focusInstruction.Mnemonic;
            List<string> bitCommands = new List<string>() { "XIC", "XIO", "OTE", "OTU", "OTL" };
            if (!bitCommands.Contains(mnemonic))
                return;

            var tag = focusInstruction.Parameters[0];
            var tagName = GetSimpleTag(tag);

            var item = _localContainer.Tags.FirstOrDefault(p => p.Name == tagName) ?? _globalContainer.Tags.FirstOrDefault(p => p.Name == tagName);

            if (item == null)
                return;

            var operand = operands.FirstOrDefault(p =>
                p.GetTagExpression()?.ToString() == tag && p.TagCollection == item.ParentCollection);
            if (operand == null)
                return;

            bool isOnline = _controller.IsOnline;

            if (item.DataTypeInfo.DataType.IsBool || operand.IsBool)
            {
                string value = val ? "1" : "0";

                if (isOnline)
                {
                    SetTagValueToPLC(item, tag, value);
                }
                else
                {
                    operand.SetValue(value);
                }
            }
        }

        public bool CanApply()
        {
            return true;
        }

        #endregion
    }
}