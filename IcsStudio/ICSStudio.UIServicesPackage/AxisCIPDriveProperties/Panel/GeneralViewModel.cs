using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.Objects;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Dialog;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class GeneralViewModel : DefaultViewModel
    {
        private IList _axisConfigurationSource;
        private IList _feedbackConfigurationSource;
        private IList _motionGroupSource;
        private IList _motionDriveSource;
        private IList _axisNumberSource;

        public GeneralViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel) :
            base(panel, parentViewModel)
        {
            CompareProperties = new[]
            {
                "AxisConfiguration",
                "FeedbackConfiguration",
                "ApplicationType", "LoopResponse",
                "DriveModelTimeConstant",
                "DampingFactor", "SystemDamping",
                "ControlMethod", "ControlMode", "FeedbackMode",
                "SystemBandwidth",
                "RegistrationInputs"
            };

            MotionGroupPropertiesCommand =
                new RelayCommand(ExecuteMotionGroupPropertiesCommand, CanMotionGroupPropertiesCommand);
            NewMotionGroupCommand =
                new RelayCommand(ExecuteNewMotionGroupCommand, CanNewMotionGroupCommand);
            AxisScheduleCommand =
                new RelayCommand(ExecuteAxisScheduleCommand, CanAxisScheduleCommand);

            UpdateAxisConfigurationSource();
            UpdateFeedbackConfigurationSource();
            ApplicationTypeSource = EnumHelper.ToDataSource<ApplicationType>();
            LoopResponseSource = EnumHelper.ToDataSource<LoopResponseType>();
            UpdateMotionGroupSource();
            UpdateMotionDriveSource();
            UpdateAxisNumberSource();
            CollectionChangedEventManager.AddHandler(ParentViewModel.Controller.DeviceModules, DeviceModules_CollectionChanged);
        }

        public override void Cleanup()
        {
            CollectionChangedEventManager.RemoveHandler(ParentViewModel.Controller.DeviceModules,
                DeviceModules_CollectionChanged);
            if (MotionDriveSource != null)
                foreach (var device in MotionDriveSource)
                {
                    var item = device?.GetType().GetProperty("Value")?.GetValue(device) as IDeviceModule;
                    if (item != null)
                        PropertyChangedEventManager.RemoveHandler(item, Item_PropertyChanged, "Name");
                }

        }

        public override void Show()
        {
            UpdateAxisConfigurationSource();
            UpdateFeedbackConfigurationSource();

            UpdateMotionGroupSource();
            UpdateMotionDriveSource();
            UpdateAxisNumberSource();

            UIVisibilityAndReadonly();
            UIRefresh();
        }

        public override int CheckValid()
        {
            if (ParentViewModel.OriginalAxisCIPDrive.AssociatedModule !=
                ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule
                || ParentViewModel.OriginalAxisCIPDrive.AxisNumber
                != ParentViewModel.ModifiedAxisCIPDrive.AxisNumber)
            {
                var cipMotionDrive = ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule as CIPMotionDrive;
                if (cipMotionDrive?.GetAxis(ParentViewModel.ModifiedAxisCIPDrive.AxisNumber) != null)
                {
                    // show page
                    ParentViewModel.ShowPanel("General");

                    // show warning
                    string message = $"Failed to modify properties for axis '{ParentViewModel.AxisTag.Name}'";
                    var warningView =
                        new WarningDialog(
                                message,
                                "Invalid channel/node for motion module.",
                                "Error 16427-80042F0A")
                            {Owner = Application.Current.MainWindow};
                    warningView.ShowDialog();

                    return -1;
                }
            }

            return 0;
        }

        public IList AxisConfigurationSource
        {
            get { return _axisConfigurationSource; }
            set { Set(ref _axisConfigurationSource, value); }
        }

        public AxisConfigurationType AxisConfiguration
        {
            get
            {
                return
                    (AxisConfigurationType) Convert.ToByte(ModifiedCIPAxis.AxisConfiguration);
            }
            set
            {
                if (Convert.ToByte(ModifiedCIPAxis.AxisConfiguration) != (byte) value)
                {
                    ChangeAxisConfiguration(value);
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public IList FeedbackConfigurationSource
        {
            get { return _feedbackConfigurationSource; }
            set { Set(ref _feedbackConfigurationSource, value); }
        }

        public FeedbackConfigurationType FeedbackConfiguration
        {
            get
            {
                return
                    (FeedbackConfigurationType) Convert.ToByte(ModifiedCIPAxis.FeedbackConfiguration);
            }
            set
            {
                if (Convert.ToByte(ModifiedCIPAxis.FeedbackConfiguration) != (byte) value)
                {
                    ChangeFeedbackConfiguration(value);
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public Visibility MoreConfigVisibility
        {
            get
            {
                switch (AxisConfiguration)
                {
                    case AxisConfigurationType.FeedbackOnly:
                        return Visibility.Hidden;
                    case AxisConfigurationType.FrequencyControl:
                        return Visibility.Hidden;
                    case AxisConfigurationType.PositionLoop:
                        return Visibility.Visible;
                    case AxisConfigurationType.VelocityLoop:
                        return Visibility.Visible;
                    case AxisConfigurationType.TorqueLoop:
                        return Visibility.Hidden;
                    case AxisConfigurationType.ConverterOnly:
                        return Visibility.Hidden;
                    default:
                        return Visibility.Hidden;
                }
            }
        }

        public IList ApplicationTypeSource { get; }

        public ApplicationType ApplicationType
        {
            get { return (ApplicationType) Convert.ToByte(ModifiedCIPAxis.ApplicationType); }
            set
            {
                if (Convert.ToByte(ModifiedCIPAxis.ApplicationType) != (byte) value)
                {
                    ChangeApplicationType(value);

                    ParentViewModel.AddEditPropertyName(nameof(ApplicationType));
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public IList LoopResponseSource { get; }

        public LoopResponseType LoopResponse
        {
            get { return (LoopResponseType) Convert.ToByte(ModifiedCIPAxis.LoopResponse); }
            set
            {
                if (Convert.ToByte(ModifiedCIPAxis.LoopResponse) != (byte) value)
                {
                    ChangeLoopResponse(value);
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public IList MotionGroupSource
        {
            get { return _motionGroupSource; }
            set { Set(ref _motionGroupSource, value); }
        }

        public ITag MotionGroup
        {
            get { return ParentViewModel.ModifiedAxisCIPDrive.AssignedGroup; }
            set
            {
                ParentViewModel.ModifiedAxisCIPDrive.AssignedGroup = value;

                MotionGroupPropertiesCommand.RaiseCanExecuteChanged();
                AxisScheduleCommand.RaiseCanExecuteChanged();

                if (value == null)
                    ModifiedCIPAxis.AxisUpdateSchedule = (byte) AxisUpdateScheduleType.Base;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public string UpdatePeriod
        {
            get
            {
                if (ParentViewModel.OriginalAxisCIPDrive.AssignedGroup == null)
                    return string.Empty;

                MotionGroup motionGroup =
                    ((Tag) ParentViewModel.OriginalAxisCIPDrive.AssignedGroup).DataWrapper as MotionGroup;
                Contract.Assert(motionGroup != null);

                return motionGroup.GetUpdatePeriod(
                           (AxisUpdateScheduleType) Convert.ToByte(OriginalCIPAxis.AxisUpdateSchedule)).ToString("F1") +
                       " ms";
            }
        }

        public IList MotionDriveSource
        {
            get { return _motionDriveSource; }
            set { Set(ref _motionDriveSource, value); }
        }

        public IDeviceModule MotionDrive
        {
            get { return ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule; }
            set
            {
                if (ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule != value)
                {
                    if (value != null)
                    {
                        if (ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule != value)
                        {
                            List<int> axisNumbers =
                                ((CIPMotionDrive) value).CandidateAxisNumbers(ParentViewModel.AxisTag);

                            if (axisNumbers.Count > 0)
                            {
                                ParentViewModel.ModifiedAxisCIPDrive.UpdateAxisChannel(value, axisNumbers[0]);
                            }
                            else
                            {
                                UpdateMotionDriveSource();
                            }
                        }

                    }
                    else
                    {
                        ParentViewModel.ModifiedAxisCIPDrive.UpdateAxisChannel(null, 0);
                    }

                    // Update
                    UpdateAxisConfigurationSource();
                    UpdateFeedbackConfigurationSource();
                    UpdateAxisNumberSource();
                    
                    RaisePropertyChanged("ModuleType");
                    RaisePropertyChanged("PowerStructure");
                    RaisePropertyChanged("AxisNumberEnabled");
                    RaisePropertyChanged("AxisNumber");

                    CheckDirty();
                    RaisePropertyChanged();

                    ParentViewModel.Refresh();
                }

            }
        }

        public string ModuleType => MotionDrive != null ? MotionDrive.CatalogNumber : "<none>";

        public string PowerStructure
        {
            get
            {
                if (MotionDrive != null)
                    return ((CIPMotionDrive) MotionDrive).PowerStructure;

                return "<none>";
            }

        }

        public IList AxisNumberSource
        {
            get { return _axisNumberSource; }
            set { Set(ref _axisNumberSource, value); }
        }

        public int AxisNumber
        {
            get { return ParentViewModel.ModifiedAxisCIPDrive.AxisNumber; }
            set
            {
                if (ParentViewModel.ModifiedAxisCIPDrive.AxisNumber != value)
                {
                    ParentViewModel.ModifiedAxisCIPDrive.UpdateAxisChannel(
                        ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule, value);

                    CheckDirty();
                    RaisePropertyChanged();

                    ParentViewModel.Refresh();
                }
            }
        }

        public bool AxisNumberEnabled
        {
            get
            {
                if (ParentViewModel.IsOnLine)
                    return false;

                if (MotionDrive == null)
                    return false;

                return true;
            }
        }

        public bool AxisConfigurationEnabled => !ParentViewModel.IsOnLine;
        public bool FeedbackConfigurationEnabled => !ParentViewModel.IsOnLine;

        public bool ApplicationTypeEnabled
        {
            get
            {
                if (ParentViewModel.IsPowerStructureEnabled)
                    return false;

                if (ParentViewModel.IsHardRunMode)
                    return false;

                return true;
            }
        }

        public bool LoopResponseEnabled
        {
            get
            {
                if (ParentViewModel.IsPowerStructureEnabled)
                    return false;

                if (ParentViewModel.IsHardRunMode)
                    return false;

                return true;
            }
        }

        public bool MotionGroupEnabled => !ParentViewModel.IsOnLine;
        public bool MotionDriveEnabled => !ParentViewModel.IsOnLine;


        public RelayCommand MotionGroupPropertiesCommand { get; }
        public RelayCommand AxisScheduleCommand { get; }
        public RelayCommand NewMotionGroupCommand { get; }

        #region Command

        private bool CanAxisScheduleCommand()
        {
            return MotionGroup != null;
        }

        private void ExecuteAxisScheduleCommand()
        {
            ICreateDialogService createDialogService =
                Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;

            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = (IVsUIShell) Package.GetGlobalService(typeof(SVsUIShell));

            if (createDialogService != null)
            {
                var window = createDialogService.CreateAxisScheduleDialog(MotionGroup);
                window.Show(uiShell);
            }
        }

        private bool CanNewMotionGroupCommand()
        {
            return !ParentViewModel.Controller.Tags.Any(tag => tag.DataTypeInfo.DataType.IsMotionGroupType);
        }

        private void ExecuteNewMotionGroupCommand()
        {
            var createDialogService =
                (ICreateDialogService) Package.GetGlobalService(typeof(SCreateDialogService));

            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = (IVsUIShell) Package.GetGlobalService(typeof(SVsUIShell));

            if (createDialogService != null)
            {
                var dialog =
                    createDialogService.CreateNewTagDialog(
                        MOTION_GROUP.Inst, 
                        ParentViewModel.Controller.Tags);
                dialog.ShowDialog(uiShell);
            }
        }

        private bool CanMotionGroupPropertiesCommand()
        {
            return MotionGroup != null;
        }

        private void ExecuteMotionGroupPropertiesCommand()
        {
            var createDialogService =
                (ICreateDialogService) Package.GetGlobalService(typeof(SCreateDialogService));

            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = (IVsUIShell) Package.GetGlobalService(typeof(SVsUIShell));

            var window =
                createDialogService?.CreateMotionGroupProperties(MotionGroup);
            window?.Show(uiShell);
        }

        #endregion

        #region Private Method

        [SuppressMessage("ReSharper", "InlineOutVariableDeclaration")]
        private void ChangeAxisConfiguration(AxisConfigurationType value)
        {
            ModifiedCIPAxis.AxisConfiguration = (byte) value;

            byte controlMode, controlMethod;
            UpdateControlMethodAndMode(value,out controlMode,out controlMethod);

            ModifiedCIPAxis.ControlMode = controlMode;
            ModifiedCIPAxis.ControlMethod = controlMethod;

            UpdateFeedbackConfigurationSource();

            RaisePropertyChanged("MoreConfigVisibility");

            ParentViewModel.Refresh();
        }

        private void UpdateControlMethodAndMode(AxisConfigurationType axisConfiguration,
            out byte controlMode, out byte controlMethod)
        {
            // rm003, page69
            switch (axisConfiguration)
            {
                case AxisConfigurationType.FeedbackOnly:
                    controlMode = (byte)ControlModeType.NoControl;
                    controlMethod = (byte)ControlMethodType.NoControl;
                    break;
                case AxisConfigurationType.FrequencyControl:
                    controlMode = (byte)ControlModeType.VelocityControl;
                    controlMethod = (byte)ControlMethodType.FrequencyControl;
                    break;
                case AxisConfigurationType.PositionLoop:
                    controlMode = (byte)ControlModeType.PositionControl;
                    controlMethod = (byte)ControlMethodType.PIVectorControl;
                    break;
                case AxisConfigurationType.VelocityLoop:
                    controlMode = (byte)ControlModeType.VelocityControl;
                    controlMethod = (byte)ControlMethodType.PIVectorControl;
                    break;
                case AxisConfigurationType.TorqueLoop:
                    controlMode = (byte)ControlModeType.TorqueControl;
                    controlMethod = (byte)ControlMethodType.PIVectorControl;
                    break;
                case AxisConfigurationType.ConverterOnly:
                    controlMode = (byte)ControlModeType.NoControl;
                    controlMethod = (byte)ControlMethodType.NoControl;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axisConfiguration), axisConfiguration, null);
            }
        }

        private void ChangeFeedbackConfiguration(FeedbackConfigurationType value)
        {
            ModifiedCIPAxis.FeedbackConfiguration = (byte) value;
            ModifiedCIPAxis.FeedbackMode = (byte) value;

            ParentViewModel.Refresh();
        }

        private void ChangeApplicationType(ApplicationType value)
        {
            ModifiedCIPAxis.ApplicationType = (byte) value;
            AxisDefaultSetting.LoadDefaultIntegratorHold(ModifiedCIPAxis, value);

            ParentViewModel.Refresh();
        }

        private void ChangeLoopResponse(LoopResponseType value)
        {
            ModifiedCIPAxis.LoopResponse = (byte) value;
            AxisDefaultSetting.LoadDefaultDamping(ModifiedCIPAxis, value);

            ParentViewModel.Refresh();
        }

        private void UpdateAxisConfigurationSource()
        {
            // keep select
            var oldAxisConfiguration = AxisConfiguration;

            var cipMotionDrive = ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule as CIPMotionDrive;
            int axisNumber = ParentViewModel.ModifiedAxisCIPDrive.AxisNumber;

            if (cipMotionDrive != null)
            {
                IList supportList = cipMotionDrive.GetSupportAxisConfigurationList(axisNumber);
                AxisConfigurationSource = EnumHelper.ToDataSource<AxisConfigurationType>(supportList);

                if (!supportList.Contains(oldAxisConfiguration))
                {
                    AxisConfiguration = (AxisConfigurationType) supportList[0];
                }
                else
                {
                    AxisConfiguration = oldAxisConfiguration;
                }
            }
            else
            {
                AxisConfigurationSource = EnumHelper.ToDataSource<AxisConfigurationType>();
            }

            RaisePropertyChanged("AxisConfiguration");
        }

        private void UpdateFeedbackConfigurationSource()
        {
            // keep select
            var oldFeedbackConfiguration = FeedbackConfiguration;

            var supportList = new List<FeedbackConfigurationType>();

            // default
            switch (AxisConfiguration)
            {
                case AxisConfigurationType.FeedbackOnly:
                    supportList.Add(FeedbackConfigurationType.MasterFeedback);
                    break;
                case AxisConfigurationType.FrequencyControl:
                    supportList.Add(FeedbackConfigurationType.NoFeedback);
                    break;
                case AxisConfigurationType.PositionLoop:
                case AxisConfigurationType.VelocityLoop:
                case AxisConfigurationType.TorqueLoop:
                    supportList.Add(FeedbackConfigurationType.MotorFeedback);
                    break;
                case AxisConfigurationType.ConverterOnly:
                    supportList.Add(FeedbackConfigurationType.NoFeedback);
                    break;
            }

            // optional
            List<FeedbackConfigurationType> optionalList = null;

            var cipMotionDrive = ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule as CIPMotionDrive;

            if (cipMotionDrive != null)
                optionalList =
                    cipMotionDrive.GetEnumList<FeedbackConfigurationType>("FeedbackConfiguration", AxisConfiguration);

            if (optionalList != null && optionalList.Count > 0)
                supportList.AddRange(optionalList);

            FeedbackConfigurationSource = EnumHelper.ToDataSource<FeedbackConfigurationType>(supportList);

            FeedbackConfiguration =
                !supportList.Contains(FeedbackConfiguration) ? supportList[0] : oldFeedbackConfiguration;

            RaisePropertyChanged("FeedbackConfiguration");
        }

        private void UpdateMotionGroupSource()
        {
            var motionGroups = ParentViewModel.Controller.Tags
                .Where(tag => tag.DataTypeInfo.DataType.IsMotionGroupType).ToList();

            var resultList = motionGroups
                .Select(value => new
                {
                    DisplayName = value.Name,
                    Value = value
                }).ToList();

            resultList.Insert(0, new {DisplayName = "<none>", Value = (ITag) null});

            // keep select
            var oldMotionGroup = MotionGroup;

            MotionGroupSource = resultList;

            MotionGroup = !motionGroups.Contains(oldMotionGroup) ? null : oldMotionGroup;

            RaisePropertyChanged("MotionGroup");
        }
        
        private void DeviceModules_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateMotionDriveSource();
        }

        private void UpdateMotionDriveSource()
        {
            if (MotionDriveSource != null)
                foreach (var device in MotionDriveSource)
                {
                    var item = device?.GetType().GetProperty("Value")?.GetValue(device) as IDeviceModule;
                    if (item != null)
                        PropertyChangedEventManager.RemoveHandler(item, Item_PropertyChanged, "Name");
                }

            var availableMotionDrives = GetAvailableMotionDrives();

            var resultList = availableMotionDrives.Select(value => new
            {
                DisplayName = value.Name,
                Value = value
            }).ToList();

            resultList.Insert(0, new { DisplayName = "<none>", Value = (IDeviceModule)null });

            // keep select
            var selectedMotionDrive = MotionDrive;

            MotionDriveSource = resultList;

            if (selectedMotionDrive != null)
                if (!availableMotionDrives.Contains(selectedMotionDrive))
                    selectedMotionDrive = null;

            MotionDrive = selectedMotionDrive;

            RaisePropertyChanged("MotionDrive");
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateMotionDriveSource();
        }

        private List<IDeviceModule> GetAvailableMotionDrives()
        {
            var availableMotionDrives = new List<IDeviceModule>();
            var allCipMotionDrives =
                ParentViewModel.Controller.DeviceModules.OfType<CIPMotionDrive>().ToList();

            foreach (var cipMotionDrive in allCipMotionDrives)
            {
                List<int> axisNumbers =
                    cipMotionDrive.CandidateAxisNumbers(ParentViewModel.AxisTag);

                if (axisNumbers?.Count > 0)
                {
                    PropertyChangedEventManager.AddHandler(cipMotionDrive, Item_PropertyChanged, "Name");
                    availableMotionDrives.Add(cipMotionDrive);
                }
            }

            return availableMotionDrives;
        }

        private void UpdateAxisNumberSource()
        {
            List<int> axisNumbers = MotionDrive != null
                ? ((CIPMotionDrive) MotionDrive).CandidateAxisNumbers(ParentViewModel.AxisTag)
                : new List<int> {0};

            int oldAxisNumber = AxisNumber;

            AxisNumberSource = axisNumbers;

            if (!axisNumbers.Contains(oldAxisNumber))
                AxisNumber = axisNumbers[0];

            RaisePropertyChanged("AxisNumber");
        }

        protected override bool PropertiesChanged()
        {
            bool result = base.PropertiesChanged();
            if (result)
                return true;

            if (ParentViewModel.OriginalAxisCIPDrive.AssignedGroup !=
                ParentViewModel.ModifiedAxisCIPDrive.AssignedGroup)
                return true;

            if (ParentViewModel.OriginalAxisCIPDrive.AssociatedModule !=
                ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule)
                return true;

            if (ParentViewModel.OriginalAxisCIPDrive.AxisNumber !=
                ParentViewModel.ModifiedAxisCIPDrive.AxisNumber)
                return true;

            return false;
        }

        private void UIVisibilityAndReadonly()
        {
            RaisePropertyChanged("AxisConfigurationEnabled");
            RaisePropertyChanged("FeedbackConfigurationEnabled");
            RaisePropertyChanged("ApplicationTypeEnabled");
            RaisePropertyChanged("LoopResponseEnabled");
            RaisePropertyChanged("MotionGroupEnabled");
            RaisePropertyChanged("MotionDriveEnabled");
            RaisePropertyChanged("AxisNumberEnabled");
        }

        private void UIRefresh()
        {
            RaisePropertyChanged("ApplicationType");
            RaisePropertyChanged("LoopResponse");
            RaisePropertyChanged("UpdatePeriod");

            MotionGroupPropertiesCommand.RaiseCanExecuteChanged();
            AxisScheduleCommand.RaiseCanExecuteChanged();
        }

        #endregion

    }
}