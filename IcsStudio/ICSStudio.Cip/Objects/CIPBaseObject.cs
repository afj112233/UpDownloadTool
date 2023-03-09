using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using ICSStudio.Cip.DataTypes;
using ICSStudio.Cip.EtherNetIP;
using NLog;

namespace ICSStudio.Cip.Objects
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum CipObjectClassCode : ushort
    {
        Identity = 0x01,
        MessageRouter = 0x02,
        DeviceNet = 0x03,
        Assembly = 0x04,
        Connection = 0x05,
        ConnectionManager = 0x06,

        File = 0x37,
        TimeSync = 0x43,
        DLR = 0x47,
        ControlNetScheduling = 0xF2,
        Port = 0xF4,
        TcpIp = 0xF5,
        EthernetLink = 0xF6,

        /* motion */
        MotionGroup = 0xB0,
        Axis = 0xB1,
        Trend = 0xB2,
        Controller = 0xB3,
        Task = 0xB4,
        Program = 0xB5,
        Routine = 0xB6,
        Module = 0xB7,
        UDType = 0xB8,

        /* vendor specific objects */
        Symbol = 0x6B, // Logix5000 Data Access
        Template = 0x6C // Logix5000 Data Access


        /*
         * 
        0x42,Motion Device Axis Object
        0x64,
        0x67,
        0x68,
        0x69,
        0x6a,
        
        0x6d,
        0x6f,
        0x70
        0x72,
        0x73,
        0x74,
        0x77,
        0x7e,
        0x7f,
        0x8b,
        0x8c,
        0x8d,
        0x8e,

        0xa1,
        0xa2,
        0xa3,
        0xac,

        

        0x0304,
        0x0316,
        0x0317,
        0x0318,
        0x0319,
        0x031a,
        0x032b,
        0x0335,
        */
    }

    public enum CipServiceCode : byte
    {
        /* Common Service */
        Reserved = 0x00,
        GetAttributesAll = 0x01,
        SetAttributesAll = 0x02,
        GetAttributeList = 0x03,
        SetAttributeList = 0x04,
        Reset = 0x05,
        Start = 0x06,
        Stop = 0x07,
        Create = 0x08,
        Delete = 0x09,
        MultipleServicePacket = 0x0A,
        ApplyAttributes = 0x0D,
        GetAttributeSingle = 0x0E,
        SetAttributeSingle = 0x10,
        FindNextObjectInstance = 0x11,
        ErrorResponse = 0x14, // used by DeviceNet only
        Restore = 0x15,
        Save = 0x16,
        NoOperation = 0x17, // NOP
        GetMember = 0x18,
        SetMember = 0x19,
        InsertMember = 0x1A,
        RemoveMember = 0x1B,
        GroupSync = 0x1C,

        /* Connection Manager Object Object-Specific Services,vol1 Table3-5.7 */
        ForwardClose = 0x4E,
        UnconnectedSend = 0x52,
        ForwardOpen = 0x54,
        GetConnectionData = 0x56,
        SearchConnectionData = 0x57,
        GetConnectionOwner = 0x5A,
        LargeForwardOpen = 0x5B,

        /* File Object Service */
        InitiateUpload = 0x4B,
        InitiateDownload = 0x4C,
        UploadTransfer = 0x4F,
        DownloadTransfer = 0x50,

        /* Customized */
        /* Motion Service */
        ExecuteCommand = 0x4C,
        QueryCommand = 0x4D
    }

    public class AttributePair
    {
        public ushort AttributeId { get; set; }
        public byte[] AttributeValue { get; set; }
    }

    public class CipBaseObject
    {
        protected static readonly ILogger Logger;

        static CipBaseObject()
        {
            Logger = LogManager.GetCurrentClassLogger();
        }

        protected CipBaseObject(ushort classId, int instanceId, ICipMessager messager)
        {
            InstanceId = instanceId;
            Messager = messager;
            ClassId = classId;
        }

        public ushort ClassId { get; }
        public int InstanceId { get; set; }
        public ICipMessager Messager { get; set; }

        public virtual IMessageRouterRequest GetAttributesAllRequest()
        {
            var request = new MessageRouterRequest
            {
                Service = (byte)CipServiceCode.GetAttributesAll,
                RequestPath = new PaddedEPath(ClassId, InstanceId),
                RequestData = null
            };

            return request;
        }

        public virtual IMessageRouterRequest GetAttributeListRequest(List<ushort> attributeIdList)
        {
            var request = new MessageRouterRequest
            {
                Service = (byte)CipServiceCode.GetAttributeList,
                RequestPath = new PaddedEPath(ClassId, InstanceId),
                RequestData = null
            };

            if ((attributeIdList != null) && (attributeIdList.Count > 0))
            {
                var attributeCount = (ushort)attributeIdList.Count;
                var byteList = new List<byte>((attributeCount + 1) * 2);

                byteList.AddRange(BitConverter.GetBytes(attributeCount));

                foreach (var attribute in attributeIdList)
                    byteList.AddRange(BitConverter.GetBytes(attribute));

                request.RequestData = byteList.ToArray();
            }


            return request;
        }

        public virtual IMessageRouterRequest SetAttributeListRequest(List<AttributePair> attributePairList)
        {
            var request = new MessageRouterRequest
            {
                Service = (byte)CipServiceCode.SetAttributeList,
                RequestPath = new PaddedEPath(ClassId, InstanceId),
                RequestData = null
            };

            if ((attributePairList != null) && (attributePairList.Count > 0))
            {
                var attributeCount = (ushort)attributePairList.Count;
                var byteList = new List<byte>(512);

                byteList.AddRange(BitConverter.GetBytes(attributeCount));

                foreach (var attributePair in attributePairList)
                {
                    byteList.AddRange(BitConverter.GetBytes(attributePair.AttributeId));
                    byteList.AddRange(attributePair.AttributeValue);
                }

                request.RequestData = byteList.ToArray();
            }

            return request;
        }

        public virtual IMessageRouterRequest GetAttributeSingleRequest(ushort attributeId)
        {
            var request = new MessageRouterRequest
            {
                Service = (byte)CipServiceCode.GetAttributeSingle,
                RequestPath = new PaddedEPath(ClassId, InstanceId, attributeId),
                RequestData = null
            };

            return request;
        }

        public virtual IMessageRouterRequest SetAttributeSingleRequest(ushort attributeId, byte[] data)
        {
            var request = new MessageRouterRequest
            {
                Service = (byte)CipServiceCode.SetAttributeSingle,
                RequestPath = new PaddedEPath(ClassId, InstanceId, attributeId),
                RequestData = data
            };

            return request;
        }

        #region Request and Reply

        private IMessageRouterRequest CreateServiceNoAttrRequest(byte code)
        {
            return new MessageRouterRequest
            {
                Service = code,
                RequestPath = new PaddedEPath(ClassId, InstanceId),
                RequestData = null
            };
        }

        private IMessageRouterRequest CreateServiceWithAttrRequest(byte code, ushort id)
        {
            return new MessageRouterRequest
            {
                Service = code,
                RequestPath = new PaddedEPath(ClassId, InstanceId, id),
                RequestData = null
            };
        }

        private IMessageRouterRequest CreateCallService(byte code, int inst, int attr, byte[] param)
        {
            return new MessageRouterRequest
            {
                Service = code,
                RequestPath = new PaddedEPath(ClassId, inst, (ushort)attr),
                RequestData = param
            };
        }

        private IMessageRouterRequest CreateCallService(byte code, int inst, byte[] param)
        {
            return new MessageRouterRequest
            {
                Service = code,
                RequestPath = new PaddedEPath(ClassId, inst),
                RequestData = param
            };
        }

        private IMessageRouterRequest CreateServiceWithData(byte code, ushort attr, byte data)
        {
            return new MessageRouterRequest
            {
                Service = code,
                RequestPath = new PaddedEPath(ClassId, InstanceId, attr),
                // ReSharper disable once RedundantExplicitArrayCreation
                RequestData = new byte[] { data }
            };
        }

        private IMessageRouterRequest CreateServiceWithData(byte code, byte[] data)
        {
            return new MessageRouterRequest
            {
                Service = code,
                RequestPath = new PaddedEPath(ClassId, InstanceId),
                RequestData = data
            };
        }

        #endregion

        public virtual IMessageRouterRequest CreateRequest()
        {
            var request = new MessageRouterRequest
            {
                Service = (byte)CipServiceCode.Create,
                RequestPath = new PaddedEPath(ClassId, InstanceId),
                RequestData = null
            };

            return request;
        }


        public static async Task GetAttributeList(string[] attributeNames, object cipObject, ICipMessager messager)
        {
            var baseObject = cipObject as CipBaseObject;
            var attributeMap = CipAttributeHelper.GetAttributeMap(cipObject.GetType());

            if ((attributeNames != null)
                && (attributeNames.Length > 0)
                && (messager != null)
                && (baseObject != null)
                && (attributeMap != null))
            {
                var attributeIdList = CipAttributeHelper.AttributeNamesToIdList(cipObject.GetType(), attributeNames);
                if ((attributeIdList == null) || (attributeIdList.Count == 0))
                {
                    Logger.Error("get attribute id list failed!");
                    return;
                }

                // request 
                var request = baseObject.GetAttributeListRequest(attributeIdList);

                // send
                var response = await messager.SendUnitData(request);
                if ((response != null) && (response.GeneralStatus == (byte)CipGeneralStatusCode.Success))
                {
                    byte[] responseData = response.ResponseData;
                    int startIndex = 0;
                    ushort attributeCount = BitConverter.ToUInt16(responseData, startIndex);
                    startIndex = 2;

                    for (int i = 0; i < attributeCount; i++)
                    {
                        // Attribute identifier
                        var attributeId = BitConverter.ToUInt16(responseData, startIndex);
                        startIndex += 2;

                        // Attribute Status
                        var attributeStatus = BitConverter.ToUInt16(responseData, startIndex);
                        startIndex += 2;

                        // Attribute Data
                        if (attributeStatus == 0)
                        {
                            if (attributeMap.ContainsKey(attributeId))
                            {
                                var attributeName = attributeMap[attributeId];
                                var p = cipObject.GetType().GetProperty(attributeName);

                                if (p != null)
                                {
                                    var cipDetailInfo =
                                        (CipDetailInfoAttribute)
                                        p.GetCustomAttributes(typeof(CipDetailInfoAttribute), true).FirstOrDefault();

                                    if (cipDetailInfo != null)
                                    {
                                        // parse
                                        var propertyValue = CipAttributeHelper.Parse(
                                            p.PropertyType,
                                            responseData,
                                            cipDetailInfo.ArraySize,
                                            ref startIndex);
                                        p.SetValue(cipObject, propertyValue);
                                    }

                                }

                            }
                        }
                        else
                        {
                            //Debug.WriteLine($"{attributeId} {attributeStatus}");
                        }
                    }

                }

            }
        }

        public static async Task SetAttributeList(List<ushort> attributeIdList, object cipObject, ICipMessager messager)
        {
            var baseObject = cipObject as CipBaseObject;
            var attributeMap = CipAttributeHelper.GetAttributeMap(cipObject.GetType());

            if ((attributeIdList != null)
                && (attributeIdList.Count > 0)
                && (messager != null)
                && (baseObject != null)
                && (attributeMap != null))
            {
                var attributePairList =
                    CipAttributeHelper.AttributeIdListToAttributePairList(cipObject, attributeIdList);
                if ((attributePairList == null) || (attributePairList.Count == 0))
                {
                    Logger.Error("get attribute pair list failed!");
                    throw new Exception("Get attribute pair list failed!");
                }

                // request 
                var request = baseObject.SetAttributeListRequest(attributePairList);

                // send
                var response = await messager.SendUnitData(request);

                if (response == null)
                    throw new Exception("SetAttributeList failed! Response is NULL!");

                if (response.GeneralStatus == (byte)CipGeneralStatusCode.Success)
                    return;

                if (response.GeneralStatus == (byte)CipGeneralStatusCode.AttributeListError)
                {
                    // get the first failed attribute
                    int offset = 0;
                    ushort count = BitConverter.ToUInt16(response.ResponseData, offset);
                    offset = 2;

                    for (int i = 0; i < count; i++)
                    {
                        ushort attributeId = BitConverter.ToUInt16(response.ResponseData, offset);
                        offset += 2;
                        ushort attributeStatus = BitConverter.ToUInt16(response.ResponseData, offset);
                        offset += 2;

                        if (attributeStatus != (byte)CipGeneralStatusCode.Success)
                            throw new Exception($"{attributeId},{attributeStatus}");
                    }

                }
                else
                {
                    throw new Exception(
                        $"SetAttributeList failed! Response Status is 0x{response.GeneralStatus:x2}!");
                }

            }
        }

        public async Task GetAttributeSingle(string name)
        {
            //使用ID获取对应的属性
            await CipBaseObject.GetAttributeSingleWithSendRRData(name, this, Messager);

        }

        public async Task<IMessageRouterResponse> ServiceNoAttr(byte code)
        {
            var request = CreateServiceNoAttrRequest(code);
            var resp = await Messager.SendRRData(request);
            return resp;
        }

        public async Task<IMessageRouterResponse> ServiceWithAttr(byte code, ushort id)
        {
            var request = CreateServiceWithAttrRequest(code, id);
            var resp = await Messager.SendRRData(request);
            return resp;
        }

        public async Task<IMessageRouterResponse> ServiceWithData(byte code, ushort attr, byte data)
        {
            var request = CreateServiceWithData(code, attr, data);
            var resp = await Messager.SendRRData(request);
            return resp;
        }

        public async Task<IMessageRouterResponse> ServiceWithData(byte code, byte[] data)
        {
            var request = CreateServiceWithData(code, data);
            var resp = await Messager.SendRRData(request);
            return resp;
        }

        public async Task<IMessageRouterResponse> CallService(byte code, int inst, byte[] param)
        {
            var request = CreateCallService(code, inst, param);
            var resp = await Messager.SendRRData(request);
            return resp;
        }

        public async Task<IMessageRouterResponse> CallService(byte code, int inst, int attr, byte[] param)
        {
            var request = CreateCallService(code, inst, attr, param);
            var resp = await Messager.SendRRData(request);
            return resp;
        }

        public static async Task GetAttributeSingleWithSendRRData(string attributeName, object cipObject,
            ICipMessager messager)
        {
            var attributeId = CipAttributeHelper.AttributeNameToId(cipObject.GetType(), attributeName);

            var baseObject = cipObject as CipBaseObject;

            if (attributeId.HasValue && (baseObject != null))
            {
                var request = baseObject.GetAttributeSingleRequest(attributeId.Value);

                // send
                if (messager != null)
                {
                    var response = await messager.SendRRData(request);
                    if ((response != null) && (response.GeneralStatus == (byte)CipGeneralStatusCode.Success))
                    {
                        var p = cipObject.GetType().GetProperty(attributeName);

                        if (p != null)
                        {
                            var cipDetailInfo =
                                (CipDetailInfoAttribute)
                                p.GetCustomAttributes(typeof(CipDetailInfoAttribute), true).FirstOrDefault();

                            if (cipDetailInfo != null)
                            {
                                // Parse
                                var startIndex = 0;
                                var propertyValue =
                                    CipAttributeHelper.Parse(
                                        p.PropertyType,
                                        response.ResponseData,
                                        cipDetailInfo.ArraySize,
                                        ref startIndex);
                                p.SetValue(cipObject, propertyValue);
                            }


                        }

                    }
                }
            }
        }

        public static async Task GetAttributeSingleWithSendUnitData(string attributeName, object cipObject,
            ICipMessager messager)
        {
            var attributeId = CipAttributeHelper.AttributeNameToId(cipObject.GetType(), attributeName);

            var baseObject = cipObject as CipBaseObject;

            if (attributeId.HasValue && (baseObject != null))
            {
                var request = baseObject.GetAttributeSingleRequest(attributeId.Value);

                // send
                if (messager != null)
                {
                    var response = await messager.SendUnitData(request);
                    if ((response != null) && (response.GeneralStatus == (byte)CipGeneralStatusCode.Success))
                    {
                        var p = cipObject.GetType().GetProperty(attributeName);

                        if (p != null)
                        {
                            var cipDetailInfo =
                                (CipDetailInfoAttribute)
                                p.GetCustomAttributes(typeof(CipDetailInfoAttribute), true).FirstOrDefault();

                            if (cipDetailInfo != null)
                            {
                                // Parse
                                var startIndex = 0;
                                var propertyValue =
                                    CipAttributeHelper.Parse(
                                        p.PropertyType,
                                        response.ResponseData,
                                        cipDetailInfo.ArraySize,
                                        ref startIndex);
                                p.SetValue(cipObject, propertyValue);
                            }


                        }

                    }
                }
            }
        }
    }
}