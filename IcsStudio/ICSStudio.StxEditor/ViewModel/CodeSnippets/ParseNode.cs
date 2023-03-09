using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Media;
using ICSStudio.AvalonEdit.Variable;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Instruction;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using NLog;
using System.Text.RegularExpressions;

namespace ICSStudio.StxEditor.ViewModel.CodeSnippets
{
    public partial class SnippetLexer
    {
        internal static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public bool CanAddError { set; get; } = false;

        private void ParseNode(SnippetInfo snippetInfo, ASTNode astNode, IDataType expectedDataType, bool isNotInAssign,
            ref bool isAccepted)
        {
            if (astNode == null) return;
            if (_backgroundWorker.CancellationPending) return;
            var stmtMode = astNode as ASTStmtMod;
            if (stmtMode != null)
            {
                if (_backgroundWorker.CancellationPending) return;
                foreach (var node in stmtMode.list.nodes)
                {
                    bool subAccepted = isAccepted;
                    ParseNode(snippetInfo, node, expectedDataType, isNotInAssign, ref subAccepted);
                }

                isAccepted = true;
            }

            var stmtError = astNode as ASTErrorStmt;
            if (stmtError != null)
            {
                if (_backgroundWorker.CancellationPending) return;
                if (stmtError.item != null)
                {
                    ConvertNodeToVariable(stmtError.item, snippetInfo, isNotInAssign, ref isAccepted, expectedDataType);
                    var variableInfos = snippetInfo.GetVariableInfos();
                    if (variableInfos.Any())
                    {
                        var variableInfo = snippetInfo.GetVariableInfos()[0];
                        if (((AoiDefinitionCollection) _controller.AOIDefinitionCollection).Find(variableInfo.Name) !=
                            null)
                        {
                            variableInfo.IsAOI = true;
                            variableInfo.IsInstr = true;
                        }
                    }
                }

                if (stmtError.number != null)
                    ConvertNodeToVariable(stmtError.number, snippetInfo, isNotInAssign, ref isAccepted,
                        expectedDataType);
                DrawFile(stmtError, snippetInfo.Parent, $"Unexpected grammar.");
                var code = snippetInfo.Parent.Substring(stmtError.ContextStart,
                    stmtError.ContextEnd - stmtError.ContextStart + 1);
                var regex = new Regex(@"\*/(?!/)|\*\)");
                var matches = regex.Matches(code);
                foreach (Match match in matches)
                {
                    DrawFile(match.Index + stmtError.ContextStart, 2, code,
                        $"\'{match.Value}\':End comment missing begin.");
                }

                isAccepted = true;
                return;
            }

            var astUnexpectedError = astNode as ASTUnexpectedError;
            if (astUnexpectedError != null)
            {
                var subAccepted = isAccepted;
                ParseNode(snippetInfo, astUnexpectedError.Item, expectedDataType, isNotInAssign, ref subAccepted);

                subAccepted = isAccepted;
                ParseNode(snippetInfo, astUnexpectedError.Expr, expectedDataType, isNotInAssign, ref subAccepted);
                DrawFile(astUnexpectedError, snippetInfo.Parent, $"Invalid syntax.");
                return;
            }

            AstCheck(astNode, snippetInfo, expectedDataType, isNotInAssign, ref isAccepted);
        }

        private string ConvertAstNameToSnippet(ASTName astName, SnippetInfo snippetInfo)
        {
            try
            {
                return snippetInfo.Parent.Substring(astName.ContextStart,
                    astName.ContextEnd - astName.ContextStart + 1);
            }
            catch (Exception e)
            {
                Debug.Assert(false, e.Message);
            }

            return "";
        }

