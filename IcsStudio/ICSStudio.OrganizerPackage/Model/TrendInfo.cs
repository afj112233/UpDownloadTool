using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;


namespace ICSStudio.OrganizerPackage.Model
{
    internal class TrendInfo : BaseSimpleInfo
    {
        private readonly Trend _trend;

        public TrendInfo(Trend trend, ObservableCollection<SimpleInfo> infoSource)
            : base(infoSource)
        {
            _trend = trend;

            if (_trend != null)
            {
                CreateInfoItems();

                PropertyChangedEventManager.AddHandler(_trend,
                    OnTrendPropertyChanged, string.Empty);

                CollectionChangedEventManager.AddHandler(_trend, OnPensChanged);

            }
        }

        private void OnPensChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                SetSimpleInfo("Number of Pens", _trend.Pens.Count().ToString());
            });

        }

        private void OnTrendPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (e.PropertyName == "Description")
                {
                    SetSimpleInfo("Description", _trend.Description);
                }
            });

        }

        private void CreateInfoItems()
        {
            if (InfoSource != null)
            {
                InfoSource.Add(new SimpleInfo { Name = "Description", Value = _trend.Description });
                InfoSource.Add(new SimpleInfo { Name = "Number of Pens", Value = _trend.Pens.Count().ToString() });
            }

        }
    }
}
