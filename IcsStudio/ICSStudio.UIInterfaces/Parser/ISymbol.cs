using System.Collections.Generic;
using ICSStudio.Interfaces.Tags;
using ICSStudio.Utils.TagExpression;

namespace ICSStudio.UIInterfaces.Parser
{
    public interface ISymbol
    {
        int Kind { get; }
        string Name { get; }

        int Row { get; }
        int Col { get; }
    }

    public interface IInstructionSymbol : ISymbol
    {
        bool IsAOI { get; }
        List<IParameterSymbol> Parameters { get; }
    }

    public interface IParameterSymbol : ISymbol
    {
        IInstructionSymbol Parent { get; }

        int Index { get; }

        bool IsOperand { get; }

        ITag Tag { get; }

        TagExpressionBase TagExpression { get; }
    }
}
