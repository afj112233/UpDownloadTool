using System;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using ICSStudio.Cip.Objects;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DeviceModule;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.Dialogs.PortDiagnostics
{
    public class PortDiagnosticsVM : ViewModelBase
    {
        private DeviceModule _deviceModule;
        private Controller _controller;
        private DispatcherTimer _timer;
        private int _num;

        public PortDiagnosticsVM(DeviceModule deviceModule, int num)
        {
            _num = num;
            Title = $"Port Diagnostics";
            _deviceModule = deviceModule;
            _controller = Controller.GetInstance();
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                _controller, "IsOnlineChanged", OnIsOnlineChanged);
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 1);
            _timer.Tick += _timer_Tick;
            if (_controller.IsOnline)
            {
                InitialCip();
                _timer.Start();
            }
        }

        public string Title { get; }
        private CIPEthernetLinkObject _cipEthernet;

        private void InitialCip()
        {
            var conn = _controller.GetMessager(_deviceModule);
            _cipEthernet = new CIPEthernetLinkObject(_num, conn.Result);
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            SetAttr();
        }

        public override void Cleanup()
        {
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                _controller, "IsOnlineChanged", OnIsOnlineChanged);

            _timer.Stop();
            _timer.Tick -= _timer_Tick;
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                if (_controller.IsOnline)
                {
                    InitialCip();
                    _timer.Start();
                }
                else
                {
                    _timer.Stop();
                    ClearAttr();
                }
            });
        }

        private void SetAttr()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await _cipEthernet.GetInterfaceCounter();
                await _cipEthernet.GetMediaCounter();
                Inbound = _cipEthernet.InterfaceCounter.InOctets.ToUInt32(null);
                Outbound = _cipEthernet.InterfaceCounter.OutOctets.ToUInt32(null);
                UnicastPacketsInbound = _cipEthernet.InterfaceCounter.InUcastPackets.ToUInt32(null);
                UnicastPacketsOutbound = _cipEthernet.InterfaceCounter.OutUcastPackets.ToUInt32(null);
                NonunicastPacketsInbound = _cipEthernet.InterfaceCounter.InNUcastPackets.ToUInt32(null);
                NonunicastPacketsOutbound = _cipEthernet.InterfaceCounter.OutNUcastPackets.ToUInt32(null);
                PacketsDiscardedInbound = _cipEthernet.InterfaceCounter.InDiscards.ToUInt32(null);
                PacketsDiscardedOutbound = _cipEthernet.InterfaceCounter.OutDiscards.ToUInt32(null);
                PacketsWithErrorsInbound = _cipEthernet.InterfaceCounter.InErrors.ToUInt32(null);
                PacketsWithErrorsOutbound = _cipEthernet.InterfaceCounter.OutErrors.ToUInt32(null);
                UnknownProtocolPacketsInbound = _cipEthernet.InterfaceCounter.InUnknownProtos.ToUInt32(null);

                AlignmentErrors = _cipEthernet.MediaCounter.AlignmentErrors.ToUInt32(null);
                FCSErrors = _cipEthernet.MediaCounter.FCSErrors.ToUInt32(null);
                SingleCollisions = _cipEthernet.MediaCounter.SingleCollisions.ToUInt32(null);
                MultipleCollisions = _cipEthernet.MediaCounter.MultipleCollisions.ToUInt32(null);
                SQETestErrors = _cipEthernet.MediaCounter.SQETestErrors.ToUInt32(null);
                DeferredTransmissions = _cipEthernet.MediaCounter.DeferredTransmissions.ToUInt32(null);
                LateCollisions = _cipEthernet.MediaCounter.LateCollisions.ToUInt32(null);
                ExcessiveCollisions = _cipEthernet.MediaCounter.ExcessiveCollision.ToUInt32(null);
                MACTransmitErrors = _cipEthernet.MediaCounter.MACTransmitErrors.ToUInt32(null);
                MACReceiveErrors = _cipEthernet.MediaCounter.MACReceiveErrors.ToUInt32(null);
                CarrierSense = _cipEthernet.MediaCounter.CarrierSenseErrors.ToUInt32(null);
                FrameTooLong = _cipEthernet.MediaCounter.FrameTooLong.ToUInt32(null);
                Raise();
            });
        }

        private void ClearAttr()
        {
            Inbound = 0;
            Outbound = 0;
            UnicastPacketsInbound = 0;
            UnicastPacketsOutbound = 0;
            NonunicastPacketsInbound = 0;
            NonunicastPacketsOutbound = 0;
            PacketsDiscardedInbound = 0;
            PacketsDiscardedOutbound = 0;
            PacketsWithErrorsInbound = 0;
            PacketsWithErrorsOutbound = 0;
            UnknownProtocolPacketsInbound = 0;

            AlignmentErrors = 0;
            FCSErrors = 0;
            SingleCollisions = 0;
            MultipleCollisions = 0;
            SQETestErrors = 0;
            DeferredTransmissions = 0;
            LateCollisions = 0;
            ExcessiveCollisions = 0;
            MACTransmitErrors = 0;
            MACReceiveErrors = 0;
            CarrierSense = 0;
            FrameTooLong = 0;
            Raise();
        }

        private void Raise()
        {
            RaisePropertyChanged("Inbound");
            RaisePropertyChanged("Outbound");
            RaisePropertyChanged("UnicastPacketsInbound");
            RaisePropertyChanged("UnicastPacketsOutbound");
            RaisePropertyChanged("NonunicastPacketsInbound");
            RaisePropertyChanged("NonunicastPacketsOutbound");
            RaisePropertyChanged("PacketsDiscardedInbound");
            RaisePropertyChanged("PacketsDiscardedOutbound");
            RaisePropertyChanged("PacketsWithErrorsInbound");
            RaisePropertyChanged("PacketsWithErrorsOutbound");
            RaisePropertyChanged("UnknownProtocolPacketsInbound");

            RaisePropertyChanged("AlignmentErrors");
            RaisePropertyChanged("FCSErrors");
            RaisePropertyChanged("SingleCollisions");
            RaisePropertyChanged("MultipleCollisions");
            RaisePropertyChanged("SQETestErrors");
            RaisePropertyChanged("DeferredTransmissions");
            RaisePropertyChanged("LateCollisions");
            RaisePropertyChanged("ExcessiveCollisions");
            RaisePropertyChanged("MACTransmitErrors");
            RaisePropertyChanged("MACReceiveErrors");
            RaisePropertyChanged("CarrierSense");
            RaisePropertyChanged("FrameTooLong");
        }

        //interface
        public uint Inbound { set; get; }
        public uint Outbound { set; get; }
        public uint UnicastPacketsInbound { set; get; }
        public uint UnicastPacketsOutbound { set; get; }
        public uint NonunicastPacketsInbound { set; get; }
        public uint NonunicastPacketsOutbound { set; get; }
        public uint PacketsDiscardedInbound { set; get; }
        public uint PacketsDiscardedOutbound { set; get; }
        public uint PacketsWithErrorsInbound { set; get; }
        public uint PacketsWithErrorsOutbound { set; get; }
        public uint UnknownProtocolPacketsInbound { set; get; }

        //media
        public uint AlignmentErrors { set; get; }
        public uint FCSErrors { set; get; }
        public uint SingleCollisions { set; get; }
        public uint MultipleCollisions { set; get; }
        public uint SQETestErrors { set; get; }
        public uint DeferredTransmissions { set; get; }
        public uint LateCollisions { set; get; }
        public uint ExcessiveCollisions { set; get; }
        public uint MACTransmitErrors { set; get; }
        public uint MACReceiveErrors { set; get; }
        public uint CarrierSense { set; get; }
        public uint FrameTooLong { set; get; }
    }
}
