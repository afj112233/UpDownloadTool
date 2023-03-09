using ICSStudio.Interfaces.DataType;

namespace ICSStudio.Interfaces.Tags
{
    public class TagInfo
    {
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public ITag BaseTag { get; }

        public ITag ImmediateTag { get; set; }

        public IDataType DataType { get; set; }

        public int BitOffset { get; set; }

        public int Dimension1 { get; set; }

        public int Dimension2 { get; set; }

        public int Dimension3 { get; set; }

        public DisplayStyle DisplayStyle { get; set; }

        public int WildcardOffset { get; set; }

        public override int GetHashCode()
        {
            if (BaseTag != null)
                return BaseTag.Uid;
            // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if ((object) (obj as TagInfo) == null)
                return false;
            return Equals((TagInfo) obj);
        }

        public bool Equals(TagInfo other)
        {
            return !(other == null) && BaseTag == other.BaseTag &&
                   (ImmediateTag == other.ImmediateTag && BitOffset == other.BitOffset) &&
                   (Dimension1 == other.Dimension1 && Dimension2 == other.Dimension2 &&
                    (Dimension3 == other.Dimension3 && DisplayStyle == other.DisplayStyle)) &&
                   WildcardOffset == other.WildcardOffset;
        }

        public static bool operator ==(TagInfo tagInfo1, TagInfo tagInfo2)
        {
            if (ReferenceEquals(tagInfo1, tagInfo2))
                return true;

            if (ReferenceEquals(null, tagInfo1))
                return false;

            return tagInfo1.Equals(tagInfo2);

        }

        public static bool operator !=(TagInfo tagInfo1, TagInfo tagInfo2)
        {
            return !(tagInfo1 == tagInfo2);
        }
    }
}
