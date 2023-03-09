using System;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace ICSStudio.Utils
{
    public static class EnumUtils
    {
        public static bool TryParse<T>(string value, out T result) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("Type provided must be an Enum.", nameof(T));
            }

            result = default(T);
            if (Enum.TryParse(value, true, out result))
                return true;

            foreach (var fieldInfo in typeof(T).GetFields())
            {
                var attribute = fieldInfo.GetCustomAttribute(typeof(EnumMemberAttribute), true) as EnumMemberAttribute;
                if (attribute != null)
                {
                    if (attribute.Value.Equals(value, StringComparison.OrdinalIgnoreCase))
                    {
                        result = (T) fieldInfo.GetValue(null);
                        return true;
                    }
                }
            }

            // remove " "
            value = value.Replace(" ", "");
            if (Enum.TryParse(value, out result))
                return true;

            return false;
        }

        public static T Parse<T>(string value) where T : struct
        {
            T result;
            if (TryParse(value, out result))
                return result;

            throw new ApplicationException($"Parse {typeof(T).Name}:{value} failed!");
        }

        public static bool IsValidName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            if (name.Length > 40 || name.EndsWith("_") ||
                name.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
            {
                return false;
            }


            Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
            if (!regex.IsMatch(name))
            {

                return false;
            }

            // key word
            string[] keyWords =
            {
                "goto",
                "repeat", "until", "or", "end_repeat",
                "return", "exit",
                "if", "then", "elsif", "else", "end_if",
                "case", "of", "end_case",
                "for", "to", "by", "do", "end_for",
                "while", "end_while",
                "not", "mod", "and", "xor", "or"
            };
            foreach (var keyWord in keyWords)
            {
                if (keyWord.Equals(name, StringComparison.OrdinalIgnoreCase))
                {

                    return false;
                }
            }



            return true;
        }
    }
}
