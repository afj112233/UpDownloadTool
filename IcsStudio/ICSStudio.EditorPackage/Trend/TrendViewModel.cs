using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.StxEditor.ViewModel.CodeSnippets;
using ICSStudio.UIInterfaces.Dialog;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NLog;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using Button = System.Windows.Controls.Button;
using IController = ICSStudio.Interfaces.Common.IController;
using MessageBox = System.Windows.MessageBox;
using TagSeries = OxyPlot.Axes.TagSeries;
using TimeSpanAxis = OxyPlot.Axes.TimeSpanAxis;
using Task = System.Threading.Tasks.Task;
using UserControl = System.Windows.Controls.UserControl;

namespace ICSStudio.EditorPackage.Trend
{
    public partial class TrendViewModel : ViewModelBase, IEditorPane
    {
        internal static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly TrendObject _trend;
        private PlotModel _plotModel;
        private bool _canExecuteRunCommand = true;
        //private readonly Timer _timer;
        //private double x = 0;
        private bool _showValueBarChecked;
        private bool _scheduleCommand4Checked;
        private string _samplePeriod;
        private bool _isZoom = false;
        private SolidColorBrush _textColorBrush;
        private Visibility _liveDataVisibility;
        private Visibility _scrollingToolbar;
        private string _backwardTooltip1;
        private string _backwardTooltip2;
        private string _forwardTooltip1;
        private string _forwardTooltip2;
        private Visibility _minMaxDataCollectionVisibility;
        private Visibility _displayIsolatedTrendVisibility = Visibility.Collapsed;
        private Visibility _displayNormalTrendVisibility;
        private string _logStatus;
        private string _logStatusValue;
        private TrendServer _trendDataCenter;
        private string _period;
        private string _ms;

        public TrendViewModel(UserControl control, ITrend trend,bool isRunImmediately = false)
        {
            Control = control;
            control.DataContext = this;
            _trend = (TrendObject)trend;
            Controller = SimpleServices.Common.Controller.GetInstance();
            if (_trend is TrendLog)
            {
                ((TrendLog) _trend).IsScrolling = false;
            }
            else
            {
                ((SimpleServices.Common.Trend)_trend).IsScrolling = false;
                ((SimpleServices.Common.Trend)_trend).RunTime=DateTime.Now;
                ((SimpleServices.Common.Trend)_trend).StartTime = DateTime.Now;
            }
            Caption = LanguageManager.GetInstance().ConvertSpecifier("Trace") +$" - {_trend.Name}{((trend is TrendLog) ? ".csv" : "")}";
            if (trend is TrendLog)
            {
                LogButtonVisibility = Visibility.Hidden;
                _canExecuteSaveLogCommand = true;
            }
            else
            {
                LogButtonVisibility = Visibility.Visible;
            }
            
            DoubleClickCommand = new RelayCommand<MouseButtonEventArgs>(ExecuteDoubleClickCommand);

            RunCommand = new RelayCommand(ExecuteRunCommand, CanExecuteRunCommand);
            StopCommand = new RelayCommand(ExecuteStopCommand, CanExecuteStopCommand);
            ErrorCommand = new RelayCommand(ExecuteErrorCommand, CanExecuteErrorCommand);
            LogButtonCommand = new RelayCommand<Button>(ExecuteLogButtonCommand);

            ScheduleCommand1 = new RelayCommand(ExecuteScheduleCommand1);
            ScheduleCommand2 = new RelayCommand(ExecuteScheduleCommand2);
            ScheduleCommand3 = new RelayCommand(ExecuteScheduleCommand3);
            ScheduleCommand4 = new RelayCommand(ExecuteScheduleCommand4);
            ScheduleCommand5 = new RelayCommand(ExecuteScheduleCommand5);
            ScheduleCommand6 = new RelayCommand(ExecuteScheduleCommand6);
            ScheduleCommand7 = new RelayCommand(ExecuteScheduleCommand7);

            PrintCommand = new RelayCommand(ExecutePrintCommand);
            ChartPropertiesCommand = new RelayCommand(ExecuteChartPropertiesCommand);
            ScrollCommand = new RelayCommand(ExecuteScrollCommand);
            ResetCommand = new RelayCommand(ExecuteResetCommand,CanExecuteResetCommand);
            ShowValueBarCommand = new RelayCommand(ExecuteShowValueBarCommand);
            ValueCommand = new RelayCommand(ExecuteValueCommand);
            DeltaCommand = new RelayCommand(ExecuteDeltaCommand);
            ExportTrendServerDataCommand = new RelayCommand(ExecuteExportTrendServerDataCommand);

            MaxMinGridCommand = new RelayCommand<Grid>(ExecuteMaxMinGridCommand);

            SaveLogCommand = new RelayCommand(ExecuteSaveLogCommand, CanExecuteSaveLogCommand);
            DeleteCommand = new RelayCommand(ExecuteDeleteCommand, CanExecuteSaveLogCommand);
            DoubleClickValueCommand=new RelayCommand<MouseButtonEventArgs>(ExecuteDoubleClickValueCommand);
            ColorDoubleClickCommand=new RelayCommand<MouseButtonEventArgs>(ExecuteColorDoubleClickCommand);

            var color = (Color)ColorConverter.ConvertFromString($"#{FormatOp.RemoveFormat(_trend.TextColor)}");
            TextColorBrush = new SolidColorBrush(color);
            ScrollingToolbar = trend.DisplayScrollingMechanism ? Visibility.Visible : Visibility.Collapsed;

            if (Trend.Position == Position.Left)
            {
                ClockVisibility = Visibility.Visible;
                BottomVisibility = Visibility.Collapsed;
                LiveDataVisibility = _trend.DisplayPenValue ? Visibility.Visible : Visibility.Collapsed;
                MinMaxDataCollectionVisibility = _trend.DisplayLineLegend ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                MinMaxDataCollectionVisibility = Visibility.Collapsed;
                LiveDataVisibility = Visibility.Collapsed;
                ClockVisibility = Visibility.Collapsed;
                BottomVisibility = Visibility.Visible;
            }

            BottomControlHeight = (_trend.MaxViewable +1)* 20;
            IsRun = trend.IsScrolling;
            ScheduleCommand4Checked = !IsRun;
            ShowValueBarChecked = true;
            _period = LanguageManager.GetInstance().ConvertSpecifier("Period");
            _ms = LanguageManager.GetInstance().ConvertSpecifier("Ms");
            _unAbleToSetTheTrance = LanguageManager.GetInstance().ConvertSpecifier("UnAbleToSetTheTrance");
            SamplePeriod = _period + $" {_trend.SamplePeriod} " + _ms;
            LogStatusValue = "Logging Stopped";
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
            PropertyChangedEventManager.AddHandler(_trend, _trend_PropertyChanged, "");
            CollectionChangedEventManager.AddHandler((trend as TrendObject), _trend_CollectionChanged);
            Controller controller = _trend.ParentController as Controller;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }

