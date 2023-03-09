using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICSStudio.TrendDisplayTest
{
    public class DataCreator
    {
        private CancellationTokenSource _tokenSource;
        private Task _task;
        private readonly List<DataPoint> _values;

        public DataCreator()
        {
            _values = new List<DataPoint>();
        }

        public void Start()
        {
            Stop();
            _tokenSource = new CancellationTokenSource();
            _task = Task.Factory.StartNew(() => Work(_tokenSource.Token), _tokenSource.Token);
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
        }

        private void Work(CancellationToken token)
        {
            DateTime dateTime = DateTime.Now;
            double a = 0.0;
            while (!token.IsCancellationRequested)
            {
                lock (_values)
                {
                    DateTime now = DateTime.Now;

                    while (dateTime < now)
                    {
                        _values.Add(new DataPoint(DateTimeAxis.ToDouble(dateTime), Math.Sin(a)));
                        dateTime = dateTime.AddMilliseconds(1.0);
                        a += 0.01;
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
