using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ICSStudio.SimpleServices.Annotations;
using System.Windows.Controls;

namespace ICSStudio.EditorPackage.RLLEditor
{
    public class InputAdorner : Adorner
    {
        private readonly TextBox _inputBox;
        private readonly VisualCollection _visual;
        private Point _point;
        private double _zoom = 1;
        //private object _inputContent;
        public string InputContent => _inputBox.Text;

        public InputAdorner([NotNull] UIElement adornedElement) : base(adornedElement)
        {
            _visual = new VisualCollection(this);
            _inputBox = new TextBox
            {
                Height = 25,
                Width = 150,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
            };

            _inputBox.AddHandler(KeyDownEvent, new KeyEventHandler(On_KeyDown));

            _visual.Add(_inputBox);
        }

        public void Resize(bool isExpression = false)
        {
            if (isExpression)
            {
                _inputBox.Height = 100;
                _inputBox.Width = 300;
                _inputBox.TextWrapping = TextWrapping.Wrap;
            }
            else
            {
                _inputBox.Height = 25;
                _inputBox.Width = 150;
                _inputBox.TextWrapping = TextWrapping.NoWrap;
            }
        }

        private void On_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RLLEditorViewModel.OnRemoveAdorner?.Invoke(this);
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visual[index];
        }

        protected override int VisualChildrenCount => _visual.Count;

        public void ResetAdorner(Point point, double zoom, string name)
        {
            _point = point;
            _zoom = zoom;
            _inputBox.Text = name;
            InvalidateMeasure();
        }

        public void SetTextFocus()
        {
            Keyboard.Focus(_inputBox);
            _inputBox.SelectAll();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {

            _inputBox.Arrange(new Rect(finalSize));
            _inputBox.Margin = new Thickness(_point.X * _zoom, _point.Y * _zoom, 0, 0);

            return base.ArrangeOverride(finalSize);
        }
    }
}
