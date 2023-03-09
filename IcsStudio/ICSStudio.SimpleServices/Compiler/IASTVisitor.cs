using System;
using Xunit;

namespace ICSStudio.SimpleServices.Compiler
{
    public interface IASTVisitor<TResult>
    {
        TResult VisitStmtMod(ASTStmtMod context);
        TResult VisitAssignStmt(ASTAssignStmt context);
        TResult VisitInstr(ASTInstr context);
        TResult VisitNodeList(ASTNodeList context);
        TResult VisitPair(ASTPair context);
        TResult VisitIf(ASTIf context);
        TResult VisitCase(ASTCase context);
        TResult VisitExit(ASTExit context);
        TResult VisitFor(ASTFor context);
        TResult VisitRepeat(ASTRepeat context);
        TResult VisitWhile(ASTWhile context);
        TResult VisitName(ASTName context);
        TResult VisitTag(ASTTag context);
        TResult VisitStackSlot(ASTStackSlot context);
        TResult VisitTagOffset(ASTTagOffset context);
        TResult VisitArrayLoader(ASTArrayLoader context);
        TResult VisitBitSelLoader(ASTBitSelLoader context);
        TResult VisitNameValue(ASTNameValue context);
        TResult VisitNameAddr(ASTNameAddr context);
        TResult VisitNullAddr(ASTNullAddr context);
        TResult VisitTempValue(ASTTempValue context);
        //TResult VisitTempAddr(ASTTempAddr context);
        TResult VisitStore(ASTStore context);
        TResult VisitFloat(ASTFloat context);
        TResult VisitInteger(ASTInteger context);
        TResult VisitUnaryOp(ASTUnaryOp context);
        TResult VisitBinRelOp(ASTBinRelOp context);
        TResult VisitBinLogicOp(ASTBinLogicOp context);
        TResult VisitBinArithOp(ASTBinArithOp context);
        TResult VisitCall(ASTCall context);
        TResult VisitRTCall(ASTRTCall context);
        TResult VisitRLLRoutine(ASTRLLRoutine context);
        TResult VisitRLLInstruction(ASTRLLInstruction context);
        TResult VisitRLLParallel(ASTRLLParallel context);
        TResult VisitRLLSequence(ASTRLLSequence context);
        TResult VisitPop(ASTPop context);
        TResult VisitNop(ASTNop context);
        TResult VisitDup(ASTDup context);
        TResult VisitTypeConv(ASTTypeConv context);
        TResult VisitLIRIf(ASTLIRIf context);
        TResult VisitRet(ASTRet context);
        TResult VisitFBDInstruction(ASTFBDInstruction context);
        TResult VisitFBDIRef(ASTFBDIRef context);
        TResult VisitFBDORef(ASTFBDORef context);
        TResult VisitFBDRoutine(ASTFBDRoutine context);
        TResult VisitPrescan(ASTPrescan context);
        TResult VisitTask(ASTTask context);
        TResult VisitStatus(ASTStatus context);
        TResult VisitError(ASTError context);
        TResult VisitEmpty(ASTEmpty context);
        TResult VisitExprTag(ASTExprTag context);
        TResult VisitExprMember(ASTExprMember context);
        TResult VisitExprArraySubscript(ASTExprArraySubscript context);
        TResult VisitExprStatus(ASTExprStatus context);
        TResult VisitExprParameter(ASTExprParameter context);
        TResult VisitExprBitSel(ASTExprBitSel context);
        TResult VisitExprConstString(ASTExprConstString context);

    }

