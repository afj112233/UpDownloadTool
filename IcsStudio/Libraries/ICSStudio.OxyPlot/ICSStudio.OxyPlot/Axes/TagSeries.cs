using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Annotations;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Tags;
using Newtonsoft.Json.Linq;
using OxyPlot.Series;
using DateTime = System.DateTime;
using Pen = System.Windows.Media.Pen;

namespace OxyPlot.Axes
{
    public class TagSeries : LineSeries
    {
        private readonly TrendObject _trend;
        private readonly IPen _pen;

        public TagSeries(string name, Tag tag, TrendObject trend, MaxMinData maxMinData, IPen pen, object syncRoot,bool setParent=true)
        {
            this.syncRoot = syncRoot;
            Name = name;
            Tag = tag;
            _trend = trend;
            MaxMinData = maxMinData;
            if(setParent)
            {
                MaxMinData.SetParent(this);
            }
            _pen = pen;
            PropertyChangedEventManager.AddHandler((pen as PenObject), _pen_PropertyChanged, "");
        }

        public double EndX { set; get; }

        public ConcurrentQueue<DataPoint> BufferPoints { get; internal set; } = new ConcurrentQueue<DataPoint>();

        public TrendObject Trend => _trend;

        /// <summary>
        /// get max value in trend for plotModel's scale
        /// </summary>
        public double Max { private set; get; } = double.NaN;

        /// <summary>
        /// get min value in trend for plotModel's scale
        /// </summary>
        public double Min { private set; get; } = double.NaN;

        public IPen Pen => _pen;

        public EventHandler UpdateEventHandler;

        private void _pen_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Units")
            {
                MaxMinData.Units = _pen.Units;
                return;
            }
            if (e.PropertyName == "Name")
            {
                Name = _pen.Name;
                MaxMinData.Name = _pen.Name;
                MaxMinData.CaptionLength = MaxMinData.CaptionLength;
                UpdateEventHandler?.Invoke(this,new EventArgs());
                return;
            }
            if (e.PropertyName == "Visible")
            {
                IsVisible = _pen.Visible;
                if (MaxMinData != null)
                {
                    MaxMinData.IsVisible = IsVisible;
                }
                
                if (PlotModel != null && PlotModel.InIsolated)
                {
                    if (_trend.Pens.Any(p => p.Visible))
                    {
                        PlotModel.Visibility = IsVisible ? Visibility.Visible : Visibility.Collapsed;
                        foreach (var plotModel in PlotModel.ParentCollection)
                        {
                            if (plotModel.IsTmpVisibility && plotModel != PlotModel)
                            {
                                plotModel.Visibility = Visibility.Collapsed;
                                plotModel.IsTmpVisibility = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        PlotModel.Visibility = Visibility.Visible;
                        PlotModel.IsTmpVisibility = true;
                    }
                }
                else
                {
                    if (LeftAxes.IsAxisVisible)
                    {
                        var series = PlotModel?.Series.FirstOrDefault(s => s.IsVisible);
                        if (series != null)
                        {
                            ((TagSeries) series).LeftAxes.IsAxisVisible = true;
                        }

                        LeftAxes.IsAxisVisible = false;
                    }
                }
            }

            if (e.PropertyName == "Style")
                LineStyle = ChooseLine(_pen.Style);
            if (e.PropertyName == "Width")
            {
                StrokeThickness = _pen.Width;
                MaxMinData?.UpdateWidth(_pen.Width);
            }

            if (e.PropertyName == "Color")
            {
                var color = (Color) ColorConverter.ConvertFromString($"#{FormatOp.RemoveFormat(_pen.Color, false)}");
                color.A = 0xff;
                //if(_trend.Background=="White")
                    //color = (color == Colors.White) ? Colors.Black : color;
                Color = OxyColor.Parse(color.ToString());
                if (MaxMinData != null) MaxMinData.PenColor = new SolidColorBrush(color);
            }

        }


        public MaxMinData MaxMinData { get; }
        public string Name { set; get; }

        public DataPoint LastDataPoint => Points.Any() ? Points.Last() : (new DataPoint(0,0));

        public new Tag Tag { get; private set; }

        public void UpdateTag(Tag tag)
        {
            Tag = tag;
        }

        public void Clean(bool isStart, bool isClose = false)
        {
            //IsError = false;
            Points.Clear();

            if (!isClose && isStart)
            {

            }
            else
                PropertyChangedEventManager.RemoveHandler((_pen as PenObject), _pen_PropertyChanged, "");

        }

        public bool HasData()
        {
            if (Points.Count > 0) return true;
            return false;
        }

        public static LineStyle ChooseLine(Style style)
        {
            switch (style)
            {
                case Style.Style2:
                    return LineStyle.LongDash;
                case Style.Style3:
                    return LineStyle.Dot;
                case Style.Style4:
                    return LineStyle.LongDashDot;
                case Style.Style5:
                    return LineStyle.LongDashDotDot;
                default:
                    return LineStyle.Automatic;
            }
        }

        public void Sync(TagSeries series)
        {
            if (Points != series.Points)
            {
                Points = series.Points;
                BufferPoints = series.BufferPoints;
            }
        }
        
        public TagSeries GetSyncSeries()
        {
            var series=new TagSeries(Name,Tag,_trend,MaxMinData,_pen,syncRoot,false);
            series.IsVisible = false;
            series.Points = Points;
            series.BufferPoints = BufferPoints;
            return series;
        }
    }
    
