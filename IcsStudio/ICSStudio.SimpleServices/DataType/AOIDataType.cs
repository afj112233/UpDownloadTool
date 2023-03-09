using System;
using ICSStudio.SimpleServices.PredefinedType;
using Newtonsoft.Json.Linq;
using ICSStudio.Interfaces.DataType;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Tags;

namespace ICSStudio.SimpleServices.DataType
{
    public sealed class AOIDataType : AssetDefinedDataType
    {
        private IDataTypeCollection _dataTypeCollection;
        private bool _copyAllValueToTag;
        private AoiDefinition _aoiDefinition;
        public AOIDataType(AoiDefinition aoiDefinition)
        {
            Name = aoiDefinition.Name;
            Description = aoiDefinition.Description;
            _aoiDefinition = aoiDefinition;
        }

        public override bool SupportsOneDimensionalArray =>true;

        public override bool SupportsMultiDimensionalArrays => true;
        public List<IDataType> InOutDataTypes { set; get; } = new List<IDataType>();

        public void Reset()
        {
            _is_init = false;
            Name = _aoiDefinition.Name;
            Description = _aoiDefinition.Description;
            foreach (var tp in ReferenceDataType)
            {
                tp.PropertyChanged -= OnMemberDataTypePropertyChanged;
            }
            ReferenceDataType.Clear();
            foreach (var member in _viewTypeMemberComponentCollection)
            {
                var aoiMember = member as AoiDataTypeMember;
                aoiMember?.OffMonitor();
            }
            _viewTypeMemberComponentCollection.DataTypeMemberClear();
            struct_list.Clear();
            PostInit(_aoiDefinition.ParentController.DataTypes);
            RaisePropertyChanged("ByteSize");
            MemberChangedList.Clear();
        }

        public void Overwrite(AoiDefinition aoiDefinition)
        {
            _aoiDefinition = aoiDefinition;
            Name = aoiDefinition.Name;
            Description = aoiDefinition.Description;
            EngineeringUnit = aoiDefinition.EngineeringUnit;
            _is_init = false;
            CanPostOverwrite = true;
        }

        public override void PostOverwrite(IDataTypeCollection collection)
        {
            if (CanPostOverwrite)
            {
                PostInit(collection);
                RaisePropertyChanged("ByteSize");
                CanPostOverwrite = false;
            }
        }


        private bool _is_init = false;
        private int _localTagsCount = 0;
        public List<Tuple<string, string>> MemberChangedList { get; } = new List<Tuple<string, string>>();

        private List<IDataType> ReferenceDataType { get; } = new List<IDataType>();

