using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICSStudio.Diagram.Flowchart;
using ICSStudio.Diagrams.Flowchart.Model;
using ICSStudio.SimpleServices.Chart;

namespace ICSStudio.Diagrams.Flowchart
{
    /// <summary>
    /// EditorTest.xaml 的交互逻辑
    /// </summary>
    public partial class EditorTest : UserControl
    {
        private ItemsControlDragHelper _dragHelper;

        public EditorTest(FlowchartModel model)
        {
            InitializeComponent();

            //var model = CreateModel();
            _editor.Controller = new Controller(_editor, model);
            _editor.DragDropTool = new DragDropTool(_editor, model);
            _editor.DragTool = new CustomMoveResizeTool(_editor, model)
            {
                MoveGridCell = _editor.GridCellSize
            };
            _editor.LinkTool = new CustomLinkTool(_editor);
            _editor.Selection.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Selection_PropertyChanged);
            _dragHelper = new ItemsControlDragHelper(_toolbox, this);
            //var stackPanel = VisualHelper.FindParent<StackPanel>(_toolbox);
            //if (stackPanel != null) stackPanel.Visibility = Visibility.Collapsed;
            FillToolbox();
        }

        public EditorTest()
        {
            InitializeComponent();

            var model = CreateModel();
            _editor.Controller = new Controller(_editor, model);
            _editor.DragDropTool = new DragDropTool(_editor, model);
            _editor.DragTool = new CustomMoveResizeTool(_editor, model)
            {
                MoveGridCell = _editor.GridCellSize
            };
            _editor.LinkTool = new CustomLinkTool(_editor);
            _editor.Selection.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Selection_PropertyChanged);
            _dragHelper = new ItemsControlDragHelper(_toolbox, this);

            FillToolbox();
        }

        private void FillToolbox()
        {
            foreach (NodeKinds nk in Enum.GetValues(typeof(NodeKinds)))
            {
                var node = new FlowNode(nk);
                node.Text = nk.ToString();
                //var ui = Controller.CreateContent(node);
                var ui = Controller.CreateIcon(node);
                if (ui == null)
                {
                    continue;
                }

                //ui.Width = 60;
                //ui.Height = 30;
                ui.Margin = new Thickness(5);
                ui.Tag = nk;
                _toolbox.Items.Add(ui);
            }
        }

        private FlowchartModel CreateModel()
        {
            var model = new FlowchartModel();
            //var subroutine=new FlowNode(NodeKinds.SubroutineOrReturn);
            //subroutine.X = 200;
            //subroutine.Y = 200;
            //model.Nodes.Add(subroutine);

            var step = new FlowNode(NodeKinds.Step);
            step.X = 180 - step.Offset;
            step.Y = 60;
            model.Nodes.Add(step);

            var cross2 = new FlowNode(NodeKinds.Transition);
            cross2.X = 120 - cross2.Offset;
            cross2.Y = 180;
            model.Nodes.Add(cross2);

            var cross = new FlowNode(NodeKinds.Transition);
            cross.X = 260 - cross.Offset;
            cross.Y = 180;
            model.Nodes.Add(cross);

            //         //model.Links.Add(new Link(cross,PortKinds.Top,cross2,PortKinds.Top));
            //         //model.Links.Add(new Link(cross, PortKinds.Bottom, cross2, PortKinds.Bottom));
            //         //var stop = new FlowNode(NodeKinds.Stop);
            //         //stop.X = 60 - stop.Offset;
            //         //stop.Y = 160;
            //         //model.Nodes.Add(stop);

            //         //model.Links.Add(new Link(step,PortKinds.Bottom, cross2,PortKinds.Top));
            //         //model.Links.Add(new Link(cross2,PortKinds.Bottom,stop,PortKinds.Top));

            //         //model.Links.Add(new Link(cond, PortKinds.Bottom, ac1, PortKinds.Top) { Text = "True" });
            //         //model.Links.Add(new Link(cond, PortKinds.Right, end, PortKinds.Top) { Text = "False" });

            var branch = new BranchFlowNode(NodeKinds.Branch,BranchFlowType.Diverge,BranchType.Selection);
            //branch.X = 0;
            branch.Y = 140;
            model.Nodes.Add(branch);

            //         var stop=new FlowNode(NodeKinds.Stop);
            //         stop.X = 80;
            //         stop.Y = 280;
            //         model.Nodes.Add(stop);

            var branchLeg = branch.BranchLeg;
            branchLeg.AddLeg(new Tuple<FlowNode, PortKinds>(cross2, PortKinds.Top));
            branchLeg.AddLeg(new Tuple<FlowNode, PortKinds>(cross, PortKinds.Top));
            model.BranchLeges.Add(branchLeg);
            //         branchLeg.ConnectNodePort=new Tuple<FlowNode, PortKinds>(stop,PortKinds.Top);
            return model;
        }

        void Selection_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var p = _editor.Selection.Primary;
            //_propertiesView.SelectedObject = p != null ? p.ModelElement : null;
        }

    }
}
