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
using System.Text.RegularExpressions;
using ICSStudio.AvalonEdit.Document;

namespace ICSStudio.AvalonEdit.Indentation
{
	/// <summary>
	/// Handles indentation by copying the indentation from the previous line.
	/// Does not support indenting multiple lines.
	/// </summary>
	public class DefaultIndentationStrategy : IIndentationStrategy
	{
		/// <inheritdoc/>
		public virtual void IndentLine(TextDocument document, DocumentLine line)
		{
			if (document == null)
				throw new ArgumentNullException("document");
			if (line == null)
				throw new ArgumentNullException("line");
			DocumentLine previousLine = line.PreviousLine;
            indentStart:
			if (previousLine != null) {
				ISegment indentationSegment = TextUtilities.GetWhitespaceAfter(document, previousLine.Offset);
				string indentation = document.GetText(indentationSegment);
                var text = document.GetText(previousLine.Offset, previousLine.Length);
                if (IsLineComment(text))
                {
                    previousLine = previousLine.PreviousLine;
                    goto indentStart;
                }

                for (int i = 0; i < CalculateIndentationDistance(text); i++) indentation += "\t";

				indentationSegment = TextUtilities.GetWhitespaceAfter(document, line.Offset);
				document.Replace(indentationSegment.Offset, indentationSegment.Length, indentation,
								 OffsetChangeMappingType.RemoveAndInsert);
				// OffsetChangeMappingType.RemoveAndInsert guarantees the caret moves behind the new indentation.
			}
		}

		/// <summary>
		/// Does nothing: indenting multiple lines is useless without a smart indentation strategy.
		/// </summary>
		public virtual void IndentLines(TextDocument document, int beginLine, int endLine)
		{
		}

        private bool IsLineComment(string lineText)
        {
            lineText = lineText.Trim();
            if (string.IsNullOrEmpty(lineText)) return false;
            if (lineText.StartsWith(@"//") ||
                lineText.StartsWith(@"(*") ||
                lineText.EndsWith(@"*)")||
                lineText.StartsWith(@"/*")||
                lineText.EndsWith(@"*/"))
            {
                //Determine whether a line is a comment, and then indent to the position of the response;
                return true;
            }

            return false;
        }

        private int CalculateIndentationDistance(string lineText)
        {
            int distance = 0;
            if (string.IsNullOrEmpty(lineText)) return distance;
            var keywords = new string[]
            {
                "if","end_if","while","end_while",
                "for","end_for","case","end_case",
                "repeat","end_repeat"
            };
            var matches = GetKeywordsWithoutComment(lineText, keywords);
            var stack = new Stack<string>();
            foreach (var item in matches)
            {
                if ("if".Equals(item.Value, StringComparison.OrdinalIgnoreCase) ||
                    "while".Equals(item.Value, StringComparison.OrdinalIgnoreCase) ||
                    "for".Equals(item.Value, StringComparison.OrdinalIgnoreCase) ||
                    "case".Equals(item.Value, StringComparison.OrdinalIgnoreCase) ||
                    "repeat".Equals(item.Value, StringComparison.OrdinalIgnoreCase))
                {
                    stack.Push(item.Value);
                }
                else
                {
                    var startKeyword = string.Empty;
                    if ("end_if".Equals(item.Value, StringComparison.OrdinalIgnoreCase))
                    {
                        startKeyword = "if";
                    }
                    else if ("end_while".Equals(item.Value, StringComparison.OrdinalIgnoreCase))
                    {
                        startKeyword = "while";
                    }
                    else if ("end_for".Equals(item.Value, StringComparison.OrdinalIgnoreCase))
                    {
                        startKeyword = "for";
                    }
                    else if ("end_case".Equals(item.Value, StringComparison.OrdinalIgnoreCase))
                    {
                        startKeyword = "case";
                    }
                    else if ("end_repeat".Equals(item.Value, StringComparison.OrdinalIgnoreCase))
                    {
                        startKeyword = "repeat";
                    }

                    if (stack.Count>0 && stack.Peek().Equals(startKeyword, StringComparison.OrdinalIgnoreCase))
                        stack.Pop();
                }
            }

            foreach (var item in stack)
            {
                if ("if".Equals(item, StringComparison.OrdinalIgnoreCase) ||
                    "while".Equals(item, StringComparison.OrdinalIgnoreCase) ||
                    "for".Equals(item, StringComparison.OrdinalIgnoreCase) ||
                    "case".Equals(item, StringComparison.OrdinalIgnoreCase) ||
                    "repeat".Equals(item, StringComparison.OrdinalIgnoreCase))
                {
                    distance++;
                }
            }

            return distance;
        }

        public static List<Match> GetKeywordsWithoutComment(string text,string[] keywords)
        {
            var matches = new List<Match>();
            if (string.IsNullOrEmpty(text) ||keywords.Length == 0) return matches;
            var keywordRegex = string.Empty;
            foreach (var item in keywords)
                keywordRegex = string.IsNullOrEmpty(keywordRegex) ? item : keywordRegex + "|" + item;
            var commentRegex = @"\(\*|\*\)|\/\*|\*\/|(//)|(#region)|(#endregion)";
            var results = Regex.Matches(text, $@"(\b({keywordRegex})\b)|{commentRegex}|(\n)", RegexOptions.IgnoreCase);
            var isBlockComment = false;
            var isLineComment = false;
            var commentStartKeyword = string.Empty;
            for (var i = 0; i < results.Count; i++)
            {
                var item = results[i];

                if (isBlockComment)
                {
                    if ((commentStartKeyword.Equals("(*") && item.Value.Equals("*)")) || (commentStartKeyword.Equals("/*") && item.Value.Equals("*/")))
                    {
                        isBlockComment = false;
                        commentStartKeyword = string.Empty;
                        continue;
                    }
                }
                else
                {
                    if (!isLineComment 
                        && (item.Value.Equals("//")||
                            item.Value.Equals("#region",StringComparison.OrdinalIgnoreCase)||
                            item.Value.Equals("#endregion",StringComparison.OrdinalIgnoreCase)))
                    {
                        isLineComment = true;
                        continue;
                    }

                    if (isLineComment && item.Value.Equals("\n"))
                    {
                        isLineComment = false;
                        continue;
                    }

                    if (isLineComment) continue;

                    if (item.Value.Equals("\n")) continue;

                    if (item.Value.Equals("(*") || item.Value.Equals("/*"))
                    {
                        isBlockComment = true;
                        commentStartKeyword = item.Value;
                        continue;
                    }

                    if (!item.Value.Equals("*)") && !item.Value.Equals("*/")) matches.Add(item);
                }
            }

            return matches;
        }
    }
}
