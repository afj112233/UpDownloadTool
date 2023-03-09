using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ICSStudio.Diagrams.Controls
{
	public interface INode
	{
		IEnumerable<IPort> Ports { get; }
        bool IsInBranch(Point point);
        bool IsInNode(Point point);
    }
}
