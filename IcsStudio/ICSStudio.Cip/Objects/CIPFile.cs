using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ICSStudio.Cip.DataTypes;
using ICSStudio.Cip.EtherNetIP;

namespace ICSStudio.Cip.Objects
{
    public enum FileAttributeId : ushort
    {
        State = 1,
        InstanceName = 2,
        FileName = 4,
        FileRevision = 5,
        FileSize = 6,
        FileChecksum = 7
    }

    // vol 1, Table 5A-42.3
    public enum StateOfFileObject : byte
    {
        Nonexistent = 0,
        FileEmpty,
        FileLoaded,
        TransferUploadInitiated,
        TransferDownloadInitiated,
        TransferUploadInProgress,
        TransferDownloadInProgress,
        Storing
    }

    // vol 1,TAble 5A-42.18
    public enum TransferPacketType : byte
    {
        FirstTransferPacket = 0,
        MiddleTransferPacket,
        LastTransferPacket,
        AbortTransfer,
        FirstAndLastPacket
    }

    public enum FileInstancesNumber : ushort
    {
        DeviceConfigFile = 0x66,
        DeviceEDSFile = 0xC8,
        DeviceRelatedEDSFile = 0xC9
    }

    public class CIPFile : CipBaseObject
    {
        public CIPFile(ushort instanceId, ICipMessager messager) : base((ushort) CipObjectClassCode.File, instanceId,
            messager)
        {
        }

        public async Task<string> UploadFile(string savePath)
        {
            // state
            await GetAttributeSingle("State");

            var state = Convert.ToByte(State);
            if (state == (byte) StateOfFileObject.FileLoaded)
            {
                // file name
                await GetAttributeSingle("FileName");
                var fileName = FileName.GetFirstString();
                Logger.Info($"{fileName}");

                // file revision
                await GetAttributeSingle("FileRevision");
                var major = FileRevision.Major;
                var minor = FileRevision.Minor;
                Logger.Info($"{major},{minor}");

                // file size
                await GetAttributeSingle("FileSize");
                var fileSize = Convert.ToUInt32(FileSize);
                Logger.Info($"File Size: {fileSize}");

                var fileBuffer = new byte[fileSize];
                var fileBufferIndex = 0;

                var initiateUploadResult = await InitiateUpload();

                if ((initiateUploadResult != null) && (initiateUploadResult.FileSize == fileSize))
                {
                    var totalTransferCount = initiateUploadResult.FileSize / initiateUploadResult.TransferSize;
                    if (initiateUploadResult.FileSize % initiateUploadResult.TransferSize != 0)
                        totalTransferCount++;

                    var leftFileSize = initiateUploadResult.FileSize;

                    for (var i = 0; i < totalTransferCount; i++)
                    {
                        // UploadTransferRequest
                        var transferNumber = (byte) i;
                        var transferSize = (byte) (leftFileSize < initiateUploadResult.TransferSize
                            ? leftFileSize
                            : initiateUploadResult.TransferSize);

                        var uploadTransferResponse = await UploadTransfer(transferNumber, transferSize);
                        if (uploadTransferResponse == null)
                        {
                            // add error log
                            Logger.Error("File UploadTransfer failed!");
                            return null;
                        }

                        if (uploadTransferResponse.TransferPacketType == (byte) TransferPacketType.AbortTransfer)
                        {
                            // abort transfer
                            Logger.Error("File AbortTransfer!");
                            return null;
                        }

                        // copy
                        Array.Copy(uploadTransferResponse.FileData, 0, fileBuffer, fileBufferIndex, transferSize);
                        fileBufferIndex += transferSize;

                        // check sum
                        if ((uploadTransferResponse.TransferPacketType ==
                             (byte) TransferPacketType.LastTransferPacket) ||
                            (uploadTransferResponse.TransferPacketType == (byte) TransferPacketType.FirstAndLastPacket))
                        {
                            var checkSum = GetCheckSum(fileBuffer);
                            if (checkSum != uploadTransferResponse.Checksum)
                            {
                                // checkSum failed
                                Logger.Error("File check sum failed! " + uploadTransferResponse.Checksum + " " +
                                             checkSum);
                                return null;
                            }
                        }

                        leftFileSize -= initiateUploadResult.TransferSize;
                    }


                    // save file
                    string fullFileName = $"{savePath}\\{fileName}";
                    var fs = new FileStream(fullFileName, FileMode.Create);
                    fs.Write(fileBuffer, 0, fileBuffer.Length);
                    fs.Close();

                    return fullFileName;
                }
                else
                {
                    Logger.Error("File Initiate Upload failed!");
                    return null;
                }
            }
            else
            {
                Logger.Warn("File state is not FileLoaded, is " + (StateOfFileObject) state);
                Debug.WriteLine("File state is not FileLoaded, is " + (StateOfFileObject) state);
                return null;
            }
        }

