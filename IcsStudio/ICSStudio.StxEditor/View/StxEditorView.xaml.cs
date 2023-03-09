using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions; 
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using ICSStudio.AvalonEdit;
using ICSStudio.AvalonEdit.CodeCompletion;
using ICSStudio.AvalonEdit.Document;
using ICSStudio.AvalonEdit.Editing;
using ICSStudio.AvalonEdit.Folding;
using ICSStudio.AvalonEdit.Highlighting;
using ICSStudio.AvalonEdit.Highlighting.Xshd;
using ICSStudio.AvalonEdit.Snippets;
using ICSStudio.AvalonEdit.Variable;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Instruction;
using ICSStudio.SimpleServices.Online;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.StxEditor.Interfaces;
using ICSStudio.StxEditor.Menu;
using ICSStudio.StxEditor.ViewModel;
using ICSStudio.StxEditor.ViewModel.CodeSnippets;
using ICSStudio.StxEditor.ViewModel.Folding;
using ICSStudio.StxEditor.ViewModel.Highlighting;
using ICSStudio.StxEditor.ViewModel.Indentation;
using ICSStudio.StxEditor.ViewModel.IntelliPrompt;
using ICSStudio.StxEditor.ViewModel.Tooltip;
using NLog;
using Type = ICSStudio.StxEditor.ViewModel.CodeSnippets.Type;

namespace ICSStudio.StxEditor.View
{
    /// <summary>
    ///     StxEditorView.xaml 的交互逻辑
    /// </summary>
    public partial class StxEditorView
    {
        private static readonly IHighlightingDefinition StxHighlightingDefinition;
        private readonly IFoldingStrategy _foldingStrategy;
        private readonly IStxEditorOptions _options;
        private readonly IController _controller;
        private readonly STRoutine _stRoutine;
        private readonly TextMarkerService _textMarkerService;
        private readonly ComponentContentProvider _componentContentProvider;
        private MenuSelector _menuSelector;
        private readonly StxEditorViewModel _stxEditorViewModel;
        private readonly StxEditorDocument _document;
        private CompletionWindow _completionWindow;
        private FoldingManager _foldingManager;
        private ToolTip _toolTip;
        internal static readonly ILogger Logger = LogManager.GetCurrentClassLogger();


        static StxEditorView()
        {
            // Load ST highlighting definition
            using (var s =
                typeof(StxEditorView).Assembly.GetManifestResourceStream("ICSStudio.StxEditor.StxHighlighting.xshd"))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");

                using (var reader = new XmlTextReader(s))
                {
                    StxHighlightingDefinition = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }

        public StxEditorView(STRoutine routine, StxEditorDocument document, IStxEditorOptions options)
        {
            Loaded += StxEditorView_Loaded;
            InitializeComponent();
            _stRoutine = routine;
            _document = document;

            _trackToolTip.ToolTipOpening += _trackToolTip_ToolTipOpening;
            _controller = _stRoutine.ParentController;
            SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Display);
            _textMarkerService = document.TextMarkerService;
            Editor.FontFamily = new FontFamily(options.FontFamilyName);
            Editor.FontSize = options.FontSize;
            Editor.SyntaxHighlighting = StxHighlightingDefinition;
            Editor.ShowLineNumbers = options.ShowLineNumbers;
            Editor.Background=new SolidColorBrush(Color.FromRgb(255,255,248));
            Editor.LineNumbersForeground = new SolidColorBrush(options.GetItemColor(StxTextItem.LineNumber).Foreground);
            WeakEventManager<TextMarkerService, NotifyCollectionChangedEventArgs>.AddHandler(_textMarkerService,
                "CollectionChanged", TextMarkerService_CollectionChanged);

            WeakEventManager<TextArea, TextCompositionEventArgs>.AddHandler(Editor.TextArea, "TextEntering",
                TextAreaOnTextEntering);

            WeakEventManager<TextArea, TextCompositionEventArgs>.AddHandler(Editor.TextArea, "TextEntered",
                TextAreaOnTextEntered);

            _stxEditorViewModel = new StxEditorViewModel(routine, document, options, Editor,
                document.SnippetLexer.StxCompletionItemCodeSnippetDatas);
            _options = options;
            DataContext = _stxEditorViewModel;

            _foldingStrategy = new BraceFoldingStrategy();

            WeakEventManager<TextEditor, MouseEventArgs>.AddHandler(Editor, "MouseHoverStopped",
                TextEditorMouseHoverStopped);

            _componentContentProvider = new ComponentContentProvider(_stRoutine.ParentCollection.ParentProgram.Tags,
                document.SnippetLexer.StxCompletionItemCodeSnippetDatas, document);

            Editor.TextArea.TextView.SetRoutine(routine);

            Editor.TextArea.TextView.BackgroundRenderers.Add(_textMarkerService);
            Editor.TextArea.TextView.LineTransformers.Add(_textMarkerService);
            Editor.TextArea.TextView.Services.AddService(typeof(TextMarkerService), _textMarkerService);
            WeakEventManager<TextEditor, KeyEventArgs>.AddHandler(Editor, "PreviewKeyDown", TextArea_PreviewKeyDown);

            WeakEventManager<TextEditor, ContextMenuEventArgs>.AddHandler(Editor, "ContextMenuOpening",
                TextEditor_ContextMenuOpening);
            _menuSelector = new MenuSelector(document, Editor);
            WeakEventManager<TextDocument, DocumentChangeEventArgs>.AddHandler(document.Document, "Changed",
                Document_Changed);

            WeakEventManager<TextEditor, MouseButtonEventArgs>.AddHandler(Editor, "PreviewMouseDown",
                Editor_PreviewMouseDown);
            WeakEventManager<TextEditor, RoutedEventArgs>.AddHandler(Editor, "GotFocus", Editor_GotFocus);
            Editor.LostFocus += Editor_LostFocus;
            WeakEventManager<TextEditor, DragEventArgs>.AddHandler(Editor, "PreviewDragEnter", Editor_PreviewDragEnter);
            WeakEventManager<TextEditor, EventArgs>.AddHandler(Editor, "DocumentChanged", Editor_DocumentChanged);
            WeakEventManager<TextArea, EventArgs>.AddHandler(Editor.TextArea, "LineChanged", LineChanged);

            PropertyChangedEventManager.AddHandler(document, Document_PropertyChanged, "");
            Editor.Options.AllowScrollBelowDocument = true;
            PreviewKeyUp += StxEditorView_PreviewKeyUp;
            PreviewKeyDown += StxEditorView_PreviewKeyDown;
            Editor.TextArea.Caret.PositionChanged += Caret_PositionChanged;
        }
        

