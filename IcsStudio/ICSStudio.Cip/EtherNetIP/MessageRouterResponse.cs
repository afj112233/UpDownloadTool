using System;
using System.Runtime.InteropServices;

namespace ICSStudio.Cip.EtherNetIP
{
    /// <summary>
    /// Table B-1.1 CIP General Status Codes
    /// </summary>
    public enum CipGeneralStatusCode : byte
    {
        Success = 0x00,
        ConnectionFailure,
        ResourceUnavailable,
        InvalidParameterValue,
        PathSegmentError,
        PathDestinationUnknown,
        PartialTransfer,
        ConnectionLost,
        ServiceNotSupported,
        InvalidAttributeValue,
        AttributeListError,
        InRequestState,
        ObjectStateConflict,
        ObjectAlreadyExisted,
        AttributeNotSettable,
        PrivilegeViolation,
        DeviceStateConflict,
        ReplyDataTooLarge,
        FragmentOfPrimitiveValue,
        NotEnoughData,
        AttributeNotSupported,
        TooMuchData,
        ObjectNotExists,
        NotInProgress,
        NoStoredAttributeData,
        StopOperationFailure,
        RoutingFailureRequestTooLarge,
        RoutingFailureResponseTooLarge,
        MissingAttributeData,
        InvalidAttributeValueList,
        EmbeddedServiceError,
        VendorSpecificError,
        InvalidParameter,
        MediumAlreadyWritten,
        InvalidReplyReceived,
        BufferOverflow,
        MessageFormatError,
        KeyFailureInPath,
        PathSizeInvalid,
        UnexpectedAttributeInList,
        InvalidMemberId,
        MemberNotSettable,
        Group2OnlyServerGeneralFailure,
        UnknownModbusError,
        AttributeNotGettable,
        InstanceNotDeletable,
        ServiceNotSupportedForSpecifiedPath,
    }

    public class MessageRouterResponse : IMessageRouterResponse
    {
        public MessageRouterResponse()
        {
            Service = 0;
            GeneralStatus = 0;
            AdditionalStatus = null;
            ResponseData = null;
        }

        public byte Service { get; private set; }

        public byte Reserved { get; private set; }

        public byte GeneralStatus { get; private set; }
        public ushort[] AdditionalStatus { get; private set; }
        public byte[] ResponseData { get; private set; }

        private int Parse(byte[] messageRouterResponseData)
        {
            // copy 
            Service = (byte) (messageRouterResponseData[0] & 0x7F);
            Reserved = messageRouterResponseData[1];
            GeneralStatus = messageRouterResponseData[2];
            var sizeOfAdditionalStatus = messageRouterResponseData[3];

            // additional status
            if (sizeOfAdditionalStatus == 0)
            {
                AdditionalStatus = null;
            }
            else
            {
                AdditionalStatus = new ushort[sizeOfAdditionalStatus];
                for (var i = 0; i < sizeOfAdditionalStatus; i++)
                    AdditionalStatus[i] = BitConverter.ToUInt16(messageRouterResponseData, 4 + i * 2);
            }

            // response data
            var responseDataLength = messageRouterResponseData.Length - (4 + 2 * sizeOfAdditionalStatus);
            if (responseDataLength == 0)
            {
                ResponseData = null;
            }
            else if (responseDataLength > 0)
            {
                ResponseData = new byte[responseDataLength];

                Array.Copy(messageRouterResponseData, 4 + 2 * sizeOfAdditionalStatus, ResponseData, 0,
                    responseDataLength);
            }
            else
            {
                // wrong length
                Console.WriteLine("response data:wrong length");
            }

            return 0;
        }

        public int ParseUnconnectedMessageData(IntPtr data)
        {
            var dataHeader =
                (CIPUnconnectedMessageDataHeader) Marshal.PtrToStructure(data,
                    typeof(CIPUnconnectedMessageDataHeader));

            var messageRouterResponseDataPtr = data +
                                               Marshal.SizeOf(typeof(CIPUnconnectedMessageDataHeader));

            var messageRouterResponseData = new byte[dataHeader.data_length];

            Marshal.Copy(messageRouterResponseDataPtr, messageRouterResponseData, 0, dataHeader.data_length);

            return Parse(messageRouterResponseData);
        }

        public int ParseUnconnectedMessageData(byte[] data)
        {
            // offset header
            var headerSize = Marshal.SizeOf(typeof(CIPUnconnectedMessageDataHeader));

            var messageRouterResponseData = new byte[data.Length - headerSize];

            Array.Copy(data, headerSize, messageRouterResponseData, 0, messageRouterResponseData.Length);

            return Parse(messageRouterResponseData);
        }

        public override string ToString()
        {
            if (ResponseData == null)
                return string.Empty;

            return ICSStudio.Utils.Utils.BytesToHex(ResponseData);
        }

        public int ParseConnectedMessageData(IntPtr data)
        {
            var dataHeader =
                (CIPConnectedMessageDataHeader) Marshal.PtrToStructure(data,
                    typeof(CIPConnectedMessageDataHeader));

            var messageRouterResponseDataPtr = data +
                                               Marshal.SizeOf(typeof(CIPConnectedMessageDataHeader));


            var dataLength = dataHeader.data_length - 2;
            var messageRouterResponseData = new byte[dataLength];

            Marshal.Copy(messageRouterResponseDataPtr, messageRouterResponseData, 0, dataLength);

            return Parse(messageRouterResponseData);
        }

        public int ParseConnectedMessageData(byte[] data)
        {
            // offset header
            var headerSize = Marshal.SizeOf(typeof(CIPConnectedMessageDataHeader));

            var messageRouterResponseData = new byte[data.Length - headerSize];

            Array.Copy(data, headerSize, messageRouterResponseData, 0, messageRouterResponseData.Length);

            return Parse(messageRouterResponseData);
        }


        #region TryParse

        public static bool TryParseByte(byte[] data, ref int startIndex, ref byte result)
        {
            if (data.Length < sizeof(byte) + startIndex)
                return false;

            var b = data[startIndex];

            result = b;
            startIndex++;

            return true;
        }

        public static bool TryParseUInt16(byte[] data, ref int startIndex, ref ushort result)
        {
            if (data.Length < sizeof(ushort) + startIndex)
                return false;

            result = BitConverter.ToUInt16(data, startIndex);
            startIndex += sizeof(ushort);

            return true;
        }

        public static bool TryParseUInt32(byte[] data, ref int startIndex, ref uint result)
        {
            if (data.Length < sizeof(uint) + startIndex)
                return false;

            result = BitConverter.ToUInt32(data, startIndex);
            startIndex += sizeof(uint);

            return true;
        }

        internal static bool TryParseByteArray(byte[] data, ref int startIndex, byte[] outData, byte outDataSize)
        {
            if (data.Length < outDataSize + startIndex)
                return false;

            Array.Copy(data, startIndex, outData, 0, outDataSize);
            startIndex += outDataSize;

            return true;
        }

        #endregion
    }
}