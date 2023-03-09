using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using ICSStudio.SimpleServices.Common;
using OxyPlot;
using OxyPlot.Wpf;
using OxyPlot.Axes;
using TimeSpanAxis = OxyPlot.Axes.TimeSpanAxis;

namespace ICSStudio.TrendTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        private PlotModel plotModel;

        public PlotModel PlotModel
        {
            get { return this.plotModel; }
            set
            {
                this.plotModel = value;
                this.RaisePropertyChanged("PlotModel");
            }

        }

        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            //CompositionTarget.Rendering += this.CompositionTarget_Rendering;
            this.PlotModel = this.CreatePlot();
            PlotModel.TitlePadding = 10;
            PlotModel.IsShowDelta = true;
            PlotModel.OnMouseWheel += OnMouseWheel;
            plotModel.PlotAreaBackground = OxyColor.FromRgb(0, 0, 0);
            this.series = new ContinueSeries();
            series.Color= OxyColor.FromRgb(255,123,13);
            PlotModel.Series.Add(series);
            //series.LeftAxes.IsAxisVisible = false;
            series.LeftAxes.Maximum=2;
            series.LeftAxes.Minimum = 0;
            series.StrokeThickness = 3;
            plotModel.Axes.Add(series.LeftAxes);
            series.LeftAxes.IsAxisVisible = true;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            timer.Tick += Timer_Tick;
            timer.Start();
            isStop = true;
            //Task.Run(() =>
            //{
            //    while (true)
            //    {
            //        //最大存储2500w多数据
            //        l.Push(DateTime.Now.ToString());
            //    }
            //});

            
        }

        //private Stack<string> l=new Stack<string>();

        private void Timer_Tick(object sender, EventArgs e)
        {
            DataPoint dataPoint = new DataPoint(this.x, 2);
            this.x += 1;
            this.series.AddPoint(dataPoint);
            //this.plotModel.Update();
            this.Update();
        }

        private void Update()
        {
            this.PlotView.InvalidatePlot(true);
        }

        public ObservableCollection<DataPoint> Data { get; } = new ObservableCollection<DataPoint>();

        private ContinueSeries series;
        //private double lastUpdateTime;

        //停止开启update
        private bool isStop = true;

        private double x = 1;

        private PlotModel CreatePlot()
        {
            //double x = 0;
            var trend = new Trend(Controller.GetInstance());
            trend.TimeSpan=new TimeSpan(0,0,0,4);
            trend.StartTime=DateTime.Now;
            var pm = new PlotModel(trend) {Title = "Plot updated: " + DateTime.Now};
            

            pm.Visibility = Visibility.Hidden;
            var axesX = new TimeSpanAxis();
            axesX.DisplayMillisecond = true;
            axesX.Position = AxisPosition.Bottom;
            axesX.TickStyle = TickStyle.None;
            //int majorLines = 30;
            //axesX.MajorStep = (double)4 / (majorLines );
            ////axesX.Angle = 90;
            axesX.MajorGridlineStyle = LineStyle.Automatic;
            axesX.MajorGridlineColor = ConvertOxyColor(Colors.Red);
            axesX.MinorGridlineStyle = LineStyle.Dot;
            //int minorLines = 10;
            //axesX.MinorStep = axesX.MajorStep / (minorLines + 1);
            axesX.MajorGridLines = 1;
            axesX.MinorGridLines = 0;
            pm.Axes.Add(axesX);
            pm.Axes.CollectionChanged += Axes_CollectionChanged;
            //var f1 = new FunctionSeries(Math.Sin, x, x + 1, 0.2);
            //pm.Axes.Add(f1.LeftAxes);
            ////f1.BrokenLineColor=ConvertOxyColor(Colors.Red);

            ////线段颜色
            //f1.Color = ConvertOxyColor(Colors.Red);
            ////线段样式
            //f1.LineStyle = LineStyle.Dash;
            ////线段粗细
            //f1.StrokeThickness = 1;

            //var f2 = new FunctionSeries(Math.Exp, x, x + 1, 0.2);
            //pm.Axes.Add(f2.LeftAxes);
            ////f1.BrokenLineColor=ConvertOxyColor(Colors.Red);

            ////线段颜色
            //f2.Color = ConvertOxyColor(Colors.Yellow);
            ////线段样式
            //f2.LineStyle = LineStyle.Dash;
            ////线段粗细
            //f2.StrokeThickness = 1;
            return pm;
        }

        private void Axes_CollectionChanged(object sender, ElementCollectionChangedEventArgs<OxyPlot.Axes.Axis> e)
        {
            foreach (var item in e.AddedItems)
            {
                if (!item.IsHorizontal())
                {
                    //item.ScaleAsPercentage = false;
                    //item.Maximum = 100;
                }
            }
        }

        private void OnMouseWheel(object sender, EventArgs e)
        {
            isStop = true;
            timer.Stop();
        }

        private OxyColor ConvertOxyColor(Color color)
        {
            return OxyColor.Parse(color.ToString());
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            this.isStop = !this.isStop;
            if (this.isStop)
            {
                this.timer.Stop();
            }
            else
            {
                this.series.Recover();
                this.timer.Start();
            }
        }

        private void ButtonBase_OnClick2(object sender, RoutedEventArgs e)
        {
            if (!this.series.CanFront())
            {
                MessageBox.Show("can not");
                return;
            }

            this.timer.Stop();
            this.isStop = true;
            this.series.Front(1);
            this.Update();
        }

        private void ButtonBase_OnClick3(object sender, RoutedEventArgs e)
        {
            if (!this.series.CanBack())
            {
                MessageBox.Show("can not");
                return;
            }

            this.timer.Stop();
            this.isStop = true;
            this.series.Back(1);
            this.Update();
        }

        //显示/隐藏X
        private void ButtonBase_OnClick4(object sender, RoutedEventArgs e)
        {
            //this.plotModel.DefaultXAxis.IsAxisVisible = !this.plotModel.DefaultXAxis.IsAxisVisible;
            plotModel.DisplayX = !plotModel.DisplayX;
            this.Update();
        }

        //显示/隐藏Y
        private void ButtonBase_OnClick5(object sender, RoutedEventArgs e)
        {
            //this.plotModel.DefaultYAxis.IsAxisVisible = !this.plotModel.DefaultYAxis.IsAxisVisible;
            plotModel.DisplayY = !plotModel.DisplayY;
            this.Update();
        }

        //还原zoom
        private void ButtonBase_OnClick6(object sender, RoutedEventArgs e)
        {
            PlotView.ActualModel.ResetAllAxes();
            PlotView.InvalidatePlot(false);
            timer.Start();
            isStop = false;
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            timer.Start();
            isStop = false;
        }

        //打印
        public void Print()
        {
            XpsExporter.Print(PlotView.Model, PlotView.ActualWidth, PlotView.ActualHeight);
        }

        private void ButtonBase_OnClick7(object sender, RoutedEventArgs e)
        {
            Print();
        }

        //显示/隐藏 标题
        private void ButtonBase_OnClick8(object sender, RoutedEventArgs e)
        {
            PlotModel.ShowTitle = !PlotModel.ShowTitle;
            PlotView.InvalidatePlot(false);
        }

        private void ButtonBase_OnClick9(object sender, RoutedEventArgs e)
        {
            if (PlotModel.DefaultYAxis.StringFormat == "N2")

                PlotModel.DefaultYAxis.StringFormat = null;
            else
            {

                PlotModel.DefaultYAxis.StringFormat = "N2";
            }

            PlotView.InvalidatePlot(false);
        }

        private void ButtonBase_OnClick10(object sender, RoutedEventArgs e)
        {
            var series = (ContinueSeries) plotModel.Series[0];
            series.LineStyle = series.LineStyle == LineStyle.Automatic ? LineStyle.Dash : LineStyle.Automatic;
            PlotView.InvalidatePlot(false);
        }

        private void ButtonBase_OnClick11(object sender, RoutedEventArgs e)
        {
            PlotModel.PlotView?.InvalidatePlot(false);
        }

        private void ButtonBase_OnClick12(object sender, RoutedEventArgs e)
        {
            series.LeftAxes.IsAxisVisible = !series.LeftAxes.IsAxisVisible;
            plotModel.PlotView?.InvalidatePlot(false);
        }
    }
}
