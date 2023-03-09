using ICSStudio.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ICSStudio.Cip.EtherNetIP
{
    public class CIPGeneralStatusCodeException : ICSStudioException
    {
        public CIPGeneralStatusCodeException(CipGeneralStatusCode code)
        {
            _code = code;
        }

        public CIPGeneralStatusCodeException(CipGeneralStatusCode code, string s) : base("Code:" + code.ToString() +",Resp:" + s)
        {
            _code = code;
        }

        private CipGeneralStatusCode _code;
    }
}
