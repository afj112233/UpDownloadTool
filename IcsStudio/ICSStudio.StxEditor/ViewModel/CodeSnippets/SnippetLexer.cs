using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Antlr4.Runtime;
using ICSStudio.AvalonEdit.Document;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Instruction;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.StxEditor.ViewModel.Highlighting;
using ICSStudio.StxEditor.ViewModel.IntelliPrompt;
using ICSStudio.UIInterfaces.Error;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Task = System.Threading.Tasks.Task;
using VariableInfo = ICSStudio.AvalonEdit.Variable.VariableInfo;

namespace ICSStudio.StxEditor.ViewModel.CodeSnippets
{

    public enum Type
    {
        Instr,
        Name,
        Enum,
        Num,
        Tag,
        None,
        InoutParameter,
        Keyword,
        Routine,
        Program,
        Module,
        Task,
    }

    public partial class SnippetLexer
    {
        private readonly TypeChecker _typeChecker;
        private readonly IController _controller;
        private readonly IProgramModule _parentProgram;
        private readonly STASTGenVisitor _stastGenVisitor;
        private readonly TextMarkerService _textMarkerService;
        private readonly STRoutine _routine;
        private readonly ConcurrentQueue<SnippetInfo> _snippetInfos = new ConcurrentQueue<SnippetInfo>();
        private AoiDataReference _connectionReference;
        private readonly List<Tuple<int, int>> _errorRange = new List<Tuple<int, int>>();
        private bool _isInitial;
        private List<Tuple<int, int>> CommentList { get; } = new List<Tuple<int, int>>();
        private bool _onlyTextMarker = false;
        readonly BackgroundWorker _backgroundWorker;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        private readonly string[] _keywords =
        {
            "#endregion", "by", "do", "else",
            "end_case", "end_for", "end_if", "end_repeat", "end_while",
            "exit", "of", "then", "to", "until", "and", "mod", "or", "xor", "not"
        };

        public SnippetLexer(TextMarkerService textMarkerService, IRoutine routine, bool addAoiDataReference = false)
        {
            _isInitial = !addAoiDataReference;
            _textMarkerService = textMarkerService;
            StxCompletionItemCodeSnippetDatas = GenerateCodeSnippetItems();
            _stastGenVisitor = new STASTGenVisitor();
            _controller = routine.ParentController;
            _routine = (STRoutine) routine;
            _parentProgram = routine.ParentCollection.ParentProgram;
            _typeChecker = new TypeChecker(_controller as Controller, _parentProgram as IProgram,
                _parentProgram as AoiDefinition);
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.WorkerSupportsCancellation = true;
            _backgroundWorker.DoWork += (sender, e) =>
            {
                if (_isRunInMain) return;
                _routine.IsCompiling = true;
                var argument = (Argument) e.Argument;
                try
                {
                    ParseCode(argument.Code, argument.IsOnlyTextMarker);
                }
                catch (Exception ex)
                {
                    Debug.Assert(false,ex.StackTrace);
                    _semaphoreSlim.Release();
                    return;
                }

                _semaphoreSlim.Release();
                if (_backgroundWorker.CancellationPending)
                {
                    return;
                }
            };

            _backgroundWorker.RunWorkerCompleted += (s, e) =>
            {
                if (!_backgroundWorker.CancellationPending)
                {
                    _routine.IsCompiling = false;
                    _isRunInMain = false;
                    if (!_routine.ParentController.IsLoading)
                    {
                        EndParse?.Invoke(this, new ParseCodeArgs() { IsNeedUpdateView = true });
                    }

                    _semaphoreSlim.Release();
                }
            };
        }
        
        public int GetEndOfCode(int offset, string code)
        {
            var node = _routine.GetCurrentMod(_routine.CurrentOnlineEditType)?.list.nodes.ToList()
                .LastOrDefault(n => n.ContextStart <= offset);
            if (node == null)
            {
                return offset;
            }
            else
            {
                var subCode = node.ContextEnd + 1 >= code.Length ? code : code.Substring(0, node.ContextEnd + 1);
                if (subCode.EndsWith("if", StringComparison.OrdinalIgnoreCase))
                {
                    return node.ContextEnd - 2;
                }

                if (subCode.EndsWith("case", StringComparison.OrdinalIgnoreCase))
                {
                    return node.ContextEnd - 4;
                }

                if (subCode.EndsWith("for", StringComparison.OrdinalIgnoreCase))
                {
                    return node.ContextEnd - 3;
                }

                if (subCode.EndsWith("while", StringComparison.OrdinalIgnoreCase))
                {
                    return node.ContextEnd - 5;
                }

                return node.ContextEnd;
            }
        }

        public class Argument
        {
            public string Code { set; get; }

            public bool IsOnlyTextMarker { set; get; }
        }

        public event EventHandler<ParseCodeArgs> EndParse;

        public void Reset()
        {
            _isInitial = true;
        }

        public AoiDataReference ConnectionReference
        {
            set
            {
                _connectionReference = value;
                Connection();
            }
            get { return _connectionReference; }
        }
        
        private ConcurrentQueue<IVariableInfo> VariableInfos =>
            _routine.GetCurrentVariableInfos(_routine.CurrentOnlineEditType);

        private void Connection()
        {
            try
            {
                if (ConnectionReference?.Routine == null)
                {
                    TransformTable = null;
                    foreach (VariableInfo variableInfo in VariableInfos)
                    {
                        if (_backgroundWorker.CancellationPending)
                        {
                            return;
                        }

                        variableInfo.Connection(_parentProgram, null);
                    }

                    return;
                }

                var program = ConnectionReference.GetReferenceProgram();

                {

                    TransformTable = new Hashtable();
                    var aoi = (AoiDefinition) _routine.ParentCollection.ParentProgram;
                    foreach (var tag in aoi.Tags)
                    {
                        if (_backgroundWorker.CancellationPending)
                        {
                            return;
                        }

                        TransformTable[tag.Name.ToUpper()] = ConnectionReference.GetTransformName(tag.Name);
                    }
                }

                foreach (VariableInfo variableInfo in VariableInfos)
                {
                    if (_backgroundWorker.CancellationPending)
                    {

                        return;
                    }

                    if (variableInfo.IsNum || variableInfo.IsEnum || !variableInfo.IsDisplay) continue;
                    variableInfo.Connection(program, TransformTable);
                }

            }
            catch (Exception e)
            {
                Debug.Assert(false, e.Message);
            }
        }

        internal Hashtable TransformTable { private set; get; }
        internal List<StxCompletionItemCodeSnippetData> StxCompletionItemCodeSnippetDatas { get; }

        private bool _isRunInMain = false;