        public async Task<int> DownloadFile(string downloadFileName)
        {

            // File Size
            byte[] fileBuffer = LoadFile(downloadFileName);
            uint fileSize = (uint) fileBuffer.Length;

            ushort checkSum = GetCheckSum(fileBuffer);

            // File Format Version,vendor-specific
            ushort fileFormatVersion = 1;

            // File Revision
            CipRevision fileRevision = GerFileRevision(downloadFileName);

            // Initiate Download
            var initiateDownloadResult =
                await InitiateDownload(
                    fileSize,
                    fileFormatVersion,
                    fileRevision,
                    Path.GetFileName(downloadFileName));


            if (initiateDownloadResult != null)
            {
                var totalTransferCount = fileSize / initiateDownloadResult.TransferSize;
                if (fileSize % initiateDownloadResult.TransferSize != 0)
                    totalTransferCount++;

                var leftFileSize = fileSize;
                int startIndex = 0;

                Debug.WriteLine("total:" + totalTransferCount);

                for (var i = 0; i < totalTransferCount; i++)
                {
                    Debug.WriteLine("send index:" + i);

                    // Download transfer
                    var transferNumber = (byte) i;

                    byte transferPacketType = (byte) TransferPacketType.MiddleTransferPacket;

                    if (totalTransferCount == 1)
                    {
                        transferPacketType = (byte) TransferPacketType.FirstAndLastPacket;
                    }
                    else
                    {
                        if (i == 0)
                        {
                            transferPacketType = (byte) TransferPacketType.FirstTransferPacket;
                        }
                        else if (i == (totalTransferCount - 1))
                        {
                            transferPacketType = (byte) TransferPacketType.LastTransferPacket;
                        }
                    }

                    var transferSize = (byte) (leftFileSize < initiateDownloadResult.TransferSize
                        ? leftFileSize
                        : initiateDownloadResult.TransferSize);

                    byte[] fileData = new byte[transferSize];
                    Array.Copy(fileBuffer, startIndex, fileData, 0, transferSize);
                    leftFileSize -= transferSize;
                    startIndex += transferSize;

                    // Download Transfer
                    var downloadTransferResult =
                        await DownloadTransfer(transferNumber, transferPacketType, fileData, checkSum);
                    if (downloadTransferResult != null)
                    {
                        if (downloadTransferResult.TransferNumber != transferNumber)
                        {
                            Logger.Error(
                                $"download Transfer response number different! {transferNumber},{downloadTransferResult.TransferNumber}");
                            return -1;
                        }
                    }
                    else
                    {
                        Logger.Error($"download Transfer failed! i:{i}, total:{totalTransferCount}");
                        return -1;
                    }

                }

                Debug.WriteLine("Send finished!");
                return 0;
            }

            Logger.Error("InitiateDownload failed!");
            return -1;
        }

        #region Cip Attribute

        [CipDetailInfo((ushort) FileAttributeId.State, "State")]
        public CipUsint State { get; set; }

        [CipDetailInfo((ushort) FileAttributeId.InstanceName, "Instance Name")]
        public CipStringi InstanceName { get; set; }

        [CipDetailInfo((ushort) FileAttributeId.FileName, "File Name")]
        public CipStringi FileName { get; set; }

        [CipDetailInfo((ushort) FileAttributeId.FileRevision, "File Revision")]
        public CipRevision FileRevision { get; set; }

        [CipDetailInfo((ushort) FileAttributeId.FileSize, "File Size")]
        public CipUdint FileSize { get; set; }

