using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.Objects;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Gui.Utils;
using ICSStudio.MultiLanguage;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class ScalingViewModel : DefaultViewModel
    {
        private IList _loadTypeSource;
        private IList _motionUnitSource;

        private bool _softTravelLimitCheckingEnabled;

        private IList _travelModeSource;

        public ScalingViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            CompareProperties = new[]
            {
                "LoadType", "TransmissionRatioInput", "TransmissionRatioOutput",
                "ActuatorType", "ActuatorLead", "ActuatorLeadUnit",
                "ActuatorDiameter", "ActuatorDiameterUnit", "PositionUnits",
                "PositionScalingNumerator", "PositionScalingDenominator",
                "MotionUnit", "TravelMode", "TravelRange", "PositionUnwindNumerator",
                "PositionUnwindDenominator", "SoftTravelLimitChecking",
                "SoftTravelLimitPositive", "SoftTravelLimitNegative",
                "MotionResolution", "ConversionConstant",
                "PositionUnwind", "FeedbackUnitRatio",

                "MotionScalingConfiguration", "ScalingSource"
            };

            PeriodicRefreshProperties = new[]
            {
                "SoftTravelLimitChecking",
                "SoftTravelLimitPositive", "SoftTravelLimitNegative",
            };

            ParametersCommand = new RelayCommand(ExecuteParametersCommand);

            UpdateLoadTypeSource();
            ActuatorTypeSource = EnumHelper.ToDataSource<ActuatorType>();
            ActuatorLeadUnitSource = EnumHelper.ToDataSource<ActuatorLeadUnitType>();
            ActuatorDiameterUnitSource = EnumHelper.ToDataSource<ActuatorDiameterUnitType>();
            UpdateMotionUnitSource();
            UpdateTravelModeSource();
        }

        public override void Show()
        {
            UpdateLoadTypeSource();
            UpdateMotionUnitSource();
            UpdateTravelModeSource();

            UIVisibilityAndReadonly();
            UIRefresh();
        }

        public override int CheckValid()
        {
            int result = 0;

            string message = string.Empty;
            string reason = string.Empty;
            string errorCode = string.Empty;

            if (result == 0)
            {
                float minValue = 1.17549e-38f;
                if (!(ActuatorLead >= minValue && ActuatorLead <= float.MaxValue))
                {
                    message = LanguageManager.GetInstance().ConvertSpecifier("Failed to modify properties for axis") + $" '{ParentViewModel.AxisTag.Name}'";
                    reason = LanguageManager.GetInstance().ConvertSpecifier("The Enter range is:") + $" {minValue} - {float.MaxValue}";
                    errorCode = LanguageManager.GetInstance().ConvertSpecifier("Error") + " 16358-80044218";

                    result = -1;
                }
            }

            if (result == 0)
            {
                float minValue = 1.17549e-38f;
                if (!(ActuatorDiameter >= minValue && ActuatorDiameter <= float.MaxValue))
                {
                    message = LanguageManager.GetInstance().ConvertSpecifier("Failed to modify properties for axis") + $" '{ParentViewModel.AxisTag.Name}'";
                    reason = LanguageManager.GetInstance().ConvertSpecifier("The Enter range is:") + $" {minValue} - {float.MaxValue}";
                    errorCode = LanguageManager.GetInstance().ConvertSpecifier("Error") + " 16358-80044218";

                    result = -1;
                }
            }

            if (result == 0)
            {
                if (LoadType == LoadType.LinearActuator && ActuatorType == ActuatorType.None)
                {
                    message = LanguageManager.GetInstance().ConvertSpecifier("Failed to modify properties for axis") + $" '{ParentViewModel.AxisTag.Name}'";
                    reason = LanguageManager.GetInstance().ConvertSpecifier("The Load Type in combination with the specified Actuator Type is invalid.");
                    errorCode = LanguageManager.GetInstance().ConvertSpecifier("Error") + " 16427-80042FC3";

                    result = -1;
                }
            }

            if (result == 0)
            {
                float minValue = 1.17549e-38f;
                if (!(PositionScalingNumerator >= minValue && PositionScalingNumerator <= float.MaxValue))
                {
                    message = LanguageManager.GetInstance().ConvertSpecifier("Failed to modify properties for axis") + $" '{ParentViewModel.AxisTag.Name}'";
                    reason = LanguageManager.GetInstance().ConvertSpecifier("The Enter range is:") + $" {minValue} - {float.MaxValue}";
                    errorCode = LanguageManager.GetInstance().ConvertSpecifier("Error") + " 16358-80044218";

                    result = -1;
                }
            }

            if (result == 0)
            {
                float minValue = 1f;
                if (!(PositionScalingDenominator >= minValue && PositionScalingDenominator <= float.MaxValue))
                {
                    message = LanguageManager.GetInstance().ConvertSpecifier("Failed to modify properties for axis ") + $"'{ParentViewModel.AxisTag.Name}'";
                    reason = LanguageManager.GetInstance().ConvertSpecifier("The Enter range is:") + $" {minValue} - {float.MaxValue}";
                    errorCode = LanguageManager.GetInstance().ConvertSpecifier("Error") + " 16358-80044218";

                    result = -1;
                }
            }

            if (result == 0)
            {
                // add motion resolution loss check
                if (IsMotionResolutionLoss())
                {
                    message = LanguageManager.GetInstance().ConvertSpecifier("Failed to modify properties for axis") + $" '{ParentViewModel.AxisTag.Name}'";
                    reason = LanguageManager.GetInstance().ConvertSpecifier("The value entered for the Scaling Position Numerator or Scaling Position Denominator causes a loss in resolution When calculating Motion Resolution.");
                    errorCode = LanguageManager.GetInstance().ConvertSpecifier("Error") + " 16427-80042FD2";

                    result = -1;
                }
            }

            if (result == 0 && SoftTravelLimitChecking && SoftTravelLimitVisibility == Visibility.Visible)
            {
                var motionResolution = Convert.ToUInt32(ModifiedCIPAxis.MotionResolution);
                double travelRangeLimit = (double) int.MaxValue / motionResolution;
                travelRangeLimit = travelRangeLimit * PositionScalingNumerator / PositionScalingDenominator;

                double maxValue = travelRangeLimit;
                double minValue = -travelRangeLimit;

                minValue = minValue > SoftTravelLimitNegative ? minValue : SoftTravelLimitNegative;

                if (!(SoftTravelLimitPositive >= minValue && SoftTravelLimitPositive <= maxValue))
                {
                    message = LanguageManager.GetInstance().ConvertSpecifier("Failed to modify properties for axis") + $" '{ParentViewModel.AxisTag.Name}'";
                    reason = LanguageManager.GetInstance().ConvertSpecifier("The Enter range is:") + $" {minValue:F2} - {maxValue:F2}";
                    errorCode = LanguageManager.GetInstance().ConvertSpecifier("Error") + " 16358-80042035";

                    result = -1;
                }

                if (result == 0)
                {
                    maxValue = SoftTravelLimitPositive;
                    minValue = -travelRangeLimit;

                    if (!(SoftTravelLimitNegative >= minValue && SoftTravelLimitNegative <= maxValue))
                    {
                        message = LanguageManager.GetInstance().ConvertSpecifier("Failed to modify properties for axis") + $" '{ParentViewModel.AxisTag.Name}'";
                        reason = LanguageManager.GetInstance().ConvertSpecifier("The Enter range is:") + $" {minValue:F2} - {maxValue:F2}";
                        errorCode = LanguageManager.GetInstance().ConvertSpecifier("Error") + " 16358-80042034";

                        result = -1;
                    }

                }

            }

            if (result < 0)
            {
                // show page
                ParentViewModel.ShowPanel("Scaling");

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

        private bool IsMotionResolutionLoss()
        {
            double numerator = PositionScalingNumerator;
            double denominator = PositionScalingDenominator;

            double unwindNumerator = PositionUnwindNumerator;
            double unwindDenominator = PositionUnwindDenominator;

            double travelRange = TravelRange;

            uint maxResolution;
            uint baseResolution;

            double motionResolution = 0;

            uint defaultValue = PropertiesCalculation.GetDefaultMotionResolution(MotionUnit);

            if (TravelMode == TravelModeType.Cyclic)
            {
                maxResolution = (uint) ((2147483648 - 1) * (numerator / denominator) /
                                        (unwindNumerator / unwindDenominator));
                baseResolution = Math.Min(defaultValue, maxResolution);

                motionResolution = ((numerator * unwindDenominator) *
                                    Math.Pow(10,
                                        (int) (Math.Log10(baseResolution / (numerator * unwindDenominator)))));

            }

            if (TravelMode == TravelModeType.Limited)
            {
                maxResolution = (uint) ((2147483648 - 1) * (numerator / denominator) / travelRange);
                baseResolution = Math.Min(defaultValue, maxResolution);
                motionResolution = (numerator * Math.Pow(10, (int) (Math.Log10(baseResolution / numerator))));
            }

            if (TravelMode == TravelModeType.Unlimited)
            {
                baseResolution = defaultValue;
                motionResolution = (numerator * Math.Pow(10, (int) (Math.Log10(baseResolution / numerator))));
            }

            uint temp = (uint) motionResolution;

            if (temp == 0)
                return true;

            if (motionResolution - temp != 0)
                return true;

            return false;
        }

        public override Visibility Visibility
        {
            get
            {
                var axisConfiguration =
                    (AxisConfigurationType) Convert.ToByte(ModifiedCIPAxis.AxisConfiguration);

                if (axisConfiguration == AxisConfigurationType.ConverterOnly)
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        public bool IsOffline => !ParentViewModel.IsOnLine;

        public IList LoadTypeSource
        {
            get { return _loadTypeSource; }
            set { Set(ref _loadTypeSource, value); }
        }

        public LoadType LoadType
        {
            get { return (LoadType) Convert.ToByte(ModifiedCIPAxis.LoadType); }
            set
            {
                ModifiedCIPAxis.LoadType = (byte) value;

                // update
                AxisDefaultSetting.UpdateMotionUnit(ModifiedCIPAxis);
                UpdateMotionUnitSource();
                UpdateTravelModeSource();

                UIVisibilityAndReadonly();
                UIRefresh();

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public bool TransmissionEnabled
        {
            get
            {
                if (ParentViewModel.IsOnLine)
                    return false;

                switch (LoadType)
                {
                    case LoadType.DirectRotary:
                        return false;
                    case LoadType.DirectLinear:
                        return false;
                    case LoadType.RotaryTransmission:
                        return true;
                    case LoadType.LinearActuator:
                        return true;
                    default:
                        return true;
                }
            }
        }

        public uint TransmissionRatioInput
        {
            get { return Convert.ToUInt32(ModifiedCIPAxis.TransmissionRatioInput); }
            set
            {
                ModifiedCIPAxis.TransmissionRatioInput = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public uint TransmissionRatioOutput
        {
            get { return Convert.ToUInt32(ModifiedCIPAxis.TransmissionRatioOutput); }
            set
            {
                ModifiedCIPAxis.TransmissionRatioOutput = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public bool ActuatorEnabled
        {
            get
            {
                if (ParentViewModel.IsOnLine)
                    return false;

                switch (LoadType)
                {
                    case LoadType.DirectRotary:
                        return false;
                    case LoadType.DirectLinear:
                        return false;
                    case LoadType.RotaryTransmission:
                        return false;
                    case LoadType.LinearActuator:
                        return true;
                    default:
                        return true;
                }
            }
        }

        public bool ActuatorLeadEnabled => ActuatorEnabled && ActuatorType == ActuatorType.Screw;

        public bool ActuatorDiameterEnabled
        {
            get
            {
                if (ActuatorEnabled)
                    switch (ActuatorType)
                    {
                        case ActuatorType.None:
                            return true;
                        case ActuatorType.Screw:
                            return false;
                        case ActuatorType.BeltAndPulley:
                            return true;
                        case ActuatorType.ChainAndSprocket:
                            return true;
                        case ActuatorType.RackAndPinion:
                            return true;
                    }

                return false;
            }
        }

        public IList ActuatorTypeSource { get; }

        public ActuatorType ActuatorType
        {
            get { return (ActuatorType) Convert.ToByte(ModifiedCIPAxis.ActuatorType); }
            set
            {
                ModifiedCIPAxis.ActuatorType = (byte) value;

                // update
                AxisDefaultSetting.UpdateMotionUnit(ModifiedCIPAxis);
                UpdateMotionUnitSource();
                UpdateTravelModeSource();

                UIVisibilityAndReadonly();
                UIRefresh();

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float ActuatorLead
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.ActuatorLead); }
            set
            {
                ModifiedCIPAxis.ActuatorLead = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float ActuatorDiameter
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.ActuatorDiameter); }
            set
            {
                ModifiedCIPAxis.ActuatorDiameter = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public IList ActuatorLeadUnitSource { get; }

        public ActuatorLeadUnitType ActuatorLeadUnit
        {
            get { return (ActuatorLeadUnitType) Convert.ToByte(ModifiedCIPAxis.ActuatorLeadUnit); }
            set
            {
                ModifiedCIPAxis.ActuatorLeadUnit = (byte) value;

                // update
                AxisDefaultSetting.UpdateMotionUnit(ModifiedCIPAxis);
                UpdateMotionUnitSource();

                UIVisibilityAndReadonly();
                UIRefresh();

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public IList ActuatorDiameterUnitSource { get; }

        public ActuatorDiameterUnitType ActuatorDiameterUnit
        {
            get { return (ActuatorDiameterUnitType) Convert.ToByte(ModifiedCIPAxis.ActuatorDiameterUnit); }
            set
            {
                ModifiedCIPAxis.ActuatorDiameterUnit = (byte) value;

                // update
                AxisDefaultSetting.UpdateMotionUnit(ModifiedCIPAxis);
                UpdateMotionUnitSource();

                UIVisibilityAndReadonly();
                UIRefresh();

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public string PositionUnits
        {
            get { return ModifiedCIPAxis.PositionUnits.GetString(); }
            set
            {
                ModifiedCIPAxis.PositionUnits = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public bool PositionUnitsEnabled
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

        public float PositionScalingNumerator
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.PositionScalingNumerator); }
            set
            {
                ModifiedCIPAxis.PositionScalingNumerator = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public bool PositionScalingNumeratorEnabled
        {
            get
            {
                if (IsOffline)
                {
                    var scalingSource = (ScalingSourceType) Convert.ToByte(ModifiedCIPAxis.ScalingSource);
                    if (scalingSource == ScalingSourceType.FromCalculator)
                        return true;
                }

                return false;
            }
        }

        public float PositionScalingDenominator
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.PositionScalingDenominator); }
            set
            {
                ModifiedCIPAxis.PositionScalingDenominator = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public bool PositionScalingDenominatorEnabled
        {
            get
            {
                if (IsOffline)
                {
                    var scalingSource = (ScalingSourceType) Convert.ToByte(ModifiedCIPAxis.ScalingSource);
                    if (scalingSource == ScalingSourceType.FromCalculator)
                        return true;
                }

                return false;
            }
        }

        public IList MotionUnitSource
        {
            get { return _motionUnitSource; }
            set { Set(ref _motionUnitSource, value); }
        }

        public MotionUnitType MotionUnit
        {
            get { return (MotionUnitType) Convert.ToByte(ModifiedCIPAxis.MotionUnit); }
            set
            {
                ModifiedCIPAxis.MotionUnit = (byte) value;

                // update
                UpdateMotionUnitSource();

                UIRefresh();

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public Visibility TravelRangeVisibility
        {
            get
            {
                var axisConfiguration =
                    (AxisConfigurationType)Convert.ToByte(ModifiedCIPAxis.AxisConfiguration);

                if (axisConfiguration == AxisConfigurationType.FrequencyControl)
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        public bool TravelRangeEnabled
        {
            get
            {
                if (TravelMode == TravelModeType.Limited)
                    return true;

                return false;
            }
        }

        public bool TravelRangeReadOnly
        {
            get
            {
                var scalingSource = (ScalingSourceType) Convert.ToByte(ModifiedCIPAxis.ScalingSource);
                if (scalingSource == ScalingSourceType.DirectScalingFactorEntry)
                    return true;

                return false;
            }
        }

        public Visibility PositionUnwindVisibility
        {
            get
            {
                var axisConfiguration =
                    (AxisConfigurationType)Convert.ToByte(ModifiedCIPAxis.AxisConfiguration);

                if (axisConfiguration == AxisConfigurationType.FrequencyControl)
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        public bool PositionUnwindEnabled
        {
            get
            {
                if (TravelMode == TravelModeType.Cyclic)
                    return true;

                return false;
            }
        }

        public bool PositionUnwindReadOnly
        {
            get
            {
                var scalingSource = (ScalingSourceType) Convert.ToByte(ModifiedCIPAxis.ScalingSource);
                if (scalingSource == ScalingSourceType.DirectScalingFactorEntry)
                    return true;

                return false;
            }
        }

        public bool SoftTravelLimitChecking
        {
            get { return Convert.ToByte(ModifiedCIPAxis.SoftTravelLimitChecking) != 0; }
            set
            {
                ModifiedCIPAxis.SoftTravelLimitChecking = value ? (byte) 1 : (byte) 0;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public bool TravelModeEnabled
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

        public Visibility SoftTravelLimitVisibility
        {
            get
            {
                var axisConfiguration =
                    (AxisConfigurationType)Convert.ToByte(ModifiedCIPAxis.AxisConfiguration);

                if (axisConfiguration == AxisConfigurationType.FrequencyControl)
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        public bool SoftTravelLimitCheckingEnabled
        {
            get
            {
                if (ParentViewModel.IsPowerStructureEnabled)
                    return false;

                if (ParentViewModel.IsHardRunMode)
                    return false;

                return _softTravelLimitCheckingEnabled;
            }
            set { Set(ref _softTravelLimitCheckingEnabled, value); }
        }

        public IList TravelModeSource
        {
            get { return _travelModeSource; }
            set { Set(ref _travelModeSource, value); }
        }

        public TravelModeType TravelMode
        {
            get { return (TravelModeType) Convert.ToByte(ModifiedCIPAxis.TravelMode); }
            set
            {
                ModifiedCIPAxis.TravelMode = (byte) value;

                // update
                UIVisibilityAndReadonly();
                UIRefresh();

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float TravelRange
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.TravelRange); }
            set
            {
                ModifiedCIPAxis.TravelRange = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float PositionUnwindNumerator
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.PositionUnwindNumerator); }
            set
            {
                ModifiedCIPAxis.PositionUnwindNumerator = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float PositionUnwindDenominator
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.PositionUnwindDenominator); }
            set
            {
                ModifiedCIPAxis.PositionUnwindDenominator = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float SoftTravelLimitPositive
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.SoftTravelLimitPositive); }
            set
            {
                ModifiedCIPAxis.SoftTravelLimitPositive = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float SoftTravelLimitNegative
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.SoftTravelLimitNegative); }
            set
            {
                ModifiedCIPAxis.SoftTravelLimitNegative = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public RelayCommand ParametersCommand { get; }

        private void ExecuteParametersCommand()
        {
            ParentViewModel.ShowPanel("Parameter List", "Scaling");
        }

        #region Private

        private void UpdateLoadTypeSource()
        {
            var supportLoadTypes = new List<LoadType>();

            // TODO(gjc): need edit here
            var axisConfiguration = (AxisConfigurationType) Convert.ToByte(ModifiedCIPAxis.AxisConfiguration);
            if (axisConfiguration == AxisConfigurationType.FeedbackOnly)
            {
                var feedback1Unit = (FeedbackUnitType) Convert.ToByte(ModifiedCIPAxis.Feedback1Unit);
                if (feedback1Unit == FeedbackUnitType.Meter)
                {
                    supportLoadTypes.Add(LoadType.DirectLinear);
                }
                else
                {
                    supportLoadTypes.Add(LoadType.DirectRotary);
                    supportLoadTypes.Add(LoadType.RotaryTransmission);
                    supportLoadTypes.Add(LoadType.LinearActuator);
                }
            }
            else
            {
                var motorType = (MotorType) Convert.ToByte(ModifiedCIPAxis.MotorType);
                if (motorType == MotorType.LinearPermanentMagnet)
                {
                    supportLoadTypes.Add(LoadType.DirectLinear);
                }
                else
                {
                    supportLoadTypes.Add(LoadType.DirectRotary);
                    supportLoadTypes.Add(LoadType.RotaryTransmission);
                    supportLoadTypes.Add(LoadType.LinearActuator);
                }
            }

            var oldLoadType = LoadType;

            LoadTypeSource = EnumHelper.ToDataSource<LoadType>(supportLoadTypes);

            if (supportLoadTypes.Count > 0)
                if (!supportLoadTypes.Contains(oldLoadType))
                    ModifiedCIPAxis.LoadType = (byte) supportLoadTypes[0];
                else
                    ModifiedCIPAxis.LoadType = (byte) oldLoadType;

            RaisePropertyChanged("LoadType");
        }

        private void UpdateMotionUnitSource()
        {
            var oldMotionUnit = MotionUnit;

            MotionUnitSource = EnumHelper.ToSingleDataSource(MotionUnit);

            ModifiedCIPAxis.MotionUnit = (byte) oldMotionUnit;

            RaisePropertyChanged("MotionUnit");
        }

        private void UpdateTravelModeSource()
        {
            var axisConfiguration =
                (AxisConfigurationType)Convert.ToByte(ModifiedCIPAxis.AxisConfiguration);

            var travelModeSource = new List<TravelModeType>();

            if (LoadType == LoadType.DirectLinear)
            {
                travelModeSource.Add(TravelModeType.Limited);
            }
            else if (LoadType == LoadType.LinearActuator
                     && (ActuatorType == ActuatorType.Screw || ActuatorType == ActuatorType.RackAndPinion))
            {
                travelModeSource.Add(TravelModeType.Limited);
            }
            else
            {
                travelModeSource.Add(TravelModeType.Unlimited);
                travelModeSource.Add(TravelModeType.Limited);
                travelModeSource.Add(TravelModeType.Cyclic);
            }

            if (axisConfiguration == AxisConfigurationType.FrequencyControl)
            {
                travelModeSource = new List<TravelModeType> { TravelModeType.Unlimited };
            }


            var oldTravelMode = TravelMode;

            TravelModeSource = EnumHelper.ToDataSource<TravelModeType>(travelModeSource);

            if (!travelModeSource.Contains(oldTravelMode))
                TravelMode = travelModeSource[0];
            else
                ModifiedCIPAxis.TravelMode = (byte) oldTravelMode;

            RaisePropertyChanged("TravelMode");
        }

        private void UpdateTravelEnabled()
        {
            switch (TravelMode)
            {
                case TravelModeType.Unlimited:
                    SoftTravelLimitCheckingEnabled = true;
                    break;
                case TravelModeType.Limited:
                    SoftTravelLimitCheckingEnabled = true;
                    break;
                case TravelModeType.Cyclic:
                    SoftTravelLimitCheckingEnabled = false;
                    SoftTravelLimitChecking = false;
                    break;
            }

            RaisePropertyChanged(nameof(TravelRangeEnabled));
            RaisePropertyChanged("TravelRangeReadOnly");

            RaisePropertyChanged(nameof(PositionUnwindEnabled));
            RaisePropertyChanged("PositionUnwindReadOnly");
        }

        private void UIVisibilityAndReadonly()
        {
            UpdateTravelEnabled();

            RaisePropertyChanged("IsOffline");
            RaisePropertyChanged("TransmissionEnabled");
            RaisePropertyChanged("ActuatorEnabled");
            RaisePropertyChanged("ActuatorLeadEnabled");
            RaisePropertyChanged("ActuatorDiameterEnabled");

            RaisePropertyChanged("PositionUnitsEnabled");
            RaisePropertyChanged("TravelModeEnabled");
            RaisePropertyChanged("SoftTravelLimitCheckingEnabled");

            RaisePropertyChanged("PositionScalingNumeratorEnabled");
            RaisePropertyChanged("PositionScalingDenominatorEnabled");

            RaisePropertyChanged(nameof(TravelRangeVisibility));
            RaisePropertyChanged(nameof(TravelRangeEnabled));
            RaisePropertyChanged("TravelRangeReadOnly");

            RaisePropertyChanged(nameof(PositionUnwindVisibility));
            RaisePropertyChanged(nameof(PositionUnwindEnabled));
            RaisePropertyChanged("PositionUnwindReadOnly");

            RaisePropertyChanged(nameof(SoftTravelLimitVisibility));
        }

        private void UIRefresh()
        {

            RaisePropertyChanged("LoadType");

            RaisePropertyChanged("TransmissionRatioInput");
            RaisePropertyChanged("TransmissionRatioOutput");

            RaisePropertyChanged("ActuatorType");
            RaisePropertyChanged("ActuatorLead");

            RaisePropertyChanged("ActuatorLeadUnit");
            RaisePropertyChanged("ActuatorDiameter");
            RaisePropertyChanged("ActuatorDiameterUnit");

            RaisePropertyChanged("PositionUnits");
            RaisePropertyChanged("PositionScalingNumerator");
            RaisePropertyChanged("PositionScalingDenominator");

            RaisePropertyChanged("MotionUnit");

            RaisePropertyChanged("TravelMode");
            RaisePropertyChanged("TravelRange");

            RaisePropertyChanged("PositionUnwindNumerator");
            RaisePropertyChanged("PositionUnwindDenominator");

            RaisePropertyChanged("SoftTravelLimitChecking");
            RaisePropertyChanged("SoftTravelLimitPositive");
            RaisePropertyChanged("SoftTravelLimitNegative");
        }

        #endregion
    }
}