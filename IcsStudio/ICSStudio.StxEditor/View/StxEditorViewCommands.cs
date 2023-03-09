using System.Windows.Input;

namespace ICSStudio.StxEditor.View
{
    public static class StxEditorViewCommands
    {
        public static RoutedUICommand IncreaseZoom { get; } =
            new RoutedUICommand("IncreaseZoom", "IncreaseZoom", typeof(StxEditorView));

        public static RoutedUICommand DecreaseZoom { get; } =
            new RoutedUICommand("DecreaseZoom", "DecreaseZoom", typeof(StxEditorView));

        public static RoutedUICommand Comment { get; } =
            new RoutedUICommand("Comment", "Comment", typeof(StxEditorView));

        public static RoutedUICommand Uncomment { get; } =
            new RoutedUICommand("Uncomment", "Uncomment", typeof(StxEditorView));

        public static RoutedUICommand IncreaseIndent { get; } =
            new RoutedUICommand("IncreaseIndent", "IncreaseIndent", typeof(StxEditorView));

        public static RoutedUICommand DecreaseIndent { get; } =
            new RoutedUICommand("DecreaseIndent", "DecreaseIndent", typeof(StxEditorView));

        public static RoutedUICommand ToggleWhite { get; } =
            new RoutedUICommand("ToggleWhite", "ToggleWhite", typeof(StxEditorView));

        public static RoutedUICommand ToggleValue { get; } =
            new RoutedUICommand("ToggleValue", "ToggleValue", typeof(StxEditorView));

        public static RoutedUICommand PendingCommand { get; } =
            new RoutedUICommand("PendingCommand", "PendingCommand", typeof(StxEditorView));

        public static RoutedUICommand OriginalCommand { get; } =
            new RoutedUICommand("OriginalCommand", "OriginalCommand", typeof(StxEditorView));

        public static RoutedUICommand TestCommand { get; } =
            new RoutedUICommand("TestCommand", "TestCommand", typeof(StxEditorView));

        #region Pending Command

        public static RoutedUICommand StartPendingRoutine { get; } =
            new RoutedUICommand("StartPendingRoutine", "StartPendingRoutine", typeof(StxEditorView));

        public static RoutedUICommand AcceptPendingRoutine { get; } = new RoutedUICommand("AcceptPendingRoutine",
            "AcceptPendingRoutine", typeof(StxEditorView));

        public static RoutedUICommand CancelPendingRoutine { get; } = new RoutedUICommand("CancelPendingRoutine",
            "CancelPendingRoutine", typeof(StxEditorView));

        public static RoutedUICommand AssembledAcceptPendingRoutine { get; } =
            new RoutedUICommand("AssembledAcceptPendingRoutine", "AssembledAcceptPendingRoutine",
                typeof(StxEditorView));

        public static RoutedUICommand CancelAcceptedPendingRoutine { get; } =
            new RoutedUICommand("CancelAcceptedPendingRoutine", "CancelAcceptedPendingRoutine", typeof(StxEditorView));

        public static RoutedUICommand AcceptPendingProgram { get; } = new RoutedUICommand("AcceptPendingProgram",
            "AcceptPendingProgram", typeof(StxEditorView));

        public static RoutedUICommand CancelPendingProgram { get; } = new RoutedUICommand("CancelPendingProgram",
            "CancelPendingProgram", typeof(StxEditorView));

        public static RoutedUICommand TestAcceptedProgram { get; } =
            new RoutedUICommand("TestAcceptedProgram", "TestAcceptedProgram", typeof(StxEditorView));

        public static RoutedUICommand UnTestAcceptedProgram { get; } = new RoutedUICommand("UnTestAcceptedProgram",
            "UnTestAcceptedProgram", typeof(StxEditorView));

        public static RoutedUICommand AssembledAcceptedProgram { get; } =
            new RoutedUICommand("AssembledAcceptedProgram", "AssembledAcceptedProgram", typeof(StxEditorView));

        public static RoutedUICommand CancelAcceptedProgram { get; } = new RoutedUICommand("CancelAcceptedProgram",
            "CancelAcceptedProgram", typeof(StxEditorView));

        public static RoutedUICommand Finalize { get; } =
            new RoutedUICommand("Finalize", "Finalize", typeof(StxEditorView));

        #endregion
    }
}
