using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.Interfaces.Notification;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.SimpleServices.Utilities;

namespace ICSStudio.SimpleServices.DeviceModule
{
    public class DeviceModuleCollection : IDeviceModuleCollection
    {
        private readonly List<IDeviceModule> _deviceModules;
        private readonly List<IDeviceModule> _trackedDeviceModules;

        public DeviceModuleCollection(IController controller)
        {
            ParentController = controller;
            Uid = Guid.NewGuid().GetHashCode();

            _deviceModules = new List<IDeviceModule>();
            _trackedDeviceModules = new List<IDeviceModule>();
        }

        public IEnumerator<IDeviceModule> GetEnumerator() => _deviceModules.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<IDeviceModule> TrackedDeviceModules => _trackedDeviceModules;

        public void AddDeviceModule(DeviceModule deviceModule)
        {
            deviceModule.ParentCollection = this;
            _deviceModules.Add(deviceModule);

            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, deviceModule));
        }

        public void AddTrackedDeviceModule(IDeviceModule deviceModule)
        {
            _trackedDeviceModules.Add(deviceModule);
        }

        public void Dispose()
        {
        }

        public IController ParentController { get; }
        public int Uid { get; }
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public int Count => _deviceModules.Count;

        public IDeviceModule this[int uid]
        {
            get
            {
                foreach (var module in _deviceModules)
                {
                    if (module.Uid == uid && !module.IsDeleted)
                        return module;
                }

                return null;
            }
        }

        public IDeviceModule this[string name]
        {
            get
            {
                foreach (var module in _deviceModules)
                {
                    if (module.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && !module.IsDeleted)
                        return module;
                }

                return null;
            }
        }

        public IDeviceModule TryGetChildByUid(int uid)
        {
            throw new NotImplementedException();
        }

        public IDeviceModule TryGetChildByName(string name)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<ComponentCoreInfo> GetComponentCoreInfoList()
        {
            throw new NotImplementedException();
        }

        public ComponentCoreInfo GetComponentCoreInfo(int uid)
        {
            throw new NotImplementedException();
        }

        public void RemoveDeviceModule(IDeviceModule deviceModule)
        {
            if (_deviceModules.Contains(deviceModule))
            {
                DeviceModule parentModule = deviceModule as DeviceModule;
                List<DeviceModule> children = new List<DeviceModule>();

                //remove child
                //TODO(gjc): add other port type
                var port = parentModule?.GetFirstPort(PortType.PointIO);
                if (port != null && !port.Upstream)
                {
                    foreach (var module in _deviceModules.OfType<DeviceModule>())
                    {
                        if (module == parentModule) continue;
                        if (module.ParentModule != parentModule)
                            continue;

                        if (module.ParentModPortId != port.Id)
                            continue;

                        children.Add(module);
                    }
                }

                foreach (var child in children)
                {
                    RemoveDeviceModule(child);
                }

                // remove axis
                CIPMotionDrive motionDrive = deviceModule as CIPMotionDrive;
                if (motionDrive != null)
                {
                    int axisCount = motionDrive.Profiles.Schema.Axes.Count;
                    for (int i = 1; i <= axisCount; i++)
                    {
                        var axisTag = motionDrive.GetAxis(i) as Tag;
                        AxisCIPDrive axisCIPDrive = axisTag?.DataWrapper as AxisCIPDrive;
                        if (axisCIPDrive != null)
                        {
                            motionDrive.RemoveAxis(axisTag, i);
                            axisCIPDrive.UpdateAxisChannel(null, 0);
                        }
                    }
                }

                parentModule?.RemoveDeviceTag();
                if (parentModule != null)
                {
                    Notifications.Publish(new MessageData()
                    {
                        Object = new List<ITag>() { parentModule.ConfigTag, parentModule.InputTag, parentModule.OutputTag },
                        Type = MessageData.MessageType.DelTag
                    });
                }
                _deviceModules.Remove(deviceModule);

                CollectionChanged?.Invoke(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, deviceModule));
            }
        }

        //public void ReplaceModule(DeviceModule deviceModule, DeviceModule replaceModule)
        //{
        //    if (_deviceModules.Contains(deviceModule))
        //    {
        //        DeviceModule parentModule = deviceModule;
        //        List<DeviceModule> children = new List<DeviceModule>();
        //        List<DeviceModule> removeList=new List<DeviceModule>();
        //        //remove child
        //        //TODO(gjc): add other port type
        //        var port = parentModule?.GetPort(PortType.Ethernet);
        //        var portIO = parentModule?.GetPort(PortType.PointIO);
        //        if (port != null && !port.Upstream)
        //        {
        //            foreach (var module in _deviceModules.OfType<DeviceModule>())
        //            {
        //                if (module == parentModule)
        //                    continue;
        //                if (module.ParentModule != parentModule)
        //                    continue;

        //                if (module.ParentModPortId == portIO.Id)
        //                {
        //                    removeList.Add(module);
        //                    continue;
        //                }

        //                children.Add(module);
        //            }
        //        }

        //        foreach (var child in removeList)
        //        {
        //            RemoveDeviceModule(child);
        //        }

        //        foreach (var child in children)
        //        {
        //            child.ParentModule = replaceModule;
        //            child.ParentModPortId= replaceModule.GetPort(PortType.Ethernet).Id;
        //        }

        //        parentModule?.RemoveDeviceTag();

        //        _deviceModules.Remove(deviceModule);

        //        CollectionChanged?.Invoke(this,
        //            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, deviceModule));
        //    }
        //}

        public void Clear()
        {
            _deviceModules.Clear();
        }

        public void Sort()
        {
            List<IDeviceModule> sortedModules = new List<IDeviceModule>();

            LocalModule localModule = this["Local"] as LocalModule;
            Contract.Assert(localModule != null);

            sortedModules.Add(localModule);

            foreach (var port in localModule.Ports)
            {
                List<DeviceModule> devices = new List<DeviceModule>();

                foreach (var module in _deviceModules)
                {
                    DeviceModule deviceModule = module as DeviceModule;
                    if (deviceModule != null)
                    {
                        if (deviceModule == localModule)
                            continue;

                        if (deviceModule.ParentModule != localModule)
                            continue;

                        if (deviceModule.ParentModPortId != port.Id)
                            continue;

                        devices.Add(deviceModule);
                    }
                }

                if (devices.Count == 0)
                    continue;

                // sort
                if (port.Type == PortType.Ethernet)
                {
                    devices.Sort(CompareEthernetAddress);

                    foreach (var device in devices)
                    {
                        sortedModules.Add(device);

                        // sub device
                        var pointIOPort = device.GetFirstPort(PortType.PointIO);
                        if (pointIOPort != null)
                        {
                            List<DeviceModule> subDevices = new List<DeviceModule>();

                            foreach (var module in _deviceModules)
                            {
                                DeviceModule deviceModule = module as DeviceModule;
                                if (deviceModule != null)
                                {
                                    if (deviceModule == localModule)
                                        continue;

                                    if (deviceModule.ParentModule != device)
                                        continue;

                                    if (deviceModule.ParentModPortId != pointIOPort.Id)
                                        continue;

                                    subDevices.Add(deviceModule);
                                }
                            }

                            if (subDevices.Count == 0)
                                continue;

                            subDevices.Sort((device0, device1) =>
                            {
                                var port0 = device0.GetFirstPort(pointIOPort.Type);
                                var port1 = device1.GetFirstPort(pointIOPort.Type);

                                return int.Parse(port0.Address) - int.Parse(port1.Address);
                            });

                            sortedModules.AddRange(subDevices);
                        }
                    }

                }
                else
                {
                    devices.Sort((device0, device1) =>
                    {
                        var port0 = device0.GetFirstPort(port.Type);
                        var port1 = device1.GetFirstPort(port.Type);

                        return int.Parse(port0.Address) - int.Parse(port1.Address);
                    });

                    sortedModules.AddRange(devices);
                }

            }

            // sort
            foreach (var module in _deviceModules.ToArray())
            {
                int newIndex = sortedModules.IndexOf(module);
                int oldIndex = _deviceModules.IndexOf(module);
                if (newIndex != oldIndex)
                {
                    var tempModule = _deviceModules[newIndex];
                    _deviceModules[newIndex] = module;
                    _deviceModules[oldIndex] = tempModule;
                }
            }

        }

        private int CompareEthernetAddress(DeviceModule deviceModule0, DeviceModule deviceModule1)
        {
            string name0 = CompareNameConverter.EthernetAddressToCompareName(deviceModule0);
            string name1 = CompareNameConverter.EthernetAddressToCompareName(deviceModule1);

            return string.Compare(name0, name1, StringComparison.OrdinalIgnoreCase);
        }

    }
}
