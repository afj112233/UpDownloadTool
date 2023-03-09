using System.Diagnostics;
using Antlr4.Runtime.Misc;
using ICSStudio.SimpleServices.PredefinedType;
using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Antlr4.Runtime.Tree;

namespace ICSStudio.SimpleServices.Compiler
{
    public class STASTGenVisitor : STGrammarBaseVisitor<ASTNode>
    {
        public override ASTNode VisitStart([NotNull] STGrammarParser.StartContext context)
        {
            OnVisitStart();
            return new ASTStmtMod(Visit(context.stmt_list()) as ASTNodeList)
            {
                ContextStart = context.Start.StartIndex,
                ContextEnd = context.Stop?.StopIndex ?? 0,
                RegionLableNodeList = regionLableNodes
            };
        }

        private void OnVisitStart()
        {
            regionLableNodes = new ASTNodeList();
        }

        public override ASTNode VisitStmtListEmpty([NotNull] STGrammarParser.StmtListEmptyContext context)
        {
            return new ASTNodeList()
                {ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0};
        }

        public override ASTNode VisitStmtList([NotNull] STGrammarParser.StmtListContext context)
        {
            var stmtList = context.stmt();

            ASTNodeList ret = new ASTNodeList();
            foreach (var statement in stmtList)
            {
                var parsedStmt = Visit(statement);
                if (statement.exception != null)
                {
                    if (parsedStmt != null && string.IsNullOrEmpty(parsedStmt.Error))
                    {
                        parsedStmt.Error = $"'{statement.exception.OffendingToken.Text}':Unexpected.";
                        parsedStmt.ErrorStart = statement.exception.OffendingToken.StartIndex;
                        parsedStmt.ErrorEnd = parsedStmt.ErrorStart + statement.exception.OffendingToken.Text.Length;
                    }
                }
                //Debug.Assert(parsedStmt != null);
                //maybe stmt is ';'
                if (parsedStmt != null)
                {
                    Debug.Assert(parsedStmt is ASTStmt || parsedStmt is ASTError);
                    ret.AddNode(parsedStmt);
                }
            }

            return ret;
        }

        //public override ASTNode VisitErrorInstrStmt([NotNull] STGrammarParser.ErrorInstrStmtContext context)
        //{
        //    return null;
        //}
        public override ASTNode VisitStmtAssign([NotNull] STGrammarParser.StmtAssignContext context)
        {
            var error = "";
            if (context.SEMICOLON() == null || context.SEMICOLON() is ErrorNodeImpl)
            {
                error = "Statement not terminated by ';'";
            }

            var assignStmt = Visit(context.assign_stmt());
            if (string.IsNullOrEmpty(error))
            {
                error = assignStmt.Error;
            }

            if (string.IsNullOrEmpty(error))
            {
                var text = context.GetText();
                var stmt = context.assign_stmt().GetText();
                text = text.Substring(stmt.Length);
                text = text.Substring(0, text.Length - 1);
                if (!string.IsNullOrEmpty(text))
                {
                    error = $"'{text}':Unexpected.";
                }
            }

            assignStmt.ContextStart = context.Start.StartIndex;
            assignStmt.ContextEnd = context.Stop?.StopIndex ?? 0;
            if (string.IsNullOrEmpty(assignStmt.Error))
                assignStmt.Error = error;
            return assignStmt;
        }

        public override ASTNode VisitStmtInstr([NotNull] STGrammarParser.StmtInstrContext context)
        {
            return Visit(context.instr_stmt());
        }

        public override ASTNode VisitStmtIf([NotNull] STGrammarParser.StmtIfContext context)
        {
            return Visit(context.if_stmt());
        }

        public override ASTNode VisitStmtCase([NotNull] STGrammarParser.StmtCaseContext context)
        {
            return Visit(context.case_stmt());
        }

        public override ASTNode VisitStmtFor([NotNull] STGrammarParser.StmtForContext context)
        {
            return Visit(context.for_stmt());
        }

        public override ASTNode VisitStmtRepeat([NotNull] STGrammarParser.StmtRepeatContext context)
        {
            return Visit(context.repeat_stmt());
        }

        public override ASTNode VisitStmtWhile([NotNull] STGrammarParser.StmtWhileContext context)
        {
            return Visit(context.while_stmt());
        }

        public override ASTNode VisitStmtExit([NotNull] STGrammarParser.StmtExitContext context)
        {
            return new ASTExit()
                {ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0};
        }

        public override ASTNode VisitStmtEmpty([NotNull] STGrammarParser.StmtEmptyContext context)
        {
            return null;
        }

        public override ASTNode VisitAssignStmt([NotNull] STGrammarParser.AssignStmtContext context)
        {
            var item = Visit(context.item());
            var op = context.op.Type;
            var expr = context.expr() == null ? new ASTError(null) : Visit(context.expr());
            var error = "";

            if (context.op.Text == "<missing ':='>")
            {
                error = "':=' expected.";
            }

            if (context.MINUS() != null)
            {
                error = "Unexpected '-'.";
            }

            var text = context.GetText();
            if (!string.IsNullOrEmpty(text))
            {
                if (text.IndexOf("<missing ')'>") > -1)
                {
                    error = "<missing ')'>";
                }

                if (text.IndexOf("<missing '('>") > -1)
                {
                    error = "<missing '('>";
                }
            }

            Debug.Assert(item is ASTName || item is ASTError||item is ASTEmpty);
            Debug.Assert(expr is ASTExpr || expr is ASTError || expr is ASTEmpty);
            if (item is ASTError) item.Error = "':=' unexpected.";
            else if (item is ASTEmpty)
            {
                error = "Invalid expression.";
            }
            if (expr is ASTError)
            {
                expr.ContextStart = context.Start?.StartIndex ?? 0;
                expr.ContextEnd = context.Stop?.StopIndex ?? 0;
                if (string.IsNullOrEmpty(expr.Error))
                {
                    expr.Error = "Invalid expr.";
                }
            }
            else if (expr is ASTEmpty)
            {
                error = "Invalid expression.";
            }

            AssignType tp = AssignType.NORMAL;
            if (op == STGrammarLexer.NRASSIGN)
            {
                tp = AssignType.NONRETENTIVE;
            }

            int errorStart = 0, errorEnd = 0;
            if (context.error_stmt() != null)
            {
                var errorStmt = context.error_stmt();
                error = $"'{errorStmt.GetText()}':Unexpected.";
                errorStart = errorStmt.Start.StartIndex;
                errorEnd = errorStmt.Stop.StopIndex;
            }

            return new ASTAssignStmt(item, expr, tp)
            {
                ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0, Error = error,
                ErrorStart = errorStart, ErrorEnd = errorEnd
            };
        }

        public override ASTNode VisitExprXor([NotNull] STGrammarParser.ExprXorContext context)
        {
            if (context.xor_expr() == null)
            {
                return new ASTError(null)
                {
                    Error = "Invalid Expr", ContextStart = context?.Start?.StartIndex ?? 0,
                    ContextEnd = context?.Stop?.StopIndex ?? 0
                };
            }

            var expr = Visit(context.xor_expr());
            if (expr == null)
                expr = new ASTEmpty()
                {
                    ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                    Error = context.exception?.Message
                };
            return expr;
        }

        private ASTNode VisitExpressionWithErrorProtect(ParserRuleContext context, string errString = "Invalid expression.")
        {
            var visitResult = Visit(context);
            if (!(visitResult is ASTExpr))
            {
                visitResult = new ASTError(visitResult)
                {
                    ContextStart = context.Start?.StartIndex ?? 0,
                    ContextEnd = context.Stop?.StopIndex ?? 0,
                    Error = errString
                };
            }
            return visitResult;
        }
        
        public override ASTNode VisitExprExpr([NotNull] STGrammarParser.ExprExprContext context)
        {
            if (context?.expr() == null || context?.xor_expr() == null)
                return new ASTError(null) {Error = "Invalid Expr"};

            var left = VisitExpressionWithErrorProtect(context.expr());
            var right = VisitExpressionWithErrorProtect(context.xor_expr());

            return new ASTBinLogicOp(ASTBinOp.Op.OR, left as ASTExpr, right as ASTExpr)
            {
                ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                Error = context.exception?.Message
            };
        }

        public override ASTNode VisitXorExprAnd([NotNull] STGrammarParser.XorExprAndContext context)
        {
            return Visit(context.and_expr());
        }

