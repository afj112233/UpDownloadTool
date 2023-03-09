using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Linq;
using Antlr4.Runtime.Misc;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.PredefinedType;

namespace ICSStudio.SimpleServices.DataType
{
    public sealed class UserDefinedDataType : AssetDefinedDataType
    {
        private JObject info_;
        private IDataTypeCollection _dataTypeCollection;
        private bool _supportsMultiDimensionalArrays;
        private bool _supportsOneDimensionalArray;
        private FamilyType _familyType;
        public override FamilyType FamilyType => _familyType;

        private static int RealSize(int dim, int type_size, bool is_bool)
        {
            Debug.Assert(!is_bool || dim != 0);
            if (is_bool) return dim / 8;
            return dim == 0 ? type_size : type_size * dim;
        }

        private static int FindIndex(List<Tuple<string, IDataType, int, int>> tuples, string name)
        {
            for (int i = 0; i < tuples.Count; ++i)
            {
                if (name == tuples[i].Item1)
                    return i;
            }

            Debug.Assert(false);
            return -1;
        }

        public UserDefinedDataType([NotNull] DataTypeCollection parentCollection, JObject info) : base()
        {
            info_ = info;
            Debug.Assert(info != null && info["Name"].Type == JTokenType.String);
            Name = ((Controller) parentCollection.ParentController).GetFinalName(typeof(IDataType),
                (string) info["Name"]);
            Description = (string) info["Description"];
            Debug.Assert(info["Family"].Type == JTokenType.Integer);
            _familyType = (FamilyType) (int) info["Family"];
            EngineeringUnit = (string) info["EngineeringUnit"];
            _supportsOneDimensionalArray = true;
            _supportsMultiDimensionalArrays = true;
        }

        public void ResetMembers(JObject info, IDataTypeCollection collection)
        {
            info_ = info;
            Name = (string) info["Name"];
            Description = (string) info["Description"];
            EngineeringUnit = (string) info["EngineeringUnit"];
            is_init = false;
            PostInit(collection);
            RaisePropertyChanged("ByteSize");
            MemberChangedList.Clear();
        }

        public override void Overwrite(JObject info, IDataTypeCollection collection)
        {
            info_ = info;
            Name = (string) info["Name"];
            Description = (string) info["Description"];
            EngineeringUnit = (string) info["EngineeringUnit"];
            _familyType = (FamilyType) (int) info["Family"];
            is_init = false;
            CanPostOverwrite = true;
        }

        public override void PostOverwrite(IDataTypeCollection collection)
        {
            if (CanPostOverwrite)
            {
                PostInit(collection);
                RaisePropertyChanged("ByteSize");
                MemberChangedList.Clear();
                CanPostOverwrite = false;
            }                                           
        }

        protected override void DisposeAction()
        {
            foreach (var member in TypeMembers)
            {
                PropertyChangedEventManager.RemoveHandler(member, OnMemberPropertyChanged, "");
            }
            base.DisposeAction();
            info_ = null;
        }

        public List<Tuple<string, string>> MemberChangedList { get; } = new List<Tuple<string, string>>();

        //用于对依赖进行排序
        private bool is_init = false;

        private void RemoveMemberListener()
        {
            foreach (var member in _viewTypeMemberComponentCollection)
            {
                PropertyChangedEventManager.RemoveHandler(member, OnMemberPropertyChanged, "");
            }
        }

