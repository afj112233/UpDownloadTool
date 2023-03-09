using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.StxEditor.ViewModel.IntelliPrompt;
using Newtonsoft.Json.Linq;

namespace ICSStudio.StxEditor.ViewModel.Tooltip
{
    internal enum InfoType
    {
        UnknownElement,
        InvalidElement,
        InvalidArray,
        NoValueElement,
        HasValueElement,
        Tag,
        InvalidBitSpecifier,
        UndefinedTag,
        ConstantTag,
    }

    internal class ComponentContentProvider
    {
        private const string INVALID_TAG_ELEMENT_TEMP =
            "Invalid Tag Element:{0}\r\nUsage:{1}\r\nTag Data Type:{2}\r\nTag Scope:{3}";

        private const string UNKNOWN_TAG_ELEMENT_TEMP =
            "Unknown Tag Element:{0}\r\nUsage:{1}\r\nTag Data Type:{2}\r\nTag Scope:{3}";

        private const string INVALID_ARRAY_SUBSCRIPT_TEMP =
            "Invalid Array Subscript:{0}\r\nUsage:{1}\r\nTag Data Type:{2}\r\nScope:{3}";

        private const string TAG_ELEMENT_VALUE_TEMP =
            "Tag Element:{0}\r\nValue:{1}\r\nElement Data Type:{2}\r\nUsage:{3}\r\nTag Data Type:{4}\r\nScope:{5}";

        private const string TAG_ELEMENT_VALUE_TEMP_CONTROLLER =
            "Tag Element:{0}\r\nValue:{1}\r\nElement Data Type:{2}\r\nTag Data Type:{3}\r\nScope:{4}";

        private const string TAG_ELEMENT_NO_VALUE_TEMP =
            "Tag Element:{0}\r\nElement Data Type:{1}\r\nUsage:{2}\r\nTag Data Type:{3}\r\nScope:{4}";

        private const string TAG_ELEMENT_NO_VALUE_TEMP_CONTROLLER =
            "Tag Element:{0}\r\nElement Data Type:{1}\r\nTag Data Type:{2}\r\nScope:{3}";

        private const string TAG_VALUE_TEMP = "Tag:{0}\r\nValue:{1}\r\nUsage:{2}\r\nData Type:{3}\r\nScope:{4}";
        private const string TAG_VALUE_TEMP_CONTROLLER = "Tag:{0}\r\nValue:{1}\r\nData Type:{2}\r\nScope:{3}";

        private const string TAG_NO_VALUE_TEMP = "Tag:{0}\r\nUsage:{1}\r\nData Type:{2}\r\nScope:{3}";
        private const string TAG_NO_VALUE_TEMP_CONTROLLER = "Tag:{0}\r\nData Type:{1}\r\nScope:{2}";

        private const string UNDEFINED_TAG = "Undefined Tag:{0}";

        private const string INVALID_BIT_SPECIFIER =
            "Invalid Bit Specifier:{0}\r\nUsage:{1}\r\nTag Data Type:{2}\r\nTag Scope:{3}";

        private const string PROGRAM = "Program:{0}";

        private readonly ITagCollection _programTags;
        private readonly ITagCollection _controllerTags;
        private readonly IController _controller;
        private readonly List<StxCompletionItemCodeSnippetData> _data;
        private StxEditorDocument _document;

        public ComponentContentProvider(ITagCollection programTags,
            List<StxCompletionItemCodeSnippetData> data, StxEditorDocument document)
        {
            _programTags = programTags;
            _controller = Controller.GetInstance();
            _data = data;
            _document = document;
            _controllerTags = _controller.Tags;
        }

