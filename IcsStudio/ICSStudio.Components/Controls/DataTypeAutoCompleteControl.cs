using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using ICSStudio.Components.Model;
using ICSStudio.Interfaces.DataType;

namespace ICSStudio.Components.Controls
{
    public class DataTypeAutoCompleteControl : ValidateNameControl
    {
        public static readonly DependencyProperty DisableMultiDimensionalArraysProperty =
            DependencyProperty.Register(nameof(DisableMultiDimensionalArrays), typeof(bool),
                typeof(DataTypeAutoCompleteControl));

        public static readonly DependencyProperty StructureMembersProperty =
            DependencyProperty.Register(nameof(StructureMembers), typeof(bool), typeof(DataTypeAutoCompleteControl));

        // ReSharper disable FieldCanBeMadeReadOnly.Local
        private static  Regex _validChars = new Regex("^[a-zA-Z0-9,_[\\]:]*$", RegexOptions.Compiled);
        private static  Regex _inValidChars = new Regex("[^a-zA-Z0-9,_\\[\\]:]+", RegexOptions.Compiled);
        private static  Regex _validCharsNoCommas = new Regex("^[a-zA-Z0-9_[\\]:]*$", RegexOptions.Compiled);
        private static  Regex _inValidCharsNoCommas = new Regex("[^a-zA-Z0-9_\\[\\]:]+", RegexOptions.Compiled);
        private static  Regex _validCharsPeriodButNoCommas = new Regex("^[a-zA-Z0-9_[\\]:\\.]*$", RegexOptions.Compiled);
        private static Regex _inValidCharsPeriodButNoCommas = new Regex("[^a-zA-Z0-9_\\[\\]:\\.]+", RegexOptions.Compiled);
        // ReSharper restore FieldCanBeMadeReadOnly.Local

        public static readonly DependencyProperty ChoiceListProperty = DependencyProperty.Register(nameof(ChoiceList),
            typeof(IEnumerable<string>), typeof(DataTypeAutoCompleteControl),
            new UIPropertyMetadata(null));

        public static readonly DependencyProperty ChoiceListWithMembersProperty =
            DependencyProperty.Register(nameof(ChoiceListWithMembers), typeof(IEnumerable<IDataType>),
                typeof(DataTypeAutoCompleteControl),
                new UIPropertyMetadata(null));

        public static readonly DependencyProperty ProcessTextInputWhenReadOnlyProperty =
            DependencyProperty.Register(nameof(ProcessTextInputWhenReadOnly), typeof(bool),
                typeof(DataTypeAutoCompleteControl), new UIPropertyMetadata(false));

        private IEnumerable<string> _autoCompleteChoices;
#pragma warning disable 414
        private IEnumerable<IDataType> _autoCompleteChoicesWithMembers;
#pragma warning restore 414
        private ComponentLookupHelper _lookupHelper;

        public DataTypeAutoCompleteControl()
        {

        }

        public bool DisableMultiDimensionalArrays
        {
            get { return (bool) GetValue(DisableMultiDimensionalArraysProperty); }
            set { SetValue(DisableMultiDimensionalArraysProperty, value); }
        }

        public bool StructureMembers
        {
            get { return (bool) GetValue(StructureMembersProperty); }
            set { SetValue(StructureMembersProperty, value); }
        }

        protected override Regex ValidChars
        {
            get
            {
                if (StructureMembers)
                    return _validCharsPeriodButNoCommas;
                return DisableMultiDimensionalArrays
                    ? _validCharsNoCommas
                    : _validChars;
            }
        }

        protected override Regex InvalidChars
        {
            get
            {
                if (StructureMembers)
                    return _inValidCharsPeriodButNoCommas;
                return DisableMultiDimensionalArrays
                    ? _inValidCharsNoCommas
                    : _inValidChars;
            }
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (_lookupHelper != null)
            {
                _lookupHelper.Dispose();
                _lookupHelper = null;
            }

            _autoCompleteChoices = null;
            _autoCompleteChoicesWithMembers = null;
            base.OnLostKeyboardFocus(e);
        }

        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            if (IsReadOnly && !ProcessTextInputWhenReadOnly)
            {
                OnPreviewTextInput(e);
            }
            else
            {
                if (!e.Handled && (!string.IsNullOrEmpty(e.Text) && ValidateText(e.Text)))
                    e.Handled = AutoComplete(e.Text);
                base.OnTextInput(e);
            }
        }

        private bool Lookup(ref string text)
        {
            CultureInfo culture = new CultureInfo("en-US");
            IEnumerable<string> strings;
            if (ChoiceList != null)
            {
                strings = ChoiceList;
            }
            else
            {
                if (_lookupHelper == null)
                {
                    _lookupHelper = new ComponentLookupHelper();
                    _autoCompleteChoices = _lookupHelper.LookupDataTypes();
                }

                strings = _autoCompleteChoices;
            }

            foreach (string str in strings)
            {
                if (str.StartsWith(text, true, culture))
                {
                    text = str;
                    return true;
                }
            }

            return false;
        }

