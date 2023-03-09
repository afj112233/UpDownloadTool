using System.ComponentModel;
using System.Reflection;
using ICSStudio.Gui.Controls.ExtendedPropertyGrid.Attributes;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Models
{
    public class PropertySetting
    {
        public static void SetPropertyVisibility(object obj, string propertyName, bool visible)
        {
            var type = typeof(BrowsableAttribute);
            var props = TypeDescriptor.GetProperties(obj);
            var attrs = props[propertyName].Attributes;
            var fld = type.GetField("browsable", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fld != null)
                fld.SetValue(attrs[type], visible);
        }

        public static bool GetPropertyVisibility(object obj, string propertyName)
        {
            var type = typeof(BrowsableAttribute);
            var props = TypeDescriptor.GetProperties(obj);
            var attrs = props[propertyName].Attributes;
            var fld = type.GetField("browsable", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fld != null)
                return (bool) fld.GetValue(attrs[type]);

            return true;
        }

        public static void SetPropertyReadOnly(object obj, string propertyName, bool readOnly)
        {
            var type = typeof(ReadOnlyAttribute);
            var props = TypeDescriptor.GetProperties(obj);

            if (props[propertyName] == null)
                return;

            var attrs = props[propertyName].Attributes;
            var fld = type.GetField("isReadOnly",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance);
            if (fld != null)
            {
                fld.SetValue(attrs[type], readOnly);
            }
        }

        public static void SetPropertyUnit(object obj, string propertyName, string newUnit)
        {
            var type = typeof(UnitAttribute);
            var props = TypeDescriptor.GetProperties(obj);

            if (props[propertyName] == null)
                return;

            var attrs = props[propertyName].Attributes;
            var fld = type.GetField("unit",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance);
            if (fld != null)
            {
                fld.SetValue(attrs[type], newUnit);
            }
        }

        public static void SetPropertyIsChanged(object obj, string propertyName, bool isChanged)
        {
            var type = typeof(IsChangedAttribute);
            var props = TypeDescriptor.GetProperties(obj);

            if (props[propertyName] == null)
                return;

            var attrs = props[propertyName].Attributes;
            var fld = type.GetField("isChanged",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance);
            if (fld != null)
            {
                fld.SetValue(attrs[type], isChanged);
            }
        }
    }
}
