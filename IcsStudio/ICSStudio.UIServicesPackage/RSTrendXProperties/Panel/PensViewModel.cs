using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.UIServicesPackage.View;
using Microsoft.VisualStudio.Shell.Interop;
using Pen = ICSStudio.SimpleServices.Common.Pen;
using Style = ICSStudio.Interfaces.Common.Style;
using Type = ICSStudio.Interfaces.Common.Type;

namespace ICSStudio.UIServicesPackage.RSTrendXProperties.Panel
{
    class PensViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private readonly ITrend _trend;
        private IList _selectedItems;
        private bool _isDirty;

        public PensViewModel(Pens panel, ITrend trend)
        {
            panel.DataContext = this;
            Control = panel;
            _trend = trend;
            EditPens.Add(new PenProperties(EditPens, null));
            SelectedItems = new List<PenProperties>();
            AddConfigureTagsCommand =
                new RelayCommand(ExecuteAddConfigureTagsCommand, CanExecuteAddConfigureTagsCommand);
            DeletePenCommand = new RelayCommand(ExecuteDeletePenCommand, CanExecuteDeletePenCommand);
            ClearCommand = new RelayCommand(ExecuteClearCommand);
            ApplyCommand = new RelayCommand(ExecuteApplyCommand);
            DeleteCommand=new RelayCommand(ExecuteDeleteCommand);
            Pens.CollectionChanged += Pens_CollectionChanged;
            SetPens();
            UpdateOtherPens();
            IsDirty = false;

            _thereAreNoCurve = LanguageManager.GetInstance().ConvertSpecifier("ThereAreNoCurve");
            WeakEventManager<LanguageManager,EventArgs>.AddHandler(LanguageManager.GetInstance(),"LanguageChanged",LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            _thereAreNoCurve = LanguageManager.GetInstance().ConvertSpecifier("ThereAreNoCurve");
        }

        public bool IsClosing { set; get; }
        public ObservableCollection<PenProperties> Pens { get; } = new ObservableCollection<PenProperties>();

        public object Owner { get; set; }
        public object Control { get; }

        public ObservableCollection<PenProperties> EditPens { get; } = new ObservableCollection<PenProperties>();

        public IList SelectedItems
        {
            set
            {
                _selectedItems = value;
                DeletePenCommand?.RaiseCanExecuteChanged();
            }
            get { return _selectedItems; }
        }

        public event EventHandler CollectionChanged;

        public void LoadOptions()
        {

        }

        #region Command
        
        public RelayCommand AddConfigureTagsCommand { get; }

        private void ExecuteAddConfigureTagsCommand()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            var pens = new List<string>();
            foreach (var pen in Pens)
            {
                pens.Add(pen.Name);
            }

            var dialog = new AddOrConfigureTag(_trend.Name, _trend.ParentController, pens);
            var uiShell = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            if (dialog.ShowDialog(uiShell) ?? true)
            {
                var remove = dialog.GetRemovePenList();
                var add = dialog.GetAddPenList();
                foreach (var pen in remove)
                {
                    var properties = Pens.FirstOrDefault(p => p.Name.Equals(pen));
                    if (properties != null)
                    {
                        Pens.Remove(properties);
                        _collectionChanged = true;
                    }
                }

                foreach (var pen in add)
                {
                    var properties = Pens.FirstOrDefault(p => p.Name.Equals(pen));
                    if (properties != null)
                        Pens.Remove(properties);
                }

                foreach (var pen in add)
                {
                    var newPen = new Pen();
                    newPen.Name = pen;
                    var properties = new PenProperties(Pens, newPen);
                    properties.Name = pen;
                    Pens.Add(properties);
                    properties.Color = TrendObject.GetColor(Pens.IndexOf(properties));
                    _collectionChanged = true;
                }
                UpdateOtherPens();
            }
        }

        private bool CanExecuteAddConfigureTagsCommand()
        {
            if (_trend is TrendLog) return false;
            return true;
        }

        private bool _collectionChanged = false;
        private string _thereAreNoCurve;
        public RelayCommand DeletePenCommand { get; }

        private void ExecuteDeletePenCommand()
        {
            var temp = new List<PenProperties>();
            foreach (PenProperties selectedItem in SelectedItems)
            {
                temp.Add(selectedItem);
            }

            foreach (var selectedItem in temp)
            {
                Pens.Remove(selectedItem);
                _collectionChanged = true;
            }

            DeletePenCommand.RaiseCanExecuteChanged();
        }

