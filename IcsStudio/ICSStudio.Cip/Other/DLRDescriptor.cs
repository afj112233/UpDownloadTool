using System.ComponentModel;
using ICSStudio.Cip.Objects;

namespace ICSStudio.Cip.Other
{
    public class DLRDescriptor
    {
        public string GetNetworkTopology(byte topology)
        {
            if (topology > (byte) NetworkTopologyType.Count)
                return string.Empty;

            NetworkTopologyType networkTopology = (NetworkTopologyType) topology;

            return TypeDescriptor.GetConverter(networkTopology).ConvertToString(networkTopology);
        }

        public string GetNetworkStatus(byte status)
        {
            if (status > (byte) NetWorkStatusType.Count)
                return string.Empty;

            NetWorkStatusType networkStatus = (NetWorkStatusType) status;

            return TypeDescriptor.GetConverter(networkStatus).ConvertToString(networkStatus);

        }
    }
}
