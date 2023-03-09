using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using ICSStudio.Dialogs.Filter;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Annotations;
using System.Collections.Generic;

namespace ICSStudio.EditorPackage.RLLEditor
{
    public class BrowseAdorner : Adorner
    {
        private readonly AutoCompleteBox _autoCompleteBox;
        private readonly Button _button;
        private readonly VisualCollection _visual;
        private readonly FilterView _view;
        private readonly FilterViewModel _filterViewModel;
        private Point _point;
        private double _zoom = 1;
        private readonly IController _controller;
        private readonly IProgramModule _parentProgram;
        private RoutedEventHandler _lostFocusEventHandler;
        private bool _hide;
        private AutoCompleteSource _autoCompleteSource = new AutoCompleteSource();

        public BrowseAdorner([NotNull] UIElement adornedElement, IController controller, IProgramModule parentProgram) : base(
            adornedElement)
        {
            _controller = controller;
            _parentProgram = parentProgram;
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
            itemsBinding.Source = _autoCompleteSource;//new AutoCompleteSource(new List<string> { "aa1", "aab2", "aabc3", "aabcd4", "aabcde5", "aeee5" });
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

            _view = new FilterView();
            _filterViewModel = new FilterViewModel(controller, parentProgram, false, false, "");
            _view.DataContext = _filterViewModel;
            _autoCompleteBox.DataContext = _filterViewModel;
            _view.HorizontalAlignment = HorizontalAlignment.Left;
            _view.VerticalAlignment = VerticalAlignment.Top;

            _autoCompleteBox.AddHandler(KeyDownEvent, new KeyEventHandler(On_KeyDown));

            _visual.Add(_autoCompleteBox);
            _visual.Add(_button);
            _button.Click += _button_Click;
            _filterViewModel.PropertyChanged += _filterViewModel_PropertyChanged;
        }

        public void SetCompleteBoxSource(List<string> data)
        {
            _autoCompleteSource.NameList = data;
        }

        private void On_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RLLEditorViewModel.OnRemoveAdorner?.Invoke(this);
            }
        }

        private void _filterViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Hide")
            {
                _visual.Remove(_view);
                InvalidateMeasure();
            }
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

        public void ResetAdorner(Point point, double zoom, string name, bool showFilter = true)
        {
            _point = point;
            _zoom = zoom;
            _hide = false;

            if (!string.IsNullOrEmpty(name) && name.IndexOf("\\", StringComparison.Ordinal) == 0)
            {
                var programName = name.IndexOf(".", StringComparison.Ordinal) > 0
                    ? name.Substring(1, name.IndexOf(".", StringComparison.Ordinal) - 1)
                    : name.Substring(1);
                _filterViewModel.SetOtherSelectedProgram(programName);
            }
            else
            {
                _filterViewModel.SetOtherSelectedProgram("<none>");
            }

            _filterViewModel.NeedGetChild = false;
            _filterViewModel.Name = name;
            _filterViewModel.NeedGetChild = true;

            if (_visual.Contains(_view))
            {
                _visual.Remove(_view);
                PropertyChangedEventManager.RemoveHandler(_filterViewModel, FilterViewModelPropertyChanged,
                    "VisibilityCol3");
            }

            _button.Visibility = showFilter ? Visibility.Visible : Visibility.Collapsed;

            InvalidateMeasure();
        }

        public void DoScrollChanged(Vector vector)
        {
            _point = _point + vector;
            if (_point.X < 0 || _point.Y < 0)
            {
                var layer = AdornerLayer.GetAdornerLayer(AdornedElement);
                layer?.Remove(this);
                _hide = true;
            }
            else
            {
                if (_hide)
                {
                    var layer = AdornerLayer.GetAdornerLayer(AdornedElement);
                    layer?.Add(this);
                }

                InvalidateArrange();
            }

        }

        public void SetTextFocus()
        {
            Keyboard.Focus(_autoCompleteBox);
        }

        public string GetName()
        {
            return _filterViewModel.Name;
        }

        public string CheckTagNameInOtherProgram(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;

            var tagName = name.Split('.')[0];
            if (tagName.IndexOf("[", StringComparison.Ordinal) > 0)
            {
                tagName = tagName.Substring(0, tagName.IndexOf("[", StringComparison.Ordinal));
            }

            var tag = _controller.Tags[tagName];
            if (tag != null) return name;
            tag = _parentProgram.Tags[tagName];
            if (tag != null) return name;

            if (_filterViewModel.SelectedOther != "<none>")
            {
                var program = _controller.Programs[_filterViewModel.SelectedOther];
                tag = program.Tags[tagName];
                if (tag != null) return $@"\{program.Name}.{name}";
            }

            return name;
        }

        private void _button_Click(object sender, RoutedEventArgs e)
        {
            if (_visual.Contains(_view))
                _visual.Remove(_view);
            else
            {
                _visual.Add(_view);
                PropertyChangedEventManager.AddHandler(_filterViewModel, FilterViewModelPropertyChanged, "VisibilityCol3");
#pragma warning disable VSTHRD001 // 避免旧线程切换 API
                Dispatcher.BeginInvoke(DispatcherPriority.Background,
#pragma warning restore VSTHRD001 // 避免旧线程切换 API
                    (Action)delegate { _view.ScrollItemToView(); });
            }
            InvalidateMeasure();
        }

        private void FilterViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("VisibilityCol3"))
            {
                InvalidateArrange();
            }
        }

        protected override int VisualChildrenCount => _visual.Count;

        protected override Visual GetVisualChild(int index)
        {
            var line = _visual[index];
            return line;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double leftSpace = 0;
            double topSpace = 0;
            _autoCompleteBox.Arrange(new Rect(finalSize));
            _autoCompleteBox.Margin =
                new Thickness((_point.X + leftSpace) * _zoom, (_point.Y + topSpace) * _zoom, 0, 0);
            _button.Arrange(new Rect(finalSize));
            _button.Margin = new Thickness(125 + (_point.X + leftSpace) * _zoom, (_point.Y + topSpace) * _zoom, 0, 0);
            if (_visual.Contains(_view))
            {
                _view.Arrange(new Rect(finalSize));
                var point = TestHit(new Point((_point.X + leftSpace) * _zoom, (_point.Y + topSpace) * _zoom + 25));
                _view.Margin = new Thickness(point.X, point.Y, 0, 0);
            }

            return base.ArrangeOverride(finalSize);
        }

        private Point TestHit(Point point)
        {
            double width = ((FrameworkElement)AdornedElement).ActualWidth - 25, height = ((FrameworkElement)AdornedElement).ActualHeight;
            Point newPoint = point;
            double viewWidth = 570, viewHeight = 290;
            if (_filterViewModel.VisibilityCol3 == Visibility.Visible) viewWidth = 730;
            if (point.X >= width || point.X + viewWidth >= width) newPoint.X = width - viewWidth + 10;
            if (point.Y >= height || point.Y + viewHeight >= height) newPoint.Y = point.Y - viewHeight - 25;
            return newPoint;
        }
    }

    public class AutoCompleteSource : INotifyPropertyChanged
    {
        public AutoCompleteSource() { }

        public AutoCompleteSource(List<string> data)
        {
            _nameList = data;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                if (PropertyChanged != null)
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        private List<string> _nameList;

        public List<string> NameList
        {
            get
            {
                return _nameList ?? new List<string>();
            }
            set
            {
                _nameList = value;
                if (PropertyChanged != null)
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(NameList)));
            }
        }
    }
}