        public override ASTNode VisitXorExpr([NotNull] STGrammarParser.XorExprContext context)
        {
            var left = VisitExpressionWithErrorProtect(context.xor_expr());
            var right = VisitExpressionWithErrorProtect(context.and_expr());

            return new ASTBinLogicOp(ASTBinOp.Op.XOR, left as ASTExpr, right as ASTExpr)
            {
                ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                Error = context.exception?.Message
            };
        }

        public override ASTNode VisitAndExpr([NotNull] STGrammarParser.AndExprContext context)
        {
            var left = VisitExpressionWithErrorProtect(context.and_expr());
            var right = VisitExpressionWithErrorProtect(context.eq_expr());

            return new ASTBinLogicOp(ASTBinOp.Op.AND, left as ASTExpr, right as ASTExpr)
            {
                ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                Error = context.exception?.Message
            };
        }

        public override ASTNode VisitAndExprEq([NotNull] STGrammarParser.AndExprEqContext context)
        {
            return Visit(context.eq_expr());
        }

        public override ASTNode VisitEqExpr([NotNull] STGrammarParser.EqExprContext context)
        {
            var op = ASTBinOp.Op.NOP;
            if (context.op.Type == STGrammarLexer.NEQ)
            {
                op = ASTBinOp.Op.NEQ;
            }
            else if (context.op.Type == STGrammarLexer.EQ)
            {
                op = ASTBinOp.Op.EQ;
            }
            else
            {
                Debug.Assert(false, context.op.Type.ToString());
            }

            var left = VisitExpressionWithErrorProtect(context.eq_expr());
            var right = VisitExpressionWithErrorProtect(context.cmp_expr());

            return new ASTBinRelOp(op, left as ASTExpr, right as ASTExpr)
            {
                ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                Error = context.exception?.Message
            };
        }

        public override ASTNode VisitEqExprCmp([NotNull] STGrammarParser.EqExprCmpContext context)
        {
            return Visit(context.cmp_expr());
        }

        public override ASTNode VisitCmpExprAdd([NotNull] STGrammarParser.CmpExprAddContext context)
        {
            return Visit(context.add_expr());
        }

        public override ASTNode VisitCmpExpr([NotNull] STGrammarParser.CmpExprContext context)
        {
            var left = VisitExpressionWithErrorProtect(context.cmp_expr());
            var right = VisitExpressionWithErrorProtect(context.add_expr());

            ASTBinOp.Op op;
            if (context.op.Type == STGrammarLexer.LT)
            {
                op = ASTBinOp.Op.LT;
            }
            else if (context.op.Type == STGrammarLexer.LE)
            {
                op = ASTBinOp.Op.LE;
            }
            else if (context.op.Type == STGrammarLexer.GT)
            {
                op = ASTBinOp.Op.GT;
            }
            else if (context.op.Type == STGrammarLexer.GE)
            {
                op = ASTBinOp.Op.GE;
            }
            else
            {
                Debug.Assert(false);
                op = ASTBinOp.Op.NOP;
            }

            return new ASTBinRelOp(op, left as ASTExpr, right as ASTExpr)
            {
                ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                Error = context.exception?.Message
            };
        }

        public override ASTNode VisitAddExprMul([NotNull] STGrammarParser.AddExprMulContext context)
        {
            return Visit(context.mul_expr());
        }

        public override ASTNode VisitAddExpr([NotNull] STGrammarParser.AddExprContext context)
        {
            var left = VisitExpressionWithErrorProtect(context.add_expr());
            var right = VisitExpressionWithErrorProtect(context.mul_expr());

            ASTBinOp.Op op = ASTBinOp.Op.NOP;
            if (context.op.Type == STGrammarLexer.PLUS)
            {
                op = ASTBinOp.Op.PLUS;
            }
            else if (context.op.Type == STGrammarLexer.MINUS)
            {
                op = ASTBinOp.Op.MINUS;
            }

            return new ASTBinArithOp(op, left as ASTExpr, right as ASTExpr)
            {
                ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                Error = context.exception?.Message
            };
        }

        public override ASTNode VisitMulExpr([NotNull] STGrammarParser.MulExprContext context)
        {
            var left = VisitExpressionWithErrorProtect(context.mul_expr());
            var right = VisitExpressionWithErrorProtect(context.not_expr());

            ASTBinOp.Op op = ASTBinOp.Op.NOP;
            if (context.op.Type == STGrammarLexer.TIMES)
            {
                op = ASTBinOp.Op.TIMES;
            }
            else if (context.op.Type == STGrammarLexer.DIVIDE)
            {
                op = ASTBinOp.Op.DIVIDE;
            }
            else if (context.op.Type == STGrammarLexer.MOD)
            {
                op = ASTBinOp.Op.MOD;
            }

            return new ASTBinArithOp(op, left as ASTExpr, right as ASTExpr)
            {
                ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                Error = context.exception?.Message
            };
        }

        public override ASTNode VisitMulExprNot([NotNull] STGrammarParser.MulExprNotContext context)
        {
            return Visit(context.not_expr());
        }

        public override ASTNode VisitNotExpr([NotNull] STGrammarParser.NotExprContext context)
        {
            var expr = VisitExpressionWithErrorProtect(context.not_expr());

            return new ASTUnaryOp(ASTUnaryOp.Op.NOT, expr as ASTExpr)
            {
                ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                Error = context.exception?.Message??expr.Error
            };
        }

        public override ASTNode VisitNotExprNeg([NotNull] STGrammarParser.NotExprNegContext context)
        {
            return Visit(context.neg_expr());
        }

        public override ASTNode VisitNegExpr([NotNull] STGrammarParser.NegExprContext context)
        {
            ASTUnaryOp.Op op = ASTUnaryOp.Op.NOP;
            if (context.op.Type == STGrammarLexer.MINUS)
            {
                op = ASTUnaryOp.Op.NEG;
            }
            else if (context.op.Type == STGrammarLexer.PLUS)
            {
                op = ASTUnaryOp.Op.PLUS;
            }
            else
            {
                Debug.Assert(false);
            }
            
            var expr = VisitExpressionWithErrorProtect(context.neg_expr());
            if (expr is ASTInteger)
            {
                if (op == ASTUnaryOp.Op.NEG)
                {
                    return new ASTInteger(-((ASTInteger) expr).value)
                    {
                        ContextStart = context.Start?.StartIndex ?? 0,
                        ContextEnd = context.Stop?.StopIndex ?? 0,
                        Error = context.exception?.Message ?? expr.Error
                    };
                }
                else if(op==ASTUnaryOp.Op.PLUS)
                {
                    return new ASTInteger(((ASTInteger)expr).value)
                    {
                        ContextStart = context.Start?.StartIndex ?? 0,
                        ContextEnd = context.Stop?.StopIndex ?? 0,
                        Error = context.exception?.Message ?? expr.Error
                    };
                }
            }else if (expr is ASTFloat)
            {
                if (op == ASTUnaryOp.Op.NEG)
                {
                    return new ASTFloat(-((ASTFloat)expr).value)
                    {
                        ContextStart = context.Start?.StartIndex ?? 0,
                        ContextEnd = context.Stop?.StopIndex ?? 0,
                        Error = context.exception?.Message ?? expr.Error
                    };
                }
                else if (op == ASTUnaryOp.Op.PLUS)
                {
                    return new ASTFloat(((ASTFloat)expr).value)
                    {
                        ContextStart = context.Start?.StartIndex ?? 0,
                        ContextEnd = context.Stop?.StopIndex ?? 0,
                        Error = context.exception?.Message ?? expr.Error
                    };
                }
            }

            return new ASTUnaryOp(op, expr as ASTExpr)
            {
                ContextStart = context.Start?.StartIndex ?? 0,
                ContextEnd = context.Stop?.StopIndex ?? 0,
                Error = context.exception?.Message ?? expr.Error
            };
        }

        public override ASTNode VisitNegExprPow([NotNull] STGrammarParser.NegExprPowContext context)
        {
            return Visit(context.pow_expr());
        }

        public override ASTNode VisitPowExprFunc([NotNull] STGrammarParser.PowExprFuncContext context)
        {
            return Visit(context.func_expr());
        }

        public override ASTNode VisitPowExpr([NotNull] STGrammarParser.PowExprContext context)
        {
            var left = VisitExpressionWithErrorProtect(context.pow_expr());
            var right = VisitExpressionWithErrorProtect(context.func_expr());

            return new ASTBinArithOp(ASTBinOp.Op.POW, left as ASTExpr, right as ASTExpr)
            {
                ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                Error = context.exception?.Message
            };
        }

