using System.Collections.Generic;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.Interfaces.DeviceModule
{
    public interface IDeviceModuleCollection: IBaseComponentCollection<IDeviceModule>
    {
        IEnumerable<IDeviceModule> TrackedDeviceModules { get; }
    }
}
