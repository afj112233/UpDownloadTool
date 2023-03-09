using ICSStudio.Interfaces.DataType;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using ICSStudio.SimpleServices.Common;
using BitConverter = System.BitConverter;

namespace ICSStudio.SimpleServices.DataType
{
    //假定BitConvert是小端格式
    public class MsgPackUtils
    {
        public static void EncodeBool(List<byte> res, bool data)
        {
            EncodeInt8(res, (sbyte) ( data ? 1 : 0));
        }

        public static void EncodeInt8(List<byte> res, sbyte data)
        {
            res.Add(0xD0);
            res.Add((byte)data);
        }

        public static void EncodeInt16(List<byte> res, Int16 data)
        {
            res.Add(0xD1);
            res.AddRange(BitConverter.GetBytes(data).Reverse());
        }

        public static void EncodeInt32(List<byte> res, int data)
        {
            if (data >= 0 && data <= 127)
            {
                res.Add(((byte)(data & 0x7F)));
            }
            else
            {
                res.Add(0xD2);
                res.AddRange(BitConverter.GetBytes(data).Reverse());
            }
        }

        public static void EncodeInt64(List<byte> res, Int64 data)
        {
            res.Add(0xD3);
            res.AddRange(BitConverter.GetBytes(data).Reverse());
        }

        public static void EncodeUInt8(List<byte> res, byte data)
        {
            res.Add(0xCC);
            res.Add((byte)data);

        }

        public static void EncodeUInt16(List<byte> res, UInt16 data)
        {
            res.Add(0xCD);
            res.AddRange(BitConverter.GetBytes(data).Reverse());
        }

        public static void EncodeUInt32(List<byte> res, uint data)
        {
            res.Add(0xCE);
            res.AddRange(BitConverter.GetBytes(data).Reverse());
        }

        public static void EncodeUInt64(List<byte> res, UInt64 data)
        {
            res.Add(0xCF);
            res.AddRange(BitConverter.GetBytes(data).Reverse());
        }

        public static void EncodeFloat32(List<byte> res, float data)
        {
            res.Add(0xCA);
            res.AddRange(BitConverter.GetBytes(data).Reverse());
        }

        public static void EncodeFloat64(List<byte> res, double data)
        {
            res.Add(0xCB);
            res.AddRange(BitConverter.GetBytes(data).Reverse());
        }

        public static void EncodeArraySize(List<byte> res, int size)
        {

            if (size <= 15)
            {
                res.Add((byte)(0x90 | (byte)size));
            }
            else if (size <= ((1 << 16) - 1))
            {
                res.Add(0xDC);
                res.Add((byte)(size >> 8));
                res.Add((byte)(size & 0xFF));
            }
            else
            {
                res.Add(0xDD);
                res.Add((byte)((size >> 24) & 0xFF));
                res.Add((byte)((size >> 16) & 0xFF));
                res.Add((byte)((size >> 8) & 0xFF));
                res.Add((byte)((size >> 0) & 0xFF));
            }

        }
    }

    public abstract class IFieldAtomic : IField
    {
        public abstract JToken ToJToken();

        public abstract IField DeepCopy();

        public abstract void ToMsgPack(List<byte> res);
    }

    public class BoolField : IFieldAtomic
    {
        public byte value { get; set; } = 0;

        public BoolField(JToken token)
        {
            Debug.Assert(token == null || token.Type == JTokenType.Integer);
            if (token != null)
            {
                var value = (byte) (token as JValue);
                Debug.Assert(value == 0 || value == 1);
                this.value = value;
            }
        }

        public override JToken ToJToken()
        {
            return new JValue(value);
        }

        public override IField DeepCopy()
        {
            return new BoolField(ToJToken());
        }

        public override void ToMsgPack(List<byte> res)
        {
            MsgPackUtils.EncodeBool(res, value != 0);
        }

    }

    public class Int8Field : IFieldAtomic
    {
        public sbyte value { get; set; } = 0;

        public Int8Field(JToken token)
        {

            Debug.Assert(token == null || token.Type == JTokenType.Integer);
            if (token != null)
            {
                var value = (sbyte) (token as JValue);
                this.value = value;
            }
        }

        public override JToken ToJToken()
        {
            return new JValue(value);
        }

        public override IField DeepCopy()
        {
            return new Int8Field(ToJToken());
        }

        public override void ToMsgPack(List<byte> res)
        {
            MsgPackUtils.EncodeInt8(res, value);
        }
    }

    public class UInt8Field : IFieldAtomic
    {
        public byte value { get; set; } = 0;

        public UInt8Field(JToken token)
        {


            Debug.Assert(token == null || token.Type == JTokenType.Integer);
            if (token != null)
            {
                var value = (byte) (token as JValue);
                this.value = value;
            }
        }

