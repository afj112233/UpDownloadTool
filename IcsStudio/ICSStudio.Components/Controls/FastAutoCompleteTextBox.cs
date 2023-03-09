using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Components.Model;
using ICSStudio.Interfaces.Tags;

namespace ICSStudio.Components.Controls
{
    public class FastAutoCompleteTextBox : AutoCompleteBox
    {
        private TextBox _textBox;
        private static readonly Regex TagNameValidChars = new Regex("^[a-zA-Z0-9_]*$", RegexOptions.Compiled);
        private static readonly Regex TagNameInvalidChars = new Regex("[^a-zA-Z0-9_\\.\\[\\]]+", RegexOptions.Compiled);

        private static readonly Regex ProgramPathValidChars =
            new Regex("^[a-zA-Z0-9,_:\\\\\\.\\[\\]]*$", RegexOptions.Compiled);

        private static readonly Regex ProgramPathInvalidChars =
            new Regex("[^a-zA-Z0-9,_:\\\\\\.\\[\\]]+", RegexOptions.Compiled);

        private static readonly Regex ControllerPathValidChars =
            new Regex("^[a-zA-Z0-9,_:\\\\\\.\\[\\]]*$", RegexOptions.Compiled);

        private static readonly Regex ControllerPathInvalidChars =
            new Regex("[^a-zA-Z0-9,_:\\\\\\.\\[\\]]+", RegexOptions.Compiled);

        private static string space = " ";

        public FastAutoCompleteTextBox()
        {
            Validation = TagPathValidationType.TagName;
            Loaded += FastAutoCompeteTextBox_Loaded;
            MinimumPrefixLength = 1;
            Populated += FastAutoCompeteTextBox_Populated;
            Populating += FastAutoCompeteTextBox_Populating;
            MouseDoubleClick += FastAutoCompleteTextBox_MouseDoubleClick;
        }

        private void FastAutoCompleteTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _textBox?.SelectAll();
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            "Data", typeof(Dictionary<ITag, TagNameNode>), typeof(FastAutoCompleteTextBox),
            new PropertyMetadata(default(Dictionary<ITag, TagNameNode>)));

