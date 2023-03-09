using ICSStudio.DeviceProfiles.Generic;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties.Generic
{
    public class ModifiedGeneralEthernet
    {
        public ModifiedGeneralEthernet(IController controller, GeneralEthernet originalModule)
        {
            Controller = controller;
            OriginalModule = originalModule;
            Profiles = originalModule.Profiles;

            Name = OriginalModule.Name;
            Description = OriginalModule.Description;

            var ethernetPort = OriginalModule.GetFirstPort(PortType.Ethernet);
            if (ethernetPort != null)
            {
                EthernetAddress = ethernetPort.Address;
            }

            CommMethod = OriginalModule.Communications.CommMethod;

            ConfigCxnPoint = OriginalModule.ConfigCxnPoint;
            InputCxnPoint = OriginalModule.Communications.Connections[0].InputCxnPoint;
            OutputCxnPoint = OriginalModule.Communications.Connections[0].OutputCxnPoint;

            ConfigSize = OriginalModule.Communications.ConfigTag.ConfigSize;
            InputSize = OriginalModule.Communications.Connections[0].InputSize;
            OutputSize = OriginalModule.Communications.Connections[0].OutputSize;

            RPI = originalModule.Communications.Connections[0].RPI;
            Inhibited = originalModule.Inhibited;
            MajorFault = originalModule.MajorFault;
            Unicast = originalModule.Communications.Connections[0].Unicast;
        }

        public IController Controller { get; }
        public GeneralEthernet OriginalModule { get; }
        public GenericEnetModuleProfiles Profiles { get; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string EthernetAddress { get; set; }

        public uint CommMethod { get; set; }

        public int ConfigCxnPoint { get; set; }
        public int InputCxnPoint { get; set; }
        public int OutputCxnPoint { get; set; }

        public int ConfigSize { get; set; }
        public int InputSize { get; set; }
        public int OutputSize { get; set; }

        public uint RPI { get; set; }
        public bool Inhibited { get; set; }
        public bool MajorFault { get; set; }
        public bool? Unicast { get; set; }
    }
}
