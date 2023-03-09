using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;

namespace ICSStudio.SimpleServices.DataType
{
    public class TypeMemberComponentCollection : ITypeMemberComponentCollection
    {
        private readonly List<IDataTypeMember> _dataTypeMembers;

        public TypeMemberComponentCollection()
        {
            _dataTypeMembers = new List<IDataTypeMember>();
            Uid = Guid.NewGuid().GetHashCode();
        }

        public IEnumerator<IDataTypeMember> GetEnumerator() => _dataTypeMembers.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private bool _isDisposed = false;
        public void Dispose()
        {
            if(_isDisposed)return;
            _dataTypeMembers.Clear();
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }

        public IController ParentController => null;
        public int Uid { get; }

#pragma warning disable 67
        public event NotifyCollectionChangedEventHandler CollectionChanged;
#pragma warning restore

        public int Count => _dataTypeMembers.Count;

        public IDataTypeMember this[int uid]
        {
            get
            {
                foreach (var dataTypeMember in _dataTypeMembers)
                {
                    if (dataTypeMember.DataTypeUid == uid)
                        return dataTypeMember;
                }

                return null;
            }
        }

        public IDataTypeMember this[string name]
        {
            get
            {
                foreach (var dataTypeMember in _dataTypeMembers)
                {
                    if (dataTypeMember.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                        return dataTypeMember;
                }

                //Debug.Assert(false);
                return null;
            }
        }

        public IDataTypeMember TryGetChildByUid(int uid)
        {
            throw new NotImplementedException();
        }

        public IDataTypeMember TryGetChildByName(string name)
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

        public void AddDataTypeMember(DataTypeMember dataTypeMember)
        {
            _dataTypeMembers.Add(dataTypeMember);

            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, dataTypeMember));
        }

        public void DataTypeMemberClear()
        {
            var removed = new List<IDataTypeMember>(_dataTypeMembers);

            _dataTypeMembers.Clear();

            if (removed.Count > 0)
                CollectionChanged?.Invoke(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed));
        }

        public IDataTypeMember FindDataType(int index)
        {
            var index1 = 0;
            if (_dataTypeMembers.Count <= index) return null;
            foreach (var dataTypeMember in _dataTypeMembers)
            {
                if (index1 == index) return dataTypeMember;
                index1++;
            }

            return null;
        }
    }
}
