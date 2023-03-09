using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Controls.ExtendedPropertyGrid.Attributes;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Models
{
    internal partial class AxisCIPParameters
    {
        private readonly Compliance _compliance;

        #region Public Property

        private float _torqueLeadLagFilterBandwidth;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Hz")]
        [ReadOnly(false)]
        [Category("Compliance")]
        public float TorqueLeadLagFilterBandwidth
        {
            get
            {
                _torqueLeadLagFilterBandwidth = Convert.ToSingle(_currentAxis.CIPAxis.TorqueLeadLagFilterBandwidth);
                return _torqueLeadLagFilterBandwidth;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.TorqueLeadLagFilterBandwidth) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.TorqueLeadLagFilterBandwidth = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _torqueLeadLagFilterGain;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Compliance")]
        public float TorqueLeadLagFilterGain
        {
            get
            {
                _torqueLeadLagFilterGain = Convert.ToSingle(_currentAxis.CIPAxis.TorqueLeadLagFilterGain);
                return _torqueLeadLagFilterGain;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.TorqueLeadLagFilterGain) - value
                    ) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.TorqueLeadLagFilterGain = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _torqueLowPassFilterBandwidth;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Hz")]
        [ReadOnly(false)]
        [Category("Compliance")]
        public float TorqueLowPassFilterBandwidth
        {
            get
            {
                _torqueLowPassFilterBandwidth = Convert.ToSingle(_currentAxis.CIPAxis.TorqueLowPassFilterBandwidth);
                return _torqueLowPassFilterBandwidth;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.TorqueLowPassFilterBandwidth) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.TorqueLowPassFilterBandwidth = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _torqueNotchFilterFrequency;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Hz")]
        [ReadOnly(false)]
        [Category("Compliance")]
        public float TorqueNotchFilterFrequency
        {
            get
            {
                _torqueNotchFilterFrequency = Convert.ToSingle(_currentAxis.CIPAxis.TorqueNotchFilterFrequency);
                return _torqueNotchFilterFrequency;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.TorqueNotchFilterFrequency) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.TorqueNotchFilterFrequency = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private AdaptiveTuningConfigurationType _adaptiveTuningConfiguration;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Compliance")]
        public AdaptiveTuningConfigurationType AdaptiveTuningConfiguration
        {
            get
            {
                _adaptiveTuningConfiguration =
                    (AdaptiveTuningConfigurationType)Convert.ToByte(_currentAxis.CIPAxis.AdaptiveTuningConfiguration);
                return _adaptiveTuningConfiguration;
            }
            set
            {
                var adaptiveTuningConfiguration =
                    (AdaptiveTuningConfigurationType)Convert.ToByte(_currentAxis.CIPAxis.AdaptiveTuningConfiguration);
                if (adaptiveTuningConfiguration != value)
                {
                    _currentAxis.CIPAxis.AdaptiveTuningConfiguration = (byte)value;

                    //_compliance.UpdateReadonly();

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }

        private float _torqueNotchFilterHighFrequencyLimit;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Hz")]
        [ReadOnly(false)]
        [Category("Compliance")]
        public float TorqueNotchFilterHighFrequencyLimit
        {
            get
            {
                _torqueNotchFilterHighFrequencyLimit =
                    Convert.ToSingle(_currentAxis.CIPAxis.TorqueNotchFilterHighFrequencyLimit);
                return _torqueNotchFilterHighFrequencyLimit;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.TorqueNotchFilterHighFrequencyLimit) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.TorqueNotchFilterHighFrequencyLimit = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }

        private float _torqueNotchFilterLowFrequencyLimit;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Hz")]
        [ReadOnly(false)]
        [Category("Compliance")]
        public float TorqueNotchFilterLowFrequencyLimit
        {
            get
            {
                _torqueNotchFilterLowFrequencyLimit =
                    Convert.ToSingle(_currentAxis.CIPAxis.TorqueNotchFilterLowFrequencyLimit);
                return _torqueNotchFilterLowFrequencyLimit;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.TorqueNotchFilterLowFrequencyLimit) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.TorqueNotchFilterLowFrequencyLimit = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _torqueNotchFilterTuningThreshold;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("% Motor Rated")]
        [ReadOnly(false)]
        [Category("Compliance")]
        public float TorqueNotchFilterTuningThreshold
        {
            get
            {
                _torqueNotchFilterTuningThreshold =
                    Convert.ToSingle(_currentAxis.CIPAxis.TorqueNotchFilterTuningThreshold);
                return _torqueNotchFilterTuningThreshold;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.TorqueNotchFilterTuningThreshold) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.TorqueNotchFilterTuningThreshold = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Enum

        private ObservableCollection<AdaptiveTuningConfigurationType> AdaptiveTuningConfigurationSource { get; set; }
            = new ObservableCollection<AdaptiveTuningConfigurationType>
            {
                AdaptiveTuningConfigurationType.Disabled,
                AdaptiveTuningConfigurationType.TrackingNotch,
                AdaptiveTuningConfigurationType.GainStabilization,
                AdaptiveTuningConfigurationType.TrackingNotchAndGainStabilization
            };

        #endregion

        private class Compliance
        {
            private readonly AxisCIPParameters _parameters;

            public Compliance(AxisCIPParameters parameters)
            {
                _parameters = parameters;
            }

            public void Refresh()
            {
                UpdateVisibility();
                UpdateReadonly();
                UpdateIsChanged();
            }

            public void UpdateVisibility()
            {
                BaseRule.ApplyVisibilityRule(_parameters, GetVisibilityRules(_parameters));
            }

            public void UpdateReadonly()
            {
                BaseRule.ApplyReadonlyRule(_parameters, GetReadonlyRules(_parameters));
            }

            public void UpdateIsChanged()
            {
                var compareProperties = GetCompareProperties();

                foreach (var propertyName in compareProperties)
                {
                    var isChange =
                        !CipAttributeHelper.EqualByAttributeName(
                            _parameters._currentAxis.CIPAxis,
                            _parameters._originalAxis.CIPAxis,
                            propertyName);

                    PropertySetting.SetPropertyIsChanged(_parameters, propertyName, isChange);
                }
            }

            private BaseRule[] GetVisibilityRules(AxisCIPParameters parameters)
            {
                var ruleList = new List<BaseRule>
                {
                    new BaseRule { Name = "TorqueLeadLagFilterBandwidth", Value = false },
                    new BaseRule { Name = "TorqueLeadLagFilterGain", Value = false },
                    new BaseRule { Name = "TorqueLowPassFilterBandwidth", Value = false },
                    new BaseRule { Name = "TorqueNotchFilterFrequency", Value = false },
                    new BaseRule { Name = "AdaptiveTuningConfiguration", Value = false },
                    new BaseRule { Name = "TorqueNotchFilterHighFrequencyLimit", Value = false },
                    new BaseRule { Name = "TorqueNotchFilterLowFrequencyLimit", Value = false },
                    new BaseRule { Name = "TorqueNotchFilterTuningThreshold", Value = false }
                };

                var axisConfiguration =
                    (AxisConfigurationType)Convert.ToByte(parameters._currentAxis.CIPAxis.AxisConfiguration);
                if ((axisConfiguration == AxisConfigurationType.PositionLoop)
                    || (axisConfiguration == AxisConfigurationType.VelocityLoop)
                    || (axisConfiguration == AxisConfigurationType.TorqueLoop))
                {
                    ruleList.Add(new BaseRule { Name = "TorqueLeadLagFilterBandwidth", Value = true });
                    ruleList.Add(new BaseRule { Name = "TorqueLeadLagFilterGain", Value = true });
                    ruleList.Add(new BaseRule { Name = "TorqueLowPassFilterBandwidth", Value = true });
                    ruleList.Add(new BaseRule { Name = "TorqueNotchFilterFrequency", Value = true });
                    ruleList.Add(new BaseRule { Name = "AdaptiveTuningConfiguration", Value = true });
                    ruleList.Add(new BaseRule { Name = "TorqueNotchFilterHighFrequencyLimit", Value = true });
                    ruleList.Add(new BaseRule { Name = "TorqueNotchFilterLowFrequencyLimit", Value = true });
                    ruleList.Add(new BaseRule { Name = "TorqueNotchFilterTuningThreshold", Value = true });
                }

                return ruleList.ToArray();
            }

            private BaseRule[] GetReadonlyRules(AxisCIPParameters parameters)
            {
                var ruleList = new List<BaseRule>();

                if (parameters._parentViewModel.IsPowerStructureEnabled || parameters._parentViewModel.IsHardRunMode)
                {
                    ruleList.Add(new BaseRule { Name = "TorqueLeadLagFilterBandwidth", Value = true });
                    ruleList.Add(new BaseRule { Name = "TorqueLeadLagFilterGain", Value = true });
                    ruleList.Add(new BaseRule { Name = "TorqueLowPassFilterBandwidth", Value = true });
                    ruleList.Add(new BaseRule { Name = "TorqueNotchFilterFrequency", Value = true });
                    ruleList.Add(new BaseRule { Name = "AdaptiveTuningConfiguration", Value = true });
                    ruleList.Add(new BaseRule { Name = "TorqueNotchFilterHighFrequencyLimit", Value = true });
                    ruleList.Add(new BaseRule { Name = "TorqueNotchFilterLowFrequencyLimit", Value = true });
                    ruleList.Add(new BaseRule { Name = "TorqueNotchFilterTuningThreshold", Value = true });
                }
                else
                {
                    ruleList.Add(new BaseRule { Name = "TorqueLeadLagFilterBandwidth", Value = false });
                    ruleList.Add(new BaseRule { Name = "TorqueLeadLagFilterGain", Value = false });
                    ruleList.Add(new BaseRule { Name = "TorqueLowPassFilterBandwidth", Value = false });
                    ruleList.Add(new BaseRule { Name = "TorqueNotchFilterFrequency", Value = false });
                    ruleList.Add(new BaseRule { Name = "AdaptiveTuningConfiguration", Value = false });
                    ruleList.Add(new BaseRule { Name = "TorqueNotchFilterHighFrequencyLimit", Value = false });
                    ruleList.Add(new BaseRule { Name = "TorqueNotchFilterLowFrequencyLimit", Value = false });
                    ruleList.Add(new BaseRule { Name = "TorqueNotchFilterTuningThreshold", Value = false });

                    //var adaptiveTuningConfiguration =
                    //    (AdaptiveTuningConfigurationType)
                    //    Convert.ToByte(parameters._currentAxis.CIPAxis.AdaptiveTuningConfiguration);
                    //if (adaptiveTuningConfiguration == AdaptiveTuningConfigurationType.Disabled)
                    //{
                    //    ruleList.Add(new BaseRule {Name = "TorqueNotchFilterHighFrequencyLimit", Value = true});
                    //    ruleList.Add(new BaseRule {Name = "TorqueNotchFilterLowFrequencyLimit", Value = true});
                    //    ruleList.Add(new BaseRule {Name = "TorqueNotchFilterTuningThreshold", Value = true});
                    //}
                }

                return ruleList.ToArray();
            }

            public string[] GetCompareProperties()
            {
                string[] compareProperties =
                {
                    "TorqueLeadLagFilterBandwidth", "TorqueLeadLagFilterGain",
                    "TorqueLowPassFilterBandwidth", "TorqueNotchFilterFrequency",
                    "AdaptiveTuningConfiguration", "TorqueNotchFilterHighFrequencyLimit",
                    "TorqueNotchFilterLowFrequencyLimit", "TorqueNotchFilterTuningThreshold"
                };

                return compareProperties;
            }

            public string[] GetPeriodicRefreshProperties()
            {
                string[] periodicRefreshProperties = new[]
                {
                    "TorqueLeadLagFilterBandwidth",
                    "TorqueLeadLagFilterGain",
                    "TorqueNotchFilterFrequency",
                    "TorqueNotchFilterHighFrequencyLimit",
                    "TorqueNotchFilterLowFrequencyLimit",
                    "TorqueNotchFilterTuningThreshold"
                };

                return periodicRefreshProperties;
            }
        }
    }
}
