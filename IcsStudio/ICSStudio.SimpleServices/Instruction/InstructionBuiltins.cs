using ICSStudio.SimpleServices.Compiler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSStudio.SimpleServices.Instruction
{
    internal class InstructionBuiltins
    {
        /*
        res.AddNode(new ASTDup(BOOL.Inst));
        res.AddNode(ASTNode.CreateLIRIf(new ASTDup(BOOL.Inst), new ASTStore(new ASTNameAddr(tag), new ASTInteger(0)), new ASTNop()));
    &*/    
        public static void XICLogic(MacroAssembler assembler)
        {
            assembler.Pop();
        }

        public static void OTULogic(MacroAssembler assembler)
        {
            assembler.Dup();
            assembler.Dup();
            var label = new MacroAssembler.Label();
            assembler.JeqL(label);

            
            
            assembler.BiPush(1);

        }

    }
}
