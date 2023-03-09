using System;
using System.Runtime.InteropServices;
using ICSStudio.Interfaces.Tags;

namespace ICSStudio.UIInterfaces.UI
{
    [Guid("2D270263-4179-443B-9211-9503E346DCCC")]
    [ComVisible(true)]
    public interface IStudioUIService
    {
        void Reset();

        event EventHandler OnReset;

        void UpdateWindowTitle();

        void Close();

        void AttachController();
        event EventHandler OnAttachController;

        void DetachController();
        event EventHandler OnDetachController;

        // for ui command
        void DeleteTag(ITag tag);
    }

    [Guid("4AA3FF97-2F85-4E12-99CD-0372881DADCF")]
    // ReSharper disable once InconsistentNaming
    public interface SStudioUIService
    {

    }
}
