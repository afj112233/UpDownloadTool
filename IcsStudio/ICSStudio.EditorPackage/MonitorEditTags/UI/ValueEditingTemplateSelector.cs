﻿using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.EditorPackage.MonitorEditTags.Models;

namespace ICSStudio.EditorPackage.MonitorEditTags.UI
{
    public class ValueEditingTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// </summary>
        public ResourceDictionary Resources { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var monitorTagItem = item as MonitorTagItem;

            if (monitorTagItem != null)
            {
                if (monitorTagItem.IsValueEnabled)
                {
                    foreach (DictionaryEntry i in Resources)
                    {
                        var type = i.Key as Type;
                        if (type != null && type == monitorTagItem.Value.GetType())
                            return i.Value as DataTemplate;
                    }
                }
                else
                {
                    return (DataTemplate) Resources["ReadOnly"];
                }

            }

            return base.SelectTemplate(item, container);
        }
    }
}