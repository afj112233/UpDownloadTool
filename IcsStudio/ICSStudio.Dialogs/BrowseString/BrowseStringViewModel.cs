using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Dialogs.BrowseString.RichTextBoxExtend;
using ICSStudio.Gui.View;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;

namespace ICSStudio.Dialogs.BrowseString
{
    internal class BrowseStringViewModel : ViewModelBase
    {
        private readonly IDataType _dataType;
        private string _inputMode = "INS";
        private int _count;
        private string _info;
        private string _error;
        private bool? _dialogResult;
        private DispatcherTimer _timer;
        private Message _message;
        private string _text;

        internal BrowseStringViewModel(IDataType dataType, Message message)
        {
            _dataType = dataType;
            _message = message;
            Title = $"Browse String - {_message.ItemName}";
            Text = message.Text;
            ButtonVisibility = message.ShowButton ? Visibility.Visible : Visibility.Collapsed;
            Debug.Assert(_dataType.FamilyType == FamilyType.StringFamily, dataType.Name);
            InsertCommand1 = new RelayCommand<Grid>(ExecuteInsertCommand1);
            InsertCommand2 = new RelayCommand<Grid>(ExecuteInsertCommand2);
            InsertCommand3 = new RelayCommand<Grid>(ExecuteInsertCommand3);
            InsertCommand4 = new RelayCommand<Grid>(ExecuteInsertCommand4);
            InsertCommand5 = new RelayCommand<Grid>(ExecuteInsertCommand5);
            InsertCommand6 = new RelayCommand<Grid>(ExecuteInsertCommand6);
            InsertCommand7 = new RelayCommand<Grid>(ExecuteInsertCommand7);
            OkCommand = new RelayCommand<Grid>(ExecuteOkCommand, CanExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            ApplyCommand = new RelayCommand<Grid>(ExecuteApplyCommand, CanExecuteApplyCommand);
            PreviousErrorCommand = new RelayCommand<Grid>(ExecutePreviousErrorCommand, CanExecutePreviousErrorCommand);
            NextErrorCommand = new RelayCommand<Grid>(ExecuteNextErrorCommand, CanExecuteNextErrorCommand);
            TextChangeCommand = new RelayCommand<TextChangedEventArgs>(ExecuteTextChangeCommand);
            KeyDownCommand = new RelayCommand<KeyEventArgs>(ExecuteKeyDownCommand);
            CloseCommand=new RelayCommand(ExecuteCloseCommand);
            SaveCommand=new RelayCommand(ExecuteSaveCommand);
            UpdateInfo(Text);
            _timer = new DispatcherTimer();
            WeakEventManager<DispatcherTimer, EventArgs>.AddHandler(_timer, "Tick", Timer_Tick);
            _timer.Interval = TimeSpan.FromMilliseconds(300);
            _timer.Start();
        }
        
        private void UpdateInfo(string text)
        {
            //_code = text;
            text = Format(text);
            string s1 = "";
            int length = 0;
            if (!string.IsNullOrEmpty(text))
            {
                if (text.Length > Size)
                {
                    s1 = text.Substring(0, Size);
                }
                else
                {
                    s1 = text;
                }

                length = text.Length;
            }

            s1 = UnFormatSpecial(s1);
            Count = length;
            RealCount = s1.Length;
        }

        public Visibility ButtonVisibility { set; get; }

        private void Timer_Tick(object sender, EventArgs e)
        {
            OkCommand.RaiseCanExecuteChanged();
            ApplyCommand.RaiseCanExecuteChanged();
            PreviousErrorCommand.RaiseCanExecuteChanged();
            NextErrorCommand.RaiseCanExecuteChanged();
        }

        public bool? DialogResult
        {
            set
            {
                Set(ref _dialogResult, value);
                _timer.Stop();
                WeakEventManager<DispatcherTimer, EventArgs>.RemoveHandler(_timer, "Tick", Timer_Tick);
            }
            get { return _dialogResult; }
        }

        public string Title { set; get; }

        //public int CaretPosition { set; get; }

        public string InputMode
        {
            set { Set(ref _inputMode, value); }
            get { return _inputMode; }
        }

        public string Error
        {
            set { Set(ref _error, value); }
            get { return _error; }
        }

        public int Count
        {
            set
            {
                Set(ref _count, value);
                _message.Len = value;
                Info = $"{_count} of {Size}";
                IsError = Size < _count;
                Error = $"{Math.Max(_count - Size, 0)} Error(s)";
            }
            get { return _count; }
        }

        public bool IsError { set; get; } = false;

        public int RealCount { set; get; }

        public string Info
        {
            set { Set(ref _info, value); }
            get { return _info; }
        }

        public string Message => _message.Text;
        public int Size => (_dataType as CompositiveType).TypeMembers["DATA"].DataTypeInfo.Dim1;

        #region InsertCommand

        public RelayCommand<Grid> InsertCommand1 { set; get; }

        private void ExecuteInsertCommand1(Grid grid)
        {
            var richTextBox = VisualTreeHelpers.FindFirstVisualChildOfType<RichTextBox>(grid);
            Command(richTextBox, "$$");
        }

        private void Command(RichTextBox richTextBox, string symbol)
        {
            if (richTextBox.Selection.IsEmpty)
            {
                var text = richTextBox.GetText();

                var index = richTextBox.CaretPosition.DocumentStart.GetOffsetToPosition(richTextBox.CaretPosition);
                index = index - 2 >= 0 ? index - 2 : index;
                Text = text.Insert(index, symbol);
                richTextBox.CaretPosition =
                    richTextBox.CaretPosition.DocumentStart.GetPositionAtOffset(index + symbol.Length*2);
                richTextBox.Focus();
            }
            else
            {
                var index = richTextBox.CaretPosition.DocumentStart.GetOffsetToPosition(richTextBox.Selection.Start);
                index = index - 2 >= 0 ? index - 2 : index;
                var text = richTextBox.GetText();
                text = text.Remove(index, richTextBox.Selection.Text.Replace("\r", "").Replace("\n", "").Length);
                Text = text.Insert(index, symbol);
                richTextBox.CaretPosition =
                    richTextBox.CaretPosition.DocumentStart.GetPositionAtOffset(index + symbol.Length*2);
                richTextBox.Focus();
            }
        }

        public RelayCommand<Grid> InsertCommand2 { set; get; }

        private void ExecuteInsertCommand2(Grid grid)
        {
            var richTextBox = VisualTreeHelpers.FindFirstVisualChildOfType<RichTextBox>(grid);
            //richTextBox?.AppendText("$'");
            Command(richTextBox, "$'");
        }

        public RelayCommand<Grid> InsertCommand3 { set; get; }

        private void ExecuteInsertCommand3(Grid grid)
        {
            var richTextBox = VisualTreeHelpers.FindFirstVisualChildOfType<RichTextBox>(grid);
            //richTextBox?.AppendText("$L");
            Command(richTextBox, "$L");
        }

        public RelayCommand<Grid> InsertCommand4 { set; get; }

        private void ExecuteInsertCommand4(Grid grid)
        {
            var richTextBox = VisualTreeHelpers.FindFirstVisualChildOfType<RichTextBox>(grid);
            //richTextBox?.AppendText("$R$L");
            Command(richTextBox, "$R$L");
        }

        public RelayCommand<Grid> InsertCommand5 { set; get; }

        private void ExecuteInsertCommand5(Grid grid)
        {
            var richTextBox = VisualTreeHelpers.FindFirstVisualChildOfType<RichTextBox>(grid);
            //richTextBox?.AppendText("$P");
            Command(richTextBox, "$P");
        }

        public RelayCommand<Grid> InsertCommand6 { set; get; }

        private void ExecuteInsertCommand6(Grid grid)
        {
            var richTextBox = VisualTreeHelpers.FindFirstVisualChildOfType<RichTextBox>(grid);
            //richTextBox?.AppendText("$R");
            Command(richTextBox, "$R");
        }

        public RelayCommand<Grid> InsertCommand7 { set; get; }

        private void ExecuteInsertCommand7(Grid grid)
        {
            var richTextBox = VisualTreeHelpers.FindFirstVisualChildOfType<RichTextBox>(grid);
            //richTextBox?.AppendText("$T");
            Command(richTextBox, "$T");
        }

        #endregion

        public RelayCommand<TextChangedEventArgs> TextChangeCommand { set; get; }

        //private string _code = "";

        private void ExecuteTextChangeCommand(TextChangedEventArgs e)
        {
            try
            {
                var richTextBox = (RichTextBox) e.Source;
                var index = richTextBox.CaretPosition.DocumentStart.GetOffsetToPosition(richTextBox.CaretPosition);
                //Debug.WriteLine($"Offset:{index}");
                var text = richTextBox.GetText();
                //int i = 0;
                IsChanged = false;

                text = Format(text);
                string s1 = "", s2 = "";
                int length = 0;
                if (!string.IsNullOrEmpty(text))
                {
                    if (text.Length > Size)
                    {
                        s1 = text.Substring(0, Size);
                        s2 = text.Substring(Size);
                    }
                    else
                    {
                        s1 = text;
                    }

                    length = text.Length;
                }

                s1 = UnFormatSpecial(s1);
                s2 = UnFormatSpecial(s2);
                Count = length;
                RealCount = s1.Length;
                var triggers = Interaction.GetTriggers(richTextBox);
                var click = triggers[0];
                triggers.Remove(click);
                richTextBox.Document.Blocks.Clear();
                var run1 = new Run(s1);
                var run2 = new Run(s2);
                run2.Foreground = Brushes.Red;
                run2.TextDecorations = TextDecorations.Underline;
                var paragraph = new Paragraph();
                paragraph.Inlines.Add(run1);
                paragraph.Inlines.Add(run2);
                richTextBox.Document.Blocks.Add(paragraph);

                triggers.Add(click);
                //CaretPosition = index;
                int len = 0;
                foreach (var textChange in e.Changes)
                {
                    len += textChange.AddedLength - textChange.RemovedLength;
                }

                Debug.WriteLine($"len:{len}---index:{index}");
                if (len >= 0)
                {
                    if (index == 3)
                    {

                    }
                    else
                    {
                        index = len > 1 ? index - 2 : index;
                    }

                    if (index - 5 == text.Length || text.Length == 1 && index == 1)
                    {
                        index += 2;
                    }
                }

                richTextBox.CaretPosition = richTextBox.CaretPosition.DocumentStart.GetPositionAtOffset(index);
                _message.Text = $"{s1}{s2}";
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            finally
            {
                if (_message?.Container != null)
                {
                    _message.Container.StaysOpen = IsError || IsOverflow;
                }
            }
        }

        public string Text
        {
            set { Set(ref _text, value); }
            get { return _text; }
        }

        private TextPointer TryGetTextPointer(int offset, RichTextBox richTextBox)
        {
            while (true)
            {
                var pointer = richTextBox.CaretPosition.DocumentStart.GetPositionAtOffset(offset);
                if (richTextBox.CaretPosition.DocumentStart.CompareTo(pointer) == 1)
                {
                    offset--;

                }
                else if (richTextBox.CaretPosition.DocumentStart.CompareTo(pointer) == -1)
                {
                    offset++;
                }
                else
                {
                    return pointer;
                }
            }
        }

        private string FormatSpecial(string value)
        {
            string str = "";
            string temp = "";
            foreach (var c in value)
            {
                temp = temp + c;
                var formatTemp = Format(temp);
                if (temp != formatTemp)
                {
                    str = str + formatTemp;
                    temp = "";
                }
            }

            if (temp.Length > 0)
                str = str + temp;
            //str = str.Replace("$", "单");
            return str;
        }

        private string Format(string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            str = Regex.Replace(str, @"\$r", "\r", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"\$l", "\n", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"\$t", "\t", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"\$p", "\f", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"\$00", "\0");
            str = Regex.Replace(str, @"\$\'", "'");
            str = Regex.Replace(str, @"\$\$", "双");
            return str;
        }

        private string UnFormatSpecial(string value)
        {
            string str = value;
            str = str.Replace("双", "$$");
            str = Regex.Replace(str, "\r", @"$r", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "\n", @"$l", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "\t", @"$t", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "\f", @"$p", RegexOptions.IgnoreCase);
            str = str.Replace("\0", "$00").Replace("'", "$'");
            return str;
        }

        public RelayCommand CloseCommand { set; get; }

        private void ExecuteCloseCommand()
        {
            _message.IsClose = true;
        }

        public RelayCommand SaveCommand { set; get; }

        private void ExecuteSaveCommand()
        {
            _message.IsClose = false;
        }

        public RelayCommand<KeyEventArgs> KeyDownCommand { set; get; }

        private void ExecuteKeyDownCommand(KeyEventArgs e)
        {
            if (e.Key == Key.Insert)
            {
                InputMode = InputMode == "INS" ? "OVR" : "INS";
            }

            if (e.Key == Key.Tab) e.Handled = true;
        }

        public RelayCommand<Grid> OkCommand { set; get; }

        private void ExecuteOkCommand(Grid grid)
        {
            ExecuteApplyCommand(grid);
            DialogResult = true;
        }

        private bool CanExecuteOkCommand(Grid grid)
        {
            if (Size >= _count) return true;
            return false;
        }

        public RelayCommand CancelCommand { set; get; }

        private void ExecuteCancelCommand()
        {
            DialogResult = false;
        }
        
        public bool IsOverflow => Count > Size;

        public bool IsChanged { set; get; } = false;
        public RelayCommand<Grid> ApplyCommand { set; get; }

        private void ExecuteApplyCommand(Grid grid)
        {
            var richTextBox = VisualTreeHelpers.FindFirstVisualChildOfType<RichTextBox>(grid);
            _message.SendMessage(richTextBox.GetText());
            IsChanged = true;
        }

        private bool CanExecuteApplyCommand(Grid grid)
        {
            if (IsChanged) return false;
            if (Size >= _count) return true;
            return false;
        }

        public RelayCommand<Grid> PreviousErrorCommand { set; get; }

        private void ExecutePreviousErrorCommand(Grid grid)
        {
            var richTextBox = VisualTreeHelpers.FindFirstVisualChildOfType<RichTextBox>(grid);
            var pos1 = richTextBox.Document.ContentStart.GetPositionAtOffset(RealCount + 2);
            richTextBox.Selection.Select(pos1, richTextBox.CaretPosition);
            richTextBox.Focus();
        }

        private bool CanExecutePreviousErrorCommand(Grid grid)
        {
            if (Count <= Size || grid == null) return false;
            var richTextBox = VisualTreeHelpers.FindFirstVisualChildOfType<RichTextBox>(grid);
            var pos = richTextBox.Document.ContentStart.GetPositionAtOffset(RealCount + 2);
            if (pos.CompareTo(richTextBox.CaretPosition) == -1) return true;
            return false;
        }

        public RelayCommand<Grid> NextErrorCommand { set; get; }

        private void ExecuteNextErrorCommand(Grid grid)
        {
            var richTextBox = VisualTreeHelpers.FindFirstVisualChildOfType<RichTextBox>(grid);
            var pos = richTextBox.Document.ContentStart.GetPositionAtOffset(RealCount + 2);
            richTextBox.Selection.Select(pos.CompareTo(richTextBox.CaretPosition) > 0 ? pos : richTextBox.CaretPosition,
                richTextBox.Document.ContentEnd);
            richTextBox.Focus();
        }

        private bool CanExecuteNextErrorCommand(Grid grid)
        {
            if (Count <= Size || grid == null) return false;
            var richTextBox = VisualTreeHelpers.FindFirstVisualChildOfType<RichTextBox>(grid);
            if (richTextBox.Document.ContentEnd.CompareTo(richTextBox.CaretPosition) == 1) return true;
            return false;
        }
    }
}
