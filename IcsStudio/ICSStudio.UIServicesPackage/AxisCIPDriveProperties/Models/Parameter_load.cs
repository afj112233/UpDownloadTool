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
        private readonly Load _load;

        #region Public Property

        private LoadCouplingType _loadCoupling;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Load")]
        public LoadCouplingType LoadCoupling
        {
            get
            {
                _loadCoupling = (LoadCouplingType) Convert.ToByte(_currentAxis.CIPAxis.LoadCoupling);
                return _loadCoupling;
            }
            set
            {
                if (Convert.ToByte(_currentAxis.CIPAxis.LoadCoupling) != (byte) value)
                {
                    _currentAxis.CIPAxis.LoadCoupling = (byte) value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _loadRatio;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Load Inertia/Motor Inertia")]
        [ReadOnly(false)]
        [Category("Load")]
        public float LoadRatio
        {
            get
            {
                _loadRatio = Convert.ToSingle(_currentAxis.CIPAxis.LoadRatio);
                return _loadRatio;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.LoadRatio) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.LoadRatio = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _rotaryMotorInertia;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("kg-m^2")]
        [ReadOnly(false)]
        [Category("Load")]
        public float RotaryMotorInertia
        {
            get
            {
                _rotaryMotorInertia = Convert.ToSingle(_currentAxis.CIPAxis.RotaryMotorInertia);
                return _rotaryMotorInertia;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.RotaryMotorInertia) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.RotaryMotorInertia = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _systemInertia;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("% Rated/(Rev/s^2)")]
        [ReadOnly(true)]
        [Category("Load")]
        public float SystemInertia
        {
            get
            {
                _systemInertia = Convert.ToSingle(_currentAxis.CIPAxis.SystemInertia);
                return _systemInertia;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.SystemInertia) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.SystemInertia = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _torqueOffset;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("% Motor Rated")]
        [ReadOnly(false)]
        [Category("Load")]
        public float TorqueOffset
        {
            get
            {
                _torqueOffset = Convert.ToSingle(_currentAxis.CIPAxis.TorqueOffset);
                return _torqueOffset;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.TorqueOffset) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.TorqueOffset = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _totalInertia;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("kg-m^2")]
        [ReadOnly(true)]
        [Category("Load")]
        public float TotalInertia
        {
            get
            {
                _totalInertia = Convert.ToSingle(_currentAxis.CIPAxis.TotalInertia);
                return _totalInertia;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.TotalInertia) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.TotalInertia = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private bool _useLoadRatio;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Load")]
        public bool UseLoadRatio
        {
            get
            {
                var bits = Convert.ToUInt16(_currentAxis.CIPAxis.GainTuningConfigurationBits);
                _useLoadRatio = FlagsEnumHelper.ContainFlag(bits,
                    GainTuningConfigurationType.UseLoadRatio);
                return _useLoadRatio;
            }
            set
            {
                var bits = Convert.ToUInt16(_currentAxis.CIPAxis.GainTuningConfigurationBits);
                var useLoadRatio = FlagsEnumHelper.ContainFlag(bits,
                    GainTuningConfigurationType.UseLoadRatio);
                if (useLoadRatio != value)
                {
                    FlagsEnumHelper.SelectFlag(GainTuningConfigurationType.UseLoadRatio, value, ref bits);
                    _currentAxis.CIPAxis.GainTuningConfigurationBits = bits;

                    _load.UpdateReadonly();

                    _load.UpdateUseLoadRatioIsChanged();

                    OnPropertyChanged();
                }
            }
        }

        private float _linearMotorMass;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("kg")]
        [ReadOnly(false)]
        [Category("Load")]
        public float LinearMotorMass
        {
            get
            {
                _linearMotorMass = Convert.ToSingle(_currentAxis.CIPAxis.LinearMotorMass);
                return _linearMotorMass;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.LinearMotorMass) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.LinearMotorMass = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _totalMass;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("kg")]
        [ReadOnly(false)]
        [Category("Load")]
        public float TotalMass
        {
            get
            {
                _totalMass = Convert.ToSingle(_currentAxis.CIPAxis.TotalMass);
                return _totalMass;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.TotalMass) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.TotalMass = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Enum

        private ObservableCollection<LoadCouplingType> LoadCouplingSource { get; set; }
            = new ObservableCollection<LoadCouplingType>
            {
                LoadCouplingType.Rigid,
                LoadCouplingType.Compliant
            };


        #endregion

        private class Load
        {
            private readonly AxisCIPParameters _parameters;

            public Load(AxisCIPParameters parameters)
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

                UpdateUseLoadRatioIsChanged();
            }

            public void UpdateUseLoadRatioIsChanged()
            {
                //UseLoadRatio
                var bits1 = Convert.ToUInt16(_parameters._currentAxis.CIPAxis.GainTuningConfigurationBits);
                var useLoadRatio1 = FlagsEnumHelper.ContainFlag(bits1,
                    GainTuningConfigurationType.UseLoadRatio);

                var bits2 = Convert.ToUInt16(_parameters._originalAxis.CIPAxis.GainTuningConfigurationBits);
                var useLoadRatio2 = FlagsEnumHelper.ContainFlag(bits2,
                    GainTuningConfigurationType.UseLoadRatio);

                PropertySetting.SetPropertyIsChanged(_parameters, "UseLoadRatio", useLoadRatio1 != useLoadRatio2);
            }

            private BaseRule[] GetVisibilityRules(AxisCIPParameters parameters)
            {
                var ruleList = new List<BaseRule>
                {
                    new BaseRule {Name = "LoadCoupling", Value = false},
                    new BaseRule {Name = "LoadRatio", Value = false},
                    new BaseRule {Name = "TorqueOffset", Value = false},
                    new BaseRule {Name = "UseLoadRatio", Value = false},
                    new BaseRule {Name = "SystemInertia", Value = false},
                    new BaseRule {Name = "RotaryMotorInertia", Value = false},
                    new BaseRule {Name = "TotalInertia", Value = false},
                    new BaseRule {Name = "LinearMotorMass", Value = false},
                    new BaseRule {Name = "TotalMass", Value = false}
                };

                // AxisConfiguration
                var axisConfiguration =
                    (AxisConfigurationType) Convert.ToByte(parameters._currentAxis.CIPAxis.AxisConfiguration);

                if ((axisConfiguration == AxisConfigurationType.PositionLoop)
                    || (axisConfiguration == AxisConfigurationType.VelocityLoop)
                    || (axisConfiguration == AxisConfigurationType.TorqueLoop))
                {
                    ruleList.Add(new BaseRule {Name = "LoadCoupling", Value = true});
                    ruleList.Add(new BaseRule {Name = "LoadRatio", Value = true});
                    ruleList.Add(new BaseRule {Name = "TorqueOffset", Value = true});
                    ruleList.Add(new BaseRule {Name = "UseLoadRatio", Value = true});

                    if ((axisConfiguration == AxisConfigurationType.PositionLoop) ||
                        (axisConfiguration == AxisConfigurationType.VelocityLoop))
                        ruleList.Add(new BaseRule {Name = "SystemInertia", Value = true});

                    // MotorDataSource, MotorType
                    var motorDataSource =
                        (MotorDataSourceType) Convert.ToByte(parameters._currentAxis.CIPAxis.MotorDataSource);
                    var motorType = (MotorType) Convert.ToByte(parameters._currentAxis.CIPAxis.MotorType);

                    if ((motorDataSource == MotorDataSourceType.Datasheet) ||
                        (motorDataSource == MotorDataSourceType.Database))
                        switch (motorType)
                        {
                            case MotorType.NotSpecified:
                                break;
                            case MotorType.RotaryPermanentMagnet:
                                ruleList.Add(new BaseRule {Name = "RotaryMotorInertia", Value = true});
                                ruleList.Add(new BaseRule {Name = "TotalInertia", Value = true});
                                break;
                            case MotorType.RotaryInduction:
                                break;
                            case MotorType.LinearPermanentMagnet:
                                ruleList.Add(new BaseRule {Name = "LinearMotorMass", Value = true});
                                ruleList.Add(new BaseRule {Name = "TotalMass", Value = true});
                                break;
                            case MotorType.LinearInduction:
                                break;
                            case MotorType.RotaryInteriorPermanentMagnet:
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

                //TODO(gjc): edit later
                if (parameters._parentViewModel.IsPowerStructureEnabled || parameters._parentViewModel.IsOnLine)
                {
                    ruleList.Add(new BaseRule {Name = "LoadCoupling", Value = true});
                    ruleList.Add(new BaseRule {Name = "LoadRatio", Value = true});
                    ruleList.Add(new BaseRule {Name = "RotaryMotorInertia", Value = true});
                    ruleList.Add(new BaseRule {Name = "SystemInertia", Value = true});
                    ruleList.Add(new BaseRule {Name = "TorqueOffset", Value = true});
                    ruleList.Add(new BaseRule {Name = "TotalInertia", Value = true});
                    ruleList.Add(new BaseRule {Name = "UseLoadRatio", Value = true});
                    ruleList.Add(new BaseRule {Name = "LinearMotorMass", Value = true});
                    ruleList.Add(new BaseRule {Name = "TotalMass", Value = true});

                }
                else
                {
                    ruleList.Add(new BaseRule {Name = "LoadCoupling", Value = false});
                    ruleList.Add(new BaseRule {Name = "LoadRatio", Value = true});
                    ruleList.Add(new BaseRule {Name = "RotaryMotorInertia", Value = true});
                    ruleList.Add(new BaseRule {Name = "SystemInertia", Value = true});
                    ruleList.Add(new BaseRule {Name = "TorqueOffset", Value = false});
                    ruleList.Add(new BaseRule {Name = "TotalInertia", Value = true});
                    ruleList.Add(new BaseRule {Name = "UseLoadRatio", Value = true});
                    ruleList.Add(new BaseRule {Name = "LinearMotorMass", Value = true});
                    ruleList.Add(new BaseRule {Name = "TotalMass", Value = true});

                    var motorDataSource =
                        (MotorDataSourceType) Convert.ToByte(parameters._currentAxis.CIPAxis.MotorDataSource);
                    var motorType = (MotorType) Convert.ToByte(parameters._currentAxis.CIPAxis.MotorType);
                    var axisConfiguration =
                        (AxisConfigurationType) Convert.ToByte(parameters._currentAxis.CIPAxis.AxisConfiguration);

                    var useLoadRation = parameters.UseLoadRatio;

                    switch (motorDataSource)
                    {
                        case MotorDataSourceType.Datasheet:
                        case MotorDataSourceType.Database:
                        {
                            if (motorType != MotorType.NotSpecified)
                            {
                                if ((axisConfiguration == AxisConfigurationType.PositionLoop) ||
                                    (axisConfiguration == AxisConfigurationType.VelocityLoop))
                                    ruleList.Add(new BaseRule {Name = "UseLoadRatio", Value = false});

                                if (useLoadRation)
                                {
                                    ruleList.Add(new BaseRule {Name = "LoadRatio", Value = false});
                                    ruleList.Add(new BaseRule {Name = "RotaryMotorInertia", Value = false});
                                    ruleList.Add(new BaseRule {Name = "LinearMotorMass", Value = false});
                                }
                                else
                                {
                                    ruleList.Add(new BaseRule {Name = "TotalInertia", Value = false});
                                    ruleList.Add(new BaseRule {Name = "TotalMass", Value = false});
                                }
                            }
                        }
                            break;
                        case MotorDataSourceType.DriveNV:
                            break;
                        case MotorDataSourceType.MotorNV:
                            ruleList.Add(new BaseRule {Name = "SystemInertia", Value = false});
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                return ruleList.ToArray();
            }

            public string[] GetCompareProperties()
            {
                string[] compareProperties =
                {
                    "LoadCoupling", "LoadRatio",
                    "SystemInertia", "TorqueOffset",
                    "RotaryMotorInertia", "TotalInertia",
                    "LinearMotorMass", "TotalMass"
                };

                return compareProperties;
            }

            public string[] GetPeriodicRefreshProperties()
            {
                string[] periodicRefreshProperties = new[]
                {
                    "TorqueOffset"
                };

                return periodicRefreshProperties;
            }
        }
    }
}
