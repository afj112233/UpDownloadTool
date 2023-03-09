using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ICSStudio.Cip.Objects;
using ICSStudio.CipConnection;
using ICSStudio.Database.Database;
using ICSStudio.SimpleServices;
using ICSStudio.SimpleServices.DeviceModule;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;

namespace ICSStudio.UIServicesPackage
{
    public partial class ConnectedToViewModel
    {
        public string Condition
        {
            get
            {
                if (_compareResult == CompareResult.ControllerIsNewer)
                    return "Condition: The project in the controller is newer than the open project.";

                if (_compareResult == CompareResult.OfflineProjectIsNewer)
                    return "Condition: The open project is newer than the project in the controller.";

                return "Condition: The open project doesn't match the project in the controller.";
            }
        }

        private string _connectedControllerName;
        public string ConnectedControllerName => _connectedControllerName;
        private string _connectedControllerType;
        public string ConnectedControllerType => _connectedControllerType;
        private string _connectedControllerCommPath;
        public string ConnectedControllerCommPath => _connectedControllerCommPath;
        private string _connectedControllerSerialNumber=null;
        public string ConnectedControllerSerialNumber => _connectedControllerSerialNumber;
        private string _connectedControllerSecurity;
        public string ConnectedControllerSecurity => _connectedControllerSecurity;

        private string _offlineProjectControllerName;
        public string OfflineProjectControllerName => _offlineProjectControllerName;
        private string _offlineProjectControllerType;
        public string OfflineProjectControllerType => _offlineProjectControllerType;
        private string _offlineProjectFile;
        public string OfflineProjectFile => _offlineProjectFile;
        private string _offlineProjectSerialNumber;
        public string OfflineProjectSerialNumber => _offlineProjectSerialNumber;
        private string _offlineProjectSecurity;
        public string OfflineProjectSecurity => _offlineProjectSecurity;

        private void CreateInformation()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await TaskScheduler.Default;

                await GetConnectedControllerInfoAsync();

                GetOfflineProjectInfo();

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                ShowInformation();

            });

        }

        private async Task<int> GetConnectedControllerInfoAsync()
        {
            try
            {
                CIPController cipController = new CIPController(0, _controller.CipMessager);

                _connectedControllerName = await cipController.GetName();

                DeviceConnection deviceConnection = _controller.CipMessager as DeviceConnection;
                Debug.Assert(deviceConnection != null);

                _connectedControllerCommPath = deviceConnection.IpAddress;

                CIPIdentity cipIdentity = new CIPIdentity(1, _controller.CipMessager);

                var result = await cipIdentity.GetAttributesAll();
                if (result == 0)
                {
                    ushort vendorId = Convert.ToUInt16(cipIdentity.VendorID);
                    ushort deviceType = Convert.ToUInt16(cipIdentity.DeviceType);
                    ushort productCode = Convert.ToUInt16(cipIdentity.ProductCode);

                    DBHelper dbHelper = new DBHelper();
                    _connectedControllerType =
                        dbHelper.GetCatalogNumber(vendorId, deviceType, productCode);

                }

                _connectedControllerSecurity = "No Protection";
                return 0;
            }
            catch (Exception)
            {
                _connectedControllerName = string.Empty;
                _connectedControllerCommPath = string.Empty;
                _connectedControllerSecurity = string.Empty;
            }

            return -1;
        }

        private void GetOfflineProjectInfo()
        {
            if (!string.IsNullOrEmpty(_controller.ProjectLocaleName))
            {
                _offlineProjectControllerName = _controller.Name;

                LocalModule localModule = _controller.DeviceModules["Local"] as LocalModule;
                Debug.Assert(localModule != null);
                _offlineProjectControllerType = localModule.CatalogNumber;

                _offlineProjectFile = _controller.ProjectLocaleName;
                _offlineProjectSerialNumber = _controller.ProjectSN.ToString("X08");
                _offlineProjectSecurity = "No Protection";
            }
        }

        private void ShowInformation()
        {
            RaisePropertyChanged(nameof(ConnectedControllerName));
            RaisePropertyChanged(nameof(ConnectedControllerType));
            RaisePropertyChanged(nameof(ConnectedControllerCommPath));
            RaisePropertyChanged(nameof(ConnectedControllerSerialNumber));
            RaisePropertyChanged(nameof(ConnectedControllerSecurity));

            RaisePropertyChanged(nameof(OfflineProjectControllerName));
            RaisePropertyChanged(nameof(OfflineProjectControllerType));
            RaisePropertyChanged(nameof(OfflineProjectFile));
            RaisePropertyChanged(nameof(OfflineProjectSerialNumber));
            RaisePropertyChanged(nameof(OfflineProjectSecurity));

        }
    }
}
