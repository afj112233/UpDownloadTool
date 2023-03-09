using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Antlr4.Runtime.Misc;
using ICSStudio.AvalonEdit.Document;
using ICSStudio.AvalonEdit.Folding;
using ICSStudio.SimpleServices.Common;
using ICSStudio.StxEditor.Interfaces;

namespace ICSStudio.StxEditor.ViewModel.Folding
{
    /// <summary>
    ///     Allows producing foldings from a document based on braces.
    /// </summary>
    public class BraceFoldingStrategy : IFoldingStrategy
    {
        private string[] _functions = {"if", "case", "for", "while", "repeat",};
        private string functionMatchStr = "(?<=(^| |\t|\n|;|:)){0}(?=(( )|\n|\r\n|\t|$))";
        private string endFunctionMatchStr = "(?<=(^| |\t|\n|;)){0}(?=(;|( )|\n|\r\n|\t|$))";

        public void UpdateFoldings(FoldingManager manager, TextDocument document)
        {
            int firstErrorOffset;
            var newFoldings = CreateNewFoldings(document, out firstErrorOffset);
            var sortedFoldingList = newFoldings.ToList();
            sortedFoldingList.Sort((x, y) => x.StartOffset - y.StartOffset);
            manager?.UpdateFoldings(sortedFoldingList, firstErrorOffset);
        }

        /// <summary>
        ///     Create <see cref="NewFolding" />s for the specified document.
        /// </summary>
        public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
        {
            firstErrorOffset = -1;
            return CreateNewFoldings(document);
        }

