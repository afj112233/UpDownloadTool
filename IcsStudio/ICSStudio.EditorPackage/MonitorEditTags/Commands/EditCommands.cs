using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Dialogs.ConfigDialogs;
using ICSStudio.EditorPackage.MonitorEditTags.Models;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIInterfaces.Editor;
using ICSStudio.Utils.CamEditorUtil;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Application = System.Windows.Application;
using TagItem = ICSStudio.EditorPackage.MonitorEditTags.Models.TagItem;
using Type = ICSStudio.UIInterfaces.Editor.Type;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.MultiLanguage;

namespace ICSStudio.EditorPackage.MonitorEditTags.Commands
{
    public static class EditCommands
    {
        public static RelayCommand<TagItem> ConfigCommand { get; } =
            new RelayCommand<TagItem>(ExecuteConfig, CanExecuteConfig);

        public static RelayCommand<TagItem> EditCommand { get; } =
            new RelayCommand<TagItem>(ExecuteEdit);

        public static RelayCommand<TagItem> PropertiesCommand { get; } =
            new RelayCommand<TagItem>(ExecuteProperties);

        public static RelayCommand<TagItem> DescriptionCommand { get; } =
            new RelayCommand<TagItem>(ExecuteDescription);

        public static RelayCommand<TagItem> DataTypeCommand { get; } =
            new RelayCommand<TagItem>(ExecuteDataType);

        public static RelayCommand<TagItem> CrossReferenceCommand { get; } =
            new RelayCommand<TagItem>(ExecuteCrossReference);

        public static RelayCommand<TagItem> FilterCommand { get; } =
            new RelayCommand<TagItem>(ExecuteFilter);

        public static RelayCommand<TagItem> MonitorCommand { get; } =
            new RelayCommand<TagItem>(ExecuteMonitor);

