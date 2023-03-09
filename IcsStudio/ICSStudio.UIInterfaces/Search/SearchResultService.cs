using ICSStudio.Interfaces.Common;
using ICSStudio.UIInterfaces.Error;

namespace ICSStudio.UIInterfaces.Search
{ 
    // ReSharper disable once InconsistentNaming
    public interface SSearchResultService
    {
    }

    public interface ISearchResultService
    {
        void AddFound(string mes,OrderType orderType,object original, OnlineEditType onlineEditType, int? line, int? offset,int? len = null);

        void AddInfo(string mes);

        void Clean();

        void FindAll();

        void FindPrevious();

        void FindNext();

        void FindSpecifiedText(string searchText);
    }
}
