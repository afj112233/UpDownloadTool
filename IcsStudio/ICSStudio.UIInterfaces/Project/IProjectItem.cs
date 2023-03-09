using System.Collections;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.UIInterfaces.Project
{

    public enum ProjectItemType
    {
        ControllerModel,
        ControllerTags,
        FaultHandler,
        PowerHandler,

        Tasks,
        UnscheduledPrograms,
        Task,
        Program,
        ProgramTags,
        Routine,
        
        MotionGroups,
        MotionGroup,
        UngroupedAxes,

        AxisCIPDrive,
        AxisVirtual,
        CoordinateSystem,

        Assets,
        AddOnInstructions,
        DataTypes,

        UserDefineds,
        Strings,
        AddOnDefineds,
        Predefineds,
        ModuleDefineds,

        AddOnInstruction,

        UserDefined,
        String,
        AddOnDefined,
        Predefined,
        ModuleDefined,
        Trends,
        Trend,
        LogicalModelView,

        IOConfiguration,

        Bus,
        Ethernet,

        EmbeddedIO,
        ExpansionIO,

        DeviceModule,
        LocalModule,

        //
        LogicalModel,
        

        AddOnDefinedTags
    }

    public interface IProjectItem
    {
        string Name { get; }
        string DisplayName { get; }
        IProjectItems Collection { get; }
        IProjectItems ProjectItems { get; }
        ProjectItemType Kind { get;  }

        IBaseObject AssociatedObject { get; }
        void Cleanup();
    }

    public interface IProjectItems : IEnumerable
    {
        IProjectItem Parent { get; }
        int Count { get; }

        void Add(IProjectItem item);
    }
}
