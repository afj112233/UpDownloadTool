using System;
using System.Windows;
using ICSStudio.MultiLanguage;

namespace ICSStudio.DeviceProperties.AdvancedUserLimits
{
    /// <summary>
    /// AdvancedUserLimitsDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AdvancedUserLimitsDialog
    {
        private readonly AdvancedUserLimitsViewModel _viewModel;

        public AdvancedUserLimitsDialog(AdvancedUserLimits advancedUserLimits)
        {
            InitializeComponent();

            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
            WeakEventManager<AdvancedUserLimitsDialog, EventArgs>.AddHandler(this, "Closed", AdvancedUserLimitsDialog_Closed);

            _viewModel = new AdvancedUserLimitsViewModel(advancedUserLimits);
            DataContext = _viewModel;
        }

        private void AdvancedUserLimitsDialog_Closed(object sender, EventArgs e)
        {
            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        public AdvancedUserLimits AdvancedUserLimits =>
            new AdvancedUserLimits()
            {
                BusRegulatorThermalOverloadUserLimit = _viewModel.BusRegulatorThermalOverloadUserLimit,
                BusUndervoltageUserLimit = _viewModel.BusUndervoltageUserLimit,
                ConverterThermalOverloadUserLimit = _viewModel.ConverterThermalOverloadUserLimit
            };
    }
}
