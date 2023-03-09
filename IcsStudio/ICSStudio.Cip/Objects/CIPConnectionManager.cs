using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ICSStudio.Cip.DataTypes;
using ICSStudio.Cip.EtherNetIP;

namespace ICSStudio.Cip.Objects
{
    public class CIPConnectionManager : CipBaseObject
    {
        private readonly object _connectionIdLock;
        private readonly ushort _pseudoRandomNumber;
        private ushort _connectionNumber;

        public CIPConnectionManager(ushort instanceId, ICipMessager messager,
            CipObjectClassCode classCode = CipObjectClassCode.ConnectionManager) : base((ushort) classCode, instanceId,
            messager)
        {
            var rd = new Random();
            _pseudoRandomNumber = (ushort) rd.Next();
            _connectionNumber = 1;
            _connectionIdLock = new object();

            Reserved = new byte[3];
        }

        public async Task<bool> ForwardOpen()
        {
            var request = GetForwardOpenRequest();
            var response = await Messager.SendRRData(request);

            if (response?.GeneralStatus == (byte) CipGeneralStatusCode.Success)
            {
                // TODO(gjc): add more
                O2TConnectionId = BitConverter.ToUInt32(response.ResponseData, 0);
                Debug.WriteLine("O2TConnectionId is " + O2TConnectionId);

                return true;
            }

            return false;
        }

        public MessageRouterRequest GetForwardOpenRequest()
        {
            var request = new MessageRouterRequest
            {
                Service = (byte) CipServiceCode.ForwardOpen,
                RequestPath = new PaddedEPath(ClassId, InstanceId),
                RequestData = null
            };

            // for connectionId
            T2OConnectionId = GenerateConnectionId();
            O2TConnectionId = T2OConnectionId + 1;
            ConnectionSerialNumber = (ushort) (T2OConnectionId & 0xFFFF);
            //

            request.RequestData = GetForwardOpenRequestData();

            return request;
        }

        private byte[] GetForwardOpenRequestData()
        {
            var byteList = new List<byte>(64)
            {
                PriorityAndTimeTick,
                TimeOutTicks
            };

            byteList.AddRange(BitConverter.GetBytes(O2TConnectionId));
            byteList.AddRange(BitConverter.GetBytes(T2OConnectionId));
            byteList.AddRange(BitConverter.GetBytes(ConnectionSerialNumber));
            byteList.AddRange(BitConverter.GetBytes(OriginatorVendorId));
            byteList.AddRange(BitConverter.GetBytes(OriginatorSerialNumber));
            byteList.Add(ConnectionTimeoutMultiplier);
            byteList.AddRange(Reserved);
            byteList.AddRange(BitConverter.GetBytes(O2TRpi));
            byteList.AddRange(BitConverter.GetBytes(O2TNetworkConnectionParameters));
            byteList.AddRange(BitConverter.GetBytes(T2ORpi));
            byteList.AddRange(BitConverter.GetBytes(T2ONetworkConnectionParameters));
            byteList.Add(TransportClassAndTrigger);
            byteList.AddRange(ConnectionPath.ToByteArray());

            return byteList.ToArray();
        }

        private uint GenerateConnectionId()
        {
            uint connectionId;
            lock (_connectionIdLock)
            {
                connectionId = ((uint) _pseudoRandomNumber << 16) | _connectionNumber;
                _connectionNumber++;
            }

            return connectionId;
        }

        #region Forward Open Request Parameters

        public byte PriorityAndTimeTick { get; set; }
        public byte TimeOutTicks { get; set; }
        public uint O2TConnectionId { get; set; }
        public uint T2OConnectionId { get; set; }
        public ushort ConnectionSerialNumber { get; set; }
        public ushort OriginatorVendorId { get; set; }
        public uint OriginatorSerialNumber { get; set; }
        public byte ConnectionTimeoutMultiplier { get; set; }
        public byte[] Reserved { get; set; }
        public uint O2TRpi { get; set; }
        public ushort O2TNetworkConnectionParameters { get; set; }
        public uint T2ORpi { get; set; }
        public ushort T2ONetworkConnectionParameters { get; set; }
        public byte TransportClassAndTrigger { get; set; }
        public PaddedEPath ConnectionPath { get; set; }

        #endregion

        #region Public Method

        public MessageRouterRequest CreateUnconnectedSendRequest(
            int slot, IMessageRouterRequest embeddedMessageRequest)
        {
            var request = new MessageRouterRequest
            {
                Service = (byte) CipServiceCode.UnconnectedSend,
                RequestPath = new PaddedEPath(ClassId, InstanceId),
                RequestData = null
            };

            request.RequestData = GetUnconnectedSendRequestData(slot, embeddedMessageRequest);

            return request;
        }

        private byte[] GetUnconnectedSendRequestData(int slot, IMessageRouterRequest embeddedMessageRequest)
        {
            // vol1 Table 3-5.24 Unconnected_Send Service Request Parameters
            // page 115

            byte priorityAndTimeTick = 7;
            byte timeoutTicks = 249;

            ushort embeddedMessageRequestSize = (ushort) embeddedMessageRequest.ToByteArray().Length;

            var byteList = new List<byte>(64)
            {
                priorityAndTimeTick,
                timeoutTicks
            };

            byteList.AddRange(BitConverter.GetBytes(embeddedMessageRequestSize));
            byteList.AddRange(embeddedMessageRequest.ToByteArray());

            // Pad
            if (embeddedMessageRequestSize % 2 == 1)
                byteList.Add(0);

            byteList.Add(1); // Path size
            byteList.Add(0); // reserved
            byteList.Add(1); // Port 1= backplane
            byteList.Add((byte) slot); //Slot

            return byteList.ToArray();
        }

        #endregion
    }
}