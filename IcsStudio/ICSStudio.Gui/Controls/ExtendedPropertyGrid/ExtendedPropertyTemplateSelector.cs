using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.Gui.Controls.ExtendedPropertyGrid.Models;
using Imagin.Common.Extensions;

namespace ICSStudio.Gui.Controls.ExtendedPropertyGrid
{
    public class ExtendedPropertyTemplateSelector : DataTemplateSelector
    {

        /// <summary>
        /// 
        /// </summary>
        public ResourceDictionary Resources
        {
            get; set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null)
            {
                foreach (DictionaryEntry i in Resources)
                {
                    if (i.Key.As<Type>() == item.As<ExtendedPropertyModel>().Primitive)
                        return i.Value as DataTemplate;
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}
