using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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
using Extensions;
using ICSStudio.Gui.View;
using ICSStudio.MultiLanguage;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using Task = System.Threading.Tasks.Task;

namespace ICSStudio.EditorPackage.Trend
{
    /// <summary>
    /// Trend.xaml 的交互逻辑
    /// </summary>
    public partial class Trend : UserControl
    {
        private readonly TextBlock _measureTextBlock;
        private bool _isInit;
        public Trend()
        {
            _measureTextBlock=new TextBlock();
            _measureTextBlock.FontSize = 12;
            InitializeComponent();
            Loaded += Trend_Loaded;
            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);

        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        internal IVsWindowFrame Frame { set; get; }

        private void Trend_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isInit)
            {
                var mainDock = VisualTreeHelpers.FindVisualParentByName(this, "MainDockTarget");
                var panel = VisualTreeHelpers.GetChildObject(mainDock, "PART_TabPanel") as Panel;
                if (panel != null)
                {
                    FrameworkElement item = null;
                    foreach (var child in panel.Children)
                    {
                        var tabItem = child as TabItem;
                        if (tabItem?.IsSelected ?? false)
                            item = tabItem;
                    }

                    if (item != null)
                    {
                        var button = VisualTreeHelpers.GetChildObject(item, "HideButton") as Button;
                        if (button != null)
                            button.PreviewMouseLeftButtonUp += Button_PreviewMouseLeftButtonUp; ;
                    }
                    _isInit = true;
                }
                
            }
            var vm = (TrendViewModel)DataContext;
            vm.Update(false,false,false);
        }

        private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var vm = DataContext as TrendViewModel;
            if (vm == null) return;
            if (vm.IsRun)
            {
                var result = MessageBox.Show(
                    LanguageManager.GetInstance().ConvertSpecifier("TrendCloseMessage"),
                    vm.Trend.Name, MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.Yes)
                {
                    ((Button) sender).PreviewMouseLeftButtonUp -= Button_PreviewMouseLeftButtonUp;
                    Frame?.CloseFrame((int) __FRAMECLOSE.FRAMECLOSE_NoSave);
                }
                else
                {

                }
            }
        }

        public void CalculateHeight()
        {
            if (!((TrendViewModel)DataContext).Trend.Isolated) return;
#pragma warning disable VSTHRD001 // 避免旧线程切换 API
            Dispatcher.InvokeAsync(() =>
#pragma warning restore VSTHRD001 // 避免旧线程切换 API
            {
                var list = new List<PlotModel>();
                foreach (PlotModel item in IsolatedPlotModels.Items)
                {
                    if (item.Visibility == Visibility.Visible) list.Add(item);
                }

                var itemHeight = (IsolatedPlotModels.ActualHeight - (30 + 17)) / list.Count;
                if (itemHeight <= 0) return;
                foreach (PlotModel item in list)
                {
                    if (item.PlotView == null) return;
                    var view = ((PlotTrendView) item.PlotView);
                    if (item.ShowTitle) view.Height = itemHeight + 30;
                    else if (item.DisplayX) view.Height = itemHeight + 17;
                    else view.Height = itemHeight;
                }
            }, DispatcherPriority.Background);
        }

        private void ResizeButton(StackPanel panel, double actualWidth)
        {
            var span = actualWidth / 7;
            var min = span - 64;
            var margin = Math.Max(min, 10);
            double width = 64;
            if (Math.Abs(margin - 10) < double.Epsilon)
            {
                width = Math.Max(0, span - 10);
            }

            int index = 0;
            foreach (Control child in panel.Children)
            {
                child.Width = width;
                if (index == 0)
                    child.Margin = new Thickness(40, 0, margin, 0);
                else
                    child.Margin = new Thickness(0, 0, margin, 0);
                index++;
            }
        }

        private void PlotView_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeButton(Buttons,
                (sender as PlotTrendView)?.ActualWidth ?? ((ItemsControl) sender).ActualWidth);
            if (sender is ItemsControl)
                CalculateHeight();
        } 

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            CalculateHeight();
        }

        private void UIElement_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool) e.NewValue) return;
            CalculateHeight();
        }

        private int _verticalOffset;
        private void ScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            _verticalOffset = (int) e.VerticalOffset;
            SetBottomCollection((int)e.VerticalOffset, (int)e.VerticalChange);
        }

        private void SetBottomCollection(int index,int change)
        {
            try
            {
                var item = GetVisible(index);
                if (item == null) return;
                var grid = VisualTreeHelper.GetChild(item, 0);
                for (int i = 0; i < 5; i++)
                {
                    var border = VisualTreeHelper.GetChild(grid, i) as Border;
                    if (border != null)
                    {
                        border.Visibility = Visibility.Visible;
                    }
                }

                var trend = ((TrendViewModel) DataContext).Trend;
                var viewableCount = Math.Min(trend.MaxViewable, trend.Pens.Count(p => p.Visible));

                if (change != 0)
                {
                    item = MaxMinBottomControl.ItemContainerGenerator.ContainerFromIndex(index - change);
                    grid = VisualTreeHelper.GetChild(item, 0);
                    for (int i = 0; i < 5; i++)
                    {
                        var border = VisualTreeHelper.GetChild(grid, i) as Border;
                        if (border != null)
                        {
                            border.Visibility = Visibility.Hidden;
                        }
                    }
                }

                ResetBottomViewable(viewableCount);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void ResetBottomViewable(int viewableCount)
        {
            for (int j = _verticalOffset + 1; j < _verticalOffset + viewableCount; j++)
            {
                var item = MaxMinBottomControl.ItemContainerGenerator.ContainerFromIndex(j);
                var grid = VisualTreeHelper.GetChild(item, 0);
                for (int i = 0; i < 5; i++)
                {
                    var border = VisualTreeHelper.GetChild(grid, i) as Border;
                    if (border != null)
                    {
                        border.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private DependencyObject GetVisible(int index)
        {
            try
            {
                if (index >= MaxMinBottomControl.ItemContainerGenerator.Items.Count || index < 0) return null;
                var item = (ContentPresenter)MaxMinBottomControl.ItemContainerGenerator.ContainerFromIndex(index);
                if (item == null) return null;
                var visible = ((MaxMinData)item.DataContext).IsVisible;
                if (!visible) return GetVisible(++index);
                return item;
            }
            catch (Exception e)
            {
                Debug.Assert(false,e.Message);
            }

            return null;
        }

        private void ContextMenu_OnOpened(object sender, RoutedEventArgs e)
        {
            var contextMenu = sender as ContextMenu;
            if (contextMenu != null)
            {
                LanguageManager.GetInstance().SetLanguage(contextMenu);
            }
        }
    }
}
