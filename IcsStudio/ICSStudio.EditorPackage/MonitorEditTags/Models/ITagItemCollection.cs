using System.Collections.Generic;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.EditorPackage.MonitorEditTags.Models
{
    public interface ITagItemCollection
    {
        AoiDataReference DataContext { get; }

        ITagCollectionContainer Scope { get; }
        IController Controller { get; }

        IDataServer DataServer { get; }

        int IndexOf(TagItem item);
        void InsertTagItems(int index, List<TagItem> listItem);

        void RemoveTagItems(List<TagItem> listItem);

    }
}