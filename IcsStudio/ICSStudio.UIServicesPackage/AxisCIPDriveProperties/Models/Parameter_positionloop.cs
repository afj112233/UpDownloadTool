using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Controls.ExtendedPropertyGrid.Attributes;
using ICSStudio.Gui.Utils;


namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Models
{
    internal partial class AxisCIPParameters
    {
        private readonly PositionLoop _positionLoop;

        #region Public Property

        private float _positionErrorTolerance;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Position Units")]
        [ReadOnly(false)]
        [Category("Position Loop")]
        public float PositionErrorTolerance
        {
            get
            {
                _positionErrorTolerance = Convert.ToSingle(_currentAxis.CIPAxis.PositionErrorTolerance);
                return _positionErrorTolerance;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.PositionErrorTolerance) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.PositionErrorTolerance = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _positionErrorToleranceTime;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("s")]
        [ReadOnly(false)]
        [Category("Position Loop")]
        public float PositionErrorToleranceTime
        {
            get
            {
                _positionErrorToleranceTime = Convert.ToSingle(_currentAxis.CIPAxis.PositionErrorToleranceTime);
                return _positionErrorToleranceTime;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.PositionErrorToleranceTime) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.PositionErrorToleranceTime = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _positionIntegratorBandwidth;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Hz")]
        [ReadOnly(false)]
        [Category("Position Loop")]
        public float PositionIntegratorBandwidth
        {
            get
            {
                _positionIntegratorBandwidth = Convert.ToSingle(_currentAxis.CIPAxis.PositionIntegratorBandwidth);
                return _positionIntegratorBandwidth;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.PositionIntegratorBandwidth) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.PositionIntegratorBandwidth = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private BooleanType _positionIntegratorHold;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Position Loop")]
        public BooleanType PositionIntegratorHold
        {
            get
            {
                var bits = Convert.ToByte(_currentAxis.CIPAxis.PositionIntegratorControl);
                _positionIntegratorHold =
                    (BooleanType) Convert.ToByte(
                        FlagsEnumHelper.ContainFlag(bits, IntegratorControlBitmap.IntegratorHoldEnable));
                return _positionIntegratorHold;
            }
            set
            {
                var bits = Convert.ToByte(_currentAxis.CIPAxis.PositionIntegratorControl);
                var positionIntegratorHold =
                    (BooleanType) Convert.ToByte(
                        FlagsEnumHelper.ContainFlag(bits, IntegratorControlBitmap.IntegratorHoldEnable));
                if (positionIntegratorHold != value)
                {
                    FlagsEnumHelper.SelectFlag(
                        IntegratorControlBitmap.IntegratorHoldEnable,
                        Convert.ToBoolean(value),
                        ref bits);

                    _currentAxis.CIPAxis.PositionIntegratorControl = bits;

                    _positionLoop.UpdatePositionIntegratorHoldIsChanged();

                    OnPropertyChanged();
                }


            }
        }

        private float _positionLeadLagFilterBandwidth;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Hz")]
        [ReadOnly(false)]
        [Category("Position Loop")]
        public float PositionLeadLagFilterBandwidth
        {
            get
            {
                _positionLeadLagFilterBandwidth = Convert.ToSingle(_currentAxis.CIPAxis.PositionLeadLagFilterBandwidth);
                return _positionLeadLagFilterBandwidth;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.PositionLeadLagFilterBandwidth) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.PositionLeadLagFilterBandwidth = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _positionLeadLagFilterGain;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Position Loop")]
        public float PositionLeadLagFilterGain
        {
            get
            {
                _positionLeadLagFilterGain = Convert.ToSingle(_currentAxis.CIPAxis.PositionLeadLagFilterGain);
                return _positionLeadLagFilterGain;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.PositionLeadLagFilterGain) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.PositionLeadLagFilterGain = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _positionLockTolerance;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Unit("Position Units")]
        [Category("Position Loop")]
        public float PositionLockTolerance
        {
            get
            {
                _positionLockTolerance = Convert.ToSingle(_currentAxis.CIPAxis.PositionLockTolerance);
                return _positionLockTolerance;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.PositionLockTolerance) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.PositionLockTolerance = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _positionLoopBandwidth;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Hz")]
        [ReadOnly(false)]
        [Category("Position Loop")]
        public float PositionLoopBandwidth
        {
            get
            {
                _positionLoopBandwidth = Convert.ToSingle(_currentAxis.CIPAxis.PositionLoopBandwidth);
                return _positionLoopBandwidth;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.PositionLoopBandwidth) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.PositionLoopBandwidth = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _velocityFeedforwardGain;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("%")]
        [ReadOnly(false)]
        [Category("Position Loop")]
        public float VelocityFeedforwardGain
        {
            get
            {
                _velocityFeedforwardGain = Convert.ToSingle(_currentAxis.CIPAxis.VelocityFeedforwardGain);
                return _velocityFeedforwardGain;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.VelocityFeedforwardGain) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.VelocityFeedforwardGain = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Enum

        private ObservableCollection<BooleanType> PositionIntegratorHoldSource { get; set; }
            = new ObservableCollection<BooleanType>
            {
                BooleanType.Disabled,
                BooleanType.Enabled
            };

        #endregion

        private class PositionLoop
        {
            private readonly AxisCIPParameters _parameters;

            public PositionLoop(AxisCIPParameters parameters)
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

                UpdatePositionIntegratorHoldIsChanged();
            }

            public void UpdatePositionIntegratorHoldIsChanged()
            {
                //PositionIntegratorHold
                var bits1 = Convert.ToUInt16(_parameters._currentAxis.CIPAxis.PositionIntegratorControl);
                var positionIntegratorHold1 = FlagsEnumHelper.ContainFlag(bits1,
                    IntegratorControlBitmap.IntegratorHoldEnable);

                var bits2 = Convert.ToUInt16(_parameters._originalAxis.CIPAxis.PositionIntegratorControl);
                var positionIntegratorHold2 = FlagsEnumHelper.ContainFlag(bits2,
                    IntegratorControlBitmap.IntegratorHoldEnable);

                PropertySetting.SetPropertyIsChanged(_parameters, "PositionIntegratorHold",
                    positionIntegratorHold1 != positionIntegratorHold2);

            }

            private BaseRule[] GetVisibilityRules(AxisCIPParameters parameters)
            {
                var ruleList = new List<BaseRule>
                {
                    new BaseRule {Name = "PositionErrorTolerance", Value = false},
                    new BaseRule {Name = "PositionErrorToleranceTime", Value = false},
                    new BaseRule {Name = "PositionIntegratorBandwidth", Value = false},
                    new BaseRule {Name = "PositionIntegratorHold", Value = false},
                    new BaseRule {Name = "PositionLeadLagFilterBandwidth", Value = false},
                    new BaseRule {Name = "PositionLeadLagFilterGain", Value = false},
                    new BaseRule {Name = "PositionLockTolerance", Value = false},
                    new BaseRule {Name = "PositionLoopBandwidth", Value = false},
                    new BaseRule {Name = "VelocityFeedforwardGain", Value = false}
                };

                // axis configuration
                var axisConfiguration =
                    (AxisConfigurationType) Convert.ToByte(parameters._currentAxis.CIPAxis.AxisConfiguration);

                if (axisConfiguration == AxisConfigurationType.PositionLoop)
                {
                    ruleList.Add(new BaseRule {Name = "PositionErrorTolerance", Value = true});
                    ruleList.Add(new BaseRule {Name = "PositionErrorToleranceTime", Value = true});
                    ruleList.Add(new BaseRule {Name = "PositionIntegratorBandwidth", Value = true});
                    ruleList.Add(new BaseRule {Name = "PositionIntegratorHold", Value = true});
                    ruleList.Add(new BaseRule {Name = "PositionLeadLagFilterBandwidth", Value = true});
                    ruleList.Add(new BaseRule {Name = "PositionLeadLagFilterGain", Value = true});
                    ruleList.Add(new BaseRule {Name = "PositionLockTolerance", Value = true});
                    ruleList.Add(new BaseRule {Name = "PositionLoopBandwidth", Value = true});
                    ruleList.Add(new BaseRule {Name = "VelocityFeedforwardGain", Value = true});
                }


                return ruleList.ToArray();
            }

            private BaseRule[] GetReadonlyRules(AxisCIPParameters parameters)
            {
                var ruleList = new List<BaseRule>();

                if (parameters._parentViewModel.IsPowerStructureEnabled || parameters._parentViewModel.IsHardRunMode)
                {
                    ruleList.Add(new BaseRule {Name = "PositionErrorTolerance", Value = true});
                    ruleList.Add(new BaseRule {Name = "PositionErrorToleranceTime", Value = true});
                    ruleList.Add(new BaseRule {Name = "PositionIntegratorBandwidth", Value = true});
                    ruleList.Add(new BaseRule {Name = "PositionIntegratorHold", Value = true});
                    ruleList.Add(new BaseRule {Name = "PositionLeadLagFilterBandwidth", Value = true});
                    ruleList.Add(new BaseRule {Name = "PositionLeadLagFilterGain", Value = true});
                    ruleList.Add(new BaseRule {Name = "PositionLockTolerance", Value = true});
                    ruleList.Add(new BaseRule {Name = "PositionLoopBandwidth", Value = true});
                    ruleList.Add(new BaseRule {Name = "VelocityFeedforwardGain", Value = true});
                }
                else
                {
                    ruleList.Add(new BaseRule {Name = "PositionErrorTolerance", Value = false});
                    ruleList.Add(new BaseRule {Name = "PositionErrorToleranceTime", Value = false});
                    ruleList.Add(new BaseRule {Name = "PositionIntegratorBandwidth", Value = false});
                    ruleList.Add(new BaseRule {Name = "PositionIntegratorHold", Value = false});
                    ruleList.Add(new BaseRule {Name = "PositionLeadLagFilterBandwidth", Value = false});
                    ruleList.Add(new BaseRule {Name = "PositionLeadLagFilterGain", Value = false});
                    ruleList.Add(new BaseRule {Name = "PositionLockTolerance", Value = false});
                    ruleList.Add(new BaseRule {Name = "PositionLoopBandwidth", Value = false});
                    ruleList.Add(new BaseRule {Name = "VelocityFeedforwardGain", Value = false});
                }

                return ruleList.ToArray();
            }


            public string[] GetCompareProperties()
            {
                string[] compareProperties =
                {
                    "PositionErrorTolerance", "PositionErrorToleranceTime",
                    "PositionIntegratorBandwidth",
                    "PositionLeadLagFilterBandwidth", "PositionLeadLagFilterGain",
                    "PositionLockTolerance", "PositionLoopBandwidth",
                    "VelocityFeedforwardGain"
                };
                return compareProperties;
            }

            public string[] GetPeriodicRefreshProperties()
            {
                string[] periodicRefreshProperties = new[]
                {
                    "PositionLoopBandwidth",
                    "PositionIntegratorBandwidth",
                    "PositionErrorTolerance",
                };

                return periodicRefreshProperties;
            }
        }
    }
}
