using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Components.Controls;
using ICSStudio.Dialogs.BrowseString;
using ICSStudio.Dialogs.BrowseString.RichTextBoxExtend;
using ICSStudio.Dialogs.Filter;
using ICSStudio.Gui.Annotations;
using ICSStudio.Gui.Utils;
using ICSStudio.Gui.View;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;

namespace ICSStudio.StxEditor.Menu.Dialog.ViewModel
{
    internal class ParamInfo : INotifyPropertyChanged
    {
        private string _param = "";

        public ParamInfo()
        {
        }

        public ParamInfo(int offset, string param)
        {
            Offset = offset;
            Param = param;
        }

        public virtual string Param
        {
            set
            {
                _param = value;
                OnPropertyChanged();
            }
            get { return _param ?? ""; }
        }

        public int Offset { set; get; } = -1;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    internal sealed class Argument : ParamInfo
    {
        private string _param = "";
        private IProgramModule _parent;
        private IDataType _targetDataType;
        private Visibility _errorVisibility;
        private string _selectedArgument;
        private Visibility _stringButtonVisibility = Visibility.Collapsed;
        private Thickness _padding;
        private string _value;
        private readonly bool _isEnum;
        private Hashtable _transformTable;

        public Argument(string parameter, string argumentTag, IDataType targetDataType, int offset,
            IProgramModule parent, Hashtable transformTable, bool isEnum = false)
        {
            Parameter = parameter;
            _isEnum = isEnum;
            _targetDataType = targetDataType;
            _parent = parent;
            _transformTable = transformTable;
            Offset = offset;
            Param = argumentTag;
            StringBrowseCommand = new RelayCommand(ExecuteStringBrowseCommand);
            NameFilterCommand = new RelayCommand<Button>(ExecuteNameFilterCommand);
            NameFilterPopup = new NameFilterPopup(parent, "", false);
            NameFilterPopup.FilterViewModel.Name = argumentTag;
            _selectedArgument = argumentTag;
            PropertyChangedEventManager.AddHandler(NameFilterPopup.FilterViewModel, FilterViewModel_PropertyChanged,
                string.Empty);
            NameFilterPopup.Closed += NameFilterPopup_Closed;
        }

        private void NameFilterPopup_Closed(object sender, EventArgs e)
        {

            ((FastAutoCompleteTextBox)NameFilterPopup.PlacementTarget).Focus();
        }

        public void Clean()
        {
            PropertyChangedEventManager.RemoveHandler(NameFilterPopup.FilterViewModel, FilterViewModel_PropertyChanged,
                string.Empty);
            NameFilterPopup = null;
        }

        public void SetEnumList(List<string> enums)
        {
            EnumList = enums;
            ComboboxVisibility = Visibility.Visible;
            TagFilterVisibility = Visibility.Collapsed;
            SelectedArgument = Param;
            IsEnabled = false;
            OnPropertyChanged("TagFilterVisibility");
            OnPropertyChanged("ComboboxVisibility");
        }

        public List<string> EnumList { private set; get; }
        public Visibility TagFilterVisibility { set; get; } = Visibility.Visible;
        public Visibility ComboboxVisibility { set; get; } = Visibility.Collapsed;
        public RelayCommand<Button> NameFilterCommand { set; get; }

        private void ExecuteNameFilterCommand(Button sender)
        {
            var parentGrid = VisualTreeHelpers.FindVisualParentOfType<Grid>(sender);
            var autoCompleteBox = VisualTreeHelpers.FindFirstVisualChildOfType<AutoCompleteBox>(parentGrid);
            //var stackPanel = VisualTreeHelpers.FindVisualParentOfType<StackPanel>(autoCompleteBox);

            if (!NameFilterPopup.IsOpen)
                NameFilterPopup.ResetAll(_parent.Name, autoCompleteBox);
            NameFilterPopup.IsOpen = !NameFilterPopup.IsOpen;
        }

        public NameFilterPopup NameFilterPopup { get; private set; }

        private void FilterViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                Param = ArgumentName;
                OnPropertyChanged("ArgumentName");
                OnPropertyChanged("Param");
            }

            if (e.PropertyName == "Hide")
            {
                NameFilterPopup.IsOpen = false;
            }
        }

        public string Parameter { get; }
        public Dictionary<ITag, TagNameNode> ArgumentList => NameFilterPopup.FilterViewModel.AutoCompleteData;

        public string ArgumentName
        {
            set
            {
                NameFilterPopup.FilterViewModel.Name = value;
                Param = value;
            }
            get { return NameFilterPopup.FilterViewModel.Name; }
        }

