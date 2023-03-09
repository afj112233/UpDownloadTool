using System;
using ICSStudio.DeviceProfiles.DIOModule;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.Utils;

namespace ICSStudio.DeviceProperties.Common
{
    public abstract class ModifiedDIOModule
    {
        protected ModifiedDIOModule(IController controller, DeviceModule deviceModule)
        {
            Controller = controller;
            OriginalDeviceModule = deviceModule;

            DiscreteIO discreteIO = deviceModule as DiscreteIO;
            AnalogIO analogIO = deviceModule as AnalogIO;

            if (discreteIO != null)
            {
                Profiles = discreteIO.Profiles;
                ConnectionConfigID = discreteIO.ConfigID;
                Slot = discreteIO.Slot;
            }
            else if (analogIO != null)
            {
                Profiles = analogIO.Profiles;
                ConnectionConfigID = analogIO.ConfigID;
                Slot = analogIO.Slot;
            }
            else
            {
                throw new NotSupportedException("Not supported device!");
            }

            Name = OriginalDeviceModule.Name;
            Description = OriginalDeviceModule.Description;

            Series = OriginalDeviceModule.CatalogNumber.GetSeries();

            Major = OriginalDeviceModule.Major;
            Minor = OriginalDeviceModule.Minor;

            EKey = OriginalDeviceModule.EKey;
            
            Inhibited = OriginalDeviceModule.Inhibited;
            MajorFault = OriginalDeviceModule.MajorFault;

        }

        public IController Controller { get; }
        public DeviceModule OriginalDeviceModule { get; }
        public DIOModuleProfiles Profiles { get; }

        public string Name { get; set; }
        public string Description { get; set; }

        public string Series { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public ElectronicKeyingType EKey { get; set; }
        public uint ConnectionConfigID { get; set; }
        public int Slot { get; set; }

        public uint RPI { get; set; }
        public bool Inhibited { get; set; }
        public bool MajorFault { get; set; }
        public bool Unicast { get; set; }
    }
}
