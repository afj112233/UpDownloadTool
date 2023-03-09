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
    public class AlarmConfigViewModel : DeviceOptionPanel
    {
        private int _channelIndex;

        private short _hhAlarmLimit; //0x6E
        private short _hAlarmLimit; //0x6C
        private short _lAlarmLimit; //0x6B
        private short _llAlarmLimit; //0x6D

        private bool _alarmDisable;
        private bool _limitAlarmLatch;

        public AlarmConfigViewModel(UserControl panel, ModifiedAnalogIO modifiedAnalogIO) : base(panel)
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

        public bool IsAlarmConfigEnabled => !IsOnline;

        public short HHAlarmLimit
        {
            get
            {
                _hhAlarmLimit = ModifiedAnalogIO.GetHHAlarmLimit(_channelIndex);
                return _hhAlarmLimit;
            }
            set
            {
                if (_hhAlarmLimit != value)
                {
                    Set(ref _hhAlarmLimit, value);

                    ModifiedAnalogIO.SetHHAlarmLimit(_channelIndex, _hhAlarmLimit);

                    IsDirty = true;
                }
            }
        }

        public short HAlarmLimit
        {
            get
            {
                _hAlarmLimit = ModifiedAnalogIO.GetHAlarmLimit(_channelIndex);
                return _hAlarmLimit;
            }
            set
            {
                if (_hAlarmLimit != value)
                {
                    Set(ref _hAlarmLimit, value);

                    ModifiedAnalogIO.SetHAlarmLimit(_channelIndex, _hAlarmLimit);

                    IsDirty = true;
                }
            }
        }

        public short LAlarmLimit
        {
            get
            {
                _lAlarmLimit = ModifiedAnalogIO.GetLAlarmLimit(_channelIndex);
                return _lAlarmLimit;
            }
            set
            {
                if (_lAlarmLimit != value)
                {
                    Set(ref _lAlarmLimit, value);

                    ModifiedAnalogIO.SetLAlarmLimit(_channelIndex, _lAlarmLimit);

                    IsDirty = true;
                }
            }
        }

        public short LLAlarmLimit
        {
            get
            {
                _llAlarmLimit = ModifiedAnalogIO.GetLLAlarmLimit(_channelIndex);
                return _llAlarmLimit;
            }
            set
            {
                if (_llAlarmLimit != value)
                {
                    Set(ref _llAlarmLimit, value);

                    ModifiedAnalogIO.SetLLAlarmLimit(_channelIndex, _llAlarmLimit);

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

                    ModifiedAnalogIO.SetAlarmDisable(_channelIndex, (sbyte)(_alarmDisable ? 1 : 0));

                    IsDirty = true;

                    RaisePropertyChanged(nameof(LimitAlarmLatchEnabled));
                    RaisePropertyChanged(nameof(IsAlarmEnabled));
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

                    ModifiedAnalogIO.SetLimitAlarmLatch(_channelIndex, (sbyte)(_limitAlarmLatch ? 1 : 0));

                    IsDirty = true;
                }
            }
        }

        public bool LimitAlarmLatchEnabled => !AlarmDisable;

        public bool IsAlarmEnabled => !AlarmDisable;

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
            RaisePropertyChanged("HHAlarmLimit");
            RaisePropertyChanged("HAlarmLimit");
            RaisePropertyChanged("LAlarmLimit");
            RaisePropertyChanged("LLAlarmLimit");

            RaisePropertyChanged("AlarmDisable");
            RaisePropertyChanged("LimitAlarmLatch");

            RaisePropertyChanged(nameof(LimitAlarmLatchEnabled));
            RaisePropertyChanged(nameof(IsAlarmEnabled));
        }

        public override bool SaveOptions()
        {
            if (Visibility == Visibility.Visible)
                if (OriginalAnalogIO.ConfigTag != null)
                {
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

            string warningReason = string.Empty;

            var module = Profiles.GetModule(ModifiedAnalogIO.Major);
            if (module != null)
            {
                int index = 0;
                for (index = 0; index < module.NumberOfInputs; index++)
                {
                    short hhAlarmLimit = ModifiedAnalogIO.GetHHAlarmLimit(index);
                    short hAlarmLimit = ModifiedAnalogIO.GetHAlarmLimit(index);
                    short lAlarmLimit = ModifiedAnalogIO.GetLAlarmLimit(index);
                    short llAlarmLimit = ModifiedAnalogIO.GetLLAlarmLimit(index);

                    if (hhAlarmLimit < hAlarmLimit)
                    {
                        warningReason = "High High Alarm must be greater than High Alarm.";
                        result = -1;
                        break;
                    }

                    if (hAlarmLimit < lAlarmLimit)
                    {
                        warningReason = "High Alarm must be greater than Low Alarm.";
                        result = -2;
                        break;
                    }

                    if (lAlarmLimit < llAlarmLimit)
                    {
                        warningReason = "Low Alarm must be greater than Low Low Alarm.";
                        result = -3;
                        break;
                    }

                }

                if (result < 0)
                {
                    MessageBox.Show($"Channel {index}: " + LanguageManager.GetInstance().ConvertSpecifier(warningReason), "ICS Studio",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    ExecuteChangeChannel(index);
                }

            }

            return result;
        }

        public override void Refresh()
        {
            base.Refresh();
            RaisePropertyChanged(nameof(IsAlarmConfigEnabled));
        }

        private List<Tuple<string, string>> CreateUpdateList()
        {
            var updateList = new List<Tuple<string, string>>();

            var module = Profiles.GetModule(ModifiedAnalogIO.Major);
            if (module != null)
            {
                for (var i = 0; i < module.NumberOfInputs; i++)
                {
                    updateList.Add(new Tuple<string, string>($"Ch{i}HHAlarmLimit",
                        ModifiedAnalogIO.GetHHAlarmLimit(i).ToString()));
                    updateList.Add(new Tuple<string, string>($"Ch{i}HAlarmLimit",
                        ModifiedAnalogIO.GetHAlarmLimit(i).ToString()));
                    updateList.Add(new Tuple<string, string>($"Ch{i}LAlarmLimit",
                        ModifiedAnalogIO.GetLAlarmLimit(i).ToString()));
                    updateList.Add(new Tuple<string, string>($"Ch{i}LLAlarmLimit",
                        ModifiedAnalogIO.GetLLAlarmLimit(i).ToString()));

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

            RaisePropertyChanged("HHAlarmLimit");
            RaisePropertyChanged("HAlarmLimit");
            RaisePropertyChanged("LAlarmLimit");
            RaisePropertyChanged("LLAlarmLimit");

            RaisePropertyChanged("AlarmDisable");
            RaisePropertyChanged("LimitAlarmLatch");

            RaisePropertyChanged(nameof(LimitAlarmLatchEnabled));
            RaisePropertyChanged(nameof(IsAlarmEnabled));
        }
    }
}
