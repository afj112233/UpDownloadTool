using System.ComponentModel;
using ICSStudio.Gui.Converters;
using ICSStudio.Gui.PropertyAttribute;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.DataTypes
{
    [DisplayName("MAW Parameters")]
    [TypeConverter(typeof(PropertiesDeluxeTypeConverter))]
    public class MAWParam : ParamBase
    {
        #region enum types

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum TriggerConditionTypes : uint
        {
            Forward = 0,
            Reverse = 1
        }

        #endregion

        public MAWParam()
        {
            TriggerCondition = TriggerConditionTypes.Forward;
            Position = 0;
        }

        [PropertyOrder(0)]
        [DisplayName("Trigger Condition")]
        [ReadOnly(false)]
        public TriggerConditionTypes TriggerCondition { get; set; }


        [PropertyOrder(1)]
        [DisplayName("Position")]
        [ReadOnly(false)]
        public float Position { get; set; }
    }
}
