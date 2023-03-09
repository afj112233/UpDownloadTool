using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace ICSStudio.UIServicesPackage.SelectModuleType.Common
{
    public enum FilterItemType
    {
        Category,
        Vendor
    }

    public class FilterItem : ViewModelBase
    {
        private bool _checked;

        public bool Checked
        {
            get { return _checked; }
            set { Set(ref _checked, value); }
        }

        public string Name { get; set; }

        public int No { get; set; }

        public FilterItemType Type { get; set; }
    }

    public class FilterItemComparer : IEqualityComparer<FilterItem>
    {
        public bool Equals(FilterItem x, FilterItem y)
        {
            if (x == null)
                return y == null;

            return x.No == y?.No;
        }

        public int GetHashCode(FilterItem item)
        {
            return item.No.GetHashCode();
        }
    }
}
