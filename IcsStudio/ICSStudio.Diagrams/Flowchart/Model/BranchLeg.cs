using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.Diagrams.Controls;
using ICSStudio.Diagram.Flowchart;
using ICSStudio.SimpleServices.Chart;

namespace ICSStudio.Diagrams.Flowchart.Model
{
    using Port = Tuple<FlowNode, PortKinds>;

    public class BranchLeg
    {
        public FlowNode BranchNode { get; private set; }

        public BranchLeg(FlowNode branch)
        {
            if (branch.Kind != NodeKinds.Branch)
            {
                throw new Exception("the kind of node is not branch.");
            }
            BranchNode = branch;
        }

        public void DeleteLeg(Port port)
        {
            var r = Points.FirstOrDefault(x => x.Item1 == port.Item1);
            if (r != null)
                Points.Remove(r);
        }

        public void AddLeg(Port port)
        {
            var r = Points.FirstOrDefault(x => x.Item1 == port.Item1);
            if (r == null)
            {
                Points.Add(port);
                if (port.Item1.Kind == NodeKinds.Branch)
                {
                    (port.Item1 as BranchFlowNode)?.BranchLeg.AddLeg(new Port(this.BranchNode,PortKinds.None));
                }
            }
        }
        
        public Point GetMaxPoint(BranchFlowNode avoidBranch)
        {
            double x = 0;
            int count = 0;
            foreach (var point in Points)
            {
                if (point.Item1.Kind == NodeKinds.Branch) continue;
                var portX = point.Item1.FindPortX();
                x = Math.Max(x, portX);
                count++;
            }
            if (ConnectNodePort != null)
            {
                var portX = ConnectNodePort.Item1.FindPortX();
                x = Math.Max(x, portX);
                count++;
            }
            if (count < 2)
            {
                BranchFlowNode rightBranch = null;
                foreach (var tuple in Points.Where(p => p.Item1.Kind == NodeKinds.Branch))
                {
                    if (tuple.Item1 == avoidBranch) continue;
                    if (rightBranch == null) rightBranch = tuple.Item1 as BranchFlowNode;
                    else
                    {
                        if (rightBranch.BranchLeg.GetMinPoint(BranchNode as BranchFlowNode).X < (tuple.Item1 as BranchFlowNode).BranchLeg.GetMinPoint(BranchNode as BranchFlowNode).X) rightBranch = tuple.Item1 as BranchFlowNode;
                    }
                }

                if (rightBranch != null)
                {
                    x = Math.Max(rightBranch.BranchLeg.GetMinPoint(BranchNode as BranchFlowNode).X,x) ;
                }
            }
            return new Point(x,BranchNode.Y);
        }

        public Point GetMinPoint(BranchFlowNode avoidBranch)
        {
            double x = 99999;
            int count = 0;
            foreach (var point in Points)
            {
                if (point.Item1.Kind == NodeKinds.Branch) continue;
                var portX = point.Item1.FindPortX();

                x = Math.Min(x, portX);
                count++;
            }

            if (ConnectNodePort != null)
            {
                var portX = ConnectNodePort.Item1.FindPortX();

                x = Math.Min(x, portX);
                count++;
            }
            if (count < 2)
            {
                BranchFlowNode leftBranch = null;
                foreach (var tuple in Points.Where(p => p.Item1.Kind == NodeKinds.Branch))
                {
                    if(tuple.Item1==avoidBranch)continue;
                    if (leftBranch == null) leftBranch = tuple.Item1 as BranchFlowNode;
                    else
                    {
                        if (leftBranch.BranchLeg.GetMinPoint(BranchNode as BranchFlowNode).X > (tuple.Item1 as BranchFlowNode).BranchLeg.GetMinPoint(BranchNode as BranchFlowNode).X) leftBranch = tuple.Item1 as BranchFlowNode;
                    }
                }

                if (leftBranch != null)
                {
                    x = Math.Min(leftBranch.BranchLeg.GetMaxPoint(BranchNode as BranchFlowNode).X,x);
                }
            }
            return new Point(x, BranchNode.Y);
        }
        
        public Tuple<FlowNode,PortKinds> ConnectNodePort { set; get; }

        public ObservableCollection<Port> Points { get; } = new ObservableCollection<Port>();
    }
}
