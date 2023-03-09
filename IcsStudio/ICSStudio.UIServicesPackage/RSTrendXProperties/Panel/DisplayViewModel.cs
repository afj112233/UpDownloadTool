using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;

namespace ICSStudio.UIServicesPackage.RSTrendXProperties.Panel
{
    internal enum TimeFormat
    {
        [EnumMember(Value = "12-hour (AM/PM)")]
        Hour_12,
        [EnumMember(Value = "24-hour")] Hour_24,

        [EnumMember(Value = "Use system time setting")]
        UseSystemTimeSetting,
    }

    internal enum ChartRadix
    {
        Decimal,
        Hexadecimal,
        Octal
    }

    internal enum Connection
    {
        [EnumMember(Value = "Connect Points")] ConnectPoints,

        [EnumMember(Value = "Show Disconnects")]
        ShowDisconnects,

        [EnumMember(Value = "Show Only Markers")]
        ShowOnlyMarkers
    }

    internal enum PenCaption
    {
        [EnumMember(Value = "Long tag name")] LongTagName,
        [EnumMember(Value = "Short tag name")] ShortTagName,
        [EnumMember(Value = "Description")] Description
    }

    internal enum ScrollMode
    {
        [EnumMember(Value = "Continuous Scroll")]
        Continuous,

        [EnumMember(Value = "Half Screen Scroll")]
        Half,

        [EnumMember(Value = "Full Screen Scroll")]
        Full
    }

    class DisplayViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private readonly ITrend _trend;
        private bool _displayMillisecond;
        private bool _isDirty;
        private Color _background;
        private Color _textColor;
        private bool _displayLineLegend;
        private bool _displayPenValue;
        private bool _displayPenIcon;
        private int _captionMaxLength;
        private bool _displayMinMaxValue;
        private int _extraData;
        private bool _displayScrollingMechanism;
        private bool _allowScrolling;
        private Position _selectedPosition;
        private bool _maxViewEnable;
        private int _maxView;

        public DisplayViewModel(Display panel, ITrend trend)
        {
            panel.DataContext = this;
            Control = panel;
            _trend = trend;
            TimerFormatList = EnumHelper.ToDataSource<TimeFormat>();
            ChartRadixList = EnumHelper.ToDataSource<ChartRadix>();
            ConnectionList = EnumHelper.ToDataSource<Connection>();
            PositionList = EnumHelper.ToDataSource<Position>();
            PenCaptionList = EnumHelper.ToDataSource<PenCaption>();
            ScrollModeList = EnumHelper.ToDataSource<ScrollMode>();
            DisplayMillisecond = trend.DisplayMillisecond;
            Background = (Color) ColorConverter.ConvertFromString($"#{FormatOp.RemoveFormat(trend.Background)}");
            TextColor = (Color) ColorConverter.ConvertFromString($"#{FormatOp.RemoveFormat(trend.TextColor)}");
            SelectedPosition = trend.Position;
            MaxView = trend.MaxViewable;
            DisplayPenValue = _trend.DisplayPenValue;
            DisplayPenIcon = _trend.DisplayPenIcons;
            DisplayLineLegend = _trend.DisplayLineLegend;
            DisplayMinMaxValue = _trend.DisplayMinAndMaxValue;
            CaptionMaxLength = trend.PenCaptionMaxLength;
            AllowScrolling = trend.IsScrolling;
            DisplayScrollingMechanism = trend.DisplayScrollingMechanism;
            ExtraData = trend.ExtraData;
            IsDirty = false;
        }

        public int MaxView
        {
            set
            {
                _maxView = value;
                IsDirty = true;
            }
            get { return _maxView; }
        }

        public Position SelectedPosition
        {
            set
            {
                _selectedPosition = value;
                MaxViewEnable = value == Position.Bottom;
                IsDirty = true;
            }
            get { return _selectedPosition; }
        }

        public bool MaxViewEnable
        {
            set
            {
                Set(ref _maxViewEnable , value);
                IsDirty = true;
            }
            get { return _maxViewEnable; }
        }

        public int ExtraData
        {
            set
            {
                _extraData = value;
                IsDirty = true;
            }
            get { return _extraData; }
        }

        public bool DisplayScrollingMechanism
        {
            set
            {
                _displayScrollingMechanism = value;
                IsDirty = true;
            }
            get { return _displayScrollingMechanism; }
        }

        public bool AllowScrolling
        {
            set
            {
                _allowScrolling = value;
                IsDirty = true;
            }
            get { return _allowScrolling; }
        }

        public int CaptionMaxLength
        {
            set
            {
                _captionMaxLength = value;
                IsDirty = true;
            }
            get { return _captionMaxLength; }
        }

        public bool DisplayMinMaxValue
        {
            set
            {
                _displayMinMaxValue = value;
                IsDirty = true;
            }
            get { return _displayMinMaxValue; }
        }

        public bool DisplayPenIcon
        {
            set
            {
                _displayPenIcon = value;
                IsDirty = true;
            }
            get { return _displayPenIcon; }
        }

        public bool DisplayPenValue
        {
            set
            {
                _displayPenValue = value;
                IsDirty = true;
            }
            get { return _displayPenValue; }
        }

        public bool DisplayLineLegend
        {
            set
            {
                Set(ref _displayLineLegend , value);
                IsDirty = true;
            }
            get { return _displayLineLegend; }
        }

        public Color Background
        {
            set
            {
                _background = value;
                IsDirty = true;
            }
            get { return _background; }
        }

        public Color TextColor
        {
            set
            {
                _textColor = value;
                IsDirty = true;
            }
            get { return _textColor; }
        }

        public IList ConnectionList { get; }

        public IList ChartRadixList { get; }

        public IList PositionList { get; }

        public IList PenCaptionList { get; }

        public IList ScrollModeList { get; }

        public bool DisplayMillisecond
        {
            set
            {
                Set(ref _displayMillisecond, value);
                IsDirty = true;
            }
            get { return _displayMillisecond; }
        }

        public bool IsClosing { set; get; }

        public void Save()
        {
            if(IsClosing)return;
            if (!IsDirty) return;
            var trend = _trend as TrendObject;
            trend.DisplayMillisecond = DisplayMillisecond;
            trend.Background = $"16{Background.ToString()}";
            trend.TextColor = $"16{TextColor.ToString()}";
            trend.DisplayPenValue = DisplayPenValue;
            trend.DisplayPenIcons = DisplayPenIcon;
            trend.DisplayLineLegend = DisplayLineLegend;
            trend.DisplayMinAndMaxValue = DisplayMinMaxValue;
            trend.PenCaptionMaxLength = CaptionMaxLength;
            trend.IsScrolling = AllowScrolling;
            trend.DisplayScrollingMechanism = DisplayScrollingMechanism;
            trend.ExtraData = ExtraData;
            trend.Position = SelectedPosition;
            trend.MaxViewable = MaxView;
            IsDirty = false;
        }

        public IList TimerFormatList { get; }
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
