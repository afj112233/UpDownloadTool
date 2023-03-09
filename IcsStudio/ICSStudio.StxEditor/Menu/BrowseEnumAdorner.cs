using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Annotations;

namespace ICSStudio.StxEditor.Menu
{
    internal class BrowseEnumAdorner:Adorner
    {
        private readonly AutoCompleteBox _autoCompleteBox;
        private readonly Button _button;
        private readonly VisualCollection _visual;
        private Point _point;
        private double _zoom = 1;
        private RoutedEventHandler _lostFocusEventHandler;
        private readonly ListView _listView;
        private readonly EnumViewModel _enumViewModel;
        public BrowseEnumAdorner([NotNull] UIElement adornedElement) : base(
            adornedElement)
        {
            _visual = new VisualCollection(this);
            _autoCompleteBox = new AutoCompleteBox();
            _autoCompleteBox.Height = 25;
            _autoCompleteBox.Width = 150;
            _autoCompleteBox.Padding = new Thickness(0, 0, 25, 0);
            _autoCompleteBox.VerticalAlignment = VerticalAlignment.Top;
            _autoCompleteBox.HorizontalAlignment = HorizontalAlignment.Left;
            _autoCompleteBox.IsTextCompletionEnabled = true;
            _autoCompleteBox.MaxDropDownHeight = 0;
            var itemsBinding = new Binding("NameList");
            BindingOperations.SetBinding(_autoCompleteBox, AutoCompleteBox.ItemsSourceProperty, itemsBinding);
            var nameBinding = new Binding("Name");
            nameBinding.Mode = BindingMode.TwoWay;
            nameBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(_autoCompleteBox, AutoCompleteBox.TextProperty, nameBinding);
            _button = new Button();
            _button.Height = 25;
            _button.Width = 25;
            _button.Background = new SolidColorBrush(Colors.Transparent);
            _button.BorderBrush = new SolidColorBrush(Colors.Transparent);

            Grid grid = new Grid();
            Line line1 = new Line()
            {
                X1 = 0,
                Y1 = 0,
                X2 = 7.5,
                Y2 = 6,
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(Colors.Black)
            };
            Line line2 = new Line()
            {
                X1 = 7.5,
                Y1 = 6,
                X2 = 15,
                Y2 = 0,
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(Colors.Black)
            };
            grid.Children.Add(line1);
            grid.Children.Add(line2);
            _button.Content = grid;

            _button.VerticalAlignment = VerticalAlignment.Top;
            _button.HorizontalAlignment = HorizontalAlignment.Left;

            _listView=new ListView();
            _listView.Width = 250;
            _listView.Height = 80;
            _listView.VerticalAlignment = VerticalAlignment.Top;
            _listView.HorizontalAlignment = HorizontalAlignment.Left;
            _enumViewModel=new EnumViewModel();
            _listView.DataContext = _enumViewModel;
            _autoCompleteBox.DataContext = _enumViewModel;
            var selectedValue=new Binding("SelectedEnum");
            selectedValue.Mode = BindingMode.TwoWay;
            selectedValue.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(_listView, ListView.SelectedValueProperty, selectedValue);
            var itemSource=new Binding("Enums");
            itemSource.Mode = BindingMode.TwoWay;
            itemSource.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(_listView, ListView.ItemsSourceProperty, itemSource);
            _listView.SelectionChanged += _listView_SelectionChanged;
            _listView.PreviewMouseLeftButtonUp += _listView_PreviewMouseLeftButtonUp;

            _visual.Add(_autoCompleteBox);
            _visual.Add(_button);
            _button.Click += _button_Click;
        }

        private void _listView_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _visual.Remove(_listView);
            e.Handled = true;
        }

        private void _listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count>0)
                _listView.ScrollIntoView(e.AddedItems[0]);
        }

        public RoutedEventHandler LostFocusEventHandler
        {
            set
            {
                if (_lostFocusEventHandler != null)
                    LostFocus -= _lostFocusEventHandler;
                _lostFocusEventHandler = value;
                LostFocus += value;
            }
            get { return _lostFocusEventHandler; }
        }

        public void ResetAdorner(Point point, double zoom, string name,List<string> enums)
        {
            _point = point;
            _zoom = zoom;
            _enumViewModel.Reset(enums, name);
            if (_visual.Contains(_listView))
                _visual.Remove(_listView);
            InvalidateMeasure();
        }

        public void SetTextFocus()
        {
            Keyboard.Focus(_autoCompleteBox);
        }

        public string GetName()
        {
            return _enumViewModel.Name;
        }
        
        private void _button_Click(object sender, RoutedEventArgs e)
        {
            if (_visual.Contains(_listView))
                _visual.Remove(_listView);
            else
            {
                _visual.Add(_listView);
            }

            InvalidateArrange();
        }

        protected override int VisualChildrenCount => _visual.Count;

        protected override Visual GetVisualChild(int index)
        {
            var line = _visual[index];
            return line;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double leftSpace = 35;
            double topSpace = 0;
            _autoCompleteBox.Arrange(new Rect(finalSize));
            _autoCompleteBox.Margin =
                new Thickness((_point.X + leftSpace) * _zoom, (_point.Y + topSpace) * _zoom, 0, 0);
            _button.Arrange(new Rect(finalSize));
            _button.Margin = new Thickness(125 + (_point.X + leftSpace) * _zoom, (_point.Y + topSpace) * _zoom, 0, 0);
            if (_visual.Contains(_listView))
            {
                _listView.Arrange(new Rect(finalSize));
                _listView.Margin = new Thickness((_point.X + leftSpace) * _zoom, (_point.Y + topSpace) * _zoom + 25, 0, 0);
            }

            return base.ArrangeOverride(finalSize);
        }
    }
}
