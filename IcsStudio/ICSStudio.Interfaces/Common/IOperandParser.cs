using System;
using ICSStudio.Interfaces.Tags;

namespace ICSStudio.Interfaces.Common
{
    public interface IOperandParser : IDisposable
    {
        OperandInfo GetReducedOperandInfo(ITagCollection tagCollection, string operand);
    }
}
