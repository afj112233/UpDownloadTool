using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using ICSStudio.Components.Automation.Peers;

namespace ICSStudio.Components.Controls
{
    public class WrappingTextBox : TextBox
    {
        public static readonly DependencyProperty WrapTextProperty = DependencyProperty.Register("WrapText",
            typeof(string), typeof(WrappingTextBox),
            new FrameworkPropertyMetadata(string.Empty,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnWrapTextChanged));

        public static readonly DependencyProperty CharactersPerLineProperty =
            DependencyProperty.Register("CharactersPerLine", typeof(int), typeof(WrappingTextBox),
                new UIPropertyMetadata(0,
                    OnCharactersPerLineChanged));

        public static readonly RoutedEvent WrapTextChangedEvent = EventManager.RegisterRoutedEvent("WrapTextChanged",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(WrappingTextBox));

        private static readonly Regex SoftLineBreaks =
            new Regex("\\r([^\\n])", RegexOptions.Compiled | RegexOptions.Singleline);

        private bool _insideWrappingText;

        public string WrapText
        {
            get { return (string) GetValue(WrapTextProperty); }
            set { SetValue(WrapTextProperty, value); }
        }

        public int CharactersPerLine
        {
            get { return (int) GetValue(CharactersPerLineProperty); }
            set { SetValue(CharactersPerLineProperty, value); }
        }

        public event RoutedEventHandler WrapTextChanged
        {
            add { AddHandler(WrapTextChangedEvent, value); }
            remove { RemoveHandler(WrapTextChangedEvent, value); }
        }

        public WrappingTextBox()
        {
            CommandManager.AddPreviewExecutedHandler(this,
                HandlePreviewExecutedHandler);
            Loaded += WrappingTextBox_Loaded;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new WrappingTextBoxAutomationPeer(this);
        }

        protected void HandlePreviewExecutedHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (e == null)
                return;
            RoutedUICommand command = e.Command as RoutedUICommand;
            if (command == null)
                return;
            if (command.Text == "Backspace" && CaretIndex > 0 && SelectionLength == 0)
            {
                if (!(Text.Substring(CaretIndex - 1, 1) == "\r"))
                    return;
                SelectionStart = CaretIndex = CaretIndex - 1;
            }
            else
            {
                if (!(command.Text == "Delete") || Text.Length <= CaretIndex ||
                    (SelectionLength != 0 || !(Text.Substring(CaretIndex, 1) == "\r")) ||
                    !(Text.Substring(CaretIndex, 2) != Environment.NewLine))
                    return;
                SelectionStart = CaretIndex = CaretIndex + 1;
            }
        }

        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            base.OnTextInput(e);
            WrappingText();
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            if (_insideWrappingText)
                return;
            WrappingText();
        }

        private static void OnWrapTextChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            WrappingTextBox wrappingTextBox = source as WrappingTextBox;
            if (wrappingTextBox == null)
                return;
            if (!wrappingTextBox._insideWrappingText)
                wrappingTextBox.Text = wrappingTextBox.WrapText;
            wrappingTextBox.RaiseWrapTextChangedEvent();
        }

        private static void OnCharactersPerLineChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            WrappingTextBox wrappingTextBox = source as WrappingTextBox;
            wrappingTextBox?.WrappingText();
        }

        private void WrappingTextBox_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void RaiseWrapTextChangedEvent()
        {
            RaiseEvent(new RoutedEventArgs(WrapTextChangedEvent));
        }

        private void WrappingText()
        {
            if (CharactersPerLine == 0 || string.IsNullOrEmpty(Text))
            {
                _insideWrappingText = true;
                WrapText = Text;
                _insideWrappingText = false;
            }
            else
            {
                int caretIndex = CaretIndex;
                int selectionLength = SelectionLength;
                caretIndex -= SoftLineBreaks.Matches(Text)
                    .OfType<Match>().Count(match => match.Index < caretIndex);
                if (selectionLength > 0)
                    selectionLength -= SoftLineBreaks.Matches(Text).OfType<Match>().Where(
                        match =>
                        {
                            if (match.Index >= caretIndex)
                                return match.Index < caretIndex + selectionLength;
                            return false;
                        }).Count();
                string input = SoftLineBreaks.Replace(Text, "$1", -1, 0);
                int insertedLineBreaks = 0;
                int insertedLineBreaksWithinSelection = 0;
                string str1 = Convert.ToString(CharactersPerLine, CultureInfo.InvariantCulture);
                string str2 =
                    new Regex("(.{1," + str1 + "})(\\r\\n|[\\s]|$)|(.{1," + str1 + "})", RegexOptions.Multiline)
                        .Replace(input, match =>
                        {
                            Match match1 = match.NextMatch();
                            if (match.Value.EndsWith(Environment.NewLine, StringComparison.CurrentCulture) ||
                                string.IsNullOrEmpty(match.NextMatch().Value))
                                return match.Value;
                            if (match1.Index <= caretIndex)
                                ++insertedLineBreaks;
                            else if (match1.Index < caretIndex + selectionLength)
                                ++insertedLineBreaksWithinSelection;
                            return match.Value + "\r";
                        });
                _insideWrappingText = true;
                Text = str2;
                SelectionStart = CaretIndex = caretIndex + insertedLineBreaks;
                SelectionLength = selectionLength + insertedLineBreaksWithinSelection;
                WrapText = input;
                _insideWrappingText = false;
            }
        }
    }
}
