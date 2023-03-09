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
    public class IFConfigViewModel : DeviceOptionPanel
    {
        private short _realTimeSample;
        private sbyte _notchFilter;

        public IFConfigViewModel(UserControl panel, ModifiedAnalogIO modifiedAnalogIO) : base(panel)
        {
            ModifiedAnalogIO = modifiedAnalogIO;

            CreateInputConfigSource();
             
            _realTimeSample = ModifiedAnalogIO.GetRealTimeSample();
            _notchFilter = ModifiedAnalogIO.GetNotchFilter();

            CreateNotchFilterSource();
        }

        public ModifiedAnalogIO ModifiedAnalogIO { get; }

        public AnalogIO OriginalAnalogIO => ModifiedAnalogIO?.OriginalAnalogIO;

        public DIOModuleProfiles Profiles => OriginalAnalogIO?.Profiles;

        public bool IsIFConfigEnabled => !IsOnline;

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
                    //TODO(gjc): Check RTS

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

            _realTimeSample = ModifiedAnalogIO.GetRealTimeSample();
            _notchFilter = ModifiedAnalogIO.GetNotchFilter();

            RaisePropertyChanged("RTS");
            RaisePropertyChanged("NotchFilter");
        }

        public override void CheckDirty()
        {
            //ignore
        }

        public override void Refresh()
        {
            base.Refresh();
            RaisePropertyChanged(nameof(IsIFConfigEnabled));
        }

        public ObservableCollection<IFChannelItem> InputConfigSource { get; set; }

        public short RTS
        {
            get { return _realTimeSample; }
            set
            {
                if (_realTimeSample != value)
                {
                    Set(ref _realTimeSample, value);

                    ModifiedAnalogIO.SetRealTimeSample(_realTimeSample);

                    IsDirty = true;
                }
            }
        }

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

        private void CreateInputConfigSource()
        {
            InputConfigSource = new ObservableCollection<IFChannelItem>();
            
            var module = Profiles.GetModule(ModifiedAnalogIO.Major);

            if (module != null)
            {
                for (var i = 0; i < module.NumberOfInputs; i++)
                {
                    short highEngineering = ModifiedAnalogIO.GetHighEngineering(i);
                    short lowEngineering = ModifiedAnalogIO.GetLowEngineering(i);
                    short digitalFilter = ModifiedAnalogIO.GetDigitalFilter(i);
                    sbyte rangeType = ModifiedAnalogIO.GetRangeType(i);

                    List<DisplayItem<sbyte>> rangeTypeSource = new List<DisplayItem<sbyte>>
                    {
                        new DisplayItem<sbyte> {DisplayName = "4-20 mA", Value = 3},
                        new DisplayItem<sbyte> {DisplayName = "0-20 mA", Value = 8},
                        new DisplayItem<sbyte> {DisplayName = "-10 to 10 V", Value = 0},
                        new DisplayItem<sbyte> {DisplayName = "0 to 10 V", Value = 2}
                    };

                    var item = new IFChannelItem(i, highEngineering, lowEngineering, digitalFilter, rangeType)
                    {
                        RangeTypeSource = rangeTypeSource
                    };

                    item.PropertyChanged += ItemOnPropertyChanged;

                    InputConfigSource.Add(item);
                }
            }

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
                    sbyte rangeType = ModifiedAnalogIO.GetRangeType(i);

                    var item = InputConfigSource[i];

                    item.HighEngineering = highEngineering;
                    item.LowEngineering = lowEngineering;
                    item.DigitalFilter = digitalFilter;
                    item.RangeType = rangeType;

                }
            }
        }

        private void CreateNotchFilterSource()
        {
            NotchFilterSource = new List<DisplayItem<sbyte>>();

            var module = Profiles.GetModule(ModifiedAnalogIO.Major);
            if (module != null)
            {
                if (module.NumberOfInputs == 2)
                {
                    NotchFilterSource.Add(new DisplayItem<sbyte>() {DisplayName = "50Hz", Value = 1});
                    NotchFilterSource.Add(new DisplayItem<sbyte>() {DisplayName = "60Hz", Value = 2});
                    NotchFilterSource.Add(new DisplayItem<sbyte>() {DisplayName = "250Hz", Value = 4});
                    NotchFilterSource.Add(new DisplayItem<sbyte>() {DisplayName = "500Hz", Value = 6});
                }
                else if (module.NumberOfInputs == 4 || module.NumberOfInputs == 8)
                {
                    NotchFilterSource.Add(new DisplayItem<sbyte>() {DisplayName = "60Hz", Value = 0});
                    NotchFilterSource.Add(new DisplayItem<sbyte>() {DisplayName = "50Hz", Value = 1});
                    NotchFilterSource.Add(new DisplayItem<sbyte>() {DisplayName = "100Hz", Value = 2});
                    NotchFilterSource.Add(new DisplayItem<sbyte>() {DisplayName = "120Hz", Value = 3});
                    NotchFilterSource.Add(new DisplayItem<sbyte>() {DisplayName = "200Hz", Value = 4});
                    NotchFilterSource.Add(new DisplayItem<sbyte>() {DisplayName = "240Hz", Value = 5});
                    NotchFilterSource.Add(new DisplayItem<sbyte>() {DisplayName = "300Hz", Value = 6});
                    NotchFilterSource.Add(new DisplayItem<sbyte>() {DisplayName = "400Hz", Value = 7});
                    NotchFilterSource.Add(new DisplayItem<sbyte>() {DisplayName = "480Hz", Value = 8});

                }
            }
        }

        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as IFChannelItem;
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
                    case "RangeType":
                        if (ModifiedAnalogIO.GetRangeType(index) != item.RangeType)
                        {
                            ModifiedAnalogIO.SetRangeType(index, item.RangeType);
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
                    updateList.Add(new Tuple<string, string>($"Ch{i}RangeType",
                        ModifiedAnalogIO.GetRangeType(i).ToString()));
                }

                updateList.Add(new Tuple<string, string>("RealTimeSample",
                    ModifiedAnalogIO.GetRealTimeSample().ToString()));

                updateList.Add(new Tuple<string, string>("NotchFilter",
                    ModifiedAnalogIO.GetNotchFilter().ToString()));
            }

            return updateList;
        }
    }

    public class IFChannelItem : ObservableObject
    {
        private short _highEngineering;
        private short _lowEngineering;
        private short _digitalFilter;
        private sbyte _rangeType;

        private List<DisplayItem<sbyte>> _rangeTypeSource;

        public IFChannelItem(
            int index,
            short highEngineering,
            short lowEngineering,
            short digitalFilter,
            sbyte rangeType)
        {
            ChannelIndex = index;

            _highEngineering = highEngineering;
            _lowEngineering = lowEngineering;
            _digitalFilter = digitalFilter;
            _rangeType = rangeType;
        }

        public int ChannelIndex { get; }

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

        public sbyte RangeType
        {
            get { return _rangeType; }
            set { Set(ref _rangeType, value); }
        }
        
        public List<DisplayItem<sbyte>> RangeTypeSource
        {
            get { return _rangeTypeSource; }
            set { Set(ref _rangeTypeSource, value); }
        }
    }
}
