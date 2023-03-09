using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Windows;
using ICSStudio.Diagrams.Annotations;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Tags;

namespace ICSStudio.QuickWatchPackage.View.Models
{
    public enum TagItemType
    {
        Default,
        Integer,
        Struct,
        String,
        Array,

        BoolArrayMember,
        IntegerMember,
        BitMember
    }

    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    [SuppressMessage("ReSharper", "UseNameofExpression")]
    public abstract class TagItem : INotifyPropertyChanged
    {
        private string _dataType;
        private bool _isConstant;
        private bool _isExpanded;
        protected string _name;
        private Tag _tag;
        private Usage _usage;

        public virtual bool IsReadOnly { get; protected set; }

        public bool IsEnable => IsReadOnly;

        public string Scope { get; protected set; }

        public Tag Tag
        {
            get { return _tag; }
            protected set
            {
                if (_tag != null)
                    WeakEventManager<Tag, PropertyChangedEventArgs>.RemoveHandler(_tag, "PropertyChanged",
                        OnTagPropertyChanged);
                _tag = value;
                if (_tag != null)
                    WeakEventManager<Tag, PropertyChangedEventArgs>.AddHandler(_tag, "PropertyChanged",
                        OnTagPropertyChanged);
                Scope = _tag?.ParentCollection.ParentProgram?.Name ?? "Controller";
                OnPropertyChanged("Scope");
            }
        }

        public IField DataField { get; protected set; }
        public DataTypeInfo DataTypeInfo { get; protected set; }

        public virtual string Name
        {
            get { return _name; }
            set
            {
                if (!string.Equals(_name, value))
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

       
        public string Placeholder
        {
            get
            {
                if (ParentItem == null)
                    return string.Empty;

                return ParentItem.Placeholder + "  ";
            }
        }

        public bool HasChildren
        {
            get
            {
                if (string.IsNullOrEmpty(Name)) return false;
                if (ItemType == TagItemType.Array ||
                    ItemType == TagItemType.Integer ||
                    ItemType == TagItemType.String ||
                    ItemType == TagItemType.Struct)
                    return true;

                return false;
            }
        }

        public ITagItemCollection ParentCollection { get; set; }
        public TagItem ParentItem { get; set; }

        public IDataServer DataServer { get; set; }

        public List<TagItem> Children { get; protected set; }

        public TagItemType ItemType { get; protected set; }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded == value) return;

                _isExpanded = value;

                if (_isExpanded)
                {
                    // insert
                    if (ParentCollection != null)
                    {
                        var index = ParentCollection.IndexOf(this);

                        if (Children == null)
                            Children = CreateChildren(ParentCollection);

                        ParentCollection.InsertTagItems(index + 1, Children);
                    }
                }
                else
                {
                    if (Children != null)
                    {
                        foreach (var item in Children)
                        {
                            item.IsExpanded = false;
                        }

                        ParentCollection.RemoveTagItems(Children);
                    }
                }

                OnPropertyChanged();
            }
        }

        public string DataType
        {
            get { return _dataType; }
            protected set
            {
                if (!string.Equals(_dataType, value))
                {
                    _dataType = value;
                    OnPropertyChanged();
                }
            }
        }

        public virtual string Description { get; set; }

        public bool IsConstant
        {
            get
            {
                if (Tag != null) _isConstant = Tag.IsConstant;

                return _isConstant;
            }
            set
            {
                _isConstant = value;

                if (Tag != null)
                    Tag.IsConstant = value;

                OnPropertyChanged();
            }
        }

        public Visibility IsConstantVisibility
        {
            get
            {
                if (ParentItem == null)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public virtual DisplayStyle DisplayStyle { get; set; }

        public virtual Usage Usage
        {
            get { return _usage; }
            set
            {
                if (_usage == value) return;

                _usage = value;
                OnPropertyChanged();
            }
        }

        public Visibility UsageVisibility
        {
            get
            {
                if (ParentItem == null)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public int MemberIndex { get; protected set; }

        // for IntegerMember and BitMember
        public int BitOffset { get; protected set; }

        protected abstract List<TagItem> CreateChildren(ITagItemCollection collection);

        protected virtual void OnTagPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == Tag)
                if (e.PropertyName.Equals("Name"))
                {
                    var indexOfLeftBracket = Name.IndexOf('[');
                    var indexOfDot = Name.IndexOf('.');

                    var index = indexOfLeftBracket;
                    if (index < 0
                        || indexOfDot >= 0 && indexOfDot < index)
                        index = indexOfDot;

                    if (index < 0)
                        Name = Tag.Name;
                    else
                        Name = Tag.Name + Name.Remove(0, index);

                    OnPropertyChanged("Name");
                }
        }

        public void Cleanup()
        {
            Tag = null;
            ParentCollection = null;
        }

        public bool CanSetDescription { set; get; } = true;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TagItemComparer : IComparer<TagItem>, IComparer
    {
        private readonly bool _descending;
        private readonly bool _includeMember;

        public TagItemComparer(bool descending, bool includeMember)
        {
            _descending = descending;
            _includeMember = includeMember;
        }

        public int Compare(TagItem x, TagItem y)
        {
            Contract.Assert(x != null);
            Contract.Assert(y != null);

            int result;

            if (x == y)
                return 0;

            if (x.Tag != y.Tag)
            {
                result = string.Compare(x.Tag.Name, y.Tag.Name, StringComparison.OrdinalIgnoreCase);

                if (_descending)
                    return -result;

                return result;
            }

            var xPath = GetPath(x);
            var yPath = GetPath(y);

            int xPathCount = xPath.Count;
            int yPathCount = yPath.Count;

            Contract.Assert(xPathCount >= 1);
            Contract.Assert(yPathCount >= 1);

            if (xPathCount < yPathCount && xPath[xPathCount - 1] == yPath[xPathCount - 1])
                return -1;

            if (yPathCount < xPathCount && yPath[yPathCount - 1] == xPath[yPathCount - 1])
                return 1;

            //
            int minCount = Math.Min(xPathCount, yPathCount);

            var xItem = xPath[minCount - 1];
            var yItem = yPath[minCount - 1];
            while (xItem.ParentItem != yItem.ParentItem)
            {
                xItem = xItem.ParentItem;
                yItem = yItem.ParentItem;
            }

            var parentItem = xItem.ParentItem;
            if (parentItem.ItemType == TagItemType.Struct
                || parentItem.ItemType == TagItemType.String)
            {
                if (_includeMember)
                {
                    result = string.Compare(xItem.Name, yItem.Name, StringComparison.OrdinalIgnoreCase);
                    if (_descending)
                        return -result;

                    return result;
                }

                return xItem.MemberIndex - yItem.MemberIndex;
            }

            if (parentItem.ItemType == TagItemType.Integer)
            {
                if ((xItem.ItemType == TagItemType.IntegerMember
                     && yItem.ItemType == TagItemType.IntegerMember))
                {
                    return xItem.BitOffset - yItem.BitOffset;
                }
            }

            if (parentItem.ItemType == TagItemType.Array)
            {
                return xItem.MemberIndex - yItem.MemberIndex;
            }

            throw new NotImplementedException();
        }

        private List<TagItem> GetPath(TagItem item)
        {
            List<TagItem> path = new List<TagItem>();

            do
            {
                path.Add(item);
                item = item.ParentItem;

            } while (item != null);

            path.Reverse();

            return path;
        }

        public int Compare(object x, object y)
        {
            if (!(x is TagItem) && !(y is TagItem))
            {
                throw new ArgumentException("TagItemComparer can only sort TagItem objects.");
            }

            return Compare(x as TagItem, y as TagItem);
        }
    }
}