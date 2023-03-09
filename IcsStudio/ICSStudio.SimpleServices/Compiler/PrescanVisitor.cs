using System.Diagnostics;
using System.ServiceModel.Channels;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.SimpleServices.Instruction;
using System;

namespace ICSStudio.SimpleServices.Compiler
{
    public class PrescanVisitor : IASTBaseVisitor
    {
        private ASTNodeList list;

        public PrescanVisitor()
        {
        }

        public ASTNode GenPrescanAST(ASTNode node)
        {
            list = new ASTNodeList();
            node.Accept(this);
            list.AddNode(new ASTRet());

            return list;
        }

        public override ASTNode VisitStmtMod(ASTStmtMod context)
        {
            context.list.Accept(this);
            return context;
        }

        public override ASTNode VisitNodeList(ASTNodeList context)
        {
            foreach (var node in context.nodes)
            {
                node.Accept(this);
            }

            return context;
        }

        public override ASTNode VisitStore(ASTStore context)
        {
            return context;
        }

        public override ASTNode VisitIf(ASTIf context)
        {
            context.then_list.Accept(this);
            foreach (var node in ((ASTNodeList)context.elsif_list).nodes)
            {
                var pair = node as ASTPair;
                pair.right.Accept(this);
            }
            context.else_list.Accept(this);
            return context;
        }

        public override ASTNode VisitFor(ASTFor context)
        {
            //context.stmt_list.Accept(this);
            return context;
        }

        public override ASTNode VisitCase(ASTCase context)
        {
            foreach (var node in context.elem_list.nodes)
            {
                var pair = node as ASTPair;
                pair?.right.Accept(this);
            }

            context.else_stmts.Accept(this);
            return context;
        }

        public override ASTNode VisitRepeat(ASTRepeat context)
        {
            //context.stmts.Accept(this);
            return context;
        }

        public override ASTNode VisitWhile(ASTWhile context)
        {
            //context.stmts.Accept(this);
            return context;
        }

        public override ASTNode VisitRLLRoutine(ASTRLLRoutine context)
        {
            context.list.Accept(this);
            return context;
        }

        public override ASTNode VisitRLLSequence(ASTRLLSequence context)
        {
            context.list.Accept(this);
            return context;
        }

        public override ASTNode VisitRLLParallel(ASTRLLParallel context)
        {
            context.list.Accept(this);
            return context;
        }

        public override ASTNode VisitRLLInstruction(ASTRLLInstruction context)
        {
            list.AddNode(new ASTPrescan(context.param_list_ast, context.instr));
            return context;
        }
        
        public override ASTNode VisitInstr(ASTInstr context)
        {
            Debug.Assert(list != null);
            list.AddNode(new ASTPrescan(context.param_list, context.instr));
            return context;
        }

        public override ASTNode VisitCall(ASTCall context)
        {
            Debug.Assert(list != null);
            list.AddNode(new ASTPrescan(context.param_list, context.instr));
            return context;
        }

        public override ASTNode VisitPair(ASTPair context)
        {
            context.left.Accept(this);
            context.right.Accept(this);
            return context;
        }

        
        public override ASTNode VisitBinRelOp(ASTBinRelOp context)
        {
            return context;
        }

        public override ASTNode VisitBinArithOp(ASTBinArithOp context)
        {
            return context;
        }

        public override ASTNode VisitBinLogicOp(ASTBinLogicOp context)
        {
            return context;
        }

        public override ASTNode VisitFBDIRef(ASTFBDIRef context)
        {
            return context;
        }

        public override ASTNode VisitExit(ASTExit context)
        {
            return context;
        }

        public override ASTNode VisitFBDInstruction(ASTFBDInstruction context)
        {
            return context;
        }
        public override ASTNode VisitFBDRoutine(ASTFBDRoutine context)
        {
            context.list.Accept(this);
            return context;
        }
        
        public override ASTNode VisitAssignStmt(ASTAssignStmt context)
        {
            return context;
        }
        
       
        public override ASTNode VisitNameAddr(ASTNameAddr context)
        {
           return context;
        }
        public override ASTNode VisitNullAddr(ASTNullAddr context)
        {
           return context;
        }
        public override ASTNode VisitNameValue(ASTNameValue context)
        {
           return context;
        }
        public override ASTNode VisitName(ASTName context)
        {
           return context;
        }
        public override ASTNode VisitTag(ASTTag context)
        {
           return context;
        }
        public override ASTNode VisitStackSlot(ASTStackSlot context)
        {
           return context;
        }
        public override ASTNode VisitTagOffset(ASTTagOffset context)
        {
           return context;
        }
        public override ASTNode VisitArrayLoader(ASTArrayLoader context)
        {
           return context;
        }

        public override ASTNode VisitBitSelLoader(ASTBitSelLoader context)
        {
           return context;
        }

        public override ASTNode VisitFloat(ASTFloat context)
        {
           return context;
        }
        public override ASTNode VisitInteger(ASTInteger context)
        {
           return context;
        }
        public override ASTNode VisitUnaryOp(ASTUnaryOp context)
        {
           return context;
        }
        
        public override ASTNode VisitRTCall(ASTRTCall context)
        {
           return context;
        }
        
        public override ASTNode VisitPop(ASTPop context)
        {
           return context;
        }
        public override ASTNode VisitNop(ASTNop context)
        {
           return context;
        }

        public override ASTNode VisitDup(ASTDup context)
        {
           return context;
        }

        public override ASTNode VisitTypeConv(ASTTypeConv context)
        {
           return context;
        }

        public override ASTNode VisitTempValue(ASTTempValue context)
        {
           return context;
        }

        /*
        public override ASTNode VisitTempAddr(ASTTempAddr context)
        {
           return context;
        }
        */
        
        public override ASTNode VisitLIRIf(ASTLIRIf context)
        {
           return context;
        }

        public override ASTNode VisitRet(ASTRet context)
        {
           return context;
        }
        
        public override ASTNode VisitFBDORef(ASTFBDORef context)
        {
           return context;
        }
        
        public override ASTNode VisitPrescan(ASTPrescan context)
        {
           return context;
        }

        public override ASTNode VisitTask(ASTTask context)
        {
           return context;
        }

        public override ASTNode VisitStatus(ASTStatus context)
        {
           return context;
        }

        public override ASTNode VisitError(ASTError context)
        {
            throw new NotImplementedException();
        }

        public override ASTNode VisitEmpty(ASTEmpty context)
        {
            return context;
        }

        public override ASTNode VisitExprTag(ASTExprTag context)
        {
            return context;
        }

        public override ASTNode VisitExprMember(ASTExprMember context)
        {
            return context;
        }

        public override ASTNode VisitExprArraySubscript(ASTExprArraySubscript context)
        {
            return context;
        }

        public override ASTNode VisitExprStatus(ASTExprStatus context)
        {
            return context;
        }
        public override ASTNode VisitExprParameter(ASTExprParameter context)
        {
            return context;
        }

        public override ASTNode VisitExprBitSel(ASTExprBitSel context)
        {
            return context;
        }

        public override ASTNode VisitExprConstString(ASTExprConstString context)
        {
            return context;
        }

    }

}