using System.Collections.Generic;
using System.Runtime.InteropServices;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.UIInterfaces.Error
{
    public enum OrderType
    {
        None,
        Order,
        OrderByDescending
    }

    public enum Destination
    {
        None,
        ToMonitorTag,
        ToRoutineLine,
        ToTagProperty,
        ToAxisProperty,
        ToControllerOrganizer
    }

    [ComVisible(true)]
    public interface IErrorOutputService
    {
        bool CanCleanUp { get; set; }
        void AddErrors(string description,OrderType orderType,OnlineEditType onlineEditType, int? line, int? offset, object original);
        void AddWarnings(string description, object original, int? line = null, int? offset = null,Destination destination = Destination.None);
        void AddMessages(string description, object original);
        void RemoveError(object original);
        void RemoveError(IRoutine original,OnlineEditType onlineEditType);
        void RemoveWarning(object original);
        void RemoveMessage(object original);
        void RemoveImportError();
        void Cleanup();
        List<IRoutine> GetErrorRoutines();
        void Summary();
    }

    // ReSharper disable once InconsistentNaming
    public interface SErrorOutputService
    {
    }
}
