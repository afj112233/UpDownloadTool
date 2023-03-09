using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using ICSStudio.DeviceProfiles.DIOModule;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties.DiscreteIOs.Panel
{
    public class FaultProgramActionViewModel : DeviceOptionPanel
    {
        public FaultProgramActionViewModel(UserControl control, ModifiedDiscreteIO modifiedDiscreteIO) : base(control)
        {
            ModifiedDiscreteIO = modifiedDiscreteIO;

            UpdateFaultProgramActionSource();
        }

        public ModifiedDiscreteIO ModifiedDiscreteIO { get; }

        public DiscreteIO OriginalDiscreteIO => ModifiedDiscreteIO?.OriginalDiscreteIO;

        public DIOModuleProfiles Profiles => OriginalDiscreteIO?.Profiles;

        public bool IsOutStateDuringEnabled => !ModifiedDiscreteIO.Controller.IsOnline;

        private List<FaultProgramActionItem> _faultProgramActionSource;

        public List<FaultProgramActionItem> FaultProgramActionSource
        {
            get { return _faultProgramActionSource; }
            set { Set(ref _faultProgramActionSource, value); }
        }

        private void UpdateFaultProgramActionSource()
        {
            var module = Profiles.GetModule(ModifiedDiscreteIO.Major);
            if (module != null)
            {
                var faultProgramActionList = new List<FaultProgramActionItem>();

                for (var i = 0; i < module.NumberOfOutputs; i++)
                {
                    ModeType programMode;
                    ModeType faultMode;

                    if (ModifiedDiscreteIO.GetProgMode(i))
                        programMode = ModeType.Hold;
                    else
                        programMode = ModifiedDiscreteIO.GetProgValue(i) ? ModeType.On : ModeType.Off;

                    if (ModifiedDiscreteIO.GetFaultMode(i))
                        faultMode = ModeType.Hold;
                    else
                        faultMode = ModifiedDiscreteIO.GetFaultValue(i) ? ModeType.On : ModeType.Off;

                    var item = new FaultProgramActionItem(i, programMode, faultMode);
                    item.PropertyChanged += ItemOnPropertyChanged;

                    faultProgramActionList.Add(item);
                }

                FaultProgramActionSource = faultProgramActionList;
            }
        }

        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as FaultProgramActionItem;
            if (item != null)
            {
                var index = item.PointIndex;

                if (e.PropertyName == "ProgramMode")
                {
                    switch (item.ProgramMode)
                    {
                        case ModeType.Off:
                            ModifiedDiscreteIO.SetProgMode(index, false);
                            ModifiedDiscreteIO.SetProgValue(index, false);
                            break;
                        case ModeType.On:
                            ModifiedDiscreteIO.SetProgMode(index, false);
                            ModifiedDiscreteIO.SetProgValue(index, true);
                            break;
                        case ModeType.Hold:
                            ModifiedDiscreteIO.SetProgMode(index, true);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    IsDirty = true;
                }
                else if (e.PropertyName == "FaultMode")
                {
                    switch (item.FaultMode)
                    {
                        case ModeType.Off:
                            ModifiedDiscreteIO.SetFaultMode(index, false);
                            ModifiedDiscreteIO.SetFaultValue(index, false);
                            break;
                        case ModeType.On:
                            ModifiedDiscreteIO.SetFaultMode(index, false);
                            ModifiedDiscreteIO.SetFaultValue(index, true);
                            break;
                        case ModeType.Hold:
                            ModifiedDiscreteIO.SetFaultMode(index, true);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    IsDirty = true;
                }
            }
        }

        private List<Tuple<string, string>> CreateUpdateList()
        {
            var updateList = new List<Tuple<string, string>>();

            var module = Profiles.GetModule(ModifiedDiscreteIO.Major);
            if (module != null)
                for (var i = 0; i < module.NumberOfOutputs; i++)
                {
                    updateList.Add(new Tuple<string, string>($"Pt{i}ProgMode",
                        ModifiedDiscreteIO.GetProgMode(i).ToString()));
                    updateList.Add(new Tuple<string, string>($"Pt{i}ProgValue",
                        ModifiedDiscreteIO.GetProgValue(i).ToString()));

                    updateList.Add(new Tuple<string, string>($"Pt{i}FaultMode",
                        ModifiedDiscreteIO.GetFaultMode(i).ToString()));
                    updateList.Add(new Tuple<string, string>($"Pt{i}FaultValue",
                        ModifiedDiscreteIO.GetFaultValue(i).ToString()));
                }

            return updateList;
        }

        public override bool SaveOptions()
        {
            UpdateFaultProgramActionSource();

            if (Visibility == Visibility.Visible)
                if (OriginalDiscreteIO.ConfigTag != null)
                {
                    var updateList = CreateUpdateList();
                    foreach (var tuple in updateList)
                        OriginalDiscreteIO.ConfigTag.SetStringValue(tuple.Item1, tuple.Item2);
                }


            IsDirty = false;

            return true;
        }

        public override void Show()
        {
            UpdateFaultProgramActionSource();
        }

        public override void Refresh()
        {
            base.Refresh();
            RaisePropertyChanged(nameof(IsOutStateDuringEnabled));
        }
    }

    public class FaultProgramActionItem : ObservableObject
    {
        private ModeType _faultMode;
        private ModeType _programMode;

        public FaultProgramActionItem(int pointIndex, ModeType programMode, ModeType faultMode)
        {
            PointIndex = pointIndex;
            ModeSource = new List<ModeType>
            {
                ModeType.Off,
                ModeType.On,
                ModeType.Hold
            };

            _programMode = programMode;
            _faultMode = faultMode;
        }

        public int PointIndex { get; }
        public List<ModeType> ModeSource { get; }

        public ModeType ProgramMode
        {
            get { return _programMode; }
            set { Set(ref _programMode, value); }
        }

        public ModeType FaultMode
        {
            get { return _faultMode; }
            set { Set(ref _faultMode, value); }
        }
    }

    public enum ModeType
    {
        Off,
        On,
        Hold
    }
}