        public override JToken ToJToken()
        {
            return new JValue(value);
        }

        public override IField DeepCopy()
        {
            return new UInt8Field(ToJToken());
        }

        public override void ToMsgPack(List<byte> res)
        {
            MsgPackUtils.EncodeUInt8(res, value);
        }
    }

    public class Int16Field : IFieldAtomic
    {
        public Int16 value { get; set; } = 0;

        public Int16Field(JToken token)
        {


            Debug.Assert(token == null || token.Type == JTokenType.Integer);
            if (token != null)
            {
                var value = (Int16) (token as JValue);
                this.value = value;
            }
        }

        public override JToken ToJToken()
        {
            return new JValue(value);
        }

        public override IField DeepCopy()
        {
            return new Int16Field(ToJToken());
        }

        public override void ToMsgPack(List<byte> res)
        {
            MsgPackUtils.EncodeInt16(res, value);
        }
    }

    public class UInt16Field : IFieldAtomic
    {
        public UInt16 value { get; set; } = 0;

        public UInt16Field(JToken token)
        {


            Debug.Assert(token == null || token.Type == JTokenType.Integer);
            if (token != null)
            {
                var value = (UInt16) (token as JValue);
                this.value = value;
            }
        }

        public override JToken ToJToken()
        {
            return new JValue(value);
        }

        public override IField DeepCopy()
        {
            return new UInt16Field(ToJToken());
        }


        public override void ToMsgPack(List<byte> res)
        {
            MsgPackUtils.EncodeUInt16(res, value);
        }

    }

    public class Int32Field : IFieldAtomic
    {
        public Int32 value { get; set; } = 0;

        public Int32Field(JToken token)
        {

            Debug.Assert(token == null || token.Type == JTokenType.Integer);
            if (token != null)
            {
                var value = (Int32) (token as JValue);
                this.value = value;
            }
        }

        public override JToken ToJToken()
        {
            return new JValue(value);
        }

        public override IField DeepCopy()
        {
            return new Int32Field(ToJToken());
        }

        public override void ToMsgPack(List<byte> res)
        {
            MsgPackUtils.EncodeInt32(res, value);
        }
    }

    public class UInt32Field : IFieldAtomic
    {
        public UInt32 value { get; set; } = 0;

        public UInt32Field(JToken token)
        {


            Debug.Assert(token == null || token.Type == JTokenType.Integer);
            if (token != null)
            {
                var value = (UInt32) (token as JValue);
                this.value = value;
            }
        }

        public override JToken ToJToken()
        {
            return new JValue(value);
        }

        public override IField DeepCopy()
        {
            return new UInt32Field(ToJToken());
        }

        public override void ToMsgPack(List<byte> res)
        {
            MsgPackUtils.EncodeUInt32(res, value);
        }
    }

    public class Int64Field : IFieldAtomic
    {
        public Int64 value { get; set; } = 0;

        public Int64Field(JToken token)
        {


            Debug.Assert(token == null || token.Type == JTokenType.Integer);
            if (token != null)
            {
                var value = (Int64) (token as JValue);
                this.value = value;
            }
        }

        public override JToken ToJToken()
        {
            return new JValue(value);
        }

        public override IField DeepCopy()
        {
            return new Int64Field(ToJToken());
        }

        public override void ToMsgPack(List<byte> res)
        {
            MsgPackUtils.EncodeInt64(res, value);
        }
    }

    public class UInt64Field : IFieldAtomic
    {
        public UInt64 value { get; set; } = 0;

        public UInt64Field(JToken token)
        {


            Debug.Assert(token == null || token.Type == JTokenType.Integer);
            if (token != null)
            {
                var value = (UInt64) (token as JValue);
                this.value = value;
            }
        }

        public override JToken ToJToken()
        {
            return new JValue(value);
        }

        public override IField DeepCopy()
        {
            return new UInt64Field(ToJToken());
        }

        public override void ToMsgPack(List<byte> res)
        {
            MsgPackUtils.EncodeUInt64(res, value);
        }
    }

    public class RealField : IFieldAtomic
    {
        public float value { get; set; } = 0;

        public RealField(JToken token)
        {
            if (token != null && token.Type == JTokenType.String)
            {
                if (((string)(token as JValue)?.Value)?.Equals("Infinity") ?? false)
                {
                    this.value = float.PositiveInfinity;
                    return;
                }

                if (((string)(token as JValue)?.Value)?.Equals("-Infinity") ?? false)
                {
                    this.value = float.NegativeInfinity;
                    return;
                }

                if (((string) (token as JValue)?.Value)?.Contains("NaN") ?? false)
                {
                    this.value = float.NaN;
                    return;
                }
            }

            Debug.Assert(token == null || token.Type == JTokenType.Float || token.Type == JTokenType.Integer);
            if (token == null)
                return;

            this.value = (float) (token as JValue);


            if (token.Type == JTokenType.Integer)
                Debug.Assert(this.value == (long) this.value);
        }


