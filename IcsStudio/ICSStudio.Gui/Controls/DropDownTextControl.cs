using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace ICSStudio.Gui.Controls
{
    public class DropDownTextControl : Control
    {
        private bool _isDropDownOpen = true;
        private Popup _popup;
        private TextBox _textBox;

        static DropDownTextControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownTextControl),
                new FrameworkPropertyMetadata(typeof(DropDownTextControl)));

            EventManager.RegisterClassHandler(typeof(DropDownTextControl), Mouse.LostMouseCaptureEvent,
                new MouseEventHandler(OnLostMouseCapture));
            EventManager.RegisterClassHandler(typeof(DropDownTextControl), Mouse.MouseDownEvent,
                new MouseButtonEventHandler(OnMouseDown), true);
            EventManager.RegisterClassHandler(typeof(DropDownTextControl), Mouse.PreviewMouseDownEvent,
                new MouseButtonEventHandler(OnPreviewMouseDown));
            EventManager.RegisterClassHandler(typeof(DropDownTextControl), GotFocusEvent,
                new RoutedEventHandler(OnGotFocus));
        }

        public DropDownTextControl()
        {
            Content = this;
            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (_popup != null)
            {
                _popup.Opened -= PopupOnOpened;
                _popup.Closed -= PopupOnClosed;
            }
        }

        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            var dropDownTextControl = (DropDownTextControl) sender;
            dropDownTextControl?.Dispatcher?.BeginInvoke(DispatcherPriority.Input, (Action) (() =>
            {
                if (ReferenceEquals(e.OriginalSource, dropDownTextControl))
                    if (!dropDownTextControl._textBox.IsFocused)
                        dropDownTextControl._textBox.Focus();

                if (ReferenceEquals(e.OriginalSource, dropDownTextControl._textBox))
                    if (dropDownTextControl._textBox.SelectionLength == 0)
                        dropDownTextControl._textBox.SelectAll();
            }));
        }

        private static void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var dropDownTextControl = (DropDownTextControl) sender;
            var originalSource = e.OriginalSource as Visual;
            Visual textVisual = dropDownTextControl._textBox;

            if (originalSource != null && textVisual != null && textVisual.IsAncestorOf(originalSource))
            {
                if (dropDownTextControl.IsDropDownOpen)
                {
                    dropDownTextControl.CloseDropDown();
                    return;
                }

                if (!dropDownTextControl.IsKeyboardFocusWithin)
                {
                    dropDownTextControl.Focus();
                    e.Handled = true;
                }
            }

            if (ReferenceEquals(Mouse.Captured, dropDownTextControl)
                && ReferenceEquals(e.OriginalSource, dropDownTextControl))
                dropDownTextControl.CloseDropDown();
        }

        private static void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var dropDownTextControl = (DropDownTextControl) sender;

            if (!dropDownTextControl.IsKeyboardFocusWithin) dropDownTextControl.Focus();

            e.Handled = true;

            if (ReferenceEquals(Mouse.Captured, dropDownTextControl))
                if (ReferenceEquals(e.OriginalSource, dropDownTextControl))
                    dropDownTextControl.CloseDropDown();
        }

        private static void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            var dropDownTextControl = (DropDownTextControl) sender;

            if (!ReferenceEquals(Mouse.Captured, dropDownTextControl))
            {
                if (!ReferenceEquals(e.OriginalSource, dropDownTextControl))
                {
                    if (IsDescendant(dropDownTextControl, e.OriginalSource as DependencyObject))
                    {
                        if (dropDownTextControl.IsDropDownOpen && Mouse.Captured == null)
                        {
                            Mouse.Capture(dropDownTextControl, CaptureMode.SubTree);
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        dropDownTextControl.CloseDropDown();
                    }
                }
                else
                {
                    if (Mouse.Captured != null)
                    {
                        if (!IsDescendant(dropDownTextControl, e.OriginalSource as DependencyObject))
                            dropDownTextControl.CloseDropDown();
                    }
                    else
                    {
                        dropDownTextControl.CloseDropDown();
                    }
                }
            }
        }

        private void CloseDropDown()
        {
            if (IsDropDownOpen)
                IsDropDownOpen = false;
        }

        internal static bool IsDescendant(DependencyObject reference, DependencyObject node)
        {
            var method =
                typeof(MenuBase).GetMethod("IsDescendant", BindingFlags.Static | BindingFlags.NonPublic);

            object[] parameters =
            {
                reference,
                node
            };

            if (method != null) return (bool) method.Invoke(null, parameters);

            return false;
        }

        public override void OnApplyTemplate()
        {
            if (_popup != null)
            {
                _popup.Opened -= PopupOnOpened;
                _popup.Closed -= PopupOnClosed;
            }

            _textBox = GetTemplateChild("PART_TextBox") as TextBox;
            _popup = GetTemplateChild("PART_Popup") as Popup;

            if (_popup != null)
            {
                _popup.Opened += PopupOnOpened;
                _popup.Closed += PopupOnClosed;
            }
        }

        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusWithinChanged(e);

            if (IsDropDownOpen
                && !IsKeyboardFocusWithin
                && !(Keyboard.FocusedElement is DependencyObject))
            {
                CloseDropDown();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            bool flag = false;

            Key key = e.Key == Key.System ? e.SystemKey : e.Key;

            if (key <= Key.Down)
            {
                if (key != Key.Return)
                {
                    if (key != Key.Escape)
                    {
                        if (key == Key.Up || key == Key.Down)
                        {
                            flag = true;
                            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                            {
                                CloseDropDown();
                            }

                        }
                    }
                    else
                    {
                        if (IsDropDownOpen)
                        {
                            _isDropDownOpen = false;
                            SetCurrentValue(DropDownTextControl.IsDropDownOpenProperty, false);
                            flag = true;
                        }
                    }
                }

            }

            if (flag)
            {
                e.Handled = true;
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (ReferenceEquals(e.OriginalSource, _textBox))
            {
                OnKeyDown(e);
            }
        }

        private void PopupOnClosed(object sender, EventArgs e)
        {
            // do nothing
        }

        private void PopupOnOpened(object sender, EventArgs e)
        {
            var textBox = GetFirstChild<TextBox>(_popup.FindName("PopupContent") as ContentPresenter);
            if (textBox != null) Keyboard.Focus(textBox);
        }

        private T GetFirstChild<T>(DependencyObject contentPresenter) where T : DependencyObject
        {
            for (var childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(contentPresenter); childIndex++)
            {
                var child = VisualTreeHelper.GetChild(contentPresenter, childIndex);

                var result = child as T;
                if (result != null)
                    return result;

                result = GetFirstChild<T>(child);
                if (result != null)
                    return result;
            }

            return default(T);
        }

        #region DependencyProperty

        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen",
            typeof(bool), typeof(DropDownTextControl),
            new FrameworkPropertyMetadata(false,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnIsDropDownOpenChanged));

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dropDownTextControl = d as DropDownTextControl;
            if (dropDownTextControl != null)
            {
                if ((bool) e.NewValue)
                {
                    dropDownTextControl._isDropDownOpen = true;
                    Mouse.Capture(dropDownTextControl, CaptureMode.SubTree);

                    dropDownTextControl._textBox?.SelectAll();

                    dropDownTextControl.DropDownText = dropDownTextControl.Text;
                }
                else
                {
                    if (dropDownTextControl.IsKeyboardFocusWithin
                        && dropDownTextControl._textBox != null
                        && !dropDownTextControl._textBox.IsKeyboardFocusWithin)
                    {
                        dropDownTextControl.Focus();
                    }

                    if (ReferenceEquals(Mouse.Captured, dropDownTextControl))
                    {
                        Mouse.Capture(null);
                    }

                    if (dropDownTextControl._isDropDownOpen)
                    {
                        dropDownTextControl.Text = dropDownTextControl.DropDownText;
                    }

                }

                dropDownTextControl.CoerceValue(ToolTipService.IsEnabledProperty);
            }
        }


        public bool IsDropDownOpen
        {
            get { return (bool) GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string),
            typeof(DropDownTextControl),
            new FrameworkPropertyMetadata(string.Empty,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }


        public static readonly DependencyProperty MaxTextLengthProperty = DependencyProperty.Register("MaxTextLength",
            typeof(int), typeof(DropDownTextControl),
            new FrameworkPropertyMetadata(0,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public int MaxTextLength
        {
            get { return (int) GetValue(MaxTextLengthProperty); }
            set { SetValue(MaxTextLengthProperty, value); }
        }

        public static readonly DependencyProperty DropDownTextProperty = DependencyProperty.Register("DropDownText",
            typeof(string), typeof(DropDownTextControl),
            new UIPropertyMetadata(string.Empty));

        public string DropDownText
        {
            get { return (string) GetValue(DropDownTextProperty); }
            set { SetValue(DropDownTextProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty =
            ContentControl.ContentProperty.AddOwner(typeof(DropDownTextControl));

        public object Content
        {
            get { return (string) GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly",
            typeof(bool), typeof(DropDownTextControl), new UIPropertyMetadata(false));

        public bool IsReadOnly
        {
            get { return (bool) GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }


        public static readonly DependencyProperty DropDownTemplateProperty
            = DependencyProperty.Register("DropDownTemplate", typeof(DataTemplate), typeof(DropDownTextControl));

        public DataTemplate DropDownTemplate
        {
            get { return (DataTemplate) GetValue(DropDownTemplateProperty); }
            set { SetValue(DropDownTemplateProperty, value); }
        }

        #endregion
    }
}