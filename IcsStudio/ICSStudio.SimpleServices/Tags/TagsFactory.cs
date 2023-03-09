using System;
using ICSStudio.SimpleServices.DataWrapper;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;

namespace ICSStudio.SimpleServices.Tags
{
    public class TagsFactory
    {
        public static Tag CreateTag(
            TagCollection parentCollection,
            string name, string typeName,
            int dim1, int dim2, int dim3)
        {
            if (dim1 == 0) Debug.Assert(dim2 == 0 && dim3 == 0);
            if (dim2 == 0) Debug.Assert(dim3 == 0);

            Tag tag = new Tag(parentCollection) {Name = name};

            var dataType = parentCollection.ParentController.DataTypes[typeName];

            tag.DataWrapper = CreateDataWrapper(parentCollection.ParentController, dataType, dim1, dim2, dim3);

            return tag;
        }

        public static Tag CreateTag(TagCollection parentCollection, JToken json)
        {

            Tag tag = new Tag(parentCollection) {Name = (string) json["Name"]};

            tag.Override((JObject) json);

            return tag;
        }

        public static Tag CreateAliasTag(
            TagCollection parentCollection,
            string name,
            string aliasSpecifier,
            DisplayStyle displayStyle)
        {
            Tag tag = new Tag(parentCollection)
            {
                Name = name,
                TagType = TagType.Alias,
                AliasSpecifier = aliasSpecifier,
                IsVerified = false,
                DisplayStyle = displayStyle
            };

            return tag;
        }

        public static DataWrapper.DataWrapper CreateDataWrapper(
            IController controller, IDataType dataType,
            int dim1, int dim2, int dim3)
        {
            if (dataType.Name.Equals("MOTION_GROUP", StringComparison.OrdinalIgnoreCase))
                return MotionGroup.Create(dataType);

            if (dataType.Name.Equals("AXIS_CIP_DRIVE", StringComparison.OrdinalIgnoreCase))
                return AxisCIPDrive.Create(dataType, controller);

            if (dataType.Name.Equals("AXIS_VIRTUAL", StringComparison.OrdinalIgnoreCase))
                return AxisVirtual.Create(dataType, controller);

            if (dataType.Name.Equals("MESSAGE", StringComparison.OrdinalIgnoreCase))
                return MessageDataWrapper.Create(dataType, controller);

            return new DataWrapper.DataWrapper(dataType, dim1, dim2, dim3, null);
        }
    }
}
