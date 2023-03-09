using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using ICSStudio.Diagram.Flowchart;
using ICSStudio.Diagrams.Flowchart.Model;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Chart;
using ICSStudio.SimpleServices.Common;
using Branch = ICSStudio.SimpleServices.Chart.Branch;
using Step = ICSStudio.SimpleServices.Chart.Step;
using Stop = ICSStudio.SimpleServices.Chart.Stop;
using TextBox = ICSStudio.SimpleServices.Chart.TextBox;

namespace ICSStudio.EditorPackage.FlowChartEditor
{
    public class FlowChartEditorViewModel:ViewModelBase,IEditorPane
    {
        public FlowChartEditorViewModel(IRoutine routine, UserControl userControl)
        {
            Routine = routine as SFCRoutine;
            //Contract.Assert(Routine != null);
            Control = userControl;
            userControl.DataContext = this;

            var model = ConvertToModel();

            TopControl = new FlowchartEditor(model);
            BottomControl=new FlowchartEditor(model);
            //TopControl = new LadderEditor
            //{
            //    Document = Document,
            //    Options = Options,
            //    FontFamily = new FontFamily("Consolas"),
            //    FontSize = 12
            //};
            //BottomControl = new LadderEditor
            //{
            //    Document = Document,
            //    Options = Options,
            //    FontFamily = new FontFamily("Consolas"),
            //    FontSize = 12
            //};
        }
        public SFCRoutine Routine { get; }
        public string Caption => $"{Routine.ParentCollection.ParentProgram?.Name} - {Routine.Name}";
        public UserControl Control { get; }
        public Action CloseAction { get; set; }
        public Action<string> UpdateCaptionAction { get; set; }

        public object TopControl { get; }
        public object BottomControl { get; }

