using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.AvalonEdit.Document;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.CodeAnalysis;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Online;
using ICSStudio.StxEditor.ViewModel.CodeSnippets;
using ICSStudio.StxEditor.ViewModel.Highlighting;

// ReSharper disable once CheckNamespace
namespace ICSStudio.ToolForCompilerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var path = $"{System.Environment.CurrentDirectory}/PendingCheckedProjects";
                var dirInfo = new DirectoryInfo(path);
                foreach (var fileInfo in dirInfo.GetFiles())
                {
                    Console.WriteLine("***开始加载工程***");
                    var controller = Controller.Open(fileInfo.FullName);
                    Console.WriteLine($"***加载工程{controller.Name}完成***");
                    foreach (var program in controller.Programs)
                    {
                        foreach (var routine in program.Routines)
                        {
                            ParseRoutine(routine);
                            Console.WriteLine("");
                            Console.WriteLine("");
                        }
                    }
                    Console.WriteLine($"***************************************");
                    Console.WriteLine($"***                                 ***");
                    Console.WriteLine($"***                                 ***");
                    Console.WriteLine($"      工程{controller.Name}语法检查完成    ");
                    Console.WriteLine($"***                                 ***");
                    Console.WriteLine($"***                                 ***");
                    Console.WriteLine("****************************************");
                    Console.WriteLine("");
                    Console.WriteLine("");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            Console.ReadLine();
        }

        private static void ParseRoutine(IRoutine routine)
        {
            if (routine is STRoutine)
            {
                Console.WriteLine($"开始校验 '{routine.Name}' - '{routine.ParentCollection.ParentProgram.Name}'");
                {
                    var st = routine as STRoutine;
                    {
                        for (int i = 0; i < st.CodeText.Count; i++)
                        {
                            Console.WriteLine($"第{i + 1}行:{st.CodeText[i]}");
                        }
                        string code = string.Join("\n", st.CodeText);

                        var document = new TextDocument(code);

                        TextMarkerService textMarkerService = new TextMarkerService(document, st);
                        textMarkerService.OnlyAddError = true;
                        var lexer = new SnippetLexer(textMarkerService, st);
                        lexer.CanAddError = true;
                        st.CurrentOnlineEditType = OnlineEditType.Original;
                        lexer.ParserWholeCode(code, false);
                        if (st.PendingCodeText != null)
                        {
                            st.CurrentOnlineEditType = OnlineEditType.Pending;
                            code = string.Join("\n", st.PendingCodeText);
                            document.Text = code;
                            lexer.ParserWholeCode(code, false);
                        }

                        if (st.TestCodeText != null)
                        {
                            st.CurrentOnlineEditType = OnlineEditType.Test;
                            code = string.Join("\n", st.TestCodeText);
                            document.Text = code;
                            lexer.ParserWholeCode(code, false);
                        }

                        st.CurrentOnlineEditType = OnlineEditType.Original;
                        Console.WriteLine($"*****共有{textMarkerService.ErrorCount}错误*****");
                        textMarkerService.Clear();
                    }
                }
            }

            if (routine is RLLRoutine)
            {
                var ld = routine as RLLRoutine;
                try
                {
                    //var codeText = ld.CodeText;
                    Console.WriteLine($"开始校验 '{routine.Name}' - '{routine.ParentCollection.ParentProgram.Name}'");

                    RLLGrammarAnalysis grammarAnalysis = new RLLGrammarAnalysis();
                    //grammarAnalysis.Analysis(string.Join("    ", codeText), (Controller)ld.ParentController, ld);
                    grammarAnalysis.Analysis(ld);

                    foreach (var error in grammarAnalysis.Errors)
                    {
                        if (error.Level == Level.Error)
                            Console.WriteLine(error.Info);
                        if (error.Level == Level.Warning)
                            Console.WriteLine(error.Info);
                    }
                }
                catch (ErrorInfo error)
                {
                    Console.WriteLine(error.Message + error.StackTrace);
                    if (error.Level == Level.Error)
                        Console.WriteLine(error.Info);
                    if (error.Level == Level.Warning)
                        Console.WriteLine(error.Info);
                }
                catch (Exception)
                {
                    Console.WriteLine("Unknown Error.");
                }
            }

            //TODO(zyl):add other routine
        }
    }
}
