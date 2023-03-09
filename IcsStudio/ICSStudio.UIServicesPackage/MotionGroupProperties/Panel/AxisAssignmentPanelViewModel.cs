using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Tags;
using Microsoft.VisualStudio.Shell;
using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;

namespace ICSStudio.UIServicesPackage.MotionGroupProperties.Panel
{
    class AxisAssignmentPanelViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private bool _isDirty;
        private readonly Controller _controller;
        private List<Item> _assignedList;
        private List<Item> _unassignedList;
        private int _axisCount = 0;
        
        public AxisAssignmentPanelViewModel(AxisAssignmentPanel panel, ITag motionGroup)
        {
            _controller = motionGroup.ParentController as Controller;
            Control = panel;
            panel.DataContext = this;
            SelectedItemsChangedCommand=new RelayCommand<SelectionChangedEventArgs>(ExecuteSelectedItemsChangedCommand);
            UnassignedSelectedItemsChangedCommand=new RelayCommand<SelectionChangedEventArgs>(ExecuteUnassignedSelectedItemsChangedCommand);
            AddCommand = new RelayCommand(Add, CanExecuteCommand);
            RemoveCommand = new RelayCommand(Remove, CanExecuteCommand);
            AssignedCollection = new ObservableCollection<Item>();
            UnassignedCollection = new ObservableCollection<Item>();
            AssignedCollection2 = new List<Item>();
            UnassignedCollection2 = new List<Item>();
            MotionGroup = motionGroup;
            GetAxis();

            TagCollection tagCollection = _controller?.Tags as TagCollection;
            if (tagCollection != null)
                tagCollection.CollectionChanged += OnCollectionChanged;
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                _controller, "IsOnlineChanged", OnIsOnlineChanged);
        }

        public override void Cleanup()
        {
            TagCollection tagCollection = _controller?.Tags as TagCollection;
            if (tagCollection != null)
                tagCollection.CollectionChanged -= OnCollectionChanged;
            var axisCollection = _controller.Tags.Where(t =>
                (t as Tag).DataWrapper is AxisCIPDrive || (t as Tag).DataWrapper is AxisVirtual);
            foreach (Tag tag in axisCollection)
            {
                tag.PropertyChanged -= OnPropertyChanged;
            }

            WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                _controller, "IsOnlineChanged", OnIsOnlineChanged);
        }

        public void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                ITag tag = null;
                int count = 0;
                foreach (var item in (TagCollection) sender)
                {
                    if (count == _axisCount)
                    {
                        tag = item;
                    }
                    else
                    {
                        count++;
                    }
                }

                if (tag != null)
                {
                    var tagItem=new Item(tag);
                    Tag tag2 = (Tag) tag;
                    AxisVirtual axisVirtual = tag2.DataWrapper as AxisVirtual;
                    AxisCIPDrive axisCIPDrive = tag2.DataWrapper as AxisCIPDrive;
                    if (axisVirtual?.AssignedGroup != null || axisCIPDrive?.AssignedGroup != null)
                    {
                        AssignedCollection.Add(tagItem);
                        AssignedCollection = new ObservableCollection<Item>(AssignedCollection.OrderBy(x => x.Tag.Name));
                        AssignedCollection2.Add(tagItem);
                        AssignedCollection2.Sort((x, y) =>
                            string.Compare(x.Tag.Name, y.Tag.Name, StringComparison.OrdinalIgnoreCase));
                        RaisePropertyChanged("AssignedCollection");
                        Compare();
                    }
                    else
                    {
                        UnassignedCollection.Add(tagItem);
                        UnassignedCollection =
                            new ObservableCollection<Item>(UnassignedCollection.OrderBy(x => x.Tag.Name));
                        UnassignedCollection2.Add(tagItem);
                        UnassignedCollection2.Sort((x, y) =>
                            string.Compare(x.Tag.Name, y.Tag.Name, StringComparison.OrdinalIgnoreCase));
                        RaisePropertyChanged("UnassignedCollection");
                        Compare();

                    }

                    _axisCount++;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                ITag tag = (Tag) e.OldItems[0];
                var item = AssignedCollection.FirstOrDefault(t => t.Tag==tag);
                AssignedCollection.Remove(item);
                 item = AssignedCollection2.FirstOrDefault(t => t.Tag == tag);
                AssignedCollection2.Remove(item);
                item = UnassignedCollection.FirstOrDefault(t => t.Tag == tag);
                UnassignedCollection.Remove(item);
                item = UnassignedCollection2.FirstOrDefault(t => t.Tag == tag);
                UnassignedCollection2.Remove(item);
                Compare();
                _axisCount--;
            }
        }

        public void Compare()
        {
            _assignedList = new List<Item>();
            foreach (var item in AssignedCollection)
            {
                _assignedList.Add(item);
            }

            _unassignedList = new List<Item>();
            foreach (var item in UnassignedCollection)
            {
                _unassignedList.Add(item);
            }

            _assignedList.Sort((x, y) => string.Compare(x.Tag.Name, y.Tag.Name, StringComparison.OrdinalIgnoreCase));
            _unassignedList.Sort((x, y) => string.Compare(x.Tag.Name, y.Tag.Name, StringComparison.OrdinalIgnoreCase));
            if (_unassignedList.Except(UnassignedCollection2).ToList().Count == 0 &&
                _assignedList.Except(AssignedCollection2).ToList().Count == 0) IsDirty = false;
            else IsDirty = true;
        }

        //设置sign和unsigned
        public void GetAxis([CallerMemberName] string funName = null)
        {
            List<string> axisNameList = new List<string>();
            AssignedCollection2.Clear();
            UnassignedCollection2.Clear();
            AssignedCollection.Clear();
            UnassignedCollection.Clear();
            ITag it = null;
            foreach (ITag tag in _controller.Tags)
            {
                if (tag.DataTypeInfo.DataType.IsMotionGroupType)
                {
                    it = tag;
                    break;
                }
            }

            foreach (var tag in _controller.Tags)
            {
                if (tag.DataTypeInfo.DataType.IsAxisType)
                    axisNameList.Add(tag.Name);
                if (funName == ".ctor") _axisCount++;
            }

            foreach (var axisName in axisNameList)
            {
                Tag axis = _controller.Tags[axisName] as Tag;
                var item=new Item(axis);
                AxisCIPDrive axisCIPDrive = axis?.DataWrapper as AxisCIPDrive;
                if (axisCIPDrive != null)
                {
                    axisCIPDrive.ParentTag.PropertyChanged += OnPropertyChanged;
                    if (axisCIPDrive.AssignedGroup == it)
                    {
                        AssignedCollection2.Add(item);
                    }
                    else
                    {
                        UnassignedCollection2.Add(item);
                    }
                }


                //TODO(gjc): add AxisVirtual
                AxisVirtual axisVirtual = axis?.DataWrapper as AxisVirtual;
                if (axisVirtual != null)
                {
                    axisVirtual.ParentTag.PropertyChanged += OnPropertyChanged;
                    if (axisVirtual.AssignedGroup == it)
                    {
                        AssignedCollection2.Add(item);
                    }
                    else
                    {
                        UnassignedCollection2.Add(item);
                    }
                }
            }

            AssignedCollection2.Sort((x, y) =>
                string.Compare(x.Tag.Name, y.Tag.Name, StringComparison.OrdinalIgnoreCase));
            UnassignedCollection2.Sort((x, y) =>
                string.Compare(x.Tag.Name, y.Tag.Name, StringComparison.OrdinalIgnoreCase));
            foreach (var item in AssignedCollection2)
            {
                AssignedCollection.Add(item);
            }

            foreach (var item in UnassignedCollection2)
            {
                UnassignedCollection.Add(item);
            }

            AssignedCollection = new ObservableCollection<Item>(AssignedCollection.OrderBy(x => x.Tag.Name));
            UnassignedCollection = new ObservableCollection<Item>(UnassignedCollection.OrderBy(x => x.Tag.Name));
            RaisePropertyChanged("UnassignedCollection");
            RaisePropertyChanged("AssignedCollection");
        }

        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
                GetAxis();
        }

        public object Owner { get; set; }
        public object Control { get; }

        public void LoadOptions()
        {

        }

        public bool SaveOptions()
        {

            return true;
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    IsDirtyChanged?.Invoke(this, EventArgs.Empty);
                }

                Set(ref _isDirty, value);
            }
        }

        public event EventHandler IsDirtyChanged;

        public ITag MotionGroup { get; }

        public List<Item> UnassignedCollection2;
        public ObservableCollection<Item> UnassignedCollection { set; get; }
        //public Item UnassignedSelectedItem { set; get; }

        public List<Item> AssignedCollection2;
        public ObservableCollection<Item> AssignedCollection { set; get; }

        private List<Item> _selectedTags=new List<Item>(); 
        private List<Item> _selectedUnassignedTags=new List<Item>(); 

        //public Item AssignedSelectedItem { set; get; }

        public RelayCommand AddCommand { set; get; }
        public RelayCommand RemoveCommand { set; get; }

        public RelayCommand<SelectionChangedEventArgs> SelectedItemsChangedCommand { set; get; }
        public RelayCommand<SelectionChangedEventArgs> UnassignedSelectedItemsChangedCommand { set; get; }

        private void ExecuteSelectedItemsChangedCommand(SelectionChangedEventArgs e)
        {
            if (e?.RemovedItems != null)
            {
                foreach (Item removedItem in e.RemovedItems)
                {
                    _selectedTags.Remove(removedItem);
                }
            }

            if (e?.AddedItems != null)
            {
                foreach (Item addedItem in e.AddedItems)
                {
                    _selectedTags.Add(addedItem);
                }
            }
        }

        private void ExecuteUnassignedSelectedItemsChangedCommand(SelectionChangedEventArgs e)
        {
            if (e?.RemovedItems != null)
            {
                foreach (Item removedItem in e.RemovedItems)
                {
                    _selectedUnassignedTags.Remove(removedItem);
                }
            }

            if (e?.AddedItems != null)
            {
                foreach (Item addedItem in e.AddedItems)
                {
                    _selectedUnassignedTags.Add(addedItem);
                }
            }
        }

        private void Add()
        {
            if (!_selectedUnassignedTags.Any()) return;
            var tmp=new Item[_selectedUnassignedTags.Count];
            _selectedUnassignedTags.CopyTo(tmp);
            foreach (var item in tmp)
            {
                AssignedCollection.Add(item);
            }

            foreach (Item item in tmp)
            {
                UnassignedCollection.Remove(item);
            }
            
            AssignedCollection = new ObservableCollection<Item>(AssignedCollection.OrderBy(x => x.Tag.Name));
            UnassignedCollection = new ObservableCollection<Item>(UnassignedCollection.OrderBy(x => x.Tag.Name));
            RaisePropertyChanged("UnassignedCollection");
            RaisePropertyChanged("AssignedCollection");
            Compare();
        }

        private bool CanExecuteCommand()
        {
            return !_controller.IsOnline;
        }

        private void Remove()
        {
            if (!_selectedTags.Any()) return;
            var tmp=new Item[_selectedTags.Count];
            _selectedTags.CopyTo(tmp);
            foreach (var item in tmp)
            {
                UnassignedCollection.Add(item);
            }

            foreach (var item in tmp)
            {
                AssignedCollection.Remove(item);
            }
           
            AssignedCollection = new ObservableCollection<Item>(AssignedCollection.OrderBy(x => x.Tag.Name));
            UnassignedCollection = new ObservableCollection<Item>(UnassignedCollection.OrderBy(x => x.Tag.Name));
            RaisePropertyChanged("UnassignedCollection");
            RaisePropertyChanged("AssignedCollection");
            Compare();
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                
                AddCommand.RaiseCanExecuteChanged();
                RemoveCommand.RaiseCanExecuteChanged();
            });
        }

    }
    
    internal class Item:ViewModelBase
    {
        private bool _isSelected;

        public Item(ITag tag)
        {
            Tag = tag;
        }

        public ITag Tag { get; }

        public bool IsSelected
        {
            set { Set(ref _isSelected , value); }
            get { return _isSelected; }
        }
    }
}
