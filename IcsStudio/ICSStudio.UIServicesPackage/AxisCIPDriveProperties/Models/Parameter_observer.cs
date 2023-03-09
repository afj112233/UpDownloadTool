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
        private readonly Observer _observer;

        #region Public Property

        private float _loadObserverBandwidth;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Unit("Hz")]
        [Category("Observer")]
        public float LoadObserverBandwidth
        {
            get
            {
                _loadObserverBandwidth = Convert.ToSingle(_currentAxis.CIPAxis.LoadObserverBandwidth);
                return _loadObserverBandwidth;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.LoadObserverBandwidth) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.LoadObserverBandwidth = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private LoadObserverConfigurationType _loadObserverConfiguration;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Observer")]
        public LoadObserverConfigurationType LoadObserverConfiguration
        {
            get
            {
                _loadObserverConfiguration =
                    (LoadObserverConfigurationType) Convert.ToByte(_currentAxis.CIPAxis.LoadObserverConfiguration);
                return _loadObserverConfiguration;
            }
            set
            {
                var loadObserverConfiguration =
                    (LoadObserverConfigurationType) Convert.ToByte(_currentAxis.CIPAxis.LoadObserverConfiguration);
                if (loadObserverConfiguration != value)
                {
                    _currentAxis.CIPAxis.LoadObserverConfiguration = (byte) value;

                    _observer.UpdateReadonly();

                    UpdateIsChanged();
                    OnPropertyChanged();

                }

            }
        }

        private float _loadObserverFeedbackGain;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Observer")]
        public float LoadObserverFeedbackGain
        {
            get
            {
                _loadObserverFeedbackGain = Convert.ToSingle(_currentAxis.CIPAxis.LoadObserverFeedbackGain);
                return _loadObserverFeedbackGain;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.LoadObserverFeedbackGain) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.LoadObserverFeedbackGain = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _loadObserverIntegratorBandwidth;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Unit("Hz")]
        [Category("Observer")]
        public float LoadObserverIntegratorBandwidth
        {
            get
            {
                _loadObserverIntegratorBandwidth =
                    Convert.ToSingle(_currentAxis.CIPAxis.LoadObserverIntegratorBandwidth);
                return _loadObserverIntegratorBandwidth;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.LoadObserverIntegratorBandwidth) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.LoadObserverIntegratorBandwidth = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Enum

        private ObservableCollection<LoadObserverConfigurationType> LoadObserverConfigurationSource { get; set; }
            = new ObservableCollection<LoadObserverConfigurationType>
            {
                LoadObserverConfigurationType.Disabled,
                LoadObserverConfigurationType.LoadObserverOnly,
                LoadObserverConfigurationType.LoadObserverWithVelocityEstimate,
                LoadObserverConfigurationType.VelocityEstimateOnly
            };

        #endregion

        private class Observer
        {
            private readonly AxisCIPParameters _parameters;

            public Observer(AxisCIPParameters parameters)
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
                    new BaseRule {Name = "LoadObserverBandwidth", Value = false},
                    new BaseRule {Name = "LoadObserverConfiguration", Value = false},
                    new BaseRule {Name = "LoadObserverFeedbackGain", Value = false},
                    new BaseRule {Name = "LoadObserverIntegratorBandwidth", Value = false}
                };

                // AxisConfiguration
                var axisConfiguration =
                    (AxisConfigurationType) Convert.ToByte(parameters._currentAxis.CIPAxis.AxisConfiguration);

                if ((axisConfiguration == AxisConfigurationType.PositionLoop)
                    || (axisConfiguration == AxisConfigurationType.VelocityLoop)
                    || (axisConfiguration == AxisConfigurationType.TorqueLoop))
                {
                    ruleList.Add(new BaseRule {Name = "LoadObserverBandwidth", Value = true});
                    ruleList.Add(new BaseRule {Name = "LoadObserverConfiguration", Value = true});
                    ruleList.Add(new BaseRule {Name = "LoadObserverFeedbackGain", Value = true});
                    ruleList.Add(new BaseRule {Name = "LoadObserverIntegratorBandwidth", Value = true});
                }

                return ruleList.ToArray();
            }

            private BaseRule[] GetReadonlyRules(AxisCIPParameters parameters)
            {
                var ruleList = new List<BaseRule>
                {
                    new BaseRule {Name = "LoadObserverBandwidth", Value = true},
                    new BaseRule {Name = "LoadObserverConfiguration", Value = true},
                    new BaseRule {Name = "LoadObserverFeedbackGain", Value = true},
                    new BaseRule {Name = "LoadObserverIntegratorBandwidth", Value = true}
                };

                if (parameters._parentViewModel.IsPowerStructureEnabled || parameters._parentViewModel.IsHardRunMode)
                    return ruleList.ToArray();

                //
                ruleList.Add(new BaseRule {Name = "LoadObserverConfiguration", Value = false});

                var loadObserverConfiguration =
                    (LoadObserverConfigurationType)
                    Convert.ToByte(parameters._currentAxis.CIPAxis.LoadObserverConfiguration);

                if (loadObserverConfiguration != LoadObserverConfigurationType.Disabled)
                {
                    ruleList.Add(new BaseRule {Name = "LoadObserverBandwidth", Value = false});
                    ruleList.Add(new BaseRule {Name = "LoadObserverFeedbackGain", Value = false});
                    ruleList.Add(new BaseRule {Name = "LoadObserverIntegratorBandwidth", Value = false});
                }

                return ruleList.ToArray();
            }


            public string[] GetCompareProperties()
            {
                string[] compareProperties =
                {
                    "LoadObserverBandwidth", "LoadObserverConfiguration",
                    "LoadObserverFeedbackGain", "LoadObserverIntegratorBandwidth"
                };
                return compareProperties;
            }
        }
    }
}
