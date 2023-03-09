using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using ICSStudio.Cip.EtherNetIP;
using NLog;

namespace ICSStudio.CipConnection
{
    public class CipSyncConnection : IDisposable
    {
        protected static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly object _droppedRoot = new object();
        private readonly SemaphoreSlim _matchPacketSemaphore;

        private readonly ArrayList _packetPairList;
        private readonly string _remoteHost;
        private ConcurrentQueue<EncapsulationPacket> _requestPacketQueue;
        private ConcurrentQueue<EncapsulationPacket> _responsePacketQueue;
        private readonly object _senderContextRoot = new object();

        private readonly SemaphoreSlim _sendPacketSemaphore;
        private readonly int _serverPort;

        private readonly TcpClient _tcpClient;

        private Thread _matchThread;

        private Thread _recvThread;

        private ulong _senderContextIndex;
        private Thread _sendThread;

        public CipSyncConnection(string remoteHost, int sendTimeout, int receiveTimeout, int port = 0xAF12)
        {
            IsDropped = false;

            _tcpClient = new TcpClient
            {
                NoDelay = true,
                SendTimeout = sendTimeout,
                ReceiveTimeout = receiveTimeout
            };

            _remoteHost = remoteHost;
            _serverPort = port;

            _sendPacketSemaphore = new SemaphoreSlim(0);
            _matchPacketSemaphore = new SemaphoreSlim(0);
            _requestPacketQueue = new ConcurrentQueue<EncapsulationPacket>();
            _responsePacketQueue = new ConcurrentQueue<EncapsulationPacket>();
            _packetPairList = new ArrayList();
        }

        public bool IsDropped { get; private set; }

        internal TcpClient TcpClient => _tcpClient;

        public void Dispose()
        {
            DropConnection();
            try
            {
                _sendThread.Abort();
                _recvThread.Abort();
                _matchThread.Abort();
                _sendPacketSemaphore.Dispose();
                _matchPacketSemaphore.Dispose();
                _requestPacketQueue = null;
                _responsePacketQueue = null;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                //ignore
            }
            //TODO(gjc): add here
        }

        public event EventHandler Disconnected = delegate { };

        public delegate void UnRegisterSessionReceivedEventHandler(uint sessionHandle);

        public event UnRegisterSessionReceivedEventHandler UnRegisterSessionReceived;

        public int Connect(int millisecondsTimeout = 1000)
        {
            try
            {
                if (_tcpClient.ConnectAsync(_remoteHost, _serverPort).Wait(millisecondsTimeout))
                {
                    Debug.WriteLine("Connect success!");

                    _sendThread = new Thread(SendTask) {IsBackground = true};
                    _recvThread = new Thread(RecvTask) {IsBackground = true};
                    _matchThread = new Thread(MatchTask) {IsBackground = true};

                    _sendThread.Start();
                    _recvThread.Start();
                    _matchThread.Start();

                    return 0;
                }

                Debug.WriteLine("Connect timeout!");
                Logger.Debug("Connect timeout!");

                DropConnection();

                return -1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return -1;
            }
        }

        private static byte[] StructToBytes(object structObject)
        {
            var size = Marshal.SizeOf(structObject);
            var bytes = new byte[size];
            var structPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structObject, structPtr, false);
            Marshal.Copy(structPtr, bytes, 0, size);
            Marshal.FreeHGlobal(structPtr);
            return bytes;
        }

        public void DropConnection()
        {
            lock (_droppedRoot)
            {
                if (IsDropped)
                    return;

                IsDropped = true;
            }

            Debug.WriteLine("DropConnection");

            _tcpClient.Close();

            Disconnected?.Invoke(this, null);
        }

        #region Encapsulation Commands, page 24,vol2

