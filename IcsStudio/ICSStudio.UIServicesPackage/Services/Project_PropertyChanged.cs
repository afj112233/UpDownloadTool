using System.Collections.Generic;
using System.ComponentModel;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Tags;

namespace ICSStudio.UIServicesPackage.Services
{
    public partial class Project
    {
        private void OnTaskPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ITask task = sender as ITask;
            if (task == null)
                return;

            List<string> taskCheckPropertyNames = new List<string>()
            {
                "Name", "Description", "Priority", "Watchdog", "Type",
                "IsInhibited", "Rate", "DisableUpdateOutputs"
            };

            Controller myController = Controller as Controller;
            if (myController != null && !myController.IsConnected)
            {
                if (taskCheckPropertyNames.Contains(e.PropertyName))
                {
                    ResetChangeLog();
                }

            }
        }

        private void OnProgramPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IProgram program = sender as IProgram;
            if (program == null)
                return;

            List<string> innerPropertyNames = new List<string>()
            {
                "UpdateRoutineRunStatus"
            };

            Controller myController = Controller as Controller;
            if (myController != null && !myController.IsConnected)
            {

                if (innerPropertyNames.Contains(e.PropertyName))
                {
                    return;
                }

                ResetChangeLog();
            }

        }

        private void OnTagPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ITag tag = sender as ITag;
            if (tag == null)
                return;

            Controller myController = Controller as Controller;
            if (myController != null && !myController.IsConnected)
            {
                if (e.PropertyName.StartsWith($"{tag.Name}."))
                    return;

                if (!IsIgnoreTagProperty(tag, e))
                {
                    Logger.Info($"TagPropertyChanged: {tag.Name}.{e.PropertyName}");

                    ResetChangeLog();
                }
            }
        }

        private bool IsIgnoreTagProperty(ITag tag, PropertyChangedEventArgs e)
        {
            Tag myTag = tag as Tag;
            DataWrapper dataWrapper = myTag?.DataWrapper as AxisCIPDrive;
            if (dataWrapper != null)
            {
                List<string> axisIgnorProperties = new List<string>()
                {
                    "ActualPosition",
                    "CommandPosition",
                    "ActualVelocity",
                    "CommandVelocity",
                    "CIPAxisStatus",
                    "CIPAxisIOStatus",
                    "CIPAxisIOStatusRA",

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

                if (axisIgnorProperties.Contains(e.PropertyName))
                    return true;
            }

            return false;
        }

        private void OnAOIDefinitionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var aoiDefinition = sender as IAoiDefinition;
            if (aoiDefinition != null)
            {
                Logger.Info($"AOIDefinition {aoiDefinition.Name} change property: {e.PropertyName}.");
            }

            ResetChangeLog();

            SetProjectDirty();
        }

        private void OnDataTypePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ResetChangeLog();

            SetProjectDirty();
        }

        private void OnModulePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ResetChangeLog();

            if (!e.PropertyName.Equals("EntryStatus") && !e.PropertyName.Equals("FaultCode") && !e.PropertyName.Equals("FaultInfo"))
                SetProjectDirty();
        }
    }
}
