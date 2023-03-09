using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Cip.DataTypes;
using ICSStudio.Cip.EtherNetIP;

namespace ICSStudio.Cip.Objects
{
    public class CIPTcp : CipBaseObject
    {
        public enum ConfigurationMode
        {
            Statically,
            BOOTP,
            DHCP,
            Reserced
        }

        public CIPTcp(ICipMessager messager, int instance = 1) : base((ushort)CipObjectClassCode.TcpIp, instance,
            messager)
        {
        }

        public CipUdint Status { set; get; }
        public CipUdint Capability { set; get; }
        public ConfigurationMode ConfigurationMethod { private set; get; }
        public CipUdint Control { set; get; }
        public PhysicalLineObject PhysicalLineObject { set; get; } = new PhysicalLineObject();
        public Configuration Configuration { set; get; } = new Configuration();
        public CipString HostName { set; get; }
        public CipUsint Pad { set; get; }
        public CipByteArray SafetyNetworkNumber { set; get; }
        public CipUsint TTL { set; get; } = 1;
        public Mcast Mcast { set; get; } = new Mcast();
        public CipBool SelectAcd { set; get; }
        public LastConflictDetected LastConflictDetected { set; get; } = new LastConflictDetected();
        public CipBool QuickConnect { set; get; }
        public CipUint EncapsulationInactivityTimeout { set; get; }
        public CipUint ActiveTCPConnections { set; get; }
        public CipUdint NonCIPEncapsulationMessagesPerSecond { set; get; }

