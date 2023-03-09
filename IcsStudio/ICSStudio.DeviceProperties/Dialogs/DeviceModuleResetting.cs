using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Windows;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DeviceModule;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Task = System.Threading.Tasks.Task;

namespace ICSStudio.DeviceProperties.Dialogs
{
    internal class DeviceModuleResetting : ResettingViewModel
    {
        private readonly DeviceModule _deviceModule;

        public DeviceModuleResetting(DeviceModule deviceModule)
        {
            Contract.Assert(deviceModule != null);

            _deviceModule = deviceModule;
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
                    var controller = _deviceModule.ParentController as Controller;

                    if (controller == null)
                    {
                        result = -1000;
                    }
                    else
                    {
                        ICipMessager messager = await controller.GetMessager(_deviceModule);

                        if (messager == null || messager.ConnectionStatus == ConnectionStatus.Disconnected)
                        {
                            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                            Message = "Connect time out!";

                            result = -1;
                        }
                        else
                        {
                            CIPIdentity cipIdentity = new CIPIdentity(1, messager);

                            var response = await messager.SendRRData(cipIdentity.GetResetRequest());

                            if (response != null
                                && response.GeneralStatus == (byte) CipGeneralStatusCode.DeviceStateConflict)
                            {
                                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                                Message = "Failed to perform required operation!";

                                result = -2;
                            }
                            else
                            {
                                await controller.RemoveMessagerByDeviceModule(_deviceModule);

                                await Task.Delay(5000);

                                messager = await controller.GetMessager(_deviceModule);

                                if (messager == null || messager.ConnectionStatus == ConnectionStatus.Disconnected)
                                {
                                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                                    Message = "Connect time out!";

                                    result = -1;
                                }
                                else
                                {
                                    result = 0;
                                }
                            }
                        }

                        //
                        if(result != 0)
                        {
                            string message = string.Empty;

                            if (result == -1)
                            {
                                message = "Requested message timed out.";
                            }
                            else if (result == -2)
                            {
                                //message = "Failed to perform required operation.\n";
                                //message += "Unable to perform service due to current module mode.";
                                message = "Failed to perform required operation.Unable to perform service due to current module mode.";
                            }
                            
                            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                            Window owner = Application.Current.MainWindow;
                            if (owner != null)
                                MessageBox.Show(owner, LanguageManager.GetInstance().ConvertSpecifier(message), "ICS Studio", MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                            else
                                MessageBox.Show(LanguageManager.GetInstance().ConvertSpecifier(message), "ICS Studio", MessageBoxButton.OK,
                                    MessageBoxImage.Information);
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
