using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using ICSStudio.AvalonEdit.Document;
using ICSStudio.AvalonEdit.Rendering;
using ICSStudio.AvalonEdit.Variable;
using ICSStudio.Dialogs.GlobalSetting;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.StxEditor.Menu;
using ICSStudio.StxEditor.ViewModel.CodeSnippets;
using Type = System.Type;

namespace ICSStudio.StxEditor.ViewModel
{
    internal partial class StxEditorViewModel
    {
        #region Shortcut

        private SnippetInfo _dialogInfo;
        private ITag _dialogTag;
        private ExpectedMenuType _expectedMenuType = ExpectedMenuType.None;
        private IProgram _dialogProgram = null;

        private enum ExpectedMenuType
        {
            None,
            Program,
            Selected,
            Empty,
            Instr,
            Name,
            Enum,
            Num,
            Tag,
            InoutParameter,
            Keyword,
            Routine,
            Module,
            Task,
            ST
        }

        public RelayCommand PropertiesCommand { get; }

        private bool CanExecutePropertiesCommand()
        {
            ParseMenuType();
            return _expectedMenuType == ExpectedMenuType.ST || _expectedMenuType == ExpectedMenuType.Program;
        }

        private void ExecutePropertiesCommand()
        {
            if (_expectedMenuType == ExpectedMenuType.ST)
                //routine properties
                PropertiesMenuItem.Properties(Routine);
            if (_expectedMenuType == ExpectedMenuType.Program)
                //program properties
                EditProgramPropertiesMenuItem.EditProgram(_dialogProgram);


        }

        public RelayCommand BrowseEnumCommand { get; }

        private bool CanExecuteBrowseEnumCommand()
        {
            ParseMenuType();
            return _expectedMenuType == ExpectedMenuType.Program || _expectedMenuType == ExpectedMenuType.Enum;
        }

        private void ExecuteBrowseEnumCommand()
        {
            Point caret =
                Editor.TextArea.TextView.GetVisualPosition(Editor.TextArea.Caret.Position, VisualYPosition.Baseline);
            caret = AdjustCaret(caret.X, caret.Y);
            if (_expectedMenuType == ExpectedMenuType.Enum)
            {
                try
                {

                    BrowseEnumAdorner.ResetAdorner(caret, Editor.FontSize / 12, _dialogInfo.CodeText,
                        _dialogInfo.Enums);
                    var layer = AdornerLayer.GetAdornerLayer(Editor);
                    layer?.Add(BrowseEnumAdorner);
                    Options.CanZoom = false;
                    Editor.GotFocus += BrowseEnumEditor_GotFocus;
                    Editor.Dispatcher.BeginInvoke((Action) delegate() { BrowseEnumAdorner.SetTextFocus(); },
                        DispatcherPriority.Loaded);
                }
                catch (Exception)
                {
                    //
                }
            }

            if (_expectedMenuType == ExpectedMenuType.Program)
            {
                //browse program
                {
                    _programBrowse = new ProgramBrowse(Editor, _dialogProgram, caret, Editor.FontSize / 12);
                    var layer = AdornerLayer.GetAdornerLayer(Editor);
                    layer?.Add(_programBrowse);
                    Editor.GotFocus += ProgramEditor_GotFocus;
                    Editor.Dispatcher.BeginInvoke((Action) delegate() { _programBrowse.SetTextFocus(); },
                        DispatcherPriority.ApplicationIdle);
                }
            }
        }

        private ProgramBrowse _programBrowse;

        private void ProgramEditor_GotFocus(object sender, RoutedEventArgs e)
        {
            var layer = AdornerLayer.GetAdornerLayer(Editor);
            layer?.Remove(_programBrowse);
            Editor.Document.Replace(_dialogInfo.Offset, _dialogProgram.Name.Length + 1,
                new StringTextSource("\\" + _programBrowse.ProgramName ?? ""));
            Editor.GotFocus -= ProgramEditor_GotFocus;
        }

