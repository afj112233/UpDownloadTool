using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ICSStudio.Diagrams.Controls;
using ICSStudio.Diagrams.Flowchart.Model;
using ICSStudio.Diagram.Flowchart;
using ICSStudio.SimpleServices.Chart;

namespace ICSStudio.Diagrams.Flowchart
{
   public class BranchLegLink:SegmentLink
    {
        private BranchFlowNode _connectedBranchFlowNode;

        public BranchLegLink(BranchFlowNode branch)
        {
            Branch = branch;
            branch.PropertyChanged += BranchLegLink_PropertyChanged;
        }

        private void BranchLegLink_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Y") UpdatePath();
        }

        protected override Point[] CalculateSegments()
        {
            if (Branch != null)
            {
                double offsetSourceY = 10, offsetTargetY = 10;
                if (Branch.BranchType == BranchType.Simultaneous)
                {
                    offsetTargetY = 7;
                }

                if (ConnectedBranchFlowNode != null)
                {
                    //Point source = ConnectedBranchFlowNode.X > Branch.X
                    //    ? ConnectedBranchFlowNode.BranchLeg.GetMaxPoint() + new Vector(10, 10)
                    //    : ConnectedBranchFlowNode.BranchLeg.GetMinPoint() + new Vector(-10, 10);
                    if (ConnectedBranchFlowNode.BranchType == BranchType.Simultaneous)
                    {
                        offsetSourceY = 7;
                    }

                    Point source =
                        new Point(
                            (ConnectedBranchFlowNode.X > Branch.X
                                ? ConnectedBranchFlowNode.FindPortX() + 20
                                : Branch.FindPortX()) + 10, ConnectedBranchFlowNode.Y + offsetSourceY);
                    Point target = new Point(source.X, Branch.Y + offsetTargetY);

                    return new Point[] {target, source};
                }

                if (Source != null)
                {
                    if (Source.IsTop) offsetTargetY = 12;
                    double y = Branch.Y + offsetTargetY;
                    Point end = new Point(Source.Center.X, y);
                    return new Point[] {end, Source.Center};
                }
            }

            return base.CalculateSegments();
        }

        public BranchFlowNode ConnectedBranchFlowNode
        {
            set
            {
                if(_connectedBranchFlowNode!=null)
                    _connectedBranchFlowNode.PropertyChanged -= BranchLegLink_PropertyChanged;
                _connectedBranchFlowNode = value;
                _connectedBranchFlowNode.PropertyChanged += BranchLegLink_PropertyChanged;

            }
            get { return _connectedBranchFlowNode; }
        }

        public new BranchFlowNode Branch { private set; get; }
    }
}
