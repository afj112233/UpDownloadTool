using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ICSStudio.Diagrams.Annotations;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.EditorPackage.Reference.Extend
{
    internal class ScopeAdorner: Adorner
    {
        private readonly ScopeTreeView _scopeTreeView;
        private AdornerLayer _layer;
        public ScopeAdorner([NotNull] UIElement adornedElement,AdornerLayer layer,Point point,SelectionTextType type,IController controller) : base(adornedElement)
        {
            _layer = layer;
            _scopeTreeView = new ScopeTreeView(layer, this,type,controller);
            _scopeTreeView.Height = 150;
            _scopeTreeView.HorizontalAlignment = HorizontalAlignment.Left;
            _scopeTreeView.VerticalAlignment = VerticalAlignment.Top;
            _scopeTreeView.Margin = new Thickness(Math.Abs(point.X), Math.Abs(point.Y), 0, 0);
            AddVisualChild(_scopeTreeView);

        }

        public void Update(SelectionTextType type)
        {
            _scopeTreeView.Update(type);
        }

        public delegate void ScopeNameHandle(object sender,ScopeNameEventArgs e);

        public event ScopeNameHandle ScopeChanged;

        public void DoScopeChanged(string name,bool isController)
        {
            ScopeChanged?.Invoke(this,new ScopeNameEventArgs(name,isController));
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index)
        {
            return _scopeTreeView;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _scopeTreeView.Measure(constraint);
            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _scopeTreeView.Arrange(new Rect(finalSize));
            return base.ArrangeOverride(finalSize);
        }
    }

    internal class ScopeNameEventArgs : EventArgs
    {
        public ScopeNameEventArgs(string name,bool isController)
        {
            IsController = isController;
            ScopeName = name;
        }

        public string ScopeName { get; }

        public bool IsController { get; }
    }
}
