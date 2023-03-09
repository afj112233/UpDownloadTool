using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace ICSStudio.Interfaces.Common
{
    public enum RoutineType
    {
        Typeless,
        RLL,
        FBD,
        SFC,
        ST,
        External,
        Sequence,
        Encrypted,
    }

    public enum SheetSize
    {
        [EnumMember(Value = "Letter - 8.5 x 11 in")]
        Letter,

        [EnumMember(Value = "Legal - 8.5 x 14 in")]
        Legal,

        [EnumMember(Value = "Tabloid - 11 x 17 in")]
        Tabloid,

        [EnumMember(Value = "A - 8.5 x 11 in")]
        A,
        [EnumMember(Value = "B - 11 x17 in")] B,
        [EnumMember(Value = "C - 17 x 22 in")] C,
        [EnumMember(Value = "D - 22 x 34 in")] D,
        [EnumMember(Value = "E - 34 x 44 in")] E,

        [EnumMember(Value = "A4 - 210 x 297 mm")]
        A4,

        [EnumMember(Value = "A3 - 297 x 420 mm")]
        A3,

        [EnumMember(Value = "A2 - 420 x 594 mm")]
        A2,

        [EnumMember(Value = "A1 - 594 x 841 mm")]
        A1,

        [EnumMember(Value = "A0 - 841 x 1189 mm")]
        A0
    }

    public enum Orientation
    {
        Landscape,
        Portrait
    }
    
    public interface IRoutine : IBaseComponent, ITrackedComponent, ISourceProtected
    {
        IRoutineCollection ParentCollection { get; set; }

        bool IsMainRoutine { get; set; }

        bool IsFaultRoutine { get; set; }

        RoutineType Type { get; }

        bool IsAoiObject { get; }

        bool ControllerEditsExist { get; }

        bool PendingEditsExist { get; }

        bool HasEditAccess { get; }

        bool HasEditContentAccess { get; }

        bool IsEncrypted { get; }

        //TODO(gjc): IsDeleted???
        bool IsAbandoned { get; set; }

        IRoutineCode Routine { get; }

        bool IsError { get; set; }
        
        void GenCode(IProgramModule program);
        JObject ConvertToJObject(bool useCode);
        bool IsCompiling { get; }

        List<IRoutine> GetJmpRoutines();
    }
}
