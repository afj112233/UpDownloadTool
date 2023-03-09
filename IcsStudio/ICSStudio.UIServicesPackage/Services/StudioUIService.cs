using System;
using System.Collections.Generic;
using System.IO;
using ICSStudio.Interfaces.Notification;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIInterfaces.Editor;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.UIInterfaces.UI;
using ICSStudio.UIInterfaces.Version;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.Services
{
    public class StudioUIService : IStudioUIService, SStudioUIService
    {
        private readonly Package _package;

        public StudioUIService(Package package)
        {
            _package = package;
        }

        private IServiceProvider ServiceProvider => _package;

        public void Reset()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                OnReset?.Invoke(this, EventArgs.Empty);
            });
        }

        public event EventHandler OnReset;

        public void UpdateWindowTitle()
        {
            string title = "ICS Studio";

            var projectInfoService =
                ServiceProvider?.GetService(typeof(SProjectInfoService)) as IProjectInfoService;

            var versionService = ServiceProvider?.GetService(typeof(SVersionService)) as IVersionService;

            var project = projectInfoService?.CurrentProject;

            if (project != null && !project.IsEmpty)
            {
                string name = project.Controller.Name;
                string localeName = project.Controller.ProjectLocaleName;

                string fileName = Path.GetFileName(localeName);

                if (string.Equals(name, Path.GetFileNameWithoutExtension(fileName)))
                {
                    title += $" - {name}";
                }
                else
                {
                    title += $" - {name} in {fileName}";
                }

                // version
                string version = versionService?.GetCoreVersion();
                if (!string.IsNullOrEmpty(version))
                {
                    title += $" [{version}]";
                }

                if (project.IsDirty)
                {
                    title += "*";
                }
            }

            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var mainWindow = System.Windows.Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.Title = title;
                }

            });
        }

        public void Close()
        {
            // close open Windows and ToolWindows
            var createDialogService =
                Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;
            var createEditorService =
                Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;

            createDialogService?.CloseAllDialogs();
            createEditorService?.CloseAllToolWindows(true);

            var controller = Controller.GetInstance();
            controller.Clear();

            var projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            var project = projectInfoService?.CurrentProject;
            if (project != null)
            {
                project.Controller = null;
                project.RecentCommPath = string.Empty;
            }

            OnReset?.Invoke(this, EventArgs.Empty);
        }

        public void AttachController()
        {
            OnAttachController?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler OnAttachController;

        public void DetachController()
        {
            OnDetachController?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler OnDetachController;

        public void DeleteTag(ITag tag)
        {
            if (tag == null)
                return;

            var createDialogService =
                Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;

            //1. remove tag
            var tagCollection = tag.ParentCollection;
            tagCollection.DeleteTag(tag, true, true,tag.ParentCollection.ParentProgram is AoiDefinition);

            //2. close all tag windows
            var windowIds = GetWindowIds(tag);
            createDialogService?.CloseDialog(windowIds);

            Notifications.Publish(new MessageData() { Object = new List<ITag>() { tag }, Type = MessageData.MessageType.DelTag });
        }

        private List<string> GetWindowIds(ITag tag)
        {
            List<string> windowIds = new List<string>();

            if (tag.DataTypeInfo.DataType.IsMotionGroupType)
            {
                windowIds.Add($"MotionGroupProperties{tag.Uid}");
                windowIds.Add($"AxisSchedule{tag.Uid}");
            }
            else if (tag.DataTypeInfo.DataType.IsAxisType)
            {
                windowIds.Add($"AxisCIPDriveProperties{tag.Uid}");
                windowIds.Add($"AxisVirtualProperties{tag.Uid}");
            }

            windowIds.Add($"TagProperties{tag.Uid}");

            return windowIds;
        }
    }
}
