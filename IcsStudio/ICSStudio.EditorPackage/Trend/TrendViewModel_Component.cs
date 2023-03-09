using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Tags;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using Axis = OxyPlot.Axes.Axis;
using LinearAxis = OxyPlot.Axes.LinearAxis;
using Pen = ICSStudio.SimpleServices.Common.Pen;
using ScaleOption = ICSStudio.Interfaces.Common.ScaleOption;
using TimeSpanAxis = OxyPlot.Axes.TimeSpanAxis;

namespace ICSStudio.EditorPackage.Trend
{
    public partial class TrendViewModel
    {
        private readonly object _trendSyncRoot = new object();

        private PlotModel CreatePlot(bool showTitle = true, bool isolated = false)
        {
            var pm = new PlotModel(_trend, _trendSyncRoot)
                {Title = $"{_trend.GraphTitle}: " + _trend.StartTime.ToString("yyyy年MM月dd日")};
            pm.IsStruct = true;
            pm.TitlePadding = 10;
            pm.ShowExtremity = true;
            if (isolated)
            {
                pm.InIsolated = true;
                pm.ParentCollection = PlotModels;
            }
            
            //TODO(zyl): using pen
            var axesY = new OxyPlot.Axes.LinearAxis();
            //axesY.IsAxisVisible = _trend.DisplayYScale;
            axesY.Position = AxisPosition.Left;

            if (!isolated)
                if (_trend.ScaleOption == ScaleOption.Independent)
                {
                    axesY.IsAxisVisible = false;
                }

            WeakEventManager<PlotModel, EventArgs>.AddHandler(pm, "OnMouseWheel", OnMouseWheel);
            WeakEventManager<ElementCollection<Axis>, ElementCollectionChangedEventArgs<OxyPlot.Axes.Axis>>.AddHandler(
                pm.Axes, "CollectionChanged", Axes_CollectionChanged);
            pm.PlotAreaBackground = OxyColor.Parse($"#{FormatOp.RemoveFormat(_trend.Background)}");
            pm.TextColor = OxyColor.Parse($"#{FormatOp.RemoveFormat(_trend.TextColor)}");
            pm.DisplayPenIcon = _trend.DisplayPenIcons;
            var axesX = new TimeSpanAxis();
            axesX.Key = "x";
            axesX.DisplayDate = _trend.DisplayDateOnScale;
            axesX.DisplayMillisecond = _trend.DisplayMillisecond;
            axesX.Position = AxisPosition.Bottom;
            axesX.MajorGridLines = _trend.MajorGridLinesX+1;
            axesX.MinorGridLines = _trend.MinorGridLinesX;
            if (!isolated)
            {
                axesY.MajorGridLines = _trend.MajorGridLinesY+1;
                axesY.MinorGridLines = _trend.MinorGridLinesY;
            }
            pm.DisplayX = _trend.DisplayXScale;
            pm.DisplayY = _trend.DisplayYScale;
            pm.Axes.Clear();
            pm.Axes.Add(axesX);
            pm.Axes.Add(axesY);
            pm.ShowTitle = showTitle && _trend.ShowGraphTitle;
            pm.Padding=new OxyThickness(15,0,25,0);
            return pm;
        }

