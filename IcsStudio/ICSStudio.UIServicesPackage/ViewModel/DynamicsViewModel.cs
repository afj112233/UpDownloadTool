using System;
using System.Runtime.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    public class DynamicsViewModel : ViewModelBase
    {
        private double _accelerationTime;
        private double _decelerationTime;
        private double _acceleration;
        private double _deceleration;
        private double _accelerationCharacteristic = 100.0;
        private double _decelerationCharacteristic = 100.0;
        private double _maximumVelocity;
        private double _jerk;
        private bool _isIndirectAsAbove = true;
        private bool _isDirectAsAbove = true;
        private bool? _dialogResult;
        private Pattern _calculatePattern;
        private ApplyValue _applyValue;

        public DynamicsViewModel(string positionUnits, double speed, double celeration, double jerk)
        {
            PositionUnits = positionUnits;
            _maximumVelocity = speed;
            _acceleration = celeration;
            _deceleration = celeration;
            _jerk = jerk;

            _accelerationTime = CompareDoubleIsEqual(jerk, 0) ? 0 : 2.0 * speed / celeration;
            _decelerationTime = _accelerationTime;

            _applyValue = new ApplyValue()
            {
                MaximumVelocity = MaximumVelocity,
                AccelerationTime = AccelerationTime,
                DecelerationTime = DecelerationTime,
                AccelerationCharacteristic = AccelerationCharacteristic,
                DecelerationCharacteristic = DecelerationCharacteristic,
                Acceleration = Acceleration,
                Deceleration = Deceleration,
                Jerk = Jerk
            };

            OKCommand = new RelayCommand(ExecuteOKCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            ApplyCommand = new RelayCommand(ExecuteApplyCommand, CanApplyCommand);
        }

        public bool? DialogResult
        {
            set { Set(ref _dialogResult, value); }
            get { return _dialogResult; }
        }

        public Pattern CalculatePattern
        {
            get { return _calculatePattern; }
            set { Set(ref _calculatePattern,value); }
        }

        public string PositionUnits { get; }

        public bool IsIndirectAsAbove
        {
            get { return _isIndirectAsAbove; }
            set
            {
                Set(ref _isIndirectAsAbove, value);
                if (value)
                {
                    _decelerationTime = AccelerationTime;
                    RaisePropertyChanged(nameof(DecelerationTime));
                }
            }
        }

        public bool IsDirectAsAbove
        {
            get { return _isDirectAsAbove; }
            set
            {
                Set(ref _isDirectAsAbove, value);
                if (value)
                {
                    _deceleration = Acceleration;
                    RaisePropertyChanged(nameof(Deceleration));
                }
            }
        }

        public double MaximumVelocity
        {
            get { return _maximumVelocity; }
            set
            {
                if (value < double.Epsilon) return;
                if (!CompareDoubleIsEqual(_maximumVelocity,value))
                {
                    _maximumVelocity = value;
                    RaisePropertyChanged(nameof(MaximumVelocity));
                    Calculate();
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public double AccelerationTime
        {
            get { return _accelerationTime; }
            set
            {
                if (value < double.Epsilon) return;
                if (!CompareDoubleIsEqual(_accelerationTime, value))
                {
                    _accelerationTime = value;
                    RaisePropertyChanged(nameof(AccelerationTime));
                    if (IsIndirectAsAbove)
                    {
                        _decelerationTime = value;
                        RaisePropertyChanged(nameof(DecelerationTime));
                    }
                    if (CalculatePattern == Pattern.Indirect) Calculate();
                }
            }
        }

        public double DecelerationTime
        {
            get { return _decelerationTime; }
            set
            {
                if (value < double.Epsilon) return;
                if (!CompareDoubleIsEqual(_decelerationTime, value))
                {
                    _decelerationTime = value;
                    RaisePropertyChanged(nameof(DecelerationTime));
                    if (IsIndirectAsAbove)
                    {
                        _accelerationTime = value;
                        RaisePropertyChanged(nameof(AccelerationTime));
                    }
                    if(CalculatePattern == Pattern.Indirect) Calculate();
                }
            }
        }

        public double AccelerationCharacteristic
        {
            get { return _accelerationCharacteristic; }
            set
            {
                _accelerationCharacteristic = value;
                RaisePropertyChanged(nameof(AccelerationCharacteristic));
                if (IsIndirectAsAbove)
                {
                    _decelerationCharacteristic = value;
                    RaisePropertyChanged(nameof(DecelerationCharacteristic));
                }
                if(CalculatePattern == Pattern.Indirect) Calculate();
            }
        }

        public double DecelerationCharacteristic
        {
            get { return _decelerationCharacteristic; }
            set
            {
                _decelerationCharacteristic = value;
                RaisePropertyChanged(nameof(DecelerationCharacteristic));
                if (IsIndirectAsAbove)
                {
                    _accelerationCharacteristic = value;
                    RaisePropertyChanged(nameof(AccelerationCharacteristic));
                }
                if (CalculatePattern == Pattern.Indirect) Calculate();
            }
        }

        public double Acceleration
        {
            get { return _acceleration; }
            set
            {
                if (value < double.Epsilon) return;
                if (!CompareDoubleIsEqual(_acceleration, value))
                {
                    _acceleration = value;
                    RaisePropertyChanged(nameof(Acceleration));
                    if (IsDirectAsAbove)
                    {
                        _deceleration = value;
                        RaisePropertyChanged(nameof(Deceleration));
                    }
                    if (CalculatePattern == Pattern.Direct) Calculate();
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public double Deceleration
        {
            get { return _deceleration; }
            set
            {
                if (value < double.Epsilon) return;
                if (!CompareDoubleIsEqual(_deceleration, value))
                {
                    _deceleration = value;
                    RaisePropertyChanged(nameof(Deceleration));
                    if (IsDirectAsAbove)
                    {
                        _acceleration = value;
                        RaisePropertyChanged(nameof(Acceleration));
                    }
                    if(CalculatePattern == Pattern.Direct) Calculate();
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public double Jerk
        {
            get { return _jerk; }
            set
            {
                if (value < double.Epsilon) return;
                if (!CompareDoubleIsEqual(_jerk, value))
                {
                    _jerk = value;
                    RaisePropertyChanged(nameof(Jerk));
                    if (CalculatePattern == Pattern.Direct) Calculate();
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public RelayCommand OKCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand ApplyCommand { get; }
        public Action ApplyAction { get; set; }

        public void ExecuteOKCommand()
        {
            DialogResult = true;
        }

        public void ExecuteCancelCommand()
        {
            DialogResult = false;
        }

        public void ExecuteApplyCommand()
        {
            ApplyAction?.Invoke();

            _applyValue.MaximumVelocity = MaximumVelocity;
            _applyValue.Acceleration = Acceleration;
            _applyValue.Deceleration = Deceleration;
            _applyValue.Jerk = Jerk;

            ApplyCommand.RaiseCanExecuteChanged();
        }

        public bool CanApplyCommand()
        {
            return IsDirty();
        }

        public bool IsDirty()
        {
            var result = !(CompareDoubleIsEqual(_applyValue.MaximumVelocity, MaximumVelocity)
                           && CompareDoubleIsEqual(_applyValue.Acceleration, Acceleration)
                           && CompareDoubleIsEqual(_applyValue.Deceleration, Deceleration)
                           && CompareDoubleIsEqual(_applyValue.Jerk, Jerk));
            return result;
        }

        public bool CompareDoubleIsEqual(double num1, double num2)
        {
            return Math.Abs(num1 - num2) < double.Epsilon;
        }

        public override void Cleanup()    
        {
            base.Cleanup();
            ApplyAction = null;
        }

        public void Calculate()
        {
            if (CalculatePattern == Pattern.Indirect)
            {
                var result = CalculateIndirect(MaximumVelocity, AccelerationCharacteristic, AccelerationTime);
                Acceleration = result.Item1;
                Jerk = result.Item2;
            }
            else if (CalculatePattern == Pattern.Direct)
            {
                var result = CalculateDirect(MaximumVelocity, Acceleration, Jerk);
                AccelerationTime = result.Item1;
                AccelerationCharacteristic = result.Item2;
            }
        }

        public Tuple<double, double> CalculateIndirect(double maximumVelocity, double accelerationCharacteristic, double accelerationTime)
        {
            var x = accelerationCharacteristic;
            var vMax = maximumVelocity;
            var t = accelerationTime;
            var boundary = 200.0 / 101.0;
            double acceleration = 0;
            double jerk = 0;

            if (x <= 100 && x > boundary)
            {
                acceleration = 100 * vMax / ((100 - 0.5 * x) * t);
                jerk = 200 * acceleration / (x * t);
            }
            else if (x >= 0 && x < boundary)
            {
                acceleration = 100 * vMax / ((100 - 0.5 * 200.0 / 101.0) * t);
                jerk = 100 * Math.Pow(Acceleration, 2) / vMax;
            }

            return new Tuple<double, double>(acceleration, jerk);
        }

        public Tuple<double, double> CalculateDirect(double maximumVelocity, double acceleration, double jerk)
        {
            var vMax = maximumVelocity;
            var boundary = Math.Pow(acceleration, 2) / jerk;
            double t = 0;
            double x = 0;

            if (vMax <= boundary)
            {
                t = vMax / acceleration + acceleration / jerk;
                x = 100;
            }
            else if (vMax > boundary)
            {
                t = vMax / acceleration + acceleration / jerk;
                x = 100 - 100 * Math.Pow(1 - 4 * vMax / (jerk * Math.Pow(t, 2)), 0.5);
            }

            return new Tuple<double, double>(t, x);
        }

        public enum Pattern
        {
            [EnumMember(Value = "Indirect")] Indirect,
            [EnumMember(Value = "Direct")] Direct
        }

        public struct ApplyValue
        {
            public double MaximumVelocity;
            public double AccelerationTime;
            public double DecelerationTime;
            public double AccelerationCharacteristic;
            public double DecelerationCharacteristic;
            public double Acceleration;
            public double Deceleration;
            public double Jerk;
        }
    }
}
