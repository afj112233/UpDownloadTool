using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using System.Windows.Threading;
using ICSStudio.AvalonEdit.Rendering;
using ICSStudio.Dialogs.BrowseString;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.AvalonEdit.Variable
{
    sealed class InlineDisplay : IDisposable
    {
        private VisualLine _visualLine;

        private bool _disposed;

        //private TextView _textView;
        public InlineDisplay(VisualLine visualLine)
        {
            _visualLine = visualLine;
        }

        public VisualLine VisualLine => _visualLine;
        private VisualInLineDrawingVisual visual;

        internal VisualInLineDrawingVisual Render(TextRunProperties textRunProperties, TextLine textLine,
            InLineLayer inLineLayer)
        {
            if (visual == null)
            {
                visual = new VisualInLineDrawingVisual(_visualLine, textRunProperties, textLine, inLineLayer);
                visual.Parent = this;
            }

            return visual;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _visualLine = null;
                visual?.Dispose();
                visual = null;
            }

            _disposed = true;
        }
    }

    sealed class VisualInLineDrawingVisual : Panel, IDisposable
    {
        public readonly VisualLine VisualLine;

        private bool _disposed;

        //public readonly double Height;
        internal bool IsAdded;
        private TextView _textView;
        // ReSharper disable once IdentifierTypo
        readonly List<InLineTextBox> _inlineBoxs = new List<InLineTextBox>();   
        // ReSharper disable once IdentifierTypo
        readonly List<InLineTextBox> _editInlineBoxs = new List<InLineTextBox>();
        private readonly InLineLayer _inLineLayer;
        public VisualInLineDrawingVisual(VisualLine visualLine, TextRunProperties textRunProperties, TextLine textLine,
            InLineLayer inLineLayer)
        {
            _inLineLayer = inLineLayer;
            inLineLayer.AddChild(this);
            
            this.VisualLine = visualLine;
            _textView = visualLine.TextView;
            Height = textLine.Height * 2;
            var lineNumber = VisualLine.FirstDocumentLine.LineNumber;
            //var infos = _textView.VariableInfos.Where(i => i.TextLocation.Line == lineNumber && !i.IsInstr);
            var infos = _textView.VariableInfos?.Where(i =>
                ((VariableInfo) i).TextLocation.Line == lineNumber && !i.IsInstr).ToList();
            if (infos == null) return;
            var tmp = new List<InLineTextBox>();
            foreach (VariableInfo info in infos)
            {
                if (!info.IsDisplay || info.IsInstr || info.IsRoutine || info.IsEnum || info.IsUnknown) continue;
                info.StartTimer();
                var inLineBox = info.GetInlineTextBox(_textView);
                
                inLineBox.IsReadOnly = true;
                var editInlineBox = info.GetEditTextBox(_textView);
                inLineBox.TwinsBox = editInlineBox;
                editInlineBox.TwinsBox = inLineBox; 
                (VisualTreeHelper.GetParent(inLineBox) as VisualInLineDrawingVisual)?.RemoveVisualChild(inLineBox);
                (VisualTreeHelper.GetParent(editInlineBox) as VisualInLineDrawingVisual)?.RemoveVisualChild(editInlineBox);
                AddVisualChild(inLineBox);
                AddVisualChild(editInlineBox);
                _editInlineBoxs.Add(editInlineBox);
                inLineBox.Height = Height / 2;
                editInlineBox.Height = Height / 2;
            
                try
                {
                    visualLine.AddPlaceHolder(info.TextLocation.Column, inLineBox, textRunProperties, textLine);
                }
                catch (Exception)
                {
                    //Controller.GetInstance().Log($"{info.Routine?.Name}--{info.Routine?.IsCompiling}\n{e.Message}");
                }

                if (!Controller.GetInstance().IsOnline)
                {
                    info.MinWidth = 0;
                }

                inLineBox.Loaded += InlineBox_Loaded;
                tmp.Add(inLineBox);
                WeakEventManager<TextBox, MouseButtonEventArgs>.AddHandler(inLineBox,
                    "PreviewMouseLeftButtonDown", InLineBox_MouseLeftButtonDown);

            }

            foreach (var inLineTextBox in tmp.OrderBy(l => l.LineOffset))
            {
                _inlineBoxs.Add(inLineTextBox);
            }

            inLineLayer.AddEvent(this);
            RenderTransform = new TranslateTransform(0, 0);
            RenderTransform.Freeze();
            Loaded += VisualInLineDrawingVisual_Loaded;
        }

        private void VisualInLineDrawingVisual_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= VisualInLineDrawingVisual_Loaded;
            foreach (var inlineBox in _inlineBoxs)
            {
                inlineBox.SizeChanged += InLineBox_SizeChanged;
            }
        }

        private void InlineBox_Loaded(object sender, RoutedEventArgs e)
        {
            var info = ((InLineTextBox)sender).DataContext as VariableInfo;
            if (info?.GetInlineVisibility(((TextBox)sender).Name == "Top") == Visibility.Visible)
                Dispatcher.InvokeAsync(() =>
                {
                    _textView.IsKeepEditInlineState = true;
                    var inline = ((InLineTextBox) sender).TwinsBox;
                    Keyboard.Focus(inline);
                    _textView.IsKeepEditInlineState = false;
                }, DispatcherPriority.ApplicationIdle);
        }

        private void InLineBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var inline = ((InLineTextBox) sender);
            if (!inline.CanRaiseSizeChanged)
            {
                inline.CanRaiseSizeChanged = true;
                return;
            }
            var info = inline.DataContext as VariableInfo;
            if (info == null)
            {
                return;
            }

            _textView.IsKeepEditInlineState = true;
            _textView.Redraw();
        }

        public new InlineDisplay Parent { get; internal set; }

        internal void EnsurePosition()
        {
            if (VisualLine.Elements == null) return;
            foreach (var inLineBox in _inlineBoxs)
            {
                if (inLineBox.DataContext == null) continue;
                var info = (VariableInfo) inLineBox.DataContext;
                try
                {
                    var inlineObject =
                        VisualLine.Elements.FirstOrDefault(e => (e as InlineObjectElement)?.Node == info);
                    Debug.Assert(inlineObject != null);
                    var count = 0;
                    if (inlineObject == null)
                    {
                        //fix
                    }
                    else
                    {
                        count = VisualLine.Elements.Count(v =>
                            v is InlineObjectElement && v.VisualColumn <= inlineObject.VisualColumn);
                    }

                    var dis = VisualLine.GetTextLine(info.TextLocation.Column + info.Name.Length - 1 + count)
                        .GetDistanceFromCharacterHit(
                            new CharacterHit(info.TextLocation.Column - 1 + count + info.Name.Length, 0));
                    dis -= inLineBox.DesiredSize.Width;
                    inLineBox.BaseOffset = dis;
                    inLineBox.TwinsBox.BaseOffset = dis;
                    inLineBox.RenderTransform = new TranslateTransform(dis, 0);
                    inLineBox.TwinsBox.RenderTransform = new TranslateTransform(dis, 0);
                }
                catch (Exception)
                {
                    //Controller.GetInstance().Log($"EnsurePosition():{info.Routine?.Name}--{info.Routine?.IsCompiling}\n{e.Message}");
                }
            }
        }
        
        private void InLineBox_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var textBox = (InLineTextBox) sender;
            var variableInfo = (VariableInfo) textBox.DataContext;
            if (variableInfo.DataType.FamilyType == FamilyType.StringFamily)
            {
                textBox.IsReadOnly = false;
                var popup = new Popup();
                var message = new ICSStudio.Dialogs.BrowseString.RichTextBoxExtend.Message(popup,
                        FormatOp.UnFormatSpecial(variableInfo.GetStringData()))
                    {ShowButton = false};
                var browse = new BrowseString(variableInfo.DataType, message);
                popup.Child = browse;
                _browseVariableInfo = variableInfo;
                popup.PlacementTarget = textBox;
                popup.Placement = PlacementMode.Relative;
                popup.VerticalOffset = textBox.VerticalOffset;
                popup.HorizontalOffset = textBox.HorizontalOffset;
                popup.Width = 400;
                popup.Height = 300;
                popup.StaysOpen = false;
                popup.Opened += Popup_Opened;
                popup.IsOpen = true;
                popup.Closed += Popup_Closed;
            }
            else
            {
                if ((_inLineLayer.LastTextBox?.DataContext as VariableInfo)?.IsCorrect ?? true)
                {
                    var info = _inLineLayer.LastTextBox?.DataContext as VariableInfo;
                    if (info != null && info != variableInfo)
                    {
                        info.SetInlineVisibility(Visibility.Collapsed, _inLineLayer.LastTextBox.Name == "Top",
                            _textView);
                    }
                    variableInfo.EditValue = variableInfo.Value;
                    variableInfo.SetInlineVisibility(Visibility.Visible,textBox.Name=="Top",_textView);
                    Dispatcher.InvokeAsync(() =>
                        {
                            var inline = textBox.TwinsBox;
                            inline.MinWidth = textBox.DesiredSize.Width;
                            inline.Focus();
                            inline.SelectionStart = 0;
                            inline.SelectionLength = inline.Text.Length;
                        },
                        DispatcherPriority.ApplicationIdle);
                }
            }
        }

        private void Popup_Opened(object sender, EventArgs e)
        {
            var popup = (Popup) sender;
            popup.Opened -= Popup_Opened;
            ((BrowseString)popup.Child ).ForceApplyTextAlignment();
            ((BrowseString)popup.Child).Focus();
        }

        private VariableInfo _browseVariableInfo;

        private void Popup_Closed(object sender, EventArgs e)
        {
            var browseString = ((BrowseString) ((Popup) sender).Child);
            if (!browseString.IsError && !_browseVariableInfo.Value.Equals($"'{browseString.Text}'") &&
                !browseString.Message.IsClose)
            {
                var str = FormatOp.FormatSpecial(browseString.Text).Trim();
                _browseVariableInfo.SetSpecialStringField(str.Length, str);
            }
        }

        public void SetInlinePosition(TranslateTransform transform)
        {

            double last = double.NaN;

            foreach (var inlineBox in _inlineBoxs)
            {
                var left = inlineBox.BaseOffset - transform.X;
                if (!double.IsNaN(last) && left < last)
                    left = last;

                //inlineBox.Margin = new Thickness(left, 0, 0, 0);
                inlineBox.RenderTransform = new TranslateTransform(left, 0);
                inlineBox.RenderTransform.Freeze();
                inlineBox.TwinsBox.RenderTransform = new TranslateTransform(left, 0);
                inlineBox.TwinsBox.RenderTransform.Freeze();
                if (inlineBox.DesiredSize.Width > 0)
                    last = left + inlineBox.ActualWidth;
            }
        }

        public void AddGetFocusEvent(EventHandler<RoutedEventArgs> action)
        {
            foreach (var inlineBox in _inlineBoxs)
            {
                WeakEventManager<InLineTextBox, RoutedEventArgs>.AddHandler(inlineBox.TwinsBox, "GotFocus", action);
            }
        }

        public void AddLostFocusEvent(EventHandler<RoutedEventArgs> action)
        {
            foreach (var inlineBox in _inlineBoxs)
            {
                WeakEventManager<InLineTextBox, RoutedEventArgs>.AddHandler(inlineBox.TwinsBox, "LostFocus", action);
            }
        }

        public void RemoveGetFocusEvent(EventHandler<RoutedEventArgs> action)
        {
            foreach (InLineTextBox inLineTextBox in _inlineBoxs)
            {
                WeakEventManager<InLineTextBox, RoutedEventArgs>.RemoveHandler(inLineTextBox.TwinsBox, "GotFocus", action);
            }
        }

        public void RemoveLostFocusEvent(EventHandler<RoutedEventArgs> action)
        {
            foreach (var inLineTextBox in _inlineBoxs)
            {
                WeakEventManager<InLineTextBox, RoutedEventArgs>.RemoveHandler(inLineTextBox.TwinsBox, "LostFocus", action);
            }
        }

        protected override int VisualChildrenCount => _inlineBoxs.Count * 2;

        protected override Visual GetVisualChild(int index)
        {
            if (index >= _inlineBoxs.Count)
            {
                return _editInlineBoxs[index-_inlineBoxs.Count];
            }
            return _inlineBoxs[index];
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var textBox in _inlineBoxs)
            {
                textBox.Measure(availableSize);
                textBox.TwinsBox.Measure(availableSize);
            }

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var pos = new Point(0, 0);
            foreach (var textBox in _inlineBoxs)
            {
                textBox.Arrange(new Rect(pos, textBox.DesiredSize));
                textBox.TwinsBox.Arrange(new Rect(pos, textBox.TwinsBox.DesiredSize));
            }

            return finalSize;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        

        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                foreach (var inlineBox in _inlineBoxs)
                {
                    WeakEventManager<TextBox, System.Windows.Input.MouseButtonEventArgs>.RemoveHandler(inlineBox,
                        "PreviewMouseLeftButtonDown", InLineBox_MouseLeftButtonDown);
                    inlineBox.SizeChanged -= InLineBox_SizeChanged;
                    var variableInfo = inlineBox.DataContext as VariableInfo;
                    if (variableInfo != null)
                    {
                        variableInfo.AbandonInline(_textView);
                        if (!_textView.IsKeepEditInlineState)
                        {
                            variableInfo.SetInlineVisibility(Visibility.Collapsed,inlineBox.Name=="Top",_textView);
                            variableInfo.StopTimer();
                        }
                    }
                    RemoveVisualChild(inlineBox);
                    RemoveVisualChild(inlineBox.TwinsBox);
                }
                
                _inlineBoxs.Clear();
                _editInlineBoxs.Clear();
                _textView = null;
            }

            _disposed = true;
        }
    }
}