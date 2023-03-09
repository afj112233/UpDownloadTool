using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Windows;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DeviceModule;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Task = System.Threading.Tasks.Task;

namespace ICSStudio.DeviceProperties.Dialogs
{
    internal class DeviceModuleRefreshing : RefreshingViewModel
    {
        private readonly DeviceModule _deviceModule;

        public DeviceModuleRefreshing(DeviceModule deviceModule)
        {
            Contract.Assert(deviceModule != null);

            _deviceModule = deviceModule;
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
                            await controller.RemoveMessagerByDeviceModule(_deviceModule);
                            await Task.Delay(3000);
                            messager = await controller.GetMessager(_deviceModule);
                        }

                        if (messager == null || messager.ConnectionStatus == ConnectionStatus.Disconnected)
                        {
                            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                            Message = "Connect time out!";

                            result = -1;
                        }
                        else
                        {
                            CIPIdentity cipIdentity = new CIPIdentity(1, messager);

                            result = await cipIdentity.GetAttributesAll();

                            if (result == 0)
                            {
                                // Protection Mode
                                var request =
                                    cipIdentity.GetAttributeSingleRequest((ushort)IdentityAttributeId.ProtectionMode);

                                var response = await messager.SendRRData(request);
                                if (response != null && response.GeneralStatus == (byte)CipGeneralStatusCode.Success)
                                {
                                    cipIdentity.ProtectionMode = BitConverter.ToUInt16(response.ResponseData, 0);
                                }

                                Descriptor = new IdentityDescriptor(cipIdentity);
                            }
                            
                        }

                        if (result != 0)
                        {
                            string message = "Requested message timed out.";

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

