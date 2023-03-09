using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Project;
using Microsoft.VisualStudio.Shell;
using CTask = ICSStudio.SimpleServices.Common.CTask;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    internal class NewTaskViewModel : ViewModelBase
    {
        private bool? _dialogResult;
        private List<TaskType> _taskTypeList;
        private TaskType _taskType;
        private Visibility _periodTypeVisibility;
        private Visibility _eventTypeVisibility;
        private Visibility _priorityVisibility;
        private float _period;
        private int _priority;
        private float _watchdog;
        private bool _disableUpdateOutputs;
        private bool _isInhibited;

        private readonly IController _controller;

        public NewTaskViewModel()
        {
            var projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            _controller = projectInfoService?.Controller;
            Contract.Assert(_controller != null);


            UpdateTaskTypeSource();
            TaskType = _taskTypeList[0];

            _period = 10.0f;
            _priority = 10;
            _watchdog = 500.0f;
            _disableUpdateOutputs = false;
            _isInhibited = false;

            OkCommand = new RelayCommand(ExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {

        }

        private void UpdateTaskTypeSource()
        {
            _taskTypeList = new List<TaskType> {TaskType.Periodic, TaskType.Event};

            bool beHaveContinuousTask = true;
            foreach (var task in _controller.Tasks)
            {
                if (task.Type == TaskType.Continuous)
                {
                    beHaveContinuousTask = false;
                    break;
                }
            }

            if (beHaveContinuousTask)
                _taskTypeList.Insert(0, TaskType.Continuous);

            TaskTypeSource = _taskTypeList.Select(value => new
            {
                DisplayName = value.ToString(),
                Value = value
            }).ToList();
        }

        public RelayCommand OkCommand { get; }
        public RelayCommand CancelCommand { get; }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public IList TaskTypeSource { get; set; }

        public TaskType TaskType
        {
            get { return _taskType; }
            set
            {
                Set(ref _taskType, value);
                PeriodTypeVisibility = _taskType == TaskType.Periodic ? Visibility.Visible : Visibility.Collapsed;
                EventTypeVisibility = _taskType == TaskType.Event ? Visibility.Visible : Visibility.Collapsed;
                PriorityVisibility = _taskType == TaskType.Continuous ? Visibility.Hidden : Visibility.Visible;
                DisableUpdateOutputs = _taskType == TaskType.Event;
            }
        }

        public Visibility PeriodTypeVisibility
        {
            get { return _periodTypeVisibility; }
            set { Set(ref _periodTypeVisibility, value); }
        }

        public Visibility EventTypeVisibility
        {
            get { return _eventTypeVisibility; }
            set { Set(ref _eventTypeVisibility, value); }
        }

        public Visibility PriorityVisibility
        {
            get { return _priorityVisibility; }
            set { Set(ref _priorityVisibility, value); }
        }

        public float Period
        {
            get { return _period; }
            set { Set(ref _period, value); }
        }

        public int Priority
        {
            get { return _priority; }
            set { Set(ref _priority, value); }
        }

        public float Watchdog
        {
            get { return _watchdog; }
            set { Set(ref _watchdog, value); }
        }

        public bool DisableUpdateOutputs
        {
            get { return _disableUpdateOutputs; }
            set { Set(ref _disableUpdateOutputs, value); }
        }

        public bool IsInhibited
        {
            get { return _isInhibited; }
            set { Set(ref _isInhibited, value); }
        }

        private void ExecuteOkCommand()
        {
            //1. check input
            if (IsValidTaskName(Name)
                && IsValidTaskType(TaskType)
                && IsValidPeriod(Period)
                && IsValidPriority(Priority)
                && IsValidWatchdog(Watchdog))
            {
                //2. create and add
                var taskCollection = _controller.Tasks as TaskCollection;
                if (taskCollection != null)
                {
                    var task = new CTask(_controller)
                    {
                        Name = Name,
                        Description = Description,
                        Type = TaskType,
                        Priority = Priority,
                        Watchdog = Watchdog,
                        IsInhibited = IsInhibited,
                        Rate = Period,
                        DisableUpdateOutputs = DisableUpdateOutputs
                    };

                    taskCollection.AddTask(task);
                }

                DialogResult = true;
            }
        }

        private void ExecuteCancelCommand()
        {
            DialogResult = false;
        }

        private bool IsValidTaskName(string taskName)
        {
            string warningMessage = LanguageManager.GetInstance().ConvertSpecifier("Failed to create task.");
            string warningReason = string.Empty;
            bool isValid = true;


            if (string.IsNullOrEmpty(taskName))
            {
                isValid = false;
                warningReason = LanguageManager.GetInstance().ConvertSpecifier("Invalid name.");
            }

            Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
            if (string.IsNullOrWhiteSpace(taskName) || !regex.IsMatch(taskName))
            {
                isValid = false;
                warningReason = LanguageManager.GetInstance().ConvertSpecifier("Invalid name.");
            }

            if (isValid)
            {
                if (taskName.Length > 40 || taskName.EndsWith("_") ||
                    taskName.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    isValid = false;
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("Invalid name.");
                }
            }

            // key word
            if (isValid)
            {
                string[] keyWords =
                {
                    "goto",
                    "repeat", "until", "or", "end_repeat",
                    "return", "exit",
                    "if", "then", "elsif", "else", "end_if",
                    "case", "of", "end_case",
                    "for", "to", "by", "do", "end_for",
                    "while", "end_while",
                    "not", "mod", "and", "xor", "or",
                    "ABS","SQRT",
                    "LOG","LN",
                    "DEG","RAD","TRN",
                    "ACS","ASN","ATN","COS","SIN","TAN"
                };
                foreach (var keyWord in keyWords)
                {
                    if (keyWord.Equals(taskName, StringComparison.OrdinalIgnoreCase))
                    {
                        isValid = false;
                        warningReason = LanguageManager.GetInstance().ConvertSpecifier("Invalid name.");
                    }
                }
            }

            if (isValid)
            {
                var task = _controller.Tasks[taskName];
                if (task != null)
                {
                    isValid = false;
                    warningMessage = LanguageManager.GetInstance().ConvertSpecifier("Failed to create task'")
                                     + $" {taskName} "
                                     + LanguageManager.GetInstance().ConvertSpecifier("New Task.'.");
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("Already exists.");
                }
            }

            if (!isValid)
            {
                var warningDialog = new WarningDialog(warningMessage, warningReason)
                    {Owner = Application.Current.MainWindow};
                warningDialog.ShowDialog();
            }

            return isValid;
        }

        private bool IsValidTaskType(TaskType taskType)
        {
            //TODO(gjc): need edit later
            if (taskType == TaskType.Continuous || taskType == TaskType.Periodic)
                return true;

            string warningMessage = LanguageManager.GetInstance().ConvertSpecifier("Failed to create task.");
            string warningReason = LanguageManager.GetInstance().ConvertSpecifier("Currently not support")
                                   + $" {taskType} "
                                   + LanguageManager.GetInstance().ConvertSpecifier(".");

            var warningDialog = new WarningDialog(warningMessage, warningReason)
                {Owner = Application.Current.MainWindow};
            warningDialog.ShowDialog();

            return false;
        }

        private bool IsValidPeriod(float period)
        {
            const float kMinPeriod = 0.1f;
            const float kMaxPeriod = 2000000.0f;

            var isValid = period >= kMinPeriod && period <= kMaxPeriod;

            if (!isValid)
            {
                string warningMessage = LanguageManager.GetInstance().ConvertSpecifier("Invalid Period.");
                string warningReason = LanguageManager.GetInstance().ConvertSpecifier("Enter a period in the range")
                                       + $" ({kMinPeriod:###0.0###} - {kMaxPeriod:###0.0###}) "
                                       + LanguageManager.GetInstance().ConvertSpecifier("ms.");

                var warningDialog = new WarningDialog(warningMessage, warningReason)
                    {Owner = Application.Current.MainWindow};
                warningDialog.ShowDialog();
            }

            return isValid;
        }

        private bool IsValidPriority(int priority)
        {
            const int kMinPriority = 1;
            const int kMaxPriority = 15;

            var isValid = priority >= kMinPriority && priority <= kMaxPriority;

            if (!isValid)
            {
                string warningMessage = LanguageManager.GetInstance().ConvertSpecifier("Invalid Priority.");
                string warningReason = LanguageManager.GetInstance().ConvertSpecifier("Enter a priority in the range")
                                    + $" ({kMinPriority} - {kMaxPriority}) "
                                    + LanguageManager.GetInstance().ConvertSpecifier("Priority.");

                var warningDialog = new WarningDialog(warningMessage, warningReason)
                    {Owner = Application.Current.MainWindow};
                warningDialog.ShowDialog();
            }

            return isValid;
        }

        private bool IsValidWatchdog(float watchdog)
        {
            const float kMinWatchdog = 0.1f;
            const float kMaxWatchdog = 2000000.0f;

            var isValid = watchdog >= kMinWatchdog && watchdog <= kMaxWatchdog;

            if (!isValid)
            {
                string warningMessage = LanguageManager.GetInstance().ConvertSpecifier("Invalid Watchdog.");
                string warningReason = LanguageManager.GetInstance().ConvertSpecifier("Enter a watchdog in the range")
                                       + $" ({kMinWatchdog:###0.0###} - {kMaxWatchdog:###0.0###}) "
                                       + LanguageManager.GetInstance().ConvertSpecifier("ms.");


                var warningDialog = new WarningDialog(warningMessage, warningReason)
                    {Owner = Application.Current.MainWindow};
                warningDialog.ShowDialog();
            }

            return isValid;
        }

        public override void Cleanup()
        {
            base.Cleanup();

            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }
    }
}
