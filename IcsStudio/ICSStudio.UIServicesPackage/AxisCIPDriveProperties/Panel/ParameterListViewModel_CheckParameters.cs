using System.Collections.Generic;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    internal partial class ParameterListViewModel
    {
        private static readonly List<CheckParameterInfo> CheckParameters = new List<CheckParameterInfo>()
        {
            //TODO(gjc): need check for
            // HomeOffset,HomePosition
            // MotionResolution

            new CheckParameterInfo()
            {
                Name = "AverageVelocityTimebase", Group = "Planner", MinValue = 0.001f, MaxValue = 32f
            },

            new CheckParameterInfo()
            {
                Name = "AccelerationLimit", Group = "Acceleration Loop", MinValue = 0f, MaxValue = float.MaxValue
            },
            new CheckParameterInfo()
            {
                Name = "DecelerationLimit", Group = "Acceleration Loop", MinValue = 0f, MaxValue = float.MaxValue
            },

            new CheckParameterInfo()
            {
                Name = "StoppingTorque", Group = "Actions", MinValue = 0f, MaxValue = 1000f
            },
            new CheckParameterInfo()
            {
                Name = "VelocityStandstillWindow", Group = "Actions", MinValue = 0f, MaxValue = 2147483.647f
            },
            new CheckParameterInfo()
            {
                Name = "VelocityThreshold", Group = "Actions", MinValue = 0f, MaxValue = 2147483.647f
            },
            new CheckParameterInfo()
            {
                Name = "CoastingTimeLimit", Group = "Actions", MinValue = 0f, MaxValue = 1000f
            },
            new CheckParameterInfo()
            {
                Name = "MechanicalBrakeEngageDelay", Group = "Actions", MinValue = 0f, MaxValue = 1000f
            },
            new CheckParameterInfo()
            {
                Name = "MechanicalBrakeReleaseDelay", Group = "Actions", MinValue = 0f, MaxValue = 1000f
            },
            new CheckParameterInfo()
            {
                Name = "StoppingTimeLimit", Group = "Actions", MinValue = 0f, MaxValue = 6553.5f
            },
            new CheckParameterInfo()
            {
                Name = "ZeroSpeedTime", Group = "Actions", MinValue = 0f, MaxValue = 1000f
            },
            new CheckParameterInfo()
            {
                Name = "ZeroSpeed", Group = "Actions", MinValue = 0f, MaxValue = float.MaxValue
            },

            new CheckParameterInfo()
            {
                Name = "BacklashCompensationWindow", Group = "Backlash", MinValue = 0f, MaxValue = float.MaxValue
            },
            new CheckParameterInfo()
            {
                Name = "BacklashReversalOffset", Group = "Backlash", MinValue = 0f, MaxValue = float.MaxValue
            },
            new CheckParameterInfo()
            {
                Name = "TorqueLeadLagFilterBandwidth", Group = "Compliance", MinValue = 0f, MaxValue = 10000f
            },
            new CheckParameterInfo()
            {
                Name = "TorqueLeadLagFilterGain", Group = "Compliance", MinValue = 0f, MaxValue = float.MaxValue
            },
            new CheckParameterInfo()
            {
                Name = "TorqueLowPassFilterBandwidth", Group = "Compliance", MinValue = 0f, MaxValue = 10000f
            },
            new CheckParameterInfo()
            {
                Name = "TorqueNotchFilterFrequency", Group = "Compliance", MinValue = 0f, MaxValue = 10000f
            },

            new CheckParameterInfo()
            {
                Name = "FrictionCompensationSliding", Group = "Friction", MinValue = 0f, MaxValue = 100f
            },
            new CheckParameterInfo()
            {
                Name = "FrictionCompensationStatic", Group = "Friction", MinValue = 0f, MaxValue = 100f
            },
            new CheckParameterInfo()
            {
                Name = "FrictionCompensationViscous", Group = "Friction", MinValue = 0f, MaxValue = 100f
            },
            new CheckParameterInfo()
            {
                Name = "FrictionCompensationWindow", Group = "Friction", MinValue = 0f, MaxValue = 2147.48f
            },

            new CheckParameterInfo()
            {
                Name = "LoadRatio", Group = "Load", MinValue = 0f, MaxValue = float.MaxValue
            },
            new CheckParameterInfo()
            {
                Name = "RotaryMotorInertia", Group = "Load", MinValue = 0f, MaxValue = float.MaxValue
            },
            new CheckParameterInfo()
            {
                Name = "TotalInertia", Group = "Load", MinValue = 0f, MaxValue = float.MaxValue
            },
            new CheckParameterInfo()
            {
                Name = "MotorOverloadLimit", Group = "Motor", MinValue = 0f, MaxValue = 100f
            },

            new CheckParameterInfo()
            {
                Name = "Feedback1AccelFilterBandwidth", Group = "Motor Feedback", MinValue = 0f,
                MaxValue = float.MaxValue
            },
            new CheckParameterInfo()
            {
                Name = "Feedback1AccelFilterTaps", Group = "Motor Feedback", MinValue = 1f, MaxValue = 32767f
            },
            new CheckParameterInfo()
            {
                Name = "Feedback1VelocityFilterBandwidth", Group = "Motor Feedback", MinValue = 0f,
                MaxValue = float.MaxValue
            },
            new CheckParameterInfo()
            {
                Name = "Feedback1VelocityFilterTaps", Group = "Motor Feedback", MinValue = 1f, MaxValue = 32767f
            },

            new CheckParameterInfo()
            {
                Name = "LoadObserverFeedbackGain", Group = "Observer", MinValue = 0f, MaxValue = float.MaxValue
            },
            new CheckParameterInfo()
            {
                Name = "LoadObserverBandwidth", Group = "Observer", MinValue = 0f, MaxValue = float.MaxValue
            },
            new CheckParameterInfo()
            {
                Name = "LoadObserverIntegratorBandwidth", Group = "Observer", MinValue = 0f, MaxValue = float.MaxValue
            },

            new CheckParameterInfo()
            {
                Name = "MaximumAcceleration", Group = "Planner", MinValue = 0f, MaxValue = float.MaxValue
            },
            new CheckParameterInfo()
            {
                Name = "MaximumAccelerationJerk", Group = "Planner", MinValue = 0f, MaxValue = float.MaxValue
            },
            new CheckParameterInfo()
            {
                Name = "MaximumSpeed", Group = "Planner", MinValue = 0f, MaxValue = float.MaxValue
            },
            new CheckParameterInfo()
            {
                Name = "OutputCamExecutionTargets", Group = "Planner", MinValue = 0, MaxValue = 8
            },

            new CheckParameterInfo()
            {
                Name = "VelocityFeedforwardGain", Group = "Position Loop", MinValue = 0, MaxValue = float.MaxValue
            },
            new CheckParameterInfo()
            {
                Name = "PositionLeadLagFilterGain", Group = "Position Loop", MinValue = 0, MaxValue = 1000f
            },
            new CheckParameterInfo()
            {
                Name = "PositionIntegratorBandwidth", Group = "Position Loop", MinValue = 0, MaxValue = float.MaxValue
            },
            new CheckParameterInfo()
            {
                Name = "PositionLeadLagFilterBandwidth", Group = "Position Loop", MinValue = 0, MaxValue = 10000f
            },
            new CheckParameterInfo()
            {
                Name = "PositionLoopBandwidth", Group = "Position Loop", MinValue = 0, MaxValue = float.MaxValue
            },
            new CheckParameterInfo()
            {
                Name = "PositionErrorTolerance", Group = "Position Loop", MinValue = 0, MaxValue = 2147.48f
            },
            new CheckParameterInfo()
            {
                Name = "PositionLockTolerance", Group = "Position Loop", MinValue = 0, MaxValue = 2147.48f
            },
            new CheckParameterInfo()
            {
                Name = "PositionErrorToleranceTime", Group = "Position Loop", MinValue = 0, MaxValue = float.MaxValue
            },

            //
            new CheckParameterInfo()
            {
                Name = "OvertorqueLimit", Group = "Torque/Current Loop", MinValue = 0, MaxValue = 1000
            },
            new CheckParameterInfo()
            {
                Name = "OvertorqueLimitTime", Group = "Torque/Current Loop", MinValue = 0, MaxValue = 1000
            },
            new CheckParameterInfo()
            {
                Name = "TorqueLimitNegative", Group = "Torque/Current Loop", MinValue = -1000f, MaxValue = 0
            },
            new CheckParameterInfo()
            {
                Name = "TorqueLimitPositive", Group = "Torque/Current Loop", MinValue = 0, MaxValue = 1000f
            },
            new CheckParameterInfo()
            {
                Name = "TorqueRateLimit", Group = "Torque/Current Loop", MinValue = 0, MaxValue = float.MaxValue
            },
            new CheckParameterInfo()
            {
                Name = "TorqueThreshold", Group = "Torque/Current Loop", MinValue = 0, MaxValue = 1000f
            },
            new CheckParameterInfo()
            {
                Name = "UndertorqueLimit", Group = "Torque/Current Loop", MinValue = 0, MaxValue = 1000f
            },
            new CheckParameterInfo()
            {
                Name = "UndertorqueLimitTime", Group = "Torque/Current Loop", MinValue = 0, MaxValue = 1000f
            },
            new CheckParameterInfo()
            {
                Name = "TorqueLoopBandwidth", Group = "Torque/Current Loop", MinValue = 0, MaxValue = float.MaxValue
            },

            //
            new CheckParameterInfo()
            {
                Name = "AccelerationFeedforwardGain", Group = "Velocity Loop", MinValue = 0, MaxValue = float.MaxValue
            },
            new CheckParameterInfo()
            {
                Name = "VelocityDroop", Group = "Velocity Loop", MinValue = 0, MaxValue = 2.14748e6f
            },
            new CheckParameterInfo()
            {
                Name = "VelocityNegativeFeedforwardGain", Group = "Velocity Loop", MinValue = 0, MaxValue = 1000
            },
            new CheckParameterInfo()
            {
                Name = "VelocityIntegratorBandwidth", Group = "Velocity Loop", MinValue = 0, MaxValue = float.MaxValue
            },
            new CheckParameterInfo()
            {
                Name = "VelocityLoopBandwidth", Group = "Velocity Loop", MinValue = 0, MaxValue = float.MaxValue
            },
            new CheckParameterInfo()
            {
                Name = "VelocityErrorTolerance", Group = "Velocity Loop", MinValue = 0, MaxValue = float.MaxValue
            },
            new CheckParameterInfo()
            {
                Name = "VelocityLimitNegative", Group = "Velocity Loop", MinValue = -2.14748e6f, MaxValue = 0
            },
            new CheckParameterInfo()
            {
                Name = "VelocityLimitPositive", Group = "Velocity Loop", MinValue = 0, MaxValue = 2.14748e6f
            },
            new CheckParameterInfo()
            {
                Name = "VelocityLockTolerance", Group = "Velocity Loop", MinValue = 0, MaxValue = float.MaxValue
            },
            new CheckParameterInfo()
            {
                Name = "VelocityOffset", Group = "Velocity Loop", MinValue = -2.14748e6f, MaxValue = 2.14748e6f
            },
            new CheckParameterInfo()
            {
                Name = "VelocityErrorToleranceTime", Group = "Velocity Loop", MinValue = 0, MaxValue = float.MaxValue
            },
            // homing
            new CheckParameterInfo()
            {
                Name = "HomeReturnSpeed", Group = "Homing", MinValue = 0, MaxValue = float.MaxValue
            },
            new CheckParameterInfo()
            {
                Name = "HomeSpeed", Group = "Homing", MinValue = 0, MaxValue = float.MaxValue
            },
            new CheckParameterInfo()
            {
                Name = "HomeOffset", Group = "Homing", MinValue = -2147.48f, MaxValue = 2147.48f
            },
            new CheckParameterInfo()
            {
                Name = "HomePosition", Group = "Homing", MinValue = -2147.48f, MaxValue = 2147.48f
            },
            // scaling
            new CheckParameterInfo()
            {
                Name = "ConversionConstant", Group = "Scaling", MinValue = 1e-12f, MaxValue = 1e12f
            },
            new CheckParameterInfo()
            {
                Name = "MotionResolution", Group = "Scaling", MinValue = 1, MaxValue = int.MaxValue
            },
        };
    }
}
