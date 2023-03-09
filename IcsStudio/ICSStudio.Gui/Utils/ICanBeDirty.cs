using System;

namespace ICSStudio.Gui.Utils
{
    public interface ICanBeDirty
    {
        bool IsDirty { get; }

        event EventHandler IsDirtyChanged;
    }
}
