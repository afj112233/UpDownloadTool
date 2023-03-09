using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ICSStudio.AvalonEdit.Document;
using ICSStudio.Gui.View;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.Interfaces.Notification;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Annotations;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.Instruction;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json.Linq;
using Task = System.Threading.Tasks.Task;

namespace ICSStudio.AvalonEdit.Variable
{
    public partial class VariableInfo : IVariableInfo, INotifyPropertyChanged, IConsumer
    {
        private ITag _tag;
        private string _value;
        private IField _field;
        private int _index = -1;
        private bool _disposed;
        private IRoutine _routine;
        private IProgramModule _program;
        private string _name = "";
        private string _codeText;

        public VariableInfo(ASTNode node, string name, IRoutine routine, int offset, string codeText,TextLocation textLocation,
            bool isInstr = false)
        {
            Debug.Assert(!string.IsNullOrEmpty(name));
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            _codeText = codeText;
            _routine = routine;
            _program = _routine.ParentCollection.ParentProgram;
            Name = name;
            IsInstr = isInstr;
            AstNode = node;
            Offset = offset;
            TextLocation = textLocation;
        }

        public bool IsDisposed => _disposed;
        
        public IRoutine Routine => _routine;
        
        public ASTNode AstNode { get; private set; }
        
        private bool _isConnected=false;
        public void StartTimer()
        {
            if (IsDisplay && !_isConnected&&!_disposed) 
            {
                _isConnected = true;
                Notifications.ConnectConsumer(this);
            }
        }

        public bool IsValueChanged { set; get; }

        public void StopTimer()
        {
            if (IsDisplay)
            {
                _isConnected = false;
                Notifications.DisconnectConsumer(this);
            }
        }

        public bool IsHit(int offset)
        {
            if (_disposed) return false;
            var length = _name.Length;
            if (AstNode != null)
            {
                length = AstNode.ContextEnd - AstNode.ContextStart + 1;
            }
            if (Offset <= offset && Offset + length >= offset) return true;
            return false;
        }
        
        public bool IsFocus(bool isTop)
        {
            if (GetInlineVisibility(isTop) != Visibility.Visible) return false;
            return true;
        }
        
        public bool IsRoutine { set; get; } = false;
        public bool IsTask { set; get; } = false;
        public bool IsInstr { get; set; }
        public bool IsProgram { set; get; }
        public bool IsModule { set; get; }
        public bool IsAOI { set; get; }
        public bool IsUnknown { set; get; }
        public bool IsNum { set; get; }
        public bool IsUseForJSR { set; get; }
        public bool IsJSR { get; set; }

        public IDeviceModule Module{get; set;}

        public bool IsEnabled
        {
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
            get
            {
                if (_dimTags.Any() || (Tag?.DataTypeInfo.DataType.IsMotionGroupType ?? false) ||
                    (Tag?.DataTypeInfo.DataType.IsAxisType ?? false))
                {
                    return false;
                }

                if (Tag?.ParentCollection.ParentProgram is AoiDefinition && _transformTable == null)
                {
                    if ("EnableIn".Equals(Name, StringComparison.OrdinalIgnoreCase) ||
                        "EnableOut".Equals(Name, StringComparison.OrdinalIgnoreCase)) return false;
                }

                return _isEnabled;
            }
        }
        
