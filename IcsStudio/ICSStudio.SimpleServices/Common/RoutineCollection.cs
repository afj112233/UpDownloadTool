using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.SimpleServices.Common
{
    public class RoutineCollection : BaseCommon, IRoutineCollection
    {
        private readonly List<IRoutine> _routines;

        public RoutineCollection(IProgramModule parentProgram)
        {
            ParentProgram = parentProgram;
            _routines = new List<IRoutine>();
        }

        public IEnumerator<IRoutine> GetEnumerator() => _routines.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IProgramModule ParentProgram { get; }
        public IEnumerable<IRoutine> TrackedRoutines { get; set; }

        public void AddRoutine(IRoutine routine)
        {
            routine.ParentCollection = this;

            _routines.Add(routine);
            var aoi = ParentProgram as AoiDefinition;
            if (aoi==null)
                routine.PropertyChanged += Routine_PropertyChanged;
            else
            {
                aoi.AddScanRoutine(routine);
            }

            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, routine));
        }

        public void DeleteRoutine(IRoutine routine)
        {
            if (_routines.Contains(routine))
            {
                routine.IsAbandoned = true;
                routine.ParentCollection = null;
                routine.Dispose();

                _routines.Remove(routine);

                var aoi = ParentProgram as AoiDefinition;
                if (aoi == null)
                    routine.PropertyChanged -= Routine_PropertyChanged;
                else
                {
                    aoi.DelScanRoutine(routine);
                }

                CollectionChanged?.Invoke(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, routine));
            }
        }

        private void Routine_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                if (!(sender is IRoutine)) return;
                var r = sender as BaseComponent;
                if (r == null) return;
                foreach (var routine in this._routines)
                {
                    var stRoutine = routine as STRoutine;
                    if (stRoutine != null)
                    {
                        bool isChanged = false;
                        foreach (var variableInfo in stRoutine.GetCurrentVariableInfos(OnlineEditType.Original).OrderByDescending(v=>v.Offset))
                        {
                            if (variableInfo.IsRoutine &&
                                variableInfo.Name.Equals(r.OldName, StringComparison.OrdinalIgnoreCase))
                            {
                                var st = string.Join("\n", stRoutine.CodeText);

                                st = st.Remove(variableInfo.Offset, r.OldName.Length).Insert(variableInfo.Offset, r.Name);
                                stRoutine.CodeText = st.Split('\n').ToList();
                                isChanged = true;
                            }
                        }

                        if (stRoutine.PendingCodeText != null)
                        {
                            foreach (var variableInfo in stRoutine.GetCurrentVariableInfos(OnlineEditType.Pending).OrderByDescending(v => v.Offset))
                            {
                                if (variableInfo.IsRoutine &&
                                    variableInfo.Name.Equals(r.OldName, StringComparison.OrdinalIgnoreCase))
                                {
                                    var st = string.Join("\n", stRoutine.PendingCodeText);

                                    st = st.Remove(variableInfo.Offset, r.OldName.Length).Insert(variableInfo.Offset, r.Name);
                                    stRoutine.PendingCodeText = st.Split('\n').ToList();
                                    isChanged = true;
                                }
                            }
                        }

                        if (stRoutine.TestCodeText != null)
                        {
                            foreach (var variableInfo in stRoutine.GetCurrentVariableInfos(OnlineEditType.Test).OrderByDescending(v => v.Offset))
                            {
                                if (variableInfo.IsRoutine &&
                                    variableInfo.Name.Equals(r.OldName, StringComparison.OrdinalIgnoreCase))
                                {
                                    var st = string.Join("\n", stRoutine.TestCodeText);

                                    st = st.Remove(variableInfo.Offset, r.OldName.Length).Insert(variableInfo.Offset, r.Name);
                                    stRoutine.TestCodeText = st.Split('\n').ToList();
                                    isChanged = true;
                                }
                            }
                        }

                        if (isChanged)
                        {
                            stRoutine.IsUpdateChanged = true;
                        }
                    }
                }
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected override void DisposeAction()
        {
            var removeRoutines = _routines.ToList();

            _routines.Clear();
            
            //
            var aoi = ParentProgram as AoiDefinition;
            
            foreach (var routine in removeRoutines)
            {
                if (aoi == null)
                    routine.PropertyChanged -= Routine_PropertyChanged;

                routine.Dispose();
            }
        }

        public override IController ParentController => ParentProgram.ParentController;
        
        public int Count => _routines.Count;

        public IRoutine this[int uid]
        {
            get { throw new NotImplementedException(); }
        }

        public IRoutine this[string name]
        {
            get
            {
                foreach (var item in _routines)
                {
                    if (item.Name.Equals(name,StringComparison.OrdinalIgnoreCase))
                        return item;
                }

                //Debug.Assert(false);
                return null;
            }
        }

        public IRoutine TryGetChildByUid(int uid)
        {
            throw new NotImplementedException();
        }

        public IRoutine TryGetChildByName(string name)
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

        public void ReplaceLogic(IRoutine routine)
        {
            if (routine.Name.Equals("Logic", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var item in _routines)
                {
                    if (item.Name.Equals("Logic",StringComparison.OrdinalIgnoreCase))
                    {
                        _routines.Remove(item);
                        break;
                    }
                }

                _routines.Add(routine);
            }
        }

        public void RemoveAll()
        {
            for (int i = _routines.Count-1; i >=0; i--)
            {
                var routine = _routines[i];
                DeleteRoutine(routine);
            }
        }
    }
}
