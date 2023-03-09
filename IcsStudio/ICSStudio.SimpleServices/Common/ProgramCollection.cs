using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Notification;
using ICSStudio.SimpleServices.Notification;

namespace ICSStudio.SimpleServices.Common
{
    public class ProgramCollection : IProgramCollection
    {
        private readonly List<IProgram> _programs;

        public ProgramCollection(IController controller)
        {
            ParentController = controller;
            Uid = Guid.NewGuid().GetHashCode();

            _programs = new List<IProgram>();
        }

        public IEnumerator<IProgram> GetEnumerator() => _programs.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose()
        {

        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Remove(int programUid)
        {
            Controller controller = ParentController as Controller;
            if (controller == null)
                return;

            var program = this[programUid];
            if (program != null)
            {
                Program myProgram = program as Program;
                if (myProgram != null)
                    myProgram.ParentCollection = null;

                if (controller.MajorFaultProgram == program.Name)
                {
                    controller.MajorFaultProgram = null;
                }

                if (controller.PowerLossProgram == program.Name)
                {
                    controller.PowerLossProgram = null;
                }

                Notifications.Publish(new MessageData() { Type = MessageData.MessageType.Verify, Object = program });
                program.Dispose();

                _programs.Remove(program);

                CollectionChanged?.Invoke(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, program));
            }
        }

        public void Remove(string programName)
        {
            Controller controller = ParentController as Controller;

            if (controller == null)
                return;

            var program = this[programName];
            if (program != null)
            {
                Program myProgram = program as Program;
                if (myProgram != null)
                    myProgram.ParentCollection = null;

                if (controller.MajorFaultProgram == program.Name)
                {
                    controller.MajorFaultProgram = null;
                }

                if (controller.PowerLossProgram == program.Name)
                {
                    controller.PowerLossProgram = null;
                }

                Notifications.Publish(new MessageData() { Type = MessageData.MessageType.Verify ,Object = program});
                program.Dispose();

                _programs.Remove(program);

                CollectionChanged?.Invoke(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, program));
            }
        }

        public IProgram Add(string programName, ProgramType programType)
        {
            throw new NotImplementedException();
        }

        public IProgram Add(string programName, ProgramType programType, ProgramCreationData programCreationData)
        {
            throw new NotImplementedException();
        }

        public void AddProgram(Program program)
        {
            program.ParentCollection = this;
            _programs.Add(program);
            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, program));
        }

        public IController ParentController { get; }
        public int Uid { get; }
        public int Count => _programs.Count;

        public IProgram this[int uid]
        {
            get
            {
                foreach (IProgram program in _programs)
                {
                    if (program.Uid == uid)
                        return program;
                }

                return null;
            }
        }

        public IProgram this[string name]
        {
            get
            {
                foreach (IProgram program in _programs)
                {
                    if (program.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                        return program;
                }

                return null;
            }
        }

        public IProgram TryGetChildByUid(int uid)
        {
            throw new NotImplementedException();
        }

        public IProgram TryGetChildByName(string name)
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

        public void Clear()
        {
            var removePrograms = _programs.ToList();

            _programs.Clear();

            foreach (var program in removePrograms)
            {
                program.Dispose();
            }

            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        internal void ScheduleProgram(IProgram program)
        {
            if (program != null && program.ParentCollection == this)
            {
                int oldIndex = _programs.IndexOf(program);
                int newIndex = _programs.Count - 1;

                if (oldIndex != newIndex)
                {
                    _programs.Remove(program);
                    _programs.Add(program);

                    CollectionChanged?.Invoke(this,
                        new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Move,
                            program,
                            newIndex,
                            oldIndex));
                }

            }

        }
    }
}
