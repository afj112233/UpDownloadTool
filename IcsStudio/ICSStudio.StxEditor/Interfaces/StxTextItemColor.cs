using System.Windows.Media;

namespace ICSStudio.StxEditor.Interfaces
{
    public class StxTextItemColor
    {
        public StxTextItemColor(Color foreground, Color background, bool transparent)
        {
            Foreground = foreground;
            Background = background;
            Transparent = transparent;
        }

        public Color Background { get; set; }

        public Color Foreground { get; set; }

        public bool Transparent { get; set; }
    }
}