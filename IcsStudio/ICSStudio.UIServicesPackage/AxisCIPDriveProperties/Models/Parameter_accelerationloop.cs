using System;
using System.Collections.Generic;
using System.ComponentModel;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Controls.ExtendedPropertyGrid.Attributes;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Models
{
    internal partial class AxisCIPParameters
    {
        private readonly AccelerationLoop _accelerationLoop;

        #region Public Property

        private float _accelerationLimit;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Position Units/s^2")]
        [ReadOnly(false)]
        [Category("Acceleration Loop")]
        public float AccelerationLimit
        {
            get
            {
                _accelerationLimit = Convert.ToSingle(_currentAxis.CIPAxis.AccelerationLimit);
                return _accelerationLimit;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.AccelerationLimit) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.AccelerationLimit = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _decelerationLimit;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Position Units/s^2")]
        [ReadOnly(false)]
        [Category("Acceleration Loop")]
        public float DecelerationLimit
        {
            get
            {
                _decelerationLimit = Convert.ToSingle(_currentAxis.CIPAxis.DecelerationLimit);
                return _decelerationLimit;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.DecelerationLimit) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.DecelerationLimit = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        private class AccelerationLoop
        {
            private readonly AxisCIPParameters _parameters;

            public AccelerationLoop(AxisCIPParameters parameters)
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
                    bool isChange =
                        !CipAttributeHelper.EqualByAttributeName(
                            _parameters._currentAxis.CIPAxis,
                            _parameters._originalAxis.CIPAxis, propertyName);

                    PropertySetting.SetPropertyIsChanged(_parameters, propertyName, isChange);
                }
            }

            private BaseRule[] GetVisibilityRules(AxisCIPParameters parameters)
            {

                var ruleList = new List<BaseRule>
                {
                    new BaseRule {Name = "AccelerationLimit", Value = false},
                    new BaseRule {Name = "DecelerationLimit", Value = false}
                };

                if (parameters._currentAxis.AssociatedModule != null)
                {
                    var axisConfiguration =
                        (AxisConfigurationType) Convert.ToByte(parameters._currentAxis.CIPAxis
                            .AxisConfiguration);

                    if (axisConfiguration == AxisConfigurationType.PositionLoop ||
                        axisConfiguration == AxisConfigurationType.VelocityLoop)
                    {
                        ruleList.Add(new BaseRule {Name = "AccelerationLimit", Value = true});
                        ruleList.Add(new BaseRule {Name = "DecelerationLimit", Value = true});
                    }
                }

                return ruleList.ToArray();
            }

            private BaseRule[] GetReadonlyRules(AxisCIPParameters parameters)
            {
                var ruleList = new List<BaseRule>();

                if (parameters._parentViewModel.IsPowerStructureEnabled || parameters._parentViewModel.IsHardRunMode)
                {
                    ruleList.Add(new BaseRule { Name = "AccelerationLimit", Value = true });
                    ruleList.Add(new BaseRule { Name = "DecelerationLimit", Value = true });
                }
                else
                {
                    ruleList.Add(new BaseRule { Name = "AccelerationLimit", Value = false });
                    ruleList.Add(new BaseRule { Name = "DecelerationLimit", Value = false });
                }

                return ruleList.ToArray();
            }

            public string[] GetCompareProperties()
            {
                string[] compareProperties =
                {
                    "AccelerationLimit", "DecelerationLimit"
                };

                return compareProperties;
            }

            public string[] GetPeriodicRefreshProperties()
            {
                string[] periodicRefreshProperties = new[]
                {
                    "AccelerationLimit", "DecelerationLimit"
                };

                return periodicRefreshProperties;
            }
        }

    }
}
