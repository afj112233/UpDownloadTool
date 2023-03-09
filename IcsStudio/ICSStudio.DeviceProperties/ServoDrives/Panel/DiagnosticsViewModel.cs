using System.Windows.Controls;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.DeviceProperties.ServoDrives.Panel
{
    public class DiagnosticsViewModel: DeviceOptionPanel
    {
        public DiagnosticsViewModel(UserControl panel) : base(panel)
        {

        }

        public bool IsTransmissionTimingStatisticsEnabled => !IsOnline;

        public override void Refresh()
        {
            base.Refresh();
            RaisePropertyChanged(nameof(IsTransmissionTimingStatisticsEnabled));
        }
    }
}
