using System.Collections.Generic;
using System.Linq;
using ICSStudio.DownloadChange;

namespace ICSStudio.UIServicesPackage.DownloadCompareTool.ViewModel.Item
{
    internal class DataTypesItem : CompareItem
    {
        private readonly List<IDiffItem> _dataTypes;

        public DataTypesItem(List<IDiffItem> dataTypes)
        {
            _dataTypes = dataTypes;

            Title = "DataTypes";

            BuildChildren();
        }

        private void BuildChildren()
        {
            Children = new List<CompareItem>();

            if (_dataTypes != null && _dataTypes.Count > 0)
            {
                foreach (var dataType in _dataTypes.OrderBy(x => x.ChangeType))
                {
                    if (dataType.ChangeType == ItemChangeType.Unchanged)
                        continue;

                    var dataTypeName = dataType.ChangeType == ItemChangeType.Added
                        ? dataType.NewValue["Name"]?.ToString()
                        : dataType.OldValue["Name"]?.ToString();

                    var dataTypeItem = new CompareItem { Title = dataTypeName, DiffItem = dataType };

                    Children.Add(dataTypeItem);

                    ItemType = CompareItemType.Modified;
                }
            }
        }
    }
}
