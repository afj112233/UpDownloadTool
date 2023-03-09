using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.UIServicesPackage.RSTrendXProperties.Panel
{
    class GeneralViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private readonly TrendObject _trend;
        private string _graphTitle;
        private bool _isDirty;
        private bool _showTitle;
        private string _selectedPen;
        private bool _xyEnable;
        private bool _isStandard;
        private bool _isXy;

        public GeneralViewModel(General panel, ITrend trend)
        {
            Control = panel;
            panel.DataContext = this;
            _trend = (TrendObject)trend;
            ShowTitle = _trend.ShowGraphTitle;
            GraphTitle = _trend.GraphTitle;
            _trend.PropertyChanged += GeneralViewModel_PropertyChanged;
            IsStandard = _trend.ChartStyle == ChartStyle.Standard;
            IsXY = !IsStandard;
            foreach (var pen in _trend.Pens)
            {
                Pens.Add(pen.Name);
            }

            XYEnable = Pens.Count > 0;
            if (Pens.Count > 0)
            {
                SelectedPen = _trend.AxisPenName?? Pens[0];
            }
            IsDirty = false;
        }

        public void PenCollectionChanged(object sender, EventArgs e)
        {
            var selectedPen = SelectedPen;
            Pens.Clear();

            if (!_trend.Pens.Any())
            {
                IsStandard = true;
                XYEnable = false;
            }
            else
            {
                foreach (var pen in _trend.Pens)
                {
                    Pens.Add(pen.Name);
                }

                if (Pens.Contains(selectedPen))
                {
                    SelectedPen = selectedPen;
                }
                else
                {
                    IsStandard = true;
                    SelectedPen = Pens[0];
                }
            }
        }

        public bool IsStandard
        {
            set
            {
                Set(ref _isStandard, value);
                if (!IsStandard)
                {
                    if (string.IsNullOrEmpty(SelectedPen))
                    {
                        if (Pens.Contains(_trend.AxisPenName))
                        {
                            SelectedPen = _trend.AxisPenName;
                        }
                        else
                        {
                            if(Pens.Count > 0)
                            SelectedPen = Pens[0];
                            else
                            {
                                _selectedPen = null;
                            }
                        }
                    }
                }
                RaisePropertyChanged("IsXY");
                IsDirty = true;
            }
            get { return _isStandard; }
        }

        public bool IsXY
        {
            set { Set(ref _isXy , value); }
            get { return _isXy; }
        }

        public bool XYEnable
        {
            set { Set(ref _xyEnable , value); }
            get { return _xyEnable; }
        }
        
        public override void Cleanup()
        {
            _trend.PropertyChanged -= GeneralViewModel_PropertyChanged;
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

        private void GeneralViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "GraphTitle")
            {
                _graphTitle = _trend.GraphTitle;
                RaisePropertyChanged("GraphTitle");
            }
        }

        public string GraphTitle
        {
            set
            {
                Set(ref _graphTitle , value);
                IsDirty = true;
            }
            get
            {
                _graphTitle = _graphTitle ?? (_graphTitle = "");
                return _graphTitle;
            }
        }

        public bool ShowTitle
        {
            set
            {
                _showTitle = value;
                IsDirty = true;
            }
            get { return _showTitle; }
        }

        public object Owner { get; set; }
        public object Control { get; }

        public bool IsClosing { set; get; }
        public void Save()
        {
            if(IsClosing)return;
            if (!IsDirty) return;
            if (_trend != null)
            {
                _trend.GraphTitle = GraphTitle;
                _trend.ShowGraphTitle = ShowTitle;
                if (!IsStandard)
                    _trend.AxisPenName = SelectedPen;
                _trend.ChartStyle = IsStandard ? ChartStyle.Standard : ChartStyle.XY;
            }
            IsDirty = false;
        }

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
                Set(ref _isDirty, value);
                IsDirtyChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler IsDirtyChanged;
    }
}
