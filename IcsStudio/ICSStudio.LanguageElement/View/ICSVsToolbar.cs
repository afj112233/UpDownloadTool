using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using ICSStudio.Dialogs.GlobalSetting;
using Microsoft.VisualStudio.PlatformUI;

namespace ICSStudio.LanguageElement.View
{
    public class ICSVsToolbar:VsToolBar
    {
        static ICSVsToolbar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ICSVsToolbar), new FrameworkPropertyMetadata(typeof(ICSVsToolbar)));
        }

        private TrackAdorner _trackAdorner;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            GridSplitter gridSplitter = (GridSplitter)base.GetTemplateChild("PART_Splitter");
            gridSplitter.DragCompleted += Gg_DragCompleted;
            gridSplitter.DragStarted += Gg_DragStarted;
            gridSplitter.DragDelta += Gg_DragDelta;
        }

        private void Gg_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var items = Items.OfType<View>();
            if (items.Any())
            {
                var view = items.First();
                var start = new Point(view.ActualWidth, 0);
                var end=new Point(e.HorizontalChange+ view.ActualWidth, view.ActualHeight);
                _trackAdorner?.UpdatePosition(start, end);

            }
            e.Handled = true;
        }

        private void Gg_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            AdornerLayer layer = null;
            var items = Items.OfType<View>();
            if (items.Any())
            {
                var view = items.First();
                _trackAdorner = new TrackAdorner(view.Grid);
                layer = AdornerLayer.GetAdornerLayer(view.Grid);
            }

            layer?.Add(_trackAdorner);
            e.Handled = true;
        }

        private void Gg_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            var gridSplitter = (GridSplitter)sender;
            if (gridSplitter != null)
            {
                var offset = e.HorizontalChange - 0;
                var width = Width + offset;
                Width = Math.Max(width, 10);
                foreach (Control item in Items)
                {
                    width = item.Width + offset;
                    item.Width = Math.Max(width, 10);
                    if (item is View)
                    {
                        Properties.Settings.Default.StLanguageWidth = item.Width;
                        Properties.Settings.Default.Save();

                        //记录拖拽后的指令宽度
                        GlobalSetting.GetInstance().ProgramInstructionWidthSetting = item.Width;
                        GlobalSetting.GetInstance().SaveConfig();
                    }
                }
                var items = Items.OfType<View>();
                if (items.Any())
                {
                    var view = items.First();
                    var layer = AdornerLayer.GetAdornerLayer(view.Grid);
                    layer?.Remove(_trackAdorner);
                    _trackAdorner = null;
                }
            }
        }
    }
}
