using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ICSStudio.Diagrams.Controls;

namespace ICSStudio.Diagrams.Tools
{
	public interface ILinkTool
	{
		void BeginDrag(Point start, ILink link, LinkThumbKind thumb);
		void BeginDragNewLink(Point start, IPort port);
		void DragTo(Vector vector);
		bool CanDrop();
		void EndDrag(bool doCommit);
	}
}
