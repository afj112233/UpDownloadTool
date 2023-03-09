using System.Collections.Generic;
using System.Linq;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.EditorPackage.MonitorEditTags.Models
{
    internal class EditTagCollection : TagItemCollection<EditTagItem>
    {
        protected override EditTagItem TagToTagItem(ITag tag, IDataServer dataServer)
        {
            return EditTagItem.TagToEditTagItem(tag);
        }
        public sealed override void UpdateDataContext(AoiDataReference dataContext)
        {
            DataContext = dataContext;
        }
        /// <summary>
        /// remove in editing
        /// </summary>
        /// <param name="editTagItem"></param>
        public void RemoveEditTagItem(EditTagItem editTagItem)
        {
            if(editTagItem == null)
                return;

            List<EditTagItem> removeItems = new List<EditTagItem> { editTagItem };

            int index = 0;
            while (index < removeItems.Count)
            {
                var item = removeItems[index];

                if (item.HasChildren && item.Children != null)
                    removeItems.AddRange(item.Children.Select(t=> t as EditTagItem));

                index++;
            }

            foreach (var item in removeItems)
            {
                Remove(item);
                item.Cleanup();
            }

        }
    }
}