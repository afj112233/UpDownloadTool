using Newtonsoft.Json.Linq;

namespace ICSStudio.DownloadChange
{
    internal class DiffItem : IDiffItem
    {
        public DiffItem(ItemChangeType changeType, JToken oldValue, JToken newValue)
        {
            ChangeType = changeType;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public ItemChangeType ChangeType { get; }
        public JToken OldValue { get; }
        public JToken NewValue { get; }

    }
}
