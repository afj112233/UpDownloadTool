using System;
using ICSStudio.Cip.Objects;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;

namespace ICSStudio.SimpleServices.DataWrapper
{
    public class AxisVirtual : DataWrapper, ICloneable
    {
        private AxisVirtualParameters _parameters;

        private CIPAxis _cipAxis;

        private ITag _assignedGroup;

        private byte _rotaryAxis;

        public static AxisVirtual Create(IDataType dataType, IController controller)
        {
            if (dataType == null
                || !dataType.Name.Equals("AXIS_VIRTUAL", StringComparison.OrdinalIgnoreCase))
                return null;

            AxisVirtual axisVirtual = new AxisVirtual(dataType, controller);

            return axisVirtual;
        }

        private AxisVirtual(IDataType dataType, IController controller) 
            : base(dataType, 0, 0, 0, null)
        {
            Controller = controller;

            _cipAxis = new CIPAxis(0, null);

            _parameters = new AxisVirtualParameters();
            //TODO(gjc): load default virtual axis value
        }

        public IController Controller { get; }

        public CIPAxis CIPAxis => _cipAxis;

        public ITag AssignedGroup
        {
            get { return _assignedGroup; }
            set
            {
                if (_assignedGroup != value)
                {
                    _assignedGroup = value;
                    NotifyParentPropertyChanged();
                }
            }
        }

        public AxisVirtualParameters Parameters
        {
            set { _parameters = value; }
            get
            {
                if (_cipAxis != null)
                    CIPAxisToParameters();

                _parameters.MotionGroup = AssignedGroup?.Name;
                return _parameters;
            }
        }

        public byte RotaryAxis
        {
            get { return _rotaryAxis; }
            set { _rotaryAxis = value; }
        }

        public void PostLoadJson()
        {
            if (_parameters == null) return;

            ParametersToCIPAxis();

            AssignedGroup = Controller.Tags[_parameters.MotionGroup];

        }

        private void ParametersToCIPAxis()
        {
            _rotaryAxis = _parameters.RotaryAxis;

            _cipAxis.ConversionConstant = _parameters.ConversionConstant;
            _cipAxis.OutputCamExecutionTargets = (uint) _parameters.OutputCamExecutionTargets;
            _cipAxis.PositionUnits = _parameters.PositionUnits;
            _cipAxis.AverageVelocityTimebase = _parameters.AverageVelocityTimebase;
            _cipAxis.PositionUnwind = (uint) _parameters.PositionUnwind;
            _cipAxis.HomeMode = _parameters.HomeMode;
            _cipAxis.HomeDirection = _parameters.HomeDirection;
            _cipAxis.HomeSequence = _parameters.HomeSequence;
            _cipAxis.HomeConfigurationBits = _parameters.HomeConfigurationBits;
            _cipAxis.HomePosition = _parameters.HomePosition;
            _cipAxis.HomeOffset = _parameters.HomeOffset;
            _cipAxis.MaximumSpeed = _parameters.MaximumSpeed;
            _cipAxis.MaximumAcceleration = _parameters.MaximumAcceleration;
            _cipAxis.MaximumDeceleration = _parameters.MaximumDeceleration;
            _cipAxis.ProgrammedStopMode = _parameters.ProgrammedStopMode;
            _cipAxis.MasterInputConfigurationBits = _parameters.MasterInputConfigurationBits;
            _cipAxis.MasterPositionFilterBandwidth = _parameters.MasterPositionFilterBandwidth;
            _cipAxis.MaximumAccelerationJerk = _parameters.MaximumAccelerationJerk;
            _cipAxis.MaximumDecelerationJerk = _parameters.MaximumDecelerationJerk;
            _cipAxis.DynamicsConfigurationBits = _parameters.DynamicsConfigurationBits;
            _cipAxis.InterpolatedPositionConfiguration = _parameters.InterpolatedPositionConfiguration;
            _cipAxis.AxisUpdateSchedule = _parameters.AxisUpdateSchedule;

        }

        private void CIPAxisToParameters()
        {
            _parameters.RotaryAxis = _rotaryAxis;

            _parameters.ConversionConstant = Convert.ToSingle(_cipAxis.ConversionConstant);
            _parameters.OutputCamExecutionTargets = Convert.ToInt32(_cipAxis.OutputCamExecutionTargets);
            _parameters.PositionUnits = _cipAxis.PositionUnits.GetString();
            _parameters.AverageVelocityTimebase = Convert.ToSingle(_cipAxis.AverageVelocityTimebase);
            _parameters.PositionUnwind = Convert.ToInt32(_cipAxis.PositionUnwind);
            _parameters.HomeMode = Convert.ToByte(_cipAxis.HomeMode);
            _parameters.HomeDirection = Convert.ToByte(_cipAxis.HomeDirection);
            _parameters.HomeSequence = Convert.ToByte(_cipAxis.HomeSequence);
            _parameters.HomeConfigurationBits = Convert.ToUInt32(_cipAxis.HomeConfigurationBits);
            _parameters.HomePosition = Convert.ToSingle(_cipAxis.HomePosition);
            _parameters.HomeOffset = Convert.ToSingle(_cipAxis.HomeOffset);
            _parameters.MaximumSpeed = Convert.ToSingle(_cipAxis.MaximumSpeed);
            _parameters.MaximumAcceleration = Convert.ToSingle(_cipAxis.MaximumAcceleration);
            _parameters.MaximumDeceleration = Convert.ToSingle(_cipAxis.MaximumDeceleration);
            _parameters.ProgrammedStopMode = Convert.ToByte(_cipAxis.ProgrammedStopMode);
            _parameters.MasterInputConfigurationBits = Convert.ToUInt32(_cipAxis.MasterInputConfigurationBits);
            _parameters.MasterPositionFilterBandwidth = Convert.ToSingle(_cipAxis.MasterPositionFilterBandwidth);
            _parameters.MaximumAccelerationJerk = Convert.ToSingle(_cipAxis.MaximumAccelerationJerk);
            _parameters.MaximumDecelerationJerk = Convert.ToSingle(_cipAxis.MaximumDecelerationJerk);
            _parameters.DynamicsConfigurationBits = Convert.ToUInt32(_cipAxis.DynamicsConfigurationBits);
            _parameters.InterpolatedPositionConfiguration = Convert.ToByte(_cipAxis.InterpolatedPositionConfiguration);
            _parameters.AxisUpdateSchedule = Convert.ToByte(_cipAxis.AxisUpdateSchedule);
        }
        
        public object Clone()
        {
            var axisVirtual = (AxisVirtual) MemberwiseClone();
            axisVirtual.AssignedGroup = null;
            axisVirtual._cipAxis = (CIPAxis)CIPAxis.Clone();
            return axisVirtual;
        }

    }
}
