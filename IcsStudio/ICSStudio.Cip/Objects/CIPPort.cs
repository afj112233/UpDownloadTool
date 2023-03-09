using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Cip.DataTypes;
using ICSStudio.Cip.EtherNetIP;

namespace ICSStudio.Cip.Objects
{
    public enum CIPPortType
    {
        Any = 0,
        EtherNet = 4,
        VendorSpecific
    }
    public class CIPPort : CipBaseObject
    {
        public CIPPort(ICipMessager messager) : base((ushort)CipObjectClassCode.Port, 0, messager)
        {

        }

        public async Task<int> GetPortInstanceInfo()
        {
            var request = GetAttributeSingleRequest(0x09);
            try
            {
                PortInfo.PortInstanceInfos.Clear();
                var response = await Messager.SendRRData(request);
                var data = response.ResponseData;
                int offset = 0;
                //PortInfo.Revision = CipUint.Parse(data, ref offset);
                //PortInfo.MaxInstance = CipUint.Parse(data, ref offset);
                //PortInfo.NumberOfInstances = CipUint.Parse(data, ref offset);
                //PortInfo.EntryPort = CipUint.Parse(data, ref offset);
                while (offset < data.Length)
                {
                    var info = new PortInstanceInfo();
                    info.PortType = CipUint.Parse(data, ref offset);
                    info.PortNumber = CipUint.Parse(data, ref offset);
                    PortInfo.PortInstanceInfos.Add(info);
                }
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }

        public PortInfo PortInfo { private set; get; } = new PortInfo();
    }


    public class PortInfo
    {
        public CipUint Revision { set; get; }

        public CipUint MaxInstance { set; get; }

        public CipUint NumberOfInstances { set; get; }

        public CipUint EntryPort { set; get; }

        public List<PortInstanceInfo> PortInstanceInfos { get; } = new List<PortInstanceInfo>();
    }

    public struct PortInstanceInfo
    {
        public CipUint PortType { set; get; }

        public CipUint PortNumber { set; get; }

        public CIPPortType GetPortType()
        {
            switch (PortType.ToUInt16(null))
            {
                case 0:
                    return CIPPortType.Any;
                case 4:
                    return CIPPortType.EtherNet;
                default:
                    return CIPPortType.VendorSpecific;
            }
        }

        public override string ToString()
        {
            return $"Type:{GetPortType()}:Number:{PortNumber}";
        }
    }
}
