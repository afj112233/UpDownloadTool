using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.PredefinedType;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.AddOnInstruction.Panel.Validate
{
    public class DataTypeValidate : ValidationRule
    {
        private bool _result;
        private bool _isRepeatShow = false;
        public DataTypeValidateParam DataTypeValidateParam { set; get; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (DataTypeValidateParam == null)
            {
                return new ValidationResult(false, "");
            }
            else
            {
                var parameterRowInfo = DataTypeValidateParam.RowInfo;
                if (parameterRowInfo != null)
                {
                    ParameterDataTypeCheck(parameterRowInfo, (string) value, DataTypeValidateParam.ParameterViewModel);
                }

                var localRowInfo = DataTypeValidateParam.LocalInfo;
                if (localRowInfo != null)
                {
                    LocalDataTypeCheck(localRowInfo, (string) value, DataTypeValidateParam.LocalTagsViewModel);
                }

                if (_result)
                    return new ValidationResult(true, "");
                else
                    return new ValidationResult(false, "");
            }
        }

        public const string ParameterDataTypeRegex =
            @"^[ ]*[A-Za-z]*((_[a-zA-Z0-9]+)*|[a-zA-Z0-9_]*)(\[[0-9 ]+(,[0-9 ]+){0,2}\])?[ ]*$";

        public const string LocalDataTypeRegex = @"^[ ]*[A-Za-z]((_[a-zA-Z0-9]+)*|[a-zA-Z0-9_]*)(\[[0-9 ]+\])?[ ]*$";

        private void ParameterDataTypeCheck(ParametersRow parametersRow, string dataType,
            ParametersViewModel parametersViewModel)
        {
            Usage? usage = parametersRow?.Usage;

            string name = parametersRow?.Name;

            int flag = 0;
            if (!string.IsNullOrEmpty(dataType) && dataType?.IndexOf('[') > 0)
            {
                if (!(usage == null || usage == Usage.InOut))
                    flag = 4;
                else
                {
                    string dim = dataType.Replace(dataType.Substring(0, dataType.IndexOf('[')), "").Replace("[", "")
                        .Replace("]", "");
                    Regex regex2 = new Regex(ParameterDataTypeRegex);

                    if (string.IsNullOrEmpty(dim) || !regex2.IsMatch(dataType) ||
                        (dataType.Substring(0, dataType.IndexOf('['))
                             .Equals("BOOL", StringComparison.OrdinalIgnoreCase) &&
                         dim.Split(',').Length > 1)) flag = 5;
                }

                dataType = dataType.Substring(0, dataType.IndexOf('['));
            }

            if (string.IsNullOrEmpty(dataType)) flag = 1;
            if (flag == 0 && parametersRow?.Controller.DataTypes[dataType] == null) flag = 2;
            if (flag == 0 && !(dataType.Equals("BOOL", StringComparison.OrdinalIgnoreCase) ||
                               dataType.Equals("SINT", StringComparison.OrdinalIgnoreCase) ||
                               dataType.Equals("INT", StringComparison.OrdinalIgnoreCase) ||
                               dataType.Equals("DINT", StringComparison.OrdinalIgnoreCase) ||
                               dataType.Equals("REAL", StringComparison.OrdinalIgnoreCase) ||
                               dataType.Equals("LINT", StringComparison.OrdinalIgnoreCase)))
                if (!(usage == null || usage == Usage.InOut))
                    flag = 3;
            if (flag == 0)
            {
                if (!IsMemberContainComponent(parametersRow?.ParentAddOnInstruction.Name,
                    parametersRow?.Controller.DataTypes[dataType] as CompositiveType))
                    flag = 6;
            }

            //TODO(clx): 暂时不支持ULINT/USINT/UINT/UDINT/LREAL/LINT
            if (flag == 0)
            {
                string[] unsupportedTypes =
                {
                    LREAL.Inst.Name,
                    ULINT.Inst.Name,
                    UDINT.Inst.Name,
                    UINT.Inst.Name,
                    USINT.Inst.Name,
                    LINT.Inst.Name
                };
                if (unsupportedTypes.Contains(dataType, StringComparer.OrdinalIgnoreCase))
                {
                    flag = 7;
                }
            }

            if (flag > 0)
            {
                _result = false;
                string reason = $"{LanguageManager.GetInstance().ConvertSpecifier("Failed to set parameter")}" +
                                $" \"{name}\" " +
                                $"{LanguageManager.GetInstance().ConvertSpecifier("default value.")}";
                string message = LanguageManager.GetInstance().ConvertSpecifier("Input or output parameter must be BOOL,SINT,INT,DINT, or REAL data type.");
                if (flag == 1) message = LanguageManager.GetInstance().ConvertSpecifier("Name is invalid.");
                else if (flag == 2) message = LanguageManager.GetInstance().ConvertSpecifier("Data type could not be found.");
                else if (flag == 4)
                    message = LanguageManager.GetInstance().ConvertSpecifier(
                        "Invalid array.Input or output parameter must be BOOL,SINT,INT,DINT,or REAL with no dimensions.");
                else if (flag == 5)
                    message = LanguageManager.GetInstance().ConvertSpecifier(
                        "Number ,size,or format of dimensions specified for this tag or type is invalid.");
                else if (flag == 6) message = LanguageManager.GetInstance().ConvertSpecifier(
                       "Tag cannot be created with data type that directly or indirectly references itself.Recursion is not allowed.");
                else if (flag == 7)
                    message = $"{LanguageManager.GetInstance().ConvertSpecifier("The")}"+
                              $" {dataType} "+
                              $"{LanguageManager.GetInstance().ConvertSpecifier("data type is not supported by this controller type.")}";
                parametersRow.ErrorMessage = message;
                parametersRow.ErrorReason = reason;
                parametersRow.IsCorrect = false;
                parametersViewModel.HasReport = true;
                //ShowWarning(reason, message);
                return;
            }

            parametersRow.ErrorMessage = null;
            parametersRow.ErrorReason = null;
            parametersRow.IsCorrect = true;
            _result = true;
        }

        private void LocalDataTypeCheck(LocalTagRow localTagRow, string dataType, LocalTagsViewModel localTagsViewModel)
        {
            string name = localTagRow?.Name;

            int flag = 0;
            if (!string.IsNullOrEmpty(dataType) && dataType?.IndexOf('[') > 0)
            {
                Regex regex2 = new Regex(LocalDataTypeRegex);
                if (!regex2.IsMatch(dataType)) flag = 5;
                dataType = dataType.Substring(0, dataType.IndexOf('['));
                if (dataType.IndexOf(",") > 0) flag = 4;
            }

            if (flag < 4)
            {
                if (string.IsNullOrEmpty(dataType)) flag = 1;
                if (flag == 0 && localTagRow?.Controller.DataTypes[dataType] == null) flag = 2;
            }

            if (flag == 0 && !AOIExtend.VerifyLocalDataType(dataType))
            {
                flag = 6;
            }

            if (flag == 0)
            {
                if (!IsMemberContainComponent(localTagRow?.ParentAddOnInstruction.Name,
                    localTagRow?.Controller.DataTypes[dataType] as CompositiveType))
                    flag = 3;
            }

            //TODO(clx): 暂时不支持ULINT/USINT/UINT/UDINT/LREAL/LINT
            if (flag == 0)
            {
                string[] unsupportedTypes =
                {
                    LREAL.Inst.Name,
                    ULINT.Inst.Name,
                    UDINT.Inst.Name,
                    UINT.Inst.Name,
                    USINT.Inst.Name,
                    LINT.Inst.Name
                };
                if (unsupportedTypes.Contains(dataType, StringComparer.OrdinalIgnoreCase))
                {
                    flag = 7;
                }
            }

            if (flag > 0)
            {
                string reason = $"Failed to set tag \"{name}\" data type.";
                string message =
                    "Tag cannot be created with data type that directly or indirectly references itself.Recursion is not allowed.";
                if (flag == 1) message = "Name is invalid.";
                else if (flag == 2) message = "Data type could not be found.";
                else if (flag == 4)
                    message =
                        "Invalid array.Input or output parameter must be BOOL,SINT,INT,DINT,or REAL with no dimensions.";
                else if (flag == 5)
                    message = "Number ,size,or format of dimensions specified for this tag or type is invalid.";
                else if (flag == 6)
                    message = "Tag can only be created as InOut parameter in Add-On Instruction.";
                else if (flag == 7)
                    message = $"The {dataType} data type is not supported by this controller type.";
                localTagRow.ErrorMessage = message;
                localTagRow.ErrorReason = reason;
                localTagRow.IsCorrect = false;
                localTagsViewModel.HasReport = true;
                //ShowWarning(reason, message);
                _result = false;
                return;
            }

            localTagRow.ErrorMessage = null;
            localTagRow.ErrorReason = null;
            localTagRow.IsCorrect = true;
            _result = true;
        }

        private void ShowWarning(string reason, string message)
        {
            if (!_isRepeatShow)
                ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    _isRepeatShow = true;

                    var dialog =
                        new WarningDialog(reason, message) {Owner = Application.Current.MainWindow};
                    dialog.ShowDialog();
                });
            else
            {
                _isRepeatShow = false;
            }

        }

        private bool IsMemberContainComponent(string dataType, CompositiveType compositiveType)
        {
            if (compositiveType == null)
            {
                return true;
            }

            bool flag = true;
            if (compositiveType.Name == dataType)
            {
                return false;
            }

            foreach (var item in compositiveType.TypeMembers)
            {

                if ((item as DataTypeMember).DisplayName == dataType)
                {
                    return false;
                }

                if ((item.DataTypeInfo.DataType as UserDefinedDataType) != null)
                {
                    flag = IsMemberContainComponent(dataType, (item.DataTypeInfo.DataType as CompositiveType));
                }

                if (!flag)
                {

                    return false;
                }
            }

            return true;
        }
        
    }
}
