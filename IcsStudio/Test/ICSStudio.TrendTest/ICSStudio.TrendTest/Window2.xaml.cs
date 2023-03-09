using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;
using TimeSpanAxis = OxyPlot.Axes.TimeSpanAxis;

namespace ICSStudio.TrendTest
{
    /// <summary>
    /// Window2.xaml 的交互逻辑
    /// </summary>
    public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();
            DataContext = this;
            CreatePlotModel();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            timer.Tick += Timer_Tick;
            //timer.Start();
            //isStop = false;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DataPoint dataPoint = new DataPoint(this.x, x * this.x);
            this.x += 1;
            int index = 0;
            foreach (var plotModel in PlotModels)
            {
                if (index == 0)
                    (plotModel.Series[0] as ContinueSeries)?.AddPoint(new DataPoint(x - 1, x));
                else
                    (plotModel.Series[0] as ContinueSeries)?.AddPoint(dataPoint);
                index++;
            }

            //this.plotModel.Update();
            this.Update();
        }

        private void Update()
        {
            foreach (var plotModel in PlotModels)
            {
                plotModel.PlotView?.InvalidatePlot();
            }

            ItemsControl itemsControl = Trends;
            //double titleHeight = 0;
            //if (itemsControl.Items.Count > 0)
            //{
            //    titleHeight = ((PlotModel) itemsControl.Items[0]).TitleArea.Height;
            //}
            var itemHeight =
                (itemsControl.ActualHeight - PlotModels[0].TitleArea.Height -
                 PlotModels[itemsControl.Items.Count - 1].Padding.Bottom) / itemsControl.Items.Count;
            double maxLeft = 0;
            foreach (PlotModel item in itemsControl.Items)
            {
                //(item.PlotView as PlotTrendView).Height = itemHeight;
                int index = itemsControl.Items.IndexOf(item);
                if (index == 0) (item.PlotView as PlotTrendView).Height = itemHeight + item.TitleArea.Height;
                else if (index == itemsControl.Items.Count - 1)
                    (item.PlotView as PlotTrendView).Height = itemHeight + item.Padding.Bottom;
                else (item.PlotView as PlotTrendView).Height = itemHeight;
                maxLeft = Math.Max(item.ActualPlotMargins.Left, maxLeft);
                if (maxLeft > 0)
                {

                }

                if (index > 0)
                {
                    //var tt = (item.PlotView as PlotTrendView).Margin;
                    //(item.PlotView as PlotTrendView).Margin=new Thickness(0, -10,0,0);
                }
            }

            foreach (PlotModel item in itemsControl.Items)
            {
                item.AxisYMaxLeft = maxLeft;
                item.Update();
            }
        }

        private DispatcherTimer timer;

        //private bool isStop = true;

        private double x = 1;

        public ObservableCollection<PlotModel> PlotModels { get; } = new ObservableCollection<PlotModel>();

        private void CreatePlotModel()
        {
            int count = 4;
            for (int i = 0; i < count; i++)
            {
                var plotModel = this.CreatePlot(i == count - 1);
                plotModel.OnMouseWheel += OnMouseWheel;
                var series = new ContinueSeries();
                plotModel.ShowExtremity = true;
                plotModel.Series.Add(series);
                if (i != 0)
                {
                    plotModel.ShowTitle = false;
                }
                plotModel.PlotAreaBackground = OxyColor.Parse(Colors.Black.ToString());
                PlotModels.Add(plotModel);
            }
        }

        private void OnMouseWheel(object sender, EventArgs e)
        {
            //isStop = true;
            timer.Stop();
        }

        private PlotModel CreatePlot(bool displayX)
        {
            //double x = 0;
            var pm = new PlotModel {Title = "Isolated Trend "};
            pm.InIsolated = true;
            pm.ParentCollection = PlotModels;
            if (!displayX)
                pm.Padding = new OxyThickness(9, 0, 9, 0);
            var axesX = new TimeSpanAxis();
            axesX.DisplayMillisecond = true;
            axesX.MajorGridLines = 2;
            axesX.MinorGridLines = 0;
            axesX.MajorGridlineStyle = LineStyle.Automatic;
            axesX.MajorGridlineColor = OxyColor.Parse(Colors.Red.ToString());
            axesX.MajorGridlineThickness = 2;
            axesX.Position = AxisPosition.Bottom;
            if (!displayX)
                //axesX.IsAxisVisible = false;
                pm.DisplayX = false;
            pm.Axes.Add(axesX);
            var f1 = new FunctionSeries(Math.Sin, x, x + 1, 0.2);
            //f1.BrokenLineColor=ConvertOxyColor(Colors.Red);

            //线段颜色
            f1.Color = ConvertOxyColor(Colors.Red);
            //线段样式
            f1.LineStyle = LineStyle.Dash;
            //线段粗细
            f1.StrokeThickness = 1;
            return pm;
        }

        private OxyColor ConvertOxyColor(Color color)
        {
            return OxyColor.Parse(color.ToString());
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            ((PlotTrendView) sender).PreviewMouseDown += Window2_PreviewMouseDown;
            ((PlotTrendView) sender).PreviewMouseUp += Window2_PreviewMouseUp;
            ((PlotTrendView) sender).PreviewMouseMove += Window2_PreviewMouseMove;

            //if (PlotModels.IndexOf(((PlotTrendView)sender).ActualModel) == PlotModels.Count-1)
            //{
            //    ItemsControl itemsControl = Trends;
            //    double titleHeight = 0;
            //    //if (itemsControl.Items.Count > 0)
            //    //{
            //    //    titleHeight = ((PlotModel) itemsControl.Items[0]).TitleArea.Height;
            //    //}
            //    var itemHeight = (itemsControl.ActualHeight - PlotModels[0].TitleArea.Height - PlotModels[itemsControl.Items.Count-1].Padding.Bottom) / itemsControl.Items.Count;
            //    double maxLeft = 0;
            //    foreach (PlotModel item in itemsControl.Items)
            //    {
            //        //(item.PlotView as PlotTrendView).Height = itemHeight;
            //        int index = itemsControl.Items.IndexOf(item);
            //        if (index == 0) (item.PlotView as PlotTrendView).Height = itemHeight + item.TitleArea.Height;
            //        else if (index == itemsControl.Items.Count - 1) (item.PlotView as PlotTrendView).Height = itemHeight + item.Padding.Bottom;
            //        else (item.PlotView as PlotTrendView).Height = itemHeight;
            //        maxLeft = Math.Max(item.ActualPlotMargins.Left, maxLeft);
            //        if (index > 0)
            //        {
            //            //var tt = (item.PlotView as PlotTrendView).Margin;
            //            //(item.PlotView as PlotTrendView).Margin=new Thickness(0, -10,0,0);
            //        }
            //    }

            //    foreach (PlotModel item in itemsControl.Items)
            //    {
            //        item.AxisYMaxLeft = maxLeft;
            //        item.Update();
            //    }
            //}
        }

        private void Window2_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var otherView = PlotModels.Where(p => p.PlotView != sender);
                var currentView = (sender as PlotTrendView);
                var currentPosition = e.GetPosition(currentView).ToScreenPoint();
                var result = currentView.Model.Series[0].GetNearestPoint(currentPosition, false);
                foreach (var plotModel in otherView)
                {
                    var point = (plotModel.Series[0] as ContinueSeries)?.Points.FirstOrDefault(s =>
                        Math.Abs(s.X - result.DataPoint.X) < Double.Epsilon);
                    var view = plotModel.PlotView as PlotTrendView;
                    if (point != null)
                    {
                        var sp = plotModel.DefaultXAxis.Transform(((DataPoint) point).X, ((DataPoint) point).Y,
                            plotModel.DefaultYAxis);
                        var tracker = new TrackerHitResult()
                        {
                            DataPoint = (DataPoint) point,
                            PlotModel = plotModel,
                            Series = plotModel.Series[0],
                            Position = sp,
                            Text =
                                $"\nX:{plotModel.DefaultXAxis.GetValue(((DataPoint) point).X)}\nY:{plotModel.DefaultYAxis.GetValue(((DataPoint) point).Y)}"
                        };
                        view?.ShowTracker(tracker);
                    }
                }
            }
        }

        private void Window2_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var otherView = PlotModels.Where(p => p.PlotView != sender);
            foreach (var plotModel in otherView)
            {
                var view = plotModel.PlotView as PlotTrendView;
                view?.HideTracker();
            }
        }

        private void Window2_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var otherView = PlotModels.Where(p => p.PlotView != sender);
            var currentView = (sender as PlotTrendView);
            var currentPosition = e.GetPosition(currentView).ToScreenPoint();
            var result = currentView.Model.Series[0].GetNearestPoint(currentPosition, false);
            foreach (var plotModel in otherView)
            {
                var point = (plotModel.Series[0] as ContinueSeries)?.Points.FirstOrDefault(s =>
                    Math.Abs(s.X - result.DataPoint.X) < Double.Epsilon);
                var view = plotModel.PlotView as PlotTrendView;
                if (point != null)
                {
                    var sp = plotModel.DefaultXAxis.Transform(((DataPoint) point).X, ((DataPoint) point).Y,
                        plotModel.DefaultYAxis);
                    var tracker = new TrackerHitResult()
                    {
                        DataPoint = (DataPoint) point, PlotModel = plotModel, Series = plotModel.Series[0],
                        Position = sp,
                        Text =
                            $"\nX:{plotModel.DefaultXAxis.GetValue(((DataPoint) point).X)}\nY:{plotModel.DefaultYAxis.GetValue(((DataPoint) point).Y)}"
                    };
                    view?.ShowTracker(tracker);
                }
            }
        }

        private void FrameworkElement_OnLoaded2(object sender, RoutedEventArgs e)
        {
            ItemsControl itemsControl = (ItemsControl) e.OriginalSource;
            //double titleHeight = 0;
            //if (itemsControl.Items.Count > 0)
            //{
            //    titleHeight = ((PlotModel) itemsControl.Items[0]).TitleArea.Height;
            //}
            var itemHeight = itemsControl.ActualHeight / itemsControl.Items.Count;
            foreach (PlotModel item in itemsControl.Items)
            {
                //(item.PlotView as PlotTrendView).Height = itemHeight;
                int index = itemsControl.Items.IndexOf(item);
                //if (index == 0) (item.PlotView as PlotTrendView).Height = itemHeight+37;
                //else if(index == itemsControl.Items.Count-1) (item.PlotView as PlotTrendView).Height = itemHeight + 9;
                //else
                //if(index==0) (item.PlotView as PlotTrendView).Height = itemHeight;
                //else
                (item.PlotView as PlotTrendView).Height = itemHeight;
                if (index > 0)
                {
                    //var tt = (item.PlotView as PlotTrendView).Margin;
                    //(item.PlotView as PlotTrendView).Margin=new Thickness(0, -10,0,0);
                }
            }
        }
    }
}
