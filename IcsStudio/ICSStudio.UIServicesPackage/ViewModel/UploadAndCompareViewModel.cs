using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using ICSStudio.DownloadChange;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json.Linq;
using NLog;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    public class UploadAndCompareViewModel: ViewModelBase
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly Controller _controller;
        private string _message;

        private bool? _dialogResult;

        public UploadAndCompareViewModel(Controller controller)
        {
            _controller = controller;
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

        public ProjectDiffModel DiffModel { get; private set; }

        public void UploadAndCompare()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                int result;
                try
                {
                    result = 0;

                    ProjectUploader uploader = new ProjectUploader(_controller.CipMessager);

                    await uploader.UploadLock();

                    //
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    Message = "Upload controller properties...";
                    
                    await TaskScheduler.Default;
                    await uploader.UploadControllerProperties();

                    //
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    Message = "Upload controller tags...";

                    await TaskScheduler.Default;
                    await uploader.UploadControllerTags();

                    //
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    Message = "Upload controller data types...";

                    await TaskScheduler.Default;
                    await uploader.UploadDataTypes();

                    //
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    Message = "Upload controller aoi definitions...";

                    await TaskScheduler.Default;
                    await uploader.UploadAOIDefinitions();

                    //
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    Message = "Upload controller aoi tasks...";

                    await TaskScheduler.Default;
                    await uploader.UploadTasks();

                    //
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    Message = "Upload controller programs...";

                    await TaskScheduler.Default;
                    await uploader.UploadPrograms();

                    //
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    Message = "Upload controller modules...";

                    await TaskScheduler.Default;
                    await uploader.UploadModules();
                    
                    await uploader.UploadUnlock();

                    // compare
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    Message = "Comparing...";

                    await TaskScheduler.Default;
                    JObject newInfo = _controller.ConvertToJObject(false, false);
                    JObject oldInfo = uploader.ProjectInfo;

                    ProjectDiffBuilder diffBuilder = new ProjectDiffBuilder();
                    DiffModel = diffBuilder.BuildDiffModel(oldInfo, newInfo);
                }
                catch (Exception e)
                {
                    //ignore
                    Logger.Error($"upload and compare failed({Message}): {e}");

                    result = -100;
                }

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                DialogResult = result == 0;
            });
        }
    }
}
