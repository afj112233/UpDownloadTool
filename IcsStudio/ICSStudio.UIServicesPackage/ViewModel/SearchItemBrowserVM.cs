using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Dialogs.Filter;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.PredefinedType;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    internal class SearchItemBrowserVM:ViewModelBase
    {
        private Controller _controller;
        private bool? _dialogResult;
        private string _selectedScope;
        private TagItem _selectedItem;
        private string _itemName;

        public SearchItemBrowserVM()
        {
            EscCommand=new RelayCommand(ExecuteEscCommand);
            OKCommand =new RelayCommand(ExecuteOKCommand,CanExecuteOKCommand);
            CancelCommand=new RelayCommand(ExecuteCancelCommand);
            ItemTypeCollection=new List<string>() { LanguageManager.GetInstance().ConvertSpecifier("Edit Zones")
                ,LanguageManager.GetInstance().ConvertSpecifier("Language Elements")
                ,"Tags"};
            SelectedItemType = ItemTypeCollection[2];
            ScopeCollection=new List<string>();
            _controller = Controller.GetInstance();
            ScopeCollection.Add($"{_controller.Name}(controller)");
            foreach (var program in _controller.Programs)
            {
                ScopeCollection.Add(program.Name);
            }

            SelectedScope = ScopeCollection[0];
        }

        public bool? DialogResult
        {
            set { Set(ref _dialogResult , value); }
            get { return _dialogResult; }
        }

        public string ItemName
        {
            set { Set(ref _itemName , value); }
            get { return _itemName; }
        }

        public List<string> ItemTypeCollection { set; get; }

        public string SelectedItemType { set; get; }

        public List<string> ScopeCollection { set; get; }

        public string SelectedScope
        {
            set
            {
                _selectedScope = value;
                if (ScopeCollection.IndexOf(_selectedScope)==0)
                {
                    var list = new List<TagItem>();
                    foreach (var tag in _controller.Tags)
                    {
                        var item = new TagItem(BaseTagItem);
                        item.Name = tag.Name;
                        item.DataType = tag.DataTypeInfo;
                        list.Add(item);
                    }

                    BaseTagItem.Children.AddRange(0, list);
                }
                else
                {
                    var program = _controller.Programs[_selectedScope];
                    if (program != null)
                    {
                        BaseTagItem.Children.Clear();
                        var list = new List<TagItem>();
                        foreach (var tag in program.Tags)
                        {
                            var item = new TagItem(BaseTagItem);
                            item.Name = tag.Name;
                            item.DataType = tag.DataTypeInfo;
                            list.Add(item);
                        }

                        BaseTagItem.Children.AddRange(0, list);
                    }
                }
            }
            get { return _selectedScope; }
        }

        public TagItem BaseTagItem { get; }=new TagItem(null);

        public TagItem SelectedItem
        {
            set
            {
                _selectedItem = value;
                ItemName = _selectedItem?.Name;
            }
            get { return _selectedItem; }
        }

        public RelayCommand EscCommand { get; }

        private void ExecuteEscCommand()
        {
            DialogResult = false;
        }

        public RelayCommand OKCommand { set; get; }

        private void ExecuteOKCommand()
        {
            DialogResult = true;
        }

        private bool CanExecuteOKCommand()
        {
            return true;
        }

        public RelayCommand CancelCommand { set; get; }

        private void ExecuteCancelCommand()
        {
            DialogResult = false;
        }
    }

    public class TagItem:ViewModelBase
    {
        private bool _isExpanded;

        public TagItem(TagItem baseTag)
        {
            BaseTagItem = baseTag;
        }
        
        public string Placeholder { set; get; }

        public TagItem BaseTagItem { get; }
        public string Name { set; get; }

        public DataTypeInfo DataType { set; get; }

        public string DataTypeName => DataType.ToString();

        public string Scope { set; get; }

        public Visibility ExpandVisibility
        {
            get
            {
                if (DataType.Dim1 > 0)
                {
                    return Visibility.Visible;
                }

                if (!DataType.DataType.IsBool)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public bool IsExpanded
        {
            set
            {
                _isExpanded = value;
                if (_isExpanded)
                {
                    GetChildren();
                    var index = BaseTagItem.Children.IndexOf(this);
                    BaseTagItem.Children.AddRange(index+1,Children.ToList());
                }
                else
                {
                    BaseTagItem.Children.RemoveTagItems(Children.ToList());
                }
            }
            get { return _isExpanded; }
        }

        public TagItemCollection Children { set; get; }=new TagItemCollection();

        private void GetChildren()
        {
            if (!Children.Any())
            {
                if (DataType.Dim1 > 0)
                {
                    GetArrayChildren();
                }else if (DataType.DataType.IsInteger)
                {
                    GetBitChildren();
                }else if (DataType.DataType is CompositiveType)
                {
                    GetCompositiveTypeChildren();
                }
            }
        }

        private void GetArrayChildren()
        {
            var children=new List<TagItem>();
            if (DataType.Dim3 > 0)
            {
                for (int i = 0; i < DataType.Dim3; i++)
                {
                    for (int j = 0; j < DataType.Dim2; j++)
                    {
                        for (int k = 0; k < DataType.Dim1; k++)
                        {
                            var child=new TagItem(BaseTagItem);
                            child.Placeholder = Placeholder + _placeholder;
                            child.Name = $"{Name}[{i},{j},{k}]";
                            child.DataType=new DataTypeInfo() {DataType = DataType.DataType};
                            children.Add(child);
                        }
                    }
                }
            }else if (DataType.Dim2 > 0)
            {
                for (int i = 0; i < DataType.Dim2; i++)
                {
                    for (int j = 0; j < DataType.Dim1; j++)
                    {
                        var child = new TagItem(BaseTagItem);
                        child.Placeholder = Placeholder + _placeholder;
                        child.Name = $"{Name}[{i},{j}]";
                        child.DataType = new DataTypeInfo() { DataType = DataType.DataType };
                        children.Add(child);
                    }
                }
            }
            else
            {
                for (int i = 0; i < DataType.Dim1; i++)
                {
                    var child = new TagItem(BaseTagItem);
                    child.Placeholder = Placeholder + _placeholder;
                    child.Name = $"{Name}[{i}]";
                    child.DataType = new DataTypeInfo() { DataType = DataType.DataType };
                    children.Add(child);
                }
            }
            Children.AddRange(0,children);
        }

        private void GetBitChildren()
        {
            var children = new List<TagItem>();
            for (int i = 0; i < DataType.DataType.BitSize; i++)
            {
                var child = new TagItem(BaseTagItem);
                child.Placeholder = Placeholder + _placeholder;
                child.Name = $"{Name}.{i}";
                child.DataType = new DataTypeInfo() { DataType = BOOL.Inst };
                children.Add(child);
            }
            Children.AddRange(0,children);
        }

        private void GetCompositiveTypeChildren()
        {
            var children = new List<TagItem>();
            var compositiveType = DataType.DataType as CompositiveType;
            foreach (var member in compositiveType.TypeMembers)
            {
                var child = new TagItem(BaseTagItem);
                child.Placeholder = Placeholder + _placeholder;
                child.Name = $"{Name}.{member.Name}";
                child.DataType = member.DataTypeInfo;
                children.Add(child);
            }
            Children.AddRange(0, children);
        }

        private const string _placeholder = "          ";
    }


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

            List<TagItem> items = (List<TagItem>)Items;
            int startIndex = index;

            //foreach (var item in insertItems)
            //{
            //    items.Insert(index, item);
            //    index++;
            //}
            items.InsertRange(index, insertItems);
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
                listItem[0].IsExpanded = false;
                return;
            }

            List<TagItem> items = (List<TagItem>)Items;
            //foreach (var item in listItem)
            //{
            //    items.Remove(item);
            //    item.IsExtend = false;
            //}
            items.RemoveRange(Items.IndexOf(listItem[0]), listItem.Count);
            OnPropertyChanged(EventArgsCache.CountPropertyChanged);
            OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);
            OnCollectionChanged(EventArgsCache.ResetCollectionChanged);
        }
    }
}
