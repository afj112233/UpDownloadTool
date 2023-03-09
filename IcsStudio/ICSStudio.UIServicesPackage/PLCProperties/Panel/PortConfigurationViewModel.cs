using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.Objects;
using ICSStudio.Dialogs.PortDiagnostics;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DeviceModule;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace ICSStudio.UIServicesPackage.PLCProperties.Panel
{
    class PortConfigurationViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private bool _isDirty;
        //private Cip.Objects.PortConfiguration _portConfiguration;
        private IController _controller;
        public PortConfigurationViewModel(PortConfiguration panel,IController controller)
        {
            Control = panel;
            panel.DataContext = this;
            _controller = controller;
            
            if (controller.IsOnline)
            {
                SetAttr();
            }
            else
            {
                Clear();
            }
        }

        private void Clear()
        {
            Ports.Clear();
            for (int i = 0; i < 2; i++)
            {
                var info = new PortInfo();
                info.Name = i.ToString();
                info.IsEnabled = true;
                Ports.Add(info);
            }
        }
        private void SetAttr()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                var portConfiguration = new Cip.Objects.PortConfiguration(((Controller) _controller).CipMessager);
                Ports.Clear();
                await portConfiguration.GetPortInfo();
                int index = 1;
                foreach (var port in portConfiguration.Ports)
                {
                    if (!(port.InterfaceState == CIPEthernetLinkObject.State.Disable ||
                          port.InterfaceState == CIPEthernetLinkObject.State.Enable)) continue;
                    var info = new PortInfo();
                    info.Name = index.ToString();
                    info.IsEnabled = port.InterfaceState == CIPEthernetLinkObject.State.Enable;
                    info.Status = port.Status.ToString();
                    info.AutoNegotiate =
                        (port.Negotiation == CIPEthernetLinkObject.NegotiationStatus.AutoNegotiationInProgress ||
                         port.Negotiation == CIPEthernetLinkObject.NegotiationStatus
                             .NegotiationSpeedAndDuplexSuccessfully);
                    info.Speed = port.Speed.ToString((IFormatProvider) null);
                    info.Duplex = port.DuplexStatus.ToString();
                    Ports.Add(info);
                    index++;
                }
                
            });

        }

        public void Refresh()
        {
            if (_controller.IsOnline)
            {
                SetAttr();
            }
            else
            {
                Clear();
            }
        }

        public ObservableCollection<PortInfo> Ports { set; get; }=new ObservableCollection<PortInfo>();
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
    }

    internal class PortInfo:ViewModelBase
    {
        private bool _autoNegotiate;
        private bool _isEnabled;

        public PortInfo()
        {
            Command=new RelayCommand(ExecuteCommand,CanExecuteCommand);
        }
        
        public string Name { set; get; }

        public bool IsEnabled
        {
            set { Set(ref _isEnabled , value); }
            get { return _isEnabled; }
        }

        public string Status { set; get; }

        public bool AutoNegotiate
        {
            set { Set(ref _autoNegotiate , value); }
            get { return _autoNegotiate; }
        }

        public string Speed { set; get; }
        
        public string Duplex { set; get; }

        public RelayCommand Command { set; get; }

        private bool CanExecuteCommand()
        {
            return false;
        }

        private void ExecuteCommand()
        {
#pragma warning disable VSTHRD010 // 在主线程上调用单线程类型
            var uiShell = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
#pragma warning restore VSTHRD010 // 在主线程上调用单线程类型
            var dialog = new PortDiagnostics();
            var vm=new PortDiagnosticsVM((DeviceModule)Controller.GetInstance().DeviceModules["Local"], int.Parse(Name));
            dialog.DataContext = vm;
            dialog.ShowDialog(uiShell);
        }
    }
}
