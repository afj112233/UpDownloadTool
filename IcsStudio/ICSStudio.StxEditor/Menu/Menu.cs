using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using ICSStudio.AvalonEdit;
using ICSStudio.AvalonEdit.Document;
using ICSStudio.Dialogs.AddSTElement;
using ICSStudio.Dialogs.ConfigDialogs;
using ICSStudio.Dialogs.GlobalSetting;
using ICSStudio.Dialogs.GoTo;
using ICSStudio.Dialogs.NewTag;
using ICSStudio.Dialogs.STDialogs;
using ICSStudio.Gui.Annotations;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.StxEditor.Interfaces;
using ICSStudio.StxEditor.ViewModel;
using ICSStudio.StxEditor.ViewModel.CodeSnippets;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIInterfaces.Editor;
using Microsoft.VisualStudio.Shell.Interop;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Instruction;
using ICSStudio.SimpleServices.Online;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.StxEditor.Menu.Dialog.View;
using ICSStudio.StxEditor.Menu.Dialog.ViewModel;
using ICSStudio.UIInterfaces.Error;
using ICSStudio.UIInterfaces.QuickWatch;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Type = ICSStudio.UIInterfaces.Editor.Type;
using ICSStudio.MultiLanguage;

namespace ICSStudio.StxEditor.Menu
{
    #region CommonMenu

    internal class GoToMenuItem : MenuItem
    {
        private readonly ITag _tag;
        private readonly string _tagName;
        private readonly TextEditor _editor;
        private readonly IRoutine _routine;
        private readonly IProgramModule _parentProgram;
        private readonly IProgramModule _program;

        public GoToMenuItem(TextEditor editor, IRoutine routine, string tagName, ITag tag, IProgramModule parentProgram,
            IProgramModule program)
        {
            Header = LanguageManager.GetInstance().ConvertSpecifier("Go To...");
            InputGestureText = "Ctrl + G";
            _tag = tag;
            _program = program;
            _tagName = tagName;
            _routine = routine;
            _editor = editor;
            _parentProgram = parentProgram;
            Click += GoToMenuItem_Click;
        }

        private void GoToMenuItem_Click(object sender, RoutedEventArgs e)
        {
            GoTo(_editor, _routine, _tagName, _tag, _parentProgram, _program);
        }

        public static void GoTo(TextEditor editor, IRoutine routine, string tagName, ITag tag,
            IProgramModule parentProgram,
            IProgramModule program)
        {
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            var dialog = new GotoDialog();
            var vm = new GoToDialogViewModel(editor.Document.GetLocation(editor.CaretOffset).Line, editor.LineCount,
                routine, tagName, tag, program);
            dialog.DataContext = vm;
            if ((bool) dialog.ShowDialog(uiShell))
            {
                if (vm.SelectedKind.Equals("Line"))
                {
                    if (vm.LineNumber.Equals("end", StringComparison.OrdinalIgnoreCase))
                    {
                        editor.CaretOffset = editor.Document.Lines[editor.LineCount - 1].Offset;
                    }
                    else
                    {
                        int line = int.Parse(vm.LineNumber) - 1;
                        editor.CaretOffset = editor.Document.Lines[line].Offset;
                        editor.ScrollToLine(line);
                    }
                }

                else if (vm.SelectedKind.Equals("Edit") || vm.SelectedKind.Equals("Monitor"))
                {
                    if ((string.IsNullOrEmpty(tagName) && tag == null))
                    {
                        //5000中没有显示任何窗口,这里显示routine属性界面
                        ICreateDialogService createDialogService =
                            Package.GetGlobalService(typeof(SCreateDialogService)) as
                                ICreateDialogService;
                        var propertiesDialog = createDialogService?.CreateRoutineProperties(routine);
                        propertiesDialog?.Show(uiShell);
                    }
                    else
                    {
                        ICreateEditorService createDialogService =
                            Package.GetGlobalService(typeof(SCreateEditorService)) as
                                ICreateEditorService;
                        //var programTmp = tag.ParentCollection.ParentProgram as IProgram;
                        //var tagNameTmp = tagName;
                        //if (tagName.IndexOf("\\") == 0)
                        //{
                        //    tagNameTmp = tagName.IndexOf(".") > 0 ? tagName.Substring(tagName.IndexOf(".") + 1) : "";
                        //}

                        createDialogService?.CreateMonitorEditTags(tag.ParentController,
                            program != null ? (ITagCollectionContainer) program : tag.ParentController, tagName,
                            vm.SelectedKind.Equals("Edit"));
                    }

                }

                else if (vm.SelectedKind.Equals("Properties"))
                {
                    if (routine != null && (string.IsNullOrEmpty(tagName) && tag == null))
                    {
                        ICreateDialogService createDialogService =
                            Package.GetGlobalService(typeof(SCreateDialogService)) as
                                ICreateDialogService;
                        var dialogProperties =
                            createDialogService?.CreateRoutineProperties(routine);
                        dialogProperties?.Show(uiShell);
                    }
                    else
                    {
                        ICreateDialogService createDialogService =
                            Package.GetGlobalService(typeof(SCreateDialogService)) as
                                ICreateDialogService;
                       
                        var dialogTagProperties =
                            createDialogService?.CreateTagProperties(tag);
                        dialogTagProperties?.Show(uiShell);
                    }
                }

                if (vm.SelectedKind.Equals("Called Routines") || vm.SelectedKind.Equals("Calling Routines"))
                {
                    //TODO(zyl):
                }

                if (vm.SelectedKind.Equals("New"))
                {
                    ICreateDialogService createDialogService =
                        Package.GetGlobalService(typeof(SCreateDialogService)) as
                            ICreateDialogService;
                    var dialogTagProperties =
                        createDialogService?.CreateNewTagDialog(DINT.Inst, parentProgram.Tags, Usage.Local);
                    dialogTagProperties?.ShowDialog(uiShell);
                }

                if (vm.SelectedKind.Equals("Cross Reference"))
                {
                    //TODO(zyl)
                }
            }
        }
    }

    internal class FindAllMenu : MenuItem
    {
        public FindAllMenu(string name)
        {
            Header = LanguageManager.GetInstance().ConvertSpecifier("Find All") + $" \"{name}\"";
            IsEnabled = false;
        }
    }

    internal class WatchTagsMenuItem : MenuItem
    {
        public WatchTagsMenuItem()
        {
            Header = LanguageManager.GetInstance().ConvertSpecifier("Watch Tags");
            InputGestureText = "Alt + 3";
            var service = Package.GetGlobalService(typeof(SQuickWatchService)) as IQuickWatchService;
            IsChecked = service?.IsVisible() ?? false;
            Click += WatchTagsMenuItem_Click;
            //InputBindings.Add(new KeyBinding(new RelayCommand(OpenWatchTags),
            //    new KeyGesture(Key.D6, ModifierKeys.Alt)));
        }

        private void WatchTagsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenWatchTags();
        }

