using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Annotations;
using Newtonsoft.Json.Linq;
using Type = ICSStudio.Interfaces.Common.Type;

namespace ICSStudio.SimpleServices.Common
{
    public abstract class PenObject : IPen, INotifyPropertyChanged
    {
        protected PenObject(JObject config)
        {
            _config = config;
            ParseConfig();
        }

        protected PenObject()
        {

        }

        public JObject ToJson()
        {
            var pen = new JObject();
            pen["Name"] = Name;
            pen["Color"] = Color;
            pen["Visible"] = Visible;
            pen["Style"] = (int) Style;
            pen["Type"] = Type.ToString();
            pen["Width"] = Width;
            pen["Marker"] = Marker;
            pen["Min"] = Min;
            pen["Max"] = Max;
            pen["EngUnits"] = Units ?? "";
            _config = pen;
            return pen;
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Description { get; set; }

        public string Color
        {
            get { return string.IsNullOrEmpty(_color) ? $"16{Colors.Green.ToString()}" : _color; }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    OnPropertyChanged();
                }
            }
        }

        public Style Style
        {
            get { return _style; }
            set
            {
                if (Style != value)
                {
                    _style = value;
                    OnPropertyChanged();
                }
            }
        }

        public Type Type { get; set; } = Type.Analog;

