using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Antlr4.Runtime.Misc;
using ICSStudio.AvalonEdit.Document;
using ICSStudio.AvalonEdit.Variable;
using ICSStudio.Dialogs.Filter;
using ICSStudio.Gui.Annotations;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.StxEditor.Interfaces;
using ICSStudio.StxEditor.ViewModel.CodeSnippets;
using ICSStudio.StxEditor.ViewModel.Highlighting;
using Microsoft.VisualStudio.Shell;
using Type = System.Type;

namespace ICSStudio.StxEditor.ViewModel
{
    public class StxEditorDocument : IWeakEventListener, INotifyPropertyChanged
    {
        private static readonly double MergeUndoItemsTimeThreshold = 100.0;

        private readonly DispatcherTimer _parseDocumentTimer;
        private readonly TextMarkerService _textMarkerService;
        private readonly SnippetLexer _snippetLexer;
        private readonly StxEditorOptions _options;

        public StxEditorDocument(TextDocument document, IStxEditorOptions options, TextMarkerService textMarkerService,
            IRoutine routine)
        {
            Document = document;
            Routine = routine as STRoutine;
            _options = (StxEditorOptions)options;
            WeakEventManager<TextDocument, DocumentChangeEventArgs>.AddHandler(document, "Changed", DocumentOnChanged);
            //document.Changed += DocumentOnChanged;
            _textMarkerService = textMarkerService;
            Reset();
            PropertyChangedEventManager.AddListener(options, this, string.Empty);
            _parseDocumentTimer = new DispatcherTimer
            {
                IsEnabled = false,
                Interval = TimeSpan.FromMilliseconds(MergeUndoItemsTimeThreshold)
            };
            _parseDocumentTimer.Tick += OnParseDocumentTimerTicked;
            _snippetLexer = new SnippetLexer(textMarkerService,
                routine);
            //_snippetLexer.ParserWholeCode(document.Text, true);
            _lastText = document.Text;
            _snippetLexer.EndParse += _snippetLexer_EndParse;
            FilterViewModel = new FilterViewModel(routine.ParentController, routine.ParentCollection.ParentProgram,
                false, false, "");
        }

        public FilterViewModel FilterViewModel { get; }

        public TextMarkerService TextMarkerService => _textMarkerService;

        public StxEditorOptions Options => _options;

        private void _snippetLexer_EndParse(object sender, ParseCodeArgs e)
        {
            IsChanged = false;
            if (e.IsNeedUpdateView && !_options.Cleanup)
                Update = true;
        }

        public void Reset()
        {
            Original = Routine.CodeText == null ? String.Empty : string.Join("\n", Routine.CodeText);
            Pending = Routine.PendingCodeText == null ? null : string.Join("\n", Routine.PendingCodeText);
            Test = Routine.TestCodeText == null ? null : string.Join("\n", Routine.TestCodeText);
            (Routine.ParentCollection.ParentProgram as Program)?.CheckTestStatus();
        }

        public void Clean()
        {
            _snippetLexer.EndParse -= _snippetLexer_EndParse;
            WeakEventManager<TextDocument, DocumentChangeEventArgs>.RemoveHandler(Document, "Changed",
                DocumentOnChanged);
            PropertyChangedEventManager.RemoveListener(_options, this, string.Empty);
            _parseDocumentTimer.Stop();
            _parseDocumentTimer.Tick -= OnParseDocumentTimerTicked;
        }

        public void OnlyResetCode()
        {
            Original = Routine.CodeText == null ? String.Empty : string.Join("\n", Routine.CodeText);
            Pending = Routine.PendingCodeText == null ? null : string.Join("\n", Routine.PendingCodeText);
            Test = Routine.TestCodeText == null ? null : string.Join("\n", Routine.TestCodeText);
            _recoverData.Clear();
        }

        public STRoutine Routine { get; }

        public bool HasPending => Routine.PendingCodeText != null;

        public bool HasTest => Routine.TestCodeText != null;

        public string Original
        {
            set
            {
                _original = value;
                var tmp = string.Join("\n", Routine.CodeText);
                if (tmp.Equals(_original)) return;
                if (value == string.Empty)
                {
                    Routine.CodeText.Clear();
                    Routine.CodeText.Add(String.Empty);
                    return;
                }
                if (_original == null)
                {
                    Routine.CodeText.Clear();
                }
                else
                {
                    Routine.CodeText = _original.Split('\n').ToList();
                }
            }
            get { return _original; }
        }

