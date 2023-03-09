using ICSStudio.Interfaces.Tags;

namespace ICSStudio.Interfaces.Common
{
    public enum ProgramType
    {
        Typeless,
        Normal,
        Phase,
        UDI,
        Sequence,
    }

    public enum TestEditsModeType
    {
        Null,
        Test,
        UnTest,
    }

    public interface IProgramModule : IBaseComponent, ITagCollectionContainer
    {
        IRoutineCollection Routines { get; }

        ProgramType Type { get; }

        string FaultRoutineName { get; }

        string MainRoutineName { get; }

        ITagDataContext GetDefaultDataContext();
        
        bool IsInMonitor { get; }
    }
}
