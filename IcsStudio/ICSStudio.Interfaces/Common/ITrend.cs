using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ICSStudio.Interfaces.Common
{
    public enum CaptureSizeType
    {
        Samples,
        TimePeriod,
        NoLimit
    }

    public enum TimeType
    {
        [EnumMember(Value = "Millisecond(s)")]
        Millisecond,
        [EnumMember(Value = "Second(s)")]
        Second,
        [EnumMember(Value = "Minute(s)")]
        Minute,
        [EnumMember(Value = "Hour(s)")]
        Hour
    }

    public enum ValueOptionType
    {
        Automatic,
        Preset,
        Custom
    }

    public enum ScaleOption
    {
        Same,
        Independent,
        UsingPen
    }

    public enum ChartStyle
    {
        Standard,
        XY,
    }

    public enum Position
    {
        [EnumMember(Value = "Left")]
        Left,
        [EnumMember(Value = "Bottom")]
        Bottom
    }

    public interface ITrend: IBaseComponent
    {
        void Add(IPen pen);
        void AddPens(IList<IPen> pens);
        JObject ToJson();
        int SamplePeriod { get; }
        int NumberOfCaptures { get; }
        CaptureSizeType CaptureSizeType { get; }
        int CaptureSize { get; }
        int StartTriggerType { get; }
        int StopTriggerType { get; }
        float TrendxVersion { get; }
        IEnumerable<IPen> Pens { get; }
        void ClearPens();
        void RemovePen(string penName);
        string GraphTitle { get; }
        bool ShowGraphTitle { get; }
        bool DisplayYScale { get; }
        int YScaleDecimalPlaces { get; }
        DateTime StartTime { get; }
        DateTime RunTime { get; }
        TimeSpan TimeSpan { get; }
        bool DisplayXScale { get; }
        bool DisplayDateOnScale { get; }
        bool IsScrolling { get; }
        string Background { get; }
        string TextColor { get; }
        int ExtraData { get; }
        bool DisplayMillisecond { get; }
        bool DisplayPenValue { get; }
        bool DisplayPenIcons { get; }
        bool DisplayLineLegend { get; }
        bool DisplayMinAndMaxValue { get; }
        int PenCaptionMaxLength { get; }
        bool DisplayScrollingMechanism { get; }
        bool Isolated { get; }
        int MajorGridLinesX { get; }
        int MinorGridLinesX { get; }
        string GridColorX { get; }
        bool DisplayGridLinesX { get; }
        int MajorGridLinesY { get; }
        int MinorGridLinesY { get; }
        string GridColorY { get; }
        bool DisplayGridLinesY { get; }
        ValueOptionType ValueOption { get; }
        double ActualMinimumValue { get; }
        double ActualMaximumValue { get; }
        bool ScaleAsPercentage { get; }
        ScaleOption ScaleOption { get; }
        bool UpdateIsolated { get; }
        bool IsStop { get; }
        ChartStyle ChartStyle { get; }

        string AxisPenName { get; }

        Position Position { get; }
        int MaxViewable { get; }
        bool IsUpdateScale { set; get; }
    }
}
