using ICSStudio.Interfaces.Common;
using System.Runtime.InteropServices;
using ICSStudio.Interfaces.Tags;

namespace ICSStudio.UIInterfaces.Command
{
    [Guid("15466FC9-84D4-4D01-9915-6A9F0E86A8D5")]
    [ComVisible(true)]
    public interface ICommandService
    {
        void GoOnlineOrOffline(IController controller, string commPath);
        void Upload(IController controller, string commPath);
        void Download(IController controller, string commPath);
        void ChangeOperationMode(IController controller, ControllerOperationMode mode);

        void ClearFaults(IController controller);

        bool IsReference(ITag tag);

        void DownloadSync(IController controller, string commPath);

        void UploadSync(IController controller, string commPath);

        void CorrelateSync(IController controller);

        void SaveToController(IController controller);
    }

    [Guid("A1933F7B-12E2-40B2-8D71-817711E0A001")]
    // ReSharper disable once InconsistentNaming
    public interface SCommandService
    {

    }
}
