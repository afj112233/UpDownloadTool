using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Interfaces.DataType;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DataType;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.AddOnInstruction.Panel.Validate
{
    class DefaultValidate : ValidationRule
    {
        private bool _result;
        private bool _isRepeatShow;
        public DataTypeValidateParam DataTypeValidateParam { set; get; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (DataTypeValidateParam == null) return new ValidationResult(false, "");
            else
            {

                if (DataTypeValidateParam.LocalTagsViewModel != null)
                {
                    //LocalDefaultCheck(DataTypeValidateParam.LocalInfo, (string)value,
                    //    DataTypeValidateParam.LocalTagsViewModel);
                    CheckDefault(DataTypeValidateParam.LocalInfo, (string)value, DataTypeValidateParam.LocalTagsViewModel);
                }
                else
                {
                    //DefaultCheck(DataTypeValidateParam.RowInfo, (string)value,
                    //    DataTypeValidateParam.ParameterViewModel);
                    CheckDefault(DataTypeValidateParam.RowInfo, (string)value, DataTypeValidateParam.ParameterViewModel);
                }

                if (_result) return new ValidationResult(true, "");
            }

            return new ValidationResult(false, "");
        }

        private void CheckDefault<T>(BaseTagRow baseTagRow, string defaultValue,
            T viewModel) where T: IOptionPanel
        {
            string dataType = baseTagRow?.DataType;
            string name = baseTagRow?.Name;
            DisplayStyle? style = baseTagRow?.Style;

            int flag = 0;
            if (string.IsNullOrEmpty(defaultValue)) flag = 1;
            if (flag < 1)
            {
                defaultValue = FormatOp.RemoveFormat(defaultValue);
                if (dataType != null)
                {
                    if (dataType.Equals("SINT", StringComparison.OrdinalIgnoreCase))
                    {
                        ValueConverter.CheckValueOverflow(style, defaultValue, ref flag, 127);
                    }
                    else if (dataType.Equals("INT", StringComparison.OrdinalIgnoreCase))
                    {
                        ValueConverter.CheckValueOverflow(style, defaultValue, ref flag, 32767);
                    }
                    else if (dataType.Equals("DINT", StringComparison.OrdinalIgnoreCase))
                    {
                        ValueConverter.CheckValueOverflow(style, defaultValue, ref flag, Int32.MaxValue);
                    }
                    else if (dataType.Equals("LINT", StringComparison.OrdinalIgnoreCase))
                    {
                        ValueConverter.CheckValueOverflow(style, defaultValue, ref flag, Int64.MaxValue);
                    }
                    else if (dataType.Equals("BOOL", StringComparison.OrdinalIgnoreCase))
                    {
                        if (defaultValue != "0" && defaultValue != "1") flag = 3;
                    }
                    else if (dataType.Equals("REAL", StringComparison.OrdinalIgnoreCase))
                    {
                        if (FormatOp.IsPositiveInfinity(defaultValue) || FormatOp.IsNegativeInfinity(defaultValue))
                        {
                            _result = true;
                            return;
                        }
                        Regex regex = new Regex(@"^[0-9]+(\.[0-9]*(((E|e)(\+))?)[0-9]*)?$");
                        if (flag == 0 && !regex.IsMatch(defaultValue)) flag = 2;
                    }
                }
            }
            var parameterViewModel = viewModel as ParametersViewModel;
            var localViewModel = viewModel as LocalTagsViewModel;
            if (flag > 0)
            {
                string reason = $"Failed to set tag \"{name}\" default value.";
                string message = LanguageManager.GetInstance().ConvertSpecifier("Signed value overflow.");
                if (flag == 1 || flag == 2)
                    message = LanguageManager.GetInstance().ConvertSpecifier("String invalid.");
              
                if(parameterViewModel!=null)
                {
                    parameterViewModel.ErrorMessage = message;
                    parameterViewModel.ErrorReason = reason;
                    parameterViewModel.HasReport = true;
                }

                if (localViewModel != null)
                {
                    localViewModel.ErrorMessage = message;
                    localViewModel.ErrorReason = reason;
                    localViewModel.HasReport = true;
                }
                ShowWarning(reason, message);
                _result = false;
                return;
            }
            if (parameterViewModel != null)
            {
                parameterViewModel.ErrorMessage = null;
                parameterViewModel.ErrorReason = null;
            }
            if (localViewModel != null)
            {
                localViewModel.ErrorMessage = null;
                localViewModel.ErrorReason = null;
            }
            _result = true;
        }

        //private void DefaultCheck(ParametersRow parametersRow, string defaultValue,
        //    ParametersViewModel parametersViewModel)
        //{
        //    string dataType = parametersRow?.DataType;
        //    string name = parametersRow?.Name;
        //    DisplayStyle? style = parametersRow?.Style;

        //    int flag = 0;
        //    if (string.IsNullOrEmpty(defaultValue)) flag = 1;
        //    if (flag < 1)
        //    {
        //        defaultValue = FormatOp.RemoveFormat(defaultValue);
        //        if (dataType != null)
        //        {
        //            if (dataType.Equals("SINT", StringComparison.OrdinalIgnoreCase))
        //            {
        //                Check.CheckValueOverflow(style, defaultValue, ref flag, 127);
        //            }
        //            else if (dataType.Equals("INT", StringComparison.OrdinalIgnoreCase))
        //            {
        //                Check.CheckValueOverflow(style, defaultValue, ref flag, 32767);
        //            }
        //            else if (dataType.Equals("DINT", StringComparison.OrdinalIgnoreCase))
        //            {
        //                Check.CheckValueOverflow(style, defaultValue, ref flag, Int32.MaxValue);
        //            }
        //            else if (dataType.Equals("LINT", StringComparison.OrdinalIgnoreCase))
        //            {
        //                Check.CheckValueOverflow(style, defaultValue, ref flag, Int64.MaxValue);
        //            }
        //            else if (dataType.Equals("BOOL", StringComparison.OrdinalIgnoreCase))
        //            {
        //                if (defaultValue != "0" && defaultValue != "1") flag = 3;
        //            }
        //            else if (dataType.Equals("REAL", StringComparison.OrdinalIgnoreCase))
        //            {
        //                if (FormatOp.IsPositiveInfinity(defaultValue) || FormatOp.IsNegativeInfinity(defaultValue))
        //                {
        //                    _result = true;
        //                    return;
        //                }
        //                Regex regex = new Regex(@"^[0-9]+.[0-9]*(((E|e)(\+))?)[0-9]*$");
        //                if (flag == 0 && !regex.IsMatch(defaultValue)) flag = 2;
        //            }
        //        }
        //    }

        //    if (flag > 0)
        //    {
        //        string reason = $"Failed to set tag \"{name}\" default value.";
        //        string message = "Signed value overflow.";
        //        if (flag == 1 || flag == 2) message = "String invalid.";
        //        parametersViewModel.ErrorMessage = message;
        //        parametersViewModel.ErrorReason = reason;
        //        parametersViewModel.HasReport = true;
        //        ShowWarning(reason, message);
        //        _result = false;
        //        return;
        //    }

        //    parametersViewModel.ErrorMessage = null;
        //    parametersViewModel.ErrorReason = null;

        //    _result = true;
        //}

        //private void LocalDefaultCheck(LocalTagRow localTagRow, string defaultValue,
        //    LocalTagsViewModel localTagsViewModel)
        //{
        //    string dataType = localTagRow?.DataType;
        //    string name = localTagRow?.Name;
        //    DisplayStyle? style = localTagRow?.Style;

        //    int flag = 0;
        //    if (string.IsNullOrEmpty(defaultValue)) flag = 1;
        //    if (flag < 1)
        //    {
        //        if (dataType != null)
        //        {
        //            defaultValue = FormatOp.RemoveFormat(defaultValue);
        //            if (dataType.Equals("SINT", StringComparison.OrdinalIgnoreCase))
        //            {
        //                Check.CheckValueOverflow(style, defaultValue, ref flag, 127);
        //            }
        //            else if (dataType.Equals("INT", StringComparison.OrdinalIgnoreCase))
        //            {
        //                Check.CheckValueOverflow(style, defaultValue, ref flag, 32767);
        //            }
        //            else if (dataType.Equals("DINT", StringComparison.OrdinalIgnoreCase))
        //            {
        //                Check.CheckValueOverflow(style, defaultValue, ref flag, Int32.MaxValue);
        //            }
        //            else if (dataType.Equals("LINT", StringComparison.OrdinalIgnoreCase))
        //            {
        //                Check.CheckValueOverflow(style, defaultValue, ref flag, Int64.MaxValue);
        //            }
        //            else if (dataType.Equals("BOOL", StringComparison.OrdinalIgnoreCase))
        //            {
        //                if (defaultValue != "0" && defaultValue != "1") flag = 3;
        //            }
        //            else if (dataType.Equals("REAL", StringComparison.OrdinalIgnoreCase))
        //            {
        //                if (FormatOp.IsPositiveInfinity(defaultValue)||FormatOp.IsNegativeInfinity(defaultValue))
        //                {
        //                    _result = true;
        //                    return;
        //                }
        //                Regex regex = new Regex(@"^[0-9]+(\.[0-9]*(((E|e)(\+))?)[0-9]*)?$");
        //                if (flag == 0 && !regex.IsMatch(defaultValue)) flag = 2;
        //            }
        //        }
        //    }

        //    if (flag > 0)
        //    {
        //        string reason = $"Failed to set tag \"{name}\" default value.";
        //        string message = "Signed value overflow.";
        //        if (flag == 1 || flag == 2) message = "String invalid.";
        //        localTagsViewModel.ErrorMessage = message;
        //        localTagsViewModel.ErrorReason = reason;
        //        localTagsViewModel.HasReport = true;
        //        ShowWarning(reason, message);
        //        _result = false;
        //        return;
        //    }

        //    localTagsViewModel.ErrorMessage = null;
        //    localTagsViewModel.ErrorReason = null;

        //    _result = true;
        //}

        private void ShowWarning(string reason, string message)
        {
            if (!_isRepeatShow)
                ThreadHelper.JoinableTaskFactory.Run(async delegate
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
    }
}
