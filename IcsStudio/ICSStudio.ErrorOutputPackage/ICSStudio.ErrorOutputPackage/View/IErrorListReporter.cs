using System.Collections.ObjectModel;
using ICSStudio.Interfaces.Common;
using ICSStudio.UIInterfaces.Error;

namespace ICSStudio.ErrorOutputPackage.View
{
    public interface IErrorList
    {
        ObservableCollection<ErrorListDataEntry> DataBindingContext { get; set; }

        bool ErrorsVisible { get; set; }
        bool WarningsVisible { get; set; }
        bool MessagesVisible { get; set; }

        void ClearAll();
        void AddError(string description, OrderType orderType, OnlineEditType onlineEditType, int? line, int? offset, object original, int? len = null);
        void AddInformation(string description, object original);
        void AddWarning(string description, object original, int? line = null, int? offset = null, Destination destination = Destination.None);
        void Remove(ErrorListLevel level, object original);
    }
}
