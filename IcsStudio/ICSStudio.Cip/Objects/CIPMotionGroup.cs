using System;
using ICSStudio.Cip.DataTypes;
using ICSStudio.Cip.EtherNetIP;

namespace ICSStudio.Cip.Objects
{
    public class CIPMotionGroup : CipBaseObject
    {
        public CIPMotionGroup(ushort instanceId, ICipMessager messager)
            : base((ushort) CipObjectClassCode.MotionGroup, instanceId, messager)
        {
        }
    }
}
