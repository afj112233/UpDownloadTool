using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ICSStudio.Gui.Utils;
using ICSStudio.Ladder.Graph;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.Ladder;
using ICSStudio.Ladder.Controls;
using ICSStudio.Ladder.Graph.Styles;
using ICSStudio.MultiLanguage;
using ICSStudio.UIInterfaces.Dialog;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Type = ICSStudio.UIInterfaces.Editor.Type;

namespace ICSStudio.EditorPackage.RLLEditor
{
    /// <summary>
    ///     RLLEditorControl.xaml 的交互逻辑
    /// </summary>
    public partial class RLLEditorControl
    {
        public RLLEditorControl()
        {
            InitializeComponent();

            try
            {
                MenuItemTemplate = Resources["MyMenuItemTemplate"] as ControlTemplate;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message + ex.StackTrace);
            }

            RLLEditorViewModel.OnRemoveAdorner += RemoveAdorner;

            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        ~RLLEditorControl()
        {
            RLLEditorViewModel.OnRemoveAdorner -= RemoveAdorner;
        }

        public RLLEditorViewModel ViewModel { get; set; }

        private void RemoveAdorner(Adorner adorner)
        {
            if (adorner != ViewModel.BrowseAdorner && adorner != ViewModel.InputAdorner && adorner != ViewModel.EnumAdorner)
                return;

            HandleAdorner();
        }

