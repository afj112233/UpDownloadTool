using System.Collections.Generic;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.Interfaces.DeviceModule
{
    public enum DeviceType
    {
        //TODO(gjc): change to CipDeviceType???
        NullType,
        Adapter,
        Scanner,
        ChassisIO,
        BlockIO,
        RootAC,
        RemoteController,
        PLC5C,
        Motion,
        MotionDrive,
        UnknownAOPModule,
        CIPMotionDrive,
        CIPMotionDevice,
        LastType,
    }

    public interface IDeviceModule : IBaseComponent
    {
        IDeviceModuleCollection ParentCollection { get; }

        DeviceType Type { get; }

        string DisplayText { get; }

        IEnumerable<string> ModulePath { get; }

        string CatalogNumber { get; }

        string IconPath { get; }
    }
}
