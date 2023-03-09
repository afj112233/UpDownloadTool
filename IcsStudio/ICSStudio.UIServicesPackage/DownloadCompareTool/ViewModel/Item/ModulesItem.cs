using System.Collections.Generic;
using System.Linq;
using ICSStudio.DownloadChange;

namespace ICSStudio.UIServicesPackage.DownloadCompareTool.ViewModel.Item
{
    internal class ModulesItem : CompareItem
    {
        private readonly List<IDiffItem> _modules;

        public ModulesItem(List<IDiffItem> modules)
        {
            _modules = modules;

            Title = "Modules";

            BuildChildren();
        }

        private void BuildChildren()
        {
            Children = new List<CompareItem>();

            if (_modules != null && _modules.Count > 0)
            {
                foreach (var module in _modules.OrderBy(x => x.ChangeType))
                {
                    if (module.ChangeType == ItemChangeType.Unchanged)
                        continue;

                    var moduleName = module.ChangeType == ItemChangeType.Added
                        ? module.NewValue["Name"]?.ToString()
                        : module.OldValue["Name"]?.ToString();

                    var moduleItem = new CompareItem { Title = moduleName, DiffItem = module };

                    Children.Add(moduleItem);

                    ItemType = CompareItemType.Modified;
                }
            }
        }
    }
}