        public override void PostInit(IDataTypeCollection collection)
        {
            if (is_init)
                return;
            RemoveMemberListener();
            var info = info_;
            _dataTypeCollection = collection;
            var members = info["Members"] as JArray;
            Debug.Assert(members != null);

            int offset = 0;
            _viewTypeMemberComponentCollection.DataTypeMemberClear();
            struct_list.Clear();
            foreach (JObject member in members)
            {
                Debug.Assert(member != null);
                var name = (string) member["Name"];
                var hidden = (bool) member["Hidden"];
                var type_name = (string) member["DataType"];
                type_name = ((Controller) collection.ParentController).GetFinalName(typeof(IDataType), type_name);
                var dim = (int) member["Dimension"];
                if (type_name == "BIT")
                {
                    Debug.Assert(dim == 0 && member.ContainsKey("Target") && member.ContainsKey("BitNumber"));
                    continue;
                }

                var tp = collection[type_name];
                if (tp is AssetDefinedDataType)
                {
                    //tp.PropertyChanged += OnMemberPropertyChanged;
                    (tp as AssetDefinedDataType).PostInit(collection);
                }

                offset = AlignUp(offset, RealAlignSize(dim, tp));
                struct_list.Add(Tuple.Create(name, tp, dim, offset));
                offset += RealSize(dim, tp.ByteSize, tp is BOOL);
            }

            _ByteSize = AlignUp(offset, 8);
            _BitSize = _ByteSize * 8;

            var BOOL = PredefinedType.BOOL.Inst;
            var SINT = PredefinedType.SINT.Inst;

            foreach (JObject member in members)
            {
                Debug.Assert(member != null);
                var name = (string) member["Name"];
                Debug.Assert(name != null);
                var type_name = (string) member["DataType"];
                type_name = ((Controller) collection.ParentController).GetFinalName(typeof(IDataType), type_name);
                Debug.Assert(type_name != null);
                var dim = (int) member["Dimension"];
                var radix = (int) member["Radix"];
                var hidden = (bool) member["Hidden"];
                var external = (int) member["ExternalAccess"];
                var description = (string) member["Description"];

                if (type_name == "BIT")
                {
                    Debug.Assert(dim == 0);
                    Debug.Assert(!hidden);
                    string target = (string) member["Target"];
                    Debug.Assert(target != null);
                    int bit_num = (int) member["BitNumber"];
                    var index = FindIndex(struct_list, target);
                    Debug.Assert(struct_list[index].Item2 is SINT);
                    var bitMember = new DataTypeMember
                    {
                        Name = name,
                        DisplayStyle = (DisplayStyle) radix,
                        ExternalAccess = (ExternalAccess) external,
                        ByteOffset = struct_list[index].Item4,
                        BitOffset = bit_num,
                        IsBit = true,
                        DataType = BOOL,
                        Dim1 = 0,
                        Dim2 = 0,
                        Dim3 = 0,
                        FieldIndex = index,
                        Description = description,
                    };
                    PropertyChangedEventManager.AddHandler(bitMember, OnMemberPropertyChanged, "");
                    _viewTypeMemberComponentCollection.AddDataTypeMember(bitMember);
                }
                else if (type_name == "BOOL")
                {
                    Debug.Assert(dim != 0 && dim % 32 == 0);
                    Debug.Assert(!hidden);
                    var index = FindIndex(struct_list, name);
                    var boolMember = new DataTypeMember
                    {
                        Name = name,
                        DisplayStyle = (DisplayStyle) radix,
                        ExternalAccess = (ExternalAccess) external,
                        ByteOffset = struct_list[index].Item4,
                        BitOffset = 0,
                        //FIXME true or false?
                        IsBit = true,
                        DataType = BOOL,
                        Dim1 = dim,
                        Dim2 = 0,
                        Dim3 = 0,
                        FieldIndex = index,
                        Description = description,
                    };
                    PropertyChangedEventManager.AddHandler(boolMember, OnMemberPropertyChanged, "");
                    _viewTypeMemberComponentCollection.AddDataTypeMember(boolMember);
                }
                else if (!hidden)
                {
                    var index = FindIndex(struct_list, name);
                    var tp = collection[type_name];
                    var dataTypeMember = new DataTypeMember
                    {
                        Name = name,
                        DisplayStyle = (DisplayStyle) radix,
                        ExternalAccess = (ExternalAccess) external,
                        ByteOffset = struct_list[index].Item4,
                        BitOffset = 0,
                        //FIXME true or false?
                        IsBit = false,
                        DataType = tp,
                        Dim1 = dim,
                        Dim2 = 0,
                        Dim3 = 0,
                        FieldIndex = index,
                        Description = description,
                    };
                    //if (dataTypeMember.DataType is AssetDefinedDataType)
                    PropertyChangedEventManager.AddHandler(dataTypeMember, OnMemberPropertyChanged, "");
                    _viewTypeMemberComponentCollection.AddDataTypeMember(dataTypeMember);
                }
            }

            is_init = true;
        }

        public JObject ConvertToJObject()
        {
            return info_;
        }

        public override bool IsStringType => FamilyType == FamilyType.StringFamily;

        public override bool SupportsOneDimensionalArray => _supportsOneDimensionalArray;

        public override bool SupportsMultiDimensionalArrays => _supportsMultiDimensionalArrays;

        private void OnMemberPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "RequestTagUpdateData")
            {
                RequestTagUpdateData = ((AssetDefinedDataType) ((DataTypeMember) sender).DataType).RequestTagUpdateData;
            }

