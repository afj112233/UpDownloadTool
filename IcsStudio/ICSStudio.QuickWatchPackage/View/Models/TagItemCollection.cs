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
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Tags;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.QuickWatchPackage.View.Models
{
    internal abstract class TagItemCollection<T>
        : ObservableCollection<T>, ITagItemCollection where T : TagItem
    {
        public ITagCollectionContainer Scope { get; protected set; }

        public IController Controller => SimpleServices.Common.Controller.GetInstance();

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

            //foreach (var item in insertItems)
            //{
            //    item.ParentCollection = this;

            //    items.Insert(index, item);
            //    index++;
            //}
            items.InsertRange(startIndex,insertItems);
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
            if (item.HasChildren && item.Children != null)
            {
                RemoveTagItems(item.Children);
            }

            Remove(item);
        }


        public void RemoveTagItems(List<TagItem> listItem)
        {
            if (listItem == null) return;

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
            //foreach (var item in removeItems)
            //{
            //    items.Remove(item);
            //}
            int start = IndexOf(removeItems[0]);
            if (start == -1)
            {
                return;
            }

            items.RemoveRange(start, removeItems.Count);
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

        public void Refresh()
        {
            var defaultView = CollectionViewSource.GetDefaultView(this) as ListCollectionView;
            if (defaultView != null)
            {
                if (defaultView.IsAddingNew)
                    defaultView.CancelNew();

                if (defaultView.IsEditingItem)
                    defaultView.CancelEdit();
            }
            Items.Clear();
            OnPropertyChanged(EventArgsCache.CountPropertyChanged);
            OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);
            OnCollectionChanged(EventArgsCache.ResetCollectionChanged);
            foreach (var tag in Scope.Tags.ToList())
            {
                var item = TagToTagItem(tag, DataServer,this);

                if (item != null)
                {
                    AddTagItem(item);
                }

            }
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
                
                Items.Clear();
                OnPropertyChanged(EventArgsCache.CountPropertyChanged);
                OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);
                OnCollectionChanged(EventArgsCache.ResetCollectionChanged);
                //WeakEventManager<TagCollection, NotifyCollectionChangedEventArgs>.RemoveHandler(
                //    Scope.Tags as TagCollection, "CollectionChanged", TagCollectionOnCollectionChanged);

                Scope = null;
            }

            if (scope == null)
                return;

            foreach (var tag in scope.Tags.ToList())
            {
                var item = TagToTagItem(tag, DataServer,this);

                if (item != null)
                {
                    AddTagItem(item);
                }

            }

            Scope = scope;

            //WeakEventManager<TagCollection, NotifyCollectionChangedEventArgs>.AddHandler(Scope.Tags as TagCollection,
            //    "CollectionChanged", TagCollectionOnCollectionChanged);
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
        
        protected abstract T TagToTagItem(ITag tag, IDataServer dataServer, ITagItemCollection collection);

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