            CreateUnIsolated();
            CreateIsolated();

            SetIsolated();
            SetTooltips();
            SyncIsolatedAndUnIsolated();
            if (_trend.ChartStyle == ChartStyle.Standard)
            {
                TurnToStandard();
            }
            else
            {
                TurnToPenAxis();
            }
            if (trend is TrendLog)
            {
                //StartTimer();
                IsRun = true;
            }

            if (isRunImmediately)
            {
                Caption = LanguageManager.GetInstance().ConvertSpecifier("Trace") + $" - <{_trend.Name}>{((_trend is TrendLog) ? ".csv" : "")}";
                ExecuteRunCommand();
            }

        }

        private void ExecuteLogButtonCommand(System.Windows.Controls.Button obj)
        {

            var menu = obj.ContextMenu;
            if (menu != null)
            {
                menu.PlacementTarget = obj;
                menu.IsOpen = true;
            }
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            _period = LanguageManager.GetInstance().ConvertSpecifier("Period");
            _ms = LanguageManager.GetInstance().ConvertSpecifier("Ms");
            LogStatus = LanguageManager.GetInstance().ConvertSpecifier(LogStatusValue);
            SamplePeriod = _period + $" {_trend.SamplePeriod} " + _ms;
            _unAbleToSetTheTrance = LanguageManager.GetInstance().ConvertSpecifier("UnAbleToSetTheTrance");
            Caption = LanguageManager.GetInstance().ConvertSpecifier("Trace") + $" - {_trend.Name}{((_trend is TrendLog) ? ".csv" : "")}";
        }

        public double BottomControlHeight
        {
            set
            {
                Set(ref _bottomControlHeight , value);
            }
            get
            {
                var count = Trend.Pens.Count(p => p.Visible);
                if (Trend.MaxViewable > count)
                {
                    return (count + 1) * 20;
                }
                return _bottomControlHeight;
            }
        }

        private void ChangePosition()
        {
            if (Trend.Position == Position.Left)
            {
                LiveDataVisibility = _trend.DisplayPenValue ? Visibility.Visible : Visibility.Collapsed;
                MinMaxDataCollectionVisibility = _trend.DisplayLineLegend ? Visibility.Visible : Visibility.Collapsed;
                ClockVisibility = Visibility.Visible;
                BottomVisibility = Visibility.Collapsed;
            }
            else
            {
                MinMaxDataCollectionVisibility = Visibility.Collapsed;
                LiveDataVisibility = Visibility.Collapsed;
                ClockVisibility = Visibility.Collapsed;
                BottomVisibility = Visibility.Visible;
            }
            (Control as Trend)?.CalculateHeight();
        }

        public Visibility ClockVisibility
        {
            set { Set(ref _clockVisibility , value); }
            get { return _clockVisibility; }
        }

        public string Clock
        {
            set { Set(ref _clock , value); }
            get
            {
                if (Trend.Position == Position.Bottom)
                {
                    if (string.IsNullOrEmpty(_clock))
                    {
                        return "Value";
                    }

                    return _clock;
                }
                return _clock;

            }
        }

        public string ClockSpan
        {
            set { Set(ref _clockSpan , value); }
            get { return _clockSpan; }
        }

        public ITrend Trend => _trend;
        
        private DateTime StartTime => _trend is TrendLog ? _trend.StartTime : _trend.RunTime;