        private string _originalName = "";
        public string GetCoverStrInfo(string str)
        {
            //check tag
            int p = 1;
            ITagCollection tagCollection = _programTags;
            _originalName = "";
            if (_document.SnippetLexer.TransformTable != null)
            {
                _originalName = str;
                str = _document.SnippetLexer.ConnectionReference.GetTransformName(str);
                tagCollection = _document.SnippetLexer.ConnectionReference.GetReferenceProgram()?.Tags ??
                                _controllerTags;
            }

            if (str.IndexOf("\\") == 0)
            {
                var otherProgramName = str.IndexOf(".") > 0 ? str.Substring(1, str.IndexOf(".") - 1) : str.Substring(1);
                var name = str.IndexOf(".") > 0 ? str.Substring(str.IndexOf(".") + 1) : null;
                var program = _controller.Programs[otherProgramName];
                if (program != null)
                {
                    if (name == null)
                    {
                        return String.Format(PROGRAM, otherProgramName);
                    }

                    foreach (Tag tag in program.Tags)
                    {
                        var info = TryGetInfo(name, tag, ref p);
                        if (info != null) return info;
                    }

                    return GetInfo(InfoType.UndefinedTag, str, null, null, null);
                }
            }

            foreach (Tag tag in tagCollection)
            {
                var info = TryGetInfo(str, tag, ref p);
                if (info != null) return info;
            }

            foreach (Tag tag in _controllerTags)
            {
                var info = TryGetInfo(str, tag, ref p);
                if (info != null) return info;
            }

            //instruction
            foreach (var aoi in _controllerTags.ParentController.AOIDefinitionCollection)
            {
                var aoi2 = (AoiDefinition) aoi;
                if (aoi2.Name.Equals(str, StringComparison.OrdinalIgnoreCase))
                {
                    var param = "";
                    foreach (var parameter in aoi2.GetParameterTags())
                    {
                        if (!(parameter.Name.Equals("EnableIn") || parameter.Name.Equals("EnableOut")))
                        {
                            if (parameter.IsRequired)
                            {
                                param = $"{param},{parameter.Name}";
                            }
                        }
                    }

                    if (param.Any())
                    {
                        param = param.Substring(1);
                    }

                    return string.IsNullOrEmpty(aoi2.Description)
                        ? $"Instruction {str}({str}" + (string.IsNullOrEmpty(param) ? ")" : $",{ param})")
                        : $"Instruction {str}({str}" + (string.IsNullOrEmpty(param) ? ")" : $",{ param})") + $"\r\n{aoi2.Description}";
                }
            }
            // Check function

            foreach (var completionData in _data)
            {
                if ((completionData).Name.Equals(str, StringComparison.OrdinalIgnoreCase))
                {
                    return $"Function {str}({completionData.Parameters})\r\n{completionData.Description}";
                }
            }

            return $"Undefined Tag:{str}";
        }

