using System;
using System.Diagnostics;

namespace ICSStudio.Cip.Objects
{
    // status information about the motion instruction
    public class CipMotionInstruction
    {
        public ushort CommandCode { get; private set; }
        public uint Flags { get; private set; }
        public ushort ErrorCode { get; private set; }
        public byte MessageStatus { get; private set; }
        public byte ExecutionState { get; private set; }
        public uint Segment { get; private set; }
        public uint ExtendedErrorCode { get; private set; }
        
        public int Parse(byte[] data)
        {
            try
            {
                CommandCode = BitConverter.ToUInt16(data, 0);
                Flags = BitConverter.ToUInt32(data, 2);
                ErrorCode = BitConverter.ToUInt16(data, 6);
                MessageStatus = data[8];
                ExecutionState = data[9];
                Segment = BitConverter.ToUInt32(data, 10);
                ExtendedErrorCode = BitConverter.ToUInt32(data, 14);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return -1;
            }

            return 0;
        }

        public string Info()
        {
            string info =
                $"Command:{CommandCode},EN:{EN},DN:{DN},ER:{ER},PC:{PC},IP:{IP},AC:{AC},DECEL:{DECEL},ACCEL:{ACCEL},ErrorCode:{ErrorCode},MessageStatus:{MessageStatus},ExecutionState{ExecutionState}";

            return info;
        }

        #region FLAGS

        // ReSharper disable once InconsistentNaming
        public bool EN => (Flags & (1 << 31)) != 0;
        // ReSharper disable once InconsistentNaming
        public bool DN => (Flags & (1 << 29)) != 0;
        // ReSharper disable once InconsistentNaming
        public bool ER => (Flags & (1 << 28)) != 0;
        // ReSharper disable once InconsistentNaming
        public bool PC => (Flags & (1 << 27)) != 0;
        // ReSharper disable once InconsistentNaming
        public bool IP => (Flags & (1 << 26)) != 0;
        // ReSharper disable once InconsistentNaming
        public bool AC => (Flags & (1 << 23)) != 0;
        // ReSharper disable once InconsistentNaming
        public bool DECEL => (Flags & (1 << 1)) != 0;
        // ReSharper disable once InconsistentNaming
        public bool ACCEL => (Flags & (1 << 0)) != 0;

        #endregion
    }
}
