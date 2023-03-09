using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;

namespace ICSStudio.UIServicesPackage.RSTrendXProperties.Panel
{
    class Y_AxisViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private readonly TrendObject _trend;
        private bool _isDirty;
        private bool _displayScale;
        private int _decimalPlaces;
        private bool _isolated;
        private int _majorGridLines;
        private int _minorGridLines;
        private Color _gridLinesColor;
        private bool _displayGridLines;
        private bool _enabled;
        private bool _automaticChecked;
        private bool _customChecked;
        private string _maxValue;
        private string _minValue;
        private bool _presetChecked;
        private bool _scaleAsPercentage;
        private bool _sameChecked;
        private bool _independentChecked;
        private bool _usingPenChecked;
        private bool _group2Enable;
        private string _selectedPen;

        public Y_AxisViewModel(Y_Axis panel, ITrend trend)
        {
            panel.DataContext = this;
            Control = panel;
            _trend = (TrendObject)trend;
            DisplayScale = trend.DisplayYScale;
            DecimalPlaces = trend.YScaleDecimalPlaces;
            DisplayGridLines = trend.DisplayGridLinesY;
            MajorGridLines = trend.MajorGridLinesY;
            MinorGridLines = trend.MinorGridLinesY;
            GridLinesColor = (Color) ColorConverter.ConvertFromString($"#{FormatOp.RemoveFormat(trend.GridColorY)}");
            Isolated = trend.Isolated;
            switch (_trend.ValueOption)
            {
                case ValueOptionType.Automatic:
                    AutomaticChecked = true;
                    break;
                case ValueOptionType.Preset:
                    PresetChecked = true;
                    break;
                case ValueOptionType.Custom:
                    CustomChecked = true;
                    break;
            }

            switch (trend.ScaleOption)
            {
                case ScaleOption.Independent:
                    IndependentChecked = true;
                    break;
                case ScaleOption.Same:
                    SameChecked = true;
                    break;
                case ScaleOption.UsingPen:
                    UsingPenChecked = true;
                    break;
            }

            MaxValue = trend.ActualMaximumValue.ToString("g6");
            MinValue = trend.ActualMinimumValue.ToString("g6");
            ScaleAsPercentage = trend.ScaleAsPercentage;
            foreach (PenObject pen in trend.Pens)
            {
                PropertyChangedEventManager.AddHandler(pen, Pen_PropertyChanged,"Name");
                Pens.Add(pen.Name);
            }
            _trend.CollectionChanged += _trend_CollectionChanged;
            SelectedPen = _trend.ScalePen?.Name ?? (trend.Pens.Any() ? Pens[0] : "");
            IsDirty = false;

        }

        private void Pen_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var pen = (PenObject) sender;
            var index = _trend.Pens.ToList().IndexOf(pen);
            var selectedIndex = Pens.IndexOf(SelectedPen);

