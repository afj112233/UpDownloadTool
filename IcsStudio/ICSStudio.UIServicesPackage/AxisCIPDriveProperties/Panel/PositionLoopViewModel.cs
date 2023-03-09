using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Utils;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    internal class PositionLoopViewModel : DefaultViewModel
    {
        public PositionLoopViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            CompareProperties = new[]
            {
                "PositionLoopBandwidth",
                "PositionIntegratorBandwidth",
                "PositionIntegratorControl",
                "VelocityFeedforwardGain",
                "PositionErrorTolerance", "PositionErrorToleranceTime",
                "PositionLockTolerance",
                // 
                "PositionLeadLagFilterBandwidth", "PositionLeadLagFilterGain",
                "PositionServoBandwidth"
            };

            //TODO(gjc): need edit later
            PeriodicRefreshProperties = new[]
            {
                "PositionLoopBandwidth",
                "PositionIntegratorBandwidth",
                "PositionErrorTolerance",
                "VelocityFeedforwardGain"
            };

            ParametersCommand = new RelayCommand(ExecuteParametersCommand);

            IntegratorHoldSource = EnumHelper.ToDataSource<BooleanType>();
        }

        public bool IsPositionLoopEnabled
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
                    (AxisConfigurationType)Convert.ToByte(ParentViewModel.ModifiedAxisCIPDrive.CIPAxis
                        .AxisConfiguration);

                if (axisConfiguration == AxisConfigurationType.PositionLoop)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public float PositionLoopBandwidth
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.PositionLoopBandwidth); }
            set
            {
                ModifiedCIPAxis.PositionLoopBandwidth = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float PositionIntegratorBandwidth
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.PositionIntegratorBandwidth); }
            set
            {
                ModifiedCIPAxis.PositionIntegratorBandwidth = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public IList IntegratorHoldSource { get; }

        public BooleanType IntegratorHold
        {
            get
            {
                var bits = Convert.ToByte(ModifiedCIPAxis.PositionIntegratorControl);
                return (BooleanType)Convert.ToByte(FlagsEnumHelper.ContainFlag(bits,
                    IntegratorControlBitmap.IntegratorHoldEnable));
            }
            set
            {
                var bits = Convert.ToByte(ModifiedCIPAxis.PositionIntegratorControl);
                FlagsEnumHelper.SelectFlag(IntegratorControlBitmap.IntegratorHoldEnable, Convert.ToBoolean(value),
                    ref bits);

                ModifiedCIPAxis.PositionIntegratorControl = bits;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float VelocityFeedforwardGain
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.VelocityFeedforwardGain); }
            set
            {
                ModifiedCIPAxis.VelocityFeedforwardGain = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float PositionErrorTolerance
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.PositionErrorTolerance); }
            set
            {
                ModifiedCIPAxis.PositionErrorTolerance = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float PositionLockTolerance
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.PositionLockTolerance); }
            set
            {
                ModifiedCIPAxis.PositionLockTolerance = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public string PositionUnits => ModifiedCIPAxis.PositionUnits.GetString();

        public RelayCommand ParametersCommand { get; }

        private void ExecuteParametersCommand()
        {
            ParentViewModel.ShowPanel("Parameter List", "Position Loop");
        }

        private void UIVisibilityAndReadonly()
        {
            RaisePropertyChanged(nameof(IsPositionLoopEnabled));
        }

        private void UIRefresh()
        {
            RaisePropertyChanged(nameof(PositionLoopBandwidth));
            RaisePropertyChanged(nameof(PositionIntegratorBandwidth));
            RaisePropertyChanged(nameof(IntegratorHold));
            RaisePropertyChanged(nameof(VelocityFeedforwardGain));
            RaisePropertyChanged(nameof(PositionErrorTolerance));
            RaisePropertyChanged(nameof(PositionLockTolerance));
            RaisePropertyChanged(nameof(PositionUnits));

        }
    }
}