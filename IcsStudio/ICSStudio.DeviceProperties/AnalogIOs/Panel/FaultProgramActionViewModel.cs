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
    public class FaultProgramActionViewModel : DeviceOptionPanel
    {
        public FaultProgramActionViewModel(UserControl control, ModifiedAnalogIO modifiedAnalogIO) : base(control)
        {
            ModifiedAnalogIO = modifiedAnalogIO;
            CreateFaultProgramActionSource();
        }

        public ModifiedAnalogIO ModifiedAnalogIO { get; }

        public AnalogIO OriginalAnalogIO => ModifiedAnalogIO?.OriginalAnalogIO;

        public DIOModuleProfiles Profiles => OriginalAnalogIO?.Profiles;

        public bool IsFaultProgramActionEnabled => !IsOnline;

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
            //TODO(gjc): add code here
        }

        public override void CheckDirty()
        {
            //ignore
        }

        public override void Refresh()
        {
            base.Refresh();
            RaisePropertyChanged(nameof(IsFaultProgramActionEnabled));
        }

        public ObservableCollection<FaultProgramActionItem> FaultProgramActionSource { get; private set; }

        private void CreateFaultProgramActionSource()
        {
            FaultProgramActionSource = new ObservableCollection<FaultProgramActionItem>();

            var module = Profiles.GetModule(ModifiedAnalogIO.Major);
            if (module != null)
            {
                for (var i = 0; i < module.NumberOfOutputs; i++)
                {
                    sbyte faultMode = ModifiedAnalogIO.GetFaultMode(i);
                    short faultValue = ModifiedAnalogIO.GetFaultValue(i);
                    sbyte progMode = ModifiedAnalogIO.GetProgMode(i);
                    short progValue = ModifiedAnalogIO.GetProgValue(i);

                    List<DisplayItem<sbyte>> faultModeSource = new List<DisplayItem<sbyte>>()
                    {
                        new DisplayItem<sbyte>() {DisplayName = "Hold Last State", Value = 0},
                        new DisplayItem<sbyte>() {DisplayName = "Go to Low Clamp", Value = 1},
                        new DisplayItem<sbyte>() {DisplayName = "Go to High Clamp", Value = 2},
                        new DisplayItem<sbyte>() {DisplayName = "Use Fault Value", Value = 3}
                    };
                    List<DisplayItem<sbyte>> progModeSource = new List<DisplayItem<sbyte>>()
                    {
                        new DisplayItem<sbyte>() {DisplayName = "Hold Last State", Value = 0},
                        new DisplayItem<sbyte>() {DisplayName = "Go to Low Clamp", Value = 1},
                        new DisplayItem<sbyte>() {DisplayName = "Go to High Clamp", Value = 2},
                        new DisplayItem<sbyte>() {DisplayName = "Use Program Value", Value = 3}
                    };

                    var item = new FaultProgramActionItem(i, faultMode, faultValue, progMode, progValue)
                    {
                        FaultModeSource = faultModeSource,
                        ProgModeSource = progModeSource
                    };

                    item.PropertyChanged += ItemOnPropertyChanged;

                    FaultProgramActionSource.Add(item);
                }
            }

        }

        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as FaultProgramActionItem;
            if (item != null)
            {
                int index = item.ChannelIndex;

                switch (e.PropertyName)
                {
                    case "FaultMode":
                        if (ModifiedAnalogIO.GetFaultMode(index) != item.FaultMode)
                        {
                            ModifiedAnalogIO.SetFaultMode(index, item.FaultMode);

                            IsDirty = true;
                        }

                        break;

                    case "FaultValue":
                        if (ModifiedAnalogIO.GetFaultValue(index) != item.FaultValue)
                        {
                            ModifiedAnalogIO.SetFaultValue(index, item.FaultValue);

                            IsDirty = true;
                        }

                        break;

                    case "ProgMode":
                        if (ModifiedAnalogIO.GetProgMode(index) != item.ProgMode)
                        {
                            ModifiedAnalogIO.SetProgMode(index, item.ProgMode);

                            IsDirty = true;
                        }

                        break;

                    case "ProgValue":
                        if (ModifiedAnalogIO.GetProgValue(index) != item.ProgValue)
                        {
                            ModifiedAnalogIO.SetProgValue(index, item.ProgValue);

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
                for (var i = 0; i < module.NumberOfOutputs; i++)
                {
                    updateList.Add(new Tuple<string, string>($"Ch{i}FaultMode",
                        ModifiedAnalogIO.GetFaultMode(i).ToString()));
                    updateList.Add(new Tuple<string, string>($"Ch{i}FaultValue",
                        ModifiedAnalogIO.GetFaultValue(i).ToString()));
                    updateList.Add(new Tuple<string, string>($"Ch{i}ProgMode",
                        ModifiedAnalogIO.GetProgMode(i).ToString()));
                    updateList.Add(new Tuple<string, string>($"Ch{i}ProgValue",
                        ModifiedAnalogIO.GetProgValue(i).ToString()));
                }
            }

            return updateList;
        }
    }

    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class FaultProgramActionItem : ObservableObject
    {
        private sbyte _faultMode;
        private short _faultValue;
        private sbyte _progMode;
        private short _progValue;

        public FaultProgramActionItem(
            int index, sbyte faultMode, short faultValue,
            sbyte progMode, short progValue)
        {
            ChannelIndex = index;

            FaultMode = faultMode;
            FaultValue = faultValue;

            ProgMode = progMode;
            ProgValue = progValue;
        }

        public int ChannelIndex { get; }

        public sbyte FaultMode
        {
            get { return _faultMode; }
            set
            {
                Set(ref _faultMode, value);
                RaisePropertyChanged("FaultValueEnable");

                if (!FaultValueEnable)
                {
                    FaultValue = 0;
                }
            }
        }

        public short FaultValue
        {
            get { return _faultValue; }
            set { Set(ref _faultValue, value); }
        }

        public List<DisplayItem<sbyte>> FaultModeSource { get; set; }

        public sbyte ProgMode
        {
            get { return _progMode; }
            set
            {
                Set(ref _progMode, value);
                RaisePropertyChanged("ProgValueEnable");

                if (!ProgValueEnable)
                {
                    ProgValue = 0;
                }
            }
        }

        public short ProgValue
        {
            get { return _progValue; }
            set { Set(ref _progValue, value); }
        }

        public List<DisplayItem<sbyte>> ProgModeSource { get; set; }

        public bool FaultValueEnable => _faultMode == 3;

        public bool ProgValueEnable => _progMode == 3;

    }
}
