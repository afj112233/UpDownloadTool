using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using ICSStudio.Gui.Annotations;
using OxyPlot;

namespace ICSStudio.Dialogs.ConfigDialogs
{
    [Serializable]
    public class AxisOptions : INotifyPropertyChanged, ISerializable, ICloneable
    {
        private bool _visible;
        private LineStyle _style;
        private MarkerType _marker;
        private int _width;
        private OxyColor _color;

        public AxisOptions()
        {
            WidthList = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            StyleList = new List<LineStyle> { LineStyle.Dot, LineStyle.Automatic, LineStyle.LongDashDot, LineStyle.LongDash, LineStyle.DashDot };
            MarkerList = new List<MarkerType>() { MarkerType.Circle, MarkerType.Square, MarkerType.Triangle, MarkerType.Diamond };
            VisibleList = new List<bool>() { true, false };
        }

        protected AxisOptions(SerializationInfo info, StreamingContext context)
        {
            SlaveValue = (string)info.GetValue(nameof(SlaveValue), typeof(string));
            Visible = (bool)info.GetValue(nameof(Visible), typeof(bool));
            Color = OxyColor.FromUInt32((uint)info.GetValue(nameof(Color), typeof(uint)));
            Width = (int)info.GetValue(nameof(Width), typeof(int));
            Style = (LineStyle)info.GetValue(nameof(Style), typeof(LineStyle));
            Marker = (MarkerType)info.GetValue(nameof(Marker), typeof(MarkerType));
        }

        public List<int> WidthList { get; set; }

        public List<LineStyle> StyleList { get; set; }

        public List<MarkerType> MarkerList { get; set; }

        public List<bool> VisibleList { get; set; }

        public string SlaveValue { get; set; }

        public OxyColor Color
        {
            get { return _color; }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged(nameof(Color));
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
                    OnPropertyChanged(nameof(Visible));
                }
            }
        }

        public int Width
        {
            get { return _width; }
            set
            {
                if (_width != value)
                {
                    _width = value;
                    OnPropertyChanged(nameof(Width));
                }
            }
        }

        public LineStyle Style
        {
            get { return _style; }
            set
            {
                if (_style != value)
                {
                    _style = value;
                    OnPropertyChanged(nameof(Style));
                }
            }
        }

        public MarkerType Marker
        {
            get { return _marker; }
            set
            {
                if (_marker != value)
                {
                    _marker = value;
                    OnPropertyChanged(nameof(Marker));
                }
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(SlaveValue), SlaveValue, typeof(string));
            info.AddValue(nameof(Color), Color.ToUint(), typeof(uint));
            info.AddValue(nameof(Visible), Visible, typeof(bool));
            info.AddValue(nameof(Width), Width, typeof(double));
            info.AddValue(nameof(Style), Style, typeof(LineStyle));
            info.AddValue(nameof(Marker), Marker, typeof(MarkerType));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public object Clone()
        {
            AxisOptions clone = new AxisOptions()
            {
                SlaveValue = SlaveValue,
                Color = Color,
                Visible = Visible,
                Width = Width,
                Style = Style,
                Marker = Marker,
            };

            return clone;
        }
    }
}
