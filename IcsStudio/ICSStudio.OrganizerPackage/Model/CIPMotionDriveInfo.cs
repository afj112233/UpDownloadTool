using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using ICSStudio.Descriptor;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.OrganizerPackage.ViewModel;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DeviceModule;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.OrganizerPackage.Model
{
    internal class CIPMotionDriveInfo : BaseSimpleInfo
    {
        private readonly CIPMotionDrive _motionDrive;
        private readonly ObservableCollection<OrganizerItemInfo> _treeViewInfo;

        public CIPMotionDriveInfo(
            CIPMotionDrive motionDrive,
            ObservableCollection<SimpleInfo> infoSource,
            ObservableCollection<OrganizerItemInfo> treeViewInfo)
            : base(infoSource)
        {
            _motionDrive = motionDrive;
            _treeViewInfo = treeViewInfo;

            if (_motionDrive != null)
            {
                CreateInfoItems();

                CreateTreeViewInfoItems();

                PropertyChangedEventManager.AddHandler(_motionDrive,
                    OnModulePropertyChanged, string.Empty);

                Controller controller = _motionDrive?.ParentController as Controller;
                if (controller != null)
                {
                    WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                        controller, "IsOnlineChanged", OnIsOnlineChanged);
                }

            }
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(
                async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    string status = "Offline";
                    string moduleFault = string.Empty;

                    if (_motionDrive.ParentController.IsOnline)
                    {
                        ModuleDescriptor descriptor = new ModuleDescriptor(_motionDrive);
                        status = descriptor.Status;
                        moduleFault = descriptor.ModuleFault;
                    }

                    SetSimpleInfo("Status", status);
                    SetSimpleInfo("Module Fault", moduleFault);

                });
        }

        private void OnModulePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (e.PropertyName == "Description")
                {
                    SetSimpleInfo("Description", _motionDrive.Description);
                }

                if (e.PropertyName == "EntryStatus")
                {
                    ModuleDescriptor descriptor = new ModuleDescriptor(_motionDrive);
                    SetSimpleInfo("Status", descriptor.Status);
                }

                if (e.PropertyName == "FaultCode")
                {
                    ModuleDescriptor descriptor = new ModuleDescriptor(_motionDrive);
                    SetSimpleInfo("Module Fault", descriptor.ModuleFault);
                }
            });
        }

        private void CreateInfoItems()
        {
            if (InfoSource != null)
            {
                string status = "Offline";
                string moduleFault = string.Empty;

                if (_motionDrive.ParentController.IsOnline)
                {
                    ModuleDescriptor descriptor = new ModuleDescriptor(_motionDrive);
                    status = descriptor.Status;
                    moduleFault = descriptor.ModuleFault;
                }

                InfoSource.Add(new SimpleInfo { Name = "Description", Value = _motionDrive.Description });
                InfoSource.Add(new SimpleInfo { Name = "Power Structure", Value = _motionDrive.PowerStructure });
                InfoSource.Add(new SimpleInfo { Name = "Status", Value = status });
                InfoSource.Add(new SimpleInfo { Name = "Module Fault", Value = moduleFault, Key = "ModuleFault" });
            }
        }

        private void CreateTreeViewInfoItems()
        {
            _treeViewInfo.Clear();

            if (_motionDrive != null)
            {
                _treeViewInfo.Add(new OrganizerItemInfo()
                {
                    Name = "AssociatedAxes",
                    IconVisibility = Visibility.Collapsed
                });

                //给驱动器状态栏窗口显示分配的轴
                ITag[] associatedAxes = _motionDrive.GetAssociatedAxes();
                if (associatedAxes != null)
                {
                    foreach (var axis in associatedAxes)
                    {
                        if (axis == null) continue;
                        _treeViewInfo.Add(new OrganizerItemInfo()
                        {
                            DisplayName = axis.Name,
                            IconVisibility = Visibility.Visible,
                            IconKind = "TagPlus",
                            IconForeground = "#FFFFD700",
                            Space = "    "
                        });
                    }
                }
            }
            else
            {
                _treeViewInfo.Add(new OrganizerItemInfo()
                    {
                        DisplayName = "<none>",
                        IconVisibility = Visibility.Collapsed
                    });
            }
        }
    }
}
