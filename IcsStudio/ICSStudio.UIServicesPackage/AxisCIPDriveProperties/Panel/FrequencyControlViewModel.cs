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
    [SuppressMessage("ReSharper", "ArrangeAccessorOwnerBody")]
    internal class FrequencyControlViewModel : DefaultViewModel
    {
        public FrequencyControlViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            CompareProperties = new[]
            {
                "FrequencyControlMethod",
                "MaximumVoltage", "MaximumFrequency",
                "BreakVoltage", "BreakFrequency",
                "StartBoost", "RunBoost",

                "VelocityLimitPositive", "VelocityLimitNegative"
            };

            PeriodicRefreshProperties = new[]
            {
                "VelocityLimitPositive",
                "VelocityLimitNegative"
            };

            ParametersCommand = new RelayCommand(ExecuteParametersCommand);

            UpdateFrequencyControlMethodSource();
        }

        private void UpdateFrequencyControlMethodSource()
        {
            var supportedTypes = new List<FrequencyControlMethodType>();

            var axisConfiguration = (AxisConfigurationType)Convert.ToByte(ModifiedCIPAxis.AxisConfiguration);

            if (axisConfiguration != AxisConfigurationType.FrequencyControl)
                return;

            // Required
            supportedTypes.Add(FrequencyControlMethodType.BasicVoltsHertz);

            // Optional
            var motionDrive = ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule as CIPMotionDrive;
            var optionalSupportedTypes =
                motionDrive?.GetEnumList<FrequencyControlMethodType>("FrequencyControlMethod", axisConfiguration);

            if (optionalSupportedTypes != null && optionalSupportedTypes.Count > 0)
                foreach (var optionalSupportedType in optionalSupportedTypes)
                    if (!supportedTypes.Contains(optionalSupportedType))
                        supportedTypes.Add(optionalSupportedType);

            FrequencyControlMethodSource = EnumHelper.ToDataSource<FrequencyControlMethodType>(supportedTypes);
            if (supportedTypes.Count > 0)
                if (!supportedTypes.Contains(FrequencyControlMethod))
                    FrequencyControlMethod = supportedTypes[0];
        }

        public override Visibility Visibility
        {
            get
            {
                var axisConfiguration =
                    (AxisConfigurationType)Convert.ToByte(ParentViewModel.ModifiedAxisCIPDrive.CIPAxis
                        .AxisConfiguration);

                if (axisConfiguration == AxisConfigurationType.FrequencyControl)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public override void Show()
        {
            UpdateFrequencyControlMethodSource();

            UIVisibilityAndReadonly();
            UIRefresh();
        }

        private IList _frequencyControlMethodSource;

        public IList FrequencyControlMethodSource
        {
            get { return _frequencyControlMethodSource; }
            private set { Set(ref _frequencyControlMethodSource, value); }
        }

        public FrequencyControlMethodType FrequencyControlMethod
        {
            get { return (FrequencyControlMethodType)Convert.ToByte(ModifiedCIPAxis.FrequencyControlMethod); }
            set
            {
                ModifiedCIPAxis.FrequencyControlMethod = (byte)value;

                CheckDirty();

                UIVisibilityAndReadonly();

                RaisePropertyChanged();
            }
        }

        public float MaximumVoltage
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.MaximumVoltage); }
            set
            {
                float oldValue = Convert.ToSingle(ModifiedCIPAxis.MaximumVoltage);
                if (Math.Abs(oldValue - value) > float.Epsilon)
                {
                    ModifiedCIPAxis.MaximumVoltage = value;

                    CheckDirty();

                    RaisePropertyChanged();
                }
            }
        }

        public float MaximumFrequency
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.MaximumFrequency); }
            set
            {
                float oldValue = Convert.ToSingle(ModifiedCIPAxis.MaximumFrequency);
                if (Math.Abs(oldValue - value) > float.Epsilon)
                {
                    ModifiedCIPAxis.MaximumFrequency = value;

                    CheckDirty();

                    RaisePropertyChanged();
                }
            }
        }

        public float BreakVoltage
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.BreakVoltage); }
            set
            {
                float oldValue = Convert.ToSingle(ModifiedCIPAxis.BreakVoltage);
                if (Math.Abs(oldValue - value) > float.Epsilon)
                {
                    ModifiedCIPAxis.BreakVoltage = value;

                    CheckDirty();

                    RaisePropertyChanged();
                }
            }
        }

        public float BreakFrequency
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.BreakFrequency); }
            set
            {
                float oldValue = Convert.ToSingle(ModifiedCIPAxis.BreakFrequency);
                if (Math.Abs(oldValue - value) > float.Epsilon)
                {
                    ModifiedCIPAxis.BreakFrequency = value;

                    CheckDirty();

                    RaisePropertyChanged();
                }
            }
        }

        public float StartBoost
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.StartBoost); }
            set
            {
                float oldValue = Convert.ToSingle(ModifiedCIPAxis.StartBoost);
                if (Math.Abs(oldValue - value) > float.Epsilon)
                {
                    ModifiedCIPAxis.StartBoost = value;

                    CheckDirty();

                    RaisePropertyChanged();
                }
            }
        }

        public float RunBoost
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.RunBoost); }
            set
            {
                float oldValue = Convert.ToSingle(ModifiedCIPAxis.RunBoost);
                if (Math.Abs(oldValue - value) > float.Epsilon)
                {
                    ModifiedCIPAxis.RunBoost = value;

                    CheckDirty();

                    RaisePropertyChanged();
                }
            }
        }

        public Visibility BreakVisibility
        {
            get
            {
                if (FrequencyControlMethod == FrequencyControlMethodType.BasicVoltsHertz)
                    return Visibility.Visible;

                return Visibility.Hidden;
            }
        }

        public Visibility BoostVisibility
        {
            get
            {
                if (FrequencyControlMethod == FrequencyControlMethodType.BasicVoltsHertz)
                    return Visibility.Visible;

                if (FrequencyControlMethod == FrequencyControlMethodType.FanPumpVoltsHertz)
                    return Visibility.Visible;

                return Visibility.Hidden;
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

        public Visibility VelocityLimitPositiveVisibility
        {
            get
            {
                var axisConfiguration =
                    (AxisConfigurationType)Convert.ToByte(ModifiedCIPAxis.AxisConfiguration);

                CIPMotionDrive motionDrive =
                    ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule as CIPMotionDrive;

                if (motionDrive != null)
                {
                    if (motionDrive.SupportAxisAttribute(axisConfiguration, "VelocityLimitPositive"))
                        return Visibility.Visible;
                }

                return Visibility.Hidden;
            }
        }

        public Visibility VelocityLimitNegativeVisibility
        {
            get
            {
                var axisConfiguration =
                    (AxisConfigurationType)Convert.ToByte(ModifiedCIPAxis.AxisConfiguration);

                CIPMotionDrive motionDrive =
                    ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule as CIPMotionDrive;

                if (motionDrive != null)
                {
                    if (motionDrive.SupportAxisAttribute(axisConfiguration, "VelocityLimitNegative"))
                        return Visibility.Visible;
                }

                return Visibility.Hidden;
            }
        }

        public Visibility VelocityLimitVisibility
        {
            get
            {
                if (VelocityLimitPositiveVisibility == Visibility.Visible)
                    return Visibility.Visible;

                if (VelocityLimitNegativeVisibility == Visibility.Visible)
                    return Visibility.Visible;

                return Visibility.Hidden;
            }
        }

        public string PositionUnits => ModifiedCIPAxis.PositionUnits.GetString();

        public RelayCommand ParametersCommand { get; }

        private void ExecuteParametersCommand()
        {
            ParentViewModel.ShowPanel("Parameter List", "Frequency Control");
        }

        private void UIVisibilityAndReadonly()
        {
            RaisePropertyChanged(nameof(BreakVisibility));
            RaisePropertyChanged(nameof(BoostVisibility));

            RaisePropertyChanged(nameof(VelocityLimitPositiveVisibility));
            RaisePropertyChanged(nameof(VelocityLimitNegativeVisibility));
            RaisePropertyChanged(nameof(VelocityLimitVisibility));
        }

        private void UIRefresh()
        {
            RaisePropertyChanged(nameof(FrequencyControlMethod));
            RaisePropertyChanged(nameof(MaximumVoltage));
            RaisePropertyChanged(nameof(MaximumFrequency));
            RaisePropertyChanged(nameof(BreakVoltage));
            RaisePropertyChanged(nameof(BreakFrequency));
            RaisePropertyChanged(nameof(StartBoost));
            RaisePropertyChanged(nameof(RunBoost));

            RaisePropertyChanged(nameof(VelocityLimitPositive));
            RaisePropertyChanged(nameof(VelocityLimitNegative));

            RaisePropertyChanged(nameof(PositionUnits));
        }
    }
}