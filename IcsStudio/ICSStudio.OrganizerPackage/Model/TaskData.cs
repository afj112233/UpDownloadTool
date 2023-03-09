using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Windows;
using GalaSoft.MvvmLight;
using ICSStudio.SimpleServices.Common;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.OrganizerPackage.Model
{
    public class TaskData : ViewModelBase
    {
        public List<string> FindScheduledPrograms(CTask task, IController controller)
        {
            return (from i in controller.Programs where i.ParentTask == task select i.Name).ToList();
        }

        public List<string> FindUnscheduledPrograms(CTask task, IController controller)
        {
            return (from i in controller.Programs where !controller.Tasks.Contains(i.ParentTask) select i.Name).ToList();
        }

        private string _description;
        private string _scanTimesMax;
        private string _scanTimesLast;
        private string _intervalMax;
        private string _intervalMin;
        private string _taskOverlapCount;
        private List<string> _scheduledList;
        private List<string> _unscheduledList;
        private List<string> _configurationData;

        public string Description
        {
            get { return _description; }
            set { Set(ref _description, value); }
        }

        public string ScanTimesMax
        {
            get { return _scanTimesMax; }
            set { Set(ref _scanTimesMax, value); }
        }

        public string ScanTimesLast
        {
            get { return _scanTimesLast; }
            set { Set(ref _scanTimesLast, value); }
        }

        public string IntervalMax
        {
            get { return _intervalMax; }
            set { Set(ref _intervalMax, value); }
        }

        public string IntervalMin
        {
            get { return _intervalMin; }
            set { Set(ref _intervalMin, value); }
        }

        public string TaskOverlapCount
        {
            get { return _taskOverlapCount; }
            set { Set(ref _taskOverlapCount, value); }
        }

        public List<string> ScheduledList
        {
            get { return _scheduledList; }
            set { Set(ref _scheduledList, value); }
        }

        public List<string> UnscheduledList
        {
            get { return _unscheduledList; }
            set { Set(ref _unscheduledList, value); }
        }

        public List<string> ConfigurationData
        {
            get { return _configurationData; }
            set { Set(ref _configurationData, value); }
        }

        public string Type;

        public TaskData(CTask task, IController controller)
        {

            Description = task.Description;
            ScanTimesMax = task.MaxScanTime.ToString();
            ScanTimesLast = task.LastScanTime.ToString();
            IntervalMax = task.MaxIntervalTime.ToString();
            IntervalMin = task.MinIntervalTime.ToString();
            TaskOverlapCount = task.OverlapCount.ToString();
            Type = task.Type.ToString();
            ConfigurationData = new List<string> { "Type", Type };
            ScheduledList = new List<string>() { "", "", "", "" };
            UnscheduledList = new List<string>() { "", "", "", "" };
            var scheduledPrograms = FindScheduledPrograms(task, controller);
            var unScheduledPrograms = FindUnscheduledPrograms(task, controller);

            #region ConfigurationData

            switch (Type)
            {
                case "Continuous":
                    ConfigurationData.Add("Disable automatic output processing\nto reduce task overhead:");
                    ConfigurationData.Add("\n" + (task.DisableUpdateOutputs ? "Yes" : "No"));
                    ConfigurationData.Add("Watchdog:");
                    ConfigurationData.Add(task.Watchdog.ToString("F3") + "ms");
                    ConfigurationData.Add("Inhibit task:");
                    ConfigurationData.Add(task.IsInhibited ? "Yes" : "No");
                    break;
                case "Periodic":
                    ConfigurationData.Add("Watchdog:");
                    ConfigurationData.Add(task.Watchdog.ToString("F3") + "ms");
                    ConfigurationData.Add("Period:");
                    ConfigurationData.Add(task.Rate.ToString("F3") + "ms");
                    ConfigurationData.Add("Disable automatic output processing\nto reduce task overhead:");
                    ConfigurationData.Add("\n" + (task.DisableUpdateOutputs ? "Yes" : "No"));
                    ConfigurationData.Add("Priority:");
                    ConfigurationData.Add(task.Priority.ToString());
                    ConfigurationData.Add("Inhibit task:");
                    ConfigurationData.Add(task.IsInhibited ? "Yes" : "No");
                    break;
                case "Event":
                    ConfigurationData.Add("Priority:");
                    ConfigurationData.Add(task.Priority.ToString());
                    ConfigurationData.Add("Trigger:");
                    ConfigurationData.Add("<none>");
                    ConfigurationData.Add("Watchdog:");
                    ConfigurationData.Add(task.Watchdog.ToString("F3") + "ms");
                    ConfigurationData.Add("Tag:");
                    ConfigurationData.Add("<none>");
                    ConfigurationData.Add("Disable automatic output processing\nto reduce task overhead:");
                    ConfigurationData.Add("\n" + (task.DisableUpdateOutputs ? "Yes" : "No"));
                    ConfigurationData.Add("Execute task if no event occur:");
                    ConfigurationData.Add("no");
                    ConfigurationData.Add("Inhibit task:");
                    ConfigurationData.Add(task.IsInhibited ? "Yes" : "No");
                    ConfigurationData.Add("Within:");
                    ConfigurationData.Add(task.Rate.ToString("F3") + "ms");
                    break;
            }

            #endregion

            int number = 0;
            foreach (var program in scheduledPrograms)
            {
                ScheduledList[number % 4] += program + "\n";
                number++;
            }

            number = 0;
            foreach (var program in unScheduledPrograms)
            {
                UnscheduledList[number % 4] += program + "\n";
                number++;
            }
            //return testdata;
        }
    }
}
