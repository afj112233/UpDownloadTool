using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Online;
using ICSStudio.UIInterfaces.Command;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;

namespace ICSStudio.UIServicesPackage
{
    public partial class ConnectedToViewModel
    {
        private string _majorFaultResult;

        public string MajorFaultResult
        {
            get
            {
                if (_controller == null)
                    return string.Empty;

                if (!_controller.IsConnected)
                    return "Offline.";


                return _majorFaultResult;
            }
        }

        private string _recentFaults;

        public string RecentFaults
        {
            get
            {
                if (_controller == null)
                    return string.Empty;

                if (!_controller.IsConnected)
                    return string.Empty;


                return _recentFaults;
            }
        }

        public RelayCommand ClearMajorsCommand { get; }

        private bool CanExecuteClearMajors()
        {
            if (_controller != null && _controller.IsConnected)
            {
                if (_controller.OperationMode == ControllerOperationMode.OperationModeFaulted)
                    return true;
            }

            return false;
        }

        private void ExecuteClearMajors()
        {
            var commandService =
                Package.GetGlobalService(typeof(SCommandService)) as ICommandService;

            commandService?.ClearFaults(_controller);
        }

        private void StartGetMajorFaultTimer()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                try
                {
                    await TaskScheduler.Default;

                    List<MajorFaultInfo> infos = new List<MajorFaultInfo>();
                    int result = await _majorFaultHelper.GetAllMajorFaultInfos(infos);


                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    if (result == 0)
                        AddMajorFaultInfos(infos);

                    Refresh();

                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception);
                }
                finally
                {
                    if (_controller.IsConnected)
                    {
                        _getMajorFaultTimer?.Start();
                    }
                }

            });
        }

        private void AddMajorFaultInfos(List<MajorFaultInfo> infos)
        {
            if (infos != null && infos.Count > 0)
            {
                foreach (var info in infos)
                {
                    _majorFaultInfos.Enqueue(info);
                }

                var faultInfos = _majorFaultInfos.ToArray();

                _majorFaultResult = "English".Equals(LanguageInfo.CurrentLanguage)
                    ? $"{faultInfos.Length} major fault since last cleared."
                    : $"上次清除后出现的严重故障{faultInfos.Length} 。";

                StringBuilder builder = new StringBuilder();

                foreach (var info in faultInfos)
                {
                    builder.AppendLine(info.ToString());
                }

                _recentFaults = builder.ToString();
            }
        }
    }
}
