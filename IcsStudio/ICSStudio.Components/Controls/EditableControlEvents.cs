using System.Windows;

namespace ICSStudio.Components.Controls
{
    public static class EditableControlEvents
    {
        public static RoutedEvent EditCommittedEvent = EventManager.RegisterRoutedEvent("EditCommitted",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EditableControlEvents));

        public static RoutedEvent EditCanceledEvent = EventManager.RegisterRoutedEvent("EditCanceled",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EditableControlEvents));
    }
}
