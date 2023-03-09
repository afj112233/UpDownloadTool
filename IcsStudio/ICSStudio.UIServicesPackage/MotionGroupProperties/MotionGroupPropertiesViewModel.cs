using System;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Interfaces.Tags;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using ICSStudio.Cip.Objects;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.UIServicesPackage.MotionGroupProperties.Panel;
using Microsoft.VisualStudio.Shell;


namespace ICSStudio.UIServicesPackage.MotionGroupProperties
{
    public class MotionGroupPropertiesViewModel : TabbedOptionsDialogViewModel, ICanApply
    {
        private readonly List<IOptionPanelDescriptor> _optionPanelDescriptors;
        private readonly IController _controller;
        private bool _isValid;
        private bool? _dialogResult;
        private string MotionGroupProperties;

        public MotionGroupPropertiesViewModel(ITag motionGroup)
        {
            if (motionGroup == null || !motionGroup.DataTypeInfo.DataType.IsMotionGroupType)
                throw new ArgumentOutOfRangeException();

            MotionGroup = motionGroup;
            _controller = motionGroup.ParentController;
            _isValid = true;
            _optionPanelDescriptors = new List<IOptionPanelDescriptor>
            {
                new DefaultOptionPanelDescriptor("1", "Axis Assignment", "Axis Assignment",
                    new AxisAssignmentPanelViewModel(new AxisAssignmentPanel(), MotionGroup), null),
                new DefaultOptionPanelDescriptor("2", "Attribute", "Attribute",
                    new AttributePanelViewModel(new AttributePanel(), MotionGroup), null),
                new DefaultOptionPanelDescriptor("3", "Variable", "Variable",
                    new TagPanelViewModel(new TagPanel(), MotionGroup), null)
            };

            TabbedOptions.AddOptionPanels(_optionPanelDescriptors);

            MotionGroupProperties = LanguageManager.GetInstance().ConvertSpecifier("MotionGroupProperties");

            Title = MotionGroupProperties+$" - {MotionGroup.Name}";

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);

        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            MotionGroupProperties = LanguageManager.GetInstance().ConvertSpecifier("MotionGroupProperties");

            Title = MotionGroupProperties + $" - {MotionGroup.Name}";
        }

