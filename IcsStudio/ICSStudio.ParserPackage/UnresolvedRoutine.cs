using System.Collections.Generic;
using ICSStudio.Interfaces.Common;
using ICSStudio.UIInterfaces.Parser;

namespace ICSStudio.ParserPackage
{
    internal class UnresolvedRoutine : IUnresolvedRoutine
    {
        public UnresolvedRoutine(IRoutine routine)
        {
            Routine = routine;
        }

        public IRoutine Routine { get; }

        public object ASTNode { get; set; }

        public List<IInstructionSymbol> Instructions { get; set; }

        public List<IParameterSymbol> Parameters { get; set; }


        public long ParseTime { get; set; }
    }
}
