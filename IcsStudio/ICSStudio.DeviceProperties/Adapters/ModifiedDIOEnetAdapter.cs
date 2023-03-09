using ICSStudio.DeviceProfiles.DIOEnetAdapter;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.Utils;

namespace ICSStudio.DeviceProperties.Adapters
{
    public class ModifiedDIOEnetAdapter
    {
        public ModifiedDIOEnetAdapter(IController controller, CommunicationsAdapter adapter)
        {
            Controller = controller;
            OriginalAdapter = adapter;
            Profiles = adapter.Profiles;

            Name = OriginalAdapter.Name;
            Description = OriginalAdapter.Description;

            var ethernetPort = OriginalAdapter.GetFirstPort(PortType.Ethernet);
            if (ethernetPort != null)
            {
                EthernetAddress = ethernetPort.Address;
            }

            Series = OriginalAdapter.CatalogNumber.GetSeries();

            Major = OriginalAdapter.Major;
            Minor = OriginalAdapter.Minor;

            EKey = OriginalAdapter.EKey;

            ConnectionConfigID = OriginalAdapter.ConfigID;

            ChassisSize = OriginalAdapter.ChassisSize;

            if (OriginalAdapter?.Communications?.Connections != null &&
                OriginalAdapter.Communications.Connections.Count > 0)
            {
                RPI = OriginalAdapter.Communications.Connections[0].RPI;
                Unicast = OriginalAdapter.Communications.Connections[0].Unicast;
            }

            Inhibited = OriginalAdapter.Inhibited;
            MajorFault = OriginalAdapter.MajorFault;
        }

        public IController Controller { get; }
        public CommunicationsAdapter OriginalAdapter { get; }
        public DIOEnetAdapterProfiles Profiles { get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string EthernetAddress { get; set; }
        public string Series { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public ElectronicKeyingType EKey { get; set; }
        public uint ConnectionConfigID { get; set; }
        public int ChassisSize { get; set; }
        public uint RPI { get; set; }
        public bool? Unicast { get; set; }
        public bool Inhibited { get; set; }
        public bool MajorFault { get; set; }
    }
}
