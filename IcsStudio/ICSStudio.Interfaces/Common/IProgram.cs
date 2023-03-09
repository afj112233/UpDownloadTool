namespace ICSStudio.Interfaces.Common
{
    public interface IProgram : IProgramModule
    {
        IProgramCollection ParentCollection { get; }

        IProgramCollection ChildCollection { get; }
        
        ITask ParentTask { get; }

        TestEditsModeType TestEditsMode { get; set; }

        bool Inhibited { get; set; }

        bool CanSchedule { get; }

        bool SynchronizeData { get; set; }

        long MaxScanTime { get; }

        long LastScanTime { get; }

        IProgram AlternateParentComponent { get; }

        void ResetMaxScanTime();

        bool CheckClientEditAccess();

    }
}
