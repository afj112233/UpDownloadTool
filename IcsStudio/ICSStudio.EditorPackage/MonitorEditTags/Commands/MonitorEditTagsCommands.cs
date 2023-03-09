using System;
using System.Windows.Input;

namespace ICSStudio.EditorPackage.MonitorEditTags.Commands
{
    public static class MonitorEditTagsCommands
    {
        public const string CopyTagsFormat = "MonitorEditTags_CopyTags";

        public static readonly RoutedUICommand NewTag =
            Create("New Variable...", nameof(NewTag), typeof(MonitorEditTagsCommands),
                new KeyGesture(Key.W, ModifierKeys.Control));


        internal static RoutedUICommand Create(string text, string name, Type type)
        {
            return new RoutedUICommand(text, name, type);
        }

        internal static RoutedUICommand Create(string text, string name, Type type, InputGesture inputGesture)
        {
            var command = new RoutedUICommand(text, name, type);
            command.InputGestures.Add(inputGesture);
            return command;
        }

    }
}
