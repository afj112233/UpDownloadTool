using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.PredefinedType;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ICSStudio.SimpleServices.DataType
{
    public abstract class AssetDefinedDataType : CompositiveType
    {
        private bool _requestTagUpdateData;

        public virtual void PostInit(IDataTypeCollection collection)
        {

        }

        public virtual void Overwrite(JObject info, IDataTypeCollection collection)
        {

        }

        public bool CanPostOverwrite { protected set; get; }

        public virtual void PostOverwrite(IDataTypeCollection collection)
        {

        }

        public override int AlignSize => Align;

        public override bool IsStruct => true;

        protected static int AlignUp(int value, int align)
        {
            return (value + align - 1) / align * align;
        }

        protected static int RealAlignSize(int dim, IDataType tp)
        {
            if (dim != 0 && tp is BOOL) return 4;
            return tp.AlignSize;
        }

        //name, type, dim, offset?
        protected List<Tuple<string, IDataType, int, int>> struct_list =
            new List<Tuple<string, IDataType, int, int>>();

        protected override void DisposeAction()
        {
            struct_list.Clear();
            _viewTypeMemberComponentCollection.Dispose();
        }

        public bool IsTmp { get; set; } = false;

        public bool RequestTagUpdateData
        {
            set
            {
                if (_requestTagUpdateData != value)
                {
                    _requestTagUpdateData = value;
                    RaisePropertyChanged();
                }
            }
            get { return _requestTagUpdateData; }
        }


        public override IField Create(JToken token)
        {
            Debug.Assert(struct_list != null);
            Debug.Assert(token == null || token.Type == JTokenType.Array);
            var arr = token as JArray;
            Debug.Assert(arr == null || arr.Count == struct_list.Count);
            if (arr?.Count != struct_list.Count) arr = null;
            var res = new UserDefinedField();
            for (int i = 0; i < struct_list.Count; ++i)
            {
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
                                    try
                                    {
                                        field.Add(Tuple.Create(tp.Create(c), tp.ByteSize * j));
                                    }
                                    catch (Exception)
                                    {
                                        field.Add(Tuple.Create(tp.FixDataField(c), tp.ByteSize * j));
                                    }
                                }

                                break;
                            }
                        }
                        {
                            var data = arr?[i] as JArray;
                            Debug.Assert(FamilyType == FamilyType.StringFamily || data == null || data.Count == dim);
                            var tp = tup.Item2;
                            for (int j = 0; j < dim; ++j)
                            {
                                if (j < data?.Count)
                                {
                                    try
                                    {
                                        field.Add(Tuple.Create(tp.Create(data[j]), tp.ByteSize * j));
                                    }
                                    catch (Exception)
                                    {
                                        field.Add(Tuple.Create(tp.FixDataField(data[j]), tp.ByteSize * j));
                                    }
                                }
                                else
                                {
                                    var tmp_field = tp.Create(null);
                                    field.Add(Tuple.Create(tmp_field, tp.ByteSize * j));
                                }
                            }
                        }
                    }
                }
            }

            return res;
        }

        public bool IsMember(AssetDefinedDataType dataType)
        {
            if (dataType == this) return true;
            foreach (var tuple in struct_list)
            {
                if (tuple.Item2 == dataType) return true;
                var userDefined = tuple.Item2 as UserDefinedDataType;
                if (userDefined != null && userDefined.IsMember(dataType)) return true;
            }

            return false;
        }

        protected int Align = 8;
    }
}