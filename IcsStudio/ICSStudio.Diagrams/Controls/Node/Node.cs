using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using ICSStudio.Diagrams.Adorners;
using ICSStudio.Diagrams.Flowchart.Model;
using ICSStudio.Diagram.Flowchart;

namespace ICSStudio.Diagrams.Controls
{
	public class Node : DiagramItem, INode
    {
        private object _content;
		static Node()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
				typeof(Node), new FrameworkPropertyMetadata(typeof(Node)));
		}

        public object GetContent()
        {
            return _content;
        }
        

		#region Properties

		#region Content Property

		public object Content 
		{
			get { return (bool)GetValue(ContentProperty); }
            set
            {
                if (value == null&& _content!=null)
                {
                    ((Control)_content).GotFocus -= Node_GotFocus;
                }
                _content = value;
                if (value != null)
                {
                    ((Control)_content).GotFocus += Node_GotFocus;
                }
                SetValue(ContentProperty, value);
            }
		}

        private void Node_GotFocus(object sender, RoutedEventArgs e)
        {
            IsSelected=true;
        }

        public static readonly DependencyProperty ContentProperty =
			DependencyProperty.Register("Content",
									   typeof(object),
									   typeof(Node));

		#endregion

		#region CanResize Property

		public bool CanResize
		{
			get { return (bool)GetValue(CanResizeProperty); }
			set { SetValue(CanResizeProperty, value); }
		}

		public static readonly DependencyProperty CanResizeProperty =
			DependencyProperty.Register("CanResize",
									   typeof(bool),
									   typeof(Node),
									   new FrameworkPropertyMetadata(true));

		#endregion

		private List<IPort> _ports = new List<IPort>();
		public ICollection<IPort> Ports { get { return _ports; } }
        public bool IsInBranch(Point point)
        {
            if (ModelElement is BranchFlowNode)
            {
                var branchNode = (FlowNode) ModelElement;
                if (point.X >= branchNode.X && point.X <= branchNode.X + ActualWidth && point.Y >= branchNode.Y &&
                    point.Y <= branchNode.Y + 20)
                    return true;
                return false;
            }

            return false;
        }

        public bool IsInNode(Point point)
        {
            if (point.X >= Bounds.X && point.X <= Bounds.X + ActualWidth && point.Y >= Bounds.Y &&
                point.Y <= Bounds.Y +ActualHeight)
            {
                return true;
            }
            return false;
        }

        public override Rect Bounds
		{
			get
			{
				//var itemRect = VisualTreeHelper.GetDescendantBounds(item);
				//return item.TransformToAncestor(this).TransformBounds(itemRect);
				var x = Canvas.GetLeft(this);
				var y = Canvas.GetTop(this);
				return new Rect(x, y, ActualWidth, ActualHeight);
			}
		}

		#endregion

		public Node()
		{
		}
        

		public void UpdatePosition()
		{
			foreach (var p in Ports)
				p.UpdatePosition();
		}

		protected override Adorner CreateSelectionAdorner()
		{
			return new SelectionAdorner(this, new SelectionFrame());
		}

		#region INode Members

		IEnumerable<IPort> INode.Ports
		{
			get { return _ports; }
		}

		#endregion
	}
}
