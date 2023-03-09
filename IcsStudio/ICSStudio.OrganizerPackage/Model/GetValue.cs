using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using ICSStudio.EditorPackage.MonitorEditTags.Models;
using ICSStudio.EditorPackage.MonitorEditTags.UI;
using ICSStudio.Gui.Utils;
using ICSStudio.SimpleServices.DataType;

namespace ICSStudio.OrganizerPackage.Model
{
    public class GetValue
    {
        public object Value { get; set; }
        public GetValue(TagItem tagItem)
        {
            if (tagItem.Usage == Interfaces.Tags.Usage.InOut)
            {
                Value = string.Empty;
                return;
            }

            if (tagItem.ItemType == TagItemType.Array ||
                tagItem.ItemType == TagItemType.Struct)
            {
                Value = new StructOrArrayValue();
                return;
            }

            if (tagItem.ItemType == TagItemType.String)
            {
                // string and udt-string
                var compositeField = (ICompositeField)tagItem.DataField;
                if (compositeField != null)
                {
                    Value = compositeField.ToString(tagItem.DisplayStyle);
                    return;
                }
            }

            // bool array
            if (tagItem.ItemType == TagItemType.BoolArrayMember)
            {
                var boolArrayField = (BoolArrayField)tagItem.DataField;
                Value = boolArrayField.Get(tagItem.MemberIndex);
                return;
            }

            if (tagItem.ItemType == TagItemType.IntegerMember
                || tagItem.ItemType == TagItemType.BitMember)
            {
                int bitOffset = tagItem.BitOffset;

                var boolField = tagItem.DataField as BoolField;
                if (boolField != null)
                {
                    Contract.Assert(bitOffset == 0);

                    Value = boolField.value != 0;
                    return;
                }

                var int8Field = tagItem.DataField as Int8Field;
                if (int8Field != null)
                {
                    Value = BitValue.Get(int8Field.value, bitOffset);
                    return;
                }

                var int16Field = tagItem.DataField as Int16Field;
                if (int16Field != null)
                {
                    Value = BitValue.Get(int16Field.value, bitOffset);
                    return;
                }

                var int32Field = tagItem.DataField as Int32Field;
                if (int32Field != null)
                {
                    Value = BitValue.Get(int32Field.value, bitOffset);
                    return;
                }

                var int64Field = tagItem.DataField as Int64Field;
                if (int64Field != null)
                {
                    Value = BitValue.Get(int64Field.value, bitOffset);
                    return;
                }
            }

            if (tagItem.ItemType == TagItemType.Default || tagItem.ItemType == TagItemType.Integer)
            {
                var boolField = tagItem.DataField as BoolField;
                if (boolField != null)
                {
                    Value = boolField.value != 0;
                    return;
                }

                var int8Field = tagItem.DataField as Int8Field;
                if (int8Field != null)
                {
                    Value = int8Field.value;
                    return;
                }

                var int16Field = tagItem.DataField as Int16Field;
                if (int16Field != null)
                {
                    Value = int16Field.value;
                    return;
                }

                var int32Field = tagItem.DataField as Int32Field;
                if (int32Field != null)
                {
                    Value = int32Field.value;
                    return;
                }

                var int64Field = tagItem.DataField as Int64Field;
                if (int64Field != null)
                {
                    Value = int64Field.value;
                    return;
                }

                var realField = tagItem.DataField as RealField;
                if (realField != null)
                {
                    Value = realField.value;
                    return;
                }
            }

            Value = "TODO";
        }
    }
}
