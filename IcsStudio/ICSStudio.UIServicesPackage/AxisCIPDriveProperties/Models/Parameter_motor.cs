using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Controls.ExtendedPropertyGrid.Attributes;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Models
{
    [SuppressMessage("ReSharper", "NotResolvedInText")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    [SuppressMessage("ReSharper", "CollectionNeverQueried.Local")]
    internal partial class AxisCIPParameters
    {
        private readonly Motor _motor;

        #region Public Property

        private float _inductionMotorRatedFrequency;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Hertz")]
        [ReadOnly(false)]
        [Category("Motor")]
        public float InductionMotorRatedFrequency
        {
            get
            {
                _inductionMotorRatedFrequency = Convert.ToSingle(_currentAxis.CIPAxis.InductionMotorRatedFrequency);
                return _inductionMotorRatedFrequency;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.InductionMotorRatedFrequency) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.InductionMotorRatedFrequency = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private string _motorCatalogNumber;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(true)]
        [Category("Motor")]
        public string MotorCatalogNumber
        {
            get
            {
                _motorCatalogNumber = _currentAxis.CIPAxis.MotorCatalogNumber.GetString();
                return _motorCatalogNumber;
            }
            set
            {
                var motorCatalogNumber = _currentAxis.CIPAxis.MotorCatalogNumber.GetString();
                if (!string.Equals(motorCatalogNumber, value))
                {
                    _currentAxis.CIPAxis.MotorCatalogNumber = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private MotorDataSourceType _motorDataSource;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Motor")]
        public MotorDataSourceType MotorDataSource
        {
            get
            {
                _motorDataSource = (MotorDataSourceType) Convert.ToByte(_currentAxis.CIPAxis.MotorDataSource);
                return _motorDataSource;
            }
            set
            {
                if ((MotorDataSourceType) Convert.ToByte(_currentAxis.CIPAxis.MotorDataSource) != value)
                {
                    _currentAxis.CIPAxis.MotorDataSource = (byte) value;

                    AxisDefaultSetting.ResetMotorProperty(_currentAxis.CIPAxis);

                    //
                    _motor.UpdateMotorTypeSource();

                    _motor.UpdateVisibility();
                    _motor.UpdateReadonly();
                    _motor.UpdateIsChanged();

                    UpdateIsChanged();
                    OnPropertyChanged();
                }


            }
        }

        private BooleanTypeA _motorIntegralThermalSwitch;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Motor")]
        public BooleanTypeA MotorIntegralThermalSwitch
        {
            get
            {
                _motorIntegralThermalSwitch =
                    (BooleanTypeA) Convert.ToByte(_currentAxis.CIPAxis.MotorIntegralThermalSwitch);
                return _motorIntegralThermalSwitch;
            }
            set
            {
                if ((BooleanTypeA) Convert.ToByte(_currentAxis.CIPAxis.MotorIntegralThermalSwitch) != value)
                {
                    _currentAxis.CIPAxis.MotorIntegralThermalSwitch = (byte) value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _motorMaxWindingTemperature;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("℃")]
        [ReadOnly(false)]
        [Category("Motor")]
        public float MotorMaxWindingTemperature
        {
            get
            {
                _motorMaxWindingTemperature = Convert.ToSingle(_currentAxis.CIPAxis.MotorMaxWindingTemperature);
                return _motorMaxWindingTemperature;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.MotorMaxWindingTemperature) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.MotorMaxWindingTemperature = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _motorOverloadLimit;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Unit("% Motor Rated")]
        [Category("Motor")]
        public float MotorOverloadLimit
        {
            get
            {
                _motorOverloadLimit = Convert.ToSingle(_currentAxis.CIPAxis.MotorOverloadLimit);
                return _motorOverloadLimit;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.MotorOverloadLimit) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.MotorOverloadLimit = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }


        private float _motorRatedContinuousCurrent;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Amps(RMS)")]
        [ReadOnly(false)]
        [Category("Motor")]
        public float MotorRatedContinuousCurrent
        {
            get
            {
                _motorRatedContinuousCurrent = Convert.ToSingle(_currentAxis.CIPAxis.MotorRatedContinuousCurrent);
                return _motorRatedContinuousCurrent;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.MotorRatedContinuousCurrent) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.MotorRatedContinuousCurrent = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _motorRatedOutputPower;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("kW")]
        [ReadOnly(false)]
        [Category("Motor")]
        public float MotorRatedOutputPower
        {
            get
            {
                _motorRatedOutputPower = Convert.ToSingle(_currentAxis.CIPAxis.MotorRatedOutputPower);
                return _motorRatedOutputPower;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.MotorRatedOutputPower) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.MotorRatedOutputPower = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _motorRatedPeakCurrent;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Amps(RMS)")]
        [ReadOnly(false)]
        [Category("Motor")]
        public float MotorRatedPeakCurrent
        {
            get
            {
                _motorRatedPeakCurrent = Convert.ToSingle(_currentAxis.CIPAxis.MotorRatedPeakCurrent);
                return _motorRatedPeakCurrent;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.MotorRatedPeakCurrent) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.MotorRatedPeakCurrent = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _motorRatedVoltage;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Volts(RMS)")]
        [ReadOnly(false)]
        [Category("Motor")]
        public float MotorRatedVoltage
        {
            get
            {
                _motorRatedVoltage = Convert.ToSingle(_currentAxis.CIPAxis.MotorRatedVoltage);
                return _motorRatedVoltage;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.MotorRatedVoltage) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.MotorRatedVoltage = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }

        private MotorType _motorType;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Motor")]
        public MotorType MotorType
        {
            get
            {
                _motorType = (MotorType) Convert.ToByte(_currentAxis.CIPAxis.MotorType);
                return _motorType;
            }
            set
            {
                var motorType = (MotorType) Convert.ToByte(_currentAxis.CIPAxis.MotorType);
                if (motorType != value)
                {
                    _currentAxis.CIPAxis.MotorType = (byte) value;

                    AxisDefaultSetting.LoadDefaultMotorValues(_currentAxis.CIPAxis, value);

                    _motor.UpdateVisibility();
                    _motor.UpdateReadonly();
                    _motor.UpdateIsChanged();

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private MotorUnitType _motorUnit;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Motor")]
        public MotorUnitType MotorUnit
        {
            get
            {
                _motorUnit = (MotorUnitType) Convert.ToByte(_currentAxis.CIPAxis.MotorUnit);
                return _motorUnit;
            }
            set
            {
                if ((MotorUnitType) Convert.ToByte(_currentAxis.CIPAxis.MotorUnit) != value)
                {
                    _currentAxis.CIPAxis.MotorUnit = (byte) value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _motorWindingToAmbientCapacitance;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("J/℃")]
        [ReadOnly(false)]
        [Category("Motor")]
        public float MotorWindingToAmbientCapacitance
        {
            get
            {
                _motorWindingToAmbientCapacitance =
                    Convert.ToSingle(_currentAxis.CIPAxis.MotorWindingToAmbientCapacitance);
                return _motorWindingToAmbientCapacitance;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.MotorWindingToAmbientCapacitance) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.MotorWindingToAmbientCapacitance = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _motorWindingToAmbientResistance;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("℃/W")]
        [ReadOnly(false)]
        [Category("Motor")]
        public float MotorWindingToAmbientResistance
        {
            get
            {
                _motorWindingToAmbientResistance =
                    Convert.ToSingle(_currentAxis.CIPAxis.MotorWindingToAmbientResistance);
                return _motorWindingToAmbientResistance;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.MotorWindingToAmbientResistance) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.MotorWindingToAmbientResistance = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        private float _PMMotorRatedTorque;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("N-m")]
        [ReadOnly(false)]
        [Category("Motor")]
        public float PMMotorRatedTorque
        {
            get
            {
                _PMMotorRatedTorque = Convert.ToSingle(_currentAxis.CIPAxis.PMMotorRatedTorque);
                return _PMMotorRatedTorque;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.PMMotorRatedTorque) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.PMMotorRatedTorque = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _rotaryMotorMaxSpeed;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("RPM")]
        [ReadOnly(false)]
        [Category("Motor")]
        public float RotaryMotorMaxSpeed
        {
            get
            {
                _rotaryMotorMaxSpeed = Convert.ToSingle(_currentAxis.CIPAxis.RotaryMotorMaxSpeed);
                return _rotaryMotorMaxSpeed;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.RotaryMotorMaxSpeed) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.RotaryMotorMaxSpeed = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private ushort _rotaryMotorPoles;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Motor")]
        public ushort RotaryMotorPoles
        {
            get
            {
                _rotaryMotorPoles = Convert.ToUInt16(_currentAxis.CIPAxis.RotaryMotorPoles);
                return _rotaryMotorPoles;
            }
            set
            {
                if (Convert.ToUInt16(_currentAxis.CIPAxis.RotaryMotorPoles) != value)
                {
                    _currentAxis.CIPAxis.RotaryMotorPoles = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _rotaryMotorRatedSpeed;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("RPM")]
        [ReadOnly(false)]
        [Category("Motor")]
        public float RotaryMotorRatedSpeed
        {
            get
            {
                _rotaryMotorRatedSpeed = Convert.ToSingle(_currentAxis.CIPAxis.RotaryMotorRatedSpeed);
                return _rotaryMotorRatedSpeed;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.RotaryMotorRatedSpeed) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.RotaryMotorRatedSpeed = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _linearMotorMaxSpeed;

        // 
        [Browsable(true)]
        [IsChanged(false)]
        [Unit("m/s")]
        [ReadOnly(false)]
        [Category("Motor")]
        public float LinearMotorMaxSpeed
        {
            get
            {
                _linearMotorMaxSpeed = Convert.ToSingle(_currentAxis.CIPAxis.LinearMotorMaxSpeed);
                return _linearMotorMaxSpeed;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.LinearMotorMaxSpeed) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.LinearMotorMaxSpeed = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _linearMotorPolePitch;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("mm")]
        [ReadOnly(false)]
        [Category("Motor")]
        public float LinearMotorPolePitch
        {
            get
            {
                _linearMotorPolePitch = Convert.ToSingle(_currentAxis.CIPAxis.LinearMotorPolePitch);
                return _linearMotorPolePitch;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.LinearMotorPolePitch) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.LinearMotorPolePitch = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _linearMotorRatedSpeed;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("m/s")]
        [ReadOnly(false)]
        [Category("Motor")]
        public float LinearMotorRatedSpeed
        {
            get
            {
                _linearMotorRatedSpeed = Convert.ToSingle(_currentAxis.CIPAxis.LinearMotorRatedSpeed);
                return _linearMotorRatedSpeed;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.LinearMotorRatedSpeed) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.LinearMotorRatedSpeed = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        private float _PMMotorRatedForce;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("N")]
        [ReadOnly(false)]
        [Category("Motor")]
        public float PMMotorRatedForce
        {
            get
            {
                _PMMotorRatedForce = Convert.ToSingle(_currentAxis.CIPAxis.PMMotorRatedForce);
                return _PMMotorRatedForce;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.PMMotorRatedForce) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.PMMotorRatedForce = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Enum Source

        private ObservableCollection<MotorDataSourceType> MotorDataSourceSource { get; set; }

        private ObservableCollection<BooleanTypeA> MotorIntegralThermalSwitchSource { get; set; }
            = new ObservableCollection<BooleanTypeA>
            {
                BooleanTypeA.No,
                BooleanTypeA.Yes
            };

        private ObservableCollection<MotorType> MotorTypeSource { get; set; }

        private ObservableCollection<MotorUnitType> MotorUnitSource { get; set; }
            = new ObservableCollection<MotorUnitType>
            {
                MotorUnitType.Rev,
                MotorUnitType.Meter
            };

        #endregion

        private class Motor
        {
            private readonly AxisCIPParameters _parameters;

            public Motor(AxisCIPParameters parameters)
            {
                _parameters = parameters;
            }

            public void Refresh()
            {
                UpdateMotorDataSource();
                UpdateMotorTypeSource();


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

            private void UpdateMotorDataSource()
            {
                _parameters.MotorDataSourceSource = new ObservableCollection<MotorDataSourceType>
                    {MotorDataSourceType.Datasheet};

                if (_parameters._currentAxis.AssociatedModule != null)
                {
                    _parameters.MotorDataSourceSource.Add(MotorDataSourceType.Database);
                    _parameters.MotorDataSourceSource.Add(MotorDataSourceType.MotorNV);
                }
            }

            public void UpdateMotorTypeSource()
            {
                var supportTypes = new List<MotorType>
                {
                    MotorType.NotSpecified
                };

                var cipMotionDrive = _parameters._currentAxis.AssociatedModule as CIPMotionDrive;
                var axisConfiguration =
                    (AxisConfigurationType) Convert.ToByte(_parameters._currentAxis.CIPAxis.AxisConfiguration);
                if (cipMotionDrive != null)
                {
                    var supportMotorType = cipMotionDrive.GetSupportMotorType(axisConfiguration);

                    if (supportMotorType != null && supportMotorType.Count > 0)
                        supportTypes.AddRange(supportMotorType);
                }

                var oldMotorType = (MotorType) Convert.ToByte(_parameters._currentAxis.CIPAxis.MotorType);

                _parameters.MotorTypeSource = new ObservableCollection<MotorType>(supportTypes);

                if (!supportTypes.Contains(oldMotorType))
                    _parameters.MotorType = supportTypes[0];
                else
                {
                    _parameters._currentAxis.CIPAxis.MotorType = (byte) oldMotorType;
                    _parameters.OnPropertyChanged("MotorType");
                }
            }

            private BaseRule[] GetVisibilityRules(AxisCIPParameters parameters)
            {
                var ruleList = new List<BaseRule>
                {
                    new BaseRule {Name = "MotorDataSource", Value = false},
                    new BaseRule {Name = "MotorUnit", Value = false},
                    new BaseRule {Name = "MotorCatalogNumber", Value = false},
                    new BaseRule {Name = "MotorType", Value = false},
                    new BaseRule {Name = "MotorIntegralThermalSwitch", Value = false},
                    new BaseRule {Name = "MotorMaxWindingTemperature", Value = false},
                    new BaseRule {Name = "MotorOverloadLimit", Value = false},
                    new BaseRule {Name = "MotorRatedContinuousCurrent", Value = false},
                    new BaseRule {Name = "MotorRatedOutputPower", Value = false},
                    new BaseRule {Name = "MotorRatedPeakCurrent", Value = false},
                    new BaseRule {Name = "MotorRatedVoltage", Value = false},
                    new BaseRule {Name = "MotorWindingToAmbientCapacitance", Value = false},
                    new BaseRule {Name = "MotorWindingToAmbientResistance", Value = false},
                    new BaseRule {Name = "PMMotorRatedTorque", Value = false},
                    new BaseRule {Name = "RotaryMotorMaxSpeed", Value = false},
                    new BaseRule {Name = "RotaryMotorPoles", Value = false},
                    new BaseRule {Name = "RotaryMotorRatedSpeed", Value = false},
                    new BaseRule {Name = "LinearMotorMaxSpeed", Value = false},
                    new BaseRule {Name = "LinearMotorPolePitch", Value = false},
                    new BaseRule {Name = "LinearMotorRatedSpeed", Value = false},
                    new BaseRule {Name = "PMMotorRatedForce", Value = false},

                    new BaseRule { Name = "InductionMotorRatedFrequency", Value = false }
                };
                var axisConfiguration =
                    (AxisConfigurationType) Convert.ToByte(parameters._currentAxis.CIPAxis.AxisConfiguration);

                if (axisConfiguration == AxisConfigurationType.FeedbackOnly)
                    return ruleList.ToArray();

                //////
                ruleList.Add(new BaseRule {Name = "MotorDataSource", Value = true});
                ruleList.Add(new BaseRule {Name = "MotorUnit", Value = true});


                // MotorDataSource
                var motorDataSource = parameters.MotorDataSource;
                if ((motorDataSource == MotorDataSourceType.Database) ||
                    (motorDataSource == MotorDataSourceType.Datasheet)
                )
                {
                    ruleList.Add(new BaseRule {Name = "MotorCatalogNumber", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorType", Value = true});
                }

                // MotorType
                var motorType = parameters.MotorType;
                switch (motorType)
                {
                    case MotorType.NotSpecified:
                        break;
                    case MotorType.RotaryPermanentMagnet:
                        ruleList.Add(new BaseRule {Name = "MotorIntegralThermalSwitch", Value = true});
                        ruleList.Add(new BaseRule {Name = "MotorMaxWindingTemperature", Value = true});
                        ruleList.Add(new BaseRule {Name = "MotorOverloadLimit", Value = true});
                        ruleList.Add(new BaseRule {Name = "MotorRatedContinuousCurrent", Value = true});
                        ruleList.Add(new BaseRule {Name = "MotorRatedOutputPower", Value = true});
                        ruleList.Add(new BaseRule {Name = "MotorRatedPeakCurrent", Value = true});
                        ruleList.Add(new BaseRule {Name = "MotorRatedVoltage", Value = true});
                        ruleList.Add(new BaseRule {Name = "MotorWindingToAmbientCapacitance", Value = true});
                        ruleList.Add(new BaseRule {Name = "MotorWindingToAmbientResistance", Value = true});
                        ruleList.Add(new BaseRule {Name = "PMMotorRatedTorque", Value = true});
                        ruleList.Add(new BaseRule {Name = "RotaryMotorMaxSpeed", Value = true});
                        ruleList.Add(new BaseRule {Name = "RotaryMotorPoles", Value = true});
                        ruleList.Add(new BaseRule {Name = "RotaryMotorRatedSpeed", Value = true});
                        break;
                    case MotorType.RotaryInduction:
                        ruleList.Add(new BaseRule { Name = "InductionMotorRatedFrequency", Value = true });
                        ruleList.Add(new BaseRule { Name = "MotorOverloadLimit", Value = true });
                        ruleList.Add(new BaseRule { Name = "MotorRatedContinuousCurrent", Value = true });
                        ruleList.Add(new BaseRule { Name = "MotorRatedOutputPower", Value = true });
                        ruleList.Add(new BaseRule { Name = "MotorRatedVoltage", Value = true });
                        ruleList.Add(new BaseRule { Name = "RotaryMotorMaxSpeed", Value = true });
                        ruleList.Add(new BaseRule { Name = "RotaryMotorPoles", Value = true });
                        ruleList.Add(new BaseRule { Name = "RotaryMotorRatedSpeed", Value = true });

                        CIPMotionDrive motionDrive =
                            parameters._parentViewModel.ModifiedAxisCIPDrive.AssociatedModule as CIPMotionDrive;
                        if (motionDrive != null)
                        {
                            ruleList.Add(new BaseRule
                            {
                                Name = "MotorRatedPeakCurrent",
                                Value = motionDrive.SupportAxisAttribute(axisConfiguration, "MotorRatedPeakCurrent")
                            });
                        }

                        break;
                    case MotorType.LinearPermanentMagnet:
                        ruleList.Add(new BaseRule {Name = "LinearMotorMaxSpeed", Value = true});
                        ruleList.Add(new BaseRule {Name = "LinearMotorPolePitch", Value = true});
                        ruleList.Add(new BaseRule {Name = "LinearMotorRatedSpeed", Value = true});

                        ruleList.Add(new BaseRule {Name = "MotorIntegralThermalSwitch", Value = true});
                        ruleList.Add(new BaseRule {Name = "MotorMaxWindingTemperature", Value = true});
                        ruleList.Add(new BaseRule {Name = "MotorOverloadLimit", Value = true});
                        ruleList.Add(new BaseRule {Name = "MotorRatedContinuousCurrent", Value = true});
                        ruleList.Add(new BaseRule {Name = "MotorRatedOutputPower", Value = true});
                        ruleList.Add(new BaseRule {Name = "MotorRatedPeakCurrent", Value = true});
                        ruleList.Add(new BaseRule {Name = "MotorRatedVoltage", Value = true});
                        ruleList.Add(new BaseRule {Name = "MotorWindingToAmbientCapacitance", Value = true});
                        ruleList.Add(new BaseRule {Name = "MotorWindingToAmbientResistance", Value = true});
                        ruleList.Add(new BaseRule {Name = "PMMotorRatedForce", Value = true});

                        break;
                    case MotorType.LinearInduction:
                        //todo(gjc): add here
                        break;
                    case MotorType.RotaryInteriorPermanentMagnet:
                        //todo(gjc): add here
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
                    ruleList.Add(new BaseRule {Name = "MotorDataSource", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorOverloadLimit", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorCatalogNumber", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorIntegralThermalSwitch", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorMaxWindingTemperature", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorRatedContinuousCurrent", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorRatedOutputPower", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorRatedPeakCurrent", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorRatedVoltage", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorType", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorUnit", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorWindingToAmbientCapacitance", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorWindingToAmbientResistance", Value = true});
                    ruleList.Add(new BaseRule {Name = "PMMotorRatedTorque", Value = true});
                    ruleList.Add(new BaseRule {Name = "RotaryMotorMaxSpeed", Value = true});
                    ruleList.Add(new BaseRule {Name = "RotaryMotorPoles", Value = true});
                    ruleList.Add(new BaseRule {Name = "RotaryMotorRatedSpeed", Value = true});
                    ruleList.Add(new BaseRule {Name = "LinearMotorMaxSpeed", Value = true});
                    ruleList.Add(new BaseRule {Name = "LinearMotorPolePitch", Value = true});
                    ruleList.Add(new BaseRule {Name = "LinearMotorRatedSpeed", Value = true});
                    ruleList.Add(new BaseRule {Name = "PMMotorRatedForce", Value = true});

                    ruleList.Add(new BaseRule { Name = "InductionMotorRatedFrequency", Value = true });
                }
                else
                {
                    ruleList.Add(new BaseRule {Name = "MotorDataSource", Value = false});
                    ruleList.Add(new BaseRule {Name = "MotorOverloadLimit", Value = false});

                    ruleList.Add(new BaseRule {Name = "MotorCatalogNumber", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorIntegralThermalSwitch", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorMaxWindingTemperature", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorRatedContinuousCurrent", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorRatedOutputPower", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorRatedPeakCurrent", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorRatedVoltage", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorType", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorUnit", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorWindingToAmbientCapacitance", Value = true});
                    ruleList.Add(new BaseRule {Name = "MotorWindingToAmbientResistance", Value = true});
                    ruleList.Add(new BaseRule {Name = "PMMotorRatedTorque", Value = true});
                    ruleList.Add(new BaseRule {Name = "RotaryMotorMaxSpeed", Value = true});
                    ruleList.Add(new BaseRule {Name = "RotaryMotorPoles", Value = true});
                    ruleList.Add(new BaseRule {Name = "RotaryMotorRatedSpeed", Value = true});
                    ruleList.Add(new BaseRule {Name = "LinearMotorMaxSpeed", Value = true});
                    ruleList.Add(new BaseRule {Name = "LinearMotorPolePitch", Value = true});
                    ruleList.Add(new BaseRule {Name = "LinearMotorRatedSpeed", Value = true});
                    ruleList.Add(new BaseRule {Name = "PMMotorRatedForce", Value = true});


                    // check IsOnLine
                    if (parameters._parentViewModel.IsOnLine)
                    {
                        ruleList.Add(new BaseRule {Name = "MotorDataSource", Value = true});
                        ruleList.Add(new BaseRule {Name = "MotorOverloadLimit", Value = true});

                        return ruleList.ToArray();
                    }


                    // MotorDataSource
                    var motorDataSource = parameters.MotorDataSource;
                    if (motorDataSource == MotorDataSourceType.MotorNV)
                    {
                        ruleList.Add(new BaseRule {Name = "MotorUnit", Value = false});
                    }
                    else if (motorDataSource == MotorDataSourceType.Datasheet)
                    {
                        ruleList.Add(new BaseRule {Name = "MotorType", Value = false});

                        // MotorType
                        var motorType = parameters.MotorType;
                        switch (motorType)
                        {
                            case MotorType.NotSpecified:
                                ruleList.Add(new BaseRule {Name = "MotorUnit", Value = false});
                                break;
                            case MotorType.RotaryPermanentMagnet:
                                ruleList.Add(new BaseRule {Name = "MotorIntegralThermalSwitch", Value = false});
                                ruleList.Add(new BaseRule {Name = "MotorMaxWindingTemperature", Value = false});
                                ruleList.Add(new BaseRule {Name = "MotorRatedContinuousCurrent", Value = false});
                                ruleList.Add(new BaseRule {Name = "MotorRatedOutputPower", Value = false});
                                ruleList.Add(new BaseRule {Name = "MotorRatedPeakCurrent", Value = false});
                                ruleList.Add(new BaseRule {Name = "MotorRatedVoltage", Value = false});
                                ruleList.Add(new BaseRule {Name = "MotorWindingToAmbientCapacitance", Value = false});
                                ruleList.Add(new BaseRule {Name = "MotorWindingToAmbientResistance", Value = false});
                                ruleList.Add(new BaseRule {Name = "PMMotorRatedTorque", Value = false});
                                ruleList.Add(new BaseRule {Name = "RotaryMotorMaxSpeed", Value = false});
                                ruleList.Add(new BaseRule {Name = "RotaryMotorPoles", Value = false});
                                ruleList.Add(new BaseRule {Name = "RotaryMotorRatedSpeed", Value = false});

                                break;
                            case MotorType.RotaryInduction:
                                ruleList.Add(new BaseRule { Name = "InductionMotorRatedFrequency", Value = false });
                                ruleList.Add(new BaseRule { Name = "MotorRatedContinuousCurrent", Value = false });
                                ruleList.Add(new BaseRule { Name = "MotorRatedOutputPower", Value = false });
                                ruleList.Add(new BaseRule { Name = "MotorRatedVoltage", Value = false });
                                ruleList.Add(new BaseRule { Name = "RotaryMotorMaxSpeed", Value = false });
                                ruleList.Add(new BaseRule { Name = "RotaryMotorPoles", Value = false });
                                ruleList.Add(new BaseRule { Name = "RotaryMotorRatedSpeed", Value = false });

                                break;
                            case MotorType.LinearPermanentMagnet:
                                ruleList.Add(new BaseRule {Name = "LinearMotorMaxSpeed", Value = false});
                                ruleList.Add(new BaseRule {Name = "LinearMotorPolePitch", Value = false});
                                ruleList.Add(new BaseRule {Name = "LinearMotorRatedSpeed", Value = false});

                                ruleList.Add(new BaseRule {Name = "MotorIntegralThermalSwitch", Value = false});
                                ruleList.Add(new BaseRule {Name = "MotorMaxWindingTemperature", Value = false});
                                ruleList.Add(new BaseRule {Name = "MotorRatedContinuousCurrent", Value = false});
                                ruleList.Add(new BaseRule {Name = "MotorRatedOutputPower", Value = false});
                                ruleList.Add(new BaseRule {Name = "MotorRatedPeakCurrent", Value = false});
                                ruleList.Add(new BaseRule {Name = "MotorRatedVoltage", Value = false});
                                ruleList.Add(new BaseRule {Name = "MotorWindingToAmbientCapacitance", Value = false});
                                ruleList.Add(new BaseRule {Name = "MotorWindingToAmbientResistance", Value = false});

                                ruleList.Add(new BaseRule {Name = "PMMotorRatedForce", Value = false});
                                break;
                            case MotorType.LinearInduction:
                                break;
                            case MotorType.RotaryInteriorPermanentMagnet:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }

                return ruleList.ToArray();
            }

            public string[] GetCompareProperties()
            {
                string[] compareProperties =
                {
                    "MotorCatalogNumber", "MotorDataSource", "MotorIntegralThermalSwitch",
                    "MotorMaxWindingTemperature",
                    "MotorOverloadLimit",
                    "MotorRatedContinuousCurrent",
                    "MotorRatedOutputPower",
                    "MotorRatedPeakCurrent",
                    "MotorRatedVoltage",
                    "MotorType", "MotorUnit",
                    "MotorWindingToAmbientCapacitance", "MotorWindingToAmbientResistance",
                    "PMMotorRatedTorque", "RotaryMotorMaxSpeed", "RotaryMotorPoles", "RotaryMotorRatedSpeed",
                    "LinearMotorMaxSpeed", "LinearMotorPolePitch", "LinearMotorRatedSpeed", "PMMotorRatedForce"
                };

                return compareProperties;
            }
        }
    }
}