        public void ParserWholeCode(string codeSnippet, bool onlyTextMarker, bool isForce = false)
        {
            try
            {
                _isRunInMain = true;
                if (_backgroundWorker.IsBusy)
                {
                    if (isForce)
                    {
                        _backgroundWorker.CancelAsync();
                        return;
                    }

                    _routine.IsCompiling = false;
                    _isRunInMain = false;
                    return;
                }

                if (_routine.IsCompiling)
                {
                    _isRunInMain = false;
                    return;
                }

                _routine.IsCompiling = true;
                ParseCode(codeSnippet, onlyTextMarker);
                _routine.IsCompiling = false;
                if (!_routine.ParentController.IsLoading)
                    EndParse?.Invoke(this, new ParseCodeArgs() {IsNeedUpdateView = false});
                _isRunInMain = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public void ParserWholeCodeInBackground(string codeSnippet, bool onlyTextMarker)
        {
            Task.Run(async delegate
            {
                if (_semaphoreSlim.Wait(1))
                {
                    _routine.IsCompiling = true;
                    if (_backgroundWorker.IsBusy)
                    {
                        _backgroundWorker.CancelAsync();
                    }

                    while (_backgroundWorker.IsBusy)
                    {
                        await Task.Delay(10);
                    }

                    _backgroundWorker.RunWorkerAsync(new Argument()
                        {Code = codeSnippet, IsOnlyTextMarker = onlyTextMarker});
                }
            });
        }

        private void ParseCode(string codeSnippet, bool onlyTextMarker)
        {
            if (onlyTextMarker)
            {
                lock (_textMarkerService.LockObject)
                {
                    _textMarkerService.Clear();
                    var astStmtMod = _routine.GetCurrentMod(_routine.CurrentOnlineEditType);
                    if (astStmtMod != null)
                    {
                        _textMarkerService.IsAddTextMarker = false;
                        foreach (var textMarker in astStmtMod.TextMarkers)
                        {
                            _textMarkerService?.CreateSkinCommentAndBlankLine(textMarker.Item2, textMarker.Item3,
                                codeSnippet, textMarker.Item4, new List<Tuple<int, int>>(), false, textMarker.Item1);
                        }

                        _textMarkerService.IsAddTextMarker = true;
                    }
                }
                return;
            }
            _routine.IsError = false;
            if (CanAddError)
            {
                var errorOutput = Package.GetGlobalService(typeof(SErrorOutputService)) as IErrorOutputService;
                errorOutput?.RemoveError(_routine, _routine.CurrentOnlineEditType);
                _addErrors.Clear();
            }
           
            {
                if (_backgroundWorker.CancellationPending)
                {
                    return;
                }

                bool isOk = true;
                while (isOk)
                {
                    IVariableInfo variable;
                    isOk = VariableInfos.TryDequeue(out variable);
                    variable?.Dispose();
                }

                CleanAoiReference();
            }


            lock (_textMarkerService.LockObject)
            {
                _textMarkerService.Clear();
            }

            {
                var isOk = true;
                while (isOk)
                {
                    SnippetInfo info;
                    isOk = _snippetInfos.TryDequeue(out info);
                }
            }
            //_snippetInfos.Clear();
            _errorRange.Clear();
            if (_backgroundWorker.CancellationPending)
            {

                return;
            }

            if (string.IsNullOrEmpty(codeSnippet))
            {
                return;
            }

            ParserByEach(codeSnippet, false);
            if (ConnectionReference?.Routine != null)
            {
                Connection();
            }
            else
            {
                if (!_onlyTextMarker)
                {
                    FixVariableInfos();
                }
            }
            
            _isInitial = false;
        }
        
        internal Type GetType(SnippetInfo targetInfo,IProgramModule program)
        {
            if (targetInfo == null)
            {
                return Type.None;
            }
           
                if (_routine.IsCompiling)
                {
                    return ObtainValue.NameToTag(targetInfo.CodeText, TransformTable, _routine.ParentCollection.ParentProgram)?.Item1 != null
                        ? Type.Tag
                        : Type.None;
                }

            try
            {
                if (targetInfo.IsRoutine) return Type.Routine;
                if (targetInfo.IsProgram) return Type.Program;
                if (targetInfo.IsModule) return Type.Module;
                if (targetInfo.IsEnum) return Type.Enum;
                if (targetInfo.IsTask) return Type.Task;
                if (targetInfo.IsAOI) return Type.Instr;
                if (targetInfo.IsInstr) return Type.Instr;
                if (targetInfo.IsNum) return Type.Num;
                if (targetInfo.IsUnknown) return Type.None;
                if (targetInfo.IsCurrent && targetInfo.GetVariableInfos().Count == 1)
                {
                    var info = targetInfo.GetVariableInfos()[0];
                    if (info.Tag != null) return Type.Tag;
                }

                if (targetInfo.IsAstNameNode)
                {
                    var stream = new AntlrInputStream(targetInfo.CodeText);
                    var lexer = new STGrammarLexer(stream);
                    var token = new CommonTokenStream(lexer);
                    var parser = new STGrammarParser(token);
                    var ast = _stastGenVisitor.Visit(parser.expr());
                    ast.Accept(_typeChecker);
                    var astName = ast as ASTName;
                    if (astName != null)
                    {
                        var info = new VariableInfo(astName, targetInfo.CodeText, _routine,
                            targetInfo.Offset + astName.ContextStart, targetInfo.Parent,
                            _textMarkerService.TextDocument.GetLocationNotVerifyAccess(astName.ContextStart));
                        info.IsDisplay = false;
                        info.LineOffset = GetOffsetInLine(info.Offset, targetInfo.Parent);
                        info.TargetDataType = astName.ExpectDataType;
                        //info.TextLocation = _textMarkerService.TextDocument.GetLocation(info.Offset);
                        VariableInfos.Enqueue(info);
                        GetVariableInfoTag(info);

                        return Type.Tag;
                    }
                }
                else
                {
                    Regex regex = new Regex(@"^(-)?( )*[0-9\.]+( )*");
                    if (regex.IsMatch(targetInfo.CodeText))
                    {
                        return Type.Num;
                    }

                    if (StxCompletionItemsGenerator.Keywords.Contains(targetInfo.CodeText,
                        StringComparer.OrdinalIgnoreCase)
                    ) return Type.Keyword;
                    //var snippets = GetSnippetByPosition.GetWholeSnippets(targetInfo.Parent, CommentList);
                    foreach (var snippetInfo in _snippetInfos)
                    {
                        if (!(snippetInfo.Offset <= targetInfo.Offset &&
                              snippetInfo.EndOffset >= targetInfo.EndOffset)) continue;
                        var trimCode = snippetInfo.CodeText.Trim();
                        var offset = snippetInfo.Parent.IndexOf(trimCode);
                        string snippet = GetSnippetByPosition.GetParserCode(trimCode);
                        regex = new Regex(GetSnippetByPosition.GetIdOrInstrRegex(), RegexOptions.IgnoreCase);
                        var matchCollection = regex.Matches(snippet);
                        var codes = new List<SnippetInfo>();
                        GetVariate(codes, matchCollection, snippetInfo, offset);
                        codes.Reverse();
                        foreach (var codeInfo in codes)
                        {
                            if (codeInfo.Offset <= targetInfo.Offset && codeInfo.EndOffset >= targetInfo.EndOffset)
                            {
                                bool isHead =
                                    codeInfo.CodeText.IndexOf(targetInfo.CodeText,
                                        targetInfo.Offset - codeInfo.Offset) ==
                                    0;
                                var stream = new AntlrInputStream(codeInfo.CodeText);
                                var lexer = new STGrammarLexer(stream);
                                var token = new CommonTokenStream(lexer);
                                var parser = new STGrammarParser(token);
                                var ast = _stastGenVisitor.Visit(parser.expr());
                                if (ast is ASTCall)
                                {
                                    if (isHead) return Type.Instr;
                                    var call = ast as ASTCall;
                                    var paramInfos = GetInstrInfo(codeInfo);
                                    int index = -1;
                                    for (int i = 0; i < paramInfos.Count; i++)
                                    {
                                        if (paramInfos[i].Offset <= targetInfo.Offset &&
                                            paramInfos[i].EndOffset >= targetInfo.EndOffset)
                                        {
                                            targetInfo.CodeText = paramInfos[i].CodeText;
                                            targetInfo.Offset = paramInfos[i].Offset;
                                            index = i;
                                        }
                                    }

                                    if (index == -1) return Type.None;

                                    if (index >= (ast as ASTCall).param_list.nodes.Count) return Type.None;
                                    var targetNode = call.param_list.nodes[index];
                                    if (!(targetNode is ASTName))
                                    {
                                        if (targetNode is ASTCall || targetNode is ASTInstr) return Type.Instr;
                                        if (targetNode is ASTInteger || targetNode is ASTFloat) return Type.Num;
                                        if (targetNode is ASTTag) return Type.Tag;
                                        return Type.None;
                                    }

                                    ConvertInstrEnum(call);
                                    targetNode = call.param_list.nodes[index];
                                    if (targetNode is ASTInteger || targetNode is ASTFloat)
                                    {
                                        var interrelated = "";
                                        if (call.name.Equals("gsv", StringComparison.OrdinalIgnoreCase))
                                        {
                                            var className = ((call.param_list.nodes[0] as ASTInteger)?.value) ?? null;
                                            if (className != null)
                                            {
                                                interrelated = ((InstrEnum.ClassName) (byte) className).ToString();
                                            }
                                        }

                                        if (call.name.Equals("ssv", StringComparison.OrdinalIgnoreCase))
                                        {
                                            var className = ((call.param_list.nodes[0] as ASTInteger)?.value) ?? null;
                                            if (className != null)
                                            {
                                                interrelated = ((InstrEnum.SSVClassName) (byte) className).ToString();
                                            }
                                        }

                                        targetInfo.Enums = GetInstrEnum(call.name, index, interrelated);
                                        return Type.Enum;
                                    }

                                    targetNode.Accept(_typeChecker);
                                    if (((targetNode as ASTName)?.type.IsAxisType ?? false) ||
                                        ((targetNode as ASTName)?.type.IsPredefinedType ?? false) ||
                                        ((targetNode as ASTName)?.type.IsUDIDefinedType ?? false)
                                        || (targetNode as ASTName)?.type is AOIDataType) return Type.Tag;
                                }

                                if (ast is ASTInteger || ast is ASTFloat) return Type.Num;
                                if (ast is ASTTag) return Type.Tag;
                                if (ast is ASTName)
                                {
                                    ast.Accept(_typeChecker);
                                    if ((ast as ASTName).type == null) return Type.None;
                                    if (_parentProgram is AoiDefinition)
                                    {
                                        targetInfo.DataType = (ast as ASTName).type.Name;
                                        foreach (var astNode in (ast as ASTName).loaders.nodes)
                                        {
                                            var node = astNode as ASTStackSlot;
                                            if (node != null && node.no != 0) return Type.InoutParameter;
                                        }
                                    }

                                    return Type.Name;
                                }

                                return Type.None;
                            }
                        }
                    }
                }
                lock (_textMarkerService.LockObject)
                    if (_textMarkerService?.GetMarkersAtOffset(targetInfo.Offset).Count() != 0)
                    {
                        return ObtainValue.NameToTag(targetInfo.CodeText, TransformTable,
                                   _routine.ParentCollection.ParentProgram)?.Item1 != null
                            ? Type.Tag
                            : Type.None;
                    }

                return Type.None;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return Type.None;
            }
        }

        private void FixVariableInfos()
        {
            Parallel.ForEach(VariableInfos, (variableInfo, state) =>
            {
                if (_backgroundWorker.CancellationPending)
                {
                    state.Stop();
                    return;
                }

                GetVariableInfoTag((VariableInfo) variableInfo);
            });
        }

        private void CleanAoiReference()
        {
            if (_isInitial) return;
            foreach (var aoi in _controller.AOIDefinitionCollection)
            {
                if (_backgroundWorker.CancellationPending)
                {

                    return;
                }

                ((AoiDefinition) aoi).CleanReferences(_routine);
            }
        }

        private void ParserByEach(string codeSnippet, bool onlyTextMarker)
        {
            using (var grammar = new GrammarRegex(_textMarkerService,_routine,CommentList,CanAddError,SetErrorLine))
            {
                var parser = grammar.GetParseTree(codeSnippet);
                var astStmtMod = _stastGenVisitor.Visit(parser) as ASTStmtMod;
                GenerateRegionLableError(astStmtMod.RegionLableNodeList, codeSnippet);
                _routine.SetMode(astStmtMod);
                Parallel.ForEach(astStmtMod.list.nodes, (node, state) =>
                {
                    if (_backgroundWorker.CancellationPending)
                    {
                        state.Stop();
                        return;
                    }

                    var snippet = new SnippetInfo(codeSnippet)
                    { CodeText = codeSnippet.Substring(node.ContextStart, node.ContextEnd - node.ContextStart + 1) };
                    _snippetInfos.Enqueue(snippet);

                    try
                    {
                        bool isAccepted = false;
                        ParseNode(snippet, node,null, true, ref isAccepted);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                        DrawFile(node, codeSnippet, e.Message);
                    }
                });

                if (!string.IsNullOrEmpty(astStmtMod.list.Error))
                {
                    DrawFile(astStmtMod.list, codeSnippet, astStmtMod.list.Error);
                }

                if (_backgroundWorker.CancellationPending)
                {
                    return;
                }

                CheckUnexpectedSign(codeSnippet);
                VerifyUnexpectedGrammar(codeSnippet, astStmtMod);
                if (_backgroundWorker.CancellationPending)
                {

                    return;
                }

                var errorLines = GetErrorLine();
                if (!onlyTextMarker)
                {
                    Parallel.ForEach(_snippetInfos, (infos, state) =>
                    {
                        if (_backgroundWorker.CancellationPending)
                        {
                            state.Stop();
                            return;
                        }

                        foreach (var v in infos.GetVariableInfos())
                        {
                            try
                            {
                                var line = _textMarkerService.TextDocument.GetLocationNotVerifyAccess(v.Offset).Line;
                                if (errorLines.Contains(line))
                                {
                                    v.IsDisplay = false;
                                    AddVariableInfo(v);
                                    continue;
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e);
                            }

                            AddVariableInfo(v);
                        }
                    });
                }
            }
        }

        private void CheckUnexpectedSign(string code)
        {
            code = RoutineCodeTextExtension.ConvertCommentToWhiteBlank(code, null);
            var regex = new Regex("[；|（|）]");
            var matches = regex.Matches(code);
            foreach (Match match in matches)
            {
                if (_backgroundWorker.CancellationPending)
                {

                    return;
                }

                DrawFile(match.Index - 1, match.Index,
                    new SnippetInfo(code) {ErrorInfo = "Unexpected grammar,check your code."});
            }
        }

        private void VerifyUnexpectedGrammar(string code, ASTStmtMod astStmtMod)
        {
            try
            {
                if (_backgroundWorker.CancellationPending)
                {

                    return;
                }

                int start = 0,unexpectedCodeStart=0;
                var unexpectedCode = "";
                for (int i = 0; i < astStmtMod.list.nodes.Count; i++)
                {
                    var node = astStmtMod.list.nodes[i];
                    var front = code.Substring(start, node.ContextStart - start);
                    front = RoutineCodeTextExtension.ConvertCommentToWhiteBlank(front, null,false,true);
                    foreach (var c in front)
                    {
                        if (_backgroundWorker.CancellationPending)
                        {

                            return;
                        }

                        if (c != ';' && !char.IsWhiteSpace(c))
                        {
                            if (string.IsNullOrEmpty(unexpectedCode))
                                unexpectedCodeStart = start;
                            unexpectedCode = $"{unexpectedCode}{c}";
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(unexpectedCode))
                            {
                                DrawFile(unexpectedCodeStart, unexpectedCode.Length, code, $"'{unexpectedCode}':Unexpected.");
                                unexpectedCode = "";
                                unexpectedCodeStart = start;
                                break;
                            }
                        }

                        start++;
                    }

                    start = node.ContextEnd + 1;
                    //code = code.Substring(node.ContextEnd + 1);
                }

                var unexpectedCode2 = code.Substring(start).PadLeft(code.Length);
                unexpectedCodeStart = 0;
                var commentList = new List<Tuple<int, int, string>>();
                unexpectedCode2 = RoutineCodeTextExtension.ConvertCommentToWhiteBlank(unexpectedCode2, commentList,false,true);
                for (; start < unexpectedCode2.Length; start++)
                {
                    var c = unexpectedCode2[start];
                    if (_backgroundWorker.CancellationPending)
                    {
                        return;
                    }

                    if (c != ';' && !char.IsWhiteSpace(c))
                    {
                        if (string.IsNullOrEmpty(unexpectedCode))
                            unexpectedCodeStart = start;
                        unexpectedCode = $"{unexpectedCode}{c}";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(unexpectedCode))
                        {
                            DrawFile(unexpectedCodeStart, unexpectedCode.Length, code, $"'{unexpectedCode}':Unexpected.");
                            unexpectedCode = "";
                            unexpectedCodeStart = start;
                            break;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(unexpectedCode))
                {
                    DrawFile(unexpectedCodeStart, unexpectedCode.Length, code, $"'{unexpectedCode}':Unexpected.");
                }
            }
            catch (Exception e)
            {
                if (e is RoutineCodeTextExtension.RemoveCommentException)
                {
                    DrawFile(((RoutineCodeTextExtension.RemoveCommentException)e).Offset, 2, code, ((RoutineCodeTextExtension.RemoveCommentException)e).Error);
                }
            }
        }

        private List<int> GetErrorLine()
        {
            var errorLines = new List<int>();
            foreach (var range in _errorRange)
            {
                try
                {
                    var startLine = _textMarkerService.TextDocument.GetLocationNotVerifyAccess(range.Item1).Line;
                    var endLine = _textMarkerService.TextDocument.GetLocationNotVerifyAccess(range.Item2).Line;
                    for (int i = startLine; i <= endLine; i++)
                    {
                        if (!errorLines.Contains(i)) errorLines.Add(i);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }

            return errorLines;
        }


        public double GetOffsetInLine(int offset, string snippetCode)
        {
            double width = 0;
            for (int i = offset - 1; i > -1; i--)
            {
                if (snippetCode[i] == '\r' || snippetCode[i] == '\n') break;
                if (snippetCode[i] == '\t')
                {
                    width = width + (30 / (40.0 / 3.0));
                }
                else
                {
                    width = width + 8 / (40.0 / 3.0);
                }
            }

            return width;
        }

        private List<SnippetInfo> GetInstrInfo(SnippetInfo codeInfo)
        {
            string code = codeInfo.CodeText;

            int lParen = code.IndexOf("(");
            int rParen = code.LastIndexOf(")");
            code = code.Substring(lParen + 1, rParen - 1 - lParen);
            List<SnippetInfo> paramInfos = new List<SnippetInfo>();
            int parenCount = 0;
            string param = "";
            int offset = 0;
            for (int i = 0; i < code.Length; i++)
            {
                if (code[i] == ',')
                {
                    if (parenCount == 0)
                    {
                        SnippetInfo info = new SnippetInfo(codeInfo.Parent)
                            {CodeText = param, Offset = lParen + offset + 1 + codeInfo.Offset};
                        paramInfos.Add(info);
                        param = "";
                        offset = i + 1;
                        continue;
                    }
                }

                if (code[i] == '(' || code[i] == '[')
                {
                    parenCount++;
                }

                if (code[i] == ')' || code[i] == ']') parenCount--;
                param = param + code[i];
            }

            ;
            paramInfos.Add(new SnippetInfo(codeInfo.Parent)
                {CodeText = param, Offset = lParen + offset + 1 + codeInfo.Offset});
            return paramInfos;
        }

        private void DrawFile(int start, int end, SnippetInfo snippetInfo)
        {
            lock (_textMarkerService.LockObject)
            {
                _routine.IsError = true;
                if (start > end)
                {
                    start = start ^ end;
                    end = start ^ end;
                    start = start ^ start;
                }

                SetErrorLine(start, end);
                string codeSnippet = snippetInfo.Parent;
                snippetInfo.IsCurrent = false;
                _textMarkerService.CreateSkinCommentAndBlankLine(start, end - start + 1, codeSnippet, Colors.LimeGreen,
                    CommentList, CanAddError, snippetInfo.ErrorInfo);
            }
        }

        private List<Tuple<int,int,string>> _addErrors=new List<Tuple<int,int, string>>();

        private void DrawFile(ASTNode node, string codeSnippet, string error, Color? color = null)
        {
            if (node == null||string.IsNullOrEmpty(error))
            {
                return;
            }

            lock (_textMarkerService.LockObject)
            {
                if(node.IsMarked)return;
                _routine.IsError = true;
                node.IsMarked = true;
                var start = node.ContextStart;
                var end = node.ContextEnd;
                bool isInError = node.ErrorEnd >= node.ErrorStart && (!(node.ErrorStart == node.ErrorEnd && node.ErrorStart <= 0 && node.ErrorEnd <= 0));
                
                if (isInError)
                {
                    start = node.ErrorStart;
                    end = node.ErrorEnd;
                }

                if (start > end)
                {
                    start = start ^ end;
                    end = start ^ end;
                    start = start ^ start;
                }

                if (_addErrors.FirstOrDefault(item =>
                        item.Item1 == start && item.Item2 == end && item.Item3.Equals(error)) != null)
                    return;
                    SetErrorLine(start, end);
                _textMarkerService.CreateSkinCommentAndBlankLine(start, end - start + 1,
                    codeSnippet, isInError ? Colors.Red : color ?? Colors.LimeGreen,
                    CommentList, CanAddError, error);
            }
        }

        private void DrawFile(int start, int len, string codeSnippet, string error, Color? color = null)
        {
            lock (_textMarkerService.LockObject)
            {
                if (_addErrors.FirstOrDefault(item =>
                        item.Item1 == start && item.Item2 == len+start-1 && item.Item3.Equals(error)) != null)
                    return;
                _routine.IsError = true;

                SetErrorLine(start, start + len - 1);
                _textMarkerService.CreateSkinCommentAndBlankLine(start, len,
                    codeSnippet, color ?? Colors.LimeGreen,
                    CommentList, CanAddError, error);
            }
        }

        private void GenerateRegionLableError(ASTNodeList regionLableNodes, string code)
        {
            if (regionLableNodes?.Count() <= 0) return;
            const string startLable = "#region";
            const string endLable = "#endregion";

            var nodeList = new List<ASTNode>(regionLableNodes.nodes);
            nodeList.Sort((x, y) => x.ContextStart - y.ContextStart);

            Stack<ASTRegionLable> regionStack = new Stack<ASTRegionLable>();
            foreach (var node in nodeList)
            {
                ASTRegionLable lable = node as ASTRegionLable;
                if (lable == null)
                {
                    continue;
                }
                if (lable.LableType != RegionLableType.Region &&
                    lable.LableType != RegionLableType.Endregion)
                {
                    continue;
                }
                else if (!IsFirstNonWhiteSpaceWordOfItsLine(lable, code))
                {
                    DrawFile(
                        lable.ContextStart, lable.KeywordLength, code,
                        $"'{startLable}' or '{endLable}' directive must appear as the first non-whitespace word on a line.");
                }
                else if (lable.LableType == RegionLableType.Region)
                {
                    regionStack.Push(lable);
                }
                else if (lable.LableType == RegionLableType.Endregion)
                {
                    if (regionStack.Count <= 0)
                    {
                        DrawFile(
                            lable.ContextStart, lable.KeywordLength, code,
                            $"'{endLable}' directive is missing its '{startLable}' directive.");
                    }
                    else
                    {
                        regionStack.Pop();
                    }
                }
            }
            if (regionStack.Any())
            {
                foreach (var lable in regionStack)
                {
                    DrawFile(
                        lable.ContextStart, lable.KeywordLength, code,
                        $"'{startLable}' directive is missing its '{endLable}' directive.");
                }
            }
        }

        private bool IsFirstNonWhiteSpaceWordOfItsLine(ASTRegionLable lable, string code)
        {
            int currentIndex = lable.ContextStart;
            while (--currentIndex >= 0)
            {
                if (code[currentIndex] == '\n')
                {
                    return true;
                }
                else if (char.IsWhiteSpace(code[currentIndex]))
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private void SetErrorLine(int start, int end)
        {
            var range = new Tuple<int, int>(start, end);
            if (!_errorRange.Contains(range)) _errorRange.Add(range);
        }

        private bool MatchInteger(MatchCollection matchCollection, int offset)
        {
            foreach (Match match in matchCollection)
            {
                if (match.Index <= offset && match.Index + match.Length >= offset) return true;
            }

            return false;
        }

        private void GetVariate(List<SnippetInfo> codes, MatchCollection matchCollection, SnippetInfo parentLine,
            int offset)
        {
            var nIntegerMatches =
                (new Regex(
                    @"\b([0-9]+\.[0-9]+[Ee](\+|\-)?[0-9]+|2#[01]+|8#[0-7]+|16#[0-9A-Fa-f]+)"))
                .Matches(parentLine.CodeText);

            foreach (Match match in matchCollection)
            {
                if (MatchInteger(nIntegerMatches, match.Index)) continue;
                var name = "";
                if (IsKeyword(match.Value) || GetSnippetByPosition.IsInstr(match.Value, ref name))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(match.Value) &&
                    !_keywords.Contains(match.Value, StringComparer.OrdinalIgnoreCase))
                {
                    var code = match.Value;
                    if (code.EndsWith(":"))
                        code = code.Substring(0, code.Length - 1);
                    var info = new SnippetInfo(parentLine.Parent)
                        {CodeText = code, Offset = match.Index + parentLine.Offset + offset};
                    codes.Add(info);
                    if (match.Value.IndexOf("[") > 0)
                    {
                        int left = match.Value.IndexOf("[") + 1;
                        int right = match.Value.LastIndexOf("]") - 1;
                        var dims = match.Value.Substring(left, right - left + 1);
                        var regex = new Regex(GetSnippetByPosition.GetIdOrInstrRegex(), RegexOptions.IgnoreCase);
                        GetDimVariate(codes, regex.Matches(dims), match.Index + parentLine.Offset + left,
                            parentLine.Parent, match.Value, info);
                    }
                }
            }
        }

        internal bool IsKeyword(string code)
        {
            foreach (var keyword in StxCompletionItemsGenerator.Keywords)
            {
                if (keyword.Equals(code, StringComparison.OrdinalIgnoreCase)) return true;
            }

            return false;
        }

        private void GetDimVariate(List<SnippetInfo> codes, MatchCollection matchCollection, int start, string parent,
            string matchString, SnippetInfo parentCode)
        {
            foreach (Match match in matchCollection)
            {
                if (!string.IsNullOrEmpty(match.Value) &&
                    !_keywords.Contains(match.Value, StringComparer.OrdinalIgnoreCase))
                {
                    if (IsInDims(match.Index + start, matchString))
                    {
                        codes.Add(new SnippetInfo(parent)
                            {CodeText = match.Value, Offset = match.Index + start, IsDisplay = false});
                        parentCode.IsEnable = false;
                        if (match.Value.IndexOf("[") > 0)
                        {
                            int left = match.Value.IndexOf("[") + 1;
                            int right = match.Value.LastIndexOf("]") - 1;
                            var dims = match.Value.Substring(left, right - left + 1);
                            var regex = new Regex(GetSnippetByPosition.GetIdOrInstrRegex(), RegexOptions.IgnoreCase);
                            GetDimVariate(codes, regex.Matches(dims), match.Index + start + left, parent, match.Value,
                                parentCode);
                        }
                    }
                }
            }
        }

        private bool IsInDims(int offset, string str)
        {
            for (int i = offset + 1; i < str.Length; i++)
            {
                if (str[i] == '[') return false;
                if (str[i] == ']') return true;
            }

            return false;
        }

        public static void ConvertInstrEnum(ASTCall call)
        {
            if (call.name.Equals("MAM", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.MAM.ParseSTParameters(call.param_list);
            else if (call.name.Equals("SFP", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.SFP.ParseSTParameters(call.param_list);
            else if (call.name.Equals("PXRQ", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.PXRQ.ParseSTParameters(call.param_list);
            else if (call.name.Equals("POVR", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.POVR.ParseSTParameters(call.param_list);
            else if (call.name.Equals("SCMD", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.SCMD.ParseSTParameters(call.param_list);
            else if (call.name.Equals("SOVR", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.SOVR.ParseSTParameters(call.param_list);
            else if (call.name.Equals("GSV", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.GSV.ParseSTParameters(call.param_list);
            else if (call.name.Equals("SSV", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.SSV.ParseSTParameters(call.param_list);
            else if (call.name.Equals("MDO", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.MDO.ParseSTParameters(call.param_list);
            else if (call.name.Equals("MDS", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.MDS.ParseSTParameters(call.param_list);
            else if (call.name.Equals("MAS", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.MAS.ParseSTParameters(call.param_list);
            else if (call.name.Equals("MAJ", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.MAJ.ParseSTParameters(call.param_list);
            else if (call.name.Equals("MAG", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.MAG.ParseSTParameters(call.param_list);
            else if (call.name.Equals("MCD", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.MCD.ParseSTParameters(call.param_list);
            else if (call.name.Equals("MRP", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.MRP.ParseSTParameters(call.param_list);
            else if (call.name.Equals("MAPC", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.MAPC.ParseSTParameters(call.param_list);
            else if (call.name.Equals("MATC", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.MATC.ParseSTParameters(call.param_list);
            else if (call.name.Equals("MDAC", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.MDAC.ParseSTParameters(call.param_list);
            else if (call.name.Equals("MGS", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.MGS.ParseSTParameters(call.param_list);
            else if (call.name.Equals("MAW", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.MAW.ParseSTParameters(call.param_list);
            else if (call.name.Equals("MAR", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.MAR.ParseSTParameters(call.param_list);
            else if (call.name.Equals("MAOC", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.MAOC.ParseSTParameters(call.param_list);
            else if (call.name.Equals("MDOC", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.MDOC.ParseSTParameters(call.param_list);
            else if (call.name.Equals("MAHD", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.MAHD.ParseSTParameters(call.param_list);
            else if (call.name.Equals("MRHD", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.MRHD.ParseSTParameters(call.param_list);
            else if (call.name.Equals("MCS", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.MCS.ParseSTParameters(call.param_list);
            else if (call.name.Equals("MCLM", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.MCLM.ParseSTParameters(call.param_list);
            else if (call.name.Equals("MCCM", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.MCCM.ParseSTParameters(call.param_list);
            else if (call.name.Equals("MCCD", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.MCCD.ParseSTParameters(call.param_list);
            else if (call.name.Equals("MCTP", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.MCTP.ParseSTParameters(call.param_list);
            else if (call.name.Equals("MDCC", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.MDCC.ParseSTParameters(call.param_list);
            else if (call.name.Equals("SWPB", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.SWPB.ParseSTParameters(call.param_list);
            else if (call.name.Equals("MDR", StringComparison.OrdinalIgnoreCase))
                call.param_list = AllFInstructions.MDR.ParseSTParameters(call.param_list);
        }

        public static Tuple<List<string>, System.Type> GetInstrEnumInfo(string instr, int pos, string interrelated)
        {
            if (string.IsNullOrEmpty(instr))
                return null;

            if (instr.Equals("MAM", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAM.GetInstrEnumType(pos);
            else if (instr.Equals("SFP", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.SFP.GetInstrEnumType(pos);
            else if (instr.Equals("PXRQ", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.PXRQ.GetInstrEnumType(pos);
            else if (instr.Equals("POVR", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.POVR.GetInstrEnumType(pos);
            else if (instr.Equals("SCMD", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.SCMD.GetInstrEnumType(pos);
            else if (instr.Equals("SOVR", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.SOVR.GetInstrEnumType(pos);
            else if (instr.Equals("GSV", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.GSV.GetInstrEnumType(pos, interrelated);
            else if (instr.Equals("SSV", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.SSV.GetInstrEnumType(pos, interrelated);
            else if (instr.Equals("MDO", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MDO.GetInstrEnumType(pos);
            else if (instr.Equals("MDS", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MDS.GetInstrEnumType(pos);
            else if (instr.Equals("MAS", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAS.GetInstrEnumType(pos);
            else if (instr.Equals("MAJ", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAJ.GetInstrEnumType(pos);
            else if (instr.Equals("MAG", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAG.GetInstrEnumType(pos);
            else if (instr.Equals("MCD", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MCD.GetInstrEnumType(pos);
            else if (instr.Equals("MRP", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MRP.GetInstrEnumType(pos);
            else if (instr.Equals("MAPC", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAPC.GetInstrEnumType(pos);
            else if (instr.Equals("MATC", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MATC.GetInstrEnumType(pos);
            else if (instr.Equals("MDAC", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MDAC.GetInstrEnumType(pos);
            else if (instr.Equals("MGS", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MGS.GetInstrEnumType(pos);
            else if (instr.Equals("MAW", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAW.GetInstrEnumType(pos);
            else if (instr.Equals("MAR", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAR.GetInstrEnumType(pos);
            else if (instr.Equals("MAOC", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAOC.GetInstrEnumType(pos);
            else if (instr.Equals("MDOC", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MDOC.GetInstrEnumType(pos);
            else if (instr.Equals("MAHD", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAHD.GetInstrEnumType(pos);
            else if (instr.Equals("MRHD", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MRHD.GetInstrEnumType(pos);
            else if (instr.Equals("MCS", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MCS.GetInstrEnumType(pos);
            else if (instr.Equals("MCLM", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MCLM.GetInstrEnumType(pos);
            else if (instr.Equals("MCCM", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MCCM.GetInstrEnumType(pos);
            else if (instr.Equals("MCCD", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MCCD.GetInstrEnumType(pos);
            else if (instr.Equals("MCTP", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MCTP.GetInstrEnumType(pos);
            else if (instr.Equals("MDCC", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MDCC.GetInstrEnumType(pos);
            else if (instr.Equals("SWPB", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.SWPB.GetInstrEnumType(pos);
            else if (instr.Equals("MDR", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MDR.GetInstrEnumType(pos);
            return null;
        }

        public static List<string> GetInstrEnum(string instr, int pos, string interrelated)
        {
            if (string.IsNullOrEmpty(instr))
                return null;
            List<string> enums = GetInstrEnumInfo(instr, pos, interrelated)?.Item1;
            if (enums != null)
                for (int i = 0; i < enums.Count; i++)
                {
                    enums[i] = enums[i].Replace(" ", "");
                }

            return enums;
        }

        private void GetVariableInfoTag(VariableInfo variableInfo)
        {
            if (variableInfo.IsModule)
            {
                variableInfo.Module = Controller.GetInstance().DeviceModules[variableInfo.Name];
                return;
            }
            if (!variableInfo.IsInstr && !variableInfo.IsNum&&!variableInfo.IsEnum)
            {
                {
                    var tag = (variableInfo.AstNode as ASTName)?.Tag;
                    if (tag != null)
                    {
                        variableInfo.Tag = tag;
                        return;
                    }
                }
                var nameList = SplitCodeSnippet(variableInfo.Name, ".");
                string baseName = nameList[0];
                if (baseName.IndexOf("[") > 0)
                {
                    baseName = baseName.Substring(0, baseName.IndexOf("["));
                }

                if (baseName.IndexOf("\\") == 0)
                {
                    var otherProgram = _controller.Programs[baseName.Substring(1)];
                    if (!(otherProgram != null && nameList.Count > 1)) return;
                    baseName = nameList[1];
                    if (baseName.IndexOf("[") > 0)
                    {
                        baseName = baseName.Substring(0, baseName.IndexOf("["));
                    }

                    foreach (var programTag in otherProgram.Tags)
                    {
                        var index = variableInfo.Name.IndexOf(programTag.Name, variableInfo.Name.IndexOf("."),
                                        StringComparison.OrdinalIgnoreCase) -
                                    nameList[0].Length - 1;
                        if (index == 0 && baseName.Equals(programTag.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            variableInfo.Tag = programTag as Tag;
                            return;
                        }
                    }
                }

                if (_parentProgram is AoiDefinition)
                {
                    foreach (var aoiTag in _parentProgram.Tags)
                    {
                        var tag = aoiTag as Tag;
                        var index = variableInfo.Name.IndexOf(tag.Name, StringComparison.OrdinalIgnoreCase);
                        if (index == 0 && baseName.Equals(tag.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            variableInfo.Tag = tag;
                            return;
                        }
                    }
                }
                else
                {
                    foreach (var programTag in _parentProgram.Tags)
                    {
                        var tag = programTag as Tag;
                        var index = variableInfo.Name.IndexOf(tag.Name, StringComparison.OrdinalIgnoreCase);
                        if (index == 0 && baseName.Equals(tag.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            variableInfo.Tag = tag;
                            return;
                        }
                    }

                    foreach (var controllerTag in _controller.Tags)
                    {
                        var tag = controllerTag as Tag;
                        var index = variableInfo.Name.IndexOf(tag.Name, StringComparison.OrdinalIgnoreCase);
                        if (index == 0 && baseName.Equals(tag.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            variableInfo.Tag = tag;
                            return;
                        }
                    }
                }

                //throw new StxEditorException("ComponentContentProvider:GetVariableInfoTag:can not find tag.");
            }
        }

        private List<string> SplitCodeSnippet(string snippet, string separator)
        {
            List<string> subCodeList = new List<string>();
            int leftBracketCount = 0;
            int p = 0;
            string code = "";
            while (p < snippet.Length)
            {
                string letter = snippet.Substring(p, 1);
                if (letter == "[") leftBracketCount++;
                if (letter == "]") leftBracketCount--;
                if (letter == separator && leftBracketCount == 0)
                {
                    subCodeList.Add(code.Trim());
                    code = "";
                    p++;
                    continue;
                }

                code = code + letter;
                p++;
            }

            subCodeList.Add(code.Trim());
            return subCodeList;
        }

        private List<StxCompletionItemCodeSnippetData> GenerateCodeSnippetItems()
        {
            var dataList = new List<StxCompletionItemCodeSnippetData>();
            //{
            //    new StxCompletionItemCodeSnippetData("#region", ""),
            //    new StxCompletionItemCodeSnippetData("case",
            //        "" +
            //        "Use CASE...OF to select what to do based on a numerical value" +
            //        ""),
            //    new StxCompletionItemCodeSnippetData("elsif",
            //        "Use ELSIF...THEN to do something if or when specific conditions occur"),
            //    new StxCompletionItemCodeSnippetData("for",
            //        "Use the FOR...DO loop to do something a specific number of times before doing anything else"),
            //    new StxCompletionItemCodeSnippetData("if",
            //        "Use IF...THEN to do something if or when specific conditions occur"),
            //    new StxCompletionItemCodeSnippetData("repeat",
            //        "Use the REPEAT...UNTIL loop to keep doing something until conditions are true"),
            //    new StxCompletionItemCodeSnippetData("while",
            //        "Use the WHILE...DO loop to keep doing something as long as certain conditions are true"),
            //    new StxCompletionItemCodeSnippetData("then", ""),
            //    new StxCompletionItemCodeSnippetData("until", "")
            //};

            #region Add Funciton keywords

            dataList.Add(new StxCompletionItemCodeSnippetData("ABS", "Absolute Value", "Source", "Function"));
            dataList.Add(new StxCompletionItemCodeSnippetData("ACOS", "Arccosine", "Source", "Function"));
            dataList.Add(new StxCompletionItemCodeSnippetData("ALM", "Alarm", "ALMTag", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("ALMA", "Analog Alarm",
                "ALMA,In,ProgAckAll,ProDisable,ProgEnable", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("ALMD", "Digital",
                "ALMD,In,ProgAck,ProgReset,ProgDisable,ProgEnable", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("ASIN", "Arcsine", "Source", "Function"));
            dataList.Add(new StxCompletionItemCodeSnippetData("ATAN", "Arctangent", "Source", "Function"));

            dataList.Add(new StxCompletionItemCodeSnippetData("BTDT", "Bit Field Distribute with Target", "BTDTTag",
                "Instruction"));

            dataList.Add(new StxCompletionItemCodeSnippetData("CC", "Coordinated Control", "CCTag", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("CONCAT", "String Concatenate", "Source A,Source B,Dest",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("COP", "Copy File", "Source,Dest,Length", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("COS", "Cosine", "Source", "Function"));
            dataList.Add(new StxCompletionItemCodeSnippetData("CPS", "Synchronous Copy File", "Source,Dest,Length",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("CTUD", "Count Up/Down", "CTUDTag", "Instruction"));

            dataList.Add(new StxCompletionItemCodeSnippetData("D2SD", "Discrete 2-State Device", "D2SDTag",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("D3SD", "Discrete 3-State Device", "D3SDTag",
                "Instruction"));
            dataList.Add(
                new StxCompletionItemCodeSnippetData("DEDT", "Deadtime", "DEDTTag,StorageArray", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("DEG", "Radians To Degrees", "Source", "Function"));
            dataList.Add(new StxCompletionItemCodeSnippetData("DELETE", "String Delete", "Source,Qty,Start,Dest",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("DERV", "Derivate", "DERVTag", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("DFF", "D Flip Flop", "DFFTag", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("DTOS", "DINT to String", "Source,Dest", "Instruction"));

            dataList.Add(new StxCompletionItemCodeSnippetData("EOT", "End Of Transition", "State Bit", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("ESEL", "Enhanced Select", "ESELTag", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("EVENT", "Triggrt Event Task", "Task", "Instruction"));

            dataList.Add(new StxCompletionItemCodeSnippetData("FGEN", "Function Generator", "FGENTag,X1,Y1,X2,Y2",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("FIND", "Find String", "Source,Search,Start,Result",
                "Instruction"));

            dataList.Add(new StxCompletionItemCodeSnippetData("HLL", "High/Low Limit", "HLLTag", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("HMIBC", "HMI Button Control", "HMIBC", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("HPF", "High-Pass-Filter", "HPFTag", "Instruction"));

            dataList.Add(
                new StxCompletionItemCodeSnippetData("IMC", "Internal Model Control", "IMCTag", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("INSERT", "Insert String", "Source A,Source B,Start,Dest",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("INTG", "Integrator", "INTGTag", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("IOT", "Immediate", "Update Tag", "Instruction"));

            dataList.Add(new StxCompletionItemCodeSnippetData("JKFF", "JK Flip Flop", "JKFFTag", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("JSR", "Jump To Subroutine",
                "Routine Name, Number of Inputs[,Input Par][,Return Par]", "Instruction"));

            dataList.Add(
                new StxCompletionItemCodeSnippetData("LDL2", "Second-Order Lead-Lag", "LDL2Tag", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("LDLG", "Lead-Lag", "LDLGTag", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("LN", "Nature Log", "Source", "Function"));
            dataList.Add(new StxCompletionItemCodeSnippetData("LOG", "Log Base  10", "Source", "Function"));
            dataList.Add(new StxCompletionItemCodeSnippetData("LOWER", "Lower Case", "Source,Dest", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("LPF", "Low-Pass Filter", "LPFTag", "Instruction"));

            dataList.Add(new StxCompletionItemCodeSnippetData("MAAT", "Motion Apply Axis Tuning", "Axis,Motion Control",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MAFR", "Motion Axis Fault Reset", "Axis,Motion Control",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MAG", "Motion Axis Gear",
                "Slave Axis,Master Axis,Motion Control,Direction,Ratio,Slave Counts,Master Counts,Master Reference,Ratio Format,Clutch,Accel Rate,Accel Units",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MAH", "Motion Axis Home", "Axis,Motion Control",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MAHD", "Motion Apply Hookup Diagnostics",
                "Axis,Motion Control,Diagnostic Test,Observed Direction", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MAJ", "Motion Axis Jog",
                "Axis,Motion Control,Direction,Speed,Speed Units,Accel Rate,Accel Units,Decel Rate,Decel Units,Profile,Accel Jerk,Decel Jerk,Jerk Units,Merge,Merge Speed,Lock Position,Lock Direction",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MAM", "Motion Axis Move",
                "Axis,Motion Control,Move Type,Position,Speed,Speed Units,Accel Rate,Accel Units,Decel Rate,Decel Units,Profile,Accel Jerk,Decel Jerk,Jerk Units,Merge,Merge Speed,Lock Position,Lock Direction,Event Distance,Calculated Data",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MAOC", "Motion Arm Output Cam",
                "Axis,Execution Target,Motion Control,Output,Input,Output Cam,Cam Start Position,Cam End Position,Output Compensation,Execution Mode,Execution Schedule,Axis Arm position,Cam Arm Position,Cam Arm Position,Cam Arm Position,Position Reference",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MAPC", "Motion Axis Position Cam",
                "Slave Axis,Master Axis,Motion Control,Direction,Cam Profile,Slave Scaling,Master Scaling,Execution Mode,Execution Schedule,Master Lock Position,Cam Lock Position ,Master Reference,Master Direction",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MAR", "Motion Arm Registration",
                "Axis Control,Trigger Condition,Windowed Registration,Min.Position,Max.Position,Input Number",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MAS", "Motion Axis Stop",
                "Axis,Motion Control,Stop Type,Change Decel,Decel Rate, Decel Units,Change Decel Jerk,Decel Jerk,Jerk Units",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MASD", "Motion Axis Shutdown", "Axis,Motion Control",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MASR", "Motion Axis Shutdown Reset",
                "Axis,Motion Control", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MATC", "Motion Axis Time Cam",
                "Axis,Motion Control,Direction,Cam Profile,Distance Scaling,Time Scaling,Execution Mode,Execution Schedule,Lock Position,Lock Direction,Instruction Mode",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MAVE", "Move Average",
                "MAVETag,StorageArray,WeightArray", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MAW", "Motion Arm Watch",
                "Axis,Motion Control,Trigger Condition,Position", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MAXC", "Maximum Capture", "MAXCTag", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MCCD", "Motion Coordinated Change Dynamics",
                "Coordinate System,Motion Control,Motion Type,Change Speed,Speed,Speed Units,Change Accel,Accel Rate,Accel Units,Change Decel,Decel Rate,Decel Units,Change Accel Jerk,Accel Jerk,Change Decel Jerk,Decel Jerk,Jerk Units,Scope",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MCCM", "Motion Coordinated Circular Move",
                "Coordinate System,Motion Control,Move Type,Position,Circle Type,Via/Center/Radius,Direction,Speed,Speed Untis,Accel Rate,Accel Units,Decel Rate,Decel Units,Profile,Accel Jerk,Decel Jerk,Jerk Units,Termination Type,Merge,Merge Speed,Command Tolerance,Lock Position,Lock Direction,Event Distance,Calculated Data",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MCCP", "Motion Calculate Cam Profile",
                "Motion Control,Cam,Length,Start Slope,End Slope,CamProfile", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MCD", "Motion Change Dynamics",
                "Axis,Motion Control,Motion Type,Change Speed,Speed,Change Accel,Accel Rate,Change Decel Rate,Change Decel,Decel Rate,Change Accel Jerk,Accel Jerk,Change Decel Jerk,Decel Jerk,Speed Units,Accel Units,Decel Units,Jerk Units",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MCLM", "Motion Coordinated Linear Move",
                "Coordinate System,Motion Control,Move Type,Position,Speed,Speed Units,Accel Units,Decel Rate,Decel Units,Profile,Accel Jerk,Decel Jerk,Jerk Units,Termination Type,Merge,Merge Speed,Command Tolerance,Lock Position,Lock Direction,Event Distance,Calculated Data",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MCPM", "Motion Coordinated Path Move",
                "Coordinate System,Motion Control,Path,Length,Dynamics,Lock Position,Lock Direction", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MCS", "Motion Coordinated Stop",
                "Coordinate System,Motion Control,Stop Type,Change Decel,Decel Rate,Decel Units,Change Decel Jerk,Jerk Units",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MCSD", "Motion Coordinated Shutdown",
                "Coordinate System,Motion Control", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MCSR", "Motion Coordinated Shutdown Reset",
                "Coordinate System,Motion Control", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MCSV", "Motion Calculate Slave Values",
                "Motion Control,Cam Profile,Master Value,Slave Value,Slop Value,Slope Derivative", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MCT", "Motion Coordinated Transform",
                "Source System,Target System,Motion Control,Orientation,Translation", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MCTO", "Motion Coordinated Transform with Orientation",
                "Cartesian System,Robot System,Motion Control,Work Frame,Tool Frame", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MCTP", "Motion Calculate Transform Position",
                "Source System,Target System,Motion Control,Orientation,Translation,Transform,Transform Direction,Reference Position,Transform Position",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MCTPO",
                "Motion Calculate Transform Position with Orientation",
                "Cartesian System,Robot System,Motion Control,Work Frame,Tool Frame,Transform Direction,Reference Position,Transform Position,Transform Position,Robot Configuration,Turns Counter",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MDAC", "Motion Master Driven Axis Control",
                "Slave Axis,Master Axis,Motion Control,Motion Type,Master Reference", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MDCC", "Motion Master Driven Coordinated Control",
                "Slave System,Master Axis,Motion Control,Master Reference,Nominal Master Velocity", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MDF", "Motion Direct Drive Off", "Axis,Motion Control",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MDO", "Motion Direct Drive On",
                "Axis,Motion Control,Driver Output,Drive Units", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MDOC", "Motion Disarm Output Cam",
                "Axis,Execution Target,Motion Control,Disarm Type", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MDR", "Motion Disarm Registration",
                "Axis,Motion Control,Input Number", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MDS", "Motion Drive Start",
                "Axis,Motion Control,Speed,Speed Units", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MDW", "Motion Disarm Watch", "Axis,Motion Control",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MGS", "Motion Group Stop",
                "Group,Motion Control,Stop Model", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MGSD", "Motion Group Shutdown", "Group,Motion Control",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MGSP", "Motion Group Strobe Position",
                "Group,Motion Control", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MGSR", "Motion Group Shutdown Reset",
                "Group,Motion Control", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MID", "Middle String", "Source,Qty,Start,Dest",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MINC", "Minimum Capture", "MINCTag", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MMC", "Modular Multivariable Control", "MMCTag",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MRAT", "Motion Run Axis Tuning", "Axis,Motion Control",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MRHD", "Motion Run Hookup Diagnostics",
                "Axis,Motion Control,Diagnostic Test", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MRP", "Motion Redefine Position",
                "Axis,Motion Control,Type,Position Select,Position", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MSF", "Motion Servo Off", "Axis,Motion Control",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MSG", "Message", "Message Control", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MSO", "Motion Servo On", "Axis,Motion Control",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MSTD", "Moving Standard Deviation",
                "MSTDTag,StorageArray", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("MVMT", "Masked Move with Target", "MVMTTag",
                "Instruction"));

            dataList.Add(new StxCompletionItemCodeSnippetData("NTCH", "Notch Filter", "NTCHTag", "Instruction"));

            dataList.Add(new StxCompletionItemCodeSnippetData("OSFI", "One Shot Falling with Input", "OSFITag",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("OSRI", "One Shot Rising with Input", "OSRITag",
                "Instruction"));

            dataList.Add(new StxCompletionItemCodeSnippetData("PATT", "Attach to Equipment Phase", "Phase Name,Result",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("PCLF", "Equipment Phase Clear Failure", "Phase Name",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("PCMD", "Equipment Phase Command",
                "Phase Name,Command,Result", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("PDET", "Detach from Equipment Phase", "Phase Name",
                "Instruction"));
            dataList.Add(
                new StxCompletionItemCodeSnippetData("PFL", "Equipment Phase Failure", "Source", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("PI", "Proportional+Integral", "PITag", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("PID", "Proportional Integral Derivative",
                "PID,Process Variable,Tieback,Control Variable,PID Master Loop,Inhold Bit,Inhold Value",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("PIDE", "Enhanced PID", "PIDETag", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("PMUL", "Pulse Multiplier", "PMULTag", "Instruction"));
            dataList.Add(
                new StxCompletionItemCodeSnippetData("POSP", "Position Proportional", "POSPTag", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("POVR", "Equipment Phase Override Command",
                "Phase Name,Command ,Result", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("PPD", "Equipment Phase Pause", "", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("PRNP", "Equipment Phase New Parameters", "",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("PXRQ", "Equipment Phase External Request",
                "Phase Instruction,External Request,Data Value", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("PSC", "Equipment Phase State Complete", "",
                "Instruction"));

            dataList.Add(new StxCompletionItemCodeSnippetData("RAD", "Degrees To Radians", "Source", "Function"));
            dataList.Add(new StxCompletionItemCodeSnippetData("RESD", "Reset Dominant", "RESDTag", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("RET", "Return from Subroutine", "[Return Par]",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("RLIM", "Rate Limiter", "RLIMTag", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("RMPS", "Ramp/Soak",
                "RMPSTAg,RampValue,SoakValue,SoakTime", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("RTOR", "Retentive Timer On with Reset", "RTORTag",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("RTOS", "Real to String", "Source,Dest", "Instruction"));

            dataList.Add(new StxCompletionItemCodeSnippetData("SASI", "Sequence Assign Sequence Id",
                "Sequence Name,Sequence Id,Result", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("SATT", "Attach to Sequence", "Sequence Name,Result",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("SBR", "Subroutine", "[Input Par]", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("SCL", "Scale", "SCLTag", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("SCLF", "Sequence Clear Failure", "Sequence Name,Result",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("SCMD", "Sequence Command",
                "Sequence Name,Command,Result", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("SCRV", "S-Curve", "SCRVTag", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("SDET", "Detach from Sequence", "Sequence Name",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("SETD", "Set Dominant", "SETDTag", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("SFP", "SFC Pause", "SFC Routine Name,Target State",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("SFR", "SFC Reset", "SFC Routine Name,Step Name",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("SIN", "Sine", "Source", "Function"));
            dataList.Add(new StxCompletionItemCodeSnippetData("SIZE", "Size in Elements", "Source,Dim,To Vary,Size",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("SNEG", "Selectable Negate", "SNEGTag", "Instruction"));
            dataList.Add(
                new StxCompletionItemCodeSnippetData("SOC", "Second-Order-Controller", "SOCTag", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("SOVR", "Sequence Override Command",
                "Sequence Name,Command,Result", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("SQRT", "Square Root", "Source", "Function"));
            dataList.Add(new StxCompletionItemCodeSnippetData("SRTP", "Split Range Time Proportional", "SRTPTag",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("SRT", "Sort File", "Array,Dim,To Vary,Control",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("SSUM", "Selected Summer", "SSUMTag", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("SSV", "Set System Value",
                "Class Name,Instance Name,Attribute Name,Source", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("STOD", "String To DINT", "Source,Dest", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("STOR", "String to Real", "Source,Dest", "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("SWPB", "Swap Byte", "Source,Order Mode,Dest",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("TONR", "Timer On Delay with Reset", "TONRTag",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("TOFR", "Timer Off Delay with Reset", "TOFRTag",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("UPPER", "Upper Case", "Source,Dest",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("LOWER", "Lower Case", "Source,Dest",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("UPDN", "Up/Down Accumulator", "UPDNTag",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("GSV", "Get System Value",
                "Class Name,Instance Name,Attribute Name,Dest",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("TRUNC", "Truncate", "Source",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("TND", "Temporary End", "",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("UID", "User Interrupt Disable", "",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("UIE", "User Interrupt Enable", "",
                "Instruction"));
            dataList.Add(new StxCompletionItemCodeSnippetData("TAN", "Tangent", "Source",
                "Instruction"));

            #endregion

            return dataList;
        }
        
        private void AddVariableInfo(VariableInfo info)
        {
            if (info == null) return;
            if (info.Offset > 0 && info.TextLocation.Column == 0 && info.TextLocation.Line == 0)
            {
                Debug.Assert(false);
            }

            VariableInfos.Enqueue(info);
        }
    }

    public class ParseCodeArgs : EventArgs
    {
        public bool IsNeedUpdateView { set; get; }
    }

    public class BackgroundWorkException : Exception
    {

    }
}
