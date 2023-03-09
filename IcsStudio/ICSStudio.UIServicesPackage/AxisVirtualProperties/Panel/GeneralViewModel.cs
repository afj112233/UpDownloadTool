using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.UIInterfaces.Dialog;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ICSStudio.UIServicesPackage.AxisVirtualProperties.Panel
{
    class GeneralViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private MotionGroup _mg;
        private readonly IController _controller;
        private readonly AxisVirtual _axisVirtual;
        private bool _isDirty;
        private string _selected;
        private ITag _tag;

        public GeneralViewModel(General panel, ITag tag)
        {
            Control = panel;
            panel.DataContext = this;
            _tag = tag;
            _axisVirtual = ((SimpleServices.Tags.Tag)tag)?.DataWrapper as AxisVirtual;
            _selected = null;
            _controller = tag.ParentController;
            ((SimpleServices.Tags.Tag) tag).PropertyChanged += AssignedGroupOnPropertyChanged;

            MGCommandRelayCommand = new RelayCommand(MGCommand, CanCommand);
            UPCommandRelayCommand = new RelayCommand(UPCommand, CanCommand);
            SetValue();

            Controller controller = _controller as Controller;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                
                RaisePropertyChanged("IsEnable");
            });
        }

        public override void Cleanup()
        {
            ((SimpleServices.Tags.Tag) _tag).PropertyChanged -= AssignedGroupOnPropertyChanged;
            Controller controller = _controller as Controller;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }
        }

        public void AssignedGroupOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            if (e.PropertyName == "Name" || e.PropertyName == "CoarseUpdatePeriod" ||
                e.PropertyName == "Alternate1UpdateMultiplier" || e.PropertyName == "Alternate2UpdateMultiplier" ||
                e.PropertyName == "AssignedGroup")
            {
                SetValue();
                RaisePropertyChanged("MotionGroup");
                RaisePropertyChanged("UpdatePeriod");
                RaisePropertyChanged("Selected");
            }
        }

        public void SetValue()
        {
            List<string> mgName = new List<string>();
            foreach (var motion in _controller.Tags)
            {
                if (motion.DataTypeInfo.DataType.IsMotionGroupType)
                {
                    motion.PropertyChanged += AssignedGroupOnPropertyChanged;
                    mgName.Add(motion.Name);
                }
            }

            mgName.Insert(0, "<none>");
            MotionGroup = mgName.Select(val => new {Value = val, Name = val}).ToList();
            Selected = "<none>";
            UpdatePeriod = "";
            if (_axisVirtual.AssignedGroup != null)
            {
                SimpleServices.Tags.Tag tag1 = _axisVirtual.AssignedGroup as SimpleServices.Tags.Tag;
                _mg = tag1?.DataWrapper as MotionGroup;

                if ((AxisUpdateScheduleType) Convert.ToByte(_axisVirtual.CIPAxis.AxisUpdateSchedule) ==
                    AxisUpdateScheduleType.Base)
                    UpdatePeriod = (_mg.CoarseUpdatePeriod / 1000f).ToString("f1");
                else if ((AxisUpdateScheduleType) Convert.ToByte(_axisVirtual.CIPAxis.AxisUpdateSchedule) ==
                         AxisUpdateScheduleType.Alternate1)
                    UpdatePeriod = (_mg.Alternate1UpdateMultiplier * _mg.CoarseUpdatePeriod / 1000f).ToString("f1");
                else
                    UpdatePeriod = (_mg.Alternate2UpdateMultiplier * _mg.CoarseUpdatePeriod / 1000f).ToString("f1");
                Selected = _axisVirtual.AssignedGroup.Name;
            }
        }



        public void Compare()
        {
            IsDirty = (Selected == "<none>" ? null : Selected) !=
                      (_axisVirtual.AssignedGroup == null ? null : _axisVirtual.AssignedGroup.Name);
        }

        public void Save()
        {
            if (Selected == "<none>")
            {
                _axisVirtual.AssignedGroup = null;
                UpdatePeriod = null;
            }
            else
            {
                _axisVirtual.AssignedGroup = _controller.Tags[Selected];

                if ((AxisUpdateScheduleType) Convert.ToByte(_axisVirtual.CIPAxis.AxisUpdateSchedule) ==
                    AxisUpdateScheduleType.Base)
                    UpdatePeriod = (_mg.CoarseUpdatePeriod / 1000f).ToString("f1");
                else if ((AxisUpdateScheduleType) Convert.ToByte(_axisVirtual.CIPAxis.AxisUpdateSchedule) ==
                         AxisUpdateScheduleType.Alternate1)
                    UpdatePeriod = (_mg.Alternate1UpdateMultiplier * _mg.CoarseUpdatePeriod / 1000f).ToString("f1");
                else
                    UpdatePeriod = (_mg.Alternate2UpdateMultiplier * _mg.CoarseUpdatePeriod / 1000f).ToString("f1");
            }

            RaisePropertyChanged("UpdatePeriod");
            MGCommandRelayCommand.RaiseCanExecuteChanged();
            UPCommandRelayCommand.RaiseCanExecuteChanged();
        }

        public RelayCommand MGCommandRelayCommand { set; get; }
        public RelayCommand UPCommandRelayCommand { set; get; }

        private void MGCommand()
        {
            ICreateDialogService createDialogService =
                Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;

            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = (IVsUIShell) Package.GetGlobalService(typeof(SVsUIShell));

            if (createDialogService != null)
            {
                var window = createDialogService.CreateMotionGroupProperties(_axisVirtual.AssignedGroup);
                window.Show(uiShell);
            }
        }

        private bool CanCommand()
        {
            return _axisVirtual.AssignedGroup != null;
        }

        public void UPCommand()
        {
            ICreateDialogService createDialogService =
                Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;

            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = (IVsUIShell) Package.GetGlobalService(typeof(SVsUIShell));

            if (createDialogService != null)
            {
                var window = createDialogService.CreateAxisScheduleDialog(_axisVirtual.AssignedGroup);
                window.Show(uiShell);
            }
        }
        
        public bool IsEnable
        {
            get { return !_controller.IsOnline; }
        }

        public IList MotionGroup { set; get; }

        public string Selected
        {
            set
            {
                _selected = value;
                Compare();
            }
            get { return _selected; }
        }

        public object Owner { get; set; }
        public object Control { get; }
        public string UpdatePeriod { set; get; }

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
    }
}