        public FlowchartModel ConvertToModel()
        {
            var model=new FlowchartModel();
            var sfcRoutine = Routine as SFCRoutine;
            if (sfcRoutine == null) return model;
           
            var nodes = sfcRoutine.Contents.Where(c => c is Step || c is Transition || c is Stop||c is TextBox);
            var branches = sfcRoutine.Contents.Where(c => c is Branch);
            var links = sfcRoutine.Contents.Where(c => c is ILink).ToList();

            foreach (var content in nodes)
            {
                if (content is Step)
                {
                    var stepContent = content as Step;
                    var step = new FlowNode(NodeKinds.Step);
                    step.Content = content;
                    step.ID = stepContent.ID;
                    step.X = stepContent.X;
                    step.Y = stepContent.Y;
                    model.Nodes.Add(step);
                }

                if (content is Transition)
                {
                    var transitionContent = content as Transition;
                    var transition = new FlowNode(NodeKinds.Transition);
                    transition.Content = content;
                    transition.ID = transitionContent.ID;
                    transition.X = transitionContent.X;
                    transition.Y = transitionContent.Y;
                    model.Nodes.Add(transition);
                }

                if (content is Stop)
                {
                    var stopContent = content as Stop;
                    var stop=new FlowNode(NodeKinds.Stop);
                    stop.Content = content;
                    stop.ID = stopContent.ID;
                    stop.X = stopContent.X + 20;
                    stop.Y = stopContent.Y;
                    model.Nodes.Add(stop);
                }

                if (content is TextBox)
                {
                    var textBoxContent = content as TextBox;
                    var textBox=new FlowNode(NodeKinds.TextBox);
                    textBox.Content = content;
                    textBox.ID = textBoxContent.ID;
                    textBox.X = textBoxContent.X;
                    textBox.Y = textBoxContent.Y;
                    model.Nodes.Add(textBox);
                }
            }
            //add branch
            foreach (var content in branches)
            {
                var branchContent = content as Branch;
                var branch = new BranchFlowNode(NodeKinds.Branch,branchContent.BranchFlow, branchContent.BranchType);
                branch.ID = branchContent.ID;
                branch.Y = branchContent.Y;
                var leg = branch.BranchLeg;
                model.Nodes.Add(branch);
                model.BranchLeges.Add(leg);
            }
            //add branch legs
            foreach (var content in branches)
            {
                var branchContent = content as Branch;
                var branch = model.Nodes.OfType<BranchFlowNode>().FirstOrDefault(n => n.ID == branchContent.ID);
                var leg = branch.BranchLeg;
                var portKind = branchContent.BranchFlow == BranchFlowType.Converge ? PortKinds.Bottom : PortKinds.Top;
                foreach (var branchContentLeg in branchContent.Legs)
                {
                    var link = links.OfType<DirectedLink>().FirstOrDefault(l => l.FromID == branchContentLeg);
                    if(link==null)continue;
                    //Debug.Assert(link!=null, branchContentLeg.ToString());
                    var node = model.Nodes.FirstOrDefault(n => n.ID == link.ToID);
                    if (node == null)
                    {
                        var branchOfLeg=branches.FirstOrDefault(b => (b as Branch).Legs.Contains(link.ToID)) as Branch;
                        if (branchOfLeg != null)
                        {
                            node = model.Nodes.FirstOrDefault(n => n.ID == branchOfLeg.ID);
                        }
                    }
                    
                    Debug.Assert(node!=null, link.ToID.ToString());
                    leg.AddLeg(node.Kind == NodeKinds.Branch
                        ? new Tuple<FlowNode, PortKinds>(node, PortKinds.None)
                        : new Tuple<FlowNode, PortKinds>(node, portKind));
                    links.Remove(link);
                }
            }

            foreach (var link in links)
            {
                if (link is DirectedLink)
                {
                    var d = link as DirectedLink;
                    var fromNode = model.Nodes.FirstOrDefault(n => n.ID == d.FromID);
                    var toNode = model.Nodes.FirstOrDefault(n => n.ID == d.ToID);
                    if (toNode == null)
                    {
                        var branchOfLeg = branches.FirstOrDefault(b => (b as Branch).Legs.Contains(d.ToID)) as Branch;
                        if (branchOfLeg != null)
                        {
                            toNode = model.Nodes.FirstOrDefault(n => n.ID == branchOfLeg.ID);
                            var toBranchNode = toNode as BranchFlowNode;
                            var kind = toBranchNode.BranchFlow == BranchFlowType.Diverge
                                ? PortKinds.Top
                                : PortKinds.Bottom;
                            (toNode as BranchFlowNode).BranchLeg.AddLeg(new Tuple<FlowNode, PortKinds>(fromNode, kind));
                            continue;
                        }
                    }
                    Debug.Assert(fromNode != null && toNode != null,$"fromId:{d.FromID}----toId:{d.ToID}");
                    if (fromNode.Kind == NodeKinds.Branch)
                    {
                        var fromBranchNode = fromNode as BranchFlowNode;
                        var kind = fromBranchNode.BranchFlow == BranchFlowType.Diverge
                            ? PortKinds.Bottom
                            : PortKinds.Top;
                        if (toNode.Kind == NodeKinds.Branch)
                        {
                            //fromBranchNode.BranchLeg.ConnectNodePort = new Tuple<FlowNode, PortKinds>(toNode, kind);
                            fromBranchNode.BranchLeg.AddLeg(new Tuple<FlowNode, PortKinds>(toNode,PortKinds.None));
                        }
                        else
                        {
                            fromBranchNode.BranchLeg.ConnectNodePort = new Tuple<FlowNode, PortKinds>(toNode, kind);
                        }
                        
                        continue;
                    }
                    if (toNode.Kind == NodeKinds.Branch)
                    {
                        var toBranchNode = toNode as BranchFlowNode;
                        var kind = toBranchNode.BranchFlow == BranchFlowType.Diverge
                            ? PortKinds.Bottom
                            : PortKinds.Top;

                        toBranchNode.BranchLeg.ConnectNodePort = new Tuple<FlowNode, PortKinds>(fromNode,kind);
                        continue;
                    }
                    model.Links.Add(new Link(fromNode,PortKinds.Bottom,toNode,PortKinds.Top));
                }

                if (link is Attachment)
                {
                    var a = link as Attachment;
                    var textBoxNode = model.Nodes.FirstOrDefault(n=>n.ID==a.FromID);
                    var toNode = model.Nodes.FirstOrDefault(n => n.ID == a.ToID);
                    Debug.Assert(textBoxNode != null && toNode != null, $"fromId:{a.FromID}----toId:{a.ToID}");
                    var attachment=new AttachLink(textBoxNode,toNode);
                    model.AttachLinks.Add(attachment);
                }
            }

            return model;
        }

        //private void CheckBranch(FlowchartModel model,IEnumerable<Branch> branches)
        //{
        //    var changeList=new List<Branch>();
        //    foreach (var branch in branches)
        //    {
        //        branch.Legs.FirstOrDefault(l=>branches.FirstOrDefault(b=>b.Legs.Contains(l)) != null)
        //    }
        //}
    }
}
