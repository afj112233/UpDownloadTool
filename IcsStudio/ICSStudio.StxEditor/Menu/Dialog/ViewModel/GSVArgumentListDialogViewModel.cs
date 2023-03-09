using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Components.Controls;
using ICSStudio.Dialogs.Filter;
using ICSStudio.Gui.Utils;
using ICSStudio.Gui.View;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.Instruction;

namespace ICSStudio.StxEditor.Menu.Dialog.ViewModel
{
    internal class GSVArgumentListDialogViewModel<T> : ViewModelBase where T :struct
    {
        private T _selectedClass;
        private Visibility _instanceComboboxVisibility;
        private Visibility _tagFilterVisibility;
        private string _selectedAttribute;
        private ObservableCollection<string> _attributeList = new ObservableCollection<string>();
        private bool? _dialogResult;
        private bool _dirty;
        private string _selectedInstanceCollection;
        private IProgramModule _program;
        internal GSVArgumentListDialogViewModel(List<ParamInfo> parameters,IProgramModule program)
        {
            _program = program;
            ClassList = EnumHelper.ToDataSource<T>();
            SelectedClass = ParseEnum<T>(parameters.Count >= 1 ? parameters[0].Param : null);
            Title = SelectedClass is InstrEnum.ClassName ? "GSV Instruction - Argument List" : "SSV Instruction - Argument List";
            if (parameters.Count >= 3)
                SelectedAttribute = parameters[2].Param;
            NameFilterCommand = new RelayCommand<Button>(ExecuteNameFilterCommand);
            NameFilterCommand2 = new RelayCommand<Button>(ExecuteNameFilterCommand2);
            OKCommand=new RelayCommand(ExecuteOKCommand);
            CancelCommand=new RelayCommand(ExecuteCancelCommand);
            ApplyCommand=new RelayCommand(ExecuteApplyCommand,CanExecuteApplyCommand);
            var preFilterDataTypes = GetPreFilterDataTypes(SelectedClass.ToString());
            NameFilterPopup = new NameFilterPopup(program,preFilterDataTypes,false);
            //NameFilterPopup.SetPreFilterDataTypes(preFilterDataTypes);
            if (parameters.Count >= 2)
            {
                SelectedInstanceCollection = parameters[1].Param;
                InstanceName = parameters[1].Param;
            }

            PropertyChangedEventManager.AddHandler(NameFilterPopup.FilterViewModel, FilterViewModel_PropertyChanged,
                string.Empty);
            //NameFilterPopup.FilterViewModel.PropertyChanged += FilterViewModel_PropertyChanged;

            NameFilterPopup2 = new NameFilterPopup(program,String.Empty, false);
            if (parameters.Count == 4)
            {
                DestinationName = parameters[3].Param;
            }
            PropertyChangedEventManager.AddHandler(NameFilterPopup2.FilterViewModel, FilterViewModel_PropertyChanged2,
                string.Empty);
            //NameFilterPopup2.FilterViewModel.PropertyChanged += FilterViewModel_PropertyChanged2;
            Dirty = false;
        }

        public bool? DialogResult
        {
            set { Set(ref _dialogResult , value); }
            get { return _dialogResult; }
        }

        public override void Cleanup()
        {
            PropertyChangedEventManager.RemoveHandler(NameFilterPopup.FilterViewModel, FilterViewModel_PropertyChanged,string.Empty);
            PropertyChangedEventManager.RemoveHandler(NameFilterPopup2.FilterViewModel, FilterViewModel_PropertyChanged2, string.Empty);
        }

        public ObservableCollection<string> InstanceCollection { get; } = new ObservableCollection<string>();

        public string SelectedInstanceCollection
        {
            set
            {
                if(string.IsNullOrEmpty(value)) return;
                _selectedInstanceCollection =
                    InstanceCollection.FirstOrDefault(item => item.Equals(value, StringComparison.OrdinalIgnoreCase)) ?? value;
                RaisePropertyChanged();
                Dirty = true;
            }
            get { return _selectedInstanceCollection; }
        }

        public bool IsUpdated { set; get; }
        
#pragma warning disable CS0693 // 类型参数与外部类型中的类型参数同名
        private T ParseEnum<T>(string parameter) where T : struct
#pragma warning restore CS0693 // 类型参数与外部类型中的类型参数同名
        {
            T className;
            var result = Enum.TryParse(parameter, true, out className);
            if (result) return className;
            return default(T);
        }

        private string GetPreFilterDataTypes(string className)
        {
            if (className.Equals("Axis"))
            {
                return
                    $"AXIS_CIP_DRIVE,AXIS_CONSUME,AXIS_GENERIC,AXIS_GENERIC_DRIVE,AXIS_SERVO,AXIS_SERVO_DRIVE,AXIS_VIRTUAL";
            }

            if (className.Equals("CoordinateSystem"))
            {
                return "Coordinate_System";
            }

            if (className.Equals("Message"))
            {
                return "Message";
            }

            if (className.Equals("MotionGroup"))
            {
                return "Motion_group";
            }

            if(className.Equals(""))
            Debug.Assert(false,className);
            return String.Empty;
        }

