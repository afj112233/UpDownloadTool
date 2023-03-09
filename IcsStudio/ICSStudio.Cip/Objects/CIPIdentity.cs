using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using ICSStudio.Cip.DataTypes;
using ICSStudio.Cip.EtherNetIP;

namespace ICSStudio.Cip.Objects
{
    /// <summary>
    /// vol1, Table 5A-2.2 Identity Object Instance Attributes
    /// </summary>
    public enum IdentityAttributeId : ushort
    {
        VendorID = 1,
        DeviceType = 2,
        ProductCode = 3,
        Revision = 4,
        Status = 5,
        SerialNumber = 6,
        ProductName = 7,
        State = 8,
        ConfigurationConsistencyValue = 9,
        HeartbeatInterval = 10,
        ActiveLanguage = 11,
        SupportedLanguageList = 12,
        InternationalProductName = 13,
        Semaphore = 14,
        AssignedName = 15,
        AssignedDescription = 16,
        GeographicLocation = 17,

        ProtectionMode = 19,
    }

    [Flags]
    public enum IdentityStatusBitmap : ushort
    {
        Owned = 1 << 0,
        Reserved0 = 1 << 1,
        Configured = 1 << 2,
        Reserved1 = 1 << 3,
        ExtendedDeviceStatus = 1 << 4, // 4-7
        MinorRecoverableFault = 1 << 8,
        MinorUnrecoverableFault = 1 << 9,
        MajorRecoverableFault = 1 << 10,
        MajorUnrecoverableFault = 1 << 11,
        ExtendedDeviceStatus2 = 1 << 12, // 12-15
    }


    public class CIPIdentityConvert
    {
        public static string InternalStateConvert(byte id)
        {
            switch (id)
            {
                case 0:
                    return "Self-test or State Unknown";
                case 1:
                    return "Flash update";
                case 2:
                    return "Communication fault";
                case 3:
                    return "Unconnected";
                case 4:
                    return "Flash configuration bad";
                case 5:
                    return "Major fault";
                case 6:
                    return "Run mode";
                case 7:
                    return "Program mode";
                default:
                    return "(16#%04X) unknown";
            }
        }

        public static string OwnedConvert(ushort id)
        {
            if (id == 0) return "No";
            if (id == 1) return "Owned";
            throw new ArgumentOutOfRangeException();
        }

        public static string ConfiguredConvert(ushort id)
        {
            if (id == 0) return "No";
            if (id == 1) return "Configured";
            throw new ArgumentOutOfRangeException();
        }

        public static string RecoverableFaultConvert(bool id)
        {
            if (!id) return "None";
            return "Recoverable";
        }

        public static string UnRecoverableFaultConvert(bool id)
        {
            if (!id) return "None";
            return "UnRecoverable";
        }

        public static string TimerHardwareConvert(ushort id)
        {
            if (id == 0) return "OK";
            if (id == 1) return "Fault";
            throw new ArgumentOutOfRangeException();
        }
    }

    public class CIPIdentity : CipBaseObject
    {
        public CIPIdentity(ushort instanceId, ICipMessager messager) : base((ushort) CipObjectClassCode.Identity,
            instanceId, messager)
        {
        }

        #region Cip Attribute

        [CipDetailInfo((ushort) IdentityAttributeId.VendorID, "VendorID")]
        public CipUint VendorID { get; set; }

        [CipDetailInfo((ushort) IdentityAttributeId.DeviceType, "DeviceType")]
        public CipUint DeviceType { get; set; }

        [CipDetailInfo((ushort) IdentityAttributeId.ProductCode, "ProductCode")]
        public CipUint ProductCode { get; set; }

        [CipDetailInfo((ushort) IdentityAttributeId.Revision, "Revision")]
        public CipRevision Revision { get; set; }

        [CipDetailInfo((ushort) IdentityAttributeId.Status, "Status")]
        public CipUint Status { get; set; }

        [CipDetailInfo((ushort) IdentityAttributeId.SerialNumber, "SerialNumber")]
        public CipUdint SerialNumber { get; set; }

