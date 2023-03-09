using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Annotations;
using ICSStudio.SimpleServices.DataWrapper;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Models
{
    internal partial class AxisCIPParameters : INotifyPropertyChanged
    {
        private readonly AxisCIPDrive _currentAxis;
        private readonly AxisCIPDrive _originalAxis;
        private readonly AxisCIPDrivePropertiesViewModel _parentViewModel;

        public AxisCIPParameters(
            AxisCIPDrive currentAxis,
            AxisCIPDrive originalAxis,
            AxisCIPDrivePropertiesViewModel parentViewModel)
        {
            _currentAxis = currentAxis;
            _originalAxis = originalAxis;
            _parentViewModel = parentViewModel;

            _motor = new Motor(this);
            _model = new Model(this);
            _motorFeedback = new MotorFeedback(this);
            _loadFeedback = new LoadFeedback(this);
            _scaling = new Scaling(this);
            _polarity = new Polarity(this);
            _load = new Load(this);
            _backlash = new Backlash(this);
            _compliance = new Compliance(this);
            _friction = new Friction(this);
            _observer = new Observer(this);
            _positionLoop = new PositionLoop(this);
            _velocityLoop = new VelocityLoop(this);
            _accelerationLoop = new AccelerationLoop(this);
            _torqueCurrentLoop = new TorqueCurrentLoop(this);
            _planner = new Planner(this);
            _frequencyControl = new FrequencyControl(this);
            _homing = new Homing(this);
            _actions = new Actions(this);
            _exceptions = new Exceptions(this);
        }

        public string[] GetCompareProperties()
        {
            List<string> properties = new List<string>();
            properties.AddRange(_motor.GetCompareProperties());
            properties.AddRange(_model.GetCompareProperties());
            properties.AddRange(_motorFeedback.GetCompareProperties());
            properties.AddRange(_loadFeedback.GetCompareProperties());
            properties.AddRange(_scaling.GetCompareProperties());
            properties.AddRange(_polarity.GetCompareProperties());
            properties.AddRange(_load.GetCompareProperties());
            properties.AddRange(_backlash.GetCompareProperties());
            properties.AddRange(_compliance.GetCompareProperties());
            properties.AddRange(_friction.GetCompareProperties());
            properties.AddRange(_observer.GetCompareProperties());
            properties.AddRange(_positionLoop.GetCompareProperties());
            properties.AddRange(_velocityLoop.GetCompareProperties());
            properties.AddRange(_accelerationLoop.GetCompareProperties());
            properties.AddRange(_torqueCurrentLoop.GetCompareProperties());
            properties.AddRange(_planner.GetCompareProperties());
            properties.AddRange(_frequencyControl.GetCompareProperties());
            properties.AddRange(_homing.GetCompareProperties());
            properties.AddRange(_actions.GetCompareProperties());
            properties.AddRange(_exceptions.GetCompareProperties());

            properties.Add("GainTuningConfigurationBits");
            properties.Add("PositionIntegratorControl");
            properties.Add("VelocityIntegratorControl");
            properties.Add("InterpolatedPositionConfiguration");
            properties.Add("MasterInputConfigurationBits");
            properties.Add("DynamicsConfigurationBits");
            properties.Add("HomeConfigurationBits");

            return properties.ToArray();

        }

        public string[] GetPeriodicRefreshProperties()
        {
            List<string> properties = new List<string>();

            properties.AddRange(_positionLoop.GetPeriodicRefreshProperties());
            properties.AddRange(_velocityLoop.GetPeriodicRefreshProperties());
            properties.AddRange(_accelerationLoop.GetPeriodicRefreshProperties());
            properties.AddRange(_exceptions.GetPeriodicRefreshProperties());
            properties.AddRange(_actions.GetPeriodicRefreshProperties());
            properties.AddRange(_load.GetPeriodicRefreshProperties());
            properties.AddRange(_friction.GetPeriodicRefreshProperties());
            properties.AddRange(_planner.GetPeriodicRefreshProperties());
            properties.AddRange(_compliance.GetPeriodicRefreshProperties());

            return properties.ToArray();
        }

        public void Refresh()
        {
            _motor.Refresh();
            _model.Refresh();
            _motorFeedback.Refresh();
            _loadFeedback.Refresh();
            _scaling.Refresh();
            _polarity.Refresh();
            _load.Refresh();
            _backlash.Refresh();
            _compliance.Refresh();
            _friction.Refresh();
            _observer.Refresh();
            _positionLoop.Refresh();
            _velocityLoop.Refresh();
            _accelerationLoop.Refresh();
            _torqueCurrentLoop.Refresh();
            _planner.Refresh();
            _frequencyControl.Refresh();
            _homing.Refresh();
            _actions.Refresh();
            _exceptions.Refresh();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateIsChanged([CallerMemberName] string propertyName = null)
        {
            var isChange =
                !CipAttributeHelper.EqualByAttributeName(
                    _currentAxis.CIPAxis,
                    _originalAxis.CIPAxis,
                    propertyName);

            PropertySetting.SetPropertyIsChanged(this, propertyName, isChange);
        }
    }
}
