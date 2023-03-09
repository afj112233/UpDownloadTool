using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ICSStudio.Components.Language;

namespace ICSStudio.Components.Controls
{
    /// <summary>
    /// TranslationViewerCtrl.xaml 的交互逻辑
    /// </summary>
    public partial class TranslationViewerCtrl
    {
        public static readonly DependencyProperty LanguagesProperty = DependencyProperty.Register(nameof(Languages),
            typeof(ObservableCollection<LanguageVM>), typeof(TranslationViewerCtrl),
            new PropertyMetadata(null));

        public static readonly DependencyProperty SortedLanguagesProperty =
            DependencyProperty.Register(nameof(SortedLanguages), typeof(ICollectionView), typeof(TranslationViewerCtrl),
                new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedLanguageProperty =
            DependencyProperty.Register(nameof(SelectedLanguage), typeof(string), typeof(TranslationViewerCtrl),
                new PropertyMetadata(null));


        public static readonly DependencyProperty PlacementTargetProperty = DependencyProperty.Register(
            nameof(PlacementTarget), typeof(UIElement), typeof(TranslationViewerCtrl),
            new PropertyMetadata(null,
                PlacementTargetPropertyChanged));

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(nameof(IsOpen),
            typeof(bool), typeof(TranslationViewerCtrl), new PropertyMetadata(false));


        private string _helpFileUrl;


        public TranslationViewerCtrl(string helpFileUrl)
        {
            _helpFileUrl = helpFileUrl;
            InitializeComponent();
            TranslationViewerPanel.IsKeyboardFocusWithinChanged +=
                TranslationViewer_IsKeyboardFocusWithinChanged;
        }

        public ObservableCollection<LanguageVM> Languages
        {
            get { return (ObservableCollection<LanguageVM>) GetValue(LanguagesProperty); }
            set
            {
                SetValue(LanguagesProperty, value);
                if (value == null)
                    return;
                SortedLanguages = CollectionViewSource.GetDefaultView(Languages);
                SortedLanguages.SortDescriptions.Clear();
                SortedLanguages.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            }
        }

        public ICollectionView SortedLanguages
        {
            get { return (ICollectionView) GetValue(SortedLanguagesProperty); }
            set { SetValue(SortedLanguagesProperty, value); }
        }

        private static void PlacementTargetPropertyChanged(
            DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            TranslationViewerCtrl translationViewerCtrl = sender as TranslationViewerCtrl;
            if (translationViewerCtrl == null)
                return;

            TextBox oldValue = e.OldValue as TextBox;

            if (oldValue != null)
            {
                oldValue.LostKeyboardFocus -=
                    translationViewerCtrl.TextBox_LostKeyboardFocus;
                oldValue.LayoutUpdated -= translationViewerCtrl.PlacementTargetMoved;
            }

            TextBox newValue = e.NewValue as TextBox;

            if (newValue == null)
                return;

            newValue.LostKeyboardFocus +=
                translationViewerCtrl.TextBox_LostKeyboardFocus;
            newValue.LayoutUpdated += translationViewerCtrl.PlacementTargetMoved;
            translationViewerCtrl.PasteButton.IsEnabled = newValue.IsEnabled && !newValue.IsReadOnly;
        }

        public UIElement PlacementTarget
        {
            get { return (UIElement) GetValue(PlacementTargetProperty); }
            set { SetValue(PlacementTargetProperty, value); }
        }

        public bool IsOpen
        {
            get { return (bool) GetValue(IsOpenProperty); }
            set
            {
                if (!value)
                {
                    BeginAnimation(OpacityProperty, null);
                    Opacity = 0.4;
                }

                SetValue(IsOpenProperty, value);
            }
        }

        public string SelectedLanguage
        {
            get { return (string) GetValue(SelectedLanguageProperty); }
            set
            {
                LanguageVM languageVm = Languages.FirstOrDefault(language => language.Name.Equals(value)) ??
                                        Languages.FirstOrDefault(
                                            language => language.IsDefault);
                if (languageVm != null)
                {
                    SetValue(SelectedLanguageProperty, languageVm.Name);
                }
                else
                {
                    if (!SortedLanguages.Cast<LanguageVM>().Any())
                        return;
                    SetValue(SelectedLanguageProperty,
                        SortedLanguages.Cast<LanguageVM>().First().Name);
                }
            }
        }

        private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (TranslationViewerPanel.IsKeyboardFocusWithin)
                return;
            IsOpen = false;
        }

        private void PlacementTargetMoved(object sender, EventArgs e)
        {
            TranslationViewer.PlacementRectangle =
                new Rect(new Point(new Random().NextDouble() / 1000.0, 0.0), new Size(1.0, 1.0));
        }

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            TextBox placementTarget = PlacementTarget as TextBox;
            if (placementTarget == null)
                return;

            if (!placementTarget.IsEnabled || placementTarget.IsReadOnly)
                return;

            placementTarget.Text = TranslatedText.Text;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            PlacementTarget.Focus();
            IsOpen = false;
        }

        private void TranslationViewer_IsKeyboardFocusWithinChanged(
            object sender,
            DependencyPropertyChangedEventArgs e)
        {
            if (TranslationViewerPanel.IsKeyboardFocusWithin || PlacementTarget.IsKeyboardFocused)
                return;
            IsOpen = false;
        }
    }


}
