using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace ICSStudio.Components.Controls
{
    /// <summary>
    /// TreeListViewControl.xaml 的交互逻辑
    /// </summary>
    public partial class TreeListViewControl
    {
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(nameof(Items),
            typeof(IEnumerable), typeof(TreeListViewControl), new PropertyMetadata(null));

        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register(nameof(Columns),
            typeof(GridViewColumnCollection), typeof(TreeListViewControl),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(TreeListViewControl),
                new PropertyMetadata(null));

        public TreeListViewControl()
        {
            SetValue(ColumnsProperty, new GridViewColumnCollection());

            DataContext = this;

            InitializeComponent();
        }

        public IEnumerable Items
        {
            get { return (IEnumerable) GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate) GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public GridViewColumnCollection Columns => (GridViewColumnCollection) GetValue(ColumnsProperty);

        public event RoutedPropertyChangedEventHandler<object> SelectedItemChanged;

        private void ListViewSelectedItemChanged(
            object sender,
            RoutedPropertyChangedEventArgs<object> e)
        {
            RoutedPropertyChangedEventHandler<object> selectedItemChanged = SelectedItemChanged;
            selectedItemChanged?.Invoke(this, e);
        }

        private void TreeListViewControlLoaded(object sender, RoutedEventArgs e)
        {
            ListView.SetColumns(Columns);
        }

        private void ScrollContentPresenterRequestBringIntoView(
            object sender,
            RequestBringIntoViewEventArgs e)
        {
            e.Handled = ListView.IsKeyboardFocusWithin;
        }

        private void TreeViewSelectedItemChanged(object sender, RoutedEventArgs e)
        {
            TreeViewItem treeViewItem = sender as TreeViewItem;
            treeViewItem?.BringIntoView();

            e.Handled = true;
        }
    }
}
