using System.ComponentModel;
using ICSStudio.Gui.PropertyAttribute;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.DataTypes
{
    [DisplayName("MDO Parameters")]
    [TypeConverter(typeof(PropertiesDeluxeTypeConverter))]
    public class MDOParam : ParamBase
    {
        #region enum types

        public enum DriveUnitsTypes : uint
        {
            Volts = 0,
            Percent = 1
        }

        #endregion

        public MDOParam()
        {
            DriveOutput = 0;
            DriveUnits = DriveUnitsTypes.Volts;
        }

        [DisplayName("Drive Output")] public float DriveOutput { get; set; }

        [DisplayName("Drive Units")] public DriveUnitsTypes DriveUnits { get; set; }
    }
}
