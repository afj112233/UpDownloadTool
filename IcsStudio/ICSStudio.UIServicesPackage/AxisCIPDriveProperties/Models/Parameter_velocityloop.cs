using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Controls.ExtendedPropertyGrid.Attributes;
using ICSStudio.Gui.Utils;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Models
{
    internal partial class AxisCIPParameters
    {
        private readonly VelocityLoop _velocityLoop;

        #region Public Property

        private float _accelerationFeedforwardGain;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("%")]
        [ReadOnly(false)]
        [Category("Velocity Loop")]
        public float AccelerationFeedforwardGain
        {
            get
            {
                _accelerationFeedforwardGain = Convert.ToSingle(_currentAxis.CIPAxis.AccelerationFeedforwardGain);
                return _accelerationFeedforwardGain;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.AccelerationFeedforwardGain) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.AccelerationFeedforwardGain = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private SLATConfigurationType _SLATConfiguration;

        [Browsable(false)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Velocity Loop")]
        // ReSharper disable InconsistentNaming
        public SLATConfigurationType SLATConfiguration
        {
            get
            {
                _SLATConfiguration = (SLATConfigurationType) Convert.ToByte(_currentAxis.CIPAxis.SLATConfiguration);
                return _SLATConfiguration;
            }
            set
            {
                if (Convert.ToByte(_currentAxis.CIPAxis.SLATConfiguration) != (byte) value)
                {
                    _currentAxis.CIPAxis.SLATConfiguration = (byte) value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _SLATSetPoint;

        [Browsable(false)]
        [IsChanged(false)]
        [Unit("Position Units/s")]
        [ReadOnly(false)]
        [Category("Velocity Loop")]
        public float SLATSetPoint
        {
            get
            {
                _SLATSetPoint = Convert.ToSingle(_currentAxis.CIPAxis.SLATSetPoint);
                return _SLATSetPoint;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.SLATSetPoint) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.SLATSetPoint = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _SLATTimeDelay;

        [Browsable(false)]
        [IsChanged(false)]
        [Unit("s")]
        [ReadOnly(false)]
        [Category("Velocity Loop")]
        public float SLATTimeDelay
        {
            get
            {
                _SLATTimeDelay = Convert.ToSingle(_currentAxis.CIPAxis.SLATTimeDelay);
                return _SLATTimeDelay;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.SLATTimeDelay) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.SLATTimeDelay = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        // ReSharper restore InconsistentNaming
        private float _velocityDroop;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("(Position Units/s)/% Rated")]
        [ReadOnly(false)]
        [Category("Velocity Loop,Frequency Control")]
        public float VelocityDroop
        {
            get
            {
                _velocityDroop = Convert.ToSingle(_currentAxis.CIPAxis.VelocityDroop);
                return _velocityDroop;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.VelocityDroop) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.VelocityDroop = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _velocityErrorTolerance;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Position Units/s")]
        [ReadOnly(false)]
        [Category("Velocity Loop")]
        public float VelocityErrorTolerance
        {
            get
            {
                _velocityErrorTolerance = Convert.ToSingle(_currentAxis.CIPAxis.VelocityErrorTolerance);
                return _velocityErrorTolerance;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.VelocityErrorTolerance) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.VelocityErrorTolerance = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _velocityErrorToleranceTime;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("s")]
        [ReadOnly(false)]
        [Category("Velocity Loop")]
        public float VelocityErrorToleranceTime
        {
            get
            {
                _velocityErrorToleranceTime = Convert.ToSingle(_currentAxis.CIPAxis.VelocityErrorToleranceTime);
                return _velocityErrorToleranceTime;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.VelocityErrorToleranceTime) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.VelocityErrorToleranceTime = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _velocityIntegratorBandwidth;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Hz")]
        [ReadOnly(false)]
        [Category("Velocity Loop")]
        public float VelocityIntegratorBandwidth
        {
            get
            {
                _velocityIntegratorBandwidth = Convert.ToSingle(_currentAxis.CIPAxis.VelocityIntegratorBandwidth);
                return _velocityIntegratorBandwidth;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.VelocityIntegratorBandwidth) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.VelocityIntegratorBandwidth = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private BooleanType _velocityIntegratorHold;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Velocity Loop")]
        public BooleanType VelocityIntegratorHold
        {
            get
            {
                var bits = Convert.ToByte(_currentAxis.CIPAxis.VelocityIntegratorControl);
                _velocityIntegratorHold = (BooleanType) Convert.ToByte(
                    FlagsEnumHelper.ContainFlag(bits, IntegratorControlBitmap.IntegratorHoldEnable));
                return _velocityIntegratorHold;
            }
            set
            {
                var bits = Convert.ToByte(_currentAxis.CIPAxis.VelocityIntegratorControl);
                var velocityIntegratorHold = (BooleanType) Convert.ToByte(
                    FlagsEnumHelper.ContainFlag(bits, IntegratorControlBitmap.IntegratorHoldEnable));
                if (velocityIntegratorHold != value)
                {
                    FlagsEnumHelper.SelectFlag(
                        IntegratorControlBitmap.IntegratorHoldEnable,
                        Convert.ToBoolean(value),
                        ref bits);

                    _currentAxis.CIPAxis.VelocityIntegratorControl = bits;

                    _velocityLoop.UpdateVelocityIntegratorHoldIsChanged();

                    OnPropertyChanged();
                }
            }
        }

        private float _velocityLimitNegative;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Position Units/s")]
        [ReadOnly(false)]
        [Category("Velocity Loop,Frequency Control")]
        public float VelocityLimitNegative
        {
            get
            {
                _velocityLimitNegative = Convert.ToSingle(_currentAxis.CIPAxis.VelocityLimitNegative);
                return _velocityLimitNegative;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.VelocityLimitNegative) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.VelocityLimitNegative = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _velocityLimitPositive;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Position Units/s")]
        [ReadOnly(false)]
        [Category("Velocity Loop,Frequency Control")]
        public float VelocityLimitPositive
        {
            get
            {
                _velocityLimitPositive = Convert.ToSingle(_currentAxis.CIPAxis.VelocityLimitPositive);
                return _velocityLimitPositive;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.VelocityLimitPositive) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.VelocityLimitPositive = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _velocityLockTolerance;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Position Units/s")]
        [ReadOnly(false)]
        [Category("Velocity Loop")]
        public float VelocityLockTolerance
        {
            get
            {
                _velocityLockTolerance = Convert.ToSingle(_currentAxis.CIPAxis.VelocityLockTolerance);
                return _velocityLockTolerance;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.VelocityLockTolerance) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.VelocityLockTolerance = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _velocityLoopBandwidth;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Hz")]
        [ReadOnly(false)]
        [Category("Velocity Loop")]
        public float VelocityLoopBandwidth
        {
            get
            {
                _velocityLoopBandwidth = Convert.ToSingle(_currentAxis.CIPAxis.VelocityLoopBandwidth);
                return _velocityLoopBandwidth;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.VelocityLoopBandwidth) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.VelocityLoopBandwidth = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _velocityLowPassFilterBandwidth;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Hz")]
        [ReadOnly(false)]
        [Category("Velocity Loop")]
        public float VelocityLowPassFilterBandwidth
        {
            get
            {
                _velocityLowPassFilterBandwidth = Convert.ToSingle(_currentAxis.CIPAxis.VelocityLowPassFilterBandwidth);
                return _velocityLowPassFilterBandwidth;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.VelocityLowPassFilterBandwidth) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.VelocityLowPassFilterBandwidth = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _velocityNegativeFeedforwardGain;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("%")]
        [ReadOnly(false)]
        [Category("Velocity Loop")]
        public float VelocityNegativeFeedforwardGain
        {
            get
            {
                _velocityNegativeFeedforwardGain =
                    Convert.ToSingle(_currentAxis.CIPAxis.VelocityNegativeFeedforwardGain);
                return _velocityNegativeFeedforwardGain;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.VelocityNegativeFeedforwardGain) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.VelocityNegativeFeedforwardGain = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _velocityOffset;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Position Units/s")]
        [ReadOnly(false)]
        [Category("Velocity Loop")]
        // TODO(gjc):need edit,rm003 page 174
        public float VelocityOffset
        {
            get
            {
                _velocityOffset = Convert.ToSingle(_currentAxis.CIPAxis.VelocityOffset);
                return _velocityOffset;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.VelocityOffset) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.VelocityOffset = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }

        #endregion

        #region Enum

        private ObservableCollection<SLATConfigurationType> SLATConfigurationSource { get; set; }
            = new ObservableCollection<SLATConfigurationType>
            {
                SLATConfigurationType.SLATDisabled,
                SLATConfigurationType.SLATMinSpeedTorque,
                SLATConfigurationType.SLATMaxSpeedTorque
            };

        private ObservableCollection<BooleanType> VelocityIntegratorHoldSource { get; set; }
            = new ObservableCollection<BooleanType>
            {
                BooleanType.Disabled,
                BooleanType.Enabled
            };

        #endregion

        private class VelocityLoop
        {
            private readonly AxisCIPParameters _parameters;

            public VelocityLoop(AxisCIPParameters parameters)
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

                UpdateVelocityIntegratorHoldIsChanged();
            }

            public void UpdateVelocityIntegratorHoldIsChanged()
            {
                //VelocityIntegratorHold
                var bits1 = Convert.ToUInt16(_parameters._currentAxis.CIPAxis.VelocityIntegratorControl);
                var velocityIntegratorHold1 = FlagsEnumHelper.ContainFlag(bits1,
                    IntegratorControlBitmap.IntegratorHoldEnable);

                var bits2 = Convert.ToUInt16(_parameters._originalAxis.CIPAxis.VelocityIntegratorControl);
                var velocityIntegratorHold2 = FlagsEnumHelper.ContainFlag(bits2,
                    IntegratorControlBitmap.IntegratorHoldEnable);

                PropertySetting.SetPropertyIsChanged(_parameters, "VelocityIntegratorHold",
                    velocityIntegratorHold1 != velocityIntegratorHold2);
            }


            private BaseRule[] GetVisibilityRules(AxisCIPParameters parameters)
            {
                var ruleList = new List<BaseRule>
                {
                    new BaseRule {Name = "VelocityIntegratorBandwidth", Value = false},
                    new BaseRule {Name = "VelocityIntegratorHold", Value = false},
                    new BaseRule {Name = "VelocityLoopBandwidth", Value = false},
                    new BaseRule {Name = "VelocityOffset", Value = false},

                    new BaseRule {Name = "AccelerationFeedforwardGain", Value = false},
                    new BaseRule {Name = "VelocityDroop", Value = false},
                    new BaseRule {Name = "VelocityErrorTolerance", Value = false},
                    new BaseRule {Name = "VelocityErrorToleranceTime", Value = false},
                    new BaseRule {Name = "VelocityLimitNegative", Value = false},
                    new BaseRule {Name = "VelocityLimitPositive", Value = false},
                    new BaseRule {Name = "VelocityLockTolerance", Value = false},
                    new BaseRule {Name = "VelocityLowPassFilterBandwidth", Value = false},
                    new BaseRule {Name = "VelocityNegativeFeedforwardGain", Value = false},
                    new BaseRule {Name = "SLATConfiguration", Value = false},
                    new BaseRule {Name = "SLATSetPoint", Value = false},
                    new BaseRule {Name = "SLATTimeDelay", Value = false}
                };

                if (parameters._currentAxis.AssociatedModule != null)
                {
                    var axisConfiguration =
                        (AxisConfigurationType) Convert.ToByte(parameters._currentAxis.CIPAxis.AxisConfiguration);

                    CIPMotionDrive motionDrive =
                        parameters._currentAxis.AssociatedModule as CIPMotionDrive;

                    switch (axisConfiguration)
                    {
                        case AxisConfigurationType.FeedbackOnly:
                            break;
                        case AxisConfigurationType.FrequencyControl:
                            ruleList.Add(new BaseRule { Name = "VelocityDroop", Value = true });

                            if (motionDrive != null)
                            {
                                ruleList.Add(new BaseRule
                                {
                                    Name = "VelocityLimitPositive",
                                    Value = motionDrive.SupportAxisAttribute(axisConfiguration, "VelocityLimitPositive")
                                });

                                ruleList.Add(new BaseRule
                                {
                                    Name = "VelocityLimitNegative",
                                    Value = motionDrive.SupportAxisAttribute(axisConfiguration, "VelocityLimitNegative")
                                });
                            }

                            break;
                        case AxisConfigurationType.PositionLoop:
                            ruleList.Add(new BaseRule {Name = "VelocityIntegratorBandwidth", Value = true});
                            ruleList.Add(new BaseRule {Name = "VelocityIntegratorHold", Value = true});
                            ruleList.Add(new BaseRule {Name = "VelocityLoopBandwidth", Value = true});
                            ruleList.Add(new BaseRule {Name = "VelocityOffset", Value = true});

                            ruleList.Add(new BaseRule {Name = "AccelerationFeedforwardGain", Value = true});
                            ruleList.Add(new BaseRule {Name = "VelocityDroop", Value = true});
                            ruleList.Add(new BaseRule {Name = "VelocityErrorTolerance", Value = true});
                            ruleList.Add(new BaseRule {Name = "VelocityErrorToleranceTime", Value = true});
                            ruleList.Add(new BaseRule {Name = "VelocityLimitNegative", Value = true});
                            ruleList.Add(new BaseRule {Name = "VelocityLimitPositive", Value = true});
                            ruleList.Add(new BaseRule {Name = "VelocityLockTolerance", Value = true});
                            ruleList.Add(new BaseRule {Name = "VelocityLowPassFilterBandwidth", Value = true});
                            ruleList.Add(new BaseRule {Name = "VelocityNegativeFeedforwardGain", Value = true});
                            break;
                        case AxisConfigurationType.VelocityLoop:
                            ruleList.Add(new BaseRule {Name = "VelocityIntegratorBandwidth", Value = true});
                            ruleList.Add(new BaseRule {Name = "VelocityIntegratorHold", Value = true});
                            ruleList.Add(new BaseRule {Name = "VelocityLoopBandwidth", Value = true});
                            ruleList.Add(new BaseRule {Name = "VelocityOffset", Value = true});

                            ruleList.Add(new BaseRule {Name = "AccelerationFeedforwardGain", Value = true});
                            ruleList.Add(new BaseRule {Name = "VelocityDroop", Value = true});
                            ruleList.Add(new BaseRule {Name = "VelocityErrorTolerance", Value = true});
                            ruleList.Add(new BaseRule {Name = "VelocityErrorToleranceTime", Value = true});
                            ruleList.Add(new BaseRule {Name = "VelocityLimitNegative", Value = true});
                            ruleList.Add(new BaseRule {Name = "VelocityLimitPositive", Value = true});
                            ruleList.Add(new BaseRule {Name = "VelocityLockTolerance", Value = true});
                            ruleList.Add(new BaseRule {Name = "VelocityLowPassFilterBandwidth", Value = true});
                            ruleList.Add(new BaseRule {Name = "VelocityNegativeFeedforwardGain", Value = true});
                            //////
                            ruleList.Add(new BaseRule {Name = "SLATConfiguration", Value = true});
                            ruleList.Add(new BaseRule {Name = "SLATSetPoint", Value = true});
                            ruleList.Add(new BaseRule {Name = "SLATTimeDelay", Value = true});

                            break;
                        case AxisConfigurationType.TorqueLoop:
                            ruleList.Add(new BaseRule {Name = "VelocityIntegratorBandwidth", Value = true});
                            ruleList.Add(new BaseRule {Name = "VelocityIntegratorHold", Value = true});
                            ruleList.Add(new BaseRule {Name = "VelocityLoopBandwidth", Value = true});
                            ruleList.Add(new BaseRule {Name = "VelocityOffset", Value = true});
                            break;
                        case AxisConfigurationType.ConverterOnly:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                return ruleList.ToArray();
            }

            private BaseRule[] GetReadonlyRules(AxisCIPParameters parameters)
            {
                var ruleList = new List<BaseRule>();

                if (parameters._parentViewModel.IsPowerStructureEnabled || parameters._parentViewModel.IsHardRunMode)
                {
                    ruleList.Add(new BaseRule {Name = "VelocityIntegratorBandwidth", Value = true});
                    ruleList.Add(new BaseRule {Name = "VelocityIntegratorHold", Value = true});
                    ruleList.Add(new BaseRule {Name = "VelocityLoopBandwidth", Value = true});
                    ruleList.Add(new BaseRule {Name = "VelocityOffset", Value = true});
                    ruleList.Add(new BaseRule {Name = "AccelerationFeedforwardGain", Value = true});
                    ruleList.Add(new BaseRule {Name = "VelocityDroop", Value = true});
                    ruleList.Add(new BaseRule {Name = "VelocityErrorTolerance", Value = true});
                    ruleList.Add(new BaseRule {Name = "VelocityErrorToleranceTime", Value = true});
                    ruleList.Add(new BaseRule {Name = "VelocityLimitNegative", Value = true});
                    ruleList.Add(new BaseRule {Name = "VelocityLimitPositive", Value = true});
                    ruleList.Add(new BaseRule {Name = "VelocityLockTolerance", Value = true});
                    ruleList.Add(new BaseRule {Name = "VelocityLowPassFilterBandwidth", Value = true});
                    ruleList.Add(new BaseRule {Name = "VelocityNegativeFeedforwardGain", Value = true});
                    ruleList.Add(new BaseRule {Name = "SLATConfiguration", Value = true});
                    ruleList.Add(new BaseRule {Name = "SLATSetPoint", Value = true});
                    ruleList.Add(new BaseRule {Name = "SLATTimeDelay", Value = true});
                }
                else
                {
                    ruleList.Add(new BaseRule {Name = "VelocityIntegratorBandwidth", Value = false});
                    ruleList.Add(new BaseRule {Name = "VelocityIntegratorHold", Value = false});
                    ruleList.Add(new BaseRule {Name = "VelocityLoopBandwidth", Value = false});
                    ruleList.Add(new BaseRule {Name = "VelocityOffset", Value = false});
                    ruleList.Add(new BaseRule {Name = "AccelerationFeedforwardGain", Value = false});
                    ruleList.Add(new BaseRule {Name = "VelocityDroop", Value = false});
                    ruleList.Add(new BaseRule {Name = "VelocityErrorTolerance", Value = false});
                    ruleList.Add(new BaseRule {Name = "VelocityErrorToleranceTime", Value = false});
                    ruleList.Add(new BaseRule {Name = "VelocityLimitNegative", Value = false});
                    ruleList.Add(new BaseRule {Name = "VelocityLimitPositive", Value = false});
                    ruleList.Add(new BaseRule {Name = "VelocityLockTolerance", Value = false});
                    ruleList.Add(new BaseRule {Name = "VelocityLowPassFilterBandwidth", Value = false});
                    ruleList.Add(new BaseRule {Name = "VelocityNegativeFeedforwardGain", Value = false});
                    ruleList.Add(new BaseRule {Name = "SLATConfiguration", Value = false});
                    ruleList.Add(new BaseRule {Name = "SLATSetPoint", Value = false});
                    ruleList.Add(new BaseRule {Name = "SLATTimeDelay", Value = false});
                }

                return ruleList.ToArray();
            }


            public string[] GetCompareProperties()
            {
                string[] compareProperties =
                {
                    "AccelerationFeedforwardGain", "VelocityDroop",
                    "VelocityErrorTolerance", "VelocityErrorToleranceTime",
                    "VelocityIntegratorBandwidth", "VelocityLimitNegative",
                    "VelocityLimitPositive", "VelocityLockTolerance",
                    "VelocityLoopBandwidth", "VelocityLowPassFilterBandwidth",
                    "VelocityNegativeFeedforwardGain",
                    "VelocityOffset",
                    "SLATConfiguration", "SLATSetPoint", "SLATTimeDelay"
                };
                return compareProperties;
            }

            public string[] GetPeriodicRefreshProperties()
            {
                string[] periodicRefreshProperties = new[]
                {
                    "VelocityErrorTolerance", "VelocityLimitPositive", "VelocityLimitNegative"
                };

                return periodicRefreshProperties;
            }
        }
    }
}
