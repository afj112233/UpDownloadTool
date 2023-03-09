using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using ICSStudio.Components.Converters;

namespace ICSStudio.Components.Controls
{
    /// <summary>
    /// DataTypeSelectorAutoCompleteControl.xaml 的交互逻辑
    /// </summary>
    [ContentProperty("DataTypeName")]
    public partial class DataTypeSelectorAutoCompleteControl : IEditableControl
    {
        public static readonly DependencyProperty DataTypeNameProperty =
            DependencyProperty.Register(nameof(DataTypeName), typeof(string),
                typeof(DataTypeSelectorAutoCompleteControl));

        public static readonly DependencyProperty DataTypeScopeProperty =
            DependencyProperty.Register(nameof(DataTypeScope), typeof(string),
                typeof(DataTypeSelectorAutoCompleteControl));

        public static readonly DependencyProperty IsEquipmentSequenceProperty =
            DependencyProperty.Register(nameof(IsEquipmentSequence), typeof(bool),
                typeof(DataTypeSelectorAutoCompleteControl));

        public static readonly DependencyProperty IsBatchParameterProperty =
            DependencyProperty.Register(nameof(IsBatchParameter), typeof(bool),
                typeof(DataTypeSelectorAutoCompleteControl));

        public static readonly DependencyProperty UsageProperty =
            DependencyProperty.Register(nameof(Usage), typeof(string), typeof(DataTypeSelectorAutoCompleteControl));

        public static readonly DependencyProperty IsInvisibleProperty = DependencyProperty.Register(nameof(IsInvisible),
            typeof(bool), typeof(DataTypeSelectorAutoCompleteControl),
            new FrameworkPropertyMetadata(false,
                HandleIsInvisibleChanged));

        public static readonly DependencyProperty IsFocusWithinProperty =
            DependencyProperty.Register(nameof(IsFocusWithin), typeof(bool),
                typeof(DataTypeSelectorAutoCompleteControl), new FrameworkPropertyMetadata());

        public static readonly DependencyProperty StructureMembersProperty =
            DependencyProperty.Register(nameof(StructureMembers), typeof(bool),
                typeof(DataTypeSelectorAutoCompleteControl));

        private static RoutedEvent EditCommittedEvent =
            EditableControlEvents.EditCommittedEvent.AddOwner(typeof(DataTypeSelectorAutoCompleteControl));

        private static RoutedEvent EditCanceledEvent =
            EditableControlEvents.EditCanceledEvent.AddOwner(typeof(DataTypeSelectorAutoCompleteControl));

        public static readonly DependencyProperty ChoiceListProperty =
            DataTypeAutoCompleteControl.ChoiceListProperty.AddOwner(typeof(DataTypeSelectorAutoCompleteControl),
                new FrameworkPropertyMetadata(
                    ChoiceListPropertyChangedCallback));

        public static readonly DependencyProperty DisableMultiDimensionalArraysProperty =
            DataTypeAutoCompleteControl.DisableMultiDimensionalArraysProperty.AddOwner(
                typeof(DataTypeSelectorAutoCompleteControl), new FrameworkPropertyMetadata()
                {
                    Inherits = true
                });


        private string _storedDataTypeName;
        private bool _isEditing;

        public IEnumerable<string> ChoiceList
        {
            get { return (IEnumerable<string>) GetValue(ChoiceListProperty); }
            set { SetValue(ChoiceListProperty, value); }
        }

        private static void ChoiceListPropertyChangedCallback(
            DependencyObject controlInstance,
            DependencyPropertyChangedEventArgs args)
        {
            ((DataTypeSelectorAutoCompleteControl) controlInstance).DataTypeAutoCompleteBox.ChoiceList =
                (IEnumerable<string>) args.NewValue;
        }

        static DataTypeSelectorAutoCompleteControl()
        {
            DataTypeAutoCompleteControl.DisableMultiDimensionalArraysProperty.OverrideMetadata(
                typeof(DataTypeAutoCompleteControl), new FrameworkPropertyMetadata()
                {
                    Inherits = true
                });
        }

        public DataTypeSelectorAutoCompleteControl()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler EditCommitted
        {
            add { AddHandler(EditCommittedEvent, value); }
            remove { RemoveHandler(EditCommittedEvent, value); }
        }

        public event RoutedEventHandler EditCanceled
        {
            add { AddHandler(EditCanceledEvent, value); }
            remove { RemoveHandler(EditCanceledEvent, value); }
        }

        public event EventHandler<TextChangedEventArgs> DataTypeNameChanged;

        public string DataTypeName
        {
            get { return (string) GetValue(DataTypeNameProperty); }
            set { SetValue(DataTypeNameProperty, value); }
        }

        public string DataTypeScope
        {
            get { return (string) GetValue(DataTypeScopeProperty); }
            set { SetValue(DataTypeScopeProperty, value); }
        }

        public bool DisableMultiDimensionalArrays
        {
            get { return (bool) GetValue(DisableMultiDimensionalArraysProperty); }
            set
            {
                SetValue(DisableMultiDimensionalArraysProperty,
                    value);
            }
        }

        public bool IsEquipmentSequence
        {
            get { return (bool) GetValue(IsEquipmentSequenceProperty); }
            set { SetValue(IsEquipmentSequenceProperty, value); }
        }

        public bool IsBatchParameter
        {
            get { return (bool) GetValue(IsBatchParameterProperty); }
            set { SetValue(IsBatchParameterProperty, value); }
        }

        public string Usage
        {
            get { return (string) GetValue(UsageProperty); }
            set { SetValue(UsageProperty, value); }
        }