        private void HandleAdorner()
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(MainGrid);
            if (adornerLayer != null)
            {
                var adorners = adornerLayer.GetAdorners(MainGrid);
                if (adorners?.Contains(ViewModel.EnumAdorner) == true)
                {
                    string name = ViewModel.EnumAdorner.GetName();
                    if (string.IsNullOrEmpty(name))
                        name = "?";
                    //if (!string.IsNullOrEmpty(name))
                    //{
                    UpdateOperand(name);
                    //}

                    adornerLayer.Remove(ViewModel.EnumAdorner);

                    Focus();
                }

                if (adorners?.Contains(ViewModel.BrowseAdorner) == true)
                {
                    string name = ViewModel.BrowseAdorner.GetName();

                    if (ViewModel.EditingControl == null)
                    {
                        bool isInstruction = !name.Equals("Rung", StringComparison.OrdinalIgnoreCase) &&
                                             !name.Equals("Branch", StringComparison.OrdinalIgnoreCase) &&
                                             !name.Equals("Branch Level", StringComparison.OrdinalIgnoreCase);
                        //插入功能块
                        if (isInstruction)
                            BoxInstructionStyleRenderer.InsertElement(ViewModel.Graph, name);
                    }
                    else
                    {
                        IInstruction instruction = null;

                        var selectedElement = ViewModel.EditingControl as VisualGroup;
                        if (selectedElement != null)
                        {
                            instruction = selectedElement.Model as IInstruction;
                        }

                        var selTextBlock = ViewModel.EditingControl as TextBlock;
                        if (selTextBlock != null && selTextBlock.Tag is IInstruction)
                        {
                            instruction = selTextBlock.Tag as IInstruction;
                        }

                        if (instruction != null)
                        {
                            string mnemonic = instruction.Mnemonic;

                            if (mnemonic?.Equals(name, StringComparison.OrdinalIgnoreCase) == false)
                            {
                                var newCode =
                                    BoxInstructionStyleRenderer.EditMnemonic(instruction, name.ToUpper());

                                if (!string.IsNullOrEmpty(newCode))
                                {
                                    ViewModel.UpdateGraphAndFocus(ViewModel.Graph, newCode, instruction.Offset);
                                    //ViewModel.UpdateGraph(ViewModel.Graph, newCode);
                                }
                            }
                        }
                        else// if (!string.IsNullOrEmpty(name))
                        {
                            if (string.IsNullOrEmpty(name))
                                name = "?";
                            UpdateOperand(name);
                        }
                    }

                    adornerLayer.Remove(ViewModel.BrowseAdorner);

                    Focus();
                }

                if (adorners?.Contains(ViewModel.InputAdorner) == true)
                {
                    var val = ViewModel.InputAdorner.InputContent;
                    var selectedTextBlock = ViewModel.EditingControl as TextBlock;

                    if (selectedTextBlock == null)
                    {
                        var rung = ViewModel.Graph.FocusedItem as IRung;
                        if (rung != null)
                        {
                            ViewModel.UpdateRungComment(rung.RungIndex, val);
                        }

                        var instruction = ViewModel.Graph.FocusedItem as IInstruction;
                        if (instruction != null)
                        {
                            ViewModel.UpdateTagDescription(instruction, val);
                        }
                    }

                    if (selectedTextBlock?.Tag is string)
                    {
                        string tag = selectedTextBlock.Tag as string;

                        if (ViewModel.ValidateValue(tag, val))
                        {
                            //更新Tag值
                            ViewModel.UpdateTagVal(tag, val);
                        }
                        else
                        {
                            ////Key非法的情况
                            //string message = "Source key is invalid.";
                            //string reason = "Source key must be 1-40 characters in length.";
                            //string errorCode = "Error 413-800420B9";

                            //WarningDialog dialog = new WarningDialog(message, reason, errorCode)
                            //{
                            //    Owner = Application.Current.MainWindow
                            //};

                            //dialog.ShowDialog();

                            MessageBox.Show(
                                $"Failed to modify the tag value.\nString invalid.",
                                "ICSStudio", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }

                    if (selectedTextBlock?.Tag is int)
                    {
                        if (!string.IsNullOrEmpty(val))
                        {
                            UpdateOperand(val);
                        }
                    }

                    if (selectedTextBlock?.Tag is IInstruction)
                    {
                        IInstruction instruction = selectedTextBlock.Tag as IInstruction;
                        string mnemonic = instruction?.Mnemonic;

                        if (mnemonic?.Equals(val, StringComparison.OrdinalIgnoreCase) == false)
                        {
                            var newCode =
                                BoxInstructionStyleRenderer.EditMnemonic(instruction, val.ToUpper());

                            if (!string.IsNullOrEmpty(newCode))
                            {
                                ViewModel.UpdateGraph(ViewModel.Graph, newCode);
                            }
                            //if (newCode != null)
                            //{
                            //    ViewModel.Routine.UpdateRungs(newCode);
                            //    ViewModel.UpdateLadderGraph();
                            //}
                        }
                    }

                    if (ViewModel.EditingControl is VisualGroup)
                    {
                        var group = ViewModel.EditingControl as VisualGroup;
                        IInstruction instruction = group?.Model as IInstruction;
                        string mnemonic = instruction?.Mnemonic;

                        if (mnemonic?.Equals(val, StringComparison.OrdinalIgnoreCase) == false)
                        {
                            string aoi = ViewModel.GetAOI(val);

                            var newCode =
                                BoxInstructionStyleRenderer.EditMnemonic(instruction,
                                    aoi ?? val?.ToUpper());

                            if (!string.IsNullOrEmpty(newCode))
                            {
                                ViewModel.UpdateGraph(ViewModel.Graph, newCode);
                            }
                            //if (newCode != null)
                            //{
                            //    ViewModel.Routine.UpdateRungs(newCode);
                            //    ViewModel.UpdateLadderGraph();
                            //}
                        }
                    }

                    adornerLayer.Remove(ViewModel.InputAdorner);

                    Focus();
                }
            }
        }

        private Func<DependencyObject, DependencyObject> _getParent = VisualTreeHelper.GetParent;

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            ViewModel.RefreshExecutable();

            if (e.RightButton == MouseButtonState.Pressed)
            {
                var tb = e.OriginalSource as TextBlock;

                if (tb != null)
                {
                    double result;
                    if (double.TryParse(tb.Text, out result))
                        return;

                    if (tb.Tag is int)
                    {
                        DependencyObject parent = _getParent(tb);
                        var group = parent as VisualGroup;
                        if (group == null)
                        {
                            for (int i = 0; i < 6; ++i)
                            {
                                parent = _getParent(parent);
                            }

                            group = parent as VisualGroup;
                        }

                        IInstruction instruction = group?.Model as IInstruction;
                        var index = (int)tb.Tag;
                        InstructionParameter para = null;
                        if (instruction != null)
                            para = instruction.Definition.Parameters.Where(p =>
                                !p.Type.Contains("Target") ||
                                (p.Type == "ReadWrite_DataTarget" && p.DataType == "SINT")).ToList()[index];

                        string tagName = tb.Text;
                        var operand = ViewModel.GetOperand(tagName);

                        bool isExisted = (operand != null);

                        string interRelated =
                            (instruction?.Mnemonic.Equals("GSV", StringComparison.OrdinalIgnoreCase) == true ||
                             instruction?.Mnemonic.Equals("SSV", StringComparison.OrdinalIgnoreCase) == true)
                                ? instruction.Parameters[0]
                                : null;
                        bool isEnum =
                            SimpleServices.Instruction.Utils.ParseEnum(instruction?.Mnemonic, index, tagName,
                                interRelated) >= 0; //false;

                        if (!isExisted)
                        {
                            if (para?.DataType.Equals("routine", StringComparison.OrdinalIgnoreCase) == true)
                            {
                                if (ViewModel.Routine.ParentCollection[tagName] != null)
                                    isExisted = true;
                            }
                        }

                        ContextMenu menu =
                            GetParameterContextMenu(tagName, operand, isExisted, isEnum, para, instruction);
                        if (menu.Items.Count > 0)
                            menu.IsOpen = true;
                        return;
                    }
                }

                var dp = e.OriginalSource as DependencyObject;
                var vg = FindVisualGroup(dp);
                if (vg != null)
                {
                    //处理Rung右键菜单
                    var rung = vg.Model as IRung;
                    if (rung != null)
                    {
                        PopRungMenu(rung);
                        return;
                    }

                    var branch = vg.Model as IBranch;
                    if (branch != null)
                    {
                        PopBranchMenu(branch);
                        return;
                    }

                    var instruction = vg.Model as IInstruction;
                    if (instruction != null)
                    {
                        PopInstructionMenu(instruction);
                    }
                }
            }
        }

