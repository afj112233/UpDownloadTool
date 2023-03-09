using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using ICSStudio.AvalonEdit.CodeCompletion;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Tags;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.EditorPackage.MonitorEditTags.Models
{
    internal enum FilterOnType
    {
        FilterOnName,
        FilterOnDescription,
        Both
    }
    internal abstract class TagItemCollection<T>
        : ObservableCollection<T>, ITagItemCollection where T : TagItem
    {
        public AoiDataReference DataContext { get; protected set; }

        public abstract void UpdateDataContext(AoiDataReference dataContext);
        public ITagCollectionContainer Scope { get; protected set; }

        public IController Controller => Scope?.Tags.ParentController;

        public IDataServer DataServer { get; set; }

        public void InsertTagItems(int index, List<TagItem> listItem)
        {
            var insertItems = listItem.Select(item => item as T).ToList();

            InsertTagItems(index, insertItems);
        }



        public void InsertTagItems(int index, List<T> insertItems)
        {
            if (index > Count || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (insertItems == null || insertItems.Count == 0)
                return;

            CheckReentrancy();

            if (insertItems.Count == 1)
            {
                insertItems[0].ParentCollection = this;
                Insert(index, insertItems[0]);
                return;
            }

            List<T> items = (List<T>) Items;
            int startIndex = index;

            foreach (var item in insertItems)
            {
                item.ParentCollection = this;

                items.Insert(index, item);
                index++;
            }

            OnPropertyChanged(EventArgsCache.CountPropertyChanged);
            OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add,
                    insertItems,
                    startIndex));
        }

        public void RemoveTagItem(T item)
        {
            if (item == null)
                return;

            Remove(item);

        }


        public void RemoveTagItems(List<TagItem> listItem)
        {
            var removeItems = listItem.Select(item => item as T).ToList();

            if (removeItems.Count == 0)
                return;

            CheckReentrancy();

            if (removeItems.Count == 1)
            {
                RemoveTagItem(removeItems[0]);
                return;
            }

            List<T> items = (List<T>) Items;
            foreach (var item in removeItems)
            {
                items.Remove(item);
            }

            OnPropertyChanged(EventArgsCache.CountPropertyChanged);
            OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);
            OnCollectionChanged(EventArgsCache.ResetCollectionChanged);
        }

        public void RemoveItemsByTag(ITag tag)
        {
            CheckReentrancy();

            List<T> items = (List<T>) Items;

            var removeItems = new List<T>();
            foreach (var item in items)
            {
                if (item.Tag == tag)
                    removeItems.Add(item);
            }

            foreach (var item in removeItems)
            {
                items.Remove(item);
                item.Cleanup();
            }

            OnPropertyChanged(EventArgsCache.CountPropertyChanged);
            OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);
            OnCollectionChanged(EventArgsCache.ResetCollectionChanged);

        }

        public int IndexOf(TagItem item)
        {
            return base.IndexOf(item as T);
        }

        public void AddTagItem(T item)
        {
            if (item == null)
                return;

            item.ParentCollection = this;
            Add(item);
        }

        public void AddTagItems(List<T> listItem)
        {
            if (listItem == null || listItem.Count == 0)
                return;

            CheckReentrancy();

            if (listItem.Count == 1)
            {
                AddTagItem(listItem[0]);
                return;
            }

            //
            List<T> items = (List<T>) Items;
            foreach (var item in listItem)
            {
                item.ParentCollection = this;
                items.Add(item);
            }

            OnPropertyChanged(EventArgsCache.CountPropertyChanged);
            OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, listItem));
        }

        public void Update(ITagCollectionContainer scope)
        {
            if (Scope == scope)
                return;

            var defaultView = CollectionViewSource.GetDefaultView(this) as ListCollectionView;
            if (defaultView != null)
            {
                if (defaultView.IsAddingNew)
                    defaultView.CancelNew();

                if (defaultView.IsEditingItem)
                    defaultView.CancelEdit();
            }

            if (Scope != null)
            {
                //foreach (var tag in Scope.Tags)
                //{
                //    RemoveItemsByTag(tag);
                //}
                Clear();

                WeakEventManager<TagCollection, NotifyCollectionChangedEventArgs>.RemoveHandler(
                    Scope.Tags as TagCollection, "CollectionChanged", TagCollectionOnCollectionChanged);

                Scope = null;
            }

            if (scope == null)
                return;

            //sort
            var tagList = scope.Tags.ToList();
            tagList.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));

            foreach (var tag in tagList)
            {
                var item = TagToTagItem(tag, DataServer);

                if (item != null)
                {
                    AddTagItem(item);
                }

            }

            Scope = scope;

            WeakEventManager<TagCollection, NotifyCollectionChangedEventArgs>.AddHandler(Scope.Tags as TagCollection,
                "CollectionChanged", TagCollectionOnCollectionChanged);
        }

        private string _filterInfo;
        private string _filterName;
        private FilterOnType _filterOnType;
        internal void SetFilterInfo(string filterInfo, string filterName,FilterOnType filterOnType)
        {
            _filterInfo = filterInfo;
            _filterName = filterName;
            _filterOnType = filterOnType;

            Clear();
            var tagList = Scope.Tags.Where(IsMatch).ToList();
            tagList.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));

            foreach (var tag in tagList)
            {
                var item = TagToTagItem(tag, DataServer);

                if (item != null)
                {
                    AddTagItem(item);
                }
            }
        }

        private bool IsMatch(ITag tag)
        {
            if ("All Variables".Equals(_filterInfo) && string.IsNullOrEmpty(_filterName)) return true;
            if(!string.IsNullOrEmpty(_filterName))
            {
                var isMatched = false;
                if(_filterOnType==FilterOnType.FilterOnName||_filterOnType==FilterOnType.Both)
                    if (tag.Name.ToLower().Contains(_filterName.ToLower()))
                    {
                        isMatched = true;
                    }

                if (_filterOnType == FilterOnType.FilterOnName&&!isMatched)
                {
                    return false;
                }

                if (!isMatched)
                {
                    if (_filterOnType == FilterOnType.FilterOnDescription || _filterOnType == FilterOnType.Both)
                    {
                        var description = Tag.GetChildDescription(tag.Description, tag.DataTypeInfo,
                            ((Tag)tag).ChildDescription, Tag.GetOperand(tag.Name));
                        if (description?.Contains(_filterName)??false)
                        {
                            isMatched = true;
                        }
                    }
                }

                if (!isMatched) return false;
            }

            if ("All Variables".Equals(_filterInfo)) return true;
            if (_filterInfo == null) return true;
            var types = _filterInfo.Split(',').ToList();
            if (types.Count > 0)
            {
                var first = types[0];
                switch (first)
                {
                    case "Input":
                        if (tag.Usage != Usage.Input)
                            return false;
                        types.RemoveAt(0);
                        break;
                    case "Output":
                        if (tag.Usage != Usage.Output)
                            return false;
                        types.RemoveAt(0);
                        break;
                    case "InOut":
                        if (tag.Usage != Usage.InOut)
                            return false;
                        types.RemoveAt(0);
                        break;
                    case "Local":
                        if (tag.Usage != Usage.Local)
                            return false;
                        types.RemoveAt(0);
                        break;
                    case "Produced":
                        if (!tag.IsProducing)
                            return false;
                        types.RemoveAt(0);
                        break;
                    case "Consumed":
                        if (!tag.IsConsuming)
                            return false;
                        types.RemoveAt(0);
                        break;
                    case "Alias":
                        if (!tag.IsAlias)
                            return false;
                        types.RemoveAt(0);
                        break;
                }
            }

            if (types.Count > 0 && !types.Contains(tag.DataTypeInfo.DataType.Name)) return false;

            return true;
        }

        public T GetTagItemAndExpandByName(string tagName)
        {
            T resultItem = null;
            foreach (var item in Items)
            {
                if (item.Name.Length <= tagName.Length && tagName.StartsWith(item.Name))
                {
                    if (resultItem == null)
                        resultItem = item;
                    else if (item.Name.Length > resultItem.Name.Length)
                        resultItem = item;

                }
            }

            if (resultItem == null)
                return null;

            while (true)
            {
                Contract.Assert(resultItem != null);

                if (resultItem.Name.Length == tagName.Length)
                    return resultItem;

                resultItem.IsExpanded = true;

                if (!resultItem.HasChildren)
                    break;

                T searchItem = resultItem;

                foreach (var item in resultItem.Children)
                {
                    if (item.Name.Length <= tagName.Length && tagName.StartsWith(item.Name))
                    {
                        Contract.Assert(searchItem != null);
                        if (item.Name.Length > searchItem.Name.Length)
                            searchItem = item as T;
                    }
                }

                if (searchItem == resultItem)
                    break;

                resultItem = searchItem;
            }

            return resultItem;
        }

        private void TagCollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(
                async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    if (sender == Scope?.Tags)
                    {
                        if (e.Action == NotifyCollectionChangedAction.Add)
                        {
                            var defaultView = CollectionViewSource.GetDefaultView(this) as IEditableCollectionView;
                            if (defaultView != null)
                            {
                                if (defaultView.IsAddingNew)
                                    return;
                            }


                            List<T> addList = new List<T>();

                            foreach (var item in e.NewItems)
                            {
                                var tagItem = TagToTagItem(item as ITag, DataServer);
                                addList.Add(tagItem);
                            }

                            AddTagItems(addList);

                        }

                        if (e.Action == NotifyCollectionChangedAction.Remove)
                        {
                            foreach (ITag tag in e.OldItems)
                            {
                                RemoveItemsByTag(tag);
                            }
                        }
                    }
                });
        }



        protected abstract T TagToTagItem(ITag tag, IDataServer dataServer);

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            //TODO(gjc): add deferred event???

            foreach (var handler in GetHandlers())
            {
                if (IsRange(args))
                {
                    ListCollectionView cv = handler.Target as ListCollectionView;
                    if (cv != null)
                    {
                        if (!cv.IsAddingNew && !cv.IsEditingItem)
                        {
                            cv.Refresh();
                        }
                    }

                }
                else
                {
                    handler(this, args);
                }

            }
        }

        private bool IsRange(NotifyCollectionChangedEventArgs e) => e.NewItems?.Count > 1 || e.OldItems?.Count > 1;

        private IEnumerable<NotifyCollectionChangedEventHandler> GetHandlers()
        {
            var info = typeof(ObservableCollection<T>).GetField(nameof(CollectionChanged),
                BindingFlags.Instance | BindingFlags.NonPublic);
            var @event = (MulticastDelegate) info?.GetValue(this);
            return @event?.GetInvocationList()
                       .Cast<NotifyCollectionChangedEventHandler>()
                       .Distinct()
                   ?? Enumerable.Empty<NotifyCollectionChangedEventHandler>();
        }

    }

    internal static class EventArgsCache
    {
        internal static readonly PropertyChangedEventArgs CountPropertyChanged = new PropertyChangedEventArgs("Count");

        internal static readonly PropertyChangedEventArgs IndexerPropertyChanged =
            new PropertyChangedEventArgs("Item[]");

        internal static readonly NotifyCollectionChangedEventArgs ResetCollectionChanged =
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
    }
}