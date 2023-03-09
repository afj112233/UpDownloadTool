using System;
using System.Collections;
using System.Windows;
using GalaSoft.MvvmLight;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.TaskProperties.panel
{
    public class ConfigurationViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private readonly Controller _controller;
        private readonly CTask _task;
        private float _period;
        private bool _isPeriodEnabled;
        private bool _isDirty;
        private int _priority;
        private float _watchdog;
        private bool _inhibitTask;
        private bool _isInhibitTaskEnabled;
        private bool _disableUpdateOutputs;

        public ConfigurationViewModel(Configuration panel, ITask task)
        {
            Control = panel;
            panel.DataContext = this;
            _task = task as CTask;
            _controller = _task?.ParentController as Controller ?? Controller.GetInstance();
            SetValue();

            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                _controller, "IsOnlineChanged", OnIsOnlineChanged);
            _isInhibitTaskEnabled = _controller.KeySwitchPosition != ControllerKeySwitch.RunKeySwitch;
            _isPeriodEnabled = _controller.KeySwitchPosition != ControllerKeySwitch.RunKeySwitch;
            WeakEventManager<Controller,EventArgs>.AddHandler(_controller, "KeySwitchChanged",OnKeySwitchChanged);
        }

        public void SetValue()
        {
            ITaskCollection itc = _task.ParentCollection;
            bool flag = false;
            foreach (var t in itc)
            {
                if (t.Type == TaskType.Continuous)
                {
                    if (t != _task) flag = true;
                }
            }

            TypeList = EnumHelper.ToDataSource<TaskType>();
            TypeList.Remove(TypeList[0]);
            if (flag) TypeList.Remove(TypeList[2]);
            TypeSelected = _task.Type;
            
            Period = _task.Rate;
            Priority = _task.Priority;
            Watchdog = _task.Watchdog;
            DisableUpdateOutputs = _task.DisableUpdateOutputs;
            InhibitTask = _task.IsInhibited;
        }

        public bool IsOnlineEnabled => !_controller.IsOnline;

        public bool IsEnabled
        {
            set { Set(ref _isEnabled, value); }
            get { return _isEnabled; }
        }

        public bool DisableUpdateOutputs
        {
            set
            {
                _disableUpdateOutputs = value;
                Compare();
            }
            get { return _disableUpdateOutputs; }
        }

        public bool InhibitTask
        {
            set
            {
                _inhibitTask = value;
                Compare();
            }
            get { return _inhibitTask; }
        }

        public bool IsInhibitTaskEnabled
        {
            get { return _isInhibitTaskEnabled; }
            set { Set(ref _isInhibitTaskEnabled, value); }
        }

        public float Watchdog
        {
            set
            {
                _watchdog = value;
                Compare();
            }
            get { return _watchdog; }
        }

        public int Priority
        {
            set
            {
                _priority = value;
                Compare();
            }
            get { return _priority; }
        }

        public float Period
        {
            set
            {
                Set(ref _period, value);
                Compare();
            }
            get { return _period; }
        }

        public bool IsPeriodEnabled
        {
            get { return _isPeriodEnabled; }
            set { Set(ref _isPeriodEnabled, value); }
        }

        public void Compare()
        {
            IsDirty = false;
            if (TypeSelected != _task.Type) IsDirty = true;
            if (Priority != _task.Priority) IsDirty = true;
            if (Math.Abs(Period - _task.Rate) > float.Epsilon) IsDirty = true;
            if (Math.Abs(Watchdog - _task.Watchdog) > float.Epsilon) IsDirty = true;
            if (InhibitTask != _task.IsInhibited) IsDirty = true;
            if (DisableUpdateOutputs != _task.DisableUpdateOutputs) IsDirty = true;
        }

        public void Save()
        {
            _task.Type = TypeSelected;
            _task.Priority = Priority;
            _task.Rate = (int) (Period * 1000) / 1000f;
            //Period = _task.Rate;
            _task.Watchdog = Watchdog;
            //_task.IsInhibited = InhibitTask;
            _task.DisableUpdateOutputs = DisableUpdateOutputs;

            if (Math.Abs(_task.Rate - Period) < double.Epsilon)
            {
                Period = _task.Rate;

                if (_task.ParentController.IsOnline)
                    ((Controller)_task.ParentController)?.SetPeriod(_task, _task.Rate);
            }

            if (InhibitTask != _task.IsInhibited)
            {
                _task.IsInhibited = InhibitTask;

                if (_task.ParentController.IsOnline)
                    ((Controller)_task.ParentController)?.SetInhibited(_task, InhibitTask);
            }
        }

        public bool CheckInvalid()
        {
            if (TypeSelected == TaskType.Periodic && (Period < 0.1 || Period > 2000000))
            {
                MessageBox.Show(LanguageManager.GetInstance().ConvertSpecifier("Invalid Period.") +"\n"
                    + LanguageManager.GetInstance().ConvertSpecifier("Enter a period in the range(0.100 - 2000000.000)ms.")
                    , "ICS Studio",
                    MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }

            if (Watchdog < 0.1 || Watchdog > 2000000)
            {
                MessageBox.Show(LanguageManager.GetInstance().ConvertSpecifier("Invalid Watchdog.") + "\n"
                        + LanguageManager.GetInstance().ConvertSpecifier("Enter a watchdog in the range(0.100 - 2000000.000)ms.")
                        , "ICS Studio",
                    MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }

            foreach (var task in _task.ParentCollection)
            {
                if (TypeSelected == TaskType.Continuous && task.Type == TaskType.Continuous && task != _task)
                {
                    string warningMessage = LanguageManager.GetInstance().ConvertSpecifier("ICS Studio only supports one continuous task.");
                    string warningReason = LanguageManager.GetInstance().ConvertSpecifier("Choose another type.");
                    var warningDialog = new WarningDialog(warningMessage, warningReason)
                        {Owner = Application.Current.MainWindow};
                    warningDialog.ShowDialog();
                    return false;
                }
            }

            return true;
        }

        public void SetVisible()
        {
            if (TypeSelected == TaskType.Continuous)
            {
                TriggerVisible = Visibility.Collapsed;
                TagVisible = Visibility.Collapsed;
                Row4Visible = Visibility.Collapsed;
                PriorityVisible = Visibility.Collapsed;
                PeriodVisible = Visibility.Collapsed;
            }
            else if (TypeSelected == TaskType.Event)
            {
                TriggerVisible = Visibility.Visible;
                TagVisible = Visibility.Visible;
                Row4Visible = Visibility.Visible;
                PriorityVisible = Visibility.Visible;
                PeriodVisible = Visibility.Collapsed;
            }
            else if (TypeSelected == TaskType.Periodic)
            {
                TriggerVisible = Visibility.Collapsed;
                TagVisible = Visibility.Collapsed;
                Row4Visible = Visibility.Collapsed;
                PriorityVisible = Visibility.Visible;
                PeriodVisible = Visibility.Visible;
            }

            RaisePropertyChanged("TriggerVisible");
            RaisePropertyChanged("TagVisible");
            RaisePropertyChanged("Row4Visible");
            RaisePropertyChanged("PriorityVisible");
            RaisePropertyChanged("PeriodVisible");

        }

        public void SetPeriod()
        {
            if (TypeSelected == TaskType.Periodic)
            {
                if (Period == 0)
                {
                    Period = 10.000F;
                }
            }
        }

        public Visibility PeriodVisible { set; get; }
        public Visibility PriorityVisible { set; get; }
        public Visibility Row4Visible { set; get; }
        public Visibility TriggerVisible { set; get; }
        public Visibility TagVisible { set; get; }
        public TaskType _taskType;
        private bool _isEnabled;

        public TaskType TypeSelected
        {
            set
            {
                _taskType = value;
                SetVisible();
                SetPeriod();
                Compare();
            }
            get { return _taskType; }
        }

        public IList TypeList { set; get; }

        public object Owner { get; set; }
        public object Control { get; }

        public void LoadOptions()
        {

        }

        public bool SaveOptions()
        {
            return true;
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    IsDirtyChanged?.Invoke(this, EventArgs.Empty);
                }

                Set(ref _isDirty, value);
            }
        }

        public event EventHandler IsDirtyChanged;


        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                RaisePropertyChanged(nameof(IsOnlineEnabled));
                
            });
        }

        private void OnKeySwitchChanged(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                InhibitTask = _task.IsInhibited;
                RaisePropertyChanged(nameof(InhibitTask));
                IsInhibitTaskEnabled = _controller.KeySwitchPosition != ControllerKeySwitch.RunKeySwitch;

                Period = _task.Rate;
                IsPeriodEnabled = _controller.KeySwitchPosition != ControllerKeySwitch.RunKeySwitch;
            });
        }

        public override void Cleanup()
        {
            base.Cleanup();
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                _controller, "IsOnlineChanged", OnIsOnlineChanged);
            WeakEventManager<Controller, EventArgs>.RemoveHandler(_controller, "KeySwitchChanged", OnKeySwitchChanged);
        }
    }
}
