using System;
using System.Windows;
using ICSStudio.DeviceProperties.Adapters;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties.ModuleDefinition
{
    /// <summary>
    /// DIOEnetAdapterDefinitionDialog.xaml 的交互逻辑
    /// </summary>
    public partial class DIOEnetAdapterDefinitionDialog
    {
        private readonly DIOEnetAdapterDefinitionViewModel _viewModel;

        public DIOEnetAdapterDefinitionDialog(ModifiedDIOEnetAdapter modifiedAdapter)
        {
            InitializeComponent();

            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
            WeakEventManager<DIOEnetAdapterDefinitionDialog, EventArgs>.AddHandler(this,"Closed", DIOEnetAdapterDefinitionDialog_Closed);

            _viewModel = new DIOEnetAdapterDefinitionViewModel(modifiedAdapter);
            DataContext = _viewModel;
        }

        private void DIOEnetAdapterDefinitionDialog_Closed(object sender, EventArgs e)
        {
            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        public string Series => _viewModel.Series;
        public int Major => _viewModel.Major;
        public int Minor => _viewModel.Minor;
        public ElectronicKeyingType EKey => _viewModel.EKey;
        public uint ConnectionConfigID => _viewModel.ConnectionConfigID;
        public int ChassisSize => _viewModel.ChassisSize;
    }
}
