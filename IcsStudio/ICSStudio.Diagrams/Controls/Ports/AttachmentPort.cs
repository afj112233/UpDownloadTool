using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Aga.Diagrams.Annotations;
using ICSStudio.Diagram.Flowchart;

namespace ICSStudio.Diagrams.Controls.Ports
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:ICSStudio.Diagrams.Controls.Ports"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:ICSStudio.Diagrams.Controls.Ports;assembly=ICSStudio.Diagrams.Controls.Ports"
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
    ///     <MyNamespace:AttachmentPort/>
    ///
    /// </summary>
    public class AttachmentPort : PortBase,INotifyPropertyChanged
    {
        private bool _isAttach;

        static AttachmentPort()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AttachmentPort), new FrameworkPropertyMetadata(typeof(AttachmentPort)));
        }

        public AttachmentPort()
        {
            IsAttach = false;
        }

        public override Point GetEdgePoint(Point target)
        {
            var a = ActualWidth / 2;
            var b = ActualHeight / 2;
            var p = new Point(target.X - Center.X, target.Y - Center.Y);
            p = GeometryHelper.EllipseLineIntersection(a, b, p);
            return new Point(p.X + Center.X, p.Y + Center.Y);
        }

        public override bool IsNear(Point point)
        {
            var a = ActualWidth / 2 + Sensitivity;
            var b = ActualHeight / 2 + Sensitivity;
            return GeometryHelper.EllipseContains(Center, a, b, point);
        }

        public bool IsAttach
        {
            set
            {
                _isAttach = value;
                if (IsAttach)
                {
                    AttachVisibility = Visibility.Visible;
                    UnattachedVisibility = Visibility.Collapsed;
                }
                else
                {
                    AttachVisibility = Visibility.Collapsed;
                    UnattachedVisibility = Visibility.Visible;
                }
            }
            get { return _isAttach; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public static readonly DependencyProperty AttachVisibilityProperty = DependencyProperty.Register(
            "AttachVisibility", typeof(Visibility), typeof(AttachmentPort), new PropertyMetadata(default(Visibility)));

        public Visibility AttachVisibility
        {
            get { return (Visibility) GetValue(AttachVisibilityProperty); }
            set { SetValue(AttachVisibilityProperty, value); }
        }

        public static readonly DependencyProperty UnattachedVisibilityProperty = DependencyProperty.Register(
            "UnattachedVisibilityProperty", typeof(Visibility), typeof(AttachmentPort), new PropertyMetadata(default(Visibility)));

        public Visibility UnattachedVisibility
        {
            get { return (Visibility) GetValue(UnattachedVisibilityProperty); }
            set { SetValue(UnattachedVisibilityProperty, value); }
        }

        public void DelLink()
        {
            var view = VisualHelper.FindParent<Canvas>(this);
            if (view != null)
            {
                var controller = (view as DiagramView)?.Controller;
                var node = view.Children.OfType<INode>()
                    .FirstOrDefault(x => x.Ports.FirstOrDefault(p => p == this) != null);
                var flowNode = (FlowNode)(node as Node).ModelElement;
                (controller as Controller)?.DelAttachLink(flowNode);
            }
        }
    }
}