        public async Task<int> GetAttributesAll()
        {
            var request = GetAttributesAllRequest();
            try
            {
                var response = await Messager.SendRRData(request);
                var data = response.ResponseData;
                int offset = 0;
                Status = CipUdint.Parse(data, ref offset);
                Capability = CipUdint.Parse(data, ref offset);
                Control = CipUdint.Parse(data, ref offset);
                var control = Control.ToUInt32(null);
                ConfigurationMethod = (ConfigurationMode)(byte)(control & 15);
                //Physical Link Object
                var size = CipUint.Parse(data, ref offset);
                var path = CipByteArray.Parse(size.ToInt32(null) * 2, data, ref offset);
                PhysicalLineObject = new PhysicalLineObject();
                PhysicalLineObject.Path = path;

                Configuration = new Configuration();
                Configuration.IP = CipUdint.Parse(data, ref offset);
                Configuration.Mask = CipUdint.Parse(data, ref offset);
                Configuration.Gateway = CipUdint.Parse(data, ref offset);
                Configuration.ServerName = CipUdint.Parse(data, ref offset);
                Configuration.ServerName2 = CipUdint.Parse(data, ref offset);
                var length = CipUint.Parse(data, ref offset).ToSByte(null);
                var domain = CipByteArray.Parse(length, data, ref offset);

                if (domain.GetCount() > 0)
                {
                    var name = System.Text.Encoding.Default.GetString(domain.GetBytes());
                    Configuration.DomainName = name;

                    if (Configuration.DomainName.GetString().Length % 2 == 1)
                    {
                        Configuration.Pad = CipUsint.Parse(data, ref offset);
                    }
                }

                length = CipUint.Parse(data, ref offset).ToSByte(null);
                var host = CipByteArray.Parse(length, data, ref offset);
                if (host.GetCount() > 0)
                {
                    var name = Encoding.Default.GetString(host.GetBytes());
                    HostName = name;

                    if (HostName.GetString().Length % 2 == 1)
                    {
                        Pad = CipUsint.Parse(data, ref offset);
                    }
                }


                if (offset >= data.Length) return 1;
                SafetyNetworkNumber = CipByteArray.Parse(6, data, ref offset);
                if (offset >= data.Length) return 1;
                TTL = CipUsint.Parse(data, ref offset);
                if (offset >= data.Length) return 1;
                //Mcast
                Mcast = new Mcast();
                Mcast.AllocControl = CipUsint.Parse(data, ref offset);
                Mcast.Reserved = CipUsint.Parse(data, ref offset);
                Mcast.NumMcast = CipUint.Parse(data, ref offset);
                Mcast.McastStartAddr = CipDint.Parse(data, ref offset);

                //selecteAcd
                if (offset >= data.Length) return 1;
                SelectAcd = CipBool.Parse(data, ref offset);

                //LastConflictDetected
                LastConflictDetected = new LastConflictDetected();
                if (offset >= data.Length) return 1;
                LastConflictDetected.AcdActivity = CipUsint.Parse(data, ref offset);
                LastConflictDetected.RemoteMAC = CipByteArray.Parse(6, data, ref offset);
                LastConflictDetected.ArpPdu = CipByteArray.Parse(28, data, ref offset);

                if (offset >= data.Length) return 1;
                QuickConnect = CipBool.Parse(data, ref offset);

                if (offset >= data.Length) return 1;
                EncapsulationInactivityTimeout = CipUint.Parse(data, ref offset);

                if (offset >= data.Length) return 1;
                ActiveTCPConnections = CipUint.Parse(data, ref offset);

                if (offset >= data.Length) return 1;
                NonCIPEncapsulationMessagesPerSecond = CipUdint.Parse(data, ref offset);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        public async Task<int> SetConfiguration(int ip, int mask, int gateway, int server, int server2, string domain,
            int instance)
        {
            if (Messager.ConnectionStatus == ConnectionStatus.Disconnected)
            {
                var result = await Messager.OnLine(true);
            }

            var data = new List<byte>();
            data.AddRange(BitConverter.GetBytes(ip));
            data.AddRange(BitConverter.GetBytes(mask));
            data.AddRange(BitConverter.GetBytes(gateway));
            data.AddRange(BitConverter.GetBytes(server));
            data.AddRange(BitConverter.GetBytes(server2));
            if (!string.IsNullOrEmpty(domain))
            {
                var nameBuffer = Encoding.ASCII.GetBytes(domain);
                data.AddRange(BitConverter.GetBytes((short)nameBuffer.Length));
                data.AddRange(nameBuffer);
                if (nameBuffer.Length % 2 == 1)
                {
                    data.Add((byte)0);
                }
            }
            else
            {
                data.Add(0);
                data.Add(0);
            }

            var request = SetAttributeSingleRequest(instance, (ushort)0x05, data.ToArray());
            try
            {
                var response = await Messager.SendRRData(request);

                return response.GeneralStatus;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        private IMessageRouterRequest SetAttributeSingleRequest(int instance, ushort attributeId, byte[] data = null)
        {
            var request = new MessageRouterRequest
            {
                Service = (byte)CipServiceCode.SetAttributeSingle,
                RequestPath = new PaddedEPath(ClassId, instance, attributeId),
                RequestData = data
            };

            return request;
        }

        public static string IntToIp(uint ipInt)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append((ipInt >> 24) & 0xFF).Append(".");
            sb.Append((ipInt >> 16) & 0xFF).Append(".");
            sb.Append((ipInt >> 8) & 0xFF).Append(".");
            sb.Append(ipInt & 0xFF);
            return sb.ToString();
        }
    }

    public class PhysicalLineObject
    {
        public int Size => (Path?.GetCount() ?? 0) / 2;
        public CipByteArray Path { get; set; }
    }

    public class Configuration
    {
        public CipUdint IP { set; get; }
        public CipUdint Mask { set; get; }
        public CipUdint Gateway { set; get; }
        public CipUdint ServerName { set; get; }
        public CipUdint ServerName2 { set; get; }
        public CipString DomainName { set; get; }
        public CipUsint Pad { set; get; }
    }

    public class Mcast
    {
        public CipUsint AllocControl { set; get; }
        public CipUsint Reserved { set; get; }
        public CipUint NumMcast { set; get; }
        public CipDint McastStartAddr { set; get; }
    }

    public class LastConflictDetected
    {
        public CipUsint AcdActivity { set; get; }
        public CipByteArray RemoteMAC { set; get; }
        public CipByteArray ArpPdu { set; get; }
    }
}
