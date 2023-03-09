using System;
using System.Collections.Generic;
using System.Linq;
using ICSStudio.Diagrams;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using ICSStudio.Diagrams.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;
using ICSStudio.Diagrams.Chart;
using ICSStudio.Diagrams.Controls.Ports;
using ICSStudio.Diagrams.Flowchart;
using ICSStudio.Diagrams.Flowchart.Model;
using ICSStudio.Diagram.Chart;
using ICSStudio.Diagrams.Exceptions;
using ICSStudio.SimpleServices.Chart;
using Branch = ICSStudio.Diagrams.Chart.Branch;
using ILink = ICSStudio.Diagrams.Controls.ILink;
using Step = ICSStudio.Diagrams.Chart.Step;
using Stop = ICSStudio.Diagrams.Chart.Stop;
using TextBox = ICSStudio.Diagrams.Chart.TextBox;
using Transition = ICSStudio.Diagram.Chart.Transition;

namespace ICSStudio.Diagram.Flowchart
{
	class Controller : IDiagramController
	{
		private class UpdateScope : IDisposable
		{
			private Controller _parent;
			public bool IsInprogress { get; set; }

			public UpdateScope(Controller parent)
			{
				_parent = parent;
			}

			public void Dispose()
			{
				IsInprogress = false;
				_parent.UpdateView();
			}
		}

		private DiagramView _view;
		private FlowchartModel _model;
		private UpdateScope _updateScope;

		public Controller(DiagramView view, FlowchartModel model)
		{
			_view = view;
			_model = model;
			_model.Nodes.CollectionChanged += NodesCollectionChanged;
			_model.Links.CollectionChanged += LinksCollectionChanged;
            _model.BranchLeges.CollectionChanged += BranchLeges_CollectionChanged;
			_updateScope = new UpdateScope(this);

			foreach (var t in _model.Nodes)
				t.PropertyChanged += NodePropertyChanged;

			UpdateView();
		}

        public void DelAttachLink(FlowNode node)
        {
            _model.AttachLinks.RemoveRange(x=>x.Source==node);
            UpdateView();
        }

        private void BranchLeges_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateView();
        }

        void NodesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
				foreach (var t in e.NewItems.OfType<INotifyPropertyChanged>())
					t.PropertyChanged += NodePropertyChanged;

