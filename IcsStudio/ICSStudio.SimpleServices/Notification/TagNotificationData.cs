using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;

namespace ICSStudio.SimpleServices.Notification
{
    public class TagNotificationData : NotificationData
    {
        public NotificationType Type { get; set; }

        public ITag Tag { get; set; }

        public IField Field { get; set; }

        public bool IsBit { get; set; }
        public int BitOffset { get; set; }

        public string Attribute { get; set; }

        public enum NotificationType
        {
            Value,
            Attribute
        }
    }
}
