using System;
using System.Windows;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.DeviceProperties.DiscreteIOs;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties.ModuleDefinition
{
    /// <summary>
    /// DIODiscreteDefinitionDialog.xaml 的交互逻辑
    /// </summary>
    public partial class DIOModuleDefinitionDialog
    {
        private readonly DIOModuleDefinitionViewModel _viewModel;

        public DIOModuleDefinitionDialog(ModifiedDIOModule modifiedDIOModule)
        {
            InitializeComponent();

            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
            WeakEventManager<DIOModuleDefinitionDialog,EventArgs>.AddHandler(this,"Closed", DIOModuleDefinitionDialog_Closed);

            _viewModel = new DIOModuleDefinitionViewModel(modifiedDIOModule);
            DataContext = _viewModel;
        }

        private void DIOModuleDefinitionDialog_Closed(object sender, EventArgs e)
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
    }
}
