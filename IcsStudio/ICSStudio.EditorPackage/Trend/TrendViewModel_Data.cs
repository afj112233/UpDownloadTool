using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using OxyPlot;
using OxyPlot.Axes;
using ScaleOption = ICSStudio.Interfaces.Common.ScaleOption;

namespace ICSStudio.EditorPackage.Trend
{
    public partial class TrendViewModel
    {
        private Thread _obtainPointThread;

        private void StartDataCenterThread()
        {
            _trendDataCenter.Start();
            
        }

        private void StopDataCenterThread()
        {
            _trendDataCenter.Stop();
        }

        private void GetData(bool start)
        {
            StartDataCenterThread();
        }

        private void CleanData(bool isOnlyCleanData)
        {
            //if (_trend.Isolated)
            {
                foreach (var plotModel in PlotModels)
                {
                    if (_trend is TrendLog) (plotModel.Series[0] as TagSeries)?.Clean(true);
                    else (plotModel.Series[0] as TagSeries)?.Clean(CanExecuteStopCommand());
                    plotModel.ClearValueBar();
                    if (!isOnlyCleanData)
                        plotModel.PlotView?.InvalidatePlot();
                }
            }
            //else
            {
                foreach (TagSeries series in PlotModel.Series)
                {
                    if (_trend is TrendLog) series.Clean(true);
                    else series.Clean(CanExecuteStopCommand());
                }

                PlotModel.ClearValueBar();
                if (!isOnlyCleanData)
                    PlotModel.PlotView?.InvalidatePlot();
            }

            _trendDataCenter?.Stop();
            _trendDataCenter?.ClearCount();
            GetData(true);
            IsRun = true;
            if (IsRun)
            {
                StartScroll();
            }
            else
            {
                IsRun = true;
            }
            ScheduleCommand4Checked = false;
            RaisePropertyChanged("ScrollChecked");
        }
        
        private int _stopLock = 0;

        private void StopTimer()
        {
            if (_stopLock > 0) return;
            Task.Run(async () =>
            {
                try
                {
                    Interlocked.Increment(ref _stopLock);
                    if (_obtainPointThread.IsAlive)
                    {
                        _waitAbort = true;
                        while (_obtainPointThread.IsAlive)
                        {
                            await Task.Delay(50).ConfigureAwait(false);
                        }
                        
                    }

                    //_obtainPointThread?.Abort();
                    _waitAbort = false;
                    _obtainPointThread = null;
                    //_timer.Stop();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    Interlocked.Decrement(ref _stopLock);
                }
            }).ConfigureAwait(false);
        }

        private int _startObject = 0;
        
        private void StartObtainPoint(bool interrupt = false)
        {
            if (_startObject > 0) return;
            
            _isResetTrend = false;
            Interlocked.Increment(ref _startObject);
            {
                if (_obtainPointThread == null)
                {
                    _obtainPointThread = new Thread(SyncObtainPoint);
                    _obtainPointThread.Start();
                    Interlocked.Decrement(ref _startObject);
                    return;
                }

                try
                {
                    if (interrupt)
                    {
                        _obtainPointThread.Abort();
                        _obtainPointThread = new Thread(SyncObtainPoint);
                        _obtainPointThread.Start();
                    }
                    else
                    {
                        if (_obtainPointThread.IsAlive)
                        {
                            _waitAbort = true;
                            while (_obtainPointThread.IsAlive)
                            {
                                Thread.Sleep(50);
                            }

                            _waitAbort = false;
                        }
                        _obtainPointThread = new Thread(SyncObtainPoint);
                        _obtainPointThread.Start();
                    }
                }
                catch (Exception)
                {
                    _obtainPointThread = new Thread(SyncObtainPoint);
                    _obtainPointThread.Start();
                }
                finally
                {
                    Interlocked.Decrement(ref _startObject);
                }
            }
        }

