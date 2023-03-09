using System.Collections.Generic;
using System.Diagnostics.Contracts;
using ICSStudio.DeviceProfiles.MotionDrive2;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties.ServoDrives
{
    public class ModifiedMotionDrive
    {
        public ModifiedMotionDrive(IController controller, CIPMotionDrive motionDrive)
        {
            Controller = controller;
            OriginalMotionDrive = motionDrive;
            Profiles = motionDrive.Profiles;

            Vendor = motionDrive.Vendor;

            Name = motionDrive.Name;
            Description = motionDrive.Description;
            CatalogNumber = motionDrive.CatalogNumber;

            Contract.Assert(motionDrive.Ports[0].Type == PortType.Ethernet);
            EthernetAddress = motionDrive.Ports[0].Address;

            Major = motionDrive.Major;
            Minor = motionDrive.Minor;
            EKey = motionDrive.EKey;
            PowerStructureID = motionDrive.PowerStructureID;
            Connection = motionDrive.Connection;
            VerifyPowerRating = motionDrive.VerifyPowerRating;

            Inhibited = motionDrive.Inhibited;
            MajorFault = motionDrive.MajorFault;

            ConverterACInputVoltage = motionDrive.ConfigData.ConverterACInputVoltage;
            ConverterACInputPhasing = motionDrive.ConfigData.ConverterACInputPhasing;
            BusConfiguration = motionDrive.ConfigData.BusConfiguration;
            BusSharingGroup = motionDrive.ConfigData.BusSharingGroup;
            //
            BusRegulatorAction = motionDrive.ConfigData.BusRegulatorAction[0];
            ShuntRegulatorResistorType =
                motionDrive.ConfigData.ShuntRegulatorResistorType;
            ExternalShuntRegulatorID = motionDrive.ConfigData.ExternalShuntRegulatorID;

            ConverterThermalOverloadUserLimit = motionDrive.ConfigData.ConverterThermalOverloadUserLimit;
            BusRegulatorThermalOverloadUserLimit = motionDrive.ConfigData.BusRegulatorThermalOverloadUserLimit;
            BusUndervoltageUserLimit = motionDrive.ConfigData.BusUndervoltageUserLimit;


            AssociatedAxes = new ITag[Profiles.Schema.Axes.Count];
            for (int i = 0; i < AssociatedAxes.Length; i++)
                AssociatedAxes[i] = motionDrive.GetAxis(i + 1);

            DigitalInputConfiguration = new List<byte>(motionDrive.ConfigData.DigitalInputConfiguration);

            FeedbackPortSelect = new List<byte>(motionDrive.ConfigData.FeedbackPortSelect);
        }

        public IController Controller { get; }
        public CIPMotionDrive OriginalMotionDrive { get; }
        public MotionDriveProfiles Profiles { get; }

        public int Vendor { get; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string CatalogNumber { get; set; }

        public string EthernetAddress { get; set; }

        public int Major { get; set; }
        public int Minor { get; set; }
        public ElectronicKeyingType EKey { get; set; }
        public int PowerStructureID { get; set; }
        public ConnectionType Connection { get; set; }
        public bool VerifyPowerRating { get; set; }

        public bool Inhibited { get; set; }
        public bool MajorFault { get; set; }

        public ushort ConverterACInputVoltage { get; set; }
        public byte ConverterACInputPhasing { get; set; }
        public byte BusConfiguration { get; set; }
        public byte BusSharingGroup { get; set; }
        public byte BusRegulatorAction { get; set; }
        public byte ShuntRegulatorResistorType { get; set; }
        public short ExternalShuntRegulatorID { get; set; }

        public float ConverterThermalOverloadUserLimit { get; set; }
        public float BusRegulatorThermalOverloadUserLimit { get; set; }
        public float BusUndervoltageUserLimit { get; set; }

        public ITag[] AssociatedAxes { get; }
        public List<byte> DigitalInputConfiguration { get; }
        public List<byte> FeedbackPortSelect { get; }
    }
}
