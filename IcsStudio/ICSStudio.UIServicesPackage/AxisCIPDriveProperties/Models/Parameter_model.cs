using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Controls.ExtendedPropertyGrid.Attributes;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Models
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal partial class AxisCIPParameters
    {
        private readonly Model _model;

        #region Public Property

        private float _PMMotorInductance;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Henries")]
        [ReadOnly(false)]
        [Category("Model")]
        public float PMMotorInductance
        {
            get
            {
                _PMMotorInductance = Convert.ToSingle(_currentAxis.CIPAxis.PMMotorInductance);
                return _PMMotorInductance;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.PMMotorInductance) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.PMMotorInductance = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _PMMotorResistance;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Ohms")]
        [ReadOnly(false)]
        [Category("Model")]
        public float PMMotorResistance
        {
            get
            {
                _PMMotorResistance = Convert.ToSingle(_currentAxis.CIPAxis.PMMotorResistance);
                return _PMMotorResistance;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.PMMotorResistance) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.PMMotorResistance = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _PMMotorRotaryVoltageConstant;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Volts(RMS)/KRPM")]
        [ReadOnly(false)]
        [Category("Model")]
        public float PMMotorRotaryVoltageConstant
        {
            get
            {
                _PMMotorRotaryVoltageConstant = Convert.ToSingle(_currentAxis.CIPAxis.PMMotorRotaryVoltageConstant);
                return _PMMotorRotaryVoltageConstant;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.PMMotorRotaryVoltageConstant) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.PMMotorRotaryVoltageConstant = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _PMMotorTorqueConstant;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("N-m/Amps(RMS)")]
        [ReadOnly(false)]
        [Category("Model")]
        public float PMMotorTorqueConstant
        {
            get
            {
                _PMMotorTorqueConstant = Convert.ToSingle(_currentAxis.CIPAxis.PMMotorTorqueConstant);
                return _PMMotorTorqueConstant;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.PMMotorTorqueConstant) - value) >
                    float.Epsilon)

                {
                    _currentAxis.CIPAxis.PMMotorTorqueConstant = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _inductionMotorFluxCurrent;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Amps(RMS)")]
        [ReadOnly(false)]
        [Category("Model")]
        public float InductionMotorFluxCurrent
        {
            get
            {
                _inductionMotorFluxCurrent = Convert.ToSingle(_currentAxis.CIPAxis.InductionMotorFluxCurrent);
                return _inductionMotorFluxCurrent;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.InductionMotorFluxCurrent) - value) >
                    float.Epsilon)

                {
                    _currentAxis.CIPAxis.InductionMotorFluxCurrent = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }


        private float _inductionMotorRatedSlipSpeed;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("RPM")]
        [ReadOnly(false)]
        [Category("Model")]
        public float InductionMotorRatedSlipSpeed
        {
            get
            {
                _inductionMotorRatedSlipSpeed = Convert.ToSingle(_currentAxis.CIPAxis.InductionMotorRatedSlipSpeed);
                return _inductionMotorRatedSlipSpeed;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.InductionMotorRatedSlipSpeed) - value) >
                    float.Epsilon)

                {
                    _currentAxis.CIPAxis.InductionMotorRatedSlipSpeed = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _inductionMotorRotorLeakageReactance;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Ohms")]
        [ReadOnly(false)]
        [Category("Model")]
        public float InductionMotorRotorLeakageReactance
        {
            get
            {
                _inductionMotorRotorLeakageReactance =
                    Convert.ToSingle(_currentAxis.CIPAxis.InductionMotorRotorLeakageReactance);
                return _inductionMotorRotorLeakageReactance;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.InductionMotorRotorLeakageReactance) - value) >
                    float.Epsilon)

                {
                    _currentAxis.CIPAxis.InductionMotorRotorLeakageReactance = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _inductionMotorStatorLeakageReactance;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Ohms")]
        [ReadOnly(false)]
        [Category("Model")]
        public float InductionMotorStatorLeakageReactance
        {
            get
            {
                _inductionMotorStatorLeakageReactance =
                    Convert.ToSingle(_currentAxis.CIPAxis.InductionMotorStatorLeakageReactance);
                return _inductionMotorStatorLeakageReactance;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.InductionMotorStatorLeakageReactance) - value) >
                    float.Epsilon)

                {
                    _currentAxis.CIPAxis.InductionMotorStatorLeakageReactance = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _inductionMotorStatorResistance;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Ohms")]
        [ReadOnly(false)]
        [Category("Model")]
        public float InductionMotorStatorResistance
        {
            get
            {
                _inductionMotorStatorResistance =
                    Convert.ToSingle(_currentAxis.CIPAxis.InductionMotorStatorResistance);
                return _inductionMotorStatorResistance;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.InductionMotorStatorResistance) - value) >
                    float.Epsilon)

                {
                    _currentAxis.CIPAxis.InductionMotorStatorResistance = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        #endregion


        private class Model
        {
            private readonly AxisCIPParameters _parameters;

            public Model(AxisCIPParameters parameters)
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

            private BaseRule[] GetVisibilityRules(AxisCIPParameters parameters)
            {
                var ruleList = new List<BaseRule>
                {
                    new BaseRule { Name = "PMMotorInductance", Value = false },
                    new BaseRule { Name = "PMMotorResistance", Value = false },
                    new BaseRule { Name = "PMMotorRotaryVoltageConstant", Value = false },
                    new BaseRule { Name = "PMMotorTorqueConstant", Value = false },

                    new BaseRule { Name = "InductionMotorFluxCurrent", Value = false },
                    new BaseRule { Name = "InductionMotorRatedSlipSpeed", Value = false },
                    new BaseRule { Name = "InductionMotorRotorLeakageReactance", Value = false },
                    new BaseRule { Name = "InductionMotorStatorLeakageReactance", Value = false },
                    new BaseRule { Name = "InductionMotorStatorResistance", Value = false },
                };

                // Data Source
                var motorDataSource =
                    (MotorDataSourceType)Convert.ToByte(parameters._currentAxis.CIPAxis.MotorDataSource);
                var motorType = (MotorType)Convert.ToByte(parameters._currentAxis.CIPAxis.MotorType);
                if ((motorDataSource == MotorDataSourceType.Database) ||
                    (motorDataSource == MotorDataSourceType.Datasheet))
                    switch (motorType)
                    {
                        case MotorType.NotSpecified:
                            break;
                        case MotorType.RotaryPermanentMagnet:
                            ruleList.Add(new BaseRule { Name = "PMMotorInductance", Value = true });
                            ruleList.Add(new BaseRule { Name = "PMMotorResistance", Value = true });
                            ruleList.Add(new BaseRule { Name = "PMMotorRotaryVoltageConstant", Value = true });
                            ruleList.Add(new BaseRule { Name = "PMMotorTorqueConstant", Value = true });
                            break;
                        case MotorType.RotaryInduction:
                            ruleList.Add(new BaseRule { Name = "InductionMotorFluxCurrent", Value = true });
                            ruleList.Add(new BaseRule { Name = "InductionMotorRatedSlipSpeed", Value = true });
                            ruleList.Add(new BaseRule { Name = "InductionMotorRotorLeakageReactance", Value = true });
                            ruleList.Add(new BaseRule { Name = "InductionMotorStatorLeakageReactance", Value = true });
                            ruleList.Add(new BaseRule { Name = "InductionMotorStatorResistance", Value = true });
                            break;
                        case MotorType.LinearPermanentMagnet:
                            break;
                        case MotorType.LinearInduction:
                            break;
                        case MotorType.RotaryInteriorPermanentMagnet:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                // todo(gjc): add code here

                return ruleList.ToArray();
            }

            private BaseRule[] GetReadonlyRules(AxisCIPParameters parameters)
            {
                var ruleList = new List<BaseRule>();

                if (parameters._parentViewModel.IsPowerStructureEnabled || parameters._parentViewModel.IsHardRunMode)
                {
                    ruleList.Add(new BaseRule { Name = "PMMotorInductance", Value = true });
                    ruleList.Add(new BaseRule { Name = "PMMotorResistance", Value = true });
                    ruleList.Add(new BaseRule { Name = "PMMotorRotaryVoltageConstant", Value = true });
                    ruleList.Add(new BaseRule { Name = "PMMotorTorqueConstant", Value = true });

                    ruleList.Add(new BaseRule { Name = "InductionMotorFluxCurrent", Value = true });
                    ruleList.Add(new BaseRule { Name = "InductionMotorRatedSlipSpeed", Value = true });
                    ruleList.Add(new BaseRule { Name = "InductionMotorRotorLeakageReactance", Value = true });
                    ruleList.Add(new BaseRule { Name = "InductionMotorStatorLeakageReactance", Value = true });
                    ruleList.Add(new BaseRule { Name = "InductionMotorStatorResistance", Value = true });

                }
                else
                {
                    ruleList.Add(new BaseRule { Name = "PMMotorInductance", Value = true });
                    ruleList.Add(new BaseRule { Name = "PMMotorResistance", Value = true });
                    ruleList.Add(new BaseRule { Name = "PMMotorRotaryVoltageConstant", Value = true });
                    ruleList.Add(new BaseRule { Name = "PMMotorTorqueConstant", Value = true });

                    ruleList.Add(new BaseRule { Name = "InductionMotorFluxCurrent", Value = true });
                    ruleList.Add(new BaseRule { Name = "InductionMotorRatedSlipSpeed", Value = true });
                    ruleList.Add(new BaseRule { Name = "InductionMotorRotorLeakageReactance", Value = true });
                    ruleList.Add(new BaseRule { Name = "InductionMotorStatorLeakageReactance", Value = true });
                    ruleList.Add(new BaseRule { Name = "InductionMotorStatorResistance", Value = true });

                    // Data Source
                    var motorDataSource =
                        (MotorDataSourceType)Convert.ToByte(parameters._currentAxis.CIPAxis.MotorDataSource);
                    switch (motorDataSource)
                    {
                        case MotorDataSourceType.Datasheet:
                            ruleList.Add(new BaseRule { Name = "PMMotorInductance", Value = false });
                            ruleList.Add(new BaseRule { Name = "PMMotorResistance", Value = false });
                            ruleList.Add(new BaseRule { Name = "PMMotorRotaryVoltageConstant", Value = false });
                            ruleList.Add(new BaseRule { Name = "PMMotorTorqueConstant", Value = false });


                            ruleList.Add(new BaseRule { Name = "InductionMotorFluxCurrent", Value = false });
                            ruleList.Add(new BaseRule { Name = "InductionMotorRatedSlipSpeed", Value = false });
                            ruleList.Add(new BaseRule { Name = "InductionMotorRotorLeakageReactance", Value = false });
                            ruleList.Add(new BaseRule { Name = "InductionMotorStatorLeakageReactance", Value = false });
                            ruleList.Add(new BaseRule { Name = "InductionMotorStatorResistance", Value = false });
                            break;
                        case MotorDataSourceType.Database:
                            break;
                        case MotorDataSourceType.DriveNV:
                            break;
                        case MotorDataSourceType.MotorNV:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    // todo(gjc): add code here
                }

                return ruleList.ToArray();
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

            public string[] GetCompareProperties()
            {
                string[] compareProperties =
                {
                    "PMMotorInductance", "PMMotorResistance",
                    "PMMotorRotaryVoltageConstant", "PMMotorTorqueConstant"
                };

                return compareProperties;
            }
        }
    }
}
