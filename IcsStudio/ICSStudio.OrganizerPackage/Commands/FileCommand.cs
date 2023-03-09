using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Xml;
using ICSStudio.Dialogs.WaitDialog;
using ICSStudio.FileConverter.JsonToL5X;
using ICSStudio.OrganizerPackage.HistoryFile;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Error;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.UIInterfaces.QuickWatch;
using ICSStudio.UIInterfaces.UI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using ICSStudio.Dialogs.Warning;
using ICSStudio.MultiLanguage;
using ICSStudio.OrganizerPackage.Utilities;
using ICSStudio.OrganizerPackage.ViewModel;
using ICSStudio.UIInterfaces.Parser;
using ICSStudio.UIInterfaces.Search;

namespace ICSStudio.OrganizerPackage.Commands
{
    internal sealed class FileCommand
    {
        private readonly Package _package;

        private FileCommand(Package package)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));

            _package = package;

            var commandService =
                ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID =
                    new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.openFileCommand);
                var menuItem = new OleMenuCommand(OpenFile, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID =
                    new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.newFileCommand);
                menuItem = new OleMenuCommand(NewFile, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID =
                    new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.saveFileCommand);
                menuItem = new OleMenuCommand(SaveFile, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);


                menuCommandID =
                    new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.saveAsFileCommand);
                menuItem = new OleMenuCommand(SaveAsFile, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID =
                    new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.buildSaveFileCommand);
                menuItem = new OleMenuCommand(BuildSaveFile, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID =
                    new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.PrintSubMenu);
                menuItem = new OleMenuCommand(PrintMenu, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printRoutine);
                menuItem = new OleMenuCommand(Print, menuCommandID);
                menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printRoutineProperties);
                menuItem = new OleMenuCommand(Print, menuCommandID);
                menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printTags);
                menuItem = new OleMenuCommand(Print, menuCommandID);
                menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet,
                    PackageIds.printSequencingAndStepTags);
                menuItem = new OleMenuCommand(Print, menuCommandID);
                menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printModuleProperties);
                menuItem = new OleMenuCommand(Print, menuCommandID);
                menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet,
                    PackageIds.printAdd_OnInstructionSignatures);
                menuItem = new OleMenuCommand(Print, menuCommandID);
                menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printAdd_OnInstruction);
                menuItem = new OleMenuCommand(Print, menuCommandID);
                menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printDataType);
                menuItem = new OleMenuCommand(Print, menuCommandID);
                menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printCrossReference);
                menuItem = new OleMenuCommand(Print, menuCommandID);
                menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printControllerOrganizer);
                menuItem = new OleMenuCommand(Print, menuCommandID);
                menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet,
                    PackageIds.printControllerProperties);
                menuItem = new OleMenuCommand(Print, menuCommandID);
                menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printTaskProperties);
                menuItem = new OleMenuCommand(Print, menuCommandID);
                menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printProgramProperties);
                menuItem = new OleMenuCommand(Print, menuCommandID);
                menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printEquipmentSequence);
                menuItem = new OleMenuCommand(Print, menuCommandID);
                menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printEntireProject);
                menuItem = new OleMenuCommand(Print, menuCommandID);
                menuItem.BeforeQueryStatus += PrintMenuOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                SetHistory(commandService);
            }
        }

        #region History

        private readonly List<string> _fileList = new List<string>();
        private readonly List<HistoryFileItemMenuCommand> _historyFile = new List<HistoryFileItemMenuCommand>();

        private void SetHistory(OleMenuCommandService commandService)
        {
            XmlDocument doc = new XmlDocument();
            if (!File.Exists(_historyPath))
            {
                var dynamicItemRootId =
                    new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.historyFileCommand);
                HistoryFileItemMenuCommand dynamicMenuCommand = new HistoryFileItemMenuCommand(OnInvokedDynamicItem,
                    OnBeforeQueryStatusDynamicItem, dynamicItemRootId, "");
                dynamicMenuCommand.Text = $"placeholder";
                commandService.AddCommand(dynamicMenuCommand);
                _historyFile.Insert(0, dynamicMenuCommand);
                return;
            }
            else
            {
                doc.Load(_historyPath);
            }

            var rootElement = doc.GetElementsByTagName("History")[0];
            if (rootElement.ChildNodes.Count > 0)
            {
                int index = 0;
                for (int i = rootElement.ChildNodes.Count; i > 0; i--)
                {
                    XmlNode childNode = rootElement.ChildNodes[i - 1];
                    var file = childNode.Attributes["path"].Value;
                    var dynamicItemRootId = new CommandID(PackageGuids.organizerPackageCmdSet,
                        PackageIds.historyFileCommand + index);
                    HistoryFileItemMenuCommand dynamicMenuCommand = new HistoryFileItemMenuCommand(OnInvokedDynamicItem,
                        OnBeforeQueryStatusDynamicItem, dynamicItemRootId, file);
                    dynamicMenuCommand.Text = $"{index + 1} {FormatFilePath(file)}";
                    commandService.AddCommand(dynamicMenuCommand);
                    _historyFile.Insert(0, dynamicMenuCommand);
                    index++;
                    _fileList.Add(file);
                }
            }
            else
            {
                var dynamicItemRootId =
                    new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.historyFileCommand);
                HistoryFileItemMenuCommand dynamicMenuCommand = new HistoryFileItemMenuCommand(OnInvokedDynamicItem,
                    OnBeforeQueryStatusDynamicItem, dynamicItemRootId, "");
                dynamicMenuCommand.Text = $"placeholder";
                commandService.AddCommand(dynamicMenuCommand);
                _historyFile.Insert(0, dynamicMenuCommand);
            }
        }

        private void OrderHistory()
        {
            Debug.Assert(_historyFile.Count == _fileList.Count);
            var orderList = _historyFile.OrderBy(h => h.CommandID.ID).ToList();
            for (int i = 0; i < _fileList.Count; i++)
            {
                var command = orderList[i];
                var file = _fileList[i];
                command.File = file;
                command.Text = $"{i + 1} {FormatFilePath(file)}";
            }
        }

        private void AddHistoryFile(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                return;
            }

            if (_fileList.Contains(file))
            {
                if (_fileList.IndexOf(file) == 0) return;
                _fileList.Remove(file);
                _fileList.Insert(0, file);
                OrderHistory();
                return;
            }

            if (_historyFile.Any())
            {
                var command = _historyFile[0];
                if (command.Text == "placeholder")
                {
                    command.File = file;
                    command.Text = $"1 {FormatFilePath(file)}";
                    command.Visible = true;
                    _fileList.Insert(0, file);
                    AddHistory(file);
                    return;
                }
            }

            var commandService =
                ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var id = _historyFile.Any() ? _historyFile[0].CommandID.ID + 1 : PackageIds.historyFileCommand;
                if (_historyFile.Count >= 8)
                {
                    var removeItem = _historyFile[_historyFile.Count - 1];
                    commandService.RemoveCommand(removeItem);
                    id = removeItem.CommandID.ID;
                    _historyFile.Remove(removeItem);
                    _fileList.RemoveAt(_fileList.Count - 1);
                    RemoveOldHistory();
                }

                var dynamicItemRootId =
                    new CommandID(PackageGuids.organizerPackageCmdSet, id);
                HistoryFileItemMenuCommand dynamicMenuCommand =
                    new HistoryFileItemMenuCommand(OnInvokedDynamicItem,
                        OnBeforeQueryStatusDynamicItem, dynamicItemRootId, file);

                dynamicMenuCommand.Text = FormatFilePath(file);
                commandService.AddCommand(dynamicMenuCommand);
                _historyFile.Insert(0, dynamicMenuCommand);
                _fileList.Insert(0, file);
                AddHistory(file);
                OrderHistory();
            }
        }

        private string FormatFilePath(string file)
        {
            var f = file;
            var index = new List<int>();
            var matchCollection = Regex.Matches(file, "\\\\");
            foreach (Match match in matchCollection)
            {
                index.Add(match.Index);
            }

            if (index.Count >= 4)
            {
                f = $"{file.Substring(0, index[1])}\\...{file.Substring(index[index.Count - 2])}";
            }
            else
            {
                f = file;
            }

            return f;
        }

        private readonly string _historyPath =
            $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\ICSStudio\\history.xml";

        private void AddHistory(string file)
        {
            XmlDocument doc = new XmlDocument();
            if (!File.Exists(_historyPath))
            {
                var root = doc.CreateElement("History");
                doc.AppendChild(root);
            }
            else
            {
                doc.Load(_historyPath);
            }

            var rootElement = doc.GetElementsByTagName("History")[0];
            var newItem = doc.CreateElement("File");

            newItem.SetAttribute("path", file);
            rootElement.AppendChild(newItem);
            if (!Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\ICSStudio"))
                Directory.CreateDirectory(
                    $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\ICSStudio");
            doc.Save(_historyPath);
        }

        private void Promote(int index)
        {
            if (index == 0) return;
            var old = _fileList[index];
            _fileList.RemoveAt(index);
            _fileList.Insert(0, old);
            OrderHistory();
            XmlDocument doc = new XmlDocument();
            doc.Load(_historyPath);
            var rootElement = doc.GetElementsByTagName("History")[0];
            index = Math.Abs(rootElement.ChildNodes.Count - index - 1);
            var item = rootElement.ChildNodes[index];
            rootElement.RemoveChild(item);
            rootElement.AppendChild(item);
            doc.Save(_historyPath);
        }

        private void RemoveOldHistory()
        {
            XmlDocument doc = new XmlDocument();
            if (!File.Exists(_historyPath))
            {
                return;
            }
            else
            {
                doc.Load(_historyPath);
            }

            var rootElement = doc.GetElementsByTagName("History")[0];
            var child = rootElement.FirstChild;
            if (child != null)
            {
                rootElement.RemoveChild(child);
                doc.Save(_historyPath);
            }
        }

        private void OnBeforeQueryStatusDynamicItem(object sender, EventArgs args)
        {
            HistoryFileItemMenuCommand matchedCommand = (HistoryFileItemMenuCommand)sender;
            matchedCommand.Enabled = true;
            matchedCommand.Visible = !matchedCommand.Text.Equals("placeholder");
            matchedCommand.Checked = false;
        }

        private void OnInvokedDynamicItem(object sender, EventArgs args)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            HistoryFileItemMenuCommand invokedCommand = (HistoryFileItemMenuCommand)sender;
            if (invokedCommand.File == Controller.GetInstance().ProjectLocaleName) return;
            if (!File.Exists(invokedCommand.File))
            {
                MessageBox.Show("File not found.", "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            var result = OpenFile(invokedCommand.File, invokedCommand.File);
            if (result < 0) return;
            Promote(_fileList.IndexOf(invokedCommand.File));
        }

        #endregion

        public static FileCommand Instance { get; private set; }

        private IServiceProvider ServiceProvider => _package;

        public static void Initialize(Package package)
        {
            Instance = new FileCommand(package);
        }

        private void NewFile(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            NewProject();
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
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            IProjectItem selectedProjectItem = null;
            if (selectedProjectItems.Count > 0)
                selectedProjectItem = selectedProjectItems[0];
            try
            {
                var task = selectedProjectItem?.AssociatedObject as CTask;
                var data = new PrintVM(menuCommand, selectedProjectItem);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        internal void NewProject()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            IProjectInfoService projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;

            string projectFile = projectInfoService?.NewFile();

            if (!string.IsNullOrEmpty(projectFile))
            {
                OpenFile(projectFile, projectFile);
            }

            AddHistoryFile(projectFile);
        }

        private void OpenFile(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            OpenProject();
        }

        internal void OpenProject()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            OpenFileDialog openDlg = new OpenFileDialog
            {
                Title = "Import file",
                Filter = "json文件(*.json)|*.json|L5X文件(*.L5X)|*.L5X"
            };

            SaveFileDialog saveDlg = new SaveFileDialog()
            {
                Title = "Save file",
                Filter = "json文件(*.json)|*.json"
            };

            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                if (openDlg.FileName.EndsWith(".L5X", true, CultureInfo.CurrentCulture))
                {
                    if (openDlg.SafeFileName != null) saveDlg.FileName = openDlg.SafeFileName.Replace(".L5X", "");
                    if (saveDlg.ShowDialog() == DialogResult.OK)
                    {
                        OpenFile(openDlg.FileName, saveDlg.FileName);
                    }
                }
                else if (openDlg.FileName.EndsWith(".json", true, CultureInfo.CurrentCulture))
                {
                    OpenFile(openDlg.FileName, openDlg.FileName);
                }

                AddHistoryFile(Controller.GetInstance().ProjectLocaleName);
            }
        }

        private int OpenFile(string openFileName, string saveFileName)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (File.GetAttributes(openFileName).ToString().IndexOf("ReadOnly", StringComparison.Ordinal) != -1)
            {
                File.SetAttributes(openFileName, FileAttributes.Normal);
            }

            var projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            var studioUIService =
                Package.GetGlobalService(typeof(SStudioUIService)) as IStudioUIService;
            var errorsOutputService =
                Package.GetGlobalService(typeof(SErrorOutputService)) as IErrorOutputService;
            var quickWatchService =
                Package.GetGlobalService(typeof(SQuickWatchService)) as IQuickWatchService;

            var parserService =
                Package.GetGlobalService(typeof(SParserService)) as IParserService;

            var searchResultService = Package.GetGlobalService(typeof(SSearchResultService)) as ISearchResultService;
            quickWatchService?.LockQuickWatch();
            //
            var project = projectInfoService?.CurrentProject;
            var controller = project?.Controller;

            // 1.go offline
            if (controller != null && controller.IsOnline)
            {
                project.GoOffline();
            }

            // 2.apply
            int saveResult = 0;
            if (project?.Controller != null && project.IsDirty)
            {
                string fileName = Path.GetFileName(project.Controller.ProjectLocaleName);

                string message = LanguageManager.GetInstance().ConvertSpecifier("Project file of") + fileName +
                                 LanguageManager.GetInstance().ConvertSpecifier("has been changed.Save the changes ?");

                var result = MessageBox.Show(message, "ICS Studio", MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Cancel)
                {
                    saveResult = -1;
                }
                else if (result == MessageBoxResult.Yes)
                {
                    saveResult = projectInfoService.Save(false);
                }

            }

            if (saveResult < 0)
            {
                UpdateUI();
                return saveResult;
            }

            // 3.close
            studioUIService?.Close();

            // 4.open
            if (openFileName.EndsWith(".L5X", true, CultureInfo.CurrentCulture))
            {
                projectInfoService?.ImportFile(openFileName, saveFileName);
            }
            else if (openFileName.EndsWith(".json", true, CultureInfo.CurrentCulture))
            {
                projectInfoService?.OpenJsonFile(openFileName);
            }

            // 5.update ui
            studioUIService?.Reset();
            studioUIService?.UpdateWindowTitle();
            errorsOutputService?.Cleanup();
            searchResultService?.Clean();
            quickWatchService?.Reset();
            parserService?.Reset();

            // 6.verify
            if (projectInfoService?.VerifyInDialog() ?? false)
            {
                parserService?.Setup(project);
            }
            else
            {
                studioUIService?.Reset();
            }

            // 7.update ui command
            UpdateUI();
            Controller.GetInstance().IsLoading = false;
            return 0;
        }

        private void SaveFile(object sender, EventArgs e)
        {
            SaveFile(false);
        }

        private void SaveAsFile(object sender, EventArgs e)
        {
            IProjectInfoService projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;

            var controller = projectInfoService?.Controller as Controller;
            if (controller == null)
                return;

            string path = controller.ProjectLocaleName;

            string fileName = Path.GetFileName(path);
            if (string.IsNullOrEmpty(fileName))
                fileName = controller.Name;


            SaveFileDialog saveDlg = new SaveFileDialog()
            {
                InitialDirectory = path.Substring(0, path.LastIndexOf("\\", StringComparison.Ordinal)),
                FileName = fileName,
                DefaultExt = ".json",
                Filter = "json文件(*.json)|*.json|L5X文件(*.L5X)|*.L5X"
            };

            saveDlg.FileOk += (o, eventArgs) =>
            {
                if (controller.IsOnline)
                {
                    string message = "Online tag values may have changed since the last upload.\n";
                    message += "To ensure that current values are saved you should upload them.\n\n";
                    message += "Upload tag value before saving the project?";

                    var messageBoxResult = MessageBox.Show(message, "ICS Studio",
                        MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        var waitingVm = new GeneralWaitingViewModel(true) { WaitingTip = "Upload tag values..." };
                        waitingVm.WorkAction += () =>
                        {
                            ThreadHelper.JoinableTaskFactory.Run(async delegate
                            {
                                int uploadResult = await controller.UploadTagValues();
                                return uploadResult;
                            });
                        };
                        var waitingDialog = new GeneralWaitingDialog(waitingVm);
                        waitingDialog.ShowDialog();
                    }
                    else if (messageBoxResult == MessageBoxResult.Cancel)
                    {
                        eventArgs.Cancel = true;
                        return;
                    }

                }

                // check file name
                int result = IsValidFileName(saveDlg.FileName);
                string warningMessage =
                    LanguageManager.GetInstance().ConvertSpecifier("Failed to save the project as '") +
                    saveDlg.FileName + "'.";
                string warningReason = "";

                switch (result)
                {
                    case 0:
                        break;
                    case 1:
                        warningReason = LanguageManager.GetInstance().ConvertSpecifier("Name is null.");
                        break;
                    case 2:
                        warningReason = LanguageManager.GetInstance().ConvertSpecifier("Name too long.");
                        break;
                    case 3:
                        warningReason = LanguageManager.GetInstance().ConvertSpecifier("Name is invalid.");
                        break;
                    case 4:
                        warningReason = LanguageManager.GetInstance().ConvertSpecifier("Name is invalid.");
                        break;
                    case 5:
                        warningReason = LanguageManager.GetInstance()
                            .ConvertSpecifier("Invalid string because it represents a reserved keyword.");
                        break;
                }

                if (result != 0)
                {
                    eventArgs.Cancel = true;
                    var warningDialog = new WarningDialog(warningMessage, warningReason)
                        { Owner = Application.Current.MainWindow };
                    warningDialog.ShowDialog();
                }
            };

            if (saveDlg.ShowDialog() != DialogResult.OK)
                return;

            fileName = saveDlg.FileName;

            var extension = Path.GetExtension(fileName);

            switch (extension)
            {
                case ".json":
                    int result = projectInfoService.SaveAs(fileName);
                    if (result != 0)
                    {
                        MessageBox.Show(
                            $"Save as failed!", "Error",
                            MessageBoxButton.OKCancel,
                            MessageBoxImage.Error);
                    }

                    break;

                case ".L5X":
                    controller.ExportL5X(fileName);
                    break;
                default:
                    throw new NotImplementedException("Save As File");
            }

            // update ui
            var studioUIService =
                ServiceProvider.GetService(typeof(SStudioUIService)) as IStudioUIService;

            studioUIService?.UpdateWindowTitle();
        }

        private void BuildSaveFile(object sender, EventArgs e)
        {
            SaveFile(true);
        }

        private void PrintMenu(object sender, EventArgs e)
        {

        }

        internal void SaveFile(bool needNativeCode)
        {
            IProjectInfoService projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;

            var controller = projectInfoService?.Controller as Controller;
            if (controller == null)
                return;

            if (controller.IsOnline)
            {
                string message = "Online tag values may have changed since the last upload.\n";
                message += "To ensure that current values are saved you should upload them.\n\n";
                message += "Upload tag value before saving the project?";

                var result = MessageBox.Show(message, "ICS Studio",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    var waitingVm = new GeneralWaitingViewModel(true) { WaitingTip = "Upload tag values..." };
                    waitingVm.WorkAction += () =>
                    {
                        ThreadHelper.JoinableTaskFactory.Run(async delegate
                        {
                            int uploadResult = await controller.UploadTagValues();
                            return uploadResult;
                        });
                    };
                    var waitingDialog = new GeneralWaitingDialog(waitingVm);
                    waitingDialog.ShowDialog();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return;
                }

            }

            int saveResult;
            try
            {
                saveResult = projectInfoService.Save(needNativeCode);
            }
            catch (Exception e)
            {
                saveResult = -100;
                controller.Log($"Save failed!{e.StackTrace}");
            }

            if (saveResult != 0)
            {
                controller.Log($"Save failed!: {saveResult}");

                MessageBox.Show(
                    $"Save failed!", "Error",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Error);
            }

            // update ui
            var studioUIService =
                ServiceProvider.GetService(typeof(SStudioUIService)) as IStudioUIService;

            studioUIService?.UpdateWindowTitle();
        }

        private int IsValidFileName(string address)
        {
            string name = Path.GetFileNameWithoutExtension(address);

            if (string.IsNullOrEmpty(name))
            {
                return 1;
            }

            if (name.Length > 40)
            {
                return 2;
            }

            if (name.EndsWith("_") ||
                name.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
            {
                return 3;
            }

            {
                Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
                if (!regex.IsMatch(name))
                {
                    return 4;
                }
            }

            // key word
            {
                string[] keyWords =
                {
                    "goto",
                    "repeat", "until", "or", "end_repeat",
                    "return", "exit",
                    "if", "then", "elsif", "else", "end_if",
                    "case", "of", "end_case",
                    "for", "to", "by", "do", "end_for",
                    "while", "end_while",
                    "not", "mod", "and", "xor", "or"
                };
                foreach (var keyWord in keyWords)
                {
                    if (keyWord.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        return 5;
                    }
                }
            }

            return 0;
        }

        internal void UpdateUI()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            IVsUIShell vsShell =
                ServiceProvider.GetService(typeof(SVsUIShell)) as IVsUIShell;

            if (vsShell != null)
            {
                int hr = vsShell.UpdateCommandUI(0);
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
            }
        }

        private void MenuItemOnBeforeQueryStatus(object sender, EventArgs e)
        {
            var menuCommand = sender as OleMenuCommand;
            var projectInfoService =
                ServiceProvider?.GetService(typeof(SProjectInfoService)) as IProjectInfoService;
            var controller = projectInfoService?.Controller;
            if (menuCommand != null)
            {
                switch (menuCommand.CommandID.ID)
                {
                    case PackageIds.historyFileCommand:

                        break;
                    case PackageIds.openFileCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Open...");
                        break;

                    case PackageIds.newFileCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("New...");
                        break;

                    case PackageIds.saveFileCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Save");
                        menuCommand.Enabled = controller != null;
                        break;

                    case PackageIds.saveAsFileCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Save As...");
                        menuCommand.Enabled = controller != null;
                        break;

                    case PackageIds.buildSaveFileCommand:
                        menuCommand.Visible = false;
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Build Save");
                        menuCommand.Enabled = controller != null;
                        break;

                    case PackageIds.PrintSubMenu:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Print");
                        menuCommand.Enabled = controller != null;
                        break;

                }
            }
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
                case PackageIds.printRoutine:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Routine...");
                    if (selectedProjectItem != null && selectedProjectItem.Kind == ProjectItemType.Routine)
                    {
                        menuCommand.Enabled = true;
                    }
                    else
                    {
                        menuCommand.Enabled = false;
                    }

                    break;
                case PackageIds.printRoutineProperties:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Routine Properties...");
                    if (selectedProjectItem != null && selectedProjectItem.Kind == ProjectItemType.Routine)
                    {
                        menuCommand.Enabled = true;
                    }
                    else
                    {
                        menuCommand.Enabled = false;
                    }

                    break;
                case PackageIds.printTags:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Tags...");
                    if (selectedProjectItem != null && (selectedProjectItem.Kind == ProjectItemType.ControllerModel
                                                        || selectedProjectItem.Kind == ProjectItemType.ControllerTags
                                                        || selectedProjectItem.Kind == ProjectItemType.ProgramTags
                                                        || selectedProjectItem.Kind == ProjectItemType.Routine
                                                        || selectedProjectItem.Kind == ProjectItemType.MotionGroup))
                    {
                        menuCommand.Enabled = true;
                    }
                    else
                    {
                        menuCommand.Enabled = false;
                    }

                    break;
                case PackageIds.printSequencingAndStepTags:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Sequencing and Step Tags...");
                    menuCommand.Enabled = false;
                    break;
                case PackageIds.printModuleProperties:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Module Properties...");
                    if (selectedProjectItem != null && (selectedProjectItem.Kind == ProjectItemType.Bus
                                                        || selectedProjectItem.Kind == ProjectItemType.DeviceModule
                                                        || selectedProjectItem.Kind == ProjectItemType.LocalModule
                                                        || selectedProjectItem.Kind == ProjectItemType.Ethernet))
                    {
                        menuCommand.Enabled = true;
                    }
                    else
                    {
                        menuCommand.Enabled = false;
                    }

                    break;
                case PackageIds.printAdd_OnInstructionSignatures:
                    menuCommand.Text = LanguageManager.GetInstance()
                        .ConvertSpecifier("User Define Funciton Signatures...");
                    menuCommand.Enabled = false;
                    break;
                case PackageIds.printAdd_OnInstruction:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("User Define Funciton...");
                    if (selectedProjectItem != null && selectedProjectItem.Kind == ProjectItemType.AddOnInstruction)
                    {
                        menuCommand.Enabled = true;
                    }
                    else
                    {
                        menuCommand.Enabled = false;
                    }

                    break;
                case PackageIds.printDataType:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("DataType...");
                    if (selectedProjectItem != null && (selectedProjectItem.Kind == ProjectItemType.Predefined
                                                        || selectedProjectItem.Kind == ProjectItemType.UserDefined
                                                        || selectedProjectItem.Kind == ProjectItemType.String
                                                        || selectedProjectItem.Kind == ProjectItemType.AddOnDefined
                                                        || selectedProjectItem.Kind == ProjectItemType.ModuleDefined))
                    {
                        menuCommand.Enabled = true;
                    }
                    else
                    {
                        menuCommand.Enabled = false;
                    }

                    break;
                case PackageIds.printCrossReference:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Cross Reference...");
                    menuCommand.Enabled = false;
                    break;
                case PackageIds.printControllerOrganizer:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Controller Organizer...");
                    if (selectedProjectItem != null && selectedProjectItem.Kind == ProjectItemType.ControllerModel)
                    {
                        menuCommand.Enabled = true;
                    }
                    else
                    {
                        menuCommand.Enabled = false;
                    }

                    break;
                case PackageIds.printControllerProperties:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Controller Properties...");
                    if (selectedProjectItem != null && selectedProjectItem.Kind == ProjectItemType.ControllerModel)
                    {
                        menuCommand.Enabled = true;
                    }
                    else
                    {
                        menuCommand.Enabled = false;
                    }

                    break;
                case PackageIds.printTaskProperties:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Task Properties...");
                    if (selectedProjectItem != null && selectedProjectItem.Kind == ProjectItemType.Task)
                    {
                        menuCommand.Enabled = true;
                    }
                    else
                    {
                        menuCommand.Enabled = false;
                    }

                    break;
                case PackageIds.printProgramProperties:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Program Properties...");
                    if (selectedProjectItem != null && (selectedProjectItem.Kind == ProjectItemType.Program))
                    {
                        menuCommand.Enabled = true;
                    }
                    else
                    {
                        menuCommand.Enabled = false;
                    }

                    break;
                case PackageIds.printEquipmentSequence:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Equipment Sequences...");
                    menuCommand.Enabled = false;
                    break;
                case PackageIds.printEntireProject:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Entire Project...");
                    if (selectedProjectItem != null && (selectedProjectItem.Kind == ProjectItemType.ControllerModel
                                                        || selectedProjectItem.Kind == ProjectItemType.Routine))
                    {
                        menuCommand.Enabled = true;
                    }
                    else
                    {
                        menuCommand.Enabled = false;
                    }

                    break;
            }
        }
    }
}