using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Project;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using ICSStudio.OrganizerPackage.Model;
using DeviceModule = ICSStudio.SimpleServices.DeviceModule.DeviceModule;

namespace ICSStudio.OrganizerPackage.ViewModel
{
    public class OrganizerItemInfo : OrganizerItem
    {
        public Visibility IconVisibility { set; get; }
        public string Space { set; get; }
    }

    public partial class ControllerOrganizerVM
    {
        public ObservableCollection<SimpleInfo> DataGridInfo { set; get; }
        public ObservableCollection<OrganizerItemInfo> ItemTreeViewInfo { set; get; }
        public Visibility ItemTreeViewInfoVisibility { set; get; }

        private BaseSimpleInfo _currentSimpleInfo;

        [SuppressMessage("ReSharper", "MergeCastWithTypeCheck")]
        private void SetSimpleInfoTreeView(OrganizerItem organizerItem)
        {
            if (_currentSimpleInfo != null)
            {
                _currentSimpleInfo.Clear();
                _currentSimpleInfo = null;
            }

            ItemTreeViewInfoVisibility = Visibility.Hidden;

            ItemTreeViewInfo.Clear();
            DataGridInfo.Clear();

            //
            if (organizerItem.Kind == ProjectItemType.ControllerModel)
            {
                IController controller = organizerItem.AssociatedObject as IController;
                _currentSimpleInfo = new ControllerInfo(controller, DataGridInfo);
            }

            if (organizerItem.Kind == ProjectItemType.Task)
            {
                CTask task = organizerItem.AssociatedObject as CTask;
                _currentSimpleInfo = new TaskInfo(task, DataGridInfo);
            }

            if (organizerItem.Kind == ProjectItemType.Program)
            {
                IProgram program = organizerItem.AssociatedObject as IProgram;
                _currentSimpleInfo = new ProgramInfo(program, DataGridInfo);
            }

            if (organizerItem.Kind == ProjectItemType.Routine)
            {
                IRoutine routine = organizerItem.AssociatedObject as IRoutine;
                _currentSimpleInfo = new RoutineInfo(routine, DataGridInfo);
            }

            if (organizerItem.Kind == ProjectItemType.MotionGroup)
            {
                Tag tag = organizerItem.AssociatedObject as Tag;
                _currentSimpleInfo = new MotionGroupInfo(tag, DataGridInfo);
            }

            if (organizerItem.Kind == ProjectItemType.AxisCIPDrive)
            {
                Tag tag = organizerItem.AssociatedObject as Tag;
                _currentSimpleInfo = new AxisCIPDriveInfo(tag, DataGridInfo, ItemTreeViewInfo);
                ItemTreeViewInfoVisibility = Visibility.Visible;
            }

            if (organizerItem.Kind == ProjectItemType.AxisVirtual)
            {
                Tag tag = organizerItem.AssociatedObject as Tag;
                _currentSimpleInfo = new AxisVirtualInfo(tag, DataGridInfo);
            }

            if (organizerItem.Kind == ProjectItemType.Assets)
            {
                //TODO(ZYL):Assets
            }

            if (organizerItem.Kind == ProjectItemType.UserDefined
                || organizerItem.Kind == ProjectItemType.String
                || organizerItem.Kind == ProjectItemType.Predefined
                || organizerItem.Kind == ProjectItemType.ModuleDefined)
            {
                IDataType dataType = organizerItem.AssociatedObject as IDataType;
                _currentSimpleInfo = new DataTypeInfo(dataType, DataGridInfo);
            }

            if (organizerItem.Kind == ProjectItemType.AddOnDefined
                || organizerItem.Kind == ProjectItemType.AddOnInstruction)
            {
                AoiDefinition aoiDefinition = organizerItem.AssociatedObject as AoiDefinition;
                _currentSimpleInfo = new AddOnInstructionInfo(aoiDefinition, DataGridInfo);
            }

            if (organizerItem.Kind == ProjectItemType.Trend)
            {
                Trend trend = organizerItem.AssociatedObject as Trend;
                _currentSimpleInfo = new TrendInfo(trend, DataGridInfo);
            }

            if (organizerItem.Kind == ProjectItemType.AddOnInstructions)
            {
                //TODO(ZYL):AddOnInstructions
            }

            if (organizerItem.Kind == ProjectItemType.IOConfiguration)
            {
                //TODO(ZYL):IOConfiguration
            }

            if (organizerItem.Kind == ProjectItemType.Ethernet)
            {
                DataGridInfo.Add(new SimpleInfo
                    { Name = "Bus Size", Value = "" });
            }

            if (organizerItem.Kind == ProjectItemType.Bus)
            {
                var module = (DeviceModule)organizerItem.AssociatedObject;
                var port = module.GetFirstPort(PortType.PointIO);
                DataGridInfo.Add(new SimpleInfo
                    { Name = "Bus Size", Value = $"{port?.Bus.Size}" });
            }

            if (organizerItem.Kind == ProjectItemType.DeviceModule
                || organizerItem.Kind == ProjectItemType.LocalModule)
            {
                DeviceModule deviceModule = organizerItem.AssociatedObject as DeviceModule;

                if (deviceModule is LocalModule)
                {
                    _currentSimpleInfo = new LocalModuleInfo(deviceModule as LocalModule, DataGridInfo);
                }
                else if (deviceModule is CIPMotionDrive)
                {
                    _currentSimpleInfo =
                        new CIPMotionDriveInfo(deviceModule as CIPMotionDrive, DataGridInfo, ItemTreeViewInfo);
                    ItemTreeViewInfoVisibility = Visibility.Visible;
                }
                else
                {
                    _currentSimpleInfo =
                        new DeviceModuleInfo(deviceModule, DataGridInfo, ItemTreeViewInfo);
                    //TODO(gjc): need edit later
                    ItemTreeViewInfoVisibility = Visibility.Collapsed;
                }

            }

        }
    }
}