        public override ASTNode VisitFuncExprFunc([NotNull] STGrammarParser.FuncExprFuncContext context)
        {
            ASTUnaryOp.Op op = ASTUnaryOp.Op.NOP;
            if (context.MINUS() != null)
            {
                op = ASTUnaryOp.Op.NEG;
            }
            else if (context.PLUS() != null)
            {
                op = ASTUnaryOp.Op.PLUS;
            }
            else
            {
                Debug.Assert(false);
            }

            var expr = Visit(context.func_expr());
            if (expr is ASTInteger)
            {
                if (op == ASTUnaryOp.Op.NEG)
                {
                    return new ASTInteger(-((ASTInteger)expr).value)
                    {
                        ContextStart = context.Start?.StartIndex ?? 0,
                        ContextEnd = context.Stop?.StopIndex ?? 0,
                        Error = context.exception?.Message ?? expr.Error
                    };
                }
                else if (op == ASTUnaryOp.Op.PLUS)
                {
                    return new ASTInteger(((ASTInteger)expr).value)
                    {
                        ContextStart = context.Start?.StartIndex ?? 0,
                        ContextEnd = context.Stop?.StopIndex ?? 0,
                        Error = context.exception?.Message ?? expr.Error
                    };
                }
            }
            else if (expr is ASTFloat)
            {
                if (op == ASTUnaryOp.Op.NEG)
                {
                    return new ASTFloat(-((ASTFloat)expr).value)
                    {
                        ContextStart = context.Start?.StartIndex ?? 0,
                        ContextEnd = context.Stop?.StopIndex ?? 0,
                        Error = context.exception?.Message ?? expr.Error
                    };
                }
                else if (op == ASTUnaryOp.Op.PLUS)
                {
                    return new ASTFloat(((ASTFloat)expr).value)
                    {
                        ContextStart = context.Start?.StartIndex ?? 0,
                        ContextEnd = context.Stop?.StopIndex ?? 0,
                        Error = context.exception?.Message ?? expr.Error
                    };
                }
            }

            return new ASTUnaryOp(op, expr as ASTExpr)
            {
                ContextStart = context.Start?.StartIndex ?? 0,
                ContextEnd = context.Stop?.StopIndex ?? 0,
                Error = context.exception?.Message ?? expr.Error
            };
        }

        public override ASTNode VisitFuncExpr([NotNull] STGrammarParser.FuncExprContext context)
        {
            var param_list = Visit(context.param_list());
            var error = "";
            if (context.RPAREN().Length > 1)
            {
                error = "')':Unexpected.";
            }

            if (string.IsNullOrEmpty(error))
                error = context.exception?.Message;
            return new ASTCall(context.ID().GetText(), param_list as ASTNodeList)
            {
                ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0, Error = error
            };
        }

        public override ASTNode VisitFuncExprPrim([NotNull] STGrammarParser.FuncExprPrimContext context)
        {
            return Visit(context.prim_expr());
        }

        public override ASTNode VisitPrimExprFloat([NotNull] STGrammarParser.PrimExprFloatContext context)
        {
            var text = context.FLOAT().GetText();
            var result = VerityNumber(ref text);
            if (!result)
                return new ASTError(context)
                {
                    ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                    Error = $"'{text}' Unexpected."
                };
            return new ASTFloat(System.Convert.ToDouble(text))
            {
                ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                Context = text, Error = context.exception?.Message
            };
        }

        public override ASTNode VisitPrimExprBitSel([NotNull] STGrammarParser.PrimExprBitSelContext context)
        {
            //return Visit(context.BITSEL());
            var text = context.BITSEL().GetText();
            var result = VerityNumber(ref text);
            if (!result)
                return new ASTError(context)
                {
                    ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                    Error = $"'{text}' Unexpected."
                };
            return new ASTFloat(System.Convert.ToDouble(text))
            {
                ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                Context = text, Error = context.exception?.Message
            };
        }

        public override ASTNode VisitPrimExprInteger([NotNull] STGrammarParser.PrimExprIntegerContext context)
        {
            return Visit(context.integer());
            //return new ASTInteger(System.Convert.ToInt32(context.integer()));
        }

        public override ASTNode VisitPrimExprItem([NotNull] STGrammarParser.PrimExprItemContext context)
        {
            return Visit(context.item());
        }

        public override ASTNode VisitPrimExprExpr([NotNull] STGrammarParser.PrimExprExprContext context)
        {
            var error = "";
            if (context.RPAREN().Length > 1)
            {
                error = "')':Unexpected.";
            }

            var node = Visit(context.expr());
            if (node != null)
                node.Error = error;
            return node;
        }

        public override ASTNode VisitItemItem([NotNull] STGrammarParser.ItemItemContext context)
        {
            if (context == null) return new ASTError(null) {Error = "Invalid item"};
            var id = new ASTNameItem(context.ID().GetText(), null)
                {ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0};
            var id_sel_list = Visit(context.id_sel_list()) as ASTNodeList;
            id_sel_list.InsertNode(id);
            var bitSel = Visit(context.bit_sel());
            return new ASTName(id_sel_list, bitSel as ASTExpr)
            {
                ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                Error = bitSel?.Error
            };
        }

        public override ASTNode VisitItemArray([NotNull] STGrammarParser.ItemArrayContext context)
        {
            try
            {
                if (context == null) return new ASTError(null) {Error = "Invalid ArrayItem"};
                var id = new ASTNameItem(context.ID().GetText(), Visit(context.array_sel_list()) as ASTNodeList)
                    {ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0};
                var id_sel_list = Visit(context.id_sel_list()) as ASTNodeList;
                id_sel_list.InsertNode(id);
                var bitSle = Visit(context.bit_sel());
                return new ASTName(id_sel_list, bitSle as ASTExpr)
                {
                    ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                    Error = bitSle?.Error
                };
            }
            catch (Exception)
            {
                return new ASTError(context)
                {
                    ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                    Error = $"{context.GetText()}:Unexpected."
                };
            }
        }

        public override ASTNode VisitIDSelListEmpty([NotNull] STGrammarParser.IDSelListEmptyContext context)
        {
            return new ASTNodeList()
                {ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0};
        }

        public override ASTNode VisitIDSelList([NotNull] STGrammarParser.IDSelListContext context)
        {
            var id_sel_list = Visit(context.id_sel_list()) as ASTNodeList;
            var id_sel = Visit(context.id_sel());
            id_sel_list.AddNode(id_sel);
            return id_sel_list;
        }

        public override ASTNode VisitIDSel([NotNull] STGrammarParser.IDSelContext context)
        {
            return new ASTNameItem(context.IDSEL().GetText().Substring(1), null)
                {ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0};
        }

        public override ASTNode VisitIDSelArray([NotNull] STGrammarParser.IDSelArrayContext context)
        {
            return new ASTNameItem(context.IDSEL().GetText().Substring(1),
                    Visit(context.array_sel_list()) as ASTNodeList)
                {ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0};
        }

        public override ASTNode VisitArraySelListExpr([NotNull] STGrammarParser.ArraySelListExprContext context)
        {
            var res = Visit(context.expr());
            var list = new ASTNodeList();
            list.AddNode(res);
            list.Line = context.expr().Start.Line;
            return list;
        }

        public override ASTNode VisitArraySelList([NotNull] STGrammarParser.ArraySelListContext context)
        {
            var list = Visit(context.array_sel_list()) as ASTNodeList;
            var expr = Visit(context.expr());
            list.AddNode(expr);
            list.Line = context.array_sel_list().Start.Line;
            return list;
        }

        public override ASTNode VisitBitSel([NotNull] STGrammarParser.BitSelContext context)
        {
            return new ASTInteger(BigInteger.Parse(context.BITSEL().GetText().Substring(1)))
                {ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0};
        }

        public override ASTNode VisitErrorBitSel([NotNull] STGrammarParser.ErrorBitSelContext context)
        {
            return new ASTError(context)
            {
                ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                Error = $"'{context.GetText()}':Unexpected."
            };
        }

        public override ASTNode VisitBitSelExpr([NotNull] STGrammarParser.BitSelExprContext context)
        {
            return Visit(context.expr());
        }

        public override ASTNode VisitBitSelEmpty([NotNull] STGrammarParser.BitSelEmptyContext context)
        {
            return null;
        }

        public override ASTNode VisitParamListExpr([NotNull] STGrammarParser.ParamListExprContext context)
        {
            var list = new ASTNodeList()
                {ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0};
            list.AddNode(Visit(context.expr()));
            return list;
        }
        
