using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Utils;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class ActionsViewModel : DefaultViewModel
    {
        private IList _stoppingActionSource;
        private IList _inverterOverloadActionSource;

        public ActionsViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            CompareProperties = new[]
            {
                "StoppingAction",
                "MotorOverloadAction",
                "InverterOverloadAction",
                //"CIPAxisExceptionAction",
                //"MotionExceptionAction",
                "VelocityStandstillWindow",
                "VelocityThreshold",
                "StoppingTorque"
            };

            ParametersCommand = new RelayCommand(ExecuteParametersCommand);

            UpdateStoppingActionSource();
            MotorOverloadActionSource = EnumHelper.ToDataSource<MotorOverloadActionType>();
            UpdateInverterOverloadActionSource();
        }

        public bool IsActionsEnabled
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

        public override void Show()
        {
            UpdateStoppingActionSource();

            UIVisibilityAndReadonly();
            UIRefresh();
        }

        public override Visibility Visibility
        {
            get
            {
                var axisConfiguration =
                    (AxisConfigurationType) Convert.ToByte(ParentViewModel.ModifiedAxisCIPDrive.CIPAxis
                        .AxisConfiguration);

                if (axisConfiguration == AxisConfigurationType.FeedbackOnly)
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        public IList StoppingActionSource
        {
            get { return _stoppingActionSource; }
            set { Set(ref _stoppingActionSource, value); }
        }

        public StoppingActionType StoppingAction
        {
            get { return (StoppingActionType) Convert.ToByte(ModifiedCIPAxis.StoppingAction); }
            set
            {
                ModifiedCIPAxis.StoppingAction = (byte) value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public Visibility OverloadActionVisibility =>
            ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule == null
                ? Visibility.Hidden
                : Visibility.Visible;

        public IList MotorOverloadActionSource { get; }

        public MotorOverloadActionType MotorOverloadAction
        {
            get { return (MotorOverloadActionType) Convert.ToByte(ModifiedCIPAxis.MotorOverloadAction); }
            set
            {
                ModifiedCIPAxis.MotorOverloadAction = (byte) value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public IList InverterOverloadActionSource
        {
            get { return _inverterOverloadActionSource; }
            set { Set(ref _inverterOverloadActionSource, value); }
        }

        public InverterOverloadActionType InverterOverloadAction
        {
            get
            {
                return
                    (InverterOverloadActionType) Convert.ToByte(ModifiedCIPAxis.InverterOverloadAction);
            }
            set
            {
                ModifiedCIPAxis.InverterOverloadAction = (byte) value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public RelayCommand ParametersCommand { get; }

        private void ExecuteParametersCommand()
        {
            ParentViewModel.ShowPanel("Parameter List", "Actions");
        }

        #region Private

        private void UpdateStoppingActionSource()
        {
            // rm003, page 217
            var supportedTypes = new List<StoppingActionType>();

            // Required
            var axisConfiguration = (AxisConfigurationType) Convert.ToByte(ModifiedCIPAxis.AxisConfiguration);
            switch (axisConfiguration)
            {
                case AxisConfigurationType.FeedbackOnly:
                    // None
                    break;
                case AxisConfigurationType.FrequencyControl:
                    supportedTypes.Add(StoppingActionType.DisableCoast);
                    break;
                case AxisConfigurationType.PositionLoop:
                    supportedTypes.Add(StoppingActionType.DisableCoast);
                    supportedTypes.Add(StoppingActionType.CurrentDecelDisable);
                    break;
                case AxisConfigurationType.VelocityLoop:
                    supportedTypes.Add(StoppingActionType.DisableCoast);
                    supportedTypes.Add(StoppingActionType.CurrentDecelDisable);
                    break;
                case AxisConfigurationType.TorqueLoop:
                    supportedTypes.Add(StoppingActionType.DisableCoast);
                    supportedTypes.Add(StoppingActionType.CurrentDecelDisable);
                    break;
                case AxisConfigurationType.ConverterOnly:
                    // None
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Optional
            var motionDrive = ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule as CIPMotionDrive;
            var optionalSupportedTypes =
                motionDrive?.GetEnumList<StoppingActionType>("StoppingAction", axisConfiguration);

            if (optionalSupportedTypes != null && optionalSupportedTypes.Count > 0)
                foreach (var optionalSupportedType in optionalSupportedTypes)
                    if (!supportedTypes.Contains(optionalSupportedType))
                        supportedTypes.Add(optionalSupportedType);

            StoppingActionSource = EnumHelper.ToDataSource<StoppingActionType>(supportedTypes);
            if (supportedTypes.Count > 0)
                if (!supportedTypes.Contains(StoppingAction))
                    StoppingAction = supportedTypes[0];
        }

        private void UpdateInverterOverloadActionSource()
        {
            var supportTypes = new List<InverterOverloadActionType>
            {
                InverterOverloadActionType.None,
                InverterOverloadActionType.CurrentFoldback
            };

            var oldInverterOverloadAction = InverterOverloadAction;

            InverterOverloadActionSource = EnumHelper.ToDataSource<InverterOverloadActionType>(supportTypes);

            if (!supportTypes.Contains(oldInverterOverloadAction))
                ModifiedCIPAxis.InverterOverloadAction = (byte) supportTypes[0];
            else
                ModifiedCIPAxis.InverterOverloadAction = (byte) oldInverterOverloadAction;

            RaisePropertyChanged("InverterOverloadAction");
        }

        private void UIVisibilityAndReadonly()
        {
            RaisePropertyChanged("OverloadActionVisibility");
            RaisePropertyChanged("IsActionsEnabled");
        }

        private void UIRefresh()
        {
            RaisePropertyChanged("StoppingAction");
            RaisePropertyChanged("MotorOverloadAction");
            RaisePropertyChanged("InverterOverloadAction");
        }

        #endregion
    }
}