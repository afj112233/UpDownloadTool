//------------------------------------------------------------------------------
// <copyright file="ControllerOrganizerView.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.OrganizerPackage.Commands;
using ICSStudio.OrganizerPackage.ViewModel;
using ICSStudio.UIInterfaces.Project;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using ScrollBar = System.Windows.Controls.Primitives.ScrollBar;
using ToolBar = System.Windows.Controls.ToolBar;
using ICSStudio.MultiLanguage;

//TODO(gjc):1.整行选中 
//TODO(gjc):2.图标 
//TODO(gjc):3.右键菜单 

namespace ICSStudio.OrganizerPackage.View
{
    /// <summary>
    /// Interaction logic for ControllerOrganizerView.
    /// </summary>
    public partial class ControllerOrganizerView
    {
        private Package _package;

        private ITrackSelection _track;

        private readonly ControllerOrganizerVM _viewModel;

        private double _treeViewRowActualHeight;
        private double _quickViewRowActualHeight;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerOrganizerView"/> class.
        /// </summary>
        public ControllerOrganizerView()
        {
            InitializeComponent();

            _viewModel = new ControllerOrganizerVM();
            DataContext = _viewModel;

            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        private void LanguageChanged(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        public void Initialize(Package package, ITrackSelection track)
        {
            _package = package;
            _track = track;

            _viewModel.Initialize(package, track);
        }

        private static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);

            return source;
        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            // remove small arrow on the right
            ToolBar toolBar = sender as ToolBar;
            if (toolBar == null)
                return;

            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }

            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness(0);
            }
        }

        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;

            if (treeViewItem == null)
                return;

            treeViewItem.IsSelected = true;
        }

        private void TreeViewItem_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            OrganizerItem organizerItem = (OrganizerItem) treeViewItem?.DataContext;

            if (organizerItem == null || organizerItem.DisplayName.Equals("<none>") ||
                organizerItem.DisplayName.Equals("Associated Axes") ||
                !organizerItem.IsSelected && organizerItem.Kind == ProjectItemType.DeviceModule) return;

            IServiceProvider serviceProvider = _package;

            IMenuCommandService commandService =
                (OleMenuCommandService) serviceProvider?.GetService(typeof(IMenuCommandService));

            if (commandService != null)
            {
                Point screenPoint = PointToScreen(e.GetPosition(this));
                commandService.ShowContextMenu(
                    new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.controllerContextMenu),
                    (int) screenPoint.X,
                    (int) screenPoint.Y);
            }
        }

        private void TreeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            ShowPropertiesWindow(sender, e);
        }

        private void ShowPropertiesWindow(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var item = sender as TreeViewItem;
            if (item == null || !item.IsSelected)
                return;

            OrganizerItem organizerItem = item.Header as OrganizerItem;
            if ((organizerItem?.AssociatedObject as IDataType)?.Name == "BOOL" ||
                (organizerItem?.AssociatedObject as IDataType)?.Name == "DINT" ||
                (organizerItem?.AssociatedObject as IDataType)?.Name == "INT" ||
                (organizerItem?.AssociatedObject as IDataType)?.Name == "LINT" ||
                (organizerItem?.AssociatedObject as IDataType)?.Name == "SINT" ||
                (organizerItem?.AssociatedObject as IDataType)?.Name == "REAL") return;

            if (organizerItem != null)
            {
                if (organizerItem.Kind == ProjectItemType.LogicalModelView)
                {
                    OrganizerCommand.Instance.ShowLogicalOrganizerToolWindow(sender, e);
                }
                else if (organizerItem.Kind == ProjectItemType.Routine)
                {
                    ContextMenuCommand.Instance.Open(sender, e);
                }
                else if (organizerItem.Kind == ProjectItemType.ControllerTags ||
                         organizerItem.Kind == ProjectItemType.ProgramTags)
                {
                    ContextMenuCommand.Instance.MonitorTags(sender, e);
                }
                else if (organizerItem.Kind == ProjectItemType.DeviceModule)
                {
                    IDeviceModule deviceModule = organizerItem.AssociatedObject as IDeviceModule;
                    if (deviceModule != null)
                    {
                        if (deviceModule.Name.Equals("Local"))
                        {
                            PropertiesCommand.Instance.ShowControllerPropertiesWindow(sender, e);
                        }
                        else
                        {
                            PropertiesCommand.Instance.ShowPropertiesWindow(sender, e);
                        }
                    }
                }
                else if (organizerItem.Kind == ProjectItemType.Trend)
                {
                    ContextMenuCommand.Instance.Open(this, EventArgs.Empty);
                }
                else
                {
                    PropertiesCommand.Instance.ShowPropertiesWindow(sender, e);
                }
            }
        }

        private void ControllerOrganizerView_OnGotFocus(object sender, RoutedEventArgs e)
        {
            CurrentObject.GetInstance().Current = DataContext;
        }

        private void RangeBase_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var scrollBar = (ScrollBar) sender;
            if (scrollBar != null)
            {
                scrollBar.Value = e.NewValue < 0.5 ? 0 : 1;

                QuickViewDataGrid.Columns[0].Visibility =
                    scrollBar.Value == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void TreeViewItem_KeyDown(object sender, KeyEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (e.Key == Key.Enter)
            {
                ShowPropertiesWindow(sender, e);
                e.Handled = true;
            }
        }

        private void HideOrShowQuickViewButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (QuickViewRow.Height.Value == 0)
            {
                if (_quickViewRowActualHeight == 0)
                {
                    TreeViewRow.Height = new GridLength(6, GridUnitType.Star);
                    QuickViewRow.Height = new GridLength(4, GridUnitType.Star);
                }
                else
                {
                    TreeViewRow.Height = new GridLength(_treeViewRowActualHeight, GridUnitType.Star);
                    QuickViewRow.Height = new GridLength(_quickViewRowActualHeight, GridUnitType.Star);
                }
            }
            else
            {
                _treeViewRowActualHeight = TreeViewRow.ActualHeight;
                _quickViewRowActualHeight = QuickViewRow.ActualHeight;

                QuickViewRow.Height = new GridLength(0, GridUnitType.Star);
            }
        }

        private void TreeViewItem_RequestBring(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            double height = QuickViewRow.Height.Value;
            HideOrShowQuickViewMenuItem.Header = height == 0
                ? LanguageManager.GetInstance().ConvertSpecifier("ShowQuickView") : LanguageManager.GetInstance().ConvertSpecifier("HideQuickView");
        }
    }
}