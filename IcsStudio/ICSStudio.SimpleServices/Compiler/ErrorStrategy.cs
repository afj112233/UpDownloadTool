using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace ICSStudio.SimpleServices.Compiler
{
    public class ErrorStrategy:DefaultErrorStrategy
    {
        public override void Recover(Parser recognizer, RecognitionException e)
        {
            var tokens = recognizer.TokenStream;
            //GetMissingSymbol(recognizer);
            if (tokens.LA(1) != STGrammarLexer.Eof)
            {
                tokens.Consume();
                base.Recover(recognizer, e);
            }
        }

        public override void ReportMatch(Parser recognizer)
        {
            base.ReportMatch(recognizer);
        }
        protected override void ReportNoViableAlternative(Parser recognizer, NoViableAltException e)
        {
            var tokens = recognizer.TokenStream;
            //GetMissingSymbol(recognizer);
            if (tokens.LA(1) != STGrammarLexer.Eof)
            {
                tokens.Consume();
                base.Recover(recognizer, e);
            }
            //Sync(recognizer);
            //recognizer.NotifyErrorListeners(e.OffendingToken, "can not parse this token.", e);
            //base.ReportNoViableAlternative(recognizer, e);
        }
        
        public override void Sync(Parser recognizer)
        {
            
        }
    }

    public class ParserListener : ConsoleErrorListener<IToken>
    {
        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line,
            int charPositionInLine, string msg, RecognitionException e)
        {
            var args = ParserError.ParseError(msg, e,line,charPositionInLine);
            SyntaxErrorEventHandler?.Invoke(this, args);
        }

        public event EventHandler<SyntaxErrorArgs> SyntaxErrorEventHandler;
    }
    
    public class LexerListener : ConsoleErrorListener<int>
    {
        public override void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line,
            int charPositionInLine, string msg, RecognitionException e)
        {
            var args = ParserError.ParseError(msg, e,line,charPositionInLine);
            SyntaxErrorEventHandler?.Invoke(this, args);
        }
        
        public event EventHandler<SyntaxErrorArgs> SyntaxErrorEventHandler;
    }

    internal class ParserError
    {
        public static SyntaxErrorArgs ParseError(string msg, RecognitionException e, int line, int column)
        {
            if (e != null)
            {
                var error = "";
                var len = 1;
                if (e.Message.Contains("Antlr4.Runtime.LexerNoViableAltException"))
                {
                    var index = msg.IndexOf('\'');
                    if (index > -1)
                    {
                        error = msg.Substring(index);
                        len = error.Length - 2;
                        error = $"{error}:Unexpected.";
                    }
                    else
                    {
                        error = msg;
                    }
                }
                else if (e.Message.Contains("Antlr4.Runtime.InputMismatchException"))
                {
                    error = msg;
                }
                else
                {
                    error = "";
                }
                return new SyntaxErrorArgs(line,column,error,len);
            }
            else
            {
               return new SyntaxErrorArgs(line,column,"",1);
            }
        }
    }

    public class SyntaxErrorArgs
    {
        public SyntaxErrorArgs(int line,int column,string msg,int length)
        {
            Line = line;
            Column = column;
            Message = msg;
            Length = length;
        }
        public int Line { get; }

        public int Column { get; }

        public string Message { get; }

        public int Length { get; }
    }
}
