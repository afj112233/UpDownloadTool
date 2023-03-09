using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ICSStudio.Components.Controls;
using ICSStudio.Components.Language;
using ICSStudio.Gui.View;

namespace ICSStudio.Components.View
{
    public class TranslationViewer
    {
        public static readonly DependencyProperty LanguagesProperty = DependencyProperty.RegisterAttached("Languages",
            typeof(ObservableCollection<LanguageVM>), typeof(TranslationViewer),
            new UIPropertyMetadata(null,
                OnLanguagesChanged));

        public static readonly DependencyProperty LanguageCollectionProperty = DependencyProperty.RegisterAttached(
            "LanguageCollection", typeof(ILanguageCollection), typeof(TranslationViewer),
            new UIPropertyMetadata(null,
                OnLanguageCollectionChanged));

        public static readonly DependencyProperty IsDefaultTranslationProperty = DependencyProperty.RegisterAttached(
            "IsDefaultTranslation", typeof(bool), typeof(TranslationViewer),
            new FrameworkPropertyMetadata(false,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private static readonly string TranslationViewerPopupRootName = "TranslationViewerPanel";

        private static TranslationViewerCtrl _translationViewer;

        public static bool TranslationViewerEnabled { get; set; } = true;

        public static string HelpFileUrl { get; set; }

        public static string CurrentLanguage { get; set; }

        public static bool IsElementInAttachedTranslationViewer(
            DependencyObject depObject,
            UIElement editableControl)
        {
            if (editableControl == null)
                throw new ArgumentNullException(nameof(editableControl));
            if (depObject == null)
                return false;
            ObservableCollection<LanguageVM> observableCollection =
                editableControl.GetValue(LanguagesProperty) as ObservableCollection<LanguageVM>;
            ILanguageCollection languageCollection =
                editableControl.GetValue(LanguageCollectionProperty) as ILanguageCollection;
            return (observableCollection != null && observableCollection.Count != 0 || languageCollection != null &&
                       languageCollection.Languages != null && languageCollection.Languages.Count != 0) &&
                   (VisualTreeHelpers.FindVisualParentByName(depObject,
                        TranslationViewerPopupRootName) != null &&
                    TranslationViewerControl.PlacementTarget == editableControl &&
                    TranslationViewerControl.IsOpen);
        }

        private static void OnLanguagesChanged(
            DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = source as TextBox;

            if (textBox == null)
                throw new ArgumentException(@"The Language viewer is only supported for textboxes", nameof(source));

            if (e.OldValue == null && e.NewValue != null)
                textBox.GotKeyboardFocus +=
                    TextBox_GotKeyboardFocus;
            if (e.OldValue == null || e.NewValue != null)
                return;
            textBox.GotKeyboardFocus -=
                TextBox_GotKeyboardFocus;
        }

        private static void OnLanguageCollectionChanged(
            DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = source as TextBox;

            if (textBox == null)
                throw new ArgumentException(@"The Language viewer is only supported for textboxes", nameof(source));

            textBox.GotKeyboardFocus +=
                TextBox_GotKeyboardFocus;
        }

        private static void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!TranslationViewerEnabled)
                return;

            if (_translationViewer == null)
                _translationViewer = new TranslationViewerCtrl(HelpFileUrl);

            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (GetIsDefaultTranslation((DependencyObject) sender))
                {
                    textBox.PreviewTextInput += TextBox_PreviewTextInput;
                    textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
                    textBox.PreviewMouseDown += TextBox_PreviewMouseDown;
                }

                textBox.LostKeyboardFocus += TextBox_LostKeyboardFocus;
            }

            FrameworkElement frameworkElement = sender as FrameworkElement;
            if (_translationViewer.IsOpen || frameworkElement == null)
                return;

            if (_translationViewer.PlacementTarget != frameworkElement)
                _translationViewer.PlacementTarget = frameworkElement;
            ObservableCollection<LanguageVM> languages = GetLanguages(sender as DependencyObject);
            if (languages == null || languages.Count == 0)
                return;

            _translationViewer.Languages = languages;
            _translationViewer.SelectedLanguage = CurrentLanguage;
            _translationViewer.IsOpen = true;
        }

        public static TranslationViewerCtrl TranslationViewerControl => _translationViewer;

        public static bool GetIsDefaultTranslation(DependencyObject obj)
        {
            return obj != null && (bool) obj.GetValue(IsDefaultTranslationProperty);
        }

        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ClearDefaultTranslation(sender as TextBox);
        }

        private static void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ClearDefaultTranslation(sender as TextBox);
        }

        private static void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ClearDefaultTranslation(sender as TextBox);
        }

        private static void TextBox_LostKeyboardFocus(object sender, KeyboardEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox == null)
                return;

            if (string.IsNullOrEmpty(textBox.Text))
            {
                ObservableCollection<LanguageVM> languages = GetLanguages((DependencyObject) sender);
                LanguageVM languageVm =
                    languages?.SingleOrDefault(p => p.IsDefault);
                if (languageVm != null && languageVm.Name != CurrentLanguage)
                {
                    textBox.Text = languageVm.Translation;
                    SetIsDefaultTranslation(textBox, true);
                }
            }

            textBox.PreviewTextInput -= TextBox_PreviewTextInput;
            textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
            textBox.PreviewMouseDown -= TextBox_PreviewMouseDown;
            textBox.LostKeyboardFocus -=
                TextBox_LostKeyboardFocus;
        }

        public static ObservableCollection<LanguageVM> GetLanguages(
            DependencyObject obj)
        {
            if (obj == null)
                return null;

            ObservableCollection<LanguageVM> observableCollection =
                (ObservableCollection<LanguageVM>) obj.GetValue(LanguagesProperty);
            if (observableCollection == null)
            {
                ILanguageCollection languageCollection =
                    (ILanguageCollection) obj.GetValue(LanguageCollectionProperty);
                observableCollection = languageCollection != null
                    ? languageCollection.Languages
                    : new ObservableCollection<LanguageVM>();
            }

            return observableCollection;
        }

        private static void ClearDefaultTranslation(TextBox textBox)
        {
            if (textBox == null)
                return;

            textBox.PreviewTextInput -= TextBox_PreviewTextInput;
            textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
            textBox.PreviewMouseDown -= TextBox_PreviewMouseDown;
            if (!GetIsDefaultTranslation(textBox))
                return;

            textBox.Text = string.Empty;
        }

        public static void SetIsDefaultTranslation(DependencyObject obj, bool value)
        {
            obj?.SetValue(IsDefaultTranslationProperty, value);
        }
    }
}
