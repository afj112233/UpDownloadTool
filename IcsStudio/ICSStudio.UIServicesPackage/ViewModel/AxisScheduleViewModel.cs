using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.Objects;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Tags;
using Imagin.Common.Extensions;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    class AxisScheduleViewModel : ViewModelBase
    {
        private readonly MotionGroup _mg;
        private readonly IController _controller;
        private bool _isDirty;
        private int _count;
        private float _baseUpdate;
        private float _alternate2Update;
        private float _alternate1Update;
        private int _axisCount = 0;

        public AxisScheduleViewModel(ITag motionGroup)
        {
            MotionGroup = motionGroup;
            _controller = motionGroup.ParentController;
            BaseAxisCollectionSource = new List<ITag>();
            Alternate1AxisCollectionSource = new List<ITag>();
            Alternate2AxisCollectionSource = new List<ITag>();

            BaseAxisCollection = new ObservableCollection<ITag>();
            Alternate1AxisCollection = new ObservableCollection<ITag>();
            Alternate2AxisCollection = new ObservableCollection<ITag>();
            OkCommand = new RelayCommand(ExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            ApplyCommand = new RelayCommand(ExecuteApplyCommand, CanExecuteApplyCommand);
            HelpCommand = new RelayCommand(ExecuteHelpCommand);

            Tag tag = motionGroup as Tag;
            _mg = tag?.DataWrapper as MotionGroup;
            SetBA1A2();
            SetList();
            Enabled = !_controller.IsOnline;
            BaseToAlternate1 = new RelayCommand(B2A1, CanExecuteCommand);
            Alternate1ToBase = new RelayCommand(A12B, CanExecuteCommand);
            Alternate1ToAlternate2 = new RelayCommand(A12A2, CanExecuteCommand);
            Alternate2ToAlternate1 = new RelayCommand(A22A1, CanExecuteCommand);
            BaseSelectionChanged=new RelayCommand<SelectionChangedEventArgs>(ExecuteBaseSelectionChanged);
            Alternate1SelectionChanged=new RelayCommand<SelectionChangedEventArgs>(ExecuteAlternate1SelectionChanged);
            Alternate2SelectionChanged=new RelayCommand<SelectionChangedEventArgs>(ExecuteAlternate2SelectionChanged);
            

            PropertyChangedEventManager.AddHandler(motionGroup, MotionGroup_PropertyChanged,"");
            CollectionChangedEventManager.AddHandler(_controller.Tags, OnCollectionChanged);
            //(_controller.Tags as TagCollection).CollectionChanged += OnCollectionChanged;

            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                (Controller) _controller, "IsOnlineChanged", OnIsOnlineChanged);
        }

        public bool Enabled
        {
            set { Set(ref _enabled, value); }
            get { return _enabled; }
        }

        public override void Cleanup()
        {
            //(_controller.Tags as TagCollection).CollectionChanged -= OnCollectionChanged;
            CollectionChangedEventManager.RemoveHandler(_controller.Tags, OnCollectionChanged);
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                (Controller) _controller, "IsOnlineChanged", OnIsOnlineChanged);
            //_mg.ParentTag.PropertyChanged -= MotionGroup_PropertyChanged;
            PropertyChangedEventManager.RemoveHandler(MotionGroup, MotionGroup_PropertyChanged, "");
        }

        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AssignedGroup" || e.PropertyName == "Name")
            {
                ITag tag = (ITag) sender;
                if (BaseAxisCollectionSource.Contains(tag))
                {

                    if (!BaseAxisCollection.Contains(tag))
                    {
                        BaseAxisCollection.Add(tag);
                        BaseAxisCollection = new ObservableCollection<ITag>(BaseAxisCollection.OrderBy(x => x.Name));
                        RaisePropertyChanged("BaseAxisCollection");
                    }

                    if (Alternate1AxisCollection.Contains(tag))
                    {
                        Alternate1AxisCollection.Remove(tag);
                        RaisePropertyChanged("Alternate1AxisCollection");
                    }

                    if (Alternate2AxisCollection.Contains(tag))
                    {
                        Alternate2AxisCollection.Remove(tag);
                        RaisePropertyChanged("Alternate2AxisCollection");
                    }

                }
                else if (Alternate1AxisCollectionSource.Contains(tag))
                {
                    if (!Alternate1AxisCollection.Contains(tag))
                    {
                        Alternate1AxisCollection.Add(tag);
                        Alternate1AxisCollection =
                            new ObservableCollection<ITag>(Alternate1AxisCollection.OrderBy(x => x.Name));
                        RaisePropertyChanged("Alternate1AxisCollection");
                    }

                    if (BaseAxisCollection.Contains(tag))
                    {
                        BaseAxisCollection.Remove(tag);
                        RaisePropertyChanged("BaseAxisCollection");
                    }

                    if (Alternate2AxisCollection.Contains(tag))
                    {
                        Alternate2AxisCollection.Remove(tag);
                        RaisePropertyChanged("Alternate2AxisCollection");
                    }
                }
                else
                {
                    if (!Alternate2AxisCollection.Contains(tag))
                    {
                        Alternate2AxisCollection.Add(tag);
                        Alternate2AxisCollection =
                            new ObservableCollection<ITag>(Alternate2AxisCollection.OrderBy(x => x.Name));
                        RaisePropertyChanged("Alternate2AxisCollection");
                    }

                    if (BaseAxisCollection.Contains(tag))
                    {
                        BaseAxisCollection.Remove(tag);
                        RaisePropertyChanged("BaseAxisCollection");
                    }

                    if (Alternate1AxisCollection.Contains(tag))
                    {
                        Alternate1AxisCollection.Remove(tag);
                        RaisePropertyChanged("Alternate1AxisCollection");
                    }
                }

                Compare();
            }
        }


        public void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems != null)
                {
                    AddNewAxis();
                    Compare();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems != null&&e.OldItems.Count>0)
                {
                    ITag tag = (ITag)e.OldItems[0];
                    if (tag != null)
                    {
                        AxisVirtual axisVirtual = ((Tag)tag).DataWrapper as AxisVirtual;
                        AxisCIPDrive axisCIPDrive = ((Tag)tag).DataWrapper as AxisCIPDrive;
                        if ((axisVirtual != null && axisVirtual.AssignedGroup == _mg.ParentTag) ||
                            (axisCIPDrive != null && axisCIPDrive.AssignedGroup == _mg.ParentTag))
                        {
                            BaseAxisCollectionSource.Remove(tag);
                            BaseAxisCollection.Remove(tag);
                            Alternate1AxisCollectionSource.Remove(tag);
                            Alternate1AxisCollection.Remove(tag);
                            Alternate2AxisCollectionSource.Remove(tag);
                            Alternate2AxisCollection.Remove(tag);
                        }
                    }

                    Compare();
                    _axisCount--;
                }
            }
        }

        public void AddNewAxis()
        {
            ITag newTag = null;
            int count = 0;
            foreach (var tag in (TagCollection) _controller.Tags)
            {
                if (count == _axisCount)
                {
                    newTag = tag;
                }
                else
                {
                    count++;
                }
            }

            _axisCount++;
            if (newTag != null)
            {
                AxisVirtual axisVirtual = ((Tag) newTag).DataWrapper as AxisVirtual;
                AxisCIPDrive axisCIPDrive = ((Tag) newTag).DataWrapper as AxisCIPDrive;
                if ((axisVirtual != null && axisVirtual.AssignedGroup == _mg.ParentTag) ||
                    (axisCIPDrive != null && axisCIPDrive.AssignedGroup == _mg.ParentTag))
                {
                    newTag.PropertyChanged += OnPropertyChanged;
                    BaseAxisCollectionSource.Add(newTag);
                    BaseAxisCollection.Add(newTag);
                    BaseAxisCollection = new ObservableCollection<ITag>(BaseAxisCollection.OrderBy(x => x.Name));
                    RaisePropertyChanged("BaseAxisCollection");
                }
            }
        }

        public RelayCommand BaseToAlternate1 { set; get; }
        public RelayCommand Alternate1ToBase { set; get; }
        public RelayCommand Alternate1ToAlternate2 { set; get; }
        public RelayCommand Alternate2ToAlternate1 { set; get; }

        public ITag MotionGroup { get; }
        public Action CloseAction { get; set; }

        public RelayCommand OkCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand ApplyCommand { get; }
        public RelayCommand HelpCommand { get; }

        public RelayCommand<SelectionChangedEventArgs> BaseSelectionChanged { get; }

        private void ExecuteBaseSelectionChanged(SelectionChangedEventArgs e)
        {
            if (e?.RemovedItems != null)
            {
                foreach (ITag removedItem in e.RemovedItems)
                {
                    BaseAxisSelectedItems.Remove(removedItem);
                }
            }

            if (e?.AddedItems != null)
            {
                foreach (ITag addedItem in e.AddedItems)
                {
                    BaseAxisSelectedItems.Add(addedItem);
                }
            }
        }

        public RelayCommand<SelectionChangedEventArgs> Alternate1SelectionChanged { get; }

        private void ExecuteAlternate1SelectionChanged(SelectionChangedEventArgs e)
        {
            if (e?.RemovedItems != null)
            {
                foreach (ITag removedItem in e.RemovedItems)
                {
                    Alternate1AxisSelectedItems.Remove(removedItem);
                }
            }

            if (e?.AddedItems != null)
            {
                foreach (ITag addedItem in e.AddedItems)
                {
                    Alternate1AxisSelectedItems.Add(addedItem);
                }
            }
        }

        public RelayCommand<SelectionChangedEventArgs> Alternate2SelectionChanged { get; }

        private void ExecuteAlternate2SelectionChanged(SelectionChangedEventArgs e)
        {
            if (e?.RemovedItems != null)
            {
                foreach (ITag removedItem in e.RemovedItems)
                {
                    Alternate2AxisSelectedItems.Remove(removedItem);
                }
            }

            if (e?.AddedItems != null)
            {
                foreach (ITag addedItem in e.AddedItems)
                {
                    Alternate2AxisSelectedItems.Add(addedItem);
                }
            }
        }

        public IList BaseUpdateList { set; get; }

        public float BaseUpdate
        {
            set
            {
                Set(ref _baseUpdate, value);
                UpdateAl();
            }
            get { return _baseUpdate; }
        }

        public IList Alternate1UpdateList { set; get; }

        public float Alternate1Update
        {
            set
            {
                Set(ref _alternate1Update, value);
                Compare();
            }
            get { return _alternate1Update; }
        }

        public IList Alternate2UpdateList { set; get; }

        public float Alternate2Update
        {
            set
            {
                Set(ref _alternate2Update, value);
                Compare();
            }
            get { return _alternate2Update; }
        }

        public List<ITag> BaseAxisCollectionSource;
        public List<ITag> Alternate1AxisCollectionSource;
        public List<ITag> Alternate2AxisCollectionSource;
        private bool _enabled;
        public ObservableCollection<ITag> BaseAxisCollection { set; get; }
        public ITag BaseAxisSelectedItem { set; get; }

        public List<ITag> BaseAxisSelectedItems { get; }=new List<ITag>();
        public ObservableCollection<ITag> Alternate1AxisCollection { set; get; }
        public ITag Alternate1AxisSelectedItem { set; get; }
        public List<ITag> Alternate1AxisSelectedItems { get; }=new List<ITag>();
        public List<ITag> Alternate2AxisSelectedItems {  get; }=new List<ITag>();
        public ObservableCollection<ITag> Alternate2AxisCollection { set; get; }
        public ITag Alternate2AxisSelectedItem { set; get; }


        private void Compare()
        {

            _isDirty = false;
            ++_count;
            if (_count < 6) return;
            if (Math.Abs(_mg.CoarseUpdatePeriod / 1000f - BaseUpdate) > float.Epsilon) _isDirty = true;
            if (Math.Abs(_mg.Alternate1UpdateMultiplier / 1000f * _mg.CoarseUpdatePeriod - Alternate1Update) >
                float.Epsilon) _isDirty = true;
            if (Math.Abs(_mg.Alternate2UpdateMultiplier / 1000f * _mg.CoarseUpdatePeriod - Alternate2Update) >
                float.Epsilon) _isDirty = true;
            List<ITag> baseAxisList = new List<ITag>();
            List<ITag> alternate1List = new List<ITag>();
            List<ITag> alternate2List = new List<ITag>();
            foreach (var item in BaseAxisCollection)
            {
                baseAxisList.Add(item);
            }

            foreach (var item in Alternate1AxisCollection)
            {
                alternate1List.Add(item);
            }

            foreach (var item in Alternate2AxisCollection)
            {
                alternate2List.Add(item);
            }

            BaseAxisCollectionSource.Sort((x, y) =>
                string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));
            Alternate1AxisCollectionSource.Sort((x, y) =>
                string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));
            Alternate2AxisCollectionSource.Sort((x, y) =>
                string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));
            baseAxisList.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));
            alternate1List.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));
            alternate2List.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));
            if (baseAxisList.Except(BaseAxisCollectionSource).ToList().Count > 0) _isDirty = true;
            if (alternate2List.Except(Alternate2AxisCollectionSource).ToList().Count > 0) _isDirty = true;
            if (alternate1List.Except(Alternate1AxisCollectionSource).ToList().Count > 0) _isDirty = true;

            ApplyCommand.RaiseCanExecuteChanged();

        }

        //设置list

        private void SetList([CallerMemberName] string funName = null)
        {
            BaseAxisCollectionSource.Clear();
            Alternate1AxisCollectionSource.Clear();
            Alternate2AxisCollectionSource.Clear();


            List<string> axisNameList = new List<string>();
            foreach (var tag in _controller.Tags)
            {
                if (tag.DataTypeInfo.DataType.IsAxisType)
                    axisNameList.Add(tag.Name);
                if (funName == ".ctor") _axisCount++;
            }

            foreach (var axisName in axisNameList)
            {
                Tag axis = _controller.Tags[axisName] as Tag;
                AxisCIPDrive axisCIPDrive = axis?.DataWrapper as AxisCIPDrive;
                ITag it = _mg.ParentTag;

                if (axisCIPDrive != null)
                {
                    if (axisCIPDrive.AssignedGroup == it)
                    {
                        if ((AxisUpdateScheduleType) Convert.ToByte(axisCIPDrive.CIPAxis.AxisUpdateSchedule) ==
                            AxisUpdateScheduleType.Base)
                        {
                            BaseAxisCollectionSource.Add(axis);
                        }
                        else if ((AxisUpdateScheduleType) Convert.ToByte(axisCIPDrive.CIPAxis.AxisUpdateSchedule) ==
                                 AxisUpdateScheduleType.Alternate1)
                        {
                            Alternate1AxisCollectionSource.Add(axis);
                        }
                        else
                        {
                            Alternate2AxisCollectionSource.Add(axis);
                        }


                    }

                    (axisCIPDrive.ParentTag as Tag).PropertyChanged += OnPropertyChanged;
                }

                //TODO(gjc): add AxisVirtual
                AxisVirtual axisVirtual = axis?.DataWrapper as AxisVirtual;

                if (axisVirtual != null)
                {
                    if (axisVirtual.AssignedGroup == it)
                    {
                        if ((AxisUpdateScheduleType) Convert.ToByte(axisVirtual.CIPAxis.AxisUpdateSchedule) ==
                            AxisUpdateScheduleType.Base)
                        {
                            BaseAxisCollectionSource.Add(axis);
                        }
                        else if ((AxisUpdateScheduleType) Convert.ToByte(axisVirtual.CIPAxis.AxisUpdateSchedule) ==
                                 AxisUpdateScheduleType.Alternate1)
                        {
                            Alternate1AxisCollectionSource.Add(axis);
                        }
                        else
                        {
                            Alternate2AxisCollectionSource.Add(axis);
                        }


                    }

                    (axisVirtual.ParentTag as Tag).PropertyChanged += OnPropertyChanged;
                }
            }

            BaseAxisCollection.Clear();
            Alternate1AxisCollection.Clear();
            Alternate2AxisCollection.Clear();
            foreach (var item in BaseAxisCollectionSource)
            {
                BaseAxisCollection.Add(item);
            }

            foreach (var item in Alternate1AxisCollectionSource)
            {
                Alternate1AxisCollection.Add(item);
            }

            foreach (var item in Alternate2AxisCollectionSource)
            {
                Alternate2AxisCollection.Add(item);
            }


            BaseAxisSelectedItem = null;
            Alternate1AxisSelectedItem = null;
            Alternate2AxisSelectedItem = null;

            BaseAxisCollection = new ObservableCollection<ITag>(BaseAxisCollection.OrderBy(x => x.Name));
            Alternate1AxisCollection = new ObservableCollection<ITag>(Alternate1AxisCollection.OrderBy(x => x.Name));
            Alternate2AxisCollection = new ObservableCollection<ITag>(Alternate2AxisCollection.OrderBy(x => x.Name));
            RaisePropertyChanged("BaseAxisCollection");
            RaisePropertyChanged("Alternate1AxisCollection");
            RaisePropertyChanged("Alternate2AxisCollection");
        }

        //设置下拉框

        // ReSharper disable once InconsistentNaming

        private bool CanExecuteCommand()
        {
            return Enabled;
        }

        private void SetBA1A2()
        {
            float i = 0.5f;
            List<float> list = new List<float>();
            while (i <= 32)
            {
                list.Add(i);
                i = i + 0.5f;
            }

            BaseUpdateList = list.Select(val => new {DisplayName = val.ToString("f1"), Value = val}).ToList();
            BaseUpdate = _mg.CoarseUpdatePeriod / 1000f;


            int j = 1;
            list = new List<float>();
            while (BaseUpdate * j <= 32)
            {
                list.Add(BaseUpdate * j);
                j++;
            }

            Alternate1UpdateList = list.Select(val => new {DisplayName = val.ToString("f1"), Value = val}).ToList();
            Alternate1Update = _mg.Alternate1UpdateMultiplier * BaseUpdate;
            Alternate2UpdateList = list.Select(val => new {DisplayName = val.ToString("f1"), Value = val}).ToList();
            Alternate2Update = _mg.Alternate2UpdateMultiplier * BaseUpdate;
        }


        private void UpdateAl()
        {
            int j = 1;
            List<float> list = new List<float>();
            while (BaseUpdate * j <= 32)
            {
                list.Add(BaseUpdate * j);
                j++;
            }

            Alternate1UpdateList = list.Select(val => new {DisplayName = val.ToString("f1"), Value = val}).ToList();
            if (!list.Contains(Alternate1Update)) Alternate1Update = list[0];

            Alternate2UpdateList = list.Select(val => new {DisplayName = val.ToString("f1"), Value = val}).ToList();
            if (!list.Contains(Alternate2Update)) Alternate2Update = list[0];

            RaisePropertyChanged("Alternate1UpdateList");
            RaisePropertyChanged("Alternate1Update");
            RaisePropertyChanged("Alternate2UpdateList");
            RaisePropertyChanged("Alternate2Update");

            Compare();
        }


        private void B2A1()
        {
            if (BaseAxisSelectedItems.Count==0) return;

            foreach (var item in BaseAxisSelectedItems.ToList())
            {
                Alternate1AxisCollection.Add(item);
                BaseAxisCollection.Remove(item);
            }
            Alternate1AxisCollection = new ObservableCollection<ITag>(Alternate1AxisCollection.OrderBy(x => x.Name));
            RaisePropertyChanged("Alternate1AxisCollection");
            Compare();
        }


        private void A12B()
        {
            if (Alternate1AxisSelectedItems.Count==0) return;
            foreach (var item in Alternate1AxisSelectedItems.ToList())
            {
                BaseAxisCollection.Add(item);
                Alternate1AxisCollection.Remove(item);
            }

            BaseAxisCollection = new ObservableCollection<ITag>(BaseAxisCollection.OrderBy(x => x.Name));
            RaisePropertyChanged("BaseAxisCollection");
            Compare();
        }


        private void A12A2()
        {
            if (Alternate1AxisSelectedItems.Count==0) return;
            foreach (var item in Alternate1AxisSelectedItems.ToList())
            {
                Alternate2AxisCollection.Add(item);
                Alternate1AxisCollection.Remove(item);
            }
            
            Alternate2AxisCollection = new ObservableCollection<ITag>(Alternate2AxisCollection.OrderBy(x => x.Name));
            RaisePropertyChanged("Alternate2AxisCollection");
            Compare();
        }

        private void A22A1()
        {
            if (Alternate2AxisSelectedItems.Count==0) return;
            foreach (var item in Alternate2AxisSelectedItems.ToList())
            {
                Alternate1AxisCollection.Add(item);
                Alternate2AxisCollection.Remove(item);
            }

            Alternate1AxisCollection = new ObservableCollection<ITag>(Alternate1AxisCollection.OrderBy(x => x.Name));
            RaisePropertyChanged("Alternate1AxisCollection");
            Compare();
        }

        private void MotionGroup_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CoarseUpdatePeriod")
            {
                BaseUpdate = (_mg.CoarseUpdatePeriod / 1000f);
            }

            if (e.PropertyName == "Alternate1UpdateMultiplier")
            {
                Alternate1Update = _mg.Alternate1UpdateMultiplier * _mg.CoarseUpdatePeriod / 1000f;
            }

            if (e.PropertyName == "Alternate2UpdateMultiplier")
            {
                Alternate2Update = _mg.Alternate2UpdateMultiplier * _mg.CoarseUpdatePeriod / 1000f;
            }
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                Enabled = !_controller.IsOnline;
                BaseToAlternate1.RaiseCanExecuteChanged();
                Alternate1ToBase.RaiseCanExecuteChanged();
                Alternate1ToAlternate2.RaiseCanExecuteChanged();
                Alternate2ToAlternate1.RaiseCanExecuteChanged();
            });
        }

        #region Command

        private void ExecuteHelpCommand()
        {
            //TODO(gjc): add code here
        }

        private bool CanExecuteApplyCommand()
        {
            //TODO(gjc): add code here
            return _isDirty;
        }

        private void ExecuteApplyCommand()
        {
            if (!_isDirty)
            {
                SetList();

                Compare();

                return;
            }

            _mg.CoarseUpdatePeriod = (int) (BaseUpdate * 1000);
            _mg.Alternate1UpdateMultiplier = (int) (Alternate1Update / BaseUpdate);
            _mg.Alternate2UpdateMultiplier = (int) (Alternate2Update / BaseUpdate);

            List<ITag> baseAxisList = new List<ITag>();
            List<ITag> alternate1List = new List<ITag>();
            List<ITag> alternate2List = new List<ITag>();
            foreach (var item in BaseAxisCollection)
            {
                baseAxisList.Add(item);
            }

            foreach (var item in Alternate1AxisCollection)
            {
                alternate1List.Add(item);
            }

            foreach (var item in Alternate2AxisCollection)
            {
                alternate2List.Add(item);
            }

            BaseAxisCollectionSource.Sort((x, y) =>
                string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));
            Alternate1AxisCollectionSource.Sort((x, y) =>
                string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));
            Alternate2AxisCollectionSource.Sort((x, y) =>
                string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));
            baseAxisList.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));
            alternate1List.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));
            alternate2List.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));
            List<ITag> temp;
            temp = baseAxisList.Except(BaseAxisCollectionSource).ToList();
            if (temp.Count > 0)
            {
                foreach (var item in temp)
                {
                    Tag axis = _controller.Tags[item.Name] as Tag;
                    AxisCIPDrive axisCIPDrive = axis?.DataWrapper as AxisCIPDrive;
                    if (axisCIPDrive != null)
                    {
                        axisCIPDrive.CIPAxis.AxisUpdateSchedule = (byte) AxisUpdateScheduleType.Base;
                        axisCIPDrive.NotifyParentPropertyChanged("AxisUpdateSchedule");
                    }

                    AxisVirtual axisVirtual = axis?.DataWrapper as AxisVirtual;
                    if (axisVirtual != null)
                    {
                        axisVirtual.CIPAxis.AxisUpdateSchedule = (byte) AxisUpdateScheduleType.Base;
                        axisVirtual.NotifyParentPropertyChanged("AxisUpdateSchedule");
                    }

                }
            }

            temp = alternate1List.Except(Alternate1AxisCollectionSource).ToList();
            if (temp.Count > 0)
            {
                foreach (var item in temp)
                {
                    Tag axis = _controller.Tags[item.Name] as Tag;
                    AxisCIPDrive axisCIPDrive = axis?.DataWrapper as AxisCIPDrive;
                    if (axisCIPDrive != null)
                    {
                        axisCIPDrive.CIPAxis.AxisUpdateSchedule = (byte) AxisUpdateScheduleType.Alternate1;
                        axisCIPDrive.NotifyParentPropertyChanged("AxisUpdateSchedule");
                    }

                    AxisVirtual axisVirtual = axis?.DataWrapper as AxisVirtual;
                    if (axisVirtual != null)
                    {
                        axisVirtual.CIPAxis.AxisUpdateSchedule = (byte) AxisUpdateScheduleType.Alternate1;
                        axisVirtual.NotifyParentPropertyChanged("AxisUpdateSchedule");
                    }

                }
            }

            temp = alternate2List.Except(Alternate2AxisCollectionSource).ToList();
            if (temp.Count > 0)
            {
                foreach (var item in temp)
                {
                    Tag axis = _controller.Tags[item.Name] as Tag;
                    AxisCIPDrive axisCIPDrive = axis?.DataWrapper as AxisCIPDrive;
                    if (axisCIPDrive != null)
                    {
                        axisCIPDrive.CIPAxis.AxisUpdateSchedule = (byte) AxisUpdateScheduleType.Alternate2;
                        axisCIPDrive.NotifyParentPropertyChanged("AxisUpdateSchedule");
                    }

                    AxisVirtual axisVirtual = axis?.DataWrapper as AxisVirtual;
                    if (axisVirtual != null)
                    {
                        axisVirtual.CIPAxis.AxisUpdateSchedule = (byte) AxisUpdateScheduleType.Alternate2;
                        axisVirtual.NotifyParentPropertyChanged("AxisUpdateSchedule");
                    }

                }
            }

            SetList();

            Compare();

            //IStudioUIService studioUIService =
            //    Package.GetGlobalService(typeof(SStudioUIService)) as IStudioUIService;
            //studioUIService?.UpdateUI();
        }


        private void ExecuteCancelCommand()
        {
            //TODO(gjc): add code here
            CloseAction?.Invoke();
        }

        private void ExecuteOkCommand()
        {
            ExecuteApplyCommand();
            //TODO(gjc): add code here
            CloseAction?.Invoke();
        }

        #endregion

    }

    public class Message
    {
        public string Type { set; get; }
        public string Value { set; get; }
        public bool IsDirty { set; get; }
    }
}