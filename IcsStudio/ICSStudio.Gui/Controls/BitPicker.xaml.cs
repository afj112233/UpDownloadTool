using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ICSStudio.Gui.Controls
{
    public enum DataType
    {
        SINT,
        INT,
        DINT
    }

    /// <summary>
    ///     BitPicker.xaml 的交互逻辑
    /// </summary>
    public partial class BitPicker
    {
        public static readonly DependencyProperty DataTypeProperty = DependencyProperty.Register("DataType",
            typeof(DataType), typeof(BitPicker), new UIPropertyMetadata(DataType.DINT));

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int),
            typeof(BitPicker), new UIPropertyMetadata(0));

        public static readonly DependencyProperty IsDirtyProperty = DependencyProperty.Register("IsDirty", typeof(bool),
            typeof(BitPicker), new UIPropertyMetadata(false));

        private string _oldValue = string.Empty;

        public BitPicker()
        {
            InitializeComponent();

            DataObject.AddPastingHandler(this, OnPasting);
        }

        public DataType DataType
        {
            get { return (DataType) GetValue(DataTypeProperty); }
            set { SetValue(DataTypeProperty, value); }
        }

        public int Value
        {
            get { return (int) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public bool IsDirty
        {
            get { return (bool) GetValue(IsDirtyProperty); }
            set { SetValue(IsDirtyProperty, value); }
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
                if (textBox.Text.Trim() != e.Text)
                    if (e.Text == "0" || e.Text == "1")
                    {
                        textBox.Text = e.Text;
                        IsDirty = true;
                    }

            OnPreviewTextInput(e);
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
                return;

            if (e.Key == Key.Delete) textBox.Text = string.Empty;

            if (e.Key == Key.Return)
            {
                SetTextBoxFocused(textBox);
                UpdateTextBox(textBox);
            }

            if (e.Key == Key.Left || e.Key == Key.Right)
            {
                var request = e.Key == Key.Right
                    ? new TraversalRequest(FocusNavigationDirection.Next)
                    : new TraversalRequest(FocusNavigationDirection.Previous);
                var focusedElement = Keyboard.FocusedElement as UIElement;
                focusedElement?.MoveFocus(request);
            }

            if (e.Key == Key.Up || e.Key == Key.Down)
            {
                var textBoxIndex = int.Parse(textBox.Tag.ToString(), CultureInfo.InvariantCulture);
                int nextTextBoxIndex;
                int maxBitIndex;

                switch (DataType)
                {
                    case DataType.SINT:
                        maxBitIndex = 7;
                        break;
                    case DataType.INT:
                        maxBitIndex = 15;
                        break;
                    case DataType.DINT:
                        maxBitIndex = 31;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (e.Key == Key.Down)
                {
                    nextTextBoxIndex = textBoxIndex + 8;

                    if (nextTextBoxIndex > maxBitIndex) nextTextBoxIndex -= maxBitIndex + 1;
                }
                else
                {
                    nextTextBoxIndex = textBoxIndex - 8;
                    if (nextTextBoxIndex < 0) nextTextBoxIndex += maxBitIndex + 1;
                }

                if (textBoxIndex != nextTextBoxIndex)
                {
                    textBox = GetTextBoxByIndex(this, 0, nextTextBoxIndex);
                    textBox?.Focus();
                    return;
                }
            }

            if (e.Key == Key.F2) textBox.SelectAll();

            if (e.Key == Key.Escape)
            {
                if (textBox.Text != _oldValue) textBox.Text = _oldValue;

                SetTextBoxFocused(textBox);
            }
        }

        private static void SetTextBoxFocused(TextBox textBox)
        {
            textBox.SelectionLength = 0;
            var parent = (FrameworkElement) textBox.Parent;

            while (parent != null)
                if (!parent.Focusable)
                {
                    parent = (FrameworkElement) parent.Parent;
                }
                else
                {
                    FocusManager.SetFocusedElement(FocusManager.GetFocusScope(textBox), parent);
                    textBox.Focus();
                    return;
                }

            FocusManager.SetFocusedElement(FocusManager.GetFocusScope(textBox), null);
            textBox.Focus();
        }

        // ReSharper disable once UnusedParameter.Local
        private TextBox GetTextBoxByIndex(object currentObject, int level, int index)
        {
            var current = currentObject as DependencyObject;

            if (current == null)
                return null;

            var enumerator = LogicalTreeHelper.GetChildren(current).GetEnumerator();

            try
            {
                while (true)
                    if (enumerator.MoveNext())
                    {
                        var current2 = enumerator.Current;
                        var textBox = current2 as TextBox;

                        if (textBox != null)
                        {
                            if (int.Parse(textBox.Tag.ToString(), CultureInfo.InvariantCulture) ==
                                index)
                                return textBox;
                        }
                        else
                        {
                            textBox = GetTextBoxByIndex(current2, level + 1, index);
                            if (textBox != null) return textBox;
                        }
                    }
                    else
                    {
                        return null;
                    }
            }
            finally
            {
                var disposable = enumerator as IDisposable;
                disposable?.Dispose();
            }
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.IsReadOnly = true;
                UpdateTextBox(textBox);
            }
        }

        private void UpdateTextBox(TextBox textBox)
        {
            if (textBox == null)
                return;

            if (!string.IsNullOrEmpty(textBox.Text.Trim()))
                if (IsDirty)
                {
                    textBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                    IsDirty = false;
                }
        }


        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.Changes.Count != 0)
            {
                var textBox = sender as TextBox;
                if (textBox != null)
                {
                    var index = int.Parse(textBox.Tag.ToString(), CultureInfo.InvariantCulture);

                    _oldValue = new BitArray(new[] {Value})[index] ? "1" : "0";

                    if (_oldValue != textBox.Text) IsDirty = true;
                }
            }
        }


        private void OnPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
                if (string.IsNullOrEmpty(textBox.Text.Trim()))
                    e.Handled = true;
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox != null)
            {
                textBox.IsReadOnly = false;
                if (!textBox.IsKeyboardFocusWithin) textBox.Focus();

                textBox.SelectAll();
                e.Handled = true;
            }
        }

        private void OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            e.CancelCommand();
        }
    }
}