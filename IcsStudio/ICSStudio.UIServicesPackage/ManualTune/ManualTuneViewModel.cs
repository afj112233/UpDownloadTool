using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.Objects;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIServicesPackage.ManualTune.ViewModel;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using NLog;
using Task = System.Threading.Tasks.Task;
using ICSStudio.MultiLanguage;

namespace ICSStudio.UIServicesPackage.ManualTune
{
    public class ManualTuneViewModel : ViewModelBase
    {
        internal static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly Timer _axisUpdateTimer;

        private bool _manualTuneChecked;
        private Visibility _manualTuneVisibility;

        private bool _additionalTuneChecked;
        private Visibility _additionalTuneVisibility;

        private List<string> _comparePropertiesList;

        public ManualTuneViewModel(ITag axisTag)
        {
            Contract.Assert(axisTag != null);

            AxisTag = axisTag;

            var tag = axisTag as Tag;
            var axisCIPDrive = tag?.DataWrapper as AxisCIPDrive;
            Contract.Assert(axisCIPDrive != null);

            OriginalAxisCIPDrive = axisCIPDrive;
            ModifiedAxisCIPDrive = (AxisCIPDrive)axisCIPDrive.Clone();

            InitializeComparePropertiesList();

            _manualTuneChecked = true;
            _manualTuneVisibility = Visibility.Visible;

            _additionalTuneChecked = false;
            _additionalTuneVisibility = Visibility.Collapsed;

            ManualTuningViewModel = new ManualTuningViewModel(this);
            AdditionalTuneViewModel = new AdditionalTuneViewModel(this);

            MotionGeneratorViewModel = new MotionGeneratorViewModel(AxisTag);

            CloseCommand = new RelayCommand(ExecuteCloseCommand);
            HelpCommand = new RelayCommand(ExecuteHelpCommand);

            Controller = axisTag.ParentController as Controller;
            if (Controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    Controller, "IsOnlineChanged", OnIsOnlineChanged);
            }

            PropertyChangedEventManager.AddHandler(
                AxisTag, OnAxisTagPropertyChanged, string.Empty);

            _axisUpdateTimer = new Timer(100);
            _axisUpdateTimer.Elapsed += CycleUpdateAxis;

            if (IsOnLine)
            {
                int instanceId = Controller.GetTagId(AxisTag);

                OriginalAxisCIPDrive.CIPAxis.InstanceId = instanceId;
                OriginalAxisCIPDrive.CIPAxis.Messager = Controller.CipMessager;
                ModifiedAxisCIPDrive.CIPAxis.InstanceId = instanceId;
                ModifiedAxisCIPDrive.CIPAxis.Messager = Controller.CipMessager;

                _axisUpdateTimer.Start();
            }

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("Title");
        }

        public ITag AxisTag { get; }
        public Controller Controller { get; }

        public AxisCIPDrive OriginalAxisCIPDrive { get; }
        public AxisCIPDrive ModifiedAxisCIPDrive { get; }

        public bool IsOnLine => Controller != null && Controller.IsOnline;

        public Action CloseAction { get; set; }
        public string Title => LanguageManager.GetInstance().ConvertSpecifier("Motion Console -") + " " + AxisTag.Name;

