using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;
using ICSStudio.CipConnection;
using ICSStudio.SimpleServices.DeviceModule;
using Microsoft.VisualStudio.Threading;
using Task = System.Threading.Tasks.Task;

namespace ICSStudio.UIServicesPackage.PLCProperties.Panel
{
    class InternetProtocolViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private bool _isDirty;
        private Controller _controller;
        private bool _enable;
        private string _ipAddress;
        private string _subnet;
        private string _gateway;
        private string _primary;
        private string _secondary;
        private string _host;
        private string _domain;
        private bool _isCheck1;

        public bool IsOnline => _controller.IsOnline;

        public InternetProtocolViewModel(InternetProtocol panel, IController controller)
        {
            Control = panel;
            panel.DataContext = this;
            _controller = (Controller)controller;
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                controller as Controller, "IsOnlineChanged", OnIsOnlineChanged);
            WeakEventManager<Controller, EventArgs>.AddHandler(
                _controller, "KeySwitchChanged", OnKeySwitchChanged);

            Enable = false;
            var localModule = controller?.DeviceModules["Local"] as LocalModule;
            Debug.Assert(localModule != null);
            if (localModule.ProductCode == 408)
            {
                DualIpVisibility = Visibility.Visible;
            }
            else if (localModule.ProductCode == 108)
            {
                DualIpVisibility = Visibility.Collapsed;
            }
            else
            {
                if (string.IsNullOrEmpty(controller.EtherNetIPMode))
                {
                    DualIpVisibility = Visibility.Collapsed;
                }
                else
                {
                    DualIpVisibility = Visibility.Visible;
                    if (controller.EtherNetIPMode == "A1/A2: Dual-IP")
                    {
                        PortNum.Add(new PortInfo(0) { Label = "A1" });
                        PortNum.Add(new PortInfo(0) { Label = "A2" });
                    }
                    else
                    {
                        PortNum.Add(new PortInfo(0) { Label = "A1/A2" });
                    }

                    if (!controller.IsOnline)
                        SelectedPort = PortNum[0];
                }
            }
            
            if (controller.IsOnline)
            {
                //SetAttr();
                ThreadHelper.JoinableTaskFactory.Run(async delegate
                {
                    await StartGetInfoAsync();
                });
            }

