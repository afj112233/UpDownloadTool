using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;

namespace ICSStudio.QuickWatchPackage.View.Models
{
    internal class MonitorTagItemCollection
        : TagItemCollection<MonitorTagItem>
    {
        protected override MonitorTagItem TagToTagItem(ITag tag, IDataServer dataServer, ITagItemCollection collection)
        {
            return MonitorTagItem.TagToMonitorTagItem(tag, dataServer,collection);
        }
    }
}