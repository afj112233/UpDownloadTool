using System.Collections.Generic;
using System.Linq;
using ICSStudio.DownloadChange;

namespace ICSStudio.UIServicesPackage.DownloadCompareTool.ViewModel.Item
{
    internal class ControllerItem : CompareItem
    {
        private readonly ProjectDiffModel _diffModel;

        public ControllerItem(ProjectDiffModel diffModel)
        {
            _diffModel = diffModel;

            Title = "Controller";

            BuildChildren();
        }

        private void BuildChildren()
        {
            Children = new List<CompareItem>();

            if (_diffModel.ControllerProperties.ChangeType != ItemChangeType.Unchanged)
            {
                Children.Add(new CompareItem { Title = "Properties", DiffItem = _diffModel.ControllerProperties });
                ItemType = CompareItemType.Modified;
            }

            var tagsItem = new CompareItem { Title = "Tags", Children = new List<CompareItem>() };

            var normalTags = new List<CompareItem>();
            var gTags = new List<CompareItem>();
            var aTags = new List<CompareItem>();
            var vTags = new List<CompareItem>();
            foreach (var tag in _diffModel.ControllerTags.OrderBy(x => x.ChangeType))
            {
                if (tag.ChangeType == ItemChangeType.Unchanged)
                    continue;

                var tagName = tag.ChangeType == ItemChangeType.Added
                    ? tag.NewValue["Name"]?.ToString()
                    : tag.OldValue["Name"]?.ToString();

                var dataType = tag.ChangeType == ItemChangeType.Added
                    ? tag.NewValue["DataType"]?.ToString()
                    : tag.OldValue["DataType"]?.ToString();

                var title = tagName;

                if (string.Equals(dataType, "AXIS_CIP_DRIVE"))
                {
                    title = $"[A] {tagName}";
                    var tagItem = new CompareItem { Title = title, DiffItem = tag, OriginalType = CompareOriginalType.Tag };
                    aTags.Add(tagItem);
                }
                else if (string.Equals(dataType, "AXIS_VIRTUAL"))
                {
                    title = $"[V] {tagName}";
                    var tagItem = new CompareItem { Title = title, DiffItem = tag, OriginalType = CompareOriginalType.Tag };
                    vTags.Add(tagItem);
                }
                else if (string.Equals(dataType, "MOTION_GROUP"))
                {
                    title = $"[G] {tagName}";
                    var tagItem = new CompareItem { Title = title, DiffItem = tag, OriginalType = CompareOriginalType.Tag };
                    gTags.Add(tagItem);
                }
                else
                {
                    var tagItem = new CompareItem { Title = title, DiffItem = tag, OriginalType = CompareOriginalType.Tag };
                    normalTags.Add(tagItem);
                }

                tagsItem.ItemType = CompareItemType.Modified;
            }

            tagsItem.Children.AddRange(gTags);
            tagsItem.Children.AddRange(aTags);
            tagsItem.Children.AddRange(vTags);
            tagsItem.Children.AddRange(normalTags);

            if (tagsItem.Children.Count > 0)
                Children.Add(tagsItem);
        }
    }
}