        public override JToken ToJToken()
        {
            return new JValue(value);
        }

        public override IField DeepCopy()
        {
            return new RealField(ToJToken());
        }

        public override void ToMsgPack(List<byte> res)
        {
            MsgPackUtils.EncodeFloat32(res, value);
        }
    }

    public class LRealField : IFieldAtomic
    {
        public double value { get; set; } = 0.0;

        public LRealField(JToken token)
        {


            Debug.Assert(token == null || token.Type == JTokenType.Float || token.Type == JTokenType.Integer);
            if (token == null)
                return;
            this.value = (double) (token as JValue);


            if (token.Type == JTokenType.Integer)
                Debug.Assert(this.value == (long) this.value, this.value.ToString());
        }

        public override JToken ToJToken()
        {
            return new JValue(value);
        }

        public override IField DeepCopy()
        {
            return new LRealField(ToJToken());
        }

        public override void ToMsgPack(List<byte> res)
        {
            MsgPackUtils.EncodeFloat64(res, value);
        }
    }

    public abstract class IArrayField : IField
    {
        public abstract JToken ToJToken();
        public abstract IField DeepCopy();
        public abstract void ToMsgPack(List<byte> data);

    }

    public class BoolArrayField : IArrayField
    {
        private BitArray fields;

        public BoolArrayField(int dim1, int dim2, int dim3)
        {


            {
                Debug.Assert(dim1 >= 1 && dim2 == 0 && dim3 == 0);
                Debug.Assert(dim1 % 32 == 0);
            }

            fields = new BitArray(dim1);
        }

        private static int BitToInt(BitArray arr, int offset)
        {
            int res = 0;
            for (int i = 31; i >= 0; --i)
            {
                res = arr[offset + i] ? res + (1 << i) : res;
            }

            return res;
        }

        public void Add(int offset, int value)
        {
            for (int i = 0; i < 32; ++i)
            {
                fields[i + offset] = ((value >> i) & 0x01) == 1;
            }
        }

        public bool Get(int index)
        {
            return fields.Get(index);
        }

        public int GetInt(int intIndex)
        {
            return BitToInt(fields, intIndex * 32);
        }

        public void Set(int index, bool value)
        {
            fields.Set(index, value);
        }

        internal int BitCount => fields.Count;

        public int getBitCount()
        {
            return BitCount;
        }

        public override JToken ToJToken()
        {
            var res = new JArray();
            for (int i = 0; i < fields.Count / 32; ++i)
            {
                int data = BitToInt(fields, i * 32);
                res.Add(data);
            }

            return res;
        }

        public override IField DeepCopy()
        {
            var copyField = new BoolArrayField(fields.Count, 0, 0);
            for (int i = 0; i < fields.Count; i++)
            {
                copyField.Set(i, Get(i));
            }

            return copyField;
        }

        public override void ToMsgPack(List<byte> res)
        {
            MsgPackUtils.EncodeArraySize(res,  fields.Count/32);
            for (int i = 0; i < fields.Count / 32; ++i)
            {
                int data = BitToInt(fields, i * 32);
                MsgPackUtils.EncodeInt32(res, data);
            }
        }
    }

    public class ArrayField : IArrayField
    {
        public int offset { get; }
        public List<Tuple<IField, int>> fields { get; } = new List<Tuple<IField, int>>();
        private int dim1, dim2, dim3;

        public int Size()
        {
            return Math.Max(dim1, 1) * Math.Max(dim2, 1) * Math.Max(dim3, 1);
        }

        public ArrayField(int dim1, int dim2, int dim3)
        {
            Debug.Assert(dim1 >= 1 && dim2 >= 0 && dim3 >= 0);
            this.dim1 = dim1;
            this.dim2 = dim2;
            this.dim3 = dim3;
            fields.Capacity = Size();
        }

        public void Add(Tuple<IField, int> info)
        {
            Debug.Assert(Size() > fields.Count);
            fields.Add(info);
        }

        public override JToken ToJToken()
        {
            var res = new JArray();
            foreach (var field in fields)
            {
                res.Add(field.Item1.ToJToken());
            }

            return res;
        }

        public override IField DeepCopy()
        {
            var copyField = new ArrayField(dim1, dim2, dim3);
            foreach (var field in fields)
            {
                copyField.Add(new Tuple<IField, int>(field.Item1.DeepCopy(), field.Item2));
            }

            return copyField;
        }

        public override void ToMsgPack(List<byte> data)
        {
            MsgPackUtils.EncodeArraySize(data, Size());
            foreach (var field in fields)
            {
                field.Item1.ToMsgPack(data);
            }
        }

    }

