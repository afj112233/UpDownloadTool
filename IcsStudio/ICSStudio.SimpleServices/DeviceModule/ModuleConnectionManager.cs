using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.CipConnection;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.SimpleServices.DeviceModule
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    internal class ModuleConnectionManager
    {
        private readonly Controller _controller;

        private readonly SemaphoreSlim _asyncLock;
        private readonly Dictionary<IDeviceModule, ICipMessager> _messagers;

        public ModuleConnectionManager(Controller controller)
        {
            _controller = controller;
            _asyncLock = new SemaphoreSlim(1);
            _messagers = new Dictionary<IDeviceModule, ICipMessager>();

            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                _controller, "IsOnlineChanged", OnIsOnlineChanged);
        }

        ~ModuleConnectionManager()
        {
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                _controller, "IsOnlineChanged", OnIsOnlineChanged);
        }

        internal async Task<ICipMessager> GetMessager(IDeviceModule deviceModule)
        {
            if (!_controller.IsOnline)
                return null;

            if (deviceModule == null)
                return null;


            DiscreteIO discreteIO = deviceModule as DiscreteIO;
            if (discreteIO != null)
            {
                return await GetMessager(discreteIO.ParentModule);
            }

            AnalogIO analogIO = deviceModule as AnalogIO;
            if (analogIO != null)
            {
                return await GetMessager(analogIO.ParentModule);
            }

            LocalModule localModule = deviceModule as LocalModule;
            if (localModule != null)
            {
                return _controller.CipMessager;
            }

            if (deviceModule is CIPMotionDrive || deviceModule is CommunicationsAdapter)
            {
                await _asyncLock.WaitAsync();

                try
                {
                    string ipAddress = GetIpAddress(deviceModule);

                    if (_messagers.ContainsKey(deviceModule))
                    {
                        return _messagers[deviceModule];
                    }
                    else
                    {
                        var message = new DeviceConnection(ipAddress);

                        int result = await message.OnLine(false);

                        if (result == 0)
                        {
                            _messagers.Add(deviceModule, message);
                            return message;
                        }
                        else
                        {
                            message.OffLine();
                            return null;
                        }
                    }

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
                finally
                {
                    _asyncLock.Release();
                }
            }

            return null;
        }

        internal async Task RemoveMessager(IDeviceModule deviceModule)
        {
            if (!_controller.IsOnline)
                return;

            if (deviceModule == null)
                return;

            IDeviceModule targetModule = deviceModule;

            DiscreteIO discreteIO = deviceModule as DiscreteIO;
            if (discreteIO != null)
            {
                targetModule = discreteIO.ParentModule;
            }

            AnalogIO analogIO = deviceModule as AnalogIO;
            if (analogIO != null)
            {
                targetModule = analogIO.ParentModule;
            }

            await _asyncLock.WaitAsync();

            if (_messagers.ContainsKey(targetModule))
            {
                var cipMessager = _messagers[targetModule];

                try
                {
                    cipMessager?.OffLine();
                }
                catch (Exception)
                {
                    //ignore
                }
                
                _messagers.Remove(targetModule);
            }

            _asyncLock.Release();
        }

        private string GetIpAddress(IDeviceModule module)
        {
            DeviceModule deviceModule = module as DeviceModule;

            if (deviceModule != null)
            {
                foreach (var port in deviceModule.Ports)
                {
                    if (port.Type == PortType.Ethernet)
                        return port.Address;
                }
            }

            return string.Empty;
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            Task.Run(async () =>
            {
                await _asyncLock.WaitAsync();

                var connections = _messagers.Values.ToList();
                _messagers.Clear();

                _asyncLock.Release();

                foreach (var cipMessager in connections)
                {
                    try
                    {
                        cipMessager?.OffLine();
                    }
                    catch (Exception)
                    {
                        //ignor
                    }

                }
            });
        }


    }
}
