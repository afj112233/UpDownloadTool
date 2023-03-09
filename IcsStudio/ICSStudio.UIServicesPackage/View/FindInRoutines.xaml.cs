using System;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.MultiLanguage;
using ICSStudio.UIServicesPackage.ViewModel;

namespace ICSStudio.UIServicesPackage.View
{
    /// <summary>
    /// FindInRoutines.xaml 的交互逻辑
    /// </summary>
    public partial class FindInRoutines : Window
    {
        public FindInRoutines(bool isShowReplace, bool isFindPrevious, string searchText, bool isShowFindDialog)
        {
            InitializeComponent();

            DataContext = new FindInRoutinesVM(Close,isFindPrevious,searchText, isShowFindDialog);
            if (isShowReplace)
            {
                ShowReplace();
            }

            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        public void Cleanup()
        {
            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            if (button.Content.Equals("查找范围>>") || button.Content.Equals("Find Within>>"))
            {
                this.Height = this.Height + 160;
                HiddenZeno.Visibility = Visibility.Visible;
            }
            else
            {
                this.Height = this.Height - 160;
                HiddenZeno.Visibility = Visibility.Collapsed;
            }
        }

        private void ButtonBase_OnClick2(object sender, RoutedEventArgs e)
        {
            ShowReplace();
        }

        private void ShowReplace()
        {
            Height = Height + 20;
            Width = Width + 15;
            Replace.Visibility = Visibility.Collapsed;
            ReplaceAllButton.Visibility = Visibility.Visible;
            ReplaceButton.Visibility = Visibility.Visible;
            Extra1.Visibility = Visibility.Visible;
            Extra2.Visibility = Visibility.Visible;
            Extra3.Visibility = Visibility.Visible;
            Extra4.Visibility = Visibility.Visible;
            Title = LanguageManager.GetInstance().ConvertSpecifier("Replace in Routines");
        }

        private void FindWhatTextBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            (sender as TextBox)?.Focus();
        }
    }
}
