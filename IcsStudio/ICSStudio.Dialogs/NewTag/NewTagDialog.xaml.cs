using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using System;

namespace ICSStudio.Dialogs.NewTag
{
    /// <summary>
    /// NewTagDialog.xaml 的交互逻辑
    /// </summary>
    public partial class NewTagDialog
    {
        private readonly NewTagViewModel _viewModel;

        public NewTagDialog(NewTagViewModel viewModel)
        {
            InitializeComponent();
            viewModel.NameFilterPopup = NameFilterPopup;
            _viewModel = viewModel;
            DataContext = _viewModel;
            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        public ITag NewTag => _viewModel.NewTag;

        private void TextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                var textBox = ((TextBox) sender);
                var caretIndex = textBox.CaretIndex;
                textBox.Text = ((TextBox) sender).Text.Insert(caretIndex, "_");
                textBox.CaretIndex = caretIndex + 1;
                e.Handled = true;
            }
        }

    }
}