        private void OpenWatchTags()
        {
            if (IsChecked)
            {
                var service = Package.GetGlobalService(typeof(SQuickWatchService)) as IQuickWatchService;
                service?.Hide();
            }
            else
            {
                var service = Package.GetGlobalService(typeof(SQuickWatchService)) as IQuickWatchService;
                service?.Show();
            }

            IsChecked = !IsChecked;
        }
    }

    internal class BrowseTagsMenuItem : MenuItem
    {
        private readonly TextEditor _textEditor;
        private readonly Point _point;
        private readonly BrowseAdorner _browseAdorner;
        private readonly IStxEditorOptions _options;
        private readonly SnippetInfo _info;

        public BrowseTagsMenuItem(TextEditor editor, Point point, BrowseAdorner browseAdorner,
            IStxEditorOptions options, SnippetInfo info)
        {
            Header = LanguageManager.GetInstance().ConvertSpecifier("Browse Tags");
            InputGestureText = "Ctrl + Space";
            _textEditor = editor;
            point = point + new Vector(0, 15);
            _point = TestHit(editor, point);
            _options = options;
            _info = info;
            _browseAdorner = browseAdorner;
            Debug.Assert(browseAdorner != null);
            Click += BrowseTagsMenuItem_Click;
            //_browseAdorner.LostFocusEventHandler = BrowseAdorner_LostFocus;
            _browseAdorner?.ResetAdorner(_point, _textEditor.FontSize / 12, _info.CodeText);
        }

        private Point TestHit(TextEditor editor, Point point)
        {
            double width = editor.ActualWidth - 50, height = editor.ActualHeight;
            Point newPoint = point;
            if (point.X >= width || point.X + 150 >= width) newPoint.X = width - 150;
            if (point.Y >= height || point.Y + 25 >= height) newPoint.Y = height - 25;
            return newPoint;
        }

        private void Editor_GotFocus(object sender, RoutedEventArgs e)
        {
            string name = _browseAdorner.GetName();
            _textEditor.Document.Replace(_info.Offset, _info.CodeText?.Length ?? 0, new StringTextSource(name ?? ""));
            _textEditor.GotFocus -= Editor_GotFocus;
            _textEditor.TextArea.TextView.ScrollOffsetChanged -= TextView_ScrollOffsetChanged;
        }

        private void BrowseTagsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var layer = AdornerLayer.GetAdornerLayer(_textEditor);
                layer?.Add(_browseAdorner);
                _options.CanZoom = false;
                _textEditor.GotFocus += Editor_GotFocus;
                _textEditor.TextArea.TextView.ScrollOffsetChanged += TextView_ScrollOffsetChanged;
                Dispatcher.BeginInvoke((Action) delegate() { _browseAdorner.SetTextFocus(); },
                    DispatcherPriority.ApplicationIdle);
            }
            catch (Exception)
            {
                //
            }
        }

        private void TextView_ScrollOffsetChanged(object sender, EventArgs e)
        {
            var vector = _textEditor.TextArea.TextView.ScrollVector;
            _browseAdorner.DoScrollChanged(vector);
        }
    }

    internal class CutMenuItem : MenuItem
    {
        private readonly TextEditor _textEditor;

        public CutMenuItem(TextEditor textEditor, IProgramModule program)
        {
            _textEditor = textEditor;
            Header = LanguageManager.GetInstance().ConvertSpecifier("Cut");
            InputGestureText = "Ctrl + X";
            IsEnabled = textEditor.CanCut();
            if ((program as AoiDefinition)?.IsSealed ?? false)
                IsEnabled = false;
            Click += CutMenuItem_Click;
        }

        private void CutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Cut(_textEditor);
        }

        public static void Cut(TextEditor textEditor)
        {
            textEditor.Cut();
        }
    }

    internal class PasteMenuItem : MenuItem
    {
        private readonly TextEditor _textEditor;

        public PasteMenuItem(TextEditor textEditor, IProgramModule program)
        {
            _textEditor = textEditor;
            Header = LanguageManager.GetInstance().ConvertSpecifier("Paste");
            InputGestureText = "Ctrl + V";
            IsEnabled = textEditor.CanPaste();
            if ((program as AoiDefinition)?.IsSealed ?? false)
                IsEnabled = false;
            Click += PasteMenuItem_Click;
        }

        private void PasteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //var data = (string) Clipboard.GetData("code");
            Paste(_textEditor);
        }

        public static void Paste(TextEditor textEditor)
        {
            textEditor.Paste();
        }
    }

    internal class CopyMenuItem : MenuItem
    {
        private readonly TextEditor _textEditor;

        public CopyMenuItem(TextEditor textEditor)
        {
            _textEditor = textEditor;
            Header = LanguageManager.GetInstance().ConvertSpecifier("Copy");
            InputGestureText = "Ctrl + C";
            IsEnabled = textEditor.CanCopy();
            Click += CopyMenuItem_Click;
        }

        private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Copy(_textEditor);
        }

        public static void Copy(TextEditor editor)
        {
            editor.Copy();
        }
    }


    #endregion

    #region TagMenuItem

    internal class MessageConfigurationMenuItem : MenuItem
    {
        private ITag _tag;
        private string _name;
        public MessageConfigurationMenuItem(ITag tag,string name)
        {
            _tag = tag;
            _name = name;
            Header = LanguageManager.GetInstance().ConvertSpecifier("Configure") + $" \"{name}\"";
            IsEnabled = !(tag.ParentCollection.ParentProgram is AoiDefinition);
            Click += MessageConfigurationMenuItem_Click;
        }

        private void MessageConfigurationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var name = string.Compare(_tag.Name, _name,StringComparison.OrdinalIgnoreCase) == 0 ? _name : $"{_name} <{_tag.Name}>";
            var title = $"Message {LanguageManager.GetInstance().ConvertSpecifier("Configuration")} - {name}";
            MessageConfigurationDialog dialog = new MessageConfigurationDialog(_tag,title)
            {
                Owner = Application.Current.MainWindow
            };

            dialog.ShowDialog();
        }
    }

    internal class ConfigureMenuItem : MenuItem
    {
        private readonly ArrayField _field;
        private readonly string _title;
        private readonly ITag _tag; 
        public ConfigureMenuItem(ArrayField field, string name,string title,ITag tag)
        {
            _field = field;
            _title = title;
            _tag = tag;
            Header = LanguageManager.GetInstance().ConvertSpecifier("_Configure") + $" \"{name}\"";
            IsEnabled = field != null;
            Click += ConfigureMenuItem_Click;
        }

        private void ConfigureMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            CamEditorViewModel viewModel = new CamEditorViewModel(_field, _title,_tag);
            CamEditorDialog dialog = new CamEditorDialog(viewModel)
            {
                Owner = Application.Current.MainWindow
            };

            dialog.ShowDialog(uiShell);
        }
    }

    internal class EditPropertiesMenuItem : MenuItem
    {
        private readonly ITag _tag;
        private readonly IProgramModule _parentProgram;

        public EditPropertiesMenuItem(ITag tag, IProgramModule parentProgram, bool isOtherProgram)
        {
            _tag = tag;
            _parentProgram = parentProgram;
            var header1 = LanguageManager.GetInstance().ConvertSpecifier("_Edit");
            var header2 = LanguageManager.GetInstance().ConvertSpecifier("Properties");
            Header = isOtherProgram
                ? $"{header1} \"\\{parentProgram.Name}.{tag.Name}\" {header2}"
                : $"{header1} \"{tag.Name}\" {header2}";
            InputGestureText = "Alt + Enter";
            Click += EditPropertiesMenuItem_Click;
        }

        private void EditPropertiesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditProperties(_tag);
        }

        public static void EditProperties(ITag tag)
        {
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            ICreateDialogService createDialogService =
                Package.GetGlobalService(typeof(SCreateDialogService)) as
                    ICreateDialogService;
            var dialog = createDialogService?.CreateTagProperties(tag);
            dialog?.Show(uiShell);
        }
    }

    internal class CreateNewTagMenuItem : MenuItem
    {
        private readonly ITagCollection _parentTagCollection;

        //private string _name;
        private readonly StxEditorDocument _document;

        //private readonly IDataType _dataType;
        private readonly SnippetInfo _info;

        public CreateNewTagMenuItem(SnippetInfo info, ITagCollection parentTagCollection, StxEditorDocument document)
        {
            _parentTagCollection = parentTagCollection;
            _info = info;
            Fix(info);
            var name = info.CodeText;
            _document = document;
            info.CodeText = name;
            Header = LanguageManager.GetInstance().ConvertSpecifier("_New Tag") + $"\"{name}\"";
            InputGestureText = "Ctrl + W";
            Click += CreateNewTagMenuItem_Click;
        }

        private static void Fix(SnippetInfo info)
        {
            var name = info.CodeText.Replace(" ", "");
            if (name.StartsWith("\\"))
            {
                var index = name.IndexOf(".");
                if (index > 0 && index + 1 < name.Length)
                {
                    name = name.Substring(index + 1);
                }
            }

            {
                var index = name.IndexOf(".");
                if (index > 0)
                {
                    name = name.Substring(0, index);
                }
            }
            info.CodeText = name;
        }

        private void CreateNewTagMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CreateNewTag(_info, _parentTagCollection, _document);
        }

        public static void CreateNewTag(SnippetInfo info, ITagCollection parentTagCollection,
            StxEditorDocument document)
        {
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            ICreateDialogService createDialogService =
                Package.GetGlobalService(typeof(SCreateDialogService)) as
                    ICreateDialogService;
            Fix(info);
            var name = info.CodeText;
            IDataType dataType = null;
            if (info.GetVariableInfos().Count > 0)
            {
                dataType = info.GetVariableInfos()[0].TargetDataType;
            }

            if (dataType == null)
                dataType = DINT.Inst;
            var dialog =
                createDialogService?.CreateNewTagDialog(dataType, parentTagCollection, Usage.Local, null, name);
            if (dialog?.ShowDialog(uiShell) ?? false)
            {
                var vm = (NewTagViewModel) dialog.DataContext;
                var tagName = vm.Name;
                if (vm.TagCollectionContainer is Program)
                {
                    if (vm.TagCollectionContainer != document.Routine.ParentCollection.ParentProgram)
                    {
                        var program = (Program) vm.TagCollectionContainer;
                        tagName = $"\\{program.Name}.{tagName}";
                    }

                }

                if (!name.Equals(tagName))
                {
                    document.Document.Replace(info.Offset, info.CodeText.Length, tagName);
                }

                var setting = GlobalSetting.GetInstance().TagSetting;
                setting.Scope = ((NewTagViewModel) dialog.DataContext).TagCollectionContainer;
                //document.UpdateLexer(false);
                document.Update = true;
            }
        }
    }

    internal class MonitorTagMenuItem : MenuItem
    {
        private readonly ITag _tag;
        private readonly SnippetInfo _info;
        private readonly AoiDataReference _selectedReference;

        public MonitorTagMenuItem(ITag tag, SnippetInfo info, IStxEditorOptions options)
        {
            _tag = tag;
            _info = info;
            _selectedReference = options?.SelectedDataReference;
            Header = LanguageManager.GetInstance().ConvertSpecifier("_Monitor") + $" \"{info.CodeText}\"";
            InputGestureText = "";
            Click += MonitorTagMenuItem_Click;
        }

        private void MonitorTagMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //var uiShell = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            ICreateEditorService createDialogService =
                Package.GetGlobalService(typeof(SCreateEditorService)) as
                    ICreateEditorService;
            var program = _tag.ParentCollection.ParentProgram;
            var tagName = _info.CodeText;
            if (tagName.IndexOf("\\") == 0)
            {
                tagName = tagName.IndexOf(".") > 0 ? tagName.Substring(tagName.IndexOf(".") + 1) : "";
            }

            createDialogService?.CreateAoiMonitorEditTags(_tag.ParentController,
                program != null ? (ITagCollectionContainer)program : _tag.ParentController, _selectedReference, tagName);
        }

    }

    internal class TrendTag : MenuItem
    {
        private readonly IController _controller;
        private readonly ITag _tag;
        private readonly string _name;
        public static int index;

        public TrendTag(IController controller, ITag tag, string name, bool isEnable)
        {
            _name = name;
            _controller = controller;
            _tag = tag;
            Header = LanguageManager.GetInstance().ConvertSpecifier("_Trend Tag") + $" \'{name}\'";
            Click += TrendTag_Click;
            IsEnabled = isEnable;
        }

        private void TrendTag_Click(object sender, RoutedEventArgs e)
        {
            var createEditorService = Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
            index++;
            string displayName = _name.Replace('.', '_');
            string trendName = displayName + index;
            while (!VerifyTrendName(trendName))
            {
                index++;
                trendName = displayName + index;
            }

            var trend = new Trend(_controller)
            {
                Name = trendName,
                Description = "temporary trend",
                SamplePeriod = 10
            };
            var pen = new Pen();
            var program = _tag.ParentCollection.ParentProgram;
            if (program != null)
            {
                pen.Name = $@"\{program.Name}.{_name}";
            }
            else
            {
                pen.Name = _name;
            }

            pen.Color = "16" + TrendObject.GetColor(0);
            trend.Add(pen);
            createEditorService?.CreateTrend(trend, true);
        }

        private bool VerifyTrendName(string trendName)
        {
            foreach (ITrend item in _controller.Trends)
            {
                if (string.Equals(item.Name, trendName, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }
    }

    internal class CrossTagMenuItem : MenuItem
    {
        private readonly ITag _tag;
        private readonly string _code;
        public CrossTagMenuItem(ITag tag, string name,string code)
        {
            _tag = tag;
            _code = code;
            Header = LanguageManager.GetInstance().ConvertSpecifier("_Go To") +
                     $" \"{name}\" " + LanguageManager.GetInstance().ConvertSpecifier("Cross Reference For");
            InputGestureText = "Ctrl + E";
            Click += CrossTagMenuItem_Click;
        }

        private void CrossTagMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CrossTag(_tag, _code);
        }

        public static void CrossTag(ITag tag, string name)
        {
            ICreateEditorService createDialogService =
                Package.GetGlobalService(typeof(SCreateEditorService)) as
                    ICreateEditorService;

            createDialogService?.CreateCrossReference(Type.Tag, tag.ParentCollection.ParentProgram,
                RemoveProgram(name));
        }

        private static string RemoveProgram(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name)) return string.Empty;
                if (name.IndexOf("\\", StringComparison.Ordinal) == 0)
                    return name.Substring(name.IndexOf(".", StringComparison.Ordinal) + 1);
                return name;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }


    #endregion

    #region EnumMenuItem

    internal class BrowseEnumMenuItem : MenuItem
    {
        private readonly BrowseEnumAdorner _browseAdorner;
        private readonly SnippetInfo _info;
        private readonly Point _point;
        private readonly TextEditor _textEditor;
        private readonly StxEditorDocument _document;
        public BrowseEnumMenuItem(TextEditor editor, Point point, StxEditorDocument document, List<string> enums,
            BrowseEnumAdorner browseAdorner, SnippetInfo info)
        {
            _info = info;
            _point = point;
            _document = document;
            _info = info;
            _textEditor = editor;
            _browseAdorner = browseAdorner;
            Header = LanguageManager.GetInstance().ConvertSpecifier("Browse Enumerations");
            InputGestureText = "Ctrl + Alt + Space";
            Click += BrowseEnumMenuItem_Click;
            _browseAdorner.ResetAdorner(_point, _textEditor.FontSize / 12, _info.CodeText, enums);
        }

        private void Editor_GotFocus(object sender, RoutedEventArgs e)
        {
            string name = _browseAdorner.GetName();
            if (!name.Equals(_info.CodeText))
            {
                _textEditor.Document.Replace(_info.Offset, _info.CodeText?.Length ?? 0, new StringTextSource(name ?? ""));
                _document.UpdateLexer();
            }
            _textEditor.GotFocus -= Editor_GotFocus;
        }

        private void BrowseEnumMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var layer = AdornerLayer.GetAdornerLayer(_textEditor);
                layer?.Add(_browseAdorner);
                _document.Options.CanZoom = false;
                _textEditor.GotFocus += Editor_GotFocus;
                Dispatcher.BeginInvoke((Action) delegate() { _browseAdorner.SetTextFocus(); },
                    DispatcherPriority.Loaded);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }

    #endregion

    #region InStrucitonMenuItem

    internal class CreateInstrMenuItem : MenuItem
    {
        private readonly string _instrName;
        private readonly StxEditorDocument _document;

        public CreateInstrMenuItem(string name, StxEditorDocument document)
        {
            Header = LanguageManager.GetInstance().ConvertSpecifier("Create instruction Tag...");
            Click += CreateInstrMenuItem_Click;
            _instrName = name;
            if ("ssv".Equals(name, StringComparison.OrdinalIgnoreCase) ||
                "gsv".Equals(name, StringComparison.OrdinalIgnoreCase)||
                "size".Equals(name,StringComparison.OrdinalIgnoreCase)||
                "cop".Equals(name,StringComparison.OrdinalIgnoreCase)||
                "cps".Equals(name,StringComparison.OrdinalIgnoreCase)||
                "jsr".Equals(name,StringComparison.OrdinalIgnoreCase)||
                "sbr".Equals(name,StringComparison.OrdinalIgnoreCase)||
                "ret".Equals(name,StringComparison.OrdinalIgnoreCase))
                IsEnabled = false;
            _document = document;
        }

        private IDataType GetInstrTargetDataType(string instrName)
        {
            switch (instrName.ToLower())
            {
                case "alm":
                    return ALARM.Inst;
                case "scl":
                    return SCALE.Inst;
                //TODO(zyl):add other instr default dataType
                default:
                    return Controller.GetInstance().DataTypes[_instrName] ?? DINT.Inst;
            }
        }

        private void CreateInstrMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            ICreateDialogService createDialogService =
                Package.GetGlobalService(typeof(SCreateDialogService)) as
                    ICreateDialogService;
            var controller = Controller.GetInstance();
            var setting = GlobalSetting.GetInstance().TagSetting.Scope;
            if ((setting is Program || setting is AoiDefinition) && setting != _document.Routine.ParentCollection.ParentProgram)
            {
                setting = _document.Routine.ParentCollection.ParentProgram;
            }
            ITagCollection tagCollection = setting == null ? controller.Tags : setting.Tags;
            var dialog =
                createDialogService?.CreateNewTagDialog(GetInstrTargetDataType(_instrName), tagCollection, Usage.Local,
                    null, "?");
            if (dialog.ShowDialog(uiShell) ?? false)
            {
                _document.UpdateLexer(false);
                _document.Update = true;
            }
        }
    }

    internal class ArgumentListMenuItem : MenuItem
    {
        private string _instrName;
        private List<ParamInfo> _parameters;
        private StxEditorDocument _document;
        private int _startOfInstr;
        private Hashtable _transformTable;

        public ArgumentListMenuItem(string instrName, StxEditorDocument document, int startOfInstr,
            List<ParamInfo> parameters, Hashtable transformTable)
        {
            _instrName = instrName;
            if (_instrName.Equals("abs", StringComparison.OrdinalIgnoreCase) ||
                _instrName.Equals("sin", StringComparison.OrdinalIgnoreCase) ||
                _instrName.Equals("asin", StringComparison.OrdinalIgnoreCase) ||
                _instrName.Equals("cos", StringComparison.OrdinalIgnoreCase) ||
                _instrName.Equals("acos", StringComparison.OrdinalIgnoreCase) ||
                _instrName.Equals("tan", StringComparison.OrdinalIgnoreCase) ||
                _instrName.Equals("atan", StringComparison.OrdinalIgnoreCase) ||
                _instrName.Equals("SQRT", StringComparison.OrdinalIgnoreCase))
            {
                IsEnabled = false;
            }

            _startOfInstr = startOfInstr;
            _parameters = parameters;
            _transformTable = transformTable;
            _document = document;
            Header = LanguageManager.GetInstance().ConvertSpecifier("Argument List...");
            InputGestureText = "Alt + A";
            Click += ArgumentListMenuItem_Click;
            //InputBindings.Add(new KeyBinding(new RelayCommand(OpenArgumentList),
            //    new KeyGesture(Key.A, ModifierKeys.Alt)));
        }

        private void ArgumentListMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenArgumentList();
        }

        private void OpenArgumentList()
        {
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            if (_instrName.Equals("gsv", StringComparison.OrdinalIgnoreCase))
            {
                var dialog = new GSVArgumentListDialog();
                var vm = new GSVArgumentListDialogViewModel<InstrEnum.ClassName>(_parameters,
                    _document.SnippetLexer.ConnectionReference?.Routine?.ParentCollection.ParentProgram ??
                    _document.Routine.ParentCollection.ParentProgram);
                dialog.DataContext = vm;
                vm.PropertyChanged += Vm_PropertyChanged;
                if (dialog?.ShowDialog(uiShell) ?? false)
                {
                    UpdateInstr(vm);
                }

                vm.PropertyChanged -= Vm_PropertyChanged;
            }
            else if (_instrName.Equals("ssv", StringComparison.OrdinalIgnoreCase))
            {
                var dialog = new GSVArgumentListDialog();
                var vm = new GSVArgumentListDialogViewModel<InstrEnum.SSVClassName>(_parameters,
                    _document.SnippetLexer.ConnectionReference?.Routine?.ParentCollection.ParentProgram ??
                    _document.Routine.ParentCollection.ParentProgram);
                dialog.DataContext = vm;
                vm.PropertyChanged += Vm_PropertyChanged;
                if (dialog?.ShowDialog(uiShell) ?? false)
                {
                    UpdateInstr(vm);
                }

                vm.PropertyChanged -= Vm_PropertyChanged;
            }
            else if (_instrName.Equals("ret", StringComparison.OrdinalIgnoreCase) ||
                     _instrName.Equals("sbr", StringComparison.OrdinalIgnoreCase))
            {
                //暂时不需要这些指令参数
                return;
                //var dialog = new RETArgumentListDialog();
                //var arguments = new ObservableCollection<Argument>();
                //foreach (var parameter in _parameters)
                //{
                //    arguments.Add(new Argument("", parameter.Param, null, parameter.Offset,
                //        _document.SnippetLexer.ConnectionReference?.Routine?.ParentCollection.ParentProgram ??
                //        _document.Routine.ParentCollection.ParentProgram, _transformTable));
                //}

                //var vm = new RETArgumentListDialogViewModel(arguments,
                //    _document.SnippetLexer.ConnectionReference?.Routine?.ParentCollection.ParentProgram ??
                //    _document.Routine.ParentCollection.ParentProgram,
                //    _instrName.ToUpper(), _transformTable);
                //dialog.DataContext = vm;
                //vm.PropertyChanged += Vm_PropertyChanged;
                //if (dialog?.ShowDialog(uiShell) ?? false)
                //{
                //    UpdateInstrRet(vm.Parameters);
                //}

                //vm.PropertyChanged -= Vm_PropertyChanged;
            }
            else if (_instrName.Equals("jsr", StringComparison.OrdinalIgnoreCase))
            {
                var dialog = new JSRArgumentListDialog();
                var vm = new JSRArgumentListDialogViewModel(
                    _document.SnippetLexer.ConnectionReference?.Routine?.ParentCollection.ParentProgram ??
                    _document.Routine.ParentCollection.ParentProgram,
                    (_parameters.Count > 0 ? _parameters[0].Param : ""));
                dialog.DataContext = vm;
                vm.PropertyChanged += Vm_PropertyChanged;
                if (dialog?.ShowDialog(uiShell) ?? false)
                {
                    if (vm.CanExecuteApplyCommand())
                        UpdateInstrJSR(vm);
                }

                vm.PropertyChanged -= Vm_PropertyChanged;
            }
            else
            {
                var dialog = new CommonArgumentListDialog();
                var vm = new CommonArgumentListDialogViewModel(_instrName, GetParametersInfo(_instrName, _parameters));
                vm.PropertyChanged += Vm_PropertyChanged;
                dialog.DataContext = vm;
                if (dialog?.ShowDialog(uiShell) ?? false)
                {
                    UpdateInstr(vm.Arguments);
                }

                vm.PropertyChanged -= Vm_PropertyChanged;
            }

            //TODO(zyl):add other instr 
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsUpdated")
            {
                if (_instrName.Equals("gsv", StringComparison.OrdinalIgnoreCase))
                {
                    var vm = (GSVArgumentListDialogViewModel<InstrEnum.ClassName>) sender;
                    UpdateInstr(vm);
                }
                else if (_instrName.Equals("ssv", StringComparison.OrdinalIgnoreCase))
                {
                    var vm = (GSVArgumentListDialogViewModel<InstrEnum.SSVClassName>) sender;
                    UpdateInstr(vm);
                }
                else if (_instrName.Equals("ret", StringComparison.OrdinalIgnoreCase) ||
                         _instrName.Equals("sbr", StringComparison.OrdinalIgnoreCase))
                {
                    var vm = (RETArgumentListDialogViewModel) sender;
                    UpdateInstrRet(vm.Parameters);
                }
                else if (_instrName.Equals("jsr", StringComparison.OrdinalIgnoreCase))
                {
                    var vm = (JSRArgumentListDialogViewModel) sender;
                    UpdateInstrJSR(vm);
                }
                else
                {
                    var vm = (CommonArgumentListDialogViewModel) sender;
                    UpdateInstr(vm.Arguments);
                }
            }
        }

        #region update text

        private void UpdateInstr<T>(GSVArgumentListDialogViewModel<T> vm) where T : struct
        {
            var text = _document.Document.Text;
            var count = _parameters.Count;
            int endOfInstr = count == 0
                ? (_startOfInstr + _instrName.Length)
                : (_parameters[count - 1].Offset + _parameters[count - 1].Param.Length);
            for (int i = count - 1; i >= 0; i--)
            {
                var param = _parameters[i];
                if (i == 0)
                {
                    text = text.Remove(param.Offset, param.Param.Length);
                    var offset = vm.SelectedClass.ToString().Length - param.Param.Length;

                    endOfInstr += offset;
                    text = text.Insert(param.Offset, vm.SelectedClass.ToString() ?? "");
                    for (int j = i + 1; j < count; j++)
                    {
                        _parameters[j].Offset += offset;
                    }

                    param.Param = vm.SelectedClass.ToString();
                    continue;
                }

                if (i == 1)
                {
                    text = text.Remove(param.Offset, param.Param.Length);
                    var offset = -param.Param.Length;
                    if (vm.TagFilterVisibility == Visibility.Visible)
                    {
                        offset += vm.InstanceName?.Length ?? 0;
                        text = text.Insert(param.Offset, vm.InstanceName ?? "");
                        param.Param = vm.InstanceName ?? "";
                    }

                    if (vm.InstanceComboboxVisibility == Visibility.Visible)
                    {
                        offset += vm.SelectedInstanceCollection?.Length ?? 0;
                        text = text.Insert(param.Offset, vm.SelectedInstanceCollection ?? "");
                        param.Param = vm.SelectedInstanceCollection ?? "";
                    }

                    endOfInstr += offset;
                    for (int j = i + 1; j < count; j++)
                    {
                        _parameters[j].Offset += offset;
                    }

                    continue;
                }

                if (i == 2)
                {
                    text = text.Remove(param.Offset, param.Param.Length);
                    var offset = -param.Param.Length;
                    offset += vm.SelectedAttribute?.Length ?? 0;
                    text = text.Insert(param.Offset, vm.SelectedAttribute ?? "");
                    endOfInstr += offset;
                    for (int j = i + 1; j < count; j++)
                    {
                        _parameters[j].Offset += offset;
                    }

                    param.Param = vm.SelectedAttribute ?? "";
                    continue;
                }

                if (i == 3)
                {
                    text = text.Remove(param.Offset, param.Param.Length);
                    var offset = -param.Param.Length;
                    offset += vm.DestinationName?.Length ?? 0;
                    text = text.Insert(param.Offset, vm.DestinationName ?? "");
                    endOfInstr += offset;
                    for (int j = i + 1; j < count; j++)
                    {
                        _parameters[j].Offset += offset;
                    }

                    param.Param = vm.DestinationName ?? "";
                }
            }

            for (int i = count; i < 4; i++)
            {
                if (i == 0)
                {

                    VerifyMissing('(', ref endOfInstr, ref text);
                    var n = new ParamInfo();
                    n.Offset = endOfInstr;
                    n.Param = vm.SelectedClass.ToString();
                    _parameters.Insert(0, n);
                    text = text.Insert(endOfInstr, vm.SelectedClass.ToString());
                    endOfInstr += vm.SelectedClass.ToString().Length;
                    continue;
                }

                if (i == 1)
                {
                    VerifyMissing(',', ref endOfInstr, ref text);
                    var n = new ParamInfo();
                    n.Offset = endOfInstr;
                    _parameters.Insert(1, n);
                    if (vm.TagFilterVisibility == Visibility.Visible)
                    {
                        var s = (vm.InstanceName ?? "");
                        text = text.Insert(endOfInstr, s);
                        endOfInstr += s.Length;
                        n.Param = s;
                    }

                    if (vm.InstanceComboboxVisibility == Visibility.Visible)
                    {
                        var s = (vm.SelectedInstanceCollection ?? "");
                        text = text.Insert(endOfInstr, s);
                        n.Param = s;
                        endOfInstr += s.Length;
                    }

                    continue;
                }

                if (i == 2)
                {
                    VerifyMissing(',', ref endOfInstr, ref text);
                    var n = new ParamInfo();
                    n.Offset = endOfInstr;
                    _parameters.Insert(2, n);
                    var s = (vm.SelectedAttribute ?? "");
                    n.Param = s;
                    text = text.Insert(endOfInstr, s);
                    endOfInstr += s.Length;
                    continue;
                }

                if (i == 3)
                {
                    VerifyMissing(',', ref endOfInstr, ref text);
                    var n = new ParamInfo();
                    n.Offset = endOfInstr;
                    _parameters.Insert(3, n);
                    var s = vm.DestinationName ?? "";
                    n.Param = s;
                    text = text.Insert(endOfInstr, s);
                    endOfInstr += s.Length;
                    VerifyMissing(')', ref endOfInstr, ref text);
                    continue;
                }
            }

            _document.Document.Text = text;
        }

        private void UpdateInstrRet(ObservableCollection<Argument> arguments)
        {
            try
            {
                var text = _document.Document.Text;
                if (_parameters == null) _parameters = new List<ParamInfo>();
                int endOfInstr = _startOfInstr + _instrName.Length;
                int index2 = 0;
                for (int i = 0; i < arguments.Count; i++)
                {
                    var argument = arguments[i];
                    if (string.IsNullOrEmpty(argument.Param)) continue;
                    if (_parameters.Count <= i)
                    {
                        _parameters.Add(new ParamInfo(-1, ""));
                    }

                    var paramInfo = _parameters[index2];
                    if (paramInfo.Offset == -1)
                    {
                        VerifyMissing(i == 0 ? '(' : ',', ref endOfInstr, ref text);
                    }
                    else
                    {
                        endOfInstr = paramInfo.Offset;
                    }

                    var offset = argument.Param.Length - paramInfo.Param.Length;
                    text = text.Remove(endOfInstr, paramInfo.Param.Length);
                    text = text.Insert(endOfInstr, argument.Param);
                    argument.Offset = endOfInstr;
                    paramInfo.Offset = argument.Offset;
                    paramInfo.Param = argument.Param;
                    endOfInstr += argument.Param.Length;
                    for (int j = i + 1; j < _parameters.Count; j++)
                    {
                        _parameters[j].Offset += offset;
                    }

                    index2++;
                }

                var l = _parameters.Count - index2;
                if (l > 0)
                {
                    for (int i = 0; i < l; i++)
                    {
                        var index = index2 - i;
                        var parameter = _parameters[index];
                        text = text.Remove(parameter.Offset - 1, parameter.Param.Length + 1);
                        _parameters.RemoveAt(index);
                    }
                }

                _document.Document.Text = text;
            }
            catch (Exception e)
            {
                Debug.Assert(false, e.Message);
            }
        }

        private void UpdateInstr(ObservableCollection<Argument> arguments)
        {
            try
            {
                var text = _document.Document.Text;
                if (_parameters == null) _parameters = new List<ParamInfo>();
                var count = _parameters.Count;
                int endOfInstr = count == 0
                    ? (_startOfInstr + _instrName.Length)
                    : (_parameters[count - 1].Offset + _parameters[count - 1].Param.Length);
                for (int i = 0; i < arguments.Count; i++)
                {
                    var argument = arguments[i];
                    if (_parameters.Count <= i)
                    {
                        _parameters.Add(new ParamInfo(-1, ""));
                    }

                    var paramInfo = _parameters[i];
                    if (argument.Offset == -1)
                    {
                        VerifyMissing(i == 0 ? '(' : ',', ref endOfInstr, ref text);
                    }
                    else
                    {
                        endOfInstr = argument.Offset;
                    }

                    var offset = argument.Param.Length - paramInfo.Param.Length;
                    text = text.Remove(endOfInstr, paramInfo.Param.Length);
                    text = text.Insert(endOfInstr, argument.Param);
                    argument.Offset = endOfInstr;
                    paramInfo.Offset = argument.Offset;
                    paramInfo.Param = argument.Param;
                    endOfInstr += argument.Param.Length;
                    for (int j = i + 1; j < arguments.Count; j++)
                    {
                        if (arguments[j].Offset == -1) continue;
                        arguments[j].Offset += offset;
                    }
                }

                VerifyMissing(')', ref endOfInstr, ref text);
                _document.Document.Text = text;
            }
            catch (Exception e)
            {
                Debug.Assert(false, e.Message);
            }
        }

        private void UpdateInstrJSR(JSRArgumentListDialogViewModel vm)
        {
            var text = _document.Document.Text;
            var routineName = vm.SelectedRoutine == "<none>" ? "" : vm.SelectedRoutine;
            var count = _parameters.Count;
            int endOfInstr = count == 0
                ? (_startOfInstr + _instrName.Length)
                : (_parameters[0].Offset + _parameters[0].Param.Length);
            if (_parameters.Count == 0)
                _parameters.Add(new ParamInfo());
            var paramInfo = _parameters[0];
            if (paramInfo.Offset == -1)
            {
                VerifyMissing('(', ref endOfInstr, ref text);
            }
            else
            {
                endOfInstr = paramInfo.Offset;
            }
            
            if (paramInfo.Param.Length > 0)
            {
                text = text.Remove(endOfInstr, paramInfo.Param.Length);
            }

            text = text.Insert(endOfInstr, routineName);
            paramInfo.Offset = endOfInstr;
            endOfInstr += routineName.Length;
            VerifyMissing(')', ref endOfInstr, ref text);
            _document.Document.Text = text;
            paramInfo.Param = routineName;
        }

        #endregion

        private void VerifyMissing(char symbol, ref int offset, ref string text)
        {
            bool isMissing = true;
            var index = offset;
            for (; index < text.Length; index++)
            {
                var c = text[index];
                if (char.IsWhiteSpace(c)) continue;
                if (c == symbol)
                {
                    isMissing = false;
                    offset = index;
                }

                break;
            }

            if (isMissing)
            {
                text = text.Insert(offset, symbol.ToString());
                offset++;
            }
            else
            {
                offset = ++index;
            }
        }

        private ObservableCollection<Argument> GetParametersInfo(string instrName, List<ParamInfo> parameters)
        {
            var instr = Controller.GetInstance().STInstructionCollection.FindInstruction(instrName);
            if (instr != null)
            {
                var infos = new ObservableCollection<Argument>();
                var index = 0;
                foreach (var tuple in instr.GetParameterInfo())
                {
                    var param = index < parameters?.Count ? parameters[index] : null;
                    var info = SnippetLexer.GetInstrEnumInfo(instrName, index, "");
                    int result;
                    var f = int.TryParse(ObtainValue.LoadNumber(param?.Param), out result);
                    var argumentParam = param?.Param;
                    if (f && info != null)
                    {
                        var e = Enum.Parse(info.Item2, param.Param);
                        var element = info.Item2.GetField(e.ToString());
                        if (element != null)
                        {
                            var attribute =
                                Attribute.GetCustomAttribute(element,
                                    typeof(EnumMemberAttribute)) as EnumMemberAttribute;
                            argumentParam = attribute?.Value;
                        }
                    }

                    var argument = new Argument(tuple.Item1, argumentParam, tuple.Item2, param?.Offset ?? -1,
                        _document.SnippetLexer.ConnectionReference?.Routine?.ParentCollection.ParentProgram ??
                        _document.Routine.ParentCollection.ParentProgram, _transformTable, info != null);

                    if (info != null)
                    {
                        argument.SetEnumList(info.Item1);
                    }

                    infos.Add(argument);
                    index++;
                }

                return infos;
            }

            return null;
        }
    }

    #endregion

    #region STMenuItem

    internal class NewTagMenuItem : MenuItem
    {
        private readonly ITagCollection _tagCollection;
        private readonly StxEditorDocument _document;

        public NewTagMenuItem(ITagCollection tagCollection, StxEditorDocument document)
        {
            _document = document;
            _tagCollection = tagCollection;
            Header = LanguageManager.GetInstance().ConvertSpecifier("New Variable...");
            InputGestureText = "Ctrl + W";
            Click += NewTagMenuItem_Click;
        }

        private void NewTagMenuItem_Click(object sender, RoutedEventArgs e)
        {
            NewTag(_tagCollection, _document);
        }

        public static void NewTag(ITagCollection tagCollection, StxEditorDocument document)
        {
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            ICreateDialogService createDialogService =
                Package.GetGlobalService(typeof(SCreateDialogService)) as
                    ICreateDialogService;
            var dialog = createDialogService?.CreateNewTagDialog(DINT.Inst, tagCollection, Usage.Local, null, "");
            if (dialog?.ShowDialog(uiShell) ?? false)
            {
                //document?.UpdateLexer();
                var setting = GlobalSetting.GetInstance().TagSetting;
                setting.Scope = ((NewTagViewModel) dialog.DataContext).TagCollectionContainer;
            }
        }
    }

    internal class PropertiesMenuItem : MenuItem
    {
        private IRoutine _routine;

        public PropertiesMenuItem(IRoutine routine)
        {
            _routine = routine;
            Header = LanguageManager.GetInstance().ConvertSpecifier("Properties");
            InputGestureText = "Alt + Enter";
            Click += PropertiesMenuItem_Click;
        }

        private void PropertiesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Properties(_routine);
        }

        public static void Properties(IRoutine routine)
        {
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            ICreateDialogService createDialogService =
                Package.GetGlobalService(typeof(SCreateDialogService)) as
                    ICreateDialogService;
            var dialog = createDialogService?.CreateRoutineProperties(routine);
            dialog?.Show(uiShell);
        }
    }

    internal class AddSTElementMenuItem : MenuItem
    {
        private readonly IController _controller;
        private readonly TextDocument _document;
        private readonly SnippetInfo _snippetInfo;

        public AddSTElementMenuItem(IController controller, TextDocument document, SnippetInfo info)
        {
            Header = LanguageManager.GetInstance().ConvertSpecifier("Add ST Element...");
            InputGestureText = "Alt + Ins";
            _controller = controller;
            _snippetInfo = info;
            _document = document;
            Click += AddSTElementMenuItem_Click;
        }

        private void AddSTElementMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddSTElement(_controller, _document, _snippetInfo);
        }

        public static void AddSTElement(IController controller, TextDocument document, SnippetInfo info)
        {
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            var dialog = new AddSTElementDialog(controller, info.CodeText);
            if ((bool) dialog.ShowDialog(uiShell))
            {
                var insert = dialog.SelectedElement;
                insert = insert + "( )";
                if (string.IsNullOrEmpty(info.CodeText))
                {
                    document.Insert(info.Offset, insert);
                }
                else
                {
                    document.Remove(info.Offset, info.CodeText.Length);
                    document.Insert(info.Offset, insert);
                }
            }
        }
    }

    internal class CloseRoutineMenuItem : MenuItem
    {
        private readonly IRoutine _routine;

        public CloseRoutineMenuItem(IRoutine routine)
        {
            _routine = routine;
            Header = LanguageManager.GetInstance().ConvertSpecifier("Close Routine");
            Click += CloseRoutineMenuItem_Click;
        }

        private void CloseRoutineMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ICreateEditorService createDialogService =
                Package.GetGlobalService(typeof(SCreateEditorService)) as
                    ICreateEditorService;
            createDialogService?.CloseSTEditor(_routine);
        }
    }

    internal class OptionsMenuItem : MenuItem
    {
        public OptionsMenuItem()
        {
            Header = LanguageManager.GetInstance().ConvertSpecifier("Options...");
        }
    }

    #endregion

    #region ProgramMenuItem

    internal class EditProgramPropertiesMenuItem : MenuItem
    {
        private readonly IProgramModule _program;

        public EditProgramPropertiesMenuItem(IProgramModule program)
        {
            _program = program;
            var header1 = LanguageManager.GetInstance().ConvertSpecifier("_Edit");
            var header2 = LanguageManager.GetInstance().ConvertSpecifier("Properties");
            Header = $"{header1} \"{program.Name}\" {header2}";
            InputGestureText = "Alt + Enter";
            Click += EditProgramPropertiesMenuItem_Click;
        }

        private void EditProgramPropertiesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditProgram(_program);
        }

        public static void EditProgram(IProgramModule program)
        {
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            ICreateDialogService createDialogService =
                Package.GetGlobalService(typeof(SCreateDialogService)) as
                    ICreateDialogService;
            var dialog = createDialogService?.CreateProgramProperties(program as IProgram);
            dialog?.Show(uiShell);
        }
    }

    internal class FindAllProgramMenuItem : MenuItem
    {
        private readonly IProgramModule _program;

        public FindAllProgramMenuItem(IProgramModule program)
        {
            _program = program;
            Header = LanguageManager.GetInstance().ConvertSpecifier("_Find All") + $" {program.Name}";
        }
    }

    internal class CrossProgramMenuItem : MenuItem
    {
        private readonly IProgramModule _program;

        //private readonly ITag _tag;
        public CrossProgramMenuItem(IProgramModule program)
        {
            _program = program;
            Header = LanguageManager.GetInstance().ConvertSpecifier("_Go To") + $" \"{program.Name}\" " + 
                     LanguageManager.GetInstance().ConvertSpecifier("Cross Reference For");
            InputGestureText = "Ctrl + E";
            Click += CrossProgramMenuItem_Click;
        }

        private void CrossProgramMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CrossProgram(_program);
        }

        public static void CrossProgram(IProgramModule program)
        {
            ICreateEditorService createDialogService =
                Package.GetGlobalService(typeof(SCreateEditorService)) as
                    ICreateEditorService;

            createDialogService?.CreateCrossReference(Type.Tag, program, null);
        }
    }

    internal class BrowseProgramMenuItem : MenuItem
    {
        private readonly IProgramModule _program;
        private readonly TextEditor _editor;
        private readonly AdornerLayer _layer;

        private readonly ProgramBrowse _programBrowse;

        //private readonly CamPoints _point;
        private readonly SnippetInfo _info;

        public BrowseProgramMenuItem(IProgramModule program, TextEditor editor, Point point, SnippetInfo info)
        {
            _program = program;
            _info = info;
            //_point = point;
            _editor = editor;
            Header = LanguageManager.GetInstance().ConvertSpecifier("Browse Program");
            InputGestureText = "Ctrl + Alt + Space";
            _layer = AdornerLayer.GetAdornerLayer(_editor);
            _programBrowse = new ProgramBrowse(editor, program, point, editor.FontSize / 12);
            Click += BrowseProgramMenuItem_Click;
        }

        private void BrowseProgramMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _layer.Add(_programBrowse);
            _editor.GotFocus += _editor_GotFocus;
            Dispatcher.BeginInvoke((Action) delegate() { _programBrowse.SetTextFocus(); },
                DispatcherPriority.ApplicationIdle);
        }

        private void _editor_GotFocus(object sender, RoutedEventArgs e)
        {
            _layer.Remove(_programBrowse);
            _editor.Document.Replace(_info.Offset, _program.Name.Length + 1,
                new StringTextSource("\\" + _programBrowse.ProgramName ?? ""));
            _editor.GotFocus -= _editor_GotFocus;
        }
    }

    #endregion

    #region Aoi

    internal class InsertDefault : MenuItem
    {
        private readonly SnippetInfo _snippetInfo;
        private readonly IXInstruction _instr;
        private TextDocument _document;

        public InsertDefault(SnippetInfo info, IXInstruction instr, TextDocument document)
        {
            Header = LanguageManager.GetInstance().ConvertSpecifier("Insert Instruction Default");
            _snippetInfo = info;
            _instr = instr;
            _document = document;
            IsEnabled = instr.DefaultArguments.Any();
            Click += InsertDefault_Click;
        }

        private void InsertDefault_Click(object sender, RoutedEventArgs e)
        {
            if (_snippetInfo.GetVariableInfos().Any() && (_snippetInfo.GetVariableInfos()[0].Parameters == null ||
                                                     _snippetInfo.GetVariableInfos()[0].Parameters.nodes?.Count == 0))
            {
                var insertElement = "";
                var text = _snippetInfo.Parent;
                var offset = _snippetInfo.Offset + _snippetInfo.CodeText.Length;
                var result = VerifyMissing('(', ref offset, ref text, ref insertElement);
                var insertOffset = result ? _snippetInfo.Offset + _snippetInfo.CodeText.Length : offset;
                int index = 0;
                foreach (var info in _instr.GetParameterInfo())
                {
                    var param = _instr.DefaultArguments.ContainsKey(info.Item1)
                        ? _instr.DefaultArguments[info.Item1]
                        : "";
                    if (index == 0)
                    {
                        insertElement += param;
                        text = text.Insert(offset, param);
                        offset += param.Length;
                    }
                    else
                    {
                        var element = $",{param}";
                        insertElement += element;
                        text = text.Insert(offset, element);
                        offset += element.Length;
                    }

                    index++;
                }

                VerifyMissing(')', ref offset, ref text, ref insertElement);
                _document.Insert(insertOffset, insertElement);
            }
        }

        private bool VerifyMissing(char symbol, ref int offset, ref string text, ref string insertElement)
        {
            bool isMissing = true;
            var index = offset;
            for (; index < text.Length; index++)
            {
                var c = text[index];
                if (char.IsWhiteSpace(c)) continue;
                if (c == symbol)
                {
                    isMissing = false;
                    offset = index;
                }

                break;
            }

            if (isMissing)
            {
                text = text.Insert(offset, symbol.ToString());
                insertElement += symbol;
                offset++;
            }
            else
            {
                offset = ++index;
            }

            return isMissing;
        }
    }

    internal class SaveDefault : MenuItem
    {
        private readonly SnippetInfo _snippetInfo;
        private readonly IXInstruction _instr;

        public SaveDefault(SnippetInfo info, IXInstruction instr)
        {
            Header = LanguageManager.GetInstance().ConvertSpecifier("Save Instruction Default");
            _snippetInfo = info;
            _instr = instr;
            Click += SaveDefault_Click;
        }

        private void SaveDefault_Click(object sender, RoutedEventArgs e)
        {
            int index = 0;
            var paramInfos = _instr.GetParameterInfo();
            if (_snippetInfo.GetVariableInfos().Any()&&_snippetInfo.GetVariableInfos()[0].Parameters != null)
            {
                var paramColl = _snippetInfo.GetVariableInfos()[0].Parameters.nodes;

                foreach (var tuple in paramInfos)
                {
                    var name = "";
                    if (index < paramColl.Count && (!(_instr is AoiDefinition.AOIInstruction) || index > 0))
                    {
                        var param = paramColl[index];
                        name = ObtainValue.GetParamName(param, _snippetInfo.Parent);
                    }

                    if (_instr.DefaultArguments.ContainsKey(tuple.Item1))
                    {
                        _instr.DefaultArguments[tuple.Item1] = name;
                    }
                    else
                    {
                        _instr.DefaultArguments.Add(tuple.Item1, name);
                    }

                    index++;
                }
            }
            else
            {
                foreach (var tuple in paramInfos)
                {
                    var name = "";
                    if (_instr.DefaultArguments.ContainsKey(tuple.Item1))
                    {
                        _instr.DefaultArguments[tuple.Item1] = name;
                    }
                    else
                    {
                        _instr.DefaultArguments.Add(tuple.Item1, name);
                    }
                }
            }
        }
    }


    internal class ClearDefault : MenuItem
    {
        private readonly IXInstruction _instr;

        public ClearDefault(IXInstruction instr)
        {
            Header = LanguageManager.GetInstance().ConvertSpecifier("Clear Instruction Default");
            _instr = instr;
            IsEnabled = instr.DefaultArguments.Any();
            Click += ClearDefault_Click;
        }

        private void ClearDefault_Click(object sender, RoutedEventArgs e)
        {
            _instr.DefaultArguments.Clear();
        }
    }


    internal class OpenLogic : MenuItem
    {
        private AoiDefinition _aoiDefinition;
        private SnippetInfo _snippetInfo;
        private IRoutine _routine;

        public OpenLogic(SnippetInfo info, AoiDefinition aoiDefinition, IRoutine routine)
        {
            Header = LanguageManager.GetInstance().ConvertSpecifier("Open Instruction Logic");
            _routine = routine;
            _snippetInfo = info;
            _aoiDefinition = aoiDefinition;
            Click += OpenLogic_Click;
        }

        private void OpenLogic_Click(object sender, RoutedEventArgs e)
        {
            var createEditorService = Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
            var aoiDataReference =
                _aoiDefinition.References.FirstOrDefault(r => r.Routine == _routine && r.Offset == _snippetInfo.Offset);
            var logic = _aoiDefinition.Routines["Logic"];
            if(logic is STRoutine)
                createEditorService?.CreateSTEditor(logic, OnlineEditType.Original, null, null,
                    null, aoiDataReference);
            else if(logic is RLLRoutine)
                createEditorService?.CreateRLLEditor(logic);
            else if (logic is SFCRoutine)
                createEditorService?.CreateSFCEditor(logic);
            else
                Debug.Assert(false,logic.Type.ToString());
        }
    }

    internal class OpenDefinition : MenuItem
    {
        private AoiDefinition _aoiDefinition;

        public OpenDefinition(AoiDefinition aoiDefinition)
        {
            Header = LanguageManager.GetInstance().ConvertSpecifier("Open Instruction Definition");
            _aoiDefinition = aoiDefinition;
            Click += OpenDefinition_Click;
        }

        private void OpenDefinition_Click(object sender, RoutedEventArgs e)
        {
            var createDialogService = Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;
            var dialog = createDialogService?.AddOnInstructionProperties(_aoiDefinition);
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            dialog.Show(uiShell);
        }
    }

    internal class NewAoiTagMenuItem : MenuItem
    {
        private readonly IProgramModule _program;

        private readonly bool _isParameter;

        //private readonly string _name;
        private readonly StxEditorDocument _document;

        //private readonly IDataType _dataType;
        private readonly SnippetInfo _snippetInfo;

        public NewAoiTagMenuItem(SnippetInfo info, IProgramModule program, bool isParameter, StxEditorDocument document)
        {
            _program = program;
            _document = document;
            _isParameter = isParameter;
            _snippetInfo = info;
            var header1 = LanguageManager.GetInstance().ConvertSpecifier("New Parameter...");
            var header2 = LanguageManager.GetInstance().ConvertSpecifier("New Local Tag...");
            var name = info?.CodeText.Replace(" ", "");
            if (program.ParentController.IsOnline || ((program as AoiDefinition)?.IsSealed ?? false))
                IsEnabled = false;
            if (string.IsNullOrEmpty(name))
                Header = _isParameter ? header1 : header2;
            else
                Header = _isParameter ? header1 + $"'{name}'..." : header2 + $"'{name}'...";
            Click += NewAoiTagMenuItem_Click;
        }

        private void NewAoiTagMenuItem_Click(object sender, RoutedEventArgs e)
        {
            NewAoiTag(_snippetInfo, _program, _isParameter, _document);
        }

        public static void NewAoiTag(SnippetInfo info, IProgramModule program, bool isParameter,
            StxEditorDocument document)
        {
            var name = info?.CodeText.Replace(" ", "");
            IDataType dataType = null;
            if (info != null && info.GetVariableInfos().Count > 0)
            {
                dataType = info.GetVariableInfos()[0].TargetDataType;
            }

            if (dataType == null)
                dataType = DINT.Inst;
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            ICreateDialogService createDialogService =
                Package.GetGlobalService(typeof(SCreateDialogService)) as
                    ICreateDialogService;
            //Sure ST
            var dialog = createDialogService?.CreateNewAoiTagDialog(dataType, program as IAoiDefinition,
                isParameter ? Usage.Input : Usage.Local, false, name);
            if (dialog?.ShowDialog(uiShell) ?? false)
            {
                document?.UpdateLexer();
            }
        }
    }

    internal class EditAoiTagMenuItem : MenuItem
    {
        private ITag _tag;

        public EditAoiTagMenuItem(ITag tag)
        {
            _tag = tag;
            Header = LanguageManager.GetInstance().ConvertSpecifier("_Edit") + $" \"{tag.Name}\" " + LanguageManager.GetInstance().ConvertSpecifier("Properties");
            InputGestureText = "Ctrl + Enter";
            Click += EditAoiTagMenuItem_Click;
        }

        private void EditAoiTagMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditAoiTag(_tag);
        }

        public static void EditAoiTag(ITag tag)
        {
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            ICreateDialogService createDialogService =
                Package.GetGlobalService(typeof(SCreateDialogService)) as
                    ICreateDialogService;
            var dialog = createDialogService?.CreateTagProperties(tag);
            dialog.Show(uiShell);
        }
    }

    #endregion

    #region Online

    public class OnlineEditsMenuitem : MenuItem
    {
        public OnlineEditsMenuitem(STRoutine routine, StxEditorOptions options, StxEditorDocument document)
        {
            Header = LanguageManager.GetInstance().ConvertSpecifier("Online Edits");
            Items.Add(new StartPendingRoutineMenuitem(routine, options, document));
            Items.Add(new AcceptPendingRoutineEditsMenuitem(routine, document, options));
            Items.Add(new CancelPendingRoutineEditsMenuitem(routine, document, options));
            //Items.Add(new Separator());
            //Items.Add(new AssembleAcceptedRoutineEditsMenuitem(routine, options));
            //Items.Add(new CancelAcceptedRoutineEditsMenuitem(routine, options));
            //Items.Add(new Separator());
            //Items.Add(new AcceptPendingProgramEditsMenuitem(routine));
            //Items.Add(new CancelPendingProgramEditsMenuitem(routine));
            //Items.Add(new Separator());
            //Items.Add(new TestAcceptedProgramEditsMenuitem(routine, options));
            //Items.Add(new UntestAcceptedProgramEditsMenuitem(routine, options));
            //Items.Add(new Separator());
            //Items.Add(new AssembleAcceptedProgramEditsMenuitem(routine, options));
            //Items.Add(new CancelAcceptedProgramEditsMenuitem(routine, options));
            Items.Add(new Separator());
            Items.Add(new FinalizeAllEditsInProgramMenuitem(routine));
        }
    }

    public class StartPendingRoutineMenuitem : MenuItem
    {
        private STRoutine _routine;
        private StxEditorDocument _document;
        private StxEditorOptions _options;

        public StartPendingRoutineMenuitem(STRoutine routine, StxEditorOptions options, StxEditorDocument document)
        {
            Header = LanguageManager.GetInstance().ConvertSpecifier("Start Pending Routine Edits");
            IsEnabled = CanStartPendingRoutine(routine);
            Click += StartPendingRoutineMenuitem_Click;
            _routine = routine;
            _options = options;
            _document = document;
        }

        private void StartPendingRoutineMenuitem_Click(object sender, RoutedEventArgs e)
        {
            ExecuteStartPendingRoutine(_document, _options, _routine.ParentCollection.ParentProgram as Program);
        }

        public static bool CanStartPendingRoutine(STRoutine routine)
        {
            if (routine.ParentCollection?.ParentProgram is Program &&
                ((Program)routine.ParentCollection.ParentProgram).ParentTask == null) return false;
            if (routine.ParentController.KeySwitchPosition == ControllerKeySwitch.RunKeySwitch) return false;
            if (OnlineEditHelper.CompilingPrograms.Contains(routine.ParentCollection?.ParentProgram)) return false;
            if (routine.ParentController?.OperationMode == ControllerOperationMode.OperationModeFaulted) return false;
            if (!(routine.ParentCollection?.ParentProgram is Program)) return false;
            if (!Controller.GetInstance().IsOnline) return false;
            if ((routine.ParentCollection.ParentProgram as AoiDefinition)?.IsSealed ?? false) return false;
            return !routine.PendingEditsExist;
        }

        public static void ExecuteStartPendingRoutine(StxEditorDocument document, StxEditorOptions options,
            Program program)
        {
            document.Routine.CurrentOnlineEditType = OnlineEditType.Pending;
            document.Routine.StartPending();
            document.IsNeedReParse = false;
            document.IsNeedBackground = false;
            options.IsOnlyTextMarker = true;
            document.Pending = document.Original;
            options.IsRaiseCommandStatus = true;
            program.CheckPendingStatus();
            options.ResetLineInitial = true;
            document.IsNeedBackground = true;
            document.IsNeedReParse = true;
            options.IsOnlyTextMarker = false;
        }

    }

    public class AcceptPendingRoutineEditsMenuitem : MenuItem
    {
        private STRoutine _routine;
        private StxEditorDocument _document;
        private StxEditorOptions _options;

        public AcceptPendingRoutineEditsMenuitem(STRoutine routine, StxEditorDocument document,
            StxEditorOptions options)
        {
            Header = LanguageManager.GetInstance().ConvertSpecifier("Accept Pending Routine Edits");
            Click += AcceptPendingRoutineEditsMenuitem_Click;
            IsEnabled = CanAcceptPendingRoutineEdits(options, routine);
            _routine = routine;
            _document = document;
            _options = options;
        }

        private void AcceptPendingRoutineEditsMenuitem_Click(object sender, RoutedEventArgs e)
        {
            ExecuteAcceptPendingRoutineEdits(_routine, _document, _options);
        }

        public static bool CanAcceptPendingRoutineEdits(StxEditorOptions options, STRoutine routine)
        {
            if (routine.ParentController.KeySwitchPosition == ControllerKeySwitch.RunKeySwitch) return false;
            if (OnlineEditHelper.CompilingPrograms.Contains(routine.ParentCollection.ParentProgram)) return false;
            if (routine.ParentController.OperationMode == ControllerOperationMode.OperationModeFaulted) return false;
            if (!(routine.ParentCollection.ParentProgram is Program)) return false;
            if (!options.CanAcceptPendingRoutineCommand) return false;
            if ((routine.ParentCollection.ParentProgram as AoiDefinition)?.IsSealed ?? false) return false;
            return routine.PendingEditsExist;
        }

        public static void ExecuteAcceptPendingRoutineEdits(
            STRoutine routine,
            StxEditorDocument document,
            StxEditorOptions options)
        {
            FinalizeAllEditsInProgramMenuitem.OnlineEditCtrl.DisableOnlineEdit(null);
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                try
                {
                    if (document.NeedParse)
                    {
                        document.IsNeedBackground = false;
                        document.SnippetLexer.ParserWholeCode(document.GetCurrentCode(), false, true);
                        document.IsNeedBackground = true;
                    }

                    if (routine.IsError)
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        FinalizeAllEditsInProgramMenuitem.OnlineEditCtrl.EnableOnlineEdit((IProgram)routine.ParentCollection.ParentProgram);
                        return;
                    }
                    //暂时去除test
                    //if (routine.ParentController.OperationMode == ControllerOperationMode.OperationModeProgram)
                    //{
                    //    if (document.HasTest)
                    //    {
                    //        document.Test = document.Pending;
                    //        options.ShowTest = true;
                    //    }
                    //    else
                    //    {
                    //        document.Original = document.Pending;
                    //        options.HideAll = true;
                    //    }

                    //    document.Pending = null;
                    //}
                    //else
                    //{
                    //    document.Test = document.Pending;
                    //    document.Pending = null;
                    //    options.ShowTest = true;
                    //}
                    document.IsNeedReParse = false;
                    document.IsNeedBackground = false;
                    options.IsOnlyTextMarker = true;
                    routine.GenCode(routine.ParentCollection.ParentProgram,routine.PendingCodeText);
                    document.Routine.CurrentOnlineEditType=OnlineEditType.Original;
                    document.Routine.ApplyPending();
                    document.Original = document.Pending;
                    options.HideAll = true;

                    document.Pending = null;
                    options.IsOnlyTextMarker = false;
                    document.IsNeedBackground = true;
                    options.IsRaiseCommandStatus = true;

                    var program =
                        (routine.ParentCollection.ParentProgram as Program);
                    if (program != null)
                    {
                        if (routine.GetCurrentVariableInfos(OnlineEditType.Original).Any(v => v.IsUseForJSR))
                        {
                            program.UpdateRoutineRunStatus = true;
                        }

                        program.CheckPendingStatus();
                    }


                    if (!routine.ParentController.IsOnline)
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        FinalizeAllEditsInProgramMenuitem.OnlineEditCtrl.EnableOnlineEdit((IProgram)routine.ParentCollection.ParentProgram);
                        return;
                    }

                    await TaskScheduler.Default;

                    OnlineEditHelper onlineEditHelper =
                        new OnlineEditHelper(((Controller) routine.ParentController).CipMessager);

                    await onlineEditHelper.ReplaceRoutine(routine,false);

                    await onlineEditHelper.UpdateProgram(program);

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    options.IsDirty = false;
                    options.ResetLineInitial = true;
                }
                catch (Exception e)
                {
                    Controller.GetInstance().Log($"ExecuteAcceptPendingRoutineEdits error:{e.Message}");
                    Controller.GetInstance().Log(e.StackTrace);
                    var errorWindow = Package.GetGlobalService(typeof(SErrorOutputService)) as IErrorOutputService;
                    errorWindow?.AddErrors(
                        $"Compile Error",
                        OrderType.Order, routine.CurrentOnlineEditType, null, null,
                        routine);
                }
                finally
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    FinalizeAllEditsInProgramMenuitem.OnlineEditCtrl.EnableOnlineEdit(
                        (IProgram) routine.ParentCollection.ParentProgram);
                }

            });
        }
    }

    public class CancelPendingRoutineEditsMenuitem : MenuItem
    {
        private STRoutine _routine;
        private StxEditorDocument _document;
        private StxEditorOptions _options;

        public CancelPendingRoutineEditsMenuitem(STRoutine routine, StxEditorDocument document,
            StxEditorOptions options)
        {
            Header = LanguageManager.GetInstance().ConvertSpecifier("Cancel Pending Routine Edits");
            IsEnabled = CanCancelPendingRoutineEdits(routine.ParentCollection.ParentProgram as Program, routine);
            _routine = routine;
            _document = document;
            _options = options;
            Click += CancelPendingRoutineEditsMenuitem_Click;
        }

        private void CancelPendingRoutineEditsMenuitem_Click(object sender, RoutedEventArgs e)
        {
            ExecuteCancelPendingRoutineEdits(_document, _options, _routine.ParentCollection.ParentProgram as Program);
        }

        public static bool CanCancelPendingRoutineEdits(Program program, STRoutine routine)
        {
            if (routine.ParentController.KeySwitchPosition == ControllerKeySwitch.RunKeySwitch) return false;
            if (OnlineEditHelper.CompilingPrograms.Contains(routine.ParentCollection.ParentProgram)) return false;
            if (program == null) return false;
            if ((routine.ParentCollection.ParentProgram as AoiDefinition)?.IsSealed ?? false) return false;
            return routine.PendingEditsExist;
        }

        public static void ExecuteCancelPendingRoutineEdits(StxEditorDocument document, StxEditorOptions options,
            Program program)
        {
            document.IsNeedBackground = false;
            options.IsOnlyTextMarker = true;
            document.Pending = null;
            if (document.HasTest)
                options.ShowTest = true;
            else options.ShowOriginal = true;
            options.IsRaiseCommandStatus = true;
            program.CheckPendingStatus();
            options.IsOnlyTextMarker = false;
            document.IsNeedBackground = true;

            options.ResetLineInitial = true;
        }
    }

    public class AssembleAcceptedRoutineEditsMenuitem : MenuItem
    {
        public AssembleAcceptedRoutineEditsMenuitem(STRoutine routine, StxEditorOptions options)
        {
            Header = LanguageManager.GetInstance().ConvertSpecifier("Assemble Accepted Routine Edits");
            IsEnabled = CanAssembleAcceptedRoutineEdits(routine, options);
            Click += AssembleAcceptedRoutineEditsMenuitem_Click;
        }

        private void AssembleAcceptedRoutineEditsMenuitem_Click(object sender, RoutedEventArgs e)
        {
            ExecuteAssembleAcceptedRoutineEdits();
        }

        public static bool CanAssembleAcceptedRoutineEdits(STRoutine routine, StxEditorOptions options)
        {
            if (routine.ParentController.OperationMode == ControllerOperationMode.OperationModeFaulted) return false;
            var program = routine.ParentCollection.ParentProgram as Program;
            if (program == null) return false;
            if (program.TestEditsMode == TestEditsModeType.Test) return false;
            if ((routine.ParentCollection.ParentProgram as AoiDefinition)?.IsSealed ?? false) return false;
            return options.CanAssembledAcceptPendingRoutineCommand;
        }

        public static void ExecuteAssembleAcceptedRoutineEdits()
        {

        }
    }

    public class CancelAcceptedRoutineEditsMenuitem : MenuItem
    {
        public CancelAcceptedRoutineEditsMenuitem(STRoutine routine, StxEditorOptions options)
        {
            Header = LanguageManager.GetInstance().ConvertSpecifier("Cancel Accepted Routine Edits");
            IsEnabled = CanCancelAcceptedRoutineEdits(routine, options);
            Click += CancelAcceptedRoutineEditsMenuitem_Click;
        }

        private void CancelAcceptedRoutineEditsMenuitem_Click(object sender, RoutedEventArgs e)
        {
            ExecuteCancelAcceptedRoutineEdits();
        }

        public static bool CanCancelAcceptedRoutineEdits(STRoutine routine, StxEditorOptions options)
        {
            var program = routine.ParentCollection.ParentProgram as Program;
            if (program == null) return false;
            if ((routine.ParentCollection.ParentProgram as AoiDefinition)?.IsSealed ?? false) return false;
            return options.CanCancelAcceptedPendingRoutineCommand;
        }

        public static void ExecuteCancelAcceptedRoutineEdits()
        {

        }
    }

    public class AcceptPendingProgramEditsMenuitem : MenuItem
    {
        private STRoutine _routine=null;

        public AcceptPendingProgramEditsMenuitem(STRoutine routine)
        {
            Header = LanguageManager.GetInstance().ConvertSpecifier("Accept Pending Program Edits");
            IsEnabled = CanAcceptPendingProgramEdits(routine);
            Click += AcceptPendingProgramEditsMenuitem_Click;
        }

        private void AcceptPendingProgramEditsMenuitem_Click(object sender, RoutedEventArgs e)
        {
            ExecuteAcceptPendingProgramEdits(_routine);
        }

        public static bool CanAcceptPendingProgramEdits(STRoutine routine)
        {
            return false;
            //if (routine.ParentController.KeySwitchPosition == ControllerKeySwitch.RunKeySwitch) return false;
            //if (routine.ParentController.OperationMode == ControllerOperationMode.OperationModeFaulted) return false;
            //var program = routine.ParentCollection.ParentProgram as Program;
            //if (program == null) return false;
            //if (program.TestEditsMode == TestEditsModeType.Test) return false;
            //if ((routine.ParentCollection.ParentProgram as AoiDefinition)?.IsSealed ?? false) return false;
            //return (routine.ParentCollection.ParentProgram as Program)?.HasPending ?? false;
        }

        public static void ExecuteAcceptPendingProgramEdits(STRoutine routine)
        {
            var program = routine.ParentCollection.ParentProgram as Program;
            if (MessageBox.Show($"Accept the Pending Edits of program '{program?.Name}'?", "ICS Studio",
                    MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                CommonFunction.VerifyAllRoutine(program);
                program?.AcceptPending();
            }
        }
    }

    static class CommonFunction
    {
        public static void VerifyAllRoutine(IProgram program)
        {
            if (program == null) return;
            var createEditorService =
                Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
            foreach (var routine in program.Routines)
            {
                createEditorService?.ParseRoutine(routine, true);
            }
        }
    }

    public class CancelPendingProgramEditsMenuitem : MenuItem
    {
        private STRoutine _routine;

        public CancelPendingProgramEditsMenuitem(STRoutine routine)
        {
            Header = LanguageManager.GetInstance().ConvertSpecifier("Cancel Pending Program Edits");
            IsEnabled = CanCancelPendingProgramEdits(routine);
            _routine = routine;
            Click += CancelPendingProgramEditsMenuitem_Click;
        }

        private void CancelPendingProgramEditsMenuitem_Click(object sender, RoutedEventArgs e)
        {
            ExecuteCancelPendingProgramEdits(_routine);
        }

        public static bool CanCancelPendingProgramEdits(STRoutine routine)
        {
            return false;
            //if (routine.ParentController.KeySwitchPosition == ControllerKeySwitch.RunKeySwitch) return false;
            //var program = routine.ParentCollection.ParentProgram as Program;
            //if (program == null) return false;
            //if ((routine.ParentCollection.ParentProgram as AoiDefinition)?.IsSealed ?? false) return false;
            //return program?.HasPending ?? false;
        }

        public static void ExecuteCancelPendingProgramEdits(STRoutine routine)
        {
            var program = routine.ParentCollection.ParentProgram as Program;
            if (MessageBox.Show($"Cancel the Pending Edits of program '{program.Name}'?", "ICS Studio",
                    MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                program?.CancelPending();
        }
    }

    public class TestAcceptedProgramEditsMenuitem : MenuItem
    {
        private STRoutine _routine;
        private StxEditorOptions _options;

        public TestAcceptedProgramEditsMenuitem(STRoutine routine, StxEditorOptions options)
        {
            Header = LanguageManager.GetInstance().ConvertSpecifier("Test Accepted Program Edits");
            _routine = routine;
            _options = options;
            IsEnabled = CanTestAcceptedProgramEdits(routine);
            Click += TestAcceptedProgramEditsMenuitem_Click;
        }

        private void TestAcceptedProgramEditsMenuitem_Click(object sender, RoutedEventArgs e)
        {
            ExecuteTestAcceptedProgramEdits(_routine, _options);
        }

        public static bool CanTestAcceptedProgramEdits(STRoutine routine)
        {
            return false;
            //var program = routine.ParentCollection.ParentProgram as Program;
            //if (program == null) return false;
            //if ((routine.ParentCollection.ParentProgram as AoiDefinition)?.IsSealed ?? false) return false;
            //return program?.TestEditsMode == TestEditsModeType.UnTest;
        }

        public static void ExecuteTestAcceptedProgramEdits(STRoutine routine, StxEditorOptions options)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var program = routine.ParentCollection.ParentProgram as Program;
            var dialog = new TestProgramCheckDialog(program, true);
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            if (dialog.ShowDialog(uiShell) ?? false)
            {
                program.TestEditsMode = TestEditsModeType.Test;
                options.IsRaiseCommandStatus = true;
            }
        }
    }

    public class UntestAcceptedProgramEditsMenuitem : MenuItem
    {
        private STRoutine _routine;
        private StxEditorOptions _options;

        public UntestAcceptedProgramEditsMenuitem(STRoutine routine, StxEditorOptions options)
        {
            Header = LanguageManager.GetInstance().ConvertSpecifier("Untest Accepted Program Edits");
            _routine = routine;
            _options = options;
            IsEnabled = CanUntestAcceptedProgramEdits(routine);
            Click += UntestAcceptedProgramEditsMenuitem_Click;
        }

        private void UntestAcceptedProgramEditsMenuitem_Click(object sender, RoutedEventArgs e)
        {
            ExecuteUntestAcceptedProgramEdits(_routine, _options);
        }

        public static bool CanUntestAcceptedProgramEdits(STRoutine routine)
        {
            var program = routine.ParentCollection.ParentProgram as Program;
            if (program == null) return false;
            if ((routine.ParentCollection.ParentProgram as AoiDefinition)?.IsSealed ?? false) return false;
            return program?.TestEditsMode == TestEditsModeType.Test;
        }

        public static void ExecuteUntestAcceptedProgramEdits(STRoutine routine, StxEditorOptions options)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var program = routine.ParentCollection.ParentProgram as Program;
            var dialog = new TestProgramCheckDialog(program, false);
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            if (dialog.ShowDialog(uiShell) ?? false)
            {
                program.TestEditsMode = TestEditsModeType.UnTest;
                options.IsRaiseCommandStatus = true;
            }
        }
    }

    public class AssembleAcceptedProgramEditsMenuitem : MenuItem
    {
        private STRoutine _routine;
        private StxEditorOptions _options;

        public AssembleAcceptedProgramEditsMenuitem(STRoutine routine, StxEditorOptions options)
        {
            Header = LanguageManager.GetInstance().ConvertSpecifier("Assemble Accepted Program Edits");
            _routine = routine;
            _options = options;
            IsEnabled = CanAssembleAcceptedProgramEdits(routine, options);
            Click += AssembleAcceptedProgramEditsMenuitem_Click;
        }

        private void AssembleAcceptedProgramEditsMenuitem_Click(object sender, RoutedEventArgs e)
        {
            ExecuteAssembleAcceptedProgramEdits(_routine);
        }

        public static bool CanAssembleAcceptedProgramEdits(STRoutine routine, StxEditorOptions options)
        {
            var program = routine.ParentCollection.ParentProgram as Program;
            if (program == null) return false;
            if ((routine.ParentCollection.ParentProgram as AoiDefinition)?.IsSealed ?? false) return false;
            return options.CanAssembledAcceptedProgramCommand;
        }

        public static void ExecuteAssembleAcceptedProgramEdits(STRoutine routine)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var program = routine.ParentCollection.ParentProgram as Program;
            var dialog = new AcceptedProgramEditsDialog(program, true);
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            if (dialog.ShowDialog(uiShell) ?? false)
                program.AssembledAccepted();
        }
    }

    public class CancelAcceptedProgramEditsMenuitem : MenuItem
    {
        private STRoutine _routine;
        private StxEditorOptions _options;

        public CancelAcceptedProgramEditsMenuitem(STRoutine routine, StxEditorOptions options)
        {
            Header = LanguageManager.GetInstance().ConvertSpecifier("Cancel Accepted Program Edits");
            _routine = routine;
            _options = options;
            IsEnabled = CanCancelAcceptedProgramEdits(routine, options);
            Click += CancelAcceptedProgramEditsMenuitem_Click;
        }

        private void CancelAcceptedProgramEditsMenuitem_Click(object sender, RoutedEventArgs e)
        {
            ExecuteCancelAcceptedProgramEdits(_routine);
        }

        public static bool CanCancelAcceptedProgramEdits(STRoutine routine, StxEditorOptions options)
        {
            return false;
            //var program = routine.ParentCollection.ParentProgram as Program;
            //if (program == null) return false;
            //if ((routine.ParentCollection.ParentProgram as AoiDefinition)?.IsSealed ?? false) return false;
            //return options.CanCancelAcceptedProgramCommand;
        }

        public static void ExecuteCancelAcceptedProgramEdits(STRoutine routine)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var program = routine.ParentCollection.ParentProgram as Program;
            var dialog = new AcceptedProgramEditsDialog(program, false);
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            if (dialog.ShowDialog(uiShell) ?? false)
                program.CancelAccepted();
        }
    }

    public class FinalizeAllEditsInProgramMenuitem : MenuItem
    {
        private STRoutine _routine;

        public FinalizeAllEditsInProgramMenuitem(STRoutine routine)
        {
            Header = LanguageManager.GetInstance().ConvertSpecifier("Finalize All Edits in Program");
            _routine = routine;
            IsEnabled = CanFinalizeAllEditsInProgram(routine);
            Click += FinalizeAllEditsInProgramMenuitem_Click;
        }

        private void FinalizeAllEditsInProgramMenuitem_Click(object sender, RoutedEventArgs e)
        {
            ExecuteFinalizeAllEditsInProgram(_routine);
        }

        public static bool CanFinalizeAllEditsInProgram(STRoutine routine)
        {
            if (OnlineEditHelper.CompilingPrograms.Contains(routine.ParentCollection.ParentProgram)) return false;
            if (routine.ParentController.OperationMode == ControllerOperationMode.OperationModeFaulted) return false;
            var program = routine.ParentCollection.ParentProgram as Program;
            if (program == null) return false;
            return program.HasTest || program.HasPending;
        }

        public static void ExecuteFinalizeAllEditsInProgram(STRoutine st)
        {
            OnlineEditCtrl.DisableOnlineEdit(st.ParentCollection.ParentProgram as IProgram);

            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate()
            {
                STRoutine stRoutine = null;
                try
                {
                    var program = st.ParentCollection.ParentProgram as Program;
                    if (program == null)
                    {
                        //await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        //OnlineEditCtrl.EnableOnlineEdit();
                        return;
                    }
                    var dialog = new FinalizeDialog(program);
                    var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    if (dialog.ShowDialog(uiShell) ?? false)
                    {
                        await TaskScheduler.Default;
                        var createEditorService =
                            Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;

                        foreach (var r in program.Routines)
                        {
                            createEditorService?.ParseRoutine(r, true, true, true, true);
                        }

                        if (!program.Routines.Any(s => s.IsError))
                        {
                            var ctrl = (Controller) st.ParentController;
                            OnlineEditHelper onlineEditHelper = new OnlineEditHelper(ctrl.CipMessager);
                            var routines = st.ParentCollection;
                            foreach (var routine in routines)
                            {
                                stRoutine = routine as STRoutine;
                                if (stRoutine != null)
                                {
                                    if (stRoutine.TestCodeText != null)
                                    {
                                        stRoutine.GenCode(stRoutine.ParentCollection.ParentProgram,stRoutine.TestCodeText);
                                        stRoutine.ApplyTest();
                                        continue;
                                    }

                                    if (stRoutine.PendingEditsExist)
                                    {
                                        stRoutine.GenCode(stRoutine.ParentCollection.ParentProgram, stRoutine.PendingCodeText);
                                        stRoutine.ApplyPending();
                                    }

                                    //TODO(ZYL):Add other routines
                                }
                            }

                            if (!routines.Any(r => r.IsError))
                            {
                                foreach (var routine in routines)
                                {
                                    stRoutine = routine as STRoutine;
                                    if (stRoutine != null && stRoutine.IsModified)
                                    {
                                        await onlineEditHelper.ReplaceRoutine(stRoutine,false);
                                    }
                                }

                                await onlineEditHelper.UpdateProgram(program);

                                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                                if (program.TestEditsMode == TestEditsModeType.Null)
                                {
                                    program.RaisePropertyChanged("TestEditsMode");
                                }
                                else
                                {
                                    program.TestEditsMode = TestEditsModeType.Null;
                                }

                                program.UpdateRoutineRunStatus = true;
                                program.ExecuteFinalizeCode = true;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Controller.GetInstance().Log($"ExecuteFinalizeAllEditsInProgram error:{e.Message}");
                    Controller.GetInstance().Log(e.StackTrace);
                    var errorWindow = Package.GetGlobalService(typeof(SErrorOutputService)) as IErrorOutputService;
                    errorWindow?.AddErrors(
                        $"Compile Error",
                        OrderType.Order, stRoutine.CurrentOnlineEditType, null,null,
                        stRoutine);
                }
                finally
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    OnlineEditCtrl.EnableOnlineEdit((IProgram)st.ParentCollection.ParentProgram);
                }
            });
        }

        public class OnlineEditCtrl
        {
            internal static void EnableOnlineEdit([NotNull] IProgram compiledProgram)
            {
                if (compiledProgram == null) throw new ArgumentNullException(nameof(compiledProgram));
                OnlineEditHelper.CompilingPrograms.TryTake(out compiledProgram);
                var createEditorService =
                    Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
                createEditorService?.UpdateAllRoutineOnlineStatus();
            }

            internal static void DisableOnlineEdit(IProgram compilingProgram)
            {
                OnlineEditHelper.CompilingPrograms.Add(compilingProgram);
                var createEditorService =
                    Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
                createEditorService?.UpdateAllRoutineOnlineStatus();
            }
        }


    }

    #endregion

    #region Routine

    internal class EditRoutineMenuItem : MenuItem
    {
        private readonly IRoutine _routine;

        public EditRoutineMenuItem(IRoutine routine)
        {
            _routine = routine;

            Header = LanguageManager.GetInstance().ConvertSpecifier("Edit") + $" \"{routine.Name}\" "+
                     LanguageManager.GetInstance().ConvertSpecifier("Properties");
            InputGestureText = "Alt + Enter";
            Click += EditRoutineMenu_Click;
        }

        private void EditRoutineMenu_Click(object sender, RoutedEventArgs e)
        {
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            var createDialogService = Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;
            createDialogService?.CreateRoutineProperties(_routine).Show(uiShell);
        }
    }

    internal class CrossReferenceRoutineMenuItem : MenuItem
    {
        private readonly string _name;
        private readonly IRoutine _routine;

        public CrossReferenceRoutineMenuItem(string name,IRoutine routine)
        {
            _name = name;
            _routine = routine;

            Header =LanguageManager.GetInstance().ConvertSpecifier("Go To") + $" \"{name}\" " + 
                    LanguageManager.GetInstance().ConvertSpecifier("Cross Reference For");
            InputGestureText = "Ctrl + E";
            Click += CrossReferenceRoutineMenu_Click;
        }

        private void CrossReferenceRoutineMenu_Click(object sender, RoutedEventArgs e)
        {
            var createEditorService = Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
            createEditorService?.CreateCrossReference(Type.Routine,_routine.ParentCollection.ParentProgram,_name);
        }
    }

    internal class OpenRoutineMenuItem : MenuItem
    {
        private readonly string _name;
        private readonly IRoutine _routine;

        public OpenRoutineMenuItem(string name, IRoutine routine)
        {
            _name = name;
            _routine = routine;

            Header = LanguageManager.GetInstance().ConvertSpecifier("Edit") + $" \"{name}\"";
            Click += OpenRoutineMenu_Click;
        }

        private void OpenRoutineMenu_Click(object sender, RoutedEventArgs e)
        {
            var createEditorService = Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
            createEditorService?.CreateSTEditor(_routine.ParentCollection[_name]);
        }
    }

    internal class BrowseRoutineMenuItem : MenuItem
    {
        private readonly TextEditor _textEditor;
        private readonly Point _point;
        private readonly BrowseRoutinesAdorner _browseAdorner;
        private readonly SnippetInfo _info;
        private readonly Action _closeAdorner;

        public BrowseRoutineMenuItem(TextEditor editor, Point point, BrowseRoutinesAdorner browseAdorner, SnippetInfo info)
        {
            _textEditor = editor;
            _browseAdorner = browseAdorner;
            Debug.Assert(_browseAdorner != null);
            _info = info;
            _point = TestHit(editor, point);
            
            Header = LanguageManager.GetInstance().ConvertSpecifier("Browse Routines");
            InputGestureText = "Ctrl + Alt + Space";
            Click += BrowseRoutineMenu_Click;

            _browseAdorner?.ResetAdorner(_point, _textEditor.FontSize / 12, _info.CodeText);

            _closeAdorner = () =>
            {
                var name = _browseAdorner.GetName();
                _textEditor.Document.Replace(_info.Offset, _info.CodeText?.Length ?? 0,
                    new StringTextSource(name ?? ""));
                _textEditor.GotFocus -= Editor_GotFocus;
                _browseAdorner.CloseAdorner -= _closeAdorner;
                var layer = AdornerLayer.GetAdornerLayer(_textEditor);
                layer?.Remove(_browseAdorner);
            };
        }

        private void BrowseRoutineMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var layer = AdornerLayer.GetAdornerLayer(_textEditor);
                layer?.Add(_browseAdorner);
                _textEditor.GotFocus += Editor_GotFocus;
                _browseAdorner.CloseAdorner += _closeAdorner;
                Dispatcher.BeginInvoke((Action)delegate { _browseAdorner.SetTextFocus(); },
                    DispatcherPriority.ApplicationIdle);
            }
            catch (Exception)
            {
                Debug.WriteLine("BrowseRoutineMenu_Click has an error ！");
            }
        }

        private void Editor_GotFocus(object sender, RoutedEventArgs e)
        {
            _closeAdorner?.Invoke();
        }

        private Point TestHit(TextEditor editor, Point point)
        {
            double width = editor.ActualWidth - 50, height = editor.ActualHeight;
            var newPoint = point;
            if (point.X >= width || point.X + 150 >= width) newPoint.X = width - 150;
            if (point.Y >= height || point.Y + 25 >= height) newPoint.Y = height - 25;
            return newPoint;
        }
    }

    internal class NewRoutineMenuItem : MenuItem
    {
        private readonly string _name;
        private readonly IProgramModule _program;

        public NewRoutineMenuItem(string name, IProgramModule program)
        {
            _name = name;
            _program = program;

            Header = LanguageManager.GetInstance().ConvertSpecifier("New Routine") + $" \"{name}\"";
            Click += EditRoutineMenu_Click;

            if (program is AoiDefinition) IsEnabled = false;
        }

        private void EditRoutineMenu_Click(object sender, RoutedEventArgs e)
        {
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            var createDialogService = Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;
            createDialogService?.CreateRoutineDialog(_program,_name).ShowDialog(uiShell);
        }
    }

    #endregion
}