        public string LogStatusValue
        {
            set
            {
                _logStatusValue = value;
                LogStatus = LanguageManager.GetInstance().ConvertSpecifier(_logStatusValue);
            }
            get
            {
                return _logStatusValue;
            }
        }

        public string LogStatus
        {
            set
            {
                Set(ref _logStatus, value);
            }
            get { return _logStatus; }
        }


        public Visibility LogButtonVisibility { set; get; }

        public Visibility DisplayIsolatedTrendVisibility
        {
            set
            {
                Set(ref _displayIsolatedTrendVisibility, value);
                DisplayNormalTrendVisibility = _displayIsolatedTrendVisibility == Visibility.Visible
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
            get { return _displayIsolatedTrendVisibility; }
        }

        public Visibility DisplayNormalTrendVisibility
        {
            set { Set(ref _displayNormalTrendVisibility, value); }
            get { return _displayNormalTrendVisibility; }
        }

        public string BackwardTooltip1
        {
            set { Set(ref _backwardTooltip1, value); }
            get { return _backwardTooltip1; }
        }

        public string BackwardTooltip2
        {
            set { Set(ref _backwardTooltip2, value); }
            get { return _backwardTooltip2; }
        }

        public string ForwardTooltip1
        {
            set { Set(ref _forwardTooltip1, value); }
            get { return _forwardTooltip1; }
        }

        public string ForwardTooltip2
        {
            set { Set(ref _forwardTooltip2, value); }
            get { return _forwardTooltip2; }
        }

        public Visibility MinMaxDataCollectionVisibility
        {
            set { Set(ref _minMaxDataCollectionVisibility, value); }
            get { return _minMaxDataCollectionVisibility; }
        }

        public Visibility ScrollingToolbar
        {
            set { Set(ref _scrollingToolbar, value); }
            get { return _scrollingToolbar; }
        }

        public SolidColorBrush TextColorBrush
        {
            set { Set(ref _textColorBrush, value); }
            get { return _textColorBrush; }
        }

        public override void Cleanup()
        {
            var trendObject = (_trend as TrendObject);
            _trendDataCenter?.Cleanup();
            //_timer.Stop();
            //_timer.Elapsed -= Timer_Tick;
            _obtainPointThread?.Abort();
            PropertyChangedEventManager.RemoveHandler(_trend, _trend_PropertyChanged, "");
            CollectionChangedEventManager.RemoveHandler(trendObject, _trend_CollectionChanged);
            foreach (var series in PlotModel.Series)
            {
                (series as TagSeries)?.Clean(!_canExecuteRunCommand, true);
            }

            foreach (var plotModel in PlotModels)
            {
                (plotModel.Series[0] as TagSeries)?.Clean(!_canExecuteRunCommand, true);
            }

            trendObject.IsStop = true;
        }


        public ObservableCollection<MaxMinData> MinMaxDataCollection { get; } = new ObservableCollection<MaxMinData>();
        
        public string SamplePeriod
        {
            set { Set(ref _samplePeriod, value); }
            get { return _samplePeriod; }
        }

        public Visibility BottomVisibility
        {
            set { Set(ref _bottomVisibility , value); }
            get { return _bottomVisibility; }
        }

        public Visibility LiveDataVisibility
        {
            set { Set(ref _liveDataVisibility, value); }
            get { return _liveDataVisibility; }
        }

        public IController Controller { get; }

        public PlotModel PlotModel
        {
            get { return _plotModel; }
            set { Set(ref _plotModel, value); }
        }

        private bool _isResetTrend;
        public void ResetIsolatedTrend()
        {
            _isResetTrend = true;
            SetBottomHeaderVisibility();

            var visibleCollection = PlotModels.Where(p => p.Visibility == Visibility.Visible)
                .ToList();
            foreach (var plotModel in PlotModels)
            {
                if (plotModel.IsTmpVisibility)
                {
                    plotModel.ShowTitle = true;
                    plotModel.DisplayX = true;
                    return;
                }
                if (plotModel.Visibility != Visibility.Visible) continue;
                var index = visibleCollection.IndexOf(plotModel);
                Debug.Assert(index != -1);
                if (index == 0)
                {
                    plotModel.DisplayX = visibleCollection.Count == 1;
                    plotModel.ShowTitle = true;
                }
                else if (index == visibleCollection.Count - 1)
                {
                    plotModel.ShowTitle = false;
                    plotModel.DisplayX = true;
                }
                else
                {
                    plotModel.ShowTitle = false;
                    plotModel.DisplayX = false;
                }
            }
            _isResetTrend = false;
        }

        public ObservableCollection<PlotModel> PlotModels { get; } = new ObservableCollection<PlotModel>();

        private double _stopX;
        public bool IsRun
        {
            set
            {
                _trend.IsScrolling = value;
                RaisePropertyChanged("ScheduleCommand4");
                RaisePropertyChanged("ScrollChecked");
            }
            get { return _trend.IsScrolling; }
        }

        #region Command

        public RelayCommand ExportTrendServerDataCommand { get; }

        //private string _testLog = "";

        private void ExecuteExportTrendServerDataCommand()
        {
            var saveDlg = new System.Windows.Forms.SaveFileDialog()
            {
                Title = @"Save file",
                Filter = @"csv文件(*.csv)|*.csv"
            };

            try
            {
                if (saveDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    using (var sw = new StreamWriter(saveDlg.FileName, false))
                    {
                        //sw.WriteLine($"{_testLog}");

                        for (int i = 0; i < ((TagSeries)PlotModels[0].Series[0]).Points.Count; i++)
                        {
                            var data = "";
                            foreach (var plotModel in PlotModels)
                            {
                                var series = (plotModel.Series[0] as TagSeries);
                                if (series != null)
                                {
                                    var ss = series.Points[i];
                                    data +=
                                        $"********{series.Pen.Name}(X:{(PlotModels[0].DefaultXAxis as TimeSpanAxis)?.GetTime(ss.X):hh:mm:ss.fff}----Y:{ss.Y})";
                                }
                            }

                            sw.WriteLine(data);
                        }

                        sw.Close();
                    }
                }

            }
            catch (Exception)
            {
                //_testLog += $"export error:{e.Message}";
            }

            //ExportTrendServerData();
        }

        //private void ExportTrendServerData()
        //{
        //    var saveDlg = new System.Windows.Forms.SaveFileDialog()
        //    {
        //        Title = @"Save file",
        //        Filter = @"csv文件(*.csv)|*.csv"
        //    };

        //    if (saveDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //    {
        //        try
        //        {
        //            using (var sw = new StreamWriter(saveDlg.FileName, false))
        //            {
        //                lock (_trendDataCenter.DataObserver)
        //                {
        //                    foreach (var ss in _trendDataCenter.DataObserver)
        //                    {
        //                        sw.WriteLine(ss);
        //                    }
        //                }

        //                sw.Close();
        //            }

        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine(e);
        //        }
        //    }
        //}

        public RelayCommand<MouseButtonEventArgs> DoubleClickCommand { get; }

        private void ExecuteDoubleClickCommand(MouseButtonEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (e.ClickCount == 2 && e.ChangedButton == MouseButton.Left)
            {
                var uiShell = (IVsUIShell)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsUIShell));
                var service = (ICreateDialogService)Microsoft.VisualStudio.Shell.Package.GetGlobalService(
                    typeof(SCreateDialogService));
                var window = service.CreateRSTrendXProperties(_trend, 3);
                window.ShowDialog(uiShell);
            }
        }

        public RelayCommand SaveLogCommand { get; }

        private void ExecuteSaveLogCommand()
        {
            var saveDlg = new System.Windows.Forms.SaveFileDialog()
            {
                Title = @"Save file",
                Filter = @"csv文件(*.csv)|*.csv",
                FileName = $"{_trend.Name}"
            };

            if (saveDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    using (var sw = new StreamWriter(saveDlg.FileName, false))
                    {
                        sw.WriteLine(
                            $"Controller Name:,{(_trend is SimpleServices.Common.Trend ? _trend.ParentController.Name : (_trend as TrendLog)?.ParentControllerName)}");
                        sw.WriteLine($"Trend Name:,{_trend.Name}");
                        sw.WriteLine($"Trend Tags:,{_trend.Pens.Count()}");
                        sw.WriteLine($"Sample Period:,{_trend.SamplePeriod} ms");
                        sw.WriteLine($"Description:{_trend.Description}");
                        sw.WriteLine("Start Trigger:,No Trigger");
                        sw.WriteLine();
                        sw.WriteLine("Stop Trigger:,No Trigger");
                        sw.WriteLine();
                        
                        DateTime? lastDateTime = DateTime.Parse(_dataCenter.Data[_dataCenter.Data.Count-1][0]);
                        DateTime? startTime = DateTime.Parse(_dataCenter.Data[0][0]);
                       
                        sw.WriteLine($"Start Time:,{startTime:yyyy/MM/dd},{startTime:HH:mm:ss;fff}");
                        var trendLog = _trend as TrendLog;
                        sw.WriteLine(trendLog != null
                            ? $"Stop Time:,{trendLog.StopTime}"
                            : $"Stop Time:,{lastDateTime:yyyy/MM/dd},{lastDateTime:HH:mm:ss;fff}");

                        sw.WriteLine("Pre-Samples:,0");
                        sw.WriteLine("Post-Samples:,0");
                        var info = "Header:,Date,Time";
                        var pens = new List<string>();
                        foreach (var pen in _trend.Pens)
                        {
                            info = info + $",{(pen.Name.StartsWith("\\") ? "Program:" : "")}{pen.Name}";
                            pens.Add(pen.Name);
                        }

                        sw.WriteLine(info);


                        for (int i = 0; i < _dataCenter.Data.Count; i++)
                        {
                            DateTime? dateInfo = null;
                            var dataInfo = "";
                            var data = _dataCenter.Data[i];
                            dateInfo =DateTime.Parse(data[0]);
                            dataInfo = string.Join(",", data.ToArray(), 1, data.Count - 1);
                            //foreach (var pen in pens)
                            //{
                            //    var data = dataMap[pen];
                            //    if (dateInfo == null && data.Count > i)
                            //    {
                            //        dateInfo = GetTime(data[i].X);
                            //    }

                            //    dataInfo = dataInfo + "," + (data.Count > i ? data[i].Y.ToString("g9") : "NaN");
                            //}

                            Debug.Assert(dateInfo != null, i.ToString());
                            sw.WriteLine($"Data,{dateInfo:yyyy/MM/dd},{dateInfo:HH:mm:ss;fff},{dataInfo}");
                        }

                        sw.Close();
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    MessageBox.Show("The process cannot access the file because it is being used by another process.",
                        "ICS Studio", MessageBoxButton.OK);
                }
            }
        }

        private bool _canExecuteSaveLogCommand;

        private bool CanExecuteSaveLogCommand()
        {
            return _canExecuteSaveLogCommand;
        }

        public RelayCommand DeleteCommand { get; }

        private void ExecuteDeleteCommand()
        {
            var trend = _trend as TrendLog;
            if (trend != null)
            {
                try
                {
                    File.Delete(trend.FilePath);
                }
                catch (Exception)
                {
                    MessageBox.Show("The process cannot delete the file because it is being used by another process.",
                        "ICS Studio", MessageBoxButton.OK);
                    return;
                }
            }

            CleanDataNotStart();
            _canExecuteSaveLogCommand = false;
            SaveLogCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
        }

        public RelayCommand<MouseButtonEventArgs> DoubleClickValueCommand { get; }

        private void ExecuteDoubleClickValueCommand(MouseButtonEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                var uiShell = (IVsUIShell)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsUIShell));
                var service = (ICreateDialogService)Microsoft.VisualStudio.Shell.Package.GetGlobalService(
                    typeof(SCreateDialogService));
                var window = service.CreateRSTrendXProperties(Trend, 1);
                window.ShowDialog(uiShell);
            }
        }

        public RelayCommand<MouseButtonEventArgs> ColorDoubleClickCommand { get; }

        private void ExecuteColorDoubleClickCommand(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                var sender = e.Source;
                var maxMinData = (sender as Grid)?.DataContext as MaxMinData;
                if (maxMinData != null)
                {
                    var colorDialog = new ColorDialog();
                    if (colorDialog.ShowDialog() == DialogResult.OK)
                    {
                        var drawingColor = colorDialog.Color;
                        var mediaColor = Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B);
                        foreach (var item in _trend.Pens)
                        {
                            if (maxMinData.Name.Equals(item.Name))
                            {
                                item.Color = $"16{mediaColor.ToString()}";
                                break;
                            }
                        }
                    }
                }
            }
        }

        private DateTime? GetTime(double x)
        {
            if (_trend.Isolated)
            {
                if (PlotModels.Count > 0)
                {
                    var axisX = PlotModels[0].Axes.FirstOrDefault(a => a is TimeSpanAxis) as TimeSpanAxis;
                    return axisX?.GetTime(x);
                }

                Debug.Assert(false);
                return null;
            }
            else
            {
                var axisX = PlotModels[0].Axes.FirstOrDefault(a => a is TimeSpanAxis) as TimeSpanAxis;
                return axisX?.GetTime(x);
            }
        }

        public RelayCommand<Grid> MaxMinGridCommand { get; }

        private void ExecuteMaxMinGridCommand(Grid grid)
        {
            var data = grid.DataContext;
            if (_trend.Isolated)
            {
                foreach (var plotModel in PlotModels)
                {
                    var series = (TagSeries) plotModel.Series[0];
                    if (series.MaxMinData == data)
                    {
                        series.MaxMinData.SelectedStateVisibility = Visibility.Visible;
                        _lastSelectedSpecial = series.Name;
                    }
                    else
                    {
                        series.MaxMinData.SelectedStateVisibility = Visibility.Collapsed;
                    }
                }
            }
            else
            {
                foreach (TagSeries series in PlotModel.Series)
                {
                    //var series = PlotModel.Series.FirstOrDefault(s => (s as TagSeries)?.MaxMinData == data);
                    if (series.MaxMinData ==data)
                    {
                        series.MaxMinData.SelectedStateVisibility = Visibility.Visible;
                        foreach (var ax in PlotModel.Axes)
                        {
                            if (ax.IsHorizontal()) continue;
                            if(series.LeftAxes==ax)
                            {
                                ax.IsAxisVisible = true;
                                _lastSelectedSpecial = series.Name;
                            }
                            else
                            {
                                ax.IsAxisVisible = false;
                            }
                        }
                    }
                    else
                    {
                        series.MaxMinData.SelectedStateVisibility = Visibility.Collapsed;
                    }
                }


                PlotModel.PlotView.InvalidatePlot(false);
            }
        }

        public RelayCommand RunCommand { get; }

        private void ExecuteRunCommand()
        {
            //清空plotview数据并开始
#pragma warning disable VSTHRD110 // 观察异步调用的结果
            Task.Run(async () =>
#pragma warning restore VSTHRD110 // 观察异步调用的结果
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                lock (_commandObject)
                {
                    var mes = VerifyTag();
                    if (!string.IsNullOrEmpty(mes))
                    {
                        MessageBox.Show(mes, "ICS Studio", MessageBoxButton.OK);
                        return;
                    }

                    if (_trend is SimpleServices.Common.Trend)
                    {
                        ((SimpleServices.Common.Trend) _trend).StartTime = DateTime.Now;
                        ((SimpleServices.Common.Trend) _trend).RunTime = DateTime.Now;
                    }
                    ResetKeepMaxMin();
                    _isReady = true;
                    //x = 0;
                    _canExecuteRunCommand = false;
                    _stopX = -1;
                    CleanData(true);
                    RunCommand.RaiseCanExecuteChanged();
                    StopCommand.RaiseCanExecuteChanged();
                    _canExecuteSaveLogCommand = false;
                    SaveLogCommand.RaiseCanExecuteChanged();
                    DeleteCommand.RaiseCanExecuteChanged();
                    LogStatusValue = "Logging";
                    _trend.IsStop = false;
                }
            });
        }

        private bool _deltaCommandChecked = false;
        private bool _valueCommandChecked = true;

        private bool CanExecuteRunCommand()
        {
            if (_trend is TrendLog) return false;
            if (!_trend.Pens.Any()) return false;
            if (!Controller.IsOnline) return false;
            return _canExecuteRunCommand;
        }

        public string VerifyTag()
        {
            if (_trendDataCenter != null)
            {
                if (_trendDataCenter.HasErrorTag) return "Has error tag pen";
            }

            if (_trend is SimpleServices.Common.Trend)
            {
                foreach (var pen in _trend.Pens)
                {
                    var name = pen.Name;
                    var type = GetSpecificDataType(name);
                    if (type == null) return "Has error tag pen";
                    if (!(type.IsNumber || type.IsBool))
                    {
                        return $"{name}:This pen operand is referencing data that is not BOOL, SINT,INT,DINT, or REAL.";
                    }

                    if (type is ArrayType)
                    {
                        return
                            $"{name}:This pen operand is referencing an entire array.Only individual elements of an array may be trended.";
                    }
                    var tag = GetTag(ref name);
                    if (tag == null) return "Has error tag pen";
                }
            }

            return "";
        }

        public RelayCommand StopCommand { get; }
        private readonly object _commandObject=new object();
        private string _clock;
        private string _clockSpan;
        private string _unAbleToSetTheTrance;
        private Visibility _bottomVisibility;
        private Visibility _clockVisibility;
        private double _bottomControlHeight;

        private void ExecuteStopCommand()
        {
#pragma warning disable VSTHRD110 // 观察异步调用的结果
            Task.Run(async () =>
#pragma warning restore VSTHRD110 // 观察异步调用的结果
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                lock (_commandObject)
                {
                    IsRun = false;
                    _waitAbort=false;
                    _isRendering = 0;
                    _completeCount = 0;
                    //try
                    //{
                    //    _obtainPointThread.Abort();
                    //}
                    //catch (Exception)
                    //{
                    //    //ignore
                    //}
                    ScheduleCommand4Checked = true;
                    RaisePropertyChanged("ScrollChecked");
                    _isReady = true;
                    StopDataCenterThread();
                    _trendDataCenter?.StartNewOne();
                    _canExecuteRunCommand = true;
                    RunCommand.RaiseCanExecuteChanged();
                    StopCommand.RaiseCanExecuteChanged();
                    _canExecuteSaveLogCommand = true;
                    SaveLogCommand.RaiseCanExecuteChanged();
                    DeleteCommand.RaiseCanExecuteChanged();
                    LogStatusValue = "Logging Stopped";
                    ((TrendObject)_trend).IsStop = true;
                }
            });
        }

        private bool CanExecuteStopCommand()
        {
            if (_trend is TrendLog) return true;
            if (!_trend.Pens.Any()) return false;
            if (!Controller.IsOnline) return false;
            return !_canExecuteRunCommand;
        }

        public RelayCommand ErrorCommand { get; }
        public RelayCommand<Button> LogButtonCommand { get; }

        private void ExecuteErrorCommand()
        {

        }

        private bool CanExecuteErrorCommand()
        {
            return false;
        }

        public RelayCommand ScheduleCommand1 { get; }

        private void ExecuteScheduleCommand1()
        {
            if (PlotModel.Series.Count == 0) return;

            {
                var series = PlotModel.Series[0] as TagSeries;
                if (series != null && series.HasData())
                {
                    Stop();
                    ResetKeepMaxMin();
                    _dataCenter?.ToOldest();
                    var timeAxis = (PlotModel.Axes.FirstOrDefault(a => a is TimeSpanAxis) as TimeSpanAxis);
                    SetTitle(timeAxis, _dataCenter.DataStartTime, _dataCenter.DataEndTime);
                    Update(false);
                    return;
                }
            }


            MessageBox.Show(
                _unAbleToSetTheTrance,
                "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        public RelayCommand ScheduleCommand2 { get; }

        private void ExecuteScheduleCommand2()
        {
            //double step = _trend.TimeSpan.TotalMilliseconds / _trend.SamplePeriod;
            var time = StartTime + TimeSpan.FromSeconds(GetCurrentX() - _trend.TimeSpan.TotalSeconds);
            Forward(time);
        }

        public RelayCommand ScheduleCommand3 { get; }

        private void ExecuteScheduleCommand3()
        {
            try
            {
                //double step = _trend.TimeSpan.TotalMilliseconds / 2 / _trend.SamplePeriod;
                var time = StartTime + TimeSpan.FromSeconds(GetCurrentX() - _trend.TimeSpan.TotalSeconds/2);
                Forward(time);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void Backward(DateTime time)
        {
            if (PlotModel.Series.Count > 0)
            {
                if (_dataCenter.CanBack(time))
                {
                    Stop();
                    ResetKeepMaxMin();
                    _dataCenter.GetTimeSpanData(time);
                    var timeAxis = (PlotModel.Axes.FirstOrDefault(a => a is TimeSpanAxis) as TimeSpanAxis);
                    SetTitle(timeAxis,_dataCenter.DataStartTime,_dataCenter.DataEndTime);
                    Update(false);
                    _stopX = ((TagSeries) PlotModel.Series[0]).LastDataPoint.X;
                    return;
                }
            }

            MessageBox.Show(
                _unAbleToSetTheTrance,
                "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void Forward(DateTime time)
        {
            if (PlotModel.Series.Count > 0)
            {
                if (time>StartTime)
                {
                    Stop();
                    ResetKeepMaxMin();
                    _dataCenter.GetTimeSpanData(time);
                    var timeAxis = (PlotModel.Axes.FirstOrDefault(a => a is TimeSpanAxis) as TimeSpanAxis);
                    SetTitle(timeAxis, _dataCenter.DataStartTime, _dataCenter.DataEndTime);
                    Update(false);
                    _stopX = ((TagSeries)PlotModel.Series[0]).LastDataPoint.X;
                    return;
                }
            }

            MessageBox.Show(
                _unAbleToSetTheTrance,
                "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        public RelayCommand ScheduleCommand4 { get; }

        private void ExecuteScheduleCommand4()
        {
            if (!IsRun)
            {
                ResetKeepMaxMin();
            }
            IsRun = !IsRun;
            ScheduleCommand4Checked = !IsRun;
            RaisePropertyChanged("ScrollChecked");
        }

        private void SeriesRecover()
        {
            //var tasks = new List<Task>();

            //foreach (var plotModel in PlotModels)
            //{
            //    tasks.Add(Task.Run(() => { (plotModel.Series[0] as TagSeries)?.Recover(); }));
            //}

            //Task.WaitAll(tasks.ToArray());
            
            _dataCenter?.GetTimeSpanData(StartTime+TimeSpan.FromSeconds(double.IsNaN(_stopX)?0:_stopX));
        }

        public bool ScheduleCommand4Checked
        {
            set { Set(ref _scheduleCommand4Checked, value); }
            get { return _scheduleCommand4Checked; }
        }

        public RelayCommand ScheduleCommand5 { get; }

        private void ExecuteScheduleCommand5()
        {
            //double step = _trend.TimeSpan.TotalMilliseconds / _trend.SamplePeriod / 2;
            
            var time = StartTime + TimeSpan.FromSeconds(GetCurrentX() + _trend.TimeSpan.TotalSeconds / 2);
            Backward(time);
        }

        private double GetCurrentX()
        {
            try
            {
                //if(_trend.ChartStyle==ChartStyle.Standard)
                //{
                //    if (PlotModel.Series.Count > 0)
                //    {
                //        var series = (TagSeries) PlotModel.Series[0];
                //        return series.Points[0].X;
                //    }
                //}
                //else
                //{

                //}
                return _dataCenter.Signal;
            }
            catch (Exception)
            {
                //ignore
            }
            
            return 0;
        }

        private void Stop()
        {
            IsRun = false;
            if (_obtainPointThread?.IsAlive??false)
            {
                _obtainPointThread.Abort();
            }
            ScheduleCommand4Checked = true;
            RaisePropertyChanged("ScrollChecked");
        }

        public RelayCommand ScheduleCommand6 { get; }

        private void ExecuteScheduleCommand6()
        {
            //double step = _trend.TimeSpan.TotalMilliseconds / _trend.SamplePeriod;
            var time = StartTime + TimeSpan.FromSeconds(GetCurrentX() + _trend.TimeSpan.TotalSeconds);
            Backward(time);
        }

        public RelayCommand ScheduleCommand7 { get; }

        private void ExecuteScheduleCommand7()
        {
            if (PlotModel.Series.Count == 0) return;
            if (IsRun)
            {
                Stop();
                return;
            }

            {
                var series = PlotModel.Series[0] as TagSeries;
                if (series != null && series.HasData())
                {
                    Stop();
                    //var tasks = new List<Task>();
                    //foreach (TagSeries tagSeries in PlotModel.Series)
                    //{
                    //    tasks.Add(Task.Run(() => { tagSeries.Recover(); }));
                    //}

                    //Task.WaitAll(tasks.ToArray());
                    _dataCenter?.Recover();
                    var timeAxis = (PlotModel.Axes.FirstOrDefault(a => a is TimeSpanAxis) as TimeSpanAxis);
                    SetTitle(timeAxis, _dataCenter.DataStartTime, _dataCenter.DataEndTime);
                    Update(false);
                    return;
                }
            }


            MessageBox.Show(
                _unAbleToSetTheTrance,
                "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        public RelayCommand PrintCommand { get; }

        private void ExecutePrintCommand()
        {
            if (_trend.Isolated)
            {
                MessageBox.Show("Can print isolated trend", "ICS Studio");
                return;
            }

            XpsExporter.Print(PlotModel, ((PlotTrendView)PlotModel.PlotView).ActualWidth,
                ((PlotTrendView)PlotModel.PlotView).ActualHeight);
        }

        public RelayCommand ChartPropertiesCommand { get; }

        private void ExecuteChartPropertiesCommand()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var service = Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;
            var shell = Package.GetGlobalService(typeof(IVsUIShell)) as IVsUIShell;
            var dialog = service?.CreateRSTrendXProperties(_trend, 0);
            dialog?.ShowDialog(shell);
        }

        public RelayCommand ScrollCommand { get; }

        private void ExecuteScrollCommand()
        {
            ExecuteScheduleCommand4();
        }

        public RelayCommand ResetCommand { get; }

        private void ExecuteResetCommand()
        {
            if(!_isZoom)return;
            _isZoom = false;
            ResetCommand.RaiseCanExecuteChanged();
            if (!_trend.Isolated)
            {
                PlotModel.PlotView.ActualModel.ResetAllAxes();
                PlotModel.PlotView.InvalidatePlot(false);
            }
            else
            {
                foreach (var plotModel in PlotModels)
                {
                    plotModel.PlotView.ActualModel.ResetAllAxes();
                    plotModel.PlotView.InvalidatePlot(false);
                }
            }

            Trend.IsUpdateScale = true;
        }

        private bool CanExecuteResetCommand()
        {
            return _isZoom;
        }
        
        public RelayCommand ShowValueBarCommand { get; }

        public bool ShowValueBarChecked
        {
            set { Set(ref _showValueBarChecked, value); }
            get { return _showValueBarChecked; }
        }

        private void ExecuteShowValueBarCommand()
        {
            ShowValueBarChecked = !ShowValueBarChecked;
            PlotModel.IsShowValueBar = ShowValueBarChecked;
            foreach (var plotModel in PlotModels)
            {
                plotModel.IsShowValueBar = ShowValueBarChecked;
            }

            if (!_trend.IsScrolling)
                Update(false, false);
            else
            {
                if (_canExecuteRunCommand)
                    Update(false, false);
            }
        }

        public RelayCommand ValueCommand { get; }

        public bool ValueCommandChecked
        {
            set { Set(ref _valueCommandChecked, value); }
            get { return _valueCommandChecked; }
        }

        private void ExecuteValueCommand()
        {
            DeltaCommandChecked = false;
            ValueCommandChecked = true;
            PlotModel.IsShowDelta = false;
            foreach (var plotModel in PlotModels)
            {
                plotModel.IsShowDelta = false;
            }
        }

        public RelayCommand DeltaCommand { get; }

        public bool DeltaCommandChecked
        {
            set { Set(ref _deltaCommandChecked, value); }
            get { return _deltaCommandChecked; }
        }

        private void ExecuteDeltaCommand()
        {
            ValueCommandChecked = false;
            DeltaCommandChecked = true;
            PlotModel.IsShowDelta = true;
            foreach (var plotModel in PlotModels)
            {
                plotModel.IsShowDelta = true;
            }
        }

        #endregion

        public string Caption { get; set; }
        public UserControl Control { get; }
        public Action CloseAction { get; set; }
        public Action<string> UpdateCaptionAction { get; set; }
        
        private void OnMouseWheel(object sender, EventArgs e)
        {
            IsRun = false;
            _isZoom = true;
            ResetCommand.RaiseCanExecuteChanged();
            ScheduleCommand4Checked = true;
            RaisePropertyChanged("ScrollChecked");
        }

        private OxyColor ConvertOxyColor(Color color)
        {
            return OxyColor.Parse(color.ToString());
        }

        private bool NeedUpdate => _stopwatch.Elapsed.TotalSeconds >= 1;

        private Tag GetTag(ref string name)
        {
            var program = GetProgram(name);
            if (program != null)
                name = name.Replace($"\\{program.Name}.", "");
            var tag = ExtendOperation.LoadTag(name, Controller, program);
            if (tag == null) return null;
            //Debug.Assert(!string.IsNullOrEmpty(tag), "name:" + name);
            if (program == null)
                return (Tag)Controller.Tags[tag];
            return (Tag)program.Tags[tag];
        }

        private IDataType GetSpecificDataType(string name)
        {
            return ObtainValue.GetLoadTag(name, null, null)?.Expr.type;
        }

        private Program GetProgram(string name)
        {
            if (name.StartsWith("\\"))
            {
                if (name.IndexOf(".") > 0)
                    name = name.Substring(0, name.IndexOf("."));
                Debug.Assert(!string.IsNullOrEmpty(name), "Name:" + name);
                if (Controller.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) return null;
                return (Program)Controller.Programs[name.Replace("\\", "")];
            }

            return null;
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                RunCommand.RaiseCanExecuteChanged();
                StopCommand.RaiseCanExecuteChanged();
                if (!e.NewValue)
                {
                    _trendDataCenter.IsOnlineChange = true;
                    ExecuteStopCommand();
                }
            });
        }
    }
}