        public override void PostInit(IDataTypeCollection collection)
        {
            if (_is_init)
                return;
            _dataTypeCollection = collection;
            _localTagsCount = 0;
            int offset = 0;
            int field_index = 0;
            InOutDataTypes.Clear();
            if (_aoiDefinition.Tags.Any())
            {
                foreach (ITag p in _aoiDefinition.Tags.Where(t => t.Usage != Usage.Local))
                {
                    if (p.Usage == Usage.InOut)
                    {
                        InOutDataTypes.Add(p.DataTypeInfo.DataType);
                        continue;
                    }

                    var type_name = ((Controller)collection.ParentController).GetFinalName(typeof(IDataType), p.DataTypeInfo.DataType.IsBool ? "bool" : p.DataTypeInfo.DataType.Name);
                    //var displayStyle = p["Radix"] == null ? DisplayStyle.NullStyle : (DisplayStyle)(int)p["Radix"];
                    var tp = collection[type_name];
                    if (tp is AssetDefinedDataType)
                    {
                        (tp as AssetDefinedDataType).PostInit(collection);
                    }
                    if (!ReferenceDataType.Contains(tp))
                    {
                        ReferenceDataType.Add(tp);
                        tp.PropertyChanged += OnMemberDataTypePropertyChanged;
                    }
                    offset = AlignUp(offset, tp.AlignSize);

                    struct_list.Add(Tuple.Create(p.Name, tp, 0, offset));

                    _viewTypeMemberComponentCollection.AddDataTypeMember(new AoiDataTypeMember(p)
                    {
                        DisplayStyle = p.DisplayStyle,
                        ExternalAccess = ExternalAccess.ReadWrite,
                        ByteOffset = offset,
                        BitOffset = 0,
                        IsBit = tp is BOOL,
                        DataType = tp,
                        Dim1 = 0,
                        Dim2 = 0,
                        Dim3 = 0,
                        FieldIndex = field_index,
                    });
                    field_index += 1;
                    offset += tp.ByteSize;
                }

                foreach (var p in _aoiDefinition.Tags.Where(t => t.Usage == Usage.Local))
                {
                    _localTagsCount++;
                    var name = p.Name;
                    var type_name = ((Controller)collection.ParentController).GetFinalName(typeof(IDataType), p.DataTypeInfo.DataType.IsBool ? "bool" : p.DataTypeInfo.DataType.Name);

                    var tp = collection[type_name];
                    if (tp is AssetDefinedDataType)
                    {
                        (tp as AssetDefinedDataType).PostInit(collection);
                    }
                    if (!ReferenceDataType.Contains(tp))
                    {
                        ReferenceDataType.Add(tp);
                        tp.PropertyChanged += OnMemberDataTypePropertyChanged;
                    }

                    var dim = p.DataTypeInfo.Dim1;

                    offset = AlignUp(offset, RealAlignSize(dim, tp));
                    struct_list.Add(Tuple.Create(name, tp, dim, offset));
                    _viewTypeMemberComponentCollection.AddDataTypeMember(new AoiDataTypeMember(p)
                    {
                        DisplayStyle = p.DisplayStyle,
                        ExternalAccess = ExternalAccess.ReadWrite,
                        ByteOffset = offset,
                        BitOffset = 0,
                        IsBit = tp is BOOL,
                        IsHidden = true,
                        DataType = tp,
                        Dim1 = dim,
                        Dim2 = 0,
                        Dim3 = 0,
                        FieldIndex = field_index,
                    });
                    field_index += 1;
                    if (dim > 0 && tp.IsBool)
                        offset += 1 * Math.Max(1, dim) / 8;
                    else
                        offset += tp.ByteSize * Math.Max(1, dim);
                }
            }
            else
            {
                var config = _aoiDefinition.GetConfig();
                var parameters = config["Parameters"];
                foreach (var p in parameters)
                {
                    Usage usage = (Usage)(int)p["Usage"];
                    var type_name = (string)p["DataType"];
                    type_name = ((Controller)collection.ParentController).GetFinalName(typeof(IDataType), type_name);
                    var tp = collection[type_name];
                    if (tp is AssetDefinedDataType)
                    {
                        (tp as AssetDefinedDataType).PostInit(collection);
                    }
                    if (usage == Usage.InOut)
                    {
                        _aoiDefinition.ParseParameter((JObject)p,false);
                        InOutDataTypes.Add(collection[(string)p["DataType"]]);
                        continue;
                    }

                    var name = (string)p["Name"];
                    var displayStyle = p["Radix"] == null ? DisplayStyle.NullStyle : (DisplayStyle)(int)p["Radix"];
                   
                    if (!ReferenceDataType.Contains(tp))
                    {
                        ReferenceDataType.Add(tp);
                        tp.PropertyChanged += OnMemberDataTypePropertyChanged;
                    }
                    offset = AlignUp(offset, tp.AlignSize);

                    struct_list.Add(Tuple.Create(name, tp, 0, offset));
                    var tag = _aoiDefinition.ParseParameter((JObject)p,false);
                    _viewTypeMemberComponentCollection.AddDataTypeMember(new AoiDataTypeMember(tag)
                    {
                        Name = name,
                        DisplayStyle = displayStyle,
                        ExternalAccess = ExternalAccess.ReadWrite,
                        ByteOffset = offset,
                        BitOffset = 0,
                        IsBit = tp is BOOL,
                        DataType = tp,
                        Dim1 = 0,
                        Dim2 = 0,
                        Dim3 = 0,
                        FieldIndex = field_index,
                    });
                    field_index += 1;
                    offset += tp.ByteSize;
                }

                var locals = config["LocalTags"];
                foreach (var p in locals)
                {
                    _localTagsCount++;
                    var obj = p as JObject;
                    var name = (string)p["Name"];
                    var type_name = (string)p["DataType"];
                    type_name = ((Controller)collection.ParentController).GetFinalName(typeof(IDataType), type_name);
                    var displayStyle = p["Radix"] == null ? DisplayStyle.NullStyle : (DisplayStyle)(int)p["Radix"];
                    var tp = collection[type_name];
                    if (tp is AssetDefinedDataType)
                    {
                        (tp as AssetDefinedDataType).PostInit(collection);
                    }
                    if (!ReferenceDataType.Contains(tp))
                    {
                        ReferenceDataType.Add(tp);
                        tp.PropertyChanged += OnMemberDataTypePropertyChanged;
                    }
                    var dim = 0;
                    if (obj.ContainsKey("Dimensions"))
                    {
                        dim = (int)p["Dimensions"];
                    }

                    offset = AlignUp(offset, RealAlignSize(dim, tp));
                    struct_list.Add(Tuple.Create(name, tp, dim, offset));
                    var tag = _aoiDefinition.ParseLocal((JObject) p,false);
                    _viewTypeMemberComponentCollection.AddDataTypeMember(new AoiDataTypeMember(tag)
                    {
                        Name = name,
                        DisplayStyle = displayStyle,
                        ExternalAccess = ExternalAccess.ReadWrite,
                        ByteOffset = offset,
                        BitOffset = 0,
                        IsBit = tp is BOOL,
                        IsHidden = true,
                        DataType = tp,
                        Dim1 = dim,
                        Dim2 = 0,
                        Dim3 = 0,
                        FieldIndex = field_index,
                    });
                    field_index += 1;
                    if (dim > 0 && tp.IsBool)
                        offset += 1 * Math.Max(1, dim) / 8;
                    else
                        offset += tp.ByteSize * Math.Max(1, dim);
                }
            }
            _ByteSize = AlignUp(offset, 8);
            _BitSize = _ByteSize * 8;
            _is_init = true;
            _aoiDefinition.PostInit((DataTypeCollection)collection);
        }

