using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using ICSStudio.Dialogs.WaitDialog;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIInterfaces.Editor;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.UIServicesPackage.AddOnInstruction.ExtendDialog;
using ICSStudio.UIServicesPackage.AddOnInstruction.panel;
using ICSStudio.UIServicesPackage.AddOnInstruction.Panel;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Type = ICSStudio.UIInterfaces.Editor.Type;

// ReSharper disable PossibleNullReferenceException

namespace ICSStudio.UIServicesPackage.AddOnInstruction
{
    public sealed class AddOnInstructionVM : AddOnInstructionOptionsDialogViewModel, ICanApply
    {
        private readonly List<IOptionPanelDescriptor> _optionPanelDescriptors;
        private bool _isCorrect = true;
        private readonly AoiDefinition _aoiDefinition;
        private bool _isClosing;
        private string _size;
        private string _byte;
        private AOIDataType _aoiDataType;
        private string _dataType;

        public AddOnInstructionVM(IAoiDefinition aoiDefinition)
        {
            if (aoiDefinition == null)
                throw new ArgumentOutOfRangeException();

            _aoiDefinition = (AoiDefinition) aoiDefinition;

            PropertyChangedEventManager.AddHandler(_aoiDefinition, OnPropertyChanged, "");
            //_addOnInstruction.PropertyChanged += OnPropertyChanged;
            _optionPanelDescriptors = new List<IOptionPanelDescriptor>
            {
                new DefaultOptionPanelDescriptor("1", "General", "General",
                    new GeneralViewModel(new General(), _aoiDefinition), null),
                new DefaultOptionPanelDescriptor("2", "Parameters", "Parameters",
                    new ParametersViewModel(new Parameters(), _aoiDefinition,this), null),
                new DefaultOptionPanelDescriptor("3", "Local Variables", "Local Variables",
                    new LocalTagsViewModel(new LocalTags(), _aoiDefinition,this), null),
                new DefaultOptionPanelDescriptor("4", "Scan Modes", "Scan Modes",
                    new ScanModesViewModel(new ScanModes(), _aoiDefinition), null),
                new DefaultOptionPanelDescriptor("5", "Signature", "Signature",
                    new SignatureViewModel(new Signature(), _aoiDefinition,this), null),
                new DefaultOptionPanelDescriptor("6", "Change History", "Change History",
                    new ChangeHistoryViewModel(new ChangeHistory(), _aoiDefinition), null),
                new DefaultOptionPanelDescriptor("7", "Help", "Help",
                    new HelpViewModel(new Help(_aoiDefinition), _aoiDefinition), null),
            };
            _aoiDataType = _aoiDefinition.datatype;
            PropertyChangedEventManager.AddHandler(_aoiDataType, OnDataTypePropertyChanged, "");
            //aoiDataType.PropertyChanged += OnDataTypePropertyChanged;
            _size = LanguageManager.GetInstance().ConvertSpecifier("Size");
            _byte = LanguageManager.GetInstance().ConvertSpecifier("Byte");
            DataSize = _dataType + _size+ $" {_aoiDataType.ByteSize} "+ _byte;
            TabbedOptions.AddOptionPanels(_optionPanelDescriptors);

            Title = LanguageManager.GetInstance().ConvertSpecifier("InstructionDefinition")
                    + $" -{_aoiDefinition.Name} v{_aoiDefinition.Revision} {_aoiDefinition.ExtendedText}";
            IsEnable = false;
            WeakEventManager<LanguageManager,EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            Title = LanguageManager.GetInstance().ConvertSpecifier("InstructionDefinition")
                    + $" -{_aoiDefinition.Name} v{_aoiDefinition.Revision} {_aoiDefinition.ExtendedText}";
            _dataType = LanguageManager.GetInstance().ConvertSpecifier("Data Type");
            _size = LanguageManager.GetInstance().ConvertSpecifier("Size");
            _byte = LanguageManager.GetInstance().ConvertSpecifier("Byte");
            DataSize = _dataType + _size + $" {_aoiDataType.ByteSize} " + _byte;
        }

