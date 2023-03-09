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
using ICSStudio.Cip.Other;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIServicesPackage.MotionDirectCommands.Panel;
using ICSStudio.UIServicesPackage.MotionDirectCommands.Panel.MotionEvent;
using ICSStudio.UIServicesPackage.MotionDirectCommands.Panel.MotionGroup;
using ICSStudio.UIServicesPackage.MotionDirectCommands.Panel.MotionMove;
using ICSStudio.UIServicesPackage.MotionDirectCommands.Panel.MotionState;
using Microsoft.VisualStudio.Shell;
using NLog;
using ICSStudio.MultiLanguage;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal partial class MotionDirectCommandsViewModel : ViewModelBase
    {
        internal static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private List<IOptionPanelDescriptor> _optionPanelDescriptors;

        // ReSharper disable once CollectionNeverQueried.Global
        protected readonly List<IOptionPanel> OptionPanels = new List<IOptionPanel>();

        private OptionPanelNode _activeNode;

        private string _optionPanelTitle;
        private object _optionPanelContent;

        private readonly IController _controller;
        private ITag _selectedAxis;
        private ITag _selectedMotionGroup;
        private ObservableCollection<ITag> _allAxises;
        private ObservableCollection<ITag> _allMotionGroups;
        private string _axisState;
        private string _axisFault;
        private string _startInhibited;
        private string _executionError;

        private readonly DispatcherTimer _timer;

        public MotionDirectCommandsViewModel(ITag motion, int index)
        {
            Contract.Assert(motion != null);

            _controller = motion.ParentController;

            Controller controller = _controller as Controller;
            if (controller != null)
            {
                CollectionChangedEventManager.AddHandler(
                    _controller.Tags, OnTagCollectionChanged);

                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
                WeakEventManager<Controller, EventArgs>.AddHandler(
                    controller, "KeySwitchChanged", OnKeySwitchChanged);
                WeakEventManager<Controller, EventArgs>.AddHandler(
                    controller, "OperationModeChanged", OnOperationModeChanged);
            }

            Index = index;

            CloseCommand = new RelayCommand(ExecuteCloseCommand);
            HelpCommand = new RelayCommand(ExecuteHelpCommand);
            ExecuteCommand = new RelayCommand(ExecuteExecuteCommand, CanExecuteExecuteCommand);
            MotionGroupShutdownCommand =
                new RelayCommand(ExecuteMotionGroupShutdownCommand, CanExecuteMotionGroupShutdownCommand);

            AxisPropertiesCommand = new RelayCommand<ITag>(ExecuteAxisPropertiesCommand);
            MotionGroupPropertiesCommand = new RelayCommand<ITag>(ExecuteMotionGroupPropertiesCommand);

            UpdateAxisSource();
            UpdateMotionGroupSource();

            InitializeOptionPanelNodes();

            // for default select
            var dataType = motion.DataTypeInfo.DataType;
            if (dataType.IsAxisType)
            {
                SelectedAxis = motion;
                OptionPanelNodes[0].Children[0].IsSelected = true;
            }
            else if (dataType.IsMotionGroupType)
            {
                SelectedMotionGroup = motion;
                OptionPanelNodes[2].Children[0].IsSelected = true;
            }

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

        public override void Cleanup()
        {
            _timer?.Stop();
            base.Cleanup();
        }

        private void CycleUpdateTimerHandle(object sender, EventArgs e)
        {
            if (_controller.IsOnline)
            {
                TagSyncController tagSyncController
                    = (_controller as Controller)?.Lookup(typeof(TagSyncController)) as TagSyncController;

                if (tagSyncController != null)
                {
                    if (SelectedAxis != null)
                    {
                        tagSyncController.Update(SelectedAxis, SelectedAxis.Name);
                    }

                    if (SelectedMotionGroup != null)
                    {
                        tagSyncController.Update(SelectedMotionGroup, SelectedMotionGroup.Name);
                    }
                }
            }
            

            // cycle update axis state
            var axis = SelectedAxis as Tag;
            var axisCIPDrive = axis?.DataWrapper as AxisCIPDrive;
            var axisVirtual = axis?.DataWrapper as AxisVirtual;
            var axisDescriptor = axisCIPDrive !=null
                ? new AxisDescriptor(axisCIPDrive.CIPAxis)
                : new AxisDescriptor(axisVirtual?.CIPAxis);
            if (axis != null && axis.ParentController.IsOnline)
            {

                AxisState = axisDescriptor.CIPAxisState;

                // axis fault
                AxisFault = axisDescriptor.AxisFault;

                // start Inhibited
                StartInhibited = axisCIPDrive != null?axisDescriptor.StartInhibited:string.Empty;
            }
            else
            {
                AxisState = string.Empty;
                AxisFault = string.Empty;
                StartInhibited = string.Empty;
            }

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
                new DefaultOptionPanelDescriptor("13", "MASD", "Motion Axis Shutdown",
                    new MASDViewModel(new MASDPanel(), this), null),
                new DefaultOptionPanelDescriptor("14", "MASR", "Motion Axis Shutdown Reset",
                    new MASRViewModel(new MASRPanel(), this), null),
                new DefaultOptionPanelDescriptor("15", "MDO", "Motion Direct Drive On",
                    new MDOViewModel(new MDOPanel(), this), null),
                new DefaultOptionPanelDescriptor("16", "MDF", "Motion Direct Drive Off",
                    new MDFViewModel(new MDFPanel(), this), null),
                new DefaultOptionPanelDescriptor("17", "MDS", "Motion Drive Start",
                    new MDSViewModel(new MDSPanel(), this), null),
                new DefaultOptionPanelDescriptor("18", "MAFR", "Motion Axis Fault Reset",
                    new MAFRViewModel(new MAFRPanel(), this), null)
            };

            var motionMovePanelDescriptors = new List<IOptionPanelDescriptor>
            {
                new DefaultOptionPanelDescriptor("21", "MAS", "Motion Axis Stop",
                    new MASViewModel(new MASPanel(), this), null),
                new DefaultOptionPanelDescriptor("22", "MAH", "Motion Axis Home",
                    new MAHViewModel(new MAHPanel(), this), null),
                new DefaultOptionPanelDescriptor("23", "MAJ", "Motion Axis Jog",
                    new MAJViewModel(new MAJPanel(), this), null),
                new DefaultOptionPanelDescriptor("24", "MAM", "Motion Axis Move",
                    new MAMViewModel(new MAMPanel(), this), null),
                new DefaultOptionPanelDescriptor("25", "MAG", "Motion Axis Gear",
                    new MAGViewModel(new MAGPanel(), this), null),
                new DefaultOptionPanelDescriptor("26", "MCD", "Motion Change Dynamics",
                    new MCDViewModel(new MCDPanel(), this), null),
                new DefaultOptionPanelDescriptor("27", "MRP", "Motion Redefine Position",
                    new MRPViewModel(new MRPPanel(), this), null),
            };

            var motionGroupPanelDescriptors = new List<IOptionPanelDescriptor>
            {
                new DefaultOptionPanelDescriptor("31", "MGS", "Motion Group Stop",
                    new MGSViewModel(new MGSPanel(), this), null),
                new DefaultOptionPanelDescriptor("32", "MGSD", "Motion Group Shutdown",
                    new MGSDViewModel(new MGSDPanel(), this), null),
                new DefaultOptionPanelDescriptor("33", "MGSR", "Motion Group Shutdown Reset",
                    new MGSRViewModel(new MGSRPanel(), this), null),
                new DefaultOptionPanelDescriptor("34", "MGSP", "Motion Group Strobe Position",
                    new MGSPViewModel(new MGSPPanel(), this), null),
            };

            var motionEventPanelDescriptors = new List<IOptionPanelDescriptor>
            {
                new DefaultOptionPanelDescriptor("41", "MAW", "Motion Arm Watch",
                    new MAWViewModel(new MAWPanel(), this), null),
                new DefaultOptionPanelDescriptor("42", "MDW", "Motion Disable Watch",
                    new MDWViewModel(new MDWPanel(), this), null),
                new DefaultOptionPanelDescriptor("43", "MAR", "Motion Arm Registration",
                    new MARViewModel(new MARPanel(), this), null),
                new DefaultOptionPanelDescriptor("44", "MDR", "Motion Disarm Registration",
                    new MDRViewModel(new MDRPanel(), this), null),
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
                new DefaultOptionPanelDescriptor("3", "Motion Group",
                    "Motion Group Direct Command Group",
                    new DefaultViewModel(new DefaultPanel(), this),
                    motionGroupPanelDescriptors),
                new DefaultOptionPanelDescriptor("4", "Motion Event",
                    "Motion Event Direct Command Group",
                    new DefaultViewModel(new DefaultPanel(), this),
                    motionEventPanelDescriptors)

            };
            OptionPanelNodes = _optionPanelDescriptors.Select(op => new OptionPanelNode(op, this)).ToList();


        }

        public Action CloseAction { get; set; }

        public int Index { get; }

        public string Title
        {
            get
            {
                var defaultViewModel = _activeNode?.OptionPanelDescriptor.OptionPanel as DefaultViewModel;
                var selectedTag = defaultViewModel?.SelectedTag;

                return selectedTag != null
                    ? LanguageManager.GetInstance().ConvertSpecifier("Motion Direct Commands...") + $" - {selectedTag.Name}:{Index}"
                    : LanguageManager.GetInstance().ConvertSpecifier("Motion Direct Commands...") + $":{Index}";

            }
        }

        public string OptionPanelTitle
        {
            get { return _optionPanelTitle; }
            set { Set(ref _optionPanelTitle, value); }
        }

        public List<OptionPanelNode> OptionPanelNodes { get; set; }

        public object OptionPanelContent
        {
            get { return _optionPanelContent; }
            set { Set(ref _optionPanelContent, value); }
        }

        public Visibility AxisStateVisibility
        {
            get
            {
                var defaultViewModel = _activeNode?.OptionPanelDescriptor.OptionPanel as DefaultViewModel;
                var selectedTag = defaultViewModel?.SelectedTag;

                if (selectedTag == null)
                    return Visibility.Collapsed;

                if (selectedTag.DataTypeInfo.DataType.IsMotionGroupType)
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        public Visibility StartInhibitedVisibility
        {
            get
            {
                var defaultViewModel = _activeNode?.OptionPanelDescriptor.OptionPanel as DefaultViewModel;
                var selectedTag = defaultViewModel?.SelectedTag;

                var axis = selectedTag as Tag;
                AxisCIPDrive axisCIPDrive = axis?.DataWrapper as AxisCIPDrive;

                if (axisCIPDrive != null)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

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

        public string ExecutionError
        {
            get { return _executionError; }
            set { Set(ref _executionError, value); }
        }

        public RelayCommand CloseCommand { get; }
        public RelayCommand HelpCommand { get; }
        public RelayCommand ExecuteCommand { get; }
        public RelayCommand MotionGroupShutdownCommand { get; }
        public RelayCommand<ITag> AxisPropertiesCommand { get; }
        public RelayCommand<ITag> MotionGroupPropertiesCommand { get; }

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

        public ITag SelectedMotionGroup
        {
            get { return _selectedMotionGroup; }
            set
            {
                _selectedMotionGroup = value;

                RaisePropertyChanged("Title");
                RaisePropertyChanged("AxisStateVisibility");
                RaisePropertyChanged("StartInhibitedVisibility");

                ExecuteCommand.RaiseCanExecuteChanged();
            }
        }


        public ObservableCollection<ITag> AllAxises
        {
            get { return _allAxises; }
            set { Set(ref _allAxises, value); }
        }

        public ObservableCollection<ITag> AllMotionGroups
        {
            get { return _allMotionGroups; }
            set { Set(ref _allMotionGroups, value); }
        }

        private void OnTagCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            bool haveAxis = false;
            bool haveMotionGroup = false;

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                UpdateAxisSource();
                UpdateMotionGroupSource();
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    ITag tag = item as ITag;
                    if (tag != null)
                    {
                        if (tag.DataTypeInfo.DataType.IsAxisType)
                            haveAxis = true;

                        if (tag.DataTypeInfo.DataType.IsMotionGroupType)
                            haveMotionGroup = true;
                    }
                }

                if (haveAxis)
                    UpdateAxisSource();
                if (haveMotionGroup)
                    UpdateMotionGroupSource();
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    ITag tag = item as ITag;
                    if (tag != null)
                    {
                        if (tag.DataTypeInfo.DataType.IsAxisType)
                            haveAxis = true;

                        if (tag.DataTypeInfo.DataType.IsMotionGroupType)
                            haveMotionGroup = true;
                    }
                }

                if (haveAxis)
                    UpdateAxisSource();
                if (haveMotionGroup)
                    UpdateMotionGroupSource();
            }
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

        private void UpdateMotionGroupSource()
        {
            List<ITag> motionGroupList = new List<ITag>();
            foreach (var tag in _controller.Tags)
            {
                if (tag.DataTypeInfo.DataType.IsMotionGroupType)
                    motionGroupList.Add(tag);
            }

            AllMotionGroups = new ObservableCollection<ITag>(motionGroupList);
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
                    MotionGroupShutdownCommand.RaiseCanExecuteChanged();
                });
        }

    }
}