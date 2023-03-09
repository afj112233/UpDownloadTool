using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Utils;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class LoadViewModel : DefaultViewModel
    {
        private bool _useLoadRatioEnabled;

        public LoadViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            CompareProperties = new[]
            {
                "LoadCoupling", "GainTuningConfigurationBits",
                "LoadRatio", "RotaryMotorInertia",
                "LinearMotorMass", "TotalInertia", "TotalMass",
                "SystemInertia", "TorqueOffset"
            };

            PeriodicRefreshProperties = new[] { "LoadRatio", "TorqueOffset" };

            LoadCouplingSource = EnumHelper.ToDataSource<LoadCouplingType>();
        }

        public bool IsLoadEnabled
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

                if (axisConfiguration == AxisConfigurationType.PositionLoop
                    || axisConfiguration == AxisConfigurationType.VelocityLoop
                    || axisConfiguration == AxisConfigurationType.TorqueLoop)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public IList LoadCouplingSource { get; }

        public LoadCouplingType LoadCoupling
        {
            get { return (LoadCouplingType) Convert.ToByte(ModifiedCIPAxis.LoadCoupling); }
            set
            {
                ModifiedCIPAxis.LoadCoupling = (byte) value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public bool UseLoadRatioEnabled
        {
            get { return _useLoadRatioEnabled; }
            set { Set(ref _useLoadRatioEnabled, value); }
        }

        public bool UseLoadRatioChecked
        {
            get
            {
                var bits = Convert.ToUInt16(ModifiedCIPAxis.GainTuningConfigurationBits);
                return FlagsEnumHelper.ContainFlag(bits,
                    GainTuningConfigurationType.UseLoadRatio);
            }
            set
            {
                var bits = Convert.ToUInt16(ModifiedCIPAxis.GainTuningConfigurationBits);
                FlagsEnumHelper.SelectFlag(GainTuningConfigurationType.UseLoadRatio, value, ref bits);
                ModifiedCIPAxis.GainTuningConfigurationBits = bits;

                UIRefresh();

                CheckDirty();
                RaisePropertyChanged();
            }
        }



        public float LoadRatio
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.LoadRatio); }
            set
            {
                ModifiedCIPAxis.LoadRatio = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public Visibility RotaryMotorInertiaVisibility
        {
            get
            {
                var motorType = (MotorType) Convert.ToByte(ModifiedCIPAxis.MotorType);
                if (motorType == MotorType.RotaryPermanentMagnet)
                    return Visibility.Visible;
                return Visibility.Hidden;
            }

        }

        public float RotaryMotorInertia
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.RotaryMotorInertia); }
            set
            {
                ModifiedCIPAxis.RotaryMotorInertia = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public Visibility LinearMotorMassVisibility
        {
            get
            {
                var motorType = (MotorType) Convert.ToByte(ModifiedCIPAxis.MotorType);
                if (motorType == MotorType.LinearPermanentMagnet)
                    return Visibility.Visible;

                return Visibility.Hidden;
            }
        }

        public float LinearMotorMass
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.LinearMotorMass); }
            set
            {
                ModifiedCIPAxis.LinearMotorMass = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public bool TotalInertiaEnabled
        {
            get
            {
                var axisConfiguration = (AxisConfigurationType) Convert.ToByte(ModifiedCIPAxis.AxisConfiguration);
                switch (axisConfiguration)
                {
                    case AxisConfigurationType.FeedbackOnly:
                        break;
                    case AxisConfigurationType.FrequencyControl:
                        break;
                    case AxisConfigurationType.PositionLoop:
                        return !UseLoadRatioChecked;
                    case AxisConfigurationType.VelocityLoop:
                        return !UseLoadRatioChecked;
                    case AxisConfigurationType.TorqueLoop:
                        return false;
                    case AxisConfigurationType.ConverterOnly:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return false;
            }
        }

        public float TotalInertia
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.TotalInertia); }
            set
            {
                ModifiedCIPAxis.TotalInertia = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public bool TotalMassEnabled => !UseLoadRatioChecked;

        public float TotalMass
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.TotalMass); }
            set
            {
                ModifiedCIPAxis.TotalMass = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public Visibility InertiaMassCompensationVisibility
        {
            get
            {
                var axisConfiguration = (AxisConfigurationType) Convert.ToByte(ModifiedCIPAxis.AxisConfiguration);
                if (axisConfiguration == AxisConfigurationType.PositionLoop
                    || axisConfiguration == AxisConfigurationType.VelocityLoop)
                    return Visibility.Visible;

                return Visibility.Hidden;

            }
        }

        public bool SystemInertiaEnabled
        {
            get
            {
                // TODO(gjc): all false???
                var motorDataSource = (MotorDataSourceType) Convert.ToByte(ModifiedCIPAxis.MotorDataSource);

                switch (motorDataSource)
                {
                    case MotorDataSourceType.Datasheet:
                        return false;
                    case MotorDataSourceType.Database:
                        return false;
                    case MotorDataSourceType.DriveNV:
                        // TODO(gjc):need edit
                        break;
                    case MotorDataSourceType.MotorNV:
                        // TODO(gjc):need edit
                        break;
                }

                return false;
            }
        }

        public float SystemInertia => Convert.ToSingle(ModifiedCIPAxis.SystemInertia);

        public float SystemAcceleration
        {
            get
            {
                if (Math.Abs(SystemInertia) < float.Epsilon)
                    return 0f;
                return 100 / SystemInertia;
            }
        }

        public float TorqueOffset
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.TorqueOffset); }
            set
            {
                ModifiedCIPAxis.TorqueOffset = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }


        #region Private

        private void UpdateUseLoadRatioEnabled()
        {
            // UseLoadRatioEnabled, depend on Motor Type
            var motorType = (MotorType) Convert.ToByte(ModifiedCIPAxis.MotorType);
            var axisConfiguration = (AxisConfigurationType) Convert.ToByte(ModifiedCIPAxis.AxisConfiguration);

            if (motorType == MotorType.NotSpecified)
            {
                UseLoadRatioEnabled = false;
                UseLoadRatioChecked = false;
            }
            else
            {
                UseLoadRatioEnabled = true;
            }


            if (UseLoadRatioEnabled)
                if (axisConfiguration == AxisConfigurationType.TorqueLoop)
                    UseLoadRatioEnabled = false;
        }

        private void UIVisibilityAndReadonly()
        {
            RaisePropertyChanged("IsLoadEnabled");

            UpdateUseLoadRatioEnabled();

            RaisePropertyChanged("RotaryMotorInertiaVisibility");
            RaisePropertyChanged("LinearMotorMassVisibility");
            RaisePropertyChanged("InertiaMassCompensationVisibility");
        }

        private void UIRefresh()
        {
            RaisePropertyChanged("LoadCoupling");
            RaisePropertyChanged("UseLoadRatioChecked");
            RaisePropertyChanged("TotalInertiaEnabled");
            RaisePropertyChanged("TotalMassEnabled");
            RaisePropertyChanged("LoadRatio");
            RaisePropertyChanged("RotaryMotorInertia");
            RaisePropertyChanged("LinearMotorMass");
            RaisePropertyChanged("TotalInertia");
            RaisePropertyChanged("TotalMass");
            RaisePropertyChanged("SystemInertia");
            RaisePropertyChanged("SystemAcceleration");
            RaisePropertyChanged("SystemInertiaEnabled");
            RaisePropertyChanged("TorqueOffset");
        }

        #endregion
    }
}