            var dataTypeMember = (DataTypeMember) sender;
            if (e.PropertyName == "ChangedInfo")
            {
                if (dataTypeMember.ChangedInfo.Count > 0)
                {
                    foreach (var tuple in dataTypeMember.ChangedInfo)
                    {
                        MemberChangedList.Add(tuple);
                    }
                }

                if (_dataTypeCollection != null)
                {
                    if (ResetInfo())
                    {
                        ResetMembers(info_, _dataTypeCollection);
                    }
                }
            }

            if (e.PropertyName == "ByteSize")
            {
                RaisePropertyChanged("ByteSize");
            }

            if (e.PropertyName == "Name")
            {
                var oldName = dataTypeMember.OldName;
                var name = dataTypeMember.Name;
                if (dataTypeMember.Dim1 > 0)
                {
                    oldName = $"{oldName}(\\[.*?\\])?";
                    name = $"{name}(\\[.*?\\])?";
                }
               
                MemberChangedList.Add(new Tuple<string, string>(oldName, name));
                var pair = struct_list.FirstOrDefault(s => s.Item1.Equals(oldName));
                if(pair!=null)
                {
                    var index = struct_list.IndexOf(pair);
                    struct_list[index]=new Tuple<string, IDataType, int, int>(name,pair.Item2,pair.Item3,pair.Item4);
                }

                ResetInfo();
            }
        }

        public bool ResetInfo()
        {
            JArray info = new JArray();
            int boolCount = 0;
            string target = "";
            info_["Name"] = Name;
            info_["Description"] = Description;
            foreach (var member in _viewTypeMemberComponentCollection)
            {
                if (member.DataTypeInfo.DataType is BOOL)
                {
                    if (member.DataTypeInfo.Dim1 != 0 && member.DataTypeInfo.Dim1 % 32 == 0)
                    {
                        JObject jObject = new JObject();
                        jObject["Name"] = member.Name;
                        jObject["Description"] = member.Description;
                        jObject["ExternalAccess"] = (byte) member.ExternalAccess;
                        jObject["DataType"] = member.DataTypeInfo.DataType.Name;
                        jObject["Dimension"] = member.DataTypeInfo.Dim1;
                        jObject["Radix"] = (byte) member.DisplayStyle;
                        jObject["Hidden"] = false;
                        info.Add(jObject);
                        boolCount = 0;
                        continue;
                    }

                    if (boolCount % 8 == 0)
                    {
                        info.Add(SINTMemberFactory(ref target, info.Count));

                    }

                    info.Add(BITMemberFactory(member, target, boolCount % 8));
                    boolCount++;
                }
                else
                {
                    JObject jObject = new JObject();
                    jObject["Name"] = member.Name;
                    jObject["Description"] = member.Description;
                    jObject["ExternalAccess"] = (byte) member.ExternalAccess;
                    jObject["DataType"] = member.DataTypeInfo.DataType.Name;
                    jObject["Dimension"] = member.DataTypeInfo.Dim1;
                    jObject["Radix"] = (byte) member.DisplayStyle;
                    jObject["Hidden"] = false;
                    info.Add(jObject);
                    boolCount = 0;
                }
            }

            if (info_["Members"].Equals(info))
            {
                return false;
            }

            info_["Members"] = info;
            return true;
        }

        private JObject SINTMemberFactory(ref string target, int count)
        {
            JObject member = new JObject();
            var name = Name;
            if (name.Length > 10)
            {
                name = name.Substring(0, 10);
            }
            target = $"ZZZZZZZZZZ{name}{count}";
            member["Name"] = target;
            member["Description"] = "";
            member["EngineeringUnit"] = "";
            member["ExternalAccess"] = (byte) ExternalAccess.ReadWrite;
            member["DataType"] = "SINT";
            member["Dimension"] = 0;
            member["Radix"] = (byte) DisplayStyle.Decimal;
            member["DisplayName"] = target;
            member["Hidden"] = true;
            return member;
        }

        private JObject BITMemberFactory(IDataTypeMember dataTypeMember, string target, int bitNumber)
        {
            JObject member = new JObject();
            member["Name"] = dataTypeMember.Name;
            member["Description"] = dataTypeMember.Description;
            member["EngineeringUnit"] = (dataTypeMember as DataTypeMember)?.EngineeringUnit;
            member["ExternalAccess"] = (byte) dataTypeMember.ExternalAccess;
            member["DataType"] = "BIT";
            member["Dimension"] = 0;
            member["Radix"] = (byte) dataTypeMember.DisplayStyle;
            member["DisplayName"] = "BOOL";
            member["Hidden"] = false;
            member["BitNumber"] = bitNumber;
            member["Target"] = target;
            return member;
        }
    }
}
