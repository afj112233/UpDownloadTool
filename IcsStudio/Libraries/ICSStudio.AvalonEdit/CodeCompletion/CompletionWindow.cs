// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using ICSStudio.AvalonEdit.Document;
using ICSStudio.AvalonEdit.Editing;

namespace ICSStudio.AvalonEdit.CodeCompletion
{
    /// <summary>
    /// The code completion window.
    /// </summary>
    public class CompletionWindow : CompletionWindowBase
    {
        readonly CompletionList completionList = new CompletionList();
        ToolTip toolTip = new ToolTip();

        /// <summary>
        /// Gets the completion list used in this completion window.
        /// </summary>
        public CompletionList CompletionList
        {
            get { return completionList; }
        }

        /// <summary>
        /// Creates a new code completion window.
        /// </summary>
        public CompletionWindow(TextArea textArea) : base(textArea)
        {
            // keep height automatic
            this.CloseAutomatically = true;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.MaxHeight = 300;
            //this.Width = 175;
            this.Content = completionList;
            // prevent user from resizing window to 0x0
            this.MinHeight = 15;
            this.MinWidth = 90;

            toolTip.PlacementTarget = this;
            toolTip.Placement = PlacementMode.Right;
            WeakEventManager<ToolTip, RoutedEventArgs>.AddHandler(toolTip, "Closed", toolTip_Closed);
            //toolTip.Closed += toolTip_Closed;
            ResizeMode = ResizeMode.NoResize;
            AttachEvents();
        }

        #region ToolTip handling

        void toolTip_Closed(object sender, RoutedEventArgs e)
        {
            // Clear content after tooltip is closed.
            // We cannot clear is immediately when setting IsOpen=false
            // because the tooltip uses an animation for closing.
            if (toolTip != null)
                toolTip.Content = null;
        }

        void completionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = completionList.SelectedItem;
            if (item == null)
                return;
            object description = item.Description;
            if (description != null)
            {
                string descriptionText = description as string;
                if (descriptionText != null)
                {
                    toolTip.Content = new TextBlock
                    {
                        Text = descriptionText,
                        TextWrapping = TextWrapping.Wrap
                    };
                }
                else
                {
                    toolTip.Content = description;
                }

                toolTip.IsOpen = true;
            }
            else
            {
                toolTip.IsOpen = false;
            }
        }

        #endregion

        void completionList_InsertionRequested(object sender, EventArgs e)
        {
            Close();
            // The window must close before Complete() is called.
            // If the Complete callback pushes stacked input handlers, we don't want to pop those when the CC window closes.
            var item = completionList.SelectedItem;
            item?.Complete(this.TextArea,
                new AnchorSegment(this.TextArea.Document, this.StartOffset, this.EndOffset - this.StartOffset + 1), e);
        }
        
        void AttachEvents()
        {
            this.completionList.InsertionRequested += completionList_InsertionRequested;
            this.completionList.SelectionChanged += completionList_SelectionChanged;
            this.TextArea.Caret.PositionChanged += CaretPositionChanged;
            this.TextArea.MouseWheel += textArea_MouseWheel;
            this.TextArea.PreviewTextInput += textArea_PreviewTextInput;
        }

        /// <inheritdoc/>
        protected override void DetachEvents()
        {
            this.completionList.InsertionRequested -= completionList_InsertionRequested;
            this.completionList.SelectionChanged -= completionList_SelectionChanged;
            this.TextArea.Caret.PositionChanged -= CaretPositionChanged;
            this.TextArea.MouseWheel -= textArea_MouseWheel;
            this.TextArea.PreviewTextInput -= textArea_PreviewTextInput;
            base.DetachEvents();
        }

