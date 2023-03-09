using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using ICSStudio.Cip.Objects;
using ICSStudio.DeviceProfiles.Common;
using ICSStudio.DeviceProfiles.MotionDrive2.Common;

namespace ICSStudio.DeviceProfiles.MotionDrive2
{
    public class MotionDriveProfiles
    {
        public Identity Identity { get; set; }
        public List<DrivePort> Ports { get; set; }

        public List<string> Categories { get; set; }

        public ExtendedProperties ExtendedProperties { get; set; }

        public MotionDriveModuleTypes ModuleTypes { get; set; }

        public Schema Schema { get; set; }

        public bool SupportAxisAttribute(AxisConfigurationType axisConfiguration, string attribute, int major)
        {
            if (Schema.Attributes?.SupportedAttributes == null)
                return false;

            List<SupportedValue<string>> supportedValueList;
            switch (axisConfiguration)
            {
                case AxisConfigurationType.FeedbackOnly:
                    supportedValueList = Schema.Attributes.SupportedAttributes.FeedbackOnly;
                    break;
                case AxisConfigurationType.FrequencyControl:
                    supportedValueList = Schema.Attributes.SupportedAttributes.FrequencyControl;
                    break;
                case AxisConfigurationType.PositionLoop:
                    supportedValueList = Schema.Attributes.SupportedAttributes.PositionLoop;
                    break;
                case AxisConfigurationType.VelocityLoop:
                    supportedValueList = Schema.Attributes.SupportedAttributes.VelocityLoop;
                    break;
                case AxisConfigurationType.TorqueLoop:
                    supportedValueList = Schema.Attributes.SupportedAttributes.TorqueLoop;
                    break;
                case AxisConfigurationType.ConverterOnly:
                    throw new ArgumentOutOfRangeException(nameof(axisConfiguration), axisConfiguration, null);
                default:
                    throw new ArgumentOutOfRangeException(nameof(axisConfiguration), axisConfiguration, null);
            }

            if (supportedValueList == null)
                return false;

            foreach (var supportedAttribute in supportedValueList)
            {
                if (string.Equals(supportedAttribute.Value, attribute))
                {
                    if (supportedAttribute.MinMajorRev > major)
                        return false;

                    return true;
                }
            }

            return false;
        }

        public bool SupportDriveAttribute(string attribute, int major)
        {
            foreach (var supportedAttribute in Schema.SupportedAttributes)
            {
                if (string.Equals(supportedAttribute.Attribute, attribute))
                {
                    if (supportedAttribute.MinMajorRev > major)
                        return false;

                    return true;
                }
            }

            return false;
        }

        public List<int> GetSupportedMotorFeedbackPortList(int channelNumber, int major)
        {
            if (channelNumber > Schema.Axes.Count)
                return null;

            var channelIndex = channelNumber - 1;
            var motorMasterPorts = Schema.Axes[channelIndex].AllowableFeedbackPorts?.MotorMaster;

            if (motorMasterPorts == null)
                return null;

            //if (motorMasterPorts.MinMajorRev > major)
            //    return null;

            var portList = new List<int>();
            foreach (var portValue in motorMasterPorts)
                //if (portValue.MinMajorRev <= major)
                portList.Add(portValue.Number);

            return portList;
        }

        public List<int> GetSupportedLoadFeedbackPortList(int channelNumber, int major)
        {
            if (channelNumber > Schema.Axes.Count)
                return null;

            var channelIndex = channelNumber - 1;
            var loadPorts = Schema.Axes[channelIndex].AllowableFeedbackPorts.Load;

            if (loadPorts == null)
                return null;

            //if (loadPorts.MinMajorRev > major)
            //    return null;

            var portList = new List<int>();
            foreach (var portValue in loadPorts)
                //if (portValue.MinMajorRev <= major)
                portList.Add(portValue.Number);

            return portList;
        }

        public string GetPortDescription(int portNumber, int lcid = 1033)
        {
            if ((portNumber == 0) || (portNumber > Schema.Feedback.Ports.Count))
                return "<none>";

            foreach (var port in Schema.Feedback.Ports)
                if (port.Number == portNumber)
                    foreach (var descriptionItem in port.Description)
                        if (descriptionItem.LCID == lcid)
                            return descriptionItem.Text;

            return "<Error>";
        }


        public ModuleTypeVariant GetDefaultModuleType()
        {
            Contract.Assert(ModuleTypes != null);

            ModuleType moduleType =
                ModuleTypes.GetModuleType(Identity.VendorID, Identity.ProductType, Identity.ProductCode);

            if (moduleType != null)
            {
                foreach (var typeVariant in moduleType.Variants)
                {
                    if (typeVariant.Default)
                        return typeVariant;
                }
            }

            return null;
        }

        public uint GetConnectionConfig(
            string moduleDefinitionID, string connectionID)
        {
            Contract.Assert(ModuleTypes != null);

            var moduleDefinition = ModuleTypes.GetModuleDefinition(moduleDefinitionID);
            if (moduleDefinition != null)
            {
                foreach (var connectionChoice in moduleDefinition.Connection.Choices)
                {
                    if (connectionChoice.ID == connectionID)
                        return connectionChoice.ConfigID;
                }
            }

            return 0;
        }
    }
}
