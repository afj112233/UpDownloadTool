using System;

namespace ICSStudio.Interfaces.Common
{
    public delegate void IsOnlineChangedEventHandler(object sender, IsOnlineChangedEventArgs e);

    public class IsOnlineChangedEventArgs : EventArgs
    {
        public bool OldValue { get; set; }
        public bool NewValue { get; set; }
    }
}
