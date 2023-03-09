using System.Windows;
using System.Windows.Controls;
using ICSStudio.QuickWatchPackage.View.Models;

namespace ICSStudio.QuickWatchPackage.View.UI
{
    public class NameSelector : DataTemplateSelector
    {
        public DataTemplate NewItemPlaceholderTemplate { get; set; }
        public DataTemplate ReadonlyTemplate { get; set; }
        public DataTemplate EditableTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var tagItem = item as MonitorTagItem;

            if (tagItem != null)
            {
                if (string.IsNullOrEmpty(tagItem.Name))
                {
                    return EditableTemplate;
                }

                return ReadonlyTemplate;
            }

            //if (item != null) return NewItemPlaceholderTemplate;
            
            return NewItemPlaceholderTemplate;
        }
    }
}