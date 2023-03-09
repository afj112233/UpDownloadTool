using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Annotations;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.SimpleServices.DataType
{
    public class DataTypeMember : IDataTypeMember
    {
        private string _name;
        private string _description;
        private IDataType _dataType;
        private string _displayName;
        private string _engineeringUnit = string.Empty;

        public IDataType ParentDataType { get; set; }

        public virtual string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    OldName = _name;
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string OldName { private set; get; }

        public virtual string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        public int DataTypeUid
        {
            get
            {
                if (DataType != null)
                    return DataType.Uid;

                return 0;
            }
        }

        public DataTypeInfo DataTypeInfo => new DataTypeInfo()
        {
            DataType = DataType,
            Dim1 = Dim1,
            Dim2 = Dim2,
            Dim3 = Dim3
        };

        public DisplayStyle DisplayStyle { get; set; }
        public ExternalAccess ExternalAccess { get; set; }
        public bool IsHidden { get; set; }
        public bool IsBit { get; set; }
        public int ByteOffset { get; set; }
        public int BitOffset { get; set; }

        public IDataType DataType
        {
            get { return _dataType; }
            set
            {
                if (_dataType != value)
                {
                    if (_dataType is AssetDefinedDataType)
                        _dataType.PropertyChanged -= DataType_PropertyChanged;
                    _dataType = value;
                    if (_dataType is AssetDefinedDataType)
                        _dataType.PropertyChanged += DataType_PropertyChanged;
                    OnPropertyChanged();
                    OnPropertyChanged("DetailList");
                    OnPropertyChanged("ExpanderVisibility");
                }
            }
        }

        public List<Tuple<string, string>> ChangedInfo { get; } = new List<Tuple<string, string>>();

        private void DataType_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ByteSize"||e.PropertyName=="Name")
            {
                var userDefined = sender as UserDefinedDataType;
                if (userDefined != null)
                {
                    if (userDefined.MemberChangedList.Count > 0)
                    {
                        foreach (var tuple in userDefined.MemberChangedList)
                        {
                            ChangedInfo.Add(new Tuple<string, string>($"{Name}.{tuple.Item1}",
                                $"{Name}.{tuple.Item2}"));
                        }

                        OnPropertyChanged("ChangedInfo");
                        ChangedInfo.Clear();
                    }
                    else
                    {
                        if (e.PropertyName == "Name")
                        {
                            OnPropertyChanged("Name");
                            return;
                        }
                    }
                }
                var aoi = sender as AOIDataType;
                if (aoi != null)
                {
                    if (e.PropertyName == "Name")
                    {
                        OnPropertyChanged("Name");
                        return;
                    }
                }
                OnPropertyChanged("ByteSize");
            }

            if (e.PropertyName == "RequestTagUpdateData")
            {
                OnPropertyChanged("RequestTagUpdateData");
            }
        }

        public int Dim1 { get; set; }
        public int Dim2 { get; set; }
        public int Dim3 { get; set; }
        public int FieldIndex { get; set; }

        public string EngineeringUnit
        {
            get { return _engineeringUnit; }
            set
            {
                if (_engineeringUnit != value)
                {
                    _engineeringUnit = value;
                    OnPropertyChanged();
                }
            }
        }

        public string DisplayName
        {
            set
            {
                if (_displayName != value)
                {
                    _displayName = value;
                    OnPropertyChanged();
                }
            }
            get
            {
                string name = "";
                if (string.IsNullOrEmpty(_displayName))
                {
                    name = DataType?.Name.Replace(":SINT", "").Replace(":INT", "").Replace(":DINT", "")
                        .Replace(":LINT", "");
                    if (Dim1 > 0)
                        return $"{name}[{Dim1}]";
                    else
                        return name;
                }

                name = _displayName.Replace(":SINT", "").Replace(":INT", "").Replace(":DINT", "")
                    .Replace(":LINT", "");
                if (Dim1 > 0)
                {

                    if (_displayName.IndexOf('[') > 0)
                    {
                        return $"{name.Substring(0, _displayName.IndexOf('['))}[{Dim1}]";
                    }
                    else
                    {
                        return $"{name}[{Dim1}]";
                    }
                }
                else
                    return name;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
