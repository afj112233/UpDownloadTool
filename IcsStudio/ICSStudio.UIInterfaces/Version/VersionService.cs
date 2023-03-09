using System.Runtime.InteropServices;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.UIInterfaces.Version
{

    public class VersionCheckResult
    {
        public int Kind { get; set; }
        public string Message { get; set; }
    }

    [Guid("159F42CE-E86B-4FE2-8385-BD77D3571097")]
    [ComVisible(true)]
    public interface IVersionService
    {
        string GetCoreVersion();
        VersionCheckResult CheckControllerVersion(IController controller);
    }

    [Guid("F72EB2FD-A92D-4EAE-9966-6556C31B51BA")]
    public interface SVersionService
    {

    }
}