        [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
        private static void ExecuteMonitor(TagItem tagItem)
        {
            var createEditorService =
                Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;

            createEditorService?.CreateMonitorEditTags(tagItem.ParentCollection.Controller,
                tagItem.ParentCollection.Scope, tagItem.Name, false);

        }

        private static void ExecuteEdit(TagItem tagItem)
        {
            var createEditorService =
                Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;

            createEditorService?.CreateMonitorEditTags(tagItem.ParentCollection.Controller,
                tagItem.ParentCollection.Scope, tagItem.Name, true);
        }

        private static void ExecuteProperties(TagItem tagItem)
        {
            if (tagItem?.Tag == null)
                return;

            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            ICreateDialogService createDialogService =
                Package.GetGlobalService(typeof(SCreateDialogService)) as
                    ICreateDialogService;
            var dialogTagProperties =
                createDialogService?.CreateTagProperties(tagItem.Tag);
            dialogTagProperties?.Show(uiShell);
        }

        private static void ExecuteConfig(TagItem tagItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (tagItem.Tag.ParentCollection?.ParentProgram is AoiDefinition)
            {
                // in aoi
                if (tagItem.Tag.Usage == Usage.InOut)
                {
                    MonitorTagCollection monitorTagCollection = tagItem.ParentCollection as MonitorTagCollection;
                    if(monitorTagCollection?.DataContext?.AoiContext == null)
                        return;
                }
            }
            
            var dataTypeInfo = tagItem.DataTypeInfo;
            var typeName = dataTypeInfo.DataType.Name;

            if ((typeName.Equals("CAM", StringComparison.OrdinalIgnoreCase)
                || typeName.Equals("CAM_PROFILE", StringComparison.OrdinalIgnoreCase)) 
                && dataTypeInfo.Dim1 > 0)
            {
                var title = tagItem.Name;

                var transformTable =
                    tagItem.ParentCollection.DataContext?.TransformTable;

                CamEditorViewModel viewModel;
                ArrayField dataField;

                if (transformTable != null)
                {
                    var convertName = (string)transformTable[tagItem.Name.ToUpper()];
                    title = $"{title} <{convertName}>";

                    var tuple = ObtainValue.GetTagField(convertName,
                        tagItem.ParentCollection.DataContext.GetReferenceProgram(),
                        transformTable);
                    var field = tuple.Item1;
                    Debug.Assert(field is ArrayField);
                    dataField = field as ArrayField;
                    viewModel = new CamEditorViewModel(dataField, title, tuple.Item2);
                }
                else
                {
                    dataField = tagItem.DataField as ArrayField ?? tagItem.Tag.DataWrapper.Data as ArrayField;
                    viewModel = new CamEditorViewModel(dataField, title, tagItem.Tag);
                }

                CamEditorDialog dialog = new CamEditorDialog(viewModel)
                {
                    Owner = Application.Current.MainWindow
                };

                for (int i = 0; i < dataField?.fields.Count; i++)
                {
                    CAMField camField = dataField.fields[i].Item1 as CAMField;
                    CAM_PROFILEField camProfileField = dataField.fields[i].Item1 as CAM_PROFILEField;

                    if (camField != null)
                    {
                        CheckLinearOrCubic((SegmentType)((Int32Field)camField.fields[2].Item1).value, i);
                    }

                    if (camProfileField != null)
                    {
                        CheckLinearOrCubic((SegmentType)((Int32Field)camProfileField.fields[1].Item1).value, i);
                    }
                }

                dialog.ShowDialog();
            }

            else if (typeName.Equals("AXIS_CIP_DRIVE", StringComparison.OrdinalIgnoreCase))
            {

                ICreateDialogService createDialogService =
                    Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;
                var uiShell = (IVsUIShell)Package.GetGlobalService(typeof(SVsUIShell));

                var window =
                    createDialogService?.CreateAxisCIPDriveProperties(tagItem.Tag);
                window?.Show(uiShell);
            }

            else if (typeName.Equals("Motion_Group", StringComparison.OrdinalIgnoreCase))
            {
                var createDialogService =
                    (ICreateDialogService)Package.GetGlobalService(typeof(SCreateDialogService));

                var uiShell = (IVsUIShell)Package.GetGlobalService(typeof(SVsUIShell));

                var window =
                    createDialogService?.CreateMotionGroupProperties(tagItem.Tag);
                window?.Show(uiShell);
            }
            else if (typeName.Equals("PID", StringComparison.OrdinalIgnoreCase))
            {
                ArrayField field = (tagItem.Tag as Tag)?.DataWrapper.Data as ArrayField;
                PidSetupDialog pidSetupDialog = new PidSetupDialog(new PidSetUpViewModel(field, tagItem.Tag))
                {
                    Owner = Application.Current.MainWindow
                };
                pidSetupDialog.ShowDialog();
            }
            else if (typeName.Equals("MESSAGE", StringComparison.OrdinalIgnoreCase))
            {
                ITag tag = null;
                var name = "";
                if (tagItem.ParentCollection.DataContext != null)
                {
                    tag = ObtainValue.NameToTag(tagItem.Name,
                                  tagItem.ParentCollection.DataContext
                                      ?.GetFinalTransformTable())
                              ?.Item1;
                    name = $"{tagItem.Name} <{tag?.Name}>";
                }
                else
                {
                    tag = tagItem.Tag;
                    name = tagItem.Name;
                }

                Debug.Assert(tag != null);
                
                var title = $"Message {LanguageManager.GetInstance().ConvertSpecifier("Configuration")} - {name}";

                MessageConfigurationDialog dialog = new MessageConfigurationDialog(tag, title)
                {
                    Owner = Application.Current.MainWindow
                };

                dialog.ShowDialog();
            }
            else
            {
                //TODO(gjc): add code here
            }
        }

        private static void CheckLinearOrCubic(SegmentType segmentType, int num)
        {
            if (segmentType != SegmentType.Cubic && segmentType != SegmentType.Linear)
            {
                MessageBox.Show(@"Value out of range for Type at array index " + num, @"Cam Editor",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private static void ExecuteDataType(TagItem tagItem)
        {
            var createEditorService =
                Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;

            createEditorService?.CreateNewDataType(tagItem.Tag.DataTypeInfo.DataType);

        }

        private static void ExecuteDescription(TagItem tagItem)
        {
            //TODO(tlm):add code here
        }

        private static void ExecuteCrossReference(TagItem tagItem)
        {
            if (tagItem?.Tag == null)
                return;

            var createEditorService =
                Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;

            createEditorService?.CreateCrossReference(
                Type.Tag,
                tagItem.Tag.ParentCollection.ParentProgram,
                tagItem.Name);
        }

        private static void ExecuteFilter(TagItem obj)
        {
            //TODO(tlm):add code here
        }

        private static bool CanExecuteConfig(TagItem tagItem)
        {
            if (tagItem?.Tag == null)
                return false;

            if (tagItem.Tag.ParentCollection?.ParentProgram is AoiDefinition)
            {
                // in aoi
                if (tagItem.Tag.Usage == Usage.Local
                    || tagItem.Tag.Usage == Usage.Input
                    || tagItem.Tag.Usage == Usage.Output)
                    return true;

                if (tagItem.Tag.Usage == Usage.InOut)
                {
                    MonitorTagCollection monitorTagCollection = tagItem.ParentCollection as MonitorTagCollection;
                    return monitorTagCollection?.DataContext?.AoiContext != null;
                }
            }

            return true;
        }
    }
}
