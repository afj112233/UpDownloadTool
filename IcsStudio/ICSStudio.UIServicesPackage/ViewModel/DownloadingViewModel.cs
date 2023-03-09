using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.CipConnection;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Error;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json;
using NLog;
using ICSStudio.DownloadOptions.Preserve;
using ICSStudio.DownloadOptions.ICSStudio.RestoreTest;
using ICSStudio.DownloadOptions.Restore;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Online;
using Task = System.Threading.Tasks.Task;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    public enum DownloadStageType
    {
        Begin,

        Build,
        ChangeProgramMode,
        SaveToController,
        Preserve,
        Download,
        SyncData,
        UpdateState,
        Restore,

        End
    }

    public class DownloadingViewModel : ViewModelBase
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly Controller _controller;

        private bool? _dialogResult;

        private string _message;
        private double _progress;

        private readonly IErrorOutputService _outputService;

        public DownloadingViewModel(Controller controller)
        {
            _controller = controller;

            _outputService = Package.GetGlobalService(typeof(SErrorOutputService)) as IErrorOutputService;

            DownloadStage = DownloadStageType.Begin;

            CancelCommand = new RelayCommand(ExecuteCancel, CanExecuteCancel);
        }

        private bool CanExecuteCancel()
        {
            //TODO(gjc): add code here
            return false;
        }

        private void ExecuteCancel()
        {
            //TODO(gjc): add code here
        }

        public RelayCommand CancelCommand { get; }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        public string Message
        {
            get { return _message; }
            set { Set(ref _message, value); }
        }

        public double Progress
        {
            set { Set(ref _progress, value); }
            get { return _progress; }
        }

        public DownloadStageType DownloadStage { get; private set; }

        public void Download(string preserveFilePath, string restoreFilePath)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                int result = int.MinValue;

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                try
                {
                    Logger.Info($"Begin download project '{_controller.Name}'");

                    //1. Build
                    await UpdateProgressAndMessageAsync(0, LanguageManager.GetInstance().ConvertSpecifier("CompilingProject") +$" '{_controller.Name}'");
                    await BuildAsync();

                    //2. Change Mode
                    await UpdateProgressAndMessageAsync(20, LanguageManager.GetInstance().ConvertSpecifier("ChangeControllerIntoProgramMode"));
                    await ChangeModeAsync();

                    //3. Save to controller
                    bool hasProject = await HasProjectAsync();
                    if (hasProject)
                    {
                        await UpdateProgressAndMessageAsync(25, LanguageManager.GetInstance().ConvertSpecifier("SaveToController"));
                        await SaveToControllerAsync();
                    }

                    ProjectInfo projectInfo = null;

                    //4. Preserve
                    if (!string.IsNullOrEmpty(preserveFilePath))
                    {
                        await UpdateProgressAndMessageAsync(35, LanguageManager.GetInstance().ConvertSpecifier("PreservingOnlineTagValues"));

                        projectInfo = await PreserveAsync(preserveFilePath);
                    }

                    //5. download project
                    await UpdateProgressAndMessageAsync(45, LanguageManager.GetInstance().ConvertSpecifier("DownloadingProject") +$" '{_controller.Name}'");
                    await DownloadAsync();

                    //6. sync data
                    await UpdateProgressAndMessageAsync(75, LanguageManager.GetInstance().ConvertSpecifier("SynchronizingDataForProject") +$" '{_controller.Name}'");
                    await SynchronizingDataAsync();

                    //7. update state
                    await UpdateProgressAndMessageAsync(85, LanguageManager.GetInstance().ConvertSpecifier("UpdateControllerState"));
                    await UpdateStateAsync();

                    //8. Restore
                    if (!string.IsNullOrEmpty(restoreFilePath) || projectInfo != null)
                    {
                        await UpdateProgressAndMessageAsync(90, LanguageManager.GetInstance().ConvertSpecifier("RestoringOnlineTagValues"));
                        await RestoreAsync(restoreFilePath, projectInfo);
                    }

                    // end
                    DownloadStage = DownloadStageType.End;
                    stopwatch.Stop();

                    await UpdateProgressAndMessageAsync(100,
                        $"Download elapsed time {stopwatch.Elapsed.TotalSeconds}.");


                    result = 0;
                }
                catch (Exception e)
                {
                    //ignore
                    Console.WriteLine(e);

                    Logger.Error($"download failed({Message}): {e}");

                    result = -100;
                }
                finally
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    DialogResult = result == 0;
                }

            });
        }

        private async Task<bool> HasProjectAsync()
        {
            CIPController cipController = new CIPController(0, _controller.CipMessager);

            int res = await cipController.GetHasProject();
            if (res == 1)
            {
                return true;
            }

            return false;
        }

        private async Task UpdateProgressAndMessageAsync(double progress, string message)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            Progress = progress;
            Message = message;

            _outputService?.AddMessages(message, null);

            Logger.Info(message);
        }

        private async Task BuildAsync()
        {
            await TaskScheduler.Default;

            DownloadStage = DownloadStageType.Build;

            _controller.GenCode();
        }

        private async Task ChangeModeAsync()
        {
            await TaskScheduler.Default;

            DownloadStage = DownloadStageType.ChangeProgramMode;

            int retryCount = 0;
            while (_controller.OperationMode != ControllerOperationMode.OperationModeProgram)
            {
                await _controller.ChangeOperationMode(ControllerOperationMode.OperationModeProgram);
                await _controller.UpdateState();
                await Task.Delay(30);

                retryCount++;
                if (retryCount >= 30)
                    throw new ApplicationException("Controller is not in program mode!");
            }
        }

        private async Task SaveToControllerAsync()
        {
            await TaskScheduler.Default;

            DownloadStage = DownloadStageType.SaveToController;

            OnlineEditHelper onlineEditHelper = new OnlineEditHelper(_controller.CipMessager);
            int result = await onlineEditHelper.SaveToController();

            if (result != 0)
                throw new ApplicationException("Save to controller failed!");

        }

        private async Task<ProjectInfo> PreserveAsync(string preserveFilePath)
        {
            await TaskScheduler.Default;

            DownloadStage = DownloadStageType.Preserve;

            if (!string.IsNullOrEmpty(preserveFilePath))
            {
                //创建cipMessage
                ICipMessager cipMessager = _controller.CipMessager;
                DeviceConnection deviceConnection = cipMessager as DeviceConnection;
                DataTypeHelper dataTypeHelper = new DataTypeHelper();
                PreserveHelper preserveHelper = new PreserveHelper(cipMessager, dataTypeHelper);

                var projectInfo = new ProjectInfo()
                {
                    ProjectName = await new CIPController(0, cipMessager).GetName(),
                    CommunicationPath = deviceConnection?.IpAddress,
                    User = Environment.MachineName + @"\" + Environment.UserName,
                    DownloadTimestamp = DateTime.Now.ToString("ddd MMM dd HH:mm:ss yyyy",
                        DateTimeFormatInfo.InvariantInfo)
                };

                //1. get data types
                await preserveHelper.GetDataTypes(projectInfo);
                dataTypeHelper.AddDataTypes(projectInfo.DataTypes);

                //2. get tag value
                int getAllTagsResult = await preserveHelper.GetAllTags(projectInfo);

                //3. save json file
                if (getAllTagsResult == 0)
                {
                    try
                    {
                        using (var sw = File.CreateText(preserveFilePath))
                        {
                            JsonSerializer serializer = new JsonSerializer
                            {
                                Formatting = Formatting.Indented
                            };
                            serializer.Serialize(sw, projectInfo);
                        }
                    }
                    catch (Exception)
                    {
                        string message = $"Save preserve file failed!: '{preserveFilePath}'";
                        await UpdateProgressAndMessageAsync(Progress, message);

                        throw;
                    }

                }
                else
                {
                    string message = "Preserving Data failed!";
                    await UpdateProgressAndMessageAsync(Progress, message);

                    throw new ApplicationException(message);
                }

                return projectInfo;
            }

            return null;
        }

        private async Task DownloadAsync()
        {
            await TaskScheduler.Default;
            DownloadStage = DownloadStageType.Download;
            await _controller.Download(ControllerOperationMode.OperationModeNull, false, false);
        }

        private async Task SynchronizingDataAsync()
        {
            await TaskScheduler.Default;

            DownloadStage = DownloadStageType.SyncData;

            int result = await _controller.RebuildTagSyncControllerAsync();

            if (result != 0)
            {
                string message = "Synchronizing Data failed!";
                await UpdateProgressAndMessageAsync(Progress, message);

                throw new ApplicationException(message);
            }
        }

        private async Task UpdateStateAsync()
        {
            await TaskScheduler.Default;
            DownloadStage = DownloadStageType.UpdateState;
            await _controller.UpdateState();
        }

        private async Task RestoreAsync(string restoreFilePath, ProjectInfo projectInfo)
        {
            await TaskScheduler.Default;

            DownloadStage = DownloadStageType.Restore;

            if (!string.IsNullOrEmpty(restoreFilePath))
            {
                try
                {
                    //1. create ProjectInfo
                    StreamReader fileStream = File.OpenText(restoreFilePath);
                    JsonSerializer serializer = new JsonSerializer();
                    projectInfo = (ProjectInfo)serializer.Deserialize(fileStream, typeof(ProjectInfo));
                }
                catch (Exception)
                {
                    string message = $"Read restore file failed!: '{restoreFilePath}'";
                    await UpdateProgressAndMessageAsync(Progress, message);
                    throw;
                }

            }

            if (projectInfo != null)
            {
                //2. restore
                RestoreHelper restoreHelper = new RestoreHelper(_controller, projectInfo);
                if (restoreHelper.Restore() < 0)
                {
                    string message = $"Restore failed!";
                    await UpdateProgressAndMessageAsync(Progress, message);
                    throw new ApplicationException(message);
                }
            }
        }
    }
}
