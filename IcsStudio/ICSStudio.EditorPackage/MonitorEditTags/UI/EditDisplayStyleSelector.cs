using System.Windows;
using System.Windows.Controls;
using ICSStudio.EditorPackage.MonitorEditTags.Models;
using ICSStudio.Interfaces.DataType;

namespace ICSStudio.EditorPackage.MonitorEditTags.UI
{
    public class EditDisplayStyleSelector : DataTemplateSelector
    {
        public DataTemplate NullStyleTemplate { get; set; }
        public DataTemplate ReadonlyTemplate { get; set; }
        public DataTemplate EditableTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var tagItem = item as EditTagItem;

            if (tagItem != null)
            {
                var displayStyle = tagItem.DisplayStyle;
                if (displayStyle == DisplayStyle.NullStyle)
                    return NullStyleTemplate;

                if (tagItem.IsDisplayStyleEnabled)
                {
                    return EditableTemplate;
                }

                return ReadonlyTemplate;
            }

            if (item != null) return NullStyleTemplate;

            return base.SelectTemplate(null, container);
        }
    }
}