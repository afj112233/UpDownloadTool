using System;
using System.Windows;
using ICSStudio.Diagrams.Controls;

namespace ICSStudio.Diagrams.Tools
{
	public interface IMoveResizeTool
	{
		void BeginDrag(Point start, DiagramItem item, DragThumbKinds kind);
		void DragTo(Vector vector);
		bool CanDrop();
		void EndDrag(bool doCommit,Vector vector);
	}
}