        private VisualGroup FindVisualGroup(DependencyObject obj)
        {
            var group = obj as VisualGroup;
            if (group != null)
            {
                return group;
            }

            if (obj is CanvasControl)
                return null;

            var parent = _getParent(obj);
            return FindVisualGroup(parent);
        }

        private void PopInstructionMenu(IInstruction instruction)
        {
            var graph = ViewModel.Graph;
            var contextMenu = new ContextMenu();

            var item = new MenuItem();
            item.Header = LanguageManager.GetInstance().ConvertSpecifier("Cut Instruction");
            item.InputGestureText = "Ctrl + X";
            item.Click += (sender, e) => ViewModel.ExecuteCut();
            contextMenu.Items.Add(item);

            item = new MenuItem();
            item.Header = LanguageManager.GetInstance().ConvertSpecifier("Copy Instruction");
            item.InputGestureText = "Ctrl + C";
            item.Click += (sender, e) => ViewModel.ExecuteCopy();
            contextMenu.Items.Add(item);

            item = new MenuItem();
            item.Header = LanguageManager.GetInstance().ConvertSpecifier("Paste");
            item.InputGestureText = "Ctrl + V";
            item.Click += (sender, e) => ViewModel.ExecutePaste();
            contextMenu.Items.Add(item);

            contextMenu.Items.Add(new Separator());

            item = new MenuItem();
            item.Header = LanguageManager.GetInstance().ConvertSpecifier("Delete Instruction");
            item.Click += (source, ev) => ViewModel.ClickMenu(graph, MenuItemType.Delete);
            item.InputGestureText = "Del";
            var rung = instruction.GetParentRung();
            item.IsEnabled = (rung.EditState == EditState.Normal || rung.EditState == EditState.Edit || rung.EditState == EditState.Insert);
            contextMenu.Items.Add(item);

            item = new MenuItem();
            item.Header = LanguageManager.GetInstance().ConvertSpecifier("Edit Main Operand Description");
            item.InputGestureText = "Ctrl + D";
            //TODO(Ender):Main Operand为常数时，应为false
            item.IsEnabled = instruction.Definition.Parameters.Exists(p => p.IsMainOperand);
            item.Click += (sender, e) => ViewModel.ExecuteEditMainOperandDescription(instruction);
            contextMenu.Items.Add(item);

            if (ViewModel.IsAOI(instruction.Mnemonic))
            {
                item = new MenuItem();
                contextMenu.Items.Add(new Separator());
                item.Header = LanguageManager.GetInstance().ConvertSpecifier("Open Instruction Logic");
                item.Click += (sender, e) => ViewModel.OpenAOILogic(instruction.Mnemonic);
                contextMenu.Items.Add(item);
            }

            contextMenu.IsOpen = true;
        }

