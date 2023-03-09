using System.Windows;
using System.Windows.Controls;
using ICSStudio.EditorPackage.MonitorEditTags.Models;
using ICSStudio.Interfaces.DataType;

namespace ICSStudio.EditorPackage.MonitorEditTags.UI
{
    public class DisplayStyleSelector : DataTemplateSelector
    {
        public DataTemplate NullStyleTemplate { get; set; }
        public DataTemplate NormalStyleTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var tagItem = item as TagItem;

            if (tagItem != null)
            {
                var displayStyle = tagItem.DisplayStyle;
                if (displayStyle == DisplayStyle.NullStyle)
                    return NullStyleTemplate;

                return NormalStyleTemplate;
            }

            if (item != null) return NullStyleTemplate;

            return base.SelectTemplate(null, container);
        }
    }
}