using System.Collections.Generic;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;

namespace ICSStudio.Interfaces.Common
{
    public interface ITrendCollection : IEnumerable<ITrend>, INotifyCollectionChanged
    {
        void Add(ITrend trend);
        void Remove(ITrend trend);
        void Clear();
        JArray ToJson();
        IController ParentController { get; }
        ITrend this[string name] { get; }
    }
}
