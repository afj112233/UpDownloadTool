using System.Collections.Generic;

namespace ICSStudio.Interfaces.Common
{
    public interface IRoutineCollection: IBaseComponentCollection<IRoutine>
    {
        IProgramModule ParentProgram { get; }

        IEnumerable<IRoutine> TrackedRoutines { get; }
    }
}
