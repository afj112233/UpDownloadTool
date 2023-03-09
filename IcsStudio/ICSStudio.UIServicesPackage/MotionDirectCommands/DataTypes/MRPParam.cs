using System.ComponentModel;
using System.Runtime.Serialization;
using ICSStudio.Gui.Converters;
using ICSStudio.Gui.PropertyAttribute;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.DataTypes
{
    [DisplayName("MRP Parameters")]
    [TypeConverter(typeof(PropertiesDeluxeTypeConverter))]
    public class MRPParam : ParamBase
    {
        public MRPParam()
        {
            Type = TypeTypes.Absolute;
            PositionSelect = PositionSelectTypes.Actual;
            Position = 0;
        }

        [PropertyOrder(0)]
        [DisplayName("Type")]
        [ReadOnly(false)]
        public TypeTypes Type { get; set; }

        [PropertyOrder(1)]
        [DisplayName("Position Select")]
        [ReadOnly(false)]
        public PositionSelectTypes PositionSelect { get; set; }

        [PropertyOrder(2)]
        [DisplayName("Position")]
        [ReadOnly(false)]
        public float Position { get; set; }

        #region enum types

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum TypeTypes : uint
        {
            [EnumMember(Value = "Absolute")] Absolute = 0,
            [EnumMember(Value = "Relative")] Relative = 1
        }

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum PositionSelectTypes : uint
        {
            [EnumMember(Value = "Actual")] Actual = 0,
            [EnumMember(Value = "Command")] Command = 1
        }

        #endregion
    }
}
