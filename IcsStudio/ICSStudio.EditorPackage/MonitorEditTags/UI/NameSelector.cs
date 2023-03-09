using System.Windows;
using System.Windows.Controls;
using ICSStudio.EditorPackage.MonitorEditTags.Models;

namespace ICSStudio.EditorPackage.MonitorEditTags.UI
{
    public class NameSelector : DataTemplateSelector
    {
        public DataTemplate NewItemPlaceholderTemplate { get; set; }
        public DataTemplate ReadonlyTemplate { get; set; }
        public DataTemplate EditableTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var tagItem = item as EditTagItem;

            if (tagItem != null)
            {
                if (tagItem.IsNameEnabled)
                {
                    return EditableTemplate;
                }

                return ReadonlyTemplate;
            }

            if (item != null) return NewItemPlaceholderTemplate;

            return base.SelectTemplate(null, container);
        }
    }
}