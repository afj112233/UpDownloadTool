using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using ICSStudio.DeviceProfiles.DIOModule;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.Gui.Utils;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties.AnalogIOs.Panel
{
    public class OutputConfigViewModel : DeviceOptionPanel
    {
        public OutputConfigViewModel(UserControl panel, ModifiedAnalogIO modifiedAnalogIO) : base(panel)
        {
            ModifiedAnalogIO = modifiedAnalogIO;

            CreateOutputConfigSource();
        }

        public ModifiedAnalogIO ModifiedAnalogIO { get; }

        public AnalogIO OriginalAnalogIO => ModifiedAnalogIO?.OriginalAnalogIO;

        public DIOModuleProfiles Profiles => OriginalAnalogIO?.Profiles;

        public bool IsOutputConfigEnabled => !IsOnline;

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
            UpdateOutputConfigSource();

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

        private List<Tuple<string, string>> CreateUpdateList()
        {
            var updateList = new List<Tuple<string, string>>();

            var module = Profiles.GetModule(ModifiedAnalogIO.Major);
            if (module != null)
            {
                for (var i = 0; i < module.NumberOfOutputs; i++)
                {
                    updateList.Add(new Tuple<string, string>($"Ch{i}LowEngineering",
                        ModifiedAnalogIO.GetLowEngineering(i).ToString()));
                    updateList.Add(new Tuple<string, string>($"Ch{i}HighEngineering",
                        ModifiedAnalogIO.GetHighEngineering(i).ToString()));
                    updateList.Add(new Tuple<string, string>($"Ch{i}RangeType",
                        ModifiedAnalogIO.GetRangeType(i).ToString()));
                }
            }

            return updateList;
        }

        public override void Show()
        {
            UpdateOutputConfigSource();
        }

        public override void CheckDirty()
        {
            //ignore
        }

        public override void Refresh()
        {
            base.Refresh();
            RaisePropertyChanged(nameof(IsOutputConfigEnabled));
        }

        public ObservableCollection<OutputChannelItem> OutputConfigSource { get; set; }

        private void CreateOutputConfigSource()
        {
            OutputConfigSource = new ObservableCollection<OutputChannelItem>();

            var module = Profiles.GetModule(ModifiedAnalogIO.Major);
            if (module != null)
            {
                for (var i = 0; i < module.NumberOfOutputs; i++)
                {
                    short highEngineering = ModifiedAnalogIO.GetHighEngineering(i);
                    short lowEngineering = ModifiedAnalogIO.GetLowEngineering(i);
                    sbyte rangeType = ModifiedAnalogIO.GetRangeType(i);

                    List<DisplayItem<sbyte>> rangeTypeSource = new List<DisplayItem<sbyte>>
                    {
                        new DisplayItem<sbyte> {DisplayName = "4-20 mA", Value = 0},
                        new DisplayItem<sbyte> {DisplayName = "0-20 mA", Value = 2},
                        new DisplayItem<sbyte> {DisplayName = "0 to 10 V", Value = 1},
                        new DisplayItem<sbyte> {DisplayName = "-10 to 10 V", Value = 3},
                    };

                    var item = new OutputChannelItem(i, highEngineering, lowEngineering, rangeType)
                    {
                        RangeTypeSource = rangeTypeSource
                    };

                    item.PropertyChanged += ItemOnPropertyChanged;

                    OutputConfigSource.Add(item);
                }
            }
        }

        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as OutputChannelItem;
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

        private void UpdateOutputConfigSource()
        {
            var module = Profiles.GetModule(ModifiedAnalogIO.Major);
            if (module != null)
            {
                for (var i = 0; i < module.NumberOfOutputs; i++)
                {
                    short highEngineering = ModifiedAnalogIO.GetHighEngineering(i);
                    short lowEngineering = ModifiedAnalogIO.GetLowEngineering(i);
                    sbyte rangeType = ModifiedAnalogIO.GetRangeType(i);

                    var item = OutputConfigSource[i];
                    item.HighEngineering = highEngineering;
                    item.LowEngineering = lowEngineering;
                    item.RangeType = rangeType;

                }
            }
        }
    }

    public class OutputChannelItem : ObservableObject
    {
        private short _highEngineering;
        private short _lowEngineering;
        private sbyte _rangeType;

        private List<DisplayItem<sbyte>> _rangeTypeSource;

        public OutputChannelItem(int index, short highEngineering, short lowEngineering, sbyte rangeType)
        {
            ChannelIndex = index;
            HighEngineering = highEngineering;
            LowEngineering = lowEngineering;
            RangeType = rangeType;
        }

        public int ChannelIndex { get; }

        public sbyte RangeType
        {
            get { return _rangeType; }
            set { Set(ref _rangeType, value); }
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

        public List<DisplayItem<sbyte>> RangeTypeSource
        {
            get { return _rangeTypeSource; }
            set { Set(ref _rangeTypeSource, value); }
        }
    }
}
