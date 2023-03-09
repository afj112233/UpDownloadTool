using System;
using System.Collections.Generic;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.Interfaces.Tags
{
    public class TagModifiedEventArgs : EventArgs
    {
        public TagModifiedEventArgs(int tagUid)
        {
            TagUid = tagUid;
        }

        public int TagUid { get; }
    }

    public interface ITagCollection : IBaseComponentCollection<ITag>
    {
        bool IsControllerScoped { get; }

        IProgramModule ParentProgram { get; }

        event EventHandler<TagModifiedEventArgs> OnTagModified;

        ITag GetDeviceTag(string name, bool deep);

        ITag TryGetDeviceTag(string name, bool deep);

        TagInfo GetTagInfo(string tagName, bool bAllowPrivateMemberReferences = false);

        void DeleteTag(ITag tag, bool allowDeleteIfReferenced, bool allowDeleteIfReadOnly,bool resetAoi);

        IEnumerable<ITag> TrackedTags { get; }
    }
}
