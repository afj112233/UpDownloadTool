using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ICSStudio.Gui.PropertyAttribute
{
    /// <summary>
    /// https://www.codeproject.com/Articles/7852/PropertyGrid-utilities
    /// </summary>
    public class PropertiesDeluxeTypeConverter : ExpandableObjectConverter
    {
        public override PropertyDescriptorCollection GetProperties(
            ITypeDescriptorContext context,
            object value,
            Attribute[] attributes)
        {
            PropertyDescriptorCollection baseProps =
                TypeDescriptor.GetProperties(value, attributes);
            PropertyDescriptorCollection deluxeProps =
                new PropertyDescriptorCollection(null);

            List<PropertyAttributes> orderedPropertyAttributesList = new List<PropertyAttributes>();
            foreach (PropertyDescriptor oProp in baseProps)
            {
                PropertyAttributes propertyAttributes = GetPropertyAttributes(oProp, value);

                if (propertyAttributes.IsBrowsable)
                {
                    orderedPropertyAttributesList.Add(propertyAttributes);
                    deluxeProps.Add(
                        new PropertyDescriptorExtension(oProp, propertyAttributes));
                }
            }

            orderedPropertyAttributesList.Sort();

            var propertyNames
                = orderedPropertyAttributesList.Select(item => item.Name).ToArray();

            return deluxeProps.Sort(propertyNames);
        }

        private PropertyAttributes GetPropertyAttributes(
            PropertyDescriptor propertyDescriptor,
            object target)
        {
            PropertyAttributes propertyAttributes =
                new PropertyAttributes(propertyDescriptor.Name);

            string displayName = null;

            foreach (Attribute attribute in propertyDescriptor.Attributes)
            {
                Type type = attribute.GetType();
                // If there's a DisplayNameAttribute defined, use that DisplayName.
                if (type == typeof(DisplayNameAttribute))
                {
                    displayName = ((DisplayNameAttribute)attribute).DisplayName;
                }
                else if (type == typeof(PropertyOrderAttribute))
                {
                    propertyAttributes.Order = ((PropertyOrderAttribute)attribute).Order;
                }
            }

            // DisplayName
            if (propertyAttributes.DisplayName == null)
            {
                propertyAttributes.DisplayName = displayName;
            }
            if (propertyAttributes.DisplayName == null)
            {
                propertyAttributes.DisplayName = propertyDescriptor.DisplayName;
            }

            // Description
            if (propertyAttributes.Description == null)
            {
                propertyAttributes.Description = propertyDescriptor.Description;
            }

            // Category
            if (propertyAttributes.Category == null)
            {
                propertyAttributes.Category = propertyDescriptor.Category;
            }

            // IsReadonly.
            propertyAttributes.IsReadOnly = propertyDescriptor.IsReadOnly;

            // IsBrowsable.
            propertyAttributes.IsBrowsable = propertyDescriptor.IsBrowsable;

            //
            // Now let target be able to override each of these property attributes
            // dynamically.
            //
            PropertyAttributesProviderAttribute propertyAttributesProviderAttribute =
                (PropertyAttributesProviderAttribute)
                propertyDescriptor.Attributes[typeof(PropertyAttributesProviderAttribute)];
            if (propertyAttributesProviderAttribute != null)
            {
                MethodInfo propertyAttributesProvider =
                    propertyAttributesProviderAttribute.GetPropertyAttributesProvider(target);
                if (propertyAttributesProvider != null)
                {
                    propertyAttributesProvider.Invoke(target, new object[] { propertyAttributes });
                }
            }

            return propertyAttributes;
        }
    }
}
