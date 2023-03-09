using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using ICSStudio.DeviceProfiles.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.Utils;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.DataType
{
    public class ModuleDefinedDataType : AssetDefinedDataType
    {
        private readonly JObject _info;

        // for test
        private IDataTypeCollection _collection;

        public ModuleDefinedDataType(JObject info)
        {
            Contract.Assert(info != null);
            _info = info;

            base.Name = _info["Name"].ToString();
            base.IsVendorDefinedType = true;

            var dataTypeClass = EnumUtils.Parse<DataTypeClass>(_info["Class"].ToString());
            IsIOType = dataTypeClass == DataTypeClass.IO;
            SupportsOneDimensionalArray = true;
        }

        public override bool IsIOType { get; }
        public override bool SupportsOneDimensionalArray { get; }

        public override void PostInit(IDataTypeCollection collection)
        {
            _collection = collection;

            var definitionList = _info["Members"].ToObject<List<DataTypeMemberDefinition>>();

            Contract.Assert(definitionList != null);

            // struct_list
            struct_list.Clear();
            int offset = 0;
            int align = 4;
            foreach (var definition in definitionList)
            {
                if (definition.DataType == "BIT")
                    continue;

                if (definition.DataType == "BITFIELD")
                    continue;

                var tp = collection[definition.DataType];

                int dim = definition.Dimension != null
                    ? int.Parse(definition.Dimension)
                    : 0;

                align = Math.Max(align, tp.AlignSize);
                offset = AlignUp(offset, RealAlignSize(dim, tp));
                struct_list.Add(Tuple.Create(definition.Name, tp, dim, offset));
                offset += RealSize(dim, tp.ByteSize, tp is BOOL);
            }

            Align = align;
            _ByteSize = AlignUp(offset, align);
            _BitSize = _ByteSize * 8;

            // view member
            _viewTypeMemberComponentCollection.DataTypeMemberClear();

            foreach (var definition in definitionList)
            {
                /*
                AB:Embedded_DiscreteIO:C:0 
                BIT and Hidden
                */

                int dim = definition.Dimension != null
                    ? int.Parse(definition.Dimension)
                    : 0;

                DataTypeMember dataTypeMember;

                if (definition.DataType == "BIT")
                {
                    Contract.Assert(dim == 0);
                    Contract.Assert(!string.IsNullOrEmpty(definition.Target));

                    int index = FindIndex(struct_list, definition.Target);

                    IDataType dataType = null;
                    if (struct_list[index].Item2 is SINT)
                        dataType = BOOL.SInst;
                    else if (struct_list[index].Item2 is INT)
                        dataType = BOOL.IInst;
                    else if (struct_list[index].Item2 is DINT)
                        dataType = BOOL.DInst;
                    else if (struct_list[index].Item2 is LINT)
                        dataType = BOOL.LInst;
                    else
                        Contract.Assert(false);

                    dataTypeMember = new DataTypeMember()
                    {
                        Name = definition.Name,
                        DisplayStyle = DisplayStyle.Decimal,
                        ByteOffset = struct_list[index].Item4,
                        BitOffset = definition.BitNumber,
                        IsBit = true,
                        IsHidden = definition.Hidden,
                        DataType = dataType,
                        FieldIndex = index
                    };

                    _viewTypeMemberComponentCollection.AddDataTypeMember(dataTypeMember);
                }
                else if (definition.DataType == "BOOL")
                {
                    Contract.Assert(false, "Add code for BOOL");
                }
                else if (definition.DataType == "BITFIELD")
                {
                    //ignore?
                }
                else
                {
                    int index = FindIndex(struct_list, definition.Name);
                    IDataType tp = collection[definition.DataType];

                    dataTypeMember = new DataTypeMember()
                    {
                        Name = definition.Name,
                        ByteOffset = struct_list[index].Item4,
                        BitOffset = 0,
                        IsBit = false,
                        IsHidden = definition.Hidden,
                        DataType = tp,
                        Dim1 = dim,
                        FieldIndex = index
                    };

                    dataTypeMember.DisplayStyle =
                        definition.Radix == DisplayStyle.NullStyle ? GetDefaultDisplayStyle(tp) : definition.Radix;

                    _viewTypeMemberComponentCollection.AddDataTypeMember(dataTypeMember);
                }
            }
        }

        public JObject ConvertToJObject()
        {
            JObject dataTypeObject = _info.DeepClone() as JObject;
            Contract.Assert(dataTypeObject!=null);

            // add more information
            // add byte size and real size
            if (dataTypeObject.ContainsKey("ByteSize"))
                dataTypeObject["ByteSize"] = ByteSize;
            else
                dataTypeObject.Add("ByteSize", ByteSize);
            
            // add offset and real size
            JArray members = dataTypeObject["Members"] as JArray;
            if (members != null)
            {
                List<JObject> bitfieldList = new List<JObject>();

                foreach (JObject member in members.OfType<JObject>())
                {
                    string name = member["Name"].ToString();
                    string dataType = member["DataType"].ToString();

                    if (dataType == "BIT")
                        continue;

                    if (dataType == "BITFIELD")
                    {
                        bitfieldList.Add(member);
                        continue;
                    }
                    
                    var tp = _collection[dataType];

                    int dim = 0;
                    if (member.ContainsKey("Dimension"))
                        dim = int.Parse(member["Dimension"].ToString());

                    int index = FindIndex(struct_list, name);
                    int byteOffset = struct_list[index].Item4;
                    int realSize = RealSize(dim, tp.ByteSize, tp is BOOL);

                    if (member.ContainsKey("ByteOffset"))
                        member["ByteOffset"] = byteOffset;
                    else
                        member.Add("ByteOffset", byteOffset);

                    if (member.ContainsKey("ByteSize"))
                        member["ByteSize"] = realSize;
                    else
                        member.Add("ByteSize", realSize);
                }

                // remove BITFIELD
                foreach (JObject member in bitfieldList)
                {
                    members.Remove(member);
                }
            }
            //end add

            return dataTypeObject;
        }

        private int RealSize(int dim, int typeSize, bool isBool)
        {
            Contract.Assert(!isBool || dim != 0);
            if (isBool) return dim / 8;
            return dim == 0 ? typeSize : typeSize * dim;
        }

        private int FindIndex(List<Tuple<string, IDataType, int, int>> tuples, string name)
        {
            for (int i = 0; i < tuples.Count; ++i)
            {
                if (name == tuples[i].Item1)
                    return i;
            }

            Contract.Assert(false);
            return -1;
        }

        private DisplayStyle GetDefaultDisplayStyle(IDataType dataType)
        {
            var defaultDisplayStyle = DisplayStyle.NullStyle;

            if (dataType is BOOL || dataType is SINT || dataType is INT || dataType is DINT || dataType is LINT)
                defaultDisplayStyle = DisplayStyle.Decimal;
            else if (dataType is REAL) defaultDisplayStyle = DisplayStyle.Float;

            return defaultDisplayStyle;
        }
    }
}
