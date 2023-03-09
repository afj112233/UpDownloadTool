namespace ICSStudio.Interfaces.Notification
{
    public class MessageData
    {
        public MessageType Type { get; set; }

        public object Object { set; get; }

        public enum MessageType
        {
            //Event
            PullFinished,
            Restored,

            //Command

            TotalCount,

            Verify,
            AddTag,
            DelTag,
            MonitorTag,
            DelModule,

            //Ladder Graph Online Edit
            UpdateLadderGraph
        }

        public bool IsForce { set; get; }
    }

    public interface IConsumer
    {
        void Consume(MessageData message);
    }
}
