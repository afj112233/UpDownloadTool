using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Controls.ExtendedPropertyGrid.Attributes;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Models
{
    [SuppressMessage("ReSharper", "NotResolvedInText")]
    internal partial class AxisCIPParameters
    {
        private readonly Actions _actions;

        #region Public Property

        private float _brakeSlipTolerance;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Position Units")]
        [ReadOnly(true)]
        [Category("Actions")]
        public float BrakeSlipTolerance
        {
            get
            {
                _brakeSlipTolerance = Convert.ToSingle(_currentAxis.CIPAxis.BrakeSlipTolerance);
                return _brakeSlipTolerance;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.BrakeSlipTolerance) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.BrakeSlipTolerance = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }

        private float _brakeTestTorque;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("% Motor Rated")]
        [ReadOnly(true)]
        [Category("Actions")]
        public float BrakeTestTorque
        {
            get
            {
                _brakeTestTorque = Convert.ToSingle(_currentAxis.CIPAxis.BrakeTestTorque);
                return _brakeTestTorque;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.BrakeTestTorque) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.BrakeTestTorque = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _coastingTimeLimit;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("s")]
        [ReadOnly(false)]
        [Category("Actions")]
        public float CoastingTimeLimit
        {
            get
            {
                _coastingTimeLimit = Convert.ToSingle(_currentAxis.CIPAxis.CoastingTimeLimit);
                return _coastingTimeLimit;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.CoastingTimeLimit) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.CoastingTimeLimit = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private InverterOverloadActionType _inverterOverloadAction;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Actions")]
        public InverterOverloadActionType InverterOverloadAction
        {
            get
            {
                _inverterOverloadAction =
                    (InverterOverloadActionType)Convert.ToByte(_currentAxis.CIPAxis.InverterOverloadAction);
                return _inverterOverloadAction;
            }
            set
            {
                if (Convert.ToByte(_currentAxis.CIPAxis.InverterOverloadAction) != (byte)value)
                {
                    _currentAxis.CIPAxis.InverterOverloadAction = (byte)value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }

        private MechanicalBrakeControlType _mechanicalBrakeControl;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Actions")]
        public MechanicalBrakeControlType MechanicalBrakeControl
        {
            get
            {
                _mechanicalBrakeControl =
                    (MechanicalBrakeControlType)Convert.ToByte(_currentAxis.CIPAxis.MechanicalBrakeControl);
                return _mechanicalBrakeControl;
            }
            set
            {
                if (Convert.ToByte(_currentAxis.CIPAxis.MechanicalBrakeControl) != (byte)value)
                {
                    _currentAxis.CIPAxis.MechanicalBrakeControl = (byte)value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _mechanicalBrakeEngageDelay;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("s")]
        [ReadOnly(false)]
        [Category("Actions")]
        public float MechanicalBrakeEngageDelay
        {
            get
            {
                _mechanicalBrakeEngageDelay = Convert.ToSingle(_currentAxis.CIPAxis.MechanicalBrakeEngageDelay);
                return _mechanicalBrakeEngageDelay;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.MechanicalBrakeEngageDelay) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.MechanicalBrakeEngageDelay = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _mechanicalBrakeReleaseDelay;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("s")]
        [ReadOnly(false)]
        [Category("Actions")]
        public float MechanicalBrakeReleaseDelay
        {
            get
            {
                _mechanicalBrakeReleaseDelay = Convert.ToSingle(_currentAxis.CIPAxis.MechanicalBrakeReleaseDelay);
                return _mechanicalBrakeReleaseDelay;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.MechanicalBrakeReleaseDelay) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.MechanicalBrakeReleaseDelay = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private MotorOverloadActionType _motorOverloadAction;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Actions")]
        public MotorOverloadActionType MotorOverloadAction
        {
            get
            {
                _motorOverloadAction =
                    (MotorOverloadActionType)Convert.ToByte(_currentAxis.CIPAxis.MotorOverloadAction);
                return _motorOverloadAction;
            }
            set
            {
                if (Convert.ToByte(_currentAxis.CIPAxis.MotorOverloadAction) != (byte)value)
                {
                    _currentAxis.CIPAxis.MotorOverloadAction = (byte)value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private ProgrammedStopModeType _programmedStopMode;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Actions")]
        public ProgrammedStopModeType ProgrammedStopMode
        {
            get
            {
                _programmedStopMode = (ProgrammedStopModeType)Convert.ToByte(_currentAxis.CIPAxis.ProgrammedStopMode);
                return _programmedStopMode;
            }
            set
            {
                if (Convert.ToByte(_currentAxis.CIPAxis.ProgrammedStopMode) != (byte)value)
                {
                    _currentAxis.CIPAxis.ProgrammedStopMode = (byte)value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }

        private BooleanType _provingConfiguration;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Actions")]
        public BooleanType ProvingConfiguration
        {
            get
            {
                _provingConfiguration = (BooleanType)Convert.ToByte(_currentAxis.CIPAxis.ProvingConfiguration);
                return _provingConfiguration;
            }
            set
            {
                if (Convert.ToByte(_currentAxis.CIPAxis.ProvingConfiguration) != (byte)value)
                {
                    _currentAxis.CIPAxis.ProvingConfiguration = (byte)value;

                    _actions.UpdateReadonly();

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private StoppingActionType _stoppingAction;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Actions")]
        public StoppingActionType StoppingAction
        {
            get
            {
                _stoppingAction = (StoppingActionType)Convert.ToByte(_currentAxis.CIPAxis.StoppingAction);
                return _stoppingAction;
            }
            set
            {
                if (Convert.ToByte(_currentAxis.CIPAxis.StoppingAction) != (byte)value)
                {
                    _currentAxis.CIPAxis.StoppingAction = (byte)value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _stoppingTimeLimit;


        [Browsable(true)]
        [IsChanged(false)]
        [Unit("s")]
        [ReadOnly(false)]
        [Category("Actions")]
        public float StoppingTimeLimit
        {
            get
            {
                _stoppingTimeLimit = Convert.ToSingle(_currentAxis.CIPAxis.StoppingTimeLimit);
                return _stoppingTimeLimit;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.StoppingTimeLimit) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.StoppingTimeLimit = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }

        private float _stoppingTorque;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("% Motor Rated")]
        [ReadOnly(false)]
        [Category("Actions")]
        public float StoppingTorque
        {
            get
            {
                _stoppingTorque = Convert.ToSingle(_currentAxis.CIPAxis.StoppingTorque);
                return _stoppingTorque;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.StoppingTorque) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.StoppingTorque = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }

        private float _torqueProveCurrent;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("% Motor Rated")]
        [ReadOnly(true)]
        [Category("Actions")]
        public float TorqueProveCurrent
        {
            get
            {
                _torqueProveCurrent = Convert.ToSingle(_currentAxis.CIPAxis.TorqueProveCurrent);
                return _torqueProveCurrent;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.TorqueProveCurrent) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.TorqueProveCurrent = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _velocityStandstillWindow;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Position Units/s")]
        [ReadOnly(false)]
        [Category("Actions")]
        public float VelocityStandstillWindow
        {
            get
            {
                _velocityStandstillWindow = Convert.ToSingle(_currentAxis.CIPAxis.VelocityStandstillWindow);
                return _velocityStandstillWindow;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.VelocityStandstillWindow) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.VelocityStandstillWindow = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }

        private float _velocityThreshold;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Position Units/s")]
        [ReadOnly(false)]
        [Category("Actions")]
        public float VelocityThreshold
        {
            get
            {
                _velocityThreshold = Convert.ToSingle(_currentAxis.CIPAxis.VelocityThreshold);
                return _velocityThreshold;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.VelocityThreshold) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.VelocityThreshold = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _zeroSpeed;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("% Motor Rated")]
        [ReadOnly(false)]
        [Category("Actions")]
        public float ZeroSpeed
        {
            get
            {
                _zeroSpeed = Convert.ToSingle(_currentAxis.CIPAxis.ZeroSpeed);
                return _zeroSpeed;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.ZeroSpeed) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.ZeroSpeed = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }

        private float _zeroSpeedTime;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("s")]
        [ReadOnly(false)]
        [Category("Actions")]
        public float ZeroSpeedTime
        {
            get
            {
                _zeroSpeedTime = Convert.ToSingle(_currentAxis.CIPAxis.ZeroSpeedTime);
                return _zeroSpeedTime;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.ZeroSpeedTime) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.ZeroSpeedTime = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }

        #endregion

        #region Enum

        private ObservableCollection<InverterOverloadActionType> InverterOverloadActionSource { get; set; }
            = new ObservableCollection<InverterOverloadActionType>
            {
                InverterOverloadActionType.None,
                InverterOverloadActionType.CurrentFoldback
            };

        private ObservableCollection<MechanicalBrakeControlType> MechanicalBrakeControlSource { get; set; }
            = new ObservableCollection<MechanicalBrakeControlType>
            {
                MechanicalBrakeControlType.Automatic,
                MechanicalBrakeControlType.BrakeRelease
            };

        private ObservableCollection<MotorOverloadActionType> MotorOverloadActionSource { get; set; }
            = new ObservableCollection<MotorOverloadActionType>
            {
                MotorOverloadActionType.None,
                MotorOverloadActionType.CurrentFoldback
            };

        private ObservableCollection<ProgrammedStopModeType> ProgrammedStopModeSource { get; set; }
            = new ObservableCollection<ProgrammedStopModeType>
            {
                ProgrammedStopModeType.FastStop,
                ProgrammedStopModeType.FastDisable,
                ProgrammedStopModeType.HardDisable,
                ProgrammedStopModeType.FastShutdown,
                ProgrammedStopModeType.HardShutdown
            };

        private ObservableCollection<BooleanType> ProvingConfigurationSource { get; set; }
            = new ObservableCollection<BooleanType>
            {
                BooleanType.Disabled,
                BooleanType.Enabled
            };

        private ObservableCollection<StoppingActionType> StoppingActionSource { get; set; }

        #endregion

        private class Actions
        {
            private readonly AxisCIPParameters _parameters;

            public Actions(AxisCIPParameters parameters)
            {
                _parameters = parameters;
            }

            public void Refresh()
            {
                UpdateStoppingActionSource();

                UpdateVisibility();
                UpdateReadonly();
                UpdateIsChanged();
            }

            private void UpdateStoppingActionSource()
            {
                var supportTypes = new List<StoppingActionType>
                {
                    StoppingActionType.DisableCoast,
                    StoppingActionType.CurrentDecelDisable
                };

                var axisConfiguration =
                    (AxisConfigurationType)Convert.ToByte(_parameters._currentAxis.CIPAxis.AxisConfiguration);

                if ((axisConfiguration == AxisConfigurationType.PositionLoop) ||
                    (axisConfiguration == AxisConfigurationType.VelocityLoop))
                    supportTypes.Add(StoppingActionType.CurrentDecelHold);

                //keep selected
                var oldStoppingAction =
                    (StoppingActionType)Convert.ToByte(_parameters._currentAxis.CIPAxis.StoppingAction);

                _parameters.StoppingActionSource = new ObservableCollection<StoppingActionType>(supportTypes);

                if (!supportTypes.Contains(oldStoppingAction))
                    // default
                    _parameters._currentAxis.CIPAxis.StoppingAction = (byte)StoppingActionType.CurrentDecelDisable;
                else
                {
                    _parameters._currentAxis.CIPAxis.StoppingAction = (byte)oldStoppingAction;
                    _parameters.OnPropertyChanged("StoppingAction");
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
            }

            private BaseRule[] GetVisibilityRules(AxisCIPParameters parameters)
            {
                var ruleList = new List<BaseRule>
                {
                    new BaseRule { Name = "BrakeSlipTolerance", Value = false },
                    new BaseRule { Name = "BrakeTestTorque", Value = false },
                    new BaseRule { Name = "CoastingTimeLimit", Value = false },
                    new BaseRule { Name = "InverterOverloadAction", Value = false },
                    new BaseRule { Name = "MechanicalBrakeControl", Value = false },
                    new BaseRule { Name = "MechanicalBrakeEngageDelay", Value = false },
                    new BaseRule { Name = "MechanicalBrakeReleaseDelay", Value = false },
                    new BaseRule { Name = "MotorOverloadAction", Value = false },
                    new BaseRule { Name = "ProgrammedStopMode", Value = false },
                    new BaseRule { Name = "ProvingConfiguration", Value = false },
                    new BaseRule { Name = "StoppingAction", Value = false },
                    new BaseRule { Name = "StoppingTimeLimit", Value = false },
                    new BaseRule { Name = "StoppingTorque", Value = false },
                    new BaseRule { Name = "TorqueProveCurrent", Value = false },
                    new BaseRule { Name = "VelocityStandstillWindow", Value = false },
                    new BaseRule { Name = "VelocityThreshold", Value = false },
                    new BaseRule { Name = "ZeroSpeed", Value = false },
                    new BaseRule { Name = "ZeroSpeedTime", Value = false }
                };

                var axisConfiguration =
                    (AxisConfigurationType)Convert.ToByte(parameters._currentAxis.CIPAxis.AxisConfiguration);
                if (axisConfiguration == AxisConfigurationType.FeedbackOnly)
                {
                    ruleList.Add(new BaseRule { Name = "ProgrammedStopMode", Value = true });
                    ruleList.Add(new BaseRule { Name = "VelocityStandstillWindow", Value = true });
                    ruleList.Add(new BaseRule { Name = "VelocityThreshold", Value = true });
                }
                else if ((axisConfiguration == AxisConfigurationType.PositionLoop)
                         || (axisConfiguration == AxisConfigurationType.VelocityLoop)
                         || (axisConfiguration == AxisConfigurationType.TorqueLoop))
                {
                    ruleList.Add(new BaseRule { Name = "BrakeSlipTolerance", Value = true });
                    ruleList.Add(new BaseRule { Name = "BrakeTestTorque", Value = true });
                    ruleList.Add(new BaseRule { Name = "CoastingTimeLimit", Value = true });
                    ruleList.Add(new BaseRule { Name = "InverterOverloadAction", Value = true });
                    ruleList.Add(new BaseRule { Name = "MechanicalBrakeControl", Value = true });
                    ruleList.Add(new BaseRule { Name = "MechanicalBrakeEngageDelay", Value = true });
                    ruleList.Add(new BaseRule { Name = "MechanicalBrakeReleaseDelay", Value = true });
                    ruleList.Add(new BaseRule { Name = "MotorOverloadAction", Value = true });
                    ruleList.Add(new BaseRule { Name = "ProgrammedStopMode", Value = true });
                    ruleList.Add(new BaseRule { Name = "ProvingConfiguration", Value = true });
                    ruleList.Add(new BaseRule { Name = "StoppingAction", Value = true });
                    ruleList.Add(new BaseRule { Name = "StoppingTimeLimit", Value = true });
                    ruleList.Add(new BaseRule { Name = "StoppingTorque", Value = true });
                    ruleList.Add(new BaseRule { Name = "TorqueProveCurrent", Value = true });
                    ruleList.Add(new BaseRule { Name = "VelocityStandstillWindow", Value = true });
                    ruleList.Add(new BaseRule { Name = "VelocityThreshold", Value = true });
                    ruleList.Add(new BaseRule { Name = "ZeroSpeed", Value = true });
                    ruleList.Add(new BaseRule { Name = "ZeroSpeedTime", Value = true });
                }
                else if (axisConfiguration == AxisConfigurationType.FrequencyControl)
                {
                    ruleList.Add(new BaseRule { Name = "CoastingTimeLimit", Value = true });
                    ruleList.Add(new BaseRule { Name = "InverterOverloadAction", Value = true });
                    ruleList.Add(new BaseRule { Name = "MechanicalBrakeControl", Value = true });
                    ruleList.Add(new BaseRule { Name = "MechanicalBrakeEngageDelay", Value = true });
                    ruleList.Add(new BaseRule { Name = "MechanicalBrakeReleaseDelay", Value = true });
                    ruleList.Add(new BaseRule { Name = "MotorOverloadAction", Value = true });
                    ruleList.Add(new BaseRule { Name = "ProgrammedStopMode", Value = true });
                    ruleList.Add(new BaseRule { Name = "StoppingAction", Value = true });
                    ruleList.Add(new BaseRule { Name = "VelocityStandstillWindow", Value = true });
                }

                return ruleList.ToArray();
            }

            private BaseRule[] GetReadonlyRules(AxisCIPParameters parameters)
            {
                List<BaseRule> ruleList = new List<BaseRule>();

                if (parameters._parentViewModel.IsPowerStructureEnabled || parameters._parentViewModel.IsHardRunMode)
                {
                    ruleList.Add(new BaseRule { Name = "BrakeSlipTolerance", Value = true });
                    ruleList.Add(new BaseRule { Name = "BrakeTestTorque", Value = true });
                    ruleList.Add(new BaseRule { Name = "CoastingTimeLimit", Value = true });
                    ruleList.Add(new BaseRule { Name = "InverterOverloadAction", Value = true });
                    ruleList.Add(new BaseRule { Name = "MechanicalBrakeControl", Value = true });
                    ruleList.Add(new BaseRule { Name = "MechanicalBrakeEngageDelay", Value = true });
                    ruleList.Add(new BaseRule { Name = "MechanicalBrakeReleaseDelay", Value = true });
                    ruleList.Add(new BaseRule { Name = "MotorOverloadAction", Value = true });
                    ruleList.Add(new BaseRule { Name = "ProgrammedStopMode", Value = true });
                    ruleList.Add(new BaseRule { Name = "ProvingConfiguration", Value = true });
                    ruleList.Add(new BaseRule { Name = "StoppingAction", Value = true });
                    ruleList.Add(new BaseRule { Name = "StoppingTimeLimit", Value = true });
                    ruleList.Add(new BaseRule { Name = "StoppingTorque", Value = true });
                    ruleList.Add(new BaseRule { Name = "TorqueProveCurrent", Value = true });
                    ruleList.Add(new BaseRule { Name = "VelocityStandstillWindow", Value = true });
                    ruleList.Add(new BaseRule { Name = "VelocityThreshold", Value = true });
                    ruleList.Add(new BaseRule { Name = "ZeroSpeed", Value = true });
                    ruleList.Add(new BaseRule { Name = "ZeroSpeedTime", Value = true });
                }
                else
                {
                    ruleList.Add(new BaseRule { Name = "BrakeSlipTolerance", Value = false });
                    ruleList.Add(new BaseRule { Name = "BrakeTestTorque", Value = false });
                    ruleList.Add(new BaseRule { Name = "CoastingTimeLimit", Value = false });
                    ruleList.Add(new BaseRule { Name = "InverterOverloadAction", Value = false });
                    ruleList.Add(new BaseRule { Name = "MechanicalBrakeControl", Value = false });
                    ruleList.Add(new BaseRule { Name = "MechanicalBrakeEngageDelay", Value = false });
                    ruleList.Add(new BaseRule { Name = "MechanicalBrakeReleaseDelay", Value = false });
                    ruleList.Add(new BaseRule { Name = "MotorOverloadAction", Value = false });
                    ruleList.Add(new BaseRule { Name = "ProgrammedStopMode", Value = false });
                    ruleList.Add(new BaseRule { Name = "ProvingConfiguration", Value = false });
                    ruleList.Add(new BaseRule { Name = "StoppingAction", Value = false });
                    ruleList.Add(new BaseRule { Name = "StoppingTimeLimit", Value = false });
                    ruleList.Add(new BaseRule { Name = "StoppingTorque", Value = false });
                    ruleList.Add(new BaseRule { Name = "TorqueProveCurrent", Value = false });
                    ruleList.Add(new BaseRule { Name = "VelocityStandstillWindow", Value = false });
                    ruleList.Add(new BaseRule { Name = "VelocityThreshold", Value = false });
                    ruleList.Add(new BaseRule { Name = "ZeroSpeed", Value = false });
                    ruleList.Add(new BaseRule { Name = "ZeroSpeedTime", Value = false });

                    // ProvingConfiguration
                    var provingConfiguration =
                        (BooleanType)Convert.ToByte(parameters._currentAxis.CIPAxis.ProvingConfiguration);

                    if (provingConfiguration != BooleanType.Enabled)
                    {
                        ruleList.Add(new BaseRule { Name = "BrakeSlipTolerance", Value = true });
                        ruleList.Add(new BaseRule { Name = "BrakeTestTorque", Value = true });
                        ruleList.Add(new BaseRule { Name = "TorqueProveCurrent", Value = true });
                    }
                }


                return ruleList.ToArray();
            }

            public string[] GetCompareProperties()
            {
                string[] compareProperties =
                {
                    "BrakeSlipTolerance", "BrakeTestTorque",
                    "CoastingTimeLimit", "InverterOverloadAction",
                    "MechanicalBrakeControl", "MechanicalBrakeEngageDelay",
                    "MechanicalBrakeReleaseDelay", "MotorOverloadAction",
                    "ProgrammedStopMode", "ProvingConfiguration",
                    "StoppingAction", "StoppingTimeLimit",
                    "StoppingTorque", "TorqueProveCurrent", "VelocityStandstillWindow",
                    "VelocityThreshold", "ZeroSpeed", "ZeroSpeedTime"
                };

                return compareProperties;
            }

            public string[] GetPeriodicRefreshProperties()
            {
                string[] periodicRefreshProperties = new[]
                {
                    "StoppingTimeLimit",
                    "MechanicalBrakeControl",
                    "MechanicalBrakeEngageDelay",
                    "MechanicalBrakeReleaseDelay"
                };

                return periodicRefreshProperties;
            }
        }
    }
}
