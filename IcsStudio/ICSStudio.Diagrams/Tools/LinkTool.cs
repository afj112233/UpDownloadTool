using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using ICSStudio.Diagrams.Controls;
using System.Windows.Documents;
using System.Windows.Controls;
using ICSStudio.Diagrams.Adorners;
using ICSStudio.Diagrams.Chart;
using ICSStudio.Diagrams.Controls.Ports;
using ICSStudio.Diagrams.Flowchart;
using ICSStudio.Diagrams.Flowchart.Model;
using ICSStudio.Diagram.Flowchart;

namespace ICSStudio.Diagrams.Tools
{
	public class LinkTool: ILinkTool
	{
		protected DiagramView View { get; private set; }
		protected IDiagramController Controller { get { return View.Controller; } }
		protected Point DragStart { get; set; }
		protected ILink Link { get; set; }
		protected LinkThumbKind Thumb { get; set; }
		protected LinkInfo InitialState { get; set; }
		protected LinkAdorner Adorner { get; set; }
        protected bool CanLinkToBranch { set; get; }
		private bool _isNewLink;

		public LinkTool(DiagramView view)
		{
			View = view;
		}

		public void BeginDrag(Point start, ILink link, LinkThumbKind thumb)
		{
			BeginDrag(start, link, thumb, false);
		}

		protected virtual void BeginDrag(Point start, ILink link, LinkThumbKind thumb, bool isNew)
		{
			_isNewLink = isNew;
			DragStart = start;
			Link = link;
			Thumb = thumb;
			InitialState = new LinkInfo(link);
			Adorner = CreateAdorner();
			View.DragAdorner = Adorner;
		}

        public void AttachmentPortDragTo(AttachmentPort port,Point target)
        {
            Link.TargetPoint = target;
            Link.UpdatePath();
            if (CanAttachLinkTo(target))
            {
                Adorner.Port = port;
            }
            else
            {
                Adorner.Port = null;
                CanLinkToBranch = false;
                Link.Branch = null;
            }
        }

        public virtual void DragTo(Vector vector)
        {

            vector = UpdateVector(vector);
            var point = DragStart + vector;
            var port = View.Children.OfType<INode>().SelectMany(p => p.Ports)
                .Where(p => p.IsNear(point))
                .OrderBy(p => GeometryHelper.Length(p.Center, point))
                .FirstOrDefault();
            var start = View.Children.OfType<INode>().SelectMany(p => p.Ports)
                .Where(p => p.IsNear(DragStart))
                .OrderBy(p => GeometryHelper.Length(p.Center, point))
                .FirstOrDefault();
            if (start is AttachmentPort)
            {
                AttachmentPortDragTo(start as AttachmentPort, point);
                return;
            }
            var branchNode = View.Items.OfType<INode>().FirstOrDefault(x => x.IsInBranch(point));
            if (((BranchFlowNode) (branchNode as Node)?.ModelElement)?.BranchLeg.ConnectNodePort != null)
            {
                return;
            }
            
            if ((Controller as Controller).IsConnected(start)) return;
            if (!CanLinkTo(start, port))
            {
                var branch = View.Children.OfType<INode>().FirstOrDefault(n => n.IsInBranch(point));
                if (branch != null)
                {
                    var b = ((Branch) (branch as Node).GetContent()).BranchLeg.Points[0];
                    if (CanLinkTo(start, (Controller as Controller).FindPort(b.Item1, b.Item2)))
                    {
                        port = start;
                        point = new Point(DragStart.X, ((FlowNode) (branch as Node).ModelElement).Y + 10);
                        CanLinkToBranch = true;
                        Link.Branch = (Branch) (branch as Node).GetContent();
                    }
                    else
                    {
                        port = null;
                        CanLinkToBranch = false;
                        Link.Branch = null;
                    }
                }
                else
                {
                    port = null;
                    CanLinkToBranch = false;
                    Link.Branch = null;
                }

            }


            UpdateLink(point, port);

            Adorner.Port = port;

            Link.UpdatePath();
        }

        protected bool CanAttachLinkTo(Point target)
        {
            var nodes = View.Children.OfType<INode>();
            
            var node = nodes.FirstOrDefault(x => x.IsInNode(target)) as Node;
            if (node != null)
            {
                var flowNode = (FlowNode)node.ModelElement;
                if (flowNode.Kind != NodeKinds.TextBox && flowNode.Kind != NodeKinds.Branch)
                {
                    var link=View.Children.OfType<AttachmentLink>().FirstOrDefault(l => l.TargetNode!=null&&l.TargetNode.Equals(node));
                    if (link != null) return false;
                    return true;
                }
            }
            return false;
        }
        
