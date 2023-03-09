using System.Windows;
using System.Windows.Controls;

namespace ICSStudio.Components.Controls
{
    public class DataGridComboBox : ComboBox, IEditableControl
    {
        private static readonly RoutedEvent EditCommittedEvent =
            EditableControlEvents.EditCommittedEvent.AddOwner(typeof(DataGridComboBox));

        private static readonly RoutedEvent EditCanceledEvent =
            EditableControlEvents.EditCanceledEvent.AddOwner(typeof(DataGridComboBox));

        private int _storedSelectedIndex;
        private bool _isEditing;

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
            return Focus();
        }

        public bool FocusAndExpandContents()
        {
            if (!Focus())
                return false;
            IsDropDownOpen = true;
            return true;
        }

        public void BeginEdit()
        {
            _storedSelectedIndex = SelectedIndex;
            IsEditing = true;
        }

        public void CancelEdit()
        {
            IsEditing = false;
            if (SelectedIndex == _storedSelectedIndex)
                return;

            GetBindingExpression(SelectedValueProperty)?.UpdateTarget();
            RaiseEditCanceledEvent();
        }

        public void CommitEdit(bool forceUpdateSource)
        {
            IsEditing = false;
            if (!forceUpdateSource && SelectedIndex == _storedSelectedIndex)
                return;

            GetBindingExpression(SelectedValueProperty)?.UpdateSource();
            RaiseEditCommittedEvent();
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
