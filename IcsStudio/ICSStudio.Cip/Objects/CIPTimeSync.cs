using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Cip.DataTypes;
using ICSStudio.Cip.EtherNetIP;

namespace ICSStudio.Cip.Objects
{
    public enum TimeSyncAttributeType
    {
        PTPEnable = 1,
        IsSynchronized = 2,
        SystemTimeMicroseconds = 3,
        OffsetFromMaster = 5,
        GrandMasterClockInfo = 8,
        LocalClockInfo = 10,

        //NumberOfPorts=11,
        PortStateInfo = 12,

        ClockDescription1 = 102,
        ClockDescription2 = 103,
        ClockDescription3 = 104,
        ClockDescription4 = 105,
        //ProductDescription=21,
        //UserDescription =23,
    }

    public class ConvertValue
    {
        public static string ConvertNetworkProtocolValue(ushort v)
        {
            switch (v)
            {
                case 1:
                    return "UDP/IPv4";
                case 2:
                    return "UDP/IPv6";
                case 3:
                    return "IEEE 802.3";
                case 4:
                    return "DeviceNet";
                case 5:
                    return "ControlNet";
                case 6:
                    return "PROFINET";
                case 7:
                    return "Reserved";
                case 8:
                    return "Local";
            }

            throw new OutOfMemoryException();
        }

        public static string ConvertIsSynchronized(ushort v)
        {
            if (v == 1) return "Synchronized";
            if (v == 0) return "NotSynchronized";
            if (v == 2) return "Master";
            throw new OutOfMemoryException();
        }

        public static string ConvertEnablePTP(ushort v)
        {
            if (v == 1) return "Enabled";
            if (v == 0) return "Disabled";
            throw new OutOfMemoryException();
        }

        public static string ConvertTimeSourceValue(ushort v)
        {
            if (v == 16)
                return "Atomic";
            if (v == 32)
                return "GPS";
            if (v == 48)
                return "Radio";
            if (v == 64)
                return "PTP";
            if (v == 80)
                return "NTP";
            if (v == 96)
                return "HandSet";
            if (v == 144)
                return "Other";
            if (v == 160)
                return "Oscillator";
            if (v >= 240 && v <= 254) return "Unknown";
            return "Reserved";
        }
    }

    public class CIPTimeSync : CipBaseObject
    {
        public CIPTimeSync(int instanceId, ICipMessager messager) : base((ushort) CipObjectClassCode.TimeSync,
            instanceId, messager)
        {
        }

        public GrandMasterClockInfo GrandMasterClockInfo { get; } = new GrandMasterClockInfo();
        public LocalClockInfo LocalClockInfo { get; } = new LocalClockInfo();
        public CipBool IsSynchronized { private set; get; }
        public CipLint OffsetFromMaster { private set; get; }
        public CipUlint SystemTimeMicroseconds { private set; get; }
        public PortStateInfo PortStateInfo { private set; get; } = new PortStateInfo();
        public CipBool PTPEnable { private set; get; }
        public ClockDescription ClockDescription { private set; get; } = new ClockDescription();