    public abstract class IASTBaseVisitor : IASTVisitor<ASTNode>
    {
        public virtual ASTNode VisitStmtMod(ASTStmtMod context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitAssignStmt(ASTAssignStmt context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitInstr(ASTInstr context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitNodeList(ASTNodeList context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitPair(ASTPair context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitIf(ASTIf context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitCase(ASTCase context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitExit(ASTExit context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitFor(ASTFor context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitRepeat(ASTRepeat context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitWhile(ASTWhile context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitNameAddr(ASTNameAddr context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitNullAddr(ASTNullAddr context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitNameValue(ASTNameValue context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitName(ASTName context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitTag(ASTTag context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitStackSlot(ASTStackSlot context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitTagOffset(ASTTagOffset context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitArrayLoader(ASTArrayLoader context)
        {
            throw new NotImplementedException(context.ToString());
        }

        public virtual ASTNode VisitBitSelLoader(ASTBitSelLoader context)
        {
            throw new NotImplementedException(context.ToString());
        }

        public virtual ASTNode VisitFloat(ASTFloat context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitInteger(ASTInteger context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitUnaryOp(ASTUnaryOp context)
        {
            throw new NotImplementedException(context.ToString());
        }

        public virtual ASTNode VisitBinRelOp(ASTBinRelOp context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitBinLogicOp(ASTBinLogicOp context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitBinArithOp(ASTBinArithOp context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitCall(ASTCall context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitRTCall(ASTRTCall context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitRLLRoutine(ASTRLLRoutine context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitRLLInstruction(ASTRLLInstruction context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitRLLParallel(ASTRLLParallel context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitRLLSequence(ASTRLLSequence context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitPop(ASTPop context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitNop(ASTNop context)
        {
            throw new NotImplementedException(context.ToString());
        }

        public virtual ASTNode VisitDup(ASTDup context)
        {
            throw new NotImplementedException(context.ToString());
        }

        public virtual ASTNode VisitTypeConv(ASTTypeConv context)
        {
            throw new NotImplementedException(context.ToString());
        }

        public virtual ASTNode VisitTempValue(ASTTempValue context)
        {
            throw new NotImplementedException(context.ToString());
        }

        /*
        public virtual ASTNode VisitTempAddr(ASTTempAddr context)
        {
            throw new NotImplementedException(context.ToString());
        }
        */

        public virtual ASTNode VisitStore(ASTStore context)
        {
            throw new NotImplementedException(context.ToString());
        }

        public virtual ASTNode VisitLIRIf(ASTLIRIf context)
        {
            throw new NotImplementedException(context.ToString());
        }

        public virtual ASTNode VisitRet(ASTRet context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitFBDInstruction(ASTFBDInstruction context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitFBDIRef(ASTFBDIRef context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitFBDORef(ASTFBDORef context)
        {
            throw new NotImplementedException(context.ToString());
        }
        public virtual ASTNode VisitFBDRoutine(ASTFBDRoutine context)
        {
            throw new NotImplementedException(context.ToString());
        }

        public virtual ASTNode VisitPrescan(ASTPrescan context)
        {
            throw new NotImplementedException(context.ToString());
        }

        public virtual ASTNode VisitTask(ASTTask context)
        {
            throw new NotImplementedException(context.ToString());
        }

        public virtual ASTNode VisitStatus(ASTStatus context)
        {
            throw new NotImplementedException(context.ToString());
        }

        public virtual ASTNode VisitError(ASTError context)
        {
            throw new NotImplementedException();
        }

        public virtual ASTNode VisitEmpty(ASTEmpty context)
        {
            throw new NotImplementedException();
        }

        public virtual ASTNode VisitExprTag(ASTExprTag context)
        {
            throw new NotImplementedException();
        }

        public virtual ASTNode VisitExprMember(ASTExprMember context)
        {
            throw new NotImplementedException();
        }

        public virtual ASTNode VisitExprArraySubscript(ASTExprArraySubscript context)
        {
            throw new NotImplementedException();
        }

        public virtual ASTNode VisitExprStatus(ASTExprStatus context)
        {
            throw new NotImplementedException();
        }
        public virtual ASTNode VisitExprParameter(ASTExprParameter context)
        {
            throw new NotImplementedException();
        }

        public virtual ASTNode VisitExprBitSel(ASTExprBitSel context)
        {
            throw new NotImplementedException();
        }

        public virtual ASTNode VisitExprConstString(ASTExprConstString context)
        {
            throw new NotImplementedException();
        }

    }
}