        [CipDetailInfo((ushort) FileAttributeId.FileChecksum, "File Checksum")]
        public CipUint FileChecksum { get; set; }

        #endregion

        #region private

        private async Task<InitiateUploadResponse> InitiateUpload()
        {
            // Initialize UploadRequest
            var request = InitiateUploadRequest();

            // Send
            var response = await Messager.SendRRData(request);

            if ((response != null) && (response.GeneralStatus == (byte) CipGeneralStatusCode.Success))
                if (response.ResponseData.Length == 5)
                {
                    var result = new InitiateUploadResponse
                    {
                        FileSize = BitConverter.ToUInt32(response.ResponseData, 0),
                        TransferSize = response.ResponseData[4]
                    };

                    return result;
                }

            return null;
        }

        private IMessageRouterRequest InitiateUploadRequest(byte maxTransferSize = 0xff)
        {
            var request = new MessageRouterRequest
            {
                Service = (byte) CipServiceCode.InitiateUpload,
                RequestPath = new PaddedEPath(ClassId, InstanceId),
                RequestData = new[] {maxTransferSize}
            };

            return request;
        }

        private async Task<UploadTransferResponse> UploadTransfer(byte transferNumber, byte transferSize)
        {
            var request = UploadTransferRequest(transferNumber);

            // Send
            var response = await Messager.SendRRData(request);

            if ((response != null) && (response.GeneralStatus == (byte) CipGeneralStatusCode.Success))
            {
                var result = new UploadTransferResponse(transferSize);
                var startIndex = 0;

                // TransferNumber
                if (MessageRouterResponse.TryParseByte(response.ResponseData, ref startIndex,
                    ref result.TransferNumber))
                    if (result.TransferNumber == transferNumber)
                        if (MessageRouterResponse.TryParseByte(response.ResponseData, ref startIndex,
                            ref result.TransferPacketType))
                        {
                            if (result.TransferPacketType == (byte) TransferPacketType.AbortTransfer)
                                return result;

                            // File Data
                            if (MessageRouterResponse.TryParseByteArray(response.ResponseData, ref startIndex,
                                result.FileData, transferSize))
                            {
                                // Transfer Checksum
                                if ((result.TransferPacketType == (byte) TransferPacketType.LastTransferPacket) ||
                                    (result.TransferPacketType == (byte) TransferPacketType.FirstAndLastPacket))
                                {
                                    if (MessageRouterResponse.TryParseUInt16(response.ResponseData, ref startIndex,
                                        ref result.Checksum))
                                        return result;

                                    // parse checksum failed
                                    return null;
                                }

                                return result;
                            }
                        }
            }

            return null;
        }

        private IMessageRouterRequest UploadTransferRequest(byte transferNumber)
        {
            var request = new MessageRouterRequest
            {
                Service = (byte) CipServiceCode.UploadTransfer,
                RequestPath = new PaddedEPath(ClassId, InstanceId),
                RequestData = new[] {transferNumber}
            };

            return request;
        }

        private ushort GetCheckSum(byte[] dataBytes)
        {
            // vol1, 5A-42.2.1.6
            ushort checkSum = 0;
            foreach (var dataByte in dataBytes)
                checkSum += dataByte;

            return (ushort) ((uint) 0x10000 - checkSum);
        }

        private byte[] LoadFile(string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            byte[] fileBuffer = new byte[fileInfo.Length];

            var fs = fileInfo.OpenRead();

            int leftSize = fileBuffer.Length;
            int offSet = 0;
            int result = 0;

            while (true)
            {
                var readSize = fs.Read(fileBuffer, offSet, leftSize);
                if (readSize > 0)
                {
                    offSet += readSize;
                    leftSize -= readSize;
                }
                else
                {
                    result = -1;
                    break;
                }

                if (leftSize == 0)
                    break;
            }

            fs.Close();

            if (result < 0)
                return null;

            return fileBuffer;

        }

        // ReSharper disable once UnusedParameter.Local
        private CipRevision GerFileRevision(string fileName)
        {
            // TODO: need edit
            return new CipRevision() {Major = 1, Minor = 1};

        }