        public override void Cleanup()
        {
            PropertyChangedEventManager.RemoveHandler(_aoiDefinition, OnPropertyChanged, "");
            PropertyChangedEventManager.RemoveHandler((_aoiDefinition as AoiDefinition)?.datatype,
                OnDataTypePropertyChanged, "");
            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        public override bool IsClosing
        {
            set
            {
                _isClosing = value;

                if (_aoiDefinition.IsDeleted) return;

                (_optionPanelDescriptors[1].OptionPanel as ParametersViewModel).IsClosed = true;
                (_optionPanelDescriptors[2].OptionPanel as LocalTagsViewModel).IsClosed = true;
                (_optionPanelDescriptors[2].OptionPanel as LocalTagsViewModel).IsCloseCancel = false;
                //bool flag = false;
                MessageBoxResult result = MessageBoxResult.No;
                if (CanExecuteApplyCommand())
                {
                    var isDirty = false;
                    foreach (var optionPanelDescriptor in _optionPanelDescriptors)
                    {
                        var optionPanel = optionPanelDescriptor.OptionPanel;
                        var dirty = optionPanel as ICanBeDirty;
                        if(dirty!=null)
                        {
                            if (dirty.IsDirty)
                            {
                                isDirty = true;
                                break;
                            }
                        }
                    }

                    if (!isDirty) return;
                     result = MessageBox.Show("Apply changes?", "ICS Studio", MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Exclamation);

                    if (result == MessageBoxResult.Cancel)
                    {
                        _isClosing = false;
                        return;
                    }
                }

                if (result==MessageBoxResult.Yes)
                {
                    _isClosing = Check(true);
                    if (_isClosing)
                        DoApply();

                }
            }
            get { return _isClosing; }
        }

        public bool NeedReset { set; get; }
        
        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_aoiDefinition == sender && e.PropertyName == "IsSealed")
            {
                if (_aoiDefinition.IsSealed)
                    ExecuteApplyCommand();
                if (!_isCorrect)
                {
                    PropertyChangedEventManager.RemoveHandler(_aoiDefinition, OnPropertyChanged, "");
                    _aoiDefinition.IsSealed = false;
                    PropertyChangedEventManager.AddHandler(_aoiDefinition, OnPropertyChanged, "");
                    return;
                }
                
                {
                    (_optionPanelDescriptors[0].OptionPanel as GeneralViewModel)?.SetControlEnable();
                    (_optionPanelDescriptors[1].OptionPanel as ParametersViewModel)?.SetControlEnable();
                    (_optionPanelDescriptors[2].OptionPanel as LocalTagsViewModel)?.SetControlEnable();
                    (_optionPanelDescriptors[3].OptionPanel as ScanModesViewModel)?.SetControlEnable();
                    (_optionPanelDescriptors[5].OptionPanel as ChangeHistoryViewModel)?.SetControlEnable();
                    (_optionPanelDescriptors[6].OptionPanel as HelpViewModel).IsAllEnabled = !_aoiDefinition.IsSealed;
                }

                {
                    (_optionPanelDescriptors[0].OptionPanel as GeneralViewModel).IsDirty = false;
                    (_optionPanelDescriptors[1].OptionPanel as ParametersViewModel).IsDirty = false;
                    (_optionPanelDescriptors[2].OptionPanel as LocalTagsViewModel).IsDirty = false;
                    (_optionPanelDescriptors[3].OptionPanel as ScanModesViewModel).IsDirty = false;
                    (_optionPanelDescriptors[5].OptionPanel as ChangeHistoryViewModel).IsDirty = false;
                }
            }
        }
        
