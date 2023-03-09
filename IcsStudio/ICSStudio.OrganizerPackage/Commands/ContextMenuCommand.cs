using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Windows;
using ICSStudio.Gui.Utils;
using ICSStudio.OrganizerPackage.Utilities;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIInterfaces.Project;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.Interfaces.Tags;
using ICSStudio.OrganizerPackage.ViewModel;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.UIInterfaces.Editor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Application = System.Windows.Application;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Error;
using ICSStudio.Utils;
using MessageBox = System.Windows.MessageBox;
using Type = ICSStudio.UIInterfaces.Editor.Type;

namespace ICSStudio.OrganizerPackage.Commands
{
    internal sealed class ContextMenuCommand
    {
        private readonly Package _package;

        private ContextMenuCommand(Package package)
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
                AddGroup0Menus(commandService);
                AddGroup4Menus(commandService);
                AddGroup5Menus(commandService);
                AddGroup8Menus(commandService);
                AddTaskAddMenus(commandService);
                AddProgramAddMenus(commandService);
                AddPrintContextMenu(commandService);
            }

        }

        public static ContextMenuCommand Instance { get; private set; }
        private IServiceProvider ServiceProvider => _package;

        public static void Initialize(Package package)
        {
            Instance = new ContextMenuCommand(package);
        }

        #region Group0

        private void AddGroup0Menus(OleMenuCommandService commandService)
        {
            var menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.verifyControllerCommand);
            var menuItem = new OleMenuCommand(VerifyController, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.newTagCommand);
            menuItem = new OleMenuCommand(NewTag, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.newProgramCommand);
            menuItem = new OleMenuCommand(NewProgram, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.newTaskCommand);
            menuItem = new OleMenuCommand(NewTask, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.newLocalTagCommand);
            menuItem = new OleMenuCommand(NewLocalTag, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.newParameterCommand);
            menuItem = new OleMenuCommand(NewParameter, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.newMotionGroupCommand);
            menuItem = new OleMenuCommand(NewMotionGroup, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.newAxisMenu);
            menuItem = new OleMenuCommand(null, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.AXIS_CIP_DRIVECommand);
            menuItem = new OleMenuCommand(NewAxisCIPDrive, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.AXIS_VIRTUALCommand);
            menuItem = new OleMenuCommand(NewAxisVirtual, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.newCoordinateSystemCommand);
            menuItem = new OleMenuCommand(NewCoordinateSystem, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.newAddOnInstructionCommand);
            menuItem = new OleMenuCommand(NewAddOnInstruction, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.newDataTypeCommand);
            menuItem = new OleMenuCommand(NewDataType, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.newStringTypeCommand);
            menuItem = new OleMenuCommand(NewStringType, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.newTrendCommand);
            menuItem = new OleMenuCommand(NewTrend, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.newModuleCommand);
            menuItem = new OleMenuCommand(NewModule, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.importProgramCommand);
            menuItem = new OleMenuCommand(ImportProgram, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet,
                PackageIds.importAddOnInstructionCommand);
            menuItem = new OleMenuCommand(ImportAoi, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.importDataTypeCommand);
            menuItem = new OleMenuCommand(ImportDataType, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.importStringTypeCommand);
            menuItem = new OleMenuCommand(ImportString, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);


            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.openCommand);
            menuItem = new OleMenuCommand(Open, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.openTrendLogCommand);
            menuItem = new OleMenuCommand(OpenTrendLog, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.importModuleCommand);
            menuItem = new OleMenuCommand(ImportModule, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.importTrendCommand);
            menuItem = new OleMenuCommand(ImportTrend, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.gotoModuleCommand);
            menuItem = new OleMenuCommand(GotoModule, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.openDefinitionCommand);
            menuItem = new OleMenuCommand(OpenDefinition, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);


        }

        private void OpenDefinition(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();
            var aoi = selectedProjectItem.AssociatedObject as AoiDefinition;
            if (aoi != null)
            {
                ICreateDialogService createDialogService =
                    ServiceProvider.GetService(typeof(SCreateDialogService)) as ICreateDialogService;
                
                var uiShell = (IVsUIShell) ServiceProvider.GetService(typeof(SVsUIShell));
                var window =
                    createDialogService?.AddOnInstructionProperties(
                        selectedProjectItem.AssociatedObject as IAoiDefinition);
                window?.Show(uiShell);
            }
        }

        private void GotoModule(object sender, EventArgs e)
        {
            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();
            var tag = (ITag) selectedProjectItem.AssociatedObject;
            foreach (var module in tag.ParentController.DeviceModules)
            {
                var cipMotion = module as CIPMotionDrive;
                if (cipMotion != null)
                {
                    var result = cipMotion.ContainAxis(tag);
                    if (result)
                    {
                        var createEditorService =
                            Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;

                        createEditorService?.CreateModuleProperties(cipMotion);
                    }
                }
            }
        }

        private void OpenTrendLog(object sender, EventArgs e)
        {
            var openDlg = new OpenFileDialog
            {
                Title = @"Import file",
                Filter = @"csv文件(*.csv)|*.csv"
            };
            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                ParseCSV(openDlg.FileName);
            }
        }

        private void ParseCSV(string file)
        {
            try
            {
                JObject config;
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var sr = new StreamReader(fs, Encoding.Default))
                    {
                        config = ConvertCSVToJObject(sr);
                    }
                }

                Debug.Assert(config != null);
                var trendLog = new TrendLog(config, file);

                IServiceProvider serviceProvider = _package;
                var createEditorService =
                    serviceProvider?.GetService(typeof(SCreateEditorService)) as ICreateEditorService;

                createEditorService?.CreateTrend(trendLog);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                System.Windows.MessageBox.Show("Please use the correct template.", "ICS Studio", MessageBoxButton.OK);
            }
        }
        private JObject ConvertCSVToJObject(StreamReader sr)
        {
            int index = 0;
            try
            {
                var config = new JObject();
                var data = new JArray();
                var pens = new JArray();
                while (!sr.EndOfStream)
                {
                    //Console.WriteLine(sr.ReadLine());
                    var line = sr.ReadLine()?.Replace("，", ",").Split(',');
                    if (index == 0)
                    {
                        config["ControllerName"] = line?[1].Replace("\"", "");
                    }

                    if (index == 1)
                    {
                        config["TrendName"] = line?[1].Replace("\"", "");
                    }

                    if (index == 2)
                    {
                        config["TrendTags"] = line?[1].Replace("\"", "");
                    }

                    if (index == 3)
                    {
                        config["SamplePeriod"] = line?[1].Replace("\"", "").Replace("ms", "").Replace(" ", "");
                    }

                    if (index == 4)
                    {
                        config["Description"] = line.Length > 1 ? line[1].Replace("\"", "") : "";
                    }

                    if (index == 9)
                    {
                        config["StartTime"] = line?[1].Replace("\"", "") + "," + line?[2].Replace("\"", "");
                    }

                    if (index == 10)
                    {
                        config["StopTime"] = line?[1].Replace("\"", "") + "," + line?[2].Replace("\"", "");
                    }

                    if (index == 13)
                    {
                        for (int i = 3; i < line.Length; i++)
                        {
                            pens.Add(new JValue(line[i].Replace("\"", "").Replace("Program:", "")));
                        }
                    }

                    if (index >= 14)
                    {
                        if (string.IsNullOrEmpty(line?[0])) continue;
                        var info = new JArray();
                        //if ("Data".Equals(line[0].Replace("\"", ""))|| "数据".Equals(line[0].Replace("\"", "")))
                        //{
                        //    info.Add($"{line[1]},{line[2]}");
                        //    for (int i = 3; i < line.Length; i++)
                        //    {
                        //        var value = line[i].Replace("\"", "");
                        //        info.Add(new JValue(value));
                        //    }

                        //    data.Add(info);
                        //}
                        info.Add($"{line[1]},{line[2]}");
                        for (int i = 3; i < line.Length; i++)
                        {
                            var value = line[i].Replace("\"", "");
                            info.Add(new JValue(value));
                        }

                        data.Add(info);
                    }

                    index++;
                }

                config["Pens"] = pens;
                config["Data"] = data;

                return config;
            }
            catch (Exception e)
            {
                throw new IOException(e.Message);
            }
        }

        private void VerifyController(object sender, EventArgs e)
        {
            IProjectInfoService projectInfoService =
                ServiceProvider.GetService(typeof(SProjectInfoService)) as IProjectInfoService;
            projectInfoService?.VerifyInDialog();
        }

        private void NewTag(object sender, EventArgs e)
        {
            ICreateDialogService createDialogService =
                ServiceProvider.GetService(typeof(SCreateDialogService)) as ICreateDialogService;
            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = (IVsUIShell) ServiceProvider.GetService(typeof(SVsUIShell));

            var controller = ServiceProvider.GetController();

            if (createDialogService != null)
            {
                var dialog = createDialogService.CreateNewTagDialog(
                    DINT.Inst,
                    controller.Tags);
                dialog.ShowDialog(uiShell);
            }
        }

        private void NewProgram(object sender, EventArgs e)
        {
            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();
            ICreateDialogService createDialogService =
                ServiceProvider.GetService(typeof(SCreateDialogService)) as ICreateDialogService;

            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = (IVsUIShell) ServiceProvider.GetService(typeof(SVsUIShell));

            if (createDialogService != null)
            {
                // Task
                ITask task = selectedProjectItem.AssociatedObject as ITask;
                if (task != null || ((OrganizerItem) selectedProjectItem).DisplayName == "Unscheduled")
                {
                    var dialog = createDialogService.CreateProgramDialog(ProgramType.Normal,
                        (ITask) selectedProjectItem.AssociatedObject);
                    dialog.ShowDialog(uiShell);
                }

                // Controller
                IController controller = selectedProjectItem.AssociatedObject as IController;
                if (controller != null)
                {
                    ITask taskC = null;
                    if (selectedProjectItem.Kind == ProjectItemType.PowerHandler)
                    {
                        taskC = new CTask(controller) {Name = "Power-Up Handler"};

                    }
                    else if (selectedProjectItem.Kind == ProjectItemType.FaultHandler)
                    {
                        taskC = new CTask(controller) {Name = "Controller Fault Handler"};
                    }

                    var dialog = createDialogService.CreateProgramDialog(ProgramType.Normal, taskC);
                    dialog.ShowDialog(uiShell);
                }

            }
        }

        private void NewLocalTag(object sender, EventArgs e)
        {
            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();

            ICreateDialogService createDialogService =
                ServiceProvider.GetService(typeof(SCreateDialogService)) as ICreateDialogService;

            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = (IVsUIShell) ServiceProvider.GetService(typeof(SVsUIShell));

            var program = selectedProjectItem.AssociatedObject as IProgram;

            if (createDialogService != null && program != null)
            {
                var dialog = createDialogService.CreateNewTagDialog(
                    DINT.Inst,
                    program.Tags, Usage.Local);
                dialog.Owner = Application.Current.MainWindow;
                dialog.ShowDialog(uiShell);
            }

            var aoiDefinition = selectedProjectItem.AssociatedObject as IAoiDefinition;
            if (createDialogService != null && aoiDefinition != null)
            {
                //sure tree
                var dialog = createDialogService.CreateNewAoiTagDialog(
                    DINT.Inst,
                    aoiDefinition, Usage.Local,true);
                dialog.Owner = Application.Current.MainWindow;
                dialog.ShowDialog(uiShell);
            }
        }

        private void NewParameter(object sender, EventArgs e)
        {
            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();

            ICreateDialogService createDialogService =
                ServiceProvider.GetService(typeof(SCreateDialogService)) as ICreateDialogService;

            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = (IVsUIShell) ServiceProvider.GetService(typeof(SVsUIShell));

            var program = selectedProjectItem.AssociatedObject as IProgram;

            if (createDialogService != null && program != null)
            {
                var dialog = createDialogService.CreateNewTagDialog(
                    DINT.Inst,
                    program.Tags, Usage.Input);
                dialog.ShowDialog(uiShell);
            }

            var aoiDefinition = selectedProjectItem.AssociatedObject as IAoiDefinition;
            if (createDialogService != null && aoiDefinition != null)
            {
                var dialog = createDialogService.CreateNewAoiTagDialog(
                    DINT.Inst,
                    aoiDefinition, Usage.Input,true);
                dialog.Owner = Application.Current.MainWindow;
                dialog.ShowDialog(uiShell);
            }
        }

        private void NewTask(object sender, EventArgs e)
        {
            ICreateDialogService createDialogService =
                ServiceProvider.GetService(typeof(SCreateDialogService)) as ICreateDialogService;

            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = (IVsUIShell) ServiceProvider.GetService(typeof(SVsUIShell));

            if (createDialogService != null)
            {
                var dialog = createDialogService.CreateNewTaskDialog();
                dialog.ShowDialog(uiShell);
            }

        }

        private void NewMotionGroup(object sender, EventArgs e)
        {
            ICreateDialogService createDialogService =
                ServiceProvider.GetService(typeof(SCreateDialogService)) as ICreateDialogService;

            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = (IVsUIShell) ServiceProvider.GetService(typeof(SVsUIShell));

            var controller = ServiceProvider.GetController();

            if (createDialogService != null)
            {
                var dialog = createDialogService.CreateNewTagDialog(MOTION_GROUP.Inst, controller.Tags);
                dialog.ShowDialog(uiShell);
            }
        }

        private void NewAxisCIPDrive(object sender, EventArgs e)
        {
            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();

            ICreateDialogService createDialogService =
                ServiceProvider.GetService(typeof(SCreateDialogService)) as ICreateDialogService;

            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = (IVsUIShell) ServiceProvider.GetService(typeof(SVsUIShell));

            var controller = ServiceProvider.GetController();

            if (createDialogService != null)
            {
                var dialog = createDialogService.CreateNewTagDialog(
                    AXIS_CIP_DRIVE.Inst,
                    controller.Tags,
                    Usage.NullParameterType,
                    (ITag) selectedProjectItem.AssociatedObject);
                dialog.ShowDialog(uiShell);
            }
        }

        private void NewAxisVirtual(object sender, EventArgs e)
        {
            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();

            ICreateDialogService createDialogService =
                ServiceProvider.GetService(typeof(SCreateDialogService)) as ICreateDialogService;

            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = (IVsUIShell) ServiceProvider.GetService(typeof(SVsUIShell));

            var controller = ServiceProvider.GetController();

            if (createDialogService != null)
            {
                var dialog = createDialogService.CreateNewTagDialog(
                    AXIS_VIRTUAL.Inst,
                    controller.Tags,
                    Usage.NullParameterType,
                    (ITag) selectedProjectItem.AssociatedObject);
                dialog.ShowDialog(uiShell);
            }
        }

        private void NewCoordinateSystem(object sender, EventArgs e)
        {
            //TODO(gjc): add code here
        }

        private void NewAddOnInstruction(object sender, EventArgs e)
        {
            //TODO(gjc): add code here
            ICreateDialogService createDialogService =
                ServiceProvider.GetService(typeof(SCreateDialogService)) as ICreateDialogService;

            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = (IVsUIShell) ServiceProvider.GetService(typeof(SVsUIShell));

            if (createDialogService != null)
            {
                var dialog = createDialogService.CreateAddOnInstruction();
                dialog.ShowDialog(uiShell);
            }
        }

        private void NewDataType(object sender, EventArgs e)
        {
            IServiceProvider serviceProvider = _package;
            var createEditorService =
                serviceProvider?.GetService(typeof(SCreateEditorService)) as ICreateEditorService;
            JObject jObject = new JObject();
            jObject["Family"] = 0;
            jObject["Name"] = "";
            jObject["Description"] = "";
            createEditorService?.CreateNewDataType(
                new UserDefinedDataType((DataTypeCollection) Controller.GetInstance().DataTypes, jObject)
                    {IsUDIDefinedType = true});
        }

        private void NewStringType(object sender, EventArgs e)
        {
            //TODO(gjc): add code here
            IServiceProvider serviceProvider = _package;
            var createEditorService =
                serviceProvider?.GetService(typeof(SCreateEditorService)) as ICreateEditorService;
            JObject jObject = new JObject();
            jObject["Family"] = 1;
            jObject["Name"] = "";
            jObject["Description"] = "";
            createEditorService?.CreateNewDataType(
                new UserDefinedDataType((DataTypeCollection) Controller.GetInstance().DataTypes, jObject)
                    {IsUDIDefinedType = true});
        }

        private void NewTrend(object sender, EventArgs e)
        {
            ICreateDialogService createDialogService =
                ServiceProvider.GetService(typeof(SCreateDialogService)) as ICreateDialogService;

            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = (IVsUIShell) ServiceProvider.GetService(typeof(SVsUIShell));

            if (createDialogService != null)
            {
                var dialog = createDialogService.CreateNewTrendDialog(ServiceProvider.GetController());
                dialog.ShowDialog(uiShell);
            }
        }

        private void NewModule(object sender, EventArgs e)
        {
            //TODO(gjc): need edit
            PortType portType = PortType.Ethernet;

            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();

            if (selectedProjectItem.Kind == ProjectItemType.Ethernet)
                portType = PortType.Ethernet;
            else if (selectedProjectItem.Kind == ProjectItemType.Bus ||
                     selectedProjectItem.Kind == ProjectItemType.DeviceModule ||
                     selectedProjectItem.Kind == ProjectItemType.ExpansionIO)
            {
                DeviceModule deviceModule = selectedProjectItem.AssociatedObject as DeviceModule;
                Contract.Assert(deviceModule != null);

                var pointIOPort = deviceModule.GetFirstPort(PortType.PointIO);
                if (pointIOPort != null && !pointIOPort.Upstream)
                    portType = PortType.PointIO;

                var compactPort = deviceModule.GetFirstPort(PortType.Compact);
                if (compactPort != null && !compactPort.Upstream)
                    portType = PortType.Compact;

            }


            ICreateDialogService createDialogService =
                ServiceProvider.GetService(typeof(SCreateDialogService)) as ICreateDialogService;

            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = (IVsUIShell) ServiceProvider.GetService(typeof(SVsUIShell));

            var controller = ServiceProvider.GetController();

            //TODO(gjc): need check here
            IDeviceModule parentModule = selectedProjectItem.AssociatedObject as IDeviceModule ??
                                         controller.DeviceModules["Local"];


            if (createDialogService != null)
            {
                var dialog =
                    createDialogService.CreateSelectModuleTypeDialog(controller, parentModule, portType);

                dialog.ShowDialog(uiShell);
            }
        }

        private void ImportProgram(object sender, EventArgs e)
        {
            IProjectInfoService projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            var selectedProject = _package.GetSelectedProjectItem();
            projectInfoService?.ImportData(selectedProject.Kind, selectedProject.AssociatedObject);
        }

        private void ImportDataType(object sender, EventArgs e)
        {
            IProjectInfoService projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            var selectedProject = _package.GetSelectedProjectItem();
            projectInfoService?.ImportData(selectedProject.Kind, selectedProject.AssociatedObject);
        }

        private void ImportAoi(object sender, EventArgs e)
        {
            IProjectInfoService projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            var selectedProject = _package.GetSelectedProjectItem();
            projectInfoService?.ImportData(selectedProject.Kind, selectedProject.AssociatedObject);
        }

        private void ImportString(object sender, EventArgs e)
        {
            IProjectInfoService projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            var selectedProject = _package.GetSelectedProjectItem();
            projectInfoService?.ImportData(selectedProject.Kind, selectedProject.AssociatedObject);
        }

        private void ImportModule(object sender, EventArgs e)
        {
            IProjectInfoService projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            var selectedProject = _package.GetSelectedProjectItem();
            projectInfoService?.ImportData(selectedProject.Kind, selectedProject.AssociatedObject);
        }

        private void ImportTrend(object sender, EventArgs e)
        {
            IProjectInfoService projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            var selectedProject = _package.GetSelectedProjectItem();
            projectInfoService?.ImportData(selectedProject.Kind, selectedProject.AssociatedObject);
        }

        public void Open(object sender, EventArgs e)
        {
            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();

            IServiceProvider serviceProvider = _package;
            var createEditorService =
                serviceProvider?.GetService(typeof(SCreateEditorService)) as ICreateEditorService;

            IRoutine routine = selectedProjectItem.AssociatedObject as IRoutine;
            if (routine != null)
            {
                switch (routine.Type)
                {
                    case RoutineType.RLL:
                        createEditorService?.CreateRLLEditor((IRoutine) selectedProjectItem.AssociatedObject);
                        break;
                    case RoutineType.ST:
                        var st = selectedProjectItem.AssociatedObject as STRoutine;
                        createEditorService?.CreateSTEditor(st,
                            st.PendingCodeText != null ? OnlineEditType.Pending : OnlineEditType.Original);
                        break;
                    case RoutineType.SFC:
                        createEditorService?.CreateSFCEditor((IRoutine) selectedProjectItem.AssociatedObject);
                        break;
                    case RoutineType.FBD:
                        createEditorService?.CreateFBDEditor((IRoutine) selectedProjectItem.AssociatedObject);
                        break;
                    //TODO(gjc): add other type
                }

            }

            var trend = selectedProjectItem.AssociatedObject as ITrend;
            if (trend != null)
            {
                createEditorService?.CreateTrend(trend);
            }
        }

        #endregion

        #region Group4

        private void AddGroup4Menus(OleMenuCommandService commandService)
        {
            var menuCommandID =
                new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.motionDirectCommandsCommand);
            var menuItem = new OleMenuCommand(MotionDirectCommands, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.manualTuneCommand);
            menuItem = new OleMenuCommand(ManualTune, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.monitorTagsCommand);
            menuItem = new OleMenuCommand(MonitorTags, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.editTagsCommand);
            menuItem = new OleMenuCommand(EditTags, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.verifyCommand);
            menuItem = new OleMenuCommand(VerifyCode, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.exportTagsCommand);
            menuItem = new OleMenuCommand(ExportTags, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.exportTrendCommand);
            menuItem = new OleMenuCommand(ExportTrend, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.crossReferenceCommand);
            menuItem = new OleMenuCommand(CrossReferenceCommand, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.monitorGroupTagCommand);
            menuItem = new OleMenuCommand(MonitorAxisTagCommand, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.monitorAxisTagCommand);
            menuItem = new OleMenuCommand(MonitorAxisTagCommand, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        private void MonitorAxisTagCommand(object sender, EventArgs e)
        {
            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();
            var tag = (ITag) selectedProjectItem.AssociatedObject;
            var createEditorService =
                Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;

            createEditorService?.CreateMonitorEditTags(tag.ParentController,
                tag.ParentController, tag.Name);
        }

        private void CrossReferenceCommand(object sender, EventArgs e)
        {
            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();
            var createEditorService =
                Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
            var tag = selectedProjectItem.AssociatedObject as ITag;
            if (tag != null)
            {
                createEditorService?.CreateCrossReference(
                    UIInterfaces.Editor.Type.Tag,
                    tag.ParentCollection.ParentProgram,
                    tag.Name);
            }

            var aoi = selectedProjectItem.AssociatedObject as AoiDefinition;
            if (aoi != null)
            {
                createEditorService?.CreateCrossReference(Type.AOI, aoi, aoi.Name);
            }
        }

        private void MotionDirectCommands(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            //1. get selected item
            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();
            if (selectedProjectItem.Kind == ProjectItemType.MotionGroup
                || selectedProjectItem.Kind == ProjectItemType.AxisCIPDrive
                || selectedProjectItem.Kind == ProjectItemType.AxisVirtual)
            {
                ICreateDialogService createDialogService =
                    ServiceProvider.GetService(typeof(SCreateDialogService)) as ICreateDialogService;

                var uiShell = (IVsUIShell) ServiceProvider.GetService(typeof(SVsUIShell));

                if (createDialogService != null)
                {
                    var window =
                        createDialogService.CreateMotionDirectCommandsDialog(
                            selectedProjectItem.AssociatedObject as ITag);

                    window.Show(uiShell);
                }
            }
        }

        private void ManualTune(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();

            if (selectedProjectItem.Kind == ProjectItemType.AxisCIPDrive)
            {
                var createDialogService =
                    (ICreateDialogService) Package.GetGlobalService(typeof(SCreateDialogService));

                var uiShell = (IVsUIShell) Package.GetGlobalService(typeof(SVsUIShell));

                var window =
                    createDialogService?.CreateManualTuneDialog(selectedProjectItem.AssociatedObject as ITag);
                window?.Show(uiShell);
            }

        }

        public void MonitorTags(object sender, EventArgs e)
        {
            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();
            IController controller = selectedProjectItem.AssociatedObject as IController;
            IProgramModule program = null;
            if (controller == null)
            {
                program = selectedProjectItem.AssociatedObject as IProgramModule;
                if (program != null)
                    controller = program.ParentController;
            }

            if (controller == null)
                return;

            IServiceProvider serviceProvider = _package;
            var createEditorService =
                serviceProvider?.GetService(typeof(SCreateEditorService)) as ICreateEditorService;

            createEditorService?.CreateMonitorEditTags(controller,
                program != null ? (ITagCollectionContainer) program : controller);
        }

        private void EditTags(object sender, EventArgs e)
        {
            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();
            IController controller = selectedProjectItem.AssociatedObject as IController;
            IProgramModule program = null;
            if (controller == null)
            {
                program = selectedProjectItem.AssociatedObject as IProgramModule;
                if (program != null)
                    controller = program.ParentController;
            }

            if (controller == null)
                return;

            IServiceProvider serviceProvider = _package;
            var createEditorService =
                serviceProvider?.GetService(typeof(SCreateEditorService)) as ICreateEditorService;

            createEditorService?.CreateMonitorEditTags(controller,
                program != null ? (ITagCollectionContainer) program : controller, "", true);
        }

        private void VerifyCode(object sender, EventArgs e)
        {
            //TODO(gjc): add code here
            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();

            IRoutine routine = selectedProjectItem.AssociatedObject as IRoutine;
            if (routine != null)
            {
                var outputService = Package.GetGlobalService(typeof(SErrorOutputService)) as IErrorOutputService;
                outputService?.Cleanup();

                VerifyRoutine(routine);

                outputService?.Summary();

                return;
            }

            IProgramModule program = selectedProjectItem.AssociatedObject as IProgramModule;
            if (program != null)
            {
                VerifyProgram(program);
            }
        }

        private void VerifyProgram(IProgramModule program)
        {
            if (program == null)
                return;

            var outputService = Package.GetGlobalService(typeof(SErrorOutputService)) as IErrorOutputService;
            outputService?.Cleanup();

            var service = Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;

            foreach (var routine in program.Routines)
            {
                service?.ParseRoutine(routine, true,true);
                //var stRoutine = routine as STRoutine;
                //if (stRoutine != null)
                //{
                //    service?.ParseRoutine(stRoutine, true);
                //}

                //TODO(gjc):add other routine
            }

            outputService?.Summary();
        }

        private void VerifyRoutine(IRoutine routine)
        {
            var service = Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
            service?.ParseRoutine(routine, true, true);
        }

        private void ExportTags(object sender, EventArgs e)
        {
            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();

            var saveFileDialog = new SaveFileDialog
            {
                Title = "Export",
                Filter = "IcsStudio Import/Export File(*.CSV)|*.CSV|IcsStudio Unicode Import/Export File(*.TXT)|*.TXT"
            };

            StringBuilder stringBuilder = new StringBuilder();
            string scope = string.Empty;
            ITagCollection tagCollection = null;
            var projectFileNameWithoutExtension =
                System.IO.Path.GetFileNameWithoutExtension(selectedProjectItem.AssociatedObject.ParentController
                    .ProjectLocaleName);

            string type = "";

            //if is aoi
            var aoiDefinition = selectedProjectItem.AssociatedObject as IAoiDefinition;
            if (aoiDefinition != null)
            {
                saveFileDialog.FileName = projectFileNameWithoutExtension + "_" + aoiDefinition.Name + "_Tags";
                tagCollection = aoiDefinition.Tags;
                scope = aoiDefinition.Name;
                type = "AOI";
            }

            //if is controller
            var conDefinition = selectedProjectItem.AssociatedObject as Controller;
            if (conDefinition != null)
            {
                saveFileDialog.FileName = projectFileNameWithoutExtension + "_Controller_Tags";
                tagCollection = conDefinition.Tags;
                scope = "";
                type = "Controller";
            }

            //if is program
            var proDefinition = selectedProjectItem.AssociatedObject as Program;
            if (proDefinition != null)
            {
                saveFileDialog.FileName = projectFileNameWithoutExtension + "_" + proDefinition.Name + "_Tags";
                tagCollection = proDefinition.Tags;
                scope = proDefinition.Name;
                type = "Program";
            }

            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

            Encoding encoding = Encoding.UTF8;

            string separator = "";

            // selected -> contents
            if (saveFileDialog.FileName.EndsWith("TXT"))
            {
                encoding = Encoding.Unicode;

                separator = "\t";
            }
            else if (saveFileDialog.FileName.EndsWith("CSV", StringComparison.OrdinalIgnoreCase))
            {
                separator = ",";
            }
            else
            {
                throw new NotImplementedException();
            }

            string contents = Controller.ExportAllTags(tagCollection, separator, scope, stringBuilder,type);

            // contents -> file
            File.WriteAllText(saveFileDialog.FileName, contents, encoding);
        }

        private void ExportTrend(object sender, EventArgs e)
        {
            ExportData();
        }

        #endregion

        #region Group5

        private void AddGroup5Menus(OleMenuCommandService commandService)
        {
            var menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.browseLogicCommand);
            var menuItem = new OleMenuCommand(BrowseLogic, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        private void BrowseLogic(object sender, EventArgs e)
        {

        }

        #endregion

        #region Group8

        private void AddGroup8Menus(OleMenuCommandService commandService)
        {
            var menuCommandID =
                new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.exportDataTypeCommand);
            var menuItem = new OleMenuCommand(ExportDataType, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID =
                new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.exportAddOnInstructionCommand);
            menuItem = new OleMenuCommand(ExportAddOnInstruction, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID =
                new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.exportProgramCommand);
            menuItem = new OleMenuCommand(ExportProgram, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID =
                new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.exportRoutineCommand);
            menuItem = new OleMenuCommand(ExportRoutine, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID =
                new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.exportModuleCommand);
            menuItem = new OleMenuCommand(ExportModule, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID =
                new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.exportStringTypeCommand);
            menuItem = new OleMenuCommand(ExportDataType, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

        }

        private void ExportDataType(object sender, EventArgs e)
        {
            ExportData();
        }

        private void ExportAddOnInstruction(object sender, EventArgs e)
        {
            ExportData();
        }

        private void ExportProgram(object sender, EventArgs e)
        {
            ExportData();
        }

        private void ExportRoutine(object sender, EventArgs e)
        {
            ExportData();
        }

        private void ExportModule(object sender, EventArgs e)
        {
            ExportData();
        }

        private void ExportData()
        {
            //TODO(zyl):refactoring
            var selectedProjectItems = ServiceProvider.GetSelectedProjectItems();
            if (!selectedProjectItems.Any()) return;
            var selectedProjectItem = selectedProjectItems.First();
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "Save file",
                Filter = "json文件(*.json)|*.json"
            };

            //TODO(TLM):Restructure Or Add New Type
            if (selectedProjectItem.AssociatedObject is CIPMotionDrive)
            {
                saveFileDialog.FileName = (selectedProjectItem.AssociatedObject as CIPMotionDrive).Name + "_Module";
            }
            else if (selectedProjectItem.AssociatedObject is DiscreteIO)
            {
                saveFileDialog.FileName = (selectedProjectItem.AssociatedObject as DiscreteIO).Name + "_Module";
            }
            else if (selectedProjectItem.AssociatedObject is CommunicationsAdapter)
            {
                saveFileDialog.FileName =
                    (selectedProjectItem.AssociatedObject as CommunicationsAdapter).Name + "_Module";
            }
            else if (selectedProjectItem.Kind == ProjectItemType.String)
            {
                saveFileDialog.FileName = (selectedProjectItem.AssociatedObject as UserDefinedDataType).Name +
                                          "_DataType_String";
            }
            else if (selectedProjectItem.Kind == ProjectItemType.UserDefined)
            {
                saveFileDialog.FileName =
                    (selectedProjectItem.AssociatedObject as UserDefinedDataType).Name + "_DataType";
            }
            else
            {
                saveFileDialog.FileName = selectedProjectItem.DisplayName;
            }

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (var sw = File.CreateText(saveFileDialog.FileName))
                {
                    using (var jw = new JsonTextWriter(sw))
                    {
                        jw.Formatting = Formatting.Indented;
                        var targetList = new List<IBaseObject>();
                        foreach(var item in selectedProjectItems)   targetList.Add(item.AssociatedObject);
                        Export(targetList).WriteTo(jw);
                    }
                }
            }
        }

        #region Export

        private JObject Export(List<IBaseObject> list)
        {
            var controller = new JObject();
            var dataTypes = new JArray();
            var aois = new JArray();
            var programs = new JArray();
            var tags = new JArray();
            var modules = new JArray();
            var trends = new JArray();
            controller.Add("DataTypes", dataTypes);
            controller.Add("AddOnInstructionDefinitions", aois);
            controller.Add("Programs", programs);
            controller.Add("Tags", tags);
            controller.Add("Modules", modules);
            controller.Add("Trends", trends);
            foreach (var baseObject in list)
            {
                var dataType = baseObject as UserDefinedDataType;
                if (dataType != null)
                {
                    DataTypeExport(controller, dataType, true);
                }

                var aoi = baseObject as AoiDefinition;
                if (aoi != null)
                {
                    AoiExport(controller, aoi, true);
                }

                var program = baseObject as Program;
                if (program != null)
                {
                    ProgramExport(controller, program, true);
                }

                var routine = baseObject as IRoutine;
                if (routine != null)
                {
                    RoutineExport(controller, routine, true);
                }

                var module = baseObject as DeviceModule;
                if (module != null)
                {
                    var config = module.ConvertToJObject();
                    config.Add("Use", "Target");
                    modules.Add(config);
                    foreach (var deviceModule in module.ParentController.DeviceModules.Where(d =>
                                 ((DeviceModule)d).ParentModule == module))
                    {
                        modules.Add(((DeviceModule)deviceModule).ConvertToJObject());
                    }
                }

                var trend = baseObject as Trend;
                if (trend != null)
                {
                    var config = trend.ToJson();
                    config.Add("Use", "Target");
                    trends.Add(config);
                }
            }

            return controller;
        }

        public void RungsExport(RLLRoutine rllRoutine,int startIndex,int endIndex)
        {
            var contextRoutine = DeepCloneUtil.DeepCloneByExpressionTrees(rllRoutine);
            var rungs = contextRoutine.CloneRungs();
            if (startIndex == rungs.Count) startIndex--;
            if (endIndex == rungs.Count) endIndex--;
            if(startIndex < 0 || endIndex >= rungs.Count || startIndex > endIndex)
                throw new ArgumentOutOfRangeException("The index is out of range!");
            var targetRungs = new List<RungType>();
            for (int i = startIndex; i <= endIndex; i++) targetRungs.Add(rungs[i]);
            contextRoutine.Rungs.Clear();
            contextRoutine.Rungs.AddRange(targetRungs);
            contextRoutine.CodeText.Clear();

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "Export Rungs",
                Filter = "json文件(*.json)|*.json",
                FileName = startIndex == endIndex? $"Rung{startIndex}_from_{rllRoutine.Name}": $"Rungs{startIndex}to{endIndex}_from_{rllRoutine.Name}"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (var sw = File.CreateText(saveFileDialog.FileName))
                {
                    using (var jw = new JsonTextWriter(sw))
                    {
                        jw.Formatting = Formatting.Indented;

                        var controllerJObj = Export(new List<IBaseObject>(){contextRoutine});
                        //make a secondary correction;
                        var program = controllerJObj["Programs"]?[0];
                        if (program == null) throw new Exception("Export failed!");
                        program["Use"] = "Context";
                        var routine = program["Routines"]?[0];
                        if (routine == null) throw new Exception("Export failed!");
                        routine["Use"] = "Context";
                        (routine as JObject)?.Remove("CodeText");
                        if (routine["Rungs"] == null) throw new Exception("Export failed!");
                        var index = startIndex;
                        foreach (var item in routine["Rungs"])
                        {
                            item["Use"] = "Target";
                            item["Number"] = index++;
                        }
                        controllerJObj.WriteTo(jw);
                    }
                }
            }
        }

        private void RoutineExport(JObject controller, IRoutine routine, bool isTarget)
        {
            var tmp = routine.ConvertToJObject(false);
            var editorService =
                Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
            var tags = editorService?.ParseRoutineTag(routine);
            if (isTarget)
                tmp["Use"] = "Target";
            var routines = new JArray(tmp);
            var program = new JObject();
            program.Add("Routines", routines);
            program.Add("Name", routine.ParentCollection.ParentProgram.Name);
            var programTags = new JArray();
            program.Add("Tags", programTags);
            if (tags != null)
            {
                foreach (Tag tag in tags.Where(t => t.ParentCollection == routine.ParentCollection.ParentProgram.Tags))
                {
                    TagExport(controller, tag);
                    programTags.Add(tag.ConvertToJObject());
                }

                foreach (Tag tag in tags.Where(t => t.ParentCollection.ParentProgram == null))
                {
                    TagExport(controller, tag);
                    AddConfig(controller, tag.ConvertToJObject(), ProjectItemType.ControllerTags);
                }

                foreach (Tag tag in tags.Where(t =>
                    t.ParentCollection != routine.ParentCollection.ParentProgram.Tags &&
                    t.ParentCollection.ParentProgram != null))
                {
                    AddReferenceTag(tag, controller);
                }
            }

            AddConfig(controller, program, ProjectItemType.Program);
        }

        private void ProgramExport(JObject controller, Program program, bool isTarget)
        {
            var tmp = program.ConvertToJObject(false);
            if (isTarget)
                tmp["Use"] = "Target";
            foreach (var tag in program.Tags)
            {
                TagExport(controller, tag);
            }

            var editorService =
                Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
            foreach (var programRoutine in program.Routines)
            {
                var tags = editorService?.ParseRoutineTag(programRoutine);
                if (tags != null)
                {
                    foreach (Tag tag in tags.Where(t =>
                        t.ParentCollection.ParentProgram == null))
                    {
                        TagExport(controller, tag);
                        AddConfig(controller, tag.ConvertToJObject(), ProjectItemType.ControllerTags);
                    }

                    foreach (Tag tag in tags.Where(t =>
                        t.ParentCollection.ParentProgram != program &&
                        t.ParentCollection.ParentProgram != null))
                    {
                        AddReferenceTag(tag, controller);
                    }
                }
            }

            AddConfig(controller, tmp, ProjectItemType.Program);
        }

        private void AddReferenceTag(Tag tag, JObject controller)
        {
            string programName = tag.ParentCollection.ParentProgram.Name;
            var programs = controller["Programs"] as JArray;
            Debug.Assert(programs != null);
            var program = programs.FirstOrDefault(p => programName.Equals((string) p["Name"]));
            if (program == null)
            {
                program = new JObject();
                program["Name"] = programName;
                program["Use"] = "Context";
                program["Tags"] = new JArray();
                program["Routines"] = new JArray();
                program["Type"] = 1;
                programs.Add(program);
            }

            var t = new JObject();
            t["Name"] = tag.Name;
            t["Use"] = "Reference";
            ((JArray) program["Tags"]).Add(t);
        }

        private void TagExport(JObject controller, ITag tag)
        {
            if (tag.IsAlias)
            {
                var baseTag = ObtainValue.NameToTag(tag.AliasSpecifier, null).Item1;
                if (tag.AliasSpecifier.IndexOf("\\", StringComparison.Ordinal) > -1)
                    AddOtherProgramTag(controller, baseTag);
                else
                {
                    TagExport(controller, baseTag);
                    AddConfig(controller, ((Tag) baseTag).ConvertToJObject(), ProjectItemType.ControllerTags);
                }

                return;
            }

            var udi = tag.DataTypeInfo.DataType as UserDefinedDataType;
            if (udi != null)
            {
                DataTypeExport(controller, udi, false);
                return;
            }

            if (tag.DataTypeInfo.DataType is AXIS_CIP_DRIVE)
            {
                var axisCIPDrive = ((Tag) tag).DataWrapper as AxisCIPDrive;
                var mg = axisCIPDrive?.AssignedGroup;
                if (mg != null)
                {
                    AddConfig(controller, ((Tag) mg).ConvertToJObject(), ProjectItemType.ControllerTags);
                }

                return;
            }

            if (tag.DataTypeInfo.DataType is AXIS_VIRTUAL)
            {
                var axisVirtual = ((Tag) tag).DataWrapper as AxisVirtual;
                var mg = axisVirtual?.AssignedGroup;
                if (mg != null)
                {
                    AddConfig(controller, ((Tag) mg).ConvertToJObject(), ProjectItemType.ControllerTags);
                }

                return;
            }

            var aoiDataType = tag.DataTypeInfo.DataType as AOIDataType;
            if (aoiDataType != null)
            {
                var aoiT = (Controller.GetInstance().AOIDefinitionCollection as AoiDefinitionCollection)?.Find(
                    aoiDataType.Name);
                AoiExport(controller, aoiT, false);
            }
        }

        private void AddOtherProgramTag(JObject controller, ITag tag)
        {
            Debug.Assert(tag.ParentCollection.ParentProgram != null);
            var program = controller["Programs"].FirstOrDefault(p =>
                (bool) tag.ParentCollection.ParentProgram?.Name.Equals(p["Name"]?.ToString())) as JObject;
            if (program == null)
            {
                program = new JObject();
                program.Add("Name", tag.ParentCollection.ParentProgram.Name);
                var tags2 = new JArray();
                program.Add("Tags", tags2);
                (controller["Programs"] as JArray)?.Add(program);
            }

            var tags = (JArray) program["Tags"];
            foreach (var tag1 in tags)
            {
                if (tag.Name.Equals(tag1["Name"]?.ToString())) return;
            }

            TagExport(controller, tag);
            tags.Add(((Tag) tag).ConvertToJObject());
        }

        private void AoiExport(JObject controller, AoiDefinition aoi, bool isTarget)
        {
            var tmp = aoi.GetConfig();
            if (isTarget)
                tmp["Use"] = "Target";
            foreach (var aoiTag in aoi.Tags)
            {
                TagExport(controller, aoiTag);
            }

            AddConfig(controller, tmp, ProjectItemType.AddOnDefined);
        }

        private void DataTypeExport(JObject controller, UserDefinedDataType udi, bool isTarget)
        {
            var tmp = udi.ConvertToJObject();
            tmp.Remove("Use"); // the "Use" Property is already existed and the value is "Target";
            if (isTarget)
                tmp["Use"] = "Target";
            foreach (var member in udi.TypeMembers)
            {
                var memberDataType = member.DataTypeInfo.DataType as UserDefinedDataType;
                if (memberDataType != null)
                {
                    DataTypeExport(controller, memberDataType, false);
                }

                var aoiMember = member.DataTypeInfo.DataType as AOIDataType;
                if (aoiMember != null)
                {
                    var aoi =
                        (Controller.GetInstance().AOIDefinitionCollection as AoiDefinitionCollection)?.Find(
                            aoiMember.Name);
                    AoiExport(controller, aoi, false);
                }
            }

            AddConfig(controller, tmp, ProjectItemType.UserDefined);
        }

        private void AddConfig(JObject controller, JObject config, ProjectItemType type)
        {
            if (controller == null || config == null) return;
            if (type == ProjectItemType.UserDefined)
            {
                var dataTypes = controller["DataTypes"] as JArray;
                foreach (var dataType in dataTypes)
                {
                    if (dataType["Name"].ToString()
                        .Equals(config["Name"].ToString(), StringComparison.OrdinalIgnoreCase)) return;
                }

                dataTypes.Add(config);
                return;
            }

            if (type == ProjectItemType.AddOnDefined)
            {
                var aois = controller["AddOnInstructionDefinitions"] as JArray;
                foreach (var aoi in aois)
                {
                    if (aoi["Name"].ToString()
                        .Equals(config["Name"].ToString(), StringComparison.OrdinalIgnoreCase)) return;
                }

                aois.Add(config);
                return;
            }

            if (type == ProjectItemType.Program)
            {
                var programs = controller["Programs"] as JArray;
                foreach (var program in programs)
                {
                    if (program["Name"].ToString()
                        .Equals(config["Name"].ToString(), StringComparison.OrdinalIgnoreCase)) return;
                }

                programs.Add(config);
                return;
            }

            if (type == ProjectItemType.ControllerTags)
            {
                var tags = controller["Tags"] as JArray;
                foreach (var tag in tags)
                {
                    if (tag["Name"].ToString()
                        .Equals(config["Name"].ToString(), StringComparison.OrdinalIgnoreCase)) return;
                }

                tags.Add(config);
            }
        }

        #endregion

        #endregion

        #region TaskAdd

        private void AddTaskAddMenus(OleMenuCommandService commandService)
        {
            var menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.taskAddMenu);
            var menuItem = new OleMenuCommand(null, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.taskNewProgramCommand);
            menuItem = new OleMenuCommand(NewProgram, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.newEquipmentPhaseCommand);
            menuItem = new OleMenuCommand(NewEquipmentPhase, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.newEquipmentSequenceCommand);
            menuItem = new OleMenuCommand(NewEquipmentSequence, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.taskImportProgramCommand);
            menuItem = new OleMenuCommand(ImportProgram, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.importEquipmentPhaseCommand);
            menuItem = new OleMenuCommand(ImportEquipmentPhase, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet,
                PackageIds.importEquipmentSequenceCommand);
            menuItem = new OleMenuCommand(ImportEquipmentSequence, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        private void ImportEquipmentSequence(object sender, EventArgs e)
        {
            IProjectInfoService projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            var selectedProject = _package.GetSelectedProjectItem();
            projectInfoService?.ImportData(selectedProject.Kind, selectedProject.AssociatedObject);
        }

        private void ImportEquipmentPhase(object sender, EventArgs e)
        {
            IProjectInfoService projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            var selectedProject = _package.GetSelectedProjectItem();
            projectInfoService?.ImportData(selectedProject.Kind, selectedProject.AssociatedObject);
        }

        private void NewEquipmentSequence(object sender, EventArgs e)
        {
            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();
            ICreateDialogService createDialogService =
                ServiceProvider.GetService(typeof(SCreateDialogService)) as ICreateDialogService;

            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = (IVsUIShell) ServiceProvider.GetService(typeof(SVsUIShell));

            if (createDialogService != null)
            {
                var dialog = createDialogService.CreateProgramDialog(ProgramType.Sequence,
                    (ITask) selectedProjectItem.AssociatedObject);
                dialog.ShowDialog(uiShell);
            }
        }

        private void NewEquipmentPhase(object sender, EventArgs e)
        {
            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();
            ICreateDialogService createDialogService =
                ServiceProvider.GetService(typeof(SCreateDialogService)) as ICreateDialogService;

            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = (IVsUIShell) ServiceProvider.GetService(typeof(SVsUIShell));

            if (createDialogService != null)
            {
                var dialog = createDialogService.CreateProgramDialog(ProgramType.Phase,
                    (ITask) selectedProjectItem.AssociatedObject);
                dialog.ShowDialog(uiShell);
            }
        }

        #endregion

        #region ProgramAdd

        private void AddProgramAddMenus(OleMenuCommandService commandService)
        {
            var menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.programAddMenu);
            var menuItem = new OleMenuCommand(null, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.newPhaseStateCommand);
            menuItem = new OleMenuCommand(NewPhaseState, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.newRoutineCommand);
            menuItem = new OleMenuCommand(NewRoutine, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.programNewLocalTagCommand);
            menuItem = new OleMenuCommand(NewLocalTag, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.programNewParameterCommand);
            menuItem = new OleMenuCommand(NewParameter, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.importRoutineCommand);
            menuItem = new OleMenuCommand(ImportRoutine, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

        }

        private void ImportRoutine(object sender, EventArgs e)
        {
            IProjectInfoService projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            var selectedProject = _package.GetSelectedProjectItem();
            projectInfoService?.ImportData(selectedProject.Kind, selectedProject.AssociatedObject);
        }

        private void NewPhaseState(object sender, EventArgs e)
        {
            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();
            ICreateDialogService createDialogService =
                ServiceProvider.GetService(typeof(SCreateDialogService)) as ICreateDialogService;

            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = (IVsUIShell) ServiceProvider.GetService(typeof(SVsUIShell));

            if (createDialogService != null)
            {
                var dialog =
                    createDialogService.CreatePhaseStateDialog((IProgram) selectedProjectItem.AssociatedObject);
                dialog.ShowDialog(uiShell);
            }
        }

        private void NewRoutine(object sender, EventArgs e)
        {

            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();
            ICreateDialogService createDialogService =
                ServiceProvider.GetService(typeof(SCreateDialogService)) as ICreateDialogService;

            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = (IVsUIShell) ServiceProvider.GetService(typeof(SVsUIShell));

            if (createDialogService != null)
            {
                var dialog = createDialogService.CreateRoutineDialog((IProgram) selectedProjectItem.AssociatedObject);
                dialog.ShowDialog(uiShell);
            }
        }

        #endregion

        #region PrintAdd
        private void AddPrintContextMenu(OleMenuCommandService commandService)
        {
            var menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.PrintContextMenu);
            var menuItem = new OleMenuCommand(PrintMenu, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printRoutine2);
            menuItem = new OleMenuCommand(Print, menuCommandID);
            menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printRoutineProperties2);
            menuItem = new OleMenuCommand(Print, menuCommandID);
            menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printTags2);
            menuItem = new OleMenuCommand(Print, menuCommandID);
            menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printSequencingAndStepTags2);
            menuItem = new OleMenuCommand(Print, menuCommandID);
            menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printModuleProperties2);
            menuItem = new OleMenuCommand(Print, menuCommandID);
            menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printAdd_OnInstructionSignatures2);
            menuItem = new OleMenuCommand(Print, menuCommandID);
            menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printAdd_OnInstruction2);
            menuItem = new OleMenuCommand(Print, menuCommandID);
            menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printDataType2);
            menuItem = new OleMenuCommand(Print, menuCommandID);
            menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printCrossReference2);
            menuItem = new OleMenuCommand(Print, menuCommandID);
            menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printControllerOrganizer2);
            menuItem = new OleMenuCommand(Print, menuCommandID);
            menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printControllerProperties2);
            menuItem = new OleMenuCommand(Print, menuCommandID);
            menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printTaskProperties2);
            menuItem = new OleMenuCommand(Print, menuCommandID);
            menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printProgramProperties2);
            menuItem = new OleMenuCommand(Print, menuCommandID);
            menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printEquipmentSequence2);
            menuItem = new OleMenuCommand(Print, menuCommandID);
            menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printEntireProject2);
            menuItem = new OleMenuCommand(Print, menuCommandID);
            menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        private void PrintMenu(object sender, EventArgs e)
        {

        }
        private void Print(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var menuCommand = sender as OleMenuCommand;
            OpenPrintWindow(menuCommand);
        }

        internal void OpenPrintWindow(OleMenuCommand menuCommand)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var selectedProjectItems = ServiceProvider.GetSelectedProjectItems();
            var uiShell = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            IProjectItem selectedProjectItem = null;
            if (selectedProjectItems.Count > 0)
                selectedProjectItem = selectedProjectItems[0];
            try
            {
                if (selectedProjectItem != null)
                {
                    var task = selectedProjectItem.AssociatedObject as CTask;
                }

                var data = new PrintVM(menuCommand, selectedProjectItem);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        #endregion




        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            //TODO(gjc): need edit here
            OleMenuCommand menuCommand = sender as OleMenuCommand;

            var controller = ServiceProvider.GetController();
            var selectedProjectItems = ServiceProvider.GetSelectedProjectItems();
            IProjectItem selectedProjectItem = null;
            if (selectedProjectItems.Count > 0)
                selectedProjectItem = selectedProjectItems[0];

            if (menuCommand != null)
            {
                menuCommand.Visible = false;

                if (controller != null && selectedProjectItem != null)
                {
                    switch (menuCommand.CommandID.ID)
                    {
                        case PackageIds.PrintContextMenu:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Print");
                            if (selectedProjectItem.Kind == ProjectItemType.ControllerModel
                                || selectedProjectItem.Kind == ProjectItemType.Routine
                                || selectedProjectItem.Kind == ProjectItemType.Program
                                || selectedProjectItem.Kind == ProjectItemType.Task
                                || selectedProjectItem.Kind == ProjectItemType.Predefined
                                || selectedProjectItem.Kind == ProjectItemType.UserDefined
                                || selectedProjectItem.Kind == ProjectItemType.String
                                || selectedProjectItem.Kind == ProjectItemType.AddOnDefined
                                || selectedProjectItem.Kind == ProjectItemType.ModuleDefined
                                || selectedProjectItem.Kind == ProjectItemType.AddOnInstruction
                                || selectedProjectItem.Kind == ProjectItemType.Bus
                                || selectedProjectItem.Kind == ProjectItemType.DeviceModule
                                || selectedProjectItem.Kind == ProjectItemType.LocalModule
                                || selectedProjectItem.Kind == ProjectItemType.ControllerTags
                                || selectedProjectItem.Kind == ProjectItemType.ProgramTags
                                || selectedProjectItem.Kind == ProjectItemType.MotionGroup
                                || selectedProjectItem.Kind == ProjectItemType.Ethernet)
                            {
                                menuCommand.Visible = true;
                            }
                            else
                            {
                                menuCommand.Visible = false;
                            }
                            break;
                        case PackageIds.browseLogicCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Browse Logic...");
                            if (selectedProjectItem.Kind == ProjectItemType.Routine ||
                                selectedProjectItem.Kind == ProjectItemType.AddOnInstruction)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = true;
                            }

                            break;
                        case PackageIds.openDefinitionCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Open Definition");
                            if (selectedProjectItem.Kind == ProjectItemType.AddOnDefined ||
                                selectedProjectItem.Kind == ProjectItemType.AddOnInstruction)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = true;
                            }

                            break;
                        case PackageIds.verifyControllerCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Verify");
                            if (selectedProjectItem.Kind == ProjectItemType.ControllerModel)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = !controller.IsOnline;
                            }

                            break;
                        case PackageIds.newTaskCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("New Task...");
                            if (selectedProjectItem.Kind == ProjectItemType.Tasks)
                                menuCommand.Visible = true;

                            //TODO(gjc): edit later
                            menuCommand.Enabled = !controller.IsOnline;

                            if (menuCommand.Enabled && selectedProjectItem.ProjectItems.Count >= 17)
                            {
                                menuCommand.Enabled = false;
                            }

                            //if (controller.IsOnline 
                            //    && controller.KeySwitchPosition == ControllerKeySwitch.RunKeySwitch)
                            //    menuCommand.Enabled = false;
                            //else
                            //    menuCommand.Enabled = true;


                            break;
                        case PackageIds.newMotionGroupCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("New Axes Groups...");
                            if (selectedProjectItem.Kind == ProjectItemType.MotionGroups)
                            {
                                menuCommand.Visible = true;

                                if (controller.IsOnline)
                                {
                                    menuCommand.Enabled = false;
                                }
                                else
                                {
                                    menuCommand.Enabled = !controller.HasMotionGroup();
                                }

                            }

                            break;
                        case PackageIds.newModuleCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("New Module...");
                            if (selectedProjectItem.Kind == ProjectItemType.IOConfiguration)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = false;
                            }
                            else if (selectedProjectItem.Kind == ProjectItemType.Ethernet)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = true;
                            }
                            else if (selectedProjectItem.Kind == ProjectItemType.Bus
                                     || selectedProjectItem.Kind == ProjectItemType.ExpansionIO)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled =
                                    CheckNewModuleCommandEnabled(controller, selectedProjectItem.AssociatedObject);
                            }
                            else if (selectedProjectItem.Kind == ProjectItemType.DeviceModule)
                            {
                                DeviceModule deviceModule = selectedProjectItem.AssociatedObject as DeviceModule;

                                if (deviceModule != null &&
                                    !(deviceModule is LocalModule))
                                {
                                    var pointIOPort = deviceModule.GetFirstPort(PortType.PointIO);
                                    if (pointIOPort != null)
                                    {
                                        if (!pointIOPort.Upstream)
                                        {
                                            menuCommand.Visible = true;
                                            menuCommand.Enabled =
                                                !selectedProjectItem.DisplayName.StartsWith("[0]")
                                                && CheckNewModuleCommandEnabled(controller, deviceModule);
                                        }
                                    }
                                }
                            }

                            if (controller.IsOnline)
                            {
                                menuCommand.Enabled = false;
                            }

                            break;
                        case PackageIds.discoverModulesCommand:
                            if (selectedProjectItem.Kind == ProjectItemType.IOConfiguration)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = false;
                            }

                            break;
                        case PackageIds.newAxisMenu:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("New Axis");
                            if (selectedProjectItem.Kind == ProjectItemType.MotionGroup ||
                                selectedProjectItem.Kind == ProjectItemType.UngroupedAxes)
                            {
                                menuCommand.Visible = true;
                            }

                            break;

                        case PackageIds.AXIS_CIP_DRIVECommand:
                           
                        case PackageIds.AXIS_VIRTUALCommand:
                            var textKey = GetMenuCommandText(menuCommand.CommandID.ID);
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier(textKey);
                            if (selectedProjectItem.Kind == ProjectItemType.MotionGroup ||
                                selectedProjectItem.Kind == ProjectItemType.UngroupedAxes)
                            {
                                menuCommand.Visible = true;

                                if (controller.IsOnline && selectedProjectItem.Kind == ProjectItemType.MotionGroup)
                                {
                                    menuCommand.Enabled = false;
                                }
                                else
                                {
                                    menuCommand.Enabled = true;
                                }

                            }
                            break;

                        case PackageIds.newCoordinateSystemCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("New Coordinate System...");
                            if (selectedProjectItem.Kind == ProjectItemType.MotionGroup ||
                                selectedProjectItem.Kind == ProjectItemType.UngroupedAxes)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = !controller.IsOnline;
                            }

                            break;
                        case PackageIds.newAddOnInstructionCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("New Add-On Instruction...");
                            if (selectedProjectItem.Kind == ProjectItemType.AddOnInstructions ||
                                selectedProjectItem.Kind == ProjectItemType.AddOnDefineds)
                            {
                                menuCommand.Visible = selectedProjectItem.AssociatedObject == null;
                                menuCommand.Enabled = !controller.IsOnline;
                            }

                            break;

                        case PackageIds.newDataTypeCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("New Data Type...");
                            if (selectedProjectItem.Kind == ProjectItemType.DataTypes ||
                                selectedProjectItem.Kind == ProjectItemType.UserDefineds)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = !controller.IsOnline;
                            }

                            break;
                        case PackageIds.newStringTypeCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("New String Type...");
                            if (selectedProjectItem.Kind == ProjectItemType.DataTypes ||
                                selectedProjectItem.Kind == ProjectItemType.Strings)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = !controller.IsOnline;
                            }

                            break;
                        case PackageIds.newTrendCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("New Trace...");
                            if (selectedProjectItem.Kind == ProjectItemType.Trends)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = controller.KeySwitchPosition != ControllerKeySwitch.RunKeySwitch;
                            }

                            break;
                        case PackageIds.openCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Open");
                            if (selectedProjectItem.Kind == ProjectItemType.Routine)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = true;
                            }

                            if (selectedProjectItem.Kind == ProjectItemType.Trend)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = EnableCommand(selectedProjectItems);
                            }

                            break;

                        case PackageIds.motionDirectCommandsCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Motion Direct Commands...");
                            if (selectedProjectItem.Kind == ProjectItemType.AxisCIPDrive
                                || selectedProjectItem.Kind == ProjectItemType.AxisVirtual
                                || selectedProjectItem.Kind == ProjectItemType.MotionGroup)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = true;
                            }

                            break;
                        case PackageIds.manualTuneCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Manual Tune..");
                            if (selectedProjectItem.Kind == ProjectItemType.AxisCIPDrive)
                            {
                                menuCommand.Visible = true;

                                //TODO(gjc): add axis configuration check here 
                                menuCommand.Enabled = true;
                            }
                            else if (selectedProjectItem.Kind == ProjectItemType.AxisVirtual)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = false;
                            }

                            break;
                        case PackageIds.monitorTagsCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Monitor Variable");
                            if (selectedProjectItem.Kind == ProjectItemType.ControllerTags
                                || selectedProjectItem.Kind == ProjectItemType.ProgramTags)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = true;
                            }

                            break;
                        case PackageIds.editTagsCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Edit Variable");
                            if (selectedProjectItem.Kind == ProjectItemType.ControllerTags
                                || selectedProjectItem.Kind == ProjectItemType.ProgramTags)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = true;
                            }

                            break;

                        case PackageIds.verifyCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Verify");
                            if (selectedProjectItem.Kind == ProjectItemType.Routine ||
                                selectedProjectItem.Kind == ProjectItemType.Program ||
                                selectedProjectItem.Kind == ProjectItemType.AddOnInstruction)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = true;
                            }

                            break;

                        case PackageIds.exportTagsCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Export Variable");
                            if (selectedProjectItem.Kind == ProjectItemType.ControllerTags
                                || selectedProjectItem.Kind == ProjectItemType.ProgramTags)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = true;
                            }

                            break;
                        case PackageIds.taskAddMenu:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Add");
                            if (selectedProjectItem.Kind == ProjectItemType.Task ||
                                selectedProjectItem.Kind == ProjectItemType.UnscheduledPrograms)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = true;
                            }

                            //TODO(gjc): edit later
                            menuCommand.Enabled = !controller.IsOnline;

                            break;

                        case PackageIds.newProgramCommand:
                           
                        case PackageIds.importProgramCommand:
                            textKey = GetMenuCommandText(menuCommand.CommandID.ID);
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier(textKey);
                            if (selectedProjectItem.Kind == ProjectItemType.FaultHandler)
                            {
                                if (string.IsNullOrEmpty(controller.MajorFaultProgram))
                                {
                                    menuCommand.Visible = true;
                                    menuCommand.Enabled = true;
                                }
                                else
                                {
                                    menuCommand.Visible = true;
                                    menuCommand.Enabled = false;
                                }
                            }
                            else if (selectedProjectItem.Kind == ProjectItemType.PowerHandler)
                            {
                                if (string.IsNullOrEmpty(controller.PowerLossProgram))
                                {
                                    menuCommand.Visible = true;
                                    menuCommand.Enabled = true;
                                }
                                else
                                {
                                    menuCommand.Visible = true;
                                    menuCommand.Enabled = false;
                                }
                            }

                            //TODO(gjc): edit later
                            menuCommand.Enabled = !controller.IsOnline;
                            break;

                        //TODO(clx):newEquipmentPhase、newEquipmentSequence暂不支持，否则和taskNewProgram一样需要限制
                        case PackageIds.taskNewProgramCommand:
                            textKey = GetMenuCommandText(menuCommand.CommandID.ID);
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier(textKey);
                            menuCommand.Visible = true;
                            menuCommand.Enabled = !controller.IsOnline;

                            if (menuCommand.Enabled && selectedProjectItem.ProjectItems.Count >= 1000)
                            {
                                menuCommand.Enabled = false;
                            }

                            break;

                        case PackageIds.newEquipmentPhaseCommand:
                        case PackageIds.newEquipmentSequenceCommand:
                        case PackageIds.importEquipmentPhaseCommand:
                        case PackageIds.importEquipmentSequenceCommand:
                            textKey = GetMenuCommandText(menuCommand.CommandID.ID);
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier(textKey);
                            menuCommand.Visible = true;
                            menuCommand.Enabled = false;
                            break;

                        case PackageIds.taskImportProgramCommand:
                        case PackageIds.programNewLocalTagCommand:
                        case PackageIds.programNewParameterCommand:
                            textKey = GetMenuCommandText(menuCommand.CommandID.ID);
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier(textKey);
                            menuCommand.Visible = true;
                            menuCommand.Enabled = !controller.IsOnline;
                            break;

                        case PackageIds.newTagCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("New Variable...");
                            if (selectedProjectItem.Kind == ProjectItemType.ControllerTags)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = true;
                            }
                            break;

                        case PackageIds.programAddMenu:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Add");
                            if (selectedProjectItem.Kind == ProjectItemType.Program)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = true;
                            }
                            break;
                        case PackageIds.newPhaseStateCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("New Phase State Routine...");
                            IProgram program = selectedProjectItem.AssociatedObject as IProgram;
                            if (program != null && program.Type == ProgramType.Phase)
                            {
                                menuCommand.Visible = true;
                                int count = 0;
                                foreach (var routine in program.Routines)
                                {
                                    if (routine.Name == "Aborting" || routine.Name == "Resetting" ||
                                        routine.Name == "Restarting" ||
                                        routine.Name == "Running" ||
                                        routine.Name == "Stopping")
                                    {
                                        count++;
                                    }
                                }

                                menuCommand.Enabled = count != 5;

                            }

                            break;
                        case PackageIds.importRoutineCommand:
                        case PackageIds.newRoutineCommand:
                            textKey = GetMenuCommandText(menuCommand.CommandID.ID);
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier(textKey);
                            program = selectedProjectItem.AssociatedObject as IProgram;
                            if (program != null && program.Type == ProgramType.Sequence)
                            {
                                menuCommand.Visible = false;
                            }
                            else
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = !controller.IsOnline;
                            }
                            break;

                        case PackageIds.newLocalTagCommand:
                        case PackageIds.newParameterCommand:
                            textKey = GetMenuCommandText(menuCommand.CommandID.ID);
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier(textKey);
                            if (selectedProjectItem.Kind == ProjectItemType.ProgramTags)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = true;
                            }
                            break;

                        case PackageIds.exportDataTypeCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Export Data Type");
                            if (selectedProjectItem.AssociatedObject != null &&
                                (selectedProjectItem.Kind == ProjectItemType.UserDefined ||
                                 selectedProjectItem.Kind == ProjectItemType.String))
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = true;
                            }

                            break;
                        case PackageIds.exportAddOnInstructionCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Export Add-On Instruction...");
                            if (selectedProjectItem.AssociatedObject != null &&
                                selectedProjectItem.Kind == ProjectItemType.AddOnInstruction)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = true;
                            }

                            break;
                        case PackageIds.exportProgramCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Export Program...");
                            if (selectedProjectItem.AssociatedObject != null &&
                                selectedProjectItem.Kind == ProjectItemType.Program)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = true;
                            }

                            break;
                        case PackageIds.exportRoutineCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Export Routine...");
                            if (selectedProjectItem.AssociatedObject != null &&
                                selectedProjectItem.Kind == ProjectItemType.Routine)
                            {
                                if (((IRoutine) selectedProjectItem.AssociatedObject).ParentCollection
                                    .ParentProgram is AoiDefinition)
                                {
                                    menuCommand.Visible = false;
                                }
                                else
                                {
                                    menuCommand.Visible = true;
                                    menuCommand.Enabled = true;
                                }
                            }

                            break;
                        case PackageIds.exportModuleCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Export Module...");
                            if (selectedProjectItem.Kind == ProjectItemType.DeviceModule)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = true;

                                if (((IDeviceModule) selectedProjectItem.AssociatedObject).IsEmbeddedIO())
                                {
                                    menuCommand.Enabled = false;
                                }
                            }

                            if (selectedProjectItem.Kind == ProjectItemType.LocalModule)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = false;
                            }

                            break;
                        case PackageIds.importDataTypeCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Import Data Type...");
                            if (selectedProjectItem.AssociatedObject == null &&
                                selectedProjectItem.Kind == ProjectItemType.UserDefineds)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = !controller.IsOnline;
                            }

                            break;
                        case PackageIds.importAddOnInstructionCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Import Add-On Instruction...");
                            if (selectedProjectItem.AssociatedObject == null &&
                                selectedProjectItem.Kind == ProjectItemType.AddOnInstructions)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = true;

                                //TODO(gjc): edit later
                                menuCommand.Enabled = !controller.IsOnline;
                            }
                            else if (selectedProjectItem.AssociatedObject == null &&
                                     selectedProjectItem.Kind == ProjectItemType.AddOnDefineds)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = false;
                            }

                            break;
                        case PackageIds.importStringTypeCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Import String Type...");

                            if (selectedProjectItem.AssociatedObject == null &&
                                selectedProjectItem.Kind == ProjectItemType.Strings)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = !controller.IsOnline;
                            }

                            break;
                        case PackageIds.exportStringTypeCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Export String Type...");
                            if (selectedProjectItem.AssociatedObject != null &&
                                selectedProjectItem.Kind == ProjectItemType.Strings)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = true;
                            }

                            break;
                        case PackageIds.openTrendLogCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Open Trace Variable...");
                            if (selectedProjectItem.Kind == ProjectItemType.Trends)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = true;
                            }

                            break;
                        case PackageIds.importModuleCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Import Module...");
                            if (selectedProjectItem.Kind == ProjectItemType.Ethernet ||
                                selectedProjectItem.Kind == ProjectItemType.Bus)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = !controller.IsOnline;
                            }

                            break;
                        case PackageIds.importTrendCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Import Trace...");
                            if (selectedProjectItem.Kind == ProjectItemType.Trends)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = true;
                            }

                            break;
                        case PackageIds.exportTrendCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Export Trace...");
                            if (selectedProjectItem.Kind == ProjectItemType.Trend)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = true;
                            }

                            break;
                        case PackageIds.monitorAxisTagCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Monitor Axis Variable");
                            if (selectedProjectItem.Kind == ProjectItemType.AxisCIPDrive)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = true;
                            }

                            break;
                        case PackageIds.monitorGroupTagCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Monitor Group Variable");
                            if (selectedProjectItem.Kind == ProjectItemType.MotionGroup)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = true;
                            }

                            break;
                        case PackageIds.crossReferenceCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Cross Reference...");
                            if (selectedProjectItem.Kind == ProjectItemType.MotionGroup ||
                                selectedProjectItem.Kind == ProjectItemType.AxisCIPDrive ||
                                selectedProjectItem.Kind == ProjectItemType.AxisVirtual ||
                                selectedProjectItem.Kind == ProjectItemType.AddOnInstruction ||
                                selectedProjectItem.Kind == ProjectItemType.AddOnDefined)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = true;
                            }

                            break;
                        case PackageIds.gotoModuleCommand:
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Goto Module");
                            if (selectedProjectItem.Kind == ProjectItemType.AxisCIPDrive)
                            {
                                menuCommand.Visible = true;
                                menuCommand.Enabled = false;
                                foreach (var module in controller.DeviceModules)
                                {
                                    var cipMotion = module as CIPMotionDrive;
                                    if (cipMotion != null)
                                    {
                                        var result = cipMotion.ContainAxis((ITag) selectedProjectItem.AssociatedObject);
                                        if (result)
                                        {
                                            menuCommand.Enabled = true;
                                            break;
                                        }
                                    }
                                }
                            }

                            break;
                    }
                }

            }
        }

        private string GetMenuCommandText(int menuCommandId)
        {
            switch (menuCommandId)
            {
                case PackageIds.importRoutineCommand:
                    return "Import Routine...";
                 
                case PackageIds.newRoutineCommand:
                    return "New Routine...";
                
                case PackageIds.newLocalTagCommand:
                    return "New Local Variable...";
                   
                case PackageIds.newParameterCommand:
                    return "New Parameter...";

                case PackageIds.newProgramCommand:
                    return "New Program...";
                 
                case PackageIds.importProgramCommand:
                    return "Import Program...";

                case PackageIds.AXIS_CIP_DRIVECommand:
                    return "AxisCipDrive";

                case PackageIds.AXIS_VIRTUALCommand:
                    return "AxisVirtual";

                case PackageIds.taskNewProgramCommand:

                    return "New Program...";
              
                case PackageIds.newEquipmentPhaseCommand:
                    return "New Equipment Phase...";
                   
                case PackageIds.newEquipmentSequenceCommand:
                    return "New Equipment Sequence...";

                case PackageIds.taskImportProgramCommand:
                    return "Import Program...";
                    
                case PackageIds.importEquipmentPhaseCommand:
                   return "Import Equipment Phase...";

                case PackageIds.importEquipmentSequenceCommand:
                    return "Import Equipment Sequence...";
                 
                case PackageIds.programNewLocalTagCommand:
                    return "New Local Variable...";

                case PackageIds.programNewParameterCommand:
                   return "New Parameter...";

                default:
                    return "";
            }
        }


        public bool EnableCommand(List<IProjectItem> selectedProjectItems)
        {
            if (selectedProjectItems == null || selectedProjectItems.Count == 0)
                return false;

            if (selectedProjectItems[0].Kind != ProjectItemType.Program &&
                selectedProjectItems[0].Kind != ProjectItemType.AddOnInstruction &&
                selectedProjectItems[0].Kind != ProjectItemType.UserDefined &&
                selectedProjectItems[0].Kind != ProjectItemType.String &&
                selectedProjectItems[0].Kind != ProjectItemType.Trend)
                return false;

            if (selectedProjectItems.Count > 1)
                return false;

            return true;
        }

        private bool CheckNewModuleCommandEnabled(IController controller, IBaseObject associatedObject)
        {
            DeviceModule deviceModule = associatedObject as DeviceModule;

            var pointIOPort = deviceModule?.GetFirstPort(PortType.PointIO);
            if (pointIOPort != null)
            {
                int totalSize = pointIOPort.Bus.Size;

                int childNum = 0;
                foreach (var module in controller.DeviceModules)
                {
                    DeviceModule module0 = module as DeviceModule;
                    if (module0 != null && module0 != deviceModule && module0.ParentModule == deviceModule)
                        childNum++;
                }

                if (totalSize > childNum + 1)
                    return true;

                return false;
            }

            return false;
        }

        private void PrintMenuOnBeforeQueryStatus(object sender, EventArgs e)
        {
            var menuCommand = sender as OleMenuCommand;
            if (menuCommand == null) return;

            var selectedProjectItems = ServiceProvider.GetSelectedProjectItems();
            IProjectItem selectedProjectItem = null;
            if (selectedProjectItems.Count > 0)
                selectedProjectItem = selectedProjectItems[0];

            if (selectedProjectItem == null)
            {
                menuCommand.Enabled = false;
                //return;
            }

            //Debug.Assert(selectedProjectItem != null, nameof(selectedProjectItem) + " != null");
            switch (menuCommand.CommandID.ID)
            {
                case PackageIds.printRoutine2:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Routine...");
                    if (selectedProjectItem != null && selectedProjectItem.Kind == ProjectItemType.Routine)
                    {
                        menuCommand.Visible = true;
                    }
                    else
                    {
                        menuCommand.Visible = false;
                    }
                    break;
                case PackageIds.printRoutineProperties2:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Routine Properties...");
                    if (selectedProjectItem != null && selectedProjectItem.Kind == ProjectItemType.Routine)
                    {
                        menuCommand.Visible = true;
                    }
                    else
                    {
                        menuCommand.Visible = false;
                    }
                    break;
                case PackageIds.printTags2:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Tags...");
                    if (selectedProjectItem != null && (selectedProjectItem.Kind == ProjectItemType.ControllerModel
                                                        || selectedProjectItem.Kind == ProjectItemType.ControllerTags
                                                        || selectedProjectItem.Kind == ProjectItemType.ProgramTags
                                                        || selectedProjectItem.Kind == ProjectItemType.Routine
                                                        || selectedProjectItem.Kind == ProjectItemType.MotionGroup))
                    {
                        menuCommand.Visible = true;
                    }
                    else
                    {
                        menuCommand.Visible = false;
                    }
                    break;
                case PackageIds.printSequencingAndStepTags2:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Sequencing and Step Tags...");
                    menuCommand.Visible = false;
                    break;
                case PackageIds.printModuleProperties2:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Module Properties...");
                    if (selectedProjectItem != null && (selectedProjectItem.Kind == ProjectItemType.Bus
                                                        || selectedProjectItem.Kind == ProjectItemType.DeviceModule
                                                        || selectedProjectItem.Kind == ProjectItemType.LocalModule
                                                        || selectedProjectItem.Kind == ProjectItemType.Ethernet))
                    {
                        menuCommand.Visible = true;
                    }
                    else
                    {
                        menuCommand.Visible = false;
                    }
                    break;
                case PackageIds.printAdd_OnInstructionSignatures2:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("User Define Funciton Signatures...");
                    menuCommand.Visible = false;
                    break;
                case PackageIds.printAdd_OnInstruction2:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("User Define Funciton...");
                    if (selectedProjectItem != null && selectedProjectItem.Kind == ProjectItemType.AddOnInstruction)
                    {
                        menuCommand.Visible = true;
                    }
                    else
                    {
                        menuCommand.Visible = false;
                    }
                    break;
                case PackageIds.printDataType2:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Data Type...");
                    if (selectedProjectItem != null && (selectedProjectItem.Kind == ProjectItemType.Predefined
                                                        || selectedProjectItem.Kind == ProjectItemType.UserDefined
                                                        || selectedProjectItem.Kind == ProjectItemType.String
                                                        || selectedProjectItem.Kind == ProjectItemType.AddOnDefined
                                                        || selectedProjectItem.Kind == ProjectItemType.ModuleDefined))
                    {
                        menuCommand.Visible = true;
                    }
                    else
                    {
                        menuCommand.Visible = false;
                    }
                    break;
                case PackageIds.printCrossReference2:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Cross Reference...");
                    menuCommand.Visible = false;
                    break;
                case PackageIds.printControllerOrganizer2:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Controller Organizer...");
                    if (selectedProjectItem != null && selectedProjectItem.Kind == ProjectItemType.ControllerModel)
                    {
                        menuCommand.Visible = true;
                    }
                    else
                    {
                        menuCommand.Visible = false;
                    }
                    break;
                case PackageIds.printControllerProperties2:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Controller Properties...");
                    if (selectedProjectItem != null && selectedProjectItem.Kind == ProjectItemType.ControllerModel)
                    {
                        menuCommand.Visible = true;
                    }
                    else
                    {
                        menuCommand.Visible = false;
                    }
                    break;
                case PackageIds.printTaskProperties2:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Task Properties...");
                    if (selectedProjectItem != null && selectedProjectItem.Kind == ProjectItemType.Task)
                    {
                        menuCommand.Visible = true;
                    }
                    else
                    {
                        menuCommand.Visible = false;
                    }
                    break;
                case PackageIds.printProgramProperties2:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Program Properties...");
                    if (selectedProjectItem != null && (selectedProjectItem.Kind == ProjectItemType.Program))
                    {
                        menuCommand.Visible = true;
                    }
                    else
                    {
                        menuCommand.Visible = false;
                    }
                    break;
                case PackageIds.printEquipmentSequence2:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Equipment Sequences...");
                    menuCommand.Visible = false;
                    break;
                case PackageIds.printEntireProject2:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Entire Project...");
                    if (selectedProjectItem != null && (selectedProjectItem.Kind == ProjectItemType.ControllerModel
                                                        || selectedProjectItem.Kind == ProjectItemType.Routine))
                    {
                        menuCommand.Visible = true;
                    }
                    else
                    {
                        menuCommand.Visible = false;
                    }
                    break;
            }
        }
    }
}
