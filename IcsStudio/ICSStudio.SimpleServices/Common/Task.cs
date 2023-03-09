using System;
using System.Linq;
using ICSStudio.Interfaces.Common;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.Common
{
    public class CTask : BaseComponent, ITask
    {
        private long _maxScanTime;
        private long _lastScanTime;
        private long _maxIntervalTime;
        private long _minIntervalTime;
        private int _overlapCount;

        private bool _isInhibited;
        private TaskType _type;
        private float _rate;
        private float _watchdog;
        private int _priority;
        private bool _disableUpdateOutputs;

        public CTask(IController controller)
        {
            ParentController = controller;
        }

        public override IController ParentController { get; }

        public ITaskCollection ParentCollection { get; set; }

        public bool DisableUpdateOutputs
        {
            get { return _disableUpdateOutputs; }
            set
            {
                if (_disableUpdateOutputs != value)
                {
                    _disableUpdateOutputs = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsInhibited
        {
            get { return _isInhibited; }
            set
            {
                if (_isInhibited != value)
                {
                    _isInhibited = value;
                    RaisePropertyChanged();
                    
                    foreach (var program in ParentController.Programs.Where(p => p.ParentTask == this))
                    {
                        ((Program)program).UpdateRoutineRunStatus = true;
                    }
                }
            }
        }

        public TaskType Type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    RaisePropertyChanged();
                }
            }
        }

        public void ScheduleProgram(IProgram program)
        {
            Program myProgram = program as Program;
            ProgramCollection programCollection = ParentController.Programs as ProgramCollection;

            if (myProgram == null)
                return;

            if (myProgram.ParentTask == null || myProgram.ParentTask == this)
            {
                myProgram.ParentTask = this;

                programCollection?.ScheduleProgram(myProgram);
            }
            else
            {
                throw new NotSupportedException($"ScheduleProgram failed: ParentTask is not {Name}");
            }

        }

        public void UnscheduleProgram(IProgram program)
        {
            Program myProgram = program as Program;

            if (myProgram?.ParentTask == null)
                return;

            if (myProgram.ParentTask == this)
                myProgram.ParentTask = null;
            else
            {
                throw new NotSupportedException($"UnscheduleProgram failed: ParentTask is not {Name}");
            }
        }

        public long MaxScanTime
        {
            get { return _maxScanTime; }
            set
            {
                if (_maxScanTime != value)
                {
                    _maxScanTime = value;
                    RaisePropertyChanged();
                }

            }
        }

        public long LastScanTime
        {
            get { return _lastScanTime; }
            set
            {
                if (_lastScanTime != value)
                {
                    _lastScanTime = value;
                    RaisePropertyChanged();
                }

            }
        }

        public long MaxIntervalTime
        {
            get { return _maxIntervalTime; }
            set
            {
                if (_maxIntervalTime != value)
                {
                    _maxIntervalTime = value;
                    RaisePropertyChanged();
                }
            }
        }

        public long MinIntervalTime
        {
            get { return _minIntervalTime; }
            set
            {
                if (_minIntervalTime != value)
                {
                    _minIntervalTime = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int OverlapCount
        {
            get { return _overlapCount; }
            set
            {
                if (_overlapCount != value)
                {
                    _overlapCount = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int Priority
        {
            get { return _priority; }
            set
            {
                if (_priority != value)
                {
                    _priority = value;
                    RaisePropertyChanged();
                }
            }
        }

        public float Watchdog
        {
            get { return _watchdog; }
            set
            {
                if (Math.Abs(_watchdog - value) > float.Epsilon)
                {
                    _watchdog = value;
                    RaisePropertyChanged();
                }
            }
        }


        public float Rate
        {
            get { return _rate; }
            set
            {
                if (Math.Abs(_rate - value) > float.Epsilon)
                {
                    _rate = value;
                    RaisePropertyChanged();
                }

            }
        }

        public JObject ConvertToJObject()
        {
            JObject task = new JObject
            {
                { "DisableUpdateOutputs", DisableUpdateOutputs },
                { "InhibitTask", IsInhibited },
                { "Name", Name },
                { "Priority", Priority },
                { "Type", (int)Type },
                { "Watchdog", Watchdog },
                { "Description", Description ?? "" }
            };

            if (Type == TaskType.Periodic)
            {
                task["Rate"] = Rate;
            }

            JArray schededPrograms = new JArray();
            foreach (var program in ParentController.Programs)
            {
                if (program.ParentTask == this)
                    schededPrograms.Add(program.Name);
            }

            task.Add("SchededPrograms", schededPrograms);

            return task;
        }
    }
}
