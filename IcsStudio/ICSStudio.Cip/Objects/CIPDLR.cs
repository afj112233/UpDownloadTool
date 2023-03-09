using System.ComponentModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Gui.Converters;

namespace ICSStudio.Cip.Objects
{

    // vol2_1 Table 5-6.2, page 122
    public enum DLRAttributeId : ushort
    {
        NetWorkTopology = 1,
        NetWorkStatus
    }

    [TypeConverter(typeof(EnumMemberValueConverter))]
    public enum NetworkTopologyType : byte
    {
        [EnumMember(Value = "Linear/Star")] Linear = 0,
        [EnumMember(Value = "Ring")] Ring,

        Count
    }

    [TypeConverter(typeof(EnumMemberValueConverter))]
    public enum NetWorkStatusType : byte
    {
        Normal = 0,
        [EnumMember(Value = "Ring Fault")] RingFault,

        [EnumMember(Value = "Unexpected Loop Detected")]
        UnexpectedLoopDetected,

        [EnumMember(Value = "Partial Network Fault")]
        PartialNetworkFault,

        [EnumMember(Value = "Rapid Fault/Restore Cycle")]
        RapidFaultOrRestoreCycle,

        Count
    }

    public class CIPDLR : CipBaseObject
    {
        public CIPDLR(ushort instanceId, ICipMessager messager)
            : base((ushort) CipObjectClassCode.DLR, instanceId, messager)
        {
        }

        public async Task<byte> GetNetworkTopology()
        {
            var request = GetAttributeSingleRequest((ushort) DLRAttributeId.NetWorkTopology);

            var response = await Messager.SendRRData(request);

            if (response != null && response.GeneralStatus == (byte) CipGeneralStatusCode.Success)
            {
                return response.ResponseData[0];
            }

            return byte.MaxValue;
        }

        public async Task<byte> GetNetWorkStatus()
        {
            var request = GetAttributeSingleRequest((ushort) DLRAttributeId.NetWorkStatus);

            var response = await Messager.SendRRData(request);

            if (response != null && response.GeneralStatus == (byte) CipGeneralStatusCode.Success)
            {
                return response.ResponseData[0];
            }

            return byte.MaxValue;
        }
    }
}
