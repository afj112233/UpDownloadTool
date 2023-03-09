using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.AvalonEdit.Document;
using ICSStudio.AvalonEdit.Variable;
using ICSStudio.SimpleServices.Common;
using ICSStudio.StxEditor.View;
using ICSStudio.StxEditor.ViewModel;
using ICSStudio.StxEditor.ViewModel.CodeSnippets;
using ICSStudio.StxEditor.ViewModel.Highlighting;

namespace ICSStudio.StxEditorTest
{
    public class MainViewModel : ViewModelBase
    {
        private STRoutine stRoutine = null;
        private StxEditorDocument document = null;
        public MainViewModel(Dispatcher dispatcher)
        {
            DisplayInlineValueCommand=new RelayCommand(ExecuteDisplayInlineValueCommand);
            string file = @"C:\Users\zyl\Desktop\zzzzzzzzzz\0216.json";
            //string file = @"D:\icsstudio\data\L5XToJsonChecked\for_stmt.json";

            var controller = Controller.Open(file);
            //R02_Durability_Test
            //STRoutine stRoutine = controller.Programs["MainProgram"].Routines["R01_Main"] as STRoutine;
            stRoutine = controller.Programs["AlamTips"].Routines["A02_Tips"] as STRoutine;
            // stRoutine = controller.Programs["MainProgram1"].Routines["CM06_SetSegment"] as STRoutine;
            Contract.Assert(stRoutine != null);

            var textDocument = new TextDocument(string.Join("\n", stRoutine.CodeText));
            var options = new StxEditorOptions();
            TextMarkerService textMarkerService = new TextMarkerService(textDocument);

            document = new StxEditorDocument(textDocument, options, textMarkerService,
                stRoutine)
            {
                Dispatcher = dispatcher
            };

            StxEditorControl = new StxEditorView(stRoutine, document, options);

            //show inline display
            

            document.ShowCaretControl = StxEditorControl;
            FormatCommand=new RelayCommand(ExecuteFormatCommand);
        }
        
        public RelayCommand DisplayInlineValueCommand { get; }

        private void ExecuteDisplayInlineValueCommand()
        {
            if (((StxEditorView) StxEditorControl).TextEditor.TextArea.TextView.LineSpacing == 2)
            {
                ((StxEditorView)StxEditorControl).TextEditor.TextArea.TextView.LineSpacing = 1;
                ((StxEditorView)StxEditorControl).TextEditor.TextArea.TextView.Redraw();
            }
            else
            {
                ((StxEditorView)StxEditorControl).TextEditor.TextArea.TextView.LineSpacing = 2;
                ((StxEditorView)StxEditorControl).TextEditor.TextArea.TextView.Redraw();
            }
               
        }

        public object StxEditorControl { get; }

        public RelayCommand FormatCommand { get; }

        private void ExecuteFormatCommand()
        {
            var formatCode = FormatCode(document.Original);
            Debug.Assert(string.Join("\n", formatCode).ToLower().Equals(document.Original.ToLower()));
        }

        private List<string> FormatCode(string snippets)
        {
            foreach (VariableInfo variableInfo in document.VariableInfos)
            {
                if (variableInfo.IsNum) continue;
                if (variableInfo.Offset + variableInfo.Name.Length >= snippets.Length) continue;
                if (variableInfo.IsInstr)
                {
                    var formatCode = document.GetFormatInstrName(variableInfo.Name);
                    snippets = Replace(snippets, variableInfo.Offset, variableInfo.Name.Length, formatCode);
                }
                else
                {
                    var formatCode = ExtendOperation.FormatCode(variableInfo.AstNode,variableInfo.Name, stRoutine);
                    snippets = Replace(snippets, variableInfo.Offset, variableInfo.Name.Length, formatCode);
                }

            }

            var codeText = snippets.Split('\n');
            return codeText.ToList();
        }


        private string Replace(string text, int index, int length, string newString)
        {
            if (string.IsNullOrEmpty(text)) return text;
            text = text.Remove(index, length).Insert(index, newString);
            return text;
        }

    }
}
