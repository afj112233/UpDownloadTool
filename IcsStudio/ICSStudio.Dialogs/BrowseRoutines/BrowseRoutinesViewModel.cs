using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Notification;

namespace ICSStudio.Dialogs.BrowseRoutines
{
    public class BrowseRoutinesViewModel : ViewModelBase, IConsumer
    {
        private readonly IRoutineCollection _routines;
        private IRoutine _selectedRoutine;

        public BrowseRoutinesViewModel(IRoutineCollection routines, double controlWidth)
        {
            _routines = routines;

            foreach (var item in routines)
                if (!item.Name.Equals(routines.ParentProgram.MainRoutineName))
                    Routines.Add(item);
            ControlWidth = controlWidth;

            routines.CollectionChanged += Routines_CollectionChanged;
            routines.ParentProgram.PropertyChanged += ParentProgram_PropertyChanged;
        }

        public ObservableCollection<IRoutine> Routines { get; } = new ObservableCollection<IRoutine>();

        public IRoutine SelectedRoutine
        {
            get { return _selectedRoutine; }
            set { Set(ref _selectedRoutine, value); }
        }

        public double ControlWidth { get; set; }
        public Action ShowChildren { get; set; }

        private void Routines_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
                foreach (var item in e.NewItems)
                    if ((item as IRoutine) != null)
                        Routines.Add(item as IRoutine);

            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
                foreach (var item in e.OldItems)
                    if ((item as IRoutine) != null)
                        Routines.Remove(item as IRoutine);
        }

        private void ParentProgram_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(IProgramModule.MainRoutineName)))
            {
                Routines.Clear();
                foreach (var item in _routines)
                    if (!item.Name.Equals(_routines.ParentProgram.MainRoutineName))
                        Routines.Add(item);
                ShowChildren?.Invoke();
            }
        }

        public void Consume(MessageData message)
        {

        }
    }
}