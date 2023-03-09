using System;

namespace ICSStudio.UIInterfaces.Parser
{
    public class ParseInformationEventArgs : EventArgs
    {
        public ParseInformationEventArgs(IUnresolvedRoutine oldUnresolved, IUnresolvedRoutine newUnresolved)
        {
            OldUnresolvedRoutine = oldUnresolved;
            NewUnresolvedRoutine = newUnresolved;
        }

        public IUnresolvedRoutine OldUnresolvedRoutine { get; }
        public IUnresolvedRoutine NewUnresolvedRoutine { get; }
    }
}