        public bool IsInvisible
        {
            get { return (bool) GetValue(IsInvisibleProperty); }
            set { SetValue(IsInvisibleProperty, value); }
        }

        public bool IsFocusWithin
        {
            get { return (bool) GetValue(IsFocusWithinProperty); }
            private set { SetValue(IsFocusWithinProperty, value); }
        }

        public bool StructureMembers
        {
            get { return (bool) GetValue(StructureMembersProperty); }
            set { SetValue(StructureMembersProperty, value); }
        }

        public bool IsEditing
        {
            get { return _isEditing; }
            private set { _isEditing = value; }
        }

        public bool FocusAndSelectContents()
        {
            if (!DataTypeAutoCompleteBox.Focus())
                return false;
            DataTypeAutoCompleteBox.SelectAll();
            return true;
        }

        public bool FocusAndExpandContents()
        {
            if (!DataTypeAutoCompleteBox.Focus())
                return false;
            LaunchDataTypeSelectorDialog();
            return true;
        }

        public void BeginEdit()
        {
            _storedDataTypeName = DataTypeName;
            IsEditing = true;
        }

        public void CancelEdit()
        {
            IsEditing = false;
            if (DataTypeAutoCompleteBox.Text != _storedDataTypeName)
            {
                GetBindingExpression(DataTypeNameProperty)?.UpdateTarget();
                OnEditCanceled();
            }

            _storedDataTypeName = string.Empty;
        }

        public void CommitEdit(bool forceUpdateSource)
        {
            IsEditing = false;
            if (forceUpdateSource || DataTypeAutoCompleteBox.Text != _storedDataTypeName)
            {
                GetBindingExpression(DataTypeNameProperty)?.UpdateSource();
                OnEditCommitted();
            }

            _storedDataTypeName = string.Empty;
        }

        protected virtual void OnDataTypeNameChanged(TextChangedEventArgs e)
        {
            EventHandler<TextChangedEventArgs> dataTypeNameChanged = DataTypeNameChanged;
            dataTypeNameChanged?.Invoke(this, e);
        }

        protected virtual void OnEditCanceled()
        {
            RaiseEvent(new RoutedEventArgs(EditCanceledEvent, this));
        }

        protected virtual void OnEditCommitted()
        {
            RaiseEvent(new RoutedEventArgs(EditCommittedEvent, this));
        }

        private static void HandleIsInvisibleChanged(
            DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            UpdateInvisibleVisualState(sender as FrameworkElement,
                (bool) e.NewValue);
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private static bool UpdateInvisibleVisualState(FrameworkElement element, bool isInvisible)
        {
            return VisualStateManager.GoToElementState(element, isInvisible ? "Invisible" : "Visible", false);
        }

        private void HandleRootLoaded(object sender, RoutedEventArgs e)
        {
            MultiBinding multiBinding = new MultiBinding
            {
                Converter = new BooleanOrMultiConverter()
            };
            multiBinding.Bindings.Add(new Binding("IsFocused")
            {
                Source = DataTypeAutoCompleteBox
            });
            multiBinding.Bindings.Add(new Binding("IsFocused")
            {
                Source = DataTypeSelectorButton
            });
            SetBinding(IsFocusWithinProperty, multiBinding);
            if (IsInvisible)
                DataTypeAutoCompleteBox.Foreground = Brushes.Transparent;
            else
                DataTypeAutoCompleteBox.Foreground = SystemColors.ControlTextBrush;
        }

        private void HandleDataTypeSelectorButtonClick(object sender, RoutedEventArgs e)
        {
            LaunchDataTypeSelectorDialog();
        }

        private void ExecuteLaunchDataTypeSelector(object sender, ExecutedRoutedEventArgs e)
        {
            LaunchDataTypeSelectorDialog();
        }

        private void HandleTextChanged(object sender, TextChangedEventArgs e)
        {
            e.Handled = true;
            OnDataTypeNameChanged(e);
        }

        private void HandleRootGotFocus(object sender, RoutedEventArgs e)
        {
            DataTypeAutoCompleteBox.Focus();
            e.Handled = true;
        }

        private void LaunchDataTypeSelectorDialog()
        {
            throw new NotImplementedException();
            //if (this.StructureMembers)
            //{
            //    DataTypeBrowser dataTypeBrowser1 = new DataTypeBrowser();
            //    dataTypeBrowser1.Owner = Window.GetWindow((DependencyObject) this);
            //    dataTypeBrowser1.Selection = this.DataTypeName;
            //    dataTypeBrowser1.DataTypeScope = this.DataTypeScope;
            //    DataTypeBrowser dataTypeBrowser2 = dataTypeBrowser1;
            //    bool? nullable = dataTypeBrowser2.ShowDialog();
            //    if ((nullable.HasValue ? (nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0)
            //        return;
            //    this.DataTypeName = dataTypeBrowser2.Selection;
            //}
            //else
            //{
            //    DataTypeSelectorDialogWrapper selectorDialogWrapper = new DataTypeSelectorDialogWrapper();
            //    if (this.IsEquipmentSequence)
            //    {
            //        if (!selectorDialogWrapper.ShowDialogForEquipmentSequence(this.DataTypeName, this.Usage,
            //            this.IsBatchParameter))
            //            return;
            //        this.DataTypeName = selectorDialogWrapper.Selection;
            //    }
            //    else
            //    {
            //        if (!selectorDialogWrapper.ShowDialog(this.DataTypeName, this.DisableMultiDimensionalArrays))
            //            return;
            //        this.DataTypeName = selectorDialogWrapper.Selection;
            //    }
            //}
        }

    }
}
