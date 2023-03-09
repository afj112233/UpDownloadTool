
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Instruction;
using ICSStudio.SimpleServices.PredefinedType;
using Antlr4.Runtime;
using ICSStudio.SimpleServices.Exceptions;

namespace ICSStudio.SimpleServices.Compiler
{
    /*
    public abstract class IRLLInstruction
    {
        public readonly IInstruction instr;

        protected  IRLLInstruction(IInstruction instr)
        {
            this.instr = instr;
        }

        public abstract ASTNodeList ParseParameter(List<string> params_string);
        
        public static ASTNode ParseExpr(string param)
        {
            return STASTGenVisitor.ParseExpr(param);
        }

        public static ASTNodeList ParseExprList(List<string> params_string)
        {
            var list = new ASTNodeList();
            foreach (var param in params_string)
            {
                list.AddNode(ParseExpr(param));
            }
            return list;
        }
    }

    public class RLLInstruction : IRLLInstruction
    {

        public RLLInstruction(IInstruction instr) : base(instr)
        {
            Debug.Assert(instr != null);
        }
        public override ASTNodeList ParseParameter(List<string> params_string)
        {
            return ParseExprList(params_string);
        }
    }
    */

    /*
    public class RLLTokenInstruction : IRLLInstruction
    {
        private readonly RTInstructionInfo info;

        public RLLTokenInstruction(RTInstructionInfo info)
        {
            Debug.Assert(info != null);
            this.info = info;
        }

        public override ASTNodeList ParseParameter(List<string> params_string)
        {
            if (params_string.Count != this.info.param_types.Count - 1)
            {
                throw new RLLParserException(params_string.ToString());jj
            }

            var list = new ASTNodeList();
            foreach (var param in params_string)
            {
                list.AddNode(ParseExpr(param));
            }
            return list;
        }

    }
     */

}
