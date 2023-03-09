using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Antlr4.Runtime.Misc;

namespace ICSStudio.SimpleServices.Common
{
    public class RoutineCodeTextExtension
    {
        public static string ConvertCommentToWhiteBlank(string code, List<Tuple<int, int, string>> commentList,
            bool isFolding = false, bool throwError = false,bool removeRegion=true)
        {
            if(removeRegion)
                code = RemoveRegion(code);
            Regex regex = new Regex(@"//|/\*|\(\*");
            while (true)
            {
                var match = regex.Match(code);
                if (match.Value == String.Empty) break;
                else if (match.Value == "//")
                {
                    code = RemoveComment2(code, match.Index, commentList, isFolding);
                }
                else
                {
                    code = RemoveComment1(code, match.Index, match.Value, commentList, throwError);
                }
            }

            return code;
        }

        public static string ConvertStatementToWhiteBlank(string code)
        {
            var commentList=new List<Tuple<int, int, string>>();
            var cleanCode=ConvertCommentToWhiteBlank(code, commentList,true);
            var newCode = "";
            for (int i = 0; i < cleanCode.Length; i++)
            {
                newCode = $"{newCode}{(char.IsWhiteSpace(cleanCode[i]) ? code[i] : ' ')}";
            }
            return newCode;
        }

        public static string RemoveRegion(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            Regex regex = new Regex(@"(^|\n)(\s)*(#region|#endregion).*", RegexOptions.IgnoreCase);
            var matches = regex.Matches(text);
            foreach (Match match in matches)
            {
                var blank = GetBlank(match.Length);
                text = text.Remove(match.Index, match.Length).Insert(match.Index, blank);
            }
            return text;
        }

        public static string RemoveComment1([NotNull] string code, int p, string commentSignal, ref int p2)
        {
            var endSignal = commentSignal == "(*" ? "*)" : "*/";
            p2 = code.IndexOf(endSignal, p + 2) + 1;
            if (p2 == 0)
            {
                p2 = code.Length - 1;
            }

            int length = p2 - p + 1;

            code = code.Remove(p, length);
            string space = "";
            while (length > 0)
            {
                space = space + " ";
                length--;
            }

            return code.Insert(p, space);
        }

        public static string RemoveComment2(string code, int p, ref int p2)
        {
            p2 = code.IndexOf("\n", p);
            if (p2 == -1) p2 = code.Length - 1;
            int length = p2 - p;

            code = code.Remove(p, length);
            string space = "";
            while (length > 0)
            {
                space = space + " ";
                length--;
            }

            return code.Insert(p, space);
        }

        private static string GetBlank(int length)
        {
            string blank = "";
            if (length > -1)
            {
                while (length > 0)
                {
                    blank = blank + " ";
                    length--;
                }
            }

            return blank;
        }

        public class RemoveCommentException: Exception
        {
            public RemoveCommentException(string error, int offset)
            {
                Error = error;
                Offset = offset;
            }
            public string Error { get; }

            public int Offset { get; }
        }

        private static string RemoveComment1([NotNull] string code, int p, string commentSignal,
            List<Tuple<int, int,string>> commentList, bool throwError)
        {
            var endSignal = commentSignal == "(*" ? "*)" : "*/";

            var regex = new Regex($"{string.Join("", commentSignal.Select(c => $"\\{c}"))}(.|\\n)*?{string.Join("", endSignal.Select(c => $"\\{c}"))}");
            var match = regex.Match(code);

            int p2 = match.Index + match.Length;
            if (!match.Success)
            {
                if (throwError)
                    throw new RemoveCommentException($"'{endSignal}':Missing end of comment.", p);
                p2 = code.Length - 1;
            }

            commentList?.Add(new Tuple<int, int, string>(p, p2, commentSignal));
            if (!match.Success)
            {
                commentList?.Add(new Tuple<int, int, string>(p, -1, commentSignal));
            }
            if (p2 == -1) return code;

            int length = match.Success ? match.Length : Math.Max(0, p2 - p + 1);
            code = code.Remove(p, length);
            string space = "";
            while (length > 0)
            {
                space = space + " ";
                length--;
            }

            return code.Insert(p, space);
        }

        private static string RemoveComment2(string code, int p, List<Tuple<int, int,string>> commentList, bool isFolding)
        {
            int p2 = code.IndexOf("\n", p);
            if (p2 == -1) p2 = code.Length - 1;

            int length = p2 - p + (isFolding ? 0 : 1);
            commentList?.Add(new Tuple<int, int,string>(p, p2,"//"));
            code = code.Remove(p, length);
            string space = "";
            while (length > 0)
            {
                space = space + " ";
                length--;
            }

            return code.Insert(p, space);
        }

    }
}