        public void SetSpecialStringField(int len, string str)
        {
            try
            {
                if (DataType.FamilyType == FamilyType.StringFamily)
                {
                    if (DataType is STRING)
                    {
                        Debug.Assert(_field is STRINGField);
                        var stringField = (STRINGField)_field;
                        var bytes = ASCIIEncoding.ASCII.GetBytes(str);
                        var strFiled = (ArrayField)stringField.fields[1].Item1;
                        if (Controller.GetInstance().IsOnline)
                        {
                            Task.Run(async delegate
                            {
                                await TaskScheduler.Default;

                                if (_transformTable != null)
                                {
                                    await Controller.GetInstance()
                                        .SetTagValueToPLC(Tag, $"{_connectionName}.LEN", len.ToString());
                                    for (int i = 0; i < bytes.Length; i++)
                                    {
                                        await Controller.GetInstance().SetTagValueToPLC(Tag,
                                            $"{_connectionName}.DATA[{i}]",
                                            ((sbyte)bytes[i]).ToString());
                                    }
                                }
                                else
                                {
                                    await Controller.GetInstance().SetTagValueToPLC(Tag, $"{Name}.LEN", len.ToString());

                                    for (int i = 0; i < bytes.Length; i++)
                                    {
                                        await Controller.GetInstance().SetTagValueToPLC(Tag, $"{Name}.DATA[{i}]",
                                            ((sbyte)bytes[i]).ToString());
                                    }
                                }
                            });
                        }
                        else
                        {
                            ((Int32Field)stringField.fields[0].Item1).value = len;
                            for (int i = 0; i < bytes.Length; i++)
                            {
                                ((Int8Field)strFiled.fields[i].Item1).value = (sbyte)bytes[i];
                            }
                        }
                    }
                    else
                    {
                        var arrayField = _field as UserDefinedField;
                        Debug.Assert(arrayField != null && arrayField.fields.Count == 2);
                        var bytes = ASCIIEncoding.ASCII.GetBytes(str);
                        var strFiled = (ArrayField)arrayField.fields[1].Item1;

                        if (Controller.GetInstance().IsOnline)
                        {
                            Task.Run(async delegate
                            {
                                await TaskScheduler.Default;

                                if (_transformTable != null)
                                {
                                    await Controller.GetInstance()
                                        .SetTagValueToPLC(Tag, $"{_connectionName}.LEN", len.ToString());
                                    for (int i = 0; i < bytes.Length; i++)
                                    {
                                        await Controller.GetInstance().SetTagValueToPLC(Tag,
                                            $"{_connectionName}.DATA[{i}]",
                                            ((sbyte)bytes[i]).ToString());
                                    }
                                }
                                else
                                {
                                    await Controller.GetInstance().SetTagValueToPLC(Tag, $"{Name}.LEN", len.ToString());

                                    for (int i = 0; i < bytes.Length; i++)
                                    {
                                        await Controller.GetInstance().SetTagValueToPLC(Tag, $"{Name}.DATA[{i}]",
                                            ((sbyte)bytes[i]).ToString());
                                    }
                                }
                            });
                        }
                        else
                        {
                            ((Int32Field)arrayField.fields[0].Item1).value = len;
                            for (int i = 0; i < bytes.Length; i++)
                            {

                                ((Int8Field)strFiled.fields[i].Item1).value = (sbyte)bytes[i];
                            }
                        }
                    }

                    ((Tag)Tag)?.RaisePropertyChanged("Data");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception($"Tag:{Tag != null}-----field:{_field != null}");
            }
        }

        public bool IsDisplay { set; get; } = true;

        public bool IsMonitorValueChanged { set; get; }

        public bool IsEnum => Enums != null;

        public List<string> Enums { set; get; }

        public ASTNodeList Parameters
        {
            set
            {
                if (value.OriginalNodeList != null)
                    _parameters = value.OriginalNodeList;
                else
                    _parameters = value;
            }
            get { return _parameters; }
        }
        
        public TextLocation TextLocation { get; private set; }
        
        private string _crossReferenceRegex = "";
        private string _crossReferenceParentRegex = "";

        public Tuple<int, int> GetLocation()
        {
            return new Tuple<int, int>(TextLocation.Line,TextLocation.Column);
        }

        public string GetCrossReferenceRegex()
        {
            if (string.IsNullOrEmpty(_crossReferenceRegex))
            {
                var astName = AstNode as ASTName;
                if (astName != null)
                    _crossReferenceRegex = ObtainValue.GetCrossReferenceRegexFromAstName(astName);
            }

            return _crossReferenceRegex;
        }

        public string GetCrossReferenceParentRegex()
        {
            if (string.IsNullOrEmpty(_crossReferenceParentRegex))
            {
                var astName = AstNode as ASTName;
                if (astName != null)
                    _crossReferenceParentRegex = ObtainValue.GetCrossReferenceParentRegexFromAstName(astName);
            }

            return _crossReferenceParentRegex;
        }

        public string GetParameterName(int index)
        {
            if (Parameters?.nodes?.Count > index)
            {
                var astName = Parameters.nodes[index] as ASTName;
                if (astName != null)
                {
                    return ObtainValue.GetAstName(astName);
                }

                var astNameAttr = Parameters.nodes[index] as ASTNameAddr;
                if (astNameAttr != null)
                {
                    return ObtainValue.GetAstName(astNameAttr.name);
                }
            }

            return "";
        }

        public override string ToString()
        {
            return Name;
        }

        public object Clone()
        {
            var variable=new VariableInfo(AstNode,_name,_routine,Offset,_codeText,TextLocation,IsInstr);
            variable.IsAOI = IsAOI;
            variable.IsRoutine = IsRoutine;
            variable.IsTask = IsTask;
            variable.IsInstr = IsInstr;
            variable.IsProgram = IsProgram;
            variable.IsModule =IsModule;
            variable.IsUnknown = IsUnknown;
            variable.IsNum = IsNum;
            variable.IsUseForJSR = IsUseForJSR;
            variable.IsJSR = IsJSR;
            variable.Enums = Enums;
            variable._field = _field;
            variable.DataType = DataType;
            variable.IsCorrect = IsCorrect;
            variable.IsDisplay = IsDisplay;
            variable.Module =Module;
            variable.LineOffset = LineOffset;
            variable._value = _value;
            variable._tag = _tag;
            variable._index = _index;
            variable.TargetDataType = TargetDataType;
            variable.DisplayStyle = DisplayStyle;
            variable._transformTable = _transformTable;
            variable.OriginalTag = OriginalTag;
            return variable;
        }

        public int GetOffsetsWithoutProgram()
        {
            var offset = 0;
            if (Name.StartsWith("\\"))
            {
                offset = Name.IndexOf(".") + 1;
            }

            return Offset + offset;
        }

        public bool IsInCode(int offset)
        {
            return Offset <= offset && Offset + Name.Length >= offset;
        }

        public double LineOffset { set; get; } = 0;

        public int Offset
        {
            get { return _offset; }
            private set
            {
                _offset = value;
            }
        }

        public string Name
        {
            private set { _name = value; }
            get
            {
                return _name?.TrimStart();
            }
        }

        public void RecoveryWhenHidden()
        {
            if (!IsCorrect)
            {
                Value = LastCorrectValue;
                IsCorrect = true;
                OnPropertyChanged("Value");
            }
        }

        public IDataType DataType { private set; get; }
        public DisplayStyle DisplayStyle { private set; get; } = DisplayStyle.Decimal;
        public IDataType TargetDataType { set; get; }

        public string Value
        {
            set
            {
                var element = Keyboard.FocusedElement;
                var textEditor = element == null
                    ? null
                    : VisualTreeHelpers.FindVisualParentOfType<TextEditor>((DependencyObject) element);
                if (textEditor == null)
                {
                    Controller.GetInstance().Log($"VariableInfo can change value(textEditor == null)");
                    OnPropertyChanged();
                    return;
                }
                else
                {
                    //if (!textEditor.TextArea.TextView.VariableInfos.Contains(this))
                    //{
                    //    OnPropertyChanged();
                    //    return;
                    //}

                    if (textEditor.IsFocusChanged)
                    {
                        OnPropertyChanged();
                        textEditor.IsFocusChanged = false;
                        return;
                    }
                }
                
                if (_value != value)
                {
                    if (_isTagSetValue)
                    {
                        _value = value;
                        return;
                    }

                    if (DisplayStyle == DisplayStyle.Ascii)
                    {
                        if (!(value.StartsWith("'") && value.EndsWith("'")))
                        {
                            value = $"'{value}'";
                        }
                    }

                    CheckValue(value);
                    //if (!IsCorrect)
                    //{
                    //    LastCorrectValue = _value;
                    //}
                    _value = AutoConvert(value);
                    if (IsCorrect)
                    {
                        SetTagValue(value);
                    }
                }

                if (IsCorrect)
                    OnPropertyChanged();
                else
                    Controller.GetInstance().Log($"VariableInfo can change value(IsCorrect)");
            }
            get
            {
                if (_value == null) return "";
                if (_value == "??" || _value == "{...}") return _value;
                if (!IsCorrect) return _value;
                string value = "";
                try
                {
                    value = FormatOp.FormatValue(_value, DisplayStyle, DataType);
                    if (value.Length > 36 && DataType.FamilyType == FamilyType.StringFamily)
                    {
                        value = $"'{value.Substring(1, 36)}...";
                    }
                }
                catch (Exception)
                {
                    Debug.Assert(false, "error convert");
                    return "??";
                }

                return value;
            }
        }

        public string EditValue
        {
            set
            {
                _editValue = value; 
                OnPropertyChanged();
            }
            get
            {
                return _editValue;
            }
        }
        
        public void Connection(IProgramModule program, Hashtable transformTable)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                if (Tag == null) return;
                _program = program;
                _value = "";
                _index = -1;
                if (Tag != null && IsDisplay)
                {
                    PropertyChangedEventManager.RemoveHandler(Tag, Tag_PropertyChanged, string.Empty);
                    if(_transformTable==null)
                        OriginalTag = Tag;
                }

                _transformTable = transformTable;
                if (transformTable == null)
                    OriginalTag = null;
                _field = null;
                _isTagSetValue = true;
                NotMonitorDimTagData();
                await SetValue();
                MonitorDimTagData();
                _isTagSetValue = false;

                if (Tag != null&&IsDisplay)
                    PropertyChangedEventManager.AddHandler(Tag, Tag_PropertyChanged, string.Empty);
            });
        }

        public ITag OriginalTag
        {
            private set { _originalTag = value; }
            get
            {
                if (_transformTable == null)
                    return Tag;
                return _originalTag;
            }
        }

        private Hashtable _transformTable;

        public ITag Tag
        {
            set
            {
                if (_tag != value)
                {
                    try
                    {
                        ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
                        {
                            if (IsDisplay)
                                PropertyChangedEventManager.RemoveHandler(_tag, Tag_PropertyChanged, string.Empty);
                            _tag = value;
                            _isTagSetValue = true;
                            NotMonitorDimTagData();
                            await SetValue();
                            MonitorDimTagData();
                            _isTagSetValue = false;
                            if (Tag != null && IsDisplay)
                                PropertyChangedEventManager.AddHandler(Tag, Tag_PropertyChanged, string.Empty);
                        });
                    }
                    catch (Exception)
                    {
                        //ignore
                    }
                    
                }
            }
            get { return _tag; }
        }

        public string LastCorrectValue { private set; get; }

        public bool IsCorrect
        {
            set
            {
                if (_isCorrect)
                {
                    LastCorrectValue = _editValue;
                }

                _isCorrect = value;
                if (!_isCorrect)
                {

                }
            }
            get { return _isCorrect; }
        }

        public void Verify(string value)
        {
            CheckValue(value);
        }

        public string Error { private set; get; }

        private string AutoConvert(string value)
        {
            try
            {
                if (DisplayStyle == DisplayStyle.Ascii)
                {
                    return DecimalToAscii(value);
                }
                else
                {
                    return AsciiToDecimal(value);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return value;
        }

        private string AsciiToDecimal(string value)
        {
            if (value.StartsWith("'") && value.EndsWith("'"))
            {
                return ValueConverter.ConvertValue(value, DisplayStyle.Ascii, DisplayStyle, DataType.BitSize,
                    ValueConverter.SelectIntType(DataType));
            }

            return value;
        }

        private string DecimalToAscii(string value)
        {
            if (value.StartsWith("'") && value.EndsWith("'"))
            {
                return value;
            }

            return ValueConverter.ConvertValue(value, DisplayStyle.Decimal, DisplayStyle.Ascii, DataType.BitSize,
                ValueConverter.SelectIntType(DataType));
        }

        private void CheckValue(string value)
        {
            try
            {
                int flag = 0;
                // _isCorrect = true;
                Error = "";
                if (string.IsNullOrEmpty(value)) flag = 1;
                value = FormatOp.ConvertValue(value, DisplayStyle, DataType);
                if ("1.#QNAN".Equals(value) || "-1.#QNAN".Equals(value))
                {
                    return;
                }

                if ("1.$".Equals(value) || "-1.$".Equals(value))
                {

                }
                else
                {
                    value = DisplayStyle == DisplayStyle.Ascii
                        ? value
                        : ValueConverter.ConvertValue(value, DisplayStyle.Decimal, DisplayStyle, DataType.BitSize,
                            ValueConverter.SelectIntType(_field));
                }

                if (flag < 1)
                {
                    //value = ConvertValue(value);
                    if (DataType != null)
                    {
                        if (DataType is SINT)
                        {
                            ValueConverter.CheckValueOverflow(DisplayStyle, value, ref flag, 127);
                        }
                        else if (DataType is INT)
                        {
                            ValueConverter.CheckValueOverflow(DisplayStyle, value, ref flag, 32767);
                        }
                        else if (DataType is DINT)
                        {
                            ValueConverter.CheckValueOverflow(DisplayStyle, value, ref flag, Int32.MaxValue);
                        }
                        else if (DataType is LINT)
                        {
                            ValueConverter.CheckValueOverflow(DisplayStyle, value, ref flag, Int64.MaxValue);
                        }
                        else if (DataType is BOOL)
                        {
                            if (value != "0" && value != "1") flag = 3;
                        }
                        else if (DataType is REAL)
                        {
                            Regex regex = new Regex(@"^(\-|\+)?(([0-9]+(\.[0-9]+([Ee](\+|\-)?[0-9]+)?)?)|(1\.\$))$");
                            if (flag == 0 && !regex.IsMatch(value)) flag = 2;
                            ValueConverter.CheckValueOverflow(DisplayStyle, value, ref flag, float.MaxValue);
                        }
                    }
                }

                if (flag > 0)
                {
                    //string reason = $"Failed to set tag \"\" default value.";
                    Error = "Signed value overflow.";
                    if (flag == 1 || flag == 2) Error = "String invalid.";

                    IsCorrect = false;
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                IsCorrect = false;
                return;
            }

            IsCorrect = true;
        }
     
        private bool _isTagSetValue = false;

        private void SetTagValue(string textValue)
        {
            textValue = FormatOp.ConvertValue(textValue, DisplayStyle, DataType);
            if (Controller.GetInstance().IsOnline)
            {
                if (DisplayStyle == DisplayStyle.Ascii)
                    textValue = ValueConverter.ConvertValue(textValue, DisplayStyle, DisplayStyle.Decimal,
                        DataType.BitSize, ValueConverter.SelectIntType(_field));
                Task.Run(async delegate
                {
                    await TaskScheduler.Default;

                    if (_transformTable != null)
                    {
                        await Controller.GetInstance().SetTagValueToPLC(Tag, _connectionName, textValue);
                    }
                    else
                        await Controller.GetInstance().SetTagValueToPLC(Tag, Name, textValue);
                });
                return;
            }

            if (_field == null)
            {
                Debug.Assert(false);
                return;
            }
            //textValue = FormatOp.FormatValue(textValue, DisplayStyle, DataType);

            var boolArray = _field as BoolArrayField;
            if (boolArray != null)
            {
                boolArray.Set(_index, int.Parse(textValue) == 1);
                ((Tag)Tag).RaisePropertyChanged("Data");
                return;
            }

            var boolField = _field as BoolField;
            if (boolField != null)
            {
                boolField.value = ValueConverter.ToByte(textValue, DisplayStyle);
                ((Tag)Tag).RaisePropertyChanged("Data");
                return;
            }

            Int8Field int8Field = _field as Int8Field;
            if (int8Field != null)
            {
                if (_index > -1)
                    int8Field.SetBitValue(_index, textValue != "0");
                else
                    int8Field.value = ValueConverter.ToSByte(textValue, DisplayStyle);
                ((Tag)Tag).RaisePropertyChanged("Data");
                return;
            }

            Int16Field int16Field = _field as Int16Field;
            if (int16Field != null)
            {
                if (_index > -1)
                    int16Field.SetBitValue(_index, textValue != "0");
                else
                    int16Field.value = ValueConverter.ToShort(textValue, DisplayStyle);
                ((Tag)Tag).RaisePropertyChanged("Data");
                return;
            }

            Int32Field int32Field = _field as Int32Field;
            if (int32Field != null)
            {
                if (_index > -1)
                    int32Field.SetBitValue(_index, textValue != "0");
                else
                    int32Field.value = ValueConverter.ToInt(textValue, DisplayStyle);
                ((Tag)Tag).RaisePropertyChanged("Data");
                return;
            }

            Int64Field int64Field = _field as Int64Field;
            if (int64Field != null)
            {
                if (_index > -1)
                    int64Field.SetBitValue(_index, textValue != "0");
                else
                    int64Field.value = ValueConverter.ToLong(textValue, DisplayStyle);
                ((Tag)Tag).RaisePropertyChanged("Data");
                return;
            }

            RealField realField = _field as RealField;
            if (realField != null)
            {
                if (_index > -1)
                    realField.SetBitValue(_index, textValue != "0");
                else
                    realField.value = ValueConverter.ToFloat(textValue);
                ((Tag)Tag).RaisePropertyChanged("Data");
                return;
            }

            LRealField lRealField = _field as LRealField;
            if (lRealField != null)
            {
                if (_index > -1)
                    lRealField.SetBitValue(_index, textValue != "0");
                else
                    lRealField.value = ValueConverter.ToLong(textValue, DisplayStyle);
                ((Tag)Tag).RaisePropertyChanged("Data");
                return;
            }

            Debug.Assert(false, "error field.");
        }

        private void Tag_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Description" || e.PropertyName == "ExternalAccess" ||
                e.PropertyName == "IsConstant" || e.PropertyName == "Usage" ||
                e.PropertyName == "IsRequired" || e.PropertyName == "IsVisible")
            {
                return;
            }

            if (e.PropertyName == "Data")
            {
                //await SetValue();
            }
            else
            {
                _field = null;
            }

            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await SetValue();
            });
        }
        
        private void MonitorDimTagData()
        {
            if(IsDisplay)
            foreach (var dimTag in _dimTags)
            {
                PropertyChangedEventManager.AddHandler(dimTag, Tag_PropertyChanged, "");
            }
        }

        private void NotMonitorDimTagData()
        {
            if (!_dimTags.Any()) return;
            foreach (var dimTag in _dimTags)
            {
                PropertyChangedEventManager.RemoveHandler(dimTag, Tag_PropertyChanged, "");
            }

            _dimTags.Clear();
        }

        private static IField GetArrayField(IField field, int index)
        {
            var boolArray = field as BoolArrayField;
            if (boolArray != null)
            {
                return boolArray;
            }

            var array = field as ArrayField;
            if (array != null)
            {
                if (array.fields.Count <= index||index<0) return null;
                return array.fields[index].Item1;
            }

            //add other array
            Debug.Assert(false);
            return null;
        }

        private string GetTagValue(string specifier)
        {
            IProgramModule program;
            if (specifier.StartsWith("\\"))
            {
                var programName = specifier.Split('.')[0].Substring(1);
                program = Controller.GetInstance().Programs[programName];
                specifier = specifier.Substring(programName.Length + 2);
                Debug.Assert(program != null);
            }
            else
            {
                program = _program;
            }
            var astName = ObtainValue.GetLoadTag(specifier, program, _transformTable);

            if (astName == null) return null;
            _tag = astName.Tag;
            if (_tag == null) return "{...}";

            if (_transformTable != null)
            {
                _connectionName = ObtainValue.GetAstName(astName);
            }

            var count = astName.loaders.nodes.Count;
            if (program is AoiDefinition && Tag.Usage == Usage.InOut && _transformTable == null)
            {
                if (astName.Expr.type.IsNumber || astName.Expr.type.IsBool)
                    return "??";
                return "{...}";
            }

            if (program is AoiDefinition && Tag.Usage != Usage.InOut) count--;
            if (count == 1)
            {
                DataType = ((Tag)Tag).DataWrapper.DataTypeInfo.DataType;
                return GetFiledValue(((Tag)Tag).DataWrapper.Data, _tag.DisplayStyle);
            }
            else
            {
                var isAoi = program is AoiDefinition;

                if (isAoi && Tag.Usage != Usage.InOut)
                {
                    var member = ObtainValue.GetAoiMember(astName, program);
                    return GetValue(member?.DataTypeInfo.DataType, ((Tag)Tag).DataWrapper.Data, astName, 2,
                        member?.DisplayStyle);
                }
                else
                {
                    return GetValue(_tag.DataTypeInfo.DataType, ((Tag)Tag).DataWrapper.Data, astName, isAoi ? 2 : 1,
                        _tag.DisplayStyle);
                }
            }
        }

        private string _connectionName = "";
        private bool _isEnabled = true;
        private bool _isCorrect = true;
        private int _offset;
        private readonly List<Tag> _dimTags = new List<Tag>();
        
        private ITag _originalTag;
        private ASTNodeList _parameters;
        private string _editValue;

        private string GetValue(IDataType dataType, IField field, ASTName astName, int index,
            DisplayStyle? displayStyle)
        {
            if (dataType == null) return null;
            if (index == astName.loaders.nodes.Count())
            {
                DataType = dataType;
                return GetFiledValue(field, displayStyle);
            }

            var node = astName.loaders.nodes[index];
            var array = node as ASTArrayLoader;
            if (array != null)
            {
                var dim = ObtainValue.GetDim(array, _program, _transformTable, _dimTags);
                // field = (field as ArrayField)?.fields[dim].Item1;
                field = GetArrayField(field, dim);
                if (field == null) return "??";
                if (field is BoolArrayField) _index = dim;
                index++;
                return GetValue(dataType, field, astName, index, displayStyle);
            }

            var tagOffset = node as ASTTagOffset;
            if (tagOffset != null)
            {
                var byteOffset = tagOffset.offset;
                index++;
                var compositiveType = dataType as CompositiveType;
                if (compositiveType == null)
                {
                    return null;
                }

                Debug.Assert(compositiveType != null);
                DataTypeMember member;
                var result = ObtainValue.TryGetBitMember(astName, compositiveType, index, byteOffset, out member);
                if (!result)
                    member = (DataTypeMember)compositiveType.TypeMembers.FirstOrDefault(
                        m => m.ByteOffset == byteOffset);
                Debug.Assert(member != null);
                displayStyle = member.DisplayStyle;
                return GetValue(member.DataTypeInfo.DataType,
                    (field as ICompositeField)?.fields[member.FieldIndex].Item1, astName, index, displayStyle);
            }

            var integer = node as ASTInteger;
            if (integer != null)
            {
                index++;
                Debug.Assert(astName.loaders.nodes.Count == index);
                DataType = astName.type;
                DisplayStyle = (DisplayStyle)displayStyle;
                if (field is BoolField)
                    return GetFiledValue(field,
                        displayStyle == DisplayStyle.Ascii ? DisplayStyle.Decimal : displayStyle);
                //return field.GetBitValue((int) integer.value) ? "1" : "0";
                _index = (int)integer.value;
                return GetFiledValue(field, displayStyle == DisplayStyle.Ascii ? DisplayStyle.Decimal : displayStyle);
            }

            var bitAstName = node as ASTName;
            if (bitAstName != null)
            {
                index++;
                var name = ObtainValue.GetAstName(bitAstName);
                DataType = astName.type;
                var bit = GetTagValue(name);
                _index = int.Parse(bit);
                return GetFiledValue(field, displayStyle == DisplayStyle.Ascii ? DisplayStyle.Decimal : displayStyle);
            }

            var call = node as ASTCall;
            if (call != null)
            {
                //TODO(zyl):get value
                return null;
            }

            var astBinArithOp = node as ASTBinArithOp;
            if (astBinArithOp != null)
            {
                int value=0;
                var res = ObtainValue.TryGetASTBinArithOpValue(astBinArithOp,_program,null,_transformTable, ref value);
                if (res) return value.ToString();
                return null;
            }
            Debug.Assert(false, node.ToString());
            return null;
        }

        public string GetStringData()
        {
            return ObtainValue.GetStringData(_field, DataType);
        }

        private string GetFiledValue(IField field, DisplayStyle? displayStyle)
        {
            if (field == null) return null;
            if (field is BoolArrayField)
            {
                if (_index < 0)
                    return null;
                if (displayStyle != null)
                    DisplayStyle = (DisplayStyle)displayStyle;
                _field = field;
                if (_index >= ((BoolArrayField)field).getBitCount()) return "??";
                return ((BoolArrayField)field).Get(_index) ? "1" : "0";
            }

            if (!(field is UserDefinedField))
                _field = field;
            if (DataType.FamilyType == FamilyType.StringFamily)
            {
                DisplayStyle = DisplayStyle.Ascii;
                _field = field;
                return GetStringData();
            }

            if (displayStyle != null)
            {
                DisplayStyle = (DisplayStyle)displayStyle;
                return FormatOp.ConvertField(field, (DisplayStyle)displayStyle, _index);
            }

            var boolField = field as BoolField;
            if (boolField != null)
            {
                return boolField.value.ToString();
            }

            Int8Field int8Field = field as Int8Field;
            if (int8Field != null)
            {
                return int8Field.value.ToString();
            }

            Int16Field int16Field = field as Int16Field;
            if (int16Field != null)
            {
                return int16Field.value.ToString();
            }

            Int32Field int32Field = field as Int32Field;
            if (int32Field != null)
            {
                return int32Field.value.ToString();
            }

            Int64Field int64Field = field as Int64Field;
            if (int64Field != null)
            {
                return int64Field.value.ToString();
            }

            RealField realField = field as RealField;
            if (realField != null)
            {
                DisplayStyle = DisplayStyle.Float;
                return realField.value.ToString();
            }

            LRealField lRealField = field as LRealField;
            if (lRealField != null)
            {
                DisplayStyle = DisplayStyle.Float;
                return lRealField.value.ToString();
            }

            Debug.Assert(false, "error field.");
            return null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Clean()
        {
            PropertyChangedEventManager.RemoveHandler(Tag, Tag_PropertyChanged, "");
            StopTimer();
            Notifications.DisconnectConsumer(this);
            _routine = null;
            _program = null;
            _field = null;
            _value = null;
            NotMonitorDimTagData();
            _tag = null;
            DataType = null;
        }

        protected void Dispose(bool disposing)
        {
            if (_disposed) return;
            Clean();
            _disposed = true;
        }
    }

    public static class ExtendList
    {
        public static void Remove<T>(this IList<T> list, Func<T, bool> func)
        {
            for (int i = list.Count - 1; i > -1; i--)
            {
                if (func(list[i])) list.RemoveAt(i);
            }
        }
    }
}