        private bool LookupWithMember(ref string text)
        {
            throw new NotImplementedException();

            //IEnumerable<IDataType> source = (this.ChoiceListWithMembers ?? this._autoCompleteChoicesWithMembers) ??
            //                                (this._autoCompleteChoicesWithMembers = ComponentLookupHelper
            //                                    .GetDataTypes<DataTypeItem, DataTypeItem>());
            //string[] items = text.Split('.');
            //if (items.Count() != 1)
            //    return this.LookupWithMember(items, ref text);
            //string name = source.FirstOrDefault<IDataType>((Func<IDataType, bool>) (dt =>
            //    dt.Name.StartsWith(items[0], StringComparison.OrdinalIgnoreCase)))?.Name;
            //if (string.IsNullOrEmpty(name))
            //    return false;
            //text = name;
            //return true;
        }

        private bool LookupWithMember(string[] items, ref string text)
        {
            throw new NotImplementedException();
            //IDataType dataType =
            //    (this.ChoiceListWithMembers ?? this._autoCompleteChoicesWithMembers).FirstOrDefault<IDataType>(
            //        (Func<IDataType, bool>) (dt => dt.Name.Equals(items[0], StringComparison.OrdinalIgnoreCase)));
            //if (dataType == null)
            //    return false;
            //IEnumerable<IDataTypeMember> source = dataType.Members;
            //List<string> memberItems = new List<string>();
            //foreach (string str in items)
            //{
            //    int num = str.IndexOf('[');
            //    if (num > 0)
            //    {
            //        memberItems.Add(str.Substring(0, num));
            //        memberItems.Add(str.Substring(num, str.Length - num));
            //    }
            //    else
            //        memberItems.Add(str);
            //}

            //string str1 = string.Empty;
            //int num1 = memberItems.Count<string>() - 1;
            //for (int i = 1; i < memberItems.Count<string>(); i++)
            //{
            //    if (i == num1)
            //    {
            //        string str2 =
            //            source.FirstOrDefault<IDataTypeMember>((Func<IDataTypeMember, bool>) (member =>
            //                    member.Name.StartsWith(memberItems[i], StringComparison.OrdinalIgnoreCase))) is
            //                DataTypeItem
            //                dataTypeItem
            //                ? dataTypeItem.FullName
            //                : (string) null;
            //        if (string.IsNullOrEmpty(str2))
            //            return false;
            //        str1 = str2;
            //    }
            //    else
            //    {
            //        // ISSUE: explicit non-virtual call
            //        source = source.FirstOrDefault<IDataTypeMember>((Func<IDataTypeMember, bool>) (dt =>
            //                dt.Name.Equals(memberItems[i], StringComparison.OrdinalIgnoreCase))) is DataTypeItem
            //            dataTypeItem
            //            ? __nonvirtual(dataTypeItem.Members)
            //            : (IEnumerable<IDataTypeMember>) null;
            //        if (source == null || dataTypeItem.Dimension > 0 && dataTypeItem.FullName.Length < text.Length &&
            //            text[dataTypeItem.FullName.Length] != '[')
            //            return false;
            //    }
            //}

            //text = str1;
            //return true;
        }

        private bool AutoComplete(string text)
        {
            string text1 = SelectionLength != 0
                ? Text.Replace(SelectedText, text)
                : Text.Insert(CaretIndex, text);
            int caretIndex = CaretIndex;
            if ((StructureMembers
                ? (LookupWithMember(ref text1) ? 1 : 0)
                : (Lookup(ref text1) ? 1 : 0)) == 0)
                return false;
            Select(0, Text.Length);
            SelectedText = text1;
            CaretIndex = caretIndex + text.Length;
            Select(CaretIndex, Text.Length - caretIndex);
            return true;
        }

        public IEnumerable<string> ChoiceList
        {
            get { return (IEnumerable<string>) GetValue(ChoiceListProperty); }
            set { SetValue(ChoiceListProperty, value); }
        }

        public IEnumerable<IDataType> ChoiceListWithMembers
        {
            get
            {
                return (IEnumerable<IDataType>) GetValue(ChoiceListWithMembersProperty);
            }
            set { SetValue(ChoiceListWithMembersProperty, value); }
        }

        public bool ProcessTextInputWhenReadOnly
        {
            get { return (bool) GetValue(ProcessTextInputWhenReadOnlyProperty); }
            set { SetValue(ProcessTextInputWhenReadOnlyProperty, value); }
        }
    }
}