        public bool IsMemberShowInOtherAoi(string memberName)
        {
            var target =
                _aoiDefinition.Tags.FirstOrDefault(t => t.Name.Equals(memberName, StringComparison.OrdinalIgnoreCase));
            if (target != null)
            {
                if (target.Usage == Usage.InOut) return false;
                return true;
            }
            //foreach (var parameter in _aoiDefinition.Tags.FirstOrDefault())
            //{
            //    if (memberName.ToLower() == parameter["Name"]?.ToString().ToLower())
            //    {
            //        if (parameter["Usage"]?.ToString() == Usage.InOut.ToString())
            //            return false;
            //        else
            //            return true;
            //    }
            //}

            return false;
        }

        public override IField Create(JToken token)
        {
            Debug.Assert(struct_list != null);
            var arr = token as JArray;
            var res = new AoiDefinitionField(this);
            var isLostLocalData = arr==null || arr.Count == struct_list.Count - (_localTagsCount == 0 ? 1 : _localTagsCount);
            if (arr?.Count == struct_list.Count ||!isLostLocalData)
            {
                for (int i = 0; i < struct_list.Count; ++i)
                {
                    if (isLostLocalData && i == arr.Count) break;
                    var tup = struct_list[i];
                    var offset = tup.Item4;
                    var dim = tup.Item3;
                    if (dim == 0)
                    {
                        try
                        {
                            res.fields.Add(Tuple.Create(tup.Item2.Create(arr?[i]), offset));
                        }
                        catch (Exception)
                        {
                            res.fields.Add(Tuple.Create(tup.Item2.FixDataField(arr?[i]), offset));
                        }
                    }
                    else
                    {
                        if (tup.Item2 is BOOL)
                        {
                            var field = new BoolArrayField(dim, 0, 0);
                            res.fields.Add(Tuple.Create(field as IField, offset));
                            Debug.Assert(arr == null || arr[i] is JArray);
                            var data = arr?[i] as JArray;
                            Debug.Assert(data == null || data.Count == dim / 32);
                            if (data != null)
                            {
                                for (int j = 0; j < dim / 32; ++j)
                                {
                                    field.Add(j * 32, (int)data[j]);
                                }
                            }
                        }
                        else
                        {
                            var field = new ArrayField(dim, 0, 0);
                            res.fields.Add(Tuple.Create(field as IField, offset));
                            Debug.Assert(arr == null || arr[i] is JArray || FamilyType == FamilyType.StringFamily);
                            {
                                var data = arr?[i] as JValue;
                                if (data != null && FamilyType == FamilyType.StringFamily)
                                {
                                    var tp = tup.Item2;
                                    var str = data.ToString();
                                    for (int j = 0; j < dim; j++)
                                    {
                                        var c = j >= str.Length ? '\0' : str[j];
                                        var tmp_field = tp.Create(c);
                                        field.Add(Tuple.Create(tmp_field, tp.ByteSize * j));
                                    }

                                    break;
                                }
                            }
                            {
                                var data = arr?[i] as JArray;
                                Debug.Assert(data == null || data.Count == dim);
                                var tp = tup.Item2;
                                for (int j = 0; j < dim; ++j)
                                {
                                    var tmp_field = tp.Create(data?[j]);
                                    field.Add(Tuple.Create(tmp_field, tp.ByteSize * j));
                                }
                            }
                        }
                    }
                }

                if (isLostLocalData)
                {
                    AoiDefinition aoi = null;
                    AoiDefinitionCollection aoiDefinitionCollection =
                        (AoiDefinitionCollection)Controller.GetInstance().AOIDefinitionCollection;
                    if (IsTmp)
                    {
                        aoi = aoiDefinitionCollection.Find(Name, true);
                    }

                    if (aoi == null)
                        aoi = aoiDefinitionCollection.Find(Name);
                    for (int i = struct_list.Count - _localTagsCount; i < struct_list.Count; i++)
                    {
                        var tup = struct_list[i];
                        var tag = (Tag)aoi.Tags[tup.Item1];
                        Debug.Assert(tag != null, $"Tag:{tup.Item1}");
                        var offset = tup.Item4;
                        res.fields.Add(Tuple.Create(tag.DataWrapper.Data.DeepCopy(), offset));
                    }
                }
            }
            else
            {
                AoiDefinition aoi = null;
                AoiDefinitionCollection aoiDefinitionCollection =
                    (AoiDefinitionCollection)Controller.GetInstance().AOIDefinitionCollection;
                if (IsTmp)
                {
                    aoi = aoiDefinitionCollection.Find(Name, true);
                }

                if (aoi == null)
                    aoi = aoiDefinitionCollection.Find(Name);
                Debug.Assert(aoi != null, Name);

                for (int i = 0; i < struct_list.Count; ++i)
                {
                    var tup = struct_list[i];
                    var tag = (Tag)aoi.Tags[tup.Item1];
                    Debug.Assert(tag != null, $"Tag:{tup.Item1}");
                    var offset = tup.Item4;

                    res.fields.Add(Tuple.Create(tag.DataWrapper.Data.DeepCopy(), offset));
                }
            }

            return res;
        }
        
        public bool CopyAllValueToTag
        {
            set
            {
                if (_copyAllValueToTag != value)
                {
                    _copyAllValueToTag = value;
                    RaisePropertyChanged();
                    RequestTagUpdateData = true;
                    RequestTagUpdateData = false;
                }

            }
            get { return _copyAllValueToTag; }
        }

        public void OffMonitor(ITag tag)
        {
            var member = TypeMembers.FirstOrDefault(m => ((AoiDataTypeMember) m).AoiTag == tag) as AoiDataTypeMember;
            member?.OffMonitor();
        }

        public void OnMonitor(ITag tag)
        {
            var member = TypeMembers.FirstOrDefault(m => ((AoiDataTypeMember)m).AoiTag == tag) as AoiDataTypeMember;
            member?.OnMonitor();
        }

        private void OnMemberDataTypePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ByteSize")
            {
                if (_dataTypeCollection != null)
                    Reset();
            }
            else if (e.PropertyName == "RequestTagUpdateData")
            {
                RaisePropertyChanged("RequestTagUpdateData");
            }
        }

    }
}