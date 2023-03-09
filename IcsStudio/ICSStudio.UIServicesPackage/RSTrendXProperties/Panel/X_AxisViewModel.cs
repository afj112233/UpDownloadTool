using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;

namespace ICSStudio.UIServicesPackage.RSTrendXProperties.Panel
{
    public enum SpanTimeType
    {
        [EnumMember(Value = "Second(s)")] Second,
        [EnumMember(Value = "Minute(s)")] Minute,
        [EnumMember(Value = "Hour(s)")] Hour,
        [EnumMember(Value = "Day(s)")] Day
    }

    class X_AxisViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private readonly ITrend _trend;
        private int _hour;
        private int _minute;
        private int _second;
        private bool _isDirty;
        private DateTime? _date;
        private SpanTimeType _selectedTimeType;
        private int _timeSpan;
        private bool _displayScale;
        private bool _displayDateOnScale;
        private int _majorGridLines;
        private int _minorGridLines;
        private Color _gridLinesColor;
        private bool _displayGridLines;
        private bool _datePickerEnable;

        public X_AxisViewModel(X_Axis panel, ITrend trend)
        {
            panel.DataContext = this;
            Control = panel;
            _trend = trend;
            Date = trend.StartTime;
            Hour = trend.StartTime.Hour;
            Minute = trend.StartTime.Minute;
            Second = trend.StartTime.Second;
            TimeTypeList = EnumHelper.ToDataSource<SpanTimeType>();
            if (!_trend.TimeSpan.TotalDays.ToString().Contains("."))
            {
                TimeSpan = (int)_trend.TimeSpan.TotalDays;
                SelectedTimeType = SpanTimeType.Day;
            }
            else if (!_trend.TimeSpan.TotalHours.ToString().Contains("."))
            {
                TimeSpan = (int)_trend.TimeSpan.TotalHours;
                SelectedTimeType = SpanTimeType.Hour;
            }
            else if (!_trend.TimeSpan.TotalMinutes.ToString().Contains("."))
            {
                TimeSpan = (int)_trend.TimeSpan.TotalMinutes;
                SelectedTimeType = SpanTimeType.Minute;
            }
            else
            {
                TimeSpan = (int)_trend.TimeSpan.TotalSeconds;
                SelectedTimeType = SpanTimeType.Second;
            }
            DisplayScale = trend.DisplayXScale;
            DisplayDateOnScale = trend.DisplayDateOnScale;
            DatePickerEnable = !trend.IsScrolling;
            DisplayGridLines = trend.DisplayGridLinesX;
            MajorGridLines = trend.MajorGridLinesX;
            MinorGridLines = trend.MinorGridLinesX;
            GridLinesColor = (Color) ColorConverter.ConvertFromString($"#{FormatOp.RemoveFormat(trend.GridColorX)}");

            IsDirty = false;
            PropertyChangedEventManager.AddHandler(_trend, Trend_PropertyChanged, "IsScrolling");
        }

        private void Trend_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            DatePickerEnable = !_trend.IsScrolling;
        }

        public override void Cleanup()
        {
            PropertyChangedEventManager.RemoveHandler(_trend, Trend_PropertyChanged, "IsScrolling");
        }

        public int MajorGridLines
        {
            set
            {
                _majorGridLines = value;
                IsDirty = true;
            }
            get { return _majorGridLines; }
        }

        public int MinorGridLines
        {
            set
            {
                _minorGridLines = value;
                IsDirty = true;
            }
            get { return _minorGridLines; }
        }

        public Color GridLinesColor
        {
            set
            {
                _gridLinesColor = value;
                IsDirty = true;
            }
            get { return _gridLinesColor; }
        }

        public bool DisplayGridLines
        {
            set
            {
                Set(ref _displayGridLines , value);
                IsDirty = true;
            }
            get { return _displayGridLines; }
        }

        public bool DatePickerEnable
        {
            set
            {
                Set(ref _datePickerEnable , value);
                TipVisibility = DatePickerEnable ? Visibility.Collapsed : Visibility.Visible;
                RaisePropertyChanged("TipVisibility");
            }
            get { return _datePickerEnable; }
        }

        public Visibility TipVisibility { set; get; }

        public int TimeSpan
        {
            set
            {
                _timeSpan = value;
                IsDirty = true;
            }
            get { return _timeSpan; }
        }

        public IList TimeTypeList { get; }

        public SpanTimeType SelectedTimeType
        {
            set
            {
                _selectedTimeType = value;
                IsDirty = true;
            }
            get { return _selectedTimeType; }
        }

        public DateTime? Date
        {
            set
            {
                _date = value;
                IsDirty = true;
            }
            get { return _date; }
        }

        public int Hour
        {
            set
            {
                Set(ref _hour, value);
                IsDirty = true;
            }
            get { return _hour; }
        }

        public int Minute
        {
            set
            {
                Set(ref _minute, value);
                IsDirty = true;
            }
            get { return _minute; }
        }

        public int Second
        {
            set
            {
                Set(ref _second, value);
                IsDirty = true;
            }
            get { return _second; }
        }

        public bool Verify()
        {
            return true;
        }

        public bool IsClosing { set; get; }
        public void Save()
        {
            if(IsClosing)return;
            if (!IsDirty) return;
            var trend = _trend as TrendObject;
            if (trend != null)
            {
                var date = Date == null
                    ? DateTime.Now
                    : new DateTime(Date.Value.Year, Date.Value.Month, Date.Value.Day, Hour, Minute, Second);

                trend.StartTime = date;
                trend.TimeSpan = ConvertToTimeSpan();
                trend.DisplayXScale = DisplayScale;
                trend.DisplayDateOnScale = DisplayDateOnScale;
                trend.MajorGridLinesX = MajorGridLines;
                trend.MinorGridLinesX = MinorGridLines;
                trend.DisplayGridLinesX = DisplayGridLines;
                trend.GridColorX = "16" + GridLinesColor.ToString();
            }

            IsDirty = false;
        }

        private TimeSpan ConvertToTimeSpan()
        {
            switch (SelectedTimeType)
            {
                case SpanTimeType.Minute:
                    return new TimeSpan(0, 0, TimeSpan, 0);
                case SpanTimeType.Hour:
                    return new TimeSpan(0, TimeSpan, 0, 0);
                case SpanTimeType.Day:
                    return new TimeSpan(TimeSpan, 0, 0, 0);
            }

            return new TimeSpan(0, 0, 0, TimeSpan);
        }

        public bool DisplayScale
        {
            set
            {
                _displayScale = value;
                IsDirty = true;
            }
            get { return _displayScale; }
        }

        public bool DisplayDateOnScale
        {
            set
            {
                _displayDateOnScale = value;
                IsDirty = true;
            }
            get { return _displayDateOnScale; }
        }

        public object Owner { get; set; }
        public object Control { get; }

        public void LoadOptions()
        {

        }

        public bool SaveOptions()
        {
            return true;
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                _isDirty = value;
                IsDirtyChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler IsDirtyChanged;
    }
}
