using System;
using System.Collections.Generic;
using System.ComponentModel;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Controls.ExtendedPropertyGrid.Attributes;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Models
{
    internal partial class AxisCIPParameters
    {
        private readonly Friction _friction;

        #region Public Property

        private float _frictionCompensationSliding;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("% Motor Rated")]
        [ReadOnly(false)]
        [Category("Friction")]
        public float FrictionCompensationSliding
        {
            get
            {
                _frictionCompensationSliding = Convert.ToSingle(_currentAxis.CIPAxis.FrictionCompensationSliding);
                return _frictionCompensationSliding;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.FrictionCompensationSliding) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.FrictionCompensationSliding = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _frictionCompensationStatic;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("% Motor Rated")]
        [ReadOnly(false)]
        [Category("Friction")]
        public float FrictionCompensationStatic
        {
            get
            {
                _frictionCompensationStatic = Convert.ToSingle(_currentAxis.CIPAxis.FrictionCompensationStatic);
                return _frictionCompensationStatic;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.FrictionCompensationStatic) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.FrictionCompensationStatic = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _frictionCompensationViscous;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("% Motor Rated/(Motor Rev/s)")]
        [ReadOnly(false)]
        [Category("Friction")]
        public float FrictionCompensationViscous
        {
            get
            {
                _frictionCompensationViscous = Convert.ToSingle(_currentAxis.CIPAxis.FrictionCompensationViscous);
                return _frictionCompensationViscous;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.FrictionCompensationViscous) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.FrictionCompensationViscous = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _frictionCompensationWindow;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Position Units")]
        [ReadOnly(false)]
        [Category("Friction")]
        public float FrictionCompensationWindow
        {
            get
            {
                _frictionCompensationWindow = Convert.ToSingle(_currentAxis.CIPAxis.FrictionCompensationWindow);
                return _frictionCompensationWindow;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.FrictionCompensationWindow) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.FrictionCompensationWindow = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }

        #endregion

        private class Friction
        {
            private readonly AxisCIPParameters _parameters;

            public Friction(AxisCIPParameters parameters)
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
                    new BaseRule {Name = "FrictionCompensationWindow", Value = false},
                    new BaseRule {Name = "FrictionCompensationSliding", Value = false},
                    new BaseRule {Name = "FrictionCompensationStatic", Value = false},
                    new BaseRule {Name = "FrictionCompensationViscous", Value = false}
                };

                // axis configuration
                var axisConfiguration =
                    (AxisConfigurationType) Convert.ToByte(parameters._currentAxis.CIPAxis.AxisConfiguration);

                switch (axisConfiguration)
                {
                    case AxisConfigurationType.FeedbackOnly:
                        break;
                    case AxisConfigurationType.FrequencyControl:
                        break;
                    case AxisConfigurationType.PositionLoop:
                        ruleList.Add(new BaseRule {Name = "FrictionCompensationWindow", Value = true});
                        ruleList.Add(new BaseRule {Name = "FrictionCompensationSliding", Value = true});
                        ruleList.Add(new BaseRule {Name = "FrictionCompensationStatic", Value = true});
                        ruleList.Add(new BaseRule {Name = "FrictionCompensationViscous", Value = true});
                        break;
                    case AxisConfigurationType.VelocityLoop:
                        ruleList.Add(new BaseRule {Name = "FrictionCompensationSliding", Value = true});
                        ruleList.Add(new BaseRule {Name = "FrictionCompensationStatic", Value = true});
                        ruleList.Add(new BaseRule {Name = "FrictionCompensationViscous", Value = true});
                        break;
                    case AxisConfigurationType.TorqueLoop:
                        ruleList.Add(new BaseRule {Name = "FrictionCompensationSliding", Value = true});
                        ruleList.Add(new BaseRule {Name = "FrictionCompensationStatic", Value = true});
                        ruleList.Add(new BaseRule {Name = "FrictionCompensationViscous", Value = true});
                        break;
                    case AxisConfigurationType.ConverterOnly:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }


                return ruleList.ToArray();
            }

            private BaseRule[] GetReadonlyRules(AxisCIPParameters parameters)
            {
                var ruleList = new List<BaseRule>();

                if (parameters._parentViewModel.IsPowerStructureEnabled || parameters._parentViewModel.IsHardRunMode)
                {
                    ruleList.Add(new BaseRule {Name = "FrictionCompensationWindow", Value = true});
                    ruleList.Add(new BaseRule {Name = "FrictionCompensationSliding", Value = true});
                    ruleList.Add(new BaseRule {Name = "FrictionCompensationStatic", Value = true});
                    ruleList.Add(new BaseRule {Name = "FrictionCompensationViscous", Value = true});
                }
                else
                {
                    ruleList.Add(new BaseRule {Name = "FrictionCompensationWindow", Value = false});
                    ruleList.Add(new BaseRule {Name = "FrictionCompensationSliding", Value = false});
                    ruleList.Add(new BaseRule {Name = "FrictionCompensationStatic", Value = false});
                    ruleList.Add(new BaseRule {Name = "FrictionCompensationViscous", Value = false});
                }

                return ruleList.ToArray();
            }

            public string[] GetCompareProperties()
            {
                string[] compareProperties =
                {
                    "FrictionCompensationSliding", "FrictionCompensationStatic",
                    "FrictionCompensationViscous", "FrictionCompensationWindow"
                };

                return compareProperties;
            }

            public string[] GetPeriodicRefreshProperties()
            {
                string[] periodicRefreshProperties = new[]
                {
                    "FrictionCompensationSliding",
                    "FrictionCompensationStatic",
                    "FrictionCompensationWindow"
                };

                return periodicRefreshProperties;
            }
        }
    }
}
