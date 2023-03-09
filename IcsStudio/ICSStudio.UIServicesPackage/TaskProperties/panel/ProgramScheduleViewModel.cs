using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.UIServicesPackage.TaskProperties.panel
{
    public class ProgramScheduleViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private CTask _task;
        private bool _isDirty;
        private readonly List<Item> _scheduledList;
        private readonly List<Item> _unscheduledList;

        public ProgramScheduleViewModel(ProgramSchedule panel, ITask task)
        {
            Control = panel;
            _task = task as CTask;
            panel.DataContext = this;
            ScheduledList = new ObservableCollection<Item>();
            UnscheduledList = new ObservableCollection<Item>();
            SelectedItemsChangedCommand =
                new RelayCommand<SelectionChangedEventArgs>(ExecuteSelectedItemsChangedCommand);
            UnassignedSelectedItemsChangedCommand =
                new RelayCommand<SelectionChangedEventArgs>(ExecuteUnassignedSelectedItemsChangedCommand);
            _scheduledList = new List<Item>();
            _unscheduledList = new List<Item>();
            MoveUp = new RelayCommand(MoveUpClick,CanExecuteCommand);
            MoveDown = new RelayCommand(MoveDownClick,CanExecuteCommand);
            AddCommand = new RelayCommand(Add,CanExecuteCommand);
            RemoveCommand = new RelayCommand(Remove,CanExecuteCommand);
            SetList();
        }

        protected virtual bool CanExecuteCommand()
        {
            var controller = Controller.GetInstance();
            return !controller.IsOnline;
        }

        public override void Cleanup()
        {
            IController controller = _task.ParentController;
            controller.Programs.CollectionChanged -= OnProgramCollectionChanged;
        }

        public void Compare()
        {
            List<Item> list1 = new List<Item>();
            List<Item> list2 = new List<Item>();
            foreach (var item in ScheduledList)
            {
                list1.Add(item);
            }

            foreach (var item in UnscheduledList)
            {
                list2.Add(item);
            }

            IsDirty = !list1.SequenceEqual(_scheduledList) || !list2.SequenceEqual(_unscheduledList);
        }

        public void Save()
        {
            IController controller = _task.ParentController;
            controller.Programs.CollectionChanged -= OnProgramCollectionChanged;

            foreach (var item in UnscheduledList)
            {
                if (!Check(item.Program)) continue;

                _task.UnscheduleProgram(item.Program);
            }

            foreach (var item in ScheduledList)
            {
                _task.ScheduleProgram(item.Program);
            }
        }

        public bool CheckInvalid()
        {
            foreach (var item in ScheduledList)
            {
                if (!Check(item.Program))
                {
                    var warningDialog = new WarningDialog("Failed to schedule programs",
                            "Program scheduled under a task other than the specified task")
                        {Owner = Application.Current.MainWindow};
                    warningDialog.ShowDialog();
                    return false;
                }
            }

            return true;
        }

        public void OnProgramCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                IProgram program = (IProgram) ((object[]) e.NewItems.SyncRoot)[0];
                var item = new Item(program);
                if (program.ParentTask == _task)
                {
                    ScheduledList.Add(item);
                    _scheduledList.Add(item);
                }
                else if (program.ParentTask == null)
                {
                    UnscheduledList.Add(item);
                    _unscheduledList.Add(item);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                IProgram program = (IProgram) e.OldItems[0];
                var item = ScheduledList.FirstOrDefault(s => s.Program == program);
                ScheduledList.Remove(item);
                item = _scheduledList.FirstOrDefault(s => s.Program == program);
                _scheduledList.Remove(item);
                item = UnscheduledList.FirstOrDefault(s => s.Program == program);
                UnscheduledList.Remove(item);
                item = _unscheduledList.FirstOrDefault(s => s.Program == program);
                _unscheduledList.Remove(item);
            }
        }

        public void SetList()
        {
            IController controller = _task.ParentController;
            controller.Programs.CollectionChanged += OnProgramCollectionChanged;
            ScheduledList.Clear();
            UnscheduledList.Clear();
            _scheduledList.Clear();
            _unscheduledList.Clear();

            foreach (IProgram program in controller.Programs)
            {
                var item = new Item(program);
                if (program.Name == controller.MajorFaultProgram || program.Name == controller.PowerLossProgram)
                {
                    continue;
                }

                if (program.ParentTask == _task)
                {
                    ScheduledList.Add(item);
                    _scheduledList.Add(item);
                }

                if (program.ParentTask == null)
                {
                    UnscheduledList.Add(item);
                    _unscheduledList.Add(item);
                }
            }

        }

        #region Command

        private List<Item> _selectedPrograms = new List<Item>();
        private List<Item> _selectedUnassignedPrograms = new List<Item>();
        public RelayCommand<SelectionChangedEventArgs> SelectedItemsChangedCommand { set; get; }
        public RelayCommand<SelectionChangedEventArgs> UnassignedSelectedItemsChangedCommand { set; get; }

        private void ExecuteSelectedItemsChangedCommand(SelectionChangedEventArgs e)
        {
            if (e?.RemovedItems != null)
            {
                foreach (Item removedItem in e.RemovedItems)
                {
                    _selectedPrograms.Remove(removedItem);
                }
            }

            if (e?.AddedItems != null)
            {
                foreach (Item addedItem in e.AddedItems)
                {
                    _selectedPrograms.Add(addedItem);
                }
            }
        }

        private void ExecuteUnassignedSelectedItemsChangedCommand(SelectionChangedEventArgs e)
        {
            if (e?.RemovedItems != null)
            {
                foreach (Item removedItem in e.RemovedItems)
                {
                    _selectedUnassignedPrograms.Remove(removedItem);
                }
            }

            if (e?.AddedItems != null)
            {
                foreach (Item addedItem in e.AddedItems)
                {
                    _selectedUnassignedPrograms.Add(addedItem);
                }
            }
        }

        public RelayCommand MoveUp { set; get; }

        public void MoveUpClick()
        {
            if (!_selectedPrograms.Any()) return;
            int index = ScheduledList.Count;
            foreach (var selectedProgram in _selectedPrograms)
            {
                index = Math.Min(index, ScheduledList.IndexOf(selectedProgram));
            }

            if (ScheduledList.Count < 2 || index == 0) return;
            var tmp = new Item[_selectedPrograms.Count];
            _selectedPrograms.CopyTo(tmp);
            foreach (var item in tmp.OrderBy(t => ScheduledList.IndexOf(t)))
            {
                //IProgram selected = ScheduledSelected;
                var offset = ScheduledList.IndexOf(item);
                var temp = ScheduledList[offset - 1];
                ScheduledList[offset - 1] = ScheduledList[offset];
                ScheduledList[offset] = temp;
                item.IsSelected = true;
            }

            RaisePropertyChanged("ScheduledSelected");
            Compare();
        }

        public RelayCommand MoveDown { set; get; }

        public void MoveDownClick()
        {
            if (!_selectedPrograms.Any()) return;
            int index = -1;
            foreach (var selectedProgram in _selectedPrograms)
            {
                index = Math.Max(index, ScheduledList.IndexOf(selectedProgram));
            }

            if (ScheduledList.Count < 2 || index == ScheduledList.Count - 1) return;
            var tmp = new Item[_selectedPrograms.Count];
            _selectedPrograms.CopyTo(tmp);
            foreach (var item in tmp.OrderByDescending(t => ScheduledList.IndexOf(t)))
            {
                var offset = ScheduledList.IndexOf(item);
                var temp = ScheduledList[offset + 1];
                ScheduledList[offset + 1] = ScheduledList[offset];
                ScheduledList[offset] = temp;
                item.IsSelected = true;
            }

            RaisePropertyChanged("ScheduledSelected");
            Compare();
        }

        public RelayCommand AddCommand { set; get; }

        public void Add()
        {
            if (!_selectedUnassignedPrograms.Any()) return;
            foreach (var unassignedProgram in _selectedUnassignedPrograms)
            {
                if (!Check(unassignedProgram.Program))
                {
                    var warningDialog = new WarningDialog("Failed to schedule programs",
                            "Program scheduled under a task other than the specified task")
                        {Owner = Application.Current.MainWindow};
                    warningDialog.ShowDialog();
                    _unscheduledList.Remove(unassignedProgram);
                    UnscheduledList.Remove(unassignedProgram);
                    return;
                }
            }

            var tmp = new Item[_selectedUnassignedPrograms.Count];
            _selectedUnassignedPrograms.CopyTo(tmp);
            foreach (var unassignedProgram in tmp)
            {
                ScheduledList.Add(unassignedProgram);
                UnscheduledList.Remove(unassignedProgram);
            }

            Compare();
        }

        public bool Check(IProgram program)
        {
            IProgram program1 = (_task.ParentController).Programs[program.Name];
            if (program1.ParentTask != null && program1.ParentTask != _task) return false;
            return true;
        }

        public RelayCommand RemoveCommand { set; get; }

        public void Remove()
        {
            if (!_selectedPrograms.Any()) return;
            var tmp = new Item[_selectedPrograms.Count];
            _selectedPrograms.CopyTo(tmp);
            foreach (var selectedProgram in tmp)
            {
                UnscheduledList.Add(selectedProgram);
                ScheduledList.Remove(selectedProgram);
            }

            Compare();
        }

        #endregion

        public object Owner { get; set; }
        public object Control { get; }

        public void LoadOptions()
        {
            //throw new NotImplementedException();
        }

        public bool SaveOptions()
        {
            return true;
        }

        public ObservableCollection<Item> ScheduledList { set; get; }

        public ObservableCollection<Item> UnscheduledList { set; get; }
        //public IProgram ScheduledSelected { set; get; }
        //public IProgram UnscheduledSelected { set; get; }

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
    }

    public class Item : ViewModelBase
    {
        private bool _isSelected;

        public Item(IProgram program)
        {
            Program = program;
        }

        public IProgram Program { get; }

        public bool IsSelected
        {
            set { Set(ref _isSelected, value); }
            get { return _isSelected; }
        }
    }
}
