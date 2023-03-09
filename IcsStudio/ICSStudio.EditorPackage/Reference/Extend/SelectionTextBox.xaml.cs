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
using ICSStudio.Gui.View;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Project;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.EditorPackage.Reference.Extend
{
    /// <summary>
    /// SelectionTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class SelectionTextBox : UserControl
    {
        private static ScopeAdorner _scopeAdorner;
        private AdornerLayer _layer;
        private Grid _grid;
        public SelectionTextBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IsControllerProperty = DependencyProperty.Register(
            "IsController", typeof(bool), typeof(SelectionTextBox), new PropertyMetadata(true));

        public bool IsController
        {
            get { return (bool) GetValue(IsControllerProperty); }
            set { SetValue(IsControllerProperty, value); }
        }

        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(
            "Type", typeof(SelectionTextType), typeof(SelectionTextBox), new PropertyMetadata(default(SelectionTextType), SelectedTextTypeChanged));

        public SelectionTextType Type
        {
            get { return (SelectionTextType) GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        private static void SelectedTextTypeChanged(DependencyObject obj,DependencyPropertyChangedEventArgs e)
        {
            _scopeAdorner?.Update((SelectionTextType) e.NewValue);
        }

        public static readonly DependencyProperty TextContentProperty = DependencyProperty.Register(
            "TextContent", typeof(string), typeof(SelectionTextBox), new PropertyMetadata(default(string)));

        public string TextContent
        {
            get { return (string) GetValue(TextContentProperty); }
            set { SetValue(TextContentProperty, value); }
        }
        
        private void ScopeText_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_scopeAdorner != null)
            {
                _layer?.Remove(_scopeAdorner);
                _scopeAdorner = null;
                e.Handled = true;
                return;
            }
            var service = Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            //var stackPanel = VisualTreeHelpers.FindVisualParentOfType<StackPanel>(Text);
            _grid = (Grid)VisualTreeHelpers.FindVisualParentByName(Text, "TopGrid");
            _grid.PreviewMouseLeftButtonDown += Grid_PreviewMouseLeftButtonDown;
            if (_layer == null)
            {
                _layer = AdornerLayer.GetAdornerLayer(_grid);
            }
            var point = _grid.TranslatePoint(new Point(), Text) + new Vector(5, -18);
            if (Math.Abs(Math.Abs(Margin.Left) - double.Epsilon) > 0)
                point = point + new Vector(-5, 0);
            _scopeAdorner = new ScopeAdorner(_grid, _layer, point,Type, service?.Controller);
            //_scopeAdorner.Margin = Margin;
            _scopeAdorner.ScopeChanged += _scopeAdorner_ScopeChanged;
            _layer?.Add(_scopeAdorner);
            e.Handled = true;
        }

        private void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_scopeAdorner != null)
            {
                _layer?.Remove(_scopeAdorner);
                _scopeAdorner = null;
                e.Handled = true;
            }

            _grid.PreviewMouseLeftButtonDown -= Grid_PreviewMouseLeftButtonDown;
        }

        private void _scopeAdorner_ScopeChanged(object sender, ScopeNameEventArgs e)
        {
            IsController = e.IsController;
            TextContent = e.ScopeName;
        }
    }

    public enum SelectionTextType
    {
        Scope,
        Show,
        Label,
        ScopeExceptAoi
    }
}
