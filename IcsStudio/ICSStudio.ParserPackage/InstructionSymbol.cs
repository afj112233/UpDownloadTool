using System;
using System.Collections.Generic;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Instruction;
using ICSStudio.UIInterfaces.Parser;

namespace ICSStudio.ParserPackage
{
    internal class InstructionSymbol : IInstructionSymbol
    {
        public int Kind { get; }
        public string Name { get; set; }

        public bool IsAOI => Instruction is AoiDefinition.AOIInstruction;

        public int Row { get; set; }
        public int Col { get; set; }

        public IXInstruction Instruction { get; set; }

        public List<IParameterSymbol> Parameters { get; set; }
    }
}
