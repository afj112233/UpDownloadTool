using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using ICSStudio.Interfaces.DataType;

namespace ICSStudio.SimpleServices.DataType
{
    public static class StringFieldExtensions
    {
        public static string ToString(this ICompositeField value, DisplayStyle displayStyle)
        {
            // for string and udt-string
            Contract.Assert(value.fields.Count == 2);

            var lenField = (Int32Field) value.fields[0].Item1;
            var arrayField = (ArrayField) value.fields[1].Item1;
            Contract.Assert(lenField != null);
            Contract.Assert(arrayField != null);

            int maxCount = arrayField.Size();
            Contract.Assert(maxCount > 0);

            var length = lenField.value;
            if (length < 0 || length > maxCount)
                length = maxCount;

            var result = new StringBuilder();

            result.Append("'");
            for (var i = 0; i < length; i++)
            {
                var int8Field = (Int8Field) arrayField.fields[i].Item1;

                var byteValue = (byte) int8Field.value;
                result.Append(byteValue.ToAsciiDisplay());
            }

            result.Append("'");

            return result.ToString();
        }

        public static bool EqualsByteList(this ICompositeField stringField, List<byte> byteList)
        {
            Contract.Assert(stringField.fields.Count == 2);
            var lengthField = (Int32Field) stringField.fields[0].Item1;
            var arrayField = (ArrayField) stringField.fields[1].Item1;
            Contract.Assert(lengthField != null);
            Contract.Assert(arrayField != null);

            int maxCount = arrayField.Size();
            Contract.Assert(byteList.Count <= maxCount);

            if (lengthField.value != byteList.Count)
                return false;

            for (int i = 0; i < byteList.Count; i++)
            {
                var int8Field = (Int8Field) arrayField.fields[i].Item1;

                if (int8Field.value != (sbyte) byteList[i])
                    return false;
            }

            return true;
        }

        public static void Set(this ICompositeField stringField, List<byte> byteList)
        {
            Contract.Assert(stringField.fields.Count == 2);
            var lengthField = (Int32Field) stringField.fields[0].Item1;
            var arrayField = (ArrayField) stringField.fields[1].Item1;
            Contract.Assert(lengthField != null);
            Contract.Assert(arrayField != null);

            int maxCount = arrayField.Size();
            Contract.Assert(byteList.Count <= maxCount);

            lengthField.value = byteList.Count;

            int i;
            for (i = 0; i < byteList.Count; i++)
            {
                var int8Field = (Int8Field) arrayField.fields[i].Item1;
                int8Field.value = (sbyte) byteList[i];
            }

            for (; i < maxCount; i++)
            {
                var int8Field = (Int8Field) arrayField.fields[i].Item1;
                int8Field.value = 0;
            }
        }

        public static List<byte> Get(this ICompositeField stringField)
        {
            Contract.Assert(stringField.fields.Count == 2);
            var lengthField = (Int32Field) stringField.fields[0].Item1;
            var arrayField = (ArrayField) stringField.fields[1].Item1;
            Contract.Assert(lengthField != null);
            Contract.Assert(arrayField != null);

            List<byte> result = new List<byte>();
            
            int maxCount = arrayField.Size();
            var length = lengthField.value;

            if (length < 0 || length > maxCount)
                length = maxCount;

            for (int i = 0; i < length; i++)
            {
                var int8Field = (Int8Field) arrayField.fields[i].Item1;
                result.Add((byte) int8Field.value);
            }

            return result;
        }
    }
}