        private int _caretIndex = 0;
        public void RememberCaretIndex()
        {
            _caretIndex = TextEditor.CaretOffset;
        }

        public void RecoverCaretIndex()
        {
            if (_caretIndex > Editor.Text.Length)
            {
                Editor.CaretOffset = Editor.Text.Length;
            }
            else
            {
                TextEditor.CaretOffset = _caretIndex;
            }
        }

        private void StxEditorView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control &&
                (e.Key != Key.LeftCtrl || e.Key != Key.RightCtrl || e.Key != Key.LeftAlt || e.Key != Key.RightAlt ||
                 e.Key != Key.LeftShift || e.Key != Key.RightShift))
            {
                _trackToolTip.IsOpen = false;
            }
        }

        private void Editor_LostFocus(object sender, RoutedEventArgs e)
        {
            _trackToolTip.IsOpen = false;
        }

        private void StxEditorView_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (_trackToolTip != null)
                    _trackToolTip.IsOpen = false;
            }
        }

        private void Caret_PositionChanged(object sender, EventArgs e)
        {
            if(_trackToolTip.IsOpen)
                TrackInstrParametersEnter();
        }

        private void Editor_PreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = _options.CanEditorInput ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void Editor_DocumentChanged(object sender, EventArgs e)
        {
            Editor.SetLineInitial();
            ((StxEditorOptions) _options).IsDirty = false;
        }

        public void SetStatus(bool isAoiInRun)
        {
            ((StxEditorViewModel) DataContext).IsAoiInRun = isAoiInRun;
        }

        private void StxEditorView_Loaded(object sender, RoutedEventArgs e)
        {
            if (Name == "Top")
            {
                _options.IsTopLoaded = true;
            }
            else
            {
                _options.IsBottomLoaded = true;
            }
        }

        public void SetIsFocusChanged(bool isFocusChanged)
        {
            Editor.IsFocusChanged = isFocusChanged;
        }

        private void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName== "Update")
            {
                if (((StxEditorDocument)sender).Update)
                {
                    Editor.TextArea.TextView.Redraw(DispatcherPriority.ApplicationIdle);
                }
            }
        }

        private void LineChanged(object sender, EventArgs e)
        {
            if (_document.DoLineChangedEvent)
                _document.LineChange();
            _document.DoLineChangedEvent = true;
        }

        public STRoutine STRoutine => _stRoutine;

        public TextEditor TextEditor => Editor;

        public void Clean()
        {
            WeakEventManager<TextMarkerService, NotifyCollectionChangedEventArgs>.RemoveHandler(_textMarkerService,
                "CollectionChanged", TextMarkerService_CollectionChanged);
            WeakEventManager<TextArea, TextCompositionEventArgs>.RemoveHandler(Editor.TextArea, "TextEntering",
                TextAreaOnTextEntering);
            WeakEventManager<TextArea, TextCompositionEventArgs>.RemoveHandler(Editor.TextArea, "TextEntered",
                TextAreaOnTextEntered);
            Editor.TextArea.TextView.Services.Dispose();
            WeakEventManager<TextEditor, MouseEventArgs>.RemoveHandler(Editor, "MouseHoverStopped",
                TextEditorMouseHoverStopped);
            WeakEventManager<TextEditor, KeyEventArgs>.RemoveHandler(Editor, "PreviewKeyDown", TextArea_PreviewKeyDown);
            WeakEventManager<TextEditor, ContextMenuEventArgs>.RemoveHandler(Editor, "ContextMenuOpening",
                TextEditor_ContextMenuOpening);
            WeakEventManager<TextDocument, DocumentChangeEventArgs>.RemoveHandler(_document.Document, "Changed",
                Document_Changed);
            WeakEventManager<TextEditor, MouseButtonEventArgs>.RemoveHandler(Editor, "PreviewMouseDown",
                Editor_PreviewMouseDown);
            WeakEventManager<TextEditor, RoutedEventArgs>.RemoveHandler(Editor, "GotFocus", Editor_GotFocus);
            WeakEventManager<TextArea, EventArgs>.RemoveHandler(Editor.TextArea, "LineChanged", LineChanged);
            PropertyChangedEventManager.RemoveHandler(_document, Document_PropertyChanged, "");
            PreviewKeyUp -= StxEditorView_PreviewKeyUp;
            PreviewKeyDown -= StxEditorView_PreviewKeyDown;
            Editor.TextArea.Caret.PositionChanged -= Caret_PositionChanged;
            Editor.TextArea.TextView.RecoveryError();
        }

        private void Editor_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_document.ShowCaretControl.Equals(this))
                _document.ShowCaretControl = this;
        }

        private void Editor_GotFocus(object sender, RoutedEventArgs e)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(Editor);
            adornerLayer?.Remove(_stxEditorViewModel.BrowseAdorner);
            adornerLayer?.Remove(_stxEditorViewModel.BrowseEnumAdorner);
            _options.CanZoom = true;
        }

        public void SetSTFocus()
        {
            WeakEventManager<TextEditor, RoutedEventArgs>.AddHandler(Editor, "Loaded", Editor_Loaded2);
        }

        private void Editor_Loaded2(object sender, RoutedEventArgs e)
        {
            Editor.Focus();
            WeakEventManager<TextEditor, RoutedEventArgs>.RemoveHandler(Editor, "Loaded", Editor_Loaded2);
        }

        public void SetCaret(int offset, int len)
        {
            Editor.Focus();
            FocusManager.SetFocusedElement(Editor, Editor);
            Editor.CaretOffset = offset;
            if (len > 0)
            {
                Editor.Select(offset, len);
            }

            Editor.TextArea.TextView.ShowCaretLayer();
            Editor.ScrollToLine(Editor.Document.GetLineByOffset(offset).LineNumber);
        }

        private void Document_Changed(object sender, DocumentChangeEventArgs e)
        {
            //_foldingUpdateTimer.Start();
            try
            {
                UpdateFoldings();
                if (e.RemovalLength > 0 && e.InsertionLength == 0)
                {
                    if (_completionWindow != null && _completionWindow.IsVisible)
                    {
                        var data = _completionWindow.CompletionList.CompletionData;
                        data.Clear();
                        List<string> enums = new List<string>();
                        var targetDataType = GetSortDataType(ref enums);
                        int start = 0, end = 0;
                        var code = CompletionWindow.GetCode(Editor.TextArea, false, ref start, ref end);
                        var str = code.Item1 + code.Item2;
                        if (str.Length >= 1)
                        {
                            if (char.IsNumber(str[0]))
                            {
                                _completionWindow = null;
                                return;
                            }
                        }

                        if (enums?.Count > 0)
                        {
                            foreach (var @enum in enums)
                            {
                                var s = new StxCompletionItemKeywordData(@enum);
                                var m = new StxCompletionItem(s);
                                data.Add(m);
                            }
                        }
                        else
                        {
                            _stxEditorViewModel.UpdateIntellisenseItems(_completionWindow, Editor, str, targetDataType);
                        }

                        _completionWindow.CompletionList.SelectItem(code.Item1 + code.Item2);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            finally
            {
                _document.NeedParse = true;
            }
        }
        
        private void CheckUpdate()
        {
            if (OnlineEditHelper.CompilingPrograms.Contains(_stRoutine.ParentCollection.ParentProgram))
            {
                return;
            }
            if(_stRoutine.IsCompiling)return;
            if (_document.NeedParse)
            {
                _document.UpdateLexer(false);
            }
        }

        private void TextEditor_ContextMenuOpening(object sender, System.Windows.Controls.ContextMenuEventArgs e)
        {
            try
            {
                Editor.ContextMenu = null;
                CheckUpdate();
                bool isOtherProgram = false;
                Point caret = _stxEditorViewModel.AdjustCaret(e.CursorLeft, e.CursorTop);

                var pos = Editor.Document.GetLocation(Editor.CaretOffset);
                var parentProgram = _stRoutine.ParentCollection.ParentProgram;
                var info = GetSnippetByPosition.GetVariableInfo(Editor, pos);
                var tmpName = info?.CodeText;
                if (info?.IsRoutine ?? false)
                {
                    Editor.ContextMenu = _menuSelector.SetRoutineContextMenu(info.CodeText, _stRoutine,caret,_stxEditorViewModel.BrowseRoutinesAdorner,info);
                    return;
                }

                if (info != null && info.CodeText.IndexOf("\\") == 0)
                {
                    isOtherProgram = true;
                    var programName = info.CodeText.IndexOf(".") > 0
                        ? info.CodeText.Substring(1, info.CodeText.IndexOf(".") - 1)
                        : info.CodeText.Substring(1);
                    var program = _controller.Programs[programName];
                    info.CodeText = info.CodeText.IndexOf(".") > 0
                        ? info.CodeText.Substring(programName.Length + 2)
                        : null;
                    if (program != null)
                        parentProgram = program;
                    if (info.CodeText == null)
                    {
                        Editor.ContextMenu = _menuSelector.SetProgramContextMenu(parentProgram, caret, info, Editor,
                            _stRoutine.ParentCollection.ParentProgram as IProgram, _options);
                        return;
                    }
                }

                if (Editor.SelectionLength > 0)
                {
                    Editor.ContextMenu = _menuSelector.SetSelectedContextMenu(_controller, info, caret,
                        _stxEditorViewModel.BrowseAdorner, _options, _stRoutine.ParentCollection.ParentProgram);
                }
                else if (info != null && !string.IsNullOrEmpty(info.CodeText))
                {
                    int index = info.CodeText.IndexOf(".");
                    string tagName = info.CodeText;
                    if (index > 0)
                    {
                        tagName = tagName.Substring(0, index);
                    }

                    index = tagName.IndexOf("[");
                    if (index > 0)
                    {
                        tagName = tagName.Substring(0, index);
                        Regex regex = new Regex(@"(?<=\w)(\[.+?\])");
                        var matches = regex.Matches(info.CodeText);
                        Regex regex2 = new Regex(@"\[( )*(\d)+( )*(,( )*(\d)+( )*){0,2}\]");
                        foreach (Match match in matches)
                        {
                            if (!regex2.IsMatch(match.Value))
                            {
                                info.CodeText = tagName;
                                break;
                            }
                        }
                    }
                    if(_document.SnippetLexer.IsKeyword(info.CodeText))return;
                    Type type;
                    ITag tag = null;
                    if (info.GetVariableInfos().Count == 1 && info.GetVariableInfos()[0].IsEnum)
                    {
                        type = Type.Enum;
                        info.Enums = info.GetVariableInfos()[0].Enums;
                    }
                    else
                    {
                        type = _document.SnippetLexer.GetType(info,parentProgram);
                    }

                    if (type == Type.Enum) info.Enums = info.Enums;
                    if (type == Type.None)
                    {
                        string name = "";
                        if (GetSnippetByPosition.IsInstr(info.CodeText, ref name))
                        {
                            type = Type.Instr;
                        }
                    }

                    if (isOtherProgram)
                    {
                        info.CodeText = "\\" + parentProgram?.Name + "." + tagName;
                        info.CodeText = tmpName;
                    }

                    if (type != Type.None && type != Type.Instr)
                    {
                        var offset = Editor.Document.GetOffset(pos);
                        var variableInfos = Editor.TextArea.TextView.VariableInfos;
                        var variableInfo =
                            variableInfos?.LastOrDefault(v =>
                                    v.Offset <= offset && offset <= v.Offset + v.Name.Length) as
                                VariableInfo;
                        if (variableInfo != null)
                        {
                            tag = variableInfo.OriginalTag ?? variableInfo.Tag;
                        }
                        else
                        {
                            tag = _stxEditorViewModel.FindTag(info.CodeText, isOtherProgram ? parentProgram : null);
                        }
                    }

                    //Debug.Assert(tag!=null);
                    if (type == Type.Num || type == Type.Keyword)
                    {
                        Editor.ContextMenu =
                            _menuSelector.SetEmptyContextMenu(_stRoutine, caret,
                                _stxEditorViewModel.BrowseAdorner, _options);
                        return;
                    }

                    Editor.ContextMenu = type == Type.None
                        ? _menuSelector.SetCreateNewTagContextMenu(info, _stRoutine,
                            caret, _stxEditorViewModel.BrowseAdorner, _options)
                        : _menuSelector.SetTagContextMenu(_document,tag, type, info, parentProgram, caret,
                            _stxEditorViewModel.BrowseAdorner, isOtherProgram,
                            _stxEditorViewModel.BrowseEnumAdorner,
                            _document.SnippetLexer.TransformTable, _stRoutine);
                }
                else
                {
                    var infoE = new SnippetInfo(Editor.Text) {Offset = Editor.CaretOffset};
                    Editor.ContextMenu = _menuSelector
                        .SetSTContextMenu(_stRoutine, infoE, caret, _stxEditorViewModel.BrowseAdorner,
                            (StxEditorOptions) _options, _document);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            if (Editor.ContextMenu != null)
                Editor.ContextMenu.IsOpen = true;
        }

        private void TextArea_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!_options.CanEditorInput) return;
            if (_toolTip != null && _toolTip.IsOpen) _toolTip.IsOpen = false;
            bool isTabKey = e.Key == Key.Tab;
            if (isTabKey)
            {
                _completionWindow?.Close();
                int length = 0;
                var code = GetSnippetByPosition.GetFontCode(Editor,ref length);
                if (!string.IsNullOrEmpty(code))
                {
                    if (code.Equals("if", StringComparison.OrdinalIgnoreCase))
                    {
                        string codeSnippet = "IF bool_expression THEN \n\t\nEND_IF; ";
                        _document.InTabInserting = true;
                        TabInsertSnippet(codeSnippet, "bool_expression",length);
                        e.Handled = true;
                    }
                    else if (code.Equals("case", StringComparison.OrdinalIgnoreCase))
                    {
                        string codeSnippet = "CASE numeric_expression OF \n\tselector:\n\tELSE\nEND_CASE; ";
                        _document.InTabInserting = true;
                        TabInsertSnippet(codeSnippet, "numeric_expression",length);
                        e.Handled = true;
                    }
                    else if (code.Equals("elsif", StringComparison.OrdinalIgnoreCase))
                    {
                        string codeSnippet = "ELSIF bool_expression THEN";
                        TabInsertSnippet(codeSnippet, "bool_expression",5);
                        e.Handled = true;
                    }
                    else if (code.Equals("for", StringComparison.OrdinalIgnoreCase))
                    {
                        string codeSnippet = "FOR count := initial_value TO final_value DO\n\t\nEND_FOR; ";
                        _document.InTabInserting = true;
                        TabInsertSnippet(codeSnippet, "count",3);
                        e.Handled = true;
                    }
                    else if (code.Equals("region", StringComparison.OrdinalIgnoreCase))
                    {
                        string codeSnippet = "REGION MyRegion\n\t\n#ENDREGION";
                        _document.InTabInserting = true;
                        TabInsertSnippet(codeSnippet, "MyRegion",6);
                        e.Handled = true;
                    }
                    else if (code.Equals("repeat", StringComparison.OrdinalIgnoreCase))
                    {
                        string codeSnippet = "REPEAT\n\t\nUNTIL bool_expression\nEND_REPEAT;";
                        TabInsertSnippet(codeSnippet, "bool_expression",6);
                        e.Handled = true;
                    }
                    else if (code.Equals("while", StringComparison.OrdinalIgnoreCase))
                    {
                        string codeSnippet = "WHILE bool_expression DO\n\t\nEND_WHILE;";
                        TabInsertSnippet(codeSnippet, "bool_expression",5);
                        e.Handled = true;
                    }

                }

            }
        }

        private void TabInsertSnippet(string codeSnippet, string selectionSnippet,int start)
        {
            if (!_options.CanEditorInput) return;
            try
            {
                var caret = Editor.TextArea.Caret ;
                Editor.Document.Replace(caret.Offset-start,start,"");
                int offset = caret.Offset;
                int line = caret.Line;
                int col = caret.Column;
                offset = offset + 1;
                int selectionSnippetOffset = codeSnippet.IndexOf(selectionSnippet);
                SnippetTextElement snippetText = new SnippetTextElement() {Text = codeSnippet};
                Snippet snippet = new Snippet() {Elements = {snippetText}};
                snippet.Insert(Editor.TextArea);
                TextViewPosition textViewPosition1, textViewPosition2;
                if (codeSnippet.IndexOf("repeat") > 0)
                {
                    textViewPosition1 = new TextViewPosition(line + 2, 7);
                    textViewPosition2 = new TextViewPosition(line + 2, 7 + selectionSnippet.Length);
                }
                else
                {
                    textViewPosition1 = new TextViewPosition(line, col + selectionSnippetOffset);
                    textViewPosition2 =
                        new TextViewPosition(line, col + selectionSnippet.Length + selectionSnippetOffset);
                }

                Selection selection = new RectangleSelection(Editor.TextArea, textViewPosition1, textViewPosition2);

                Editor.TextArea.Selection = selection;
                caret.Offset = offset + selectionSnippet.Length + selectionSnippetOffset - 1;
                _document.UpdateLexer();
            }
            catch (Exception)
            {
                //do noting
            }
        }

        private void TextMarkerService_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems.Count > 0)
                {
                    var marker = (TextMarkerService.TextMarker) e.NewItems[0];
                    _textMarkerService.Redraw(Editor, marker);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems.Count > 0)
                {
                    var marker = (TextMarkerService.TextMarker) e.OldItems[0];
                    _textMarkerService.Redraw(Editor, marker);
                }
            }
        }

        private void ToolTipClosed(object sender, RoutedEventArgs e)
        {
            _toolTip.Closed -= ToolTipClosed;
            _toolTip = null;
        }

        private void TextEditorMouseHoverStopped(object sender, MouseEventArgs e)
        {
            if (_toolTip != null)
            {
                _toolTip.IsOpen = false;
                e.Handled = true;
            }
        }

        private void UpdateFoldings()
        {
            if (_foldingManager == null || Editor.Document == null) return;
            _foldingStrategy?.UpdateFoldings(_foldingManager, Editor.Document);
        }

        private bool IsInCommentOrRegion(int offset)
        {
            if (GetSnippetByPosition.IsInRegionComment(Editor.Text, offset)) return true;
            if (GetSnippetByPosition.IsInComment(Editor.Text, offset)) return true;
            return false;
        }

        private bool IsInErrorMark(int offset)
        {
            var markers = _textMarkerService.GetMarkersAtOffset(offset);
            return markers.Any();
        }

       
        private void TextAreaOnTextEntered(object sender, TextCompositionEventArgs e)
        {
            if (!_options.CanEditorInput) return;
            if (IsInCommentOrRegion(Editor.CaretOffset - 1)) return;
            if(Regex.IsMatch(e.Text, @"[\u4e00-\u9fa5]"))return;
            if ((_completionWindow == null ||!_completionWindow.IsActive)&&
                (e.Text == "." || char.IsLetter(e.Text[0])||char.IsNumber(e.Text[0]) || e.Text == "\\" ||
                 e.Text == "_"||e.Text=="%"||e.Text=="#"))
            {
                _completionWindow = new CompletionWindow(Editor.TextArea);
                _completionWindow.TrackToolTip = _trackToolTip;
                _completionWindow.CloseWhenCaretAtBeginning = true;
                WeakEventManager<CompletionWindow, KeyEventArgs>.AddHandler(_completionWindow, "PreviewKeyDown",
                    TextArea_PreviewKeyDown);
                var data = _completionWindow.CompletionList.CompletionData;
                List<string> enums = new List<string>();
                var targetDataType = GetSortDataType(ref enums);
                int start = 0, end = 0;
                var code = CompletionWindow.GetCode(Editor.TextArea,false, ref start, ref end);
                var str = code.Item1 + code.Item2;
                if (str.Length >= 1)
                {
                    if (char.IsNumber(str[0]))
                    {
                        _completionWindow = null;
                        return;
                    }
                }
                if (enums?.Count > 0)
                {
                    foreach (var @enum in enums)
                    {
                        var s = new StxCompletionItemKeywordData(@enum);
                        var m = new StxCompletionItem(s);
                        data.Add(m);
                    }
                }
                else
                {
                    _stxEditorViewModel.UpdateIntellisenseItems(_completionWindow, Editor, e.Text, targetDataType);
                }

                if (char.IsLetter(e.Text[0]) || e.Text == "_" || char.IsNumber(e.Text[0]) || e.Text == "%")
                {
                    if (enums?.Count > 0)
                    {
                        _completionWindow.StartOffset = start;
                        _completionWindow.EndOffset = end;
                    }

                    _completionWindow.CompletionList.SelectItem(code.Item1 + code.Item2);
                }


                if (data.Count > 0&&!_completionWindow.IsVisible)
                    _completionWindow.Show();
                WeakEventManager<CompletionWindow, EventArgs>.AddHandler(_completionWindow, "Closed", delegate
                {
                    WeakEventManager<CompletionWindow, KeyEventArgs>.RemoveHandler(_completionWindow, "PreviewKeyDown",
                        TextArea_PreviewKeyDown);

                    _completionWindow = null;
                });
            }

            if (e.Text == "," || e.Text == "(")
            {
                TrackInstrParametersEnter();
            }

            if ("f".Equals(e.Text, StringComparison.OrdinalIgnoreCase) ||
                "r".Equals(e.Text, StringComparison.OrdinalIgnoreCase) ||
                "e".Equals(e.Text, StringComparison.OrdinalIgnoreCase) ||
                "t".Equals(e.Text, StringComparison.OrdinalIgnoreCase) ||
                "l".Equals(e.Text, StringComparison.OrdinalIgnoreCase))
                if (EndKeywordAutoIndent()) _completionWindow.Close();
        }
        
        private bool EndKeywordAutoIndent()
        {
            var currentLine = TextEditor.Document.GetLineByNumber(TextEditor.TextArea.Caret.Line);
            var lineContent = TextEditor.Document.GetText(currentLine.Offset, currentLine.Length).Trim();
            if (string.IsNullOrEmpty(lineContent)) return false;
            var keywords = new[]
            {
                "end_if", "end_for", "else", "elsif", "end_case",
                "until", "end_while", "end_repeat"
            };

            bool flag = false;
            for (int i = 0; i < keywords.Length; i++)
            {
                if (keywords[i].Equals(lineContent, StringComparison.OrdinalIgnoreCase))
                {
                    flag = true;
                    break;
                }
            }
            if (!flag) return false;

            var text = TextEditor.TextArea.Document.GetText(0, currentLine.Offset);

            var match = Indentation.GetKeywordScopeLocation(text,lineContent);
            if (match == null) return false;

            var line = TextEditor.TextArea.Document.GetLineByOffset(match.Index);
            var content = TextEditor.TextArea.Document.GetText(line.Offset, match.Index - line.Offset);

            var lineBuilder = new StringBuilder(lineContent);
            for (var k = match.Index - line.Offset - 1; k >= 0; k--)
                if (char.IsWhiteSpace(content[k]))
                    lineBuilder.Insert(0, content[k]);
                else
                    lineBuilder.Insert(0, " ");
            TextEditor.Document.Replace(currentLine.Offset, currentLine.Length, lineBuilder.ToString());
            return true;
        }

        private readonly TrackTooltip _trackToolTip = new TrackTooltip();
        private void TrackInstrParametersEnter()
        {
            if (!TextEditor.TextArea.Caret.IsVisible) return;
            if (TextEditor.TextArea.Caret.ChangeType==0)
            {
                _trackToolTip.IsOpen = false;
                return;
            }

            if (TextEditor.TextArea.Caret.ChangeType == 1&&!_trackToolTip.IsOpen)
            {
                return;
            }

            var offset = Editor.CaretOffset;
            var instr = "";
            var result=GetSnippetByPosition.TryGetInstr(Editor.Text, ref offset,ref instr);
            if (!result) return;
            IXInstruction instruction = null;
            if (TryGetInstr(instr, ref instruction))
            {
                var info = new SnippetInfo(Editor.Text);
                info.CodeText = instruction.Name;
                info.Offset = offset;
                var location=TextEditor.TextArea.Document.GetLocation(offset+info.CodeText.Length);
                int count = 0;
                var parameters = MenuSelector.GetInstrParametersWithoutVariables(info, ref count, Editor.CaretOffset, _stRoutine, true);
                if (parameters.Count > 0)
                {
                    StxCompletionItemData instrCodeSnippetData =
                        _document.SnippetLexer.StxCompletionItemCodeSnippetDatas.FirstOrDefault(s =>
                            s.Name.Equals(instruction.Name, StringComparison.OrdinalIgnoreCase)) ??
                        (StxCompletionItemData) _stxEditorViewModel.GetAoiData(instruction.Name);
                    if (instrCodeSnippetData != null)
                    {
                        var completionItemTrack = new StxCompletionItemTrack(instruction, instrCodeSnippetData, parameters.Count - 1);

                        _trackToolTip.ResetPosition(TextEditor.TextArea,location);
                        _trackToolTip.PlacementTarget = TextEditor.TextArea.TextView;
                        _trackToolTip.Placement = PlacementMode.RelativePoint;
                        _trackToolTip.Content = ((IFancyCompletionItemData)completionItemTrack).Description;
                        _trackToolTip.IsOpen = true;
                        _trackToolTip.StaysOpen = true;
                        _trackToolTip.MaxWidth = StxCompletionItemData.TooltipWidth;
                    }
                }
            }
            else
            {
                if (_trackToolTip != null)
                {
                    _trackToolTip.IsOpen = false;
                    _trackToolTip.StaysOpen = false;
                }
            }
        }

        private void _trackToolTip_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            if (_toolTip != null)
            {
                if (_toolTip.IsOpen)
                    _toolTip.IsOpen = false;
            }
        }

        private bool TryGetInstr(string code, ref IXInstruction instr)
        {
            var index = code.IndexOf("(");
            if (index > -1)
            {
                var str = code.Substring(0, index);
                instr = ((Controller)_stRoutine.ParentController).STInstructionCollection.FindInstruction(str.Trim());
                return instr != null;
            }
            return false;
        }
        
        private void TextAreaOnTextEntering(object sender, TextCompositionEventArgs e)
        {
            if (!_options.CanEditorInput) return;
            if (e.Text.Length > 0 && _completionWindow != null)
                if (!(char.IsLetter(e.Text[0]) || char.IsNumber(e.Text[0]) || e.Text[0] == '_'))
                    //_completionWindow.CompletionList.RequestInsertion(e);
                    _completionWindow?.Close();
            // do not set e.Handled=true - we still want to insert the character that was typed
        }

        private IDataType GetSortDataType(ref List<string> enums)
        {
            var regex = new Regex(@"(\b[A-Za-z][A-Za-z0-9_]*)(?=[ \r\n\t]*\()");
            foreach (Match match in regex.Matches(Editor.Text))
            {
                int index = 0;
                if (IsTargetCode(match.Index + match.Length, GetInstrParamStart(Editor.CaretOffset - 1, ref index)))
                {
                    var instr = (_controller as Controller).STInstructionCollection.FindInstruction(match.Value);
                    var interrelated = "";
                    if ((match.Value.Equals("gsv", StringComparison.OrdinalIgnoreCase) ||
                         match.Value.Equals("ssv", StringComparison.OrdinalIgnoreCase)) && index == 2)
                    {
                        bool flag = false;
                        for (int i = match.Index + match.Length; i < Editor.Text.Length; i++)
                        {
                            if (Editor.Text[i] == '(') flag = true;
                            if (Editor.Text[i] == ',') break;
                            if (flag)
                            {
                                if (char.IsLetter(Editor.Text[i])) interrelated += Editor.Text[i];
                            }
                        }
                    }

                    enums = SnippetLexer.GetInstrEnum(match.Value, index, interrelated);
                    if (enums?.Count > 0) return null;
                    if (index==1&&(match.Value.Equals("gsv", StringComparison.OrdinalIgnoreCase) ||
                        match.Value.Equals("ssv", StringComparison.OrdinalIgnoreCase)))
                    {
                        return GetSsvOrGsvInstanceDataType(Editor.Text, match.Index);
                    }
                    return (instr as FixedInstruction)?.GetParamDataType(index);
                }
            }
            return null;
        }

        private IDataType GetSsvOrGsvInstanceDataType(string code, int offset)
        {
            offset += 4;
            var @class = "";
            while (offset < code.Length)
            {
                var c = code[offset];
                if (char.IsWhiteSpace(c))
                {
                    if (string.IsNullOrEmpty(@class))
                    {
                       
                    }
                    else
                    {
                        break;
                    }

                }
                else if (char.IsLetter(c))
                {
                    @class += c;
                }
                else
                {
                    break;
                }

                offset++;
            }

            if ("program".Equals(@class, StringComparison.OrdinalIgnoreCase))
            {
                return new ProgramDataType();
            }

            if ("routine".Equals(@class, StringComparison.OrdinalIgnoreCase))
            {
                return new RoutineDataType();
            }

            if ("module".Equals(@class, StringComparison.OrdinalIgnoreCase))
            {
                return new ModuleDataType();
            }

            if ("task".Equals(@class, StringComparison.OrdinalIgnoreCase))
            {
                return new TaskDataType();
            }
            return DINT.Inst;
        }

        internal class ProgramDataType : DataType
        {

        }
        internal class RoutineDataType : DataType
        {

        }

        internal class ModuleDataType:DataType
        {
            
        }

        internal class TaskDataType : DataType
        {

        }
        private int GetInstrParamStart(int offset, ref int index)
        {
            var rp = 0;
            var text = Editor.Text;
            for (int i = offset; i >= 0; i--)
            {
                if (text[i] == '(')
                {
                    if (rp > 0)
                    {
                        rp--;
                        continue;
                    }

                    return i;
                }

                if (text[i] == ')')
                {
                    rp++;
                    continue;
                }

                if (text[i] == ',' && rp == 0) index++;
            }

            return -1;
        }

        private bool IsTargetCode(int offset, int targetOffset)
        {
            if (offset == -1) return false;
            for (int i = offset; i < Editor.Text.Length; i++)
            {
                if (Editor.Text[i] == '\n' || Editor.Text[i] == '\r' || Editor.Text[i] == '\t' ||
                    Editor.Text[i] == ' ') continue;
                if (Editor.Text[i] == '(' && i == targetOffset) return true;
                return false;
            }

            return false;
        }

        private void Editor_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var ctrl = Keyboard.Modifiers == ModifierKeys.Control;
            if (ctrl)
            {
                _stxEditorViewModel?.UpdateFontSize(e.Delta > 0);
                e.Handled = true;
            }
        }

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            if (_foldingManager == null) _foldingManager = FoldingManager.Install(Editor.TextArea);
            UpdateFoldings();
        }

        private void Editor_MouseHover(object sender, MouseEventArgs e)
        {
            // from SharpDevelop:TextMarkerToolTipProvider.cs
            try
            {
                var pos = Editor.GetPositionFromPoint(e.GetPosition(Editor));
                if (pos != null)
                {
                    var offset = Editor.Document.GetOffset(pos.Value.Line, pos.Value.Column);
                    if (IsInCommentOrRegion(offset)) return;
                    var foldingsAtOffset = _foldingManager.GetFoldingsAt(offset);

                    FoldingSection collapsedSection = foldingsAtOffset.FirstOrDefault(section => section.IsFolded);
                    if (collapsedSection != null)
                    {
                        _toolTip.PlacementTarget = this;
                        _toolTip.Closed += ToolTipClosed;
                        _toolTip.ToolTipOpening += _toolTip_ToolTipOpening;
                        _toolTip.Content = collapsedSection.TextContent;
                        _toolTip.IsOpen = true;
                        e.Handled = true;
                        return;
                    }
                }

                bool inDocument = pos.HasValue;
                if (inDocument)
                {
                    CheckUpdate();
                    TextLocation logicalPosition = pos.Value.Location;
                    string info = "";
                    int offset = Editor.Document.GetOffset(logicalPosition);
                    //if (IsInErrorMark(offset)) return;
                    if (IsInCommentOrRegion(offset)) return;
                    var coverStr = GetSnippetByPosition.GetVariableInfo(Editor, logicalPosition);
                    if (GetSnippetByPosition.IsMathStatusFlag(coverStr?.CodeText))
                    {
                        e.Handled = true;
                        return;
                    }
                    if (_document.SnippetLexer.IsKeyword(coverStr?.CodeText)) return;
                    var type = _document.SnippetLexer.GetType(coverStr,_stRoutine.ParentCollection.ParentProgram);
                    if(type==Type.Enum)return;
                    if (type == Type.Routine)
                    {
                        if (_toolTip == null)
                        {
                            var routine = _stRoutine.ParentCollection[coverStr.CodeText];
                            var content = "";
                            if (routine == null) content = $"Undefined routine:{coverStr.CodeText}";
                            else if (routine is STRoutine) content = $"ST Routine:{routine.Name}";
                            else if (routine is RLLRoutine) content = $"RLL Routine:{routine.Name}";
                            else if (routine is SFCRoutine) content = $"SFC Routine:{routine.Name}";
                            else if (routine is FBDRoutine) content = $"FBD Routine:{routine.Name}";
                            _toolTip = new ToolTip();
                            _toolTip.ToolTipOpening += _toolTip_ToolTipOpening;
                            _toolTip.Closed += ToolTipClosed;
                            _toolTip.PlacementTarget = this;
                            _toolTip.Content = content;
                            _toolTip.IsOpen = true;
                            e.Handled = true;
                        }

                        return;
                    }

                    if (type == Type.Program || type == Type.Module || type == Type.Task)
                    {
                        if (_toolTip == null)
                        {
                            var content = $"{type.ToString()}:{coverStr?.CodeText}";
                            _toolTip = new ToolTip();
                            _toolTip.Closed += ToolTipClosed;
                            _toolTip.PlacementTarget = this;
                            _toolTip.Content = content;
                            _toolTip.IsOpen = true;
                            e.Handled = true;
                        }

                        return;
                    }

                    if (type == Type.InoutParameter)
                    {
                        if (_toolTip == null)
                        {
                            _toolTip = new ToolTip();
                            _toolTip.ToolTipOpening += _toolTip_ToolTipOpening;
                            _toolTip.Closed += ToolTipClosed;
                            _toolTip.PlacementTarget = this;
                            _toolTip.Content =
                                $"Tag:{coverStr.CodeText}\nUsage:{ExtendUsage.ToString(Usage.InOut)}\nData Type:{coverStr.DataType}\nScope:{_stRoutine.ParentCollection.ParentProgram.Name}";
                            _toolTip.IsOpen = true;
                            e.Handled = true;
                        }

                        return;
                    }

                    if (type == Type.Enum)
                    {
                        if (_toolTip == null)
                        {
                            _toolTip = new ToolTip();
                            _toolTip.ToolTipOpening += _toolTip_ToolTipOpening;
                            _toolTip.Closed += ToolTipClosed;
                            _toolTip.PlacementTarget = this;
                            _toolTip.Content = coverStr.CodeText;
                            _toolTip.IsOpen = true;
                            e.Handled = true;
                        }

                        return;
                    }

                    if (type == Type.Num)
                    {
                        string valueStr = coverStr.CodeText;
                        if (_toolTip == null)
                        {
                            _toolTip = new ToolTip();
                            _toolTip.ToolTipOpening += _toolTip_ToolTipOpening;
                            _toolTip.Closed += ToolTipClosed;
                            _toolTip.PlacementTarget = this;
                            _toolTip.Content = new TextBlock
                            {
                                Text = $"Value:{valueStr}",
                                TextWrapping = TextWrapping.Wrap
                            };
                            _toolTip.IsOpen = true;
                            e.Handled = true;
                        }

                        return;
                    }

                    if (coverStr == null) return;
                    if (string.IsNullOrEmpty(coverStr.CodeText) ||
                        GetSnippetByPosition.Keyword.Contains(coverStr.CodeText, StringComparer.OrdinalIgnoreCase))
                        return;

                    if (type == Type.Instr)
                    {
                        var snippetData = _document.SnippetLexer.StxCompletionItemCodeSnippetDatas.Find(x =>
                            x.Name.Equals(Regex.Replace(coverStr.CodeText, "end_", "", RegexOptions.IgnoreCase),
                                StringComparison.OrdinalIgnoreCase));
                        if (snippetData == null)
                        {
                            var aoiData = _stxEditorViewModel.GetAoiData(coverStr.CodeText);
                            if (aoiData != null)
                            {
                                if (_toolTip == null)
                                {
                                    _toolTip = new ToolTip();
                                    _toolTip.ToolTipOpening += _toolTip_ToolTipOpening;
                                    _toolTip.Closed += ToolTipClosed;
                                    _toolTip.PlacementTarget = this;
                                    _toolTip.Content = ((IFancyCompletionItemData) aoiData).Description;
                                    _toolTip.IsOpen = true;
                                    e.Handled = true;
                                }

                                return;
                            }
                        }
                        else
                        {
                            if (!snippetData.IsInstructionOrFunction) return;
                            if (_toolTip == null)
                            {
                                _toolTip = new ToolTip();
                                _toolTip.ToolTipOpening += _toolTip_ToolTipOpening;
                                _toolTip.Closed += ToolTipClosed;
                                _toolTip.PlacementTarget = this;
                                _toolTip.Content = ((IFancyCompletionItemData) snippetData).Description;
                                _toolTip.IsOpen = true;
                                e.Handled = true;
                            }

                            return;
                        }
                    }


                    if (type == Type.Num)
                    {
                        string valueStr = coverStr.CodeText;
                        if (_toolTip == null)
                        {
                            _toolTip = new ToolTip();
                            _toolTip.ToolTipOpening += _toolTip_ToolTipOpening;
                            _toolTip.Closed += ToolTipClosed;
                            _toolTip.PlacementTarget = this;
                            _toolTip.Content = new TextBlock
                            {
                                Text = $"Value:{valueStr}",
                                TextWrapping = TextWrapping.Wrap
                            };
                            _toolTip.IsOpen = true;
                            e.Handled = true;
                        }

                        return;

                    }

                    info = _componentContentProvider.GetCoverStrInfo(coverStr.CodeText) ??
                           $"Undefined Tag:{coverStr.CodeText}";

                    if (_toolTip == null)
                    {
                        _toolTip = new ToolTip();
                        _toolTip.ToolTipOpening += _toolTip_ToolTipOpening;
                        _toolTip.Closed += ToolTipClosed;
                        _toolTip.PlacementTarget = this;
                        _toolTip.Content = new TextBlock
                        {
                            Text = info,
                            TextWrapping = TextWrapping.Wrap
                        };
                        _toolTip.IsOpen = true;
                        e.Handled = true;
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.StackTrace);
            }
        }

        private void _toolTip_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            if (_trackToolTip != null)
            {
                if (_trackToolTip.IsOpen)
                    _trackToolTip.IsOpen = false;
            }
        }

        private void Editor_MouseHoverStopped(object sender, MouseEventArgs e)
        {
            if (_toolTip != null)
                _toolTip.IsOpen = false;
        }
    }
}