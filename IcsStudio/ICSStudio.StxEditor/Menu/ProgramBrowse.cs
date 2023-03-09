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
using System.Windows.Shapes;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Annotations;

namespace ICSStudio.StxEditor.Menu
{
    internal class ProgramBrowse : Adorner, INotifyPropertyChanged
    {
        private readonly VisualCollection _visual;
        private readonly AutoCompleteBox _autoCompleteBox;
        private readonly Button _button;
        private readonly IProgramModule _program;
        private string _name;
        private readonly ListView _listView;
        private Visibility _listViewVisibility = Visibility.Collapsed;
        private string _selectedProgram;
        private readonly Point _point;
        private readonly double _zoom;
        private bool _setSelected = true;
        
        public ProgramBrowse([NotNull] UIElement adornedElement, IProgramModule program, Point point, double zoom) : base(
            adornedElement)
        {
            //point = point + new Vector(330, 70);
            _visual = new VisualCollection(this);
            _point = point;
            _zoom = zoom;
            _program = program;
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
            var nameBinding = new Binding("ProgramName");
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

            _autoCompleteBox.DataContext = this;

            _visual.Add(_autoCompleteBox);
            _visual.Add(_button);
            _button.Click += _button_Click;

            _listView = new ListView();
            _listView.ItemsSource = NameList;
            _listView.Width = 200;
            _listView.Height = 60;
            _listView.VerticalAlignment = VerticalAlignment.Top;
            _listView.HorizontalAlignment = HorizontalAlignment.Left;
            _listView.DataContext = this;

            var selectedBinding = new Binding("SelectedProgram");
            selectedBinding.Mode = BindingMode.TwoWay;
            selectedBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(_listView, ListView.SelectedItemProperty, selectedBinding);

            var visibility = new Binding("ListViewVisibility");
            visibility.Mode = BindingMode.TwoWay;
            visibility.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(_listView, ListView.VisibilityProperty, visibility);
            _visual.Add(_listView);
            SetNameList();
            ProgramName = program.Name;
        }

        public void SetTextFocus()
        {
            Keyboard.Focus(_autoCompleteBox);
        }

        public Visibility ListViewVisibility
        {
            set
            {
                _listViewVisibility = value;
                OnPropertyChanged();
            }
            get { return _listViewVisibility; }
        }

        public string ProgramName
        {
            set
            {
                if (_name == value) return;
                _name = value;
                if (_setSelected)
                {
                    if (NameList.Contains(_name))
                    {
                        SelectedProgram = _name;
                        _listView.ScrollIntoView(_name);
                    }
                }

                OnPropertyChanged();
            }
            get { return _name; }
        }

        public List<string> NameList { set; get; } = new List<string>();

        public string SelectedProgram
        {
            set
            {
                _selectedProgram = value;
                _setSelected = false;
                ProgramName = SelectedProgram;
                _setSelected = true;
                OnPropertyChanged();
            }
            get { return _selectedProgram; }
        }

        private void SetNameList()
        {
            foreach (var program in _program.ParentController.Programs)
            {
                NameList.Add(program.Name);
            }
        }

        private void _button_Click(object sender, RoutedEventArgs e)
        {
            ListViewVisibility = ListViewVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            //InvalidateArrange();
        }

        protected override int VisualChildrenCount => _visual.Count;

        protected override Visual GetVisualChild(int index)
        {
            var line = _visual[index];
            return line;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
            _listView.Arrange(new Rect(finalSize));
            _listView.Margin = new Thickness((_point.X + leftSpace) * _zoom, (_point.Y + topSpace) * _zoom + 25, 0, 0);

            return base.ArrangeOverride(finalSize);
        }
    }
}
