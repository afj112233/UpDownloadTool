using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Utils;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class MotorFeedbackViewModel : DefaultViewModel
    {
        private IList _feedback1TypeSource;
        private IList _feedback1UnitSource;
        private IList _feedbackCommutationAlignedSource;

        public MotorFeedbackViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            CompareProperties = new[]
            {
                "Feedback1Type", "Feedback1Unit",
                "Feedback1CycleResolution", "Feedback1CycleInterpolation",
                "Feedback1StartupMethod", "Feedback1Length", "Feedback1Turns",
                "FeedbackCommutationAligned", "CommutationAlignment",
                "CommutationOffset", "CommutationPolarity",
                "Feedback1AccelFilterBandwidth", "Feedback1AccelFilterTaps",
                "Feedback1VelocityFilterBandwidth", "Feedback1VelocityFilterTaps"
            };

            PeriodicRefreshProperties = new[]
            {
                "CommutationOffset"
            };

            ParametersCommand = new RelayCommand(ExecuteParametersCommand);

            UpdateFeedback1TypeSource();
            UpdateFeedback1UnitSource();
            StartupMethodSource = EnumHelper.ToDataSourceOrderByEnumMember<FeedbackStartupMethodType>();
            UpdateFeedbackCommutationAlignedSource();
            CommutationPolaritySource = EnumHelper.ToDataSourceOrderByEnumMember<PolarityType>();
        }

        public override void Show()
        {
            UpdateFeedback1TypeSource();
            UpdateFeedback1UnitSource();
            UpdateFeedbackCommutationAlignedSource();

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

                if (axisConfiguration == AxisConfigurationType.PositionLoop
                    || axisConfiguration == AxisConfigurationType.VelocityLoop
                    || axisConfiguration == AxisConfigurationType.TorqueLoop)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public IList Feedback1TypeSource
        {
            get { return _feedback1TypeSource; }
            set { Set(ref _feedback1TypeSource, value); }
        }

        public FeedbackType Feedback1Type
        {
            get { return (FeedbackType)Convert.ToByte(ModifiedCIPAxis.Feedback1Type); }
            set
            {
                var feedback1Type = (FeedbackType)Convert.ToByte(ModifiedCIPAxis.Feedback1Type);
                var feedback1Unit = (FeedbackUnitType)Convert.ToByte(ModifiedCIPAxis.Feedback1Unit);
                if (feedback1Type != value)
                {
                    ModifiedCIPAxis.Feedback1Type = (byte)value;

                    AxisDefaultSetting.LoadDefaultFeedback1Setting(ModifiedCIPAxis, value, feedback1Unit);

                    UIVisibilityAndReadonly();

                    UIRefresh();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public bool MotorParameterEditEnabled
        {
            get
            {
                if (ParentViewModel.IsOnLine)
                    return false;

                var motorDataSource = (MotorDataSourceType)Convert.ToByte(ModifiedCIPAxis.MotorDataSource);
                var motorCatalogNumber = ModifiedCIPAxis.MotorCatalogNumber.GetString();

                if (motorDataSource == MotorDataSourceType.Database &&
                    !string.Equals(motorCatalogNumber, "<none>"))
                    return false;

                return true;
            }
        }

        public IList Feedback1UnitSource
        {
            get { return _feedback1UnitSource; }
            set { Set(ref _feedback1UnitSource, value); }
        }

        public FeedbackUnitType Feedback1Unit
        {
            get { return (FeedbackUnitType)Convert.ToByte(ModifiedCIPAxis.Feedback1Unit); }
            set
            {
                var feedback1Unit = (FeedbackUnitType)Convert.ToByte(ModifiedCIPAxis.Feedback1Unit);
                if (feedback1Unit != value)
                {
                    ModifiedCIPAxis.Feedback1Unit = (byte)value;

                    UIVisibilityAndReadonly();

                    UIRefresh();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public Visibility FeedbackSettingVisibility =>
            Feedback1Type == FeedbackType.NotSpecified ? Visibility.Hidden : Visibility.Visible;

        public Visibility LengthVisibility
        {
            get
            {
                if (Feedback1StartupMethod == FeedbackStartupMethodType.Absolute
                    && Feedback1Unit == FeedbackUnitType.Meter)
                    return Visibility.Visible;

                return Visibility.Hidden;
            }
        }

        public Visibility TurnsVisibility
        {
            get
            {
                if (Feedback1StartupMethod == FeedbackStartupMethodType.Absolute
                    && Feedback1Unit == FeedbackUnitType.Rev)
                    return Visibility.Visible;

                return Visibility.Hidden;
            }
        }

        public uint Feedback1CycleResolution
        {
            get { return Convert.ToUInt32(ModifiedCIPAxis.Feedback1CycleResolution); }
            set
            {
                if (Convert.ToUInt32(ModifiedCIPAxis.Feedback1CycleResolution) != value)
                {
                    ModifiedCIPAxis.Feedback1CycleResolution = value;

                    UIRefresh();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public uint Feedback1CycleInterpolation
        {
            get { return Convert.ToUInt32(ModifiedCIPAxis.Feedback1CycleInterpolation); }
            set
            {
                if (Convert.ToUInt32(ModifiedCIPAxis.Feedback1CycleInterpolation) != value)
                {
                    ModifiedCIPAxis.Feedback1CycleInterpolation = value;

                    UIRefresh();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public ulong EffectiveResolution => (ulong)Feedback1CycleResolution * Feedback1CycleInterpolation;

        public IList StartupMethodSource { get; }

        public FeedbackStartupMethodType Feedback1StartupMethod
        {
            get
            {
                return
                    (FeedbackStartupMethodType)Convert.ToByte(ModifiedCIPAxis.Feedback1StartupMethod);
            }
            set
            {
                var feedback1StartupMethod =
                    (FeedbackStartupMethodType)Convert.ToByte(ModifiedCIPAxis.Feedback1StartupMethod);
                if (feedback1StartupMethod != value)
                {
                    ModifiedCIPAxis.Feedback1StartupMethod = (byte)value;

                    UIVisibilityAndReadonly();
                    UIRefresh();
                }

                CheckDirty();
                RaisePropertyChanged();
            }

        }

        public bool Feedback1StartupMethodEnabled => !ParentViewModel.IsOnLine;

        public float Feedback1Length
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.Feedback1Length); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedCIPAxis.Feedback1Length) - value) > float.Epsilon)
                {
                    ModifiedCIPAxis.Feedback1Length = value;

                    UIRefresh();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public uint Feedback1Turns
        {
            get { return Convert.ToUInt32(ModifiedCIPAxis.Feedback1Turns); }
            set
            {
                if (Convert.ToUInt32(ModifiedCIPAxis.Feedback1Turns) != value)
                {
                    ModifiedCIPAxis.Feedback1Turns = value;

                    UIRefresh();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public Visibility FeedbackCommutationVisibility
        {
            get
            {
                // Motor Type
                var motorType = (MotorType)Convert.ToByte(ModifiedCIPAxis.MotorType);
                if (motorType == MotorType.NotSpecified)
                {
                    return Visibility.Collapsed;
                }

                switch (Feedback1Type)
                {
                    case FeedbackType.NotSpecified:
                        return Visibility.Collapsed;
                    case FeedbackType.DigitalAqB:
                        return Visibility.Collapsed;
                    case FeedbackType.DigitalAqBWithUvw:
                        break;
                    case FeedbackType.DigitalParallel:
                        break;
                    case FeedbackType.SineCosine:
                        break;
                    case FeedbackType.SineCosineWithUvw:
                        break;
                    case FeedbackType.Hiperface:
                        break;
                    case FeedbackType.EnDatSineCosine:
                        break;
                    case FeedbackType.EnDatDigital:
                        break;
                    case FeedbackType.Resolver:
                        break;
                    case FeedbackType.SsiDigital:
                        break;
                    case FeedbackType.Ldt:
                        break;
                    case FeedbackType.HiperfaceDsl:
                        break;
                    case FeedbackType.BissDigital:
                        break;
                    case FeedbackType.Integrated:
                        break;
                    case FeedbackType.SsiSineCosine:
                        break;
                    case FeedbackType.SsiAqB:
                        break;
                    case FeedbackType.BissSineCosine:
                        break;
                    case FeedbackType.TamagawaSerial:
                        break;
                    case FeedbackType.StahlSsi:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return Visibility.Visible;
            }
        }

        public IList FeedbackCommutationAlignedSource
        {
            get { return _feedbackCommutationAlignedSource; }
            set { Set(ref _feedbackCommutationAlignedSource, value); }
        }

        public FeedbackCommutationAlignedType FeedbackCommutationAligned
        {
            get
            {
                return
                    (FeedbackCommutationAlignedType)Convert.ToByte(ModifiedCIPAxis.FeedbackCommutationAligned);
            }
            set
            {
                var feedbackCommutationAligned =
                    (FeedbackCommutationAlignedType)Convert.ToByte(ModifiedCIPAxis.FeedbackCommutationAligned);
                if (feedbackCommutationAligned != value)
                {
                    // TODO(gjc):need check
                    ModifiedCIPAxis.FeedbackCommutationAligned = (byte)value;
                    ModifiedCIPAxis.CommutationAlignment = (byte)value;

                    UIVisibilityAndReadonly();
                    UIRefresh();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public bool FeedbackCommutationAlignedEnabled => !ParentViewModel.IsOnLine;

        public bool CommutationOffsetEnabled
        {
            get
            {
                switch (FeedbackCommutationAligned)
                {
                    case FeedbackCommutationAlignedType.NotAligned:
                        break;
                    case FeedbackCommutationAlignedType.ControllerOffset:
                        return true;
                    case FeedbackCommutationAlignedType.MotorOffset:
                        break;
                    case FeedbackCommutationAlignedType.SelfSense:
                        break;
                    case FeedbackCommutationAlignedType.DatabaseOffset:
                        break;
                }

                return false;
            }
        }

        public float CommutationOffset
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.CommutationOffset); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedCIPAxis.CommutationOffset) - value) > float.Epsilon)
                {
                    ModifiedCIPAxis.CommutationOffset = value;

                    UIRefresh();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public Visibility CommutationPolarityVisibility
        {
            get
            {
                if (ParentViewModel.ModifiedAxisCIPDrive.SupportAttribute("CommutationPolarity"))
                    return Visibility.Visible;

                return Visibility.Hidden;
            }
        }

        public IList CommutationPolaritySource { get; }

        public PolarityType CommutationPolarity
        {
            get { return (PolarityType)Convert.ToByte(ModifiedCIPAxis.CommutationPolarity); }
            set
            {

                ModifiedCIPAxis.CommutationPolarity = (byte)value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public bool CommutationPolarityEnabled
        {
            get
            {
                if (ParentViewModel.IsOnLine)
                    return false;

                var motorDataSource = (MotorDataSourceType)Convert.ToByte(ModifiedCIPAxis.MotorDataSource);
                if (motorDataSource == MotorDataSourceType.Datasheet)
                    return true;

                return false;
            }
        }

        public RelayCommand ParametersCommand { get; }

        private void ExecuteParametersCommand()
        {
            ParentViewModel.ShowPanel("Parameter List", "Motor Feedback");
        }

        #region Private

        private void UpdateFeedback1TypeSource()
        {
            List<FeedbackType> supportFeedbackTypes = null;
            var cipMotionDrive = ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule as CIPMotionDrive;

            if (cipMotionDrive != null)
                supportFeedbackTypes =
                    cipMotionDrive.GetSupportFeedback1Types(ParentViewModel.ModifiedAxisCIPDrive.AxisNumber);

            if (supportFeedbackTypes == null)
                supportFeedbackTypes = new List<FeedbackType>();

            if (!supportFeedbackTypes.Contains(FeedbackType.NotSpecified))
                supportFeedbackTypes.Insert(0, FeedbackType.NotSpecified);

            var oldFeedback1Type = Feedback1Type;

            Feedback1TypeSource = EnumHelper.ToDataSource<FeedbackType>(supportFeedbackTypes);

            if (!supportFeedbackTypes.Contains(oldFeedback1Type))
                ModifiedCIPAxis.Feedback1Type = (byte)supportFeedbackTypes[0];
            else
                ModifiedCIPAxis.Feedback1Type = (byte)oldFeedback1Type;

            RaisePropertyChanged("Feedback1Type");
        }

        private void UpdateFeedback1UnitSource()
        {
            //(gjc):depend on motor type
            var supportFeedbackUnit = new List<FeedbackUnitType>();
            var motorType = (MotorType)Convert.ToByte(ModifiedCIPAxis.MotorType);
            switch (motorType)
            {
                case MotorType.NotSpecified:
                    supportFeedbackUnit.Add(FeedbackUnitType.Rev);
                    supportFeedbackUnit.Add(FeedbackUnitType.Meter);
                    break;
                case MotorType.RotaryPermanentMagnet:
                    supportFeedbackUnit.Add(FeedbackUnitType.Rev);
                    break;
                case MotorType.RotaryInduction:
                    supportFeedbackUnit.Add(FeedbackUnitType.Rev);
                    break;
                case MotorType.LinearPermanentMagnet:
                    supportFeedbackUnit.Add(FeedbackUnitType.Meter);
                    break;
                case MotorType.LinearInduction:
                    supportFeedbackUnit.Add(FeedbackUnitType.Meter);
                    break;
                case MotorType.RotaryInteriorPermanentMagnet:
                    supportFeedbackUnit.Add(FeedbackUnitType.Rev);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var oldFeedback1Unit = Feedback1Unit;

            Feedback1UnitSource = EnumHelper.ToDataSource<FeedbackUnitType>(supportFeedbackUnit);

            Feedback1Unit =
                !supportFeedbackUnit.Contains(oldFeedback1Unit)
                    ? supportFeedbackUnit[0]
                    : oldFeedback1Unit;

            RaisePropertyChanged("Feedback1Unit");
        }

        private void UpdateFeedbackCommutationAlignedSource()
        {
            //TODO(gjc):need edit
            var supportTypes = new List<FeedbackCommutationAlignedType>
            {
                FeedbackCommutationAlignedType.NotAligned,
                FeedbackCommutationAlignedType.ControllerOffset,
                FeedbackCommutationAlignedType.MotorOffset
            };

            var oldFeedbackCommutationAligned = FeedbackCommutationAligned;

            FeedbackCommutationAlignedSource =
                EnumHelper.ToDataSource<FeedbackCommutationAlignedType>(supportTypes);

            FeedbackCommutationAligned = !supportTypes.Contains(oldFeedbackCommutationAligned)
                ? supportTypes[0]
                : oldFeedbackCommutationAligned;

            RaisePropertyChanged("FeedbackCommutationAligned");
        }

        private void UIVisibilityAndReadonly()
        {
            RaisePropertyChanged("FeedbackSettingVisibility");
            RaisePropertyChanged("LengthVisibility");
            RaisePropertyChanged("TurnsVisibility");
            RaisePropertyChanged("FeedbackCommutationVisibility");
            RaisePropertyChanged("CommutationPolarityVisibility");

            RaisePropertyChanged("CommutationOffsetEnabled");
            RaisePropertyChanged("MotorParameterEditEnabled");
            RaisePropertyChanged("Feedback1StartupMethodEnabled");
            RaisePropertyChanged("FeedbackCommutationAlignedEnabled");
            RaisePropertyChanged("CommutationPolarityEnabled");
        }

        private void UIRefresh()
        {
            RaisePropertyChanged("Feedback1Type");
            RaisePropertyChanged("Feedback1Unit");

            RaisePropertyChanged("Feedback1CycleResolution");
            RaisePropertyChanged("Feedback1CycleInterpolation");
            RaisePropertyChanged("EffectiveResolution");
            RaisePropertyChanged("Feedback1StartupMethod");
            RaisePropertyChanged("Feedback1Length");
            RaisePropertyChanged("Feedback1Turns");
            RaisePropertyChanged("FeedbackCommutationAligned");
            RaisePropertyChanged("CommutationOffset");
            RaisePropertyChanged("CommutationPolarity");
        }

        #endregion
    }
}