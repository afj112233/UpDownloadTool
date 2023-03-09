using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ICSStudio.Interfaces.Common;
using ICSStudio.QuickWatchPackage.View.Models;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.Tags;

namespace ICSStudio.QuickWatchPackage.View.UI
{
    internal class TagNameValidationRule : ValidationRule
    {
        private bool _isCorrect = true;
        public ValidationParam Param { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return ValidateName(value?.ToString(), Param.TagItem as MonitorTagItem, Param.DataContent.ParentModel,ref _isCorrect);
        }

        public static ValidationResult ValidateName(string tagName,MonitorTagItem tagItem,QuickWatchViewModel vm ,ref bool isCorrect)
        {
            if (!string.IsNullOrEmpty(tagName))
            {
                if (string.IsNullOrEmpty(tagName)) return new ValidationResult(false, "Tag name is empty.");

                var regex = new Regex(@"^[a-zA-Z0-9_\:\-\.\[\]]*$");
                if (!regex.IsMatch(tagName))
                {
                    isCorrect = false;
                    return new ValidationResult(false, "Error tag name");
                }

                var endRegex = new Regex(@"^[a-zA-Z0-9\]]*$");
                if (!endRegex.IsMatch(tagName[tagName.Length - 1].ToString()))
                {
                    MessageBox.Show("Failed to add tag to Watch Pane.\nInvalid expression or tag.", "ICS Studio",
                        MessageBoxButton.OK, MessageBoxImage.Asterisk);

                    isCorrect = false;
                    return new ValidationResult(false, "Error tag name");
                }

                ASTName tag = ObtainValue.GetLoadTag(tagName, tagItem?.ParentScope, null);
                IProgramModule program = tagItem.ParentScope;
                if (tagName.StartsWith("\\"))
                {
                    program = Controller.GetInstance().Programs[tagName.Substring(1,tagName.IndexOf("."))];
                }
                else
                {
                    if (tag == null)
                    {
                        var reachableScope = vm.GetReachableScope();

                        foreach (var scope in reachableScope)
                        {
                            if (tagItem.ParentScope == scope) continue;
                            tag = ObtainValue.GetLoadTag(tagName, scope, null);
                            if (tag != null)
                            {
                                program = scope;
                                break;
                            }
                        }
                    }
                }

                if (tag != null && !string.IsNullOrEmpty(tag.Error))
                {
                    tag = null;
                    isCorrect = true;
                }

                if (tag == null)
                {
                    if (isCorrect)
                    {
                        MessageBox.Show("Failed to add tag to Watch Pane.\nReferenced tag is undefined.", "ICS Studio",
                            MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }

                    isCorrect = false;
                    return new ValidationResult(false, "Error tag name");
                }

                var collection = tagItem?.ParentCollection as MonitorTagItemCollection;
                Debug.Assert(collection != null);
                //TODO(zyl):可以添加不同域同名
                if (collection?.AsParallel()
                        .Any(item =>
                            (item != tagItem) &&
                            tagName.Equals(item?.Name, StringComparison.OrdinalIgnoreCase) && item.ParentScope == program) ?? false)
                {
                    if (isCorrect)
                    {
                        MessageBox.Show("Failed to add tag to Watch Pane.\nAlready exists.", "ICS Studio",
                            MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }

                    isCorrect = false;
                    return new ValidationResult(false, "Error tag name");
                }

                tagItem.ParentScope = program;
            }

            isCorrect = true;
            return ValidationResult.ValidResult;
        }
    }
}