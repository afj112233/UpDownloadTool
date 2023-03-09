using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Interfaces.DataType;
using ICSStudio.MultiLanguage;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;
using ICSStudio.Utils.TagExpression;

namespace ICSStudio.Dialogs.ConfigDialogs
{
    public partial class MessageConfigurationViewModel
    {
        private readonly Dictionary<string, List<string>> _errors
            = new Dictionary<string, List<string>>();

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return _errors.Values;

            if (_errors.ContainsKey(propertyName))
                return _errors[propertyName];

            return null;
        }

        public bool HasErrors => _errors.Values.Count > 0;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private void SetError(string propertyName, List<string> propertyErrors)
        {
            _errors.Remove(propertyName);

            _errors.Add(propertyName, propertyErrors);

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void ClearError(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);

                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        private void ClearErrors()
        {
            var propertyNames = _errors.Keys.ToList();
            foreach (var propertyName in propertyNames)
            {
                ClearError(propertyName);
            }
        }

        #region Check

        private int CheckInput()
        {
            ClearErrors();

            int result = CheckConfiguration();
            if (result < 0)
            {
                SelectedIndex = 0;

                return result;
            }

            result = CheckCommunication();
            if (result < 0)
            {
                SelectedIndex = 1;
                return result;
            }

            result = CheckTag();
            if (result < 0)
            {
                SelectedIndex = 2;
                return result;
            }

            return result;
        }

        private int CheckTag()
        {
            int result = 0;

            string propertyName = string.Empty;
            string message = $"Failed to modify the properties for the tag '{_tag.Name}'.";
            string reason = LanguageManager.GetInstance().ConvertSpecifier("VariableNameInvalid");
            string errorCode = "Error 11271-8004202F";

            // check tag name
            if (string.Equals(_tag.Name, _tagName, StringComparison.OrdinalIgnoreCase))
                return result;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (result == 0)
            {
                Regex regex = new Regex("^[a-zA-Z_][a-zA-Z0-9_]*$");
                if (string.IsNullOrEmpty(_tagName) || !regex.IsMatch(_tagName))
                {
                    result = -1;
                }

                if (result == 0)
                {
                    if (_tagName.Length > 40 || _tagName.EndsWith("_") ||
                        _tagName.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        result = -2;
                    }
                }

                if (result == 0)
                {
                    if (_tag.ParentCollection[_tagName] != null)
                    {
                        result = -3;
                    }
                }

                if (result == 0)
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
                        if (keyWord.Equals(_tagName, StringComparison.OrdinalIgnoreCase))
                        {
                            result = -4;
                            break;
                        }
                    }
                }

                if (result != 0)
                {
                    propertyName = nameof(TagName);
                }
            }

