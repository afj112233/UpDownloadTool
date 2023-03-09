using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.Gui.Utils;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.View;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class AutotuneViewModel : DefaultViewModel
    {
        private string _tuneStatus;

        private LoadParametersTuned _loadParameters;
        private List<ParametersTuned> _loadParametersTunedList;
        private ObservableCollection<ParametersTuned> _loadParametersTunedSource;
        private LoopParametersTuned _loopParameters;
        private List<ParametersTuned> _loopParametersTunedList;
        private ObservableCollection<ParametersTuned> _loopParametersTunedSource;
        private Visibility _isVisibility;

        public AutotuneViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            CompareProperties = new[]
            {
                "ApplicationType", "LoopResponse", "LoadCoupling",
                "GainTuningConfigurationBits",
                "TuningSelect", "TuningTravelLimit", "TuningSpeed", "TuningTorque",
                "TuningDirection"
            };

            LoopParameters = new[]
            {
                "PositionLoopBandwidth",
                "PositionIntegratorBandwidth",
                "VelocityLoopBandwidth",
                "VelocityIntegratorBandwidth",
                "VelocityFeedforwardGain",
                "AccelerationFeedforwardGain",
                "LoadObserverBandwidth",
                "LoadObserverIntegratorBandwidth",
                "TorqueLowPassFilterBandwidth",
                "PositionErrorTolerance",
                "VelocityErrorTolerance",
                "DampingFactor"
            };

            LoadParameters = new[]
            {
                "MaximumAcceleration",
                "MaximumDeceleration",
                "SystemInertia",
                "LoadRatio",
                "RotaryMotorInertia",
                "TotalInertia",
                "TorqueOffset",
                "FrictionCompensationSliding",
                "MaximumAccelerationJerk",
                "MaximumDecelerationJerk",
                "TuneInertiaMass",
                "TuneLoadOffset",
                "TuneFriction",
                "TuneAcceleration",
                "TuneDeceleration",
                "TuneAccelerationTime",
                "TuneDecelerationTime"
            };

            ResizeCommand = new RelayCommand(ExecuteResizeCommand);
            StartCommand = new RelayCommand(ExecuteStartCommand, CanStartCommand);
            StopCommand = new RelayCommand(ExecuteStopCommand, CanStopCommand);
            AcceptTunedValuesCommand = new RelayCommand(
                ExecuteAcceptTunedValuesCommand,
                CanAcceptTunedValuesCommand);


            ApplicationTypeSource = EnumHelper.ToDataSource<ApplicationType>();
            LoopResponseSource = EnumHelper.ToDataSource<LoopResponseType>();
            LoadCouplingSource = EnumHelper.ToDataSource<LoadCouplingType>();
            TuningDirectionSource = EnumHelper.ToDataSource<TuningDirectionType>();

            InitializeLoopParametersTunedSource();
            InitializeLoadParametersTunedSource();
        }

        public Visibility IsVisibility
        {
            get
            {
                return _isVisibility;
            }
            set
            {
                Set(ref _isVisibility, value);
            }
        }

        public string Command => IsVisibility == Visibility.Visible ? "-" : "+";

        public bool IsAutotuneEnabled
        {
            get
            {
                if (ParentViewModel.IsPowerStructureEnabled)
                    return false;

                if (ParentViewModel.IsHardRunMode)
                    return false;

                return true;
            }
        }

        public override void Show()
        {
            UIVisibilityAndReadonly();
            UIRefresh();
        }

        public override Visibility Visibility
        {
            get
            {
                var axisConfiguration =
                    (AxisConfigurationType) Convert.ToByte(ParentViewModel.ModifiedAxisCIPDrive.CIPAxis
                        .AxisConfiguration);

                if (axisConfiguration == AxisConfigurationType.PositionLoop
                    || axisConfiguration == AxisConfigurationType.VelocityLoop)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public string[] LoopParameters { get; }
        public string[] LoadParameters { get; }

        public IList ApplicationTypeSource { get; }

        public ApplicationType ApplicationType
        {
            get { return (ApplicationType) Convert.ToByte(ModifiedCIPAxis.ApplicationType); }
            set
            {
                ModifiedCIPAxis.ApplicationType = (byte) value;

                ParentViewModel.AddEditPropertyName(nameof(ApplicationType));

                // update
                UIVisibilityAndReadonly();

                AxisDefaultSetting.LoadDefaultIntegratorHold(ModifiedCIPAxis, value);

                CheckDirty();
                RaisePropertyChanged();

                ParentViewModel.Refresh();
            }
        }

        public IList LoopResponseSource { get; }

        public LoopResponseType LoopResponse
        {
            get { return (LoopResponseType) Convert.ToByte(ModifiedCIPAxis.LoopResponse); }
            set
            {
                ModifiedCIPAxis.LoopResponse = (byte) value;

                switch (value)
                {
                    case LoopResponseType.Low:
                        ModifiedCIPAxis.DampingFactor = 1.5f;
                        ModifiedCIPAxis.SystemDamping = 1.5f;
                        break;
                    case LoopResponseType.Medium:
                        ModifiedCIPAxis.DampingFactor = 1.0f;
                        ModifiedCIPAxis.SystemDamping = 1.0f;
                        break;
                    case LoopResponseType.High:
                        ModifiedCIPAxis.DampingFactor = 0.8f;
                        ModifiedCIPAxis.SystemDamping = 0.8f;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }

                CheckDirty();
                RaisePropertyChanged();

                ParentViewModel.Refresh();
            }
        }

        public IList LoadCouplingSource { get; }

        public LoadCouplingType LoadCoupling
        {
            get { return (LoadCouplingType) Convert.ToByte(ModifiedCIPAxis.LoadCoupling); }
            set
            {
                ModifiedCIPAxis.LoadCoupling = (byte) value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public bool GainTuningConfigurationEnabled
        {
            get
            {
                if (ApplicationType == ApplicationType.Custom)
                    return true;

                return false;
            }
        }

        public bool TunePosIntegratorChecked
        {
            get
            {
                var bits = Convert.ToUInt16(ModifiedCIPAxis.GainTuningConfigurationBits);
                return FlagsEnumHelper.ContainFlag(bits,
                    GainTuningConfigurationType.TunePosIntegrator);
            }
            set
            {
                var bits = Convert.ToUInt16(ModifiedCIPAxis.GainTuningConfigurationBits);
                FlagsEnumHelper.SelectFlag(GainTuningConfigurationType.TunePosIntegrator, value, ref bits);
                ModifiedCIPAxis.GainTuningConfigurationBits = bits;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public bool TuneVelIntegratorChecked
        {
            get
            {
                var bits = Convert.ToUInt16(ModifiedCIPAxis.GainTuningConfigurationBits);
                return FlagsEnumHelper.ContainFlag(bits,
                    GainTuningConfigurationType.TuneVelIntegrator);
            }
            set
            {
                var bits = Convert.ToUInt16(ModifiedCIPAxis.GainTuningConfigurationBits);
                FlagsEnumHelper.SelectFlag(GainTuningConfigurationType.TuneVelIntegrator, value, ref bits);
                ModifiedCIPAxis.GainTuningConfigurationBits = bits;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public bool TuneVelFeedforwardChecked
        {
            get
            {
                var bits = Convert.ToUInt16(ModifiedCIPAxis.GainTuningConfigurationBits);
                return FlagsEnumHelper.ContainFlag(bits,
                    GainTuningConfigurationType.TuneVelFeedforward);
            }
            set
            {
                var bits = Convert.ToUInt16(ModifiedCIPAxis.GainTuningConfigurationBits);
                FlagsEnumHelper.SelectFlag(GainTuningConfigurationType.TuneVelFeedforward, value, ref bits);
                ModifiedCIPAxis.GainTuningConfigurationBits = bits;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public bool TuneAccelFeedforwardChecked
        {
            get
            {
                var bits = Convert.ToUInt16(ModifiedCIPAxis.GainTuningConfigurationBits);

                return FlagsEnumHelper.ContainFlag(bits,
                    GainTuningConfigurationType.TuneAccelFeedforward);
            }
            set
            {
                var bits = Convert.ToUInt16(ModifiedCIPAxis.GainTuningConfigurationBits);
                FlagsEnumHelper.SelectFlag(GainTuningConfigurationType.TuneAccelFeedforward, value, ref bits);
                ModifiedCIPAxis.GainTuningConfigurationBits = bits;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public bool TuneTorqueLowPassFilterChecked
        {
            get
            {
                var bits = Convert.ToUInt16(ModifiedCIPAxis.GainTuningConfigurationBits);
                return FlagsEnumHelper.ContainFlag(bits,
                    GainTuningConfigurationType.TuneTorqueLowPassFilter);
            }
            set
            {
                var bits = Convert.ToUInt16(ModifiedCIPAxis.GainTuningConfigurationBits);
                FlagsEnumHelper.SelectFlag(GainTuningConfigurationType.TuneTorqueLowPassFilter, value, ref bits);
                ModifiedCIPAxis.GainTuningConfigurationBits = bits;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public bool PerformTuneEnabled
        {
            get
            {
                if (ParentViewModel.IsOnLine)
                    return true;

                return false;
            }
        }

        public bool RunInertiaTestChecked
        {
            get
            {
                var bits = Convert.ToUInt16(ModifiedCIPAxis.GainTuningConfigurationBits);
                return FlagsEnumHelper.ContainFlag(bits,
                    GainTuningConfigurationType.RunInertiaTest);
            }
            set
            {
                var bits = Convert.ToUInt16(ModifiedCIPAxis.GainTuningConfigurationBits);
                FlagsEnumHelper.SelectFlag(GainTuningConfigurationType.RunInertiaTest, value, ref bits);
                ModifiedCIPAxis.GainTuningConfigurationBits = bits;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public TuningSelectType TuningSelect
        {
            get { return (TuningSelectType) Convert.ToByte(ModifiedCIPAxis.TuningSelect); }
            set
            {
                if (Convert.ToByte(ModifiedCIPAxis.TuningSelect) != (byte) value)
                {
                    ModifiedCIPAxis.TuningSelect = (byte) value;

                    ParentViewModel.RaiseAutoApplyPropertyChanged();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float TuningTravelLimit
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.TuningTravelLimit); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedCIPAxis.TuningTravelLimit) - value) > float.Epsilon)
                {
                    ModifiedCIPAxis.TuningTravelLimit = value;

                    ParentViewModel.RaiseAutoApplyPropertyChanged();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float TuningSpeed
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.TuningSpeed); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedCIPAxis.TuningSpeed) - value) > float.Epsilon)
                {
                    ModifiedCIPAxis.TuningSpeed = value;

                    ParentViewModel.RaiseAutoApplyPropertyChanged();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float TuningTorque
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.TuningTorque); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedCIPAxis.TuningTorque) - value) > float.Epsilon)
                {
                    ModifiedCIPAxis.TuningTorque = value;

                    ParentViewModel.RaiseAutoApplyPropertyChanged();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public IList TuningDirectionSource { get; }

        public TuningDirectionType TuningDirection
        {
            get { return (TuningDirectionType) Convert.ToByte(ModifiedCIPAxis.TuningDirection); }
            set
            {
                if (Convert.ToByte(ModifiedCIPAxis.TuningDirection) != (byte) value)
                {
                    ModifiedCIPAxis.TuningDirection = (byte) value;

                    ParentViewModel.RaiseAutoApplyPropertyChanged();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public string PositionUnits => ModifiedCIPAxis.PositionUnits.GetString();

        public string TuneStatus
        {
            get { return _tuneStatus; }
            set { Set(ref _tuneStatus, value); }
        }

        public ObservableCollection<ParametersTuned> LoopParametersTunedSource
        {
            get { return _loopParametersTunedSource; }
            set { Set(ref _loopParametersTunedSource, value); }
        }

        public ObservableCollection<ParametersTuned> LoadParametersTunedSource
        {
            get { return _loadParametersTunedSource; }
            set { Set(ref _loadParametersTunedSource, value); }
        }

        public RelayCommand ResizeCommand { get; }
        public RelayCommand StartCommand { get; }
        public RelayCommand StopCommand { get; }
        public RelayCommand AcceptTunedValuesCommand { get; }

        #region Command

        private bool CanAcceptTunedValuesCommand()
        {
            // TODO(gjc): need edit here
            // TuneStatus
            return true;
        }

        private void ExecuteAcceptTunedValuesCommand()
        {
            // TODO(gjc): add code here
        }

        private bool CanStopCommand()
        {
            // Stop is unavailable:
            // 1.In offline mode.
            // 2.In online mode when the axis is Servo On.
            // 3.The tune is ready to be executed.

            if (!ParentViewModel.IsOnLine)
                return false;

            if (ParentViewModel.IsPowerStructureEnabled)
                return false;

            // TODO(gjc): add code here

            return true;
        }

        private void ExecuteStopCommand()
        {
            // TODO(gjc): add code here
        }

        private bool CanStartCommand()
        {
            // Start is unavailable:
            // 1.In offline mode.
            // 2.In online mode when the axis is Servo On.
            // 3.When a tune is in progress.

            if (!ParentViewModel.IsOnLine)
                return false;

            if (ParentViewModel.IsPowerStructureEnabled)
                return false;

            //TODO(gjc):add code here

            return true;
        }

        private void ExecuteResizeCommand()
        {
            IsVisibility = IsVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            RaisePropertyChanged(nameof(Command));
        }

        private void ExecuteStartCommand()
        {
            //TODO(gjc): add Pending edits apply

            //TODO(gjc): need test
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate { await ExecuteStartCommandAsync(); });
        }

        private async Task ExecuteStartCommandAsync()
        {
            var propertiesCalculation = new PropertiesCalculation(ParentViewModel.ModifiedAxisCIPDrive);
            if (!ParentViewModel.IsOnLine)
                return;

            Controller controller = ParentViewModel.Controller as Controller;
            if (controller == null)
                return;

            int instanceId = controller.GetTagId(ParentViewModel.AxisTag);
            var messager = controller.CipMessager;

            // LoadParametersTuned
            _loadParameters = null;
            _loopParameters = null;

            if (RunInertiaTestChecked)
            {
                // execute Autotune
                var autotuneRequest = MotionDirectCommandHelper.Autotune(instanceId);
                var motionInstruction = new CipMotionInstruction();
                var response = await messager.SendUnitData(autotuneRequest);
                if (response != null && response.GeneralStatus == (byte) CipGeneralStatusCode.Success)
                {
                    if (motionInstruction.Parse(response.ResponseData) == 0)
                    {
                        // query command
                        var queryRequest = MotionDirectCommandHelper.QueryCommandRequest(
                            (ushort) CipObjectClassCode.Axis, instanceId, MotionDirectCommand.AutoTune);
                        var dialog = new AutotuneProgressDialog(messager, queryRequest)
                        {
                            Owner = Application.Current.MainWindow
                        };

                        var dialogResult = dialog.ShowDialog();
                        TuneStatus = dialog.TestState;

                        if (dialogResult != null && dialogResult.Value)
                        {
                            // success
                            // get TuneAccelerationTime and TuneDecelerationTime
                            ModifiedCIPAxis.InstanceId = instanceId;
                            ModifiedCIPAxis.Messager = messager;

                            await ModifiedCIPAxis.GetAttributeList(new[]
                                {"TuneAccelerationTime", "TuneDecelerationTime"});

                            _loadParameters = propertiesCalculation.CalculateLoadParametersTuned2();

                            UpdateLoadParametersTunedSource(_loadParameters);
                        }
                        else
                        {
                            // update???
                            UpdateLoadParametersTunedSource();
                        }
                    }
                    else
                    {
                        Debug.WriteLine("motion instruction parse failed!");
                    }
                }
                else
                {
                    Debug.WriteLine("Execute autotune failed!");
                }

                //TODO(gjc):add code here
            }

            // LoopParametersTuned
            _loopParameters = propertiesCalculation.CalculateLoopParametersTuned2(_loadParameters);
            UpdateLoopParametersTunedSource(_loopParameters);
        }

        #endregion

        #region Private

        private void UIVisibilityAndReadonly()
        {
            RaisePropertyChanged("IsAutotuneEnabled");

            RaisePropertyChanged("PerformTuneEnabled");

            RaisePropertyChanged(nameof(GainTuningConfigurationEnabled));
            
            UpdateGainTuningConfigurationChecked();

            StartCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();
            AcceptTunedValuesCommand.RaiseCanExecuteChanged();
        }

        private void UIRefresh()
        {
            RaisePropertyChanged("ApplicationType");
            RaisePropertyChanged("LoopResponse");
            RaisePropertyChanged("LoadCoupling");

            RaisePropertyChanged("PositionUnits");
        }

        private void UpdateGainTuningConfigurationChecked()
        {
            if(ParentViewModel.HasEditPropertyName(nameof(ApplicationType)))
            {
                switch (ApplicationType)
                {
                    case ApplicationType.Custom:
                        break;
                    case ApplicationType.Basic:
                        TunePosIntegratorChecked = false;
                        TuneVelIntegratorChecked = false;
                        TuneVelFeedforwardChecked = true;
                        TuneAccelFeedforwardChecked = false;
                        TuneTorqueLowPassFilterChecked = true;
                        break;
                    case ApplicationType.Tracking:
                        TunePosIntegratorChecked = false;
                        TuneVelIntegratorChecked = true;
                        TuneVelFeedforwardChecked = true;
                        TuneAccelFeedforwardChecked = true;
                        TuneTorqueLowPassFilterChecked = true;
                        break;
                    case ApplicationType.PointToPoint:
                        TunePosIntegratorChecked = true;
                        TuneVelIntegratorChecked = false;
                        TuneVelFeedforwardChecked = false;
                        TuneAccelFeedforwardChecked = false;
                        TuneTorqueLowPassFilterChecked = true;
                        break;
                    case ApplicationType.ConstantSpeed:
                        TunePosIntegratorChecked = false;
                        TuneVelIntegratorChecked = true;
                        TuneVelFeedforwardChecked = true;
                        TuneAccelFeedforwardChecked = false;
                        TuneTorqueLowPassFilterChecked = true;
                        break;
                }
            }
        }

        private void InitializeLoopParametersTunedSource()
        {
            _loopParametersTunedList = GetParametersTunedList(LoopParameters);

            LoopParametersTunedSource = new ObservableCollection<ParametersTuned>(_loopParametersTunedList);
        }

        private void InitializeLoadParametersTunedSource()
        {
            _loadParametersTunedList = GetParametersTunedList(LoadParameters);

            LoadParametersTunedSource = new ObservableCollection<ParametersTuned>(_loadParametersTunedList);
        }

        private List<ParametersTuned> GetParametersTunedList(string[] parameters)
        {
            var parametersList = new List<ParametersTuned>();

            foreach (var parameter in parameters)
            {
                var parametersTuned = new ParametersTuned
                {
                    //Name = Regex.Replace(parameter, @"\s", "")
                    Name = parameter
                };

                var units = CipAttributeHelper.AttributeNameToUnits<CIPAxis>(parametersTuned.Name);
                if (units.Contains("$Units"))
                {
                    // PositionUnits
                    var positionUnits = ModifiedCIPAxis.PositionUnits.GetString();
                    units = units.Replace("$Units", positionUnits);
                }

                parametersTuned.Units = units;


                parametersList.Add(parametersTuned);
            }

            return parametersList;
        }

        private void UpdateLoadParametersTunedSource(LoadParametersTuned loadParameters = null)
        {
            if (loadParameters != null)
            {
                ModifiedCIPAxis.TuneAcceleration = (float) loadParameters.TuneAcceleration;
                ModifiedCIPAxis.TuneDeceleration = (float) loadParameters.TuneDeceleration;
                ModifiedCIPAxis.TuneInertiaMass = (float) loadParameters.TuneInertiaMass;
                ModifiedCIPAxis.TuneFriction = (float) loadParameters.TuneFriction;
                ModifiedCIPAxis.TuneLoadOffset = (float) loadParameters.TuneLoadOffset;
            }

            UpdateParametersTunedList(_loadParametersTunedList, loadParameters);

            LoadParametersTunedSource = new ObservableCollection<ParametersTuned>(_loadParametersTunedList);
        }

        private void UpdateLoopParametersTunedSource(LoopParametersTuned loopParameters = null)
        {
            UpdateParametersTunedList(_loopParametersTunedList, loopParameters);

            LoopParametersTunedSource = new ObservableCollection<ParametersTuned>(_loopParametersTunedList);
        }

        private void UpdateParametersTunedList<T>(List<ParametersTuned> parametersList, T tunedParameters)
        {
            foreach (var parametersTuned in parametersList)
            {
                var propertyName = parametersTuned.Name;

                // get current value
                double currentValue = GetAxisPropertyValue(propertyName);
                parametersTuned.Current = currentValue.ToString("G");

                if (tunedParameters != null)
                {
                    // get tuned value
                    var tunedValue = GetPropertyValue(tunedParameters, propertyName);
                    parametersTuned.Tuned = tunedValue.ToString("G");

                    // check different
                    parametersTuned.Different =
                        Math.Abs(currentValue - tunedValue) < double.Epsilon
                            ? string.Empty
                            : "*";
                }
                else
                {
                    parametersTuned.Tuned = string.Empty;
                    parametersTuned.Different = string.Empty;
                }
            }
        }

        private float GetAxisPropertyValue(string propertyName)
        {
            var p = typeof(CIPAxis).GetProperty(propertyName);
            return p == null ? default(float) : Convert.ToSingle(p.GetValue(ModifiedCIPAxis));
        }

        private double GetPropertyValue<T>(T parameters, string propertyName)
        {
            var p = typeof(T).GetProperty(propertyName);
            return p == null ? default(float) : Convert.ToDouble(p.GetValue(parameters));
        }

        #endregion

        public class ParametersTuned
        {
            public string Different { get; set; }
            public string Name { get; set; }
            public string Current { get; set; }
            public string Tuned { get; set; }
            public string Units { get; set; }
        }
    }
}