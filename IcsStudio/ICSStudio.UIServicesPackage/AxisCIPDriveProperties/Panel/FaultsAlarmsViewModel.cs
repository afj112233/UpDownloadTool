using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Data;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.Objects;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Online;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Timer = System.Timers.Timer;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    [SuppressMessage("ReSharper", "UsePatternMatching")]
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class FaultsAlarmsViewModel : DefaultViewModel
    {
        private readonly Timer _getLogTimer;

        private bool _isFaultVisible;
        private bool _isAlarmVisible;
        private bool _isResetVisible;

        private readonly object _axisLogLock = new object();

        //500ms, refresh
        public FaultsAlarmsViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            ClearLogCommand = new RelayCommand(ExecuteClearLog, CanExecuteClearLog);

            AxisLogSource = new ObservableCollection<AxisLogItem>();
            BindingOperations.EnableCollectionSynchronization(AxisLogSource, _axisLogLock);

            _getLogTimer = new Timer(500);
            _getLogTimer.Elapsed += GetLogHandle;
            _getLogTimer.AutoReset = false;

            _isFaultVisible = true;
            _isAlarmVisible = true;
            _isResetVisible = true;
        }

        public override void Cleanup()
        {
            _getLogTimer.Stop();
            _getLogTimer.Elapsed -= GetLogHandle;

            base.Cleanup();
        }

        public RelayCommand ClearLogCommand { get; }

        public ObservableCollection<AxisLogItem> AxisLogSource { get; }

        public bool IsLogEnable => ParentViewModel.IsOnLine;

        public bool IsFaultVisible
        {
            get { return _isFaultVisible; }
            set { Set(ref _isFaultVisible, value); }
        }

        public bool IsAlarmVisible
        {
            get { return _isAlarmVisible; }
            set { Set(ref _isAlarmVisible, value); }
        }

        public bool IsResetVisible
        {
            get { return _isResetVisible; }
            set { Set(ref _isResetVisible, value); }
        }

        public override void Show()
        {
            UIVisibilityAndReadonly();
            UIRefresh();
        }

        private void UIRefresh()
        {
            if (ParentViewModel.IsOnLine)
            {
                _getLogTimer.Start();
            }
            else
            {
                _getLogTimer.Stop();
            }
        }

        private void UIVisibilityAndReadonly()
        {
            ClearLogCommand.RaiseCanExecuteChanged();

            RaisePropertyChanged("IsLogEnable");
        }

        private bool CanExecuteClearLog()
        {
            if (ParentViewModel.IsOnLine)
                return true;

            return false;
        }

        private void ExecuteClearLog()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                try
                {
                    await TaskScheduler.Default;

                    Controller controller = ParentViewModel.Controller as Controller;
                    AxisLogController axisLogController
                        = controller?.Lookup(typeof(AxisLogController)) as AxisLogController;

                    if (axisLogController == null)
                        return;

                    await axisLogController.ClearLogAsync(ParentViewModel.AxisTag);

                    var snapshot = axisLogController.GetSnapshot(ParentViewModel.AxisTag);

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    SyncLogs(snapshot);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }

        private void GetLogHandle(object sender, ElapsedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                try
                {
                    await TaskScheduler.Default;

                    Controller controller = ParentViewModel.Controller as Controller;
                    AxisLogController axisLogController
                        = controller?.Lookup(typeof(AxisLogController)) as AxisLogController;

                    if (axisLogController == null)
                        return;

                    await axisLogController.GetLogsAsync(ParentViewModel.AxisTag);

                    var snapshot = axisLogController.GetSnapshot(ParentViewModel.AxisTag);

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    SyncLogs(snapshot);

                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
                finally
                {
                    if (ParentViewModel.IsOnLine)
                    {
                        _getLogTimer.Start();
                    }
                }

            });
        }

        private void SyncLogs(AxisExceptionLogItem[] snapshot)
        {
            if (snapshot == null)
            {
                AxisLogSource.Clear();
            }
            else if (snapshot.Length == 0)
            {
                AxisLogSource.Clear();
            }
            else if (snapshot.Length > 0)
            {
                var displayList = AxisLogSource.ToList().Select(i => i.Item).ToList();

                var removeList = displayList.Except(snapshot).ToList();
                var addList = snapshot.Except(displayList).ToList();

                foreach (var item in removeList)
                {
                    var removeItem = AxisLogSource.First(i => i.Item == item);
                    AxisLogSource.Remove(removeItem);
                }

                foreach (var item in addList)
                {
                    AxisLogSource.Add(new AxisLogItem(item));
                }

                //AxisLogSource.Except(snapshot);
                //var displayList  = AxisLogSource.ToList();
                //var removeList = displayList.Except(snapshot.ToList(),);

            }

        }

    }

    internal class AxisLogItem
    {
        public AxisLogItem(AxisExceptionLogItem item)
        {
            Item = item;

            IsReset = AxisLogDescriptor.IsReset(item);
            DateTime = AxisLogDescriptor.GetDateTime(item.Timestamp);

            if ((item.Type & 0x100) == 0)
            {
                IsFault = true;

                Source = AxisLogDescriptor.GetFaultSource(item);
                Condition = AxisLogDescriptor.GetFaultCondition(item);
                SubCode = AxisLogDescriptor.GetFaultSubCode(item);
                Action = AxisLogDescriptor.GetFaultAction(item);
                EndState = AxisLogDescriptor.GetFaultEndState(item);

                ToolTip = AxisLogDescriptor.GetFaultEntryToolTip(item);

            }
            else
            {
                IsAlarm = true;

                Source = AxisLogDescriptor.GetAlarmSource(item);
                Condition = AxisLogDescriptor.GetAlarmCondition(item);
                SubCode = AxisLogDescriptor.GetAlarmSubCode(item);
                Action = AxisLogDescriptor.GetAlarmAction(item);
                EndState = string.Empty;

                ToolTip = AxisLogDescriptor.GetAlarmEntryToolTip(item);
            }

        }

        public AxisExceptionLogItem Item { get; }
        public string DateTime { get; }
        public string Source { get; }
        public string Condition { get; }
        public string SubCode { get; }
        public string Action { get; }
        public string EndState { get; }

        public bool IsFault { get; }
        public bool IsAlarm { get; }
        public bool IsReset { get; }

        public string ToolTip { get; }
    }

    internal static class AxisLogDescriptor
    {
        private static readonly Dictionary<byte, string> FaultTypeDictionary = new Dictionary<byte, string>
        {
            { 0, "Faults Cleared" },
            { 1, "Initialization Fault" },
            { 2, "Initialization Fault - Mfg" },
            { 3, "Axis Fault" },
            { 4, "Axis Fault - Mfg" },
            { 5, "Motion Fault" },
            { 6, "Module Fault" },
            { 7, "Group Fault" },
            { 8, "Configuration Fault" },
            { 9, "APR Fault" },
            { 10, "APR Fault - Mfg" },
            { 11, "Axis Safety Fault" },
            { 12, "Axis Safety Fault - Mfg" },
            { 128, "Guard Fault" }

        };

        private static readonly Dictionary<byte, string> AlarmTypeDictionary = new Dictionary<byte, string>()
        {
            { 0, "No Alarms" },
            { 1, "Start Inhibit" },
            { 2, "Start Inhibit - Mfg" },
            { 3, "Axis Alarm" },
            { 4, "Axis Alarm - Mfg" },
            { 5, "Motion Alarm" },
            { 6, "Module Alarm" },
            { 7, "Group Alarm" },
            { 8, "Remote Get Alarm" },
            { 9, "Axis Safety Alarm" },
            { 10, "Axis Safety Alarm - Mfg" }
        };


        private static readonly Dictionary<byte, string> AxisFaultBits = new Dictionary<byte, string>()
        {
            { 1, "Motor Overcurrent" },
            { 2, "Motor Commutation" },
            { 3, "Motor Overspeed Factory Limit" },
            { 4, "Motor Overspeed User Limit" },
            { 5, "Motor Overtemperature Factory Limit" },
            { 6, "Motor Overtemperature User Limit" },
            { 7, "Motor Thermal Overload Factory Limit" },
            { 8, "Motor Thermal Overload User Limit" },
            { 9, "Motor Phase Loss" },
            { 10, "Inverter Overcurrent" },
            { 11, "Inverter Overtemperature Factory Limit" },
            { 12, "Inverter Overtemperature User Limit" },
            { 13, "Inverter Thermal Overload Factory Limit" },
            { 14, "Inverter Thermal Overload User Limit" },
            { 15, "Converter Overcurrent" },
            { 16, "Converter Ground Current Factory Limit" },
            { 17, "Converter Ground Current User Limit" },
            { 18, "Converter Overtemperature Factory Limit" },
            { 19, "Converter Overtemperature User Limit" },
            { 20, "Converter Thermal Overload Factory Limit" },
            { 21, "Converter Thermal Overload User Limit" },
            { 22, "Converter AC Power Loss" },
            { 23, "Converter AC Single Phase Loss" },
            { 24, "Converter AC Phase Short" },
            { 25, "Converter Pre-Charge Failure" },
            { 27, "Bus Regulator Overtemperature Factory Limit" },
            { 28, "Bus Regulator Overtemperature User Limit" },
            { 29, "Bus Regulator Thermal Overload Factory Limit" },
            { 30, "Bus Regulator Thermal Overload User Limit" },
            { 31, "Bus Regulator Failure" },
            { 32, "Bus Capacitor Module Failure" },
            { 33, "Bus Undervoltage Factory Limit" },
            { 34, "Bus Undervoltage User Limit" },
            { 35, "Bus Overvoltage Factory Limit" },
            { 36, "Bus Overvoltage User Limit" },
            { 37, "Bus Power Loss" },
            { 38, "Bus Power Blown Fuse" },
            { 39, "Bus Power Leakage" },
            { 40, "Bus Power Sharing" },
            { 41, "Feedback Signal Noise Factory Limit" },
            { 42, "Feedback Signal Noise User Limit" },
            { 43, "Feedback Signal Loss Factory Limit" },
            { 44, "Feedback Signal Loss User Limit" },
            { 45, "Feedback Data Loss Factory Limit" },
            { 46, "Feedback Data Loss User Limit" },
            { 47, "Feedback Device Failure" },
            { 48, "Sensor Failure" },
            { 49, "Brake Slip" },
            { 50, "Hardware Overtravel Positive" },
            { 51, "Hardware Overtravel Negative" },
            { 52, "Position Overtravel Positive" },
            { 53, "Position Overtravel Negative" },
            { 54, "Excessive Position Error" },
            { 55, "Excessive Velocity Error" },
            { 56, "Overtorque Limit" },
            { 57, "Undertorque Limit" },
            { 58, "Excessive Bus Voltage Error" },
            { 59, "Ambient Temperature Rise" },
            { 60, "Illegal Control Mode" },
            { 61, "Enable Input Deactivated" },
            { 62, "Controller Initiated Exception" },
            { 63, "External Exception Input" },

        };

        private static readonly Dictionary<byte, string> AxisFaultMfgBits = new Dictionary<byte, string>()
        {
            { 5, "Feedback Battery Loss" },
            { 6, "Feedback Battery Low" },
            { 26, "Runtime Error" }
        };

        private static readonly Dictionary<byte, string> ModuleFaultBits = new Dictionary<byte, string>()
        {
            { 0, "Control Sync Fault" },
            { 1, "Module Sync Fault" },
            { 2, "Timer Event Fault" },
            { 3, "Module Hard Fault" },
            { 6, "Module Connection Fault" }, // Reserved, need check
            { 7, "Conn. Format Fault" },
            { 8, "Local Mode Fault" },
            { 9, "CPU Fault" },
            { 10, "Clock Jitter Fault" },
            { 11, "Cyclic Read Fault" },
            { 12, "Cyclic Write Fault" },
            { 13, "Clock Skew Fault" },
            { 14, "Control Conn. Fault" },
            { 16, "Module Clock Sync Fault" },
            { 17, "Logic Fault" },
            { 18, "Duplicate Address" },
        };

        private static readonly Dictionary<byte, string> AxisAlarmMfgBits = new Dictionary<byte, string>()
        {
            { 5, "Feedback Battery Loss" },
            { 6, "Feedback Battery Low" },
            { 26, "Runtime Error" }
        };

        private static readonly Dictionary<byte, string> ActionDictionary = new Dictionary<byte, string>()
        {
            { 0, "No Action" },
            { 1, "Planner Stop" },
            { 2, "Ramped Stop" },
            { 3, "Torque Limited Stop" },
            { 4, "Immediate Stop(Coast)" }
        };

        private static readonly Dictionary<byte, string> EndStateDictionary = new Dictionary<byte, string>()
        {
            { 0, "No Action" },
            { 1, "Hold" },
            { 2, "Disabled" },
            { 3, "Shutdown" }
        };

        public static string GetDateTime(long timeStamp)
        {
            DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddTicks(timeStamp / 100);
            return time.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss.fff");
        }

        public static bool IsReset(AxisExceptionLogItem item)
        {
            if (item.Type == 0)
            {
                if (item.Code == 1
                    || item.Code == 2
                    || item.Code == 3
                    || item.Code == 4
                    || item.Code == 255)
                    return true;
            }
            else if (item.Type == 0x100)
            {
                if (item.Code == 255)
                    return true;
            }

            return false;
        }

        public static string GetFaultSource(AxisExceptionLogItem item)
        {
            byte type = (byte)(item.Type & 0xFF);

            if (FaultTypeDictionary.ContainsKey(type))
                return FaultTypeDictionary[type];

            return item.Type.ToString();
        }

        public static string GetFaultCondition(AxisExceptionLogItem item)
        {
            byte type = (byte)(item.Type & 0xFF);

            if (type == 0)
            {
                switch (item.Code)
                {
                    case 0:
                        return "No Faults";
                    case 1:
                        return "Module Reset";
                    case 2:
                        return "Fault Reset";
                    case 3:
                        return "Shutdown Reset";
                    case 4:
                        return "Connection Reset";
                    case 255:
                        return "Fault Log Reset";

                }
            }
            else if (type == 3)
            {
                // Axis Fault
                if (AxisFaultBits.ContainsKey(item.Code))
                {
                    return AxisFaultBits[item.Code];
                }
            }
            else if (type == 4)
            {
                // Axis Fault - Mfg
                if (AxisFaultMfgBits.ContainsKey(item.Code))
                {
                    return AxisFaultMfgBits[item.Code];
                }
            }
            else if (type == 5)
            {
                // Motion Fault
                switch (item.Code)
                {
                    case 1:
                        return "Soft Travel Limit Positive Fault";
                    case 2:
                        return "Soft Travel Limit Negative Fault";
                }
            }
            else if (type == 6)
            {
                // Module Fault
                if (ModuleFaultBits.ContainsKey(item.Code))
                {
                    return ModuleFaultBits[item.Code];
                }
            }

            return item.Code.ToString();
        }

        public static string GetFaultSubCode(AxisExceptionLogItem item)
        {
            return item.SubCode.ToString();
        }

        public static string GetFaultAction(AxisExceptionLogItem item)
        {
            if (ActionDictionary.ContainsKey(item.StopAction))
            {
                return ActionDictionary[item.StopAction];
            }

            return item.StopAction.ToString();
        }

        public static string GetFaultEndState(AxisExceptionLogItem item)
        {
            if (EndStateDictionary.ContainsKey(item.StateChange))
            {
                return EndStateDictionary[item.StateChange];
            }

            return item.StateChange.ToString();
        }

        public static string GetAlarmSource(AxisExceptionLogItem item)
        {
            byte type = (byte)(item.Type & 0xFF);

            if (AlarmTypeDictionary.ContainsKey(type))
            {
                return AlarmTypeDictionary[type];
            }

            return type.ToString();
        }

        public static string GetAlarmCondition(AxisExceptionLogItem item)
        {
            byte type = (byte)(item.Type & 0xFF);

            if (type == 0)
            {
                switch (item.Code)
                {
                    case 255:
                        return "Alarm Log Reset";
                }
            }
            else if (type == 1)
            {
                // Start Inhibit
                switch (item.Code)
                {
                    case 1:
                        return "Axis Enable Input";
                    case 2:
                        return "Motor Not Configured";
                    case 3:
                        return "Feedback Not Configured";
                    case 4:
                        return "Commutation Not Configured";
                    case 5:
                        return "Safe Torque Off Active";
                }
            }
            else if (type == 2)
            {
                // Start Inhibit - Mfg
                switch (item.Code)
                {
                    case 5:
                        return "Safe Torque Off";
                }
            }
            else if (type == 3)
            {
                // Axis Alarm use AxisFaultBits
                if (AxisFaultBits.ContainsKey(item.Code))
                {
                    return AxisFaultBits[item.Code];
                }
            }
            else if (type == 4)
            {
                // Axis Alarm - Mfg , use AxisFaultMfgBits???
                if (AxisAlarmMfgBits.ContainsKey(item.Code))
                {
                    return AxisAlarmMfgBits[item.Code];
                }
            }
            else if (type == 5)
            {
                // Motion Alarm
                switch (item.Code)
                {
                    case 1:
                        return "Soft Travel Limit Positive Alarm";
                    case 2:
                        return "Soft Travel Limit Negative Alarm";
                }
            }

            return item.Code.ToString();
        }

        public static string GetAlarmSubCode(AxisExceptionLogItem item)
        {
            return item.SubCode.ToString();
        }

        public static string GetAlarmAction(AxisExceptionLogItem item)
        {
            switch (item.StopAction)
            {
                case 0:
                    return "Alarm Off";
                case 1:
                    return "Alarm On";
            }

            return item.StopAction.ToString();
        }

        public static string GetFaultEntryToolTip(AxisExceptionLogItem item)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Fault Entry -");
            builder.AppendLine($"Type: {item.Type & 0xFF}");
            builder.AppendLine($"Code: {item.Code}");
            builder.AppendLine($"SubCode: {item.SubCode}");
            builder.AppendLine($"StopAction: {item.StopAction}");
            builder.AppendLine($"StateChange: {item.StateChange}");
            builder.AppendLine($"Timestamp: {item.Timestamp}");

            return builder.ToString();
        }

        public static string GetAlarmEntryToolTip(AxisExceptionLogItem item)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Alarm Entry -");
            builder.AppendLine($"Type: {item.Type & 0xFF}");
            builder.AppendLine($"Code: {item.Code}");
            builder.AppendLine($"SubCode: {item.SubCode}");
            builder.AppendLine($"State: {item.StopAction}");
            builder.AppendLine($"Timestamp: {item.Timestamp}");

            return builder.ToString();
        }
    }
}