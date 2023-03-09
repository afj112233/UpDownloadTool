using ICSStudio.Cip.DataTypes;
using ICSStudio.Cip.EtherNetIP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSStudio.Cip.Objects
{
    public class CIPUDType : CIPUDTypeStub
    {
        public CIPUDType(int instanceId, ICipMessager messager) : 
            base(instanceId, messager)
        {
        }
    }
}
