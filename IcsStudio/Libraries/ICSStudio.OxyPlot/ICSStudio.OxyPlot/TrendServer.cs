using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.Utils;
using OxyPlot.Axes;

namespace OxyPlot
{
    public class TrendServer
    {
        private List<TagSeries> _tagSeries;
        private List<TagSeries> _isolateTagSeries;
        private ConcurrentQueue<SampleData> _queue;
        private readonly Trend _trend;
        private MicroTimer _microTimer;

        private List<Tuple<ITag, string>> _sampleTags;

        public TrendServer(Controller controller, long samplePeriod, List<string> tags)
        {
            Controller = controller;
            SamplePeriod = samplePeriod;
            Tags = tags;

            _queue = new ConcurrentQueue<SampleData>();
            _sampleTags = new List<Tuple<ITag, string>>();

            _microTimer = new MicroTimer(samplePeriod * 1000);
            _microTimer.IgnoreEventIfLateBy = _microTimer.Interval / 2;
            _microTimer.MicroTimerElapsed += OnTimer;

            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                controller, "IsOnlineChanged", OnIsOnlineChanged);
        }

        public TrendServer(Trend trend)
        {
            _trend = trend;
            Controller = trend.ParentController as Controller;
            SamplePeriod = trend.SamplePeriod;
            Tags = new List<string>();
            foreach (var pen in trend.Pens)
            {
                Tags.Add(pen.Name);
            }

            _queue = new ConcurrentQueue<SampleData>();
            _sampleTags = new List<Tuple<ITag, string>>();

            _microTimer = new MicroTimer(SamplePeriod * 1000);
            _microTimer.IgnoreEventIfLateBy = _microTimer.Interval / 2;
            _microTimer.MicroTimerElapsed += OnTimer;

            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                Controller, "IsOnlineChanged", OnIsOnlineChanged);
        }

        public bool IsOnlineChange { set; get; } = false;

        public void StartNewOne()
        {
            if (!IsOnlineChange)
                Cleanup();
            else
                IsOnlineChange = false;
            Reset();
        }

        public void UpdateSamplePeriod(double samplePeriod)
        {
            _microTimer.Interval = (long) (samplePeriod * 1000);
            _microTimer.IgnoreEventIfLateBy = _microTimer.Interval / 2;

            Clear();
        }

        public void Cleanup()
        {
            Stop();
        }

        private DataCenter _dataCenter;

        public void ResetDataCenter(DataCenter dataCenter)
        {
            _dataCenter = dataCenter;
        }

        public void AddTagSeries(List<TagSeries> tagSeries)
        {
            _tagSeries = tagSeries;
        }

        public void AddIsolateTagSeries(List<TagSeries> isolateTagSeries)
        {
            _isolateTagSeries = isolateTagSeries;
        }

        public void AddTagSeries(TagSeries tagSeries)
        {
            if (tagSeries != null && !_tagSeries.Contains(tagSeries)) _tagSeries.Add(tagSeries);
            Reset();
        }

        public void TagNameChanged()
        {
            Tags.Clear();
            foreach (var pen in _trend.Pens)
            {
                Tags.Add(pen.Name);
            }
        }

        private void Reset()
        {
            Cleanup();
            SamplePeriod = _trend.SamplePeriod;
            Tags.Clear();
            foreach (var pen in _trend.Pens)
            {
                Tags.Add(pen.Name);
            }

            _queue = new ConcurrentQueue<SampleData>();
            _sampleTags = new List<Tuple<ITag, string>>();

            _microTimer.MicroTimerElapsed -= OnTimer;
            _microTimer = new MicroTimer(SamplePeriod * 1000);
            _microTimer.IgnoreEventIfLateBy = _microTimer.Interval / 2;
            _microTimer.MicroTimerElapsed += OnTimer;

        }

        public void AddIsolateTagSeries(TagSeries tagSeries)
        {
            if (tagSeries != null && !_isolateTagSeries.Contains(tagSeries)) _isolateTagSeries.Add(tagSeries);
        }

        public void DeleteTagSeries(TagSeries tagSeries)
        {
            if (tagSeries != null && _tagSeries.Contains(tagSeries)) _tagSeries.Remove(tagSeries);
            Reset();
        }

        public void DeleteIsolateTagSeries(TagSeries tagSeries)
        {
            if (tagSeries != null && _isolateTagSeries.Contains(tagSeries)) _isolateTagSeries.Remove(tagSeries);
        }

        public void UpdateSamplePeriod()
        {
            Reset();
        }

        public void ClearCount()
        {
            //Count = 0;
            Clear();
            foreach (var tagSeries in _tagSeries)
            {
                tagSeries.MaxMinData.Max = 0;
                tagSeries.MaxMinData.Min = 0;
            }

            foreach (var tagSeries in _isolateTagSeries)
            {
                tagSeries.MaxMinData.Max = 0;
                tagSeries.MaxMinData.Min = 0;
            }
        }
        
        public void Clear()
        {
            while (!_queue.IsEmpty)
            {
                SampleData trendData;
                _queue.TryDequeue(out trendData);
            }

            _dataCenter?.Clean();
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            Task.Run(() =>
            {
                if (!Controller.IsOnline)
                {
                    _microTimer.Enabled = false;
                }
            });
        }

        public Controller Controller { get; }
        public long SamplePeriod { get; private set; } //ms
        public List<string> Tags { get; }

        public void Start()
        {
            if (_dataCenter != null)
            {
                _dataCenter.IsEnable = true;
            }
            _microTimer.Enabled = false;

            while (!_queue.IsEmpty)
            {
                SampleData trendData;
                _queue.TryDequeue(out trendData);
            }

            if (Controller.IsOnline)
            {
                CreateSampleTags(_sampleTags, Tags);

                _microTimer.Enabled = true;
            }
        }

        private void CreateSampleTags(List<Tuple<ITag, string>> tags, List<string> tagNames)
        {
            tags.Clear();

            foreach (var name in tagNames)
            {
                tags.Add(NameToTag(name));
            }

        }

        private Tuple<ITag, string> NameToTag(string name)
        {
            string specifier = name;
            ITagCollection tagCollection = Controller.Tags;

            if (name.StartsWith("\\"))
            {
                var index = name.IndexOf(".", StringComparison.OrdinalIgnoreCase);
                string program = name.Substring(1, index - 1);
                specifier = name.Substring(index + 1, name.Length - index - 1);

                tagCollection = Controller.Programs[program].Tags;
            }

            int indexOfDot = specifier.IndexOf(".", StringComparison.OrdinalIgnoreCase);
            string tagName = indexOfDot > 0 ? specifier.Substring(0, indexOfDot) : specifier;

            int indexOfBracket = tagName.IndexOf("[", StringComparison.OrdinalIgnoreCase);
            if (indexOfBracket > 0)
                tagName = tagName.Substring(0, indexOfBracket);

            ITag tag = tagCollection[tagName];
            return new Tuple<ITag, string>(tag, specifier);
        }

        public void Stop()
        {
            //_microTimer.Enabled = false;
            _microTimer.Stop();
            if (_dataCenter != null)
            {
                _dataCenter.IsEnable = false;
            }
        }

        public bool IsEnable => _microTimer.Enabled;

        public bool IsEmpty => _queue.IsEmpty;

        public double DataCount => _queue.Count;

        public bool TryGetTrendData(out SampleData data)
        {
            if (HasErrorTag)
            {
                data = null;
                return false;
            }

            return _queue.TryDequeue(out data);
        }

        public bool HasErrorTag { get; private set; }

        private void OnTimer(object sender, MicroTimerEventArgs args)
        {
            if (Controller.IsOnline)
            {
                HasErrorTag = false;
                int count = _sampleTags.Count;
                Task<Tuple<int, string>>[] tasks = new Task<Tuple<int, string>>[count];
                try
                {
                    var time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                    for (int i = 0; i < count; i++)
                    {
                        var item = _sampleTags[i];
                        if (item.Item1 == null || string.IsNullOrEmpty(item.Item2))
                        {
                            HasErrorTag = true;
                            Stop();
                            return;
                        }

                        tasks[i] = Controller.GetTagValueFromPLC(item.Item1, item.Item2);
                    }
                    
                    // ReSharper disable once CoVariantArrayConversion
                    Task.WaitAll(tasks);
                    var data=new List<string>();
                    data.Add(time);
                    for (int i = 0; i < count; i++)
                    {
                        //sampleData.Values.Add(tasks[i].Result.Item2);
                        var value = tasks[i].Result.Item2;

                        if (value.Equals("Infinity", StringComparison.OrdinalIgnoreCase))
                        {
                           // _tagSeries[i].Enqueue(time,float.MaxValue.ToString());
                            data.Add(float.MaxValue.ToString());
                        }
                        else if (
                            value.Equals("-Infinity", StringComparison.OrdinalIgnoreCase))
                        {
                           // _tagSeries[i].Enqueue(time,float.MinValue.ToString());
                            data.Add(float.MinValue.ToString());
                        }
                        else
                        {
                            //_tagSeries[i].Enqueue(time,value);
                            data.Add(value);
                        }
                    }
                    _dataCenter.Enqueue(data);
                    //_queue.Enqueue(sampleData);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    if (!Controller.IsOnline)
                        Stop();
                }

            }
        }
    }

    public class SampleData
    {
        public DateTime Time { get; set; }
        public List<string> Values { get; set; }
    }
}