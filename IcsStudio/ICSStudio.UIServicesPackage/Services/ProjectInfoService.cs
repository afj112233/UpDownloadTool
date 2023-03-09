using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml;
using ICSStudio.Cip.Objects;
using ICSStudio.Dialogs.GlobalSetting;
using ICSStudio.EditorPackage.STEditor;
using ICSStudio.FileConverter.L5XToJson;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Recover;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIInterfaces.Editor;
using ICSStudio.UIInterfaces.Error;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.UIInterfaces.UI;
using ICSStudio.UIServicesPackage.View;
using ICSStudio.UIServicesPackage.ViewModel;
using ICSStudio.Utils;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MessageBox = System.Windows.MessageBox;
using ICSStudio.UIServicesPackage.ImportConfiguration;
using Application = System.Windows.Application;
using Type = System.Type;
using ICSStudio.UIServicesPackage.ImportConfiguration.Dialog;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Exceptions;
using ICSStudio.SimpleServices.Utilities;
using NLog;

namespace ICSStudio.UIServicesPackage.Services
{
    [SuppressMessage("ReSharper", "UsePatternMatching")]
    public class ProjectInfoService : IProjectInfoService, SProjectInfoService
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly Package _package;

        public ProjectInfoService(Package package)
        {
            _package = package;
        }

        private IServiceProvider ServiceProvider => _package;

        public IProject CurrentProject => Project.Instance;

        public IController Controller => CurrentProject?.Controller;

        public void OpenJsonFile(string fileName, bool needAddDataType)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                if (File.Exists($"{fileName}.Recovery"))
                {
                    var uiShell =
                        Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;

                    var dialog = new RecoverDialog()
                    {
                        DataContext = new RecoverViewModel(fileName)
                    };

                    if (dialog.ShowDialog(uiShell) ?? false)
                    {

                    }
                    else
                    {
                        return;
                    }
                }

                CurrentProject.Controller = null;

                var controller = SimpleServices.Common.Controller.Open(fileName, needAddDataType);

