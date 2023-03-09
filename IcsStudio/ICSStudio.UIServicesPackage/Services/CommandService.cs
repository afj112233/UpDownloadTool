using ICSStudio.UIInterfaces.Command;
using Microsoft.VisualStudio;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using ICSStudio.Cip.Objects;
using ICSStudio.Dialogs;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.UIInterfaces.Editor;
using ICSStudio.UIInterfaces.Error;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.UIInterfaces.QuickWatch;
using ICSStudio.UIInterfaces.Search;
using ICSStudio.UIInterfaces.UI;
using ICSStudio.UIInterfaces.Version;
using ICSStudio.UIServicesPackage.View;
using ICSStudio.UIServicesPackage.ViewModel;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using NLog;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using Task = System.Threading.Tasks.Task;
using ICSStudio.UIServicesPackage.DownloadCompareTool.View;
using ICSStudio.UIServicesPackage.DownloadCompareTool.ViewModel;

namespace ICSStudio.UIServicesPackage.Services
{
    [SuppressMessage("ReSharper", "UsePatternMatching")]
    public class CommandService : ICommandService, SCommandService
    {
        internal static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly Package _package;

        public CommandService(Package package)
        {
            _package = package;
            LanguageManager.Instance.LanguageChanged += Instance_LanguageChanged;
            Controller.GetInstance().IsOnlineChanged += CommandService_IsOnlineChanged;
        }

