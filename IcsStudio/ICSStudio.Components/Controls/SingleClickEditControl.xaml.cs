using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ICSStudio.Components.View;
using ICSStudio.Gui.View;

namespace ICSStudio.Components.Controls
{
    /// <summary>
    /// SingleClickEditControl.xaml 的交互逻辑
    /// </summary>
    public partial class SingleClickEditControl
    {
        private static readonly Brush EditableForegroundBrush = SystemColors.ControlTextBrush;
        private static readonly Brush TransparentBrush = new SolidColorBrush(Colors.Transparent);
        private static readonly string TranslationViewerPopupRootName = "TranslationViewerPanel";

        private DataGrid _parentGrid;
        private IEditableControl _editableControlInterface;

        public SingleClickEditControl()
        {
            InitializeComponent();
            SetBinding(ParentDataGridRowIsSelectedProperty,
                new Binding("IsSelected")
                {
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor)
                    {
                        AncestorType = typeof(DataGridRow)
                    }
                });
            SetBinding(IsKeyboardFocusWithinParentGridProperty,
                new Binding("IsKeyboardFocusWithin")
                {
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor)
                    {
                        AncestorType = typeof(DataGrid)
                    }
                });
            SetBinding(ParentDataGridCellIsFocusedProperty,
                new Binding("IsFocused")
                {
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor)
                    {
                        AncestorType = typeof(DataGridCell)
                    }
                });
        }

        #region DependencyProperty

        #region EditableControlProperty

        public static readonly DependencyProperty EditableControlProperty = DependencyProperty.Register(
            nameof(EditableControl), typeof(Control), typeof(SingleClickEditControl),
            new FrameworkPropertyMetadata(
                HandleEditableControlChanged));


        public Control EditableControl
        {
            get { return (Control) GetValue(EditableControlProperty); }
            set { SetValue(EditableControlProperty, value); }
        }

        private static void HandleEditableControlChanged(
            DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            SingleClickEditControl clickEditControl = (SingleClickEditControl) sender;
            Control newValue = (Control) e.NewValue;
            if (clickEditControl.IsInDescriptionMode)
                newValue.Foreground = new SolidColorBrush(Colors.Transparent);

            clickEditControl._editableControlInterface = e.NewValue as IEditableControl;
            if (clickEditControl.AllowDrop)
            {
                if (clickEditControl._editableControlInterface == null)
                    return;
                ObjectAnimationUsingKeyFrames resource1 =
                    (ObjectAnimationUsingKeyFrames) clickEditControl.Resources[
                        "InvisibilityAnimationDropEnabled"];

                // ReSharper disable once SuspiciousTypeConversion.Global
                Storyboard.SetTarget(resource1,
                    (DependencyObject) clickEditControl._editableControlInterface);

                Storyboard resource2 = (Storyboard) clickEditControl.Resources["MakeVisibleStoryboard"];
                resource2.Children.Add(resource1);
                resource2.Children.Remove(
                    (Timeline) clickEditControl.Resources["InvisibilityAnimationDropDisabled"]);
            }
            else
            {
                DoubleAnimationUsingKeyFrames resource1 =
                    (DoubleAnimationUsingKeyFrames) clickEditControl.Resources[
                        "InvisibilityAnimationDropDisabled"];
                Storyboard.SetTarget(resource1, clickEditControl.EditableControl);
                Storyboard resource2 = (Storyboard) clickEditControl.Resources["MakeVisibleStoryboard"];
                resource2.Children.Add(resource1);
                resource2.Children.Remove(
                    (Timeline) clickEditControl.Resources["InvisibilityAnimationDropEnabled"]);
            }

            clickEditControl.UpdateCommonStatesGroup();
            clickEditControl.EditableControlPlaceholder.Children.Add(newValue);
        }

        #endregion

        #region ReadOnlyTextProperty

        public static readonly DependencyProperty ReadOnlyTextProperty =
            DependencyProperty.Register(nameof(ReadOnlyText), typeof(string), typeof(SingleClickEditControl));

        public string ReadOnlyText
        {
            get { return (string) GetValue(ReadOnlyTextProperty); }
            set { SetValue(ReadOnlyTextProperty, value); }
        }


        #endregion

        #region ReadOnlyTextPaddingProperty

        public static readonly DependencyProperty ReadOnlyTextPaddingProperty =
            DependencyProperty.Register(nameof(ReadOnlyTextPadding), typeof(Thickness), typeof(SingleClickEditControl));

        public Thickness ReadOnlyTextPadding
        {
            get { return (Thickness) GetValue(ReadOnlyTextPaddingProperty); }
            set { SetValue(ReadOnlyTextPaddingProperty, value); }
        }

        #endregion

        #region IsReadOnlyProperty

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly),
            typeof(bool), typeof(SingleClickEditControl),
            new FrameworkPropertyMetadata(
                HandleIsReadOnlyPropertyChanged));

        public bool IsReadOnly
        {
            get { return (bool) GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        private static void HandleIsReadOnlyPropertyChanged(
            DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            SingleClickEditControl clickEditControl = sender as SingleClickEditControl;
            if (clickEditControl == null)
                return;

            if ((bool) e.NewValue)
            {
                clickEditControl.DisableEditing();
                clickEditControl.CancelEdit();
            }
            else
                clickEditControl.EnableEditing();

            clickEditControl.UpdateCommonStatesGroup();
            clickEditControl.OnIsReadOnlyChanged();
        }

        #endregion

        #region ParentDataGridRowIsSelectedProperty

        public static readonly DependencyProperty ParentDataGridRowIsSelectedProperty = DependencyProperty.Register(
            "ParentDataGridRowIsSelected", typeof(bool), typeof(SingleClickEditControl),
            new FrameworkPropertyMetadata(false,
                HandleParentDataGridRowIsSelectedChanged));


        private static void HandleParentDataGridRowIsSelectedChanged(
            DependencyObject sender,
            DependencyPropertyChangedEventArgs args)
        {
            SingleClickEditControl clickEditControl = sender as SingleClickEditControl;

            clickEditControl?.UpdateCommonStatesGroup();
        }

        public bool IsParentDataGridRowSelected =>
            (bool) GetValue(ParentDataGridRowIsSelectedProperty);

        #endregion

        #region ParentDataGridCellIsFocusedProperty

        public static readonly DependencyProperty ParentDataGridCellIsFocusedProperty = DependencyProperty.Register(
            "ParentDataGridCellIsFocused", typeof(bool), typeof(SingleClickEditControl),
            new FrameworkPropertyMetadata(false,
                HandleParentDataGridCellIsFocusedChanged));

        private static void HandleParentDataGridCellIsFocusedChanged(
            DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            SingleClickEditControl clickEditControl = sender as SingleClickEditControl;

            clickEditControl?.UpdateCommonStatesGroup();
        }

        public bool IsParentDataGridCellFocused =>
            (bool) GetValue(ParentDataGridCellIsFocusedProperty);

        #endregion

        #region IsKeyboardFocusWithinParentGridProperty

        public static readonly DependencyProperty IsKeyboardFocusWithinParentGridProperty =
            DependencyProperty.Register(nameof(IsKeyboardFocusWithinParentGrid), typeof(bool),
                typeof(SingleClickEditControl), new FrameworkPropertyMetadata(false));

        public bool IsKeyboardFocusWithinParentGrid
        {
            get { return (bool) GetValue(IsKeyboardFocusWithinParentGridProperty); }
            set { SetValue(IsKeyboardFocusWithinParentGridProperty, value); }
        }

        #endregion

        #region ReadOnlyTextBlockStyleProperty

        public static readonly DependencyProperty ReadOnlyTextBlockStyleProperty =
            DependencyProperty.Register(nameof(ReadOnlyTextBlockStyle), typeof(Style), typeof(SingleClickEditControl),
                new FrameworkPropertyMetadata());

        public Style ReadOnlyTextBlockStyle
        {
            get { return (Style) GetValue(ReadOnlyTextBlockStyleProperty); }
            set { SetValue(ReadOnlyTextBlockStyleProperty, value); }
        }

        #endregion

        #region IsInDescriptionModeProperty

        public static readonly DependencyProperty IsInDescriptionModeProperty = DependencyProperty.Register(
            nameof(IsInDescriptionMode), typeof(bool), typeof(SingleClickEditControl),
            new FrameworkPropertyMetadata(false,
                HandleIsInDescriptionModePropertyChanged));

        public bool IsInDescriptionMode
        {
            get { return (bool) GetValue(IsInDescriptionModeProperty); }
            set { SetValue(IsInDescriptionModeProperty, value); }
        }

        private static void HandleIsInDescriptionModePropertyChanged(
            DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            SingleClickEditControl clickEditControl = (SingleClickEditControl) sender;
            DoubleKeyFrame keyFrame =
                ((DoubleAnimationUsingKeyFrames)
                    ((TimelineGroup) clickEditControl.Resources["MakeVisibleStoryboard"]).Children
                    .First(t => t.Name == "ReadOnlyTextBlockAnimation")).KeyFrames[0];
            if ((bool) e.NewValue)
            {
                keyFrame.Value = 1.0;
                if (clickEditControl.EditableControl == null)
                    return;
                clickEditControl.EditableControl.Foreground = new SolidColorBrush(Colors.Transparent);
            }
            else
                keyFrame.Value = 0.0;
        }

        #endregion

        #region ShowEditableControlOnlyOnEditProperty

        public static readonly DependencyProperty ShowEditableControlOnlyOnEditProperty =
            DependencyProperty.Register(nameof(ShowEditableControlOnlyOnEdit), typeof(bool),
                typeof(SingleClickEditControl));

        public bool ShowEditableControlOnlyOnEdit
        {
            get { return (bool) GetValue(ShowEditableControlOnlyOnEditProperty); }
            set { SetValue(ShowEditableControlOnlyOnEditProperty, value); }
        }

        #endregion

        #region ForceUpdateSourceOnCommitProperty

        public static readonly DependencyProperty ForceUpdateSourceOnCommitProperty =
            DependencyProperty.Register(nameof(ForceUpdateSourceOnCommit), typeof(bool),
                typeof(SingleClickEditControl));


        public bool ForceUpdateSourceOnCommit
        {
            get { return (bool) GetValue(ForceUpdateSourceOnCommitProperty); }
            set { SetValue(ForceUpdateSourceOnCommitProperty, value); }
        }

        #endregion



        #endregion

        public void RefreshVisualState()
        {
            UpdateCommonStatesGroup();
        }

        public void CommitEdit()
        {
            if (_editableControlInterface == null || !_editableControlInterface.IsEditing)
                return;
            _editableControlInterface.CommitEdit(ForceUpdateSourceOnCommit);
            HideEditableContent();
        }

        public void CommitEditAndMoveFocus()
        {
            VisualTreeHelpers.FindVisualParentOfType<DataGridCell>(this)?.Focus();
            CommitEdit();
        }

        public void CancelEdit()
        {
            if (_editableControlInterface == null || !_editableControlInterface.IsEditing)
                return;
            _editableControlInterface.CancelEdit();
            HideEditableContent();
        }

        #region Protected

        public event EventHandler IsReadOnlyChanged;

        protected virtual void OnIsReadOnlyChanged()
        {
            EventHandler isReadOnlyChanged = IsReadOnlyChanged;
            isReadOnlyChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Private

        private void UpdateCommonStatesGroup()
        {
            if (IsReadOnly && IsParentDataGridRowSelected && IsKeyboardFocusWithinParentGrid)
                VisualStateManager.GoToElementState(Root, "ReadOnly_RowSelected_FocusInGrid", true);
            else if (IsReadOnly && IsParentDataGridRowSelected && !IsKeyboardFocusWithinParentGrid)
                VisualStateManager.GoToElementState(Root, "ReadOnly_RowSelected_FocusNotInGrid", true);
            else if (IsReadOnly)
                VisualStateManager.GoToElementState(Root, "ReadOnly_RowUnselected", true);
            else if ((AllowDrop && _editableControlInterface != null &&
                      !_editableControlInterface.IsFocusWithin ||
                      !AllowDrop && !EditableControl.IsFocused) &&
                     (!ShowEditableControlOnlyOnEdit && IsMouseOver))
                VisualStateManager.GoToElementState(Root, "Unfocused_MouseOver", true);
            else if ((AllowDrop && _editableControlInterface != null &&
                      _editableControlInterface.IsFocusWithin ||
                      !AllowDrop && EditableControl.IsFocused) && IsMouseOver)
                VisualStateManager.GoToElementState(Root, "Focused_MouseOver", true);
            else if ((AllowDrop && _editableControlInterface != null &&
                      _editableControlInterface.IsFocusWithin ||
                      !AllowDrop && EditableControl.IsFocused) && !IsMouseOver)
                VisualStateManager.GoToElementState(Root, "Focused_MouseNotOver", true);
            else if (IsParentDataGridCellFocused && !ShowEditableControlOnlyOnEdit)
                VisualStateManager.GoToElementState(Root, "Parent_Cell_Focused", false);
            else
                VisualStateManager.GoToElementState(Root, "Normal", true);
        }

        private void ShowEditableContent()
        {
            if (IsInDescriptionMode)
            {
                EditableControl.ClearValue(WidthProperty);
                EditableControl.ClearValue(HeightProperty);
                if (EditableForegroundBrush != null)
                    EditableControl.Foreground = EditableForegroundBrush;

                TextBoxBase editableControl = EditableControl as TextBoxBase;
                if (editableControl != null)
                    editableControl.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            }

            ReadOnlyTextBlock.Visibility = Visibility.Hidden;
        }

        private void HideEditableContent()
        {
            ReadOnlyTextBlock.Visibility = Visibility.Visible;
            if (!IsInDescriptionMode)
                return;

            EditableControl.Width = ReadOnlyTextBlock.ActualWidth;
            EditableControl.Height = ReadOnlyTextBlock.ActualHeight;
            EditableControl.Foreground = TransparentBrush;

            TextBoxBase editableControl = EditableControl as TextBoxBase;
            if (editableControl != null)
            {
                editableControl.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }

        }

        private void DisableEditing()
        {
            if (EditableControl == null)
                return;
            EditableControl.IsEnabled = false;
        }

        private void EnableEditing()
        {
            if (EditableControl == null)
                return;
            EditableControl.IsEnabled = true;
        }

        private void ExecuteCommitEdit(object sender, ExecutedRoutedEventArgs e)
        {
            CommitEditAndMoveFocus();
        }

        private void ExecuteCancelEdit(object sender, ExecutedRoutedEventArgs e)
        {
            VisualTreeHelpers.FindVisualParentOfType<DataGridCell>(this)?.Focus();
            CancelEdit();
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            if (EditableControl != null)
            {
                EditableControl.PreviewGotKeyboardFocus +=
                    HandleEditableControlPreviewGotKeyboardFocus;
                EditableControl.PreviewLostKeyboardFocus +=
                    HandleEditableControlPreviewLostKeyboardFocus;
                EditableControl.LostFocus += HandleEditableControlLostFocus;
                EditableControl.GotKeyboardFocus +=
                    HandleEditableControlGotKeyboardFocus;
            }

            _parentGrid = VisualTreeHelpers.FindVisualParentOfType<DataGrid>(this);
            if (_parentGrid != null)
                _parentGrid.IsKeyboardFocusWithinChanged +=
                    HandleIsKeyboardFocusWithinParentGridChanged;
            HideEditableContent();
        }

        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            if (_parentGrid != null)
                _parentGrid.IsKeyboardFocusWithinChanged -=
                    HandleIsKeyboardFocusWithinParentGridChanged;
            if (EditableControl == null)
                return;
            EditableControl.PreviewGotKeyboardFocus -=
                HandleEditableControlPreviewGotKeyboardFocus;
            EditableControl.PreviewLostKeyboardFocus -=
                HandleEditableControlPreviewLostKeyboardFocus;
            EditableControl.LostFocus -= HandleEditableControlLostFocus;
            EditableControl.GotKeyboardFocus -=
                HandleEditableControlGotKeyboardFocus;
        }

        private void HandleMouseEnter(object sender, MouseEventArgs e)
        {
            UpdateCommonStatesGroup();
        }

        private void HandleMouseLeave(object sender, MouseEventArgs e)
        {
            UpdateCommonStatesGroup();
        }

        private void HandleGotFocus(object sender, RoutedEventArgs e)
        {
            EditableControl.Focus();
            e.Handled = true;
        }

        private void CanExecuteDelete(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            e.Handled = true;
        }

        private void HandleEditableControlPreviewGotKeyboardFocus(
            object sender,
            KeyboardFocusChangedEventArgs e)
        {
            if (IsReadOnly)
                return;
            ShowEditableContent();
            UpdateCommonStatesGroup();
        }

        private void HandleEditableControlPreviewLostKeyboardFocus(
            object sender,
            KeyboardFocusChangedEventArgs e)
        {
            DependencyObject newFocus = e.NewFocus as DependencyObject;
            if (TranslationViewer.IsElementInAttachedTranslationViewer(newFocus, EditableControl))
            {
                Keyboard.AddPreviewLostKeyboardFocusHandler(VisualTreeHelpers.FindVisualRoot(this),
                    HandleTrackedKeyboardFocusLost);
                FrameworkElement visualParentByName =
                    VisualTreeHelpers.FindVisualParentByName(newFocus,
                        TranslationViewerPopupRootName);
                if (visualParentByName.Tag != null)
                    return;
                Keyboard.AddPreviewLostKeyboardFocusHandler(visualParentByName,
                    HandleTrackedKeyboardFocusLost);
                Keyboard.AddPreviewGotKeyboardFocusHandler(visualParentByName,
                    HandlePopupKeyboardFocusGot);
                visualParentByName.Unloaded += HandleTranslationPanelUnloaded;
                visualParentByName.Tag = new object();
            }
            else if (newFocus != null && FocusManager.GetFocusedElement(newFocus) != null &&
                     VisualTreeHelpers.IsObjectChildOf((DependencyObject) FocusManager.GetFocusedElement(newFocus),
                         this))
                ShowEditableContent();
            else
                HideEditableContent();
        }

        private void HandleEditableControlLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox originalSource = e.OriginalSource as TextBox;
            if (originalSource != null)
                originalSource.CaretIndex = 0;
        }

        private void HandleEditableControlGotKeyboardFocus(
            object sender,
            KeyboardFocusChangedEventArgs e)
        {
            UpdateCommonStatesGroup();
            if (_editableControlInterface == null || _editableControlInterface.IsEditing)
                return;
            _editableControlInterface.BeginEdit();
        }

        private void HandleIsKeyboardFocusWithinParentGridChanged(
            object sender,
            DependencyPropertyChangedEventArgs e)
        {
            UpdateCommonStatesGroup();
        }

        private void HandleTrackedKeyboardFocusLost(object sender, KeyboardFocusChangedEventArgs e)
        {
            DependencyObject newFocus = (DependencyObject) e.NewFocus;
            if (VisualTreeHelpers.IsObjectChildOf(newFocus, EditableControl))
            {
                Keyboard.RemovePreviewLostKeyboardFocusHandler(VisualTreeHelpers.FindVisualRoot(this),
                    HandleTrackedKeyboardFocusLost);
            }
            else
            {
                if (TranslationViewer.IsElementInAttachedTranslationViewer(newFocus, EditableControl))
                    return;
                Keyboard.RemovePreviewLostKeyboardFocusHandler(VisualTreeHelpers.FindVisualRoot(this),
                    HandleTrackedKeyboardFocusLost);
                HideEditableContent();
            }
        }

        private void HandlePopupKeyboardFocusGot(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!TranslationViewer.IsElementInAttachedTranslationViewer((DependencyObject) e.NewFocus, EditableControl))
                return;
            ShowEditableContent();
        }

        private void HandleTranslationPanelUnloaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;
            Contract.Assert(frameworkElement != null);

            Keyboard.RemovePreviewGotKeyboardFocusHandler(frameworkElement, HandlePopupKeyboardFocusGot);
            Keyboard.RemovePreviewLostKeyboardFocusHandler(frameworkElement, HandleTrackedKeyboardFocusLost);
            frameworkElement.Unloaded -= HandleTranslationPanelUnloaded;
            frameworkElement.Tag = null;
        }

        #endregion
    }
}