			if (e.OldItems != null)
				foreach (var t in e.OldItems.OfType<INotifyPropertyChanged>())
					t.PropertyChanged -= NodePropertyChanged;
			UpdateView();
		}

		void LinksCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			UpdateView();
		}

		void NodePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var fn = sender as FlowNode;
			var n = _view.Children.OfType<Node>().FirstOrDefault(p => p.ModelElement == fn);
			if (fn != null && n != null)
				UpdateNode(fn, n);
		}

		private void UpdateView()
		{
			if (!_updateScope.IsInprogress)
			{
				_view.Children.Clear();

				foreach (var node in _model.Nodes)
                {
                    _view.Children.Add(UpdateNode(node, null));
                    node.SetDiagramView(_view);
                }
                //to set branch.X
                _view.UpdateBranch();
				foreach (var link in _model.Links)
					_view.Children.Add(CreateLink(link));
                foreach (var link in _model.AttachLinks)
                    _view.Children.Add(CreateAttachedLink(link));
                
                foreach (var branchLeg in _model.BranchLeges)
                {
                    var legs = CreateBranchLeg(branchLeg.BranchNode as BranchFlowNode);
                    foreach (var control in legs)
                    {
                        _view.Children.Add(control);
                    }

                    if (branchLeg.ConnectNodePort != null)
                    {
                        var link=new BranchLegLink(branchLeg.BranchNode as BranchFlowNode);
                        link.ModelElement = branchLeg;
                        link.Source = FindPort(branchLeg.ConnectNodePort.Item1, branchLeg.ConnectNodePort.Item2);
                        link.UpdatePath();
                        _view.Children.Add(link);
                    }
                }
			}
		}

		private Node UpdateNode(FlowNode node, Node item)
		{
			if (item == null)
			{
				item = new Node();
				item.ModelElement = node;
				CreatePorts(node, item);
				item.Content = CreateContent(item);
			}
			//item.Width = _view.GridCellSize.Width - 20;
			//item.Height = _view.GridCellSize.Height - 50;
			item.CanResize = false;
            item.SetValue(Canvas.LeftProperty, node.X);
            item.SetValue(Canvas.TopProperty, node.Y);
            //if (node.X > 0 || node.Y > 0)
            //{
            //    item.SetValue(Canvas.LeftProperty, node.X);
            //    item.SetValue(Canvas.TopProperty, node.Y);
            //}
            //else
            //else
            //{
            //    item.SetValue(Canvas.LeftProperty, node.Column * _view.GridCellSize.Width + 10);
            //    item.SetValue(Canvas.TopProperty, node.Row * _view.GridCellSize.Height + 25);
            //}
            if (node.Kind != NodeKinds.TextBox)
            {
                var link = _view.Children.OfType<AttachmentLink>().FirstOrDefault(l => l.TargetNode.Equals(item)) as AttachmentLink;
                link?.UpdatePath();
            }
            return item;
		}

        private void UpdateRelatedBranchLinks(FlowNode node)
        {
            var chart = _view.Items.OfType<Node>().FirstOrDefault(n => n.ModelElement == node);
            foreach (var branchLegLink in _view.Items.OfType<BranchLegLink>().Where(l => chart.Ports.Contains(l.Source)))
            {
                branchLegLink.UpdatePath();
            }
        }

        public static FrameworkElement CreateIcon(FlowNode node)
        {
            if (node.Kind == NodeKinds.Stop)
            {
                var ui=new ICSStudio.Diagrams.icon.Stop();
                return ui;
            }
            else if (node.Kind==NodeKinds.Transition)
            {
                var ui = new ICSStudio.Diagrams.icon.Transition();
                return ui;
            }
            else if (node.Kind == NodeKinds.Step)
            {
                var ui=new ICSStudio.Diagrams.icon.Step();
                return ui;
            }
            else if(node.Kind==NodeKinds.StepAndTransition)
            {
                var ui=new ICSStudio.Diagrams.icon.Step_Transition();
                return ui;
            }
            else if (node.Kind == NodeKinds.SelectionBranchDiverge)
            {
                var ui=new ICSStudio.Diagrams.icon.SelectionBranchDiverge();
                return ui;
            }
            else if (node.Kind==NodeKinds.SimultaneousBranchDiverge)
            {
                var ui=new ICSStudio.Diagrams.icon.SimultaneousBranchDiverge();
                return ui;
            }
            else if (node.Kind == NodeKinds.SubroutineOrReturn)
            {
                var ui=new ICSStudio.Diagrams.icon.SubroutineOrReturn();
                return ui;
            }
            else if (node.Kind == NodeKinds.TextBox)
            {
                var ui=new ICSStudio.Diagrams.icon.TextBox();
                return ui;
            }
            return null;
        }

        public static FrameworkElement CreateContent(Node n)
        {
            FlowNode node = (FlowNode)n.ModelElement;
            var textBlock = new TextBlock()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            var b = new Binding("Text");
            b.Source = node;
            textBlock.SetBinding(TextBlock.TextProperty, b);

            if (node.Kind == NodeKinds.Transition)
            {
                var ui = new Transition();
                TransitionViewModel vm = new TransitionViewModel(node);
                ui.DataContext = vm;
                return ui;
                //var ui=new ICSStudio.Diagrams.icon.Transition();
                //return ui;
            }
            else if (node.Kind == NodeKinds.Step)
            {
                var ui = new Step();
                StepViewModel vm = new StepViewModel(node);
                ui.DataContext = vm;
                return ui;
            }
            else if (node.Kind == NodeKinds.Stop)
            {
                var ui = new Stop();
                StopViewModel vm = new StopViewModel(node);
                ui.DataContext = vm;
                return ui;
            }
            else if (node.Kind == NodeKinds.Branch)
            {
                var ui = new Branch((node as BranchFlowNode)?.BranchLeg);
                return ui;
            }
            else if (node.Kind == NodeKinds.SubroutineOrReturn)
            {
                var ui = new SubroutineOrReturn();
                SubroutineOrReturnViewModel vm = new SubroutineOrReturnViewModel();
                ui.DataContext = vm;
                return ui;
            }
            else if (node.Kind == NodeKinds.TextBox)
            {
                var ui = new TextBox();
                var vm = new TextBoxViewModel(node);
                ui.DataContext = vm;
                return ui;
            }
            else
            {
                var ui = new Path();
                ui.Stroke = Brushes.Black;
                ui.StrokeThickness = 1;
                ui.Fill = Brushes.Pink;
                var converter = new GeometryConverter();
                ui.Data = (Geometry)converter.ConvertFrom("M 0,0.25 L 0.5 0 L 1,0.25 L 0.5,0.5 Z");
                ui.Stretch = Stretch.Uniform;

                var grid = new Grid();
                grid.Children.Add(ui);
                grid.Children.Add(textBlock);

                return grid;
            }
        }

        public Node GetBranchPortLink(IPort port)
        {
            var r = _model.BranchLeges.Where(l =>
                l.Points.Where(p => FindPort(p.Item1, p.Item2) == port).ToList().Count > 0).ToList();

            if (r.Count > 0)
            {
                var branchNode = _view.Items.FirstOrDefault(x => (FlowNode) x.ModelElement == r[0].BranchNode);
                return branchNode as Node;
            }

            var branchLeg = _model.BranchLeges.FirstOrDefault(b => b.ConnectNodePort!=null&& FindPort(b.ConnectNodePort.Item1, b.ConnectNodePort.Item2) == port);
            var branch = _view.Items.FirstOrDefault(x =>x.ModelElement is FlowNode&& (FlowNode) x.ModelElement == branchLeg?.BranchNode);
            return branch as Node;
        }

        //private double CalculateBranchWidth(FlowNode branch)
        //{
        //    if(branch.Kind!=NodeKinds.Branch)throw new Exception("the kind of node is not branch.");
        //    double minW = 99999, maxW = 0;
        //    var branchLeg  =_model.BranchLeges.FirstOrDefault(x => x.BranchNode == branch);
        //    if (branchLeg != null)
        //    {
        //        Debug.Assert(branchLeg.Points.Count>=2);
        //        foreach (var point in branchLeg.Points)
        //        {
        //            minW = Math.Min(minW, point.Item1.X);
        //            maxW = Math.Max(maxW, point.Item1.X);
        //        }
        //    }

        //    return maxW - minW + 40;
        //}

		private void CreatePorts(FlowNode node, Node item)
		{
            if (node.Kind == NodeKinds.TextBox)
            {
                var port = new AttachmentPort();
                port.Width = 23;
                port.Height = 23;
                port.Tag = PortKinds.Top;
                port.HorizontalAlignment = HorizontalAlignment.Left;
                port.VerticalAlignment = VerticalAlignment.Bottom;
                port.Cursor = Cursors.Cross;
                port.CanCreateLink = true;
                item.Ports.Add(port);
                return;
            }
			foreach (var kind in node.GetPorts())
			{
				var port = new ICSStudio.Diagrams.Controls.EllipsePort();
				port.Width = 10;
				port.Height = 10;
                int top = -3,left=40;

                if (node.Kind == NodeKinds.Transition)
                {
                    left = 35;
                    //top = 0;
                    if (kind == PortKinds.Bottom)
                        top = 75;
                }
                else if (node.Kind == NodeKinds.Step)
                {
                    left = 35;
                    //top = 0;
                    if (kind == PortKinds.Bottom)
                        top = 55;
                }
                else if (node.Kind == NodeKinds.Stop)
                {
                    left = 15;
                }
               
				port.Margin = new Thickness(left, top, 0,0);
				port.Visibility = Visibility.Visible;
                //port.VerticalAlignment = ToVerticalAligment(kind);
                //port.HorizontalAlignment = ToHorizontalAligment(kind);
                port.VerticalAlignment = VerticalAlignment.Top;
                port.HorizontalAlignment = HorizontalAlignment.Left;
                port.CanAcceptIncomingLinks = kind == PortKinds.Top;
				port.CanAcceptOutgoingLinks = !port.CanAcceptIncomingLinks;
				port.Tag = kind;
				port.Cursor = Cursors.Cross;
				port.CanCreateLink = true;
				item.Ports.Add(port);
			}
		}

        private Control CreateAttachedLink(AttachLink link)
        {
            var source = FindPort(link.Source, PortKinds.Top);
            var node = _view.Children.OfType<INode>()
                .FirstOrDefault(x => (FlowNode) (x as Node).ModelElement == link.Target);
            var item=new AttachmentLink(source);
            item.TargetNode = node;
            (source as AttachmentPort).IsAttach = true;
            return item;
        }

		private Control CreateLink(Link link)
		{
            var item = new OrthogonalLink();
            item.ModelElement = link;
            item.EndCap = true;
            item.Source = FindPort(link.Source, link.SourcePort);
            item.Target = FindPort(link.Target, link.TargetPort);

            var b = new Binding("Text");
            b.Source = link;
            item.SetBinding(LinkBase.LabelProperty, b);

            return item;
        }

        private List<Control> CreateBranchLeg(BranchFlowNode branchFlowNode)
        {
            var branch = branchFlowNode.BranchLeg;
            var legs=new List<Control>();
            var branchNode = _view.Items.FirstOrDefault(p => p.ModelElement == branchFlowNode);
            var content= (Branch)(branchNode as Node)?.GetContent();
            //Debug.Assert(content != null);
            //content.Width = CalculateBranchWidth(branch.BranchNode);
            content?.Update(_view);
            foreach (var point in branch.Points)
            {
                var item = new BranchLegLink(branchFlowNode);
                item.ModelElement = branch;
                if (point.Item1.Kind == NodeKinds.Branch)
                {
                    if (_view.Items.OfType<BranchLegLink>()
                            .FirstOrDefault(l =>l.Branch==point.Item1&& l.ConnectedBranchFlowNode == branchFlowNode) != null)continue;
                        item.ConnectedBranchFlowNode = point.Item1 as BranchFlowNode;
                }
                else
                    item.Source = FindPort(point.Item1, point.Item2);
                item.UpdatePath();
                legs.Add(item);
            }

            return legs;
        }
        
		public ICSStudio.Diagrams.Controls.IPort FindPort(FlowNode node, PortKinds portKind)
		{
			var inode = _view.Items.FirstOrDefault(p => p.ModelElement == node) as ICSStudio.Diagrams.Controls.INode;
			if (inode == null)
				return null;
			var port = inode.Ports.OfType<FrameworkElement>().FirstOrDefault(
				p => {
                    if (portKind == PortKinds.Bottom)
                    {
                        if(p.Margin.Top>0)
                            return true;
                        return false;
                    }
                    else
                    {
                        if (p.Margin.Top <= 0) return true;
                        return false;
                    } }
				);
			return (ICSStudio.Diagrams.Controls.IPort)port;
		}

        public bool IsConnected(IPort port)
        {
            var r=_model.BranchLeges.Where(l => l.Points.FirstOrDefault(p => FindPort(p.Item1, p.Item2) == port)!=null).ToList();
            if (r.Count > 0) return true;
            var r2=_model.BranchLeges.Where(x => x.ConnectNodePort == port).ToList();
            if (r2.Count > 0) return true;
            var r3 = _model.Links.Where(l =>
                FindPort(l.Source, l.SourcePort) == port || FindPort(l.Target, l.TargetPort) == port).ToList();
            if (r3.Count > 0) return true;
            return false;
        }

		private VerticalAlignment ToVerticalAligment(PortKinds kind)
		{
			if (kind == PortKinds.Top)
				return VerticalAlignment.Top;
			if (kind == PortKinds.Bottom)
				return VerticalAlignment.Bottom;
			else
				return VerticalAlignment.Center;
		}

		private HorizontalAlignment ToHorizontalAligment(PortKinds kind)
		{
			if (kind == PortKinds.Right)
				return HorizontalAlignment.Right;
			else
				return HorizontalAlignment.Left;
		}

		private void DeleteSelection()
		{
			using (BeginUpdate())
			{
				var nodes = _view.Selection.Select(p => p.ModelElement as FlowNode).Where(p => p != null);
                var ports = GetPortFromNodes(nodes);
				var links = _view.Selection.Select(p => p.ModelElement as Link).Where(p => p != null);
                var branchLinks = _view.Selection.Select(p => p as BranchLegLink).Where(p => p != null);
               
                
                _model.BranchLeges.RemoveRange(p=>nodes.Contains(p.BranchNode));
                foreach (var branchLeg in _model.BranchLeges)
                {
                    branchLeg.Points.RemoveRange(x=>nodes.Contains(x.Item1));
                    if (ports.FirstOrDefault(p=>branchLeg.ConnectNodePort!=null&&p==FindPort(branchLeg.ConnectNodePort.Item1, branchLeg.ConnectNodePort.Item2))!=null) branchLeg.ConnectNodePort = null;
                }
                foreach (var branchLegLink in branchLinks)
                {
                    var branchLeg = branchLegLink.Branch.BranchLeg;
                    branchLeg.Points.RemoveRange(x=>FindPort(x.Item1,x.Item2)==branchLegLink.Source|| nodes.Contains(x.Item1));
                    if (branchLeg.ConnectNodePort!=null&&FindPort(branchLeg.ConnectNodePort.Item1, branchLeg.ConnectNodePort.Item2) == branchLegLink.Source) branchLeg.ConnectNodePort = null;
                }

                foreach (var branchLeg in _model.BranchLeges)
                {
                    if (branchLeg.Points.Count < 2)
                    {
                        _model.Nodes.Remove(branchLeg.BranchNode);
                        //check related branch and link
                        RemoveRelation(branchLeg.BranchNode as BranchFlowNode);
                    }
                }
                _model.AttachLinks.RemoveRange(l => nodes.Contains(l.Source) || nodes.Contains(l.Target));

                _model.Nodes.RemoveRange(p => nodes.Contains(p));
                _model.Links.RemoveRange(p => links.Contains(p));
                _model.Links.RemoveRange(p => nodes.Contains(p.Source) || nodes.Contains(p.Target));
                _model.BranchLeges.RemoveRange(b=>!_model.Nodes.Contains(b.BranchNode));
            }
		}

        private void RemoveRelation(BranchFlowNode removeNode)
        {
            var branchLeg = removeNode.BranchLeg;
            foreach (var point in branchLeg.Points)
            {
                if (point.Item1.Kind == NodeKinds.Branch)
                {
                    var relatedBranchLeg = (point.Item1 as BranchFlowNode).BranchLeg;
                    relatedBranchLeg.Points.Remove(new Tuple<FlowNode, PortKinds>(removeNode, PortKinds.None));
                    if (relatedBranchLeg.Points.Count < 2)
                    {
                        _model.Nodes.Remove(relatedBranchLeg.BranchNode);
                        RemoveRelation(relatedBranchLeg.BranchNode as BranchFlowNode);
                    }
                }
            }
        }

        public IEnumerable<IPort> GetPortFromNodes(IEnumerable<FlowNode> nodes)
        {
            foreach (var node in nodes)
            {
                foreach (var port in node.GetPorts())
                {
                    yield return FindPort(node, port);
                }
            }
        }

		private IDisposable BeginUpdate()
		{
			_updateScope.IsInprogress = true;
			return _updateScope;
		}

        #region IDiagramController Members

        public void UpdateItemsBounds(ICSStudio.Diagrams.Controls.DiagramItem[] items, Rect[] bounds)
        {
            for (int i = 0; i < items.Length; i++)
            {
                var node = items[i].ModelElement as FlowNode;
                if (node != null)
                {
                    node.X = bounds[i].X;
                    node.Y = bounds[i].Y;
                    //node.Column = (int)(bounds[i].X / _view.GridCellSize.Width);
                    //node.Row = (int)(bounds[i].Y / _view.GridCellSize.Height);
                    if (!(items[i].ModelElement is BranchFlowNode))
                    {
                        foreach (var port in (items[i] as Node).Ports)
                        {
                            var branchNode = GetBranchPortLink(port);
                            (branchNode?.GetContent() as Branch)?.Update(_view);
                        }
                    }
                }
            }
        }

        public void UpdateItems(DiagramItem[] items,Vector vector)
        {
            vector = AdjustVector(vector);
            foreach (var t in items)
            {
                var node = t.ModelElement as FlowNode;
                if (node != null)
                {
                    node.X = node.X + vector.X;
                    node.Y = node.Y + vector.Y;
                    //if (!(t.ModelElement is BranchFlowNode))
                    //{
                    //    foreach (var port in (t as Node).Ports)
                    //    {
                    //        var branchNode = GetBranchPortLink(port);
                    //        var branch = (branchNode?.GetContent() as Branch);
                    //        if (branch != null)
                    //        {
                    //            branch?.Update(_view);
                    //            UpdateRelatedBranchLinks(branch.BranchLeg.BranchNode);
                    //        }
                    //    }
                    //}
                    UpdateView();
                }
            }
        }

        public Vector AdjustVector(Vector vector)
        {
            double x=vector.X, y=vector.Y;
            if (Math.Abs(x) <= 10) x = 0;
            if (Math.Abs(y) <= 10) y = 0;
            if (x < 0)
            {
                x = -Math.Ceiling(Math.Abs(x )/ 20) * 20;
            }
            else
            {
                x = Math.Ceiling(x / 20) * 20;
            }

            if (y < 0)
            {
                y = -Math.Ceiling(Math.Abs(y) / 20) * 20;
            }
            else
            {
                y = Math.Ceiling(y / 20) * 20;
            }
           
            return new Vector(x,y);
        }

        public Tuple<FlowNode, PortKinds> ChangePort(IPort port)
        {
            var node=_view.Items.OfType<INode>().FirstOrDefault(n => n.Ports.FirstOrDefault(p => p == port) != null);
            if (node != null)
            {
                return new Tuple<FlowNode, PortKinds>((FlowNode)(node as Node).ModelElement,port.IsTop?PortKinds.Top:PortKinds.Bottom);
            }
            return null;
        }

		public void UpdateLink(LinkInfo initialState, ICSStudio.Diagrams.Controls.ILink link)
		{
			using (BeginUpdate())
			{
                if (link.Branch!=null)
                {
                    var port = link.Source ?? link.Target;
                    var branch = link.Branch;
                    branch.BranchLeg.ConnectNodePort = ChangePort(port);
                    UpdateView();
                    return;
                }

                if (link is AttachmentLink)
                {
                    if(link.TargetPoint==null)return;
                    var point = (Point)link.TargetPoint;
                    var nodes = _view.Children.OfType<INode>();
                    var node = nodes.FirstOrDefault(x => x.IsInNode(point));
                    if (node != null)
                    {
                        (link as AttachmentLink).TargetNode = node;
                        var sourceFlowNode =
                            nodes.FirstOrDefault(x => x.Ports.FirstOrDefault(p => p == link.Source) != null);
                        _model.AttachLinks.Add(new AttachLink((FlowNode) (sourceFlowNode as Node).ModelElement,
                            (FlowNode) (node as Node).ModelElement));
                        UpdateView();
                    }
                    return;
                }
				var sourcePort = link.Source as PortBase;
				var source = VisualHelper.FindParent<Node>(sourcePort);
				var targetPort = link.Target as PortBase;
				var target = VisualHelper.FindParent<Node>(targetPort);
                
                BranchLeg branchLeg= GetBranchLeg(target,targetPort);
                if (branchLeg == null)
                {
                    branchLeg = GetBranchLeg(source, sourcePort);
                    var temp = sourcePort;
                    sourcePort = targetPort;
                    targetPort = temp;
                    var temp2 = source;
                    source = target;
                    target = temp2;
                }

                var branchType = GetBranchType(source);
                if ((PortKinds)sourcePort.Tag == (PortKinds)targetPort.Tag)
                {
                    
                    if (branchLeg != null)
                    {
                        branchLeg.AddLeg(new Tuple<FlowNode, PortKinds>((FlowNode)source.ModelElement, (PortKinds)sourcePort.Tag));
                        var item = new BranchLegLink(branchLeg.BranchNode as BranchFlowNode);
                        item.ModelElement = branchLeg.BranchNode;
                        item.Source = FindPort((FlowNode)source.ModelElement, (PortKinds)sourcePort.Tag);
                        _view.Children.Add(item);
                    }
                    else
                    {
                        BranchFlowNode newBranch =new BranchFlowNode(NodeKinds.Branch,((PortKinds)sourcePort.Tag==PortKinds.Top?BranchFlowType.Diverge:BranchFlowType.Converge), branchType);
                        newBranch.X = Math.Min(((FlowNode) source.ModelElement).X, ((FlowNode) target.ModelElement).X)+20;
                        if ((PortKinds) sourcePort.Tag == PortKinds.Bottom)
                        {
                            newBranch.Y = Math.Max(sourcePort.Center.Y,
                                              sourcePort.Center.Y) + 40;
                        }
                        else
                        {
                            newBranch.Y = Math.Min(sourcePort.Center.Y,
                                              sourcePort.Center.Y) - 40;
                        }
                        BranchLeg newBranchLeg= newBranch.BranchLeg;
                        newBranchLeg.AddLeg(new Tuple<FlowNode, PortKinds>((FlowNode)source.ModelElement, (PortKinds)sourcePort.Tag));
                        newBranchLeg.AddLeg(new Tuple<FlowNode, PortKinds>((FlowNode)target.ModelElement, (PortKinds)targetPort.Tag));
                        _model.Nodes.Add(newBranch);
                        _model.BranchLeges.Add(newBranchLeg);
                    }
                }
                else
                {
                    _model.Links.Remove((link as LinkBase).ModelElement as Link);
                    if (branchLeg != null)
                    {
                        var newBranch=new BranchFlowNode(NodeKinds.Branch,BranchFlowType.Converge, branchType);
                        newBranch.Y = Math.Ceiling((sourcePort.Center.Y + targetPort.Center.Y) / 2 / 20) * 20;
                        var newBranchLeg = newBranch.BranchLeg;
                        newBranchLeg.AddLeg(new Tuple<FlowNode, PortKinds>((FlowNode)source.ModelElement,(PortKinds)sourcePort.Tag));
                        branchLeg.DeleteLeg(new Tuple<FlowNode, PortKinds>((FlowNode)target.ModelElement,(PortKinds)targetPort.Tag));
                        branchLeg.AddLeg(new Tuple<FlowNode, PortKinds>(newBranch,PortKinds.None));
                        newBranchLeg.ConnectNodePort=new Tuple<FlowNode, PortKinds>((FlowNode)target.ModelElement, (PortKinds)targetPort.Tag);
                        _model.Nodes.Add(newBranch);
                        _model.BranchLeges.Add(newBranchLeg);
                    }
                    else
                        _model.Links.Add(
                        new Link((FlowNode)source.ModelElement, (PortKinds)sourcePort.Tag,
                            (FlowNode)target.ModelElement, (PortKinds)targetPort.Tag)
                    );
                }
			}
		}

        public BranchType GetBranchType(Node node)
        {
            var flowNode = (FlowNode) node.ModelElement;
            if(flowNode is BranchFlowNode)
                throw new ChartException("GetBranchType:error node type.");
            if (flowNode.Kind == NodeKinds.Step||flowNode.Kind==NodeKinds.Stop)
            {
                return BranchType.Simultaneous;
            }

            return BranchType.Selection;
        }
        
        public BranchLeg GetBranchLeg(Node node, PortBase port)
        {
            if (node == null || port == null) return null;
            var l = _model.BranchLeges.FirstOrDefault(x =>
                x.Points.FirstOrDefault(p =>
                    (p.Item1 == (FlowNode) node.ModelElement && p.Item2 == (PortKinds?)port.Tag)) != null);
            return l;
        }

        public INode GetNode(IPort port)
        {
            var node = _view.Children.OfType<INode>().FirstOrDefault(x => x.Ports.Contains(port));
            return node;
        }

		public void ExecuteCommand(System.Windows.Input.ICommand command, object parameter)
		{
			if (command == ApplicationCommands.Delete)
				DeleteSelection();
		}

		public bool CanExecuteCommand(System.Windows.Input.ICommand command, object parameter)
		{
			if (command == ApplicationCommands.Delete)
				return true;
			else
				return false;
		}

		#endregion
	}
}
