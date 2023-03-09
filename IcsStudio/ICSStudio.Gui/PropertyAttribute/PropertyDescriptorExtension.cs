using System;
using System.ComponentModel;

namespace ICSStudio.Gui.PropertyAttribute
{
    public class PropertyDescriptorExtension : PropertyDescriptor
    {
        private readonly PropertyDescriptor _baseDescriptor;
        private readonly PropertyAttributes _propertyAttributes;

        public PropertyDescriptorExtension(
            PropertyDescriptor baseDescriptor,
            PropertyAttributes propertyAttributes)
            : base(baseDescriptor)
        {
            _baseDescriptor = baseDescriptor;
            _propertyAttributes = propertyAttributes;
        }
        
        public override bool CanResetValue(object component)
        {
            return _baseDescriptor.CanResetValue(component);
        }

        public override object GetValue(object component)
        {
            return _baseDescriptor.GetValue(component);
        }

        public override void ResetValue(object component)
        {
            _baseDescriptor.ResetValue(component);
        }

        public override void SetValue(object component, object value)
        {
            _baseDescriptor.SetValue(component, value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return _baseDescriptor.ShouldSerializeValue(component);
        }

        public override string Name => _baseDescriptor.Name;
        public override Type ComponentType => _baseDescriptor.ComponentType;
        public override string DisplayName => _propertyAttributes.DisplayName;
        public override string Description => _propertyAttributes.Description;
        public override string Category => _propertyAttributes.Category;
        public override bool IsReadOnly => _propertyAttributes.IsReadOnly;
        public override bool IsBrowsable => _propertyAttributes.IsBrowsable;
        public override Type PropertyType => _baseDescriptor.PropertyType;
    }
}
