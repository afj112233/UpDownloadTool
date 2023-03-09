using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.UIServicesPackage.ImportConfiguration.CommonPanel;
using ICSStudio.UIServicesPackage.ImportConfiguration.Dialog;
using ICSStudio.UIServicesPackage.ImportConfiguration.Panel;
using ICSStudio.UIServicesPackage.ImportConfiguration.MultiplePanel;
using ICSStudio.UIServicesPackage.ImportConfiguration.Panel.Validate;
using ICSStudio.UIServicesPackage.Services;
using Newtonsoft.Json.Linq;
using UserControl = System.Windows.Controls.UserControl;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.Instruction;
using ICSStudio.UIServicesPackage.ImportConfiguration.Panel.Rung;
using ICSStudio.Utils;

namespace ICSStudio.UIServicesPackage.ImportConfiguration
{
    public class ImportDialogViewModel : ViewModelBase
    {
        private string _panelContentTitle;
        private object _panelContent;
        private bool? _dialogResult;
        private string _title;


        public ImportDialogViewModel(ProjectItemType type, JObject config, IBaseObject baseObject, int? startIndex = null, int? endIndex = null)
        {
            OkCommand = new RelayCommand(ExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            FindReplaceCommand = new RelayCommand(ExecuteFindReplaceCommand);
            SelectedItemChangedCommand = new RelayCommand<RoutedPropertyChangedEventArgs<object>>(ExecuteSelectedItemChangedCommand);

            ImportDialogInit(type, config, baseObject,startIndex,endIndex);
        }

        public bool? DialogResult
        {
            set { Set(ref _dialogResult, value); }
            get { return _dialogResult; }
        }
        public List<ImportTreeNode> Items { get; } = new List<ImportTreeNode>();

        public string Title
        {
            set { Set(ref _title, value); }
            get
            {
                var name = "Import Configuration";
                var text = LanguageManager.GetInstance().ConvertSpecifier(name);
                if (string.IsNullOrEmpty(text))
                {
                    return name + " - " + _title;
                }
                else
                {
                    return text + " - " + _title;
                }
            }
        }

        public string PanelContentTitle
        {
            set { Set(ref _panelContentTitle, value); }
            get { return _panelContentTitle; }
        }
        public object PanelContent
        {
            set { Set(ref _panelContent, value); }
            get { return _panelContent; }
        }

        public RelayCommand OkCommand { get; }
        public RelayCommand FindReplaceCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand<RoutedPropertyChangedEventArgs<object>> SelectedItemChangedCommand { get; }


        private void ExecuteOkCommand()
        {
            if (!VerifyItems(Items)) return;
            DialogResult = true;
        }
        private void ExecuteCancelCommand()
        {
            DialogResult = false;
        }
        private void ExecuteFindReplaceCommand()
        {
            new FindReplaceDialog().ShowDialog();
        }

        private void ExecuteSelectedItemChangedCommand(RoutedPropertyChangedEventArgs<object> e)
        {
            if(e.OldValue == null) return;
            if (((ImportTreeNode)e.OldValue).Content != null)
            {
                var error = (((UserControl)((ImportTreeNode)e.OldValue).Content).DataContext as IVerify).Error;
                if (!string.IsNullOrEmpty(error))
                {
                    ((ImportTreeNode)e.OldValue).IsSelected = true;
                    ((ImportTreeNode)e.NewValue).IsSelected = false;
                    e.Handled = true;
                    MessageBox.Show(error, "ICSStudio", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void Vm_SelectedOperationChanged(object sender, EventArgs e)
        {
            var componentRoot = Items.First().ChildNodes.First();//主操作界面所在节点;
            var vm = (ConfirmationViewModel)sender;
            foreach (var importTreeNode in componentRoot.ChildNodes)
            {
                if (importTreeNode.Content is Reference)
                {
                    var referenceViewModel = (ReferenceViewModel)((Reference)importTreeNode.Content).DataContext;

                    foreach (var item in referenceViewModel.Items.ToList())
                    {
                        if (string.IsNullOrEmpty(item.Name)) referenceViewModel.Items.Remove(item);
                    }
                    if (componentRoot.TreeNodeType == ImportTreeNode.NodeType.Program)
                    {
                        var beOverwriteProgram = Controller.GetInstance().Programs[vm.FinalName];
                        if (beOverwriteProgram != null)
                        {
                            if (importTreeNode.TreeNodeType == ImportTreeNode.NodeType.ParametersAndLocalTags)
                            {
                                var parametersTags = referenceViewModel.Items.Select(r => r.Name).ToList();
                                foreach (var item in beOverwriteProgram.Tags)
                                {
                                    if (!parametersTags.Contains(item.Name)) referenceViewModel.Items.Add(new ReferenceItem(item));
                                }
                            }

                            if (importTreeNode.TreeNodeType == ImportTreeNode.NodeType.Routines)
                            {
                                var routines = referenceViewModel.Items.Select(r => r.Name).ToList();
                                foreach (var item in beOverwriteProgram.Routines)
                                    if (!routines.Contains(item.Name)) referenceViewModel.Items.Add(new ReferenceItem(item));
                            }
                        }
                    }

                    foreach (var item in referenceViewModel.Items.ToList())
                    {
                        var referenceItem = item as ReferenceItem;
                        if(referenceItem == null) continue;
                        if ("Overwrite".Equals(vm.SelectedOperation))
                        {
                            if (!string.IsNullOrEmpty(referenceItem.Name))
                            {
                                if (referenceItem.ProjectItemType == ProjectItemType.AddOnDefinedTags)
                                {
                                    if ("EnableIn".Equals(referenceItem.Name) || "EnableOut".Equals(referenceItem.Name))
                                        referenceItem.Operation = "Overwrite";
                                    else
                                        referenceItem.Operation = "Create";
                                }
                                else
                                {
                                    if (componentRoot.TreeNodeType == ImportTreeNode.NodeType.Program)
                                    {
                                        var beOverwriteProgram = Controller.GetInstance().Programs[vm.FinalName];
                                        if (importTreeNode.TreeNodeType == ImportTreeNode.NodeType.ParametersAndLocalTags)
                                        {
                                            if (beOverwriteProgram != null)
                                            {
                                                var localTag = beOverwriteProgram.Tags[referenceItem.FinalName];
                                                referenceItem.Operation = localTag != null ? "Overwrite" : "Create";
                                            }
                                        }
                                        if (importTreeNode.TreeNodeType == ImportTreeNode.NodeType.Routines)
                                        {
                                            if (beOverwriteProgram != null)
                                            {
                                                var routine = beOverwriteProgram.Routines[referenceItem.FinalName];
                                                referenceItem.Operation = routine != null ? "Overwrite":"Create";
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                referenceItem.Operation = "Delete";
                            }
                        }
                        else if ("Use Existing".Equals(vm.SelectedOperation))
                        {
                            if (!string.IsNullOrEmpty(referenceItem.Name))
                            {
                                if (referenceItem.ProjectItemType == ProjectItemType.AddOnDefinedTags)
                                {
                                    if ("EnableIn".Equals(referenceItem.Name) || "EnableOut".Equals(referenceItem.Name))
                                        referenceItem.Operation = "Use Existing";
                                    else
                                        referenceItem.Operation = "Delete";
                                }
                                else
                                {
                                    referenceItem.Operation = "Use Existing";
                                }
                            }
                            else
                            {
                                referenceItem.Operation = "Use Existing";
                            }
                        }
                        else if ("Create".Equals(vm.SelectedOperation))
                        {
                            referenceItem.Operation = "Create";
                        }
                        else if ("Discard".Equals(vm.SelectedOperation))
                        {
                            referenceItem.Operation = "Discard";
                        }
                        else
                        {
                            Debug.Assert(false, "The selected operation has an error!");
                        }
                    }
                }
            }
        }

        private void Node_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                var node = (ImportTreeNode)sender;
                if (node.IsSelected)
                {
                    PanelContent = node.Content;
                    PanelContentTitle = node.ContentTitle;
                }
            }
        }

        public void FinalizeImport()
        {
            FinalizeImport(Items);
        }

        private void FinalizeImport(List<ImportTreeNode> nodes)
        {
            if (nodes.Count == 0) return;
            foreach (var node in nodes)
            {
                FinalizeImport(node.ChildNodes);
                var confirmation = node.Content as Confirmation;
                if (confirmation != null) (confirmation.DataContext as ConfirmationViewModel)?.CollectionChangedName();

                var reference = node.Content as Reference;
                if (reference != null) (reference.DataContext as ReferenceViewModel)?.CollectionChangedName();
            }
        }

        private bool VerifyItems(List<ImportTreeNode> items)
        {
            foreach (var importTreeNode in items)
            {
                if (importTreeNode.Content == null)
                {
                    if (!VerifyItems(importTreeNode.ChildNodes)) return false;
                    continue;
                }

                var verify = (IVerify)((UserControl)importTreeNode.Content).DataContext;
                Debug.Assert(verify != null);
                if (!string.IsNullOrEmpty(verify.Error))
                {
                    MessageBox.Show(verify.Error, "ICSStudio", MessageBoxButton.OK, MessageBoxImage.Information);
                    return false;
                }

                if (!VerifyItems(importTreeNode.ChildNodes)) return false;
            }

            return true;
        }

        private void ImportDialogInit(ProjectItemType type, JObject config, IBaseObject baseObject, int? startIndex = null, int? endIndex = null)
        {
            var controller = Controller.GetInstance();
            if (type == ProjectItemType.UserDefined || type == ProjectItemType.Strings)
                ImportUserDefinedOrStrings(type, config, baseObject);
            else if (type == ProjectItemType.AddOnInstructions)
                ImportAddOnInstructions(controller, type, config, baseObject);
            else if (type == ProjectItemType.Task)
                ImportProgram(controller, type, config, baseObject);
            else if (type == ProjectItemType.Program)
                ImportRoutine(type, config, baseObject);
            else if (type == ProjectItemType.Routine && startIndex != null && endIndex != null)
                ImportRLLRung(config, baseObject, (int)startIndex, (int)endIndex);
            else if (type == ProjectItemType.Bus || type == ProjectItemType.Ethernet)
                ImportBusOrEthernet(type, config, baseObject);

            var errorsOrWarnings = new ImportTreeNode(ImportTreeNode.NodeType.ErrorsWarnings, "Errors/Warnings", false);
            var errorsOrWarningsPanel = new ErrorsOrWarningsPanel();
            var errorsOrWarningsVm = new ErrorsOrWarningsViewModel();
            errorsOrWarningsPanel.DataContext = errorsOrWarningsVm;
            errorsOrWarnings.Content = errorsOrWarningsPanel;
            PropertyChangedEventManager.AddHandler(errorsOrWarnings, Node_PropertyChanged, "");
            Items.Add(errorsOrWarnings);

            var componentRoot = Items.First().ChildNodes.First();
            ((componentRoot.Content as Confirmation)?.DataContext as ConfirmationViewModel)?.Verify();
        }

        private void ImportUserDefinedOrStrings(ProjectItemType type, JObject config, IBaseObject baseObject)
        {
            var dataTypes = new ImportTreeNode(ImportTreeNode.NodeType.Folder, "Data Types",false);
            Items.Add(dataTypes);

            var importUserDefinedOrStringsConfig = config["DataTypes"];
            var targets = new List<JToken>();
            foreach (var item in importUserDefinedOrStringsConfig)
                if ("Target".Equals(item["Use"]?.ToString()))
                    targets.Add(item);
            if (targets.Count > 1)
            {
                var multipleUserDefinedOrStrings =
                    new ImportTreeNode(ImportTreeNode.NodeType.DataType, "Multiple Data Types");
                var multipleUserDefinedOrStringsPanel = new MultipleItemsPanel();
                var multipleUserDefinedOrStringsVm =
                    new MultipleItemsViewModel(type, (JArray)importUserDefinedOrStringsConfig, baseObject);
                multipleUserDefinedOrStringsPanel.DataContext = multipleUserDefinedOrStringsVm;
                multipleUserDefinedOrStrings.Content = multipleUserDefinedOrStringsPanel;
                PropertyChangedEventManager.AddHandler(multipleUserDefinedOrStrings, Node_PropertyChanged, "");
                dataTypes.ChildNodes.Add(multipleUserDefinedOrStrings);
                multipleUserDefinedOrStrings.IsSelected = true;

                var references = new ImportTreeNode(ImportTreeNode.NodeType.Folder, "References", false);
                multipleUserDefinedOrStrings.ChildNodes.Add(references);

                var childDataTypes = new ImportTreeNode(ImportTreeNode.NodeType.DataTypes);
                var childDataTypesPanel = new DataListPanel();
                childDataTypesPanel.DataContext = multipleUserDefinedOrStringsVm;
                childDataTypes.Content = childDataTypesPanel;
                PropertyChangedEventManager.AddHandler(childDataTypes, Node_PropertyChanged, "");
                references.ChildNodes.Add(childDataTypes);
            }
            else
            {
                var target = ProjectInfoService.GetTarget(config, type);
                var content = new Confirmation();
                var vm = new ConfirmationViewModel(type, target, baseObject);
                vm.SelectedOperationChanged += Vm_SelectedOperationChanged;
                content.DataContext = vm;
                var dataType = new ImportTreeNode(ImportTreeNode.NodeType.DataType, $"{target["Name"]}");
                dataType.Content = content;
                PropertyChangedEventManager.AddHandler(dataType, Node_PropertyChanged, "");
                dataTypes.ChildNodes.Add(dataType);
                dataType.IsSelected = true;

                var reference = new ImportTreeNode(ImportTreeNode.NodeType.Folder, "Reference", false);

                var aoiConfigs = config["AddOnInstructionDefinitions"] as JArray;
                if (aoiConfigs?.Count > 0)
                {
                    var aoiReferenceVM = new ReferenceViewModel(aoiConfigs.ToList(), null, ProjectItemType.AddOnDefined,
                        baseObject);
                    var aoiReferencePanel = new Reference();
                    aoiReferencePanel.DataContext = aoiReferenceVM;
                    var addOnInstructions = new ImportTreeNode(ImportTreeNode.NodeType.AddOnInstructions);
                    addOnInstructions.Content = aoiReferencePanel;
                    reference.ChildNodes.Add(addOnInstructions);
                    PropertyChangedEventManager.AddHandler(addOnInstructions, Node_PropertyChanged, "");
                }

                var dataTypeConfigs = config["DataTypes"] as JArray;
                if (dataTypeConfigs?.Count > 1)
                {
                    var dataTypeReferenceVM = new ReferenceViewModel(dataTypeConfigs.ToList(), target,
                        ProjectItemType.UserDefined, baseObject);
                    var dataTypeReferencePanel = new Reference();
                    dataTypeReferencePanel.DataContext = dataTypeReferenceVM;
                    var attachDataType = new ImportTreeNode(ImportTreeNode.NodeType.DataTypes);
                    attachDataType.Content = dataTypeReferencePanel;
                    reference.ChildNodes.Add(attachDataType);
                    dataType.IsSelected = true;
                    PropertyChangedEventManager.AddHandler(attachDataType, Node_PropertyChanged, "");
                }
                if (reference.ChildNodes.Count > 0) dataType.ChildNodes.Add(reference);
            }
        }

        private void ImportAddOnInstructions(IController controller, ProjectItemType type, JObject config,
            IBaseObject baseObject)
        {
            var addOnInstructions = new ImportTreeNode(ImportTreeNode.NodeType.Folder, "UD Function Block", false);
            Items.Add(addOnInstructions);

            var importAoisConfig = config["AddOnInstructionDefinitions"];
            var targets = new List<JToken>();
            foreach (var item in importAoisConfig)
                if ("Target".Equals(item["Use"]?.ToString()))
                    targets.Add(item);
            if (targets.Count > 1)
            {
                var multipleInstructions =
                    new ImportTreeNode(ImportTreeNode.NodeType.AddOnInstruction, "Multiple Instructions");
                var multipleInstructionsPanel = new MultipleItemsPanel();
                var multipleInstructionsVm = new MultipleItemsViewModel(type, (JArray)importAoisConfig, baseObject);
                multipleInstructionsPanel.DataContext = multipleInstructionsVm;
                multipleInstructions.Content = multipleInstructionsPanel;
                PropertyChangedEventManager.AddHandler(multipleInstructions, Node_PropertyChanged, "");
                addOnInstructions.ChildNodes.Add(multipleInstructions);
                multipleInstructions.IsSelected = true;

                var references = new ImportTreeNode(ImportTreeNode.NodeType.Folder, "References", false);
                multipleInstructions.ChildNodes.Add(references);

                var childAddOnInstructions = new ImportTreeNode(ImportTreeNode.NodeType.AddOnInstructions);
                var childAddOnInstructionsPanel = new DataListPanel();
                childAddOnInstructionsPanel.DataContext = multipleInstructionsVm;
                childAddOnInstructions.Content = childAddOnInstructionsPanel;
                PropertyChangedEventManager.AddHandler(childAddOnInstructions, Node_PropertyChanged, "");
                references.ChildNodes.Add(childAddOnInstructions);
            }
            else
            {
                var target = ProjectInfoService.GetTarget(config, type);
                baseObject =
                    ((AoiDefinitionCollection)controller.AOIDefinitionCollection).Find((string)target["Name"]);
                var aoi = new ImportTreeNode(ImportTreeNode.NodeType.AddOnInstruction, $"{target["Name"]}");
                var content = new Confirmation();
                var vm = new ConfirmationViewModel(ProjectItemType.AddOnDefined, target, baseObject);
                vm.SelectedOperationChanged += Vm_SelectedOperationChanged;
                content.DataContext = vm;
                aoi.Content = content;
                PropertyChangedEventManager.AddHandler(aoi, Node_PropertyChanged, "");
                addOnInstructions.ChildNodes.Add(aoi);
                aoi.IsSelected = true;

                var paramsAndLocal = new ImportTreeNode(ImportTreeNode.NodeType.ParametersAndLocalTags);
                aoi.ChildNodes.Add(paramsAndLocal);
                var tags = new List<JToken>();
                foreach (var item in target["Parameters"]) tags.Add(item);

                foreach (var item in target["LocalTags"])
                {
                    item["Usage"] = (byte)Usage.Local;
                    tags.Add(item);
                }

                var paramAndLocalReferenceVM =
                    new ReferenceViewModel(tags, null, ProjectItemType.AddOnDefinedTags, baseObject, true);
                var paramAndLocalReferencePanel = new Reference();
                paramsAndLocal.Content = paramAndLocalReferencePanel;
                paramAndLocalReferencePanel.DataContext = paramAndLocalReferenceVM;
                PropertyChangedEventManager.AddHandler(paramsAndLocal, Node_PropertyChanged, "");

                var routines = new ImportTreeNode(ImportTreeNode.NodeType.Routines);
                var routinesReferenceVM = new ReferenceViewModel((target["Routines"] as JArray).ToList(), null,
                    ProjectItemType.Routine, baseObject, true);
                var routinesReferencePanel = new Reference();
                routinesReferencePanel.DataContext = routinesReferenceVM;
                routines.Content = routinesReferencePanel;
                PropertyChangedEventManager.AddHandler(routines, Node_PropertyChanged, "");
                aoi.ChildNodes.Add(routines);

                var references = new ImportTreeNode(ImportTreeNode.NodeType.Folder, "References", false);
                aoi.ChildNodes.Add(references);

                var referenceAoi = (config["AddOnInstructionDefinitions"] as JArray).ToList();
                if (referenceAoi.Count > 1)
                {
                    var aois = new ImportTreeNode(ImportTreeNode.NodeType.AddOnInstructions);
                    var aoisReferenceVM =
                        new ReferenceViewModel(referenceAoi, target, ProjectItemType.AddOnDefined, null);
                    var aoisReferencePanel = new Reference();
                    aoisReferencePanel.DataContext = aoisReferenceVM;
                    aois.Content = aoisReferencePanel;
                    PropertyChangedEventManager.AddHandler(aois, Node_PropertyChanged, "");
                    references.ChildNodes.Add(aois);
                }

                var referenceDataTypes = (config["DataTypes"] as JArray).ToList();
                if (referenceDataTypes.Count > 0)
                {
                    var dataTypes = new ImportTreeNode(ImportTreeNode.NodeType.DataTypes);
                    var dataTypesReferenceVM =
                        new ReferenceViewModel(referenceDataTypes, null, ProjectItemType.UserDefined, null);
                    var dataTypesReferencePanel = new Reference();
                    dataTypesReferencePanel.DataContext = dataTypesReferenceVM;
                    dataTypes.Content = dataTypesReferencePanel;
                    PropertyChangedEventManager.AddHandler(dataTypes, Node_PropertyChanged, "");
                    references.ChildNodes.Add(dataTypes);
                }
            }
        }

        private void ImportProgram(IController controller, ProjectItemType type, JObject config, IBaseObject baseObject)
        {
            var programs = new ImportTreeNode(ImportTreeNode.NodeType.Folder, "Programs", false);
            Items.Add(programs);

            var importProgramsConfig = config["Programs"];
            var targets = new List<JToken>();
            foreach (var item in importProgramsConfig)
                if ("Target".Equals(item["Use"]?.ToString()))
                    targets.Add(item);

            if (targets.Count > 1000)
                throw new ICSStudioException("Failed to import file.\nThe number of imported components exceeds the maximum allowed components");

            if (targets.Count > 1)
            {
                //throw new ICSStudioException("The batch import function of the program is being improved, please wait for the next update.");
                var multiplePrograms = new ImportTreeNode(ImportTreeNode.NodeType.Program,
                    LanguageManager.GetInstance().ConvertSpecifier("Multiple Programs")+$"({importProgramsConfig.Count()})");
                var multipleProgramsPanel = new MultipleItemsPanel();

                var multipleProgramsVm =
                    new MultipleItemsViewModel(ProjectItemType.Program, (JArray)importProgramsConfig, baseObject);
                multipleProgramsPanel.DataContext = multipleProgramsVm;
                multiplePrograms.Content = multipleProgramsPanel;
                PropertyChangedEventManager.AddHandler(multiplePrograms, Node_PropertyChanged, "");
                programs.ChildNodes.Add(multiplePrograms);
                multiplePrograms.IsSelected = true;

                var childPrograms = new ImportTreeNode(ImportTreeNode.NodeType.Programs);
                var childProgramsPanel = new DataListPanel();
                childProgramsPanel.DataContext = multipleProgramsVm;
                childPrograms.Content = childProgramsPanel;
                PropertyChangedEventManager.AddHandler(childPrograms, Node_PropertyChanged, "");
                multiplePrograms.ChildNodes.Add(childPrograms);
            }
            else
            {
                var target = ProjectInfoService.GetTarget(config, type);
                var existProgram = controller.Programs[target["Name"]?.ToString()];
                var program = new ImportTreeNode(ImportTreeNode.NodeType.Program, $"{target["Name"]}");
                var content = new Confirmation();
                var vm = new ConfirmationViewModel(ProjectItemType.Program, target, baseObject);
                vm.SelectedOperationChanged += Vm_SelectedOperationChanged;
                content.DataContext = vm;
                program.Content = content;
                PropertyChangedEventManager.AddHandler(program, Node_PropertyChanged, "");
                programs.ChildNodes.Add(program);
                program.IsSelected = true;

                var parameters = new ImportTreeNode(ImportTreeNode.NodeType.ParametersAndLocalTags);
                program.ChildNodes.Add(parameters);
                var paramReferenceVM = new ReferenceViewModel((target["Tags"] as JArray).ToList(), null,
                    ProjectItemType.ProgramTags, existProgram, true);
                var paramReferencePanel = new Reference();
                paramReferencePanel.DataContext = paramReferenceVM;
                parameters.Content = paramReferencePanel;
                PropertyChangedEventManager.AddHandler(parameters, Node_PropertyChanged, "");

                var routines = new ImportTreeNode(ImportTreeNode.NodeType.Routines);
                program.ChildNodes.Add(routines);
                var routinesReferenceVM = new ReferenceViewModel((target["Routines"] as JArray).ToList(), null,
                    ProjectItemType.Routine, existProgram, true);
                var routinesReferencePanel = new Reference();
                routinesReferencePanel.DataContext = routinesReferenceVM;
                routines.Content = routinesReferencePanel;
                PropertyChangedEventManager.AddHandler(routines, Node_PropertyChanged, "");

                var otherPrograms = new ImportTreeNode(ImportTreeNode.NodeType.Programs);
                program.ChildNodes.Add(otherPrograms);
                var otherProgramsReferenceVM = new ReferenceViewModel(null, null, ProjectItemType.Program, null, true);
                var otherProgramReferencePanel = new Reference();
                otherProgramReferencePanel.DataContext = otherProgramsReferenceVM;
                otherPrograms.Content = otherProgramReferencePanel;
                PropertyChangedEventManager.AddHandler(otherPrograms, Node_PropertyChanged, "");

                var references = new ImportTreeNode(ImportTreeNode.NodeType.Folder, "References", false);

                var referenceTags = new List<JToken>();
                foreach (var tag in config["Tags"]) referenceTags.Add(tag);

                foreach (var programC in config["Programs"])
                {
                    if (programC == target) continue;
                    //var referenceProgram = controller.Programs[programC["Name"]?.ToString()];
                    foreach (var tag in programC["Tags"])
                    {
                        tag["Name"] = $"\\{programC["Name"]}.{tag["Name"]}";
                        referenceTags.Add(tag);
                    }
                }

                if (referenceTags.Count > 0)
                {
                    var tags = new ImportTreeNode(ImportTreeNode.NodeType.Tags);
                    references.ChildNodes.Add(tags);
                    var tagsReferenceVM =
                        new ReferenceViewModel(referenceTags, null, ProjectItemType.ControllerTags, null);
                    var tagsReferencePanel = new Reference();
                    tagsReferencePanel.DataContext = tagsReferenceVM;
                    tags.Content = tagsReferencePanel;
                    PropertyChangedEventManager.AddHandler(tags, Node_PropertyChanged, "");
                }


                var referenceAoi = (config["AddOnInstructionDefinitions"] as JArray).ToList();
                if (referenceAoi.Count > 0)
                {
                    var aoi = new ImportTreeNode(ImportTreeNode.NodeType.AddOnInstructions);
                    references.ChildNodes.Add(aoi);
                    var aoiReferenceVM = new ReferenceViewModel(referenceAoi
                        , null, ProjectItemType.AddOnDefined,
                        null);
                    var aoiReferencePanel = new Reference();
                    aoiReferencePanel.DataContext = aoiReferenceVM;
                    aoi.Content = aoiReferencePanel;
                    PropertyChangedEventManager.AddHandler(aoi, Node_PropertyChanged, "");
                }

                var referenceDataType = (config["DataTypes"] as JArray).ToList();
                if (referenceDataType.Count > 0)
                {
                    var dataTypes = new ImportTreeNode(ImportTreeNode.NodeType.DataTypes);
                    references.ChildNodes.Add(dataTypes);
                    var dataTypesReferenceVM =
                        new ReferenceViewModel(referenceDataType, null, ProjectItemType.UserDefined, null);
                    var dataTypesReferencePanel = new Reference();
                    dataTypesReferencePanel.DataContext = dataTypesReferenceVM;
                    dataTypes.Content = dataTypesReferencePanel;
                    PropertyChangedEventManager.AddHandler(dataTypes, Node_PropertyChanged, "");
                }

                if(references.ChildNodes.Count>0) program.ChildNodes.Add(references);
            }

            var connections = new ImportTreeNode(ImportTreeNode.NodeType.Connections, "Connections", false);
            var connectionsPanel = new ConnectionsPanel();
            var connectionsVm = new ConnectionsViewModel();
            connectionsPanel.DataContext = connectionsVm;
            connections.Content = connectionsPanel;
            PropertyChangedEventManager.AddHandler(connections, Node_PropertyChanged, "");
            Items.Add(connections);
        }

        private void ImportRoutine(ProjectItemType type, JObject config, IBaseObject baseObject)
        {
            var programs = new ImportTreeNode(ImportTreeNode.NodeType.Folder, "Programs", false);
            Items.Add(programs);

            var parentProgram = baseObject as IProgramModule;
            var program = new ImportTreeNode(ImportTreeNode.NodeType.Program, $"{parentProgram?.Name}", false);
            programs.ChildNodes.Add(program);

            var target = ProjectInfoService.GetTarget(config, type);
            var routine = new ImportTreeNode(ImportTreeNode.NodeType.STRoutine, $"{target["Name"]}");//routineType
            program.ChildNodes.Add(routine);
            var confirmation = new Confirmation();
            var confirmationVM = new ConfirmationViewModel(ProjectItemType.Routine, target, parentProgram);
            confirmationVM.SelectedOperationChanged += Vm_SelectedOperationChanged;
            confirmation.DataContext = confirmationVM;
            routine.Content = confirmation;
            PropertyChangedEventManager.AddHandler(routine, Node_PropertyChanged, "");
            routine.IsSelected = true;

            var references = new ImportTreeNode(ImportTreeNode.NodeType.Folder, "References", false);
            routine.ChildNodes.Add(references);

            var allTags = new List<JToken>();
            foreach (var tag in config["Tags"])
            {
                allTags.Add(tag);
                tag["CanChangeScope"] = false;
            }

            foreach (var p in config["Programs"])
            {
                var programName = p["Name"]?.ToString();
                if (p["Routines"]?.Contains(target) ?? false)
                    programName = parentProgram.Name;
                foreach (var t in p["Tags"])
                {
                    t["Name"] = $"\\{programName}.{t["Name"]}";
                    t["CanChangeScope"] = false;
                    allTags.Add(t);
                }
            }

            if (allTags.Count > 0)
            {
                var tags = new ImportTreeNode(ImportTreeNode.NodeType.Tags);
                var tagsReferenceVM = new ReferenceViewModel(allTags, null, ProjectItemType.ControllerTags, null);
                var tagReferencePanel = new Reference();
                tagReferencePanel.DataContext = tagsReferenceVM;
                tags.Content = tagReferencePanel;
                PropertyChangedEventManager.AddHandler(tags, Node_PropertyChanged, "");
                references.ChildNodes.Add(tags);
            }

            var referenceAoi = (config["AddOnInstructionDefinitions"] as JArray).ToList();
            if (referenceAoi.Count > 0)
            {
                var aois = new ImportTreeNode(ImportTreeNode.NodeType.AddOnInstructions);
                var aoisReferenceVM =
                    new ReferenceViewModel(referenceAoi, target, ProjectItemType.AddOnDefined, null);
                var aoisReferencePanel = new Reference();
                aoisReferencePanel.DataContext = aoisReferenceVM;
                aois.Content = aoisReferencePanel;
                PropertyChangedEventManager.AddHandler(aois, Node_PropertyChanged, "");
                references.ChildNodes.Add(aois);
            }

            var referenceDataTypes = (config["DataTypes"] as JArray).ToList();
            if (referenceDataTypes.Count > 0)
            {
                var dataTypes = new ImportTreeNode(ImportTreeNode.NodeType.DataTypes);
                var dataTypesReferenceVM =
                    new ReferenceViewModel(referenceDataTypes, null, ProjectItemType.UserDefined, null);
                var dataTypesReferencePanel = new Reference();
                dataTypesReferencePanel.DataContext = dataTypesReferenceVM;
                dataTypes.Content = dataTypesReferencePanel;
                PropertyChangedEventManager.AddHandler(dataTypes, Node_PropertyChanged, "");
                references.ChildNodes.Add(dataTypes);
            }
        }

        private void ImportRLLRung(JObject config, IBaseObject baseObject,int startIndex,int endIndex)
        {
            var rllRoutine = baseObject as RLLRoutine;
            Debug.Assert(rllRoutine != null);
            var programs = new ImportTreeNode(ImportTreeNode.NodeType.Folder, "Programs", false);
            Items.Add(programs);

            var contextProgram = config["Programs"].ToList().First();
            var program = new ImportTreeNode(ImportTreeNode.NodeType.Program, contextProgram["Name"]?.ToString(), false);
            programs.ChildNodes.Add(program);
            var contextRoutine = contextProgram["Routines"]?.ToList().First();
            if(!(contextRoutine != null && contextRoutine["Use"] != null && "Context".Equals(contextRoutine["Use"].ToString())))
                throw new Exception("The data which will be imported has an error!");
            var routine = new ImportTreeNode(ImportTreeNode.NodeType.RLLRoutine, contextRoutine?["Name"] + "(Rungs)");
            program.ChildNodes.Add(routine);
            var rungPane = new RungPane();
            rungPane.DataContext = new RungViewModel(contextProgram,rllRoutine,startIndex,endIndex);
            routine.Content = rungPane;
            PropertyChangedEventManager.AddHandler(routine, Node_PropertyChanged, "");
            routine.IsSelected = true;

            //references
            var references = new ImportTreeNode(ImportTreeNode.NodeType.Folder, "References", false);

            var allTags = new List<JToken>();
            foreach (var tag in config["Tags"])
            {
                allTags.Add(tag);
                tag["CanChangeScope"] = false;
            }

            var programName = contextProgram["Name"]?.ToString();
            if (contextProgram["Routines"]?.Contains(contextRoutine) ?? false)
                programName = (baseObject as RLLRoutine).ParentCollection.ParentProgram.Name;
            foreach (var t in contextProgram["Tags"])
            {
                t["Name"] = $"\\{programName}.{t["Name"]}";
                t["CanChangeScope"] = false;
                allTags.Add(t);
            }

            if (allTags.Count > 0)
            {
                var tags = new ImportTreeNode(ImportTreeNode.NodeType.Tags);
                var tagsReferenceVM = new ReferenceViewModel(allTags, null, ProjectItemType.ControllerTags, null);
                var tagReferencePanel = new Reference();
                tagReferencePanel.DataContext = tagsReferenceVM;
                tags.Content = tagReferencePanel;
                PropertyChangedEventManager.AddHandler(tags, Node_PropertyChanged, "");
                references.ChildNodes.Add(tags);
            }

            var referenceAoi = (config["AddOnInstructionDefinitions"] as JArray).ToList();
            if (referenceAoi.Count > 0)
            {
                var aois = new ImportTreeNode(ImportTreeNode.NodeType.AddOnInstructions);
                var aoisReferenceVM = 
                    new ReferenceViewModel(referenceAoi, null, ProjectItemType.AddOnDefined, null);
                var aoisReferencePanel = new Reference();
                aoisReferencePanel.DataContext = aoisReferenceVM;
                aois.Content = aoisReferencePanel;
                PropertyChangedEventManager.AddHandler(aois, Node_PropertyChanged, "");
                references.ChildNodes.Add(aois);
            }

            var referenceDataTypes = (config["DataTypes"] as JArray).ToList();
            if (referenceDataTypes.Count > 0)
            {
                var dataTypes = new ImportTreeNode(ImportTreeNode.NodeType.DataTypes);
                var dataTypesReferenceVM =
                    new ReferenceViewModel(referenceDataTypes, null, ProjectItemType.UserDefined, null);
                var dataTypesReferencePanel = new Reference();
                dataTypesReferencePanel.DataContext = dataTypesReferenceVM;
                dataTypes.Content = dataTypesReferencePanel;
                PropertyChangedEventManager.AddHandler(dataTypes, Node_PropertyChanged, "");
                references.ChildNodes.Add(dataTypes);
            }

            //otherComponents
            var otherComponents = new List<Tuple<string,string>>();
            var codeText = new List<string>();
            if(contextRoutine["Rungs"] == null) return;
            foreach (var item in contextRoutine["Rungs"])
            {
                codeText.Add(Convert.ToString(item["Text"]));
            }

            var ast = RLLGrammarParser.Parse(codeText, Controller.GetInstance());
            var dictionary = new Dictionary<string, Dictionary<string, int>>();
            foreach (var item in ast.list.nodes)
            {
                var sequence = item as ASTRLLSequence;
                if(sequence == null) continue;
                foreach (var element in sequence.list.nodes)
                {
                    var instruction = element as ASTRLLInstruction;
                    if (instruction != null)
                    {
                        var name = string.Empty;
                        var @class = string.Empty;
                        if ("SSV".Equals(instruction.name, StringComparison.OrdinalIgnoreCase)
                            || "GSV".Equals(instruction.name, StringComparison.OrdinalIgnoreCase))
                        {
                            name = instruction.param_list[1];
                            @class = instruction.param_list[0];
                        }
                        else if ("JSR".Equals(instruction.name, StringComparison.OrdinalIgnoreCase)
                                 || "FOR".Equals(instruction.name, StringComparison.OrdinalIgnoreCase))
                        {
                            name = instruction.param_list[0];
                            @class = "Routine";
                        }

                        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(@class)) continue;

                        if (!(dictionary.ContainsKey(name) && dictionary[name].ContainsKey(@class)))
                        {
                            otherComponents.Add(new Tuple<string, string>(name, @class));

                            if (!dictionary.ContainsKey(name))
                            {
                                dictionary.Add(name, new Dictionary<string, int>() { { @class, 1 } });
                            }
                            else
                            {
                                dictionary[name].Add(@class, 1);
                            }
                        }
                    }
                }
            }
            var otherComponentsNode = new ImportTreeNode(ImportTreeNode.NodeType.OtherComponents);
            var vm = new ReferenceViewModel(otherComponents);
            var view = new Reference();
            view.DataContext = vm;
            otherComponentsNode.Content = view;
            PropertyChangedEventManager.AddHandler(otherComponentsNode, Node_PropertyChanged, "");
            references.ChildNodes.Add(otherComponentsNode);

            if (references.ChildNodes.Count > 0)
                routine.ChildNodes.Add(references);
        }

        private void ImportBusOrEthernet(ProjectItemType type, JObject config, IBaseObject baseObject)
        {
            var parentModule = baseObject as DeviceModule;
            Debug.Assert(parentModule != null, baseObject.GetType().Name);
            var parent = new ImportTreeNode(ImportTreeNode.NodeType.Module, $"{parentModule.Name}", false);
            Items.Add(parent);

            var target = ProjectInfoService.GetTarget(config, type);
            var importModule = new ImportTreeNode(ImportTreeNode.NodeType.Module, $"{target["Name"]}");
            parent.ChildNodes.Add(importModule);
            var confirmation = new Confirmation();
            var confirmationVM = new ConfirmationViewModel(ProjectItemType.ModuleDefined, target, baseObject);
            confirmationVM.SelectedOperationChanged += Vm_SelectedOperationChanged;
            confirmation.DataContext = confirmationVM;
            importModule.Content = confirmation;
            PropertyChangedEventManager.AddHandler(importModule, Node_PropertyChanged, "");
            importModule.IsSelected = true;

            var childModules = new ImportTreeNode(ImportTreeNode.NodeType.Module, "Child Modules");
            importModule.ChildNodes.Add(childModules);
            var childModulesReferenceVM = new ReferenceViewModel((config["Modules"] as JArray).ToList(), target,
                ProjectItemType.ModuleDefined, null);
            var childModulesReferencePanel = new Reference();
            childModulesReferencePanel.DataContext = childModulesReferenceVM;
            childModules.Content = childModulesReferencePanel;
            PropertyChangedEventManager.AddHandler(childModules, Node_PropertyChanged, "");
        }
    }

    public class ImportTreeNode : ViewModelBase
    {
        private const string FolderOpenImage = "./images/Folder_Open.png";
        private const string FolderCloseImage = "./images/Folder_Close.png";
        private const string ProgramImage = "./images/Program.png";
        private const string TagImage = "./images/Tag.png";
        private const string STImage = "./images/Routine_ST.png";
        private const string RLLImage = "./images/Routine_LD.png";
        private const string ErrorsWarningsImage = "./images/Error.png";
        private const string ConnectionsImage = "./images/connections.png";
        private const string AOIImage = "./images/AOI.png";
        private const string DataTypeImage = "./images/DataType.png";
        private const string ModuleImage = "./images/Eip.png";

        private bool _isSelected;
        private readonly string _imageSource = string.Empty;
        private bool _isExpanded = true;
        private readonly string _contentTitle = string.Empty;
        private readonly string _header;

        public ImportTreeNode(NodeType nodeType = NodeType.Folder,string header = "",bool isEnabled = true)
        {
            TreeNodeType = nodeType;
            _header = header;
            IsEnabled = isEnabled;
            switch (nodeType)
            {
                case NodeType.Program:
                    _imageSource = ProgramImage;
                    _contentTitle = "Program";
                    break;
                case NodeType.STRoutine:
                    _imageSource = STImage;
                    _contentTitle = "Routine";
                    break;
                case NodeType.RLLRoutine:
                    _imageSource = RLLImage;
                    _contentTitle = "Routine";
                    break;
                case NodeType.Rung:
                    _imageSource = RLLImage;
                    _contentTitle = "Rung";
                    break;
                case NodeType.AddOnInstruction:
                    _imageSource = AOIImage;
                    _contentTitle = "Add-On Instruction";
                    break;
                case NodeType.DataType:
                    _imageSource = DataTypeImage;
                    _contentTitle = "Data Type";
                    break;
                case NodeType.Module:
                    _imageSource = ModuleImage;
                    _contentTitle = "Module";
                    break;
            }

            if (!string.IsNullOrEmpty(_contentTitle))
            {
                _contentTitle = "Configure" + " " + _contentTitle + " " + "Properties";
            }
            else
            {
                switch (nodeType)
                {
                    case NodeType.ParametersAndLocalTags:
                        _imageSource = TagImage;
                        _contentTitle = "Parameters and Local Variable";
                        break;
                    case NodeType.Routines:
                        _imageSource = STImage;
                        _contentTitle = "Routines";
                        break;
                    case NodeType.Programs:
                        _imageSource = ProgramImage;
                        _contentTitle = "Programs";
                        break;
                    case NodeType.Tags:
                        _imageSource = TagImage;
                        _contentTitle = "Variable";
                        break;
                    case NodeType.AddOnInstructions:
                        _imageSource = AOIImage;
                        _contentTitle = "Add-On Instructions";
                        break;
                    case NodeType.DataTypes:
                        _imageSource = DataTypeImage;
                        _contentTitle = "Data Types";
                        break;
                    case NodeType.OtherComponents:
                        _imageSource = ModuleImage;//other components;
                        _contentTitle = "Other Components";
                        break;
                    case NodeType.ErrorsWarnings:
                        _imageSource = ErrorsWarningsImage;
                        _contentTitle = "Errors/Warnings";
                        break;
                }

                if (string.IsNullOrEmpty(Header) && !string.IsNullOrEmpty(_contentTitle)) _header = _contentTitle;
            }

            if (string.IsNullOrEmpty(_contentTitle) && nodeType == NodeType.Connections)
            {
                _contentTitle = "Configure" + " " + "Connections";
                _imageSource = ConnectionsImage;
                if (string.IsNullOrEmpty(Header)) _header = "Connections";
            }
        }

        public NodeType TreeNodeType { get; }

        public string Header
        {
            get
            {
                var text = LanguageManager.GetInstance().ConvertSpecifier(_header);
                return string.IsNullOrEmpty(text)? _header:text;
            }
        }

        public string ImageSource
        {
            get
            {
                if (TreeNodeType == NodeType.Folder)
                {
                    return IsExpanded ? FolderOpenImage : FolderCloseImage;
                }
                return _imageSource;
            }
        }
        public List<ImportTreeNode> ChildNodes { get; } = new List<ImportTreeNode>();

        public bool IsSelected
        {
            set
            {
                if (IsEnabled) Set(ref _isSelected, value);
            }
            get { return _isSelected; }
        }

        public bool IsEnabled { set; get; } = true;

        public bool IsExpanded
        {
            set
            {
                if (IsEnabled)
                {
                    Set(ref _isExpanded, value);
                    if (TreeNodeType == NodeType.Folder) RaisePropertyChanged(nameof(ImageSource));
                }
            }
            get { return _isExpanded; }
        }

        public object Content { set; get; }

        public string ContentTitle
        {
            get
            {
                var title = LanguageManager.GetInstance().ConvertSpecifier(_contentTitle);
                return string.IsNullOrEmpty(title) ? _contentTitle : title;
            }
        }

        public enum NodeType
        {
            None,
            Folder,
            STRoutine,
            RLLRoutine,
            Program,
            DataType,
            AddOnInstruction,
            Rung,
            Module,
            ParametersAndLocalTags,
            Routines,
            Programs,
            Tags,
            AddOnInstructions,
            DataTypes,
            OtherComponents,
            Connections,
            ErrorsWarnings
        }
    }
}
