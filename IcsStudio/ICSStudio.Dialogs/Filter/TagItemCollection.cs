using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ICSStudio.Dialogs.Filter
{
    public class TagItemCollection : LargeCollection<TagItem>
    {
        public override void AddRange(int index, List<TagItem> insertItems)
        {
            if (index > Count || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (insertItems == null || insertItems.Count == 0)
                return;

            CheckReentrancy();

            if (insertItems.Count == 1)
            {
                Insert(index, insertItems[0]);
                return;
            }

            List<TagItem> items = (List<TagItem>) Items;
            int startIndex = index;
            
            items.InsertRange(index,insertItems);
            OnPropertyChanged(EventArgsCache.CountPropertyChanged);
            OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add,
                    insertItems,
                    startIndex));
        }

        public override void RemoveTagItems(List<TagItem> listItem)
        {
            if (listItem.Count == 0)
                return;

            CheckReentrancy();

            if (listItem.Count == 1)
            {
                RemoveTagItem(listItem[0]);
                listItem[0].IsExtend = false;
                return;
            }

            List<TagItem> items = (List<TagItem>) Items;
            //foreach (var item in listItem)
            //{
            //    items.Remove(item);
            //    item.IsExtend = false;
            //}
            var index = Items.IndexOf(listItem[0]);
            if(index>-1)
            {
                items.RemoveRange(index, listItem.Count);
                OnPropertyChanged(EventArgsCache.CountPropertyChanged);
                OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);
                OnCollectionChanged(EventArgsCache.ResetCollectionChanged);
            }
        }
    }

    public class NameCollection : LargeCollection<string>
    {
        public object SyncRoot { get; }=new object();
        public override void AddRange(int index, List<string> insertItems)
        {
            lock (SyncRoot)
            {
                if (index > Count || index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index));

                if (insertItems == null || insertItems.Count == 0)
                    return;
                
                CheckReentrancy();

                if (insertItems.Count == 1)
                {
                    Insert(index, insertItems[0]);
                    return;
                }

                List<string> items = (List<string>) Items;
                int startIndex = index;

                //foreach (var item in insertItems)
                //{
                //    items.Insert(index, item);
                //    index++;
                //}
                if(index>-1)
                {
                    items.InsertRange(index, insertItems);
                    OnPropertyChanged(EventArgsCache.CountPropertyChanged);
                    OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);
                    OnCollectionChanged(
                        new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Add,
                            insertItems,
                            startIndex));
                }
                else
                {
                    Debug.Assert(false);
                }
            }
        }

        public override void RemoveTagItems(List<string> listItem)
        {
            if (listItem.Count == 0)
                return;

            CheckReentrancy();

            if (listItem.Count == 1)
            {
                RemoveTagItem(listItem[0]);
                return;
            }

            List<string> items = (List<string>) Items;
            //foreach (var item in listItem)
            //{
            //    items.Remove(item);
            //}
            var index = Items.IndexOf(listItem[0]);
            if(index>-1)
            {
                items.RemoveRange(Items.IndexOf(listItem[0]), listItem.Count);
                OnPropertyChanged(EventArgsCache.CountPropertyChanged);
                OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);
                OnCollectionChanged(EventArgsCache.ResetCollectionChanged);
            }
            else
            {
                Debug.Assert(false,listItem[0]);
            }
        }
    }

    public abstract class LargeCollection<T> : ObservableCollection<T>
    {
        public abstract void AddRange(int index, List<T> insertItems);

        public abstract void RemoveTagItems(List<T> listItem);

        public void RemoveTagItem(T item)
        {
            if (item == null)
                return;

            Remove(item);

        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            //TODO(gjc): add deferred event???

            try
            {
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
            catch (Exception)
            {
                Debug.Assert(false);
                //throw;
            }
        }

        protected bool IsRange(NotifyCollectionChangedEventArgs e) => e.NewItems?.Count > 1 || e.OldItems?.Count > 1;

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

    public static class EventArgsCache
    {
        public static readonly PropertyChangedEventArgs CountPropertyChanged = new PropertyChangedEventArgs("Count");

        public static readonly PropertyChangedEventArgs IndexerPropertyChanged =
            new PropertyChangedEventArgs("Item[]");

        public static readonly NotifyCollectionChangedEventArgs ResetCollectionChanged =
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
    }
}
