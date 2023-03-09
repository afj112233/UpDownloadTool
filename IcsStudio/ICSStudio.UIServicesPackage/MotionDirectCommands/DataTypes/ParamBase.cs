using System.Collections.Generic;
using ICSStudio.Gui.PropertyAttribute;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.DataTypes
{
    public delegate void DynamicPropertyChangedHandler(object obj);

    public abstract class ParamBase
    {
        private readonly Dictionary<string, bool> _readOnlyDictionary;

        protected ParamBase()
        {
            _readOnlyDictionary = new Dictionary<string, bool>();
        }

        public DynamicPropertyChangedHandler OnDynamicPropertyChanged;

        protected void SetPropertyReadOnly(string propertyName, bool readOnly)
        {
            if (_readOnlyDictionary.ContainsKey(propertyName))
            {
                _readOnlyDictionary[propertyName] = readOnly;
            }
            else
            {
                _readOnlyDictionary.Add(propertyName, readOnly);
            }
        }

        public void DynamicPropertyAttributesProvider(PropertyAttributes attributes)
        {
            if (_readOnlyDictionary.ContainsKey(attributes.Name))
            {
                attributes.IsReadOnly = _readOnlyDictionary[attributes.Name];
            }
        }
    }
}
