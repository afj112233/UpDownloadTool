using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.EditorPackage.MonitorEditTags.Models
{
    internal class MonitorTagCollection
        : TagItemCollection<MonitorTagItem>
    {
        protected override MonitorTagItem TagToTagItem(ITag tag, IDataServer dataServer)
        {
            return MonitorTagItem.TagToMonitorTagItem(tag, dataServer);
        }

        public sealed override void UpdateDataContext(AoiDataReference dataContext)
        {
            DataContext = dataContext;

            foreach (var item in Items)
            {
                item.OnPropertyChanged("IsValueEnabled");
                item.OnPropertyChanged("Value");
            }
        }
    }
}