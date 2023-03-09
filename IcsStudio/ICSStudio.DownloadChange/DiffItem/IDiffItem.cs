using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ICSStudio.DownloadChange
{
    public enum ItemChangeType
    {
        Unchanged,
        Added,
        Deleted,
        Modified
    }

    public interface IDiffItem
    {
        ItemChangeType ChangeType { get; }
        JToken OldValue { get; }
        JToken NewValue { get; }
    }

    public interface IProgramDiffItem : IDiffItem
    {
        IDiffItem Properties { get; }

        List<IDiffItem> Tags { get; }

        List<IDiffItem> Routines { get; }
    }
}