        private async Task<InitiateDownloadResponse> InitiateDownload(
            uint fileSize,
            ushort fileFormatVersion,
            CipRevision fileRevision,
            string fileName)
        {
            // InitiateUploadRequest
            var request = InitiateDownloadRequest(fileSize, fileFormatVersion, fileRevision, fileName);

            // Send
            var response = await Messager.SendRRData(request);
            if ((response != null) && (response.GeneralStatus == (byte) CipGeneralStatusCode.Success))
            {
                var result = new InitiateDownloadResponse();
                var startIndex = 0;

                // Incremental Burn
                if (MessageRouterResponse.TryParseUInt32(response.ResponseData, ref startIndex,
                    ref result.IncrementalBurn))
                {
                    // Incremental Burn Time
                    if (MessageRouterResponse.TryParseUInt16(response.ResponseData, ref startIndex,
                        ref result.IncrementalBurnTime))
                    {
                        if (MessageRouterResponse.TryParseByte(response.ResponseData, ref startIndex,
                            ref result.TransferSize))
                        {
                            return result;
                        }
                    }
                }

            }

            return null;
        }

        private async Task<DownloadTransferResponse> DownloadTransfer(
            byte transferNumber,
            byte transferPacketType,
            byte[] fileData,
            ushort checkSum = 0)
        {
            // DownloadTransferRequest
            var request = DownloadTransferRequest(transferNumber, transferPacketType, fileData, checkSum);

            // Send
            var response = await Messager.SendRRData(request);
            if ((response != null) && (response.GeneralStatus == (byte) CipGeneralStatusCode.Success))
            {
                var result = new DownloadTransferResponse();
                var startIndex = 0;

                if (MessageRouterResponse.TryParseByte(response.ResponseData, ref startIndex,
                    ref result.TransferNumber))
                {
                    return result;
                }

            }

            return null;
        }

        private IMessageRouterRequest InitiateDownloadRequest(
            uint fileSize,
            ushort fileFormatVersion,
            CipRevision fileRevision,
            string fileName)
        {
            var request = new MessageRouterRequest
            {
                Service = (byte) CipServiceCode.InitiateDownload,
                RequestPath = new PaddedEPath(ClassId, InstanceId),
                RequestData = null
            };

            List<byte> byteList = new List<byte>();

            byteList.AddRange(BitConverter.GetBytes(fileSize));
            byteList.AddRange(BitConverter.GetBytes(fileFormatVersion));
            byteList.AddRange(fileRevision.GetBytes());

            // File Name
            CipStringi cipFileName = new CipStringi();
            cipFileName.AddString(Path.GetFileName(fileName));
            byteList.AddRange(cipFileName.GetBytes());

            request.RequestData = byteList.ToArray();

            return request;
        }

        private IMessageRouterRequest DownloadTransferRequest(
            byte transferNumber,
            byte transferPacketType,
            byte[] fileData,
            ushort checkSum)
        {
            var request = new MessageRouterRequest
            {
                Service = (byte) CipServiceCode.DownloadTransfer,
                RequestPath = new PaddedEPath(ClassId, InstanceId),
                RequestData = null
            };

            List<byte> byteList = new List<byte> {transferNumber, transferPacketType};
            byteList.AddRange(fileData);

            if (transferPacketType == (byte) TransferPacketType.LastTransferPacket ||
                transferPacketType == (byte) TransferPacketType.FirstAndLastPacket)
            {
                byteList.AddRange(BitConverter.GetBytes(checkSum));
            }

            request.RequestData = byteList.ToArray();

            return request;
        }

        #endregion
    }

    #region Sub Class

    public class InitiateUploadResponse
    {
        public uint FileSize;
        public byte TransferSize;
    }

    public class UploadTransferResponse
    {
        public ushort Checksum;
        public byte[] FileData;
        public byte TransferNumber;
        public byte TransferPacketType;

        public UploadTransferResponse(byte transferSize)
        {
            FileData = new byte[transferSize];
        }
    }

    public class InitiateDownloadResponse
    {
        public uint IncrementalBurn;
        public ushort IncrementalBurnTime;
        public byte TransferSize;
    }

    public class DownloadTransferResponse
    {
        public byte TransferNumber;
    }

    #endregion
}
