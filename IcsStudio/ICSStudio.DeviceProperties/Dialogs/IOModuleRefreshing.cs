using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.CipConnection;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DeviceModule;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;

namespace ICSStudio.DeviceProperties.Dialogs
{
    internal class IOModuleRefreshing: RefreshingViewModel
    {
        private readonly DeviceModule _ioModule;
        private readonly int _slot;

        public IOModuleRefreshing(DiscreteIO discreteIO)
        {
            Contract.Assert(discreteIO != null);

            _ioModule = discreteIO;
            _slot = discreteIO.Slot;
        }

        public IOModuleRefreshing(AnalogIO analogIO)
        {
            Contract.Assert(analogIO != null);

            _ioModule = analogIO;
            _slot = analogIO.Slot;
        }

        public override void Refresh()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                int result = int.MinValue;

                try
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    Message = "Begin refresh......";

                    await TaskScheduler.Default;
                    var controller = _ioModule.ParentController as Controller;

                    if (controller == null)
                    {
                        result = -1000;
                    }
                    else
                    {
                        ICipMessager messager = await controller.GetMessager(_ioModule);
                        if (messager == null || messager.ConnectionStatus == ConnectionStatus.Disconnected)
                        {
                            await controller.RemoveMessagerByDeviceModule(_ioModule);
                            await System.Threading.Tasks.Task.Delay(3000);
                            messager = await controller.GetMessager(_ioModule);
                        }

                        if (messager == null || messager.ConnectionStatus == ConnectionStatus.Disconnected)
                        {
                            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                            Message = "Connect time out!";

                            result = -1;
                        }
                        else
                        {
                            DeviceConnection deviceConnection = messager as DeviceConnection;

                            CIPConnectionManager connectionManager = deviceConnection?.ConnectionManager;

                            if (connectionManager != null)
                            {
                                CIPIdentity cipIdentity =
                                    new CIPIdentity(1, messager);

                                var request =
                                    connectionManager.CreateUnconnectedSendRequest(
                                        _slot,
                                        cipIdentity.GetAttributesAllRequest());

                                var response = await messager.SendRRData(request);
                                if (cipIdentity.ParseGetAttributesAllResponse(response) == 0)
                                {
                                    Descriptor = new IdentityDescriptor(cipIdentity);

                                    // Protection Mode
                                    request = connectionManager.CreateUnconnectedSendRequest(
                                        _slot,
                                        cipIdentity.GetAttributeSingleRequest((ushort)IdentityAttributeId.ProtectionMode));

                                    response = await messager.SendRRData(request);
                                    if (response != null && response.GeneralStatus == (byte)CipGeneralStatusCode.Success)
                                    {
                                        cipIdentity.ProtectionMode = BitConverter.ToUInt16(response.ResponseData, 0);
                                    }

                                    result = 0;
                                }
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    result = -100;
                }
                finally
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    DialogResult = result == 0;
                }
            });

        }
    }
}
