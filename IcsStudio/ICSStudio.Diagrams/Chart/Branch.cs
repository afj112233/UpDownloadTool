using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using ICSStudio.Diagrams.Controls;
using ICSStudio.Diagrams.Flowchart;
using ICSStudio.Diagrams.Flowchart.Model;
using ICSStudio.Diagram.Flowchart;
using ICSStudio.Diagrams.Exceptions;
using ICSStudio.SimpleServices.Chart;

namespace ICSStudio.Diagrams.Chart
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:ICSStudio.Diagrams.Chart"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:ICSStudio.Diagrams.Chart;assembly=ICSStudio.Diagrams.Chart"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误: 
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:Branch/>
    ///
    /// </summary>
    public class Branch : Control
    {
        static Branch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Branch), new FrameworkPropertyMetadata(typeof(Branch)));
        }

        public Branch(BranchLeg branchLeg)
        {
            BranchLeg = branchLeg;
            Width = 120;
            if ((BranchLeg.BranchNode as BranchFlowNode).BranchType == BranchType.Selection)
            {
                SingleVisibility = Visibility.Visible;
                SimultaneousVisibility = Visibility.Collapsed;
            }
            else
            {
                SingleVisibility = Visibility.Collapsed;
                SimultaneousVisibility = Visibility.Visible;
            }
        }

        public static readonly DependencyProperty SingleVisibilityProperty = DependencyProperty.Register(
            "SingleVisibility", typeof(Visibility), typeof(Branch), new PropertyMetadata(System.Windows.Visibility.Visible));

        public Visibility SingleVisibility
        {
            get { return (Visibility) GetValue(SingleVisibilityProperty); }
            set { SetValue(SingleVisibilityProperty, value); }
        }

        public static readonly DependencyProperty SimultaneousVisibilityProperty = DependencyProperty.Register(
            "SimultaneousVisibility", typeof(Visibility), typeof(Branch), new PropertyMetadata(System.Windows.Visibility.Collapsed));

        public Visibility SimultaneousVisibility
        {
            get { return (Visibility) GetValue(SimultaneousVisibilityProperty); }
            set { SetValue(SimultaneousVisibilityProperty, value); }
        }

        public void Update(DiagramView view)
        {
            Point offset = new Point(99999, 0);
            if (BranchLeg.Points.Count < 2)return;
            double minW = 99999, maxW = 0;
           if(BranchLeg == null)
               throw new ChartException("Branch:branchLeg is null.");
            var min = BranchLeg.GetMinPoint(BranchLeg.BranchNode as BranchFlowNode);
            var max = BranchLeg.GetMaxPoint(BranchLeg.BranchNode as BranchFlowNode);
            offset.X = min.X;
            minW = min.X;
            maxW = max.X;
            //foreach (var point in BranchLeg.Points)
            //{
            //    if (point.Item1.Kind == NodeKinds.Branch)
            //    {
            //        //var branchFlowNode = point.Item1 as BranchFlowNode;
            //        //var min = branchFlowNode.BranchLeg.GetMinPoint();
            //        //var max = branchFlowNode.BranchLeg.GetMaxPoint();
            //        //offset.X = Math.Min(offset.X, min.X);
            //        //minW = Math.Min(minW, min.X);
            //        //maxW = Math.Max(maxW, max.X);
            //        continue;
            //    };
            //    offset.X = Math.Min(offset.X, point.Item1.X);
            //    minW = Math.Min(minW, point.Item1.X);
            //    maxW = Math.Max(maxW, point.Item1.X);
            //}
            
            if (BranchLeg.ConnectNodePort != null)
            {
                var pointX =BranchLeg.ConnectNodePort.Item1.FindPortX();
                //point=point+new Vector(-40,0);
                offset.X = Math.Min(offset.X, pointX);
                minW = Math.Min(minW, pointX);
                maxW = Math.Max(maxW, pointX);
            }

            //if (Math.Abs(BranchLeg.BranchNode.X - (offset.X)) > double.Epsilon)
            //{
            //    int count = BranchLeg.Points.Count(n => n.Item1.Kind != NodeKinds.Branch);
            //    if (count < 1)
            //        BranchLeg.BranchNode.X = offset.X + 40;
            //    else if (count >= 2) 
            //        BranchLeg.BranchNode.X = offset.X+20;
            //    else
            //        BranchLeg.BranchNode.X = offset.X ;

            //    if (BranchLeg.ConnectNodePort != null&&count<2)
            //        BranchLeg.BranchNode.X += 20;
            //}

            ////if (Math.Abs(BranchLeg.BranchNode.X - (offset.X + 20)) > double.Epsilon)
            ////{
            ////    BranchLeg.BranchNode.X = offset.X + 20;
            ////}
            //var w = maxW - minW + 40;
            //if (Math.Abs(w - Width) > double.Epsilon)
            //{
            //    int count = BranchLeg.Points.Count(i => i.Item1.Kind != NodeKinds.Branch) +
            //                (BranchLeg.ConnectNodePort == null ? 0 : 1);
            //    if (count < 2)
            //    {
            //        Width = Math.Max(w - 40, 40);
            //    }
            //    else
            //        Width = Math.Max(w,40);
            //}
            BranchLeg.BranchNode.X = offset.X -20;
            Width =Math.Abs(maxW - minW )+ 40;
        }
        
        public DiagramView View { get; }

        //public FrameworkElement ContentPatentNode { get; }

        public BranchLeg BranchLeg { get;}

        //public ObservableCollection<Point> Ports { get; }=new ObservableCollection<Point>();
        
    }
}
