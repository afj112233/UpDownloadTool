using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Project;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.ProgramProperties.Panel
{
    class ConfigurationViewModel<T> : ViewModelBase, IOptionPanel, ICanBeDirty where T : FrameworkElement
    {
        private readonly Program _program;
        private bool _inhibited;
        private bool _isInhibitedEnabled;
        private bool _isDirty;
        private IRoutine _selectMain;
        private IRoutine _selectFault;
        private bool _completeStateIfNotImpl;
        private InitialStateType _selectedInitialState;
        private LossOfCommCmdType _selectedLossOfCommCmd;
        private bool _autoValueAssignStepToPhase;
        private bool _autoValueAssignPhaseToStepOnComplete;
        private bool _autoValueAssignPhaseToStepOnStopped;
        private bool _autoValueAssignPhaseToStepOnAborted;
        private bool _retainSequenceIDOnReset;
        private ValuesToUseOnStartType _valuesToUseOnStart;
        private ValuesToUseOnResetType _valuesToUseOnReset;
        private ExternalRequestActionType _selectedExternalRequestAction;
        private int _unitID;
        private byte _initialStepIndex;
        private IRoutine _mainRoutine;
        private IRoutine _faultRoutine;
        private IRoutine _noneRoutine;

        public ConfigurationViewModel(T panel, IProgram program)
        {
            Control = panel;

            panel.DataContext = this;
            _program = program as Program;
            MainList = new ObservableCollection<IRoutine>();
            FaultList = new ObservableCollection<IRoutine>();
            InitialStateList = EnumHelper.ToDataSource<InitialStateType>();
            LossOfCommCmdList = EnumHelper.ToDataSource<LossOfCommCmdType>();
            ExternalRequestActionList = EnumHelper.ToDataSource<ExternalRequestActionType>();

            CollectionChangedEventManager.AddHandler(program.Routines, OnCollectionChanged);

            _noneRoutine = new RLLRoutine(null) {Name = "<none>"};
            SetValue();

            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                _program?.ParentController as Controller, "IsOnlineChanged", OnIsOnlineChanged);
            _isInhibitedEnabled = _program?.ParentController.KeySwitchPosition != ControllerKeySwitch.RunKeySwitch;
            WeakEventManager<Controller, EventArgs>.AddHandler(_program?.ParentController as Controller, "KeySwitchChanged", OnKeySwitchChanged);
        } 

        public override void Cleanup()
        {
            CollectionChangedEventManager.RemoveHandler(_program.Routines, OnCollectionChanged);
            PropertyChangedEventManager.RemoveHandler(_program, _program_PropertyChanged, "Inhibited");
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                _program?.ParentController as Controller, "IsOnlineChanged", OnIsOnlineChanged);
            WeakEventManager<Controller, EventArgs>.RemoveHandler(_program?.ParentController as Controller, "KeySwitchChanged", OnKeySwitchChanged);
        }
        
        public void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender == _program.Routines && e.Action == NotifyCollectionChangedAction.Add)
            {
                IRoutine routine = (IRoutine) ((object[]) e.NewItems.SyncRoot)[0];
                if ((routine.Name == "Aborting" || routine.Name == "Resetting" || routine.Name == "Restarting" ||
                     routine.Name == "Running" ||
                     routine.Name == "Stopping") && _program.Type == ProgramType.Phase)
                {
                    return;
                }

                if (routine.IsMainRoutine)
                {
                    _mainRoutine = routine;
                    MainList.Add(routine);
                    SelectMain = routine;
                }
                else if (routine.IsFaultRoutine)
                {
                    _faultRoutine = routine;
                    FaultList.Add(routine);
                    SelectFault = routine;
                }
                else
                {
                    MainList.Add(routine);
                    FaultList.Add(routine);
                }
            }
            else if (sender == _program.Routines && e.Action == NotifyCollectionChangedAction.Remove)
            {
                IRoutine routine = (IRoutine) (e.OldItems)[0];
                if ((routine.Name == "Aborting" || routine.Name == "Resetting" || routine.Name == "Restarting" ||
                     routine.Name == "Running" ||
                     routine.Name == "Stopping") && _program.Type == ProgramType.Phase)
                {
                    return;
                }

                if (routine.IsMainRoutine)
                {
                    _mainRoutine = null;
                    SelectMain = _noneRoutine;

                    MainList.Remove(routine);
                    FaultList.Remove(routine);
                }

                else if (routine.IsFaultRoutine)
                {
                    _faultRoutine = null;
                    SelectFault = _noneRoutine;

                    FaultList.Remove(routine);
                    MainList.Remove(routine);
                }
                else
                {
                    MainList.Remove(routine);
                    FaultList.Remove(routine);
                }
            }
        }

        private bool _isInhibitedDirty;

        public void SetValue()
        {
            _selectMain = _noneRoutine;
            _selectFault = _noneRoutine;
            _inhibited = _program.Inhibited;
            _isInhibitedDirty = false;
            foreach (var routine in _program.Routines)
            {
                if ((routine.Name == "Aborting" || routine.Name == "Resetting" || routine.Name == "Restarting" ||
                     routine.Name == "Running" ||
                     routine.Name == "Stopping") && _program.Type == ProgramType.Phase)
                {
                    continue;
                }

                MainList.Add(routine);
                FaultList.Add(routine);
                if (routine.IsFaultRoutine)
                {
                    SelectFault = routine;
                    _faultRoutine = routine;
                }

                if (routine.IsMainRoutine)
                {
                    SelectMain = routine;
                    _mainRoutine = routine;
                }
            }

            MainList.Insert(0, _noneRoutine);
            FaultList.Insert(0, _noneRoutine);

            if (_program.Type == ProgramType.Phase)
            {
                _selectedInitialState = _program.ProgramProperties.InitialState;
                CompleteStateIfNotImpl =
                    (_program.ProgramProperties.CompleteStateIfNotImpl != CompleteStateIfNotImplType.NoAction);
                InitialStepIndex = _program.ProgramProperties.InitialStepIndex;
                SelectedLossOfCommCmd = _program.ProgramProperties.LossOfCommCmd;
                SelectedExternalRequestAction = _program.ProgramProperties.ExternalRequestAction;
                AutoValueAssignStepToPhase = _program.ProgramProperties.AutoValueAssignStepToPhase;
                AutoValueAssignPhaseToStepOnComplete =
                    _program.ProgramProperties.AutoValueAssignPhaseToStepOnComplete;
                AutoValueAssignPhaseToStepOnStopped =
                    _program.ProgramProperties.AutoValueAssignPhaseToStepOnStopped;
                AutoValueAssignPhaseToStepOnAborted =
                    _program.ProgramProperties.AutoValueAssignPhaseToStepOnAborted;
            }

            if (_program.Type == ProgramType.Sequence)
            {
                RetainSequenceIDOnReset = _program.ProgramProperties.RetainSequenceIDOnReset;
                UnitID = _program.ProgramProperties.UnitID;
                ValuesToUseOnStart = _program.ProgramProperties.ValuesToUseOnStart;
                ValuesToUseOnReset = _program.ProgramProperties.ValuesToUseOnReset;
            }
            PropertyChangedEventManager.AddHandler(_program, _program_PropertyChanged, "Inhibited");
        }
        

        private void _program_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!_isInhibitedDirty)
            {
                _inhibited = _program.Inhibited;
                RaisePropertyChanged("Inhibited");
            }
        }

        public void Compare()
        {
            IsDirty = false;
            if (Inhibited != _program.Inhibited) IsDirty = true;
            if (_program.Type != ProgramType.Sequence)
            {
                if ((string.IsNullOrEmpty(_program.FaultRoutineName) ? "<none>" : _program.FaultRoutineName) !=
                    SelectFault.Name)
                    IsDirty = true;
                if ((string.IsNullOrEmpty(_program.MainRoutineName) ? "<none>" : _program.MainRoutineName) !=
                    SelectMain.Name)
                    IsDirty = true;
            }

            if (_program.Type == ProgramType.Phase)
            {
                if (SelectedInitialState != _program.ProgramProperties.InitialState) IsDirty = true;
                if (CompleteStateIfNotImpl !=
                    (_program.ProgramProperties.CompleteStateIfNotImpl != CompleteStateIfNotImplType.NoAction))
                    IsDirty = true;
                if (InitialStepIndex != _program.ProgramProperties.InitialStepIndex) IsDirty = true;
                if (SelectedLossOfCommCmd != _program.ProgramProperties.LossOfCommCmd) IsDirty = true;
                if (SelectedExternalRequestAction != _program.ProgramProperties.ExternalRequestAction) IsDirty = true;
                if (AutoValueAssignStepToPhase != _program.ProgramProperties.AutoValueAssignStepToPhase) IsDirty = true;
                if (AutoValueAssignPhaseToStepOnComplete !=
                    _program.ProgramProperties.AutoValueAssignPhaseToStepOnComplete) IsDirty = true;
                if (AutoValueAssignPhaseToStepOnStopped !=
                    _program.ProgramProperties.AutoValueAssignPhaseToStepOnStopped) IsDirty = true;
                if (AutoValueAssignPhaseToStepOnAborted !=
                    _program.ProgramProperties.AutoValueAssignPhaseToStepOnAborted) IsDirty = true;
            }

            if (_program.Type == ProgramType.Sequence)
            {
                if (RetainSequenceIDOnReset != _program.ProgramProperties.RetainSequenceIDOnReset) IsDirty = true;
                if (UnitID != _program.ProgramProperties.UnitID) IsDirty = true;
                if (ValuesToUseOnStart != _program.ProgramProperties.ValuesToUseOnStart) IsDirty = true;
                if (ValuesToUseOnReset != _program.ProgramProperties.ValuesToUseOnReset) IsDirty = true;
            }
        }

        public bool GetFaultListEnabled()
        {
            var projectInfoService = Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            var controller = projectInfoService?.Controller;
            if (_program != null && controller != null)
            {
                return
                    (_program.Name != controller.MajorFaultProgram &&
                     _program.Name != controller.PowerLossProgram) && !_program.ParentController.IsOnline;
            }
            return false;
        }

        public ObservableCollection<IRoutine> MainList { set; get; }
        public ObservableCollection<IRoutine> FaultList { set; get; }

        public bool IsMainListEnabled => !_program.ParentController.IsOnline;

        //暂时不支持program的fault，从l5x导入的项目可能会用到
        //public bool IsFaultListEnabled => GetFaultListEnabled();

        public bool IsFaultListEnabled => false;

        public IList InitialStateList { set; get; }

        public InitialStateType SelectedInitialState
        {
            set
            {
                _selectedInitialState = value;
                Compare();
            }
            get { return _selectedInitialState; }
        }

        public bool CompleteStateIfNotImpl
        {
            set
            {
                _completeStateIfNotImpl = value;
                Compare();
            }
            get { return _completeStateIfNotImpl; }
        }

        public IList LossOfCommCmdList { set; get; }

        public ExternalRequestActionType SelectedExternalRequestAction
        {
            set
            {
                _selectedExternalRequestAction = value;
                Compare();
            }
            get { return _selectedExternalRequestAction; }
        }

        public LossOfCommCmdType SelectedLossOfCommCmd
        {
            set
            {
                _selectedLossOfCommCmd = value;
                Compare();
            }
            get { return _selectedLossOfCommCmd; }
        }

        public IList ExternalRequestActionList { set; get; }


        public byte InitialStepIndex
        {
            set
            {
                _initialStepIndex = value;
                Compare();
            }
            get { return _initialStepIndex; }
        }

        public bool AutoValueAssignStepToPhase
        {
            set
            {
                _autoValueAssignStepToPhase = value;
                Compare();
            }
            get { return _autoValueAssignStepToPhase; }
        }

        public bool AutoValueAssignPhaseToStepOnComplete
        {
            set
            {
                _autoValueAssignPhaseToStepOnComplete = value;
                Compare();
            }
            get { return _autoValueAssignPhaseToStepOnComplete; }
        }

        public bool AutoValueAssignPhaseToStepOnStopped
        {
            set
            {
                _autoValueAssignPhaseToStepOnStopped = value;
                Compare();
            }
            get { return _autoValueAssignPhaseToStepOnStopped; }
        }

        public bool AutoValueAssignPhaseToStepOnAborted
        {
            set
            {
                _autoValueAssignPhaseToStepOnAborted = value;
                Compare();
            }
            get { return _autoValueAssignPhaseToStepOnAborted; }
        }

        public bool RetainSequenceIDOnReset
        {
            set
            {
                _retainSequenceIDOnReset = value;
                Compare();
            }
            get { return _retainSequenceIDOnReset; }
        }

        public bool GenerateSequenceEvents { set; get; }

        public int UnitID
        {
            set
            {
                _unitID = value;
                Compare();
            }
            get { return _unitID; }
        }


        public ValuesToUseOnStartType ValuesToUseOnStart
        {
            set
            {
                _valuesToUseOnStart = value;
                Compare();
            }
            get { return _valuesToUseOnStart; }
        }

        public ValuesToUseOnResetType ValuesToUseOnReset
        {
            set
            {
                _valuesToUseOnReset = value;
                Compare();
            }
            get { return _valuesToUseOnReset; }
        }

        public IRoutine SelectMain
        {
            set
            {
                if (SelectMain == null && value.Name == "<none>")
                {
                    _selectMain = value;
                }
                else
                {
                    if (SelectMain != value)
                    {
                        if (value.Name != "<none>")
                        {
                            FaultList.Remove(value);

                            if (_selectMain != null && _selectMain.Name != "<none>")
                                FaultList.Add(_selectMain);
                        }
                        else
                        {
                            if (_selectMain != null && _selectMain.Name != "<none>")
                                FaultList.Add(_selectMain);
                        }

                        FaultList = new ObservableCollection<IRoutine>(FaultList.OrderBy(x => x.Name));
                        _selectMain = value;
                        RaisePropertyChanged("FaultList");
                        RaisePropertyChanged("SelectMain");
                        Compare();
                    }
                }

            }
            get { return _selectMain; }
        }

        public IRoutine SelectFault
        {
            set
            {
                if (SelectFault == null && value.Name == "<none>")
                {
                    _selectFault = value;
                }
                else
                {
                    if (SelectFault != value)
                    {
                        if (value.Name != "<none>")
                        {
                            MainList.Remove(value);
                            if (_selectFault != null && _selectFault.Name != "<none>")
                                MainList.Add(_selectFault);
                        }
                        else
                        {
                            if (_selectFault != null && _selectFault.Name != "<none>")
                                MainList.Add(_selectFault);
                        }

                        MainList = new ObservableCollection<IRoutine>(MainList.OrderBy(x => x.Name));
                        _selectFault = value;
                        RaisePropertyChanged("MainList");
                        RaisePropertyChanged("SelectFault");
                        Compare();
                    }
                }

            }
            get { return _selectFault; }
        }
        
        public bool Inhibited
        {
            set
            {
                _inhibited = value;
                _isInhibitedDirty = true;
                Compare();
                RaisePropertyChanged();
            }
            get { return _inhibited; }
        }

        public bool IsInhibitedEnabled
        {
            get { return _isInhibitedEnabled; }
            set { Set(ref _isInhibitedEnabled, value); }
        }

        public void Save()
        {
            if (SelectMain.Name != _program.MainRoutineName)
            {
                if (!(SelectMain.Name == "<none>" && string.IsNullOrEmpty(_program.MainRoutineName)))
                {
                    if (_mainRoutine != null)
                        _mainRoutine.IsMainRoutine = false;

                    if (SelectMain.Name != "<none>")
                        SelectMain.IsMainRoutine = true;

                    _program.RaisePropertyChanged(nameof(_program.MainRoutineName));
                }
            }

            if (SelectFault.Name != _program.FaultRoutineName)
            {
                if (!(SelectFault.Name == "<none>" && string.IsNullOrEmpty(_program.FaultRoutineName)))
                {
                    if (_faultRoutine != null)
                        _faultRoutine.IsFaultRoutine = false;

                    if (SelectFault.Name != "<none>")
                        SelectFault.IsFaultRoutine = true;

                    _program.RaisePropertyChanged(nameof(_program.FaultRoutineName));
                }
            }

            _faultRoutine = SelectFault;
            _mainRoutine = SelectMain;

            if (_program.Type == ProgramType.Phase)
            {
                _program.ProgramProperties.InitialState = SelectedInitialState;
                _program.ProgramProperties.CompleteStateIfNotImpl = !CompleteStateIfNotImpl
                    ? CompleteStateIfNotImplType.NoAction
                    : CompleteStateIfNotImplType.StateComplete;
                _program.ProgramProperties.InitialStepIndex = InitialStepIndex;
                _program.ProgramProperties.LossOfCommCmd = SelectedLossOfCommCmd;
                _program.ProgramProperties.AutoValueAssignStepToPhase = AutoValueAssignStepToPhase;
                _program.ProgramProperties.AutoValueAssignPhaseToStepOnComplete = AutoValueAssignPhaseToStepOnComplete;
                _program.ProgramProperties.AutoValueAssignPhaseToStepOnStopped = AutoValueAssignPhaseToStepOnStopped;
                _program.ProgramProperties.AutoValueAssignPhaseToStepOnAborted = AutoValueAssignPhaseToStepOnAborted;
                _program.ProgramProperties.ExternalRequestAction = SelectedExternalRequestAction;
            }

            if (_program.Type == ProgramType.Sequence)
            {

                _program.ProgramProperties.RetainSequenceIDOnReset = RetainSequenceIDOnReset;
                _program.ProgramProperties.UnitID = UnitID;
                _program.ProgramProperties.ValuesToUseOnStart = ValuesToUseOnStart;
                _program.ProgramProperties.ValuesToUseOnReset = ValuesToUseOnReset;
            }

            if (_program.Inhibited != Inhibited)
            {
                PropertyChangedEventManager.RemoveHandler(_program,_program_PropertyChanged, "Inhibited");
                _program.Inhibited = Inhibited;
                PropertyChangedEventManager.AddHandler(_program, _program_PropertyChanged, "Inhibited");
                _isInhibitedDirty = false;
                if (_program.ParentController.IsOnline)
                {
                    ((Controller)_program.ParentController)?.SetInhibited(_program, Inhibited);
                }
            }
            Compare();
        }

        public object Owner { get; set; }
        public object Control { get; }

        public void LoadOptions()
        {
            //throw new NotImplementedException();
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
                if (_isDirty != value)
                {
                    _isDirty = value;
                    IsDirtyChanged?.Invoke(this, EventArgs.Empty);
                }

                Set(ref _isDirty, value);
            }
        }

        public event EventHandler IsDirtyChanged;

        public bool CheckMainRoutine()
        {
            foreach (var routine in _program.Routines)
            {
                var st = routine as STRoutine;
                if (st != null)
                {
                    foreach (var variableInfo in st.GetCurrentVariableInfos(OnlineEditType.Original).Where(v=>v.IsUseForJSR))
                    {
                        if (variableInfo.Name.Equals(SelectMain.Name, StringComparison.OrdinalIgnoreCase)) return false;
                    }

                    if (st.PendingCodeText != null)
                    {
                        foreach (var variableInfo in st.GetCurrentVariableInfos(OnlineEditType.Pending).Where(v => v.IsUseForJSR))
                        {
                            if (variableInfo.Name.Equals(SelectMain.Name, StringComparison.OrdinalIgnoreCase)) return false;
                        }
                    }

                    if (st.TestCodeText != null)
                    {
                        foreach (var variableInfo in st.GetCurrentVariableInfos(OnlineEditType.Test).Where(v => v.IsUseForJSR))
                        {
                            if (variableInfo.Name.Equals(SelectMain.Name, StringComparison.OrdinalIgnoreCase)) return false;
                        }
                    }
                }
            }

            return true;
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                RaisePropertyChanged(nameof(IsMainListEnabled));
                RaisePropertyChanged(nameof(IsFaultListEnabled));
            });
        }

        private void OnKeySwitchChanged(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                Inhibited = _program.Inhibited;
                IsInhibitedEnabled = _program.ParentController.KeySwitchPosition != ControllerKeySwitch.RunKeySwitch;
            });
        }
    }
}
