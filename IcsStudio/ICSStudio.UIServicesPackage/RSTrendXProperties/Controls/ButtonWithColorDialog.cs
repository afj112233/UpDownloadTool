using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Button = System.Windows.Controls.Button;

namespace ICSStudio.UIServicesPackage.RSTrendXProperties.Controls
{
    public class ButtonWithColorDialog : Button
    {
        public ButtonWithColorDialog():base()
        {
            Foreground = new SolidColorBrush(Colors.Transparent);
            Click += ChangeColor_Click;
        }

        private void ChangeColor_Click(object sender, RoutedEventArgs e)
        {
            var colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                var drawingColor = colorDialog.Color;
                var mediaColor = Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B);
                var button = (Button)sender;
                if (button != null)
                {
                    button.Background = new SolidColorBrush(mediaColor);
                    button.Content = mediaColor;
                }
            }
        }
    }
}