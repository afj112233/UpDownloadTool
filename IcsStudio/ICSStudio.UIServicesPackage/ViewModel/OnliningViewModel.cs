using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Editor;
using ICSStudio.UIInterfaces.Project;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using NLog;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    public class OnliningViewModel : ViewModelBase
    {
        internal static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly Controller _controller;

        private string _message;

        private bool? _dialogResult;

        public OnliningViewModel(Controller controller)
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

        public void GoOnline()
        {

            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                int result;
                try
                {
                    var editorService = Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
                    var projectInfoService =
                        Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;

                    var transactionManager =
                        _controller.Lookup(typeof(SimpleServices.TransactionManager)) as
                            SimpleServices.TransactionManager;

                    if (transactionManager?.ReplacedRoutines != null
                        && transactionManager.ReplacedRoutines.Count > 0)
                    {
                        foreach (var tuple in transactionManager.ReplacedRoutines)
                        {
                            var program = _controller.Programs[tuple.Item1];
                            var routine = program.Routines[tuple.Item2];

                            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                            Message = $"Verify routine '{tuple.Item2}'";

                            await TaskScheduler.Default;

                            //TODO(gjc): need edit later
                            STRoutine stRoutine = routine as STRoutine;
                            if (stRoutine != null)
                            {
                                var editorWindow = editorService?.GetWindow(stRoutine.Uid);
                                if (editorWindow != null)
                                {
                                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                                    stRoutine.IsUpdateChanged = true;
                                }
                                else
                                {
                                    await TaskScheduler.Default;

                                    stRoutine.IsError = true;
                                    projectInfoService?.Verify(stRoutine);
                                }

                            }
                            else
                            {
                                //service?.ParseRoutine(routine, true, true, true);
                            }
                            //end

                        }
                    }

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    Message = $"Synchronizing Data for project '{_controller.Name}'";

                    await TaskScheduler.Default;
                    result = await _controller.RebuildTagSyncControllerAsync();

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    Message = $"Update controller state";

                    await TaskScheduler.Default;
                    await _controller.UpdateState();
                }
                catch (Exception e)
                {
                    Logger.Error($"Onlining failed:{e}");
                    //ignore
                    result = -100;
                }

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (result < 0)
                {
                    Logger.Error($"Onlining failed in tag sync:{result}");
                }

                DialogResult = result == 0;
            });

        }
    }
}
