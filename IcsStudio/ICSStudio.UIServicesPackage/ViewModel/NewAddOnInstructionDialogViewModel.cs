using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Dialogs.GlobalSetting;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIInterfaces.Project;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json.Linq;
using ICSStudio.Gui.Utils;
using ICSStudio.UIInterfaces.Editor;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    public class NewAddOnInstructionDialogViewModel : ViewModelBase
    {
        private bool? _dialogResult;
        private readonly IController _controller;

        public NewAddOnInstructionDialogViewModel()
        {
            IProjectInfoService projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            _controller = projectInfoService?.Controller;
            List<RoutineType> listRoutineTypes = new List<RoutineType>()
                { RoutineType.RLL, RoutineType.FBD, RoutineType.ST };
            OkCommand = new RelayCommand(ExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);

            TypeList = listRoutineTypes.Select(x =>
                {
                    var name = "";
                    switch (x)
                    {
                        case RoutineType.RLL:
                            name = "Ladder Diagram";
                            break;
                        case RoutineType.FBD:
                            name = "Function Block Diagram";
                            break;
                        case RoutineType.ST:
                            name = "Structured Text";
                            break;
                    }

                    return new
                    {
                        Value = x,
                        DisplayName = name
                    };
                }
            ).ToList();
            Type = RoutineType.ST;
            IsOpenDefinition = GlobalSetting.GetInstance().NewAoiDialogSetting.IsOpenDefinition;
            IsOpenLogicRoutine = GlobalSetting.GetInstance().NewAoiDialogSetting.IsOpenLogic;
        }

        public string Name { set; get; }
        public string Description { set; get; }
        public IList TypeList { set; get; }
        public RoutineType Type { set; get; }
        public int Major { set; get; } = 1;
        public int Minor { set; get; }
        public string ExtendedText { set; get; }
        public string RevisionNote { set; get; }
        public string Vendor { set; get; }
        public bool IsOpenLogicRoutine { get; set; }
        public bool IsOpenDefinition { get; set; }
        public RelayCommand OkCommand { set; get; }


        private void ExecuteOkCommand()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (!IsValidName(Name)) return;

            var config = new JObject();
            var routines = new JArray();
            var routine = new JObject();
            routine["Name"] = "Logic";
            routine["Type"] = (byte)Type;
            routine["CodeText"] = new JArray();
            routines.Add(routine);
            config["Routines"] = routines;

            config["Name"] = Name;
            config["Description"] = Description ?? "";
            string revision = $"{Major}.{Minor}";
            config["Revision"] = revision;
            config["ExtendedText"] = ExtendedText ?? "";
            config["RevisionNote"] = RevisionNote ?? "";
            config["Vendor"] = Vendor ?? "";
            var parameters = new JArray();
            var enableIn = new JObject();
            enableIn["Name"] = "EnableIn";
            enableIn["Usage"] = (byte)Usage.Input;
            enableIn["DataType"] = "BOOL";
            enableIn["Description"] = "Enable Input - System Defined Parameter";
            enableIn["Radix"] = (byte)DisplayStyle.Decimal;
            JObject data = new JObject();
            data["Value"] = 1;
            data["Radix"] = DisplayStyle.Decimal.ToString();
            data["DataType"] = "BOOL";
            enableIn["DefaultData"] = data;
            enableIn["Required"] = false;
            enableIn["Visible"] = false;
            enableIn["ExternalAccess"] = (byte)ExternalAccess.ReadOnly;

            var enableOut = new JObject();
            enableOut["Name"] = "EnableOut";
            enableOut["Usage"] = (byte)Usage.Output;
            enableOut["DataType"] = "BOOL";
            enableOut["Description"] = "Enable Output - System Defined Parameter";
            enableOut["Radix"] = (byte)DisplayStyle.Decimal;
            data = new JObject();
            data["Value"] = 0;
            data["Radix"] = DisplayStyle.Decimal.ToString();
            data["DataType"] = "BOOL";
            enableOut["DefaultData"] = data;
            enableOut["Required"] = false;
            enableOut["Visible"] = false;
            enableOut["ExternalAccess"] = (byte)ExternalAccess.ReadOnly;
            parameters.Add(enableIn);
            parameters.Add(enableOut);
            config["Parameters"] = parameters;

            string hostName = Environment.MachineName + @"\" + System.Environment.UserName;
            config["EditedBy"] = hostName;
            config["EditedDate"] = DateTime.Now.ToString("yyyy/M/dd HH:mm:ss");
            config["CreatedBy"] = hostName;
            config["CreatedDate"] = DateTime.Now.ToString("yyyy/M/dd HH:mm:ss");

            config["LocalTags"] = new JArray();
            AoiDefinition aoiDefinition = new AoiDefinition(config, _controller);
            //aoiDefinition.ParserTags();
            //aoiDefinition.PostInit(_controller.DataTypes as DataTypeCollection);
            aoiDefinition.datatype.PostInit(_controller.DataTypes);
            (_controller.AOIDefinitionCollection as AoiDefinitionCollection)?.Add(aoiDefinition);
            (_controller.DataTypes as DataTypeCollection)?.AddDataType(aoiDefinition.datatype);
             GlobalSetting.GetInstance().NewAoiDialogSetting.IsOpenDefinition= IsOpenDefinition;
             GlobalSetting.GetInstance().NewAoiDialogSetting.IsOpenLogic= IsOpenLogicRoutine;
            DialogResult = true;

            if (IsOpenDefinition)
            {
                var createDialogService = Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;
                var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
                createDialogService?.AddOnInstructionProperties(aoiDefinition)?.Show(uiShell);
            }
            if (IsOpenLogicRoutine)
            {
                var createEditorService =
                    Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
                createEditorService?.CreateSTEditor(aoiDefinition.Routines.First());
            }
        }

        private bool IsValidName(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            string warningMessage = "Failed to create the instruction.";
            string warningReason = string.Empty;
            bool isValid = true;

            if (isValid)
            {
                if (name.Length > 40 || name.EndsWith("_") ||
                    name.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    isValid = false;
                    warningReason = "Name is invalid.";
                }
            }

            if (isValid)
            {
                Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
                // ReSharper disable once StringIndexOfIsCultureSpecific.1
                if (!regex.IsMatch(name) || name.IndexOf("__") > -1 || name.EndsWith("_"))
                {
                    isValid = false;
                    warningReason = "Name is invalid.";
                }
            }

            if (isValid)
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
                    "not", "mod", "and", "xor", "or",
                    "ABS","SQRT",
                    "LOG","LN",
                    "DEG","RAD","TRN",
                    "ACS","ASN","ATN","COS","SIN","TAN"
                };
                foreach (var keyWord in keyWords)
                {
                    if (keyWord.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        isValid = false;
                        warningReason = "Name is invalid.";
                    }
                }
            }

            //和数据类型重名的判断
            if (isValid)
            {
                foreach (var item in _controller.DataTypes)
                {
                    if (item.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        isValid = false;
                        warningReason = "The name must be a unique instruction or data type name";
                    }
                }
            }

            //和指令重名的判断
            if (isValid)
            {
                var stInstructionCollection = Controller.GetInstance().STInstructionCollection;
                var rllInstructionCollection = Controller.GetInstance().RLLInstructionCollection;
                var fbdInstructionCollection = Controller.GetInstance().FBDInstructionCollection;

                if (stInstructionCollection.FindInstruction(Name) != null ||
                    rllInstructionCollection.FindInstruction(Name) != null ||
                    fbdInstructionCollection.FindInstruction(Name) != null)
                {
                    isValid = false;
                    warningReason =
                        "This is a reserved instruction name.";
                }
            }

            if (isValid)
            {
                var instruction = Controller.GetInstance().STInstructionCollection.FindInstruction(name);
                if (instruction != null)
                {
                    isValid = false;
                    warningReason = "This is a reserved instruction name.";
                }
            }

            if (!isValid)
            {
                var warningDialog = new WarningDialog(warningMessage, warningReason)
                    { Owner = Application.Current.MainWindow };
                warningDialog.ShowDialog();
            }

            return isValid;
        }

        public RelayCommand CancelCommand { set; get; }

        private void ExecuteCancelCommand()
        {
            DialogResult = false;
        }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }
    }
}
