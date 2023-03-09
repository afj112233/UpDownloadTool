using System.Runtime.InteropServices;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.UIInterfaces.QuickWatch
{
    // ReSharper disable once InconsistentNaming
    public interface SQuickWatchService
    {
        
    }

    [ComVisible(true)]
    public interface IQuickWatchService
    {
        void UnlockQuickWatch();
        void LockQuickWatch();
        void AddMonitorRoutine(IRoutine routine);
        void RemoveMonitorRoutine(IRoutine routine);
        void AddScope(ITagCollectionContainer container);
        void RemoveScope(ITagCollectionContainer container);
        void AddExplicitScope(ITagCollectionContainer container);
        void RemoveExplicitScope(ITagCollectionContainer container);
        void SetAoiMonitor(IRoutine routine, AoiDataReference reference);
        void Clean();
        void Reset();
        void Show();
        void Hide();
        bool IsVisible();
    }
}
