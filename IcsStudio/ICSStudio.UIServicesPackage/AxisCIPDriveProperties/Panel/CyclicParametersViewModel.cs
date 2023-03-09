using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using ICSStudio.Cip.Objects;
using ICSStudio.Dialogs.Warning;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    [SuppressMessage("ReSharper", "InlineOutVariableDeclaration")]
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class CyclicParametersViewModel : DefaultViewModel
    {
        private ObservableCollection<CycleParameterItem> _parametersToBeReadSource;

        private ObservableCollection<CycleParameterItem> _parametersToBeWrittenSource;

        public CyclicParametersViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            InitializeReadSource();
            InitializeWrittenSource();
        }

        private void InitializeReadSource()
        {
            var supportReadParameters =
                GetSupportCycleReadList((AxisConfigurationType) Convert.ToByte(ModifiedCIPAxis.AxisConfiguration),
                    (FeedbackConfigurationType) Convert.ToByte(ModifiedCIPAxis.FeedbackConfiguration));

            var cyclicReadUpdateList = ParentViewModel.OriginalAxisCIPDrive.CyclicReadUpdateList;

            List<string> validCyclicReadUpdateList = new List<string>();
            if (supportReadParameters != null && cyclicReadUpdateList != null)
                validCyclicReadUpdateList = supportReadParameters.Intersect(cyclicReadUpdateList).ToList();

            ParentViewModel.ModifiedAxisCIPDrive.CyclicReadUpdateList = validCyclicReadUpdateList;

            ParametersToBeReadSource =
                CreateItemSource(supportReadParameters, validCyclicReadUpdateList);
        }

        private void InitializeWrittenSource()
        {
            var supportWriteParameters =
                GetSupportCycleWriteList((AxisConfigurationType) Convert.ToByte(ModifiedCIPAxis.AxisConfiguration));

            var cyclicWriteUpdateList = ParentViewModel.OriginalAxisCIPDrive.CyclicWriteUpdateList;

            List<string> validCyclicWriteUpdateList = new List<string>();
            if (supportWriteParameters != null && cyclicWriteUpdateList != null)
                validCyclicWriteUpdateList = supportWriteParameters.Intersect(cyclicWriteUpdateList).ToList();

            ParentViewModel.ModifiedAxisCIPDrive.CyclicWriteUpdateList = validCyclicWriteUpdateList;

            ParametersToBeWrittenSource = CreateItemSource(supportWriteParameters, validCyclicWriteUpdateList);
        }

        private ObservableCollection<CycleParameterItem> CreateItemSource(
            List<string> supportParameters, List<string> selectParameters)
        {
            ObservableCollection<CycleParameterItem> source = new ObservableCollection<CycleParameterItem>();

            if (supportParameters != null)
            {
                int index = 0;
                foreach (var parameter in supportParameters)
                {
                    var item = new CycleParameterItem(this)
                    {
                        Checked = selectParameters?.Contains(parameter) ?? false,
                        Index = index,
                        Name = parameter,
                        Value = "0.0"
                    };

                    item.CheckedChanged += OnCycleParameterItemCheckedChanged;

                    source.Add(item);

                    index++;
                }
            }


            return source;
        }


        public override void Show()
        {
            UIVisibilityAndReadonly();
            UIRefresh();
        }

        protected override bool PropertiesChanged()
        {
            bool result = base.PropertiesChanged();
            if (result)
                return true;

            if (!StringListEqual.Equal(
                ParentViewModel.OriginalAxisCIPDrive.CyclicReadUpdateList,
                ParentViewModel.ModifiedAxisCIPDrive.CyclicReadUpdateList))
                return true;

            if (!StringListEqual.Equal(
                ParentViewModel.OriginalAxisCIPDrive.CyclicWriteUpdateList,
                ParentViewModel.ModifiedAxisCIPDrive.CyclicWriteUpdateList))
                return true;

            return false;
        }

        public override int CheckValid()
        {
            int result = 0;

            string message = LanguageManager.GetInstance().ConvertSpecifier("Failed to modify") +
                             $" axis '{ParentViewModel.AxisTag.Name}' " +
                             LanguageManager.GetInstance().ConvertSpecifier("properties for.");
            string reason = string.Empty;
            string errorCode = string.Empty;

            if (ParentViewModel.ModifiedAxisCIPDrive.CyclicReadUpdateList.Count > 10)
            {
                reason = "Cyclic Read Attribute is limited to 10 selections.";
                errorCode = "Error 16427-80042FA2";

                result = -1;
            }

            if (result == 0)
            {
                if (ParentViewModel.ModifiedAxisCIPDrive.CyclicWriteUpdateList.Count > 10)
                {
                    reason = "Cyclic Write Attribute is limited to 10 selections.";
                    errorCode = "Error 16427-80042FA3";
                    result = -1;
                }

            }

            // finale
            if (result < 0)
            {
                // show page
                ParentViewModel.ShowPanel("Drive Parameters");

                // show warning 
                var warningDialog =
                    new WarningDialog(
                        message,
                        reason,
                        errorCode)
                    {
                        Owner = Application.Current.MainWindow
                    };
                warningDialog.ShowDialog();
            }

            return result;
        }

        public ObservableCollection<CycleParameterItem> ParametersToBeReadSource
        {
            get { return _parametersToBeReadSource; }
            set { Set(ref _parametersToBeReadSource, value); }
        }

        public ObservableCollection<CycleParameterItem> ParametersToBeWrittenSource
        {
            get { return _parametersToBeWrittenSource; }
            set { Set(ref _parametersToBeWrittenSource, value); }
        }

        public bool IsPowerStructureEnabled => 
            ParentViewModel.IsPowerStructureEnabled || ParentViewModel.IsHardRunMode;

        #region Private

        private void OnCycleParameterItemCheckedChanged(object sender, EventArgs e)
        {
            CycleParameterItem item = sender as CycleParameterItem;
            if (item != null)
            {
                if (ParametersToBeReadSource != null && ParametersToBeReadSource.Contains(item))
                {
                    if (ParentViewModel.ModifiedAxisCIPDrive.CyclicReadUpdateList == null)
                        ParentViewModel.ModifiedAxisCIPDrive.CyclicReadUpdateList = new List<string>();

                    var cyclicReadUpdateList = ParentViewModel.ModifiedAxisCIPDrive.CyclicReadUpdateList;

                    if (item.Checked)
                        cyclicReadUpdateList.Add(item.Name);
                    else
                        cyclicReadUpdateList.Remove(item.Name);

                    cyclicReadUpdateList.Sort();

                }
                else if (ParametersToBeWrittenSource != null && ParametersToBeWrittenSource.Contains(item))
                {
                    if (ParentViewModel.ModifiedAxisCIPDrive.CyclicWriteUpdateList == null)
                        ParentViewModel.ModifiedAxisCIPDrive.CyclicWriteUpdateList = new List<string>();

                    var cyclicWriteUpdateList = ParentViewModel.ModifiedAxisCIPDrive.CyclicWriteUpdateList;

                    if (item.Checked)
                        cyclicWriteUpdateList.Add(item.Name);
                    else
                        cyclicWriteUpdateList.Remove(item.Name);

                    cyclicWriteUpdateList.Sort();

                }

                CheckDirty();

                RaisePropertyChanged();
            }
        }

        private void UpdateParametersToBeReadSource()
        {
            var supportReadParameters =
                GetSupportCycleReadList((AxisConfigurationType) Convert.ToByte(ModifiedCIPAxis.AxisConfiguration),
                    (FeedbackConfigurationType) Convert.ToByte(ModifiedCIPAxis.FeedbackConfiguration));

            //TODO(gjc): check here
            if (supportReadParameters.Count != ParametersToBeReadSource.Count)
            {
                var cyclicReadUpdateList = ParentViewModel.ModifiedAxisCIPDrive.CyclicReadUpdateList;

                var validCyclicReadUpdateList = supportReadParameters.Intersect(cyclicReadUpdateList).ToList();

                ParentViewModel.ModifiedAxisCIPDrive.CyclicReadUpdateList = validCyclicReadUpdateList;

                ParametersToBeReadSource =
                    CreateItemSource(supportReadParameters, validCyclicReadUpdateList);
            }
            else
            {
                foreach (var item in ParametersToBeReadSource)
                {
                    item.RaisePropertyChanged("Visibility");
                    item.RaisePropertyChanged("IsEnabled");
                }
            }
        }

        private void UpdateParametersToBeWrittenSource()
        {
            var supportWriteParameters =
                GetSupportCycleWriteList((AxisConfigurationType) Convert.ToByte(ModifiedCIPAxis.AxisConfiguration));

            if (supportWriteParameters.Count != ParametersToBeWrittenSource.Count)
            {
                var cyclicWriteUpdateList = ParentViewModel.ModifiedAxisCIPDrive.CyclicWriteUpdateList;

                var validCyclicWriteUpdateList = supportWriteParameters.Intersect(cyclicWriteUpdateList).ToList();

                ParentViewModel.ModifiedAxisCIPDrive.CyclicWriteUpdateList = validCyclicWriteUpdateList;

                ParametersToBeWrittenSource = CreateItemSource(supportWriteParameters, validCyclicWriteUpdateList);
            }
            else
            {
                foreach (var item in ParametersToBeWrittenSource)
                {
                    item.RaisePropertyChanged("Visibility");
                    item.RaisePropertyChanged("IsEnabled");
                }
            }

        }

        private List<string> GetSupportCycleReadList(AxisConfigurationType axisConfiguration,
            FeedbackConfigurationType feedbackConfiguration)
        {
            List<string> cycleReadList;

            switch (axisConfiguration)
            {
                case AxisConfigurationType.FeedbackOnly:
                    cycleReadList = new List<string>
                    {
                        "PositionFeedback1", // DINT
                        "VelocityFeedback",
                        "AccelerationFeedback"
                    };
                    break;
                case AxisConfigurationType.FrequencyControl:
                    cycleReadList = new List<string>
                    {
                        "VelocityReference",
                        "VelocityFeedback",
                        "SlipCompensation",
                        "CurrentLimitSource", //DINT
                        "OutputFrequency",
                        "OutputCurrent",
                        "OutputVoltage",
                        "OutputPower",
                        "ConverterOutputCurrent",
                        "ConverterOutputPower",
                        "DCBusVoltage",
                        "MotorCapacity",
                        "InverterCapacity",
                        "ConverterCapacity",
                        "BusRegulatorCapacity"
                    };
                    break;
                case AxisConfigurationType.PositionLoop:
                    cycleReadList = new List<string>
                    {
                        "PositionFineCommand",
                        "PositionReference",
                        "PositionFeedback1", // DINT
                        "PositionError",
                        "PositionIntegratorOutput",
                        "PositionLoopOutput",
                        "VelocityFineCommand",
                        "VelocityFeedforwardCommand",
                        "VelocityReference",
                        "VelocityFeedback",
                        "VelocityError",
                        "VelocityIntegratorOutput",
                        "VelocityLoopOutput",
                        "VelocityLimitSource", //DINT
                        "AccelerationFineCommand",
                        "AccelerationFeedforwardCommand",
                        "AccelerationReference",
                        "AccelerationFeedback",
                        "LoadObserverAccelerationEstimate",
                        "LoadObserverTorqueEstimate",
                        "TorqueReference",
                        "TorqueReferenceFiltered",
                        "TorqueReferenceLimited",
                        "TorqueNotchFilterFrequencyEstimate",
                        "TorqueNotchFilterMagnitudeEstimate",
                        "TorqueLowPassFilterBandwidthEstimate",
                        "AdaptiveTuningGainScalingFactor",
                        "CurrentCommand",
                        "CurrentReference",
                        "CurrentFeedback",
                        "CurrentError",
                        "FluxCurrentReference",
                        "FluxCurrentFeedback",
                        "FluxCurrentError",
                        "OperativeCurrentLimit",
                        "CurrentLimitSource", //DINT
                        "MotorElectricalAngle",
                        "OutputFrequency",
                        "OutputCurrent",
                        "OutputVoltage",
                        "OutputPower",
                        "ConverterOutputCurrent",
                        "ConverterOutputPower",
                        "DCBusVoltage",
                        "MotorCapacity",
                        "InverterCapacity",
                        "ConverterCapacity",
                        "BusRegulatorCapacity"
                    };
                    break;
                case AxisConfigurationType.VelocityLoop:
                    cycleReadList = new List<string>
                    {
                        "PositionFeedback1", // DINT
                        "VelocityFineCommand",
                        "VelocityReference",
                        "VelocityFeedback",
                        "VelocityError",
                        "VelocityIntegratorOutput",
                        "VelocityLoopOutput",
                        "VelocityLimitSource", //DINT
                        "AccelerationFineCommand",
                        "AccelerationFeedforwardCommand",
                        "AccelerationReference",
                        "AccelerationFeedback",
                        "LoadObserverAccelerationEstimate",
                        "LoadObserverTorqueEstimate",
                        "TorqueReference",
                        "TorqueReferenceFiltered",
                        "TorqueReferenceLimited",
                        "TorqueNotchFilterFrequencyEstimate",
                        "TorqueNotchFilterMagnitudeEstimate",
                        "TorqueLowPassFilterBandwidthEstimate",
                        "AdaptiveTuningGainScalingFactor",
                        "CurrentCommand",
                        "CurrentReference",
                        "CurrentFeedback",
                        "CurrentError",
                        "FluxCurrentReference",
                        "FluxCurrentFeedback",
                        "FluxCurrentError",
                        "OperativeCurrentLimit",
                        "CurrentLimitSource", //DINT
                        "MotorElectricalAngle",
                        "OutputFrequency",
                        "OutputCurrent",
                        "OutputVoltage",
                        "OutputPower",
                        "ConverterOutputCurrent",
                        "ConverterOutputPower",
                        "DCBusVoltage",
                        "MotorCapacity",
                        "InverterCapacity",
                        "ConverterCapacity",
                        "BusRegulatorCapacity"
                    };
                    break;
                case AxisConfigurationType.TorqueLoop:
                    cycleReadList = new List<string>
                    {
                        "PositionFeedback1", // DINT
                        "VelocityFeedback",
                        "AccelerationFineCommand",
                        "AccelerationFeedback",
                        "LoadObserverAccelerationEstimate",
                        "LoadObserverTorqueEstimate",
                        "TorqueReference",
                        "TorqueReferenceFiltered",
                        "TorqueReferenceLimited",
                        "TorqueNotchFilterFrequencyEstimate",
                        "TorqueNotchFilterMagnitudeEstimate",
                        "TorqueLowPassFilterBandwidthEstimate",
                        "AdaptiveTuningGainScalingFactor",
                        "CurrentCommand",
                        "CurrentReference",
                        "CurrentFeedback",
                        "CurrentError",
                        "FluxCurrentReference",
                        "FluxCurrentFeedback",
                        "FluxCurrentError",
                        "OperativeCurrentLimit",
                        "CurrentLimitSource",
                        "MotorElectricalAngle",
                        "OutputFrequency",
                        "OutputCurrent",
                        "OutputVoltage",
                        "OutputPower",
                        "ConverterOutputCurrent",
                        "ConverterOutputPower",
                        "DCBusVoltage",
                        "MotorCapacity",
                        "InverterCapacity",
                        "ConverterCapacity",
                        "BusRegulatorCapacity"
                    };
                    break;
                case AxisConfigurationType.ConverterOnly:
                    cycleReadList = new List<string>
                    {
                        "DCBusVoltage"
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axisConfiguration), axisConfiguration, null);
            }

            // PositionFeedback2
            if (axisConfiguration == AxisConfigurationType.PositionLoop ||
                axisConfiguration == AxisConfigurationType.VelocityLoop)
            {
                if (feedbackConfiguration == FeedbackConfigurationType.LoadFeedback ||
                    feedbackConfiguration == FeedbackConfigurationType.DualFeedback)
                {
                    int index = cycleReadList.IndexOf("PositionFeedback1");
                    if (index > 0)
                    {
                        cycleReadList.Insert(index + 1, "PositionFeedback2"); // DINT
                    }
                }
            }

            cycleReadList.Sort();

            // Optional attribute
            string[] optionalAttributes = {"VelocityLimitSource", "CurrentLimitSource" };

            var cipMotionDrive = ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule as CIPMotionDrive;
            if (cipMotionDrive?.Profiles != null)
            {
                foreach (var attribute in optionalAttributes)
                {
                    if (cycleReadList.Contains(attribute))
                    {
                        if (!cipMotionDrive.Profiles.SupportAxisAttribute(
                            axisConfiguration,
                            attribute,
                            cipMotionDrive.Major))
                            cycleReadList.Remove(attribute);
                    }
                }
            }

            return cycleReadList;
        }

        private List<string> GetSupportCycleWriteList(AxisConfigurationType axisConfiguration)
        {
            List<string> cycleWriteList = null;

            switch (axisConfiguration)
            {
                case AxisConfigurationType.FeedbackOnly:
                    cycleWriteList = new List<string>();
                    break;
                case AxisConfigurationType.FrequencyControl:
                    cycleWriteList = new List<string>
                    {
                        "VelocityTrim"
                    };
                    break;
                case AxisConfigurationType.PositionLoop:
                    cycleWriteList = new List<string>
                    {
                        "PositionTrim",
                        "VelocityTrim",
                        "TorqueTrim",
                        "VelocityFeedforwardGain",
                        "AccelerationFeedforwardGain",
                        "PositionLoopBandwidth",
                        "PositionIntegratorBandwidth",
                        "VelocityLoopBandwidth",
                        "VelocityIntegratorBandwidth",
                        "LoadObserverBandwidth",
                        "LoadObserverIntegratorBandwidth",
                        "TorqueLimitPositive",
                        "TorqueLimitNegative",
                        "VelocityLowPassFilterBandwidth",
                        "TorqueLowPassFilterBandwidth",
                        "SystemInertia"
                    };
                    break;
                case AxisConfigurationType.VelocityLoop:
                    cycleWriteList = new List<string>
                    {
                        "VelocityTrim",
                        "TorqueTrim",
                        "AccelerationFeedforwardGain",
                        "VelocityLoopBandwidth",
                        "VelocityIntegratorBandwidth",
                        "LoadObserverBandwidth",
                        "LoadObserverIntegratorBandwidth",
                        "TorqueLimitPositive",
                        "TorqueLimitNegative",
                        "VelocityLowPassFilterBandwidth",
                        "TorqueLowPassFilterBandwidth",
                        "SystemInertia"
                    };
                    break;
                case AxisConfigurationType.TorqueLoop:
                    cycleWriteList = new List<string>
                    {
                        "TorqueTrim",
                        "LoadObserverBandwidth",
                        "LoadObserverIntegratorBandwidth",
                        "TorqueLimitPositive",
                        "TorqueLimitNegative",
                        "TorqueLowPassFilterBandwidth"
                    };
                    break;
                case AxisConfigurationType.ConverterOnly:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axisConfiguration), axisConfiguration, null);
            }

            cycleWriteList?.Sort();

            return cycleWriteList;
        }

        private void RefreshParametersValues(ObservableCollection<CycleParameterItem> parametersSource)
        {
            if (parametersSource != null && parametersSource.Count > 0)
                foreach (var parameterItem in parametersSource)
                {
                    parameterItem.Value = ParentViewModel.AxisTag.GetMemberValue(parameterItem.Name,true);

                    /*
                    var p = typeof(CIPAxis).GetProperty(parameterItem.Name);
                    if (p != null)
                    {
                        //TODO(gjc):need check here!
                        var parameterValue = p.GetValue(OriginalCIPAxis);

                        if (parameterValue is CipDint)
                        {
                            parameterItem.Value = Convert.ToInt32(parameterValue).ToString();
                        }
                        else
                        {
                            float floatValue = Convert.ToSingle(parameterValue);

                            string result =
                                floatValue.ToString("r", CultureInfo.InvariantCulture)
                                    .ToLower(CultureInfo.InvariantCulture);

                            if (result.Contains('e'))
                                result = floatValue.ToString("g9", CultureInfo.InvariantCulture);

                            if (!result.Contains('.'))
                                result += ".0";

                            parameterItem.Value = result;
                        }

                        //parameterItem.Value = parameterValue is CipDint
                        //    ? Convert.ToInt32(parameterValue).ToString()
                        //    : Convert.ToSingle(parameterValue).ToString(CultureInfo.CurrentCulture);

                        //if (parameterItem.Name == "AccelerationFeedforwardGain")
                        //{
                        //    //float accelerationFeedforwardGain = Convert.ToSingle(OriginalCIPAxis.AccelerationFeedforwardGain);
                        //    Debug.WriteLine($"AccelerationFeedforwardGain: {parameterItem.Value}");
                        //}
                    }
                    else
                    {
                        Debug.WriteLine($"Not found {parameterItem.Name}");
                    }
                    */
                }
        }

        private void UIVisibilityAndReadonly()
        {
            UpdateParametersToBeReadSource();
            UpdateParametersToBeWrittenSource();
        }

        private void UIRefresh()
        {
            RefreshParametersValues(ParametersToBeReadSource);
            RefreshParametersValues(ParametersToBeWrittenSource);
        }

        #endregion

    }

    [SuppressMessage("ReSharper", "ArrangeAccessorOwnerBody")]
    internal class CycleParameterItem : ObservableObject
    {
        private readonly CyclicParametersViewModel _parent;
        private bool _checked;

        private string _value;

        public CycleParameterItem(CyclicParametersViewModel parent)
        {
            _parent = parent;
        }

        public int Index { get; set; }

        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (_checked != value)
                {
                    Set(ref _checked, value);
                    CheckedChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public string Name { get; set; }

        public string Value
        {
            get { return _value; }
            set { Set(ref _value, value); }
        }

        public Visibility Visibility
        {
            get
            {
                if (_parent.IsPowerStructureEnabled)
                {
                    return _checked ? Visibility.Visible : Visibility.Collapsed;
                }

                return Visibility.Visible;
            }
        }

        public bool IsEnabled
        {
            get { return !_parent.IsPowerStructureEnabled; }
        }

        public event EventHandler CheckedChanged;
    }
}