        public int Width
        {
            get { return _width; }
            set
            {
                if (_width != value)
                {
                    _width = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Marker { get; set; } = 0;
        public float Min { get; set; } = 0;
        public float Max { get; set; } = 100;

        public string Units
        {
            get { return _units; }
            set
            {
                if (_units != value)
                {
                    _units = value;
                    OnPropertyChanged();
                }
            }
        }

        private JObject _config;
        private string _color;
        private string _name;
        private bool _visible = true;
        private Style _style = Style.Style1;
        private int _width = 1;
        private string _units;

        protected virtual void ParseConfig()
        {
            if (_config != null)
            {
                Name = (string) _config["Name"];
                Color = (string) _config["Color"];
                Visible = (bool) _config["Visible"];
                Style = (Style) (byte) _config["Style"];
                Type = _config["Type"].ToObject<Type>();
                Width = (int) _config["Width"];
                Marker = (int) _config["Marker"];
                Min = (float) _config["Min"];
                Max = (float) _config["Max"];
                Units = (string) _config["EngUnits"];
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public abstract class TrendObject : ITrend, INotifyCollectionChanged
    {
        private List<IPen> _pens = new List<IPen>();
        protected JObject _config;
        private string _graphTitle;
        private bool _showGraphTitle = true;
        private bool _displayXScale = true;
        private bool _displayYScale = true;
        private bool _displayDateOnScale = false;
        private DateTime _startTime = DateTime.Now;
        private string _name;
        private string _description;
        private TimeSpan _timeSpan = new TimeSpan(0, 0, 0, 4);
        private int _yScaleDecimalPlaces = 0;
        private int _samplePeriod = 10;
        private bool _displayMillisecond = false;
        private string _background = $"16{Colors.Black.ToString()}";
        private string _textColor = $"16{Colors.Black.ToString()}";
        private bool _displayPenValue = true;
        private bool _displayPenIcons = true;
        private int _extraData = 200;
        private bool _displayLineLegend = true;
        private bool _displayMinAndMaxValue = true;
        private int _penCaptionMaxLength = 40;
        private bool _displayScrollingMechanism = true;
        private int _captureSize = 60000;
        private bool _isScrolling = false;
        private bool _isolated = false;
        private int _majorGridLinesX = 4;
        private int _minorGridLinesX = 0;
        private string _gridColorX = "16#808080";
        private bool _displayGridLinesX = true;
        private int _majorGridLinesY = 4;
        private int _minorGridLinesY = 0;
        private string _gridColorY = "16#808080";
        private bool _displayGridLinesY = true;
        private ValueOptionType _valueOption = ValueOptionType.Automatic;
        private double _actualMinimumValue = 0;
        private double _actualMaximumValue = 100;
        private bool _scaleAsPercentage = false;
        private ScaleOption _scaleOption = ScaleOption.Independent;
        private bool _updateIsolated;
        private bool _isStop = true;

        //public object Lock { get; }=new object();
        protected TrendObject(JObject config, IController controller)
        {
            ParentController = controller;
            _config = config;
            ParseConfig();
            Uid = Guid.NewGuid().GetHashCode();
        }

        protected TrendObject(IController controller)
        {
            ParentController = controller;
            Uid = Guid.NewGuid().GetHashCode();
        }

        public bool IsUpdateScale
        {
            set
            {
                _isUpdateScale = value; 
                OnPropertyChanged();
            }
            get { return _isUpdateScale; }
        }

        public bool IsStop
        {
            set
            {
                _isStop = value;
                OnPropertyChanged();
            }
            get { return _isStop; }
        }

        public ChartStyle ChartStyle
        {
            get { return _chartStyle; }
            set
            {
                if (_chartStyle != value)
                {
                    _chartStyle = value;
                    OnPropertyChanged();
                }

            }
        }

        public string AxisPenName { get; set; } = "";

        public Position Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MaxViewable
        {
            get { return _maxViewable; }
            set
            {
                if (MaxViewable != value)
                {
                    _maxViewable = value;
                    OnPropertyChanged();
                }
            }
        }

        public void Add(IPen pen)
        {
            if (!_pens.Contains(pen))
            {
                _pens.Add(pen);
                CollectionChanged?.Invoke(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, pen));
            }
        }

        public void AddPens(IList<IPen> pens)
        {
            if (pens != null&&pens.Count>0)
            {
                _pens.AddRange(pens);
                CollectionChanged?.Invoke(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, pens.ToArray()));
            }
        }

        public bool ResetView { set; get; }

        public JObject ToJson()
        {
            var trend = new JObject();
            trend["Name"] = Name;
            trend["SamplePeriod"] = SamplePeriod;
            trend["NumberOfCaptures"] = NumberOfCaptures;
            trend["CaptureSizeType"] = CaptureSizeType.ToString();
            trend["CaptureSize"] = CaptureSize;
            trend["StartTriggerType"] = StartTriggerType;
            trend["StopTriggerType"] = StopTriggerType;
            trend["TrendxVersion"] = TrendxVersion;
            trend["Isolated"] = Isolated;
            trend["TimeSpan"] = TimeSpan.TotalSeconds;

            trend["Description"] = Description ?? "";
            trend["GraphTitle"] = GraphTitle ?? "";
            trend["ShowGraphTitle"] = ShowGraphTitle;
            trend["DisplayYScale"] = DisplayYScale;
            trend["YScaleDecimalPlaces"] = YScaleDecimalPlaces;
            trend["DisplayXScale"] = DisplayXScale;
            trend["DisplayDateOnScale"] = DisplayDateOnScale;
            trend["Background"] = Background;
            trend["TextColor"] = TextColor;
            trend["ExtraData"] = ExtraData;
            trend["DisplayMillisecond"] = DisplayMillisecond;
            trend["DisplayPenValue"] = DisplayPenValue;
            trend["DisplayPenIcons"] = DisplayPenIcons;
            trend["DisplayLineLegend"] = DisplayLineLegend;
            trend["DisplayMinAndMaxValue"] = DisplayMinAndMaxValue;
            trend["PenCaptionMaxLength"] = PenCaptionMaxLength;
            trend["DisplayScrollingMechanism"] = DisplayScrollingMechanism;
            trend["MajorGridLinesX"] = MajorGridLinesX;
            trend["MinorGridLinesX"] = MinorGridLinesX;
            trend["GridColorX"] = GridColorX;
            trend["DisplayGridLinesX"] = DisplayGridLinesX;
            trend["MajorGridLinesY"] = MajorGridLinesY;
            trend["MinorGridLinesY"] = MinorGridLinesY;
            trend["GridColorY"] = GridColorY;
            trend["DisplayGridLinesY"] = DisplayGridLinesY;
            trend["ValueOption"] = (byte) ValueOption;
            trend["ActualMinimumValue"] = ActualMinimumValue;
            trend["ActualMaximumValue"] = ActualMaximumValue;
            trend["ScaleAsPercentage"] = ScaleAsPercentage;
            trend["ScaleOption"] = (byte) ScaleOption;
            trend["ChartStyle"] = (byte)ChartStyle;
            trend["AxisPenName"] = AxisPenName;
            trend["Position"] = (byte) Position;
            trend["MaxViewable"] = MaxViewable;
            if (ScalePen != null)
                trend["ScalePen"] = ScalePen.Name;
            var pens = new JArray();
            foreach (var pen in Pens)
            {
                pens.Add(pen.ToJson());
            }

            trend["Pens"] = pens;
            _config = trend;
            return _config;
        }

        public int SamplePeriod
        {
            set
            {
                if (_samplePeriod != value)
                {
                    _samplePeriod = value;
                    OnPropertyChanged();
                }
            }
            get { return _samplePeriod; }
        }

        public int NumberOfCaptures { set; get; } = 1;
        public CaptureSizeType CaptureSizeType { set; get; }

        public int CaptureSize
        {
            set
            {
                if (_captureSize != value)
                {
                    _captureSize = value;
                    OnPropertyChanged();
                }
            }
            get { return _captureSize; }
        }

        public int StartTriggerType { set; get; }
        public int StopTriggerType { set; get; }
        public float TrendxVersion { set; get; }

        public IEnumerable<IPen> Pens
        {
            get
            {
                foreach (var pen in _pens)
                {
                    yield return pen;
                }
            }
        }

        public void ClearPens()
        {
            var pens =
                _pens.ToArray();

            _pens.Clear();
            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, pens));
        }

        public void RemovePen(string penName)
        {
            var pen = _pens.FirstOrDefault(p => p.Name.Equals(penName));
            if (pen != null) _pens.Remove(pen);
        }

        public string GraphTitle
        {
            get { return _graphTitle ?? (_graphTitle = Name); }
            set
            {
                if (_graphTitle != value)
                {
                    _graphTitle = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowGraphTitle
        {
            get { return _showGraphTitle; }
            set
            {
                if (_showGraphTitle != value)
                {
                    _showGraphTitle = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool DisplayYScale
        {
            get { return _displayYScale; }
            set
            {
                if (_displayYScale != value)
                {
                    _displayYScale = value;
                    OnPropertyChanged();
                }
            }
        }

        public int YScaleDecimalPlaces
        {
            get { return _yScaleDecimalPlaces; }
            set
            {
                if (_yScaleDecimalPlaces != value)
                {
                    _yScaleDecimalPlaces = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime StartTime
        {
            get { return _startTime; }
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime RunTime { set; get; }

        public TimeSpan TimeSpan
        {
            get { return _timeSpan; }
            set
            {
                if (_timeSpan != value)
                {
                    _timeSpan = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool DisplayXScale
        {
            get { return _displayXScale; }
            set
            {
                if (_displayXScale != value)
                {
                    _displayXScale = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool DisplayDateOnScale
        {
            get { return _displayDateOnScale; }
            set
            {
                if (_displayDateOnScale != value)
                {
                    _displayDateOnScale = value;
                    OnPropertyChanged();
                }
            }
        }

        private readonly object _syncScroll = new object();
        private ChartStyle _chartStyle;
        private int _maxViewable = 8;
        private Position _position;
        private bool _isUpdateScale;

        public bool IsScrolling
        {
            get { return _isScrolling; }
            set
            {
                if (_isScrolling != value)
                {
                    lock (_syncScroll)
                    {
                        _isScrolling = value;
                        if (!(this is TrendLog) && _isScrolling)
                        {
                            StartTime = DateTime.Now;
                        }

                        OnPropertyChanged();
                    }
                }
            }
        }

        public string Background
        {
            get { return _background; }
            set
            {
                if (_background != value)
                {
                    _background = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TextColor
        {
            get { return _textColor; }
            set
            {
                if (_textColor != value)
                {
                    _textColor = value;
                    OnPropertyChanged();
                }
            }
        }

        public int ExtraData
        {
            get { return _extraData; }
            set
            {
                if (_extraData != value)
                {
                    _extraData = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool DisplayMillisecond
        {
            get { return _displayMillisecond; }
            set
            {
                if (_displayMillisecond != value)
                {
                    _displayMillisecond = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool DisplayPenValue
        {
            get { return _displayPenValue; }
            set
            {
                if (_displayPenValue != value)
                {
                    _displayPenValue = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool DisplayPenIcons
        {
            get { return _displayPenIcons; }
            set
            {
                if (_displayPenIcons != value)
                {
                    _displayPenIcons = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool DisplayLineLegend
        {
            get { return _displayLineLegend; }
            set
            {
                if (_displayLineLegend != value)
                {
                    _displayLineLegend = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool DisplayMinAndMaxValue
        {
            get { return _displayMinAndMaxValue; }
            set
            {
                if (_displayMinAndMaxValue != value)
                {
                    _displayMinAndMaxValue = value;
                    OnPropertyChanged();
                }
            }
        }

        public int PenCaptionMaxLength
        {
            get { return _penCaptionMaxLength; }
            set
            {
                if (_penCaptionMaxLength != value)
                {
                    _penCaptionMaxLength = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool DisplayScrollingMechanism
        {
            get { return _displayScrollingMechanism; }
            set
            {
                if (_displayScrollingMechanism != value)
                {
                    _displayScrollingMechanism = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool Isolated
        {
            get { return _isolated; }
            set
            {
                if (_isolated != value)
                {
                    _isolated = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MajorGridLinesX
        {
            get { return _majorGridLinesX; }
            set
            {
                if (_majorGridLinesX != value)
                {
                    _majorGridLinesX = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MinorGridLinesX
        {
            get { return _minorGridLinesX; }
            set
            {
                if (_minorGridLinesX != value)
                {
                    _minorGridLinesX = value;
                    OnPropertyChanged();
                }
            }
        }

        public string GridColorX
        {
            get { return _gridColorX; }
            set
            {
                if (_gridColorX != value)
                {
                    _gridColorX = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool DisplayGridLinesX
        {
            get { return _displayGridLinesX; }
            set
            {
                if (_displayGridLinesX != value)
                {
                    _displayGridLinesX = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MajorGridLinesY
        {
            get { return _majorGridLinesY; }
            set
            {
                if (_majorGridLinesY != value)
                {
                    _majorGridLinesY = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MinorGridLinesY
        {
            get { return _minorGridLinesY; }
            set
            {
                if (_minorGridLinesY != value)
                {
                    _minorGridLinesY = value;
                    OnPropertyChanged();
                }
            }
        }

        public string GridColorY
        {
            get { return _gridColorY; }
            set
            {
                if (_gridColorY != value)
                {
                    _gridColorY = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool DisplayGridLinesY
        {
            get { return _displayGridLinesY; }
            set
            {
                if (_displayGridLinesY != value)
                {
                    _displayGridLinesY = value;
                    OnPropertyChanged();
                }
            }
        }

        public ValueOptionType ValueOption
        {
            get { return _valueOption; }
            set
            {
                if (_valueOption != value)
                {
                    _valueOption = value;
                    OnPropertyChanged();
                }
            }
        }

        public double ActualMinimumValue
        {
            get { return _actualMinimumValue; }
            set
            {
                if (Math.Abs(_actualMinimumValue - value) > Double.Epsilon)
                {
                    _actualMinimumValue = value;
                    OnPropertyChanged();
                }
            }
        }

        public double ActualMaximumValue
        {
            get { return _actualMaximumValue; }
            set
            {
                if (Math.Abs(_actualMaximumValue - value) > Double.Epsilon)
                {
                    _actualMaximumValue = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ScaleAsPercentage
        {
            get { return _scaleAsPercentage; }
            set
            {
                if (_scaleAsPercentage != value)
                {
                    _scaleAsPercentage = value;
                    OnPropertyChanged();
                }
            }
        }

        public ScaleOption ScaleOption
        {
            get { return _scaleOption; }
            set
            {
                if (_scaleOption != value)
                {
                    _scaleOption = value;
                    OnPropertyChanged();
                }
            }
        }

        public IPen ScalePen { set; get; }
        public bool UpdateIsolated
        {
            get { return _updateIsolated; }
            set
            {
                _updateIsolated = value;
                if (value) OnPropertyChanged();
            }
        }

        public void Dispose()
        {

        }

        public IController ParentController { protected set; get; }
        public int Uid { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsVerified { get; }
        public bool IsDeleted { get; }
        public int ParentProgramUid { get; }
        public int ParentRoutineUid { get; }

        public void BeginTransactionSet()
        {
            throw new NotImplementedException();
        }

        public void EndTransactionSet()
        {
            throw new NotImplementedException();
        }

        public void CancelTransactionSet()
        {
            throw new NotImplementedException();
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        public int InstanceNumber { get; }
        public bool IsSafety { get; }
        public bool IsTypeLess { get; }

        public bool IsDescriptionDefaultLocale()
        {
            throw new NotImplementedException();
        }

        public Language[] GetDescriptionTranslations()
        {
            throw new NotImplementedException();
        }

        protected virtual void ParseConfig()
        {
            if (_config != null)
            {
                Name = (string) _config["Name"];
                Description = (string) _config["Description"];
                SamplePeriod = (int) _config["SamplePeriod"];
                NumberOfCaptures = (int) _config["NumberOfCaptures"];
                CaptureSizeType type;
                if (Enum.TryParse((string) _config["CaptureSizeType"], out type))
                {
                    CaptureSizeType = type;
                }

                CaptureSize = (int) _config["CaptureSize"];
                StartTriggerType = (int) _config["StartTriggerType"];
                StopTriggerType = (int) _config["StopTriggerType"];
                TrendxVersion = (float) _config["TrendxVersion"];

                if (_config["Isolated"] != null)
                {
                    Isolated = (bool) _config["Isolated"];
                }

                if (_config["TimeSpan"] != null)
                {
                    TimeSpan = TimeSpan.FromSeconds((double) _config["TimeSpan"]);
                }

                if (_config["Description"] != null)
                {
                    Description = _config["Description"]?.ToString();
                }

                if (_config["GraphTitle"] != null)
                {
                    GraphTitle = _config["GraphTitle"]?.ToString();
                }

                if (_config["ShowGraphTitle"] != null)
                {
                    ShowGraphTitle = (bool) _config["ShowGraphTitle"];
                }

                if (_config["DisplayYScale"] != null)
                {
                    DisplayYScale = (bool) _config["DisplayYScale"];
                }

                if (_config["YScaleDecimalPlaces"] != null)
                {
                    YScaleDecimalPlaces = (int) _config["YScaleDecimalPlaces"];
                }

                if (_config["DisplayXScale"] != null)
                {
                    DisplayXScale = (bool) _config["DisplayXScale"];
                }

                if (_config["DisplayDateOnScale"] != null)
                {
                    DisplayDateOnScale = (bool) _config["DisplayDateOnScale"];
                }

                if (_config["Background"] != null)
                {
                    Background = (string) _config["Background"];
                }

                if (_config["TextColor"] != null)
                {
                    TextColor = (string) _config["TextColor"];
                }

                if (_config["ExtraData"] != null)
                {
                    ExtraData = (int) _config["ExtraData"];
                }

                if (_config["DisplayMillisecond"] != null)
                {
                    DisplayMillisecond = (bool) _config["DisplayMillisecond"];
                }

                if (_config["DisplayPenValue"] != null)
                {
                    DisplayPenValue = (bool) _config["DisplayPenValue"];
                }

                if (_config["DisplayPenIcons"] != null)
                {
                    DisplayPenIcons = (bool) _config["DisplayPenIcons"];
                }

                if (_config["DisplayLineLegend"] != null)
                {
                    DisplayLineLegend = (bool) _config["DisplayLineLegend"];
                }

                if (_config["DisplayMinAndMaxValue"] != null)
                {
                    DisplayMinAndMaxValue = (bool) _config["DisplayMinAndMaxValue"];
                }

                if (_config["PenCaptionMaxLength"] != null)
                {
                    PenCaptionMaxLength = (int) _config["PenCaptionMaxLength"];
                }

                if (_config["DisplayScrollingMechanism"] != null)
                {
                    DisplayScrollingMechanism = (bool) _config["DisplayScrollingMechanism"];
                }

                if (_config["MajorGridLinesX"] != null)
                {
                    MajorGridLinesX = (int) _config["MajorGridLinesX"];
                }

                if (_config["MinorGridLinesX"] != null)
                {
                    MinorGridLinesX = (int) _config["MinorGridLinesX"];
                }

                if (_config["GridColorX"] != null)
                {
                    GridColorX = (string) _config["GridColorX"];
                }

                if (_config["DisplayGridLinesX"] != null)
                {
                    DisplayGridLinesX = (bool) _config["DisplayGridLinesX"];
                }

                if (_config["MajorGridLinesY"] != null)
                {
                    MajorGridLinesY = (int) _config["MajorGridLinesY"];
                }

                if (_config["MinorGridLinesY"] != null)
                {
                    MinorGridLinesY = (int) _config["MinorGridLinesY"];
                }

                if (_config["GridColorY"] != null)
                {
                    GridColorY = (string) _config["GridColorY"];
                }

                if (_config["DisplayGridLinesY"] != null)
                {
                    DisplayGridLinesY = (bool) _config["DisplayGridLinesY"];
                }

                if (_config["ValueOption"] != null)
                {
                    ValueOption = (ValueOptionType) (byte) _config["ValueOption"];
                }

                if (_config["ActualMinimumValue"] != null)
                {
                    ActualMinimumValue = (double) _config["ActualMinimumValue"];
                }

                if (_config["ActualMaximumValue"] != null)
                {
                    ActualMaximumValue = (double) _config["ActualMaximumValue"];
                }

                if (_config["ScaleAsPercentage"] != null)
                {
                    ScaleAsPercentage = (bool) _config["ScaleAsPercentage"];
                }

                if (_config["ScaleOption"] != null)
                {
                    ScaleOption = (Interfaces.Common.ScaleOption) (byte) _config["ScaleOption"];
                }
                
                if (_config["ChartStyle"]!=null)
                {
                    ChartStyle=(Interfaces.Common.ChartStyle)(byte)_config["ChartStyle"];
                }
                if (_config["AxisPenName"] != null)
                {
                    AxisPenName = (string)_config["AxisPenName"];
                }

                if (_config["Position"] != null)
                {
                    Position = (Position) (byte) _config["Position"];
                }

                if (_config["MaxViewable"] != null)
                {
                    MaxViewable = (int) _config["MaxViewable"];
                }
                var pens = _config["Pens"] as JArray;
                if (pens != null && pens.Count > 0)
                {
                    foreach (JObject penConfig in pens)
                    {
                        var pen = new Pen(penConfig);
                        if (ScaleOption == ScaleOption.UsingPen&&pen.Name.Equals((string)_config["ScalePen"]))
                        {
                            ScalePen = pen;
                        }
                        Add(pen);
                    }
                }
            }
        }
        
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public static Color GetColor(int index)
        {
            switch (index)
            {
                case 0:
                    return Colors.Blue;
                case 1:
                    return Colors.Red;
                case 2:
                    return Colors.Green;
                case 3:
                    return Colors.Yellow;
                case 4:
                    return Colors.BlueViolet;
                case 5:
                    return Colors.Aqua;
                case 6:
                    return Colors.Gray;
                case 7:
                    return Colors.Firebrick;
                case 8:
                    return Colors.Orchid;
            }

            return Colors.Chartreuse;
        }
    }

    public class Pen : PenObject
    {
        public Pen(JObject config) : base(config)
        {
        }

        public Pen() : base()
        {

        }
    }

    public class Trend : TrendObject
    {

        public Trend(JObject config, IController controller) : base(config, controller)
        {
        }

        public Trend(IController controller) : base(controller)
        {
        }
    }

    public class TrendCollection : ITrendCollection
    {
        public TrendCollection(IController controller)
        {
            ParentController = controller;
        }

        public void Add(ITrend trend)
        {
            if (!_trends.Contains(trend))
            {
                _trends.Add(trend);

                CollectionChanged?.Invoke(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, trend));
            }

        }

        public void Remove(ITrend trend)
        {
            if (_trends.Contains(trend))
            {
                _trends.Remove(trend);

                CollectionChanged?.Invoke(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, trend));
            }

        }

        public JArray ToJson()
        {
            var res = new JArray();
            foreach (var t in _trends)
            {
                res.Add(t.ToJson());
            }

            return res;
        }

        public void Clear()
        {
            _trends.Clear();

            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private readonly List<ITrend> _trends = new List<ITrend>();

        public IController ParentController { get; }

        public ITrend this[string name]
        {
            get
            {
                var trend = _trends.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                return trend;
            }
        }

        public IEnumerator<ITrend> GetEnumerator()
        {
            foreach (var trend in _trends)
            {
                yield return trend;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }

    public class TrendLog : TrendObject
    {
        public TrendLog(JObject config, string path) : base(config, null)
        {
            //ParentController = controller;
            FilePath = path;
            IsScrolling = true;
        }

        public string FilePath { private set; get; }
        public string ParentControllerName { private set; get; }
        public string StopTime { private set; get; }

        protected override void ParseConfig()
        {
            if (_config != null)
            {
                Name = (string) _config["TrendName"];
                ParentControllerName = (string) _config["ControllerName"];
                Description = (string) _config["Description"];
                SamplePeriod = (int) _config["SamplePeriod"];
                NumberOfCaptures = 1;
                //CaptureSizeType type;
                //if (Enum.TryParse((string)_config["CaptureSizeType"], out type))
                //{
                //    CaptureSizeType = type;
                //}

                //CaptureSize = (int)_config["CaptureSize"];
                //StartTriggerType = (int)_config["StartTriggerType"];
                //StopTriggerType = (int)_config["StopTriggerType"];
                //TrendxVersion = (float)_config["TrendxVersion"];

                if (_config["Description"] != null)
                {
                    Description = _config["Description"]?.ToString();
                }

                if (_config["GraphTitle"] != null)
                {
                    GraphTitle = _config["GraphTitle"]?.ToString();
                }

                if (_config["ShowGraphTitle"] != null)
                {
                    ShowGraphTitle = (bool) _config["ShowGraphTitle"];
                }

                if (_config["DisplayYScale"] != null)
                {
                    DisplayYScale = (bool) _config["DisplayYScale"];
                }

                if (_config["YScaleDecimalPlaces"] != null)
                {
                    YScaleDecimalPlaces = (int) _config["YScaleDecimalPlaces"];
                }

                if (_config["DisplayXScale"] != null)
                {
                    DisplayXScale = (bool) _config["DisplayXScale"];
                }

                if (_config["DisplayDateOnScale"] != null)
                {
                    DisplayDateOnScale = (bool) _config["DisplayDateOnScale"];
                }

                if (_config["Background"] != null)
                {
                    Background = (string) _config["Background"];
                }

                if (_config["TextColor"] != null)
                {
                    TextColor = (string) _config["TextColor"];
                }

                if (_config["ExtraData"] != null)
                {
                    ExtraData = (int) _config["ExtraData"];
                }

                if (_config["DisplayMillisecond"] != null)
                {
                    DisplayMillisecond = (bool) _config["DisplayMillisecond"];
                }

                if (_config["DisplayPenValue"] != null)
                {
                    DisplayPenValue = (bool) _config["DisplayPenValue"];
                }

                if (_config["DisplayPenIcons"] != null)
                {
                    DisplayPenIcons = (bool) _config["DisplayPenIcons"];
                }

                if (_config["DisplayLineLegend"] != null)
                {
                    DisplayLineLegend = (bool) _config["DisplayLineLegend"];
                }

                if (_config["DisplayMinAndMaxValue"] != null)
                {
                    DisplayMinAndMaxValue = (bool) _config["DisplayMinAndMaxValue"];
                }

                if (_config["PenCaptionMaxLength"] != null)
                {
                    PenCaptionMaxLength = (int) _config["PenCaptionMaxLength"];
                }

                if (_config["DisplayScrollingMechanism"] != null)
                {
                    DisplayScrollingMechanism = (bool) _config["DisplayScrollingMechanism"];
                }

                if (_config["MajorGridLinesX"] != null)
                {
                    MajorGridLinesX = (int) _config["MajorGridLinesX"];
                }

                if (_config["MinorGridLinesX"] != null)
                {
                    MinorGridLinesX = (int) _config["MinorGridLinesX"];
                }

                if (_config["GridColorX"] != null)
                {
                    GridColorX = (string) _config["GridColorX"];
                }

                if (_config["DisplayGridLinesX"] != null)
                {
                    DisplayGridLinesX = (bool) _config["DisplayGridLinesX"];
                }

                if (_config["MajorGridLinesY"] != null)
                {
                    MajorGridLinesY = (int) _config["MajorGridLinesY"];
                }

                if (_config["MinorGridLinesY"] != null)
                {
                    MinorGridLinesY = (int) _config["MinorGridLinesY"];
                }

                if (_config["GridColorY"] != null)
                {
                    GridColorY = (string) _config["GridColorY"];
                }

                if (_config["DisplayGridLinesY"] != null)
                {
                    DisplayGridLinesY = (bool) _config["DisplayGridLinesY"];
                }

                if (_config["ValueOption"] != null)
                {
                    ValueOption = (ValueOptionType) (byte) _config["ValueOption"];
                }

                if (_config["ActualMinimumValue"] != null)
                {
                    ActualMinimumValue = (double) _config["ActualMinimumValue"];
                }

                if (_config["ActualMaximumValue"] != null)
                {
                    ActualMaximumValue = (double) _config["ActualMaximumValue"];
                }

                if (_config["ScaleAsPercentage"] != null)
                {
                    ScaleAsPercentage = (bool) _config["ScaleAsPercentage"];
                }

                if (_config["ScaleOption"] != null)
                {
                    ScaleOption = (Interfaces.Common.ScaleOption) (byte) _config["ScaleOption"];
                }

                var date = ((string) _config["StartTime"]).Replace(",", " ").Replace(";", ".");
                RunTime = StartTime = DateTime.Parse(date);
                StopTime = (string) _config["StopTime"];
                var pens = _config["Pens"] as JArray;
                if (pens != null && pens.Count > 0)
                {
                    foreach (JValue name in pens)
                    {
                        var pen = new PenLog((string) name.Value);
                        pen.Color = $"16{GetColor(pens.IndexOf(name)).ToString()}";
                        Add(pen);
                    }
                }

                var data = _config["Data"] as JArray;
                if (data != null && data.Count > 0)
                {
                    foreach (JArray member in data)
                    {
                        var dataItem = new List<string>();
                        var time = ((JValue) member[0]).Value.ToString().Replace(";", ".");
                        dataItem.Add(time);

                        for (int i = 1; i <= pens.Count; i++)
                        {
                            dataItem.Add(((JValue) member[i]).Value.ToString());
                        }

                        Data.Add(dataItem);
                    }
                }

                CaptureSize = data?.Count ?? 0 + 1;
            }
        }

        public List<List<string>> Data { get; } = new List<List<string>>();
    }

    public class PenLog : PenObject
    {
        public PenLog(string name) : base()
        {
            Name = name;
        }

        //public List<Tuple<string,string>> Data { get; } = new List<Tuple<string, string>>();
    }
}