        public async Task<int> GetAllTimeSyncInfo()
        {
            try
            {
                var r = await GetPTPEnable();
                if (r == -1) return -1;
                r = await GetSystemTimeMicroseconds();
                if (r == -1) return -1;
                r = await GetOffsetFromMaster();
                if (r == -1) return -1;
                r = await GetIsSynchronized();
                if (r == -1) return -1;
                r = await GetGrandmasterTimeSync();
                if (r == -1) return -1;
                r = await GetLocalClockInfo();
                if (r == -1) return -1;
                r = await GetClockDescription();
                if (r == -1) return -1;
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        public async Task<int> GetSystemTimeMicroseconds()
        {
            var request = GetAttributeSingleRequest((ushort) TimeSyncAttributeType.SystemTimeMicroseconds);
            var response = await Messager.SendRRData(request);
            if (response != null)
            {
                var data = response.ResponseData;
                Debug.Assert(data != null);
                SystemTimeMicroseconds = BitConverter.ToUInt64(data, 0);
                return 1;
            }

            return -1;
        }

        public async Task<int> GetOffsetFromMaster()
        {
            var request = GetAttributeSingleRequest((ushort) TimeSyncAttributeType.OffsetFromMaster);
            var response = await Messager.SendRRData(request);
            if (response != null)
            {
                var data = response.ResponseData;
                Debug.Assert(data != null);
                OffsetFromMaster = BitConverter.ToInt64(data, 0);
                return 1;
            }

            return -1;
        }

        public async Task<int> GetIsSynchronized()
        {
            var request = GetAttributeSingleRequest((ushort) TimeSyncAttributeType.IsSynchronized);
            var response = await Messager.SendRRData(request);
            if (response != null)
            {
                var data = response.ResponseData;
                Debug.Assert(data != null);
                IsSynchronized = data[0];
                return 1;
            }

            return -1;
        }

        public async Task<int> GetGrandmasterTimeSync()
        {
            var request = GetAttributeSingleRequest((ushort) TimeSyncAttributeType.GrandMasterClockInfo);
            var response = await Messager.SendRRData(request);
            if (response != null)
            {
                var data = response.ResponseData;
                Debug.Assert(data != null);
                int offset = 0;
                for (int i = 0; i < 8; i++)
                {
                    GrandMasterClockInfo.ClockIdentity.SetValue(i, data[i]);
                }

                offset += 8;

                GrandMasterClockInfo.ClockClass = BitConverter.ToUInt16(data, offset);
                offset += 2;

                GrandMasterClockInfo.TimeAccuracy = BitConverter.ToUInt16(data, offset);
                offset += 2;

                GrandMasterClockInfo.OffsetScaledLogVariance = BitConverter.ToUInt16(data, offset);
                offset += 2;

                GrandMasterClockInfo.CurrentUtcOffset = BitConverter.ToUInt16(data, offset);
                offset += 2;

                GrandMasterClockInfo.TimePropertyFlags = BitConverter.ToUInt16(data, offset);
                offset += 2;

                GrandMasterClockInfo.TimeSource = BitConverter.ToUInt16(data, offset);
                offset += 2;

                GrandMasterClockInfo.Priority1 = BitConverter.ToUInt16(data, offset);
                offset += 2;

                GrandMasterClockInfo.Priority2 = BitConverter.ToUInt16(data, offset);

                return 1;
            }

            return 0;
        }

        public async Task<int> GetLocalClockInfo()
        {
            var request = GetAttributeSingleRequest((ushort) TimeSyncAttributeType.LocalClockInfo);
            var response = await Messager.SendRRData(request);
            if (response != null)
            {

                var data = response.ResponseData;
                Debug.Assert(data != null);
                int offset = 0;
                for (int i = 0; i < 8; i++)
                {
                    LocalClockInfo.ClockIdentity.SetValue(i, data[i]);
                }

                offset += 8;

                LocalClockInfo.ClockClass = BitConverter.ToUInt16(data, offset);
                offset += 2;

                LocalClockInfo.TimeAccuracy = BitConverter.ToUInt16(data, offset);
                offset += 2;

                LocalClockInfo.OffsetScaledLogVariance = BitConverter.ToUInt16(data, offset);
                offset += 2;

                LocalClockInfo.CurrentUtcOffset = BitConverter.ToUInt16(data, offset);
                offset += 2;

                LocalClockInfo.TimePropertyFlags = BitConverter.ToUInt16(data, offset);
                offset += 2;

                LocalClockInfo.TimeSource = BitConverter.ToUInt16(data, offset);

                return 1;
            }

            return -1;
        }

        //public async Task<int> GetUserDescription()
        //{
        //    var request = GetAttributeSingleRequest((ushort)TimeSyncAttributeType.UserDescription);
        //    var response = await Messager.SendRRData(request);
        //    if (response != null)
        //    {

        //        return 1;
        //    }

        //    return -1;
        //}

        public async Task<int> GetPortStateInfo()
        {
            var request = GetAttributeSingleRequest((ushort) TimeSyncAttributeType.PortStateInfo);
            var response = await Messager.SendRRData(request);
            if (response != null)
            {
                var data = response.ResponseData;
                Debug.Assert(data != null);
                int offset = 0;
                PortStateInfo.NumberOfPorts = BitConverter.ToUInt16(data, offset);
                offset += 2;
                PortStateInfo.PortInfos.Clear();
                for (int i = 0; i < PortStateInfo.NumberOfPorts.ToUInt16(null); i++)
                {
                    var number = BitConverter.ToUInt16(data, offset);
                    offset += 2;

                    var state = BitConverter.ToUInt16(data, offset);
                    offset += 2;
                    var info = new Tuple<CipUint, CipUint>(number, state);
                    PortStateInfo.PortInfos.Add(info);
                }

                return 1;
            }

            return -1;
        }

        public async Task<int> GetPTPEnable()
        {
            var request = GetAttributeSingleRequest((ushort) TimeSyncAttributeType.PTPEnable);
            var response = await Messager.SendRRData(request);
            if (response != null)
            {
                var data = response.ResponseData;
                Debug.Assert(data != null);
                PTPEnable = data[0];
                return 1;
            }

            return -1;
        }

        public async Task<int> GetClockDescription()
        {
            var r = await GetClockDescription1();
            if (r == -1) return -1;
            r = await GetClockDescription2();
            if (r == -1) return -1;
            r = await GetClockDescription3();
            if (r == -1) return -1;
            r = await GetClockDescription4();
            if (r == -1) return -1;
            return 1;
        }

        private async Task<int> GetClockDescription1()
        {
            var request = GetAttributeSingleRequest((ushort) TimeSyncAttributeType.ClockDescription1);
            var response = await Messager.SendRRData(request);
            if (response != null)
            {
                var data = response.ResponseData;
                Debug.Assert(data != null);
                int offset = 0;
                for (; offset < 8; offset++)
                {
                    ClockDescription.IdentityDataClockID.SetValue(offset, data[offset]);
                }

                ClockDescription.IdentityDataClockPort = BitConverter.ToUInt16(data, offset);
                return 1;
            }

            return -1;
        }

        private async Task<int> GetClockDescription2()
        {
            var request = GetAttributeSingleRequest((ushort) TimeSyncAttributeType.ClockDescription2);
            var response = await Messager.SendRRData(request);
            if (response != null)
            {
                var data = response.ResponseData;
                Debug.Assert(data != null);
                int offset = 0;
                for (; offset < 8; offset++)
                {
                    ClockDescription.ClockDataClockID.SetValue(offset, data[offset]);
                }

                for (int i = 0; i < 2; i++)
                {
                    ClockDescription.ClockDataClockType.SetValue(i, data[offset++]);
                }

                for (int i = 0; i < 4; i++)
                {
                    ClockDescription.ClockDataManufactureID.SetValue(i, data[offset++]);
                }

                ClockDescription.ClockDataProdDescManuf = CipShortString.Parse(data, ref offset);

                ClockDescription.ClockDataProdDescModel = CipShortString.Parse(data, ref offset);

                ClockDescription.ClockDataProdDescSerialNum = CipShortString.Parse(data, ref offset);

                ClockDescription.ClockDataHWRevision = CipShortString.Parse(data, ref offset);

                ClockDescription.ClockDataFWRevision = CipShortString.Parse(data, ref offset);

                ClockDescription.ClockDataSWRevision = CipShortString.Parse(data, ref offset);

                for (int i = 0; i < 6; i++)
                {
                    ClockDescription.ClockDataProfileID.SetValue(i, data[offset++]);
                }

                return 1;
            }

            return -1;
        }

        private async Task<int> GetClockDescription3()
        {
            var request = GetAttributeSingleRequest((ushort) TimeSyncAttributeType.ClockDescription3);
            var response = await Messager.SendRRData(request);
            if (response != null)
            {
                var data = response.ResponseData;
                Debug.Assert(data != null);
                int offset = 0;
                for (; offset < 8; offset++)
                {
                    ClockDescription.PortDataClockID.SetValue(offset, data[offset]);
                }

                ClockDescription.PortDataClockPort = BitConverter.ToUInt16(data, offset);
                offset += 2;

                ClockDescription.PortDataPhysicalLayerProto = CipShortString.Parse(data, ref offset);

                var count = BitConverter.ToUInt16(data, offset);
                offset += 2;
                ClockDescription.PortDataPhysicalAddress = new CipByteArray(count);
                for (int i = 0; i < count; i++)
                {
                    ClockDescription.PortDataPhysicalAddress.SetValue(i, data[offset++]);
                }

                ClockDescription.PortDataNetworkProtocol = BitConverter.ToUInt16(data, offset);
                offset += 2;

                count = BitConverter.ToUInt16(data, offset);
                offset += 2;
                ClockDescription.PortDataProtocolAddress = new CipByteArray(count);
                for (int i = 0; i < count; i++)
                {
                    ClockDescription.PortDataProtocolAddress.SetValue(i, data[offset++]);
                }

                return 1;
            }

            return -1;
        }

        private async Task<int> GetClockDescription4()
        {
            var request = GetAttributeSingleRequest((ushort) TimeSyncAttributeType.ClockDescription4);
            var response = await Messager.SendRRData(request);
            if (response != null)
            {
                var data = response.ResponseData;
                Debug.Assert(data != null);
                int offset = 0;
                for (; offset < 8; offset++)
                {
                    ClockDescription.UserDataClockID.SetValue(offset, data[offset]);
                }
                
                ClockDescription.UserDataDescriptionName = CipShortString.Parse(data, ref offset);

                ClockDescription.UserDataDescriptionLoc = CipShortString.Parse(data, ref offset);
                return 1;
            }

            return -1;
        }
    }

    public class ClockDescription
    {
        //<!-- #102 Clock Description Identity Data Structure attributes	-->
        public CipByteArray IdentityDataClockID { get; } = new CipByteArray(8);

        public CipUint IdentityDataClockPort { set; get; }

        //<!-- #103 Clock Description Clock Data Structure attributes	-->
        public CipByteArray ClockDataClockID { get; } = new CipByteArray(8);
        public CipByteArray ClockDataClockType { get; } = new CipByteArray(2);
        public CipByteArray ClockDataManufactureID { get; } = new CipByteArray(4);
        public CipShortString ClockDataProdDescManuf { set; get; }
        public CipShortString ClockDataProdDescModel { set; get; }
        public CipShortString ClockDataProdDescSerialNum { set; get; }
        public CipShortString ClockDataHWRevision { set; get; }
        public CipShortString ClockDataFWRevision { set; get; }
        public CipShortString ClockDataSWRevision { set; get; }

        public CipByteArray ClockDataProfileID { get; } = new CipByteArray(6);

        //<!-- #104 Clock Description Port Data Structure -->
        public CipByteArray PortDataClockID { get; } = new CipByteArray(8);
        public CipUint PortDataClockPort { set; get; }
        public CipShortString PortDataPhysicalLayerProto { set; get; }
        public CipByteArray PortDataPhysicalAddress { set; get; }
        public CipUint PortDataNetworkProtocol { set; get; }

        public CipByteArray PortDataProtocolAddress { set; get; }

        //<!-- #105 Clock Description User Data Structure -->
        public CipByteArray UserDataClockID { get; } = new CipByteArray(8);
        public CipShortString UserDataDescriptionName { set; get; }
        public CipShortString UserDataDescriptionLoc { set; get; }
    }

    public class PortStateInfo
    {
        public CipUint NumberOfPorts { set; get; }

        public List<Tuple<CipUint, CipUint>> PortInfos { get; } = new List<Tuple<CipUint, CipUint>>();
    }

    public class LocalClockInfo
    {
        public CipByteArray ClockIdentity { get; } = new CipByteArray(8);

        public CipUint ClockClass { set; get; }

        public CipUint TimeAccuracy { set; get; }

        public CipUint OffsetScaledLogVariance { set; get; }

        public CipUint CurrentUtcOffset { set; get; }

        public CipUint TimePropertyFlags { set; get; }

        public CipUint TimeSource { set; get; }
    }

    public class GrandMasterClockInfo : LocalClockInfo
    {
        public CipUint Priority1 { set; get; }

        public CipUint Priority2 { set; get; }
    }
}
