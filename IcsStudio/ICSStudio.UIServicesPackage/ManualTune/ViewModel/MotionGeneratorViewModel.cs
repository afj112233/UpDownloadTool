using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIServicesPackage.ManualTune.Panel;
using ICSStudio.UIServicesPackage.ManualTune.Panel.MotionMove;
using ICSStudio.UIServicesPackage.ManualTune.Panel.MotionState;
using ICSStudio.UIServicesPackage.ManualTune.DataTypes;
using ICSStudio.Utils;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NLog;
using ICSStudio.MultiLanguage;

namespace ICSStudio.UIServicesPackage.ManualTune.ViewModel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public partial class MotionGeneratorViewModel : ViewModelBase
    {
        private object _selectedParam;
        internal static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly MAJParam _majParam;
        private readonly MAMParam _mamParam;
        private readonly MASParam _masParam;
        private readonly MDSParam _mdsParam;

        private List<IOptionPanelDescriptor> _optionPanelDescriptors;
        protected readonly List<IOptionPanel> OptionPanels = new List<IOptionPanel>();
        private OptionPanelNode _activeNode;
        private readonly IController _controller;
        private ITag _selectedAxis;
        private ObservableCollection<ITag> _allAxises;
        private string _axisState;
        private string _axisFault;
        private string _startInhibited;

        private string _optionPanelTitle;
        private object _optionPanelContent;
        private string _executionError;

        private readonly DispatcherTimer _timer;

        public MotionGeneratorViewModel(ITag axisTag)
        {

            Contract.Assert(axisTag != null);

            _controller = axisTag.ParentController;

            Controller controller = _controller as Controller;
            if (controller != null)
            {
                //CollectionChangedEventManager.AddHandler(
                //    _controller.Tags, OnTagCollectionChanged);

                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
                WeakEventManager<Controller, EventArgs>.AddHandler(
                    controller, "KeySwitchChanged", OnKeySwitchChanged);
                WeakEventManager<Controller, EventArgs>.AddHandler(
                    controller, "OperationModeChanged", OnOperationModeChanged);
            }

            AxisTag = axisTag;
            MoreCommands = new RelayCommand(ExecuteMoreCommands);
            ExecuteCommand = new RelayCommand(ExecuteExecuteCommand,CanExecuteExecuteCommand);
            DisableAxisCommand = new RelayCommand(ExecuteDisableAxisCommand,CanExecuteDisableCommand);

            AxisPropertiesCommand = new RelayCommand<ITag>(ExecuteAxisPropertiesCommand);

            _controller = axisTag.ParentController;
            Index = 1;

            _majParam = new MAJParam();
            _majParam.OnDynamicPropertyChanged += OnReadOnlyChanged;
            _mamParam = new MAMParam();
            _mamParam.OnDynamicPropertyChanged += OnReadOnlyChanged;
            _masParam = new MASParam();
            _masParam.OnDynamicPropertyChanged += OnReadOnlyChanged;
            _mdsParam = new MDSParam();
            _mdsParam.OnDynamicPropertyChanged += OnReadOnlyChanged;

            SelectedCommand = MotionGeneratorCommand.MSO;

            UpdateAxisSource();
            InitializeOptionPanelNodes();

            // for default select
            var dataType = axisTag.DataTypeInfo.DataType;
            SelectedAxis = axisTag;
            OptionPanelNodes[0].Children[0].IsSelected = true;

            // update timer, 500ms
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };

            _timer.Tick += CycleUpdateTimerHandle;

            _timer.Start();
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("Title");
        }

        public ITag AxisTag { get; }
        public RelayCommand MoreCommands { get; }
        public RelayCommand ExecuteCommand { get; }
        public RelayCommand DisableAxisCommand { get; }
        public RelayCommand<ITag> AxisPropertiesCommand { get; }
        public int Index { get; }

        //TODO(gjc): add code here
        public string AxisState
        {
            get { return _axisState; }
            set { Set(ref _axisState, value); }
        }

        public string AxisFault
        {
            get { return _axisFault; }
            set { Set(ref _axisFault, value); }
        }

        public string StartInhibited
        {
            get { return _startInhibited; }
            set { Set(ref _startInhibited, value); }
        }

        public MotionGeneratorCommand SelectedCommand { get; private set; }

        public object SelectedParam
        {
            get { return _selectedParam; }
            set { Set(ref _selectedParam, value); }
        }

        public Visibility ParamVisibility
        {
            get
            {
                if (SelectedParam != null)
                    return Visibility.Visible;
                return Visibility.Hidden;
            }
        }

        private void ExecuteMoreCommands()
        {
            ICreateDialogService createDialogService =
                Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;

            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = (IVsUIShell) Package.GetGlobalService(typeof(SVsUIShell));

            if (createDialogService != null)
            {
                var window = createDialogService.CreateMotionDirectCommandsDialog(AxisTag);

                window.Show(uiShell);
            }
        }

        private void ExecuteAxisPropertiesCommand(ITag axis)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (axis != null)
            {
                ICreateDialogService createDialogService =
                    Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;

                var uiShell = (IVsUIShell)Package.GetGlobalService(typeof(SVsUIShell));

                if (createDialogService != null && uiShell != null)
                {
                    var dataType = axis.DataTypeInfo.DataType;
                    Window window = null;
                    if (dataType is AXIS_CIP_DRIVE)
                    {
                        window =
                            createDialogService.CreateAxisCIPDriveProperties(axis);
                    }
                    else if (dataType is AXIS_VIRTUAL)
                    {
                        window = createDialogService.CreateAxisVirtualProperties(axis);
                    }

                    window?.Show(uiShell);
                }
            }
        }

        internal void ChangeCommand(string command)
        {
            foreach (var i in OptionPanelNodes)
            {
                foreach (var j in i.Children)
                {
                    if (j.Title == command)
                    {
                        _activeNode = j;
                        SelectNode(_activeNode);
                        break;
                    }
                }
            }

            SelectedCommand = EnumUtils.Parse<MotionGeneratorCommand>(command);
            switch (SelectedCommand)
            {
                case MotionGeneratorCommand.MAJ:
                    SelectedParam = _majParam;
                    break;
                case MotionGeneratorCommand.MAM:
                    SelectedParam = _mamParam;
                    break;
                case MotionGeneratorCommand.MAS:
                    SelectedParam = _masParam;
                    break;
                case MotionGeneratorCommand.MDS:
                    SelectedParam = _mdsParam;
                    break;
                default:
                    SelectedParam = null;
                    break;
            }

            RaisePropertyChanged("ParamVisibility");
        }

        private void OnReadOnlyChanged(object obj)
        {
            if (obj != null)
            {
                SelectedParam = null;
                SelectedParam = obj;
            }
        }

        public List<OptionPanelNode> OptionPanelNodes { get; set; }
        public string OptionPanelTitle
        {
            get { return _optionPanelTitle; }
            set { Set(ref _optionPanelTitle, value); }
        }


        public object OptionPanelContent
        {
            get { return _optionPanelContent; }
            set { Set(ref _optionPanelContent, value); }
        }

        public string ExecutionError
        {
            get { return _executionError; }
            set { Set(ref _executionError, value); }
        }

        public ObservableCollection<ITag> AllAxises
        {
            get { return _allAxises; }
            set { Set(ref _allAxises, value); }
        }

        //private void OnTagCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    bool haveAxis = false;

        //    if (e.Action == NotifyCollectionChangedAction.Reset)
        //    {
        //        UpdateAxisSource();
        //    }
        //    else if (e.Action == NotifyCollectionChangedAction.Add)
        //    {
        //        foreach (var item in e.NewItems)
        //        {
        //            ITag tag = item as ITag;
        //            if (tag != null)
        //            {
        //                if (tag.DataTypeInfo.DataType.IsAxisType)
        //                    haveAxis = true;

        //            }
        //        }

        //        if (haveAxis)
        //            UpdateAxisSource();
        //    }
        //    else if (e.Action == NotifyCollectionChangedAction.Remove)
        //    {
        //        foreach (var item in e.OldItems)
        //        {
        //            ITag tag = item as ITag;
        //            if (tag != null)
        //            {
        //                if (tag.DataTypeInfo.DataType.IsAxisType)
        //                    haveAxis = true;

        //            }
        //        }

        //        if (haveAxis)
        //            UpdateAxisSource();
        //    }
        //}

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            Refresh();
        }

        private void OnOperationModeChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        private void OnKeySwitchChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        private void Refresh()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(
                async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    ExecuteCommand.RaiseCanExecuteChanged();
                    DisableAxisCommand.RaiseCanExecuteChanged();
                });
        }

        private void SelectNode(OptionPanelNode node)
        {
            if (node == _activeNode)
                return;

            if (_activeNode != null)
            {
                _activeNode.IsActive = false;
            }

            _activeNode = node;

            OptionPanelTitle = node.Label;
            OptionPanelContent = node.Content;

            node.IsExpanded = true;
            node.IsActive = true;

            RaisePropertyChanged("Title");
            RaisePropertyChanged("AxisStateVisibility");
            RaisePropertyChanged("StartInhibitedVisibility");

            ExecuteCommand.RaiseCanExecuteChanged();
        }
        public ITag SelectedAxis
        {
            get { return _selectedAxis; }
            set
            {
                _selectedAxis = value;

                RaisePropertyChanged("Title");
                RaisePropertyChanged("AxisStateVisibility");
                RaisePropertyChanged("StartInhibitedVisibility");

                ExecuteCommand.RaiseCanExecuteChanged();
            }
        }

        public string Title
        {
            get
            {
                var defaultViewModel = _activeNode?.OptionPanelDescriptor.OptionPanel as DefaultViewModel;
                var selectedTag = defaultViewModel?.SelectedTag;
                //Motion Direct Commands...
                return selectedTag != null
                    ? LanguageManager.GetInstance().ConvertSpecifier("Motion Direct Commands...") + $" - {selectedTag.Name}:{Index}"
                    : LanguageManager.GetInstance().ConvertSpecifier("Motion Direct Commands...") + $":{Index}";

            }
        }

        private void UpdateExecutionError(bool error)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(
                async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    ExecutionError = error ? "Execution Error." : string.Empty;
                });
        }

        private void UpdateAxisSource()
        {
            List<ITag> axisList = new List<ITag>();
            foreach (var tag in _controller.Tags)
            {
                if (tag.DataTypeInfo.DataType.IsAxisType)
                    axisList.Add(tag);
            }

            AllAxises = new ObservableCollection<ITag>(axisList);
        }

        private void InitializeOptionPanelNodes()
        {
            //
            var motionStatePanelDescriptors = new List<IOptionPanelDescriptor>
            {
                new DefaultOptionPanelDescriptor("11", "MSO", "Motion Servo On",
                    new MSOViewModel(new MSOPanel(), this), null),
                new DefaultOptionPanelDescriptor("12", "MSF", "Motion Servo Off",
                    new MSFViewModel(new MSFPanel(), this), null),
                new DefaultOptionPanelDescriptor("17", "MDS", "Motion Drive Start",
                    new MDSViewModel(new MDSPanel(), this,_mdsParam), null),
                new DefaultOptionPanelDescriptor("18", "MAFR", "Motion Axis Fault Reset",
                    new MAFRViewModel(new MAFRPanel(), this), null)
            };

            var motionMovePanelDescriptors = new List<IOptionPanelDescriptor>
            {
                new DefaultOptionPanelDescriptor("21", "MAS", "Motion Axis Stop",
                    new MASViewModel(new MASPanel(), this,_masParam), null),
                new DefaultOptionPanelDescriptor("22", "MAH", "Motion Axis Home",
                    new MAHViewModel(new MAHPanel(), this), null),
                new DefaultOptionPanelDescriptor("23", "MAJ", "Motion Axis Jog",
                    new MAJViewModel(new MAJPanel(), this,_majParam), null),
                new DefaultOptionPanelDescriptor("24", "MAM", "Motion Axis Move",
                    new MAMViewModel(new MAMPanel(), this,_mamParam), null),
            };


            _optionPanelDescriptors = new List<IOptionPanelDescriptor>
            {
                new DefaultOptionPanelDescriptor("1", "Motion State",
                    "Motion State Direct Command Group",
                    new DefaultViewModel(new DefaultPanel(), this), motionStatePanelDescriptors),
                new DefaultOptionPanelDescriptor("2", "Motion Move",
                    "Motion Move Direct Command Group",
                    new DefaultViewModel(new DefaultPanel(), this),
                    motionMovePanelDescriptors),

            };
            OptionPanelNodes = _optionPanelDescriptors.Select(op => new OptionPanelNode(op, this)).ToList();


        }

        private void CycleUpdateTimerHandle(object sender, EventArgs e)
        {
            // cycle update axis state
            var axis = SelectedAxis as Tag;
            AxisCIPDrive axisCIPDrive = axis?.DataWrapper as AxisCIPDrive;

            if (axisCIPDrive != null)
            {
                if (axis.ParentController.IsOnline)
                {
                    var axisDescriptor = new AxisDescriptor(axisCIPDrive.CIPAxis);

                    AxisState = axisDescriptor.CIPAxisState;

                    // axis fault
                    AxisFault = axisDescriptor.AxisFault;

                    // start Inhibited
                    StartInhibited = axisDescriptor.StartInhibited;
                }
                else
                {
                    AxisState = string.Empty;
                    AxisFault = string.Empty;
                    StartInhibited = string.Empty;
                }

            }

        }
    }
}
