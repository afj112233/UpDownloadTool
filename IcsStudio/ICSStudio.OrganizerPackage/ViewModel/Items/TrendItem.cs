using System.ComponentModel;
using ICSStudio.Interfaces.Common;
using ICSStudio.UIInterfaces.Project;

namespace ICSStudio.OrganizerPackage.ViewModel.Items
{
    internal class TrendItem : OrganizerItem
    {
        private readonly ITrend _trend;

        public TrendItem(ITrend trend)
        {
            _trend = trend;

            Name = _trend.Name;
            Kind = ProjectItemType.Trend;
            AssociatedObject = _trend;

            PropertyChangedEventManager.AddHandler(_trend, OnPropertyChanged, "");
        }

        public override void Cleanup()
        {
            PropertyChangedEventManager.RemoveHandler(_trend, OnPropertyChanged, "");
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                Name = _trend.Name;

                SortByName();
            }
        }

        protected override void DisplayNameConvert()
        {
            DisplayName = Name;
        }
    }
}
