using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Cip.DataTypes;
using ICSStudio.Cip.EtherNetIP;

namespace ICSStudio.Cip.Objects
{
    public class PortConfiguration
    {
        private readonly ICipMessager _conn;
        public PortConfiguration(ICipMessager conn)
        {
            _conn = conn;
        }
        public List<CIPEthernetLinkObject> Ports { get; }=new List<CIPEthernetLinkObject>();
        public async Task GetPortInfo()
        {
            try
            {
                EthernetLinkObjectClassAttr cipClassAttr = new EthernetLinkObjectClassAttr(_conn);
                await cipClassAttr.GetMaxInstance();
                var num = cipClassAttr.MaxInstance.ToUInt16(null);
                for (int i = 1; i <= num; i++)
                {
                    CIPEthernetLinkObject cip = new CIPEthernetLinkObject(i, _conn);
                    Ports.Add(cip);
                    await cip.GetNeedAttributes();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
    public class EthernetLinkObjectClassAttr : CipBaseObject
    {
        public EthernetLinkObjectClassAttr(ICipMessager messager, int instance=0) : base((ushort)CipObjectClassCode.EthernetLink, instance, messager)
        {
        }
        public CipUint MaxInstance { set; get; }
        public async Task<int> GetMaxInstance()
        {
            var request = GetAttributeSingleRequest(2);

            try
            {
                var response = await Messager.SendRRData(request);
                var data = response.ResponseData;
                var offset = 0;
                MaxInstance=CipUint.Parse(data,ref offset);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }
        public async Task<int> GetInterfaceLabel()
        {
            var request = GetAttributeSingleRequest(0x0a);
            try
            {
                var response = await Messager.SendRRData(request);
                var data = response.ResponseData;
                var offset = 0;
                Label = CipShortString.Parse(data, ref offset);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        public CipShortString Label { set; get; }
    }

    public class CIPEthernetLinkObject : CipBaseObject
    {
        public enum EthernetLinkAttributeId
        {
            Speed=1,
            Flags,
            PhysicalAddress,
            InterfaceCounters,
            MediaCounters,
            InterfaceControl,
            InterfaceType,
            InterfaceState,
            AdminState,
            InterfaceLabel,
            InterfaceCapability,
            HCInterfaceCounters,
            HCMediaCounters,
            EthernetErrors,
            LinkDownCounter,
        }
        public enum LinkStatus
        {
            Inactive,
            Active
        }

        public enum Duplex
        {
            Half,
            Full,
        }

        public enum NegotiationStatus
        {
            AutoNegotiationInProgress,
            AutoNegotiationAndSpeedDetectionFailed,
            AutoNegotiationFailedButDetectedSpeed,
            NegotiationSpeedAndDuplexSuccessfully,
            AutoNegotiationNotAttempted
        }

        public enum LocalHardwareFault
        {
            NoFault,
            Fault
        }

        public enum State
        {
            Unknown,
            Enable,
            Disable,
            Testing,
            Reserved
        }

        public CIPEthernetLinkObject(int instanceId, ICipMessager messager) : base(
            (ushort) CipObjectClassCode.EthernetLink,
            instanceId, messager)
        {
        }

        public uint Speed => _speed.ToUInt32(null);
        private CipUdint _speed;
        private CipUdint _flags;
        public LinkStatus Status { private set; get; }
        public Duplex DuplexStatus { private set; get; }
        public NegotiationStatus Negotiation { private set; get; }
        public LocalHardwareFault HardwareFault { private set; get; }
        public CipByteArray PhysicalAddress { set; get; }
        public Counter InterfaceCounter { set; get; }
        public MediaSpecialCounter MediaCounter { set; get; }
        public ConfigurationForPhysical ConfigurationForPhysical { set; get; }
        public CipUsint InterfaceType { set; get; }

        public State InterfaceState
        {
            get
            {
                switch (_interfaceState.ToSByte(null))
                {
                    case 0:
                        return State.Unknown;
                    case 1:
                        return  State.Enable;
                    case 2:
                        return State.Disable;
                    case 3:
                       return State.Testing;
                    default:
                        return State.Reserved;
                }
            }
        }
        private CipUsint _interfaceState;
        public CipUsint AdminState { set; get; }
        public CipShortString InterfaceLabel { set; get; }
        public InterfaceCapability InterfaceCapability { set; get; }
        //暂时只支持这些属性
        public async Task GetNeedAttributes()
        {
            await GetFlag();
            await GetSpeed();
            await GetLabel();
            await GetState();
        }

        //public async Task<int> GetAttributesAll()
        //{
        //    var request = GetAttributesAllRequest();
        //    try
        //    {
        //        var response = await Messager.SendRRData(request);
        //        var data = response.ResponseData;
        //        int offset = 0;

        //        _speed = CipUdint.Parse(data, ref offset);
        //        _flags = CipUdint.Parse(data, ref offset);
        //        ParseFlag();
        //        PhysicalAddress = CipByteArray.Parse(6, data, ref offset);

        //        InterfaceCounter = new Counter();
        //        InterfaceCounter.InOctets = CipUdint.Parse(data, ref offset);
        //        InterfaceCounter.InUcastPackets = CipUdint.Parse(data, ref offset);
        //        InterfaceCounter.InNUcastPackets = CipUdint.Parse(data, ref offset);
        //        InterfaceCounter.InDiscards = CipUdint.Parse(data, ref offset);
        //        InterfaceCounter.InErrors = CipUdint.Parse(data, ref offset);
        //        InterfaceCounter.InUnknownProtos = CipUdint.Parse(data, ref offset);
        //        InterfaceCounter.OutOctets = CipUdint.Parse(data, ref offset);
        //        InterfaceCounter.OutUcastPackets = CipUdint.Parse(data, ref offset);
        //        InterfaceCounter.OutNUcastPackets = CipUdint.Parse(data, ref offset);
        //        InterfaceCounter.OutDiscards = CipUdint.Parse(data, ref offset);
        //        InterfaceCounter.OutErrors = CipUdint.Parse(data, ref offset);

        //        MediaCounter = new MediaSpecialCounter();
        //        MediaCounter.AlignmentErrors = CipUdint.Parse(data, ref offset);
        //        MediaCounter.FCSErrors = CipUdint.Parse(data, ref offset);
        //        MediaCounter.SingleCollisions = CipUdint.Parse(data, ref offset);
        //        MediaCounter.MultipleCollisions = CipUdint.Parse(data, ref offset);
        //        MediaCounter.SQETestErrors = CipUdint.Parse(data, ref offset);
        //        MediaCounter.DeferredTransmissions = CipUdint.Parse(data, ref offset);
        //        MediaCounter.LateCollisions = CipUdint.Parse(data, ref offset);
        //        MediaCounter.ExcessiveCollision = CipUdint.Parse(data, ref offset);
        //        MediaCounter.MACTransmitErrors = CipUdint.Parse(data, ref offset);
        //        MediaCounter.CarrierSenseErrors = CipUdint.Parse(data, ref offset);
        //        MediaCounter.FrameTooLong = CipUdint.Parse(data, ref offset);
        //        MediaCounter.MACReceiveErrors = CipUdint.Parse(data, ref offset);

        //        ConfigurationForPhysical = new ConfigurationForPhysical();
        //        ConfigurationForPhysical.ControlBits = CipUint.Parse(data, ref offset);
        //        ConfigurationForPhysical.ForceInterfaceSpeed = CipUint.Parse(data, ref offset);

        //        InterfaceType = CipUsint.Parse(data, ref offset);
        //        _interfaceState = CipUsint.Parse(data, ref offset);
        //        AdminState = CipUsint.Parse(data, ref offset);
        //        InterfaceLabel = CipShortString.Parse(data, ref offset);

        //        InterfaceCapability = new InterfaceCapability();
        //        InterfaceCapability.CapabilityBits = CipUdint.Parse(data, ref offset);
        //        InterfaceCapability.Count = CipUsint.Parse(data, ref offset);
        //        for (int i = 0; i < InterfaceCapability.Count.ToInt32(null); i++)
        //        {
        //            var speedDuplex = new InterfaceCapability.SpeedDuplex();
        //            speedDuplex.Speed = CipUint.Parse(data, ref offset);
        //            speedDuplex.Duplex = CipUsint.Parse(data, ref offset);
        //            InterfaceCapability.SpeedDuplexArray.Add(speedDuplex);
        //        }

        //        return 1;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        return -1;
        //    }
        //}
        
        public async Task<int> GetInterfaceCounter()
        {
            var request = GetAttributeSingleRequest((ushort)EthernetLinkAttributeId.InterfaceCounters);
            try
            {
                var response = await Messager.SendRRData(request);
                var data = response.ResponseData;
                var offset = 0;
                InterfaceCounter = new Counter();
                InterfaceCounter.InOctets = CipUdint.Parse(data, ref offset);
                InterfaceCounter.InUcastPackets = CipUdint.Parse(data, ref offset);
                InterfaceCounter.InNUcastPackets = CipUdint.Parse(data, ref offset);
                InterfaceCounter.InDiscards = CipUdint.Parse(data, ref offset);
                InterfaceCounter.InErrors = CipUdint.Parse(data, ref offset);
                InterfaceCounter.InUnknownProtos = CipUdint.Parse(data, ref offset);
                InterfaceCounter.OutOctets = CipUdint.Parse(data, ref offset);
                InterfaceCounter.OutUcastPackets = CipUdint.Parse(data, ref offset);
                InterfaceCounter.OutNUcastPackets = CipUdint.Parse(data, ref offset);
                InterfaceCounter.OutDiscards = CipUdint.Parse(data, ref offset);
                InterfaceCounter.OutErrors = CipUdint.Parse(data, ref offset);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        public async Task<int> GetMediaCounter()
        {
            var request = GetAttributeSingleRequest((ushort)EthernetLinkAttributeId.MediaCounters);
            try
            {
                var response = await Messager.SendRRData(request);
                var data = response.ResponseData;
                var offset = 0;
                MediaCounter = new MediaSpecialCounter();
                MediaCounter.AlignmentErrors = CipUdint.Parse(data, ref offset);
                MediaCounter.FCSErrors = CipUdint.Parse(data, ref offset);
                MediaCounter.SingleCollisions = CipUdint.Parse(data, ref offset);
                MediaCounter.MultipleCollisions = CipUdint.Parse(data, ref offset);
                MediaCounter.SQETestErrors = CipUdint.Parse(data, ref offset);
                MediaCounter.DeferredTransmissions = CipUdint.Parse(data, ref offset);
                MediaCounter.LateCollisions = CipUdint.Parse(data, ref offset);
                MediaCounter.ExcessiveCollision = CipUdint.Parse(data, ref offset);
                MediaCounter.MACTransmitErrors = CipUdint.Parse(data, ref offset);
                MediaCounter.CarrierSenseErrors = CipUdint.Parse(data, ref offset);
                MediaCounter.FrameTooLong = CipUdint.Parse(data, ref offset);
                MediaCounter.MACReceiveErrors = CipUdint.Parse(data, ref offset);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        public async Task<int> GetFlag()
        {
            var request = GetAttributeSingleRequest((ushort)EthernetLinkAttributeId.Flags);
            try
            {
                var response = await Messager.SendRRData(request);
                var data = response.ResponseData;
                var offset = 0;
                _flags = CipUdint.Parse(data, ref offset);
                ParseFlag();
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        public void ParseFlag()
        {
            var flags = _flags.ToUInt32(null);
            Status = (LinkStatus) (byte) (flags & 1);
            DuplexStatus = (Duplex) (byte) (flags >> 1 & 1);
            Negotiation = (NegotiationStatus) (byte) (flags >> 2 & 7);
            HardwareFault = (LocalHardwareFault) (byte) (flags >> 6 & 1);
        }

        public async Task<int> GetSpeed()
        {
            var request = GetAttributeSingleRequest((ushort)EthernetLinkAttributeId.Speed);
            try
            {
                var response = await Messager.SendRRData(request);
                var data = response.ResponseData;
                var offset = 0;
                _speed = CipUdint.Parse(data, ref offset);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        public async Task<int> GetLabel()
        {
            var request = GetAttributeSingleRequest((ushort)EthernetLinkAttributeId.InterfaceLabel);
            try
            {
                var response = await Messager.SendRRData(request);
                var data = response.ResponseData;
                var offset = 0;
                InterfaceLabel = CipShortString.Parse(data, ref offset);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        public async Task<int> GetState()
        {
            var request = GetAttributeSingleRequest((ushort)EthernetLinkAttributeId.InterfaceState);
            try
            {
                var response = await Messager.SendRRData(request);
                var data = response.ResponseData;
                var offset = 0;
                _interfaceState = CipUsint.Parse(data, ref offset);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        public async Task<int> GetCapability()
        {
            var request = GetAttributeSingleRequest((ushort)EthernetLinkAttributeId.InterfaceCapability);
            try
            {
                var response = await Messager.SendRRData(request);
                var data = response.ResponseData;
                var offset = 0;
                InterfaceCapability = new InterfaceCapability();
                InterfaceCapability.CapabilityBits = CipUdint.Parse(data, ref offset);
                InterfaceCapability.Count = CipUsint.Parse(data, ref offset);
                for (int i = 0; i < InterfaceCapability.Count.ToInt32(null); i++)
                {
                    var speedDuplex = new InterfaceCapability.SpeedDuplex();
                    speedDuplex.Speed = CipUint.Parse(data, ref offset);
                    speedDuplex.Duplex = CipUsint.Parse(data, ref offset);
                    InterfaceCapability.SpeedDuplexArray.Add(speedDuplex);
                }
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }
    }

    public class InterfaceCapability
    {
        public CipUdint CapabilityBits { set; get; }

        public CipUsint Count { set; get; }

        public List<SpeedDuplex> SpeedDuplexArray { set; get; } = new List<SpeedDuplex>();

        public class SpeedDuplex
        {
            public CipUint Speed { set; get; }
            public CipUsint Duplex { set; get; }
        }
    }

    public class Counter
    {
        public CipUdint InOctets { set; get; }
        public CipUdint InUcastPackets { set; get; }
        public CipUdint InNUcastPackets { set; get; }
        public CipUdint InDiscards { set; get; }
        public CipUdint InErrors { set; get; }
        public CipUdint InUnknownProtos { set; get; }
        public CipUdint OutOctets { set; get; }
        public CipUdint OutUcastPackets { set; get; }
        public CipUdint OutNUcastPackets { set; get; }
        public CipUdint OutDiscards { set; get; }
        public CipUdint OutErrors { set; get; }
    }

    public class MediaSpecialCounter
    {
        public CipUdint AlignmentErrors { set; get; }
        public CipUdint FCSErrors { set; get; }
        public CipUdint SingleCollisions { set; get; }
        public CipUdint MultipleCollisions { set; get; }
        public CipUdint SQETestErrors { set; get; }
        public CipUdint DeferredTransmissions { set; get; }
        public CipUdint LateCollisions { set; get; }
        public CipUdint ExcessiveCollision { set; get; }
        public CipUdint MACTransmitErrors { set; get; }
        public CipUdint CarrierSenseErrors { set; get; }
        public CipUdint FrameTooLong { set; get; }
        public CipUdint MACReceiveErrors { get; set; }
    }

    public class ConfigurationForPhysical
    {
        public CipUint ControlBits { set; get; }
        public CipUint ForceInterfaceSpeed { set; get; }
    }
}