        private void PopBranchMenu(IBranch branch)
        {
            var graph = ViewModel.Graph;
            var contextMenu = new ContextMenu();

            MenuItem menuItem = new MenuItem();
            menuItem.Header = LanguageManager.GetInstance().ConvertSpecifier("Cut Branch");
            menuItem.InputGestureText = "Ctrl+X";
            menuItem.Click += (source, ev) => ViewModel.ClickMenu(graph, MenuItemType.Cut);
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = LanguageManager.GetInstance().ConvertSpecifier("Copy Branch");
            menuItem.InputGestureText = "Ctrl+C";
            menuItem.Click += (source, ev) => ViewModel.ClickMenu(graph, MenuItemType.Copy);
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = LanguageManager.GetInstance().ConvertSpecifier("Paste");
            menuItem.InputGestureText = "Ctrl+V";
            menuItem.Click += (source, ev) => ViewModel.ClickMenu(graph, MenuItemType.Paste);
            contextMenu.Items.Add(menuItem);

            contextMenu.Items.Add(new Separator());

            menuItem = new MenuItem();
            menuItem.Header = LanguageManager.GetInstance().ConvertSpecifier("Delete Branch");
            menuItem.InputGestureText = "Del";
            menuItem.Click += (source, ev) => ViewModel.ClickMenu(graph, MenuItemType.Delete);
            contextMenu.Items.Add(menuItem);

            contextMenu.Items.Add(new Separator());

            menuItem = new MenuItem();
            menuItem.Header = LanguageManager.GetInstance().ConvertSpecifier("Append New Level");
            menuItem.Click += (source, ev) => ViewModel.AddBranchLevel(branch);
            contextMenu.Items.Add(menuItem);

            contextMenu.IsOpen = true;
        }

