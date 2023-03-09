using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ICSStudio.Utils.TagExpression
{
    public class TagExpressionParser
    {
        public TagExpressionBase Parser(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            int lastIndex = input.Length - 1;

            SimpleTagExpression simpleTagExpression = TryParserSimpleTagExpression(input);
            TagExpressionBase target = simpleTagExpression;

            while (target != null && target.EndOffset < lastIndex - 1)
            {
                int offset = target.EndOffset + 1;
                char nextChar = input[offset];
                char nextNextChar = input[offset + 1];

                if (nextChar == '.')
                {
                    if (nextNextChar == '[')
                    {
                        target = TryParserBitMemberExpressionAccessExpression(input, offset, target);
                        break;
                    }

                    if (char.IsDigit(nextNextChar))
                    {
                        target = TryParserBitMemberNumberAccessExpression(input, offset, target);
                        break;
                    }

                    target = TryParserMemberAccessExpression(input, offset, target);

                }
                else if (nextChar == '[')
                {
                    //TODO(gjc): need edit later for array[tag,tag,tag]
                    var nextTarget = TryParserElementAccessExpression(input, offset, target);
                    if (nextTarget == target)
                        break;

                    target = nextTarget;
                }
                else
                {
                    return null;
                }

            }

            return target;
        }

        private TagExpressionBase TryParserElementAccessExpression(string input, int offset, TagExpressionBase target)
        {
            int closeSquareBraceIndex = input.IndexOf(']', offset);
            if (closeSquareBraceIndex < 0)
                return target;

            string indexString = input.Substring(offset, closeSquareBraceIndex - offset + 1);

            MatchCollection matchCollection = Regex.Matches(indexString, "(?<=\\W+)\\d+",
                RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

            List<int> indexes = new List<int>();
            int index1;
            int index2;
            int index3;

            List<SimpleTagExpression> expressionIndexes = null;

            switch (matchCollection.Count)
            {

                case 1:
                    if (int.TryParse(matchCollection[0].Value, out index1))
                    {
                        indexes.Add(index1);
                        break;
                    }

                    return target;

                case 2:
                    if (int.TryParse(matchCollection[0].Value, out index2)
                        && int.TryParse(matchCollection[1].Value, out index1))
                    {
                        indexes.Add(index2);
                        indexes.Add(index1);
                        break;
                    }

                    return target;

                case 3:
                    if (int.TryParse(matchCollection[0].Value, out index3)
                        && int.TryParse(matchCollection[1].Value, out index2)
                        && int.TryParse(matchCollection[2].Value, out index1))
                    {
                        indexes.Add(index3);
                        indexes.Add(index2);
                        indexes.Add(index1);
                        break;
                    }

                    return target;

                default:
                    expressionIndexes = TryParseExpressionIndexes(indexString, offset);
                    break;
            }

            //
            ElementAccessExpression next = null;
            if (indexes.Count > 0)
            {
                next = new ElementAccessExpression
                {
                    StartOffset = offset,
                    EndOffset = closeSquareBraceIndex,
                    Indexes = indexes,
                    Target = target
                };
            }
            else if (expressionIndexes != null && expressionIndexes.Count > 0)
            {
                next = new ElementAccessExpression()
                {
                    StartOffset = offset,
                    EndOffset = closeSquareBraceIndex,
                    ExpressionIndexes = expressionIndexes,
                    Target = target
                };
            }

            if (next == null)
                return target;

            target.Next = next;
            return next;

        }

        private List<SimpleTagExpression> TryParseExpressionIndexes(string input, int offset)
        {
            string trimInput = input.TrimStart('[').TrimEnd(']');
            offset += 1;

            string[] expressStrings = trimInput.Split(',');

            List<SimpleTagExpression> expressions = new List<SimpleTagExpression>();

            foreach (var expressString in expressStrings)
            {
                SimpleTagExpression simpleTagExpression = TryParserSimpleTagExpression(expressString);
                if (simpleTagExpression != null)
                {
                    simpleTagExpression.StartOffset += offset;
                    simpleTagExpression.EndOffset += offset;

                    expressions.Add(simpleTagExpression);

                    offset += (expressString.Length+1);
                }
                else
                {
                    return null;
                }
            }

            return expressions.Count > 0 ? expressions : null;
        }

        private TagExpressionBase TryParserMemberAccessExpression(string input, int offset, TagExpressionBase target)
        {
            int dotIndex = input.IndexOf('.', offset + 1);
            int openSquareBraceIndex = input.IndexOf('[', offset + 1);

            string memberName;
            int endOffset;

            if (dotIndex < 0 && openSquareBraceIndex < 0)
            {
                memberName = input.Substring(offset + 1);
                endOffset = input.Length - 1;
            }
            else if (dotIndex > 0 && openSquareBraceIndex < 0)
            {
                memberName = input.Substring(offset + 1, dotIndex - offset - 1);
                endOffset = dotIndex - 1;
            }
            else if (dotIndex < 0 && openSquareBraceIndex > 0)
            {
                memberName = input.Substring(offset + 1, openSquareBraceIndex - offset - 1);
                endOffset = openSquareBraceIndex - 1;
            }
            else
            {
                int minIndex = Math.Min(dotIndex, openSquareBraceIndex);
                memberName = input.Substring(offset + 1, minIndex - offset - 1);
                endOffset = minIndex - 1;
            }

            var next = new MemberAccessExpression()
            {
                StartOffset = offset,
                EndOffset = endOffset,
                Name = memberName,
                Target = target
            };
            target.Next = next;

            return next;
        }

        private TagExpressionBase TryParserBitMemberNumberAccessExpression(
            string input, int offset, TagExpressionBase target)
        {
            string numberString = input.Substring(offset + 1);
            int number;

            if (int.TryParse(numberString, out number))
            {
                var next = new BitMemberNumberAccessExpression()
                {
                    StartOffset = offset,
                    EndOffset = input.Length - 1,
                    Number = number,
                    Target = target
                };
                target.Next = next;
                return next;
            }

            return target;
        }

        private TagExpressionBase TryParserBitMemberExpressionAccessExpression(
            string input, int offset, TagExpressionBase target)
        {
            int closeSquareBraceIndex = input.IndexOf(']', offset);
            if (closeSquareBraceIndex < 0)
                return target;

            string numberString = input.Substring(offset + 2, closeSquareBraceIndex - offset - 2);
            int number;

            if (int.TryParse(numberString, out number))
            {
                var next = new BitMemberExpressionAccessExpression()
                {
                    StartOffset = offset,
                    EndOffset = closeSquareBraceIndex,
                    Number = number,
                    Target = target
                };
                target.Next = next;
                return next;
            }
            else
            {
                SimpleTagExpression simpleTagExpression = TryParserSimpleTagExpression(numberString);
                if (simpleTagExpression != null)
                {
                    simpleTagExpression.StartOffset = offset + 2;
                    simpleTagExpression.EndOffset = closeSquareBraceIndex;

                    var next = new BitMemberExpressionAccessExpression()
                    {
                        StartOffset = offset,
                        EndOffset = closeSquareBraceIndex,
                        ExpressionNumber = simpleTagExpression,
                        Target = target
                    };
                    target.Next = next;
                    return next;
                }
            }

            return target;
        }

        private SimpleTagExpression TryParserSimpleTagExpression(string input)
        {
            int dotIndex = input.IndexOf('.');
            int length = input.Length;

            string scope = string.Empty;
            string tagName;

            int offset = 0;
            int endOffset;

            Regex regex = new Regex($"[\\s\\+\\-\\*/=()]", RegexOptions.IgnoreCase);
            if (regex.IsMatch(input))
                return null;

            if (input[0] == '\\')
            {
                if (dotIndex < 2 && dotIndex >= length - 1)
                    return null;

                scope = input.Substring(1, dotIndex - 1);
                offset = dotIndex + 1;
            }

            dotIndex = input.IndexOf('.', offset);
            int openSquareBraceIndex = input.IndexOf('[', offset);


            if (dotIndex < 0 && openSquareBraceIndex < 0)
            {
                tagName = input.Substring(offset);
                endOffset = length - 1;
            }
            else if (dotIndex > 0 && openSquareBraceIndex < 0)
            {
                tagName = input.Substring(offset, dotIndex - offset);
                endOffset = dotIndex - 1;
            }
            else if (dotIndex < 0 && openSquareBraceIndex > 0)
            {
                tagName = input.Substring(offset, openSquareBraceIndex - offset);
                endOffset = openSquareBraceIndex - 1;
            }
            else
            {
                int minIndex = Math.Min(dotIndex, openSquareBraceIndex);
                if (minIndex - offset <= 0)
                    return null;

                tagName = input.Substring(offset, minIndex - offset);
                endOffset = minIndex - 1;
            }

            return new SimpleTagExpression
            {
                StartOffset = 0, EndOffset = endOffset, Scope = scope, TagName = tagName
            };
        }

        public SimpleTagExpression GetSimpleTagExpression(TagExpressionBase tagExpression)
        {
            var simpleTagExpression = tagExpression as SimpleTagExpression;
            if (simpleTagExpression != null)
                return simpleTagExpression;

            var tagMemberAccessExpression = tagExpression as TagMemberAccessExpressionBase;
            if (tagMemberAccessExpression != null)
                return GetSimpleTagExpression(tagMemberAccessExpression.Target);

            var elementAccessExpression = tagExpression as ElementAccessExpression;
            if (elementAccessExpression != null)
                return GetSimpleTagExpression(elementAccessExpression.Target);

            return null;
        }

        public string GetNextTagExpression(SimpleTagExpression simpleTagExpression)
        {
            string result = string.Empty;

            TagExpressionBase next = simpleTagExpression.Next;

            while (next != null)
            {
                var elementAccessExpression = next as ElementAccessExpression;
                if (elementAccessExpression != null)
                {
                    result += $"[{string.Join(",", elementAccessExpression.Indexes)}]";
                }

                var memberAccessExpression = next as MemberAccessExpression;
                if (memberAccessExpression != null)
                {
                    result += $".{memberAccessExpression.Name}";
                }

                var bitMemberNumberAccessExpression = next as BitMemberNumberAccessExpression;
                if (bitMemberNumberAccessExpression != null)
                {
                    result += $".{bitMemberNumberAccessExpression.Number}";
                }

                var bitMemberExpressionAccessExpression = next as BitMemberExpressionAccessExpression;
                if (bitMemberExpressionAccessExpression != null)
                {
                    result += $".[{bitMemberExpressionAccessExpression.Number}]";
                }

                next = next.Next;
            }

            return result;
        }
    }
}