    public class MaxMinData : INotifyPropertyChanged
    {
        private double _max;
        private double _min;
        private SolidColorBrush _penColor;
        private int _width;
        private Visibility _valueVisibility;
        private int _captionLength;
        private string _displayName;
        private bool _isVisible=true;
        private string _name;
        private TagSeries _parent;
        private Visibility _visibility;
        private Visibility _crossVisibility=Visibility.Collapsed;

        public MaxMinData(Color background, Color penColor, int width, string name, Visibility valueVisibility,
            int captionLength,string units, bool isVisible, bool isIsolated,bool isReal)
        {
            IsReal = isReal;
            Background = new SolidColorBrush(background);
            PenColor = new SolidColorBrush(penColor);
            Width = width * 2;
            Name = name;
            ValueVisibility = valueVisibility;
            CaptionLength = captionLength;
            IsVisible = isVisible;
            IsIsolated = isIsolated;
            Units = units;
        }

        public Visibility HeaderVisibility
        {
            set
            {
                if (_headerVisibility != value)
                {
                    _headerVisibility = value;
                    OnPropertyChanged();
                }
            }
            get
            {
                return _headerVisibility;
            }
        }

        public Visibility SelectedStateVisibility
        {
            set
            {
                _selectedStateVisibility = value; 
                OnPropertyChanged();
            }
            get { return _selectedStateVisibility; }
        }

        public bool IsIsolated { get; }

        internal void SetParent(TagSeries parent)
        {
            _parent = parent;
        }

        public Visibility CrossVisibility
        {
            set
            {
                _crossVisibility = value; 
                OnPropertyChanged();
            }
            get { return _crossVisibility; }
        }

        public string Units
        {
            set
            {
                if (_units!= value)
                {
                    _units = value;
                    OnPropertyChanged();
                }
            }
            get { return _units; }
        }

        public bool IsVisible
        {
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    Visibility = value ? Visibility.Visible : Visibility.Collapsed;

                    OnPropertyChanged();
                }
            }
            get { return _isVisible; }
        }

        public Visibility Visibility
        {
            set
            {
                _visibility = value;
                OnPropertyChanged();
            }
            get { return _visibility; }
        }

        public int CaptionLength
        {
            set
            {
                _captionLength = value;
                if (Name.Length > value)
                {
                    DisplayName = Name.Substring(0, value);
                }
                else
                {
                    DisplayName = Name;
                }

                OnPropertyChanged();
            }
            get { return _captionLength; }
        }

        public Visibility ValueVisibility
        {
            set
            {
                _valueVisibility = value;
                OnPropertyChanged();
            }
            get { return _valueVisibility; }
        }

        public void UpdateWidth(int width)
        {
            Width = width * 2;
        }

        public string Name
        {
            set
            {
                _name = value;
                OnPropertyChanged();
            }
            get { return _name; }
        }