        private bool CanExecuteDeletePenCommand()
        {
            if (SelectedItems?.Count > 0) return true;
            return false;
        }

        public RelayCommand ClearCommand { get; }

        private void ExecuteClearCommand()
        {
            var editPen = EditPens[0];
            editPen.Visible = "On";
            editPen.Width = 1;
            editPen.Type = Type.Analog;
            editPen.Style = Style.Style1;
            editPen.Marker = MarkerType.None;
            editPen.Min = "0.000000";
            editPen.Max = "100.000000";
        }

        public RelayCommand ApplyCommand { get; }

        private void ExecuteApplyCommand()
        {
            if (SelectedItems == null || SelectedItems.Count == 0)
            {
                MessageBox.Show(
                    _thereAreNoCurve,
                    "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            var editPen = EditPens[0];
            foreach (PenProperties selectedItem in SelectedItems)
            {
                selectedItem.ApplyEditPen(editPen);
            }
        }

        public RelayCommand DeleteCommand { get; }

        private void ExecuteDeleteCommand()
        {
            if (CanExecuteDeletePenCommand())
            {
                ExecuteDeletePenCommand();
            }
        }

        #endregion

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

        public void Save()
        {
            if(IsClosing||!IsDirty)return;
            if (_collectionChanged)
            {
                _trend.ClearPens();
            }

            var pens =new List<IPen>();
            foreach (var properties in Pens)
            {
                {
                    var pen = properties.Pen as PenObject;
                    if (pen != null)
                    {
                        pen.Name = properties.Name;
                        pen.Color = $"16{properties.Color.ToString()}";
                        //var v = properties.Visible.Equals("On");
                        //if (pen.Visible != v)
                        //{
                        //    ((TrendObject) _trend).PenChangedCount++;
                        //    pen.Visible = !pen.Visible;
                        //}
                        bool visible= properties.Visible.Equals("On");
                        pen.Visible = visible;
                        pen.Width = properties.Width;
                        pen.Type = properties.Type;
                        pen.Style = properties.Style;
                        pen.Marker = (int) properties.Marker;
                        pen.Min = float.Parse(properties.Min);
                        pen.Max = float.Parse(properties.Max);
                        pen.Description = properties.Description;
                        pen.Units = properties.Units;
                        if (_collectionChanged)
                            pens.Add(pen);
                    }
                }

                //TODO(ZYL):add upper and lower

            }
            if(_collectionChanged)
                _trend.AddPens(pens);
            CollectionChanged?.Invoke(_trend.Pens,new EventArgs());
            ((TrendObject) _trend).UpdateIsolated = true;
            _collectionChanged = false;
            IsDirty = false;
        }
        
        public bool Verify()
        {
            foreach (var pen in Pens)
            {
                var min = float.Parse(pen.Min);
                var max = float.Parse(pen.Max);
                if (min >= max)
                {
                    MessageBox.Show("Please enter a minimum value less than the maximum value.",
                        "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return false;
                }
            }

            return true;
        }

        private void SetPens()
        {
            Pens.Clear();
            foreach (var pen in _trend.Pens)
            {
                var properties = new PenProperties(Pens, pen);
                properties.Name = pen.Name;
                properties.Description = pen.Description;

                Color color;
                try
                {
                    color = (Color) ColorConverter.ConvertFromString($"#{FormatOp.RemoveFormat(pen.Color, false)}");
                }
                catch (Exception)
                {
                    color = Colors.Blue;
                }
                color.A = 0xff;
                properties.Color = color;
                properties.Visible = pen.Visible ? "On" : "Off";
                properties.Style = pen.Style;
                properties.Type = pen.Type;
                properties.Width = pen.Width;
                properties.Units = pen.Units;
                properties.Marker = (MarkerType) pen.Marker;
                properties.Min = pen.Min.ToString("0.000000");
                properties.Max = pen.Max.ToString("0.000000");
                Pens.Add(properties);
            }
        }

        private void UpdateOtherPens()
        {
            foreach (var penProperties in Pens)
            {
                penProperties.UpdateOtherPenList();
            }
        }

        private void Pens_CollectionChanged(object sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (PenProperties item in e.NewItems)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (PenProperties item in e.OldItems)
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }

            IsDirty = true;
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            IsDirty = true;
        }

    }

    public class PenProperties : ViewModelBase
    {
        private string _visible;
        private Color _color = Colors.Blue;
        private readonly ObservableCollection<PenProperties> _parentPens;
        private int _width = 1;
        private Type _type;
        private Style _style;
        private MarkerType _marker;
        private string _min = "0.000000";
        private string _max = "100.000000";
        private readonly IPen _pen;
        private string _units;

        public PenProperties(ObservableCollection<PenProperties> parentPens, IPen pen)
        {
            _pen = pen;
            _parentPens = parentPens;
            Visible = "On";
            VisibleCommand = new RelayCommand(ExecuteVisibleCommand);
            Types = EnumHelper.ToDataSource<Interfaces.Common.Type>();
            Styles = EnumHelper.ToDataSource<Style>();
            Markers = EnumHelper.ToDataSource<MarkerType>();
        }

        public IPen Pen => _pen;

        public void UpdateOtherPenList()
        {
            var temp = new List<string>();
            temp.Add(null);
            foreach (var pen in _parentPens.Where(p => !p.Name.Equals(Name)))
            {
                temp.Add(pen.Name);
            }

            OtherPens = temp.Select(p =>
            {
                if (p == null) return new {DisplayName = "None", Value = ""};
                var index = GetIndex(p);
                return new {DisplayName = $"Pen {index + 1}", Value = p};
            }).ToList();
            RaisePropertyChanged("OtherPens");
        }

        private int GetIndex(string name)
        {
            var pen = _parentPens.FirstOrDefault(p => p.Name.Equals(name));
            if (pen == null) return -1;
            var index = _parentPens.IndexOf(pen);
            return index;
        }

        public IList OtherPens { set; get; }
        public string Upper { set; get; } = "";
        public string Lower { set; get; } = "";
        public string Description { set; get; }

        public string Min
        {
            set
            {
                var s = Format(value);
                Set(ref _min, s);
            }
            get { return _min; }
        }

        public string Max
        {
            set
            {
                var s = Format(value);
                Set(ref _max, s);
            }
            get { return _max; }
        }

        public MarkerType Marker
        {
            set
            {
                Set(ref _marker, value);
                RaisePropertyChanged("MarkerS");
            }
            get { return _marker; }
        }

        public string MarkerS
        {
            get
            {
                var e = Attribute.GetCustomAttribute(Marker.GetType().GetField(Marker.ToString()),
                    typeof(EnumMemberAttribute)) as EnumMemberAttribute;
                return e?.Value ?? Marker.ToString();
            }
        }

        public IList Markers { get; }

        public Style Style
        {
            set
            {
                Set(ref _style, value);
                RaisePropertyChanged("StyleS");
            }
            get { return _style; }
        }

        public string StyleS
        {
            get
            {
                var e = Attribute.GetCustomAttribute(Style.GetType().GetField(Style.ToString()),
                    typeof(EnumMemberAttribute)) as EnumMemberAttribute;
                return e?.Value;
            }
        }

        public IList Styles { get; }

        public Type Type
        {
            set
            {
                Set(ref _type, value);
                RaisePropertyChanged("TypeS");
            }
            get { return _type; }
        }

        public string TypeS
        {
            get
            {
                var e = Attribute.GetCustomAttribute(Type.GetType().GetField(Type.ToString()),
                    typeof(EnumMemberAttribute)) as EnumMemberAttribute;
                return e?.Value ?? Type.ToString();
            }
        }

        public IList Types { get; }
        public string Name { set; get; }

        public string Units
        {
            set { Set(ref _units , value); }
            get { return _units; }
        }

        public Color Color
        {
            set { Set(ref _color, value); }
            get { return _color; }
        }

        public string Visible
        {
            set { Set(ref _visible, value); }
            get { return _visible; }
        }

        public List<int> WidthList { get; } = new List<int>() {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};

        public int Width
        {
            set { Set(ref _width, value); }
            get { return _width; }
        }

        public RelayCommand VisibleCommand { get; }

        public void ApplyEditPen(PenProperties editPen)
        {
            Visible = editPen.Visible;
            Width = editPen.Width;
            Type = editPen.Type;
            Style = editPen.Style;
            Marker = editPen.Marker;
            Min = editPen.Min;
            Max = editPen.Max;
            Units = editPen.Units;
        }

        private void ExecuteVisibleCommand()
        {
            Visible = Visible.Equals("On") ? "Off" : "On";
        }

        private string Format(string value)
        {
            try
            {
                var f = float.Parse(value);
                return f.ToString("0.000000");
            }
            catch (Exception)
            {
                return "0.000000";
            }
        }
    }
}
