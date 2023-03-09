using System.Runtime.InteropServices;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.UIInterfaces.ControllerOrganizer
{

    [Guid("C7AD26F6-32F8-4713-A176-C71E54EEA4ED")]
    [ComVisible(true)]
    public interface IControllerOrganizerService
    {
        void RungsExport(RLLRoutine routine, int startIndex, int endIndex);
        void SelectOrganizerItem(IBaseObject baseObject);

        void ShowControllerOrganizerToolWindow();
    }

    [Guid("20314661-1554-40AE-A848-0D84FEC2B916")]
    public interface SControllerOrganizerService
    {

    }
}
