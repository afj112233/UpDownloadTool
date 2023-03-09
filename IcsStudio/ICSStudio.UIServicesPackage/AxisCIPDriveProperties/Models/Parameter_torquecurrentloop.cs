using System;
using System.Collections.Generic;
using System.ComponentModel;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Controls.ExtendedPropertyGrid.Attributes;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Models
{
    internal partial class AxisCIPParameters
    {
        private readonly TorqueCurrentLoop _torqueCurrentLoop;

        #region Public Property

        private float _currentVectorLimit;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("% Motor Rated")]
        [ReadOnly(false)]
        [Category("Torque/Current Loop")]
        public float CurrentVectorLimit
        {
            get
            {
                _currentVectorLimit = Convert.ToSingle(_currentAxis.CIPAxis.CurrentVectorLimit);
                return _currentVectorLimit;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.CurrentVectorLimit) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.CurrentVectorLimit = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _overtorqueLimit;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("% Motor Rated")]
        [ReadOnly(false)]
        [Category("Torque/Current Loop")]
        public float OvertorqueLimit
        {
            get
            {
                _overtorqueLimit = Convert.ToSingle(_currentAxis.CIPAxis.OvertorqueLimit);
                return _overtorqueLimit;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.OvertorqueLimit) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.OvertorqueLimit = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _overtorqueLimitTime;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("s")]
        [ReadOnly(false)]
        [Category("Torque/Current Loop")]
        public float OvertorqueLimitTime
        {
            get
            {
                _overtorqueLimitTime = Convert.ToSingle(_currentAxis.CIPAxis.OvertorqueLimitTime);
                return _overtorqueLimitTime;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.OvertorqueLimitTime) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.OvertorqueLimitTime = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }

        private float _torqueLimitNegative;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("% Motor Rated")]
        [ReadOnly(false)]
        [Category("Torque/Current Loop")]
        public float TorqueLimitNegative
        {
            get
            {
                _torqueLimitNegative = Convert.ToSingle(_currentAxis.CIPAxis.TorqueLimitNegative);
                return _torqueLimitNegative;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.TorqueLimitNegative) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.TorqueLimitNegative = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _torqueLimitPositive;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("% Motor Rated")]
        [ReadOnly(false)]
        [Category("Torque/Current Loop")]
        public float TorqueLimitPositive
        {
            get
            {
                _torqueLimitPositive = Convert.ToSingle(_currentAxis.CIPAxis.TorqueLimitPositive);
                return _torqueLimitPositive;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.TorqueLimitPositive) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.TorqueLimitPositive = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _torqueLoopBandwidth;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Hz")]
        [ReadOnly(false)]
        [Category("Torque/Current Loop")]
        public float TorqueLoopBandwidth
        {
            get
            {
                _torqueLoopBandwidth = Convert.ToSingle(_currentAxis.CIPAxis.TorqueLoopBandwidth);
                return _torqueLoopBandwidth;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.TorqueLoopBandwidth) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.TorqueLoopBandwidth = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _torqueRateLimit;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("% Motor Rated/s")]
        [ReadOnly(false)]
        [Category("Torque/Current Loop")]
        public float TorqueRateLimit
        {
            get
            {
                _torqueRateLimit = Convert.ToSingle(_currentAxis.CIPAxis.TorqueRateLimit);
                return _torqueRateLimit;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.TorqueRateLimit) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.TorqueRateLimit = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _torqueThreshold;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("% Motor Rated")]
        [ReadOnly(false)]
        [Category("Torque/Current Loop")]
        public float TorqueThreshold
        {
            get
            {
                _torqueThreshold = Convert.ToSingle(_currentAxis.CIPAxis.TorqueThreshold);
                return _torqueThreshold;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.TorqueThreshold) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.TorqueThreshold = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }

        private float _undertorqueLimit;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("% Motor Rated")]
        [ReadOnly(false)]
        [Category("Torque/Current Loop")]
        public float UndertorqueLimit
        {
            get
            {
                _undertorqueLimit = Convert.ToSingle(_currentAxis.CIPAxis.UndertorqueLimit);
                return _undertorqueLimit;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.UndertorqueLimit) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.UndertorqueLimit = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }

        private float _undertorqueLimitTime;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("s")]
        [ReadOnly(false)]
        [Category("Torque/Current Loop")]
        public float UndertorqueLimitTime
        {
            get
            {
                _undertorqueLimitTime = Convert.ToSingle(_currentAxis.CIPAxis.UndertorqueLimitTime);
                return _undertorqueLimitTime;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.UndertorqueLimitTime) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.UndertorqueLimitTime = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }

        #endregion

        private class TorqueCurrentLoop
        {
            private readonly AxisCIPParameters _parameters;

            public TorqueCurrentLoop(AxisCIPParameters parameters)
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
                    new BaseRule {Name = "CurrentVectorLimit", Value = false},
                    new BaseRule {Name = "OvertorqueLimit", Value = false},
                    new BaseRule {Name = "OvertorqueLimitTime", Value = false},
                    new BaseRule {Name = "TorqueLimitNegative", Value = false},
                    new BaseRule {Name = "TorqueLimitPositive", Value = false},
                    new BaseRule {Name = "TorqueLoopBandwidth", Value = false},
                    new BaseRule {Name = "TorqueRateLimit", Value = false},
                    new BaseRule {Name = "TorqueThreshold", Value = false},
                    new BaseRule {Name = "UndertorqueLimit", Value = false},
                    new BaseRule {Name = "UndertorqueLimitTime", Value = false}
                };

                var axisConfiguration =
                    (AxisConfigurationType) Convert.ToByte(parameters._currentAxis.CIPAxis.AxisConfiguration);
                if ((axisConfiguration == AxisConfigurationType.PositionLoop)
                    || (axisConfiguration == AxisConfigurationType.VelocityLoop)
                    || (axisConfiguration == AxisConfigurationType.TorqueLoop))
                {
                    CIPMotionDrive motionDrive =
                        parameters._parentViewModel.ModifiedAxisCIPDrive.AssociatedModule as CIPMotionDrive;
                    if (motionDrive != null)
                    {
                        ruleList.Add(new BaseRule
                        {
                            Name = "CurrentVectorLimit",
                            Value = motionDrive.SupportAxisAttribute(axisConfiguration, "CurrentVectorLimit")
                        });
                    }

                    ruleList.Add(new BaseRule {Name = "OvertorqueLimit", Value = true});
                    ruleList.Add(new BaseRule {Name = "OvertorqueLimitTime", Value = true});
                    ruleList.Add(new BaseRule {Name = "TorqueLimitNegative", Value = true});
                    ruleList.Add(new BaseRule {Name = "TorqueLimitPositive", Value = true});
                    ruleList.Add(new BaseRule {Name = "TorqueLoopBandwidth", Value = true});
                    ruleList.Add(new BaseRule {Name = "TorqueRateLimit", Value = true});
                    ruleList.Add(new BaseRule {Name = "TorqueThreshold", Value = true});
                    ruleList.Add(new BaseRule {Name = "UndertorqueLimit", Value = true});
                    ruleList.Add(new BaseRule {Name = "UndertorqueLimitTime", Value = true});
                }

                return ruleList.ToArray();
            }

            private BaseRule[] GetReadonlyRules(AxisCIPParameters parameters)
            {
                var ruleList = new List<BaseRule>();

                if (parameters._parentViewModel.IsPowerStructureEnabled || parameters._parentViewModel.IsHardRunMode)
                {
                    ruleList.Add(new BaseRule {Name = "CurrentVectorLimit", Value = true});
                    ruleList.Add(new BaseRule {Name = "OvertorqueLimit", Value = true});
                    ruleList.Add(new BaseRule {Name = "OvertorqueLimitTime", Value = true});
                    ruleList.Add(new BaseRule {Name = "TorqueLimitNegative", Value = true});
                    ruleList.Add(new BaseRule {Name = "TorqueLimitPositive", Value = true});
                    ruleList.Add(new BaseRule {Name = "TorqueLoopBandwidth", Value = true});
                    ruleList.Add(new BaseRule {Name = "TorqueRateLimit", Value = true});
                    ruleList.Add(new BaseRule {Name = "TorqueThreshold", Value = true});
                    ruleList.Add(new BaseRule {Name = "UndertorqueLimit", Value = true});
                    ruleList.Add(new BaseRule {Name = "UndertorqueLimitTime", Value = true});
                }
                else
                {
                    ruleList.Add(new BaseRule {Name = "CurrentVectorLimit", Value = false});
                    ruleList.Add(new BaseRule {Name = "OvertorqueLimit", Value = false});
                    ruleList.Add(new BaseRule {Name = "OvertorqueLimitTime", Value = false});
                    ruleList.Add(new BaseRule {Name = "TorqueLimitNegative", Value = false});
                    ruleList.Add(new BaseRule {Name = "TorqueLimitPositive", Value = false});
                    ruleList.Add(new BaseRule {Name = "TorqueLoopBandwidth", Value = false});
                    ruleList.Add(new BaseRule {Name = "TorqueRateLimit", Value = false});
                    ruleList.Add(new BaseRule {Name = "TorqueThreshold", Value = false});
                    ruleList.Add(new BaseRule {Name = "UndertorqueLimit", Value = false});
                    ruleList.Add(new BaseRule {Name = "UndertorqueLimitTime", Value = false});
                }

                return ruleList.ToArray();
            }

            public string[] GetCompareProperties()
            {
                string[] compareProperties =
                {
                    "CurrentVectorLimit", "OvertorqueLimit",
                    "OvertorqueLimitTime", "TorqueLimitNegative", "TorqueLimitPositive",
                    "TorqueLoopBandwidth", "TorqueRateLimit",
                    "TorqueThreshold", "UndertorqueLimit", "UndertorqueLimitTime"
                };
                return compareProperties;
            }
        }
    }
}
