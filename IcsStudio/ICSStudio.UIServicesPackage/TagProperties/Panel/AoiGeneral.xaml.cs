using ICSStudio.MultiLanguage;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ICSStudio.UIServicesPackage.TagProperties.Panel
{
    /// <summary>
    /// AoiGeneral.xaml 的交互逻辑
    /// </summary>
    public partial class AoiGeneral
    {
        public AoiGeneral()
        {
            InitializeComponent();
            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(NameBox);
            if (NameBox.IsFocused)
            {
                NameBox.SelectAll();
            }
        }

        private void TextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                var textBox = ((TextBox)sender);
                var caretIndex = textBox.CaretIndex;
                textBox.Text = ((TextBox)sender).Text.Insert(caretIndex, "_");
                textBox.CaretIndex = caretIndex + 1;
                e.Handled = true;
            }
        }
    }
}
