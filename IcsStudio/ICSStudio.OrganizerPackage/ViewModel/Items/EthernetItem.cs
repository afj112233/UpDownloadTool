using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;
using ICSStudio.Descriptor;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.SimpleServices.Utilities;

namespace ICSStudio.OrganizerPackage.ViewModel.Items
{
    public class EthernetItem : OrganizerItem
    {
        private readonly DeviceModule _deviceModule;
        private readonly Port _port;
        private readonly Controller _controller;

        public EthernetItem(IDeviceModule deviceModule, PortType portType)
        {
            _deviceModule = deviceModule as DeviceModule;
            Contract.Assert(_deviceModule != null);

            _port = _deviceModule.GetFirstPort(portType);
            Contract.Assert(_port != null);

            Name = $"{_deviceModule.DisplayText}";

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

                UpdateInhibited();
                UpdateIsWarning();
            }

            UpdateToolTip();
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

        private void OnPortPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Address")
            {
                UpdateToolTip();

                if (!(_deviceModule is LocalModule))
                {
                    SortByAddress();
                }
            }
        }

        private void SortByAddress()
        {
            OrganizerItems items = Collection as OrganizerItems;
            Contract.Assert(items != null);

            int oldIndex = items.IndexOf(this);

            List<string> itemNames = items.Select(x =>
            {
                DeviceModule deviceModule = x.AssociatedObject as DeviceModule;

                if (deviceModule != null)
                {
                    return CompareNameConverter.EthernetAddressToCompareName(deviceModule);
                }

                return x.Name;
            }).ToList();

            string itemName = itemNames[oldIndex];
            itemNames.Sort((x, y) => string.Compare(x, y, StringComparison.OrdinalIgnoreCase));
            int newIndex = itemNames.IndexOf(itemName);

            if (oldIndex != newIndex)
                items.Move(oldIndex, newIndex);
        }

        private void OnDevicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DisplayText")
            {
                Name = $"{_deviceModule.DisplayText}";
                UpdateToolTip();
            }

            if (e.PropertyName == "EntryStatus")
            {
                UpdateInhibited();
                UpdateIsWarning();
            }

            if (e.PropertyName == "Inhibited")
            {
                UpdateInhibited();
            }
        }


        private void UpdateToolTip()
        {
            string toolTip = "IP Address: ";

            if (string.IsNullOrEmpty(_port.Address))
            {
                if (_deviceModule != null)
                {
                    toolTip = $"{_deviceModule.DisplayText}";
                }
            }
            else
            {
                toolTip += _port.Address;
            }

            ToolTip = toolTip;
        }

        protected override void DisplayNameConvert()
        {
            DisplayName = Name;
        }
    }
}
