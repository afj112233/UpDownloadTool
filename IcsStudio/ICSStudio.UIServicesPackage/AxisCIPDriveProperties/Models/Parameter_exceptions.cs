using System;
using System.Collections.Generic;
using System.ComponentModel;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Controls.ExtendedPropertyGrid.Attributes;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Models
{
    internal partial class AxisCIPParameters
    {
        private readonly Exceptions _exceptions;

        #region Public Property

        private uint _feedbackDataLossUserLimit;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Data Packets")]
        [ReadOnly(false)]
        [Category("Exceptions")]
        public uint FeedbackDataLossUserLimit
        {
            get
            {
                _feedbackDataLossUserLimit = Convert.ToUInt32(_currentAxis.CIPAxis.FeedbackDataLossUserLimit);
                return _feedbackDataLossUserLimit;
            }
            set
            {
                if (Convert.ToUInt32(_currentAxis.CIPAxis.FeedbackDataLossUserLimit) != value)
                {
                    _currentAxis.CIPAxis.FeedbackDataLossUserLimit = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }


        private float _inverterThermalOverloadUserLimit;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("% Inverter Rated")]
        [ReadOnly(false)]
        [Category("Exceptions")]
        public float InverterThermalOverloadUserLimit
        {
            get
            {
                _inverterThermalOverloadUserLimit =
                    Convert.ToSingle(_currentAxis.CIPAxis.InverterThermalOverloadUserLimit);
                return _inverterThermalOverloadUserLimit;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.InverterThermalOverloadUserLimit) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.InverterThermalOverloadUserLimit = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }


        private float _motorOverspeedUserLimit;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("% Motor Rated")]
        [ReadOnly(false)]
        [Category("Exceptions")]
        public float MotorOverspeedUserLimit
        {
            get
            {
                _motorOverspeedUserLimit = Convert.ToSingle(_currentAxis.CIPAxis.MotorOverspeedUserLimit);
                return _motorOverspeedUserLimit;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.MotorOverspeedUserLimit) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.MotorOverspeedUserLimit = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _motorThermalOverloadUserLimit;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("% Motor Rated")]
        [ReadOnly(false)]
        [Category("Exceptions")]
        public float MotorThermalOverloadUserLimit
        {
            get
            {
                _motorThermalOverloadUserLimit = Convert.ToSingle(_currentAxis.CIPAxis.MotorThermalOverloadUserLimit);
                return _motorThermalOverloadUserLimit;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.MotorThermalOverloadUserLimit) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.MotorThermalOverloadUserLimit = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }

        #endregion

        private class Exceptions
        {
            private readonly AxisCIPParameters _parameters;

            public Exceptions(AxisCIPParameters parameters)
            {
                _parameters = parameters;
            }

            public void Refresh()
            {
                UpdateVisibility();
                UpdateReadonly();
                UpdateIsChanged();
            }

            public string[] GetCompareProperties()
            {
                string[] compareProperties =
                {
                    "FeedbackDataLossUserLimit", "InverterThermalOverloadUserLimit",
                    "MotorOverspeedUserLimit", "MotorThermalOverloadUserLimit"
                };

                return compareProperties;
            }

            public string[] GetPeriodicRefreshProperties()
            {
                string[] periodicRefreshProperties = new[]
                {
                    "MotorOverspeedUserLimit",
                };

                return periodicRefreshProperties;
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
                    new BaseRule {Name = "FeedbackDataLossUserLimit", Value = false},
                    new BaseRule {Name = "InverterThermalOverloadUserLimit", Value = false},
                    new BaseRule {Name = "MotorOverspeedUserLimit", Value = false},
                    new BaseRule {Name = "MotorThermalOverloadUserLimit", Value = false},
                };

                var axisConfiguration =
                    (AxisConfigurationType) Convert.ToByte(parameters._currentAxis.CIPAxis.AxisConfiguration);

                var feedback1Type = (FeedbackType) Convert.ToByte(parameters._currentAxis.CIPAxis.Feedback1Type);

                if (axisConfiguration == AxisConfigurationType.FeedbackOnly)
                {
                    if (feedback1Type != FeedbackType.NotSpecified)
                        ruleList.Add(new BaseRule {Name = "FeedbackDataLossUserLimit", Value = true});
                }
                else if ((axisConfiguration == AxisConfigurationType.PositionLoop)
                         || (axisConfiguration == AxisConfigurationType.VelocityLoop)
                         || (axisConfiguration == AxisConfigurationType.TorqueLoop))
                {
                    if (feedback1Type != FeedbackType.NotSpecified)
                        ruleList.Add(new BaseRule {Name = "FeedbackDataLossUserLimit", Value = true});

                    ruleList.Add(new BaseRule {Name = "InverterThermalOverloadUserLimit", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorOverspeedUserLimit", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorThermalOverloadUserLimit", Value = true});
                }
                else if (axisConfiguration == AxisConfigurationType.FrequencyControl)
                {
                    ruleList.Add(new BaseRule { Name = "InverterThermalOverloadUserLimit", Value = true });
                    ruleList.Add(new BaseRule { Name = "MotorThermalOverloadUserLimit", Value = true });
                }

                return ruleList.ToArray();
            }

            private BaseRule[] GetReadonlyRules(AxisCIPParameters parameters)
            {
                List<BaseRule> ruleList = new List<BaseRule>();

                if (parameters._parentViewModel.IsPowerStructureEnabled || parameters._parentViewModel.IsHardRunMode)
                {
                    ruleList.Add(new BaseRule {Name = "FeedbackDataLossUserLimit", Value = true});
                    ruleList.Add(new BaseRule {Name = "InverterThermalOverloadUserLimit", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorOverspeedUserLimit", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorThermalOverloadUserLimit", Value = true});
                }
                else
                {
                    ruleList.Add(new BaseRule {Name = "FeedbackDataLossUserLimit", Value = false});
                    ruleList.Add(new BaseRule {Name = "InverterThermalOverloadUserLimit", Value = false});
                    ruleList.Add(new BaseRule {Name = "MotorOverspeedUserLimit", Value = false});
                    ruleList.Add(new BaseRule {Name = "MotorThermalOverloadUserLimit", Value = false});
                }
                
                return ruleList.ToArray();
            }
        }
    }
}
