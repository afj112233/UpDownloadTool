using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSStudio.Diagrams.Tools;
using ICSStudio.Diagrams;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.Diagrams.Chart;
using ICSStudio.Diagrams.Flowchart.Model;
using ICSStudio.SimpleServices.Chart;

namespace ICSStudio.Diagram.Flowchart
{
	class DragDropTool: IDragDropTool
	{
		DiagramView _view; 
		FlowchartModel _model;
		//int _row, _column;
        double _x, _y;

		public DragDropTool(DiagramView view, FlowchartModel model)
		{
			_view = view;
			_model = model;
		}

		public void OnDragEnter(System.Windows.DragEventArgs e)
		{
		}

		public void OnDragOver(System.Windows.DragEventArgs e)
		{
			e.Effects = DragDropEffects.None;
			if (e.Data.GetDataPresent(typeof(FlowNode)))
			{
				var position = e.GetPosition(_view);
                _x = position.X;
                _y = position.Y;
                //_column = (int)(position.X / _view.GridCellSize.Width);
                //_row = (int)(position.Y / _view.GridCellSize.Height);
                //if (_column >= 0 && _row >= 0)
                //	if (_model.Nodes.Where(p => p.Column == _column && p.Row == _row).Count() == 0)
                //		e.Effects = e.AllowedEffects;
                e.Effects = e.AllowedEffects;
            }
            //e.Effects = e.AllowedEffects;

            e.Handled = true;
		}

		public void OnDragLeave(System.Windows.DragEventArgs e)
		{
		}

		public void OnDrop(System.Windows.DragEventArgs e)
        {
			var node = (FlowNode)e.Data.GetData(typeof(FlowNode));
            //node.Row = _row;
            //node.Column = _column;
            if (node.Kind == NodeKinds.StepAndTransition)
            {
                var step=new FlowNode(NodeKinds.Step);
                step.X = GetNearCross(_x + node.Offset) - node.Offset;
                step.Y = GetNearCross(_y);

                var transition=new FlowNode(NodeKinds.Transition);
                transition.X = step.X;
                transition.Y = step.Y + 100;

                _model.Nodes.Add(step);
                _model.Nodes.Add(transition);

                _model.Links.Add(new Link(step,PortKinds.Bottom,transition,PortKinds.Top));
            }
            else if (node.Kind == NodeKinds.SelectionBranchDiverge)
            {
                var transition=new FlowNode(NodeKinds.Transition);
                transition.X = GetNearCross(_x + transition.Offset) - transition.Offset;
                transition.Y = GetNearCross(_y);

                var transition2=new FlowNode(NodeKinds.Transition);
                transition2.X = transition.X + 100;
                transition2.Y = transition.Y;
                _model.Nodes.Add(transition);
                _model.Nodes.Add(transition2);

                var branch = new BranchFlowNode(NodeKinds.Branch,BranchFlowType.Diverge,BranchType.Selection);
                //branch.X = transition.X;
                branch.Y = transition.Y - 40;

                _model.Nodes.Add(branch);
                var branchLeg=branch.BranchLeg;
                branchLeg.AddLeg(new Tuple<FlowNode, PortKinds>(transition,PortKinds.Top));
                branchLeg.AddLeg(new Tuple<FlowNode, PortKinds>(transition2,PortKinds.Top));
                _model.BranchLeges.Add(branchLeg);
            }
            else if (node.Kind == NodeKinds.SimultaneousBranchDiverge)
            {
                var step=new FlowNode(NodeKinds.Step);
                step.X = GetNearCross(_x + step.Offset) - step.Offset;
                step.Y = GetNearCross(_y);

                var step2=new FlowNode(NodeKinds.Step);
                step2.X = step.X + 100;
                step2.Y = step.Y;
                _model.Nodes.Add(step);
                _model.Nodes.Add(step2);

                var branch=new BranchFlowNode(NodeKinds.Branch,BranchFlowType.Diverge,BranchType.Simultaneous);
               // branch.X = step.X;
                branch.Y = step.Y - 40;
                _model.Nodes.Add(branch);
                var branchLeg = branch.BranchLeg;
                branchLeg.AddLeg(new Tuple<FlowNode, PortKinds>(step,PortKinds.Top));
                branchLeg.AddLeg(new Tuple<FlowNode, PortKinds>(step2,PortKinds.Top));
                _model.BranchLeges.Add(branchLeg);
            }
            else
            {
                node.X = GetNearCross(_x + node.Offset) - node.Offset;
                node.Y = GetNearCross(_y);
                _model.Nodes.Add(node);
            }
            
			e.Handled = true;
		}

        private double GetNearCross(double p)
        {
            p = ((int) (p / 20) + 1) * 20;
            return p;
        }
	}
}
