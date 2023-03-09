using System;
using System.Collections.Generic;

namespace ICSStudio.Dialogs.ConfigDialogs
{
    public class CommandManager
    {
        private Stack<ICommand> _undoCommands;
        private Stack<ICommand> _redoCommands;

        public CommandManager()
        {
            _undoCommands = new Stack<ICommand>();
            _redoCommands = new Stack<ICommand>();
        }

        public bool CanUndo
        {
            get { return _undoCommands.Count > 0; }
        }

        public bool CanRedo
        {
            get { return _redoCommands.Count > 0; }
        }

        public void Undo(int level)
        {
            int undoCount = Math.Min(level, _undoCommands.Count);

            for (int i = 0; i < undoCount; i++)
            {
                var command = _undoCommands.Pop();

                command.UnExecute();

                _redoCommands.Push(command);
            }
        }

        public void Redo(int level)
        {
            int redoCount = Math.Min(level, _redoCommands.Count);

            for (int i = 0; i < redoCount; i++)
            {
                var command = _redoCommands.Pop();

                command.Execute();

                _undoCommands.Push(command);
            }
        }

        public void AddCommand(ICommand command)
        {
            _undoCommands.Push(command);
            _redoCommands.Clear();
        }
    }
}