        public string DisplayName
        {
            set
            {
                _displayName = value;
                OnPropertyChanged();
            }
            get { return _displayName; }
        }

        public SolidColorBrush Background
        {
            set
            {
                _background = value;
                OnPropertyChanged();
            }
            get { return _background; }
        }

        public SolidColorBrush PenColor
        {
            set
            {
                _penColor = value;
                OnPropertyChanged();
            }
            get { return _penColor; }
        }

        public int Width
        {
            private set
            {
                _width = value;
                OnPropertyChanged();
            }
            get { return _width; }
        }

        public string Format { set; get; }

        public string ShowMax =>
            ((PlotModel) _parent.Parent)?.Axes.FirstOrDefault(a => !a.IsHorizontal()&&a.IsAxisVisible)?.FormatValue(Max);

        public string ShowMin =>
            ((PlotModel) _parent.Parent)?.Axes.FirstOrDefault(a => !a.IsHorizontal() && a.IsAxisVisible)?.FormatValue(Min);

        internal double Max
        {
            set
            {
                _max = value;
                OnPropertyChanged("ShowMax");
            }
            get { return _max; }
        }

        internal double Min
        {
            set
            {
                _min = value;
                OnPropertyChanged("ShowMin");
            }
            get { return _min; }
        }

        public void Update()
        {
            var axisY = _parent.PlotModel.Axes.FirstOrDefault(a => !a.IsHorizontal()&&a.IsAxisVisible);
            if (IsIsolated)
            {
                Max = axisY?.ActualMaximum ??
                      0;
                Min = axisY?.ActualMinimum ??
                      0;
            }
            else
            {
                if (_parent.Trend.ScaleOption == ICSStudio.Interfaces.Common.ScaleOption.Same||_parent.Trend.ScaleOption==ICSStudio.Interfaces.Common.ScaleOption.UsingPen)
                {

                    Max = axisY?.ActualMaximum ??
                          0;
                    Min = axisY?.ActualMinimum ??
                          0;
                }
                else
                {
                    Max = _parent.LeftAxes.ActualMaximum;
                    Min = _parent.LeftAxes.ActualMinimum;
                }
            }
            OnPropertyChanged("ShowValue");
        }
        public bool IsReal { get; } = false;
        public string ShowValue
        {
            get
            {
                var v = IsReal?((PlotModel)_parent.Parent)?.Axes.FirstOrDefault(a => !a.IsHorizontal())?.FormatValue(Value):Value.ToString();
                if (!IsReal)
                {
                    if (!string.IsNullOrWhiteSpace(v) && v.IndexOf(".") > 0)
                    {
                        v = v.Substring(0, v.IndexOf("."));
                    }
                }
                return FixValue(v);
            }
        }

        private string FixValue(string value)
        {
            value = value.Replace(",", "");
            if (value.Contains("e")) return value;
            var f = value.Split('.');
            if (f.Length > 1 && value.Length - 1 > 9)
            {
                if (Math.Abs(double.Parse(value)) < 100)
                {
                    return Math.Round(double.Parse(value), 10 - f[0].Replace("-", "").Length).ToString($"F{10 - f[0].Length}");
                }

                return Math.Round(double.Parse(value), 9 - f[0].Replace("-", "").Length).ToString($"F{9 - f[0].Length}");
            }

            return value;
        }

        public double Value
        {
            set
            {
                _value = value;
                OnPropertyChanged("ShowValue");
            }
            get { return _value; }
        }
        private double _value;
        private string _units;
        private Visibility _selectedStateVisibility=Visibility.Collapsed;
        private Visibility _headerVisibility=Visibility.Collapsed;
        private SolidColorBrush _background;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
    public class DataCenter
    {
        //private List<TagSeries> _series;
        private Thread _bufferToDataThread;
        private readonly ITrend _trend;
        private PlotModel _plotModel;
        private ObservableCollection<PlotModel> _isolatePlotModels;
        private bool _isStandard;
        public DataCenter(ITrend trend)
        {
            _trend = trend;
        }

        public void TurnToStandard()
        {
            _isStandard = true;
        }

