using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using ICSStudio.Cip.Objects;
using ICSStudio.DownloadChange;
using ICSStudio.SimpleServices;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Error;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using NLog;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    [SuppressMessage("ReSharper", "CommentTypo")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    internal class DownloadChangeViewModel : ViewModelBase
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly Controller _controller;
        private readonly ProjectDiffModel _diffModel;
        private readonly bool _isDownloadAxisParameters;
        private string _message;

        private bool? _dialogResult;

        public DownloadChangeViewModel(Controller controller, ProjectDiffModel diffModel, bool isDownloadAxisParameters)
        {
            _controller = controller;
            _diffModel = diffModel;
            _isDownloadAxisParameters = isDownloadAxisParameters;
        }

        public string Message
        {
            get { return _message; }
            set { Set(ref _message, value); }
        }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        public void DownloadChange()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                // ReSharper disable once RedundantAssignment
                int result = 0;
                try
                {
                    while (true)
                    {
                        Logger.Info("Begin download change...");

                        DownloadChangeHelper helper = new DownloadChangeHelper(_diffModel, _controller);
                        if (!helper.CanDownload)
                        {
                            Logger.Info("Can't download change!");
                            result = -1;
                            break;
                        }

                        await helper.Lock();

                        //1. controller tags
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        Message = $"Create Controller Tags...";

                        await TaskScheduler.Default;
                        result = await helper.CreateControllerTags();
                        if (result < 0)
                        {
                            await helper.Unlock();
                            break;
                        }

                        //2. program tags
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        Message = $"Create program Tags...";

                        await TaskScheduler.Default;
                        result = await helper.CreateProgramTags();
                        if (result < 0)
                        {
                            await helper.Unlock();
                            break;
                        }

                        //3. inhibit tasks
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        Message = $"Inhibit tasks...";

                        await TaskScheduler.Default;
                        result = await helper.InhibitTasks();
                        if (result < 0)
                        {
                            await helper.Unlock();
                            break;
                        }

                        //4. inhibit program
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        Message = $"Inhibit programs...";

                        await TaskScheduler.Default;
                        result = await helper.InhibitPrograms();
                        if (result < 0)
                        {
                            await helper.Unlock();
                            break;
                        }

                        //5. update code
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        Message = $"Update code...";

                        await TaskScheduler.Default;
                        result = await helper.UpdatePrograms();
                        if (result < 0)
                        {
                            await helper.Unlock();
                            break;
                        }

                        //6. update task rate
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        Message = $"Update tasks rate...";

                        await TaskScheduler.Default;
                        result = await helper.UpdateTasksRate();
                        if (result < 0)
                        {
                            await helper.Unlock();
                            break;
                        }

                        //7. uninhibit program
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        Message = $"Uninhibit programs...";

                        await TaskScheduler.Default;
                        result = await helper.UninhibitPrograms();
                        if (result < 0)
                        {
                            await helper.Unlock();
                            break;
                        }

                        //8. uninhibit task 
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        Message = $"Uninhibit tasks...";

                        await TaskScheduler.Default;
                        result = await helper.UninhibitTasks();
                        if (result < 0)
                        {
                            await helper.Unlock();
                            break;
                        }

                        //9. set axis properties
                        if (_isDownloadAxisParameters)
                        {
                            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                            Message = $"Set axis properties...";

                            await TaskScheduler.Default;

                            var axisPropertiesChange = helper.GetAxisPropertiesChange();

                            result = await SetAxisPropertiesAsync(axisPropertiesChange);
                            if (result < 0)
                            {
                                await helper.Unlock();
                                break;
                            }
                        }

                        //TODO(gjc): add code here


                        var transactionManager =
                            _controller.Lookup(typeof(TransactionManager)) as
                                TransactionManager;

                        if (transactionManager != null)
                        {
                            transactionManager.Reset();
                            await transactionManager.PostUploadSetupAsync(_controller.CipMessager);
                        }

                        await helper.Unlock();

                        // end download change

                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        Message = $"Synchronizing Data for project '{_controller.Name}'";

                        await TaskScheduler.Default;
                        result = await _controller.RebuildTagSyncControllerAsync();

                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        Message = $"Update controller state";

                        await TaskScheduler.Default;
                        await _controller.UpdateState();

                        Logger.Info("End download change...");

                        break;
                    }

                }
                catch (Exception e)
                {
                    //ignore
                    Logger.Error($"Download change failed({Message}): {e}");

                    result = -100;
                }
                finally
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    DialogResult = result == 0;
                }

            });
        }

        private async Task<int> SetAxisPropertiesAsync(Dictionary<string, List<string>> axisPropertiesChange)
        {
            var outputService = Package.GetGlobalService(typeof(SErrorOutputService)) as IErrorOutputService;

            CIPController cipController = new CIPController(0, _controller.CipMessager);

            foreach (var keyValuePair in axisPropertiesChange)
            {
                string axisName = keyValuePair.Key;
                List<string> properties = keyValuePair.Value;

                var axisTag = _controller.Tags[axisName] as Tag;
                AxisCIPDrive axisCIPDrive = axisTag?.DataWrapper as AxisCIPDrive;
                Debug.Assert(axisCIPDrive != null);

                int instanceId = await cipController.FindTagId(axisName);

                axisCIPDrive.CIPAxis.InstanceId = instanceId;
                axisCIPDrive.CIPAxis.Messager = _controller.CipMessager;

                foreach (var property in properties)
                {
                    await TaskScheduler.Default;

                    Logger.Info($"DownloadAxisParameters: Axis {axisName}: {property}");

                    int updateResult =
                        await UpdateAxisParameterAsync(axisCIPDrive, property);

                    if (updateResult != 0)
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        outputService?.AddWarnings($"DownloadAxisParameters failed: Axis {axisName}: {property}", null);

                        Logger.Info($"DownloadAxisParameters failed: Axis {axisName}: {property}");
                    }
                }
            }


            return 0;
        }

        private async Task<int> UpdateAxisParameterAsync(AxisCIPDrive axisCIPDrive, string axisParameter)
        {
            try
            {
                await axisCIPDrive.CIPAxis.SetAttributeSingle(axisParameter);

                return 0;
            }
            catch (Exception)
            {
                //TODO(gjc): remove later for TorqueOffset
                if (axisParameter != "TorqueOffset")
                {
                    return -1;
                }

            }

            return 0;
        }
    }
}
