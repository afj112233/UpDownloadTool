using System;
using System.Diagnostics;
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
    public class InternetProtocolViewModel : DeviceOptionPanel
    {
        private readonly DeviceModule _deviceModule;
        private readonly Controller _controller;
        private bool _isCheck1;
        private bool _isCheck2;
        private bool _isCheck3;
        private bool _isCheck4;
        private string _ip;
        private string _mask;
        private string _gateway;
        private string _domain;
        private string _host;
        private string _server1;
        private string _server2;
        public InternetProtocolViewModel(UserControl panel, DeviceModule deviceModule) : base(panel)
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
                            var cipTcp = new CIPTcp(messager);

                            await messager.SendRRData(cipTcp.GetAttributesAllRequest());

                            if (cipTcp.GetAttributesAll().Result != -1)
                            {
                                Update(cipTcp);
                            }
                            else
                            {
                                IP = "";
                                Mask = "";
                                Gateway = "";
                                Server1 = "";
                                Server2 = "";
                                Domain = "";
                                Host = "";
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

        private void Update(CIPTcp cipTcp)
        {
            IP = CIPTcp.IntToIp(cipTcp.Configuration.IP.ToUInt32(null));
            Mask = CIPTcp.IntToIp(cipTcp.Configuration.Mask.ToUInt32(null));
            Gateway = CIPTcp.IntToIp(cipTcp.Configuration.Gateway.ToUInt32(null));
            Server1 = CIPTcp.IntToIp(cipTcp.Configuration.ServerName.ToUInt32(null));
            Server2 = CIPTcp.IntToIp(cipTcp.Configuration.ServerName2.ToUInt32(null));
            Domain = cipTcp.Configuration.DomainName?.GetString();
            Host = cipTcp.HostName?.GetString();
            switch (cipTcp.ConfigurationMethod)
            {
                case CIPTcp.ConfigurationMode.Statically:
                    IsCheck1 = true;
                    break;
                case CIPTcp.ConfigurationMode.BOOTP:
                    IsCheck2 = true;
                    break;
                case CIPTcp.ConfigurationMode.DHCP:
                    IsCheck3 = true;
                    break;
                case CIPTcp.ConfigurationMode.Reserced:
                    IsCheck4 = true;
                    break;
            }
        }

        public bool IsCheck1
        {
            set { Set(ref _isCheck1, value); }
            get { return _isCheck1; }
        }

        public bool IsCheck2
        {
            set { Set(ref _isCheck2, value); }
            get { return _isCheck2; }
        }

        public bool IsCheck3
        {
            set { Set(ref _isCheck3, value); }
            get { return _isCheck3; }
        }

        public bool IsCheck4
        {
            set { Set(ref _isCheck4, value); }
            get { return _isCheck4; }
        }

        public string IP
        {
            set { Set(ref _ip, value); }
            get { return _ip; }
        }

        public string Mask
        {
            set { Set(ref _mask, value); }
            get { return _mask; }
        }

        public string Gateway
        {
            set { Set(ref _gateway, value); }
            get { return _gateway; }
        }

        public string Domain
        {
            set { Set(ref _domain, value); }
            get { return _domain; }
        }

        public string Host
        {
            set { Set(ref _host, value); }
            get { return _host; }
        }

        public string Server1
        {
            set { Set(ref _server1, value); }
            get { return _server1; }
        }

        public string Server2
        {
            set { Set(ref _server2, value); }
            get { return _server2; }
        }
    }
}
