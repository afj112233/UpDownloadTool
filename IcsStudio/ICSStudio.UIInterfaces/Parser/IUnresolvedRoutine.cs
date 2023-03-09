using System.Collections.Generic;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.UIInterfaces.Parser
{
    /// <summary>
    /// Represents a routine that was parsed.
    /// </summary>
    public interface IUnresolvedRoutine
    {
        IRoutine Routine { get; }
        object ASTNode { get; }

        List<IInstructionSymbol> Instructions { get; }
        List<IParameterSymbol> Parameters { get; }

        long ParseTime { get; }
    }
}