        public void TurnToPenAxis()
        {
            _isStandard = false;
            _penIndex = 0;
            foreach (var pen in _trend.Pens)
            {
                if(pen.Name==_trend.AxisPenName)break;
                _penIndex++;                                                                   
            }
        }

        private int _penIndex;
        public void AddPlotModel(PlotModel model)
        {
            _plotModel = model;
        }

        public void AddPlotModel(ObservableCollection<PlotModel> models)
        {
            _isolatePlotModels = models;
        }

        public bool IsEnable
        {
            set
            {
                _isEnable = value;
                if (!_isEnable)
                {
                    Stop();
                }
            }
            get { return _isEnable; }
        }

        public void Stop()
        {
            StopThread();
        }

        public void Clean()
        {
            while (!_bufferData.IsEmpty)
            {
                List<string> data;
                _bufferData.TryDequeue(out data);
            }

            Data.Clear();
        }

        public Controller Controller => (Controller) _trend.ParentController;

        public void Enqueue(List<string> data)
        {
            //缓存数据过多是否要舍弃部分数据
            _bufferData.Enqueue(data);
            //Task.Run(() => { StartThread(); });
                StartThread();
        }

        private void StartThread()
        {
            if (_isInGettingData || !IsEnable) return;

            if (Interlocked.CompareExchange(ref _isAlive, 1, 0) == 0)
            {
                _isQuit = 0;
                _bufferToDataThread = new Thread(BufferToData);
                _bufferToDataThread.Start();
            }
        }

        private void StopThread()
        {
            if (_bufferToDataThread == null) return;
            Interlocked.Exchange(ref _isQuit, 1);
            while (true)
            {
                Interlocked.Exchange(ref _isQuit, 1);
                if (!_bufferToDataThread.IsAlive)
                {
                    return;
                }

                Thread.Sleep(_trend.SamplePeriod);
            }
        }

        private int _isQuit = 0;
        private int _isAlive = 0;

