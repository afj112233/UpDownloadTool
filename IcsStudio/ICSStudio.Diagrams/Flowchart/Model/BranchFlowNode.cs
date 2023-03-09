using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Diagram.Flowchart;
using ICSStudio.SimpleServices.Chart;

namespace ICSStudio.Diagrams.Flowchart.Model
{
    
    public class BranchFlowNode:FlowNode
    {

        public BranchFlowNode(NodeKinds kind,BranchFlowType branchFlow,BranchType branchType) : base(kind)
        {
            BranchLeg=new BranchLeg(this);
            BranchFlow = branchFlow;
            BranchType = branchType;
        }

        public BranchType BranchType { get; private set; }

        public BranchLeg BranchLeg { get; }
        
        public BranchFlowType BranchFlow { get; private set; }

        public bool CanExist()
        {
            if (BranchLeg.Points.Count < 2)
            {
                return false;
            }

            return true;
        }
    }
}
