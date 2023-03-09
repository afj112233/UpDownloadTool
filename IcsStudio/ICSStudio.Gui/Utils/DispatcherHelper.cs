using System;
using System.Windows.Threading;

namespace ICSStudio.Gui.Utils
{
    public static class DispatcherHelper
    {
        public static void Run(Dispatcher dispatcher, Action action)
        {
            Run(dispatcher, action, DispatcherPriority.Normal);
        }

        public static void Run(Dispatcher dispatcher, Action action, DispatcherPriority priority)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            if (dispatcher == null || dispatcher.CheckAccess())
                action();
            else
                dispatcher.BeginInvoke(priority, action);
        }
    }
}
