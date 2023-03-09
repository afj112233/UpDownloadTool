using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Threading;
using ThreadHelper = Microsoft.VisualStudio.Shell.ThreadHelper;

namespace ICSStudio.DeviceProperties.ServoDrives.Panel
{
    public class TimeSyncViewModel : DeviceOptionPanel
    {
        private string _cipSyncTime;
        private string _utcSystemTime;
        private string _grandmasterDescription;
        private string _userName;
        private string _grandmasterIdentity;
        private string _grandmasterClass;
        private string _grandmasterAccuracy;
        private string _grandmasterVariance;
        private string _grandmasterSource;
        private string _grandmasterPriority1;
        private string _grandmasterPriority2;
        private string _location;
        private string _protocolAddress;
        private string _physicalAddress;
        private string _clockType;
        private string _manufacturerName;
        private string _model;
        private string _serialNumber;
        private string _hardwareRevision;
        private string _firmwareRevision;
        private string _softwareRevision;
        private string _profileIdentity;
        private string _physicalProtocol;
        private string _networkProtocol;
        private string _portNumber;
        private string _synchronizationStatus;
        private string _offsetToMaster;
        private string _localIdentity;
        private string _localClass;
        private string _localAccuracy;
        private string _localVariance;
        private string _localSource;
        private readonly ModifiedMotionDrive _modifiedMotionDrive;
        private readonly Controller _controller;

        public TimeSyncViewModel(UserControl panel, ModifiedMotionDrive modifiedMotionDrive) : base(panel)
        {
            _modifiedMotionDrive = modifiedMotionDrive;
            _controller = Controller.GetInstance();
        }

