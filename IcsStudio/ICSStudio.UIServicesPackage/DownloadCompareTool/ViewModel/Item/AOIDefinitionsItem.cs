using System.Collections.Generic;
using System.Linq;
using ICSStudio.DownloadChange;

namespace ICSStudio.UIServicesPackage.DownloadCompareTool.ViewModel.Item
{
    class AOIDefinitionsItem:CompareItem
    {
        private readonly List<IDiffItem> _aoiDefinitions;

        public AOIDefinitionsItem(List<IDiffItem> aoiDefinitions)
        {
            _aoiDefinitions = aoiDefinitions;

            Title = "AOIDefinitions";

            BuildChildren();
        }

        private void BuildChildren()
        {
            Children = new List<CompareItem>();

            if (_aoiDefinitions != null && _aoiDefinitions.Count > 0)
            {
                foreach (var aoiDefinition in _aoiDefinitions.OrderBy(x => x.ChangeType))
                {
                    if (aoiDefinition.ChangeType == ItemChangeType.Unchanged)
                        continue;

                    var aoiName = aoiDefinition.ChangeType == ItemChangeType.Added
                        ? aoiDefinition.NewValue["Name"]?.ToString()
                        : aoiDefinition.OldValue["Name"]?.ToString();

                    var aoiItem = new CompareItem { Title = aoiName, DiffItem = aoiDefinition };

                    Children.Add(aoiItem);

                    ItemType = CompareItemType.Modified;
                }
            }
        }
    }
}
