using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.Interfaces.Notification;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.OrganizerPackage.Utilities;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIInterfaces.Editor;
using ICSStudio.UIInterfaces.GlobalClipboard;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.UIInterfaces.UI;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json.Linq;
using IRoutine = ICSStudio.Interfaces.Common.IRoutine;
using Program = ICSStudio.SimpleServices.Common.Program;
using Task = ICSStudio.SimpleServices.Common.CTask;

namespace ICSStudio.OrganizerPackage.Commands
{
    /// <summary>
    /// menu command, always visible
    /// </summary>
    internal sealed class ClipboardCommand
    {
        private readonly Package _package;

        private ClipboardCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            _package = package;

            OleMenuCommandService commandService =
                ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.cutCommand);
                var menuItem = new OleMenuCommand(DoCut, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.copyCommand);
                menuItem = new OleMenuCommand(DoCopy, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.pasteCommand);
                menuItem = new OleMenuCommand(DoPaste, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet,
                    PackageIds.pasteWithConfigurationCommand);
                menuItem = new OleMenuCommand(DoPaste, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.deleteCommand);
                menuItem = new OleMenuCommand(DoDelete, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
                commandService.AddCommand(menuItem);

                //调试状态下，在Task下的Edit Tags中新增或修改数据的DataType为实轴时，会因为这里崩掉重启，暂时注释掉
                //InputManager.Current.PostNotifyInput += Current_PostNotifyInput;
            }
        }

        //private void Current_PostNotifyInput(object sender, NotifyInputEventArgs e)
        //{
        //    ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
        //    {
        //        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        //        if (e.StagingItem.Input.RoutedEvent == Keyboard.GotKeyboardFocusEvent)
        //        {
        //            KeyboardFocusChangedEventArgs focusArgs = (KeyboardFocusChangedEventArgs)e.StagingItem.Input;
        //            CurrentObject.GetInstance().CurrentControl = focusArgs.NewFocus;
        //            UnAttach(CurrentObject.GetInstance().PreviousControl as IInputElement);
        //            Attach(CurrentObject.GetInstance().CurrentControl as IInputElement);
        //            FileCommand.Instance.UpdateUI();
        //        }
        //    });

        //}

        private void UnAttach(IInputElement element)
        {
            var textBox = element as TextBox;
            if (textBox != null)
            {
                textBox.SelectionChanged -= TextBox_SelectionChanged;
            }

            var dataGrid = element as DataGrid;
            if (dataGrid != null)
            {
                dataGrid.SelectionChanged -= DataGrid_SelectionChanged;
            }
        }

        private void Attach(IInputElement element)
        {
            var textBox = element as TextBox;
            if (textBox != null)
            {
                textBox.SelectionChanged += TextBox_SelectionChanged;
            }

            var dataGrid = element as DataGrid;
            if (dataGrid != null)
            {
                dataGrid.SelectionChanged += DataGrid_SelectionChanged;
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            FileCommand.Instance.UpdateUI();
        }

        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            FileCommand.Instance.UpdateUI();
        }

        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            //TODO(gjc): need edit here
            OleMenuCommand menuCommand = sender as OleMenuCommand;
            if (menuCommand == null) return;
            switch (menuCommand.CommandID.ID)
            {
                case PackageIds.cutCommand:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Cut");
                    break;
                case PackageIds.copyCommand:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Copy");
                    break;
                case PackageIds.pasteCommand:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Paste");
                    break;
                case PackageIds.pasteWithConfigurationCommand:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Paste With Configuration...");
                    break;
                case PackageIds.deleteCommand:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Delete");
                    break;
            }

            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();
            if (selectedProjectItem == null)
            {
                menuCommand.Enabled = false;
                return;
            }

            var itemType = selectedProjectItem.Kind;

            menuCommand.Enabled = false;

            switch (menuCommand.CommandID.ID)
            {
                case PackageIds.cutCommand:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Cut");
                    if (itemType == ProjectItemType.ControllerModel
                        ||itemType == ProjectItemType.AddOnDefined
                        || itemType == ProjectItemType.ControllerTags
                        || itemType == ProjectItemType.ProgramTags)
                    {
                        menuCommand.Visible = false;
                    }
                    else
                    {
                        menuCommand.Visible = true;
                        menuCommand.Enabled = CanCut(selectedProjectItem);
                    }

                    break;
                case PackageIds.copyCommand:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Copy");
                    if (itemType == ProjectItemType.ControllerModel
                        || itemType == ProjectItemType.AddOnDefined
                        || itemType == ProjectItemType.ControllerTags
                        || itemType == ProjectItemType.ProgramTags)
                    {
                        menuCommand.Visible = false;
                    }
                    else
                    {
                        menuCommand.Visible = true;
                        menuCommand.Enabled = CanCopy(selectedProjectItem);
                    }
                    break;
                case PackageIds.pasteCommand:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Paste");
                    if (itemType == ProjectItemType.ControllerModel
                        || itemType == ProjectItemType.AddOnDefined
                        || itemType == ProjectItemType.ControllerTags
                        || itemType == ProjectItemType.ProgramTags)
                    {
                        menuCommand.Visible = false;
                    }
                    else if (itemType == ProjectItemType.Tasks)
                    {
                        if (selectedProjectItem.ProjectItems.Count >= 17)
                        {
                            menuCommand.Enabled = false;
                        }
                        else
                        {
                            menuCommand.Visible = true;
                            menuCommand.Enabled = CanPaste(selectedProjectItem);
                        }
                    }
                    else if (itemType == ProjectItemType.Task)
                    {
                        if (selectedProjectItem.ProjectItems.Count >= 1000)
                        {
                            menuCommand.Enabled = false;
                        }
                        else
                        {
                            menuCommand.Visible = true;
                            menuCommand.Enabled = CanPaste(selectedProjectItem);
                        }
                    }
                    else
                    {
                        menuCommand.Visible = true;
                        menuCommand.Enabled = CanPaste(selectedProjectItem);
                    }

                    break;
                case PackageIds.pasteWithConfigurationCommand:
                    menuCommand.Text = "English".Equals(LanguageInfo.CurrentLanguage)
                        ? "Paste With Configuration..."
                        : "粘贴(含配置)...";
                    if (itemType == ProjectItemType.ControllerModel
                        || itemType == ProjectItemType.AddOnDefined
                        || itemType == ProjectItemType.ControllerTags
                        || itemType == ProjectItemType.ProgramTags)
                    {
                        menuCommand.Visible = false;
                    }
                    else
                    {
                        menuCommand.Visible = true;
                    }

                    break;
                case PackageIds.deleteCommand:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Delete");
                    if (itemType == ProjectItemType.ControllerModel
                        || itemType == ProjectItemType.AddOnDefined
                        || itemType == ProjectItemType.ControllerTags
                        || itemType == ProjectItemType.ProgramTags)
                    {
                        menuCommand.Visible = false;
                    }
                    else
                    {
                        menuCommand.Visible = true;
                        if (selectedProjectItem.Kind == ProjectItemType.Bus)
                            menuCommand.Enabled = false;
                        else
                            menuCommand.Enabled = CanExecuteDeleteCommand(ServiceProvider
                                .GetSelectedProjectItems()
                                .Select(p => p.AssociatedObject as IBaseComponent).ToList());
                        //menuCommand.Enabled = CanExecuteDeleteCommand(selectedProjectItem.AssociatedObject);
                    }

                    break;
            }
        }

        public static ClipboardCommand Instance { get; private set; }

        private IServiceProvider ServiceProvider => _package;

        public static void Initialize(Package package)
        {
            Instance = new ClipboardCommand(package);
        }

        public bool CanCopy(IProjectItem selectedProjectItem)
        {
            var globalClipboard = CurrentObject.GetInstance().Current as IGlobalClipboard;
            if (globalClipboard != null)
            {
                return globalClipboard.CanCut();
            }

            if (selectedProjectItem == null) return false;
            if (selectedProjectItem.AssociatedObject is LocalModule ||
                ((selectedProjectItem.AssociatedObject as IDeviceModule)?.IsEmbeddedIO() ?? false) ||
                selectedProjectItem.Kind == ProjectItemType.FaultHandler ||
                selectedProjectItem.Kind == ProjectItemType.PowerHandler ||
                selectedProjectItem.Kind == ProjectItemType.ControllerTags ||
                selectedProjectItem.Kind == ProjectItemType.Bus ||
                selectedProjectItem.Kind == ProjectItemType.ProgramTags ||
                selectedProjectItem.Kind == ProjectItemType.ModuleDefined ||
                selectedProjectItem.Kind == ProjectItemType.Predefined)
            {
                return false;
            }

            var obj = selectedProjectItem.AssociatedObject as IDeviceModule;
            if (obj != null && obj.IsEmbeddedIO())
                return false;

            //var module = selectedProjectItem.AssociatedObject as DeviceModule;
            if (selectedProjectItem.DisplayName?.StartsWith("[0]") ?? false)
            {
                return false;
            }

            var routine = selectedProjectItem.AssociatedObject as IRoutine;
            if (routine != null)
            {
                if (routine.ParentCollection.ParentProgram is AoiDefinition) return false;
            }

            return selectedProjectItem.AssociatedObject != null;
        }

        private void DoCut(object sender, EventArgs e)
        {
            //var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();
            //if (CanExecuteDeleteCommand(selectedProjectItem.AssociatedObject))
            Cut();
        }

        public void Cut()
        {
            var globalClipboard = CurrentObject.GetInstance().Current as IGlobalClipboard;
            if (globalClipboard != null)
            {
                globalClipboard.DoCut();
                return;
            }

            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();
            var selectedProjectItems = ServiceProvider.GetSelectedProjectItems();
            var program = selectedProjectItem.AssociatedObject as Program;
            if (program != null)
            {
                bool result = Delete(false);
                if (result)
                {
                    if (selectedProjectItems != null && selectedProjectItems.Count > 0)
                        _pastedObject = selectedProjectItems.Select(p => p.AssociatedObject as IBaseComponent).ToList();
                    Clipboard.SetDataObject(program.Name);
                }
            }

            var routine = selectedProjectItem.AssociatedObject as IRoutine;
            if (routine != null)
            {
                bool result = Delete(false);
                if (result)
                {
                    _pastedObject = routine;
                    Clipboard.SetDataObject(routine.Name);
                }
            }

            var axis = selectedProjectItem.AssociatedObject as ITag;
            if (axis != null)
            {
                bool result = Delete(false);
                if (result)
                {
                    _pastedObject = axis;
                    Clipboard.SetDataObject(axis.Name);
                }
            }

            var aoi = selectedProjectItem.AssociatedObject as AoiDefinition;
            if (aoi != null)
            {
                bool result = Delete(false);
                if (result)
                {
                    if (selectedProjectItems != null && selectedProjectItems.Count > 0)
                        _pastedObject = selectedProjectItems.Select(p => p.AssociatedObject as IBaseComponent).ToList();
                    Clipboard.SetDataObject(aoi.Name);
                }
            }

            var udt = selectedProjectItem.AssociatedObject as UserDefinedDataType;
            if (udt != null)
            {
                bool result = Delete(false);
                if (result)
                {
                    if (selectedProjectItems != null && selectedProjectItems.Count > 0)
                        _pastedObject = selectedProjectItems.Select(p => p.AssociatedObject as IBaseComponent).ToList();
                    Clipboard.SetDataObject(udt.Name);
                }
            }

            var module = selectedProjectItem.AssociatedObject as DeviceModule;
            if (module != null)
            {
                bool result = Delete(false);
                if (result)
                {
                    _pastedObject = module;
                    Clipboard.SetDataObject(module.Name);
                }
            }

            var trend = selectedProjectItem.AssociatedObject as ITrend;
            if (trend != null)
            {
                bool result = Delete(false);
                if (result)
                {
                    if (selectedProjectItems != null && selectedProjectItems.Count > 0)
                        _pastedObject = selectedProjectItems.Select(p => p.AssociatedObject as IBaseComponent).ToList();
                    Clipboard.SetDataObject(trend.Name);
                }
            }
        }

        public void Delete()
        {
            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();

            var program = selectedProjectItem.AssociatedObject as Program;
            if (program != null)
            {
                bool result = Delete(true);
            }

            var task = selectedProjectItem.AssociatedObject as Task;
            if (task != null)
            {
                bool result = Delete(true);
            }

            var routine = selectedProjectItem.AssociatedObject as IRoutine;
            if (routine != null)
            {
                bool result = Delete(true);
            }

            var trend = selectedProjectItem.AssociatedObject as ITrend;
            if (trend != null)
            {
                bool result = Delete(true);
            }

            if (selectedProjectItem.AssociatedObject is AoiDefinition ||
                selectedProjectItem.AssociatedObject is UserDefinedDataType)
            {
                Delete(true);
            }

            var module = selectedProjectItem.AssociatedObject as DeviceModule;
            if (module != null)
            {
                bool result = Delete(true);
            }

            var motionGroup = selectedProjectItem.AssociatedObject as MotionGroup;
            if (motionGroup != null)
            {
                bool result = Delete(true);
            }

            var axis = selectedProjectItem.AssociatedObject as Tag;
            if (axis != null)
            {
                if ((axis.DataTypeInfo.DataType is AXIS_VIRTUAL) ||
                    (axis.DataTypeInfo.DataType is AXIS_CIP_DRIVE) ||
                    (axis.DataTypeInfo.DataType is MOTION_GROUP))
                {
                    bool result = Delete(true);
                }
            }
        }

        private void DoCopy(object sender, EventArgs e)
        {
            Copy();
        }

        public void Copy()
        {
            var globalClipboard = CurrentObject.GetInstance().Current as IGlobalClipboard;
            if (globalClipboard != null)
            {
                globalClipboard.DoCopy();
                return;
            }

            var selectedProjectItems = ServiceProvider.GetSelectedProjectItems();
            if (selectedProjectItems != null && selectedProjectItems.Count > 1)
            {
                var os = selectedProjectItems.Select(p => p.AssociatedObject as IBaseComponent).ToList();
                _pastedObject = os;
            }
            else
            {
                var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();
                var o = selectedProjectItem.AssociatedObject as IBaseComponent;
                _pastedObject = o;
                //Clipboard.SetDataObject(o?.Name ?? "");
            }
        }

        private void DoPaste(object sender, EventArgs e)
        {
            //var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();
            //if (CanExecutePasteCommand(selectedProjectItem.AssociatedObject))
            Paste();
        }

        public void Paste()
        {
            var globalClipboard = CurrentObject.GetInstance().Current as IGlobalClipboard;
            if (globalClipboard != null)
            {
                globalClipboard.DoPaste();
                return;
            }

            //TODO(gjc): need remove reset
            var service = Package.GetGlobalService(typeof(SStudioUIService)) as IStudioUIService;
            service?.DetachController();

            var projectInfoService = Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            var selectedProjectItems = ServiceProvider.GetSelectedProjectItems();
            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();
            if (selectedProjectItems != null && selectedProjectItems.Count > 1)
                selectedProjectItem = selectedProjectItems[0];

            if (selectedProjectItem.Kind == ProjectItemType.Tasks && _pastedObject is Task)
            {
                var controller = Controller.GetInstance();
                var config = (_pastedObject as Task)?.ConvertToJObject().DeepClone();
                var tasks = controller.Tasks.Select(t => t.Name).ToList();
                var name = Utils.Utils.GetNotDuplicateName((string)config["Name"], tasks);
                config["Name"] = name;
                var continueTask = controller.Tasks.FirstOrDefault(t => t.Type == TaskType.Continuous);
                if (continueTask != null)
                {
                    config["Type"] = 2;
                }

                config["SchededPrograms"] = null;
                controller.AddTask(config);
            }

            var task = selectedProjectItem.AssociatedObject as Task;
            if (task != null && ((_pastedObject is IProgram) || _pastedObject.GetType().IsGenericType))
            {
                var controller = Controller.GetInstance();
                var programs = controller.Programs.Select(p => p.Name).ToList();

                if (_pastedObject is IProgram)
                {
                    var config = (_pastedObject as Program)?.ConvertToJObject(false).DeepClone();
                    var name = Utils.Utils.GetNotDuplicateName((string)config["Name"], programs, 0);
                    config["Name"] = name;
                    var program1 = controller.AddProgram(config);
                    program1.ParentTask = task;
                    foreach (var routine in program1.Routines)
                    {
                        routine.IsError = true;
                        projectInfoService?.Verify(routine);
                    }
                }

                if (_pastedObject.GetType().IsGenericType)
                {
                    foreach (var p in (List<IBaseComponent>)_pastedObject)
                    {
                        var to = p as Program;
                        var config = to?.ConvertToJObject(false).DeepClone();
                        var name = Utils.Utils.GetNotDuplicateName((string)config["Name"], programs);
                        config["Name"] = name;
                        var program1 = controller.AddProgram(config);
                        program1.ParentTask = task;
                        foreach (var routine in program1.Routines)
                        {
                            routine.IsError = true;
                            projectInfoService?.Verify(routine);
                        }
                    }
                }
            }

            var program = selectedProjectItem.AssociatedObject as IProgram;
            if (program != null && _pastedObject is IRoutine)
            {
                var controller = Controller.GetInstance();
                var pastedRoutine = _pastedObject as IRoutine;
                var config = pastedRoutine.ConvertToJObject(false).DeepClone();
                var routines = program.Routines.Select(p => p.Name).ToList();
                var name = Utils.Utils.GetNotDuplicateName((string)config["Name"], routines, 0);
                config["Name"] = name;

                var r = controller.CreateRoutine((JObject)config);
                ((Program)program).AddRoutine(r);
                r.IsError = true;
                projectInfoService?.Verify(r);
            }

            if ((selectedProjectItem.Kind == ProjectItemType.MotionGroup ||
                 selectedProjectItem.Kind == ProjectItemType.UngroupedAxes) &&
                (_pastedObject as Tag)?.DataTypeInfo.DataType is AXIS_COMMON)
            {
                var controller = Controller.GetInstance();
                var config = (_pastedObject as Tag)?.ConvertToJObject().DeepClone() ;
                var tags = controller.Tags.Select(t => t.Name).ToList();
                var name = Utils.Utils.GetNotDuplicateName((string)config["Name"], tags, 0);
                config["Name"] = name;
                var coll = (TagCollection)controller.Tags;

                //
                if ((string)config["DataType"] == "AXIS_CIP_DRIVE")
                {
                    JObject axisConfig = config as JObject;
                    if (axisConfig != null && axisConfig.ContainsKey("Parameters"))
                    {
                        JObject parameters = axisConfig["Parameters"] as JObject;
                        if (parameters != null && parameters.ContainsKey("MotionModule"))
                        {
                            parameters["MotionModule"] = "<NA>";
                        }

                        if (parameters != null && parameters.ContainsKey("AxisID"))
                        {
                            parameters["AxisID"] = controller.CreateAxisID();
                        }
                    }
                }

                //

                var tag = TagsFactory.CreateTag(coll, config);

                coll.AddTag(tag, false, false);

                var c = tag.DataWrapper as AxisCIPDrive;
                if (c != null)
                {
                    c.PostLoadJson();
                }

                var v = tag.DataWrapper as AxisVirtual;
                if (v != null)
                {
                   v.PostLoadJson();
                }

                Notifications.Publish(new MessageData() { Object = tag, Type = MessageData.MessageType.AddTag });
            }

            if (selectedProjectItem.Kind == ProjectItemType.AddOnInstructions &&
                ((_pastedObject is AoiDefinition) || _pastedObject.GetType().IsGenericType))
            {
                var controller = Controller.GetInstance();
                var aois = controller.AOIDefinitionCollection.Select(a => a.Name).ToList();

                if (_pastedObject is AoiDefinition)
                {
                    var config = (_pastedObject as AoiDefinition)?.ConvertToJObject().DeepClone();
                    var name = Utils.Utils.GetNotDuplicateName((string)config["Name"], aois, 0);
                    config["Name"] = name;
                    controller.AddAOIDefinition((JObject)config);
                    var aoi = ((AoiDefinitionCollection)controller.AOIDefinitionCollection).Find(name);
                    //aoi.ParserTags();
                    //aoi.PostInit((DataTypeCollection)controller.DataTypes);
                    aoi.datatype.PostInit((DataTypeCollection)controller.DataTypes);
                    foreach (var routine in aoi.Routines)
                    {
                        routine.IsError = true;
                        projectInfoService?.Verify(routine);
                    }
                }

                if (_pastedObject.GetType().IsGenericType)
                {
                    foreach (var p in (List<IBaseComponent>)_pastedObject)
                    {
                        var obj = p as AoiDefinition;
                        var config = obj?.ConvertToJObject().DeepClone();
                        var name = Utils.Utils.GetNotDuplicateName((string)config["Name"], aois, 0);
                        config["Name"] = name;
                        controller.AddAOIDefinition((JObject)config);
                        var aoi = ((AoiDefinitionCollection)controller.AOIDefinitionCollection).Find(name);
                        //aoi.PostInit((DataTypeCollection)controller.DataTypes);
                        aoi.datatype.PostInit((DataTypeCollection)controller.DataTypes);
                        foreach (var routine in aoi.Routines)
                        {
                            routine.IsError = true;
                            projectInfoService?.Verify(routine);
                        }
                    }
                }
            }

            if (selectedProjectItem.Kind == ProjectItemType.UserDefineds &&
                ((_pastedObject is UserDefinedDataType) || _pastedObject.GetType().IsGenericType) &&
                selectedProjectItem.AssociatedObject == null)
            {
                var controller = Controller.GetInstance();
                var types = controller.DataTypes.Where(d => d is UserDefinedDataType).Select(d => d.Name).ToList();
                if (_pastedObject is UserDefinedDataType)
                {
                    var config = (_pastedObject as UserDefinedDataType).ConvertToJObject().DeepClone();
                    var name = Utils.Utils.GetNotDuplicateName((string)config["Name"], types, 0);
                    config["Name"] = name;
                    controller.AddDataType(config);
                    var udt = controller.DataTypes[name];
                    (udt as UserDefinedDataType)?.PostInit((DataTypeCollection)controller.DataTypes);
                }

                if (_pastedObject.GetType().IsGenericType)
                {
                    foreach (var p in (List<IBaseComponent>)_pastedObject)
                    {
                        var obj = p as UserDefinedDataType;
                        var config = obj?.ConvertToJObject().DeepClone();
                        var name = Utils.Utils.GetNotDuplicateName((string)config["Name"], types, 0);
                        config["Name"] = name;
                        controller.AddDataType(config);
                        var udt = controller.DataTypes[name];
                        (udt as UserDefinedDataType)?.PostInit((DataTypeCollection)controller.DataTypes);
                    }
                }
            }

            if (selectedProjectItem.Kind == ProjectItemType.UserDefined && _pastedObject is UserDefinedDataType &&
                !((UserDefinedDataType)_pastedObject).IsStringType && selectedProjectItem.AssociatedObject == null)
            {
                var controller = Controller.GetInstance();
                var config = (_pastedObject as UserDefinedDataType).ConvertToJObject().DeepClone();
                var types = controller.DataTypes.Where(d => d is UserDefinedDataType).Select(d => d.Name).ToList();
                var name = Utils.Utils.GetNotDuplicateName((string)config["Name"], types, 0);
                config["Name"] = name;
                controller.AddDataType(config);
                var udt = controller.DataTypes[name];
                (udt as UserDefinedDataType)?.PostInit((DataTypeCollection)controller.DataTypes);
            }

            if (selectedProjectItem.Kind == ProjectItemType.Strings)
            {
                var controller = Controller.GetInstance();
                var types = controller.DataTypes.Where(d => d is UserDefinedDataType).Select(d => d.Name).ToList();
                if (_pastedObject is UserDefinedDataType && ((UserDefinedDataType)_pastedObject).IsStringType &&
                    selectedProjectItem.AssociatedObject == null)
                {
                    var config = (_pastedObject as UserDefinedDataType).ConvertToJObject().DeepClone();
                    var name = Utils.Utils.GetNotDuplicateName((string)config["Name"], types, 0);
                    config["Name"] = name;
                    controller.AddDataType(config);
                    var udt = controller.DataTypes[name];
                    (udt as UserDefinedDataType)?.PostInit((DataTypeCollection)controller.DataTypes);
                }

                if (_pastedObject.GetType().IsGenericType)
                {
                    var objs = (List<IBaseComponent>)_pastedObject;
                    if (objs != null && objs.Count > 1 && ((UserDefinedDataType)objs[0]).IsStringType &&
                        selectedProjectItem.AssociatedObject == null)
                    {
                        foreach (var p in objs)
                        {
                            var obj = p as UserDefinedDataType;
                            var config = obj?.ConvertToJObject().DeepClone();
                            var name = Utils.Utils.GetNotDuplicateName((string)config["Name"], types, 0);
                            config["Name"] = name;
                            controller.AddDataType(config);
                            var udt = controller.DataTypes[name];
                            (udt as UserDefinedDataType)?.PostInit((DataTypeCollection)controller.DataTypes);
                        }
                    }
                }
            }

            if (selectedProjectItem.Kind == ProjectItemType.String && _pastedObject is UserDefinedDataType &&
                ((UserDefinedDataType)_pastedObject).IsStringType && selectedProjectItem.AssociatedObject == null)
            {
                var controller = Controller.GetInstance();
                var config = (_pastedObject as UserDefinedDataType).ConvertToJObject().DeepClone();
                var types = controller.DataTypes.Where(d => d is UserDefinedDataType).Select(d => d.Name).ToList();
                var name = Utils.Utils.GetNotDuplicateName((string)config["Name"], types, 0);
                config["Name"] = name;
                controller.AddDataType(config);
                var udt = controller.DataTypes[name];
                (udt as UserDefinedDataType)?.PostInit((DataTypeCollection)controller.DataTypes);
            }

            if (selectedProjectItem.Kind == ProjectItemType.Ethernet && _pastedObject is IDeviceModule &&
                !(_pastedObject is DiscreteIO || _pastedObject is AnalogIO))
            {
                var controller = Controller.GetInstance();
                var config = (_pastedObject as DeviceModule)?.ConvertToJObject().DeepClone();

                config["ParentModule"] = "Local";

                var ports = config["Ports"] as JArray;
                if (ports != null)
                {
                    foreach (var port in ports)
                    {
                        if (string.Equals(port["Type"]?.ToString(), "Ethernet"))
                        {
                            if (port["Address"] != null)
                                port["Address"] = "";
                        }
                    }

                }

                var modules = controller.DeviceModules.Select(d => d.Name).ToList();
                var name = Utils.Utils.GetNotDuplicateName((string)config["Name"], modules);
                config["Name"] = name;

                controller.AddDeviceModule(config);

                //post 
                {
                    var deviceModule = controller.DeviceModules[name] as DeviceModule;
                    Debug.Assert(deviceModule != null);
                    deviceModule.ParentModule = controller.DeviceModules["Local"];
                    deviceModule?.PostLoadJson();
                }
            }

            if ((selectedProjectItem.Kind == ProjectItemType.Bus ||
                 selectedProjectItem.AssociatedObject is CommunicationsAdapter) &&
                (_pastedObject is DiscreteIO || _pastedObject is AnalogIO))
            {
                var parentModule = selectedProjectItem.AssociatedObject as CommunicationsAdapter;
                var controller = Controller.GetInstance();
                var config = (_pastedObject as DeviceModule)?.ConvertToJObject().DeepClone();
                var modules = controller.DeviceModules.Select(d => d.Name).ToList();
                var name = Utils.Utils.GetNotDuplicateName((string)config["Name"], modules);
                config["ParentModule"] = parentModule.Name;
                config["Name"] = name;
                var port = config["Ports"][0];
                var solt = (int)port["Address"];
                solt = GetNotDuplicateSlot(parentModule, solt);
                port["Address"] = solt;
                controller.AddDeviceModule(config);
                //post 
                {
                    var deviceModule = controller.DeviceModules[name] as DeviceModule;
                    Debug.Assert(deviceModule != null);
                    deviceModule.ParentModule = parentModule;
                    deviceModule?.PostLoadJson();
                }
            }

            if (selectedProjectItem.Kind == ProjectItemType.Trends)
            {
                var controller = Controller.GetInstance();
                var trends = controller.Trends.Select(t => t.Name).ToList();

                if (_pastedObject is ITrend)
                {
                    var config = (_pastedObject as TrendObject)?.ToJson();
                    var name = Utils.Utils.GetNotDuplicateName((string)config["Name"], trends);
                    config["Name"] = name;
                    controller.AddTrend(config);
                }

                if (_pastedObject.GetType().IsGenericType)
                {
                    foreach (var p in (List<IBaseComponent>)_pastedObject)
                    {
                        var to = p as TrendObject;
                        var config = to?.ToJson();
                        var name = Utils.Utils.GetNotDuplicateName((string)config["Name"], trends);
                        config["Name"] = name;
                        controller.AddTrend(config);
                    }
                }
            }

            //TODO(gjc): need remove reset
            service?.AttachController();
            service?.Reset();
        }

        private int GetNotDuplicateSlot(CommunicationsAdapter parent, int slot)
        {
            var controller = Controller.GetInstance();
            var slots = controller.DeviceModules.Where(m => ((DeviceModule)m).ParentModule == parent)
                .Select(m => (m as DiscreteIO)?.Slot ?? (m as AnalogIO)?.Slot).ToList();
            if (slots.Contains(slot))
            {
                slots = slots.OrderBy(a => a).ToList();
                for (int i = 0; i < parent.ChassisSize; i++)
                {
                    if (slot == i + 1) continue;

                    if (i < slots.Count)
                    {
                        if (i + 1 != slots[i])
                            return i + 1;
                        continue;
                    }

                    return i + 1;
                }
            }

            return slot;
        }

        private void DoDelete(object sender, EventArgs e)
        {
            Delete(true);
        }

        private bool Delete(bool isDelete)
        {
            var globalClipboard = CurrentObject.GetInstance().Current as IGlobalClipboard;
            if (globalClipboard != null)
            {
                return globalClipboard.CanPasted();
            }

            var selectedProjectItems = ServiceProvider.GetSelectedProjectItems();
            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();
            if (selectedProjectItems != null && selectedProjectItems.Count > 1)
                selectedProjectItem = selectedProjectItems[0];

            var service = Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;
            var serviceEditorPane = Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
            var studioUIService =
                Package.GetGlobalService(typeof(SStudioUIService)) as IStudioUIService;

            var result = false;
            ITag tag = selectedProjectItem.AssociatedObject as ITag;
            if (tag != null)
            {
                Controller control = tag.ParentController as Controller;
                string message = string.Empty;

                IDataType dataType = tag.DataTypeInfo.DataType;
                if (dataType.IsMotionGroupType)
                {
                    message = $"{(isDelete ? "Delete" : "Cut")} the motion group '{tag.Name}'?";
                }
                else if (dataType.IsAxisType)
                {
                    message = $"{(isDelete ? "Delete" : "Cut")} the axis '{tag.Name}'?";
                }

                if (MessageBox.Show(message, "ICS Studio", MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    studioUIService?.DeleteTag(tag);

                    result = true;
                }
            }
            else if (selectedProjectItem.AssociatedObject is IRoutine)
            {
                var routine = (IRoutine)selectedProjectItem.AssociatedObject;
                if (MessageBox.Show($"{(isDelete ? "Delete" : "Cut")} the routine '{routine.Name}'?", "ICS Studio",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    if (routine.ParentController.IsOnline)
                    {
                        if (!IsRoutineReferenced(routine, isDelete)) return false;
                    }

                    result = true;

                    (routine.ParentCollection as RoutineCollection)?.DeleteRoutine(routine);

                    service?.CloseDialog(GetWindowId(routine));
                    serviceEditorPane?.CloseWindow(routine.Uid);
                }
            }
            else if (selectedProjectItem.AssociatedObject is IProgram)
            {
                var program = (IProgram)selectedProjectItem.AssociatedObject;
                string msg = $"{(isDelete ? "Delete" : "Cut")} the program '{program.Name}'?";
                if (selectedProjectItems != null && selectedProjectItems.Count > 1)
                    msg =
                        $"{(isDelete ? "Delete" : "Cut")} selected programs will also delete their descendant programs within the logical model.\r\n\r\n{(isDelete ? "Delete" : "Cut")} selected programs and their descendants?";
                if (MessageBox.Show(msg, "ICS Studio",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    result = true;
                    if (selectedProjectItems != null && selectedProjectItems.Count > 0)
                    {
                        foreach (var item in selectedProjectItems)
                        {
                            var controller = Controller.GetInstance();
                            var prog = (IProgram)item.AssociatedObject;
                            (prog.ParentController).Programs.Remove(prog.Name);

                            foreach (var routine in prog.Routines)
                            {
                                service?.CloseDialog(GetWindowId(routine));
                                serviceEditorPane?.CloseWindow(routine.Uid);
                            }

                            service?.CloseDialog(GetWindowId(prog));
                            serviceEditorPane?.CloseMonitorEditTags(controller, program != null ? (ITagCollectionContainer)program : controller);
                        }
                    }
                }
            }
            else if (selectedProjectItem.AssociatedObject is ITask)
            {
                var task = (ITask)selectedProjectItem.AssociatedObject;
                if (MessageBox.Show($"{(isDelete ? "Delete" : "Cut")} the task '{task.Name}'?", "ICS Studio",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    result = true;
                    (task.ParentCollection as TaskCollection)?.DeleteTask(task);
                    service?.CloseDialog(GetWindowId(task));
                }
            }
            else if (selectedProjectItem.AssociatedObject is UserDefinedDataType)
            {
                var dataType = (IDataType)selectedProjectItem.AssociatedObject;
                string msg = $"{(isDelete ? "Delete" : "Cut")} the data type '{dataType.Name}'?";
                if (selectedProjectItems != null && selectedProjectItems.Count > 1)
                    msg = $"{(isDelete ? "Delete" : "Cut")} selected data types?";
                if (MessageBox.Show(msg, "ICS Studio",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    result = true;
                    if (selectedProjectItems != null && selectedProjectItems.Count > 0)
                    {
                        IProjectInfoService projectInfoService =
                            Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
                        var controller = projectInfoService?.Controller;
                        foreach (var item in selectedProjectItems)
                        {
                            var dt = (IDataType)item.AssociatedObject;
                            (controller?.DataTypes as DataTypeCollection)?.DeleteDataType(dt);
                            serviceEditorPane?.CloseWindow(dt.Uid);
                        }
                    }
                }
            }
            else if (selectedProjectItem.AssociatedObject is AoiDefinition)
            {
                var aoi = (AoiDefinition)selectedProjectItem.AssociatedObject;
                string msg = $"{(isDelete ? "Delete" : "Cut")} the instruction '{aoi.Name}'?";
                if (selectedProjectItems != null && selectedProjectItems.Count > 1)
                    msg = $"{(isDelete ? "Delete" : "Cut")} selected instructions?";
                if (MessageBox.Show(msg, "ICS Studio",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    result = true;
                    if (selectedProjectItems != null && selectedProjectItems.Count > 0)
                    {
                        foreach (var item in selectedProjectItems)
                        {
                            var a = item.AssociatedObject as AoiDefinition;
                            if (a != null)
                            {
                                var controller = Controller.GetInstance();
                                controller.AOIDefinitionCollection.Remove(a);
                                a.Dispose();
                                service?.CloseDialog(GetWindowId(a));
                                foreach (var r in a.Routines)
                                {
                                    service?.CloseDialog(GetWindowId(r));
                                    serviceEditorPane?.CloseWindow(r.Uid);
                                }
                                serviceEditorPane?.CloseMonitorEditTags(controller, a, a.Name);
                            }
                        }
                    }
                }
            }
            else if (selectedProjectItem.AssociatedObject is IDeviceModule)
            {
                var module = (DeviceModule)selectedProjectItem.AssociatedObject;
                if (MessageBox.Show($"{(isDelete ? "Delete" : "Cut")} the module '{module.DisplayText}'?", "ICS Studio",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    IController controller = module.ParentController;
                    (controller.DeviceModules as DeviceModuleCollection)?.RemoveDeviceModule(module);

                    var deviceModulesInOpen = serviceEditorPane?.GetDeviceModulesInOpen();
                    if (deviceModulesInOpen != null)
                    {
                        foreach (var deviceModule in deviceModulesInOpen)
                        {
                            if (controller.DeviceModules[deviceModule.Uid] == null)
                                serviceEditorPane.CloseWindow(deviceModule.Uid);
                        }
                    }

                    result = true;
                }
            }
            else if (selectedProjectItem.AssociatedObject is ITrend)
            {
                var trend = (ITrend)selectedProjectItem.AssociatedObject;
                string msg = $"{(isDelete ? "Delete" : "Cut")} the trend '{trend.Name}'?";
                if (selectedProjectItems != null && selectedProjectItems.Count > 1)
                    msg = $"{(isDelete ? "Delete" : "Cut")} selected trends?";
                if (MessageBox.Show(msg, "ICS Studio",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    result = true;
                    if (selectedProjectItems != null && selectedProjectItems.Count > 0)
                    {
                        foreach (var item in selectedProjectItems)
                        {
                            var tr = item.AssociatedObject as ITrend;
                            if (tr != null)
                            {
                                serviceEditorPane?.CloseWindow(tr.Uid);
                                trend.ParentController.Trends.Remove(tr);
                            }
                        }
                    }
                }
            }

            return result;
        }

        private bool IsRoutineReferenced(IRoutine routine, bool isDelete)
        {
            bool isReference = true;
            foreach (var r in routine.ParentCollection)
            {
                var st = r as STRoutine;
                if (st != null)
                {
                    foreach (var variable in st.GetCurrentVariableInfos(OnlineEditType.Original))
                    {
                        if (variable.IsRoutine &&!variable.IsUnknown&&
                            variable.Name.Equals(routine.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            isReference = false;
                            break;
                        }
                    }

                    if (st.PendingCodeText != null)
                    {
                        foreach (var variable in st.GetCurrentVariableInfos(OnlineEditType.Pending))
                        {
                            if (variable.IsRoutine &&
                                variable.Name.Equals(routine.Name, StringComparison.OrdinalIgnoreCase))
                            {
                                isReference = false;
                                break;
                            }
                        }
                    }

                    if (st.TestCodeText != null)
                    {
                        foreach (var variable in st.GetCurrentVariableInfos(OnlineEditType.Test))
                        {
                            if (variable.IsRoutine &&
                                variable.Name.Equals(routine.Name, StringComparison.OrdinalIgnoreCase))
                            {
                                isReference = false;
                                break;
                            }
                        }
                    }

                    if (!isReference) break;
                }

                //TODO(zyl):check other routine
            }

            if (!isReference)
            {
                MessageBox.Show(
                    $"Failed to {(isDelete ? "Delete" : "Cut")} routine '{routine.Name}'.\nOperation not allowed on objects referenced by other objects.",
                    "ICS Studio", MessageBoxButton.OK);
                return false;
            }

            return true;
        }

        private List<string> GetWindowId(object baseObject)
        {
            List<string> ids = new List<string>();
            if (baseObject is ITag)
            {
                using (var tag = (ITag)baseObject)
                {
                    if (tag.DataTypeInfo.DataType.IsMotionGroupType)
                    {
                        ids.Add($"MotionGroupProperties{tag.Uid}");
                        ids.Add($"AxisSchedule{tag.Uid}");
                    }
                    else if (tag.DataTypeInfo.DataType is AXIS_CIP_DRIVE)
                    {
                        ids.Add($"AxisCIPDriveProperties{tag.Uid}");
                    }
                    else if (tag.DataTypeInfo.DataType is AXIS_VIRTUAL)
                    {
                        ids.Add($"AxisVirtualProperties{tag.Uid}");
                    }

                    ids.Add($"TagProperties{tag.Uid}");
                }
            }
            else if (baseObject is IRoutine)
            {
                var routine = (IRoutine)baseObject;
                ids.Add($"RoutineProperties{routine.Uid}");
            }
            else if (baseObject is IProgram)
            {
                var program = (IProgram)baseObject;
                ids.Add($"ProgramProperties{program.Uid}");
            }
            else if (baseObject is ITask)
            {
                var task = (ITask)baseObject;
                ids.Add($"TaskProperties{task.Uid}");
            }
            else if (baseObject is AoiDefinition)
            {
                var aoi = (AoiDefinition)baseObject;
                ids.Add($"AddOnInstruction{aoi.Uid}");
            }

            return ids;
        }

        public bool CanPaste(IProjectItem projectItem)
        {
            var globalClipboard = CurrentObject.GetInstance().Current as IGlobalClipboard;
            if (globalClipboard != null)
            {
                return globalClipboard.CanPasted();
            }

            if (projectItem != null && _pastedObject != null)
            {
                if (projectItem.Kind == ProjectItemType.Bus)
                {
                    var adapter = projectItem.AssociatedObject as CommunicationsAdapter;
                    if (adapter != null)
                    {
                        var children = Controller.GetInstance().DeviceModules
                            .Where(d => (d as DeviceModule)?.ParentModule == adapter);
                        if (children.Count() < adapter.ChassisSize - 1)
                            return true;
                        return false;
                    }
                }

                if (projectItem.Kind == ProjectItemType.Tasks && _pastedObject is Task && !Controller.GetInstance().IsOnline)
                {
                    return true;
                }

                var task = projectItem.AssociatedObject as Task;
                if (task != null && ((_pastedObject is IProgram) || _pastedObject.GetType().IsGenericType))
                {
                    if (Controller.GetInstance().IsOnline)
                    {
                        return false;
                    }

                    return true;
                }

                var program = projectItem.AssociatedObject as IProgram;
                if (program != null && _pastedObject is IRoutine)
                {
                    if (Controller.GetInstance().IsOnline)
                    {
                        return false;
                    }

                    return true;
                }

                if ((projectItem.Kind == ProjectItemType.MotionGroup ||
                     projectItem.Kind == ProjectItemType.UngroupedAxes) &&
                    (_pastedObject as Tag)?.DataTypeInfo.DataType is AXIS_COMMON)
                {
                    return true;
                }

                if (projectItem.Kind == ProjectItemType.AddOnInstructions &&
                    ((_pastedObject is AoiDefinition) || _pastedObject.GetType().IsGenericType) &&
                    projectItem.AssociatedObject == null)
                {
                    if (Controller.GetInstance().IsOnline)
                    {
                        return false;
                    }

                    return true;
                }

                if (projectItem.Kind == ProjectItemType.UserDefined && _pastedObject is UserDefinedDataType &&
                    !((UserDefinedDataType)_pastedObject).IsStringType && projectItem.AssociatedObject == null)
                {
                    return true;
                }

                //右键点击UserDefineds目录
                if (projectItem.Kind == ProjectItemType.UserDefineds && projectItem.AssociatedObject == null)
                {
                    //复制的是单个UDT
                    if (_pastedObject is UserDefinedDataType && !((UserDefinedDataType)_pastedObject).IsStringType)
                        return true;
                    //复制的是多个UDT
                    if (_pastedObject.GetType()
                        .IsGenericType) // && !((List<UserDefinedDataType>)_pastedObject)[0].IsStringType
                    {
                        var objs = _pastedObject as List<IBaseComponent>;
                        if (objs != null && objs.Count > 1 && !((UserDefinedDataType)objs[0]).IsStringType)
                            return true;
                    }
                }

                if (projectItem.Kind == ProjectItemType.Strings)
                {
                    if (_pastedObject is UserDefinedDataType &&
                        ((UserDefinedDataType)_pastedObject).IsStringType)
                        return true;
                    if (_pastedObject.GetType().IsGenericType)
                    {
                        var objs = _pastedObject as List<IBaseComponent>;
                        if (objs != null && objs.Count > 1 && ((UserDefinedDataType)objs[0]).IsStringType)
                            return true;
                    }
                }
                //if (projectItem.Kind == ProjectItemType.String && _pastedObject is UserDefinedDataType &&
                //    ((UserDefinedDataType)_pastedObject).IsStringType)
                //{
                //    return true;
                //}

                if (projectItem.Kind == ProjectItemType.Ethernet && _pastedObject is IDeviceModule &&
                    !(_pastedObject is DiscreteIO || _pastedObject is AnalogIO))
                {
                    if (Controller.GetInstance().IsOnline)
                    {
                        return false;
                    }

                    return true;
                }

                if (projectItem.Kind == ProjectItemType.Trends && _pastedObject != null &&
                    ((_pastedObject is ITrend) || _pastedObject.GetType().IsGenericType))
                {
                    return true;
                }

                if (projectItem.AssociatedObject is CommunicationsAdapter &&
                    (_pastedObject is DiscreteIO || _pastedObject is AnalogIO))
                {
                    if (Controller.GetInstance().IsOnline)
                    {
                        return false;
                    }

                    var size = ((CommunicationsAdapter)projectItem.AssociatedObject).ChassisSize;
                    var child = Controller.GetInstance().DeviceModules.Where(d =>
                        ((DeviceModule)d).ParentModule == projectItem.AssociatedObject);
                    if (child.Count() >= size - 1) return false;
                    return true;
                }
            }

            return false;
        }

        private object _pastedObject;

        public bool CanCut(IProjectItem selectedProjectItem)
        {
            var globalClipboard = CurrentObject.GetInstance().Current as IGlobalClipboard;
            if (globalClipboard != null)
            {
                return globalClipboard.CanCut();
            }

            if (selectedProjectItem == null) return false;
            if (selectedProjectItem.Kind == ProjectItemType.Bus ||
                selectedProjectItem.Kind == ProjectItemType.ProgramTags ||
                selectedProjectItem.Kind == ProjectItemType.FaultHandler ||
                selectedProjectItem.Kind == ProjectItemType.PowerHandler ||
                selectedProjectItem.Kind == ProjectItemType.ControllerTags ||
                (selectedProjectItem.DisplayName?.StartsWith("[0]") ?? false))
                return false;
            //else
            //    return CanExecuteDeleteCommand(selectedProjectItem.AssociatedObject);
            return CanExecuteDeleteCommand(ServiceProvider.GetSelectedProjectItems()
                .Select(p => p.AssociatedObject as IBaseComponent).ToList());
        }

        public bool CanExecuteDeleteCommand(List<IBaseComponent> objs)
        {
            if (objs == null || objs.Count < 1)
                return false;

            bool canDelete = true;
            foreach (object baseObject in objs)
            {
                if (!CanExecuteDeleteCommand(baseObject))
                    canDelete = false;
                break;
            }

            return canDelete;
        }

        public bool CanExecuteDeleteCommand(object baseObject)
        {
            if (baseObject != null)
            {
                if (baseObject is ITag)
                {
                    Tag tag = baseObject as Tag;
                    if (tag != null)
                    {
                        if (tag.ParentController.IsOnline)
                        {
                            var dataType = tag.DataTypeInfo.DataType;

                            if (dataType.IsMotionGroupType)
                                return false;

                            if (dataType.IsAxisType)
                            {
                                var axisCIPDrive = tag.DataWrapper as AxisCIPDrive;
                                if (axisCIPDrive != null && axisCIPDrive.AssignedGroup != null)
                                    return false;

                                var axisVirtual = tag.DataWrapper as AxisVirtual;
                                if (axisVirtual != null && axisVirtual.AssignedGroup != null)
                                    return false;
                            }
                        }
                    }

                    return true;
                }

                else if (baseObject is IRoutine)
                {
                    if (((IRoutine)baseObject).ParentCollection.ParentProgram is AoiDefinition) return false;
                    if (((IRoutine)baseObject).ParentController.IsOnline)
                    {
                        var program = (Program)((IRoutine)baseObject).ParentCollection.ParentProgram;
                        if (program.MainRoutineName == ((IRoutine)(baseObject)).Name ||
                            program.FaultRoutineName == ((IRoutine)(baseObject)).Name) return false;
                    }

                    return true;
                }
                else if (baseObject is IProgram)
                {
                    if (Controller.GetInstance().IsOnline)
                    {
                        return false;
                    }

                    return true;
                }
                else if (baseObject is ITask)
                {
                    if (Controller.GetInstance().IsOnline &&
                        Controller.GetInstance().KeySwitchPosition == ControllerKeySwitch.RunKeySwitch)
                        return false;

                    bool haveChild = false;
                    foreach (var p in ((ITask)baseObject).ParentController.Programs)
                    {
                        if (haveChild) continue;
                        if (p.ParentTask == baseObject) haveChild = true;
                    }

                    if (haveChild) return false;
                    else return true;
                }
                else if (baseObject is UserDefinedDataType)
                {
                    return !DataTypeExtend.CheckDataTypeIsUsed((IDataType)baseObject);
                }
                else if (baseObject is AoiDefinition)
                {
                    var aoi = (AoiDefinition)baseObject;
                    IDataType aoiDataType = aoi.ParentController.DataTypes[aoi.Name];
                    return !DataTypeExtend.CheckDataTypeIsUsed(aoiDataType);
                }
                else if (baseObject is IDeviceModule)
                {
                    if (Controller.GetInstance().IsOnline)
                    {
                        return false;
                    }

                    if (((IDeviceModule)baseObject).Name != "Local" &&
                        !((IDeviceModule)baseObject).IsEmbeddedIO())
                    {
                        foreach (var deviceModule in Controller.GetInstance().DeviceModules)
                        {
                            if (baseObject == deviceModule) continue;
                            if (((DeviceModule)deviceModule).ParentModule == baseObject) return false;
                        }

                        return true;
                    }
                }
                else if (baseObject is ITrend) return true;
            }

            return false;
        }
    }
}
