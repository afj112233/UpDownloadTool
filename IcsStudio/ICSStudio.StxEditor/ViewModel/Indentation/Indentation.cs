using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using ICSStudio.AvalonEdit;
using ICSStudio.AvalonEdit.Indentation;
using ICSStudio.Gui.Annotations;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.StxEditor.ViewModel.Indentation
{
    internal static class Indentation
    {
        public static void Increase(TextEditor editor)
        {
            AddSomething("\t", editor);
        }

        public static void Decrease(TextEditor editor)
        {
            RemoveSpace("\t", editor);
        }

        public static void Comment(TextEditor editor)
        {
            AddSomething("//", editor);
        }

        public static void Uncomment(TextEditor editor)
        {
            var mark = CommentMark(editor);
            RemoveComment(!string.IsNullOrEmpty(mark) ? mark : "//", editor);
        }

        public static bool CheckIndentStatus(TextEditor editor)
        {
            if (editor.Text.Length > 0 && (editor.SelectionStart - 1 == -1 ||
                                           editor.Text.Substring(editor.SelectionStart - 1, 1) == "\n") &&
                (editor.SelectionLength + editor.SelectionStart + 1 > editor.Text.Length ||
                 editor.Text.Substring(editor.SelectionLength + editor.SelectionStart, 1) == "\r"))
            {
                if (editor.Text.Length > 0 && (editor.SelectionStart - 1 == -1 ||
                                               editor.Text.Substring(editor.SelectionStart - 1, 1) == "\n" &&

                                               editor.SelectionStart + 1 > editor.Text.Length) &&
                    editor.SelectionLength == 0)
                {
                    return false;
                }

                return true;
            }

            if (editor.SelectedText.Contains("\n")) return true;
            return false;
        }

        public static bool CheckCommentStatus(TextEditor editor)
        {
            Tuple<int, int> commentRange;
            if (IsSelectedComment(editor.Text,editor.SelectionStart,editor.SelectionLength,out commentRange)) return true;
            string[] strings = editor.Text.Split('\n');
            int start = editor.SelectionStart;
            int length = editor.SelectionLength;
            int totalLength = 0;
            int i = 0;
            var regex = new Regex(@"^//");
            foreach (var s in strings)
            {
                if (totalLength != 0 && totalLength >= start + length) return false;
                totalLength += s.Length;
                if (i > 0) totalLength += 1;
                if (totalLength >= start)
                {
                    var match = regex.Match(s);
                    if(match.Success)
                        if (match.Success && IsCommentStart(match.Index, editor.Text)) return true;
                }

                i++;
            }

            return false;
        }

        [CanBeNull]
        public static Match GetKeywordScopeLocation(string text, string endKeyword)
        {
            var startKeyword = string.Empty;
            switch (endKeyword)
            {
                case "end_if":
                case "elsif":
                    startKeyword = "if";
                    break;
                case "end_for":
                    startKeyword = "for";
                    break;
                case "end_case":
                    startKeyword = "case";
                    break;
                case "end_while":
                    startKeyword = "while";
                    break;
                case "end_repeat":
                case "until":
                    startKeyword = "repeat";
                    break;
                case "else":
                    startKeyword = "if|case";
                    break;
            }

            if (string.IsNullOrEmpty(startKeyword) || string.IsNullOrEmpty(endKeyword) ||
                string.IsNullOrEmpty(text)) return null;
            var startKeywordRegex = new Regex(startKeyword, RegexOptions.IgnoreCase);
            var matches = DefaultIndentationStrategy.GetKeywordsWithoutComment(text,new []{startKeyword,endKeyword});
            var stack = new Stack<string>();
            stack.Push(endKeyword);
            for (var i = matches.Count - 1; i >= 0; i--)
            {
                var item = matches[i];
                if (item.Value.Equals(endKeyword, StringComparison.OrdinalIgnoreCase))
                {
                    stack.Push(endKeyword);
                }
                else if (startKeywordRegex.IsMatch(item.Value))
                {
                    ExecutePopOperation:
                    if (stack.Count > 0 && !stack.Peek().Equals(endKeyword, StringComparison.OrdinalIgnoreCase))
                        stack.Pop();

                    if (stack.Count > 0 && stack.Peek().Equals(endKeyword, StringComparison.OrdinalIgnoreCase))
                        stack.Pop();

                    if (stack.Count == 0) return item;

                    if ("elsif".Equals(endKeyword, StringComparison.OrdinalIgnoreCase))
                    {
                        stack.Push(startKeyword);
                        goto ExecutePopOperation;
                    }
                }
            }

            return null;
        }

        private static string GetCommentSkinWhiteSpace(string text,ref int offset,int step,bool isEnd)
        {
            var code = "";
            while (step > 0 ? offset < text.Length : offset > -1)
            {
                var c = text[offset];
                if (char.IsWhiteSpace(c))
                {
                    if(string.IsNullOrEmpty(code))
                    {
                        offset += step;
                        continue;
                    }

                    offset -= step;
                    break;
                }else if (c == '*' ||  c == (isEnd?')': '(') || c == '/')
                {
                    code = step > 0 ? $"{code}{c}" : $"{c}{code}";
                    if(code.Length==2)
                        break;
                    offset += step;
                    continue;
                }
                else
                {
                    if(!string.IsNullOrEmpty(code))
                        offset -= step;
                    break;
                }
            }

            return code;
        }

        private static string CommentMark(TextEditor editor)
        {
            var location = editor.Document.GetLocation(editor.SelectionStart);
            int startLine = location.Line - 1;
            int start = editor.Document.Lines[startLine].Offset;
            if (editor.SelectionStart - 2 > -1 &&
                editor.SelectionStart + editor.SelectionLength + 2 <= editor.Text.Length)
            {
                var offset = editor.SelectionStart;
                var startCode1 = GetCommentSkinWhiteSpace(editor.Text,ref offset, 1, false);
                offset = editor.SelectionStart - 1;
                var startCode2 = GetCommentSkinWhiteSpace(editor.Text,ref offset , -1, false);
                offset = editor.SelectionStart + editor.SelectionLength - 1;
                var endCode1 = GetCommentSkinWhiteSpace(editor.Text, ref offset, -1, true);
                offset = editor.SelectionStart + editor.SelectionLength;
                var endCode2 = GetCommentSkinWhiteSpace(editor.Text,ref offset, 1, true);
                if ((startCode1 == "(*" || startCode2 == "(*") &&( endCode1 == "*)" || endCode2 == "*)")) 
                {
                    var flag = editor.Text.Substring(start, editor.SelectionStart - start).Contains("//");
                    if (flag) goto Step2;
                    return "(*";
                }

                if ((startCode1 == "/*" || startCode2 == "/*") &&(endCode1 == "*/" || endCode2 == "*/"))
                {
                    var flag = editor.Text.Substring(start, editor.SelectionStart - start).Contains("//");
                    if (flag) goto Step2;
                    return "/*";
                }
            }

            Step2:
            {
                string[] strings = editor.Text.Split('\n');
                var end = editor.Document.GetLocation(editor.SelectionStart + editor.SelectionLength - 1);
                var regex=new Regex(@"(^//)");
                for (int j = startLine; j < end.Line-1; j++)
                {
                    var s = strings[j];
                    var match = regex.Match(s);
                    if (match.Success)
                    {
                        return match.Value;
                    }
                }
            }

            return string.Empty;
        }

        private static bool IsSelectedComment(string text,int start,int length,out Tuple<int,int> commentRange)
        {
            try
            {
                commentRange=new Tuple<int, int>(-1,-1);
                if (length>0&&start > -1 &&
                    start + length <= text.Length)
                {
                    var offset = start;
                    var startCode1 = GetCommentSkinWhiteSpace(text, ref offset, 1, false);
                    var offset2 = start - 1;
                    var startCode2 = GetCommentSkinWhiteSpace(text, ref offset2, -1, false);
                    var offset3 = start + length - 1;
                    var endCode1 = GetCommentSkinWhiteSpace(text,ref offset3 , -1, true);
                    var offset4 = start + length;
                    var endCode2 = GetCommentSkinWhiteSpace(text,ref offset4 , 1, true);
                    if ((startCode1 == "(*" || startCode2 == "(*") && (endCode1 == "*)" || endCode2 == "*)"))
                    {
                        var flag = text.Substring(start, start - start).Contains("//");
                        if (flag) return false;
                        commentRange = new Tuple<int, int>(startCode1 == "(*" ? offset - 1 : offset2,
                            endCode1 == "*)" ? offset3 : offset4 - 1);
                        return true;
                    }

                    if ((startCode1 == "/*" || startCode2 == "/*") && (endCode1 == "*/" || endCode2 == "*/"))
                    {
                        var flag = text.Substring(start, start - start).Contains("//");
                        if (flag) return false;
                        commentRange = new Tuple<int, int>(startCode1 == "(*" ? offset - 1 : offset2,
                            endCode1 == "*)" ? offset3 : offset4 - 1);
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                commentRange = new Tuple<int, int>(-1, -1);
                return false;
            }

            return false;
        }

        private static void AddSomething(string something, TextEditor editor)
        {
            var editStatus = editor.GetLineEditStatus();
            string str = editor.SelectedText;
            string[] strs = str.Split('\n');
            string[] strings = editor.Text.Split('\n');
            int selectedStart = editor.SelectionStart;
            bool isCaretAtStart = editor.SelectionStart == editor.CaretOffset;
            int length = editor.SelectionLength;
            int index = FindInsertIndex(strs, strings, editor);
            var lines = editor.Document.Lines;
            int startLine = editor.TextArea.Document.GetLineByOffset(editor.SelectionStart).LineNumber - 1;
            if (length != 0 && length != editor.Text.Length && !IsSelectedWholeLine(strs, strings, startLine) &&
                something == "//" && !IsInLineComment(editor.Text, selectedStart,length))
            {
                editor.Document.Insert(selectedStart + length, "*)");
                editor.Document.Insert(selectedStart, "(*");
                editor.SelectionLength = 0;
                editor.SelectionStart = 0;
                editor.SelectionStart = selectedStart + 2;
                editor.SelectionLength = length;
                editor.Document.UndoStack.MergeCommemtOperation();
                return;
            }
            
            if (index == -1) return;
            bool isCommentStartLine = false;
            int count = 0;
            for (int i = 0; i < strs.Length; i++)
            {
                if (lines.Count >= index + i - 1)
                {
                    var code = strings[index + i];
                    if (length == 0)
                    {
                        isCommentStartLine = true;
                        strings[index + i] = code.Insert(code.StartsWith("\r") ? 1 : 0, something);
                        if (index + i < editStatus.Count)
                        {
                            editStatus[index + i] = true;
                        }
                        else
                        {
                            Debug.Assert(false);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(code.Trim().Replace("\r", "")))
                        {
                            if (i == 0)
                                isCommentStartLine = true;
                            count++;
                            strings[index + i] = code.Insert(code.StartsWith("\r")?1:0, something);
                            if (index + i < editStatus.Count)
                            {
                                editStatus[index + i] = true;
                            }
                            else
                            {
                                Debug.Assert(false);
                            }
                        }
                        else { editStatus[index + i] = false; }
                    }
                }
            }
            
            editor.Document.Replace(0,editor.Document.TextLength, String.Join("\n", strings));
            editor.RecoverLineEditStatus(editStatus);
            editor.SelectionLength = 0;
            editor.SelectionStart = 0;
            try
            {
                if (length == editor.Text.Length)
                {
                    editor.SelectionStart = 0;
                    editor.SelectionLength = editor.Text.Length;

                    if (isCaretAtStart)
                    {
                        editor.CaretOffset = editor.SelectionStart;
                    }
                    else
                    {
                        editor.CaretOffset = editor.SelectionLength;
                    }
                }
                else
                {
                    editor.SelectionStart = selectedStart + (isCommentStartLine ? something.Length : 0);
                    editor.SelectionLength =
                        length == 0 ? 0 : length + something.Length * (count - (isCommentStartLine ? 1 : 0));
                    if (isCaretAtStart)
                    {
                        editor.CaretOffset = editor.SelectionStart;
                    }
                    else
                    {
                        editor.CaretOffset = editor.SelectionStart + editor.SelectionLength;
                    }
                }
            }
            catch (Exception e)
            {
                Controller.GetInstance().Log($"AddSomething:{e.StackTrace}");
            }
        }

        private static void RemoveSpace(string something, TextEditor editor)
        {
            string str = editor.SelectedText;
            string[] strs = str.Split('\n');
            string[] strings = editor.Text.Split('\n');
            int index = FindInsertIndex(strs, strings, editor);
            int selectedStart = editor.SelectionStart;
            int length = editor.SelectionLength;
            Tuple<int, int> commentRange;
            if (something == "//" && IsSelectedComment(editor.Text,editor.SelectionStart,editor.SelectionLength,out commentRange))
            {
                string selectedText = editor.Text.Substring(selectedStart, length);
                if (!selectedText.Contains("//"))
                {
                    editor.Document.Remove(selectedStart + length, 2);
                    editor.Document.Remove(selectedStart - 2, 2);

                    editor.SelectionLength = 0;
                    editor.SelectionStart = 0;
                    editor.SelectionStart = selectedStart;
                    editor.SelectionLength = length;
                    editor.Document.UndoStack.MergeCommemtOperation();
                    return;
                }
            }

            if (index == -1) return;
            int totalLength = 0;
            int count = 0;
            int lossCount = 0;
            bool isStartOfLine = selectedStart == 0 || editor.Text.Substring(selectedStart - 1, 1) == "\n";
            for (int i = 0; i < strs.Length; i++)
            {
                var position = GetNotNullOrEmptyPosition(strings[index + i]);
                if (position == -1) continue;
                var removeIndex = strings[index + i].IndexOf(something);

                if (removeIndex == 0)
                {
                    if (i == 0)
                    {
                        lossCount += something.Length;
                    }

                    strings[index + i] = strings[index + i].Remove(removeIndex, something.Length);
                    //editor.Document.Remove(lines[index + i].Offset + removeIndex, something.Length);
                    if (strs[i].IndexOf(something) > -1)
                        totalLength += something.Length;
                }
                else
                {
                    for (int j = 3; j > 0; j--)
                    {

                        if (strings[index + i].IndexOf(" ") == 0)
                        {
                            if (i == 0)
                            {
                                lossCount++;
                                if (string.Join("\n", strings).Substring(selectedStart, 1) == " ")
                                    count++;
                            }

                            strings[index + i] = strings[index + i].Substring(1);
                            //editor.Document.Remove(lines[index + i].Offset, 1);
                            totalLength++;
                        }
                    }
                }
            }
            
            editor.Document.Replace(0,editor.Document.TextLength, string.Join("\n", strings));
            editor.SelectionLength = 0;
            editor.SelectionStart = 0;
            if (selectedStart != 0)
                if (isStartOfLine)
                    editor.SelectionStart = selectedStart;
                else
                    editor.SelectionStart = selectedStart - lossCount + count;
            else
                editor.SelectionStart = 0;
            if (length > 0)
                editor.SelectionLength = length - totalLength - (count > 0 ? count - 3 : 0);
        }

        private static bool IsStartWithLineComment(string[] strings,int line)
        {
            Regex regex= new Regex("^[ ]*//");
            for (int i = line; i < strings.Length; i++)
            {
                if (regex.IsMatch(strings[i])) return true;
            }

            return false;
        }

        private static void RemoveComment(string something, TextEditor editor)
        {
            try
            {
                string selectedText = editor.SelectedText;
                string[] strs = selectedText.Split('\n');
                string[] strings = editor.Text.Split('\n');
                int index = FindInsertIndex(strs, strings, editor);
                int selectedStart = editor.SelectionStart;
                int length = editor.SelectionLength;
                var location = editor.Document.GetLocation(editor.SelectionStart);
                Tuple<int, int> commentRange;
                if (IsSelectedComment(editor.Text,selectedStart,length,out commentRange))
                {
                    if (something != "//" || !IsStartWithLineComment(strings, location.Line-1))
                    {

                        if (selectedStart + length > commentRange.Item2)
                        {
                            length -= 2;
                        }

                        if (commentRange.Item1 < selectedStart)
                        {
                            selectedStart -= 2;
                        }
                        else
                        {
                            length -= 2;
                        }
                        
                        editor.Document.Remove(commentRange.Item2, 2);
                        editor.Document.Remove(commentRange.Item1,2);
                        editor.SelectionLength = 0;
                        editor.SelectionStart = 0;
                        editor.SelectionStart = selectedStart;
                        editor.SelectionLength = length;
                        editor.Document.UndoStack.MergeCommemtOperation();
                        return;
                    }
                }

                if (index == -1) return;
                int totalLength = 0;

                for (int i = 0; i < strs.Length; i++)
                {
                    var removeIndex = strings[index + i].IndexOf(something);

                    if (removeIndex ==0)
                    {
                        strings[index + i] = strings[index + i].Remove(removeIndex, something.Length);
                        if (i+ index == location.Line-1)
                        {
                            if (location.Column-1 == 0)
                            {
                                totalLength += something.Length;
                            }
                            else
                            {
                                selectedStart -= something.Length;
                            }
                            continue;
                        }
                        if (strs[i].IndexOf(something) > -1)
                            totalLength += something.Length;
                    }
                }

                editor.Document.Replace(0,editor.Document.TextLength, string.Join("\n", strings));

                editor.SelectionLength = 0;
                editor.SelectionStart = 0;
                editor.SelectionStart = Math.Max(selectedStart, 0);

                if (length > 0)
                {
                    var selectedLength = length - totalLength;

                    editor.SelectionLength = selectedLength;
                }
            }
            catch (Exception e)
            {
                Controller.GetInstance().Log($"RemoveComment():{e.StackTrace}");
            }

        }

        private static bool IsCommentStart(int index, string text)
        {
            index--;
            while (index > -1)
            {
                if (char.IsWhiteSpace(text[index]))
                {

                }
                else if (text[index] == '\n')
                {
                    return true;
                }
                else
                {
                    return false;
                }

                index--;
            }

            return true;
        }

        private static int GetNotNullOrEmptyPosition(string str)
        {
            int p = -1;
            var strs = str.ToCharArray();
            foreach (var c in strs)
            {
                if (c == ' ' || c == '\t') p++;
                else
                {
                    return p;
                }
            }

            return p;
        }

        private static int FindInsertIndex(string[] strs, string[] strings, TextEditor editor)
        {
            int startLine = 0;
            for (int i = 0; i < editor.SelectionStart; i++)
            {
                if (editor.Text.Substring(i, 1) == "\n") startLine = startLine + 1;
            }

            for (int i = startLine; i < strings.Length; i++)
            {
                if (strings[i].Contains(strs[0]))
                {
                    return i;
                }
            }

            Debug.Assert(false);
            return -1;
        }

        private static bool IsSelectedWholeLine(string[] strs, string[] strings,int startLine)
        {
            try
            {
                for (int i = 0; i < strs.Length; i++)
                {
                    if (strings[startLine + i].Replace("\r","") != strs[i].Replace("\r", "")) return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool IsInLineComment(string text,int start,int length)
        {
            string[] strings = text.Split('\n');
            int commentStart = 0;
            int commentEnd = 0;
            int offset = 0;
            foreach (var s in strings)
            {
                if (s.StartsWith("//"))
                {
                    commentStart = offset;
                    commentEnd= offset = offset + s.Length + 1;

                    if (commentStart < commentEnd && ((start <= commentStart &&
                                                       (start + length) >= commentStart) ||
                                                      (start >= commentStart &&
                                                       start <= commentEnd))) return true;
                    continue;
                }
                else
                {
                    int index = 0;
                    while (index>-1)
                    {
                        bool isChecked = false;
                        if (s.IndexOf("(*",index)>-1)
                        {
                            index = s.IndexOf("(*", index);
                            commentStart = offset+ index;
                            index++;
                            isChecked = true;
                        }
                        if (s.IndexOf("*)", index) > -1)
                        {
                            index = s.IndexOf("*)", index);
                            commentEnd = offset + index;
                            index++;
                            isChecked = true;
                        }
                        if (s.IndexOf("/*", index) > -1)
                        {
                            index = s.IndexOf("/*", index);
                            commentStart = offset + index;
                            index++;
                            isChecked = true;
                        }
                        if (s.IndexOf("*/", index) > -1)
                        {
                            index = s.IndexOf("*/", index);
                            commentStart = offset + index;
                            index++;
                            isChecked = true;
                        }
                        if(!isChecked)break;

                        if (commentStart < commentEnd && ((start <= commentStart &&
                                                           (start + length) >= commentStart) ||
                                                          (start >= commentStart &&
                                                           start <= commentEnd))) return true;
                    }

                    offset = offset + s.Length + 1;
                }
            }

            return false;
        }

    }
}
