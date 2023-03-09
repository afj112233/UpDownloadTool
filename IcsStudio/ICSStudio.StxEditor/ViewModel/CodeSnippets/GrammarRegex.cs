using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using Antlr4.Runtime;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.StxEditor.ViewModel.Highlighting;
using ICSStudio.StxEditor.ViewModel.IntelliPrompt;

namespace ICSStudio.StxEditor.ViewModel.CodeSnippets
{
    internal class GrammarRegex:IDisposable
    {
        private bool _isDisposed = false;
        private LexerListener _listListener;
        private ParserListener _parserListener;
        private TextMarkerService _textMarkerService;
        private STRoutine _routine;
        private List<Tuple<int, int>> _commentList;
        private bool _canAddError;
        private Action<int,int> _setErrorLineAction;
        private string _code = "";
        public GrammarRegex(TextMarkerService textMarkerService, STRoutine stRoutine, List<Tuple<int, int>> commentList,
            bool canAddError, Action<int, int> setErrorLineAction)
        {
            _listListener = new LexerListener();
            _parserListener=new ParserListener();
            _textMarkerService = textMarkerService;
            _routine = stRoutine;
            _commentList = commentList;
            _canAddError = canAddError;
            _setErrorLineAction = setErrorLineAction;
        }

        public ParserRuleContext GetParseTree(string str)
        {
            var stream = new AntlrInputStream(str);
            _code = str;
            var lexer = new STGrammarLexer(stream);
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(_listListener);
            _listListener.SyntaxErrorEventHandler += ListListener_SyntaxErrorEventHandler;
            var token=new CommonTokenStream(lexer);
            var parser = new STGrammarParser(token);
            _parserListener.SyntaxErrorEventHandler += ListListener_SyntaxErrorEventHandler;
            parser.RemoveErrorListeners();
            parser.AddErrorListener(_parserListener);
            return parser.start();
        }
        
        private void ListListener_SyntaxErrorEventHandler(object sender, SyntaxErrorArgs e)
        {
            lock (_textMarkerService.LockObject)
            {
                if(string.IsNullOrEmpty(e.Message))return;
                _routine.IsError = true;
                var start = _textMarkerService.TextDocument.GetOffsetWithoutVerifyAccess(e.Line, e.Column + 1);
                int end = start + e.Length - 1;
                if (start > end)
                {
                    start = start ^ end;
                    end = start ^ end;
                    start = start ^ start;
                }

                _setErrorLineAction.Invoke(start, end);
                _textMarkerService.CreateSkinCommentAndBlankLine(start, end - start + 1, _code, Colors.LimeGreen,
                    _commentList, _canAddError, e.Message);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDispose)
        {
            if(_isDisposed)return;
            if (isDispose)
            {
                _listListener.SyntaxErrorEventHandler -= ListListener_SyntaxErrorEventHandler;
                _parserListener.SyntaxErrorEventHandler -= ListListener_SyntaxErrorEventHandler;
            }
        }
    }
}
