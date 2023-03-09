using Newtonsoft.Json.Linq;

namespace ICSStudio.DownloadChange
{
    public class CompareTuple
    {
        public JToken OldValue { get; set; }
        public JToken NewValue { get; set; }
    }
}
