using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Tags;

namespace ICSStudio.EditorPackage.MonitorEditTags.Models
{
    internal partial class MonitorTagItem
    {
        public static MonitorTagItem TagToMonitorTagItem(ITag t, IDataServer dataServer)
        {
            var tag = t as Tag;
            if (tag == null) return null;

            MonitorTagItem item = CreateMonitorTagItem(
                dataServer,
                tag, tag.Name, tag.DataTypeInfo, tag.DataWrapper.Data,
                null, 0,
                tag.Description,
                tag.DisplayStyle);

            return item;
        }

        private static MonitorTagItem CreateMonitorTagItem(
            IDataServer dataServer,
            ITag t, string name, DataTypeInfo dataTypeInfo, IField dataField,
            MonitorTagItem parentItem, int memberIndex,
            // ReSharper disable once UnusedParameter.Local
            string description, DisplayStyle style)
        {
            var tag = t as Tag;
            if (tag == null) return null;

            MonitorTagItem monitorTagItem = new MonitorTagItem
            {
                CanSetDescription = false,
                DataServer = dataServer,
                Name = name,
                Usage = tag.Usage,
                DataType = dataTypeInfo.ToString(),
                Description = Tag.GetChildDescription(tag.Description, tag.DataTypeInfo, tag.ChildDescription, Tag.GetOperand(name)),
                DisplayStyle = style,

                Tag = tag,
                ParentItem = parentItem,
                MemberIndex = memberIndex,

                DataTypeInfo = dataTypeInfo,
                DataField = dataField
            };
       
            monitorTagItem.CanSetDescription = true;

            if (dataTypeInfo.Dim1 == 0)
            {
                var dataType = dataTypeInfo.DataType;

                if (dataType.IsBool)
                {
                    monitorTagItem.DataType = "BOOL";

                    if (parentItem != null)
                    {
                        if (parentItem.ItemType == TagItemType.Array)
                            monitorTagItem.ItemType = TagItemType.BoolArrayMember;

                        if (parentItem.ItemType == TagItemType.Integer)
                            monitorTagItem.ItemType = TagItemType.IntegerMember;
                    }

                }

                if (dataType.IsInteger)
                {
                    monitorTagItem.ItemType = TagItemType.Integer;
                }

                if (dataType.IsStruct)
                {
                    monitorTagItem.ItemType = TagItemType.Struct;
                }

                if (dataType is UserDefinedDataType || dataType is ModuleDefinedDataType || dataType is AOIDataType)
                {
                    monitorTagItem.ItemType = TagItemType.Struct;
                }

                if (dataType.IsStringType)
                {
                    monitorTagItem.ItemType = TagItemType.String;
                }

            }
            else
            {
                monitorTagItem.ItemType = TagItemType.Array;
            }


            if (dataServer != null)
            {
                monitorTagItem.DataOperand = dataServer.CreateDataOperand(tag.ParentCollection, name);
                monitorTagItem.DataOperand?.StartMonitoring(true, false);
            }

            return monitorTagItem;
        }



        protected override List<TagItem> CreateChildren()
        {
            if (ItemType == TagItemType.Integer)
            {
                return CreateIntegerChildren();
            }

            if (ItemType == TagItemType.Array)
            {
                return CreateArrayChildren();
            }

            if (ItemType == TagItemType.Struct || ItemType == TagItemType.String)
            {
                return CreateStructChildren();
            }

            throw new NotSupportedException();
        }

        private List<TagItem> CreateStructChildren()
        {
            IDataType dataType = DataTypeInfo.DataType;

            var compositeField = DataField as ICompositeField;
            var compositiveType = dataType as CompositiveType;

            Contract.Assert(compositeField != null);
            Contract.Assert(compositiveType != null);

            List<TagItem> children = new List<TagItem>();

            int index = 0;
            foreach (var member in compositiveType.TypeMembers)
            {
                if (member.IsHidden)
                    continue;

                var dataTypeInfo = member.DataTypeInfo;

                var dataTypeMember = member as DataTypeMember;
                Contract.Assert(dataTypeMember != null);

                IField memberDataField = compositeField.fields[dataTypeMember.FieldIndex].Item1;

                var item = CreateMonitorTagItem(
                    DataServer,
                    Tag,
                    $"{Name}.{member.Name}",
                    dataTypeInfo,
                    memberDataField,
                    this, index,
                    Tag.GetChildDescription(Tag.Description, Tag.DataTypeInfo, Tag.ChildDescription, Tag.GetOperand($"{Name}.{member.Name}")),
                    member.DisplayStyle);
               
                //TODO(gjc): special for udt bool array
                if (dataTypeMember.IsBit && dataTypeInfo.Dim1 == 0)
                {
                    item.BitOffset = dataTypeMember.BitOffset;
                    item.ItemType = TagItemType.BitMember;
                }

                children.Add(item);
                index++;
            }

            return children;
        }

        private List<TagItem> CreateArrayChildren()
        {
            List<TagItem> children = new List<TagItem>();

            var isBoolArray = DataTypeInfo.DataType.IsBool;
            if (isBoolArray)
                Contract.Assert(DataField is BoolArrayField);
            else
                Contract.Assert(DataField is ArrayField);

            var dim3 = Math.Max(DataTypeInfo.Dim3, 1);
            var dim2 = Math.Max(DataTypeInfo.Dim2, 1);
            var dim1 = DataTypeInfo.Dim1;
            Contract.Assert(dim1 > 0);

            int index = 0;
            DataTypeInfo dataTypeInfo = new DataTypeInfo
            {
                DataType = DataTypeInfo.DataType
            };

            var arrayField = DataField as ArrayField;

            for (var z = 0; z < dim3; z++)
            for (var y = 0; y < dim2; y++)
            for (var x = 0; x < dim1; x++)
            {
                string itemName;
                if (DataTypeInfo.Dim3 > 0)
                    itemName = $"{Name}[{z},{y},{x}]";
                else if (DataTypeInfo.Dim2 > 0)
                    itemName = $"{Name}[{y},{x}]";
                else
                    itemName = $"{Name}[{x}]";

                IField dataField = DataField;
                if (!isBoolArray)
                    dataField = arrayField.fields[index].Item1;

                MonitorTagItem item = CreateMonitorTagItem(
                    DataServer,
                    Tag, itemName, dataTypeInfo,
                    dataField,
                    this, index,
                    Tag.GetChildDescription(Tag.Description, Tag.DataTypeInfo, Tag.ChildDescription, Tag.GetOperand(itemName)), DisplayStyle);
              
                children.Add(item);

                index++;
            }

            return children;
        }

        private List<TagItem> CreateIntegerChildren()
        {
            Contract.Assert(DataField is IFieldAtomic);

            List<TagItem> children = new List<TagItem>();

            DataTypeInfo dataTypeInfo = new DataTypeInfo
            {
                DataType = Tag.ParentController.DataTypes["BOOL"]
            };

            int bitSize = DataTypeInfo.DataType.BitSize;

            for (var i = 0; i < bitSize; i++)
            {
                var item = CreateMonitorTagItem(
                    DataServer,
                    Tag, $"{Name}.{i}",
                    dataTypeInfo, DataField,
                    this, 0,
                    Tag.GetChildDescription(Tag.Description, Tag.DataTypeInfo, Tag.ChildDescription, Tag.GetOperand($"{Name}.{i}")), DisplayStyle.Decimal);
                
                item.BitOffset = i;
                children.Add(item);
            }

            return children;
        }
    }
}
