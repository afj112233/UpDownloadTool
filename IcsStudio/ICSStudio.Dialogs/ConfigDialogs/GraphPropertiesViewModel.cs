using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Gui.Annotations;
using OxyPlot;

namespace ICSStudio.Dialogs.ConfigDialogs
{
    class GraphPropertiesViewModel : INotifyPropertyChanged
    {
        private CamEditorOptions _camOptions;
        
        private readonly CamEditorViewModel _camEditorViewModel;
        private bool? _graphPropertiesResult;
        private double _masterVelocity;

        public GraphPropertiesViewModel(CamEditorViewModel camEditorViewModel)
        {
            _camEditorViewModel = camEditorViewModel;
            
            _camOptions = camEditorViewModel.Options.Clone() as CamEditorOptions;

            DataItemsSource = new ObservableCollection<AxisOptions>();
            _masterVelocity = _camOptions.MasterVelocity;

            DataItemsSource.Add(_camOptions.Position);
            DataItemsSource.Add(_camOptions.Velocity);
            DataItemsSource.Add(_camOptions.Acceleration);
            DataItemsSource.Add(_camOptions.Jerk);

            OKCommand = new RelayCommand(ExecuteOKCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            ApplyCommand = new RelayCommand(ExecuteApplyCommand, CanExecuteApplyCommand);
            
            _camOptions.PropertyChanged += OnPropertyChanged;
        }

        public double MasterVelocity
        {
            get { return _masterVelocity;}
            set
            {
                if (_masterVelocity != value)
                {
                    _masterVelocity = value;
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ApplyCommand.RaiseCanExecuteChanged();
        }

        private bool CanExecuteApplyCommand()
        {
            //TODO(TLM):只有第一遍APPLY会好用 待解决
            return Equals(_camOptions, _camEditorViewModel.Options);
        }

        private bool Equals(CamEditorOptions options0, CamEditorOptions options1)
        {
            bool result = AxisEquals(options0.Position, options1.Position)
                   || AxisEquals(options0.Velocity, options1.Velocity)
                   || AxisEquals(options0.Acceleration, options1.Acceleration)
                   || AxisEquals(options0.Jerk, options1.Jerk)
                   || options0.EnableGrid != options1.EnableGrid
                   || options0.GridColor != options1.GridColor
                   || options0.MasterVelocity != _masterVelocity;
            return result;
        }

        private bool AxisEquals(AxisOptions a , AxisOptions b)
        {
            bool result = a.Color != b.Color ||
                   a.Visible != b.Visible ||
                   a.Marker != b.Marker ||
                   a.Width != b.Width ||
                   a.SlaveValue != b.SlaveValue ||
                   a.Style != b.Style;
            return result;
        }

        private void Apply(CamEditorOptions editOptions, CamEditorOptions originalOptions)
        {
            originalOptions.Position = editOptions.Position;
            originalOptions.Velocity = editOptions.Velocity;
            originalOptions.Acceleration = editOptions.Acceleration;
            originalOptions.Jerk = editOptions.Jerk;
            originalOptions.EnableGrid = editOptions.EnableGrid;
            originalOptions.GridColor = editOptions.GridColor;
            originalOptions.MasterVelocity = _masterVelocity;
        }

        private void ExecuteApplyCommand()
        {
            Apply(_camOptions, _camEditorViewModel.Options);
            
            _camOptions.Save(MasterVelocity);

            _camEditorViewModel.UpdateColor();
            _camEditorViewModel.UpdatePlotModel();
            
            ApplyCommand.RaiseCanExecuteChanged();
        } 


        private void ExecuteCancelCommand()
        {
            GraphPropertiesResult = false;
        }

        private void ExecuteOKCommand()
        {
            ExecuteApplyCommand();

            GraphPropertiesResult = false;
        }

        public bool? GraphPropertiesResult
        {
            get { return _graphPropertiesResult;}
            set
            {
                if (_graphPropertiesResult != value)
                {
                    _graphPropertiesResult = value;
                    OnPropertyChanged(nameof(GraphPropertiesResult));
                }
            }
        }

        public RelayCommand OKCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand ApplyCommand { get; }

        //更改命名
        public ObservableCollection<AxisOptions> DataItemsSource { get; set; }

        public OxyColor SelectedColor { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