        public int NOP()
        {
            EncapsulationHeader header;

            header.command = (ushort) EncapsulationCommandType.NOP;
            header.length = 0;
            header.session_handle = 0;
            header.status = 0;
            header.sender_context = new byte[8];
            header.options = 0;

            var packet = new EncapsulationPacket(header, null);

            SendRequestPacket(packet);

            return 0;
        }

        public int ListServices()
        {
            var packet = CreateListServicesPacket();

            SendRequestPacket(packet);

            return 0;
        }

        public async Task<EncapsulationPacket> ListServicesAsync()
        {
            var requestPacket = CreateListServicesPacket();

            var responsePacket = await SendPacketAsync(requestPacket);

            return responsePacket;
        }

        public int ListIdentity(ushort maxResponseDelay = 0)
        {
            var packet = CreateListIdentityPacket(maxResponseDelay);

            SendRequestPacket(packet);

            return 0;
        }

        public async Task<EncapsulationPacket> ListIdentityAsync(ushort maxResponseDelay = 0)
        {
            var requestPacket = CreateListIdentityPacket(maxResponseDelay);

            var responsePacket = await SendPacketAsync(requestPacket);

            return responsePacket;
        }

        public int ListInterfaces()
        {
            var packet = CreateListInterfacesPacket();

            SendRequestPacket(packet);

            return 0;
        }

        public async Task<EncapsulationPacket> ListInterfacesAsync()
        {
            var requestPacket = CreateListInterfacesPacket();

            var responsePacket = await SendPacketAsync(requestPacket);

            return responsePacket;
        }


        public int RegisterSession(ushort protocolVersion = 1, ushort optionFlags = 0)
        {
            var packet = CreateRegisterSessionPacket(protocolVersion, optionFlags);

            SendRequestPacket(packet);

            return 0;
        }

        public async Task<EncapsulationPacket> RegisterSessionAsync(ushort protocolVersion = 1, ushort optionFlags = 0)
        {
            var requestPacket = CreateRegisterSessionPacket(protocolVersion, optionFlags);

            var responsePacket = await SendPacketAsync(requestPacket);

            return responsePacket;
        }

        public int UnRegisterSession(uint sessionHandle)
        {
            EncapsulationHeader header;

            header.command = (ushort) EncapsulationCommandType.UnRegisterSession;
            header.length = 0;
            header.session_handle = sessionHandle;
            header.status = 0;
            header.sender_context = CreateUniqueSenderContext();
            header.options = 0;

            var packet = new EncapsulationPacket(header, null);

            SendRequestPacket(packet);

            return 0;
        }

        public int SendRRData(
            uint sessionHandle,
            byte[] unconnectedData)
        {
            var packet = CreateRRDataPacket(sessionHandle, unconnectedData);

            SendRequestPacket(packet);

            return 0;
        }

        public async Task<EncapsulationPacket> SendRRDataAsync(
            uint sessionHandle,
            byte[] unconnectedData)
        {
            var requestPacket = CreateRRDataPacket(sessionHandle, unconnectedData);

            var responsePacket = await SendPacketAsync(requestPacket);

            return responsePacket;
        }


        public int SendUnitData(
            uint sessionHandle,
            uint connectionId,
            ushort sequenceCount,
            byte[] connectedData)
        {
            // send
            var packet = CreateUnitDataPacket(sessionHandle, connectionId, sequenceCount, connectedData);

            SendRequestPacket(packet);

            return 0;
        }

        public async Task<EncapsulationPacket> SendUnitDataAsync(
            uint sessionHandle,
            uint connectionId,
            ushort sequenceCount,
            byte[] connectedData)
        {
            var requestPacket = CreateUnitDataPacket(sessionHandle, connectionId, sequenceCount, connectedData);

            var responsePacket = await SendPacketAsync(requestPacket);

            return responsePacket;
        }

        #endregion

        #region Private Method