        [CipDetailInfo((ushort) IdentityAttributeId.ProductName, "ProductName")]
        public CipString ProductName { get; set; }

        [CipDetailInfo((ushort) IdentityAttributeId.State, "State")]
        public CipUsint State { get; set; }

        [CipDetailInfo((ushort) IdentityAttributeId.ConfigurationConsistencyValue, "ConfigurationConsistencyValue")]
        public CipUint ConfigurationConsistencyValue { get; set; }

        [CipDetailInfo((ushort) IdentityAttributeId.HeartbeatInterval, "HeartbeatInterval")]
        public CipSint HeartbeatInterval { get; set; }

        [CipDetailInfo((ushort) IdentityAttributeId.ActiveLanguage, "ActiveLanguage")]
        public LanguageDataTypes ActiveLanguage { get; set; }

        [CipDetailInfo((ushort) IdentityAttributeId.SupportedLanguageList, "SupportedLanguageList")]
        public List<LanguageDataTypes> SupportedLanguageList { get; set; }

        [CipDetailInfo((ushort) IdentityAttributeId.InternationalProductName, "InternationalProductName")]
        public CipStringi InternationalProductName { get; set; }

        [CipDetailInfo((ushort) IdentityAttributeId.Semaphore, "Semaphore")]
        public SemaphoreInfo Semaphore { get; set; }

        [CipDetailInfo((ushort) IdentityAttributeId.AssignedName, "AssignedName")]
        public CipStringi AssignedName { get; set; }

        [CipDetailInfo((ushort) IdentityAttributeId.AssignedDescription, "AssignedDescription")]
        public CipStringi AssignedDescription { get; set; }

        [CipDetailInfo((ushort) IdentityAttributeId.GeographicLocation, "GeographicLocation")]
        public CipStringi GeographicLocation { get; set; }

        //[CipDetailInfo((ushort)IdentityAttributeId.ModbusIdentityInfo, "ModbusIdentityInfo")]
        //public ModbusIdentityInfo ModbusIdentityInfo { get; set; }

        [CipDetailInfo((ushort) IdentityAttributeId.ProtectionMode, "ProtectionMode")]
        public CipUint ProtectionMode { get; set; }

        #endregion

        #region Cip Service

