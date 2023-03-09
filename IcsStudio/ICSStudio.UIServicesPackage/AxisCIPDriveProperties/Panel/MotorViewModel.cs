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
using ICSStudio.Database.Database;
using ICSStudio.Database.Table.Motion;
using ICSStudio.Dialogs.ChangeCatalogNumber;
using ICSStudio.Dialogs.Warning;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class MotorViewModel : DefaultViewModel
    {
        private readonly MotionDbHelper _motionDbHelper;

        private IList _motorDataSources;
        private IList _motorTypeSource;
        private IList _motorUnitSource;

        public MotorViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            CompareProperties = new[]
            {
                "MotorDataSource", "MotorCatalogNumber", "MotorType", "MotorUnit",
                "MotorRatedOutputPower", "MotorRatedVoltage", "MotorRatedContinuousCurrent",
                "RotaryMotorRatedSpeed",
                "LinearMotorRatedSpeed", "LinearMotorMaxSpeed", "LinearMotorPolePitch",
                "PMMotorRatedTorque", "PMMotorRatedForce", "RotaryMotorPoles", "RotaryMotorMaxSpeed",
                "MotorRatedPeakCurrent", "MotorOverloadLimit",
                "InductionMotorRatedFrequency",
                // additional
                "MotorDeviceCode",
                "MotorIntegralThermalSwitch", "MotorWindingToAmbientCapacitance",
                "MotorWindingToAmbientResistance", "MotorMaxWindingTemperature"
            };

            _motionDbHelper = new MotionDbHelper();

            UpdateMotorDataSources();
            UpdateMotorTypeSource();
            UpdateMotorUnitSource();

            ChangeCatalogNumberCommand = new RelayCommand(ExecuteChangeCatalogNumberCommand);
            ParametersCommand = new RelayCommand(ExecuteParametersCommand);
        }

        public override void Show()
        {
            UpdateMotorDataSources();
            UpdateMotorTypeSource();
            UpdateMotorUnitSource();

            UIVisibilityAndReadonly();
            UIRefresh();
        }

        public override Visibility Visibility
        {
            get
            {
                var axisConfiguration =
                    (AxisConfigurationType)Convert.ToByte(ParentViewModel.ModifiedAxisCIPDrive.CIPAxis
                        .AxisConfiguration);

                if (axisConfiguration == AxisConfigurationType.ConverterOnly
                    || axisConfiguration == AxisConfigurationType.FeedbackOnly)
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        public override int CheckValid()
        {
            int result = 0;

            string message = string.Empty;
            string reason = string.Empty;
            string errorCode = string.Empty;

            if (MotorType == MotorType.RotaryPermanentMagnet || MotorType == MotorType.RotaryInduction)
            {
                float maxValue = float.MaxValue;
                float minValue = RotaryMotorRatedSpeed;

                if (!(RotaryMotorMaxSpeed >= minValue && RotaryMotorMaxSpeed <= maxValue))
                {
                    message = $"Failed to modify properties for axis '{ParentViewModel.AxisTag.Name}'";
                    reason = $"Enter a RotaryMotorMaxSpeed between {minValue} and {maxValue}.";
                    errorCode = "Error 16358-80044218";

                    result = -1;
                }
            }

            if (result < 0)
            {
                // show page
                ParentViewModel.ShowPanel("Motor");

                // show warning
                var warningDialog =
                    new WarningDialog(
                        message,
                        reason,
                        errorCode)
                    {
                        Owner = Application.Current.MainWindow
                    };
                warningDialog.ShowDialog();
            }

            return result;
        }

        public bool CatalogNumberEnabled
        {
            get
            {
                if (ParentViewModel.IsOnLine)
                    return false;

                return MotorDataSource == MotorDataSourceType.Database;
            }
        }

        public bool ChangeCatalogEnabled => CatalogNumberEnabled;

        public Visibility CatalogNumberVisibility =>
            MotorDataSource == MotorDataSourceType.MotorNV ? Visibility.Hidden : Visibility.Visible;

        public Visibility MotorTypeVisibility =>
            MotorDataSource == MotorDataSourceType.MotorNV ? Visibility.Hidden : Visibility.Visible;

        public IList MotorDataSources
        {
            get { return _motorDataSources; }
            set { Set(ref _motorDataSources, value); }
        }

        public MotorDataSourceType MotorDataSource
        {
            get { return (MotorDataSourceType)Convert.ToByte(ModifiedCIPAxis.MotorDataSource); }
            set
            {
                var motorDataSource = (MotorDataSourceType)Convert.ToByte(ModifiedCIPAxis.MotorDataSource);
                if (motorDataSource != value)
                {
                    ModifiedCIPAxis.MotorDataSource = (byte)value;

                    // Reset Motor Property
                    AxisDefaultSetting.ResetMotorProperty(ModifiedCIPAxis);

                    // update
                    UpdateMotorTypeSource();

                    RaisePropertyChanged("CatalogNumberEnabled");
                    RaisePropertyChanged("ChangeCatalogEnabled");
                    RaisePropertyChanged("CatalogNumberVisibility");
                    RaisePropertyChanged("MotorCatalogNumber");
                    RaisePropertyChanged("MotorTypeVisibility");
                    RaisePropertyChanged("MotorTypeEnabled");
                    RaisePropertyChanged("MotorUnitEnabled");

                    RaisePropertyChanged("RotaryPMParametersVisibility");
                    RaisePropertyChanged("LinearPMParametersVisibility");
                    RaisePropertyChanged("RotaryInductionParametersVisibility");

                    ParentViewModel.Refresh();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public bool MotorDataSourceEnabled => !ParentViewModel.IsOnLine;

        public string MotorCatalogNumber
        {
            get { return ModifiedCIPAxis.MotorCatalogNumber.GetString(); }
            set
            {
                ModifiedCIPAxis.MotorCatalogNumber = value;

                CheckDirty();
                RaisePropertyChanged();

                ParentViewModel.Refresh();

                // update
                UIVisibilityAndReadonly();
                UIRefresh();
            }
        }

        public bool MotorTypeEnabled
        {
            get
            {
                if (ParentViewModel.IsOnLine)
                    return false;

                return MotorDataSource != MotorDataSourceType.Database;
            }
        }

        public IList MotorTypeSource
        {
            get { return _motorTypeSource; }
            set { Set(ref _motorTypeSource, value); }
        }

        public MotorType MotorType
        {
            get { return (MotorType)Convert.ToByte(ModifiedCIPAxis.MotorType); }
            set
            {
                var motorType = (MotorType)Convert.ToByte(ModifiedCIPAxis.MotorType);
                if (motorType != value)
                {
                    ModifiedCIPAxis.MotorType = (byte)value;
                    AxisDefaultSetting.LoadDefaultMotorValues(ModifiedCIPAxis, value);
                }

                CheckDirty();

                // update
                UIVisibilityAndReadonly();
                UIRefresh();

                RaisePropertyChanged();
            }
        }

        public bool MotorUnitEnabled
        {
            get
            {
                if (ParentViewModel.IsOnLine)
                    return false;

                if (MotorDataSource == MotorDataSourceType.Datasheet && MotorType == MotorType.NotSpecified)
                    return true;

                if (MotorDataSource == MotorDataSourceType.MotorNV)
                    return true;

                return false;
            }
        }

        public IList MotorUnitSource
        {
            get { return _motorUnitSource; }
            set { Set(ref _motorUnitSource, value); }
        }

        public MotorUnitType MotorUnit
        {
            get { return (MotorUnitType)Convert.ToByte(ModifiedCIPAxis.MotorUnit); }
            set
            {
                ModifiedCIPAxis.MotorUnit = (byte)value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public Visibility RotaryPMParametersVisibility
        {
            get
            {
                if (MotorDataSource == MotorDataSourceType.Database ||
                    MotorDataSource == MotorDataSourceType.Datasheet)
                {
                    if (MotorType == MotorType.RotaryPermanentMagnet)
                        return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
        }

        public Visibility LinearPMParametersVisibility
        {
            get
            {
                if (MotorDataSource == MotorDataSourceType.Database ||
                    MotorDataSource == MotorDataSourceType.Datasheet)
                {
                    if (MotorType == MotorType.LinearPermanentMagnet)
                        return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
        }

        public Visibility RotaryInductionParametersVisibility
        {
            get
            {
                if (MotorDataSource == MotorDataSourceType.Database ||
                    MotorDataSource == MotorDataSourceType.Datasheet)
                {
                    if (MotorType == MotorType.RotaryInduction)
                        return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
        }

        public RelayCommand ChangeCatalogNumberCommand { get; }
        public RelayCommand ParametersCommand { get; }

        #region Parameters

        public bool MotorParameterEditEnabled
        {
            get
            {
                if (ParentViewModel.IsOnLine)
                    return false;

                if (MotorDataSource == MotorDataSourceType.Database &&
                    !string.Equals(MotorCatalogNumber, "<none>"))
                    return false;

                return true;
            }
        }

        public bool MotorOverloadLimitEnabled => !ParentViewModel.IsOnLine;

        public float MotorRatedOutputPower
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.MotorRatedOutputPower); }
            set
            {
                ModifiedCIPAxis.MotorRatedOutputPower = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float MotorRatedVoltage
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.MotorRatedVoltage); }
            set
            {
                ModifiedCIPAxis.MotorRatedVoltage = value;


                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float RotaryMotorRatedSpeed
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.RotaryMotorRatedSpeed); }
            set
            {
                ModifiedCIPAxis.RotaryMotorRatedSpeed = value;

                // TODO(gjc): Check max speed
                //if (RotaryMotorMaxSpeed < RotaryMotorRatedSpeed)
                //    RotaryMotorMaxSpeed = RotaryMotorRatedSpeed;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float MotorRatedContinuousCurrent
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.MotorRatedContinuousCurrent); }
            set
            {
                ModifiedCIPAxis.MotorRatedContinuousCurrent = value;

                // TODO(gjc): Check MotorRatedPeakCurrent
                //if (MotorRatedPeakCurrent < MotorRatedContinuousCurrent)
                //    MotorRatedPeakCurrent = MotorRatedContinuousCurrent;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float PMMotorRatedTorque
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.PMMotorRatedTorque); }
            set
            {
                ModifiedCIPAxis.PMMotorRatedTorque = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public ushort RotaryMotorPoles
        {
            get { return Convert.ToUInt16(ModifiedCIPAxis.RotaryMotorPoles); }
            set
            {
                ModifiedCIPAxis.RotaryMotorPoles = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float RotaryMotorMaxSpeed
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.RotaryMotorMaxSpeed); }
            set
            {
                ModifiedCIPAxis.RotaryMotorMaxSpeed = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float MotorRatedPeakCurrent
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.MotorRatedPeakCurrent); }
            set
            {
                ModifiedCIPAxis.MotorRatedPeakCurrent = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public Visibility MotorRatedPeakCurrentVisibility
        {
            get
            {
                var axisConfiguration =
                    (AxisConfigurationType)Convert.ToByte(ModifiedCIPAxis.AxisConfiguration);

                CIPMotionDrive motionDrive =
                    ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule as CIPMotionDrive;

                if (motionDrive != null)
                {
                    if (motionDrive.SupportAxisAttribute(axisConfiguration, "MotorRatedPeakCurrent"))
                        return Visibility.Visible;
                }

                return Visibility.Hidden;
            }
        }

        public float MotorOverloadLimit
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.MotorOverloadLimit); }
            set
            {
                ModifiedCIPAxis.MotorOverloadLimit = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        //
        public float LinearMotorRatedSpeed
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.LinearMotorRatedSpeed); }
            set
            {
                ModifiedCIPAxis.LinearMotorRatedSpeed = value;

                // TODO(gjc):Check max speed
                //if (LinearMotorMaxSpeed < LinearMotorRatedSpeed)
                //    LinearMotorMaxSpeed = LinearMotorRatedSpeed;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float PMMotorRatedForce
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.PMMotorRatedForce); }
            set
            {
                ModifiedCIPAxis.PMMotorRatedForce = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float LinearMotorPolePitch
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.LinearMotorPolePitch); }
            set
            {
                ModifiedCIPAxis.LinearMotorPolePitch = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float LinearMotorMaxSpeed
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.LinearMotorMaxSpeed); }
            set
            {
                ModifiedCIPAxis.LinearMotorMaxSpeed = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        //
        public float InductionMotorRatedFrequency
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.InductionMotorRatedFrequency); }
            set
            {
                ModifiedCIPAxis.InductionMotorRatedFrequency = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Command

        private void ExecuteParametersCommand()
        {
            ParentViewModel.ShowPanel("Parameter List", "Motor");
        }

        private void ExecuteChangeCatalogNumberCommand()
        {
            var cipMotionDrive = ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule as CIPMotionDrive;
            if (cipMotionDrive == null)
                return;

            bool isSupportConverterACInputVoltage =
                cipMotionDrive.Profiles.SupportDriveAttribute("ConverterACInputVoltage", cipMotionDrive.Major);
            bool isSupportConverterACInputPhasing =
                cipMotionDrive.Profiles.SupportDriveAttribute("ConverterACInputPhasing", cipMotionDrive.Major);

            int driveTypeId = -1;
            if (isSupportConverterACInputVoltage && isSupportConverterACInputPhasing)
            {
                driveTypeId = _motionDbHelper.GetMotionDriveTypeId(
                    cipMotionDrive.CatalogNumber,
                    cipMotionDrive.ConfigData.ConverterACInputVoltage,
                    cipMotionDrive.ConfigData.ConverterACInputPhasing);
            }
            else if (isSupportConverterACInputVoltage)
            {

            }
            else if (isSupportConverterACInputPhasing)
            {

            }
            else
            {
                var drives = _motionDbHelper.GetMotionDrive(cipMotionDrive.CatalogNumber);
                if (drives != null && drives.Count == 1)
                {
                    driveTypeId = drives[0].DriveTypeID;
                }
            }


            List<FeedbackType> supportFeedbackTypes
                = cipMotionDrive.GetSupportFeedback1Types(ParentViewModel.ModifiedAxisCIPDrive.AxisNumber);


            var supportMotorTypes =
                cipMotionDrive.GetEnumList<MotorType>("MotorType",
                    (AxisConfigurationType)Convert.ToByte(ModifiedCIPAxis.AxisConfiguration));

            var dialog =
                new ChangeCatalogNumberDialog(
                    driveTypeId,
                    supportFeedbackTypes,
                    supportMotorTypes,
                    MotorCatalogNumber)
                {
                    Owner = Application.Current.MainWindow
                };

            var dialogResult = dialog.ShowDialog();
            if (dialogResult != null && dialogResult.Value)
            {
                var motorId = dialog.SearchMotorId;

                if (motorId == 0)
                    MotorType = MotorType.NotSpecified;
                else
                    UpdateMotorParameters(motorId, driveTypeId);

                //Send changed message
                MotorCatalogNumber = dialog.SearchMotorCatalogNumber;
            }
        }

        #endregion

        #region Private Method

        private void UpdateMotorDataSources()
        {
            var supportTypes = new List<MotorDataSourceType>
            {
                MotorDataSourceType.Datasheet
            };

            if (ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule != null)
            {
                supportTypes.Add(MotorDataSourceType.Database);
                supportTypes.Add(MotorDataSourceType.MotorNV);
            }

            // 
            var oldMotorDataSource = MotorDataSource;

            MotorDataSources =
                EnumHelper.ToDataSource<MotorDataSourceType>(supportTypes);

            if (!supportTypes.Contains(oldMotorDataSource))
                MotorDataSource = supportTypes[0];

            RaisePropertyChanged("MotorDataSource");
        }

        private void UpdateMotorTypeSource()
        {
            var supportTypes = new List<MotorType>
            {
                MotorType.NotSpecified
            };

            var cipMotionDrive = ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule as CIPMotionDrive;
            var axisConfiguration = (AxisConfigurationType)Convert.ToByte(ModifiedCIPAxis.AxisConfiguration);

            if (cipMotionDrive != null)
            {
                var supportMotorType = cipMotionDrive.GetSupportMotorType(axisConfiguration);

                if (supportMotorType != null && supportMotorType.Count > 0)
                    supportTypes.AddRange(supportMotorType);
            }

            // for keep select
            var oldMotorType = (MotorType)Convert.ToByte(ModifiedCIPAxis.MotorType);

            MotorTypeSource = EnumHelper.ToDataSource<MotorType>(supportTypes);

            if (!supportTypes.Contains(oldMotorType))
                ModifiedCIPAxis.MotorType = (byte)supportTypes[0];
            else
                ModifiedCIPAxis.MotorType = (byte)oldMotorType;

            RaisePropertyChanged("MotorType");
        }

        private void UpdateMotorUnitSource()
        {
            var supportTypes = new List<MotorUnitType>
            {
                MotorUnitType.Rev,
                MotorUnitType.Meter
            };

            // keep select
            var oldMotorUnit = MotorUnit;

            MotorUnitSource = EnumHelper.ToDataSource<MotorUnitType>(supportTypes);

            if (!supportTypes.Contains(oldMotorUnit))
                MotorUnit = supportTypes[0];

            RaisePropertyChanged("MotorUnit");
        }

        private void UIVisibilityAndReadonly()
        {
            // ReSharper disable ExplicitCallerInfoArgument
            RaisePropertyChanged("RotaryPMParametersVisibility");
            RaisePropertyChanged("LinearPMParametersVisibility");
            RaisePropertyChanged("RotaryInductionParametersVisibility");

            RaisePropertyChanged("MotorDataSourceEnabled");
            RaisePropertyChanged("CatalogNumberEnabled");
            RaisePropertyChanged("ChangeCatalogEnabled");
            RaisePropertyChanged("CatalogNumberVisibility");
            RaisePropertyChanged("MotorTypeEnabled");
            RaisePropertyChanged("MotorTypeVisibility");
            RaisePropertyChanged("MotorUnitEnabled");

            RaisePropertyChanged("MotorParameterEditEnabled");
            RaisePropertyChanged("MotorOverloadLimitEnabled");
            // ReSharper restore ExplicitCallerInfoArgument

            RaisePropertyChanged(nameof(MotorRatedPeakCurrentVisibility));
        }

        private void UIRefresh()
        {
            // ReSharper disable ExplicitCallerInfoArgument
            RaisePropertyChanged("MotorDataSource");
            RaisePropertyChanged("MotorCatalogNumber");

            RaisePropertyChanged("MotorUnit");
            RaisePropertyChanged("MotorRatedOutputPower");
            RaisePropertyChanged("MotorRatedVoltage");
            RaisePropertyChanged("RotaryMotorRatedSpeed");
            RaisePropertyChanged("MotorRatedContinuousCurrent");
            RaisePropertyChanged("PMMotorRatedTorque");
            RaisePropertyChanged("RotaryMotorPoles");
            RaisePropertyChanged("RotaryMotorMaxSpeed");
            RaisePropertyChanged("MotorRatedPeakCurrent");
            RaisePropertyChanged("MotorOverloadLimit");

            RaisePropertyChanged("LinearMotorRatedSpeed");
            RaisePropertyChanged("PMMotorRatedForce");
            RaisePropertyChanged("LinearMotorPolePitch");
            RaisePropertyChanged("LinearMotorMaxSpeed");

            RaisePropertyChanged(nameof(InductionMotorRatedFrequency));
            // ReSharper restore ExplicitCallerInfoArgument
        }

        private void UpdateMotorParameters(int motorId, int driveTypeId)
        {
            var baseMotorParameters = _motionDbHelper.GetBaseMotorParameters(motorId);
            if (baseMotorParameters == null)
                return;

            MotorType = (MotorType)baseMotorParameters.MotorTypeID;
            if (MotorType == MotorType.RotaryPermanentMagnet)
            {
                var rotaryMotorParameters = _motionDbHelper.GetPMRotaryMotorParameters(motorId);
                var feedbackDeviceParameters =
                    _motionDbHelper.GetFeedbackDeviceParameters(baseMotorParameters.FeedbackDeviceID);

                if (rotaryMotorParameters == null || feedbackDeviceParameters == null)
                    return;

                UpdatePMRotaryMotorParameters(
                    driveTypeId,
                    baseMotorParameters,
                    rotaryMotorParameters,
                    feedbackDeviceParameters);
            }
            else if (MotorType == MotorType.LinearPermanentMagnet)
            {
                // TODO(gjc):add code here
            }
        }

        private void UpdatePMRotaryMotorParameters(
            int driveTypeId,
            Motor motorParameters,
            PMRotaryMotor rotaryMotorParameters,
            FeedbackDeviceView feedbackDeviceParameters)
        {
            MotorRatedOutputPower = (float)motorParameters.RatedPower;
            MotorRatedVoltage = (float)motorParameters.RatedVoltage;
            RotaryMotorPoles = (ushort)rotaryMotorParameters.PolesPerRev;

            RotaryMotorRatedSpeed = rotaryMotorParameters.RatedVelocity;
            RotaryMotorMaxSpeed = rotaryMotorParameters.MaxVelocity;

            MotorRatedContinuousCurrent = (float)motorParameters.RatedCurrent;
            MotorRatedPeakCurrent = (float)motorParameters.PeakCurrent;

            PMMotorRatedTorque = rotaryMotorParameters.RatedTorque;
            MotorOverloadLimit = (float)motorParameters.OverloadLimit;

            // additional
            ModifiedCIPAxis.MotorDeviceCode = (uint)motorParameters.MotorID;
            ModifiedCIPAxis.MotorIntegralThermalSwitch = motorParameters.IntegralThermostat ? (byte)1 : (byte)0;
            ModifiedCIPAxis.MotorWindingToAmbientCapacitance = motorParameters.Cthwa;
            ModifiedCIPAxis.MotorWindingToAmbientResistance = (float)motorParameters.Rthwa;
            ModifiedCIPAxis.MotorMaxWindingTemperature = (float)motorParameters.MtrMaxWindingTemp;

            // model
            ModifiedCIPAxis.PMMotorTorqueConstant = rotaryMotorParameters.TorqueConstant;
            ModifiedCIPAxis.PMMotorRotaryVoltageConstant = rotaryMotorParameters.VoltageConstant;
            ModifiedCIPAxis.PMMotorResistance = rotaryMotorParameters.Resistance;
            ModifiedCIPAxis.PMMotorInductance = rotaryMotorParameters.Inductance;

            ModifiedCIPAxis.PMMotorFluxSaturation.SetValue(0, rotaryMotorParameters.FluxSaturation1);
            ModifiedCIPAxis.PMMotorFluxSaturation.SetValue(1, rotaryMotorParameters.FluxSaturation2);
            ModifiedCIPAxis.PMMotorFluxSaturation.SetValue(2, rotaryMotorParameters.FluxSaturation3);
            ModifiedCIPAxis.PMMotorFluxSaturation.SetValue(3, rotaryMotorParameters.FluxSaturation4);
            ModifiedCIPAxis.PMMotorFluxSaturation.SetValue(4, rotaryMotorParameters.FluxSaturation5);
            ModifiedCIPAxis.PMMotorFluxSaturation.SetValue(5, rotaryMotorParameters.FluxSaturation6);
            ModifiedCIPAxis.PMMotorFluxSaturation.SetValue(6, rotaryMotorParameters.FluxSaturation7);
            ModifiedCIPAxis.PMMotorFluxSaturation.SetValue(7, rotaryMotorParameters.FluxSaturation8);

            //Motor feedback
            ModifiedCIPAxis.Feedback1Type = (byte)feedbackDeviceParameters.CipID;
            ModifiedCIPAxis.Feedback1CycleResolution = (uint)feedbackDeviceParameters.SingleTurnResolution;
            ModifiedCIPAxis.Feedback1CycleInterpolation =
                (uint)_motionDbHelper.GetInterpolationFactor(feedbackDeviceParameters.FeedbackTypeID, driveTypeId);
            ModifiedCIPAxis.Feedback1StartupMethod = (byte)feedbackDeviceParameters.Mode;
            ModifiedCIPAxis.Feedback1Turns = (uint)feedbackDeviceParameters.MultiTurnResolution;
            ModifiedCIPAxis.Feedback1VelocityFilterTaps = 1;
            ModifiedCIPAxis.Feedback1AccelFilterTaps = 1;
            //Feedback1Length

            ModifiedCIPAxis.FeedbackCommutationAligned =
                (byte)GetDefaultCommutationAlignment(
                    (FeedbackType)feedbackDeviceParameters.CipID,
                    rotaryMotorParameters.FactoryAligned);
            ModifiedCIPAxis.CommutationAlignment = ModifiedCIPAxis.FeedbackCommutationAligned;
            ModifiedCIPAxis.CommutationOffset = rotaryMotorParameters.CommutationOffset;

            //Load
            var bits = Convert.ToUInt16(ModifiedCIPAxis.GainTuningConfigurationBits);
            FlagsEnumHelper.SelectFlag(GainTuningConfigurationType.UseLoadRatio, true, ref bits);
            ModifiedCIPAxis.GainTuningConfigurationBits = bits;

            ModifiedCIPAxis.LoadRatio = 0;
            ModifiedCIPAxis.RotaryMotorInertia = rotaryMotorParameters.Inertia;
            ModifiedCIPAxis.TotalInertia = rotaryMotorParameters.Inertia;

        }

        private FeedbackCommutationAlignedType GetDefaultCommutationAlignment(
            FeedbackType feedbackType, bool factoryAligned)
        {
            //Commutation Alignment,page 210,rm003
            if (factoryAligned)
                switch (feedbackType)
                {
                    case FeedbackType.NotSpecified:
                        break;
                    case FeedbackType.DigitalAqB:
                        break;
                    case FeedbackType.DigitalAqBWithUvw:
                        return FeedbackCommutationAlignedType.DatabaseOffset;
                    case FeedbackType.DigitalParallel:
                        return FeedbackCommutationAlignedType.DatabaseOffset;
                    case FeedbackType.SineCosine:
                        break;
                    case FeedbackType.SineCosineWithUvw:
                        return FeedbackCommutationAlignedType.DatabaseOffset;
                    case FeedbackType.Hiperface:
                        return FeedbackCommutationAlignedType.MotorOffset;
                    case FeedbackType.EnDatSineCosine:
                        return FeedbackCommutationAlignedType.MotorOffset;
                    case FeedbackType.EnDatDigital:
                        return FeedbackCommutationAlignedType.MotorOffset;
                    case FeedbackType.Resolver:
                        return FeedbackCommutationAlignedType.DatabaseOffset;
                    case FeedbackType.SsiDigital:
                        return FeedbackCommutationAlignedType.DatabaseOffset;
                    case FeedbackType.Ldt:
                        // not found
                        break;
                    case FeedbackType.HiperfaceDsl:
                        return FeedbackCommutationAlignedType.MotorOffset;
                    case FeedbackType.BissDigital:
                        return FeedbackCommutationAlignedType.MotorOffset;
                    case FeedbackType.Integrated:
                        // not found
                        break;
                    case FeedbackType.SsiSineCosine:
                        return FeedbackCommutationAlignedType.DatabaseOffset;
                    case FeedbackType.SsiAqB:
                        return FeedbackCommutationAlignedType.DatabaseOffset;
                    case FeedbackType.BissSineCosine:
                        return FeedbackCommutationAlignedType.DatabaseOffset;
                    case FeedbackType.TamagawaSerial:
                        return FeedbackCommutationAlignedType.MotorOffset;
                    case FeedbackType.StahlSsi:
                        return FeedbackCommutationAlignedType.DatabaseOffset;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(feedbackType), feedbackType, null);
                }
            else
                switch (feedbackType)
                {
                    case FeedbackType.NotSpecified:
                        break;
                    case FeedbackType.DigitalAqB:
                        return FeedbackCommutationAlignedType.SelfSense;
                    case FeedbackType.DigitalAqBWithUvw:
                        return FeedbackCommutationAlignedType.NotAligned;
                    case FeedbackType.DigitalParallel:
                        return FeedbackCommutationAlignedType.NotAligned;
                    case FeedbackType.SineCosine:
                        return FeedbackCommutationAlignedType.SelfSense;
                    case FeedbackType.SineCosineWithUvw:
                        return FeedbackCommutationAlignedType.NotAligned;
                    case FeedbackType.Hiperface:
                        return FeedbackCommutationAlignedType.NotAligned;
                    case FeedbackType.EnDatSineCosine:
                        return FeedbackCommutationAlignedType.NotAligned;
                    case FeedbackType.EnDatDigital:
                        return FeedbackCommutationAlignedType.NotAligned;
                    case FeedbackType.Resolver:
                        return FeedbackCommutationAlignedType.NotAligned;
                    case FeedbackType.SsiDigital:
                        return FeedbackCommutationAlignedType.NotAligned;
                    case FeedbackType.Ldt:
                        break;
                    case FeedbackType.HiperfaceDsl:
                        return FeedbackCommutationAlignedType.NotAligned;
                    case FeedbackType.BissDigital:
                        return FeedbackCommutationAlignedType.NotAligned;
                    case FeedbackType.Integrated:
                        break;
                    case FeedbackType.SsiSineCosine:
                        return FeedbackCommutationAlignedType.NotAligned;
                    case FeedbackType.SsiAqB:
                        return FeedbackCommutationAlignedType.NotAligned;
                    case FeedbackType.BissSineCosine:
                        return FeedbackCommutationAlignedType.NotAligned;
                    case FeedbackType.TamagawaSerial:
                        return FeedbackCommutationAlignedType.NotAligned;
                    case FeedbackType.StahlSsi:
                        return FeedbackCommutationAlignedType.NotAligned;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(feedbackType), feedbackType, null);
                }

            return FeedbackCommutationAlignedType.NotAligned;
        }

        #endregion
    }
}