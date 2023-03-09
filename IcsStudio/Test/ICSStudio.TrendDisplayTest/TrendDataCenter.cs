using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using OxyPlot;
using OxyPlot.Axes;


namespace ICSStudio.TrendDisplayTest
{
    public class TrendDataCenter
    {
        private string _json = @"E:\2020\icsstudio\build\Debug\json\trend_display.json";

        private readonly Controller _controller;

        private readonly SimpleTrendServer _trendServer;

        private Task _task;
        private CancellationTokenSource _tokenSource;

        private readonly List<DataPoint> _values;

        public TrendDataCenter()
        {
            _values = new List<DataPoint>();

            _controller = Controller.Open(_json);

            _trendServer = new SimpleTrendServer(_controller, 2, new List<string>()
            {
                @"\MainProgram.result"
            });
        }


        public void Start()
        {
            Stop();

            _tokenSource = new CancellationTokenSource();

            Task.Run(async () => { await StartAsync(); });

        }

        public void Stop()
        {
            if (_task != null)
            {
                _tokenSource.Cancel();
                _task.Wait();
                _task.Dispose();
                _tokenSource.Dispose();
                _task = null;
                _tokenSource = null;
            }

            lock (_values)
                _values.Clear();

            _trendServer.Stop();

            _controller.GoOffline();
        }

        private async Task StartAsync()
        {
            _controller.GenCode();
            await _controller.ConnectAsync("192.168.1.211");
            await _controller.Download(ControllerOperationMode.OperationModeNull, false, false);
            await _controller.RebuildTagSyncControllerAsync();
            await _controller.UpdateState();
            await _controller.ChangeOperationMode(ControllerOperationMode.OperationModeRun);

            _trendServer.Start();

            _task = Task.Factory.StartNew(() => Work(_tokenSource.Token), _tokenSource.Token);
        }

        private void Work(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                lock (_values)
                {
                    while (true)
                    {
                        SampleData sampleData;
                        if (_trendServer.TryGetTrendData(out sampleData))
                        {
                            _values.Add(new DataPoint(DateTimeAxis.ToDouble(sampleData.Time),
                                double.Parse(sampleData.Values[0])));
                        }
                        else
                        {
                            break;
                        }
                    }

                    int removeCount = _values.Count - 60000;
                    if (removeCount > 0)
                    {
                        _values.RemoveRange(0, removeCount);
                    }

                }

                Thread.Sleep(250);
            }
        }

        public List<DataPoint> GetData(DateTime startTime, DateTime endTime)
        {
            double start = DateTimeAxis.ToDouble(startTime);
            double end = DateTimeAxis.ToDouble(endTime);
            lock (_values)
                return _values.Where(x => x.X >= start && x.Y <= end).ToList();
        }
    }
}