        private void BufferToData()
        {
            try
            {
                while (_isQuit == 0)
                {
                    List<string> data;
                    var result = _bufferData.TryDequeue(out data);
                    if (result)
                    {
                        Data.Add(data);
                        if (Data.Count > _trend.ExtraData + _trend.CaptureSize)
                        {
                            Data.RemoveAt(0);
                        }

                        continue;
                    }

                    if (_isQuit == 1) return;
                    Thread.Sleep(_trend.SamplePeriod);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                Debug.Assert(false);
            }
            finally
            {
                Interlocked.Exchange(ref _isAlive, 0);
            }
        }

        //public object DataSyncObject { get; } = new object();

        private bool _isInGettingData = false;
        private DateTime _lastTime;

        public void GetTimeSpanData(DateTime time)
        {
            int index = 0;
            try
            {
                _lastTime = time;
                _isInGettingData = true;
                StopThread();
                if (!Data.Any())
                {
                    _isInGettingData = false;
                    return;
                }

                index++;
                var startTime = _trend is TrendLog?_trend.StartTime:_trend.RunTime;
                var timeSpan = _trend.TimeSpan;
                var start = Data.AsParallel().LastOrDefault(d => d != null && DateTime.Parse(d[0]) <= time); index++;
                var end = Data.AsParallel().FirstOrDefault(d => d != null && DateTime.Parse(d[0]) >= time + timeSpan); index++;
                int offset = -1, endOffset = -1;
                if (start != null)
                {
                    offset = Data.IndexOf(start);
                }
                else
                {
                    //start=new List<string>();
                    //start.Add(time.ToString("hh:mm:ss.fff"));
                    //for (int i = 0; i < _series.Count; i++)
                    //{
                    //    start.Add(float.NaN.ToString());
                    //}

                    offset = Data.IndexOf(Data.AsParallel()
                        .FirstOrDefault(d => d != null && DateTime.Parse(d[0]) >= time));
                }
                index++;
                if (end != null)
                {
                    endOffset = Data.IndexOf(end);
                }
                else
                {
                    //end = new List<string>();
                    //end.Add((time + timeSpan).ToString("hh:mm:ss.fff"));
                    //for (int i = 0; i < _series.Count; i++)
                    //{
                    //    end.Add(float.NaN.ToString());
                    //}
                    endOffset = Data.IndexOf(Data.AsParallel().LastOrDefault(d =>
                        d != null && DateTime.Parse(d[0]) <= time + timeSpan));
                }
                index++;
                //x = (DateTime.Parse(start[0]) - startTime).TotalMilliseconds * (double) _trend.SamplePeriod / 1000;
                //for (int i = 1; i <= _series.Count; i++)
                //{
                //    var series = _series[i - 1];
                //    series.Points.Add(new DataPoint(x, Double.Parse(start[i])));
                //}
                if (offset < 0)
                {
                    if (_trend is TrendLog)
                    {
                        StopThread();
                        ((TrendLog) _trend).IsScrolling = false;
                        return;
                    }

                    {
                        _isInGettingData = false;
                        StartThread();
                        return;
                    }
                }

                if (offset >= endOffset && _trend is TrendLog)
                {
                    StopThread();
                    ((TrendLog) _trend).IsScrolling = false;
                    return;
                }
                Parallel.For(0, _plotModel.Series.Count, i =>
                {
                    var series = (TagSeries)_plotModel.Series[i];
                    while (!series.BufferPoints.IsEmpty)
                    {
                        DataPoint d;
                        series.BufferPoints.TryDequeue(out d);
                    }
                });
                _plotModel.IsBeginning = offset == 0;
                foreach (var isolatePlotModel in _isolatePlotModels)
                {
                    isolatePlotModel.IsBeginning = offset == 0;
                }
                if (offset < endOffset)
                {
                    for (int i = offset; i <= endOffset; i++)
                    {
                        var data = Data[i];
                        if (i == offset)
                        {
                            DataStartTime = DateTime.Parse(data[0]);
                            Signal = (DataStartTime - startTime).TotalSeconds;

                        }

                        if (endOffset == i)
                        {
                            DataEndTime = DateTime.Parse(data[0]);
                        }
                        var x = _isStandard ? (DateTime.Parse(data[0]) - startTime).TotalSeconds : double.Parse(data[_penIndex + 1]);
                        for (int j = 1; j <= _plotModel.Series.Count; j++)
                        {
                            var series = (TagSeries)_plotModel.Series[j - 1];
                            series.BufferPoints.Enqueue(new DataPoint(x, Double.Parse(data[j])));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //ignore
                Debug.Assert(false, e.Message);
            }
            finally
            {
                _isInGettingData = false;
                StartThread();
            }
        }

        public DateTime DataStartTime { get; private set; }
        public DateTime DataEndTime { get; private set; }

        public double Signal { private set; get; }

        public void Recover()
        {
            _isInGettingData = true;
            StopThread();
            if (!Data.Any()) return;
            var time = DateTime.Parse(Data[Data.Count - 1][0]) - _trend.TimeSpan;
            GetTimeSpanData(time);
        }

        public void ToOldest()
        {
            _isInGettingData = true;
            StopThread();
            if (!Data.Any()) return;
            var time = DateTime.Parse(Data[0][0]);
            GetTimeSpanData(time);
        }

        private readonly ConcurrentQueue<List<string>> _bufferData = new ConcurrentQueue<List<string>>();
        private bool _isEnable;
        public List<List<string>> Data { get; private set; } = new List<List<string>>();

        public bool CanFront(DateTime time)
        {
            _isInGettingData = true;
            StopThread();
            if (Data.Any(d => DateTime.Parse(d[0]) < time))
            {
                _isInGettingData = false;
                StartThread();
                return true;
            }

            _isInGettingData = false;
            StartThread();
            return false;
        }

        public bool CanBack(DateTime time)
        {
            if (Data.Count <= 0) return false;
            var lastTime = DateTime.Parse(Data.Last()[0]);
            if (time < lastTime)
            {
                var start = DateTime.Parse(Data[0][0]);
                if ((lastTime - start).TotalSeconds > _trend.TimeSpan.TotalSeconds / 2)
                    return true;
            }
            return false;
        }

        public void UpdateSize()
        {
            if (_lastTime != default(DateTime))
            {
                _isInGettingData = true;
                StopThread();

                GetTimeSpanData(_lastTime);
            }
        }

        public void SetLogData(List<List<string>> data)
        {
            Data = data;
        }
    }
}