    public abstract class ICompositeField : IField
    {
        public List<Tuple<IField, int>> fields { get; } = new List<Tuple<IField, int>>();

        public virtual JToken ToJToken()
        {
            var res = new JArray();
            foreach (var field in fields)
            {
                res.Add(field.Item1.ToJToken());
            }

            return res;
        }

        public abstract IField DeepCopy();

        public virtual void ToMsgPack(List<byte> data)
        {
            MsgPackUtils.EncodeArraySize(data, fields.Count);
            foreach (var field in fields)
            {
                field.Item1.ToMsgPack(data);
            }
        }

    }

    public class PreDefinedField : ICompositeField
    {
        public override IField DeepCopy()
        {
            var copyField = new PreDefinedField();
            foreach (var field in fields)
            {
                copyField.fields.Add(new Tuple<IField, int>(field.Item1.DeepCopy(), field.Item2));
            }

            return copyField;
        }
    }

    public class UserDefinedField : ICompositeField
    {
        public override IField DeepCopy()
        {
            var copyField = new UserDefinedField();
            foreach (var field in fields)
            {
                copyField.fields.Add(new Tuple<IField, int>(field.Item1.DeepCopy(), field.Item2));
            }
             
            return copyField;
        }
    }

    public class AoiDefinitionField : UserDefinedField
    {
        private readonly AOIDataType _aoiDataType;
        public AoiDefinitionField(AOIDataType aoiDataType)
        {
            _aoiDataType = aoiDataType;
            PropertyChangedEventManager.AddHandler(_aoiDataType, Aoi_PropertyChanged, "CopyAllValueToTag");
        }

        private void Aoi_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_aoiDataType.CopyAllValueToTag)
            {
                Reset();
            }

        }

        private void Reset()
        {
            var res= _aoiDataType.Create(null) as UserDefinedField;
            
            ResetData(this,res);
            
        }

        private void ResetData(IField data, IField defaultData)
        {
            var boolField = data as BoolField;
            if (boolField != null)
            {
                boolField.value = ((BoolField) defaultData).value;
                return;
            }

            var sintField = data as Int8Field;
            if (sintField != null)
            {
                sintField.value = ((Int8Field) defaultData).value;
                return;
            }

            var usintField = data as UInt8Field;
            if (usintField != null)
            {
                usintField.value = ((UInt8Field) defaultData).value;
                return;
            }

            var int16Field = data as Int16Field;
            if (int16Field != null)
            {
                int16Field.value = ((Int16Field) defaultData).value;
                return;
            }

            var uint16Field = data as UInt16Field;
            if (uint16Field != null)
            {
                uint16Field.value = ((UInt16Field) defaultData).value;
                return;
            }

            var int32Field = data as Int32Field;
            if (int32Field != null)
            {
                int32Field.value = ((Int32Field) defaultData).value;
                return;
            }

            var uint32Field = data as UInt32Field;
            if (uint32Field != null)
            {
                uint32Field.value = ((UInt32Field) defaultData).value;
                return;
            }

            var int64Field = data as Int64Field;
            if (int64Field != null)
            {
                int64Field.value = ((Int64Field) defaultData).value;
                return;
            }

            var uint64Field = data as UInt64Field;
            if (uint64Field != null)
            {
                uint64Field.value = ((UInt64Field) defaultData).value;
                return;
            }

            var boolArrayField = data as BoolArrayField;
            if (boolArrayField != null)
            {
                var defaultField = ((BoolArrayField) defaultData);
                for (int i = 0; i < boolArrayField.BitCount; i++)
                {
                    boolArrayField.Set(i,defaultField.Get(i));
                }
                return;
            }

            var arrayField = data as ArrayField;
            if (arrayField != null)
            {
                var defaultField = ((ArrayField)defaultData);
                for (int i = 0; i < arrayField.fields.Count; i++)
                {
                    ResetData(arrayField.fields[i].Item1,defaultField.fields[i].Item1);
                }
                return;
            }

            var compositeField = data as ICompositeField;
            if (compositeField != null)
            {
                var defaultField = ((ICompositeField)defaultData);
                for (int i = 0; i < compositeField.fields.Count; i++)
                {
                    ResetData(compositeField.fields[i].Item1, defaultField.fields[i].Item1);
                }
                return;
            }
            
            Debug.Assert(false,data.GetType().Name);

        }

        public override IField DeepCopy()
        {
            var copyField = new AoiDefinitionField(_aoiDataType);
            foreach (var field in fields)
            {
                copyField.fields.Add(new Tuple<IField, int>(field.Item1.DeepCopy(), field.Item2));
            }

            return copyField;
        }

    }
}
