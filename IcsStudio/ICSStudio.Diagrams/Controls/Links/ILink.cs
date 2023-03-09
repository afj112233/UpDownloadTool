using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ICSStudio.Diagrams.Chart;

namespace ICSStudio.Diagrams.Controls
{
	public interface ILink
	{
		IPort Source { get; set; }
		IPort Target { get; set; }
		Point? SourcePoint { get; set; }
		Point? TargetPoint { get; set; }
        Branch Branch { set; get; }

		void UpdatePath();
	}
}
