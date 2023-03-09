using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using ICSStudio.Gui.Annotations;

namespace ICSStudio.LanguageElement.View
{
    internal class TrackAdorner: Adorner
    {
        public TrackAdorner([NotNull] UIElement adornedElement) : base(adornedElement)
        {
        }
        private Point _startPoint;
        private Point _endPoint;

        public void UpdatePosition(Point start, Point end)
        {
            _startPoint = start;
            _endPoint = end;
            InvalidateVisual();
        }
        
        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Black, 1), new Rect(_startPoint, _endPoint));
        }
    }
}
