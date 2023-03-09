using System;
using System.Windows;
using System.Windows.Input;
using ICSStudio.Gui.View;

namespace ICSStudio.Components.Controls
{
    public class EditableWrappingTextBox : WrappingTextBox, IEditableControl
    {
        private static readonly RoutedEvent EditCommittedEvent =
            EditableControlEvents.EditCommittedEvent.AddOwner(typeof(EditableWrappingTextBox));

        private static readonly RoutedEvent EditCanceledEvent =
            EditableControlEvents.EditCanceledEvent.AddOwner(typeof(EditableWrappingTextBox));

        private string _storedText;
        private bool _isEditing;
        private bool _enterPressedLast;


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

        public bool IsFocusWithin => IsFocused;
        public bool IsInvisible { get; set; }

        public bool IsEditing
        {
            get { return _isEditing; }
            private set { _isEditing = value; }
        }

        public bool FocusAndSelectContents()
        {
            if (!Focus())
                return false;
            SelectAll();
            return true;
        }

        public bool FocusAndExpandContents()
        {
            return false;
        }

        public void BeginEdit()
        {
            IsEditing = true;
            _storedText = Text;
        }

        public void CancelEdit()
        {
            IsEditing = false;
            if (Text != _storedText)
            {
                GetBindingExpression(WrapTextProperty)?.UpdateTarget();
                RaiseEditCanceledEvent();
            }

            _storedText = string.Empty;
        }

        public void CommitEdit(bool forceUpdateSource)
        {
            IsEditing = false;
            if (forceUpdateSource || Text != _storedText)
            {
                GetBindingExpression(WrapTextProperty)?.UpdateSource();
                RaiseEditCommittedEvent();
            }

            _storedText = string.Empty;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Return && e.KeyboardDevice.Modifiers != ModifierKeys.Control)
            {
                if (_enterPressedLast)
                {
                    if (CaretIndex >= Text.Length)
                        Text = Text.Remove(CaretIndex - Environment.NewLine.Length,
                            Environment.NewLine.Length);

                    SingleClickEditControl visualParentOfType =
                        VisualTreeHelpers.FindVisualParentOfType<SingleClickEditControl>(this);

                    if (visualParentOfType != null)
                    {
                        visualParentOfType.CommitEditAndMoveFocus();
                        e.Handled = true;
                    }

                    _enterPressedLast = false;
                }
                else
                    _enterPressedLast = true;
            }
            else
                _enterPressedLast = false;
        }

        private void RaiseEditCanceledEvent()
        {
            RaiseEvent(new RoutedEventArgs(EditCanceledEvent, this));
        }

        private void RaiseEditCommittedEvent()
        {
            RaiseEvent(new RoutedEventArgs(EditCommittedEvent, this));
        }
    }
}
