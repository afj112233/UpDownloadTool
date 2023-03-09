using System.Windows;
using ICSStudio.Interfaces.DataType;

namespace ICSStudio.EditorPackage.MonitorEditTags.UI
{
    public class Wrapper : DependencyObject
    {
        public static readonly DependencyProperty DisplayStyleProperty =
            DependencyProperty.Register("DisplayStyle", typeof(DisplayStyle),
                typeof(Wrapper), new FrameworkPropertyMetadata(default(DisplayStyle)));

        public DisplayStyle DisplayStyle
        {
            get { return (DisplayStyle) GetValue(DisplayStyleProperty); }
            set { SetValue(DisplayStyleProperty, value); }
        }
    }
}