        public Point AdjustCaret(double x, double y)
        {
            x = Math.Floor(x / Editor.TextArea.TextView.FontSize) * Editor.TextArea.TextView.FontSize;
            y = Math.Floor(y / (Editor.TextArea.TextView.DefaultLineHeight * Editor.TextArea.TextView.LineSpacing)) *
                (Editor.TextArea.TextView.DefaultLineHeight * Editor.TextArea.TextView.LineSpacing) +
                Editor.TextArea.TextView.DefaultLineHeight;
            return new Point(x, y);
        }

        private void BrowseEnumEditor_GotFocus(object sender, RoutedEventArgs e)
        {
            string name = BrowseEnumAdorner.GetName();
            Editor.Document.Replace(_dialogInfo.Offset, _dialogInfo.CodeText?.Length ?? 0,
                new StringTextSource(name ?? ""));
            Editor.GotFocus -= BrowseEnumEditor_GotFocus;
        }

        public RelayCommand CrossTagCommand { get; }

        private bool CanExecuteCrossTagCommand()
        {
            ParseMenuType();
            return _expectedMenuType == ExpectedMenuType.Tag || _expectedMenuType == ExpectedMenuType.Program;
        }

        private void ExecuteCrossTagCommand()
        {
            if (_expectedMenuType == ExpectedMenuType.Tag)
                CrossTagMenuItem.CrossTag(_dialogTag, _dialogInfo.OriginalCodeText);
            if (_expectedMenuType == ExpectedMenuType.Program)
                CrossProgramMenuItem.CrossProgram(_dialogProgram);
        }

        public RelayCommand EditTagPropertiesCommand { get; }

        private bool CanExecuteEditTagPropertiesCommand()
        {
            ParseMenuType();
            return _expectedMenuType == ExpectedMenuType.Tag;
        }

        private void ExecuteEditTagPropertiesCommand()
        {
            if (Routine.ParentCollection.ParentProgram is AoiDefinition)
            {
                //aoi tag edit
                EditAoiTagMenuItem.EditAoiTag(_dialogTag);
            }
            else
                EditPropertiesMenuItem.EditProperties(_dialogTag);


        }

        public RelayCommand GoToCommand { get; }

        private void ExecuteGoToCommand()
        {
            ParseMenuType();
            if (_expectedMenuType == ExpectedMenuType.Tag)
            {
                GoToMenuItem.GoTo(Editor, null, _dialogInfo.CodeText, _dialogTag,
                    Routine.ParentCollection.ParentProgram, null);
            }
            else if (_expectedMenuType == ExpectedMenuType.None)
            {
                GoToMenuItem.GoTo(Editor, null, _dialogInfo.CodeText, null, Routine.ParentCollection.ParentProgram,
                    null);
            }
            else if (_expectedMenuType == ExpectedMenuType.ST || _expectedMenuType == ExpectedMenuType.Empty)
            {
                GoToMenuItem.GoTo(Editor, Routine, null, null, Routine.ParentCollection.ParentProgram, null);
            }
            else if (_expectedMenuType == ExpectedMenuType.Program)
            {
                GoToMenuItem.GoTo(Editor, null, _dialogProgram.Name, null, Routine.ParentCollection.ParentProgram,
                    _dialogProgram);
            }

            //GoToMenuItem.GoTo(Editor, Routine,);
        }

        public RelayCommand NewTagCommand { get; }

        private readonly bool _needParseMenuType=true;

        private bool CanExecuteNewTagCommand()
        {
            ParseMenuType();
            return _expectedMenuType == ExpectedMenuType.None || _expectedMenuType == ExpectedMenuType.ST;
        }

