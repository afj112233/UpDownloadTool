using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using OxyPlot;
using OxyPlot.Axes;
using ScaleOption = ICSStudio.Interfaces.Common.ScaleOption;
using Task = System.Threading.Tasks.Task;

namespace ICSStudio.EditorPackage.Trend
{
    public partial class TrendViewModel
    {
        private void SyncIsolatedScale()
        {
            if (!(_trend.ScaleOption == ScaleOption.Same && _trend.Isolated)) return;
            double min = 0, max = 0;
            foreach (var plotModel in PlotModels)
            {
                var series = plotModel.Series[0] as TagSeries;
                if (series != null)
                {
                    min = Math.Min(series.Min, min);
                    max = Math.Max(series.Max, max);
                }
            }

            foreach (var plotModel in PlotModels)
            {
                var axesY = plotModel.Axes.FirstOrDefault(a => !(a is TimeSpanAxis));
                if (axesY != null)
                {
                    axesY.Maximum = max;
                    axesY.Minimum = min;
                }
            }
        }

        internal void Update(bool getData = false, bool seriesGetData = true, bool syncControlWidth = true)
        {
            if (Interlocked.CompareExchange(ref _isRendering, 1, 0) == 0)
            {
                if (!_trend.Pens.Any())
                {
                    Interlocked.Exchange(ref _isRendering, 0);
                    return;
                }

                if (_trend.Isolated)
                {
                    double maxLeft = 0;

                    if (Trend.ScaleOption == ScaleOption.UsingPen)
                    {
                        //获取scale pen 范围
                        PlotModel scalePlotModel= PlotModels.FirstOrDefault(p => p.Name == _trend.ScalePen.Name);
                        Debug.Assert(scalePlotModel!=null);
                        if (!_trend.ScalePen.Visible)
                        {
                            scalePlotModel?.UpdateNotVisible();
                        }

                        var scaleY = scalePlotModel.Axes.FirstOrDefault(a => !a.IsHorizontal()&&a.IsAxisVisible);
                        foreach (var plotModel in PlotModels.Where(p => p.Visibility == Visibility.Visible).OrderByDescending(p => p==scalePlotModel))
                        {
                            if (plotModel != scalePlotModel&&((TagSeries)scalePlotModel.Series[0]).Points.Any())
                            {
                                var axisYCollection = plotModel.Axes.Where(a => !a.IsHorizontal());
                                foreach (var axisY in axisYCollection)
                                {
                                    axisY.Maximum = scaleY.ActualMaximum;
                                    axisY.Minimum = scaleY.ActualMinimum;
                                }
                            }
                            plotModel.PlotView?.InvalidatePlot(seriesGetData);
                            maxLeft = Math.Max(maxLeft, plotModel.ActualPlotMargins.Left);
                        }
                    }
                    else
                    {
                        foreach (var plotModel in PlotModels.Where(p => p.Visibility == Visibility.Visible).OrderByDescending(p => p.ShowTitle))
                        {
                            plotModel.PlotView?.InvalidatePlot(seriesGetData);
                            maxLeft = Math.Max(maxLeft, plotModel.ActualPlotMargins.Left);
                        }
                    }
                    

                    foreach (var plotModel in PlotModels.Where(p => p.Visibility == Visibility.Visible).OrderBy(p => p.ShowTitle))
                    {
                        if (getData)
                            plotModel.CompleteArrange += PlotModel_CompleteArrange;
                        plotModel.AxisYMaxLeft = maxLeft;
                        plotModel.PlotView?.InvalidatePlot(false);
                        foreach (var series in plotModel.Series)
                        {
                            var tagSeries = series as TagSeries;
                            tagSeries?.MaxMinData.Update();
                        }
                    }
                   
                }
                else
                {
                    if (getData)
                        PlotModel.CompleteArrange += PlotModel_CompleteArrange;
                    PlotModel.PlotView?.InvalidatePlot(seriesGetData);

                    foreach (var s in PlotModel.Series)
                    {
                        var tagSeries = s as TagSeries;
                        tagSeries?.MaxMinData.Update();
                    }
                
                }

                if (!getData)
                    Interlocked.Exchange(ref _isRendering, 0);
            }
        }

