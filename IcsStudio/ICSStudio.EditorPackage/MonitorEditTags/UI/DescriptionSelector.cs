using System.Windows;
using System.Windows.Controls;
using ICSStudio.EditorPackage.MonitorEditTags.Models;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.EditorPackage.MonitorEditTags.UI
{
    public class DescriptionSelector : DataTemplateSelector
    {
        public DataTemplate ReadonlyTemplate { get; set; }
        public DataTemplate EditableTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var tagItem = item as TagItem;

            if (tagItem != null)
            {
                var controller = tagItem.Tag?.ParentController;
                if (controller != null && controller.IsOnline)
                    return ReadonlyTemplate;

                return EditableTemplate;
            }

            if (item != null) return EditableTemplate;

            return base.SelectTemplate(null, container);
        }
    }
}
