using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ICSStudio.Diagrams.Controls;
using ICSStudio.Diagrams.Controls.Ports;

namespace ICSStudio.Diagrams.Flowchart
{
    class AttachmentLink: SegmentLink
    {
        public AttachmentLink(IPort port)
        {
            Source = port;
        }
        
        public override void UpdatePath()
        {
            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            if(Source==null)return;
            figure.StartPoint = Source.Center;
            figure.Segments.Add(new PolyLineSegment(new Point[] {Calculate()}, true));
            geometry.Figures.Add(figure);
            this.PathGeometry = geometry;
        }

        protected override Point[] CalculateSegments()
        {
            return null;
        }

        public INode TargetNode { set; get; }

        private Point Calculate()
        {
            if (TargetNode == null) return (Point)TargetPoint;
            var node = TargetNode as Node;
            var rect = node.Bounds;
            double x=0, y=0;
            if (Source.Center.X < rect.X) x = rect.X;
            else if (Source.Center.X >= rect.X && Source.Center.X <= rect.X + rect.Width) x = Source.Center.X;
            else x = rect.X + rect.Width;

            if (Source.Center.Y >= rect.Y+rect.Height) y = rect.Y+rect.Height-10;
            else if (Source.Center.Y >= rect.Y && Source.Center.Y < rect.Y + rect.Height) y = Source.Center.Y;
            else y = rect.Y ;
            return new Point(x,y);
        }
    }
}
