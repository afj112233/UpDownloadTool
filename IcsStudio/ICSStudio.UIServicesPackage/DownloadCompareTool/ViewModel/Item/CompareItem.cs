using System;
using System.Collections.Generic;
using ICSStudio.DownloadChange;

namespace ICSStudio.UIServicesPackage.DownloadCompareTool.ViewModel.Item
{
    internal enum CompareItemType
    {
        Unchanged,
        Added,
        Deleted,
        Modified
    }

    internal enum CompareOriginalType
    {
        General,
        Tag,
        AOI,
        DataType,
        Module,
        Program,
        Task
    }

    internal class CompareItem
    {
        private CompareItemType _itemType = CompareItemType.Unchanged;

        public string Title { get; set; }

        public CompareOriginalType OriginalType { get; set; }

        public List<CompareItem> Children { get; set; }

        public IDiffItem DiffItem { get; set; }

        public CompareItemType ItemType
        {
            get
            {
                if (DiffItem != null)
                {
                    switch (DiffItem.ChangeType)
                    {
                        case ItemChangeType.Unchanged:
                            return CompareItemType.Unchanged;
                        case ItemChangeType.Added:
                            return CompareItemType.Added;
                        case ItemChangeType.Deleted:
                            return CompareItemType.Deleted;
                        case ItemChangeType.Modified:
                            return CompareItemType.Modified;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                return _itemType;
            }
            set{ if (DiffItem == null) _itemType = value; }
        }
    }
}
