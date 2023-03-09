using System.ComponentModel;
using System.Diagnostics.Contracts;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.UIInterfaces.Project;

namespace ICSStudio.OrganizerPackage.ViewModel.Items
{
    internal class PointIORootItem : OrganizerItem
    {
        private readonly DeviceModule _deviceModule;
        private readonly Port _port;

        public PointIORootItem(IDeviceModule deviceModule)
        {
            _deviceModule = deviceModule as DeviceModule;
            Contract.Assert(_deviceModule != null);

            _port = _deviceModule.GetFirstPort(PortType.PointIO);
            Contract.Assert(_port != null);

            Name = $"PointIO {_port.Bus.Size} Slot Chassis";
            Kind = ProjectItemType.Bus;
            AssociatedObject = _deviceModule;

            PropertyChangedEventManager.AddHandler(_port.Bus, OnPortPropertyChanged, "");
        }

        public override void Cleanup()
        {
            base.Cleanup();
            PropertyChangedEventManager.RemoveHandler(_port.Bus, OnPortPropertyChanged, "");
        }

        private void OnPortPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Size")
            {
                Name = $"PointIO {_port.Bus.Size} Slot Chassis";
            }
        }

        protected override void DisplayNameConvert()
        {
            DisplayName = Name;
        }
    }
}
