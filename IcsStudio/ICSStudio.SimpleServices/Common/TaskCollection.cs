using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.SimpleServices.Common
{
    public class TaskCollection : ITaskCollection
    {
        private readonly List<ITask> _tasks;

        public TaskCollection(IController controller)
        {
            ParentController = controller;
            Uid = Guid.NewGuid().GetHashCode();

            _tasks = new List<ITask>();
        }

        public IEnumerator<ITask> GetEnumerator() => _tasks.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IController ParentController { get; }
        public int Uid { get; }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Count => _tasks.Count;

        public ITask this[int uid]
        {
            get { throw new NotImplementedException(); }
        }

        public ITask this[string name]
        {
            get
            {
                foreach (var task in _tasks)
                {
                    if (task.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                        return task;
                }

                return null;
            }
        }

        public ITask TryGetChildByUid(int uid)
        {
            throw new NotImplementedException();
        }

        public ITask TryGetChildByName(string name)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<ComponentCoreInfo> GetComponentCoreInfoList()
        {
            throw new NotImplementedException();
        }

        public ComponentCoreInfo GetComponentCoreInfo(int uid)
        {
            throw new NotImplementedException();
        }

        public void AddTask(CTask task)
        {
            task.ParentCollection = this;

            _tasks.Add(task);

            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, task));
        }

        public void DeleteTask(ITask task)
        {
            if (_tasks.Contains(task))
            {
                CTask myTask = task as CTask;
                if (myTask != null)
                    myTask.ParentCollection = null;

                _tasks.Remove(task);

                CollectionChanged?.Invoke(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, task));
            }
        }

        public void Clear()
        {
            _tasks.Clear();

            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public int GetIndex(ITask task)
        {
            return _tasks.IndexOf(task);
        }
    }
}
