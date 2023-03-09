using System;
using System.Runtime.Serialization;

namespace ICSStudio.SimpleServices.DataWrapper
{
    public class AxisVirtualParameters
    {

        public enum PositioningMode : byte
        {
            [EnumMember(Value = "Linear")] Linear,
            [EnumMember(Value = "Rotary")] Rotary,
        }

        [Flags]
        public enum MasterInputConfigurationTypes
        {
            MasterDelayCompensation = 1 << 0,
            MasterPositionFilter = 1 << 1
        }

        #region L5X

        public string MotionGroup { get; set; }
        public float ConversionConstant { set; get; }
        public int OutputCamExecutionTargets { set; get; }
        public string PositionUnits { set; get; }
        public float AverageVelocityTimebase { set; get; }
        public byte RotaryAxis { set; get; }
        public int PositionUnwind { set; get; }
        public byte HomeMode { set; get; }
        public byte HomeDirection { set; get; }
        public byte HomeSequence { set; get; }
        public uint HomeConfigurationBits { set; get; }
        public float HomePosition { set; get; }
        public float HomeOffset { set; get; }
        public float MaximumSpeed { set; get; }
        public float MaximumAcceleration { set; get; }
        public float MaximumDeceleration { set; get; }
        public byte ProgrammedStopMode { set; get; }
        public uint MasterInputConfigurationBits { set; get; }
        public float MasterPositionFilterBandwidth { set; get; }
        public float MaximumAccelerationJerk { set; get; }
        public float MaximumDecelerationJerk { set; get; }
        public uint DynamicsConfigurationBits { set; get; }
        public byte InterpolatedPositionConfiguration { set; get; }
        public byte AxisUpdateSchedule { set; get; }

        #endregion
    }

}
