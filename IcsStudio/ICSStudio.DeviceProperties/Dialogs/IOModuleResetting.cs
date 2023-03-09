using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.CipConnection;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DeviceModule;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Task = System.Threading.Tasks.Task;

namespace ICSStudio.DeviceProperties.Dialogs
{
    internal class IOModuleResetting : ResettingViewModel
    {
        private readonly DeviceModule _ioModule;
        private readonly int _slot;

        public IOModuleResetting(DiscreteIO discreteIO)
        {
            Contract.Assert(discreteIO != null);

            _ioModule = discreteIO;
            _slot = discreteIO.Slot;
        }

        public IOModuleResetting(AnalogIO analogIO)
        {
            Contract.Assert(analogIO != null);

            _ioModule = analogIO;
            _slot = analogIO.Slot;
        }

        public override void Reset()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                int result = int.MinValue;

                try
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    Message = "Begin reset......";

                    await TaskScheduler.Default;
                    var controller = _ioModule.ParentController as Controller;
                    if (controller != null)
                    {
                        ICipMessager messager = await controller.GetMessager(_ioModule);

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

                            if (connectionManager == null)
                            {
                                result = -2;
                            }
                            else
                            {
                                CIPIdentity cipIdentity =
                                    new CIPIdentity(1, messager);

                                //TODO(gjc): need check EPath
                                var request =
                                    connectionManager.CreateUnconnectedSendRequest(
                                        _slot,
                                        cipIdentity.GetResetRequest());

                                var response = await messager.SendRRData(request);

                                if (response == null || response.GeneralStatus != (byte) CipGeneralStatusCode.Success)
                                {
                                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                                    Message = "Reset failed!";

                                    result = -3;
                                }
                                else
                                {
                                    // sleep 5 seconds
                                    await Task.Delay(5000);

                                    request =
                                        connectionManager.CreateUnconnectedSendRequest(
                                            _slot,
                                            cipIdentity.GetAttributesAllRequest());

                                    response = await messager.SendRRData(request);

                                    if (response == null ||
                                        response.GeneralStatus != (byte) CipGeneralStatusCode.Success)
                                    {
                                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                                        Message = "Reconnect failed!";

                                        result = -4;
                                    }
                                }

                                result = 0;

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
