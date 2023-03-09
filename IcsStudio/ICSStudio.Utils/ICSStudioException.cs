using System;
using System.Runtime.Serialization;

namespace ICSStudio.Utils

{
    public class ICSStudioException : Exception
    {
        public ICSStudioException() : base()
        {
        }

        public ICSStudioException(string s) : base(s)
        {

        }

        public ICSStudioException(string s, Exception e) : base(s, e)
        {

        }

        protected ICSStudioException(SerializationInfo info, StreamingContext ctx) : base(info, ctx)
        {

        }
    }
}
