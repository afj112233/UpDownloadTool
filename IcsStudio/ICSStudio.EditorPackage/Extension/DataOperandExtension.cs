using System;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.DataType;

namespace ICSStudio.EditorPackage.Extension
{
    public static class DataOperandExtension
    {
        public static bool ValidateValue(this IDataOperand operand, string value)
        {
            var field = operand.Field;

            if (operand.IsBool)
            {
                return value == "1" || value == "0";
            }
            if (field is UInt8Field)
            {
                sbyte dest;
                return sbyte.TryParse(value, out dest);
            }
            if (field is UInt16Field)
            {
                ushort dest;
                return ushort.TryParse(value, out dest);
            }
            if (field is UInt32Field)
            {
                uint dest;
                return uint.TryParse(value, out dest);
            }
            if (field is UInt64Field)
            {
                ulong dest;
                return ulong.TryParse(value, out dest);
            }
            if (field is Int8Field)
            {
                byte dest;
                return byte.TryParse(value, out dest);
            }
            if (field is Int16Field)
            {
                short dest;
                return short.TryParse(value, out dest);
            }
            if (field is Int32Field)
            {
                int dest;
                return int.TryParse(value, out dest);
            }
            if (field is Int64Field)
            {
                long dest;
                return long.TryParse(value, out dest);
            }

            if (field is RealField)
            {
                float dest;
                return float.TryParse(value, out dest);
            }

            if (operand.IsString)
            {
                try
                {
                    var bytes = ValueConverter.ToBytes(value);

                    var cfield = field as ICompositeField;
                    if (cfield == null)
                        return false;

                    if (cfield.fields.Count == 2)
                    {
                        var lenField = (Int32Field)cfield.fields[0].Item1;
                        var arrayField = (ArrayField)cfield.fields[1].Item1;

                        if (lenField != null && arrayField != null)
                        {
                            int maxCount = arrayField.Size();

                            if (bytes.Count >= 0 && bytes.Count <= maxCount)
                                return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message + ex.StackTrace);
                    return false;
                }
            }

            return false;
        }
    }
}
