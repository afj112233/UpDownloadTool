using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSStudio.DataGridTest.Models
{
    public class MonitorTagCollection
        : ObservableCollection<MonitorTagItem>
    {
        public void AddMonitorTagItem(MonitorTagItem item)
        {
            item.ParentCollection = this;
            Add(item);
        }

        public void AddMonitorTagItems(List<MonitorTagItem> listItem)
        {
            foreach (var item in listItem)
            {
                item.ParentCollection = this;
                Add(item);
            }
        }

        public void InsertMonitorTagItem(int index, MonitorTagItem item)
        {
            if (index > Count || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (index == Count)
                AddMonitorTagItem(item);
            else
            {
                item.ParentCollection = this;
                Insert(index, item);
            }
        }


        public void InsertMonitorTagItems(int index, List<MonitorTagItem> listItem)
        {
            if (index > Count || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (index == Count)
                AddMonitorTagItems(listItem);
            else
            {
                for (int i = listItem.Count - 1; i >= 0; i--)
                {
                    listItem[i].ParentCollection = this;
                    Insert(index, listItem[i]);
                }
            }
        }

        public void RemoveMonitorTagItems(string startName)
        {
            List<MonitorTagItem> removeItems = new List<MonitorTagItem>();
            foreach (var item in Items)
            {
                if (item.Name.Length > startName.Length &&
                    item.Name.StartsWith(startName))
                {
                    removeItems.Add(item);
                }
            }

            foreach (var item in removeItems)
            {
                Remove(item);
                item.ParentCollection = null;
            }
        }
    }
}
