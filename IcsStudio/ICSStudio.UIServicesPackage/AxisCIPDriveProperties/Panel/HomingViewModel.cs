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
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class HomingViewModel : DefaultViewModel
    {
        // ReSharper disable once InconsistentNaming
        private const uint HomeSwitchNormallyClosedBits = 0x2;

        private IList _homeModeSource;
        private IList _homeSequenceSource;

        public HomingViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            CompareProperties = new[]
            {
                "HomeMode", "HomePosition", "HomeOffset", "HomeSequence",
                "HomeConfigurationBits", "HomeDirection",
                "HomeSpeed", "HomeReturnSpeed"
            };
            UpdateHomeModeSource();
            UpdateHomeSequenceSource();
            HomeDirectionSource = EnumHelper.ToDataSource<HomeDirectionType>();

            TestMarkerCommand = new RelayCommand(ExecuteTestMarkerCommand, CanExecuteTestMarkerCommand);
        }

        public RelayCommand TestMarkerCommand { get; }

        public override void Show()
        {
            UIVisibilityAndReadonly();
            UIRefresh();
        }

        public override int CheckValid()
        {
            int result = 0;

            string message = string.Empty;
            string reason = string.Empty;
            string errorCode = string.Empty;

            var motionResolution = Convert.ToUInt32(ModifiedCIPAxis.MotionResolution);
            var positionScalingNumerator = Convert.ToSingle(ModifiedCIPAxis.PositionScalingNumerator);
            var positionScalingDenominator = Convert.ToSingle(ModifiedCIPAxis.PositionScalingDenominator);
            double travelRangeLimit = (double)int.MaxValue / motionResolution;
            travelRangeLimit = travelRangeLimit * positionScalingNumerator / positionScalingDenominator;

            double maxValue = travelRangeLimit;
            double minValue = -travelRangeLimit;

            if (Convert.ToByte(ModifiedCIPAxis.SoftTravelLimitChecking) != 0)
            {
                maxValue = Convert.ToSingle(ModifiedCIPAxis.SoftTravelLimitPositive);
                minValue = Convert.ToSingle(ModifiedCIPAxis.SoftTravelLimitNegative);
            }

            double maxPos = maxValue;
            double minPos = minValue;

            TravelModeType travelMode = (TravelModeType)Convert.ToByte(ModifiedCIPAxis.TravelMode);
            if (travelMode == TravelModeType.Cyclic)
            {
                minPos = 0;
                maxPos = Convert.ToSingle(ModifiedCIPAxis.PositionUnwindNumerator) /
                         Convert.ToSingle(ModifiedCIPAxis.PositionUnwindDenominator);
            }

            if (!(HomePosition >= minPos && HomePosition <= maxPos))
            {
                message = $"Failed to modify properties for axis '{ParentViewModel.AxisTag.Name}'";
                reason = $"Enter a HomePosition between {minPos:F2} and {maxPos:F2}.";
                errorCode = "Error 16358-80042035";

                result = -1;
            }

            if (result == 0)
            {
                if (!(HomeOffset >= minValue && HomeOffset <= maxValue))
                {
                    message = $"Failed to modify properties for axis '{ParentViewModel.AxisTag.Name}'";
                    reason = $"Enter a HomeOffset between {minValue:F2} and {maxValue:F2}.";
                    errorCode = "Error 16358-80042035";

                    result = -1;
                }
            }

            float conversionConstant = Convert.ToSingle(ModifiedCIPAxis.ConversionConstant);

            float maxpos = 2147483648 / conversionConstant;
            float maxspd = 1000 * maxpos;

            if (result == 0 && ActiveHomeSequenceGroupEnabled)
            {
                minValue = 0;
                maxValue = maxspd;

                if (!(HomeSpeed >= minValue && HomeSpeed <= maxValue))
                {
                    message = $"Failed to modify properties for axis '{ParentViewModel.AxisTag.Name}'";
                    reason = $"Enter a HomeSpeed between {minValue:F2} and {maxValue:e5}.";
                    errorCode = "Error 16358-80042035";

                    result = -1;
                }
            }

            if (result == 0 && ActiveHomeSequenceGroupEnabled)
            {
                minValue = 0;
                maxValue = maxspd;

                if (!(HomeReturnSpeed >= minValue && HomeReturnSpeed <= maxValue))
                {
                    message = $"Failed to modify properties for axis '{ParentViewModel.AxisTag.Name}'";
                    reason = $"Enter a HomeReturnSpeed between {minValue:F2} and {maxValue:e5}.";
                    errorCode = "Error 16358-80042035";

                    result = -1;
                }
            }

            //TODO(gjc): edit here
            if (!ActiveHomeSequenceGroupEnabled)
            {
                if (!(HomeSpeed >= minValue && HomeSpeed <= maxValue))
                    HomeSpeed = 0;

                if (!(HomeReturnSpeed >= minValue && HomeReturnSpeed <= maxValue))
                    HomeReturnSpeed = 0;
            }


            if (result < 0)
            {
                // show page
                ParentViewModel.ShowPanel("Homing");

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

        public bool IsHomingEnabled
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

        public override Visibility Visibility
        {
            get
            {
                var axisConfiguration =
                    (AxisConfigurationType)Convert.ToByte(ParentViewModel.ModifiedAxisCIPDrive.CIPAxis
                        .AxisConfiguration);

                if (axisConfiguration == AxisConfigurationType.ConverterOnly
                    || axisConfiguration == AxisConfigurationType.FrequencyControl)
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        public IList HomeModeSource
        {
            get { return _homeModeSource; }
            set { Set(ref _homeModeSource, value); }
        }

        public HomeModeType HomeMode
        {
            get { return (HomeModeType)Convert.ToByte(ModifiedCIPAxis.HomeMode); }
            set
            {
                ModifiedCIPAxis.HomeMode = (byte)value;

                UpdateHomeSequenceSource();

                UIVisibilityAndReadonly();

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float HomePosition
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.HomePosition); }
            set
            {
                ModifiedCIPAxis.HomePosition = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public string PositionUnits => ModifiedCIPAxis.PositionUnits.GetString();

        public bool OffsetEnabled => HomeSequence != HomeSequenceType.Immediate;

        public float HomeOffset
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.HomeOffset); }
            set
            {
                ModifiedCIPAxis.HomeOffset = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public IList HomeSequenceSource
        {
            get { return _homeSequenceSource; }
            set { Set(ref _homeSequenceSource, value); }
        }

        public HomeSequenceType HomeSequence
        {
            get { return (HomeSequenceType)Convert.ToByte(ModifiedCIPAxis.HomeSequence); }
            set
            {
                ModifiedCIPAxis.HomeSequence = (byte)value;

                UIVisibilityAndReadonly();

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public bool LimitSwitchEnabled
        {
            get
            {
                if (HomeSequence == HomeSequenceType.HomeToSwitch ||
                    HomeSequence == HomeSequenceType.HomeToSwitchThenMarker)
                    return true;

                return false;
            }
        }

        public bool HomeSwitchNormallyOpen
        {
            get { return !HomeSwitchNormallyClosed; }
            set
            {
                HomeSwitchNormallyClosed = !value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public bool HomeSwitchNormallyClosed
        {
            get
            {
                var homeConfigurationBits = Convert.ToUInt32(ModifiedCIPAxis.HomeConfigurationBits);
                return (homeConfigurationBits & HomeSwitchNormallyClosedBits) != 0;
            }
            set
            {
                var homeConfigurationBits = Convert.ToUInt32(ModifiedCIPAxis.HomeConfigurationBits);

                if (value)
                    homeConfigurationBits = homeConfigurationBits | HomeSwitchNormallyClosedBits;
                else
                    homeConfigurationBits = homeConfigurationBits & ~HomeSwitchNormallyClosedBits;

                ModifiedCIPAxis.HomeConfigurationBits = homeConfigurationBits;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public Visibility ActiveHomeSequenceGroupVisibility
        {
            get
            {
                var axisConfiguration = (AxisConfigurationType)Convert.ToByte(ModifiedCIPAxis.AxisConfiguration);
                if (axisConfiguration == AxisConfigurationType.PositionLoop ||
                    axisConfiguration == AxisConfigurationType.VelocityLoop)
                    return Visibility.Visible;

                return Visibility.Hidden;
            }
        }

        public bool ActiveHomeSequenceGroupEnabled
        {
            get
            {
                if (HomeSequence == HomeSequenceType.Immediate)
                    return false;

                return HomeMode == HomeModeType.Active;
            }
        }

        public IList HomeDirectionSource { get; }

        public HomeDirectionType HomeDirection
        {
            get { return (HomeDirectionType)Convert.ToByte(ModifiedCIPAxis.HomeDirection); }
            set
            {
                ModifiedCIPAxis.HomeDirection = (byte)value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float HomeSpeed
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.HomeSpeed); }
            set
            {
                ModifiedCIPAxis.HomeSpeed = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float HomeReturnSpeed
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.HomeReturnSpeed); }
            set
            {
                ModifiedCIPAxis.HomeReturnSpeed = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        #region Private

        private void UpdateHomeModeSource()
        {
            // rm003,page 102
            var supportTypes = new List<HomeModeType>
            {
                HomeModeType.Passive
            };

            var axisConfiguration = (AxisConfigurationType)Convert.ToByte(ModifiedCIPAxis.AxisConfiguration);
            switch (axisConfiguration)
            {
                case AxisConfigurationType.FeedbackOnly:
                    break;
                case AxisConfigurationType.FrequencyControl:
                    break;
                case AxisConfigurationType.PositionLoop:
                    supportTypes.Add(HomeModeType.Active);
                    break;
                case AxisConfigurationType.VelocityLoop:
                    supportTypes.Add(HomeModeType.Active);
                    break;
                case AxisConfigurationType.TorqueLoop:
                    supportTypes.Add(HomeModeType.Active);
                    break;
                case AxisConfigurationType.ConverterOnly:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var oldHomeMode = HomeMode;

            HomeModeSource = EnumHelper.ToDataSource<HomeModeType>(supportTypes);

            if (!supportTypes.Contains(oldHomeMode))
                ModifiedCIPAxis.HomeMode = (byte)supportTypes[0];
            else
                ModifiedCIPAxis.HomeMode = (byte)oldHomeMode;

            RaisePropertyChanged("HomeMode");
        }

        private void UpdateHomeSequenceSource()
        {
            IList supportTypes = null;

            var axisConfiguration = (AxisConfigurationType)Convert.ToByte(ModifiedCIPAxis.AxisConfiguration);
            var homeMode = (HomeModeType)Convert.ToByte(ModifiedCIPAxis.HomeMode);

            var cipMotionDrive = ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule as CIPMotionDrive;
            int axisNumber = ParentViewModel.ModifiedAxisCIPDrive.AxisNumber;
            if (cipMotionDrive != null)
                supportTypes =
                    cipMotionDrive.GetSupportHomeSequenceTypes(axisNumber);

            if (supportTypes == null)
                supportTypes = new List<HomeSequenceType>();

            if (!supportTypes.Contains(HomeSequenceType.Immediate))
                supportTypes.Insert(0, HomeSequenceType.Immediate);

            // special handle
            if (axisConfiguration == AxisConfigurationType.TorqueLoop && homeMode == HomeModeType.Active)
            {
                supportTypes.Clear();
                supportTypes.Add(HomeSequenceType.Immediate);
            }

            var oldHomeSequence = (HomeSequenceType)Convert.ToByte(ModifiedCIPAxis.HomeSequence);

            HomeSequenceSource = EnumHelper.ToDataSource<HomeSequenceType>(supportTypes);

            if (!supportTypes.Contains(oldHomeSequence))
                ModifiedCIPAxis.HomeSequence = (byte)supportTypes[0];
            else
                ModifiedCIPAxis.HomeSequence = (byte)oldHomeSequence;

            RaisePropertyChanged("HomeSequence");
        }


        private void UIVisibilityAndReadonly()
        {
            RaisePropertyChanged("ActiveHomeSequenceGroupVisibility");

            RaisePropertyChanged("IsHomingEnabled");
            RaisePropertyChanged("OffsetEnabled");
            RaisePropertyChanged("LimitSwitchEnabled");
            RaisePropertyChanged("ActiveHomeSequenceGroupEnabled");
        }

        private void UIRefresh()
        {
            RaisePropertyChanged("HomeMode");
            RaisePropertyChanged("HomePosition");
            RaisePropertyChanged("HomeOffset");
            RaisePropertyChanged("HomeSequence");
            RaisePropertyChanged("HomeSwitchNormallyOpen");
            RaisePropertyChanged("HomeSwitchNormallyClosed");
            RaisePropertyChanged("HomeDirection");
            RaisePropertyChanged("HomeSpeed");
            RaisePropertyChanged("HomeReturnSpeed");

            RaisePropertyChanged("PositionUnits");

            TestMarkerCommand.RaiseCanExecuteChanged();
        }

        private bool CanExecuteTestMarkerCommand()
        {
            return ParentViewModel.IsOnLine;
        }

        private void ExecuteTestMarkerCommand()
        {
            //TODO(ltw): add code here
        }

        #endregion
    }
}