        public override void Cleanup()
        {
            base.Cleanup();
            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        public ITag MotionGroup { get; }

        protected override bool CanExecuteApplyCommand()
        {
            try
            {
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
                                    // set dirty
                                    IProjectInfoService projectInfoService =
                                        Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
                                    projectInfoService?.SetProjectDirty();

                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return false;
        }

        protected override void ExecuteApplyCommand()
        {
            if (!Check()) return;
            DoApply();
        }

        private void DoApply()
        {
            Tag tag = MotionGroup as Tag;

            #region AxisAssignment

            AxisAssignmentPanelViewModel panel = _optionPanelDescriptors[0].OptionPanel as AxisAssignmentPanelViewModel;

            if (panel != null && panel.IsDirty)
            {
                List<Item> assignedList = new List<Item>();
                List<Item> unassignedList = new List<Item>();
                foreach (var item in panel.AssignedCollection)
                {
                    assignedList.Add(item);
                }

                foreach (var item in panel.UnassignedCollection)
                {
                    unassignedList.Add(item);
                }

                var assigned = assignedList.Except(panel.AssignedCollection2).ToList();
                var unassigned = unassignedList.Except(panel.UnassignedCollection2).ToList();

                foreach (var item in assigned)
                {
                    Tag axisTag = _controller.Tags[item.Tag.Name] as Tag;
                    AxisCIPDrive axisCIPDrive = axisTag?.DataWrapper as AxisCIPDrive;
                    if (axisCIPDrive != null)
                    {
                        axisCIPDrive.AssignedGroup = MotionGroup;
                    }

                    AxisVirtual axisVirtual = axisTag?.DataWrapper as AxisVirtual;
                    if (axisVirtual != null)
                    {
                        axisVirtual.AssignedGroup = MotionGroup;
                    }
                }

                foreach (var item in unassigned)
                {
                    Tag axisTag = _controller.Tags[item.Tag.Name] as Tag;
                    AxisCIPDrive axisCIPDrive = axisTag?.DataWrapper as AxisCIPDrive;
                    if (axisCIPDrive != null)
                    {
                        axisCIPDrive.AssignedGroup = null;
                        axisCIPDrive.CIPAxis.AxisUpdateSchedule = (byte) AxisUpdateScheduleType.Base;
                    }


                    AxisVirtual axisVirtual = axisTag?.DataWrapper as AxisVirtual;
                    if (axisVirtual != null)
                    {
                        axisVirtual.AssignedGroup = null;
                        axisVirtual.CIPAxis.AxisUpdateSchedule = (byte) AxisUpdateScheduleType.Base;
                    }

                }

                panel.GetAxis();
                panel.Compare();
            }

            #endregion

            #region AttributePanel

            AttributePanelViewModel panel2 = _optionPanelDescriptors[1].OptionPanel as AttributePanelViewModel;
            if (panel2 != null && panel2.IsDirty)
            {
                MotionGroup mg = tag?.DataWrapper as MotionGroup;
                if (mg != null)
                {
                    mg.GeneralFaultType = panel2.Selected;
                    var coarseUpdatePeriod = (int) (float.Parse(panel2.BaseUpdateP) * 1000);
                    if (coarseUpdatePeriod != mg.CoarseUpdatePeriod)
                    {
                        mg.CoarseUpdatePeriod = coarseUpdatePeriod;
                        mg.Alternate1UpdateMultiplier = 1;
                        mg.Alternate2UpdateMultiplier = 1;
                    }
                }

                panel2.Compare();
            }

            #endregion

            #region TagPanelView

            TagPanelViewModel panel3 = _optionPanelDescriptors[2].OptionPanel as TagPanelViewModel;
            if (panel3 != null && panel3.IsDirty)
            {

                MotionGroup.Name = panel3.TagName;
                Title = $"Motion Group Properties - {MotionGroup.Name}";
                MotionGroup.Description = panel3.Description;
                panel3.Compare();
            }

            #endregion
        }

        private bool Check()
        {
            _isValid = true;
            TagPanelViewModel panel3 = _optionPanelDescriptors[2].OptionPanel as TagPanelViewModel;
            if (panel3 != null && panel3.IsDirty)
                _isValid = IsValidTagName(panel3.TagName);
            return _isValid;
        }

        protected override void ExecuteCancelCommand()
        {
            _dialogResult = false;
            CloseAction.Invoke();
        }

        protected override void ExecuteOkCommand()
        {
            if (CanExecuteApplyCommand())
                ExecuteApplyCommand();

            if (_isValid)
            {
                _dialogResult = true;
                CloseAction?.Invoke();
            }
        }

        protected override void ExecuteClosingCommand(CancelEventArgs args)
        {
            if (!_dialogResult.HasValue)
            {
                var tag = MotionGroup as Tag;
                if (tag != null && tag.IsDeleted)
                    return;

                if (CanExecuteApplyCommand())
                {
                    string message = "Apply changes?";
                    string caption = "ICS Studio";

                    var messageBoxResult = MessageBox.Show(message, caption, MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Exclamation);

                    if (messageBoxResult == MessageBoxResult.Cancel)
                    {
                        args.Cancel = true;
                    }
                    else if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        ExecuteApplyCommand();
                        if (!_isValid)
                            args.Cancel = true;
                    }
                }
            }
        }

        private bool IsValidTagName(string name)
        {
            string warningMessage = $"Failed to modify the properties for the tag '{MotionGroup.Name}'.";
            string warningReason = string.Empty;
            bool isValid = true;

            if (string.IsNullOrEmpty(name))
            {
                isValid = false;
                warningReason = "Tag name is empty.";
            }

            if (isValid)
            {
                if (name.Length > 40 || name.EndsWith("_") ||
                    name.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    isValid = false;
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("VariableNameInvalid");
                }
            }

            if (isValid)
            {
                Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
                if (!regex.IsMatch(name))
                {
                    isValid = false;
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("VariableNameInvalid");
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
                    "not", "mod", "and", "xor", "or"
                };
                foreach (var keyWord in keyWords)
                {
                    if (keyWord.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        isValid = false;
                        warningReason = LanguageManager.GetInstance().ConvertSpecifier("VariableNameInvalid");
                    }
                }
            }

            if (isValid)
            {
                var tag = MotionGroup.ParentController.Tags[name];
                if (tag != null && tag != MotionGroup)
                {
                    isValid = false;
                    warningReason = "Already exists.";
                }
            }



            if (!isValid)
            {
                var warningDialog = new WarningDialog(warningMessage, warningReason)
                    {Owner = Application.Current.MainWindow};
                warningDialog.ShowDialog();
            }

            return isValid;
        }

        private void CycleUpdateTimerHandle(object state, EventArgs e)
        {
            ApplyCommand.RaiseCanExecuteChanged();
        }

        public int Apply()
        {
            if (!Check()) return -1;
            DoApply();
            return 0;
        }

        public bool CanApply()
        {
            return CanExecuteApplyCommand();
        }
    }
}
