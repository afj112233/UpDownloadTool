using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Dialog;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ICSStudio.Dialogs.NewTag;
using ICSStudio.MultiLanguage;

namespace ICSStudio.DeviceProperties.ServoDrives.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    class AssociatedAxesViewModel : DeviceOptionPanel
    {
        private List<ITag> _monitorAxisList;

        private IList _axis1Source;
        private IList _axis2Source;
        private IList _axis3Source;
        private IList _axis4Source;

        public AssociatedAxesViewModel(UserControl panel, ModifiedMotionDrive modifiedMotionDrive)
            : base(panel)
        {
            ModifiedMotionDrive = modifiedMotionDrive;

            AxisPropertiesCommand =
                new RelayCommand<ITag>(ExecuteAxisPropertiesCommand, CanExecuteAxisPropertiesCommand);
            NewAxisCommand = new RelayCommand<int>(ExecuteNewAxisCommand);

            UpdateAxis1Source();
            UpdateAxis1MotorFeedbackSource();
            UpdateAxis1LoadFeedbackSource();

            UpdateAxis2Source();
            UpdateAxis2MotorFeedbackSource();

            UpdateAxis3Source();
            UpdateAxis3MotorFeedbackSource();
            UpdateAxis3LoadFeedbackSource();

            UpdateAxis4Source();
            UpdateAxis4MotorFeedbackSource();

            ModifiedMotionDrive.Controller.Tags.CollectionChanged += TagsOnCollectionChanged;
            CreateMonitorAxisList();
        }

        public ModifiedMotionDrive ModifiedMotionDrive { get; }
        public CIPMotionDrive OriginalMotionDrive => ModifiedMotionDrive.OriginalMotionDrive;

        public int Axis1Index => 1;
        public int Axis2Index => 2;
        public int Axis3Index => 3;
        public int Axis4Index => 4;

        public bool Enable
        {
            get
            {
                if (ModifiedMotionDrive.Controller.IsOnline)
                    return false;

                return true;
            }
        }

        public ITag Axis1
        {
            get { return ModifiedMotionDrive.AssociatedAxes[0]; }
            set
            {
                if (ModifiedMotionDrive.AssociatedAxes[0] != value)
                {
                    ModifiedMotionDrive.AssociatedAxes[0] = value;
                    RaisePropertyChanged();

                    AxisPropertiesCommand.RaiseCanExecuteChanged();

                    CheckDirty();
                }
            }
        }

        public IList Axis1Source
        {
            get { return _axis1Source; }
            set { Set(ref _axis1Source, value); }
        }

        public int Axis1MotorFeedback
        {
            get { return ModifiedMotionDrive.FeedbackPortSelect[(1 - 1) * 4]; }
            set
            {
                ModifiedMotionDrive.FeedbackPortSelect[(1 - 1) * 4] = (byte) value;
                RaisePropertyChanged();
                CheckDirty();
            }
        }

        public IList Axis1MotorFeedbackSource { get; set; }

        public int Axis1LoadFeedback
        {
            get { return ModifiedMotionDrive.FeedbackPortSelect[(1 - 1) * 4 + 1]; }
            set
            {
                ModifiedMotionDrive.FeedbackPortSelect[(1 - 1) * 4 + 1] = (byte) value;
                RaisePropertyChanged();
                CheckDirty();
            }
        }

        public IList Axis1LoadFeedbackSource { get; set; }

        public ITag Axis2
        {
            get
            {
                if (Axis2Visibility == Visibility.Visible)
                    return ModifiedMotionDrive.AssociatedAxes[1];

                return null;
            }
            set
            {
                if (Axis2Visibility == Visibility.Visible
                    && ModifiedMotionDrive.AssociatedAxes[1] != value)
                {
                    ModifiedMotionDrive.AssociatedAxes[1] = value;
                    RaisePropertyChanged();

                    AxisPropertiesCommand.RaiseCanExecuteChanged();

                    CheckDirty();
                }
            }
        }

        public IList Axis2Source
        {
            get { return _axis2Source; }
            set { Set(ref _axis2Source, value); }
        }

        public int Axis2MotorFeedback
        {
            get
            {
                if (Axis2Visibility == Visibility.Visible)
                {
                    return ModifiedMotionDrive.FeedbackPortSelect[(2 - 1) * 4];
                }

                return 0;
            }
            set
            {
                if (Axis2Visibility == Visibility.Visible)
                {
                    ModifiedMotionDrive.FeedbackPortSelect[(2 - 1) * 4] = (byte) value;
                    RaisePropertyChanged();
                    CheckDirty();
                }
            }
        }

        public IList Axis2MotorFeedbackSource { get; set; }

        public ITag Axis3
        {
            get
            {
                if (Axis3Visibility == Visibility.Visible)
                    return ModifiedMotionDrive.AssociatedAxes[2];

                return null;
            }
            set
            {
                if (Axis3Visibility == Visibility.Visible
                    && ModifiedMotionDrive.AssociatedAxes[2] != value)
                {
                    ModifiedMotionDrive.AssociatedAxes[2] = value;
                    RaisePropertyChanged();

                    AxisPropertiesCommand.RaiseCanExecuteChanged();

                    CheckDirty();
                }
            }
        }

        public IList Axis3Source
        {
            get { return _axis3Source; }
            set { Set(ref _axis3Source, value); }
        }

        public int Axis3MotorFeedback
        {
            get
            {
                if (Axis3MotorFeedbackCmbVisibility == Visibility.Visible)
                {
                    return ModifiedMotionDrive.FeedbackPortSelect[(3 - 1) * 4];
                }

                return 0;
            }
            set
            {
                if (Axis3MotorFeedbackCmbVisibility == Visibility.Visible)
                {
                    ModifiedMotionDrive.FeedbackPortSelect[(3 - 1) * 4] = (byte) value;
                    RaisePropertyChanged();
                    CheckDirty();
                }
            }
        }

        public IList Axis3MotorFeedbackSource { get; set; }

        public int Axis3LoadFeedback
        {
            get
            {
                if (Axis3MotorFeedbackCmbVisibility == Visibility.Visible)
                {
                    return ModifiedMotionDrive.FeedbackPortSelect[(3 - 1) * 4 + 1];
                }

                return 0;
            }
            set
            {
                if (Axis3MotorFeedbackCmbVisibility == Visibility.Visible)
                {
                    ModifiedMotionDrive.FeedbackPortSelect[(3 - 1) * 4 + 1] = (byte) value;
                    RaisePropertyChanged();
                    CheckDirty();
                }
            }
        }

        public IList Axis3LoadFeedbackSource { get; set; }

        public ITag Axis4
        {
            get
            {
                if (Axis4Visibility == Visibility.Visible)
                    return ModifiedMotionDrive.AssociatedAxes[3];

                return null;
            }
            set
            {
                if (Axis4Visibility == Visibility.Visible
                    && ModifiedMotionDrive.AssociatedAxes[3] != value)
                {
                    ModifiedMotionDrive.AssociatedAxes[3] = value;
                    RaisePropertyChanged();

                    AxisPropertiesCommand.RaiseCanExecuteChanged();

                    CheckDirty();
                }
            }
        }

        public IList Axis4Source
        {
            get { return _axis4Source; }
            set { Set(ref _axis4Source, value); }
        }

        public int Axis4MotorFeedback
        {
            get
            {
                if (Axis4Visibility == Visibility.Visible)
                {
                    return ModifiedMotionDrive.FeedbackPortSelect[(4 - 1) * 4];
                }

                return 0;
            }
            set
            {
                if (Axis4Visibility == Visibility.Visible)
                {
                    ModifiedMotionDrive.FeedbackPortSelect[(4 - 1) * 4] = (byte) value;
                    RaisePropertyChanged();
                    CheckDirty();
                }
            }
        }

        public IList Axis4MotorFeedbackSource { get; set; }

        #region Visibility

        public Visibility Axis1MotorFeedbackCmbVisibility
        {
            get
            {
                if (ModifiedMotionDrive.Profiles.Schema.Feedback.Ports[0].Hidden)
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        public Visibility Axis1MotorFeedbackTxtVisibility
        {
            get
            {
                if (Axis1MotorFeedbackCmbVisibility == Visibility.Visible)
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        public Visibility Axis1LoadVisibility
        {
            get
            {
                if (ModifiedMotionDrive.Profiles?.Schema?.Axes[0]?.AllowableFeedbackPorts?.Load == null)
                    return Visibility.Hidden;

                return Visibility.Visible;
            }
        }

        public Visibility Axis2Visibility
        {
            get
            {
                if (ModifiedMotionDrive.AssociatedAxes.Length >= 2)
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

        public Visibility Axis2MotorVisibility
        {
            get
            {
                if (Axis2Visibility != Visibility.Visible
                    || ModifiedMotionDrive.Profiles.Schema.Axes[1].AllowableFeedbackPorts?.MotorMaster == null)
                    return Visibility.Hidden;

                return Visibility.Visible;
            }
        }

        public Visibility Axis3Visibility
        {
            get
            {
                if (ModifiedMotionDrive.AssociatedAxes.Length >= 3)
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

        public Visibility Axis3MotorFeedbackCmbVisibility
        {
            get
            {
                if (Axis3Visibility != Visibility.Visible
                    || ModifiedMotionDrive.Profiles.Schema.Feedback.Ports[2].Hidden)
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        public Visibility Axis3MotorFeedbackTxtVisibility
        {
            get
            {
                if (Axis3Visibility != Visibility.Visible
                    || Axis3MotorFeedbackCmbVisibility == Visibility.Visible)
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        public Visibility Axis3LoadVisibility
        {
            get
            {
                if (Axis3Visibility != Visibility.Visible
                    || ModifiedMotionDrive.Profiles.Schema.Axes[2]?.AllowableFeedbackPorts?.Load == null)
                    return Visibility.Hidden;

                return Visibility.Visible;
            }
        }

        public Visibility Axis4Visibility
        {
            get
            {
                if (ModifiedMotionDrive.AssociatedAxes.Length >= 4)
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

        #endregion

        public RelayCommand<ITag> AxisPropertiesCommand { get; }
        public RelayCommand<int> NewAxisCommand { get; }

        #region Command

        private void ExecuteNewAxisCommand(int axisIndex)
        {
            var dialog = new NewTagDialog(
                new NewTagViewModel(
                    "AXIS_CIP_DRIVE",
                    ModifiedMotionDrive.Controller.Tags,
                    Usage.NullParameterType, null))
            {
                Owner = Application.Current.MainWindow
            };

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                Tag newTag = dialog.NewTag as Tag;
                AxisCIPDrive newAxisCIPDrive = newTag?.DataWrapper as AxisCIPDrive;
                if (newAxisCIPDrive != null)
                {
                    Tag oldTag = OriginalMotionDrive.GetAxis(axisIndex) as Tag;
                    AxisCIPDrive oldAxisCIPDrive = oldTag?.DataWrapper as AxisCIPDrive;
                    if (oldAxisCIPDrive != null)
                    {
                        OriginalMotionDrive.RemoveAxis(oldTag, axisIndex);
                        oldAxisCIPDrive.UpdateAxisChannel(null, 0);
                    }

                    if (OriginalMotionDrive.AddAxis(newTag, axisIndex) == 0)
                    {
                        newAxisCIPDrive.UpdateAxisChannel(OriginalMotionDrive, axisIndex);
                    }
                }
            }
        }

        private bool CanExecuteAxisPropertiesCommand(ITag axis)
        {
            if (axis == null)
                return false;

            return true;
        }

        private void ExecuteAxisPropertiesCommand(ITag axis)
        {
            if (axis == null)
                return;

            ICreateDialogService createDialogService =
                Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;
            var uiShell = (IVsUIShell) Package.GetGlobalService(typeof(SVsUIShell));

            var window =
                createDialogService?.CreateAxisCIPDriveProperties(axis);
            window?.Show(uiShell);
        }

        #endregion

        #region Override

        public override void Show()
        {
            RaisePropertyChanged("Enable");
            UIRefresh();
        }

        public override void CheckDirty()
        {
            int length = ModifiedMotionDrive.AssociatedAxes.Length;
            for (int i = 0; i < length; i++)
            {
                if (ModifiedMotionDrive.AssociatedAxes[i]
                    != OriginalMotionDrive.GetAxis(i + 1))
                {
                    IsDirty = true;
                    return;
                }
            }

            if (!ModifiedMotionDrive.FeedbackPortSelect.SequenceEqual(
                OriginalMotionDrive.ConfigData.FeedbackPortSelect)
            )
            {
                IsDirty = true;
                return;
            }

            IsDirty = false;
        }

        public override int CheckValid()
        {
            string message = string.Empty;
            int result = 0;

            // Axis
            List<ITag> axisList = new List<ITag>();
            for (int i = 0; i < ModifiedMotionDrive.AssociatedAxes.Length; i++)
            {
                var axis = ModifiedMotionDrive.AssociatedAxes[i];

                if (axis != null)
                {
                    if (axis != OriginalMotionDrive.GetAxis(i + 1))
                    {
                        Tag tag = axis as Tag;
                        AxisCIPDrive axisCIPDrive = tag?.DataWrapper as AxisCIPDrive;
                        if (axisCIPDrive?.AssociatedModule != null)
                        {
                            message = LanguageManager.GetInstance().ConvertSpecifier("Failed to modify properties.");
                            message += "\n" + LanguageManager.GetInstance().ConvertSpecifier("Failed to set the 'MotionModule' property(Axis has be assigned to other drive or drive channel.)");
                            result = -1;
                            goto End;
                        }
                    }

                    if (axisList.Contains(axis))
                    {
                        message = LanguageManager.GetInstance().ConvertSpecifier("Failed to modify properties.");
                        message += "\n" + LanguageManager.GetInstance().ConvertSpecifier("Failed to set the 'MotionModule' property(Same axis cannot be assigned to more than one drive or drive channel.)");
                        result = -1;
                        goto End;
                    }

                    axisList.Add(axis);
                }
            }

            // FeedbackPortSelect
            List<byte> byteList = new List<byte>();
            int length = ModifiedMotionDrive.FeedbackPortSelect.Count;
            for (int i = 0; i < length; i++)
            {
                var feedbackPortSelect = ModifiedMotionDrive.FeedbackPortSelect[i];
                if (feedbackPortSelect != 0)
                {
                    if (byteList.Contains(feedbackPortSelect))
                    {
                        message = LanguageManager.GetInstance().ConvertSpecifier("Feedback Device Invalid.") + "\n" 
                            + LanguageManager.GetInstance().ConvertSpecifier("Duplicate Feedback Devices.");
                        result = -1;
                        goto End;
                    }

                    byteList.Add(feedbackPortSelect);
                }
            }


            End:
            if (result < 0)
            {
                MessageBox.Show(message, "ICS Studio", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }

            return result;
        }

        public override bool SaveOptions()
        {
            // AssociatedAxes
            int length = ModifiedMotionDrive.AssociatedAxes.Length;
            for (int i = 0; i < length; i++)
            {
                var axis = ModifiedMotionDrive.AssociatedAxes[i];
                var originalAxis = OriginalMotionDrive.GetAxis(i + 1);
                if (axis != originalAxis)
                {
                    if (originalAxis != null)
                    {
                        Tag tag = originalAxis as Tag;
                        AxisCIPDrive axisCIPDrive = tag?.DataWrapper as AxisCIPDrive;

                        if (axisCIPDrive != null)
                        {
                            if (OriginalMotionDrive.RemoveAxis(originalAxis, i + 1) == 0)
                            {
                                axisCIPDrive.UpdateAxisChannel(null, 0);
                            }
                        }
                    }

                    if (axis != null)
                    {
                        Tag tag = axis as Tag;
                        AxisCIPDrive axisCIPDrive = tag?.DataWrapper as AxisCIPDrive;
                        if (axisCIPDrive != null)
                        {
                            if (OriginalMotionDrive.AddAxis(axis, i + 1) == 0)
                            {
                                axisCIPDrive.UpdateAxisChannel(OriginalMotionDrive, i + 1);
                            }
                        }
                    }
                }
            }

            // FeedbackPortSelect
            length = OriginalMotionDrive.ConfigData.FeedbackPortSelect.Count;
            for (int i = 0; i < length; i++)
            {
                OriginalMotionDrive.ConfigData.FeedbackPortSelect[i] =
                    ModifiedMotionDrive.FeedbackPortSelect[i];
            }

            return true;
        }

        #endregion

        #region Private

        private void UpdateAxis1Source()
        {
            var oldAxis = Axis1;

            var axisList = GetAxisSourceByIndex(1);
            Axis1Source = axisList;

            Axis1 = Contains(axisList, oldAxis) ? oldAxis : null;

            RaisePropertyChanged("Axis1");
        }

        private void UpdateAxis1MotorFeedbackSource()
        {
            if (Axis1MotorFeedbackCmbVisibility == Visibility.Visible)
            {
                var supportedPortList =
                    ModifiedMotionDrive.Profiles.GetSupportedMotorFeedbackPortList(1, ModifiedMotionDrive.Major);
                if (supportedPortList != null && supportedPortList.Count > 0)
                {
                    Axis1MotorFeedbackSource = PortListToDisplayItemList(supportedPortList);
                }
                else
                {
                    Axis1MotorFeedbackSource = null;
                }
            }
            else
            {
                Axis1MotorFeedbackSource = null;
            }
        }

        private void UpdateAxis1LoadFeedbackSource()
        {
            if (Axis1LoadVisibility == Visibility.Visible)
            {
                var supportedPortList =
                    ModifiedMotionDrive.Profiles.GetSupportedLoadFeedbackPortList(1, ModifiedMotionDrive.Major);
                if (supportedPortList != null && supportedPortList.Count > 0)
                {
                    Axis1LoadFeedbackSource = PortListToDisplayItemList(supportedPortList);
                }
                else
                {
                    Axis1LoadFeedbackSource = null;
                }
            }
            else
            {
                Axis1LoadFeedbackSource = null;
            }
        }

        private void UpdateAxis2Source()
        {
            if (Axis2Visibility == Visibility.Visible)
            {
                var oldAxis = Axis2;

                var axisList = GetAxisSourceByIndex(2);
                Axis2Source = axisList;

                Axis2 = Contains(axisList, oldAxis) ? oldAxis : null;

                RaisePropertyChanged("Axis2");
            }

        }

        private void UpdateAxis2MotorFeedbackSource()
        {
            if (Axis2Visibility == Visibility.Visible)
            {
                var supportedPortList =
                    ModifiedMotionDrive.Profiles.GetSupportedMotorFeedbackPortList(2, ModifiedMotionDrive.Major);
                if (supportedPortList != null && supportedPortList.Count > 0)
                {
                    Axis2MotorFeedbackSource = PortListToDisplayItemList(supportedPortList);
                }
                else
                {
                    Axis2MotorFeedbackSource = null;
                }
            }
            else
            {
                Axis2MotorFeedbackSource = null;
            }
        }

        private void UpdateAxis3Source()
        {
            if (Axis2Visibility == Visibility.Visible)
            {
                var oldAxis = Axis3;

                var axisList = GetAxisSourceByIndex(3);
                Axis3Source = axisList;

                Axis3 = Contains(axisList, oldAxis) ? oldAxis : null;

                RaisePropertyChanged("Axis3");
            }
        }

        private void UpdateAxis3MotorFeedbackSource()
        {
            if (Axis3MotorFeedbackCmbVisibility == Visibility.Visible)
            {
                var supportedPortList =
                    ModifiedMotionDrive.Profiles.GetSupportedMotorFeedbackPortList(3, ModifiedMotionDrive.Major);
                if (supportedPortList != null && supportedPortList.Count > 0)
                {
                    Axis3MotorFeedbackSource = PortListToDisplayItemList(supportedPortList);
                }
                else
                {
                    Axis3MotorFeedbackSource = null;
                }
            }
            else
            {
                Axis3MotorFeedbackSource = null;
            }
        }

        private void UpdateAxis3LoadFeedbackSource()
        {
            if (Axis3LoadVisibility == Visibility.Visible)
            {
                var supportedPortList =
                    ModifiedMotionDrive.Profiles.GetSupportedLoadFeedbackPortList(3, ModifiedMotionDrive.Major);
                if (supportedPortList != null && supportedPortList.Count > 0)
                {
                    Axis3LoadFeedbackSource = PortListToDisplayItemList(supportedPortList);
                }
                else
                {
                    Axis3LoadFeedbackSource = null;
                }
            }
            else
            {
                Axis3LoadFeedbackSource = null;
            }
        }

        private void UpdateAxis4Source()
        {
            if (Axis4Visibility == Visibility.Visible)
            {
                var oldAxis = Axis4;

                var axisList = GetAxisSourceByIndex(4);
                Axis4Source = axisList;

                Axis4 = Contains(axisList, oldAxis) ? oldAxis : null;

                RaisePropertyChanged("Axis4");
            }
        }

        private void UpdateAxis4MotorFeedbackSource()
        {
            if (Axis4Visibility == Visibility.Visible)
            {
                var supportedPortList =
                    ModifiedMotionDrive.Profiles.GetSupportedMotorFeedbackPortList(4, ModifiedMotionDrive.Major);
                if (supportedPortList != null && supportedPortList.Count > 0)
                {
                    Axis4MotorFeedbackSource = PortListToDisplayItemList(supportedPortList);
                }
                else
                {
                    Axis4MotorFeedbackSource = null;
                }
            }
            else
            {
                Axis4MotorFeedbackSource = null;
            }
        }

        private List<DisplayItem<ITag>> GetAxisSourceByIndex(int axisIndex)
        {
            var axisList = new List<DisplayItem<ITag>>();

            var controller = ModifiedMotionDrive.Controller;
            if (controller != null)
            {
                foreach (var t in controller.Tags)
                {
                    Tag tag = t as Tag;
                    AxisCIPDrive axisCIPDrive = tag?.DataWrapper as AxisCIPDrive;
                    if (axisCIPDrive != null)
                    {
                        if (axisCIPDrive.AssociatedModule == null)
                            axisList.Add(new DisplayItem<ITag>()
                            {
                                DisplayName = t.Name,
                                Value = t
                            });
                    }
                }
            }

            var axis = OriginalMotionDrive.GetAxis(axisIndex);
            if (axis != null)
            {
                axisList.Add(new DisplayItem<ITag>()
                {
                    DisplayName = axis.Name,
                    Value = axis
                });
            }

            axisList.Sort((x, y) => string.Compare(x.DisplayName, y.DisplayName, StringComparison.Ordinal));

            // insert null 
            axisList.Insert(0, new DisplayItem<ITag> {DisplayName = "<none>", Value = null});

            return axisList;
        }

        private bool Contains(List<DisplayItem<ITag>> list, ITag value)
        {
            if (list == null)
                return false;

            foreach (var item in list)
            {
                if (ReferenceEquals(item.Value, value))
                    return true;
            }

            return false;
        }

        private IList PortListToDisplayItemList(List<int> portList)
        {
            var displayItemList = new List<DisplayItem<int>>();

            foreach (var port in portList)
            {
                var displayItem = new DisplayItem<int>
                {
                    Value = port,
                    DisplayName = ModifiedMotionDrive.Profiles.GetPortDescription(port)
                };

                displayItemList.Add(displayItem);
            }

            return displayItemList;
        }

        private void UIRefresh()
        {
            RaisePropertyChanged("Axis1");
            RaisePropertyChanged("Axis2");
            RaisePropertyChanged("Axis3");
            RaisePropertyChanged("Axis4");

            NewAxisCommand.RaiseCanExecuteChanged();
            AxisPropertiesCommand.RaiseCanExecuteChanged();
        }

        private void CreateMonitorAxisList()
        {
            _monitorAxisList = new List<ITag>();

            var controller = ModifiedMotionDrive.Controller;
            if (controller != null)
            {
                foreach (var t in controller.Tags)
                {
                    Tag tag = t as Tag;
                    AxisCIPDrive axisCIPDrive = tag?.DataWrapper as AxisCIPDrive;
                    if (axisCIPDrive != null)
                    {
                        tag.PropertyChanged += AxisOnPropertyChanged;
                        axisCIPDrive.AssociatedChannelChanged += AxisCIPDriveOnAssociatedChannelChanged;

                        _monitorAxisList.Add(tag);
                    }
                }
            }

        }

        private void AxisCIPDriveOnAssociatedChannelChanged(
            object sender, AssociatedChannelChangedEventArgs e)
        {
            if (e.OldModule == OriginalMotionDrive)
            {
                ModifiedMotionDrive.AssociatedAxes[e.OldNumber - 1] = OriginalMotionDrive.GetAxis(e.OldNumber);
            }

            if (e.NewModule == OriginalMotionDrive)
            {
                ModifiedMotionDrive.AssociatedAxes[e.NewNumber - 1] = OriginalMotionDrive.GetAxis(e.NewNumber);
            }

            UpdateAxis1Source();
            UpdateAxis2Source();
            UpdateAxis3Source();
            UpdateAxis4Source();

            AxisPropertiesCommand.RaiseCanExecuteChanged();
            NewAxisCommand.RaiseCanExecuteChanged();
        }

        private void AxisOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                UpdateAxis1Source();
                UpdateAxis2Source();
                UpdateAxis3Source();
                UpdateAxis4Source();
            }

        }

        private void TagsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateAxis1Source();
            UpdateAxis2Source();
            UpdateAxis3Source();
            UpdateAxis4Source();

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ITag item in e.NewItems)
                {
                    Tag tag = item as Tag;
                    AxisCIPDrive axisCIPDrive = tag?.DataWrapper as AxisCIPDrive;
                    if (axisCIPDrive != null)
                    {
                        tag.PropertyChanged += AxisOnPropertyChanged;
                        axisCIPDrive.AssociatedChannelChanged += AxisCIPDriveOnAssociatedChannelChanged;

                        _monitorAxisList.Add(tag);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ITag item in e.OldItems)
                {
                    if (_monitorAxisList.Contains(item))
                    {
                        item.PropertyChanged -= AxisOnPropertyChanged;

                        Tag tag = item as Tag;
                        AxisCIPDrive axisCIPDrive = tag?.DataWrapper as AxisCIPDrive;
                        if (axisCIPDrive != null)
                        {
                            axisCIPDrive.AssociatedChannelChanged -= AxisCIPDriveOnAssociatedChannelChanged;
                        }

                        _monitorAxisList.Remove(item);
                    }
                }
            }
        }

        #endregion


    }
}
