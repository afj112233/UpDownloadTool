using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Controls.ExtendedPropertyGrid.Attributes;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Models
{
    internal partial class AxisCIPParameters
    {
        private readonly FrequencyControl _frequencyControl;

        #region Public Property

        private float _breakFrequency;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Hertz")]
        [ReadOnly(false)]
        [Category("Frequency Control")]
        public float BreakFrequency
        {
            get
            {
                _breakFrequency = Convert.ToSingle(_currentAxis.CIPAxis.BreakFrequency);
                return _breakFrequency;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.BreakFrequency) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.BreakFrequency = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _breakVoltage;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Volts(RMS)")]
        [ReadOnly(false)]
        [Category("Frequency Control")]
        public float BreakVoltage
        {
            get
            {
                _breakVoltage = Convert.ToSingle(_currentAxis.CIPAxis.BreakVoltage);
                return _breakVoltage;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.BreakVoltage) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.BreakVoltage = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private FluxUpControlType _fluxUpControl;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Frequency Control")]
        public FluxUpControlType FluxUpControl
        {
            get
            {
                _fluxUpControl = (FluxUpControlType)Convert.ToByte(_currentAxis.CIPAxis.FluxUpControl);
                return _fluxUpControl;
            }
            set
            {
                if ((FluxUpControlType)Convert.ToByte(_currentAxis.CIPAxis.FluxUpControl) != value)
                {
                    _currentAxis.CIPAxis.FluxUpControl = (byte)value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _fluxUpTime;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("s")]
        [ReadOnly(false)]
        [Category("Frequency Control")]
        public float FluxUpTime {
            get
            {
                _fluxUpTime = Convert.ToSingle(_currentAxis.CIPAxis.FluxUpTime);
                return _fluxUpTime;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.FluxUpTime) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.FluxUpTime = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }


        private FrequencyControlMethodType _frequencyControlMethod;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Frequency Control")]
        public FrequencyControlMethodType FrequencyControlMethod
        {
            get
            {
                _frequencyControlMethod =
                    (FrequencyControlMethodType)Convert.ToByte(_currentAxis.CIPAxis.FrequencyControlMethod);
                return _frequencyControlMethod;
            }
            set
            {
                if ((FrequencyControlMethodType)Convert.ToByte(_currentAxis.CIPAxis.FrequencyControlMethod) != value)
                {
                    _currentAxis.CIPAxis.FrequencyControlMethod = (byte)value;

                    _frequencyControl.UpdateVisibility();

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _maximumFrequency;
        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Hertz")]
        [ReadOnly(false)]
        [Category("Frequency Control")]
        public float MaximumFrequency
        {
            get
            {
                _maximumFrequency = Convert.ToSingle(_currentAxis.CIPAxis.MaximumFrequency);
                return _maximumFrequency;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.MaximumFrequency) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.MaximumFrequency = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _maximumVoltage;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Volts(RMS)")]
        [ReadOnly(false)]
        [Category("Frequency Control")]
        public float MaximumVoltage
        {
            get
            {
                _maximumVoltage = Convert.ToSingle(_currentAxis.CIPAxis.MaximumVoltage);
                return _maximumVoltage;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.MaximumVoltage) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.MaximumVoltage = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _runBoost;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Volts(RMS)")]
        [ReadOnly(false)]
        [Category("Frequency Control")]
        public float RunBoost
        {
            get
            {
                _runBoost = Convert.ToSingle(_currentAxis.CIPAxis.RunBoost);
                return _runBoost;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.RunBoost) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.RunBoost = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _skipSpeed1;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Position Units/s")]
        [ReadOnly(false)]
        [Category("Frequency Control")]
        public float SkipSpeed1
        {
            get
            {
                _skipSpeed1 = Convert.ToSingle(_currentAxis.CIPAxis.SkipSpeed1);
                return _skipSpeed1;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.SkipSpeed1) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.SkipSpeed1 = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _skipSpeed2;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Position Units/s")]
        [ReadOnly(false)]
        [Category("Frequency Control")]
        public float SkipSpeed2
        {
            get
            {
                _skipSpeed2 = Convert.ToSingle(_currentAxis.CIPAxis.SkipSpeed2);
                return _skipSpeed2;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.SkipSpeed2) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.SkipSpeed2 = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _skipSpeedBand;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Position Units/s")]
        [ReadOnly(false)]
        [Category("Frequency Control")]
        public float SkipSpeedBand
        {
            get
            {
                _skipSpeedBand = Convert.ToSingle(_currentAxis.CIPAxis.SkipSpeedBand);
                return _skipSpeedBand;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.SkipSpeedBand) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.SkipSpeedBand = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _startBoost;
        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Volts(RMS)")]
        [ReadOnly(false)]
        [Category("Frequency Control")]
        public float StartBoost
        {
            get
            {
                _startBoost = Convert.ToSingle(_currentAxis.CIPAxis.StartBoost);
                return _startBoost;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.StartBoost) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.StartBoost = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Enum Source

        private ObservableCollection<FluxUpControlType> FluxUpControlSource { get; set; }
        private ObservableCollection<FrequencyControlMethodType> FrequencyControlMethodSource { get; set; }

        #endregion


        private class FrequencyControl
        {
            private readonly AxisCIPParameters _parameters;

            public FrequencyControl(AxisCIPParameters parameters)
            {
                _parameters = parameters;
            }

            public void Refresh()
            {
                UpdateFluxUpControlSource();
                UpdateFrequencyControlMethodSource();

                UpdateVisibility();
                UpdateReadonly();
                UpdateIsChanged();
            }

            private void UpdateFrequencyControlMethodSource()
            {
                var supportTypes = new List<FrequencyControlMethodType>()
                    { FrequencyControlMethodType.BasicVoltsHertz };

                // Optional
                var motionDrive = _parameters._currentAxis.AssociatedModule as CIPMotionDrive;
                var axisConfiguration =
                    (AxisConfigurationType)Convert.ToByte(_parameters._currentAxis.CIPAxis.AxisConfiguration);

                if (motionDrive != null)
                {
                    var optionalSupportedTypes =
                        motionDrive.GetEnumList<FrequencyControlMethodType>("FrequencyControlMethod",
                            axisConfiguration);

                    supportTypes.AddRange(optionalSupportedTypes);
                }

                _parameters.FrequencyControlMethodSource =
                    new ObservableCollection<FrequencyControlMethodType>(supportTypes);
            }

            private void UpdateFluxUpControlSource()
            {
                _parameters.FluxUpControlSource = new ObservableCollection<FluxUpControlType>()
                {
                    FluxUpControlType.NoDelay
                };

                //TODO(gjc):need edit later

                _parameters.FluxUpControlSource.Add(FluxUpControlType.ManualDelay);
                _parameters.FluxUpControlSource.Add(FluxUpControlType.AutomaticDelay);

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
                    new BaseRule { Name = nameof(_parameters.BreakFrequency), Value = false },
                    new BaseRule { Name = nameof(_parameters.BreakVoltage), Value = false },
                    new BaseRule { Name = nameof(_parameters.FluxUpControl), Value = false },
                    new BaseRule { Name = nameof(_parameters.FluxUpTime), Value = false },
                    new BaseRule { Name = nameof(_parameters.FrequencyControlMethod), Value = false },
                    new BaseRule { Name = nameof(_parameters.MaximumFrequency), Value = false },
                    new BaseRule { Name = nameof(_parameters.MaximumVoltage), Value = false },
                    new BaseRule { Name = nameof(_parameters.RunBoost), Value = false },
                    new BaseRule { Name = nameof(_parameters.SkipSpeed1), Value = false },
                    new BaseRule { Name = nameof(_parameters.SkipSpeed2), Value = false },
                    new BaseRule { Name = nameof(_parameters.SkipSpeedBand), Value = false },
                    new BaseRule { Name = nameof(_parameters.StartBoost), Value = false }
                };

                var axisConfiguration =
                    (AxisConfigurationType)Convert.ToByte(parameters._currentAxis.CIPAxis.AxisConfiguration);

                var motorType = (MotorType)Convert.ToByte(parameters._currentAxis.CIPAxis.MotorType);

                if (axisConfiguration == AxisConfigurationType.FrequencyControl)
                {
                    if (motorType == MotorType.RotaryInduction)
                    {
                        ruleList.Add(new BaseRule { Name = nameof(_parameters.FluxUpControl), Value = true });
                        ruleList.Add(new BaseRule { Name = nameof(_parameters.FluxUpTime), Value = true });
                    }
                    
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.FrequencyControlMethod), Value = true });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.MaximumFrequency), Value = true });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.MaximumVoltage), Value = true });

                    if (parameters._currentAxis.AssociatedModule != null)
                    {
                        ruleList.Add(new BaseRule { Name = nameof(_parameters.SkipSpeed1), Value = true });
                        ruleList.Add(new BaseRule { Name = nameof(_parameters.SkipSpeed2), Value = true });
                        ruleList.Add(new BaseRule { Name = nameof(_parameters.SkipSpeedBand), Value = true });
                    }
                }

                switch (_parameters.FrequencyControlMethod)
                {
                    case FrequencyControlMethodType.BasicVoltsHertz:
                        ruleList.Add(new BaseRule { Name = nameof(_parameters.BreakFrequency), Value = true });
                        ruleList.Add(new BaseRule { Name = nameof(_parameters.BreakVoltage), Value = true });
                        ruleList.Add(new BaseRule { Name = nameof(_parameters.RunBoost), Value = true });
                        ruleList.Add(new BaseRule { Name = nameof(_parameters.StartBoost), Value = true });
                        break;
                    case FrequencyControlMethodType.FanPumpVoltsHertz:
                        ruleList.Add(new BaseRule { Name = nameof(_parameters.RunBoost), Value = true });
                        ruleList.Add(new BaseRule { Name = nameof(_parameters.StartBoost), Value = true });
                        break;
                    case FrequencyControlMethodType.SensorlessVector:
                        break;
                    case FrequencyControlMethodType.SensorlessVectorEconomy:
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
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.BreakFrequency), Value = true });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.BreakVoltage), Value = true });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.FluxUpControl), Value = true });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.FluxUpTime), Value = true });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.FrequencyControlMethod), Value = true });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.MaximumFrequency), Value = true });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.MaximumVoltage), Value = true });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.RunBoost), Value = true });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.SkipSpeed1), Value = true });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.SkipSpeed2), Value = true });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.SkipSpeedBand), Value = true });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.StartBoost), Value = true });
                }
                else
                {
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.BreakFrequency), Value = false });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.BreakVoltage), Value = false });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.FluxUpControl), Value = false });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.FluxUpTime), Value = false });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.FrequencyControlMethod), Value = false });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.MaximumFrequency), Value = false });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.MaximumVoltage), Value = false });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.RunBoost), Value = false });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.SkipSpeed1), Value = false });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.SkipSpeed2), Value = false });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.SkipSpeedBand), Value = false });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.StartBoost), Value = false });
                }

                return ruleList.ToArray();
            }

            private void UpdateIsChanged()
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
                    "BreakFrequency", "BreakVoltage",
                    "FluxUpControl", "FluxUpTime",
                    "FrequencyControlMethod",
                    "MaximumFrequency",
                    "MaximumVoltage",
                    "RunBoost", "SkipSpeed1", "SkipSpeed2",
                    "SkipSpeedBand", "StartBoost", "VelocityDroop"
                };

                return compareProperties;
            }
        }
    }
}