        public override ASTNode VisitParamList([NotNull] STGrammarParser.ParamListContext context)
        {
            var list = Visit(context.param_list()) as ASTNodeList;
            list.AddNode(Visit(context.expr()));
            return list;
        }

        public override ASTNode VisitInstrStmt([NotNull] STGrammarParser.InstrStmtContext context)
        {
            var error = "";
            if (context.SEMICOLON() == null || context.SEMICOLON() is ErrorNodeImpl)
            {
                error = "Statement not terminated by ';'";
            }

            if (context.RPAREN() == null || context.RPAREN().Length == 0)
            {
                error = "Missing right parenthesis ')' or invalid expression.";
            }

            if (context.RPAREN().Length > 1)
            {
                error = "')':Unexpected.";
            }

            var paramNodes = string.IsNullOrEmpty(context.param_list().GetText())
                ? new ASTNodeList()
                {
                    ContextStart = context.param_list().Start?.StartIndex ?? 0,
                    ContextEnd = context.param_list().Stop?.StopIndex ?? 0
                }
                : Visit(context.param_list()) as ASTNodeList;
            
            var name = context.ID().GetText();
            return new ASTInstr(name, paramNodes)
            {
                Error = error, ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0
            };
        }
        public override ASTNode VisitErrorInstrStmt([NotNull] STGrammarParser.ErrorInstrStmtContext context)
        {
            var error = "";
            var name = context.ID().GetText();
            error = $"'{name}':Missing right parenthesis ')' or invalid expression.";

            var paramNodes = Visit(context.param_list()) as ASTNodeList;
            return new ASTInstr(name, paramNodes)
            {
                Error = error,
                ContextStart = context.Start?.StartIndex ?? 0,
                ContextEnd = context.Stop?.StopIndex ?? 0
            };
        }

        private void ParseErrorNodeImpl(List<ErrorNodeImpl> errorNodeImpls, ref string error, ref int errorStart,
            ref int errorEnd)
        {
            if (errorNodeImpls.Any())
            {
                var errorNodeImpl = errorNodeImpls[0];
                error = errorNodeImpl.GetText();
                if (error.StartsWith("<"))
                    error = error.Substring(0, error.Length - 1).Substring(1);
                if (!error.StartsWith("Missing", StringComparison.OrdinalIgnoreCase))
                {
                    error = $"'{error}':Unexpected.";
                    if (Math.Abs(errorNodeImpl.Symbol.StartIndex - errorNodeImpl.Symbol.StopIndex)>0)
                    {
                        errorStart = errorNodeImpl.Symbol.StartIndex;
                        errorEnd = errorNodeImpl.Symbol.StopIndex;
                    }
                }
                else
                {
                    errorStart = errorNodeImpl.Symbol.StartIndex;
                    errorEnd = errorNodeImpl.Symbol.StopIndex;
                }
            }
        }

