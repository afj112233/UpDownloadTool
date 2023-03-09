using System.Windows;

namespace ICSStudio.Components.Controls
{
    public interface IEditableControl
    {
        event RoutedEventHandler EditCommitted;

        event RoutedEventHandler EditCanceled;

        bool IsFocusWithin { get; }

        bool IsInvisible { get; set; }

        bool IsEditing { get; }

        bool FocusAndSelectContents();

        bool FocusAndExpandContents();

        void BeginEdit();

        void CancelEdit();

        void CommitEdit(bool forceUpdateSource);
    }
}
