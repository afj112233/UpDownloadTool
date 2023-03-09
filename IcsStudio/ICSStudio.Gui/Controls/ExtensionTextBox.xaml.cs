using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;


namespace ICSStudio.Gui.Controls
{
    public partial class ExtensionTextBox : UserControl
    {
        private Key _lastKey = Key.Delete;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text), typeof(string), typeof(ExtensionTextBox), 
            new FrameworkPropertyMetadata(default(string),FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public ExtensionTextBox()
        {
            InitializeComponent();
        }

        private void ShowExtensionTextBox_OnClick(object sender, RoutedEventArgs e)
        {
            var contextMenu = ((FrameworkElement)sender).ContextMenu;
            if (contextMenu == null) return;
            contextMenu.PlacementTarget = sender as Button;
            contextMenu.Margin = new Thickness(10, 0, 10, 0);
            contextMenu.Placement = PlacementMode.Bottom;
            contextMenu.HorizontalOffset = -332;
            contextMenu.IsOpen = !contextMenu.IsOpen;

            var extensionTextBox = contextMenu.Template.FindName("ExtensionTextBox", contextMenu) as TextBox;
            if (extensionTextBox == null) return;
            extensionTextBox.Text = Text;
            extensionTextBox.Focus();
            extensionTextBox.SelectAll();
        }

        private void ExtensionTextBox_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && _lastKey == Key.Enter) Focus();

            if (e.Key == Key.Enter)
                _lastKey = Key.Enter;
            else
                _lastKey = Key.Delete;
        }

        private void MainTextBox_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) Focus();
        }
        private void MainTextBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox != null)
            {
                textBox.Focus();

                if (textBox.IsFocused) textBox.SelectAll();
            }
        }

        private void ExtensionTextBox_OnPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var extensionTextBox = sender as TextBox;
            if (extensionTextBox == null) return;
            Text = extensionTextBox.Text.Trim();
        }

        private void TextBox_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            (sender as TextBox)?.SelectAll();
        }
    }
}
