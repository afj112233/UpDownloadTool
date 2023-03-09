using System.ComponentModel;
using System.Runtime.Serialization;
using ICSStudio.Gui.Converters;
using ICSStudio.Gui.PropertyAttribute;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.DataTypes
{
    [DisplayName("MDR Parameters")]
    [TypeConverter(typeof(PropertiesDeluxeTypeConverter))]
    public class MDRParam : ParamBase
    {
        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum InputNumberTypes : uint
        {
            [EnumMember(Value = "1")] Registration1Position = 1,
            [EnumMember(Value = "2")] Registration2Position = 2
        }

        public MDRParam()
        {
            InputNumber = InputNumberTypes.Registration1Position;
        }

        [DisplayName("Input Number")]
        [PropertyOrder(0)]
        [ReadOnly(false)]
        public InputNumberTypes InputNumber { get; set; }
    }
}