        protected bool CanLinkTo(IPort source,IPort target)
        {
            var sourceBase = source as PortBase;
            var targetBase = target as PortBase;
            var nodes = View.Children.OfType<INode>();
            INode sourceNode=null, targetNode=null;
            //set the rule:hwo to link ports
            foreach (var node in nodes)
            {
                if (sourceNode != null && targetNode != null)
                {
                    break;
                }
                foreach (var port in node.Ports)
                {
                    if (port == source) sourceNode = node;
                    if (port == target) targetNode = node;
                }
            }

            if (sourceNode == targetNode || sourceNode == null || targetNode == null) return false;
            var flowNodeSource = (FlowNode) (sourceNode as Node)?.ModelElement;
            var flowNodeTarget = (FlowNode) (targetNode as Node)?.ModelElement;
            if (flowNodeSource?.Kind == NodeKinds.Transition)
            {
                if (flowNodeTarget?.Kind == NodeKinds.Transition)
                {
                    if (targetBase?.IsTop == sourceBase?.IsTop) return true;
                    return false;
                }
                else if (flowNodeTarget?.Kind == NodeKinds.Step)
                {
                    if (targetBase?.IsTop != sourceBase?.IsTop) return true;
                    return false;
                }

                return true;
            }
            else if (flowNodeSource?.Kind == NodeKinds.Step)
            {
                if (flowNodeTarget?.Kind==NodeKinds.Step)
                {
                    if (targetBase?.IsTop == sourceBase?.IsTop) return true;
                    return false;
                }
                else if (flowNodeTarget?.Kind==NodeKinds.Transition)
                {
                    if (targetBase.IsTop && !sourceBase.IsTop) return true;
                    return false;
                }
                else if(flowNodeTarget?.Kind==NodeKinds.Stop)
                {
                    if (sourceBase.IsTop) return true;
                    return false;
                }
            }
            else if (flowNodeSource?.Kind == NodeKinds.Stop)
            {
                if (flowNodeTarget?.Kind == NodeKinds.Stop) return true;
                else if (flowNodeTarget?.Kind == NodeKinds.Step)
                {
                    if (targetBase.IsTop) return true;
                    return false;
                }
                else if (flowNodeTarget?.Kind==NodeKinds.Transition)
                {
                    if (!targetBase.IsTop) return true;
                    return false;
                }
            }
            else
            {
                return true;
            }

            return false;
        }

		protected virtual void UpdateLink(Point point, IPort port)
		{
			if (Thumb == LinkThumbKind.Source)
			{
				Link.Source = port;
				Link.SourcePoint = port == null ? point : (Point?)null;
			}
			else
			{
				Link.Target = port;
				Link.TargetPoint = port == null ? point : (Point?)null;
			}
		}

		protected virtual bool CanLinkTo(IPort port)
		{
			var pb = port as PortBase;
			if (pb != null)
			{
				if (Thumb == LinkThumbKind.Source)
					return pb.CanAcceptOutgoingLinks;
				else
					return pb.CanAcceptIncomingLinks;
			}
			else
				return true;
		}

		public virtual bool CanDrop()
		{
            if (Adorner.Port != null)
            {

            }
			return Adorner.Port != null||CanLinkToBranch;
		}

		public virtual void EndDrag(bool doCommit)
		{
			if (doCommit)
			{
				Controller.UpdateLink(InitialState, Link);
            }
			else
			{
				if (_isNewLink)
					View.Children.Remove((Control)Link);
				else
					InitialState.UpdateLink(Link);
			}
			Link?.UpdatePath();
			Link = null;
			Adorner = null;
            CanLinkToBranch = false;
        }

		public virtual void BeginDragNewLink(Point start, IPort port)
        {
             var r=View.Children.OfType<OrthogonalLink>().Where(l=>l.Source==port||l.Target==port).ToList();
            if(r.Count>0)return;

			var link = port is AttachmentPort?new AttachmentLink(port) : CreateNewLink(port);
			if (link != null && link is Control)
			{
				var thumb = (link.Source != null) ? LinkThumbKind.Target : LinkThumbKind.Source;
				View.Children.Add((Control)link);
				BeginDrag(start, link, thumb, true);
			}
		}

		protected virtual ILink CreateNewLink(IPort port)
		{
			var link = new SegmentLink();
			BindNewLinkToPort(port, link);
			return link;
		}

		protected virtual void BindNewLinkToPort(IPort port, LinkBase link)
		{
			link.EndCap = true;
			var portBase = port as PortBase;
			if (portBase != null)
			{
				if (portBase.CanAcceptIncomingLinks && !portBase.CanAcceptOutgoingLinks)
					link.Target = port;
				else
					link.Source = port;
			}
			else
				link.Source = port;
		}

		protected virtual LinkAdorner CreateAdorner()
		{
			return new LinkAdorner(View, DragStart) { Cursor = Cursors.Cross };
		}

		protected virtual Vector UpdateVector(Vector vector)
		{
			return vector;
		}
	}
}
