using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using ICSStudio.SimpleServices.Common;
using Newtonsoft.Json.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace ICSStudio.TrendTest
{
    /// <summary>
    /// XYPlotTrend.xaml 的交互逻辑
    /// </summary>
    public partial class XYPlotTrend : Window
    {
        private TrendLog _trendLog;
        public XYPlotTrend()
        {
            InitializeComponent();
            DataContext = this;
            //timer = new DispatcherTimer();
            //timer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            //timer.Tick += Timer_Tick; ;
            //timer.Start();
            string file = @"C:\Users\zyl\Desktop\a.csv";
            JObject config;
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var sr = new StreamReader(fs, Encoding.Default))
                {
                    config = ConvertCSVToJObject(sr);
                }
            }

            Debug.Assert(config != null);
            _trendLog = new TrendLog(config, file);

            CreatePlot();
        }
        
        private JObject ConvertCSVToJObject(StreamReader sr)
        {
            int index = 0;
            try
            {
                var config = new JObject();
                var data = new JArray();
                var pens = new JArray();
                while (!sr.EndOfStream)
                {
                    //Console.WriteLine(sr.ReadLine());
                    var line = sr.ReadLine()?.Split(',');
                    if (index == 0)
                    {
                        config["ControllerName"] = line?[1].Replace("\"", "");
                    }

                    if (index == 1)
                    {
                        config["TrendName"] = line?[1].Replace("\"", "");
                    }

                    if (index == 2)
                    {
                        config["TrendTags"] = line?[1].Replace("\"", "");
                    }

                    if (index == 3)
                    {
                        config["SamplePeriod"] = line?[1].Replace("\"", "").Replace("ms", "").Replace(" ", "");
                    }

                    if (index == 4)
                    {
                        config["Description"] = line.Length > 1 ? line[1].Replace("\"", "") : "";
                    }

                    if (index == 9)
                    {
                        config["StartTime"] = line?[1].Replace("\"", "") + "," + line?[2].Replace("\"", "");
                    }

                    if (index == 10)
                    {
                        config["StopTime"] = line?[1].Replace("\"", "") + "," + line?[2].Replace("\"", "");
                    }

                    if (index == 13)
                    {
                        for (int i = 3; i < line.Length; i++)
                        {
                            pens.Add(new JValue(line[i].Replace("\"", "").Replace("Program:", "")));
                        }
                    }

                    if (index >= 14)
                    {
                        var info = new JArray();
                        if ("Data".Equals(line[0].Replace("\"", "")))
                        {
                            info.Add($"{line[1]},{line[2]}");
                            for (int i = 3; i < line.Length; i++)
                            {
                                var value = line[i].Replace("\"", "");
                                info.Add(new JValue(value));
                            }

                            data.Add(info);
                        }
                    }

                    index++;
                }

                config["Pens"] = pens;
                config["Data"] = data;

                return config;
            }
            catch (Exception e)
            {
                throw new IOException(e.Message);
            }
        }
        
        public PlotModel PlotModel { set; get; }

        private void CreatePlot()
        {
            //double x = 0;
            PlotModel = new PlotModel { Title = "XYPlot " };
            var series = new LineSeries();
            series.Color = ConvertOxyColor(Colors.Red);
            var series2 = new LineSeries();
            series2.Color = ConvertOxyColor(Colors.Green);
            var penAxis=new LinearAxis();
            penAxis.IsAxisVisible = true;
            penAxis.Maximum = 23;
            foreach (var data in _trendLog.Data)
            {
                double x,y;
                x = double.Parse(data[1]);
                y = double.Parse(data[2]);
                series.Points.Add(new DataPoint(x,x));
                
                series2.Points.Add(new DataPoint(x,y));
            }
            //PlotModel.Series.Add(series);
            series.IsVisible = false;
            PlotModel.Series.Add(series2);
           
            PlotModel.Axes.Add(penAxis);
        }



        private OxyColor ConvertOxyColor(Color color)
        {
            return OxyColor.Parse(color.ToString());
        }
    }
}