            IsDirty = false;
        }
        
        public PortInfo SelectedPort
        {
            set
            {
                Set(ref _selectedPort, value);
                Update();
            }
            get { return _selectedPort; }
        }

        public CIPTcp CipTcp { set; get; }
        public async Task<int> GetIpInterfaceAsync(int instanceId)
        {
            int result = 0;
            if (_controller.CipMessager.ConnectionStatus == ConnectionStatus.Disconnected)
                result = await _controller.CipMessager.OnLine(true);
            if (result == -1) return result;
            CipTcp = new CIPTcp(_controller.CipMessager, instanceId);
            result = await CipTcp.GetAttributesAll();
            return result;
        }
        
        public override void Cleanup()
        {
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                _controller as Controller, "IsOnlineChanged", OnIsOnlineChanged);
        }

        public string IPAddress
        {
            set
            {
                Set(ref _ipAddress, value);
                Compare();
            }
            get { return _ipAddress; }
        }

        public Visibility DualIpVisibility { set; get; }
        public string Subnet
        {
            set
            {
                Set(ref _subnet, value);
                Compare();
            }
            get { return _subnet; }
        }

        public string Gateway
        {
            set
            {
                Set(ref _gateway, value);
                Compare();
            }
            get { return (_gateway); }
        }

        public string Domain
        {
            set
            {
                Set(ref _domain, value);
                Compare();
            }
            get { return _domain; }
        }

        public string Primary
        {
            set
            {
                Set(ref _primary, value);
                Compare();
            }
            get { return (_primary); }
        }

        public string Host
        {
            set
            {
                Set(ref _host, value);
                Compare();
            }
            get { return _host; }
        }

        public string Secondary
        {
            set
            {
                Set(ref _secondary, value);
                Compare();
            }
            get { return (_secondary); }
        }

        public bool Enable
        {
            set
            {
                Set(ref _enable, value);
                Compare();
            }
            get
            {
                return _enable;
            }
        }

        public bool IsCheck1
        {
            set { Set(ref _isCheck1, value); }
            get { return _isCheck1; }
        }

        public bool IsCheck2 { set; get; }

        public object Owner { get; set; }
        public object Control { get; }

        public void LoadOptions()
        {

        }

        public bool SaveOptions()
        {
            return true;
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                _isDirty = value;
                IsDirtyChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler IsDirtyChanged;

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                if (_controller.IsOnline)
                {
                    await StartGetInfoAsync();
                }
                else
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    Clear();
                }
            });
        }

        private void OnKeySwitchChanged(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                if (_controller.IsOnline)
                {
                    await StartGetInfoAsync();
                    if (_controller.KeySwitchPosition == ControllerKeySwitch.RunKeySwitch)
                        Enable = false;
                }
                else
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    Clear();
                }
                RaisePropertyChanged(nameof(IsOnline));
            });
        }

        public void Clear()
        {
            IPAddress = "";
            Subnet = "";
            Gateway = "";
            Primary = "";
            Secondary = "";
            Domain = "";
            Host = "";
            Enable = false;
            IsDirty = false;
        }

        public void DoApply()
        {
            if (!VerifyAll()) return;

            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                var cipTcp = new CIPTcp(((Controller) _controller).CipMessager);
                try
                {
                    var ip = IpToInt(IPAddress);
                    var mask = IpToInt(Subnet);
                    UpdatePortNum();
                    var gateway = IpToInt(Gateway);
                    var server = IpToInt(Primary);
                    var server2 = IpToInt(Secondary);
                    var result = await cipTcp.SetConfiguration(ip, mask, gateway, server, server2, Domain, SelectedPort.Instance);
                    CopyToc();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            });
        }
        private void UpdatePortNum()
        {
            foreach (var item in PortNum)
                if (item.Instance == SelectedPort.Instance)
                {
                    item.IP = IPAddress;
                    item.Mask = Subnet;
                    item.Gateway = Gateway;
                }
        }

        private bool VerifyAll()
        {
            if (!(VerifyIpAddress(IPAddress) && VerifyIpAddress(Subnet) && VerifyIpAddress(Gateway))) return false;

            if (!VerifySubnetMask(Subnet))
                foreach (var item in PortNum)
                    if (item.Instance == SelectedPort.Instance)
                        Subnet = item.Mask;

            if (!VerifyRangesOverlap(IPAddress,Subnet)) return false;

            if (!VerifyGateway(IPAddress, Subnet, Gateway)) return false;

            return true;
        }

        private bool VerifyRangesOverlap(string ipAddress,string subnet)
        {
            foreach (var item in PortNum)
            {
                if (DualIpVisibility != Visibility.Visible&&item.Instance>1)
                {
                    return true;
                }
                if (item.Instance != SelectedPort.Instance)
                {
                    if (item.IP == IPAddress)
                    {
                        MessageForIpRangesOverlap();
                        return false;
                    }

                    var currentGateway = CalculateGateway(ipAddress, subnet);
                    var anotherGateway = CalculateGateway(item.IP, subnet);
                    for (var i = 0; i < anotherGateway.Length; i++)
                    {
                        if (currentGateway[i] != anotherGateway[i]) break;
                        if (i == anotherGateway.Length - 1)
                        {
                            MessageForIpRangesOverlap();
                            return false;
                        }
                    }

                    currentGateway = CalculateGateway(ipAddress, item.Mask);
                    anotherGateway = CalculateGateway(item.IP, item.Mask);
                    for (var i = 0; i < anotherGateway.Length; i++)
                    {
                        if (currentGateway[i] != anotherGateway[i]) break;
                        if (i == anotherGateway.Length - 1)
                        {
                            MessageForIpRangesOverlap();
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void MessageForIpRangesOverlap()
        {
            const string message = "Failed to modify the Internet Protocol configuration.\nIP address ranges overlap.";
            const string caption = "ICS Studio";
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        public bool VerifyIpAddress(string ip)
        {
            const string pattern = @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$";
            return Regex.IsMatch(ip, pattern);
        }

        public bool VerifyGateway(string ipAddress, string subnet, string gateway)
        {
            var realGateway = CalculateGateway(ipAddress, subnet);
            var defaultGateway = CalculateGateway(gateway, subnet);
            for (var i = 0; i < realGateway.Length; i++)
                if (realGateway[i] != defaultGateway[i])
                {
                    var message =
                        "Failed to modify the Internet Protocol configuration.\nGateway Address must be on same subnet as IP Address.";
                    var caption = "ICS Studio";
                    MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    return false;
                }

            return true;
        }

        public bool VerifySubnetMask(string subnet)
        {
            var list = subnet.Split('.');
            if (list.Length != 4) return false;
            var zero = false;
            for (var i = 0; i < list.Length; i++)
            {
                int k;
                if (!int.TryParse(list[i], out k)) return false;
                if (k < 0 || k > 255) return false;
                if (zero)
                {
                    if (k != 0) return false;
                }
                else
                {
                    for (var j = 7; j >= 0; j--)
                        if (((k >> j) & 1) == 0)
                        {
                            zero = true;
                        }
                        else
                        {
                            if (zero) return false;
                        }
                }
            }

            return true;
        }

        public int[] CalculateGateway(string ip, string subnet)
        {
            var ipStrings = ip.Split('.');
            var gatewayStrings = subnet.Split('.');
            var target = new int[4];
            for (var i = 0; i < 4; i++) target[i] = int.Parse(ipStrings[i]) & int.Parse(gatewayStrings[i]);
            return target;
        }

        public static int IpToInt(string ip)
        {
            char[] separator = new char[] {'.'};
            string[] items = ip.Split(separator);
            return int.Parse(items[0]) << 24
                   | int.Parse(items[1]) << 16
                   | int.Parse(items[2]) << 8
                   | int.Parse(items[3]);
        }
        
        public CIPPort CIPPort { get; private set; }

        private void Update()
        {
            if (SelectedPort == null||!_controller.IsOnline) return;
            ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                Again:
                await TaskScheduler.Default;
                var result = await GetIpInterfaceAsync(SelectedPort.Instance);
                if (result != -1)
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    IPAddress = System.Net.IPAddress.Parse(CipTcp.Configuration.IP.ToUInt32(null).ToString()).ToString();
                    Subnet = System.Net.IPAddress.Parse(CipTcp.Configuration.Mask.ToUInt32(null).ToString()).ToString();
                    Gateway = System.Net.IPAddress.Parse(CipTcp.Configuration.Gateway.ToUInt32(null).ToString())
                        .ToString();
                    UpdatePortNum();
                    Primary = System.Net.IPAddress.Parse(CipTcp.Configuration.ServerName.ToUInt32(null).ToString())
                        .ToString();
                    Secondary = System.Net.IPAddress.Parse(CipTcp.Configuration.ServerName2.ToUInt32(null).ToString())
                        .ToString();
                    Domain = CipTcp.Configuration.DomainName?.GetString();
                    Host = CipTcp.HostName?.GetString();
                    IsCheck1 = CipTcp.ConfigurationMethod == CIPTcp.ConfigurationMode.Statically;
                    Enable = true;
                    if (_controller.IsOnline&&_controller.KeySwitchPosition== ControllerKeySwitch.RunKeySwitch)
                    {
                        Enable = false;
                    }
                    CopyToc();
                    IsDirty = false;
                }
                else
                {
                    await TaskScheduler.Default;
                    await Task.Delay(500);
                    goto Again;
                }
            });
        }
        
        public async Task<int> GetCipPortAsync()
        {
            int result = 0;
            if (_controller.CipMessager.ConnectionStatus == ConnectionStatus.Disconnected)
                result = await _controller.CipMessager.OnLine(true);
            if (result == -1) return result;
            if (CIPPort == null || CIPPort.Messager != _controller.CipMessager)
                CIPPort = new CIPPort(_controller.CipMessager);
            result = await CIPPort.GetPortInstanceInfo();
            return result;
        }
        public ObservableCollection<PortInfo> PortNum { set; get; } = new ObservableCollection<PortInfo>();
        private async Task<int> SetPortLabelAsync()
        {
            var result = await GetCipPortAsync();
            int instance = 1;
            int index = 1;
            if (result != 1) return -1;
            try
            {
                foreach (var info in CIPPort.PortInfo.PortInstanceInfos)
                {
                    if (info.PortType.ToUInt16(null) == (ushort)CIPPortType.Any) continue;
                    else if (info.PortType.ToUInt16(null) == (ushort)CIPPortType.EtherNet)
                    {
                        var portInfo = new PortInfo(instance);
                        var ethernetLabel = new EthernetLinkObjectClassAttr(_controller.CipMessager, index);
                        await ethernetLabel.GetInterfaceLabel();
                        portInfo.Label = ethernetLabel.Label.GetString();
                        var outcome = await GetIpInterfaceAsync(instance);
                        if (outcome != -1)
                        {
                            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                            portInfo.IP = System.Net.IPAddress.Parse(CipTcp.Configuration.IP.ToUInt32(null).ToString()).ToString();
                            portInfo.Mask = System.Net.IPAddress.Parse(CipTcp.Configuration.Mask.ToUInt32(null).ToString()).ToString();
                            portInfo.Gateway = System.Net.IPAddress.Parse(CipTcp.Configuration.Gateway.ToUInt32(null).ToString()).ToString();
                        }
                        index++;
                        instance++;
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        PortNum.Add(portInfo);
                    }
                    else
                    {
                        instance++;
                        continue;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }

            return result;
        }

        public class PortInfo
        {
            public PortInfo(int instance)
            {
                Instance = instance;
            }
            public int Instance { get; }
            public string Label { set; get; }

            public string IP { set; get; }

            public string Mask { set; get; }

            public string Gateway { set; get; }

            //public string Primary { set; get; }

            //public string Secondary { set; get; }

            //public string Domain { set; get; }

            //public string Host { set; get; }

            //public string Status { set; get; }

            //public int Config { set; get; }

        }

        private void CopyToc()
        {
            _cIp = IPAddress;
            _cMask = Subnet;
            _cGateway = Gateway;
            _cPrimary = Primary;
            _cSecondary = Secondary;
            _cDomain = Domain;
            _cHost = Host;
            IsDirty = false;
        }

        private void Compare()
        {
            IsDirty = !(_cIp == IPAddress &&
                        _cMask == Subnet &&
                        _cGateway == Gateway &&
                        _cPrimary == Primary &&
                        _cSecondary == Secondary &&
                        _cDomain == Domain &&
                        _cHost == Host);
        }

        private string _cIp;
        private string _cMask;
        private string _cGateway;
        private string _cPrimary;
        private string _cSecondary;
        private string _cDomain;
        private string _cHost;
        private PortInfo _selectedPort;

        private void UpdateInternet()
        {

        }

        private int _gettingPortInfo = 0;
        private async Task StartGetInfoAsync()
        {
            if (Interlocked.Exchange(ref _gettingPortInfo, 1) != 0) return;
            try
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                PortNum.Clear();
                await TaskScheduler.Default;
                Again:
                var result = await SetPortLabelAsync();
                if (result != -1)
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    SelectedPort = PortNum[0];
                }
                else
                {
                    await Task.Delay(300);
                    goto Again;
                }
            }
            finally
            {
                Interlocked.Exchange(ref _gettingPortInfo, 0);
            }
        }

    }
}
