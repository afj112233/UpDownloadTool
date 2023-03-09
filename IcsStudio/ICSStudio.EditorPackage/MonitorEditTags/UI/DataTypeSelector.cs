using System.Windows;
using System.Windows.Controls;
using ICSStudio.EditorPackage.MonitorEditTags.Models;

namespace ICSStudio.EditorPackage.MonitorEditTags.UI
{
    public class DataTypeSelector : DataTemplateSelector
    {
        public DataTemplate ReadonlyTemplate { get; set; }
        public DataTemplate EditableTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var tagItem = item as EditTagItem;

            if (tagItem != null)
            {
                var controller = tagItem.Tag?.ParentController;
                if (controller != null && controller.IsOnline)
                    return ReadonlyTemplate;

                return tagItem.IsDataTypeEnabled ? EditableTemplate : ReadonlyTemplate;
            }

            if (item != null)
                return EditableTemplate;


            return base.SelectTemplate(null, container);
        }
    }
}