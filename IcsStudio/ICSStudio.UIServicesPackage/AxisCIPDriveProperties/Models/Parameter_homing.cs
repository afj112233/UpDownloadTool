using System;
using System.Collections;
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
    internal partial class AxisCIPParameters
    {
        private readonly Homing _homing;

        private const uint HomeSwitchNormallyClosedBits = 0x2;

        #region Public Property

        private HomeDirectionType _homeDirection;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(true)]
        [Category("Homing")]
        public HomeDirectionType HomeDirection
        {
            get
            {
                _homeDirection = (HomeDirectionType) Convert.ToByte(_currentAxis.CIPAxis.HomeDirection);
                return _homeDirection;
            }
            set
            {
                if (Convert.ToByte(_currentAxis.CIPAxis.HomeDirection) != (byte) value)
                {
                    _currentAxis.CIPAxis.HomeDirection = (byte) value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private HomeLimitSwitchType _homeLimitSwitch;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(true)]
        [Category("Homing")]
        public HomeLimitSwitchType HomeLimitSwitch
        {
            get
            {
                var homeConfigurationBits = Convert.ToUInt32(_currentAxis.CIPAxis.HomeConfigurationBits);
                _homeLimitSwitch = (HomeLimitSwitchType) ((homeConfigurationBits & HomeSwitchNormallyClosedBits) >> 1);

                return _homeLimitSwitch;
            }
            set
            {
                var homeConfigurationBits = Convert.ToUInt32(_currentAxis.CIPAxis.HomeConfigurationBits);
                var homeLimitSwitch =
                    (HomeLimitSwitchType) ((homeConfigurationBits & HomeSwitchNormallyClosedBits) >> 1);

                if (homeLimitSwitch != value)
                {
                    if (value == HomeLimitSwitchType.NormallyClosed)
                        homeConfigurationBits = homeConfigurationBits | HomeSwitchNormallyClosedBits;
                    else
                        homeConfigurationBits = homeConfigurationBits & ~HomeSwitchNormallyClosedBits;

                    _currentAxis.CIPAxis.HomeConfigurationBits = homeConfigurationBits;

                    _homing.UpdateHomeLimitSwitchIsChanged();

                    OnPropertyChanged();
                }

            }
        }

        private HomeModeType _homeMode;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Homing")]
        public HomeModeType HomeMode
        {
            get
            {
                _homeMode = (HomeModeType) Convert.ToByte(_currentAxis.CIPAxis.HomeMode);
                return _homeMode;
            }
            set
            {
                if (Convert.ToByte(_currentAxis.CIPAxis.HomeMode) != (byte) value)
                {
                    _currentAxis.CIPAxis.HomeMode = (byte) value;

                    _homing.UpdateHomeSequenceSource();

                    _homing.UpdateReadonly();

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _homeOffset;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Position Units")]
        [ReadOnly(true)]
        [Category("Homing")]
        public float HomeOffset
        {
            get
            {
                _homeOffset = Convert.ToSingle(_currentAxis.CIPAxis.HomeOffset);
                return _homeOffset;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.HomeOffset) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.HomeOffset = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _homePosition;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Position Units")]
        [ReadOnly(false)]
        [Category("Homing")]
        public float HomePosition
        {
            get
            {
                _homePosition = Convert.ToSingle(_currentAxis.CIPAxis.HomePosition);
                return _homePosition;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.HomePosition) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.HomePosition = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }

        private float _homeReturnSpeed;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Position Units/s")]
        [ReadOnly(true)]
        [Category("Homing")]
        public float HomeReturnSpeed
        {
            get
            {
                _homeReturnSpeed = Convert.ToSingle(_currentAxis.CIPAxis.HomeReturnSpeed);
                return _homeReturnSpeed;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.HomeReturnSpeed) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.HomeReturnSpeed = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }

        private HomeSequenceType _homeSequence;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Homing")]
        public HomeSequenceType HomeSequence
        {
            get
            {
                _homeSequence = (HomeSequenceType) Convert.ToByte(_currentAxis.CIPAxis.HomeSequence);
                return _homeSequence;
            }
            set
            {
                if (Convert.ToByte(_currentAxis.CIPAxis.HomeSequence) != (byte) value)
                {
                    _currentAxis.CIPAxis.HomeSequence = (byte) value;

                    _homing.UpdateReadonly();

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }

        private float _homeSpeed;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Position Units/s")]
        [ReadOnly(true)]
        [Category("Homing")]
        public float HomeSpeed
        {
            get
            {
                _homeSpeed = Convert.ToSingle(_currentAxis.CIPAxis.HomeSpeed);
                return _homeSpeed;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.HomeSpeed) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.HomeSpeed = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Enum

        private ObservableCollection<HomeDirectionType> HomeDirectionSource { get; set; }
            = new ObservableCollection<HomeDirectionType>
            {
                HomeDirectionType.BidirectionalForward,
                HomeDirectionType.BidirectionalReverse,
                HomeDirectionType.UnidirectionalForward,
                HomeDirectionType.UnidirectionalReverse
            };

        private ObservableCollection<HomeLimitSwitchType> HomeLimitSwitchSource { get; set; }
            = new ObservableCollection<HomeLimitSwitchType>
            {
                HomeLimitSwitchType.NormallyOpen,
                HomeLimitSwitchType.NormallyClosed
            };

        private ObservableCollection<HomeModeType> HomeModeSource { get; set; }
        private ObservableCollection<HomeSequenceType> HomeSequenceSource { get; set; }

        #endregion

        private class Homing
        {
            private readonly AxisCIPParameters _parameters;

            public Homing(AxisCIPParameters parameters)
            {
                _parameters = parameters;
            }

            public void Refresh()
            {
                UpdateHomeModeSource();
                UpdateHomeSequenceSource();

                UpdateVisibility();
                UpdateReadonly();
                UpdateIsChanged();
            }

            public void UpdateHomeModeSource()
            {
                // rm003,page 102
                var supportTypes = new List<HomeModeType>
                {
                    HomeModeType.Passive
                };

                var axisConfiguration =
                    (AxisConfigurationType) Convert.ToByte(_parameters._currentAxis.CIPAxis.AxisConfiguration);
                switch (axisConfiguration)
                {
                    case AxisConfigurationType.FeedbackOnly:
                        break;
                    case AxisConfigurationType.FrequencyControl:
                        break;
                    case AxisConfigurationType.PositionLoop:
                        supportTypes.Add(HomeModeType.Active);
                        break;
                    case AxisConfigurationType.VelocityLoop:
                        supportTypes.Add(HomeModeType.Active);
                        break;
                    case AxisConfigurationType.TorqueLoop:
                        supportTypes.Add(HomeModeType.Active);
                        break;
                    case AxisConfigurationType.ConverterOnly:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var oldHomeMode = (HomeModeType) Convert.ToByte(_parameters._currentAxis.CIPAxis.HomeMode);

                _parameters.HomeModeSource = new ObservableCollection<HomeModeType>(supportTypes);

                if (!supportTypes.Contains(oldHomeMode))
                    _parameters._currentAxis.CIPAxis.HomeMode = (byte) supportTypes[0];
                else
                {
                    _parameters._currentAxis.CIPAxis.HomeMode = (byte) oldHomeMode;
                    _parameters.OnPropertyChanged("HomeMode");
                }
            }

            public void UpdateHomeSequenceSource()
            {
                IList supportTypes = null;

                var cipMotionDrive = _parameters._currentAxis.AssociatedModule as CIPMotionDrive;
                int axisNumber = _parameters._currentAxis.AxisNumber;
                if (cipMotionDrive != null)
                    supportTypes = cipMotionDrive.GetSupportHomeSequenceTypes(axisNumber);

                if (supportTypes == null)
                    supportTypes = new List<HomeSequenceType>();

                if (!supportTypes.Contains(HomeSequenceType.Immediate))
                    supportTypes.Insert(0, HomeSequenceType.Immediate);

                // special handle
                var axisConfiguration =
                    (AxisConfigurationType) Convert.ToByte(_parameters._currentAxis.CIPAxis.AxisConfiguration);
                var homeMode = (HomeModeType) Convert.ToByte(_parameters._currentAxis.CIPAxis.HomeMode);
                if ((axisConfiguration == AxisConfigurationType.TorqueLoop) && (homeMode == HomeModeType.Active))
                {
                    supportTypes.Clear();
                    supportTypes.Add(HomeSequenceType.Immediate);
                }

                var oldHomeSequence = (HomeSequenceType) Convert.ToByte(_parameters._currentAxis.CIPAxis.HomeSequence);

                _parameters.HomeSequenceSource =
                    new ObservableCollection<HomeSequenceType>((List<HomeSequenceType>) supportTypes);

                if (!supportTypes.Contains(oldHomeSequence))
                    _parameters._currentAxis.CIPAxis.HomeSequence = (byte) supportTypes[0];
                else
                {
                    _parameters._currentAxis.CIPAxis.HomeSequence = (byte) oldHomeSequence;
                    _parameters.OnPropertyChanged("HomeSequence");
                }
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

                UpdateHomeLimitSwitchIsChanged();
            }

            public void UpdateHomeLimitSwitchIsChanged()
            {
                // HomeLimitSwitch
                var homeConfigurationBits0 = Convert.ToUInt32(_parameters._currentAxis.CIPAxis.HomeConfigurationBits);
                var homeLimitSwitch0 =
                    (HomeLimitSwitchType) ((homeConfigurationBits0 & HomeSwitchNormallyClosedBits) >> 1);

                var homeConfigurationBits1 = Convert.ToUInt32(_parameters._originalAxis.CIPAxis.HomeConfigurationBits);
                var homeLimitSwitch1 =
                    (HomeLimitSwitchType) ((homeConfigurationBits1 & HomeSwitchNormallyClosedBits) >> 1);

                PropertySetting.SetPropertyIsChanged(_parameters, "HomeLimitSwitch",
                    homeLimitSwitch0 != homeLimitSwitch1);
            }

            private BaseRule[] GetVisibilityRules(AxisCIPParameters parameters)
            {
                var ruleList = new List<BaseRule>
                {
                    new BaseRule {Name = "HomeDirection", Value = false},
                    new BaseRule {Name = "HomeSpeed", Value = false},
                    new BaseRule {Name = "HomeReturnSpeed", Value = false}
                };

                var axisConfiguration =
                    (AxisConfigurationType) Convert.ToByte(parameters._currentAxis.CIPAxis.AxisConfiguration);
                switch (axisConfiguration)
                {
                    case AxisConfigurationType.FeedbackOnly:
                        break;
                    case AxisConfigurationType.FrequencyControl:
                        break;
                    case AxisConfigurationType.PositionLoop:
                        ruleList.Add(new BaseRule {Name = "HomeDirection", Value = true});
                        ruleList.Add(new BaseRule {Name = "HomeSpeed", Value = true});
                        ruleList.Add(new BaseRule {Name = "HomeReturnSpeed", Value = true});
                        break;
                    case AxisConfigurationType.VelocityLoop:
                        ruleList.Add(new BaseRule {Name = "HomeDirection", Value = true});
                        ruleList.Add(new BaseRule {Name = "HomeSpeed", Value = true});
                        ruleList.Add(new BaseRule {Name = "HomeReturnSpeed", Value = true});
                        break;
                    case AxisConfigurationType.TorqueLoop:
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
                    ruleList.Add(new BaseRule {Name = "HomeMode", Value = true});
                    ruleList.Add(new BaseRule {Name = "HomePosition", Value = true});
                    ruleList.Add(new BaseRule {Name = "HomeSequence", Value = true});
                    ruleList.Add(new BaseRule {Name = "HomeOffset", Value = true});
                    ruleList.Add(new BaseRule {Name = "HomeLimitSwitch", Value = true});
                    ruleList.Add(new BaseRule {Name = "HomeDirection", Value = true});
                    ruleList.Add(new BaseRule {Name = "HomeSpeed", Value = true});
                    ruleList.Add(new BaseRule {Name = "HomeReturnSpeed", Value = true});
                }
                else
                {
                    ruleList.Add(new BaseRule {Name = "HomeMode", Value = false});
                    ruleList.Add(new BaseRule {Name = "HomePosition", Value = false});
                    ruleList.Add(new BaseRule {Name = "HomeSequence", Value = false});
                    ruleList.Add(new BaseRule {Name = "HomeOffset", Value = true});
                    ruleList.Add(new BaseRule {Name = "HomeLimitSwitch", Value = true});
                    ruleList.Add(new BaseRule {Name = "HomeDirection", Value = true});
                    ruleList.Add(new BaseRule {Name = "HomeSpeed", Value = true});
                    ruleList.Add(new BaseRule {Name = "HomeReturnSpeed", Value = true});

                    var homeMode = (HomeModeType) Convert.ToByte(parameters._currentAxis.CIPAxis.HomeMode);
                    var homeSequence = (HomeSequenceType) Convert.ToByte(parameters._currentAxis.CIPAxis.HomeSequence);
                    switch (homeMode)
                    {
                        case HomeModeType.Passive:
                        {
                            switch (homeSequence)
                            {
                                case HomeSequenceType.Immediate:
                                    break;
                                case HomeSequenceType.HomeToSwitch:
                                    ruleList.Add(new BaseRule {Name = "HomeOffset", Value = false});
                                    ruleList.Add(new BaseRule {Name = "HomeLimitSwitch", Value = false});
                                    break;
                                case HomeSequenceType.HomeToMarker:
                                    ruleList.Add(new BaseRule {Name = "HomeOffset", Value = false});
                                    break;
                                case HomeSequenceType.HomeToSwitchThenMarker:
                                    ruleList.Add(new BaseRule {Name = "HomeOffset", Value = false});
                                    ruleList.Add(new BaseRule {Name = "HomeLimitSwitch", Value = false});
                                    break;
                                case HomeSequenceType.HomeToTorque:
                                    break;
                                case HomeSequenceType.HomeToTorqueThenMarker:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                            break;
                        case HomeModeType.Active:
                        {
                            switch (homeSequence)
                            {
                                case HomeSequenceType.Immediate:
                                    break;
                                case HomeSequenceType.HomeToSwitch:
                                    ruleList.Add(new BaseRule {Name = "HomeOffset", Value = false});
                                    ruleList.Add(new BaseRule {Name = "HomeLimitSwitch", Value = false});
                                    ruleList.Add(new BaseRule {Name = "HomeDirection", Value = false});
                                    ruleList.Add(new BaseRule {Name = "HomeSpeed", Value = false});
                                    ruleList.Add(new BaseRule {Name = "HomeReturnSpeed", Value = false});
                                    break;
                                case HomeSequenceType.HomeToMarker:
                                    ruleList.Add(new BaseRule {Name = "HomeOffset", Value = false});
                                    ruleList.Add(new BaseRule {Name = "HomeDirection", Value = false});
                                    ruleList.Add(new BaseRule {Name = "HomeSpeed", Value = false});
                                    ruleList.Add(new BaseRule {Name = "HomeReturnSpeed", Value = false});
                                    break;
                                case HomeSequenceType.HomeToSwitchThenMarker:
                                    ruleList.Add(new BaseRule {Name = "HomeOffset", Value = false});
                                    ruleList.Add(new BaseRule {Name = "HomeLimitSwitch", Value = false});
                                    ruleList.Add(new BaseRule {Name = "HomeDirection", Value = false});
                                    ruleList.Add(new BaseRule {Name = "HomeSpeed", Value = false});
                                    ruleList.Add(new BaseRule {Name = "HomeReturnSpeed", Value = false});
                                    break;
                                case HomeSequenceType.HomeToTorque:
                                    break;
                                case HomeSequenceType.HomeToTorqueThenMarker:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
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
                    "HomeDirection", "HomeMode",
                    "HomeOffset", "HomePosition", "HomeReturnSpeed",
                    "HomeSequence", "HomeSpeed"
                };

                return compareProperties;
            }
        }
    }
}
