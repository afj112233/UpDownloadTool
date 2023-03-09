namespace ICSStudio.Interfaces.Common
{
    public enum TaskType
    {
        NullType,
        Event,
        Periodic,
        Continuous,
    }

    public enum TaskTrigger
    {
        AxisHome,
        AxisRegistration1,
        AxisRegistration2,
        AxisWatch,
        ConsumedTag,
        InstructionOnly,
        ModuleInputDataStateChange,
        MotionGroupExecution,
        WindowsEvent,
        LastTrigger,
    }

    public interface ITask : IBaseComponent
    {
        ITaskCollection ParentCollection { get; }

        bool IsInhibited { get; }

        TaskType Type { get; }

        void ScheduleProgram(IProgram program);

        void UnscheduleProgram(IProgram program);

        long MaxScanTime { get; }
        long LastScanTime { get; }
        long MaxIntervalTime { get; }
        long MinIntervalTime { get; }
        int OverlapCount { get; }
    }
}
