using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ICSStudio.AvalonEdit.Document;
using ICSStudio.AvalonEdit.Rendering;

namespace ICSStudio.AvalonEdit.Variable
{
    internal class InLineTextBox : TextBox
    {
        private bool _isInMonitor;
        public InLineTextBox(bool isEdit)
        {
            if (!isEdit)
            {
                MouseEnter += InLineBox_MouseEnter;
                MouseLeave += InLineBox_MouseLeave;
                TextAlignment = TextAlignment.Right;
            }
            else
            {
                TextAlignment = TextAlignment.Left;
            }
        }
        
        private void InLineBox_MouseLeave(object sender, MouseEventArgs e)
        {
            RecoverState(false);
        }

        internal void RecoverState(bool isRaiseSizeChanged)
        {
            if (IsEnabled)
            {
                Background = new SolidColorBrush(Color.FromArgb(95, 204, 255, 204));
                var variable = DataContext as VariableInfo;
                if (variable != null&&!isRaiseSizeChanged)
                {
                    CanRaiseSizeChanged = false;
                }
                BorderThickness = new Thickness(0);
            }
        }

        private void InLineBox_MouseEnter(object sender, MouseEventArgs e)
        {
            if (IsEnabled)
            {
                Background = new SolidColorBrush(Colors.White);
                var variable = DataContext as VariableInfo;
                if (variable != null)
                {
                    CanRaiseSizeChanged = false;
                }
                BorderThickness = new Thickness(1);
            }
        }

        public bool CanRaiseSizeChanged { get; internal set; } = true;

        public InLineTextBox TwinsBox { set; get; }

        public static readonly DependencyProperty LineOffsetProperty = DependencyProperty.Register(
            "LineOffset", typeof(double), typeof(InLineTextBox), new PropertyMetadata(default(double)));

        public double LineOffset
        {
            get { return (double) GetValue(LineOffsetProperty); }
            set { SetValue(LineOffsetProperty, value); }
        }

        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register(
            "Offset", typeof(int), typeof(InLineTextBox), new PropertyMetadata(default(int)));

        public int Offset
        {
            get { return (int) GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        public static readonly DependencyProperty TextPositionProperty = DependencyProperty.Register(
            "TextPosition", typeof(TextLocation), typeof(InLineTextBox), new PropertyMetadata(default(TextLocation)));

        public TextLocation TextPosition
        {
            get { return (TextLocation) GetValue(TextPositionProperty); }
            set { SetValue(TextPositionProperty, value); }
        }

        public double BaseOffset { set; get; }

        public double VariantWidth { set; get; }

        public void OnMonitor()
        {
            lock (_syncRoot)
            {
                if (_isInMonitor) return;
                KeyUp += InLineBox_KeyUp;
                _isInMonitor = true;
            }
        }

        private TextBlock _textBlock = null;

        public TextBlock CreatePlaceHolder()
        {
            if (_textBlock == null)
            {
                _textBlock = new TextBlock();
                _textBlock.Height = 1;
            }

            return _textBlock;
        }

        public void OffMonitor()
        {
            lock (_syncRoot)
            {
                if (!_isInMonitor) return;
                KeyUp -= InLineBox_KeyUp;
                _isInMonitor = false;
            }
        }

        private readonly object _syncRoot = new object();
        private bool _isAbort = false;

        public bool IsAbort
        {
            set
            {
                if (_isAbort) return;
                _isAbort = value;
                if (value)
                {
                    VariableInfo variableInfo = DataContext as VariableInfo;
                    if (variableInfo != null)
                    {
                        variableInfo?.StopTimer();
                        variableInfo.IsCorrect = true;
                    }
                }
            }
            get { return _isAbort; }
        }

        private void InLineBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                var variableInfo = (VariableInfo) ((TextBox) sender).DataContext;
                variableInfo.IsCorrect = true;
                variableInfo.SetInlineVisibility(Visibility.Collapsed, Name == "Top", null);
            }

            if (e.Key == Key.Enter)
            {
                var variableInfo = (VariableInfo) ((TextBox) sender).DataContext;

                if (variableInfo.GetInlineVisibility(Name == "Top") == Visibility.Visible)
                {
                    OffMonitor();
                    TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Previous);
                    ((TextBox) sender).MoveFocus(request);
                }
            }

            e.Handled = true;
        }
    }
}
