using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using ICSStudio.DeviceProperties.Common;

namespace ICSStudio.DeviceProperties.ServoDrives.Panel
{
    public class NetworkViewModel : DeviceOptionPanel
    {
        public NetworkViewModel(UserControl panel) : base(panel)
        {
            RefreshCommand = new RelayCommand(ExecuteRefreshCommand, CanExecuteRefreshCommand);
        }

        public RelayCommand RefreshCommand { get; }

        #region Command

        private bool CanExecuteRefreshCommand()
        {
            return false;
        }

        private void ExecuteRefreshCommand()
        {
        }

        #endregion
    }
}
