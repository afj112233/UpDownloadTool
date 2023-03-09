using System;
using System.Collections.Generic;

namespace ICSStudio.SimpleServices.DeviceModule
{
    [Serializable]
    public class CIPMotionDriveConfigData
    {
        public byte ConfigurationBits { get; set; }
        public byte BusConfiguration { get; set; }
        public List<byte> BusRegulatorAction { get; set; }
        public float BusRegulatorThermalOverloadUserLimit { get; set; }
        public byte BusSharingGroup { get; set; }
        public float BusUndervoltageUserLimit { get; set; }
        public byte ConverterACInputPhasing { get; set; }
        public ushort ConverterACInputVoltage { get; set; }
        public float ConverterThermalOverloadUserLimit { get; set; }
        public List<byte> DigitalInputConfiguration { get; set; }
        public byte EnableTransmissionTimingStatistics { get; set; }
        public short ExternalShuntRegulatorID { get; set; }
        public List<byte> FeedbackPortSelect { get; set; }
        public List<byte> NumberOfConfigurableInputs { get; set; }
        public byte ShuntRegulatorResistorType { get; set; }
    }
}
