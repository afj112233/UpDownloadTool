using ICSStudio.Gui.Annotations;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ICSStudio.EditorPackage.STEditor;
using ICSStudio.Gui.Dialogs;

namespace ICSStudio.LanguageElement.View
{
    /// <summary>
    /// View.xaml 的交互逻辑
    /// </summary>
    public partial class View : UserControl
    {
        public View()
        {
            InitializeComponent();
            Items.ItemsSource = LanguageElementOption.GetElements(Element_PropertyChanged); ;
            Loaded += LanguageElement_Loaded;
        }

        private void LanguageElement_Loaded(object sender, RoutedEventArgs e)
        {
            var item = (Element)Items.ItemContainerGenerator.Items[0];
            item.IsSelected = true;
        }

        private void Element_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var element = (Element)sender;
            if (e.PropertyName == "IsSelected")
            {
                var dependencyObject = Items.ItemContainerGenerator.ContainerFromItem(element);
                if (dependencyObject == null)
                {
                    return;
                }
                var dependency = VisualTreeHelper.GetChild(dependencyObject, 0);
                var border = (Border) dependency;
                if (element.IsSelected)
                {
                    border.Background = Brushes.White;
                    Elements.ItemsSource = element.Children;
                }
                else
                {
                    border.Background = new SolidColorBrush(Color.FromRgb(216, 219, 233));
                }
            }
        }
        