            if (result < 0)
            {
                WarningDialog dialog = new WarningDialog(message, reason, errorCode)
                {
                    Owner = Application.Current.MainWindow
                };
                dialog.ShowDialog();

                ThreadHelper.JoinableTaskFactory.RunAsync(
                    async delegate
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        await Task.Delay(100);

                        SetError(propertyName, new List<string> {reason});
                    }
                );
            }

            return result;
        }

        private int CheckCommunication()
        {
            int result = 0;

            string propertyName = string.Empty;
            string message = string.Empty;
            string reason = string.Empty;
            string errorCode = string.Empty;

            // check path
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (result == 0)
            {
                if (string.IsNullOrEmpty(_connectionPath))
                {
                    result = -1;

                    message = "Failed to set Path value.";
                    reason = "Value not supplied.";
                    errorCode = "Error 23044-80070057";
                }
                else
                {
                    if (_connectionPath.Contains(","))
                    {
                        bool isValid = false;
                        var nodes = _connectionPath.Split(',');

                        if (nodes.Length == 2)
                        {
                            byte byte0;
                            byte byte1;
                            IPAddress ipAddress;
                            if (byte.TryParse(nodes[0].Trim(), out byte0) && byte.TryParse(nodes[1].Trim(), out byte1))
                            {
                                isValid = true;
                            }
                            else if (byte.TryParse(nodes[0], out byte0) &&
                                     IPAddress.TryParse(nodes[1].Trim(), out ipAddress))
                            {
                                isValid = true;
                            }
                        }

                        if (!isValid)
                        {
                            result = -1;

                            message = $"Failed to Path value to: {_connectionPath}";
                            reason = "Invalid path format";
                            errorCode = "Error 23039-80042040";
                        }

                    }
                    else
                    {
                        if (_connectionPath != "THIS")
                        {
                            result = -1;

                            message = $"Failed to set Path value to: {_connectionPath}";
                            reason = "Module could not be found.";
                            errorCode = "Error 23039-80042056";
                        }
                    }
                }

                if (result != 0)
                {
                    propertyName = nameof(ConnectionPath);
                }
            }

            //TODO(gjc): add code here


            if (result < 0)
            {
                WarningDialog dialog = new WarningDialog(message, reason, errorCode)
                {
                    Owner = Application.Current.MainWindow
                };
                dialog.ShowDialog();

                ThreadHelper.JoinableTaskFactory.RunAsync(
                    async delegate
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        await Task.Delay(100);

                        SetError(propertyName, new List<string> {reason});
                    }
                );
            }

            return result;
        }

        private int CheckConfiguration()
        {
            int result = 0;

            string propertyName = string.Empty;
            string message = string.Empty;
            string reason = string.Empty;
            string errorCode = string.Empty;

            //TODO(gjc): edit later
            if (_messageType != MessageTypeEnum.CIPGeneric)
            {
                result = -999;

                message = "Failed to set message type.";
                reason = "Only support CIP Generic.";
                errorCode = "Error 999";
                propertyName = nameof(MessageType);

            }

            if (_messageType == MessageTypeEnum.CIPGeneric)
            {
                ServiceTypeInfo serviceTypeInfo = ServiceTypeInfo.GetServiceTypeInfo(_serviceType);

                // check service code
                if (!serviceTypeInfo.ServiceCodeReadOnly)
                {
                    if (result == 0)
                    {
                        if (string.IsNullOrEmpty(_serviceCode))
                        {
                            result = -1;

                            message = "Failed to set Service Code value.";
                            reason = "Value not supplied";
                            errorCode = "Error 23044-80070057";
                        }
                        else
                        {
                            int serviceCode;
                            if (!int.TryParse(_serviceCode, NumberStyles.HexNumber,
                                    CultureInfo.InvariantCulture, out serviceCode))
                            {
                                result = -1;

                                message = $"Failed to set Service Code value to: {_serviceCode}";
                                reason = "String invalid.";
                                errorCode = "Error 23039-8004251A";
                            }
                            else if (serviceCode < 0 || serviceCode > 0x7fff)
                            {
                                result = -1;

                                message = "Failed to set Service Code value.";
                                reason = "Immediate value out of range.\nValue must be within 0 to 7fff.";
                                errorCode = "Error 23361-80042C2D";
                            }
                        }

                        if (result != 0)
                        {
                            propertyName = nameof(ServiceCode);
                        }
                    }

                }

                // check class
                if (!serviceTypeInfo.ClassIDReadOnly)
                {
                    if (result == 0)
                    {
                        if (string.IsNullOrEmpty(_classID))
                        {
                            result = -2;

                            message = "Failed to set Class value.";
                            reason = "Value not supplied";
                            errorCode = "Error 23044-80070057";
                        }
                        else
                        {
                            int classID;
                            if (!int.TryParse(_classID, NumberStyles.HexNumber,
                                    CultureInfo.InvariantCulture, out classID))
                            {
                                result = -2;

                                message = $"Failed to set Class value to: {_classID}";
                                reason = "String invalid.";
                                errorCode = "Error 23039-8004251A";
                            }
                            else if (classID < 0 || classID > 0x7fff)
                            {
                                result = -2;

                                message = "Failed to set Class value.";
                                reason = "Immediate value out of range.\nValue must be within 0 to 7fff.";
                                errorCode = "Error 23361-80042C2D";
                            }
                        }

                        if (result != 0)
                        {
                            propertyName = nameof(ClassID);
                        }
                    }


                }

                // check instance
                if (!serviceTypeInfo.InstanceIDReadOnly)
                {
                    if (result == 0)
                    {
                        if (string.IsNullOrEmpty(_instanceID))
                        {
                            result = -3;

                            message = "Failed to set Instance value.";
                            reason = "Value not supplied.";
                            errorCode = "Error 23044-80070057";
                        }
                        else
                        {
                            int instance;
                            if (!int.TryParse(_instanceID, out instance))
                            {
                                result = -3;

                                message = $"Failed to set Instance value to: {_instanceID}.";
                                reason = "String invalid.";
                                errorCode = "Error 23097-80042C2D";
                            }
                            else if (instance < 0 || instance > 65535)
                            {
                                result = -3;

                                message = "Failed to set Instance value.";
                                reason = "Immediate value out of range.\nValue must be within 0 to 65535.";
                                errorCode = "Error 23097-80042C2D";
                            }
                        }


                        if (result != 0)
                        {
                            propertyName = nameof(InstanceID);
                        }
                    }

                }

                // check attribute
                if (!serviceTypeInfo.AttributeIDReadOnly)
                {
                    if (result == 0)
                    {
                        if (string.IsNullOrEmpty(_attributeID))
                        {
                            result = -4;

                            message = "Failed to set Attribute value.";
                            reason = "Value not supplied.";
                            errorCode = "Error 23044-80070057";
                        }
                        else
                        {
                            int attributeID;
                            if (!int.TryParse(_attributeID, NumberStyles.HexNumber,
                                    CultureInfo.InvariantCulture, out attributeID))
                            {
                                result = -4;

                                message = $"Failed to set Attribute value to: {_attributeID}";
                                reason = "String invalid.";
                                errorCode = "Error 23039-8004251A";
                            }
                        }

                        if (result != 0)
                        {
                            propertyName = nameof(AttributeID);
                        }
                    }


                }

                // special for custom
                if (result == 0)
                {
                    if (_serviceType == CIPServiceTypeEnum.Custom)
                    {
                        var serviceType = ServiceTypeInfo.GetServiceType(
                            ushort.Parse(_serviceCode, NumberStyles.HexNumber),
                            ushort.Parse(_classID, NumberStyles.HexNumber),
                            uint.Parse(_instanceID),
                            ushort.Parse(_attributeID, NumberStyles.HexNumber));

                        if (serviceType != CIPServiceTypeEnum.Custom)
                        {
                            serviceTypeInfo = ServiceTypeInfo.GetServiceTypeInfo(serviceType);
                        }
                    }
                }

                // check source element
                if (!serviceTypeInfo.SourceElementReadOnly && result == 0)
                {
                    result = CheckSourceElement(_sourceElement, _sourceLength,
                        ref propertyName, ref message, ref reason, ref errorCode);
                }

                // check destination element
                if (!serviceTypeInfo.DestinationElementReadOnly && result == 0)
                {
                    result = CheckDestinationElement(_destinationElement,
                        ref propertyName, ref message, ref reason, ref errorCode);

                }
            }

            if (result < 0)
            {
                WarningDialog dialog = new WarningDialog(message, reason, errorCode)
                {
                    Owner = Application.Current.MainWindow
                };
                dialog.ShowDialog();

                ThreadHelper.JoinableTaskFactory.RunAsync(
                    async delegate
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        await Task.Delay(100);

                        SetError(propertyName, new List<string> {reason});
                    }
                );
            }

            return result;
        }

        private int CheckDestinationElement(
            string destinationElement,
            ref string propertyName, ref string message,
            ref string reason, ref string errorCode)
        {
            int result = 0;

            if (!string.IsNullOrEmpty(_destinationElement))
            {
                TagExpressionParser parser = new TagExpressionParser();
                var tagExpression = parser.Parser(destinationElement);

                if (tagExpression == null
                    || !string.Equals(tagExpression.ToString(), destinationElement, StringComparison.OrdinalIgnoreCase))
                {
                    result = -16;

                    message = $"Failed to set Destination Tag value to: {destinationElement}.";
                    reason = "Invalid expression or tag.";
                    errorCode = "Error 23039-80042C02";
                }
                else
                {
                    SimpleTagExpression simpleTagExpression = parser.GetSimpleTagExpression(tagExpression);
                    if (!string.IsNullOrEmpty(simpleTagExpression.Scope))
                    {
                        result = -16;

                        message = $"Failed to set Destination Tag value to: {destinationElement}.";
                        reason = "Invalid expression or tag.";
                        errorCode = "Error 23039-80042C02";
                    }
                    else
                    {
                        var dataServer = _tag.ParentController.CreateDataServer();
                        var dataOperand = dataServer.CreateDataOperand(_tag.ParentCollection, destinationElement);
                        if (!dataOperand.IsOperandValid)
                        {
                            result = -17;

                            message = $"Failed to set Destination Tag value to: {destinationElement}.";
                            reason = "Referenced tag is undefined.";
                            errorCode = "Error 23039-80042C01";
                        }
                        else if (dataOperand.IsBool)
                        {
                            result = -18;

                            message = $"Failed to save configuration for tag '{_tag.Name}'";
                            reason = "Message references a tag of BOOL data type.";
                            errorCode = "Error 23059-80042C01";
                        }
                        else if (IsNonDataTableType(dataOperand.DataTypeInfo.DataType))
                        {
                            result = -19;

                            message = $"Failed to save configuration for tag '{_tag.Name}'";
                            reason = "Message references a tag of non-datatable type.";
                            errorCode = "Error 23059-80043115";
                        }
                    }

                }
            }

            if (result != 0)
            {
                propertyName = nameof(DestinationElement);
            }

            return result;
        }

        private int CheckSourceElement(
            string sourceElement, short sourceLength,
            ref string propertyName, ref string message, ref string reason, ref string errorCode)
        {
            int result = 0;

            if (string.IsNullOrEmpty(sourceElement) && sourceLength > 0)
            {
                result = -5;

                message = $"Failed to save configuration for tag '{_tag.Name}'";
                reason = "MSG configuration incomplete because no tag is specified.";
                errorCode = "Error 23059-80043104";
            }
            else if (!string.IsNullOrEmpty(sourceElement))
            {
                TagExpressionParser parser = new TagExpressionParser();
                var tagExpression = parser.Parser(sourceElement);

                if (tagExpression == null
                    || !string.Equals(tagExpression.ToString(), sourceElement, StringComparison.OrdinalIgnoreCase))
                {
                    result = -6;

                    message = $"Failed to set Source Tag value to: {sourceElement}.";
                    reason = "Invalid expression or tag.";
                    errorCode = "Error 23039-80042C02";
                }
                else
                {
                    SimpleTagExpression simpleTagExpression = parser.GetSimpleTagExpression(tagExpression);
                    if (!string.IsNullOrEmpty(simpleTagExpression.Scope))
                    {
                        result = -6;

                        message = $"Failed to set Source Tag value to: {sourceElement}.";
                        reason = "Invalid expression or tag.";
                        errorCode = "Error 23039-80042C02";
                    }
                    else
                    {
                        var dataServer = _tag.ParentController.CreateDataServer();
                        var dataOperand = dataServer.CreateDataOperand(_tag.ParentCollection, sourceElement);
                        if (!dataOperand.IsOperandValid)
                        {
                            result = -7;

                            message = $"Failed to set Source Tag value to: {sourceElement}.";
                            reason = "Referenced tag is undefined.";
                            errorCode = "Error 23039-80042C01";
                        }
                        else if (dataOperand.IsBool)
                        {
                            result = -8;

                            message = $"Failed to save configuration for tag '{_tag.Name}'";
                            reason = "Message references a tag of BOOL data type.";
                            errorCode = "Error 23059-80042C01";
                        }
                        else if (IsNonDataTableType(dataOperand.DataTypeInfo.DataType))
                        {
                            result = -9;

                            message = $"Failed to save configuration for tag '{_tag.Name}'";
                            reason = "Message references a tag of non-datatable type.";
                            errorCode = "Error 23059-80043115";
                        }
                    }

                }

            }

            if (result != 0)
            {
                propertyName = nameof(SourceElement);
            }

            return result;
        }

        private bool IsNonDataTableType(IDataType dataType)
        {
            //Non - data table tags types are:
            //ALARM_ANALOG, ALARM_DIGITAL,
            //AXIS_CIP_DRIVE, AXIS_GENERIC, AXIS_GENERIC_DRIVE, AXIS_VIRTUAL, AXIS_SERVO, AXIS_SERVO_DRIVE,
            //COORDINATE_SYSTEM,
            //ENERGY_BASE, ENERGY_ELECTRICAL,
            //Message, and Motion_Group.

            List<string> nonDataTableTypes = new List<string>()
            {
                "ALARM_ANALOG", "ALARM_DIGITAL",
                "AXIS_CIP_DRIVE", "AXIS_GENERIC", "AXIS_GENERIC_DRIVE", "AXIS_VIRTUAL", "AXIS_SERVO",
                "AXIS_SERVO_DRIVE",
                "COORDINATE_SYSTEM", "ENERGY_BASE", "ENERGY_ELECTRICAL",
                "Message", "Motion_Group"
            };


            if (dataType == null)
                return false;

            foreach (string nonDataTableType in nonDataTableTypes)
            {
                if (string.Equals(dataType.Name, nonDataTableType, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;

        }

        #endregion
    }
}