        private void SendTask()
        {
            var ns = _tcpClient.GetStream();

            while (true)
            {
                if (IsDropped)
                    return;

                _sendPacketSemaphore.Wait(100); // 100ms

                EncapsulationPacket requestPacket;
                while (_requestPacketQueue.TryDequeue(out requestPacket))
                {
                    requestPacket.PacketTime = DateTime.Now;

                    var sendbuffer = requestPacket.GetBytes();
                    try
                    {
                        ns.Write(sendbuffer, 0, sendbuffer.Length);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);

                        Logger.Error(ex);

                        DropConnection();

                        return;
                    }

                    //Thread.Sleep(3);
                }
            }
        }

        private void RecvTask()
        {
            var ns = _tcpClient.GetStream();
            //Console.WriteLine($"qqqqqqqqqqa:{");

            // use ReceiveTimeout
            //ns.ReadTimeout = System.Threading.Timeout.Infinite;

            while (true)
            {
                if (IsDropped)
                    return;

                try
                {
                    EncapsulationPacket packet = null;

                    // header
                    var headerBuffer = new byte[24];
                    var header = new EncapsulationHeader();

                    var result = ReadFixedSize(ns, headerBuffer);
                    if (result == 0)
                    {
                        header.ParseBytes(headerBuffer);
                        if (header.length > 0)
                        {
                            // data
                            var dataBuffer = new byte[header.length];
                            result = ReadFixedSize(ns, dataBuffer);

                            if (result == 0)
                                packet = new EncapsulationPacket(header, dataBuffer);
                            else
                                DropConnection();
                        }
                        else
                        {
                            packet = new EncapsulationPacket(header, null);
                        }

                        if (packet != null)
                            OnRecvEncapsulationPacket(packet);
                    }
                    else
                    {
                        DropConnection();
                    }
                }

                catch (Exception ex)
                {
                    Debug.WriteLine($"Connection lost");

                    Logger.Error(ex);
                    //Console.WriteLine($"xxxxx:{ex.StackTrace}");
                    DropConnection();
                }

            }
        }

        private int ReadFixedSize(NetworkStream ns, byte[] buffer)
        {
            var leftSize = buffer.Length;
            var offset = 0;

            while (true)
            {
                var readSize = ns.Read(buffer, offset, leftSize);
                if (readSize == 0)
                {
                    Debug.WriteLine("read size is 0!");

                    Logger.Debug("read size is 0!");

                    return -1;
                }

                offset += readSize;
                leftSize -= readSize;

                if (leftSize == 0)
                    break;
            }

            return 0;
        }

        private void MatchTask()
        {
            while (true)
            {
                // 0. drop
                if (IsDropped)
                {
                    lock (_packetPairList.SyncRoot)
                    {
                        foreach (var item in _packetPairList)
                        {
                            var packetPair = item as EncapsulationPacketPair;
                            if (packetPair != null)
                            {
                                packetPair.Drop();
                            }
                        }

                        _packetPairList.Clear();

                        return;
                    }
                }

                _matchPacketSemaphore.Wait(2); // 10ms

                // 1.match
                EncapsulationPacket responsePacket;
                while (_responsePacketQueue.TryDequeue(out responsePacket))
                    lock (_packetPairList.SyncRoot)
                    {
                        //foreach (var item in _packetPairList)
                        //{
                        //    var packetPair = item as EncapsulationPacketPair;
                        //    if (packetPair != null)
                        //        if (packetPair.TryMatch(responsePacket))
                        //            break;
                        //}
                        int i;
                        for (i = 0; i < _packetPairList.Count; i++)
                        {
                            var packetPair = _packetPairList[i] as EncapsulationPacketPair;
                            if (packetPair != null)
                                if (packetPair.TryMatch(responsePacket))
                                    break;
                        }

                        if (i >= _packetPairList.Count)
                            Debug.WriteLine(responsePacket.PacketHeader.GetSenderContext()
                                            + " not found! "
                                            + responsePacket.PacketTime.ToString("HH: mm:ss.ffff"));
                    }

                // 2. remove done pair
                lock (_packetPairList.SyncRoot)
                {
                    for (var index = _packetPairList.Count - 1; index >= 0; index--)
                    {
                        var packetPair = _packetPairList[index] as EncapsulationPacketPair;
                        if (packetPair == null)
                        {
                            _packetPairList.RemoveAt(index);
                        }
                        else
                        {
                            packetPair.CheckTimeOut();

                            if (packetPair.MatchResult != MatchResultTypes.Waiting)
                                _packetPairList.RemoveAt(index);
                        }
                    }
                }
            }
        }

        private void OnRecvEncapsulationPacket(EncapsulationPacket packet)
        {
            //Debug.WriteLine((EncapsulationCommandType) packet.PacketHeader.command);
            packet.PacketTime = DateTime.Now;

            if (packet.PacketHeader.command == (ushort) EncapsulationCommandType.UnRegisterSession)
            {
                UnRegisterSessionReceived?.Invoke(packet.PacketHeader.session_handle);
            }
            else
            {
                _responsePacketQueue.Enqueue(packet);
                _matchPacketSemaphore.Release();
            }

        }

        private void SendRequestPacket(EncapsulationPacket packet)
        {
            _requestPacketQueue.Enqueue(packet);
            _sendPacketSemaphore.Release();
        }

        private async Task<EncapsulationPacket> SendPacketAsync(EncapsulationPacket requestPacket)
        {
            if (IsDropped)
                return null;

            EncapsulationPacketPair packetPair;
            lock (_packetPairList.SyncRoot)
            {
                //add to packetPairList
                packetPair = new EncapsulationPacketPair(requestPacket);
                _packetPairList.Add(packetPair);
            }

            // send
            SendRequestPacket(requestPacket);

            // waiting response
            await packetPair.MatchedSemaphoreSlim.WaitAsync(20000);
            if (packetPair.MatchResult != MatchResultTypes.Matched)
            {
                Debug.WriteLine($"{packetPair.MatchResult}");

            }

            return packetPair.MatchResult == MatchResultTypes.Matched ? packetPair.Response : null;
        }

        private byte[] CreateUniqueSenderContext()
        {
            byte[] senderContext;

            lock (_senderContextRoot)
            {
                _senderContextIndex++;
                senderContext = BitConverter.GetBytes(_senderContextIndex);
            }

            return senderContext;
        }

        private EncapsulationPacket CreateListServicesPacket()
        {
            EncapsulationHeader header;

            header.command = (ushort) EncapsulationCommandType.ListServices;
            header.length = 0;
            header.session_handle = 0;
            header.status = 0;
            header.sender_context = CreateUniqueSenderContext();
            header.options = 0;

            return new EncapsulationPacket(header, null);
        }

        private EncapsulationPacket CreateListIdentityPacket(ushort maxResponseDelay)
        {
            EncapsulationHeader header;

            header.command = (ushort) EncapsulationCommandType.ListIdentity;
            header.length = 0;
            header.session_handle = 0;
            header.status = 0;
            header.sender_context = new byte[8];
            header.options = 0;

            var delay = BitConverter.GetBytes(maxResponseDelay);
            Array.Copy(delay, header.sender_context, delay.Length);

            for (var i = 2; i < 8; i++)
                header.sender_context[i] = 0;

            return new EncapsulationPacket(header, null);
        }

        private EncapsulationPacket CreateListInterfacesPacket()
        {
            EncapsulationHeader header;

            header.command = (ushort) EncapsulationCommandType.ListInterfaces;
            header.length = 0;
            header.session_handle = 0;
            header.status = 0;
            header.sender_context = CreateUniqueSenderContext();
            header.options = 0;

            return new EncapsulationPacket(header, null);
        }

        private EncapsulationPacket CreateRegisterSessionPacket(ushort protocolVersion, ushort optionFlags)
        {
            EncapsulationHeader header;

            header.command = (ushort) EncapsulationCommandType.RegisterSession;
            header.length = 4;
            header.session_handle = 0;
            header.status = 0;
            header.sender_context = CreateUniqueSenderContext();
            header.options = 0;

            var commandSpecificData = new byte[4];

            var temp = BitConverter.GetBytes(protocolVersion);
            Array.Copy(temp, 0, commandSpecificData, 0, 2);

            temp = BitConverter.GetBytes(optionFlags);
            Array.Copy(temp, 0, commandSpecificData, 2, 2);

            return new EncapsulationPacket(header, commandSpecificData);
        }

        private EncapsulationPacket CreateRRDataPacket(
            uint sessionHandle,
            byte[] unconnectedData)
        {
            CIPUnconnectedMessageDataHeader unconnectedDataHeader;
            unconnectedDataHeader.interface_handle = 0;
            unconnectedDataHeader.time_out = 0;
            unconnectedDataHeader.item_count = 2;

            unconnectedDataHeader.address_type_id = (ushort) CommonPacketFormatItemType.NullAddressItem;
            unconnectedDataHeader.address_length = 0;

            unconnectedDataHeader.data_type_id = (ushort) CommonPacketFormatItemType.UnconnectedDataItem;
            unconnectedDataHeader.data_length = (ushort) unconnectedData.Length;

            // header
            EncapsulationHeader header;
            header.command = (ushort) EncapsulationCommandType.SendRRData;
            header.length = (ushort) (Marshal.SizeOf(typeof(CIPUnconnectedMessageDataHeader))
                                      + unconnectedData.Length);
            header.session_handle = sessionHandle;
            header.status = 0;
            header.sender_context = CreateUniqueSenderContext();
            header.options = 0;

            // copy
            var commandSpecificData = new byte[header.length];

            var unconnectedDataHeaderData = StructToBytes(unconnectedDataHeader);

            Array.Copy(unconnectedDataHeaderData, commandSpecificData, unconnectedDataHeaderData.Length);
            Array.Copy(unconnectedData, 0, commandSpecificData, unconnectedDataHeaderData.Length,
                unconnectedData.Length);

            return new EncapsulationPacket(header, commandSpecificData);
        }

        private EncapsulationPacket CreateUnitDataPacket(
            uint sessionHandle,
            uint connectionId,
            ushort sequenceCount,
            byte[] connectedData)
        {
            CIPConnectedMessageDataHeader connectedDataHeader;
            connectedDataHeader.interface_handle = 0;
            connectedDataHeader.time_out = 0;

            connectedDataHeader.item_count = 2;
            connectedDataHeader.address_type_id = (ushort) CommonPacketFormatItemType.ConnectedAddressItem;
            connectedDataHeader.address_length = 4;
            connectedDataHeader.address_data = connectionId;

            connectedDataHeader.data_type_id = (ushort) CommonPacketFormatItemType.ConnectedDataItem;
            connectedDataHeader.data_length = (ushort) (connectedData.Length + Marshal.SizeOf(typeof(ushort)));
            //// including the CIP Sequence Count
            connectedDataHeader.sequence_count = sequenceCount;

            // header
            EncapsulationHeader header;
            header.command = (ushort) EncapsulationCommandType.SendUnitData;
            header.length = (ushort) (Marshal.SizeOf(typeof(CIPConnectedMessageDataHeader))
                                      + connectedData.Length);
            header.session_handle = sessionHandle;
            header.status = 0;
            header.sender_context = CreateUniqueSenderContext();
            header.options = 0;

            // copy
            var commandSpecificData = new byte[header.length];

            var connectedDataHeaderData = StructToBytes(connectedDataHeader);

            Array.Copy(connectedDataHeaderData, commandSpecificData, connectedDataHeaderData.Length);
            Array.Copy(connectedData, 0, commandSpecificData, connectedDataHeaderData.Length, connectedData.Length);

            return new EncapsulationPacket(header, commandSpecificData);
        }

        #endregion
    }
}