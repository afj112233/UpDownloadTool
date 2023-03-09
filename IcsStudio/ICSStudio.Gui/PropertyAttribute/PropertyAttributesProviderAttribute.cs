using System;
using System.Reflection;

namespace ICSStudio.Gui.PropertyAttribute
{
    public delegate void PropertyAttributesProvider(PropertyAttributes propertyAttributes);

    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyAttributesProviderAttribute : Attribute
    {
        private readonly string _propertyAttributesProviderName;

        public PropertyAttributesProviderAttribute(string propertyAttributesProviderName)
        {
            this._propertyAttributesProviderName = propertyAttributesProviderName;
        }

        public MethodInfo GetPropertyAttributesProvider(object target)
        {
            return target.GetType().GetMethod(_propertyAttributesProviderName);
        }

        public string Name => _propertyAttributesProviderName;
    }
}
