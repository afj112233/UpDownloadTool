using System.Collections.Generic;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.QuickWatchPackage.View.Models
{
    public interface ITagItemCollection
    {
        ITagCollectionContainer Scope { get; }
        IController Controller { get; }

        IDataServer DataServer { get; }

        int IndexOf(TagItem item);
        void InsertTagItems(int index, List<TagItem> listItem);

        void RemoveTagItems(List<TagItem> listItem);

    }
}