        protected override bool CanExecuteApplyCommand()
        {
            try
            {
                int i = 1;
                bool flag = false;
                foreach (IOptionPanelDescriptor descriptor in _optionPanelDescriptors)
                {
                    if (descriptor != null)
                    {
                        if (descriptor.HasOptionPanel)
                        {
                            var optionPanel = descriptor.OptionPanel;
                            ICanBeDirty dirty = optionPanel as ICanBeDirty;
                            if (dirty != null)
                            {
                                if (dirty.IsDirty)
                                {
                                    if (i == 2 || i == 3) DataSize = _dataType+" Size:?? byte(s)";
                                    flag = true;
                                }

                            }
                        }
                    }

                    i++;
                }

                if (flag)
                {
                    // set dirty
                    IProjectInfoService projectInfoService =
                        Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
                    projectInfoService?.SetProjectDirty();
                }

                return flag;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        protected override void ExecuteApplyCommand()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _isCorrect = true;
           

            if (!Check(true))
            {
                _isCorrect = false;
                return;
            }

            var paramList = (_optionPanelDescriptors[1].OptionPanel as ParametersViewModel)?.GetParamInfos();
            if (paramList == null||!_aoiDefinition.References.Any())
            {
                DoApply();
            }
            else
            {
                if ((_optionPanelDescriptors[1].OptionPanel as ParametersViewModel).IsSequenceChanged||paramList.Any(p => p.Item2||string.IsNullOrEmpty(p.Item1)))
                {
                    var dialog = new CheckDialog();
                    var vm = new CheckDialogViewModel(_aoiDefinition, paramList);
                    dialog.DataContext = vm;

                    var uiShell = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
                    if (dialog.ShowDialog(uiShell) ?? false)
                    {
                        IRoutine routine = null;
                        foreach (var referenceItem in vm.Items.OrderBy(item=>item.Routine).ThenByDescending(item=>item.Start))
                        {
                            if (routine==null)
                            {
                                routine = referenceItem.AoiDataReference.Routine;
                            }
                            else
                            {
                                if (routine != referenceItem.AoiDataReference.Routine)
                                {
                                    ((STRoutine) routine).IsUpdateChanged = true;
                                    routine = referenceItem.AoiDataReference.Routine;
                                }
                            }
                            referenceItem.UpdateInstrParam();
                        }
                        //((STRoutine)routine).UpdateChanged = true;
                        var st = routine as STRoutine;
                        if (st != null)
                            st.IsUpdateChanged = true;

                        DoApply();
                        if (vm.IsOpenCrossReference)
                        {
                            ICreateEditorService createEditorService =
                                Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
                            createEditorService?.CreateCrossReference(Type.AOI,null,_aoiDefinition.Name);
                        }
                    }
                }
                else
                {
                    DoApply();
                }
            }
        }

        private void DoApply()
        {
            var action = new Action(() =>
            {
                Controller.GetInstance().CanVerify = false;
                (_optionPanelDescriptors[1].OptionPanel as ParametersViewModel).RemoveListener();
                (_optionPanelDescriptors[2].OptionPanel as LocalTagsViewModel).RemoveListener();
                var tagColl = (TagCollection) _aoiDefinition.Tags;
                tagColl.IsNeedVerifyRoutine = false;
                tagColl.IsSaveTagNameChange = true;
                bool isDirty = false;
                if ((_optionPanelDescriptors[0].OptionPanel as GeneralViewModel).IsDirty)
                {
                    (_optionPanelDescriptors[0].OptionPanel as GeneralViewModel).Save();
                    isDirty = true;
                }
                if ((_optionPanelDescriptors[1].OptionPanel as ParametersViewModel).IsDirty)
                {
                    (_optionPanelDescriptors[1].OptionPanel as ParametersViewModel).Save();
                    isDirty = true;
                }
                if ((_optionPanelDescriptors[2].OptionPanel as LocalTagsViewModel).IsDirty)
                {
                    (_optionPanelDescriptors[2].OptionPanel as LocalTagsViewModel).Save();
                    isDirty = true;
                }
                if ((_optionPanelDescriptors[3].OptionPanel as ScanModesViewModel).IsDirty)
                {
                    (_optionPanelDescriptors[3].OptionPanel as ScanModesViewModel).Save();
                    isDirty = true;
                }
                if ((_optionPanelDescriptors[5].OptionPanel as ChangeHistoryViewModel).IsDirty)
                {
                    (_optionPanelDescriptors[5].OptionPanel as ChangeHistoryViewModel).Save();
                    isDirty = true;
                }
                if ((_optionPanelDescriptors[6].OptionPanel as HelpViewModel).IsDirty)
                {
                    (_optionPanelDescriptors[6].OptionPanel as HelpViewModel).Save();
                    isDirty = true;
                }
                tagColl.IsNeedVerifyRoutine = true;
                tagColl.ApplyTagNameChanged();
                Controller.GetInstance().CanVerify = true;
                if (NeedReset)
                {
                    var parameters = (_optionPanelDescriptors[1].OptionPanel as ParametersViewModel).GetOrderTags();
                    var locals = (_optionPanelDescriptors[2].OptionPanel as LocalTagsViewModel).GetOrderTags();
                    parameters.AddRange(locals);
                    ((TagCollection) _aoiDefinition.Tags).Order(parameters);
                    //_aoiDefinition?.Reset();
                    _aoiDefinition.datatype.Reset();
                    CodeSynchronization.GetInstance().Update();
                    IProjectInfoService projectInfoService =
                        Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
                    foreach (var aoiDefinitionRoutine in _aoiDefinition.Routines)
                    {
                           projectInfoService?.Verify(aoiDefinitionRoutine);
                    }

                    NeedReset = false;
                }

                if (IsChecked && !NeedReset)
                {
                    _aoiDefinition.datatype.CopyAllValueToTag = true;
                }

                (_optionPanelDescriptors[1].OptionPanel as ParametersViewModel).AddListener();
                (_optionPanelDescriptors[2].OptionPanel as LocalTagsViewModel).AddListener();
                DataSize = _dataType+$" Size:{(_aoiDefinition as AoiDefinition).datatype.ByteSize} byte(s)";
                //IStudioUIService studioUIService =
                //    Package.GetGlobalService(typeof(SStudioUIService)) as IStudioUIService;
                //studioUIService?.UpdateUI();
                (_optionPanelDescriptors[0].OptionPanel as GeneralViewModel).IsDirty = false;
                (_optionPanelDescriptors[1].OptionPanel as ParametersViewModel).IsDirty = false;
                (_optionPanelDescriptors[2].OptionPanel as LocalTagsViewModel).IsDirty = false;
                (_optionPanelDescriptors[3].OptionPanel as ScanModesViewModel).IsDirty = false;
                (_optionPanelDescriptors[5].OptionPanel as ChangeHistoryViewModel).IsDirty = false;
                (_optionPanelDescriptors[6].OptionPanel as HelpViewModel).IsDirty = false;
                IsEnable = false;
                _aoiDefinition.datatype.CopyAllValueToTag = false;
                if(isDirty)
                    _aoiDefinition.UpdateChangeHistory();
                PendingCompileRoutine.GetInstance().CompilePendingRoutines();
            });

            var dialog = new Wait(action)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();

        }

        internal bool Check(bool showTip)
        {
            //判断当前aoi的名字是否规范
            if (!(_optionPanelDescriptors[0].OptionPanel as GeneralViewModel).IsValidName())
            {
                (TabbedOptions.Items[0] as TabbedOptions.OptionTabPage).Focus();
                return false;
            }

            //判断aoi下的Parameters和LocalTag的名字是否规范
            #region check Parameters and LocalTag

            string errorMes1 = (_optionPanelDescriptors[1].OptionPanel as ParametersViewModel)?.ParametersRows.Child
                .FirstOrDefault(r => !r.IsCorrect)?.ErrorMessage;
            string errorRea1 = (_optionPanelDescriptors[1].OptionPanel as ParametersViewModel)?.ParametersRows.Child
                .FirstOrDefault(r => !r.IsCorrect)?.ErrorReason;
            string errorMes2 = (_optionPanelDescriptors[2].OptionPanel as LocalTagsViewModel)?.LocalTagRows.Child
                .FirstOrDefault(r => !r.IsCorrect)?.ErrorMessage;
            string errorRea2 = (_optionPanelDescriptors[2].OptionPanel as LocalTagsViewModel)?.LocalTagRows.Child
                .FirstOrDefault(r => !r.IsCorrect)?.ErrorReason;
            if (errorMes1 != null)
            {
                (TabbedOptions.Items[1] as TabbedOptions.OptionTabPage).Focus();
                if (showTip)
                {
                    ShowMessage(errorRea1, errorMes1);
                }

                _isClosing = false;
                return false;
            }
            else if (errorMes2 != null)
            {
                (TabbedOptions.Items[2] as TabbedOptions.OptionTabPage).Focus();
                if (showTip)
                {
                    ShowMessage(errorRea2, errorMes2);
                }

                _isClosing = false;
                return false;
            }

            List<string> names = new List<string>();
            bool isRepeat = false;
            bool isValid = true;
            string errorName = "";
            string warningMessage = "Failed to create a new tag.";
            string warningReason = LanguageManager.GetInstance().ConvertSpecifier("A Parameter or Local Tag by the same name already exists in this Add-On-Instruction Defined.");

            //对Parameters的命名判断逻辑
            foreach (var item in (_optionPanelDescriptors[1].OptionPanel as ParametersViewModel).ParametersRows.Child)
            {
                warningMessage = string.IsNullOrEmpty(item.OldName) ? "Failed to create a new tag." : $"Failed to set tag {item.OldName} name.";

                if (!item.IsMember) continue;
                if (!isValid || isRepeat || (string.IsNullOrEmpty(item.Name) &&
                                             (_optionPanelDescriptors[1].OptionPanel as
                                                 ParametersViewModel)
                                             .ParametersRows.Child.IndexOf(item) ==
                                             (_optionPanelDescriptors[1].OptionPanel as
                                                 ParametersViewModel)
                                             .ParametersRows.Child.Count - 1) ) continue;
                if (string.IsNullOrEmpty(item.Name))
                {
                    isValid = false;
                    warningReason = "Tag name is empty";
                    break;
                }

                if (item.Usage != Usage.InOut && !(item.DataType.Equals("BOOL", StringComparison.OrdinalIgnoreCase) ||
                                                   item.DataType.Equals("SINT", StringComparison.OrdinalIgnoreCase) ||
                                                   item.DataType.Equals("INT", StringComparison.OrdinalIgnoreCase) ||
                                                   item.DataType.Equals("DINT", StringComparison.OrdinalIgnoreCase) ||
                                                   item.DataType.Equals("LINT", StringComparison.OrdinalIgnoreCase) ||
                                                   item.DataType.Equals("REAL", StringComparison.OrdinalIgnoreCase)))
                {
                    isValid = false;
                    warningMessage = $"Failed to set tag \"${item.Name}\" usage.";
                    warningReason = "Input or ouput parameter must be BOOL,SINT,INT,DINT, or REAL data type.";
                }

                if (isValid)
                {
                    if (item.Name.Length > 40 || item.Name.EndsWith("_") ||
                        item.Name.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        isValid = false;
                        warningReason = "Name is invalid.";
                    }
                }

                // key word
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
                        if (keyWord.Equals(item.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            isValid = false;
                            warningReason = "Name is invalid.";
                        }
                    }
                }

                if (isValid)
                {
                    Regex regex = new Regex(@"^[a-zA-Z][a-zA-Z0-9_]*$");
                    if (item.Usage != Usage.InOut && !regex.IsMatch(item.Name))
                    {
                        isValid = false;
                        errorName = item.Name;
                    }
                }
                else continue;

                if (names.Contains(item.Name, StringComparer.OrdinalIgnoreCase)) isRepeat = true;
                else
                {
                    names.Add(item.Name);
                    if (item.Name.Equals("EnableIn", StringComparison.OrdinalIgnoreCase) ||
                        item.Name.Equals("EnableOut", StringComparison.OrdinalIgnoreCase) ||
                        item.DataType.Equals("BOOL")) continue;
                }
            }

            if (!isValid)
            {
                (TabbedOptions.Items[1] as TabbedOptions.OptionTabPage).Focus();
            }

            //对LocalTags的命名判断逻辑
            if (!isRepeat && isValid)
            {
                int Num = 0;
                foreach (var item in (_optionPanelDescriptors[2].OptionPanel as LocalTagsViewModel).LocalTagRows.Child)
                {
                    Num++;
                    if (!item.IsMember) continue;

                    if (!isValid || isRepeat || (string.IsNullOrEmpty(item.Name) &&
                                                 (_optionPanelDescriptors[2].OptionPanel as LocalTagsViewModel).LocalTagRows.Child.IndexOf(item) ==
                                                 (_optionPanelDescriptors[2].OptionPanel as LocalTagsViewModel).LocalTagRows.Child.Count - 1)) continue;

                    if (string.IsNullOrEmpty(item.Name))
                    {
                        isValid = false;
                        warningReason = "Tag Name is Empty.";
                        break;
                    }

                    if (isValid)
                    {
                        if (item.Name.Length > 40 || item.Name.EndsWith("_") ||
                            item.Name.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
                        {
                            isValid = false;
                            warningReason = "Name is invalid.";
                            break;
                        }
                    }

                    // key word
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
                            if (keyWord.Equals(item.Name, StringComparison.OrdinalIgnoreCase))
                            {
                                isValid = false;
                                warningReason = "Name is invalid.";
                                break;
                            }
                        }
                    }

                    if (isValid)
                    {
                        Regex regex = new Regex(@"^[a-zA-Z][a-zA-Z0-9_]*$");
                        if (!regex.IsMatch(item.Name))
                        {
                            isValid = false;
                            warningReason = "Tag name is invalid";
                            break;
                        }
                    }
                    else continue;

                    if (names.Contains(item.Name, StringComparer.OrdinalIgnoreCase)) isRepeat = true;
                    else
                    {
                        names.Add(item.Name);
                        if (item.DataType.Equals("BOOL")) continue;
                    }
                }
                if (!isValid)
                {
                    (TabbedOptions.Items[2] as TabbedOptions.OptionTabPage).Focus();
                }
            }
            if (isRepeat || !isValid)
            {
                if (!isValid)
                {
                    if (!warningReason.Equals("Tag Name is Empty."))
                    {
                        warningReason = errorName.IndexOf('[') > 0
                            ? "Number,size,or format of dimensions specified for this tag or type is invalid."
                            : "Name is invalid.";
                    }
                }

                if (showTip)
                {
                    var warningDialog =
                        new ICSStudio.Dialogs.Warning.WarningDialog(warningMessage, warningReason)
                            {Owner = Application.Current.MainWindow};
                    warningDialog.ShowDialog();
                }

                return false;
            }

