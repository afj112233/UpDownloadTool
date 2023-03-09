using GalaSoft.MvvmLight;
using ICSStudio.MultiLanguage;
using ICSStudio.Utils;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    class CalculateJerkViewModel : ViewModelBase
    {
        private double _accel;

        private double _jerk;

        private double _percentTime;

        private string _positionUnits;

        private double _speed;

        private string _title;

        public CalculateJerkViewModel(
            string title,
            string positionUnits,
            double speed, double accel, double jerk)
        {
            _title = LanguageManager.GetInstance().ConvertSpecifier(title);
            PositionUnits = positionUnits;
            Speed = speed;
            Accel = accel;
            Jerk = jerk;

            _percentTime = JerkRateCalculation.CalculatePercentTime(Speed, Accel, Jerk);
        }

        public string Title => LanguageManager.GetInstance().ConvertSpecifier("Calculate") +" " + _title;

        public string Label => _title;

        public string PositionUnits
        {
            get { return _positionUnits; }
            set { Set(ref _positionUnits, value); }
        }

        public double Speed
        {
            get { return _speed; }
            set { Set(ref _speed, value); }
        }

        public double Accel
        {
            get { return _accel; }
            set { Set(ref _accel, value); }
        }

        public double Jerk
        {
            get { return _jerk; }
            set { Set(ref _jerk, value); }
        }

        public double PercentTime
        {
            get { return _percentTime; }
            set
            {
                Set(ref _percentTime, value);

                Jerk = JerkRateCalculation.CalculateJerk(Speed, Accel, value < 0.01 ? 0.01f : value);
            }
        }
    }
}
