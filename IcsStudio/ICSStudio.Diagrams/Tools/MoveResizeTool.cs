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
using ICSStudio.Diagrams.Flowchart;
using ICSStudio.Diagrams.Flowchart.Model;
using ICSStudio.Diagram.Flowchart;

namespace ICSStudio.Diagrams.Tools
{
	public class MoveResizeTool : IMoveResizeTool
	{
		public DragThumbKinds DragKind { get; protected set; }
		public Rect[] InitialBounds { get; protected set; }
		public DiagramItem[] DragItems { get; protected set; }
		//public bool SnapToGrid { get; set; }
		public Size MoveGridCell { get; set; }
		public Size ResizeGridCell { get; set; }

		protected DiagramView View { get; private set; }
		protected IDiagramController Controller { get { return View.Controller; } }
		protected Point Start { get; set; }

		public MoveResizeTool(DiagramView view)
		{
			View = view;
		}

		public virtual void BeginDrag(Point start, DiagramItem item, DragThumbKinds kind)
		{
			Start = start;
			DragKind = kind;
			if (kind == DragThumbKinds.Center)
			{
				if (!item.CanMove || !IsMovable(item))
					return;
				if (!View.Selection.Contains(item))
					View.Selection.Set(item);
				DragItems = View.Selection.Where(p => p.CanMove && IsMovable(p)).ToArray();
			}
			else
			{
				DragItems = new DiagramItem[] { item };
			}
			InitialBounds = DragItems.Select(p => p.Bounds).ToArray();
			View.DragAdorner = CreateAdorner();
		}

		protected bool IsMovable(DiagramItem item)
		{
			return !(item is LinkBase);
		}

		public virtual void DragTo(Vector vector)
		{
			vector = UpdateVector(vector);
			for (int i = 0; i < DragItems.Length; i++)
			{
				var item = DragItems[i];
				var rect = InitialBounds[i];
                if (vector.Y > 0)
                {

                }
                if (((FlowNode) item.ModelElement).Kind != NodeKinds.Branch)
                    Canvas.SetLeft(item, rect.X + vector.X);
                Canvas.SetTop(item, rect.Y + vector.Y);
                var link = View.Children.OfType<AttachmentLink>().FirstOrDefault(l => l.TargetNode.Equals(item));
                link?.UpdatePath();
                //if (!(item.ModelElement is BranchFlowNode))
                //{
                //    foreach (var port in (item as Node).Ports)
                //    {
                //        var branchNode = (Controller as Controller)?.GetBranchPortLink(port);
                //        if (branchNode != null)
                //        {
                //            Console.WriteLine("2222222222:---" + (branchNode.GetContent() as Branch).Width);
                //            (branchNode.GetContent() as Branch)?.Update();
                //        }
                //    }
                //}
            }
            
        }

		public virtual bool CanDrop()
		{
			return true;
		}
		
		public virtual void EndDrag(bool doCommit,Vector vector)
		{
			if (doCommit)
			{
                //var bounds = DragItems.Select(p => p.Bounds).ToArray();
                //Controller.UpdateItemsBounds(DragItems, bounds);
                
                Controller.UpdateItems(DragItems, vector);
            }
			else
			{
				RestoreBounds();
			}
			DragItems = null;
			InitialBounds = null;
		}

		protected virtual Adorner CreateAdorner()
		{
			return new MoveResizeAdorner(View, Start) { Cursor = GetCursor() };
		}

		protected Cursor GetCursor()
		{
			switch (DragKind)
			{
				case DragThumbKinds.Center:
					return Cursors.SizeAll;
				case DragThumbKinds.Bottom:
				case DragThumbKinds.Top:
					return Cursors.SizeNS;
				case DragThumbKinds.Left:
				case DragThumbKinds.Right:
					return Cursors.SizeWE;
				case DragThumbKinds.TopLeft:
				case DragThumbKinds.BottomRight:
					return Cursors.SizeNWSE;
				case DragThumbKinds.TopRight:
				case DragThumbKinds.BottomLeft:
					return Cursors.SizeNESW;
			}
			return null;
		}

		protected virtual Vector UpdateVector(Vector vector)
		{
			Size cell;
			if (DragKind == DragThumbKinds.Center)
				cell = MoveGridCell;
			else
				cell = ResizeGridCell;

			if (cell.Width > 0 && cell.Height > 0)
			{
				var x = Math.Round(vector.X / cell.Width) * cell.Width;
				var y = Math.Round(vector.Y / cell.Height) * cell.Height;
				return new Vector(x, y);
			}
			else
				return vector;
		}

		protected virtual void RestoreBounds()
		{
			for (int i = 0; i < DragItems.Length; i++)
			{
				var item = DragItems[i];
				var rect = InitialBounds[i];
				Canvas.SetLeft(item, rect.X);
				Canvas.SetTop(item, rect.Y);
				item.Width = rect.Width;
				item.Height = rect.Height;
			}
		}
	}
}
