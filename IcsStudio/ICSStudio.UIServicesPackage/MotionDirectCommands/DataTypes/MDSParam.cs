using System.ComponentModel;
using System.Runtime.Serialization;
using ICSStudio.Gui.Converters;
using ICSStudio.Gui.PropertyAttribute;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.DataTypes
{
    [DisplayName("MDS Parameters")]
    [TypeConverter(typeof(PropertiesDeluxeTypeConverter))]
    public class MDSParam : ParamBase
    {
        #region enum types

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum SpeedUnitsTypes //: uint
        {
            [EnumMember(Value = "Units per sec")] UnitsPerSec = 0,
            [EnumMember(Value = "% of Maximum")] PercentOfMaximum = 1
        }

        #endregion

        public MDSParam()
        {
            Speed = 0;
            SpeedUnits = SpeedUnitsTypes.UnitsPerSec;
        }

        [DisplayName("Speed")] public float Speed { get; set; }

        [DisplayName("Speed Units")] public SpeedUnitsTypes SpeedUnits { get; set; }
    }
}
