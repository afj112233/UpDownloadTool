using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.RSTrendXProperties.Panel
{
    class SamplingViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private bool _isDirty;
        private readonly ITrend _trend;
        private int _samplePeriod;
        private int _samples;
        private long _timePeriod;
        private bool _radioChecked1;
        private bool _radioChecked2;
        private bool _radioChecked3;
        private TimeType _selectedSamplePeriod;
        private TimeType _selectedTimePeriod;
        private string _size;
        private bool _allEnable;

        public SamplingViewModel(Sampling panel, ITrend trend)
        {
            panel.DataContext = this;
            Control = panel;
            _trend = trend;
            TimePeriodTypeList = EnumHelper.ToDataSource<TimeType>();
            TimeTypeList = EnumHelper.ToDataSource<TimeType>();
            TimeTypeList.RemoveAt(3);
            switch (trend.CaptureSizeType)
            {
                case CaptureSizeType.Samples:
                    _radioChecked1 = true;
                    break;
                case CaptureSizeType.TimePeriod:
                    _radioChecked2 = true;
                    break;
                case CaptureSizeType.NoLimit:
                    RadioChecked3 = true;
                    break;
            }

            RadioChecked1 = true;
            _selectedSamplePeriod = TimeType.Millisecond;
            _selectedTimePeriod = TimeType.Millisecond;
            _samples = trend.CaptureSize;
            var p = trend.SamplePeriod;
       
            if (p % 1000 == 0)
            {
                p = p / 1000;
                _selectedSamplePeriod = TimeType.Second;
                if (p % 60 == 0)
                {
                    p = p / 60;
                    _selectedSamplePeriod = TimeType.Minute;
                }
            }

            SamplePeriod = p;
            IsDirty = false;
            if (_trend is TrendLog)
            {
                AllEnable = false;
            }
            else
            {
                AllEnable = trend.IsStop;
                PropertyChangedEventManager.AddHandler(_trend, Trend_PropertyChanged, "IsStop");
            }
        }

        private void Trend_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            AllEnable = _trend.IsStop;
        }

        public override void Cleanup()
        {
            PropertyChangedEventManager.RemoveHandler(_trend, Trend_PropertyChanged, "IsStop");
        }

        public string Size
        {
            set { Set(ref _size, value); }
            get { return _size; }
        }

        public bool AllEnable
        {
            set { Set(ref _allEnable, value); }
            get { return _allEnable; }
        }

        public bool RadioChecked1
        {
            set
            {
                _radioChecked1 = value;
                IsDirty = true;
                SamplePeriod = SamplePeriod;
            }
            get { return _radioChecked1; }
        }

        public bool RadioChecked2
        {
            set
            {
                _radioChecked2 = value;
                IsDirty = true;
                SamplePeriod = SamplePeriod;
            }
            get { return _radioChecked2; }
        }

        public bool RadioChecked3
        {
            set
            {
                _radioChecked3 = value;
                IsDirty = true;
            }
            get { return _radioChecked3; }
        }

        public int SamplePeriod
        {
            set
            {
                _samplePeriod = value;
                //if ((float) _samplePeriod * ConvertToMillisecondRate(SelectedSamplePeriod) > 1800000)
                //{
                //    if (SelectedSamplePeriod == TimeType.Minute)
                //        Set(ref _samplePeriod, 30);
                //    if (SelectedSamplePeriod == TimeType.Second)
                //        Set(ref _samplePeriod, 30 * 60);
                //    if (SelectedSamplePeriod == TimeType.Millisecond)
                //        Set(ref _samplePeriod, 30 * 60 * 1000);
                //}

                IsDirty = true;
                if (RadioChecked1)
                {
                    _timePeriod = ((long) _samplePeriod * Samples *
                                   ConvertToMillisecondRate(SelectedSamplePeriod));
                    _selectedTimePeriod = TimeType.Millisecond;
                    AdjustTimePeriod();
                    RaisePropertyChanged("TimePeriod");
                }
                else
                {
                    _samples = (int) ((float) _timePeriod / _samplePeriod *
                                      ConvertToMillisecondRate(SelectedTimePeriod) /
                                      ConvertToMillisecondRate(SelectedSamplePeriod));
                    RaisePropertyChanged("Samples");
                }
            }
            get { return _samplePeriod; }
        }

        private void AdjustTimePeriod()
        {
            if (SelectedTimePeriod == TimeType.Millisecond)
            {
                if (_timePeriod % 1000 == 0)
                {
                    _timePeriod = _timePeriod / 1000;
                    _selectedTimePeriod = TimeType.Second;
                }
            }

            if (SelectedTimePeriod == TimeType.Second)
            {
                if (_timePeriod % 60 == 0)
                {
                    _timePeriod = _timePeriod / 60;
                    _selectedTimePeriod = TimeType.Minute;
                }
            }

            if (SelectedTimePeriod == TimeType.Minute)
            {
                if (_timePeriod % 60 == 0)
                {
                    _timePeriod = (uint) TimePeriod / 60;
                    _selectedTimePeriod = TimeType.Hour;
                }
            }

            RaisePropertyChanged("SelectedTimePeriod");
            RaisePropertyChanged("TimePeriod");
        }

        public int Samples
        {
            set
            {
                _samples = value;
                IsDirty = true;
                SamplePeriod = SamplePeriod;
                var mb = Math.Ceiling((double) (_samples - 68) / 135) * 0.01 + 0.02;
                Size = Math.Abs(mb) < double.Epsilon ? "Not enough information" : $"{mb} MB";
                RaisePropertyChanged("TimePeriod");
            }
            get { return _samples; }
        }

        public long TimePeriod
        {
            set
            {
                _timePeriod = value;
                IsDirty = true;
                _samples = (int) ((long) _timePeriod / SamplePeriod * ConvertToMillisecondRate(SelectedTimePeriod) /
                                  ConvertToMillisecondRate(SelectedSamplePeriod));
                RaisePropertyChanged("Samples");
            }
            get { return _timePeriod; }
        }

        public bool Verify()
        {
            //Sample Periode（P） （1ms≤P≤30min）
            //Samples（S）
            //Time Period（T）

            //Size of Each Capture：Samples
            //    当P≥8s，1≤S≤1000
            //当P < 8s，1≤S≤TRUNC（7200 / P）

            //Size of Each Capture：Time Period
            //当P≥8s，P≤T≤1000P
            //当P < 8s，P≤T≤2hours
            var timePeriod = TimePeriod * ConvertToMillisecondRate(SelectedTimePeriod);
            var period = SamplePeriod * ConvertToMillisecondRate(SelectedSamplePeriod);
            if (period < 1 || period > 30 * 60 * 1000)
            {
                MessageBox.Show(
                    LanguageManager.GetInstance().ConvertSpecifier("FailedToModifyTrendPropertiesSample") +" (1 millisecond - 30 minutes).",
                    "ICS Studio", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
                return false;
            }

            if (RadioChecked1)
            {
                if (period >= 8 * 1000)
                {
                    if (Samples < 1 || Samples > 1000)
                    {
                        MessageBox.Show(
                            LanguageManager.GetInstance().ConvertSpecifier("FailedToModifyTrendPropertiesSample") + " (1 - 1000).",
                            "ICS Studio", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
                        return false;
                    }
                }
                else
                {
                    if (Samples < 1 || Samples > 7200000 / period)
                    {
                        MessageBox.Show(
                            LanguageManager.GetInstance().ConvertSpecifier("FailedToModifyTrendPropertiesSample") + $" (1 - {Math.Floor((decimal) 7200000 / period)}).",
                            "ICS Studio", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
                        return false;
                    }
                }
            }

            if (RadioChecked2)
            {
                if (period >= 8 * 1000)
                {
                    if (!(period <= timePeriod && timePeriod <= 1000 * period))
                    {
                        MessageBox.Show(
                            LanguageManager.GetInstance().ConvertSpecifier("FailedToModifyTrendPropertiesTime") +$" ({SamplePeriod} {SelectedSamplePeriod.ToString()} - {GetAppropriateTime(1000 * period)}).",
                            "ICS Studio", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
                        return false;
                    }
                }
                else
                {
                    if (!(period <= timePeriod && timePeriod <= 2 * 60 * 60 * 1000))
                    {
                        MessageBox.Show(
                            LanguageManager.GetInstance().ConvertSpecifier("FailedToModifyTrendPropertiesTime")+$" ({period} - 2hours).",
                            "ICS Studio", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
                        return false;
                    }
                }
            }

            return true;
        }

        private string GetAppropriateTime(long totalMilliSeconds)
        {
            var timeSpan = TimeSpan.FromMilliseconds(totalMilliSeconds);

            if (Math.Abs((long) timeSpan.TotalHours - timeSpan.TotalHours) < Double.Epsilon)
            {
                return $"{timeSpan.TotalHours} Hour(s)";
            }

            if (Math.Abs((long)timeSpan.TotalMinutes - timeSpan.TotalMinutes) < Double.Epsilon)
            {
                return $"{timeSpan.TotalMinutes} Minute(s)";
            }

            if ((long) timeSpan.TotalSeconds == totalMilliSeconds)
            {
                return $"{timeSpan.TotalMinutes} Second(s)";
            }

            return $"{totalMilliSeconds} Millisecond(s)";
        }

        public TimeType SelectedSamplePeriod
        {
            set
            {
                Set(ref _selectedSamplePeriod, value);
                //SelectedTimePeriod = value;
                SamplePeriod = SamplePeriod;
            }
            get { return _selectedSamplePeriod; }
        }

        public TimeType SelectedTimePeriod
        {
            set
            {
                Set(ref _selectedTimePeriod, value);
                TimePeriod = TimePeriod;
            }
            get { return _selectedTimePeriod; }
        }

        public IList TimeTypeList { get; }
        public IList TimePeriodTypeList { get; }
        public object Owner { get; set; }
        public object Control { get; }

        public void LoadOptions()
        {

        }
        public bool IsClosing { set; get; }
        public void Save()
        {
            if(IsClosing)return;
            if(!IsDirty)return;
            var trend = _trend as TrendObject;
            if (RadioChecked1) trend.CaptureSizeType = CaptureSizeType.Samples;
            if (RadioChecked2) trend.CaptureSizeType = CaptureSizeType.TimePeriod;
            if (RadioChecked3) trend.CaptureSizeType = CaptureSizeType.NoLimit;
            trend.SamplePeriod = SamplePeriod * ConvertToMillisecondRate(SelectedSamplePeriod);
            trend.CaptureSize = Samples;
            IsDirty = false;
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

        private int ChangedTime(int value, TimeType old, TimeType current)
        {
            return value / ConvertToMillisecondRate(old) * ConvertToMillisecondRate(current);
        }

        private int ConvertToMillisecondRate(TimeType type)
        {
            int value = 1;
            if (type == TimeType.Millisecond) return value;
            value = value * 1000;
            if (type == TimeType.Second) return value;
            value = value * 60;
            if (type == TimeType.Minute) return value;
            value = value * 60;
            return value;
        }

    }
}