        public string Test
        {
            set
            {
                _test = value;
                var tmp = Routine.TestCodeText == null ? null : string.Join("\n", Routine.TestCodeText);
                if (string.Equals(tmp, _test)) return;
                if (value == string.Empty)
                {
                    Routine.TestCodeText = new List<string>();
                    Routine.TestCodeText.Add(String.Empty);
                    return;
                }
                if (_test == null)
                {
                    Routine.TestCodeText = null;
                }
                else
                {
                    Routine.TestCodeText = _test.Split('\n').ToList();
                }
            }
            get { return _test; }
        }

        public string Pending
        {
            set
            {
                _pending = value;

                var tmp = Routine.PendingCodeText == null ? null : string.Join("\n", Routine.PendingCodeText);
                if (string.Equals(tmp,_pending)) return;
                if (value == string.Empty)
                {
                    Routine.PendingCodeText = new List<string>();
                    Routine.PendingCodeText.Add(String.Empty);
                    return;
                }
                if (_pending == null)
                {
                    Routine.PendingCodeText = null;
                }
                else
                {
                    Routine.PendingCodeText = _pending.Split('\n').ToList();
                }
            }
            get { return _pending; }
        }

        public void UpdateCode()
        {
            _original = Routine.CodeText?.Count == 0
                ? String.Empty
                : string.Join("\n", Routine.CodeText ?? new List<string>());
            _pending = Routine.PendingCodeText == null
                ? null
                : string.Join("\n", Routine.PendingCodeText ?? new List<string>());
            _test = Routine.TestCodeText == null ? null : string.Join("\n", Routine.TestCodeText ?? new List<string>());
        }

        public void ChangedEditor()
        {
            if (Routine.CurrentOnlineEditType == OnlineEditType.Original)
            {
                if (!Document.Text.Equals(Original, StringComparison.OrdinalIgnoreCase))
                    Document.Text = Original;
            }

            if (Routine.CurrentOnlineEditType == OnlineEditType.Pending)
            {
                if (!Document.Text.Equals(Pending, StringComparison.OrdinalIgnoreCase))
                    Document.Text = Pending;
            }

            if (Routine.CurrentOnlineEditType == OnlineEditType.Test)
            {
                if (!Document.Text.Equals(Test, StringComparison.OrdinalIgnoreCase))
                    Document.Text = Test;
            }
        }

        public object ShowCaretControl { set; get; }

        //public OnlineEditType CurrentType
        //{
        //    set
        //    {
        //        if (Routine != null)
        //        {
        //            Routine.CurrentOnlineEditType = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //    get { return Routine.CurrentOnlineEditType; }
        //}

