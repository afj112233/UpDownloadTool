using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;

namespace ICSStudio.Gui.Controls
{
    public class TreeViewExtensions : DependencyObject
    {
        /// <summary>
        /// Gets the value of the dependency property "EnableMultiSelect".
        /// </summary>
        /// <param name="obj">Dependency Object</param>
        /// <returns></returns>
        public static bool GetEnableMultiSelect(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableMultiSelectProperty);
        }

        /// <summary>
        /// Sets the value of the dependency property "EnableMultiSelect".
        /// </summary>
        /// <param name="obj">Dependency Object</param>
        /// <param name="value"></param>
        public static void SetEnableMultiSelect(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableMultiSelectProperty, value);
        }

        // Using a DependencyProperty as the backing store for EnableMultiSelect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableMultiSelectProperty =
            DependencyProperty.RegisterAttached("EnableMultiSelect", typeof(bool), typeof(TreeViewExtensions), new FrameworkPropertyMetadata(false)
            {
                PropertyChangedCallback = EnableMultiSelectChanged,
                BindsTwoWayByDefault = true
            });

        /// <summary>
        /// Gets the value of the dependency property "SelectedItems".
        /// </summary>
        /// <param name="obj">Dependency Object</param>
        /// <returns></returns>
        public static IList GetSelectedItems(DependencyObject obj)
        {
            return (IList)obj.GetValue(SelectedItemsProperty);
        }

        /// <summary>
        /// Sets the value of the dependency property "SelectedItems".
        /// </summary>
        /// <param name="obj">Dependency Object</param>
        /// <param name="value"></param>
        public static void SetSelectedItems(DependencyObject obj, IList value)
        {
            obj.SetValue(SelectedItemsProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectedItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.RegisterAttached("SelectedItems", typeof(IList), typeof(TreeViewExtensions), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of the dependency property "AnchorItem".
        /// </summary>
        /// <param name="obj">Dependency Object</param>
        /// <returns></returns>
        static TreeViewItem GetAnchorItem(DependencyObject obj)
        {
            return (TreeViewItem)obj.GetValue(AnchorItemProperty);
        }

        /// <summary>
        /// Sets the value of the dependency property "AnchorItem".
        /// </summary>
        /// <param name="obj">Dependency Object</param>
        /// <param name="value"></param>
        static void SetAnchorItem(DependencyObject obj, TreeViewItem value)
        {
            obj.SetValue(AnchorItemProperty, value);
        }

        // Using a DependencyProperty as the backing store for AnchorItem.  This enables animation, styling, binding, etc...
        static readonly DependencyProperty AnchorItemProperty = DependencyProperty.RegisterAttached("AnchorItem", typeof(TreeViewItem), typeof(TreeViewExtensions), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of the dependency property "IsSelected".
        /// </summary>
        /// <param name="obj">Dependency Object</param>
        /// <returns></returns>
        public static bool GetIsSelected(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSelectedProperty);
        }

        /// <summary>
        /// Sets the value of the dependency property "IsSelected".
        /// </summary>
        /// <param name="obj">Dependency Object</param>
        /// <param name="value"></param>
        public static void SetIsSelected(DependencyObject obj, bool value)
        {
            var item = obj as TreeViewItem;
            if (item == null)
                return;
            if (value)
            {
                // ReSharper disable once PossibleNullReferenceException
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0078D7"));
                item.Background = brush;
            }
            else
            {
                item.Background = Brushes.Transparent;
            }

            obj.SetValue(IsSelectedProperty, value);

        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.RegisterAttached("IsSelected", typeof(bool), typeof(TreeViewExtensions), new PropertyMetadata(false)
            {
                PropertyChangedCallback = RealSelectedChanged
            });

        public static string GetNodeType(DependencyObject obj)
        {
            return (string)obj.GetValue(NodeTypeProperty);
        }

        public static void SetNodeType(DependencyObject obj, string value)
        {
            obj.SetValue(NodeTypeProperty, value);
        }

        // Using a DependencyProperty as the backing store for NodeType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NodeTypeProperty =
            DependencyProperty.RegisterAttached("NodeType", typeof(string), typeof(TreeViewExtensions));

        /// <summary>
        /// "EnableMultiSelect" changed event.
        /// </summary>
        /// <param name="s">Dependency Object</param>
        /// <param name="args">Event parameter</param>
        static void EnableMultiSelectChanged(DependencyObject s, DependencyPropertyChangedEventArgs args)
        {
            TreeView tree = (TreeView)s;
            var wasEnable = (bool)args.OldValue;
            var isEnabled = (bool)args.NewValue;
            if (wasEnable)
            {
                tree.RemoveHandler(UIElement.PreviewMouseDownEvent, new MouseButtonEventHandler(ItemClicked));
                tree.RemoveHandler(UIElement.KeyDownEvent, new KeyEventHandler(KeyDown));
            }
            if (isEnabled)
            {
                tree.AddHandler(UIElement.PreviewMouseDownEvent, new MouseButtonEventHandler(ItemClicked), true);//UIElement.MouseDownEvent
                tree.AddHandler(UIElement.KeyDownEvent, new KeyEventHandler(KeyDown), true);
            }
        }

        /// <summary>
        /// Gets TreeView which contains the TreeViewItem.
        /// </summary>
        /// <param name="item">item</param>
        /// <returns>TreeView</returns>
        static TreeView GetTree(TreeViewItem item)
        {
            Func<DependencyObject, DependencyObject> getParent = VisualTreeHelper.GetParent;
            FrameworkElement currentItem = item;
            while (!(getParent(currentItem) is TreeView))
            {
                currentItem = (FrameworkElement)getParent(currentItem);
            }
            return (TreeView)getParent(currentItem);
        }

        /// <summary>
        /// TreeViewItem selected changed event.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="args">event parameter</param>
        static void RealSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            TreeViewItem item = (TreeViewItem)sender;

            var selectedItems = GetSelectedItems(GetTree(item));
            var isSelected = GetIsSelected(item);
            if (selectedItems != null)
            {
                if (isSelected)
                {
                    try
                    {
                        selectedItems.Add(item.Header);
                    }
                    catch (ArgumentException)
                    {
                    }
                }
                else
                {
                    selectedItems.Remove(item.Header);
                }
            }
            else if (isSelected)
            {
                try
                {
                    SetSelectedItems(GetTree(item), new List<object>() { item.Header });
                }
                catch (ArgumentException)
                {
                }
            }
        }

        /// <summary>
        /// Key down event.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event parameter</param>
        static void KeyDown(object sender, KeyEventArgs e)
        {
            //暂时不需要全选快捷键
            //TreeView tree = (TreeView)sender;
            //if (e.Key == Key.A && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            //{
            //    foreach (var item in GetExpandedTreeViewItems(tree))
            //    {
            //        SetIsSelected(item, true);
            //    }
            //    e.Handled = true;
            //}
            var tree = (TreeView)sender;
            var expandedItems = GetExpandedTreeViewItems(tree);

            var treeViewItem = e.OriginalSource as TreeViewItem;
            if (treeViewItem == null)
                return;

            TreeViewItem targetItem = null;
            //int offset = 0;
            switch (e.Key)
            {
                case Key.Down:
                    targetItem = GetRelativeItem(expandedItems.ToList(), treeViewItem, 1);
                    //offset = 1;
                    break;
                case Key.Up:
                    targetItem = GetRelativeItem(expandedItems.ToList(), treeViewItem, -1);
                    //offset = -1;
                    break;
            }

            if (targetItem == null)
                return;

            //switch (Keyboard.Modifiers)
            //{
            //case ModifierKeys.Control:
            //    MakeToggleSelection(tree, targetItem);
            //    break;
            //case ModifierKeys.Shift:
            //    MakeAnchorSelection(tree, targetItem, true);
            //    break;
            //case ModifierKeys.None:
            MakeSingleSelection(tree, targetItem);
            //        break;
            //}

            //TreeViewAutomationPeer lvap = new TreeViewAutomationPeer(tree);
            //var svap = lvap.GetPattern(PatternInterface.Scroll) as ScrollViewerAutomationPeer;
            //var scroll = svap.Owner as ScrollViewer;
            //var scrollHeight = scroll.ActualHeight;
            //var treeHeight = tree.ActualHeight;
            //scroll.ScrollToVerticalOffset(scroll.VerticalOffset + offset*targetItem.ActualHeight);
        }

        private static TreeViewItem GetRelativeItem(List<TreeViewItem> expandedItems, TreeViewItem item, int relativePosition)
        {
            if (item != null)
            {
                int index = expandedItems.IndexOf(item);
                if (index >= 0)
                {
                    var relativeIndex = index + relativePosition;
                    if (relativeIndex >= 0 && relativeIndex < expandedItems.Count)
                    {
                        return expandedItems[relativeIndex];
                    }
                }
            }

            return null;
        }

        private static readonly List<string> EnabledMultiSelectionCollection = new List<string>() { "Program", "AddOnInstruction", "UserDefined", "String", "Trend" };

        /// <summary>
        /// Item clicked event.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event parameter</param>
        static void ItemClicked(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = FindTreeViewItem(e.OriginalSource);
            if (item == null)
            {
                return;
            }
            TreeView tree = (TreeView)sender;

            var mouseButton = e.ChangedButton;
            if (mouseButton != MouseButton.Left)
            {
                if ((mouseButton == MouseButton.Right) && ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) == ModifierKeys.None))
                {
                    if (GetIsSelected(item))
                    {
                        UpdateAnchorAndActionItem(tree, item);
                        return;
                    }
                    MakeSingleSelection(tree, item);
                }
                return;
            }
            if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != (ModifierKeys.Shift | ModifierKeys.Control))
            {
                if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    //先判断是否同级，不是同级则变为单选，同级则继续多选。
                    if (IsSameLv(tree, item))
                        MakeToggleSelection(tree, item);
                    else
                        MakeSingleSelection(tree, item);

                    return;
                }

                if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    if (IsSameLv(tree, item))
                        MakeAnchorSelection(tree, item, true);
                    else
                        MakeSingleSelection(tree, item);
                    return;
                }
                MakeSingleSelection(tree, item);
            }
        }

        static bool IsSameLv(TreeView tree, TreeViewItem item)
        {
            bool isSame = true;
            foreach (TreeViewItem selectedItem in GetExpandedTreeViewItems(tree))
            {
                if (selectedItem == null)
                {
                    continue;
                }
                if (selectedItem == item)
                {
                    //遍历到当前点击的节点
                    continue;
                }

                if (GetIsSelected(selectedItem))
                {
                    Debug.WriteLine(GetNodeType(item));
                    //遍历到已选中的节点
                    if (GetNodeType(selectedItem) != GetNodeType(item))
                    {
                        isSame = false;
                        break;
                    }

                    if (GetParentItem(selectedItem) != GetParentItem(item))
                    {
                        isSame = false;
                        break;
                    }

                    if (!EnabledMultiSelectionCollection.Contains(GetNodeType(item)))
                    {
                        isSame = false;
                        break;
                    }
                }
            }
            return isSame;
        }

        /// <summary>
        /// Find TreeViewItem which contains the object.
        /// </summary>
        /// <param name="obj">obj</param>
        /// <returns></returns>
        private static TreeViewItem FindTreeViewItem(object obj)
        {
            DependencyObject dpObj = obj as DependencyObject;
            if (dpObj == null)
            {
                return null;
            }
            if (dpObj is TreeViewItem)
            {
                return (TreeViewItem)dpObj;
            }
            return FindTreeViewItem(VisualTreeHelper.GetParent(dpObj));
        }

        /// <summary>
        /// Gets all expanded TreeViewItems.
        /// </summary>
        /// <param name="tree">TreeView</param>
        /// <returns></returns>
        private static IEnumerable<TreeViewItem> GetExpandedTreeViewItems(ItemsControl tree)
        {
            for (int i = 0; i < tree.Items.Count; i++)
            {
                var item = (TreeViewItem)tree.ItemContainerGenerator.ContainerFromIndex(i);
                if (item == null)
                {
                    continue;
                }
                yield return item;
                if (item.IsExpanded)
                {
                    foreach (var subItem in GetExpandedTreeViewItems(item))
                    {
                        yield return subItem;
                    }
                }
            }
        }

        private static void MakeAnchorSelection(TreeView tree, TreeViewItem actionItem, bool clearCurrent)
        {
            var anchor = GetAnchorItem(tree);
            if (anchor == actionItem)
            {
                MakeSingleSelection(tree, actionItem);
                return;
            }

            var items = GetSameLevelItems(actionItem);
            bool betweenBoundary = false;
            foreach (var item in items)
            {
                bool isBoundary = item == anchor || item == actionItem;
                if (isBoundary)
                {
                    betweenBoundary = !betweenBoundary;
                }
                if (betweenBoundary || isBoundary)
                {
                    SetIsSelected(item, true);
                }
                else
                {
                    if (clearCurrent)
                    {
                        SetIsSelected(item, false);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private static TreeViewItem GetParentItem(TreeViewItem item)
        {
            Func<DependencyObject, DependencyObject> getParent = (o) => VisualTreeHelper.GetParent(o);
            FrameworkElement currentItem = item;
            while (!(getParent(currentItem) is TreeViewItem))
            {
                currentItem = (FrameworkElement)getParent(currentItem);
            }
            return (TreeViewItem)getParent(currentItem);
        }

        private static List<TreeViewItem> GetSameLevelItems(TreeViewItem cur)
        {
            List<TreeViewItem> items = new List<TreeViewItem>();
            TreeViewItem parent = GetParentItem(cur);
            for (int i = 0; i < parent.Items.Count; i++)
            {
                var item = (TreeViewItem)parent.ItemContainerGenerator.ContainerFromIndex(i);
                if (item == null)
                {
                    continue;
                }
                items.Add(item);
            }

            return items;
        }

        /// <summary>
        /// Select by left mouse button.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="item"></param>
        public static void MakeSingleSelection(TreeView tree, TreeViewItem item)
        {
            foreach (TreeViewItem selectedItem in GetExpandedTreeViewItems(tree))
            {
                if (selectedItem == null)
                {
                    continue;
                }
                if (selectedItem != item)
                {
                    SetIsSelected(selectedItem, false);
                }
                else
                {
                    SetIsSelected(selectedItem, true);
                }
            }
            UpdateAnchorAndActionItem(tree, item);
        }

        /// <summary>
        /// Select by Ctrl key.
        /// </summary>
        /// <param name="tree">TreeView</param>
        /// <param name="item">TreeViewItem</param>
        private static void MakeToggleSelection(TreeView tree, TreeViewItem item)
        {
            SetIsSelected(item, !GetIsSelected(item));
            UpdateAnchorAndActionItem(tree, item);
        }

        /// <summary>
        /// Update the Anchor TreeViewItem.
        /// </summary>
        /// <param name="tree">TreeView</param>
        /// <param name="item">TreeViewItem</param>
        private static void UpdateAnchorAndActionItem(TreeView tree, TreeViewItem item)
        {
            SetAnchorItem(tree, item);
        }

        /// <summary>
        ///     Recursively search for an item in this subtree.
        /// </summary>
        /// <param name="container">
        ///     The parent ItemsControl. This can be a TreeView or a TreeViewItem.
        /// </param>
        /// <param name="item">
        ///     The item to search for.
        /// </param>
        /// <returns>
        ///     The TreeViewItem that contains the specified item.
        /// </returns>
        public static TreeViewItem GetTreeViewItem(ItemsControl container, object item)
        {
            if (container != null)
            {
                if (container.DataContext == item) return container as TreeViewItem;

                // Expand the current container
                if (container is TreeViewItem && !((TreeViewItem)container).IsExpanded)
                    container.SetValue(TreeViewItem.IsExpandedProperty, true);

                // Try to generate the ItemsPresenter and the ItemsPanel.
                // by calling ApplyTemplate.  Note that in the
                // virtualizing case even if the item is marked
                // expanded we still need to do this step in order to
                // regenerate the visuals because they may have been virtualized away.

                container.ApplyTemplate();
                var itemsPresenter =
                    (ItemsPresenter)container.Template.FindName("ItemsHost", container);
                if (itemsPresenter != null)
                {
                    itemsPresenter.ApplyTemplate();
                }
                else
                {
                    // The Tree template has not named the ItemsPresenter,
                    // so walk the descendents and find the child.
                    itemsPresenter = FindVisualChild<ItemsPresenter>(container);
                    if (itemsPresenter == null)
                    {
                        container.UpdateLayout();

                        itemsPresenter = FindVisualChild<ItemsPresenter>(container);
                    }
                }

                var itemsHostPanel = (Panel)VisualTreeHelper.GetChild(itemsPresenter, 0);

                // Ensure that the generator for this panel has been created.
                var children = itemsHostPanel.Children;

                var virtualizingPanel =
                    itemsHostPanel as MyVirtualizingStackPanel;

                for (int i = 0, count = container.Items.Count; i < count; i++)
                {
                    TreeViewItem subContainer;
                    if (virtualizingPanel != null)
                    {
                        // Bring the item into view so
                        // that the container will be generated.
                        virtualizingPanel.BringIntoView(i);

                        subContainer =
                            (TreeViewItem)container.ItemContainerGenerator.ContainerFromIndex(i);
                    }
                    else
                    {
                        subContainer =
                            (TreeViewItem)container.ItemContainerGenerator.ContainerFromIndex(i);

                        // Bring the item into view to maintain the
                        // same behavior as with a virtualizing panel.
                        subContainer.BringIntoView();
                    }

                    if (subContainer != null)
                    {
                        // Search the next level for the object.
                        var resultContainer = GetTreeViewItem(subContainer, item);
                        if (resultContainer != null)
                            return resultContainer;
                        subContainer.IsExpanded = false;
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///     Search for an element of a certain type in the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of element to find.</typeparam>
        /// <param name="visual">The parent element.</param>
        /// <returns></returns>
        public static T FindVisualChild<T>(Visual visual) where T : Visual
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            {
                var child = (Visual)VisualTreeHelper.GetChild(visual, i);
                var correctlyTyped = child as T;
                if (correctlyTyped != null) return correctlyTyped;

                var descendent = FindVisualChild<T>(child);
                if (descendent != null) return descendent;
            }

            return null;
        }

        public class MyVirtualizingStackPanel : VirtualizingStackPanel
        {
            /// <summary>
            ///     Publically expose BringIndexIntoView.
            /// </summary>
            public void BringIntoView(int index)
            {
                BringIndexIntoView(index);
            }
        }
    }
}