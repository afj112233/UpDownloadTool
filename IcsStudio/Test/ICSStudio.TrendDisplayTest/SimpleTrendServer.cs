using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.Utils;

namespace ICSStudio.TrendDisplayTest
{
    public class SimpleTrendServer
    {
        private readonly ConcurrentQueue<SampleData> _queue;

        private readonly MicroTimer _microTimer;

        private readonly List<Tuple<ITag, string>> _sampleTags;

        public SimpleTrendServer(Controller controller, long samplePeriod, List<string> tags)
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

        public Controller Controller { get; }
        public long SamplePeriod { get; private set; } //ms
        public List<string> Tags { get; }

        public void Start()
        {
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

        public void Stop()
        {
            //_microTimer.Enabled = false;
            _microTimer.Stop();
        }

        public bool TryGetTrendData(out SampleData data)
        {
            return _queue.TryDequeue(out data);
        }

        private void OnTimer(object sender, MicroTimerEventArgs args)
        {
            if (Controller.IsOnline)
            {
                SampleData sampleData = new SampleData()
                {
                    Time = DateTime.Now,
                    Values = new List<string>()
                };

                int count = _sampleTags.Count;
                Task<Tuple<int, string>>[] tasks = new Task<Tuple<int, string>>[count];

                for (int i = 0; i < count; i++)
                {
                    var item = _sampleTags[i];

                    tasks[i] = Controller.GetTagValueFromPLC(item.Item1, item.Item2);
                }

                try
                {
                    // ReSharper disable once CoVariantArrayConversion
                    Task.WaitAll(tasks);

                    for (int i = 0; i < count; i++)
                    {
                        sampleData.Values.Add(tasks[i].Result.Item2);
                    }

                    _queue.Enqueue(sampleData);

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    Stop();
                }

            }
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
    }

    public class SampleData
    {
        public DateTime Time { get; set; }
        public List<string> Values { get; set; }
    }
}
