using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DeviceModule;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;

namespace ICSStudio.DeviceProperties.Adapters.Panel
{
    public class PortConfigurationViewModel : DeviceOptionPanel
    {
        private readonly DeviceModule _deviceModule;
        private readonly Controller _controller;
        private bool _isEnable1;
        private bool _isEnable2;
        private string _status1;
        private string _status2;
        private bool _autoNegotiate1;
        private bool _autoNegotiate2;
        private string _speed1;
        private string _speed2;
        private string _duplex1;
        private string _duplex2;

        public PortConfigurationViewModel(UserControl panel, DeviceModule deviceModule) : base(panel)
        {
            _deviceModule = deviceModule;
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
                        ICipMessager messager = await _controller.GetMessager(_deviceModule);

                        if (messager != null)
                        {
                            var portConfiguration = new PortConfiguration(messager);
                            await portConfiguration.GetPortInfo();

                            Update(portConfiguration);

                        }
                    }

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            });
        }

        private void Update(PortConfiguration portConfiguration)
        {
            var ports = portConfiguration.Ports.Where(port =>
                port.InterfaceState == CIPEthernetLinkObject.State.Disable ||
                port.InterfaceState == CIPEthernetLinkObject.State.Enable).ToList();

            var count = ports.Count;
            Debug.Assert(count <= 2);
            if (count == 2)
            {
                var port = ports[1];
                IsEnable2 = port.InterfaceState == CIPEthernetLinkObject.State.Enable;
                Status2 = port.Status.ToString();
                AutoNegotiate2 =
                    (port.Negotiation == CIPEthernetLinkObject.NegotiationStatus.AutoNegotiationInProgress ||
                     port.Negotiation == CIPEthernetLinkObject.NegotiationStatus
                         .NegotiationSpeedAndDuplexSuccessfully);
                Speed2 = port.Speed.ToString((IFormatProvider)null);
                Duplex2 = port.DuplexStatus.ToString();
                count--;
            }

            if (count == 1)
            {
                var port = ports[0];
                IsEnable1 = port.InterfaceState == CIPEthernetLinkObject.State.Enable;
                Status1 = port.Status.ToString();
                AutoNegotiate1 =
                    (port.Negotiation == CIPEthernetLinkObject.NegotiationStatus.AutoNegotiationInProgress ||
                     port.Negotiation == CIPEthernetLinkObject.NegotiationStatus
                         .NegotiationSpeedAndDuplexSuccessfully);
                Speed1 = port.Speed.ToString((IFormatProvider)null);
                Duplex1 = port.DuplexStatus.ToString();

            }
        }

        public void Clear()
        {
            IsEnable1 = false;
            IsEnable2 = false;
            Status1 = "";
            Status2 = "";
            AutoNegotiate1 = false;
            AutoNegotiate2 = false;
            Speed1 = "";
            Speed2 = "";
            Duplex1 = "";
            Duplex2 = "";
        }

        public bool IsEnable1
        {
            set { Set(ref _isEnable1, value); }
            get { return _isEnable1; }
        }

        public bool IsEnable2
        {
            set { Set(ref _isEnable2, value); }
            get { return _isEnable2; }
        }

        public string Status1
        {
            set { Set(ref _status1, value); }
            get { return _status1; }
        }

        public string Status2
        {
            set { Set(ref _status2, value); }
            get { return _status2; }
        }

        public bool AutoNegotiate1
        {
            set { Set(ref _autoNegotiate1, value); }
            get { return _autoNegotiate1; }
        }

        public bool AutoNegotiate2
        {
            set { Set(ref _autoNegotiate2, value); }
            get { return _autoNegotiate2; }
        }

        public string Speed1
        {
            set { Set(ref _speed1, value); }
            get { return _speed1; }
        }

        public string Speed2
        {
            set { Set(ref _speed2, value); }
            get { return _speed2; }
        }

        public string Duplex1
        {
            set { Set(ref _duplex1, value); }
            get { return _duplex1; }
        }

        public string Duplex2
        {
            set { Set(ref _duplex2, value); }
            get { return _duplex2; }
        }
    }
}
