using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSStudio.Diagrams.Tools;
using ICSStudio.Diagrams.Controls;
using System.Windows;
using ICSStudio.Diagrams;
using System.Windows.Controls;

namespace ICSStudio.Diagram.Flowchart
{
	class CustomLinkTool: LinkTool
	{
		public CustomLinkTool(DiagramView view)
			: base(view)
		{
		}

		protected override ILink CreateNewLink(IPort port)
		{
            var link = new OrthogonalLink();
			BindNewLinkToPort(port, link);
			return link;
		}

		protected override void UpdateLink(Point point, IPort port)
		{
			base.UpdateLink(point, port);
			var link = Link as OrthogonalLink;
		}
	}
}