        public override ASTNode VisitIfStmt([NotNull] STGrammarParser.IfStmtContext context)
        {
            ParserRuleContext parserRuleContext = null;
            int errorStart = 0, errorEnd = 0;
            try
            {
                var error = "";
                if (context.exception != null && !"<EOF>".Equals(context.exception.OffendingToken.Text))
                {
                    errorStart = context.exception.OffendingToken.StartIndex;
                    errorEnd = errorStart + context.exception.OffendingToken.Text.Length;
                    error = $"'{context.exception.OffendingToken.Text}':Unexpected.";
                }

                ParseErrorNodeImpl(context.children.OfType<ErrorNodeImpl>().ToList(), ref error, ref errorStart,
                    ref errorEnd);

                ASTNode expr = Visit(context.expr());
                if (expr is ASTError)
                {
                    if (string.IsNullOrEmpty(expr.Error))
                    {
                        expr.Error = "Bool (conditional) expression expected.";
                    }
                }
                else if (expr is ASTEmpty)
                {
                    error = "Bool (conditional) expression expected.";
                    errorStart = context.IF().Symbol.StartIndex;
                    errorEnd = context.IF().Symbol.StopIndex;
                }
                else if (expr == null)
                {
                    Debug.Assert(context.expr() != null);
                    error = $"Unexpected grammar.";
                    errorStart = context.expr().Start.StartIndex;
                    errorEnd = context.expr().Stop.StopIndex;
                }

                if (string.IsNullOrEmpty(error))
                {
                    if (context.SEMICOLON() == null || context.SEMICOLON() is ErrorNodeImpl)
                    {
                        error = "Statement not terminated by ';'";
                    }

                    if (context.END_IF() == null || context.END_IF() is ErrorNodeImpl)
                    {
                        error = "Keyword 'END_IF' expected.";
                        errorStart = context.IF().Symbol.StartIndex;
                        errorEnd = context.IF().Symbol.StopIndex;
                    }

                    if (context.THEN() == null || context.THEN() is ErrorNodeImpl)
                    {
                        error = "Keyword 'THEN' expected.";
                        errorStart = context.expr().Start.StartIndex;
                        errorEnd = context.expr().Stop.StopIndex;
                    }
                }

                parserRuleContext = context.stmt_list();
                ASTNode stmt = null;
                try
                {
                    stmt = Visit(parserRuleContext);
                }
                catch (Exception)
                {
                    stmt = new ASTError(parserRuleContext)
                    {
                        ContextStart = (parserRuleContext ?? context).Start?.StartIndex ?? 0,
                        ContextEnd = (parserRuleContext ?? context).Stop?.StopIndex ?? 0,
                        Error = "'IF' error component."
                    };
                }

                parserRuleContext = context.elsif_stmt_list();
                ASTNode elsif = null;
                try
                {
                    elsif = Visit(parserRuleContext);
                }
                catch (Exception)
                {
                    elsif = new ASTError(parserRuleContext)
                    {
                        ContextStart = (parserRuleContext ?? context).Start?.StartIndex ?? 0,
                        ContextEnd = (parserRuleContext ?? context).Stop?.StopIndex ?? 0,
                        Error = "'IF' error component."
                    };
                }

                parserRuleContext = context.if_else_stmt();
                ASTNode ifElse = null;
                try
                {
                    ifElse = Visit(parserRuleContext);
                    if (parserRuleContext.exception != null)
                    {
                        if (ifElse == null)
                        {
                            if (errorStart<=0&&errorEnd<=0)
                            {
                                errorStart = parserRuleContext.exception.OffendingToken.StartIndex;
                                errorEnd = parserRuleContext.exception.OffendingToken.StopIndex;
                                error = parserRuleContext.exception.OffendingToken.Text.Contains("EOF")
                                    ? $"Keyword 'END_IF' expected."
                                    : $"'{parserRuleContext.exception.OffendingToken.Text}':Unexpected.";
                                if (parserRuleContext.exception.OffendingToken.Text.Contains("EOF"))
                                {
                                    errorStart = context.IF().Symbol.StartIndex;
                                    errorEnd = context.IF().Symbol.StopIndex;
                                }
                            }
                        }
                        else
                        {
                            if (parserRuleContext.exception.OffendingToken.Text.Contains("EOF"))
                            {
                                errorStart = context.IF().Symbol.StartIndex;
                                errorEnd = context.IF().Symbol.StopIndex;
                                error = $"Keyword 'END_IF' expected.";
                            }
                            else
                            {
                                ifElse.Error = $"'{parserRuleContext.exception.OffendingToken.Text}':Unexpected.";
                                ifElse.ErrorStart = parserRuleContext.exception.OffendingToken.StartIndex;
                                ifElse.ErrorEnd = parserRuleContext.exception.OffendingToken.StopIndex;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    ifElse = new ASTError(parserRuleContext)
                    {
                        ContextStart = (parserRuleContext ?? context).Start?.StartIndex ?? 0,
                        ContextEnd = (parserRuleContext ?? context).Stop?.StopIndex ?? 0,
                        Error = "'IF' error component."
                    };
                }

                return new ASTIf(expr, stmt, elsif, ifElse)
                {
                    Error = error, ContextStart = context.Start?.StartIndex ?? 0,
                    ContextEnd = context.Stop?.StopIndex ?? 0, ErrorStart = errorStart, ErrorEnd = errorEnd
                };
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return new ASTError(context)
                {
                    Error = "'IF' error component.", ContextStart = context.Start?.StartIndex ?? 0,
                    ContextEnd = context.Stop?.StopIndex ?? 0
                };
            }
        }

        public override ASTNode VisitElseifStmtList([NotNull] STGrammarParser.ElseifStmtListContext context)
        {
            var list = Visit(context.elsif_stmt_list()) as ASTNodeList;
            list.ContextStart = context.Start?.StartIndex ?? 0;
            list.ContextEnd = context.Stop?.StopIndex ?? 0;
            try
            {
                var expr = Visit(context.expr());
                var stmt_list = Visit(context.stmt_list());
                var pair = new ASTPair(expr, stmt_list)
                {
                    IsNeedBoolExpr = true,
                    ContextStart = context.Start?.StartIndex ?? 0,
                    ContextEnd = context.Stop?.StopIndex ?? 0
                };
                if (expr is ASTEmpty)
                {
                    pair.Error = $"BOOL (conditional) expression expected.";
                    pair.ErrorStart = context.ELSIF().Symbol.StartIndex;
                    pair.ErrorEnd = context.ELSIF().Symbol.StopIndex;
                }
                list.AddNode(pair);
            }
            catch (Exception)
            {
                list.Error = "'IF' error component.";
            }
            finally
            {
                if (context.THEN() is ErrorNodeImpl || context.THEN() == null)
                {
                    list.Error = "Keyword 'THEN' expected.";
                    list.ErrorStart = context.ELSIF().Symbol.StartIndex;
                    list.ErrorEnd = context.ELSIF().Symbol.StopIndex;
                }
            }

            return list;
        }

        public override ASTNode VisitElseifStmtListEmpty([NotNull] STGrammarParser.ElseifStmtListEmptyContext context)
        {
            return new ASTNodeList()
                {ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0};
        }

        public override ASTNode VisitIfElseStmt([NotNull] STGrammarParser.IfElseStmtContext context)
        {
            return Visit(context.stmt_list());
        }

        public override ASTNode VisitIfElseStmtEmpty([NotNull] STGrammarParser.IfElseStmtEmptyContext context)
        {
            return new ASTNodeList()
                {ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0};
        }

        public override ASTNode VisitCaseStmt([NotNull] STGrammarParser.CaseStmtContext context)
        {
            try
            {
                var error = "";
                int errorStart = 0, errorEnd = 0;
                if (context.exception != null && !"<EOF>".Equals(context.exception.OffendingToken.Text))
                {
                    error = $"'{context.exception.OffendingToken.Text}':Unexpected.";
                    errorStart = context.exception.OffendingToken.StartIndex;
                    errorEnd = errorStart + context.exception.OffendingToken.Text.Length;
                }

                ParseErrorNodeImpl(context.children.OfType<ErrorNodeImpl>().ToList(), ref error, ref errorStart,
                    ref errorEnd);

                var expr = Visit(context.expr());
                if (expr is ASTError)
                {
                    expr.Error = "Expression expected.";
                }
                else if (expr is ASTEmpty)
                {
                    error = "Bool (conditional) expression expected.";
                    errorStart = context.CASE().Symbol.StartIndex;
                    errorEnd = context.CASE().Symbol.StopIndex;
                }
                else if (expr == null)
                {
                    Debug.Assert(context.expr() != null);
                    error = $"Unexpected grammar.";
                    errorStart = context.expr().Start.StartIndex;
                    errorEnd = context.expr().Stop.StopIndex;
                }

                if (string.IsNullOrEmpty(error))
                {
                    if (context.SEMICOLON() == null || context.SEMICOLON() is ErrorNodeImpl)
                    {
                        error = "Statement not terminated by ';'";
                    }

                    if (context.OF() == null || context.OF() is ErrorNodeImpl)
                    {
                        error = "Keyword 'OF' expected.";
                    }

                    if (context.END_CASE() == null || context.END_CASE() is ErrorNodeImpl)
                    {
                        error = "Keyword 'END_CASE' expected.";
                        errorStart = context.CASE().Symbol.StartIndex;
                        errorEnd = context.CASE().Symbol.StopIndex;
                    }
                }

                var case_list = Visit(context.case_elem_list());
                var case_else_list = Visit(context.case_else_stmt());
                if (context.case_else_stmt().exception != null)
                {
                    if (case_else_list == null)
                    {
                        if (errorStart <= 0 && errorEnd <= 0)
                        {

                            if (context.case_else_stmt().exception.OffendingToken.Text.Contains("EOF"))
                            {
                                error = $"Keyword 'END_CASE' expected.";
                                errorStart = context.CASE().Symbol.StartIndex;
                                errorEnd = context.CASE().Symbol.StopIndex;
                            }
                            else
                            {
                                errorStart = context.case_else_stmt().exception.OffendingToken.StartIndex;
                                errorEnd = context.case_else_stmt().exception.OffendingToken.StopIndex;
                                error = $"'{context.case_else_stmt().exception.OffendingToken.Text}':Unexpected.";
                            }
                        }
                    }
                    else
                    {
                        if (context.case_else_stmt().exception.OffendingToken.Text.Contains("EOF"))
                        {
                            error = $"Keyword 'END_CASE' expected.";
                            errorStart = context.CASE().Symbol.StartIndex;
                            errorEnd = context.CASE().Symbol.StopIndex;
                        }
                        else
                        {
                            case_else_list.Error =
                                $"'{context.case_else_stmt().exception.OffendingToken.Text}':Unexpected.";
                            ;
                            case_else_list.ErrorStart = context.case_else_stmt().exception.OffendingToken.StartIndex;
                            case_else_list.ErrorEnd = context.case_else_stmt().exception.OffendingToken.StopIndex;
                        }
                    }
                }

                return new ASTCase(expr, case_list as ASTNodeList,
                    case_else_list as ASTNodeList)
                {
                    Error = error, ContextStart = context.Start?.StartIndex ?? 0,
                    ContextEnd = context.Stop?.StopIndex ?? 0, ErrorStart = errorStart, ErrorEnd = errorEnd
                };
            }
            catch (Exception)
            {
                return new ASTError(context)
                {
                    Error = "'CASE' error component.", ContextStart = context.Start?.StartIndex ?? 0,
                    ContextEnd = context.Stop?.StopIndex ?? 0
                };
            }
        }

        public override ASTNode VisitCaseElemListEmpty([NotNull] STGrammarParser.CaseElemListEmptyContext context)
        {
            return new ASTNodeList()
                {ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0};
        }

        public override ASTNode VisitCaseElemList([NotNull] STGrammarParser.CaseElemListContext context)
        {
            string error = "";
            int errorStart=0, errorEnd = 0;
            ParseErrorNodeImpl(context.children.OfType<ErrorNodeImpl>().ToList(),ref error,ref errorStart,ref errorEnd);
            var list = Visit(context.case_elem_list()) as ASTNodeList;
            var node = Visit(context.case_elem());
            list.AddNode(node);
            list.Error = error;
            list.ErrorStart = errorStart;
            list.ErrorEnd = errorEnd;
            return list;
        }

        public override ASTNode VisitCaseElem([NotNull] STGrammarParser.CaseElemContext context)
        {
            string error = "";
            int errorStart = 0, errorEnd = 0;
            ParseErrorNodeImpl(context.children.OfType<ErrorNodeImpl>().ToList(), ref error, ref errorStart, ref errorEnd);
            var lhs = Visit(context.case_selector_multi());
            var rhs = Visit(context.stmt_list());
            Debug.Assert(lhs != null, context.case_selector_multi().GetText());
            Debug.Assert(rhs != null);
            return new ASTPair(lhs, rhs)
                {ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,Error = error,ErrorStart = errorStart,ErrorEnd = errorEnd};
        }

        public override ASTNode VisitCaseSelectorMulti([NotNull] STGrammarParser.CaseSelectorMultiContext context)
        {
            var list = Visit(context.case_selector_multi()) as ASTNodeList;
            var node = Visit(context.case_selector());
            list.AddNode(node);
            return list;
        }

        public override ASTNode VisitErrorCaseSelector([NotNull] STGrammarParser.ErrorCaseSelectorContext context)
        {
            return new ASTError(context)
            {
                ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                Error = $"'{context.GetText()}':Unexpected."
            };
        }

        public override ASTNode VisitErrorCaseSelectorRange(
            [NotNull] STGrammarParser.ErrorCaseSelectorRangeContext context)
        {
            var error = "";
            int errorStart = 0, errorEnd = 0;
            var selector1 = Visit(context.error_stmt(0)) as ASTErrorStmt;
            ParserRuleContext errorNode = selector1?.number == null ? context.error_stmt(0) : context.error_stmt(1);
            error = $"'{errorNode.GetText()}':Unexpected.";
            errorStart = errorNode.Start.StartIndex;
            errorEnd = errorNode.Stop.StopIndex;
            return new ASTError(context)
            {
                ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0, Error = error,
                ErrorStart = errorStart, ErrorEnd = errorEnd
            };
        }

        public override ASTNode VisitCaseSelectorMultiCaseSelector(
            [NotNull] STGrammarParser.CaseSelectorMultiCaseSelectorContext context)
        {
            var res = new ASTNodeList();
            res.AddNode(Visit(context.case_selector()));
            return res;
        }

        public override ASTNode VisitCaseSelectorNumber([NotNull] STGrammarParser.CaseSelectorNumberContext context)
        {
            return Visit(context.number());
        }

        public override ASTNode VisitCaseSelectorCaseSelectorRange(
            [NotNull] STGrammarParser.CaseSelectorCaseSelectorRangeContext context)
        {
            return Visit(context.case_selector_range());
        }

        public override ASTNode VisitCaseSelectorRange([NotNull] STGrammarParser.CaseSelectorRangeContext context)
        {
            var lhs = Visit(context.number(0));
            var rhs = Visit(context.number(1));

            Debug.Assert(lhs != null, lhs.ToString());
            Debug.Assert(rhs != null, rhs.ToString());

            return new ASTPair(lhs, rhs)
                {ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0};
        }

        public override ASTNode VisitNumberInteger([NotNull] STGrammarParser.NumberIntegerContext context)
        {
            bool negative = context.GetText().StartsWith("-");
            var node = Visit(context.integer());
            return new ASTInteger(negative ? -((ASTInteger) node).value : ((ASTInteger) node).value)
            {
                ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                Context = context.GetText()
            };
        }

        public override ASTNode VisitNumberFloat([NotNull] STGrammarParser.NumberFloatContext context)
        {
            var text = context.GetText();
            var result = VerityNumber(ref text);
            if (!result)
                return new ASTError(context)
                {
                    ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                    Error = $"'{text}' Unexpected."
                };
            return new ASTFloat(System.Convert.ToDouble(text))
            {
                ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0, Context = text
            };
        }

        public override ASTNode VisitCaseElseStmt([NotNull] STGrammarParser.CaseElseStmtContext context)
        {
            return Visit(context.stmt_list());
        }

        public override ASTNode VisitCaseElseStmtEmpty([NotNull] STGrammarParser.CaseElseStmtEmptyContext context)
        {
            return new ASTNodeList()
                {ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0};
        }

        public override ASTNode VisitForStmt([NotNull] STGrammarParser.ForStmtContext context)
        {
            try
            {
                var error = "";
                int errorStart = 0, errorEnd = 0;
                if (context.exception != null && !"<EOF>".Equals(context.exception.OffendingToken.Text))
                {
                    error = $"'{context.exception.OffendingToken.Text}':Unexpected.";
                    errorStart = context.exception.OffendingToken.StartIndex;
                    errorEnd =   context.exception.OffendingToken.StopIndex;
                    return new ASTError(context)
                    {
                        ContextStart = context.Start?.StartIndex ?? 0,
                        ContextEnd = context.Stop?.StopIndex ?? 0,
                        ErrorStart = errorStart,
                        ErrorEnd = errorEnd,
                        Error = error
                    };
                }

                ParseErrorNodeImpl(context.children.OfType<ErrorNodeImpl>().ToList(), ref error, ref errorStart,
                    ref errorEnd);

                var expr = Visit(context.expr());
                if (expr is ASTError)
                {
                    expr.Error = "Expression expected after keyword 'TO'.";
                }
                else if (expr is ASTEmpty)
                {
                    error = "Bool (conditional) expression expected.";
                    errorStart = context.FOR().Symbol.StartIndex;
                    errorEnd = context.FOR().Symbol.StopIndex;
                }
                else if (expr == null)
                {
                    Debug.Assert(context.expr() != null);
                    error = $"Unexpected grammar.";
                    errorStart = context.expr().Start.StartIndex;
                    errorEnd = context.expr().Stop.StopIndex;
                }

                if (string.IsNullOrEmpty(error))
                {

                    if (context.SEMICOLON() == null || context.SEMICOLON() is ErrorNodeImpl)
                    {
                        error = "Statement not terminated by ';'";
                    }

                    if (context.END_FOR() == null || context.END_FOR() is ErrorNodeImpl)
                    {
                        error = "Keyword 'END_FOR' expected.";
                        errorStart = context.FOR().Symbol.StartIndex;
                        errorEnd = context.FOR().Symbol.StopIndex;
                    }

                    if (context.DO() == null || context.DO() is ErrorNodeImpl)
                    {
                        error = "Keyword 'DO' expected.";
                    }

                    if (context.TO() == null || context.TO() is ErrorNodeImpl)
                    {
                        error = "Keyword 'TO' expected.";
                    }

                }

                ASTNode assign;
                if (context.assign_stmt() != null)
                {
                    assign = Visit(context.assign_stmt());
                }
                else
                {
                    assign = new ASTError(context.assign_stmt());
                    error = "'FOR' loop invalid index initialization.";
                }

                var optional = Visit(context.optional_by());
                if (optional is ASTError)
                {
                    optional.Error = "Expression expected after keyword 'By'";
                }

                var stmtList = Visit(context.stmt_list());
                if (context.stmt_list().exception != null)
                {
                    if (stmtList == null)
                    {
                        if (context.stmt_list().exception.OffendingToken.Text.Contains("EOF"))
                        {
                            error = $"Keyword 'END_FOR' expected.";
                            errorStart = context.FOR().Symbol.StartIndex;
                            errorEnd = context.FOR().Symbol.StopIndex;
                        }
                        else
                        {
                            errorStart = context.stmt_list().exception.OffendingToken.StartIndex;
                            errorEnd = context.stmt_list().exception.OffendingToken.StopIndex;
                            error = $"'{context.stmt_list().exception.OffendingToken.Text}':Unexpected.";
                        }
                    }
                    else
                    {
                        if (context.stmt_list().exception.OffendingToken.Text.Contains("EOF"))
                        {
                            error = $"Keyword 'END_FOR' expected.";
                            errorStart = context.FOR().Symbol.StartIndex;
                            errorEnd = context.FOR().Symbol.StopIndex;
                        }
                        else
                        {
                            stmtList.Error = $"'{context.stmt_list().exception.OffendingToken.Text}':Unexpected.";
                            stmtList.ErrorStart = context.stmt_list().exception.OffendingToken.StartIndex;
                            stmtList.ErrorEnd = context.stmt_list().exception.OffendingToken.StopIndex;
                        }

                    }
                }

                return new ASTFor(assign, expr, Visit(context.optional_by()), stmtList as ASTNodeList)
                {
                    Error = error, ContextStart = context.Start?.StartIndex ?? 0,
                    ContextEnd = context.Stop?.StopIndex ?? 0, ErrorStart = errorStart, ErrorEnd = errorEnd
                };
            }
            catch (Exception)
            {
                return new ASTError(context)
                {
                    Error = "'FOR' error component.", ContextStart = context.Start?.StartIndex ?? 0,
                    ContextEnd = context.Stop?.StopIndex ?? 0
                };
            }
        }

        public override ASTNode VisitOptionalBy([NotNull] STGrammarParser.OptionalByContext context)
        {
            var node = Visit(context.expr());
            if (node is ASTEmpty)
            {
                return new ASTError(context.expr())
                {
                    ContextStart = context.Start?.StartIndex ?? 0,
                    ContextEnd = context.Stop?.StopIndex ?? 0,
                    Error = "Expression expected after keyword 'BY'"
                };
            }

            node.ContextStart = context.expr().Start?.StartIndex ?? 0;
            node.ContextEnd = context.expr().Stop?.StopIndex ?? 0;
            return node;
        }

        public override ASTNode VisitOptionalByEmpty([NotNull] STGrammarParser.OptionalByEmptyContext context)
        {
            return new ASTInteger(1)
                {ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0};
        }

        public override ASTNode VisitRepeatStmt([NotNull] STGrammarParser.RepeatStmtContext context)
        {
            try
            {
                var error = "";
                int errorStart = 0, errorEnd = 0;
                if (context.exception != null && !"<EOF>".Equals(context.exception.OffendingToken.Text))
                {
                    error = $"'{context.exception.OffendingToken.Text}':Unexpected.";
                    errorStart = context.exception.OffendingToken.StartIndex;
                    errorEnd = errorStart + context.exception.OffendingToken.Text.Length;
                    return new ASTError(context)
                    {
                        ContextStart = context.Start?.StartIndex ?? 0,
                        ContextEnd = context.Stop?.StopIndex ?? 0,
                        ErrorStart = errorStart,
                        ErrorEnd = errorEnd,
                        Error = error
                    };
                }

                ParseErrorNodeImpl(context.children.OfType<ErrorNodeImpl>().ToList(), ref error, ref errorStart,
                    ref errorEnd);

                if (context.SEMICOLON() == null || context.SEMICOLON() is ErrorNodeImpl)
                {
                    error = "Statement not terminated by ';'";
                }

                if (context.END_REPEAT() == null || context.END_REPEAT() is ErrorNodeImpl)
                {
                    error = "Keyword 'END_REPEAT' expected.";
                    errorStart = context.REPEAT().Symbol.StartIndex;
                    errorEnd = context.REPEAT().Symbol.StopIndex;
                }

                if (context.UNTIL() == null || context.UNTIL() is ErrorNodeImpl)
                {
                    error = "Keyword 'UNTIL' expected.";
                }

                ASTNode expr = null;
                try
                {
                    expr = Visit(context.expr());
                }
                catch (Exception)
                {
                    if (context.expr() != null)
                    {
                        expr = new ASTError(context.expr())
                        {
                            ContextStart = context.expr().Start.StartIndex, ContextEnd = context.expr().Stop.StopIndex,
                            Error = "BOOL(conditional) expression expected."
                        };
                    }
                    else
                    {
                        error = "Unexpected grammar.";
                        errorStart = context.REPEAT().Symbol.StartIndex;
                        errorEnd = context.REPEAT().Symbol.StopIndex;
                    }
                }

                if (expr is ASTError)
                {
                    expr.Error = "BOOL(conditional) expression expected.";
                }

                ASTNode stmt = null;
                try
                {
                    stmt = Visit(context.stmt_list());
                }
                finally
                {
                    if (context.stmt_list().exception != null)
                    {
                        if (stmt == null)
                        {
                            if (context.stmt_list().exception.OffendingToken.Text.Contains("EOF"))
                            {
                                error = $"Keyword 'END_REPEAT' expected.";
                                errorStart = context.REPEAT().Symbol.StartIndex;
                                errorEnd = context.REPEAT().Symbol.StopIndex;
                            }
                            else
                            {
                                errorStart = context.stmt_list().exception.OffendingToken.StartIndex;
                                errorEnd = context.stmt_list().exception.OffendingToken.StopIndex;
                                error = $"'{context.stmt_list().exception.OffendingToken.Text}':Unexpected.";
                            }
                        }
                        else
                        {
                            if (context.stmt_list().exception.OffendingToken.Text.Contains("EOF"))
                            {
                                error = $"Keyword 'END_REPEAT' expected.";
                                errorStart = context.REPEAT().Symbol.StartIndex;
                                errorEnd = context.REPEAT().Symbol.StopIndex;
                            }
                            else
                            {
                                stmt.Error = $"'{context.stmt_list().exception.OffendingToken.Text}':Unexpected.";
                                stmt.ErrorStart = context.stmt_list().exception.OffendingToken.StartIndex;
                                stmt.ErrorEnd = context.stmt_list().exception.OffendingToken.StopIndex;
                            }
                        }
                    }
                }

                return new ASTRepeat(stmt as ASTNodeList, expr)
                {
                    Error = error, ContextStart = context.Start?.StartIndex ?? 0,
                    ContextEnd = context.Stop?.StopIndex ?? 0, ErrorStart = errorStart, ErrorEnd = errorEnd
                };
            }
            catch (Exception)
            {
                return new ASTError(context)
                {
                    Error = "'REPEAT' error component.", ContextStart = context.Start?.StartIndex ?? 0,
                    ContextEnd = context.Stop?.StopIndex ?? 0
                };
            }
        }


        public override ASTNode VisitWhileStmt([NotNull] STGrammarParser.WhileStmtContext context)
        {
            try
            {
                var error = "";
                int errorStart = 0, errorEnd = 0;
                if (context.exception != null && !"<EOF>".Equals(context.exception.OffendingToken.Text))
                {
                    errorStart = context.exception.OffendingToken.StartIndex;
                    errorEnd = errorStart + context.exception.OffendingToken.Text.Length;
                    error = $"'{context.exception.OffendingToken.Text}':Unexpected.";
                    return new ASTError(context)
                    {
                        ContextStart = context.Start?.StartIndex ?? 0,
                        ContextEnd = context.Stop?.StopIndex ?? 0,
                        ErrorStart = errorStart,
                        ErrorEnd = errorEnd,
                        Error = error
                    };
                }

                ParseErrorNodeImpl(context.children.OfType<ErrorNodeImpl>().ToList(), ref error, ref errorStart,
                    ref errorEnd);

                var expr = Visit(context.expr());
                if (expr is ASTError)
                {
                    expr.Error = "BOOL(conditional) expression expected.";
                }
                else if (expr is ASTEmpty)
                {
                    error = "BOOL(conditional) expression expected.";
                    errorStart = context.WHILE().Symbol.StartIndex;
                    errorEnd = context.WHILE().Symbol.StopIndex;
                }
                else if (expr == null)
                {
                    Debug.Assert(context.expr() != null);
                    error = $"Unexpected grammar.";
                    errorStart = context.expr().Start.StartIndex;
                    errorEnd = context.expr().Stop.StopIndex;
                }

                if (string.IsNullOrEmpty(error))
                {
                    if (context.SEMICOLON() == null || context.SEMICOLON() is ErrorNodeImpl)
                    {
                        error = "Statement not terminated by ';'";
                    }

                    if (context.END_WHILE() == null || context.END_WHILE() is ErrorNodeImpl)
                    {
                        error = "Keyword 'END_WHILE' expected.";
                        errorStart = context.WHILE().Symbol.StartIndex;
                        errorEnd = context.WHILE().Symbol.StopIndex;
                    }

                    if (context.DO() == null || context.DO() is ErrorNodeImpl)
                    {
                        error = "Keyword 'DO' expected.";
                    }
                }

                ASTNode stmt = null;
                try
                {
                    stmt = Visit(context.stmt_list());
                }
                finally
                {
                    if (context.stmt_list().exception != null)
                    {
                        if (stmt == null)
                        {
                            if (context.stmt_list().exception.OffendingToken.Text.Contains("EOF"))
                            {
                                error = $"Keyword 'END_WHILE' expected.";
                                errorStart = context.WHILE().Symbol.StartIndex;
                                errorEnd = context.WHILE().Symbol.StopIndex;
                            }
                            else
                            {
                                errorStart = context.stmt_list().exception.OffendingToken.StartIndex;
                                errorEnd = context.stmt_list().exception.OffendingToken.StopIndex;
                                error = $"'{context.stmt_list().exception.OffendingToken.Text}':Unexpected.";
                            }
                        }
                        else
                        {
                            if (context.stmt_list().exception.OffendingToken.Text.Contains("EOF"))
                            {
                                error = $"Keyword 'END_WHILE' expected.";
                                errorStart = context.WHILE().Symbol.StartIndex;
                                errorEnd = context.WHILE().Symbol.StopIndex;
                            }
                            else
                            {
                                stmt.Error = $"'{context.stmt_list().exception.OffendingToken.Text}':Unexpected.";
                                stmt.ErrorStart = context.stmt_list().exception.OffendingToken.StartIndex;
                                stmt.ErrorEnd = context.stmt_list().exception.OffendingToken.StopIndex;
                            }
                        }
                    }
                }

                return new ASTWhile(expr, stmt as ASTNodeList)
                {
                    Error = error, ContextStart = context.Start?.StartIndex ?? 0,
                    ContextEnd = context.Stop?.StopIndex ?? 0, ErrorStart = errorStart, ErrorEnd = errorEnd
                };
            }
            catch (Exception)
            {
                return new ASTError(context)
                {
                    Error = "'WHILE' error component.", ContextStart = context.Start?.StartIndex ?? 0,
                    ContextEnd = context.Stop?.StopIndex ?? 0
                };
            }
        }

        public override ASTNode VisitIntegerDec([NotNull] STGrammarParser.IntegerDecContext context)
        {
            var text = context.DEC_INTEGER().GetText();
            var result = VerityNumber(ref text);
            if (!result)
                return new ASTError(context)
                {
                    ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                    Error = $"'{text}' Unexpected."
                };
            try
            {
                return new ASTInteger(BigInteger.Parse(text))
                {
                    ContextStart = context.Start?.StartIndex ?? 0,
                    ContextEnd = context.Stop?.StopIndex ?? 0,
                    Context = text
                };
            }
            catch (Exception)
            {
                return new ASTError(context)
                {
                    ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                    Error = $"Invalid Argument."
                };
            }

        }

        private bool VerityNumber(ref string number)
        {
            if (number.StartsWith("2#") || number.StartsWith("8#"))
            {
                number = number.Substring(2);
            }
            else if (number.StartsWith("16#"))
            {
                number = number.Substring(3);
            }

            if (number.StartsWith("_") || number.Contains("__")) return false;
            number = number.Replace("_", "");
            return true;
        }

        private BigInteger ParseBinary(string str)
        {
            try
            {
                var value = Convert.ToInt32(str, 2);
                return new BigInteger(value);
            }
            catch (Exception)
            {
                throw new CompilerException($"'{str}' value out of range.");
            }

            //return str.Aggregate(new BigInteger(), (b, c) => b * 2 + c - '0');
        }

        public override ASTNode VisitIntegerBin([NotNull] STGrammarParser.IntegerBinContext context)
        {
            var text = context.BIN_INTEGER().GetText();
            var result = VerityNumber(ref text);
            if (!result)
                return new ASTError(context)
                {
                    ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                    Error = $"'{text}' Unexpected."
                };
            try
            {
                return new ASTInteger(ParseBinary(text))
                {
                    ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                    Context = text
                };
            }
            catch (Exception e)
            {
                return new ASTError(context)
                {
                    ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                    Error = e.Message
                };
            }
        }

        private BigInteger ParseOct(string str)
        {
            try
            {
                var value = Convert.ToInt32(str, 8);
                return new BigInteger(value);
            }
            catch (Exception)
            {
                throw new CompilerException($"'{str}' value out of range.");
            }

            //return str.Aggregate(new BigInteger(), (b, c) => b * 8 + c - '0');
        }

        public override ASTNode VisitIntegerOct([NotNull] STGrammarParser.IntegerOctContext context)
        {
            var text = context.OCT_INTEGER().GetText();
            var result = VerityNumber(ref text);
            if (!result)
                return new ASTError(context)
                {
                    ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                    Column = context.Start?.Column??0,Line = context.Stop?.Line??0,
                    Error = $"'{text}' Unexpected."
                };
            try
            {
                return new ASTInteger(ParseOct(text))
                {
                    ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                    Context = text
                };
            }
            catch (Exception e)
            {
                return new ASTError(context)
                {
                    ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                    Error = e.Message
                };
            }
        }

        public override ASTNode VisitIntegerHex([NotNull] STGrammarParser.IntegerHexContext context)
        {
            var text = context.HEX_INTEGER().GetText();
            var result = VerityNumber(ref text);
            if (!result)
                return new ASTError(context)
                {
                    ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                    Error = $"'{text}' Unexpected."
                };
            try
            {
                var value = Convert.ToInt32(text, 16);
                return new ASTInteger(new BigInteger(value))
                {
                    ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                    Context = text
                };
            }
            catch (Exception)
            {
                return new ASTError(context)
                {
                    ContextStart = context.Start?.StartIndex ?? 0, ContextEnd = context.Stop?.StopIndex ?? 0,
                    Error = $"'{text}' value out of range."
                };
            }

            //return new ASTInteger(BigInteger.Parse("0" + text.Substring(3),
            //    NumberStyles.HexNumber))
            //    { ContextStart = context.Start?.StartIndex??0, ContextEnd = context.Stop?.StopIndex??0,Context = text };
        }

        public override ASTNode VisitINFINITYExpr([NotNull] STGrammarParser.INFINITYExprContext context)
        {
            var astFloat = new ASTFloat(float.PositiveInfinity);
            return astFloat;
        }

        public override ASTNode VisitNANExpr([NotNull] STGrammarParser.NANExprContext context)
        {
            return new ASTFloat(float.NaN);
        }

        public override ASTNode VisitEmpty([NotNull] STGrammarParser.EmptyContext context)
        {
            return null;
        }



        public override ASTNode VisitErrorStmt([NotNull] STGrammarParser.ErrorStmtContext context)
        {
            ASTName item = null;
            if (context.item() != null)
                item = Visit(context.item()) as ASTName;
            ASTExpr number = null;
            if (context.number() != null)
                number = Visit(context.number()) as ASTExpr;

            //Debug.Assert(!(item == null && number == null));
            return new ASTErrorStmt(item, number, context)
                {ContextStart = context.Start.StartIndex, ContextEnd = context.Stop.StopIndex};
        }

        public override ASTNode VisitErrorExpr([NotNull] STGrammarParser.ErrorExprContext context)
        {
            return new ASTError(context)
            {
                ContextStart = context.Start.StartIndex, ContextEnd = context.Stop.StopIndex,
                Error = $"'{context.error_stmt().GetText()}':Unexpected."
            };
        }

        public override ASTNode VisitStmtChinese(STGrammarParser.StmtChineseContext context)
        {
            return new ASTError(context)
            {
                ContextStart = context.Start.StartIndex,
                ContextEnd = context.Stop.StopIndex,
                Error = $"'{context.GetText()}':Unexpected."
            };
        }

        public override ASTNode VisitStmtUnexpected(STGrammarParser.StmtUnexpectedContext context)
        {
            var item = Visit(context.item());
            var expr = Visit(context.expr());

            return new ASTUnexpectedError(context,item,expr)
            {
                ContextStart = context.Start.StartIndex,
                ContextEnd = context.Stop.StopIndex,
                Error = $"'{context.GetText()}':Unexpected."
            };
        }

        //public override ASTNode VisitStmtUnexpectedSignal(STGrammarParser.StmtUnexpectedSignalContext context)
        //{
        //    return new ASTError(context)
        //    {
        //        ContextStart = context.Start.StartIndex,
        //        ContextEnd = context.Stop.StopIndex,
        //        Error = $"'{context.GetText()}':Unexpected."
        //    };
        //}

        private ASTNodeList regionLableNodes;

        public override ASTNode VisitStmtRegion([NotNull] STGrammarParser.StmtRegionContext context)
        {
            OnVisitRegionLable(context, RegionLableType.Region);
            return base.VisitStmtRegion(context);
        }

        public override ASTNode VisitStmtEndregion([NotNull] STGrammarParser.StmtEndregionContext context)
        {
            OnVisitRegionLable(context, RegionLableType.Endregion);
            return base.VisitStmtEndregion(context);
        }

        private void OnVisitRegionLable(STGrammarParser.Region_stmtContext context, RegionLableType type)
        {
            var text = context.GetText();
            text = text.Trim();

            var lableAstObj = new ASTRegionLable(type, text)
            {
                ContextStart = context.Start?.StartIndex ?? 0,
                ContextEnd = context.Stop?.StopIndex ?? 0,
                Error = context.exception?.Message
            };
            regionLableNodes.AddNode(lableAstObj);
        }

        public override ASTNode VisitErrorChineseExpr([NotNull] STGrammarParser.ErrorChineseExprContext context)
        {
            return new ASTError(context)
            {
                ContextStart = context.Start.StartIndex, ContextEnd = context.Stop.StopIndex,
                Error = $"'{context.GetText()}':Unexpected."
            };
        }

        //public override ASTNode VisitErrorExprExpr([NotNull] STGrammarParser.ErrorExprExprContext context)
        //{
        //    var error = context.error_stmt();
        //    return new ASTError(context)
        //    {
        //        ContextStart = error.Start.StartIndex, ContextEnd = error.Stop.StopIndex,
        //        Error = $"'{error.GetText()}':Unexpected."
        //    };
        //}

        public static ASTNode Parse(List<string> CodeText)
        {
            AntlrInputStream input = new AntlrInputStream(String.Join("\r\n", CodeText));

            STGrammarLexer lexer = new STGrammarLexer(input);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            STGrammarParser parser = new STGrammarParser(tokens);
            var tree = parser.start();
            STASTGenVisitor visitor = new STASTGenVisitor();
            var ast = visitor.Visit(tree);
            return ast;
        }

        public static ASTExpr ParseExpr(string expr)
        {
            var input = new AntlrInputStream(expr);
            var lexer = new STGrammarLexer(input);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            var parser = new STGrammarParser(tokens);
            var tree = parser.expr();
            var visitor = new STASTGenVisitor();
            var ast = visitor.Visit(tree);
            Debug.Assert(ast is ASTExpr, expr);
            return ast as ASTExpr;
        }

    }
}