        private bool _waitAbort = false;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private bool _trendLogFirstObtain = true;
        private void SyncObtainPoint()
        {
            try
            {
                if(_waitAbort)
                {
                    return;
                }
                if (_stopwatch.IsRunning)
                {
                    while (!NeedUpdate)
                    {
                        Thread.Sleep(200);
                    }

                }

                if (_waitAbort)
                {
                    return;
                }
                
                _stopwatch.Restart();
                FillBuffer();

                while (_isResetTrend)
                {
                    Thread.Sleep(40);
                }
                UpdateView();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void FillBuffer()
        {
            if (_trend is TrendLog)
            {
                var x = GetCurrentX();
                DateTime time;
                if (_trendLogFirstObtain)
                {
                    _trendLogFirstObtain = false;
                    time = _trend.StartTime + TimeSpan.FromSeconds(x);
                }
                else
                {
                    time = _trend.StartTime + TimeSpan.FromSeconds(x + 1);
                }

                var timeAxis = (PlotModel.Axes.FirstOrDefault(a => a is TimeSpanAxis) as TimeSpanAxis);
                var endTime = time + _trend.TimeSpan;

                SetTitle(timeAxis, time, endTime);
                _dataCenter?.GetTimeSpanData(time);
            }
            else
            {
                var now = DateTime.Now;
                var end = now.AddSeconds(-_trend.TimeSpan.TotalSeconds);
                _dataCenter?.GetTimeSpanData(end);
                var timeAxis = (PlotModel.Axes.FirstOrDefault(a => a is TimeSpanAxis) as TimeSpanAxis);
                SetTitle(timeAxis,now,end);
            }
        }

        private void SetTitle(TimeSpanAxis timeAxis,DateTime now,DateTime end)
        {

            Clock = timeAxis?.FormatTime(now);
            if (_trend.ChartStyle == ChartStyle.XY)
            {
                SetXYPlotTitle($"{timeAxis?.FormatTime(now.AddSeconds(-_trend.TimeSpan.TotalSeconds))} - {Clock}", GetDate(now, end));
            }
            else
            {
                SetPlotTitle(GetDate(now, end));
            }
        }

        private string GetDate(DateTime start,DateTime end)
        {
            if (start.Date == end.Date)
            {
                return start.ToString("yyyy年MM月dd日");
            }
            else
            {
                return $"{start:yyyy年MM月dd日} - {end:yyyy年MM月dd日}";
            }
        }

        private void ReGetData()
        {
            var x = GetCurrentX();
            DateTime time;
            if (_trendLogFirstObtain)
            {
                _trendLogFirstObtain = false;
                time = StartTime + TimeSpan.FromSeconds(x);
            }
            else
            {
                time = StartTime + TimeSpan.FromSeconds(x + 1);
            }
            _dataCenter?.GetTimeSpanData(time);
        }

        private void SetXYPlotTitle(string timeInfo,string date)
        {
            var title= $"{timeInfo}  {$"{_trend.GraphTitle}: " + date}";
            PlotModel.Title = title;
            foreach (var plotModel in PlotModels)
            {
                plotModel.Title = title;
            }
        }

        private void SetPlotTitle(string date)
        {
            var title = $"{$"{_trend.GraphTitle}: " + date}";
            PlotModel.Title = title;
            foreach (var plotModel in PlotModels)
            {
                plotModel.Title = title;
            }
        }

        private void CleanDataNotStart()
        {
            IsRun = false;
            ScheduleCommand4Checked = true;
            RaisePropertyChanged("ScrollChecked");
            GetData(false);
            
            {
                foreach (var plotModel in PlotModels)
                {
                    (plotModel.Series[0] as TagSeries)?.Clean(CanExecuteStopCommand());
                    plotModel.PlotView?.InvalidatePlot();
                }
            }

            {
                foreach (TagSeries series in PlotModel.Series)
                {
                    series.Clean(CanExecuteStopCommand());
                }

                PlotModel.PlotView?.InvalidatePlot();
            }
        }

    }
}