        public bool ManualTuneChecked
        {
            get { return _manualTuneChecked; }
            set
            {
                Set(ref _manualTuneChecked, value);
                ManualTuneVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility ManualTuneVisibility
        {
            get { return _manualTuneVisibility; }
            set { Set(ref _manualTuneVisibility, value); }
        }

        public bool AdditionalTuneChecked
        {
            get { return _additionalTuneChecked; }
            set
            {
                Set(ref _additionalTuneChecked, value);
                AdditionalTuneVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility AdditionalTuneVisibility
        {
            get { return _additionalTuneVisibility; }
            set { Set(ref _additionalTuneVisibility, value); }
        }

        public string[] PeriodicRefreshProperties { get; } =
        {
            "PositionLoopBandwidth", "PositionIntegratorBandwidth",
            "PositionIntegratorControl",
            "PositionErrorTolerance",

            "VelocityLoopBandwidth","VelocityIntegratorBandwidth",
            "VelocityIntegratorControl",
            "VelocityErrorTolerance",

            "VelocityFeedforwardGain","AccelerationFeedforwardGain",

            "SystemInertia","TorqueOffset",
            "FrictionCompensationSliding",
            "FrictionCompensationWindow",
            "BacklashCompensationWindow",
            "LoadObserverConfiguration",
            "LoadObserverBandwidth",
            "LoadObserverIntegratorBandwidth",

            "TorqueLowPassFilterBandwidth",
            "TorqueNotchFilterFrequency",
            "TorqueLeadLagFilterGain",
            "TorqueLeadLagFilterBandwidth",
            "AdaptiveTuningConfiguration",
            "TorqueNotchFilterHighFrequencyLimit",
            "TorqueNotchFilterLowFrequencyLimit",
            "TorqueNotchFilterTuningThreshold",

            "TorqueLimitPositive","TorqueLimitNegative",
            "AccelerationLimit","DecelerationLimit",
            "VelocityLimitPositive",
            "VelocityLimitNegative",

            "MaximumSpeed","MaximumAcceleration","MaximumDeceleration",
            "MaximumAccelerationJerk","MaximumDecelerationJerk",
        };

        public ManualTuningViewModel ManualTuningViewModel { get; }
        public MotionGeneratorViewModel MotionGeneratorViewModel { get; }
        public AdditionalTuneViewModel AdditionalTuneViewModel { get; }

        public RelayCommand CloseCommand { get; }
        public RelayCommand HelpCommand { get; }

        public override void Cleanup()
        {
            if (_axisUpdateTimer != null)
            {
                _axisUpdateTimer.Stop();
                _axisUpdateTimer.Elapsed -= CycleUpdateAxis;
            }
        }

        #region Command

        private void ExecuteHelpCommand()
        {
            //TODO(gjc): add code here
        }

        private void ExecuteCloseCommand()
        {
            CloseAction?.Invoke();
        }


        #endregion

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (IsOnLine)
                {
                    int instanceId = Controller.GetTagId(AxisTag);

                    OriginalAxisCIPDrive.CIPAxis.InstanceId = instanceId;
                    OriginalAxisCIPDrive.CIPAxis.Messager = Controller.CipMessager;
                    ModifiedAxisCIPDrive.CIPAxis.InstanceId = instanceId;
                    ModifiedAxisCIPDrive.CIPAxis.Messager = Controller.CipMessager;
                }

                if (IsOnLine)
                    _axisUpdateTimer?.Start();
                else
                    _axisUpdateTimer?.Stop();

                Refresh();

            });
        }

        private void Refresh()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                ManualTuningViewModel.Refresh();

                AdditionalTuneViewModel.Refresh();
            });
        }

        private void OnAxisTagPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_comparePropertiesList.Contains(e.PropertyName))
            {
                ModifiedAxisCIPDrive.CIPAxis.Apply(OriginalAxisCIPDrive.CIPAxis,
                    CipAttributeHelper.AttributeNamesToIdList<CIPAxis>(new[] { e.PropertyName }));

                Refresh();
            }

        }

        private void InitializeComparePropertiesList()
        {
            _comparePropertiesList = new List<string>()
            {
                "PositionUnits",
                "SystemBandwidth", "SystemDamping",
                "ApplicationType", "LoadCoupling",
                "GainTuningConfigurationBits",
                "PositionLoopBandwidth", "PositionIntegratorBandwidth",
                "PositionIntegratorControl", "PositionErrorTolerance",
                "VelocityLoopBandwidth", "VelocityIntegratorBandwidth",
                "VelocityIntegratorControl", "VelocityErrorTolerance",

                "VelocityFeedforwardGain",
                "AccelerationFeedforwardGain",
                "SystemInertia",
                "TorqueOffset",
                "FrictionCompensationSliding",
                "FrictionCompensationWindow",
                "BacklashCompensationWindow",
                "LoadObserverConfiguration",
                "LoadObserverBandwidth",
                "LoadObserverIntegratorBandwidth",
                "TorqueLowPassFilterBandwidth",
                "TorqueNotchFilterFrequency",
                "TorqueLeadLagFilterGain",
                "TorqueLeadLagFilterBandwidth",
                "AdaptiveTuningConfiguration",
                "TorqueNotchFilterHighFrequencyLimit",
                "TorqueNotchFilterLowFrequencyLimit",
                "TorqueNotchFilterTuningThreshold",
                "TorqueLimitPositive",
                "TorqueLimitNegative",
                "AccelerationLimit",
                "DecelerationLimit",
                "VelocityLimitPositive",
                "VelocityLimitNegative",
                "MaximumSpeed",
                "MaximumAcceleration",
                "MaximumDeceleration",
                "MaximumAccelerationJerk",
                "MaximumDecelerationJerk"
            };
        }

        internal void SetAttributeSingle([CallerMemberName] string propertyName = null)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await TaskScheduler.Default;

                if (IsOnLine)
                {
                    try
                    {
                        //TODO(gjc): need check here
                        // stop refresh

                        //
                        if (propertyName == "PositionIntegratorHold")
                        {
                            propertyName = "PositionIntegratorControl";
                        }

                        if (propertyName == "VelocityIntegratorHold")
                        {
                            propertyName = "VelocityIntegratorControl";
                        }

                        Logger.Trace(
                            $"Manual Set, {AxisTag.Name}.{propertyName}, value:{ModifiedAxisCIPDrive.CIPAxis.GetAttributeValueString(propertyName)}");

                        await ModifiedAxisCIPDrive.CIPAxis.SetAttributeSingle(propertyName);

                        await Task.Delay(100);

                        await OriginalAxisCIPDrive.CIPAxis.GetAttributeSingle(propertyName);

                        Logger.Trace(
                            $"Manual Get, {AxisTag.Name}.{propertyName}, value:{OriginalAxisCIPDrive.CIPAxis.GetAttributeValueString(propertyName)}");

                        // start refresh
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                    finally
                    {
                        OriginalAxisCIPDrive.NotifyParentPropertyChanged(propertyName);
                    }

                }

            });
        }

        private void CycleUpdateAxis(object sender, ElapsedEventArgs e)
        {
            TagSyncController tagSyncController
                = Controller?.Lookup(typeof(TagSyncController)) as TagSyncController;

            if (tagSyncController != null)
            {
                tagSyncController.Update(AxisTag, AxisTag.Name);

                foreach (var property in PeriodicRefreshProperties)
                {
                    tagSyncController.UpdateAxisProperties(AxisTag, property);
                }
            }
        }
    }
}