        /// <summary>
        ///     Create <see cref="NewFolding" />s for the specified document.
        /// </summary>
        public IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
        {
            var newFoldings = new List<NewFolding>();
            if (document == null)
            {
                return newFoldings;
            }
            string text = RoutineCodeTextExtension.ConvertCommentToWhiteBlank(document.Text,null,true,false,false);
            foreach (var s in _functions)
            {
                CreateFunctionFoldings(newFoldings, text, s);
            }

            try
            {
                // TODO:(Tony)暂时捕获异常不处理。
                // 需要重构 CreateRegionFoldings（用语法解析结果，不用正则）。
                // 现在正反向用例都不影响，只是测试反向用例时，方法返回会多没用的信息。
                CreateRegionFoldings(newFoldings, text);
                newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return newFoldings;
        }

        private void CreateFunctionFoldings(List<NewFolding> newFoldings, string text, string functionName)
        {
            try
            {
                Stack<int> stack = new Stack<int>();
                int endLength = functionName.Length + 4 + 1;
                string endFunctionName = $"end_{functionName}";
                Regex regex = new Regex(string.Format(functionMatchStr, functionName), RegexOptions.IgnoreCase);
                Regex endRegex = new Regex(string.Format(endFunctionName, endFunctionName), RegexOptions.IgnoreCase);
                var matches = regex.Matches(text);
                var endMatches = endRegex.Matches(text);
                int i = 0;

                for (int j = 0; j < matches.Count; j++)
                {
                    stack.Push(matches[j].Index);
                    if (endMatches.Count == 0 || i >= endMatches.Count || j + 1 >= matches.Count) continue;
                    if (j + 1 < matches.Count)
                    {
                        while (endMatches[i].Index < matches[j + 1].Index)
                        {
                            if (stack.Count <= 0) break;
                            int start = stack.Pop();
                            var offset = GetNewLineSymbol(start, endMatches[i].Index + 1, text);
                            if (offset != -1)
                            {
                                var end = Math.Min(endMatches[i].Index + endLength, text.Length);
                                newFoldings.Add(new NewFolding(offset, end));
                            }
                            
                            i++;
                            if (i >= endMatches.Count)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (i < endMatches.Count)
                        {
                            var offset = GetNewLineSymbol(stack.Pop(), endMatches[i].Index + 1, text);
                            if (offset != -1)
                            {
                                var end = Math.Min(endMatches[i].Index + endLength, text.Length);
                                newFoldings.Add(new NewFolding(offset, end));
                            }

                            i++;
                        }
                    }
                }

                for (int j = i; j < endMatches.Count; j++)
                {
                    if (stack.Count <= 0) break;
                    var offset = GetNewLineSymbol(stack.Pop(), endMatches[j].Index + 1, text);
                    if (offset != -1)
                    {
                        var end = Math.Min(endMatches[j].Index + endLength, text.Length);
                        newFoldings.Add(new NewFolding(offset, end));
                    }
                }

                if (stack.Count > 0)
                {
                    foreach (var i1 in stack)
                    {
                        var offset = GetNewLineSymbol(i1, text.Length - 1, text);
                        if (offset != -1)
                            newFoldings.Add(new NewFolding(offset, text.Length));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private int GetNewLineSymbol(int offset, int endOffset, string text)
        {
            if (offset < endOffset && offset > -1 && endOffset < text.Length)
                for (int i = offset + 1; i < endOffset; i++)
                {
                    if (text[i] == '\n') return i - 1;
                }

            return -1;
        }

        /// <summary>
        /// 新建 region、endregion 标签的代码折叠对象
        /// TODO:亟待重构(Tony)
        /// </summary>
        /// <param name="newFoldings">折叠对象列表</param>
        /// <param name="text">原始文本</param>
        private void CreateRegionFoldings(List<NewFolding> newFoldings, string text)
        {
            Stack<int> stack = new Stack<int>();
            Regex regionRegex = new Regex(string.Format(functionMatchStr, "#region"), RegexOptions.IgnoreCase);
            Regex endRegionRegex = new Regex(string.Format(endFunctionMatchStr, "#endregion"), RegexOptions.IgnoreCase);
            var matches = regionRegex.Matches(text);
            var endMatches = endRegionRegex.Matches(text);
            int endLength = 11;
            int i = 0;
            for (int j = 0; j < matches.Count; j++)
            {
                stack.Push(matches[j].Index);
                if (endMatches.Count == 0) continue;
                if (j + 1 < matches.Count)
                {
                    while (endMatches[i].Index < matches[j + 1].Index)
                    {
                        if (i < endMatches.Count)
                        {
                            var offset = stack.Pop();
                            if (offset != -1 && offset < endMatches[i].Index + endLength - 1)
                                newFoldings.Add(new NewFolding(offset, endMatches[i].Index + endLength - 1)
                                    {Name = GetRegionExplanation(offset, text)});
                            else
                            {
                                stack.Push(offset);
                            }

                            i++;
                            if (i >= endMatches.Count)
                                break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    if (i < endMatches.Count)
                    {
                        var offset = stack.Pop();
                        if (offset != -1 && offset < endMatches[i].Index + endLength)
                            newFoldings.Add(new NewFolding(offset, endMatches[i].Index + endLength - 1)
                                {Name = GetRegionExplanation(offset, text)});
                        i++;
                    }
                }
            }

            for (int j = i; j < endMatches.Count; j++)
            {
                if (stack.Count <= 0) break;
                var offset = stack.Pop();
                if (offset != -1 && offset < endMatches[i].Index + endLength - 1)
                    newFoldings.Add(new NewFolding(offset, endMatches[j].Index + endLength - 1)
                        {Name = GetRegionExplanation(offset, text)});
            }
        }

        private string GetRegionExplanation(int offset, string text)
        {
            string explanation = "";
            bool flag = false;
            for (int i = offset + 7; i < text.Length; i++)
            {
                if (text[i] == '\n') break;
                if (flag)
                {
                    explanation = explanation + text[i];
                }
                else
                {
                    if (text[i] == ' ' || text[i] == '\t')
                    {
                        flag = true;
                    }
                }
            }

            if (explanation.Replace(" ", "").Replace("\r", "").Replace("\t", "").Replace("\n", "") == "")
                explanation = "#region";
            return explanation;
        }
    }
}