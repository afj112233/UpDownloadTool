namespace ICSStudio.SimpleServices.Notification
{
    public interface INotifiable<in T> 
        where T: NotificationData
    {
        void OnNotification(T notificationData);
    }
}