        public Dictionary<ITag, TagNameNode> Data
        {
            get { return (Dictionary<ITag, TagNameNode>)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        private void OnSpace(object parameter)
        {
            InsertValidCharacters(space);
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
            if (_textBox == null) return;
            if (_textBox.IsReadOnly)
                return;
            if (Text == null)
                Text = "";
            text = text.Replace(' ', '_');
            text = InvalidChars.Replace(text, string.Empty);
            if (_textBox.MaxLength > 0 && Text.Length + text.Length - _textBox.SelectedText.Length > _textBox.MaxLength)
                text = text.Substring(0, _textBox.MaxLength - (Text.Length - _textBox.SelectedText.Length));
            if (_textBox.SelectionLength == 0)
            {
                int caretIndex = _textBox.CaretIndex;
                _textBox.BeginChange();
                _textBox.Text = _textBox.Text.Insert(caretIndex, text);
                //Text = Text.Insert(caretIndex, text);
                _textBox.CaretIndex = caretIndex + text.Length;
                _textBox.EndChange();
            }
            else
            {
                _textBox.BeginChange();
                _textBox.SelectedText = text;
                _textBox.CaretIndex += _textBox.SelectedText.Length;
                _textBox.SelectionLength = 0;
                _textBox.EndChange();
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

        public TagPathValidationType Validation { get; set; }

        private TagNameNode GetData(string[] splitName)
        {
            var name = splitName[0];
            var index = name.IndexOf('[');
            if (index > 0)
            {
                name = name.Substring(0, index);
            }

            var collection = Data
                ?.Where(d => d.Key.Name?.StartsWith(name, StringComparison.OrdinalIgnoreCase) ?? false);
            
            var tagNameNode = (collection
                ?.FirstOrDefault(d => d.Key.Name?.Equals(name, StringComparison.OrdinalIgnoreCase) ?? false).Value);
            if (tagNameNode == null&&(collection?.Any()??false))
            {
                tagNameNode = collection.First().Value;
            }
            return tagNameNode?.GetSpecificNode(splitName, 0);
        }

        private TagNameNode _lastTagNameNode;

        private void FastAutoCompeteTextBox_Populating(object sender, PopulatingEventArgs e)
        {
            if (Data != null)
            {
                var parameter = e.Parameter ?? "";

                var splitName = parameter.Split('.');

                if (splitName.Length == 1 && splitName[0].IndexOf("[") < 0)
                {
                    _lastTagNameNode = null;

                    ItemsSource = Data.Select(d => d.Key.Name).ToList();
                    PopulateComplete();
                    return;
                }

                var tagNameNode = GetData(splitName);

                if (tagNameNode == null)
                {
                    _lastTagNameNode = null;
                    ItemsSource = Data.Select(d => d.Key.Name).ToList();

                    //PopulateComplete();
                    return;
                }

                if (_lastTagNameNode == tagNameNode)
                {
                    //ItemsSource = ItemsSource;
                    //PopulateComplete();
                    return;
                }


                _lastTagNameNode = tagNameNode;

                var data = tagNameNode.GetSubNameList();
                ItemsSource = data.Where(d => d.StartsWith(e.Parameter));

                //k   PopulateComplete();
            }
        }

        private void FastAutoCompeteTextBox_Populated(object sender, PopulatedEventArgs e)
        {
            //Controller.GetInstance().Log($"-----FastAutoCompeteTextBox_Populated-----{e.Data}");
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (Template == null) return;
            _textBox = Template.FindName("Text", this) as TextBox;
            if (_textBox == null) return;
            _textBox.TextChanged += _textBox_TextChanged;
            Debug.Assert(_textBox != null);
            CommandManager.AddPreviewExecutedHandler(_textBox,
                HandlePreviewExecuteHandler);
            _textBox.InputBindings.Add(new KeyBinding(
                new RelayCommand<object>(OnSpace), new KeyGesture(Key.Space)));
            _textBox.InputBindings.Add(new KeyBinding(
                new RelayCommand<object>(OnSpace),
                new KeyGesture(Key.Space, ModifierKeys.Shift)));
        }

        private void _textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var index = _textBox.CaretIndex;
            var selectedLength = _textBox.SelectionLength;
            foreach (var textChange in e.Changes)
            {
                index = textChange.Offset;
                index = index + textChange.AddedLength;
            }

            if (index > -1)
            {
                _textBox.CaretIndex = index;
                _textBox.SelectionLength = selectedLength;
            }

            Text = _textBox.Text;
        }

        private void FastAutoCompeteTextBox_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.TextFilter = (search, value) =>
            {
                if (value.Length > 0)
                {
                    if (value.ToUpper().StartsWith(search.ToUpper()))
                    {
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            };
        }
    }

    public class TagNameNode
    {
        public TagNameNode(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public List<TagNameNode> SubNameNodes { get; } = new List<TagNameNode>();

        public List<string> GetSubNameList()
        {
            return SubNameNodes.Select(n => n.Name).ToList();
        }

        public TagNameNode GetSpecificNode(string[] splitName, int index)
        {
            if (splitName.Length - 2 < 0)
            {
                return this;
            }

            if (splitName.Length - 2 == index)
            {
                if (splitName[index].IndexOf("[") <= 0)
                    return this;
                else
                {
                    var name = string.Join(".", splitName, 0, index + 1);
                    foreach (var subNameNode in SubNameNodes)
                    {
                        if (name.Equals(subNameNode.Name, StringComparison.OrdinalIgnoreCase))
                            return subNameNode;
                    }
                }
            }

            {
                var name = string.Join(".", splitName, 0, index + 2);

                foreach (var subNameNode in SubNameNodes)
                {
                    if (name.Equals(subNameNode.Name, StringComparison.OrdinalIgnoreCase))
                        return subNameNode.GetSpecificNode(splitName, ++index);
                }
            }

            return null;
        }
    }
}
