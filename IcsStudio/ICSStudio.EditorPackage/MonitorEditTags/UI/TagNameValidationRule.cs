using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.MultiLanguage;

namespace ICSStudio.EditorPackage.MonitorEditTags.UI
{
    internal class TagNameValidationRule : ValidationRule
    {
        public ValidationParam Param { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value != null)
            {
                var tagName = value.ToString();

                if (string.IsNullOrEmpty(tagName))
                {
                    return new ValidationResult(false, "Tag name is empty.");
                }

                if (tagName.Length > 40 || tagName.EndsWith("_") ||
                    tagName.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    return new ValidationResult(false, "Tag name is invalid.");
                }

                var regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
                if (!regex.IsMatch(tagName))
                {
                    return new ValidationResult(false, LanguageManager.GetInstance().ConvertSpecifier("VariableNameInvalid"));
                }

                var tagItem = Param?.TagItem;
                var scope = Param?.Scope;

                Contract.Assert(tagItem != null);
                Contract.Assert(scope != null);

                if (tagItem.Tag == null ||
                    !string.Equals(tagItem.Tag.Name, tagName, StringComparison.OrdinalIgnoreCase))
                {
                    var tag = scope.Tags[tagName];
                    if (tag != null)
                    {
                        return new ValidationResult(false, "Already exists.");
                    }
                }

                string[] keyWords =
                {
                    "goto",
                    "repeat", "until", "or", "end_repeat",
                    "return", "exit",
                    "if", "then", "elsif", "else", "end_if",
                    "case", "of", "end_case",
                    "for", "to", "by", "do", "end_for",
                    "while", "end_while",
                    "not", "mod", "and", "xor", "or",
                    "ABS", "SQRT",
                    "LOG", "LN",
                    "DEG", "RAD", "TRN",
                    "ACS", "ASN", "ATN", "COS", "SIN", "TAN"
                };
                foreach (var keyWord in keyWords)
                {
                    if (keyWord.Equals(tagName, StringComparison.OrdinalIgnoreCase))
                    {
                        return new ValidationResult(false, LanguageManager.GetInstance().ConvertSpecifier("VariableNameInvalid"));
                    }
                }

                //TODO(gjc): add code here
            }

            return ValidationResult.ValidResult;
        }
    }
}