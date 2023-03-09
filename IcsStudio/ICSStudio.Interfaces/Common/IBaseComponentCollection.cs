using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ICSStudio.Interfaces.Common
{
    public interface IBaseComponentCollection<out TBaseComponent> : IEnumerable<TBaseComponent>,
        IBaseCommon, INotifyCollectionChanged
    {
        int Count { get; }

        TBaseComponent this[int uid] { get; }

        TBaseComponent this[string name] { get; }

        TBaseComponent TryGetChildByUid(int uid);

        TBaseComponent TryGetChildByName(string name);

        ReadOnlyCollection<ComponentCoreInfo> GetComponentCoreInfoList();

        ComponentCoreInfo GetComponentCoreInfo(int uid);
    }
}