        private void PopRungMenu(IRung rung)
        {
            var graph = ViewModel.Graph;
            var contextMenu = new ContextMenu();

            MenuItem menuItem = new MenuItem();
            menuItem.Header = LanguageManager.GetInstance().ConvertSpecifier("Cut Rung");
            menuItem.Click += (source, ev) => ViewModel.ClickMenu(graph, MenuItemType.Cut);
            //LadderDiagramService.OnClickMenu?.Invoke(graph, MenuItemType.Cut);
            menuItem.InputGestureText = "Ctrl+X";
            menuItem.IsEnabled = (rung.EditState == EditState.Normal || rung.EditState == EditState.Edit || rung.EditState == EditState.Insert);//|| rung.EditState == EditState.EditAcceptEdit || rung.EditState == EditState.AcceptEdit || rung.EditState == EditState.AcceptInsert
            if (rung.IsEndRung)
                menuItem.IsEnabled = false;
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = LanguageManager.GetInstance().ConvertSpecifier("Copy Rung");
            menuItem.Click += (source, ev) => ViewModel.ClickMenu(graph, MenuItemType.Copy);
            //LadderDiagramService.OnClickMenu?.Invoke(graph, MenuItemType.Copy);
            menuItem.InputGestureText = "Ctrl+C";
            if (rung.IsEndRung)
                menuItem.IsEnabled = false;
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = LanguageManager.GetInstance().ConvertSpecifier("Paste");
            menuItem.Click += (source, ev) => ViewModel.ClickMenu(graph, MenuItemType.Paste);
            //LadderDiagramService.OnClickMenu?.Invoke(graph, MenuItemType.Paste);
            menuItem.InputGestureText = "Ctrl+V";
            menuItem.IsEnabled =
            (rung.EditState != EditState.Edit);//&& rung.EditState != EditState.AcceptEdit && rung.EditState != EditState.EditAcceptEdit && rung.EditState != EditState.DeleteAcceptEdit && rung.EditState != EditState.EditAcceptEditOriginal
            if (rung.IsEndRung)
                menuItem.IsEnabled = false; contextMenu.Items.Add(menuItem);
            contextMenu.Items.Add(new Separator());

            menuItem = new MenuItem();
            menuItem.Header = LanguageManager.GetInstance().ConvertSpecifier("Delete Rung");
            menuItem.Click += (source, ev) => ViewModel.ClickMenu(graph, MenuItemType.Delete);
            //LadderDiagramService.OnClickMenu?.Invoke(graph, MenuItemType.Delete);
            menuItem.InputGestureText = "Del";
            menuItem.IsEnabled = (rung.EditState == EditState.Normal || rung.EditState == EditState.Edit || rung.EditState == EditState.Insert);// || rung.EditState == EditState.EditAcceptEdit || rung.EditState == EditState.AcceptEdit || rung.EditState == EditState.AcceptInsert
            if (rung.IsEndRung)
                menuItem.IsEnabled = false;
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = LanguageManager.GetInstance().ConvertSpecifier("Add Rung");
            menuItem.Click += (source, ev) =>
            {
                if (rung.IsEndRung)
                    BoxInstructionStyleRenderer.InsertElement(graph, "Rung");
                else
                    ViewModel.ClickMenu(graph, MenuItemType.Add);
                //LadderDiagramService.OnClickMenu?.Invoke(graph, MenuItemType.Add);
            };
            menuItem.InputGestureText = "Ctrl+R";
            menuItem.IsEnabled =
            (rung.EditState != EditState.Edit);// && rung.EditState != EditState.AcceptEdit && rung.EditState != EditState.EditAcceptEdit && rung.EditState != EditState.DeleteAcceptEdit && rung.EditState != EditState.EditAcceptEditOriginal
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = LanguageManager.GetInstance().ConvertSpecifier("Edit Rung");
            menuItem.InputGestureText = "Enter";
            if (rung.IsEndRung)
            {
                menuItem.Click += (source, ev) =>
                    BoxInstructionStyleRenderer.InsertElement(graph, "Rung");
                //BoxInstructionStyleRenderer.OnClickMenu?.Invoke(graph, MenuItemType.Add);
                menuItem.IsEnabled = true;
            }
            else
            {
                menuItem.Click += (source, ev) => ViewModel.ClickMenu(graph, MenuItemType.Edit);
                //LadderDiagramService.OnClickMenu?.Invoke(graph, MenuItemType.Edit);
                menuItem.IsEnabled = ((!graph.IsOnline && rung.EditState == EditState.Normal) || rung.EditState == EditState.Edit || rung.EditState == EditState.Insert);// || rung.EditState == EditState.EditAcceptEdit
            }
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = LanguageManager.GetInstance().ConvertSpecifier("Edit Rung Comment");
            menuItem.InputGestureText = "Ctrl+D";
            menuItem.Click += (source, ev) => ViewModel.ClickMenu(graph, MenuItemType.EditComment);//LadderDiagramService.OnClickMenu?.Invoke(graph, MenuItemType.EditComment);
            menuItem.IsEnabled = !rung.IsEndRung;
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = LanguageManager.GetInstance().ConvertSpecifier("Import Rungs...");
            menuItem.Click += (source, ev) => ViewModel.ClickMenu(graph, MenuItemType.ImportRungs);//LadderDiagramService.OnClickMenu?.Invoke(graph, MenuItemType.ImportRungs);
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = LanguageManager.GetInstance().ConvertSpecifier("Export Rungs...");
            menuItem.Click += (source, ev) => ViewModel.ClickMenu(graph, MenuItemType.ExportRungs);//LadderDiagramService.OnClickMenu?.Invoke(graph, MenuItemType.ExportRungs);
            menuItem.IsEnabled = !rung.IsEndRung;
            contextMenu.Items.Add(menuItem);

            contextMenu.Items.Add(new Separator());

            menuItem = new MenuItem();
            menuItem.Header = LanguageManager.GetInstance().ConvertSpecifier("Verify Rung");
            menuItem.Click += (source, ev) => ViewModel.ClickMenu(graph, MenuItemType.VerifyRung);//LadderDiagramService.OnClickMenu?.Invoke(graph, MenuItemType.VerifyRung);
            if (rung.IsEndRung || graph.IsOnline)
                menuItem.IsEnabled = false;
            contextMenu.Items.Add(menuItem);

            contextMenu.Opened += (sender, args) =>
            {
                LanguageManager.GetInstance().SetLanguage(contextMenu);
            };
            contextMenu.IsOpen = true;
        }

        bool IsEditor(object source)
        {
            Func<DependencyObject, DependencyObject> getParent = VisualTreeHelper.GetParent;
            FrameworkElement currentItem = source as FrameworkElement;

            while (currentItem != null)
            {
                if (currentItem is TextBox)
                    return true;
                if (currentItem.Name == "ButtonFinishEdit")
                    return true;

                currentItem = getParent(currentItem) as FrameworkElement;
            }
            return false;
        }

