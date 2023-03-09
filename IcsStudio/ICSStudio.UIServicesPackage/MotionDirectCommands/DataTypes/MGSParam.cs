using System.ComponentModel;
using System.Runtime.Serialization;
using ICSStudio.Gui.Converters;
using ICSStudio.Gui.PropertyAttribute;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.DataTypes
{
    [DisplayName("MGS Parameters")]
    [TypeConverter(typeof(PropertiesDeluxeTypeConverter))]
    public class MGSParam : ParamBase
    {
        #region enum types

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum StopModeTypes : uint
        {
            [EnumMember(Value = "Programmed")] Programmed = 0x00,
            [EnumMember(Value = "Fast Stop")] FastStop = 0x01,
            [EnumMember(Value = "Fast Disable")] FastDisable = 0x02
        }

        #endregion

        public MGSParam()
        {
            StopMode = StopModeTypes.FastStop;
        }

        [PropertyOrder(0)]
        [DisplayName("Stop Mode")]
        [ReadOnly(false)]
        public StopModeTypes StopMode { get; set; }
    }
}
