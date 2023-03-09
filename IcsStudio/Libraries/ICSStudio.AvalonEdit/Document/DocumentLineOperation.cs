using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSStudio.AvalonEdit.Document
{
    sealed class DocumentLineOperation: IUndoableOperation
    {
        private TextDocument _textDocument;
        private bool? _oldValue;
        private bool? _newValue;
        private int _line;
        public DocumentLineOperation(TextDocument document,int line,bool? oldValue,bool? newValue,bool isComment)
        {
            _textDocument = document;
            _line = line;
            _oldValue = oldValue;
            _newValue = newValue;
            IsComment = isComment;
        }

        public bool IsComment { get; } = false;

        public void Undo()
        {
            var line = _textDocument.GetLineByNumber(_line);
            line.IsEdit = _oldValue;
        }

        public void Redo()
        {
            var line = _textDocument.GetLineByNumber(_line);
            line.IsEdit = _newValue;
        }
    }

    //sealed class DocumentRemoveOperation : IUndoableOperation
    //{
    //    private TextDocument _textDocument;
    //    private bool? _oldValue;
    //    private bool? _newValue;
    //    private int _offset;
    //    private int _len;
    //    public DocumentRemoveOperation(TextDocument document,int offset,int len, bool? oldValue, bool? newValue)
    //    {
    //        _offset = offset;
    //        _len = len;
    //        _textDocument = document;
    //        _oldValue = oldValue;
    //        _newValue = newValue;
    //    }
    //    public void Undo()
    //    {
    //        var text=_textDocument.GetText(_offset, _len);
    //        var startLine = _textDocument.GetLineByOffset(_offset);
    //        for (int i = 0; i < text.Count(c=>c=='\n'); i++)
    //        {
    //            var line = _textDocument.GetLineByNumber(startLine.LineNumber + i);
    //            line.IsEdit = _oldValue;
    //        }
    //    }

    //    public void Redo()
    //    {
    //        var text = _textDocument.GetText(_offset, _len);
    //        var startLine = _textDocument.GetLineByOffset(_offset);
    //        for (int i = 0; i < text.Count(c => c == '\n'); i++)
    //        {
    //            var line = _textDocument.GetLineByNumber(startLine.LineNumber + i);
    //            line.IsEdit = _newValue;
    //        }
    //    }
    //}
}
