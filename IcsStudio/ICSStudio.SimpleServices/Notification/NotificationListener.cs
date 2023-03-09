
using System.Threading.Tasks;

namespace ICSStudio.SimpleServices.Notification
{
    public class NotificationListener<T>
        where T : NotificationData
    {
        private readonly INotifiable<T> _target;
        private readonly ListenType _listenType;

        public NotificationListener(INotifiable<T> target) : this(target, ListenType.Sync)
        {
            _target = target;
        }

        public NotificationListener(INotifiable<T> target, ListenType listenType)
        {
            _target = target;
            _listenType = listenType;
        }

        public bool IsAsync => _listenType == ListenType.Async;

        public void OnNotification(T notificationData)
        {
            if (IsAsync)
            {
                Task.Run(() =>
                {
                    _target?.OnNotification(notificationData);
                });
            }
            else
            {
                _target?.OnNotification(notificationData);
            }

        }

        public enum ListenType
        {
            Sync,
            Async,
        }
    }
}