        public async Task<int> GetAttributeList(List<ushort> attributeIdList)
        {
            var request = GetAttributeListRequest(attributeIdList);

            try
            {
                var response = await Messager.SendUnitData(request);
                if ((response != null) && (response.GeneralStatus == (byte) CipGeneralStatusCode.Success))
                {
                    byte[] responseData = response.ResponseData;

                    int offset = 0;
                    ushort attributeCount = BitConverter.ToUInt16(responseData, offset);
                    offset += 2;

                    if (attributeCount != attributeIdList.Count)
                    {
                        Console.WriteLine($"Count difference!");
                        return -1;
                    }

                    for (int i = 0; i < attributeCount; i++)
                    {
                        // Attribute identifier
                        var attributeId = BitConverter.ToUInt16(responseData, offset);
                        offset += 2;

                        // Attribute Status
                        var attributeStatus = BitConverter.ToUInt16(responseData, offset);
                        offset += 2;

                        // Attribute Data
                        if (attributeStatus == 0)
                        {
                            Parse(attributeId, responseData, ref offset);
                        }
                        else
                        {
                            Console.WriteLine($"Attribute {attributeId} Status: {attributeStatus}");
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }

            return -1;
        }

        public async Task<int> GetAttributesAll()
        {
            var request = GetAttributesAllRequest();
            try
            {
                var response = await Messager.SendRRData(request);
                return ParseGetAttributesAllResponse(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        public async Task<int> Reset()
        {
            var request = GetResetRequest();

            try
            {
                var response = await Messager.SendRRData(request);

                if (response != null)
                    return response.GeneralStatus;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }

            return -1;
        }

        public IMessageRouterRequest GetResetRequest()
        {
            var request = new MessageRouterRequest()
            {
                Service = (byte)CipServiceCode.Reset,
                RequestPath = new PaddedEPath(ClassId, InstanceId),
                RequestData = null
            };

            return request;
        }

        #endregion

        public int ParseGetAttributesAllResponse(IMessageRouterResponse response)
        {
            if (response != null && response.GeneralStatus == (byte) CipGeneralStatusCode.Success)
            {
                var data = response.ResponseData;
                Contract.Assert(data != null);

                int offset = 0;
                //vender
                Parse((ushort) IdentityAttributeId.VendorID, data, ref offset);

                //device
                Parse((ushort) IdentityAttributeId.DeviceType, data, ref offset);

                //product code
                Parse((ushort) IdentityAttributeId.ProductCode, data, ref offset);

                //revision
                Parse((ushort) IdentityAttributeId.Revision, data, ref offset);

                //Status
                Parse((ushort) IdentityAttributeId.Status, data, ref offset);

                //serial
                Parse((ushort) IdentityAttributeId.SerialNumber, data, ref offset);

                //product name
                Parse((ushort) IdentityAttributeId.ProductName, data, ref offset);

                State = 255;
                ConfigurationConsistencyValue = 0;
                HeartbeatInterval = 0;

                if (data.Length > offset)
                {
                    Parse((ushort) IdentityAttributeId.State, data, ref offset);
                }

                if (data.Length > offset)
                {
                    Parse((ushort) IdentityAttributeId.ConfigurationConsistencyValue, data, ref offset);
                }

                if (data.Length > offset)
                {
                    Parse((ushort) IdentityAttributeId.HeartbeatInterval, data, ref offset);
                }

                return 0;
            }

            return -1;
        }

        /*
        public JObject ConvertIdentity()
        {
            var attrs = new JObject();
            //vender
            DBHelper dbHelper = new DBHelper();
            attrs["VendorId"] = dbHelper.GetVendorName(VendorID.ToUInt16(null));

            //device
            attrs["DeviceType"] = ((CipDeviceType) DeviceType.ToUInt16(null)).ToString();

            //product code
            attrs["ProductCode"] = ProductCode.ToUInt16(null);

            //revision
            attrs["Revision"] = Revision.ToString();

            //Status
            var status = new JObject();
            attrs["Status"] = status;
            var bitList = new bool[16];
            for (int i = 0; i < 16; i++)
            {
                var val = 1 << i;
                bitList[i] = (Status.ToUInt16(null) & val) == val;
            }

            status["Owned"] = CIPIdentityConvert.OwnedConvert((ushort) GetIntFromBit(bitList, 0, 2));
            status["Configured"] = CIPIdentityConvert.ConfiguredConvert((ushort) GetIntFromBit(bitList, 2, 2));
            status["ExtendDeviceStatus"] = GetIntFromBit(bitList, 4, 4);
            status["MinorRecoverableFault"] = CIPIdentityConvert.RecoverableFaultConvert(bitList[8]);
            status["MinorUnrecoverableFault"] = CIPIdentityConvert.UnRecoverableFaultConvert(bitList[9]);
            status["MajorRecoverableFault"] = CIPIdentityConvert.RecoverableFaultConvert(bitList[10]);
            status["MajorUnrecoverableFault"] = CIPIdentityConvert.UnRecoverableFaultConvert(bitList[11]);
            status["ExtendDeviceStatus2"] = GetIntFromBit(bitList, 12, 4);

            //serial
            var serial = "";
            var data = SerialNumber.GetBytes();
            foreach (var t in data.Reverse())
            {
                serial += t.ToString("X");
            }

            attrs["Serial"] = serial;

            //product name
            attrs["ProductName"] = ProductName.ToString();

            attrs["State"] = CIPIdentityConvert.InternalStateConvert(unchecked((byte) State.ToSByte(null)));
            attrs["ConfigurationConsistencyValue"] = ConfigurationConsistencyValue.ToUInt16(null);
            attrs["HeartbeatInterval"] = unchecked((byte) HeartbeatInterval.ToSByte(null));
            return attrs;
        }
        
        private int GetIntFromBit(bool[] data, int index, int len)
        {
            try
            {
                var val = "";
                for (int i = index; i < index + len; i++)
                {
                    val = (data[i] ? "1" : "0") + val;
                }

                return int.Parse(val);
            }
            catch (Exception)
            {
                throw new IndexOutOfRangeException();
            }
        }
        */

        private void Parse(ushort attributeId, byte[] buffer, ref int offset)
        {
            IdentityAttributeId identityAttributeId = (IdentityAttributeId) attributeId;
            switch (identityAttributeId)
            {
                case IdentityAttributeId.VendorID:
                    VendorID = BitConverter.ToUInt16(buffer, offset);
                    offset += 2;
                    break;
                case IdentityAttributeId.DeviceType:
                    DeviceType = BitConverter.ToUInt16(buffer, offset);
                    offset += 2;
                    break;
                case IdentityAttributeId.ProductCode:
                    ProductCode = BitConverter.ToUInt16(buffer, offset);
                    offset += 2;
                    break;
                case IdentityAttributeId.Revision:
                    Revision = CipRevision.Parse(buffer, ref offset);
                    break;
                case IdentityAttributeId.Status:
                    Status = BitConverter.ToUInt16(buffer, offset);
                    offset += 2;
                    break;
                case IdentityAttributeId.SerialNumber:
                    SerialNumber = BitConverter.ToUInt32(buffer, offset);
                    offset += 4;
                    break;
                case IdentityAttributeId.ProductName:
                    ProductName = StringConverter.ToShortString(buffer, ref offset);
                    break;
                case IdentityAttributeId.State:
                    State = buffer[offset];
                    offset += 1;
                    break;
                case IdentityAttributeId.ConfigurationConsistencyValue:
                    ConfigurationConsistencyValue = BitConverter.ToUInt16(buffer, offset);
                    offset += 2;
                    break;
                case IdentityAttributeId.HeartbeatInterval:
                    HeartbeatInterval = (sbyte) buffer[offset];
                    offset += 1;
                    break;
                case IdentityAttributeId.ActiveLanguage:
                    ActiveLanguage.Parse(buffer, ref offset);
                    break;
                case IdentityAttributeId.SupportedLanguageList:
                    throw new ArgumentException("I don't know how to parse!");

                case IdentityAttributeId.InternationalProductName:
                    InternationalProductName = CipStringi.Parse(buffer, ref offset);
                    break;
                case IdentityAttributeId.Semaphore:
                    Semaphore.Parse(buffer, ref offset);
                    break;
                case IdentityAttributeId.AssignedName:
                    AssignedName = CipStringi.Parse(buffer, ref offset);
                    break;
                case IdentityAttributeId.AssignedDescription:
                    AssignedDescription = CipStringi.Parse(buffer, ref offset);
                    break;
                case IdentityAttributeId.GeographicLocation:
                    GeographicLocation = CipStringi.Parse(buffer, ref offset);
                    break;
                case IdentityAttributeId.ProtectionMode:
                    ProtectionMode = BitConverter.ToUInt16(buffer, offset);
                    offset += 2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    #region Sub Class

    public class Revision
    {
        public byte Major { get; set; }
        public byte Minor { get; set; }

        public void Parse(byte[] buffer, ref int offset)
        {
            Major = buffer[offset];
            Minor = buffer[offset + 1];
            offset += 2;
        }
    }

    public class LanguageDataTypes
    {
        public byte Language1DataType { get; set; }
        public byte Language2DataType { get; set; }
        public byte Language3DataType { get; set; }

        public void Parse(byte[] buffer, ref int offset)
        {
            Language1DataType = buffer[offset];
            Language2DataType = buffer[offset + 1];
            Language3DataType = buffer[offset + 2];
            offset += 3;

        }
    }

    public class SemaphoreInfo
    {
        public ushort VendorNumber { get; set; }
        public uint ClientSerialNumber { get; set; }
        public short SemaphoreTimer { get; set; }

        public void Parse(byte[] buffer, ref int offset)
        {
            VendorNumber = BitConverter.ToUInt16(buffer, offset);
            offset += 2;

            ClientSerialNumber = BitConverter.ToUInt32(buffer, offset);
            offset += 4;

            SemaphoreTimer = BitConverter.ToInt16(buffer, offset);
            offset += 2;
        }
    }

    #endregion
}
