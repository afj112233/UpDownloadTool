using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using ICSStudio.DeviceProfiles.DIOModule;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties.DiscreteIOs.Panel
{
    public class InputFilterTimeConfigViewModel : DeviceOptionPanel
    {
        public InputFilterTimeConfigViewModel(UserControl control, ModifiedDiscreteIO modifiedDiscreteIO) :
            base(control)
        {
            ModifiedDiscreteIO = modifiedDiscreteIO;

            CreateInputFilterSource();
        }

        public ModifiedDiscreteIO ModifiedDiscreteIO { get; }

        public DiscreteIO OriginalDiscreteIO => ModifiedDiscreteIO?.OriginalDiscreteIO;

        public DIOModuleProfiles Profiles => OriginalDiscreteIO?.Profiles;

        public bool IsInputFitterTimeEnabled => !ModifiedDiscreteIO.Controller.IsOnline;

        public override Visibility Visibility
        {
            get
            {
                // listen only return Visibility.Collapsed
                if (Profiles
                    .GetConnectionStringByConfigID(ModifiedDiscreteIO.ConnectionConfigID, ModifiedDiscreteIO.Major)
                    .Contains("Listen Only"))
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        public ObservableCollection<InputFilterTimeItem> InputFilterTimeSource { get; set; }

        public override bool SaveOptions()
        {
            UpdateInputFilterSource();

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
            UpdateInputFilterSource();
        }

        public override void CheckDirty()
        {
            //TODO(gjc): add code here
        }

        public override void Refresh()
        {
            base.Refresh();
            RaisePropertyChanged(nameof(IsInputFitterTimeEnabled));
        }

        private void CreateInputFilterSource()
        {
            if (Visibility != Visibility.Visible)
                return;

            InputFilterTimeSource = new ObservableCollection<InputFilterTimeItem>();

            var module = Profiles.GetModule(ModifiedDiscreteIO.Major);
            if (module != null)
            {
                for (var i = 0; i < module.NumberOfInputs; i++)
                {
                    var offOn = ModifiedDiscreteIO.GetFilterOffOn(i);
                    var onOff = ModifiedDiscreteIO.GetFilterOnOff(i);
                    ushort minValue = 0;
                    var maxValue = ushort.MaxValue;

                    var item = new InputFilterTimeItem(i, offOn, onOff, minValue, maxValue);
                    item.PropertyChanged += ItemOnPropertyChanged;

                    InputFilterTimeSource.Add(item);
                }
            }
        }

        private void UpdateInputFilterSource()
        {
            if (Visibility != Visibility.Visible)
                return;

            if (InputFilterTimeSource == null)
            {
                CreateInputFilterSource();
            }

            var module = Profiles.GetModule(ModifiedDiscreteIO.Major);
            if (module != null)
            {
                Contract.Assert(InputFilterTimeSource.Count == module.NumberOfInputs);

                for (var i = 0; i < module.NumberOfInputs; i++)
                {
                    var offOn = ModifiedDiscreteIO.GetFilterOffOn(i);
                    var onOff = ModifiedDiscreteIO.GetFilterOnOff(i);

                    //ushort minValue = 0;
                    //var maxValue = ushort.MaxValue;

                    var item = InputFilterTimeSource[i];
                    item.OffOn = offOn / 1000f;
                    item.OnOff = onOff / 1000f;
                }

            }
        }

        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as InputFilterTimeItem;
            if (item != null)
            {
                var index = item.PointIndex;

                if (e.PropertyName == "OffOn")
                {
                    var offOn = (ushort) Math.Round(item.OffOn * 1000);
                    if (ModifiedDiscreteIO.GetFilterOffOn(index) != offOn)
                    {
                        ModifiedDiscreteIO.SetFilterOffOn(index, offOn);

                        IsDirty = true;
                    }
                }
                else if (e.PropertyName == "OnOff")
                {
                    var onOff = (ushort) Math.Round(item.OnOff * 1000);
                    if (ModifiedDiscreteIO.GetFilterOnOff(index) != onOff)
                    {
                        ModifiedDiscreteIO.SetFilterOnOff(index, onOff);
                        IsDirty = true;
                    }
                }
            }
        }

        private List<Tuple<string, string>> CreateUpdateList()
        {
            var updateList = new List<Tuple<string, string>>();

            var module = Profiles.GetModule(ModifiedDiscreteIO.Major);
            if (module != null)
                for (var i = 0; i < module.NumberOfInputs; i++)
                {
                    updateList.Add(new Tuple<string, string>($"Pt{i}FilterOffOn",
                        ModifiedDiscreteIO.GetFilterOffOn(i).ToString()));
                    updateList.Add(new Tuple<string, string>($"Pt{i}FilterOnOff",
                        ModifiedDiscreteIO.GetFilterOnOff(i).ToString()));
                }

            return updateList;
        }
    }

    public class InputFilterTimeItem : ObservableObject
    {
        private readonly float _maxValue;
        private readonly float _minValue;
        private float _offOn;
        private float _onOff;

        public InputFilterTimeItem(
            int pointIndex,
            ushort offOn,
            ushort onOff,
            ushort minValue,
            ushort maxValue)
        {
            PointIndex = pointIndex;
            _offOn = offOn / 1000f;
            _onOff = onOff / 1000f;

            _minValue = minValue / 1000f;
            _maxValue = maxValue / 1000f;
        }

        public int PointIndex { get; }

        public float OffOn
        {
            get { return _offOn; }
            set
            {
                if (value < _minValue)
                    value = _maxValue;
                if (value > _maxValue)
                    value = _minValue;

                if (Math.Abs(_offOn - value) > float.Epsilon) Set(ref _offOn, value);
            }
        }

        public float OnOff
        {
            get { return _onOff; }
            set
            {
                if (value < _minValue)
                    value = _maxValue;
                if (value > _maxValue)
                    value = _minValue;

                if (Math.Abs(_onOff - value) > float.Epsilon) Set(ref _onOff, value);
            }
        }
    }
}