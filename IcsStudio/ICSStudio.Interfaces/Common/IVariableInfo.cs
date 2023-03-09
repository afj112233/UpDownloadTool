using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.Interfaces.Tags;

namespace ICSStudio.Interfaces.Common
{
    public interface IVariableInfo: IDisposable, ICloneable
    {
        string Name { get; }
        string Value { get; }
        bool IsRoutine { get; }
        bool IsTask { get; }
        bool IsInstr { get; }
        bool IsProgram { get; }
        bool IsModule { get; }
        bool IsAOI { get; }
        bool IsUnknown { get; }
        bool IsNum { get; }
        bool IsEnabled { get; }
        bool IsDisplay { get; }
        bool IsEnum { get; }
        bool IsUseForJSR { get; }

        bool IsJSR { get; }
        double LineOffset { get; }
        int Offset { get; }
        bool IsHit(int offset);
        ITag OriginalTag { get; }
        ITag Tag { get; }
        IDeviceModule Module { get; }
        bool IsInCode(int offset);
        int GetOffsetsWithoutProgram();

        Tuple<int, int> GetLocation();
        string GetCrossReferenceRegex();
        string GetCrossReferenceParentRegex();

        string GetParameterName(int index);
    }
}
