using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Windows;
using ICSStudio.Descriptor;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.UIInterfaces.Project;

namespace ICSStudio.OrganizerPackage.ViewModel.Items
{
    internal class PointIOItem : OrganizerItem
    {
        private readonly DeviceModule _deviceModule;
        private readonly Port _port;
        private readonly Controller _controller;

        public PointIOItem(IDeviceModule deviceModule, PortType portType)
        {
            _deviceModule = deviceModule as DeviceModule;
            Contract.Assert(_deviceModule != null);

            _port = _deviceModule.GetFirstPort(portType);
            Contract.Assert(_port != null);

            Name = $"[{_port.Address}] {_deviceModule.DisplayText}";

            Kind = ProjectItemType.DeviceModule;

            if (_deviceModule is LocalModule)
                Kind = ProjectItemType.LocalModule;

            AssociatedObject = _deviceModule;

            PropertyChangedEventManager.AddHandler(_deviceModule, OnDevicePropertyChanged, "");
            PropertyChangedEventManager.AddHandler(_port, OnPortPropertyChanged, "");

            _controller = _deviceModule?.ParentController as Controller;
            if (_controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    _controller, "IsOnlineChanged", OnIsOnlineChanged);

                UpdateIsWarning();
                UpdateInhibited();
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();
            PropertyChangedEventManager.RemoveHandler(_deviceModule, OnDevicePropertyChanged, "");
            PropertyChangedEventManager.RemoveHandler(_port, OnPortPropertyChanged, "");
            if (_controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                    _controller, "IsOnlineChanged", OnIsOnlineChanged);
                
            }
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            UpdateIsWarning();
            UpdateInhibited();
        }

        private void UpdateIsWarning()
        {
            if (_deviceModule.ParentController.IsOnline)
            {
                if (Kind != ProjectItemType.LocalModule)
                {
                    ModuleDescriptor descriptor = new ModuleDescriptor(_deviceModule);
                    if (descriptor.Status != "Running" && descriptor.Status != "Inhibited")
                    {
                        IsWarning = true;
                    }
                    else
                    {
                        IsWarning = false;
                    }
                }
            }
            else
            {
                IsWarning = false;
            }
        }

        private void UpdateInhibited()
        {
            ModuleDescriptor descriptor = new ModuleDescriptor(_deviceModule);
            if (_controller.IsOnline)
            {
                Inhibited = descriptor.Status == "Inhibited";
            }
            else
            {
                Inhibited = _deviceModule.Inhibited;
            }
        }

        public string Address => _port.Address;

        private void OnPortPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Address")
            {
                Name = $"[{_port.Address}] {_deviceModule.DisplayText}";

                SortByPortIndex();
            }
        }

        private void SortByPortIndex()
        {
            var parentItems = Collection as OrganizerItems;
            Contract.Assert(parentItems != null);

            int oldIndex = parentItems.IndexOf(this);

            int newPortIndex = int.Parse(_port.Address);

            int newIndex = 0;
            foreach (var item in parentItems)
            {
                if (item.AssociatedObject == parentItems.Parent.AssociatedObject)
                {
                    newIndex++;
                    continue;
                }

                if (item == this)
                {
                    continue;
                }

                PointIOItem pointIOItem = item as PointIOItem;
                Contract.Assert(pointIOItem != null);

                int portIndex = int.Parse(pointIOItem.Address);

                if (portIndex > newPortIndex)
                    break;

                newIndex++;
            }

            if (oldIndex != newIndex)
                parentItems.Move(oldIndex, newIndex);
        }

        private void OnDevicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DisplayText")
            {
                Name = $"[{_port.Address}] {_deviceModule.DisplayText}";
            }

            if (e.PropertyName == "Inhibited")
            {
                UpdateInhibited();
            }

            if (e.PropertyName == "EntryStatus")
            {
                UpdateInhibited();
                UpdateIsWarning();
            }
        }

        protected override void DisplayNameConvert()
        {
            DisplayName = Name;
        }
    }
}
