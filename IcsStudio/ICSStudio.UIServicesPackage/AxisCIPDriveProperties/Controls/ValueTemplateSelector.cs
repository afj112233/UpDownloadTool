using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Models;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Controls
{
    internal class ValueTemplateSelector : DataTemplateSelector
    {
        public ResourceDictionary Resources { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var parameterItem = item as ParameterItem;

            if (parameterItem != null)
            {
                //if(parameterItem.IsReadOnly)
                //    return (DataTemplate)Resources["ReadOnly"];
                
                if(parameterItem.ParameterType.IsEnum)
                    return (DataTemplate)Resources[typeof(Enum)];

                foreach (DictionaryEntry i in Resources)
                {
                    var type = i.Key as Type;
                    
                    if (type == parameterItem.Value?.GetType())
                        return i.Value as DataTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