        private void SetPlotModel(PlotModel plotModel, PropertyChangedEventArgs e)
        {
            if (plotModel.InIsolated)
            {
                plotModel.DisplayX = false;
            }

            if (PlotModels.Contains(plotModel))
            {
                var visibleCollection = PlotModels.Where(p => p.Visibility == Visibility.Visible).ToList();
                var index = visibleCollection.IndexOf(plotModel);
                plotModel.ShowTitle = index == 0 && _trend.ShowGraphTitle;
            }   
            else
            {
                if (e.PropertyName == "ScaleOption")
                {
                    plotModel.ShowTitle = _trend.ShowGraphTitle;
                    if (_trend.ScaleOption == Interfaces.Common.ScaleOption.Independent)
                    {
                        plotModel.Axes.FirstOrDefault(a => !a.IsHorizontal()).IsAxisVisible = false;
                        if (plotModel.Series.Count > 0)
                        {
                            ((TagSeries) plotModel.Series[0]).LeftAxes.IsAxisVisible = true;
                        }

                        foreach (var model in PlotModels)
                        {
                            for (int i = model.Series.Count-1; i > 0; i--)
                            {
                                model.Series.RemoveAt(i);
                            }
                        }
                    }
                    else if (_trend.ScaleOption == ScaleOption.UsingPen)
                    {

                    }
                    else
                    {
                        var axesCollection = plotModel.Axes.Where(a => !a.IsHorizontal());
                        foreach (var axis in axesCollection)
                        {
                            var defaultAxis = PlotModel.Axes.FirstOrDefault(a => !a.IsHorizontal());
                            if (defaultAxis == axis)
                                defaultAxis.IsAxisVisible = true;
                            else
                                axis.IsAxisVisible = false;
                        }

                        {
                            foreach (var model in PlotModels)
                            {
                                var series = model.Series[0];
                                foreach (var plotModel1 in PlotModels)
                                {
                                    if(plotModel1==model)continue;
                                    plotModel1.Series.Add(((TagSeries)series).GetSyncSeries());
                                }
                            }
                        }
                        //                    plotModel.DefaultYAxis.IsAxisVisible = true;
                    }
                }
            }

            plotModel.Title = $"{_trend.GraphTitle}: " + DateTime.Now.ToString("yyyy年MM月dd日");
            var axesX = (TimeSpanAxis) plotModel.Axes.FirstOrDefault(a => a is TimeSpanAxis);
            var axesY =  plotModel.Axes.Where(a => a != axesX);
            Debug.Assert(axesX != null && axesY.Any());
            if (plotModel.InIsolated)
            {
                if (plotModel.Visibility == Visibility.Visible)
                {
                    var visibleCollection = PlotModels.Where(p => p.Visibility == Visibility.Visible).ToList();
                    if (visibleCollection.IndexOf(plotModel) == visibleCollection.Count - 1)
                    {
                        plotModel.DisplayX = _trend.DisplayXScale;
                    }
                }
            }
            else
            {
                plotModel.DisplayX = _trend.DisplayXScale;
            }

            plotModel.DisplayY = _trend.DisplayYScale;

            plotModel.DisplayPenIcon = _trend.DisplayPenIcons;
            
            axesX.DisplayDate = _trend.DisplayDateOnScale;
            axesX.DisplayMillisecond = _trend.DisplayMillisecond;
            SamplePeriod = $"Periodic {_trend.SamplePeriod} ms";
            plotModel.PlotAreaBackground = OxyColor.Parse($"#{FormatOp.RemoveFormat(_trend.Background)}");
            plotModel.TextColor = OxyColor.Parse($"#{FormatOp.RemoveFormat(_trend.TextColor)}");
            var color = (Color) ColorConverter.ConvertFromString($"#{FormatOp.RemoveFormat(_trend.TextColor)}");
            TextColorBrush = new SolidColorBrush(color);
            if (e.PropertyName == "TimeSpan")
            {
                if (IsRun)
                {
                    return;
                }
                UpdateSeries();
            }
            if (e.PropertyName == "YScaleDecimalPlaces")
            {
                foreach (var axis in axesY)
                {
                    axis.StringFormat = $"N{_trend.YScaleDecimalPlaces}";
                }
            }

            foreach (var ax in plotModel.Axes)
            {
                SetAxesGrid(ax, plotModel);
            }

            foreach (TagSeries tagSeries in plotModel.Series)
            {
                tagSeries.MaxMinData.Update();
                tagSeries.MaxMinData.Background = new SolidColorBrush(OxyColor.Parse($"#{FormatOp.RemoveFormat(_trend.Background)}").ToColor());
            }

            if (!_trend.IsScrolling)
            {
                if (_trend.Isolated)
                {
                    foreach (var model in PlotModels)
                    {
                        model.IsLoad = true;
                        model.PlotView?.InvalidatePlot();
                        model.IsLoad = false;
                    }
                }
                else
                {
                    plotModel.IsLoad = true;
                    plotModel.PlotView?.InvalidatePlot();
                    plotModel.IsLoad = false;
                }
            }
            else
            {
                if (_canExecuteRunCommand)
                {
                    if (_trend.Isolated)
                    {
                        foreach (var model in PlotModels)
                        {
                            model.IsLoad = true;
                            model.PlotView?.InvalidatePlot();
                            model.IsLoad = false;
                        }
                    }
                    else
                    {
                        plotModel.IsLoad = true;
                        plotModel.PlotView?.InvalidatePlot();
                        plotModel.IsLoad = false;
                    }
                }
            }
        }

        private void ResetKeepMaxMin()
        {
            foreach (var ax in PlotModel.Axes)
            {
                ax.ResetKeepMaxMin();
            }

            foreach (var plotModel in PlotModels)
            {
                foreach (var ax in plotModel.Axes)
                {
                    ax.ResetKeepMaxMin();
                }
            }
        }

        private void CreateUnIsolated()
        {
            PlotModel = CreatePlot(true);
            CreateSeries();
        }