        private void CommandService_IsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(
                async delegate
                {
                    var controller = (Controller) sender;
                    if (controller.IsOnline)
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        UpdateUI();
                    }
                });
        }

        private void Instance_LanguageChanged(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            UpdateUI();
        }

        private IServiceProvider ServiceProvider => _package;

        public void GoOnlineOrOffline(IController controller, string commPath)
        {
            var createEditorService = Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
            if (createEditorService != null)
                if (createEditorService.IsThereATrendRunning())
                {
                    var message =
                        "English".Equals(LanguageInfo.CurrentLanguage)
                            ? "One or more trends are collecting data samples. Going offline will stop " +
                              "the trend operation(s), and cause trend data to be lost." +
                              "\n\n\nContinue going offline?"
                            : "一个或多个 Trend 正在收集数据样本，脱机会停止 Trend 操作，并导致Trend数据丢失\n\n继续脱机？";
                    var result = MessageBox.Show(message, "ICS Studio", MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);
                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }

            if (CheckCommunicationPath(commPath))
            {
                ThreadHelper.JoinableTaskFactory.RunAsync(
                    async delegate
                    {
                        if (controller.IsOnline)
                        {
                            GoOffline(controller);
                        }
                        else
                        {
                            await GoOnlineAsync(controller, commPath);
                        }

                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        UpdateUI();
                    });
            }
        }

        public void Upload(IController controller, string commPath)
        {
            if (CheckCommunicationPath(commPath))
            {
                ThreadHelper.JoinableTaskFactory.RunAsync(async delegate { await UploadAsync(controller, commPath); });
            }
        }

        public void Download(IController controller, string commPath)
        {
            if (CheckCommunicationPath(commPath))
            {
                ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
                {
                    await DownloadAsync(controller, commPath);
                });
            }
        }

        public void DownloadSync(IController controller, string commPath)
        {
            if (CheckCommunicationPath(commPath))
            {
                ThreadHelper.JoinableTaskFactory.Run(async delegate { await DownloadAsync(controller, commPath); });
            }
        }

        public void UploadSync(IController controller, string commPath)
        {
            if (CheckCommunicationPath(commPath))
            {
                ThreadHelper.JoinableTaskFactory.Run(async delegate { await UploadAsync(controller, commPath); });
            }
        }

        public void CorrelateSync(IController input)
        {
            ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await TaskScheduler.Default;

                Controller controller = input as Controller;
                if (controller == null)
                    return;

                int result = -1000;

                // correlate
                try
                {
                    var transactionManager =
                        controller.Lookup(typeof(TransactionManager)) as
                            TransactionManager;

                    if (transactionManager != null)
                    {
                        CIPController cipController = new CIPController(0, controller.CipMessager);

                        result = await cipController.WriterLockRetry();
                        if (result == 0)
                        {
                            result = await transactionManager.Correlate(controller.CipMessager);
                            await cipController.WriterUnLock();
                        }
                    }

                }
                catch (Exception)
                {
                    //ignore
                    result = -100;
                }

                // online
                if (result == 0)
                {
                    // 5. onlining
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    OnliningDialog onliningDialog = new OnliningDialog(controller)
                    {
                        Owner = Application.Current.MainWindow
                    };

                    var dialogResult = onliningDialog.ShowDialog();
                    if (!(dialogResult.HasValue && dialogResult.Value))
                    {
                        result = -200;
                    }
                }

                if (result != 0)
                {
                    GoOffline(controller);

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    MessageBox.Show(
                        LanguageManager.GetInstance().ConvertSpecifier("Go online failed!"), "ICS Studio",
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Error);
                }

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                UpdateUI();
            });
        }

        public void SaveToController(IController input)
        {
            Controller controller = input as Controller;
            if (controller == null)
                return;

            if (!controller.IsOnline)
                return;

            ThreadHelper.JoinableTaskFactory.RunAsync(
                async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    SavingToControllerDialog dialog = new SavingToControllerDialog(controller);
                    dialog.Owner = Application.Current.MainWindow;

                    dialog.ShowDialog();

                });

        }

        public void ChangeOperationMode(IController input, ControllerOperationMode mode)
        {
            Controller controller = input as Controller;
            if (controller == null)
                return;

            string modeType = string.Empty;
            switch (mode)
            {
                case ControllerOperationMode.OperationModeProgram:
                    modeType = "Program";
                    break;
                case ControllerOperationMode.OperationModeRun:
                    modeType = "Run";
                    break;
                case ControllerOperationMode.OperationModeDebug:
                    modeType = "Test";
                    break;
            }

            if (MessageBox.Show(
                    "English".Equals(LanguageInfo.CurrentLanguage)
                        ? $"Change controller mode to {modeType}?"
                        : $"将控制器模式更改为 {modeType}?",
                    "ICS Studio",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {

                ThreadHelper.JoinableTaskFactory.RunAsync(
                    async delegate
                    {
                        await controller.ChangeOperationMode(mode);

                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        UpdateUI();
                    });
            }
        }

        public void ClearFaults(IController input)
        {
            Controller controller = input as Controller;
            if (controller == null)
                return;

            ThreadHelper.JoinableTaskFactory.RunAsync(
                async delegate
                {
                    await controller.ClearFaults();

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    UpdateUI();
                });
        }

        public bool IsReference(ITag tag)
        {
            if (tag == null) return false;
            if (tag.ParentCollection.ParentProgram is AoiDefinition)
            {
                var program = tag.ParentCollection.ParentProgram as AoiDefinition;
                foreach (var routine in program.Routines)
                {
                    var stRoutine = routine as STRoutine;
                    if (stRoutine != null)
                    {
                        var t = stRoutine.GetAllReferenceTags().AsParallel().FirstOrDefault(v => v == tag);
                        if (t != null) return true;
                    }
                    //TODO(zyl):check other routine
                }
            }
            else
            {
                foreach (var program in Controller.GetInstance().Programs)
                {
                    foreach (var routine in program.Routines)
                    {
                        var stRoutine = routine as STRoutine;
                        if (stRoutine != null)
                        {
                            var t = stRoutine.GetAllReferenceTags().AsParallel().FirstOrDefault(v => v == tag);
                            if (t != null) return true;
                        }
                    }
                }
            }

            return false;
        }


        #region Private

        private bool CheckCommunicationPath(string commPath)
        {
            if (string.IsNullOrEmpty(commPath))
            {
                WarningDialog dialog = new WarningDialog(
                        LanguageManager.GetInstance().ConvertSpecifier("failed to go online with the controller."),
                        LanguageManager.GetInstance().ConvertSpecifier("Communications path needed."),
                        LanguageManager.GetInstance().ConvertSpecifier("Error 701-8004280D"))
                    { Owner = Application.Current.MainWindow };
                dialog.ShowDialog();

                return false;
            }

            return true;
        }

        private async Task GoOnlineAsync(IController input, string commPath)
        {
            Controller controller = input as Controller;
            if (controller == null)
                return;

            var projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            var versionService = Package.GetGlobalService(typeof(SVersionService)) as IVersionService;

            // 0. apply
            var project = projectInfoService?.CurrentProject;
            if (project != null && project.IsDirty)
            {
                int saveResult = projectInfoService.Save(false);
                if (saveResult < 0)
                {
                    return;
                }
            }

            // 1. connect
            int connectingResult = await ConnectingAsync(controller, commPath);
            if (connectingResult < 0)
                return;

            // 2.1 check controller type
            int result = await CheckControllerTypeAsync(controller);
            if (result < 0)
            {
                GoOffline(controller);

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                string message = LanguageManager.GetInstance()
                    .ConvertSpecifier("Failed to go online with the controller!") + "\n\n";
                message += LanguageManager.GetInstance()
                    .ConvertSpecifier(
                        "The actual controller type does not match the controller type for this project.");

                MessageBox.Show(message
                    , "ICS Studio",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Error);

                return;
            }

            // 2.2 check version
            var versionCheckResult = versionService?.CheckControllerVersion(controller);
            if (versionCheckResult == null || versionCheckResult.Kind != 0)
            {
                GoOffline(controller);

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                string message = LanguageManager.GetInstance()
                    .ConvertSpecifier("Failed to go online with the controller!") + "\n\n";
                message += "Check version Failed!\n\n";

                if (versionCheckResult != null)
                {
                    message += versionCheckResult.Message;
                }

                MessageBox.Show(message
                    , "ICS Studio",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Error);

                return;
            }

            // 3. get state
            await TaskScheduler.Default;
            await controller.UpdateState();

            // 4. compare
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            ComparingDialog comparingDialog = new ComparingDialog(controller)
            {
                Owner = Application.Current.MainWindow
            };

            var dialogResult = comparingDialog.ShowDialog();
            var viewModel = comparingDialog.ViewModel;

            if (dialogResult.HasValue && dialogResult.Value)
            {
                // 5. onlining
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                OnliningDialog onliningDialog = new OnliningDialog(controller)
                {
                    Owner = Application.Current.MainWindow
                };

                dialogResult = onliningDialog.ShowDialog();
                if (!(dialogResult.HasValue && dialogResult.Value))
                {
                    GoOffline(controller);

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    MessageBox.Show(
                        LanguageManager.GetInstance().ConvertSpecifier("Go online failed!"), "ICS Studio",
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Error);
                }
            }
            else if (dialogResult.HasValue && viewModel.CompareResult != CompareResult.Exception)
            {
                // 6. connected to
                ConnectedToViewModel connectedToViewModel
                    = new ConnectedToViewModel(controller, ConnectedToType.Login, viewModel.CompareResult);
                ConnectedToDialog connectedToDialog = new ConnectedToDialog()
                {
                    DataContext = connectedToViewModel,
                    Owner = Application.Current.MainWindow
                };

                dialogResult = connectedToDialog.ShowDialog();
                if (!(dialogResult.HasValue && dialogResult.Value))
                {
                    GoOffline(controller);
                }
            }
            else
            {
                GoOffline(controller);

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                MessageBox.Show(
                    LanguageManager.GetInstance().ConvertSpecifier("Go online failed!"), "ICS Studio",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Error);
            }

            // 7. update ui
            UpdateUI();
        }

        private async Task UploadAsync(IController input, string commPath)
        {
            Controller controller = input as Controller;
            if (controller == null)
                return;

            controller.IsLoading = true;

            var projectInfoService =
                ServiceProvider?.GetService(typeof(SProjectInfoService)) as IProjectInfoService;
            var quickWatchService =
                Package.GetGlobalService(typeof(SQuickWatchService)) as IQuickWatchService;
            var versionService = Package.GetGlobalService(typeof(SVersionService)) as IVersionService;

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            quickWatchService?.LockQuickWatch();

            // apply
            var project = projectInfoService?.CurrentProject;
            if (project != null && project.IsDirty)
            {
                int saveResult = projectInfoService.Save(false);
                if (saveResult < 0)
                {
                    controller.IsLoading = false;
                    return;
                }
            }

            // 1. connect
            int connectingResult = await ConnectingAsync(controller, commPath);
            if (connectingResult < 0)
            {
                controller.IsLoading = false;
                return;
            }

            CIPController cipController = new CIPController(0, controller.CipMessager);
            var res = await cipController.GetHasProject();
            if (res < 1)
            {
                MessageBox.Show(
                    "English".Equals(LanguageInfo.CurrentLanguage)
                        ? "There is no project in the controller for ICS Studio to upload."
                        : "控制器中没有可供 ICSStudio 上传的项目", "ICS Studio",
                    MessageBoxButton.OK);

                GoOffline(controller);
                controller.IsLoading = false;
                return;
            }

            // 1.2 check version
            var versionCheckResult = versionService?.CheckControllerVersion(controller);
            if (versionCheckResult == null || versionCheckResult.Kind != 0)
            {
                GoOffline(controller);
                controller.IsLoading = false;

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                string message = LanguageManager.GetInstance()
                    .ConvertSpecifier("Failed to go online with the controller!") + "\n\n";
                message += "Check version Failed!\n\n";

                if (versionCheckResult != null)
                {
                    message += versionCheckResult.Message;
                }

                MessageBox.Show(message
                    , "ICS Studio",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Error);

                return;
            }

            // 2. get state
            await TaskScheduler.Default;
            await controller.UpdateState();

            // 3. file name
            string controllerName = await cipController.GetName();

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            SaveFileDialog saveDlg = new SaveFileDialog()
            {
                FileName = controllerName,
                Title = LanguageManager.GetInstance().ConvertSpecifier("SaveFile"),
                Filter = "json文件(*.json)|*.json"
            };

            if (saveDlg.ShowDialog() != DialogResult.OK)
            {
                GoOffline(controller);
                controller.IsLoading = false;
                return;
            }

            string fileName = saveDlg.FileName;

            // 4. close all windows
            var errorsOutputService = Package.GetGlobalService(typeof(SErrorOutputService)) as IErrorOutputService;
            errorsOutputService?.Cleanup();
            var searchResultService = Package.GetGlobalService(typeof(SSearchResultService)) as ISearchResultService;
            searchResultService?.Clean();
            var studioUIService =
                Package.GetGlobalService(typeof(SStudioUIService)) as IStudioUIService;

            studioUIService?.Close();

            // 5. uploading
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            UploadingDialog uploadingDialog = new UploadingDialog(controller, fileName)
            {
                Owner = Application.Current.MainWindow
            };
            var dialogResult = uploadingDialog.ShowDialog();

            if (!(dialogResult.HasValue && dialogResult.Value))
            {
                //TODO(gjc): need check later
                GoOffline(controller);

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                MessageBox.Show(
                    LanguageManager.GetInstance().ConvertSpecifier("Upload failed!"), "ICS Studio",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Error);
            }

            // 6. verify
            if (project != null)
            {
                project.Controller = controller;
                project.RecentCommPath = commPath;
            }

            controller.IsLoading = true;
            projectInfoService?.VerifyInDialog();

            // 7. update ui
            studioUIService?.Reset();
            studioUIService?.UpdateWindowTitle();
            errorsOutputService?.Cleanup();
            searchResultService?.Clean();
            quickWatchService?.Reset();
            controller.IsLoading = false;
        }

        private async Task DownloadAsync(IController input, string commPath)
        {
            Controller controller = input as Controller;
            if (controller == null)
                return;

            controller.IsDownloading = true;

            var projectInfoService = Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            var outputService = Package.GetGlobalService(typeof(SErrorOutputService)) as IErrorOutputService;
            var versionService = Package.GetGlobalService(typeof(SVersionService)) as IVersionService;

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            // apply
            var project = projectInfoService?.CurrentProject;
            if (project != null && project.IsDirty)
            {
                int saveResult = projectInfoService.Save(false);
                if (saveResult < 0)
                {
                    controller.IsDownloading = false;
                    return;
                }
            }

            // verify
            projectInfoService?.VerifyInDialog();

            if (!controller.IsPass)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                MessageBox.Show(
                    LanguageManager.GetInstance().ConvertSpecifier("Download failed! Clear error first!"),
                    "ICS Studio");

                controller.IsDownloading = false;
                return;
            }

            await TaskScheduler.Default;

            // 1. connect
            int result = await ConnectingAsync(controller, commPath);
            if (result < 0)
            {
                controller.IsDownloading = false;
                return;
            }

            // 2.1 check controller type
            result = await CheckControllerTypeAsync(controller);
            if (result < 0)
            {
                controller.IsDownloading = false;
                GoOffline(controller);

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                string message = LanguageManager.GetInstance()
                    .ConvertSpecifier("Failed to go online with the controller!") + "\n\n";
                message += LanguageManager.GetInstance()
                    .ConvertSpecifier(
                        "The actual controller type does not match the controller type for this project.");

                MessageBox.Show(message
                    , "ICS Studio",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Error);

                return;
            }

            // 2.2 check version
            var versionCheckResult = versionService?.CheckControllerVersion(controller);
            if (versionCheckResult == null || versionCheckResult.Kind != 0)
            {
                controller.IsDownloading = false;
                GoOffline(controller);

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                string message = LanguageManager.GetInstance()
                    .ConvertSpecifier("Failed to go online with the controller!") + "\n\n";
                message += "Check version Failed!\n\n";

                if (versionCheckResult != null)
                {
                    message += versionCheckResult.Message;
                }

                MessageBox.Show(message
                    , "ICS Studio",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Error);

                return;
            }

            // 2. get state
            await controller.UpdateState();

            var keySwitchPosition = controller.KeySwitchPosition;
            var operationMode = controller.OperationMode;

            Debug.WriteLine($"KeySwitch:{keySwitchPosition}, OperationMode:{operationMode}!");

            if (operationMode == ControllerOperationMode.OperationModeFaulted)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                MessageBox.Show(
                    LanguageManager.GetInstance()
                        .ConvertSpecifier("Can't download to faulted controller. Clear faults and try again."),
                    "ICS Studio",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Error);

                GoOffline(controller);
                controller.IsDownloading = false;
            }
            else
            {
                //TODO(gjc): add controller info here
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                DownloadDialog downloadDialog = new DownloadDialog();
                DownloadViewModel viewModel =
                    new DownloadViewModel(controller);

                downloadDialog.DataContext = viewModel;
                downloadDialog.Owner = Application.Current.MainWindow;

                var dialogResult = downloadDialog.ShowDialog();

                if (!(dialogResult.HasValue && dialogResult.Value))
                {
                    GoOffline(controller);

                    controller.IsDownloading = false;
                    return;
                }

                if (viewModel.IsOnlineChange)
                {
                    // download change
                    //1. upload project and compare
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    UploadAndCompareDialog uploadAndCompareDialog = new UploadAndCompareDialog(controller)
                    {
                        Owner = Application.Current.MainWindow
                    };
                    dialogResult = uploadAndCompareDialog.ShowDialog();
                    if (!(dialogResult.HasValue && dialogResult.Value))
                    {
                        GoOffline(controller);

                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        MessageBox.Show(
                            LanguageManager.GetInstance().ConvertSpecifier("Compare failed!"), "ICS Studio",
                            MessageBoxButton.OKCancel,
                            MessageBoxImage.Error);

                        controller.IsDownloading = false;
                        return;
                    }
                    else
                    {
                        //2. show compare result
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        CompareResultViewModel compareResultViewModel =
                            new CompareResultViewModel(controller, uploadAndCompareDialog.ViewModel.DiffModel);
                        CompareResultView compareResultView = new CompareResultView()
                        {
                            Owner = Application.Current.MainWindow
                        };
                        compareResultView.DataContext = compareResultViewModel;

                        dialogResult = compareResultView.ShowDialog();

                        if (!(dialogResult.HasValue && dialogResult.Value))
                        {
                            // cancel download change

                            GoOffline(controller);

                            controller.IsDownloading = false;
                            return;
                        }

                        //3. download change
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        DownloadChangeDialog downloadChangeDialog =
                            new DownloadChangeDialog(
                                controller,
                                uploadAndCompareDialog.ViewModel.DiffModel,
                                compareResultViewModel.IsDownloadAxisParameters);
                        downloadChangeDialog.Owner = Application.Current.MainWindow;
                        dialogResult = downloadChangeDialog.ShowDialog();

                        if (!(dialogResult.HasValue && dialogResult.Value))
                        {
                            GoOffline(controller);

                            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                            MessageBox.Show(
                                LanguageManager.GetInstance().ConvertSpecifier("Download change failed!"), "ICS Studio",
                                MessageBoxButton.OKCancel,
                                MessageBoxImage.Error);

                            controller.IsDownloading = false;
                            return;
                        }
                    }
                }
                else if (viewModel.IsDownload)
                {
                    // normal download
                    // 4. change state
                    if (keySwitchPosition == ControllerKeySwitch.RemoteKeySwitch &&
                        operationMode == ControllerOperationMode.OperationModeRun)
                    {
                        await TaskScheduler.Default;
                        await controller.ChangeOperationMode(ControllerOperationMode.OperationModeProgram);
                    }

                    string preserveFilePath = string.Empty;
                    string restoreFilePath = string.Empty;

                    if (viewModel.IsPreserve)
                    {
                        //临时路径
                        preserveFilePath = Path.GetTempFileName() + ".json";

                        if (viewModel.IsBackUp)
                        {
                            preserveFilePath = viewModel.PreserveFilePath;
                        }
                    }

                    if (viewModel.IsRestore)
                    {
                        restoreFilePath = viewModel.RestoreFilePath;
                    }

                    // 5. downloading
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    DownloadingDialog downloadingDialog =
                        new DownloadingDialog(controller, preserveFilePath, restoreFilePath)
                        {
                            Owner = Application.Current.MainWindow
                        };
                    dialogResult = downloadingDialog.ShowDialog();

                    if (!(dialogResult.HasValue && dialogResult.Value))
                    {
                        GoOffline(controller);

                        await ShowDownloadFailedInfoAsync(
                            controller,
                            downloadingDialog.ViewModel,
                            preserveFilePath, restoreFilePath);

                        controller.IsDownloading = false;
                        return;
                    }

                    outputService?.Summary();
                    // 6. change back state
                    if (keySwitchPosition == ControllerKeySwitch.RemoteKeySwitch &&
                        operationMode == ControllerOperationMode.OperationModeRun)
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        Window owner = Application.Current.MainWindow;

                        string message = LanguageManager.GetInstance()
                            .ConvertSpecifier("Done downloading. Change controller mode back to Remote Run?");
                        if ("English".Equals(LanguageInfo.CurrentLanguage))
                        {
                            message = LanguageManager.GetInstance()
                                .ConvertSpecifier("Done downloading. Change controller mode back to Remote Run?");
                        }

                        string caption = "ICS Studio";

                        MessageBoxResult messageBoxResult;

                        if (owner != null)
                        {
                            messageBoxResult = MessageBox.Show(
                                owner, message, caption,
                                MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        }
                        else
                        {
                            messageBoxResult = MessageBox.Show(
                                message, caption,
                                MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        }


                        if (messageBoxResult == MessageBoxResult.Yes)
                        {
                            await TaskScheduler.Default;
                            await controller.ChangeOperationMode(ControllerOperationMode.OperationModeRun);
                        }

                    }
                }
            }

            controller.IsDownloading = false;
        }

        private async Task ShowDownloadFailedInfoAsync(
            Controller controller,
            DownloadingViewModel viewModel,
            string preserveFilePath, string restoreFilePath)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            string message = "Download failed!";
            string caption = "ICS Studio";

            switch (viewModel.DownloadStage)
            {
                case DownloadStageType.Begin:
                    break;
                case DownloadStageType.Build:
                    message += $"\r\nCompile project '{controller.Name}' failed!";
                    break;
                case DownloadStageType.ChangeProgramMode:
                    message += "\r\nCan't change controller into program mode!";
                    message += "\r\nPlease download later!";
                    break;
                case DownloadStageType.SaveToController:
                    message += "\r\nCan't save to controller!";
                    message += "\r\nPlease download later!";
                    break;
                case DownloadStageType.Preserve:
                    message += "\r\nPreserve online tag values failed!";
                    message += "\r\nPlease download again!";
                    break;

                case DownloadStageType.Download:
                case DownloadStageType.SyncData:
                case DownloadStageType.UpdateState:
                    if (string.IsNullOrEmpty(preserveFilePath))
                    {
                        message += $"\r\nDownload project '{controller.Name}' failed!";
                        message += "\r\nPlease download again!";
                    }
                    else
                    {
                        message += $"\r\nDownload project '{controller.Name}' failed!";
                        message += "\r\nPlease download again with restore!";
                        message += "\r\nRestore file is";
                        message += $"\r\n{preserveFilePath}";
                    }

                    break;

                case DownloadStageType.Restore:
                    if (!string.IsNullOrEmpty(restoreFilePath))
                    {
                        message += "\r\nRestore online tag values failed!";
                        message += "\r\nPlease download again with restore!";
                        message += "\r\nRestore file is";
                        message += $"\r\n{restoreFilePath}";
                    }
                    else if (!string.IsNullOrEmpty(preserveFilePath))
                    {
                        message += "\r\nRestore online tag values failed!";
                        message += "\r\nPlease download again with restore!";
                        message += "\r\nRestore file is";
                        message += $"\r\n{preserveFilePath}";
                    }
                    else
                    {
                        Logger.Info("Not run here in restore failed!");
                    }

                    break;
                case DownloadStageType.End:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CopyableMessageBox.Show(message, caption);
        }

        private async Task<int> CheckControllerTypeAsync(Controller controller)
        {
            LocalModule localModule = controller.DeviceModules["Local"] as LocalModule;
            if (localModule == null)
                return -1;

            CIPIdentity cipIdentity = new CIPIdentity(1, controller.CipMessager);

            var result = await cipIdentity.GetAttributesAll();
            if (result == 0)
            {
                ushort vendorId = Convert.ToUInt16(cipIdentity.VendorID);
                ushort deviceType = Convert.ToUInt16(cipIdentity.DeviceType);
                ushort productCode = Convert.ToUInt16(cipIdentity.ProductCode);

                //TODO(gjc): check later
                if (vendorId == 1447
                    && deviceType == 14 &&
                    localModule.ProductCode == productCode)
                    return 0;
                Logger.Error(
                    $"Check Controller type failed(Vendor:{vendorId}----deviceType{deviceType}-------productCode{productCode}---local{localModule.ProductCode})");
            }

            return -1;
        }

        private bool CheckControllerError(Controller controller)
        {
            foreach (var controllerProgram in controller.Programs)
            {
                foreach (var routine in controllerProgram.Routines)
                {
                    if (routine.IsError)
                        return true;
                }
            }

            foreach (var aoi in controller.AOIDefinitionCollection)
            {

                foreach (var routine in aoi.Routines)
                {
                    if (routine.IsError)
                        return true;

                }
            }

            return false;
        }

        private async Task<int> ConnectingAsync(Controller controller, string commPath)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            ConnectingDialog connectingDialog = new ConnectingDialog(controller, commPath)
            {
                Owner = Application.Current.MainWindow
            };

            var dialogResult = connectingDialog.ShowDialog();

            if (!(dialogResult.HasValue && dialogResult.Value))
            {
                if (!connectingDialog.ViewModel.IsUserCancel)
                {
                    string warningMessage = LanguageManager.GetInstance()
                        .ConvertSpecifier("Failed to go online with the controller.");
                    string warningReason = LanguageManager.GetInstance().ConvertSpecifier("Communications timed out.");

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    var warningDialog =
                        new WarningDialog(warningMessage, warningReason)
                            { Owner = Application.Current.MainWindow };
                    warningDialog.ShowDialog();
                }

                return -1;
            }

            return 0;
        }

        //private void CloseAllWindows()
        //{
        //    ICreateDialogService createDialogService =
        //        ServiceProvider.GetService(typeof(SCreateDialogService)) as ICreateDialogService;
        //    createDialogService?.CloseAllDialogs();

        //    var createEditorService =
        //        ServiceProvider.GetService(typeof(SCreateEditorService)) as ICreateEditorService;
        //    createEditorService?.CloseAllToolWindows(true);
        //}

        private void UpdateUI()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            IVsUIShell vsShell =
                ServiceProvider.GetService(typeof(SVsUIShell)) as IVsUIShell;

            if (vsShell != null)
            {
                int hr = vsShell.UpdateCommandUI(0);
                ErrorHandler.ThrowOnFailure(hr);
            }
        }

        private void GoOffline(IController controller)
        {
            var projectInfoService =
                ServiceProvider?.GetService(typeof(SProjectInfoService)) as IProjectInfoService;
            var project = projectInfoService?.CurrentProject;

            if (project?.Controller != null)
                project.GoOffline();
            else
                controller?.GoOffline();
        }

        #endregion
    }
}
