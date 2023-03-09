using System;
using System.Windows;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.UIServicesPackage.SelectModuleType
{
    /// <summary>
    /// SelectModuleTypeDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SelectModuleTypeDialog
    {
        public SelectModuleTypeDialog(
            IController controller,
            IDeviceModule parentModule,
            PortType portType)
        {
            InitializeComponent();

            SelectModuleTypeViewModel viewModel =
                new SelectModuleTypeViewModel(controller, parentModule, portType);
            DataContext = viewModel;

            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }
    }
}
