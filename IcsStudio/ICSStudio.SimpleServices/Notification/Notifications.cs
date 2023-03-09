using GalaSoft.MvvmLight.Messaging;
using ICSStudio.Interfaces.Notification;

namespace ICSStudio.SimpleServices.Notification
{
    public static class Notifications
    {
        private static string PublishToken = "icon-global";

        static Notifications()
        {
        }

        private static readonly Messenger Messenger = new Messenger();


        #region Notification

        public static void RegisterNotification<T>(NotificationListener<T> listener)
            where T : NotificationData
        {
            Messenger.Register<T>(listener, listener.OnNotification);
        }

        public static void UnRegisterNotification<T>(NotificationListener<T> listener)
            where T : NotificationData
        {
            Messenger.Unregister<T>(listener, listener.OnNotification);
        }

        public static void SendNotificationData<T>(T data)
            where T : NotificationData
        {
            Messenger.Send(data);
        }

        #endregion

        
        #region Message

        public static void Publish(MessageData message)
        {
            Messenger.Send(message, PublishToken);
        }

        public static void ConnectConsumer(IConsumer consumer)
        {
            Messenger.Register<MessageData>(consumer, PublishToken, consumer.Consume);
        }

        public static void DisconnectConsumer(IConsumer consumer)
        {
            Messenger.Unregister<MessageData>(consumer, PublishToken, consumer.Consume);
        }

        #endregion
    }
}
