
using ICSStudio.MultiLanguage;
using System;
using System.Windows;

namespace ICSStudio.Dialogs.ConfigDialogs
{
    /// <summary>
    /// GraphProperties.xaml 的交互逻辑
    /// </summary>
    public partial class GraphProperties
    {
        public GraphProperties(CamEditorViewModel camEditorViewModel)
        {
            InitializeComponent();
            DataContext = new GraphPropertiesViewModel(camEditorViewModel);
            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }
    }
}
