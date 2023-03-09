using System;
using System.Collections;
using GalaSoft.MvvmLight;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.Utils.CamEditorUtil;
using CursorType = OxyPlot.CursorType;
using MessageBox = System.Windows.MessageBox;
using TickStyle = OxyPlot.Axes.TickStyle;
using ICSStudio.MultiLanguage;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;

namespace ICSStudio.Dialogs.ConfigDialogs
{

    public enum SelectMode
    {
        All,
        Position,
        Velocity,
        Acceleration,
        Jerk
    }

    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public partial class CamEditorViewModel : ViewModelBase
    {
        private const string PositionKey = "Position";
        private const string VelocityKey = "Velocity";
        private const string AccelerationKey = "Acceleration";
        private const string JerkKey = "Jerk";
        private const string MasterKey = "Master";
        private string _xAxisTitle = LanguageManager.GetInstance().ConvertSpecifier("Master");
        private string _yAxisTitle = LanguageManager.GetInstance().ConvertSpecifier("Slave Position");
        private string _tipsTitle;
        private string _tips;

        public int PositionZoomNum;
        public int VelocityZoomNum;
        public int AccelerationZoomNum;
        public int JerkZoomNum;
        public int MasterZoomNum;
        public ArrayField CamArrayField;

        private readonly string _title;
        private readonly int _maxRowsNum;
        private readonly int _minRowsNum;

        private float _endSlope;
        private float _startSlope;
        private float _oldEndSlope;
        private float _oldStartSlope;

        private float _master;
        private float _position;
        private float _velocity;
        private float _acceleration;
        private float _jerk;

        private bool _initializing;
        private bool? _dialogResult;
        private bool _isCamProfile;
        private bool _canPointCreate;

        private Controller _controller;
        private CamEditorOptions _camOptions;
        public CommandManager CommandManager;
        private int _commandManagerLevel;

        private PlotModel _plotModel;
        private LinearAxis _positionAxis;
        private LinearAxis _velocityAxis;
        private LinearAxis _accelerationAxis;
        private LinearAxis _jerkAxis;
        private LinearAxis _masterAxis;

        private ArrayField _arrayField;
        private readonly ITag _tag;

        public CamEditorViewModel(ITag tag) : this((tag as Tag)?.DataWrapper.Data as ArrayField, tag.Name, tag)
        {
            Contract.Assert(tag != null);
        }

        public CamEditorViewModel(ArrayField field, string title, ITag parent)
        {
            _tag = parent;
            _arrayField = field;
            CamArrayField = _arrayField;

            _title = title;
            _initializing = true;
            PositionZoomNum = 0;
            VelocityZoomNum = 0;
            AccelerationZoomNum = 0;
            JerkZoomNum = 0;
            MasterZoomNum = 0;
            _endSlope = 0;
            _startSlope = 0;
            SelectMode = SelectMode.All;
            PreSelectMode = SelectMode.All;
            _canPointCreate = false;

            _maxRowsNum = _arrayField.fields.Count;
            _minRowsNum = _arrayField.fields.Count;

            _controller = SimpleServices.Common.Controller.GetInstance();

            OKCommand = new RelayCommand(ExecuteOKCommand);
            HelpCommand = new RelayCommand(ExecuteHelpCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            ApplyCommand = new RelayCommand(ExecuteApplyCommand, CanApplyCommandExecute);
            ChangeSelectModeCommand = new RelayCommand<SelectMode>(ChangeSelectMode);

            ZoomInCommand = new RelayCommand(ExecuteZoomInCommand);
            ZoomOutCommand = new RelayCommand(ExecuteZoomOutCommand);
            ZoomToFitCommand = new RelayCommand(ExecuteZoomToFitCommand);

            InsertCamSegmentsCommand =
                new RelayCommand(ExecuteInsertCamSegmentsCommand, CanInsertCamSegmentsCommandExecute);
            PropertiesCommand = new RelayCommand(ExecuteProperties);
            SetToLinearCommand = new RelayCommand(ExecuteSetToLinearCommand, CanSetToLinearCommandExecute);
            SetToCubicCommand = new RelayCommand(ExecuteSetToCubicCommand, CanSetToCubicCommandExecute);

            UndoCommand = new RelayCommand(ExecuteUndoCommand, CanUndoExecute);
            RedoCommand = new RelayCommand(ExecuteRedoCommand, CanRedoExecute);
            DeleteCommand = new RelayCommand(ExecuteDeleteCommand, CanDeleteCommandExecute);
            ContentRenderedCommand = new RelayCommand<EventArgs>(OnContentRenderedCommand);

            InsertCommand = new RelayCommand(ExecuteInsertCommand, CanInsertCommandExecute);
            CutCommand = new RelayCommand(ExecuteCutCommand, CanCutCommandExecute);
            CopyCommand = new RelayCommand(ExecuteCopyCommand, CanCopyCommandExecute);
            PasteCommand = new RelayCommand(ExecutePasteCommand, CanPasteCommandExecute);

            ClosingCommand = new RelayCommand<CancelEventArgs>(ExecuteClosingCommand);

            var camProfileField = field.fields[0].Item1 as CAM_PROFILEField;
            var camField = field.fields[0].Item1 as CAMField;

            Contract.Assert(camProfileField != null || camField != null);
            Contract.Assert(_arrayField.fields.Count > 0);

            _isCamProfile = camProfileField != null;

            CamPoints = new ObservableCollection<CamPoint>();
            CamPoints.CollectionChanged += PointsOnCollectionChanged;
            LoadCamPointsFromTag(CamPoints);

            _plotModel = new PlotModel()
            {
                PlotAreaBorderThickness = new OxyThickness(0, 0, 0, 0),
                PlotMargins = new OxyThickness(5, 5, 5, 5)
            };

            _camOptions = CamEditorOptions.Create();

            CreateAllAxis();
            RebuildSeriesAndPoints();

            _plotModel.MouseMove += PlotModelOnMouseMove;
            _plotModel.MouseDown += PlotModelOnMouseDown;
            _plotModel.MouseUp += (s, e) =>
            {
                // Stop editing
                if (_canPointToMove)
                {
                    _canPointToMove = false;
                    _plotModel.InvalidatePlot(false);
                    e.Handled = true;
                }
            };

            CommandManager = new CommandManager();
            _commandManagerLevel = 1;

            UpdateColor();
            UpdatePlotModel();
            _initializing = false;

            Controller = new PlotController();
            Controller.BindMouseEnter(
                new DelegatePlotCommand<OxyMouseEventArgs>((view, controller, args) =>
                    controller.AddHoverManipulator(view,
                        new PointAnnotationTrackerManipulator(view, CamPoints), args)));

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged",
                LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("CamTitle");
            _xAxisTitle = LanguageManager.GetInstance().ConvertSpecifier("Master");
            _yAxisTitle = LanguageManager.GetInstance().ConvertSpecifier("Slave Position");
            UpdateAxis();
            _plotModel.InvalidatePlot(false);
        }

        public bool IsReadOnlyEnabled
        {
            get
            {
                return _controller.OperationMode == ControllerOperationMode.OperationModeRun && _controller.IsOnline;
            }
        }

        private bool CanPasteCommandExecute()
        {
            return !(_controller.OperationMode == ControllerOperationMode.OperationModeRun && _controller.IsOnline);
        }

        private bool CanCopyCommandExecute()
        {
            return !(_controller.OperationMode == ControllerOperationMode.OperationModeRun && _controller.IsOnline);
        }

        private bool CanCutCommandExecute()
        {
            return !(_controller.OperationMode == ControllerOperationMode.OperationModeRun && _controller.IsOnline);
        }

        private bool CanInsertCommandExecute()
        {
            return !(_controller.OperationMode == ControllerOperationMode.OperationModeRun && _controller.IsOnline);
        }

        private void ExecuteClosingCommand(CancelEventArgs obj)
        {
            if (!CamDialogResult.HasValue)
            {
                obj.Cancel = !DoCheck();
            }
            else
            {
                if (CamDialogResult == true)
                {
                    obj.Cancel = false;
                }
                else
                {
                    CamDialogResult = null;
                    obj.Cancel = true;
                }
            }
        }

        public RelayCommand<CancelEventArgs> ClosingCommand { get; set; }

        private void ExecuteInsertCommand()
        {
            if (SelectedItems != null && SelectedItems.Count > 0)
            {
                int insertNumber = -1;
                foreach (var selectedItem in SelectedItems)
                {
                    if (selectedItem == null)
                    {
                        insertNumber = CamPoints.Count;
                        break;
                    }

                    var selectedIndex = CamPoints.IndexOf(selectedItem);

                    if (insertNumber < selectedIndex)
                    {
                        insertNumber = selectedIndex;
                    }
                }

                if (MaxRowsNum)
                {
                    var point = new CamPoint();
                    CamPoints.Insert(insertNumber, point);

                    UpdateCamPoints();

                    var addCommand = new AddCommand(this, insertNumber, point);
                    CommandManager.AddCommand(addCommand);
                    UpdateUndoRedoCanExecute();
                }
                else
                {
                    _tips = "Too many segments. Cam array is full.";
                    _tipsTitle = "Cam Editor";

                    MessageBox.Show(_tips, _tipsTitle,
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        public void UpdateCamPoints()
        {
            //更新数据源，刷新数据列表保证序号正确
            var replaceCamPoints = new ObservableCollection<CamPoint>();
            foreach (var camPoint in CamPoints)
            {
                replaceCamPoints.Add(camPoint);
            }

            CamPoints.Clear();
            foreach (var camPoint in replaceCamPoints)
            {
                CamPoints.Add(camPoint);
            }
        }

        private void ExecutePasteCommand()
        {
            //TODO(TLM):
        }

        private void ExecuteCopyCommand()
        {
            string copyList = "";

            foreach (var point in SelectedItems)
            {
                copyList += point.Master + "\t" +
                            point.Slave + "\t" +
                            point.Type + "\t" +
                            point.Status + "\t" +
                            point.C0 + "\t" +
                            point.C1 + "\t" +
                            point.C2 + "\t" +
                            point.C3 + "\r\n";
            }

            Clipboard.SetDataObject(copyList);
        }

        private void ExecuteCutCommand()
        {
            //TODO(TLM):
        }

        public RelayCommand InsertCommand { get; }
        public RelayCommand CutCommand { get; }
        public RelayCommand CopyCommand { get; }
        public RelayCommand PasteCommand { get; }

        private void OnChanged(IList dataset)
        {
            if (dataset != null && dataset.Count > 0)
            {
                SelectedItems = new ObservableCollection<CamPoint>();

                foreach (var point in dataset)
                {
                    var a = point as CamPoint;
                    if (a != null)
                    {
                        SelectedItems.Add(a);
                    }
                }

                List<int> order = new List<int>();
                foreach (var item in SelectedItems)
                {
                    var a = CamPoints.IndexOf(item);
                    if (a > -1)
                    {
                        order.Add(a);
                    }
                }

                order.Sort();

                SelectedItems.Clear();
                foreach (var item in order)
                {
                    SelectedItems.Add(CamPoints[item]);
                }
            }
        }

        private RelayCommand<IList> _selectionChangedCommand;

        public RelayCommand<IList> SelectionChangedCommand => _selectionChangedCommand ??
                                                              (_selectionChangedCommand =
                                                                  new RelayCommand<IList>(OnChanged));

        public List<string> SelectedCells { get; set; }

        private bool CanDeleteCommandExecute()
        {
            return SelectedItems != null && !(_controller.OperationMode == ControllerOperationMode.OperationModeRun &&
                                              _controller.IsOnline);
        }

        private void ExecuteDeleteCommand()
        {
            if (SelectedItems != null && SelectedItems.Count > 0)
            {
                List<Tuple<int, CamPoint>> deleteList = new List<Tuple<int, CamPoint>>();

                foreach (CamPoint point in SelectedItems)
                {
                    if (point != null)
                    {
                        Tuple<int, CamPoint> tuple = new Tuple<int, CamPoint>(CamPoints.IndexOf(point), point);
                        deleteList.Add(tuple);
                    }
                }

                //Do Delete
                if (deleteList.Count > 0)
                {
                    DeletePoint(deleteList);
                }
            }
        }

        private void ExecuteSetToCubicCommand()
        {
            if (SelectedItems != null && SelectedItems.Count > 0)
            {
                foreach (CamPoint point in SelectedItems)
                {
                    int index = CamPoints.IndexOf(point);
                    if (index >= 0)
                    {
                        if (CamPoints[index].Type != SegmentType.Cubic)
                        {
                            CamPoints[index].Type = SegmentType.Cubic;
                        }
                    }

                }
            }
        }

        private bool CanSetToCubicCommandExecute()
        {
            return !(_controller.OperationMode == ControllerOperationMode.OperationModeRun && _controller.IsOnline);
        }

        private void ExecuteSetToLinearCommand()
        {
            if (SelectedItems != null && SelectedItems.Count > 0)
            {
                foreach (CamPoint point in SelectedItems)
                {
                    int index = CamPoints.IndexOf(point);
                    if (index >= 0)
                    {
                        if (CamPoints[index].Type != SegmentType.Linear)
                        {
                            CamPoints[index].Type = SegmentType.Linear;
                        }
                    }

                }
            }
        }

        private bool CanSetToLinearCommandExecute()
        {
            return !(_controller.OperationMode == ControllerOperationMode.OperationModeRun && _controller.IsOnline);
        }

        public PlotController Controller { get; set; }



        private void ExecuteRedoCommand()
        {
            CommandManager.Redo(_commandManagerLevel);
            UpdateUndoRedoCanExecute();
        }

        private void ExecuteUndoCommand()
        {
            CommandManager.Undo(_commandManagerLevel);
            UpdateUndoRedoCanExecute();
        }

        public void UpdateUndoRedoCanExecute()
        {
            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();
        }

        public RelayCommand OKCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand ApplyCommand { get; }
        public RelayCommand HelpCommand { get; }
        public RelayCommand<SelectMode> ChangeSelectModeCommand { get; }
        public RelayCommand ZoomInCommand { get; }
        public RelayCommand ZoomOutCommand { get; }
        public RelayCommand ZoomToFitCommand { get; }
        public RelayCommand InsertCamSegmentsCommand { get; }

        public RelayCommand SetToLinearCommand { get; }

        public RelayCommand SetToCubicCommand { get; }

        public RelayCommand UndoCommand { get; }

        public RelayCommand RedoCommand { get; }

        public RelayCommand PropertiesCommand { get; }
        public RelayCommand DeleteCommand { get; }

        public RelayCommand<EventArgs> ContentRenderedCommand { get; }

        public CamEditorOptions Options => _camOptions;

        public bool CanUndoExecute()
        {
            return CommandManager.CanUndo && !(_controller.OperationMode == ControllerOperationMode.OperationModeRun &&
                                               _controller.IsOnline);
        }

        public bool CanRedoExecute()
        {
            return CommandManager.CanRedo && !(_controller.OperationMode == ControllerOperationMode.OperationModeRun &&
                                               _controller.IsOnline);
        }

        private void ExecuteProperties()
        {
            var dialog = new GraphProperties(this);
            dialog.Owner = Application.Current.MainWindow;
            dialog.ShowDialog();
        }

        public Color PositionButtonColor { get; set; }

        public float Master
        {
            get { return _master; }
            set { Set(ref _master, value); }
        }

        public float Position
        {
            get { return _position; }
            set { Set(ref _position, value); }
        }

        public float Velocity
        {
            get { return _velocity; }
            set { Set(ref _velocity, value); }
        }

        public float Acceleration
        {
            get { return _acceleration; }
            set { Set(ref _acceleration, value); }
        }

        public float Jerk
        {
            get { return _jerk; }
            set { Set(ref _jerk, value); }
        }

        public SolidColorBrush ColorPosition { get; set; }
        public SolidColorBrush ColorVelocity { get; set; }
        public SolidColorBrush ColorAcceleration { get; set; }
        public SolidColorBrush ColorJerk { get; set; }

        private void PlotModelOnMouseMove(object sender, OxyMouseEventArgs e)
        {
            if (_canPointToMove)
            {
                if (CamPoints.Count > 1)
                {
                    if (_indexOfCamPointsNum == 0)
                    {
                        if (_masterAxis.InverseTransform(e.Position.X) >
                            CamPoints[_indexOfCamPointsNum + 1].Master)
                        {
                            CamPoints[_indexOfCamPointsNum].Master =
                                CamPoints[_indexOfCamPointsNum + 1].Master - 0.00333333f;
                            CamPoints[_indexOfCamPointsNum].Slave =
                                (float)_positionAxis.InverseTransform(e.Position.Y);
                        }
                        else
                        {
                            CamPoints[_indexOfCamPointsNum].Master =
                                (float)_masterAxis.InverseTransform(e.Position.X);
                            CamPoints[_indexOfCamPointsNum].Slave =
                                (float)_positionAxis.InverseTransform(e.Position.Y);
                        }
                    }
                    else if (_indexOfCamPointsNum == CamPoints.Count - 1)
                    {
                        if (_masterAxis.InverseTransform(e.Position.X) <
                            CamPoints[_indexOfCamPointsNum - 1].Master)
                        {
                            CamPoints[_indexOfCamPointsNum].Master =
                                CamPoints[_indexOfCamPointsNum - 1].Master + 0.00333333f;
                            CamPoints[_indexOfCamPointsNum].Slave =
                                (float)_positionAxis.InverseTransform(e.Position.Y);
                        }
                        else
                        {
                            CamPoints[_indexOfCamPointsNum].Master =
                                (float)_masterAxis.InverseTransform(e.Position.X);
                            CamPoints[_indexOfCamPointsNum].Slave =
                                (float)_positionAxis.InverseTransform(e.Position.Y);
                        }
                    }
                    else
                    {
                        if (_masterAxis.InverseTransform(e.Position.X) <
                            CamPoints[_indexOfCamPointsNum - 1].Master)
                        {
                            CamPoints[_indexOfCamPointsNum].Master =
                                CamPoints[_indexOfCamPointsNum - 1].Master + 0.00333333f;
                            CamPoints[_indexOfCamPointsNum].Slave =
                                (float)_positionAxis.InverseTransform(e.Position.Y);
                        }
                        else if (_masterAxis.InverseTransform(e.Position.X) >
                                 CamPoints[_indexOfCamPointsNum + 1].Master)
                        {
                            CamPoints[_indexOfCamPointsNum].Master =
                                CamPoints[_indexOfCamPointsNum + 1].Master - 0.00333333f;
                            CamPoints[_indexOfCamPointsNum].Slave =
                                (float)_positionAxis.InverseTransform(e.Position.Y);
                        }
                        else
                        {
                            CamPoints[_indexOfCamPointsNum].Master =
                                (float)_masterAxis.InverseTransform(e.Position.X);
                            CamPoints[_indexOfCamPointsNum].Slave =
                                (float)_positionAxis.InverseTransform(e.Position.Y);
                        }
                    }
                }
                else
                {
                    CamPoints[_indexOfCamPointsNum].Master = (float)_masterAxis.InverseTransform(e.Position.X);
                    CamPoints[_indexOfCamPointsNum].Slave = (float)_positionAxis.InverseTransform(e.Position.Y);
                }

                _plotModel.InvalidatePlot(false);
                e.Handled = true;
            }

            Master = (float)_masterAxis.InverseTransform(e.Position.X);
            Position = (float)_positionAxis.InverseTransform(e.Position.Y);

            Master = Master;
            Position = Position;

            List<CamProfile> profiles = new List<CamProfile>();

            if (CamPoints.Count > 1 && Master > CamPoints[CamPoints.Count - 1].Master)
            {
                Velocity = 0;
                Acceleration = 0;
                Jerk = 0;
            }
            else
            {
                double x0 = 0;
                double y0 = 0;
                double c0 = 0;
                double c1 = 0;
                double c2 = 0;
                double c3 = 0;
                double pdx = 0;
                for (int i = 0; i < CamPoints.Count; i++)
                {
                    if (Master < CamPoints[i].Master)
                    {
                        if (i != 0)
                        {
                            pdx = Master - CamPoints[i - 1].Master;
                            x0 = CamPoints[i - 1].Master;
                            y0 = CamPoints[i - 1].Slave;
                            c0 = CamPoints[i - 1].C0;
                            c1 = CamPoints[i - 1].C1;
                            c2 = CamPoints[i - 1].C2;
                            c3 = CamPoints[i - 1].C3;
                            break;
                        }

                        //当鼠标在整个坐标系的左侧时也有值 是最左一段曲线的延长线 延用第一段曲线的值
                        pdx = Master - CamPoints[i].Master;
                        x0 = CamPoints[0].Master;
                        y0 = CamPoints[0].Slave;
                        c0 = CamPoints[0].C0;
                        c1 = CamPoints[0].C1;
                        c2 = CamPoints[0].C2;
                        c3 = CamPoints[0].C3;
                        break;
                    }
                }

                Velocity = (float)((c1 + 2 * c2 * pdx + 3 * c3 * pdx * pdx) * _camOptions.MasterVelocity);
                Acceleration =
                    (float)((2 * c2 + 6 * c3 * pdx) * _camOptions.MasterVelocity * _camOptions.MasterVelocity);
                Jerk = (float)((6 * c3) * _camOptions.MasterVelocity * _camOptions.MasterVelocity *
                               _camOptions.MasterVelocity);
            }
        }

        public SelectMode SelectMode { get; protected set; }
        public SelectMode PreSelectMode { get; protected set; }

        public string CamTitle
        {
            get { return LanguageManager.GetInstance().ConvertSpecifier("Cam Editor -") + " " + _title; }
        }


        public bool? CamDialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }


        public float StartSlope
        {
            get { return _startSlope; }
            set
            {
                if (value != _startSlope)
                {
                    _startSlope = value;

                    UpdatePlotModel();
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public float EndSlope
        {
            get { return _endSlope; }
            set
            {
                if (value != _endSlope)
                {
                    _endSlope = value;

                    UpdatePlotModel();
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public ObservableCollection<CamPoint> CamPoints { get; set; }

        public ObservableCollection<CamPoint> SelectedItems { get; set; }

        public CamPoint SelectedItem { get; set; }

        public bool MaxRowsNum => CamPoints.Count < _maxRowsNum;
        public bool MinRowsNum => _minRowsNum > 0;

        private void ChangeSelectMode(SelectMode mode)
        {
            PreSelectMode = SelectMode;
            SelectMode = SelectMode == mode ? SelectMode.All : mode;

            UpdateAxis();

            if (SelectMode != SelectMode.All)
            {
                for (int i = 0; i < _plotModel.Axes.Count - 1; i++)
                {
                    if (_plotModel.Axes[i].IsPanEnabled)
                    {
                        var temp = _plotModel.Axes[i];
                        _plotModel.Axes.Remove(temp);
                        _plotModel.Axes.Insert(0, temp);
                    }
                }
            }

            _plotModel.InvalidatePlot(false);
        }

        private bool CanApplyCommandExecute()
        {
            return IsDirty() && !(_controller.OperationMode == ControllerOperationMode.OperationModeRun &&
                                  _controller.IsOnline);
        }

        private void ExecuteInsertCamSegmentsCommand()
        {
            _canPointCreate = true;
        }

        private bool CanInsertCamSegmentsCommandExecute()
        {
            return !(_controller.OperationMode == ControllerOperationMode.OperationModeRun && _controller.IsOnline);
        }

        private void ExecuteOKCommand()
        {
            if (DoApply() == 0)
                CamDialogResult = true;
        }

        private void ExecuteApplyCommand()
        {
            DoApply();
            ApplyCommand.RaiseCanExecuteChanged();
        }

        private void ExecuteCancelCommand()
        {
            CamDialogResult = DoCheck();
        }

        private bool DoCheck()
        {
            if (IsDirty())
            {
                _tipsTitle = "ICS Studio";
                _tips = "Cam Profile data has changed. Save changes ?";

                var dr = MessageBox.Show(_tips, _tipsTitle,
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
                switch (dr)
                {
                    case MessageBoxResult.Yes:
                        ExecuteOKCommand();
                        break;
                    case MessageBoxResult.No:
                        return true;
                    case MessageBoxResult.Cancel:
                        return false;
                }
            }

            return true;
        }

        private void ExecuteHelpCommand()
        {
            //TODO(tlm): add code here
        }

        public void DeletePoint(List<Tuple<int, CamPoint>> deleteList)
        {
            foreach (var point in deleteList)
            {
                CamPoints.Remove(point.Item2);
            }

            var deleteCommand = new DeleteCommand(this, deleteList);
            CommandManager.AddCommand(deleteCommand);
            UpdateUndoRedoCanExecute();
            UpdateCamPoints();
            //更新视图
            UpdatePlotModel();
        }

        public void UpdateView()
        {
            //通知界面 把界面所有元素重新构造一遍 ----强制刷新
            var defaultView = CollectionViewSource.GetDefaultView(CamPoints) as ListCollectionView;
            defaultView?.Refresh();
        }

        private void ExecuteZoomInCommand()
        {
            switch (SelectMode)
            {
                case SelectMode.Position:
                    PositionZoomNum++;
                    ZoomIn(PositionZoomNum, _positionAxis);
                    break;
                case SelectMode.Velocity:
                    VelocityZoomNum++;
                    ZoomIn(VelocityZoomNum, _velocityAxis);
                    break;
                case SelectMode.Acceleration:
                    AccelerationZoomNum++;
                    ZoomIn(AccelerationZoomNum, _accelerationAxis);
                    break;
                case SelectMode.Jerk:
                    JerkZoomNum++;
                    ZoomIn(JerkZoomNum, _jerkAxis);
                    break;
                case SelectMode.All:
                    PositionZoomNum++;
                    ZoomIn(PositionZoomNum, _positionAxis);
                    VelocityZoomNum++;
                    ZoomIn(VelocityZoomNum, _velocityAxis);
                    AccelerationZoomNum++;
                    ZoomIn(AccelerationZoomNum, _accelerationAxis);
                    JerkZoomNum++;
                    ZoomIn(JerkZoomNum, _jerkAxis);
                    MasterZoomNum++;
                    ZoomIn(MasterZoomNum, _masterAxis);
                    break;
            }

            _plotModel.InvalidatePlot(false);


            _masterAxis.Pan(_masterZoomNum * Math.Abs(_masterAxis.Scale));
            _positionAxis.Pan(_positionZoomNum * Math.Abs(_positionAxis.Scale));
            _velocityAxis.Pan(_velocityZoomNum * Math.Abs(_velocityAxis.Scale));
            _accelerationAxis.Pan(_accelerationZoomNum * Math.Abs(_accelerationAxis.Scale));
            _jerkAxis.Pan(_jerkZoomNum * Math.Abs(_jerkAxis.Scale));
        }

        private double _masterZoomNum = 0.0;
        private double _positionZoomNum = 0.0;
        private double _velocityZoomNum = 0.0;
        private double _accelerationZoomNum = 0.0;
        private double _jerkZoomNum = 0.0;

        private void ZoomIn(int zoomNum, Axis axis)
        {
            double multipleNum = 0.0;

            if (zoomNum % 3 == 0)
            {
                multipleNum = 2.5;
            }
            else
            {
                multipleNum = 2;
            }

            if (axis.Key == "Master")
            {
                _masterZoomNum = -((axis.ActualMaximum - axis.ActualMinimum) / (multipleNum * multipleNum));
            }

            axis.ZoomAt(multipleNum, 0);
            axis.MajorStep /= multipleNum;

            switch (axis.Key)
            {
                case "Position":
                    _positionZoomNum = ((axis.ActualMaximum - axis.ActualMinimum) / (multipleNum * multipleNum));
                    break;
                case "Velocity":
                    _velocityZoomNum = ((axis.ActualMaximum - axis.ActualMinimum) / (multipleNum * multipleNum));
                    break;
                case "Acceleration":
                    _accelerationZoomNum = ((axis.ActualMaximum - axis.ActualMinimum) / (multipleNum * multipleNum));
                    break;
                case "Jerk":
                    _jerkZoomNum = ((axis.ActualMaximum - axis.ActualMinimum) / (multipleNum * multipleNum));
                    break;
            }
        }

        private void ExecuteZoomOutCommand()
        {
            switch (SelectMode)
            {
                case SelectMode.Position:
                    PositionZoomNum--;
                    ZoomOut(PositionZoomNum, _positionAxis);
                    break;
                case SelectMode.Velocity:
                    VelocityZoomNum--;
                    ZoomOut(VelocityZoomNum, _velocityAxis);
                    break;
                case SelectMode.Acceleration:
                    AccelerationZoomNum--;
                    ZoomOut(AccelerationZoomNum, _accelerationAxis);
                    break;
                case SelectMode.Jerk:
                    JerkZoomNum--;
                    ZoomOut(JerkZoomNum, _jerkAxis);
                    break;
                case SelectMode.All:
                    PositionZoomNum--;
                    ZoomOut(PositionZoomNum, _positionAxis);
                    VelocityZoomNum--;
                    ZoomOut(VelocityZoomNum, _velocityAxis);
                    AccelerationZoomNum--;
                    ZoomOut(AccelerationZoomNum, _accelerationAxis);
                    JerkZoomNum--;
                    ZoomOut(JerkZoomNum, _jerkAxis);
                    MasterZoomNum--;
                    ZoomOut(MasterZoomNum, _masterAxis);
                    break;
            }

            _plotModel.InvalidatePlot(false);

            _masterAxis.Pan(_masterZoomNum * Math.Abs(_masterAxis.Scale));
            _positionAxis.Pan(_positionZoomNum * Math.Abs(_positionAxis.Scale));
            _velocityAxis.Pan(_velocityZoomNum * Math.Abs(_velocityAxis.Scale));
            _accelerationAxis.Pan(_accelerationZoomNum * Math.Abs(_accelerationAxis.Scale));
            _jerkAxis.Pan(_jerkZoomNum * Math.Abs(_jerkAxis.Scale));
        }

        private void ExecuteZoomToFitCommand()
        {
            switch (SelectMode)
            {
                case SelectMode.Position:
                    ZoomToFitPositionAxis();
                    break;
                case SelectMode.Velocity:
                    ZoomToFitVelocityAxis();
                    break;
                case SelectMode.Acceleration:
                    ZoomToFitAccelerationAxis();
                    break;
                case SelectMode.Jerk:
                    ZoomToFitJerkAxis();
                    break;
                case SelectMode.All:
                    ZoomToFitAllAxes();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            _plotModel.InvalidatePlot(false);
        }

        private void ZoomOut(int zoomNum, Axis axis)
        {
            double multipleNum = 0;

            if (zoomNum % 3 == 2 || zoomNum % 3 == -1)
            {
                multipleNum = 0.4;
            }
            else
            {
                multipleNum = 0.5;
            }

            axis.ZoomAt(multipleNum, 0);
            axis.MajorStep /= multipleNum;

            if (axis.Key == "Master")
            {
                _masterZoomNum = ((axis.ActualMaximum - axis.ActualMinimum) * (multipleNum * multipleNum));
            }

            switch (axis.Key)
            {
                case "Position":
                    _positionZoomNum = -((axis.ActualMaximum - axis.ActualMinimum) * (multipleNum * multipleNum));
                    break;
                case "Velocity":
                    _velocityZoomNum = -((axis.ActualMaximum - axis.ActualMinimum) * (multipleNum * multipleNum));
                    break;
                case "Acceleration":
                    _accelerationZoomNum = -((axis.ActualMaximum - axis.ActualMinimum) * (multipleNum * multipleNum));
                    break;
                case "Jerk":
                    _jerkZoomNum = -((axis.ActualMaximum - axis.ActualMinimum) * (multipleNum * multipleNum));
                    break;
            }
        }

        private int CheckData()
        {
            int result = 0;
            for (int i = 1; i < CamPoints.Count; i++)
            {
                if (CamPoints[i].Master <= CamPoints[i - 1].Master)
                {
                    _tipsTitle = "ICS Studio";
                    _tips = "Bad Cam Segment at array index ";

                    MessageBox.Show(_tips + i, _tipsTitle,
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);

                    result = -1;
                    break;
                }
            }

            return result;
        }

        private void CopyData()
        {
            //TODO(TLM):只需要把修改的值进行重新赋值即可，未作修改的值不参与CopyData

            ArrayField arrayField = _arrayField;
            Contract.Assert(arrayField != null);

            // slope
            _oldStartSlope = _startSlope;
            _oldEndSlope = _endSlope;

            // data
            if (_isCamProfile)
            {
                int i;

                bool isDataDirty = false;

                List<int> ChangedDataNum = new List<int>();

                for (i = 0; i < CamPoints.Count; i++)
                {
                    CAM_PROFILEField camField = arrayField.fields[i].Item1 as CAM_PROFILEField;

                    if (camField != null)
                    {
                        if (((Int32Field)camField.fields[0].Item1).value != CamPoints[i].Status)
                        {
                            ((Int32Field)camField.fields[0].Item1).value = CamPoints[i].Status; // Status
                            isDataDirty = true;
                            ChangedDataNum.Add(0);
                        }

                        if (((Int32Field)camField.fields[1].Item1).value != (int)CamPoints[i].Type)
                        {
                            ((Int32Field)camField.fields[1].Item1).value = (int)CamPoints[i].Type;// SegmentType
                            isDataDirty = true;
                            ChangedDataNum.Add(1);
                        }

                        if (Math.Abs(((LRealField)camField.fields[2].Item1).value - (float)CamPoints[i].Master) > float.Epsilon)
                        {
                            ((LRealField)camField.fields[2].Item1).value = (float)CamPoints[i].Master; // X
                            isDataDirty = true;
                            ChangedDataNum.Add(2);
                        }

                        if (Math.Abs(((LRealField)camField.fields[3].Item1).value - (float)CamPoints[i].Slave) > float.Epsilon)
                        {
                            isDataDirty = true;
                            ((LRealField)camField.fields[3].Item1).value = (float)CamPoints[i].Slave; // Y
                            ChangedDataNum.Add(3);
                        }

                        if (Math.Abs(((LRealField)camField.fields[4].Item1).value - CamPoints[i].C0) > double.Epsilon)
                        {
                            ((LRealField)camField.fields[4].Item1).value = CamPoints[i].C0; // C0
                            isDataDirty = true;
                            ChangedDataNum.Add(4);
                        }

                        if (Math.Abs(((LRealField)camField.fields[5].Item1).value - CamPoints[i].C1) > double.Epsilon)
                        {
                            ((LRealField)camField.fields[5].Item1).value = CamPoints[i].C1; // C1
                            isDataDirty = true;
                            ChangedDataNum.Add(5);
                        }

                        if (Math.Abs(((LRealField)camField.fields[6].Item1).value - CamPoints[i].C2) > double.Epsilon)
                        {
                            ((LRealField)camField.fields[6].Item1).value = CamPoints[i].C2; // C2
                            isDataDirty = true;
                            ChangedDataNum.Add(6);
                        }

                        if (Math.Abs(((LRealField)camField.fields[7].Item1).value - CamPoints[i].C3) > double.Epsilon)
                        {
                            ((LRealField)camField.fields[7].Item1).value = CamPoints[i].C3; // C3
                            isDataDirty = true;
                            ChangedDataNum.Add(7);
                        }

                        if (isDataDirty)
                        {
                            NoticeCamProfileDataChanged(camField);

                            if (_controller.OperationMode == ControllerOperationMode.OperationModeProgram && _controller.IsOnline)
                            {
                                NoticeCamProfileDataChangedToPLC(camField, ChangedDataNum, i);
                            }
                        }
                    }
                }

                for (; i < arrayField.fields.Count; i++)
                {
                    CAM_PROFILEField camField = arrayField.fields[i].Item1 as CAM_PROFILEField;
                    if (camField != null)
                    {
                        ((Int32Field)camField.fields[0].Item1).value = 0;
                        ((Int32Field)camField.fields[1].Item1).value = 0;
                        ((LRealField)camField.fields[2].Item1).value = 0;
                        ((LRealField)camField.fields[3].Item1).value = 0;
                        ((LRealField)camField.fields[4].Item1).value = 0;
                        ((LRealField)camField.fields[5].Item1).value = 0;
                        ((LRealField)camField.fields[6].Item1).value = 0;
                        ((LRealField)camField.fields[7].Item1).value = 0;
                        if (isDataDirty)
                        {
                            NoticeCamProfileDataChanged(camField);

                            if (_controller.OperationMode == ControllerOperationMode.OperationModeProgram && _controller.IsOnline)
                            {
                                NoticeCamProfileDataChangedToPLC(camField, ChangedDataNum, i);
                            }
                        }
                    }
                }
            }
            else
            {
                bool isDataDirty = false;
                List<int> ChangedDataNum = new List<int>();

                int i;

                for (i = 0; i < CamPoints.Count; i++)
                {
                    CAMField camField = arrayField.fields[i].Item1 as CAMField;

                    if (camField != null)
                    {
                        if (Math.Abs(((RealField)camField.fields[0].Item1).value - (float)CamPoints[i].Master) > float.Epsilon)
                        {
                            ((RealField)camField.fields[0].Item1).value = (float)CamPoints[i].Master;
                            isDataDirty = true;
                            ChangedDataNum.Add(0);
                        }

                        if (Math.Abs(((RealField)camField.fields[1].Item1).value - (float)CamPoints[i].Slave) > float.Epsilon)
                        {
                            ((RealField)camField.fields[1].Item1).value = (float)CamPoints[i].Slave;
                            isDataDirty = true;
                            ChangedDataNum.Add(1);
                        }

                        if (((Int32Field)camField.fields[2].Item1).value != (int)CamPoints[i].Type)
                        {
                            ((Int32Field)camField.fields[2].Item1).value = (int)CamPoints[i].Type;
                            isDataDirty = true;
                            ChangedDataNum.Add(2);
                        }

                        if (isDataDirty)
                        {
                            NoticeCamDataChanged(camField);
                            if (_controller.OperationMode == ControllerOperationMode.OperationModeProgram && _controller.IsOnline)
                            {
                                NoticeCamDataChangedToPLC(camField, ChangedDataNum, i);
                            }
                        }
                    }
                }

                for (; i < arrayField.fields.Count; i++)
                {
                    CAMField camField = arrayField.fields[i].Item1 as CAMField;
                    if (camField != null)
                    {
                        ((RealField)camField.fields[0].Item1).value = 0;
                        ((RealField)camField.fields[1].Item1).value = 0;
                        ((Int32Field)camField.fields[2].Item1).value = 0;
                        if (isDataDirty)
                        {
                            NoticeCamDataChanged(camField);
                            if (_controller.OperationMode == ControllerOperationMode.OperationModeProgram && _controller.IsOnline)
                            {
                                NoticeCamDataChangedToPLC(camField, ChangedDataNum, i);
                            }
                        }
                    }
                }
            }

            var a = _tag;
        }

        private void NoticeCamProfileDataChangedToPLC(CAM_PROFILEField camProfileField, List<int> NumDataList,int num)
        {
            for (var i = 0; i < NumDataList.Count; i++)
            {
                int index = NumDataList[i];
                ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
                {
                    var lastName = "";
                    switch (index)
                    {
                        case 0:
                            lastName = "Status";
                            break;
                        case 1:
                            lastName = "SegmentType";
                            break;
                        case 2:
                            lastName = "Master";
                            break;
                        case 3:
                            lastName = "Slave";
                            break;
                    }

                    var name = _tag.Name + "[" + num + "]." + lastName;

                    await TaskScheduler.Default;

                    await _controller.SetTagValueToPLC(_tag, name, camProfileField.fields[index].Item1.ToString());
                });
            }
        }

        private void NoticeCamDataChangedToPLC(CAMField camField, List<int> NumDataList, int num)
        {
            for (var i = 0; i < NumDataList.Count; i++)
            {
                int index = NumDataList[i];
                ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
                {
                    var lastName = "";
                    switch (index)
                    {
                        case 0:
                            lastName = "Master";
                            break;
                        case 1:
                            lastName = "Slave";
                            break;
                        case 2:
                            lastName = "SegmentType";
                            break;
                    }

                    var name = _tag.Name + "[" + num + "]." + lastName;

                    await TaskScheduler.Default;

                    await _controller.SetTagValueToPLC(_tag, name, camField.fields[index].Item1.ToString());
                });
            }
        }

        private void NoticeCamDataChanged(CAMField camField)
        {
            for (int i = 0; i < camField.fields.Count; i++)
            {
                Console.WriteLine(i);
                Notifications.SendNotificationData(new TagNotificationData()
                {
                    Tag = _tag,
                    Type = TagNotificationData.NotificationType.Value,
                    Field = camField.fields[i].Item1
                });
            }
        }

        private void NoticeCamProfileDataChanged(CAM_PROFILEField camProfileField)
        {
            for (int i = 0; i < camProfileField.fields.Count; i++)
            {
                Notifications.SendNotificationData(new TagNotificationData()
                {
                    Tag = _tag,
                    Type = TagNotificationData.NotificationType.Value,
                    Field = camProfileField.fields[i].Item1
                });
            }
        }

        private int DoApply()
        {
            int result = CheckData();
            if (result == 0)
            {
                CopyData();
            }

            return result;
        }

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        private bool IsDirty()
        {
            // slope
            if (_startSlope != _oldStartSlope)
                return true;

            if (_endSlope != _oldEndSlope)
                return true;

            // data
            ArrayField arrayField = _arrayField;

            if (arrayField != null && CamPoints.Count == GetOriginalDataNum(arrayField))
            {
                if (_isCamProfile)
                {
                    for (int i = 0; i < CamPoints.Count; i++)
                    {
                        CAM_PROFILEField camField = arrayField.fields[i].Item1 as CAM_PROFILEField;
                        if (camField != null)
                        {
                            if ((float)((LRealField)camField.fields[2].Item1).value !=
                                CamPoints[i].Master ||
                                (float)((LRealField)camField.fields[3].Item1).value !=
                                CamPoints[i].Slave ||
                                ((Int32Field)camField.fields[1].Item1).value != (int)CamPoints[i].Type)
                            {
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < CamPoints.Count; i++)
                    {
                        CAMField camField = arrayField.fields[i].Item1 as CAMField;
                        if (camField != null)
                        {
                            if ((float)((RealField)camField.fields[0].Item1).value !=
                                CamPoints[i].Master ||
                                (float)((RealField)camField.fields[1].Item1).value !=
                                CamPoints[i].Slave ||
                                ((Int32Field)camField.fields[2].Item1).value != (int)CamPoints[i].Type)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            return true;
        }

        [SuppressMessage("ReSharper", "RedundantIfElseBlock")]
        private int GetOriginalDataNum(ArrayField arrayField)
        {
            int num = 0;

            if (_isCamProfile)
            {
                for (int i = arrayField.fields.Count - 1; i >= 0; i--)
                {
                    CAM_PROFILEField camField = arrayField.fields[i].Item1 as CAM_PROFILEField;

                    //从后往前第一个Status为2的点之后的数据就不显示了；
                    if (camField != null && ((Int32Field)camField.fields[0].Item1).value == 2)
                    {
                        //返回的是有多少个可以显示的数据 所以需要在索引基础上加1
                        num = i + 1;
                        break;
                    }
                }
            }
            else
            {
                num++;

                for (int i = 1; i < arrayField.fields.Count; i++)
                {
                    CAMField camField0 = arrayField.fields[i - 1].Item1 as CAMField;

                    CAMField camField1 = arrayField.fields[i].Item1 as CAMField;

                    //Cam类型的Tag，打开时如果存在后一个小于前一个Tag的master的情况，则错误数据之后的点不显示；
                    if (camField0 != null
                        && camField1 != null
                        && ((RealField)camField1.fields[0].Item1).value <=
                        ((RealField)camField0.fields[0].Item1).value)
                    {
                        break;
                    }

                    num++;
                }
            }

            return num;
        }

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        private void LoadCamPointsFromTag(ObservableCollection<CamPoint> camPoints)
        {
            ArrayField arrayField = _arrayField;
            Contract.Assert(arrayField != null);

            int validCount = GetOriginalDataNum(arrayField);

            if (_isCamProfile)
            {
                for (int i = 0; i < validCount; i++)
                {
                    CAM_PROFILEField camProfileField = arrayField.fields[i].Item1 as CAM_PROFILEField;
                    if (camProfileField != null)
                    {
                        int status = ((Int32Field)camProfileField.fields[0].Item1).value;
                        SegmentType segmentType = (SegmentType)((Int32Field)camProfileField.fields[1].Item1).value;
                        double master = ((LRealField)camProfileField.fields[2].Item1).value;
                        double slave = ((LRealField)camProfileField.fields[3].Item1).value;
                        double c0 = ((LRealField)camProfileField.fields[4].Item1).value;
                        double c1 = ((LRealField)camProfileField.fields[5].Item1).value;
                        double c2 = ((LRealField)camProfileField.fields[6].Item1).value;
                        double c3 = ((LRealField)camProfileField.fields[7].Item1).value;


                        camPoints.Add(new CamPoint
                        {
                            Status = status,
                            Type = segmentType,
                            Master = (float)master,
                            Slave = (float)slave,
                            C0 = c0,
                            C1 = c1,
                            C2 = c2,
                            C3 = c3
                        });
                    }
                }
            }
            else
            {
                for (int i = 0; i < validCount; i++)
                {
                    CAMField camField = arrayField.fields[i].Item1 as CAMField;
                    if (camField != null)
                    {
                        float master = ((RealField)camField.fields[0].Item1).value;
                        float slave = ((RealField)camField.fields[1].Item1).value;
                        SegmentType segmentType = (SegmentType)((Int32Field)camField.fields[2].Item1).value;
                        camPoints.Add(new CamPoint
                        {
                            Master = master,
                            Slave = slave,
                            Type = segmentType
                        });
                    }
                }
            }

            if (validCount >= 2)
            {
                if (camPoints[0].Type == SegmentType.Cubic)
                {
                    _startSlope = (float)camPoints[0].C1;
                }

                if (camPoints[camPoints.Count - 2].Type == SegmentType.Cubic)
                {
                    _endSlope = (float)camPoints[camPoints.Count - 1].C1;
                }
            }

            _oldStartSlope = _startSlope;
            _oldEndSlope = _endSlope;
        }

        private void PointsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object item in e.NewItems)
                    {
                        CamPoint camPoint = item as CamPoint;
                        if (camPoint != null)
                        {
                            PropertyChangedEventManager.AddHandler(camPoint, OnCamPointPropertyChanged,
                                "Master");
                            PropertyChangedEventManager.AddHandler(camPoint, OnCamPointPropertyChanged,
                                "Slave");
                            PropertyChangedEventManager.AddHandler(camPoint, OnCamPointPropertyChanged,
                                "Type");
                        }
                    }

                    CheckCamPoints();

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (object item in e.OldItems)
                    {
                        CamPoint camPoint = item as CamPoint;
                        if (camPoint != null)
                        {
                            PropertyChangedEventManager.RemoveHandler(camPoint, OnCamPointPropertyChanged,
                                "Master");
                            PropertyChangedEventManager.RemoveHandler(camPoint, OnCamPointPropertyChanged,
                                "Slave");
                            PropertyChangedEventManager.RemoveHandler(camPoint, OnCamPointPropertyChanged,
                                "Type");
                        }
                    }

                    CheckCamPoints();

                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            UpdatePlotModel();
            RaisePropertyChanged("MaxRowsNum");
            RaisePropertyChanged("MinRowsNum");
            ApplyCommand.RaiseCanExecuteChanged();
        }

        private void OnCamPointPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CamPointPropertyChangedEventArgs args = e as CamPointPropertyChangedEventArgs;
            if (args != null)
            {
                int index = CamPoints.IndexOf(sender as CamPoint);

                var editCommand = new EditCommand(this, index, args.OldPoint, args.NewPoint);
                CommandManager.AddCommand(editCommand);
                UpdateUndoRedoCanExecute();
            }

            if (!_initializing && e.PropertyName == "Master")
            {
                CheckCamPoints();
            }

            ApplyCommand.RaiseCanExecuteChanged();
            UpdatePlotModel();
        }

        private void CheckCamPoints()
        {
            if (CamPoints != null && CamPoints.Count > 1)
            {
                double[] masters = new double[CamPoints.Count];
                for (int i = 0; i < CamPoints.Count; i++)
                {
                    if (!double.TryParse(CamPoints[i].Master.ToString(), out masters[i]))
                    {
                        masters[i] = double.NaN;
                    }
                }

                CamPoints[0].IsBadCamPoint = double.IsNaN(masters[0]);

                for (int n = 1; n < CamPoints.Count; n++)
                {
                    if (double.IsNaN(masters[n]))
                    {
                        CamPoints[n].IsBadCamPoint = true;
                    }
                    else
                    {
                        CamPoints[n].IsBadCamPoint = false;
                        for (int i = 0; i < n; i++)
                        {
                            if (double.IsNaN(masters[i]))
                                continue;

                            if (masters[i] >= masters[n])
                            {
                                CamPoints[n].IsBadCamPoint = true;
                                break;
                            }
                        }
                    }
                }

            }
        }

        public PlotModel PlotModel
        {
            get { return _plotModel; }
            set { Set(ref _plotModel, value); }
        }

        public void UpdateColor()
        {
            ColorPosition = new SolidColorBrush(Color.FromArgb(_camOptions.Position.Color.A,
                _camOptions.Position.Color.R, _camOptions.Position.Color.G, _camOptions.Position.Color.B));
            ColorVelocity = new SolidColorBrush(Color.FromArgb(_camOptions.Velocity.Color.A,
                _camOptions.Velocity.Color.R, _camOptions.Velocity.Color.G, _camOptions.Velocity.Color.B));
            ColorAcceleration = new SolidColorBrush(Color.FromArgb(_camOptions.Acceleration.Color.A,
                _camOptions.Acceleration.Color.R, _camOptions.Acceleration.Color.G, _camOptions.Acceleration.Color.B));
            ColorJerk = new SolidColorBrush(Color.FromArgb(_camOptions.Jerk.Color.A, _camOptions.Jerk.Color.R,
                _camOptions.Jerk.Color.G, _camOptions.Jerk.Color.B));
            RaisePropertyChanged("ColorPosition");
            RaisePropertyChanged("ColorVelocity");
            RaisePropertyChanged("ColorAcceleration");
            RaisePropertyChanged("ColorJerk");
        }

        public void UpdatePlotModel()
        {
            if (!_initializing)
            {
                _plotModel.Series.Clear();
                _plotModel.Annotations.Clear();
                RebuildSeriesAndPoints();
                _plotModel.InvalidatePlot(true);
            }
        }

        private void CreateAllAxis()
        {
            _plotModel.Axes.Add(_positionAxis = CreateOneAxis(PositionKey, true));
            _plotModel.Axes.Add(_velocityAxis = CreateOneAxis(VelocityKey, false));
            _plotModel.Axes.Add(_accelerationAxis = CreateOneAxis(AccelerationKey, false));
            _plotModel.Axes.Add(_jerkAxis = CreateOneAxis(JerkKey, false));
            _plotModel.Axes.Add(_masterAxis = CreateOneAxis(MasterKey, true));
        }

        public LinearAxis CreateOneAxis(string key, bool isAxisVisible)
        {
            LinearAxis axis = new LinearAxis
            {
                TitleColor = OxyColors.Black,
                Position = AxisPosition.Right,
                Key = key,
                IsAxisVisible = isAxisVisible,
                PositionAtZeroCrossing = true,
                AxislineStyle = LineStyle.Solid,
                MinorTickSize = 0,
                MajorTickSize = 4,
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColor.FromArgb(180, 206, 255, 204),
                MajorGridlineThickness = 1,
                TickStyle = TickStyle.Crossing,
                TitleFontSize = 11,
                FontSize = 11,
                //可能是与屏幕的像素点关联的，应该是对应60个像素点，可以自己算一下放大系数IntervalLength
                MajorStep = 0.2,
                AxislineThickness = 1.8,
                IsPanEnabled = true
            };

            if (axis.Key != MasterKey)
            {
                axis.TitlePosition = 0.1;
                axis.Title = _yAxisTitle;
                axis.Maximum = 0.65;
                //TODO(TLM):Y轴竖向拉伸时，最大值不变，最小值会随着窗口的大小变化，规则待定；
                axis.Minimum = -0.6;
            }
            else
            {
                axis.TitlePosition = 0.05;
                axis.Position = AxisPosition.Bottom;
                axis.Title = _xAxisTitle;
                axis.Maximum = 1.5;
                //TODO(TLM):X轴横向拉伸时，最小值不变，最大值会随窗口变化，规则待定；
                axis.Minimum = -0.05;
            }

            return axis;
        }


        //TODO(TLM):构建曲线和点的时候 需要增加一条限制 构建时遇到错误的点就跳过
        public void RebuildSeriesAndPoints()
        {
            //add some points
            List<CamProfile> profiles = new List<CamProfile>();
            foreach (CamPoint t in CamPoints)
            {
                profiles.Add(new CamProfile
                {
                    X = t.Master,
                    Y = t.Slave,
                    Type = t.Type
                });
            }

            //add Slope
            CamProfilesCompute.Compute(profiles, _startSlope, _endSlope);


            //Return profiles
            for (int i = 0; i < profiles.Count; i++)
            {
                CamPoints[i].Status = (int)profiles[i].Status;
                CamPoints[i].Master = (float)profiles[i].X;
                CamPoints[i].Slave = (float)profiles[i].Y;
                CamPoints[i].Type = profiles[i].Type;
                CamPoints[i].C0 = profiles[i].C0;
                CamPoints[i].C1 = profiles[i].C1;
                CamPoints[i].C2 = profiles[i].C2;
                CamPoints[i].C3 = profiles[i].C3;
            }

            //drawing equations
            for (int i = 0; i < CamPoints.Count - 1; i++)
            {
                double x0 = CamPoints[i].Master;
                double y0 = CamPoints[i].Slave;
                double c0 = CamPoints[i].C0;
                double c1 = CamPoints[i].C1;
                double c2 = CamPoints[i].C2;
                double c3 = CamPoints[i].C3;

                //get the dx
                var deltaX = (CamPoints[i + 1].Master - CamPoints[i].Master) / 1000;
                if (Math.Abs(deltaX) < float.Epsilon)
                    deltaX = 0.001f;

                var positionFunction = new FunctionSeries(x =>
                {
                    double dx = x - x0;
                    return y0 + c0 + c1 * dx + c2 * dx * dx + c3 * dx * dx * dx;
                }, CamPoints[i].Master, CamPoints[i + 1].Master, deltaX)
                {
                    Color = _camOptions.Position.Color,
                    YAxisKey = PositionKey,
                    IsVisible = _camOptions.Position.Visible,
                    StrokeThickness = _camOptions.Position.Width,
                    LineStyle = _camOptions.Position.Style
                };

                var velocityFunction = new FunctionSeries(x =>
                {
                    double dx = x - x0;
                    return (c1 + 2 * c2 * dx + 3 * c3 * dx * dx) * _camOptions.MasterVelocity;
                }, CamPoints[i].Master, CamPoints[i + 1].Master, deltaX)
                {
                    Color = _camOptions.Velocity.Color,
                    LineStyle = _camOptions.Velocity.Style,
                    YAxisKey = VelocityKey,
                    IsVisible = _camOptions.Velocity.Visible,
                    StrokeThickness = _camOptions.Velocity.Width,
                };

                var accelerationFunction = new FunctionSeries(x =>
                {
                    double dx = x - x0;
                    return (2 * c2 + 6 * c3 * dx) * _camOptions.MasterVelocity * _camOptions.MasterVelocity;
                }, CamPoints[i].Master, CamPoints[i + 1].Master, deltaX)
                {
                    Color = _camOptions.Acceleration.Color,
                    LineStyle = _camOptions.Acceleration.Style,
                    YAxisKey = AccelerationKey,
                    IsVisible = _camOptions.Acceleration.Visible,
                    StrokeThickness = _camOptions.Acceleration.Width
                };

                var jerkFunction = new FunctionSeries(
                    x =>
                    {
                        return (6 * c3) * _camOptions.MasterVelocity * _camOptions.MasterVelocity *
                               _camOptions.MasterVelocity;
                    },
                    CamPoints[i].Master, CamPoints[i + 1].Master, deltaX)
                {
                    Color = _camOptions.Jerk.Color,
                    LineStyle = _camOptions.Jerk.Style,
                    YAxisKey = JerkKey,
                    IsVisible = _camOptions.Jerk.Visible,
                    StrokeThickness = _camOptions.Jerk.Width
                };


                double pdx = 0;
                var positionPoint = new PointAnnotation()
                {
                    X = CamPoints[i].Master,
                    Y = y0 + c0 + c1 * pdx + c2 * pdx * pdx + c3 * pdx * pdx * pdx,
                    Fill = OxyColors.White,
                    Stroke = _camOptions.Position.Color,
                    Shape = _camOptions.Position.Marker,
                    StrokeThickness = _camOptions.Position.Width,
                    Size = 3,
                    YAxisKey = PositionKey
                };

                var velocityPoint = new PointAnnotation()
                {
                    X = CamPoints[i].Master,
                    Y = (c1 + 2 * c2 * pdx + 3 * c3 * pdx * pdx) * _camOptions.MasterVelocity,
                    Fill = OxyColors.White,
                    Stroke = _camOptions.Velocity.Color,
                    Shape = _camOptions.Velocity.Marker,
                    StrokeThickness = _camOptions.Velocity.Width,
                    Size = 3,
                    YAxisKey = VelocityKey
                };

                var accelerationPoint = new PointAnnotation()
                {
                    X = CamPoints[i].Master,
                    Y = (2 * c2 + 6 * c3 * pdx) * _camOptions.MasterVelocity * _camOptions.MasterVelocity,
                    Fill = OxyColors.White,
                    Stroke = _camOptions.Acceleration.Color,
                    Shape = _camOptions.Acceleration.Marker,
                    StrokeThickness = _camOptions.Acceleration.Width,
                    Size = 3,
                    YAxisKey = AccelerationKey
                };

                var jerkPoint = new PointAnnotation()
                {
                    X = CamPoints[i].Master,
                    Y = (6 * c3) * _camOptions.MasterVelocity * _camOptions.MasterVelocity * _camOptions.MasterVelocity,
                    Fill = OxyColors.White,
                    Stroke = _camOptions.Jerk.Color,
                    Shape = _camOptions.Jerk.Marker,
                    StrokeThickness = _camOptions.Jerk.Width,
                    Size = 3,
                    YAxisKey = JerkKey
                };

                _plotModel.Series.Add(positionFunction);
                _plotModel.Series.Add(velocityFunction);
                _plotModel.Series.Add(accelerationFunction);
                _plotModel.Series.Add(jerkFunction);

                _plotModel.Annotations.Add(positionPoint);
                _plotModel.Annotations.Add(velocityPoint);
                _plotModel.Annotations.Add(accelerationPoint);
                _plotModel.Annotations.Add(jerkPoint);
            }

            if (CamPoints.Count > 0)
            {
                double y0 = CamPoints[CamPoints.Count - 1].Slave;
                double c0 = CamPoints[CamPoints.Count - 1].C0;
                double c1 = CamPoints[CamPoints.Count - 1].C1;
                double c2 = CamPoints[CamPoints.Count - 1].C2;
                double c3 = CamPoints[CamPoints.Count - 1].C3;

                var positionPoint = new PointAnnotation()
                {
                    X = CamPoints[CamPoints.Count - 1].Master,
                    Y = y0 + c0,
                    Fill = OxyColors.White,
                    Stroke = _camOptions.Position.Color,
                    Shape = _camOptions.Position.Marker,
                    StrokeThickness = _camOptions.Position.Width,
                    Size = 3,
                    YAxisKey = PositionKey
                };

                _plotModel.Annotations.Add(positionPoint);

                _plotModel.Annotations.Add(new PointAnnotation()
                {
                    X = CamPoints[CamPoints.Count - 1].Master,
                    Y = (c1) * _camOptions.MasterVelocity,
                    Fill = OxyColors.White,
                    Stroke = _camOptions.Velocity.Color,
                    Shape = _camOptions.Velocity.Marker,
                    StrokeThickness = _camOptions.Velocity.Width,
                    Size = 3,
                    YAxisKey = VelocityKey
                });

                _plotModel.Annotations.Add(new PointAnnotation()
                {
                    X = CamPoints[CamPoints.Count - 1].Master,
                    Y = (2 * c2) * _camOptions.MasterVelocity * _camOptions.MasterVelocity,
                    Fill = OxyColors.White,
                    Stroke = _camOptions.Acceleration.Color,
                    Shape = _camOptions.Acceleration.Marker,
                    StrokeThickness = _camOptions.Acceleration.Width,
                    Size = 3,
                    YAxisKey = AccelerationKey
                });

                _plotModel.Annotations.Add(new PointAnnotation()
                {
                    X = CamPoints[CamPoints.Count - 1].Master,
                    Y = (6 * c3) * _camOptions.MasterVelocity * _camOptions.MasterVelocity * _camOptions.MasterVelocity,
                    Fill = OxyColors.White,
                    Stroke = _camOptions.Jerk.Color,
                    Shape = _camOptions.Jerk.Marker,
                    StrokeThickness = _camOptions.Jerk.Width,
                    Size = 3,
                    YAxisKey = JerkKey
                });
            }
        }

        public static IViewCommand<OxyMouseDownEventArgs> Pan { get; set; }

        private bool _canPointToMove = false;
        private int _indexOfCamPointsNum = -1;

        private void PlotModelOnMouseDown(object sender, OxyMouseDownEventArgs e)
        {
            if (!(_controller.OperationMode == ControllerOperationMode.OperationModeRun && _controller.IsOnline))
            {
                if (e.ChangedButton == OxyMouseButton.Left)
                {
                    for (int j = 0; j < CamPoints.Count; j++)
                    {
                        var nearestPoint = new ScreenPoint(_masterAxis.Transform(CamPoints[j].Master),
                            _positionAxis.Transform(CamPoints[j].Slave));
                        if ((nearestPoint - e.Position).Length < 10)
                        {
                            SelectedItem = CamPoints[j];
                            _canPointToMove = true;
                            _indexOfCamPointsNum = j;
                            _plotModel.InvalidatePlot(false);
                            e.Handled = true;
                            break;
                        }
                    }
                }

                if (_canPointCreate)
                {
                    if (MaxRowsNum)
                    {
                        int index = CamPoints.Count;
                        if (index > 0)
                        {
                            if (Master < CamPoints[0].Master)
                            {
                                index = 0;
                            }

                            for (int i = 0; i < CamPoints.Count - 1; i++)
                            {
                                if (Master > CamPoints[i].Master &&
                                    Master < CamPoints[i + 1].Master)
                                {
                                    index = i + 1;
                                    break;
                                }
                            }

                            if (Master > CamPoints[CamPoints.Count - 1].Master)
                            {
                                index = CamPoints.Count;
                            }
                        }

                        var point = new CamPoint
                        {
                            Master = (float)Master,
                            Slave = (float)Position,
                            Type = SegmentType.Linear
                        };

                        CamPoints.Insert(index, point);

                        UpdateCamPoints();

                        var addCommand = new AddCommand(this, index, point);
                        CommandManager.AddCommand(addCommand);
                        UpdateUndoRedoCanExecute();

                        _canPointCreate = false;
                    }
                    else
                    {
                        _tips = "Too many segments. Cam array is full.";
                        _tipsTitle = "Cam Editor";

                        MessageBox.Show(_tips, _tipsTitle,
                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }

                //移动整体视图
                if (e.ChangedButton == OxyMouseButton.Right && SelectMode == SelectMode.All)
                {
                    Pan = new DelegatePlotCommand<OxyMouseDownEventArgs>((view, controller, args) =>
                        controller.AddMouseManipulator(view, new PanClass(view, SelectMode), args));
                    (sender as PlotController).BindMouseDown(OxyMouseButton.Right, Pan);
                }
                else
                {
                    Pan = null;
                }
            }
        }

        public void UpdateAxis()
        {
            if (SelectMode != SelectMode.All)
            {
                _positionAxis.IsAxisVisible = SelectMode == SelectMode.Position;
                _velocityAxis.IsAxisVisible = SelectMode == SelectMode.Velocity;
                _accelerationAxis.IsAxisVisible = SelectMode == SelectMode.Acceleration;
                _jerkAxis.IsAxisVisible = SelectMode == SelectMode.Jerk;
            }

            _positionAxis.IsPanEnabled = _positionAxis.IsAxisVisible || SelectMode == SelectMode.All;
            _velocityAxis.IsPanEnabled = _velocityAxis.IsAxisVisible || SelectMode == SelectMode.All;
            _accelerationAxis.IsPanEnabled = _accelerationAxis.IsAxisVisible || SelectMode == SelectMode.All;
            _jerkAxis.IsPanEnabled = _jerkAxis.IsAxisVisible || SelectMode == SelectMode.All;

            switch (SelectMode)
            {
                case SelectMode.Position:
                    _positionAxis.Title = LanguageManager.GetInstance().ConvertSpecifier("Slave Position");
                    _positionAxis.TitleColor = OxyColor.FromRgb(_camOptions.Position.Color.R,
                        _camOptions.Position.Color.G, _camOptions.Position.Color.B);
                    _positionAxis.AxislineColor = OxyColor.FromRgb(_camOptions.Position.Color.R,
                        _camOptions.Position.Color.G, _camOptions.Position.Color.B);
                    _plotModel.DefaultXAxis.AxislineColor = OxyColor.FromRgb(_camOptions.Position.Color.R,
                        _camOptions.Position.Color.G, _camOptions.Position.Color.B);
                    break;
                case SelectMode.Velocity:
                    _velocityAxis.Title = LanguageManager.GetInstance().ConvertSpecifier("Slave Velocity");
                    _velocityAxis.TitleColor = OxyColor.FromRgb(_camOptions.Velocity.Color.R,
                        _camOptions.Velocity.Color.G, _camOptions.Velocity.Color.B);
                    _velocityAxis.AxislineColor = OxyColor.FromRgb(_camOptions.Velocity.Color.R,
                        _camOptions.Velocity.Color.G, _camOptions.Velocity.Color.B);
                    _plotModel.DefaultXAxis.AxislineColor = OxyColor.FromRgb(_camOptions.Velocity.Color.R,
                        _camOptions.Velocity.Color.G, _camOptions.Velocity.Color.B);
                    break;
                case SelectMode.Acceleration:
                    _accelerationAxis.Title = LanguageManager.GetInstance().ConvertSpecifier("Slave Acceleration");
                    _accelerationAxis.TitleColor = OxyColor.FromRgb(_camOptions.Acceleration.Color.R,
                        _camOptions.Acceleration.Color.G, _camOptions.Acceleration.Color.B);
                    _accelerationAxis.AxislineColor = OxyColor.FromRgb(_camOptions.Acceleration.Color.R,
                        _camOptions.Acceleration.Color.G, _camOptions.Acceleration.Color.B);
                    _plotModel.DefaultXAxis.AxislineColor = OxyColor.FromRgb(_camOptions.Acceleration.Color.R,
                        _camOptions.Acceleration.Color.G, _camOptions.Acceleration.Color.B);
                    break;
                case SelectMode.Jerk:
                    _jerkAxis.Title = LanguageManager.GetInstance().ConvertSpecifier("Slave Jerk");
                    _jerkAxis.TitleColor = OxyColor.FromRgb(_camOptions.Jerk.Color.R, _camOptions.Jerk.Color.G,
                        _camOptions.Jerk.Color.B);
                    _jerkAxis.AxislineColor = OxyColor.FromRgb(_camOptions.Jerk.Color.R, _camOptions.Jerk.Color.G,
                        _camOptions.Jerk.Color.B);
                    _plotModel.DefaultXAxis.AxislineColor = OxyColor.FromRgb(_camOptions.Jerk.Color.R,
                        _camOptions.Jerk.Color.G, _camOptions.Jerk.Color.B);
                    break;
                case SelectMode.All:
                    _plotModel.DefaultYAxis.Title = LanguageManager.GetInstance().ConvertSpecifier("Slave Position");
                    _plotModel.DefaultYAxis.TitleColor = OxyColors.Black;
                    _plotModel.DefaultYAxis.AxislineColor = OxyColors.Black;
                    _plotModel.DefaultXAxis.AxislineColor = OxyColors.Black;
                    break;
            }
        }
    }

    public class PanClass : MouseManipulator
    {
        private SelectMode _model;

        public PanClass(IPlotView plotView, SelectMode model) : base(plotView)
        {
            _model = model;
        }

        private ScreenPoint PreviousPosition { get; set; }

        private bool IsPanEnabled { get; set; }

        public override void Completed(OxyMouseEventArgs e)
        {
            base.Completed(e);
            if (!IsPanEnabled)
            {
                return;
            }

            View.SetCursorType(CursorType.Default);
            e.Handled = true;
        }

        public override void Delta(OxyMouseEventArgs e)
        {
            if (!IsPanEnabled)
            {
                return;
            }

            if (_model == SelectMode.All)
            {
                foreach (var t in PlotView.ActualModel.Axes)
                {
                    t.Pan(this.PreviousPosition, e.Position);
                }
            }
            else
            {
                PlotView.ActualModel.DefaultYAxis.Pan(this.PreviousPosition, e.Position);
                PlotView.ActualModel.DefaultXAxis.Pan(this.PreviousPosition, e.Position);
            }

            PlotView.InvalidatePlot();
            PreviousPosition = e.Position;
            e.Handled = true;
        }

        public override void Started(OxyMouseEventArgs e)
        {
            base.Started(e);
            PreviousPosition = e.Position;

            IsPanEnabled = true;

            if (IsPanEnabled)
            {
                View.SetCursorType(CursorType.Pan);
                e.Handled = true;
            }
        }
    }
}