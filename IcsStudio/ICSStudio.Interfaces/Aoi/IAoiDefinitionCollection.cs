using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ICSStudio.Interfaces.Aoi
{
    public interface IAoiDefinitionCollection : IEnumerable<IAoiDefinition>,
        IDisposable,
        INotifyCollectionChanged
    {
        IEnumerable<IAoiDefinition> TrackedAoiDefinitions { get; }

        void Remove(int aoiUid);

        void Remove(string aoiName,bool isTmp=false);
        void Remove(IAoiDefinition aoi, bool isTmp = false);
    }
}
