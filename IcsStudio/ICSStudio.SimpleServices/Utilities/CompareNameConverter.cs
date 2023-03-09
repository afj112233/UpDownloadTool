using System;
using System.Diagnostics.Contracts;
using System.Net;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.SimpleServices.Utilities
{
    public class CompareNameConverter
    {
        public static string EthernetAddressToCompareName(DeviceModule.DeviceModule deviceModule)
        {
            if (deviceModule is LocalModule)
                return $"0{deviceModule.Name}";

            var port = deviceModule.GetFirstPort(PortType.Ethernet);
            Contract.Assert(port != null);

            string address = port.Address;
            if (string.IsNullOrEmpty(address))
                return $"1{deviceModule.Name}";

            IPAddress ipAddress;
            bool isIPAddress = IPAddress.TryParse(port.Address, out ipAddress);

            if (isIPAddress)
            {
                uint value = BitConverter.ToUInt32(ipAddress.GetAddressBytes(), 0);
                return $"2{value:D10}";
            }

            return $"3{address}";
        }
    }
}
