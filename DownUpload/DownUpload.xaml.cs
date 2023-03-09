using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
using DownUpload.ViewModel;
using DownUpload.Model;

namespace DownUpload
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new DownUploadViewModel();
            ConsoleManager.Show();
        }
    }

    public static class TextBlockExtensions
        {
            public static IEnumerable<Inline> GetBindableInlines(DependencyObject obj)
            {
                return (IEnumerable<Inline>)obj.GetValue(BindableInlinesProperty);
            }

            public static void SetBindableInlines(DependencyObject obj, IEnumerable<Inline> value)
            {
                obj.SetValue(BindableInlinesProperty, value);
            }

            public static readonly DependencyProperty BindableInlinesProperty =
                DependencyProperty.RegisterAttached("BindableInlines", typeof(IEnumerable<Inline>), typeof(TextBlockExtensions), new PropertyMetadata(null, OnBindableInlinesChanged));

            private static void OnBindableInlinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                var Target = d as TextBlock;

                if (Target != null)
                {
                    Target.Inlines.Clear();
                    Target.Inlines.AddRange((System.Collections.IEnumerable)e.NewValue);
                }
            }
        }
}
