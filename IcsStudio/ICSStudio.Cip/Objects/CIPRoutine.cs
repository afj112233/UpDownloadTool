using ICSStudio.Cip.DataTypes;
using ICSStudio.Cip.EtherNetIP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSStudio.Cip.Objects
{
    public enum ServiceCode
    {
        GetCodeText = 0x4B,
        GetCodeBin = 0x4C,
        GetPool = 0x4D
    };

    public class CIPRoutine : CIPRoutineStub
    {
        public CIPRoutine(ushort instanceId, ICipMessager messager) : 
            base(instanceId, messager)
        {
        }
    }
}
