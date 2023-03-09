using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ICSStudio.AvalonEdit.Rendering
{
    internal class PendingLayer : Layer
    {
        private readonly Image _image;

        public PendingLayer(TextView textView, KnownLayer knownLayer) : base(textView, knownLayer)
        {
            _image = new Image();
            var bitImage = new BitmapImage(new Uri(@"pack://application:,,,/ICSStudio.AvalonEdit;component/Images/Pending.png", UriKind.RelativeOrAbsolute));
            _image.Width = bitImage.Width;
            _image.Height = bitImage.Height;
            bitImage.Freeze();
            _image.Source = bitImage;
            AddVisualChild(_image);
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override Visual GetVisualChild(int index)
        {
            return _image;
        }

        protected override void ArrangeCore(Rect finalRect)
        {
            _image.VerticalAlignment = VerticalAlignment.Bottom;
            _image.HorizontalAlignment = HorizontalAlignment.Right;
            _image.Arrange(finalRect);
            base.ArrangeCore(finalRect);
        }
    }
}
