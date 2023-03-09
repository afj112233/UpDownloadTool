using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Utils;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class VelocityLoopViewModel : DefaultViewModel
    {
        public VelocityLoopViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            CompareProperties = new[]
            {
                "VelocityLoopBandwidth", "VelocityIntegratorBandwidth",
                "VelocityIntegratorControl",
                "AccelerationFeedforwardGain", "VelocityErrorTolerance",
                "VelocityLockTolerance", "VelocityLimitPositive",
                "VelocityLimitNegative",
                "VelocityServoBandwidth"
            };

            PeriodicRefreshProperties = new[]
            {
                "VelocityLoopBandwidth",
                "VelocityIntegratorBandwidth",
                "AccelerationFeedforwardGain",
                "VelocityErrorTolerance",
                "VelocityLimitPositive",
                "VelocityLimitNegative"
            };

            ParametersCommand = new RelayCommand(ExecuteParametersCommand);

            IntegratorHoldSource = EnumHelper.ToDataSource<BooleanType>();
        }

        public bool IsVelocityLoopEnabled
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
                    || axisConfiguration == AxisConfigurationType.VelocityLoop)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public float VelocityLoopBandwidth
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.VelocityLoopBandwidth); }
            set
            {
                ModifiedCIPAxis.VelocityLoopBandwidth = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float VelocityIntegratorBandwidth
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.VelocityIntegratorBandwidth); }
            set
            {
                ModifiedCIPAxis.VelocityIntegratorBandwidth = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public IList IntegratorHoldSource { get; }

        public BooleanType IntegratorHold
        {
            get
            {
                var bits = Convert.ToByte(ModifiedCIPAxis.VelocityIntegratorControl);
                return (BooleanType) Convert.ToByte(FlagsEnumHelper.ContainFlag(bits,
                    IntegratorControlBitmap.IntegratorHoldEnable));
            }
            set
            {
                var bits = Convert.ToByte(ModifiedCIPAxis.VelocityIntegratorControl);
                FlagsEnumHelper.SelectFlag(IntegratorControlBitmap.IntegratorHoldEnable, Convert.ToBoolean(value),
                    ref bits);

                ModifiedCIPAxis.VelocityIntegratorControl = bits;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float AccelerationFeedforwardGain
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.AccelerationFeedforwardGain); }
            set
            {
                ModifiedCIPAxis.AccelerationFeedforwardGain = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public Visibility LimitsVisibility =>
            ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule == null
                ? Visibility.Hidden
                : Visibility.Visible;

        public float VelocityErrorTolerance
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.VelocityErrorTolerance); }
            set
            {
                ModifiedCIPAxis.VelocityErrorTolerance = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float VelocityLockTolerance
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.VelocityLockTolerance); }
            set
            {
                ModifiedCIPAxis.VelocityLockTolerance = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float VelocityLimitPositive
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.VelocityLimitPositive); }
            set
            {
                ModifiedCIPAxis.VelocityLimitPositive = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float VelocityLimitNegative
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.VelocityLimitNegative); }
            set
            {
                ModifiedCIPAxis.VelocityLimitNegative = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public string PositionUnits => ModifiedCIPAxis.PositionUnits.GetString();

        public RelayCommand ParametersCommand { get; }

        private void ExecuteParametersCommand()
        {
            ParentViewModel.ShowPanel("Parameter List", "Velocity Loop");
        }

        private void UIVisibilityAndReadonly()
        {
            RaisePropertyChanged("IsVelocityLoopEnabled");
        }

        private void UIRefresh()
        {
            RaisePropertyChanged("LimitsVisibility");
            RaisePropertyChanged("PositionUnits");

            RaisePropertyChanged("VelocityLoopBandwidth");
            RaisePropertyChanged("VelocityIntegratorBandwidth");
            RaisePropertyChanged("IntegratorHold");
            RaisePropertyChanged("AccelerationFeedforwardGain");
            RaisePropertyChanged("VelocityErrorTolerance");
            RaisePropertyChanged("VelocityLockTolerance");
            RaisePropertyChanged("VelocityLimitPositive");
            RaisePropertyChanged("VelocityLimitNegative");
        }
    }
}