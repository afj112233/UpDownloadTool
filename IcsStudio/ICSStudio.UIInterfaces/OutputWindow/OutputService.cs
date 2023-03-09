using System.Runtime.InteropServices;

namespace ICSStudio.UIInterfaces.OutputWindow
{
    public enum MessageType
    {
        None,
        Error,
        Warning,
        Information,
    }

    [Guid("4394F31B-65A8-4D9C-A210-2604F1882F95")]
    [ComVisible(true)]
    public interface IOutputService
    {
        void OutputString(MessageType messageType, string message);
    }

    [Guid("CBFD8536-084B-4C01-BC54-98449410991F")]
    // ReSharper disable once InconsistentNaming
    public interface SOutputService
    {

    }

}
