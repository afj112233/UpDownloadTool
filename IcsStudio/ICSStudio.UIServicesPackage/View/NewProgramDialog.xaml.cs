using System;
using System.Windows;
using System.Windows.Media;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.UIServicesPackage.ViewModel;

namespace ICSStudio.UIServicesPackage.View
{
    /// <summary>
    /// NewProgram.xaml 的交互逻辑
    /// </summary>
    public partial class NewProgramDialog
    {
        public NewProgramDialog(ProgramType type,ITask task)
        {
            InitializeComponent();
            DataContext = new NewProgramDialogViewModel(type,task);
            Width = 440;
            Height = 290;
            if (type == ProgramType.Sequence)
            {
                Height = 380;
            }

            if (type == ProgramType.Phase)
            {
                Height = 260;
            }

            UseAsFolder.Foreground = UseAsFolder.IsEnabled ? Brushes.Black : Brushes.LightGray;

            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        private void UIElement_OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UseAsFolder.Foreground = UseAsFolder.IsEnabled ? Brushes.Black : Brushes.LightGray;
        }
    }
}
