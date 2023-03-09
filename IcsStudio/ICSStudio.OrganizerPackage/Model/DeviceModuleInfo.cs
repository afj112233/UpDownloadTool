using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using ICSStudio.Descriptor;
using ICSStudio.Interfaces.Common;
using ICSStudio.OrganizerPackage.ViewModel;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DeviceModule;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.OrganizerPackage.Model
{
    internal class DeviceModuleInfo : BaseSimpleInfo
    {
        private readonly DeviceModule _deviceModule;
        private readonly ObservableCollection<OrganizerItemInfo> _treeViewInfo;

        public DeviceModuleInfo(
            DeviceModule deviceModule,
            ObservableCollection<SimpleInfo> infoSource,
            ObservableCollection<OrganizerItemInfo> treeViewInfo)
            : base(infoSource)
        {
            _deviceModule = deviceModule;
            _treeViewInfo = treeViewInfo;

            if (_deviceModule != null)
            {
                CreateInfoItems();

                CreateTreeViewInfoItems();

                PropertyChangedEventManager.AddHandler(_deviceModule,
                    OnModulePropertyChanged, string.Empty);

                Controller controller = _deviceModule?.ParentController as Controller;
                if (controller != null)
                {
                    WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                        controller, "IsOnlineChanged", OnIsOnlineChanged);
                }

            }
        }

        private void OnModulePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (e.PropertyName == "Description")
                {
                    SetSimpleInfo("Description", _deviceModule.Description);
                }

                if (e.PropertyName == "EntryStatus")
                {
                    ModuleDescriptor descriptor = new ModuleDescriptor(_deviceModule);
                    SetSimpleInfo("Status", descriptor.Status);
                }

                if (e.PropertyName == "FaultCode")
                {
                    ModuleDescriptor descriptor = new ModuleDescriptor(_deviceModule);
                    SetSimpleInfo("Module Fault", descriptor.ModuleFault);
                }
            });
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            UpdateModuleStatusAndFault();
        }

        private void CreateTreeViewInfoItems()
        {
            _treeViewInfo.Clear();

            //TODO(TLM):目前项目中当IB/OB模块配置为Rack Optimization，缺少AENTR:1:I/AENTR:1:O，所以暂时显示为none
            if (_deviceModule.InputTag != null || _deviceModule.OutputTag != null || _deviceModule.ConfigTag != null)
            {
                _treeViewInfo.Add(new OrganizerItemInfo()
                {
                    Name = "ModuleDefinedVariables",
                    IconVisibility = Visibility.Collapsed
                });

                if (_deviceModule.InputTag != null)
                {
                    _treeViewInfo.Add(new OrganizerItemInfo
                    {
                        DisplayName = _deviceModule.InputTag.Name,
                        IconVisibility = Visibility.Visible,
                        IconKind = "TagPlus",
                        IconForeground = "#FFFFD700",
                        Space = "    "
                    });
                }

                if (_deviceModule.OutputTag != null)
                {
                    _treeViewInfo.Add(new OrganizerItemInfo
                    {
                        DisplayName = _deviceModule.OutputTag.Name,
                        IconVisibility = Visibility.Visible,
                        IconKind = "TagPlus",
                        IconForeground = "#FFFFD700",
                        Space = "    "
                    });
                }

                if (_deviceModule.ConfigTag != null)
                {
                    _treeViewInfo.Add(new OrganizerItemInfo
                    {
                        DisplayName = _deviceModule.ConfigTag.Name,
                        IconVisibility = Visibility.Visible,
                        IconKind = "TagPlus",
                        IconForeground = "#FFFFD700",
                        Space = "    "
                    });
                }
            }
            else
            {
                _treeViewInfo.Add(new OrganizerItemInfo()
                    {DisplayName = "<none>", IconVisibility = Visibility.Collapsed});
            }
        }

        private void CreateInfoItems()
        {
            if (InfoSource != null)
            {
                string status = "Offline";
                string moduleFault = string.Empty;

                if (_deviceModule.ParentController.IsOnline)
                {
                    ModuleDescriptor descriptor = new ModuleDescriptor(_deviceModule);
                    status = descriptor.Status;
                    moduleFault = descriptor.ModuleFault;
                }

                InfoSource.Add(new SimpleInfo { Name = "Description", Value = _deviceModule.Description });
                InfoSource.Add(new SimpleInfo { Name = "Status", Value = status });
                InfoSource.Add(new SimpleInfo { Name = "Module Fault", Value = moduleFault, Key = "ModuleFault" });
            }
        }

        private void UpdateModuleStatusAndFault()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(
                async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    string status = "Offline";
                    string moduleFault = string.Empty;

                    if (_deviceModule.ParentController.IsOnline)
                    {
                        ModuleDescriptor descriptor = new ModuleDescriptor(_deviceModule);
                        status = descriptor.Status;
                        moduleFault = descriptor.ModuleFault;
                    }

                    SetSimpleInfo("Status", status);
                    SetSimpleInfo("Module Fault", moduleFault);

                });
        }
    }
}
