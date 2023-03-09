using System.Collections.Generic;
using GalaSoft.MvvmLight;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.EditorPackage.MonitorEditTags.Models
{
    public sealed class ComboBoxTreeItem : ViewModelBase
    {
        private readonly string _name;
        private bool _isExpanded;
        private bool _isEnable = true;

        public ComboBoxTreeItem(string name, ITagCollectionContainer container = null)
        {
            _name = name;
            TagCollectionContainer = container;
            Children = new List<ComboBoxTreeItem>();
            _isExpanded = false;
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

        public string Name
        {
            get
            {
                var baseComponent = TagCollectionContainer as IBaseComponent;
                if (baseComponent != null)
                    return baseComponent.Name;

                return _name;
            }
        }

        public bool IsEnable
        {
            get { return _isEnable; }
            set
            {
                Set(ref _isEnable, value);
            }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;

                if (_isExpanded)
                {
                    // insert
                    if (HasChildren && ParentCollection != null)
                    {
                        var index = ParentCollection.IndexOf(this);
                        ParentCollection.InsertItems(index + 1, Children);
                    }
                }
                else
                {
                    // remove
                    if (HasChildren) ParentCollection?.RemoveItems(Children);
                }

                RaisePropertyChanged();
            }
        }

        public List<ComboBoxTreeItem> Children { get; }
        public ComboBoxTreeCollection ParentCollection { get; set; }
        public ComboBoxTreeItem ParentItem { get; private set; }

        public bool HasChildren
        {
            get
            {
                if (Children != null && Children.Count > 0)
                    return true;

                return false;
            }
        }

        public ITagCollectionContainer TagCollectionContainer { get; }

        public void AddChildren(ComboBoxTreeItem item)
        {
            if (!Children.Contains(item))
            {
                Children.Add(item);
                item.ParentItem = this;
            }
        }
    }
}