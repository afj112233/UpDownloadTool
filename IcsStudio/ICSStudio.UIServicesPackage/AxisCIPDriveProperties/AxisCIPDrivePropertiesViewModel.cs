using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel;
using NLog;
using ThreadHelper = Microsoft.VisualStudio.Shell.ThreadHelper;
using ICSStudio.MultiLanguage;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public sealed partial class AxisCIPDrivePropertiesViewModel
        : ViewModelBase, ICanApply
    {
        internal static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private List<IOptionPanelDescriptor> _flatDescriptors;
        private OptionPanelNode _activeNode;
        private List<string> _comparePropertiesList;
        private readonly List<string> _editPropertiesList;

        private string _title;
        private object _optionPanelContent;

        private bool _beNeedCalculateOutOfBoxTuning;
        private bool _beNeedCalculateFeedbackUnitRatio;
        private bool _beNeedCalculateScalingFactor;
        private bool _beNeedCalculateFrequencyControl;

        private bool _beTagNameChanged;
        private bool _beTagDescriptionChanged;
        private bool _beAssignedGroupChanged;
        private bool _beAssociatedAxisNumberChanged;
        private bool _beCyclicReadUpdateListChanged;
        private bool _beCyclicWriteUpdateListChanged;

        private readonly Timer _axisUpdateTimer;
        private CIPAxisStateType _oldAxisState;

        public AxisCIPDrivePropertiesViewModel(ITag axisTag)
        {
            Contract.Assert(axisTag != null);

            _editPropertiesList = new List<string>();

            AxisTag = axisTag;
            Controller = axisTag.ParentController;
            Title = LanguageManager.GetInstance().ConvertSpecifier("Axis Properties -") + " " + AxisTag.Name;

            var tag = axisTag as Tag;
            var axisCIPDrive = tag?.DataWrapper as AxisCIPDrive;
            Contract.Assert(axisCIPDrive != null);

            OriginalAxisCIPDrive = axisCIPDrive;
            AxisDefaultSetting.UpdateMotionUnit(OriginalAxisCIPDrive.CIPAxis);

            ModifiedAxisCIPDrive = (AxisCIPDrive)axisCIPDrive.Clone();
            ModifiedTagName = AxisTag.Name;
            ModifiedDescription = AxisTag.Description;

            _oldAxisState = (CIPAxisStateType)Convert.ToByte(OriginalAxisCIPDrive.CIPAxis.CIPAxisState);

            // Command
            ManualTuneCommand = new RelayCommand(ExecuteManualTuneCommand, CanExecuteManualTuneCommand);
            OkCommand = new RelayCommand(ExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            ApplyCommand = new RelayCommand(ExecuteApplyCommand, CanExecuteApplyCommand);
            HelpCommand = new RelayCommand(ExecuteHelpCommand);

            ClosingCommand = new RelayCommand<CancelEventArgs>(ExecuteClosingCommand);

            // Event
            if (AxisTag != null)
            {
                PropertyChangedEventManager.AddHandler(
                    AxisTag, OnAxisTagPropertyChanged, string.Empty);
            }

            if (OriginalAxisCIPDrive.AssignedGroup != null)
            {
                PropertyChangedEventManager.RemoveHandler(
                    OriginalAxisCIPDrive.AssignedGroup, OnAssignedGroupPropertyChanged, string.Empty);
                PropertyChangedEventManager.AddHandler(
                    OriginalAxisCIPDrive.AssignedGroup, OnAssignedGroupPropertyChanged, string.Empty);
            }

            Controller controller = Controller as Controller;
            if (controller != null)
            {
                CollectionChangedEventManager.AddHandler(
                    controller.DeviceModules, OnDeviceModulesChanged);

                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
                WeakEventManager<Controller, EventArgs>.AddHandler(
                    controller, "KeySwitchChanged", OnKeySwitchChanged);
                WeakEventManager<Controller, EventArgs>.AddHandler(
                    controller, "OperationModeChanged", OnOperationModeChanged);
            }

            //TODO(gjc): need check here
            AutoApplyPropertyChanged += async delegate(object sender, PropertyChangedEventArgs args)
            {
                await OnAutoApplyPropertyChangedAsync(sender, args);
            };

            //
            InitializeOptionPanelNodes();

            SelectNodeByTitle("General");

            AutoApplyProperties = new[]
            {
                "TuningSelect", "TuningTravelLimit",
                "TuningSpeed", "TuningTorque", "TuningDirection",
                "LoadRatio", "TotalInertia", "SystemInertia",
                "MaximumAcceleration", "MaximumDeceleration",
                "MaximumAccelerationJerk", "MaximumDecelerationJerk",
                "TorqueLowPassFilterBandwidth",
                "PositionLoopBandwidth", "PositionErrorTolerance",
                "VelocityLoopBandwidth", "VelocityErrorTolerance"
                //TODO(gjc): add more
            };

            _axisUpdateTimer = new Timer(100);
            _axisUpdateTimer.Elapsed += CycleUpdateAxis;
            if (IsOnLine)
                _axisUpdateTimer.Start();

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.Instance, 
                "LanguageChanged",
                OnLanguageChanged);
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            Title = LanguageManager.GetInstance().ConvertSpecifier("Axis Properties -") + " " + AxisTag.Name;

            foreach (var node in OptionPanelNodes)
            {
                RaiseNodePropertyChanged(node,"Title");
            }

            RaisePropertyChanged(nameof(OptionPanelTitle));
        }

        private void RaiseNodePropertyChanged(OptionPanelNode node, string propertyName)
        {
            if (node != null)
            {
                node.RaisePropertyChanged(propertyName);

                if (node.Children != null)
                {
                    foreach (var child in node.Children)
                    {
                        child.RaisePropertyChanged(propertyName);
                    }
                }
            }
        }

        public ITag AxisTag { get; }
        public IController Controller { get; }

        public AxisCIPDrive OriginalAxisCIPDrive { get; }
        public AxisCIPDrive ModifiedAxisCIPDrive { get; }

        public string ModifiedTagName { get; set; }
        public string ModifiedDescription { get; set; }

        public string[] AutoApplyProperties { get; }

        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }

        public string OptionPanelTitle => _activeNode != null ? _activeNode.Label : string.Empty;

        public List<OptionPanelNode> OptionPanelNodes { get; private set; }

        public object OptionPanelContent
        {
            get { return _optionPanelContent; }
            set { Set(ref _optionPanelContent, value); }
        }

        public string AxisState
        {
            get
            {
                if (IsOnLine)
                {
                    var axisDescriptor = new AxisDescriptor(OriginalAxisCIPDrive.CIPAxis);
                    return axisDescriptor.CIPAxisState;
                }

                return string.Empty;
            }
        }

        public Action CloseAction { get; set; }
        public RelayCommand ManualTuneCommand { get; }
        public RelayCommand OkCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand ApplyCommand { get; }
        public RelayCommand HelpCommand { get; }

        public RelayCommand<CancelEventArgs> ClosingCommand { get; }

        public override void Cleanup()
        {
            if (_axisUpdateTimer != null)
            {
                _axisUpdateTimer.Stop();
                _axisUpdateTimer.Elapsed -= CycleUpdateAxis;
            }

            foreach (var descriptor in _flatDescriptors)
            {
                var viewModel = descriptor.OptionPanel as DefaultViewModel;
                if (viewModel != null)
                {
                    if (viewModel.Visibility == Visibility.Visible)
                    {
                        viewModel.Cleanup();
                    }
                }
            }


            // ReSharper disable once UsePatternMatching
            Controller controller = Controller as Controller;
            if (controller != null)
            {
                CollectionChangedEventManager.RemoveHandler(
                    controller.DeviceModules, OnDeviceModulesChanged);

                WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }

            base.Cleanup();
        }

        public void Refresh()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                foreach (var descriptor in _flatDescriptors)
                {
                    var viewModel = descriptor.OptionPanel as DefaultViewModel;
                    if (viewModel != null)
                    {
                        if (viewModel.Visibility == Visibility.Visible)
                        {
                            viewModel.Show();
                        }
                    }
                }


                if (OptionPanelNodes != null)
                {
                    foreach (var optionPanelNode in OptionPanelNodes)
                    {
                        RefreshOptionPanelNode(optionPanelNode);
                    }
                }

                RaisePropertyChanged("AxisState");
                ManualTuneCommand.RaiseCanExecuteChanged();
                ApplyCommand.RaiseCanExecuteChanged();
            });
        }

        public void ShowPanel(string title, string parameterGroup = null)
        {
            if (!title.Equals("Parameter List"))
                SelectNodeByTitle(title);
            else
            {
                var parameterListNode = FindFirstNodeByTitle(title, OptionPanelNodes);
                var viewModel = parameterListNode.OptionPanel as ParameterListViewModel;
                if (viewModel != null)
                    viewModel.ParameterGroup = parameterGroup;

                parameterListNode.IsSelected = true;
            }
        }


        #region auto apply

        private event PropertyChangedEventHandler AutoApplyPropertyChanged;

        public void RaiseAutoApplyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler propertyChanged = this.AutoApplyPropertyChanged;
            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // ReSharper disable once UnusedParameter.Local
        private async Task OnAutoApplyPropertyChangedAsync(object sender, PropertyChangedEventArgs args)
        {
            //TODO(gjc): need edit here
            List<ushort> differentAttributeList =
                CipAttributeHelper.AttributeNamesToIdList<CIPAxis>(new[] { args.PropertyName });

            if (IsOnLine)
            {
                try
                {
                    Controller controller = Controller as Controller;
                    Contract.Assert(controller != null);

                    int instanceId = controller.GetTagId(AxisTag);
                    ModifiedAxisCIPDrive.CIPAxis.InstanceId = instanceId;
                    ModifiedAxisCIPDrive.CIPAxis.Messager = controller.CipMessager;

                    await ModifiedAxisCIPDrive.CIPAxis.SetAttributeList(differentAttributeList);
                }
                catch (Exception)
                {
                    // ignore
                }
            }

            OriginalAxisCIPDrive.CIPAxis.Apply(ModifiedAxisCIPDrive.CIPAxis, differentAttributeList);

            var propertyNames =
                CipAttributeHelper.AttributeIdsToNames(
                    OriginalAxisCIPDrive.CIPAxis.GetType(),
                    differentAttributeList);

            OriginalAxisCIPDrive.NotifyParentPropertyChanged(propertyNames.ToArray());
        }

        #endregion

        public bool IsOnLine => Controller.IsOnline;

        public bool IsPowerStructureEnabled
        {
            get
            {
                if (IsOnLine)
                {
                    //TODO(gjc): edit here later

                    //var axisState =
                    //    (CIPAxisStateType)Convert.ToByte(OriginalAxisCIPDrive.CIPAxis.CIPAxisState);
                    //if (axisState == CIPAxisStateType.Running)
                    //    return true;


                    var cipAxisStatus =
                        (CIPAxisStatusBitmap)Convert.ToUInt32(OriginalAxisCIPDrive.CIPAxis.CIPAxisStatus);
                    if ((cipAxisStatus & CIPAxisStatusBitmap.PowerStructureEnabled) != 0)
                        return true;
                }

                return false;
            }
        }

        public bool IsHardRunMode =>
            Controller.IsOnline && Controller.KeySwitchPosition == ControllerKeySwitch.RunKeySwitch;

        #region Private

        private void InitializeOptionPanelNodes()
        {
            _flatDescriptors = new List<IOptionPanelDescriptor>();

            // motor child
            var motorChildOptionPanelDescriptors = new List<IOptionPanelDescriptor>
            {
                // Model
                new DefaultOptionPanelDescriptor("101", "Model", "Motor model Phase to Phase Parameters",
                    "CIPTag Properties Model",
                    new ModelViewModel(new ModelPanel(), this), null),
                // Analyzer
                new DefaultOptionPanelDescriptor("102", "Analyzer", "Analyze Motor to Determine Motor Model",
                    new AnalyzerViewModel(new AnalyzerPanel(), this), null)
            };

            // load child
            var loadChildOptionPanelDescriptors = new List<IOptionPanelDescriptor>
            {
                // Backlash
                new DefaultOptionPanelDescriptor("201", "Backlash", "Backlash Compensation",
                    new BacklashViewModel(new BacklashPanel(), this), null),
                // Compliance
                new DefaultOptionPanelDescriptor("202", "Compliance", "Compliance Compensation",
                    new ComplianceViewModel(new CompliancePanel(), this), null),
                // Friction
                new DefaultOptionPanelDescriptor("203", "Friction", "Friction Compensation",
                    new FrictionViewModel(new FrictionPanel(), this), null),
                // Observer
                new DefaultOptionPanelDescriptor("204", "Observer", "Load Observer",
                    new ObserverViewModel(new ObserverPanel(), this), null)
            };

            var optionPanelDescriptors = new List<IOptionPanelDescriptor>
            {
                // General
                new DefaultOptionPanelDescriptor("1", "General", "General",
                    new GeneralViewModel(new GeneralPanel(), this), null),
                // Motor
                new DefaultOptionPanelDescriptor("2", "Motor", "Motor Device Specification",
                    new MotorViewModel(new MotorPanel(), this), motorChildOptionPanelDescriptors),
                // Master Feedback
                new DefaultOptionPanelDescriptor("3", "Master Feedback", "Master Feedback Device Specification",
                    new MasterFeedbackViewModel(new MasterFeedbackPanel(), this), null),
                // Motor Feedback
                new DefaultOptionPanelDescriptor("4", "Motor Feedback", "Motor Feedback Device Specification",
                    new MotorFeedbackViewModel(new MotorFeedbackPanel(), this), null),
                // Load Feedback
                new DefaultOptionPanelDescriptor("5", "Load Feedback", "Load Feedback Device Specification",
                    new LoadFeedbackViewModel(new LoadFeedbackPanel(), this), null),
                // Scaling
                new DefaultOptionPanelDescriptor("6", "Scaling",
                    "Scaling to Convert Motion from Controller Units to User Defined Units",
                    "AxisPropertiesScaling",
                    new ScalingViewModel(new ScalingPanel(), this), null),
                // Hookup Tests
                new DefaultOptionPanelDescriptor("7", "Hookup Tests", "Test Motor and Feedback Device Wiring",
                    new HookupTestsViewModel(new HookupTestsPanel(), this), null),
                // Polarity
                new DefaultOptionPanelDescriptor("8", "Polarity", "Motion. Motor. and Feedback Polarity",
                    new PolarityViewModel(new PolarityPanel(), this), null),
                // Autotune
                new DefaultOptionPanelDescriptor("9", "Autotune", "Tune Control Loop by Measuring Load Characteristics",
                    new AutotuneViewModel(new AutotunePanel(), this), null),
                // Load
                new DefaultOptionPanelDescriptor("10", "Load", "Characteristics of Motor Load",
                    new LoadViewModel(new LoadPanel(), this), loadChildOptionPanelDescriptors),
                // Position Loop
                new DefaultOptionPanelDescriptor("11", "Position Loop", "Position Loop",
                    new PositionLoopViewModel(new PositionLoopPanel(), this), null),
                // Velocity Loop
                new DefaultOptionPanelDescriptor("12", "Velocity Loop", "Velocity Loop",
                    new VelocityLoopViewModel(new VelocityLoopPanel(), this), null),
                // Acceleration Loop
                new DefaultOptionPanelDescriptor("13", "Acceleration", "Acceleration",
                    new AccelerationLoopViewModel(new AccelerationLoopPanel(), this), null),
                // Torque/Current Loop
                new DefaultOptionPanelDescriptor("14", "Torque/Current Loop", "Torque/Current Loop",
                    new TorqueCurrentLoopViewModel(new TorqueCurrentLoopPanel(), this), null),
                // Planner
                new DefaultOptionPanelDescriptor("15", "Planner", "Characteristics of Motion Planner",
                    new PlannerViewModel(new PlannerPanel(), this), null),
                // Homing
                new DefaultOptionPanelDescriptor("16", "Homing", "", "CIPTag Properties Homing",
                    new HomingViewModel(new HomingPanel(), this), null),
                // Frequency Control
                new DefaultOptionPanelDescriptor("17", "Frequency Control", "Frequency Control",
                    new FrequencyControlViewModel(new FrequencyControlPanel(), this), null),
                // Actions
                new DefaultOptionPanelDescriptor("18", "Actions", "Actions to Take Upon Conditions",
                    new ActionsViewModel(new ActionsPanel(), this), null),
                // Exceptions
                new DefaultOptionPanelDescriptor("19", "Exceptions", "Actions to Take Upon Exception Condition",
                    new ExceptionsViewModel(new ExceptionsPanel(), this), null),
                // Cyclic Parameters
                new DefaultOptionPanelDescriptor("20", "Monitor Parameters", "Cyclic Read/Write Parameter List",
                    new CyclicParametersViewModel(new CyclicParametersPanel(), this), null),
                // Parameter List
                new DefaultOptionPanelDescriptor("21", "Parameter List", "Motion Axis Parameters",
                    new ParameterListViewModel(new ParameterListPanel(), this), null),
                // Status
                new DefaultOptionPanelDescriptor("22", "Status", "Motion Status",
                    new StatusViewModel(new StatusPanel(), this), null),
                // Faults & Alarms
                new DefaultOptionPanelDescriptor("23", "Faults Alarms", "Faults and Alarms Log",
                    new FaultsAlarmsViewModel(new FaultsAlarmsPanel(), this), null),
                // Tag
                new DefaultOptionPanelDescriptor("24", "AxisTag", "Axis Tag Properties",
                    new TagViewModel(new TagPanel(), this), null)
            };

            OptionPanelNodes = optionPanelDescriptors.Select(op => new OptionPanelNode(op, this)).ToList();

            foreach (var node in OptionPanelNodes)
            {
                node.PropertyChanged += OnTitlePropertyChanged;
            }


            _flatDescriptors.AddRange(motorChildOptionPanelDescriptors);
            _flatDescriptors.AddRange(loadChildOptionPanelDescriptors);
            _flatDescriptors.AddRange(optionPanelDescriptors);

            _comparePropertiesList = new List<string>();
            foreach (var descriptor in _flatDescriptors)
            {
                var viewModel = descriptor.OptionPanel as DefaultViewModel;
                if (viewModel != null)
                {
                    if (viewModel.CompareProperties != null && viewModel.CompareProperties.Length > 0)
                    {
                        _comparePropertiesList.AddRange(viewModel.CompareProperties);
                    }
                }
            }
        }

        private void RefreshOptionPanelNode(OptionPanelNode optionPanelNode)
        {
            // ReSharper disable ExplicitCallerInfoArgument
            optionPanelNode.RaisePropertyChanged("Title");
            optionPanelNode.RaisePropertyChanged("Visibility");
            // ReSharper restore ExplicitCallerInfoArgument

            if (optionPanelNode.Children != null)
            {
                foreach (var childrenNode in optionPanelNode.Children)
                {
                    RefreshOptionPanelNode(childrenNode);
                }
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

            // special for model
            if (node.Title == "Model")
            {
                string modelLabel = "Motor model Phase to Phase Parameters";
                var motorType = (MotorType)Convert.ToByte(ModifiedAxisCIPDrive.CIPAxis.MotorType);
                if (motorType == MotorType.RotaryInduction)
                {
                    modelLabel = "Motor model Phase to Neutral Parameters";
                }

                var descriptor = node.OptionPanelDescriptor as DefaultOptionPanelDescriptor;
                if (descriptor != null)
                {
                    descriptor.Label = modelLabel;
                }
            }
            //end for model

            _activeNode = node;

            OptionPanelContent = node.Content;

            node.IsExpanded = true;
            node.IsActive = true;

            RaisePropertyChanged(nameof(OptionPanelTitle));
            ApplyCommand.RaiseCanExecuteChanged();
        }

        private void SelectNodeByTitle(string panelTitle)
        {
            var result = FindFirstNodeByTitle(panelTitle, OptionPanelNodes);
            if (result != null)
                result.IsSelected = true;
        }

        public OptionPanelNode FindFirstNodeByTitle(
            string panelTitle,
            List<OptionPanelNode> optionPanelNodeList)
        {
            foreach (var optionPanelNode in optionPanelNodeList)
            {
                if (optionPanelNode.OptionPanelDescriptor.Title.Equals(panelTitle))
                    return optionPanelNode;

                if (optionPanelNode.Children != null)
                {
                    var result = FindFirstNodeByTitle(panelTitle, optionPanelNode.Children);
                    if (result != null)
                        return result;
                }
            }

            return null;
        }

        #endregion

        public sealed class OptionPanelNode : ViewModelBase
        {
            public readonly IOptionPanelDescriptor OptionPanelDescriptor;
            public readonly OptionPanelNode Parent;
            public readonly AxisCIPDrivePropertiesViewModel ViewModel;

            private IAxisCIPDrivePanel _optionPanel;
            private List<OptionPanelNode> _children;
            private bool _isActive;
            private bool _isExpanded;
            private bool _isSelected;

            public OptionPanelNode(IOptionPanelDescriptor optionPanel, AxisCIPDrivePropertiesViewModel viewModel)
            {
                OptionPanelDescriptor = optionPanel;
                ViewModel = viewModel;

                _isExpanded = true;

                OptionPanel.IsDirtyChanged += OptionPanelOnIsDirtyChanged;
            }

            private OptionPanelNode(IOptionPanelDescriptor optionPanel, OptionPanelNode parent)
            {
                OptionPanelDescriptor = optionPanel;
                Parent = parent;
                ViewModel = parent.ViewModel;

                OptionPanel.IsDirtyChanged += OptionPanelOnIsDirtyChanged;
            }

            public string ID => OptionPanelDescriptor.ID;

            public string Title
            {
                get
                {
                    OptionPanel.CheckDirty();

                    if (OptionPanel.IsDirty)
                        return "*" + OptionPanelDescriptor.DisplayTitle;

                    return OptionPanelDescriptor.DisplayTitle;
                }
            }

            public string Label => OptionPanelDescriptor.DisplayLabel;

            internal IAxisCIPDrivePanel OptionPanel
            {
                get
                {
                    if (_optionPanel == null)
                    {
                        _optionPanel = OptionPanelDescriptor.OptionPanel as IAxisCIPDrivePanel;
                        if (_optionPanel == null)
                            return null;

                        _optionPanel.LoadOptions();
                    }

                    return _optionPanel;
                }
            }

            public object Content => OptionPanel.Control;

            public List<OptionPanelNode> Children
            {
                get
                {
                    return _children ?? (_children = OptionPanelDescriptor.ChildOptionPanelDescriptors
                        .Select(op => new OptionPanelNode(op, this)).ToList());
                }
            }

            public bool IsActive
            {
                get { return _isActive; }
                set { Set(ref _isActive, value); }
            }

            public bool IsExpanded
            {
                get { return _isExpanded; }
                set { Set(ref _isExpanded, value); }
            }

            public bool IsSelected
            {
                get { return _isSelected; }
                set
                {
                    Set(ref _isSelected, value);

                    if (_isSelected)
                    {
                        ViewModel.SelectNode(this);
                        _optionPanel.Show();
                    }
                }
            }

            public Visibility Visibility => OptionPanel.Visibility;

            public override string ToString()
            {
                return Title;
            }

            private void OptionPanelOnIsDirtyChanged(object sender, EventArgs e)
            {
                // ReSharper disable ExplicitCallerInfoArgument
                RaisePropertyChanged("Title");
                // ReSharper restore ExplicitCallerInfoArgument
            }
        }

        public int Apply()
        {
            int result = ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                int applyResult = await DoApplyAsync();

                return applyResult;
            });

            return result;
        }

        public bool CanApply()
        {
            return true;
        }

        internal void AddEditPropertyName(string propertyName)
        {
            if (!_editPropertiesList.Contains(propertyName))
                _editPropertiesList.Add(propertyName);
        }

        internal bool HasEditPropertyName(string propertyName)
        {
            return _editPropertiesList.Contains(propertyName);
        }
    }
}