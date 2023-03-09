using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Windows;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.UIInterfaces.Project;

namespace ICSStudio.OrganizerPackage.ViewModel.Items
{
    class ExpansionIORootItem : OrganizerItem
    {
        private readonly DeviceModule _deviceModule;
        private readonly Port _port;

        public ExpansionIORootItem(IDeviceModule deviceModule, PortType portType)
        {

            _deviceModule = deviceModule as DeviceModule;
            Contract.Assert(_deviceModule != null);

            _port = _deviceModule.GetFirstPort(portType);
            Contract.Assert(_port != null);

            Name = "Expansion";
            Kind = ProjectItemType.ExpansionIO;
            AssociatedObject = _deviceModule;

            PropertyChangedEventManager.AddHandler(_port.Bus, OnPortPropertyChanged, "");
        }

        public override void Cleanup()
        {
            base.Cleanup();
            PropertyChangedEventManager.RemoveHandler(_port.Bus, OnPortPropertyChanged, "");
        }

        protected override void DisplayNameConvert()
        {
            DisplayName = LanguageManager.GetInstance().ConvertSpecifier(Name) + $" I/O, {_port.Bus.Size - 2} Modules";
        }

        private void OnPortPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Size")
            {
                DisplayNameConvert();
            }
        }
    }
}
