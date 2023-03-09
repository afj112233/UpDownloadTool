using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.AvalonEdit.Document;
using ICSStudio.AvalonEdit.Variable;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.StxEditor.Menu;
using ICSStudio.StxEditor.View;
using ICSStudio.StxEditor.ViewModel;
using ICSStudio.StxEditor.ViewModel.CodeSnippets;
using ICSStudio.StxEditor.ViewModel.Highlighting;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIInterfaces.Editor;
using ICSStudio.UIInterfaces.GlobalClipboard;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.UIInterfaces.QuickWatch;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.EditorPackage.STEditor
{
    public class STEditorViewModel : ViewModelBase, IEditorPane, ICanApply, IGlobalClipboard
    {
        private bool _originalCommandCheck;
        private bool _pendingCommandCheck;
        private bool _testCommandCheck;
        private readonly TextMarkerService _textMarkerService;
        private readonly DispatcherTimer _uiUpdateTimer;
        public STEditorViewModel(IRoutine routine, UserControl userControl)
        {
            Routine = routine as STRoutine;
            Contract.Assert(Routine != null);

            Options = new StxEditorOptions();
            var aoi = Routine.ParentCollection.ParentProgram as AoiDefinition;
            if (aoi != null)
            {
                AoiVisibility = Visibility.Visible;
                CollectionChangedEventManager.AddHandler(aoi.References, References_CollectionChanged);
                Reference.Add(new AoiDataReference(null, null, null,OnlineEditType.Original, $"{aoi.Name} <definition>"));
                foreach (var reference in aoi.References)
                {
                    if (reference?.ParamList == null) continue;
                    AddReference(reference);
                }

                MarkReference();
                SelectedReference = Reference[0];
                PropertyChangedEventManager.AddHandler(aoi, Aoi_PropertyChanged, "IsSealed");
            }

            Program = routine.ParentCollection.ParentProgram as Program;
            Control = userControl;
            userControl.DataContext = this;
            string codeText;

            if (Routine.PendingEditsExist)
            {
                codeText = string.Join("\n", Routine.PendingCodeText);
            }
            else if (Routine.TestCodeText != null)
            {
                codeText = string.Join("\n", Routine.TestCodeText);
            }
            else
            {
                codeText = string.Join("\n", Routine.CodeText);
            }

            var document = new TextDocument(codeText);
            Caption = $"{Routine.ParentCollection.ParentProgram?.Name} - {Routine.Name}";
            _textMarkerService = new TextMarkerService(document, (STRoutine) routine);
            Document = new StxEditorDocument(document, Options, _textMarkerService,
                routine)
            {
                Dispatcher = userControl.Dispatcher
            };
            OnlineToolbarVisibility = routine.ParentController.IsOnline ? Visibility.Visible : Visibility.Collapsed;

            if (Routine.PendingCodeText != null || Routine.TestCodeText != null)
            {
                if (Document.HasPending)
                {
                    PendingCommandCheck = true;
                    Routine.CurrentOnlineEditType = OnlineEditType.Pending;
                }
                else if (Document.HasTest)
                {
                    TestCommandCheck = true;
                    Routine.CurrentOnlineEditType = OnlineEditType.Test;
                }
            }

            AddListener();
            //_document = document;
            var top = new StxEditorView(Routine, Document, Options) {Name = "Top"};
            top.TextEditor.TextArea.TextView.Name = "Top";
            TopControl = top;
            var bottom = new StxEditorView(Routine, Document, Options) {Name = "Bottom"};
            bottom.TextEditor.TextArea.TextView.Name = "Bottom";
            BottomControl = bottom;
            ((StxEditorView) TopControl).GotFocus += STEditorViewModel_GotFocus;
            ((StxEditorView) BottomControl).GotFocus += STEditorViewModel_GotFocus;
            Document.ShowCaretControl = BottomControl;

            SaveCommand = new RelayCommand(ExecuteSaveCommand);
            StartPendingRoutineCommand =
                new RelayCommand(ExecuteStartPendingRoutineCommand, CanExecuteStartPendingRoutineCommand);
            AcceptPendingRoutineCommand =
                new RelayCommand(ExecuteAcceptPendingRoutineCommand, CanExecuteAcceptPendingRoutineCommand);
            CancelPendingRoutineCommand =
                new RelayCommand(ExecuteCancelPendingRoutineCommand, CanExecuteCancelPendingRoutineCommand);
            AssembledAcceptPendingRoutineCommand = new RelayCommand(ExecuteAssembledAcceptPendingRoutineCommand,
                CanExecuteAssembledAcceptPendingRoutineCommand);
            CancelAcceptedPendingRoutineCommand = new RelayCommand(ExecuteCancelAcceptedPendingRoutineCommand,
                CanExecuteCancelAcceptedPendingRoutineCommand);
            AcceptPendingProgramCommand =
                new RelayCommand(ExecuteAcceptPendingProgramCommand, CanExecuteAcceptPendingProgramCommand);
            CancelPendingProgramCommand =
                new RelayCommand(ExecuteCancelPendingProgramCommand, CanExecuteCancelPendingProgramCommand);
            TestAcceptedProgramCommand =
                new RelayCommand(ExecuteTestAcceptedProgramCommand, CanExecuteTestAcceptedProgramCommand);
            UnTestAcceptedProgramCommand = new RelayCommand(ExecuteUnTestAcceptedProgramCommand,
                CanExecuteUnTestAcceptedProgramCommand);
            AssembledAcceptedProgramCommand = new RelayCommand(ExecuteAssembledAcceptedProgramCommand,
                CanExecuteAssembledAcceptedProgramCommand);
            CancelAcceptedProgramCommand = new RelayCommand(ExecuteCancelAcceptedProgramCommand,
                CanExecuteCancelAcceptedProgramCommand);
            FinalizeCommand = new RelayCommand(ExecuteFinalizeCommand, CanExecuteFinalizeCommand);
            GoCommand = new RelayCommand(ExecuteGoCommand, CanExecuteGoCommand);
            Options.CanCancelAcceptedProgramCommand = Program?.HasTest ?? false;
            ((StxEditorView) BottomControl).SetSTFocus();

            var service = Package.GetGlobalService(typeof(SQuickWatchService)) as IQuickWatchService;
            service?.AddMonitorRoutine(routine);
            WeakEventManager<CurrentObject, EventArgs>.AddHandler(CurrentObject.GetInstance(), "CurrentObjectChanged",
                TextEditor_CurrentObjectChanged);
            WeakEventManager<Controller, EventArgs>.AddHandler(
                (Controller) routine.ParentController, "OperationModeChanged", Controller_OperationModeChanged);
            _uiUpdateTimer = new DispatcherTimer(DispatcherPriority.SystemIdle)
            {
                Interval = TimeSpan.FromMilliseconds(1000)
            };
            _uiUpdateTimer.Tick += OnTick;
            if (routine.ParentController.IsOnline)
                _uiUpdateTimer.Start();

            LanguageManager.GetInstance().SetLanguage(Control);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
            WeakEventManager<Controller,EventArgs>.AddHandler(Routine.ParentController as Controller, "KeySwitchChanged",OnKeySwitchChanged);
        }

        public string GetSelection()
        {
            return FocusedView?.TextEditor?.SelectedText ?? "";
        }

        private void LanguageChanged(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(Control);
        }

        private void OnKeySwitchChanged(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                StartPendingRoutineCommand.RaiseCanExecuteChanged();
                AcceptPendingRoutineCommand.RaiseCanExecuteChanged();
                AcceptPendingProgramCommand.RaiseCanExecuteChanged();
                CancelPendingRoutineCommand.RaiseCanExecuteChanged();
                CancelPendingProgramCommand.RaiseCanExecuteChanged();
                FinalizeCommand.RaiseCanExecuteChanged();
            });
        }

        public void IsOnScreenChanged(bool newIsOnScreen)
        {
            if (newIsOnScreen)
            {
                if(!_uiUpdateTimer.IsEnabled&&Routine.ParentController.IsOnline)
                    _uiUpdateTimer.Start();
            }
            else
            {
                if(_uiUpdateTimer.IsEnabled)
                    _uiUpdateTimer.Stop();
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            try
            {
                var controller = Controller.GetInstance();
                if (!controller.IsOnline)
                    return;
                TagSyncController tagSyncController
                    = controller.Lookup(typeof(TagSyncController)) as TagSyncController;
                if (tagSyncController == null)
                    return;
                List<ICSStudio.Interfaces.Tags.ITag> updateTags = new List<ICSStudio.Interfaces.Tags.ITag>();
                foreach (var variableInfo in Routine.GetCurrentVariableInfos(Routine.CurrentOnlineEditType).ToList())
                {
                    if (variableInfo.Tag == null) continue;
                    if (!updateTags.Contains(variableInfo.Tag))
                    {
                        updateTags.Add(variableInfo.Tag);
                    }
                }

                foreach (var updateTag in updateTags)
                {
                    tagSyncController.Update((Tag)updateTag, updateTag.Name);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        public void InsertLanguageElement(string instr)
        {
            if (Options.CanEditorInput)
            {
                var index = FocusedView.TextEditor.CaretOffset;
                if (FocusedView.TextEditor.SelectionLength > 0)
                {
                    FocusedView.TextEditor.Document.Remove(FocusedView.TextEditor.SelectionStart,FocusedView.TextEditor.SelectionLength);
                }
                FocusedView.TextEditor.Document.Insert(index,instr);
            }
        }

        private void Controller_OperationModeChanged(object sender, EventArgs e)
        {
            var controller = (Controller) sender;
            if (controller.IsOnline)
            {
                ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    // You're now on the UI thread.
                    RaiseCommand();
                });
            }
        }
        

        private void Program_PropertyChanged1(object sender, PropertyChangedEventArgs e)
        {
            SetAoiStatus();
        }
        
        private void Aoi_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseCommand();
        }

        private void TextEditor_CurrentObjectChanged(object sender, EventArgs e)
        {
            var isFocusChanged = CurrentObject.GetInstance().Current != this;
            ((StxEditorView) TopControl).SetIsFocusChanged(isFocusChanged);
            ((StxEditorView) BottomControl).SetIsFocusChanged(isFocusChanged);
        }

        public bool IsInitial { set; get; }=true;

        public bool IsDirty
        {
            set
            {
                if(IsInitial || !Document.CanExecuteDocumentChanged)return;
                if (_isDirty != value)
                {
                    _isDirty = value;
                    if (value)
                    {
                        IProjectInfoService projectInfoService =
                            Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
                        projectInfoService?.SetProjectDirty();
                        if (!Caption.EndsWith("*"))
                        {
                            Caption = $"{Caption}*";
                        }
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
            get { return _isDirty; }
        }

        private void DocumentOnChanged(object sender, DocumentChangeEventArgs e)
        {
            IsDirty = true;
        }

        public bool IsOnScreen
        {
            set
            {
                _isNeedLoad = value && !_isOnScreen;
                _isOnScreen = value;
            }
            get { return _isOnScreen; }
        }

        private bool _isNeedLoad = false;

        public void SetView()
        {
            ((StxEditorView) TopControl).TextEditor.TextArea.TextView.Redraw();
            ((StxEditorView) BottomControl).TextEditor.TextArea.TextView.Redraw();
        }

        public void SetReference(AoiDataReference reference)
        {
            Debug.Assert(Reference.Contains(reference));
            SelectedReference = reference;
        }

        private void References_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
#pragma warning disable VSTHRD110 // 观察异步调用的结果
            System.Threading.Tasks.Task.Run(async () =>
#pragma warning restore VSTHRD110 // 观察异步调用的结果
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                if (e.NewItems != null)
                {
                    lock (_syncReference)
                    {
                        foreach (AoiDataReference item in e.NewItems)
                        {
                            AddReference(item);
                        }

                        MarkReference();
                    }
                }

                if (e.OldItems != null)
                {
                    lock (_syncReference)
                    {
                        foreach (AoiDataReference item in e.OldItems)
                        {
                            var referenceItem = Reference.FirstOrDefault(r =>
                                r.Line == item.Line && r.Column == item.Column && r.Routine == item?.Routine);
                            Debug.Assert(referenceItem != null);
                            if (referenceItem != null)
                            {
                                if (Equals(SelectedReference, referenceItem))
                                {
                                    SelectedReference = Reference[0];
                                }

                                Reference.Remove(referenceItem);
                            }
                        }

                        MarkReference();
                    }
                }
            });
        }

        private readonly object _syncReference = new object();

        private void AddReference(AoiDataReference aoiDataReference)
        {
            lock (_syncReference)
            {
                var referenceItem = Reference.LastOrDefault(r => r.Routine == aoiDataReference?.Routine &&
                                                            r.Line == aoiDataReference.Line &&
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
                        var scope = aoiTagName.Substring(1, aoiTagName.IndexOf(".") - 1);
                        aoiDataReference.Title = $"{aoiTagName} ({scope})";
                    }
                    else
                    {
                        var tagName = aoiTagName;
                        int index = tagName.IndexOf(".");
                        if (index > -1)
                        {
                            tagName = tagName.Substring(0, index);
                        }

                        index = tagName.IndexOf("[");
                        if (index > -1)
                        {
                            tagName = tagName.Substring(0, index);
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
                        Reference.Insert(Reference.IndexOf(item) + 1, aoiDataReference);
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
                var index = name.LastIndexOf("[");
                return name.Substring(0, index);
            }

            return name;
        }

        public Visibility AoiVisibility { set; get; } = Visibility.Collapsed;

        public StxEditorView FocusedView
        {
            private set { _focusedView = value; }
            get
            {
                if (_focusedView == null)
                    return (StxEditorView) BottomControl;
                return _focusedView;
            }
        }

        public override void Cleanup()
        {
            if (CurrentObject.GetInstance().Current == this)
            {
                CurrentObject.GetInstance().Current = null;
            }
            
            try
            {
                Options.Cleanup = true;
                ((StxEditorView) TopControl).Clean();
                ((StxEditorView) TopControl).GotFocus -= STEditorViewModel_GotFocus;
                ((StxEditorView) BottomControl).Clean();
                ((StxEditorView) BottomControl).GotFocus -= STEditorViewModel_GotFocus;
                ((StxEditorView) TopControl).TextEditor.TextArea.TextView.CleanInLineLayer();
                ((StxEditorView) BottomControl).TextEditor.TextArea.TextView.CleanInLineLayer();
                RemoveListener();
                Document.Clean();
                var aoi = Routine.ParentCollection.ParentProgram as AoiDefinition;
                if (aoi != null)
                {
                    PropertyChangedEventManager.RemoveHandler(aoi, Aoi_PropertyChanged, "IsSealed");
                    CollectionChangedEventManager.RemoveHandler(aoi.References, References_CollectionChanged);
                }

                var service = Package.GetGlobalService(typeof(SQuickWatchService)) as IQuickWatchService;
                service?.RemoveMonitorRoutine(Routine);
                WeakEventManager<CurrentObject, EventArgs>.RemoveHandler(CurrentObject.GetInstance(),
                    "CurrentObjectChanged", TextEditor_CurrentObjectChanged);
                WeakEventManager<Controller, EventArgs>.RemoveHandler(
                    (Controller) Routine.ParentController, "OperationModeChanged", Controller_OperationModeChanged);

                foreach (VariableInfo currentVariableInfo in Routine.GetCurrentVariableInfos(Routine.CurrentOnlineEditType))
                {
                    currentVariableInfo.StopTimer();
                }

                if(_uiUpdateTimer.IsEnabled)
                    _uiUpdateTimer.Stop();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }

            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
            WeakEventManager<Controller, EventArgs>.RemoveHandler(Routine.ParentController as Controller, "KeySwitchChanged", OnKeySwitchChanged);
        }

        public bool IsCloseProject { set; get; }

        public Visibility OnlineToolbarVisibility
        {
            set { Set(ref _onlineToolbarVisibility, value); }
            get { return _onlineToolbarVisibility; }
        }

        public bool OriginalCommandCheck
        {
            set { Set(ref _originalCommandCheck, value); }
            get { return _originalCommandCheck; }
        }

        public bool PendingCommandCheck
        {
            set { Set(ref _pendingCommandCheck, value); }
            get { return _pendingCommandCheck; }
        }

        public bool TestCommandCheck
        {
            set { Set(ref _testCommandCheck, value); }
            get { return _testCommandCheck; }
        }

        public void SetCaret(int line, int offset, int len)
        {
            var index = Document.Document.GetOffset(new TextLocation(line + 1, offset + 1));

            if (Document.ShowCaretControl == TopControl)
                ((StxEditorView) TopControl).SetCaret(index, len);
            else
                ((StxEditorView) BottomControl).SetCaret(index, len);

        }

        public ObservableCollection<AoiDataReference> Reference { get; } =
            new ObservableCollection<AoiDataReference>();

        public AoiDataReference SelectedReference
        {
            set
            {
                if (_selectedReference?.Routine != null)
                {
                    var program = _selectedReference.GetReferenceProgram();
                    if(program!=null)
                    PropertyChangedEventManager.RemoveHandler(program, Program_PropertyChanged1, "UpdateRoutineRunStatus");
                }

                _selectedReference = value;

                if (_selectedReference?.Routine != null)
                {
                    var program = _selectedReference.GetReferenceProgram();
                    if (program != null)
                        PropertyChangedEventManager.AddHandler(program, Program_PropertyChanged1, "UpdateRoutineRunStatus");
                }
                Options.SelectedDataReference = value;
                Options.IsConnecting = value?.Routine != null;
                if (value?.Routine == null)
                {
                    GoTip = "Go To Location of Selected Data Context";
                }
                else
                {
                    GoTip =
                        $"Go To Location of Selected Data Context\n'{value?.Routine.ParentCollection.ParentProgram.Name} - {value?.Routine.Name}{((value.Routine as STRoutine)?.PendingCodeText != null ? (value.OnlineEditType == OnlineEditType.Pending) ? "Pend" : "Original" : "")} ,line {value?.Line}'";
                    var innerReference = value.InnerDataReference;
                    if (innerReference != null)
                    {
                        var innerTip = "";
                        while (innerReference != null)
                        {
                            innerTip =
                                $"{innerTip}\n'{innerReference?.Routine.ParentCollection.ParentProgram.Name} - {innerReference?.Routine.Name} ,line {innerReference?.Line}'";
                            innerReference = innerReference.InnerDataReference;
                        }

                        GoTip = GoTip + innerTip;
                    }
                }

                SetAoiStatus();
                Document?.SetAoiConnection(value);
                GoCommand?.RaiseCanExecuteChanged();
                RaisePropertyChanged();
                var service = Package.GetGlobalService(typeof(SQuickWatchService)) as IQuickWatchService;
                service?.SetAoiMonitor(Routine, value);
            }
            get { return _selectedReference; }
        }

        private void SetAoiStatus()
        {
            if (!Routine.ParentController.IsOnline ||
                !(Routine.ParentCollection.ParentProgram is AoiDefinition)) return;
            if (BottomControl == null || TopControl == null) return;
            if (SelectedReference?.Routine == null)
            {
                ((StxEditorView) BottomControl).SetStatus(false);
                ((StxEditorView) TopControl).SetStatus(false);
            }
            else
            {
                var referenceRoutine = SelectedReference.GetTargetRoutine() as STRoutine;
                if (referenceRoutine != null)
                {
                    if (RoutineExtend.CheckRoutineInRun(referenceRoutine))
                    {
                        ((StxEditorView) BottomControl).SetStatus(true);
                        ((StxEditorView) TopControl).SetStatus(true);
                    }
                    else
                    {
                        ((StxEditorView) BottomControl).SetStatus(false);
                        ((StxEditorView) TopControl).SetStatus(false);
                    }
                }
                else
                {
                    ((StxEditorView) BottomControl).SetStatus(false);
                    ((StxEditorView) TopControl).SetStatus(false);
                }

            }
        }

        public STRoutine Routine { get; }
        public Program Program { get; }
        public StxEditorOptions Options { get; }
        public StxEditorDocument Document { get; }

        public void UpdateControl()
        {
            RaisePropertyChanged("TopControl");
            RaisePropertyChanged("BottomControl");
            RaisePropertyChanged("StartPendingRoutineCommand");
            RaisePropertyChanged("AcceptPendingRoutineCommand");
            RaisePropertyChanged("CancelPendingRoutineCommand");
            RaisePropertyChanged("AssembledAcceptPendingRoutineCommand");
            RaisePropertyChanged("CancelAcceptedPendingRoutineCommand");
            RaisePropertyChanged("AcceptPendingProgramCommand");
            RaisePropertyChanged("CancelPendingProgramCommand");
            RaisePropertyChanged("TestAcceptedProgramCommand");
            RaisePropertyChanged("UnTestAcceptedProgramCommand");
            RaisePropertyChanged("AssembledAcceptedProgramCommand");
            RaisePropertyChanged("CancelAcceptedProgramCommand");
            RaisePropertyChanged("FinalizeCommand");
            RaisePropertyChanged("OnlineToolbarVisibility");
            RaisePropertyChanged("SaveCommand");
        }

        public object TopControl { get; }
        public object BottomControl { get; }

        public string Caption
        {
            private set
            {
                _caption = value;
                UpdateCaptionAction?.Invoke(Caption);
            }
            get { return _caption; }
        }

        public string GoTip
        {
            set { Set(ref _goTip, value); }
            get { return _goTip; }
        }

        public UserControl Control { get; }
        public Action CloseAction { get; set; }
        public Action<string> UpdateCaptionAction { get; set; }

        public void OnActive()
        {
            ((StxEditorView) BottomControl).TextEditor.IsFocusChanged = false;
            ((StxEditorView) TopControl).TextEditor.IsFocusChanged = false;
            if (Document.ShowCaretControl == null)
            {
                if (_isNeedLoad)
                    ((StxEditorView) BottomControl).TextEditor.Loaded += TextEditor_Loaded;
                else
                    ((StxEditorView) BottomControl).TextEditor.Focus();
                return;
            }

            if (Document.ShowCaretControl == TopControl)
            {
                if (_isNeedLoad)
                    ((StxEditorView) TopControl).TextEditor.Loaded += TextEditor_Loaded;
                else
                    ((StxEditorView) TopControl).TextEditor.Focus();
            }
            else
            {
                if (_isNeedLoad)
                    ((StxEditorView) BottomControl).TextEditor.Loaded += TextEditor_Loaded;
                else
                    ((StxEditorView) BottomControl).TextEditor.Focus();
            }

            _isNeedLoad = false;
        }

        private void TextEditor_Loaded(object sender, RoutedEventArgs e)
        {
            var control = (Control) sender;
            control.Focus();
            control.Loaded -= TextEditor_Loaded;
        }

        #region Pending

        public RelayCommand StartPendingRoutineCommand { get; }
        public RelayCommand AcceptPendingRoutineCommand { get; }
        public RelayCommand CancelPendingRoutineCommand { get; }
        public RelayCommand AssembledAcceptPendingRoutineCommand { get; }

        public RelayCommand CancelAcceptedPendingRoutineCommand { get; }
        public RelayCommand AcceptPendingProgramCommand { get; }
        public RelayCommand CancelPendingProgramCommand { get; }
        public RelayCommand TestAcceptedProgramCommand { get; }
        public RelayCommand UnTestAcceptedProgramCommand { get; }
        public RelayCommand AssembledAcceptedProgramCommand { get; }

        public RelayCommand CancelAcceptedProgramCommand { get; }

        public RelayCommand GoCommand { get; }

        public RelayCommand FinalizeCommand { get; }

        //private bool _canFinalizeCommand;
        private void ExecuteGoCommand()
        {
            var createEditorService = Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
            createEditorService?.CreateSTEditor(SelectedReference.Routine, SelectedReference.OnlineEditType,
                SelectedReference.Line - 1,
                SelectedReference.Column - 1, 0, SelectedReference.InnerDataReference);
        }

        private bool CanExecuteGoCommand()
        {
            return Options.IsConnecting;
        }

        private string _caption;
        private Visibility _onlineToolbarVisibility;
        private AoiDataReference _selectedReference;
        private string _goTip;
        private bool _isOnScreen = true;
        private bool _isDirty;
        private StxEditorView _focusedView;

        private void ExecuteStartPendingRoutineCommand()
        {
            StartPendingRoutineMenuitem.ExecuteStartPendingRoutine(Document, Options, Program);
        }

        private bool CanExecuteStartPendingRoutineCommand()
        {

            return StartPendingRoutineMenuitem.CanStartPendingRoutine(Routine);
        }

        private bool CanExecuteAcceptPendingRoutineCommand()
        {
            return AcceptPendingRoutineEditsMenuitem.CanAcceptPendingRoutineEdits(Options, Routine);
        }

        private bool CanExecuteCancelPendingRoutineCommand()
        {
            return CancelPendingRoutineEditsMenuitem.CanCancelPendingRoutineEdits(Program, Routine);
        }

        private void ExecuteAcceptPendingRoutineCommand()
        {
            AcceptPendingRoutineEditsMenuitem.ExecuteAcceptPendingRoutineEdits(Routine, Document, Options);
        }

        private void ExecuteCancelPendingRoutineCommand()
        {
            CancelPendingRoutineEditsMenuitem.ExecuteCancelPendingRoutineEdits(Document, Options, Program);
        }

        private void ExecuteAssembledAcceptPendingRoutineCommand()
        {
            AssembleAcceptedRoutineEditsMenuitem.ExecuteAssembleAcceptedRoutineEdits();
        }

        private bool CanExecuteAssembledAcceptPendingRoutineCommand()
        {
            return AssembleAcceptedRoutineEditsMenuitem.CanAssembleAcceptedRoutineEdits(Routine, Options);
        }

        private void ExecuteCancelAcceptedPendingRoutineCommand()
        {
            CancelAcceptedRoutineEditsMenuitem.ExecuteCancelAcceptedRoutineEdits();
        }

        private bool CanExecuteCancelAcceptedPendingRoutineCommand()
        {
            return CancelAcceptedRoutineEditsMenuitem.CanCancelAcceptedRoutineEdits(Routine, Options);
        }

        private void ExecuteAcceptPendingProgramCommand()
        {
            AcceptPendingProgramEditsMenuitem.ExecuteAcceptPendingProgramEdits(Routine);
        }

        private bool CanExecuteAcceptPendingProgramCommand()
        {
            return AcceptPendingProgramEditsMenuitem.CanAcceptPendingProgramEdits(Routine);
        }

        private void ExecuteCancelPendingProgramCommand()
        {
            CancelPendingProgramEditsMenuitem.ExecuteCancelPendingProgramEdits(Routine);
        }

        private bool CanExecuteCancelPendingProgramCommand()
        {
            return CancelPendingProgramEditsMenuitem.CanCancelPendingProgramEdits(Routine);
        }

        private void ExecuteTestAcceptedProgramCommand()
        {
            TestAcceptedProgramEditsMenuitem.ExecuteTestAcceptedProgramEdits(Routine, Options);
        }

        private bool CanExecuteTestAcceptedProgramCommand()
        {
            return TestAcceptedProgramEditsMenuitem.CanTestAcceptedProgramEdits(Routine);
        }

        private void ExecuteUnTestAcceptedProgramCommand()
        {
            UntestAcceptedProgramEditsMenuitem.ExecuteUntestAcceptedProgramEdits(Routine, Options);
        }

        private bool CanExecuteUnTestAcceptedProgramCommand()
        {
            return UntestAcceptedProgramEditsMenuitem.CanUntestAcceptedProgramEdits(Routine);
        }

        private void ExecuteAssembledAcceptedProgramCommand()
        {
            AssembleAcceptedProgramEditsMenuitem.ExecuteAssembleAcceptedProgramEdits(Routine);
        }

        private bool CanExecuteAssembledAcceptedProgramCommand()
        {
            return AssembleAcceptedProgramEditsMenuitem.CanAssembleAcceptedProgramEdits(Routine, Options);
        }

        private void ExecuteCancelAcceptedProgramCommand()
        {
            CancelAcceptedProgramEditsMenuitem.ExecuteCancelAcceptedProgramEdits(Routine);
        }

        private bool CanExecuteCancelAcceptedProgramCommand()
        {
            return CancelAcceptedProgramEditsMenuitem.CanCancelAcceptedProgramEdits(Routine, Options);
        }

        private void ExecuteFinalizeCommand()
        {
            FinalizeAllEditsInProgramMenuitem.ExecuteFinalizeAllEditsInProgram(Routine);
        }

        private void FinalizeCode()
        {
            Document.IsNeedReParse = false;
            Document.IsNeedBackground = false;
            Options.IsOnlyTextMarker = true;
            Options.HideAll = true;
            Options.CanEditorInput = !Routine.ParentController.IsOnline;
            Document.IsNeedBackground = true;
            Options.IsOnlyTextMarker = false;
            Document.IsNeedReParse = true;
            Options.CanAssembledAcceptPendingRoutineCommand = false;
            Options.CanCancelAcceptedPendingRoutineCommand = false;
            Options.CanAssembledAcceptedProgramCommand = false;
            Options.CanCancelAcceptedProgramCommand = false;
            RaiseCommand();
        }

        private bool CanExecuteFinalizeCommand()
        {
            if (Routine.ParentController.KeySwitchPosition == ControllerKeySwitch.RunKeySwitch) return false;
            return FinalizeAllEditsInProgramMenuitem.CanFinalizeAllEditsInProgram(Routine);
        }

        private void RaiseCommand()
        {
            StartPendingRoutineCommand.RaiseCanExecuteChanged();
            AcceptPendingRoutineCommand.RaiseCanExecuteChanged();
            CancelPendingRoutineCommand.RaiseCanExecuteChanged();
            AssembledAcceptPendingRoutineCommand.RaiseCanExecuteChanged();
            CancelAcceptedPendingRoutineCommand.RaiseCanExecuteChanged();
            AcceptPendingProgramCommand.RaiseCanExecuteChanged();
            CancelPendingProgramCommand.RaiseCanExecuteChanged();
            TestAcceptedProgramCommand.RaiseCanExecuteChanged();
            UnTestAcceptedProgramCommand.RaiseCanExecuteChanged();
            AssembledAcceptedProgramCommand.RaiseCanExecuteChanged();
            CancelAcceptedProgramCommand.RaiseCanExecuteChanged();
            FinalizeCommand.RaiseCanExecuteChanged();
        }

        #endregion

        public RelayCommand SaveCommand { get; }

        private void ExecuteSaveCommand()
        {
            ((StxEditorView)TopControl).TextEditor.SetLineSaved();
            ((StxEditorView)BottomControl).TextEditor.SetLineSaved();
            //FormatSaveTextCode();
            //Routine.IsUpdateChanged = true;
            IsDirty = false;
        }

        private void AddListener()
        {
            Controller controller = Routine.ParentController as Controller;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }

            PropertyChangedEventManager.AddHandler(Routine, Routine_PropertyChanged, string.Empty);
            PropertyChangedEventManager.AddHandler(Options, OptionPropertyChanged, string.Empty);
            var programModule = Routine.ParentCollection.ParentProgram;
            PropertyChangedEventManager.AddHandler(programModule, Program_PropertyChanged, string.Empty);

            WeakEventManager<TextDocument, DocumentChangeEventArgs>.AddHandler(Document.Document, "Changing",
                DocumentOnChanged);
        }

        private void RemoveListener()
        {
            Controller controller = Routine.ParentController as Controller;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }

            PropertyChangedEventManager.RemoveHandler(Routine, Routine_PropertyChanged, string.Empty);
            PropertyChangedEventManager.RemoveHandler(Options, OptionPropertyChanged, string.Empty);
            var programModule = Routine.ParentCollection.ParentProgram;
            PropertyChangedEventManager.RemoveHandler(programModule, Program_PropertyChanged, string.Empty);
            WeakEventManager<TextDocument, DocumentChangeEventArgs>.RemoveHandler(Document.Document, "Changing",
                DocumentOnChanged);
        }
        
        private void FormatSaveTextCode()
        {
            ((StxEditorView) TopControl).TextEditor.SetLineSaved();
            ((StxEditorView) BottomControl).TextEditor.SetLineSaved();
            try
            {
                if (Routine != null && !Routine.IsAbandoned)
                {
                    var currentOnlineEditType = Routine.CurrentOnlineEditType;
                    if (Document.NeedParse)
                    {
                        Routine.CurrentOnlineEditType = Routine.PendingEditsExist ? OnlineEditType.Pending : OnlineEditType.Original;
                        Document.IsNeedBackground = false;
                        Document.UpdateLexer(false);
                        Document.IsNeedBackground = true;
                    }
                    else
                    {
                        CodeSynchronization.WaitCompiler(Routine);
                    }
                    
                    Routine.CurrentOnlineEditType = OnlineEditType.Original;
                    Routine.CodeText = FormatCode(Document.Original,OnlineEditType.Original);
                    if (Document.HasPending)
                        if (Document.Original.Equals(Document.Pending, StringComparison.OrdinalIgnoreCase))
                        {
                            string[] tmp = new string[Routine.CodeText.Count];
                            Routine.CodeText.CopyTo(tmp);
                            Routine.PendingCodeText = tmp.ToList();
                        }
                        else
                        {
                            Routine.CurrentOnlineEditType = OnlineEditType.Pending;
                            Document.UpdateVariables(Document.Pending, true);
                            Routine.PendingCodeText = FormatCode(Document.Pending,OnlineEditType.Pending);
                        }

                    if (Document.HasTest)
                        if (Document.Original.Equals(Document.Test, StringComparison.OrdinalIgnoreCase))
                        {
                            string[] tmp = new string[Routine.CodeText.Count];
                            Routine.CodeText.CopyTo(tmp);
                            Routine.TestCodeText = tmp.ToList();
                        }
                        else
                        {
                            Routine.CurrentOnlineEditType = OnlineEditType.Test;
                            Document.UpdateVariables(Document.Test, true);
                            Routine.TestCodeText = FormatCode(Document.Test,OnlineEditType.Test);
                        }

                    Routine.CurrentOnlineEditType = currentOnlineEditType;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                IsDirty = false;
            }
        }

        private List<string> FormatCode(string snippets, OnlineEditType onlineEditType)
        {
            foreach (var variableInfo in Routine.GetCurrentVariableInfos(onlineEditType).ToList())
            {
                if (variableInfo.IsNum) continue;
                if (variableInfo.Offset + variableInfo.Name.Length >= snippets.Length) continue;
                if (variableInfo.IsInstr)
                {
                    var formatCode = Document.GetFormatInstrName(variableInfo.Name);
                    snippets = Replace(snippets, variableInfo.Offset, variableInfo.Name.Length, formatCode);
                }
                else
                {
                    var formatCode = ExtendOperation.FormatCode(((VariableInfo) variableInfo).AstNode,
                        variableInfo.Name, Routine);
                    snippets = Replace(snippets, variableInfo.Offset, variableInfo.Name.Length, formatCode);
                }
            }

            var codeText = snippets.Split('\n');
            return codeText.ToList();
        }

        private string Replace(string text, int index, int length, string newString)
        {
            if (string.IsNullOrEmpty(text)) return text;
            text = text.Remove(index, length).Insert(index, newString);
            return text;
        }

        private void Routine_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Name"))
            {
                UpdateCaptionAction?.Invoke($"{Routine.ParentCollection.ParentProgram.Name} - {Routine.Name}");
            }

            if (e.PropertyName.Equals("IsUpdateChanged"))
            {
                if (Routine.IsUpdateChanged)
                {
                    Document.UpdateCode();
                    Document.CanExecuteDocumentChanged = false;
                    Document.ChangedEditor();
                    Document.CanExecuteDocumentChanged = true;
                    ResetLineInitial();
                    Document.UpdateLexer();
                }
            }

            if (e.PropertyName.Equals("IsAbandoned"))
            {
                if (Routine.IsAbandoned) CloseAction?.Invoke();
            }
        }

        internal void ResetLineInitial()
        {
            ((StxEditorView) TopControl).TextEditor.SetLineInitial();
            ((StxEditorView) BottomControl).TextEditor.SetLineInitial();
        }

        private void Program_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            if (e.PropertyName == "Name")
            {
                UpdateCaptionAction?.Invoke($"{Routine.ParentCollection.ParentProgram.Name} - {Routine.Name}");
                return;
            }

            var program = sender as Program;
            if (program == null) return;

            if (e.PropertyName == "TestEditsMode")
            {
                ThreadHelper.JoinableTaskFactory.RunAsync(async delegate()
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    Document.OnlyResetCode();
                    //Document.StopVariableConnection();
                    if (program.TestEditsMode == TestEditsModeType.Test)
                    {
                        Caption = $"{Routine.ParentCollection.ParentProgram.Name} - {Routine.Name} (Testing Edits)";
                        Options.CanCancelAcceptedProgramCommand = false;
                        Options.CanAssembledAcceptedProgramCommand = true;
                        Options.CanAcceptPendingRoutineCommand = false;
                    }
                    else if (program.TestEditsMode == TestEditsModeType.UnTest)
                    {
                        Caption = $"{Routine.ParentCollection.ParentProgram.Name} - {Routine.Name}";
                        Options.CanCancelAcceptedProgramCommand = true;
                        Options.CanAssembledAcceptedProgramCommand = false;
                        Options.CanAcceptPendingRoutineCommand = true;
                    }
                    else
                    {
                        Caption = $"{Routine.ParentCollection.ParentProgram.Name} - {Routine.Name}";
                        Options.CanCancelAcceptedProgramCommand = false;
                        Options.CanAssembledAcceptedProgramCommand = false;
                        Options.CanAcceptPendingRoutineCommand = true;
                        Options.IsOnlyTextMarker = true;
                        if (Document.HasPending)
                        {
                            Document.IsNeedReParse = false;
                            Options.ShowPending = true;
                            Document.IsNeedReParse = true;
                        }
                        else
                        {
                            Document.IsNeedReParse = false;
                            Options.HideAll = true;
                            Document.IsNeedReParse = true;
                        }
                        Options.IsOnlyTextMarker = false;
                    }

                    RaiseCommand();
                });
            }

            if (e.PropertyName == "HasPending")
            {
                Document.Reset();
                if (Document.HasPending) Options.ShowPending = true;
                else if (Document.HasTest) Options.ShowTest = true;
                else Options.HideAll = true;
                RaiseCommand();
                IsDirty = false;
            }

            if (e.PropertyName == "UpdateAllRoutine")
            {
                if (program.UpdateAllRoutine)
                {
                    Document.Reset();
                    if (Document.HasPending)
                    {
                        Options.ShowPending = true;
                        Document.TransformToPending();
                    }
                    else if (Document.HasTest)
                    {
                        Options.ShowTest = true;
                        Document.TransformToTest();
                    }
                    else
                    {
                        Options.HideAll = true;
                        Document.TransformToOriginal();
                    }

                    if (Routine.IsError)
                    {
                        IsDirty = true;
                    }
                    else
                    {
                        IsDirty = false;
                        ResetLineInitial();
                    }

                }
            }

            if (e.PropertyName == "ExecuteFinalizeCode")
            {
                ThreadHelper.JoinableTaskFactory.Run(async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    if (program.ExecuteFinalizeCode)
                    {
                        FinalizeCode();
                        
                        if (!Routine.IsError)
                        {
                            ResetLineInitial();
                        }
                    }
                });
            }
        }

        private void OptionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsRaiseCommandStatus")
            {
                RaiseCommand();
                return;
            }

            if (e.PropertyName == "ResetLineInitial")
            {
                ResetLineInitial();
                return;
            }

            if (e.PropertyName == "IsDirty")
            {
                IsDirty = Options.IsDirty;
                return;
            }

            if (e.PropertyName == "ShowPending")
            {
                if (Options.ShowPending)
                {
                    bool canDo = !PendingCommandCheck || (OriginalCommandCheck || TestCommandCheck)||Options.IsOnlyTextMarker;
                    PendingCommandCheck = true;
                    if (!canDo) return;
                    RememberCaretIndex();
                    OriginalCommandCheck = false;
                    TestCommandCheck = false;
                    Document.TransformToPending();
                    RecoverCaretIndex();
                }
            }

            if (e.PropertyName == "ShowTest")
            {
                if (Options.ShowTest)
                {
                    bool canDo = !TestCommandCheck || (OriginalCommandCheck || PendingCommandCheck) || Options.IsOnlyTextMarker;
                    TestCommandCheck = true;
                    if (!canDo) return;
                    RememberCaretIndex();
                    OriginalCommandCheck = false;
                    PendingCommandCheck = false;
                    Document.TransformToTest();
                    RecoverCaretIndex();
                }
            }

            if (e.PropertyName == "ShowOriginal")
            {
                bool canDo = !OriginalCommandCheck || (TestCommandCheck || PendingCommandCheck) || Options.IsOnlyTextMarker;
                if (Document.HasPending || Document.HasTest)
                    OriginalCommandCheck = true;
                else
                {
                    OriginalCommandCheck = false;
                }

                if (!canDo) return;
                RememberCaretIndex();
                TestCommandCheck = false;
                PendingCommandCheck = false;
                Document.TransformToOriginal();
                RecoverCaretIndex();
            }

            if (e.PropertyName == "HideAll")
            {
                if (Options.HideAll)
                {
                    OriginalCommandCheck = false;
                    TestCommandCheck = false;
                    PendingCommandCheck = false;
                    RememberCaretIndex();
                    Document.TransformToOriginal();
                    RecoverCaretIndex();
                }
            }

            if (e.PropertyName == "ShowInLineDisplay")
            {
                if (!Options.ShowInLineDisplay)
                {
                    FocusedView?.TextEditor.TextArea.TextView.RecoveryError();
                }
            }
        }

        private void RecoverError()
        {
            ((StxEditorView)TopControl).TextEditor.TextArea.TextView.RecoveryError();
            ((StxEditorView)BottomControl).TextEditor.TextArea.TextView.RecoveryError();
        }

        private void RememberCaretIndex()
        {
            ((StxEditorView)TopControl).RememberCaretIndex();
            ((StxEditorView)BottomControl).RememberCaretIndex();
        }

        private void RecoverCaretIndex()
        {
            ((StxEditorView)TopControl).RecoverCaretIndex();
            ((StxEditorView)BottomControl).RecoverCaretIndex();
        }
        
        public void RecoveryError()
        {
            FocusedView?.TextEditor.TextArea.TextView.RecoveryError();
        }

        private void STEditorViewModel_GotFocus(object sender, RoutedEventArgs e)
        {
            FocusedView = (StxEditorView) sender;
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                foreach (VariableInfo currentVariableInfo in Routine.GetCurrentVariableInfos(Routine.CurrentOnlineEditType))
                {
                    currentVariableInfo.MinWidth = 0;
                }
                if (IsOnScreen)
                {
                    if (Routine.ParentController.IsOnline)
                    {
                        if (!_uiUpdateTimer.IsEnabled)
                            _uiUpdateTimer.Start();
                    }
                    else
                    {
                        if (_uiUpdateTimer.IsEnabled)
                            _uiUpdateTimer.Stop();
                    }

                    ((StxEditorView) BottomControl).TextEditor.TextArea.TextView.Redraw();
                    ((StxEditorView) TopControl).TextEditor.TextArea.TextView.Redraw();
                }
                RaiseCommand();
                OnlineToolbarVisibility = Routine.ParentController.IsOnline ? Visibility.Visible : Visibility.Collapsed;

            });
        }

        public int Apply()
        {
            //if (!Controller.GetInstance().IsDownloading)
            //{
            //    FormatSaveTextCode();
            //    Routine.IsUpdateChanged = false;
            //    IsDirty = false;
            //}
            ((StxEditorView)TopControl).TextEditor.SetLineSaved();
            ((StxEditorView)BottomControl).TextEditor.SetLineSaved();
            IsDirty = false;
            return 0;
        }

        public bool CanApply()
        {
            return IsDirty;
        }

        public bool CanPasted()
        {
            return FocusedView.TextEditor.CanPaste();
        }

        public bool CanCut()
        {
            return FocusedView.TextEditor.CanCut();
        }

        public bool CanCopy()
        {
            return FocusedView.TextEditor.CanCopy();
        }

        public void DoPaste()
        {
            FocusedView.TextEditor.Paste();
        }

        public void DoCut()
        {
            FocusedView.TextEditor.Cut();
        }

        public void DoCopy()
        {
            FocusedView.TextEditor.Copy();
        }
    }
}