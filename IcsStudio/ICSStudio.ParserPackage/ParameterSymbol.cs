using ICSStudio.Interfaces.Tags;
using ICSStudio.UIInterfaces.Parser;
using ICSStudio.Utils.TagExpression;

namespace ICSStudio.ParserPackage
{
    internal class ParameterSymbol : IParameterSymbol
    {
        private readonly InstructionSymbol _parent;

        public ParameterSymbol(string name, int index, InstructionSymbol parent)
        {
            _parent = parent;

            Name = name;

            Index = index;
        }

        public int Kind { get; }
        public string Name { get; }

        public int Row => _parent?.Row ?? 0;

        public int Col => _parent?.Col ?? 0;

        public int Index { get; }

        public IInstructionSymbol Parent => _parent;

        public bool IsOperand { get; set; }

        public ITag Tag { get; set; }

        public TagExpressionBase TagExpression { get; set; }
    }
}
