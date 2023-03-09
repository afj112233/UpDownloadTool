using System;
using System.Collections.Generic;
using System.ComponentModel;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Controls.ExtendedPropertyGrid.Attributes;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Models
{
    internal partial class AxisCIPParameters
    {
        private readonly Backlash _backlash;

        #region Public Property

        private float _backlashCompensationWindow;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Position Units")]
        [ReadOnly(false)]
        [Category("Backlash")]
        public float BacklashCompensationWindow
        {
            get
            {
                _backlashCompensationWindow = Convert.ToSingle(_currentAxis.CIPAxis.BacklashCompensationWindow);
                return _backlashCompensationWindow;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.BacklashCompensationWindow) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.BacklashCompensationWindow = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _backlashReversalOffset;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Position Units")]
        [ReadOnly(false)]
        [Category("Backlash")]
        public float BacklashReversalOffset
        {
            get
            {
                _backlashReversalOffset = Convert.ToSingle(_currentAxis.CIPAxis.BacklashReversalOffset);
                return _backlashReversalOffset;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.BacklashReversalOffset) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.BacklashReversalOffset = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        private class Backlash
        {
            private readonly AxisCIPParameters _parameters;

            public Backlash(AxisCIPParameters parameters)
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
                            _parameters._originalAxis.CIPAxis, propertyName);

                    PropertySetting.SetPropertyIsChanged(_parameters, propertyName, isChange);
                }
            }

            private BaseRule[] GetVisibilityRules(AxisCIPParameters parameters)
            {
                var ruleList = new List<BaseRule>
                {
                    new BaseRule {Name = "BacklashCompensationWindow", Value = false},
                    new BaseRule {Name = "BacklashReversalOffset", Value = false}
                };

                // axis configuration
                var axisConfiguration =
                    (AxisConfigurationType) Convert.ToByte(parameters._currentAxis.CIPAxis.AxisConfiguration);

                if (axisConfiguration == AxisConfigurationType.PositionLoop)
                {
                    ruleList.Add(new BaseRule {Name = "BacklashCompensationWindow", Value = true});
                    ruleList.Add(new BaseRule {Name = "BacklashReversalOffset", Value = true});
                }

                return ruleList.ToArray();
            }

            private BaseRule[] GetReadonlyRules(AxisCIPParameters parameters)
            {
                var ruleList = new List<BaseRule>();

                if (parameters._parentViewModel.IsPowerStructureEnabled || parameters._parentViewModel.IsHardRunMode)
                {
                    ruleList.Add(new BaseRule {Name = "BacklashCompensationWindow", Value = true});
                    ruleList.Add(new BaseRule {Name = "BacklashReversalOffset", Value = true});
                }
                else
                {
                    ruleList.Add(new BaseRule {Name = "BacklashCompensationWindow", Value = false});
                    ruleList.Add(new BaseRule {Name = "BacklashReversalOffset", Value = false});
                }

                return ruleList.ToArray();
            }

            public string[] GetCompareProperties()
            {
                string[] compareProperties =
                {
                    "BacklashCompensationWindow", "BacklashReversalOffset"
                };

                return compareProperties;
            }
        }
    }
}