        public override string Param
        {
            set
            {
                _param = value;
                OnPropertyChanged();
                if (_isEnum)
                {
                    StringButtonVisibility = Visibility.Collapsed;
                }
                else if (TagFilterVisibility == Visibility.Visible)
                {
                    _needSetSource = false;
                    try
                    {
                        VerifyString();
                        Tag tag = null;
                        Value = ObtainValue.GetTagValue(value, _parent, _transformTable,ref tag);
                        Verify();
                    }
                    catch (Exception)
                    {
                        Value = "";
                        ErrorVisibility = Visibility.Visible;
                    }

                    _needSetSource = true;
                    if (StringButtonVisibility != Visibility.Visible)
                        IsEnabled = !string.IsNullOrEmpty(Value);
                    OnPropertyChanged("Value");
                }
            }
            get { return _param ?? ""; }
        }

        public Tag GetTag()
        {
            return ObtainValue.NameToTag(Param, _transformTable, _parent)?.Item1 as Tag;
        }

        public Thickness Padding
        {
            set
            {
                _padding = value;
                OnPropertyChanged();
            }
            get { return _padding; }
        }

        public string SelectedArgument
        {
            set
            {
                _selectedArgument = value;
                _param = value;
                if (EnumList.Contains(value))
                {
                    ErrorVisibility = Visibility.Collapsed;
                }
                else
                {
                    ErrorVisibility = Visibility.Visible;
                }

                IsEnabled = false;
                OnPropertyChanged("Param");
            }
            get { return _selectedArgument; }
        }

        public Visibility ErrorVisibility
        {
            set
            {
                _errorVisibility = value;
                OnPropertyChanged("ErrorVisibility");
            }
            get { return _errorVisibility; }
        }

        public bool IsEnabled
        {
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
            get { return _isEnabled; }
        }

        public string Tip
        {
            get
            {
                var info = "";
                var expectDataType = _targetDataType as ExpectType;
                var exceptDataType = _targetDataType as ExceptType;
                if (expectDataType != null)
                {
                    info = expectDataType.ToString();
                }
                else if (exceptDataType != null)
                {
                    info = exceptDataType.ToString();
                }
                else
                {
                    info = _targetDataType?.Name;
                }

                return $"Parameter:{Parameter}\nData Type:{info}";
            }
        }

        public string Value
        {
            set
            {
                if (_needSetSource)
                {
                    var field = ObtainValue.GetTagField(Param, _parent, _transformTable).Item1;
                    var result = CheckAndSetValue(value, field);
                    if (!result) return;
                }

                _value = value;
                OnPropertyChanged();
            }
            get { return _value; }
        }

        private bool CheckAndSetValue(string value, IField field)
        {
            Debug.Assert(field != null);
            if (StringButtonVisibility == Visibility.Visible)
            {
                var str = FormatOp.FormatSpecial(value);
                var bytes = Encoding.ASCII.GetBytes(str);
                var stringField = field as STRINGField;
                ArrayField arr;
                Int32Field len;
                if (stringField != null)
                {
                    arr = stringField.fields[1].Item1 as ArrayField;
                    len = stringField.fields[0].Item1 as Int32Field;
                }
                else
                {
                    arr = (field as UserDefinedField).fields[1].Item1 as ArrayField;
                    len = (field as UserDefinedField).fields[0].Item1 as Int32Field;
                }

                if (Controller.GetInstance().IsOnline)
                {
                    Task.Run(async delegate
                    {
                        await TaskScheduler.Default;
                        var tag = GetTag();

                        await Controller.GetInstance()
                            .SetTagValueToPLC(tag, $"{tag.Name}.LEN", bytes.Length.ToString());
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            await Controller.GetInstance().SetTagValueToPLC(tag, $"{tag.Name}.Data[{i}]",
                                ((sbyte) bytes[i]).ToString());
                        }
                    });
                }
                else
                {
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        (arr.fields[i].Item1 as Int8Field).value = (sbyte) bytes[i];
                    }

                    len.value = bytes.Length;
                    GetTag()?.RaisePropertyChanged("Data");
                }

