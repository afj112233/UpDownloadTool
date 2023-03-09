using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Tags;

namespace ICSStudio.QuickWatchPackage.View
{
    internal class MonitorTagCollection : ITagCollection
    {
        private readonly List<ITag> _tags;

        public MonitorTagCollection()
        {
            _tags = new List<ITag>();

            Uid = Guid.NewGuid().GetHashCode();
        }
        
        public IEnumerator<ITag> GetEnumerator() => _tags.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        public void Dispose()
        {
        }
        
        public int Uid { get; }
        //public event NotifyCollectionChangedEventHandler CollectionChanged;
        public int Count => _tags.Count;

        public ITag this[int uid]
        {
            get { throw new NotImplementedException(); }
        }

        public ITag this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                    return null;

                foreach (var tag in _tags)
                {
                    if (tag.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                        return tag;
                }

                return null;
            }
        }

        public ITag TryGetChildByUid(int uid)
        {
            throw new NotImplementedException();
        }

        public ITag TryGetChildByName(string name)
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

        public bool IsControllerScoped => false;

        public IProgramModule ParentProgram => null;
#pragma warning disable 67
        public event EventHandler<TagModifiedEventArgs> OnTagModified;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
#pragma warning restore
        public ITag GetDeviceTag(string name, bool deep)
        {
            throw new NotImplementedException();
        }

        public ITag TryGetDeviceTag(string name, bool deep)
        {
            throw new NotImplementedException();
        }

        public TagInfo GetTagInfo(string tagName, bool bAllowPrivateMemberReferences = false)
        {
            throw new NotImplementedException();
        }

        public void DeleteTag(ITag tag, bool allowDeleteIfReferenced, bool allowDeleteIfReadOnly,bool resetAoi)
        {
            if (tag == null) return;
            if (_tags.Contains(tag))
            {
                _tags.Remove(tag);
                
                CollectionChanged?.Invoke(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, tag));
            }
        }

        public IEnumerable<ITag> TrackedTags { get; set; }

        public IController ParentController
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void AddTag(ITag tag, bool isTmpAoi = false)
        {
            if (tag == null) return;
            if (!_tags.Contains(tag))
            {
                _tags.Add(tag);
                CollectionChanged?.Invoke(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, tag));
            }
        }

        public void Clear()
        {
            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, _tags));
            _tags.Clear();
        }
        
    }
}
