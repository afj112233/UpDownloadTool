using System.ComponentModel;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.UIInterfaces.Project
{
    public interface IProject : INotifyPropertyChanged
    {
        IController Controller { get; set; }

        bool IsDirty { get; }

        bool IsEmpty { get; }

        bool Saved { get; set; }

        string RecentCommPath { get; set; }

        int Save(bool needNativeCode);

        int SaveAs(string fileName);

        void GoOffline();
    }
}
