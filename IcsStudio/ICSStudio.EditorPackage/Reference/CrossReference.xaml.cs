using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ICSStudio.MultiLanguage;

namespace ICSStudio.EditorPackage.Reference
{
    /// <summary>
    ///     CrossReference.xaml 的交互逻辑
    /// </summary>
    public partial class CrossReference : UserControl
    {
        private GridViewColumnHeader _lastHeaderClicked;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;

        public CrossReference()
        {
            InitializeComponent();
            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        private void GridViewColumnHeaderClickedHandler(object sender,
            RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            var listViewSort = e.Source as ListView;
            if (listViewSort == null) return;

            if (headerClicked != null)
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    ListSortDirection direction;
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                            direction = ListSortDirection.Descending;
                        else
                            direction = ListSortDirection.Ascending;
                    }

                    var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                    var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

                    Sort(listViewSort, sortBy, direction);

                    if (direction == ListSortDirection.Ascending)
                        headerClicked.Column.HeaderTemplate =
                            Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    else
                        headerClicked.Column.HeaderTemplate =
                            Resources["HeaderTemplateArrowDown"] as DataTemplate;

                    // Remove arrow from previously sorted header
                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                        _lastHeaderClicked.Column.HeaderTemplate = null;

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
        }

        private void Sort(ListView listViewSort, string sortBy, ListSortDirection direction)
        {
            if (listViewSort.ItemsSource == null) return;
            var dataView =
                CollectionViewSource.GetDefaultView(listViewSort.ItemsSource);

            dataView.SortDescriptions.Clear();
            var sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }
    }
}