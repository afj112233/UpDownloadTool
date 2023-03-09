using System;
using System.Windows;
using ICSStudio.DeviceProperties;
using ICSStudio.DeviceProperties.Adapters;
using ICSStudio.DeviceProperties.AnalogIOs;
using ICSStudio.DeviceProperties.DiscreteIOs;
using ICSStudio.DeviceProperties.Generic;
using ICSStudio.DeviceProperties.ServoDrives;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.UIServicesPackage.SelectModuleType
{
    /// <summary>
    /// NewDeviceModuleDialog.xaml 的交互逻辑
    /// </summary>
    public partial class NewDeviceModuleDialog
    {
        public NewDeviceModuleDialog(IController controller, IDeviceModule deviceModule)
        {
            InitializeComponent();

            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
            WeakEventManager<NewDeviceModuleDialog,EventArgs>.AddHandler(this, "Closed",NewDeviceModuleDialog_Closed);

            if (deviceModule is CIPMotionDrive)
            {
                var control = new DevicePropertiesControl();
                var viewModel = new ServoDrivesViewModel(
                    controller, deviceModule, true);

                ContentPresenter.Content = control;
                DataContext = viewModel;
            }

            if (deviceModule is CommunicationsAdapter)
            {
                var control = new DevicePropertiesControl();
                var viewModel = new DIOEnetAdapterViewModel(controller, deviceModule, true);

                ContentPresenter.Content = control;
                DataContext = viewModel;
            }

            if (deviceModule is DiscreteIO)
            {
                var control = new DevicePropertiesControl();
                var viewModel = new DIODiscreteViewModel(controller, deviceModule, true);

                ContentPresenter.Content = control;
                DataContext = viewModel;
            }

            if (deviceModule is AnalogIO)
            {
                var control = new DevicePropertiesControl();
                var viewModel = new DIOAnalogViewModel(controller, deviceModule, true);

                ContentPresenter.Content = control;
                DataContext = viewModel;
            }

            if (deviceModule is GeneralEthernet)
            {
                var control = new DevicePropertiesControl();
                var viewModel = new GenericEnetViewModel(controller, deviceModule, true);

                ContentPresenter.Content = control;
                DataContext = viewModel;
            }

            //TODO(gjc): add other device
        }

        private void NewDeviceModuleDialog_Closed(object sender, EventArgs e)
        {
            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
            WeakEventManager<NewDeviceModuleDialog, EventArgs>.RemoveHandler(this, "Closed", NewDeviceModuleDialog_Closed);
        }

        private void LanguageChanged(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }
    }
}
