using System;
using ICSStudio.Interfaces.DeviceModule;

namespace ICSStudio.OrganizerPackage.Utilities
{
    public static class DeviceModuleExtensions
    {
        public static bool IsEmbeddedIO(this IDeviceModule deviceModule)
        {
            if (deviceModule == null)
                return false;

            if (deviceModule.CatalogNumber.StartsWith("Embedded",
                StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}