        public string TryGetInfo(string str, Tag tag, ref int p)
        {
            try
            {
                var arrayStr = SplitCodeSnippet(str, ".");
                string baseTag = arrayStr[0];
                if (baseTag.IndexOf("[") > 0)
                    baseTag = baseTag.Substring(0, baseTag.IndexOf("["));
                if (tag.Name.Equals(baseTag, StringComparison.OrdinalIgnoreCase))
                {
                    if (arrayStr[0].IndexOf("[") < 0)
                    {
                        if (arrayStr.Count == 1)
                        {
                            var data = FormatOp.ConvertField(tag.DataWrapper.Data, tag.DisplayStyle);
                            if (data != null) return GetInfo(InfoType.Tag, str, tag, data, null);
                            if (tag.DataTypeInfo.DataType.FamilyType == FamilyType.StringFamily)
                            {
                                var value = ObtainValue.GetStringData(tag.DataWrapper.Data, tag.DataTypeInfo.DataType);
                                value = $"'{value}'";
                                return GetInfo(InfoType.Tag, str, tag, value, null);
                            }
                            return GetInfo(InfoType.Tag, str, tag, null, null);
                        }
                        else
                        {
                            if (tag.DataTypeInfo.Dim1 > 0)
                            {
                                return GetInfo(InfoType.UnknownElement, str, tag, null, null);
                            }

                            return GetContent(arrayStr, tag.DataTypeInfo, tag.DisplayStyle, ref p, tag.DataWrapper.Data,
                                tag);
                        }
                    }
                    else
                    {
                        string arrayInt = str.Substring(arrayStr[0].IndexOf("[") + 1,
                            arrayStr[0].Length - 1 - arrayStr[0].IndexOf("[") - 1);
                        List<int> dimList = new List<int>();
                        arrayInt = ConvertArrayCodeSnippetToValue(arrayInt, dimList);
                        if (string.IsNullOrEmpty(arrayInt))
                            return GetInfo(InfoType.InvalidElement, str, tag, null, null);

                        Regex regex = new Regex(@"^[0-9]+(\,[0-9]+){0,2}$");
                        if (!regex.IsMatch(arrayInt))
                            return GetInfo(InfoType.InvalidElement, str, tag, null, null);

                        if (arrayStr.Count == 1)
                        {
                            int offset = 0;
                            if (!GetArrayIndex(tag.DataTypeInfo, ref offset, dimList))
                                return GetInfo(InfoType.InvalidArray, str, tag, null,
                                    null);

                            IField field = tag.DataWrapper.Data is IArrayField
                                ? ObtainValue.GetArrayField((IArrayField) tag.DataWrapper.Data, offset)
                                : (tag.DataWrapper.Data as ICompositeField)?.fields?[offset].Item1;
                            if (field == null)
                            {
                                if (tag.DataWrapper.Data is BoolArrayField)
                                {
                                    var value = ((BoolArrayField) tag.DataWrapper.Data)?.Get(offset);
                                    if (value != null)
                                    {
                                        return GetInfo(InfoType.HasValueElement, str, tag, (bool) value ? "1" : "0",
                                            tag.DataTypeInfo.ToString());
                                    }
                                }

                                return GetInfo(InfoType.InvalidArray, str, tag, null, null);
                            }
                            else
                            {
                                // 字符串数组特殊处理
                                if (tag.DataTypeInfo.DataType.FamilyType == FamilyType.StringFamily)
                                {
                                    var value = ObtainValue.GetStringData(field, tag.DataTypeInfo.DataType);
                                    value = $"'{value}'";
                                    return GetInfo(InfoType.Tag, str, tag, value, null);
                                }
                                var data = field.ToJToken();
                                if (!(data is JValue))
                                    return GetInfo(InfoType.NoValueElement, str, tag, null,
                                        tag.DataTypeInfo.DataType.Name
                                    );
                                else
                                {
                                    return GetInfo(InfoType.HasValueElement, str, tag,
                                        FormatOp.ConvertField(field, tag.DisplayStyle),
                                        tag.DataTypeInfo.DataType.Name);
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < dimList.Count; i++)
                            {
                                if (i == 0 && dimList[i] >= tag.DataTypeInfo.Dim1)
                                    return GetInfo(InfoType.InvalidArray, str, tag, null, null);
                                if (i == 1 && dimList[i] >= tag.DataTypeInfo.Dim2)
                                    return GetInfo(InfoType.InvalidArray, str, tag, null, null);
                                if (i == 2 && dimList[i] >= tag.DataTypeInfo.Dim3)
                                    return GetInfo(InfoType.InvalidArray, str, tag, null, null);
                            }

                            int offset = 0;
                            if (!GetArrayIndex(tag.DataTypeInfo, ref offset, dimList))
                                return GetInfo(InfoType.InvalidArray, str, tag, null,
                                    null);
                            var value = tag.DataWrapper.Data is IArrayField
                                ? ObtainValue.GetArrayField((IArrayField) tag.DataWrapper.Data, offset)
                                : ((ICompositeField) tag.DataWrapper.Data)?.fields?[offset].Item1;
                            if (tag.DataWrapper.Data is BoolArrayField)
                            {
                                return GetInfo(InfoType.InvalidBitSpecifier, str, tag, null, null);
                            }

                            return GetContent(arrayStr, new DataTypeInfo() {DataType = tag.DataTypeInfo.DataType},
                                tag.DisplayStyle, ref p, value, tag
                                );
                        }
                    }
                }
            }
            catch (Exception)
            {
                //ignore
            }

            return null;
        }

        public JValue GetCodeValue(string code)
        {
            if (code.IndexOf("\\") == 0)
            {
                var otherProgramName =
                    code.IndexOf(".") > 0 ? code.Substring(1, code.IndexOf(".") - 1) : code.Substring(1);
                var name = code.IndexOf(".") > 0 ? code.Substring(code.IndexOf(".") + 1) : "";
                var program = _controller.Programs[otherProgramName];
                if (program != null)
                {
                    foreach (Tag tag in program.Tags)
                    {
                        var jValue = DoGetCodeValue(name, tag);
                        if (jValue != null) return jValue;
                    }
                }

                return null;
            }

            foreach (var programTag in _programTags)
            {
                var jValue = DoGetCodeValue(code, programTag as Tag);
                if (jValue != null) return jValue;
            }

            foreach (var controllerTag in _controllerTags)
            {
                var jValue = DoGetCodeValue(code, controllerTag as Tag);
                if (jValue != null) return jValue;
            }

            return null;
        }

        private JValue DoGetCodeValue(string str, Tag tag)
        {
            int p = 1;
            List<string> arrayStr = SplitCodeSnippet(str.Replace(" ", ""), ".");
            string baseTag = arrayStr[0];
            if (baseTag.IndexOf("[") > 0)
                baseTag = baseTag.Substring(0, baseTag.IndexOf("["));
            if (tag.Name.Equals(baseTag, StringComparison.OrdinalIgnoreCase))
            {
                if (arrayStr[0].IndexOf("[") < 0)
                {
                    if (arrayStr.Count == 1)
                    {
                        if (tag.DataWrapper.Data.ToJToken() is JValue)
                            return ((JValue) tag.DataWrapper.Data.ToJToken());
                        else
                        {
                            return new JValue(0);
                        }
                    }
                    else
                    {
                        if (tag.DataTypeInfo.Dim1 > 0)
                        {
                            return null;
                        }

                        return GetValue(arrayStr, tag.DataTypeInfo, ref p, tag.DataWrapper.Data);
                    }
                }
                else
                {
                    string arrayInt = str.Substring(arrayStr[0].IndexOf("[") + 1,
                        arrayStr[0].Length - 1 - arrayStr[0].IndexOf("[") - 1);

                    List<int> dimList = new List<int>();
                    if (!SearchTagValue(arrayInt, dimList)) return null;

                    var tagData = tag.DataWrapper.Data;
                    if (arrayStr.Count == 1)
                    {
                        int offset = 0;
                        if (!GetArrayIndex(tag.DataTypeInfo, ref offset, dimList)) return null;
                        var value = (tagData.ToJToken() as JArray)?[offset];
                        if (value is JValue)
                            return value as JValue;
                        else
                        {
                            if (value != null)
                                return new JValue(0);
                            return null;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < dimList.Count; i++)
                        {
                            if (i == 0 && dimList[i] >= tag.DataTypeInfo.Dim1) return null;
                            if (i == 1 && dimList[i] >= tag.DataTypeInfo.Dim2)
                                return null;
                            if (i == 2 && dimList[i] >= tag.DataTypeInfo.Dim3)
                                return null;
                        }

                        int offset = 0;
                        if (!GetArrayIndex(tag.DataTypeInfo, ref offset, dimList)) return null;

                        //var value = (tagData as ArrayField)?.fields?[offset].Item1;
                        var value = ObtainValue.GetArrayField(tagData, offset);

                        return GetValue(arrayStr, tag.DataTypeInfo, ref p, value);
                    }
                }
            }

            return null;
        }

        public List<string> SplitCodeSnippet(string snippet, string separator)
        {
            List<string> subCodeList = new List<string>();
            int leftBracketCount = 0;
            int p = 0;
            string code = "";
            while (p < snippet.Length)
            {
                string letter = snippet.Substring(p, 1);
                if (letter == "[") leftBracketCount++;
                if (letter == "]") leftBracketCount--;
                if (letter == separator && leftBracketCount == 0)
                {
                    subCodeList.Add(code.Trim());
                    code = "";
                    p++;
                    continue;
                }

                code = code + letter;
                p++;
            }

            subCodeList.Add(code.Trim());
            return subCodeList;
        }

        private bool GetArrayIndex(DataTypeInfo dataTypeInfo, ref int offset, List<int> values)
        {
            if (dataTypeInfo.Dim3 > 0)
            {
                if (values.Count == 3)
                    offset = dataTypeInfo.Dim2 * dataTypeInfo.Dim1 * values[0] +
                             dataTypeInfo.Dim1 * values[1] + values[2];
                else
                    return false;
            }
            else if (dataTypeInfo.Dim2 > 0)
            {
                if (values.Count == 2)
                    offset = dataTypeInfo.Dim1 * values[0] + values[1];
                else
                    return false;
            }
            else
            {
                if (values.Count == 1)
                    offset = values[0];
                else
                    return false;
            }

            if (offset >= dataTypeInfo.Dim1 * (dataTypeInfo.Dim2 == 0 ? 1 : dataTypeInfo.Dim2) *
                (dataTypeInfo.Dim3 == 0 ? 1 : dataTypeInfo.Dim3))
            {
                if (offset == 0) return true;
                return false;
            }

            return true;
        }

        private string ConvertArrayCodeSnippetToValue(string arrayInt, List<int> dimList)
        {
            if (!SearchTagValue(arrayInt, dimList)) return null;
            arrayInt = "";
            foreach (var value in dimList)
            {
                arrayInt = arrayInt + "," + value;
            }

            if (arrayInt.Length < 2) return "";

            arrayInt = arrayInt.Substring(1);
            return arrayInt;
        }

        private bool SearchTagValue(string arrayInt, List<int> dimList)
        {
            List<string> array = SplitCodeSnippet(arrayInt.Replace(" ", ""), ",");

            Regex regex2 = new Regex("^[0-9]+$");
            foreach (var s in array)
            {
                if (string.IsNullOrEmpty(s)) continue;

                if (regex2.IsMatch(s))
                {
                    dimList.Add(int.Parse(s));
                }
                else
                {
                    Tag tag = null;
                    //JValue value = GetCodeValue(s);
                    var value = ObtainValue.GetTagValue(s,
                        _document.SnippetLexer.ConnectionReference?.Routine?.ParentCollection.ParentProgram ??
                        _programTags.ParentProgram, _document.SnippetLexer.TransformTable, ref tag);
                    //if (value == null) return false;
                    int result;
                    var flag = int.TryParse(value, out result);
                    if (flag)
                        dimList.Add(result);
                    else
                        return false;
                }
            }

            return true;
        }

        private string GetInfo(InfoType type, string tagName, Tag tagSource, string convertedValue,
            string elementDataType)
        {
            if (!string.IsNullOrEmpty(convertedValue)) convertedValue = convertedValue.Trim();
            var dataTypeInfo
                = tagSource?.DataTypeInfo.ToString().Replace(":SINT", "");
            if (!string.IsNullOrEmpty(elementDataType))
                elementDataType = elementDataType.Replace(":SINT", "");
            var scope = tagSource?.IsControllerScoped ?? true
                ? "Controller"
                : tagSource.ParentCollection?.ParentProgram?.Name;
            //var description = tagSource?.GetChildDescription(Tag.GetOperand(tagName));
            var description = Tag.GetChildDescription(tagSource.Description, tagSource.DataTypeInfo,
                tagSource.ChildDescription, Tag.GetOperand(tagName));
            if (type == InfoType.InvalidElement)
            {
                return string.Format(INVALID_TAG_ELEMENT_TEMP,
                           string.IsNullOrEmpty(_originalName) ? tagName : _originalName,
                           ExtendUsage.ToString(tagSource.Usage),
                           dataTypeInfo, scope) + (string.IsNullOrEmpty(description)
                           ? ""
                           : $"\nDescription:{description}");
            }

            if (type == InfoType.UnknownElement)
            {
                return string.Format(UNKNOWN_TAG_ELEMENT_TEMP,
                           string.IsNullOrEmpty(_originalName) ? tagName : _originalName,
                           ExtendUsage.ToString(tagSource.Usage),
                           dataTypeInfo, scope) + (string.IsNullOrEmpty(description)
                           ? ""
                           : $"\nDescription:{description}");
            }

            if (type == InfoType.NoValueElement)
            {
                if (tagSource.ParentCollection?.ParentProgram == null)
                {
                    return (tagSource.IsConstant ? "Constant " : "") + string.Format(
                               TAG_ELEMENT_NO_VALUE_TEMP_CONTROLLER,
                               string.IsNullOrEmpty(_originalName) ? tagName : _originalName,
                               elementDataType,
                               dataTypeInfo, scope) + (string.IsNullOrEmpty(description)
                               ? ""
                               : $"\nDescription:{description}");
                }

                return (tagSource.IsConstant ? "Constant " : "") + string.Format(TAG_ELEMENT_NO_VALUE_TEMP,
                           string.IsNullOrEmpty(_originalName) ? tagName : _originalName,
                           elementDataType,
                           ExtendUsage.ToString(tagSource.Usage),
                           dataTypeInfo, scope) + (string.IsNullOrEmpty(description)
                           ? ""
                           : $"\nDescription:{description}");
            }

            if (type == InfoType.HasValueElement)
            {
                if (tagSource.ParentCollection?.ParentProgram == null)
                    return (tagSource.IsConstant ? "Constant " : "") + string.Format(TAG_ELEMENT_VALUE_TEMP_CONTROLLER,
                               string.IsNullOrEmpty(_originalName) ? tagName : _originalName, convertedValue,
                               elementDataType,
                               dataTypeInfo, scope) + (string.IsNullOrEmpty(description)
                               ? ""
                               : $"\nDescription:{description}");
                return (tagSource.IsConstant ? "Constant " : "") + string.Format(TAG_ELEMENT_VALUE_TEMP,
                           string.IsNullOrEmpty(_originalName) ? tagName : _originalName,
                           convertedValue, elementDataType,
                           ExtendUsage.ToString(tagSource.Usage), dataTypeInfo, scope) +
                       (string.IsNullOrEmpty(description) ? "" : $"\nDescription:{description}");
            }

            if (type == InfoType.Tag)
            {
                if (!string.IsNullOrEmpty(convertedValue))
                {
                    if (tagSource.ParentCollection?.ParentProgram == null)
                        return (tagSource.IsConstant ? "Constant " : "") + string.Format(TAG_VALUE_TEMP_CONTROLLER,
                                   string.IsNullOrEmpty(_originalName) ? tagName : _originalName, convertedValue,
                                   dataTypeInfo, scope) + (string.IsNullOrEmpty(description)
                                   ? ""
                                   : $"\nDescription:{description}");
                    return (tagSource.IsConstant ? "Constant " : "") + string.Format(TAG_VALUE_TEMP,
                               string.IsNullOrEmpty(_originalName) ? tagName : _originalName,
                               convertedValue, ExtendUsage.ToString(tagSource.Usage),
                               dataTypeInfo, scope) + (string.IsNullOrEmpty(description)
                               ? ""
                               : $"\nDescription:{description}");
                }

                if (tagSource.ParentCollection?.ParentProgram == null)
                    return (tagSource.IsConstant ? "Constant " : "") + string.Format(TAG_NO_VALUE_TEMP_CONTROLLER,
                               string.IsNullOrEmpty(_originalName) ? tagName : _originalName, dataTypeInfo,
                               scope) + (string.IsNullOrEmpty(description)
                               ? ""
                               : $"\nDescription:{description}");
                return (tagSource.IsConstant ? "Constant " : "") + string.Format(TAG_NO_VALUE_TEMP,
                           string.IsNullOrEmpty(_originalName) ? tagName : _originalName,
                           ExtendUsage.ToString(tagSource.Usage), dataTypeInfo,
                           scope) + (string.IsNullOrEmpty(description)
                           ? ""
                           : $"\nDescription:{description}");
            }

            if (type == InfoType.InvalidArray)
            {
                return string.Format(INVALID_ARRAY_SUBSCRIPT_TEMP,
                           string.IsNullOrEmpty(_originalName) ? tagName : _originalName,
                           ExtendUsage.ToString(tagSource.Usage),
                           dataTypeInfo, scope) + (string.IsNullOrEmpty(description)
                           ? ""
                           : $"\nDescription:{description}");
            }

            if (type == InfoType.InvalidBitSpecifier)
            {
                return string.Format(INVALID_BIT_SPECIFIER,
                           string.IsNullOrEmpty(_originalName) ? tagName : _originalName,
                           ExtendUsage.ToString(tagSource.Usage),
                           dataTypeInfo, scope) + (string.IsNullOrEmpty(description)
                           ? ""
                           : $"\nDescription:{description}");
            }

            if (type == InfoType.UndefinedTag)
            {
                return string.Format(UNDEFINED_TAG, string.IsNullOrEmpty(_originalName) ? tagName : _originalName);
            }

            return null;
        }

        private string GetContent(List<string> codeList, DataTypeInfo dataTypeInfo, DisplayStyle displayStyle,
            ref int p, IField field, Tag tagSource)
        {
            if (dataTypeInfo.DataType is CompositiveType && dataTypeInfo.Dim1 == 0)
            {
                string baseName = codeList[p];
                List<int> values = new List<int>();
                if (baseName.IndexOf("[") > 0)
                {
                    if (baseName.IndexOf("]") < 0)
                        return GetInfo(InfoType.InvalidElement, string.Join(".", codeList), tagSource, null, null
                        );
                    var arrayInt = baseName.Substring(baseName.IndexOf("[") + 1,
                        baseName.Length - 1 - baseName.IndexOf("[") - 1);
                    baseName = baseName.Substring(0, baseName.IndexOf("["));
                    if (!SearchTagValue(arrayInt, values))
                        return GetInfo(InfoType.InvalidArray, string.Join(".", codeList), tagSource, null,
                            null);
                }

                if (p >= codeList.Count)
                    return GetInfo(InfoType.InvalidArray, string.Join(".", codeList), tagSource, null, null
                    );
                foreach (DataTypeMember member in (dataTypeInfo.DataType as CompositiveType).TypeMembers)
                {
                    if (member.Name.Equals(baseName, StringComparison.OrdinalIgnoreCase))
                    {

                        //field = field is ArrayField
                        //    ? ((ArrayField) field)?.fields?[member.FieldIndex].Item1
                        //    : ((ICompositeField) field)?.fields?[member.FieldIndex].Item1;
                        field = field is IArrayField
                            ? ObtainValue.GetArrayField(field, member.FieldIndex)
                            : ((ICompositeField) field)?.fields?[member.FieldIndex].Item1;
                        if (member.IsBit && member.Dim1 == 0)
                        {
                            field = new BoolField(field.GetBitValue(member.BitOffset) ? 1 : 0);
                        }

                        if (member.DataTypeInfo.Dim1 > 0)
                        {
                            if (codeList[p].IndexOf("[") < 0)
                                return GetInfo(InfoType.NoValueElement, string.Join(".", codeList), tagSource, null,
                                    member.DataTypeInfo.ToString());
                            return GetContent(codeList, member.DataTypeInfo, member.DisplayStyle, ref p, field,
                                tagSource);
                        }

                        if (codeList.Count == p + 1)
                        {
                            var data = FormatOp.ConvertField(field, member.DisplayStyle);
                            if (data != null)
                            {
                                return GetInfo(InfoType.HasValueElement,
                                    string.Join(".", codeList), tagSource, data, member.DataTypeInfo.ToString()
                                );
                            }
                            else
                            {
                                return GetInfo(InfoType.NoValueElement,
                                    string.Join(".", codeList), tagSource, null, member.DataTypeInfo.ToString()
                                );
                            }

                        }

                        p = p + 1;
                        var info = GetContent(codeList, member.DataTypeInfo, member.DisplayStyle, ref p, field,
                            tagSource);
                        if (info != null) return info;
                    }
                }
            }
            else
            {
                var data = field.ToJToken() as JValue;
                if (data != null)
                {
                    Regex regex = new Regex(@"^[0-9]+$");
                    var codeSnippet = codeList[p];
                    if (regex.IsMatch(codeSnippet))
                    {
                        if (dataTypeInfo.DataType is SINT || dataTypeInfo.DataType is DINT ||
                            dataTypeInfo.DataType is INT || dataTypeInfo.DataType is LINT)
                        {
                            int codeIndex = int.Parse(codeSnippet);
                            if (codeIndex >= dataTypeInfo.DataType.BitSize)
                                return GetInfo(InfoType.InvalidBitSpecifier, string.Join(".", codeList), tagSource,
                                    null, null);
                            var offset = 1L << codeIndex;
                            var bitValue = (int.Parse(data.Value.ToString()) & offset) > 0 ? 1 : 0;
                            return GetInfo(InfoType.HasValueElement, string.Join(".", codeList), tagSource,
                                bitValue.ToString(), "BOOL");
                        }

                        return GetInfo(InfoType.InvalidBitSpecifier, string.Join(".", codeList), tagSource, null,
                            null);
                    }

                    if (dataTypeInfo.Dim1 > 0 && codeList[p].IndexOf("[") > 0)
                    {
                        return GetInfo(InfoType.HasValueElement, string.Join(".", codeList), tagSource,
                            data.Value.ToString(), "BOOL");
                    }

                    return GetInfo(InfoType.UnknownElement, string.Join(".", codeList), tagSource, null, null
                    );
                    //return GetInfo(InfoType.HasValueElement, string.Join(".", codeList), tagSource, ConvertField(field,displayStyle),
                    //    dataTypeInfo.ToString());
                }

                if (field is IArrayField)
                {
                    string baseName = codeList[p];
                    if (baseName.IndexOf("[") > 0)
                    {
                        if (baseName.IndexOf("]") < 0)
                            return GetInfo(InfoType.InvalidElement, string.Join(".", codeList), tagSource, null, null
                            );
                        var arrayInt = baseName.Substring(baseName.IndexOf("[") + 1,
                            baseName.Length - 1 - baseName.IndexOf("[") - 1);
                        List<int> values = new List<int>();
                        if (!SearchTagValue(arrayInt, values))
                            return GetInfo(InfoType.InvalidArray, string.Join(".", codeList), tagSource, null,
                                null);
                        //Regex regex = new Regex(@"^[0-9]+(\,[0-9]+){0,2}$");

                        //if (!regex.IsMatch(arrayInt))
                        //    return GetInfo(InfoType.InvalidElement, string.Join(".", codeList), tagSource, null, null,
                        //        parentRoutine);

                        int offset = 0;
                        if (!GetArrayIndex(dataTypeInfo, ref offset, values))
                            return GetInfo(InfoType.InvalidArray, string.Join(".", codeList), tagSource, null,
                                null);
                        //field = (field as ArrayField).fields[offset].Item1;
                        field = ObtainValue.GetArrayField(field, offset);
                        p++;
                        if (p < codeList.Count)
                        {
                            var newDataTypeInfo = new DataTypeInfo() {DataType = dataTypeInfo.DataType};
                            return GetContent(codeList, newDataTypeInfo, displayStyle, ref p, field, tagSource
                                );
                        }

                        return GetInfo(InfoType.HasValueElement,
                            string.Join(".", codeList), tagSource, FormatOp.ConvertField(field, displayStyle),
                            dataTypeInfo.ToString()
                        );
                    }
                }
            }

            return GetInfo(InfoType.UnknownElement, string.Join(".", codeList), tagSource, null, null);
        }

        private JValue GetValue(List<string> codeList, DataTypeInfo dataTypeInfo, ref int p, IField data)
        {
            if (dataTypeInfo.DataType is CompositiveType)
            {
                string baseName = codeList[p];
                if (baseName.IndexOf("[") > 0)
                {
                    if (baseName.IndexOf("]") < 0) return null;
                    var arrayInt = baseName.Substring(baseName.IndexOf("[") + 1,
                        baseName.Length - 1 - baseName.IndexOf("[") - 1);
                    baseName = baseName.Substring(0, baseName.IndexOf("["));
                    List<string> array = SplitCodeSnippet(arrayInt, ",");
                    List<int> values = new List<int>();
                    Regex regex2 = new Regex("^[0-9]+$");
                    foreach (var s in array)
                    {
                        if (regex2.IsMatch(s))
                        {
                            values.Add(int.Parse(s));
                        }
                        else
                        {
                            JValue value = GetCodeValue(s);

                            if (value == null) return null;
                            values.Add((int) value.Value);
                        }
                    }

                    Regex regex = new Regex(@"^[0-9]+(\,[0-9]+){0,2}$");

                    if (!regex.IsMatch(arrayInt)) return null;
                    int offset = 0;
                    if (!GetArrayIndex(dataTypeInfo, ref offset, values)) return null;
                    data = ObtainValue.GetArrayField(data, offset);
                }

                if (p >= codeList.Count) return null;
                foreach (DataTypeMember member in (dataTypeInfo.DataType as CompositiveType).TypeMembers)
                {
                    if (member.Name.Equals(baseName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (codeList.Count == p + 1)
                        {
                            return data is IArrayField
                                ? ObtainValue.GetArrayField(data, member.FieldIndex).ToJToken() as JValue
                                : (data as ICompositeField).fields[member.FieldIndex].Item1.ToJToken() as JValue;
                        }

                        if (member.DataTypeInfo.Dim1 > 0)
                        {
                            if (codeList[p].IndexOf("[") < 0)
                                return null;
                        }

                        p = p + 1;
                        var info = GetValue(codeList, member.DataTypeInfo, ref p,
                            data is IArrayField
                                ? ObtainValue.GetArrayField(data, member.FieldIndex)
                                : (data as ICompositeField).fields[member.FieldIndex].Item1);
                        if (info != null) return info;
                    }
                }
            }
            else
            {
                var value = data.ToJToken() as JValue;
                if (value != null)
                {
                    Regex regex = new Regex(@"^[0-9]+$");
                    var codeSnippet = codeList[p];
                    if (string.IsNullOrEmpty(codeSnippet)) return value;
                    if (regex.IsMatch(codeSnippet))
                    {
                        if (dataTypeInfo.DataType is SINT || dataTypeInfo.DataType is DINT ||
                            dataTypeInfo.DataType is INT || dataTypeInfo.DataType is LINT)
                        {
                            int codeIndex = int.Parse(codeSnippet);
                            if (codeIndex >= dataTypeInfo.DataType.BitSize)
                                return null;
                            var bitValue = Convert.ToString(int.Parse(value.Value.ToString()), 2)
                                .PadLeft(dataTypeInfo.DataType.BitSize, '0')
                                .Substring(dataTypeInfo.DataType.BitSize - codeIndex - 1, 1);
                            return new JValue(int.Parse(bitValue));
                        }
                    }
                    else
                    {
                        return null;
                    }

                }
            }

            return null;
        }

    }
}