            #endregion

            return true;
        }

        protected override void ExecuteOkCommand()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (CanExecuteApplyCommand())
                ExecuteApplyCommand();
            if (!_isCorrect) return;
            CloseAction?.Invoke();
        }

        protected override void ExecuteLogicCommand()
        {
            var createEditorService =
                Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
            IRoutine routine = (_aoiDefinition as AoiDefinition).Routines["Logic"];
            if (routine != null)
            {
                switch (routine.Type)
                {
                    case RoutineType.RLL:
                        createEditorService?.CreateRLLEditor(routine);
                        break;
                    case RoutineType.ST:
                        createEditorService?.CreateSTEditor(routine);
                        break;
                    case RoutineType.FBD:
                        createEditorService?.CreateFBDEditor(routine);
                        break;
                    //TODO(gjc): add other type
                }

            }

        }
        
        private void OnDataTypePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ByteSize")
            {
                DataSize = _dataType+$" Size:{(_aoiDefinition as AoiDefinition).datatype.ByteSize} byte(s)";
            }
        }

        private void ShowMessage(string reason, string message)
        {
            var dialog =
                new WarningDialog(reason, message) {Owner = Application.Current.MainWindow};
            dialog.ShowDialog();
        }

        public int Apply()
        {
            if (!Check(false)) return -1;
            DoApply();
            return 0;
        }

        public bool CanApply()
        {
            return CanExecuteApplyCommand();
        }
    }

    public class TmpTag
    {
        private readonly IAoiDefinition _aoi;
        public TmpTag(ITag tag, IAoiDefinition aoi)
        {
            _aoi = aoi;
            Tag = tag;
            TmpName = tag?.Name;
        }

        public Tag NewTag()
        {
            var newTag = new Tag((TagCollection) _aoi.Tags);
            Tag = newTag;
            return newTag;
        }

        public ITag Tag { get; private set; }
        public string TmpName { set; get; }
    }
}