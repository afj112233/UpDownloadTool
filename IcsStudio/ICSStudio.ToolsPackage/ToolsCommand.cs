//------------------------------------------------------------------------------
// <copyright file="ToolsCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Notification;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.ToolsPackage.SourceProtection;
using ICSStudio.UIInterfaces.Project;
using Microsoft.VisualStudio.Shell;
using Application = System.Windows.Application;
using ICSStudio.ToolsPackage.Import;
using ICSStudio.UIInterfaces.Error;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json.Linq;
using ICSStudio.MultiLanguage;
using MessageBox = System.Windows.MessageBox;
using ICSStudio.SimpleServices.DataType;

namespace ICSStudio.ToolsPackage
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ToolsCommand
    {
        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package _package;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolsCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private ToolsCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            this._package = package;

            OleMenuCommandService commandService =
                this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            if (commandService != null)
            {

                var menuCommandID = new CommandID(PackageGuids.toolsCommandPackageCmdSet, PackageIds.optionsCommand);
                var menuItem = new OleMenuCommand(this.ConfigureOptions, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.toolsCommandPackageCmdSet,
                    PackageIds.securityCommand);
                menuItem = new OleMenuCommand(Security, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.toolsCommandPackageCmdSet,
                    PackageIds.languagesCommand);
                menuItem = new OleMenuCommand(DocunmentationLanguage, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.toolsCommandPackageCmdSet,
                    PackageIds.importCommand);
                menuItem = new OleMenuCommand(Import, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.toolsCommandPackageCmdSet,
                    PackageIds.exportCommand);
                menuItem = new OleMenuCommand(Export, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.toolsCommandPackageCmdSet,
                    PackageIds.logOnCommand);
                menuItem = new OleMenuCommand(Export, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.toolsCommandPackageCmdSet,
                    PackageIds.logOffCommand);
                menuItem = new OleMenuCommand(Export, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.toolsCommandPackageCmdSet,
                    PackageIds.refreshPrivilegesCommand);
                menuItem = new OleMenuCommand(Export, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.toolsCommandPackageCmdSet,
                    PackageIds.lockAllUnlockedContentCommand);
                menuItem = new OleMenuCommand(Export, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.toolsCommandPackageCmdSet,
                    PackageIds.lockComponentCommand);
                menuItem = new OleMenuCommand(Export, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.toolsCommandPackageCmdSet,
                    PackageIds.unlockComponentCommand);
                menuItem = new OleMenuCommand(Export, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.toolsCommandPackageCmdSet,
                    PackageIds.configureSourceProtectionCommand);
                menuItem = new OleMenuCommand(ConfigureSourceProtection, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.toolsCommandPackageCmdSet,
                    PackageIds.exportTagsCommand);
                menuItem = new OleMenuCommand(ExportTags, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.toolsCommandPackageCmdSet,
                    PackageIds.importTagsCommand);
                menuItem = new OleMenuCommand(ImportTags, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);


                menuCommandID = new CommandID(PackageGuids.toolsCommandPackageCmdSet,
                    PackageIds.importComponentCommand);
                menuItem = new OleMenuCommand(ImportComponent, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.toolsCommandPackageCmdSet,
                    PackageIds.exportComponentCommand);
                menuItem = new OleMenuCommand(ExportComponent, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.toolsCommandPackageCmdSet,
                    PackageIds.updateCommand);
                menuItem = new OleMenuCommand(Update, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.toolsCommandPackageCmdSet,
                    PackageIds.gatewayCommand);
                menuItem = new OleMenuCommand(Gateway, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.toolsCommandPackageCmdSet,
                    PackageIds.compareCommand);
                menuItem = new OleMenuCommand(Compare, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ToolsCommand Instance { get; private set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => this._package;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new ToolsCommand(package);
        }

        private void MenuItemOnBeforeQueryStatus(object sender, EventArgs e)
        {
            var menuCommand = sender as OleMenuCommand;

            var projectInfoService =
                ServiceProvider?.GetService(typeof(SProjectInfoService)) as IProjectInfoService;
            var controller = projectInfoService?.Controller;

            if (menuCommand != null)
                switch (menuCommand.CommandID.ID)
                {
                    case PackageIds.optionsCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Options...");
                        break;
                    case PackageIds.securityCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Security");
                        break;
                    case PackageIds.languagesCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Documentation Languages...");
                        break;
                    case PackageIds.importCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Import");
                        break;
                    case PackageIds.exportCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Export");
                        break;
                    case PackageIds.logOnCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Log On...");
                        break;
                    case PackageIds.logOffCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Log Off");
                        break;
                    case PackageIds.refreshPrivilegesCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Refresh Privileges");
                        break;
                    case PackageIds.lockAllUnlockedContentCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Lock All Unlocked Content");
                        break;
                    case PackageIds.lockComponentCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Lock Component");
                        break;
                    case PackageIds.unlockComponentCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("UnLock Component");
                        break;

                    case PackageIds.configureSourceProtectionCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Configure Source Protection");
                        if (controller == null)
                            menuCommand.Enabled = false;
                        else
                        {
                            menuCommand.Enabled = !controller.IsOnline;
                        }

                        break;

                    case PackageIds.importTagsCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Variables and Logic Comments...");
                        if (controller == null)
                            menuCommand.Enabled = false;
                        else
                            menuCommand.Enabled = !controller.IsOnline;

                        break;
                    case PackageIds.importComponentCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Component");
                        break;
                    case PackageIds.exportTagsCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Variables and Logic Comments...");
                        break;
                    case PackageIds.exportComponentCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Component");
                        break;
                }
        }

        public void OpenExeFile(string path, string softwareName)
        {
            if (!File.Exists(path))
            {
                MessageBox.Show($"{softwareName} {LanguageManager.GetInstance().ConvertSpecifier("cannot be opened correctly.")}",
                    "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                Controller.GetInstance().Log($"{path} is not exist.");
                return;
            }

            try
            {
                using (var process = new Process())
                {
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.FileName = path;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        private void Update(object sender, EventArgs e)
        {
            var currentDirectory = @"Extensions\IconUpdater\IconUpdater.exe";
            var softwareName = "IconUpdater";
            OpenExeFile(currentDirectory, softwareName);
        }

        private void Gateway(object sender, EventArgs e)
        {
            var currentDirectory = @"Extensions\ICSGateway\ICSGateway.exe";
            var softwareName = "ICSGateway";
            OpenExeFile(currentDirectory, softwareName);
        }

        private void Compare(object sender, EventArgs e)
        {
            var currentDirectory = @"Extensions\ICSStudio.CompareTool\ICSStudio.CompareTool.exe";
            var softwareName = "CompareTool";
            OpenExeFile(currentDirectory, softwareName);
        }

        private void ImportComponent(object sender, EventArgs e)
        {

        }
        private void ExportComponent(object sender, EventArgs e)
        {

        }
        private void DocunmentationLanguage(object sender, EventArgs e)
        {

        }
        private void Import(object sender, EventArgs e)
        {

        }
        private void Export(object sender, EventArgs e)
        {

        }
        private void LogOff(object sender, EventArgs e)
        {

        }
        private void Security(object sender, EventArgs e)
        {

        }

        private void LogOn(object sender, EventArgs e)
        {

        }

        private void ConfigureOptions(object sender, EventArgs e)
        {
            //TODO(gjc): add code here
        }

        private void ConfigureSourceProtection(object sender, EventArgs e)
        {
            SourceProtectionDialog dialog = new SourceProtectionDialog
            {
                Owner = Application.Current.MainWindow
            };

            SourceProtectionViewModel viewModel =
                new SourceProtectionViewModel(Controller.GetInstance().SourceProtectionManager);
            dialog.DataContext = viewModel;

            dialog.ShowDialog();
        }

        private void ExportTags(object sender, EventArgs e)
        {
            var projectInfoService =
                ServiceProvider?.GetService(typeof(SProjectInfoService)) as IProjectInfoService;
            var controller = projectInfoService?.Controller as Controller;

            if (controller == null)
                return;

            var saveFileDialog = new SaveFileDialog
            {
                Title = @"Export",
                Filter =
                    @"IcsStudio Import//Export File(*.CSV)|*.CSV|IcsStudio Unicode Import//Export File(*.TXT)|*.TXT"
            };

            saveFileDialog.FileName = controller.GetFileName(controller) + "_Tags";

            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

            string contents = string.Empty;
            Encoding encoding = Encoding.UTF8;


            List<ITagCollection> tagCollectionList = new List<ITagCollection>();
            List<string> scope = new List<string>();
            List<string> type = new List<string>();

            //controller
            tagCollectionList.Add(controller.Tags);
            scope.Add(String.Empty);
            type.Add("Controller");

            // aoi
            var aoiDefinition = controller.AOIDefinitionCollection;
            if (aoiDefinition != null)
            {
                foreach (var aoi in aoiDefinition)
                {
                    tagCollectionList.Add(aoi.Tags);
                    scope.Add(aoi.Name + ":AOI");
                    type.Add("AOI");
                }
            }

            // program
            var proDefinition = controller.Programs;
            if (proDefinition != null)
            {
                foreach (var program in proDefinition)
                {
                    tagCollectionList.Add(program.Tags);
                    scope.Add(program.Name);
                    type.Add("Program");
                }
            }

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


            StringBuilder stringBuilder = new StringBuilder();
            DateTime dateNum = DateTime.Now;

            List<string> header1 = new List<string>() { "remark", "CSV-Import-Export" };
            List<string> header2 = new List<string>()
            {
                "remark",
                "Date = " + dateNum.ToString("ddd MMM dd HH:mm:ss yyyy", CultureInfo.CreateSpecificCulture("en-GB"))
            };
            List<string> header3 = new List<string>() { "remark", "Version = ICSStudio" };
            List<string> header4 = new List<string>() { "remark", "Owner = " };
            List<string> header5 = new List<string>() { "remark", "Company = " };
            List<string> header6 = new List<string>() { "0.3" };
            List<string> header7 = new List<string>()
            {
                "TYPE", "SCOPE", "NAME", "DESCRIPTION", "DATATYPE", "SPECIFIER", "ATTRIBUTES"
            };
            stringBuilder.AppendLine(string.Join(separator, header1));
            stringBuilder.AppendLine(string.Join(separator, header2));
            stringBuilder.AppendLine(string.Join(separator, header3));
            stringBuilder.AppendLine(string.Join(separator, header4));
            stringBuilder.AppendLine(string.Join(separator, header5));
            stringBuilder.AppendLine(string.Join(separator, header6));

            for (int i = 0; i < tagCollectionList.Count; i++)
            {
                if (tagCollectionList[i].Count != 0)
                {
                    stringBuilder.AppendLine(string.Join(separator, header7));
                }

                contents = Controller.ExportAllTags(tagCollectionList[i], separator, scope[i], stringBuilder, type[i]);
            }

            // contents -> file
            File.WriteAllText(saveFileDialog.FileName, contents, encoding);
        }

        private void ImportTags(object sender, EventArgs e)
        {
            var controller = Controller.GetInstance();

            var hasMotionGroupTagInController = controller.HasMotionGroup();

            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Title = @"Import",
                Filter =
                    @"Logix Designer Import/Export Files(*.CSV)|*.CSV|Logix Designer Unicode Import/Export Files(*.TXT)|*.TXT"
            };
            var result = fileDialog.ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Yes)
            {
                var errorOutputService = Package.GetGlobalService(typeof(SErrorOutputService)) as IErrorOutputService;
                errorOutputService?.RemoveImportError();

                var lineCount = 0;
                var index = 0;
                var tags = new List<JObject>();

                using (TextFieldParser parser = new TextFieldParser(fileDialog.FileName, Encoding.UTF8))
                {
                    parser.TextFieldType = FieldType.Delimited;

                    parser.SetDelimiters(",");

                    parser.TrimWhiteSpace = false;

                    List<string> errorDescription = new List<string>();
                    List<string> warningDescription = new List<string>();

                    while (!parser.EndOfData)
                    {
                        lineCount++;

                        string[] fields = parser.ReadFields();

                        if (index < 5)
                        {
                            if (!"remark".Equals(fields[0], StringComparison.OrdinalIgnoreCase))
                            {
                                errorDescription.Add(
                                    $"Error: Line {lineCount}: Import Export version mismatch.Expected version is 0.3.Check Import Export version syntax.");
                                continue;
                            }

                            index++;
                            continue;
                        }

                        if (index == 5)
                        {
                            if (!"0.3".Equals(fields[0]))
                            {
                                errorDescription.Add(
                                    $"Error: Line {lineCount}: Import Export version mismatch.Expected version is 0.3.Cannot import or paste into an older version of Logix Designer");
                                continue;
                            }

                            index++;
                            continue;
                        }

                        var type = fields[0];
                        index++;
                        if ("TYPE".Equals(type, StringComparison.OrdinalIgnoreCase))
                        {
                            if (!("TYPE".Equals(fields[0], StringComparison.OrdinalIgnoreCase) &&
                                  "SCOPE".Equals(fields[1], StringComparison.OrdinalIgnoreCase) &&
                                  "NAME".Equals(fields[2], StringComparison.OrdinalIgnoreCase) &&
                                  "DESCRIPTION".Equals(fields[3], StringComparison.OrdinalIgnoreCase) &&
                                  "DATATYPE".Equals(fields[4], StringComparison.OrdinalIgnoreCase) &&
                                  "SPECIFIER".Equals(fields[5], StringComparison.OrdinalIgnoreCase) &&
                                  "ATTRIBUTES".Equals(fields[6], StringComparison.OrdinalIgnoreCase)))
                            {
                                errorDescription.Add(
                                    $"Error: Line {lineCount}:The CSV Schema line or Record line is invalid.");
                                errorDescription.Add(
                                    $"Error: Line {lineCount}:Syntax error found while scanning import file.");
                            }

                            continue;
                        }

                        if ("tag".Equals(type, StringComparison.OrdinalIgnoreCase))
                        {
                            var scope = fields[1];
                            var name = fields[2];
                            var description = DescriptionConvert(fields[3]);
                            var dataType = fields[4];
                            var dataTypeInfo = controller.DataTypes.ParseDataTypeInfo(dataType);
                            var dataTypeInCSV = dataTypeInfo == null ? null : dataTypeInfo.DataType;

                            //Can only have one Tag in Program which Type is MotionGroup.
                            if (dataTypeInCSV != null && dataTypeInCSV.IsMotionGroupType)
                            {
                                if (hasMotionGroupTagInController)
                                {
                                    errorDescription.Add($"Error: Line {lineCount}: Maximum number of motion group tags has been reached.");
                                    index++;
                                    continue;
                                }
                            }

                            var IsValidTagNameResult = IsValidTagName(name, scope);
                            if (IsValidTagName(name, scope) < 0)
                            {
                                switch (IsValidTagNameResult)
                                {
                                    case -1:
                                        errorDescription.Add(
                                            $"Error: Line {lineCount}:Error creating 'Tag[@Name = \"{name}\"]'.");
                                        index++;
                                        continue;

                                    case -2:
                                        {
                                            //if tag name which in CSV is same with Contoller's.
                                            if (!controller.Tags.Any(Item => Item.Name.Equals(name)))
                                            {
                                                errorDescription.Add($"Error: Line {lineCount}:Error creating 'Tag[@Name = \"{name}\"]'.");
                                                index++;
                                                continue;
                                            }
                                            else
                                            {
                                                //if Tag is Module_Defined, Need special Error Tips
                                                if (dataTypeInCSV != null && dataTypeInCSV is ModuleDefinedDataType)
                                                {
                                                    errorDescription.Add($"Error: Line {lineCount}: IO-Tags can not be created as part of CSV import (only overwritten)");
                                                }
                                            }

                                            break;
                                        }
                                }
                            }
                            
                            foreach (var item in tags)
                            {
                                //If the tag name is the same as the previous tag name in the CSV.
                                if (item["Name"].ToString().Equals(name) && item["Scope"].ToString().Equals(scope))
                                {
                                    tags = tags.Where(t =>
                                            !(t["Name"].ToString().Equals(name) && t["Scope"].ToString().Equals(scope)))
                                        .ToList();

                                    warningDescription.Add($"Warning: Line {lineCount}: Name collision: Tag '{name}' overwritten with imported definition");

                                }

                                //One more MotionGroup Tag In CSV
                                var aboveDataTypeInCSV = controller.DataTypes.ParseDataTypeInfo(item["DataType"].ToString()).DataType;
                                if (aboveDataTypeInCSV != null && aboveDataTypeInCSV.IsMotionGroupType)
                                {
                                    errorDescription.Add($"Error: Line {lineCount}: Maximum number of motion group tags has been reached.");
                                    index++;
                                    continue;
                                }
                            }
                            
                            var specifier = fields[5];
                            var attributes = fields[6];
                            if (!string.IsNullOrEmpty(scope))
                            {
                                if (scope.EndsWith(":aoi", StringComparison.OrdinalIgnoreCase))
                                {
                                    scope = scope.Substring(0, scope.Length - 4);
                                    if (((AoiDefinitionCollection) controller.AOIDefinitionCollection)
                                        .Find(scope) == null)
                                    {
                                        errorDescription.Add(
                                            $"Error: Line {lineCount}:Scope parameter indicates a non-existent program or an invalid component path.");
                                        index++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (controller.Programs[scope] == null)
                                    {
                                        errorDescription.Add(
                                            $"Error: Line {lineCount}:Scope parameter indicates a non-existent program or an invalid component path.");
                                        index++;
                                        continue;
                                    }
                                }
                            }

                            if (controller.DataTypes.ParseDataTypeInfo(dataType).DataType == null)
                            {
                                errorDescription.Add($"Error: Line {lineCount}:Data type could not be found.");
                                index++;
                                continue;
                            }

                            var tag = new JObject();
                            tag["Name"] = name;
                            tag["Scope"] = scope;
                            tag["Description"] = description;
                            tag["DataType"] = dataType;
                            tag["Attributes"] = attributes;
                            tags.Add(tag);
                        }
                        else
                        {
                            errorDescription.Add(
                                $"Error: Line {lineCount}:The CSV Schema line or Record line is invalid.");
                            errorDescription.Add(
                                $"Error: Line {lineCount}:Syntax error found while scanning import file.");
                            continue;
                        }

                        index++;
                    }

                    var dialog = new ImportingDialog(tags);
                    dialog.Owner = Application.Current.MainWindow;
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    if (dialog.ShowDialog() ?? false)
                    {
                        Notifications.Publish(new MessageData() {Type = MessageData.MessageType.Verify});
                    }


                    foreach (var description in errorDescription)
                    {
                        errorOutputService?.AddErrors(description, OrderType.None, OnlineEditType.Original, null, null,
                            fileDialog.FileName);
                    }
                    foreach(var description in warningDescription)
                    {
                        errorOutputService?.AddWarnings(description, null);
                    }
                }
            }
        }

        private int IsValidTagName(string name, string scope)
        {
            if (string.IsNullOrEmpty(name))
            {
                return -1;
            }

            if (name.Length > 40 || name.EndsWith("_") ||
                name.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
            {
                return -1;
            }

            Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
            if (!regex.IsMatch(name))
            {
                return -2;
            }

            // key word
            string[] keyWords =
            {
                "goto",
                "repeat", "until", "or", "end_repeat",
                "return", "exit",
                "if", "then", "elsif", "else", "end_if",
                "case", "of", "end_case",
                "for", "to", "by", "do", "end_for",
                "while", "end_while",
                "not", "mod", "and", "xor", "or",
                "ABS", "SQRT",
                "LOG", "LN",
                "DEG", "RAD", "TRN",
                "ACS", "ASN", "ATN", "COS", "SIN", "TAN"
            };
            foreach (var keyWord in keyWords)
            {
                if (keyWord.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return -1;
                }
            }

            return 0;
        }

        public static string DescriptionConvert(string input)
        {
            StringBuilder result = new StringBuilder();

            input = input.Replace("$$", "{$$}");

            string descriptionAfterConvert = DeUnicode(input);

            descriptionAfterConvert = descriptionAfterConvert.Replace("{$$}", "$$");

            for (int i = 0; i < descriptionAfterConvert.Length; i++)
            {
                if (descriptionAfterConvert[i].Equals('$'))
                {
                    if (descriptionAfterConvert[i + 1].Equals('$'))
                    {
                        result.Append('$');
                        i++;
                    }
                    else if (descriptionAfterConvert[i + 1].Equals('\''))
                    {
                        result.Append('\'');
                        i++;
                    }
                    else if (descriptionAfterConvert[i + 1].Equals('Q'))
                    {
                        result.Append('"');
                        i++;
                    }
                    else if (descriptionAfterConvert[i + 1].Equals('N'))
                    {
                        result.Append("\r\n");
                        i++;
                    }
                    else if (descriptionAfterConvert[i + 1].Equals('T'))
                    {
                        result.Append("\t");
                        i++;
                    }
                    else
                    {
                        result.Append(descriptionAfterConvert[i]);
                    }
                }
                else
                {
                    result.Append(descriptionAfterConvert[i]);
                }
            }

            return result.ToString();
        }

        public static string DeUnicode(string str)
        {
            Regex reg = new Regex(@"(?i)[$]([0-9a-f]{4})");
            return reg.Replace(str,
                delegate(Match m) { return ((char) Convert.ToInt32(m.Groups[1].Value, 16)).ToString(); });
        }
    }
}
