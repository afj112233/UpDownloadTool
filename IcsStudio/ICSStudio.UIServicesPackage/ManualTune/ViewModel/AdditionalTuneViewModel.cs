using System;
using System.Collections;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Utils;
using ICSStudio.SimpleServices.DataWrapper;

namespace ICSStudio.UIServicesPackage.ManualTune.ViewModel
{
    public class AdditionalTuneViewModel : ViewModelBase
    {
        private readonly ManualTuneViewModel _parentViewModel;

        public AdditionalTuneViewModel(ManualTuneViewModel parentViewModel)
        {
            _parentViewModel = parentViewModel;

            AdaptiveTuningConfigurationSource = EnumHelper.ToDataSource<AdaptiveTuningConfigurationType>();
        }

        public AxisCIPDrive ModifiedAxisCIPDrive => _parentViewModel.ModifiedAxisCIPDrive;

        public bool EditEnabled => ModifiedAxisCIPDrive.Controller.IsOnline;

        #region Feedforward

        public float VelocityFeedforwardGain
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.VelocityFeedforwardGain); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.VelocityFeedforwardGain) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.VelocityFeedforwardGain = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public float AccelerationFeedforwardGain
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.AccelerationFeedforwardGain); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.AccelerationFeedforwardGain) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.AccelerationFeedforwardGain = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Compensation

        public string PositionUnits => ModifiedAxisCIPDrive.CIPAxis.PositionUnits.GetString();

        public float SystemInertia
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.SystemInertia); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.SystemInertia) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.SystemInertia = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public float TorqueOffset
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.TorqueOffset); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.TorqueOffset) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.TorqueOffset = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public float FrictionCompensationSliding
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.FrictionCompensationSliding); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.FrictionCompensationSliding) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.FrictionCompensationSliding = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public float FrictionCompensationWindow
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.FrictionCompensationWindow); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.FrictionCompensationWindow) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.FrictionCompensationWindow = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public float BacklashCompensationWindow
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.BacklashCompensationWindow); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.BacklashCompensationWindow) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.BacklashCompensationWindow = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public string LoadObserverConfiguration
        {
            get
            {
                var loadObserverConfiguration =
                    (LoadObserverConfigurationType)Convert.ToByte(ModifiedAxisCIPDrive.CIPAxis
                        .LoadObserverConfiguration);
                return EnumHelper.GetEnumMember(loadObserverConfiguration);
            }
        }

        public float LoadObserverBandwidth
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.LoadObserverBandwidth); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.LoadObserverBandwidth) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.LoadObserverBandwidth = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public float LoadObserverIntegratorBandwidth
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.LoadObserverIntegratorBandwidth); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.LoadObserverIntegratorBandwidth) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.LoadObserverIntegratorBandwidth = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public bool LoadObserverEnabled
        {
            get
            {
                var loadObserverConfiguration =
                    (LoadObserverConfigurationType)Convert.ToByte(ModifiedAxisCIPDrive.CIPAxis
                        .LoadObserverConfiguration);
                if (loadObserverConfiguration == LoadObserverConfigurationType.Disabled)
                    return false;

                return true;
            }
        }

        public bool LoadObserverEditEnabled => EditEnabled && LoadObserverEnabled;

        #endregion

        #region Filters

        public float TorqueLowPassFilterBandwidth
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.TorqueLowPassFilterBandwidth); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.TorqueLowPassFilterBandwidth) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.TorqueLowPassFilterBandwidth = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public float TorqueNotchFilterFrequency
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.TorqueNotchFilterFrequency); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.TorqueNotchFilterFrequency) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.TorqueNotchFilterFrequency = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public float TorqueLeadLagFilterGain
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.TorqueLeadLagFilterGain); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.TorqueLeadLagFilterGain) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.TorqueLeadLagFilterGain = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public float TorqueLeadLagFilterBandwidth
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.TorqueLeadLagFilterBandwidth); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.TorqueLeadLagFilterBandwidth) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.TorqueLeadLagFilterBandwidth = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public IList AdaptiveTuningConfigurationSource { get; }

        public AdaptiveTuningConfigurationType AdaptiveTuningConfiguration
        {
            get
            {
                return (AdaptiveTuningConfigurationType)Convert.ToByte(ModifiedAxisCIPDrive.CIPAxis
                    .AdaptiveTuningConfiguration);
            }
            set
            {
                if (Convert.ToByte(ModifiedAxisCIPDrive.CIPAxis.AdaptiveTuningConfiguration) != (byte)value)
                {
                    ModifiedAxisCIPDrive.CIPAxis.AdaptiveTuningConfiguration = (byte)value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public float TorqueNotchFilterHighFrequencyLimit
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.TorqueNotchFilterHighFrequencyLimit); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.TorqueNotchFilterHighFrequencyLimit) -
                             value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.TorqueNotchFilterHighFrequencyLimit = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public float TorqueNotchFilterLowFrequencyLimit
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.TorqueNotchFilterLowFrequencyLimit); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.TorqueNotchFilterLowFrequencyLimit) -
                             value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.TorqueNotchFilterLowFrequencyLimit = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public float TorqueNotchFilterTuningThreshold
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.TorqueNotchFilterTuningThreshold); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.TorqueNotchFilterTuningThreshold) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.TorqueNotchFilterTuningThreshold = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public bool AdaptiveTuningEnabled
        {
            get
            {
                var adaptiveTuningConfiguration =
                    (AdaptiveTuningConfigurationType)Convert.ToByte(ModifiedAxisCIPDrive.CIPAxis
                        .AdaptiveTuningConfiguration);
                if (adaptiveTuningConfiguration == AdaptiveTuningConfigurationType.Disabled)
                    return false;
                return true;
            }
        }

        public bool AdaptiveTuningEditEnabled => EditEnabled;

        #endregion

        #region Limits

        public float TorqueLimitPositive
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.TorqueLimitPositive); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.TorqueLimitPositive) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.TorqueLimitPositive = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public float TorqueLimitNegative
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.TorqueLimitNegative); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.TorqueLimitNegative) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.TorqueLimitNegative = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public float AccelerationLimit
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.AccelerationLimit); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.AccelerationLimit) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.AccelerationLimit = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public float DecelerationLimit
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.DecelerationLimit); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.DecelerationLimit) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.DecelerationLimit = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public float VelocityLimitPositive
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.VelocityLimitPositive); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.VelocityLimitPositive) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.VelocityLimitPositive = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public float VelocityLimitNegative
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.VelocityLimitNegative); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.VelocityLimitNegative) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.VelocityLimitNegative = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Planner

        public float MaximumSpeed
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.MaximumSpeed); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.MaximumSpeed) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.MaximumSpeed = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public float MaximumAcceleration
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.MaximumAcceleration); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.MaximumAcceleration) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.MaximumAcceleration = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public float MaximumDeceleration
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.MaximumDeceleration); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.MaximumDeceleration) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.MaximumDeceleration = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public float MaximumAccelerationJerk
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.MaximumAccelerationJerk); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.MaximumAccelerationJerk) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.MaximumAccelerationJerk = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public float MaximumDecelerationJerk
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.MaximumDecelerationJerk); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.MaximumDecelerationJerk) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.MaximumDecelerationJerk = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        public void Refresh()
        {
            RaisePropertyChanged(nameof(EditEnabled));

            RaisePropertyChanged(nameof(VelocityFeedforwardGain));
            RaisePropertyChanged(nameof(AccelerationFeedforwardGain));
            RaisePropertyChanged(nameof(PositionUnits));
            RaisePropertyChanged(nameof(SystemInertia));
            RaisePropertyChanged(nameof(TorqueOffset));
            RaisePropertyChanged(nameof(FrictionCompensationSliding));
            RaisePropertyChanged(nameof(FrictionCompensationWindow));
            RaisePropertyChanged(nameof(BacklashCompensationWindow));
            RaisePropertyChanged(nameof(LoadObserverConfiguration));
            RaisePropertyChanged(nameof(LoadObserverBandwidth));
            RaisePropertyChanged(nameof(LoadObserverIntegratorBandwidth));
            RaisePropertyChanged(nameof(LoadObserverEnabled));
            RaisePropertyChanged(nameof(LoadObserverEditEnabled));
            RaisePropertyChanged(nameof(TorqueLowPassFilterBandwidth));
            RaisePropertyChanged(nameof(TorqueNotchFilterFrequency));
            RaisePropertyChanged(nameof(TorqueLeadLagFilterGain));
            RaisePropertyChanged(nameof(TorqueLeadLagFilterBandwidth));
            RaisePropertyChanged(nameof(AdaptiveTuningConfiguration));
            RaisePropertyChanged(nameof(TorqueNotchFilterHighFrequencyLimit));
            RaisePropertyChanged(nameof(TorqueNotchFilterLowFrequencyLimit));
            RaisePropertyChanged(nameof(TorqueNotchFilterTuningThreshold));
            RaisePropertyChanged(nameof(AdaptiveTuningEnabled));
            RaisePropertyChanged(nameof(AdaptiveTuningEditEnabled));
            RaisePropertyChanged(nameof(TorqueLimitPositive));
            RaisePropertyChanged(nameof(TorqueLimitNegative));
            RaisePropertyChanged(nameof(AccelerationLimit));
            RaisePropertyChanged(nameof(DecelerationLimit));
            RaisePropertyChanged(nameof(VelocityLimitPositive));
            RaisePropertyChanged(nameof(VelocityLimitNegative));
            RaisePropertyChanged(nameof(MaximumSpeed));
            RaisePropertyChanged(nameof(MaximumAcceleration));
            RaisePropertyChanged(nameof(MaximumDeceleration));
            RaisePropertyChanged(nameof(MaximumAccelerationJerk));
            RaisePropertyChanged(nameof(MaximumDecelerationJerk));

        }
    }
}
