using System.Collections.Generic;
using System.Linq;
using ICSStudio.DownloadChange;

namespace ICSStudio.UIServicesPackage.DownloadCompareTool.ViewModel.Item
{
    internal class ModifiedProgramItem : CompareItem
    {
        private IProgramDiffItem _diffItem;

        public ModifiedProgramItem(IProgramDiffItem diffItem)
        {
            _diffItem = diffItem;

            DiffItem = diffItem;

            Title = diffItem.OldValue["Name"]?.ToString();

            BuildChildren();
        }

        private void BuildChildren()
        {
            Children = new List<CompareItem>();

            //Properties
            if (_diffItem.Properties != null && _diffItem.Properties.ChangeType != ItemChangeType.Unchanged)
                Children.Add(new CompareItem() { Title = "Properties", DiffItem = _diffItem.Properties });

            //Tags
            var tagsItem = new CompareItem { Title = "Tags", Children = new List<CompareItem>() };
            Children.Add(tagsItem);

            if (_diffItem.Tags != null && _diffItem.Tags.Count > 0)
            {
                foreach (var tag in _diffItem.Tags.OrderBy(x => x.ChangeType))
                {
                    if (tag.ChangeType == ItemChangeType.Unchanged)
                        continue;

                    var tagName = tag.ChangeType == ItemChangeType.Added
                        ? tag.NewValue["Name"]?.ToString()
                        : tag.OldValue["Name"]?.ToString();

                    var tagItem = new CompareItem { Title = tagName, DiffItem = tag };

                    tagsItem.Children.Add(tagItem);

                    tagItem.ItemType = CompareItemType.Modified;
                }
            }

            //Routines
            var routinesItem = new CompareItem { Title = "Routines", Children = new List<CompareItem>() };
            Children.Add(routinesItem);

            if (_diffItem.Routines != null && _diffItem.Routines.Count > 0)
            {
                foreach (var routine in _diffItem.Routines.OrderBy(x => x.ChangeType))
                {
                    if (routine.ChangeType == ItemChangeType.Unchanged)
                        continue;

                    var routineName = routine.ChangeType == ItemChangeType.Added
                        ? routine.NewValue["Name"]?.ToString()
                        : routine.OldValue["Name"]?.ToString();

                    var routineItem = new CompareItem { Title = routineName, DiffItem = routine };

                    routinesItem.Children.Add(routineItem);

                    routinesItem.ItemType = CompareItemType.Modified;
                }
            }
        }
    }
}