        private void RLLEditorControl_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel.LdtEditorVisible != Visibility.Collapsed && !IsEditor(e.OriginalSource))
                ViewModel.LdtEditorVisible = Visibility.Collapsed;

            HandleAdorner();

            //if (e.RightButton == MouseButtonState.Pressed)
            //{
            //    var tb = e.OriginalSource as TextBlock;

            //    if (tb == null)
            //        return;

            //    double result;
            //    if (double.TryParse(tb.Text, out result))
            //        return;

            //    if (tb.Tag is int)
            //    {
            //        Func<DependencyObject, DependencyObject> getParent = VisualTreeHelper.GetParent;
            //        DependencyObject parent = getParent(tb);
            //        var group = parent as VisualGroup;
            //        if (group == null)
            //        {
            //            for (int i = 0; i < 6; ++i)
            //            {
            //                parent = getParent(parent);
            //            }
            //            group = parent as VisualGroup;
            //        }
            //        IInstruction instruction = group?.Model as IInstruction;
            //        var index = (int)tb.Tag;
            //        InstructionParameter para = null;
            //        if (instruction != null)
            //            para = instruction.Definition.Parameters.Where(p => !p.Type.Contains("Target") || (p.Type == "ReadWrite_DataTarget" && p.DataType == "SINT")).ToList()[index];

            //        string tagName = tb.Text;
            //        var operand = ViewModel.GetOperand(tagName);

            //        bool isExisted = (operand != null);
            //        if (!isExisted)
            //        {
            //            if (para?.DataType.Equals("routine", StringComparison.OrdinalIgnoreCase) == true)
            //            {
            //                if (ViewModel.Routine.ParentCollection[tagName] != null)
            //                    isExisted = true;
            //            }
            //        }