        private void ParseMenuType()
        {
            try
            {
                if(!_needParseMenuType)return;
                if (StxEditorDocument.NeedParse)
                    StxEditorDocument.UpdateLexer(false);
                bool isOtherProgram = false;
                //Point caret = Editor.TextArea.TextView.GetVisualPosition(Editor.TextArea.Caret.Position, VisualYPosition.Baseline);

                var pos = Editor.Document.GetLocation(Editor.CaretOffset);
                var parentProgram = Routine.ParentCollection.ParentProgram;
                _dialogInfo = GetSnippetByPosition.GetVariableInfo(Editor, pos);
                var tmpName = _dialogInfo?.CodeText;
                if (_dialogInfo?.IsRoutine ?? false)
                {
                    _expectedMenuType = ExpectedMenuType.Routine;
                    return;
                }

                if (_dialogInfo != null && _dialogInfo.CodeText.IndexOf("\\") == 0)
                {
                    isOtherProgram = true;
                    var programName = _dialogInfo.CodeText.IndexOf(".") > 0
                        ? _dialogInfo.CodeText.Substring(1, _dialogInfo.CodeText.IndexOf(".") - 1)
                        : _dialogInfo.CodeText.Substring(1);
                    _dialogProgram = Controller.Programs[programName];
                    _dialogInfo.CodeText = _dialogInfo.CodeText.IndexOf(".") > 0
                        ? _dialogInfo.CodeText.Substring(programName.Length + 2)
                        : null;
                    if (_dialogProgram != null)
                        parentProgram = _dialogProgram;
                    if (_dialogInfo.CodeText == null)
                    {
                        //Editor.ContextMenu = _menuSelector.SetProgramContextMenu(parentProgram, caret, _dialogInfo, Editor,
                        //    _stRoutine.ParentCollection.ParentProgram as IProgram, _options);
                        _expectedMenuType = ExpectedMenuType.Program;
                        return;
                    }
                }

                if (Editor.SelectionLength > 0)
                {
                    //Editor.ContextMenu = _menuSelector.SetSelectedContextMenu(_controller, _dialogInfo, caret,
                    //    _stxEditorViewModel.BrowseAdorner, _options);
                    _expectedMenuType = ExpectedMenuType.Selected;
                    return;
                }
                else if (_dialogInfo != null && !string.IsNullOrEmpty(_dialogInfo.CodeText))
                {
                    int index = _dialogInfo.CodeText.IndexOf(".");
                    string tagName = _dialogInfo.CodeText;
                    if (index > 0)
                    {
                        tagName = tagName.Substring(0, index);
                    }

                    index = tagName.IndexOf("[");
                    if (index > 0)
                    {
                        tagName = tagName.Substring(0, index);
                        Regex regex = new Regex(@"(?<=\w)(\[.+?\])");
                        var matches = regex.Matches(_dialogInfo.CodeText);
                        Regex regex2 = new Regex(@"\[( )*\d( )*(,( )*\d( )*){0,2}\]");
                        foreach (Match match in matches)
                        {
                            if (!regex2.IsMatch(match.Value))
                            {
                                _dialogInfo.CodeText = tagName;
                                break;
                            }
                        }
                    }
                    if (StxEditorDocument.SnippetLexer.IsKeyword(_dialogInfo.CodeText)) return;
                    CodeSnippets.Type type;
                    if (_dialogInfo.GetVariableInfos().Count == 1 && _dialogInfo.GetVariableInfos()[0].IsEnum)
                    {
                        type = CodeSnippets.Type.Enum;
                        _expectedMenuType = ExpectedMenuType.Enum;
                        _dialogInfo.Enums = _dialogInfo.GetVariableInfos()[0].Enums;
                    }
                    else
                    {
                        type = StxEditorDocument.SnippetLexer.GetType(_dialogInfo,parentProgram);
                    }

                    if (type == CodeSnippets.Type.Enum)
                    {
                        _dialogInfo.Enums = _dialogInfo.Enums;
                        return;
                    }
                    if (type == CodeSnippets.Type.None)
                    {
                        string name = "";
                        if (GetSnippetByPosition.IsInstr(_dialogInfo.CodeText, ref name))
                        {
                            _expectedMenuType = ExpectedMenuType.Instr;
                            type = CodeSnippets.Type.Instr;
                        }
                    }

                    if (isOtherProgram)
                    {
                        _dialogInfo.CodeText = "\\" + parentProgram?.Name + "." + tagName;
                        _dialogInfo.CodeText = tmpName;
                    }

                    _dialogTag = null;
                    if (type != CodeSnippets.Type.None && type != CodeSnippets.Type.Instr)
                    {
                        var offset = Editor.Document.GetOffset(pos);
                        var variableInfos = Editor.TextArea.TextView.VariableInfos;
                        var variableInfo =
                            variableInfos?.LastOrDefault(v =>
                                    v.Offset <= offset && offset <= v.Offset + v.Name.Length) as
                                VariableInfo;
                        _dialogTag = variableInfo?.Tag ??
                                     (isOtherProgram
                                         ? FindTag(_dialogInfo.CodeText, parentProgram)
                                         : FindTag(_dialogInfo.CodeText));
                    }

                    //Debug.Assert(tag!=null);
                    if (type == CodeSnippets.Type.Num || type == CodeSnippets.Type.Keyword)
                    {
                        //Editor.ContextMenu =
                        //    _menuSelector.SetEmptyContextMenu(_stRoutine, caret,
                        //        _stxEditorViewModel.BrowseAdorner, _options);
                        _expectedMenuType = ExpectedMenuType.Empty;
                        return;
                    }

                    if (type == CodeSnippets.Type.Tag)
                    {
                        _expectedMenuType = ExpectedMenuType.Tag;
                    }
                    else
                    {
                        _expectedMenuType = ExpectedMenuType.None;
                    }

                    return;
                    //Editor.ContextMenu = type == Type.None
                    //    ? _menuSelector.SetCreateNewTagContextMenu(_dialogInfo, _stRoutine,
                    //        caret, _stxEditorViewModel.BrowseAdorner, _options)
                    //    : _menuSelector.SetTagContextMenu(tag, type, _dialogInfo, parentProgram, caret,
                    //        _stxEditorViewModel.BrowseAdorner, _options, isOtherProgram, _stxEditorViewModel.BrowseEnumAdorner,
                    //        _document.SnippetLexer.TransformTable);
                }
                else
                {
                    _dialogInfo = new SnippetInfo(Editor.Text) {Offset = Editor.CaretOffset};
                    //Editor.ContextMenu = _menuSelector
                    //    .SetSTContextMenu(_stRoutine, infoE, caret, _stxEditorViewModel.BrowseAdorner, _options);
                    _expectedMenuType = ExpectedMenuType.ST;
                    return;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        internal ITag FindTag(string name, IProgramModule otherProgram = null)
        {
            name = name.Replace(" ", "");
            if (name.IndexOf('.') > -1)
            {
                name = name.Substring(0, name.IndexOf('.'));
            }

            if (name.IndexOf('[') > -1)
            {
                name = name.Substring(0, name.IndexOf('['));
            }

            if (otherProgram != null)
            {
                name = name.Substring(otherProgram.Name.Length + 2);
                var tag = otherProgram.Tags[name];
                return tag;
            }

            foreach (var tag in Routine.ParentCollection.ParentProgram.Tags)
            {
                if (tag.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) return tag;
            }

            foreach (var tag in Routine.ParentController.Tags)
            {
                if (tag.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) return tag;
            }

            if (!(Routine.ParentCollection.ParentProgram is AoiDefinition))
                foreach (var tag in Controller.Tags)
                {
                    if (tag.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) return tag;
                }

            return null;
        }

        private void ExecuteNewTagCommand()
        {
            if (Routine.ParentCollection.ParentProgram is AoiDefinition)
                NewAoiTagMenuItem.NewAoiTag(_dialogInfo, Routine.ParentCollection.ParentProgram, false,
                    StxEditorDocument);
            else
            {
                var setting = GlobalSetting.GetInstance().TagSetting.Scope;
                ITagCollection tagCollection = setting == null ? Controller.GetInstance().Tags : setting.Tags;
                if (string.IsNullOrEmpty(_dialogInfo.CodeText))
                    NewTagMenuItem.NewTag(tagCollection, StxEditorDocument);
                else
                    CreateNewTagMenuItem.CreateNewTag(_dialogInfo, tagCollection,
                        StxEditorDocument);
            }
        }

        public RelayCommand BrowseTagsCommand { get; }

        private bool CanExecuteBrowseTagsCommand()
        {
            ParseMenuType();
            return _expectedMenuType == ExpectedMenuType.Empty || _expectedMenuType == ExpectedMenuType.ST ||
                   _expectedMenuType == ExpectedMenuType.None || _expectedMenuType == ExpectedMenuType.Tag ||
                   _expectedMenuType == ExpectedMenuType.Selected;
        }

        private void ExecuteBrowseTagsCommand()
        {
            Editor.TextArea.TextView.GetVisualPosition(Editor.TextArea.Caret.Position, VisualYPosition.Baseline);
            try
            {
                var layer = AdornerLayer.GetAdornerLayer(Editor);
                layer?.Add(_browseAdorner);
                Options.CanZoom = false;
                Editor.GotFocus += Editor_GotFocus;
                Editor.TextArea.TextView.ScrollOffsetChanged += TextView_ScrollOffsetChanged;
                Editor.Dispatcher.BeginInvoke((Action) delegate() { _browseAdorner.SetTextFocus(); },
                    DispatcherPriority.ApplicationIdle);
            }
            catch (Exception)
            {
                //
            }
        }

        private void Editor_GotFocus(object sender, RoutedEventArgs e)
        {
            string name = _browseAdorner.GetName();
            Editor.Document.Replace(_dialogInfo.Offset, _dialogInfo.CodeText?.Length ?? 0,
                new StringTextSource(name ?? ""));
            Editor.GotFocus -= Editor_GotFocus;
            Editor.TextArea.TextView.ScrollOffsetChanged -= TextView_ScrollOffsetChanged;
        }

        private void TextView_ScrollOffsetChanged(object sender, EventArgs e)
        {
            var vector = Editor.TextArea.TextView.ScrollVector;
            _browseAdorner.DoScrollChanged(vector);
        }

        public RelayCommand AddSTElementCommand { get; }

        private bool CanExecuteAddSTElementCommand()
        {
            ParseMenuType();
            return _expectedMenuType == ExpectedMenuType.ST || _expectedMenuType == ExpectedMenuType.Tag ||
                   _expectedMenuType == ExpectedMenuType.Selected;
        }

        private void ExecuteAddSTElementCommand()
        {
            AddSTElementMenuItem.AddSTElement(Controller, Document, _dialogInfo);
        }

        public RelayCommand CopyCommand { get; }

        private void ExecuteCopyCommand()
        {
            CopyMenuItem.Copy(Editor);
        }

        public RelayCommand CutCommand { get; }

        private void ExecuteCutCommand()
        {
            CutMenuItem.Cut(Editor);
        }

        public RelayCommand PasteCommand { get; }

        private bool CanExecutePasteCommand()
        {
            return !string.IsNullOrEmpty(Clipboard.GetText());
        }

        private void ExecutePasteCommand()
        {
            PasteMenuItem.Paste(Editor);
        }

        public RelayCommand ArgumentListCommand { get; }

        private void ExecuteArgumentListCommand()
        {
            //TODO(zyl):add command
        }

        private bool CanExecuteArgumentListCommand()
        {
            return false;
        }

        private RelayCommand WatchTagsCommand { get; }

        private void ExecuteWatchTagsCommand()
        {
            //TODO(zyl):add command
        }

        private bool CanExecuteWatchTagsCommand()
        {
            return false;
        }
        #endregion
    }
}