        public void LineChange()
        {
            if (DoLineChangeParser && NeedParse)
                try
                {
                    Parser(ref _lastText);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
        }

        public bool DoLineChangeParser { set; get; } = true;

        public void StopVariableConnection()
        {
            Parallel.ForEach(Routine.GetCurrentVariableInfos(Routine.CurrentOnlineEditType),
                v => { ((VariableInfo)v).StopTimer(); });
        }

        public bool IsNeedReParse { set; get; } = true;
        
        public void TransformToOriginal()
        {
            if (Routine.CurrentOnlineEditType == OnlineEditType.Original && !_options.IsOnlyTextMarker)
            {
                return;
            }
            
            WeakEventManager<TextDocument, DocumentChangeEventArgs>.RemoveHandler(Document, "Changed",
            DocumentOnChanged);
            if (IsNeedReParse)
                StopVariableConnection();
            Routine.CurrentOnlineEditType = OnlineEditType.Original;
            Document.Text = Original;
            //if (IsNeedReParse)
                UpdateLexer(false);
            NeedParse = false;
            _recoverData.Clear();
            WeakEventManager<TextDocument, DocumentChangeEventArgs>.AddHandler(Document, "Changed", DocumentOnChanged);
        }

        public void TransformToPending()
        {
            if (Routine.CurrentOnlineEditType == OnlineEditType.Pending && !_options.IsOnlyTextMarker)
            {
                return;
            }
            
            WeakEventManager<TextDocument, DocumentChangeEventArgs>.RemoveHandler(Document, "Changed",
                DocumentOnChanged);
            if (IsNeedReParse)
                StopVariableConnection();
            Routine.CurrentOnlineEditType = OnlineEditType.Pending;
            Document.Text = Pending;
            //if (IsNeedReParse)
                UpdateLexer(false);
            NeedParse = false;
            _recoverData.Clear();
            WeakEventManager<TextDocument, DocumentChangeEventArgs>.AddHandler(Document, "Changed", DocumentOnChanged);
        }

        public void TransformToTest()
        {
            if (Routine.CurrentOnlineEditType == OnlineEditType.Test && !_options.IsOnlyTextMarker)
            {
                return;
            }
            WeakEventManager<TextDocument, DocumentChangeEventArgs>.RemoveHandler(Document, "Changed",
                DocumentOnChanged);
            if (IsNeedReParse)
                StopVariableConnection();
            Routine.CurrentOnlineEditType = OnlineEditType.Test;
            Document.Text = Test;
            //if (IsNeedReParse)
                UpdateLexer(false);
            NeedParse = false;
            _recoverData.Clear();
            WeakEventManager<TextDocument, DocumentChangeEventArgs>.AddHandler(Document, "Changed", DocumentOnChanged);
        }

        public bool NeedParse
        {
            set { _needParse = value; }
            get
            {
                return _needParse;
            }
        }

        public bool IsNeedBackground { set; get; } = true;

        public void UpdateLexer(bool useDispatcher = true, bool isUpdateView = true)
        {
            NeedParse = false;
            if (!useDispatcher)
            {
                //var document = Document.CreateSnapshot();
                if (IsNeedBackground)
                    _snippetLexer.ParserWholeCodeInBackground(GetCurrentCode(), _options.IsOnlyTextMarker);
                else
                    _snippetLexer.ParserWholeCode(GetCurrentCode(), _options.IsOnlyTextMarker);
                if (!_options.IsOnlyTextMarker)
                    Update = true;
            }
            else
                DispatcherHelper.Run(Dispatcher, () => UpdateContainerTextChange(Document, null));
        }

        public void UpdateVariables(string codeText, bool reset)
        {
            if (reset)
                _snippetLexer.Reset();
            _snippetLexer.ParserWholeCode(codeText, false);
        }

        public string GetFormatInstrName(string name)
        {
            var instr = _snippetLexer.StxCompletionItemCodeSnippetDatas.FirstOrDefault(i =>
                i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (instr != null) return instr.Name;
            return name;
        }

        public bool Update
        {
            set
            {
                ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    _update = value;
                    OnPropertyChanged();
                });
            }
            get { return _update; }
        }

        private void Parser(ref string lastText)
        {
            if (lastText != Document.Text && NeedParse)
            {
                //DispatcherHelper.Run(Dispatcher, () => UpdateContainerTextChange(Document, null));
                _snippetLexer.ParserWholeCodeInBackground(Document.Text, false);
                lastText = Document.Text;
                NeedParse = false;
                _recoverData.Clear();
            }
            else
            {
                RecoverVariable();
            }
        }

        public void SetAoiConnection(AoiDataReference aoiDataReference)
        {
            SnippetLexer.ConnectionReference = aoiDataReference;
        }

        public SnippetLexer SnippetLexer => _snippetLexer;

        public ConcurrentQueue<IVariableInfo> VariableInfos =>
            Routine.GetCurrentVariableInfos(Routine.CurrentOnlineEditType);

        public TextDocument Document { get; }

        public Dispatcher Dispatcher { get; set; }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (!(managerType == typeof(PropertyChangedEventManager)))
                return false;

            //TODO(gjc): add code here
            // for IStxEditorOptions

            return true;
        }

        public bool InTabInserting { set; get; } = false;

        private string _lastText;
        private string _original;
        private string _test;
        private string _pending;

        //private string GetChangedText(DocumentChangeEventArgs e)
        //{
        //    var text = Document.Text;
        //    if (!string.IsNullOrEmpty(e.RemovedText.Text))
        //    {
        //        text = text.Remove(e.Offset, e.RemovalLength);
        //    }

        //    if (!string.IsNullOrEmpty(e.InsertedText.Text))
        //    {
        //        text = text.Insert(e.Offset, e.InsertedText.Text);
        //    }

