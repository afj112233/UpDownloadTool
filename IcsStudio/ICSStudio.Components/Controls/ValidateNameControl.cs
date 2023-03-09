using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Components.Model;

namespace ICSStudio.Components.Controls
{
    public class ValidateNameControl : TextBox
    {
        private static readonly Regex TagNameValidChars = new Regex("^[a-zA-Z0-9_]*$", RegexOptions.Compiled);
        private static readonly Regex TagNameInvalidChars = new Regex("[^a-zA-Z0-9_]+", RegexOptions.Compiled);

        private static readonly Regex ProgramPathValidChars =
            new Regex("^[a-zA-Z0-9,_:\\\\\\.\\[\\]]*$", RegexOptions.Compiled);

        private static readonly Regex ProgramPathInvalidChars =
            new Regex("[^a-zA-Z0-9,_:\\\\\\.\\[\\]]+", RegexOptions.Compiled);

        private static readonly Regex ControllerPathValidChars =
            new Regex("^[a-zA-Z0-9,_:\\\\\\.\\[\\]]*$", RegexOptions.Compiled);

        private static readonly Regex ControllerPathInvalidChars =
            new Regex("[^a-zA-Z0-9,_:\\\\\\.\\[\\]]+", RegexOptions.Compiled);

        private static string space = " ";

        public ValidateNameControl()
        {
            Validation = TagPathValidationType.TagName;
            CommandManager.AddPreviewExecutedHandler(this,
                HandlePreviewExecuteHandler);
            InputBindings.Add(new KeyBinding(
                new RelayCommand<object>(OnSpace), new KeyGesture(Key.Space)));
            InputBindings.Add(new KeyBinding(
                new RelayCommand<object>(OnSpace),
                new KeyGesture(Key.Space, ModifierKeys.Shift)));

            MouseDoubleClick += ValidateNameControl_MouseDoubleClick;
        }

        private void ValidateNameControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            (sender as TextBox)?.SelectAll();
        }

        public TagPathValidationType Validation { get; set; }

        protected virtual Regex ValidChars
        {
            get
            {
                switch (Validation)
                {
                    case TagPathValidationType.TagName:
                        return TagNameValidChars;
                    case TagPathValidationType.ProgramScopedOperand:
                        return ProgramPathValidChars;
                    case TagPathValidationType.ControllerScopedOperand:
                        return ControllerPathValidChars;
                    default:
                        return TagNameValidChars;
                }
            }
        }

        protected virtual Regex InvalidChars
        {
            get
            {
                switch (Validation)
                {
                    case TagPathValidationType.TagName:
                        return TagNameInvalidChars;
                    case TagPathValidationType.ProgramScopedOperand:
                        return ProgramPathInvalidChars;
                    case TagPathValidationType.ControllerScopedOperand:
                        return ControllerPathInvalidChars;
                    default:
                        return TagNameInvalidChars;
                }
            }
        }

        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            if (e.Handled)
                return;
            if (e.Text == space)
                InsertValidCharacters(space);
            e.Handled = !ValidateText(e.Text);
            base.OnTextInput(e);
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            SetDragDropEffects(e);
            base.OnDragEnter(e);
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            SetDragDropEffects(e);
            base.OnDragOver(e);
        }

        protected override void OnDrop(DragEventArgs e)
        {
            SetDragDropEffects(e);
            base.OnDrop(e);
        }

        protected bool ValidateText(string text)
        {
            return ValidChars.IsMatch(text);
        }

        protected void HandlePreviewExecuteHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (e == null || e.Command != ApplicationCommands.Paste)
                return;
            if (Clipboard.ContainsText())
                InsertValidCharacters(Clipboard.GetText());
            e.Handled = true;
        }

        private void InsertValidCharacters(string text)
        {
            if (IsReadOnly)
                return;
            text = text.Replace(' ', '_');
            text = InvalidChars.Replace(text, string.Empty);
            if (MaxLength > 0 && Text.Length + text.Length - SelectedText.Length > MaxLength)
                text = text.Substring(0, MaxLength - (Text.Length - SelectedText.Length));
            if (SelectionLength == 0)
            {
                int caretIndex = CaretIndex;
                BeginChange();
                Text = Text.Insert(caretIndex, text);
                CaretIndex = caretIndex + text.Length;
                EndChange();
            }
            else
            {
                BeginChange();
                SelectedText = text;
                CaretIndex += SelectedText.Length;
                SelectionLength = 0;
                EndChange();
            }
        }

        private void SetDragDropEffects(DragEventArgs e)
        {
            string data = e.Data.GetData(DataFormats.UnicodeText, true) as string;

            if (data != null && ValidateText(data))
                return;

            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void OnSpace(object parameter)
        {
            InsertValidCharacters(space);
        }
    }
}
