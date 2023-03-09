using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ICSStudio.Cip.DataTypes;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using NLog;

namespace ICSStudio.CipConnection
{
    public class DeviceConnection : ICipMessager
    {
        private readonly ILogger _logger;
        private readonly object _sequenceCountRoot = new object();
        private CipSyncConnection _cipSyncConnection;
        private ushort _sequenceCount;

        public DeviceConnection(string ipAddress = "",
            CipObjectClassCode classCode = CipObjectClassCode.ConnectionManager, ushort instance = 1)
        {
            _logger = LogManager.GetCurrentClassLogger();
            IpAddress = ipAddress;
            ConnectionStatus = ConnectionStatus.Disconnected;

            // TODO(gjc) : for test
            ConnectionManager = new CIPConnectionManager(instance, this, classCode);
        }

        public CIPConnectionManager ConnectionManager { get; set; }
        public string IpAddress { get; set; }

        // TODO(gjc): need edit
        public uint SessionHandle { get; private set; }
        public uint O2TConnectionId { get; private set; }
        public uint T2OConnectionId { get; private set; }

        public ushort SequenceCount
        {
            get
            {
                ushort count;
                lock (_sequenceCountRoot)
                {
                    count = _sequenceCount;
                    _sequenceCount++;
                }

                return count;
            }
            private set
            {
                lock (_sequenceCountRoot)
                {
                    _sequenceCount = value;
                }
            }
        }

        public int SendTimeout { get; set; }

        private int _receiveTimeout;

        public int ReceiveTimeout
        {
            get
            {
                return _receiveTimeout;
            }
            set
            {
                if (_receiveTimeout != value)
                {
                    _receiveTimeout = value;

                    if (_cipSyncConnection != null)
                    {
                        _cipSyncConnection.TcpClient.ReceiveTimeout = value;
                    }
                }
            }
        }
        
        public async Task<IMessageRouterResponse> SendRRData(IMessageRouterRequest request)
        {
            if ((ConnectionStatus == ConnectionStatus.RegisterSession) ||
                (ConnectionStatus == ConnectionStatus.ForwardOpen))
            {
                var responsePacket =
                    await _cipSyncConnection.SendRRDataAsync(SessionHandle, request.ToByteArray());
                if (responsePacket == null)
                {
                    Console.WriteLine("response is null");
                    return null;
                }

                var mrResponse = new MessageRouterResponse();

                mrResponse.ParseUnconnectedMessageData(responsePacket.PacketData);

                return mrResponse;
            }

            return null;
        }

        public async Task<IMessageRouterResponse> SendUnitData(IMessageRouterRequest request)
        {
            if (ConnectionStatus != ConnectionStatus.ForwardOpen)
                return null;

            var responsePacket =
                await _cipSyncConnection.SendUnitDataAsync(
                    SessionHandle,
                    O2TConnectionId,
                    SequenceCount,
                    request.ToByteArray());

            if (responsePacket == null)
                return null;

            var mrResponse = new MessageRouterResponse();

            mrResponse.ParseConnectedMessageData(responsePacket.PacketData);

            return mrResponse;
        }

        public async Task ListIdentity()
        {
            if ((ConnectionStatus == ConnectionStatus.RegisterSession) ||
                (ConnectionStatus == ConnectionStatus.ForwardOpen))
            {
                await _cipSyncConnection.ListIdentityAsync();
            }
        }

        public async Task<int> OnLine(bool beForwardOpen)
        {
            OffLine();

            // connect
            var result = Connect();
            if (result < 0)
                return -1;

            // RegisterSession
            result = await RegisterSession();
            if (result < 0)
                return -1;

            // ForwardOpen
            if (beForwardOpen)
            {
                result = await ForwardOpen();
                if (result < 0)
                    return -1;
            }

            Connected?.Invoke(this, EventArgs.Empty);

            return 0;
        }

        public int OffLine()
        {
            Debug.WriteLine("Device Connection Offline!");

            UnRegisterSession();

            _cipSyncConnection?.DropConnection();

            ConnectionStatus = ConnectionStatus.Disconnected;

            return 0;
        }

        public void SetConnectAddress(string address)
        {
            IpAddress = address;
        }

