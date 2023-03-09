using System;
using System.Windows;
using ICSStudio.DeviceProfiles.MotionDrive2;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties.ModuleDefinition
{
    /// <summary>
    /// ServoDriveDefinitionDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ServoDriveDefinitionDialog
    {
        private readonly ServoDriveDefinitionViewModel _viewModel;

        public ServoDriveDefinitionDialog(MotionDriveProfiles profiles, ServoDriveDefinition definition)
        {
            InitializeComponent();

            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
            WeakEventManager<ServoDriveDefinitionDialog,EventArgs>.AddHandler(this, "Closed",ServoDriveDefinitionDialog_Closed);

            _viewModel = new ServoDriveDefinitionViewModel(profiles, definition);
            DataContext = _viewModel;
        }

        private void ServoDriveDefinitionDialog_Closed(object sender, EventArgs e)
        {
            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        public int Major => _viewModel.Major;

        public int Minor => _viewModel.Minor;

        public ElectronicKeyingType EKey => _viewModel.EKey;

        public int PowerStructureID => _viewModel.PowerStructureID;

        public bool VerifyPowerRating => _viewModel.VerifyPowerRating;

        public ConnectionType Connection => _viewModel.Connection;
    }
}
