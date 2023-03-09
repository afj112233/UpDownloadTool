using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Instruction.Instructions;

namespace ICSStudio.SimpleServices.DataType
{
    public class ArrayType : DataType
    {
        public readonly IDataType Type;
        public readonly int Dim1;
        public readonly int Dim2;
        public readonly int Dim3;

        public ArrayType(IDataType type, int dim1, int dim2 = 0, int dim3 = 0)
        {
            this.Type = type;
            this.Dim1 = dim1;
            this.Dim2 = dim2;
            this.Dim3 = dim3;

        }

        public override int ByteSize => Type.ByteSize * Math.Max(1, this.Dim1) * Math.Max(1, this.Dim2) * Math.Max(1, this.Dim3);
        public override int BaseDataSize => Type.ByteSize;

        public static IDataType InfoToType(DataTypeInfo info)
        {
            if (info.Dim1 == 0 && info.Dim2 == 0 && info.Dim3 == 0)
            {
                return info.DataType;
            }
            return new ArrayType(info.DataType, info.Dim1, info.Dim2, info.Dim3);
        }

        public override string ToString()
        {
            if (Type == null)
                return string.Empty;

            if (Dim3 > 0)
                return $"{Type.Name}[{Dim3},{Dim2},{Dim1}]";
            if (Dim2 > 0)
                return $"{Type.Name}[{Dim2},{Dim1}]";
            if (Dim1 > 0)
                return $"{Type.Name}[{Dim1}]";

            return Type.Name;
        }
    }


}
