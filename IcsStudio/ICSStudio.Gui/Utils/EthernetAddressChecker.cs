using System.Net;

namespace ICSStudio.Gui.Utils
{
    public enum EthernetAddressType
    {
        PrivateNetwork,
        IPAddress,
        HostName,
    }

    public class EthernetAddressChecker
    {
        public static EthernetAddressType GetAddressType(string address)
        {
            if (string.IsNullOrEmpty(address))
                return EthernetAddressType.IPAddress;

            IPAddress special0 = IPAddress.Parse("192.168.1.0");
            IPAddress special1 = IPAddress.Parse("192.168.1.255");

            IPAddress ipAddress;
            if (IPAddress.TryParse(address, out ipAddress))
            {
                if (address.StartsWith("192.168.1."))
                {
                    if ((!ipAddress.Equals(special0)) && (!ipAddress.Equals(special1)))
                        return EthernetAddressType.PrivateNetwork;
                }

                return EthernetAddressType.IPAddress;
            }

            return EthernetAddressType.HostName;
        }
    }
}