        public ConnectionStatus ConnectionStatus { get; private set; }

        public event EventHandler Connected;
        public event EventHandler Disconnected;

        #region Private Method

        protected int Connect()
        {
            _cipSyncConnection = new CipSyncConnection(IpAddress, SendTimeout, ReceiveTimeout);
            _cipSyncConnection.Disconnected += OnDisconnect;
            _cipSyncConnection.UnRegisterSessionReceived += OnUnRegisterSessionReceived;

            if (_cipSyncConnection.Connect(20000) < 0)
                return -1;

            ConnectionStatus = ConnectionStatus.Netconnected;
            return 0;
        }

        private void OnUnRegisterSessionReceived(uint sessionHandle)
        {
            if (SessionHandle == sessionHandle)
            {
                Task.Run(() => { OffLine(); });
            }
        }

        protected async Task<int> RegisterSession()
        {
            if (ConnectionStatus == ConnectionStatus.Disconnected)
                return -1;

            var responsePacket = await _cipSyncConnection.RegisterSessionAsync();
            if ((responsePacket != null) && (responsePacket.PacketHeader.status == 0))
            {
                ConnectionStatus = ConnectionStatus.RegisterSession;
                SessionHandle = responsePacket.PacketHeader.session_handle;

                _logger.Debug("SessionHandle is " + SessionHandle);

                return 0;
            }

            _logger.Error("RegisterSession failed!");
            return -1;
        }

        private async Task<int> ForwardOpen()
        {
            if ((ConnectionStatus == ConnectionStatus.Disconnected) ||
                (ConnectionStatus == ConnectionStatus.Netconnected))
                return -1;

            // for test
            ConnectionManager.PriorityAndTimeTick |= 0x7;
            ConnectionManager.TimeOutTicks = 249;

            ConnectionManager.OriginatorVendorId = 0x004d; // Rockwell Software, Inc.
            ConnectionManager.OriginatorSerialNumber = 0x078e3ff2;

            ConnectionManager.ConnectionTimeoutMultiplier = 0;
            ConnectionManager.Reserved[0] = 0;
            ConnectionManager.Reserved[1] = 0;
            ConnectionManager.Reserved[2] = 0;

            ConnectionManager.O2TRpi = 0x007A1200; // 8000ms
            ConnectionManager.O2TNetworkConnectionParameters = 0x43f4;
            ConnectionManager.T2ORpi = 0x007A1200; // 8000ms
            ConnectionManager.T2ONetworkConnectionParameters = 0x43f4;

            ConnectionManager.TransportClassAndTrigger = 0xA3;

            ConnectionManager.ConnectionPath = new PaddedEPath((ushort)CipObjectClassCode.MessageRouter, 0x01);
            //end test

            var request = ConnectionManager.GetForwardOpenRequest();
            var response = await SendRRData(request);
            if (response?.GeneralStatus == (byte)CipGeneralStatusCode.Success)
            {
                ConnectionStatus = ConnectionStatus.ForwardOpen;

                // TODO(gjc): add more
                O2TConnectionId = BitConverter.ToUInt32(response.ResponseData, 0);
                Debug.WriteLine("O2TConnectionId is " + O2TConnectionId);

                // TODO(gjc): for match???
                T2OConnectionId = BitConverter.ToUInt32(response.ResponseData, 4);

                SequenceCount = 1;

                return 0;
            }

            return -1;
        }

        protected void OnDisconnect(object sender, EventArgs e)
        {
            if (sender == _cipSyncConnection)
            {
                ConnectionStatus = ConnectionStatus.Disconnected;

                //Debug.WriteLine("disconnected!");

                _logger.Error($"{IpAddress} Disconnected!");

                Disconnected?.Invoke(this, EventArgs.Empty);
            }
        }

        protected void UnRegisterSession()
        {
            if ((ConnectionStatus == ConnectionStatus.Disconnected)
                || (ConnectionStatus == ConnectionStatus.Netconnected))
                return;

            _cipSyncConnection?.UnRegisterSession(SessionHandle);

            ConnectionStatus = ConnectionStatus.Netconnected;
        }

        #endregion
    }
}