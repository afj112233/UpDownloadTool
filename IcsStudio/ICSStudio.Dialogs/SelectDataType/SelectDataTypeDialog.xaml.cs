using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;

namespace ICSStudio.Dialogs.SelectDataType
{
    /// <summary>
    ///     SelectDataTypeDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SelectDataTypeDialog
    {
        private readonly SelectDataTypeViewModel _viewModel;

        public SelectDataTypeDialog(
            IController controller,
            string dataType,
            bool supportsOneDimensionalArray,
            bool supportsMultiDimensionalArrays)
        {
            InitializeComponent();

            _viewModel = new SelectDataTypeViewModel(
                controller, dataType, supportsOneDimensionalArray, supportsMultiDimensionalArrays);

            DataContext = _viewModel;
            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        public string DataType => _viewModel.DataType;

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(NameBox);
        }

        private void ScrollIntoView()
        {
            if (LbxDataTypes.Visibility == Visibility.Visible && LbxDataTypes.SelectedItem != null)
                LbxDataTypes.ScrollIntoView(LbxDataTypes.SelectedItem);

            if (TvwDataTypes.Visibility == Visibility.Visible && TvwDataTypes.SelectedItem != null)
            {
                var treeViewItem =
                    ContainerFromItemRecursive(TvwDataTypes.ItemContainerGenerator, TvwDataTypes.SelectedItem);
                treeViewItem?.BringIntoView();
            }
        }

        private void LbxDataTypes_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ScrollIntoView();
        }

        private void TvwDataTypes_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ScrollIntoView();
        }

        private void LbxDataTypes_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ScrollIntoView();
        }

        private void TvwDataTypes_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ScrollIntoView();
        }

        private static TreeViewItem ContainerFromItemRecursive(ItemContainerGenerator root, object item)
        {
            var treeViewItem = root.ContainerFromItem(item) as TreeViewItem;
            if (treeViewItem != null)
                return treeViewItem;
            foreach (var subItem in root.Items)
            {
                treeViewItem = root.ContainerFromItem(subItem) as TreeViewItem;
                if (treeViewItem != null)
                {
                    var search = ContainerFromItemRecursive(treeViewItem.ItemContainerGenerator, item);
                    if (search != null)
                        return search;
                }
            }

            return null;
        }

        private void TreeViewItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _viewModel.ExecuteOkCommand();
        }

        private void ListBoxItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _viewModel.ExecuteOkCommand();
        }
    }
}