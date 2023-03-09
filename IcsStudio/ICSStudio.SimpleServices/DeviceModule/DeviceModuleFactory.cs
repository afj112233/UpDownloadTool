using System;
using ICSStudio.Cip;
using ICSStudio.DeviceProfiles;
using ICSStudio.DeviceProfiles.DIOEnetAdapter;
using ICSStudio.DeviceProfiles.DIOModule;
using ICSStudio.DeviceProfiles.Generic;
using ICSStudio.DeviceProfiles.MotionDrive2;
using ICSStudio.Utils;

namespace ICSStudio.SimpleServices.DeviceModule
{
    public class DeviceModuleFactory
    {
        public DeviceModule Create(CipDeviceType productType, string catalogNumber)
        {
            switch (productType)
            {
                case CipDeviceType.CIPMotionDrive:
                    return CreateCIPMotionDrive(catalogNumber);
                case CipDeviceType.CommunicationsAdapter:
                    return CreateCommunicationsAdapter(catalogNumber);
                case CipDeviceType.GeneralPurposeDiscreteIO:
                    return CreateGeneralPurposeDiscreteIO(catalogNumber);
                case CipDeviceType.GeneralPurposeAnalogIO:
                    return CreateGeneralPurposeAnalogIO(catalogNumber);
                case CipDeviceType.ProgrammableLogicController:
                    break;
                case CipDeviceType.EmbeddedComponent:
                    break;

                case CipDeviceType.Generic:
                    if (string.Equals(catalogNumber, "ETHERNET-MODULE", StringComparison.OrdinalIgnoreCase))
                    {
                        return CreateGeneralEthernetModule(catalogNumber);
                    }

                    break;

                //TODO(gjc):add here
            }

            return null;
        }

        private AnalogIO CreateGeneralPurposeAnalogIO(string catalogNumber)
        {
            string dllPath = AssemblyUtils.AssemblyDirectory;
            string folder = dllPath + @"\ModuleProfiles\DIO Analog\";

            var fileName =
                folder + $@"{catalogNumber.RemoveSeries()}.json";

            var analogIOProfiles =
                ProfilesExtension.DeserializeFromFile<DIOModuleProfiles>(fileName);

            var analogModuleTypes =
                ProfilesExtension.DeserializeFromFile<DIOModuleTypes>(
                    folder + analogIOProfiles.PSFile);

            analogIOProfiles.DIOModuleTypes = analogModuleTypes;

            return new AnalogIO(null, analogIOProfiles);
        }

        private DiscreteIO CreateGeneralPurposeDiscreteIO(string catalogNumber)
        {
            string dllPath = AssemblyUtils.AssemblyDirectory;
            string folder = dllPath + @"\ModuleProfiles\DIO Discrete\";

            var fileName =
                folder + $@"{catalogNumber.RemoveSeries()}.json";
            var discreteIOProfiles =
                ProfilesExtension.DeserializeFromFile<DIOModuleProfiles>(fileName);

            var discreteModuleTypes =
                ProfilesExtension.DeserializeFromFile<DIOModuleTypes>(
                    folder + discreteIOProfiles.PSFile);

            discreteIOProfiles.DIOModuleTypes = discreteModuleTypes;

            return new DiscreteIO(null, discreteIOProfiles);
        }

        private CommunicationsAdapter CreateCommunicationsAdapter(string catalogNumber)
        {
            string dllPath = AssemblyUtils.AssemblyDirectory;
            var fileName = dllPath + $@"\ModuleProfiles\DIO Enet Adapter\{catalogNumber}.json";

            var profiles = ProfilesExtension.DeserializeFromFile<DIOEnetAdapterProfiles>(fileName);

            return new CommunicationsAdapter(null, profiles);
        }

        private GeneralEthernet CreateGeneralEthernetModule(string catalogNumber)
        {
            string dllPath = AssemblyUtils.AssemblyDirectory;
            var fileName = dllPath + $@"\ModuleProfiles\Generic\{catalogNumber}.json";

            var profiles = ProfilesExtension.DeserializeFromFile<GenericEnetModuleProfiles>(fileName);

            return new GeneralEthernet(null, profiles);
        }

        private CIPMotionDrive CreateCIPMotionDrive(string catalogNumber)
        {
            string dllPath = AssemblyUtils.AssemblyDirectory;

            string folder = dllPath + @"\ModuleProfiles\MotionDrive\";

            var fileName = folder + $@"{catalogNumber}.json";

            var profiles = ProfilesExtension.DeserializeFromFile<MotionDriveProfiles>(fileName);

            var moduleTypes =
                ProfilesExtension.DeserializeFromFile<MotionDriveModuleTypes>(
                    folder + profiles.ExtendedProperties.PSFile);

            profiles.ModuleTypes = moduleTypes;

            return new CIPMotionDrive(null, profiles);

        }
    }
}
