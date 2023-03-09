using System;

namespace ICSStudio.Cip.Other
{
    public class MotionGroupDescriptor
    {
        public string GetGroupStatus(string status)
        {
            //TODO(gjc): edit later

            UInt32 groupStatus;
            if (UInt32.TryParse(status, out groupStatus))
            {
                if (groupStatus == 0)
                    return "Not Synchronized";

                GroupStatusBitmap result = (GroupStatusBitmap) (groupStatus & 0x7);

                return result.ToString();
            }

            return "Unknown Status";
        }

        public string GetGroupFault(string fault)
        {
            //TODO(gjc): edit later

            UInt32 groupFault;
            if (UInt32.TryParse(fault, out groupFault))
            {
                if (groupFault == 0)
                    return "No Faults";

                GroupFaultBitmap result = (GroupFaultBitmap) (groupFault & 0x7);

                return result.ToString();
            }

            return "Unknown Fault";
        }

        public string GetAxisFault(string fault)
        {
            //TODO(gjc): edit later

            UInt32 axisFault;
            if (UInt32.TryParse(fault, out axisFault))
            {
                if (axisFault == 0)
                    return "No Faults";

                AxisFaultBitmap result = (AxisFaultBitmap) (axisFault & 0x7);

                return result.ToString();
            }

            return "Unknown Fault";
        }

        // ReSharper disable UnusedMember.Local
        [Flags]
        private enum GroupStatusBitmap : UInt32
        {
            InhibitStatus = 1 << 0,
            Synchronized = 1 << 1, // GroupSynced
            AxisInhibitStatus = 1 << 2
        }

        [Flags]
        private enum GroupFaultBitmap : UInt32
        {

            GroupOverlapFault = 1 << 0,
            CSTLossFault = 1 << 1,
            GroupTaskLoadingFault = 1 << 2,
        }

        [Flags]
        private enum AxisFaultBitmap : UInt32
        {
            PhysicalAxisFault = 1 << 0,
            ModuleFault = 1 << 1,
            ConfigFault = 1 << 2,
        }
        // ReSharper restore UnusedMember.Local


    }
}