        private void UIElement_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var border = (Border)sender;
            var element = (Element)border.DataContext;
            if (element.IsSelected) return;
            for (int i = 0; i < Items.Items.Count; i++)
            {
                var item = (Element)Items.ItemContainerGenerator.Items[i];
                item.IsSelected = false;
            }
            element.IsSelected = true;

        }

        private void UIElement_OnMouseEnter(object sender, MouseEventArgs e)
        {
            var border = (Border)sender;
            var element = (Element)border.DataContext;
            if (!element.IsSelected)
                border.Background = Brushes.DimGray;
        }

        private void UIElement_OnMouseLeave(object sender, MouseEventArgs e)
        {
            var border = (Border)sender;
            var element = (Element)border.DataContext;
            if (!element.IsSelected)
                border.Background = new SolidColorBrush(Color.FromRgb(216, 219, 233));
        }

        private void Items_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var tb = (TextBlock)sender;
            DragDrop.DoDragDrop(tb, tb.DataContext, DragDropEffects.Move);
        }

        private void Items_OnPreviewDragEnter(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(typeof(Element));
            if (data != null)
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        private void Items_OnPreviewDrop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(typeof(Element));
            if (data != null)
            {
                var tb = (TextBlock)sender;
                var element1 = (Element)tb.DataContext;
                var element2 = (Element)data;
                if (element1 == element2) return;
                var tmp = element1.Name;
                element1.Name = element2.Name;
                element2.Name = tmp;
                var tmpList = element1.Children;
                element1.Children = element2.Children;
                element2.Children = tmpList;

                Dispatcher.InvokeAsync(() =>
                {
                    element1.IsSelected = true;
                    element2.IsSelected = false;
                }, DispatcherPriority.SystemIdle);
            }
        }

        private void Element_OnMouseEnter(object sender, MouseEventArgs e)
        {
            var border = (Border) sender;
            border.Background=new SolidColorBrush(Color.FromRgb(253,244,191));
        }

        private void Element_OnMouseLeave(object sender, MouseEventArgs e)
        {
            var border = (Border)sender;
            border.Background = new SolidColorBrush(Color.FromRgb(216, 219, 233));
        }

        private void Element_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var stEditorViewModel = CurrentObject.GetInstance().Current as STEditorViewModel;
            stEditorViewModel?.InsertLanguageElement($"{(string)((Border)sender).DataContext}( )");
        }

        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var tb = (Border)sender;
                DragDrop.DoDragDrop(tb, $"{(string)tb.DataContext}( )", DragDropEffects.All);
            }
        }

        private void UIElement_OnPreviewQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (e.KeyStates == DragDropKeyStates.None)
            {
                e.Action = DragAction.Cancel;
            }
        }
    }

    public static class LanguageElementOption
    {
        public static List<Element> GetElements(PropertyChangedEventHandler handler)
        {
            var list = new List<Element>();

            Create("Favorites", list, handler, "JSR", "SBR", "RET", "ABS", "TRUNC", "SIZE", "SFR", "SFP", "EOT");
            Create("Process", list, handler, "ALM",
                "SCL",
                "PID",
                "PIDE",
                "IMC",
                "CC",
                "MMC",
                "RMPS",
                "POSP",
                "SRTP",
                "LDLG",
                "FGEN",
                "TOT",
                "DEDT",
                "D2SD",
                "D3SD");
            Create("Drives", list, handler, "PMUL",
                "SCRV",
                "PI",
                "INTG",
                "SOC",
                "UPDN"
            );
            Create("Statistical", list, handler, "MAVE",
                "MSTD",
                "MINC",
                "MAXC"
            );
            Create("Select/Limit", list, handler, "ESEL",
                "SSUM",
                "SNEG",
                "HLL",
                "RLIM"
            );
            Create("Bit", list, handler, "OSRI", "OSFI");
            Create("Alarms", list, handler, "ALMD", "ALMA");
            Create("Filters", list, handler, "HPF",
                "LPF",
                "NTCH",
                "LDL2",
                "DERV");
            Create("Timer/Counter", list, handler, "TONR",
                "TOFR",
                "RTOR",
                "CTUD");
            Create("Compute/Math", list, handler, "SQRT", "ABS");
            Create("Move/Logical", list, handler, "MVMT",
                "SWPB",
                "BTDT",
                "DFF",
                "JKFF",
                "SETD",
                "RESD");
            Create("HMI", list, handler, "HMIBC");
            Create("Trigonometry", list, handler, "SIN", "COS", "TAN", "ASIN", "ACOS", "ATAN");
            Create("Advanced Math", list, handler, "LN", "LOG");
            Create("Math Conversions", list, handler, "DEG", "RAD", "TRUNC");
            Create("File/Misc.", list, handler, "COP", "SRT", "SIZE", "CPS");
            //Create("Equipment Phase", list, handler, "PSC", "PFL", "PCMD", "PCLF", "PXRQ", "PPD", "PRNP", "PATT", "PDET",
            //    "POVR");
            //Create("Equipment Sequence", list, handler, "SATT", "SDET", "SCMD", "SCLF", "SOVR", "SASI");
            Create("Program Control", list, handler, "JSR", "RET", "SBR", "TND", "UID", "UIE", "SFR", "SFP", "EOT", "EVENT");
            Create("Input/Output", list, handler, "MSG", "GSV", "SSV", "IOT");
            Create("Motion State", list, handler, "MSO", "MSF", "MASD", "MASR", "MDO", "MDF", "MDS", "MAFR");
            Create("Motion Move", list, handler, "MAS", "MAH", "MAJ", "MAM", "MAG", "MCD", "MRP", "MCCP", "MCSV", "MAPC", "MATC",
                "MDAC");
            Create("Motion Group", list, handler, "MGS", "MGSD", "MGSR", "MGSP");
            Create("Motion Event", list, handler
                , "MAW", "MDW", "MAR", "MDR", "MAOC", "MDOC");
            Create("Motion Config", list, handler, "MAAT", "MRAT", "MAHD", "MRHD");
            //Create("Motion Coordinated", list, handler, "MCS", "MCLM", "MCCM", "MCCD", "MCT", "MCTP", "MCSD", "MCSR", "MDCC");
            //Create("ASCII Serial Port", list, handler, "AWT", "AWA", "ARD", "ARL", "ABL", "ACB", "AHL", "ACL");
            Create("ASCII String", list, handler, "FIND", "INSERT", "CONCAT", "MID", "DELETE");
            Create("ASCII Conversion", list, handler, "DTOS", "STOD", "RTOS", "STOR", "UPPER", "LOWER");
            return list;
        }

        private static Element Create(string name, List<Element> parent, PropertyChangedEventHandler handler, params string[] children)
        {
            var element = new Element(name);
            element.Children.AddRange(children);
            parent.Add(element);
            element.PropertyChanged += handler;
            return element;
        }
    }

    public class Element : INotifyPropertyChanged
    {
        private bool _isSelected;
        private string _name;

        public Element(string name)
        {
            Name = name;
        }

        public bool IsSelected
        {
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
            get { return _isSelected; }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public List<string> Children { get; set; } = new List<string>();
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