        /// <inheritdoc/>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (toolTip != null)
            {
                toolTip.IsOpen = false;
                toolTip = null;
            }
        }

        /// <inheritdoc/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled)
            {
                completionList.HandleKey(e);
            }
        }

        void textArea_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = RaiseEventPair(this, PreviewTextInputEvent, TextInputEvent,
                new TextCompositionEventArgs(e.Device, e.TextComposition));
        }

        void textArea_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = RaiseEventPair(GetScrollEventTarget(),
                PreviewMouseWheelEvent, MouseWheelEvent,
                new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta));
        }

        UIElement GetScrollEventTarget()
        {
            if (completionList == null)
                return this;
            return completionList.ScrollViewer ?? completionList.ListBox ?? (UIElement) completionList;
        }

        /// <summary>
        /// Gets/Sets whether the completion window should close automatically.
        /// The default value is true.
        /// </summary>
        public bool CloseAutomatically { get; set; }

        /// <inheritdoc/>
        protected override bool CloseOnFocusLost
        {
            get { return this.CloseAutomatically; }
        }

        /// <summary>
        /// When this flag is set, code completion closes if the caret moves to the
        /// beginning of the allowed range. This is useful in Ctrl+Space and "complete when typing",
        /// but not in dot-completion.
        /// Has no effect if CloseAutomatically is false.
        /// </summary>
        public bool CloseWhenCaretAtBeginning { get; set; }

        void CaretPositionChanged(object sender, EventArgs e)
        {
            int offset = this.TextArea.Caret.Offset;
            if (offset == this.StartOffset)
            {
                if (CloseAutomatically && CloseWhenCaretAtBeginning)
                {
                    Close();
                }
                else
                {
                    completionList.SelectItem(string.Empty);
                }

                return;
            }

            if (offset < this.StartOffset || offset > this.EndOffset+1)
            {
                if (CloseAutomatically)
                {
                    Close();
                }
            }
            //else
            //{
            //    TextDocument document = this.TextArea.Document;
            //    if (document != null)
            //    {
            //        int start = 0, end = 0;
            //        var code = GetCode(TextArea, false, ref start, ref end);
            //        completionList.SelectItem(code.Item1 + code.Item2);
            //    }
            //}
        }

        public static Tuple<string, string> GetCode(TextArea textArea, bool isWhole, ref int start, ref int end)
        {
            List<string> endSign = new List<string>()
                {" ", "\n", "\t", "=", "+", "*", "-", "&", ";", ",", "\r", "(", ")", "<", ">", "|", "&", "%"};
            int rightBracketCount = 0;
            string frontCode = "", backCode = "";
            string text = textArea.Document.Text;
            start = textArea.Caret.Offset - 1;
            while (start > -1)
            {
                var c = text[start];
                if (endSign.Contains(c.ToString()))
                {
                    if (isWhole && rightBracketCount > 0)
                    {

                    }
                    else
                    {
                        start++;
                        break;
                    }
                }

                if (c == ']')
                {
                    if (!isWhole)
                    {
                        start++;
                        break;
                    }

                    rightBracketCount++;
                }

                if (c == '[')
                {
                    if (!isWhole || rightBracketCount == 0)
                    {
                        start++;
                        break;
                    }

                    rightBracketCount--;
                }

                frontCode = c + frontCode;
                start--;
            }

            bool isColon = false;
            if (start < 0)
            {
                start = 0;
            }

            end = textArea.Caret.Offset;
            //rightBracketCount = 0;
            while (end < text.Length)
            {
                var c = text[end];

                if (c == ':')
                {
                    isColon = true;
                }

                if (c == '[')
                {
                    if (!isWhole || rightBracketCount == 0)
                    {
                        end--;
                        break;
                    }

                    rightBracketCount++;
                }

                if (c == ']')
                {
                    if (!isWhole)
                    {
                        end--;
                        break;
                    }

                    rightBracketCount--;
                }

                if (endSign.Contains(c.ToString()))
                {
                    if (isColon)
                    {
                        backCode = backCode.Remove(backCode.Length - 1, 1);
                        end -= 2;
                        break;
                    }

                    if (isWhole && rightBracketCount > 0)
                    {

                    }
                    else
                    {
                        end--;
                        break;
                    }
                }

                if ((c == '.' && rightBracketCount == 0) || rightBracketCount < 0)
                {
                    end--;
                    break;
                }

                backCode = backCode + c;
                end++;
            }

            if (end >= text.Length)
                end = text.Length - 1;
            if (!isWhole && frontCode.StartsWith(".") && (frontCode.Length + backCode.Length) > 1)
            {
                frontCode = frontCode.Substring(1);
                start++;
            }

            return new Tuple<string, string>(frontCode, backCode);
        }
    }
}
