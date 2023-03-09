using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.OrganizerPackage.Model
{
    internal class TaskInfo : BaseSimpleInfo
    {
        private readonly CTask _task;

        public TaskInfo(CTask task, ObservableCollection<SimpleInfo> infoSource)
            : base(infoSource)
        {
            _task = task;

            CreateInfoItems();

            PropertyChangedEventManager.AddHandler(_task,
                OnTaskPropertyChanged, string.Empty);

            Controller controller = _task?.ParentController as Controller;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                string status;
                string maxScan = string.Empty;
                string lastScan = string.Empty;

                if (_task.ParentController.IsOnline)
                {
                    status = _task.IsInhibited ? "Inhibited" : "Enabled";
                    maxScan = $"{_task.MaxScanTime / 1000f:0.000} ms";
                    lastScan = $"{_task.LastScanTime / 1000f:0.000} ms";
                }
                else
                {
                    status = _task.IsInhibited ? "Inhibited" : "";
                }

                SetSimpleInfo("Status", status);
                SetSimpleInfo("Max Scan", maxScan);
                SetSimpleInfo("Last Scan", lastScan);
            });
        }

        private void OnTaskPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (e.PropertyName == "Type")
                {
                    CreateInfoItems();
                }

                if (e.PropertyName == "Description")
                {
                    SetSimpleInfo("Description", _task.Description);
                }

                if (e.PropertyName == "IsInhibited")
                {
                    string status;
                    if (_task.ParentController.IsOnline)
                    {
                        status = _task.IsInhibited ? "Inhibited" : "Enabled";
                    }
                    else
                    {
                        status = _task.IsInhibited ? "Inhibited" : "";
                    }

                    SetSimpleInfo("Status", status);
                }

                if (e.PropertyName == "Rate")
                {
                    SetSimpleInfo("Period", $"{_task.Rate:0.000} ms");
                }

                if (e.PropertyName == "Priority")
                {
                    SetSimpleInfo("Priority", _task.Priority.ToString());
                }

                if (e.PropertyName == "MaxScanTime")
                {
                    string maxScan = string.Empty;

                    if (_task.ParentController.IsOnline)
                    {
                        maxScan = $"{_task.MaxScanTime / 1000f:0.000} ms";
                    }

                    SetSimpleInfo("Max Scan", maxScan);
                }

                if (e.PropertyName == "LastScanTime")
                {
                    string lastScan = string.Empty;

                    if (_task.ParentController.IsOnline)
                    {
                        lastScan = $"{_task.LastScanTime / 1000f:0.000} ms";
                    }

                    SetSimpleInfo("Last Scan", lastScan);
                }

                if (e.PropertyName == "Trigger")
                {
                    //TODO(gjc): add for Trigger
                }
            });

        }

        private void CreateInfoItems()
        {
            if (InfoSource != null && _task != null)
            {
                string status;
                string maxScan = string.Empty;
                string lastScan = string.Empty;

                if (_task.ParentController.IsOnline)
                {
                    status = _task.IsInhibited ? "Inhibited" : "Enabled";
                    maxScan = $"{_task.MaxScanTime / 1000f:0.000} ms";
                    lastScan = $"{_task.LastScanTime / 1000f:0.000} ms";
                }
                else
                {
                    status = _task.IsInhibited ? "Inhibited" : "";
                }

                InfoSource.Add(new SimpleInfo { Name = "Type", Value = EnumHelper.GetEnumMember(_task.Type) });
                InfoSource.Add(new SimpleInfo { Name = "Description", Value = _task.Description });
                InfoSource.Add(new SimpleInfo { Name = "Status", Value = status });

                switch (_task.Type)
                {
                    case TaskType.NullType:
                        break;
                    case TaskType.Event:
                        InfoSource.Add(new SimpleInfo { Name = "Trigger", Value = "" });
                        InfoSource.Add(new SimpleInfo { Name = "Priority", Value = _task.Priority.ToString() });
                        break;
                    case TaskType.Periodic:
                        InfoSource.Add(new SimpleInfo { Name = "Period", Value = $"{_task.Rate:0.000} ms" });
                        InfoSource.Add(new SimpleInfo { Name = "Priority", Value = _task.Priority.ToString() });
                        break;
                    case TaskType.Continuous:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                InfoSource.Add(new SimpleInfo { Name = "Max Scan", Value = maxScan });
                InfoSource.Add(new SimpleInfo { Name = "Last Scan", Value = lastScan });
            }

        }
    }
}
