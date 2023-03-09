using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using ICSStudio.DeviceProfiles.DIOModule;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties.AnalogIOs.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class LimitsConfigViewModel : DeviceOptionPanel
    {
        private int _channelIndex;

        private short _highLimit;
        private short _lowLimit;

        private bool _alarmDisable;
        private bool _limitAlarmLatch;

        public LimitsConfigViewModel(UserControl panel, ModifiedAnalogIO modifiedAnalogIO) : base(panel)
        {
            _channelIndex = 0;

            ModifiedAnalogIO = modifiedAnalogIO;

            ChangeChannelCommand = new RelayCommand<int>(ExecuteChangeChannel);
        }

        public ModifiedAnalogIO ModifiedAnalogIO { get; }

        public AnalogIO OriginalAnalogIO => ModifiedAnalogIO?.OriginalAnalogIO;

        public DIOModuleProfiles Profiles => OriginalAnalogIO?.Profiles;

        public RelayCommand<int> ChangeChannelCommand { get; }

        public int ChannelIndex => _channelIndex;

        public bool IsLimitsConfigEnabled => !IsOnline;

        public short HighLimit
        {
            get
            {
                _highLimit = ModifiedAnalogIO.GetHighLimit(_channelIndex);
                return _highLimit;
            }
            set
            {
                if (_highLimit != value)
                {
                    Set(ref _highLimit, value);

                    ModifiedAnalogIO.SetHighLimit(_channelIndex, _highLimit);

                    IsDirty = true;
                }
            }
        }

        public short LowLimit
        {
            get
            {
                _lowLimit = ModifiedAnalogIO.GetLowLimit(_channelIndex);
                return _lowLimit;
            }
            set
            {
                if (_lowLimit != value)
                {
                    Set(ref _lowLimit, value);

                    ModifiedAnalogIO.SetLowLimit(_channelIndex, _lowLimit);

                    IsDirty = true;
                }
            }
        }

        public bool AlarmDisable
        {
            get
            {
                _alarmDisable = ModifiedAnalogIO.GetAlarmDisable(_channelIndex) != 0;
                return _alarmDisable;
            }
            set
            {
                if (_alarmDisable != value)
                {
                    Set(ref _alarmDisable, value);

                    ModifiedAnalogIO.SetAlarmDisable(_channelIndex, (sbyte) (_alarmDisable ? 1 : 0));

                    IsDirty = true;

                    RaisePropertyChanged("LimitAlarmLatchEnabled");
                }
            }
        }

        public bool LimitAlarmLatch
        {
            get
            {
                _limitAlarmLatch = ModifiedAnalogIO.GetLimitAlarmLatch(_channelIndex) != 0;
                return _limitAlarmLatch;
            }
            set
            {
                if (_limitAlarmLatch != value)
                {
                    Set(ref _limitAlarmLatch, value);

                    ModifiedAnalogIO.SetLimitAlarmLatch(_channelIndex, (sbyte) (_limitAlarmLatch ? 1 : 0));

                    IsDirty = true;
                }
            }
        }

        public bool LimitAlarmLatchEnabled
        {
            get
            {
                if (AlarmDisable)
                    return false;

                return true;
            }
        }

        public override Visibility Visibility
        {
            get
            {
                // listen only return Visibility.Collapsed
                if (Profiles
                    .GetConnectionStringByConfigID(ModifiedAnalogIO.ConnectionConfigID, ModifiedAnalogIO.Major)
                    .Contains("Listen Only"))
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        public override void Show()
        {
            RaisePropertyChanged("HighLimit");
            RaisePropertyChanged("LowLimit");

            RaisePropertyChanged("AlarmDisable");
            RaisePropertyChanged("LimitAlarmLatch");
            RaisePropertyChanged("LimitAlarmLatchEnabled");
        }

        public override bool SaveOptions()
        {
            if (Visibility == Visibility.Visible)
                if (OriginalAnalogIO.ConfigTag != null)
                {
                    //TODO(gjc): add check

                    var updateList = CreateUpdateList();
                    foreach (var tuple in updateList)
                        OriginalAnalogIO.ConfigTag.SetStringValue(tuple.Item1, tuple.Item2);
                }


            IsDirty = false;

            return true;
        }

        public override int CheckValid()
        {
            int result = 0;

            var module = Profiles.GetModule(ModifiedAnalogIO.Major);
            if (module != null)
            {
                int index;
                for (index = 0; index < module.NumberOfOutputs; index++)
                {
                    short highLimit = ModifiedAnalogIO.GetHighLimit(index);
                    short lowLimit = ModifiedAnalogIO.GetLowLimit(index);

                    if (highLimit < lowLimit)
                    {
                        result = -1;
                        break;
                    }
                }

                if (result < 0)
                {
                    string warningReason = LanguageManager.GetInstance().ConvertSpecifier("High Alarm must be greater than Low Alarm.") + $"\nChannel {index}";
                    MessageBox.Show(warningReason, "ICS Studio",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    ExecuteChangeChannel(index);
                }
            }

            return result;
        }

        public override void Refresh()
        {
            base.Refresh();
            RaisePropertyChanged(nameof(IsLimitsConfigEnabled));
        }

        private List<Tuple<string, string>> CreateUpdateList()
        {
            var updateList = new List<Tuple<string, string>>();

            var module = Profiles.GetModule(ModifiedAnalogIO.Major);
            if (module != null)
            {
                for (var i = 0; i < module.NumberOfOutputs; i++)
                {
                    updateList.Add(new Tuple<string, string>($"Ch{i}HighLimit",
                        ModifiedAnalogIO.GetHighLimit(i).ToString()));
                    updateList.Add(new Tuple<string, string>($"Ch{i}LowLimit",
                        ModifiedAnalogIO.GetLowLimit(i).ToString()));


                    updateList.Add(new Tuple<string, string>($"Ch{i}AlarmDisable",
                        ModifiedAnalogIO.GetAlarmDisable(i).ToString()));
                    updateList.Add(new Tuple<string, string>($"Ch{i}LimitAlarmLatch",
                        ModifiedAnalogIO.GetLimitAlarmLatch(i).ToString()));
                }
            }

            return updateList;
        }

        private void ExecuteChangeChannel(int index)
        {
            _channelIndex = index;

            RaisePropertyChanged("ChannelIndex");
            RaisePropertyChanged("HighLimit");
            RaisePropertyChanged("LowLimit");

            RaisePropertyChanged("AlarmDisable");
            RaisePropertyChanged("LimitAlarmLatch");
            RaisePropertyChanged("LimitAlarmLatchEnabled");
        }
    }
}
