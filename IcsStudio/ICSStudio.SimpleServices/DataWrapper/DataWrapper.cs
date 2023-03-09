using System;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;

namespace ICSStudio.SimpleServices.DataWrapper
{
    public class DataWrapper
    {
        private readonly IDataType _dataType;
        private readonly int _dim1;
        private readonly int _dim2;
        private readonly int _dim3;
        public IField Data;

        public DataWrapper(IDataType dataType, int dim1, int dim2, int dim3, JToken data)
        {
            _dataType = dataType;
            _dim1 = dim1;
            _dim2 = dim2;
            _dim3 = dim3;

            int totalDimension = Math.Max(dim1, 1) * Math.Max(dim2, 1) * Math.Max(dim3, 1);

            if (_dim1 != 0 && dataType is BOOL)
            {
                Debug.Assert(data == null || data.Type == JTokenType.Array);
                var arr = data as JArray;
                Debug.Assert(totalDimension % 32 == 0);
                Debug.Assert(arr == null || arr.Count == totalDimension / 32);
                var res = new BoolArrayField(_dim1, _dim2, _dim3);
                for (int i = 0; i < totalDimension / 32; ++i)
                {
                    res.Add(i * 32, arr == null ? 0 : (int)arr[i]);
                }

                Data = res;

            }
            else if (_dim1 != 0)
            {

                Debug.Assert(data == null || data.Type == JTokenType.Array);
                var arr = data as JArray;
                Debug.Assert(arr == null || arr.Count == totalDimension);
                var res = new ArrayField(_dim1, _dim2, _dim3);
                for (int i = 0; i < totalDimension; ++i)
                {
                    try
                    {
                        res.Add(Tuple.Create(dataType.Create(arr?[i]), dataType.ByteSize * i));
                    }
                    catch (Exception)
                    {
                        res.Add(Tuple.Create(dataType.FixDataField(arr?[i]), dataType.ByteSize * i));
                    }
                }

                Data = res;
            }
            else
            {
                try
                {
                    Data = dataType.Create(data); ;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Data = dataType.FixDataField(data);
                }
            }

        }

        // for alias tag
        public DataWrapper(DataTypeInfo info, IField field)
        {
            _dataType = info.DataType;
            _dim1 = info.Dim1;
            _dim2 = info.Dim2;
            _dim3 = info.Dim3;

            Data = field;
        }

        public DataTypeInfo DataTypeInfo
        {
            get
            {
                DataTypeInfo dataTypeInfo = new DataTypeInfo
                {
                    DataType = _dataType, Dim1 = _dim1, Dim2 = _dim2, Dim3 = _dim3
                };

                return dataTypeInfo;
            }
        }

        public ITag ParentTag { get; set; }

        protected void NotifyParentPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Tag parentTag = ParentTag as Tag;
            parentTag?.RaisePropertyChanged(propertyName);
        }

        public void NotifyParentPropertyChanged(params string[] propertyNames)
        {
            Tag parentTag = ParentTag as Tag;
            parentTag?.RaisePropertyChanged(propertyNames);
        }
    }
}
