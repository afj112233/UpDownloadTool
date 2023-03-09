using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ICSStudio.Diagrams.Controls
{
	public interface IPort
	{
		ICollection<ILink> Links { get; }
		Point Center { get; }

		bool IsNear(Point point);
        bool IsTop { get; }

        Point GetEdgePoint(Point target);
		void UpdatePosition();
	}
}
