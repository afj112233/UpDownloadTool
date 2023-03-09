using System.Collections.Generic;
using ICSStudio.Cip.DataTypes;

namespace ICSStudio.Cip.EtherNetIP
{
    public class MessageRouterRequest : IMessageRouterRequest
    {
        public MessageRouterRequest()
        {
            Service = 0;
            RequestPath = null;
            RequestData = null;
        }

        public byte Service { get; set; }
        public PaddedEPath RequestPath { get; set; }
        public byte[] RequestData { get; set; }

        public byte[] ToByteArray()
        {
            var byteList = new List<byte>(512) {Service};


            byteList.AddRange(RequestPath.ToByteArray());

            if (RequestData != null)
                byteList.AddRange(RequestData);

            return byteList.ToArray();
        }
    }
}