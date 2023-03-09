using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.Interfaces.Tags;
using ICSStudio.OrganizerPackage.ViewModel.Items;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.SimpleServices.Utilities;
using ICSStudio.UIInterfaces.Project;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.OrganizerPackage.ViewModel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    public partial class ControllerOrganizerVM
    {
        private readonly ObservableCollection<OrganizerItem> _selectedItems;

        private IController _controller;

        public IList<OrganizerItem> SelectedItems => _selectedItems;

        public OrganizerItems ControllerOrganizerItems => _root.ProjectItems as OrganizerItems;

        private void CleanupProjectItems()
        {
            foreach (var organizerItem in ((OrganizerItems)_root.ProjectItems))
            {
                organizerItem.Cleanup();
            }
        }

        private void RebuildItems()
        {
            RemoveHandlers(_controller);

            IProjectInfoService projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            var controller = projectInfoService?.Controller;

            _controller = controller;
            
            ((OrganizerItems)_root.ProjectItems).Clear();

            if (controller != null)
            {
                // Controller
                OrganizerItem controllerItem = CreateControllerItem(controller);
                _root.ProjectItems.Add(controllerItem);

                // Tasks
                OrganizerItem tasksItem = CreateTasksItem(controller);
                _root.ProjectItems.Add(tasksItem);

                // Motion Groups
                OrganizerItem motionGroupsItem = CreateMotionGroupsItem(controller);
                _root.ProjectItems.Add(motionGroupsItem);

                // Assets
                OrganizerItem assetsItem = CreateAssetsItem(controller);
                _root.ProjectItems.Add(assetsItem);

                // Logical Model
                //var logicalModelItem = CreateLogicalModelItem(controller);
                //_root.ProjectItems.Add(logicalModelItem);

                // I/O Configuration
                OrganizerItem ioConfigurationItem = CreateIOConfigurationItem(controller);
                _root.ProjectItems.Add(ioConfigurationItem);

            }

            AddHandlers(_controller);

            RaisePropertyChanged("ControllerOrganizerItems");
        }

        private void AddHandlers(IController controller)
        {
            if (controller != null)
            {
                CollectionChangedEventManager.AddHandler(controller.Tasks, OnTasksChanged);
                CollectionChangedEventManager.AddHandler(controller.Programs, OnProgramsChanged);
                CollectionChangedEventManager.AddHandler(controller.Tags, OnTagsChanged);
                CollectionChangedEventManager.AddHandler(controller.AOIDefinitionCollection, OnAOIDefinitionsChanged);
                CollectionChangedEventManager.AddHandler(controller.DataTypes, OnDataTypesChanged);
                CollectionChangedEventManager.AddHandler(controller.Trends, OnTrendsChanged);
                CollectionChangedEventManager.AddHandler(controller.DeviceModules, OnDeviceModulesChanged);

                foreach (var tag in controller.Tags)
                {
                    PropertyChangedEventManager.AddHandler(tag, OnTagChanged, string.Empty);
                }
            }
        }

        private void RemoveHandlers(IController controller)
        {
            if (controller != null)
            {
                foreach (var tag in controller.Tags)
                {
                    PropertyChangedEventManager.RemoveHandler(tag, OnTagChanged, string.Empty);
                }

                foreach (var program in controller.Programs)
                {
                    CollectionChangedEventManager.RemoveHandler(program.Routines, OnRoutinesChanged);
                }

                foreach (var aoiDefinition in controller.AOIDefinitionCollection)
                {
                    CollectionChangedEventManager.RemoveHandler(aoiDefinition.Routines, OnRoutinesChanged);
                }


                CollectionChangedEventManager.RemoveHandler(controller.Tasks, OnTasksChanged);
                CollectionChangedEventManager.RemoveHandler(controller.Programs, OnProgramsChanged);
                CollectionChangedEventManager.RemoveHandler(controller.Tags, OnTagsChanged);
                CollectionChangedEventManager.RemoveHandler(controller.AOIDefinitionCollection,
                    OnAOIDefinitionsChanged);
                CollectionChangedEventManager.RemoveHandler(controller.DataTypes, OnDataTypesChanged);
                CollectionChangedEventManager.RemoveHandler(controller.Trends, OnTrendsChanged);
                CollectionChangedEventManager.RemoveHandler(controller.DeviceModules, OnDeviceModulesChanged);

            }
        }

        private void OnTagChanged(object sender, PropertyChangedEventArgs e)
        {
            Tag tag = sender as Tag;

            if (tag == null)
                return;

            if (e.PropertyName == "AssignedGroup")
            {
                OrganizerItem axisItem = GetAxisItem(tag);
                SelectedItems.Remove(axisItem);
            }

            if (e.PropertyName == "DataWrapper")
            {
                var args = e as PropertyChangedExtendedEventArgs<DataWrapper>;
                if (args != null)
                {
                    var oldValue = args.OldValue;
                    var newValue = args.NewValue;

                    if (oldValue != null)
                    {
                        var dataType = oldValue.DataTypeInfo.DataType;

                        OrganizerItem removeItem = null;

                        if (dataType.IsMotionGroupType)
                        {
                            removeItem = GetMotionGroupItem(tag);
                        }

                        if (dataType.IsAxisType)
                        {
                            removeItem = GetAxisItem(tag);
                        }

                        if (dataType.IsCoordinateSystemType)
                        {
                            removeItem = GetCoordinateSystemItem(tag);
                        }

                        if (removeItem != null)
                        {
                            var items = removeItem.Collection as OrganizerItems;
                            Contract.Assert(items != null);

                            items.Remove(removeItem);

                            ResetSelectedItems();
                            removeItem.Cleanup();
                        }
                    }

                    if (newValue != null)
                    {
                        var dataType = newValue.DataTypeInfo.DataType;

                        if (dataType.IsMotionGroupType)
                        {
                            var motionGroupItem = new MotionGroupItem(tag);

                            OrganizerItem motionGroupsItem = GetMotionGroupsItem();

                            var items = motionGroupsItem.ProjectItems as OrganizerItems;
                            Contract.Assert(items != null);

                            items.Insert(0, motionGroupItem);
                        }

                        if (dataType.IsAxisType)
                        {
                            AddAxisItem(tag);
                        }

                        if (dataType.IsCoordinateSystemType)
                        {
                            AddCoordinateSystemItem(tag);
                        }
                    }
                }
            }

        }

        private void OnRoutinesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var routine in e.NewItems.OfType<IRoutine>())
                    {
                        IProgram program = routine.ParentCollection.ParentProgram as IProgram;
                        IAoiDefinition aoiDefinition = routine.ParentCollection.ParentProgram as IAoiDefinition;

                        if (program != null)
                        {
                            var programItem = GetProgramItem(program);
                            var routineItem = new RoutineItem(routine);

                            InsertRoutineItem(programItem, routineItem);
                        }
                        else if (aoiDefinition != null)
                        {
                            var aoiItem = GetAddOnInstructionItem(aoiDefinition);
                            var routineItem = new RoutineItem(routine);

                            InsertRoutineItemInAoi(aoiItem, routineItem);
                        }
                        else
                        {
                            throw new NotImplementedException("Check here!");
                        }

                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var routine in e.OldItems.OfType<IRoutine>())
                    {
                        var routineItem = GetRoutineItem(routine);
                        if (routineItem != null)
                        {
                            RemoveItem(routineItem);
                        }
                    }

                    ResetSelectedItems();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void InsertRoutineItemInAoi(OrganizerItem aoiItem, RoutineItem routineItem)
        {
            IAoiDefinition aoiDefinition = aoiItem.AssociatedObject as IAoiDefinition;
            Contract.Assert(aoiDefinition != null);

            OrganizerItems items = aoiItem.ProjectItems as OrganizerItems;
            Contract.Assert(items != null);

            List<string> routineNames = new List<string>
            {
                "", "Logic", "Prescan", "Postscan", "EnableInFalse"
            };

            List<string> itemNames = aoiItem.ProjectItems.OfType<OrganizerItem>().Select(x =>
            {
                if (x.Kind == ProjectItemType.ProgramTags)
                    return "0";

                if (x.Kind == ProjectItemType.Routine)
                {
                    IRoutine routine = x.AssociatedObject as IRoutine;
                    if (routine != null)
                    {
                        int index = routineNames.IndexOf(routine.Name);
                        if (index >= 0)
                        {
                            return index.ToString();
                        }
                    }
                }

                return x.DisplayName;
            }).ToList();

            IRoutine insertRoutine = routineItem.AssociatedObject as IRoutine;
            string insertName = routineItem.DisplayName;

            if (insertRoutine != null)
            {
                int i = routineNames.IndexOf(insertRoutine.Name);
                if (i >= 0)
                {
                    insertName = i.ToString();
                }
            }

            itemNames.Add(insertName);
            itemNames.Sort((x, y) => string.Compare(x, y, StringComparison.OrdinalIgnoreCase));

            int insertIndex = itemNames.IndexOf(insertName);

            items.Insert(insertIndex, routineItem);
        }

        private void InsertRoutineItem(OrganizerItem programItem, RoutineItem routineItem)
        {
            IProgram program = programItem.AssociatedObject as IProgram;
            Contract.Assert(program != null);

            OrganizerItems items = programItem.ProjectItems as OrganizerItems;
            Contract.Assert(items != null);

            List<string> itemNames = programItem.ProjectItems.OfType<OrganizerItem>().Select(x =>
            {
                if (x.Kind == ProjectItemType.ProgramTags)
                    return "0";

                if (x.Kind == ProjectItemType.Routine)
                {
                    IRoutine routine = x.AssociatedObject as IRoutine;
                    if (routine != null)
                    {
                        if (routine.IsMainRoutine)
                            return "0" + x.DisplayName;
                        if (routine.IsFaultRoutine)
                            return "1" + x.DisplayName;
                    }
                }

                return x.DisplayName;
            }).ToList();

            IRoutine insertRoutine = routineItem.AssociatedObject as IRoutine;
            string insertName = routineItem.DisplayName;
            if (insertRoutine != null)
            {
                if (insertRoutine.IsMainRoutine)
                    insertName = "0" + insertName;
                if (insertRoutine.IsFaultRoutine)
                    insertName = "1" + insertName;
            }

            itemNames.Add(insertName);
            itemNames.Sort((x, y) => string.Compare(x, y, StringComparison.OrdinalIgnoreCase));

            int index = itemNames.IndexOf(insertName);

            items.Insert(index, routineItem);
        }

        private void OnDeviceModulesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var device in e.NewItems.OfType<DeviceModule>())
                    {
                        OrganizerItem deviceItem = null;

                        DeviceModule parentModule = device.ParentModule as DeviceModule;
                        Contract.Assert(parentModule != null);

                        var parentModulePort = parentModule.GetPortById(device.ParentModPortId);
                        Contract.Assert(parentModulePort != null);

                        var parentBusItem = GetParentBusItem(device.ParentModule, device.ParentModPortId);
                        switch (parentModulePort.Type)
                        {
                            case PortType.Ethernet:
                                deviceItem = new EthernetItem(device, PortType.Ethernet);
                                break;
                            case PortType.PointIO:
                                deviceItem = new PointIOItem(device, PortType.PointIO);
                                break;
                            case PortType.Compact:
                                break;
                            case PortType.ICP:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        Contract.Assert(deviceItem != null);

                        InsertDeviceItem(parentBusItem, parentModulePort.Type, deviceItem);

                        // PointIO
                        var port = device.GetFirstPort(PortType.PointIO);
                        if (port != null && !port.Upstream)
                        {
                            var pointIOItem = new PointIORootItem(device);
                            deviceItem.ProjectItems.Add(pointIOItem);

                            AddPointIOModuleItems(_controller, device, pointIOItem, port);
                        }
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var device in e.OldItems.OfType<DeviceModule>())
                    {
                        var deviceItem = GetDeviceItem(device);
                        if (deviceItem != null)
                        {
                            OrganizerItems items = deviceItem.Collection as OrganizerItems;
                            var pointIORootItem = items?.Parent as PointIORootItem;

                            RemoveItem(deviceItem);

                            if (pointIORootItem != null)
                            {
                                if (items.Count == 1)
                                {
                                    if (pointIORootItem.AssociatedObject == items[0].AssociatedObject)
                                    {
                                        items.Clear();
                                    }
                                }
                            }
                        }
                    }

                    ResetSelectedItems();

                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private void OnTrendsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var trend in e.NewItems.OfType<ITrend>())
                    {
                        OrganizerItem parentItem = GetTrendsItem();

                        InsertOrganizerItem(parentItem, new TrendItem(trend));
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var trend in e.OldItems.OfType<ITrend>())
                    {
                        OrganizerItem item = GetTrendItem(trend);
                        RemoveItem(item);
                    }

                    ResetSelectedItems();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnDataTypesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    foreach (var dataType in e.NewItems.OfType<UserDefinedDataType>())
                    {
                        OrganizerItem parentItem;

                        if (dataType.IsStringType)
                        {
                            parentItem = GetStringsItem();
                            InsertOrganizerItem(parentItem, new StringTypeItem(dataType));
                        }
                        else
                        {
                            parentItem = GetUserDefinedsItem();
                            InsertOrganizerItem(parentItem, new UserDefinedItem(dataType));
                        }
                    }

                    foreach (var dataType in e.NewItems.OfType<ModuleDefinedDataType>())
                    {
                        OrganizerItem parentItem = GetModuleDefinedsItem();

                        var item = new OrganizerItem()
                        {
                            DisplayName = dataType.Name,
                            Kind = ProjectItemType.ModuleDefined,
                            AssociatedObject = dataType
                        };

                        InsertOrganizerItem(parentItem, item);
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var dataType in e.OldItems.OfType<UserDefinedDataType>())
                    {
                        var item = GetUserDefinedItem(dataType);
                        RemoveItem(item);
                    }

                    ResetSelectedItems();

                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnAOIDefinitionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Contract.Assert(e.NewItems.Count == 1);

                    IAoiDefinition definition = e.NewItems[0] as IAoiDefinition;
                    Contract.Assert(definition != null);

                    OrganizerItem addOnInstructionsItem = GetAddOnInstructionsItem();
                    InsertOrganizerItem(addOnInstructionsItem, new AddOnInstructionItem(definition));

                    OrganizerItem addOnDefinedsItem = GetAddOnDefinedsItem();
                    InsertOrganizerItem(addOnDefinedsItem, new AddOnDefinedItem(definition));

                    break;
                case NotifyCollectionChangedAction.Remove:

                    foreach (var removeDefinition in e.OldItems.OfType<IAoiDefinition>())
                    {
                        var aoiDefinitionItem = GetAddOnInstructionItem(removeDefinition);
                        RemoveItem(aoiDefinitionItem);

                        var addOnDefinedItem = GetAddOnDefinedItem(removeDefinition);
                        RemoveItem(addOnDefinedItem);
                    }

                    ResetSelectedItems();

                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RemoveItem(OrganizerItem item)
        {
            OrganizerItems items = item?.Collection as OrganizerItems;
            items?.Remove(item);
            item?.Cleanup();
        }

        private void OnTagsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ITag tag;
            OrganizerItems items;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    //Contract.Assert(e.NewItems.Count == 1);
                    foreach (var item in e.NewItems)
                    {
                        tag = item as ITag;
                        Contract.Assert(tag != null);

                        if (!tag.IsAlias && tag.DataTypeInfo.DataType.IsMotionGroupType)
                        {
                            var motionGroupItem = new MotionGroupItem(tag);

                            OrganizerItem motionGroupsItem = GetMotionGroupsItem();

                            items = motionGroupsItem.ProjectItems as OrganizerItems;
                            Contract.Assert(items != null);

                            items.Insert(0, motionGroupItem);
                        }

                        if (!tag.IsAlias && tag.DataTypeInfo.DataType.IsAxisType)
                        {
                            AddAxisItem(tag);
                        }

                        if (!tag.IsAlias && tag.DataTypeInfo.DataType.IsCoordinateSystemType)
                        {
                            AddCoordinateSystemItem(tag);
                        }

                        if (tag.IsControllerScoped)
                        {
                            PropertyChangedEventManager.AddHandler(tag, OnTagChanged, string.Empty);
                        }

                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var removeTag in e.OldItems.OfType<ITag>())
                    {
                        OrganizerItem removeItem = null;

                        if (!removeTag.IsAlias && removeTag.DataTypeInfo.DataType.IsMotionGroupType)
                        {
                            removeItem = GetMotionGroupItem(removeTag);
                        }

                        if (!removeTag.IsAlias && removeTag.DataTypeInfo.DataType.IsAxisType)
                        {
                            removeItem = GetAxisItem(removeTag);
                        }

                        if (!removeTag.IsAlias && removeTag.DataTypeInfo.DataType.IsCoordinateSystemType)
                        {
                            removeItem = GetCoordinateSystemItem(removeTag);
                        }

                        if (removeItem != null)
                        {
                            items = removeItem.Collection as OrganizerItems;
                            Contract.Assert(items != null);
                            items.Remove(removeItem);
                            removeItem.Cleanup();
                        }

                        if (removeTag.IsControllerScoped)
                        {
                            PropertyChangedEventManager.RemoveHandler(removeTag, OnTagChanged, string.Empty);
                        }
                    }

                    ResetSelectedItems();

                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private void AddCoordinateSystemItem(ITag tag)
        {
            //由于此处会导致导入有坐标系类型tag时报错等问题，所以暂时注掉
            //throw new NotImplementedException();
        }

        private void AddAxisItem(ITag tag)
        {
            if (_controller == null)
                return;

            Tag axis = tag as Tag;
            Contract.Assert(axis != null);

            AxisCIPDrive axisCIPDrive = axis.DataWrapper as AxisCIPDrive;
            AxisVirtual axisVirtual = axis.DataWrapper as AxisVirtual;

            ITag assignedGroup;

            if (axisCIPDrive != null)
            {
                assignedGroup = axisCIPDrive.AssignedGroup;
            }
            else if (axisVirtual != null)
            {
                assignedGroup = axisVirtual.AssignedGroup;
            }
            else
            {
                throw new NotImplementedException("Check here!");
            }

            var parentItem =
                assignedGroup != null ? GetMotionGroupItem(assignedGroup) : GetUngroupedAxesItem();

            Contract.Assert(parentItem != null);

            var axisItem = new AxisItem(tag);
            InsertAxisItem(parentItem, axisItem);
        }

        private void InsertAxisItem(OrganizerItem parentItem, AxisItem axisItem)
        {
            OrganizerItems items = parentItem.ProjectItems as OrganizerItems;
            Contract.Assert(items != null);

            int index = 0;
            foreach (var item in items)
            {
                if (item.Kind == ProjectItemType.CoordinateSystem)
                    break;

                if (string.Compare(
                        item.DisplayName,
                        axisItem.DisplayName,
                        StringComparison.OrdinalIgnoreCase) > 0)
                    break;

                index++;
            }

            items.Insert(index, axisItem);
        }

        private void InsertOrganizerItem(OrganizerItem parentItem, OrganizerItem newItem)
        {
            OrganizerItems items = parentItem.ProjectItems as OrganizerItems;
            Contract.Assert(items != null);

            int index = 0;
            foreach (var item in items)
            {
                if (string.Compare(
                        item.DisplayName,
                        newItem.DisplayName,
                        StringComparison.OrdinalIgnoreCase) > 0)
                    break;

                index++;
            }

            items.Insert(index, newItem);
        }

        private void InsertDeviceItem(OrganizerItem parentItem, PortType portType, OrganizerItem newItem)
        {
            OrganizerItems items = parentItem.ProjectItems as OrganizerItems;
            Contract.Assert(items != null);

            DeviceModule newDevice = newItem.AssociatedObject as DeviceModule;
            Contract.Assert(newDevice != null);

            Port newDevicePort = newDevice.GetFirstPort(portType);
            Contract.Assert(newDevicePort != null);

            int index = 0;
            foreach (var item in items)
            {
                if (parentItem.AssociatedObject == item.AssociatedObject)
                {
                    index++;
                    continue;
                }

                DeviceModule device = item.AssociatedObject as DeviceModule;
                Contract.Assert(device != null);

                Port port = device.GetFirstPort(portType);
                Contract.Assert(port != null);

                if (portType == PortType.Ethernet)
                {
                    if (CompareEthernetAddress(device, newDevice) > 0)
                        break;
                }
                else if (portType == PortType.PointIO)
                {
                    int portIndex = int.Parse(port.Address);
                    int newPortIndex = int.Parse(newDevicePort.Address);

                    if (portIndex > newPortIndex)
                        break;
                }
                else
                {
                    if (string.Compare(port.Address, newDevicePort.Address, StringComparison.OrdinalIgnoreCase) > 0)
                        break;
                }


                index++;
            }

            if (portType == PortType.PointIO && items.Count == 0)
            {
                // Local
                var localItem = new PointIOItem(parentItem.AssociatedObject as IDeviceModule, portType);
                items.Add(localItem);
                items.Add(newItem);
            }
            else
            {
                items.Insert(index, newItem);
            }

        }

        private void OnTasksChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_controller == null)
                return;

            var tasksItem = GetTasksItem();
            if (tasksItem == null)
                return;

            var items = tasksItem.ProjectItems as OrganizerItems;
            Contract.Assert(items != null);

            ITask task;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    Contract.Assert(e.NewItems.Count == 1);

                    task = e.NewItems[0] as ITask;
                    Contract.Assert(task != null);

                    var names = _controller.Tasks.Select(x => x.Name).ToList();
                    names.Sort((x, y) =>
                        string.Compare(x, y, StringComparison.OrdinalIgnoreCase));

                    int index = names.IndexOf(task.Name);

                    items.Insert(index, new TaskItem(task));

                    break;
                case NotifyCollectionChangedAction.Remove:
                    Contract.Assert(e.OldItems.Count == 1);

                    task = e.OldItems[0] as ITask;
                    Contract.Assert(task != null);

                    var taskItem = GetTaskItem(task);

                    items.Remove(taskItem);

                    SelectedItems.Remove(taskItem);
                    taskItem.Cleanup();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnProgramsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_controller == null)
                return;

            OrganizerItem taskItem;
            OrganizerItems items;
            OrganizerItem programItem;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    foreach (var program in e.NewItems.OfType<IProgram>())
                    {
                        programItem = NewProgramItem(program);

                        if (program.ParentTask != null)
                        {
                            taskItem = GetTaskItem(program.ParentTask);

                            taskItem.ProjectItems.Add(programItem);
                        }
                        else if (program.Name == _controller.MajorFaultProgram)
                        {
                            var faultHandlerItem = GetFaultHandlerItem();
                            faultHandlerItem.ProjectItems.Add(programItem);
                        }
                        else if (program.Name == _controller.PowerLossProgram)
                        {
                            var powerUpHandlerItem = GetPowerUpHandlerItem();
                            powerUpHandlerItem.ProjectItems.Add(programItem);
                        }
                        else
                        {
                            var unscheduledProgramsItem = GetUnscheduledProgramsItem();

                            items = unscheduledProgramsItem.ProjectItems as OrganizerItems;
                            Contract.Assert(items != null);

                            var names = items.Select(x => x.DisplayName).ToList();
                            names.Add(programItem.DisplayName);

                            names.Sort((x, y) => string.Compare(x, y, StringComparison.OrdinalIgnoreCase));
                            int index = names.IndexOf(programItem.DisplayName);

                            items.Insert(index, programItem);
                        }
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:

                    foreach (var program in e.OldItems.OfType<IProgram>())
                    {
                        if (program.ParentTask != null)
                        {
                            taskItem = GetTaskItem(program.ParentTask);

                            items = taskItem.ProjectItems as OrganizerItems;
                            Contract.Assert(items != null);

                            programItem = GetProgramItem(items, program);
                            Contract.Assert(programItem != null);

                            items.Remove(programItem);
                            SelectedItems.Remove(programItem);
                            programItem.Cleanup();
                        }
                        else
                        {
                            // Fault Handler
                            var faultHandlerItem = GetFaultHandlerItem();
                            items = faultHandlerItem.ProjectItems as OrganizerItems;
                            Contract.Assert(items != null);

                            programItem = GetProgramItem(items, program);
                            if (programItem != null)
                            {
                                items.Remove(programItem);
                                SelectedItems.Remove(programItem);
                                programItem.Cleanup();
                            }

                            // Power-Up Handler
                            var powerUpHandlerItem = GetPowerUpHandlerItem();
                            items = powerUpHandlerItem.ProjectItems as OrganizerItems;
                            Contract.Assert(items != null);

                            programItem = GetProgramItem(items, program);
                            if (programItem != null)
                            {
                                items.Remove(programItem);
                                SelectedItems.Remove(programItem);
                                programItem.Cleanup();
                            }

                            // Unscheduled
                            var unscheduledProgramsItem = GetUnscheduledProgramsItem();
                            items = unscheduledProgramsItem.ProjectItems as OrganizerItems;
                            Contract.Assert(items != null);

                            programItem = GetProgramItem(items, program);
                            if (programItem != null)
                            {
                                items.Remove(programItem);
                                SelectedItems.Remove(programItem);
                                programItem.Cleanup();
                            }
                        }
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    Contract.Assert(e.NewItems.Count == 1);

                    IProgram movedProgram = e.NewItems[0] as IProgram;
                    Contract.Assert(movedProgram?.ParentTask != null);

                    taskItem = GetTaskItem(movedProgram.ParentTask);
                    items = taskItem.ProjectItems as OrganizerItems;
                    Contract.Assert(items != null);

                    // move end
                    programItem = GetProgramItem(items, movedProgram);
                    int oldIndex = items.IndexOf(programItem);
                    int newIndex = items.Count - 1;
                    items.Move(oldIndex, newIndex);

                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region Get Item

        public OrganizerItem GetRoutineItem(IRoutine routine)
        {
            return GetRoutineItem(_root, routine);
        }

        private OrganizerItem GetRoutineItem(OrganizerItem rootItem, IRoutine routine)
        {
            if (rootItem.AssociatedObject == routine)
                return rootItem;

            foreach (var item in rootItem.ProjectItems.OfType<OrganizerItem>())
            {
                OrganizerItem routineItem = GetRoutineItem(item, routine);
                if (routineItem != null)
                    return routineItem;
            }

            return null;
        }

        private OrganizerItem GetDeviceItem(DeviceModule device)
        {
            return GetDeviceItem(_root, device);
        }

        private OrganizerItem GetDeviceItem(OrganizerItem rootItem, DeviceModule device)
        {
            if (rootItem.AssociatedObject == device)
                return rootItem;

            foreach (var item in rootItem.ProjectItems.OfType<OrganizerItem>())
            {
                OrganizerItem deviceItem = GetDeviceItem(item, device);
                if (deviceItem != null)
                    return deviceItem;
            }

            return null;
        }

        private OrganizerItem GetParentBusItem(IDeviceModule parentModule, int portId)
        {
            LocalModule localModule = parentModule as LocalModule;
            if (localModule != null)
            {
                var port = localModule.GetPortById(portId);
                if (port != null)
                {
                    switch (port.Type)
                    {
                        case PortType.Ethernet:
                            return GetEthernetRootItem(_root, localModule);
                        case PortType.PointIO:
                            return GetPointIORootItem(_root, localModule);
                        case PortType.Compact:
                            break;
                        case PortType.ICP:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            else
            {
                DeviceModule parentDeviceModule = parentModule as DeviceModule;
                Contract.Assert(parentDeviceModule != null);

                var port = parentDeviceModule.GetPortById(portId);
                Contract.Assert(port != null);

                OrganizerItem deviceItem = GetDeviceItem(parentDeviceModule);
                Contract.Assert(deviceItem != null);

                foreach (var item in deviceItem.ProjectItems.OfType<OrganizerItem>())
                {
                    switch (port.Type)
                    {
                        case PortType.Ethernet:
                            if (item.Kind == ProjectItemType.Ethernet)
                                return item;
                            break;
                        case PortType.PointIO:
                            if (item.Kind == ProjectItemType.Bus)
                                return item;
                            break;
                        case PortType.Compact:
                            if (item.Kind == ProjectItemType.Bus)
                                return item;
                            break;
                        case PortType.ICP:
                            if (item.Kind == ProjectItemType.Bus)
                                return item;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            return null;
        }

        private OrganizerItem GetPointIORootItem(OrganizerItem item, IDeviceModule device)
        {
            if (item.Kind == ProjectItemType.Bus && item.AssociatedObject == device)
                return item;

            foreach (var subItem in item.ProjectItems.OfType<OrganizerItem>())
            {
                var rootItem = GetPointIORootItem(subItem, device);
                if (rootItem != null)
                    return rootItem;
            }

            return null;
        }

        private OrganizerItem GetEthernetRootItem(OrganizerItem item, IDeviceModule device)
        {
            if (item.Kind == ProjectItemType.Ethernet && item.AssociatedObject == device)
                return item;

            foreach (var subItem in item.ProjectItems.OfType<OrganizerItem>())
            {
                var rootItem = GetEthernetRootItem(subItem, device);
                if (rootItem != null)
                    return rootItem;
            }

            return null;
        }

        private OrganizerItem GetTrendItem(ITrend trend)
        {
            var trendsItem = GetTrendsItem();
            if (trendsItem != null)
            {
                foreach (var item in trendsItem.ProjectItems.OfType<OrganizerItem>())
                {
                    if (item.AssociatedObject == trend)
                        return item;
                }
            }

            return null;
        }

        private OrganizerItem GetTrendsItem()
        {
            var assetsItem = GetAssetsItem();
            if (assetsItem != null)
            {
                foreach (var item in assetsItem.ProjectItems.OfType<OrganizerItem>())
                {
                    if (item.Kind == ProjectItemType.Trends)
                        return item;
                }
            }

            return null;
        }

        private OrganizerItem GetModuleDefinedsItem()
        {
            var dataTypesItem = GetDataTypesItem();
            if (dataTypesItem != null)
            {
                foreach (var item in dataTypesItem.ProjectItems.OfType<OrganizerItem>())
                {
                    if (item.Kind == ProjectItemType.ModuleDefineds)
                        return item;
                }
            }

            return null;
        }

        private OrganizerItem GetUserDefinedItem(UserDefinedDataType dataType)
        {
            var userDefinedsItem = GetUserDefinedsItem();
            if (userDefinedsItem != null)
            {
                foreach (var item in userDefinedsItem.ProjectItems.OfType<OrganizerItem>())
                {
                    if (item.AssociatedObject == dataType)
                        return item;
                }
            }

            var stringsItem = GetStringsItem();
            if (stringsItem != null)
            {
                foreach (var item in stringsItem.ProjectItems.OfType<OrganizerItem>())
                {
                    if (item.AssociatedObject == dataType)
                        return item;
                }
            }

            return null;
        }

        private OrganizerItem GetUserDefinedsItem()
        {
            var dataTypesItem = GetDataTypesItem();
            if (dataTypesItem != null)
            {
                foreach (var item in dataTypesItem.ProjectItems.OfType<OrganizerItem>())
                {
                    if (item.Kind == ProjectItemType.UserDefineds)
                        return item;
                }
            }

            return null;
        }

        private OrganizerItem GetStringsItem()
        {
            var dataTypesItem = GetDataTypesItem();
            if (dataTypesItem != null)
            {
                foreach (var item in dataTypesItem.ProjectItems.OfType<OrganizerItem>())
                {
                    if (item.Kind == ProjectItemType.Strings)
                        return item;
                }
            }

            return null;
        }

        private OrganizerItem GetAddOnDefinedItem(IAoiDefinition definition)
        {
            var addOnDefinedsItem = GetAddOnDefinedsItem();
            if (addOnDefinedsItem != null)
            {
                foreach (var item in addOnDefinedsItem.ProjectItems.OfType<OrganizerItem>())
                {
                    if (item.AssociatedObject == definition)
                        return item;
                }
            }

            return null;
        }

        private OrganizerItem GetAddOnDefinedsItem()
        {
            var dataTypesItem = GetDataTypesItem();
            if (dataTypesItem != null)
            {
                foreach (var item in dataTypesItem.ProjectItems.OfType<OrganizerItem>())
                {
                    if (item.Kind == ProjectItemType.AddOnDefineds)
                        return item;
                }
            }

            return null;
        }

        public OrganizerItem GetAddOnInstructionItem(IAoiDefinition definition)
        {
            var addOnInstructionsItem = GetAddOnInstructionsItem();
            foreach (var item in addOnInstructionsItem.ProjectItems.OfType<OrganizerItem>())
            {
                if (item.AssociatedObject == definition)
                    return item;
            }

            return null;
        }

        private OrganizerItem GetDataTypesItem()
        {
            var assetsItem = GetAssetsItem();
            if (assetsItem != null)
            {
                foreach (var item in assetsItem.ProjectItems.OfType<OrganizerItem>())
                {
                    if (item.Kind == ProjectItemType.DataTypes)
                        return item;
                }
            }

            return null;
        }

        private OrganizerItem GetAddOnInstructionsItem()
        {
            var assetsItem = GetAssetsItem();
            if (assetsItem != null)
            {
                foreach (var item in assetsItem.ProjectItems.OfType<OrganizerItem>())
                {
                    if (item.Kind == ProjectItemType.AddOnInstructions)
                        return item;
                }
            }

            return null;
        }

        private OrganizerItem GetAssetsItem()
        {
            foreach (var item in _root.ProjectItems.OfType<OrganizerItem>())
            {
                if (item.Kind == ProjectItemType.Assets)
                    return item;
            }

            return null;
        }

        private OrganizerItem GetCoordinateSystemItem(ITag tag)
        {
            var motionGroupsItem = GetMotionGroupsItem();
            if (motionGroupsItem != null)
            {
                foreach (var item in motionGroupsItem.ProjectItems.OfType<OrganizerItem>())
                {
                    foreach (var subItem in item.ProjectItems.OfType<OrganizerItem>())
                    {
                        if (subItem.AssociatedObject == tag
                            && subItem.Kind == ProjectItemType.CoordinateSystem)
                            return subItem;

                    }
                }
            }

            return null;
        }

        public OrganizerItem GetAxisItem(ITag tag)
        {
            var motionGroupsItem = GetMotionGroupsItem();
            if (motionGroupsItem != null)
            {
                foreach (var item in motionGroupsItem.ProjectItems.OfType<OrganizerItem>())
                {
                    foreach (var subItem in item.ProjectItems.OfType<OrganizerItem>())
                    {
                        if (subItem.AssociatedObject == tag)
                            return subItem;

                    }
                }
            }

            return null;
        }

        private OrganizerItem GetUngroupedAxesItem()
        {
            var motionGroupsItem = GetMotionGroupsItem();
            if (motionGroupsItem != null)
            {
                foreach (var item in motionGroupsItem.ProjectItems.OfType<OrganizerItem>())
                {
                    if (item.Kind == ProjectItemType.UngroupedAxes)
                        return item;
                }
            }

            return null;
        }

        public OrganizerItem GetMotionGroupItem(ITag tag)
        {
            var motionGroupsItem = GetMotionGroupsItem();
            if (motionGroupsItem != null)
            {
                foreach (var item in motionGroupsItem.ProjectItems.OfType<OrganizerItem>())
                {
                    if (item.AssociatedObject == tag)
                        return item;
                }
            }

            return null;
        }

        private OrganizerItem GetMotionGroupsItem()
        {
            foreach (var item in _root.ProjectItems.OfType<OrganizerItem>())
            {
                if (item.Kind == ProjectItemType.MotionGroups)
                    return item;
            }

            return null;
        }

        public OrganizerItem GetProgramItem(IProgram program)
        {
            return GetProgramItem(_root, program);
        }

        private OrganizerItem GetProgramItem(OrganizerItem rootItem, IProgram program)
        {
            if (rootItem.AssociatedObject == program)
                return rootItem;

            foreach (var item in rootItem.ProjectItems.OfType<OrganizerItem>())
            {
                OrganizerItem routineItem = GetProgramItem(item, program);
                if (routineItem != null)
                    return routineItem;
            }

            return null;
        }

        private OrganizerItem GetProgramItem(OrganizerItems items, IProgram program)
        {
            foreach (var item in items)
            {
                if (item.AssociatedObject == program)
                    return item;
            }

            return null;
        }

        private OrganizerItem GetPowerUpHandlerItem()
        {
            var controllerItem = GetControllerItem();
            if (controllerItem != null)
            {
                foreach (var item in controllerItem.ProjectItems.OfType<OrganizerItem>())
                {
                    if (item.Kind == ProjectItemType.PowerHandler)
                        return item;
                }
            }

            return null;
        }

        private OrganizerItem GetFaultHandlerItem()
        {
            var controllerItem = GetControllerItem();
            if (controllerItem != null)
            {
                foreach (var item in controllerItem.ProjectItems.OfType<OrganizerItem>())
                {
                    if (item.Kind == ProjectItemType.FaultHandler)
                        return item;
                }
            }

            return null;
        }

        private OrganizerItem GetUnscheduledProgramsItem()
        {
            var tasksItem = GetTasksItem();
            if (tasksItem != null)
            {
                foreach (var item in tasksItem.ProjectItems.OfType<OrganizerItem>())
                {
                    if (item.Kind == ProjectItemType.UnscheduledPrograms)
                        return item;
                }
            }

            return null;

        }

        public OrganizerItem GetTaskItem(ITask task)
        {
            var tasksItem = GetTasksItem();
            if (tasksItem != null)
            {
                foreach (var item in tasksItem.ProjectItems.OfType<OrganizerItem>())
                {
                    if (item.AssociatedObject == task)
                        return item;
                }
            }

            return null;
        }

        private OrganizerItem GetTasksItem()
        {
            foreach (var item in _root.ProjectItems.OfType<OrganizerItem>())
            {
                if (item.Kind == ProjectItemType.Tasks)
                    return item;
            }

            return null;
        }

        private OrganizerItem GetControllerItem()
        {
            foreach (var item in _root.ProjectItems.OfType<OrganizerItem>())
            {
                if (item.Kind == ProjectItemType.ControllerModel)
                    return item;
            }

            return null;
        }


        #endregion



        private OrganizerItem CreateControllerItem(IController controller)
        {
            var controllerItem = new ControllerItem(controller);

            var tagsItem = new OrganizerItem()
            {
                Name = "Global Variables",
                Kind = ProjectItemType.ControllerTags,
                AssociatedObject = controller
            };
            controllerItem.ProjectItems.Add(tagsItem);

            var faultHandlerItem = new OrganizerItem()
            {
                Name = "Controller Fault Handler",
                Kind = ProjectItemType.FaultHandler,
                AssociatedObject = controller
            };
            controllerItem.ProjectItems.Add(faultHandlerItem);

            var powerUpHandlerItem = new OrganizerItem()
            {
                Name = "Power-Up Handler",
                Kind = ProjectItemType.PowerHandler,
                AssociatedObject = controller
            };
            controllerItem.ProjectItems.Add(powerUpHandlerItem);

            foreach (IProgram program in controller.Programs)
            {
                if (program.Name == controller.MajorFaultProgram)
                {
                    var programItem = NewProgramItem(program);
                    faultHandlerItem.ProjectItems.Add(programItem);
                }
                else if (program.Name == controller.PowerLossProgram)
                {
                    var programItem = NewProgramItem(program);
                    powerUpHandlerItem.ProjectItems.Add(programItem);
                }
            }
            
            return controllerItem;
        }

        private OrganizerItem CreateAssetsItem(IController controller)
        {
            var assetsItem = new OrganizerItem()
            {
                Name = "Assets",
                Kind = ProjectItemType.Assets
            };

            //Add-On Instructions
            var addOnInstructionsItem = CreateAddOnInstructionsItem(controller);
            assetsItem.ProjectItems.Add(addOnInstructionsItem);

            //Data Types
            var dataTypesItem = CreateDataTypesItem(controller);
            assetsItem.ProjectItems.Add(dataTypesItem);

            //Trends
            var trends = CreateTrendsItem(controller);
            assetsItem.ProjectItems.Add(trends);

            return assetsItem;
        }

        private OrganizerItem CreateTrendsItem(IController controller)
        {
            var trendsItem = new OrganizerItem()
            {
                Name = "Trends",
                Kind = ProjectItemType.Trends,
                IsExpanded = false
            };

            List<ITrend> trends = controller.Trends.ToList();
            trends.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));

            foreach (var trend in trends)
            {
                var item = new TrendItem(trend);
                trendsItem.ProjectItems.Add(item);
            }

            return trendsItem;
        }

        private OrganizerItem CreateAddOnInstructionsItem(IController controller)
        {
            var addOnInstructionsItem = new OrganizerItem()
            {
                Name = "Ud Function Block",
                Kind = ProjectItemType.AddOnInstructions,
                IsExpanded = false
            };

            List<IAoiDefinition> aoiDefinitions = controller.AOIDefinitionCollection.ToList();
            aoiDefinitions.Sort((x, y)
                => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));

            foreach (var item in aoiDefinitions.OfType<AoiDefinition>())
            {
                var aoiItem = new AddOnInstructionItem(item);

                addOnInstructionsItem.ProjectItems.Add(aoiItem);

                CollectionChangedEventManager.AddHandler(item.Routines, OnRoutinesChanged);
            }

            return addOnInstructionsItem;
        }

        private OrganizerItem CreateDataTypesItem(IController controller)
        {
            var dataTypesItem = new OrganizerItem()
            {
                Name = "Data Types",
                Kind = ProjectItemType.DataTypes,
                IsExpanded = true
            };

            //User Defined
            var userDefinedsItem = CreateUserDefinedsItem(controller);
            dataTypesItem.ProjectItems.Add(userDefinedsItem);

            //Strings
            var stringsItem = CreateStringsItem(controller);
            dataTypesItem.ProjectItems.Add(stringsItem);

            //Add-On-Defined
            var addOnDefinedItem = CreateAddOnDefinedItem(controller);
            dataTypesItem.ProjectItems.Add(addOnDefinedItem);

            //Predefined
            var predefinedItem = CreatePredefinedItem(controller);
            dataTypesItem.ProjectItems.Add(predefinedItem);

            //Module Defined
            var moduleDefinedsItem = CreateModuleDefinedsItem(controller);
            dataTypesItem.ProjectItems.Add(moduleDefinedsItem);

            return dataTypesItem;
        }

        private OrganizerItem CreateModuleDefinedsItem(IController controller)
        {
            var moduleDefinedItem = new OrganizerItem()
            {
                Name = "Module-Defined",
                Kind = ProjectItemType.ModuleDefineds,
                IsExpanded = false
            };

            foreach (var dataType in controller.DataTypes.OrderBy(x => x.Name))
            {
                if (dataType is ModuleDefinedDataType)
                {
                    var item = new OrganizerItem()
                    {
                        Name = dataType.Name,
                        Kind = ProjectItemType.ModuleDefined,
                        AssociatedObject = dataType
                    };

                    moduleDefinedItem.ProjectItems.Add(item);
                }
            }

            return moduleDefinedItem;
        }

        private OrganizerItem CreatePredefinedItem(IController controller)
        {
            var predefinedItem = new OrganizerItem()
            {
                Name = "Predefined",
                Kind = ProjectItemType.Predefineds,
                IsExpanded = false
            };

            foreach (var dataType in controller.DataTypes.OrderBy(x => x.Name))
            {
                if (dataType.Name == "BOOL:INT"
                    || dataType.Name == "BOOL:LINT"
                    || dataType.Name == "BOOL:DINT"
                    || dataType.Name == "BOOL:SINT")
                {
                    continue;
                }

                if (dataType.IsPredefinedType)
                {
                    var item = new OrganizerItem()
                    {
                        Name = dataType.Name,
                        Kind = ProjectItemType.Predefined,
                        AssociatedObject = dataType
                    };
                    predefinedItem.ProjectItems.Add(item);
                }

            }

            return predefinedItem;
        }

        private OrganizerItem CreateAddOnDefinedItem(IController controller)
        {
            var addOnDefinedsItem = new OrganizerItem()
            {
                Name = "AddOnDefined",
                Kind = ProjectItemType.AddOnDefineds,
                IsExpanded = false
            };

            foreach (var definition in controller.AOIDefinitionCollection)
            {
                var item = new AddOnDefinedItem(definition);
                addOnDefinedsItem.ProjectItems.Add(item);
            }

            return addOnDefinedsItem;
        }

        private OrganizerItem CreateStringsItem(IController controller)
        {
            var stringsItem = new OrganizerItem()
            {
                Name = "Strings",
                Kind = ProjectItemType.Strings,
                IsExpanded = false
            };

            var dataTypes = controller.DataTypes.OfType<UserDefinedDataType>().ToList();
            dataTypes.Sort((x, y)
                => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));

            foreach (var dataType in dataTypes)
            {
                if (dataType.IsStringType)
                {
                    var item = new StringTypeItem(dataType);
                    stringsItem.ProjectItems.Add(item);
                }
            }

            return stringsItem;
        }

        private OrganizerItem CreateUserDefinedsItem(IController controller)
        {
            var userDefinedsItem = new OrganizerItem()
            {
                Name = "User-Defined",
                Kind = ProjectItemType.UserDefineds,
                IsExpanded = false
            };

            var dataTypes = controller.DataTypes.OfType<UserDefinedDataType>().ToList();
            dataTypes.Sort((x, y)
                => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));

            foreach (var dataType in dataTypes)
            {
                if (!dataType.IsStringType)
                {
                    var item = new UserDefinedItem(dataType);
                    userDefinedsItem.ProjectItems.Add(item);
                }
            }

            return userDefinedsItem;
        }

        private OrganizerItem CreateLogicalModelItem(IController controller)
        {
            var logicalModelItem = new OrganizerItem()
            {
                DisplayName = "Logical Model",
                Kind = ProjectItemType.LogicalModelView
            };

            return logicalModelItem;
        }

        private OrganizerItem CreateIOConfigurationItem(IController controller)
        {
            var ioConfigurationItem = new OrganizerItem()
            {
                Name = "I/O Configuration",
                Kind = ProjectItemType.IOConfiguration
            };

            // PointIO
            OrganizerItem pointIOItem = CreatePointIOItem(controller);
            if (pointIOItem != null)
                ioConfigurationItem.ProjectItems.Add(pointIOItem);

            // Ethernet
            List<OrganizerItem> ethernetItems = CreateEthernetItem(controller);
            if (ethernetItems != null && ethernetItems.Count > 0)
            {
                foreach (var item in ethernetItems)
                {
                    ioConfigurationItem.ProjectItems.Add(item);
                }
            }

            return ioConfigurationItem;
        }

        private OrganizerItem CreateMotionGroupsItem(IController controller)
        {
            var motionGroupsItem = new OrganizerItem()
            {
                Name = "Motion Groups",
                Kind = ProjectItemType.MotionGroups,
                IsExpanded = false
            };

            // Motion Group
            OrganizerItem motionGroupItem = null;
            foreach (ITag tag in controller.Tags)
            {
                if (!tag.IsAlias && tag.DataTypeInfo.DataType.IsMotionGroupType)
                {
                    motionGroupItem = new MotionGroupItem(tag);
                    break;
                }
            }

            if (motionGroupItem != null)
            {
                motionGroupsItem.ProjectItems.Add(motionGroupItem);
            }

            // Ungrouped Axes
            var ungroupedAxesItem = new OrganizerItem()
            {
                Name = "Ungrouped Axes",
                Kind = ProjectItemType.UngroupedAxes,
            };
            motionGroupsItem.ProjectItems.Add(ungroupedAxesItem);

            // add axis
            AddAxisItems(controller, motionGroupItem, ungroupedAxesItem);
            // add coordinate system
            AddCoordinateSystemItems(motionGroupItem, ungroupedAxesItem);

            return motionGroupsItem;
        }

        private OrganizerItem CreateTasksItem(IController controller)
        {
            var tasksItem = new OrganizerItem()
            {
                Name = "Tasks",
                Kind = ProjectItemType.Tasks
            };

            var taskList = controller.Tasks.ToList();
            taskList.Sort((x, y)
                => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));

            foreach (ITask task in taskList)
            {
                var taskItem = new TaskItem(task);
                tasksItem.ProjectItems.Add(taskItem);

                // program
                foreach (IProgram program in controller.Programs)
                {
                    if (program.ParentTask == task)
                    {
                        var programItem = NewProgramItem(program);
                        taskItem.ProjectItems.Add(programItem);
                    }
                }
            }

            var unscheduledItem = new OrganizerItem()
            {
                Name = "Unscheduled",
                Kind = ProjectItemType.UnscheduledPrograms,
                IsExpanded = false
            };

            tasksItem.ProjectItems.Add(unscheduledItem);

            List<IProgram> unscheduledPrograms = new List<IProgram>();
            foreach (IProgram program in controller.Programs)
            {
                if (program.ParentTask == null)
                {
                    if (program.Name != controller.MajorFaultProgram && program.Name != controller.PowerLossProgram)
                    {
                        unscheduledPrograms.Add(program);
                    }

                }
            }

            unscheduledPrograms.Sort((x, y)
                => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));

            foreach (var program in unscheduledPrograms)
            {
                unscheduledItem.ProjectItems.Add(NewProgramItem(program));
            }

            return tasksItem;
        }

        private OrganizerItem CreatePointIOItem(IController controller)
        {
            OrganizerItem pointIOItem = null;

            LocalModule localModule = controller.DeviceModules["Local"] as LocalModule;
            var port = localModule?.GetFirstPort(PortType.PointIO);
            if (port != null)
            {
                pointIOItem = new OrganizerItem()
                {
                    Name = "Basic I/O",
                    Kind = ProjectItemType.Bus,
                    AssociatedObject = localModule
                };

                AddPointIOModuleItems(controller, pointIOItem, port);
            }

            return pointIOItem;
        }

        private void AddPointIOModuleItems(IController controller, OrganizerItem pointIOItem, Port parentPort)
        {
            // Local
            LocalModule localModule = controller.DeviceModules["Local"] as LocalModule;
            if (localModule != null)
            {
                var localModuleItem = new PointIOItem(localModule, parentPort.Type);

                pointIOItem.ProjectItems.Add(localModuleItem);
            }

            // Embedded IO , must have only one embedded io
            var embeddedIOItem = new OrganizerItem()
            {
                Name = "Embedded I/O",
                Kind = ProjectItemType.EmbeddedIO,
                AssociatedObject = localModule,
            };

            pointIOItem.ProjectItems.Add(embeddedIOItem);

            // Expansion IO
            var expansionIOItem = new ExpansionIORootItem(localModule, parentPort.Type);
            pointIOItem.ProjectItems.Add(expansionIOItem);

            List<DeviceModule> embeddedDevices = new List<DeviceModule>();
            Dictionary<string, DeviceModule> expansionDeviceDictionary = new Dictionary<string, DeviceModule>();

            foreach (var module in controller.DeviceModules)
            {
                DeviceModule deviceModule = module as DeviceModule;
                if (deviceModule != null)
                {
                    if (string.Equals(deviceModule.Name, "Local", StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (!string.Equals(deviceModule.ParentModule?.Name, "Local", StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (deviceModule.ParentModPortId != parentPort.Id)
                        continue;

                    if (deviceModule.CatalogNumber.StartsWith("Embedded", StringComparison.OrdinalIgnoreCase))
                    {
                        embeddedDevices.Add(deviceModule);
                    }
                    else
                    {
                        var port = deviceModule.GetFirstPort(parentPort.Type);
                        expansionDeviceDictionary.Add(port.Address, deviceModule);
                    }

                }
            }

            Debug.Assert(embeddedDevices.Count > 0);

            foreach (var module in embeddedDevices)
            {
                var moduleItem = new PointIOItem(module, parentPort.Type);
                embeddedIOItem.ProjectItems.Add(moduleItem);
            }

            var expansionDevices = expansionDeviceDictionary.OrderBy(o => int.Parse(o.Key)).ToList();

            foreach (var module in expansionDevices)
            {
                var moduleItem = new PointIOItem(module.Value, parentPort.Type);
                expansionIOItem.ProjectItems.Add(moduleItem);
            }

        }

        private void AddPointIOModuleItems(
            IController controller,
            DeviceModule parentModule,
            OrganizerItem pointIOItem,
            Port parentPort)
        {
            Dictionary<string, DeviceModule> expansionDeviceDictionary = new Dictionary<string, DeviceModule>();

            foreach (var module in controller.DeviceModules)
            {
                DeviceModule deviceModule = module as DeviceModule;
                if (deviceModule != null)
                {
                    if (deviceModule.ParentModule != parentModule)
                        continue;

                    if (deviceModule.ParentModPortId != parentPort.Id)
                        continue;

                    var port = deviceModule.GetFirstPort(parentPort.Type);
                    expansionDeviceDictionary.Add(port.Address, deviceModule);
                }
            }

            var expansionDevices = expansionDeviceDictionary.OrderBy(o => int.Parse(o.Key)).ToList();

            if (expansionDevices.Count > 0)
            {
                // Local
                var localItem = new PointIOItem(parentModule, parentPort.Type);
                pointIOItem.ProjectItems.Add(localItem);

                foreach (var module in expansionDevices)
                {
                    var moduleItem = new PointIOItem(module.Value, parentPort.Type);
                    pointIOItem.ProjectItems.Add(moduleItem);
                }
            }
        }

        private List<OrganizerItem> CreateEthernetItem(IController controller)
        {
            List<OrganizerItem> result = new List<OrganizerItem>();

            OrganizerItem ethernetItem = null;

            LocalModule localModule = controller.DeviceModules["Local"] as LocalModule;

            //TODO(gjc): need edit here
            if (controller.EtherNetIPMode != "A1/A2: Dual-IP")
            {
                var port = localModule?.GetFirstPort(PortType.Ethernet);

                if (port != null)
                {
                    ethernetItem = new OrganizerItem()
                    {
                        Name = "Ethernet:",
                        Kind = ProjectItemType.Ethernet,
                        AssociatedObject = localModule
                    };

                    AddEthernetModuleItems(controller, ethernetItem, port);

                    result.Add(ethernetItem);
                }
            }
            else
            {
                var ports = localModule?.GetPorts(PortType.Ethernet);
                if (ports != null && ports.Count == 2)
                {
                    //A1
                    ethernetItem = new OrganizerItem()
                    {
                        Name = "A1,Ethernet",
                        Kind = ProjectItemType.Ethernet,
                        AssociatedObject = localModule
                    };

                    AddEthernetModuleItems(controller, ethernetItem, ports[0]);
                    result.Add(ethernetItem);

                    //A2
                    ethernetItem = new OrganizerItem()
                    {
                        Name = "A2,Ethernet",
                        Kind = ProjectItemType.Ethernet,
                        AssociatedObject = localModule
                    };

                    AddEthernetModuleItems(controller, ethernetItem, ports[1]);
                    result.Add(ethernetItem);
                }
            }

            return result;
        }

        private OrganizerItem NewProgramItem(IProgram program)
        {
            var programItem = new ProgramItem(program);

            // Tags
            var programTagsItem = new OrganizerItem
            {
                Name = "Parameters And Local Tag",
                Kind = ProjectItemType.ProgramTags,
                AssociatedObject = program
            };
            programItem.ProjectItems.Add(programTagsItem);

            // routine
            var main = program.Routines[program.MainRoutineName];
            var fault = program.Routines[program.FaultRoutineName];
            if (main != null)
            {
                var routineItem = new RoutineItem(main);
                programItem.ProjectItems.Add(routineItem);
            }

            if (fault != null)
            {
                var routineItem = new RoutineItem(fault);
                programItem.ProjectItems.Add(routineItem);
            }

            // sort
            List<IRoutine> routines = program.Routines.ToList();
            routines.Sort((x, y)
                => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));

            foreach (var routine in routines)
            {
                if (routine == main || routine == fault) continue;

                var routineItem = new RoutineItem(routine);
                programItem.ProjectItems.Add(routineItem);
            }

            CollectionChangedEventManager.AddHandler(program.Routines, OnRoutinesChanged);

            return programItem;
        }

        private void AddEthernetModuleItems(
            IController controller, OrganizerItem ethernetItem, Port parentPort)
        {
            LocalModule localModule = controller.DeviceModules["Local"] as LocalModule;
            if (localModule != null)
            {
                var localModuleItem = new EthernetItem(localModule, parentPort.Type);
                ethernetItem.ProjectItems.Add(localModuleItem);
            }

            // sort by address
            List<DeviceModule> ethernetDevices = new List<DeviceModule>();

            foreach (var module in controller.DeviceModules)
            {
                DeviceModule deviceModule = module as DeviceModule;
                if (deviceModule != null)
                {
                    if (!string.Equals(deviceModule.ParentModule.Name, "Local", StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (deviceModule.ParentModPortId != parentPort.Id)
                        continue;

                    ethernetDevices.Add(deviceModule);
                }
            }

            ethernetDevices.Sort(CompareEthernetAddress);

            foreach (var device in ethernetDevices)
            {
                var deviceModuleItem = new EthernetItem(device, parentPort.Type);
                ethernetItem.ProjectItems.Add(deviceModuleItem);

                // PointIO
                var port = device.GetFirstPort(PortType.PointIO);
                if (port != null && !port.Upstream)
                {
                    var pointIOItem = new PointIORootItem(device);
                    deviceModuleItem.ProjectItems.Add(pointIOItem);

                    AddPointIOModuleItems(controller, device, pointIOItem, port);
                }
            }

        }

        // ReSharper disable UnusedParameter.Local
        private void AddCoordinateSystemItems(
            OrganizerItem motionGroupItem,
            OrganizerItem ungroupedAxesItem)
        {
            //TODO(gjc): add code here

        }
        // ReSharper restore UnusedParameter.Local

        private void AddAxisItems(
            IController controller,
            OrganizerItem motionGroupItem, OrganizerItem ungroupedAxesItem)
        {
            List<string> axisNameList = new List<string>();
            foreach (var tag in controller.Tags)
            {
                if (!tag.IsAlias && tag.DataTypeInfo.DataType.IsAxisType)
                    axisNameList.Add(tag.Name);
            }

            axisNameList.Sort((x, y) => string.Compare(x, y, StringComparison.OrdinalIgnoreCase));

            foreach (var axisName in axisNameList)
            {
                Tag axis = controller.Tags[axisName] as Tag;
                AxisCIPDrive axisCIPDrive = axis?.DataWrapper as AxisCIPDrive;
                if (axisCIPDrive != null)
                {
                    var axisCIPDriveItem = new AxisItem(axis);

                    if (motionGroupItem != null
                        && axisCIPDrive.AssignedGroup == motionGroupItem.AssociatedObject)
                    {
                        motionGroupItem.ProjectItems.Add(axisCIPDriveItem);
                    }
                    else
                    {
                        ungroupedAxesItem.ProjectItems.Add(axisCIPDriveItem);
                    }
                }

                AxisVirtual axisVirtual = axis?.DataWrapper as AxisVirtual;
                if (axisVirtual != null)
                {
                    var axisVirtualItem = new AxisItem(axis);

                    if (motionGroupItem != null
                        && axisVirtual.AssignedGroup == motionGroupItem.AssociatedObject)
                    {
                        motionGroupItem.ProjectItems.Add(axisVirtualItem);
                    }
                    else
                    {
                        ungroupedAxesItem.ProjectItems.Add(axisVirtualItem);
                    }
                }
            }
        }

        private void ResetSelectedItems()
        {
            List<OrganizerItem> items = SelectedItems.ToList();

            foreach (var item in items)
            {
                if (!IsItemInTheTree(item))
                    SelectedItems.Remove(item);
            }
        }

        private bool IsItemInTheTree(IProjectItem item)
        {
            if (item == null)
                return false;

            if (item == _root)
                return true;

            OrganizerItems items = item.Collection as OrganizerItems;
            if (items == null)
                return false;

            return IsItemInTheTree(items.Parent);
        }

        private int CompareEthernetAddress(DeviceModule deviceModule0, DeviceModule deviceModule1)
        {
            string name0 = CompareNameConverter.EthernetAddressToCompareName(deviceModule0);
            string name1 = CompareNameConverter.EthernetAddressToCompareName(deviceModule1);

            return string.Compare(name0, name1, StringComparison.OrdinalIgnoreCase);
        }
    }
}
