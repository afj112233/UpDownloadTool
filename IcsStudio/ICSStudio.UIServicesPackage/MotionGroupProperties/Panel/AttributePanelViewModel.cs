using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Dialog;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;

namespace ICSStudio.UIServicesPackage.MotionGroupProperties.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class AttributePanelViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private readonly MotionGroup _mg;
        private readonly DispatcherTimer _timer;

        private bool _isDirty;

        private GeneralFaultType _selected;

        private string _scanMax;
        private string _scanLast;
        private string _scanAverage;

        private bool _baseUpdatePEnabled;
        private string _baseUpdateP;
        private bool _baseUpdateStatus;

        public AttributePanelViewModel(AttributePanel panel, ITag motionGroup)
        {
            Control = panel;
            panel.DataContext = this;

            MotionGroup = motionGroup;

            Tag tag = motionGroup as Tag;
            _mg = tag?.DataWrapper as MotionGroup;

            if (_mg != null)
            {
                BaseUpdateP = (_mg.CoarseUpdatePeriod / 1000f).ToString("f1");
                Alternate1UpdateMultiplier =
                    (_mg.Alternate1UpdateMultiplier * _mg.CoarseUpdatePeriod / 1000f).ToString("f1");
                Alternate2UpdateMultiplier =
                    (_mg.Alternate2UpdateMultiplier * _mg.CoarseUpdatePeriod / 1000f).ToString("f1");

                GeneralFaultType = EnumHelper.ToDataSource<GeneralFaultType>();
                Selected = _mg.GeneralFaultType;
            }

            if (tag != null)
                tag.PropertyChanged += AssignedGroupOnPropertyChanged;

            BaseUpdatePEnabled = !IsOnline;

            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                (Controller) tag?.ParentController, "IsOnlineChanged", OnIsOnlineChanged);


            AxisScheduleCommand = new RelayCommand(ExecuteAxisScheduleCommand);
            ResetMaxCommand = new RelayCommand(ExecuteResetMaxCommand, CanResetMaxCommand);

            _timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(500)};
            _timer.Tick += _timer_Tick;
            if (IsOnline)
                _timer.Start();
        }

        public ITag MotionGroup { get; }

        public bool IsOnline => MotionGroup.ParentController.IsOnline;

        public bool BaseUpdatePEnabled
        {
            set
            {
                Set(ref _baseUpdatePEnabled, value);
                BaseUpdateStatus = !value;
            }
            get { return _baseUpdatePEnabled; }
        }

        public bool BaseUpdateStatus
        {
            set { Set(ref _baseUpdateStatus, value); }
            get { return _baseUpdateStatus; }
        }

        public string ScanMax
        {
            set { Set(ref _scanMax, value); }
            get { return _scanMax; }
        }

        public string ScanLast
        {
            set { Set(ref _scanLast, value); }
            get { return _scanLast; }
        }

        public string ScanAverage
        {
            set { Set(ref _scanAverage, value); }
            get { return _scanAverage; }
        }

        public override void Cleanup()
        {
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                (Controller) _mg.ParentTag.ParentController, "IsOnlineChanged", OnIsOnlineChanged);

            var tag = MotionGroup as Tag;
            if (tag != null)
                tag.PropertyChanged -= AssignedGroupOnPropertyChanged;

            _timer.Stop();
        }

        public GeneralFaultType Selected
        {
            set
            {
                _selected = value;
                Compare();
            }
            get { return _selected; }
        }

        public void Compare()
        {
            IsDirty = Selected != _mg.GeneralFaultType;
            IsDirty = BaseUpdateP != (_mg.CoarseUpdatePeriod / 1000f).ToString("f1");
        }

        public IList GeneralFaultType { set; get; }

        public string BaseUpdateP
        {
            set
            {
                _baseUpdateP = value;
                Compare();
            }
            get { return _baseUpdateP; }
        }

        public string Alternate1UpdateMultiplier { set; get; }
        public string Alternate2UpdateMultiplier { set; get; }
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

        public RelayCommand AxisScheduleCommand { get; }

        public RelayCommand ResetMaxCommand { get; }

        #region Private

        private void ExecuteAxisScheduleCommand()
        {
            ICreateDialogService createDialogService =
                Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;

            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = (IVsUIShell) Package.GetGlobalService(typeof(SVsUIShell));

            if (createDialogService != null)
            {
                var window = createDialogService.CreateAxisScheduleDialog(MotionGroup);
                window.Show(uiShell);
            }
        }

        private void _timer_Tick(object sender, EventArgs e)
        {

            Controller controller = MotionGroup?.ParentController as Controller;
            TagSyncController tagSyncController
                = controller?.Lookup(typeof(TagSyncController)) as TagSyncController;
            tagSyncController?.Update(MotionGroup, MotionGroup.Name);


            Tag tag = null;
            ScanMax = ObtainValue.GetTagValue($"{MotionGroup.Name}.TaskMaxScanTime",
                MotionGroup.ParentCollection.ParentProgram as IProgram, null, ref tag);
            ScanLast = ObtainValue.GetTagValue($"{MotionGroup.Name}.TaskLastScanTime",
                MotionGroup.ParentCollection.ParentProgram as IProgram, null, ref tag);
            ScanAverage = ObtainValue.GetTagValue($"{MotionGroup.Name}.TaskAverageScanTime",
                MotionGroup.ParentCollection.ParentProgram as IProgram, null, ref tag);
        }

        private bool CanResetMaxCommand()
        {
            return IsOnline;
        }

        private void ExecuteResetMaxCommand()
        {
            if (IsOnline)
            {
                SetTagValueToPLC(MotionGroup, $"{MotionGroup.Name}.TaskMaxScanTime", "0");
            }
        }

        private void SetTagValueToPLC(ITag tag, string specifier, string value)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await TaskScheduler.Default;

                Controller controller = MotionGroup.ParentController as Controller;
                if (controller != null)
                    await controller.SetTagValueToPLC(tag, specifier, value);
            });
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (IsOnline)
                {
                    _timer.Start();
                    BaseUpdatePEnabled = false;
                }
                else
                {
                    _timer.Stop();
                    ScanMax = "";
                    ScanLast = "";
                    ScanAverage = "";
                    BaseUpdatePEnabled = true;
                }

                ResetMaxCommand.RaiseCanExecuteChanged();

            });
        }

        private void AssignedGroupOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CoarseUpdatePeriod" || e.PropertyName == "Alternate1UpdateMultiplier" ||
                e.PropertyName == "Alternate2UpdateMultiplier" || e.PropertyName == "AssignedGroup")
            {
                BaseUpdateP = (_mg.CoarseUpdatePeriod / 1000f).ToString("f1");
                Alternate1UpdateMultiplier =
                    (_mg.Alternate1UpdateMultiplier * _mg.CoarseUpdatePeriod / 1000f).ToString("f1");
                Alternate2UpdateMultiplier =
                    (_mg.Alternate2UpdateMultiplier * _mg.CoarseUpdatePeriod / 1000f).ToString("f1");

                RaisePropertyChanged("BaseUpdateP");
                RaisePropertyChanged("Alternate1UpdateMultiplier");
                RaisePropertyChanged("Alternate2UpdateMultiplier");
            }
        }

        #endregion
    }
}
