using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.PredefinedType;

namespace ICSStudio.SimpleServices.Instruction.Instructions
{
    internal static class CheckInstrParamType
    {
        public static bool  CheckMotionParam(ASTNameAddr instr)
        {
            var type = instr?.type as RefType;
            var refType = type?.type;
            if( refType == MOTION_INSTRUCTION.Inst)return true;
            return false;
        }
    }
}
