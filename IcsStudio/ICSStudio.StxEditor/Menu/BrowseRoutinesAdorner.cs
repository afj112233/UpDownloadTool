using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ICSStudio.Dialogs.BrowseRoutines;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Annotations;

namespace ICSStudio.StxEditor.Menu
{
    internal class BrowseRoutinesAdorner : Adorner
    {
        private readonly AutoCompleteBox _autoCompleteBox;
        private readonly Button _button;
        private readonly VisualCollection _visual;
        private Point _point;
        private double _zoom = 1;
        private readonly IRoutineCollection _routines;
        private readonly BrowseRoutines _browseRoutinesView;
        private readonly BrowseRoutinesViewModel _browseRoutinesViewModel;

        public BrowseRoutinesAdorner([NotNull] UIElement adornedElement, IRoutineCollection routines) : base(
            adornedElement)
        {
            _routines = routines;

            _visual = new VisualCollection(this);
            _autoCompleteBox = new AutoCompleteBox();
            _autoCompleteBox.Height = 25;
            _autoCompleteBox.Width = 150;
            _autoCompleteBox.Padding = new Thickness(0, 0, 25, 0);
            _autoCompleteBox.VerticalAlignment = VerticalAlignment.Top;
            _autoCompleteBox.HorizontalAlignment = HorizontalAlignment.Left;
            _autoCompleteBox.VerticalContentAlignment = VerticalAlignment.Center;
            _autoCompleteBox.IsTextCompletionEnabled = true;
            _autoCompleteBox.MaxDropDownHeight = 0;

            _button = new Button();
            _button.Height = 25;
            _button.Width = 25;
            _button.Background = new SolidColorBrush(Colors.Transparent);
            _button.BorderBrush = new SolidColorBrush(Colors.Transparent);

            var grid = new Grid();
            var line1 = new Line
            {
                X1 = 0,
                Y1 = 0,
                X2 = 7.5,
                Y2 = 6,
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(Colors.Black)
            };
            var line2 = new Line
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

            _browseRoutinesView = new BrowseRoutines();
            _browseRoutinesViewModel = new BrowseRoutinesViewModel(_routines, _autoCompleteBox.Width);
            _browseRoutinesViewModel.ShowChildren += () =>
            {
                if (!_visual.Contains(_browseRoutinesView))
                {
                    _visual.Add(_browseRoutinesView);
                    InvalidateArrange();
                }
            };
            _browseRoutinesView.DataContext = _browseRoutinesViewModel;
            _autoCompleteBox.DataContext = _browseRoutinesViewModel;
            _browseRoutinesView.HorizontalAlignment = HorizontalAlignment.Left;
            _browseRoutinesView.VerticalAlignment = VerticalAlignment.Top;

            _visual.Add(_autoCompleteBox);
            _visual.Add(_button);
            _button.Click += _button_Click;
            _autoCompleteBox.KeyDown += _autoCompleteBox_KeyDown;
            _browseRoutinesViewModel.PropertyChanged += _browseRoutinesViewModel_PropertyChanged;
        }

        public Action CloseAdorner { get; set; }

        private void _autoCompleteBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CloseAdorner?.Invoke();
            }
        }

        public void ResetAdorner(Point point, double zoom, string name)
        {
            _point = point;
            _zoom = zoom;

            _browseRoutinesViewModel.SelectedRoutine = _routines?[name];

            InvalidateMeasure();
        }

        public void SetTextFocus()
        {
            Keyboard.Focus(_autoCompleteBox);
        }

        public string GetName()
        {
            return _autoCompleteBox?.Text;
        }

        private void _button_Click(object sender, RoutedEventArgs e)
        {
            if (_visual.Contains(_browseRoutinesView))
                _visual.Remove(_browseRoutinesView);
            else
                _visual.Add(_browseRoutinesView);

            InvalidateArrange();
        }

        private void _browseRoutinesViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("SelectedRoutine"))
            {
                var text = (sender as BrowseRoutinesViewModel)?.SelectedRoutine?.Name;
                if (!string.IsNullOrEmpty(text)) _autoCompleteBox.Text = text;
                Keyboard.Focus(_autoCompleteBox);
                _visual.Remove(_browseRoutinesView);
                InvalidateMeasure();
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
            double leftSpace = 35;
            double topSpace = 0;
            _autoCompleteBox.Arrange(new Rect(finalSize));
            _autoCompleteBox.Margin =
                new Thickness((_point.X + leftSpace) * _zoom, (_point.Y + topSpace) * _zoom, 0, 0);
            _button.Arrange(new Rect(finalSize));
            _button.Margin = new Thickness(125 + (_point.X + leftSpace) * _zoom, (_point.Y + topSpace) * _zoom, 0, 0);
            if (_visual.Contains(_browseRoutinesView))
            {
                _browseRoutinesView.Arrange(new Rect(finalSize));
                var point = TestHit(new Point((_point.X + leftSpace) * _zoom, (_point.Y + topSpace) * _zoom + 25));
                _browseRoutinesView.Margin = new Thickness(point.X, point.Y, 0, 0);
            }

            return base.ArrangeOverride(finalSize);
        }

        private Point TestHit(Point point)
        {
            double width = ((Control)AdornedElement).ActualWidth - 25,
                height = ((Control)AdornedElement).ActualHeight;
            Point newPoint = point;
            double viewWidth = _autoCompleteBox.Width, viewHeight = 250;
            if (point.X >= width || point.X + viewWidth >= width) newPoint.X = width - viewWidth + 10;
            if (point.Y >= height || point.Y + viewHeight >= height) newPoint.Y = point.Y - viewHeight - 25;
            return newPoint;
        }

    }
}