        private void ParseAstNameArr(ASTName astName, SnippetInfo snippetInfo, ref bool isAccepted)
        {
            bool isHiddenVariable = snippetInfo.HiddenVariable;
            try
            {
                if (!isAccepted)
                {
                    astName.Accept(_typeChecker);
                    isAccepted = true;
                }

                foreach (var astNode in astName.id_list.nodes)
                {
                    var astNameItem = astNode as ASTNameItem;
                    if (astNameItem?.arr_list != null)
                    {
                        snippetInfo.HiddenVariable = true;
                        foreach (var node in astNameItem.arr_list.nodes)
                        {
                            if (node is ASTInteger) continue;
                            var subAstName = node as ASTName;
                            if (subAstName != null)
                            {
                                ConvertNodeToVariable(subAstName, snippetInfo, true, ref isAccepted, null);
                                continue;
                            }
                            else
                            {
                                ParseNode(snippetInfo, node, null, true, ref isAccepted);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Debug.Assert(false, e.Message);
            }
            finally
            {
                snippetInfo.HiddenVariable = isHiddenVariable;
            }
        }

        private void ConvertNodeToVariable(ASTNode node, SnippetInfo snippetInfo, bool isNotInAssign,
            ref bool isAccepted, IDataType expectedDataType = null, bool isGoOnParse = true)
        {
            if (_backgroundWorker.CancellationPending) return;

            if (!string.IsNullOrEmpty(node.Error))
            {
                AddErrorNode(node, snippetInfo);
                DrawFile(node, snippetInfo.Parent, node.Error, Colors.Red);
            }

            var astName = node as ASTName;
            if (astName != null)
            {
                if (!string.IsNullOrEmpty(astName.Error))
                {
                    throw new Exception(astName.Error);
                }

                try
                {
                    if (astName.type == null)
                    {
                        if (!isAccepted)
                        {
                            astName.Accept(_typeChecker);
                            isAccepted = true;
                        }
                    }

                    if (!_onlyTextMarker)
                    {
                        var info = new VariableInfo(astName, ConvertAstNameToSnippet(astName, snippetInfo), _routine,
                            snippetInfo.Offset + astName.ContextStart, snippetInfo.Parent,
                            _textMarkerService.TextDocument.GetLocationNotVerifyAccess(
                                snippetInfo.Offset + astName.ContextStart));
                        if ((astName.loaders?.nodes.Count > 0 && astName.loaders?.nodes[0] is ASTStatus) ||
                            snippetInfo.HiddenVariable) info.IsDisplay = false;

                        info.LineOffset = GetOffsetInLine(info.Offset, snippetInfo.Parent);
                        info.TargetDataType = astName.ExpectDataType;
                        snippetInfo.AddVariable(info);
                    }

                    if (astName.Expr is ASTExprStatus)
                    {
                        var name = ObtainValue.GetAstName(astName);
                        if ("S:V".Equals(name, StringComparison.OrdinalIgnoreCase) ||
                            "S:Z".Equals(name, StringComparison.OrdinalIgnoreCase) ||
                            "S:N".Equals(name, StringComparison.OrdinalIgnoreCase) ||
                            "S:C".Equals(name, StringComparison.OrdinalIgnoreCase))
                        {
                            throw new Exception("Match Status flag can only be used within Ladder routines.");
                        }
                    }
                }
                catch (Exception e)
                {
                    DrawFile(astName, snippetInfo.Parent, e.Message, Colors.Red);
                    AddErrorNode(node, snippetInfo);
                }

                ParseAstNameArr(astName, snippetInfo, ref isAccepted);

                return;
            }

            var astInt = node as ASTInteger;
            if (astInt != null)
            {
                if (astInt.value < int.MinValue || astInt.value > int.MaxValue)
                {
                    DrawFile(astInt, snippetInfo.Parent, "Value out of range");
                }

                if (!_onlyTextMarker)
                {
                    var numInfo = new VariableInfo(astInt, astInt.Context, _routine, astInt.ContextStart,
                        snippetInfo.Parent,
                        _textMarkerService.TextDocument.GetLocationNotVerifyAccess(astInt.ContextStart))
                    {
                        IsNum = true,
                        IsDisplay = false,
                    };
                    snippetInfo.AddVariable(numInfo);
                }

                return;
            }

            var astFloat = node as ASTFloat;
            if (astFloat != null)
            {
                if (!_onlyTextMarker)
                {
                    var numInfo = new VariableInfo(astFloat, astFloat.Context, _routine, astFloat.ContextStart,
                        snippetInfo.Parent,
                        _textMarkerService.TextDocument.GetLocationNotVerifyAccess(astFloat.ContextStart))
                    {
                        IsNum = true,
                        IsDisplay = false,
                    };
                    snippetInfo.AddVariable(numInfo);
                }

                return;
            }

            if (isGoOnParse)
                ParseNode(snippetInfo, node, expectedDataType, isNotInAssign, ref isAccepted);
            //Debug.Assert(false);
        }

        private void AstCheck(ASTNode ast, SnippetInfo snippetInfo, IDataType expectedDataType, bool isNotInAssign,
            ref bool isAccepted)
        {
            var astError = ast as ASTError;
            if (astError != null)
            {
                DrawFile(astError, snippetInfo.Parent, astError.Error);
                return;
            }

            if (ast is ASTInteger || ast is ASTName || ast is ASTFloat)
            {
                ConvertNodeToVariable(ast, snippetInfo, isNotInAssign, ref isAccepted, expectedDataType);
                return;
            }

            var assign = ast as ASTAssignStmt;
            if (assign != null)
            {
                if (!StmtCheck(assign, snippetInfo, ref isAccepted))
                {

                }

                return;
            }

            var astStore = ast as ASTStore;
            if (astStore != null)
            {
                StmtCheck(astStore, snippetInfo, ref isAccepted);
                return;
            }

            var ifStmt = ast as ASTIf;
            if (ifStmt != null)
            {
                var info = new SnippetInfo(snippetInfo.Parent)
                {
                    CodeText = snippetInfo.CodeText,
                    Offset = snippetInfo.Offset,
                };
                IfAst(ifStmt, info, ref isAccepted);
                foreach (var infoVariableInfo in info.GetVariableInfos())
                {
                    snippetInfo.AddVariable(infoVariableInfo);
                }

                return;
            }

            var caseStmt = ast as ASTCase;
            if (caseStmt != null)
            {
                var info = new SnippetInfo(snippetInfo.Parent)
                {
                    CodeText = snippetInfo.CodeText,
                    Offset = snippetInfo.Offset,
                };
                CaseAst(caseStmt, info, ref isAccepted);
                foreach (var infoVariableInfo in info.GetVariableInfos())
                {
                    snippetInfo.AddVariable(infoVariableInfo);
                }

                return;
            }

            var repeatStmt = ast as ASTRepeat;
            if (repeatStmt != null)
            {
                var info = new SnippetInfo(snippetInfo.Parent)
                {
                    CodeText = snippetInfo.CodeText,
                    Offset = snippetInfo.Offset,
                };
                RepeatAst(repeatStmt, info, ref isAccepted);
                foreach (var infoVariableInfo in info.GetVariableInfos())
                {
                    snippetInfo.AddVariable(infoVariableInfo);
                }

                return;
            }

            var forStmt = ast as ASTFor;
            if (forStmt != null)
            {
                var info = new SnippetInfo(snippetInfo.Parent)
                {
                    CodeText = snippetInfo.CodeText,
                    Offset = snippetInfo.Offset,
                };
                ForAst(forStmt, info, ref isAccepted);
                foreach (var infoVariableInfo in info.GetVariableInfos())
                {
                    snippetInfo.AddVariable(infoVariableInfo);
                }

                return;
            }

            var whileStmt = ast as ASTWhile;
            if (whileStmt != null)
            {
                var info = new SnippetInfo(snippetInfo.Parent)
                {
                    CodeText = snippetInfo.CodeText,
                    Offset = snippetInfo.Offset,
                };
                WhileAst(whileStmt, info, ref isAccepted);
                foreach (var infoVariableInfo in info.GetVariableInfos())
                {
                    snippetInfo.AddVariable(infoVariableInfo);
                }

                return;
            }

            var astStmtMod = ast as ASTStmtMod;
            if (astStmtMod != null)
            {
                StmtModeAST(astStmtMod, snippetInfo, ref isAccepted);
                return;
            }

            var astInstr = ast as ASTInstr;
            if (astInstr != null)
            {
                InstrAst(astInstr, snippetInfo, isNotInAssign, ref isAccepted);
                return;
            }

            var astBinOp = ast as ASTBinOp;
            if (astBinOp != null)
            {
                BinOpAst(astBinOp, snippetInfo, expectedDataType, isNotInAssign, ref isAccepted);
                return;
            }

            var astUnaryOp = ast as ASTUnaryOp;
            if (astUnaryOp != null)
            {
                UnaryOpAst(astUnaryOp,snippetInfo,expectedDataType,isNotInAssign,ref isAccepted);
                return;
            }

            var astCall = ast as ASTCall;
            if (astCall != null)
            {
                var instr = new ASTInstr(astCall.name, astCall.param_list)
                {
                    ContextStart = astCall.ContextStart,
                    ContextEnd = astCall.ContextEnd,
                    Error = astCall.Error
                };
                isAccepted = true;
                InstrAst(instr, snippetInfo, isNotInAssign, ref isAccepted);
                return;
            }

            var astNodeList = ast as ASTNodeList;
            if (astNodeList != null)
            {
                if (!string.IsNullOrEmpty(astNodeList.Error))
                {
                    DrawFile(astNodeList, snippetInfo.Parent, astNodeList.Error);
                    snippetInfo.IsGrammarError = true;
                    isAccepted = true;
                    return;
                }

                foreach (var astNode in astNodeList.nodes)
                {
                    if (snippetInfo.IsGrammarError) break;
                    var subAccepted = isAccepted;
                    ParseNode(snippetInfo, astNode, expectedDataType, isNotInAssign, ref subAccepted);
                }

                isAccepted = true;
                return;
            }

            var astPair = ast as ASTPair;
            if (astPair != null)
            {
                if (!string.IsNullOrEmpty(astPair.Error))
                {
                    DrawFile(astPair, snippetInfo.Parent, astPair.Error);
                }

                var left = astPair.left;
                var subAccepted = isAccepted;
                if (astPair.IsNeedBoolExpr)
                {
                    CheckCond(astPair.left, snippetInfo, isAccepted);
                }

                ParseNode(snippetInfo, left, expectedDataType, isNotInAssign, ref subAccepted);
                var right = astPair.right;
                subAccepted = isAccepted;
                ParseNode(snippetInfo, right, null, isNotInAssign, ref subAccepted);
                isAccepted = true;
                if (left == null || right == null)
                {
                    DrawFile(astPair, snippetInfo.Parent, "Error grammar.");
                }

                return;
            }

            var astNameAttr = ast as ASTNameAddr;
            if (astNameAttr != null)
            {
                var subAccepted = isAccepted;
                ParseNode(snippetInfo, astNameAttr.name, null, isNotInAssign, ref subAccepted);
                isAccepted = true;
                return;
            }

            var astExit = ast as ASTExit;
            if (astExit != null)
            {
                return;
            }

            var astEmpty = ast as ASTEmpty;
            if (astEmpty != null)
            {
                return;
            }

            Debug.Assert(false, ast?.GetType().FullName ?? "");
            //TODO(zyl):add more
        }

        private IDataType GetRelCommonType(ASTBinOp astBinOp)
        {
            if (astBinOp.left.type.IsReal || astBinOp.right.type.IsReal)
            {
                if (astBinOp is ASTBinRelOp)
                {
                    return BOOL.Inst;
                }

                return REAL.Inst;
            }
            else if (astBinOp.left.type.IsInteger || astBinOp.right.type.IsInteger)
            {
                var leftInt = astBinOp.left as ASTInteger;
                var rightInt = astBinOp.right as ASTInteger;
                if (leftInt != null && rightInt != null)
                {
                    if ((leftInt.value == 1 || leftInt.value == 0) && (rightInt.value == 1 || rightInt.value == 0))
                    {
                        if (astBinOp is ASTBinRelOp)
                        {
                            return BOOL.Inst;
                        }

                        return new ExpectType(DINT.Inst, BOOL.Inst);
                    }
                }

                IDataType left = astBinOp.left.type;
                IDataType right = astBinOp.right.type;
                if (astBinOp.left is ASTBinOp)
                {
                    left = GetRelCommonType((ASTBinOp) astBinOp.left);
                    if (rightInt != null && (rightInt.value == 1 || rightInt.value == 0))
                    {
                        if (astBinOp is ASTBinRelOp)
                        {
                            return BOOL.Inst;
                        }

                        return left;
                    }
                }

                if (astBinOp.right is ASTBinOp)
                {
                    right = GetRelCommonType((ASTBinOp) astBinOp.right);
                    if (leftInt != null && (leftInt.value == 1 || leftInt.value == 0))
                    {
                        if (astBinOp is ASTBinRelOp)
                        {
                            return BOOL.Inst;
                        }

                        return right;
                    }
                }

                if (astBinOp is ASTBinRelOp)
                {
                    return BOOL.Inst;
                }

                return DINT.Inst;
            }
            else
            {
                Debug.Assert(false);
                return BOOL.Inst;
            }
        }

        private void UnaryOpAst(ASTUnaryOp astUnaryOp, SnippetInfo snippetInfo, IDataType expectedDataType,bool isNotInAssign,ref bool isAccepted)
        {
            var expr = astUnaryOp.expr;
            if (!isAccepted)
            {
                expr = expr.Accept(_typeChecker) as ASTExpr;
                isAccepted = true;
            }

            if (expectedDataType != null)
            {
                if (expectedDataType.IsBool)
                {
                    if (astUnaryOp.op != ASTUnaryOp.Op.NOT)
                    {
                        DrawFile(astUnaryOp, snippetInfo.Parent,
                            $" '{ObtainValue.ConvertASTUnaryOp(astUnaryOp.op)}':Operator not allowed in BOOL expression.");
                    }
                    CheckCond(expr, snippetInfo, isAccepted);
                }
                else
                {
                    if (!expr?.type.Equal(expectedDataType, false) ?? true)
                    {
                        DrawFile(astUnaryOp, snippetInfo.Parent, "Not expected in the expression.");
                    }
                }
            }

            var integer = expr as ASTInteger;
            if (integer != null)
            {
                var value = astUnaryOp.op == ASTUnaryOp.Op.NEG ? -integer.value : integer.value;
                if (value > int.MaxValue || value < int.MinValue)
                {
                    DrawFile(astUnaryOp, snippetInfo.Parent, "Value out of range");
                }

                ConvertNodeToVariable(
                    new ASTInteger(value)
                    { ContextStart = astUnaryOp.ContextStart, ContextEnd = astUnaryOp.ContextEnd }, snippetInfo,
                    isNotInAssign, ref isAccepted, expectedDataType);
                return;
            }

            var real = expr as ASTFloat;
            if (real != null)
            {
                var value = astUnaryOp.op == ASTUnaryOp.Op.NEG ? -real.value : real.value;
                if (value > float.MaxValue || value < float.MinValue)
                {
                    DrawFile(astUnaryOp, snippetInfo.Parent, "Value out of range");
                }

                ConvertNodeToVariable(
                    new ASTFloat(value)
                    { ContextStart = astUnaryOp.ContextStart, ContextEnd = astUnaryOp.ContextEnd }, snippetInfo,
                    isNotInAssign, ref isAccepted, expectedDataType);
                return;
            }

            ConvertNodeToVariable(expr, snippetInfo, isNotInAssign, ref isAccepted, expectedDataType);
        }

        private void CheckAstBinOpBoolType(ASTExpr a, ASTExpr b, IDataType expectedDataType, SnippetInfo snippetInfo)
        {
            if (expectedDataType == null) return;
            if (b?.type?.IsInteger ?? false)
            {
                var integer = b as ASTInteger;
                if (integer != null)
                {
                    if (integer.value == 0 || integer.value == 1)
                    {

                    }
                    else
                    {
                        if (expectedDataType?.Equal(a.type) ?? false)
                        {
                            DrawFile(integer, snippetInfo.Parent,
                                $"'{snippetInfo.Parent.Substring(integer.ContextStart, integer.ContextEnd - integer.ContextStart + 1)}':BOOL tag expected.");
                        }
                        else
                        {
                            if (IsPrimNode(a))
                                DrawFile(a, snippetInfo.Parent,
                                    $"'{snippetInfo.Parent.Substring(a.ContextStart, a.ContextEnd - a.ContextStart + 1)}':BOOL tag not expected in expression.");
                        }
                    }
                }
                else
                {
                    if (b is ASTBinOp)
                    {
                        var type = GetRelCommonType((ASTBinOp) b);
                        if (!type.Equal(BOOL.Inst))
                        {
                            if (expectedDataType?.Equal(a.type) ?? false)
                            {
                                var isAccepted = true;
                                BinOpAst((ASTBinOp) b, snippetInfo, expectedDataType, true, ref isAccepted);
                            }
                            else
                            {
                                if (a is ASTBinOp)
                                {
                                    var isAccepted = true;
                                    BinOpAst((ASTBinOp) a, snippetInfo, expectedDataType, true, ref isAccepted);
                                }
                                else
                                {
                                    if (expectedDataType != null &&  IsPrimNode(a))
                                        DrawFile(a, snippetInfo.Parent,
                                            $"'{snippetInfo.Parent.Substring(a.ContextStart, a.ContextEnd - a.ContextStart + 1)}':BOOL tag not expected in expression.");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (expectedDataType?.Equal(a.type) ?? false)
                        {
                            if (IsPrimNode(b))
                                DrawFile(b, snippetInfo.Parent,
                                    $"'{snippetInfo.Parent.Substring(b.ContextStart, b.ContextEnd - b.ContextStart + 1)}':BOOL tag expected.");
                        }
                        else
                        {
                            if (a is ASTBinOp)
                            {
                                var isAccepted = true;
                                BinOpAst((ASTBinOp) a, snippetInfo, expectedDataType, true, ref isAccepted);
                            }
                            else
                            {
                                if (expectedDataType != null && IsPrimNode(a))
                                    DrawFile(a, snippetInfo.Parent,
                                        $"'{snippetInfo.Parent.Substring(a.ContextStart, a.ContextEnd - a.ContextStart + 1)}':BOOL tag not expected in expression.");
                            }
                        }
                    }
                }
            }


            if (b?.type?.IsReal ?? false)
            {
                if (b is ASTName)
                    DrawFile(b, snippetInfo.Parent,
                        $"'{snippetInfo.Parent.Substring(b.ContextStart, b.ContextEnd - b.ContextStart + 1)}':BOOL tag expected.");
            }
        }

        private void BinArithOpAst(ASTBinArithOp astBinArithOp, SnippetInfo snippetInfo, IDataType expectedDataType,
            ref bool isAccepted)
        {
            var op = astBinArithOp.op;
            if (expectedDataType?.IsBool ?? false)
            {
                if (IsPrimNode(astBinArithOp.left))
                    DrawFile(astBinArithOp.left, snippetInfo.Parent,
                        $"'{ObtainValue.ConvertOp(op)}':Operator not allowed in BOOL expression.");
            }

            if (!(astBinArithOp.left.type.IsInteger || astBinArithOp.left.type.IsReal))
            {
                if (astBinArithOp.left.type.IsBool)
                {
                    if (IsPrimNode(astBinArithOp.left))
                        DrawFile(astBinArithOp.left, snippetInfo.Parent,
                            $"'{snippetInfo.Parent.Substring(astBinArithOp.left.ContextStart, astBinArithOp.left.ContextEnd - astBinArithOp.left.ContextStart + 1)}':BOOL tag not expected in expression.");
                }
                else
                {
                    if (IsPrimNode(astBinArithOp.left))
                        DrawFile(astBinArithOp.left, snippetInfo.Parent,
                            $"Invalid data type.Argument must match parameter data type.");
                }
            }

            if (!(astBinArithOp.right.type.IsInteger || astBinArithOp.right.type.IsReal))
            {
                if (astBinArithOp.right.type.IsBool)
                {
                    if (IsPrimNode(astBinArithOp.right))
                        DrawFile(astBinArithOp.right, snippetInfo.Parent,
                            $"'{snippetInfo.Parent.Substring(astBinArithOp.right.ContextStart, astBinArithOp.right.ContextEnd - astBinArithOp.right.ContextStart + 1)}':BOOL tag not expected in expression.");
                }
                else
                {
                    if (IsPrimNode(astBinArithOp.right))
                        DrawFile(astBinArithOp.right, snippetInfo.Parent,
                            $"Invalid data type.Argument must match parameter data type.");
                }
            }
        }

        private bool IsPrimNode(ASTNode node)
        {
            return node is ASTName || node is ASTInteger || node is ASTFloat || node is ASTUnaryOp;
        }

        private void BinLogicOpAst(ASTBinLogicOp astBinOp, SnippetInfo snippetInfo, IDataType expectedDataType,
            ref bool isAccepted)
        {
            bool isChecked = false;
            if (astBinOp.left.type?.IsBool ?? false)
            {
                if (astBinOp.right?.type?.IsInteger ?? false)
                {
                    CheckAstBinOpBoolType(astBinOp.left, astBinOp.right, expectedDataType, snippetInfo);
                    isChecked = true;
                }else if (astBinOp.right?.type?.IsBool ?? false)
                {
                    if (!(expectedDataType?.Equal(astBinOp.left.type)??true))
                    {
                        if (IsPrimNode(astBinOp.right))
                            DrawFile(astBinOp.right, snippetInfo.Parent,
                                $"'{snippetInfo.Parent.Substring(astBinOp.right.ContextStart, astBinOp.right.ContextEnd - astBinOp.right.ContextStart + 1)}':BOOL tag not expected in expression.");
                        if (IsPrimNode(astBinOp.left))
                            DrawFile(astBinOp.left, snippetInfo.Parent,
                                $"'{snippetInfo.Parent.Substring(astBinOp.left.ContextStart, astBinOp.left.ContextEnd - astBinOp.left.ContextStart + 1)}':BOOL tag not expected in expression");
                    }
                    isChecked = true;
                }
                else if (astBinOp.right?.type?.IsReal ?? false)
                {
                    if (expectedDataType != null)
                    {
                        if (astBinOp.right.type.Equal(expectedDataType))
                        {
                            if (IsPrimNode(astBinOp.left))
                                DrawFile(astBinOp.left, snippetInfo.Parent,
                                    $"'{snippetInfo.Parent.Substring(astBinOp.left.ContextStart, astBinOp.left.ContextEnd - astBinOp.left.ContextStart + 1)}':BOOL tag expected.");
                        }
                        else
                        {
                            if (IsPrimNode(astBinOp.right))
                                DrawFile(astBinOp.right, snippetInfo.Parent,
                                    $"'{snippetInfo.Parent.Substring(astBinOp.right.ContextStart, astBinOp.right.ContextEnd - astBinOp.right.ContextStart + 1)}':BOOL tag expected.");
                        }
                    }
                    isChecked = true;
                }
                else if (!(astBinOp.right?.type?.IsAtomic ?? true))
                {
                    if (IsPrimNode(astBinOp.right))
                        DrawFile(astBinOp.right, snippetInfo.Parent,
                            $"'{snippetInfo.Parent.Substring(astBinOp.right.ContextStart, astBinOp.right.ContextEnd - astBinOp.right.ContextStart + 1)}':BOOL tag expected.");
                    isChecked = true;
                }
            }

            if ((astBinOp.right?.type?.IsBool ?? false) && !isChecked)
            {
                if (expectedDataType != null)
                {
                    if (astBinOp.left?.type?.IsInteger ?? false)
                    {
                        CheckAstBinOpBoolType(astBinOp.right, astBinOp.left, expectedDataType, snippetInfo);
                        isChecked = true;
                    }
                    else if (astBinOp.left?.type?.IsReal ?? false)
                    {
                        if (astBinOp.right.type.Equal(expectedDataType))
                        {
                            if (IsPrimNode(astBinOp.left))
                                DrawFile(astBinOp.left, snippetInfo.Parent,
                                    $"'{snippetInfo.Parent.Substring(astBinOp.left.ContextStart, astBinOp.left.ContextEnd - astBinOp.left.ContextStart + 1)}':BOOL tag expected.");
                        }
                        else
                        {
                            if (IsPrimNode(astBinOp.right))
                                DrawFile(astBinOp.right, snippetInfo.Parent,
                                    $"'{snippetInfo.Parent.Substring(astBinOp.right.ContextStart, astBinOp.right.ContextEnd - astBinOp.right.ContextStart + 1)}':BOOL tag expected.");
                        }
                        isChecked = true;
                    }
                    else if (!astBinOp.left?.type?.IsAtomic ?? true)
                    {
                        if (IsPrimNode(astBinOp.left))
                            DrawFile(astBinOp.left, snippetInfo.Parent,
                                $"'{snippetInfo.Parent.Substring(astBinOp.left.ContextStart, astBinOp.left.ContextEnd - astBinOp.left.ContextStart + 1)}':BOOL tag expected.");
                        isChecked = true;
                    }
                }
            }

            var op = astBinOp.op;
            if (((astBinOp.left.type?.IsInteger ?? false) || (astBinOp.left.type?.IsReal ?? false)) && !isChecked)
            {
                if (((astBinOp.right?.type?.IsInteger ?? false) || (astBinOp.right?.type?.IsReal ?? false)))
                {
                    if (op == ASTBinOp.Op.AND || op == ASTBinOp.Op.OR || op == ASTBinOp.Op.XOR)
                    {
                        if (expectedDataType?.IsBool ?? false)
                        {
                            var integer = astBinOp.left as ASTInteger;
                            if (integer != null)
                            {
                                if (!(integer?.value == 1 || integer?.value == 0))
                                    if (IsPrimNode(astBinOp.left))
                                        DrawFile(astBinOp.left, snippetInfo.Parent,
                                            $"'{snippetInfo.Parent.Substring(astBinOp.left.ContextStart, astBinOp.left.ContextEnd - astBinOp.left.ContextStart + 1)}':BOOL tag expected.");
                            }

                            var astBin = astBinOp.left as ASTBinOp;
                            if (astBin != null)
                            {
                                var type = GetRelCommonType(astBin);
                                if (!(expectedDataType?.Equal(type) ?? true))
                                {
                                    BinOpAst(astBin, snippetInfo, expectedDataType, false, ref isAccepted);
                                    //DrawFile(astBinOp.left, snippetInfo.Parent, $"'{snippetInfo.Parent.Substring(astBinOp.left.ContextStart, astBinOp.left.ContextEnd - astBinOp.left.ContextStart + 1)}':BOOL tag expected.");
                                }
                            }

                            if (integer == null && astBin == null)
                            {
                                if (IsPrimNode(astBinOp.left))
                                    DrawFile(astBinOp.left, snippetInfo.Parent,
                                        $"'{snippetInfo.Parent.Substring(astBinOp.left.ContextStart, astBinOp.left.ContextEnd - astBinOp.left.ContextStart + 1)}':BOOL tag expected.");
                            }

                            integer = astBinOp.right as ASTInteger;
                            if (integer != null)
                            {
                                if (!(integer?.value == 1 || integer?.value == 0))
                                    if (IsPrimNode(astBinOp.right))
                                        DrawFile(astBinOp.right, snippetInfo.Parent,
                                            $"'{snippetInfo.Parent.Substring(astBinOp.right.ContextStart, astBinOp.right.ContextEnd - astBinOp.right.ContextStart + 1)}':BOOL tag expected.");
                            }

                            astBin = astBinOp.right as ASTBinOp;
                            if (astBin != null)
                            {
                                var type = GetRelCommonType(astBin);
                                if (!(expectedDataType?.Equal(type) ?? true))
                                {
                                    BinOpAst(astBin, snippetInfo, expectedDataType, false, ref isAccepted);
                                    //DrawFile(astBinOp.right, snippetInfo.Parent, $"'{snippetInfo.Parent.Substring(astBinOp.right.ContextStart, astBinOp.right.ContextEnd - astBinOp.right.ContextStart + 1)}':BOOL tag expected.");
                                }
                            }

                            var astUnaryOp = astBinOp.right as ASTUnaryOp;
                            if (astUnaryOp != null)
                            {
                                ParseNode(snippetInfo, astUnaryOp, expectedDataType, false, ref isAccepted);
                            }

                            if (integer == null && astBin == null && astUnaryOp == null)
                            {
                                if (IsPrimNode(astBinOp.right))
                                    DrawFile(astBinOp.right, snippetInfo.Parent,
                                        $"'{snippetInfo.Parent.Substring(astBinOp.right.ContextStart, astBinOp.right.ContextEnd - astBinOp.right.ContextStart + 1)}':BOOL tag expected.");
                            }
                        }
                    }

                    isChecked = true;
                }
            }

            if (!isChecked)
            {
                if (astBinOp.left.type != null)
                    if (!(astBinOp.left.type.IsAtomic))
                    {
                        if (IsPrimNode(astBinOp.left))
                            DrawFile(astBinOp.left, snippetInfo.Parent,
                                $"Invalid data type.Argument must match parameter data type.");
                    }

                if (astBinOp.right.type != null)
                    if (!(astBinOp.right.type.IsAtomic))
                    {
                        if (IsPrimNode(astBinOp.right))
                            DrawFile(astBinOp.right, snippetInfo.Parent,
                                $"Invalid data type.Argument must match parameter data type.");
                    }
            }
        }

        private void BinRelOpAst(ASTBinRelOp astBinRelOp, SnippetInfo snippetInfo, IDataType expectedDataType,
            ref bool isAccepted)
        {
            var op = astBinRelOp.op;
            if (!(expectedDataType?.IsBool ?? true))
            {
                if (IsPrimNode(astBinRelOp.left))
                    DrawFile(astBinRelOp.left, snippetInfo.Parent,
                        $"'{ObtainValue.ConvertOp(op)}':Operator not allowed in numerical expression.");
            }

            if (!(astBinRelOp.left.type.IsInteger || astBinRelOp.left.type.IsReal))
            {
                if (astBinRelOp.left.type.IsBool)
                {
                    if (IsPrimNode(astBinRelOp.left))
                        DrawFile(astBinRelOp.left, snippetInfo.Parent,
                            $"'{snippetInfo.Parent.Substring(astBinRelOp.left.ContextStart, astBinRelOp.left.ContextEnd - astBinRelOp.left.ContextStart + 1)}':BOOL tag not expected in expression.");
                }
                else
                {
                    if (IsPrimNode(astBinRelOp.left))
                        DrawFile(astBinRelOp.left, snippetInfo.Parent,
                            $"Invalid data type.Argument must match parameter data type.");
                }
            }

            if (!(astBinRelOp.right.type.IsInteger || astBinRelOp.right.type.IsReal))
            {
                if (astBinRelOp.right.type.IsBool)
                {
                    if (IsPrimNode(astBinRelOp.right))
                        DrawFile(astBinRelOp.right, snippetInfo.Parent,
                            $"'{snippetInfo.Parent.Substring(astBinRelOp.right.ContextStart, astBinRelOp.right.ContextEnd - astBinRelOp.right.ContextStart + 1)}':BOOL tag not expected in expression.");
                }
                else
                {
                    if (IsPrimNode(astBinRelOp.right))
                        DrawFile(astBinRelOp.right, snippetInfo.Parent,
                            $"Invalid data type.Argument must match parameter data type.");
                }
            }
        }

        private IDataType GetBinOpAstExpectedDataType(ASTBinOp astBinOp,IDataType expectedDataType)
        {
            if (astBinOp is ASTBinRelOp || astBinOp is ASTBinArithOp) return DINT.Inst;
            return expectedDataType;
            //if (astBinOp.left.type == null)
            //{
            //    try
            //    {
            //        return (astBinOp.left.Accept(_typeChecker) as ASTExpr)?.type??expectedDataType;
            //    }
            //    catch (Exception)
            //    {

            //    }
            //}

            //if (astBinOp.left is ASTInteger)
            //{
            //    if (((ASTInteger) astBinOp.left).value == 0 || ((ASTInteger) astBinOp.left).value == 1) return expectedDataType;
            //}
            //return astBinOp.left.type;
        }

        private void BinOpAst(ASTBinOp astBinOp, SnippetInfo snippetInfo, IDataType expectedDataType,
            bool isNotInAssign, ref bool isAccepted)
        {
            if (!string.IsNullOrEmpty(astBinOp.Error))
            {
                DrawFile(astBinOp, snippetInfo.Parent, astBinOp.Error);
            }

            var subAccepted = isAccepted;
            if (!string.IsNullOrEmpty(astBinOp.left.Error))
            {
                DrawFile(astBinOp.left, snippetInfo.Parent, astBinOp.left.Error);
            }

            var astBinOpExpectedDataType = GetBinOpAstExpectedDataType(astBinOp, expectedDataType);
            ConvertNodeToVariable(astBinOp.left, snippetInfo, false, ref subAccepted, astBinOpExpectedDataType);
            subAccepted = isAccepted;
            if (!string.IsNullOrEmpty(astBinOp.right.Error))
            {
                DrawFile(astBinOp.right, snippetInfo.Parent, astBinOp.right.Error);
            }

            ConvertNodeToVariable(astBinOp.right, snippetInfo, false, ref subAccepted, astBinOpExpectedDataType);
            if (astBinOp.left.type == null)
            {
                if (!isAccepted)
                {
                    try
                    {
                        if (astBinOp.left.IsMarked)
                        {
                            return;
                        }

                        astBinOp.left.Accept(_typeChecker);
                    }
                    catch (Exception)
                    {
                        isAccepted = true;
                    }
                }
            }

            if (astBinOp.right.type == null)
            {
                if (!isAccepted)
                {
                    try
                    {
                        if (astBinOp.right.IsMarked)
                        {
                            return;
                        }

                        astBinOp.right.Accept(_typeChecker);
                        isAccepted = true;
                    }
                    catch (Exception)
                    {
                        isAccepted = true;
                    }
                }
            }

            if (astBinOp is ASTBinArithOp)
            {
                BinArithOpAst((ASTBinArithOp)astBinOp, snippetInfo, expectedDataType, ref isAccepted);
            }
            else if (astBinOp is ASTBinLogicOp)
            {
                BinLogicOpAst((ASTBinLogicOp)astBinOp, snippetInfo, expectedDataType, ref isAccepted);
            }
            else if (astBinOp is ASTBinRelOp)
            {
                BinRelOpAst((ASTBinRelOp)astBinOp, snippetInfo, expectedDataType, ref isAccepted);
            }

            var left = astBinOp.left as ASTName;
            if (left != null)
            {
                if (left.Expr == null)
                {
                    left.Accept(_typeChecker);
                }

                if (left.Expr?.type is ArrayType)
                {
                    DrawFile(left, snippetInfo.Parent, $"Missing reference to array element.");
                }
            }

            var right = astBinOp.right as ASTName;
            if (right != null)
            {
                if (right.Expr == null)
                {
                    right.Accept(_typeChecker);
                }

                if (right.Expr?.type is ArrayType)
                {
                    DrawFile(right, snippetInfo.Parent, $"Missing reference to array element.");
                }
            }

            isAccepted = true;
        }

        private void CheckConstantInAoiInstr(AoiDefinition.AOIInstruction instr, ASTName node, int index, string code)
        {
            if (instr == null) return;
            var type = instr.GetParameterType(index);
            if (type == ParameterType.INPUT) return;
            var name = ObtainValue.GetAstName(node);
            if (ObtainValue.IsConstant(name, TransformTable, _routine.ParentCollection.ParentProgram))
            {
                DrawFile(node, code, $"Parameter {++index},'{name}':Routines cannot write Constant tags.");
            }
        }

        private void SetLocation(AoiDataReference aoiDataReference, AoiDefinition aoi, bool isInner)
        {
            try
            {
                var location = _textMarkerService.TextDocument.GetLocationNotVerifyAccess(aoiDataReference.Offset);
                aoiDataReference.Line = location.Line;
                aoiDataReference.Column = location.Column;
                if (isInner)
                {
                    aoi.AddInnerReference(aoiDataReference);
                }
                else
                    aoi.AddReference(aoiDataReference);
            }
            catch (Exception e)
            {
                Logger.Error($"aoi:{aoi.Name} SetLocation.error:{e.StackTrace}");
            }
        }

        private bool VerifyInstrParam(string specifier, IDataType targetDataType)
        {
            try
            {
                if (targetDataType == null)
                {
                    //targetDataType=new ExpectType(DINT.Inst);
                    return true;
                }

                if (string.IsNullOrEmpty(specifier)) return false;
                var info = ObtainValue.GetTargetDataTypeInfo(specifier,
                    _connectionReference?.Routine?.ParentCollection.ParentProgram ?? _parentProgram, TransformTable);
                IDataType currentDataType;
                if (info.Dim3 > 0 || info.Dim2 > 0)
                {
                    currentDataType = new ArrayTypeNormal(info.DataType);
                }
                else if (info.Dim1 > 0)
                {
                    currentDataType = new ArrayTypeDimOne(info.DataType);
                }
                else
                {
                    currentDataType = info.DataType;
                }

                if (currentDataType == null)
                {
                    return false;
                }

                return DataTypeExtend.IsMatched(currentDataType, targetDataType);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return true;
            }
        }

        private void StmtModeAST(ASTStmtMod astStmtMod, SnippetInfo snippetInfo, ref bool isAccepted)
        {
            foreach (var astNode in astStmtMod.list.nodes)
            {
                var subAccepted = isAccepted;
                if (!StmtCheck(astNode, snippetInfo, ref subAccepted))
                {
                    DrawFile(snippetInfo.Offset, snippetInfo.EndOffset, snippetInfo);
                }
            }

            isAccepted = true;
        }

        private void CheckCond(ASTNode cond, SnippetInfo snippetInfo, bool isAccepted)
        {
            if (cond != null && !(cond is ASTError))
            {
                if (cond is ASTInteger)
                {
                    var astInt = (ASTInteger) cond;
                    ConvertNodeToVariable(cond, snippetInfo, true, ref isAccepted, BOOL.Inst);
                    if (!(astInt.value == 0 || astInt.value == 1))
                    {
                        DrawFile(cond, snippetInfo.Parent,
                            $"'{snippetInfo.Parent.Substring(cond.ContextStart, cond.ContextEnd - cond.ContextStart + 1)}':BOOL tag expected.");
                    }
                }
                else if (cond is ASTFloat)
                {
                    ConvertNodeToVariable(cond, snippetInfo, true, ref isAccepted, BOOL.Inst);
                    DrawFile(cond, snippetInfo.Parent,
                        $"'{snippetInfo.Parent.Substring(cond.ContextStart, cond.ContextEnd - cond.ContextStart + 1)}':BOOL tag expected.");
                }
                else if (cond is ASTName)
                {
                    ConvertNodeToVariable(cond, snippetInfo, true, ref isAccepted, BOOL.Inst);
                    if (!isAccepted)
                        cond.Accept(_typeChecker);
                    var astName = (ASTName) cond;
                    if (astName.type is BOOL)
                    {
                        if (astName.Expr.type is ArrayType)
                        {
                            DrawFile(cond, snippetInfo.Parent, $"Missing reference to array element.");
                        }
                    }
                    else
                    {
                        DrawFile(cond, snippetInfo.Parent,
                            $"'{snippetInfo.Parent.Substring(cond.ContextStart, cond.ContextEnd - cond.ContextStart + 1)}':BOOL tag expected.");
                    }
                }
                else if (cond is ASTInstr || cond is ASTCall)
                {
                    ConvertNodeToVariable(cond, snippetInfo, true, ref isAccepted, BOOL.Inst);
                    DrawFile(cond, snippetInfo.Parent,
                        $"'{snippetInfo.Parent.Substring(cond.ContextStart, cond.ContextEnd - cond.ContextStart + 1)}':BOOL tag expected.");
                }
                else
                {
                    ParseNode(snippetInfo, cond, BOOL.Inst, true, ref isAccepted);
                }
            }
            else
            {
                DrawFile(cond, snippetInfo.Parent, cond?.Error);
            }
        }

        private void IfAst(ASTIf ifStmt, SnippetInfo snippetInfo, ref bool isAccepted)
        {
            CheckCond(ifStmt.cond, snippetInfo, isAccepted);

            if (ifStmt.then_list != null && !(ifStmt.then_list is ASTError))
            {
                var subAccepted = isAccepted;
                ParseNode(snippetInfo, ifStmt.then_list, null, true, ref subAccepted);
            }
            else
            {
                DrawFile(ifStmt.then_list, snippetInfo.Parent, ifStmt.then_list?.Error);
            }

            if (snippetInfo.IsGrammarError) return;
            if (ifStmt.else_list != null && !(ifStmt.else_list is ASTError))
            {
                var subAccepted = isAccepted;
                ParseNode(snippetInfo, ifStmt.else_list, null, true, ref subAccepted);
            }
            else
            {
                DrawFile(ifStmt.else_list, snippetInfo.Parent, ifStmt.else_list?.Error);
            }

            if (snippetInfo.IsGrammarError) return;
            if (ifStmt.elsif_list != null && !(ifStmt.elsif_list is ASTError))
            {
                var subAccepted = isAccepted;
                ParseNode(snippetInfo, ifStmt.elsif_list, BOOL.Inst, true, ref subAccepted);
            }
            else
            {
                DrawFile(ifStmt.elsif_list, snippetInfo.Parent, ifStmt.elsif_list?.Error);
            }

            if (!string.IsNullOrEmpty(ifStmt.Error))
            {
                DrawFile(ifStmt, snippetInfo.Parent, ifStmt.Error);
                snippetInfo.IsCurrent = false;
                snippetInfo.IsGrammarError = true;
            }

            isAccepted = true;
        }

        private void CaseAst(ASTCase caseStmt, SnippetInfo snippetInfo, ref bool isAccepted)
        {
            var elemList = caseStmt.elem_list;
            if (elemList != null)
            {
                var subAccepted = isAccepted;
                ParseNode(snippetInfo, elemList, null, true, ref subAccepted);
            }

            if (snippetInfo.IsGrammarError) return;
            var elseList = caseStmt.else_stmts;
            if (elseList != null)
            {
                var subAccepted = isAccepted;
                ParseNode(snippetInfo, elseList, null, true, ref subAccepted);
            }

            if (snippetInfo.IsGrammarError) return;
            if (caseStmt.expr != null && !(caseStmt.expr is ASTError))
            {
                try
                {
                    var subAccepted = isAccepted;
                    ConvertNodeToVariable(caseStmt.expr, snippetInfo, true, ref subAccepted,
                        new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
                    var expr = subAccepted ? caseStmt.expr as ASTExpr : caseStmt.expr.Accept(_typeChecker) as ASTExpr;
                    if (expr?.type == null)
                    {
                        expr = caseStmt.expr.Accept(_typeChecker) as ASTExpr;
                    }

                    if (expr != null)
                    {
                        if (((expr.type.IsInteger || expr.type.IsReal)))
                        {
                            var astName = expr as ASTName;
                            if (astName != null)
                            {
                                if (astName.dim1 > 0)
                                {
                                    DrawFile(caseStmt.expr, snippetInfo.Parent, "Missing reference to array element.");
                                }
                            }
                        }
                        else
                        {
                            //some controllers (eg. 5580 controller) support LINT in V32 5000.
                            DrawFile(caseStmt.expr, snippetInfo.Parent,
                                "Invalid data type.Argument must match parameter data type.");
                        }
                    }

                }
                catch (Exception e)
                {
                    if (!(caseStmt.expr is ASTEmpty))
                        DrawFile(caseStmt.expr, snippetInfo.Parent, e.Message);
                }
            }
            else
            {
                DrawFile(caseStmt.expr, snippetInfo.Parent, caseStmt.expr?.Error);
            }

            if (!string.IsNullOrEmpty(caseStmt.Error))
            {
                DrawFile(caseStmt, snippetInfo.Parent, caseStmt.Error);
                snippetInfo.IsCurrent = false;
                snippetInfo.IsGrammarError = true;
            }

            isAccepted = true;
        }

        private void RepeatAst(ASTRepeat repeatStmt, SnippetInfo snippetInfo, ref bool isAccepted)
        {

            var expr = repeatStmt.expr as ASTExpr;
            if (expr == null)
            {
                DrawFile(repeatStmt.expr, snippetInfo.Parent, repeatStmt.expr?.Error, Colors.Red);
            }
            else
            {
                CheckCond(expr, snippetInfo, isAccepted);
            }

            if (snippetInfo.IsGrammarError) return;
            var stmts = repeatStmt.stmts;
            if (stmts != null)
            {
                var subAccepted = isAccepted;
                ParseNode(snippetInfo, stmts, null, true, ref subAccepted);
            }

            if (!string.IsNullOrEmpty(repeatStmt.Error))
            {
                DrawFile(repeatStmt, snippetInfo.Parent, repeatStmt.Error);
                snippetInfo.IsCurrent = false;
                snippetInfo.IsGrammarError = true;
            }

            isAccepted = true;
        }

        private void ForAst(ASTFor forStmt, SnippetInfo snippetInfo, ref bool isAccepted)
        {
            var expr = forStmt.expr as ASTExpr;
            if (expr == null)
            {
                DrawFile(forStmt.expr, snippetInfo.Parent, forStmt.expr.Error, Colors.Red);
            }
            else
            {
                var subAccepted = isAccepted;
                ConvertNodeToVariable(expr, snippetInfo, true, ref subAccepted,
                    new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
                IsNumber(expr, snippetInfo);
            }

            if (!snippetInfo.IsCurrent) return;
            var assignStmt = forStmt.assign_stmt;
            if (assignStmt != null)
            {
                var subAccepted = isAccepted;
                if (!StmtCheck(assignStmt, snippetInfo, ref subAccepted))
                {

                }
            }

            if (snippetInfo.IsGrammarError) return;
            var optional = forStmt.optional;
            if (optional is ASTError)
            {
                DrawFile(optional, snippetInfo.Parent, optional.Error);
            }
            else
            {
                var subAccepted = isAccepted;
                ConvertNodeToVariable(optional, snippetInfo, true, ref subAccepted,
                    new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
                IsNumber(optional, snippetInfo);
            }

            var stmtList = forStmt.stmt_list;
            if (stmtList != null)
            {
                var subAccepted = isAccepted;
                ParseNode(snippetInfo, stmtList, null, true, ref subAccepted);
            }

            if (!string.IsNullOrEmpty(forStmt.Error))
            {
                DrawFile(forStmt, snippetInfo.Parent, forStmt.Error);
                snippetInfo.IsCurrent = false;
                snippetInfo.IsGrammarError = true;
            }

            isAccepted = true;
        }

        private void IsNumber(ASTNode expr, SnippetInfo snippetInfo)
        {
            var astName = expr as ASTName;
            if (astName != null)
            {
                if (astName.Expr?.type?.IsBool ?? false)
                {
                    DrawFile(expr, snippetInfo.Parent,
                        $"'{ObtainValue.GetAstName(astName)}':BOOL tag not expected in expression.", Colors.Red);
                }
            }

            var astCall = expr as ASTCall;
            if (astCall != null)
            {
                if (!("sqrt".Equals(astCall.name) || "abs".Equals(astCall.name)))
                {
                    DrawFile(expr, snippetInfo.Parent,
                        $"'{astCall.name}':Unexpected.", Colors.Red);
                }
            }

        }

        private void WhileAst(ASTWhile whileStmt, SnippetInfo snippetInfo, ref bool isAccepted)
        {

            var expr = whileStmt.expr as ASTExpr;
            if (expr == null)
            {
                DrawFile(whileStmt.expr, snippetInfo.Parent, whileStmt.expr.Error, Colors.Red);
            }
            else
            {
                var subAccepted = isAccepted;
                ConvertNodeToVariable(expr, snippetInfo, true, ref subAccepted, BOOL.Inst);
                if (expr?.type != null)
                {
                    if (!expr.type.IsBool)
                    {
                        var integer = expr as ASTInteger;
                        if (integer != null)
                        {
                            if (!(integer.value == 1 || integer.value == 0))
                            {
                                DrawFile(expr, snippetInfo.Parent, "BOOL tag expected.");
                            }
                        }
                        else
                        {
                            DrawFile(expr, snippetInfo.Parent, "BOOL tag expected.");
                        }

                        //TODO(ZYL):Add other 
                    }
                }
            }

            var stmt = whileStmt.stmts;
            if (stmt != null)
            {
                var subAccepted = isAccepted;
                ParseNode(snippetInfo, stmt, null, true, ref subAccepted);
            }

            if (!string.IsNullOrEmpty(whileStmt.Error))
            {
                DrawFile(whileStmt, snippetInfo.Parent, whileStmt.Error);
                snippetInfo.IsCurrent = false;
                snippetInfo.IsGrammarError = true;
            }

            isAccepted = true;
        }

        private bool CheckAssignOtherProgramTag(ASTName ast)
        {
            try
            {
                var programItem = ast.id_list.nodes[0] as ASTNameItem;
                if (programItem.id.IndexOf("\\") < 0)
                {
                    return true;
                }

                var program = _controller.Programs[programItem.id.Substring(1)];
                if (program == _parentProgram) return true;
                if (program == null) return false;
                if (ast.id_list.nodes.Count < 2) return false;
                var name = ast.id_list.nodes[1] as ASTNameItem;
                var tag = program.Tags[name?.id];
                if (tag == null) return false;
                if (tag.Usage == Usage.InOut || tag.Usage == Usage.Local || tag.Usage == Usage.Output)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void AddErrorNode(ASTNode node, SnippetInfo snippetInfo)
        {
            if (_onlyTextMarker) return;
            var astName = node as ASTName;
            if (astName != null)
            {
                var name = new VariableInfo(node, ConvertAstNameToSnippet(astName, snippetInfo), _routine,
                    node.ContextStart, snippetInfo.Parent,
                    _textMarkerService.TextDocument.GetLocationNotVerifyAccess(node.ContextStart))
                {
                    IsUnknown = true,
                    IsDisplay = false,
                    IsCorrect = false,
                    TargetDataType = astName.ExpectDataType
                };
                if (astName.IsEnum)
                {
                    name.Enums = new List<string>();
                }

                snippetInfo.AddVariable(name);
                return;
            }

            //Debug.Assert(false,node.GetType().FullName);
        }

        //after typeChecker
        private bool StmtCheck(ASTNode astNode, SnippetInfo snippetInfo, ref bool isAccepted)
        {
            var flag = false;
            var assignStmt = astNode as ASTAssignStmt;
            if (assignStmt != null)
            {
                if (!string.IsNullOrEmpty(assignStmt.Error))
                {
                    DrawFile(assignStmt, snippetInfo.Parent, assignStmt.Error);
                    return false;
                }

                flag = true;
                bool isCorrect = true;
                if (assignStmt.name is ASTError)
                {
                    snippetInfo.ErrorInfo = assignStmt.name.Error;
                    snippetInfo.IsCurrent = false;
                    DrawFile(assignStmt.name, snippetInfo.Parent, assignStmt.name.Error);
                    isCorrect = false;
                }
                else
                {
                    try
                    {
                        if (!isAccepted || (assignStmt.name as ASTExpr)?.type == null)
                            assignStmt.name.Accept(_typeChecker);
                        bool subAccepted = true;
                        ConvertNodeToVariable(assignStmt.name, snippetInfo, false, ref subAccepted, null);
                    }
                    catch (Exception e)
                    {
                        AddErrorNode(assignStmt.name, snippetInfo);
                        Debug.WriteLine(e);
                        DrawFile(assignStmt.name, snippetInfo.Parent, e.Message);
                        isCorrect = false;
                    }
                }

                if (assignStmt.expr is ASTError)
                {
                    snippetInfo.ErrorInfo = assignStmt.expr.Error;
                    snippetInfo.IsCurrent = false;
                    DrawFile(assignStmt.expr, snippetInfo.Parent, assignStmt.expr.Error);
                    isCorrect = false;
                }
                else
                {
                    try
                    {
                        if (!isAccepted || (assignStmt.expr as ASTExpr)?.type == null)
                            assignStmt.expr.Accept(_typeChecker);
                        bool subAccepted = true;
                        ConvertNodeToVariable(assignStmt.expr, snippetInfo, false, ref subAccepted,
                            (assignStmt.name as ASTExpr)?.type);
                    }
                    catch (Exception e)
                    {
                        AddErrorNode(assignStmt.expr, snippetInfo);
                        Debug.WriteLine(e);
                        DrawFile(assignStmt.expr, snippetInfo.Parent, e.Message);
                        isCorrect = false;
                    }
                }

                if (!isCorrect) return false;
                var name = assignStmt.name as ASTName;
                if (name == null) return false;
                var expr = assignStmt.expr as ASTExpr;
                ASTNode error = null;
                if (!AssignTypeCheck(name.type, expr, ref error))
                {
                    Debug.Assert(error != null);
                    if (error != null)
                    {
                        var mess = "'{1}';{0} tag expected in expression.";
                        DrawFile(error, snippetInfo.Parent,
                            string.Format(mess, name.type.Name,
                                snippetInfo.Parent.Substring(error.ContextStart,
                                    error.ContextEnd - error.ContextStart + 1)));
                    }

                    return false;
                }

                //assignStmt.Accept(_typeChecker);
                if (name.IsConstant)
                {
                    DrawFile(assignStmt.name, snippetInfo.Parent, "Routine cannot write constant tags.");
                    return false;
                }

                if ((((ASTExpr) assignStmt.expr).type?.IsAtomic ?? false))
                {
                    if (!CheckAssignOtherProgramTag(name))
                    {
                        //snippetInfo.ErrorInfo =
                        //    "Routines cannot access InOut parameters or Local tags of other programs.";
                        DrawFile(assignStmt.name, snippetInfo.Parent,
                            "Routines cannot access InOut parameters or Local tags of other programs.");
                        return false;
                    }

                    if (name.type.IsNumber)
                    {
                        if (name.Expr?.type is ArrayType)
                        {
                            //snippetInfo.ErrorInfo = "Invalid array subscript specifier.";
                            DrawFile(assignStmt.name, snippetInfo.Parent, "Invalid array subscript specifier.");
                            return false;
                        }

                        var astName = assignStmt.expr as ASTName;
                        if (astName != null)
                        {
                            if (astName.Expr?.type is ArrayType)
                            {
                                DrawFile(assignStmt, snippetInfo.Parent, $"Missing reference to array element.");
                                return false;
                            }

                            if (astName.type.IsBool)
                            {
                                DrawFile(astName, snippetInfo.Parent,
                                    $"'{snippetInfo.Parent.Substring(astName.ContextStart, astName.ContextEnd - astName.ContextStart + 1)}':BOOL tag not expected in expression.");
                                return false;
                            }
                        }

                        ParseNode(snippetInfo,assignStmt.expr,name.type,false,ref isAccepted);
                        return true;
                    }

                    if (name.type.IsBool)
                    {
                        if (assignStmt.expr is ASTUnaryOp)
                        {
                            UnaryOpAst((ASTUnaryOp)assignStmt.expr,snippetInfo,BOOL.Inst,false,ref isAccepted);
                            return true;
                        }

                        if (assignStmt.expr is ASTFloat)
                        {
                            DrawFile(assignStmt.expr, snippetInfo.Parent, $"bool tag excepted.");
                            return false;
                        }

                        var astInter = assignStmt.expr as ASTInteger;
                        if (astInter != null)
                        {
                            if ((astInter.value == 0 || astInter.value == 1))
                                return true;
                            //snippetInfo.ErrorInfo = "bool tag excepted.";
                            DrawFile(assignStmt.expr, snippetInfo.Parent, "bool tag excepted.");
                            return false;
                        }

                        var astName = assignStmt.expr as ASTName;
                        if (astName != null)
                        {
                            if (astName.Expr?.type is ArrayType)
                            {
                                //snippetInfo.ErrorInfo = "bool tag excepted.";
                                DrawFile(assignStmt.expr, snippetInfo.Parent, "bool tag excepted.");
                                return false;
                            }

                            if (astName.type.IsBool) return true;
                            if (astName.type.IsInteger || astName.type.IsReal)
                            {
                                //snippetInfo.ErrorInfo = "bool tag excepted.";
                                DrawFile(assignStmt.expr, snippetInfo.Parent, "bool tag excepted.");
                                return false;
                            }
                        }

                        var binOp = assignStmt.expr as ASTBinOp;
                        if (binOp != null)
                        {
                            var subAccepted = isAccepted;
                            BinOpAst(binOp,snippetInfo,BOOL.Inst,false,ref subAccepted);
                            return true;
                        }

                        var call = assignStmt.expr as ASTCall;
                        if (call != null)
                        {
                            DrawFile(assignStmt.expr, snippetInfo.Parent, $"'{call.instr?.Name}':BOOL tag expected.");
                            return false;
                        }
                    }

                    if (name.type.IsUDIDefinedType ||
                        name.type.IsStringType ||
                        name.type.FamilyType == FamilyType.StringFamily ||
                        name.type.IsStruct ||
                        name.type is AOIDataType)
                    {
                        //snippetInfo.ErrorInfo = "Invalid data type.Argument must parameter data type.";
                        DrawFile(assignStmt.name, snippetInfo.Parent,
                            "Invalid data type.Argument must parameter data type.");
                        return false;
                    }

                    //TODO(zyl):TO Check call
                    //var astCall = assign.expr as ASTCall;
                    return true;
                }
                else if (((ASTExpr) assignStmt.expr).type?.IsBool ?? false)
                {
                    if (name.type.IsBool)
                    {
                        if ((assignStmt.expr as ASTName)?.Expr?.type is ArrayType)
                        {
                            //snippetInfo.ErrorInfo = "Invalid array subscript specifier.";
                            DrawFile(assignStmt.expr, snippetInfo.Parent, "Invalid array subscript specifier.");
                            return false;
                        }

                        return true;
                    }

                    if (name.type.IsInteger || name.type.IsReal)
                    {
                        var astInter = assignStmt.expr as ASTInteger;
                        if (astInter != null)
                        {
                            if ((astInter.value == 0 || astInter.value == 1))
                                return true;
                            DrawFile(assignStmt.expr, snippetInfo.Parent, "Bool expression expected.");
                            return false;
                        }

                        var astName = assignStmt.expr as ASTName;
                        if (astName != null)
                        {
                            if (astName?.Expr?.type == null)
                            {
                                astName.Accept(_typeChecker);
                            }

                            if (astName.Expr.type is ArrayType)
                            {
                                //snippetInfo.ErrorInfo = "Miss reference to array element.";
                                DrawFile(assignStmt.expr, snippetInfo.Parent, "Miss reference to array element.");
                                return false;
                            }

                            if (astName.type.IsInteger) return true;
                            DrawFile(assignStmt.expr, snippetInfo.Parent, "BOOL tag not expected in expression.");
                            //snippetInfo.ErrorInfo = "Invalid data type.Argument must parameter data type.";
                            return false;
                        }

                        var astBin = assignStmt.expr as ASTBinOp;
                        if (astBin != null)
                        {
                            var subAccepted = isAccepted;
                            BinOpAst(astBin, snippetInfo, (assignStmt.name as ASTExpr)?.type, false, ref subAccepted);
                            return true;
                        }

                        DrawFile(assignStmt, snippetInfo.Parent, "BOOL tag not expected in expression.");
                        return false;
                    }
                }
                else if (((ASTExpr) assignStmt.expr).type?.FamilyType == FamilyType.StringFamily ||
                         ((ASTExpr) assignStmt.expr).type?.FamilyType == FamilyType.StringFamily)
                {
                    DrawFile(assignStmt, snippetInfo.Parent,
                        "Invalid data type.Argument must match parameter data type.");
                    return false;
                }
                else if (((ASTExpr) assignStmt.expr).type is CompositiveType)
                {
                    DrawFile(assignStmt, snippetInfo.Parent,
                        $"'{ObtainValue.GetAstName(assignStmt.expr as ASTName)}':{((ASTExpr) assignStmt.name).type?.Name} tag expected in expression.");
                    return false;
                }
                else
                {
                    Debug.Assert(false, ((ASTExpr) assignStmt.expr).type?.ToString() ?? "");
                }

            }

            var astStore = astNode as ASTStore;
            if (astStore != null)
            {
                ASTExpr expr = astStore.value;
                if (expr is ASTTypeConv)
                {
                    expr = ((ASTTypeConv) expr).expr;
                }

                var assign = new ASTAssignStmt(astStore.addr.name, expr);
                isAccepted = true;
                return StmtCheck(assign, snippetInfo, ref isAccepted);
            }

            var astPair = astNode as ASTPair;
            if (astPair != null)
            {
                var left = astPair.left;
                if (left != null)
                {
                    var subAccepted = isAccepted;
                    ParseNode(snippetInfo, left, null, true, ref subAccepted);
                }

                var right = astPair.right;
                if (right != null)
                {
                    var subAccepted = isAccepted;
                    ParseNode(snippetInfo, right, null, true, ref subAccepted);
                }

                flag = true;
                return true;
            }

            var astBinRelOp = astNode as ASTBinRelOp;
            if (astBinRelOp != null)
            {
                flag = true;
                ParseNode(snippetInfo, astNode, BOOL.Inst, true, ref isAccepted);
                if (astBinRelOp.left.type.IsNumber && astBinRelOp.right.type.IsNumber) return true;
                DrawFile(astBinRelOp, snippetInfo.Parent, "Invalid data type.Argument must parameter data type.");
                //snippetInfo.ErrorInfo = "Invalid data type.Argument must parameter data type.";
                return true;
            }

            var astBinArithOp = astNode as ASTBinArithOp;
            if (astBinArithOp != null)
            {
                flag = true;
                ParseNode(snippetInfo, astNode, new ExpectType(DINT.Inst, REAL.Inst), true, ref isAccepted);
                return true;
            }

            var astBinLogicOp = astNode as ASTBinLogicOp;
            if (astBinLogicOp != null)
            {
                flag = true;
                ParseNode(snippetInfo, astNode, new ExpectType(DINT.Inst, REAL.Inst), true, ref isAccepted);
                return true;
            }

            if (!flag)
            {
                //AstCheck(astNode, snippetInfo, parserTree);
                ParseNode(snippetInfo, astNode, null, true, ref isAccepted);
            }

            return true;
        }

        private bool AssignTypeCheck(IDataType expectedType, ASTExpr expr, ref ASTNode errorNode)
        {
            if (expr != null)
            {
                var astBinLogicOp = expr as ASTBinLogicOp;
                if (astBinLogicOp != null)
                {
                    #region AstName

                    {
                        var left = astBinLogicOp.left as ASTName;
                        if (left != null)
                        {
                            if (!left.type.Equal(expectedType))
                            {
                                errorNode = left;
                                return false;
                            }
                        }

                        var right = astBinLogicOp.right as ASTName;
                        if (right != null)
                        {
                            if (!right.type.Equal(expectedType))
                            {
                                errorNode = right;
                                return false;
                            }
                        }
                    }

                    #endregion

                    #region AstInteger

                    {
                        if (astBinLogicOp.left is ASTFloat || astBinLogicOp.right is ASTFloat)
                        {
                            if (expectedType.IsBool)
                            {
                                errorNode = astBinLogicOp.left is ASTFloat ? astBinLogicOp.left : astBinLogicOp.right;
                                return false;
                            }
                        }

                        var left = astBinLogicOp.left as ASTInteger;
                        if (left != null)
                        {
                            if (left.value == 0 || left.value == 1)
                            {
                                if (!(expectedType.IsBool || expectedType.IsInteger || expectedType.IsReal))
                                {
                                    errorNode = left;
                                    return false;
                                }
                            }
                        }

                        var right = astBinLogicOp.right as ASTInteger;
                        if (right != null)
                        {
                            if (right.value == 0 || right.value == 1)
                            {
                                if (!(expectedType.IsBool || expectedType.IsInteger || expectedType.IsReal))
                                {
                                    errorNode = right;
                                    return false;
                                }
                            }
                        }
                    }

                    #endregion

                    var leftBinLogicOp = astBinLogicOp.left as ASTBinLogicOp;
                    if (leftBinLogicOp != null)
                    {
                        if (!AssignTypeCheck(expectedType, leftBinLogicOp, ref errorNode))
                            return false;
                    }

                    var rightBinLogicOp = astBinLogicOp.right as ASTBinLogicOp;
                    if (rightBinLogicOp != null)
                    {
                        return AssignTypeCheck(expectedType, rightBinLogicOp, ref errorNode);
                    }
                }
            }

            return true;
        }

    }
}
