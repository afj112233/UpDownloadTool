using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace ICSStudio.TrendDisplayTest
{
    public class MainViewModel : ViewModelBase
    {
        private readonly DispatcherTimer _timer;
        private readonly DataCreator _dataCreator;
        private readonly Stopwatch _stopwatch;

        private readonly TrendDataCenter _dataCenter;

        public MainViewModel()
        {
            _stopwatch = new Stopwatch();

            SetupModel();

            RunCommand = new RelayCommand(ExecuteRun);
            StopCommand = new RelayCommand(ExecuteStop);
            Run1Command = new RelayCommand(ExecuteRun1);

            _timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1.0)
            };
            _timer.Tick += OnTick;
            _timer.Stop();

            _dataCreator = new DataCreator();

            _dataCenter = new TrendDataCenter();
        }

        public override void Cleanup()
        {
            _dataCenter.Stop();

            _timer.Stop();
            
            base.Cleanup();
        }

        public PlotModel PlotModel0 { get; private set; }

        public PlotModel PlotModel1 { get; private set; }

        public RelayCommand RunCommand { get; }

        public RelayCommand StopCommand { get; }

        public RelayCommand Run1Command { get; }

        private void SetupModel()
        {
            PlotModel0 = CreatePlotModel();
            PlotModel1 = CreatePlotModel();
        }

        private PlotModel CreatePlotModel()
        {
            PlotModel plotModel = new PlotModel()
            {
                Background = OxyColors.White
            };
            DateTime dateTime = DateTime.Now;
            dateTime = dateTime.AddMilliseconds(-dateTime.Millisecond);
            DateTimeAxis dateTimeAxis1 = new DateTimeAxis();
            dateTimeAxis1.Position = AxisPosition.Bottom;
            dateTimeAxis1.StringFormat = "hh:mm:ss";
            dateTimeAxis1.IsAxisVisible = true;
            dateTimeAxis1.MinorTickSize = 0.0;
            dateTimeAxis1.MajorTickSize = 0.0;
            dateTimeAxis1.MajorGridlineStyle = LineStyle.Solid;
            dateTimeAxis1.MajorGridlineColor = OxyColors.Black;
            dateTimeAxis1.MajorGridlineThickness = 1.0;
            dateTimeAxis1.MajorStep = 1.15740740740741E-05;
            dateTimeAxis1.Minimum = DateTimeAxis.ToDouble(dateTime) - 2.31481481481481E-05;
            dateTimeAxis1.Maximum = DateTimeAxis.ToDouble(dateTime);
            DateTimeAxis dateTimeAxis2 = dateTimeAxis1;
            LinearAxis linearAxis1 = new LinearAxis();
            linearAxis1.Position = AxisPosition.Left;
            linearAxis1.IsAxisVisible = false;
            LinearAxis linearAxis2 = linearAxis1;
            plotModel.Axes.Add(dateTimeAxis2);
            plotModel.Axes.Add(linearAxis2);
            LineSeries lineSeries1 = new LineSeries();
            lineSeries1.Color = OxyColor.FromArgb(byte.MaxValue, 78, 154, 6);
            lineSeries1.MarkerFill = OxyColor.FromArgb(byte.MaxValue, 78, 154, 6);
            lineSeries1.MarkerStroke = OxyColors.ForestGreen;
            lineSeries1.MarkerType = MarkerType.None;
            lineSeries1.StrokeThickness = 1.0;
            lineSeries1.DataFieldX = "Time";
            lineSeries1.DataFieldY = "Value";
            LineSeries lineSeries2 = lineSeries1;
            plotModel.Series.Add(lineSeries2);
            return plotModel;
        }

        private void OnTick(object sender, EventArgs e)
        {
            //DateTime now = DateTime.Now;
            //DateTime endTime = now.AddMilliseconds(-now.Millisecond);
            //DateTime startTime = endTime.AddSeconds(-2.0);
            //List<DataPoint> data = _dataCreator.GetData(startTime, endTime);

            //LineSeries lineSeries = PlotModel0.Series[0] as LineSeries;

            //if (lineSeries != null)
            //{
            //    lineSeries.Points.Clear();
            //    lineSeries.Points.AddRange(data);
            //}

            //DateTimeAxis defaultXAxis = PlotModel0.DefaultXAxis as DateTimeAxis;

            //if (defaultXAxis != null)
            //{
            //    defaultXAxis.Minimum = DateTimeAxis.ToDouble(startTime);
            //    defaultXAxis.Maximum = DateTimeAxis.ToDouble(endTime);
            //}

            //PlotModel0.InvalidatePlot(true);

            UpdatePlotModel1();
        }

        private void UpdatePlotModel1()
        {
            DateTime now = DateTime.Now;
            DateTime endTime = now.AddMilliseconds(-now.Millisecond);
            DateTime startTime = endTime.AddSeconds(-2.0);
            List<DataPoint> data = _dataCenter.GetData(startTime, endTime);

            LineSeries lineSeries = PlotModel1.Series[0] as LineSeries;

            if (lineSeries != null)
            {
                lineSeries.Points.Clear();
                lineSeries.Points.AddRange(data);
            }

            DateTimeAxis defaultXAxis = PlotModel1.DefaultXAxis as DateTimeAxis;

            if (defaultXAxis != null)
            {
                defaultXAxis.Minimum = DateTimeAxis.ToDouble(startTime);
                defaultXAxis.Maximum = DateTimeAxis.ToDouble(endTime);
            }

            PlotModel1.InvalidatePlot(true);
        }

        private void ExecuteRun()
        {
            _dataCreator.Start();
            _timer.Start();
        }

        private void ExecuteStop()
        {
            _timer.Stop();
            _dataCreator.Stop();
        }

        private void ExecuteRun1()
        {
            _dataCenter.Start();
            _timer.Start();
        }
    }
}
