using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ICSStudio.AvalonEdit.Rendering;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Tags;
using Microsoft.VisualStudio.Threading;

namespace ICSStudio.AvalonEdit.Variable
{
    sealed class InLineLayer:Layer
    {
        //private TextView _textView;
        public InLineLayer(TextView textView) : base(textView, KnownLayer.InLine)
        {
            //_textView = textView;
        }

        public void Clean()
        {
            LastTextBox = null;
            foreach (var visual in visuals)
            {
                visual.RemoveGetFocusEvent(InlineBox_GotFocus);
                visual.RemoveLostFocusEvent(InlineBox_LostFocus);
                visual.Dispose();
            }
        }

        internal int index;

        List<VisualInLineDrawingVisual> visuals = new List<VisualInLineDrawingVisual>();

        internal void SetVisualLines(ICollection<VisualInLineDrawingVisual> visualLines)
        {
            foreach (VisualInLineDrawingVisual v in visuals)
            {
                if(visualLines.Contains(v))continue;
                //if (v.VisualLine.IsDisposed)
                v.RemoveGetFocusEvent(InlineBox_GotFocus);
                v.RemoveLostFocusEvent(InlineBox_LostFocus);
                v.Dispose();
                RemoveVisualChild(v);
            }

            LastTextBox = null;
            visuals.Clear();
            foreach (VisualInLineDrawingVisual v in visualLines)
            {
                //VisualInLineDrawingVisual v = newLine.Render();
                if (!v.IsAdded)
                {
                    AddVisualChild(v);
                    v.IsAdded = true;
                    v.AddGetFocusEvent(InlineBox_GotFocus);
                    v.AddLostFocusEvent(InlineBox_LostFocus);
                }

                visuals.Add(v);
            }
            InvalidateVisual();
        }

        public void AddChild(VisualInLineDrawingVisual v)
        {
            if (!v.IsAdded)
            {
                AddVisualChild(v);
                v.IsAdded = true;
            }
        }

        public void AddEvent(VisualInLineDrawingVisual v)
        {
            v.AddGetFocusEvent(InlineBox_GotFocus);
            v.AddLostFocusEvent(InlineBox_LostFocus);
        }
        
        public void RecoveryWhenHidden()
        {
            if (LastTextBox != null)
            {
                var variableInfo = (VariableInfo) LastTextBox.DataContext;
                variableInfo.RecoveryWhenHidden();
                variableInfo.SetInlineVisibility(Visibility.Collapsed,LastTextBox.Name=="Top",textView);
            }
        }

        public InLineTextBox LastTextBox { private set; get; }
        
        private void InlineBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if(textView.IsKeepEditInlineState)return;
            InLineTextBox currentTextBox = ((InLineTextBox) sender);
            if (currentTextBox.IsAbort)
            {
                return;
            }

            var variableInfo = ((VariableInfo)currentTextBox.DataContext);
               
            if (variableInfo.GetInlineVisibility(currentTextBox.Name=="Top")!=Visibility.Visible)
            {  
                if (!textView.CaretVisible)
                    textView.ShowCaretLayer();
                return; 
            }
            
            LastTextBox?.OffMonitor();
            if (!variableInfo.IsEnabled)
            {
                if (!textView.CaretVisible)
                    textView.ShowCaretLayer();
                return;
            }
            
            if (!variableInfo.IsCorrect)
            {
                MessageBox.Show(
                    $"Failed to modify the tag value.\n{(string.IsNullOrEmpty(variableInfo.Error) ? "String invalid." : variableInfo.Error)}",
                    "ICSStudio", MessageBoxButton.OK, MessageBoxImage.Information);

                if (textView.CaretVisible)
                   textView.HideCaretLayer();
                Dispatcher.InvokeAsync(() =>
                    {
                        currentTextBox.Focus();
                    },
                    DispatcherPriority.ApplicationIdle);
                e.Handled = true;
                return;
            }
            else
            {
                if (!textView.CaretVisible)
                    textView.ShowCaretLayer();                                                                               
                if(variableInfo.IsValueChanged)
                {
                    CanInlineBoxFocus = false;
                    textView.Redraw();
                }
            }

            variableInfo.Value = variableInfo.EditValue;
            variableInfo.SetInlineVisibility(Visibility.Collapsed,currentTextBox.Name=="Top", textView);
            variableInfo.IsValueChanged = false;
            e.Handled = true;
        }

        public bool CanInlineBoxFocus { set; get; }

        private void InlineBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if(!CanInlineBoxFocus)
            {
                return;
            }
            if (textView.CaretVisible)
                textView.HideCaretLayer();
            
            if (LastTextBox == null)
            {
                LastTextBox = (InLineTextBox)sender;
            }
            else
            {
                if (LastTextBox != sender)
                {
                    if (!((LastTextBox.DataContext as VariableInfo)?.IsCorrect??true))
                    {
                        var info = ((InLineTextBox) sender).DataContext as VariableInfo;
                        if (info != null)
                        {
                            info.SetInlineVisibility(Visibility.Collapsed, ((InLineTextBox)sender).Name == "Top", textView);
                        }
                        var tb = LastTextBox;
                        Dispatcher.InvokeAsync(() =>
                        {
                            tb.Focus();
                        }, DispatcherPriority.ApplicationIdle);
                    }
                    else
                    {
                        LastTextBox?.OffMonitor();
                        LastTextBox = (InLineTextBox) sender;
                    }
                }
                else
                {
                   // LastTextBox.Focus();
                }
            }

            var variableInfo = LastTextBox?.DataContext as VariableInfo;
            if (variableInfo != null)
            {
                var tb = LastTextBox;
                //tb.IsReadOnly = false;
                 Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await Task.Delay(300);
                    tb?.OnMonitor();
                }, DispatcherPriority.ApplicationIdle);
            }
            e.Handled = true;
        }
        
        protected override int VisualChildrenCount
        {
            get { return visuals.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            var line = visuals[index];
            return line;
        }

        protected override Size MeasureCore(Size availableSize)
        {
            if (Visibility != Visibility.Visible) return availableSize;
            if (availableSize.Height > 32000)
                availableSize.Height = 32000;
            foreach (var visual in visuals)
            {
                visual.Measure(availableSize);
            }
            return availableSize;
        }

        protected override void ArrangeCore(Rect finalRect)
        {
            if (Visibility != Visibility.Visible) return;
            Point point = new Point(0, 0);
            foreach (var visual in visuals)
            {
                visual.Arrange(new Rect(point, visual.DesiredSize));
            }
            textView.ArrangeInLineLayer(visuals, finalRect);
        }
    }
}