                return true;
            }

            if (!IsNumber(value))
            {
                MessageBox.Show($"Failed to set tag \"{Param}\" value.\nString invalid.", "ICS Studio",
                    MessageBoxButton.OK);
                return false;
            }

            if (Controller.GetInstance().IsOnline)
            {
                Task.Run(async delegate
                {
                    await TaskScheduler.Default;
                    await Controller.GetInstance().SetTagValueToPLC(GetTag(), Param, value);
                });
                return true;
            }

            var special = field as SpecialField;
            var boolFiled = field as BoolField;
            if (special != null || boolFiled != null)
            {
                if (value == "1" || value == "0")
                {
                    if (special != null)
                        special.SetValue(value == "1");
                    else
                        boolFiled.value = (byte) (value == "1" ? 1 : 0);
                    GetTag()?.RaisePropertyChanged("Data");
                    return true;
                }

                return false;
            }

            Int8Field int8Field = field as Int8Field;
            if (int8Field != null)
            {
                sbyte result;
                if (sbyte.TryParse(value, out result))
                {
                    int8Field.value = result;
                }
                else
                {
                    int8Field.value = sbyte.MaxValue;
                }

                GetTag()?.RaisePropertyChanged("Data");
                return true;
            }

            Int16Field int16Field = field as Int16Field;
            if (int16Field != null)
            {
                Int16 result;
                if (Int16.TryParse(value, out result))
                {
                    int16Field.value = result;
                }
                else
                {
                    int16Field.value = Int16.MaxValue;
                }

                GetTag()?.RaisePropertyChanged("Data");
                return true;
            }

            Int32Field int32Field = field as Int32Field;
            if (int32Field != null)
            {
                Int32 result;
                if (Int32.TryParse(value, out result))
                {
                    int32Field.value = result;
                }
                else
                {
                    int32Field.value = Int32.MaxValue;
                }

                GetTag()?.RaisePropertyChanged("Data");
                return true;
            }

            Int64Field int64Field = field as Int64Field;
            if (int64Field != null)
            {
                Int64 result;
                if (Int64.TryParse(value, out result))
                {
                    int64Field.value = result;
                }
                else
                {
                    int64Field.value = Int64.MaxValue;
                }

                GetTag()?.RaisePropertyChanged("Data");
                return true;
            }

            RealField realField = field as RealField;
            if (realField != null)
            {
                float result;
                if (float.TryParse(value, out result))
                {
                    realField.value = result;
                    GetTag()?.RaisePropertyChanged("Data");
                    return true;
                }
                else
                {
                    MessageBox.Show($"Failed to set tag \"{Param}\" value.\nFloating-point overflow occured.",
                        "ICS Studio", MessageBoxButton.OK);
                    return false;
                }
            }

            LRealField lRealField = field as LRealField;
            if (lRealField != null)
            {
                double result;
                if (double.TryParse(value, out result))
                {
                    lRealField.value = result;
                    GetTag()?.RaisePropertyChanged("Data");
                    return true;
                }
                else
                {
                    MessageBox.Show($"Failed to set tag \"{Param}\" value.\nFloating-point overflow occured.",
                        "ICS Studio", MessageBoxButton.OK);
                    return false;
                }
            }

            return false;
        }

        private bool IsNumber(string val)
        {
            var regex = new Regex(
                @"^(\-|\+)?(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*)([eE](\-|\+)?[0-9]+)?)$");
            return regex.IsMatch(val);
        }

        private bool _needSetSource = true;
        private bool _isEnabled;
        public string Description { set; get; }

        public void Verify()
        {
            if (string.IsNullOrEmpty(Parameter)) return;
            var info = ObtainValue.GetTargetDataTypeInfo(Param, _parent, _transformTable);
            IDataType currentDataType;
            if (info.Dim2 > 0)
            {
                currentDataType = new ArrayTypeNormal(info.DataType);
            }
            else if (info.Dim1 > 0)
            {
                currentDataType = new ArrayTypeDimOne(info.DataType);
            }
            else
            {
                currentDataType = info.DataType;
            }

            if (currentDataType == null)
            {
                ErrorVisibility = Visibility.Visible;
                return;
            }

            var flag = true;
            var expectDataType = _targetDataType as ExpectType;
            var exceptDataType = _targetDataType as ExceptType;
            if (expectDataType != null)
            {
                flag = expectDataType.IsMatched(currentDataType);
            }
            else if (exceptDataType != null)
            {
                flag = exceptDataType.IsMatched(currentDataType);
            }
            else
            {
                flag = _targetDataType.Equal(currentDataType);
            }

            ErrorVisibility = flag ? Visibility.Hidden : Visibility.Visible;
        }

        public Visibility StringButtonVisibility
        {
            set
            {
                _stringButtonVisibility = value;
                if (value == Visibility.Visible)
                {
                    IsEnabled = false;
                    Padding = new Thickness(30, 0, 0, 0);
                }
                else Padding = new Thickness();

                OnPropertyChanged();
            }
            get { return _stringButtonVisibility; }
        }

        public RelayCommand StringBrowseCommand { get; }

        private void ExecuteStringBrowseCommand()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            var dataType = ObtainValue.GetTargetDataTypeInfo(Param, _parent, _transformTable);
            var message = new Message(null,Value, Param);
            PropertyChangedEventManager.AddHandler(message, Message_PropertyChanged, "Text");
            //var vm = new BrowseStringViewModel(DataTypeL, message);
            var dialog = new BrowseStringDialog(dataType.DataType, message);
            //dialog.DataContext = vm;
            var uiShell = (IVsUIShell) Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsUIShell));
            if (dialog.ShowDialog(uiShell) ?? false)
            {

            }

            PropertyChangedEventManager.RemoveHandler(message, Message_PropertyChanged, "Text");
        }

        private void VerifyString()
        {
            var dataTypeInfo = ObtainValue.GetTargetDataTypeInfo(Param, _parent, _transformTable);
            if (dataTypeInfo.DataType.FamilyType == FamilyType.StringFamily)
            {
                StringButtonVisibility = Visibility.Visible;
            }
            else
            {
                StringButtonVisibility = Visibility.Collapsed;
            }
        }

        private void Message_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Text")
            {
                Value = ((Message) sender).Text;
            }
        }
    }
}