            //        ContextMenu menu = GetParameterContextMenu(tagName, operand, isExisted, para);
            //        if (menu.Items.Count > 0)
            //            menu.IsOpen = true;
            //    }
            //}
        }

        public void EditProperties(ITag tag)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            ICreateDialogService createDialogService =
                Package.GetGlobalService(typeof(SCreateDialogService)) as
                    ICreateDialogService;
            var dialog = createDialogService?.CreateTagProperties(tag);
            dialog?.Show(uiShell);
        }

        private void EditRoutineProperties(IRoutine routine)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            ICreateDialogService createDialogService =
                Package.GetGlobalService(typeof(SCreateDialogService)) as
                    ICreateDialogService;
            var dialog = createDialogService?.CreateRoutineProperties(routine);
            dialog?.Show(uiShell);
        }

        private ControlTemplate MenuItemTemplate;

        private Type GetCrossType(InstructionParameter para)
        {
            if (para.Formats?.Contains(FormatType.Tag) == true || para.Formats?.Contains(FormatType.TagArray) == true)
                return Type.Tag;

            if (para.Formats?.Contains(FormatType.Routine) == true)
                return Type.Routine;

            if (para.Formats?.Contains(FormatType.Label) == true)
                return Type.Label;

            if (para.Formats?.Contains(FormatType.Phase) == true)
                return Type.EquipmentPhase;

            return Type.None;
            //throw new NotSupportedException("Type Not Support Go Across Function.");
        }

        private ContextMenu GetParameterContextMenu(string tag, IDataOperand operand, bool isExisted, bool isEnum, InstructionParameter para, IInstruction instruction)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var graph = ViewModel.Graph;
            var contextMenu = new ContextMenu();

            bool isSysFlag = tag.Equals("s:fs", StringComparison.OrdinalIgnoreCase);
            var type = GetCrossType(para);

            MenuItem item = new MenuItem();

            if (!isSysFlag && !isEnum)
            {
                //TODO(Ender):解析数组索引为表达式的Tag

                if (isExisted)
                {
                    if (operand != null)
                    {
                        item.Header = $"{LanguageManager.GetInstance().ConvertSpecifier("RLLEditor.Edit")}" +
                                      $"\"{tag}\"{LanguageManager.GetInstance().ConvertSpecifier("RLLEditor.Properties")}";
                        item.Template = MenuItemTemplate;
                        item.Click += (sender, e) => EditProperties(operand.Tag);
                        contextMenu.Items.Add(item);

                        item = new MenuItem();
                        item.Header = $"{LanguageManager.GetInstance().ConvertSpecifier("RLLEditor.Goto")}" +
                                      $"\"{tag}\"{LanguageManager.GetInstance().ConvertSpecifier("RLLEditor.CrossReference")}";
                        item.Template = MenuItemTemplate;
                        item.Click += (sender, e) => ViewModel.GotoReference(tag, type);
                        contextMenu.Items.Add(item);

                        item = new MenuItem();
                        item.Tag = tag;
                        item.Header = $"{LanguageManager.GetInstance().ConvertSpecifier("RLLEditor.Monitor")}" +
                                      $"\"{tag}\"";
                        item.Template = MenuItemTemplate;
                        item.Click += (sender, e) => Monitor_Click(tag);
                        contextMenu.Items.Add(item);
                        //if (para?.DataType.Equals("routine", StringComparison.OrdinalIgnoreCase) == false)
                        //{
                        //contextMenu.Items.Add(new Separator());
                        //}
                    }
                    else if (para?.DataType.Equals("routine", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        var routine = ViewModel.Routine.ParentCollection[tag];
                        item.Header = $"{LanguageManager.GetInstance().ConvertSpecifier("RLLEditor.Edit")}" +
                                      $"\"{tag}\"{LanguageManager.GetInstance().ConvertSpecifier("RLLEditor.Properties")}";
                        item.Template = MenuItemTemplate;
                        item.Click += (sender, e) => EditRoutineProperties(routine);
                        contextMenu.Items.Add(item);

                        item = new MenuItem();
                        item.Header = $"{LanguageManager.GetInstance().ConvertSpecifier("RLLEditor.Goto")}" +
                                      $"\"{tag}\"{LanguageManager.GetInstance().ConvertSpecifier("RLLEditor.CrossReference")}";
                        item.Template = MenuItemTemplate;
                        item.Click += (sender, e) => ViewModel.GotoReference(tag, type);
                        contextMenu.Items.Add(item);
                    }
                }
                else
                {
                    if (para.DataType.Equals("label", StringComparison.OrdinalIgnoreCase))
                    {
                        item = new MenuItem();
                        item.Header = $"{LanguageManager.GetInstance().ConvertSpecifier("RLLEditor.Goto")}" +
                                      $"\"{tag}\"{LanguageManager.GetInstance().ConvertSpecifier("RLLEditor.CrossReference")}";
                        item.Template = MenuItemTemplate;
                        item.Click += (sender, e) => ViewModel.GotoReference(tag, type);
                        contextMenu.Items.Add(item);
                    }
                    else
                    {
                        var simpleTag = ViewModel.GetSimpleTag(tag);
                        item.Header = $"{LanguageManager.GetInstance().ConvertSpecifier("RLLEditor.New")}" +
                                      $"\"{simpleTag}\"";
                        item.Template = MenuItemTemplate;
                        item.InputGestureText = @"Ctrl + W";
                        string dataType = simpleTag == tag ? para.DataType : "DINT";
                        item.Click += (sender, e) => NewTag_Click(simpleTag, tag, dataType);
                        contextMenu.Items.Add(item);
                    }
                }
                contextMenu.Items.Add(new Separator());
            }

            item = new MenuItem();
            item.Header = LanguageManager.GetInstance().ConvertSpecifier("Cut Instruction");
            item.InputGestureText = "Ctrl + X";
            item.Click += (sender, e) => ViewModel.ExecuteCut();
            contextMenu.Items.Add(item);

            item = new MenuItem();
            item.Header = LanguageManager.GetInstance().ConvertSpecifier("Copy Instruction");
            item.InputGestureText = "Ctrl + C";
            item.Click += (sender, e) => ViewModel.ExecuteCopy();
            contextMenu.Items.Add(item);

            item = new MenuItem();
            item.Header = LanguageManager.GetInstance().ConvertSpecifier("Paste");
            item.InputGestureText = "Ctrl + V";
            item.Click += (sender, e) => ViewModel.ExecutePaste();
            contextMenu.Items.Add(item);

            contextMenu.Items.Add(new Separator());

            item = new MenuItem();
            item.Header = LanguageManager.GetInstance().ConvertSpecifier("Delete Instruction");
            item.Click += (source, ev) => ViewModel.ClickMenu(graph, MenuItemType.Delete);
            item.InputGestureText = "Del";
            var rung = instruction.GetParentRung();
            item.IsEnabled = (rung.EditState == EditState.Normal || rung.EditState == EditState.Edit || rung.EditState == EditState.Insert);
            contextMenu.Items.Add(item);

            item = new MenuItem();
            item.Header = LanguageManager.GetInstance().ConvertSpecifier("Edit Main Operand Description");
            item.InputGestureText = "Ctrl + D";
            //TODO(Ender):Main Operand为常数时，应为false
            item.IsEnabled = instruction.Definition.Parameters.Exists(p => p.IsMainOperand);
            item.Click += (sender, e) => ViewModel.ExecuteEditMainOperandDescription(instruction);
            contextMenu.Items.Add(item);

            if (ViewModel.IsAOI(instruction.Mnemonic))
            {
                item = new MenuItem();
                contextMenu.Items.Add(new Separator());
                item.Header = LanguageManager.GetInstance().ConvertSpecifier("Open Instruction Logic");
                item.Click += (sender, e) => ViewModel.OpenAOILogic(instruction.Mnemonic);
                contextMenu.Items.Add(item);
            }

            if (para?.DataType.Equals("Bool", StringComparison.OrdinalIgnoreCase) == true)
            {
                contextMenu.Items.Add(new Separator());
                item = new MenuItem();
                item.Header = LanguageManager.GetInstance().ConvertSpecifier("Toggle Bit");
                item.InputGestureText = @"Ctrl + T";
                item.Click += (sender, e) => ToggleBit_Click(tag);
                item.IsEnabled = isExisted;

                if (operand != null && operand.FormattedValueString != "0" && operand.FormattedValueString != "1")
                    item.IsEnabled = false;

                contextMenu.Items.Add(item);
            }

            contextMenu.Opened += (sender, args) =>
            {
                LanguageManager.GetInstance().SetLanguage(contextMenu);
            };

            return contextMenu;
        }

        private void NewTag_Click(string tag, string operandName, string dataType)
        {
            ViewModel.NewTag(tag, operandName, dataType);
        }

        private void Monitor_Click(string tag)
        {
            ViewModel.MonitorTag(tag);
        }

        private void ToggleBit_Click(string tag)
        {
            ViewModel.ToggleBit(tag);
        }

        private void UpdateOperand(string val)
        {
            var tb = ViewModel.EditingControl as TextBlock;
            //var selElement = Ladder.Graph.Styles.BoxInstructionStyleRenderer.focusElement;
            var model = ViewModel.Graph.FocusedItem as IInstruction;//selElement?.Model as IInstruction;
            if (model != null && tb?.Tag != null)
            {
                var oldRungs = ViewModel.Routine.CloneRungs();
                int parIndex = (int)tb.Tag;
                if (parIndex < model.Parameters.Count)
                {
                    model.Parameters[parIndex] = val;
                    if ((model.Mnemonic == "GSV" || model.Mnemonic == "SSV") && parIndex == 0 && val == "Controller")
                    {
                        model.Parameters[1] = "";
                    }
                    //tb.Text = val;
                    //TODO:更新Routine
                    var rung = model.GetParentRung();
                    var rungs = ViewModel.Routine.Rungs;
                    rungs[rung.RungIndex].Text = Ladder.Utils.LadderHelper.GetText(rung);// = new RungType { Text = Ladder.Utils.LadderHelper.GetText(rung) };

                    FocusInfo info = new FocusInfo { RungIndex = model.GetParentRung().RungIndex, Offset = model.Offset };
                    ViewModel.UpdateLadderGraph(info);
                    //ViewModel.UpdateLadderGraph();
                    ViewModel.RecordChange(oldRungs);
                    ViewModel.RegisterOperand();
                }
            }
        }

        public static void MarkCurCanvasControl(CanvasControl canvasControl)
        {
            LadderDiagramService.CurGram = canvasControl;
        }

        public static void InsertElement(string mnemonic)
        {
            if (LadderDiagramService.CurGram == null)
                return;

            BoxInstructionStyleRenderer.InsertElement(((LadderControl)LadderDiagramService.CurGram)?.Graph, mnemonic);
        }

        public static void ClearDropPoints()
        {
            BoxInstructionStyleRenderer.HideDropLayer();
        }

        private void ContentPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //不应该清掉所有缓存，只应该清掉落点
            BoxInstructionStyleRenderer.HideDropLayer(true, ViewModel.BottomControl as CanvasControl);
            BoxInstructionStyleRenderer.HideDropLayer(true, ViewModel.TopControl as CanvasControl);
        }
    }
}