        private void FilterViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                Dirty = true;
                RaisePropertyChanged("InstanceName");
            }

            if (e.PropertyName == "Hide")
            {
                //var layer = AdornerLayer.GetAdornerLayer(Control);
                //layer?.Remove(NameFilterAdorner);
                NameFilterPopup.IsOpen = false;
            }
        }

        private void FilterViewModel_PropertyChanged2(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                Dirty = true;
                RaisePropertyChanged("DestinationName");
            }

            if (e.PropertyName == "Hide")
            {
                //var layer = AdornerLayer.GetAdornerLayer(Control);
                //layer?.Remove(NameFilterAdorner);
                NameFilterPopup2.IsOpen = false;
            }
        }

        public string InstanceName
        {
            set { NameFilterPopup.FilterViewModel.Name = value; }
            get { return NameFilterPopup.FilterViewModel.Name; }
        }

        public string DestinationName
        {
            set { NameFilterPopup2.FilterViewModel.Name = value; }
            get { return NameFilterPopup2.FilterViewModel.Name; }
        }

        public Dictionary<ITag, TagNameNode> InstanceNameList => NameFilterPopup.FilterViewModel.AutoCompleteData;
        public Dictionary<ITag, TagNameNode> DestinationNameList => NameFilterPopup2.FilterViewModel.AutoCompleteData;

        public NameFilterPopup NameFilterPopup { get; }

        public NameFilterPopup NameFilterPopup2 { get; }

        public RelayCommand<Button> NameFilterCommand { set; get; }

        private void ExecuteNameFilterCommand(Button sender)
        {
            var parentGrid = VisualTreeHelpers.FindVisualParentOfType<Grid>(sender);
            var autoCompleteBox = VisualTreeHelpers.FindFirstVisualChildOfType<AutoCompleteBox>(parentGrid);
            if (!NameFilterPopup.IsOpen)
            {
                NameFilterPopup.ResetAll(_program.Name, autoCompleteBox, true);
                NameFilterPopup2.FilterViewModel.NeedRaisePropertyChanged = false;
                NameFilterPopup.FilterViewModel.Name = autoCompleteBox.Text;
                NameFilterPopup2.FilterViewModel.NeedRaisePropertyChanged = true;
            }
            NameFilterPopup.IsOpen = !NameFilterPopup.IsOpen;
        }

        public RelayCommand<Button> NameFilterCommand2 { set; get; }

        private void ExecuteNameFilterCommand2(Button sender)
        {
            var parentGrid = VisualTreeHelpers.FindVisualParentOfType<Grid>(sender);
            var autoCompleteBox = VisualTreeHelpers.FindFirstVisualChildOfType<AutoCompleteBox>(parentGrid);

            if (!NameFilterPopup2.IsOpen)
            {
                NameFilterPopup2.ResetAll(_program.Name, autoCompleteBox, true);
                NameFilterPopup2.FilterViewModel.NeedRaisePropertyChanged = false;
                NameFilterPopup2.FilterViewModel.Name = autoCompleteBox.Text;
                NameFilterPopup2.FilterViewModel.NeedRaisePropertyChanged = true;
            }
            NameFilterPopup2.IsOpen = !NameFilterPopup2.IsOpen;
        }


        public RelayCommand OKCommand { get; }

        private void ExecuteOKCommand()
        {
            DialogResult = true;
        }

        public RelayCommand CancelCommand { get; }

        private void ExecuteCancelCommand()
        {
            DialogResult = false;
        }

        public RelayCommand ApplyCommand { get; }

        private void ExecuteApplyCommand()
        {
            Dirty = false;
            RaisePropertyChanged("IsUpdated");
        }

        public bool Dirty
        {
            set
            {
                _dirty = value; 
                ApplyCommand?.RaiseCanExecuteChanged();
            }
            get { return _dirty; }
        }

        private bool CanExecuteApplyCommand()
        {
            return Dirty;
        }
        
        public IList ClassList { get; }

        public T SelectedClass
        {
            set
            {
                _selectedClass = value;
                Dirty = true;
                Choose();
                NameFilterPopup?.SetPreFilterDataTypes(GetPreFilterDataTypes(value.ToString()));
            }
            get { return _selectedClass; }
        }

        public Visibility InstanceComboboxVisibility
        {
            set
            {
                Set(ref _instanceComboboxVisibility, value);
                if (_instanceComboboxVisibility == Visibility.Visible)
                {
                    TagFilterVisibility = Visibility.Collapsed;
                }
            }
            get { return _instanceComboboxVisibility; }
        }

        public Visibility TagFilterVisibility
        {
            set
            {
                Set(ref _tagFilterVisibility, value);
                if (_tagFilterVisibility == Visibility.Visible)
                {
                    InstanceComboboxVisibility = Visibility.Collapsed;
                }
            }
            get { return _tagFilterVisibility; }
        }

        public ObservableCollection<string> AttributeList
        {
            set { Set(ref _attributeList, value); }
            get { return _attributeList; }
        }

        public string SelectedAttribute
        {
            set { Set(ref _selectedAttribute, value);
                Dirty = true;
            }
            get { return _selectedAttribute; }
        }

        public string Title { set; get; }
        
        private void Choose()
        {
            AttributeList.Clear();
            InstanceCollection.Clear();
            if (SelectedClass is InstrEnum.ClassName)
            {
                #region Class Name

                switch ((InstrEnum.ClassName)Enum.Parse(typeof(InstrEnum.ClassName), SelectedClass.ToString()))
                {
                    case InstrEnum.ClassName.AddOnInstructionDefinition:
                        InstanceComboboxVisibility = Visibility.Visible;
                        foreach (var aoi in Controller.GetInstance().AOIDefinitionCollection)
                        {
                            InstanceCollection.Add(aoi.Name);
                        }
                        
                        foreach (var @enum in SortArray(typeof(InstrEnum.AddOnInstructionDefinition).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.ClassName.DataLog:
                        InstanceComboboxVisibility = Visibility.Visible;
                        foreach (var @enum in SortArray(typeof(InstrEnum.DataLog).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.ClassName.Program:
                        InstanceComboboxVisibility = Visibility.Visible;
                        if (_program is Program)
                        {
                            foreach (var program in Controller.GetInstance().Programs)
                            {
                                InstanceCollection.Add(program.Name);
                            }
                        }

                        foreach (var @enum in SortArray(typeof(InstrEnum.Program).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.ClassName.Routine:
                        InstanceComboboxVisibility = Visibility.Visible;
                        if (_program is Program)
                        {
                            foreach (var routine in _program.Routines)
                            {
                                InstanceCollection.Add(routine.Name);
                            }
                        }
                        foreach (var @enum in SortArray(typeof(InstrEnum.Routine).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.ClassName.Task:
                        InstanceComboboxVisibility = Visibility.Visible;
                        if (_program is Program)
                            foreach (var task in Controller.GetInstance().Tasks)
                            {
                                InstanceCollection.Add(task.Name);
                            }
                        foreach (var @enum in SortArray(typeof(InstrEnum.Task).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.ClassName.Axis:
                        TagFilterVisibility = Visibility.Visible;
                        foreach (var @enum in SortArray(typeof(InstrEnum.Axis).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.ClassName.CoordinateSystem:
                        TagFilterVisibility = Visibility.Visible;
                        foreach (var @enum in SortArray(typeof(InstrEnum.CoordinateSystem).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.ClassName.Message:
                        TagFilterVisibility = Visibility.Visible;
                        foreach (var @enum in SortArray(typeof(InstrEnum.Message).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.ClassName.MotionGroup:
                        TagFilterVisibility = Visibility.Visible;
                        foreach (var @enum in SortArray(typeof(InstrEnum.MotionGroup).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.ClassName.Controller:
                        InstanceComboboxVisibility = Visibility.Collapsed;
                        TagFilterVisibility = Visibility.Collapsed;
                        foreach (var @enum in SortArray(typeof(InstrEnum.Controller).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.ClassName.ControllerDevice:
                        InstanceComboboxVisibility = Visibility.Collapsed;
                        TagFilterVisibility = Visibility.Collapsed;
                        foreach (var @enum in SortArray(typeof(InstrEnum.ControllerDevice).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.ClassName.CST:
                        InstanceComboboxVisibility = Visibility.Collapsed;
                        TagFilterVisibility = Visibility.Collapsed;
                        foreach (var @enum in SortArray(typeof(InstrEnum.CST).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.ClassName.DF1:
                        InstanceComboboxVisibility = Visibility.Collapsed;
                        TagFilterVisibility = Visibility.Collapsed;
                        foreach (var @enum in SortArray(typeof(InstrEnum.DF1).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.ClassName.FaultLog:
                        InstanceComboboxVisibility = Visibility.Collapsed;
                        TagFilterVisibility = Visibility.Collapsed;
                        foreach (var @enum in SortArray(typeof(InstrEnum.FaultLog).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.ClassName.SerialPort:
                        InstanceComboboxVisibility = Visibility.Collapsed;
                        TagFilterVisibility = Visibility.Collapsed;
                        foreach (var @enum in SortArray(typeof(InstrEnum.SerialPort).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.ClassName.TimeSynchronize:
                        InstanceComboboxVisibility = Visibility.Collapsed;
                        TagFilterVisibility = Visibility.Collapsed;
                        foreach (var @enum in SortArray(typeof(InstrEnum.TimeSynchronize).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.ClassName.WallClockTime:
                        InstanceComboboxVisibility = Visibility.Collapsed;
                        TagFilterVisibility = Visibility.Collapsed;
                        foreach (var @enum in SortArray(typeof(InstrEnum.WallClockTime).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.ClassName.Module:
                        TagFilterVisibility = Visibility.Collapsed;
                        if (_program is Program)
                        {
                            foreach (var deviceModule in Controller.GetInstance().DeviceModules)
                            {
                                if (deviceModule is LocalModule||deviceModule is DiscreteIO) continue;
                                InstanceCollection.Add(deviceModule.Name);
                            }
                        }
                        foreach (var @enum in SortArray(typeof(InstrEnum.ModuleAttr).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }
                        break;

                }

                #endregion
            }
            else
            {
                #region SSV Class Name

                switch ((InstrEnum.SSVClassName)Enum.Parse(typeof(InstrEnum.SSVClassName), SelectedClass.ToString()))
                {
                    case InstrEnum.SSVClassName.Program:
                        InstanceComboboxVisibility = Visibility.Visible;
                        if (_program is Program)
                        {
                            foreach (var program in Controller.GetInstance().Programs)
                            {
                                InstanceCollection.Add(program.Name);
                            }
                        }
                        foreach (var @enum in SortArray(typeof(InstrEnum.SSVProgram).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.SSVClassName.Routine:
                        InstanceComboboxVisibility = Visibility.Visible;
                        if (_program is Program)
                        {
                            foreach (var routine in _program.Routines)
                            {
                                InstanceCollection.Add(routine.Name);
                            }
                        }
                        foreach (var @enum in SortArray(typeof(InstrEnum.SSVRoutine).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.SSVClassName.Task:
                        InstanceComboboxVisibility = Visibility.Visible;
                        if (_program is Program)
                            foreach (var task in Controller.GetInstance().Tasks)
                            {
                                InstanceCollection.Add(task.Name);
                            }
                        foreach (var @enum in SortArray(typeof(InstrEnum.Task).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.SSVClassName.Axis:
                        TagFilterVisibility = Visibility.Visible;
                        foreach (var @enum in SortArray(typeof(InstrEnum.SSVAxis).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.SSVClassName.CoordinateSystem:
                        TagFilterVisibility = Visibility.Visible;
                        foreach (var @enum in SortArray(typeof(InstrEnum.SSVCoordinateSystem).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.SSVClassName.Message:
                        TagFilterVisibility = Visibility.Visible;
                        foreach (var @enum in SortArray(typeof(InstrEnum.Message).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.SSVClassName.MotionGroup:
                        TagFilterVisibility = Visibility.Visible;
                        foreach (var @enum in SortArray(typeof(InstrEnum.SSVMotionGroup).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.SSVClassName.Controller:
                        InstanceComboboxVisibility = Visibility.Collapsed;
                        TagFilterVisibility = Visibility.Collapsed;
                        foreach (var @enum in SortArray(typeof(InstrEnum.SSVController).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.SSVClassName.FaultLog:
                        InstanceComboboxVisibility = Visibility.Collapsed;
                        TagFilterVisibility = Visibility.Collapsed;
                        foreach (var @enum in SortArray(typeof(InstrEnum.FaultLog).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.SSVClassName.TimeSynchronize:
                        InstanceComboboxVisibility = Visibility.Collapsed;
                        TagFilterVisibility = Visibility.Collapsed;
                        foreach (var @enum in SortArray(typeof(InstrEnum.SSVTimeSynchronize).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.SSVClassName.WallClockTime:
                        InstanceComboboxVisibility = Visibility.Collapsed;
                        TagFilterVisibility = Visibility.Collapsed;
                        foreach (var @enum in SortArray(typeof(InstrEnum.WallClockTime).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }

                        break;
                    case InstrEnum.SSVClassName.Module:
                        TagFilterVisibility = Visibility.Collapsed;
                        if (_program is Program)
                        {
                            foreach (var deviceModule in Controller.GetInstance().DeviceModules)
                            {
                                if (deviceModule is LocalModule) continue;
                                InstanceCollection.Add(deviceModule.Name);
                            }
                        }
                        foreach (var @enum in SortArray(typeof(InstrEnum.SSVModuleAttr).GetEnumValues()))
                        {
                            AttributeList.Add(@enum.ToString());
                        }
                        break;

                }

                #endregion
            }
           
        }

        private Array SortArray(Array array)
        {
            Array.Sort(array, Comparer<IComparable>.Create((x, y) => x.ToString().CompareTo(y.ToString())));
            return array;
        }
    }
}