        private void CreateIsolated()
        {
            int index = 0;
            List<TagSeries> tagSeriesList = new List<TagSeries>();
            foreach (var pen in _trend.Pens)
            {
                var plotModel = CreatePlot(index == 0, true);
                plotModel.Name = pen.Name;
                if (index != _trend.Pens.Count() - 1)
                {
                    plotModel.DisplayX = false;
                }

                plotModel.Padding = new OxyThickness(15, 0, 25, 0);
                var name = pen.Name;
                Tag tag = null;
                if (pen is Pen)
                {
                    tag = GetTag(ref name);
                }

                var color = (Color) ColorConverter.ConvertFromString($"#{FormatOp.RemoveFormat(pen.Color, false)}");
                color.A = 0xff;
                var maxMinData = new MaxMinData(Colors.Black, color, pen.Width, pen.Name,
                    _trend.DisplayMinAndMaxValue ? Visibility.Visible : Visibility.Collapsed,
                    _trend.PenCaptionMaxLength,pen.Units, pen.Visible, true,IsReal(pen.Name, tag?.ParentCollection.ParentProgram));
                var series = new TagSeries(name, tag, _trend, maxMinData, pen,
                    plotModel.SyncRoot);
                if (index == 0)
                {
                    series.MaxMinData.HeaderVisibility = Visibility.Visible;
                }
                series.Color = ConvertOxyColor(color);
                series.LineStyle = TagSeries.ChooseLine(pen.Style);
                series.StrokeThickness = pen.Width;
                series.IsVisible = pen.Visible;
                series.UpdateEventHandler = SeriesUpdateHandle;
                plotModel.Series.Add(series);
                tagSeriesList.Add(series);
                if (!pen.Visible && plotModel.InIsolated)
                    plotModel.Visibility = Visibility.Collapsed;
                plotModel.Axes.FirstOrDefault(a => !a.IsHorizontal()).IsAxisVisible = false;
                series.LeftAxes.IsAxisVisible = true;
                plotModel.Axes.Add(series.LeftAxes);
                PlotModels.Add(plotModel);
                index++;
            }

            var trend = _trend as SimpleServices.Common.Trend;
            if (trend != null)
            {
                if (_trendDataCenter == null)
                {
                    _trendDataCenter = new TrendServer(trend);
                }

                if (_dataCenter == null)
                {
                    _dataCenter = new DataCenter(_trend);
                }

                _dataCenter?.AddPlotModel(PlotModels);

                _trendDataCenter.AddIsolateTagSeries(tagSeriesList);
            }

            if (_trend is TrendLog)
            {
                if (_dataCenter == null)
                    _dataCenter = new DataCenter(_trend);
                _dataCenter?.AddPlotModel(PlotModels);
            }
        }
        
