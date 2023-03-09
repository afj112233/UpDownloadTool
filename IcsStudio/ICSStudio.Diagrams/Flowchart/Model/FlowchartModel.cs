using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ICSStudio.Diagrams.Flowchart.Model;

namespace ICSStudio.Diagram.Flowchart
{
	public class FlowchartModel
	{
		private ObservableCollection<FlowNode> _nodes = new ObservableCollection<FlowNode>();
        public ObservableCollection<FlowNode> Nodes
		{
			get { return _nodes; }
		}

		private ObservableCollection<Link> _links = new ObservableCollection<Link>();
        public ObservableCollection<Link> Links
		{
			get { return _links; }
		}

        private ObservableCollection<AttachLink> _attachLinks = new ObservableCollection<AttachLink>();
        public ObservableCollection<AttachLink> AttachLinks
        {
            get { return _attachLinks; }
        }

        private ObservableCollection<BranchLeg> _branchLeges=new ObservableCollection<BranchLeg>();
        public ObservableCollection<BranchLeg> BranchLeges
        {
            get { return _branchLeges; }
        }
	}
}