        //    return text;
        //}

        private void RemoveVariableInLine(int line)
        {
            for (int i = 0; i < VariableInfos.Count; i++)
            {
                IVariableInfo variableInfo;
                var result = VariableInfos.TryDequeue(out variableInfo);
                if (result)
                {
                    if (((VariableInfo)variableInfo).TextLocation.Line != line)
                    {
                        VariableInfos.Enqueue(variableInfo);
                    }
                    else
                    {
                        _recoverData.Add(variableInfo);
                        i--;
                    }
                }
            }
        }

        private readonly List<IVariableInfo> _recoverData =
            new ArrayList<IVariableInfo>();

        private bool _update;
        private bool _needParse = false;

        private void RecoverVariable()
        {
            var flag = false;
            foreach (var variableInfo in _recoverData)
            {
                flag = true;
                VariableInfos.Enqueue(variableInfo);
            }

            if (flag)
                Update = true;
            _recoverData.Clear();
        }

        public bool IsChanged { get; private set; }
        public bool CanExecuteDocumentChanged { set; get; } = true;

        private void DocumentOnChanged(object sender, DocumentChangeEventArgs e)
        {
            if (!CanExecuteDocumentChanged) return;
            IsChanged = true;
            var line = GetPosLine(e.Offset);
            var changedText = Document.Text;
            //var changedText = GetChangedText(e);

            RemoveVariableInLine(line.LineNumber);
            if (Routine.CurrentOnlineEditType == OnlineEditType.Test)
            {
                Test = changedText;
            }
            else if (Routine.CurrentOnlineEditType == OnlineEditType.Pending)
            {
                Pending = changedText;
            }
            else
            {
                Original = changedText;
            }

            if (InTabInserting)
            {
                if (e.InsertedText.Text?.IndexOf("end_") > -1 || e.InsertedText.Text?.IndexOf("#endregion") > -1)
                {
                    _snippetLexer.ParserWholeCodeInBackground(changedText, false);
                    NeedParse = false;
                    InTabInserting = false;
                }

                return;
            }

            if ((e.InsertedText.Text?.IndexOf("\n") > -1 || e.RemovedText.Text?.IndexOf("\n") > -1))
            {
                NeedParse = false;
                _snippetLexer.ParserWholeCodeInBackground(changedText, false);
                DoLineChangedEvent = false;
            }
            else
            {
                //Document.InLintLayerInitial = false;
                NeedParse = true;
                _textMarkerService.RemoveLineMarker(line);
                _textMarkerService.RemoveMarketInComment(changedText);
            }

            (Routine.ParentCollection.ParentProgram as AoiDefinition)?.UpdateChangeHistory();
        }

        public bool DoLineChangedEvent { internal set; get; }

        private DocumentLine GetPosLine(int pos)
        {
            foreach (var line in Document.Lines)
            {
                if (line.Offset <= pos && line.EndOffset >= pos)
                    return line;
            }

            Debug.Assert(false);
            return null;
        }

        // ReSharper disable UnusedParameter.Local
        private void UpdateContainerTextChange(object sender, DocumentChangeEventArgs e)
        {
            StartParseDocumentTimer();
        }
        // ReSharper restore UnusedParameter.Local

        private void OnParseDocumentTimerTicked(object sender, EventArgs e)
        {
            _parseDocumentTimer.Stop();

            //TODO(gjc): add code here
            //var document = Document.CreateSnapshot();
            if (IsNeedBackground && !_options.IsOnlyTextMarker)
                _snippetLexer.ParserWholeCodeInBackground(GetCurrentCode(), false);
            else
                _snippetLexer.ParserWholeCode(GetCurrentCode(), false);

        }

        private void StartParseDocumentTimer()
        {
            if (_parseDocumentTimer.IsEnabled)
                _parseDocumentTimer.Stop();

            _parseDocumentTimer.Start();
        }

        public string GetCurrentCode()
        {
            if (Routine.CurrentOnlineEditType == OnlineEditType.Original)
                return Original;
            if (Routine.CurrentOnlineEditType == OnlineEditType.Pending)
                return Pending;
            if (Routine.CurrentOnlineEditType == OnlineEditType.Test)
                return Test;
            Debug.Assert(false);
            return Original;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}