                CurrentProject.Controller = controller;
                GlobalSetting.GetInstance().MonitorTagSetting.CheckFilterType();
            }
            catch (Exception e)
            {
                Logger.Error($"Open {fileName} failed!\n{e.Message}\n{e.StackTrace}");

                MessageBox.Show($"Open {fileName} failed!{(e is NotSupportedException ? $"\n{e.Message}" : "")}",
                    "Error", MessageBoxButton.OKCancel,
                    MessageBoxImage.Error);

                //TODO(gjc): remove later
                SimpleServices.Common.Controller.GetInstance().ProjectLocaleName = string.Empty;

            }
        }

        public void ImportFile(string importFile, string saveFile)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                Converter.L5XToJson(importFile, saveFile);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                MessageBox.Show($"Import {importFile} failed!{(ex is NotSupportedException ? $"\n{ex.Message}" : "")}",
                    "Error", MessageBoxButton.OKCancel,
                    MessageBoxImage.Error);
                return;
            }

            OpenJsonFile(saveFile, false);
        }

        public int Save(bool needNativeCode)
        {
            int result = 0;

            var dialogService = Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;
            if (dialogService != null)
            {
                result = dialogService.ApplyAllDialogs();
            }

            if (result < 0)
            {
                return result;
            }

            //////
            var createEditorService =
                Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
            if (createEditorService != null)
            {
                result = createEditorService.ApplyAllToolWindows();
            }

            if (result < 0)
                return result;

            //////
            result = CurrentProject.Save(needNativeCode);

            return result;
        }

        public int SaveAs(string fileName)
        {
            Controller controller = Controller as Controller;
            if (controller == null)
                return 0;

            int result = 0;
            var dialogService = Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;
            if (dialogService != null)
            {
                result = dialogService.ApplyAllDialogs();
            }

            if (result == 0)
            {
                var createEditorService =
                    Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
                if (createEditorService != null)
                {
                    result = createEditorService.ApplyAllToolWindows();
                }
            }

            if (result == 0)
            {
                result = CurrentProject.SaveAs(fileName);
            }

            return result;
        }

        public int ExportFile(string exportFile)
        {
            Controller controller = Controller as Controller;
            if (controller == null)
                return 0;

            return controller.Export(exportFile);
        }


        public bool ImportData(ProjectItemType kind, IBaseObject associatedObject, int? startIndex = null, int? endIndex = null)
        {
            var service = Package.GetGlobalService(typeof(SStudioUIService)) as IStudioUIService;
            service?.DetachController();

            bool hasPreLoad = false;
            JObject config = null;
            try
            {
#pragma warning disable VSTHRD010 // 在主线程上调用单线程类型
                var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
#pragma warning restore VSTHRD010 // 在主线程上调用单线程类型

                var controller = Controller as Controller;
                if (controller == null)
                    return false;

                OpenFileDialog openFileDialog = new OpenFileDialog()
                    { Title = @"Open File", Filter = @"json文件(*.json)|*.json|L5X文件(*.L5X)|*.L5X" };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (var file = File.OpenText(openFileDialog.FileName))
                    {
                        //fix import type
                        if (kind == ProjectItemType.UserDefineds)
                        {
                            kind = ProjectItemType.UserDefined;
                        }

                        if (openFileDialog.FilterIndex == 1)
                        {
                            using (var reader = new JsonTextReader(file))
                            {
                                config = (JObject)JToken.ReadFrom(reader);
                                if (kind == ProjectItemType.LocalModule || kind == ProjectItemType.Ethernet)
                                {
                                    var module = GetTarget(config, kind);
                                    var mess = CheckModule(associatedObject as DeviceModule, module, kind);
                                    if (!string.IsNullOrEmpty(mess))
                                    {
                                        if (MessageBox.Show(mess, "ICS Studio", MessageBoxButton.OKCancel,
                                                MessageBoxImage.Asterisk) == MessageBoxResult.OK)
                                            ImportData(kind, associatedObject,startIndex,endIndex);
                                        return false;
                                    }
                                }
                            }
                        }
                        else if (openFileDialog.FilterIndex == 2)
                        {
                            var doc = new XmlDocument();
                            doc.Load(file);
                            config = kind == ProjectItemType.Bus || kind == ProjectItemType.Ethernet
                                ? ConvertModule(doc)
                                : Converter.ToJObject(doc, true);
                            if (kind == ProjectItemType.LocalModule || kind == ProjectItemType.Ethernet)
                            {
                                var module = GetTarget(config, kind);
                                var mess = CheckModule(associatedObject as DeviceModule, module, kind);
                                if (!string.IsNullOrEmpty(mess))
                                {
                                    if (MessageBox.Show(mess, "ICS Studio", MessageBoxButton.OKCancel,
                                            MessageBoxImage.Asterisk) == MessageBoxResult.OK)
                                        ImportData(kind, associatedObject,startIndex,endIndex);
                                    return false;
                                }
                            }

                            CancelPreLoad(config); // clear redundant elements which added by "Converter.ToJObject" (xml -> JObject)

                        //#if DEBUG
                        //    var saveFileDialog = new SaveFileDialog
                        //    {
                        //        Title = "Save file",
                        //        Filter = "json文件(*.json)|*.json",
                        //        FileName = "L5XToJsonTestFile"
                        //    };
                        //    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        //        File.WriteAllText(saveFileDialog.FileName, Convert.ToString(config));
                        //#endif

                        }

                        if (kind == ProjectItemType.Trends)
                        {
                            var trend = GetTarget(config, kind);
                            if (trend == null)
                            {
                                MessageBox.Show(
                                    $"Filed to import file '{openFileDialog.FileName}'.\nThe specified json file does not contain a Trend export.",
                                    "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                                return false;
                            }

                            var names = new List<string>();

                            foreach (var existTrend in controller.Trends) names.Add(existTrend.Name);

                            trend["Name"] = Utils.Utils.GetNotDuplicateName((string)trend["Name"], names);
                            controller.AddTrend(trend);
                            return true;
                        }

                        PreLoad(config);
                        hasPreLoad = true;
                        var dialog = new ImportDialog();
                        dialog.ShowInTaskbar = false;
                        dialog.Owner = Application.Current.MainWindow;
                        var vm = new ImportDialogViewModel(kind, config, associatedObject,startIndex,endIndex);
                        vm.Title = openFileDialog.SafeFileName;
                        dialog.DataContext = vm;
                        if (!(dialog.ShowDialog(uiShell) ?? false))
                        {
                            CancelPreLoad(config);
                            return false;
                        }

                        CancelPreLoad(config);
                        hasPreLoad = false;
                        vm.FinalizeImport();

                        var pendingVerifyRoutines = new List<IRoutine>();
                        var importingDialog = new ImportingDialog();
                        importingDialog.ShowInTaskbar = false;
                        importingDialog.Owner = Application.Current.MainWindow;
                        var importingVm = new ImportingViewModel();
                        importingVm.WorkAction = () =>
                        {
                            var result = ImportAsync(config, kind, associatedObject, pendingVerifyRoutines, importingVm)
                                .ConfigureAwait(false).GetAwaiter().GetResult();
                            if (!string.IsNullOrEmpty(result))
                            {
                                if (MessageBox.Show(result, "ICS Studio", MessageBoxButton.OKCancel,
                                        MessageBoxImage.Asterisk) == MessageBoxResult.OK)
                                    ImportData(kind, associatedObject,startIndex,endIndex);
                            }
                            else
                            {
                                var createEditorService =
                                    Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
                                ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
                                {
                                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                                    importingVm.ImportingTitle = "Verifying . . .";
                                    importingVm.Progress = 0;
                                    importingVm.Maximum = pendingVerifyRoutines.Count();
                                });

                                foreach (var pendingVerifyRoutine in pendingVerifyRoutines)
                                {
                                    pendingVerifyRoutine.IsError = true;
                                    createEditorService?.ParseRoutine(pendingVerifyRoutine, true, true);
                                    importingVm.Worker.ReportProgress(1);
                                }
                            }
                        };
                        importingDialog.DataContext = importingVm;
                        importingDialog.ShowDialog();
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                string message = string.Empty;
                if (e is ICSStudioException)
                {
                    message = e.Message;
                }
                else
                {
                    message = LanguageManager.GetInstance().ConvertSpecifier("Failed to Import file.The import was aborted due to errors.");
                }
                var result = MessageBox.Show(message, "ICS Studio", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK) ImportData(kind, associatedObject,startIndex,endIndex);
                return false;
            }
            finally
            {
                if (hasPreLoad)
                    CancelPreLoad(config);

                (Controller as Controller)?.Changed.Clear();

                service?.AttachController();
                service?.Reset();

                Verify(Controller);
            }

            return true;
        }

        public string NewFile()
        {
            var dialog = new NewProjectDialog();
            var vm = new NewProjectDialogViewModel();
            dialog.DataContext = vm;
            dialog.Owner = Application.Current.MainWindow;

            dialog.ShowDialog();

            return vm.ProjectFile;
        }

        public void VerifyReference(List<ITag> delTags)
        {
            if (!delTags.Any()) return;
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += (sender, e) =>
            {
                var stEditorViewModel = CurrentObject.GetInstance().Current as STEditorViewModel;
                var currentRoutine = stEditorViewModel?.Routine;
                Verify(currentRoutine);
                var delTag = delTags[0];
                if (delTag?.ParentCollection.ParentProgram is AoiDefinition)
                {
                    var aoi = (AoiDefinition)delTag.ParentCollection.ParentProgram;
                    foreach (var r in aoi.Routines.Where(r =>
                                 (r as STRoutine)?.GetAllReferenceTags().Any(delTags.Contains) ?? false))
                    {
                        if (r == currentRoutine || r.IsError) continue;
                        r.IsError = true;
                        Verify(r);
                    }
                }
                else
                {
                    if (Controller != null)
                    {
                        foreach (var program in Controller.Programs)
                        {
                            var referenceRoutines = program.Routines.Where(r =>
                                (r as STRoutine)?.GetAllReferenceTags().Any(delTags.Contains) ?? false);
                            foreach (var routine in referenceRoutines)
                            {
                                if (routine == currentRoutine || routine.IsError) continue;
                                routine.IsError = true;
                                Verify(routine);
                            }
                        }
                    }

                }
            };
            backgroundWorker.RunWorkerAsync();
        }

        public void VerifyParameterConnection()
        {
            if(Controller == null)
                return;

            var outputService = Package.GetGlobalService(typeof(SErrorOutputService)) as IErrorOutputService;
            foreach (IParameterConnection connection in Controller.ParameterConnections)
            {
                var arg = Controller.ParameterConnections.VerifyConnection(connection);
                if (arg != null)
                {
                    if (arg.Message.Contains(" exists."))
                    {
                        if (arg.Connection == connection) continue;
                        var sourceTag = ObtainValue.NameToTag(connection.SourcePath, null);
                        var destTag = ObtainValue.NameToTag(connection.DestinationPath, null);
                        if (sourceTag == null && destTag == null)
                        {
                            var message =
                                $"Tag or parameter usage types are incompatible.({connection.SourcePath} <-> {connection.DestinationPath})";
                            outputService?.Cleanup();
                            outputService?.AddErrors(message, OrderType.None, OnlineEditType.Original, null, null,
                                null);
                            throw new InvalidParameterConnection(message);
                        }
                        outputService?.AddErrors($"Error: Parameter '{connection.SourcePath}':{arg.Message}",
                            OrderType.None, OnlineEditType.Original, null, null, sourceTag?.Item1);
                        outputService?.AddErrors($"Error: Parameter '{connection.DestinationPath}':{arg.Message}",
                            OrderType.None, OnlineEditType.Original, null, null, destTag?.Item1);
                    }
                    else
                    {
                        if (arg.Message.Contains("Tag doesn't reference"))
                        {
                            var tag = ObtainValue.NameToTag(connection.SourcePath, null);
                            if (tag?.Item1 == null)
                                tag = ObtainValue.NameToTag(connection.DestinationPath, null);
                            if (tag == null)
                            {
                                var message =
                                    $"Tag or parameter usage types are incompatible.({connection.SourcePath} <-> {connection.DestinationPath})";
                                outputService?.Cleanup();
                                outputService?.AddErrors(message, OrderType.None, OnlineEditType.Original, null, null,
                                    null);
                                throw new InvalidParameterConnection(message);
                            }
                            outputService?.AddErrors(arg.Message, OrderType.None, OnlineEditType.Original, null, null,
                                tag?.Item1);
                        }
                        else
                        {
                            var sourceTag = ObtainValue.NameToTag(connection.SourcePath, null);
                            var destTag = ObtainValue.NameToTag(connection.DestinationPath, null);
                            if (sourceTag == null && destTag == null)
                            {
                                var message =
                                    $"Tag or parameter usage types are incompatible.({connection.SourcePath} <-> {connection.DestinationPath})";
                                outputService?.Cleanup();
                                outputService?.AddErrors(message, OrderType.None, OnlineEditType.Original, null, null,
                                    null);
                                throw new InvalidParameterConnection(message);
                            }
                            outputService?.AddErrors($"Error: Parameter '{connection.SourcePath}':{arg.Message}",
                                OrderType.None, OnlineEditType.Original, null, null, sourceTag?.Item1);
                            outputService?.AddErrors($"Error: Parameter '{connection.DestinationPath}':{arg.Message}",
                                OrderType.None, OnlineEditType.Original, null, null, destTag?.Item1);
                        }
                    }
                }
            }

            foreach (var program in Controller.Programs)
            {
                foreach (var tag in program.Tags.Where(t => t.Usage == Usage.InOut))
                {
                    var arg = Controller.ParameterConnections.VerifyInOutTag(tag);
                    if (arg != null)
                    {
                        outputService?.AddErrors(arg.Message, OrderType.None, OnlineEditType.Original, null, null, tag);
                    }
                }
            }
        }

        public void SetProjectDirty()
        {
            CurrentProject.Saved = false;
            
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var studioUIService =
                    ServiceProvider.GetService(typeof(SStudioUIService)) as IStudioUIService;

                // update title
                studioUIService?.UpdateWindowTitle();

            });
        }

        #region Import

        private async Task<string> ImportAsync(JObject importConfig, ProjectItemType kind, IBaseObject associatedObject,
            List<IRoutine> pendingVerifyRoutines, ImportingViewModel importingVm)
        {
            var result = CheckTarget(importConfig, kind, associatedObject);
            if ("Use Existing".Equals(result)) return "";
            if (!string.IsNullOrEmpty(result))
            {
                (Controller as Controller)?.Changed.Clear();
                return result;
            }

            return await ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                return result = LoadAttachConfig(associatedObject, importConfig, kind, pendingVerifyRoutines,
                    importingVm);
            });
        }

        private JObject ConvertModule(XmlDocument doc)
        {
            JObject controller = new JObject();
            // Modules
            JArray modules = new JArray();
            var xmlNodeList = doc?.GetElementsByTagName("Module");
            var nameList = new List<string>();
            if (xmlNodeList != null)
            {
                foreach (XmlElement node in xmlNodeList)
                {
                    modules.Add(Converter.ToDeviceModule(node, nameList));
                }
            }

            controller.Add("Modules", modules);

            // Programs
            JArray programs = new JArray();
            controller.Add("Programs", programs);

            // Tags
            JArray tags = new JArray();
            controller.Add("Tags", tags);

            // Tasks
            JArray tasks = new JArray();
            controller.Add("Tasks", tasks);

            // DataTypes
            JArray dataTypes = new JArray();
            controller.Add("DataTypes", dataTypes);

            //aoi
            JArray aoi = new JArray();
            controller.Add("AddOnInstructionDefinitions", aoi);

            return controller;
        }

        public static JObject GetTarget(JObject importConfig, ProjectItemType kind)
        {
            if (kind == ProjectItemType.UserDefined || kind == ProjectItemType.Strings)
            {
                var dataTypes = importConfig["DataTypes"];
                foreach (JObject dataType in dataTypes)
                {
                    if ("Target".Equals(dataType["Use"]?.ToString())) return dataType;
                }

                return null;
            }

            if (kind == ProjectItemType.AddOnInstructions)
            {
                var addOnInstructionDefinitions = importConfig["AddOnInstructionDefinitions"];
                foreach (JObject aoi in addOnInstructionDefinitions)
                {
                    if ("Target".Equals(aoi["Use"]?.ToString())) return aoi;
                }

                return null;
            }

            if (kind == ProjectItemType.Task ||
                kind == ProjectItemType.UnscheduledPrograms ||
                kind == ProjectItemType.FaultHandler ||
                kind == ProjectItemType.PowerHandler)
            {
                var programs = importConfig["Programs"];
                foreach (JObject program in programs)
                {
                    if ("Target".Equals(program["Use"]?.ToString())) return program;
                }

                return null;
            }

            if (kind == ProjectItemType.Program)
            {
                var programs = importConfig["Programs"];
                foreach (JObject program in programs)
                {
                    foreach (JObject routine in program["Routines"])
                    {
                        if ("Target".Equals(routine["Use"]?.ToString())) return routine;
                    }
                }

                return null;
            }

            if (kind == ProjectItemType.Routine)
            {
                var rungs = importConfig["Programs"]?[0]?["Routines"]?[0]["Rungs"];
                foreach (JObject rung in rungs)
                {
                    if ("Target".Equals(rung["Use"]?.ToString())) return rung;
                }

                return null;
            }

            if (kind == ProjectItemType.Bus || kind == ProjectItemType.Ethernet)
            {
                var modules = importConfig["Modules"];
                foreach (JObject module in modules)
                {
                    if ("Target".Equals(module["Use"]?.ToString())) return module;
                }
            }

            if (kind == ProjectItemType.Trends)
            {
                var trends = importConfig["Trends"];
                foreach (JObject trend in trends)
                {
                    if ("Target".Equals(trend["Use"]?.ToString())) return trend;
                }
            }

            return null;
        }

        private string CheckModule(IDeviceModule parentModule, JObject module, ProjectItemType kind)
        {
            var child = module["CatalogNumber"]?.ToString().Split('-');
            if (parentModule is LocalModule)
            {
                if (child[1].IndexOf("ib", StringComparison.OrdinalIgnoreCase) > -1 ||
                    child[1].IndexOf("ob", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    return "Module import failed.";
                }

                //foreach (DeviceModule deviceModule in Controller.DeviceModules)
                //{
                //    if (deviceModule.ParentModule is LocalModule)
                //    {
                //        if (deviceModule.Name.Equals(module["Name"]?.ToString(), StringComparison.OrdinalIgnoreCase))
                //            return "Discard";
                //    }
                //}

                //var local = associatedObject as LocalModule;
                return "";
            }

            //Debug.Assert(parentModule != null,associatedObject.GetType().Name);
            //适配器下是否含有同名module或者相同slot

            var slot = (int)module["Ports"]?[0]["Address"];

            if (Controller != null)
            {
                foreach (DeviceModule deviceModule in Controller.DeviceModules)
                {
                    if (deviceModule.ParentModule == parentModule)
                    {
                        //if (deviceModule.Name.Equals(module["Name"]?.ToString(), StringComparison.OrdinalIgnoreCase))
                        //    return "Discard";
                        if (deviceModule.Ports[0].Address == slot.ToString())
                            return "No more available slots in the chassis.";
                    }
                }
            }
            

            //是否在一个系列
            var parent = parentModule?.CatalogNumber.Split('-');
            if (parent != null)
            {
                Debug.Assert(child != null && child?.Length > 1, module["CatalogNumber"]?.ToString() ?? "");
                if (!parent[0].Equals(child?[0])) return "Child module incompatible with parent module.";
                //point io下适配器只能存在一个;ib、ob可以多个但要看chassis数量

                bool isValid = false;
                string[] validIOTypes = { "ib", "ob", "iq", "ov", "if", "ir", "of" };

                foreach (var validIOType in validIOTypes)
                {
                    if (child[1].StartsWith(validIOType, StringComparison.OrdinalIgnoreCase))
                    {
                        isValid = true;
                        break;
                    }
                }

                if (!isValid)
                {
                    return "Child module incompatible with parent module.";
                }

                var communicationsAdapter = parentModule as CommunicationsAdapter;
                Debug.Assert(communicationsAdapter != null, parentModule.GetType().Name);
                if (slot >= communicationsAdapter.ChassisSize ||
                    communicationsAdapter.GetMaxChildSlot() >= communicationsAdapter.ChassisSize)
                    return "No more available slots in the chassis.";
            }

            return "";
        }

        private string CheckTarget(JObject importConfig, ProjectItemType kind, IBaseObject associatedObject)
        {
            var target = GetTarget(importConfig, kind);
            if (target == null) return "Failed to import file.The file does not contain a export.";
            if ("Use Existing".Equals((string)target["Operation"])) return "Use Existing";
            //if (IsExist(kind, associatedObject, target["Name"]?.ToString()))
            //{
            //    if (kind == ProjectItemType.UserDefined || kind == ProjectItemType.Strings)
            //    {
            //        return "Data Type is exist";
            //    }
            //    else if (kind == ProjectItemType.AddOnInstructions)
            //    {
            //        return $"AddOnInstructionDefinition is exist";
            //    }
            //    else if (kind == ProjectItemType.Task ||
            //             kind == ProjectItemType.UnscheduledTasks ||
            //             kind == ProjectItemType.FaultHandler ||
            //             kind == ProjectItemType.PowerHandler)
            //    {
            //        return $"Program is exist";
            //    }
            //    else if (kind == ProjectItemType.Program)
            //    {
            //        return $"Routine is exist";
            //    }
            //}

            return "";
        }

        public static bool IsExist(ProjectItemType kind, IBaseObject associatedObject, string name)
        {
            if (kind == ProjectItemType.UserDefined || kind == ProjectItemType.Strings)
            {
                return SimpleServices.Common.Controller.GetInstance().DataTypes.FirstOrDefault(d =>
                    !d.IsPredefinedType && d.Name.Equals(name) &&
                    !((d as AssetDefinedDataType)?.IsTmp ?? true)) != null;
            }
            else
            {
                return GetExistObject(kind, associatedObject, name) != null;
            }
        }

        public static bool IsSame(ProjectItemType kind, IBaseObject associatedObject, string name)
        {
            if (kind == ProjectItemType.UserDefined || kind == ProjectItemType.Strings)
            {
                var exist = ((DataTypeCollection)SimpleServices.Common.Controller.GetInstance().DataTypes)
                    .FindUserDataType(name, false);
                var target = ((DataTypeCollection)SimpleServices.Common.Controller.GetInstance().DataTypes)
                    .FindUserDataType(name, true);
                return DataTypeIsSame(exist, target);

            }

            return false;
        }

        private static bool DataTypeIsSame(IDataType a, IDataType b)
        {
            if (a.IsPredefinedType && b.IsPredefinedType)
            {
                return a == b;
            }

            var a1 = a as AssetDefinedDataType;
            var b1 = b as AssetDefinedDataType;
            if (a1 == null || b1 == null)
            {
                return false;
            }
            else
            {
                if (a1.TypeMembers.Count != b1.TypeMembers.Count) return false;
                for (int i = 0; i < a1.TypeMembers.Count; i++)
                {
                    var aMember = ((TypeMemberComponentCollection)a1.TypeMembers).FindDataType(i);
                    var bMember = ((TypeMemberComponentCollection)b1.TypeMembers).FindDataType(i);
                    if (!(aMember.DataTypeInfo.Dim1 == bMember.DataTypeInfo.Dim1 &&
                          aMember.DataTypeInfo.Dim2 == bMember.DataTypeInfo.Dim2 &&
                          aMember.DataTypeInfo.Dim3 == bMember.DataTypeInfo.Dim3)) return false;
                    if (!DataTypeIsSame(aMember.DataTypeInfo.DataType, bMember.DataTypeInfo.DataType)) return false;
                }
            }

            return true;
        }

        public static IBaseComponent GetExistObject(ProjectItemType kind, IBaseObject associatedObject, string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (kind == ProjectItemType.UserDefined || kind == ProjectItemType.Strings)
            {
                var dataType = SimpleServices.Common.Controller.GetInstance().DataTypes.FindUserDataType(name, false);
                if (!((dataType as AssetDefinedDataType)?.IsTmp ?? true))
                    return dataType;
            }
            else if (kind == ProjectItemType.AddOnInstructions || kind == ProjectItemType.AddOnDefined)
            {

                return (SimpleServices.Common.Controller.GetInstance().AOIDefinitionCollection as
                    AoiDefinitionCollection)?.Find(name);
            }
            else if (kind == ProjectItemType.Program)
            {
                return SimpleServices.Common.Controller.GetInstance().Programs[name] as BaseComponent;
            }
            else if (kind == ProjectItemType.ProgramTags)
            {
                var program = associatedObject as Program;
                return program?.Tags[name];
            }
            else if (kind == ProjectItemType.ControllerTags)
            {
                return SimpleServices.Common.Controller.GetInstance().Tags[name] as BaseComponent;
            }
            else if (kind == ProjectItemType.AddOnDefinedTags)
            {
                var aoi = associatedObject as AoiDefinition;
                return aoi?.Tags[name] as BaseComponent;
            }
            else if (kind == ProjectItemType.Routine)
            {
                var programModule = associatedObject as IProgramModule;
                return programModule?.Routines[name] as BaseComponent;
            }
            else if (kind == ProjectItemType.ModuleDefined || kind == ProjectItemType.Bus ||
                     kind == ProjectItemType.Ethernet)
            {
                return SimpleServices.Common.Controller.GetInstance().DeviceModules[name] as IBaseComponent;
            }

            Debug.Assert(false);
            return null;
        }

        public static Type GetItemType(ProjectItemType type)
        {
            if (type == ProjectItemType.UserDefined || type == ProjectItemType.Strings)
            {
                return typeof(IDataType);
            }

            if (type == ProjectItemType.AddOnDefined)
            {
                return typeof(IAoiDefinition);
            }

            if (type == ProjectItemType.ModuleDefined)
            {
                return typeof(IDeviceModule);
            }

            if (type == ProjectItemType.ControllerTags || type == ProjectItemType.AddOnDefinedTags ||
                type == ProjectItemType.ProgramTags)
            {
                return typeof(ITag);
            }

            Debug.Assert(false, type.ToString());
            return null;
        }

        private string LoadConfig(ProjectItemType kind, IBaseObject associatedObject, JObject json,
            List<IRoutine> pendingVerifyRoutines, bool isTmp = false)
        {
            if (json == null) throw new ICSStudioException();
            if (kind == ProjectItemType.UserDefined || kind == ProjectItemType.Strings)
            {
                var dataType = json;
                (Controller as Controller)?.AddDataType(dataType, isTmp);
            }
            else if (kind == ProjectItemType.AddOnInstructions)
            {
                var aoi = json;
                var aoiDefinition = ((Controller)Controller).AddAOIDefinition(aoi, isTmp);
                pendingVerifyRoutines?.AddRange(aoiDefinition.Routines);
            }
            else if (kind == ProjectItemType.Task ||
                     kind == ProjectItemType.UnscheduledPrograms ||
                     kind == ProjectItemType.FaultHandler ||
                     kind == ProjectItemType.PowerHandler)
            {

                var jsonProgram = json;
                if (Controller?.Programs[jsonProgram["Name"]?.ToString()] == null)
                {
                    Program program;
                    try
                    {
                        program = (Controller as Controller)?.AddProgram(jsonProgram);
                    }
                    catch (Exception)
                    {
                        program = new Program(Controller);
                        program.Name = jsonProgram["Name"]?.ToString();

                        // Tags
                        foreach (var tag in jsonProgram["Tags"])
                        {
                            program.AddTag(tag);
                        }

                        //program.CheckTestStatus();
                        ((ProgramCollection)Controller?.Programs)?.AddProgram(program);
                    }

                    if (program != null)
                        program.ParentTask = Controller?.Tasks[Convert.ToString(jsonProgram["ScheduleIn"])];

                    if (kind == ProjectItemType.FaultHandler)
                    {
                        Controller myController = Controller as Controller;
                        if (myController != null && program != null)
                            myController.MajorFaultProgram = program.Name;
                    }

                    if (kind == ProjectItemType.PowerHandler)
                    {
                        Controller myController = Controller as Controller;
                        if (myController != null && program != null)
                            myController.PowerLossProgram = program.Name;
                    }

                    if (program != null)
                        pendingVerifyRoutines?.AddRange(program.Routines);
                }
            }
            else if (kind == ProjectItemType.Program)
            {
                var jsonRoutine = json;
                var selectedProject = associatedObject as Program;
                if (selectedProject != null &&
                    selectedProject.Routines[jsonRoutine["Name"]?.ToString()] == null)
                {
                    var routine = (Controller as Controller)?.CreateRoutine(jsonRoutine);
                    if (routine != null)
                    {
                        pendingVerifyRoutines?.Add(routine);
                        selectedProject?.AddRoutine(routine);
                    }
                    
                }
            }
            else if (kind == ProjectItemType.ControllerTags)
            {
                var jsonTag = json;
                if (Controller?.Tags[jsonTag["Name"].ToString()] == null)
                {
                    try
                    {
                        (Controller as Controller)?.AddTag(jsonTag);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);

                    }
                }
            }
            else if (kind == ProjectItemType.ProgramTags)
            {
                var parentProgram = associatedObject as IProgramModule;
                Debug.Assert(parentProgram != null);
                var jsonTag = json;
                if (parentProgram.Tags[jsonTag["Name"].ToString()] == null)
                {
                    try
                    {
                        var tag = TagsFactory.CreateTag((TagCollection)parentProgram.Tags, jsonTag);
                        ((TagCollection)parentProgram.Tags).AddTag(tag, false, false);
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine(exception);

                    }
                }
            }
            else if (kind == ProjectItemType.DeviceModule)
            {
                var jsonModule = json;
                if (Controller?.DeviceModules[jsonModule["Name"]?.ToString()] == null)
                {
                    (Controller as Controller)?.AddDeviceModule(jsonModule);
                }
            }
            else
            {
                return $"Failed to import file.The file does not contain a export.";
            }

            return "";
        }

        private string PreLoad(JObject config)
        {
            List<string> dataTypeList = new List<string>();
            List<string> aoiList = new List<string>();
            var addOnInstructionDefinitions = config["AddOnInstructionDefinitions"];
            foreach (JObject aoi in addOnInstructionDefinitions)
            {
                aoiList.Add(aoi["Name"]?.ToString());
                var result = LoadConfig(ProjectItemType.AddOnInstructions, null, (JObject)aoi, null, true);
                dataTypeList.Add(aoi["Name"]?.ToString());
                if (!string.IsNullOrEmpty(result)) return result;
            }

            var dataTypes = config["DataTypes"];
            foreach (JObject dataType in dataTypes)
            {
                dataTypeList.Add(dataType["Name"]?.ToString());
                var result = LoadConfig(ProjectItemType.UserDefined, null, dataType, null, true);
                if (!string.IsNullOrEmpty(result)) return result;
            }

            //FinalizeTypeCreation
            var dataTypeCollection = (DataTypeCollection)Controller?.DataTypes;
            foreach (var dataTypeName in dataTypeList)
            {
                var dataType = dataTypeCollection?[dataTypeName];
                var udi = dataType as AssetDefinedDataType;
                udi?.PostInit(Controller?.DataTypes);
            }

            //var aoiDefinitionCollection = (AoiDefinitionCollection) Controller.AOIDefinitionCollection;
            //foreach (var aoiName in aoiList)
            //{
            //    var aoi = aoiDefinitionCollection?.Find(aoiName, true);

            //    aoi?.PostInit(Controller.DataTypes as DataTypeCollection);
            //}

            //FinalizeAoi
            //foreach (var aoiName in aoiList)
            //{
            //    var aoi = aoiDefinitionCollection?.Find(aoiName, true);
            //    aoi?.ParserTags();
            //}

            return "";
        }

        private void CancelPreLoad(JObject config)
        {
            var addOnInstructionDefinitions = config["AddOnInstructionDefinitions"];
            foreach (JObject aoi in addOnInstructionDefinitions)
            {
                (Controller?.AOIDefinitionCollection as AoiDefinitionCollection)?.Remove(aoi["Name"]
                    ?.ToString(), true);
            }

            var dataTypeCollection = (DataTypeCollection)Controller?.DataTypes;
            var dataTypes = config["DataTypes"];
            foreach (JObject dataType in dataTypes)
            {
                var tmpDataType = dataTypeCollection?.FindUserDataType(dataType["Name"]?.ToString(), true);
                ((DataTypeCollection)Controller?.DataTypes)?.DeleteDataType(tmpDataType);
            }
        }

        private string LoadTag(JObject tag, IBaseObject associatedObject)
        {
            if ("Create".Equals(tag["Operation"]?.ToString()))
            {
                var finalName = tag["FinalName"]?.ToString();
                if (finalName.StartsWith("\\"))
                {
                    var programName = finalName.Substring(1, finalName.IndexOf(".") - 1);
                    var program = Controller?.Programs[programName];
                    Debug.Assert(program != null, finalName);

                    tag["Name"] = tag["Name"]?.ToString().Substring(program.Name.Length + 2);
                    var result = LoadConfig(ProjectItemType.ProgramTags, program, tag, null);
                    if (!string.IsNullOrEmpty(result)) return result;
                }
                else
                {
                    var result =
                        LoadConfig(
                            associatedObject == null ? ProjectItemType.ControllerTags : ProjectItemType.ProgramTags,
                            associatedObject, tag, null);
                    if (!string.IsNullOrEmpty(result)) return result;
                }


            }
            else if ("Discard".Equals(tag["Operation"]?.ToString()) ||
                     "Use Existing".Equals(tag["Operation"]?.ToString()) ||
                     "Undefined".Equals(tag["Operation"]?.ToString()))
            {

            }
            else if ("Overwrite".Equals(tag["Operation"]?.ToString()))
            {
                var finalName = tag["FinalName"]?.ToString();
                if (finalName.StartsWith("\\"))
                {
                    var programName = finalName.Substring(1, finalName.IndexOf(".") - 1);
                    var program = Controller?.Programs[programName];
                    Debug.Assert(program != null, finalName);

                    var tagName = finalName.Substring(programName.Length + 2);
                    var existTag = program.Tags[tagName];
                    Debug.Assert(existTag != null, $"error tag{tag["Name"]}");
                    (existTag as Tag)?.Override(tag);
                }
                else
                {
                    var existTag = associatedObject == null
                        ? Controller?.Tags[tag["Name"]?.ToString()]
                        : (associatedObject as IProgramModule)?.Tags[tag["Name"]?.ToString()];

                    Debug.Assert(existTag != null, $"error tag{tag["Name"]}");
                    (existTag as Tag)?.Override(tag);
                }
            }
            else
            {

            }

            return "";
        }

        private string LoadAttachConfig(IBaseObject associatedObject, JObject config, ProjectItemType kind,
            List<IRoutine> pendingVerifyRoutines, ImportingViewModel importingVm)
        {
            List<IDeviceModule> moduleInstances = new List<IDeviceModule>();
            List<ITag> tagInstances = new List<ITag>();
            List<string> dataTypeList = new List<string>();
            List<string> aoiList = new List<string>();

            var dataTypes = config["DataTypes"];
            importingVm.Maximum = dataTypes.Count();
            foreach (JObject dataType in dataTypes)
            {
                if ("Create".Equals(dataType["Operation"]?.ToString()))
                {
                    dataTypeList.Add(dataType["Name"]?.ToString());
                    var result = LoadConfig(ProjectItemType.UserDefined, associatedObject, dataType,
                        pendingVerifyRoutines);
                    if (!string.IsNullOrEmpty(result)) return result;
                }
                else if ("Discard".Equals(dataType["Operation"]?.ToString()) ||
                         "Use Existing".Equals(dataType["Operation"]?.ToString()))
                {
                    continue;
                }
                else if ("Overwrite".Equals(dataType["Operation"]?.ToString()))
                {
                    var existDataType = Controller?.DataTypes[dataType["Name"]?.ToString()];
                    dataTypeList.Add(existDataType?.Name);
                    ((UserDefinedDataType)existDataType)?.Overwrite(dataType, Controller?.DataTypes);
                }
                else
                {
                    Debug.Assert(false);
                }

                importingVm.Worker.ReportProgress(1);
            }

            var addOnInstructionDefinitions = config["AddOnInstructionDefinitions"];
            importingVm.Maximum = addOnInstructionDefinitions.Count();
            foreach (JObject aoi in addOnInstructionDefinitions)
            {
                if ("Create".Equals(aoi["Operation"]?.ToString()))
                {
                    aoiList.Add(aoi["Name"]?.ToString());
                    var result = LoadConfig(ProjectItemType.AddOnInstructions, associatedObject, (JObject)aoi,
                        pendingVerifyRoutines);
                    dataTypeList.Add(aoi["Name"]?.ToString());
                    if (!string.IsNullOrEmpty(result)) return result;
                }
                else if ("Discard".Equals(aoi["Operation"]?.ToString()) ||
                         "Use Existing".Equals(aoi["Operation"]?.ToString()))
                {
                    continue;
                }
                else if ("Overwrite".Equals(aoi["Operation"]?.ToString()))
                {
                    aoiList.Add(aoi["Name"]?.ToString());
                    dataTypeList.Add(aoi["Name"]?.ToString());
                    var existAoi = (Controller?.AOIDefinitionCollection as AoiDefinitionCollection)?.Find(aoi["Name"]
                        ?.ToString());
                    Debug.Assert(existAoi != null);
                    existAoi.Overwrite(aoi, Controller);
                    pendingVerifyRoutines.AddRange(existAoi.Routines);
                }
                else
                {
                    Debug.Assert(false);
                }

                importingVm.Worker.ReportProgress(1);
            }

            //FinalizeTypeCreation
            importingVm.Maximum = dataTypeList.Count();
            foreach (var dataTypeName in dataTypeList)
            {
                var dataType = Controller?.DataTypes[dataTypeName];
                var udi = dataType as AssetDefinedDataType;
                if (udi != null)
                {
                    if (udi.CanPostOverwrite)
                    {
                        udi.PostOverwrite(Controller?.DataTypes);
                    }
                    else
                    {
                        udi.PostInit(Controller?.DataTypes);
                    }
                }
                else
                {
                    Debug.Assert(false, dataTypeName);
                }

                importingVm.Worker.ReportProgress(1);
            }

            importingVm.Maximum = aoiList.Count();
            foreach (var aoiName in aoiList)
            {
                var aoi = (Controller?.AOIDefinitionCollection as AoiDefinitionCollection)?.Find(aoiName);
                if (aoi.CanPostOverwrite)
                    aoi.PostOverwrite();
                //else
                //    aoi?.PostInit(Controller.DataTypes as DataTypeCollection);
                importingVm.Worker.ReportProgress(1);
            }

            //FinalizeAoi
            //foreach (var aoiName in aoiList)
            //{
            //    var aoi = (Controller.AOIDefinitionCollection as AoiDefinitionCollection)?.Find(aoiName);
            //    aoi?.ParserTags();
            //}

            var tags = config["Tags"];
            importingVm.Maximum = tags.Count();
            foreach (var tag in tags)
            {
                try
                {
                    var result = LoadTag((JObject)tag, null);
                    if (!string.IsNullOrEmpty(result)) return result;
                    if ("Create".Equals(tag["Operation"]?.ToString()))
                        tagInstances.Add(Controller?.Tags[(string)tag["Name"]]);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    throw;
                }

                importingVm.Worker.ReportProgress(1);
            }

            var modules = config["Modules"];
            importingVm.Maximum = modules.Count();
            foreach (JObject module in modules)
            {
                if ("Create".Equals(module["Operation"]?.ToString()))
                {
                    //TODO(zyl):refactor
                    var parentModuleType = module["ParentModule"]?.ToString();
                    IDeviceModule parentModule = Controller?.DeviceModules[parentModuleType];
                    if (parentModule == null && "local".Equals(parentModuleType, StringComparison.OrdinalIgnoreCase))
                    {
                        parentModule = Controller?.DeviceModules.FirstOrDefault(d => d is LocalModule);
                    }

                    if (parentModule == null) parentModule = (IDeviceModule)associatedObject;
                    var mes = CheckModule(parentModule, module, kind);
                    if (!string.IsNullOrEmpty(mes))
                    {
                        if (mes.Equals("Discard")) continue;
                        return mes;
                    }

                    var result = LoadConfig(ProjectItemType.DeviceModule, associatedObject, module,
                        pendingVerifyRoutines);
                    if (!string.IsNullOrEmpty(result)) return result;
                    moduleInstances.Add(Controller?.DeviceModules[(string)module["Name"]]);
                }
                else if ("Discard".Equals(module["Operation"]?.ToString()) ||
                         "Use Existing".Equals(module["Operation"]?.ToString()))
                {
                    continue;
                }
                else if ("Overwrite".Equals(module["Operation"]?.ToString()))
                {
                    //TODO(zyl):Override

                    if (Controller != null)
                    {
                        foreach (var deviceModule in Controller.DeviceModules)
                        {
                            if (deviceModule.Name.Equals(module["Name"]?.ToString(),
                                StringComparison.OrdinalIgnoreCase))
                            {
                                (Controller.DeviceModules as DeviceModuleCollection)?.RemoveDeviceModule(deviceModule);
                                var serviceEditorPane = Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
                                var deviceModulesInOpen = serviceEditorPane?.GetDeviceModulesInOpen();
                                if (deviceModulesInOpen != null)
                                {
                                    foreach (var item in deviceModulesInOpen)
                                    {
                                        if (Controller.DeviceModules[item.Uid] == null)
                                            serviceEditorPane.CloseWindow(item.Uid);
                                    }
                                }

                                var parentModuleType = module["ParentModule"]?.ToString();
                                IDeviceModule parentModule = Controller.DeviceModules[parentModuleType];
                                if (parentModule == null && "local".Equals(parentModuleType, StringComparison.OrdinalIgnoreCase))
                                {
                                    parentModule = Controller.DeviceModules.FirstOrDefault(d => d is LocalModule);
                                }

                                if (parentModule == null) parentModule = (IDeviceModule)associatedObject;
                                var mes = CheckModule(parentModule, module, kind);
                                if (!string.IsNullOrEmpty(mes))
                                {
                                    if (mes.Equals("Discard")) continue;
                                    return mes;
                                }

                                var result = LoadConfig(ProjectItemType.DeviceModule, associatedObject, module,
                                    pendingVerifyRoutines);
                                if (!string.IsNullOrEmpty(result)) return result;
                                moduleInstances.Add(Controller.DeviceModules[(string)module["Name"]]);

                                break;
                            }
                        }
                    }
                }
                else
                {

                }

                importingVm.Worker.ReportProgress(1);
            }

            var programs = config["Programs"];
            if (kind == ProjectItemType.Task)
            {
                var scheduleInTask = Controller.Tasks[Convert.ToString(programs.First()["ScheduleIn"])];
                var programScheduleInTaskCounts = 0;
                if (scheduleInTask != null)
                {
                    foreach (var item in Controller.Programs)
                    {
                        if (item.ParentTask == scheduleInTask)
                        {
                            programScheduleInTaskCounts++;
                        }
                    }
                }
                int programWillBeCreatedCounts = 0;
                foreach (var item in programs)
                {
                    if ("Create".Equals(Convert.ToString(item["Operation"])))
                        programWillBeCreatedCounts++;
                    if (programWillBeCreatedCounts > 1000)
                        break;
                }
                if (programScheduleInTaskCounts + programWillBeCreatedCounts > 1000)
                {
                    foreach (var item in programs)
                    {
                        if ("Create".Equals(Convert.ToString(item["Operation"])))
                            item["ScheduleIn"] = "Unscheduled";
                    }
                }
            }

            importingVm.Maximum = programs.Count();
            Tuple<string, string> parentProgram = null;
            foreach (var program in programs)
            {
                if (kind == ProjectItemType.Program)
                {
                    //routine
                    if (!(program["Routines"] != null && program["Routines"]
                                .FirstOrDefault(r => r["Use"] != null && "Target".Equals(r["Use"]?.ToString())) !=
                            null)) continue;
                    var routines = program["Routines"] as JArray;
                    var routine = routines?[0] as JObject;
                    if ("Create".Equals(routine["Operation"]?.ToString()))
                    {
                        var result = LoadConfig(ProjectItemType.Program, associatedObject, routine,
                            pendingVerifyRoutines);
                        if (!string.IsNullOrEmpty(result)) return result;
                        foreach (JObject tagConfig in program["Tags"])
                        {
                            var result2 = LoadTag(tagConfig, associatedObject);
                            if (!string.IsNullOrEmpty(result2)) return result2;
                        }
                    }
                    else if ("Discard".Equals(routine["Operation"]?.ToString()) ||
                             "Use Existing".Equals(routine["Operation"]?.ToString()))
                    {
                        continue;
                    }
                    else if ("Overwrite".Equals(routine["Operation"]?.ToString()))
                    {
                        //TODO(zyl):Override
                        IProgram existProgram = null;
                        if ("Target".Equals((string)routine["Use"]))
                        {
                            existProgram = associatedObject as IProgram;
                        }
                        else
                        {
                            existProgram = Controller?.Programs[program["Name"]?.ToString()];
                        }

                        Debug.Assert(existProgram != null, kind.ToString());
                        var existRoutine = existProgram.Routines[routine["Name"]?.ToString()];
                        Debug.Assert(existRoutine != null, $"Error routine{routine["Name"]}");
                        ((RoutineCollection)existProgram.Routines).DeleteRoutine(existRoutine);
                        var result = LoadConfig(ProjectItemType.Program, associatedObject, routine,
                            pendingVerifyRoutines);
                        if (!string.IsNullOrEmpty(result)) return result;
                        foreach (JObject tagConfig in program["Tags"])
                        {
                            var result2 = LoadTag((JObject)tagConfig, associatedObject);
                            if (!string.IsNullOrEmpty(result2)) return result2;
                        }
                    }
                }
                else if (kind == ProjectItemType.Routine)
                {
                    //rung
                    if (!(program["Routines"] != null && program["Routines"]
                                .FirstOrDefault(r => r["Use"] != null && "Context".Equals(r["Use"]?.ToString())) !=
                            null)) continue;
                    var rllRoutine = associatedObject as RLLRoutine;
                    var contextRoutine = program["Routines"]?[0] as JObject;
                    if ("Create".Equals(contextRoutine["Operation"]?.ToString()))
                    {
                        foreach (JObject tagConfig in program["Tags"])
                        {
                            var result2 = LoadTag(tagConfig, rllRoutine.ParentCollection.ParentProgram);
                            if (!string.IsNullOrEmpty(result2)) return result2;
                        }

                        var codeText = new List<string>();
                        if (rllRoutine.CodeText.Count == 0)
                        {
                            foreach (var item in contextRoutine["Rungs"])
                                codeText.Add(item["Text"].ToString());
                        }
                        else
                        {
                            var endIndex = int.Parse(contextRoutine["EndIndex"]?.ToString());
                            for (var i = 0; i < rllRoutine.CodeText.Count; i++)
                            {
                                codeText.Add(rllRoutine.CodeText[i]);
                                if (i == endIndex)
                                    foreach (var item in contextRoutine["Rungs"])
                                        codeText.Add(item["Text"].ToString());
                            }
                        }

                        rllRoutine.UpdateRungs(codeText);
                        pendingVerifyRoutines.Add(rllRoutine);
                    }
                    else if ("Discard".Equals(contextRoutine["Operation"]?.ToString()) ||
                             "Use Existing".Equals(contextRoutine["Operation"]?.ToString()))
                    {
                        continue;
                    }
                    else if ("Overwrite".Equals(contextRoutine["Operation"]?.ToString()))
                    {
                        foreach (JObject tagConfig in program["Tags"])
                        {
                            var result2 = LoadTag(tagConfig, rllRoutine.ParentCollection.ParentProgram);
                            if (!string.IsNullOrEmpty(result2)) return result2;
                        }

                        var startIndex = int.Parse(contextRoutine["StartIndex"]?.ToString());
                        var endIndex = int.Parse(contextRoutine["EndIndex"]?.ToString());
                        var codeText = new List<string>();
                        for (var i = 0; i < rllRoutine.CodeText.Count; i++)
                            if (i >= startIndex && i <= endIndex)
                                foreach (var item in contextRoutine["Rungs"])
                                    codeText.Add(item["Text"].ToString());
                            else
                                codeText.Add(rllRoutine.CodeText[i]);
                        rllRoutine.UpdateRungs(codeText);
                    }
                }
                else
                {
                    //program
                    if ("Create".Equals(program["Operation"]?.ToString()))
                    {
                        var result = LoadConfig(kind, associatedObject, (JObject)program, pendingVerifyRoutines);

                        if (!string.IsNullOrEmpty(result)) return result;
                    }
                    else if ("Discard".Equals(program["Operation"]?.ToString()) ||
                             "Use Existing".Equals(program["Operation"]?.ToString()))
                    {
                        continue;
                    }
                    else if ("Overwrite".Equals(program["Operation"]?.ToString()))
                    {
                        var existProgram = ((Controller)Controller)?.Programs[program["Name"]?.ToString()];
                        Debug.Assert(existProgram != null, $"error program{program["Name"]}");
                        if (program["ScheduleIn"] != null)
                        {
                            var scheduleIn = Convert.ToString(program["ScheduleIn"]);
                            var parentTask = existProgram.ParentController.Tasks[scheduleIn];
                            if (parentTask != null)
                            {
                                if (existProgram.ParentTask != null)
                                {
                                    if (!existProgram.ParentTask.Name.Equals(scheduleIn, StringComparison.OrdinalIgnoreCase))
                                    {
                                        existProgram.ParentTask.UnscheduleProgram(existProgram);
                                        parentTask.ScheduleProgram(existProgram);
                                    }
                                }
                                else
                                {
                                    parentTask.ScheduleProgram(existProgram);
                                }
                            }
                            else
                            {
                                if (existProgram.ParentTask != null)
                                {
                                    parentTask.ScheduleProgram(existProgram);
                                }
                            }
                        }
                        ((Program)existProgram).Override((JObject)program, existProgram.ParentCollection);
                        pendingVerifyRoutines.AddRange(((Program)existProgram).Routines);
                    }

                    if (program["Parent"] != null)
                    {
                        parentProgram =
                            new Tuple<string, string>(program["Name"]?.ToString(), program["Parent"]?.ToString());
                    }
                }

                importingVm.Worker.ReportProgress(1);
            }

            if (kind == ProjectItemType.Task && parentProgram != null)
            {
                var parent = ((Controller)Controller)?.Programs[parentProgram.Item2];
                (parent?.ChildCollection as ProgramCollection)?.AddProgram(
                    Controller.Programs[parentProgram.Item1] as Program);
            }


            var ignoreAxisPass = kind == ProjectItemType.Program || kind == ProjectItemType.Routine ||
                                 kind == ProjectItemType.Task;
            ((Controller)Controller)?.PostImportJson(moduleInstances, tagInstances, ignoreAxisPass);
            return "";
        }

        #endregion

        #region Verify

        public bool VerifyInDialog()
        {
            var dialog = new VerifyDialog
            {
                Owner = Application.Current.MainWindow
            };

            return dialog.ShowDialog() ?? false;
        }

        public static ExpectType GetUnsignedDataType(IController controller)
        {
            var list = new List<IDataType>() { USINT.Inst, UINT.Inst, UDINT.Inst, ULINT.Inst,LREAL.Inst };
            var expectedType = new ExpectType(USINT.Inst, UINT.Inst, UDINT.Inst, ULINT.Inst, LREAL.Inst);
            foreach (var type in controller.DataTypes.Reverse())
            {
                if (type.IsPredefinedType) break;
                if (CheckIsContainUnsignedDataType(type as CompositiveType, expectedType)) list.Add(type);
            }

            return new ExpectType(list.ToArray());
        }

        private static bool CheckIsContainUnsignedDataType(CompositiveType dataType, ExpectType expectedType)
        {
            if (dataType == null) return false;
            var compositiveType = dataType;
            foreach (var member in compositiveType.TypeMembers)
            {
                var type = member.DataTypeInfo.DataType as CompositiveType;
                if (type != null)
                {
                    if (type.IsPredefinedType) continue;
                    if (CheckIsContainUnsignedDataType(type, expectedType)) return true;
                    else continue;
                }

                IDataType memberType = member.DataTypeInfo.DataType;
                if (member.DataTypeInfo.Dim1 > 0)
                {
                    memberType = new ArrayType(memberType, member.DataTypeInfo.Dim1, member.DataTypeInfo.Dim2,
                        member.DataTypeInfo.Dim3);
                }

                if (memberType.Equal(expectedType, true))
                {
                    return true;
                }
            }

            return false;
        }

        private void CheckResourceLimit(IController controller, IErrorOutputService outputService)
        {
            var localModule = controller.DeviceModules["Local"] as LocalModule;
            var type = localModule?.DisplayText;

            var resourceLimit = new KeyValuePair<string, ResourceLimitExtensions.ResourceType>();
            foreach (var item in ResourceLimitExtensions.ResourceLimitDictionary)
            {
                if (type.StartsWith(item.Key, StringComparison.OrdinalIgnoreCase))
                {
                    resourceLimit = item;
                }
            }

            #region 1.task 优先级、类型
            var taskResults = ResourceLimitExtensions.CheckTask(controller);
            foreach (var taskResult in taskResults)
            {
                switch (taskResult)
                {
                    case ResourceLimitExtensions.TaskResult.TypeError:
                        if (ResourceLimitExtensions.ContinuousTaskList.Count > 1)
                        {
                            var str = "";
                            foreach (var item in ResourceLimitExtensions.ContinuousTaskList)
                            {
                                if (str == "")
                                {
                                    str = $"'{item.Name}'";
                                }
                                else
                                {
                                    str = string.Format($"{str}, '{item.Name}'");
                                }
                            }

                            outputService?.AddErrors(
                                $"This controller only supports 1 Continuous task, but now there are {ResourceLimitExtensions.ContinuousTaskList.Count} Continuous tasks : {str}",
                                OrderType.None, OnlineEditType.Original, null, null, null);
                        }
                        else if (ResourceLimitExtensions.EventPeriodicTaskList.Count > 15)
                        {
                            var str = "";
                            foreach (var item in ResourceLimitExtensions.EventPeriodicTaskList)
                            {
                                if (str == "")
                                {
                                    str = $"'{item.Name}'";
                                }
                                else
                                {
                                    str = string.Format($"{str}, '{item.Name}'");
                                }
                            }

                            outputService?.AddErrors(
                                $"This controller only supports 15 Event and Periodic tasks, but now there are {ResourceLimitExtensions.EventPeriodicTaskList.Count} Event and Periodic tasks : {str}",
                                OrderType.None, OnlineEditType.Original, null, null, null);
                        }

                        break;
                    case ResourceLimitExtensions.TaskResult.PriorityError:
                        var messageDictionary = new Dictionary<int, string>();
                        foreach (var item in ResourceLimitExtensions.TaskPriorityDictionary)
                        {
                            if (item.Value.Count > 1)
                            {
                                foreach (var task in item.Value)
                                {
                                    var message = $"Task: the task '{task.Name}' cannot be set the priority '{item.Key}'. Task priority must be different from others";
                                    outputService?.AddErrors(message, OrderType.None, OnlineEditType.Original, null, null, task);
                                }
                            }
                        }
                        break;
                    case ResourceLimitExtensions.TaskResult.Normal:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            #endregion

            #region 2.program 数量
            var programResult = ResourceLimitExtensions.CheckProgram(controller);
            switch (programResult)
            {
                case ResourceLimitExtensions.ProgramResult.OutRange:
                    var str = "";
                    foreach (var item in ResourceLimitExtensions.ProgramDictionary)
                    {
                        if (item.Value > 1000)
                        {
                            if (str == "")
                            {
                                str = $"'{item.Key.Name}'";
                            }
                            else
                            {
                                str = string.Format($"{str}, '{item.Key.Name}'");
                            }
                        }
                    }

                    outputService?.AddErrors(
                        $"This controller supports only 1000 programs, {str} has more than 1000 programs",
                        OrderType.None, OnlineEditType.Original, null, null, null);
                    break;
                case ResourceLimitExtensions.ProgramResult.Normal:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            #endregion

            #region 3.axis 数量
            var axisResult = ResourceLimitExtensions.CheckAxis(controller);
            switch (axisResult)
            {
                case ResourceLimitExtensions.AxisResult.OutRange:
                    outputService?.AddErrors(
                        $"{resourceLimit.Key} only supports {resourceLimit.Value.Axis} axes, but now has {ResourceLimitExtensions.AxisCount} axes",
                        OrderType.None, OnlineEditType.Original, null, null, null);
                    break;
                case ResourceLimitExtensions.AxisResult.Normal:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            #endregion

            #region 4.Axis-Position数量
            var axisPositionResult = ResourceLimitExtensions.CheckAxisPosition(controller);
            switch (axisPositionResult)
            {
                case ResourceLimitExtensions.AxisPositionResult.OutRange:
                    outputService?.AddErrors(
                        $"{resourceLimit.Key} only supports {resourceLimit.Value.AxisPosition} Axis Position, but now has {ResourceLimitExtensions.AxisPositionCount} Axis Position",
                        OrderType.None, OnlineEditType.Original, null, null, null);
                    break;
                case ResourceLimitExtensions.AxisPositionResult.Normal:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            #endregion

            #region 5.network 数量
            var networkResult = ResourceLimitExtensions.CheckNetwork(controller);
            switch (networkResult)
            {
                case ResourceLimitExtensions.NetworkResult.OutRange:
                    outputService?.AddErrors(
                        $"{resourceLimit.Key} only supports {resourceLimit.Value.Network} network, but now has {ResourceLimitExtensions.NetworkCount} network",
                        OrderType.None, OnlineEditType.Original, null, null, null);
                    break;
                case ResourceLimitExtensions.NetworkResult.Normal:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            #endregion

            #region 6.AOI输入参数加上输出参数加上本地标记 数量 7.AOI InOutput参数 数量
            var aoiTagResults = ResourceLimitExtensions.CheckAOITag(controller);
            if (aoiTagResults.Contains(ResourceLimitExtensions.AOITagResult.InputOutputLocalOutRange))
            {
                var str = "";
                foreach (var item in ResourceLimitExtensions.AoiDefinitionDictionary)
                {
                    if (item.Value.InputOutputLocal > 512)
                    {
                        if (str == "")
                        {
                            str = $"'{item.Key.Name}'";
                        }
                        else
                        {
                            str = String.Format($"{str}, '{item.Key.Name}'");
                        }
                    }
                }
                outputService?.AddErrors(
                    $"There are no more than 512 Input tags plus Output tags plus Local tags in this controller, but {str} has more than 512"
                    , OrderType.None, OnlineEditType.Original, null, null, null);
            }
            if (aoiTagResults.Contains(ResourceLimitExtensions.AOITagResult.InOutOutRange))
            {
                var str = "";
                foreach (var item in ResourceLimitExtensions.AoiDefinitionDictionary)
                {
                    if (item.Value.InOut > 64)
                    {
                        if (str == "")
                        {
                            str = $"'{item.Key.Name}'";
                        }
                        else
                        {
                            str = String.Format($"{str}, '{item.Key.Name}'");
                        }
                    }
                }
                outputService?.AddErrors(
                    $"There are no more than 64 InOut tags in this controller, but {str} has more than 64"
                    , OrderType.None, OnlineEditType.Original, null, null, null);
            }
            #endregion

            //TODO(clx):待补充 梯形图Rung嵌套层数限制、AOI嵌套层数限制、JSR指令调用深度和InOuts/Outputs限制
        }

        public void Verify(IController controller)
        {
            if(controller == null)
                return;

            ((Controller)controller).IsPass = true;

            var outputService = Package.GetGlobalService(typeof(SErrorOutputService)) as IErrorOutputService;
            outputService?.Cleanup();

            //检查资源限制
            CheckResourceLimit(controller,outputService);

             //TODO(zyl):add more verification
             var createEditorService =
                Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;

            var unsignedTypes = GetUnsignedDataType(controller);
            {
                var tags = controller.Tags.Where(t =>
                    t.Name.Length > VerifyViewModel.NameLengthLimit || ((Tag)t).DataWrapper is MessageDataWrapper ||
                    ((Tag)t).DataWrapper is MotionGroup || t.DataTypeInfo.DataType.Equal(unsignedTypes, true)).ToList();
                var mg = tags.FirstOrDefault(t => ((Tag)t).DataWrapper is MotionGroup);
                tags.Remove(mg);
                if (mg != null)
                {
                    var cup = ((MotionGroup)((Tag)mg).DataWrapper).CoarseUpdatePeriod;
                    if (cup % 500 == 0 && cup % 1000 != 0)
                    {
                        outputService?.AddErrors("Error: Motion Group : 'Base Update Period' can be set *.5ms",
                            OrderType.None, OnlineEditType.Original, null, null, null);
                    }
                }

                var messageTags = tags.Where(t => ((Tag)t).DataWrapper is MessageDataWrapper).ToList();
                foreach (var messageTag in messageTags)
                {
                    VerifyViewModel.VerifyMessageConfig((Tag)messageTag, outputService);
                    tags.Remove(messageTag);
                }

                foreach (var type in unsignedTypes.ExpectTypes)
                {
                    if (type.IsPredefinedType) continue;
                    outputService?.AddErrors(
                        $"Error:The userDataType '{type.Name}' contain unsigned data type.This controller doesn't support unsigned data type. ",
                        OrderType.None, OnlineEditType.Original, null, null, type);
                }

                foreach (var tag in tags)
                {
                    if (tag.Name.Length > VerifyViewModel.NameLengthLimit)
                    {
                        outputService?.AddErrors($"Error:The Tag '{tag.Name}' Name's length limits in NameLengthLimit.",
                            OrderType.None, OnlineEditType.Original, null, null, tag);
                        continue;
                    }

                    outputService?.AddErrors(
                        $"Error:The Tag '{tag.Name}' contain unsigned data type.This controller doesn't support unsigned data type. ",
                        OrderType.None, OnlineEditType.Original, null, null, tag);
                }

            }

            foreach (var program in controller.Programs)
            {
                var tags = program.Tags.Where(t =>
                    t.Name.Length > VerifyViewModel.NameLengthLimit ||
                    t.DataTypeInfo.DataType.Equal(unsignedTypes, true)).ToList();
                foreach (var tag in tags)
                {
                    if (tag.Name.Length > VerifyViewModel.NameLengthLimit)
                    {
                        outputService?.AddErrors($"Error:The Tag '{tag.Name}' Name's length limits in NameLengthLimit.",
                            OrderType.None, OnlineEditType.Original, null, null, tag);
                        continue;
                    }

                    outputService?.AddErrors(
                        $"Error:The Tag '{tag.Name}' in '{program.Name}' contain unsigned data type.This controller doesn't support unsigned data type. ",
                        OrderType.None, OnlineEditType.Original, null, null, tag);
                }

                if (string.IsNullOrEmpty(program.MainRoutineName))
                    outputService?.AddWarnings($"Warning: {program.Name}:Program doesn't have associated main routine.",
                        program);
                foreach (var routine in program.Routines)
                {
                    var stRoutine = routine as STRoutine;
                    if (stRoutine != null)
                    {
                        createEditorService?.ParseRoutine(stRoutine, true);
                    }

                    //TODO(zyl):add other routine
                }
            }

            foreach (var program in controller.Programs)
            {
                foreach (var routine in program.Routines)
                {
                    var stRoutine = routine as STRoutine;
                    if (stRoutine != null)
                    {
                        if (!(createEditorService?.CheckRoutineInRun(stRoutine) ?? true))
                            outputService?.AddWarnings(
                                $"Warning: {routine.Name}:Routine cannot be reached by the main routine:'{routine.Name} of Program {program.Name}'",
                                stRoutine,null,null,Destination.ToControllerOrganizer);
                    }

                    //TODO(zyl):add other routine
                }
            }

            foreach (var aoi in controller.AOIDefinitionCollection)
            {
                var tags = aoi.Tags.Where(t =>
                    t.Name.Length > VerifyViewModel.NameLengthLimit ||
                    t.DataTypeInfo.DataType.Equal(unsignedTypes, true)).ToList();
                foreach (var tag in tags)
                {
                    if (tag.Name.Length > VerifyViewModel.NameLengthLimit)
                    {
                        outputService?.AddErrors($"Error:The Tag '{tag.Name}' Name's length limits in NameLengthLimit.",
                            OrderType.None, OnlineEditType.Original, null, null, tag);
                        continue;
                    }

                    outputService?.AddErrors(
                        $"Error:The Tag '{tag.Name}' in '{aoi.Name}' contain unsigned data type.This controller doesn't support unsigned data type. ",
                        OrderType.None, OnlineEditType.Original, null, null, tag);
                }

                foreach (var routine in aoi.Routines)
                {
                    var stRoutine = routine as STRoutine;
                    if (stRoutine != null)
                    {
                        createEditorService?.ParseRoutine(stRoutine, true);
                    }

                    //TODO(zyl):add other routine
                }
            }

            // module verify
            foreach (var module in controller.DeviceModules)
            {
                Verify(module);
            }

            // axis tag verify
            foreach (var tag in controller.Tags)
            {
                if (tag.DataTypeInfo.DataType.IsAxisType)
                {
                    VerifyAxisTag(tag);
                }
            }

            VerifyParameterConnection();
        }

        public void VerifyReferenceProgram(IProgram deleteProgram)
        {
            if (Controller == null)
                return;

            var createEditorService =
                Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;

            foreach (var program in Controller.Programs)
            {
                if (program == deleteProgram) continue;
                foreach (var routine in program.Routines)
                {
                    var stRoutine = routine as STRoutine;
                    if (stRoutine != null)
                    {
                        if (stRoutine.HasTagInOtherProgram(deleteProgram))
                        {
                            stRoutine.IsError = true;
                            createEditorService?.ParseRoutine(stRoutine, true);
                        }
                    }

                    //TODO(zyl):add other routine
                }
            }
        }

        public void Verify(IRoutine routine)
        {
            if (routine == null)
                return;

            var createEditorService = Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
            createEditorService?.ParseRoutine(routine, true);
        }

        //TODO(gjc): change name???
        public void Verify(ITag tag)
        {
            if (tag == null)
                return;

            //TODO(zyl):add more verification
            var createEditorService = Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
            if (tag.ParentCollection.ParentProgram is AoiDefinition)
            {
                foreach (var routine in ((AoiDefinition)tag.ParentCollection.ParentProgram).Routines)
                {
                    var stRoutine = routine as STRoutine;
                    if (stRoutine != null)
                    {
                        if (stRoutine.GetAllReferenceTags().Any(v => v == tag))
                            createEditorService?.ParseRoutine(stRoutine, true);
                    }
                }
            }
            else
            {
                if (Controller != null)
                {
                    foreach (var program in Controller.Programs)
                    {
                        foreach (var routine in program.Routines)
                        {
                            var stRoutine = routine as STRoutine;
                            if (stRoutine != null)
                            {
                                if (stRoutine.GetAllReferenceTags().Any(v => v == tag))
                                    createEditorService?.ParseRoutine(stRoutine, true);
                            }

                            //TODO(zyl):add other routine
                        }
                    }
                }
                
            }

            //foreach (var aoi in Controller.AOIDefinitionCollection)
            //{
            //    foreach (var routine in aoi.Routines)
            //    {
            //        var stRoutine = routine as STRoutine;
            //        if (stRoutine != null)
            //        {
            //            if (stRoutine.VariableInfos.Any(v => v.Tag == tag))
            //                createEditorService?.ParseRoutine(stRoutine, true);
            //        }

            //        //TODO(zyl):add other routine
            //    }
            //}
        }

        public void Verify(IDeviceModule module)
        {
            if (module == null)
                return;

            if (module is LocalModule)
                return;

            var outputService = Package.GetGlobalService(typeof(SErrorOutputService)) as IErrorOutputService;
            string message = string.Empty;

            CIPMotionDrive motionDrive = module as CIPMotionDrive;
            if (motionDrive != null)
            {
                var axisList = motionDrive.CheckAxisUpdatePeriod();
                if (axisList != null && axisList.Count > 0)
                {
                    foreach (var axis in axisList)
                    {
                        message =
                            $"{axis.Name}: The axis is associated to a module which has another axis associated to it at a different Update Period.";
                        outputService?.AddErrors(message, OrderType.None, OnlineEditType.Original, null, null, axis);
                    }

                }
            }

            var deviceModule = (DeviceModule)module;
            if (deviceModule.Ports.Any())
            {
                var ipPort = deviceModule.Ports.Where(p => p.Type == PortType.Ethernet);
                foreach (var port in ipPort)
                {
                    if (string.IsNullOrEmpty(port.Address))
                    {
                        outputService?.AddErrors($"{deviceModule.Name}:Invalid Ethernet Address.", OrderType.None,
                            OnlineEditType.Original, null, null, deviceModule);
                        return;
                    }
                }
            }

            //TODO(gjc): add other module verify
        }

        public void VerifyAxisTag(ITag tag)
        {
            var outputService = Package.GetGlobalService(typeof(SErrorOutputService)) as IErrorOutputService;
            string message = string.Empty;

            Tag tagObject = tag as Tag;

            if (tagObject == null)
                return;

            if (!tagObject.DataTypeInfo.DataType.IsAxisType)
                return;


            AxisCIPDrive axisCIPDrive = tagObject.DataWrapper as AxisCIPDrive;
            if (axisCIPDrive != null)
            {
                if (axisCIPDrive.AssignedGroup == null)
                {
                    message = $"Warning: {tag.Name}: Axis not associated with a motion group.";
                    outputService?.AddWarnings(message, tag,null,null,Destination.ToAxisProperty);
                    return;
                }

                float maximumDeceleration = Convert.ToSingle(axisCIPDrive.CIPAxis.MaximumDeceleration);
                float maximumSpeed = Convert.ToSingle(axisCIPDrive.CIPAxis.MaximumSpeed);
                float maximumDecelerationJerk = Convert.ToSingle(axisCIPDrive.CIPAxis.MaximumDecelerationJerk);
                float maximumAcceleration = Convert.ToSingle(axisCIPDrive.CIPAxis.MaximumAcceleration);
                float maximumAccelerationJerk = Convert.ToSingle(axisCIPDrive.CIPAxis.MaximumAccelerationJerk);

                if (maximumDeceleration == 0)
                {
                    message =
                        $"Warning: {tag.Name}: Axis Maximum Deceleration is set to 0. Motion cannot be started on the axis.";
                    outputService?.AddWarnings(message, tag,null,null,Destination.ToAxisProperty);
                }

                if (maximumSpeed == 0 || maximumDeceleration == 0 || maximumDecelerationJerk == 0)
                {
                    message =
                        $"Warning: {tag.Name}: Axis Maximum Speed, Maximum Deceleration, or Maximum Deceleration Jerk is set to 0.  S-curve motion cannot be started on the axis.";
                    outputService?.AddWarnings(message, tag, null, null, Destination.ToAxisProperty);
                }

                if (maximumAcceleration == 0 || maximumAccelerationJerk == 0)
                {
                    message = $"Warning: {tag.Name}: Axis Maximum Acceleration Jerk is set to 0% of Max Accel Time.";
                    outputService?.AddWarnings(message, tag, null, null, Destination.ToAxisProperty);
                }

                // Power Structure
                if (axisCIPDrive.AssociatedModule == null)
                {
                    message = $"Warning: {tag.Name}: Axis does not have a Power Structure associated with it.";
                    outputService?.AddWarnings(message, tag, null, null, Destination.ToAxisProperty);
                }

                // motor
                var motorType = (MotorType)Convert.ToByte(axisCIPDrive.CIPAxis.MotorType);
                if (motorType == MotorType.NotSpecified)
                {
                    message = $"Warning: {tag.Name}: Axis does not have a motor associated to it.";
                    outputService?.AddWarnings(message, tag, null, null, Destination.ToAxisProperty);
                }
                else
                {
                    var axisConfiguration = (AxisConfigurationType)Convert.ToByte(axisCIPDrive.CIPAxis.AxisConfiguration);
                    var feedback1Type = (FeedbackType)Convert.ToByte(axisCIPDrive.CIPAxis.Feedback1Type);
                    if (feedback1Type == FeedbackType.NotSpecified)
                    {
                        if (axisConfiguration == AxisConfigurationType.PositionLoop ||
                            axisConfiguration == AxisConfigurationType.VelocityLoop ||
                            axisConfiguration == AxisConfigurationType.TorqueLoop ||
                            axisConfiguration == AxisConfigurationType.FeedbackOnly)
                        {
                            message = $"Warning: {tag.Name}: Configuration requires a feedback type to be defined.";
                            outputService?.AddWarnings(message, tag, null, null, Destination.ToAxisProperty);
                        }
                        
                    }

                    //TODO(gjc): add code here
                    // feedback2Type
                }

                // motion module
                if (axisCIPDrive.AssociatedModule == null)
                {
                    message = $"Warning: {tag.Name}: Axis not associated with a motion module.";
                    outputService?.AddWarnings(message, tag, null, null, Destination.ToAxisProperty);
                }
            }

            AxisVirtual axisVirtual = tagObject.DataWrapper as AxisVirtual;
            if (axisVirtual != null)
            {
                if (axisVirtual.AssignedGroup == null)
                {
                    message = $"Warning: {tag.Name}: Axis not associated with a motion group.";
                    outputService?.AddWarnings(message, tag, null, null, Destination.ToAxisProperty);
                    return;
                }

                float maximumDeceleration = Convert.ToSingle(axisVirtual.CIPAxis.MaximumDeceleration);
                float maximumSpeed = Convert.ToSingle(axisVirtual.CIPAxis.MaximumSpeed);
                float maximumDecelerationJerk = Convert.ToSingle(axisVirtual.CIPAxis.MaximumDecelerationJerk);
                float maximumAcceleration = Convert.ToSingle(axisVirtual.CIPAxis.MaximumAcceleration);
                float maximumAccelerationJerk = Convert.ToSingle(axisVirtual.CIPAxis.MaximumAccelerationJerk);

                if (maximumDeceleration == 0)
                {
                    message =
                        $"Warning: {tag.Name}: Axis Maximum Deceleration is set to 0. Motion cannot be started on the axis.";
                    outputService?.AddWarnings(message, tag, null, null, Destination.ToAxisProperty);
                }

                if (maximumSpeed == 0 || maximumDeceleration == 0 || maximumDecelerationJerk == 0)
                {
                    message =
                        $"Warning: {tag.Name}: Axis Maximum Speed, Maximum Deceleration, or Maximum Deceleration Jerk is set to 0.  S-curve motion cannot be started on the axis.";
                    outputService?.AddWarnings(message, tag, null, null, Destination.ToAxisProperty);
                }

                if (maximumAcceleration == 0 || maximumAccelerationJerk == 0)
                {
                    message = $"Warning: {tag.Name}: Axis Maximum Acceleration Jerk is set to 0% of Max Accel Time.";
                    outputService?.AddWarnings(message, tag, null, null, Destination.ToAxisProperty);
                }
            }
        }

        //Check the project path length
        public void VerifyToolchain()
        {
            var outputService = Package.GetGlobalService(typeof(SErrorOutputService)) as IErrorOutputService;
            string toolchainPath = Environment.GetEnvironmentVariable("ICON_TOOLCHAIN_PATH");

            var root = AssemblyUtils.AssemblyDirectory;
            Debug.WriteLine(root.Length);

            if (string.IsNullOrEmpty(toolchainPath)) toolchainPath = $"{root}\\Toolchains";
            Debug.WriteLine(toolchainPath.Length);

            if (toolchainPath.Length <= 123) return;

            var message = "The Ics Studio project path is too long.";
            outputService?.AddErrors(message, OrderType.None, OnlineEditType.Original, null, null, null);
        }
        #endregion

    }
}
