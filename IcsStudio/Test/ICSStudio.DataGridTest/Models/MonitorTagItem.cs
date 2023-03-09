using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace ICSStudio.DataGridTest.Models
{
    public class MonitorTagItem : ViewModelBase
    {
        private bool _isExpanded;

        public MonitorTagCollection ParentCollection { get; set; }
        public MonitorTagItem ParentItem { get; set; }

        public bool IsChanged { get; set; }
        public string Name { get; set; }

        public string Placeholder
        {
            get
            {
                if (ParentItem == null)
                    return string.Empty;

                return ParentItem.Placeholder + "  ";
            }
        }

        public bool HasChildren
        {
            get
            {
                if (Children != null && Children.Count > 0)
                    return true;

                return false;
            }
        }

        public List<MonitorTagItem> Children { get; set; }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;

                if (_isExpanded)
                {
                    // insert
                    if (HasChildren)
                    {
                        if (ParentCollection != null)
                        {
                            int index = ParentCollection.IndexOf(this);
                            ParentCollection.InsertMonitorTagItems(index + 1, Children);
                        }
                    }
                }
                else
                {
                    // remove
                    ParentCollection?.RemoveMonitorTagItems(Name);
                }

                RaisePropertyChanged();
            }
        }
    }
}