        private bool IsReal(string specifier, IProgramModule program)
        {
            try
            {
                if (_trend is TrendLog) return true;
                var dataTypeInfo = ObtainValue.GetTargetDataTypeInfo(specifier, program, null);
                return dataTypeInfo.DataType?.IsReal ?? true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void SetIsolated()
        {
            if (_trend.Pens.Any(p => p.Visible))
                DisplayIsolatedTrendVisibility = _trend.Isolated ? Visibility.Visible : Visibility.Collapsed;
            else
                DisplayIsolatedTrendVisibility = Visibility.Collapsed;
            if (PlotModels.Count > 0 && _trend.Isolated)
            {
                ResetIsolatedTrend();
            }

            SetLiveDataCollection();
        }

        private void SetLiveDataCollection()
        {
            MinMaxDataCollection.Clear();

            if (_trend.Isolated)
            {
                foreach (var plotModel in PlotModels)
                {
                    var series = plotModel.Series[0] as TagSeries;
                    if (series != null)
                    {
                        MinMaxDataCollection.Add(series.MaxMinData);
                    }
                }
            }
            else
            {
                foreach (TagSeries series in PlotModel.Series)
                {
                    MinMaxDataCollection.Add(series.MaxMinData);
                }

            }
        }

        private void SetAxesGrid(OxyPlot.Axes.Axis item, PlotModel plotModel)
        {
            item.TickStyle = TickStyle.None;
            if (item.IsHorizontal())
            {
                if (_trend.DisplayGridLinesX)
                {
                    item.MajorGridlineStyle = LineStyle.Automatic;
                    var color = (Color)ColorConverter.ConvertFromString(
                        $"#{FormatOp.RemoveFormat(_trend.GridColorX)}");
                    item.MajorGridlineColor = ConvertOxyColor(color);
                    item.MinorGridlineStyle = LineStyle.Dot;
                    item.MinorGridlineColor = ConvertOxyColor(color);
                    if (_trend.MajorGridLinesX == 0)
                    {
                        item.MajorGridlineStyle = LineStyle.None;
                        item.MajorGridLines = 1;
                    }
                    else
                    {
                        item.MajorGridLines = _trend.MajorGridLinesX+1;
                    }

                    item.MinorGridLines = _trend.MinorGridLinesX;
                }
                else
                {
                    item.MajorGridlineStyle = LineStyle.None;
                    item.MinorGridlineStyle = LineStyle.None;
                }
            }
            else
            {
                item.StringFormat = $"N{_trend.YScaleDecimalPlaces}";
                if (_trend.DisplayGridLinesY && !plotModel.InIsolated)
                {
                    item.MajorGridlineStyle = LineStyle.Automatic;
                    var color = (Color)ColorConverter.ConvertFromString(
                        $"#{FormatOp.RemoveFormat(_trend.GridColorY)}");
                    item.MajorGridlineColor = ConvertOxyColor(color);
                    item.MinorGridlineStyle = LineStyle.Dot;
                    item.MinorGridlineColor = ConvertOxyColor(color);
                    if (_trend.MajorGridLinesY == 0)
                    {
                        item.MajorGridlineStyle = LineStyle.None;
                        item.MajorGridLines = 1;
                    }
                    else
                    {
                        item.MajorGridLines = _trend.MajorGridLinesY+1;
                    }

                    item.MinorGridLines = _trend.MinorGridLinesY;
                }
                else
                {
                    item.MajorGridlineStyle = LineStyle.None;
                    item.MinorGridlineStyle = LineStyle.None;
                }

                if (plotModel.InIsolated)
                {
                    item.MajorGridLines = 2;
                    item.MinorGridLines = 0;
                }

                if (_trend.ValueOption == ValueOptionType.Automatic)
                {
                    item.Maximum = double.NaN;
                    item.Minimum = double.NaN;
                }

                if (_trend.ValueOption == ValueOptionType.Preset)
                {
                    if (_trend.ScaleOption == ScaleOption.Same)
                    {
                        double max = 1, min = 0;
                        foreach (var pen in _trend.Pens)
                        {
                            max = Math.Max(pen.Max, max);
                            min = Math.Min(pen.Min, min);
                        }

                        item.Maximum = max;
                        item.Minimum = min;
                    }
                    else
                    {
                        if (plotModel.InIsolated)
                        {
                            if (plotModel.Series.Count > 0)
                            {
                                item.Maximum = ((TagSeries)plotModel.Series[0]).Pen.Max;
                                item.Minimum = ((TagSeries)plotModel.Series[0]).Pen.Min;
                            }
                        }
                        else
                        {
                            var series =
                                plotModel.Series.FirstOrDefault(s => (s as TagSeries)?.LeftAxes == item) as TagSeries;
                            if (series != null)
                            {
                                item.Maximum = (series).Pen.Max;
                                item.Minimum = (series).Pen.Min;
                            }
                        }
                    }
                }

                if (_trend.ValueOption == ValueOptionType.Custom)
                {
                    item.Maximum = _trend.ActualMaximumValue;
                    item.Minimum = _trend.ActualMinimumValue;
                }
            }
        }

        private void CreateSeries()
        {
            List<TagSeries> tagSeriesList = new List<TagSeries>();
            int index = 0;
            foreach (var pen in _trend.Pens)
            {
                var name = pen.Name;
                Tag tag = null;
                if (!(_trend is TrendLog) && pen is Pen)
                {
                    tag = GetTag(ref name);
                }

                var color = (Color) ColorConverter.ConvertFromString($"#{FormatOp.RemoveFormat(pen.Color, false)}");
                color.A = 0xff;
                //if (_trend.Background == "White"&& color == Colors.White)
                    //color = Colors.Black;
                var maxMinData = new MaxMinData(Colors.Black, color, pen.Width, pen.Name,
                    _trend.DisplayMinAndMaxValue ? Visibility.Visible : Visibility.Collapsed,
                    _trend.PenCaptionMaxLength,pen.Units, pen.Visible, false ,IsReal(pen.Name, tag?.ParentCollection.ParentProgram));
              
                var series = new TagSeries(name, tag, _trend, maxMinData, pen,
                    PlotModel.SyncRoot);
                if (index == 0&&pen.Visible)
                {
                    series.LeftAxes.IsAxisVisible = true;
                    series.MaxMinData.HeaderVisibility = Visibility.Visible;
                    series.MaxMinData.SelectedStateVisibility = Visibility.Visible;
                    PlotModel.Axes.FirstOrDefault(a => !a.IsHorizontal()).IsAxisVisible = false;
                }
                PlotModel.Axes.Add(series.LeftAxes);
                series.Color = ConvertOxyColor(color);
                series.LineStyle = TagSeries.ChooseLine(pen.Style);
                series.StrokeThickness = pen.Width;
                series.IsVisible = pen.Visible;
                series.UpdateEventHandler = SeriesUpdateHandle;
                PlotModel.Series.Add(series);
                tagSeriesList.Add(series);
                if (pen.Visible)
                    index++;
            }

            var trend = _trend as SimpleServices.Common.Trend;
            if (trend != null)
            {
                if (_trendDataCenter == null)
                {
                    _trendDataCenter = new TrendServer(trend);
                }

                if (_dataCenter == null)
                {
                    _dataCenter = new DataCenter(_trend);
                }

                _dataCenter?.AddPlotModel(PlotModel);
                _trendDataCenter.AddTagSeries(tagSeriesList);
                _trendDataCenter.ResetDataCenter(_dataCenter);
            }
            else
            {
                if (_trend is TrendLog)
                {
                    if (_dataCenter == null)
                    {
                        _dataCenter = new DataCenter(_trend);
                    }
                    _dataCenter?.AddPlotModel(PlotModel);
                    _dataCenter.SetLogData(((TrendLog) _trend).Data);
                }
            }
        }

        private DataCenter _dataCenter;

        private void SetTooltips()
        {
            double time = 0;
            var unit = "";
            if (!_trend.TimeSpan.TotalDays.ToString().Contains("."))
            {
                time = _trend.TimeSpan.TotalDays;
                unit = "Day(s)";
            }
            else if (!_trend.TimeSpan.TotalHours.ToString().Contains("."))
            {
                time = _trend.TimeSpan.TotalHours;
                unit = "Hour(s)";
            }
            else if (!_trend.TimeSpan.TotalMinutes.ToString().Contains("."))
            {
                time = _trend.TimeSpan.TotalMinutes;
                unit = "Minute(s)";
            }
            else
            {
                time = _trend.TimeSpan.TotalSeconds;
                unit = "Second(s)";
            }

            BackwardTooltip1 = $"Backward {time} {unit}";
            BackwardTooltip2 = $"Backward {time / 2} {unit}";
            ForwardTooltip1 = $"Forward {time / 2} {unit}";
            ForwardTooltip2 = $"Forward {time} {unit}";
        }

        private string _lastSelectedSpecial = "";
        private void _trend_CollectionChanged(object sender,
            NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var coll=new List<IPen>();
                foreach (IPen pen in e.NewItems)
                {
                    coll.Add(pen);
                }
                foreach (IPen pen in coll)
                {
                    var name = pen.Name;
                    Tag tag = null;
                    if (pen is Pen)
                    {
                        tag = GetTag(ref name);
                    }

                    {
                        var color = (Color) ColorConverter.ConvertFromString(
                            $"#{FormatOp.RemoveFormat(pen.Color, false)}");
                        color.A = 0xff;
                        var maxMinData = new MaxMinData(Colors.Black, color, pen.Width, pen.Name,
                            _trend.DisplayMinAndMaxValue ? Visibility.Visible : Visibility.Collapsed,
                            _trend.PenCaptionMaxLength,pen.Units, pen.Visible, false, IsReal(pen.Name, tag?.ParentCollection.ParentProgram));
                        var series = new TagSeries(name, tag, _trend, maxMinData, pen, PlotModel.SyncRoot);
                        maxMinData.IsVisible = pen.Visible;
                        series.Color = ConvertOxyColor(color);
                        series.LineStyle = TagSeries.ChooseLine(pen.Style);
                        series.StrokeThickness = pen.Width;
                        series.IsVisible = pen.Visible;
                        series.UpdateEventHandler = SeriesUpdateHandle;
                        PlotModel.Series.Add(series);
                        _trendDataCenter.AddTagSeries(series);
                        PlotModel.Axes.Add(series.LeftAxes);
                        if (!_trend.Isolated)
                        {
                            MinMaxDataCollection.Add(maxMinData);
                        }
                    }

                    {
                        var plotModel = CreatePlot(false, true);
                        plotModel.Axes.FirstOrDefault(a => !a.IsHorizontal()).IsAxisVisible = false;
                        plotModel.Name = pen.Name;
                        plotModel.DisplayX = false;
                        plotModel.Padding = new OxyThickness(15, 0, 25, 0);
                        var color = (Color) ColorConverter.ConvertFromString(
                            $"#{FormatOp.RemoveFormat(pen.Color, false)}");
                        color.A = 0xff;
                        var maxMinData = new MaxMinData(Colors.Black, color, pen.Width, pen.Name,
                            _trend.DisplayMinAndMaxValue ? Visibility.Visible : Visibility.Collapsed,
                            _trend.PenCaptionMaxLength,pen.Units, pen.Visible, true, IsReal(pen.Name, tag?.ParentCollection.ParentProgram));
                     
                        var series = new TagSeries(name, tag, _trend, maxMinData, pen, plotModel.SyncRoot);
                        maxMinData.IsVisible = pen.Visible;
                        series.Color = ConvertOxyColor(color);
                        series.LineStyle = TagSeries.ChooseLine(pen.Style);
                        series.StrokeThickness = pen.Width;
                        series.IsVisible = pen.Visible;
                        series.UpdateEventHandler = SeriesUpdateHandle;
                        series.LeftAxes.IsAxisVisible = true;
                        plotModel.Series.Add(series);
                        plotModel.Axes.Add(series.LeftAxes);
                        _trendDataCenter.AddIsolateTagSeries(series);
                        if (!pen.Visible && plotModel.InIsolated)
                            plotModel.Visibility = Visibility.Collapsed;
                        PlotModels.Add(plotModel);
                        if (_trend.Isolated)
                        {
                            MinMaxDataCollection.Add(maxMinData);
                        }

                        WeakEventManager<PlotModel, EventArgs>.AddHandler(plotModel, "Updated", PlotModel_Updated);
                    }

                }

                StopTimer();
                IsRun = false;
                ScheduleCommand4Checked = true;
                RaisePropertyChanged("ScrollChecked");
                _canExecuteRunCommand = true;
                RunCommand.RaiseCanExecuteChanged();
                StopCommand.RaiseCanExecuteChanged();
                SyncIsolatedAndUnIsolated();
                if (_trend.Isolated)
                {
                    if (string.IsNullOrEmpty(_lastSelectedSpecial))
                    {
                        if (PlotModels.Count > 0)
                        {
                            var series = (TagSeries)PlotModels[0].Series[0];
                            series.MaxMinData.SelectedStateVisibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        foreach (var plotModel in PlotModels)
                        {
                            var series = (TagSeries)plotModel.Series[0];
                            if (series.Name.Equals(_lastSelectedSpecial))
                            {
                                series.MaxMinData.SelectedStateVisibility = Visibility.Visible;
                                _lastSelectedSpecial = series.Name;
                            }
                            else
                            {
                                series.MaxMinData.SelectedStateVisibility = Visibility.Collapsed;
                            }
                        }
                    }
                    
                }
                else
                {
                    if (string.IsNullOrEmpty(_lastSelectedSpecial))
                    {
                        if (Trend.Pens.Any())
                        {
                            var series = (TagSeries)PlotModel.Series[0];
                            series.LeftAxes.IsAxisVisible = true;
                        }
                        else
                        {
                            var axis = PlotModel.Axes.FirstOrDefault(a => !a.IsHorizontal());
                            if (axis != null)
                                axis.IsAxisVisible = true;
                        }
                    }
                    else
                    {
                        if (Trend.Pens.Any())
                        {
                            bool isSet = false;
                            foreach (TagSeries series in PlotModel.Series)
                            {
                                //var series = PlotModel.Series.FirstOrDefault(s => (s as TagSeries)?.MaxMinData == data);
                                if (series.Name.Equals(_lastSelectedSpecial))
                                {
                                    series.MaxMinData.SelectedStateVisibility = Visibility.Visible;
                                    foreach (var ax in PlotModel.Axes)
                                    {
                                        if (ax.IsHorizontal()) continue;
                                        if (series.LeftAxes == ax)
                                        {
                                            isSet = true;
                                            ax.IsAxisVisible = true;
                                            _lastSelectedSpecial = series.Name;
                                        }
                                        else
                                        {
                                            ax.IsAxisVisible = false;
                                        }
                                    }
                                }
                                else
                                {
                                    series.MaxMinData.SelectedStateVisibility = Visibility.Collapsed;
                                }
                            }

                            if (!isSet)
                            {
                                var series = (TagSeries)PlotModel.Series[0];
                                series.LeftAxes.IsAxisVisible = true;
                            }
                        }
                        else
                        {
                            var axis = PlotModel.Axes.FirstOrDefault(a => !a.IsHorizontal());
                            if (axis != null)
                                axis.IsAxisVisible = true;
                        }
                        
                    }
                       
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    {
                        var series = (TagSeries) PlotModel.Series.FirstOrDefault(s => (s as TagSeries)?.Pen == item);
                        if (series != null)
                        {
                            PlotModel.Series.Remove(series);
                            PlotModel.Axes.Remove(series.LeftAxes);
                            MinMaxDataCollection.Remove(series.MaxMinData);
                            _trendDataCenter.DeleteTagSeries(series);
                        }
                    }

                    var plotModel = PlotModels.FirstOrDefault(p => (p.Series[0] as TagSeries)?.Pen == item);
                    if (plotModel != null)
                    {
                        PlotModels.Remove(plotModel);
                        var series = plotModel.Series[0] as TagSeries;
                        if (series != null)
                        {
                            MinMaxDataCollection.Remove(series.MaxMinData);
                            _trendDataCenter.DeleteIsolateTagSeries(series);
                        }

                        WeakEventManager<PlotModel, EventArgs>.RemoveHandler(plotModel, "Updated", PlotModel_Updated);
                    }
                }
            }

            if (_trend.Pens.Any(p => p.Visible))
            {
                DisplayIsolatedTrendVisibility = _trend.Isolated ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                DisplayIsolatedTrendVisibility = Visibility.Collapsed;
            }

            Update(false);
            SetBottomHeaderVisibility();
        }
        private void SetBottomHeaderVisibility()
        {
            var plotMode = PlotModel;
            if (plotMode == null) return;
           
            {
                int index = 0;
                foreach (var plotModel in PlotModels)
                {
                    if (index == 0 && plotMode.Visibility == Visibility.Visible)
                    {
                        ((TagSeries)plotModel.Series[0]).MaxMinData.HeaderVisibility = Visibility.Visible;
                    }
                    else
                    {
                        ((TagSeries)plotModel.Series[0]).MaxMinData.HeaderVisibility = Visibility.Collapsed;
                    }

                    index++;
                }
            }
            
            {
                int index = 0;
                foreach (TagSeries series in PlotModel.Series)
                {
                    if (index == 0 && series.IsVisible )
                    {
                        series.MaxMinData.HeaderVisibility = Visibility.Visible;
                    }
                    else
                    {
                        series.MaxMinData.HeaderVisibility = Visibility.Collapsed;
                    }

                    index++;
                }
            }
        }
        
        private void TurnToStandard()
        {
            {
                PlotModel.Title = $"{_trend.GraphTitle}: " + _trend.StartTime.ToString("yyyy年MM月dd日");
                var penAxis =
                    PlotModel.Axes.FirstOrDefault(a => a.Position == AxisPosition.Bottom && !(a is TimeSpanAxis));
                if (penAxis != null)
                {
                    PlotModel.Axes.Remove(penAxis);
                }
                foreach (TagSeries series in PlotModel.Series)
                {
                    series.MaxMinData.CrossVisibility = Visibility.Collapsed;
                }
                var defaultAxis = PlotModel.Axes.FirstOrDefault(a => a.Position == AxisPosition.Bottom);
                if (defaultAxis != null)
                {
                    defaultAxis.IsAxisVisible = true;
                }
            }
            {
                foreach (var plotModel in PlotModels)
                {
                    plotModel.Title = $"{_trend.GraphTitle}: " + _trend.StartTime.ToString("yyyy年MM月dd日");
                    var penAxis =
                        plotModel.Axes.FirstOrDefault(a => a.Position == AxisPosition.Bottom && !(a is TimeSpanAxis));
                    if (penAxis != null)
                    {
                        plotModel.Axes.Remove(penAxis);
                    }
                    foreach (TagSeries series in plotModel.Series)
                    {
                        series.MaxMinData.CrossVisibility = Visibility.Collapsed;
                    }
                    var defaultAxis = plotModel.Axes.FirstOrDefault(a => a.Position == AxisPosition.Bottom);
                    if (defaultAxis != null)
                    {
                        defaultAxis.IsAxisVisible = true;
                    }
                }
            }
            _dataCenter.TurnToStandard();
        }

        private void TurnToPenAxis()
        {
            {
                SetPenAxis(PlotModel);
            }
            {
                foreach (var plotModel in PlotModels)
                {
                   SetPenAxis(plotModel);
                }
            }
            _dataCenter.TurnToPenAxis();
        }

        private void SetPenAxis(PlotModel plotModel)
        {
            var defaultAxis = plotModel.Axes.FirstOrDefault(a => a.Position == AxisPosition.Bottom);
            if (defaultAxis != null)
            {
                defaultAxis.IsAxisVisible = false;
            }
            foreach (TagSeries series in plotModel.Series)
            {
                if (series.Name == _trend.AxisPenName)
                {
                    series.MaxMinData.CrossVisibility = Visibility.Visible;
                }
                else
                {
                    series.MaxMinData.CrossVisibility = Visibility.Collapsed;
                }
            }
            var penAxis = new LinearAxis();
            penAxis.StringFormat = $"N{_trend.YScaleDecimalPlaces}";
            penAxis.Position = AxisPosition.Bottom;
            penAxis.MajorGridLines = _trend.MajorGridLinesX+1;
            penAxis.MinorGridLines = _trend.MinorGridLinesX;
            penAxis.IsAxisVisible = true;
            plotModel.Axes.Add(penAxis);
        }
        
        private void _trend_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsUpdateScale")
            {
                foreach (TagSeries series in PlotModel.Series)
                {
                    series.MaxMinData.Update();
                }
                return;
            }
            if (e.PropertyName == "ScaleOption")
            {
                ResetKeepMaxMin();
            }
            if (e.PropertyName == "MaxViewable")
            {
                BottomControlHeight = (Trend.MaxViewable +1)* 20;
                var viewableCount = Math.Min(Trend.MaxViewable, Trend.Pens.Count(p => p.Visible));
                ((Trend)Control).ResetBottomViewable(viewableCount);
                return;
            }
            if (e.PropertyName == "Position")
            {
                ChangePosition();
                return;
            }

            if (e.PropertyName== "DisplayPenValue")
            {
                LiveDataVisibility = _trend.DisplayPenValue ? Visibility.Visible : Visibility.Collapsed;
                return;
            }

            if (e.PropertyName == "DisplayLineLegend")
            {
                MinMaxDataCollectionVisibility = _trend.DisplayLineLegend ? Visibility.Visible : Visibility.Collapsed;
                return;
            }
            if (e.PropertyName == "IsStop")
            {
                return;
            }

            if (e.PropertyName == "StartTime")
            {
                //TODO(zyl):scroll to the specified time

                return;
            }

            if (e.PropertyName == "UpdateIsolated")
            {
                ResetIsolatedTrend();
                if (_trend.Isolated)
                {
                    (Control as Trend)?.CalculateHeight();
                    //if (_trend.IsScrolling)
                    //{
                    //    return;
                    //}
                    foreach (var plotModel in PlotModels)
                    {
                        plotModel?.PlotView?.InvalidatePlot(false);
                    }
                }

                AbortObtainThread();
                //SyncScale();
                if (_trend.IsScrolling)
                {
                    FillBuffer();
                    UpdateView(false,true);
                    Task.Run(() => { StartObtainPoint(); }).ConfigureAwait(false);
                }
                else
                {
                    UpdateView(false,false);
                }
                return;
            }

            if (e.PropertyName == "ChartStyle")
            {
                if (_trend.IsScrolling)
                {
                    if (_trend.ChartStyle == ChartStyle.Standard)
                    {
                        TurnToStandard();
                    }
                    else
                    {
                        TurnToPenAxis();
                    }
                }
                else
                {
                    if (_trend.ChartStyle == ChartStyle.Standard)
                    {
                        TurnToStandard();
                    }
                    else
                    {
                        TurnToPenAxis();
                    }
                    ReGetData();
                    Update(false);
                }
                return;
            }
            if (e.PropertyName == "IsScrolling")
            {
                //var trend = Trend as SimpleServices.Common.Trend;
                //if (trend != null)
                //{
                //    trend.RunTime=DateTime.Now;
                //}
                if (Trend.IsScrolling)
                    StartScroll();
                else
                    AbortObtainThread();
                return;
            }

            if (e.PropertyName == "CaptureSize" || e.PropertyName == "ExtraData")
            {
                CleanDataNotStart();
                return;
            }

            if (e.PropertyName == "SamplePeriod")
            {
                CleanDataNotStart();
                _trendDataCenter.UpdateSamplePeriod();
                UpdateSeries();
                //_trendServer.UpdateSamplePeriod(_trend.SamplePeriod);
                //_isolatedTrendServer.UpdateSamplePeriod(_trend.SamplePeriod);
                SamplePeriod = $"Periodic {_trend.SamplePeriod} ms";
                return;
            }

            if (e.PropertyName == "DisplayScrollingMechanism")
            {
                ScrollingToolbar = _trend.DisplayScrollingMechanism ? Visibility.Visible : Visibility.Collapsed;
            }

            if (e.PropertyName == "DisplayMinAndMaxValue")
            {
                foreach (var maxMinData in MinMaxDataCollection)
                {
                    maxMinData.ValueVisibility =
                        _trend.DisplayMinAndMaxValue ? Visibility.Visible : Visibility.Collapsed;
                }

                return;
            }

            if (e.PropertyName == "PenCaptionMaxLength")
            {
                foreach (var maxMinData in MinMaxDataCollection)
                {
                    maxMinData.CaptionLength =
                        _trend.PenCaptionMaxLength;
                }

                return;
            }

            if (e.PropertyName == "Name")
            {
                Caption = $"Trend - {_trend.Name}";
                UpdateCaptionAction?.Invoke(Caption);
                if (_trend.Isolated)
                {
                    foreach (var model in PlotModels)
                    {
                        model.PlotView?.InvalidatePlot(false);
                    }
                }
                else
                    PlotModel.PlotView?.InvalidatePlot(false);
                return;
            }

            if (e.PropertyName == "Isolated")
            {
                SetIsolated();
                
                SyncPlotLeft();
                return;
            }

            {
                foreach (var plotModel in PlotModels)
                {
                    SetPlotModel(plotModel, e);
                }
            }

            {
                SetPlotModel(PlotModel, e);
            }

            SyncPlotLeft();
            SetTooltips();
        }

        private void AbortObtainThread()
        {
            try
            {
                if (_obtainPointThread.IsAlive)
                {
                    if (_trend.Isolated)
                    {
                        foreach (var plotModel in PlotModels.Where(p => p.Visibility == Visibility.Visible))
                        {
                            plotModel.CompleteArrange -= PlotModel_CompleteArrange;
                        }
                    }
                    else
                    {
                        PlotModel.CompleteArrange -= PlotModel_CompleteArrange;
                    }
                    //_obtainPointThread.Abort();
                    _isRendering = 0;
                    _completeCount = 0;
                }
            }
            catch (Exception)
            {
                //ignore
            }
        }

        private void StartScroll()
        {
            ScheduleCommand4Checked = !IsRun;
            RaisePropertyChanged("ScrollChecked");
            //if (!(_trend is TrendLog))
            //{
            //    if (!Controller.IsOnline) return;
            //    if (CanExecuteRunCommand()) return;
            //}

            if (_isZoom)
            {
                ExecuteResetCommand();
                if (_trend.Isolated)
                {
                    foreach (var plotModel in PlotModels)
                    {
                        plotModel.PlotView.ActualModel.ResetAllAxes();
                        plotModel.PlotView.InvalidatePlot();
                    }
                }

                _isZoom = false;
            }

            try
            {
                ResetKeepMaxMin();
                if (IsRun)
                {
                    StartObtainPoint(true);
                }
                else
                {
                    if (PlotModel?.Series.Any() ?? false)
                        _stopX = ((TagSeries)PlotModel.Series[0]).LastDataPoint.X;
                    try
                    {
                        _obtainPointThread?.Abort();
                    }
                    finally
                    {
                        _waitAbort = false;
                        _obtainPointThread = null;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Trace($"{_trend.Name}.Isrun.error:{e.Message}");
            }
        }

        private void Axes_CollectionChanged(object sender, ElementCollectionChangedEventArgs<OxyPlot.Axes.Axis> e)
        {
            foreach (var item in e.AddedItems)
            {
                //if(PlotModels.Contains(item.Parent))return;
                SetAxesGrid(item, item.PlotModel);
            }
        }

        private void SyncIsolatedAndUnIsolated()
        {
            foreach (var plotModel in PlotModels)
            {
                var index = PlotModels.IndexOf(plotModel);
                var unIsolatedSeries = PlotModel.Series[index] as TagSeries;
                var series = plotModel.Series[0] as TagSeries;
                Debug.Assert(series != null && unIsolatedSeries != null);
                series?.Sync(unIsolatedSeries);
            }
        }
    }
}