        public override void Show()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                try
                {
                    await TaskScheduler.Default;

                    if (_controller != null)
                    {
                        ICipMessager messager = await _controller.GetMessager(_modifiedMotionDrive.OriginalMotionDrive);

                        if (messager != null)
                        {
                            var cipTimeSync =
                                new CIPTimeSync((ushort) 1, messager);

                            await messager.SendRRData(cipTimeSync.GetAttributesAllRequest());

                            if (cipTimeSync.GetAllTimeSyncInfo().Result != -1)
                            {
                                Update(cipTimeSync);
                            }

                        }
                    }

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            });
        }
        
        private void Update(CIPTimeSync cipTimeSync)
        {
            var timeSync = cipTimeSync;
            CIPSyncTime = ConvertValue.ConvertEnablePTP(timeSync.PTPEnable.ToUInt16(null));

            DateTime pointOfReference = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            UTCSystemTime = pointOfReference.AddTicks(timeSync.SystemTimeMicroseconds.ToInt64(null) * 10)
                .ToString("yyyy/MM/dd HH:mm:ss");

            var gmClock = timeSync.GrandMasterClockInfo;
            GrandmasterIdentity = ConvertArrayToString(gmClock.ClockIdentity.GetBytes(), "", true);
            GrandmasterClass = gmClock.ClockClass.ToString(null);
            GrandmasterAccuracy = gmClock.TimeAccuracy.ToString(null);
            GrandmasterVariance = gmClock.OffsetScaledLogVariance.ToString(null);
            GrandmasterSource = ConvertValue.ConvertTimeSourceValue(gmClock.TimeSource.ToUInt16(null));
            GrandmasterPriority1 = gmClock.Priority1.ToString(null);
            GrandmasterPriority2 = gmClock.Priority2.ToString(null);

            var lClock = timeSync.LocalClockInfo;
            SynchronizationStatus = ConvertValue.ConvertIsSynchronized(timeSync.IsSynchronized.ToUInt16(null));
            OffsetToMaster = timeSync.OffsetFromMaster.ToString(null);

            LocalIdentity = ConvertArrayToString(lClock.ClockIdentity.GetBytes(), "", true);
            LocalClass = lClock.ClockClass.ToString(null);
            LocalAccuracy = lClock.TimeAccuracy.ToString(null);
            LocalVariance = lClock.OffsetScaledLogVariance.ToString(null);
            LocalSource = ConvertValue.ConvertTimeSourceValue(lClock.TimeSource.ToUInt16(null));

            UserName = timeSync.ClockDescription.UserDataDescriptionName.GetString();
            Location = timeSync.ClockDescription.UserDataDescriptionLoc.GetString();

            ProtocolAddress = ConvertArrayToString(timeSync.ClockDescription.PortDataProtocolAddress.GetBytes(), ".");

            PhysicalAddress =
                ConvertArrayToString(timeSync.ClockDescription.PortDataPhysicalAddress.GetBytes(), "-", true);

            ClockType = ConvertArrayToString(timeSync.ClockDescription.ClockDataClockType.GetBytes(), ",");

            ManufacturerName = timeSync.ClockDescription.ClockDataProdDescManuf.GetString();

            Model = timeSync.ClockDescription.ClockDataProdDescModel.GetString();

            SerialNumber = timeSync.ClockDescription.ClockDataProdDescSerialNum.GetString();

            HardwareRevision = timeSync.ClockDescription.ClockDataHWRevision.GetString();

            FirmwareRevision = timeSync.ClockDescription.ClockDataFWRevision.GetString();

            SoftwareRevision = timeSync.ClockDescription.ClockDataSWRevision.GetString();

            ProfileIdentity = ConvertArrayToString(timeSync.ClockDescription.ClockDataProfileID.GetBytes(), "-", true);

            PhysicalProtocol = timeSync.ClockDescription.PortDataPhysicalLayerProto.GetString();

            NetworkProtocol =
                ConvertValue.ConvertNetworkProtocolValue(
                    timeSync.ClockDescription.PortDataNetworkProtocol.ToUInt16(null));

            PortNumber = timeSync.PortStateInfo.NumberOfPorts.ToInt32(null).ToString();

            var portInfo = "";
            for (int i = 0; i < timeSync.PortStateInfo.NumberOfPorts.ToUInt16(null); i++)
            {
                portInfo += (i == 0 ? "" : "\n") +
                            $"{timeSync.PortStateInfo.PortInfos[i].Item1.ToString(null)}  (Port {i + 1})";
            }

            Port1 = portInfo;
        }

        private string ConvertArrayToString(byte[] array, string signal, bool isHex = false)
        {
            if (array == null || array.Length == 0) return "";
            var str = "";
            for (int i = 0; i < array.Length; i++)
            {
                str += signal + (isHex ? array[i].ToString("X") : array[i].ToString());
            }

            return str.Substring(signal.Length);
        }

        public string Port1 { set; get; }

        public string CIPSyncTime
        {
            get { return _cipSyncTime; }
            set { Set(ref _cipSyncTime, value); }
        }

        public string UTCSystemTime
        {
            get { return _utcSystemTime; }
            set { Set(ref _utcSystemTime, value); }
        }

        public string GrandmasterDescription
        {
            get { return _grandmasterDescription; }
            set { Set(ref _grandmasterDescription, value); }
        }

        public string UserName
        {
            get { return _userName; }
            set { Set(ref _userName, value); }
        }

        public string GrandmasterIdentity
        {
            get { return _grandmasterIdentity; }
            set { Set(ref _grandmasterIdentity, value); }
        }

        public string GrandmasterClass
        {
            get { return _grandmasterClass; }
            set { Set(ref _grandmasterClass, value); }
        }

        public string GrandmasterAccuracy
        {
            get { return _grandmasterAccuracy; }
            set { Set(ref _grandmasterAccuracy, value); }
        }

        public string GrandmasterVariance
        {
            get { return _grandmasterVariance; }
            set { Set(ref _grandmasterVariance, value); }
        }

        public string GrandmasterSource
        {
            get { return _grandmasterSource; }
            set { Set(ref _grandmasterSource, value); }
        }

        public string GrandmasterPriority1
        {
            get { return _grandmasterPriority1; }
            set { Set(ref _grandmasterPriority1, value); }
        }

        public string GrandmasterPriority2
        {
            get { return _grandmasterPriority2; }
            set { Set(ref _grandmasterPriority2, value); }
        }

        #region description

        public string Location
        {
            set { Set(ref _location, value); }
            get { return _location; }
        }

        public string ProtocolAddress
        {
            set { Set(ref _protocolAddress, value); }
            get { return _protocolAddress; }
        }

        public string PhysicalAddress
        {
            set { Set(ref _physicalAddress, value); }
            get { return _physicalAddress; }
        }

        public string ClockType
        {
            set { Set(ref _clockType, value); }
            get { return _clockType; }
        }

        public string ManufacturerName
        {
            set { Set(ref _manufacturerName, value); }
            get { return _manufacturerName; }
        }

        public string Model
        {
            set { Set(ref _model, value); }
            get { return _model; }
        }

        public string SerialNumber
        {
            set { Set(ref _serialNumber, value); }
            get { return _serialNumber; }
        }

        public string HardwareRevision
        {
            set { Set(ref _hardwareRevision, value); }
            get { return _hardwareRevision; }
        }

        public string FirmwareRevision
        {
            set { Set(ref _firmwareRevision, value); }
            get { return _firmwareRevision; }
        }

        public string SoftwareRevision
        {
            set { Set(ref _softwareRevision, value); }
            get { return _softwareRevision; }
        }

        public string ProfileIdentity
        {
            set { Set(ref _profileIdentity, value); }
            get { return _profileIdentity; }
        }

        public string PhysicalProtocol
        {
            set { Set(ref _physicalProtocol, value); }
            get { return _physicalProtocol; }
        }

        public string NetworkProtocol
        {
            set { Set(ref _networkProtocol, value); }
            get { return _networkProtocol; }
        }

        public string PortNumber
        {
            set { Set(ref _portNumber, value); }
            get { return _portNumber; }
        }

        #endregion

        public string SynchronizationStatus
        {
            set { Set(ref _synchronizationStatus, value); }
            get { return _synchronizationStatus; }
        }

        public string OffsetToMaster
        {
            set { Set(ref _offsetToMaster, value); }
            get { return _offsetToMaster; }
        }

        public string LocalIdentity
        {
            set { Set(ref _localIdentity, value); }
            get { return _localIdentity; }
        }

        public string LocalClass
        {
            set { Set(ref _localClass, value); }
            get { return _localClass; }
        }

        public string LocalAccuracy
        {
            set { Set(ref _localAccuracy, value); }
            get { return _localAccuracy; }
        }

        public string LocalVariance
        {
            set { Set(ref _localVariance, value); }
            get { return _localVariance; }
        }

        public string LocalSource
        {
            set { Set(ref _localSource, value); }
            get { return _localSource; }
        }
    }
}
