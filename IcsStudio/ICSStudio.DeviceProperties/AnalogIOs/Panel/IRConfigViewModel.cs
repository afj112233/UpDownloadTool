using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using ICSStudio.DeviceProfiles.DIOModule;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.Gui.Utils;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties.AnalogIOs.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class IRConfigViewModel : DeviceOptionPanel
    {
        private sbyte _notchFilter;

        public IRConfigViewModel(UserControl panel, ModifiedAnalogIO modifiedAnalogIO) : base(panel)
        {
            ModifiedAnalogIO = modifiedAnalogIO;

            CreateInputConfigSource();

            _notchFilter = ModifiedAnalogIO.GetNotchFilter();

            CreateNotchFilterSource();
        }

        public ModifiedAnalogIO ModifiedAnalogIO { get; }

        public AnalogIO OriginalAnalogIO => ModifiedAnalogIO?.OriginalAnalogIO;

        public DIOModuleProfiles Profiles => OriginalAnalogIO?.Profiles;

        public bool IsIRConfigEnabled => !IsOnline;

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

        public override bool SaveOptions()
        {
            UpdateInputConfigSource();

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

        public override void Show()
        {
            UpdateInputConfigSource();

            _notchFilter = ModifiedAnalogIO.GetNotchFilter();

            RaisePropertyChanged("NotchFilter");
        }

        public override void CheckDirty()
        {
            //ignore
        }

        public override void Refresh()
        {
            base.Refresh();
            RaisePropertyChanged(nameof(IsIRConfigEnabled));
        }

        public ObservableCollection<IRChannelItem> InputConfigSource { get; set; }

        public sbyte NotchFilter
        {
            get { return _notchFilter; }
            set
            {
                if (_notchFilter != value)
                {
                    Set(ref _notchFilter, value);

                    ModifiedAnalogIO.SetNotchFilter(_notchFilter);

                    IsDirty = true;
                }
            }
        }

        public List<DisplayItem<sbyte>> NotchFilterSource { get; private set; }

        private void CreateNotchFilterSource()
        {
            NotchFilterSource = new List<DisplayItem<sbyte>>();

            var module = Profiles.GetModule(ModifiedAnalogIO.Major);
            if (module != null)
            {
                NotchFilterSource.Add(new DisplayItem<sbyte>() { DisplayName = "50Hz", Value = 0 });
                NotchFilterSource.Add(new DisplayItem<sbyte>() { DisplayName = "60Hz", Value = 1 });
                NotchFilterSource.Add(new DisplayItem<sbyte>() { DisplayName = "100Hz", Value = 2 });
                NotchFilterSource.Add(new DisplayItem<sbyte>() { DisplayName = "120Hz", Value = 3 });
                NotchFilterSource.Add(new DisplayItem<sbyte>() { DisplayName = "200Hz", Value = 4 });
                NotchFilterSource.Add(new DisplayItem<sbyte>() { DisplayName = "240Hz", Value = 5 });
                NotchFilterSource.Add(new DisplayItem<sbyte>() { DisplayName = "300Hz", Value = 6 });
                NotchFilterSource.Add(new DisplayItem<sbyte>() { DisplayName = "400Hz", Value = 7 });
                NotchFilterSource.Add(new DisplayItem<sbyte>() { DisplayName = "480Hz", Value = 8 });

            }
        }

        private void CreateInputConfigSource()
        {
            InputConfigSource = new ObservableCollection<IRChannelItem>();

            var module = Profiles.GetModule(ModifiedAnalogIO.Major);

            if (module != null)
            {
                for (var i = 0; i < module.NumberOfInputs; i++)
                {
                    sbyte sensorType = ModifiedAnalogIO.GetSensorType(i);
                    sbyte tempMode = ModifiedAnalogIO.GetTempMode(i);
                    short highEngineering = ModifiedAnalogIO.GetHighEngineering(i);
                    short lowEngineering = ModifiedAnalogIO.GetLowEngineering(i);
                    short digitalFilter = ModifiedAnalogIO.GetDigitalFilter(i);

                    var item = new IRChannelItem(
                        i, sensorType, tempMode, highEngineering, lowEngineering, digitalFilter)
                    {
                        SensorTypeSource = CreateSensorTypeSource(),
                        TempModeSource = CreateTempModeSource()
                    };

                    item.PropertyChanged += ItemOnPropertyChanged;

                    InputConfigSource.Add(item);
                }
            }
        }

        private static List<DisplayItem<sbyte>> CreateTempModeSource()
        {
            return new List<DisplayItem<sbyte>>()
            {
                new DisplayItem<sbyte>() { DisplayName = "Custom Scale", Value = 0 },
                new DisplayItem<sbyte>() { DisplayName = "Celsius", Value = 1 },
                new DisplayItem<sbyte>() { DisplayName = "Fahrenheit", Value = 2 },
                new DisplayItem<sbyte>() { DisplayName = "Kelvin", Value = 3 },
                new DisplayItem<sbyte>() { DisplayName = "Rankine", Value = 4 },
            };
        }

        private static List<DisplayItem<sbyte>> CreateSensorTypeSource()
        {
            return new List<DisplayItem<sbyte>>()
            {
                new DisplayItem<sbyte>() { DisplayName = "Ohms", Value = 0 },
                new DisplayItem<sbyte>() { DisplayName = "100 ohm Pt 385", Value = 1 },
                new DisplayItem<sbyte>() { DisplayName = "200 ohm Pt 385", Value = 2 },
                new DisplayItem<sbyte>() { DisplayName = "100 ohm Pt 3916", Value = 5 },
                new DisplayItem<sbyte>() { DisplayName = "200 ohm Pt 3916", Value = 6 },
                new DisplayItem<sbyte>() { DisplayName = "10 ohm Cu 427", Value = 9 },
                new DisplayItem<sbyte>() { DisplayName = "120 ohm Ni 672", Value = 10 },
                new DisplayItem<sbyte>() { DisplayName = "100 ohm Ni 618", Value = 11 },
                new DisplayItem<sbyte>() { DisplayName = "120 ohm Ni 618", Value = 12 },
            };
        }

        private void UpdateInputConfigSource()
        {
            var module = Profiles.GetModule(ModifiedAnalogIO.Major);
            if (module != null)
            {
                for (var i = 0; i < module.NumberOfInputs; i++)
                {
                    short highEngineering = ModifiedAnalogIO.GetHighEngineering(i);
                    short lowEngineering = ModifiedAnalogIO.GetLowEngineering(i);
                    short digitalFilter = ModifiedAnalogIO.GetDigitalFilter(i);
                    sbyte sensorType = ModifiedAnalogIO.GetSensorType(i);
                    sbyte tempMode = ModifiedAnalogIO.GetTempMode(i);

                    var item = InputConfigSource[i];

                    item.HighEngineering = highEngineering;
                    item.LowEngineering = lowEngineering;
                    item.DigitalFilter = digitalFilter;
                    item.SensorType = sensorType;
                    item.TempMode = tempMode;

                }
            }
        }

        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as IRChannelItem;
            if (item != null)
            {
                int index = item.ChannelIndex;

                switch (e.PropertyName)
                {
                    case "HighEngineering":

                        if (ModifiedAnalogIO.GetHighEngineering(index) != item.HighEngineering)
                        {
                            ModifiedAnalogIO.SetHighEngineering(index, item.HighEngineering);
                            IsDirty = true;
                        }

                        break;

                    case "LowEngineering":
                        if (ModifiedAnalogIO.GetLowEngineering(index) != item.LowEngineering)
                        {
                            ModifiedAnalogIO.SetLowEngineering(index, item.LowEngineering);
                            IsDirty = true;
                        }

                        break;

                    case "DigitalFilter":
                        if (ModifiedAnalogIO.GetDigitalFilter(index) != item.DigitalFilter)
                        {
                            ModifiedAnalogIO.SetDigitalFilter(index, item.DigitalFilter);
                            IsDirty = true;
                        }

                        break;

                    case "SensorType":
                        if (ModifiedAnalogIO.GetSensorType(index) != item.SensorType)
                        {
                            ModifiedAnalogIO.SetSensorType(index, item.SensorType);
                            IsDirty = true;
                        }

                        break;
                    case "TempMode":
                        if (ModifiedAnalogIO.GetTempMode(index) != item.TempMode)
                        {
                            ModifiedAnalogIO.SetTempMode(index, item.TempMode);
                            IsDirty = true;
                        }

                        break;
                }
            }
        }

        private List<Tuple<string, string>> CreateUpdateList()
        {
            var updateList = new List<Tuple<string, string>>();

            var module = Profiles.GetModule(ModifiedAnalogIO.Major);
            if (module != null)
            {
                for (var i = 0; i < module.NumberOfInputs; i++)
                {
                    updateList.Add(new Tuple<string, string>($"Ch{i}LowEngineering",
                        ModifiedAnalogIO.GetLowEngineering(i).ToString()));
                    updateList.Add(new Tuple<string, string>($"Ch{i}HighEngineering",
                        ModifiedAnalogIO.GetHighEngineering(i).ToString()));
                    updateList.Add(new Tuple<string, string>($"Ch{i}DigitalFilter",
                        ModifiedAnalogIO.GetDigitalFilter(i).ToString()));

                    updateList.Add(new Tuple<string, string>($"Ch{i}SensorType",
                        ModifiedAnalogIO.GetSensorType(i).ToString()));

                    updateList.Add(new Tuple<string, string>($"Ch{i}TempMode",
                        ModifiedAnalogIO.GetTempMode(i).ToString()));
                }

                updateList.Add(new Tuple<string, string>("NotchFilter",
                    ModifiedAnalogIO.GetNotchFilter().ToString()));
            }

            return updateList;
        }
    }

    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class IRChannelItem : ObservableObject
    {
        private sbyte _sensorType;
        private List<DisplayItem<sbyte>> _sensorTypeSource;

        private sbyte _tempMode;
        private List<DisplayItem<sbyte>> _tempModeSource;

        private short _highEngineering;
        private short _lowEngineering;
        private short _digitalFilter;

        public IRChannelItem(
            int index,
            sbyte sensorType,
            sbyte tempMode,
            short highEngineering,
            short lowEngineering,
            short digitalFilter)
        {
            ChannelIndex = index;

            SensorType = sensorType;
            TempMode = tempMode;
            HighEngineering = highEngineering;
            LowEngineering = lowEngineering;
            DigitalFilter = digitalFilter;
        }

        public int ChannelIndex { get; }

        public sbyte SensorType
        {
            get { return _sensorType; }
            set { Set(ref _sensorType, value); }
        }

        public List<DisplayItem<sbyte>> SensorTypeSource
        {
            get { return _sensorTypeSource; }
            set { Set(ref _sensorTypeSource, value); }
        }

        public sbyte TempMode
        {
            get { return _tempMode; }
            set
            {
                Set(ref _tempMode, value);
                RaisePropertyChanged("IsCustomScale");
            }
        }

        public List<DisplayItem<sbyte>> TempModeSource
        {
            get { return _tempModeSource; }
            set { Set(ref _tempModeSource, value); }
        }

        public short HighEngineering
        {
            get { return _highEngineering; }
            set { Set(ref _highEngineering, value); }
        }

        public short LowEngineering
        {
            get { return _lowEngineering; }
            set { Set(ref _lowEngineering, value); }
        }

        public short DigitalFilter
        {
            get { return _digitalFilter; }
            set { Set(ref _digitalFilter, value); }
        }

        public bool IsCustomScale
        {
            get
            {
                if (_tempMode == 0)
                    return true;
                return false;
            }
        }
    }
}