        private void SyncPlotLeft()
        {
            if (!Trend.Isolated)
            {
                PlotModel.NeedSeriesGetData = false;
                PlotModel.PlotView?.InvalidatePlot(true);
                PlotModel.NeedSeriesGetData = true;
                return;
            }

            double maxLeft = 0;

            foreach (var plotModel in PlotModels.OrderByDescending(p => p.ShowTitle))
            {
                plotModel.NeedSeriesGetData = false;
                plotModel.PlotView?.InvalidatePlot(true);
                plotModel.NeedSeriesGetData = true;
                maxLeft = Math.Max(maxLeft, plotModel.ActualPlotMargins.Left);
            }

            foreach (var plotModel in PlotModels.OrderBy(p => p.ShowTitle))
            {
                plotModel.AxisYMaxLeft = maxLeft;
                plotModel.PlotView?.InvalidatePlot(false);
            }
        }

        public void OnActiveChanged(bool isActive)
        {
            Interlocked.Exchange(ref _isActive, isActive ? 1 : 0);
            if (!isActive && _isRendering > 0)
            {
                StartObtainPoint(true);
            }
        }

        private bool _isReady = true;

        private void UpdateView(bool getData=true, bool seriesGetData = true)
        {
            if (!_isReady || _isRendering == 1)
            {
                return;
            }

            if (_isActive < 1)
            {
                StartObtainPoint(true);
                return;
            }
            
            //var hasError = false;
            SyncIsolatedScale();
#pragma warning disable VSTHRD001 // 避免旧线程切换 API
            Control?.Dispatcher.InvokeAsync(() =>
#pragma warning restore VSTHRD001 // 避免旧线程切换 API
            {
                Update(getData, seriesGetData);
                if (_trend.Isolated)
                {
                    foreach (var plotModel in PlotModels)
                    {
                        var series = plotModel.Series[0] as TagSeries;
                        if (series != null)
                        {
                            series.MaxMinData.Value = series.LastDataPoint.Y;
                        }
                    }
                }
                else
                {
                    foreach (TagSeries series in PlotModel.Series)
                    {
                        series.MaxMinData.Value = series.LastDataPoint.Y;
                    }
                }
            },DispatcherPriority.Render);
        }

        private int _completeCount = 0;
        private int _isRendering = 0;
        private int _isActive = 1;

        private void PlotModel_CompleteArrange(object sender, EventArgs e)
        {
            var plotModel = (PlotModel) sender;
            if (plotModel.Visibility != Visibility.Visible) return;
            plotModel.CompleteArrange -= PlotModel_CompleteArrange;
            if (!IsRun || _waitAbort)
            {
                Interlocked.Exchange(ref _isRendering, 0);
                return;
            }

            if (_trend.Isolated)
            {
                _completeCount++;
                if (_completeCount >= PlotModels.Count(p=>p.Visibility==Visibility.Visible))
                {
                    Interlocked.Exchange(ref _isRendering, 0);
                    _completeCount = 0;
                    if (CanExecuteStopCommand())
                        StartObtainPoint();
                }
            }
            else
            {
                Interlocked.Exchange(ref _isRendering, 0);
                if (CanExecuteStopCommand())
                    StartObtainPoint();
            }

        }

        private void UpdateSeries()
        {
            _dataCenter.UpdateSize();
            SyncScale();
        }

        private void SyncScale()
        {
            if (!_trend.IsScrolling)
            {
                SyncIsolatedScale();
                Update(false);
            }
            else
            {
                if (_canExecuteRunCommand)
                {
                    SyncIsolatedScale();
                    Update(false);
                }
            }
        }

        private void SeriesUpdateHandle(object sender, EventArgs e)
        {
            _trendDataCenter.TagNameChanged();
        }

        private void PlotModel_Updated(object sender, EventArgs e)
        {

            var plotModel = (PlotModel) sender;
            WeakEventManager<PlotModel, EventArgs>.RemoveHandler(plotModel, "Updated", PlotModel_Updated);
            ResetIsolatedTrend();
        }
    }
}
