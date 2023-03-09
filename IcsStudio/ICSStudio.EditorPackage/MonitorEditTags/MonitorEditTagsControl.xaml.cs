using System;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.MultiLanguage;

namespace ICSStudio.EditorPackage.MonitorEditTags
{
    /// <summary>
    ///     MonitorEditTagsControl.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorEditTagsControl
    {
        public MonitorEditTagsControl()
        {
            InitializeComponent();
            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        public void Refresh()
        {
            if (TabControl.SelectedIndex == 0)
                MonitorTagsControl.Refresh();
            else if (TabControl.SelectedIndex == 1)
                EditTagsControl.Refresh();
        }

        public void Refresh(int index)
        {
            switch (index)
            {
                case 0:
                    MonitorTagsControl.Refresh();
                    break;
                case 1:
                    EditTagsControl.Refresh();
                    break;
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.ContextMenu != null)
            {
                if (button.ContextMenu.DataContext == null)
                    button.ContextMenu.DataContext = DataContext;
                button.ContextMenu.IsOpen = true;
            }
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ((MenuItem) sender).IsChecked = true;
            e.Handled = true;
        }

        private void ContextMenu_OnOpened(object sender, RoutedEventArgs e)
        {
            var contextMenu = sender as ContextMenu;
            if (contextMenu != null)
            {
                LanguageManager.GetInstance().SetLanguage(contextMenu);
            }
        }
    }
}