            Pens.RemoveAt(index);
            Pens.Insert(index,pen.Name);
            if (index == selectedIndex)
                SelectedPen = pen.Name;
        }

        private void _trend_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (PenObject item in e.NewItems)
                {
                    PropertyChangedEventManager.AddHandler(item, Pen_PropertyChanged, "Name");
                    Pens.Add(item.Name);
                    if (SelectedPen == "")
                        SelectedPen = item.Name;
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (PenObject item in e.OldItems)
                {
                    PropertyChangedEventManager.RemoveHandler(item, Pen_PropertyChanged, "Name");
                    Pens.Remove( item.Name);
                    if (SelectedPen == item.Name)
                    {
                        if (Pens.Any())
                        {
                            SelectedPen = Pens[0];
                        }
                        else
                        {
                            SelectedPen = "";
                        }
                    }
                }
            }
        }

        public ObservableCollection<string> Pens { get; }=new ObservableCollection<string>();

        public string SelectedPen
        {
            set
            {
                Set(ref _selectedPen , value);
                IsDirty = true;
            }
            get { return _selectedPen; }
        }

        public bool Group2Enable
        {
            set { Set(ref _group2Enable, value); }
            get { return _group2Enable; }
        }

        public bool SameChecked
        {
            set
            {
                Set(ref _sameChecked, value);
                if (value)
                {
                    IndependentChecked = false;
                    UsingPenChecked = false;
                }

                IsDirty = true;
            }
            get { return _sameChecked; }
        }

        public bool IndependentChecked
        {
            set
            {
                Set(ref _independentChecked, value);
                if (value)
                {
                    SameChecked = false;
                    UsingPenChecked = false;
                }

                IsDirty = true;
            }
            get { return _independentChecked; }
        }

        public bool UsingPenChecked
        {
            set
            {
                Set(ref _usingPenChecked, value);
                if (value)
                {
                    SameChecked = false;
                    IndependentChecked = false;
                }

                IsDirty = true;
            }
            get { return _usingPenChecked; }
        }

        public bool ScaleAsPercentage
        {
            set
            {
                _scaleAsPercentage = value;
                IsDirty = true;
            }
            get { return _scaleAsPercentage; }
        }

        public string MaxValue
        {
            set
            {
                Set(ref _maxValue , value);
                IsDirty = true;
            }
            get { return _maxValue; }
        }

        public string MinValue
        {
            set
            {
                Set(ref _minValue , value);
                IsDirty = true;
            }
            get { return _minValue; }
        }

        public bool AutomaticChecked
        {
            set
            {
                Set(ref _automaticChecked, value);
                if (value)
                {
                    CustomChecked = false;
                    PresetChecked = false;
                }

                IsDirty = true;
            }
            get { return _automaticChecked; }
        }

        public bool PresetChecked
        {
            set
            {
                Set(ref _presetChecked, value);
                if (value)
                {
                    AutomaticChecked = false;
                    CustomChecked = false;
                }

                IsDirty = true;
            }
            get { return _presetChecked; }
        }

        public bool CustomChecked
        {
            set
            {
                Set(ref _customChecked, value);
                if (value)
                {
                    AutomaticChecked = false;
                    PresetChecked = false;
                    Group2Enable = false;
                }
                else
                {
                    Group2Enable = true;
                }

                IsDirty = true;
            }
            get { return _customChecked; }
        }

        public bool Enabled
        {
            set { Set(ref _enabled, value); }
            get { return _enabled; }
        }

        public int MajorGridLines
        {
            set
            {
                _majorGridLines = value;
                IsDirty = true;
            }
            get { return _majorGridLines; }
        }

        public int MinorGridLines
        {
            set
            {
                _minorGridLines = value;
                IsDirty = true;
            }
            get { return _minorGridLines; }
        }

        public Color GridLinesColor
        {
            set
            {
                _gridLinesColor = value;
                IsDirty = true;
            }
            get { return _gridLinesColor; }
        }

        public bool DisplayGridLines
        {
            set
            {
                Set(ref _displayGridLines, value);
                IsDirty = true;
            }
            get { return _displayGridLines; }
        }

        public bool Verify()
        {
            double min, max;
            var res = double.TryParse(MinValue, out min);
            res = double.TryParse(MaxValue, out max) && res;
            if (!res)
            {
                MessageBox.Show(LanguageManager.GetInstance().ConvertSpecifier("PleaseEnterANumber"), "ICS Studio", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                return false;
            }

            if (min < float.MinValue || min > float.MaxValue || max < float.MinValue || max > float.MaxValue)
            {
                MessageBox.Show(LanguageManager.GetInstance().ConvertSpecifier("PleaseEnterANumberBetween"), "ICS Studio", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                return false;
            }

            if (min > max)
            {
                MessageBox.Show(
                    LanguageManager.GetInstance().ConvertSpecifier("MinimumValueExceedsTheMaximum"),
                    "ICS Studio", MessageBoxButton.OK);
                MaxValue = MinValue;
                MinValue = _trend.ActualMinimumValue.ToString();
                return false;
            }

            MinValue = min.ToString("g6");
            MaxValue = max.ToString("g6");
            return true;
        }
        
        public bool Isolated
        {
            set
            {
                _isolated = value;
                Enabled = !value;
                IsDirty = true;
            }
            get { return _isolated; }
        }

        public bool DisplayScale
        {
            set
            {
                Set(ref _displayScale , value);
                IsDirty = true;
            }
            get { return _displayScale; }
        }

        public int DecimalPlaces
        {
            set
            {
                _decimalPlaces = value;
                IsDirty = true;
            }
            get { return _decimalPlaces; }
        }
        public bool IsClosing { set; get; }
        public void Save()
        {
            if(IsClosing)return;
            if (!IsDirty) return;
            var trend = _trend as TrendObject;
            if (trend != null)
            {
                trend.DisplayYScale = DisplayScale;
                trend.YScaleDecimalPlaces = DecimalPlaces;
                trend.Isolated = Isolated;
                trend.MajorGridLinesY = MajorGridLines;
                trend.MinorGridLinesY = MinorGridLines;
                trend.DisplayGridLinesY = DisplayGridLines;
                trend.GridColorY = "16" + GridLinesColor;
                if (AutomaticChecked)
                    trend.ValueOption = ValueOptionType.Automatic;
                else if (PresetChecked)
                    trend.ValueOption = ValueOptionType.Preset;
                else
                    trend.ValueOption = ValueOptionType.Custom;

                if (SameChecked)
                    trend.ScaleOption = ScaleOption.Same;
                else if (IndependentChecked)
                    trend.ScaleOption = ScaleOption.Independent;
                else
                {
                    trend.ScaleOption = ScaleOption.UsingPen;
                    var pen = trend.Pens.FirstOrDefault(p => p.Name == SelectedPen);
                    trend.ScalePen = pen;
                }

                trend.ActualMaximumValue = double.Parse(MaxValue);
                trend.ActualMinimumValue = double.Parse(MinValue);
                trend.ScaleAsPercentage = ScaleAsPercentage;
            }

            IsDirty = false;
        }

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
                _isDirty = value;
                IsDirtyChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler IsDirtyChanged;
    }
}
