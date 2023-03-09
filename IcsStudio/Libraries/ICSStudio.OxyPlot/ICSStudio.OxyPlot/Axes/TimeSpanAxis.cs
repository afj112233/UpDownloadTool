// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSpanAxis.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Represents an axis presenting <see cref="System.TimeSpan" /> values.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;

namespace OxyPlot.Axes
{
    using System;
    using System.Linq;

    /// <summary>
    /// Represents an axis presenting <see cref="System.TimeSpan" /> values.
    /// </summary>
    /// <remarks>The values should be in seconds.
    /// The StringFormat value can be used to force formatting of the axis values
    /// <code>"h:mm"</code> shows hours and minutes
    /// <code>"m:ss"</code> shows minutes and seconds</remarks>
    public class TimeSpanAxis : LinearAxis
    {
        private DateTime _startTime;

        public TimeSpanAxis()
        {
            StartTime=DateTime.Now;
            if (PlotModel?.Trend != null)
            {
                ActualMinimum = 0;
                ActualMaximum = PlotModel.Trend.TimeSpan.TotalSeconds;
            }
        }
        /// <summary>
        /// Converts a time span to a double.
        /// </summary>
        /// <param name="s">The time span.</param>
        /// <returns>A double value.</returns>
        public static double ToDouble(TimeSpan s)
        {
            return s.TotalSeconds;
        }

        public DateTime StartTime
        {
            set { _startTime = value; }
            get
            {
                if (PlotModel.Trend != null)
                {
                    if (PlotModel.Trend is TrendLog)
                    {
                        return PlotModel.Trend.StartTime;
                    }
                    if((PlotModel.Trend.IsStop || PlotModel.Trend.Pens.Count(p => p.Visible) == 0))
                    {
                        if (PlotModel.IsZoom||(PlotModel.Series.Any() && (PlotModel.Series[0] as TagSeries)?.Points.Count > 0))
                        {
                            return PlotModel.Trend.RunTime;
                        }

                        return DateTime.Now;
                    }
                    return PlotModel.Trend.RunTime;
                }
                return _startTime;
            }
        }

        public bool DisplayDate { set; get; } = false;

        public bool DisplayMillisecond { set; get; } = false;

        /// <summary>
        /// Converts a double to a time span.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A time span.</returns>
        public static TimeSpan ToTimeSpan(double value)
        {
            return TimeSpan.FromSeconds(value);
        }

        /// <summary>
        /// Gets the value from an axis coordinate, converts from double to the correct data type if necessary. e.g. DateTimeAxis returns the DateTime and CategoryAxis returns category strings.
        /// </summary>
        /// <param name="x">The coordinate.</param>
        /// <returns>The value.</returns>
        public override object GetValue(double x)
        {
            return FormatValueOverride(x);
        }

        /// <summary>
        /// Gets the default format string.
        /// </summary>
        /// <returns>
        /// The default format string.
        /// </returns>
        protected override string GetDefaultStringFormat()
        {
            return null;
        }

        /// <summary>
        /// Formats the value to be used on the axis.
        /// </summary>
        /// <param name="x">The value to format.</param>
        /// <returns>The formatted value.</returns>
        protected override string FormatValueOverride(double x)
        {
            var span = ToTimeSpan(x);
            var now = StartTime + span;
            //var fmt = this.ActualStringFormat ?? this.StringFormat ?? string.Empty;
            //fmt = fmt.Replace(":", "\\:");
            //fmt = string.Concat("{0:", fmt, "}");
            //just show timespan
            //return string.Format(this.ActualCulture, fmt, span);
            var str = DisplayMillisecond
                ? string.Format(this.ActualCulture, now.ToString("HH:mm:ss.fff"), span)
                : string.Format(this.ActualCulture, now.ToString("HH:mm:ss"), span);
            if (DisplayDate)
                str = $"{str}\n{now:yyyy/MM/dd}";
            return str;
        }

        public string FormatTime(DateTime time)
        {
            var str = DisplayMillisecond
                ? string.Format(this.ActualCulture, time.ToString("HH:mm:ss.fff"))
                : string.Format(this.ActualCulture, time.ToString("HH:mm:ss"));
            if (DisplayDate)
                str = $"{str}\n{time:yyyy/MM/dd}";
            return str;
        }
       
        /// <summary>
        /// Calculates the actual interval.
        /// </summary>
        /// <param name="availableSize">Size of the available area.</param>
        /// <param name="maxIntervalSize">Maximum length of the intervals.</param>
        /// <returns>The calculate actual interval.</returns>
        protected override double CalculateActualInterval(double availableSize, double maxIntervalSize)
        {
            double range = Math.Abs(this.ActualMinimum - this.ActualMaximum);
            double interval = 1;
            var goodIntervals = new[] {1.0, 5, 10, 30, 60, 120, 300, 600, 900, 1200, 1800, 3600};

            int maxNumberOfIntervals = Math.Max((int) (availableSize / maxIntervalSize), 2);

            while (true)
            {
                if (range / interval < maxNumberOfIntervals)
                {
                    return interval;
                }

                double nextInterval = goodIntervals.FirstOrDefault(i => i > interval);
                if (Math.Abs(nextInterval) < double.Epsilon)
                {
                    nextInterval = interval * 2;
                }

                interval = nextInterval;
            }
        }

        /// <summary>
        /// Gets the time from an axis coordinate
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public DateTime GetTime(double x)
        {
            var span = ToTimeSpan(x);
            return StartTime + span;
        }
        
        public override void GetTickValues(
            out IList<double> majorLabelValues, out IList<double> majorTickValues, out IList<double> minorTickValues)
        {
            if (!PlotModel.IsZoom && PlotModel.Trend != null&&PlotModel.Series.Any())
            {
                var series = PlotModel.Series[0] as TagSeries;
                if (series != null)
                {
                    if (!series.Points.Any() || DataMaximum < PlotModel.Trend.TimeSpan.TotalSeconds)
                    {
                        ActualMinimum = ActualMaximum - PlotModel.Trend.TimeSpan.TotalSeconds;
                    }
                    else
                    {
                        if (series.Points.Any() && Math.Abs(DataMaximum - DataMinimum) < PlotModel.Trend.TimeSpan.TotalSeconds)
                        {
                            ActualMaximum = ActualMinimum + PlotModel.Trend.TimeSpan.TotalSeconds;
                        }
                        else
                        {
                            ActualMinimum = ActualMaximum - PlotModel.Trend.TimeSpan.TotalSeconds;
                        }
                    }
                }
            }
            
            if (MajorGridLines != -1 && MinorGridLines != -1)
            {
                double majorStep = (ActualMaximum - ActualMinimum) / MajorGridLines;
                double minorStep = majorStep / (MinorGridLines + 1);
                minorTickValues = this.CreateTickValues(this.ActualMinimum, this.ActualMaximum, minorStep);
                majorTickValues = this.CreateTickValues(this.ActualMinimum, this.ActualMaximum, majorStep);
                majorLabelValues = majorTickValues;
            }
            else
            {
                minorTickValues = this.CreateTickValues(this.ActualMinimum, this.ActualMaximum, this.ActualMinorStep);
                majorTickValues = this.CreateTickValues(this.ActualMinimum, this.ActualMaximum, this.ActualMajorStep);
                majorLabelValues = majorTickValues;
            }
        }
    }
}
