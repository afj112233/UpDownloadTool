using System;
using System.Text.RegularExpressions;
using System.Windows;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;

namespace ICSStudio.UIServicesPackage
{
    public class Checking
    {
        public enum CheckModeType
        {
            CreateTagName,
            ChangeTagName,

            //ChangeModuleName
        }

        public static bool CheckTagName(
            string tagName,
            IController controller,
            CheckModeType checkModeType = CheckModeType.CreateTagName)
        {
            string warningMessage;
            string warningReason = string.Empty;
            bool isValid = true;

            switch (checkModeType)
            {
                case CheckModeType.CreateTagName:
                    warningMessage = "Failed to create a new tag.";
                    break;
                case CheckModeType.ChangeTagName:
                    warningMessage = "Failed to change tag name.";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(checkModeType), checkModeType, null);
            }

            if (string.IsNullOrEmpty(tagName))
            {
                isValid = false;
                warningReason = "Tag name is empty.";
            }

            if (isValid)
            {
                if (tagName.Length > 40 || tagName.EndsWith("_") ||
                    tagName.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    isValid = false;
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("VariableNameInvalid");
                }
            }

            if (isValid)
            {
                Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
                if (!regex.IsMatch(tagName))
                {
                    isValid = false;
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("VariableNameInvalid");
                }
            }

            // key word
            if (isValid)
            {
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
                    if (keyWord.Equals(tagName, StringComparison.OrdinalIgnoreCase))
                    {
                        isValid = false;
                        warningReason = LanguageManager.GetInstance().ConvertSpecifier("VariableNameInvalid");
                    }
                }
            }

            if (isValid && controller != null)
            {
                var tag = controller.Tags[tagName];
                if (tag != null)
                {
                    isValid = false;
                    warningReason = "Already exists.";
                }
            }

            if (!isValid)
            {
                var warningDialog = new WarningDialog(warningMessage, warningReason)
                    {Owner = Application.Current.MainWindow};
                warningDialog.ShowDialog();
            }

            return isValid;
        }
    }
}
